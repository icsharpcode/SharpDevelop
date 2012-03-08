// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Parser;

namespace ICSharpCode.SharpDevelop.Refactoring
{
	/// <summary>
	/// Contains information about code around the caret in the editor - useful for implementing Context actions.
	/// Do not keep your own references to EditorContext.
	/// It serves as one-time cache and does not get updated when editor text changes.
	/// </summary>
	public class EditorContext
	{
		public ITextEditor Editor { get; private set; }
		
		public int CaretLine { get; private set; }
		public int CaretColumn { get; private set; }
		
		/// <summary>
		/// ParseInformation for current file.
		/// </summary>
		public ParseInformation CurrentParseInformation { get; private set; }
		
		public ICompilation Compilation { get; private set; }
		
		/// <summary>
		/// The editor line containing the caret.
		/// </summary>
		public IDocumentLine CurrentLine { get; private set; }
		
		/// <summary>
		/// Caches values shared by Context actions. Used in <see cref="GetCached"/>.
		/// </summary>
		Dictionary<Type, object> cachedValues = new Dictionary<Type, object>();
		
		/// <summary>
		/// Fully initializes the EditorContext.
		/// </summary>
		public EditorContext(ITextEditor editor)
		{
			if (editor == null)
				throw new ArgumentNullException("editor");
			this.Editor = editor;
			this.CaretLine = editor.Caret.Line;
			this.CaretColumn = editor.Caret.Column;
			if (CaretColumn > 1 && editor.Document.GetCharAt(editor.Document.GetOffset(CaretLine, CaretColumn - 1)) == ';') {
				// If caret is just after ';', pretend that caret is before ';'
				// (works well e.g. for this.Foo();(*caret*) - we want to get "this.Foo()")
				// This is equivalent to pretending that ; don't exist, and actually it's not such a bad idea.
				CaretColumn -= 1;
			}
			
			this.CurrentParseInformation = ParserService.Parse(editor.FileName, editor.Document);
			
			this.CurrentLine = editor.Document.GetLine(CaretLine);
		}
		
		/// <summary>
		/// The resolved symbol at editor caret.
		/// </summary>
		public ResolveResult CurrentSymbol {
			get {
				throw new NotImplementedException();
			}
		}
		
		/// <summary>
		/// Gets cached value shared by context actions. Initializes a new value if not present.
		/// </summary>
		public T GetCached<T>() where T : IContextActionCache, new()
		{
			lock (cachedValues) {
				Type t = typeof(T);
				if (cachedValues.ContainsKey(t)) {
					return (T)cachedValues[t];
				} else {
					T cached = new T();
					cached.Initialize(this);
					cachedValues[t] = cached;
					return cached;
				}
			}
		}
		
		/// <summary>
		/// Do not call from your Context actions - used by SharpDevelop.
		/// Sets contents of editor context to null to prevent memory leaks. Used in case users implementing IContextActionProvider
		/// keep long-lived references to EditorContext even when warned not to do so.
		/// </summary>
		public void Clear()
		{
			this.Editor = null;
			this.CurrentLine = null;
			this.CurrentParseInformation = null;
			this.Compilation = null;
		}
	}
}
