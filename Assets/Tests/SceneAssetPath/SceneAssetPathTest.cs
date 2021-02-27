using GameWorkstore.Patterns;
using UnityEngine;

public class SceneAssetPathTest : MonoBehaviour
{
    public SceneAssetPath Scene;
    public bool Enabled;

    private void OnValidate()
    {
        Scene.OnValidate();
    }
}
