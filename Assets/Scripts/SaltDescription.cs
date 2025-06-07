using TMPro;
using UnityEngine;

namespace Revolve
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class SaltDescription : MonoBehaviour
    {
        private TextMeshProUGUI saltBio;

        private void Awake()
        {
            saltBio = GetComponent<TextMeshProUGUI>();

#if UNITY_EDITOR
            saltBio.text = "Please supply the correct salt.txt file at \"Assets/StreamingAssets/\" and then reboot.";
#else
            saltBio.text = "Please supply the correct salt.txt file at \"Revolve_Data/StreamingAssets/\" and then reboot.";
#endif
        }
    }
}