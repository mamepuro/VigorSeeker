using System.Linq;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class CreateButtonUi
{
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
            }
        }
    }

    /// <summary>
    /// �{�^���̕`��֐�
    /// </summary>
    private static void ShowButtons(Vector2 sceneSize)
    {
        var count = 10;
        var buttonSize = 40;

        foreach (var i in Enumerable.Range(0, count))
        {
            //var block = new Block();
            // ��ʉ����A�����A�����񂹂��R���g���[������ Rect
            var rect = new Rect(
              sceneSize.x / 2 - buttonSize * count / 2 + buttonSize * i,
              sceneSize.y - buttonSize * 1.6f,
              buttonSize,
              buttonSize);

            if (GUI.Button(rect, i.ToString()))
                //{
                //    var go = (Block)PrefabUtility.InstantiatePrefab(_block);
                //    Selection.activeObject = go;
                //    Undo.RegisterCreatedObjectUndo(go, "create object");
                //var pre = AssetDatabase.LoadAssetAtPath<GameObject>("")
                //Block block = PrefabUtility.InstantiatePrefab(block) as Block;
                Debug.Log("押された");
        }
    }

}

//private static Block LoadDataTable()
//{
//    return AssetDatabase.LoadAssetAtPath<Block>("Assets/Script/Block.asset");
//}
