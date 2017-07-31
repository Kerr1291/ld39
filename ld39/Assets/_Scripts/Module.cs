using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Module : MonoBehaviour
{
    public GameRunner game;

    public TextMesh display;

    public GameObject damageEffect;
    public GameObject explodeEffect;
    public GameObject exploded;

    public PulseCore core;

    public GameObject moduleRoot;

    public GameObject recentlyRepairedEffect;
    public GameObject recentlyDamagedEffect;

    public float timeSinceLastUsed;
    public float timeSinceLastDamage;

    public GameObject floatingText;

    public enum ModuleType
    {
        Shield,
        Weapon
    };

    public ModuleType type;

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
            float newPower = value;
            float oldPower = _power;

            float delta = newPower - oldPower;
            _power = Mathf.Clamp(value,MinPower,MaxPower);

            if( delta < 0f )
                Damage( delta );
            if( delta > 0f )
                Repair( delta );
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

    public bool Active {
        get {
            return moduleRoot.gameObject.activeInHierarchy;
        }
        set {
            moduleRoot.gameObject.SetActive( value );
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

    private void Awake()
    {
        timeSinceLastUsed = recentRepairTime;
        timeSinceLastDamage = recentDamageTime;
        core = GameObject.FindObjectOfType<PulseCore>();
        game = GameObject.FindObjectOfType<GameRunner>();
    }

    public void OnEnable()
    {
        timeSinceLastUsed = recentRepairTime;
        timeSinceLastDamage = recentDamageTime;

        if( exploded != null )
            Destroy( exploded );
    }

    public virtual void Damage(float value)
    {
        GameObject text = (GameObject)GameObject.Instantiate(floatingText);
        TextMesh tm = text.GetComponent<TextMesh>();
        text.transform.position = transform.position;
        tm.color = Color.red;
        tm.text = System.Convert.ToString( (int)( 100f * value ) );

        timeSinceLastDamage = 0f;

        if( !Active )
        {
            ShowText( Color.red, "Module Destoryed! Core Directly Damaged!" );


            //TODO play power surge from here to core damage animation?
            core.CoreHealth -= value * .5f;
            GameObject.Instantiate( damageEffect ).transform.position = core.transform.position;
            return;
        }

        if( value < 0 )
        {
            GameObject.Instantiate( damageEffect ).transform.position = transform.position;
        }

        float damage = 0f;

        if( type == ModuleType.Shield )
        {
            if( Power > .5f )
            {
                damage = .1f * value;
            }
            else if( Power > .25f )
            {
                damage = .2f * value;
            }
            else
            {
                damage = .5f * value;
            }
        }

        if( type == ModuleType.Weapon )
        {
            if( Power > .4f )
            {
                damage = .05f * value;
            }
            else if( Power > .2f )
            {
                damage = .1f * value;
            }
            else
            {
                damage = .4f * value;
            }
        }

        if( WasRecentlyRepaired )
            damage *= .5f;

        if( WasRecentlyDamaged )
            damage *= 2f;

        timeSinceLastUsed = recentRepairTime;

        core.CoreHealth += damage;

        if( Active && _power <= MinPower )
            OnZeroPower();
    }

    public void ShowText(Color color, string value)
    {
        GameObject text = (GameObject)GameObject.Instantiate(floatingText);
        TextMesh tm = text.GetComponent<TextMesh>();
        text.transform.position = transform.position;
        tm.color = color;
        tm.text = value;
    }

    public void Repair( float amount )
    {
        GameObject text = (GameObject)GameObject.Instantiate(floatingText);
        TextMesh tm = text.GetComponent<TextMesh>();
        text.transform.position = transform.position;
        tm.color = Color.green;
        tm.text = System.Convert.ToString( (int)( 100f * amount ) );

        if( Power >= MaxPower )
        {
            ShowText( Color.blue, "Fully Repaired Bonus!" );
            game.Score += 20;
            core.CoreHealth += .1f;
        }
        else
        {
            core.CoreHealth += .01f;
        }
    }

    public virtual bool Use( PlayerController player )
    {
        if( Power >= MaxPower )
            return false;

        if( WasRecentlyDamaged )
            game.Score += 5;
        else if( !WasRecentlyRepaired )
            game.Score += 2;
        else
            game.Score += 1;

        timeSinceLastUsed = 0f;

        Power += player.repairPower;

        if( type == ModuleType.Shield )
        {

        }
        if( type == ModuleType.Weapon )
        {

        }

        return true;
    }    

    void Update()
    {
        if( display != null )
        {
            display.text = System.Convert.ToString( (int)( PercentPower ) );
        }

        timeSinceLastUsed += Time.deltaTime;
        timeSinceLastDamage += Time.deltaTime;
        
        recentlyDamagedEffect.SetActive( WasRecentlyDamaged );
        recentlyRepairedEffect.SetActive( WasRecentlyRepaired );
    }

    public float recentRepairTime = 1f;

    public bool WasRecentlyRepaired {
        get {
            return ( timeSinceLastUsed < recentRepairTime );
        }
    }

    public float recentDamageTime = 1f;

    public bool WasRecentlyDamaged {
        get {
            return ( !WasRecentlyRepaired && timeSinceLastDamage < recentDamageTime );
        }
    }

    public virtual void OnZeroPower()
    {
        Active = false;

        if( exploded != null )
            Destroy( exploded );

        ShowText( Color.red, "Power Module Destoryed!" );

        GameObject.Instantiate( explodeEffect ).transform.position = transform.position;


        exploded = (GameObject)GameObject.Instantiate( recentlyDamagedEffect );
        exploded.transform.position = transform.position;

        if( type == ModuleType.Shield )
        {
            core.CoreHealth -= .1f;
        }
        if( type == ModuleType.Weapon )
        {
            core.CoreHealth -= .05f;
        }
    }

}
