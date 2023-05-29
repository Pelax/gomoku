using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardView : MonoBehaviour
{
    public GameObject cellPrefab;

    Board logicBoard;

    CellView[][] goCells;

    public void InitializeLogic(Board _logic)
    {
        logicBoard = _logic;
        BuildBoard();
    }

    public void BuildBoard()
    {
        transform.position = Vector3.zero;
		foreach (Transform child in transform)
		{
			Destroy(child.gameObject);
		}        
        Vector3 cellSize = cellPrefab.GetComponent<SpriteRenderer>().sprite.bounds.size;
        cellSize = cellSize * 1.1f;
        goCells = new CellView[logicBoard.Size][];

        for (int col = 0; col < logicBoard.Size; col++)
        {
            goCells[col] = new CellView[logicBoard.Size];
            for (int row = 0; row < logicBoard.CharCells[col].Length; row++)
            {
                GameObject goCell = Instantiate(cellPrefab);
                goCell.transform.SetParent(transform);
                goCell.transform.position = new Vector3(col * cellSize.x, -row * cellSize.y, 0);
                CellView viewCell = goCell.GetComponent<CellView>();
                viewCell.Cell = logicBoard.ObjectCells[col][row];
                goCells[col][row] = viewCell;
            }
        }
        transform.position = new Vector3(-cellSize.x * logicBoard.Size * 0.5f + cellSize.x * 0.5f,
                                         cellSize.y * logicBoard.Size * 0.5f - cellSize.y * 0.5f);
    }

    public void RefreshBoard()
    {
        for (int col = 0; col < logicBoard.Size; col++)
        {
            for (int row = 0; row < logicBoard.CharCells[col].Length; row++)
            {
                CellView viewCell = goCells[col][row];
                viewCell.State = logicBoard.CharCells[col][row];
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (logicBoard.Dirty)
        {
            RefreshBoard();
            logicBoard.Dirty = false;
        }
    }
}
