using UnityEngine;

public class CameraMover : MonoBehaviour
{
    public GameObject target;
    private float distance = 2f;
    private float lookAt = 1f;

    private float smoothSpeed = 0.2f;

    // Update is called once per frame
    void LateUpdate()
    {
        var playerBackPosition = target.transform.position - target.transform.forward * distance;
        var desiredPosition = new Vector3(playerBackPosition.x, playerBackPosition.y + 2, playerBackPosition.z);
        var smoothPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothPosition;
        transform.LookAt(target.transform.position + new Vector3(0, lookAt, 0));
    }
}
