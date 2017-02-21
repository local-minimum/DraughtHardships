using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DailyWeather
{
    public delegate void WeatherReport(int dayOfYear, Seasons season, Weathers weather, float temperature);

    public class DailyWeather : MonoBehaviour
    {

        public static DailyWeather instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<DailyWeather>();
                }
                if (_instance == null)
                {
                    Debug.LogError("The scene has no weather object");
                }
                return _instance;
            }

        }

        static DailyWeather _instance;

        public event WeatherReport OnWeatherReport;

        [SerializeField, HideInInspector]
        List<Season> seasons = new List<Season>();

        [SerializeField]
        float reportInterval;

        float nextReport;
        
        [SerializeField]
        int startYear;

        [SerializeField]
        int daysPerYear = 356;

        [SerializeField, Range(0, 1)]
        float yearProgressionAtStart;

        [SerializeField]
        float yearInRealTime = 60;

        [SerializeField]
        bool debugLogWeathers;

        public AnimationCurve annualMeanTemperature;

        Season currentSeason;
        Weather currentWeather = null;

        public float YearProgress
        {
            get
            {
                return (Time.timeSinceLevelLoad/yearInRealTime + yearProgressionAtStart) % 1f;
            }
        }

        public int DayOfYear
        {
            get
            {
                return Mathf.FloorToInt(YearProgress * daysPerYear) + 1;
            }
        }

        public int Year
        {
            get
            {
                return Mathf.FloorToInt(Time.timeSinceLevelLoad / yearInRealTime + yearProgressionAtStart);
            }
        }

        public void EmitDailyWeatherReport()
        {
            float yearProgress = YearProgress;
            currentSeason = currentSeason.GetSeasonTransition(yearProgress);
            //Debug.Log(currentSeason.name);
            currentWeather = currentSeason.GetWeatherTransition(currentWeather, yearProgress);
            //Debug.Log(currentWeather.name);
            float temp = GetTemperature(yearProgress);

            if (OnWeatherReport != null)
            {
                OnWeatherReport(
                    DayOfYear,
                    currentSeason.seasonType,
                    currentWeather.weatherType,
                    temp
                    );
            }

            if (debugLogWeathers)
            {
                Debug.Log(string.Format("Y {0}, d {1}: {2}, {3} ({4} C)",
                    Year, DayOfYear, currentSeason.seasonType,
                    currentWeather.weatherType, temp));
            }
        }

        float GetTemperature(float yearProgress)
        {
            return annualMeanTemperature.Evaluate(yearProgress) +
                currentWeather.anualTemperatureModification.Evaluate(yearProgress);
        }

        private void Start()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else if (_instance != this)
            {
                Debug.LogError(string.Format(
                    "Two daily weather systems in scene! {0} kept {1} removed.",
                    _instance, this
                    ));
                Destroy(this);
                return;
            }

            SetWeatherSystem();
            currentSeason = GetInitSeason();
        }

        void SetWeatherSystem()
        {
            for (int i=0, l=seasons.Count; i<l; i++)
            {
                seasons[i].SetParentWeather(this);
                seasons[i].SetWeatherParents();
            }
        }

        public Season GetSeason(Seasons seasonType)
        {
            for (int i = 0, l = seasons.Count; i < l; i++)
            {
                if (seasons[i].seasonType == seasonType)
                {
                    return seasons[i];
                }
            }
            return null;
        }

        Season GetInitSeason()
        {
            if (currentSeason == null)
            {
                float yearProgress = YearProgress;
                float pTot = 0;
                int l = seasons.Count;
                float[] pVector = new float[l];

                for (int i = 0; i < l; i++)
                {
                    pVector[i] = seasons[i].GetInitProbability(yearProgress);
                    pTot += pVector[i];
                }

                float p = Random.value * pTot;
                pTot = 0;

                for (int i = 0; i < l; i++)
                {
                    pTot += pVector[i];
                    if (pTot < p)
                    {
                        return seasons[i];
                    }
                }

                return seasons[l - 1];
            }
            else
            {
                return currentSeason;
            }
        }

        private void Update()
        {
            if (Time.timeSinceLevelLoad > nextReport)
            {
                nextReport = Mathf.Max(nextReport + reportInterval, Time.timeSinceLevelLoad);
                EmitDailyWeatherReport();
            }
        }

        public bool HasSeason(Seasons seasonType)
        {
            int l = seasons.Count;

            for (int i = 0; i < l; i++)
            {
                if (seasons[i].seasonType == seasonType)
                {
                    return true;
                }
            }
            return false;
        }

        public bool AddSeason(Seasons seasonType)
        {
            int l = seasons.Count;

            for (int i = 0; i < l; i++)
            {
                if (seasons[i].seasonType == seasonType)
                {
                    return false;
                }
            }

            Season s = new Season(seasonType);
            seasons.Add(s);

            for (int i = 0; i < l; i++)
            {
                seasons[i].AddSeason(s);
                s.AddSeason(seasons[i]);
            }
            return true;
        }
    }
}