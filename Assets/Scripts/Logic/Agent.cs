using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent
{
    protected Board board;
    protected char player;

    public Agent(Board _board, char _player)
    {
        board = _board;
        player = _player;
    }

    virtual public void DoMove()
    {
        
    }
}
