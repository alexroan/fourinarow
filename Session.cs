using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using FourInARow.State;
using FourInARow.Strategies;

namespace FourInARow
{
    public class Session
    {

        public T[,] To2D<T>(T[][] source)
        {
            try
            {
                int FirstDim = source.Length;
                int SecondDim = source.GroupBy(row => row.Length).Single().Key; // throws InvalidOperationException if source is not rectangular

                var result = new T[FirstDim, SecondDim];
                for (int i = 0; i < FirstDim; ++i)
                    for (int j = 0; j < SecondDim; ++j)
                        result[i, j] = source[i][j];

                return result;
            }
            catch (InvalidOperationException)
            {
                throw new InvalidOperationException("The given jagged array is not rectangular.");
            }
        }

        public void Run()
        {
            Console.SetIn(new StreamReader(Console.OpenStandardInput(512)));
            string line;
            Board board = new Board();
            IStrategy strategy = new AlphaBetaMinimaxStrategy();//MinimaxStrategy();
            while ((line = Console.ReadLine()) != null)
            {
                if (line == String.Empty) continue;
                var parts = line.Split(' ');
                switch (parts[0])
                {
                    case "settings":

                        switch (parts[1])
                        {
                            case "your_botid":
                                var myBotId = int.Parse(parts[2]);
                                board.MyBotId = myBotId;
                                break;
                        }
                        break;
                    case "update":
                        switch (parts[1])
                        {
                            case "game":
                                switch (parts[2])
                                {
                                    case "field":
                                        var boardArray = To2D(parts[3].Split(';').Select(x => x.Split(',').Select(int.Parse).ToArray()).ToArray());
                                        board.Update(boardArray);
                                    break;
                                }
                            break;
                        }
                        break;
                    case "action":
                        var move = strategy.NextMove(board);
                        Console.WriteLine("place_disc {0}", move);
                        break;
                }
            }
        }
    }
}