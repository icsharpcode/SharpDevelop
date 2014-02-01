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

namespace ICSharpCode.CodeCoverage.Tests.Utils
{
	public class CodeCoverageMethodXElementBuilder
	{
		public XElement MethodElement { get; private set; }
		
		public CodeCoverageMethodXElementBuilder(string methodSignature)
		{
			MethodElement = new XElement("Method");
			
			var nameElement = new XElement("Name");
			nameElement.Value = methodSignature;
			MethodElement.Add(nameElement);
		}
		
		public void MakePropertyGetter()
		{
			SetGetterAttribute(true);
		}
		
		void SetGetterAttribute(bool value)
		{
			SetBooleanAttribute("isGetter", value);
		}
		
		void SetBooleanAttribute(string name, bool value)
		{
			SetAttributeValue(name, value.ToString().ToLowerInvariant());
		}
		
		void SetAttributeValue(string name, object value)
		{
			MethodElement.SetAttributeValue(name, value);
		}
		
		public void MakePropertySetter()
		{
			SetSetterAttribute(true);
		}
		
		void SetSetterAttribute(bool value)
		{
			SetBooleanAttribute("isSetter", value);
		}
		
		public static XElement CreatePropertySetterWithMethodSignature(string methodSignature)
		{
			var builder = new CodeCoverageMethodXElementBuilder(methodSignature);
			builder.SetSetterAttribute(true);
			builder.SetGetterAttribute(false);
			return builder.MethodElement;
		}
		
		public static XElement CreatePropertyGetterWithMethodSignature(string methodSignature)
		{
			var builder = new CodeCoverageMethodXElementBuilder(methodSignature);
			builder.SetSetterAttribute(false);
			builder.SetGetterAttribute(true);
			return builder.MethodElement;
		}
		
		/// <summary>
		/// Generates a setter method signature from the property name and type 
		/// (e.g. "System.Void set_PROPERTYNAME(PROPERTYTYPE)"
		/// </summary>
		public static XElement CreatePropertySetter(string className, string propertyName, string propertyType)
		{
			string methodSignature = String.Format("System.Void {0}::set_{1}({2})", className, propertyName, propertyType);
			return CreatePropertySetterWithMethodSignature(methodSignature);
		}
		
		public static XElement CreateIntegerPropertySetter(string className, string propertyName)
		{
			return CreatePropertySetter(className, propertyName, "System.Int32");
		}
		
		/// <summary>
		/// Generates a getter method signature from the property name and type 
		/// (e.g. "PROPERTYTYPE get_PROPERTYNAME()"
		/// </summary>
		public static XElement CreatePropertyGetter(string className, string propertyName, string propertyType)
		{
			string methodSignature = String.Format("{0} {1}::get_{2}()", propertyType, className, propertyName);
			return CreatePropertyGetterWithMethodSignature(methodSignature);
		}
		
		public static XElement CreateIntegerPropertyGetter(string className, string propertyName)
		{
			return CreatePropertyGetter(className, propertyName, "System.Int32");
		}
		
		public static XElement CreateMethod(string className, string methodName)
		{
			return CreateMethod(className, methodName, "System.Void");
		}
		
		public static XElement CreateMethod(string className, string methodName, string returnType)
		{
			string methodSignature = String.Format("{0} {1}::{2}()", returnType, className, methodName);
			var builder = new CodeCoverageMethodXElementBuilder(methodSignature);
			return builder.MethodElement;
		}
	}
}
