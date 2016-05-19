﻿using FourInARow.State;

namespace FourInARow.Strategies
{
    public interface IStrategy
    {
        int NextMove(Board board);
    }
}