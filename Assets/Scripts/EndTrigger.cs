using UnityEngine;
using System.Collections;

public class EndTrigger : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        StartCoroutine(NextFloor());
    }

    IEnumerator NextFloor()
    {
        yield return new WaitForSeconds(0.1f);
        GameManager.Instance.OnFloorWin();
    }
}
