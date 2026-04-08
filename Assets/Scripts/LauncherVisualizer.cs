using System.Collections.Generic;
using InitializationContent;
using UnityEngine;

public class LauncherVisualizer : MonoBehaviour
{
    [SerializeField] private LineRenderer linePrefab;

    [Header("Trajectory")] [SerializeField]
    private int linePoints = 30;

    [SerializeField] private float lineStep = 0.1f;

    [Header("Spread")] [SerializeField] private float maxSpreadWidth = 1.5f;

    [Header("Colors")] [SerializeField] private Color normalColor = Color.black;
    [SerializeField] private Color spreadColor = Color.red;

    [SerializeField] private ScreenData screenData;

    private LineRenderer leftLine;
    private LineRenderer rightLine;
    private LineRenderer centerLine;

    #region PUBLIC

    public void Init(ScreenData screen)
    {
        screenData = screen;

        centerLine = Instantiate(linePrefab, transform);
        leftLine = Instantiate(linePrefab, transform);
        rightLine = Instantiate(linePrefab, transform);

        Hide();
    }

    public void UpdateTrajectory(
        Vector3 ballPos,
        Vector3 launchDir,
        float launchForce,
        float ballRadius,
        float spread,
        bool isFullPower)
    {
        List<Vector3> centerPoints = CalculateCenterTrajectory(
            ballPos,
            launchDir,
            launchForce,
            ballRadius
        );

        if (!isFullPower)
        {
            DrawLine(centerLine, centerPoints, normalColor);
            centerLine.enabled = true;
            leftLine.enabled = false;
            rightLine.enabled = false;
        }
        else
        {
            var smoothPoints = SmoothPath(centerPoints, 10);
            BuildSpread(smoothPoints, spread, out var left, out var right);

            centerLine.enabled = false;
            DrawLine(leftLine, left, spreadColor);
            DrawLine(rightLine, right, spreadColor);
        }
    }
    
    public void Hide()
    {
        centerLine.enabled = false;
        leftLine.enabled = false;
        rightLine.enabled = false;
    }

    #endregion

    #region CORE

    List<Vector3> CalculateCenterTrajectory(
        Vector3 startPos,
        Vector3 dir,
        float force,
        float radius)
    {
        List<Vector3> points = new();

        float left = -screenData.Width / 2f + radius;
        float right = screenData.Width / 2f - radius;
        float top = screenData.Height / 2f - radius;

        Vector3 pos = startPos;
        Vector3 vel = dir.normalized * force;

        /*for (int i = 0; i < linePoints; i++)
        {
            pos += vel * lineStep;

            // отражение только по X
            if (pos.x <= left || pos.x >= right)
            {
                vel.x = -vel.x;
                pos.x = Mathf.Clamp(pos.x, left, right);
            }

            // верхняя граница — остановка траектории
            if (pos.y >= top)
            {
                pos.y = top;
                points.Add(pos);
                break; // 🔥 как было в старом коде
            }

            points.Add(pos);
        }*/
        for (int i = 0; i < linePoints; i++)
        {
            Vector3 nextPos = pos + vel * lineStep;
    
            bool hitWall = false;
            bool hitLeftWall = nextPos.x < left;
            bool hitRightWall = nextPos.x > right;
    
            if (hitLeftWall || hitRightWall)
            {
                float wallX = hitLeftWall ? left : right;
                float t = (wallX - pos.x) / vel.x;
                t = Mathf.Clamp01(t);
            
                pos.x = wallX;
                pos.y += vel.y * t;
                vel.x = -vel.x;
            
                float remaining = lineStep - t;
                if (remaining > 0)
                {
                    pos += vel * remaining;
                }
            
                if (pos.x < left) pos.x = left;
                if (pos.x > right) pos.x = right;
            }
            else
            {
                pos = nextPos;
            }

            if (pos.y >= top)
            {
                pos.y = top;
                points.Add(pos);
                break;
            }

            points.Add(pos);
        }

        return points;
    }
    
    List<Vector3> SmoothPath(List<Vector3> points, int iterations = 2)
    {
        if (points.Count < 3)
            return points;

        List<Vector3> result = new(points);

        for (int k = 0; k < iterations; k++)
        {
            List<Vector3> temp = new();
            temp.Add(result[0]);

            for (int i = 1; i < result.Count - 1; i++)
            {
                Vector3 prev = result[i - 1];
                Vector3 current = result[i];
                Vector3 next = result[i + 1];

                Vector3 smooth = (prev + current * 2f + next) / 4f;

                temp.Add(smooth);
            }

            temp.Add(result[^1]);
            result = temp;
        }

        return result;
    }
    
    void BuildSpread(
        List<Vector3> center,
        float spread,
        out List<Vector3> left,
        out List<Vector3> right)
    {
        left = new List<Vector3>();
        right = new List<Vector3>();

        Vector3 prevDir = Vector3.zero;

        for (int i = 0; i < center.Count; i++)
        {
            float t = i / (float)center.Count;

            Vector3 dir;

            if (i == 0)
            {
                dir = (center[i + 1] - center[i]).normalized;
            }
            else if (i == center.Count - 1)
            {
                dir = (center[i] - center[i - 1]).normalized;
            }
            else
            {
                Vector3 dir1 = (center[i] - center[i - 1]).normalized;
                Vector3 dir2 = (center[i + 1] - center[i]).normalized;

                dir = (dir1 + dir2).normalized; // 🔥 ключ
            }

            // дополнительное сглаживание
            if (i > 0)
            {
                dir = Vector3.Lerp(prevDir, dir, 0.5f).normalized;
            }

            prevDir = dir;

            Vector3 normal = new Vector3(-dir.y, dir.x, 0f);

            float offset = maxSpreadWidth * spread * t;

            left.Add(center[i] + normal * offset);
            right.Add(center[i] - normal * offset);
        }
    }

    void DrawLine(LineRenderer line, List<Vector3> points, Color color)
    {
        line.enabled = true;
        line.positionCount = points.Count;

        line.startColor = color;
        line.endColor = color;
        line.material.color = color;

        for (int i = 0; i < points.Count; i++)
        {
            line.SetPosition(i, points[i]);
        }
    }

    #endregion
}