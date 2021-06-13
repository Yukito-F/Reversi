using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    public Text brack, white;
    public Text brackTurn, whiteTurn;

    // 個数のカウントとターン表示の切り替え
    public void reload(int[,] boardInfo, int turn)
    {
        int brackCount = 0, whiteCount = 0;
        for(int i = 0; i < 8; i++)
        {
            for(int j = 0; j < 8; j++)
            {
                if (boardInfo[i, j] == 1) brackCount++;
                if (boardInfo[i, j] == -1) whiteCount++;
            }
        }
        brack.text = brackCount.ToString();
        white.text = whiteCount.ToString();

        if(turn == 1)
        {
            brackTurn.text = "✔";
            whiteTurn.text = "";
        }
        else
        {
            brackTurn.text = "";
            whiteTurn.text = "✔";
        }
    }
}
