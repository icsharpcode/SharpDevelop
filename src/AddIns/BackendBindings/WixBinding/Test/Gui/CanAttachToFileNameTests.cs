// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
			view.SetName("Setup.wxs");
			Assert.IsTrue(binding.CanAttachTo(view));
		}
		
		[Test]
		public void WixIncludeFileName()
		{
			MockTextEditorViewContent view = new MockTextEditorViewContent();
			view.SetName("Setup.wxi");
			Assert.IsTrue(binding.CanAttachTo(view));
		}
		
		[Test]
		public void WixFileNameUppercase()
		{
			MockTextEditorViewContent view = new MockTextEditorViewContent();
			view.SetName("SETUP.WXS");
			Assert.IsTrue(binding.CanAttachTo(view));
		}
		
		[Test]
		public void NonWixFileName()
		{
			MockTextEditorViewContent view = new MockTextEditorViewContent();
			view.SetName("Setup.txt");
			Assert.IsFalse(binding.CanAttachTo(view));
		}
		
		[Test]
		public void NonTextEditorProviderView()
		{
			MockViewContent view = new MockViewContent();
			view.SetName("Setup.wxs");
			Assert.IsFalse(binding.CanAttachTo(view));
		}
		
		[Test]
		public void UntitledWixFileName()
		{
			MockTextEditorViewContent view = new MockTextEditorViewContent();
			view.SetUntitledName("Setup.wxs");
			Assert.IsTrue(binding.CanAttachTo(view));
		}
		
		[Test]
		public void UntitledNonWixFileName()
		{
			MockTextEditorViewContent view = new MockTextEditorViewContent();
			view.SetUntitledName("Setup.txt");
			Assert.IsFalse(binding.CanAttachTo(view));
		}
		
		[Test]
		public void NullUntitledFileName()
		{
			MockTextEditorViewContent view = new MockTextEditorViewContent();
			view.SetUntitledName(null);
			Assert.IsFalse(binding.CanAttachTo(view));			
		}
		
		[Test]
		public void NullFileName()
		{
			MockTextEditorViewContent view = new MockTextEditorViewContent();
			view.SetName(null);
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
