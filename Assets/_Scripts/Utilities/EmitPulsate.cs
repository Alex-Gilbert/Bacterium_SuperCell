using UnityEngine;
using System.Collections;

public class EmitPulsate : MonoBehaviour 
{

    Renderer render;
    Color emissionColor;
    public float frequency = 1f;
    public float amplitude = 1f;
    public float baseMult = 0.75f;

    // Use this for initialization
    void Start()
    {
        render = GetComponent<Renderer>();
        emissionColor = render.material.GetColor("_EmissionColor") * baseMult * amplitude;
    }

    // Update is called once per frame
    void Update()
    {
        if (render.enabled)
        {
            float glow = (2 + Mathf.Cos(Time.time * frequency)) * amplitude;
            render.material.SetColor("_EmissionColor", emissionColor * glow);
        }
    }
}
