// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Reflection.Emit;
using ICSharpCode.Python.Build.Tasks;
using NUnit.Framework;

namespace Python.Build.Tasks.Tests
{
	[TestFixture]
	public class PythonCompilerTests
	{
		[Test]
		public void NoMainFileSpecifiedForWindowsApplication()
		{
			try {
				PythonCompiler compiler = new PythonCompiler();
				compiler.TargetKind = PEFileKinds.WindowApplication;
				compiler.OutputAssembly = "test.exe";
				compiler.SourceFiles = new string[0];
				compiler.MainFile = null;
				compiler.Compile();
				
				Assert.Fail("Expected PythonCompilerException.");
			} catch (PythonCompilerException ex) {
				Assert.AreEqual(Resources.NoMainFileSpecified, ex.Message);
			}
		}
		
		[Test]
		public void NoMainSpecifiedForLibraryThrowsNoError()
		{
			PythonCompiler compiler = new PythonCompiler();
			compiler.TargetKind = PEFileKinds.Dll;
			compiler.OutputAssembly = "test.dll";
			compiler.SourceFiles = new string[0];
			compiler.MainFile = null;
			compiler.VerifyParameters();
		}
	}
}
