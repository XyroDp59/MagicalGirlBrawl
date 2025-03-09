using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    private float speed = 13f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject,10f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.right * Time.deltaTime * speed;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        Movement m;
        if (other.TryGetComponent(out m))
        {
            if (!m.isActive)
            {
                return;
            }
        }
        Destroy(gameObject);
    }
}
