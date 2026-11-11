using GameServer.Domain.ColliderSystem.Core;
using GameServer.Domain.ValueObjects;
using GameServer.Domain.ValueObjects.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.Player.Components
{

    public sealed class PlayerInputBuffer
    {
        private const int BUFFER_SIZE = 64;
        private const uint MAX_PENDING_TICKS = 8;

        private readonly PlayerStateSnapShootClient[] _buffer =
            new PlayerStateSnapShootClient[BUFFER_SIZE];

        private readonly bool[] _hasCommand =
            new bool[BUFFER_SIZE];

        private PlayerStateSnapShootClient _lastCommand;

        private uint _nextExpectedTick;
        private uint _lastReceivedTick;

        private bool _isInitialized;

        public void AddInput(in PlayerStateSnapShootClient input)
        {
            if (!_isInitialized)
            {
                _nextExpectedTick = input.Tick;
                _lastReceivedTick = input.Tick;
                _isInitialized = true;
            }

            // Отбрасываем уже обработанные команды.
            if (IsOlder(input.Tick, _nextExpectedTick))
                return;

            // Не принимаем команды слишком далеко впереди.
            if (input.Tick - _nextExpectedTick >= BUFFER_SIZE)
                return;

            int index = (int)(input.Tick % BUFFER_SIZE);

            // Не перезаписываем существующую команду.
            if (_hasCommand[index])
            {
                if (_buffer[index].Tick == input.Tick)
                    return; // Дубликат пакета.

                // Слот занят другой необработанной командой.
                return;
            }

            _buffer[index] = input;
            _hasCommand[index] = true;

            if (IsNewer(input.Tick, _lastReceivedTick))
                _lastReceivedTick = input.Tick;
        }

        public PlayerStateSnapShootClient FetchNextInput()
        {
            if (!_isInitialized)
                return default;

            int index = (int)(_nextExpectedTick % BUFFER_SIZE);

            if (_hasCommand[index] &&
                _buffer[index].Tick == _nextExpectedTick)
            {
                _lastCommand = _buffer[index];
                _hasCommand[index] = false;

                _nextExpectedTick++;

                return _lastCommand;
            }


            _nextExpectedTick++;

            return new PlayerStateSnapShootClient(
                _nextExpectedTick - 1,
                _lastCommand.MoveDirection,
                _lastCommand.Rotation,
                _lastCommand.LookAtPoint,
                ActionFlags.None
            );
        }

        private void SkipUntil(uint targetTick)
        {
            while (_nextExpectedTick != targetTick)
            {
                int index = (int)(_nextExpectedTick % BUFFER_SIZE);

                if (_hasCommand[index] &&
                    _buffer[index].Tick == _nextExpectedTick)
                {
                    _hasCommand[index] = false;
                }

                _nextExpectedTick++;
            }
        }

        private static bool IsOlder(uint a, uint b)
        {
            return unchecked((int)(a - b)) < 0;
        }

        private static bool IsNewer(uint a, uint b)
        {
            return unchecked((int)(a - b)) > 0;
        }
    }

}
