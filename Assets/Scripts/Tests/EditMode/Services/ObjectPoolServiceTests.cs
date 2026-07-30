using System;
using Core.Services;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core.Tests.EditMode.Services
{
    /// <summary>
    /// ObjectPoolService tests
    /// </summary>
    public class ObjectPoolServiceTests
    {
        private ObjectPoolService _pool;
        private GameObject _prefabGameObject;
        private PoolableRect _prefab;

        [SetUp]
        public void SetUp()
        {
            _pool = new ObjectPoolService();
            _prefabGameObject = new GameObject("PoolablePrefab");
            _prefabGameObject.SetActive(false);
            _prefab = _prefabGameObject.AddComponent<PoolableRect>();
            _prefabGameObject.AddComponent<RectTransform>();
        }

        [TearDown]
        public void TearDown()
        {
            _pool.Clear<PoolableRect>();
            if (_prefabGameObject != null) Object.DestroyImmediate(_prefabGameObject);
        }

        [Test]
        public void Get_WithoutPrewarm_CreatesNewInstance_AndInvokesOnSpawned()
        {
            _pool.Register(_prefab);

            var instance = _pool.Get<PoolableRect>();

            Assert.IsNotNull(instance);
            Assert.AreEqual(1, instance.SpawnedCount);
            Assert.IsTrue(instance.gameObject.activeSelf);
        }

        [Test]
        public void Get_AfterReturn_ReusesTheSameInstance_InsteadOfCreatingANewOne()
        {
            _pool.Register(_prefab);
            var first = _pool.Get<PoolableRect>();

            _pool.Return(first);
            var second = _pool.Get<PoolableRect>();

            Assert.AreSame(first, second);
            Assert.AreEqual(2, second.SpawnedCount); // spawned once, reused once
        }

        [Test]
        public void Return_DeactivatesInstance_AndInvokesOnReturned()
        {
            _pool.Register(_prefab);
            var instance = _pool.Get<PoolableRect>();

            _pool.Return(instance);

            Assert.IsFalse(instance.gameObject.activeSelf);
            Assert.AreEqual(1, instance.ReturnedCount);
        }

        [Test]
        public void Clear_DestroysPooledInstances_AndForgetsRegistration()
        {
            _pool.Register(_prefab, prewarmCount: 2);

            _pool.Clear<PoolableRect>();

            Assert.Throws<InvalidOperationException>(() => _pool.Get<PoolableRect>());
        }

        [Test]
        public void Return_ThenGet_KeepsLocalScaleAtOne_RegardlessOfPreviousParentScale()
        {
            // Regression test for the "cards get bigger on every Reset" bug:
            // ObjectPoolService.Return() used to call transform.SetParent(null) with
            // worldPositionStays defaulting to true. 
            _pool.Register(_prefab);

            var scaledParent = new GameObject("ScaledStackRoot", typeof(RectTransform))
                .GetComponent<RectTransform>();
            scaledParent.localScale = new Vector3(2f, 2f, 2f); // simulates a canvas-scaled stack root

            var instance = _pool.Get<PoolableRect>(scaledParent);
            instance.Rect.localScale = Vector3.one; // as PushTop leaves it

            _pool.Return(instance);
            var reused = _pool.Get<PoolableRect>(); // re-added to a fresh, unscaled parent

            Assert.AreEqual(Vector3.one, reused.Rect.localScale);

            Object.DestroyImmediate(scaledParent.gameObject);
        }
    }
}
