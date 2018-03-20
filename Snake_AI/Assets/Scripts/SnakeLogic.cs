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
    //List<Perceptron> InputLayer = new List<Perceptron>();
    //List<Perceptron> HiddenLayer = new List<Perceptron>();
    //List<Perceptron> OutputLayer = new List<Perceptron>();

    GameObject Food;

    public GameObject FoodSpawnScriptObject;
    private FoodSpawn fs;

    //Perceptron snakeyLeft = new Perceptron((int)Features.COUNT);
    //Perceptron snakeyRight = new Perceptron((int)Features.COUNT);
    //Perceptron snakeyUp = new Perceptron((int)Features.COUNT);
    //Perceptron snakeyDown = new Perceptron((int)Features.COUNT);

    NeuralNet SnakeNN;

    Perceptron[] InputLayerArray =  new Perceptron[5];
    Perceptron[] HiddenLayerArray = new Perceptron[8];
    Perceptron[] OutputLayerArray = new Perceptron[4];

    Perceptron snakeyIn1 = new Perceptron();
    Perceptron snakeyIn2 = new Perceptron();
    Perceptron snakeyIn3 = new Perceptron();
    Perceptron snakeyIn4 = new Perceptron();
    Perceptron snakeyIn5 = new Perceptron();

    Perceptron Hidden1 = new Perceptron();
    Perceptron Hidden2 = new Perceptron();
    Perceptron Hidden3 = new Perceptron();
    Perceptron Hidden4 = new Perceptron();
    Perceptron Hidden5 = new Perceptron();
    Perceptron Hidden6 = new Perceptron();
    Perceptron Hidden7 = new Perceptron();
    Perceptron Hidden8 = new Perceptron();
                  
    Perceptron Output1 = new Perceptron();
    Perceptron Output2 = new Perceptron();
    Perceptron Output3 = new Perceptron();
    Perceptron Output4 = new Perceptron();

    int movesLeft = 400;

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
        Food = GameObject.FindGameObjectWithTag("Food");
        fs = FoodSpawnScriptObject.GetComponent<FoodSpawn>();

        List<float> weightVector = new List<float>(5);

        weightVector.Insert((int)Features.FoodX, 0.4f);
        weightVector.Insert((int)Features.FoodY, 0.4f);
        weightVector.Insert((int)Features.FoodDirX, 0.7f);
        weightVector.Insert((int)Features.FoodDirY, 0.7f);
        weightVector.Insert((int)Features.DistToFood, 0.8f);
        
        snakeyIn1.SetInputWeights(weightVector[0]);
        snakeyIn2.SetInputWeights(weightVector[1]);
        snakeyIn3.SetInputWeights(weightVector[2]);
        snakeyIn4.SetInputWeights(weightVector[3]);
        snakeyIn5.SetInputWeights(weightVector[4]);
        
        InputLayerArray[0] = snakeyIn1;
        InputLayerArray[1] = snakeyIn2;
        InputLayerArray[2] = snakeyIn3;
        InputLayerArray[3] = snakeyIn4;
        InputLayerArray[4] = snakeyIn5;

        List<float> HiddenWeightVector = new List<float>(5);

        HiddenWeightVector.Insert(0, 0.5f);
        HiddenWeightVector.Insert(1, 0.4f);
        HiddenWeightVector.Insert(2, 0.3f);
        HiddenWeightVector.Insert(3, 0.6f);
        HiddenWeightVector.Insert(4, 0.4f);

        Hidden1.SetWeights(HiddenWeightVector);
        Hidden2.SetWeights(HiddenWeightVector);
        Hidden3.SetWeights(HiddenWeightVector);
        Hidden4.SetWeights(HiddenWeightVector);
        Hidden5.SetWeights(HiddenWeightVector);
        Hidden6.SetWeights(HiddenWeightVector);
        Hidden7.SetWeights(HiddenWeightVector);
        Hidden8.SetWeights(HiddenWeightVector);

        HiddenLayerArray[0] = Hidden1;
        HiddenLayerArray[1] = Hidden2;
        HiddenLayerArray[2] = Hidden3;
        HiddenLayerArray[3] = Hidden4;
        HiddenLayerArray[4] = Hidden5;
        HiddenLayerArray[5] = Hidden6;
        HiddenLayerArray[6] = Hidden7;
        HiddenLayerArray[7] = Hidden8;

        List<float> OutputWeightVector = new List<float>(8);

        OutputWeightVector.Insert(0, 0.3f);
        OutputWeightVector.Insert(1, 0.2f);
        OutputWeightVector.Insert(2, 0.1f);
        OutputWeightVector.Insert(3, 0.6f);
        OutputWeightVector.Insert(4, 0.3f);
        OutputWeightVector.Insert(5, 0.25f);
        OutputWeightVector.Insert(6, 0.6f);
        OutputWeightVector.Insert(7, 0.8f);

        Output1.SetWeights(OutputWeightVector);
        Output2.SetWeights(OutputWeightVector);
        Output3.SetWeights(OutputWeightVector);
        Output4.SetWeights(OutputWeightVector);

        OutputLayerArray[0] = Output1;
        OutputLayerArray[1] = Output2;
        OutputLayerArray[2] = Output3;
        OutputLayerArray[3] = Output4;

        //Perceptron temp = new Perceptron((int)Features.COUNT);

        //temp.SetWeights(weightVector);

        //InputLayerArray[0] = temp;

        for (int i = 0; i < InputLayerArray.Length; i++)
        {
            InputLayerArray[i].RandomizeValues();
        }

        for (int i = 0; i < HiddenLayerArray.Length; i++)
        {
            HiddenLayerArray[i].RandomizeValues();
        }

        for (int i = 0; i < OutputLayerArray.Length; i++)
        {
            OutputLayerArray[i].RandomizeValues();
        }

        NeuralNet SnakeNNTemp = new NeuralNet(InputLayerArray, HiddenLayerArray, OutputLayerArray);

        SnakeNN = SnakeNNTemp;  

        SnakeNN.Randomize();

        //temp.RandomizeValues();

        //snakeyUp = snakeyUp.Crossover(snakeyUp, temp);
        //snakeyLeft = snakeyLeft.Crossover(snakeyLeft, temp);
        //snakeyRight = snakeyRight.Crossover(snakeyRight, temp);
        //snakeyDown = snakeyDown.Crossover(snakeyDown, temp);
        
        InvokeRepeating("Move", .1f, .1f);
    }

    // Update is called once per frame
    void Update()
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

        /*
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
        */
    }

    void Move()
    {
        movesLeft--;   

        // Saves current head position
        Vector2 v = transform.position;


        float[] featureVec = new float[(int)Features.COUNT];

        featureVec[(int)Features.FoodX] = foodX / 10f;
        featureVec[(int)Features.FoodY] = foodY / 10f;
        featureVec[(int)Features.FoodDirX] = foodDirX / 10f;
        featureVec[(int)Features.FoodDirY] = foodDirY / 10f;
        featureVec[(int)Features.DistToFood] = DistanceToFood /10f;

        //snakeyIn1
        //Debug.Log("Food X " + foodX);
        //Debug.Log("Food Y " + foodY);

        SnakeNN.EvaluateNN(featureVec);

        /*
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
        */


        perceptronUpOutput = SnakeNN.OutputFloats[0];
        perceptronDownOutput = SnakeNN.OutputFloats[1];
        perceptronLeftOutput = SnakeNN.OutputFloats[2];
        perceptronRightOutput = SnakeNN.OutputFloats[3];

        // Debug.Log("Output of NeuralNet Up " + perceptronUpOutput);
        // Debug.Log("Output of NeuralNet Down " + perceptronDownOutput);
        // Debug.Log("Output of NeuralNet Left " + perceptronLeftOutput);
        // Debug.Log("Output of NeuralNet Right " + perceptronRightOutput);

          if (perceptronUpOutput > perceptronDownOutput && perceptronUpOutput > perceptronLeftOutput && perceptronUpOutput > perceptronRightOutput)
          {
              dir = Vector2.up;
          }
          
          else if (perceptronDownOutput > perceptronUpOutput && perceptronDownOutput > perceptronLeftOutput && perceptronDownOutput > perceptronRightOutput)
          {
              dir = Vector2.down;
          }

        else if (perceptronLeftOutput > perceptronRightOutput && perceptronLeftOutput > perceptronUpOutput && perceptronLeftOutput > perceptronDownOutput)
          {
              dir = Vector2.left;
          }

        else if (perceptronRightOutput > perceptronLeftOutput && perceptronRightOutput > perceptronUpOutput && perceptronRightOutput > perceptronDownOutput)
          {
              dir = Vector2.right;
          }
        
        //else
        //{
        //    //Debug.Log("OUTPUTS ARE SAME SHIT");
        //    dir = Vector2.up;
        //}
        
        transform.Translate(dir);

        //snakeyUp.bias = 0.0f;
        //snakeyDown.bias = 0.0f;
        //snakeyLeft.bias = 0.0f;
        //snakeyRight.bias = 0.0f;

        if (foodEaten)
        {
            GameObject g = (GameObject)Instantiate(tailPrefab, v, Quaternion.identity);

            tail.Insert(0, g.transform);
            len++;

            foodEaten = false;

            fs.foodSpawned--;

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

            fs.SpawnFood();

            Food = GameObject.FindGameObjectWithTag("Food");
        }

        // Collided with Tail or Border
        else if (coll.CompareTag("Border")) //|| coll.CompareTag("Segment"))
        {
            //Perceptron temp = new Perceptron((int)Features.COUNT);
            //temp.RandomizeValues();

            //snakeyUp = snakeyUp.Crossover(snakeyUp, temp);
            //snakeyLeft = snakeyLeft.Crossover(snakeyLeft, temp);
            //snakeyRight = snakeyRight.Crossover(snakeyRight, temp);
            //snakeyDown = snakeyDown.Crossover(snakeyDown, temp);

            tail.Clear();
            DeleteSegments();
            transform.SetPositionAndRotation(new Vector3(0,0,0), Quaternion.identity);

            SnakeNN.Randomize();
        }
    }
}

