﻿using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

    public Transform player;
    NavMeshAgent agent;

	// Use this for initialization
	void Start () {
        agent = GetComponent<NavMeshAgent>();	
	}
	
	// Update is called once per frame
	void Update () {
        agent.destination = player.position;
	}
}
