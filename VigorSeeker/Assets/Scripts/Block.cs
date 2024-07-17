using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;
//using UnityEditor.SearchService;
using UnityEngine;

[ExecuteAlways]
public class Block : MonoBehaviour
{
    [SerializeField] Mesh mesh;
    // [SerializeField] MeshFilter meshFilter;
    [SerializeField] List<Vector3> v;
    [SerializeField] public int ID;
    Spring spring;
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
        v = new List<Vector3>(mesh.vertices);
        foreach (Vector3 v3 in mesh.vertices)
        {
            v.Add(transform.TransformPoint(v3));

        }
        Debug.Log("Info: Block awaked. ID is " + ID);
       // Debug.Log("Now" + DateTime.Now);
        var triangle = mesh.triangles;
        foreach (var x in triangle)
        {
            //Debug.Log(x);
        }
        _leftPocketInsertingBlock = new List<Block>();
        _rightPocketInsertingBlock = new List<Block>();

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
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            this.transform.Translate(-0.1f, 0.0f, 0.0f);
        }
    }
}
