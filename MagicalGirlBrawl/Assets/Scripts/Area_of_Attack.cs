using UnityEngine;

public class Area_of_Attack : MonoBehaviour
{
    [SerializeField] private GameObject hitbox;
    [SerializeField] private AnimationCurve SizeCurve;
    [SerializeField] private AnimationCurve DamageCurve;
    private float size;
    public float charged_time;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        size = SizeCurve.Evaluate(charged_time);
        //hitbox.transform.localScale = new Vector3(size, size, size);
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        Color c = sprite.color;
        c.a = size;
        sprite.color = c;

        Destroy(gameObject,0.25f);
        GetComponent<CollisionDamager>().damage = (int) DamageCurve.Evaluate(charged_time);
    }
}
