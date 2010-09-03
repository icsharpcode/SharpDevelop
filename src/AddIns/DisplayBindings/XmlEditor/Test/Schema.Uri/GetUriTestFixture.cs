// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
			
			Assert.AreEqual(expectedUri, XmlSchemaCompletion.GetUri(fileName));
		}
		
		[Test]
		public void NullFileName()
		{
			Assert.AreEqual(String.Empty, XmlSchemaCompletion.GetUri(null));
		}
		
		[Test]
		public void EmptyString()
		{
			Assert.AreEqual(String.Empty, XmlSchemaCompletion.GetUri(String.Empty));
		}
	}
}
