using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class Cogs : MonoBehaviour
{
    public int TotalCogs = 0;
    public UnityEvent<int> OnChangeTotalCogs;
    int _totalCogs
    {
        get { return TotalCogs; }
        set { 
            TotalCogs = value;
            OnChangeTotalCogs?.Invoke(TotalCogs);
        }
    }

    async public void Spill()
    {
        int pieceSize = 500;
        Vector3 position = transform.position;
        while (TotalCogs > 0)
        {
            int cogAmount = Random.Range(pieceSize / 2, pieceSize);
            if (cogAmount > TotalCogs)
            {
                cogAmount = Mathf.CeilToInt(TotalCogs);
            }
            TotalCogs -= cogAmount;

            GameObject cogGameObject = Instantiate(GameManager.Instance.CogPrefab);
            cogGameObject.transform.position = position + Vector3.up * 2;
            float scaleRatio = (float)cogAmount / (float)pieceSize;
            cogGameObject.transform.localScale = new Vector3(scaleRatio, scaleRatio, scaleRatio);
            cogGameObject.transform.rotation = Quaternion.Euler(
                Random.Range(0, 360),
                Random.Range(0, 360),
                Random.Range(0, 360)
                );
            Cog cog = cogGameObject.GetComponent<Cog>();
            cog.amount = cogAmount;

            await Task.Delay(50);
        }
    }

    public void Collect(int amount)
    {
        _totalCogs += amount;
    }

    public void Spend(int amount)
    {
        _totalCogs -= amount;
    }
}
