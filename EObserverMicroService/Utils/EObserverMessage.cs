using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMaintanance.Utils
{
    public class EObserverMessage
    {
        public EObserverMessage(string msg, string type = "Warning", bool isexception = true, string exception = null)
        {
            this.Message = msg;
            this.Type = type;
            this.IsException = isexception;
            this.Exception = exception;
        }
        public string Message { get; set; }
        public string Type { get; set; }
        public string Exception { get; set; }
        public bool IsException { get; set; }
    }
}
