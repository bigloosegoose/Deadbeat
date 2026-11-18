using UnityEngine;

public class StepSync : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] Footsteps footsteps;
    [SerializeField] float naturalStepBpm = 120f;
    [SerializeField] float stepsPerCycle = 2f;
    [SerializeField] float plantPhase = 0f; //first foot time
    [SerializeField] float correction = 0.5f; //correction for sync


    static readonly int StepSpeedHash = Animator.StringToHash("StepSpeed");

    void Update()
    {
        float baseSpeed = footsteps.stepBpm / naturalStepBpm;
        double cyclePos = footsteps.SongPosition / footsteps.Interval / stepsPerCycle;
        float wanted = (float)(cyclePos - System.Math.Floor(cyclePos));

        float actual = animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1f;
        float error = Mathf.Repeat(wanted + plantPhase - actual + 0.5f, 1f) - 0.5f;

        float speed = baseSpeed * (1f + correction * error);
        //Debug.Log(speed);
        animator.SetFloat(StepSpeedHash, footsteps.isMoving ? speed : 1f);
    }

}
