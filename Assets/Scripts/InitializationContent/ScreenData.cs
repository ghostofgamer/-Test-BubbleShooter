using UnityEngine;

namespace InitializationContent
{
    public class ScreenData : MonoBehaviour
    {
        public float Width { get; private set; }
        public float Height { get; private set; }
        public float Top { get; private set; }
        public float Bottom { get; private set; }
        public float Left { get; private set; }
        public float Right { get; private set; }

        public ScreenData(Camera camera)
        {
            Height = camera.orthographicSize * 2;
            Width = Height * camera.aspect;

            Vector3 camPos = camera.transform.position;

            Top = camPos.y + Height / 2;
            Bottom = camPos.y - Height / 2;
            Left = camPos.x - Width / 2;
            Right = camPos.x + Width / 2;
        }
    }
}