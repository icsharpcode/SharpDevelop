// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Util;
using NUnit.Framework;
using System.Text.RegularExpressions;

namespace ICSharpCode.SharpDevelop.Tests
{
	[TestFixture]
	public class CheckAssemblyFlags
	{
		string binPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..");
		
		bool Get32BitFlags(string assembly)
		{
			assembly = Path.Combine(binPath, assembly);
			string corflags = FileUtility.GetSdkPath("corflags.exe");
			Assert.IsNotNull(corflags, "corflags.exe not found, this test requires the .NET SDK!");
			ProcessRunner pr = new ProcessRunner();
			Console.WriteLine(corflags + " \"" + assembly + "\"");
			pr.Start(corflags, "\"" + assembly + "\"");
			if (!pr.WaitForExit(5000)) {
				pr.Kill();
				throw new InvalidOperationException("Timeout running corflags");
			} else {
				Console.WriteLine(pr.StandardOutput);
				Match m = Regex.Match(pr.StandardOutput, @"32BIT\s*:\s*([01])");
				if (m.Success) {
					return m.Groups[1].Value == "1";
				} else {
					throw new InvalidOperationException("Invalid corflags output");
				}
			}
		}
		
		// All these processes must use the same value for 32-bit-flag:
		[Test]
		public void CheckSharpDevelop32Bit()
		{
			Assert.IsTrue(Get32BitFlags("SharpDevelop.exe"));
		}
		
		[Test]
		public void CheckBooCompiler32Bit()
		{
			Assert.IsTrue(Get32BitFlags(@"..\src\AddIns\BackendBindings\Boo\RequiredLibraries\booc.exe"));
		}
	}
}
