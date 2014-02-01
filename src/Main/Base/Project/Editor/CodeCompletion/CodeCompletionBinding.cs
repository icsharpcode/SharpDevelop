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
using System.IO;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Editor.CodeCompletion
{
	/// <summary>
	/// Interface that gives backend bindings the possibility to control what characters and
	/// keywords invoke code completion.
	/// </summary>
	public interface ICodeCompletionBinding
	{
		/// <summary>
		/// This method is called when typing a character in the editor, immediately before
		/// the character is inserted into the document.
		/// </summary>
		/// <param name="editor">The editor</param>
		/// <param name="ch">The character being inserted.</param>
		/// <returns>Returns whether the completion binding has shown code completion.</returns>
		CodeCompletionKeyPressResult HandleKeyPress(ITextEditor editor, char ch);
		
		/// <summary>
		/// This method is called after typing a character in the editor,
		/// after character is inserted into the document.
		/// </summary>
		/// <param name="editor">The editor</param>
		/// <param name="ch">The character that was inserted.</param>
		/// <returns>Returns whether the completion binding has shown code completion.</returns>
		bool HandleKeyPressed(ITextEditor editor, char ch);
		
		/// <summary>
		/// Invokes ctrl-space completion.
		/// </summary>
		/// <returns>Returns whether the completion binding has shown code completion.</returns>
		bool CtrlSpace(ITextEditor editor);
	}
	
	/// <summary>
	/// The result of <see cref="ICodeCompletionBinding.HandleKeyPress"/>.
	/// </summary>
	public enum CodeCompletionKeyPressResult
	{
		/// <summary>
		/// The binding did not run code completion. The pressed key will be handled normally.
		/// </summary>
		None,
		/// <summary>
		/// The binding handled code completion, the pressed key will be handled normally.
		/// The pressed key will not be included in the completion expression, but will be
		/// in front of it (this is usually used when the key is '.').
		/// </summary>
		Completed,
		/// <summary>
		/// The binding handled code completion, the pressed key will be handled normally.
		/// The pressed key will be included in the completion expression.
		/// This is used when starting to type any character starts code completion.
		/// </summary>
		CompletedIncludeKeyInCompletion,
		/// <summary>
		/// The binding handled code completion, and the key will not be handled by the text editor.
		/// </summary>
		EatKey
	}
	
	/// <summary>
	/// Creates code completion bindings that manage code completion for one language.
	/// </summary>
	/// <attribute name="class" use="required">
	/// Name of the ICodeCompletionBinding class (normally deriving from DefaultCodeCompletionBinding).
	/// </attribute>
	/// <attribute name="extensions" use="optional">
	/// List of semicolon-separated entries of the file extensions handled by the binding.
	/// If no extensions attribute is specified, the binding is activated in all files.
	/// </attribute>
	/// <usage>Only in /SharpDevelop/ViewContent/TextEditor/CodeCompletion</usage>
	/// <returns>
	/// The ICodeCompletionBinding class specified with the 'class' attribute, or a
	/// wrapper that lazy-loads the actual class when it is used in a file with the specified
	/// extension.
	/// </returns>
	public class CodeCompletionBindingDoozer : IDoozer
	{
		public bool HandleConditions {
			get {
				return false;
			}
		}
		
		public object BuildItem(BuildItemArgs args)
		{
			string ext = args.Codon["extensions"];
			if (ext != null && ext.Length > 0)
				return new LazyCodeCompletionBinding(args.Codon, ext.Split(';'));
			else
				return args.AddIn.CreateObject(args.Codon["class"]);
		}
	}
	
	public sealed class LazyCodeCompletionBinding : ICodeCompletionBinding
	{
		Codon codon;
		string[] extensions;
		ICodeCompletionBinding binding;
		
		public LazyCodeCompletionBinding(Codon codon, string[] extensions)
		{
			this.codon = codon;
			this.extensions = extensions;
		}
		
		bool MatchesExtension(ITextEditor editor)
		{
			string ext = Path.GetExtension(editor.FileName);
			foreach (string extension in extensions) {
				if (ext.Equals(extension, StringComparison.OrdinalIgnoreCase)) {
					if (binding == null) {
						binding = (ICodeCompletionBinding)codon.AddIn.CreateObject(codon.Properties["class"]);
						if (binding == null)
							return false;
					}
					return true;
				}
			}
			return false;
		}
		
		public CodeCompletionKeyPressResult HandleKeyPress(ITextEditor editor, char ch)
		{
			if (MatchesExtension(editor))
				return binding.HandleKeyPress(editor, ch);
			else
				return CodeCompletionKeyPressResult.None;
		}
		
		public bool HandleKeyPressed(ITextEditor editor, char ch)
		{
			if (MatchesExtension(editor))
				return binding.HandleKeyPressed(editor, ch);
			else
				return false;
		}
		
		public bool CtrlSpace(ITextEditor editor)
		{
			if (MatchesExtension(editor))
				return binding.CtrlSpace(editor);
			else
				return false;
		}
	}
}
