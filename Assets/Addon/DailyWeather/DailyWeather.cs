using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void WeatherReport(int dayOfYear, Seasons season, Weathers weather, float temperature);

public class DailyWeather : MonoBehaviour {

    public static DailyWeather instance
    {
        get {
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
    float gameTimePerDay = 5;

    [SerializeField]
    int daysPerYear = 365;

    int dayOfYear;

    [SerializeField]
    int year;

    [SerializeField]
    int startDayOfYear;

    [SerializeField]
    bool debugLogWeathers;

    public AnimationCurve annualMeanTemperature;

    Season currentSeason;
    Weather currentWeather = null;

    public float YearProgress
    {
        get
        {
            return dayOfYear / (float)daysPerYear;
        }
    }

    public int DayOfYear
    {
        get
        {
            return dayOfYear;
        }
    }

    int CalculateDayOfYear()
    {
        return (Mathf.FloorToInt(Time.timeSinceLevelLoad / gameTimePerDay) + startDayOfYear) % daysPerYear;
    }

    public void EmitDailyWeatherReport()
    {
        float yearProgress = YearProgress;
        currentSeason = currentSeason.GetSeasonTransition(yearProgress);
        currentWeather = currentSeason.GetWeatherTransition(currentWeather, yearProgress);
        float temp = GetTemperature(yearProgress);

        if (OnWeatherReport != null)
        {
            OnWeatherReport(
                dayOfYear, 
                currentSeason.seasonType,
                currentWeather.weatherType,
                temp
                );
        }

        if (debugLogWeathers)
        {
            Debug.Log(string.Format("Y {0}, d {1}: {2}, {3} ({4} C)",
                year, dayOfYear, currentSeason.seasonType,
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
        } else if (_instance != this)
        {
            Debug.LogError(string.Format(
                "Two daily weather systems in scene! {0} kept {1} removed.",
                _instance, this
                ));
            Destroy(this);
        }
        currentSeason = GetInitSeason();
    }

    Season GetInitSeason()
    {
        if (currentSeason == null)
        {
            float yearProgress = YearProgress;
            float pTot = 0;
            int l = seasons.Count;
            float[] pVector = new float[l];

            for (int i=0; i<l; i++)
            {
                pVector[i] = seasons[i].GetInitProbability(yearProgress);
                pTot += pVector[i];
            }

            float p = Random.value * pTot;
            pTot = 0;

            for (int i=0; i<l; i++)
            {
                pTot += pVector[i];
                if (pTot < p)
                {
                    return seasons[i];
                }
            }

            return seasons[l - 1];
        } else
        {
            return currentSeason;
        }        
    }

    private void Update()
    {
        int calcDay = CalculateDayOfYear();
        if (calcDay != dayOfYear)
        {
            dayOfYear = calcDay;
            EmitDailyWeatherReport();
        }
    }

    public bool AddSeason(Seasons seasonType)
    {
        int l = seasons.Count;

        for (int i=0; i<l; i++)
        {
            if (seasons[i].seasonType == seasonType)
            {
                return false;
            }
        }

        Season s = new Season(seasonType);
        seasons.Add(s);

        for (int i=0; i<l; i++)
        {
            seasons[i].AddSeason(s);
            s.AddSeason(seasons[i]);
        }
        return true;
    }
}
