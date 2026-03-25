using UnityEngine;

public class BubbleFieldGenerator : MonoBehaviour
{
    public GameObject bubblePrefab; // Префаб шарика
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
    }
}