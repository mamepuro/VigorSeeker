using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.TextCore;
using UnityEngine.UIElements;

[ExecuteAlways]
public class MassPoint : MonoBehaviour
{
    /// <summary>
    /// この質点が属するブロック
    /// </summary>
    [SerializeField] public Block block;
    [SerializeField] public float _mass;
    [SerializeField] public Vector3 _velocity;
    [SerializeField] public int _index;
    [SerializeField] public Vector3 _position;
    [SerializeField] public Vector3 _force;
    [SerializeField] public Vector3 _acc;
    [SerializeField] public bool _isFixed = false;
    /// <summary>
    /// この質点に接続されているばね
    /// </summary>
    [SerializeField] List<Spring> _springs;
    // Start is called before the first frame update
    void OnEnable()
    {
        //Debug.Log("Info: MassPoint awaked.");
        _springs = new List<Spring>();
    }
    void Start()
    {
        //Debug.Log("Info: MassPoint start.");
    }
    public void SetMassSpring(float mass, Vector3 velocity, int index, Vector3 position, Block block)
    {
        _mass = mass;
        _velocity = velocity;
        _index = index;
        _position = position;
        _force = Vector3.zero;
        _acc = Vector3.zero;
        this.block = block;
        // if (this._index == 2)
        // {
        //     _isFixed = true;
        // }
        this.block = block;
    }
    public void AddSpring(Spring spring)
    {
        _springs.Add(spring);
    }
    /// <summary>
    /// 質点に働く力を計算する
    /// </summary>
    /// <returns></returns>
    public Vector3 CalcForce()
    {
        Vector3 force = Vector3.zero;
        int index = this._index;
        if (_isFixed)
        {
            return force;
        }
        //Debug.Log("calc force");
        foreach (Spring spring in _springs)
        {
            force += spring.GetForce(this);
        }
        return force;
    }
    // Update is called once per frame
    void Update()
    {
        float dt = 0.01f;
        Vector3 acc = CalcForce() / _mass;
        _force = CalcForce();
        _acc = acc;
        //Debug.Log("acc: " + acc + " ID is " + _index);
        _velocity += (acc * dt);
        _position = _position + _velocity * dt;

    }
}
