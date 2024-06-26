using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class CreateButtonUi
{
    public static int ID = 0;
    public static Block _block;
    static CreateButtonUi()
    {
        SceneView.duringSceneGui += OnGui;
        SceneView.duringSceneGui += SceneViewOnDuringSceneGui;
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

        Handles.EndGUI();
    }

    private static void SceneViewOnDuringSceneGui(SceneView obj)
    {
        var ev = Event.current;
        if (ev.type == EventType.KeyDown)
        {
            Debug.Log(ev.keyCode);
            if (ev.keyCode == KeyCode.Space)
            {
                Debug.Log(Selection.gameObjects.Length);
                //var b = (Block)Selection.gameObjects[0];
                //b
                //var b = (Selection.gameObjects[0] as Block).GetComponent<Block>();
                //var b = Selection.activeGameObject
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
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefab/blockv3.prefab");
                if (prefab != null)
                {
                    Debug.Log("prefab is null");
                    var obj = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                    Selection.activeObject = obj;
                    obj.name = "block:ID " + ID;
                    Undo.RegisterCreatedObjectUndo(obj, "create object");
                    Debug.Log("Info: gameObject Added. ID is " + ID);
                    //blockプレハブにアタッチされているblock.csにアクセスする
                    var block = obj.GetComponent<Block>();
                    block.ID = ID;
                    ID++;
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
