using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OccaSoftware.Altos.Runtime
{
    /// <summary>
    /// This class defines a Sky Definition data asset. 
    /// A Sky Definition describes and manages the system time, the period of day, the environmental lighting.
    /// </summary>
	[CreateAssetMenu(fileName = "Sky Definition", menuName = "Altos/Sky Definition")]
    public class SkyDefinition : ScriptableObject
    {
        private void OnValidate()
        {
            initialTime = Mathf.Max(0, initialTime);
        }

        /// <summary>
        /// Sorts the periods of day key frames by start time, ascending.
        /// </summary>
        public void SortPeriodsOfDay()
		{
            periodsOfDay = periodsOfDay.OrderBy(x => x.startTime).ToList();
        }

        /// <summary>
        /// Represents the current set of periods of day key frames.
        /// </summary>
        public List<PeriodOfDay> periodsOfDay;

        /// <summary>
        /// Controls the active day and time of day that will be initialized when the game starts. Represented as hours from an arbitrary start of 0.
        /// </summary>
        public float initialTime = 0f;

        /// <summary>
        /// Represents the current in-game time. A function of the initial time of day and the day-night cycle duration.
        /// </summary>
        public float CurrentTime
		{
            get => timeSystem % 24;
		}

        /// <summary>
        /// Represents the cumulative number of hours (in Altos time) elapsed from an activeTimeOfDay value of 0 on the game start event.
        /// For example, if your time of day is 12 and you start the game, this value will be initialized to 12. 
        /// 24 hours later (in Altos time), this value will be 36.
        /// </summary>
        public float timeSystem = 0f;

        /// <summary>
        /// Represents the number of 24-hour periods (in Altos time) elapsed from a timeSystem value of 0.
        /// </summary>
        public int CurrentDay
		{
            get => (int)timeSystem / 24;
		}

        /// <summary>
        /// Used to specify the duration in real-world hours of a complete in-game day-night cycle (from 0 - 24h).
        /// For example, a day night cycle duration of 1 means that one in-game day-night cycle will complete per 1 hour of real time.
        /// </summary>
        [Min(0)]
        public float dayNightCycleDuration = 1f;

        /// <summary>
        /// Tracks the current index for the time of day period keyframe in use.
        /// </summary>
        private int periodIndex = 0;
        /// <summary>
        /// Tracks the expected next index for the time of day period keyframe in use. Wraps around to 0 when in excess of the list length.
        /// </summary>
        private int nextPeriodIndex = 0;

        /// <summary>
        /// Contains the current interpolated sky colors based on the current time and period of day keyframes.
        /// </summary>
        private SkyColorSet skyColors;

        /// <summary>
        /// Contains the current interpolated sky colors based on the current time and period of day keyframes.
        /// </summary>
        public SkyColorSet SkyColors
        {
            get => skyColors;
        }

        /// <summary>
        /// Initializes the Sky Definition, setting up the current time, period of day, and sky properties.
        /// </summary>
        public void Initialize()
		{
            timeSystem = initialTime;

            UpdateTime();
            UpdatePeriod();
            UpdateSky();
		}

        /// <summary>
        /// Updates the Sky Definition, updating the current time, period of day, and sky properties.
        /// </summary>
        public void Update()
		{
            UpdateTime();
            UpdatePeriod();
            UpdateSky();
		}

        /// <summary>
        /// Updates the current time. In edit mode, this will set the current time to the initial time.
        /// </summary>
        public void UpdateTime()
		{
            if (!Application.isPlaying)
			{
                timeSystem = initialTime;
                return;
            }
                

            if (dayNightCycleDuration > 0f)
            {
                const float CONVERSION_FACTOR = 24f * 1f / 3600f;
                float t = Time.deltaTime * CONVERSION_FACTOR / dayNightCycleDuration;
                timeSystem += t;
            }
        }

        /// <summary>
        /// Sets the system time directly. The system time is continuous and includes the day count (i.e., 1 day + 12h = 36).
        /// </summary>
        public void SetSystemTime(float timeSystem)
		{
            this.timeSystem = timeSystem;
            UpdatePeriod();
            UpdateSky();
		}

        /// <summary>
        /// Sets the current day and time of day directly. Time will be wrapped in the range [0, 24]. Day has a min value of 0.
        /// </summary>
        public void SetDayAndTime(int day, float time)
		{
            timeSystem = (Mathf.Max(0, day) * 24) + (time % 24);
            UpdatePeriod();
            UpdateSky();
        }

        /// <summary>
        /// Updates the current period of day, used during the sky color interpolation method.
        /// </summary>
        public void UpdatePeriod()
        {
            if (Application.isPlaying)
            {
                HandlePeriod(CurrentTime);
            }
            else
            {
                HandlePeriod(CurrentTime);
            }

        }


        private void HandlePeriod(float time)
        {
            if (time < periodsOfDay[0].startTime)
            {
                periodIndex = periodsOfDay.Count - 1;
            }
            else
            {
                for (int i = 0; i < periodsOfDay.Count; i++)
                {
                    if (time >= periodsOfDay[i].startTime)
                    {
                        periodIndex = i;
                    }
                }
            }


            nextPeriodIndex = periodIndex + 1;

            if (nextPeriodIndex >= periodsOfDay.Count)
            {
                nextPeriodIndex = 0;
            }
        }


        /// <summary>
        /// Sets the Horizon and Zenith colors based on the current time of day.
        /// As we approach the next time of day, we transition smoothly to the Horizon and Zenith colors for that time of day.
        /// </summary>
        public void UpdateSky()
        {
            if (Application.isPlaying)
            {
                HandleSkyColor(CurrentTime);
            }
            else
            {
                HandleSkyColor(initialTime);
            }

            SetLightingEnvironment(SkyColors);
        }


        /// <summary>
        /// Updates the current sky color values based on the current and subsequent period keyframes as well as the current time.
        /// </summary>
        /// <param name="time"></param>
        private void HandleSkyColor(float time)
        {
            // Handle day/night rollover
            float startTime = periodsOfDay[periodIndex].startTime;
            if (startTime > periodsOfDay[nextPeriodIndex].startTime)
            {
                startTime -= 24f;
            }

            if (time > periodsOfDay[nextPeriodIndex].startTime)
            {
                time -= 24f;
            }


            // Calculate and set colors.
            float t = Helpers.Remap(time, startTime, periodsOfDay[nextPeriodIndex].startTime, 0, 1);

            skyColors.equatorColor = Color.Lerp(periodsOfDay[periodIndex].horizonColor, periodsOfDay[nextPeriodIndex].horizonColor, t);
            skyColors.skyColor = Color.Lerp(periodsOfDay[periodIndex].zenithColor, periodsOfDay[nextPeriodIndex].zenithColor, t);
            skyColors.groundColor = Color.Lerp(periodsOfDay[periodIndex].groundColor, periodsOfDay[nextPeriodIndex].groundColor, t);
        }

        /// <summary>
        /// Overrides Unity's lighting environment to a Trilight setup with the given colors defined in the input SkyColorSet.
        /// </summary>
        /// <param name="skyColorSet"></param>
        public void SetLightingEnvironment(SkyColorSet skyColorSet)
        {
            RenderSettings.skybox = null;
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
            RenderSettings.ambientGroundColor = skyColorSet.groundColor;
            RenderSettings.ambientEquatorColor = skyColorSet.equatorColor;
            RenderSettings.ambientSkyColor = skyColorSet.skyColor;
        }
    }

    /// <summary>
    /// A set of interpolated colors for the sky's ground, equator, and sky colors.
    /// </summary>
    public struct SkyColorSet
    {
        public Color groundColor;
        public Color equatorColor;
        public Color skyColor;
    }
    /// <summary>
    /// Contains all the information about a given Period of Day keyframe.
    /// </summary>
    [System.Serializable]
    public class PeriodOfDay
    {
        [SerializeField]
        [Tooltip("(Optional) Descriptive Name")]
        public string description;
        [SerializeField, Range(0f, 24f)]
        [Tooltip("Set the Start Time for this Period of Day")]
        public float startTime;
        [SerializeField, ColorUsage(false, true)]
        [Tooltip("Set the Horizon Color for this Period of Day")]
        public Color horizonColor;
        [SerializeField, ColorUsage(false, true)]
        [Tooltip("Set the Zenith Color for this Period of Day")]
        public Color zenithColor;
        [SerializeField, ColorUsage(false, true)]
        [Tooltip("Set the Ground Color for this Period of Day")]
        public Color groundColor;

        public PeriodOfDay(string desc, float start, Color horizon, Color zenith, Color ground)
        {
            description = desc;
            startTime = start;
            horizonColor = horizon;
            zenithColor = zenith;
            groundColor = ground;
        }
    }

}
