using System;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace GardenPlanner.Garden
{
    [JsonObject]
    public class VarietyKeySeq
    {
        public readonly string FamilyKey;
        public readonly string PlantKey;
        public readonly string VarietyKey;

        [JsonConstructor]
        public VarietyKeySeq(string FamilyKey, string PlantKey, string VarietyKey)
        {
            this.FamilyKey = FamilyKey;
            this.PlantKey = PlantKey;
            this.VarietyKey = VarietyKey;
        }

        public override int GetHashCode() => FamilyKey.GetHashCode() ^ PlantKey.GetHashCode() ^ VarietyKey.GetHashCode();

        public override string ToString()
        {
            return FamilyKey + "/" + PlantKey + "/" + VarietyKey;
        }
    }


    public class VarietyKeySeqConverter : JsonConverter<VarietyKeySeq>
    {
        public override bool CanRead { get { return true; } }
        public override bool CanWrite { get { return true; } }

        public override VarietyKeySeq ReadJson(JsonReader reader, Type objectType, VarietyKeySeq existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            string s = reader.ReadAsString();
            string[] a = s.Split('/');
            return new VarietyKeySeq(a[0], a[1], a[2]);
        }

        public override void WriteJson(JsonWriter writer, VarietyKeySeq value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }
    }

}
