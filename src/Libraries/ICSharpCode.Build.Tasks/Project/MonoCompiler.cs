// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.CodeDom.Compiler;
using System.IO;

namespace ICSharpCode.Build.Tasks
{
	public class MonoCompiler
	{
		CompilerResults results;
		
		public MonoCompiler()
		{
		}
		
		public int Run(string compiler, string args, ICompilerResultsParser parser)
		{
			string responseFileName = Path.GetTempFileName();
			
			using (StreamWriter writer = new StreamWriter(responseFileName)) {
				writer.Write(args);
			}
		
			//string outstr = String.Concat(compilerFileName, compilerparameters.NoConfig ? " /noconfig" : String.Empty, " \"@", responseFileName, "\"");
			string outputFileName = String.Empty;
			string errorFileName  = String.Empty;
			TempFileCollection tempFiles = new TempFileCollection();
			string command = String.Concat(compiler, " \"@", responseFileName, "\"");
			
			int returnValue = Executor.ExecWaitWithCapture(command, tempFiles, ref outputFileName, ref errorFileName);
			
			results = parser.Parse(tempFiles, outputFileName, errorFileName);
			
			File.Delete(responseFileName);
			File.Delete(outputFileName);
			File.Delete(errorFileName);
			
			return returnValue;
		}
		
		public CompilerResults Results {
			get {
				return results;
			}
		}
	}
}
