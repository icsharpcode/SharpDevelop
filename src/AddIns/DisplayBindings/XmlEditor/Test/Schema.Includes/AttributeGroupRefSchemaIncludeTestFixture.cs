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

using ICSharpCode.XmlEditor;
using NUnit.Framework;
using System;
using System.IO;
using System.Text;
using System.Xml;
using XmlEditor.Tests.Utils;
using XmlEditor.Tests.Schema;

namespace XmlEditor.Tests.Schema.Includes
{
	/// <summary>
	/// Tests that schemas referenced via xs:include elements are used when
	/// generating completion data.
	/// </summary>
	[TestFixture]
	public class AttributeGroupRefSchemaIncludeTestFixture : AttributeGroupRefTestFixture
	{
		[TestFixtureTearDown]
		public void FixtureTearDown()
		{
			SchemaIncludeTestFixtureHelper.FixtureTearDown();
		}
		
		protected override XmlSchemaCompletion CreateSchemaCompletionObject()
		{	
			return SchemaIncludeTestFixtureHelper.CreateSchemaCompletionDataObject(GetMainSchema(), GetSchema());
		}
		
		string GetMainSchema()
		{
			return "<xs:schema \r\n" +
				"targetNamespace=\"http://www.w3schools.com\" \r\n" +
				"xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" \r\n" +
				"elementFormDefault=\"qualified\">\r\n" +
				"\t<xs:include schemaLocation=\"include.xsd\"/>\r\n" +
				"</xs:schema>";
		}
	}
}
