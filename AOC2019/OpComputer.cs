using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static System.String;

namespace AOC2019
{
    public class OpComputer
    {
        private readonly List<int> memory;
        private int pos = 0;
        private readonly Queue<int> inputQueue = new Queue<int>();
        private readonly Queue<int> outputQueue = new Queue<int>();

        public string readMemory() => Join(',', memory);
        public int readMemoryPosition(int pos) => memory[pos];

        public enum OpCodes
        {
            OpAdd = 1,
            OpMultiply = 2,
            OpInput = 3,
            OpOutput = 4,
            OpJumpIfTrue = 5,
            OpJumpIfFalse = 6,
            OpIsLessThan = 7,
            OpIsEqualTo = 8,
            OpHalt = 99
        }
        
        

        public enum ParameterMode
        {
            Position = 0,
            Immediate = 1
        }
        
        public struct Operation
        {
            public readonly OpCodes opCode;
            public readonly ParameterMode[] parameterModes;
            public int[] parameters;
        
            public Operation(int value)
            {
                opCode = (OpCodes) (value % 100);
                parameters = Array.Empty<int>();

                if (!Enum.IsDefined(typeof(OpCodes), opCode))
                    throw new UnknownOperationException(opCode);

                parameterModes =
                    value
                        .ToString("00000").ToCharArray().ToList()
                        .ConvertAll(m => (ParameterMode) m - 48)
                        .Take(3)
                        .Reverse()
                        .ToArray();
            }

            public override string ToString() => 
                Format($"{opCode}({Join(',', parameters)})");
        }

        public OpComputer(string program)
        {
            memory =
                program
                    .Split(",")
                    .ToList()
                    .ConvertAll(int.Parse);
        }

        public OpComputer(List<int> program)
        {
            memory = program;
        }

        public void Run()
        {
            while (true)
            {
                var op = readOp();
                
                // HelperFunctions.Log("Memory::" + Join(',', memory));
                // HelperFunctions.Log("Inputs::" + Join(',', inputQueue));
                // HelperFunctions.Log("Outputs:" + Join(',', outputQueue));
                // HelperFunctions.Log($"{pos}:{op.ToString()}");

                switch (op.opCode)
                {
                    case OpCodes.OpAdd:
                        write(op.parameters[0] + op.parameters[1], op.parameters[2]);
                        break;

                    case OpCodes.OpMultiply:
                        write(op.parameters[0] * op.parameters[1], op.parameters[2]);
                        break;

                    case OpCodes.OpInput:
                        if (hasInput)
                            write(inputQueue.Dequeue(), op.parameters[0]);
                        else
                            throw new EmptyInputQueueException();
                        break;

                    case OpCodes.OpOutput:
                        outputQueue.Enqueue(op.parameters[0]);
                        break;

                    case OpCodes.OpJumpIfTrue:
                        if (op.parameters[0] != 0)
                            jump(op.parameters[1]);
                        break;
                    
                    case OpCodes.OpJumpIfFalse:
                        if (op.parameters[0] == 0)
                            jump(op.parameters[1]);
                        break;

                    case OpCodes.OpIsLessThan:
                        write(op.parameters[0] < op.parameters[1] ? 1 : 0, op.parameters[2]);
                        break;
                    
                    case OpCodes.OpIsEqualTo:
                        write(op.parameters[0] == op.parameters[1] ? 1 : 0, op.parameters[2]);
                        break;

                    case OpCodes.OpHalt:
                        return;

                    default:
                        throw new UnknownOperationException(op.opCode);
                }
            }
        }

        private bool hasInput => inputQueue.Count > 0;
        
        public void input(int value) => inputQueue.Enqueue(value);
        
        public bool hasOutput => outputQueue.Count > 0;

        public int readOutput() => outputQueue.Dequeue();

        private Operation readOp()
        {
            var operation = new Operation(read(ParameterMode.Immediate));
            
            switch (operation.opCode)
            {
                case OpCodes.OpAdd:
                case OpCodes.OpMultiply:
                case OpCodes.OpIsLessThan:
                case OpCodes.OpIsEqualTo:
                    operation.parameters = new[]
                    {
                        read(operation.parameterModes[0]),
                        read(operation.parameterModes[1]),
                        read(ParameterMode.Immediate)
                    };
                    break;
                case OpCodes.OpJumpIfTrue:
                case OpCodes.OpJumpIfFalse:
                    operation.parameters = new[]
                    {
                        read(operation.parameterModes[0]),
                        read(operation.parameterModes[1])
                    };
                    break;
                case OpCodes.OpOutput:
                    operation.parameters = new[]
                    {
                        read(operation.parameterModes[0])
                    };
                    break;
                case OpCodes.OpInput:
                    operation.parameters = new[]
                    {
                        read(ParameterMode.Immediate)
                    };
                    break;
                case OpCodes.OpHalt:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return operation;
        }

        private int read(ParameterMode mode)
        {
            switch (mode)
            {
                case ParameterMode.Position:
                    return memory[memory[pos++]];
                case ParameterMode.Immediate:
                    return memory[pos++];
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
        }

        private void write(int value, int address) => memory[address] = value;

        private void jump(int newPos)
        {
            if (newPos < 0 || newPos >= memory.Count)
                throw new JumpAddressOutOfRange(newPos);
            
            pos = newPos;
        }
    }

    public class UnknownOperationException : Exception
    {
        public OpComputer.OpCodes OpCode { get; }

        public UnknownOperationException(OpComputer.OpCodes opCode) :
            base($"Unknown OperationCode ({opCode}).")
        {
            OpCode = opCode;
        }
    }

    public class EmptyInputQueueException : Exception
    {
    }

    public class JumpAddressOutOfRange : Exception
    {
        public int Address { get; }

        public JumpAddressOutOfRange(int address) :
            base($"Jump operation to invalid address ({address})")
        {
            Address = address;
        }
    }
}
