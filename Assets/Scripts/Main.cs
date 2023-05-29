using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    public int boardSize;
    public InputField boardSizeInput;
    public bool player1IsAI;
    public Toggle player1IsAIToggle;
    public bool player2IsAI;
    public Toggle player2IsAIToggle;
    public Text result;

    public BoardView viewBoard;

    public static Main instance = null;

    Board logicBoard;
    bool turnOf1 = true;
    Agent player1 = null;
    Agent player2 = null;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("Created more than one Main instance");
        }
    }

    // Use this for initialization
    void Start()
    {
        Reset();
    }

    public void Reset()
    {
        turnOf1 = true;
        result.text = "";
        int.TryParse(boardSizeInput.text, out boardSize);
        if (boardSize == 0)
        {
            boardSize = 15;
        }
		player1IsAI = player1IsAIToggle.isOn;
		player2IsAI = player2IsAIToggle.isOn;

		logicBoard = new Board(boardSize);
		viewBoard.InitializeLogic(logicBoard);
		if (player1IsAI)
		{
			player1 = new SimpleAgent(logicBoard, '1');
		}
		if (player2IsAI)
		{
			player2 = new SimpleAgent(logicBoard, '2');
		}
	}

    // Update is called once per frame
    void Update()
    {
        if (logicBoard.Finished)
        {
            result.text = logicBoard.Result;
            return;
        }
        if (turnOf1 && player1IsAI)
        {
            player1.DoMove();
            turnOf1 = !turnOf1;
        }
        else if (!turnOf1 && player2IsAI)
        {
            player2.DoMove();
            turnOf1 = !turnOf1;
        }
    }

    public void CellClicked(int _col, int _row, CellView _viewCell)
    {
        if (logicBoard.Finished)
        {
            return;
        }
        if ((turnOf1 && !player1IsAI) || (!turnOf1 && !player2IsAI))
        {
            if (_viewCell.State == '0')
            {
                logicBoard.MarkCell(_col, _row, turnOf1 ? '1' : '2');
                turnOf1 = !turnOf1;
            }
        }
    }
}
