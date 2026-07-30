using System;
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
            _topBarView.OnResetButtonPressed += Reset;
        }

        private void Start()
        {
            _topBarView.SetButtonsVisibility(true, true);
        }

        private void Reset()
        {
            
        }

        private void OnDestroy()
        {
            if (_topBarView != null)
            {
                _topBarView.OnResetButtonPressed -= Reset;
            }
        }
    }
}