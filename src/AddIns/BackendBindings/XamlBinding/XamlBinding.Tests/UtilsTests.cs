// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.NRefactory;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using NUnit.Framework;

namespace ICSharpCode.XamlBinding.Tests
{
	[TestFixture]
	public class UtilsTests
	{
		[Test]
		public void DiffTestSimple()
		{
			string xaml = "<Test val1=\"Test\" />";
			int offset = "<Test val1=\"Te".Length;
			int expectedResult = offset - "<Test val1=\"".Length;
			
			int actualResult = Utils.GetOffsetFromValueStart(xaml, offset);
			
			Assert.AreEqual(expectedResult, actualResult);
		}
		
		[Test]
		public void DiffTestSimple2()
		{
			string xaml = "<Test val1=\"Test\" />";
			int offset = "<Test val1=\"".Length;
			int expectedResult = offset - "<Test val1=\"".Length;
			
			int actualResult = Utils.GetOffsetFromValueStart(xaml, offset);
			
			Assert.AreEqual(expectedResult, actualResult);
		}
		
		[Test]
		public void GetOffsetTest1()
		{
			string text = @"SharpDevelop uses the MSBuild
libraries for compilation. But when you compile a project
inside SharpDevelop, there's more going on than a
simple call to MSBuild.";
			
			int expected = 0;
			int line = 1;
			int col = 1;
			
			int result = Utils.GetOffsetFromFilePos(text, line, col);
			
			Assert.AreEqual(expected, result);
		}
		
		[Test]
		public void GetOffsetTest2()
		{
			string text = @"SharpDevelop uses the MSBuild
libraries for compilation. But when you compile a project
inside SharpDevelop, there's more going on than a
simple call to MSBuild.";
			
			int expected = 4;
			int line = 1;
			int col = 5;
			
			int result = Utils.GetOffsetFromFilePos(text, line, col);
			
			Assert.AreEqual(expected, result);
		}
		
		[Test]
		public void GetOffsetTest3()
		{
			string text = @"SharpDevelop uses the MSBuild
libraries for compilation. But when you compile a project
inside SharpDevelop, there's more going on than a
simple call to MSBuild.";
			
			int expected = 0;
			int line = 0;
			int col = 5;
			
			int result = Utils.GetOffsetFromFilePos(text, line, col);
			
			Assert.AreEqual(expected, result);
		}
		
		[Test]
		public void GetOffsetTest4()
		{
			string text = @"SharpDevelop uses the MSBuild
libraries for compilation. But when you compile a project
inside SharpDevelop, there's more going on than a
simple call to MSBuild.";
			
			int expected = @"SharpDevelop uses the MSBuild
libraries".Length - 1;
			int line = 2;
			int col = 10;
			
			int result = Utils.GetOffsetFromFilePos(text, line, col);
			
			Assert.AreEqual(expected, result);
		}

		[Test]
		public void GetOffsetTest5()
		{
			string text = @"SharpDevelop uses the MSBuild
libraries for compilation. But when you compile a project
inside SharpDevelop, there's more going on than a
simple call to MSBuild.";
			
			int expected = text.Length;
			int line = 10;
			int col = 10;
			
			int result = Utils.GetOffsetFromFilePos(text, line, col);
			
			Assert.AreEqual(expected, result);
		}
		
		[Test]
		public void GetOffsetTest6()
		{
			string text = @"SharpDevelop uses the MSBuild
libraries for compilation. But when you compile a project
inside SharpDevelop, there's more going on than a
simple call to MSBuild.";
			
			int expected = @"SharpDevelop uses the MSBuild
libraries for compilation. But when you compile a project
inside SharpDevelop, there's more going on than a
simple".Length - 1;
			
			int line = 4;
			int col = 7;
			
			int result = Utils.GetOffsetFromFilePos(text, line, col);
			
			Assert.AreEqual(expected, result);
		}
		
		[Test]
		public void GetLocationTest1()
		{
			string text = @"SharpDevelop uses the MSBuild
libraries for compilation. But when you compile a project
inside SharpDevelop, there's more going on than a
simple call to MSBuild.";
			
			Location location = Utils.GetLocationInfoFromOffset(text, 0);
			
			Assert.AreEqual(1, location.Line);
			Assert.AreEqual(1, location.Column);
		}
		
		[Test]
		public void GetLocationTest2()
		{
			string text = @"SharpDevelop uses the MSBuild
libraries for compilation. But when you compile a project
inside SharpDevelop, there's more going on than a
simple call to MSBuild.";
			
			int offset = "SharpDevelop".Length;
			
			Location location = Utils.GetLocationInfoFromOffset(text, offset);
			
			Assert.AreEqual(1, location.Line);
			Assert.AreEqual(13, location.Column);
		}
		
		[Test]
		public void GetLocationTest3()
		{
			string text = @"SharpDevelop uses the MSBuild
libraries for compilation. But when you compile a project
inside SharpDevelop, there's more going on than a
simple call to MSBuild.";
			
			int offset = @"SharpDevelop uses the MSBuild
".Length;
			
			Location location = Utils.GetLocationInfoFromOffset(text, offset);
			
			Assert.AreEqual(2, location.Line);
			Assert.AreEqual(1, location.Column);
		}
		
		[Test]
		public void GetLocationTest4()
		{
			string text = @"SharpDevelop uses the MSBuild
libraries for compilation. But when you compile a project
inside SharpDevelop, there's more going on than a
simple call to MSBuild.";
			
			int offset = @"SharpDevelop uses the MSBuild
libraries".Length;
			
			Location location = Utils.GetLocationInfoFromOffset(text, offset);
			
			Assert.AreEqual(2, location.Line);
			Assert.AreEqual(10, location.Column);
		}
		
		[Test]
		public void GetLocationTest5()
		{
			string text = @"SharpDevelop uses the MSBuild
libraries for compilation. But when you compile a project
inside SharpDevelop, there's more going on than a
simple call to MSBuild.";
			
			int offset = @"SharpDevelop uses the MSBuild".Length;
			
			Location location = Utils.GetLocationInfoFromOffset(text, offset);
			
			Assert.AreEqual(1, location.Line);
			Assert.AreEqual(30, location.Column);
		}
	}
}