internal class Perceptron
{
    public float bias = 0.0f;

    private System.Random random = new System.Random();

    public List<float> weights = new List<float>();

    public List<float> featureVector = new List<float>();

    public List<Perceptron> outputs = new List<Perceptron>();

    public Perceptron()
    { 
    }

    public Perceptron(Perceptron other)
    {
        bias = other.bias;
        weights = other.weights;
    }

    public void setFeatureVector(List<float> a_featureVector)
    {
        featureVector = a_featureVector;
    }

    public Perceptron Crossover(Perceptron p1, Perceptron p2)
    {
        Perceptron result = new Perceptron(p1);

        for (int i = 0; i < result.featureVector.Count; i++)
        {
            result.weights[i] = (p1.weights[i] + p2.weights[i]) / 2.0f;
        }

        result.bias = (p1.bias + p2.bias) / 2.0f;

        return result;
    }
    
    // Used when creating a Neural Net
    public float Evaluate()
    {
        float result = 0.0f;

        for (int i = 0; i < featureVector.Count; i++)
        {
            result += featureVector[i] * weights[i];
        }

        result += bias;

        float sigmResult = SigmoidFunction(result);

        foreach(Perceptron p in outputs)
        {
            p.featureVector.Add(sigmResult);
        }

        return sigmResult;
    }

