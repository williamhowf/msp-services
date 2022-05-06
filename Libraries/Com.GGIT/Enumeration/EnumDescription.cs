namespace Com.GGIT.Enumeration
{
    public class EnumDescription : System.Attribute
    {
        public EnumDescription(string value)
        {
            Value = value;
        }

        public string Value { get; }
    }
}
