using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class MapMover : MonoBehaviour
{
    public Transform playerIcon;
    public List<Transform> nodes; // Lista de pontos no mapa
    private int currentNodeIndex = 0;
    public float moveSpeed = 5f;
    private bool isMoving = false;

    // Variável pra controlar o input do eixo horizontal (evitar múltiplos disparos)
    private bool horizontalAxisInUse = false;
    // Zona morta pra evitar que pequenas variações acionem o movimento
    public float axisDeadZone = 0.5f;

    private void Update()
    {
        if (!isMoving)
        {
            HandleInput();
        }
    }

    private void HandleInput()
    {
        // Pega o input horizontal, que já funciona pro teclado e controle
        float horizontal = Input.GetAxis("Horizontal");

        // Se o input ainda não estiver sendo usado
        if (!horizontalAxisInUse)
        {
            if (horizontal > axisDeadZone && currentNodeIndex < nodes.Count - 1)
            {
                MoveToNode(currentNodeIndex + 1);
                horizontalAxisInUse = true;
            }
            else if (horizontal < -axisDeadZone && currentNodeIndex > 0)
            {
                MoveToNode(currentNodeIndex - 1);
                horizontalAxisInUse = true;
            }
        }
        // Quando o stick ou as teclas voltarem pro neutro, libera o input
        if (Mathf.Abs(horizontal) < axisDeadZone)
        {
            horizontalAxisInUse = false;
        }
    }

    private void MoveToNode(int targetNodeIndex)
    {
        currentNodeIndex = targetNodeIndex;
        StartCoroutine(MovePlayer(nodes[currentNodeIndex].position));
    }

    private IEnumerator MovePlayer(Vector3 targetPosition)
    {
        isMoving = true;

        while (Vector3.Distance(playerIcon.position, targetPosition) > 0.1f)
        {
            playerIcon.position = Vector3.MoveTowards(playerIcon.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        playerIcon.position = targetPosition;
        isMoving = false;
    }
}


