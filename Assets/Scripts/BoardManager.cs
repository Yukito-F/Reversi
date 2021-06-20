using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    private int[,] boardInfo = new int[8, 8];
    private GameObject[,] discList = new GameObject[8, 8];
    private CursorController[,] cursorList = new CursorController[8, 8];

    private List<int[]>[,] expectedTable = new List<int[]>[8, 8];
    private int turn = 1; // 1:Black, -1:White

    private CanvasController boardCounter;
    // 1:Black 0:void -1:White
    private void Start()
    {
        boardCounter = GameObject.Find("CanvasController").GetComponent<CanvasController>();
        
        GameObject disc = Resources.Load("Disc") as GameObject;
        GameObject cursor = Resources.Load("Cursor") as GameObject;

        // �R�}�A�}�X�̃C���X�^���X����
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                boardInfo[i, j] = 0;
                discList[i, j] = Instantiate(disc,
                                             new Vector3(-3.5f + j, 0.5f, -3.5f + i),
                                             Quaternion.identity);
                discList[i, j].SetActive(false);
                var _cursor = Instantiate(cursor,
                                          new Vector3(-3.5f + j, 0, -3.5f + i),
                                          Quaternion.identity);
                cursorList[i, j] = _cursor.GetComponent<CursorController>();
            }
        }

        // ������4�}�X�̐ݒ�
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

        // ����p�̐ݒu�\�T��
        checkBoard(-1);
        boardCounter.reload(boardInfo, turn);
    }

    public void put(int row, int column)
    {
        // �C�ӂ̃}�X��turn�̒l�ɕύX(��or���̔z�u)
        boardInfo[row, column] = turn;

        // �R�}�̔z�u����
        GameObject disc = discList[row, column];
        if (!disc.activeSelf) disc.SetActive(true);
        if (turn == -1) disc.transform.Rotate(180, 0, 0);

        // �R�}��u�����ۂɂЂ�����Ԃ�����i�[�����z��expectedTable�ɋL�q����Ă���R�}�𔽓]
        foreach (int[] index in expectedTable[row, column])
        {
            discList[index[0], index[1]].GetComponent<Disc>().reload(turn);
            boardInfo[index[0], index[1]] = turn;
        }

        // �ՖʒT��(�ݒu�\����)
        if (checkBoard(turn))
        {
            // �X�L�b�v�����p�Ƀ^�[������ꊷ���čēx�T��
            turn *= -1;
            if (checkBoard(turn))
            {
                // ��A���X�L�b�v�ŏI��(���݂̓��O�̂݁B�䂭�䂭�̓V�[���؂�ւ�)
                Debug.Log("END");
            }
        };

        // �ʏ�̃^�[���؂�ւ�
        turn *= -1;
        boardCounter.reload(boardInfo, turn);

        // ������ꏊ����}�X���������Ɏ����I�ɐݒu
        List<int[]> indexes = new List<int[]>();
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (cursorList[i, j].gameObject.name == "Enable")
                {
                    indexes.Add(new int[] { i, j });
                }
            }
        }
        if (indexes.Count == 1)
        {
            Debug.Log("AutoPut");
            StartCoroutine(DelayMethod(2.0f, () =>
            {
                put(indexes[0][0], indexes[0][1]);
            }));
        }
    }

    // �ՖʒT��(�ݒu�\����)�A�Ԃ�l:�ݒu�\�ȃ}�X���ꂩ���ł������false
    private bool checkBoard(int enemy)
    {
        // �Ԃ�l�p
        bool exist = true;

        // �e�}�X���ƂɂЂ�����Ԃ�����i�[����z��
        expectedTable = new List<int[]>[8, 8];
        // �Ֆʂ̑S�T��(8�~8)
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                // �C�ӂ̃}�X���󂩂ǂ����̔���
                if (boardInfo[i, j] == 0)
                {
                    // �C�ӂ̃}�X��8�����̒T���A�Ђ�����Ԃ���R�}�̌�₪�Ԃ��Ă���
                    var tempList = checkRadiation(i, j, enemy);
                    // �Ђ�����Ԃ��₪���݂���ꍇ�B�܂�ݒu�\�ȃ}�X�ł���ꍇ
                    if (tempList != null)
                    {
                        // �e�}�X���ƂɂЂ�����Ԃ�����i�[
                        expectedTable[i, j] = tempList;
                        // �ݒu�\�ȃ}�X�����݂���̂�false�ɕύX
                        exist = false;
                    }
                }
                // �F�̕ύX
                cursorList[i, j].changeColor(expectedTable[i, j] != null);
            }
        }
        // �ݒu�\�ȃ}�X�����݂��邩�Ԃ��Btrue:�Ȃ��Afalse:����
        return exist;
    }

    // �C�ӂ̃}�X��8�����T��
    private List<int[]> checkRadiation(int row, int column, int enemy)
    {
        // �Ђ�����Ԃ���̔z����i�[���郊�X�g
        List<int[]> expectedRadiation = new List<int[]>();
        // �ݒu�\���ǂ������i�[����ϐ�
        bool exist = false;

        // 8�����̒T��
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                // �ՖʊO�ł͂Ȃ� and �T���Ώۃ}�X���G�R�} and �ݒu��������Ă���}�X�ł͂Ȃ��ꍇ
                if (outDetect(row + i, column + j) && (i != 0 || j != 0) && boardInfo[row + i, column + j] == enemy)
                {
                        // �G�A���R�}�̐�Ɏ��R�}�����݂��邩�̒T���A�Ђ�����Ԃ���R�}�̌�₪�Ԃ��Ă���
                        var tempList = checkLine(row, column, i, j, enemy);
                    // �Ђ�����Ԃ��₪���݂���ꍇ�B�܂�ݒu�\�ȃ}�X�ł���ꍇ
                    if (tempList != null)
                    {
                        // �ݒu�\�Ȃ̂�true�ɕύX
                        exist = true;
                        // �Ԃ��Ă������̃��X�g����̃��X�g�Ƃ��ēZ�߂�Bint[] �~ n => int[] 
                        foreach (int[] item in tempList)
                        {
                            expectedRadiation.Add(item);
                        }
                    }
                }
            }
        }

        // 8���������ꂩ�̕����ɂ��Đݒu�\�ł���΁A�Ђ�����Ԃ����Ԃ�
        if (exist)
        {
            return expectedRadiation;
        }
        // �ݒu�s�ł���΁Anull��Ԃ�
        return null;
    }

    // �G�A���R�}�̐�Ɏ��R�}�����݂��邩�̒T��
    private List<int[]> checkLine(int row, int column, int up, int right, int enemy)
    {
        // �Ђ�����Ԃ���̔z����i�[���郊�X�g
        List<int[]> expectedLine = new List<int[]>();

        // ���͂��ꂽ�����ɉ����ăC���f�b�N�X�̏����l�̐ݒ�
        int rowCursor = row + up;
        int columnCursor = column + right;

        // �A������
        // �T���Ώ̃}�X���ՖʊO�ł͂Ȃ� and �G�R�}�̊Ԃ�����
        while (outDetect(rowCursor, columnCursor) && boardInfo[rowCursor, columnCursor] == enemy)
        {
            // �ʉ߂����}�X�̃C���f�b�N�X�z������X�g�ɒǉ�
            expectedLine.Add(new int[] { rowCursor, columnCursor });
            // �J�[�\���̉��Z
            rowCursor += up;
            columnCursor += right;
            // ���R�}�����݂���Β~�ς��ꂽ���X�g��Ԃ�
            if (outDetect(rowCursor, columnCursor) && boardInfo[rowCursor, columnCursor] == -enemy)
            {
                return expectedLine;
            }
        }
        // ���R�}��������Ȃ����null��Ԃ�
        return null;
    }

    // �Ֆʂ̊O�ɏo�Ă��Ȃ����̔���
    private bool outDetect(int row, int column)
    {
        return (Mathf.Abs(3.5f - row) < 4 && Mathf.Abs(3.5f - column) < 4);
    }

    private IEnumerator DelayMethod(float waitTime, Action action)
    {
        yield return new WaitForSeconds(waitTime);
        action();
    }
}