using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Melanchall.DryWetMidi.Interaction;
using System;
public class Lane : MonoBehaviour
{
    public Melanchall.DryWetMidi.MusicTheory.NoteName noteRestriction; //restricts notes to a certain key
    public KeyCode input;
    public GameObject notePrefab;
    List<Note> notes = new List<Note>();
    public List<double> timeStamps = new List<double>();//timestamps where player needs to press THIS input

    int spawnIndex = 0;//keeps track of what timestamp needs to be spawned or detected 
    int inputIndex = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void SetTimeStamps(Melanchall.DryWetMidi.Interaction.Note[] array) 
    {
        foreach (var note in array)
        {
            //check if notename is = to our note restriction
            if (note.NoteName == noteRestriction) 
            {
                var metricTimeSpan = TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, SongManager.midiFile.GetTempoMap());//get note time (convert from midi time to metric
                //convert metric time to seconds -- add it to the list
                timeStamps.Add((double)metricTimeSpan.Minutes * 60f + metricTimeSpan.Seconds + (double)metricTimeSpan.Milliseconds / 1000f);
            }
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (SongManager.Instance == null) return;
        //Handle Spawning Notes
        if (spawnIndex < timeStamps.Count) 
        {
            if (SongManager.GetAudioSourceTime() >= timeStamps[spawnIndex]- SongManager.Instance.noteTime) //want to spawn notes before player taps it!!
            {
                var note = Instantiate(notePrefab, transform); //instantiates the note and adds it to the list
                notes.Add(note.GetComponent<Note>());
                note.GetComponent<Note>().assignedTime = (float)timeStamps[spawnIndex];//set vars assigned time so node knows how to position itself
                spawnIndex++;
            }
        }

        //Handle Inputs
        if (inputIndex < timeStamps.Count) 
        {
            //accessing stuff
            double timeStamp = timeStamps[inputIndex];
            double errorMargin = SongManager.Instance.ErrorMargin;
            double audioTime = SongManager.GetAudioSourceTime() - (SongManager.Instance.inputDeleyinMs /1000.0);

            if (Input.GetKeyDown(input)) 
            {
                //See if tap is within ErrorMargin
                if (Math.Abs(audioTime - timeStamp) < errorMargin)
                {
                    Hit();
                    print($"Hit on {inputIndex} note");
                    Destroy(notes[inputIndex].gameObject);
                    inputIndex++;
                }
                else //bad hit
                {
                    print($"Hit inaccurate on {inputIndex} note");
                }
            }
            if (timeStamp + errorMargin <= audioTime) //missed note
            {
                Miss();
                print($"Missed {inputIndex} Note");
                inputIndex++;
            }
        }
    }

    private void Miss()
    {
        ScoreManager.miss();
    }

    private void Hit()
    {
        ScoreManager.hit();
    }
}
