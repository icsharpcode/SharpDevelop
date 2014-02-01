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
using System.Xml.Linq;

namespace ICSharpCode.CodeCoverage
{
	public class CodeCoverageMethodElement
	{
		XElement element;
		
		public CodeCoverageMethodElement(XElement element)
		{
			this.element = element;
			Init();
		}
		
		public bool IsGetter { get; private set; }
		public bool IsSetter { get; private set; }
		public string MethodName { get; private set; }
		
		public bool IsProperty {
			get { return IsGetter || IsSetter; }
		}
		
		void Init()
		{
			IsGetter = GetBooleanAttributeValue("isGetter");
			IsSetter = GetBooleanAttributeValue("isSetter");
			MethodName = GetMethodName();
		}
		
		bool GetBooleanAttributeValue(string name)
		{
			return GetBooleanAttributeValue(element.Attribute(name));
		}
		
		bool GetBooleanAttributeValue(XAttribute attribute)
		{
			if (attribute != null) {
				bool value = false;
				if (Boolean.TryParse(attribute.Value, out value)) {
					return value;
				}
			}
			return false;
		}
		
		string GetMethodName()
		{
			XElement nameElement = element.Element("Name");
			if (nameElement != null) {
				return GetMethodName(nameElement.Value);
			}
			return String.Empty;
		}
		
		string GetMethodName(string methodSignature)
		{
			int startIndex = methodSignature.IndexOf("::");
			int endIndex = methodSignature.IndexOf('(', startIndex);
			return methodSignature
				.Substring(startIndex, endIndex - startIndex)
				.Substring(2);
		}
	}
}
