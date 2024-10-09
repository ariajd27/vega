namespace PittAPI.APITypes
{
    public class APIOfferingDetails(int crse_offer_nbr, string subject, string catalog_nbr, string acad_career, string acad_group, string acad_org,
        string campus, string campus_cd, string req_group, string planner_message, APIOpenTerm[] open_terms, APIEnrollableTerm[] enrollable_terms)
    {
        public readonly int crse_offer_nbr = crse_offer_nbr;
        public readonly string subject = subject;
        public readonly string catalog_nbr = catalog_nbr;
        public readonly string acad_career = acad_career;
        public readonly string acad_group = acad_group;
        public readonly string acad_org = acad_org;
        public readonly string campus = campus;
        public readonly string campus_cd = campus_cd;
        public readonly string req_group = req_group;
        public readonly string planner_message = planner_message;
        public readonly APIOpenTerm[] open_terms = open_terms;
        public readonly APIEnrollableTerm[] enrollable_terms = enrollable_terms;
    }
}