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
using ICSharpCode.PythonBinding;
using NUnit.Framework;

namespace PythonBinding.Tests.Expressions
{
	[TestFixture]
	public class IsImportExpressionTestFixture
	{
		[Test]
		public void EmptyStringIsNotImportExpression()
		{
			string text = String.Empty;
			int offset = 0;
			Assert.IsFalse(PythonImportExpression.IsImportExpression(text, offset));
		}
		
		[Test]
		public void ImportStringFollowedByWhitespaceIsImportExpression()
		{
			string text = "import    ";
			int offset = text.Length - 1;
			Assert.IsTrue(PythonImportExpression.IsImportExpression(text, offset));			
		}
		
		[Test]
		public void ImportStringSurroundedByExtraWhitespaceIsImportExpression()
		{
			string text = "    import    ";
			int offset = text.Length - 1;
			Assert.IsTrue(PythonImportExpression.IsImportExpression(text, offset));
		}
		
		[Test]
		public void ImportInsideAnotherStringIsNotImportExpression()
		{
			string text = "Start-import-End";
			int offset = text.Length - 1;
			Assert.IsFalse(PythonImportExpression.IsImportExpression(text, offset));
		}
		
		[Test]
		public void ImportSystemIsImportExpression()
		{
			string text = "import System";
			int offset = text.Length - 1;
			Assert.IsTrue(PythonImportExpression.IsImportExpression(text, offset));			
		}
		
		[Test]
		public void IsImportExpressionReturnsFalseWhenOffsetIsMinusOne()
		{
			string text = "import a";
			int offset = -1;
			Assert.IsFalse(PythonImportExpression.IsImportExpression(text, offset));
		}
		
		[Test]
		public void IsImportExpressionReturnsTrueWhenExpressionContainsOnlyFromPart()
		{
			string text = "from a";
			int offset = text.Length - 1;
			Assert.IsTrue(PythonImportExpression.IsImportExpression(text, offset));
		}
		
		[Test]
		public void FromExpressionFollowedByWhitespaceIsImportExpression()
		{
			string text = "from    ";
			int offset = text.Length - 1;
			Assert.IsTrue(PythonImportExpression.IsImportExpression(text, offset));			
		}
	}
}
