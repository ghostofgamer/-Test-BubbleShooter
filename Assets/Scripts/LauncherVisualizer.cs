using InitializationContent;
using UnityEngine;

public class LauncherVisualizer : MonoBehaviour
{
    [Header("Settings")] [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Color _colorLine;
    [SerializeField] private Color _spreadColor;
    [SerializeField] private Material _normalMaterial;
    [SerializeField] private Material _spreadMaterial;
    
    [SerializeField] private int linePoints = 30;
    [SerializeField] private float lineStep = 0.1f;
    [SerializeField] private float minWidth = 0.05f;
    [SerializeField] private float maxWidth = 0.3f;

    [SerializeField] private ScreenData _screenData;

    public void Init(ScreenData screen)
    {
        if (_normalMaterial != null)
            lineRenderer.material = _normalMaterial;
        lineRenderer.startColor = _colorLine;
        lineRenderer.endColor = _colorLine;

        _screenData = screen;
        Hide();
    }
    
public void UpdateTrajectory(Vector3 ballPos, Vector3 launchDir, float launchForce, float ballRadius, float spread, bool isFullPower = false)
    {
        if (lineRenderer == null) return;

        Color lineColor = isFullPower ? _spreadColor : _colorLine;
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;
        lineRenderer.material.color = lineColor;
        
        if (isFullPower && _spreadMaterial != null)
            lineRenderer.material = _spreadMaterial;
        else if (_normalMaterial != null)
            lineRenderer.material = _normalMaterial;

        float left = -_screenData.Width / 2f + ballRadius;
        float right = _screenData.Width / 2f - ballRadius;
        float top = _screenData.Height / 2f - ballRadius;
        float bottom = -_screenData.Height / 2f;

        lineRenderer.positionCount = linePoints;

        Vector3 pos = ballPos;
        Vector3 vel = launchDir.normalized * launchForce;

        AnimationCurve widthCurve = new AnimationCurve();
        widthCurve.AddKey(0f, minWidth);

        for (int pointIndex = 0; pointIndex < linePoints; pointIndex++)
        {
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
                
                if (spread > 0)
                {
                    float t = (float)pointIndex / linePoints;
                    float width = Mathf.Lerp(minWidth, maxWidth, t * spread / 15f);
                    widthCurve.AddKey(t, width);
                }
                break;
            }

            lineRenderer.SetPosition(pointIndex, new Vector3(pos.x, pos.y, -0.1f));
            
            if (spread > 0)
            {
                float t = (float)pointIndex / linePoints;
                float width = Mathf.Lerp(minWidth, maxWidth, t * spread / 15f);
                widthCurve.AddKey(t, width);
            }
        }
        
        lineRenderer.widthCurve = widthCurve;

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