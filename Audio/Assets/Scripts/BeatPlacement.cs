using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

struct FrequencyBand
{
    public int minFreq;
    public int maxFreq;
}


public class BeatPlacement : MonoBehaviour
{
    public AudioSource mutedAudioSource;
    public AudioSource audioSource;

    public SongChoice songChoiceScript;

    System.Random rand = new System.Random();

    public GameObject beatPrefab;
    GameObject beatGameObject;
    Vector3 beatPos;
    Vector3 leftPos;
    Vector3 rightPos;

    //How long it takes a beat to travel down the screen
    //float beatTravelTime = 1.66f;
    float beatTravelTime = 1.58f;
    float beatDelay;
    float startDelay = 3.0f;

    //How many samples to use for spectrum data
    //Unity will use x2 because the second half mirrors the first
    const int sampleNum = 2048;
    float[] currentSpectrum;
    float[] previousSpectrum;
    const float thresholdMultiplier = 5.0f;
    //How many neighbours are used when calculating energy of potential beat
    const int calcWindowSize = 30;

    //Frequency value of a single band
    float freqBand = (44100 / 2) / sampleNum;

    float[] spectrum = new float[sampleNum];

    int indexToProcess = calcWindowSize / 2;

    List<BeatInfo> spectrumSamples;

    List<float> beatTimes;

    bool ready;

    bool beatBool;

    //Spawn beats in multiple lanes or no
    bool multiLane;

    //Which lane to spawn beat
    int lane;

    //A number of frequency bands for use in detecting a beat in a particular frequency band only
    FrequencyBand subBass = new FrequencyBand();
    FrequencyBand bass = new FrequencyBand();
    FrequencyBand lowMid = new FrequencyBand();
    FrequencyBand midRange = new FrequencyBand();
    FrequencyBand uppRange = new FrequencyBand();
    FrequencyBand presence = new FrequencyBand();
    FrequencyBand brilliance = new FrequencyBand();
    FrequencyBand normal = new FrequencyBand();

    // Start is called before the first frame update
    void Start()
    {
        if (songChoiceScript)
        {
            audioSource.clip = songChoiceScript.GetChosenSong();
            mutedAudioSource.clip = songChoiceScript.GetChosenSong();
        }

        mutedAudioSource.Play();

        //Spawning position of a beat
        beatPos = new Vector3(0.0f, -3.0f, 49.0f);
        leftPos = new Vector3(-5.0f, 0.0f, 0.0f);
        rightPos = new Vector3(5.0f, 0.0f, 0.0f);

        //-------------------
        currentSpectrum = new float[sampleNum];
        previousSpectrum = new float[sampleNum];

        spectrumSamples = new List<BeatInfo>();

        beatTimes = new List<float>();

        ready = false;
        beatBool = false;

        beatDelay = 0.0f;

        //Band definitions - e.g. subBass goes from 20HZ to 60HZ
        subBass.minFreq = 20;
        subBass.maxFreq = 60;

        bass.minFreq = 60;
        bass.maxFreq = 250;

        lowMid.minFreq = 250;
        lowMid.maxFreq = 500;

        midRange.minFreq = 500;
        midRange.maxFreq = 2000;

        uppRange.minFreq = 2000;
        uppRange.maxFreq = 4000;

        presence.minFreq = 4000;
        presence.maxFreq = 6000;

        brilliance.minFreq = 6000;
        brilliance.maxFreq = 20000;

        //Normal consists of all frequencies
        normal.minFreq = 0;
        normal.maxFreq = 44000 / 2;

        //multiLane = false;
        multiLane = songChoiceScript.GetMultiLane();

        lane = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //Get the spectrum data an danalyse it
        mutedAudioSource.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);
        AnalyseSpectrum(spectrum, mutedAudioSource.time);

