using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;
//using UnityEditor.SearchService;
using UnityEngine;

[ExecuteAlways]
public class Block : MonoBehaviour
{
    [SerializeField] public Mesh mesh;
    [SerializeField] List<Vector3> v;
    [SerializeField] public int ID;
    static int col = 0;
    /// <summary>
    /// 新しく頂点を追加するための一時的な頂点リスト
    /// </summary>
    [SerializeField] List<Vector3> _tmpVertices;
    Spring spring;
    /// <summary>
    /// シーンマネージャーへの参照
    /// </summary>
    DefaultScene defaultScene;
    const int _leftLegIndex = 1;
    const int _rightLegIndex = 5;
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

    public void OnEnable()
    {
        mesh = GetComponent<MeshFilter>().sharedMesh;
        v = new List<Vector3>();
        foreach (Vector3 v3 in mesh.vertices)
        {
            v.Add(transform.TransformPoint(v3));
        }
        Debug.Log("Info: Block awaked. ID is " + ID);
        // Debug.Log("Now" + DateTime.Now);
        var triangle = mesh.triangles;
        Debug.Log("triangle.Length is " + triangle.Length);
        foreach (var x in triangle)
        {
            Debug.Log("triangle is " + x);
        }
        _leftPocketInsertingBlock = new List<Block>();
        _rightPocketInsertingBlock = new List<Block>();
        _tmpVertices = new List<Vector3>();
    }
    /// <summary>
    /// �I�u�W�F�N�g�������_�����O����ۂɌĂ΂��֐�
    /// </summary>
    private void OnRenderObject()
    {
        //�ҏW���[�h�̏ꍇ
        if (!Application.isPlaying)
        {
            //�S�ẴI�u�W�F�N�g��Update�֐����Ă�
            //EditorApplication.QueuePlayerLoopUpdate();
            //SceneView.RepaintAll();

        }
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //  v.Clear();
        //Debug.Log("a");
        //this.transform.position += new Vector3(0.1f, 0, 0);
        if (Input.GetKey(KeyCode.A))
        {
            this.transform.Translate(-0.1f, 0.0f, 0.0f);
        }
    }
    public void OnSpaceKeyPress()
    {
        Debug.Log("On Space key is pressed");
        foreach (var vertex in mesh.vertices)
        {
            _tmpVertices.Add(vertex);
        }
        _tmpVertices[0] = new Vector3(100, 0, 0);
        mesh.SetVertices(_tmpVertices);
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        _tmpVertices.Clear();
        UpdateVertices();
    }
    void UpdateVertices()
    {
        v.Clear();
        foreach (Vector3 v3 in mesh.vertices)
        {
            v.Add(transform.TransformPoint(v3));
        }
    }
}
