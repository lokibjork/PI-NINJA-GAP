using UnityEngine;

public class BreakablePlatform : MonoBehaviour
{
    public AudioClip blockSound;
    public GameObject explosionEffect;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bala"))
        {
            // Plataforma atingida por uma bala
            Break();

            Destroy(collision.gameObject);
        }else if (collision.gameObject.CompareTag("Player"))
        {
            Break();
        }
    }

    private void Break()
    {

        SoundManagerSO.PlaySoundFXClip(blockSound, transform.position, 1f);
        Instantiate(explosionEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}