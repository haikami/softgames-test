using UnityEngine;

namespace Core.UI
{
    public class SpinningIcon : MonoBehaviour
    {
        [SerializeField] private float _speed = 90f;
        private void Update()
        {
            transform.Rotate(0f, 0f, - _speed * Time.deltaTime);
        }
    }
}