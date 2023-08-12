using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTowards : MonoBehaviour
{
    public Transform to;

    // Update is called once per frame
    void Update()
    {
        if(to == null) return;
        transform.position = Vector3.MoveTowards(transform.position, to.position + new Vector3(0, 3), Time.deltaTime * 35);
    }
}
