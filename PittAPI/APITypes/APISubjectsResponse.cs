namespace PittAPI.APITypes
{
    public class APISubjectsResponse(APISubject[] subjects) : IHttpArrayResponse<APISubject>
    {
        public APISubject[] subjects = subjects;
        public APISubject[] GetContents() => subjects;
    }
}
