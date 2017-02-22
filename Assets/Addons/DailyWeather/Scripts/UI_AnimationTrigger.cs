using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DailyWeather
{

    public class UI_AnimationTrigger : MonoBehaviour
    {
        [SerializeField]
        Text seasonText;

        [SerializeField]
        Text temperatureText;

        [SerializeField]
        string tempPattern = "Temp: {0} °C";

        [SerializeField]
        string disactivationTrigger = "no_report";

        [SerializeField, HideInInspector]
        string[] triggers;

        Animator anim;

        DailyWeather dw;

        private void Start()
        {
            anim = GetComponent<Animator>();
            if (!string.IsNullOrEmpty(disactivationTrigger))
            {
                anim.SetTrigger(disactivationTrigger);
            }
        }

        private void OnEnable()
        {
            dw = DailyWeather.instance;
            dw.OnWeatherReport += Instance_OnWeatherReport;
        }

        private void OnDisable()
        {
            dw.OnWeatherReport -= Instance_OnWeatherReport;
        }

        private void Instance_OnWeatherReport(int dayOfYear, Seasons season, Weathers weather, float temperature)
        {
            string t = triggers[(int)weather];
            anim.SetTrigger(t);
            if (temperatureText)
            {
                temperatureText.text = string.Format(tempPattern, temperature);
            }
            if (seasonText)
            {
                seasonText.text = System.Enum.GetName(typeof(Seasons), season);
            }
        }
    }
}