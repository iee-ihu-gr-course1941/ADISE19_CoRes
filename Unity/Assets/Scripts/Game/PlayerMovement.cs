﻿using System.Collections;
using Schema;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private int routePosition;
    private Player player;
    public float rotationTime = 1f;
    public Route currentRoute;
    public Vector3 offset;

    private void Start()
    {
        var game = GameManager.Instance.Game;
        player = game.Players[transform.GetSiblingIndex()];
    }

    public IEnumerator Move(int location)
    {
        yield return new WaitForSecondsRealtime(3);
        
        var steps = location - player.Position;
        if (steps < 0) steps += 40;
        
        while (steps > 0)
        {
            routePosition++;
            routePosition %= currentRoute.childNodeList.Count;
            
            var nextPos = currentRoute.childNodeList[routePosition].position + offset;
            while (Step(nextPos)) yield return null; 
            
            yield return new WaitForSeconds(0.1f);
            
            steps--;
            
            if (routePosition % 10 == 0)
            {
                yield return StartCoroutine(RotateMe(Vector3.up * 90, rotationTime));
            }
        }

        player.SetPosition(location);
    }
    
    IEnumerator RotateMe(Vector3 byAngles, float inTime) 
    {   
        var fromAngle = transform.rotation;
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
