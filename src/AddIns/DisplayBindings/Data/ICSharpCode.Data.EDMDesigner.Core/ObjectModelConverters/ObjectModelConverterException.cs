// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
		public ObjectModelConverterException(string message, string detail, ObjectModelConverterExceptionEnum type)
			: base(message)
		{
			Detail = detail;
			ExceptionType = type;
		}

		public string FullMessage { get { return Message + "\n\nDetailed error message:\n" + Detail; } }
		public string Detail { get; protected set; }
		public ObjectModelConverterExceptionEnum ExceptionType { get; protected set; }
		
		public override string ToString()
		{
			return String.Format("{0}\n{1}", FullMessage, base.ToString());
		}
	}
}
