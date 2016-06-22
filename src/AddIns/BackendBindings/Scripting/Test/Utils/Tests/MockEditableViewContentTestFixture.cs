// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

//using System;
//using ICSharpCode.AvalonEdit;
//using ICSharpCode.SharpDevelop;
//using ICSharpCode.SharpDevelop.Editor;
//using ICSharpCode.SharpDevelop.Gui;
//using NUnit.Framework;
//using ICSharpCode.Scripting.Tests.Utils;
//
//namespace ICSharpCode.Scripting.Tests.Utils.Tests
//{
//	[TestFixture]
//	public class MockEditableViewContentTestFixture
//	{
//		MockEditableViewContent view;
//		
//		[SetUp]
//		public void Init()
//		{
//			view = new MockEditableViewContent();
//		}
//		
//		[Test]
//		public void ImplementsIEditableInterface()
//		{
//			Assert.IsNotNull(view as IEditable);
//		}
//		
//		[Test]
//		public void ImplementsITextEditorProviderInterface()
//		{
//			Assert.IsNotNull(view as ITextEditorProvider);
//		}
//		
//		[Test]
//		public void CanReplaceTextEditorOptions()
//		{
//			MockTextEditorOptions options = new MockTextEditorOptions();
//			view.MockTextEditorOptions = options;
//			Assert.AreSame(options, view.TextEditor.Options);
//		}
//	}
//}
