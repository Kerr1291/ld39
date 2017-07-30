using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Level1 : MonoBehaviour
{
    public TextMesh timeRemainingText;

    public List<Module> allModules;

    public float levelTime = 30f;

    public float minEventTime = 1f;
    public float maxEventTime = 5f;

    public float loadTime = .1f;

    IEnumerator Start()
    {
        yield return new WaitForSeconds( loadTime );
        allModules = GameObject.FindObjectsOfType<Module>().ToList();
        yield return null;

        float nextEvent = UnityEngine.Random.Range(minEventTime,maxEventTime);

        float timeRemaining = levelTime;
        while( timeRemaining > 0f)
        {
            //TODO: check game conditions

            timeRemainingText.text = System.Convert.ToString( (int)timeRemaining );            

            float dt = Time.fixedDeltaTime * Time.timeScale;
            timeRemaining -= dt;
            nextEvent -= dt;

            if(nextEvent <= 0f)
            {
                nextEvent = UnityEngine.Random.Range( minEventTime, maxEventTime );
                DoNextEvent();
            }

            yield return new WaitForFixedUpdate();
        }

        timeRemainingText.text = "Level Complete!";

        yield return 0;
    }

    void DoNextEvent()
    {
        int module = UnityEngine.Random.Range(0,allModules.Count);
        
        //TODO: select event from a list of loaded events
        if( module < allModules.Count && allModules[module] != null )
        {
            allModules[ module ].Power -= .1f;
            Debug.Log( allModules[ module ].name + " damaged to " + allModules[ module ].Power );
        }
    }

}
