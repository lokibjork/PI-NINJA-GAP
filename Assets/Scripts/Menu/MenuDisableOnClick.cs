using UnityEngine;
using System.Collections;

public class MenuDisableOnClick : MonoBehaviour
{
    [SerializeField] private float disableDelay = 2f;

    public void DisableWithDelay() => StartCoroutine(DisableAfterDelay());

    private IEnumerator DisableAfterDelay()
    {
        yield return new WaitForSeconds(disableDelay);
        gameObject.SetActive(false);
    }
}