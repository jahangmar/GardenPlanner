// Copyright (c) 2020 Jahangmar
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <https://www.gnu.org/licenses/>.
//

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


    public class DictionaryVarietyKeySeqConverter : JsonConverter<Dictionary<VarietyKeySeq, PlantingInfo>>
    {
        public override Dictionary<VarietyKeySeq, PlantingInfo> ReadJson(JsonReader reader, Type objectType, Dictionary<VarietyKeySeq, PlantingInfo> existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            List<KeyValuePair<VarietyKeySeq, PlantingInfo>> list = serializer.Deserialize<List<KeyValuePair<VarietyKeySeq, PlantingInfo>>>(reader);
            Dictionary<VarietyKeySeq, PlantingInfo> result = new Dictionary<VarietyKeySeq, PlantingInfo>();
            foreach (KeyValuePair<VarietyKeySeq, PlantingInfo> pair in list)
                result.Add(pair.Key, pair.Value);
            return result;
        }

        public override void WriteJson(JsonWriter writer, Dictionary<VarietyKeySeq, PlantingInfo> value, JsonSerializer serializer)
        {
            List<KeyValuePair<VarietyKeySeq, PlantingInfo>> list = new List<KeyValuePair<VarietyKeySeq, PlantingInfo>>();
            foreach (VarietyKeySeq key in value.Keys)
                list.Add(new KeyValuePair<VarietyKeySeq, PlantingInfo>(key, value[key]));
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
