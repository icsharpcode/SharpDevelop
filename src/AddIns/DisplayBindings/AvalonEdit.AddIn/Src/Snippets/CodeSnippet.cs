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
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;

using ICSharpCode.AvalonEdit.Snippets;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Refactoring;

namespace ICSharpCode.AvalonEdit.AddIn.Snippets
{
	/// <summary>
	/// A code snippet.
	/// </summary>
	public class CodeSnippet : AbstractFreezable, INotifyPropertyChanged
	{
		string name = string.Empty, description = string.Empty, text = string.Empty, keyword = string.Empty;
		
		public CodeSnippet()
		{
		}
		
		public CodeSnippet(CodeSnippet copy)
		{
			this.name = copy.name;
			this.description = copy.description;
			this.text = copy.text;
			this.keyword = copy.keyword;
		}
		
		public string Name {
			get { return name; }
			set {
				FreezableHelper.ThrowIfFrozen(this);
				if (name != value) {
					name = value ?? string.Empty;
					OnPropertyChanged("Name");
				}
			}
		}
		
		public string Text {
			get { return text; }
			set {
				FreezableHelper.ThrowIfFrozen(this);
				if (text != value) {
					text = value ?? string.Empty;
					OnPropertyChanged("Text");
				}
			}
		}
		
		public string Description {
			get { return description; }
			set {
				FreezableHelper.ThrowIfFrozen(this);
				if (description != value) {
					description = value ?? string.Empty;
					OnPropertyChanged("Description");
				}
			}
		}
		
		public bool HasSelection {
			get {
				return pattern.Matches(this.Text)
					.OfType<Match>()
					.Any(item => item.Value == "${Selection}");
			}
		}
		
