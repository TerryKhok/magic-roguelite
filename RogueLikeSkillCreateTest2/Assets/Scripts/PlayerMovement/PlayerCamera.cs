using UnityEngine;

public class PlayerCamera: MonoBehaviour
{
    public float m_dampTime = 0.15f;
    public Transform m_target;

    private Vector3 m_velocity;

    private void Update()
    {
        if (!m_target) return;

        var camera = Camera.main;
        var selfPosition = transform.position;
        var targetPosition = m_target.position;
        var point = camera.WorldToViewportPoint(targetPosition);
        var delta = targetPosition - camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z));
        var destination = selfPosition + delta;
        transform.position = Vector3.SmoothDamp(selfPosition, destination, ref m_velocity, m_dampTime);
    }
}