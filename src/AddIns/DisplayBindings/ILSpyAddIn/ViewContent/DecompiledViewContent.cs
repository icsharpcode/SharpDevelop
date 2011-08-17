// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Linq;
using System.Threading;

using ICSharpCode.Core;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Ast;
using ICSharpCode.ILSpyAddIn.LaunchILSpy;
using ICSharpCode.ILSpyAddIn.ViewContent;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.Utils;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Bookmarks;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;
using Mono.Cecil;

namespace ICSharpCode.ILSpyAddIn
{
	/// <summary>
	/// Hosts a decompiled type.
	/// </summary>
	class DecompiledViewContent : AbstractViewContentWithoutFile
	{
		readonly string assemblyFile;
		readonly string fullTypeName;
		readonly string tempFileName;
		
		/// <summary>
		/// Entity to jump to once decompilation has finished.
		/// </summary>
		string jumpToEntityTagWhenDecompilationFinished;
		
		bool decompilationFinished;
		
		readonly CodeView codeView;
		readonly CancellationTokenSource cancellation = new CancellationTokenSource();
		
		#region Constructor
		public DecompiledViewContent(string assemblyFile, string fullTypeName, string entityTag)
		{
			// TODO: create options for decompiling in a specific language
			this.tempFileName = string.Format("{0}{1}{2}.cs", assemblyFile, DecompiledBreakpointBookmark.SEPARATOR, fullTypeName);
			this.codeView = new CodeView(tempFileName);
			
			this.assemblyFile = assemblyFile;
			this.fullTypeName = fullTypeName;
			this.jumpToEntityTagWhenDecompilationFinished = entityTag;
			
			string shortTypeName = fullTypeName.Substring(fullTypeName.LastIndexOf('.') + 1);
			this.TitleName = "[" + shortTypeName + "]";
			
			this.InfoTip = tempFileName;
			
			Thread thread = new Thread(DecompilationThread);
			thread.Name = "Decompiler (" + shortTypeName + ")";
			thread.Start();
			
			BookmarkManager.Removed += BookmarkManager_Removed;
			BookmarkManager.Added += BookmarkManager_Added;
			
			// add services
			this.Services.AddService(typeof(ITextEditor), this.codeView.TextEditor);
		}
		#endregion
		
		#region Properties
		public string AssemblyFile {
			get { return assemblyFile; }
		}
		
		public string FullTypeName {
			get { return fullTypeName; }
		}
		
		public override object Control {
			get { return codeView; }
		}
		
		public override bool IsReadOnly {
			get { return true; }
		}
		
		public MemberReference MemberReference {
			get; private set;
		}
		
		public override object Tag {
			get { return MemberReference; }
		}
		
		#endregion
		
		#region Dispose
		public override void Dispose()
		{
			cancellation.Cancel();
			codeView.Dispose();
			BookmarkManager.Added -= BookmarkManager_Added;
			BookmarkManager.Removed -= BookmarkManager_Removed;
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
		public void JumpToEntity(string entityTag)
		{
			if (!decompilationFinished) {
				this.jumpToEntityTagWhenDecompilationFinished = entityTag;
				return;
			}
			// TODO: implement this
		}
		#endregion
		
		#region Decompilation
		void DecompilationThread()
		{
			try {
				StringWriter writer = new StringWriter();
				RunDecompiler(assemblyFile, fullTypeName, new PlainTextOutput(writer), cancellation.Token);
				if (!cancellation.IsCancellationRequested) {
					WorkbenchSingleton.SafeThreadAsyncCall(OnDecompilationFinished, writer);
				}
			} catch (OperationCanceledException) {
				// ignore cancellation
			} catch (Exception ex) {
				if (cancellation.IsCancellationRequested) {
					MessageService.ShowException(ex);
					return;
				}
				AnalyticsMonitorService.TrackException(ex);
				
				StringWriter writer = new StringWriter();
				writer.WriteLine("Exception while decompiling " + fullTypeName);
				writer.WriteLine();
				writer.WriteLine(ex.ToString());
				WorkbenchSingleton.SafeThreadAsyncCall(OnDecompilationFinished, writer);
			}
		}
		
		void RunDecompiler(string assemblyFile, string fullTypeName, ITextOutput textOutput, CancellationToken cancellationToken)
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
			var nodes = TreeTraversal
				.PreOrder((AstNode)astBuilder.CompilationUnit, n => n.Children)
				.Where(n => n is AttributedNode && n.Annotation<Tuple<int, int>>() != null);
			MemberReference = typeDefinition;
			
			int token = MemberReference.MetadataToken.ToInt32();
			var info = new DecompileInformation {
				CodeMappings = astBuilder.CodeMappings,
				LocalVariables = astBuilder.LocalVariables,
				DecompiledMemberReferences = astBuilder.DecompiledMemberReferences,
				AstNodes = nodes
			};
			
			// save the data
			DebuggerDecompilerService.DebugInformation.AddOrUpdate(token, info, (k, v) => info);
		}
		
		void OnDecompilationFinished(StringWriter output)
		{
			if (cancellation.IsCancellationRequested)
				return;
			codeView.Document.Text = output.ToString();
			codeView.Document.UndoStack.ClearAll();
			
			this.decompilationFinished = true;
			JumpToEntity(this.jumpToEntityTagWhenDecompilationFinished);
			
			// update UI
			UpdateIconMargin(output.ToString());
			
			// fire events
			OnDecompilationFinished(EventArgs.Empty);
		}
		#endregion
		
		#region Update UI
		void UpdateIconMargin(string text)
		{
			codeView.IconBarManager.UpdateClassMemberBookmarks(ParserService.ParseFile(tempFileName, new StringTextBuffer(text)));
			
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
			if (MemberReference == null || MemberReference.MetadataToken == null)
				return;
			
			int typeToken = MemberReference.MetadataToken.ToInt32();
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
		
		public void JumpToLineNumber(int lineNumber)
		{
			if (codeView == null || codeView.Document == null)
				return;
			
			if (lineNumber <= 0 || lineNumber > codeView.Document.LineCount)
				return;
			
			codeView.UnfoldAndScroll(lineNumber);
		}
		
		void UpdateCurrentLineBookmark(int lineNumber)
		{
			if (lineNumber <= 0)
				return;
			
			CurrentLineBookmark.SetPosition(codeView.TextEditor.FileName, codeView.TextEditor.Document, lineNumber, 0, lineNumber, 0);
			var currentLineBookmark = BookmarkManager.Bookmarks.OfType<CurrentLineBookmark>().FirstOrDefault();
			if (currentLineBookmark != null) {
				// update bookmark & marker
				codeView.IconBarManager.Bookmarks.Add(currentLineBookmark);
				currentLineBookmark.Document = this.codeView.TextEditor.Document;
			}
		}
		
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
			if (mark != null && mark is BreakpointBookmark && mark.FileName == this.codeView.TextEditor.FileName) {
				codeView.IconBarManager.Bookmarks.Add(mark);
				mark.Document = this.codeView.TextEditor.Document;
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
