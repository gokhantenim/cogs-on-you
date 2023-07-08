using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cog : MonoBehaviour
{
    public int amount = 0;

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag != "Player") return;
        Destroy(gameObject);

        Cogs cogs = collider.gameObject.GetComponent<Cogs>();
        if(cogs == null) return;
        cogs.Collect(amount);
    }
}
