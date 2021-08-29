using Photon.Pun;
using UnityEngine;

public class SetPosition : MonoBehaviourPunCallbacks
{
    BoardManager board;

    private void Start()
    {
        board = GameObject.Find("BoardManager").GetComponent<BoardManager>();

        var pos = new Vector3(-5.5f, 0, -2);
        if (photonView.OwnerActorNr == 2)
        {
            pos.x = 5.5f;
        }
        transform.position = pos;
    }
}