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

	float speed = 2.5f;

    //Used for enabling/disabling see-through door meshes
    public GameObject[] pillars;

    private int[,] stateMachine;
    private int[,] doorStateMachine = new int[,] { { 1, 0 }, { 1, 0 }, { -1, -1 }, { -1, -1 }, { -1, -1 }, { -1, -1 } };
    private int[,] door2StateMachine = new int[,] { { -1, -1 }, { -1, -1 }, { 1, 0 }, { -1, -1 }, { -1, -1 }, { -1, -1 } };
    private int[,] door3StateMachine = new int[,] { { -1, -1 }, { -1, -1 }, { -1, -1 }, { -1, -1 }, { 1, 0 }, { -1, -1 } };
    private int[,] enemyHallwayStateMachine = new int[,] { { 0, 0, 0 }, { 0, 1, 2 }, { 1, 1, 2 }, { 0, 1, 2 }, { 0, 2, 2 }, { 2, 2, 2 } };
    private int[,] enemyRotateStateMachine = new int[,] { { 0, 0, 0 }, { 0, 1, 2 }, { 1, 1, 2 }, { 0, 1, 2 }, { 0, 1, 2 }, { 2, 2, 2 } };

    // Use this for initialization
    void Start() {
        speed = 2.5f;

        agent = gameObject;
        agentName = gameObject.name;
        navAgent = GetComponent<NavMeshAgent>();

        pillars = GameObject.FindGameObjectsWithTag("Pillars");

        if (agentName == "Door")
        {
            stateMachine = doorStateMachine;
        }

        if (agentName == "Door 2")
        {
            stateMachine = door2StateMachine;
        }

        if (agentName == "Door 3")
        {
            stateMachine = door3StateMachine;
        }

        if (agentName == "Enemy Hallway")
        {
            stateMachine = enemyHallwayStateMachine;
        }

        if (agentName == "Enemy Rotate")
        {
            stateMachine = enemyRotateStateMachine;
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (agentName == "Door" || agentName == "Door 2" || agentName == "Door 3")
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
        } else if (agentName == "Enemy Hallway")
        {
            switch (caseNum)
            {
                case 0:
                    patrol();
                    currentState = 0;
                    break;

                case 1:
                    wobble();
                    patrol();
                    flash();
                    currentState = 1;
                    break;
                case 2:
                    kill();
                    currentState = 2;
                    break;
            }
        }
        else if (agentName == "Enemy Rotate")
        {
            switch (caseNum)
            {
                case 0:
                    patrol();
                    currentState = 0;
                    break;

                case 1:
                    patrol();
                    flash();
                    currentState = 1;
                    break;

                case 2:
                    patrol();
                    flash();
                    wobble();
                    currentState = 2;
                    break;

            }
        }
    }

    private void openDoor()
    {
        if (agentName == "Door 3")
        {
            BoxCollider collider = GetComponent<BoxCollider>();
            collider.enabled = false;

            foreach (GameObject pillar in pillars)
            {
                pillar.GetComponent<MeshRenderer>().enabled = false;
            }
        }
        else
        {
            MeshRenderer mesh = GetComponent<MeshRenderer>();
            BoxCollider collider = GetComponent<BoxCollider>();
            mesh.enabled = false;
            collider.enabled = false;
        }
    }

    private void closeDoor()
    {
        if (agentName == "Door 3")
        {
            BoxCollider collider = GetComponent<BoxCollider>();
            collider.enabled = true;

            foreach (GameObject pillar in pillars)
            {
                pillar.GetComponent<MeshRenderer>().enabled = true;
            }
        }
        else
        {
            MeshRenderer m = GetComponent<MeshRenderer>();
            BoxCollider c = GetComponent<BoxCollider>();
            m.enabled = true;
            c.enabled = true;
        }
    }

    public void postMessage(string m) {
		// get the trigger as an index
		int val = Convert.ToInt32(m);
        currentTrigger = val;
        caseNum = stateMachine[currentTrigger, currentState];

    }

	public void patrol() {
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

	public void attack() {

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

    public void wobble()
    {
        transform.rotation = Quaternion.LookRotation(new Vector3(UnityEngine.Random.Range(-1,1), UnityEngine.Random.Range(1, 1), UnityEngine.Random.Range(-5, 1)), new Vector3(0,0,0));
    }

    public void kill() {
        Destroy(gameObject);
    }

    public void flash()
    {
        GetComponent<Light>().enabled = true;
    }


    public void stopFlash()
    {
        GetComponent<Light>().enabled = false;
    }
}
