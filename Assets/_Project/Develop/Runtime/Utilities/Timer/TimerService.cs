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

        /// <summary>
        /// Перезапуск с новой длительностью. Добавлено для per-instance разброса фаз
        /// у призраков: длительность там своя на каждый заход в фазу, а cooldown
        /// задавался только конструктором и после этого был неизменяем.
        ///
        /// Перегрузка аддитивна: Restart() без аргумента ведёт себя ровно как раньше,
        /// поэтому существующие потребители (уровневый таймер через
        /// InGameTimerPresenter) не затронуты.
        /// </summary>
        public void Restart(float cooldown)
        {
            _cooldown = cooldown;
            Restart();
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
