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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using ICSharpCode.XmlEditor;

namespace XmlEditor.Tests.Utils
{
	public class MockXmlSchemaCompletionDataFactory : IXmlSchemaCompletionDataFactory
	{
		Dictionary<string, string> schemaXml = new Dictionary<string, string>();
		Dictionary<string, string> baseUris = new Dictionary<string, string>();
		Exception exceptionToThrowWhenCreateXmlSchemaCalled;
		
		public MockXmlSchemaCompletionDataFactory()
		{
		}
		
		public XmlSchemaCompletion CreateXmlSchemaCompletionData(string baseUri, string fileName)
		{
			baseUris.Add(fileName, baseUri);
			
			if (exceptionToThrowWhenCreateXmlSchemaCalled != null) {
				throw exceptionToThrowWhenCreateXmlSchemaCalled;
			}
			
			TextReader reader = CreateTextReader(fileName);
			return new XmlSchemaCompletion(reader);
		}
		
		public Exception ExceptionToThrowWhenCreateXmlSchemaCalled {
			get { return exceptionToThrowWhenCreateXmlSchemaCalled; }
			set { exceptionToThrowWhenCreateXmlSchemaCalled = value; }
		}
		
		
		TextReader CreateTextReader(string fileName)
		{
			string xml;
			if (schemaXml.TryGetValue(fileName, out xml)) {
				return new StringReader(xml);
			}
			return new StringReader("<a/>");
		}	
		
		public string GetBaseUri(string fileName)
		{
			string baseUri;
			if (baseUris.TryGetValue(fileName, out baseUri)) {
				return baseUri;
			}
			return null;
		}		
		
		public void AddSchemaXml(string fileName, string xml)
		{
			schemaXml.Add(fileName, xml);
		}
	}
}
