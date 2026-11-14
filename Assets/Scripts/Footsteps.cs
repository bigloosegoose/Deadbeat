using UnityEngine;

public class Footsteps : MonoBehaviour
{
    [SerializeField] AudioSource source;
    [SerializeField] AudioClip[] clips;
    [SerializeField] CharacterController controller;
    [SerializeField] float minClipWeight = 0.5f;
    [SerializeField] Vector2 pitchRange = new Vector2(0.95f, 1.05f);

    int lastIndex = -1;
    public void Footstep(AnimationEvent e)
    {
        if (e.animatorClipInfo.weight < minClipWeight) return;
        if (!controller.isGrounded) return;

        int i;
        do { i = Random.Range(0, clips.Length); }
        while (clips.Length > 1 && i == lastIndex);
        lastIndex = i;

        source.pitch = Random.Range(pitchRange.x, pitchRange.y);
        source.PlayOneShot(clips[i]);

    }

}
