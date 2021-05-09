using System;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public int[,] boardInfo = new int[8, 8];
    private GameObject[,] discList = new GameObject[8, 8];
    private GameObject[,] cursorList = new GameObject[8, 8];

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
                cursorList[i, j] = Instantiate(cursor,
                                               new Vector3(-3.5f + i, 0.01f, -3.5f + j),
                                               Quaternion.identity);
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
    }

    public void put(int row, int column)
    {
        // black
        boardInfo[row, column] = 1;
        GameObject disc = discList[row, column];
        if (!disc.activeSelf) disc.SetActive(true);

        // white
        //boardInfo[row, column] = -1;
        //GameObject disc = discList[row, column];
        //if (!disc.activeSelf) disc.SetActive(true);
        //discList[row, column].transform.Rotate(180, 0, 0);
    }
}
