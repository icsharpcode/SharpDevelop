// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
