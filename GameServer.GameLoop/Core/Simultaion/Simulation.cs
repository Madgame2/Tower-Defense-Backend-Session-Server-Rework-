using GameServer.Domain.SessionWorld;
using GameServer.GameLoop.Core.Simultaion.Interfaces;
using GameServer.GameLoop.Core.Simultaion.Meta.Base;
using GameServer.GameLoop.Core.Simultaion.Steps.Interfaces;
using GameServer.Profiling.Metrics;
using Microsoft.Extensions.Logging;
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

        private readonly ILogger<Simulation> _logger;
        private readonly TickMetricsCollector _tickMetrics;

        public uint CurrentTick { get; private set; }

        public Simulation(GameRoom simulatedWorld,
            List<ITickable> Systems,
            List<INetworkTickable> networkSystems,
            ILogger<Simulation> logger,
            TickMetricsCollector tickMetricsCollector)
        {
            _simulatedWorld = simulatedWorld;
            _systems = Systems;
            _networkSystems = networkSystems;
            _logger = logger;
            _tickMetrics = tickMetricsCollector;
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

            TimeSpan tickInterval =
                TimeSpan.FromMilliseconds(1000.0 / simulationTPS);

            const float networkInterval = 1.0f / 20.0f;

            float networkAccumulator = 0f;

            using var timer = new PeriodicTimer(tickInterval);

            var stopwatch = Stopwatch.StartNew();

            uint gameTicks = 0;

            try
            {
                CurrentTick = gameTicks;

                AwaekeSystems(_systems);
                AwaekeSystems(_networkSystems);

                while (await timer.WaitForNextTickAsync(token))
                {
                    var measurement = _tickMetrics.BeginTick();

                    try
                    {
                        float deltaTime = (float)stopwatch.Elapsed.TotalSeconds;
                        stopwatch.Restart();

                        if (deltaTime > 0.2f)
                            deltaTime = 0.2f;

                        // Simulation
                        for (int i = 0; i < _systems.Count; i++)
                        {
                            await _systems[i].Tick(
                                deltaTime,
                                gameTicks,
                                _simulatedWorld);
                        }

                        // Network
                        networkAccumulator += deltaTime;

                        if (networkAccumulator >= networkInterval)
                        {
                            float networkDelta = networkAccumulator;
                            networkAccumulator -= networkInterval;

                            for (int i = 0; i < _networkSystems.Count; i++)
                            {
                                _networkSystems[i].NetworkTick(
                                    networkDelta,
                                    gameTicks,
                                    _simulatedWorld);
                            }
                        }

                        gameTicks++;
                    }
                    finally
                    {
                        _tickMetrics.EndTick(in measurement);
                    }
                }
            }
            catch (OperationCanceledException)
                when (token.IsCancellationRequested)
            {
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Game cycle failed");
            }
            finally
            {
                stopwatch.Stop();
            }
        }

        private void AwaekeSystems(List<ITickable> systems)
        {
            foreach (ITickable system in systems)
            {
                system.OnAwake(_simulatedWorld);
            }
        }

        private void AwaekeSystems(List<INetworkTickable> systems)
        {
            foreach (INetworkTickable system in systems)
            {
                system.OnAwake(_simulatedWorld);
            }
        }
    }
}
