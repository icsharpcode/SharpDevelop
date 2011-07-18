// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.Core;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Ast;
using ICSharpCode.Decompiler.ILAst;
using ICSharpCode.ILSpyAddIn.ViewContent;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.Utils;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Bookmarks;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Dom;
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
		
		readonly CodeView codeView = new CodeView();
		readonly CancellationTokenSource cancellation = new CancellationTokenSource();
		
		#region Constructor
		public DecompiledViewContent(string assemblyFile, string fullTypeName, string entityTag)
		{
			this.assemblyFile = assemblyFile;
			this.fullTypeName = fullTypeName;
			this.jumpToEntityTagWhenDecompilationFinished = entityTag;
			
			string shortTypeName = fullTypeName.Substring(fullTypeName.LastIndexOf('.') + 1);
			this.TitleName = "[" + shortTypeName + "]";
			
			Thread thread = new Thread(DecompilationThread);
			thread.Name = "Decompiler (" + shortTypeName + ")";
			thread.Start();
			BookmarkManager.Removed += BookmarkManager_Removed;
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
		#endregion
		
		#region Dispose
		public override void Dispose()
		{
			cancellation.Cancel();
			codeView.Dispose();
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
		
		static void RunDecompiler(string assemblyFile, string fullTypeName, ITextOutput textOutput, CancellationToken cancellationToken)
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
			DebuggerService.ExternalDebugInformation = new DecompileInformation {
				CodeMappings = astBuilder.CodeMappings,
				LocalVariables = astBuilder.LocalVariables,
				DecompiledMemberReferences = astBuilder.DecompiledMemberReferences,
				AstNodes = nodes
			};
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
		}
		#endregion
		
		#region Update UI
		void UpdateIconMargin(string text)
		{
			string tempFileName = string.Format("decompiled/{0}.cs", fullTypeName);
			
			codeView.IconBarManager.UpdateClassMemberBookmarks(ParserService.ParseFile(tempFileName, new StringTextBuffer(text)));
		}
		
		void UpdateDebuggingUI()
		{
			if (!DebuggerService.IsDebuggerStarted)
				return;
			
			if (DebuggerService.DebugStepInformation != null) {
				// get debugging information
				DecompileInformation debugInformation = (DecompileInformation)DebuggerService.ExternalDebugInformation;
				int token = DebuggerService.DebugStepInformation.Item1;
				int ilOffset = DebuggerService.DebugStepInformation.Item2;
				int line;
				MemberReference member;
				if (!debugInformation.CodeMappings.ContainsKey(token))
					return;
				
				debugInformation.CodeMappings[token].GetInstructionByTokenAndOffset(token, ilOffset, out member, out line);
				
				// update bookmark & marker
				codeView.UnfoldAndScroll(line);
				CurrentLineBookmark.SetPosition(this, line, 0, line, 0);
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
		#endregion
	}
}
