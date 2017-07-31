using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PulseCore : MonoBehaviour
{
    public TextMesh display;

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

    public UnityEvent onCoreAtZeroPower;

    public GameObject floatingText;

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
            float newPower = value;
            float oldPower = coreHealth;

            float delta = newPower - oldPower;


            GameObject text = (GameObject)GameObject.Instantiate(floatingText);
            TextMesh tm = text.GetComponent<TextMesh>();
            text.transform.position = transform.position;
            if(delta < 0)
                tm.color = Color.red;
            else
                tm.color = Color.green;
            tm.text = System.Convert.ToString( (int)(100f * delta) );


            coreHealth = Mathf.Clamp01(value);
            if( coreHealth <= 0f )
                onCoreAtZeroPower.Invoke();
            display.text = System.Convert.ToString( (int)( PercentPower ) ) + "%";
        }
    }

    private void Awake()
    {
        if( meshToPulse == null )
            meshToPulse = GetComponent<MeshRenderer>();

        StartCoroutine( RecoveryRoutine() );
    }

    public float coreRecoveryAmount = .01f;

    public float recoveryRate = 1f;

    IEnumerator RecoveryRoutine()
    {
        for( ;;)
        {
            float nextTime = recoveryRate;
            while( nextTime > 0 )
            {
                nextTime -= Time.fixedDeltaTime * Time.timeScale;

                yield return new WaitForFixedUpdate();
            }

            if(CoreHealth < 1f)
            {
                CoreHealth += coreRecoveryAmount;
            }

            yield return new WaitForFixedUpdate();
        }
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

    public float PercentPower {
        get {
            return CoreHealth * 100f;
        }
        set {
            float v = value / 100f;
            CoreHealth = v;
        }
    }

    void Update()
    {
        if( display != null )
        {
            display.text = System.Convert.ToString( (int)( PercentPower ) ) + "%";
        }
    }
}
