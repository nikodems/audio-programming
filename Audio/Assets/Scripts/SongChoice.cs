using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongChoice : MonoBehaviour
{
    public AudioClip[] songList = new AudioClip[4];

    public GlobalScript globalScript;

    int songChoice;
    // Start is called before the first frame update
    void Awake()
    {
        globalScript = GameObject.Find("GlobalGameObject").GetComponent<GlobalScript>();
        if(globalScript != null)
        {
            songChoice = globalScript.GetChosenSong();
        }
        else
        {
            songChoice = 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public AudioClip GetChosenSong()
    {
        return songList[songChoice];
    }

    public bool GetMultiLane()
    {
        bool a = globalScript.GetMultiLane();
        return a;
    }
}
