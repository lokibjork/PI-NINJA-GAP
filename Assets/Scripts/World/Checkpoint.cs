using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public string checkpointID;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerPrefs.SetFloat("CheckpointX", transform.position.x);
            PlayerPrefs.SetFloat("CheckpointY", transform.position.y);
            PlayerPrefs.SetString("CheckpointID", checkpointID);
            PlayerPrefs.Save();
            Debug.Log("Checkpoint salvo: " + checkpointID);
        }
    }
}
