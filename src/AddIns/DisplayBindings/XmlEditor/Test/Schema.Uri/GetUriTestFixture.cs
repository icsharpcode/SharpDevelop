// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 915 $</version>
// </file>

using ICSharpCode.TextEditor.Gui.CompletionWindow;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using System;
using System.IO;
using System.Text;
using System.Xml;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Schema.Uri
{
	/// <summary>
	/// Tests the <see cref="XmlSchemaCompletionData.GetUri"/> method.
	/// </summary>
	[TestFixture]
	public class GetUriTestFixture
	{
		[Test]
		public void SimpleFileName()
		{
			string fileName = @"C:\temp\foo.xml";
			string expectedUri = "file:///C:/temp/foo.xml";
			
			Assert.AreEqual(expectedUri, XmlSchemaCompletionData.GetUri(fileName));
		}
		
		[Test]
		public void NullFileName()
		{
			Assert.AreEqual(String.Empty, XmlSchemaCompletionData.GetUri(null));
		}
		
		[Test]
		public void EmptyString()
		{
			Assert.AreEqual(String.Empty, XmlSchemaCompletionData.GetUri(String.Empty));
		}
	}
}
