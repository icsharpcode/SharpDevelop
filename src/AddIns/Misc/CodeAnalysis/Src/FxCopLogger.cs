/*
 * Created by SharpDevelop.
 * User: Daniel Grunwald
 * Date: 16.07.2006
 * Time: 20:29
 */

using System;
using Microsoft.Build.Framework;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

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
				engine.MessageView.AppendText("Running FxCop...\r\n");
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
				AppendError(e.File, e.LineNumber, e.ColumnNumber, e.Code, e.Message, false);
			}
			
			void OnWarning(object sender, BuildWarningEventArgs e)
			{
				AppendError(e.File, e.LineNumber, e.ColumnNumber, e.Code, e.Message, true);
			}
			
			void AppendError(string file, int lineNumber, int columnNumber, string code, string message, bool isWarning)
			{
				BuildError err = engine.CurrentErrorOrWarning;
				if (file.StartsWith("positionof#")) {
					string memberName = file.Substring(11);
					file = "";
					IProject project = ProjectService.GetProject(engine.CurrentProjectFile);
					if (project != null) {
						IProjectContent pc = ParserService.GetProjectContent(project);
						if (pc != null) {
							Position pos = pc.GetPosition(memberName);
							if (pos != null && pos.Cu != null) {
								err.FileName = pos.Cu.FileName ?? "";
								err.Line = pos.Line;
								err.Column = pos.Column;
							}
						}
					}
				}
			}
		}
	}
}
