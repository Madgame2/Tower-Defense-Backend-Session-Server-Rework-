using GameServer.Domain.ColliderSystem.Core;
using GameServer.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.Player.Components
{
    public class PlayerInputBuffer
    {
        private const int BUFFER_SIZE = 64;
        private MoveInputCommand[] _moveCommandsBuffer = new MoveInputCommand[BUFFER_SIZE];
        private bool[] _hasCommand = new bool[BUFFER_SIZE];
        private MoveInputCommand _lastComand;

        private uint _lastProcessedTick;
        private uint _nextExpectedTick;
        private bool _isInitialized = false;

        public void AddInput(in MoveInputCommand input)
        {
            if (!_isInitialized)
            {
                _lastProcessedTick = input.Tick;
                _isInitialized = true;
            }

            if (input.Tick < _lastProcessedTick)
                return;

            int index = (int)input.Tick % BUFFER_SIZE;
            _moveCommandsBuffer[index] = input;
            _hasCommand[index] = true;

            if(input.Tick > _lastProcessedTick)
            {
                _lastProcessedTick = input.Tick;
            }
        }


        public MoveInputCommand FetchNextInput()
        {
            if (!_isInitialized) 
                return default;

            uint pendingCount = _lastProcessedTick - _nextExpectedTick;
            if (pendingCount > 8)
            {
                _nextExpectedTick = _lastProcessedTick - 2;
            }

            int index = (int)(_nextExpectedTick % BUFFER_SIZE);

            if (_hasCommand[index])
            {
                _lastComand = _moveCommandsBuffer[index];
                _hasCommand[index] = false; 
                _nextExpectedTick++;
                return _lastComand;
            }

            _nextExpectedTick++;
            return _lastComand;
        }
    }
}
