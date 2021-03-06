﻿using System;
using System.Linq;
using FourInARow.State;

namespace FourInARow.Strategies
{
    /// <summary>
    ///     Minimax strategy
    /// </summary>
    public class MinimaxStrategy : Strategy
    {
        /// <summary>
        ///     max depth of minimax strategy
        /// </summary>
        private static readonly int _maxDepth = 5;

        /// <summary>
        ///     Initializes new instance of Minimax with max depth
        /// </summary>
        /// <param name="maxDepth"></param>
        public MinimaxStrategy(int maxDepth) : base(maxDepth)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MinimaxStrategy" /> class.
        /// </summary>
        public MinimaxStrategy() : this(_maxDepth)
        {
        }

        /// <summary>
        ///     returns the next move
        /// </summary>
        /// <param name="board">The board.</param>
        /// <returns></returns>
        public override int NextMove(Board board)
        {
            board.MyTurn = false;
            return Minimax(board);
        }

        /// <summary>
        ///     Minimax algorithm
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        private int Minimax(Board state)
        {
            var children = Children(state);
            var childKeys = children.Keys;
            var maxValue = int.MinValue;
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
                var rand = new Random();
                while (true)
                {
                    var num = rand.Next(0, 7);
                    if (childKeys.Contains(num))
                        return num;
                }
            }
            return highestValueAction;
        }

        /// <summary>
        ///     Return the maximum value of the node
        /// </summary>
        /// <param name="state">The state.</param>
        /// <param name="currentDepth"></param>
        /// <returns></returns>
        private int MaxValue(Board state, int currentDepth)
        {
            if (IsTerminal(state) || currentDepth == MaxDepth)
            {
                return state.Utility();
            }
            var value = int.MinValue;
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
        ///     Return the minimum value of the node
        /// </summary>
        /// <param name="state">The state.</param>
        /// <param name="currentDepth"></param>
        /// <returns></returns>
        private int MinValue(Board state, int currentDepth)
        {
            if (IsTerminal(state) || currentDepth == MaxDepth)
            {
                return state.Utility();
            }
            var value = int.MaxValue;
            currentDepth++;
            foreach (var child in Children(state).Values)
            {
                var maxval = MaxValue(child, currentDepth);
                if (maxval < value)
                    value = maxval;
            }
            return value;
        }
    }
}