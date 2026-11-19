using GameServer.Domain.Core.Synchronization;
using GameServer.Domain.Sessions;
using GameServer.Domain.Sessions.Events;
using GameServer.Domain.Sessions.StateMachine;
using GameServer.Domain.SessionWorld;
using GameServer.Domain.SessionWorld.Generators;
using GameServer.Domain.SessionWorld.Interfaces;
using GameServer.GameLoop.Core.Simultaion.Interfaces;
using System.Threading.Channels;

namespace GameServer.Application.Sessions
{
    public class GameSession
    {
        public Guid Id { get; set; }
        public SessionDifficuty Difficulty { get; init; }
        public GameRoom CurrentRoom { 
            get =>_world;
            set
            {
                if (_world != null) return;
                _world = value;
            }
        }
        public ISimulation Simulation
        {
            get => _simulation;
            set
            {
                if (_simulation != null) return;
                _simulation = value;
            }
        }

        private HashSet<string> _playersResurvation = new();
        private HashSet<string> _onlinePlayers = new();
        private SessionStateMachine _sessionStatemachine;
        public ClientSyncBarrier SyncBarrier { get; } = new();
        private Task _eventLoop;
        
        private GameRoom _world;
        private ISimulation _simulation;

        public Action<Guid>? _terminateSession;

        public HashSet<string> OnlinePlayersIDs { get => _onlinePlayers; }
        public HashSet<string> ResurvatePlayersIDs { get => _playersResurvation; }

        private readonly Channel<SessionEvent> _events = Channel.CreateUnbounded<SessionEvent>();
        public SessionStateMachine StateMachine { get => _sessionStatemachine; }

        private readonly TaskCompletionSource<GameRoom> _roomReady =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        public void SetRoom(GameRoom room)
        {
            if (CurrentRoom != null)
                throw new InvalidOperationException("GameRoom already initialized.");

            CurrentRoom = room;
            _roomReady.TrySetResult(room);
        }

        public Task<GameRoom> WaitForRoomAsync(
            CancellationToken cancellationToken)
        {
            return _roomReady.Task.WaitAsync(cancellationToken);
        }

        public void Init(SessionStateMachine sm)
        {
            _sessionStatemachine = sm;
        }
        public bool hasPlayerReserwation(string playerId)
        {
            return _playersResurvation.Contains(playerId);
        }
        public void reservForPlayer(string playerId)
        {
            _playersResurvation.Add(playerId);
        }
        public async Task StartSession()
        {
            await _sessionStatemachine.Start();

            _eventLoop = Run();
        }

        public void RemovePlayer(string playerId)
        {
            _onlinePlayers.Remove(playerId);
            _sessionStatemachine.Publish(new PlayerDisconnected(playerId));

            if (_onlinePlayers.Count <= 0)
            {
                _terminateSession?.Invoke(Id);
                _terminateSession = null;
            }
        }

        private async Task Run()
        {
            while (true)
            {
                try
                {
                    while (_events.Reader.TryRead(out var evt))
                    {
                        ProcessEvent(evt);
                    }

                    await Task.Delay(16);
                }catch(Exception ex)
                {
                    //Console.WriteLine($"[ERROR]: {ex.Message}");
                }
            }
        }

        public void Enqueue(SessionEvent evt)
        {
            _events.Writer.TryWrite(evt);
        }

        private async Task ProcessEvent(SessionEvent evt)
        {
            switch (evt)
            {
                case PlayerConnected connected:

                    _onlinePlayers.Add(connected.UserId);

                    await _sessionStatemachine.Publish(evt);

                    break;

                case PlayerDisconnected disconnected:

                    _onlinePlayers.Remove(disconnected.UserId);

                    await _sessionStatemachine.Publish(evt);

                    break;
            }
        }
    }
}
