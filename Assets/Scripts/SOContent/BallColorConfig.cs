using UnityEngine;

namespace SOContent
{
    [CreateAssetMenu(fileName = "BallColorConfig", menuName = "SOContent/BallColorConfig")]
    public class BallColorConfig : ScriptableObject
    {
        [SerializeField] private BallColor[] colors;
        
        public Color GetColor(string type)
        {
            foreach (var bc in colors)
            {
                if (bc.Type.ToUpper() == type.ToUpper())
                    return bc.Color;
            }
            
            return Color.white;
        }
    }
    
    [System.Serializable]
    public struct BallColor
    {
        public string Type;
        public Color Color; 
    }
}