using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Vega.PittAPI.APITypes;

namespace Vega.PittAPI
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

        public Attribute(string categoryShort, string categoryDescr, string valueShort, string valueDescr)
        {
            CategoryShort = categoryShort;
            CategoryDescr = categoryDescr;
            ValueShort = valueShort;
            ValueDescr = valueDescr;
        }
    }
}