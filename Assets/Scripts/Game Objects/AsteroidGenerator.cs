using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidGenerator : MonoBehaviour
{
    public Transform target;
    public float targetRadius;

    public float minRadius;
    public float maxRadius;

    public float minSpeed;
    public float maxSpeed;

    public float minWaitTime;
    public float maxWaitTime;

    public GameObject asteroidPrefab;
    public Transform container;


    // Start is called before the first frame update
    void Start()
    {
        transform.localPosition = Vector3.zero;
        StartCoroutine(GenererateAsteroid());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(target.position, targetRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(target.position, minRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(target.position, maxRadius);

    }

    IEnumerator GenererateAsteroid()
    {
        while (true)
        {
            Vector2 displacement = Random.insideUnitCircle.normalized * Random.Range(minRadius, maxRadius);
            Vector2 position = (Vector2)target.position + displacement;
            Vector2 targetPos = (Vector2)transform.position + Random.insideUnitCircle.normalized * Random.Range(0f, targetRadius);
            Vector2 direction = (targetPos - position).normalized;
            Vector2 velocity = direction * Random.Range(minSpeed, maxSpeed);

            GameObject asteroid = Instantiate(asteroidPrefab, position, Quaternion.identity, container);
            asteroid.GetComponent<Rigidbody2D>().velocity = velocity;

            yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));
        }
    }

    public void Clear()
    {
        var asteroids = container.GetComponentsInChildren<Asteroid>();
        foreach(var asteroid in asteroids)
        {
            Destroy(asteroid.gameObject);
        }
    }


}
