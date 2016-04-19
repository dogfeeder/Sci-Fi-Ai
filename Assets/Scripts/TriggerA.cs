using UnityEngine;
using System.Collections;

public class TriggerA : MonoBehaviour {

    AudioSource source;
    public AudioClip open;
    public AudioClip closed;

    public GameObject enemyPathfinder;

    void Start()
    {
        source = GetComponent<AudioSource>();
    }

	void OnTriggerEnter(Collider other)
    {
        source.clip = open;
        source.Play();
		// get the receiver objects
		GameObject [] receivers = GameObject.FindGameObjectsWithTag("DFAAgent");
		// for each receiver object
		foreach (GameObject obj in receivers) {
			// Get the script
			AgentReceiver r = (AgentReceiver) obj.GetComponent(typeof(AgentReceiver));
			// call the receiver post method
			r.postMessage ("0");
		}

	}

    void OnTriggerExit(Collider other)
    {
        enemyPathfinder.GetComponent<pathAgentScript>().pathTrigger = true;
        source.clip = closed;
        source.Play();
        // get the receiver objects
        GameObject[] receivers = GameObject.FindGameObjectsWithTag("DFAAgent");
        // for each receiver object
        foreach (GameObject obj in receivers)
        {
            // Get the script
            AgentReceiver r = (AgentReceiver)obj.GetComponent(typeof(AgentReceiver));
            // call the receiver post method
            r.postMessage("0");
        }

    }

}
