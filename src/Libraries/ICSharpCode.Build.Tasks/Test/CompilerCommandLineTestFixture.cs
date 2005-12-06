// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Build.Tasks;
using NUnit.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;

namespace ICSharpCode.Build.Tasks.Tests
{
	[TestFixture]
	public class CompilerCommandLineTestFixture
	{
		[Test]
		public void NoArguments()
		{
			Mcs mcs = new Mcs();
			
			CompilerCommandLineArguments args = new CompilerCommandLineArguments(mcs);
			Assert.AreEqual("-warn:0", args.ToString());
		}
		
		[Test]
		public void OutputAssembly()
		{
			Mcs mcs = new Mcs();
			string outputAssembly = @"obj\debug\test.exe";
			mcs.OutputAssembly = new TaskItem(outputAssembly);
			
			CompilerCommandLineArguments args = new CompilerCommandLineArguments(mcs);
			Assert.AreEqual(@"-warn:0 -out:obj\debug\test.exe", args.ToString());
		}
		
		[Test]
		public void OutputAssemblyWithSpace()
		{
			Mcs mcs = new Mcs();
			string outputAssembly = @"obj\debug\test this.exe";
			mcs.OutputAssembly = new TaskItem(outputAssembly);
			
			CompilerCommandLineArguments args = new CompilerCommandLineArguments(mcs);
			Assert.AreEqual("-warn:0 -out:\"obj\\debug\\test this.exe\"", args.ToString());			
		}
		
		[Test]
		public void WinExeTarget()
		{
			Mcs mcs = new Mcs();
			mcs.TargetType = "Exe";
			
			CompilerCommandLineArguments args = new CompilerCommandLineArguments(mcs);
			Assert.AreEqual("-warn:0 -target:exe", args.ToString());			
		}
		
		[Test]
		public void ModuleTarget()
		{
			Mcs mcs = new Mcs();
			mcs.TargetType = "Module";
			
			CompilerCommandLineArguments args = new CompilerCommandLineArguments(mcs);
			Assert.AreEqual("-warn:0 -target:module", args.ToString());			
		}
		
		[Test]
		public void FullDebugging()
		{
			Mcs mcs = new Mcs();
			mcs.DebugType = "Full";
			
			CompilerCommandLineArguments args = new CompilerCommandLineArguments(mcs);
			Assert.AreEqual("-warn:0 -debug", args.ToString());						
		}
		
		[Test]
		public void Optimize()
		{
			Mcs mcs = new Mcs();
			mcs.Optimize = true;
			
			CompilerCommandLineArguments args = new CompilerCommandLineArguments(mcs);
			Assert.AreEqual("-warn:0 -optimize", args.ToString());						
		}
		
		[Test]
		public void NoLogo()
		{
			Mcs mcs = new Mcs();
			mcs.NoLogo = true;
			
			CompilerCommandLineArguments args = new CompilerCommandLineArguments(mcs);
			Assert.AreEqual("-warn:0 -nologo", args.ToString());						
		}		
		
		[Test]
		public void Unsafe()
		{
			Mcs mcs = new Mcs();
			mcs.AllowUnsafeBlocks = true;
			
			CompilerCommandLineArguments args = new CompilerCommandLineArguments(mcs);
			Assert.AreEqual("-warn:0 -unsafe", args.ToString());						
		}	
		
		[Test]
		public void NoStandardLib()
		{
			Mcs mcs = new Mcs();
			mcs.NoStandardLib = true;
			
			CompilerCommandLineArguments args = new CompilerCommandLineArguments(mcs);
			Assert.AreEqual("-warn:0 -nostdlib", args.ToString());						
		}			
		
		[Test]
		public void DelaySign()
		{
			Mcs mcs = new Mcs();
			mcs.DelaySign = true;
			
			CompilerCommandLineArguments args = new CompilerCommandLineArguments(mcs);
			Assert.AreEqual("-warn:0 -delaysign", args.ToString());						
		}	
		
		[Test]
		public void DefineConstants()
		{
			Mcs mcs = new Mcs();
			mcs.DefineConstants = "DEBUG;TRACE";
			
			CompilerCommandLineArguments args = new CompilerCommandLineArguments(mcs);
			Assert.AreEqual("-warn:0 -define:\"DEBUG;TRACE\"", args.ToString());						
		}	
		
		[Test]
		public void WarnAsError()
		{
			Mcs mcs = new Mcs();
			mcs.TreatWarningsAsErrors = true;
			
			CompilerCommandLineArguments args = new CompilerCommandLineArguments(mcs);
			Assert.AreEqual("-warn:0 -warnaserror", args.ToString());						
		}	
		
