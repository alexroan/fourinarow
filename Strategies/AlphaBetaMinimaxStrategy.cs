using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FourInARow.State;

namespace FourInARow.Strategies
{
    /// <summary>
    /// Alpha Beta implementation of minimax strategy
    /// </summary>
    public class AlphaBetaMinimaxStrategy : MinimaxStrategy
    {
        /// <summary>
        /// Max depth of the alpha beta implementation of minimax
        /// </summary>
        private static readonly int _maxDepth = 7;

        /// <summary>
        /// Initializes a new instance of the <see cref="MinimaxStrategy"/> class.
        /// </summary>
        public AlphaBetaMinimaxStrategy() : base(_maxDepth)
        {
        }

        /// <summary>
        /// returns the next move
        /// </summary>
        /// <param name="board">The board.</param>
        /// <returns></returns>
        public override int NextMove(Board board)
        {
            board.MyTurn = false;
            return Minimax(board);
        }
        
        /// <summary>
        /// Return the maximum value of the node
        /// </summary>
        /// <param name="state">The state.</param>
        /// <param name="currentDepth"></param>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        /// <returns></returns>
        protected int MaxValue(Board state, int currentDepth, int alpha, int beta)
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
        /// Return the minimum value of the node
        /// </summary>
        /// <param name="state">The state.</param>
        /// <param name="currentDepth"></param>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        /// <returns></returns>
        protected int MinValue(Board state, int currentDepth, int alpha, int beta)
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
