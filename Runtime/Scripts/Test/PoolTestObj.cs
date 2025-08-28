using System;
using UnityEngine;

namespace SanderSaveli.UDK
{
    public class PoolTestObj : MonoBehaviour, IPoolableObject<PoolTestObj>
    {
        public Action<PoolTestObj> OnBackToPool { get; set; }
        private void Update()
        {
            if (transform.position.y < -5)
            {
                OnBackToPool(this);
            }
            transform.position += new Vector3(0, -2) * Time.deltaTime;
        }
        public void OnActive()
        {
            Vector3 pos = Vector3.zero;
            pos.x = UnityEngine.Random.Range(-5, 5);
            pos.y = 8;
            transform.position = pos;
        }
    }
}
