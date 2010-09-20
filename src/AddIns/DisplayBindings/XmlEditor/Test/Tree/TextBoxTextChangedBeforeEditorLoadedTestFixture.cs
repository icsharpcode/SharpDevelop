// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Forms;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Tree
{
	/// <summary>
	/// Fixes a null exception that could occur if the TextBoxTextChanged event is fired before the
	/// XmlTreeEditor is created.
	/// </summary>
	[TestFixture]
	public class TextBoxTextChangedBeforeEditorLoadedTestFixture
	{
		[Test]
		public void FireTextBoxTextChangedEventBeforeXmlTreeEditorCreated()
		{
			DerivedXmlTreeViewContainerControl treeViewContainer = new DerivedXmlTreeViewContainerControl();
			treeViewContainer.CallTextBoxTextChanged();
		}
	}
}
