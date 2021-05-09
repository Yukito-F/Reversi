using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [SerializeField]
    Texture2D defaultCursor = null;

    Camera mainCamera;
    RaycastHit hit;
    GameObject targetObject;

    BoardManager board;

    void Start()
    {
        mainCamera = Camera.main;
        Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
        board = GameObject.Find("BoardManager").GetComponent<BoardManager>();
    }
    void Update()
    {
        // �}�E�X�̍��{�^�����������ꂽ��ADisc��z�u
        if (Input.GetMouseButtonDown(0))
        {
            clicked();
            return;
        }
        CastRay();
    }

    // �}�E�X�J�[�\���̈ʒu����u���C�v���΂��āA�����̃R���C�_�[�ɓ����邩�ǂ������`�F�b�N
    void CastRay()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity))
        {
            targetObject = hit.collider.gameObject;
            if (targetObject.name == "Cursor(Clone)")
            {
                transform.position = targetObject.transform.position;
            }
        }
        else
        {
            targetObject = null;
            transform.position = new Vector3(0, -1, 0);
        }
    }

    // Disc�̔z�u����
    void clicked()
    {
        if (targetObject == null)
        {
            return;
        }
        board.put((int)(transform.position.x + 3.5f), (int)(transform.position.z + 3.5f));
    }
}
