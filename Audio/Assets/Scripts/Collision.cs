using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Collision : MonoBehaviour
{
    int score;
    bool colliding;

    public Text scoreText;
    // Start is called before the first frame update
    void Start()
    {
        score = 0;

        colliding = false;
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = score.ToString();
    }

    //Upon collision being registered
    private void OnTriggerEnter(Collider other)
    {
        if (!colliding)
        {
            //Increase score
            score += 1;

            //Set bool to true so only one collision a frame
            colliding = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(colliding)
        {
            colliding = false;

            //Destroy the other object
            Destroy(other.gameObject);
        }
    }
}
