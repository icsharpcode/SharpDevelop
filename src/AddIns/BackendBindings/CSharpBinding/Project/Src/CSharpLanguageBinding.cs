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
using System.Collections.Generic;

using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using CSharpBinding.Completion;
using CSharpBinding.FormattingStrategy;
using CSharpBinding.Refactoring;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Refactoring;

namespace CSharpBinding
{
	/// <summary>
	/// Description of CSharpLanguageBinding.
	/// </summary>
	public class CSharpLanguageBinding : DefaultLanguageBinding
	{
		public CSharpLanguageBinding()
		{
			this.container.AddService(typeof(IFormattingStrategy), new CSharpFormattingStrategy());
			this.container.AddService(typeof(IBracketSearcher), new CSharpBracketSearcher());
			this.container.AddService(typeof(CodeGenerator), new CSharpCodeGenerator());
			this.container.AddService(typeof(System.CodeDom.Compiler.CodeDomProvider), new Microsoft.CSharp.CSharpCodeProvider());
		}
		
		public override ICodeCompletionBinding CreateCompletionBinding(FileName fileName, TextLocation currentLocation, ICSharpCode.NRefactory.Editor.ITextSource fileContent)
		{
			if (fileName == null)
				throw new ArgumentNullException("fileName");
			return new CSharpCompletionBinding(fileName, currentLocation, fileContent);
		}
	}
	
	public class CSharpTextEditorExtension : ITextEditorExtension
	{
		ITextEditor editor;
		IssueManager inspectionManager;
		IList<IContextActionProvider> contextActionProviders;
		CodeManipulation codeManipulation;
		
		public void Attach(ITextEditor editor)
		{
			this.editor = editor;
			inspectionManager = new IssueManager(editor);
			codeManipulation = new CodeManipulation(editor);
			
			if (!editor.ContextActionProviders.IsReadOnly) {
				contextActionProviders = AddInTree.BuildItems<IContextActionProvider>("/SharpDevelop/ViewContent/TextEditor/C#/ContextActions", null);
				editor.ContextActionProviders.AddRange(contextActionProviders);
			}
		}
		
		public void Detach()
		{
			codeManipulation.Dispose();
			if (inspectionManager != null) {
				inspectionManager.Dispose();
				inspectionManager = null;
			}
			if (contextActionProviders != null) {
				editor.ContextActionProviders.RemoveAll(contextActionProviders.Contains);
			}
			this.editor = null;
		}
	}
}
