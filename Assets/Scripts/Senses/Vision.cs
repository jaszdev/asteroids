using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Vision : MonoBehaviour
{
    [Range(0f, 360f)]
    public float angle;
    
    [Range(0f, 30f)]
    public float radius;

    public string[] targets;

    public List<Transform> InSightObjects
    {
        get
        {
            var all = Utils.ColliderListToListTransform(Physics2D.OverlapCircleAll(transform.position, radius).ToList());
            var inSight = all.Where(t => InSight(t)).ToList();
            if (limit > 0 && inSight.Count > limit) inSight = inSight.GetRange(0, limit);
            return inSight;
        }
    }

    [Range(0, 20)]
    public int limit = -1;

    private float initAngle = 0f;
    private float endAngle = 0f;
    private Vector3 initVector = new();
    private Vector3 endVector = new();

    private bool InSight(Transform _transform)
    {
        if (_transform.CompareTag("Wall") || _transform.CompareTag("Loot")) return false;

        var dir = _transform.position - transform.position;
        var ang = Mathf.Atan2(dir.y, dir.x);
        return initAngle <= ang && ang <= endAngle;
    }

    private 


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAngles();
    }

    void UpdateAngles()
    {
        var center = transform.up;
        var centerAngle = Mathf.Atan2(center.y, center.x); // in radians
        var angleRadDiv2 = angle * Mathf.Deg2Rad / 2;
        initAngle = centerAngle - angleRadDiv2;
        endAngle = centerAngle + angleRadDiv2;
        initVector = new Vector3(Mathf.Cos(initAngle), Mathf.Sin(initAngle)) * radius;
        endVector = new Vector3(Mathf.Cos(endAngle), Mathf.Sin(endAngle)) * radius;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radius);

        var inSightObjects = InSightObjects;
        if (inSightObjects.Count > 0) Gizmos.color = Color.magenta;
        UpdateAngles();
        Gizmos.DrawLine(transform.position, transform.position + initVector);
        Gizmos.DrawLine(transform.position, transform.position + endVector);

        Gizmos.color = Color.magenta;
        foreach (var obj in InSightObjects)
        {
            Gizmos.DrawLine(transform.position, obj.transform.position);
        }
    }

}
