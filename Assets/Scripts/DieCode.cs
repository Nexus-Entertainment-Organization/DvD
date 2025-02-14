using TMPro;
using UnityEngine;

public class DieCode : MonoBehaviour
{
    public int toll;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!gameObject.CompareTag(collision.tag) && collision.name != name)
        {
            collision.GetComponent<StatsController>().TakeAToll(toll);
            Destroy(gameObject);
        }
    }
}
