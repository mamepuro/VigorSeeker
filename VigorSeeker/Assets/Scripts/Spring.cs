using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public enum SpringType
{
    Leg,
    Block,
    Tenchi,
}
[ExecuteAlways]
public class Spring : MonoBehaviour
{
    /// <summary>
    /// ばねに接続されている質点のインデックス
    /// </summary>
    [SerializeField] public List<int> _massPointIndexes;
    [SerializeField] public SpringType springType;
    [SerializeField] public MassPoint _leftMassPoint;
    [SerializeField] public MassPoint _rightMassPoint;
    [SerializeField] public float _springConstant;
    /// <summary>
    /// ばねの自然長
    /// </summary>
    [SerializeField] public float _springLength;
    [SerializeField] public float _dampingConstant;
    [SerializeField] public float _restLength;

    void OnEnable()
    {
        //Debug.Log("Info: Spring awaked.");
        _massPointIndexes = new List<int>();
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    public void SetSpring(MassPoint leftMassPoint, MassPoint rightMassPoint, float springConstant, float springLength, float dampingConstant, float restLength, SpringType springType)
    {
        _leftMassPoint = leftMassPoint;
        _rightMassPoint = rightMassPoint;
        _springConstant = springConstant;
        _springLength = springLength;
        _dampingConstant = dampingConstant;
        _restLength = restLength;
        _massPointIndexes.Add(leftMassPoint._index);
        _massPointIndexes.Add(rightMassPoint._index);
        this.springType = springType;
    }

    public Vector3 GetForce(MassPoint massPoint)
    {
        Vector3 force = Vector3.zero;
        //ばねに接続されている質点でない場合は力を返さない
        if (!(massPoint == _leftMassPoint || massPoint == _rightMassPoint))
        {
            Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            return force;
        }
        //左側の質点についての場合
        if (massPoint == _leftMassPoint)
        {
            //Debug.Log("left mass point");
            Vector3 r = _rightMassPoint._position - _leftMassPoint._position;
            Vector3 v = _rightMassPoint._velocity - _leftMassPoint._velocity;
            force = _springConstant * (r.magnitude - _springLength) * r.normalized + _dampingConstant * v;
            return force;
            //自動生成したコード
            // Vector3 direction = _rightMassPoint._position - _leftMassPoint._position;
            // float distance = direction.magnitude;
            // float displacement = distance - _springLength;
            // force = -_springConstant * displacement * direction / distance;
            // force += -_dampingConstant * Vector3.Dot(_rightMassPoint._velocity - _leftMassPoint._velocity, direction.normalized) * direction.normalized;
        }
        // masspointが右側の質点の場合
        else
        {
            //Debug.Log("right mass point");
            Vector3 r = _leftMassPoint._position - _rightMassPoint._position;
            Vector3 v = _leftMassPoint._velocity - _rightMassPoint._velocity;
            force = _springConstant * (r.magnitude - _springLength) * r.normalized + _dampingConstant * v;
            return force;
            //自動生成したコード
            // Vector3 direction = _leftMassPoint._position - _rightMassPoint._position;
            // float distance = direction.magnitude;
            // float displacement = distance - _springLength;
            // force = -_springConstant * displacement * direction / distance;
            // force += -_dampingConstant * Vector3.Dot(_leftMassPoint._velocity - _rightMassPoint._velocity, direction.normalized) * direction.normalized;
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
}
