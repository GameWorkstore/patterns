using UnityEngine;

namespace GameWorkstore.Patterns.Tests
{
    public class HighSpeedArraySerializationTest : MonoBehaviour
    {
        public HighSpeedArray<int> IntArray = new HighSpeedArray<int>(1);

        public void Awake()
        {
            foreach(int integer in IntArray)
            {
                Debug.Log(integer);
            }
        }
    }

    public class TestNewFunctions : MonoBehaviour
	{
        public void NewFunction()
		{
            //TODO:
		}
	}
}