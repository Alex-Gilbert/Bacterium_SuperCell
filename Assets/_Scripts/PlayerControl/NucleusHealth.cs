using UnityEngine;
using System.Collections;

public class NucleusHealth : MonoBehaviour 
{
    Renderer render;
    Color emissionColor;
    public float frequency = 1f;
    public float amplitude = 1f;
    public float baseMult = 0.75f;

    public Color FullColor;
    public Color MidColor;
    public Color LowColor;

    // Use this for initialization
    void Start()
    {
        render = GetComponent<Renderer>();
        emissionColor = FullColor * baseMult * amplitude;
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

    public void SetHealth(int Health)
    {
        switch(Health)
        {
            case 3:
                render.enabled = true;
                emissionColor = FullColor * baseMult * amplitude;
                break;
            case 2:
                render.enabled = true;
                emissionColor = MidColor * baseMult * amplitude;
                break;
            case 1:
                render.enabled = true;
                emissionColor = LowColor * baseMult * amplitude;
                break;
            case 0:
                render.enabled = false;
                break;
        }

        
    }
}
