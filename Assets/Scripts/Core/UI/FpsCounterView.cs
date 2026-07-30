using TMPro;
using UnityEngine;

namespace Core.UI
{
    /// <summary>
    /// Displays relevant frame metrics.
    /// Used one single text field for simplicity.
    /// Also, this component will always be displayed so for this test there is no logic to show/hide
    /// </summary>
    public class FpsCounterView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _displayText;

        private const float UpdateInterval = 1f;

        private float _elapsed;
        private int _frameCount;
        private float _worstFrameTimeMs;

        private void Update()
        {
            var deltaTime = Time.unscaledDeltaTime;
            var frameTimeMs = deltaTime * 1000f;

            _elapsed += deltaTime;
            _frameCount++;

            if (frameTimeMs > _worstFrameTimeMs)
            {
                _worstFrameTimeMs = frameTimeMs;
            }

            if (_elapsed >= UpdateInterval)
            {
                UpdateCounters();
                _elapsed = 0f;
                _frameCount = 0;
                _worstFrameTimeMs = 0f;
            }
        }

        private void UpdateCounters()
        {
            var averageFps = _frameCount / _elapsed;
            var averageFrameTimeMs = _elapsed / _frameCount * 1000f;

            _displayText.text = $"FPS: {averageFps:0}" +
                                $"\nAvg: {averageFrameTimeMs:0.0} ms" + 
                                $"\nWorst: {_worstFrameTimeMs:0.0} ms";
        }
    }
}