// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;
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
		readonly ITextEditor editor;
		
		/// <summary>
		/// The text editor for which this editor context was created.
		/// </summary>
		public ITextEditor Editor {
			get {
				WorkbenchSingleton.AssertMainThread();
				return editor;
			}
		}
		
		/// <summary>
		/// Gets/Sets the file name.
		/// </summary>
		public FileName FileName { get; private set; }
		
		/// <summary>
		/// A snapshot of the editor content, at the time when this editor context was created.
		/// </summary>
		public ITextSource TextSource { get; private set; }
		
		readonly int caretOffset;
		
		/// <summary>
		/// Gets the offset of the caret, at the time when this editor context was created.
		/// </summary>
		public int CaretOffset {
			get { return caretOffset; }
		}
		
		volatile bool inInitializationPhase = true;
		System.Threading.Tasks.Task<ParseInformation> parseInformation;
		System.Threading.Tasks.Task<ICompilation> compilation;
		
		/// <summary>
		/// ParseInformation for the file.
		/// </summary>
		public ParseInformation ParseInformation {
			get {
				CheckForDeadlock();
				return parseInformation.Result;
			}
		}
		
		public ICompilation Compilation {
			get {
				CheckForDeadlock();
				return compilation.Result;
			}
		}
		
		void CheckForDeadlock()
		{
			if (inInitializationPhase) {
				var workbench = WorkbenchSingleton.Workbench;
				if (workbench != null) {
					if (workbench.MainWindow.Dispatcher.CheckAccess()) {
						throw new InvalidOperationException("Cannot access this property on the main thread during the EditorContext initialization phase - could cause deadlocks.");
					}
				}
			}
		}
		
		/// <summary>
		/// Waits until the initialization of this editor context is done.
		/// This is necessary to avoid deadlocks due to the main thread
		/// waiting for the parser service.
		/// </summary>
		public System.Threading.Tasks.Task WaitForInitializationAsync()
		{
			return compilation.ContinueWith(_ => { inInitializationPhase = false; });
		}
		
		/// <summary>
		/// Caches values shared by Context actions. Used in <see cref="GetCached"/>.
		/// </summary>
		readonly Dictionary<Type, object> cachedValues = new Dictionary<Type, object>();
		
		/// <summary>
		/// Fully initializes the EditorContext.
		/// </summary>
		public EditorContext(ITextEditor editor)
		{
			if (editor == null)
				throw new ArgumentNullException("editor");
			this.editor = editor;
			caretOffset = editor.Caret.Offset;
			if (caretOffset > 0 && editor.Document.GetCharAt(caretOffset - 1) == ';') {
				// If caret is just after ';', pretend that caret is before ';'
				// (works well e.g. for this.Foo();(*caret*) - we want to get "this.Foo()")
				// This is equivalent to pretending that ; don't exist, and actually it's not such a bad idea.
				caretOffset -= 1;
			}
			
			this.FileName = editor.FileName;
			this.TextSource = editor.Document.CreateSnapshot();
			this.parseInformation = ParserService.ParseAsync(this.FileName, this.TextSource);
			this.compilation = parseInformation.ContinueWith(_ => ParserService.GetCompilationForFile(this.FileName));
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
	}
}
