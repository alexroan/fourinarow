using System;
using System.Collections.Generic;
using System.Linq;

namespace FourInARow.Strategies
{
    /// <summary>
    /// 
    /// </summary>
    public class MinimaxStrategy : IStrategy
    {

        private static readonly int _maxDepth = 5;

        /// <summary>
        /// Initializes a new instance of the <see cref="MinimaxStrategy"/> class.
        /// </summary>
        public MinimaxStrategy()
        {
        }

        /// <summary>
        /// returns the next move
        /// </summary>
        /// <param name="board">The board.</param>
        /// <returns></returns>
        public int NextMove(Board board)
        {
            board.MyTurn = false;
            return Minimax(board);
        }

        /// <summary>
        /// Minimax algorithm (I think)
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        private int Minimax(Board state)
        {
            var children = Children(state);
            var childKeys = children.Keys;

            int maxValue = Int32.MinValue;
            //the next move
            var highestValueAction = -1;
            foreach (var childKey in childKeys)
            {
                var thisState = children[childKey];

                if (IsTerminal(thisState))
                    return childKey;

                var possibleMaxValue = MinValue(thisState, 0);
                if (possibleMaxValue > maxValue)
                {
                    highestValueAction = childKey;
                    maxValue = possibleMaxValue;
                }
            }
            
            //If no good moves are found then
            if (highestValueAction == -1)
            {
                Random rand = new Random();
                while (true)
                {
                    int num = rand.Next(0, 7);
                    if (childKeys.Contains(num))
                        return num;
                }
            }
            return highestValueAction;
        }

        /// <summary>
        /// Return the maximum value of the node
        /// </summary>
        /// <param name="state">The state.</param>
        /// <param name="currentDepth"></param>
        /// <returns></returns>
        private int MaxValue(Board state, int currentDepth)
        {
            if (IsTerminal(state) || currentDepth == _maxDepth)
            {
                return Utility(state);
            }
            int value = Int32.MinValue;
            currentDepth++;
            foreach (var child in Children(state).Values)
            {
                var minval = MinValue(child, currentDepth);
                if (minval > value)
                    value = minval;
            }
            return value;
        }

        /// <summary>
        /// Return the minimum value of the node
        /// </summary>
        /// <param name="state">The state.</param>
        /// <param name="currentDepth"></param>
        /// <returns></returns>
        private int MinValue(Board state, int currentDepth)
        {
            if (IsTerminal(state) || currentDepth == _maxDepth)
            {
                return Utility(state);
            }
            int value = Int32.MaxValue;
            currentDepth++;
            foreach (var child in Children(state).Values)
            {
                var maxval = MaxValue(child, currentDepth);
                if (maxval < value)
                    value = maxval;
            }
            return value;
        }

        /// <summary>
        /// Utilities the specified state.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private int Utility(Board state)
        {
            return state.Utility();
        }

        private Dictionary<int, Board> Children(Board state)
        {
            //key is action (e,i the column move)
            Dictionary<int, Board> children = new Dictionary<int, Board>();
            for (int i = 0; i < 7; i++)
            {
                Board newBoard = new Board(state);
                if (newBoard.PlaceMove(i))
                {
                    children.Add(i,newBoard);
                }
            }
            return children;
        }

        /// <summary>
        /// Determines whether the specified state is terminal.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        private bool IsTerminal(Board state)
        {
            PositionState winner = state.WinningPlayer();
            if (winner != PositionState.Free)
                return true;
            return false;
        }
    }
}