using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Level1 : MonoBehaviour
{
    public TextMesh levelText;

    public List<GameObject> events;

    public List<float> weights;

    public int baseTime = 30;
    public int scaleTime = 10;
        
    public int baseEvents = 10;
    public int scaleEvents = 5;

    public GameObject levelRoot;

    public float TotalWeight {
        get {
            float t = 0f;
            foreach( var v in weights )
                t += v;
            return t;
        }
    }

    public GameObject RandomWeightedEvent {
        get {
            float w = UnityEngine.Random.Range(0f,TotalWeight);
            float c = 0f;
            for(int i = 0; i < events.Count;++i)
            {
                c += weights[ i ];
                if( c > w )
                    return events[ i ];
            }
            return events[ events.Count-1 ];
        }
    }

    public void Setup( GameRunner game, int level )
    {
        levelText.text = System.Convert.ToString( level );

        int levelTime = baseTime + scaleTime * (1 + (level / 5));

        int minEvents = baseEvents * (1 + (level / 5));
        int maxEvents = minEvents + scaleEvents * level;

        int numEvents = UnityEngine.Random.Range(minEvents,maxEvents);

        Debug.Log( "numEvents " + numEvents );

        game.events.Clear();

        float minInterval = .5f;
        float currentTime = 0f;
        float extraTime = 0f;
        float timeRemaining = levelTime;
        while( timeRemaining > 3f )
        {
            float maxInterval = (levelTime / numEvents) + extraTime;

            if( maxInterval < minInterval )
                maxInterval = minInterval;

            float nextInterval = UnityEngine.Random.Range(minInterval,maxInterval);

            extraTime = maxInterval - nextInterval;

            float nextEventTime = nextInterval + currentTime;

            currentTime = nextEventTime;

            nextEventTime = levelTime - currentTime;

            timeRemaining = nextEventTime;

            GameObject gevent = (GameObject)GameObject.Instantiate(RandomWeightedEvent);
            GameEvent gEvent = gevent.GetComponent<GameEvent>();
            gEvent.eventTime = nextEventTime;

            gevent.transform.SetParent( levelRoot.transform );

            Debug.Log( "event added with time " + nextEventTime );

            game.events.Enqueue( gEvent );
        }

        game.levelTime = levelTime;
        game.StartLevel();
    }
}
