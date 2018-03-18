using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;


public enum Features
{
    //LeftDir,
    //RightDir,
    //TopDir,
    //BottomDir,
    //SnakeX,
    //SnakeY,
    FoodX,
    FoodY,
    FoodDirX,
    FoodDirY,
    DistToFood,
    //FoodDirection,
    COUNT
}

public enum Outputs
{
    NONE,
    Left,
    Up,
    Down,
    Right
}

public class SnakeLogic : MonoBehaviour
{
    List<Transform> tail = new List<Transform>();

    GameObject Food;

    Perceptron snakeyLeft = new Perceptron((int)Features.COUNT);
    Perceptron snakeyRight = new Perceptron((int)Features.COUNT);
    Perceptron snakeyUp = new Perceptron((int)Features.COUNT);
    Perceptron snakeyDown = new Perceptron((int)Features.COUNT);

    //Outputs result = Outputs.NONE;

    //float[] vision = new float[24]; //The inputs for the neural
    //float[] decision; // Output

    //int lifetime = 0;

    //long fitness = 0;

    int movesLeft = 400;

    //int growCount = 0;

    int len = 1; // Length of Snake

    float perceptronUpOutput = 0.0f;
    float perceptronDownOutput = 0.0f;
    float perceptronLeftOutput = 0.0f;
    float perceptronRightOutput = 0.0f;

    // Initialize Feature Values
    float foodX, foodY, foodDirX, foodDirY = 0;
    Vector2 FoodDirection = new Vector2(0, 0);
    float DistanceToFood = 0;

    //Set Initial Direction to Move
    Vector2 dir = Vector2.right;

    //bool alive = true;
    //bool testing = false;

    // Use this for initialization
    void Start()
    {
        InvokeRepeating("Move", .1f, .1f);
        Food = GameObject.FindGameObjectWithTag("Food");

        float[] weightVector = new float[(int)Features.COUNT];

        weightVector[(int)Features.FoodX] = 0.4f;
        weightVector[(int)Features.FoodY] = 0.4f;
        weightVector[(int)Features.FoodDirX] = 0.7f;
        weightVector[(int)Features.FoodDirY] = 0.7f;
        weightVector[(int)Features.DistToFood] = 0.8f;

        snakeyUp.bias = 0.0f;
        snakeyDown.bias = 0.0f;
        snakeyLeft.bias = 0.0f;
        snakeyRight.bias = 0.0f;
    
        snakeyUp.SetWeights(weightVector);
        snakeyLeft.SetWeights(weightVector);
        snakeyRight.SetWeights(weightVector);
        snakeyDown.SetWeights(weightVector);
        
        Perceptron temp = new Perceptron((int)Features.COUNT);
        temp.RandomizeValues();

        snakeyUp = snakeyUp.Crossover(snakeyUp, temp);
        snakeyLeft = snakeyLeft.Crossover(snakeyLeft, temp);
        snakeyRight = snakeyRight.Crossover(snakeyRight, temp);
        snakeyDown = snakeyDown.Crossover(snakeyDown, temp);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Debug.Log(movesLeft);

        //if (Input.GetKey(KeyCode.RightArrow) && dir != Vector2.left)
        //    dir = Vector2.right;
        //
        //else if (Input.GetKey(KeyCode.DownArrow) && dir != Vector2.up)
        //    dir = Vector2.down;
        //
        //else if (Input.GetKey(KeyCode.LeftArrow) && dir != Vector2.right)
        //    dir = Vector2.left; 
        //
        //else if (Input.GetKey(KeyCode.UpArrow) && dir != Vector2.down)
        //    dir = Vector2.up;


        Food = GameObject.FindGameObjectWithTag("Food");

        //Vector to Hold Direction from Snake To Food
        Vector2 FoodDirection = Food.transform.position - transform.position;
        foodDirX = FoodDirection.x;
        foodDirY = FoodDirection.y;

        foodX = Food.transform.position.x;
        foodY = Food.transform.position.y;

        // Float Value to Hold Distance from Snake Head to Food
        DistanceToFood = FoodDirection.magnitude;

        if (movesLeft == 0)
        {
            Perceptron temp = new Perceptron((int)Features.COUNT);
            temp.RandomizeValues();
            snakeyUp = snakeyUp.Crossover(snakeyUp, temp);
            snakeyLeft = snakeyLeft.Crossover(snakeyLeft, temp);
            snakeyRight = snakeyRight.Crossover(snakeyRight, temp);
            snakeyDown = snakeyDown.Crossover(snakeyDown, temp);

            tail.Clear();
            DeleteSegments();
            transform.SetPositionAndRotation(new Vector3(0, 0, 0), Quaternion.identity);

            movesLeft = 400;
        }
    }

