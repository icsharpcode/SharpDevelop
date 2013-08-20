// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Threading;

using ICSharpCode.AvalonEdit.AddIn;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.Core;
using ICSharpCode.Decompiler;
using ICSharpCode.ILSpyAddIn;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor.Bookmarks;
using ICSharpCode.SharpDevelop.Workbench;

namespace ICSharpCode.ILSpyAddIn
{
	/// <summary>
	/// Hosts a decompiled type.
	/// </summary>
	class DecompiledViewContent : AbstractViewContentWithoutFile
	{
		readonly FileName assemblyFile;
		readonly string fullTypeName;
		public DecompiledTypeReference DecompiledTypeName { get; private set; }
		
		public override FileName PrimaryFileName {
			get { return this.DecompiledTypeName.ToFileName(); }
		}
		
		/// <summary>
		/// Entity to jump to once decompilation has finished.
		/// </summary>
		string jumpToEntityIdStringWhenDecompilationFinished;
		
		bool decompilationFinished;
		
		readonly CodeEditor codeEditor = new CodeEditor();
		readonly CancellationTokenSource cancellation = new CancellationTokenSource();
		
		Dictionary<string, TextLocation> memberLocations;
		public Dictionary<string, MethodDebugSymbols> DebugSymbols { get; private set; }
		
		#region Constructor
		public DecompiledViewContent(FileName assemblyFile, string fullTypeName, string entityTag)
		{
			this.DecompiledTypeName = new DecompiledTypeReference(assemblyFile, new FullTypeName(fullTypeName));
			
			this.Services = codeEditor.GetRequiredService<IServiceContainer>();
			
			this.assemblyFile = assemblyFile;
			this.fullTypeName = fullTypeName;
			this.jumpToEntityIdStringWhenDecompilationFinished = entityTag;
			
			string shortTypeName = fullTypeName.Substring(fullTypeName.LastIndexOf('.') + 1);
			this.TitleName = "[" + ReflectionHelper.SplitTypeParameterCountFromReflectionName(shortTypeName) + "]";
			
			DecompilationThread();
//			Thread thread = new Thread(DecompilationThread);
//			thread.Name = "Decompiler (" + shortTypeName + ")";
//			thread.Start();
//			thread.Join();
			
			SD.BookmarkManager.BookmarkRemoved += BookmarkManager_Removed;
			SD.BookmarkManager.BookmarkAdded += BookmarkManager_Added;
			
			this.codeEditor.FileName = this.DecompiledTypeName.ToFileName();
			this.codeEditor.ActiveTextEditor.IsReadOnly = true;
			this.codeEditor.ActiveTextEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("C#");
		}
		#endregion
		
		public static DecompiledViewContent Get(DecompiledTypeReference name)
		{
			var viewContents = SD.Workbench.ViewContentCollection.OfType<DecompiledViewContent>();
			return viewContents.FirstOrDefault(c => c.DecompiledTypeName == name);
		}
		
		public static DecompiledViewContent Get(IEntity entity)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");
			
			// Get the underlying entity for generic instance members
			if (entity is IMember)
				entity = ((IMember)entity).MemberDefinition;
			
			ITypeDefinition declaringType = (entity as ITypeDefinition) ?? entity.DeclaringTypeDefinition;
			if (declaringType == null)
				return null;
			// get the top-level type
			while (declaringType.DeclaringTypeDefinition != null)
				declaringType = declaringType.DeclaringTypeDefinition;
			
			FileName assemblyLocation = declaringType.ParentAssembly.GetRuntimeAssemblyLocation();
			if (assemblyLocation != null && File.Exists(assemblyLocation)) {
				return Get(assemblyLocation, declaringType.ReflectionName);
			}
			return null;
		}
		
		public static DecompiledViewContent Get(FileName assemblyFile, string typeName)
		{
			if (assemblyFile == null)
				throw new ArgumentNullException("assemblyFile");
			if (string.IsNullOrEmpty(typeName))
				throw new ArgumentException("typeName is null or empty");
			
			foreach (var viewContent in SD.Workbench.ViewContentCollection.OfType<DecompiledViewContent>()) {
				if (viewContent.AssemblyFile == assemblyFile && typeName == viewContent.FullTypeName) {
					return viewContent;
				}
			}
			
			var newViewContent = new DecompiledViewContent(assemblyFile, typeName, null);
			SD.Workbench.ShowView(newViewContent);
			return newViewContent;
		}
		
		#region Properties
		public FileName AssemblyFile {
			get { return assemblyFile; }
		}
		
