using System.Text.Json.Serialization;
using Vega.PittAPI.APITypes;

namespace Vega.PittAPI
{
    public class Attribute
    {
        string CategoryShort { get; }
        string CategoryDescr { get; }
        string ValueShort { get; }
        string ValueDescr { get; }

        public override string ToString()
        {
            return CategoryDescr + ":" + ValueDescr;
        }

        public Attribute (APIAttribute attribute)
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
