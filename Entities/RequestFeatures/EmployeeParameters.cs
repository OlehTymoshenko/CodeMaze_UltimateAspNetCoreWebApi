namespace Entities.RequestFeatures
{
    public class EmployeeParameters : RequestParameters
    {
        public EmployeeParameters()
        {
            OrderBy = "name";
        }

        public uint MinAge { get; set; }
        public uint MaxAge { get; set; } = int.MaxValue;

        public bool IsAgeRangeValid => MinAge <= MaxAge;

        public string NameToSearch { get; set; }
    }
}
