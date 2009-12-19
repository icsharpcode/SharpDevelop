// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace XmlEditor.Tests.Utils
{
	public class XPathNodeMatchComparisonResult
	{
		public XPathNodeMatchComparisonResult(bool result, string message)
		{
			this.Result = result;
			this.Message = message;
		}
		
		public XPathNodeMatchComparisonResult()
		{
		}
		
		public bool Result { get; set; }
		public string Message { get; set; }
		
		public override string ToString()
		{
			return String.Format("Result: {0}, Message: '{1}'", Result, Message);
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		
		public override bool Equals(object obj)
		{
			XPathNodeMatchComparisonResult rhs = obj as XPathNodeMatchComparisonResult;
			if (rhs != null) {
				return (rhs.Result == Result) && (rhs.Message == Message);
			}
			return false;
		}
	}
}
