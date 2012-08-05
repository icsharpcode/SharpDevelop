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
			MethodElement = new XElement(XName.Get("Method"));
			
			var nameElement = new XElement(XName.Get("Name"));
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
			MethodElement.SetAttributeValue(name, value.ToString().ToLowerInvariant());
		}
		
		public void MakePropertySetter()
		{
			SetSetterAttribute(true);
		}
		
		void SetSetterAttribute(bool value)
		{
			SetBooleanAttribute("isSetter", value);
		}
		
		public static XElement CreateSetterMethod(string methodSignature)
		{
			var builder = new CodeCoverageMethodXElementBuilder(methodSignature);
			builder.SetSetterAttribute(true);
			builder.SetGetterAttribute(false);
			return builder.MethodElement;
		}
		
		public static XElement CreateGetterMethod(string methodSignature)
		{
			var builder = new CodeCoverageMethodXElementBuilder(methodSignature);
			builder.SetSetterAttribute(false);
			builder.SetGetterAttribute(true);
			return builder.MethodElement;
		}
	}
}
