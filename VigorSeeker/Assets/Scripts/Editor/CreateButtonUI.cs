using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.Remoting;
using Codice.Client.BaseCommands;
using Codice.CM.Common;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.Shapes;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;


[InitializeOnLoad]
public static class CreateButtonUi
{
    public static int ID = 0;
    public static Block _block;
    public static List<Block> _blocks;
    public static DefaultScene defaultScene;
    public static float _margin = 0.2f;
    public static float blockVallaySize = 4.454382f - 4.378539f;

    public static float margin = 4.378539f - 3.99421f;
    public static int rowSize = 35;
    public static bool isDebug = true;

    static CreateButtonUi()
    {
        SceneView.duringSceneGui += OnGui;
        SceneView.duringSceneGui += SceneViewOnDuringSceneGui;
        defaultScene = new DefaultScene();
        //ヒエラルキーで選択されたオブジェクトが変更されたときに呼び出される関数を登録
        Selection.selectionChanged += () =>
        {
            Debug.Log("selection changed");
            Debug.Log("selection.activeGameObject is " + Selection.gameObjects.Length);
            if (Selection.activeGameObject != null)
            {
                var x = Selection.activeGameObject.GetComponent<Transform>();
                Debug.Log("child count is " + x.childCount);

            }


            if (defaultScene.selectedBlock == null
            && Selection.activeGameObject != null)
            {
                if (Selection.activeGameObject.GetComponent<Block>() != null)
                {
                    Debug.Log("selectedBlock is not null");
                    Debug.Log("selection.activeGameObject is " + Selection.gameObjects.Length);
                    defaultScene.selectedBlock = Selection.activeGameObject.GetComponent<Block>();
                    if (defaultScene.connectedBlock != null)
                    {
                        defaultScene.connectedBlock._isAnimatable = true;
                    }
                    defaultScene.selectedBlock._isAnimatable = false;
                }

            }
            else if (defaultScene.selectedBlock != null && Selection.activeGameObject != null)
            {
                if (Selection.activeObject.GetComponent<Block>() != null)
                {
                    //Debug.Log("swapping selectedBlock and connectedBlock");
                    //旧選択対象を接続対象に設定
                    if (defaultScene.connectedBlock != null)
                    {
                        defaultScene.connectedBlock._isAnimatable = true;
                    }
                    defaultScene.connectedBlock = defaultScene.selectedBlock;
                    defaultScene.selectedBlock = Selection.activeGameObject.GetComponent<Block>();
                    defaultScene.selectedBlock._isAnimatable = false;
                }
            }
            else if (Selection.gameObjects.Length == 0)
            {
                Debug.Log("Yes");
                defaultScene.selectedBlock._isAnimatable = true;
                defaultScene.connectedBlock._isAnimatable = true;
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
        var rect = new Rect(10, 10, 400, 120);
        GUI.Box(rect, "current info");
        GUI.Label(new Rect(20, 30, 180, 20), "slecting block id: " + defaultScene.selectedBlock?.ID);
        GUI.Label(new Rect(20, 50, 180, 20), "connected block id: " + defaultScene.connectedBlock?.ID);
        //GUI.Label(new Rect(20, 70, 180, 20), "spring force" + defaultScene.selectedBlock?._massPoints[2].CalcForce());
        GUI.Label(new Rect(20, 90, 180, 20), "Message " + defaultScene?.isVisible);

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
            /*
            ← : 右脚を左ポケットに
            → : 左脚を右ポケットに
            ↑ : 縦に連結
            */
            if (defaultScene.selectedBlock != null
            && defaultScene.connectedBlock != null)
            {
                if (ev.keyCode == KeyCode.UpArrow)
                {
                    Debug.Log("Up arrow key is pressed");
                    defaultScene.selectedBlock.OnUpKeyPress();
                    defaultScene.connectedBlock.OnUpKeyPress();

                }
                if (ev.keyCode == KeyCode.LeftArrow)
                {
                    Debug.Log("Left arrow key is pressed");
                    defaultScene.selectedBlock.OnLeftKeyPress();
                    defaultScene.connectedBlock.OnLeftKeyPress();
                }
                if (ev.keyCode == KeyCode.RightArrow)
                {
                    Debug.Log("Right arrow key is pressed");
                    defaultScene.selectedBlock.OnRightKeyPress();
                    defaultScene.connectedBlock.OnRightKeyPress();
                    defaultScene.selectedBlock.NewConnectSpring(connectType: ConnectType.Right_ConnectedToSelect);
                    defaultScene.connectedBlock.NewConnectSpring(connectType: ConnectType.Right_ConnectedToSelect);
                }
                if (ev.keyCode == KeyCode.A)
                {
                    Debug.Log("A key is pressed");
                    defaultScene.selectedBlock.OnAKeyPress();
                    defaultScene.connectedBlock.OnAKeyPress();
                    defaultScene.selectedBlock.NewConnectSpring(connectType: ConnectType.Left_SelectToConnected);
                    defaultScene.connectedBlock.NewConnectSpring(connectType: ConnectType.Left_SelectToConnected);
                }
                if (ev.keyCode == KeyCode.D)
                {
                    Debug.Log("D key is pressed");
                    defaultScene.selectedBlock.OnRightKeyPress();
                    defaultScene.connectedBlock.OnRightKeyPress();
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
            var rect3 = new Rect(
            sceneSize.x / 2 - buttonSize * (count) / 2 + buttonSize * (i + 2),
            sceneSize.y - 60,
            buttonSize,
            40);
            var rect4 = new Rect(
            sceneSize.x / 2 - buttonSize * (count) / 2 + buttonSize * (i + 3),
            sceneSize.y - 60,
            buttonSize,
            40);
            var rect5 = new Rect(
            sceneSize.x / 2 - buttonSize * (count) / 2 + buttonSize * (i + 4),
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
                    mesh.RecalculateNormals();
                    //mesh.SetNormals();
                    meshfilter.mesh = mesh;
                    block.mesh = mesh;
                    if (_blocks.Count != 0)
                    {
                        obj.transform.position = _blocks[ID - 1].transform.position + new Vector3(margin * 5, 0, 0);
                    }
                    block.SetVertices();
                    block.defaultScene = defaultScene;
                    block.ID = ID;
                    ID++;
                    _blocks.Add(block);
                }
            }
            if (GUI.Button(rect2, "ブロックに変換"))
            {
                Debug.Log("convert to block");
                if (Selection.gameObjects.Length == 1
                && Selection.activeGameObject.GetComponent<ProBuilderShape>() != null)
                {
                    var shape = Selection.activeGameObject.GetComponents<ProBuilderShape>();
                    var c_Transform = Selection.activeGameObject.transform;
                    Debug.Log("shape size: " + shape.Length);
                    foreach (var s in shape)
                    {
                        Debug.Log("component size: " + s.m_Size);

                    }
                    var row = GetRow(shape[0].m_Size);
                    var column = GetColumn(shape[0].m_Size);
                    if (row == -1 || column == -1)
                    {
                        Debug.Log("Error: row or column is not correct");
                        defaultScene.message = "Error: row or column is not correct";
                    }
                    else
                    {
                        defaultScene.message = "row: " + row + " column: " + column;
                        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefab/blockv1.prefab");
                        var parent = new GameObject("parent");
                        //var scale = AdjustScale(shape[0].m_Size, ref column);
                        if (prefab != null)
                        {
                            for (int c = 0; c < column; c++)
                            {
                                for (int r = 0; r < rowSize; r++)
                                {
                                    var obj = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                                    obj.transform.parent = parent.transform;
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
                                    block.defaultScene = defaultScene;
                                    block._isFixed = true;
                                    ID++;
                                    _blocks.Add(block);
                                    var size = ChangeBlockVallySize(shape[0].m_Size, block);
                                    block.TransformInsertionModel();
                                    var radius = size * rowSize * 2 / (2 * Mathf.PI);
                                    if (radius >= 2.0)
                                    {
                                        radius = radius - 2.0f;
                                    }
                                    else
                                    {
                                        radius = 0.0f;
                                    }
                                    block.transform.position = new Vector3(c_Transform.position.x, c_Transform.position.y - shape[0].m_Size.y / 2 + c * _margin, c_Transform.position.z + radius);
                                    if (c % 2 == 0)
                                    {
                                        block.transform.RotateAround(c_Transform.position, Vector3.up, 360.0f / (float)rowSize * (float)r);
                                        //Debug.Log("rotate around " + 360 / row * r);
                                    }
                                    else
                                    {
                                        block.transform.RotateAround(c_Transform.position, Vector3.up, 360.0f / (float)rowSize * (float)r + 180.0f / (float)rowSize);
                                        Debug.Log("rotate around " + 360 / row * r);
                                    }
                                    if (!isDebug)
                                    {
                                        if (c != 0)
                                        {
                                            //ブロックに差し込む
                                            if (c % 2 == 0)
                                            {
                                                int myIndex = ID - 1;
                                                var rightPocket = _blocks[myIndex - rowSize];
                                                var leftIndex = myIndex - rowSize - 1;
                                                if (r == 0)
                                                {
                                                    leftIndex = myIndex - 1;
                                                }
                                                var leftPocket = _blocks[leftIndex];
                                                block._rightPocketInsertingBlock.Add(rightPocket);
                                                block._leftPocketInsertingBlock.Add(leftPocket);
                                                var spring = obj.AddComponent<Spring>();
                                                var spring2 = obj.AddComponent<Spring>();
                                                var massPoint1 = block._massPoints[2];
                                                var massPoint2 = leftPocket._massPoints[2];
                                                //TODO: distanceは遅いのでmagintudeを使う
                                                var initialLength1 = Vector3.Distance(massPoint1._position, massPoint2._position);
                                                spring.SetSpring(massPoint1, massPoint2,
                                                10.0f, springLength: initialLength1, 20.0f, 1.0f, springType: SpringType.Block);
                                                block._springs.Add(spring);
                                                massPoint1.AddSpring(spring);
                                                massPoint2.AddSpring(spring);
                                                //TODO: springsの追加は本当にこれでOKか？
                                            }
                                            //ブロックに差し込む
                                            if (c % 2 == 1)
                                            {
                                                int myIndex = ID - 1;
                                                var rightIndex = myIndex - rowSize + 1;
                                                var leftIndex = myIndex - rowSize;
                                                if (r == rowSize - 1)
                                                {
                                                    rightIndex = myIndex - rowSize - rowSize + 1;
                                                }
                                                var leftPocket = _blocks[leftIndex];
                                                var rightPocket = _blocks[rightIndex];
                                                block._rightPocketInsertingBlock.Add(rightPocket);
                                                block._leftPocketInsertingBlock.Add(leftPocket);
                                                var spring = obj.AddComponent<Spring>();
                                                var spring2 = obj.AddComponent<Spring>();
                                                var massPoint1 = block._massPoints[5];
                                                var massPoint2 = leftPocket._massPoints[5];
                                                //TODO: distanceは遅いのでmagintudeを使う
                                                var initialLength1 = Vector3.Distance(massPoint1._position, massPoint2._position);
                                                spring.SetSpring(massPoint1, massPoint2,
                                                10.0f, springLength: initialLength1, 20.0f, 1.0f, springType: SpringType.Block);
                                                block._springs.Add(spring);
                                                massPoint1.AddSpring(spring);
                                                massPoint2.AddSpring(spring);
                                                //TODO: springsの追加は本当にこれでOKか？
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

            }
            if (GUI.Button(rect3, "Spring"))
            {
                defaultScene.isVisible = !defaultScene.isVisible;
            }
            if (GUI.Button(rect4, "ground"))
            {
                if (defaultScene.selectedBlock != null)
                {

                }
            }
            if (GUI.Button(rect5, "sky"))
            {
                if (defaultScene.selectedBlock != null)
                {

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

    public static int GetRow(Vector3 size)
    {
        float blockVallaySize = 4.454382f - 4.378539f;
        //2.0 is the size of the block
        if (size.x <= 4.0)
        {
            return -1;
        }
        //真円と仮定して作成する
        float circumference = size.x * Mathf.PI;
        int row = (int)(circumference / blockVallaySize);
        //Debug.Log("row is " + row);
        if (row % 2 == 1)
        {
            row++;
        }
        //row を半分にする
        row = row / 2;
        return row;
    }
    public static int GetColumn(Vector3 size)
    {
        //ブロックの背の高さ
        float blockBackSize = 3.039667f - 1.119001f;
        if (size.y <= blockBackSize)
        {
            return -1;
        }
        float height = size.y;
        //+1は一番下のブロックの分
        int column = (int)((height - blockBackSize) / _margin) + 1;
        return column;
    }


    /// <summary>
    /// ブロックのスケールを調整する(案1)
    /// </summary>
    /// <param name="size">サイズ</param>
    /// <param name="column">カラム</param>
    /// <returns></returns>
    public static float AdjustScale(Vector3 cylinderSize, ref int column)
    {
        Debug.Log("init column is " + column);
        float size = (cylinderSize.x * Mathf.PI / (rowSize * 2));
        Debug.Log("size is " + size + "blockVallaySize is " + blockVallaySize);
        size = size / blockVallaySize;
        float height = cylinderSize.y;
        float blockBackSize = (3.039667f - 1.119001f);
        if (height <= blockBackSize)
        {
            return -1;
        }
        column = (int)((height - size) / _margin) + 1;
        Debug.Log("after column is " + column);
        Debug.Log("size is " + size);
        return size;
    }

    public static float ChangeBlockVallySize(Vector3 cylinderSize, Block block)
    {
        float size = (cylinderSize.x * Mathf.PI / (rowSize * 2));
        Debug.Log("size is " + size + "blockVallaySize is " + blockVallaySize); ;
        Debug.Log("after size is " + size + "blockVallaySize is " + blockVallaySize);
        if (size >= 0.5)
        {
            Debug.Log("!!!!!!!!!!CAUTION!!!!!!!!!! size is too big");
        }
        float diff = size - blockVallaySize;
        if (diff > 0.0)
        {
        }
        Debug.Log("diff is " + diff);
        block.UpdateValleySize(diff);
        return size;
    }
}

//private static Block LoadDataTable()
//{
//    return AssetDatabase.LoadAssetAtPath<Block>("Assets/Script/Block.asset");
//}
