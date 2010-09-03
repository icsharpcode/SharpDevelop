// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.FormsDesigner;
using ICSharpCode.RubyBinding;
using ICSharpCode.Scripting.Tests.Utils;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Designer
{
	/// <summary>
	/// Tests the RubyFormsDesignerDisplayBinding.
	/// </summary>
	[TestFixture]
	public class FormsDesignerDisplayBindingTestFixture
	{
		DerivedRubyFormsDesignerDisplayBinding displayBinding;
		MockTextEditorViewContent viewContent;
		bool canAttachToDesignableClass;
		ParseInformation parseInfo;
		
		[SetUp]
		public void SetUp()
		{
			displayBinding = new DerivedRubyFormsDesignerDisplayBinding();
			viewContent = new MockTextEditorViewContent();
			viewContent.PrimaryFileName =  new FileName("test.rb");
			viewContent.TextEditor.Document.Text = "text content";
			parseInfo = new ParseInformation(new DefaultCompilationUnit(new DefaultProjectContent()));
			displayBinding.ParseServiceParseInfoToReturn = parseInfo;
			displayBinding.IsParseInfoDesignable = true;
			canAttachToDesignableClass = displayBinding.CanAttachTo(viewContent);
		}
		
		[Test]
		public void ReattachWhenParserServiceIsReady()
		{
			Assert.IsTrue(displayBinding.ReattachWhenParserServiceIsReady);
		}
		
		[Test]
		public void CanAttachToNullViewContent()
		{
			Assert.IsFalse(displayBinding.CanAttachTo(null));
		}
		
		[Test]
		public void CanAttachToDesignableClass()
		{
			Assert.IsTrue(canAttachToDesignableClass);
		}
		
		[Test]
		public void CannotAttachToNonTextEditorViewContent()
		{
			MockViewContent viewContent = new MockViewContent();
			Assert.IsFalse(displayBinding.CanAttachTo(viewContent));
		}
		
		[Test]
		public void ParseInfoPassedToFormsDesignerIsDesignableMethod()
		{
			Assert.AreEqual(parseInfo, displayBinding.ParseInfoTestedForDesignability);
		}
		
		[Test]
		public void ParseInfoIsNotDesignable()
		{
			displayBinding.IsParseInfoDesignable = false;
			Assert.IsFalse(displayBinding.CanAttachTo(viewContent));
		}
		
		[Test]
		public void NullViewContentFileName()
		{
			viewContent.PrimaryFileName = null;
			Assert.IsFalse(displayBinding.CanAttachTo(viewContent));
		}
		
		[Test]
		public void FileNamePassedToGetParseInfo()
		{
			Assert.AreEqual("test.rb", displayBinding.FileNamePassedToGetParseInfo);
		}
		
		[Test]
		public void TextContentPassedToGetParseInfo()
		{
			Assert.AreEqual("text content", displayBinding.TextContentPassedToGetParseInfo);
		}
		
		[Test]
		public void NonRubyFileNameCannotBeAttachedTo()
		{
			viewContent.PrimaryFileName = new FileName("test.cs");
			Assert.IsFalse(displayBinding.CanAttachTo(viewContent));
		}
		
		[Test]
		public void NullViewContentPrimaryFileName()
		{
			viewContent.PrimaryFileName = null;
			Assert.IsFalse(displayBinding.CanAttachTo(viewContent));
		}
		
		[Test]
		public void CreatesRubyFormsDesigner()
		{
			MockTextEditorViewContent view = new MockTextEditorViewContent();
			IViewContent[] views = displayBinding.CreateSecondaryViewContent(view, new MockTextEditorOptions());
			Assert.AreEqual(1, views.Length);
			Assert.IsTrue(views[0] is FormsDesignerViewContent);
			views[0].Dispose();
		}
		
		[Test]
		public void FormDesignerNotCreatedIfAlreadyAttached()
		{
			MockTextEditorViewContent view = new MockTextEditorViewContent();			
			IViewContent[] views = null;
			using (FormsDesignerViewContent formsDesigner = new FormsDesignerViewContent(view, new MockOpenedFile("test.rb"))) {
				view.SecondaryViewContents.Add(formsDesigner);
				views = displayBinding.CreateSecondaryViewContent(view, new MockTextEditorOptions());
			}
			Assert.AreEqual(0, views.Length);
		}
	}
}
