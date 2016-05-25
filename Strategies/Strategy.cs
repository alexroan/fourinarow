using System;
using System.Collections.Generic;
using FourInARow.Enums;
using FourInARow.State;

namespace FourInARow.Strategies
{
    /// <summary>
    ///     Base strategy class
    /// </summary>
    public class Strategy : IStrategy
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="maxDepth"></param>
        public Strategy(int maxDepth)
        {
            MaxDepth = maxDepth;
        }

        /// <summary>
        ///     Max depth
        /// </summary>
        protected int MaxDepth { get; set; }

        protected int Round { get; set; }

        /// <summary>
        ///     Generates next move
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        public virtual int NextMove(Board board)
        {
            throw new NotImplementedException();
        }

        public virtual void UpdateRound(int round)
        {
            Round = round;
        }

        /// <summary>
        ///     Generates available children
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        protected Dictionary<int, Board> Children(Board state)
        {
            //key is action (e,i the column move)
            var children = new Dictionary<int, Board>();
            for (var i = 0; i < 7; i++)
            {
                var newBoard = new Board(state);
                if (newBoard.PlaceMove(i))
                {
                    children.Add(i, newBoard);
                }
            }
            return children;
        }

        /// <summary>
        ///     Determines whether the specified state is terminal.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        protected bool IsTerminal(Board state)
        {
            var winner = state.WinningPlayer();
            if (winner != PositionState.Free)
                return true;
            return false;
        }
    }
}