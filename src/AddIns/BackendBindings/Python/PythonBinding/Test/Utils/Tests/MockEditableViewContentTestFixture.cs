// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.AvalonEdit;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Utils.Tests
{
	[TestFixture]
	public class MockEditableViewContentTestFixture
	{
		MockEditableViewContent view;
		
		[SetUp]
		public void Init()
		{
			view = new MockEditableViewContent();
		}
		
		[Test]
		public void ImplementsIEditableInterface()
		{
			Assert.IsNotNull(view as IEditable);
		}
		
		[Test]
		public void ImplementsITextEditorProviderInterface()
		{
			Assert.IsNotNull(view as ITextEditorProvider);
		}
		
		[Test]
		public void CanReplaceTextEditorOptions()
		{
			MockTextEditorOptions options = new MockTextEditorOptions();
			view.MockTextEditorOptions = options;
			Assert.AreSame(options, view.TextEditor.Options);
		}
	}
}
