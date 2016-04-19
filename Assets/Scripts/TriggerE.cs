using UnityEngine;
using System.Collections;

public class TriggerE : MonoBehaviour {

    private bool triggered;
    public AudioClip explosion;
    public GameObject enemy;

	void OnTriggerEnter(Collider other) {
        if (!triggered)
        {
            AudioSource source = GetComponent<AudioSource>();
            source.clip = explosion;
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
                r.postMessage("4");
            }
        }

    }
}
