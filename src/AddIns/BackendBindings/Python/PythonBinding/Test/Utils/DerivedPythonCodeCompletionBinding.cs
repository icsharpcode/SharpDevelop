// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using System;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.TextEditor.Gui.CompletionWindow;

namespace PythonBinding.Tests.Utils
{
	/// <summary>
	/// Derived PythonCodeCompletion class that gives us access to 
	/// various protected methods for testing.
	/// </summary>
	public class DerivedPythonCodeCompletionBinding : PythonCodeCompletionBinding
	{
		bool ctrlSpaceCompletionDataProviderCreated;
		bool codeCompletionWindowDisplayed;
		ICSharpCode.SharpDevelop.Editor.ITextEditor textEditorUsedToShowCompletionWindow;
		AbstractCompletionItemProvider completionProviderUsedWhenDisplayingCodeCompletionWindow;
		CtrlSpaceCompletionItemProvider ctrlSpaceCompletionDataProvider;
		char completionCharacter = '\0';
		ExpressionContext expressionContext;
		
		public DerivedPythonCodeCompletionBinding()
		{
		}
		
		/// <summary>
		/// Gets whether the data provider was created by the 
		/// base class PythonCodeCompletionBinding.
		/// </summary>
		public bool IsCtrlSpaceCompletionDataProviderCreated {
			get { return ctrlSpaceCompletionDataProviderCreated; }
		}
		
		/// <summary>
		/// Gets whether the base class PythonCodeCompletionBinding
		/// displayed the code completion window.
		/// </summary>
		public bool IsCodeCompletionWindowDisplayed {
			get { return codeCompletionWindowDisplayed; }
		}
		
		public ICSharpCode.SharpDevelop.Editor.ITextEditor TextEditorUsedToShowCompletionWindow {
			get { return textEditorUsedToShowCompletionWindow; }
		}
		
		public AbstractCompletionItemProvider CompletionProviderUsedWhenDisplayingCodeCompletionWindow {
			get { return completionProviderUsedWhenDisplayingCodeCompletionWindow; }
		}
		
		/// <summary>
		/// Gets the CtrlSpaceCompletionDataProvider created via the
		/// CreateCtrlSpaceCompletionDataProvider method.
		/// </summary>
		public CtrlSpaceCompletionItemProvider CtrlSpaceCompletionDataProvider {
			get { return ctrlSpaceCompletionDataProvider; }
		}
		
		/// <summary>
		/// Gets the character used when calling the TextAreaControl's 
		/// ShowCompletionWindow method.
		/// </summary>
		public char CompletionCharacter {
			get { return completionCharacter; }
		}
		
		/// <summary>
		/// Gets the expression context used when the
		/// CtrlSpaceCompletionDataProvider is created.
		/// </summary>
		public ExpressionContext ExpressionContext {
			get { return expressionContext; }
		}
		
		/// <summary>
		/// Overrides the completion data provider creation to make sure
		/// it is called at the correct time. 
		/// </summary>
		protected override CtrlSpaceCompletionItemProvider CreateCtrlSpaceCompletionDataProvider(ExpressionContext expressionContext)
		{
			ctrlSpaceCompletionDataProviderCreated = true;
			this.expressionContext = expressionContext;
			ctrlSpaceCompletionDataProvider = base.CreateCtrlSpaceCompletionDataProvider(expressionContext);
			return ctrlSpaceCompletionDataProvider;
		}
		
		/// <summary>
		/// Overrides the base class method so a code completion window is
		/// not displayed but the fact that this method is called is
		/// recorded. 
		/// </summary>
		protected override void ShowCodeCompletionWindow(ICSharpCode.SharpDevelop.Editor.ITextEditor textEditor, AbstractCompletionItemProvider completionDataProvider, char ch)
		{
			textEditorUsedToShowCompletionWindow = textEditor;
			codeCompletionWindowDisplayed = true;
			completionCharacter = ch;
			completionProviderUsedWhenDisplayingCodeCompletionWindow = completionDataProvider;
		}
	}
}
