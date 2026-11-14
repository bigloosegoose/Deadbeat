using Unity.VisualScripting;
using UnityEngine;

public class SwordSocketDriver : MonoBehaviour
{

    [SerializeField] Transform socket;
    [SerializeField] float smoothing = 12f;

    Vector3 targetPos;
    Quaternion targetRot;


    void Awake()
    {
        targetPos = socket.localPosition;
        targetRot = socket.localRotation;
    }

    public void SetPose(Vector3 pos, Vector3 euler)
    {
        targetPos = pos;
        targetRot = Quaternion.Euler(euler);
    }
    void LateUpdate()
    {
        float t = Mathf.Clamp01(Time.deltaTime * smoothing);
        socket.localPosition = Vector3.Lerp(socket.localPosition, targetPos, t);
        socket.localRotation = Quaternion.Lerp(socket.localRotation, targetRot, t);
    }
}
