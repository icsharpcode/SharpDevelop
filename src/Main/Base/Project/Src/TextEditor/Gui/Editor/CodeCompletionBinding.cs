// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor
{
	/// <summary>
	/// Interface that gives backend bindings the possibility to control what characters and
	/// keywords invoke code completion.
	/// </summary>
	public interface ICodeCompletionBinding
	{
		bool HandleKeyPress(SharpDevelopTextAreaControl editor, char ch);
	}
	
	public class DefaultCodeCompletionBinding : ICodeCompletionBinding
	{
		string extension;
		
		public DefaultCodeCompletionBinding(string extension)
		{
			this.extension = extension;
		}
		
		public bool CheckExtension(SharpDevelopTextAreaControl editor)
		{
			string ext = Path.GetExtension(editor.FileName);
			return string.Compare(ext, extension, true) == 0;
		}
		
		bool enableMethodInsight = true;
		bool enableIndexerInsight = true;
		bool enableXmlCommentCompletion = false;
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
			if (!CheckExtension(editor))
				return false;
			switch (ch) {
				case '(':
					if (enableMethodInsight) {
						editor.ShowInsightWindow(new MethodInsightDataProvider());
						return true;
					} else {
						return false;
					}
				case '[':
					if (enableIndexerInsight) {
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
						editor.ShowCompletionWindow(editor.CreateCodeCompletionDataProvider(false), ch);
						return true;
					} else {
						return false;
					}
				case ' ':
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
	}
}
