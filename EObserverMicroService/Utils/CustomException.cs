using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMaintanance.Utils
{
    public class CustomException : Exception
    {
        public string Caption { get; set; }
        public string Message { get; set; }
        public string Type { get; set; }
        public Object Exception { get; set; }
        public bool IsException { get; set; }

        public CustomException()
        {

        }

        public CustomException(string message, String type, bool isException, Object exception) : base(message)
        {
            this.Message = message;
            this.Type = type;
            this.IsException = isException;
            this.Exception = exception;
        }

        public CustomException(string caption, string message, String type, bool isException, Object exception) : base(message)
        {
            this.Caption = caption;
            this.Message = message;
            this.Type = type;
            this.IsException = isException;
            this.Exception = exception;
        }

    }

}
