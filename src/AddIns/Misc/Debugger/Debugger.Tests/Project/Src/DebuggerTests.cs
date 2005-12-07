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
		
		
		[Test]
		public void RunSimpleProgram()
		{
			debugger.Start((string)programs["SimpleProgram"], tempPath, "");
			debugger.WaitForPrecessExit();
		}
		
		[Test]
		public void RunDiagnosticsDebugHelloWorld()
		{
			string log = "";
			debugger.LogMessage += delegate(object sender, MessageEventArgs e) { log += e.Message; };
			debugger.Start((string)programs["DiagnosticsDebugHelloWorld"], tempPath, "");
			debugger.WaitForPrecessExit();
			
			Assert.AreEqual("Hello world!\r\n", log);
		}
		
		[Test]
		public void RunDiagnosticsDebugHelloWorldWithBreakpoint()
		{
			string log = "";
			debugger.LogMessage += delegate(object sender, MessageEventArgs e) { log += e.Message; };
			
			string souceCodeFilename = ((string)programs["DiagnosticsDebugHelloWorld"]).Replace(".exe",".cs");
			debugger.AddBreakpoint(new SourcecodeSegment(souceCodeFilename, 16), true);
			
			debugger.Start((string)programs["DiagnosticsDebugHelloWorld"], tempPath, "");
			debugger.WaitForPause();
			
			Assert.AreEqual(PausedReason.Breakpoint, debugger.PausedReason);
			Assert.AreEqual("", log);
			
			debugger.Continue();
			
			debugger.WaitForPrecessExit();
			
			Assert.AreEqual("Hello world!\r\n", log);
		}
	}
}
