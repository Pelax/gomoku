using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellView : MonoBehaviour
{
    public Sprite[] states;

    Cell cell;

    public char State
    {
        get { return cell.State; }
        set
        {
            cell.State = value;
            GetComponent<SpriteRenderer>().sprite = states[int.Parse(cell.State.ToString())];
        }
    }

    public Cell Cell
    {
        get { return cell; }
        set { cell = value; }
    }

    void OnMouseDown()
    {
        Main.instance.CellClicked(cell.Col, cell.Row, this);
    }
}
