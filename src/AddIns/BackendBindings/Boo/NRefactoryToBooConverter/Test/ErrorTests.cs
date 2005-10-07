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
using System.IO;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.Ast;
using NUnit.Framework;

namespace NRefactoryToBooConverter.Tests
{
	/// <summary>
	/// Tests for special cases that have to fail.
	/// </summary>
	//[TestFixture]
	public class ErrorTests : TestHelper
	{
		/*
		 For the following errors are currently no checks implemented:
		 
		 enum Enumeration : WithBaseType
		 enum Enumeration<T>
		 static class Bla : WithBaseType
		 static class Bla<T>
		 enum Enumeration { void Main(); } // check that enums can only have fields is not implemented
		 */
	}
}
