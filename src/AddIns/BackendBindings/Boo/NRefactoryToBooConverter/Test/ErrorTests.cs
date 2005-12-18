// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

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
