using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Follow : MonoBehaviour
{
    public Transform followTarget;
    Transform thisTransform;

    public static Follow singleton;
    private void Start()
    {
        singleton = this;
        thisTransform = this.GetComponent<Transform>();
    }
    private void Update()
    {
        if (followTarget != null)
        {
            thisTransform.position = followTarget.transform.position;
        }
    }
    public void SetTarget(Transform sentTransform)
    {
        Debug.Log("Sent Transform");
        followTarget = sentTransform;
    }
}
