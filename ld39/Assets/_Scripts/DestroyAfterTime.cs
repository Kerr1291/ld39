using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    [SerializeField]
    float destroyAfterTime = 1f;

    // Use this for initialization
    IEnumerator Start()
    {
        yield return new WaitForSeconds( destroyAfterTime );
        Destroy( gameObject );
    }
}
