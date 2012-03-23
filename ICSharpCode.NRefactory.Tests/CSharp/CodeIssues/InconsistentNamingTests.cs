// 
// InconsistentNamingTests.cs
//  
// Author:
//       Mike Krüger <mkrueger@xamarin.com>
// 
// Copyright (c) 2012 Xamarin <http://xamarin.com>
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
using System;
using NUnit.Framework;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.CSharp.CodeActions;

namespace ICSharpCode.NRefactory.CSharp.CodeIssues
{
	[TestFixture]
	public class InconsistentNamingTests : InspectionActionTestBase
	{
		void CheckNaming (string input, string output)
		{
			TestRefactoringContext context;
			var issues = GetIssues (new InconsistentNamingIssue (), input, out context);
			Assert.Greater (issues.Count, 0);
			CheckFix (context, issues [0], output);
		}

		[Test]
		public void TestNamespaceName ()
		{
			var input = @"namespace anIssue {}";
			var output = @"namespace AnIssue {}";
			CheckNaming (input, output);
		}
		
		[Test]
		public void TestClassName ()
		{
			var input = @"class anIssue {}";
			var output = @"class AnIssue {}";
			CheckNaming (input, output);
		}
		
		[Test]
		public void TestStructName ()
		{
			var input = @"struct anIssue {}";
			var output = @"struct AnIssue {}";
			CheckNaming (input, output);
		}

		[Test]
		public void TestInterfaceName ()
		{
			var input = @"interface anIssue {}";
			var output = @"interface IAnIssue {}";
			CheckNaming (input, output);
		}

		[Test]
		public void TestEnumName ()
		{
			var input = @"enum anIssue {}";
			var output = @"enum AnIssue {}";
			CheckNaming (input, output);
		}

		[Test]
		public void TestDelegateName ()
		{
			var input = @"delegate void anIssue ();";
			var output = @"delegate void AnIssue ();";
			CheckNaming (input, output);
		}

		[Test]
		public void TestPrivateFieldName ()
		{
			var input = @"class AClass { int Field; }";
			var output = @"class AClass { int field; }";
			CheckNaming (input, output);
		}
		
		[Test]
		public void TestPublicFieldName ()
		{
			var input = @"class AClass { public int field; }";
			var output = @"class AClass { public int Field; }";
			CheckNaming (input, output);
		}
		
		[Test]
		public void TestMethodName ()
		{
			var input = @"class AClass { int method () {} }";
			var output = @"class AClass { int Method () {} }";
			CheckNaming (input, output);
		}

		[Test]
		public void TestPropertyName ()
		{
			var input = @"class AClass { int property { get; set; } }";
			var output = @"class AClass { int Property { get; set; } }";
			CheckNaming (input, output);
		}

		[Test]
		public void TestParameterName ()
		{
			var input = @"class AClass { int Method (int Param) {} }";
			var output = @"class AClass { int Method (int param) {} }";
			CheckNaming (input, output);
		}
	}
}

