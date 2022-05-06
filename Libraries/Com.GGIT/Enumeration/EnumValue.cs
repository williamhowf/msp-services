namespace Com.GGIT.Enumeration
{
    public class EnumValue : System.Attribute
    {
        public EnumValue(string value)
        {
            Value = value;
        }

        public string Value { get; }
    }
}
