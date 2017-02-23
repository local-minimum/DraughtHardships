using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DailyWeather
{

    public delegate void NewSeason(Seasons fromSeason, Seasons toSeason);

    public class SmoothSeasons : MonoBehaviour
    {
        static SmoothSeasons _instance;

        public static SmoothSeasons instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<SmoothSeasons>();
                }
                return _instance;
            }
        }

        DailyWeather dw;
        public event NewSeason OnNewSeason;

        Seasons current;

        Seasons[] buffer;
        int idx = 0;

        [SerializeField, Range(2, 30)]
        int smoothness = 10;

        bool firstSignal = true;

        private void Start()
        {
            if (_instance == null)
            {
                _instance = this;
            } else if (_instance != this)
            {
                Destroy(this);
                return;
            }
            buffer = new Seasons[smoothness];
        }

        private void OnEnable()
        {
            dw = DailyWeather.instance;
            dw.OnWeatherReport += Dw_OnWeatherReport;
        }

        private void OnDisable()
        {
            dw.OnWeatherReport -= Dw_OnWeatherReport;
        }

        private void Dw_OnWeatherReport(int dayOfYear, Seasons season, Weathers weather, float temperature)
        {
            StoreSeason(season);
            Seasons current = Current;
            Seasons newSeason = CalculateCurrent();
            this.current = newSeason;
            if ((firstSignal || newSeason != current) && OnNewSeason != null)
            {
                OnNewSeason(current, newSeason);
            }
            firstSignal = false;
        }

        void StoreSeason(Seasons s)
        {
            if (firstSignal)
            {
                for (int i = 0; i < buffer.Length; i++)
                {
                    buffer[i] = s;
                }
                idx = 0;
            }
            else
            {
                buffer[idx] = s;
            }
            idx++;
            idx %= buffer.Length;
        }

        Seasons CalculateCurrent()
        {
            Dictionary<Seasons, int> counts = new Dictionary<Seasons, int>();            

            for (int i =0; i<buffer.Length; i++)
            {
                if (!counts.ContainsKey(buffer[i]))
                {
                    counts[buffer[i]] = 1;
                } else
                {
                    counts[buffer[i]] += 1;
                }
            }

            Seasons mostCommon = current;
            int freq = !firstSignal && counts.ContainsKey(current) ? counts[current] + 1 : -1;

            foreach (Seasons s in counts.Keys)
            {
                if (counts[s] > freq)
                {
                    mostCommon = s;
                    freq = counts[s];
                }
            }

            
            return mostCommon;
        }

        public Seasons Current
        {
            get
            {
                return current;
            }
        }
    }
}
