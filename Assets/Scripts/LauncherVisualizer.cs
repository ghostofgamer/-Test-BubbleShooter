using InitializationContent;
using UnityEngine;

public class LauncherVisualizer : MonoBehaviour
{
    [Header("Settings")] [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Color _colorLine;
   
    [SerializeField] private int linePoints = 30;
    [SerializeField] private float lineStep = 0.1f;

    [SerializeField] private ScreenData _screenData;

    public void Init(ScreenData screen)
    {
        lineRenderer.material.color = _colorLine;

        _screenData = screen;
        Hide();
    }
    
    public void UpdateTrajectory(Vector3 ballPos, Vector3 launchDir, float launchForce, float ballRadius, float spread)
    {
        if (lineRenderer == null) return;

        lineRenderer.positionCount = linePoints;

        Vector3 pos = ballPos;
        Vector3 vel = launchDir.normalized * launchForce;
        
        float gravity = 9.8f;

        float left = -_screenData.Width / 2f + ballRadius;
        float right = _screenData.Width / 2f - ballRadius;
        float top = _screenData.Height / 2f - ballRadius;
        float bottom = -_screenData.Height / 2f;

        int pointIndex = 0;

        while (pointIndex < linePoints)
        {
            vel.y -= gravity * lineStep;
            pos += vel * lineStep;

            if (pos.x <= left || pos.x >= right)
            {
                vel.x = -vel.x;
                pos.x = Mathf.Clamp(pos.x, left, right);
            }

            if (pos.y <= bottom || pos.y >= top)
            {
                if (pos.y >= top) pos.y = top;
                lineRenderer.positionCount = pointIndex + 1;
                lineRenderer.SetPosition(pointIndex, new Vector3(pos.x, pos.y, -0.1f));
                break;
            }

            lineRenderer.SetPosition(pointIndex, new Vector3(pos.x, pos.y, -0.1f));
            pointIndex++;
        }

        lineRenderer.enabled = true;
    }

    public void Hide()
    {
        if (lineRenderer != null)
            lineRenderer.enabled = false;
    }

    public void Show()
    {
        if (lineRenderer != null)
            lineRenderer.enabled = true;
    }
}