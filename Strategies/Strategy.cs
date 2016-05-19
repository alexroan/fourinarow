using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FourInARow.Enums;
using FourInARow.State;

namespace FourInARow.Strategies
{
    /// <summary>
    /// Base strategy class
    /// </summary>
    public class Strategy : IStrategy
    {
        /// <summary>
        /// Max depth
        /// </summary>
        protected int MaxDepth { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="maxDepth"></param>
        public Strategy(int maxDepth)
        {
            MaxDepth = maxDepth;
        }

        /// <summary>
        /// Generates next move
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        public virtual int NextMove(Board board)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Generates available children
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        protected Dictionary<int, Board> Children(Board state)
        {
            //key is action (e,i the column move)
            Dictionary<int, Board> children = new Dictionary<int, Board>();
            for (int i = 0; i < 7; i++)
            {
                Board newBoard = new Board(state);
                if (newBoard.PlaceMove(i))
                {
                    children.Add(i, newBoard);
                }
            }
            return children;
        }

        /// <summary>
        /// Determines whether the specified state is terminal.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        protected bool IsTerminal(Board state)
        {
            PositionState winner = state.WinningPlayer();
            if (winner != PositionState.Free)
                return true;
            return false;
        }
    }
}