		/// <summary>
		/// The reflection name of the top-level type displayed in this view content.
		/// </summary>
		public string FullTypeName {
			get { return fullTypeName; }
		}
		
		public override object Control {
			get { return codeEditor; }
		}
		
		public override bool IsReadOnly {
			get { return true; }
		}
		
		#endregion
		
		#region Dispose
		public override void Dispose()
		{
			cancellation.Cancel();
			codeEditor.Dispose();
			SD.BookmarkManager.BookmarkAdded -= BookmarkManager_Added;
			SD.BookmarkManager.BookmarkRemoved -= BookmarkManager_Removed;
//			DecompileInformation data;
//			DebuggerDecompilerService.DebugInformation.TryRemove(decompiledType.MetadataToken.ToInt32(), out data);
			base.Dispose();
		}
		#endregion
		
		#region Load/Save
		public override void Load()
		{
			// nothing to do...
		}
		
		public override void Save()
		{
			if (!decompilationFinished)
				return;
			// TODO: show Save As dialog to allow the user to save the decompiled file
		}
		#endregion
		
		public override INavigationPoint BuildNavPoint()
		{
			return codeEditor.BuildNavPoint();
		}
		
		#region JumpToEntity
		public void JumpToEntity(string entityIdString)
		{
			if (!decompilationFinished) {
				this.jumpToEntityIdStringWhenDecompilationFinished = entityIdString;
				return;
			}
			TextLocation location;
			if (entityIdString != null && memberLocations != null && memberLocations.TryGetValue(entityIdString, out location))
				codeEditor.JumpTo(location.Line, location.Column);
		}
		#endregion
		
		#region Decompilation
		void DecompilationThread()
		{
			try {
				var file = ILSpyDecompilerService.DecompileType(DecompiledTypeName);
				memberLocations = file.MemberLocations;
				DebugSymbols = file.DebugSymbols;
				OnDecompilationFinished(file.Writer);
			} catch (OperationCanceledException) {
				// ignore cancellation
			} catch (Exception ex) {
				if (cancellation.IsCancellationRequested) {
					MessageService.ShowException(ex);
					return;
				}
				SD.AnalyticsMonitor.TrackException(ex);
				
				StringWriter writer = new StringWriter();
				writer.WriteLine(string.Format("Exception while decompiling {0} ({1})", fullTypeName, assemblyFile));
				writer.WriteLine();
				writer.WriteLine(ex.ToString());
				SD.MainThread.InvokeAsyncAndForget(() => OnDecompilationFinished(writer));
			}
		}
		
		void OnDecompilationFinished(StringWriter output)
		{
			if (cancellation.IsCancellationRequested)
				return;
			codeEditor.Document.Text = output.ToString();
			codeEditor.Document.UndoStack.ClearAll();
			
			this.decompilationFinished = true;
			JumpToEntity(this.jumpToEntityIdStringWhenDecompilationFinished);
			
			// update UI
			//UpdateIconMargin();
			
			// fire events
			OnDecompilationFinished(EventArgs.Empty);
		}
		#endregion
		
		#region Update UI
		/*
		void UpdateIconMargin()
		{
			codeView.IconBarManager.UpdateClassMemberBookmarks(
				ParserService.ParseFile(tempFileName, new AvalonEditDocumentAdapter(codeView.Document, null)),
				null);
			
			// load bookmarks
			foreach (SDBookmark bookmark in BookmarkManager.GetBookmarks(this.codeView.TextEditor.FileName)) {
				bookmark.Document = this.codeView.TextEditor.Document;
				codeView.IconBarManager.Bookmarks.Add(bookmark);
			}
		}
		 */
		#endregion
		
		#region Bookmarks
		void BookmarkManager_Removed(object sender, BookmarkEventArgs e)
		{
			var mark = e.Bookmark;
			if (mark != null && codeEditor.IconBarManager.Bookmarks.Contains(mark)) {
				codeEditor.IconBarManager.Bookmarks.Remove(mark);
				mark.Document = null;
			}
		}
		
		void BookmarkManager_Added(object sender, BookmarkEventArgs e)
		{
			var mark = e.Bookmark;
			if (mark != null && mark.FileName == PrimaryFileName) {
				codeEditor.IconBarManager.Bookmarks.Add(mark);
				mark.Document = this.codeEditor.Document;
			}
		}
		#endregion
		
		#region Events
		
		public event EventHandler DecompilationFinished;
		
		protected virtual void OnDecompilationFinished(EventArgs e)
		{
			if (DecompilationFinished != null) {
				DecompilationFinished(this, e);
			}
		}
		
		#endregion
	}
}
