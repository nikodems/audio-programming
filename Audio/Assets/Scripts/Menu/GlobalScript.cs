using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalScript : MonoBehaviour
{
    int chosenSong;
    bool multiLane;
    // Start is called before the first frame update
    void Start()
    {
        //chosenSong = 0;
        //multiLane = false;

        //Makes the game object attached to this script persist throughout scenes - allowing for data to transfer from one scene to another e.g. song choice
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Setter for chosen song
    public void SetChosenSong(int song)
    {
        chosenSong = song;
    }

    //Getter for chosen song
    public int GetChosenSong()
    {
        return chosenSong;
    }

    public void SetMultiLane(bool multi)
    {
        multiLane = multi;
    }

    public bool GetMultiLane()
    {
        return multiLane;
    }
}
