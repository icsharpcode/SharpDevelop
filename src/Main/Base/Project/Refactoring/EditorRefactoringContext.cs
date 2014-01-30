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
using System.Collections.Concurrent;
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
	public class EditorRefactoringContext
	{
		readonly object syncRoot = new object();
		readonly ITextEditor editor;
		
		/// <summary>
		/// The text editor for which this editor context was created.
		/// </summary>
		public ITextEditor Editor {
			get {
				SD.MainThread.VerifyAccess();
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
		readonly TextLocation caretLocation;
		
		/// <summary>
		/// Gets the offset of the caret, at the time when this editor context was created.
		/// </summary>
		public int CaretOffset {
			get { return caretOffset; }
		}
		
		/// <summary>
		/// Gets caret location, at the time when this editor context was created.
		/// </summary>
		public TextLocation CaretLocation {
			get { return caretLocation; }
		}
		
		Task<ParseInformation> parseInformation;
		Task<ICompilation> compilation;
		
		/// <summary>
		/// Gets the ParseInformation for the file.
		/// </summary>
		public Task<ParseInformation> GetParseInformationAsync()
		{
			lock (syncRoot) {
				if (parseInformation == null)
					parseInformation = SD.ParserService.ParseAsync(this.FileName, this.TextSource);
				return parseInformation;
			}
		}
		
		/// <summary>
		/// Gets the ParseInformation for the file.
		/// </summary>
		public ParseInformation GetParseInformation()
		{
			// waiting for the task is safe because we specified the text source in the ParseAsync call
			return GetParseInformationAsync().Result;
		}
		
		/// <summary>
		/// Gets the ICompilation for the file.
		/// </summary>
		public Task<ICompilation> GetCompilationAsync()
		{
			lock (syncRoot) {
				if (compilation == null)
					compilation = Task.FromResult(SD.ParserService.GetCompilationForFile(this.FileName));
				return compilation;
			}
		}
		
		/// <summary>
		/// Gets the ICompilation for the file.
		/// </summary>
		public ICompilation GetCompilation()
		{
			return GetCompilationAsync().Result;
		}
		
		/// <summary>
		/// Caches values shared by Context actions. Used in <see cref="GetCached"/>.
		/// </summary>
		readonly ConcurrentDictionary<Type, Task> cachedValues = new ConcurrentDictionary<Type, Task>();
		
		/// <summary>
		/// Creates a new EditorContext.
		/// </summary>
		public EditorRefactoringContext(ITextEditor editor)
		{
			if (editor == null)
				throw new ArgumentNullException("editor");
			this.editor = editor;
			caretOffset = editor.Caret.Offset;
			caretLocation = editor.Caret.Location;
			
			this.FileName = editor.FileName;
			this.TextSource = editor.Document.CreateSnapshot();
		}
		
		Task<ResolveResult> currentSymbol;
		
		/// <summary>
		/// The resolved symbol at editor caret.
		/// </summary>
		public Task<ResolveResult> GetCurrentSymbolAsync()
		{
			lock (syncRoot) {
				if (currentSymbol == null)
					currentSymbol = ResolveCurrentSymbolAsync();
				return currentSymbol;
			}
		}
		
		async Task<ResolveResult> ResolveCurrentSymbolAsync()
		{
			var parseInfo = await GetParseInformationAsync().ConfigureAwait(false);
			if (parseInfo == null)
				return null;
			var compilation = await GetCompilationAsync().ConfigureAwait(false);
			return await Task.Run(() => SD.ParserService.ResolveAsync(this.FileName, caretLocation, this.TextSource, compilation)).ConfigureAwait(false);
		}
		
		/// <summary>
		/// Gets cached value shared by context actions. Initializes a new value if not present.
		/// </summary>
		public Task<T> GetCachedAsync<T>(Func<EditorRefactoringContext, T> initializationFunc)
		{
			return (Task<T>)cachedValues.GetOrAdd(typeof(T), _ => Task.FromResult(initializationFunc(this)));
		}
		
		/// <summary>
		/// Gets cached value shared by context actions. Initializes a new value if not present.
		/// </summary>
		public Task<T> GetCachedAsync<T>(Func<EditorRefactoringContext, Task<T>> initializationFunc)
		{
			return (Task<T>)cachedValues.GetOrAdd(typeof(T), _ => initializationFunc(this));
		}
	}
}
