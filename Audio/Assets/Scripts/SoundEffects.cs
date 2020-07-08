using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffects : MonoBehaviour
{
    float[] lowPassCoefficients = new float[7];

    public AudioSource audioSource;
    AudioClip buttonSound;
    // Start is called before the first frame update
    void Start()
    {
        //Coefficients provided by Adam Sampson - from Lab 4
        lowPassCoefficients[0] = 0.0152193f;
        lowPassCoefficients[1] = 0.08115433f;
        lowPassCoefficients[2] = 0.23941581f;
        lowPassCoefficients[3] = 0.32842112f;
        lowPassCoefficients[4] = 0.23941581f;
        lowPassCoefficients[5] = 0.08115433f;
        lowPassCoefficients[6] = 0.0152193f;

        buttonSound = audioSource.clip;

        if (buttonSound)
        {
            buttonSound = LowPassFilter(buttonSound);

            buttonSound = Normalize(buttonSound);

            audioSource.clip = buttonSound;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown("f"))
        {
            audioSource.Play();
        }
    }

    //Play the sound
    public void PlayButtonSound()
    {
        audioSource.Play();
    }

    //4000hz low pass filter - removes samples above this frequency
    AudioClip LowPassFilter(AudioClip clip)
    {
        //Get samples
        float[] clipData = new float[clip.samples];
        clip.GetData(clipData, 0);

        float[] filteredData = new float[clip.samples];

        //Apply filter
        for(int i = 0; i < clip.samples; i++)
        {
            if (i >= 3 && i < clip.samples - 3)
            {
                filteredData[i] = clipData[i - 3] * lowPassCoefficients[0]
                    + clipData[i - 2] * lowPassCoefficients[1]
                    + clipData[i - 1] * lowPassCoefficients[2]
                    + clipData[i] * lowPassCoefficients[3]
                    + clipData[i + 1] * lowPassCoefficients[4]
                    + clipData[i + 2] * lowPassCoefficients[5]
                    + clipData[i + 3] * lowPassCoefficients[6];
            }
        }

        //Set new filtered samples as the data
        clip.SetData(filteredData, 0);

        return clip;
    }

    //Normalize the sound level
    AudioClip Normalize(AudioClip clip)
    {
        float[] clipData = new float[clip.samples];
        clip.GetData(clipData, 0);

        float biggestAmplitude = 0.0f;

        //Find the biggest amplitude value
        for(int i = 0; i < clip.samples; i++)
        {
            if(clipData[i] > biggestAmplitude)
            {
                biggestAmplitude = clipData[i];
            }
        }

        //Multiply all values by 1.0f / biggestAmplitude so that the biggest value reached 1.0f
        for (int i = 0; i < clip.samples; i++)
        {
            clipData[i] *= 1.0f / biggestAmplitude;
        }

        clip.SetData(clipData, 0);

        return clip;
    }
}
