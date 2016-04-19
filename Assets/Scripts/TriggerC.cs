using UnityEngine;
using System.Collections;

public class TriggerC : MonoBehaviour {
    private bool triggered;

	void OnTriggerEnter(Collider other) {
        if (!triggered)
        {
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
                r.postMessage("2");
            }
        }
	}
}
