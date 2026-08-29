using GameServer.Domain.SessionWorld;
using GameServer.GameLoop.Core.Simultaion.Interfaces;
using GameServer.GameLoop.Core.Simultaion.Meta.Base;
using GameServer.GameLoop.Core.Simultaion.Steps.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace GameServer.GameLoop.Core.Simultaion
{
    public class Simulation : ISimulation
    {
        private GameRoom _simulatedWorld;
        private Task _simulationTask;

        private readonly List<ITickable> _systems;
        private readonly List<INetworkTickable> _networkSystems;

        private CancellationTokenSource _cts;

        public Simulation(GameRoom simulatedWorld,
            List<ITickable> Systems,
            List<INetworkTickable> networkSystems)
        {
            _simulatedWorld = simulatedWorld;
            _systems = Systems;
            _networkSystems = networkSystems;
        }

        public void Start()
        {
            if (_simulationTask != null && !_simulationTask.IsCompleted)
                return;

            _cts = new CancellationTokenSource();

            _simulationTask = GameCycle(_cts.Token);
        }

        public void Stop()
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
                _cts = null;
            }
        }

        private async Task GameCycle(CancellationToken token)
        {
            const int simulationTPS = 60;
            TimeSpan tickInterval = TimeSpan.FromMilliseconds(1000.0 / simulationTPS);

            const float networkInterval = 1.0f / 20.0f;
            float networkAccumulator = 0f;

            using var timer = new PeriodicTimer(tickInterval);
            var stopwatch = Stopwatch.StartNew();


            uint gameTicks = 0;
            try
            {
                while (await timer.WaitForNextTickAsync(token))
                {
                    float deltaTime = (float)stopwatch.Elapsed.TotalSeconds;
                    stopwatch.Restart();

                    if (deltaTime > 0.2f)
                        deltaTime = 0.2f;

                    for (int i = 0; i < _systems.Count; i++)
                    {
                        _systems[i].Tick(deltaTime, _simulatedWorld);
                    }

                    networkAccumulator += deltaTime;

                    if (networkAccumulator >= networkInterval)
                    {
                        float networkDelta = networkAccumulator;
                        networkAccumulator -= networkInterval; 

                        for (int i = 0; i < _networkSystems.Count; i++)
                        {
                            _networkSystems[i].NetworkTick(networkDelta, gameTicks, _simulatedWorld);
                        }
                    }

                    gameTicks++;
                }
            }
            catch (OperationCanceledException)
            {

            }
            finally
            {
                stopwatch.Stop();
            }
        }
    }
}
