using Cysharp.Threading.Tasks;
using UnityEngine;

public class PlayFieldSpawner : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private Transform _container;

    [Header("Prefabs")] [SerializeField] private GameObject _wallPrefab;
    [SerializeField] private GameObject _ceilingPrefab;
    [SerializeField] private GameObject _backgroundPrefab;

    [Header("Sizes")] [SerializeField] private float _wallThickness = 0.5f;
    [SerializeField] private float _ceilingThickness = 0.3f;
    [SerializeField] private float _floorThickness = 0.5f;
    
    private Vector3 CamPos => _camera.transform.position;

    public async UniTask Init(float camHeight, float camWidth)
    {
        SpawnBackground(camHeight, camWidth);
        
        Transform ceiling = SpawnWall("Ceiling",
            new Vector3(CamPos.x, CamPos.y + (camHeight / 2f - _ceilingThickness / 2f), 0),
            new Vector3(camWidth, _ceilingThickness, 1f), _ceilingPrefab);
        Transform leftWall = SpawnWall("LeftWall",
            new Vector3(CamPos.x - (camWidth / 2f - _wallThickness / 2f), CamPos.y, 0),
            new Vector3(_wallThickness, camHeight, 1f));
        Transform rightWall = SpawnWall("RightWall",
            new Vector3(CamPos.x + (camWidth / 2f - _wallThickness / 2f), CamPos.y, 0),
            new Vector3(_wallThickness, camHeight, 1f));
    }

    private void SpawnBackground(float camHeight, float camWidth)
    {
        SpawnWall("Background", new Vector3(CamPos.x, CamPos.y, 10f), new Vector3(camWidth, camHeight, 1f),
            _backgroundPrefab);
    }

    private Transform SpawnWall(string name, Vector3 position, Vector3 scale, GameObject prefab = null)
    {
        if (prefab == null) prefab = _wallPrefab;
        if (prefab == null) return null;

        GameObject obj = Instantiate(prefab, _container);
        obj.name = name;
        obj.transform.position = position;
        obj.transform.localScale = scale;
        obj.transform.SetParent(_container, true);
        
        if (obj.transform.childCount > 0)
            obj.transform.GetChild(0).localScale = Vector3.one;

        return obj.transform;
    }
}