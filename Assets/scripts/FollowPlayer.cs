using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    // public Transform target;
    // public float SmoothDamptime = 0.15f;
    // private Vector3 SmoothDampVelocity = Vector3.zero;
    // public Transform LeftBounds;
    // public Transform RightBounds;
    // private float levelMin, levelMax, camHeight, camWidth;
    
    public Transform target;
    private Vector3 offset = new Vector3(0,0,-10);
    private float smoothtime = 0.25f;
    private Vector3 velocity = Vector3.zero;
    void Start()
    {
        // camHeight = Camera.main.orthographicSize * 2;
        // camWidth = camHeight * Camera.main.aspect;

        // float LeftBoundswidth = LeftBounds.GetComponentInChildren<SpriteRenderer> ().bounds.size.x / 2;
        // float RightBoundswidth = RightBounds.GetComponentInChildren<SpriteRenderer> ().bounds.size.x / 2;

        // levelMin = LeftBounds.position.x + LeftBoundswidth + (camWidth/2);
        // levelMax = RightBounds.position.x + RightBoundswidth - (camWidth/2);
    }

    void Update()
    {
        // float Targetx = Mathf.Max(levelMin,Mathf.Min(levelMax,target.position.x));
        // float x = Mathf.SmoothDamp(transform.position.x, Targetx,ref SmoothDampVelocity.x, SmoothDamptime);
        // transform.position = new Vector3(x, transform.position.y, transform.position.z);
        Vector3 targetPosition = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position,targetPosition, ref velocity, smoothtime);
    }
}