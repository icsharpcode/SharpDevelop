using System;

using System;
using System.IO;
using NUnit.Framework;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.Editor;
using System.Text;

namespace ICSharpCode.NRefactory.CSharp.Indentation
{
	[TestFixture]
	public class IndentationTests
	{
		static CSharpIndentEngine CreateEngine (string text)
		{
			var policy = FormattingOptionsFactory.CreateMono ();

			var sb = new StringBuilder();
			int offset = 0;
			for (int i = 0; i < text.Length; i++) {
				var ch = text [i];
				if (ch == '$') {
					offset = i;
					continue;
				}
				sb.Append (ch);
			}
			var document = new ReadOnlyDocument(sb.ToString ());

			var options = new TextEditorOptions();

			var result = new CSharpIndentEngine(document, options, policy);
			result.UpdateToOffset(offset);
			return result;
		}

		[Test]
		public void TestNamespaceIndent ()
		{
			var indent = CreateEngine("namespace Foo {$");
			Assert.AreEqual("", indent.ThisLineIndent);
			Assert.AreEqual("\t", indent.NewLineIndent);
		}

		[Test]
		public void TestPreProcessorDirectives ()
		{
			var indent = CreateEngine(@"
namespace Foo {
	class Foo {
#if NOTTHERE
	{
#endif
		$");
			Assert.AreEqual("\t\t", indent.ThisLineIndent);
			Assert.AreEqual("\t\t", indent.NewLineIndent);
		}

		[Test]
		public void TestPreProcessorDirectives2 ()
		{
			var indent = CreateEngine(@"
namespace Foo {
	class Foo {
#if NOTTHERE || true
	{
#endif
		$");
			Assert.AreEqual("\t\t\t", indent.ThisLineIndent);
			Assert.AreEqual("\t\t\t", indent.NewLineIndent);
		}

		[Test]
		public void TestIf ()
		{
			var indent = CreateEngine(@"
class Foo {
	void Test ()
	{
		if (true)$");
			Assert.AreEqual("\t\t", indent.ThisLineIndent);
			Assert.AreEqual("\t\t\t", indent.NewLineIndent);
		}

		[Test]
		public void TestFor ()
		{
			var indent = CreateEngine(@"
class Foo {
	void Test ()
	{
		for (;;)$");
			Assert.AreEqual("\t\t", indent.ThisLineIndent);
			Assert.AreEqual("\t\t\t", indent.NewLineIndent);
		}

		[Test]
		public void TestForEach ()
		{
			var indent = CreateEngine(@"
class Foo {
	void Test ()
	{
		foreach (;;)$");
			Assert.AreEqual("\t\t", indent.ThisLineIndent);
			Assert.AreEqual("\t\t\t", indent.NewLineIndent);
		}


		[Test]
		public void TestDo ()
		{
			var indent = CreateEngine(@"
class Foo {
	void Test ()
	{
		do
$");
			Assert.AreEqual("\t\t\t", indent.ThisLineIndent);
		}

		[Test]
		public void TestNestedDo ()
		{
			var indent = CreateEngine(@"
class Foo {
	void Test ()
	{
		do do
$");
			Assert.AreEqual("\t\t\t\t", indent.ThisLineIndent);
		}

		[Test]
		public void TestNestedDoContinuationSetBack ()
		{
			var indent = CreateEngine(@"
class Foo {
	void Test ()
	{
		do do do
foo();$");
			Assert.AreEqual("\t\t\t\t\t", indent.ThisLineIndent);
			Assert.AreEqual("\t\t\t\t", indent.NewLineIndent);
		}

		[Test]
		public void TestWhile ()
		{
			var indent = CreateEngine(@"
class Foo {
	void Test ()
	{
		while(true)$");
			Assert.AreEqual("\t\t", indent.ThisLineIndent);
			Assert.AreEqual("\t\t\t", indent.NewLineIndent);
		}

		[Test]
		public void TestParameters ()
		{
			var indent = CreateEngine(@"
class Foo {
	void Test ()
	{
		Foo(true,$");
			Assert.AreEqual("\t\t", indent.ThisLineIndent);
			Assert.AreEqual("\t\t   ", indent.NewLineIndent);
		}

		[Test]
		public void TestParametersCase2 ()
		{
			var indent = CreateEngine(@"
class Foo {
	void Test ()
	{
		Foo($");
			Assert.AreEqual("\t\t", indent.ThisLineIndent);
			Assert.AreEqual("\t\t\t", indent.NewLineIndent);
		}
	}
}

