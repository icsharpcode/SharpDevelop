// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using Microsoft.Build.Framework;

namespace ICSharpCode.CodeAnalysis
{
	/// <summary>
	/// Not all FxCop messages come with correct line number information,
	/// so this logger fixes the position.
	/// Additionally, it registers the context menu containing the 'suppress message' command.
	/// </summary>
	public class FxCopLogger : IMSBuildAdditionalLogger
	{
		public ILogger CreateLogger(MSBuildEngine engineWorker)
		{
			return new FxCopLoggerImpl(engineWorker);
		}
		
		private class FxCopLoggerImpl : ILogger
		{
			MSBuildEngine engineWorker;
			
			public FxCopLoggerImpl(MSBuildEngine engineWorker)
			{
				this.engineWorker = engineWorker;
			}
			
			public LoggerVerbosity Verbosity { get; set; }
			public string Parameters { get; set; }
			
			IEventSource eventSource;
			
			public void Initialize(IEventSource eventSource)
			{
				this.eventSource = eventSource;
				engineWorker.OutputText("${res:ICSharpCode.CodeAnalysis.RunningFxCopOn} " + Path.GetFileNameWithoutExtension(engineWorker.CurrentProjectFile));
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
				LoggingService.Debug("Got " + (isWarning ? "warning" : "error") + ":\n"
				                     + "  file: " + file + "\n"
				                     + "  line: " + lineNumber + ", col: " + columnNumber + "\n"
				                     + "  message: " + message + "\n"
				                     + "  category: " + category + "\n"
				                     + "  checkId: " + checkId + "\n"
				                     + "  subcategory: " + subcategory);
				
				string[] moreData = (subcategory ?? "").Split('|');
				BuildError err = engineWorker.CurrentErrorOrWarning;
				err.ErrorCode = (checkId != null) ? checkId.Split(':')[0] : null;
				if (FileUtility.IsValidPath(file) &&
				    Path.GetFileName(file) == "SharpDevelop.CodeAnalysis.targets")
				{
					err.FileName = null;
				}
				IProject project = ProjectService.GetProject(engineWorker.CurrentProjectFile);
				if (project != null) {
					IProjectContent pc = ParserService.GetProjectContent(project);
					if (pc != null) {
						if (file.StartsWith("positionof#")) {
							string memberName = file.Substring(11);
							file = "";
							FilePosition pos = GetPosition(pc, memberName);
							if (pos.IsEmpty == false && pos.CompilationUnit != null) {
								err.FileName = pos.FileName ?? "";
								err.Line = pos.Line;
								err.Column = pos.Column;
							} else {
								err.FileName = null;
							}
						}
						
						if (moreData.Length > 1 && !string.IsNullOrEmpty(moreData[0])) {
							err.Tag = new FxCopTaskTag {
								ProjectContent = pc,
								TypeName = moreData[0],
								MemberName = moreData[1],
								Category = category,
								CheckID = checkId
							};
						} else {
							err.Tag = new FxCopTaskTag {
								ProjectContent = pc,
								Category = category,
								CheckID = checkId
							};
						}
						err.ContextMenuAddInTreeEntry = "/SharpDevelop/Pads/ErrorList/CodeAnalysisTaskContextMenu";
						if (moreData.Length > 2) {
							(err.Tag as FxCopTaskTag).MessageID = moreData[2];
						}
					}
				}
			}
			
			static FilePosition GetPosition(IProjectContent pc, string memberName)
			{
				// memberName is a special syntax used by our FxCop task:
				// className#memberName
				int pos = memberName.IndexOf('#');
				if (pos <= 0)
					return FilePosition.Empty;
				string className = memberName.Substring(0, pos);
				memberName = memberName.Substring(pos + 1);
				return SuppressMessageCommand.GetPosition(pc, className, memberName);
			}
		}
	}
	
	public class FxCopTaskTag
	{
		public IProjectContent ProjectContent { get; set; }
		public string TypeName { get; set; }
		public string MemberName { get; set; }
		public string Category { get; set; }
		public string CheckID { get; set; }
		public string MessageID { get; set; }
	}
}
