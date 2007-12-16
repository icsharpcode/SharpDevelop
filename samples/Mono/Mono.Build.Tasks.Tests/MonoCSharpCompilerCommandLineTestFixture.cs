// SharpDevelop samples
// Copyright (c) 2006, AlphaSierraPapa
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, are
// permitted provided that the following conditions are met:
//
// - Redistributions of source code must retain the above copyright notice, this list
//   of conditions and the following disclaimer.
//
// - Redistributions in binary form must reproduce the above copyright notice, this list
//   of conditions and the following disclaimer in the documentation and/or other materials
//   provided with the distribution.
//
// - Neither the name of the SharpDevelop team nor the names of its contributors may be used to
//   endorse or promote products derived from this software without specific prior written
//   permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS &AS IS& AND ANY EXPRESS
// OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER
// IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT
// OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using Mono.Build.Tasks;
using NUnit.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;

namespace Mono.Build.Tasks.Tests
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
