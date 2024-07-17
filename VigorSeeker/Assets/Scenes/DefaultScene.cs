using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class DefaultScene : MonoBehaviour
{
    /// <summary>[
    /// 選択中のブロック
    /// </summary>
    public Block selectedBlock;
    /// <summary>
    /// 接続対象のブロック
    /// </summary>
    public Block connectedBlock;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("DefaltScene.cs is set up");
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("✅DefaltScene.cs is calling");
    }
}