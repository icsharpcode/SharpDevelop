// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
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
	public class MonoCSharpCompilerCommandLineTestFixture
	{
		[Test]
		public void NoArguments()
		{
			MockMonoCSharpCompilerTask mcs = new MockMonoCSharpCompilerTask();
			Assert.AreEqual(String.Empty, mcs.GetCommandLine());
		}
		
		[Test]
		public void OutputAssembly()
		{
			MockMonoCSharpCompilerTask mcs = new MockMonoCSharpCompilerTask();
			string outputAssembly = @"obj\debug\test.exe";
			mcs.OutputAssembly = new TaskItem(outputAssembly);
			Assert.AreEqual(@"-out:obj\debug\test.exe", mcs.GetCommandLine());
		}
		
		[Test]
		public void OutputAssemblyWithSpace()
		{
			MockMonoCSharpCompilerTask mcs = new MockMonoCSharpCompilerTask();
			string outputAssembly = @"obj\debug\test this.exe";
			mcs.OutputAssembly = new TaskItem(outputAssembly);
			Assert.AreEqual("-out:\"obj\\debug\\test this.exe\"", mcs.GetCommandLine());
		}
		
		[Test]
		public void WinExeTarget()
		{
			MockMonoCSharpCompilerTask mcs = new MockMonoCSharpCompilerTask();
			mcs.TargetType = "Exe";
			Assert.AreEqual("-target:exe", mcs.GetCommandLine());	
		}
		
		[Test]
		public void ModuleTarget()
		{
			MockMonoCSharpCompilerTask mcs = new MockMonoCSharpCompilerTask();
			mcs.TargetType = "Module";
			Assert.AreEqual("-target:module", mcs.GetCommandLine());	
		}
		
		[Test]
		public void EmitDebuggingInfo()
		{
			MockMonoCSharpCompilerTask mcs = new MockMonoCSharpCompilerTask();
			mcs.EmitDebugInformation = true;
			Assert.AreEqual("-debug", mcs.GetCommandLine());			
		}
		
		[Test]
		public void Optimize()
		{
			MockMonoCSharpCompilerTask mcs = new MockMonoCSharpCompilerTask();
			mcs.Optimize = true;
			Assert.AreEqual("-optimize", mcs.GetCommandLine());			
		}
		
		[Test]
		public void NoLogo()
		{
			MockMonoCSharpCompilerTask mcs = new MockMonoCSharpCompilerTask();
			mcs.NoLogo = true;
			Assert.AreEqual("-nologo", mcs.GetCommandLine());
		}		
		
		[Test]
		public void Unsafe()
		{
			MockMonoCSharpCompilerTask mcs = new MockMonoCSharpCompilerTask();
			mcs.AllowUnsafeBlocks = true;
			Assert.AreEqual("-unsafe", mcs.GetCommandLine());					
		}	
		
		[Test]
		public void NoStandardLib()
		{
			MockMonoCSharpCompilerTask mcs = new MockMonoCSharpCompilerTask();
			mcs.NoStandardLib = true;
			Assert.AreEqual("-nostdlib", mcs.GetCommandLine());				
		}			
		
		[Test]
		public void DelaySign()
		{
			MockMonoCSharpCompilerTask mcs = new MockMonoCSharpCompilerTask();
			mcs.DelaySign = true;			
			Assert.AreEqual("-delaysign", mcs.GetCommandLine());				
		}	
		
		[Test]
		public void DefineConstants()
		{
			MockMonoCSharpCompilerTask mcs = new MockMonoCSharpCompilerTask();
			mcs.DefineConstants = "DEBUG;TRACE";
			Assert.AreEqual("-define:\"DEBUG;TRACE\"", mcs.GetCommandLine());						
		}	
		
		[Test]
		public void WarnAsError()
		{
			MockMonoCSharpCompilerTask mcs = new MockMonoCSharpCompilerTask();
			mcs.TreatWarningsAsErrors = true;
			Assert.AreEqual("-warnaserror", mcs.GetCommandLine());					
		}	
		
		[Test]
		public void DisabledWarnings()
		{
			MockMonoCSharpCompilerTask mcs = new MockMonoCSharpCompilerTask();
			mcs.DisabledWarnings = "1234,5678";
			Assert.AreEqual("-nowarn:\"1234,5678\"", mcs.GetCommandLine());					
		}	
		
		[Test]
		public void MainEntryPoint()
		{
			MockMonoCSharpCompilerTask mcs = new MockMonoCSharpCompilerTask();
			mcs.MainEntryPoint = "Console.MainClass.Main";
			Assert.AreEqual("-main:Console.MainClass.Main", mcs.GetCommandLine());					
		}	
		
		[Test]
		public void DocumentationFile()
		{
			MockMonoCSharpCompilerTask mcs = new MockMonoCSharpCompilerTask();
			mcs.DocumentationFile = @"obj\debug test\test.exe.xml";
			Assert.AreEqual("-doc:\"obj\\debug test\\test.exe.xml\"", mcs.GetCommandLine());
		}
		
		[Test]
		public void SingleSourceFile()
		{
			MockMonoCSharpCompilerTask mcs = new MockMonoCSharpCompilerTask();
			mcs.Sources = new TaskItem[] { new TaskItem("proj src\\Main.cs") };
			Assert.AreEqual("\"proj src\\Main.cs\"", mcs.GetCommandLine());
		}
		
		[Test]
		public void MultipleSourceFiles()
		{
			MockMonoCSharpCompilerTask mcs = new MockMonoCSharpCompilerTask();
			mcs.Sources = new TaskItem[] { new TaskItem("proj src\\Main.cs"), 
				new TaskItem("AssemblyInfo.cs") };
			Assert.AreEqual("\"proj src\\Main.cs\" AssemblyInfo.cs", mcs.GetCommandLine());
		}
		
		[Test]
		public void SingleReference()
		{
			MockMonoCSharpCompilerTask mcs = new MockMonoCSharpCompilerTask();
			mcs.References = new TaskItem[] { new TaskItem("proj refs\\Test.dll") };
			Assert.AreEqual("-r:\"proj refs\\Test.dll\"", mcs.GetCommandLine());
		}
		
		[Test]
		public void NetModuleReference()
		{
			MockMonoCSharpCompilerTask mcs = new MockMonoCSharpCompilerTask();
			mcs.References = new TaskItem[] { new TaskItem("proj refs\\Test.dll"),
				new TaskItem("proj refs\\Run.netmodule") };
			Assert.AreEqual("-r:\"proj refs\\Test.dll\" -addmodule:\"proj refs\\Run.netmodule\"", mcs.GetCommandLine());
		}	
		
		[Test]
		public void AdditionalLibPaths()
		{
			MockMonoCSharpCompilerTask mcs = new MockMonoCSharpCompilerTask();
			mcs.AdditionalLibPaths = new string[] { "proj\\My libs", "proj\\My libs2" };
			Assert.AreEqual("-lib:\"proj\\My libs\",\"proj\\My libs2\"", mcs.GetCommandLine());
		}		
		
		[Test]
		public void EmbeddedResources()
		{
			MockMonoCSharpCompilerTask mcs = new MockMonoCSharpCompilerTask();
			mcs.Resources = new TaskItem[] { new TaskItem("proj res\\Test.xml"),
				new TaskItem("proj res\\Run.xml") };
			Assert.AreEqual("-resource:\"proj res\\Test.xml\" -resource:\"proj res\\Run.xml\"", mcs.GetCommandLine());
		}	
		
		[Test]
		public void Win32Resource()
		{
			MockMonoCSharpCompilerTask mcs = new MockMonoCSharpCompilerTask();
			mcs.Win32Resource = "Project Resources\\Test.res";
			Assert.AreEqual("-win32res:\"Project Resources\\Test.res\"", mcs.GetCommandLine());
		}	
		
		[Test]
		public void Win32Icon()
		{
			MockMonoCSharpCompilerTask mcs = new MockMonoCSharpCompilerTask();
			mcs.Win32Icon = "Project Icons\\app.ico";
			Assert.AreEqual("-win32icon:\"Project Icons\\app.ico\"", mcs.GetCommandLine());
		}	
		
		[Test]
		public void Checked()
		{
			MockMonoCSharpCompilerTask mcs = new MockMonoCSharpCompilerTask();
			mcs.CheckForOverflowUnderflow = true;
			Assert.AreEqual("-checked", mcs.GetCommandLine());			
		}	
		
		[Test]
		public void WarningLevel()
		{
			MockMonoCSharpCompilerTask mcs = new MockMonoCSharpCompilerTask();
			mcs.WarningLevel = 3;
			Assert.AreEqual("-warn:3", mcs.GetCommandLine());						
		}
	}
}
