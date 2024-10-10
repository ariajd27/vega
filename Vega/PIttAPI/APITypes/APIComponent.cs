namespace Vega.PittAPI.APITypes
{
    public class APIComponent(string descr, string optional)
    {
        public readonly string descr = descr;
        public readonly string optional = optional;
    }
}