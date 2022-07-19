using GameWorkstore.Patterns;
using UnityEngine;


public class FakeServiceListener : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ServiceProvider.GetService<EventService>().Update.Register(OnUpdate);
    }

    private void OnUpdate()
    {
        Debug.Log("Exists!");
    }

    private void OnDestroy()
    {
        ServiceProvider.GetService<EventService>().Update.Unregister(OnUpdate);
    }
}
