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

        SmoothSeasons ss;

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

            ss = SmoothSeasons.instance;
            if (ss)
            {
                ss.OnNewSeason += Ss_OnNewSeason;
            }
        }

        private void Ss_OnNewSeason(Seasons fromSeason, Seasons toSeason)
        {
            if (seasonText)
            {
                seasonText.text = System.Enum.GetName(typeof(Seasons), toSeason);
            }
        }

        private void OnDisable()
        {
            dw.OnWeatherReport -= Instance_OnWeatherReport;
            if (ss)
            {
                ss.OnNewSeason -= Ss_OnNewSeason;
            }
        }

        private void Instance_OnWeatherReport(int dayOfYear, Seasons season, Weathers weather, float temperature)
        {
            string t = triggers[(int)weather];
            anim.SetTrigger(t);
            if (temperatureText)
            {
                temperatureText.text = string.Format(tempPattern, temperature);
            }
            if (seasonText && ss == null)
            {
                seasonText.text = System.Enum.GetName(typeof(Seasons), season);
            }
        }
    }
}