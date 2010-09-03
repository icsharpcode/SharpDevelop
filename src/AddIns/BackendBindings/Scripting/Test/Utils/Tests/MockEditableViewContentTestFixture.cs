// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.AvalonEdit;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;
using NUnit.Framework;
using ICSharpCode.Scripting.Tests.Utils;

namespace ICSharpCode.Scripting.Tests.Utils.Tests
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
