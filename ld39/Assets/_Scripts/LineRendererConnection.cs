using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(LineRenderer))]
public class LineRendererConnection : MonoBehaviour {

    LineRenderer lr;

    public Transform a;
    public Transform b;

    private void Reset()
    {
        lr = GetComponent<LineRenderer>();
    }

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update () {

        if( a == null )
            return;

        if( b == null )
            return;

        lr.SetPosition( 0, a.transform.position );
        lr.SetPosition( 1, b.transform.position );
    }
}
