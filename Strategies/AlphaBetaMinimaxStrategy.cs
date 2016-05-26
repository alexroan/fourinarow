﻿using System;
using System.Diagnostics;
using System.Linq;
using FourInARow.State;

namespace FourInARow.Strategies
{
    /// <summary>
    ///     Alpha Beta implementation of minimax strategy
    /// </summary>
    public class AlphaBetaMinimaxStrategy : MinimaxStrategy
    {

        /// <summary>
        ///     Different Depths for different stages
        /// </summary>
        private static readonly int[] Depths = {5, 7, 9, 11, 13};

        /// <summary>
        ///     Initializes a new instance of the <see cref="MinimaxStrategy" /> class.
        /// </summary>
        public AlphaBetaMinimaxStrategy() : base(Depths[0])
        {
        }

        /// <summary>
        ///     alters the depth after 21 moves
        /// </summary>
        /// <param name="round"></param>
        public override void UpdateRound(int round)
        {
            base.UpdateRound(round);
            if (round >= 7 && round < 14)
                MaxDepth = Depths[1];
            else if (round >= 14 && round < 21)
                MaxDepth = Depths[2];
            else if (round >= 21 && round < 28)
                MaxDepth = Depths[3];
            else if (round >= 28)
                MaxDepth = Depths[4];
            }

        /// <summary>
        ///     returns the next move
        /// </summary>
        /// <param name="board">The board.</param>
        /// <returns></returns>
        public override int NextMove(Board board)
        {
            board.MyTurn = false;
            return AlphaBetaMinimax(board);
        }

        /// <summary>
        ///     AlphaBetaMinimax algorithm
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        private int AlphaBetaMinimax(Board state)
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
                var possibleMaxValue = MinValue(thisState, 0, int.MinValue, int.MaxValue);
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
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        /// <returns></returns>
        private int MaxValue(Board state, int currentDepth, int alpha, int beta)
        {
            if (IsTerminal(state) || currentDepth == MaxDepth)
            {
                return state.Utility();
            }
            currentDepth++;
            foreach (var child in Children(state).Values)
            {
                var minval = MinValue(child, currentDepth, alpha, beta);
                if (minval > alpha)
                    alpha = minval;
                if (alpha >= beta)
                    return alpha;
            }
            return alpha;
        }

        /// <summary>
        ///     Return the minimum value of the node
        /// </summary>
        /// <param name="state">The state.</param>
        /// <param name="currentDepth"></param>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        /// <returns></returns>
        private int MinValue(Board state, int currentDepth, int alpha, int beta)
        {
            if (IsTerminal(state) || currentDepth == MaxDepth)
            {
                return state.Utility();
            }
            currentDepth++;
            foreach (var child in Children(state).Values)
            {
                var maxval = MaxValue(child, currentDepth, alpha, beta);
                if (maxval < beta)
                    beta = maxval;
                if (beta <= alpha)
                    return beta;
            }
            return beta;
        }
    }
}