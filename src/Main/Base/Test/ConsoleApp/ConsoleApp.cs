// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace ICSharpCode.NAntAddIn.Tests.ConsoleApp
{
	public class ConsoleApp
	{
		/// <summary>
		/// Command line argument telling the app to echo the parameter text
		/// back to the caller.
		/// </summary>
		public static readonly string EchoArgument = "-echo";
		
		/// <summary>
		/// Command line argument telling the app to send the contents of
		/// the text file back to the caller via standard output.
		/// </summary>
		public static readonly string FileArgument = "-file";

		/// <summary>
		/// Command line argument telling the app to echo the parameter text
		/// back to the caller via standard error.
		/// </summary>
		public static readonly string ErrorEchoArgument = "-error.echo";
		
		/// <summary>
		/// Command line argument telling the app to send the contents of
		/// the text file back to the caller via standard error.
		/// </summary>
		public static readonly string ErrorFileArgument = "-error.file";

		/// <summary>
		/// Command line argument telling the app to return a 
		/// particular exit code.
		/// </summary>
		public static readonly string ExitCodeArgument = "-exitcode";
		
		/// <summary>
		/// Argument that will keep the console app running forever until
		/// it is killed.
		/// </summary>
		public static readonly string ForeverArgument = "-forever";
		
		public static int Main(string[] args)
		{
			ConsoleApp App = new ConsoleApp();
			return App.Run(args);
		}
		
		public int Run(string[] args)
		{
			int exitCode = 0;
			
			if (args.Length > 0) {
				string firstArg = args[0];
				
				if (firstArg.StartsWith(EchoArgument)) {
					Console.WriteLine(firstArg.Substring(EchoArgument.Length + 1));
				} else if(firstArg.StartsWith(FileArgument)) {
					string outputText = ReadTextFile(firstArg.Substring(FileArgument.Length + 1));
					Console.Write(outputText);
				} else if(firstArg.StartsWith(ErrorEchoArgument)) {
					Console.Error.WriteLine(firstArg.Substring(ErrorEchoArgument.Length + 1));				          	
				} else if(firstArg.StartsWith(ErrorFileArgument)) {
					string outputText = ReadTextFile(firstArg.Substring(ErrorFileArgument.Length + 1));
					Console.Error.Write(outputText);
				} else if(firstArg.StartsWith(ExitCodeArgument)) {
					exitCode = Convert.ToInt32(firstArg.Substring(ExitCodeArgument.Length + 1));
				} else if(firstArg == ForeverArgument) {
					Thread.Sleep(Timeout.Infinite);
				}
			}
			
			return exitCode;
		}
		
		private string ReadTextFile(string filename)
		{			
			FileStream stream = File.Open(filename, FileMode.Open);
			
			byte[] bytesRead = new byte[stream.Length];
			
			stream.Read(bytesRead, 0, bytesRead.Length);
			string readText = UnicodeEncoding.UTF8.GetString(bytesRead);
	
			stream.Close();
			
			return readText;
		}
	}
}
