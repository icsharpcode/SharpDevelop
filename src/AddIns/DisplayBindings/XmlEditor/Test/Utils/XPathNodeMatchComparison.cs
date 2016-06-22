// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
