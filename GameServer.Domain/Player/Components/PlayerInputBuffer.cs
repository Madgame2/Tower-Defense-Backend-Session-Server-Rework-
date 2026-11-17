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

        private readonly PlayerStateSnapShootClient[] _buffer =
            new PlayerStateSnapShootClient[BUFFER_SIZE];

        private readonly bool[] _hasCommand =
            new bool[BUFFER_SIZE];

        private PlayerStateSnapShootClient _lastCommand;
        private uint _lastProcessedTick;

        private bool _isInitialized;
        private bool _hasProcessed;

        public void AddInput(in PlayerStateSnapShootClient input)
        {
            if (!_isInitialized)
            {
                _isInitialized = true;
                _lastCommand = input;
            }

            // Не принимаем уже обработанные snapshot.
            if (_hasProcessed &&
                !IsNewer(input.Tick, _lastProcessedTick))
                return;

            int index = (int)(input.Tick % BUFFER_SIZE);

            if (_hasCommand[index])
            {
                // Не перезаписываем более новую команду старой.
                if (!IsNewer(input.Tick, _buffer[index].Tick))
                    return;
            }

            _buffer[index] = input;
            _hasCommand[index] = true;
        }

        public PlayerStateSnapShootClient FetchNextInput()
        {
            int newestIndex = -1;
            uint newestTick = 0;

            for (int i = 0; i < BUFFER_SIZE; i++)
            {
                if (!_hasCommand[i])
                    continue;

                uint tick = _buffer[i].Tick;

                if (_hasProcessed && !IsNewer(tick, _lastProcessedTick))
                {
                    _hasCommand[i] = false;
                    continue;
                }

                if (newestIndex == -1 || IsNewer(tick, newestTick))
                {
                    newestIndex = i;
                    newestTick = tick;
                }
            }

            // Новых snapshot нет — сохраняем последнее состояние.
            if (newestIndex == -1)
            {
                return new PlayerStateSnapShootClient(
                    _lastProcessedTick,
                    _lastCommand.MoveDirection,
                    _lastCommand.Rotation,
                    _lastCommand.LookAtPoint,
                    _lastCommand.Actions
                );
            }

            _lastCommand = _buffer[newestIndex];
            _lastProcessedTick = newestTick;
            _hasProcessed = true;

            // Удаляем выбранный snapshot.
            _hasCommand[newestIndex] = false;

            return _lastCommand;
        }

        private static bool IsNewer(uint a, uint b)
        {
            return unchecked((int)(a - b)) > 0;
        }
    }


}
