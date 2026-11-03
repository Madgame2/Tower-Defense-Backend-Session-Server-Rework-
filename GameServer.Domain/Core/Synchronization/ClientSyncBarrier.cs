using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.Core.Synchronization
{
    public sealed class ClientSyncBarrier
    {
        private readonly object _lock = new();

        private readonly HashSet<string> _pendingClients = new();

        private TaskCompletionSource _completion = new(
            TaskCreationOptions.RunContinuationsAsynchronously);

        public void Begin(IEnumerable<string> clientIds)
        {
            lock (_lock)
            {
                _pendingClients.Clear();

                foreach (var id in clientIds)
                    _pendingClients.Add(id);

                _completion = new TaskCompletionSource(
                    TaskCreationOptions.RunContinuationsAsynchronously);

                if (_pendingClients.Count == 0)
                    _completion.TrySetResult();
            }
        }

        public void Complete(string clientId)
        {
            lock (_lock)
            {
                if (!_pendingClients.Remove(clientId))
                    return;

                if (_pendingClients.Count == 0)
                    _completion.TrySetResult();
            }
        }

        public async Task<bool> WaitAsync(
            TimeSpan timeout,
            CancellationToken cancellationToken)
        {
            try
            {
                await _completion.Task.WaitAsync(timeout, cancellationToken);
                return true;
            }
            catch (TimeoutException)
            {
                return false;
            }
        }
    }
}
