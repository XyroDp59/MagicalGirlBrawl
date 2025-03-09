using UnityEngine;

public class ChargingParticle : MonoBehaviour
{
    ParticleSystem _particleSystem;

    [SerializeField] AnimationCurve sizeCurve;
    [SerializeField] Gradient gradient;

    private void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
    }

    public void Evaluate(float t)
    {

        transform.localScale = Vector3.one * sizeCurve.Evaluate(t);
        if(_particleSystem == null) _particleSystem = GetComponent<ParticleSystem>();
        var p = _particleSystem.main;
        p.startColor = gradient.Evaluate(t);
    }
}
