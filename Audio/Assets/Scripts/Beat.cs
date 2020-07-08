using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beat : MonoBehaviour
{
    public GameObject beat;

    float velocity;
    float timeAlive;
    // Start is called before the first frame update
    void Start()
    {
        velocity = -20.0f * Time.deltaTime;

        timeAlive = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        beat.transform.position += new Vector3(0.0f, 0.0f, velocity);

        //1s / 60 frames
        timeAlive += Time.deltaTime;

        //Destroy this object after its been alive for some time
        if(timeAlive > 3.0f)
        {
            Destroy(beat);
        }
    }
}
