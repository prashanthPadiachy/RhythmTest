using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using System.IO;
using UnityEngine.Networking;
using System;
public class SongManager : MonoBehaviour
{
    public static SongManager Instance;
    public AudioSource audioSource;
    public float songDelayinSecs;
    public int inputDeleyinMs;

    public string fileLocation;
    public float noteTime;
    public float noteSpawnY;//where note instantiates
    public float noteTapY;//where it should be tapped

    public double ErrorMargin;//Secs
    public float noteDespawnY
    {
        get 
        {
            return noteTapY - (noteSpawnY - noteTapY);
        }
    }

    public static MidiFile midiFile;

    public Lane[] lanes;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Instance = this;

        if (Application.streamingAssetsPath.StartsWith("Http"))
        {
            StartCoroutine(ReadFromWebsite());
        }
        else 
        {
            ReadFromFile();
        }
    }

    private void ReadFromFile()
    {
        MidiFile.Read(Application.streamingAssetsPath+"/"+fileLocation);
        GetDataFromMidi();
    }

    private IEnumerator ReadFromWebsite()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(Application.streamingAssetsPath + "/" + fileLocation)) 
        {
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else 
            {
                byte[] results = www.downloadHandler.data;
                using (var stream = new MemoryStream(results))
                { 
                    midiFile = MidiFile.Read(stream);
                    GetDataFromMidi();
                }
            }
        }
    }

    public void GetDataFromMidi()
    { 
        var notes = midiFile.GetNotes();
        var array = new Melanchall.DryWetMidi.Interaction.Note[notes.Count];
        notes.CopyTo(array, 0);

        foreach (var lane in lanes) lane.SetTimeStamps(array);

        Invoke(nameof(StartSong), songDelayinSecs);
    }

    public void StartSong() 
    { 
        audioSource.Play();
    }

    public static double GetAudioSourceTime() 
    {
        return Instance.audioSource.timeSamples / Instance.audioSource.clip.frequency;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
