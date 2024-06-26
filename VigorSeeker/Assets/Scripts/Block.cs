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
    public GameObject pref;
    Spring spring;

    public void OnEnable()
    {
        v = new List<Vector3>(mesh.vertices);
        foreach (Vector3 v3 in mesh.vertices)
        {
            v.Add(transform.TransformPoint(v3));

        }
        Debug.Log("Info: Block awaked");
        Debug.Log("Now" + DateTime.Now);
        var triangle = mesh.triangles;
        foreach (var x in triangle)
        {
            //Debug.Log(x);
        }

    }
    /// <summary>
    /// オブジェクトをレンダリングする際に呼ばれる関数
    /// </summary>
    private void OnRenderObject()
    {
        //編集モードの場合
        if (!Application.isPlaying)
        {
            //全てのオブジェクトのUpdate関数を呼ぶ
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
