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

namespace VBNetBinding
{
	/*
	public class VbcEncodingFixingLogger : IMSBuildAdditionalLogger
	{
		public ILogger CreateLogger(MSBuildEngine engine)
		{
			return new VbcLoggerImpl(engine);
		}
		
		private class VbcLoggerImpl : ILogger
		{
			MSBuildEngine engine;
			
			public VbcLoggerImpl(MSBuildEngine engine)
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
				FixMessage();
			}
			
			void OnWarning(object sender, BuildWarningEventArgs e)
			{
				FixMessage();
			}
			
			void FixMessage()
			{
				engine.CurrentErrorOrWarning.ErrorText = FixEncoding(engine.CurrentErrorOrWarning.ErrorText);
				//engine.CurrentErrorOrWarning.FileName = FixEncoding(engine.CurrentErrorOrWarning.FileName);
			}
			
			TODO: Fix SD2-995
			
			static string FixEncoding(string encoding)
			{
				return encoding;
			}
		}
	}
	*/
}