        //After some time has passed and some beats have been calculated, play the song
        if(mutedAudioSource.time > (startDelay))
        {
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
            if (!ready)
            {
                ready = true;
            }
        }
        if(ready)
        {
            if (beatTimes.Count > 0)
            {
                //Compare the time values within 1 dec point
                //Always looking at the first member in beatTimes since it will be the one with the smallest time
                if ((float)Math.Round(audioSource.time, 1) == ((float)Math.Round(beatTimes[0], 1)))
                {
                    //Choose randomly in which lane to spawn beat
                    //Randomly get int between 0 and 2
                    if (multiLane)
                    {
                        lane = rand.Next(0, 3);

                        if (lane == 0)
                        {
                            beatGameObject = Instantiate(beatPrefab, beatPos + leftPos, Quaternion.identity);
                        }
                        else if (lane == 1)
                        {
                            beatGameObject = Instantiate(beatPrefab, beatPos, Quaternion.identity);
                        }
                        else if (lane == 2)
                        {
                            beatGameObject = Instantiate(beatPrefab, beatPos + rightPos, Quaternion.identity);
                        }
                    }
                    else
                    {
                        beatGameObject = Instantiate(beatPrefab, beatPos, Quaternion.identity);
                    }
                    
                    //Remove it from list
                    beatTimes.RemoveAt(0);
                }
            }
        }
    }

    //based on https://medium.com/giant-scam/algorithmic-beat-mapping-in-unity-real-time-audio-analysis-using-the-unity-api-6e9595823ce4
    //Analyse the spectrum data
    public void AnalyseSpectrum(float[] spectrum, float time)
    {
        CopySpectrums(spectrum);

        //Create new potential beat info object
        BeatInfo curBeat = new BeatInfo();
        curBeat.time = time;
        curBeat.energy = CalcBeatEnergy();
        //Add onto list of potential beats
        spectrumSamples.Add(curBeat);

        //After x number of potential beats have been added onto the list
        //Function requires a number of potential beats because it uses surrounding spectrum data 
        if (spectrumSamples.Count >= calcWindowSize)
        {
            //Bool to make sure beatBool and beatDelay only get their value assigned once
            if (!beatBool)
            {
                beatDelay = time;
                beatBool = true;
            }

            //Calculate the threshold required for this spectrum data to be considered a beat
            spectrumSamples[indexToProcess].threshold = CalcBeatThreshold(indexToProcess);

            //Calculate the energy after threshold, if this value is positive means the threshold was surpassed
            spectrumSamples[indexToProcess].thresholdEnergy = CalcBeatThresholdEnergy(indexToProcess);

            int indexToDetectPeak = indexToProcess - 1;

            bool curPeak = IsPeak(indexToDetectPeak);

            if (curPeak)
            {
                spectrumSamples[indexToDetectPeak].isPeak = true;

                if (time > beatTravelTime)
                {
                    //Add this beats' time onto the beatTime list, correcting for the time it takes for an object to travel from the top of the screen to the bottom
                    beatTimes.Add(time - beatTravelTime);
                }

            }
            indexToProcess++;
        }
        else
        {

            //Debug.Log(string.Format("Not ready yet.  At spectral flux sample size of {0} growing to {1}", spectralFluxSamples.Count, calcWindowSize));
        }

    }

    //Is this a peak?
    public bool IsPeak(int index)
    {
        //If the post threshold energy value of this potential beat is greater than the surrounding neighbours then yes
        if (index > 0)
        {
            if (spectrumSamples[index].thresholdEnergy > spectrumSamples[index + 1].thresholdEnergy &&
               spectrumSamples[index].thresholdEnergy > spectrumSamples[index - 1].thresholdEnergy)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    //Return energy after comparison with threshold
    public float CalcBeatThresholdEnergy(int index)
    {
        return Mathf.Max(0f, spectrumSamples[index].energy - spectrumSamples[index].threshold);
    }

    //Calculate threshold for detecting a beat
    public float CalcBeatThreshold(int index)
    {
        //Calculate the starting and ending index for energy calculation
        int windowStartIndex = Mathf.Max(0, index - calcWindowSize / 2);
        int windowEndIndex = Mathf.Min(spectrumSamples.Count - 1, index + calcWindowSize / 2);

        //Sum up all the samples in a calcWindowSize / 2 around the current sample
        float energy = 0.0f;
        for (int i = windowStartIndex; i < windowEndIndex; i++)
        {
            energy += spectrumSamples[i].energy;
        }

        float avg = energy / (windowEndIndex - windowStartIndex);
        return avg * thresholdMultiplier;
    }

    //Calculate the energy value 
    public float CalcBeatEnergy()
    {
        float energy = 0.0f;

        FrequencyBand chosenBand = new FrequencyBand();

        chosenBand = normal;
        
        //Within the chosen frequency band sum up all the spectrum values
        for (int i = chosenBand.minFreq / (int)freqBand; i < Mathf.Min(chosenBand.maxFreq / (int)freqBand, sampleNum); i++)
        {
            energy += Mathf.Max(0f, currentSpectrum[i] - previousSpectrum[i]);
        }

        return energy;
    }

    //Swap around spectrum arrays
    public void CopySpectrums(float[] spec)
    {
        Array.Copy(currentSpectrum, previousSpectrum, sampleNum);
        Array.Copy(spec, currentSpectrum, sampleNum);
    }
}
