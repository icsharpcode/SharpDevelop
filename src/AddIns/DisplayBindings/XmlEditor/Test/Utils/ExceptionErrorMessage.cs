// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace XmlEditor.Tests.Utils
{
	public class ExceptionErrorMessage
	{
		Exception ex;
		string message;
		
		public ExceptionErrorMessage(Exception ex, string message)
		{
			this.ex = ex;
			this.message = message;
		}
		
		public override bool Equals(object obj)
		{
			ExceptionErrorMessage rhs = obj as ExceptionErrorMessage;
			return (message == rhs.message) && (ex.Message == rhs.ex.Message);
		}
		
		public override string ToString()
		{
			return "Message \"" + message + "\" Exception \"" + ex.Message + "\"";
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
