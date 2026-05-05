using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.Features.MainHero;
using Assets._Project.Develop.Runtime.Utilites.AudioManagement;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature
{
    public class PlungeAudioView : EntityView
    {
        [SerializeField] private string _loopKey = "AbilityImpactPlungeLoop";
        [SerializeField] private string _landKey = "AbilityImpactPlunge";
        [SerializeField] private float _maxPitch = 1.5f;

        private AudioService _audio;
        private string _activeLoopId;
        private float _time;

        protected override void OnEntityStartedWork(Entity entity)
        {
            _audio = entity.GetComponent<AudioComponent>().Service;

            entity.IsPlunging.Subscribe((_, active) => {
                if (active) StartLoop(); else StopLoop();
            });

            entity.IsGrounded.Subscribe((_, grounded) => {
                if (grounded && _time > 0.1f) PlayLand();
            });
        }

        private void Update()
        {
            if (string.IsNullOrEmpty(_activeLoopId))
                return;

            _time += Time.deltaTime;
            float pitch = Mathf.Lerp(1f, _maxPitch, _time / 0.5f);
            _audio.SetPitch(_activeLoopId, pitch);
        }

        private void StartLoop()
        {
            _time = 0;
            _activeLoopId = _audio.PlaySfxVariationLoop(_loopKey, 1, 3);
        }

        private void PlayLand()
        {
            _audio.PlaySfxVariation(_landKey, 1, 3);
            StopLoop();
        }

        private void StopLoop()
        {
            if (!string.IsNullOrEmpty(_activeLoopId)) _audio.StopSfx(_activeLoopId);
            _activeLoopId = null;
        }
    }
}