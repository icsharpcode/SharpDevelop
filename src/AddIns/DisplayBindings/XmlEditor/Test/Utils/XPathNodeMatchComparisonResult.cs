// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
