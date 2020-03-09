using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soundtrack : MonoBehaviour
{
    public static Soundtrack Instance;

    public static bool PlayClearCadence = false;

    //starts at G4, going down chromatically
    public AudioClip[] XylophoneClips;

    public SoundtrackPart HornLow;
    public SoundtrackPart HornMid;
    public SoundtrackPart HornHigh;

    public SoundtrackPart SnareLow;
    public SoundtrackPart SnareMid;
    public SoundtrackPart SnareHigh;

    public SoundtrackPart ViolinLow;
    public SoundtrackPart ViolinMid;
    public SoundtrackPart ViolinHigh;

    public double SecondsPerBar = 2.285714;

    public AudioSource AmbienceSource;

    private Instrument Horn;
    private Instrument Snare;
    private Instrument Violin;
    private Xylophone Xylo;

    private float PointGainRate = 0f;
    private float LastPointTotal = 0f;

    private Queue<Section> currentSong = new Queue<Section>();
    private Section currentSection;

    private double PlayHead = 0.0;
    private double CurrentSongEndTime = 0.0;

    public static bool SoundtrackActive = false;

    public void Start()
    {
        Instance = this;

        //assign audio sources
        var sources = GetComponentsInChildren<AudioSource>();

        Xylo = new Xylophone(transform.Find("Xylo").GetComponent<AudioSource>(), transform.Find("Xylo2").GetComponent<AudioSource>(), XylophoneClips, SecondsPerBar / 4.0);

        Horn = new Instrument(transform.Find("Horn").GetComponent<AudioSource>(), transform.Find("Horn2").GetComponent<AudioSource>(), HornLow, HornMid, HornHigh, SecondsPerBar / 4.0);
        Snare = new Instrument(transform.Find("Snare").GetComponent<AudioSource>(), transform.Find("Snare2").GetComponent<AudioSource>(), SnareLow, SnareMid, SnareHigh, SecondsPerBar / 4.0);
        Violin = new Instrument(transform.Find("Violin").GetComponent<AudioSource>(), transform.Find("Violin2").GetComponent<AudioSource>(), ViolinLow, ViolinMid, ViolinHigh, SecondsPerBar / 4.0);
    }

    public void FixedUpdate()
    {
        var nextTime = Time.fixedDeltaTime + PlayHead;

        //generate rest + new song
        if (nextTime > CurrentSongEndTime)
        {
            var numbers = Numberizer.GetDisplayNumbers();

            var sectionsCount = numbers.GetNumeral(8) + 4;

            for(var i = 0; i < sectionsCount; i++)
            {
                var toAdd = new Section();

                currentSong.Enqueue(toAdd);

                CurrentSongEndTime += toAdd.length;
            }

            //start rest section,
            var restCount = numbers.GetNumeral(3) + 1;
            
            //add [restcount] 4 bar rest[s]
            currentSection = new Section();

            CurrentSongEndTime += currentSection.length;
        }

        //start next section
        if(nextTime > currentSection.endTime)
        {
            var startTime = currentSection.endTime;
            currentSection = currentSong.Dequeue();

            if(currentSection != null && currentSection.instruments != null)
                foreach(var instrument in currentSection.instruments)
                {
                    instrument.Play(nextTime);
                }
        }
    }

    public void Update()
    {
        if (!SoundtrackActive)
        {
            return;
        }

        if(LastPointTotal == 0)
        {
            LastPointTotal = Director.PlayerScore;
        }

        if(PlayerController.Instance == null)
        {
            return;
        }

        //Get Horn Intensity
        PointGainRate = PointGainRate * (1 - Time.deltaTime) + (Director.PlayerScore - LastPointTotal) * Time.deltaTime;
        if (PointGainRate < 0) PointGainRate = 0;
        Horn.Intensity = Mathf.Clamp(PointGainRate * 3.5f, 0, 2.99f);

        //Get Violin Intensity
        var playerLocation = PlayerController.Instance.transform.position;
        var centerZone = Director.CenterZone();
        var centerLocation = centerZone.transform.position + centerZone.IslandCenterOffset;
        var distance = Vector3.Distance(playerLocation, centerLocation);
        var speed = PlayerController.Instance.GetComponent<Rigidbody2D>().velocity.magnitude;
        if(distance > 30 && speed < 1)
        {
            Violin.Intensity = 0.1f;
        }
        else if(distance > 30)
        {
            Violin.Intensity = 2.1f;
        }
        else if(distance <= 30 && speed > 1)
        {
            Violin.Intensity = 1.1f;
        }

        //Get Snare Intensity
        Snare.Intensity = Mathf.Clamp(PointGainRate * 3.5f - 1, 0, 1.99f);
        if (PlayClearCadence)
        {
            Snare.Intensity = 2.1f;
        }

        LastPointTotal = Director.PlayerScore;
    }
}
