using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.NRefactory.Editor;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
    [TestFixture]
    public class ScriptTests
    {
        [Test]
        public void InsertAfterStatementIndentsLikeStatement()
        {
            var input =@"
public class Test
{
		void DoublyIndented()
		{
			System.Console.WriteLine(""hello"");
		}
}";
            var expected = @"
public class Test
{
		void DoublyIndented()
		{
			System.Console.WriteLine(""hello"");
			return;
		}
}";

            DoInsertAfterTest(input, expected, (syntaxTree, script) =>
            {
                var nodeToInsert = new ReturnStatement();
                var nodeToInsertAfter = syntaxTree.DescendantsAndSelf.OfType<ExpressionStatement>().Single();
                script.InsertAfter(nodeToInsertAfter,nodeToInsert);
            });
        }

        [Test]
        public void InsertParameterDeclarationDoesNotIndent()
        {
            var input =@"
public class Test
{
	void Test(int a)
	{
	}
}";
            var expected = @"
public class Test
{
	void Test(int a, int b)
	{
	}
}";
            DoInsertAfterTest(input, expected, (syntaxTree, script) =>
            {
                var nodeToInsert = new ParameterDeclaration(new PrimitiveType("int"),"b");
                var nodeToInsertAfter = syntaxTree.DescendantsAndSelf.OfType<ParameterDeclaration>().Single();
                script.InsertAfter(nodeToInsertAfter,nodeToInsert);
            });
        }

        [Test]
        public void InsertFirstParameterDeclarationIntoMethod()
        {
            var input =@"
public class Test
{
	void Test()
	{
	}
}";
            var expected = @"
public class Test
{
	void Test(int a)
	{
	}
}";
            DoInsertAfterTest(input, expected, (syntaxTree, script) =>
            {
                var nodeToInsert = new ParameterDeclaration(new PrimitiveType("int"),"a");
                var nodeToInsertAfter = syntaxTree.DescendantsAndSelf.OfType<MethodDeclaration>().Single().LParToken;
                script.InsertAfter(nodeToInsertAfter,nodeToInsert);
            });
        }

        private static void DoInsertAfterTest(string s, string expected, Action<SyntaxTree, DocumentScript> doInsertion)
        {
            var script = new DocumentScript(new StringBuilderDocument(s), FormattingOptionsFactory.CreateEmpty(), new TextEditorOptions());
            doInsertion(new CSharpParser().Parse(s), script);
            Assert.AreEqual(expected, script.CurrentDocument.Text);
        }
    }
}
