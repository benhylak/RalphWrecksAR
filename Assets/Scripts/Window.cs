using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Window : MonoBehaviour {

	public FracturedObject windowPane;

	bool hammerIsInside = false;

	public List<Brick> bricks = new List<Brick>(); //bricks thrown at the window. cleared when fixed;
	
	// Use this for initialization
	void Start () {	
		
	}
	
	// Update is called once per frame
	void Update () {
		if(GameManager.Instance.isFixing && hammerIsInside)
		{
			windowPane.ResetChunks();

			foreach(var brick in bricks)
			{
				Destroy(brick.gameObject);
			}

			bricks.Clear();
		}
	}

	void OnCollisionEnter(Collision collision)
	{
		if(collision.gameObject.tag == "Player")
		{
			hammerIsInside = true;
		}
	}

	void OnCollisionExit(Collision collision)
	{
		if(collision.gameObject.tag == "Player")
		{
			hammerIsInside = false;
		}
	}
}
