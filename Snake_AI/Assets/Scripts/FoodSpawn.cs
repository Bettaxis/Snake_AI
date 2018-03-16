using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSpawn : MonoBehaviour
{

    public GameObject foodPrefab;

    public GameObject Snake;
    private SnakeLogic sl;

    //Reference to Borders
    public Transform borderTop;
    public Transform borderBottom;
    public Transform borderLeft;
    public Transform borderRight;

    private bool foodSpawned = false;

    // Use this for initialization
    void Start()
    {
        int x = (int)Random.Range(borderLeft.position.x, borderRight.position.x);
        int y = (int)Random.Range(borderTop.position.y, borderBottom.position.y);
        Instantiate(foodPrefab, new Vector2(x, y), Quaternion.identity);
        foodSpawned = true;

        InvokeRepeating("SpawnFood", .5f, .5f);
        sl = Snake.GetComponent<SnakeLogic>();
    }

    void SpawnFood()
    {
        if (foodSpawned == false)
        {
            int x = (int)Random.Range(borderLeft.position.x, borderRight.position.x);
            int y = (int)Random.Range(borderTop.position.y, borderBottom.position.y);
            Instantiate(foodPrefab, new Vector2(x, y), Quaternion.identity);
            foodSpawned = true;
        }
    }

    void Update()
    {
        if (sl.foodEaten == true)
        {
            foodSpawned = false;
        }
    }

}
