using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;


struct Energy
{
    public float energy;
    public int sample;
}

//Old implementation of beat detection
//based on https://www.gamedev.net/articles/programming/math-and-physics/beat-detection-algorithms-r1952
public class MusicLoad : MonoBehaviour
{
    public AudioClip musicClip;

    float[] musicSamples;

    int sampleRate = 44100;

    List<int> beats = new List<int>();

    List<int> returnBeats = new List<int>();

    
    // Start is called before the first frame update
    void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Function1()
    {
        if (musicClip)
        {
            print("Frequency: " + musicClip.frequency);
            print("Samples: " + musicClip.samples);
            print("Channels: " + musicClip.channels);

            musicSamples = new float[musicClip.samples];

            musicClip.GetData(musicSamples, 0);

            Energy[] energyBuffer = new Energy[43];
            //Keep track of energyBuffer position
            int b = 0;

            float songLength = musicClip.samples / sampleRate;
            print("Song length: " + songLength);

            //k = seconds
            for (int k = 0; k < songLength; k++)
            {
                //1 second chunks
                //43 * 1024 = 44032 = ~ 44100
                //Calculate energy in 43, 1024 sample sized chunks
                for (int j = (k * 43); j < ((k + 1) * 43); j++)
                {
                    //Energy of 1024 samples
                    float energy = 0;

                    //Calculate energy in 1024 samples
                    for (int i = (j * 1024); i < ((j + 1) * 1024); i++)
                    {
                        energy = energy + Mathf.Pow(musicSamples[i], 2);
                    }
                    //Add energy of samples onto buffer
                    energyBuffer[b].energy = energy;
                    energyBuffer[b].sample = j;

                    b++;
                }

                float totalEnergy = 0;

                //Total energy in a chunk
                for (int i = 0; i < energyBuffer.Length; i++)
                {
                    totalEnergy += energyBuffer[i].energy;
                }

                float averageEnergy = totalEnergy / energyBuffer.Length;

                float variance = 0;
                float totalVariance = 0;

                //The variance in the energies
                for (int i = 0; i < energyBuffer.Length; i++)
                {
                    variance = Mathf.Pow(averageEnergy - energyBuffer[i].energy, 2);
                    totalVariance += variance;
                }

                float averageVariance = totalVariance / energyBuffer.Length;

                float c = 0;

                //C constant - threshold value for determining if the energy is high enough for a beat
                //c = (float)-0.00000015 * averageVariance + (float)1.5142857;
                c = (float)-0.0025714 * averageVariance + (float)1.5142857;

                for (int i = 0; i < energyBuffer.Length; i++)
                {
                    //If energy of a chunk is higher than c * the average energy of chunks
                    if (energyBuffer[i].energy > c * averageEnergy)
                    {
                        //Add this as a beat
                        beats.Add(energyBuffer[i].sample);
                    }

                }

                b = 0;
            }
        }
    }

    //Reduce the number of closely packed beats
    void ReduceBeats()
    {
        List<int> beatsToDelete = new List<int>();
        for (int i = 1; i < beats.Count; i++)
        {
            if (Math.Round((float)beats[i] / 43, 1) == Math.Round((float)beats[i - 1] / 43, 1))
            {
                beatsToDelete.Add(beats[i]);
            }
        }

        print("Num to delete: " + beatsToDelete.Count);

        returnBeats = beats.Except(beatsToDelete).ToList();

        print("Count: " + returnBeats.Count);
    }

    public List<int> GetBeats()
    {
        return returnBeats;
    }

}
