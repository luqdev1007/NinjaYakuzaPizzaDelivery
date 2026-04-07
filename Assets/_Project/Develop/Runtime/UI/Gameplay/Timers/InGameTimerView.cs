using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.Timers
{
    public class InGameTimerView : MonoBehaviour
    {
        [field: SerializeField] public CanvasGroup Group { get; private set; }
        [field: SerializeField] public TextMeshProUGUI TimerText { get; private set; }
        [field: SerializeField] public Image FilledCircle { get; private set; }
        [field: SerializeField] public Image BackgroundCircle { get; private set; }

        public void SetText(string text) => TimerText.text = text;

        public void SetProgress(float progress) => FilledCircle.fillAmount = progress;

        public void UpdateColors(Color textMin, Color textMax, Color bg)
        {
            TimerText.colorGradient = new VertexGradient(textMax, textMax, textMin, textMin);
            BackgroundCircle.color = bg;
        }
    }
}

