using UnityEngine;

public class Area_of_Attack : MonoBehaviour
{
    [SerializeField] private GameObject hitbox;
    [SerializeField] private AnimationCurve Size;
    [SerializeField] private AnimationCurve Damage;
    private float size;
    public float charged_time;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        size = Size.Evaluate(charged_time);
        hitbox.transform.localScale = new Vector3(size, size, size);
        Destroy(gameObject,0.25f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        return;//Add damage dealt to player using the Damage curve
    }
}
