using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulseLight : MonoBehaviour {

    public Light lightToPulse;
    public float pulseRate = 1f;
    public float pulseRange = 1f;
    float pulseMod = 0f;

    private void Awake()
    {
        if( lightToPulse == null )
            lightToPulse = GetComponent<Light>();
    }

    IEnumerator Start () {
		
        for( ;;)
        {
            float amount = Time.fixedDeltaTime;
            while(pulseMod < pulseRange)
            {
                pulseMod += amount * Time.timeScale * pulseRate;

                lightToPulse.intensity += amount * Time.timeScale * pulseRate;

                yield return new WaitForFixedUpdate();
            }

            while( pulseMod > -pulseRange )
            {
                pulseMod -= amount * Time.timeScale * pulseRate;

                lightToPulse.intensity -= amount * Time.timeScale * pulseRate;

                yield return new WaitForFixedUpdate();
            }
        }
	}
}
