using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Event_Damage : GameEvent
{
    List<Module> allModules;

    public AudioSource attackSound;
    public AudioSource damageSound;

    public float randomDamageDelay = 1f;

    public Camera gameCamera;

    public float damageAmount = 1f;

    //[Header("Smaller is more often")]
    //public int chanceToPlayAttackSound = 5;
    
    //TODO: camera shake



    protected override IEnumerator Running()
    {
        gameCamera = GameObject.FindObjectOfType<Camera>();

        //if( chanceToPlayAttackSound >= 0 )
        {
            //int playPreSound = UnityEngine.Random.Range(0,chanceToPlayAttackSound);

            //if( ( attackSound != null && damageSound != null && playPreSound == 0 ) )
            {
                float delay = UnityEngine.Random.Range(0f,randomDamageDelay);
                //attackSound.Play();
                yield return new WaitForSeconds( delay * .5f );
                //damageSound.Play();
                yield return new WaitForSeconds( delay );
            }
        }

        allModules = GameObject.FindObjectsOfType<Module>().ToList();

        int module = UnityEngine.Random.Range(0,allModules.Count);

        if( module < allModules.Count && allModules[ module ] != null )
        {
            allModules[ module ].Power -= damageAmount;

            Debug.Log( allModules[ module ].moduleRoot.name + " damaged to " + allModules[ module ].Power );
        }

        yield return new WaitForSeconds( 1f );
        Destroy( gameObject );
        yield return 0;
    }

    private void OnDisable()
    {
        Destroy( gameObject );
    }


}
