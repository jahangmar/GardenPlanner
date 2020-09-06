using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
namespace GardenPlanner.Garden
{
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

        public VarietyKeySeq FromString(string s)
        {
            if (s.Equals("null"))
                return null;

            string[] a = s.Split('/');
            return new VarietyKeySeq(a[0], a[1], a[2]);
        }
    }


    public class DictionaryVarietyKeySeqConverter : JsonConverter<Dictionary<VarietyKeySeq, int>>
    {
        public override Dictionary<VarietyKeySeq, int> ReadJson(JsonReader reader, Type objectType, Dictionary<VarietyKeySeq, int> existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            List<KeyValuePair<VarietyKeySeq, int>> list = serializer.Deserialize<List<KeyValuePair<VarietyKeySeq, int>>>(reader);
            Dictionary<VarietyKeySeq, int> result = new Dictionary<VarietyKeySeq, int>();
            foreach (KeyValuePair<VarietyKeySeq, int> pair in list)
                result.Add(pair.Key, pair.Value);
            return result;
        }

        public override void WriteJson(JsonWriter writer, Dictionary<VarietyKeySeq, int> value, JsonSerializer serializer)
        {
            List<KeyValuePair<VarietyKeySeq, int>> list = new List<KeyValuePair<VarietyKeySeq, int>>();
            foreach (VarietyKeySeq key in value.Keys)
                list.Add(new KeyValuePair<VarietyKeySeq, int>(key, value[key]));
            //writer.WriteValue(JsonConvert.SerializeObject(list));
            serializer.Serialize(writer, list);
        }
    }
    /*
    public class VarietyKeySeqConverter : JsonConverter<VarietyKeySeq>
    {
        public override bool CanRead { get { return true; } }
        public override bool CanWrite { get { return true; } }

        public override VarietyKeySeq ReadJson(JsonReader reader, Type objectType, VarietyKeySeq existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            System.Console.WriteLine("converter read json");
            string s = reader.ReadAsString();
            string[] a = s.Split('/');
            return new VarietyKeySeq(a[0], a[1], a[2]);
        }

        public override void WriteJson(JsonWriter writer, VarietyKeySeq value, JsonSerializer serializer)
        {
            System.Console.WriteLine("converter write json");
            writer.WriteValue(value.ToString());
        }
    }
    */

}
