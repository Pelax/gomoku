using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotentialWin
{
    int matched;
    Cell start;
    Direction direction;
    char player;

    public PotentialWin(int _matched, Cell _start, Direction _direction, char _player)
    {
        matched = _matched;
        start = _start;
        direction = _direction;
        player = _player;

        //Debug.Log("Potential win added, start at " + start.Col + ", " + start.Row + "  state is: " + start.State + " player is: " + player
		         //+ "\n" + " direction is " + direction.col + ", " + direction.row);
	}

    public bool CanStillWin()
    {
		Cell nextCell = start;
        Board board = start.Board;
		for (int i = 0; i < 5; i++)
		{
            if (nextCell.State != player && nextCell.State != '0')
			{
                //Debug.Log("Potential win with start at " + start.Col + ", " + start.Row + " can't win anymore, cell " + nextCell.Col + ", " + nextCell.Row + " is: " + nextCell.State
                 //         + "\n" + " direction is " + direction.col + ", " + direction.row);
                return false;
			}
            if (i < 4)
            {
                nextCell = board.ObjectCells[nextCell.Col + direction.col][nextCell.Row + direction.row];
            }
		}
        return true;
	}

    public int Matched
    {
        get
        {
            return matched;
        }
    }

	public Cell Start
	{
		get
		{
			return start;
		}
	}

    public Direction Direction
	{
		get
		{
            return direction;
		}
	}

    public char Player
    {
        get
        {
            return player;
        }
    }

}
