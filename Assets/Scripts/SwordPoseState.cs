using UnityEngine;

public class SwordPoseState : StateMachineBehaviour
{
    public Vector3 localPosition;
    public Vector3 localEuler;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var driver = animator.GetComponent<SwordSocketDriver>();
        if (driver) driver.SetPose(localPosition, localEuler);
    }
}
