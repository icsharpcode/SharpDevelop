#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.ObjectModelConverters
{
    public enum ObjectModelConverterExceptionEnum
    { 
        CSDL,
        EDM,
        SSDL
    }
    
    public class ObjectModelConverterException : Exception
    {
        #region Constructor

        public ObjectModelConverterException(string message, string detail, ObjectModelConverterExceptionEnum type) : base(message)
        {
            Detail = detail;
            ExceptionType = type;
        }

        #endregion

        #region Properties

        public string FullMessage { get { return Message + "\n\nDetailed error message:\n" + Detail; } }
        public string Detail { get; protected set; }
        public ObjectModelConverterExceptionEnum ExceptionType { get; protected set; }

        #endregion
    }
}
