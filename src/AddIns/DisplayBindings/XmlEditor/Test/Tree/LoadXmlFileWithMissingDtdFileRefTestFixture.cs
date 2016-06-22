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
using System.Xml;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Tree
{
	/// <summary>
	/// If the XML loaded contains a DTD reference then if the XmlDocument.XmlResolver is not 
	/// set to null then the XmlTreeEditor will throw an unhandled FileNotFoundException.
	/// </summary>
	[TestFixture]
	public class LoadXmlFileWithMissingDtdFileRefTestFixture : XmlTreeViewTestFixtureBase
	{		
		[SetUp]
		public void Init()
		{
			base.InitFixture();
		}

		[Test]
		public void RootElementIsLoaded()
		{
			Assert.AreEqual("Library", editor.Document.DocumentElement.Name);
		}
		
		/// <summary>
		/// Returns the xhtml strict schema as the default schema.
		/// </summary>
		protected override XmlSchemaCompletion DefaultSchemaCompletion {
			get { return new XmlSchemaCompletion(ResourceManager.ReadXhtmlStrictSchema()); }
		}
		
		protected override string GetXml()
		{
			return "<!DOCTYPE Library  SYSTEM \"Library.dtd\">\r\n" +
				"<Library> \r\n" +
				"   <Book  ISBN=\"9999-44-44-3333\" > \r\n" +
				"        <Title>SharpDevelop</Title> \r\n" +
				"        <Author>abc</Author>\r\n" +
				"        <Publisher>SharpDevelop Publications</Publisher>\r\n" +
				"        <Date_Published>22/10/1999</Date_Published>\r\n" +
				"   </Book>\r\n" +
				"</Library>";
		}
	}
}
