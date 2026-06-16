using UnityEngine;

/// <summary>
/// MapGeneratorModel の実行をUnity側から指示する薄いラッパー
/// </summary>
public class MapGenerator : MonoBehaviour
{
    [SerializeField] private int _targetRoomCount = 20;
    public int TargetRoomCount => _targetRoomCount;

    public GeneratedLayout Generate()
    {
        GeneratedLayout layout = TryGenerate();
        while (layout.RoomCount < _targetRoomCount) { layout = TryGenerate(); }
        return layout;
    }
    private GeneratedLayout TryGenerate()
    {
        MapGeneratorModel model = new MapGeneratorModel(_targetRoomCount);
        return model.Generate(RoomDatabase.Instance);
    }
}