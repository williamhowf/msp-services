namespace Rate.Core.Model.CoindeskPlatform
{
    public class CoindeskHttpBase
    {
        public virtual int StatusCode { get; set; }
        public virtual string Message { get; set; }
        public virtual string Error { get; set; }
    }
}
