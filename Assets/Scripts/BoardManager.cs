using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public int[,] boardInfo = new int[8, 8];
    private GameObject[,] discList = new GameObject[8,8];

    // 1:Black 0:void -1:White
    private void Start()
    {
        GameObject prefab = Resources.Load("Disc") as GameObject;
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                boardInfo[i, j] = 1;
                discList[i, j] = Instantiate(prefab,
                                             new Vector3(-3.5f + i, 1, -3.5f + j),
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

        if (Input.GetKeyDown(KeyCode.A))
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    boardInfo[i, j] = Random.Range(-1, 1);
                }
            }
        }
    }
}
