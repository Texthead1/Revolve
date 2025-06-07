using UnityEngine;

namespace Revolve
{
    [RequireComponent(typeof(RectTransform))]
    public class HoveringText : MonoBehaviour
    {
        [SerializeField] private float amplitudeFrequency;
        [SerializeField] private float amplitudeMultiplier;

        private RectTransform rectTransform;
        private Vector3 initPosition;
        private float initTime;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        private void OnEnable()
        {
            initPosition = rectTransform.anchoredPosition;
            initTime = Time.time;
        }

        private void OnDisable()
        {
            rectTransform.anchoredPosition = initPosition;
        }

        private void Update()
        {
            float sinValue = Mathf.Sin((Time.time - initTime) * amplitudeFrequency);
            rectTransform.anchoredPosition = new Vector2(initPosition.x, initPosition.y + (sinValue * amplitudeMultiplier));
        }
    }
}