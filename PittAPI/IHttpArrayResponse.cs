namespace PittAPI
{
    public interface IHttpArrayResponse<T>
    {
        public T[] GetContents();
    }
}
