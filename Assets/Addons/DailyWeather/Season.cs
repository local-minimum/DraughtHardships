using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DailyWeather
{
    public enum Seasons { Winter, Spring, Summer, Fall };

    [System.Serializable]
    public class Season
    {

        [HideInInspector]
        public string name;

        [HideInInspector]
        public Seasons seasonType;

        [SerializeField]
        List<Season> seasonTransitions = new List<Season>();

        [SerializeField]
        List<AnimationCurve> seasonTransitionProbabilities = new List<AnimationCurve>();

        [SerializeField]
        List<Weather> weathers = new List<Weather>();

        public Season(Seasons seasonType)
        {
            this.seasonType = seasonType;
            name = System.Enum.GetName(typeof(Seasons), seasonType);
            seasonTransitionProbabilities.Add(new AnimationCurve());
            seasonTransitions.Add(this);
        }

        public void AddSeason(Season season)
        {
            seasonTransitions.Add(season);
            seasonTransitionProbabilities.Add(new AnimationCurve());
        }

        public bool AddWeather(Weathers weatherType)
        {
            int l = weathers.Count;
            for (int i = 0; i < l; i++)
            {
                if (weathers[i].weatherType == weatherType)
                {
                    return false;
                }
            }

            Weather w = new Weather(this, weatherType);
            for (int i = 0; i < l; i++)
            {
                weathers[i].AddWeather(w);
                w.AddWeather(weathers[i]);
            }

            return false;
        }

        public Season GetSeasonTransition(float yearProgress)
        {
            return seasonTransitions[0];
        }

        public float GetInitProbability(float yearProgress)
        {
            return seasonTransitionProbabilities[0].Evaluate(yearProgress);
        }

        public Weather GetWeatherTransition(Weather previousWeather, float yearProgress)
        {
            Weather weather = GetMyWeatherClone(previousWeather);
            if (weather == null)
            {
                weather = GetInitialStateWeather(yearProgress);
            }

            return weather.GetWeatherTransition(yearProgress);
        }

        Weather GetMyWeatherClone(Weather previousWeather)
        {
            if (previousWeather == null)
            {
                return null;
            }
            else if (previousWeather.IsMySeason(this))
            {
                return previousWeather;
            }
            else
            {
                for (int i = 0, l = weathers.Count; i < l; i++)
                {
                    if (weathers[i].weatherType == previousWeather.weatherType)
                    {
                        return weathers[i];
                    }
                }
            }

            return null;
        }

        Weather GetInitialStateWeather(float yearProgress)
        {
            float pTot = 0;
            int l = weathers.Count;
            float[] pVector = new float[l];

            for (int i = 0; i < l; i++)
            {
                pVector[i] = weathers[i].GetInitProbability(yearProgress);
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