using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public Light doorLight;

    public BoxCollider doorTrigger;

    public Transform leftDoor;
    public Transform rightDoor;

    public float openDistance = 1f;

    public float timeOffset = .2f;
    public float openTime = 1f;
    public float stayTime = 1f;
    public float closeTime = 1f;

    public float doorLightLevel = 3f;

    Vector3 leftStart;
    Vector3 rightStart;

    Vector3 leftEnd {
        get {
            return new Vector3( 0f, 0f, -openDistance ) + leftStart;
        }
    }
    Vector3 rightEnd {
        get {
            return new Vector3( 0f, 0f, openDistance ) + rightStart;
        }
    }

    private void Awake()
    {
        leftStart = leftDoor.position;
        rightStart = rightDoor.position;
    }

    IEnumerator Open()
    {
        float t = 0f;
        t = openTime + timeOffset;
        while(t > 0f)
        {
            float normTime = 1f - t / openTime;

            leftDoor.position = Vector3.Lerp( leftStart, leftEnd, normTime );
            rightDoor.position = Vector3.Lerp( rightStart, rightEnd, normTime );

            doorLight.intensity = Mathf.Lerp( 0f, doorLightLevel, normTime );

            t -= Time.fixedDeltaTime * Time.timeScale;
            yield return new WaitForFixedUpdate();
        }

        t = stayTime;
        while( t > 0f )
        {
            t -= Time.fixedDeltaTime * Time.timeScale;
            yield return new WaitForFixedUpdate();
        }

        t = closeTime + timeOffset;
        while( t > 0f )
        {
            float normTime = 1f - t / openTime;

            leftDoor.position = Vector3.Lerp( leftEnd, leftStart, normTime );
            rightDoor.position = Vector3.Lerp( rightEnd, rightStart, normTime );

            doorLight.intensity = Mathf.Lerp( doorLightLevel, 0f, normTime );

            t -= Time.fixedDeltaTime * Time.timeScale;
            yield return new WaitForFixedUpdate();
        }

        yield return 0;
    }

    private void OnTriggerEnter( Collider other )
    {
        OpenDoor();
    }

    [ContextMenu("OpenDoor")]
    void OpenDoor()
    {
        StopAllCoroutines();
        StartCoroutine( Open() );
    }
}
