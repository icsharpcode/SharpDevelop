// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.RubyBinding;
using ICSharpCode.FormsDesigner;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Designer
{
	/// <summary>
	/// Base class that tests the RubyDesignerGenerator.InsertEventComponent method.
	/// </summary>
	public class InsertEventHandlerTestFixtureBase
	{
		protected string file;
		protected int position;
		protected bool insertedEventHandler;
		protected MockTextEditorViewContent mockViewContent;
		protected DerivedFormDesignerViewContent viewContent;
		protected string fileName = @"C:\Projects\Ruby\mainform.rb";
		protected DerivedRubyDesignerGenerator generator;
		protected MockTextEditorProperties textEditorProperties;

		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			textEditorProperties = new MockTextEditorProperties();
			generator = new DerivedRubyDesignerGenerator(textEditorProperties);
			mockViewContent = new MockTextEditorViewContent();
			viewContent = new DerivedFormDesignerViewContent(mockViewContent, new MockOpenedFile(fileName));
			generator.Attach(viewContent);
			viewContent.DesignerCodeFileContent = GetTextEditorCode();
			
			ParseInformation parseInfo = new ParseInformation();
			RubyParser parser = new RubyParser();
			ICompilationUnit parserCompilationUnit = parser.Parse(new DefaultProjectContent(), fileName, GetTextEditorCode());
			parseInfo.SetCompilationUnit(parserCompilationUnit);
			generator.ParseInfoToReturnFromParseFileMethod = parseInfo;
			
			AfterSetUpFixture();
		}
		
		/// <summary>
		/// Called at the end of the SetUpFixture method.
		/// </summary>
		public virtual void AfterSetUpFixture()
		{
		}
		
		/// <summary>
		/// Gets the form's code.
		/// </summary>
		protected virtual string GetTextEditorCode()
		{
			return String.Empty;
		}
	}
}
