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

    public bool foodSpawned = false;

    // Use this for initialization
    void Start()
    {
        int x = (int)Random.Range(borderLeft.position.x + 2, borderRight.position.x - 2);
        int y = (int)Random.Range(borderTop.position.y - 2, borderBottom.position.y + 2);
        Instantiate(foodPrefab, new Vector2(x,y), Quaternion.identity);
        foodSpawned = true;

        InvokeRepeating("SpawnFood", .5f, .5f);
        sl = Snake.GetComponent<SnakeLogic>();
    }

    public void SpawnFood()
    {
        if (!foodSpawned)
        {
            int x = (int)Random.Range(borderLeft.position.x + 2, borderRight.position.x - 2);
            int y = (int)Random.Range(borderTop.position.y - 2, borderBottom.position.y + 2);

            Instantiate(foodPrefab, new Vector2(x, y), Quaternion.identity);
            foodSpawned = true;
        }
    }

    void Update()
    {
    }

}
