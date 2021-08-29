using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class CustomPropertiesCallbacks: MonoBehaviourPunCallbacks
{
    int row, column;

    BoardManager board;

    private void Start()
    {
        board = GameObject.Find("BoardManager").GetComponent<BoardManager>();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        Debug.Log($"{targetPlayer.NickName}({targetPlayer.ActorNumber})");

        foreach (var prop in changedProps)
        {
            Debug.Log($"{prop.Key}: {prop.Value}");
            if (prop.Key.ToString() == "X") row = (int)prop.Value;
            if (prop.Key.ToString() == "Y") column = (int)prop.Value;
        }

        if ((int)targetPlayer.ActorNumber == board.getTurn())
        {
            board.put(row, column);
        }
    }
}