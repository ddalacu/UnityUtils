using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace Tests
{
    public class ListUtilitiesTests
    {
        [Test]
        public void ListUtilitiesCreateFromBufferSuccess()
        {
            var buffer = new[]
            {
                1,2,3,5,6
            };

            var result = ListUtilities.CreateFromBuffer(buffer);

            Debug.Assert(result.Count == buffer.Length);

            for (int i = 0; i < buffer.Length; i++)
                Debug.Assert(result[i] == buffer[i]);
        }

        [Test]
        public void ListUtilitiesExtractBufferSuccess()
        {
            var buffer = new[]
            {
                1,2,3,5,6
            };

            var result = ListUtilities.CreateFromBuffer(buffer);
            var extractBuffer = ListUtilities.ExtractBuffer(result);
            Debug.Assert(buffer == extractBuffer);
        }

        [Test]
        public void ListUtilitiesExtractBufferSuccess2()
        {
            var buffer = new[]
            {
                1,2,3,5,6
            };

            var result = new List<int>();
            ListUtilities.UseBuffer(result,buffer);

            Debug.Assert(result.Count == buffer.Length);

            for (int i = 0; i < buffer.Length; i++)
                Debug.Assert(result[i] == buffer[i]);

            var extractBuffer = ListUtilities.ExtractBuffer(result);
            Debug.Assert(buffer == extractBuffer);
        }
    }
}
