using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class SnakeLogic : MonoBehaviour
{
    List<Transform> tail = new List<Transform>();

    GameObject Food;

    //private NeuralNet Brain;

    float[] vision = new float[24]; //The inputs for the neural
    float[] decision; // Output

    int lifetime = 0;

    long fitness = 0;

    int leftToLive = 200;

    int growCount = 0;

    int len = 1; // Length of Snake

    //Set Initial Direction to Move
    Vector2 dir = Vector2.right;

    bool alive = true;
    bool testing = false;

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
            len++;

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

internal class Perceptron
{
    public float bias = 0.0f;

    private System.Random random;

    private unsafe float[] weights;

    int featureVectorSize;

    public Perceptron(int _featureVectorSize)
    {
        featureVectorSize = _featureVectorSize;

        weights = new float[featureVectorSize];

        for (int i = 0; i < featureVectorSize; ++i)
        {
            weights[i] = 0.0f;
        }
    }

    public Perceptron(Perceptron other)
    {
        featureVectorSize = other.featureVectorSize;
        bias = other.bias;
        weights = new float[featureVectorSize];

        // Copy the values from the other Perceptron.
        for (int i = 0; i < featureVectorSize; ++i)
        {
            weights[i] = other.weights[i];
        }
    }

     ~Perceptron()
    {
        weights.Initialize();
    }

    public Perceptron Crossover(Perceptron p1, Perceptron p2)
    {
        Perceptron result = new Perceptron(p1.featureVectorSize);

        for(int i = 0; i < result.featureVectorSize; i++)
        {
            result.weights[i] = (p1.weights[i] + p2.weights[i]) / 2.0f;
        }

        result.bias = (p1.bias + p2.bias) / 2.0f;

        return result;
    }

    public float Evaluate(float[] featureVector)
    {
        float result = 0.0f;

        for (int i = 0; i < featureVectorSize; ++i)
        {
            result += featureVector[i] * weights[i];
        }

        result += bias;

        return SigmoidFunction(result);
    }

    public void SetWeights(float[] _weights)
    {
        for (int i = 0; i < featureVectorSize; ++i)
        {
            weights[i] = _weights[i];
        }
    }

    float SigmoidFunction(float val)
    {
        const float e = 2.71828182845904523536f;

        return 1.0f / (1.0f + Mathf.Pow(e, -val));
    }

    float RandomRange(float min, float max)
    {
        return min + ((max - min) * (float)random.NextDouble() / 1.0f);
    }

    void RandomizeValues()
    {
        for (int i = 0; i < featureVectorSize; ++i)
        {
            weights[i] = RandomRange(-2.0f, 2.0f); // This range of [-2, 2] is arbitrary.
        }

        bias = RandomRange(-2.0f, 2.0f);
    }
}

