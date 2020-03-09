using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Xylophone
{
    private AudioSource mySource;
    private AudioSource mySource2;
    private AudioClip[] myClips;

    private double beatLen;

    public Xylophone(AudioSource audioSource, AudioSource audioSource2, AudioClip[] clips, double beatLength)
    {
        mySource = audioSource;
        mySource2 = audioSource2;

        myClips = clips;

        beatLen = beatLength / 4.0; //16th notes
    }

    public void Play(int note)
    {
        mySource.clip = myClips[note];

        mySource.PlayScheduled(((int)(AudioSettings.dspTime / beatLen)) * beatLen + beatLen);
    }
}
