using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    public Dropdown songSelect;

    public UnityEngine.UI.Toggle laneToggle;

    public GlobalScript globalScript;

    int chosenSong;
    bool multiLane;
    // Start is called before the first frame update
    void Start()
    {
        //Adds a listener to the Dropdown UI object
        //Listener checks for if the value of the object has changed, in that case it will call DropdownValueChanged()
        songSelect.onValueChanged.AddListener(delegate
        {
            DropdownValueChanged(songSelect);
        });

        laneToggle.onValueChanged.AddListener(delegate
        {
            ToggleValueChanged(laneToggle);
        });

        chosenSong = 0;
        multiLane = true;

        globalScript.SetChosenSong(chosenSong);
        globalScript.SetMultiLane(multiLane);
    }

    // Update is called once per frame
    void Update()
    {
        globalScript.SetChosenSong(chosenSong);
        globalScript.SetMultiLane(multiLane);
    }

    //Load play scene
    public void GoToPlay()
    {
        SceneManager.LoadScene("Play", LoadSceneMode.Single);
    }

    //Set chosen song
    void DropdownValueChanged(Dropdown change)
    {
        globalScript.SetChosenSong(change.value);
        chosenSong = change.value;
    }

   void ToggleValueChanged(Toggle change)
    {
        globalScript.SetMultiLane(change.isOn);
        multiLane = change.isOn;
    }
}
