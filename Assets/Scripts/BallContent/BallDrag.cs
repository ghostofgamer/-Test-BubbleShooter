using UnityEngine;

namespace BallContent
{
    public class BallDrag : MonoBehaviour
    {
        [SerializeField] private Transform _shootPoint;
        [SerializeField] private float _maxDragDistance = 2f;

        private Vector2 _startPosition;
        private bool _isDragging = false;

        private void Start()
        {
            ResetBall();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
                _isDragging = true;

            if (Input.GetMouseButton(0) && _isDragging)
                DragBall();

            if (Input.GetMouseButtonUp(0))
            {
                _isDragging = false;
                ResetBall();
            }
        }

        private void DragBall()
        {
            Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = mouseWorld - (Vector2)_shootPoint.position;

            if (direction.magnitude > _maxDragDistance)
                direction = direction.normalized * _maxDragDistance;

            transform.position = _shootPoint.position - (Vector3)direction;
        }

        private void ResetBall()
        {
            transform.position = _shootPoint.position;
        }
    }
}