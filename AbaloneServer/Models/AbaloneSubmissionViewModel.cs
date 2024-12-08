namespace AbaloneServer.Models
{
    public class AbaloneSubmissionViewModel
    {
        public AbaloneSex Sex { get; set; }
        public double Length { get; set; }
        public double Diameter { get; set; }
        public double Height { get; set; }
        public double WholeWeight { get; set; }
        public double ShuckedWeight { get; set; }
        public double VisceraWeight { get; set; }
        public double ShellWeight { get; set; }

        public int EstimatedAge { get; set; }
    }
}
