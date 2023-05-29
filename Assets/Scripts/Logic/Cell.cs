using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    Cell[] objectNeighbours;
    int[] strNeighbours;

    char state;
    int col;
    int row;
    Board board;

    public char State
    {
        get { return state; }
        set { state = value; }
    }

    public int Col
    {
        get { return col; }
        set { col = value; }
    }

    public int Row
    {
        get { return row; }
        set { row = value; }
    }

    public Board Board
    {
        get { return board; }
    }

    public Cell[] ObjectNeighbours
    {
        get { return objectNeighbours; }
    }

    public Cell(int _col, int _row, Board _board)
    {
        col = _col;
        row = _row;
        state = '0';
        board = _board;
    }

    public int GetNeighbourIndex(Cell _cell)
    {
        for (int index = 0; index < objectNeighbours.Length; index++)
        {
            if (_cell == objectNeighbours[index])
            {
                return index;
            }
        }
        // not a neighbour
        return -1;
    }

    public Cell GetRandomFreeNeighBour()
    {
        List<Cell> allNeighbours = new List<Cell>();
        allNeighbours.AddRange(objectNeighbours);
        while (allNeighbours.Count > 0)
        {
            Cell randomNeighbour = allNeighbours[Random.Range(0, allNeighbours.Count)];
            allNeighbours.Remove(randomNeighbour);
            if (randomNeighbour != null && randomNeighbour.State == '0')
            {
                return randomNeighbour;
            }
        }
        return null;
    }

    public int CountNeighboursOfState(char _state)
    {
        int count = 0;
        for (int i = 0; i < objectNeighbours.Length; i++)
        {
            if (objectNeighbours[i] != null && objectNeighbours[i].State == _state)
            {
                count++;
            }
        }
        return count;
    }

    public void SetNeighbours()
    {
        objectNeighbours = new Cell[8];

        int nCol;
        int nRow;

        // top left neighbour
        nCol = col - 1; nRow = row - 1;
        objectNeighbours[0] = nCol > -1 && nRow > -1 ? board.ObjectCells[nCol][nRow] : null;

        // top neighbour
        nRow = row - 1;
        objectNeighbours[1] = nRow > -1 ? board.ObjectCells[col][nRow] : null;

        // top right neighbour
        nCol = col + 1; nRow = row - 1;
        objectNeighbours[2] = nCol < board.Size && nRow > -1 ? board.ObjectCells[nCol][nRow] : null;

        // right neighbour
        nCol = col + 1;
        objectNeighbours[3] = nCol < board.Size ? board.ObjectCells[nCol][row] : null;

        // bottom right neighbour
        nCol = col + 1; nRow = row + 1;
        objectNeighbours[4] = nCol < board.Size && nRow < board.Size ? board.ObjectCells[nCol][nRow] : null;

        // bottom neighbour
        nRow = row + 1;
        objectNeighbours[5] = nRow < board.Size ? board.ObjectCells[col][nRow] : null;

        // bottom left neighbour
        nCol = col - 1; nRow = row + 1;
        objectNeighbours[6] = nCol > -1 && nRow < board.Size ? board.ObjectCells[nCol][nRow] : null;

        // left neighbour
        nCol = col - 1;
        objectNeighbours[7] = nCol > -1 ? board.ObjectCells[nCol][row] : null;

        // debug
        //if ((col == 0 & row == 0) || (col == 14 & row == 0) || (col == 7 & row == 7) || (col == 0 & row == 14) || (col == 14 & row == 14))
        //{
        //    Debug.Log("debug cell, printint neighbours...");
        //    for (int i = 0; i < objectNeighbours.Length; i++)
        //    {
        //        if (objectNeighbours[i] != null)
        //        {
        //            board.MarkCell(objectNeighbours[i], '1');
        //            Debug.Log("c " + objectNeighbours[i].Col + " r " + objectNeighbours[i].Row);
        //        }
        //    }
        //}
    }

}
