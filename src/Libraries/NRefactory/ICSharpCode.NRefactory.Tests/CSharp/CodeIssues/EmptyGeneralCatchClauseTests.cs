// 
// EmptyGeneralCatchClauseTests.cs
//  
// Author:
//       Ji Kun <jikun.nus@gmail.com>
// 
// Copyright (c) 2013 Ji Kun <jikun.nus@gmail.com>
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
	public class EmptyGeneralCatchClauseTests : InspectionActionTestBase
	{
		
		[Test]
		public void TestInspectorCase1()
		{
			var input = @"using System;
	using System.IO;
	namespace Application
	{
		public class BaseClass
		{
			public void method()
			{
				try
				{
					F ();
				}
				catch (Exception e)
				{
				}
			}
		}
	}
";
			TestRefactoringContext context;
			var issues = GetIssues(new EmptyGeneralCatchClauseIssue(), input, out context);
			Assert.AreEqual(1, issues.Count);
		}


		[Test]
		public void TestInspectorCase2()
		{
			var input = @"using System;
	using System.IO;
	namespace Application
	{
		public class BaseClass
		{
			public void method()
			{
				try
				{
					F ();
				}
				catch
				{
				}
			}
		}
	}
";
			TestRefactoringContext context;
			var issues = GetIssues(new EmptyGeneralCatchClauseIssue(), input, out context);
			Assert.AreEqual(1, issues.Count);
		}

		[Test]
		public void TestResharperDisableRestore()
		{
			var input = @"using System;
	using System.IO;
	namespace Application
	{
		public class BaseClass
		{
			public void method()
			{
				try
				{
					F ();
				}
//Resharper disable EmptyGeneralCatchClause
				catch (Exception e)
				{
				}
//Resharper restore EmptyGeneralCatchClause
			}
		}
	}
";
			
			TestRefactoringContext context;
			var issues = GetIssues(new EmptyGeneralCatchClauseIssue(), input, out context);
			Assert.AreEqual(0, issues.Count);
		}
	}
}