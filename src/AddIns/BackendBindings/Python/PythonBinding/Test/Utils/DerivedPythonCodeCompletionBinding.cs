// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Dom;
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
		SharpDevelopTextAreaControl textAreaControlUsedToShowCompletionWindow;
		ICompletionDataProvider completionProviderUsedWhenDisplayingCodeCompletionWindow;
		CtrlSpaceCompletionDataProvider ctrlSpaceCompletionDataProvider;
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
			get {
				return ctrlSpaceCompletionDataProviderCreated;
			}
		}
		
		/// <summary>
		/// Gets whether the base class PythonCodeCompletionBinding
		/// displayed the code completion window.
		/// </summary>
		public bool IsCodeCompletionWindowDisplayed {
			get {
				return codeCompletionWindowDisplayed;
			}
		}
		
		public SharpDevelopTextAreaControl TextAreaControlUsedToShowCompletionWindow {
			get {
				return textAreaControlUsedToShowCompletionWindow;
			}
		}
		
		public ICompletionDataProvider CompletionProviderUsedWhenDisplayingCodeCompletionWindow {
			get {
				return completionProviderUsedWhenDisplayingCodeCompletionWindow;
			}
		}
		
		/// <summary>
		/// Gets the CtrlSpaceCompletionDataProvider created via the
		/// CreateCtrlSpaceCompletionDataProvider method.
		/// </summary>
		public CtrlSpaceCompletionDataProvider CtrlSpaceCompletionDataProvider {
			get {
				return ctrlSpaceCompletionDataProvider;
			}
		}
		
		/// <summary>
		/// Gets the character used when calling the TextAreaControl's 
		/// ShowCompletionWindow method.
		/// </summary>
		public char CompletionCharacter {
			get {
				return completionCharacter;
			}
		}
		
		/// <summary>
		/// Gets the expression context used when the
		/// CtrlSpaceCompletionDataProvider is created.
		/// </summary>
		public ExpressionContext ExpressionContext {
			get {
				return expressionContext;
			}
		}
		
		/// <summary>
		/// Overrides the completion data provider creation to make sure
		/// it is called at the correct time. 
		/// </summary>
		protected override CtrlSpaceCompletionDataProvider CreateCtrlSpaceCompletionDataProvider(ExpressionContext expressionContext)
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
		protected override void ShowCodeCompletionWindow(SharpDevelopTextAreaControl textAreaControl, ICompletionDataProvider completionDataProvider, char ch)
		{
			textAreaControlUsedToShowCompletionWindow = textAreaControl;
			codeCompletionWindowDisplayed = true;
			completionCharacter = ch;
			completionProviderUsedWhenDisplayingCodeCompletionWindow = completionDataProvider;
		}
	}
}
