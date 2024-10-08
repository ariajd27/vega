using System.Text.Json.Serialization;
using PittAPI.APITypes;

namespace PittAPI
{
    public class Attribute
    {
        public string CategoryShort { get; set; }
        public string CategoryDescr { get; set; }
        public string ValueShort { get; set; }
        public string ValueDescr { get; set; }

        public override string ToString()
        {
            return CategoryShort + ": " + ValueDescr;
        }

        public Attribute(APIAttribute attribute)
        {
            CategoryShort = attribute.crse_attribute;
            CategoryDescr = attribute.crse_attribute_descr;
            ValueShort = attribute.crse_attribute_value;
            ValueDescr = attribute.crse_attribute_value_descr;
        }

        [JsonConstructor]
        public Attribute(string categoryShort, string categoryDescr, string valueShort, string valueDescr)
        {
            CategoryShort = categoryShort;
            CategoryDescr = categoryDescr;
            ValueShort = valueShort;
            ValueDescr = valueDescr;
        }
    }
}