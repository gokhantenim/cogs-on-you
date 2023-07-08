using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Scanner : MonoBehaviour
{
    public UnityEvent<List<GameObject>> OnScan;
    [SerializeField] LayerMask _scanLayers;
    [SerializeField] LayerMask _ignoreLayers;
    [SerializeField] string _lookingTag = "";
    [SerializeField] bool _drawGizmos = false;
    public int ScanPerSecond = 6;
    public float ScanDistance = 75;
    public float ScanAngle = 90;

    float _halfScanAngle => ScanAngle / 2;
    float _scanInterval => 1 / ScanPerSecond;
    float _scanTimer = 0;

    Collider[] _seenColliders = new Collider[100];
    List<GameObject> _seenEnemies = new List<GameObject>();
    int _seenCount = 0;
    

    private void OnDrawGizmos()
    {
        if (!_drawGizmos) return;
        Gizmos.color = _seenEnemies.Count > 0 ? Color.magenta : Color.green;
        Gizmos.DrawWireSphere(transform.position, ScanDistance);
        Gizmos.DrawLine(transform.position, transform.position + (Quaternion.Euler(0, _halfScanAngle, 0) * transform.forward * ScanDistance));
        Gizmos.DrawLine(transform.position, transform.position + (Quaternion.Euler(0, -_halfScanAngle, 0) * transform.forward * ScanDistance));
        Gizmos.color = Color.white;
        foreach (GameObject enemy in _seenEnemies)
        {
            Gizmos.DrawLine(transform.position + new Vector3(0, 4f, 0), enemy.transform.position);
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        _scanTimer -= Time.deltaTime;
        if (_scanTimer < 0)
        {
            _scanTimer += _scanInterval;
            Scan();
        }
    }

    public void Scan()
    {
        _seenCount = Physics.OverlapSphereNonAlloc(transform.position, ScanDistance, _seenColliders, _scanLayers);
        _seenEnemies.Clear();
        for (int i = 0; i < _seenCount; i++)
        {
            Collider seenCollider = _seenColliders[i];
            if (seenCollider.tag == _lookingTag && InSight(seenCollider.gameObject))
            {
                _seenEnemies.Add(seenCollider.gameObject);
            }
        }
        OnScan?.Invoke(_seenEnemies);
    }

    bool InSight(GameObject seenObject)
    {
        Vector3 direction = seenObject.transform.position - transform.position;
        float angle = Vector3.Angle(direction, transform.forward);
        if (angle > _halfScanAngle) return false;

        RaycastHit hit;
        if (Physics.Linecast(transform.position + new Vector3(0, 4f, 0), seenObject.transform.position, out hit, ~_ignoreLayers)
            && hit.collider.gameObject.Equals(seenObject))
        {
            return true;
        }
        return false;
    }
}
