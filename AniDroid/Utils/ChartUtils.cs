using System;
using AniDroid.AniList;
using AniDroid.Utils.Extensions;
using MikePhil.Charting.Components;
using MikePhil.Charting.Data;
using MikePhil.Charting.Formatter;

namespace AniDroid.Utils
{
    public class ChartUtils
    {
        public class AxisValueCeilingFormatter : ValueFormatter
        {
            private int CeilingValue { get; }

            public AxisValueCeilingFormatter(int ceilingVal)
            {
                CeilingValue = ceilingVal;
            }

            public override string GetAxisLabel(float value, AxisBase axis)
            {
                return (Math.Ceiling(value / CeilingValue) * CeilingValue).ToString();
            }
        }

        public class AxisAniListEnumFormatter<T> : ValueFormatter where T : AniListEnum
        {
            public override string GetAxisLabel(float value, AxisBase axis)
            {
                return AniListEnum.GetDisplayValue<T>((int)value);
            }
        }

        public class NumberValueFormatter : ValueFormatter
        {
            public override string GetBarLabel(BarEntry barEntry)
            {
                return barEntry.GetY().KiloFormat();
            }

            public override string GetPointLabel(Entry entry)
            {
                return entry.GetY().KiloFormat();
            }
        }

        public class DateValueFormatter : ValueFormatter
        {
            private readonly string _formatString;

            public DateValueFormatter(string formatString = null)
            {
                _formatString = formatString;
            }

            public override string GetAxisLabel(float value, AxisBase axis)
            {
                return DateTimeOffset.FromUnixTimeSeconds((long) value).ToString(_formatString ?? "MMM d");
            }
        }

        public class IntegerValueFormatter : ValueFormatter
        {
            public override string GetBarLabel(BarEntry barEntry)
            {
                return barEntry.GetY().ToString("#");
            }

            public override string GetPointLabel(Entry entry)
            {
                return entry.GetY().ToString("#");
            }
        }
    }
}