using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;
//using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Specialized;

[ExecuteAlways]
public class Block : MonoBehaviour
{
    [SerializeField] public Mesh mesh;
    [SerializeField] List<Vector3> v;
    [SerializeField] public int ID;
    /// <summary>
    /// 新しく頂点を追加するための一時的な頂点リスト
    /// </summary>
    [SerializeField] Vector3[] _tmpVertices;
    [SerializeField] public List<Spring> _springs;
    [SerializeField] public List<MassPoint> _massPoints;
    [SerializeField] public bool _isFixed = true;
    /// <summary>
    /// シーンマネージャーへの参照
    /// </summary>
    DefaultScene defaultScene;
    const int _leftLegIndex = 2;
    const int _rightLegIndex = 5;
    /// <summary>
    /// ブロックの頂点間に貼るバネの初期インデックス
    /// </summary>
    readonly int[,] _initialSpringIndex = { { 0, 1 }, { 1, 2 }, { 2, 0 }, { 1, 4 }, { 0, 3 }, { 3, 4 }, { 4, 5 }, { 5, 3 } };
    readonly int[,] _legSpring = { { 2, 5 } };
    /// <summary>
    /// 左足を挿入しているブロック
    /// </summary>
    [SerializeField] public Block _leftLegInsertedBlock;

    /// <summary>
    /// 右足を挿入しているブロック
    /// </summary>
    [SerializeField] public Block _rightLegInsertedBlock;

    /// <summary>
    /// 左ポケットに足を挿入しているブロック
    /// </summary>
    [SerializeField] public List<Block> _leftPocketInsertingBlock;

    /// <summary>
    /// 右ポケットに足を挿入しているブロック
    /// </summary>
    [SerializeField] public List<Block> _rightPocketInsertingBlock;
    /// <summary>
    /// ブロックが他のブロックに対して接続可能か
    /// </summary>
    [SerializeField] public bool _isConnectable;

