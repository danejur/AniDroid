using System;
using AniDroid.AniList;
using MikePhil.Charting.Components;
using MikePhil.Charting.Data;
using MikePhil.Charting.Formatter;
using MikePhil.Charting.Util;

namespace AniDroid.Utils
{
    public class ChartUtils
    {
        public class AxisValueCeilingFormatter : Java.Lang.Object, IAxisValueFormatter
        {
            private int CeilingValue { get; }

            public AxisValueCeilingFormatter(int ceilingVal)
            {
                CeilingValue = ceilingVal;
            }

            public string GetFormattedValue(float value, AxisBase axis)
            {
                return (Math.Ceiling(value / CeilingValue) * CeilingValue).ToString();
            }
        }

        public class AxisAniListEnumFormatter<T> : Java.Lang.Object, IAxisValueFormatter where T : AniListEnum
        {
            public string GetFormattedValue(float value, AxisBase axis)
            {
                return AniListEnum.GetDisplayValue<T>((int)value);
            }
        }

        public class IntegerValueFormatter : Java.Lang.Object, IValueFormatter
        {
            public string GetFormattedValue(float value, Entry entry, int dataSetIndex, ViewPortHandler viewPortHandler)
            {
                return value.ToString("#");
            }
        }
    }
}