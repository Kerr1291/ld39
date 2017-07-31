using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Weapons : MonoBehaviour
{

    public TextMesh display;

    public List<Module> weapons;

    public PulseCore core;

    public float TotalPower {
        get {
            float max = weapons.Count * 1f;
            float total = 0f;
            for( int i = 0; i < weapons.Count; ++i )
            {
                total += weapons[ i ].Power;
            }
            return total / max;
        }
    }

    public float PercentPower {
        get {
            return ( TotalPower ) * 100f;
        }
    }

    private void Awake()
    {
        weapons = GetComponentsInChildren<Module>().ToList();
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
            core.CoreHealth -= .1f;
            gameObject.SetActive( false );
        }
    }
}