    public float Evaluate(float[] inputVector)
    {
        float result = 0.0f;

        for (int i = 0; i < inputVector.Length; i++)
        {
            result += inputVector[i] * weights[i];
        }


        result += bias;

        float sigmResult = SigmoidFunction(result);

        //foreach (Perceptron p in outputs)
        //{
        //    p.featureVector.Add(sigmResult);
        //}

        return sigmResult;
    }

    // Used when Using Single Perceptron
    //  public float Evaluate(float[] featureVectorTemp)
    //  {
    //      float result = 0.0f;
    //  
    //      for (int i = 0; i < featureVector.Count; ++i)
    //      {
    //          result += featureVectorTemp[i] * weights[i];
    //      }
    //  
    //      result += bias;
    //  
    //      return SigmoidFunction(result);
    //  }

    //Sets Weights to float[]  
    public void SetWeights(List<float> _weights)
    {
        weights = new List<float>(_weights);

        //for (int i = 0; i < _weights.Count; ++i)
        //{
        //    weights[i] = _weights[i];
        //}
    }

    //Sets Weights to float
    public void SetInputWeights(float _weights)
    {
        weights.Add(_weights);
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
        //Debug.Log("Random Range = " + temp);
        return temp;
    }

    public void RandomizeValues()
    {
        for (int i = 0; i < weights.Count; ++i)
        {
            weights[i] = UnityEngine.Random.Range(-2.0f, 2.0f);
        }

        bias = RandomRange(0.0f, 1.0f);
    }
}

internal class NeuralNet
{
    //Float[] of Hidden Layer Feature Vectors
    //private unsafe float[] HiddenFVToSet = new float[4];

    //Float[] of Output Layer Feature Vectors
    //private unsafe float[] OutputFVToSet = new float[8];

