// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.IO;

using NUnit.Framework;

using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Parser.AST;

namespace ICSharpCode.NRefactory.Tests.AST
{
	[TestFixture]
	public class OptionDeclarationTests
	{
		
		#region C#
		// No C# representation
		#endregion
		
		#region VB.NET
		[Test]
		public void VBNetStrictOptionDeclarationTest()
		{
			string program = "Option Strict On\n";
			OptionDeclaration opDec = (OptionDeclaration)ParseUtilVBNet.ParseGlobal(program, typeof(OptionDeclaration));
			Assert.AreEqual(OptionType.Strict, opDec.OptionType);
			Assert.IsTrue(opDec.OptionValue);
		}
		
		[Test]
		public void VBNetExplicitOptionDeclarationTest()
		{
			string program = "Option Explicit Off\n";
			OptionDeclaration opDec = (OptionDeclaration)ParseUtilVBNet.ParseGlobal(program, typeof(OptionDeclaration));
			Assert.AreEqual(OptionType.Explicit, opDec.OptionType);
			Assert.IsFalse(opDec.OptionValue, "Off option value excepted!");
		}
		
		[Test]
		public void VBNetCompareBinaryOptionDeclarationTest()
		{
			string program = "Option Compare Binary\n";
			OptionDeclaration opDec = (OptionDeclaration)ParseUtilVBNet.ParseGlobal(program, typeof(OptionDeclaration));
			Assert.AreEqual(OptionType.CompareBinary, opDec.OptionType);
			Assert.IsTrue(opDec.OptionValue);
		}
		
		[Test]
		public void VBNetCompareTextOptionDeclarationTest()
		{
			string program = "Option Compare Text\n";
			OptionDeclaration opDec = (OptionDeclaration)ParseUtilVBNet.ParseGlobal(program, typeof(OptionDeclaration));
			Assert.AreEqual(OptionType.CompareText, opDec.OptionType);
			Assert.IsTrue(opDec.OptionValue);
		}
		#endregion
		
	}
}
