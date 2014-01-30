// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using ICSharpCode.NRefactory;
using NUnit.Framework;

namespace ICSharpCode.XamlBinding.Tests
{
	[TestFixture]
	public class UtilsTests
	{
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
libraries".Length;
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
simple".Length;
			
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
			
			TextLocation location = Utils.GetLocationInfoFromOffset(text, 0);
			
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
			
			TextLocation location = Utils.GetLocationInfoFromOffset(text, offset);
			
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
			
			TextLocation location = Utils.GetLocationInfoFromOffset(text, offset);
			
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
			
			TextLocation location = Utils.GetLocationInfoFromOffset(text, offset);
			
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
			
			TextLocation location = Utils.GetLocationInfoFromOffset(text, offset);
			
			Assert.AreEqual(1, location.Line);
			Assert.AreEqual(30, location.Column);
		}
		
		
		static object[] ParseNameCases = {
			new[] { "Name", "", "Name", "" },
			new[] { "x:Name", "x", "Name", "" },
			new[] { "x:Name.Member", "x", "Name", "Member" },
			new[] { "x:N.M", "x", "N", "M" },
			new[] { "N.M", "", "N", "M" }
		};
		
		[Test, TestCaseSource("ParseNameCases")]
		public void ParseNameTest(string input, string expectedPrefix, string expectedName, string expectedMember)
		{
			string outputPrefix, outputMember;
			string outputName = XamlResolver.ParseName(input, out outputPrefix, out outputMember);
			
			Assert.AreEqual(expectedPrefix, outputPrefix);
			Assert.AreEqual(expectedMember, outputMember);
			Assert.AreEqual(expectedName, outputName);
		}
	}
}
