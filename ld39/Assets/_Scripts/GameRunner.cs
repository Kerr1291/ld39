using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameRunner : MonoBehaviour
{
    public TextMesh scoreText;

    public TextMesh timeRemainingText;

    public List<Module> allModules;

    public float levelTime;

    //public float minEventTime;
    //public float maxEventTime;

    public float loadTime;

    public GameObject startScreen;
    public PlayerController player;

    public PulseCore powerCore;

    public Queue<GameEvent> events;

    public Level1 levelSetup;

    public int startLevel = 1;

    public GameObject levelRoot;

    private int score = 0;

    public GameObject floatingText;

    public int Score {
        get {
            return score;
        }

        set {
            if(value > 0)
            {

                float newscore = value;
                float oldscore = score;

                float delta = newscore - oldscore;

                GameObject text = (GameObject)GameObject.Instantiate(floatingText);
                TextMesh tm = text.GetComponent<TextMesh>();
                text.transform.position = player.agent.transform.position;
                tm.color = Color.yellow;
                tm.text = "+"+System.Convert.ToString( (int)( delta ) );
            }

            score = value;
            scoreText.text = System.Convert.ToString( score );
        }
    }

    private void Awake()
    {
        events = new Queue<GameEvent>();
    }

    void Start()
    {
        if( !startScreen.activeInHierarchy )
        {
            StartGame();
        }
    }

    public void StartGame()
    {
        if( allModules.Count < 1 )
            allModules = GameObject.FindObjectsOfType<Module>().ToList();
        foreach( var v in allModules )
        {
            v.Power = 1f;
            v.gameObject.SetActive( true );
        }

        startScreen.SetActive( false );
        player.Active = true;
        levelRoot.SetActive( true );
        powerCore.CoreHealth = 1f;
        levelSetup.Setup( this, startLevel );
        Score = 0;
    }

    public void EndGame()
    {
        foreach(var v in GameObject.FindObjectsOfType<PowerUp>())
        {
            Destroy( v.gameObject );
        }

        events.Clear();
        Debug.Log( "Game over! Score:"+Score );
        player.Active = false;
        startScreen.SetActive( true );
        levelRoot.SetActive( false );
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void StartLevel()
    {
        StopAllCoroutines();
        StartCoroutine( PlayLevel() );
    }

    IEnumerator PlayLevel()
    { 
        yield return new WaitForSeconds( loadTime );

        //float nextEvent = UnityEngine.Random.Range(minEventTime,maxEventTime);

        float timeRemaining = levelTime;
        while( timeRemaining > 0f )
        {
            //TODO: check game conditions

            timeRemainingText.text = System.Convert.ToString( (int)timeRemaining );

            float dt = Time.fixedDeltaTime * Time.timeScale;
            timeRemaining -= dt;

            if( events.Count > 0 && events.Peek().eventTime > timeRemaining )
            {
                GameEvent ge = events.Dequeue();
                ge.Run();
            }

            yield return new WaitForFixedUpdate();
        }

        timeRemainingText.text = "Level Complete!";

        Score += 10;

        yield return new WaitForSeconds( loadTime );
        NextLevel();
        yield return 0;
    }

    public void NextLevel()
    {
        startLevel++;
        levelSetup.Setup( this, startLevel );
    }


    //void DoNextEvent()
    //{
    //    int module = UnityEngine.Random.Range(0,allModules.Count);

    //    //TODO: select event from a list of loaded events
    //    if( module < allModules.Count && allModules[ module ] != null )
    //    {
    //        allModules[ module ].Power -= .1f;
    //        Debug.Log( allModules[ module ].name + " damaged to " + allModules[ module ].Power );
    //    }
    //}

}
