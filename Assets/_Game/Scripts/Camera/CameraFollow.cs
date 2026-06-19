using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 5, -8);
    public float followSpeed = 5f;

    private void LateUpdate()
    {
        if (target == null)
        {
            // Attempt to find player dynamically
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null) target = player.transform;
            return;
        }
        Vector3 targetPos = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * followSpeed);
        transform.LookAt(target.position + Vector3.up * 1f);
    }
}