using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    [Header("Set by user")]
    public NavMeshAgent agent;
    public Camera gameCamera;

    public bool canMove = true;
    public bool canUse = true;


    public KeyCode moveKey = KeyCode.Mouse1;
    public KeyCode useKey = KeyCode.Mouse0;

    public LayerMask groundLayer;
    public string groundTag;

    public float extraGroundMoveHeight = 0f;

    public bool debugDraw = false;


    public CapsuleCollider useRange;
    public LayerMask useLayer;

    public AudioSource useSound;

    [Header("Set by script")]
    public Vector3 destination;

    public Module willUse;
    public Module isUsing;

    public float standDistance;

    public float useDelay = .1f;

    public float repairPower = .1f;

    public bool Active {
        get {
            return agent.gameObject.activeInHierarchy;
        }
        set {
            agent.gameObject.SetActive( value );
        }
    }

    public float DistToDestionation {
        get {
            return ( destination - PlayerPos ).magnitude;
        }
    }

    Vector3 PlayerPos { get {
            return agent.transform.position;
        }
    }

    public Module WillUse {
        get {
            return willUse;
        }

        set {
            willUse = value;
        }
    }

    void ClearModules()
    {
        WillUse = null;
        isUsing = null;
    }

    IEnumerator Start()
    {
        standDistance = useRange.radius * .7f;
        ClearModules();
        yield return 0;
        StartCoroutine( UseModule() );
    }

    IEnumerator UseModule()
    {
        for( ;;)
        {
            if( isUsing == null )
            {
                yield return null;
                continue;
            }

            if( isUsing != null )
            {
                if( !isUsing.CanUse )
                {
                    isUsing = null;
                }
                else
                {
                    if( isUsing.Use( this ) )
                    {
                        if( useSound != null )
                            useSound.Play();
                    }
                    else
                    {
                        isUsing = null;
                    }
                }
            }
            yield return new WaitForSeconds( useDelay );
        }
    }

    /*
    Debug.Log( System.Reflection.MethodInfo.GetCurrentMethod().Name );
    */
    
    bool IsNearEnoughToUse( Module m )
    {
        return useRange.bounds.Contains( m.transform.position );
    }

    Vector3 GetGroundPoint()
    {
        Vector3 mpos = Input.mousePosition;
        Ray moveRay = gameCamera.ScreenPointToRay(mpos);
        RaycastHit[] groundCast = Physics.RaycastAll(moveRay);
        int found = -1;
        for( int i = 0; i < groundCast.Length; ++i )
        {
            if( groundCast[ i ].collider.gameObject.CompareTag( groundTag ) )
            {
                found = i;
                break;
            }
        }
        if( found >= 0 )
        {
            Vector3 groundPoint = groundCast[ found ].point;
            groundPoint.y += extraGroundMoveHeight;
            return groundPoint;
        }
        return Vector3.zero;
    }

    void TryUse()
    {
        if( Input.GetKeyDown( useKey ) )
        {
            if( canUse )
            {
                Module m = null;
                Vector3 mpos = Input.mousePosition;
                Ray useRay = gameCamera.ScreenPointToRay(mpos);
                RaycastHit[] useRaycast = Physics.RaycastAll(useRay);
                int found = -1;
                for( int i = 0; i < useRaycast.Length; ++i )
                {
                    m = useRaycast[ i ].collider.GetComponent<Module>();
                    if( m != null )
                    {
                        if( m.CanUse )
                        {
                            found = i;
                            break;
                        }
                    }
                }


                if( found >= 0 )
                {
                    WillUse = null;
                    isUsing = null;
                    if( IsNearEnoughToUse( m ) )
                    {
                        isUsing = m;
                    }
                    else if( canMove )
                    {
                        WillUse = m;

                        Vector3 modulePos = m.transform.position;
                        Vector3 groundPoint = GetGroundPoint();
                        modulePos.y = groundPoint.y;

                        Vector3 toplayer = PlayerPos - modulePos;
                        toplayer.y = 0f;

                        Vector3 moveToPos = modulePos + toplayer.normalized * standDistance;

                        destination = moveToPos;

                        agent.SetDestination( destination );
                    }
                    else
                    {
                        Debug.Log( "Can't move to reach module!" );
                    }
                }
                else if( canMove )
                {
                    TryMove( useKey );
                }
            }
            else
            {
                if( canMove )
                {
                    TryMove( useKey );
                }
                else
                {
                    Debug.Log( "Can't use anything right now!" );
                }
            }
        }
    }

    void TryMove(KeyCode mKey)
    {
        if( Input.GetKeyDown( mKey ) )
        {
            if( canMove )
            {
                Vector3 mpos = Input.mousePosition;
                Ray moveRay = gameCamera.ScreenPointToRay(mpos);
                RaycastHit[] groundCast = Physics.RaycastAll(moveRay);
                int found = -1;
                for( int i = 0; i < groundCast.Length; ++i )
                {
                    if( groundCast[ i ].collider.gameObject.CompareTag( groundTag ) )
                    {
                        found = i;
                        break;
                    }
                }
                if( found >= 0 )
                {
                    WillUse = null;
                    isUsing = null;
                    destination = groundCast[ found ].point;
                    destination.y += extraGroundMoveHeight;
                    agent.SetDestination( destination );
                }
            }
            else
            {
                Debug.Log( "Can't move to destination!" );
            }
        }
    }

    void DebugDraw()
    {
        if( debugDraw )
        {
            Debug.DrawLine( PlayerPos, destination, Color.blue );
        }

        if( WillUse != null )
        {
            Debug.DrawLine( PlayerPos, WillUse.transform.position, Color.yellow );
        }

        if( isUsing != null )
        {
            Debug.DrawLine( PlayerPos, isUsing.transform.position, Color.green );
        }
    }


    void Update()
    {
        TryMove(moveKey);
        TryUse();
        DebugDraw();
        if( willUse != null && DistToDestionation < .01f )
        {
            //TODO throw error?
            willUse = null;
            Debug.Log( "Cannot reach destionation" );
        }
    }

    private void OnTriggerEnter( Collider other )
    {
        Module m = other.gameObject.GetComponent<Module>();

        if( m != null )
        {
            if( WillUse != null && WillUse == m )
            {
                //Debug.Log( System.Reflection.MethodInfo.GetCurrentMethod().Name + " " + m.name );
                WillUse = null;
                isUsing = m;
            }
        }
    }
}
