// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.XmlEditor;
using NUnit.Framework;

namespace XmlEditor.Tests.Editor
{
	[TestFixture]
	public class NullXmlEditorFileExtensionsAddInTreeNodeTestFixture
	{
		[Test]
		public void EmptyFileExtensionListReturnedWhenAddInTreeNodeIsNull()
		{
			DefaultXmlFileExtensions fileExtensions = new DefaultXmlFileExtensions(null);
			Assert.AreEqual(0, fileExtensions.Count);
		}
	}
}
