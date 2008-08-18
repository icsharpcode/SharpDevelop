// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using ICSharpCode.PythonBinding;
using NUnit.Framework;

namespace PythonBinding.Tests.Converter
{
	/// <summary>
	/// Converts a C# try-catch-finally to python.
	/// </summary>
	[TestFixture]
	[Ignore("Not ported")]
	public class TryCatchFinallyConversionTestFixture
	{
		CodeTryCatchFinallyStatement tryStatement;
		CodeCatchClause catchClause;
		
		string csharp = "class Loader\r\n" +
						"{\r\n" +
						"\tpublic void load(string xml)\r\n" +
						"\t{\r\n" +
						"\t\ttry {\r\n" +
						"\t\t\tXmlDocument doc = new XmlDocument();\r\n" +
						"\t\t\tdoc.LoadXml(xml);\r\n" +
						"\t\t} catch (XmlException ex) {\r\n" +
						"\t\t\tConsole.WriteLine(ex.ToString());\r\n" +
						"\t\t} finally {\r\n" +
						"\t\t\tConsole.WriteLine(xml);\r\n" +
						"\t\t}\r\n" +
						"\t}\r\n" +
						"}";

		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			CSharpToPythonConverter converter = new CSharpToPythonConverter();
			CodeCompileUnit codeCompileUnit = converter.ConvertToCodeCompileUnit(csharp);
			if (codeCompileUnit.Namespaces.Count > 0) {
				CodeNamespace codeNamespace = codeCompileUnit.Namespaces[0];
				if (codeNamespace.Types.Count > 0) {
					CodeTypeDeclaration codeTypeDeclaration = codeNamespace.Types[0];
					if (codeTypeDeclaration.Members.Count > 0) {
						CodeMemberMethod method = codeTypeDeclaration.Members[0] as CodeMemberMethod;
						if (method != null && method.Statements.Count > 0) {
							tryStatement = method.Statements[0] as CodeTryCatchFinallyStatement;
							if (tryStatement != null && tryStatement.CatchClauses.Count > 0) {
								catchClause = tryStatement.CatchClauses[0];
							}
						}
					}
				}
			}
		}
		
		/// <summary>
		/// Note that Python seems to need to nest the try-catch
		/// inside a try-finally if the finally exists.
		/// </summary>
		[Test]
		public void ConvertedCode()
		{
			string expectedPython = "class Loader(object):\r\n" +
									"\tdef load(self, xml):\r\n" +
									"\t\ttry:\r\n" +
									"\t\t\ttry:\r\n" +
									"\t\t\t\tdoc = XmlDocument()\r\n" +
									"\t\t\t\tdoc.LoadXml(xml)\r\n" +
									"\t\t\texcept XmlException, ex:\r\n" +
									"\t\t\t\tConsole.WriteLine(ex.ToString())\r\n" +
									"\t\tfinally:\r\n" +
									"\t\t\tConsole.WriteLine(xml)";
			
			CSharpToPythonConverter converter = new CSharpToPythonConverter();
			string python = converter.Convert(csharp);
		
			Assert.AreEqual(expectedPython, python);
		}
		
		[Test]
		public void TryStatementExists()
		{
			Assert.IsNotNull(tryStatement);
		}
		
		[Test]
		public void TryStatementHasTwoChildStatements()
		{
			Assert.AreEqual(2, tryStatement.TryStatements.Count);
		}
		
		[Test]
		public void OneCatchStatement()
		{
			Assert.AreEqual(1, tryStatement.CatchClauses.Count);
		}
		
		[Test]
		public void CatchStatementHasOneChildStatement()
		{
			Assert.AreEqual(1, catchClause.Statements.Count);
		}
		
		[Test]
		public void CatchStatementChildIsExpressionStatement()
		{
			Assert.IsInstanceOfType(typeof(CodeExpressionStatement), catchClause.Statements[0]);
		}
		
		[Test]
		public void CatchClauseCatchesXmlException()
		{
			Assert.AreEqual("XmlException", catchClause.CatchExceptionType.BaseType);
		}
		
		[Test]
		public void CatchClauseVariableName()
		{
			Assert.AreEqual("ex", catchClause.LocalName);
		}
		
		[Test]
		public void FinallyBlockHasOneStatement()
		{
			Assert.AreEqual(1, tryStatement.FinallyStatements.Count);
		}
	}
}
