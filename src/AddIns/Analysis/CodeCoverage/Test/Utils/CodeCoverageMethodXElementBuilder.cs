// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
