using NUnit.Framework;
using UnityEngine;

namespace Tests
{
    public class ComponentGetComponentsExtensionsTests
    {
        private GameObject CreateGameObject()
        {
            var go = new GameObject();
            go.AddComponent<BoxCollider>();
            go.AddComponent<Rigidbody>();

            var parent = new GameObject();
            parent.AddComponent<SpriteRenderer>();
            parent.AddComponent<Rigidbody2D>();
            go.transform.SetParent(parent.transform);

            var child = new GameObject();
            child.AddComponent<SpriteRenderer>();
            child.AddComponent<Rigidbody2D>();
            child.transform.SetParent(go.transform);

            return go;
        }

        private void TestQueryEquality<T>(GetComponentQuery<T> query, T[] array) where T : class
        {
            var index = 0;
            foreach (var component in query)
            {
                Debug.Assert(array[index] == component);
                index++;
            }
        }

        [Test]
        public void GetComponentsSuccess()
        {
            var go = CreateGameObject();

            TestQueryEquality(go.transform.GetComponentsQuery<Component>(true),
                go.transform.GetComponents<Component>());
        }

        [Test]
        public void GetComponentsInChildrenSuccess()
        {
            var go = CreateGameObject();

            TestQueryEquality(go.transform.GetComponentsQuery<Component>(true),
                go.transform.GetComponents<Component>());
        }

        [Test]
        public void GetComponentsInParentSuccess()
        {
            var go = CreateGameObject();

            TestQueryEquality(go.transform.GetComponentsQuery<Component>(true),
                go.transform.GetComponents<Component>());
        }
    }
}
