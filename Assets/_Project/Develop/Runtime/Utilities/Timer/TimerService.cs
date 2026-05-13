using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;
using System.Collections;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Utilities.Timer
{
    public class TimerService : IDisposable
    {
        private float _cooldown;
        private ReactiveEvent _cooldownEnded;
        private ReactiveVariable<float> _currentTime;
        private ICoroutinesPerformer _coroutinesPerformer;
        private Coroutine _cooldownProcess;

        public TimerService(ICoroutinesPerformer coroutinesPerformer, float cooldown)
        {
            _coroutinesPerformer = coroutinesPerformer;
            _cooldown = cooldown;
            _cooldownEnded = new ReactiveEvent();
            _currentTime = new ReactiveVariable<float>();
        }

        public IReadOnlyEvent CooldownEnded => _cooldownEnded;
        public IReadOnlyVariable<float> CurrentTime => _currentTime;
        public bool IsOver => _currentTime.Value <= 0;

        public void Dispose() => Stop();

        public void Stop()
        {
            if (_cooldownProcess != null)
                _coroutinesPerformer.StopPerform(_cooldownProcess);

            _cooldownProcess = null;
        }

        public void Restart()
        {
            Stop();
            _cooldownProcess = _coroutinesPerformer.StartPerform(CooldownProcess());
        }

        private IEnumerator CooldownProcess()
        {
            _currentTime.Value = _cooldown;

            while (!IsOver)
            {
                _currentTime.Value -= Time.deltaTime;
                yield return null;
            }

            _cooldownEnded.Invoke();
        }
    }
}