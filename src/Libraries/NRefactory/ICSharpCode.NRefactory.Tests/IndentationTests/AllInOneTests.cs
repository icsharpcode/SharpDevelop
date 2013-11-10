//
// AllInOneTests.cs
//
// Author:
//       Mike Krüger <mkrueger@xamarin.com>
//
// Copyright (c) 2013 Xamarin Inc. (http://xamarin.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using ICSharpCode.NRefactory.CSharp;
using NUnit.Framework;
using System;

namespace ICSharpCode.NRefactory.IndentationTests
{
	[TestFixture]
	public class AllInOneTests
	{
		const string ProjectDir = "../../";
		const string TestFilesPath = "ICSharpCode.NRefactory.Tests/IndentationTests/TestFiles";

		public void BeginFileTest(string fileName, CSharpFormattingOptions policy = null, TextEditorOptions options = null)
		{
			Helper.ReadAndTest(System.IO.Path.Combine(ProjectDir, TestFilesPath, fileName), policy, options);
		}

		[Test]
		public void TestAllInOne_Simple()
		{
			BeginFileTest("Simple.cs");   
		}

		[Test]
		public void TestAllInOne_PreProcessorDirectives()
		{
			BeginFileTest("PreProcessorDirectives.cs");
		}

		[Test]
		public void TestAllInOne_Comments()
		{
			BeginFileTest("Comments.cs");
		}

		[Test]
		public void TestAllInOne_Strings()
		{
			BeginFileTest("Strings.cs");
		}

		[Test]
		public void TestAllInOne_IndentEngine()
		{
			BeginFileTest("IndentEngine.cs");
		}

		[Test]
		public void TestAllInOne_CSharpParser()
		{
			BeginFileTest("CSharpParser.cs", FormattingOptionsFactory.CreateSharpDevelop());
		}

		[Test]
		public void TestAllInOne_TextArea()
		{
			BeginFileTest("TextArea.cs");
		}

//		[Test]
//		public void TestAll()
//		{
//			foreach (var file in System.IO.Directory.EnumerateFiles("/home/mkrueger/work/monodevelop", "*.cs", System.IO.SearchOption.AllDirectories)) {
//				Helper.ReadAndTest(file, null, null);
//			}
//		}

		[Test]
		public void TestAllInOne_IndentState()
		{
			var policy = FormattingOptionsFactory.CreateMono();
			policy.IndentSwitchBody = true;
			policy.IndentCaseBody = true;
			policy.IndentBreakStatements = true;
			policy.AlignToFirstIndexerArgument = policy.AlignToFirstMethodCallArgument = true;
			BeginFileTest("IndentState.cs", policy);
		}

		[Test]
		public void TestAllInOne_SwitchCase()
		{
			var policy = FormattingOptionsFactory.CreateMono();
			policy.IndentSwitchBody = true;
			policy.IndentCaseBody = true;
			policy.IndentBreakStatements = false;

			BeginFileTest("SwitchCase.cs", policy);
		}

		[Test]
		public void TestAllInOne_InheritStatements()
		{
			BeginFileTest("InheritStatements.cs");
		}
	}
}
