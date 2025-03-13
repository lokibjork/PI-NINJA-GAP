using UnityEngine;

public class ParticleSpawner : MonoBehaviour
{
    [SerializeField] private GameObject particlePrefab;
    
    private CircleCollider2D circleCollider;

    private void Awake()
    {
        circleCollider = GetComponent<CircleCollider2D>();
    }

    public void SpawnImpactParticles(Vector3 spawnPosition, Vector3 dir)
    {
        Quaternion rot = Quaternion.FromToRotation(Vector3.right, dir);
        Instantiate(particlePrefab, spawnPosition, rot);
    }
}
