// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Reflection;
using System.Text;
using ICSharpCode.XmlEditor;

namespace XmlEditor.Tests.Utils
{
	public class XPathNodeMatchComparison
	{
		StringBuilder reason = new StringBuilder();
		
		public XPathNodeMatchComparison()
		{
		}
		
		public bool AreEqual(XPathNodeMatch lhs, XPathNodeMatch rhs)
		{
			reason.Clear();
			
			foreach (PropertyInfo property in typeof(XPathNodeMatch).GetProperties()) {
				ComparePropertyValues(property, lhs, rhs);
			}
			if (lhs.HasLineInfo() != rhs.HasLineInfo()) {
				AppendPropertyDoesNotMatchMessage("LineNumber", GetLineNumberIfHasLineInfo(lhs), GetLineNumberIfHasLineInfo(rhs));
			}
			return !HasReasonForNotMatching;
		}
		
		void ComparePropertyValues(PropertyInfo property, XPathNodeMatch lhs, XPathNodeMatch rhs)
		{
			string lhsPropertyValue = GetPropertyValue(property, lhs);
			string rhsPropertyValue = GetPropertyValue(property, rhs);
			if (lhsPropertyValue != rhsPropertyValue) {
				AppendPropertyDoesNotMatchMessage(property.Name, lhsPropertyValue, rhsPropertyValue);
			}
		}
		
		bool HasReasonForNotMatching {
			get { return reason.Length > 0; }
		}
		
		string GetPropertyValue(PropertyInfo property, XPathNodeMatch nodeMatch)
		{
			return property.GetValue(nodeMatch, new object[0]).ToString();
		}
		
		void AppendPropertyDoesNotMatchMessage(string name, string lhs, string rhs)
		{
			reason.AppendFormat("{0}s do not match. Expected '{1}' but was '{2}'.\r\n", name, lhs, rhs);
		}
		
		string GetLineNumberIfHasLineInfo(XPathNodeMatch nodeMatch)
		{
			if (nodeMatch.HasLineInfo()) {
				return nodeMatch.LineNumber.ToString();
			}
			return "null";
		}
		
		public string GetReasonForNotMatching()
		{
			return reason.ToString().Trim();
		}
	}
}
