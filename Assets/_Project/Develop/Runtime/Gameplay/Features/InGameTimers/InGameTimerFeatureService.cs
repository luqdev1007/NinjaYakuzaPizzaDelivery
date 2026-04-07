using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.InGameTimers
{
    public class InGameTimerFeatureService
    {
        public event Action OnTimerShowRequested;
        public event Action OnTimerHideRequested;

        // Методы, которые будет вызывать Стейт
        public void Show() => OnTimerShowRequested?.Invoke();
        public void Hide() => OnTimerHideRequested?.Invoke();
    }
}
