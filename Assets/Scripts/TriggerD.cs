using UnityEngine;
using System.Collections;

public class TriggerD : MonoBehaviour {

    public GameObject enemyPathfinder;
    public bool triggered;

    void OnTriggerEnter(Collider other) {
        if (!triggered)
        {
            enemyPathfinder.GetComponent<pathAgentScript>().pathTrigger = true;
            // get the receiver objects
            GameObject[] receivers = GameObject.FindGameObjectsWithTag("DFAAgent");
            // for each receiver object
            foreach (GameObject obj in receivers)
            {
                // Get the script
                AgentReceiver r = (AgentReceiver)obj.GetComponent(typeof(AgentReceiver));
                // call the receiver post method
                triggered = true;
                r.postMessage("3");
            }
        }
		
	}
}
