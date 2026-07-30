using System.Collections.Generic;
using UnityEngine;

namespace Features.PhoenixFlame.Controllers
{
    /// <summary>
    /// Standalone flame prefab containing animation logic
    /// </summary>
    public class FlameController : MonoBehaviour
    {
        [SerializeField] private Animator _flameAnimator;

        private IReadOnlyList<string> _colorStateNames;
        private float _transitionDuration;
        private ParticleSystem[] _particleSystems;
        private int _currentColorIndex;

        public string CurrentColorState => _colorStateNames[_currentColorIndex];
        public string NextColorState => _colorStateNames[NextIndex()];


        private void Awake()
        {
            _particleSystems = GetComponentsInChildren<ParticleSystem>(includeInactive: true);
        }

        /// Must be called once before use — colors/timing come from outside
        public bool Initialize(IReadOnlyList<string> colorStateNames, float transitionDuration)
        {
            _colorStateNames = colorStateNames;
            _transitionDuration = transitionDuration;
            return ValidateColorStates();
        }

        public void Play()
        {
            PlayAllParticles();
            SnapToColor(0);
        }

        /// <summary>
        /// Uses animator CrossFade method to smoothly move
        /// from current state to the next
        /// _transitionDuration is taken from config
        /// </summary>
        public void AdvanceColor()
        {
            _currentColorIndex = NextIndex();
            _flameAnimator.CrossFade(CurrentColorState, _transitionDuration);
        }

        public void ResetFlame()
        {
            StopAllParticles();
            PlayAllParticles();
            SnapToColor(0);
        }
        
        private int NextIndex() => (_currentColorIndex + 1) % _colorStateNames.Count;

        private void SnapToColor(int index)
        {
            _currentColorIndex = index;
            _flameAnimator.Play(_colorStateNames[index], layer: 0, normalizedTime: 0f);
        }

        private void PlayAllParticles()
        {
            foreach (var ps in _particleSystems) ps.Play(withChildren: true);
        }

        private void StopAllParticles()
        {
            foreach (var ps in _particleSystems)
                ps.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        /// <summary>
        /// Check in case color states dont match with the ones from animator
        /// </summary>
        /// <returns></returns>
        private bool ValidateColorStates()
        {
            var allValid = true;
            foreach (var stateName in _colorStateNames)
            {
                if (_flameAnimator.HasState(0, Animator.StringToHash(stateName))) continue;
                
                allValid = false;
                Debug.LogError($"FlameController: Animator has no state '{stateName}' on layer 0.");
            }
            
            return allValid;
        }

        private void OnDestroy() => StopAllParticles();
    }
}