    //Float[] of Outputs
    public float[] OutputFloats = new float[4];

    //Perceptron Arrays to hold the new Neural Net Cross
    public List<Perceptron> Inputs = new List<Perceptron>();
    public List<Perceptron> Hiddens = new List<Perceptron>();
    public List<Perceptron> Outputs = new List<Perceptron>();

    public Perceptron[] OutputLayer;

    //Creates a Neural Net and Sets the Final Output Layer to OutputLayer
    public NeuralNet(Perceptron[] Input, Perceptron[] Hidden, Perceptron[] Output)
    {
        Inputs = new List<Perceptron>(Input);
        Hiddens = new List<Perceptron>(Hidden);
        Outputs = new List<Perceptron>(Output);

        foreach(Perceptron p in Inputs)
        {
            foreach(Perceptron h in Hiddens)
            {
                p.outputs.Add(h);
            }
        }

        foreach (Perceptron h in Hiddens)
        {
            foreach (Perceptron o in Outputs)
            {
                h.outputs.Add(o);
            }
        }
    }

    public void EvaluateNN(float[] _InputFloats)
    {
        List<float> InputResults = new List<float>();

        foreach(Perceptron p in Hiddens)
            p.featureVector.Clear();
        foreach (Perceptron p in Outputs)
            p.featureVector.Clear();

        //Obtains Hidden Layer Inputs from First Layer Output
        for (int i = 0; i < Inputs.Count; i++)
        {
            Inputs[i].featureVector.Clear();
            Inputs[i].featureVector.Add(_InputFloats[i]);
            //InputResults.Add(Inputs[i].Evaluate());
            Inputs[i].Evaluate();
        }

        //for (int i = 0; i < InputResults.Count; i++)
        //    Debug.Log(InputResults[i]);

        //List<float> HiddenResults = new List<float>();

        foreach (Perceptron h in Hiddens)
        {
            //h.featureVector = InputResults.ToList<float>();
            //Debug.Log(h.featureVector[4]);
            h.Evaluate();

            //  for (int i = 0; i < h.weights.Count; i++)
            //  {
            //      Debug.Log(h.weights[i]);
            //  }
            //h.RandomizeValues();
            //float r = h.Evaluate(InputResults.ToArray());
            //HiddenResults.Add(r);
        }            

        OutputFloats = new float[Outputs.Count];
        string debugStr = "OUTPUTS: ";
        for (int i = 0; i < Outputs.Count; i++)
        {
            OutputFloats[i] = Outputs[i].Evaluate();
            debugStr += Outputs[i].Evaluate() + ",";
        }
        Debug.Log(debugStr);
    }

    //Creates Perceptron Layers with provided Feature Vector Sizes
    public void CreateLayers(int fv1, int fv2, int fv3)
    {
        //for (int i = 0; i < 5; i++)
        //{
        //    Inputs[i] = new Perceptron(fv1);
        //}
        //
        //for (int i = 0; i < 8; i++)
        //{
        //    Hiddens[i] = new Perceptron(fv2);
        //}
        //
        //for (int i = 0; i < 4; i++)
        //{
        //    Outputs[i] = new Perceptron(fv3);
        //}
    }
    /*
    public NeuralNet Crossover(NeuralNet n1, NeuralNet n2)
    {
        NeuralNet result = new NeuralNet(Inputs, Hidden, Outputs);

        for (int i = 0; i < n1.HiddenFVToSet.Length; i++)
        {
            float[] temp = (n1.Hidden[i].weights[i] + n2.Hidden[i].weights[i]) / 100.0f;
            result.Hidden[i].SetWeights(temp); 
        }

        for (int i = 0; i < n1.OutputWeightsToSet.Length; i++)
        {
            result.OutputWeightsToSet[i] = (n1.OutputWeightsToSet[i] + n2.OutputWeightsToSet[i]) / 100.0f;
        }

        //Figure out if the Perceptron Bias has to cross over in here or not.
        //result[i].bias = (p1.bias + p2.bias) / 100.0f;
            
        return result;
    }
    */


    public void Randomize()
    {  
        for (int i = 0; i < Inputs.Count; ++i)
        {
            Inputs[i].RandomizeValues();
        }

        for (int i = 0; i < Hiddens.Count; ++i)
        {
            Hiddens[i].RandomizeValues();
        }

        for (int i = 0; i < Outputs.Count; ++i)
        {
            Outputs[i].RandomizeValues();
        }
    }
}
