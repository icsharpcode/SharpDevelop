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
		bool completionDataProviderCreated;
		bool codeCompletionWindowDisplayed;
		SharpDevelopTextAreaControl textAreaControlUsedToShowCompletionWindow;
		ICompletionDataProvider completionProviderUsedWhenDisplayingCodeCompletionWindow;
		ICompletionDataProvider completionDataProvider;
		char completionCharacter = '\0';
		
		public DerivedPythonCodeCompletionBinding()
		{
		}
		
		/// <summary>
		/// Gets whether the data provider was created by the 
		/// base class PythonCodeCompletionBinding.
		/// </summary>
		public bool IsCompletionDataProviderCreated {
			get { return completionDataProviderCreated; }
		}
		
		/// <summary>
		/// Gets whether the base class PythonCodeCompletionBinding
		/// displayed the code completion window.
		/// </summary>
		public bool IsCodeCompletionWindowDisplayed {
			get { return codeCompletionWindowDisplayed; }
		}
		
		public SharpDevelopTextAreaControl TextAreaControlUsedToShowCompletionWindow {
			get { return textAreaControlUsedToShowCompletionWindow; }
		}
		
		public ICompletionDataProvider CompletionProviderUsedWhenDisplayingCodeCompletionWindow {
			get { return completionProviderUsedWhenDisplayingCodeCompletionWindow; }
		}
		
		/// <summary>
		/// Gets the CompletionDataProvider created via the
		/// CreateCompletionDataProvider method.
		/// </summary>
		public ICompletionDataProvider CompletionDataProvider {
			get { return completionDataProvider; }
		}
		
		/// <summary>
		/// Gets the character used when calling the TextAreaControl's 
		/// ShowCompletionWindow method.
		/// </summary>
		public char CompletionCharacter {
			get { return completionCharacter; }
		}
		
		/// <summary>
		/// Overrides the completion data provider creation to make sure
		/// it is called at the correct time. 
		/// </summary>
		protected override ICompletionDataProvider CreateCompletionDataProvider()
		{
			completionDataProviderCreated = true;
			completionDataProvider = base.CreateCompletionDataProvider();
			return completionDataProvider;
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
