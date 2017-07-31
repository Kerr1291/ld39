using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Shields : MonoBehaviour {

    public TextMesh display;

    public List<Module> shields;

    public PulseCore core;

    public float TotalPower {
        get {
            float max = shields.Count * 1f;
            float total = 0f;
            for( int i = 0; i < shields.Count; ++i )
            {
                total += shields[ i ].Power;
            }
            return total / max;
        }
    }

    public float PercentPower {
        get {
            return (TotalPower) * 100f;
        }
    }

    private void Awake()
    {
        shields = GetComponentsInChildren<Module>().ToList();
        core = GameObject.FindObjectOfType<PulseCore>();
    }

    void Update()
    {
        if( display != null )
        {
            display.text = System.Convert.ToString( (int)( PercentPower ) ) + "%";
        }

        if( TotalPower <= 0f )
        {
            core.CoreHealth -= .2f;
            gameObject.SetActive( false );
        }
    }
}
