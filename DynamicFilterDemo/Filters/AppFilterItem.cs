namespace DynamicFilterDemo.Filters
{
    public class AppFilterItem
    {
        public string Field { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }
        public string Logic { get; set; } = "and";
    }

    public class AppFilterItem2
    {
        public string PropertyName { get; set; }
        public string Value { get; set; }

        public int OperatorType { get; set; }

        public Operator Operator
        {
            get
            {
                return (Operator)OperatorType;
            }
            set
            {
                OperatorType = (int)value;
            }
        }
    }

    public enum Operator
    {
        Contains = 1,
        GreaterThan = 2,
        GreaterThanOrEqual = 3,
        LessThan = 4,
        LessThanOrEqualTo = 5,
        StartsWith = 6,
        EndsWith = 7,
        Equals = 8,
        NotEqual = 9,
        IsFalse = 11,
        IsTrue = 12
    }
}
