// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NUnit.Framework;

namespace NRefactoryToBooConverter.Tests
{
	[TestFixture]
	public class ComplexTests : TestHelper
	{
		[Test]
		public void MovingLocals()
		{
			TestInClass("public void Run() { if (a) { int b = 1; } else { int b = 2; } }",
			            "public def Run() as System.Void:\n" +
			            "\tb as System.Int32\n" +
			            "\tif a:\n" +
			            "\t\tb = 1\n" +
			            "\telse:\n" +
			            "\t\tb = 2");
		}
		
		[Test]
		public void RenamingLocals()
		{
			TestInClass("public void Run() { if (a) { int b = 1; } else { double b = 2; } }",
			            "public def Run() as System.Void:\n" +
			            "\tif a:\n" +
			            "\t\tb as System.Int32 = 1\n" +
			            "\telse:\n" +
			            "\t\tb__2 as System.Double = 2");
		}
	}
}
