using TMPro;
using UnityEngine;

namespace Revolve
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class FigureDescription : MonoBehaviour
    {
        private TextMeshProUGUI figureBio;

        private void Awake()
        {
            figureBio = GetComponent<TextMeshProUGUI>();
        }

        public void CreateAlert(string skylanderName)
        {
            figureBio.text = $"Please remove {skylanderName} from the Portal of Power.";
        }
    }
}