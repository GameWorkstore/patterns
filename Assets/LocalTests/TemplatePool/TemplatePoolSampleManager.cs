using UnityEngine;


public class TemplatePoolSampleManager : MonoBehaviour
{
    [SerializeField] TemplatePoolSampleList TemplatePoolSampleList;
    [SerializeField] int _currentValue = 0;

    public void Awake()
    {
        TemplatePoolSampleList.EnsureTemplateDisabled();
    }

    public void Update()
    {
        TemplatePoolSampleList.SetActiveCount(_currentValue);
        for (int i = 0; i < TemplatePoolSampleList.Count; i++)
        {
            TemplatePoolSampleList[i].SetData(i);
        }
    }
}
