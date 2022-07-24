using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

[RequireComponent(typeof(Vision), typeof(Rigidbody2D))]
public class Ship : Agent
{
    public float speed;
    public float rotationSpeed;
    public int life = 3;
    public int maxTicks = 1000;
    public int ticks;

    Vision vision;
    Rigidbody2D rb;
    SpriteRenderer sr;
    new Collider2D collider;
    public AsteroidGenerator asteroidGenerator;
    public LootGenerator lootGenerator;

    private bool hitCoroutine = false;

    // Rewards
    public int collisionReward = -50;
    public int lootReward = 50;
    public int wallReward = -10;

    // Score info
    int episode = 0;
    int score = 0;

    public Vector2 playArea;

    private void Awake()
    {
        vision = GetComponent<Vision>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        collider = GetComponent<Collider2D>();
    }

    public override void OnEpisodeBegin()
    {
        transform.localPosition = Vector3.zero;
        StopAllCoroutines();
        hitCoroutine = false;

        asteroidGenerator.Clear();
        lootGenerator.Clear();
        
        life = 3;
        var color = sr.color;
        color.a = 1;
        sr.color = color;
        collider.enabled = true;

        episode++;
        score = 0;
        ticks = 0;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);
        // Add observations of obstacle objects sensed with Vision
        var gameObjects = vision.InSightObjects;
        var visionLength = vision.limit;
        // 0 -> Object not present 1 -> Object present
        for (int i = 0; i < visionLength; i++)
        {
            if (i < gameObjects.Count)
            {
                sensor.AddObservation(gameObjects[i].transform.position);
                sensor.AddObservation(1);
            }
            else
            {
                sensor.AddObservation(Vector3.zero);
                sensor.AddObservation(0);
            }
        }
        // Add Loot Observation
        var loot = lootGenerator.currentLoot;
        var lootObservation = loot != null ? loot.position : Vector3.zero;
        sensor.AddObservation(lootObservation);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        var xAxis = actions.ContinuousActions[0];
        var yAxis = actions.ContinuousActions[1];
        var rotate = actions.ContinuousActions[2];

        rb.velocity = speed * Time.deltaTime * new Vector2(xAxis, yAxis);
        if (rotate != 0) transform.Rotate(new Vector3(0f, 0f, rotate * rotationSpeed * Time.deltaTime));
        if (!InsidePlayArea())
        {
            transform.localPosition = Vector3.zero;
        }
        ticks++;
        if (ticks == maxTicks)
        {
            Debug.Log($"Episode {episode} Score: {score}");
            EndEpisode();
        }
    }

    public bool InsidePlayArea()
    {
        bool x = -playArea.x / 2f < transform.localPosition.x && transform.localPosition.x < playArea.x / 2f; 
        bool y = -playArea.y / 2f < transform.localPosition.y && transform.localPosition.y < playArea.y / 2f;
        return x && y;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
        continuousActions[2] = Input.GetAxisRaw("Rotate");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var other = collision.gameObject;
        if (other.CompareTag("Obstacle") && !hitCoroutine)
        {
            life--;
            AddReward(collisionReward);
            score += collisionReward;
            StartCoroutine(HitCoroutine());
            if (life == 0)
            {
                Debug.Log($"Episode {episode} Score: {score}");
                EndEpisode();
            }
        }
        else if (other.CompareTag("Loot"))
        {
            AddReward(lootReward);
            score += lootReward;
            Destroy(other);
            ticks = 0;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var other = collision.gameObject;
        if (other.CompareTag("Wall"))
        {
            AddReward(wallReward);
            score += wallReward;
        }
    }

    IEnumerator HitCoroutine()
    {
        hitCoroutine = true;
        collider.enabled = false;
        var srColor = sr.color;
        srColor.a = 0.2f;
        sr.color = srColor;
        yield return new WaitForSeconds(1.5f);
        collider.enabled = true;
        srColor.a = 1f;
        sr.color = srColor;
        hitCoroutine = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, playArea);
        if (lootGenerator != null) { 
            var loot = lootGenerator.currentLoot;
            if (loot != null)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawLine(transform.position, loot.position);
            }
        }
    }

}
