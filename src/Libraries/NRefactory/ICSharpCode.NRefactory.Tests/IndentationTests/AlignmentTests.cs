//
// AlignmentTests.cs
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
using NUnit.Framework;
using ICSharpCode.NRefactory.CSharp;

namespace ICSharpCode.NRefactory.IndentationTests
{
	[TestFixture]
	public class AlignmentTests
	{
		[Test]
		public void MethodCallAlignment()
		{
			CSharpFormattingOptions fmt = FormattingOptionsFactory.CreateMono();
			fmt.AlignToFirstMethodCallArgument = false;
			var indent = Helper.CreateEngine(@"
class Foo
{
	void Test ()
	{
		Call(A,$", fmt);
			Assert.AreEqual("\t\t\t", indent.NextLineIndent);
		}

		[Test]
		public void IndexerAlignment()
		{
			CSharpFormattingOptions fmt = FormattingOptionsFactory.CreateMono();
			fmt.AlignToFirstIndexerArgument = false;
			var indent = Helper.CreateEngine(@"
class Foo
{
void Test ()
{
Call[A,$", fmt);
			Assert.AreEqual("\t\t\t", indent.NextLineIndent);
		}

		[Test]
		public void BinaryExpressionAlignment()
		{
			CSharpFormattingOptions fmt = FormattingOptionsFactory.CreateMono();
			fmt.AlignToFirstIndexerArgument = false;
			var indent = Helper.CreateEngine(@"
class Foo
{
	void Test ()
	{
		public static bool IsComplexExpression(AstNode expr)
		{
			return expr.StartLocation.Line != expr.EndLocation.Line ||
				expr is ConditionalExpression ||$", fmt);
			Assert.AreEqual("\t\t\t\t", indent.ThisLineIndent);
			Assert.AreEqual("\t\t\t\t", indent.NextLineIndent);
		}

		[Test]
		public void MethodContinuation()
		{
			CSharpFormattingOptions fmt = FormattingOptionsFactory.CreateMono();
			fmt.AlignToFirstMethodCallArgument = false;
			var indent = Helper.CreateEngine(@"
class Foo
{
	void Test ()
	{
		var a = Call(A)
			.Foo ()$", fmt);
			Assert.AreEqual("\t\t\t", indent.ThisLineIndent);
		}

		[Test]
		public void MethodContinuationDeep()
		{
			CSharpFormattingOptions fmt = FormattingOptionsFactory.CreateMono();
			fmt.AlignToFirstMethodCallArgument = false;
			var indent = Helper.CreateEngine(@"
class Foo
{
	void Test ()
	{
		var a = Call(A)
			.Foo ()
			.Foo ()
			.Foo ()$", fmt);
			Assert.AreEqual("\t\t\t", indent.ThisLineIndent);
		}

		[Test]
		public void MethodContinuation_AlignToMemberReferenceDot()
		{
			CSharpFormattingOptions fmt = FormattingOptionsFactory.CreateMono();
			fmt.AlignToFirstMethodCallArgument = false;
			fmt.AlignToMemberReferenceDot = true;
			var indent = Helper.CreateEngine(@"
class Foo
{
	void Test ()
	{
		var a = Call(A).Foo ()
		               .Foo ()
		               .Foo ()$", fmt);
			Assert.AreEqual("\t\t               ", indent.ThisLineIndent);
		}

		[Test]
		public void AlignEmbeddedIfStatements()
		{
			CSharpFormattingOptions fmt = FormattingOptionsFactory.CreateMono();
			fmt.AlignEmbeddedStatements = true;
			var indent = Helper.CreateEngine(@"
class Foo
{
	void Test ()
	{
		if (true)
		if (true)
		if (true) $", fmt);
			Assert.AreEqual("\t\t", indent.ThisLineIndent);
			Assert.AreEqual("\t\t\t", indent.NextLineIndent);
		}

		[Test]
		public void UnalignEmbeddedIfStatements()
		{
			CSharpFormattingOptions fmt = FormattingOptionsFactory.CreateMono();
			fmt.AlignEmbeddedStatements = false;
			var indent = Helper.CreateEngine(@"
class Foo
{
	void Test ()
	{
		if (true)
			if (true)
				if (true) $", fmt);
			Assert.AreEqual("\t\t\t\t", indent.ThisLineIndent);
			Assert.AreEqual("\t\t\t\t\t", indent.NextLineIndent);
		}

		[Test]
		public void AlignEmbeddedUsingStatements()
		{
			CSharpFormattingOptions fmt = FormattingOptionsFactory.CreateMono();
			fmt.AlignEmbeddedStatements = true;
			var indent = Helper.CreateEngine(@"
class Foo
{
	void Test (IDisposable a, IDisposable b)
	{
		using (a)
		using (a)
		using (b) $", fmt);
			Assert.AreEqual("\t\t", indent.ThisLineIndent);
			Assert.AreEqual("\t\t\t", indent.NextLineIndent);
		}

		[Test]
		public void AlignEmbeddedLockStatements()
		{
			CSharpFormattingOptions fmt = FormattingOptionsFactory.CreateMono();
			fmt.AlignEmbeddedStatements = true;
			var indent = Helper.CreateEngine(@"
class Foo
{
	void Test (IDisposable a, IDisposable b)
	{
		lock (a)
		lock (a)
		lock (b) $", fmt);
			Assert.AreEqual("\t\t", indent.ThisLineIndent);
			Assert.AreEqual("\t\t\t", indent.NextLineIndent);
		}
		[Test]
		public void UnalignEmbeddedUsingStatements()
		{
			CSharpFormattingOptions fmt = FormattingOptionsFactory.CreateMono();
			fmt.AlignEmbeddedStatements = false;
			var indent = Helper.CreateEngine(@"
class Foo
{
	void Test (IDisposable a, IDisposable b)
	{
		using (a)
			using (a)
				using (b) $", fmt);
			Assert.AreEqual("\t\t\t\t", indent.ThisLineIndent);
			Assert.AreEqual("\t\t\t\t\t", indent.NextLineIndent);
		}

		[Test]
		public void AlignNamedAttributeArgument()
		{
			CSharpFormattingOptions fmt = FormattingOptionsFactory.CreateMono();
			fmt.AlignToFirstMethodCallArgument = true;
			var indent = Helper.CreateEngine(@"
[Attr (1,
       Foo = 2,$
       Bar = 3", fmt);
			Assert.AreEqual("       ", indent.ThisLineIndent, "this line indent doesn't match");
			Assert.AreEqual("       ", indent.NextLineIndent, "next line indent doesn't match");
		}

		[Test]
		public void UnalignNamedAttributeArguments()
		{
			CSharpFormattingOptions fmt = FormattingOptionsFactory.CreateMono();
			fmt.AlignToFirstMethodCallArgument = false;
			var indent = Helper.CreateEngine(@"
[Attr (1,
	Foo = 2,$
	Bar = 3", fmt);
			Assert.AreEqual("\t", indent.ThisLineIndent, "this line indent doesn't match");
			Assert.AreEqual("\t", indent.NextLineIndent, "next line indent doesn't match");
		}

		[Test]
		public void TestFormatFirstLineKeepFalse()
		{
			var fmt = FormattingOptionsFactory.CreateMono();
			fmt.KeepCommentsAtFirstColumn = false;
			var indent = Helper.CreateEngine(@"
class Foo 
{
 // Hello World$", fmt);
			Assert.AreEqual("\t", indent.ThisLineIndent);
			Assert.AreEqual("\t", indent.NextLineIndent);
		}

		[Ignore ("Should not be respected")]
		[Test]
		public void TestFormatFirstLineKeepTrue()
		{
			var fmt = FormattingOptionsFactory.CreateMono();
			fmt.KeepCommentsAtFirstColumn = true;
			var indent = Helper.CreateEngine(@"
class Foo 
{
// Hello World$", fmt);
			Assert.AreEqual("", indent.ThisLineIndent);
			Assert.AreEqual("\t", indent.NextLineIndent);
		}

		[Test]
		public void TestLongBinaryExpressionAlignmentBug()
		{
			var fmt = FormattingOptionsFactory.CreateMono();
			fmt.KeepCommentsAtFirstColumn = true;
			var indent = Helper.CreateEngine(@"
class Foo 
{
	bool ISupportsInterning.EqualsForInterning(ISupportsInterning other)
	{
		return p != null && type == p.type && name == p.name &&
			defaultValue == p.defaultValue && region == p.region && (flags & ~1) == (p.flags & ~1) && ListEquals(attributes, p.attributes);$", fmt);
			Assert.AreEqual("\t\t\t", indent.ThisLineIndent);
			Assert.AreEqual("\t\t", indent.NextLineIndent);
		}

		[Test]
		public void TestIsRightHandExpression_MethodNamedArgs()
		{
			var fmt = FormattingOptionsFactory.CreateMono();
			fmt.AlignToFirstMethodCallArgument = false;
			var indent = Helper.CreateEngine(@"
class Foo 
{
	void Test()
	{ 
		Bar (Named = $", fmt);
			Assert.AreEqual("\t\t", indent.ThisLineIndent);
			Assert.AreEqual("\t\t\t", indent.NextLineIndent);
		}

		[Test]
		public void TestIsRightHandExpression_MethodNamedArgs2()
		{
			var fmt = FormattingOptionsFactory.CreateMono();
			fmt.AlignToFirstMethodCallArgument = true;
			var indent = Helper.CreateEngine(@"
class Foo 
{
	void Test()
	{ 
		Bar (Named = $", fmt);
			Assert.AreEqual("\t\t", indent.ThisLineIndent);
			Assert.AreEqual("\t\t     ", indent.NextLineIndent);
		}

		[Test]
		public void TestIsRightHandExpression_MethodNamedArgs3()
		{
			var fmt = FormattingOptionsFactory.CreateMono();
			fmt.AlignToFirstMethodCallArgument = true;
			var indent = Helper.CreateEngine(@"
class Foo 
{
	void Test()
	{ 
		Bar (Named = 1 +
		     2, $", fmt);
			Assert.AreEqual("\t\t     ", indent.ThisLineIndent);
			Assert.AreEqual("\t\t     ", indent.NextLineIndent);
		}

		[Test]
		public void TestIsRightHandExpression_RelationalOperator()
		{
			var indent = Helper.CreateEngine(@"
class Foo 
{
	void Test()
	{ 
		return x == 1 $");
			Assert.AreEqual("\t\t", indent.ThisLineIndent);
			Assert.AreEqual("\t\t\t", indent.NextLineIndent);
		}


		[Test]
		public void TestIsRightHandExpression_RelationalOperator2()
		{
			var indent = Helper.CreateEngine(@"
class Foo 
{
	void Test()
	{ 
		return x >= 1 $");
			Assert.AreEqual("\t\t", indent.ThisLineIndent);
			Assert.AreEqual("\t\t\t", indent.NextLineIndent);
		}


		[Test]
		public void TestIsRightHandExpression_RelationalOperator3()
		{
			var indent = Helper.CreateEngine(@"
class Foo 
{
	void Test()
	{ 
		return x <= 1 $");
			Assert.AreEqual("\t\t", indent.ThisLineIndent);
			Assert.AreEqual("\t\t\t", indent.NextLineIndent);
		}


		[Test]
		public void TestIsRightHandExpression_RelationalOperator4()
		{
			var indent = Helper.CreateEngine(@"
class Foo 
{
	void Test()
	{ 
		return x != 1 $");
			Assert.AreEqual("\t\t", indent.ThisLineIndent);
			Assert.AreEqual("\t\t\t", indent.NextLineIndent);
		}


		[Test]
		public void TestIsRightHandExpression_ShortHandOperator()
		{
			var indent = Helper.CreateEngine(@"
class Foo 
{
	void Test()
	{ 
		x += 1 $");
			Assert.AreEqual("\t\t", indent.ThisLineIndent);
			Assert.AreEqual("\t\t\t", indent.NextLineIndent);
		}

		[Test]
		public void TestIsRightHandExpression_ShortHandOperator2()
		{
			var indent = Helper.CreateEngine(@"
class Foo 
{
	void Test()
	{ 
		x -= 1 $");
			Assert.AreEqual("\t\t", indent.ThisLineIndent);
			Assert.AreEqual("\t\t\t", indent.NextLineIndent);
		}

		[Test]
		public void TestIsRightHandExpression_ShortHandOperator3()
		{
			var indent = Helper.CreateEngine(@"
class Foo 
{
	void Test()
	{ 
		x *= 1 $");
			Assert.AreEqual("\t\t", indent.ThisLineIndent);
			Assert.AreEqual("\t\t\t", indent.NextLineIndent);
		}

		[Test]
		public void TestIsRightHandExpression_ShortHandOperator4()
		{
			var indent = Helper.CreateEngine(@"
class Foo 
{
	void Test()
	{ 
		x /= 1 $");
			Assert.AreEqual("\t\t", indent.ThisLineIndent);
			Assert.AreEqual("\t\t\t", indent.NextLineIndent);
		}

		[Test]
		public void TestIsRightHandExpression_ShortHandOperator5()
		{
			var indent = Helper.CreateEngine(@"
class Foo 
{
	void Test()
	{ 
		x %= 1 $");
			Assert.AreEqual("\t\t", indent.ThisLineIndent);
			Assert.AreEqual("\t\t\t", indent.NextLineIndent);
		}

		[Test]
		public void TestIsRightHandExpression_ShortHandOperator6()
		{
			var indent = Helper.CreateEngine(@"
class Foo 
{
	void Test()
	{ 
		x &= 1 $");
			Assert.AreEqual("\t\t", indent.ThisLineIndent);
			Assert.AreEqual("\t\t\t", indent.NextLineIndent);
		}

		[Test]
		public void TestIsRightHandExpression_ShortHandOperator7()
		{
			var indent = Helper.CreateEngine(@"
class Foo 
{
	void Test()
	{ 
		x |= 1 $");
			Assert.AreEqual("\t\t", indent.ThisLineIndent);
			Assert.AreEqual("\t\t\t", indent.NextLineIndent);
		}

		[Test]
		public void TestIsRightHandExpression_ShortHandOperator8()
		{
			var indent = Helper.CreateEngine(@"
class Foo 
{
	void Test()
	{ 
		x ^= 1 $");
			Assert.AreEqual("\t\t", indent.ThisLineIndent);
			Assert.AreEqual("\t\t\t", indent.NextLineIndent);
		}

		[Test]
		[Ignore("fixme")]
		public void TestIsRightHandExpression_ShortHandOperator9()
		{
			var indent = Helper.CreateEngine(@"
class Foo 
{
	void Test()
	{ 
		x >>= 1 $");
			Assert.AreEqual("\t\t", indent.ThisLineIndent);
			Assert.AreEqual("\t\t     ", indent.NextLineIndent);
		}

		[Test]
		[Ignore("fixme")]
		public void TestIsRightHandExpression_ShortHandOperator10()
		{
			var indent = Helper.CreateEngine(@"
class Foo 
{
	void Test()
	{ 
		x <<= 1 $");
			Assert.AreEqual("\t\t", indent.ThisLineIndent);
			Assert.AreEqual("\t\t     ", indent.NextLineIndent);
		}


		[Test]
		public void TestIsRightHandExpression_Statement()
		{
			var indent = Helper.CreateEngine(@"
class Foo 
{
	void Test()
	{ 
		if (1 == 1)
			x = 
				$");
			Assert.AreEqual("\t\t\t\t", indent.ThisLineIndent);
			Assert.AreEqual("\t\t\t\t", indent.NextLineIndent);
		}

		[Test]
		public void TestIsRightHandExpression_Statement2()
		{
			var indent = Helper.CreateEngine(@"
class Foo 
{
	void Test()
	{ 
		if (1 == 1)
			x = 
				1; $");
			Assert.AreEqual("\t\t\t\t", indent.ThisLineIndent);
			Assert.AreEqual("\t\t", indent.NextLineIndent);
		}

		[Test]
		public void TestIsRightHandExpression_Statement3()
		{
			var indent = Helper.CreateEngine(@"
class Foo 
{
	void Test()
	{ 
		if (1 == 1)
			x = 1 +
				2; $");
			Assert.AreEqual("\t\t\t\t", indent.ThisLineIndent);
			Assert.AreEqual("\t\t", indent.NextLineIndent);
		}

		[Test]
		public void TestIsRightHandExpression_MultipleAssignments()
		{
			var indent = Helper.CreateEngine(@"
class Foo 
{
	void Test()
	{ 
		x = y = z = 
			$");
			Assert.AreEqual("\t\t\t", indent.ThisLineIndent);
			Assert.AreEqual("\t\t\t", indent.NextLineIndent);
		}

		[Test]
		public void TestIsRightHandExpression_MultipleAssignments2()
		{
			var indent = Helper.CreateEngine(@"
class Foo 
{
	void Test()
	{ 
		x = y = z = 1 +
			2; $");
			Assert.AreEqual("\t\t\t", indent.ThisLineIndent);
			Assert.AreEqual("\t\t", indent.NextLineIndent);
		}

		[Test]
		public void BasicMethodContinuation()
		{
			CSharpFormattingOptions fmt = FormattingOptionsFactory.CreateMono();
			fmt.AlignToFirstMethodCallArgument = false;
			var indent = Helper.CreateEngine(@"
class Foo
{
	void Test ()
	{
		Call(A)
			.Foo ()$", fmt);
			Assert.AreEqual("\t\t\t", indent.ThisLineIndent);
		}

		[Test]
		public void DeepMethodContinuation()
		{
			CSharpFormattingOptions fmt = FormattingOptionsFactory.CreateMono();
			fmt.AlignToFirstMethodCallArgument = false;
			var indent = Helper.CreateEngine(@"
class Foo
{
	void Test ()
	{
		Call(A)
			.Foo ()
			.Foo ()
			.Foo ()$", fmt);
			Assert.AreEqual("\t\t\t", indent.ThisLineIndent);
		}

		[Test]
		public void DeepMethodContinuationStatement()
		{
			CSharpFormattingOptions fmt = FormattingOptionsFactory.CreateMono();
			fmt.AlignToFirstMethodCallArgument = false;
			var indent = Helper.CreateEngine(@"
class Foo
{
	void Test ()
	{
		if (true)
			Call(A)
				.Foo ()
				.Foo ()
				.Foo (); $", fmt);
			Assert.AreEqual("\t\t\t\t", indent.ThisLineIndent);
			Assert.AreEqual("\t\t", indent.NextLineIndent);
		}

		[Test]
		public void DeepMethodContinuationStatement_AlignToMemberReferenceDot()
		{
			CSharpFormattingOptions fmt = FormattingOptionsFactory.CreateMono();
			fmt.AlignToMemberReferenceDot = true;
			var indent = Helper.CreateEngine(@"
class Foo
{
	void Test ()
	{
		if (true)
			Call(A).Foo ()
			       .Foo ()
			       .Foo (); $", fmt);
			Assert.AreEqual("\t\t\t       ", indent.ThisLineIndent);
			Assert.AreEqual("\t\t", indent.NextLineIndent);
		}

		[Test]
		public void TestMethodContinuation()
		{
			CSharpFormattingOptions fmt = FormattingOptionsFactory.CreateMono();
			var indent = Helper.CreateEngine(@"
class Foo
{
	void Test ()
	{
		obj
			.Foo (); $", fmt);
			Assert.AreEqual("\t\t\t", indent.ThisLineIndent);
			Assert.AreEqual("\t\t", indent.NextLineIndent);
		}

		[Test]
		public void TestMethodContinuationCase2()
		{
			CSharpFormattingOptions fmt = FormattingOptionsFactory.CreateMono();
			var indent = Helper.CreateEngine(@"
class Foo
{
	void Test ()
	{
		var foo = obj
			.Foo (); $", fmt);
			Assert.AreEqual("\t\t\t", indent.ThisLineIndent);
			Assert.AreEqual("\t\t", indent.NextLineIndent);
		}

		[Test]
		public void TestMethodContinuationCase3a()
		{
			CSharpFormattingOptions fmt = FormattingOptionsFactory.CreateMono();
			var indent = Helper.CreateEngine(@"
class Foo
{
	int Test ()
	{
		return obj
			.Foo ()$", fmt);
			Assert.AreEqual("\t\t\t", indent.ThisLineIndent);
		}

		[Test]
		public void TestMethodContinuationCase3b()
		{
			CSharpFormattingOptions fmt = FormattingOptionsFactory.CreateMono();
			var indent = Helper.CreateEngine(@"
class Foo
{
	int Test ()
	{
		return 
			obj
				.Foo ()$", fmt);
			Assert.AreEqual("\t\t\t\t", indent.ThisLineIndent);
		}
	}
}

