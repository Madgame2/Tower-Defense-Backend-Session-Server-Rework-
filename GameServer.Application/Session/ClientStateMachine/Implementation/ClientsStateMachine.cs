using GameServer.Application.Model;
using GameServer.Application.Session.ClientStateMachine.Factory.Interfaces;
using GameServer.Application.Session.ClientStateMachine.States.Base;
using GameServer.Application.Sessions;
using GameServer.Domain.Services.ClientStateMachine;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Application.Session.PlayersStateMachine.Implementation
{
    public class ClientsStateMachine : IClientStateMachine, IDisposable, IAsyncDisposable
    {
        private readonly IClientStateFactory _stateFactory;

        private Task? _clientMachineTask;

        private CancellationTokenSource? _cts;

        private ClientConnection? _clientConnection;
        private GameSession? _linkedSession;

        private ClientStateBase? _currentState;

        private bool _initialized;
        private bool _disposed;

        public ClientsStateMachine(
            IInitialClientState initialClientState,
            IClientStateFactory stateFactory)
        {
            _currentState = initialClientState as ClientStateBase
                ?? throw new InvalidOperationException(
                    "Initial state must inherit from ClientStateBase.");

            _stateFactory = stateFactory;
        }

        public void Init(ClientConnection clientConnection, GameSession session)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            if (_initialized)
                throw new InvalidOperationException(
                    "ClientStateMachine is already initialized.");

            _clientConnection = clientConnection;
            _linkedSession = session;

            _initialized = true;
        }

        public void Start(CancellationToken kestrelToken)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            if (!_initialized)
                throw new InvalidOperationException(
                    "ClientStateMachine must be initialized before Start.");

            if (_clientMachineTask != null)
                throw new InvalidOperationException(
                    "ClientStateMachine has already been started.");

            _cts = CancellationTokenSource.CreateLinkedTokenSource(
                kestrelToken);

            _clientMachineTask = StateMachineCycle(_cts.Token);
        }


        private async Task StateMachineCycle(
            CancellationToken cancellationToken)
        {
            while (_currentState != null)
            {
                cancellationToken.ThrowIfCancellationRequested();

                ClientStateBase currentState = _currentState;

                Type? nextStateType = null;

                try
                {
                    // Вход в состояние
                    await currentState.OnEnter(
                        _clientConnection!,
                        _linkedSession!,
                        cancellationToken);

                    // Ожидание разрешения перехода
                    await currentState.WaitForTransitionAsync(
                        _clientConnection!,
                        _linkedSession!,
                        cancellationToken);

                    cancellationToken.ThrowIfCancellationRequested();

                    // Определение следующего состояния
                    nextStateType = currentState.NextState;
                }
                finally
                {
                    // Выход из текущего состояния
                    await currentState.OnExit(
                        _clientConnection!,
                        _linkedSession!,
                        CancellationToken.None);
                }

                cancellationToken.ThrowIfCancellationRequested();

                // Завершение машины состояний
                if (nextStateType == null)
                {
                    _currentState = null;
                    break;
                }

                // Создание следующего состояния
                _currentState = _stateFactory.Create(nextStateType);
            }
        }

        public async Task StopAsync()
        {
            if (_cts == null)
                return;

            _cts.Cancel();

            if (_clientMachineTask != null)
            {
                try
                {
                    await _clientMachineTask;
                }
                catch (OperationCanceledException)
                    when (_cts.IsCancellationRequested)
                {
                }
            }
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
            _cts?.Cancel();
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposed)
                return;

            await StopAsync();

            _disposed = true;
            _cts?.Dispose();
        }
    }
}
