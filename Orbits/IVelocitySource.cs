using UnityEngine;

public interface IVelocitySource
{   
    public Vector3 GetVelocity();
    public Vector3 GetVelocity(Vector3 predictedPosition);
}
