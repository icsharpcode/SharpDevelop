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

using System;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.WixBinding;
using NUnit.Framework;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.Gui
{
	/// <summary>
	/// Tests that the WixDialogDesignerDisplayBinding can attach to
	/// Wix documents.
	/// </summary>
	[TestFixture]
	public class CanAttachToFileNameTestFixture
	{	
		WixDialogDesignerDisplayBinding binding;
		
		[SetUp]
		public void Init()
		{
			binding = new WixDialogDesignerDisplayBinding();
		}
		
		[Test]
		public void WixFileName()
		{
			MockTextEditorViewContent view = new MockTextEditorViewContent();
			view.SetFileName("Setup.wxs");
			Assert.IsTrue(binding.CanAttachTo(view));
		}
		
		[Test]
		public void WixIncludeFileName()
		{
			MockTextEditorViewContent view = new MockTextEditorViewContent();
			view.SetFileName("Setup.wxi");
			Assert.IsTrue(binding.CanAttachTo(view));
		}
		
		[Test]
		public void WixFileNameUppercase()
		{
			MockTextEditorViewContent view = new MockTextEditorViewContent();
			view.SetFileName("SETUP.WXS");
			Assert.IsTrue(binding.CanAttachTo(view));
		}
		
		[Test]
		public void NonWixFileName()
		{
			MockTextEditorViewContent view = new MockTextEditorViewContent();
			view.SetFileName("Setup.txt");
			Assert.IsFalse(binding.CanAttachTo(view));
		}
		
		[Test]
		public void NonTextEditorProviderView()
		{
			MockViewContent view = new MockViewContent();
			view.SetFileName("Setup.wxs");
			Assert.IsFalse(binding.CanAttachTo(view));
		}
		
		[Test]
		public void UntitledWixFileName()
		{
			MockTextEditorViewContent view = new MockTextEditorViewContent();
			view.SetUntitledFileName("Setup.wxs");
			Assert.IsTrue(binding.CanAttachTo(view));
		}
		
		[Test]
		public void UntitledNonWixFileName()
		{
			MockTextEditorViewContent view = new MockTextEditorViewContent();
			view.SetUntitledFileName("Setup.txt");
			Assert.IsFalse(binding.CanAttachTo(view));
		}
		
		[Test]
		public void NullUntitledFileName()
		{
			MockTextEditorViewContent view = new MockTextEditorViewContent();
			view.SetUntitledFileName(null);
			Assert.IsFalse(binding.CanAttachTo(view));			
		}
		
		[Test]
		public void NullFileName()
		{
			MockTextEditorViewContent view = new MockTextEditorViewContent();
			view.SetFileName(null);
			Assert.IsFalse(binding.CanAttachTo(view));			
		}
		
		[Test]
		public void ReattachIsFalse()
		{
			Assert.IsFalse(binding.ReattachWhenParserServiceIsReady);
		}
	}
}
