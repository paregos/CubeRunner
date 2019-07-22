using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class SettingsController : MonoBehaviour
    {

        public void SetRandomColorFloorBlockFlag(Toggle toggle)
        {
            GameConstants.setRandomColorBlocks = toggle.isOn;
        }

        public void SetSwitchAreaValue(Slider slider)
        {
            GameConstants.switchAreaValue = slider.value;
        }

        public void SetHighLightValidPathh(Toggle toggle)
        {
            GameConstants.highlightValidPath = toggle.isOn;
        }
    }
}