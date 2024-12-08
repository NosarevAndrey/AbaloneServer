namespace AbaloneServer.Models
{
    public class GalleryItemViewModel
    {
        public int Id { get; set; }
        public int EstimatedAge { get; set; }
        public string SubmitterName { get; set; }
        public string Description { get; set; }
        public AbaloneMeasurements Measurements { get; set; }

        public GalleryItemViewModel(int id, string submitterName, string description, AbaloneSubmissionViewModel abaloneData)
        {
            Id = id;
            SubmitterName = submitterName;
            Description = description;
            EstimatedAge = abaloneData.EstimatedAge;
            Measurements = new AbaloneMeasurements
            {
                Sex = abaloneData.Sex,
                Length = abaloneData.Length,
                Diameter = abaloneData.Diameter,
                Height = abaloneData.Height,
                WholeWeight = abaloneData.WholeWeight,
                ShuckedWeight = abaloneData.ShuckedWeight,
                VisceraWeight = abaloneData.VisceraWeight,
                ShellWeight = abaloneData.ShellWeight
            };
        }
    }

    public class AbaloneMeasurements
    {
        public AbaloneSex Sex { get; set; }
        public double Length { get; set; }
        public double Diameter { get; set; }
        public double Height { get; set; }
        public double WholeWeight { get; set; }
        public double ShuckedWeight { get; set; }
        public double VisceraWeight { get; set; }
        public double ShellWeight { get; set; }
    }
}
