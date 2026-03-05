using System.Collections;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow Instance { get; private set; }

    [SerializeField] Transform target;

    bool _isShaking;

    void Awake()
    {
        Instance = this;
    }

    void LateUpdate()
    {
        if (_isShaking || target == null) return;
        transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);
    }

    public void Shake(float duration = 0.2f, float magnitude = 0.15f)
    {
        StopAllCoroutines();
        StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        _isShaking = true;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            Vector2 offset = Random.insideUnitCircle * magnitude;
            if (target != null)
                transform.position = new Vector3(
                    target.position.x + offset.x,
                    target.position.y + offset.y,
                    transform.position.z);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        _isShaking = false;
    }
}
