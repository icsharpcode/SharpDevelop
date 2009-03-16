// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.IO;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor
{
	/// <summary>
	/// Interface that gives backend bindings the possibility to control what characters and
	/// keywords invoke code completion.
	/// </summary>
	public interface ICodeCompletionBinding
	{
		bool HandleKeyPress(SharpDevelopTextAreaControl editor, char ch);
		bool CtrlSpace(SharpDevelopTextAreaControl editor);
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
		
		public object BuildItem(object caller, Codon codon, ArrayList subItems)
		{
			string ext = codon.Properties["extensions"];
			if (ext != null && ext.Length > 0)
				return new LazyCodeCompletionBinding(codon, ext.Split(';'));
			else
				return codon.AddIn.CreateObject(codon.Properties["class"]);
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
		
		public bool HandleKeyPress(SharpDevelopTextAreaControl editor, char ch)
		{
			string ext = Path.GetExtension(editor.FileName);
			foreach (string extension in extensions) {
				if (ext.Equals(extension, StringComparison.OrdinalIgnoreCase)) {
					if (binding == null) {
						binding = (ICodeCompletionBinding)codon.AddIn.CreateObject(codon.Properties["class"]);
					}
					return binding.HandleKeyPress(editor, ch);
				}
			}
			return false;
		}
		
		public bool CtrlSpace(SharpDevelopTextAreaControl editor)
		{
			string ext = Path.GetExtension(editor.FileName);
			foreach (string extension in extensions) {
				if (ext.Equals(extension, StringComparison.OrdinalIgnoreCase)) {
					if (binding == null) {
						binding = (ICodeCompletionBinding)codon.AddIn.CreateObject(codon.Properties["class"]);
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
		
		public virtual bool HandleKeyPress(SharpDevelopTextAreaControl editor, char ch)
		{
			switch (ch) {
				case '(':
					if (enableMethodInsight && CodeCompletionOptions.InsightEnabled) {
						editor.ShowInsightWindow(new MethodInsightDataProvider());
						return true;
					} else {
						return false;
					}
				case '[':
					if (enableIndexerInsight && CodeCompletionOptions.InsightEnabled) {
						editor.ShowInsightWindow(new IndexerInsightDataProvider());
						return true;
					} else {
						return false;
					}
				case '<':
					if (enableXmlCommentCompletion) {
						editor.ShowCompletionWindow(new CommentCompletionDataProvider(), ch);
						return true;
					} else {
						return false;
					}
				case '.':
					if (enableDotCompletion) {
						editor.ShowCompletionWindow(new CodeCompletionDataProvider(), ch);
						return true;
					} else {
						return false;
					}
				case ' ':
					if (!CodeCompletionOptions.KeywordCompletionEnabled)
						return false;
					string word = editor.GetWordBeforeCaret();
					if (word != null)
						return HandleKeyword(editor, word);
					else
						return false;
				default:
					return false;
			}
		}
		
		public virtual bool HandleKeyword(SharpDevelopTextAreaControl editor, string word)
		{
			// DefaultCodeCompletionBinding does not support Keyword handling, but this
			// method can be overridden
			return false;
		}
		
		public virtual bool CtrlSpace(SharpDevelopTextAreaControl editor)
		{
			CtrlSpaceCompletionDataProvider provider = new CtrlSpaceCompletionDataProvider();
			provider.AllowCompleteExistingExpression = true;
			editor.ShowCompletionWindow(provider, '\0');
			return true;
		}
	}
}
