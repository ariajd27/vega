namespace PittAPI.APITypes
{
    public class APIOffering(string crse_offer_nbr, string[] careers)
    {
        public readonly string crse_offer_nbr = crse_offer_nbr;
        public readonly string[] careers = careers;
    }
}
