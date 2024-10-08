namespace PittAPI.APITypes
{
    public class APIOpenTerm(string strm, string descr, bool default_term)
    {
        public readonly string strm = strm;
        public readonly string descr = descr;
        public readonly bool default_term = default_term;
    }
}