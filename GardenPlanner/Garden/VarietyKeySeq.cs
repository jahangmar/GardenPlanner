using System;
namespace GardenPlanner.Garden
{
    public class VarietyKeySeq
    {
        public readonly string FamilyKey;
        public readonly string PlantKey;
        public readonly string VarietyKey;

        public VarietyKeySeq(string FamilyKey, string PlantKey, string VarietyKey)
        {
            this.FamilyKey = FamilyKey;
            this.PlantKey = PlantKey;
            this.VarietyKey = VarietyKey;
        }

        public override int GetHashCode() => FamilyKey.GetHashCode() ^ PlantKey.GetHashCode() ^ VarietyKey.GetHashCode();
    }
}
