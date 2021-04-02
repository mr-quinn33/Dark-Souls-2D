using Assets.Scripts.Characters;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    [RequireComponent(typeof(Slider))]
    internal abstract class StatsBar : MonoBehaviour
    {
        public Character character;

        private protected Slider slider;

        private protected virtual void Start()
        {
            if (!character)
            {
                throw new NullReferenceException($"No character reference set in {this}!");
            }
            slider = GetComponent<Slider>();
        }

        private protected void SetMaxValue(float maxValue)
        {
            if (slider)
            {
                slider.maxValue = maxValue;
            }
        }

        private protected void SetMaxValue(int maxValue)
        {
            if (slider)
            {
                if (!slider.wholeNumbers)
                {
                    slider.wholeNumbers = true;
                }
                slider.maxValue = maxValue;
            }
        }

        private protected void SetValue(float value)
        {
            if (slider)
            {
                slider.value = value;
            }
        }

        private protected void SetValue(int value)
        {
            if (slider)
            {
                if (!slider.wholeNumbers)
                {
                    slider.wholeNumbers = true;
                }
                slider.value = value;
            }
        }
    }
}