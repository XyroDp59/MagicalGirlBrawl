using UnityEngine;

public class StartingBlockTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Movement m;
        if (collision.TryGetComponent(out m))
        {
            GameController.instance.PlayerIsReady(m.transform.parent.GetComponent<Player>());
        }
    }
}
