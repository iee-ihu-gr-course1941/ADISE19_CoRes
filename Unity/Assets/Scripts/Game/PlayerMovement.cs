using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerMovement : MonoBehaviour
{
    public Route currentRoute;
    
    private int routePosition;
    
    public int steps;

    private bool isMoving;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isMoving)
        {
            int die1 = Random.Range(1, 7);
            int die2 = Random.Range(1, 7);
            steps = die1 + die2;
            Debug.Log("Rolled: " + die1 + " + " + + die2 + " = " + steps);

            StartCoroutine(Move());
        }
    }

    IEnumerator Move()
    {
        if (isMoving)
        {
            yield break;
        }
        isMoving = true;

        while (steps > 0)
        {
            routePosition++;
            routePosition %= currentRoute.childNodeList.Count;
            
            Vector3 nextPos = currentRoute.childNodeList[routePosition].position;
            while (Step(nextPos)) { yield return null; }
            yield return new WaitForSeconds(0.1f);
            steps--;
        }

        isMoving = false;
    }

    bool Step(Vector3 goal)
    {
        return goal != (transform.position = Vector3.MoveTowards(transform.position, goal, 5f * Time.deltaTime));
    }
}
