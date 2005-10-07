#region license
// Copyright (c) 2005, Daniel Grunwald (daniel@danielgrunwald.de)
// All rights reserved.
//
// NRefactoryToBoo is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// NRefactoryToBoo is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with NRefactoryToBoo; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
#endregion

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
			            "public final def Run() as System.Void:\n" +
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
			            "public final def Run() as System.Void:\n" +
			            "\tif a:\n" +
			            "\t\tb as System.Int32 = 1\n" +
			            "\telse:\n" +
			            "\t\tb__2 as System.Double = 2");
		}
	}
}
