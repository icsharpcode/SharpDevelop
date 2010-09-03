// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Windows.Controls;
using System.Xml;
using ICSharpCode.Core;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Editor
{
	[TestFixture]
	public class EmptyXmlSchemasPanelTestFixture
	{
		MockXmlSchemasPanel panel;
		RegisteredXmlSchemasEditor schemasEditor;
		
		[SetUp]
		public void Init()
		{
			RegisteredXmlSchemas registeredXmlSchemas = new RegisteredXmlSchemas(new string[0], String.Empty, null, null);
			XmlSchemaFileAssociations associations = new XmlSchemaFileAssociations(new Properties(), new DefaultXmlSchemaFileAssociations(null), registeredXmlSchemas.Schemas);
			panel = new MockXmlSchemasPanel();
			schemasEditor = new RegisteredXmlSchemasEditor(registeredXmlSchemas, new string[0], associations, panel, null);
			schemasEditor.LoadOptions();
		}
		
		[Test]
		public void RemoveSchemaButtonIsDisabled()
		{
			Assert.IsFalse(panel.RemoveSchemaButtonEnabled);
		}
	}
}
