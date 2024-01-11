using UnityEngine;

public class Gravity : MonoBehaviour
{
    [SerializeField] private Transform _centralBody;
    [SerializeField] [Range(100.0f, 10000.0f)] private float _gravityConstant = 1000.0f;

    private Rigidbody _rigidbody;
    private Rigidbody _objectToOrbitRigidbody;

    

    public float GravityConstant { get { return _gravityConstant; } }

    private void Awake()
    { 
        _rigidbody = GetComponent<Rigidbody>();
        _objectToOrbitRigidbody = _centralBody.GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        ApplyGravity();
    }

    private void ApplyGravity()
    {
        float m1 = _rigidbody.mass;
        float m2 = _objectToOrbitRigidbody.mass;
        float r = Vector3.Distance(transform.position, _centralBody.position);

        // Direction vector from Satellite to Central Body
        Vector3 directionVector = (_centralBody.position - transform.position).normalized;

        // Force due to Gravity: F = G * (m1 * m2) / r^2
        _rigidbody.AddForce(directionVector * GravityConstant * (m1 * m2) / (r * r));
    }

    public Vector3 GetGravity(Vector3 predictedPosition)
    {   
        float m1 = _rigidbody.mass;
        float m2 = _objectToOrbitRigidbody.mass;
        float r = Vector3.Distance(predictedPosition, _centralBody.position);

        // Direction vector from Satellite to Central Body
        Vector3 directionVector = (_centralBody.position - predictedPosition).normalized;

        // Force due to Gravity: F = G * (m1 * m2) / r^2
        return directionVector * GravityConstant * (m1 * m2) / (r * r);
    }

    public Transform GetCentralBody() => _centralBody;
}
