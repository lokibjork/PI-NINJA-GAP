using Unity.Cinemachine;
using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    public float parallaxFactor; // Fator de velocidade do parallax (0 = estático)
    private Transform cameraTransform;
    private Vector3 lastCameraPosition;
    private Vector3 startPosition;
    private float textureUnitSizeX; // Tamanho horizontal de uma unidade da textura (para tiling infinito)
    private CinemachineCamera virtualCamera; // Referência à Virtual Camera do Cinemachine

    void Start()
    {
        virtualCamera = FindObjectOfType<CinemachineCamera>();
        if (virtualCamera != null)
        {
            cameraTransform = virtualCamera.transform;
        }
        else
        {
            cameraTransform = Camera.main.transform;
            Debug.LogWarning("Cinemachine Virtual Camera não encontrada. Usando a Main Camera diretamente, o jittering pode persistir.");
        }
        lastCameraPosition = cameraTransform.position;
        startPosition = transform.position;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null && sr.drawMode == SpriteDrawMode.Tiled)
        {
            textureUnitSizeX = sr.bounds.size.x;
        }
    }

    void LateUpdate()
    {
        Vector3 currentCameraPosition = cameraTransform.position;
        Vector3 deltaMovement = currentCameraPosition - lastCameraPosition;
        transform.position += deltaMovement * parallaxFactor;
        lastCameraPosition = currentCameraPosition;

        // Para tiling infinito horizontal (opcional)
        if (textureUnitSizeX > 0)
        {
            float offsetX = (currentCameraPosition.x - startPosition.x) * parallaxFactor;
            transform.position = new Vector3(startPosition.x + offsetX % textureUnitSizeX, transform.position.y, transform.position.z);
        }
    }
}