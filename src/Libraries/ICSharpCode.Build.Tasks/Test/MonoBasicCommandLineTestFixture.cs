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
	public class MonoBasicCommandLineTestFixture
	{
		[Test]
		public void NoArgs()
		{
			MockMbas mbas = new MockMbas();
			Assert.AreEqual(String.Empty, mbas.GetCommandLine());
		}
		
		[Test]
		public void NoStandardLib()
		{
			MockMbas mbas = new MockMbas();
			mbas.NoStandardLib = true;
			Assert.AreEqual("-nostdlib", mbas.GetCommandLine());				
		}	
		
		[Test]
		public void OutputAssembly()
		{
			MockMbas mbas = new MockMbas();
			string outputAssembly = @"obj\debug\test.exe";
			mbas.OutputAssembly = new TaskItem(outputAssembly);
			Assert.AreEqual(@"-out:obj\debug\test.exe", mbas.GetCommandLine());
		}
		
		[Test]
		public void Unsafe()
		{
			MockMbas mbas = new MockMbas();
			mbas.AllowUnsafeBlocks = true;
			Assert.AreEqual("-unsafe", mbas.GetCommandLine());					
		}	
		
		[Test]
		public void WarnAsError()
		{
			MockMbas mbas = new MockMbas();
			mbas.TreatWarningsAsErrors = true;
			Assert.AreEqual("-warnaserror", mbas.GetCommandLine());					
		}	
		
		[Test]
		public void WinExeTarget()
		{
			MockMbas mbas = new MockMbas();
			mbas.TargetType = "Exe";
			Assert.AreEqual("-target:exe", mbas.GetCommandLine());			
		}

		[Test]
		public void FullDebugging()
		{
			MockMbas mbas = new MockMbas();
			mbas.DebugType = "Full";
			Assert.AreEqual("-debug:full", mbas.GetCommandLine());			
		}

		[Test]
		public void EmitDebuggingInfo()
		{
			MockMbas mbas = new MockMbas();
			mbas.EmitDebugInformation = true;
			Assert.AreEqual("-debug", mbas.GetCommandLine());
		}
		
		[Test]
		public void NoLogo()
		{
			MockMbas mbas = new MockMbas();
			mbas.NoLogo = true;
			Assert.AreEqual("-nologo", mbas.GetCommandLine());				
		}	
		
		[Test]
		public void DefineConstants()
		{
			MockMbas mbas = new MockMbas();
			mbas.DefineConstants = "DEBUG=1,TRACE=1";
			Assert.AreEqual("-define:\"DEBUG=1,TRACE=1\"", mbas.GetCommandLine());
		}	
		
		[Test]
		public void MainEntryPoint()
		{
			MockMbas mbas = new MockMbas();
			mbas.MainEntryPoint = "Console.MainClass.Main";
			Assert.AreEqual("-main:Console.MainClass.Main",  mbas.GetCommandLine());						
		}	

		[Test]
		public void SingleSourceFile()
		{
			MockMbas mbas = new MockMbas();
			mbas.Sources = new TaskItem[] { new TaskItem("proj src\\Main.vb") };
			Assert.AreEqual("\"proj src\\Main.vb\"", mbas.GetCommandLine());
		}

		[Test]
		public void SingleReference()
		{
			MockMbas mbas = new MockMbas();
			mbas.References = new TaskItem[] { new TaskItem("proj refs\\Test.dll") };
			Assert.AreEqual("-r:\"proj refs\\Test.dll\"", mbas.GetCommandLine());
		}

		[Test]
		public void AdditionalLibPaths()
		{
			MockMbas mbas = new MockMbas();
			mbas.AdditionalLibPaths = new string[] { "proj\\My libs", "proj\\My libs2" };
			Assert.AreEqual("-lib:\"proj\\My libs\",\"proj\\My libs2\"", mbas.GetCommandLine());
		}		
		
		[Test]
		public void EmbeddedResources()
		{
			MockMbas mbas = new MockMbas();
			mbas.Resources = new TaskItem[] { new TaskItem("proj res\\Test.xml"),
				new TaskItem("proj res\\Run.xml") };
			Assert.AreEqual("-resource:\"proj res\\Test.xml\" -resource:\"proj res\\Run.xml\"", mbas.GetCommandLine());
		}		

		[Test]
		public void OptionStrict()
		{
			MockMbas mbas = new MockMbas();
			mbas.OptionStrict = true;
			Assert.AreEqual("-optionstrict", mbas.GetCommandLine());				
		}
		
		[Test]
		public void OptionExplicit()
		{
			MockMbas mbas = new MockMbas();
			mbas.OptionExplicit = true;
			Assert.AreEqual("-optionexplicit", mbas.GetCommandLine());				
		}

		[Test]
		public void MultipleImports()
		{
			MockMbas mbas = new MockMbas();
			mbas.Imports = new TaskItem[] { new TaskItem("System.IO"),
				new TaskItem("Microsoft.VisualBasic") };
			Assert.AreEqual("-imports:System.IO -imports:Microsoft.VisualBasic", mbas.GetCommandLine());
		}	
		
		[Test]
		public void RemoveIntChecks()
		{
			MockMbas mbas = new MockMbas();
			mbas.RemoveIntegerChecks = true;
			Assert.AreEqual("-removeintchecks", mbas.GetCommandLine());				
		}	
		
		[Test]
		public void RootNamespace()
		{
			MockMbas mbas = new MockMbas();
			mbas.RootNamespace = "MyNamespace";
			Assert.AreEqual("-rootnamespace:MyNamespace", mbas.GetCommandLine());				
		}	
		
		[Test]
		public void WarningLevel()
		{
			MockMbas mbas = new MockMbas();
			mbas.WarningLevel = 3;
			Assert.AreEqual("-wlevel:3", mbas.GetCommandLine());						
		}
		
		[Test]
		public void NoWarnings()
		{
			MockMbas mbas = new MockMbas();
			mbas.NoWarnings = true;
			Assert.AreEqual("-nowarn", mbas.GetCommandLine());						
		}
		
		[Test]
		public void DisabledWarnings()
		{
			MockMbas mbas = new MockMbas();
			mbas.DisabledWarnings = "1234,5678";
			Assert.AreEqual("-ignorewarn:\"1234,5678\"", mbas.GetCommandLine());					
		}	

	}
}
