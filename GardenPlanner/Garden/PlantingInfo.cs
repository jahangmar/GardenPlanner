using System;
namespace GardenPlanner.Garden
{
    public class PlantingInfo
    {
        public DateTime ExactPlantingDate = new DateTime();
        public int Count = 0;
        public int Rows = 0;
        public int RowSpacing = 0;
        public int InBetweenSpacing = 0;
        public int SeedsPerGroup = 1;
        public bool DirectlySown = false;
        public int HarvestCount = 0;
        public int HarvestWeight = 0;

        public PlantingInfo()
        {
        }

        public PlantingInfo(PlantingInfo plantingInfo)
        {
            this.ExactPlantingDate = plantingInfo.ExactPlantingDate;
            this.Count = plantingInfo.Count;
            this.Rows = plantingInfo.Rows;
            this.RowSpacing = plantingInfo.RowSpacing;
            this.InBetweenSpacing = plantingInfo.InBetweenSpacing;
            this.SeedsPerGroup = plantingInfo.SeedsPerGroup;
            this.DirectlySown = plantingInfo.DirectlySown;
            this.HarvestCount = plantingInfo.HarvestCount;
            this.HarvestWeight = plantingInfo.HarvestWeight;
        }
    }
}
