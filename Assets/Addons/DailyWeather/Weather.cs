using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DailyWeather
{

    public enum Weathers
    {
        Sunny, ScatteredClouds, Cloudy, Overcast, Drizzle, RainShowers,
        Raining, Thunderstorm, HailShower, Snowing
    };


    [System.Serializable]
    public class Weather
    {

        [HideInInspector]
        public string name;

        [HideInInspector]
        public Weathers weatherType;

        [SerializeField]
        Season season;

        public AnimationCurve anualTemperatureModification;

        [SerializeField]
        List<AnimationCurve> weatherTransitions = new List<AnimationCurve>();

        [SerializeField]
        List<Weather> weathers = new List<Weather>();

        public Weather(Season season, Weathers weatherType)
        {
            this.season = season;
            this.weatherType = weatherType;
            name = System.Enum.GetName(typeof(Weathers), weatherType);
            weatherTransitions.Add(new AnimationCurve());
            weathers.Add(this);
        }

        public void AddWeather(Weather weather)
        {
            weathers.Add(weather);
            weatherTransitions.Add(new AnimationCurve());
        }

        public bool IsMySeason(Season season)
        {
            return this.season == season;
        }

        public float GetInitProbability(float yearProgress)
        {
            return weatherTransitions[0].Evaluate(yearProgress);
        }

        public Weather GetWeatherTransition(float yearProgress)
        {
            float pTot = 0;
            int l = weathers.Count;
            float[] pVector = new float[l];

            for (int i = 0; i < l; i++)
            {
                pVector[i] = weatherTransitions[i].Evaluate(yearProgress);
                pTot += pVector[i];
            }

            float v = Random.value * pTot;
            pTot = 0;
            for (int i = 0; i < l; i++)
            {
                pTot += pVector[i];
                if (v < pTot)
                {
                    return weathers[i];
                }
            }

            return weathers[l - 1];

        }
    }
}