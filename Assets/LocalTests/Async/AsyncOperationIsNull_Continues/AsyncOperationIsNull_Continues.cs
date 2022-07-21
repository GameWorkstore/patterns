using GameWorkstore.Patterns;
using UnityEngine;

public class AsyncOperationIsNull_Continues : MonoBehaviour
{
    // Start is called before the first frame update
    private async void Start()
    {
        AsyncOperation op = null;
        await Await.AsyncOp(op);
        Debug.Log("AsyncOperationIsNull_Continues");
    }
}
