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
