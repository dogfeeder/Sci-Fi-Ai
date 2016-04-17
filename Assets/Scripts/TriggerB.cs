using UnityEngine;
using System.Collections;

public class TriggerB : MonoBehaviour {

	void OnTriggerEnter(Collider other) {
		// get the receiver objects
		GameObject [] receivers = GameObject.FindGameObjectsWithTag("DFAAgent");
		// for each receiver object
		foreach (GameObject obj in receivers) {
			// Get the script
			AgentReceiver r = (AgentReceiver) obj.GetComponent(typeof(AgentReceiver));
			// call the receiver post method
			r.postMessage ("1");
		}
	}
}
