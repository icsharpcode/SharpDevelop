// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.FormsDesigner;
using ICSharpCode.RubyBinding;
using ICSharpCode.Scripting.Tests.Utils;
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
		protected MockTextEditorOptions textEditorOptions;

		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			textEditorOptions = new MockTextEditorOptions();
			generator = new DerivedRubyDesignerGenerator(textEditorOptions);
			mockViewContent = new MockTextEditorViewContent();
			viewContent = new DerivedFormDesignerViewContent(mockViewContent, new MockOpenedFile(fileName));
			generator.Attach(viewContent);
			viewContent.DesignerCodeFileContent = GetTextEditorCode();
			
			RubyParser parser = new RubyParser();
			ICompilationUnit parserCompilationUnit = parser.Parse(new DefaultProjectContent(), fileName, GetTextEditorCode());
			ParseInformation parseInfo = new ParseInformation(parserCompilationUnit);
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
