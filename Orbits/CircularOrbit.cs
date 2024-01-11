using UnityEngine;

[RequireComponent(typeof(Gravity))]
public class CircularOrbit : MonoBehaviour, IVelocitySource
{   
    private Gravity _gravity;
    private Transform _centralBody;
    private Rigidbody _centralBodyRigidbody;
    private Rigidbody _rigidbody;

    private Vector3 _initalPosition;

    private void Awake()
    { 
        _rigidbody = GetComponent<Rigidbody>();
        _gravity = GetComponent<Gravity>();
        _centralBody = _gravity.GetCentralBody();
        _centralBodyRigidbody = _centralBody.GetComponent<Rigidbody>();
    }

    private void Start()
    {
        SetInitialVelocity();
        _initalPosition = transform.position;
    }

    private void SetInitialVelocity()
    {
        float m2 = _centralBodyRigidbody.mass;
        float r = Vector3.Distance(transform.position, _centralBody.position);

        transform.LookAt(_centralBody);

        // Initial Velocity for Circular Orbits: v = sqrt((G * m) / r)
        _rigidbody.velocity += transform.right * Mathf.Sqrt((_gravity.GravityConstant * m2) / r);
    }
    
    public Vector3 GetInitialVelocity(Vector3 predictedPosition)
    {
        float m2 = _centralBodyRigidbody.mass;
        // We use _initalPosition because that's the position used when setting the initial velocity
        float r = Vector3.Distance(_initalPosition, _centralBody.position);

        // Cannot be called otherwise will always face Object to Orbit
        // transform.LookAt(_objectToOrbit);

        /*
        The cross product of 2 vectors will result in the vector perpendicular to them.
        We multiply the result by -1 because Unity uses the right-hand rule for cross product but
        left-handed coordinate system. 
        */

        Vector3 directionVector = (_centralBody.position - transform.position).normalized;
        Vector3 newRightVector = -1 * Vector3.Cross(directionVector, transform.up);

        // Initial Velocity for Circular Orbits: v = sqrt((G * m) / r)
        return newRightVector * Mathf.Sqrt((_gravity.GravityConstant * m2) / r);
    }

    public Vector3 GetVelocity() => Vector3.zero;
    public Vector3 GetVelocity(Vector3 predictedPosition) => GetInitialVelocity(predictedPosition);
}
