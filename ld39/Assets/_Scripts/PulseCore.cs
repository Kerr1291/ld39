using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulseCore : MonoBehaviour
{
    public Light coreLight;
    public MeshRenderer meshToPulse;
    public float pulseRate = 1f;
    public float pulseRange = 1f;
    float pulseMod = 0f;

    [ColorUsage(true, true, 0f, 10f, 0f, 10f)]
    public Color goodColor;
    [ColorUsage(true, true, 0f, 10f, 0f, 10f)]
    public Color okColor;
    [ColorUsage(true, true, 0f, 10f, 0f, 10f)]
    public Color badColor;

    public float midHealth = .5f;

    [SerializeField]
    private float coreHealth = 1f;

    public Color CoreColor {
        get {
            if(CoreHealth < midHealth )
            {
                return Color.Lerp( badColor, okColor, CoreHealth / midHealth );
            }
            else
            {
                return Color.Lerp( okColor, goodColor, ( CoreHealth - midHealth ) / ( 1f - midHealth ) );
            }
        }
    }

    public Color CoreOn {
        get {
            return CoreColor + CoreColor * pulseRange;
        }
    }

    public Color CoreOff {
        get {
            return CoreColor - CoreColor * pulseRange;
        }
    }

    public float CoreHealth {
        get {
            return coreHealth;
        }

        set {
            coreHealth = value;
        }
    }

    private void Awake()
    {
        if( meshToPulse == null )
            meshToPulse = GetComponent<MeshRenderer>();
    }

    IEnumerator Start()
    {

        for( ;;)
        {
            float amount = Time.fixedDeltaTime;

            pulseMod = 0f;
            while( pulseMod < pulseRange )
            {
                pulseMod += amount * Time.timeScale * pulseRate;

                Color pulseColor = Color.Lerp(CoreOff,CoreOn, 1f - pulseMod/pulseRange);

                coreLight.color = pulseColor;

                meshToPulse.sharedMaterial.SetColor( "_EmissionColor", pulseColor );

                yield return new WaitForFixedUpdate();
            }

            pulseMod = 0f;
            while( pulseMod < pulseRange )
            {
                pulseMod += amount * Time.timeScale * pulseRate;

                Color pulseColor = Color.Lerp(CoreOn,CoreOff, 1f - pulseMod/pulseRange);

                coreLight.color = pulseColor;

                meshToPulse.sharedMaterial.SetColor( "_EmissionColor", pulseColor );

                yield return new WaitForFixedUpdate();
            }
        }
    }

    private void OnDisable()
    {
        meshToPulse.sharedMaterial.SetColor( "_EmissionColor", Color.black );
        coreLight.color = Color.black;
    }
}
