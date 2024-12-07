using System.Collections;
using UnityEngine;

namespace JabberWockStudio.Calculator
{
    public enum ButtonType
    {
        Digit,
        Operator,
        Equal,
        Clear,
    }

    public class Button : MonoBehaviour
    {
        [SerializeField] private string buttonName;
        [SerializeField] private ButtonType buttonType;
        private float pressDistance = 0.2f;
        private float pressDuration = .05f;
        private Vector3 originalPosition;
        private Material textMat;

        private void Start()
        {
            originalPosition = transform.localPosition;
            textMat = transform.GetChild(0).GetComponent<Renderer>().material;
        }

        public void OnButtonClicked()
        {
            StartCoroutine(ButtonPress());
            //Debug.Log("Button Pressed");
        }

        private IEnumerator ButtonPress()
        {
            Vector3 pressedPosition = originalPosition - transform.right * pressDistance;
            float elapsedTime = 0f;
            textMat.color = Color.yellow;

            while (elapsedTime < pressDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / pressDuration;
                transform.localPosition = Vector3.Lerp(originalPosition, pressedPosition, t);
                yield return null;
            }

            Calculator.ButtonClicked?.Invoke(buttonType, buttonName);
            yield return new WaitForSeconds(0.05f);

            elapsedTime = 0f;
            while (elapsedTime < pressDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / pressDuration;
                transform.localPosition = Vector3.Lerp(pressedPosition, originalPosition, t);
                yield return null;
            }
            textMat.color = Color.white;

            transform.localPosition = originalPosition;


        }
    }

}