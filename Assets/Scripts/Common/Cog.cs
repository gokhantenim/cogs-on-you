using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cog : MonoBehaviour
{
    public int amount = 0;
    MoveTowards _moveTowards;

    void Awake()
    {
        _moveTowards = GetComponent<MoveTowards>();
    }

    public void SetTowards(Transform to)
    {
        _moveTowards.to = to;
        _moveTowards.enabled = true;
    }

    void OnTriggerEnter(Collider collider)
    {
        if(collider.gameObject.tag == "Magnet" && !_moveTowards.enabled)
        {
            SetTowards(collider.transform.parent);
        }
        if (collider.gameObject.tag != "Player") return;
        Destroy(gameObject);

        Cogs cogs = collider.gameObject.GetComponent<Cogs>();
        if(cogs == null) return;
        cogs.Collect(amount);
    }
}
