using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;


[InitializeOnLoad]
public static class CreateButtonUi
{
    public static int ID = 0;
    public static Block _block;
    public static List<Block> _blocks;
    public static DefaultScene defaultScene;
    static CreateButtonUi()
    {
        SceneView.duringSceneGui += OnGui;
        SceneView.duringSceneGui += SceneViewOnDuringSceneGui;
        defaultScene = new DefaultScene();
        //ヒエラルキーで選択されたオブジェクトが変更されたときに呼び出される関数を登録
        Selection.selectionChanged += () =>
        {
            if (defaultScene.selectedBlock == null
            && Selection.activeGameObject.GetComponent<Block>() != null)
            {
                Debug.Log("selectedBlock is not null");
                defaultScene.selectedBlock = Selection.activeGameObject.GetComponent<Block>();
            }
            else if (defaultScene.selectedBlock != null
            && Selection.activeGameObject.GetComponent<Block>() != null)
            {
                Debug.Log("swapping selectedBlock and connectedBlock");
                //旧選択対象を接続対象に設定
                defaultScene.connectedBlock = defaultScene.selectedBlock;
                defaultScene.connectedBlock =
                defaultScene.selectedBlock = Selection.activeGameObject.GetComponent<Block>();
            }
            else if (Selection.activeGameObject.GetComponent<Block>() == null)
            {
                defaultScene.selectedBlock = null;
                defaultScene.connectedBlock = null;
            }
        };
        _blocks = new List<Block>();
    }

    private static void OnGui(SceneView sceneView)
    {
        Handles.BeginGUI();
        //if (_block == null)
        //{
        //    _block = LoadDataTable();
        //}
        // ������ UI��`�悷�鏈�����L�q
        ShowButtons(sceneView.position.size);
        ShowInfoPanel();
        Handles.EndGUI();
    }
    private static void ShowInfoPanel()
    {
        var rect = new Rect(10, 10, 200, 100);
        GUI.Box(rect, "current info");
        GUI.Label(new Rect(20, 30, 180, 20), "slecting block id: " + defaultScene.selectedBlock?.ID);
        GUI.Label(new Rect(20, 50, 180, 20), "connected block id: " + defaultScene.connectedBlock?.ID);
        GUI.Label(new Rect(20, 70, 180, 20), "�L�q�̏���");
    }

    /// <summary>
    /// シーンビューのイベントを監視する
    /// シーンビューでのキーイベントはここで登録する
    /// </summary>
    /// <param name="obj"></param>
    private static void SceneViewOnDuringSceneGui(SceneView obj)
    {
        var ev = Event.current;
        if (ev.type == EventType.KeyDown)
        {
            Debug.Log(ev.keyCode);
            if (ev.keyCode == KeyCode.Space)
            {
                Debug.Log("Space key is pressed");
                foreach (var block in _blocks)
                {
                    Debug.Log("[foreach] block id: " + block.ID);
                    block.OnSpaceKeyPress();
                    foreach (var vertices in block.mesh.vertices)
                    {
                        Debug.Log("vertices: " + vertices);
                    }
                }
            }
        }
    }

    /// <summary>
    /// set button
    /// </summary>
    private static void ShowButtons(Vector2 sceneSize)
    {
        var count = 1;
        var buttonSize = 90;

        foreach (var i in Enumerable.Range(0, count))
        {
            //var block = new Block();
            // ボタンサイズ
            var rect = new Rect(
              sceneSize.x / 2 - buttonSize * count / 2 + buttonSize * i,
              sceneSize.y - 60,
              buttonSize,
              40);

            if (GUI.Button(rect, "ブロックを追加"))
            {
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefab/block.prefab");
                if (prefab != null)
                {
                    var obj = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                    Selection.activeObject = obj;
                    obj.name = "block:ID " + ID;
                    Undo.RegisterCreatedObjectUndo(obj, "create object");
                    Debug.Log("Info: gameObject Added. ID is " + ID);
                    //blockプレハブにアタッチされているblock.csにアクセスする
                    var block = obj.GetComponent<Block>();
                    block.ID = ID;
                    ID++;
                    _blocks.Add(block);
                }
                //var pre = AssetDatabase.LoadAssetAtPath<GameObject>("");
                Debug.Log("押された");
            }
        }
    }

}

//private static Block LoadDataTable()
//{
//    return AssetDatabase.LoadAssetAtPath<Block>("Assets/Script/Block.asset");
//}
