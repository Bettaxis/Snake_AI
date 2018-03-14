using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SnakeLogic : MonoBehaviour
{
    List<Transform> tail = new List<Transform>();

    //Set Initial Direction to Move
    Vector2 dir = Vector2.right;

    // Use this for initialization
    void Start()
    {
        InvokeRepeating("Move", 0.1f, 0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.RightArrow) && dir != Vector2.left)
            dir = Vector2.right;

        else if (Input.GetKey(KeyCode.DownArrow) && dir != Vector2.up)
            dir = Vector2.down;

        else if (Input.GetKey(KeyCode.LeftArrow) && dir != Vector2.right)
            dir = Vector2.left; 

        else if (Input.GetKey(KeyCode.UpArrow) && dir != Vector2.down)
            dir = Vector2.up;
    }

    void Move()
    {
        // Saves current head position
        Vector2 v = transform.position;

        transform.Translate(dir);

        if (foodEaten)
        {
            GameObject g = (GameObject)Instantiate(tailPrefab, v, Quaternion.identity);

            tail.Insert(0, g.transform);

            foodEaten = false;
        }


        else if (tail.Count > 0)
        {
            // Sets Last segment to Current Head Position
            tail.Last().position = v;

            // Add Front Segment to Last and remove the last one.
            tail.Insert(0, tail.Last());
            tail.RemoveAt(tail.Count - 1);
        }
    }

    void DeleteSegments()
    {
        GameObject[] segments = GameObject.FindGameObjectsWithTag("Segment");

        foreach (GameObject segment in segments)
            GameObject.Destroy(segment);
    }

    public bool foodEaten = false;
    public GameObject tailPrefab;

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.name.StartsWith("Food"))
        {
            // Get longer in next Move call
            foodEaten = true;

            // Remove the Food
            Destroy(coll.gameObject);
        }

        // Collided with Tail or Border
        else if (coll.CompareTag("Border"))
        {
            tail.Clear();
            DeleteSegments();
            transform.SetPositionAndRotation(new Vector3(0,0,0), Quaternion.identity);
        }

        else if (coll.CompareTag("Segment"))
        {
            tail.Clear();
            DeleteSegments();
            transform.SetPositionAndRotation(new Vector3(0, 0, 0), Quaternion.identity);
        }
    }
}
