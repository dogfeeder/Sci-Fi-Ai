using UnityEngine;
using System;

public class AgentReceiver : MonoBehaviour {

    //Agent
    private GameObject agent;
    private NavMeshAgent navAgent;
    private string agentName;

    //State vars
    private int caseNum;
    private int currentState;
    private int currentTrigger;

    // the start and end positions of patrol

    public GameObject[] patrolPoints;
    public int currentPatrolPoint = 0;
    private float patrolPointDistance = 1.0f;

    private Vector3 patrol_start;
	private Vector3 patrol_end;
	private Vector3 hide_pos;
	private Vector3 centre_pos;
	private Vector3 target_patrol;
	float speed = 2.5f;

    private int[,] stateMachine;
    private int[,] doorStateMachine = new int[,] { {1,0}, {-1,-1} };
    private int[,] enemyStateMachine = new int[,] { {0,0}, {1,0} };

    // Use this for initialization
    void Start () {
		speed = 2.5f;

        agent = gameObject;
        agentName = gameObject.name;
        navAgent = GetComponent<NavMeshAgent>();
        

        if (agentName == "Door")
        {
            stateMachine = doorStateMachine;
        }

        if (agentName == "Enemy")
        {
            stateMachine = enemyStateMachine;
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
        } else if (agentName == "Enemy")
        {
            switch (caseNum)
            {
                case 0:
                    patrol();
                    currentState = 0;
                    break;

                case 1:
                    chase();
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

        currentTrigger = val;
        caseNum = stateMachine[currentTrigger, currentState];

    }

	public void patrol() {
        Debug.Log("patrolling");
        //Move towards current patrol point
        navAgent.SetDestination(patrolPoints[currentPatrolPoint].transform.position);

        //Close to/arrived at patrol point. Switch to next/first patrol point
        if (Vector3.Distance(transform.position, patrolPoints[currentPatrolPoint].transform.position) < patrolPointDistance)
        {

            if (currentPatrolPoint == patrolPoints.Length - 1)
                currentPatrolPoint = 0;
            else
                currentPatrolPoint++;
        }
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
        Debug.Log("attacking");
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


    public void chase()
    {
        Debug.Log("Chase");

        Vector3 targetDir;
        Vector3 newDir;

        // Rotating to follow player hasn't been finished.
        targetDir = (transform.position);
        transform.position = Vector3.MoveTowards(transform.position, agent.transform.position, Time.deltaTime * speed);
        newDir = Vector3.RotateTowards(transform.forward, targetDir, 2.0f * Time.deltaTime, 0.0F);
        transform.rotation = Quaternion.LookRotation(newDir - agent.transform.position);
    }
}
