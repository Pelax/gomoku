using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board
{
    char[][] charCells;
    Cell[][] objectCells;
    List<Cell> freeCells;

    int size;
    bool dirty;
    bool finished;
    public string Result = "";

    List<Cell> moves;

    public Board(int _size)
    {
        size = _size;
        ResetCells();
    }

    public Board(Board _other)
    {
        size = _other.size;
        ResetCells();
        for (int col = 0; col < size; col++)
        {
            for (int row = 0; row < size; row++)
            {
                charCells[col][row] = _other.CharCells[col][row];
            }
        }
    }

    public char[][] CharCells
    {
        get { return charCells; }
    }

    public Cell[][] ObjectCells
    {
        get { return objectCells; }
    }

    public List<Cell> FreeCells
    {
        get { return freeCells; }
    }

    public List<Cell> Moves
    {
        get { return moves; }
    }

    public int Size
    {
        get { return size; }
    }

    public bool Dirty
    {
        get { return dirty; }
        set { dirty = value; }
    }

    public bool Finished
    {
        get { return finished; }
        set { finished = value; }
    }

    public void ResetCells()
    {
        charCells = new char[size][];
        objectCells = new Cell[size][];
        freeCells = new List<Cell>();
        moves = new List<Cell>();

        for (int col = 0; col < size; col++)
        {
            charCells[col] = new char[size];
            objectCells[col] = new Cell[size];
            for (int row = 0; row < size; row++)
            {
                charCells[col][row] = '0';
                Cell cell = new Cell(col, row, this);
                objectCells[col][row] = cell;
                freeCells.Add(cell);
            }
        }
        for (int index = 0; index < freeCells.Count; index++)
        {
            freeCells[index].SetNeighbours();
        }
        finished = false;
        dirty = true;
    }

    void MarkIntCell(int _col, int _row, char _value)
    {
        charCells[_col][_row] = _value;

        if (CheckWin(_col, _row, _value))
        {
            Result = "PLAYER " + _value + " WINS!";
//            Debug.Log(Result);
            finished = true;
        }
        else if (freeCells.Count == 0)
        {
            Result = "DRAW!";
//            Debug.Log(Result);
            finished = true;
        }
        dirty = true;
    }

    public void MarkCell(int _col, int _row, char _value)
    {
        for (int index = 0; index < freeCells.Count; index++)
        {
            Cell freeCell = freeCells[index];
            if (freeCell.Col == _col && freeCell.Row == _row)
            {
                MarkCell(freeCell, _value);
                break;
            }
        }
    }

    public void MarkCell(Cell _freeCell, char _value)
    {
        _freeCell.State = _value;
        freeCells.Remove(_freeCell);
        moves.Add(_freeCell);
        MarkIntCell(_freeCell.Col, _freeCell.Row, _value);
    }

    bool CheckWin(int _col, int _row, char _value)
    {
        bool result = CheckCol(_col, _row, _value) == 5 || CheckRow(_col, _row, _value) == 5
            || CheckDiagDesc(_col, _row, _value) == 5 || CheckDiagAsc(_col, _row, _value) == 5;
        return result;
    }

    public List<PotentialWin> CheckPotentialWinsVertical(int _col, int _row, char _value, int _minMatches)
    {
        int farestRow = _row - 4;
        List<PotentialWin> potentialWins = new List<PotentialWin>();
        for (int line = 0; line < 5; line++)
        {
            int matched = 0;
            int rowToStart = farestRow + line;
            int rowToEnd = rowToStart + 5;

            for (int row = rowToStart; row < rowToEnd; row++)
            {
                if (row < 0 || row > size - 1)
                {
                    matched = 0;
                    break;
                }
                if (charCells[_col][row] == _value)
                {
                    // one more matching cell in the line, add to matched
                    matched++;
                }
                else if (charCells[_col][row] != '0')
                {
                    // found an enemy cell in the line, no possible win
                    matched = 0;
                    break;
                }
            }
            if (matched >= _minMatches)
            {
                potentialWins.Add(new PotentialWin(matched, objectCells[_col][rowToStart], new Direction(0, 1), _value));
            }
        }
        return potentialWins;
    }

    public List<PotentialWin> CheckPotentialWinsHorizontal(int _col, int _row, char _value, int _minMatches)
    {
        int farestCol = _col - 4;
        List<PotentialWin> potentialWins = new List<PotentialWin>();
        for (int line = 0; line < 5; line++)
        {
            int matched = 0;
            int colToStart = farestCol + line;
            int colToEnd = colToStart + 5;
            //Debug.Log("line start: " + lineStart);
            for (int col = colToStart; col < colToEnd; col++)
            {
                if (col < 0 || col > size - 1)
                {
                    matched = 0;
                    break;
                }
                //Debug.Log("cell " + col + ", " + _row);
                if (charCells[col][_row] == _value)
                {
                    // one more matching cell in the line, add to matched
                    matched++;
                }
                else if (charCells[col][_row] != '0')
                {
                    // found an enemy cell in the line, no possible win
                    matched = 0;
                    break;
                }
            }
            if (matched >= _minMatches)
            {
                potentialWins.Add(new PotentialWin(matched, objectCells[colToStart][_row], new Direction(1, 0), _value));
            }
        }
        return potentialWins;
    }

    public List<PotentialWin> CheckPotentialWinsDiagonalDesc(int _col, int _row, char _value, int _minMatches)
    {
        int farestCol = _col - 4;
        int farestRow = _row - 4;
        List<PotentialWin> potentialWins = new List<PotentialWin>();
        for (int line = 0; line < 5; line++)
        {
            int matched = 0;
            int colToStart = farestCol + line;
            int rowToStart = farestRow + line;
            int col = colToStart;
            int row = rowToStart;
            //Debug.Log("line start: " + lineStart);
            for (int currentCell = 0; currentCell < 5; currentCell++)
            {
                if (col < 0 || col > size - 1 || row < 0 || row > size - 1)
                {
                    matched = 0;
                    break;
                }
                //Debug.Log("cell " + col + ", " + row);
                if (charCells[col][row] == _value)
                {
                    // one more matching cell in the line, add to matched
                    matched++;
                }
                else if (charCells[col][row] != '0')
                {
                    // found an enemy cell in the line, no possible win
                    matched = 0;
                    break;
                }
                col++;
                row++;
            }
            if (matched >= _minMatches)
            {
                potentialWins.Add(new PotentialWin(matched, objectCells[colToStart][rowToStart], new Direction(1, 1), _value));
            }
        }
        return potentialWins;
    }


    public List<PotentialWin> CheckPotentialWinsDiagonalAsc(int _col, int _row, char _value, int _minMatches)
    {
        int farestCol = _col - 4;
        int farestRow = _row + 4;
        List<PotentialWin> potentialWins = new List<PotentialWin>();
        for (int line = 0; line < 5; line++)
        {
            int matched = 0;
            int colToStart = farestCol + line;
            int rowToStart = farestRow - line;
            int col = colToStart;
            int row = rowToStart;
            //Debug.Log("line start: " + lineStart);
            for (int currentCell = 0; currentCell < 5; currentCell++)
            {
                if (col < 0 || col > size - 1 || row < 0 || row > size - 1)
                {
                    matched = 0;
                    break;
                }
                //Debug.Log("cell " + col + ", " + row);
                if (charCells[col][row] == _value)
                {
                    // one more matching cell in the line, add to matched
                    matched++;
                }
                else if (charCells[col][row] != '0')
                {
                    // found an enemy cell in the line, no possible win
                    matched = 0;
                    break;
                }
                col++;
                row--;
            }
            if (matched >= _minMatches)
            {
                potentialWins.Add(new PotentialWin(matched, objectCells[colToStart][rowToStart], new Direction(1, -1), _value));
            }
        }
        return potentialWins;
    }

    /// <summary>
    /// Checks the column
    /// All these 4 methods work similar. In all of them the direction variable is used to move in the direction of
    /// the pattern, and also as a flag to end the loop, since we add 2 to switch from -1 to 1, and then to 3
    /// </summary>
    /// <returns>The amount of consecutive matches in the column affected by the move</returns>
    /// <param name="_col">Col.</param>
    /// <param name="_row">Row.</param>
    /// <param name="_value">Value.</param>
    public int CheckCol(int _col, int _row, char _value)
    {
        int matched = 1;
        int direction = -1;
        int row = _row;
        while (direction != 3)
        {
            row += direction;
            if (row > -1 && row < size - 1 && charCells[_col][row] == _value)
            {
                matched++;
                if (matched == 5)
                {
                    return matched;
                }
            }
            else
            {
                row = _row;
                direction += 2;
            }
        }
        return matched;
    }

    public int CheckRow(int _col, int _row, char _value)
    {
        int matched = 1;
        int direction = -1;
        int col = _col;
        while (direction != 3)
        {
            col += direction;
            if (col > -1 && col < size - 1 && charCells[col][_row] == _value)
            {
                matched++;
                if (matched == 5)
                {
                    return matched;
                }
            }
            else
            {
                col = _col;
                direction += 2;
            }
        }
        return matched;
    }

    public int CheckDiagDesc(int _col, int _row, char _value)
    {
        int matched = 1;
        int direction = -1;
        int col = _col; int row = _row;
        while (direction != 3)
        {
            col += direction; row += direction;
            if (col > -1 && col < size - 1 && row > -1 && row < size - 1
                && charCells[col][row] == _value)
            {
                matched++;
                if (matched == 5)
                {
                    return matched;
                }
            }
            else
            {
                col = _col;
                row = _row;
                direction += 2;
            }
        }
        return matched;
    }

    public int CheckDiagAsc(int _col, int _row, char _value)
    {
        int matched = 1;
        int directionCol = -1;
        int directionRow = 1;
        int col = _col; int row = _row;
        while (directionCol != 3)
        {
            col += directionCol; row += directionRow;
            if (col > -1 && col < size - 1 && row > -1 && row < size - 1
                            && charCells[col][row] == _value)
            {
                matched++;
                if (matched == 5)
                {
                    return matched;
                }
            }
            else
            {
                col = _col;
                row = _row;
                directionCol += 2;
                directionRow -= 2;
            }
        }
        return matched;
    }



}
