using Core.Interfaces;
using UnityEngine;

namespace Core.UI
{
    public class LoadingView : MonoBehaviour, ILoadingScreen
    {
        public void Show(object caller = null)
        {
            gameObject.SetActive(true);
        }

        public void Hide(object caller = null)
        {
            gameObject.SetActive(false);
        }
    }
}