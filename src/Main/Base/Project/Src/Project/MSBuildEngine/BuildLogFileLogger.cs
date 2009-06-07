/*
 * Created by SharpDevelop.
 * User: Daniel
 * Date: 07.06.2009
 * Time: 14:56
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Microsoft.Build.Framework;
using Microsoft.Build.Logging;
using System.IO;

namespace ICSharpCode.SharpDevelop.Project
{
	sealed class BuildLogFileLogger : ConsoleLogger
	{
		string fileName;
		StreamWriter writer;
		
		public BuildLogFileLogger(string fileName, LoggerVerbosity verbosity)
			: base(verbosity)
		{
			this.fileName = fileName;
			base.WriteHandler = Write;
		}
		
		public override void Initialize(IEventSource eventSource)
		{
			OpenFile();
			base.Initialize(eventSource);
		}
		
		public override void Initialize(IEventSource eventSource, int nodeCount)
		{
			OpenFile();
			base.Initialize(eventSource, nodeCount);
		}
		
		void OpenFile()
		{
			writer = new StreamWriter(fileName);
		}
		
		public override void Shutdown()
		{
			base.Shutdown();
			writer.Close();
			writer = null;
		}
		
		void Write(string text)
		{
			if (writer != null)
				writer.Write(text);
		}
	}
}
