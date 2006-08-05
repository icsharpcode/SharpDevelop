// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using Microsoft.Build.Framework;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.CodeAnalysis
{
	public class FxCopLogger : IMSBuildAdditionalLogger
	{
		public ILogger CreateLogger(MSBuildEngine engine)
		{
			return new FxCopLoggerImpl(engine);
		}
		
		private class FxCopLoggerImpl : ILogger
		{
			MSBuildEngine engine;
			
			public FxCopLoggerImpl(MSBuildEngine engine)
			{
				this.engine = engine;
			}
			
			public LoggerVerbosity Verbosity {
				get {
					throw new NotImplementedException();
				}
				set {
					throw new NotImplementedException();
				}
			}
			
			public string Parameters {
				get {
					throw new NotImplementedException();
				}
				set {
					throw new NotImplementedException();
				}
			}
			
			IEventSource eventSource;
			
			public void Initialize(IEventSource eventSource)
			{
				this.eventSource = eventSource;
				engine.MessageView.AppendText("Running FxCop on " + Path.GetFileNameWithoutExtension(engine.CurrentProjectFile) + "\r\n");
				eventSource.ErrorRaised += OnError;
				eventSource.WarningRaised += OnWarning;
			}
			
			public void Shutdown()
			{
				if (eventSource != null) {
					eventSource.ErrorRaised -= OnError;
					eventSource.WarningRaised -= OnWarning;
					eventSource = null;
				}
			}
			
			void OnError(object sender, BuildErrorEventArgs e)
			{
				AppendError(e.File, e.LineNumber, e.ColumnNumber, e.Message, false,
				            e.HelpKeyword, e.Code, e.Subcategory);
			}
			
			void OnWarning(object sender, BuildWarningEventArgs e)
			{
				AppendError(e.File, e.LineNumber, e.ColumnNumber, e.Message, true,
				            e.HelpKeyword, e.Code, e.Subcategory);
			}
			
			void AppendError(string file, int lineNumber, int columnNumber,
			                 string message, bool isWarning,
			                 string category, string checkId, string subcategory)
			{
				string[] moreData = (subcategory ?? "").Split('|');
				BuildError err = engine.CurrentErrorOrWarning;
				if (Path.GetFileName(file) == "SharpDevelop.CodeAnalysis.targets") {
					err.FileName = null;
				}
				IProject project = ProjectService.GetProject(engine.CurrentProjectFile);
				if (project != null) {
					IProjectContent pc = ParserService.GetProjectContent(project);
					if (pc != null) {
						if (file.StartsWith("positionof#")) {
							string memberName = file.Substring(11);
							file = "";
							FilePosition pos = pc.GetPosition(memberName);
							if (pos.IsEmpty == false && pos.CompilationUnit != null) {
								err.FileName = pos.FileName ?? "";
								err.Line = pos.Line;
								err.Column = pos.Column;
							} else {
								err.FileName = null;
							}
						}
						
						if (moreData.Length > 0 && moreData[0].Length > 0) {
							err.Tag = new FxCopTaskTag(pc, moreData[0], category, checkId);
							err.ContextMenuAddInTreeEntry = "/SharpDevelop/Pads/ErrorList/CodeAnalysisTaskContextMenu";
							if (moreData.Length > 1) {
								(err.Tag as FxCopTaskTag).MessageID = moreData[1];
							}
						}
					}
				}
			}
		}
	}
	
	public class FxCopTaskTag
	{
		IProjectContent projectContent;
		string memberName;
		string category;
		string checkID;
		string messageID;
		
		public IProjectContent ProjectContent {
			get {
				return projectContent;
			}
		}
		
		public string MemberName {
			get {
				return memberName;
			}
		}
		
		public string Category {
			get {
				return category;
			}
		}
		
		public string CheckID {
			get {
				return checkID;
			}
		}
		
		public string MessageID {
			get {
				return messageID;
			}
			set {
				messageID = value;
			}
		}
		
		public FxCopTaskTag(IProjectContent projectContent, string memberName, string category, string checkID)
		{
			this.projectContent = projectContent;
			this.memberName = memberName;
			this.category = category;
			this.checkID = checkID;
		}
	}
}
