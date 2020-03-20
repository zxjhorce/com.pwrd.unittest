using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class NewTestScript
    {
        // A Test behaves as an ordinary method
        [Test]
        public void NewTestScriptSimplePasses()
        {
            // Use the Assert class to test conditions
            Assert.IsTrue(IsLargeOne());
            Debug.Log("curTime:" + Time.realtimeSinceStartup);
            int b = 0;
            for(int i = 1; i < 555555555; i++)
            {
                float a = i / 47 / 49 / 63;
                b += (int)a;
            }
            Debug.Log("after curTime:" + Time.realtimeSinceStartup);
        }

        bool IsLargeOne()
        {
            int value = 2;
            return value > 1;
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator NewTestScriptWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
    }
}
