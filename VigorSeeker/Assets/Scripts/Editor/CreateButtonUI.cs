using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.Shapes;


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
        GUI.Label(new Rect(20, 70, 180, 20), "spring force" + defaultScene.selectedBlock?._massPoints[2].CalcForce());
        GUI.Label(new Rect(20, 90, 180, 20), "spring force" + defaultScene.selectedBlock?._massPoints[2]._position);

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
            if (ev.keyCode == KeyCode.Space)
            {
                Debug.Log("Space key is pressed");
                foreach (var block in _blocks)
                {
                    Debug.Log("[foreach] block id: " + block.ID);
                    block.OnSpaceKeyPress();
                }
            }
            if (ev.keyCode == KeyCode.LeftArrow)
            {
                Debug.Log("Left arrow key is pressed");
                foreach (var block in _blocks)
                {
                    Debug.Log("[foreach] block id: " + block.ID);
                    block.OnLeftKeyPress();
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
            var rect2 = new Rect(
              sceneSize.x / 2 - buttonSize * (count) / 2 + buttonSize * (i + 1),
              sceneSize.y - 60,
              buttonSize,
              40);

            if (GUI.Button(rect, "ブロックを追加"))
            {
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefab/blockv1.prefab");
                if (prefab != null)
                {
                    var obj = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                    var vertices = CreateMeshVertices(ReadObjFile("block"));
                    var triangles = CreateTriangles(ReadObjFile("block"));
                    Selection.activeObject = obj;
                    obj.name = "block:ID " + ID;
                    Undo.RegisterCreatedObjectUndo(obj, "create object");
                    Debug.Log("Info: gameObject Added. ID is " + ID);
                    //blockプレハブにアタッチされているblock.csにアクセスする
                    var block = obj.GetComponent<Block>();
                    var meshfilter = obj.GetComponent<MeshFilter>();
                    var mesh = new Mesh();
                    mesh.SetVertices(vertices);
                    mesh.SetTriangles(triangles, 0);
                    //mesh.SetNormals();
                    meshfilter.mesh = mesh;
                    block.mesh = mesh;
                    block.SetVertices();
                    block.ID = ID;
                    ID++;
                    _blocks.Add(block);
                }
            }
            if (GUI.Button(rect2, "ブロックに変換"))
            {
                Debug.Log("convert to block");
                Debug.Log("gameobjects length: " + Selection.gameObjects.Length);
                var components = Selection.activeGameObject.GetComponents<Component>();
                Debug.Log("components length: " + components.Length);
                if (Selection.gameObjects.Length == 1
                && Selection.activeGameObject.GetComponent<ProBuilderShape>() != null)
                {
                    var shape = Selection.activeGameObject.GetComponents<ProBuilderShape>();
                    Debug.Log("shape size: " + shape.Length);
                    foreach (var s in shape)
                    {
                        Debug.Log("component size: " + s.m_Size);

                    }
                }

            }
        }
    }
    public static string[] ReadObjFile(string fileName)
    {
        string texts = (Resources.Load(fileName, typeof(TextAsset)) as TextAsset).text;
        string[] lines = texts.Split('\n');
        return lines;
    }
    public static List<Vector3> CreateMeshVertices(string[] lines)
    {
        var vertices = new List<Vector3>();
        foreach (var line in lines)
        {
            if (line.StartsWith("v"))
            {
                var v = line.Split(' ');
                if (v[0] == "v")
                {
                    vertices.Add(new Vector3(float.Parse(v[1]), float.Parse(v[2]), float.Parse(v[3])));
                }
            }
        }

        return vertices;
    }
    /// <summary>
    /// 三角形のインデックスを格納する
    /// </summary>
    /// <param name="lines">objファイルの中身</param>
    /// <returns>三角形インデックスのリスト</returns>
    public static List<int> CreateTriangles(string[] lines)
    {
        var triangles = new List<int>();
        foreach (var line in lines)
        {
            if (line.StartsWith("f"))
            {
                var f = line.Split(' ');
                if (f[0] == "f" && f.Length == 4)
                {
                    var f1 = f[1].Split('/');
                    var f2 = f[2].Split('/');
                    var f3 = f[3].Split('/');
                    //unityの仕様上、triangleのインデックスを時計周りで格納する必要がある
                    //objファイルは反時計回りでインデックスが格納される
                    triangles.Add(int.Parse(f3[0]) - 1);
                    triangles.Add(int.Parse(f2[0]) - 1);
                    triangles.Add(int.Parse(f1[0]) - 1);
                }
            }
        }
        return triangles;
    }
}

//private static Block LoadDataTable()
//{
//    return AssetDatabase.LoadAssetAtPath<Block>("Assets/Script/Block.asset");
//}
