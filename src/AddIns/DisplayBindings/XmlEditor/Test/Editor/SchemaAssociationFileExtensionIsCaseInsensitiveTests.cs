// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.XmlEditor;
using NUnit.Framework;

namespace XmlEditor.Tests.Editor
{
	[TestFixture]
	public class SchemaAssociationFileExtensionIsCaseInsensitiveTests
	{
		[Test]
		public void FileExtensionIsLowerCased()
		{
			XmlSchemaFileAssociation schemaAssociation = new XmlSchemaFileAssociation(".XML", String.Empty);
			Assert.AreEqual(".xml", schemaAssociation.FileExtension);
		}
	}
}
