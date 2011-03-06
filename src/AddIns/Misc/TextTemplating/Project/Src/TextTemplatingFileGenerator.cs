// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.TextTemplating
{
	public class TextTemplatingFileGenerator : ITextTemplatingFileGenerator
	{
		ITextTemplatingHost host;
		FileProjectItem projectFile;
		ITextTemplatingCustomToolContext context;
		
		public TextTemplatingFileGenerator(
			ITextTemplatingHost host,
			FileProjectItem projectFile,
			ITextTemplatingCustomToolContext context)
		{
			this.host = host;
			this.projectFile = projectFile;
			this.context = context;
		}
		
		public void Dispose()
		{
			host.Dispose();
		}
		
		public void ProcessTemplate()
		{
			GenerateOutputFileForTemplate();	
			AddOutputFileToProjectIfRequired();
		}

		void GenerateOutputFileForTemplate()
		{
			string inputFileName = projectFile.FileName;
			string outputFileName = GetOutputFileName(inputFileName);
			host.ProcessTemplate(inputFileName, outputFileName);
		}
		
		string GetOutputFileName(string inputFileName)
		{
			return Path.ChangeExtension(inputFileName, ".cs");
		}

		void AddOutputFileToProjectIfRequired()
		{
			context.EnsureOutputFileIsInProject(projectFile, host.OutputFile);
		}

		
//		internal static void LogicalSetData (string name, object value,
//			System.CodeDom.Compiler.CompilerErrorCollection errors)
//		{
//			//FIXME: CallContext.LogicalSetData not implemented in Mono
//			try {
//				System.Runtime.Remoting.Messaging.CallContext.LogicalSetData (name, value);
//			} catch (NotImplementedException) {
//				errors.Add (new System.CodeDom.Compiler.CompilerError (
//					null, -1, -1, null,
//					"Could not set " + name +  " - CallContext.LogicalSetData not implemented in this Mono version"
//				) { IsWarning = true });
//			}
//		}
	}
}