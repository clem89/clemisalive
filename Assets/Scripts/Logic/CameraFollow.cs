using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform target;

    void LateUpdate()
    {
        if (target == null) return;
        // z축(카메라 심도)은 유지하고 xy만 즉시 추적
        transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);
    }
}
