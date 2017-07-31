using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour {
    
    public Vector3 velocity;

	// Update is called once per frame
	void FixedUpdate () {
        transform.Translate( velocity );
	}
}
