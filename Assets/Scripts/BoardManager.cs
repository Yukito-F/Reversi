using System;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    private int[,] boardInfo = new int[8, 8];
    private GameObject[,] discList = new GameObject[8, 8];
    private CursorController[,] cursorList = new CursorController[8, 8];

    private List<int[]>[,] expectedTable = new List<int[]>[8, 8];
    private int turn = 1; // 1:Black, -1:White

    // 1:Black 0:void -1:White
    private void Start()
    {
        GameObject disc = Resources.Load("Disc") as GameObject;
        GameObject cursor = Resources.Load("Cursor") as GameObject;

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                boardInfo[i, j] = 0;
                discList[i, j] = Instantiate(disc,
                             new Vector3(-3.5f + i, 0.5f, -3.5f + j),
                             Quaternion.identity);
                discList[i, j].SetActive(false);
                var _cursor = Instantiate(cursor,
                                          new Vector3(-3.5f + i, 0, -3.5f + j),
                                          Quaternion.identity);
                cursorList[i, j] = _cursor.GetComponent<CursorController>();
            }
        }
        for (int i = 3; i < 5; i++)
        {
            for (int j = 3; j < 5; j++)
            {
                discList[i, j].SetActive(true);
                if ((i + j) % 2 == 0)
                {
                    boardInfo[i, j] = 1;
                }
                else
                {
                    boardInfo[i, j] = -1;
                    discList[i, j].transform.Rotate(180, 0, 0);
                }
            }
        }
        checkBoard(-1);
    }

    public void put(int row, int column)
    {
        boardInfo[row, column] = turn;
        GameObject disc = discList[row, column];
        if (!disc.activeSelf) disc.SetActive(true);
        if (turn == -1) disc.transform.Rotate(180, 0, 0);

        foreach (int[] index in expectedTable[row, column])
        {
            discList[index[0], index[1]].GetComponent<Disc>().reload(turn);
            boardInfo[index[0], index[1]] = turn;
        }

        if (checkBoard(turn))
        {
            turn *= -1;
            if (checkBoard(turn))
            {
                Debug.Log("END");
            }
        };
        turn *= -1;
    }

    private bool checkBoard(int enemy)
    {
        bool exist = true;

        expectedTable = new List<int[]>[8, 8];
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                var tempList = checkRadiation(i, j, enemy);
                if (tempList != null)
                {
                    expectedTable[i, j] = tempList;
                    exist = false;
                }
                cursorList[i, j].changeColor(tempList != null);
            }
        }

        return exist;
    }

    private List<int[]> checkRadiation(int row, int column, int enemy)
    {
        List<int[]> expectedRadiation = new List<int[]>();
        bool exist = false;
        if (boardInfo[row, column] == 0)
        {
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    var tempList = checkLine(row, column, i, j, enemy);
                    if (tempList != null)
                    {
                        exist = true;
                        foreach (int[] item in tempList)
                        {
                            expectedRadiation.Add(item);
                        }
                    }
                }
            }
        }
        if (exist)
        {
            return expectedRadiation;
        }
        return null;
    }

    private List<int[]> checkLine(int row, int column, int up, int right, int enemy)
    {
        List<int[]> expectedLine = new List<int[]>();

        int rowCursor = row + up;
        int columnCursor = column + right;
        if (Mathf.Abs(3.5f - rowCursor) > 4 || Mathf.Abs(3.5f - columnCursor) > 4 || (up == 0 && right == 0))
        {
            return null;
        }
        if (boardInfo[rowCursor, columnCursor] == enemy)
        {
            while (outDetect(rowCursor, columnCursor) && boardInfo[rowCursor, columnCursor] == enemy)
            {
                expectedLine.Add(new int[] { rowCursor, columnCursor });
                rowCursor += up;
                columnCursor += right;
                if(outDetect(rowCursor, columnCursor) && boardInfo[rowCursor, columnCursor] == -enemy)
                {
                    return expectedLine;
                }
            }
        }
        return null;
    }

    private bool outDetect(int row, int column)
    {
        return (Mathf.Abs(3.5f - row) < 4 && Mathf.Abs(3.5f - column) < 4);
    }
}
