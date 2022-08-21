using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Snake : MonoBehaviour
{
    public int xSize, ySize;
    public GameObject block;

    GameObject head;
    GameObject food;

    public Material headMaterial, tailMaterial;

    List<GameObject> tail;

    Vector3 direction;
    bool down, up, right, left = true;

    float passedTime, timeBetweenMovement;

    bool isAlive;

    public GameObject gameOverUI;

    public Text points;
    // Start is called before the first frame update
    void Start()
    {
        timeBetweenMovement = 0.5f;
        direction = Vector3.right;
        down = true; up = true; right = true; left = false;
        CreateGrid();
        CreatePlayer();
        SpawnFood();
        isAlive = true;
    }

    // Update is called once per frame (GameLoop)
    void Update()
    {
        if(Input.GetKey(KeyCode.DownArrow) && down == true)
        {
            direction = Vector3.down;
            down = true; up = false; right = true; left = true;
        }
        else if(Input.GetKey(KeyCode.LeftArrow) && left == true)
        {
            direction = Vector3.left;
            down = true; up = true; right = false; left = true;
        }
        else if(Input.GetKey(KeyCode.RightArrow) && right == true)
        {
            direction = Vector3.right;
            down = true; up = true; right = true; left = false;
        }
        else if(Input.GetKey(KeyCode.UpArrow) && up == true)
        {
            direction = Vector3.up;
            down = false; up = true; right = true; left = true;
        }

        passedTime += Time.deltaTime;
        if(timeBetweenMovement < passedTime && isAlive)
        {
            passedTime = 0;
            //Move
            Vector3 newPosition = head.GetComponent<Transform>().position + direction;

            //Check if snake collides with border
            if(newPosition.x >= xSize/2 || newPosition.x <= -xSize/2 || newPosition.y >= ySize/2 || newPosition.y <= -ySize/2)
            {
                //GameOver!
                GameOver();
            }

            //Check if snake collides with itself
            foreach(var item in tail)
            {
                if(item.transform.position == newPosition)
                {
                    //GameOver
                    GameOver();
                }
            }

            //Check if snake collides with food
            if(newPosition.x == food.transform.position.x && newPosition.y == food.transform.position.y)
            {
                GameObject newHead = Instantiate(block);
                newHead.transform.position = food.transform.position;
                DestroyImmediate(food);
                head.GetComponent<MeshRenderer>().material = tailMaterial;
                tail.Add(head);
                head = newHead;
                head.GetComponent<MeshRenderer>().material = headMaterial;
                SpawnFood();
                points.text = "Points: " + tail.Count;
            }

            if(tail.Count == 0)
            {
                head.transform.position = newPosition;
            }
            else
            {
                head.GetComponent<MeshRenderer>().material = tailMaterial;
                tail.Add(head);
                head = tail[0];
                head.GetComponent<MeshRenderer>().material = headMaterial;
                tail.RemoveAt(0);
                head.transform.position = newPosition;
            }
        }
    }

    public void restart()
    {
        SceneManager.LoadScene(0);
    }

    private void GameOver()
    {
        isAlive = false;
        gameOverUI.SetActive(true);
    }

    private Vector3 GetRandomPosition()
    {
        return new Vector3(Random.Range((-xSize / 2) + 1, xSize / 2), Random.Range((-ySize / 2) + 1, ySize / 2), 0);
    }

    private bool ContainedInSnake(Vector3 spawnPosition)
    {
        bool isInHead = spawnPosition.x == head.transform.position.x && spawnPosition.y == head.transform.position.y;
        bool isInTail = false;
        foreach(var item in tail)
        {
            if(item.transform.position.x == spawnPosition.x && item.transform.position.y == spawnPosition.y)
            {
                isInTail = true;
            }
        }
        return isInHead || isInTail;
    }

    private void SpawnFood()
    {
        Vector3 spawnPosition = GetRandomPosition();
        while(ContainedInSnake(spawnPosition))
        {
            spawnPosition = GetRandomPosition();
        }
        food = Instantiate(block);
        food.transform.position = spawnPosition;
    }

    private void CreatePlayer()
    {
        head = Instantiate(block) as GameObject;
        head.GetComponent<MeshRenderer>().material = headMaterial;
        tail = new List<GameObject> ();
    }

    private void CreateGrid()
    {
        for (int x = 0; x <= xSize; x++)
        {
            //Here we instantiate the game object block, which will compose our wall 
            GameObject borderBottom = Instantiate(block) as GameObject;
            borderBottom.GetComponent<Transform>().position = new Vector3(x - (xSize / 2), -ySize / 2, 0);

            GameObject borderTop = Instantiate(block) as GameObject;
            borderTop.GetComponent<Transform>().position = new Vector3(x - (xSize / 2), (ySize / 2), 0);
        }

        for (int y = 0; y <= ySize; y++)
        {
            GameObject borderRight = Instantiate(block) as GameObject;
            borderRight.GetComponent<Transform>().position = new Vector3(xSize / 2 , y - (ySize / 2), 0);

            GameObject borderLeft = Instantiate(block) as GameObject;
            borderLeft.GetComponent<Transform>().position = new Vector3(-xSize/2 , y - (ySize / 2), 0);
        }
    }
}
