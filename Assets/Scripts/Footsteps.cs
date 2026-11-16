using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Audio;

// Handles both song and footstep for now

public class Footsteps : MonoBehaviour
{
    [Header("References")]
    [SerializeField] SongData songData;

    [SerializeField] CharacterController controller;
    [SerializeField] AudioMixerGroup musicGroup, sfxGroup;

    [Header("Audio")]
    [SerializeField] int poolSize = 4;
    [SerializeField] Vector2 pitchRange = new Vector2(0.95f, 1.05f);
    [SerializeField] float lookAhead = 0.15f; //dspTime error corrector

    [Header("Movement Gate")]
    [SerializeField] float minSpeed = 0.5f;
    [SerializeField] float minMovingTime = 0.08f;

    [Header("Testing")]
    [SerializeField] bool autoStart = true; // no conductor


    //[SerializeField] float minClipWeight = 0.5f;

    int lastIndex = -1;

    // Le conductor vars
    AudioSource songPlayer;
    AudioSource[] pool;
    int poolIndex;
    int lastClip = -1;
    double interval;
    double songStartDsp;
    long nextStep;
    bool running;
    float movingTime;

    public float stepBpm => songData.footstepBpm;
    public double Interval => interval;
    public double SongPosition => AudioSettings.dspTime - songStartDsp - songData.firstBeatOffset;
    public bool isMoving => movingTime >= minMovingTime && controller.isGrounded;
    


    void Awake()
    {
        if (songData == null)
        {
            Debug.LogError($"{nameof(Footsteps)} requires a SongData asset.", this);
            enabled = false; //disables the script
            return;
        }

        interval = 60.0 / songData.footstepBpm;

        pool = new AudioSource[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            pool[i] = gameObject.AddComponent<AudioSource>();
            pool[i].outputAudioMixerGroup = sfxGroup;
            pool[i].playOnAwake = false;
            pool[i].spatialBlend = 0f;
        }
    }

    private void Start()
    {
        if (!autoStart) return;

        double dspStart = AudioSettings.dspTime + 0.2;

        
        songPlayer = gameObject.AddComponent<AudioSource>();
        songPlayer.outputAudioMixerGroup = musicGroup;
        songPlayer.playOnAwake = false;
        songPlayer.spatialBlend = 0f;
        songPlayer.clip = songData.songFile;
        //songPlayer.volume = 0.2f; //debug remove later

        if (songData.songFile != null) songPlayer.PlayScheduled(dspStart);
        StartGrid(dspStart);
    }

    public void StartGrid(double dspStart)
    {
        Debug.Log("Grid Started");
        songStartDsp = dspStart;
        nextStep = 0;
        running = true;
    }

    public void Update()
    {
        if (!running) return;
        Vector3 v = controller.velocity; v.y = 0f;
        movingTime = v.magnitude > minSpeed ? movingTime + Time.deltaTime : 0f;

        double now = AudioSettings.dspTime;
        double horizon = now + lookAhead;

        while(StepTime(nextStep) < horizon)
        {
            double t = StepTime(nextStep);
            if (t >= now && isMoving) Schedule(t);
            nextStep++;
        }
    }

    double StepTime(long n) => songStartDsp + songData.firstBeatOffset + n * interval;

    void Schedule(double t)
    {
        var clips = songData.footstepSounds;
        if (clips == null || clips.Length == 0) return;

        int i;
        do { i = Random.Range(0, clips.Length); }
        while (clips.Length > 1 && i == lastIndex);
        lastIndex = i;

        var src = pool[poolIndex];
        poolIndex = (poolIndex + 1) % pool.Length;

        src.clip = clips[i];
        src.pitch = Random.Range(pitchRange.x, pitchRange.y);
        src.PlayScheduled(t);
    }

    public void Footstep(AnimationEvent e) //remove
    {

    }

}
