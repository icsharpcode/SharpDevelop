// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
