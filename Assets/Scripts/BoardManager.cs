using System;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public int[,] boardInfo = new int[8, 8];
    private GameObject[,] discList = new GameObject[8, 8];
    private GameObject[,] cursorList = new GameObject[8, 8];

    GameObject disc;

    // 1:Black 0:void -1:White
    private void Start()
    {
        disc = Resources.Load("Disc") as GameObject;
        GameObject cursor = Resources.Load("Cursor") as GameObject;

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                boardInfo[i, j] = 0;
                cursorList[i, j] = Instantiate(cursor,
                                               new Vector3(-3.5f + i, 0.01f, -3.5f + j),
                                               Quaternion.identity);
            }
        }
    }

    private void Update()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                int state = boardInfo[i, j];
                GameObject disc = discList[i, j];
                if (state == 0)
                {
                    if (disc.activeSelf) disc.SetActive(false);
                }
                else
                {
                    if (!disc.activeSelf) disc.SetActive(true);
                }
                disc.GetComponent<Disc>().reload(state);
            }
        }
    }

    public void put(int row, int column)
    {
        // black
        boardInfo[row, column] = 1;
        discList[row, column] = Instantiate(disc,
                             new Vector3(-3.5f + row, 1, -3.5f + column),
                             Quaternion.identity);

        // white
        //boardInfo[row, column] = -1;
        //discList[row, column] = Instantiate(disc,
        //                     new Vector3(-3.5f + row, 1, -3.5f + column),
        //                     Quaternion.identity);
        //discList[row, column].transform.Rotate(180, 0, 0);
    }
}
