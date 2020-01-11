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
    public float rotationTime = 1f;

    private bool isMoving;

    public Vector3 offset;

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
            
            Vector3 nextPos = currentRoute.childNodeList[routePosition].position + offset;
            while (Step(nextPos)) { yield return null; }
            yield return new WaitForSeconds(0.1f);
            steps--;
            if (routePosition % 10 == 0)
            {
                yield return StartCoroutine(RotateMe(Vector3.up * 90, rotationTime));
            }
        }

        isMoving = false;
    }
    
    IEnumerator RotateMe(Vector3 byAngles, float inTime) 
    {    var fromAngle = transform.rotation;
        var toAngle = Quaternion.Euler(transform.eulerAngles + byAngles);
        for(var t = 0f; t <= 1; t += Time.deltaTime/inTime) {
            transform.rotation = Quaternion.Slerp(fromAngle, toAngle, t);
            yield return null;
        }
        transform.rotation = toAngle;
    }

    bool Step(Vector3 goal)
    {
        return goal != (transform.position = Vector3.MoveTowards(transform.position, goal, 5f * Time.deltaTime));
    }
}