		[Test]
		public void NoWarn()
		{
			Mcs mcs = new Mcs();
			mcs.DisabledWarnings = "1234,5678";
			
			CompilerCommandLineArguments args = new CompilerCommandLineArguments(mcs);
			Assert.AreEqual("-warn:0 -nowarn:\"1234,5678\"", args.ToString());						
		}	
		
		[Test]
		public void MainEntryPoint()
		{
			Mcs mcs = new Mcs();
			mcs.MainEntryPoint = "Console.MainClass.Main";
			
			CompilerCommandLineArguments args = new CompilerCommandLineArguments(mcs);
			Assert.AreEqual("-warn:0 -main:Console.MainClass.Main", args.ToString());						
		}	
		
		[Test]
		public void DocumentationFile()
		{
			Mcs mcs = new Mcs();
			mcs.DocumentationFile = @"obj\debug test\test.exe.xml";
			
			CompilerCommandLineArguments args = new CompilerCommandLineArguments(mcs);
			Assert.AreEqual("-warn:0 -doc:\"obj\\debug test\\test.exe.xml\"", args.ToString());
		}
		
		[Test]
		public void SingleSourceFile()
		{
			Mcs mcs = new Mcs();
			mcs.Sources = new TaskItem[] { new TaskItem("proj src\\Main.cs") };
			
			CompilerCommandLineArguments args = new CompilerCommandLineArguments(mcs);
			Assert.AreEqual("-warn:0 \"proj src\\Main.cs\"", args.ToString());
		}
		
		[Test]
		public void MultipleSourceFiles()
		{
			Mcs mcs = new Mcs();
			mcs.Sources = new TaskItem[] { new TaskItem("proj src\\Main.cs"), 
				new TaskItem("AssemblyInfo.cs") };
			
			CompilerCommandLineArguments args = new CompilerCommandLineArguments(mcs);
			Assert.AreEqual("-warn:0 \"proj src\\Main.cs\" AssemblyInfo.cs", args.ToString());
		}
		
		[Test]
		public void SingleReference()
		{
			Mcs mcs = new Mcs();
			mcs.References = new TaskItem[] { new TaskItem("proj refs\\Test.dll") };
			
			CompilerCommandLineArguments args = new CompilerCommandLineArguments(mcs);
			Assert.AreEqual("-warn:0 -r:\"proj refs\\Test.dll\"", args.ToString());
		}
		
		[Test]
		public void NetModuleReference()
		{
			Mcs mcs = new Mcs();
			mcs.References = new TaskItem[] { new TaskItem("proj refs\\Test.dll"),
				new TaskItem("proj refs\\Run.netmodule") };
			
			CompilerCommandLineArguments args = new CompilerCommandLineArguments(mcs);
			Assert.AreEqual("-warn:0 -r:\"proj refs\\Test.dll\" -addmodule:\"proj refs\\Run.netmodule\"", args.ToString());
		}	
		
		[Test]
		public void AdditionalLibPaths()
		{
			Mcs mcs = new Mcs();
			mcs.AdditionalLibPaths = new string[] { "proj\\My libs", "proj\\My libs2" };
			
			CompilerCommandLineArguments args = new CompilerCommandLineArguments(mcs);
			Assert.AreEqual("-warn:0 -lib:\"proj\\My libs\",\"proj\\My libs2\"", args.ToString());
		}		
		
		[Test]
		public void EmbeddedResources()
		{
			Mcs mcs = new Mcs();
			mcs.Resources = new TaskItem[] { new TaskItem("proj res\\Test.xml"),
				new TaskItem("proj res\\Run.xml") };
			
			CompilerCommandLineArguments args = new CompilerCommandLineArguments(mcs);
			Assert.AreEqual("-warn:0 -resource:\"proj res\\Test.xml\" -resource:\"proj res\\Run.xml\"", args.ToString());
		}	
		
		[Test]
		public void Win32Resource()
		{
			Mcs mcs = new Mcs();
			mcs.Win32Resource = "Project Resources\\Test.res";
			
			CompilerCommandLineArguments args = new CompilerCommandLineArguments(mcs);
			Assert.AreEqual("-warn:0 -win32res:\"Project Resources\\Test.res\"", args.ToString());
		}	
		
		[Test]
		public void Win32Icon()
		{
			Mcs mcs = new Mcs();
			mcs.Win32Icon = "Project Icons\\app.ico";
			
			CompilerCommandLineArguments args = new CompilerCommandLineArguments(mcs);
			Assert.AreEqual("-warn:0 -win32icon:\"Project Icons\\app.ico\"", args.ToString());
		}	
		
		[Test]
		public void Checked()
		{
			Mcs mcs = new Mcs();
			mcs.CheckForOverflowUnderflow = true;
			
			CompilerCommandLineArguments args = new CompilerCommandLineArguments(mcs);
			Assert.AreEqual("-warn:0 -checked", args.ToString());						
		}	
	}
}
