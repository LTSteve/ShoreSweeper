using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Instrument
{
    public float Intensity = 0;

    private AudioSource mySource;
    private AudioSource mySource2;
    private SoundtrackPart lowClips;
    private SoundtrackPart midClips;
    private SoundtrackPart highClips;

    private double barLen;
    private double playEnd = -1.0;

    public Instrument(AudioSource audioSource, AudioSource audioSource2, SoundtrackPart lowClips, SoundtrackPart midClips, SoundtrackPart highClips, double beatLength)
    {
        mySource = audioSource;
        mySource2 = audioSource2;

        this.lowClips = lowClips;
        this.midClips = midClips;
        this.highClips = highClips;

        barLen = beatLength * 4.0; //1 4/4 bar
    }

    public void Play(double atTime)
    {

    }
}
