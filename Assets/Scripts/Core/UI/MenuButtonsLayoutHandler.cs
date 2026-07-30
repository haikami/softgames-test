using UnityEngine;
using UnityEngine.UI;

namespace Core.UI
{
    /// <summary>
    /// Changes menu buttons layout when aspect ratio gets past a threshold
    /// </summary>
    public sealed class MenuButtonsLayoutHandler : MonoBehaviour
    {
        [SerializeField] private GridLayoutGroup _gridLayout;
        [SerializeField] private float _landscapeAspectThreshold = 1.3f;

        private bool? _isLandscape;

        private void Awake()
        {
            RefreshLayout(force: true);
        }

        private void Update()
        {
            RefreshLayout(force: false);
        }

        private void RefreshLayout(bool force)
        {
            var isLandscape = (float)Screen.width / Screen.height >= _landscapeAspectThreshold;

            if (!force && _isLandscape == isLandscape)
                return;

            _isLandscape = isLandscape;

            _gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            _gridLayout.constraintCount = isLandscape ? 2 : 1;
        }
    }
}