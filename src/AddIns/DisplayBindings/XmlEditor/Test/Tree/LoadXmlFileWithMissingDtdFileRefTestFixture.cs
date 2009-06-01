// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
		protected override XmlSchemaCompletionData DefaultSchemaCompletionData {
			get {
				XmlTextReader reader = ResourceManager.GetXhtmlStrictSchema();
				return new XmlSchemaCompletionData(reader);
			}
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
