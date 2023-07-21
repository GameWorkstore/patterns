using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameWorkstore.Patterns
{
    public class Gate : CustomYieldInstruction
    {
        private bool _released;
        
        public override bool keepWaiting => !_released;
        
        public void Release()
        {
            _released = true;
        }
    }
}
