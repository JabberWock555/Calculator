using System;
using System.Collections.Generic;
using UnityEngine;

namespace JabberWockStudio.Calculator
{
    public class Calculator : MonoBehaviour
    {
        [SerializeField]
        private TMPro.TextMeshProUGUI displayText;
        [SerializeField]
        private TMPro.TextMeshProUGUI timeText;

        private string currentExpression = "", currentTime = "";
        private bool isNewCalculation = true;
        private AudioSource audioSource;

        public static Action<ButtonType, string> ButtonClicked;

        private void Start()
        {
            ButtonClicked += OnButtonClicked;
            audioSource = GetComponent<AudioSource>();
        }

        private void Update()
        {
            if (timeText != null)
            {
                currentTime = DateTime.Now.ToString("HH:mm");
                timeText.text = currentTime;
            }
        }

        private void OnButtonClicked(ButtonType type, string value)
        {
            audioSource.Play();
            switch (type)
            {
                case ButtonType.Digit:
                    AddDigit(value);
                    break;
                case ButtonType.Operator:
                    AddOperator(value);
                    break;
                case ButtonType.Equal:
                    Calculate();
                    break;
                case ButtonType.Clear:
                    Clear();
                    break;
            }
        }

        private void AddDigit(string digit)
        {
            if (isNewCalculation)
            {
                currentExpression = "";
                isNewCalculation = false;
            }
            currentExpression += digit;
            UpdateDisplay();
        }

        private void AddOperator(string op)
        {
            if (currentExpression.Length > 0 && !IsOperator(currentExpression[currentExpression.Length - 1]))
            {
                currentExpression += op;
                isNewCalculation = false;
                UpdateDisplay();
            }
        }

        public void DeleteLastCharacter()
        {
            if (currentExpression.Length > 0)
            {
                currentExpression = currentExpression.Substring(0, currentExpression.Length - 1);
                UpdateDisplay();

                if (currentExpression.Length == 0)
                {
                    Clear();
                }
            }
        }

        public void Clear()
        {
            currentExpression = "";
            UpdateDisplay();
        }

        public void Calculate()
        {
            if (currentExpression.Length > 0)
            {
                if (currentExpression.Length > 0)
                {
                    if (IsOperator(currentExpression[currentExpression.Length - 1]))
                    {
                        displayText.text = "Invalid Expression";
                        return;
                    }

                    float result = EvaluateExpression(currentExpression);
                    if (float.IsNaN(result))
                    {
                        displayText.text = "Error";
                    }
                    else
                    {
                        currentExpression = result.ToString();
                        UpdateDisplay();
                    }
                    isNewCalculation = true;
                }
            }
        }

        private float EvaluateExpression(string expression)
        {
            List<char> operators = new List<char>();
            List<float> numbers = new List<float>();

            string currentNumber = "";
            for (int i = 0; i < expression.Length; i++)
            {
                if (IsOperator(expression[i]))
                {
                    if (currentNumber != "")
                    {
                        numbers.Add(float.Parse(currentNumber));
                        currentNumber = "";
                    }
                    operators.Add(expression[i]);
                }
                else
                {
                    currentNumber += expression[i];
                }
            }
            if (currentNumber != "")
            {
                numbers.Add(float.Parse(currentNumber));
            }

            Handle_MultiplyDivide(operators, numbers);

            float finalResult = Handle_AddSub(operators, numbers);

            return finalResult;
        }

        private void Handle_MultiplyDivide(List<char> operators, List<float> numbers)
        {
            for (int i = 0; i < operators.Count; i++)
            {
                if (operators[i] == '*' || operators[i] == '/')
                {
                    float result;
                    if (operators[i] == '*')
                    {
                        result = numbers[i] * numbers[i + 1];
                    }
                    else
                    {
                        if (numbers[i + 1] == 0)
                        {
                            return /* float.NaN */;
                        }
                        result = numbers[i] / numbers[i + 1];
                    }

                    numbers[i] = result;
                    numbers.RemoveAt(i + 1);
                    operators.RemoveAt(i);
                    i--;
                }
            }
        }

        private float Handle_AddSub(List<char> operators, List<float> numbers)
        {
            float finalResult = numbers[0];
            for (int i = 0; i < operators.Count; i++)
            {
                if (operators[i] == '+')
                {
                    finalResult += numbers[i + 1];
                }
                else if (operators[i] == '-')
                {
                    finalResult -= numbers[i + 1];
                }
            }

            return finalResult;
        }

        private bool IsOperator(char c)
        {
            return c == '+' || c == '-' || c == '*' || c == '/';
        }

        private void UpdateDisplay()
        {
            displayText.text = currentExpression;
        }

        private void OnDestroy()
        {
            ButtonClicked -= OnButtonClicked;
        }
    }

}