    void Move()
    {
        movesLeft--;   

        // Saves current head position
        Vector2 v = transform.position;


        float[] featureVec = new float[(int)Features.COUNT];

        featureVec[(int)Features.FoodX] = foodX;
        featureVec[(int)Features.FoodY] = foodY;
        featureVec[(int)Features.FoodDirX] = foodDirX;
        featureVec[(int)Features.FoodDirY] = foodDirY;
        featureVec[(int)Features.DistToFood] = DistanceToFood;

        Debug.Log("Food X " + foodX);
        Debug.Log("Food Y " + foodY);

        if (foodX > transform.position.x)
        {
            snakeyRight.bias += 0.05f;

            snakeyLeft.bias -= 0.05f;
            snakeyUp.bias -= 0.05f;
            snakeyDown.bias -= 0.05f;
        }

        if (foodX < transform.position.x)
        {
            snakeyLeft.bias += 0.05f;

            snakeyRight.bias -= 0.05f;
            snakeyUp.bias -= 0.05f;
            snakeyDown.bias -= 0.05f;
        }

        if (foodY > transform.position.y)
        {
            snakeyUp.bias += 0.06f;

            snakeyLeft.bias -= 0.05f;
            snakeyRight.bias -= 0.05f;
            snakeyDown.bias -= 0.05f;
        }

        if (foodY < transform.position.y)
        {
            snakeyDown.bias += 0.06f;

            snakeyLeft.bias -= 0.05f;
            snakeyRight.bias -= 0.05f;
            snakeyUp.bias -= 0.05f;
        }

        perceptronUpOutput = snakeyUp.Evaluate(featureVec);
        perceptronDownOutput = snakeyDown.Evaluate(featureVec);
        perceptronLeftOutput = snakeyLeft.Evaluate(featureVec);
        perceptronRightOutput = snakeyRight.Evaluate(featureVec);

        Debug.Log("Output of SnakeyTron Up " + perceptronUpOutput);
        Debug.Log("Output of SnakeyTron Down " + perceptronDownOutput);
        Debug.Log("Output of SnakeyTron Left " + perceptronLeftOutput);
        Debug.Log("Output of SnakeyTron Right " + perceptronRightOutput);

        if (perceptronUpOutput > perceptronDownOutput && perceptronUpOutput > perceptronLeftOutput && perceptronUpOutput > perceptronRightOutput)
        {
            dir = Vector2.up;
        }

        if (perceptronDownOutput > perceptronUpOutput && perceptronDownOutput > perceptronLeftOutput && perceptronDownOutput > perceptronRightOutput)
        {
            dir = Vector2.down;
        }

        if (perceptronLeftOutput > perceptronRightOutput && perceptronLeftOutput > perceptronUpOutput && perceptronLeftOutput > perceptronDownOutput)
        {
            dir = Vector2.left;
        }

        if (perceptronRightOutput > perceptronLeftOutput && perceptronRightOutput > perceptronUpOutput && perceptronRightOutput > perceptronDownOutput)
        {
            dir = Vector2.right;
        }

        //else
        //{
        //    Debug.Log("OUTPUTS ARE SAME SHIT");
        //    dir = Vector2.zero;
        //}

        transform.Translate(dir);

        snakeyUp.bias = 0.0f;
        snakeyDown.bias = 0.0f;
        snakeyLeft.bias = 0.0f;
        snakeyRight.bias = 0.0f;

        if (foodEaten)
        {
            GameObject g = (GameObject)Instantiate(tailPrefab, v, Quaternion.identity);

            tail.Insert(0, g.transform);
            len++;

            foodEaten = false;

            Food = GameObject.FindGameObjectWithTag("Food");

            movesLeft = 400;
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
        else if (coll.CompareTag("Border")) //|| coll.CompareTag("Segment"))
        {
            Perceptron temp = new Perceptron((int)Features.COUNT);
            temp.RandomizeValues();

            snakeyUp = snakeyUp.Crossover(snakeyUp, temp);
            snakeyLeft = snakeyLeft.Crossover(snakeyLeft, temp);
            snakeyRight = snakeyRight.Crossover(snakeyRight, temp);
            snakeyDown = snakeyDown.Crossover(snakeyDown, temp);

            tail.Clear();
            DeleteSegments();
            transform.SetPositionAndRotation(new Vector3(0,0,0), Quaternion.identity);
        }
    }
}

internal class Perceptron
{
    public float bias = 0.0f;

    private System.Random random = new System.Random();

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
            result.weights[i] = (p1.weights[i] + p2.weights[i]) / 100.0f;
        }

        result.bias = (p1.bias + p2.bias) / 100.0f;

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
        //return (Mathf.Pow(e, val) / (Mathf.Pow(e, val) + 1));
    } 

    float RandomRange(float min, float max)
    {
        float temp = min + ((max - min) * (float)random.NextDouble() / 1.0f);
        Debug.Log("Random Range = " + temp);
        return temp;
    }

    public void RandomizeValues()
    {
        for (int i = 0; i < featureVectorSize; ++i)
        {
            weights[i] = RandomRange(-2.0f, 2.0f);
        }

        bias = RandomRange(0.0f, 1.0f);
    }
}

