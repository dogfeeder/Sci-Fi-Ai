using UnityEngine;
using System;

public class AgentReceiver : MonoBehaviour {

    //Agent
    private GameObject agent;
    private string agentName;

    //State vars
    private int caseNum;
    private int currentState;
    private int currentTrigger;

    // the start and end positions of patrol
    private Vector3 patrol_start;
	private Vector3 patrol_end;
	private Vector3 hide_pos;
	private Vector3 centre_pos;
	private Vector3 target_patrol;
	float speed = 2.5f;

    private int[,] stateMachine;
    private int[,] doorStateMachine = new int[,] { {1,0} };

	// Use this for initialization
	void Start () {
		patrol_start = new Vector3 (1414.4f, 9.5f, 1301.5f);
		patrol_end = new Vector3 (1414.4f, 9.5f, 1326.5f);
		hide_pos = new Vector3 (1416.7f, 9.5f, 1317.3f);
		centre_pos = new Vector3 (1414.4f, 9.5f, 1317.3f);
		target_patrol = centre_pos;
		speed = 2.5f;

        agent = this.gameObject;
        agentName = this.gameObject.name;
        

        if (agentName == "Door")
        {
            stateMachine = doorStateMachine;
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (agentName == "Door")
        {
            switch (caseNum)
            {
                case 0:
                    closeDoor();
                    currentState = 0;
                    break;

                case 1:
                    openDoor();
                    currentState = 1;
                    break;
            }
        }
    }

    private void openDoor()
    {
        MeshRenderer m = GetComponent<MeshRenderer>();
        BoxCollider c = GetComponent<BoxCollider>();
        m.enabled = false;
        c.enabled = false;
    }

    private void closeDoor()
    {
        MeshRenderer m = GetComponent<MeshRenderer>();
        BoxCollider c = GetComponent<BoxCollider>();
        m.enabled = true;
        c.enabled = true;
    }

    public void postMessage(string m) {
		// get the trigger as an index
		int val = Convert.ToInt32(m);

        currentTrigger = val - 1;
        caseNum = stateMachine[currentTrigger, currentState];

    }

	public void patrol() {
		Vector3 targetDir;
		Vector3 newDir;
		// if at start
		if (Vector3.Distance(transform.position, patrol_start) < 0.1f) {
			target_patrol = patrol_end;
		} 
		if (Vector3.Distance(transform.position, patrol_end) < 0.1f) {
			target_patrol = patrol_start;
		}
		targetDir = target_patrol - transform.position;
		newDir = Vector3.RotateTowards (transform.forward, targetDir, 2.0f * Time.deltaTime, 0.0F);
		transform.rotation = Quaternion.LookRotation (newDir);
		transform.position = Vector3.MoveTowards (transform.position, target_patrol, Time.deltaTime * speed);
	}

	public void hide() {
		Vector3 targetDir;
		Vector3 newDir;
		// if at centre position
		if (Vector3.Distance(transform.position, centre_pos) < 0.1f) {
			target_patrol = hide_pos;
		}
		// if at hide position
		if (Vector3.Distance(transform.position, hide_pos) < 0.1f) {
			targetDir = centre_pos - transform.position;
			newDir = Vector3.RotateTowards (transform.forward, targetDir, 2.0f * Time.deltaTime, 0.0F);
			transform.rotation = Quaternion.LookRotation (newDir);
		}
		targetDir = target_patrol - transform.position;
		newDir = Vector3.RotateTowards (transform.forward, targetDir, 2.0f * Time.deltaTime, 0.0F);
		transform.rotation = Quaternion.LookRotation (newDir);
		transform.position = Vector3.MoveTowards (transform.position, target_patrol, Time.deltaTime * speed);
	}

	public void attack() {
		Vector3 targetDir;
		Vector3 newDir;
		// if at start
		if (Vector3.Distance(transform.position, hide_pos) < 0.1f) {
			target_patrol = centre_pos;
		} 
		if (Vector3.Distance(transform.position, centre_pos) < 0.1f) {
			target_patrol = Camera.main.transform.position;
			target_patrol.y = 9.5f;
		}

		targetDir = target_patrol - transform.position;
		newDir = Vector3.RotateTowards (transform.forward, targetDir, 2.0f * Time.deltaTime, 0.0F);
		transform.rotation = Quaternion.LookRotation (newDir);
		transform.position = Vector3.MoveTowards (transform.position, target_patrol, Time.deltaTime * speed);
	}
}
