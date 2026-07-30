using Core.Interfaces;
using UnityEngine;

namespace Core.UI
{
    /// <summary>
    /// Simple loading view implementation, might track callers and improve logic.
    /// </summary>
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