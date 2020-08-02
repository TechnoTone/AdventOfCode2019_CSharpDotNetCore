using System;
using System.Collections.Generic;
using System.Linq;
using AOC2019;

public class Day13
{
    private const int BLOCK = 2;
    private const int PADDLE = 3;
    private const int BALL = 4;

    private readonly OpComputer computer;

    public Day13(string program) =>
        computer = new OpComputer(program);

    public int RunGameToEndAndCountBlocks()
    {
        var programOutput = computer.RunUntilHalt();
        var tiles = parseTiles(programOutput);
        return tiles.Count(x => x.Item3 == BLOCK);
    }

    public long BeatTheGameAndReadTheScore()
    {
        var score = 0L;
        var ballX = 0L;
        var paddleX = 0L;

        var outputs = new long[3];
        var outputIndex = 0;
        while (true)
        {
            switch (computer.Run())
            {
                case (OpComputer.Response.Halt, _):
                    return score;

                case (OpComputer.Response.Output, long value):
                    outputs[outputIndex++] = value;
                    if (outputIndex == 3)
                    {
                        outputIndex = 0;
                        switch ((outputs[0], outputs[2]))
                        {
                            case (long newX, BALL):
                                ballX = newX;
                                HelperFunctions.Log($"Ball: {outputs.JoinToString()}");
                                break;
                            case (long newX, PADDLE):
                                paddleX = newX;
                                HelperFunctions.Log($"Paddle: {outputs.JoinToString()}");
                                break;
                            case (-1, long newScore):
                                score = newScore;
                                // HelperFunctions.Log($"Score: {score}");
                                break;
                        }
                    }
                    break;

                case (OpComputer.Response.AwaitingInput, _):
                    var joystickInput = ballX.CompareTo(paddleX);
                    HelperFunctions.Log($"Joystick: {joystickInput}");
                    computer.Input(joystickInput);
                    break;

            }
        }
    }

    private static IEnumerable<(long, long, long)> parseTiles(IReadOnlyList<long> input)
    {
        var tiles = new List<(long, long, long)>();
        var i = 0;
        while (i <= input.Count - 3)
            tiles.Add((input[i++], input[i++], input[i++]));

        return tiles;
    }
}