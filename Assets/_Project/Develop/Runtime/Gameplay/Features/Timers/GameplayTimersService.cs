using Assets._Project.Develop.Runtime.Utilites.Timer;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Timers
{
    public class GameplayTimersService
    {
        private readonly TimerServiceFactory _timerServiceFactory;

        public GameplayTimersService(TimerServiceFactory timerServiceFactory)
        {
            _timerServiceFactory = timerServiceFactory;
        }

        public TimerService PreperationTimer { get; private set; }

        public void CreatePreperationTimer(float time)
        {
            PreperationTimer = _timerServiceFactory.Create(time);
        }
    }
}
