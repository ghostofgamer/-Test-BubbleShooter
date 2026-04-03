using System;
using UnityEngine;

public class BubbleFieldGenerator : MonoBehaviour
{
    [Header("Настройки шаров")] [SerializeField]
    private GameObject bubblePrefab; // Один префаб для всех шаров
    [SerializeField] private float _spawnOffsetX = 0f;
    [SerializeField] private float _spawnOffsetY = 0f;
    
    [Header("Границы поля")] public Transform _leftWall;
    public Transform _rightWall;
    public Transform _ceiling;

    [Header("Файл уровня")] [SerializeField]
    private TextAsset levelFile;

    private float leftX, rightX, topY;

    private float bubbleSize;
    /*private void Start()
    {
        // Вычисляем границы игрового поля
        leftX = _leftWall.position.x + _leftWall.localScale.x / 2f;
        rightX = _rightWall.position.x - _rightWall.localScale.x / 2f;
        topY = _ceiling.position.y - _ceiling.localScale.y / 2f;

        if (levelFile != null && bubblePrefab != null)
            SpawnBubblesFromFile(levelFile);
    }*/

    private void CalculateBubbleSize()
    {
        SpriteRenderer sr = bubblePrefab.GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            bubbleSize = sr.bounds.size.x; // ВСЁ: спрайт уже учитывает масштаб объекта
            // Для квадратных шаров можно использовать sr.bounds.size.y, если нужно
        }
    }

    public void Init(Transform leftWall, Transform rightWall, Transform ceiling)
    {
        _leftWall = leftWall;
        _rightWall = rightWall;
        _ceiling = ceiling;
        CalculateBubbleSize();

        leftX = _leftWall.position.x + _leftWall.localScale.x / 2f;
        rightX = _rightWall.position.x - _rightWall.localScale.x / 2f;
        topY = _ceiling.position.y - _ceiling.localScale.y / 2f;


        float bubbleDiameter = 0f;
        
        SpriteRenderer sr = bubblePrefab.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            float bubbleWidth = sr.bounds.size.x; // ширина шара в мировых единицах
            float bubbleHeight = sr.bounds.size.y; // высота шара
            bubbleDiameter = Mathf.Max(bubbleWidth, bubbleHeight);

            Debug.Log($"Игровой размер шара: {bubbleDiameter}");
        }

        float leftEdge = leftWall.position.x + leftWall.localScale.x / 2f;
        float rightEdge = rightWall.position.x - rightWall.localScale.x / 2f;
        float fieldWidth = rightEdge - leftEdge;

        int maxBubblesPerRow = Mathf.FloorToInt(fieldWidth / bubbleDiameter);

        Debug.Log($"Максимум шаров в ряду: {maxBubblesPerRow}");

        /*if (levelFile != null && bubblePrefab != null)
            SpawnBubblesFromFile(levelFile, maxBubblesPerRow);*/

        VisualizeTestBubbleGrid(6);
    }

    public void VisualizeTestBubbleGrid(int numRows)
    {
        if (bubblePrefab == null)
        {
            Debug.LogWarning("BubblePrefab не назначен!");
            return;
        }

        float bubbleDiameter = bubbleSize;

        // Вычисляем ширину игрового поля
        float fieldWidth = rightX - leftX;
        int maxBubblesPerRow = Mathf.FloorToInt(fieldWidth / bubbleDiameter);
        Debug.Log($"Максимальное количество шаров в ряду: {maxBubblesPerRow}");

        float startY = topY - bubbleDiameter / 2f;

        for (int row = 0; row < numRows; row++)
        {
            float rowWidth = maxBubblesPerRow * bubbleDiameter;
            float offsetX = leftX + (rightX - leftX - rowWidth) / 2f+_spawnOffsetX;

            // Шахматный офсет: нечетные ряды смещаем на половину диаметра
            float rowOffset = (row % 2 == 0) ? 0f : bubbleDiameter / 2f;

            for (int col = 0; col < maxBubblesPerRow; col++)
            {
                /*Vector3 pos = new Vector3(
                    offsetX + col * bubbleDiameter + rowOffset,
                    startY - row * bubbleDiameter * 0.87f,
                    0
                );*/
                
                Vector3 pos = new Vector3(
                    offsetX + col * bubbleDiameter + rowOffset,
                    startY - row * bubbleDiameter * 0.87f - _spawnOffsetY,
                    0
                );

                // Создаем тестовую точку
                GameObject debugDot = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                debugDot.transform.position = pos;
                debugDot.transform.localScale = Vector3.one * bubbleDiameter; // диаметр = шар
                debugDot.name = $"DebugPos_Row{row}_Col{col}";

                // Цвет точки
                Renderer rend = debugDot.GetComponent<Renderer>();
                rend.material = new Material(Shader.Find("Standard"));
                rend.material.color = Color.gray;
            }
        }
    }

    /*private void SpawnBubblesFromFile(TextAsset file, int maxBubblesPerRow)
    {
        if (file == null)
        {
            Debug.LogWarning("Файл уровня не назначен!");
            return;
        }

        string[] lines = file.text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        Debug.Log($"Количество рядов в файле: {lines.Length}");

        float bubbleDiameter = bubbleSize;
        float startY = topY - bubbleDiameter / 2f;

        for (int row = 0; row < lines.Length; row++)
        {
            string line = lines[row].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            // Ограничиваем по максимуму
            int totalPositions = Mathf.Min(line.Length, maxBubblesPerRow);

            // Вычисляем ширину ряда и смещение для центрирования
            float rowWidth = totalPositions * bubbleDiameter;
            float offsetX = leftX + (rightX - leftX - rowWidth) / 2f;

            // Шахматный офсет: нечетные ряды смещаем на половину диаметра
            float rowOffset = (row % 2 == 0) ? 0f : bubbleDiameter / 2f;

            for (int col = 0; col < totalPositions; col++)
            {
                char c = line[col];

                // Вычисляем позицию для точки/шара
                Vector3 pos = new Vector3(
                    offsetX + col * bubbleDiameter + rowOffset,
                    startY - row * bubbleDiameter * 0.87f, // 0.87 ~ sqrt(3)/2 для треугольного офсета
                    0
                );


                /*if (c == ' ')
                {
                    Debug.Log("!!!!!!!!!!!!!!!!!");
                    GameObject bubble = Instantiate(bubblePrefab, pos, Quaternion.identity, transform);
                    bubble.name = $"Bubble_Row{row}_Col{col}";
                    bubble.transform.position = pos;
                    // bubble.transform.localScale = Vector3.one * bubbleDiameter * 0.2f;
                }

                // Визуализация: создаем маленькую точку
                GameObject debugDot = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                debugDot.transform.position = pos;
                debugDot.transform.localScale = Vector3.one * bubbleDiameter * 0.2f;
                debugDot.name = $"DebugPos_Row{row}_Col{col}";

                // Цвет точки: зеленый = шар, серый = пустое место
                Renderer rend = debugDot.GetComponent<Renderer>();
                rend.material = new Material(Shader.Find("Standard"));
                rend.material.color = (c == '.' || c == ' ') ? Color.gray : Color.green;#1#

                // Пропускаем пустые позиции
                if (c == '.' || c == ' ')
                {
                    Debug.Log($"Ряд {row + 1}, позиция {col + 1}: пусто (символ '{c}')");
                    continue;
                }

                // Спавним настоящий шар
                GameObject bubble = Instantiate(bubblePrefab, pos, Quaternion.identity, transform);
                bubble.name = $"Bubble_Row{row}_Col{col}";

                // Дебаг
                if (c == '.' || c == ' ')
                    Debug.Log($"Ряд {row + 1}, позиция {col + 1}: пусто (символ '{c}')");
                else
                    Debug.Log($"Ряд {row + 1}, позиция {col + 1}: спавним шар (символ '{c}')");
            }
        }
    }*/

    /*
    private void SpawnBubblesFromFile(TextAsset file, int maxBubblesPerRow)
    {
        if (file == null || bubblePrefab == null)
        {
            Debug.LogWarning("Файл уровня или префаб шара не назначен!");
            return;
        }

        string[] lines = file.text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        Debug.Log($"Количество рядов в файле: {lines.Length}");

        float bubbleDiameter = bubbleSize;
        float startY = topY - bubbleDiameter / 2f;

        for (int row = 0; row < lines.Length; row++)
        {
            string line = lines[row].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            // Ограничиваем по максимуму
            int totalPositions = Mathf.Min(line.Length, maxBubblesPerRow);

            // Центрирование ряда
            float rowWidth = totalPositions * bubbleDiameter;
            float offsetX = leftX + (rightX - leftX - rowWidth) / 2f;

            // Шахматный офсет для нечетных рядов
            float rowOffset = (row % 2 == 0) ? 0f : bubbleDiameter / 2f;

            for (int col = 0; col < totalPositions; col++)
            {
                char c = line[col];

                // Пропускаем пустые позиции
                if (c == '.' || c == ' ')
                {
                    Debug.Log($"Ряд {row + 1}, позиция {col + 1}: пусто (символ '{c}')");
                    continue;
                }

                // Вычисляем позицию для шара
                Vector3 pos = new Vector3(
                    offsetX + col * bubbleDiameter + rowOffset,
                    startY - row * bubbleDiameter * 0.87f, // вертикальный офсет
                    0
                );

                // Создаем шар
                GameObject bubble = Instantiate(bubblePrefab, pos, Quaternion.identity, transform);
                bubble.name = $"Bubble_Row{row}_Col{col}";

                // Присваиваем цвет шара, если есть функция TryGetColorFromChar
                if (TryGetColorFromChar(c, out Color color))
                {
                    SetBubbleColor(bubble, color);
                }

                Debug.Log($"Ряд {row + 1}, позиция {col + 1}: спавним шар (символ '{c}')");
            }
        }
    }
    */

    private bool TryGetColorFromChar(char c, out Color color)
    {
        switch (c)
        {
            case 'R':
                color = Color.red;
                return true;
            case 'G':
                color = Color.green;
                return true;
            case 'B':
                color = Color.blue;
                return true;
            case 'Y':
                color = Color.yellow;
                return true;
            default:
                color = Color.white;
                return false; // пропускаем неизвестные
        }
    }

    private void SetBubbleColor(GameObject bubble, Color color)
    {
        SpriteRenderer sr = bubble.GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
            sr.color = color;
    }

