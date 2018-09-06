using System;
using System.Drawing;
using Newtonsoft.Json;

namespace NeverEmptyPantry.Common.Models
{
    public class ChartDataSet
    {
        public string[] Label { get; set; }
        [JsonConverter(typeof(ColorConverter))]
        public Color BackgroundColor { get; set; }
        [JsonConverter(typeof(ColorConverter))]
        public Color BorderColor { get; set; }
        public ChartPointLocationRadius Data { get; set; }
        public bool Hidden { get; set; } = false;
    }

    public class ChartPointLocationRadius
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double R { get; set; }
    }

    public class ColorConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Color val = (Color)value;
            writer.WriteValue($"rgba({val.R}, {val.G}, {val.B}, {val.A})");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return Color.FromArgb(Convert.ToInt32(reader.Value));
        }

        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(Color));
        }
    }
}