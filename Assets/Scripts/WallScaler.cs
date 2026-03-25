using UnityEngine;

public class WallScaler : MonoBehaviour
{
    public bool isVertical = true; // Для стен (true), для потолка (false)
    public float thickness = 0.5f;
    public bool _leftWall;

    void Start()
    {
        Resize();
    }

    void Resize()
    {
        // Получаем размеры камеры в мировых координатах
        Camera mainCamera = Camera.main;
        float cameraHeight = mainCamera.orthographicSize * 2;
        float cameraWidth = cameraHeight * mainCamera.aspect;

        // Настраиваем масштаб и позицию в зависимости от типа стены
        if (isVertical) // Левая или правая стена
        {
            transform.localScale = new Vector3(thickness, cameraHeight, 1);
            // Позиционируем стену по краю экрана
            float xPos = mainCamera.transform.position.x + (cameraWidth / 2 - thickness / 2);


            if (_leftWall)
                xPos = mainCamera.transform.position.x - (cameraWidth / 2 - thickness / 2);

            transform.position = new Vector3(xPos, mainCamera.transform.position.y, 0);
        }
        else // Потолок
        {
            transform.localScale = new Vector3(cameraWidth, thickness, 1);
            // Позиционируем потолок сверху
            float yPos = mainCamera.transform.position.y + (cameraHeight / 2 - thickness / 2);
            transform.position = new Vector3(mainCamera.transform.position.x, yPos, 0);
        }
    }
}