    const float _dampingConstant = 0;
    const float _springConstant = 10.0f;
    const float _restLength = 0.1f;
    bool _isDebug = true;
    public void OnEnable()
    {
        // mesh = GetComponent<MeshFilter>().sharedMesh;
        // v = new List<Vector3>();
        // foreach (Vector3 v3 in mesh.vertices)
        // {
        //     v.Add(transform.TransformPoint(v3));
        // }
        // Debug.Log("Info: Block awaked. ID is " + ID);
        // // Debug.Log("Now" + DateTime.Now);
        // var triangle = mesh.triangles;
        // Debug.Log("triangle.Length is " + triangle.Length);
        // foreach (var x in triangle)
        // {
        //     Debug.Log("triangle is " + x);
        // }
        _leftPocketInsertingBlock = new List<Block>();
        _rightPocketInsertingBlock = new List<Block>();
        //頂点数は6固定
        _tmpVertices = new Vector3[6];
    }
    /// <summary>
    /// 毎フレームレンダリングする
    /// </summary>
    private void OnRenderObject()
    {
        if (!Application.isPlaying)
        {

            EditorApplication.QueuePlayerLoopUpdate();
            SceneView.RepaintAll();

        }
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        UpdateVertices();
    }
    public void SetVertices()
    {
        if (this.mesh != null)
        {
            mesh = GetComponent<MeshFilter>().sharedMesh;
            v = new List<Vector3>();
            foreach (Vector3 v3 in mesh.vertices)
            {
                v.Add(transform.TransformPoint(v3));
            }
            //Debug.Log("Info: Block awaked. ID is " + ID);
            Initiate();
        }
    }
    /// <summary>
    /// ブロックを挿入時のモデルに変換する
    /// </summary>
    public void TransformInsertionModel()
    {
        Debug.Log("TransformInsertionModel is called");
        Debug.Log("On Space key is pressed");
        int i = 0;
        foreach (var vertex in mesh.vertices)
        {
            _tmpVertices[i] = vertex;
            i++;
        }
        //testbending
        _tmpVertices[2] = new Vector3(_tmpVertices[0].x, _tmpVertices[2].y, _tmpVertices[2].z);
        _tmpVertices[5] = new Vector3(_tmpVertices[3].x, _tmpVertices[5].y, _tmpVertices[5].z);
        mesh.SetVertices(_tmpVertices);
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        if (_massPoints.Count != 0)
        {
            _massPoints[2]._position = new Vector3(_massPoints[0]._position.x, _massPoints[2]._position.y, _massPoints[2]._position.z);
            _massPoints[5]._position = new Vector3(_massPoints[3]._position.x, _massPoints[5]._position.y, _massPoints[5]._position.z);
            foreach (var spring in _springs)
            {
                //バネの張り直し
                if (spring._massPointIndexes.Contains(2) && spring._massPointIndexes.Contains(5))
                {
                    spring._springLength = Vector3.Distance(_massPoints[2]._position, _massPoints[5]._position);
                }
            }
        }
    }
    public void OnSpaceKeyPress()
    {
        Debug.Log("On Space key is pressed");
        int i = 0;
        foreach (var vertex in mesh.vertices)
        {
            _tmpVertices[i] = vertex;
            i++;
        }
        //testbending
        _tmpVertices[2] = new Vector3(_tmpVertices[2].x + 0.1f, _tmpVertices[2].y, _tmpVertices[2].z);
        _massPoints[2]._position = new Vector3(_massPoints[2]._position.x + 0.1f, _massPoints[2]._position.y, _massPoints[2]._position.z);
        _tmpVertices[5] = new Vector3(_tmpVertices[5].x - 0.1f, _tmpVertices[5].y, _tmpVertices[5].z);
        _massPoints[5]._position = new Vector3(_massPoints[5]._position.x - 0.1f, _massPoints[5]._position.y, _massPoints[5]._position.z);
        mesh.SetVertices(_tmpVertices);
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        //Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        //UpdateVertices();
    }
    public void OnLeftKeyPress()
    {
        Debug.Log("On Left key is pressed");
        this._isFixed = false;
    }
    void UpdateVertices()
    {
        if (_massPoints.Count == 0 || _isDebug)
        {
            //Debug.Log("massPoints.Count is 0");
            v.Clear();
            foreach (Vector3 v3 in mesh.vertices)
            {
                v.Add(transform.TransformPoint(v3));
            }
        }
        else
        {
            v.Clear();
            foreach (var m in _massPoints)
            {
                v.Add(m._position);
                //ワールド座標からローカル座標に変換する
                _tmpVertices[m._index] = transform.InverseTransformPoint(m._position);
            }
            mesh.SetVertices(_tmpVertices);
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
        }

    }
    void Initiate()
    {
        //リストの初期化
        _springs = new List<Spring>();
        _massPoints = new List<MassPoint>();
        //Debug.Log(v.Count);
        for (int i = 0; i < v.Count; i++)
        {
            var massPoint = gameObject.AddComponent<MassPoint>();
            massPoint.SetMassSpring(30.0f, Vector3.zero, i, v[i], this);
            _massPoints.Add(massPoint);
        }
        for (int i = 0; i < _initialSpringIndex.GetLength(0); i++)
        {
            var spring = gameObject.AddComponent<Spring>();
            var massPoint1 = _massPoints[_initialSpringIndex[i, 0]];
            var massPoint2 = _massPoints[_initialSpringIndex[i, 1]];
            //TODO: distanceは遅いのでmagintudeを使う
            var initialLength = Vector3.Distance(massPoint1._position, massPoint2._position);
            spring.SetSpring(massPoint1, massPoint2,
            10.0f, springLength: initialLength, 20.0f, 1.0f, springType: SpringType.Leg);
            _springs.Add(spring);
            massPoint1.AddSpring(spring);
            massPoint2.AddSpring(spring);
        }
        for (int i = 0; i < _legSpring.GetLength(0); i++)
        {
            var spring = gameObject.AddComponent<Spring>();
            var massPoint1 = _massPoints[_legSpring[i, 0]];
            var massPoint2 = _massPoints[_legSpring[i, 1]];
            var initialLength = Vector3.Distance(massPoint1._position, massPoint2._position);
            spring.SetSpring(massPoint1, massPoint2,
            10.0f, springLength: initialLength, 20.0f, 1.0f, springType: SpringType.Leg);
            _springs.Add(spring);
            massPoint1.AddSpring(spring);
            massPoint2.AddSpring(spring);
        }
    }
    public void UpdateValleySize(float diff)
    {
        int i = 0;
        foreach (var vertex in mesh.vertices)
        {
            _tmpVertices[i] = vertex;
            i++;
        }
        //testbending
        var extra = diff / 2;
        _tmpVertices[0] = new Vector3(_tmpVertices[0].x - extra, _tmpVertices[0].y, _tmpVertices[0].z);
        _massPoints[0]._position = new Vector3(_massPoints[0]._position.x - extra, _massPoints[0]._position.y, _massPoints[0]._position.z);
        _tmpVertices[1] = new Vector3(_tmpVertices[1].x - extra, _tmpVertices[1].y, _tmpVertices[1].z);
        _massPoints[1]._position = new Vector3(_massPoints[1]._position.x - extra, _massPoints[1]._position.y, _massPoints[1]._position.z);
        _tmpVertices[2] = new Vector3(_tmpVertices[2].x - extra, _tmpVertices[2].y, _tmpVertices[2].z);
        _massPoints[2]._position = new Vector3(_massPoints[2]._position.x - extra, _massPoints[2]._position.y, _massPoints[2]._position.z);
        _tmpVertices[3] = new Vector3(_tmpVertices[3].x + extra, _tmpVertices[3].y, _tmpVertices[3].z);
        _massPoints[3]._position = new Vector3(_massPoints[3]._position.x + extra, _massPoints[3]._position.y, _massPoints[3]._position.z);
        _tmpVertices[4] = new Vector3(_tmpVertices[4].x + extra, _tmpVertices[4].y, _tmpVertices[4].z);
        _massPoints[4]._position = new Vector3(_massPoints[4]._position.x + extra, _massPoints[4]._position.y, _massPoints[4]._position.z);
        _tmpVertices[5] = new Vector3(_tmpVertices[5].x + extra, _tmpVertices[5].y, _tmpVertices[5].z);
        _massPoints[5]._position = new Vector3(_massPoints[5]._position.x + extra, _massPoints[5]._position.y, _massPoints[5]._position.z);
        mesh.SetVertices(_tmpVertices);
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        foreach (var spring in _springs)
        {
            //バネの張り直し
            spring._springLength = Vector3.Distance(_massPoints[spring._massPointIndexes[0]]._position, _massPoints[spring._massPointIndexes[1]]._position);
        }
        //Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        //UpdateVertices();
    }
}
