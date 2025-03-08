using UnityEngine;

public class StartingBlockTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("hihihiha");
        Movement m;
        if (collision.TryGetComponent(out m))
        {
            GameController.instance.PlayerIsReady();
        }
    }
}
