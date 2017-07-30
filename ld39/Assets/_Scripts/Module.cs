using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Module : MonoBehaviour
{
    public TextMesh display;

    public GameObject damageEffect;

    public float PercentPower {
        get {
            return _power * 100f;
        }
        set {
            float v = value / 100f;
            _power = v;
        }
    }

    public float Power {
        get {
            return _power;
        }
        set {

            if( value < _power )
                GameObject.Instantiate( damageEffect ).transform.position = transform.position;

            _power = Mathf.Clamp(value,MinPower,MaxPower);
        }
    }

    public float MaxPower {
        get {
            return _maxPower;
        }

        set {
            _maxPower = value;
        }
    }

    public float MinPower {
        get {
            return _minPower;
        }

        set {
            _minPower = value;
        }
    }

    public virtual bool CanUse {
        get {
            return _canUse;
        }

        set {
            _canUse = value;
        }
    }

    [ SerializeField ]
    protected float _minPower;

    [ SerializeField ]
    protected float _maxPower = 1f;

    [ SerializeField ]
    protected float _power = 1f;

    [ SerializeField ]
    protected bool _canUse = true;
    
    public virtual bool Use( PlayerController player )
    {
        if( Power >= MaxPower )
            return false;
        Power += .1f;
        return true;
    }    

    void Update()
    {
        if( display != null )
        {
            display.text = System.Convert.ToString( (int)(Power * 100f) );
        }
    }
}
