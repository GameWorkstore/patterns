using GameWorkstore.Patterns;
using System;
using UnityEngine;
using UnityEngine.UI;


public class TemplatePoolSample : MonoBehaviour
{
    public Text _number;

    public void SetData(int number)
    {
        _number.text = number.ToString();
    }
}

[Serializable]
public class TemplatePoolSampleList : TemplatePool<TemplatePoolSample> { }
