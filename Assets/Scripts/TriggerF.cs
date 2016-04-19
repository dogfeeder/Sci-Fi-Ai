using UnityEngine;
using System.Collections;

public class TriggerF : MonoBehaviour
{

    private bool triggered;
    public GameObject enemyPathfinder;

    void OnTriggerEnter(Collider other)
    {
        if (!triggered)
        {
            enemyPathfinder.GetComponent<pathAgentScript>().pathTrigger = true;
            AudioSource source = GetComponent<AudioSource>();
            source.Play();
            // get the receiver objects
            GameObject[] receivers = GameObject.FindGameObjectsWithTag("DFAAgent");
            // for each receiver object
            foreach (GameObject obj in receivers)
            {
                // Get the script
                AgentReceiver r = (AgentReceiver)obj.GetComponent(typeof(AgentReceiver));
                // call the receiver post method
                triggered = true;
                r.postMessage("5");
            }
        }
    }

}
