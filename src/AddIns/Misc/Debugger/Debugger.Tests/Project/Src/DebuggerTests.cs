// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using Debugger;
using Microsoft.CSharp;
using NUnit.Framework;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Threading;

namespace DebuggerLibrary.Tests
{
	/// <summary>
	/// This class contains methods that test the debugger
	/// </summary>
	[TestFixture]
	public class DebuggerTests
	{
		string resourcePrefix = "Debugger.Tests.Src.TestPrograms.";
		string tempPath = MakeTempDirectory();
		Hashtable programs = new Hashtable();
		
		NDebugger debugger = new NDebugger();
		
		public DebuggerTests()
		{
			CompileTestPrograms();
		}
		
		public void CompileTestPrograms()
		{
			Assembly assembly = Assembly.GetExecutingAssembly();
			foreach(string name in assembly.GetManifestResourceNames()) {
				if (name.StartsWith(resourcePrefix)) {
					string programName = name.Substring(resourcePrefix.Length, name.Length - resourcePrefix.Length - ".cs".Length);
					
					Stream codeStream = assembly.GetManifestResourceStream(name);
					string code = new StreamReader(codeStream).ReadToEnd();
					
					string codeFilename = Path.Combine(tempPath, programName + ".cs");
					string exeFilename = Path.Combine(tempPath, programName + ".exe");
					
					StreamWriter file = new StreamWriter(codeFilename);
					file.Write(code);
					file.Close();
					
					CompilerParameters compParams = new CompilerParameters();
					compParams.GenerateExecutable = true;
					compParams.GenerateInMemory = false;
					compParams.TreatWarningsAsErrors = false;
					compParams.IncludeDebugInformation = true;
					compParams.ReferencedAssemblies.Add("System.dll");
					compParams.OutputAssembly = exeFilename;
					
					CSharpCodeProvider compiler = new CSharpCodeProvider();
					CompilerResults result = compiler.CompileAssemblyFromFile(compParams, codeFilename);
					
					if (result.Errors.Count > 0) {
						throw new System.Exception("There was an error(s) during compilation of test program:\n" + result.Errors[0].ToString());
					}
					
					programs.Add(programName, exeFilename);
				}
			}
		}
		
		~DebuggerTests()
		{
			//Directory.Delete(tempPath, true);
		}
		
		static string MakeTempDirectory()
		{
			Random rand = new Random();
			string path;
			do {
				path = Path.Combine(Path.GetTempPath(), "SharpDevelop");
				path = Path.Combine(path, "DebuggerTests" + rand.Next(10000,99999));
			} while (Directory.Exists(path));
			Directory.CreateDirectory(path);
			return path;
		}
		
		
		[Test, Ignore("Disabled because of deadlock problem")]
		public void RunSimpleProgram()
		{
			ManualResetEvent exitedEvent = new ManualResetEvent(false);
			debugger.ProcessExited += delegate {
				exitedEvent.Set();
			};
			debugger.Start((string)programs["SimpleProgram"], tempPath, "");
			if (!exitedEvent.WaitOne(1000, false)) {
				throw new System.Exception("Time out");
			}
		}
	}
}
