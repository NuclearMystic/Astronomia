using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownCameraController : MonoBehaviour
{
    private Transform followTarget;

    private void Start()
    {
        followTarget = GameObject.FindWithTag("Player").transform;
    }

    private void Update()
    {
        if(transform.position != followTarget.position)
        {
            Vector3 camFollowVector = new Vector3(followTarget.position.x, followTarget.position.y, -10f);
            transform.position = camFollowVector;
        }
    }
}
