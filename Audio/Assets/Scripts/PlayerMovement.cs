using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public GameObject player;

    Vector3 playerStartPos;
    Vector3 moveLeft;
    Vector3 moveRight;
    // Start is called before the first frame update
    void Start()
    {
        playerStartPos = new Vector3();
        playerStartPos = player.transform.position;

        moveLeft = new Vector3(-5.0f, 0.0f, 0.0f);
        moveRight = new Vector3(5.0f, 0.0f, 0.0f);
    }

    // Update is called once per frame
    void Update()
    {
        //Move player to the left
        if(Input.GetKeyDown("left"))
        {
            if (player.transform.position.x > playerStartPos.x + moveLeft.x)
            {
                player.transform.position += moveLeft;
            }
        }
        //Move player to the right
        if (Input.GetKeyDown("right"))
        {
            if (player.transform.position.x < playerStartPos.x + moveRight.x)
            {
                player.transform.position += moveRight;
            }
        }
        if(Input.GetKeyDown("r"))
        {
            SceneManager.LoadScene("Play", LoadSceneMode.Single);
        }
        if(Input.GetKeyDown("b"))
        {
            SceneManager.LoadScene("Menu", LoadSceneMode.Single);
            Destroy(GameObject.Find("GlobalGameObject"));
        }
        if(Input.GetKeyDown("q"))
        {
            Application.Quit();
        }
    }
}
