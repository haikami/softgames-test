using Core;
using Core.Interfaces;
using UnityEngine;

namespace Features.PhoenixFlame.Controllers
{
    public class PhoenixFlameController : MonoBehaviour
    {
        private ITopBarView _topBarView;
        
        private void Awake()
        {
            _topBarView = ServiceLocator.Get<ITopBarView>();
        }

        private void Start()
        {
            _topBarView.SetButtonsVisibility(true);
        }
    }
}