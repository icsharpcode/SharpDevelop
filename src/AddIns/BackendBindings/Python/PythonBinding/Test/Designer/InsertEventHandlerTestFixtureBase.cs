// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.FormsDesigner;
using ICSharpCode.PythonBinding;
using ICSharpCode.Scripting.Tests.Utils;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Designer
{
	/// <summary>
	/// Base class that tests the PythonDesignerGenerator.InsertEventComponent method.
	/// </summary>
	public class InsertEventHandlerTestFixtureBase
	{
		protected string file;
		protected int position;
		protected bool insertedEventHandler;
		protected MockTextEditorViewContent mockViewContent;
		protected DerivedFormDesignerViewContent viewContent;
		protected string fileName = @"C:\Projects\Python\mainform.py";
		protected DerivedPythonDesignerGenerator generator;
		protected MockTextEditorOptions textEditorOptions;

		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			textEditorOptions = new MockTextEditorOptions();
			generator = new DerivedPythonDesignerGenerator(textEditorOptions);
			mockViewContent = new MockTextEditorViewContent();
			viewContent = new DerivedFormDesignerViewContent(mockViewContent, new MockOpenedFile(fileName));
			generator.Attach(viewContent);
			viewContent.DesignerCodeFileContent = GetTextEditorCode();
			
			PythonParser parser = new PythonParser();
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
