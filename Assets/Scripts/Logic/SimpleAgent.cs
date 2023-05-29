using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple agent.
/// This agent allocates 2 separate lists that are modified every time a move is made
/// The threats list contains the potential ways in which the enemy could win
/// The attacks list contains the potential ways in which this agent could win
/// </summary>
public class SimpleAgent : Agent
{
    List<PotentialWin> threats;
    List<PotentialWin> attacks;

    int threatMinMatches = 2;
    int attackMinMatches = 1;

    public SimpleAgent(Board _board, char _player) : base(_board, _player)
    {
        threats = new List<PotentialWin>();
        attacks = new List<PotentialWin>();
    }

    public override void DoMove()
    {
        // first, check if no moves were made.
        if (board.Moves.Count == 0)
        {
            // take the center
            board.MarkCell(7, 7, player);
            // check the new attacks created
            FindNewPotentialWins(attacks, board.ObjectCells[7][7], attackMinMatches);
            return;
        }
        // the game already started, we should check our enemy's last move
        Cell enemysLastMove = board.Moves[board.Moves.Count - 1];
        // due to the enemy's move, maybe our attacks changed
        RemoveBlockedPotentialWins(attacks);
        // find the enemy's highest threat
        if (enemysLastMove.State == '0' || enemysLastMove.State == player)
        {
            Debug.LogError("error while checking threats, " + enemysLastMove.Col + ", " + enemysLastMove.Row + "   state " + enemysLastMove.State);
        }
        FindNewPotentialWins(threats, enemysLastMove, threatMinMatches);
        // define the variable for our next move so it can't be null.
        // defined as Random if nothing better happens, but it should in most cases
        Cell nextMove;
        //PrintPotentialWins(attacks, "attacks BEFORE");
        //PrintPotentialWins(threats, "threats BEFORE");
        // check if we are going to attack or block
        // by defaulf if there is a threat, we shouldn't attack
        bool attack = threats.Count <= 0;
        // but maybe our attack is more powerful than the threat
        if (!attack && attacks.Count > 0)
        {
            attack = attacks[attacks.Count - 1].Matched >= threats[threats.Count - 1].Matched;
        }

        if (attack)
        {
            //Debug.Log("ATTACKED");
            // no significant threats, addresss our attacks
            if (attacks.Count > 0)
            {
                PotentialWin closestWin = attacks[attacks.Count - 1];
                List<Cell> possibleMoves = GetPossibleMoves(closestWin);
                nextMove = possibleMoves[Random.Range(0, possibleMoves.Count)];
            }
            else
            {
                // we don't have any active attacks. this only happens if the player started
                // and the AI has the second move. pick a random free neighbour of last move
                nextMove = enemysLastMove.GetRandomFreeNeighBour();
            }
        }
        else
        {
            //Debug.Log("BLOCKED");
            // we need to address the highest threat
            PotentialWin highestThreat = threats[threats.Count - 1];
            List<Cell> possibleMoves = GetPossibleMoves(highestThreat);
            nextMove = possibleMoves[0]; // first cell by default

            // search for empty cells in that line that have most enemy neighbours
            int mostNeighbours = 0;
            char enemy = player == '1' ? '2' : '1';
            for (int index = 0; index < possibleMoves.Count; index++)
            {
                Cell possibleMove = possibleMoves[index];
                int enemyNeighbours = possibleMove.CountNeighboursOfState(enemy);
                if (enemyNeighbours > mostNeighbours)
                {
                    mostNeighbours = enemyNeighbours;
                    nextMove = possibleMove;
                }
            }
        }
        // if the move wasn't assigned (not a random free neighbour of the last enemy's move), get a random free cell
        if (nextMove == null)
        {
            nextMove = board.FreeCells[Random.Range(0, board.FreeCells.Count)];
        }
        // do a move no matter what
        board.MarkCell(nextMove, player);
        // if we blocked a thread we need to remove it
        RemoveBlockedPotentialWins(threats);
        // if we open new potential wins we need to find them
        if (nextMove.State == '0' || nextMove.State != player)
        {
            Debug.LogError("error while checking attacks, " + nextMove.Col + ", " + nextMove.Row + "   state " + nextMove.State);
        }
        FindNewPotentialWins(attacks, nextMove, attackMinMatches);
        //PrintPotentialWins(attacks, "attacks AFTER");
        //PrintPotentialWins(threats, "threats AFTER");
    }

    List<Cell> GetPossibleMoves(PotentialWin _line)
    {
        List<Cell> potentialWinLine = new List<Cell>();
        List<Cell> possibleMoves = new List<Cell>();
        Cell nextCell = _line.Start;
        for (int i = 0; i < 5; i++)
        {
            potentialWinLine.Add(nextCell);
            if (nextCell.State == '0')
            {
                //Debug.Log("Adding possible move: " + nextCell.Col + ", " + nextCell.Row);
                possibleMoves.Add(nextCell);
            }
            if (i < 4)
            {
                nextCell = board.ObjectCells[nextCell.Col + _line.Direction.col][nextCell.Row + _line.Direction.row];
            }
        }
        return possibleMoves;

    }

    public void FindNewPotentialWins(List<PotentialWin> _listToAdd, Cell _lastMove, int _minMatches)
    {
        List<PotentialWin> potentialWins = new List<PotentialWin>();

        potentialWins.AddRange(board.CheckPotentialWinsVertical(_lastMove.Col, _lastMove.Row, _lastMove.State, _minMatches));

        potentialWins.AddRange(board.CheckPotentialWinsHorizontal(_lastMove.Col, _lastMove.Row, _lastMove.State, _minMatches));

        potentialWins.AddRange(board.CheckPotentialWinsDiagonalDesc(_lastMove.Col, _lastMove.Row, _lastMove.State, _minMatches));

        potentialWins.AddRange(board.CheckPotentialWinsDiagonalAsc(_lastMove.Col, _lastMove.Row, _lastMove.State, _minMatches));

        _listToAdd.AddRange(potentialWins);

        _listToAdd.Sort((a, b) => a.Matched.CompareTo(b.Matched));
    }

    void PrintPotentialWins(List<PotentialWin> _lines, string _type)
    {
        string toPrint = "Showing " + _type + " (" + _lines.Count + "):";

        for (int i = 0; i < _lines.Count; i++)
        {
            toPrint += "\n" + _lines[i].Matched + " cell: " + _lines[i].Start.Col + ", " + _lines[i].Start.Row + "   state is: " + _lines[i].Start.State + " player is: " + _lines[i].Player;
        }

        Debug.Log(toPrint);
    }

    public void RemoveBlockedPotentialWins(List<PotentialWin> _lines)
    {
        for (int i = _lines.Count - 1; i >= 0; i--)
        {
            if (!_lines[i].CanStillWin())
            {
                _lines.RemoveAt(i);
            }
        }
    }
}
