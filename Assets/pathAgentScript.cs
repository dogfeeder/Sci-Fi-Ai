using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct search_node
{
	public Vector3 cell;
	public int parent;
	public float g_value;

	public search_node(Vector3 a, int b, float c) {
		cell = a;
		parent = b;
		g_value = c;
	}
}

public class pathAgentScript : MonoBehaviour {

    public Transform player;
	// the gris size
	public float grid_width = 0.1f;
	// the list of wall references
	private GameObject [] walls;
	// the current position of this object
	private Vector3 curr_pos;
	// the path
	private LinkedList<Vector3> path = null;
	// mutex for A-star coroutine
	private bool isPathfinding;

	// Use this for initialization
	void Start () {
		// Gets a reference to every wall in the scene
		walls = GameObject.FindGameObjectsWithTag ("obstacle");
		// get the current poistion
		curr_pos = this.transform.position;
		// not pathfinding yet
		isPathfinding = false;
	}
	
	// Update is called once per frame
	void Update () {
		// if there was a mouse click
		if (!isPathfinding && Input.GetButtonDown("Fire1")) {
			// get the hit point
            Vector3 point = player.position;
			// if the point is in the maze
			// if the point is not under a wall
			if (!IsPointTooNearWall(point, grid_width)) {
				Debug.Log("OK");
				StartCoroutine(AStarCoroutine(point, grid_width));
				//AStarPathSearch(point, grid_width);
			} else {
				Debug.Log("Wall");
			}
		}

        if (path != null && path.Count > 0 && !moving)
        {
            // get the first
            Vector3 pos = path.First.Value;
            Debug.Log(pos.ToString());
            pos.y = transform.position.y;
            transform.Translate((pos - transform.position));
            if (path.Count == 1)
            {
                curr_pos = pos;
            }
            path.RemoveFirst();
        }
	}


	/*
	 * This method checks a goal position against all of the walls to test
	 * if the position is inside a wall.
	 */
	public bool IsPointTooNearWall(Vector3 p, float grid_size) {
		// define bound
		Bounds bp = new Bounds(p, new Vector3(grid_size, grid_size, grid_size));
		// for every wall
		foreach (GameObject w in walls) {
			// get the wall bounds
			Bounds b = w.GetComponent<Collider>().bounds;
			// if the wall contains point p
			if (b.Intersects (bp)) {
				//Debug.Log("Bounds intersect");
				return true;
			}
		}
		return false;
	}

	/*
	 * This method just tests if a position is outside of the maze.
	 */
	public bool isInMaze(Vector3 p) {
		if (p.z > -12f && p.z < 12f && p.x > -12f && p.x < 12f) {
			return true;
		}
		return false;
	}

	/*
	 * This method gets the nearest grid centre
	 */ 
	public Vector3 getNearestGridCenter(Vector3 pos, float grid_size) {
		// compute the number of grids dividing x and z
		float g_x = grid_size * Mathf.Round(pos.x / grid_size);
		float g_z = grid_size * Mathf.Round(pos.z / grid_size);
		return new Vector3 (g_x, pos.y, g_z);
	}

	/*
	 * This method performs the A* search and returns a path or null
	 * if no path can be found. 
	 */
	public LinkedList<Vector3> AStarPathSearch(Vector3 goal, float grid_size) {

		isPathfinding = true;

        // the open set		
        SortedList<float, search_node> open = new SortedList<float, search_node>();
        // make the root search node from current position
        search_node root = new search_node(curr_pos, -1, 0.0f);
        // add the root
        open.Add(Vector3.Distance(curr_pos, goal), root);
        // the closed set
        SortedList<int, search_node> closed = new SortedList<int, search_node>();
        // the child list
        List<search_node> children = new List<search_node>();
        // the current search node
        search_node X;

        // while the open set is not empty
        while (open.Count > 0)
        {
            // get the next search node
            X = open.Values[0];
            // remove at index
            open.RemoveAt(0);
            // the index of the parent
            int par_ind = closed.Count;
            // add to the closed set
            closed.Add(par_ind, X);
            // the g value for all children
            float the_g_value = X.g_value + grid_size;
            // add the children
            children.Add(new search_node(new Vector3(X.cell.x + grid_size, X.cell.y, X.cell.z), par_ind, the_g_value));
            children.Add(new search_node(new Vector3(X.cell.x - grid_size, X.cell.y, X.cell.z), par_ind, the_g_value));
            children.Add(new search_node(new Vector3(X.cell.x, X.cell.y, X.cell.z + grid_size), par_ind, the_g_value));
            children.Add(new search_node(new Vector3(X.cell.x, X.cell.y, X.cell.z - grid_size), par_ind, the_g_value));
            // the g value for all children
            the_g_value = X.g_value + (Mathf.Sqrt(2.0f) * grid_size);
            children.Add(new search_node(new Vector3(X.cell.x + grid_size, X.cell.y, X.cell.z + grid_size), par_ind, the_g_value));
            children.Add(new search_node(new Vector3(X.cell.x - grid_size, X.cell.y, X.cell.z + grid_size), par_ind, the_g_value));
            children.Add(new search_node(new Vector3(X.cell.x + grid_size, X.cell.y, X.cell.z - grid_size), par_ind, the_g_value));
            children.Add(new search_node(new Vector3(X.cell.x - grid_size, X.cell.y, X.cell.z - grid_size), par_ind, the_g_value));
            // for each child
            while (children.Count > 0)
            {
                // get a child
                X = children[0];
                // remove from the list
                children.RemoveAt(0);
                // if the child is not in a wall
                if (!IsPointTooNearWall(X.cell, grid_size))
                {
                    // if the distance to the goal is less that grid size
                    if (Vector3.Distance(X.cell, goal) <= grid_size)
                    {
                        // we have the goal
                        LinkedList<Vector3> retList = new LinkedList<Vector3>();
                        // add to the front
                        retList.AddFirst(goal);
                        retList.AddFirst(X.cell);
                        // while not at root construct parent-wise path
                        while (X.parent != -1)
                        {
                            // if fails to get the next parent
                            if (!closed.TryGetValue(X.parent, out X))
                            {
                                isPathfinding = false;
                                return null;
                            }
                            Debug.Log("Path " + X.cell.ToString());
                            // add to path list
                            retList.AddFirst(X.cell);
                        }
                        isPathfinding = false;
                        return retList;
                    }
                    // if not in the opened and closed lists 
                    if (!isInList<float>(X.cell, open) && !isInList<int>(X.cell, closed))
                    {
                        // add to the opened list
                        open.Add(X.g_value + Vector3.Distance(X.cell, goal) + 0.1f * Random.value, X);
                    }

                }


            }
        }
        isPathfinding = false;
        return null;
	}

	public bool isInList<T>(Vector3 p, SortedList<T,search_node> l) {
		// get count
		int size = l.Count;
		// for every element
		for (int i  = 0; i < size; i++) {
			if (l.Values[i].cell == p) {
				return true;
			}
		}
			return false;
	}

	/*
	 * A coroutine to run path find without locking Update
	 */
	IEnumerator AStarCoroutine(Vector3 g, float g_size) {
		// search for a path
		path = AStarPathSearch(g, g_size);
		yield return 0;
	}

}

