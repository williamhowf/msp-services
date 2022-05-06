namespace Rmq.Core.Model.Base
{
    public class ResultObject
    {
        public ResultObject()
        {
            ReturnMessage = "Success";
        }
        public virtual int ReturnCode { get; set; }

        public virtual string ReturnMessage { get; set; }
    }
}
