// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Linq;
using System.Threading;

using ICSharpCode.Core;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Ast;
using ICSharpCode.ILSpyAddIn.ViewContent;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.Utils;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Bookmarks;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Gui;
using Mono.Cecil;

namespace ICSharpCode.ILSpyAddIn
{
	/// <summary>
	/// Hosts a decompiled type.
	/// </summary>
	public class DecompiledViewContent : AbstractViewContentWithoutFile
	{
		readonly string assemblyFile;
		readonly string fullTypeName;
		
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
			codeView = new CodeView(string.Format("{0},{1}", assemblyFile, fullTypeName));
			this.assemblyFile = assemblyFile;
			this.fullTypeName = fullTypeName;
			this.jumpToEntityTagWhenDecompilationFinished = entityTag;
			
			string shortTypeName = fullTypeName.Substring(fullTypeName.LastIndexOf('.') + 1);
			this.TitleName = "[" + shortTypeName + "]";
			
			Thread thread = new Thread(DecompilationThread);
			thread.Name = "Decompiler (" + shortTypeName + ")";
			thread.Start();
			
			BookmarkManager.Removed += BookmarkManager_Removed;
			BookmarkManager.Added += BookmarkManager_Added;
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
			readerParameters.AssemblyResolver = new DefaultAssemblyResolver();
			
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
			if (!DebuggerService.ExternalDebugInformation.ContainsKey(token)) {
				DebuggerService.ExternalDebugInformation.Add(token, new DecompileInformation {
				                                             	CodeMappings = astBuilder.CodeMappings,
				                                             	LocalVariables = astBuilder.LocalVariables,
				                                             	DecompiledMemberReferences = astBuilder.DecompiledMemberReferences,
				                                             	AstNodes = nodes
				                                             });
			} else {
				DebuggerService.ExternalDebugInformation[token] = new DecompileInformation {
				                                             	CodeMappings = astBuilder.CodeMappings,
				                                             	LocalVariables = astBuilder.LocalVariables,
				                                             	DecompiledMemberReferences = astBuilder.DecompiledMemberReferences,
				                                             	AstNodes = nodes
				                                             };
			}
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
			UpdateDebuggingUI();
			
			// fire event
			OnDecompilationFinished(EventArgs.Empty);
		}
		#endregion
		
		#region Update UI
		void UpdateIconMargin(string text)
		{
			string tempFileName = string.Format("decompiled/{0}.cs", fullTypeName);
			
			codeView.IconBarManager.UpdateClassMemberBookmarks(ParserService.ParseFile(tempFileName, new StringTextBuffer(text)));
			
			// load bookmarks
			foreach (SDBookmark bookmark in BookmarkManager.GetBookmarks(codeView.Adapter.FileName)) {
				bookmark.Document = codeView.Adapter.Document;
				codeView.IconBarManager.Bookmarks.Add(bookmark);
			}
		}
		
		void UpdateDebuggingUI()
		{
			if (!DebuggerService.IsDebuggerStarted)
				return;
			
			if (DebuggerService.DebugStepInformation != null) {
				// get debugging information
				DecompileInformation debugInformation = (DecompileInformation)DebuggerService.ExternalDebugInformation[MemberReference.MetadataToken.ToInt32()];
				int token = DebuggerService.DebugStepInformation.Item1;
				int ilOffset = DebuggerService.DebugStepInformation.Item2;
				int line;
				MemberReference member;
				if (debugInformation.CodeMappings == null || !debugInformation.CodeMappings.ContainsKey(token))
					return;
				
				debugInformation.CodeMappings[token].GetInstructionByTokenAndOffset(token, ilOffset, out member, out line);
				
				// update bookmark & marker
				codeView.UnfoldAndScroll(line);
				CurrentLineBookmark.SetPosition(this, line, 0, line, 0);
			}
		}
		
		public void JumpTo(int lineNumber)
		{
			if (lineNumber <= 0)
				return;
			
			if (codeView == null)
				return;
			
			codeView.UnfoldAndScroll(lineNumber);
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
			if (mark != null && mark is BreakpointBookmark && mark.FileName == codeView.DecompiledFullTypeName) {
				codeView.IconBarManager.Bookmarks.Add(mark);
				mark.Document = codeView.Adapter.Document;
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
