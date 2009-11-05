// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
