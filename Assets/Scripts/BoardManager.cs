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

        // コマ、マスのインスタンス生成
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

        // 初期の4マスの設定
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

        // 初回用の設置可能探索
        checkBoard(-1);
        boardCounter.reload(boardInfo, turn);
    }

    public void put(int row, int column)
    {
        // 任意のマスをturnの値に変更(白or黒の配置)
        boardInfo[row, column] = turn;

        // コマの配置処理
        GameObject disc = discList[row, column];
        if (!disc.activeSelf) disc.SetActive(true);
        if (turn == -1) disc.transform.Rotate(180, 0, 0);

        // コマを置いた際にひっくり返る候補を格納した配列expectedTableに記述されているコマを反転
        foreach (int[] index in expectedTable[row, column])
        {
            discList[index[0], index[1]].GetComponent<Disc>().reload(turn);
            boardInfo[index[0], index[1]] = turn;
        }

        // 盤面探索(設置可能判定)
        if (checkBoard(turn))
        {
            // スキップ処理用にターンを入れ換えて再度探索
            turn *= -1;
            if (checkBoard(turn))
            {
                // 二連続スキップで終了(現在はログのみ。ゆくゆくはシーン切り替え)
                Debug.Log("END");
            }
        };

        // 通常のターン切り替え
        turn *= -1;
        boardCounter.reload(boardInfo, turn);

        // おける場所が一マスだった時に自動的に設置
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

    // 盤面探索(設置可能判定)、返り値:設置可能なマスが一か所でもあればfalse
    private bool checkBoard(int enemy)
    {
        // 返り値用
        bool exist = true;

        // 各マスごとにひっくり返る候補を格納する配列
        expectedTable = new List<int[]>[8, 8];
        // 盤面の全探索(8×8)
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                // 任意のマスが空かどうかの判定
                if (boardInfo[i, j] == 0)
                {
                    // 任意のマスの8方向の探索、ひっくり返せるコマの候補が返ってくる
                    var tempList = checkRadiation(i, j, enemy);
                    // ひっくり返る候補が存在する場合。つまり設置可能なマスである場合
                    if (tempList != null)
                    {
                        // 各マスごとにひっくり返る候補を格納
                        expectedTable[i, j] = tempList;
                        // 設置可能なマスが存在するのでfalseに変更
                        exist = false;
                    }
                }
                // 色の変更
                cursorList[i, j].changeColor(expectedTable[i, j] != null);
            }
        }
        // 設置可能なマスが存在するか返す。true:ない、false:ある
        return exist;
    }

    // 任意のマスの8方向探索
    private List<int[]> checkRadiation(int row, int column, int enemy)
    {
        // ひっくり返る候補の配列を格納するリスト
        List<int[]> expectedRadiation = new List<int[]>();
        // 設置可能かどうかを格納する変数
        bool exist = false;

        // 8方向の探索
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                // 盤面外ではない and 探索対象マスが敵コマ and 設置判定をしているマスではない場合
                if (outDetect(row + i, column + j) && (i != 0 || j != 0) && boardInfo[row + i, column + j] == enemy)
                {
                        // 敵連続コマの先に自コマが存在するかの探索、ひっくり返せるコマの候補が返ってくる
                        var tempList = checkLine(row, column, i, j, enemy);
                    // ひっくり返る候補が存在する場合。つまり設置可能なマスである場合
                    if (tempList != null)
                    {
                        // 設置可能なのでtrueに変更
                        exist = true;
                        // 返ってきた候補のリストを一つのリストとして纏める。int[] × n => int[] 
                        foreach (int[] item in tempList)
                        {
                            expectedRadiation.Add(item);
                        }
                    }
                }
            }
        }

        // 8方向いずれかの方向について設置可能であれば、ひっくり返る候補を返す
        if (exist)
        {
            return expectedRadiation;
        }
        // 設置不可であれば、nullを返す
        return null;
    }

    // 敵連続コマの先に自コマが存在するかの探索
    private List<int[]> checkLine(int row, int column, int up, int right, int enemy)
    {
        // ひっくり返る候補の配列を格納するリスト
        List<int[]> expectedLine = new List<int[]>();

        // 入力された方向に応じてインデックスの初期値の設定
        int rowCursor = row + up;
        int columnCursor = column + right;

        // 連続判定
        // 探索対称マスが盤面外ではない and 敵コマの間ずっと
        while (outDetect(rowCursor, columnCursor) && boardInfo[rowCursor, columnCursor] == enemy)
        {
            // 通過したマスのインデックス配列をリストに追加
            expectedLine.Add(new int[] { rowCursor, columnCursor });
            // カーソルの加算
            rowCursor += up;
            columnCursor += right;
            // 自コマが存在すれば蓄積されたリストを返す
            if (outDetect(rowCursor, columnCursor) && boardInfo[rowCursor, columnCursor] == -enemy)
            {
                return expectedLine;
            }
        }
        // 自コマが見つからなければnullを返す
        return null;
    }

    // 盤面の外に出ていないかの判定
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