using Unity.Cinemachine;
using UnityEngine;

public class ScreenShaker : MonoBehaviour
{
    [SerializeField] public float shakeForce = 1f;
    
    private CinemachineImpulseSource impulseSource;

    private void Awake()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public void Shake(Vector2 direction)
    {
        impulseSource.GenerateImpulseWithVelocity(-direction * shakeForce);
    }
}