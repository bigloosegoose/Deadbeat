using UnityEngine;

[CreateAssetMenu(fileName = "NewSong", menuName ="Song Data")]
public class SongData : ScriptableObject
{
    public float songBpm;
    public float footstepBpm;
    public float firstBeatOffset;
    public AudioClip[] footstepSounds;
    public AudioClip songFile;
}
