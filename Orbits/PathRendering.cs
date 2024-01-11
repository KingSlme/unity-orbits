using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Gravity))]
public class PathRendering : MonoBehaviour
{   
    [SerializeField] private LineRenderer _lineRendererRealPath;
    [SerializeField] private LineRenderer _lineRendererPredictedPath;
    [SerializeField] [Range(0, 100)] private int _maxPositions = 50;
    [SerializeField] private bool _renderRealPath = true;
    [SerializeField] private bool _renderPredictedPath = true;

    private int _numPositions = 0;
    private bool _isDelaying = false;
    private Rigidbody _rigidbody;
    private Gravity _gravity;

    private Vector3 _totalExternalVelocity;

    private void Awake()
    {   
        _rigidbody = GetComponent<Rigidbody>();
        _gravity = GetComponent<Gravity>();
    }

    private void Start()
    {
        _lineRendererRealPath.positionCount = 0;
        _lineRendererPredictedPath.positionCount = 0;
    }

    private void Update()
    {
        if(_renderRealPath) {
            RenderRealPath();
        }
        else
        {   
            _lineRendererRealPath.positionCount = 0;
            _numPositions = 0;
        }
        if(_renderPredictedPath) {
            RenderPredictedPath();
        }
        else
        {
            _lineRendererPredictedPath.positionCount = 0;
        }
    }

    private void RenderRealPath()
    {   
        if(!_isDelaying)
        {       
                if(_numPositions < _maxPositions) {
                    StartCoroutine(DelayNewPosition());
                    _lineRendererRealPath.positionCount++;
                    _lineRendererRealPath.SetPosition(_numPositions, transform.position);
                    _numPositions++;
                }
                else if(_numPositions == _maxPositions)
                {   
                    StartCoroutine(DelayNewPosition());
                        for(int i = 0; i<_lineRendererRealPath.positionCount; i++)
                        {   
                            if(i == _lineRendererRealPath.positionCount - 1)
                            {
                                _lineRendererRealPath.SetPosition(i, transform.position);
                            }
                            else
                            {
                                 _lineRendererRealPath.SetPosition(i, _lineRendererRealPath.GetPosition(i+1));
                            }
                        }
                }
                else
                {
                    _lineRendererRealPath.positionCount = 0;
                    _numPositions = 0;
                }
        }
    }

    private IEnumerator DelayNewPosition()
    {   
        _isDelaying = true;
        yield return new WaitForSecondsRealtime(0.1f);
        _isDelaying = false;
    }

    private Vector3 CalculatePosition(float time)
    {   
        // Final Position: xf = x0 + (vf * t)
        // Final Velocity: vf = vi + (a * t)

        /*
        Since the acceleration will be different at different points,
        we will integrate the acceleration to get the instantaneous velocity.
        */

        Vector3 currentPredictedPosition = transform.position;

        _totalExternalVelocity = Vector3.zero;
        foreach(IVelocitySource v in GetComponents<IVelocitySource>())
        {   
            _totalExternalVelocity += v.GetVelocity();
            _totalExternalVelocity += v.GetVelocity(currentPredictedPosition);
        }
        Vector3 currentPredictedVelocity = _totalExternalVelocity;

        float currentTime = 0.0f;
        // Small time step for integration
        float timeStep = 0.01f;

        while (currentTime < time)
        {
            Vector3 gravitationalForce = _gravity.GetGravity(currentPredictedPosition);

            // Acceleration: a = F / m
            Vector3 acceleration = gravitationalForce / _rigidbody.mass;

            // Numerical integration
            currentPredictedVelocity += acceleration * timeStep;
            currentPredictedPosition += currentPredictedVelocity * timeStep;

            currentTime += timeStep;
        }
        return currentPredictedPosition;
    }

    private void RenderPredictedPath()
    {   
            _lineRendererPredictedPath.positionCount = 0;
            for(int i = 0; i<_maxPositions; i++)
            {   
                float j = i * 0.1f;
                _lineRendererPredictedPath.positionCount++;
                _lineRendererPredictedPath.SetPosition(i, CalculatePosition(j));
            }
    }
}
