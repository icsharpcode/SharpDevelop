// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
		object syncRoot;
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
		
		Task<ParseInformation> parseInformation;
		ICompilation compilation;
		
		/// <summary>
		/// Gets the ParseInformation for the file.
		/// </summary>
		/// <remarks><inheritdoc cref="ParserService.ParseAsync"/></remarks>
		public Task<ParseInformation> GetParseInformationAsync()
		{
			lock (syncRoot) {
				if (parseInformation == null)
					parseInformation = ParserService.ParseAsync(this.FileName, this.TextSource);
				return parseInformation;
			}
		}
		
		/// <summary>
		/// Gets the ICompilation for the file.
		/// </summary>
		public Task<ICompilation> GetCompilationAsync()
		{
			var c = LazyInitializer.EnsureInitialized(ref compilation, () => ParserService.GetCompilationForFile(this.FileName));
			return Task.FromResult(c);
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
			
			this.FileName = editor.FileName;
			this.TextSource = editor.Document.CreateSnapshot();
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
