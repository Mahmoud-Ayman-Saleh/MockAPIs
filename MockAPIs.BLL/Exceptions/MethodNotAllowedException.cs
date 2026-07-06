namespace MockAPIs.BLL.Exceptions
{
    public class MethodNotAllowedException : Exception
    {
        public MethodNotAllowedException(string message) : base(message) { }
    }
}