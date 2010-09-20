// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace XmlEditor.Tests.Utils
{
	public class FormattedErrorMessage
	{
		string messageFormat = String.Empty;
		string parameter = String.Empty;
		
		public FormattedErrorMessage(string messageFormat, string parameter)
		{
			this.messageFormat = messageFormat;
			this.parameter = parameter;
		}
		
		public override string ToString()
		{
			return "Message \"" + messageFormat + "\" Parameter \"" + parameter + "\"";
		}
		
		public override bool Equals(object obj)
		{
			FormattedErrorMessage rhs = obj as FormattedErrorMessage;
			return (messageFormat == rhs.messageFormat) && (parameter == rhs.parameter);
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}	
	}
}