		public string Keyword {
			get { return keyword; }
			set {
				FreezableHelper.ThrowIfFrozen(this);
				if (keyword != value) {
					keyword = value ?? string.Empty;
					OnPropertyChanged("Keyword");
				}
			}
		}
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null) {
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		
		public Snippet CreateAvalonEditSnippet(ITextEditor context)
		{
			return CreateAvalonEditSnippet(context, this.Text);
		}
		
		public ISnippetCompletionItem CreateCompletionItem(ITextEditor context)
		{
			return new SnippetCompletionItem(context, this) { AlwaysInsertSnippet = context.SelectionLength > 0 };
		}
		
		readonly static Regex pattern = new Regex(@"\$\{([^\}]*)\}", RegexOptions.CultureInvariant);
		
		public static Snippet CreateAvalonEditSnippet(ITextEditor context, string snippetText)
		{
			if (snippetText == null)
				throw new ArgumentNullException("text");
			var replaceableElements = new Dictionary<string, SnippetReplaceableTextElement>(StringComparer.OrdinalIgnoreCase);
			foreach (Match m in pattern.Matches(snippetText)) {
				string val = m.Groups[1].Value;
				int equalsSign = val.IndexOf('=');
				if (equalsSign > 0) {
					string name = val.Substring(0, equalsSign);
					replaceableElements[name] = new SnippetReplaceableTextElement();
				}
			}
			Snippet snippet = new Snippet();
			int pos = 0;
			foreach (Match m in pattern.Matches(snippetText)) {
				if (pos < m.Index) {
					snippet.Elements.Add(new SnippetTextElement { Text = snippetText.Substring(pos, m.Index - pos) });
					pos = m.Index;
				}
				snippet.Elements.Add(CreateElementForValue(context, replaceableElements, m.Groups[1].Value, m.Index, snippetText));
				pos = m.Index + m.Length;
			}
			if (pos < snippetText.Length) {
				snippet.Elements.Add(new SnippetTextElement { Text = snippetText.Substring(pos) });
			}
			if (!snippet.Elements.Any(e => e is SnippetCaretElement)) {
				int index = snippet.Elements.FindIndex(e2 => e2 is SnippetSelectionElement);
				if (index > -1)
					snippet.Elements.Insert(index + 1, new SnippetCaretElement());
			}
			return snippet;
		}
		
		readonly static Regex functionPattern = new Regex(@"^([a-zA-Z]+)\(([^\)]*)\)$", RegexOptions.CultureInvariant);
		
		static SnippetElement CreateElementForValue(ITextEditor context, Dictionary<string, SnippetReplaceableTextElement> replaceableElements, string val, int offset, string snippetText)
		{
			SnippetReplaceableTextElement srte;
			int equalsSign = val.IndexOf('=');
			if (equalsSign > 0) {
				string name = val.Substring(0, equalsSign);
				if (replaceableElements.TryGetValue(name, out srte)) {
					if (srte.Text == null)
						srte.Text = val.Substring(equalsSign + 1);
					return srte;
				}
			}
			
			foreach (ISnippetElementProvider provider in SnippetManager.Instance.SnippetElementProviders) {
				SnippetElement element = provider.GetElement(new SnippetInfo(val, snippetText, offset));
				if (element != null)
					return element;
			}
			
			if (replaceableElements.TryGetValue(val, out srte))
				return new SnippetBoundElement { TargetElement = srte };
			Match m = functionPattern.Match(val);
			if (m.Success) {
				Func<string, string> f = GetFunction(context, m.Groups[1].Value);
				if (f != null) {
					string innerVal = m.Groups[2].Value;
					if (replaceableElements.TryGetValue(innerVal, out srte))
						return new FunctionBoundElement { TargetElement = srte, function = f };
					string result2 = GetValue(context, innerVal);
					if (result2 != null)
						return new SnippetTextElement { Text = f(result2) };
					else
						return new SnippetTextElement { Text = f(innerVal) };
				}
			}
			string result = GetValue(context, val);
			if (result != null)
				return new SnippetTextElement { Text = result };
			else
				return new SnippetReplaceableTextElement { Text = val }; // ${unknown} -> replaceable element
		}
		
		static string GetValue(ITextEditor editor, string propertyName)
		{
			if ("ClassName".Equals(propertyName, StringComparison.OrdinalIgnoreCase)) {
				var c = GetCurrentClass(editor);
				if (c != null)
					return c.Name;
			}
			return Core.StringParser.GetValue(propertyName);
		}
		
		static IUnresolvedTypeDefinition GetCurrentClass(ITextEditor editor)
		{
			var parseInfo = SD.ParserService.GetExistingUnresolvedFile(editor.FileName);
			if (parseInfo != null) {
				return parseInfo.GetInnermostTypeDefinition(editor.Caret.Location);
			}
			return null;
		}
		
		static Func<string, string> GetFunction(ITextEditor context, string name)
		{
			if ("toLower".Equals(name, StringComparison.OrdinalIgnoreCase))
				return s => s.ToLower();
			if ("toUpper".Equals(name, StringComparison.OrdinalIgnoreCase))
				return s => s.ToUpper();
			
			if ("toFieldName".Equals(name, StringComparison.OrdinalIgnoreCase))
				return s => context.Language.CodeGenerator.GetFieldName(s);
			if ("toPropertyName".Equals(name, StringComparison.OrdinalIgnoreCase))
				return s => context.Language.CodeGenerator.GetPropertyName(s);
			if ("toParameterName".Equals(name, StringComparison.OrdinalIgnoreCase))
				return s => context.Language.CodeGenerator.GetParameterName(s);
			return null;
		}
		
		sealed class FunctionBoundElement : SnippetBoundElement
		{
			internal Func<string, string> function;
			
			public override string ConvertText(string input)
			{
				return function(input);
			}
		}
		
		/// <summary>
		/// Reports the snippet usage to UDC
		/// </summary>
		internal void TrackUsage(string activationMethod)
		{
			bool isUserModified = !SnippetManager.Instance.defaultSnippets.Any(g => g.Snippets.Contains(this, CodeSnippetComparer.Instance));
			SD.AnalyticsMonitor.TrackFeature(typeof(CodeSnippet), isUserModified ? "usersnippet" : Name, activationMethod);
		}
	}
}
