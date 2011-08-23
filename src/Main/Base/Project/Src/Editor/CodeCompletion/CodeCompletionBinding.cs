// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
		CodeCompletionKeyPressResult HandleKeyPress(ITextEditor editor, char ch);
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
	/// <usage>Only in /AddIns/DefaultTextEditor/CodeCompletion</usage>
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
		
		public CodeCompletionKeyPressResult HandleKeyPress(ITextEditor editor, char ch)
		{
			string ext = Path.GetExtension(editor.FileName);
			foreach (string extension in extensions) {
				if (ext.Equals(extension, StringComparison.OrdinalIgnoreCase)) {
					if (binding == null) {
						binding = (ICodeCompletionBinding)codon.AddIn.CreateObject(codon.Properties["class"]);
						if (binding == null)
							break;
					}
					return binding.HandleKeyPress(editor, ch);
				}
			}
			return CodeCompletionKeyPressResult.None;
		}
		
		public bool CtrlSpace(ITextEditor editor)
		{
			string ext = Path.GetExtension(editor.FileName);
			foreach (string extension in extensions) {
				if (ext.Equals(extension, StringComparison.OrdinalIgnoreCase)) {
					if (binding == null) {
						binding = (ICodeCompletionBinding)codon.AddIn.CreateObject(codon.Properties["class"]);
						if (binding == null)
							break;
					}
					return binding.CtrlSpace(editor);
				}
			}
			return false;
		}
	}
	
	public class DefaultCodeCompletionBinding : ICodeCompletionBinding
	{
		bool enableMethodInsight = true;
		bool enableIndexerInsight = true;
		bool enableXmlCommentCompletion = true;
		bool enableDotCompletion = true;
		protected IInsightWindowHandler insightHandler;
		
		public bool EnableMethodInsight {
			get {
				return enableMethodInsight;
			}
			set {
				enableMethodInsight = value;
			}
		}
		
		public bool EnableIndexerInsight {
			get {
				return enableIndexerInsight;
			}
			set {
				enableIndexerInsight = value;
			}
		}
		
		public bool EnableXmlCommentCompletion {
			get {
				return enableXmlCommentCompletion;
			}
			set {
				enableXmlCommentCompletion = value;
			}
		}
		
		public bool EnableDotCompletion {
			get {
				return enableDotCompletion;
			}
			set {
				enableDotCompletion = value;
			}
		}
		
		public virtual CodeCompletionKeyPressResult HandleKeyPress(ITextEditor editor, char ch)
		{
			switch (ch) {
				case '(':
					if (enableMethodInsight && CodeCompletionOptions.InsightEnabled) {
						IInsightWindow insightWindow = editor.ShowInsightWindow(new MethodInsightProvider().ProvideInsight(editor));
						if (insightWindow != null && insightHandler != null) {
							insightHandler.InitializeOpenedInsightWindow(editor, insightWindow);
							insightHandler.HighlightParameter(insightWindow, 0);
						}
						return CodeCompletionKeyPressResult.Completed;
					}
					break;
				case '[':
					if (enableIndexerInsight && CodeCompletionOptions.InsightEnabled) {
						IInsightWindow insightWindow = editor.ShowInsightWindow(new IndexerInsightProvider().ProvideInsight(editor));
						if (insightWindow != null && insightHandler != null)
							insightHandler.InitializeOpenedInsightWindow(editor, insightWindow);
						return CodeCompletionKeyPressResult.Completed;
					}
					break;
				case '<':
					if (enableXmlCommentCompletion) {
						new CommentCompletionItemProvider().ShowCompletion(editor);
						return CodeCompletionKeyPressResult.Completed;
					}
					break;
				case '.':
					if (enableDotCompletion) {
						new DotCodeCompletionItemProvider().ShowCompletion(editor);
						return CodeCompletionKeyPressResult.Completed;
					}
					break;
				case ' ':
					if (CodeCompletionOptions.KeywordCompletionEnabled) {
						string word = editor.GetWordBeforeCaret();
						if (!string.IsNullOrEmpty(word)) {
							if (HandleKeyword(editor, word))
								return CodeCompletionKeyPressResult.Completed;
						}
					}
					break;
			}
			return CodeCompletionKeyPressResult.None;
		}
		
		public virtual bool HandleKeyword(ITextEditor editor, string word)
		{
			// DefaultCodeCompletionBinding does not support Keyword handling, but this
			// method can be overridden
			return false;
		}
		
		public virtual bool CtrlSpace(ITextEditor editor)
		{
			return false;
		}
	}
}
