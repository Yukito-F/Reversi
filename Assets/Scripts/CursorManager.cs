using Photon.Pun;
using UnityEngine;

public class CursorManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    Texture2D defaultCursor = null;

    Camera mainCamera;
    RaycastHit hit;
    GameObject targetObject;

    private void Start()
    {
        mainCamera = Camera.main;
        Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
    }
    private void Update()
    {
        // マウスの左ボタンが押下されたら、Discを配置
        if (Input.GetMouseButtonDown(0))
        {
            clicked();
            return;
        }
        CastRay();
    }

    // マウスカーソルの位置から「レイ」を飛ばして、何かのコライダーに当たるかどうかをチェック
    private void CastRay()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity))
        {
            targetObject = hit.collider.gameObject;
            if (targetObject.name == "Enable")
            {
                transform.position = targetObject.transform.position;
            }
            else
            {
                targetObject = null;
                transform.position = new Vector3(0, -1, 0);
            }
        }
        else
        {
            targetObject = null;
            transform.position = new Vector3(0, -1, 0);
        }
    }

    // Discの配置処理
    private void clicked()
    {
        if (targetObject == null)
        {
            return;
        }
        int row = (int)(transform.position.z + 3.5f);
        int column = (int)(transform.position.x + 3.5f);

        PhotonNetwork.LocalPlayer.PosUpdate(row, column);
    }
}
