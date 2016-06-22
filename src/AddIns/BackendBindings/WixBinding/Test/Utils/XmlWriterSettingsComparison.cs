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
using System.Xml;

namespace WixBinding.Tests.Utils
{
	public class XmlWriterSettingsComparison
	{
		bool nullSettings;
		
		public XmlWriterSettingsComparison()
		{
		}
		
		public bool AreEqual(XmlWriterSettings lhs, XmlWriterSettings rhs)
		{
			if ((lhs == null) || (rhs == null)) {
				nullSettings = true;
				return false;
			}
			
			foreach (PropertyInfo property in typeof(XmlWriterSettings).GetProperties()) {
				object lhsValue = property.GetValue(lhs, new object[0]);
				object rhsValue = property.GetValue(rhs, new object[0]);
				
				if (property.PropertyType == typeof(bool)) {
					if ((bool)lhsValue != (bool)rhsValue) {
						PropertyName = property.Name;
						return false;
					}
				} else if (property.PropertyType == typeof(string)) {
					if ((string)lhsValue != (string)rhsValue) {
						PropertyName = property.Name;
						return false;
					}
				}
			}
			return true;
		}
		
		public string PropertyName { get; set; }
		
		public override string ToString() 
		{
			if (nullSettings) {
				return "XmlWriterSetting is null.";
			}
			
			if (String.IsNullOrEmpty(PropertyName)) {
				return String.Empty;
			}
			
			return "Property " + PropertyName + " is different.";
		}
	}
}
