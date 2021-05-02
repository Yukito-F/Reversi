using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public int[,] boardInfo = new int[8, 8];
    private GameObject[,] discInfo = new GameObject[8,8];

    // 1:Black 0:void -1:White
    private void Start()
    {
        GameObject prefab = Resources.Load("Disc") as GameObject;
        for (int i = 0; i < 64; i++)
        {
            int row = (int)i / 8;
            int column = i % 8;
            // boardInfo[row, column] = 0;
            boardInfo[row, column] = 1 - 2 * (i % 2);
            discInfo[row, column] = 
                Instantiate(prefab, 
                            new Vector3(-3.5f + i % 8, 1, -3.5f +(int)i / 8), 
                            Quaternion.identity);
        }   
    }

    private void Update()
    {
        for (int i = 0; i < 64; i++)
        {
            int cell = boardInfo[(int)i / 8, i % 8];
            GameObject disc = discInfo[(int)i / 8, i % 8];
            if (cell == 0)
            {
                if (disc.activeSelf) disc.SetActive(false);
            }
            else
            {
                if (!disc.activeSelf) disc.SetActive(true);
                Vector3 angles = Vector3.zero;
                if (cell == -1) angles.x = 180;
                disc.transform.localEulerAngles = angles;
            }
        }
    }
}
