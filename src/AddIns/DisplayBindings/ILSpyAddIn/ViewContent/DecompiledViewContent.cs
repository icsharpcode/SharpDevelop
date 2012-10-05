// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Threading;

using ICSharpCode.Core;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Ast;
using ICSharpCode.ILSpyAddIn.LaunchILSpy;
using ICSharpCode.ILSpyAddIn.ViewContent;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor.Bookmarks;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;
using ICSharpCode.SharpDevelop.Editor.Bookmarks;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Workbench;
using Mono.Cecil;

namespace ICSharpCode.ILSpyAddIn
{
	/// <summary>
	/// Hosts a decompiled type.
	/// </summary>
	class DecompiledViewContent : AbstractViewContentWithoutFile
	{
		readonly FileName assemblyFile;
		readonly string fullTypeName;
		readonly FileName virtualFileName;
		
		/// <summary>
		/// Entity to jump to once decompilation has finished.
		/// </summary>
		string jumpToEntityIdStringWhenDecompilationFinished;
		
		bool decompilationFinished;
		
		readonly CodeView codeView;
		readonly CancellationTokenSource cancellation = new CancellationTokenSource();
		
		Dictionary<string, TextLocation> memberLocations;
		
		#region Constructor
		public DecompiledViewContent(FileName assemblyFile, string fullTypeName, string entityTag)
		{
			this.virtualFileName = FileName.Create("ilspy://" + assemblyFile + "/" + fullTypeName);
			this.codeView = new CodeView();
			
			this.assemblyFile = assemblyFile;
			this.fullTypeName = fullTypeName;
			this.jumpToEntityIdStringWhenDecompilationFinished = entityTag;
			
			string shortTypeName = fullTypeName.Substring(fullTypeName.LastIndexOf('.') + 1);
			this.TitleName = "[" + ReflectionHelper.SplitTypeParameterCountFromReflectionName(shortTypeName) + "]";
			
			Thread thread = new Thread(DecompilationThread);
			thread.Name = "Decompiler (" + shortTypeName + ")";
			thread.Start();
			
			SD.BookmarkManager.BookmarkRemoved += BookmarkManager_Removed;
			SD.BookmarkManager.BookmarkAdded += BookmarkManager_Added;
		}
		#endregion
		
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
			get { return codeView; }
		}
		
		public override bool IsReadOnly {
			get { return true; }
		}
		
		#endregion
		
		#region Dispose
		public override void Dispose()
		{
			cancellation.Cancel();
			codeView.Dispose();
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
		
		#region JumpToEntity
		public void JumpToEntity(string entityIdString)
		{
			if (!decompilationFinished) {
				this.jumpToEntityIdStringWhenDecompilationFinished = entityIdString;
				return;
			}
			TextLocation location;
			if (memberLocations != null && memberLocations.TryGetValue(entityIdString, out location))
				codeView.JumpTo(location.Line, location.Column);
		}
		#endregion
		
		#region Decompilation
		void DecompilationThread()
		{
			try {
				StringWriter writer = new StringWriter();
				RunDecompiler(assemblyFile, fullTypeName, new DebuggerTextOutput(new PlainTextOutput(writer)), cancellation.Token);
				if (!cancellation.IsCancellationRequested) {
					SD.MainThread.InvokeAsync(() => OnDecompilationFinished(writer)).FireAndForget();
				}
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
				SD.MainThread.InvokeAsync(() => OnDecompilationFinished(writer)).FireAndForget();
			}
		}
		
		void RunDecompiler(string assemblyFile, string fullTypeName, DebuggerTextOutput textOutput, CancellationToken cancellationToken)
		{
			ReaderParameters readerParameters = new ReaderParameters();
			// Use new assembly resolver instance so that the AssemblyDefinitions can be garbage-collected
			// once the code is decompiled.
			readerParameters.AssemblyResolver = new ILSpyAssemblyResolver(Path.GetDirectoryName(assemblyFile));
			
			ModuleDefinition module = ModuleDefinition.ReadModule(assemblyFile, readerParameters);
			TypeDefinition typeDefinition = module.GetType(fullTypeName);
			if (typeDefinition == null)
				throw new InvalidOperationException("Could not find type");
			DecompilerContext context = new DecompilerContext(module);
			context.CancellationToken = cancellationToken;
			AstBuilder astBuilder = new AstBuilder(context);
			astBuilder.AddType(typeDefinition);
			astBuilder.GenerateCode(textOutput);
			
			// save decompilation data
			memberLocations = textOutput.MemberLocations;
		}
		
		void OnDecompilationFinished(StringWriter output)
		{
			if (cancellation.IsCancellationRequested)
				return;
			codeView.Document.Text = output.ToString();
			codeView.Document.UndoStack.ClearAll();
			
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
		
		public void UpdateDebuggingUI()
		{
			if (!DebuggerService.IsDebuggerStarted)
				return;
			if (decompiledType == null || decompiledType.MetadataToken == null)
				return;
			
			int typeToken = decompiledType.MetadataToken.ToInt32();
			if (!DebuggerDecompilerService.DebugInformation.ContainsKey(typeToken))
				return;
			var decompilerService = DebuggerDecompilerService.Instance;
			if (decompilerService == null || decompilerService.DebugStepInformation == null)
				return;
			
			// get debugging information
			DecompileInformation debugInformation = (DecompileInformation)DebuggerDecompilerService.DebugInformation[typeToken];
			int methodToken = decompilerService.DebugStepInformation.Item1;
			int ilOffset = decompilerService.DebugStepInformation.Item2;
			int line;
			MemberReference member;
			if (debugInformation.CodeMappings == null || !debugInformation.CodeMappings.ContainsKey(methodToken))
				return;
			
			debugInformation.CodeMappings[methodToken].GetInstructionByTokenAndOffset(methodToken, ilOffset, out member, out line);
			
			// if the codemappings are not built
			if (line <= 0) {
				DebuggerService.CurrentDebugger.StepOver();
				return;
			}
			
			// jump to line - scoll and unfold
			this.UpdateCurrentLineBookmark(line);
			this.JumpToLineNumber(line);
		}
		
		void UpdateCurrentLineBookmark(int lineNumber)
		{
			if (lineNumber <= 0)
				return;
			
			CurrentLineBookmark.SetPosition(uri, codeView.Document, lineNumber, 0, lineNumber, 0);
			var currentLineBookmark = BookmarkManager.Bookmarks.OfType<CurrentLineBookmark>().FirstOrDefault();
			if (currentLineBookmark != null) {
				// update bookmark & marker
				codeView.IconBarManager.Bookmarks.Add(currentLineBookmark);
				currentLineBookmark.Document = this.codeView.TextEditor.Document;
			}
		}*/
		
		#endregion
		
		#region Bookmarks
		void BookmarkManager_Removed(object sender, BookmarkEventArgs e)
		{
			var mark = e.Bookmark;
			if (mark != null && codeView.IconBarManager.Bookmarks.Contains(mark)) {
				codeView.IconBarManager.Bookmarks.Remove(mark);
				mark.Document = null;
			}
		}
		
		void BookmarkManager_Added(object sender, BookmarkEventArgs e)
		{
			var mark = e.Bookmark;
			if (mark != null && mark is BreakpointBookmark && mark.FileName == virtualFileName) {
				codeView.IconBarManager.Bookmarks.Add(mark);
				mark.Document = this.codeView.Document;
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
