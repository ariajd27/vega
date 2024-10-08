namespace PittAPI.APITypes
{
    public class APIAttribute(string crse_attribute, string crse_attribute_descr, string crse_attribute_value, string crse_attribute_value_descr)
    {
        public readonly string crse_attribute = crse_attribute;
        public readonly string crse_attribute_descr = crse_attribute_descr;
        public readonly string crse_attribute_value = crse_attribute_value;
        public readonly string crse_attribute_value_descr = crse_attribute_value_descr;
    }
}