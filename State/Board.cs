using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using FourInARow.Enums;

namespace FourInARow.State
{
    /// <summary>
    ///     A game board
    /// </summary>
    public class Board
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Board" /> class.
        /// </summary>
        public Board()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Board" /> class.
        /// </summary>
        public Board(Board board)
        {
            BoardArray = (int[,]) board.BoardArray.Clone();
            MyBotId = board.MyBotId;
            MyTurn = !board.MyTurn;
        }

        /// <summary>
        ///     Gets the board array.
        /// </summary>
        /// <value>
        ///     The board array.
        /// </value>
        public int[,] BoardArray { get; private set; }

        /// <summary>
        ///     Gets or sets my bot identifier.
        /// </summary>
        /// <value>
        ///     My bot identifier.
        /// </value>
        public int MyBotId { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether [my turn].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [my turn]; otherwise, <c>false</c>.
        /// </value>
        public bool MyTurn { get; set; }

        /// <summary>
        ///     Places the move.
        /// </summary>
        /// <param name="col">The col.</param>
        /// <returns></returns>
        public bool PlaceMove(int col)
        {
            var movePlacePosition = CanPlaceMovePosition(col);
            if (movePlacePosition != -1)
            {
                if (MyTurn)
                    BoardArray[movePlacePosition, col] = MyBotId;
                else
                    BoardArray[movePlacePosition, col] = 0 - MyBotId;
                return true;
            }
            return false;
        }

        /// <summary>
        ///     Determines whether this instance [can place move position] the specified col.
        /// </summary>
        /// <param name="col">The col.</param>
        /// <returns>-1= can't move, any other is the position it will move to</returns>
        private int CanPlaceMovePosition(int col)
        {
            var openPosition = -1;
            for (var i = 5; i >= 0; i--)
            {
                var stateOfPosition = PositionState(i, col);
                if (stateOfPosition == Enums.PositionState.Free)
                {
                    openPosition = i;
                    break;
                }
            }
            return openPosition;
        }

        /// <summary>
        ///     Updates the specified board array.
        /// </summary>
        /// <param name="boardArray">The board array.</param>
        public void Update(int[,] boardArray)
        {
            BoardArray = boardArray;
        }

        /// <summary>
        ///     States the specified column field state
        /// </summary>
        /// <param name="col">The col.</param>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        public PositionState PositionState(int row, int col)
        {
            if (BoardArray[row, col] == 0) return Enums.PositionState.Free;
            if (BoardArray[row, col] == MyBotId) return Enums.PositionState.Me;
            return Enums.PositionState.Opponent;
        }


        ////////////////////UTILITY CALCULATOR LOOPS///////////////////
        public int Utility()
        {
            var utility = 0;
            for (var row = 5; row > -1; row--)
            {
                for (var col = 0; col < 7; col++)
                {
                    if (row > 2)
                    {
                        utility += VertFromBottomUtility(row, col);
                        if (col < 3)
                        {
                            utility += DiagRightFromBottomUtility(row, col);
                            utility += HorizontalFromBottomLeftUtility(row, col);
                        }
                        else if (col > 3)
                        {
                            utility += DiagLeftFromBottomUtility(row, col);
                        }
                        else if (col == 3)
                        {
                            utility += DiagLeftFromBottomUtility(row, col);
                            utility += DiagRightFromBottomUtility(row, col);
                            utility += HorizontalFromBottomLeftUtility(row, col);
                        }
                    }
                    else
                    {
                        if (col < 4)
                        {
                            utility += HorizontalFromBottomLeftUtility(row, col);
                        }
                    }
                }
            }
            return utility;
        }

        public PositionState WinningPlayer()
        {
            for (var row = 5; row > -1; row--)
            {
                for (var col = 0; col < 7; col++)
                {
                    if (row > 2)
                    {
                        if (VertFromBottomWinner(row, col))
                        {
                            return PositionState(row, col);
                        }
                        if (col < 3)
                        {
                            if (DiagRightFromBottomWinner(row, col) || HorizontalFromLeftWinner(row, col))
                            {
                                return PositionState(row, col);
                            }
                        }
                        else if (col > 3)
                        {
                            if (DiagLeftFromBottomWinner(row, col))
                            {
                                return PositionState(row, col);
                            }
                        }
                        else if (col == 3)
                        {
                            if (DiagLeftFromBottomWinner(row, col) || DiagRightFromBottomWinner(row, col) ||
                                HorizontalFromLeftWinner(row, col))
                            {
                                return PositionState(row, col);
                            }
                        }
                    }
                    else
                    {
                        if (col < 4)
                        {
                            if (HorizontalFromLeftWinner(row, col))
                            {
                                return PositionState(row, col);
                            }
                        }
                    }
                }
            }
            return Enums.PositionState.Free;
        }

        //////////////////////////////////////////////////////////////

        /////////////////UTILITY CALCULATORS///////////////////////

        /// <summary>
        /// Calculates the utility from state count.
        /// </summary>
        /// <param name="free">The free.</param>
        /// <param name="me">Me.</param>
        /// <param name="opponent">The opponent.</param>
        /// <returns></returns>
        public int CalculateUtilityFromStateCount(int free, int me, int opponent)
        {
            //some frees
            if (free != 0)
            {
                //just me and frees
                if (me != 0 && opponent == 0)
                {
                    if (me == 1)
                    {
                        return PlayerUtility(Enums.PositionState.Me, 1);
                    }
                    else if (me == 2)
                    {
                        return PlayerUtility(Enums.PositionState.Me, 200);
                    }
                    else if (me == 3)
                    {
                        return PlayerUtility(Enums.PositionState.Me, 500);
                    }
                }
                //just opponent and frees
                if (opponent != 0 && me == 0)
                {
                    if (me == 1)
                    {
                        return PlayerUtility(Enums.PositionState.Opponent, 1);
                    }
                    else if (me == 2)
                    {
                        return PlayerUtility(Enums.PositionState.Opponent, 200);
                    }
                    else if (me == 3)
                    {
                        return PlayerUtility(Enums.PositionState.Opponent, 500);
                    }
                }
            }
            //some me's
            else if (me != 0)
            {
                //just me's = WIN! me and opponents = dead 4
                return opponent == 0 ? PlayerUtility(Enums.PositionState.Me, 100000) : 0;
            }
            //some opponents
            else if (opponent != 0)
            {
                //just opponents = LOSS! opponents and me = dead 4
                return me == 0 ? PlayerUtility(Enums.PositionState.Opponent, 100000) : 0;
            }
            return 0;
        }

        /// <summary>
        /// Diagonal leaning right. taken from the bottom left
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        private int DiagRightFromBottomUtility(int row, int col)
        {
            int freeCount = 0;
            int meCount = 0;
            int opponentCount = 0;
            for (int i = 0; i < 4; i++)
            {
                PositionState state = PositionState(row - i, col + i);
                if (state == Enums.PositionState.Free)
                    freeCount++;
                else if (state == Enums.PositionState.Me)
                    meCount++;
                else if (state == Enums.PositionState.Opponent)
                    opponentCount++;
            }
            return CalculateUtilityFromStateCount(freeCount, meCount, opponentCount);
        }

        /// <summary>
        /// Diagonal utility leaning left from bottom.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        private int DiagLeftFromBottomUtility(int row, int col)
        {
            int freeCount = 0;
            int meCount = 0;
            int opponentCount = 0;
            for (int i = 0; i < 4; i++)
            {
                PositionState state = PositionState(row - i, col - i);
                if (state == Enums.PositionState.Free)
                    freeCount++;
                else if (state == Enums.PositionState.Me)
                    meCount++;
                else if (state == Enums.PositionState.Opponent)
                    opponentCount++;
            }
            return CalculateUtilityFromStateCount(freeCount, meCount, opponentCount);
        }

        /// <summary>
        /// Vertical utility from bottom
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        private int VertFromBottomUtility(int row, int col)
        {
            int freeCount = 0;
            int meCount = 0;
            int opponentCount = 0;
            for (int i = 0; i < 4; i++)
            {
                PositionState state = PositionState(row - i, col);
                if (state == Enums.PositionState.Free)
                    freeCount++;
                else if (state == Enums.PositionState.Me)
                    meCount++;
                else if (state == Enums.PositionState.Opponent)
                    opponentCount++;
            }
            return CalculateUtilityFromStateCount(freeCount, meCount, opponentCount);
        }

        /// <summary>
        /// Horizontal utility from bottom left
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        private int HorizontalFromBottomLeftUtility(int row, int col)
        {
            int freeCount = 0;
            int meCount = 0;
            int opponentCount = 0;
            for (int i = 0; i < 4; i++)
            {
                PositionState state = PositionState(row, col+i);
                if (state == Enums.PositionState.Free)
                    freeCount++;
                else if (state == Enums.PositionState.Me)
                    meCount++;
                else if (state == Enums.PositionState.Opponent)
                    opponentCount++;
            }
            return CalculateUtilityFromStateCount(freeCount, meCount, opponentCount);
        }
        /////////////////////////////////////////////////////


        ////////////////POSITION WINNER CHECKERS//////////////
        /// <summary>
        ///     Checks the diag right from bottom.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="col">The col.</param>
        /// <returns></returns>
        private bool DiagRightFromBottomWinner(int row, int col)
        {
            var thisState = PositionState(row, col);
            if (thisState == Enums.PositionState.Free)
                return false;

            if (PositionState(row - 1, col + 1) == thisState
                && PositionState(row - 2, col + 2) == thisState
                && PositionState(row - 3, col + 3) == thisState)
                return true;
            return false;
        }

        /// <summary>
        ///     Checks the diag left from bottom.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="col">The col.</param>
        /// <returns></returns>
        private bool DiagLeftFromBottomWinner(int row, int col)
        {
            var thisState = PositionState(row, col);
            if (thisState == Enums.PositionState.Free)
                return false;

            if (PositionState(row - 1, col - 1) == thisState
                && PositionState(row - 2, col - 2) == thisState
                && PositionState(row - 3, col - 3) == thisState)
                return true;
            return false;
        }

        /// <summary>
        ///     Checks the vert from bottom.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="col">The col.</param>
        /// <returns></returns>
        private bool VertFromBottomWinner(int row, int col)
        {
            var thisState = PositionState(row, col);
            if (thisState == Enums.PositionState.Free)
                return false;

            if (PositionState(row - 1, col) == thisState
                && PositionState(row - 2, col) == thisState
                && PositionState(row - 3, col) == thisState)
                return true;
            return false;
        }

        /// <summary>
        ///     Checks the horizontal from left.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="col">The col.</param>
        /// <returns></returns>
        private bool HorizontalFromLeftWinner(int row, int col)
        {
            var thisState = PositionState(row, col);
            if (thisState == Enums.PositionState.Free)
                return false;

            if (PositionState(row, col + 1) == thisState
                && PositionState(row, col + 2) == thisState
                && PositionState(row, col + 3) == thisState)
                return true;
            return false;
        }

        ///////////////////////////////////////////////////////////////


        /// <summary>
        ///     Prints out to string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var builder = new StringBuilder();
            for (var row = 0; row < 6; row++)
            {
                for (var col = 0; col < 7; col++)
                {
                    var player = PositionState(row, col);
                    switch (player)
                    {
                        case Enums.PositionState.Free:
                            builder.Append("_ ");
                            break;
                        case Enums.PositionState.Me:
                            builder.Append("X ");
                            break;
                        case Enums.PositionState.Opponent:
                            builder.Append("O ");
                            break;
                    }
                }
                builder.Append("\n");
            }
            return builder.ToString();
        }

        /// <summary>
        ///     Returns utility based on player position and utility
        /// </summary>
        /// <param name="player"></param>
        /// <param name="utility"></param>
        /// <returns></returns>
        private int PlayerUtility(PositionState player, int utility)
        {
            if (player == Enums.PositionState.Me)
                return utility;
            if (player == Enums.PositionState.Opponent)
                return -utility;
            return 0;
        }
    }
}