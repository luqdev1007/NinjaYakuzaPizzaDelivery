using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Enemies
{
    public class AnimatorRandomOffsetView : EntityView
    {
        [SerializeField] private Animator _animator;

        [Tooltip("Названия состояний, которым нужно рандомизировать старт. " +
                 "Оставь пустым — применится ко всем слоям по текущему состоянию.")]
        [SerializeField] private string[] _stateNames;

        protected override void OnEntityStartedWork(Entity entity)
        {
            if (_animator == null) return;

            _animator.Update(0f); // форсим инициализацию аниматора

            if (_stateNames == null || _stateNames.Length == 0)
            {
                // Просто сдвигаем все слои по текущему состоянию
                for (int i = 0; i < _animator.layerCount; i++)
                {
                    AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(i);
                    _animator.Play(stateInfo.fullPathHash, i, Random.value);
                }
            }
            else
            {
                foreach (string stateName in _stateNames)
                    _animator.Play(stateName, 0, Random.value);
            }
        }
    }
}