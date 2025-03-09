using UnityEngine;

public class CollisionDamager : MonoBehaviour
{
    public int damage = -1;
    public Movement playerCaster;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(damage > 0) damage = -damage;

        Movement m;
        if(collision.TryGetComponent(out m))
        {
        //    if (m.playerID == playerCaster.playerID) return;
            if (m.isActive)
            {
                HealthSystem h = m.GetComponent<HealthSystem>();
                h.addHealth(damage);
            }
        }
    }
    //public int playerID = 0;

}
