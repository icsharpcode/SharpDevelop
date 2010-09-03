// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