/*public GameObject bubblePrefab; // Префаб шарика
public TextAsset fieldData; // Текстовый файл с описанием поля
public float bubbleSize = 1f;

void Start()
{
    GenerateField();
}

void GenerateField()
{
    string[] lines = fieldData.text.Split('\n');
    for (int y = 0; y < lines.Length; y++)
    {
        string line = lines[y].Trim();
        for (int x = 0; x < line.Length; x++)
        {
            char bubbleType = line[x];
            if (bubbleType != '.')
            {
                Vector3 position = CalculatePosition(x, y);
                GameObject bubble = Instantiate(bubblePrefab, position, Quaternion.identity);
                // Настрой цвет шарика в зависимости от bubbleType
                // SetBubbleColor(bubble, bubbleType);
            }
        }
    }
}

Vector3 CalculatePosition(int x, int y)
{
    // Логика расчёта позиции для гексагональной сетки
    float offsetX = (y % 2 == 0) ? 0 : bubbleSize / 2f;
    float posX = x * bubbleSize + offsetX;
    float posY = -y * bubbleSize * 0.866f; // 0.866 ≈ sin(60°)
    return new Vector3(posX, posY, 0);
}

void SetBubbleColor(GameObject bubble, char bubbleType)
{
    SpriteRenderer renderer = bubble.GetComponent<SpriteRenderer>();

    switch (bubbleType)
    {
        case 'R':
            renderer.color = Color.red;
            break;
        case 'G':
            renderer.color = Color.green;
            break;
        case 'B':
            renderer.color = Color.blue;
            break;
    }
}*/
}