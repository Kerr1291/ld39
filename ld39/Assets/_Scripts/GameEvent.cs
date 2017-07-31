using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvent : MonoBehaviour
{
    //when the event will fire
    public float eventTime = 0f;

    public virtual void Run()
    {
        StartCoroutine( Running() );
    }

    protected virtual IEnumerator Running()
    {
        yield return 0;
    }
}
