using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Event_Powerup : GameEvent
{
    List<Module> allModules;

    public List<GameObject> powerups;

    public List<float> weights;

    public List<PowerupSpawnLocation> spawns;
    
    public float TotalWeight {
        get {
            float t = 0f;
            foreach( var v in weights )
                t += v;
            return t;
        }
    }

    public GameObject RandomWeightedPowerup {
        get {
            float w = UnityEngine.Random.Range(0f,TotalWeight);
            float c = 0f;
            for( int i = 0; i < powerups.Count; ++i )
            {
                c += weights[ i ];
                if( c > w )
                    return powerups[ i ];
            }
            return powerups[ powerups.Count - 1 ];
        }
    }

    protected override IEnumerator Running()
    {
        GameObject gevent = (GameObject)GameObject.Instantiate(RandomWeightedPowerup);
        //GameEvent gEvent = gevent.GetComponent<GameEvent>();

        spawns = GameObject.FindObjectsOfType<PowerupSpawnLocation>().ToList();

        int selection = UnityEngine.Random.Range(0,spawns.Count);

        gevent.transform.position = spawns[ selection ].transform.position;

        yield return new WaitForSeconds( 1f );
        Destroy( gameObject );
        yield return 0;
    }

    private void OnDisable()
    {
        Destroy( gameObject );
    }


}
