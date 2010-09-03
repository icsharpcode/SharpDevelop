// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;
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
		
		[Test]
		public void CreatesWixDialogDesigner()
		{
			MockTextEditorViewContent view = new MockTextEditorViewContent();
			IViewContent[] views = binding.CreateSecondaryViewContent(view);
			Assert.AreEqual(1, views.Length);
			Assert.IsTrue(views[0] is WixDialogDesigner);
			views[0].Dispose();
		}
	}
}
