// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.DefaultEditor.Codons;
using ICSharpCode.TextEditor.Document;
using NUnit.Framework;

namespace PythonBinding.Tests
{
	/// <summary>
	/// Tests the Python.xshd syntax mode information.
	/// </summary>
	[TestFixture]
	public class PythonSyntaxModeTestFixture
	{
		AddIn addin;
		Codon syntaxModeCodon;
		HighlightRuleSet defaultRuleSet;
		Span lineCommentSpan;
		DefaultHighlightingStrategy highlightingStrategy;
		XmlDocument syntaxModeDocument;
		Span stringSpan;
		Span charSpan;
		Span docCommentSpan;
		Span singleQuoteDocCommentSpan;
		const string SyntaxModePath = "/SharpDevelop/ViewContent/DefaultTextEditor/SyntaxModes";
		XmlElement importsKeyWordsElement;
		XmlElement iterationStatementsKeyWordsElement;
		XmlElement jumpStatementsKeyWordsElement;
		XmlElement operatorStatementsKeyWordsElement;
		XmlElement markPreviousElement;
		XmlElement selectionStatementsKeyWordsElement;
		XmlElement functionDefinitionKeyWordsElement;
		XmlElement exceptionHandlingKeyWordsElement;
		XmlElement withStatementKeyWordsElement;
		XmlElement passStatementKeyWordsElement;
		XmlElement classStatementKeyWordsElement;
		XmlElement builtInStatementsKeyWordsElement;
		AddIn formsDesignerAddIn;
		
		[TestFixtureSetUp]
		public void SetupFixture()
		{
			// Add forms designer addin to the AddInTree.
			bool formsDesignerAddInExists = false;
			foreach (AddIn addIn in AddInTree.AddIns) {
				if (addIn.Manifest.Identities.ContainsKey("ICSharpCode.FormsDesigner")) {
					formsDesignerAddInExists = true;
					break;
				}
			}
			
			if (!formsDesignerAddInExists) {
				formsDesignerAddIn = AddIn.Load(new StringReader(GetFormsDesignerAddInFile()));
				formsDesignerAddIn.Enabled = true;
				
				string codeBase = typeof(PythonSyntaxModeTestFixture).Assembly.CodeBase.Replace("file:///", String.Empty);
				string folder = Path.GetDirectoryName(codeBase);
				
				Type type = formsDesignerAddIn.GetType();
				FieldInfo fieldInfo = type.GetField("addInFileName", BindingFlags.NonPublic | BindingFlags.Instance);
				fieldInfo.SetValue(formsDesignerAddIn, Path.Combine(folder, "FormsDesigner.addin"));
				AddInTree.InsertAddIn(formsDesignerAddIn);
			}
			
			using (TextReader reader = PythonBindingAddInFile.ReadAddInFile()) {
				addin = AddIn.Load(reader, String.Empty);
							
				// Get syntax mode codon.
				syntaxModeCodon = null;
				ExtensionPath path = addin.Paths[SyntaxModePath];
				foreach (Codon codon in path.Codons) {
					if (codon.Id == "Python.SyntaxMode") {
						syntaxModeCodon = codon;
						break;
					}
				}
				
				// Get syntax mode.
				highlightingStrategy = null;
				if (syntaxModeCodon != null) {
					SyntaxModeDoozer doozer = new SyntaxModeDoozer();
					AddInTreeSyntaxMode syntaxMode = (AddInTreeSyntaxMode)doozer.BuildItem(null, syntaxModeCodon, null);
					highlightingStrategy = HighlightingDefinitionParser.Parse(syntaxMode, syntaxMode.CreateTextReader());
				
					// Load Python syntax file into XML document since
					// we cannot get all the information stored in this
					// document through the HighlightRuleSet class. For
					// example KeyWords are only accessible through a
					// LookupTable does not have a getter which is easy
					// to use.
					syntaxModeDocument = new XmlDocument();
					syntaxModeDocument.Load(syntaxMode.CreateTextReader());
					
					// Get default ruleset.
					foreach (HighlightRuleSet ruleSet in highlightingStrategy.Rules) {
						if (String.IsNullOrEmpty(ruleSet.Name)) {
							defaultRuleSet = ruleSet;
							break;
						}
					}
					
					// Get keywords elements.
					importsKeyWordsElement = GetKeyWordsElement("Imports");
					iterationStatementsKeyWordsElement = GetKeyWordsElement("IterationStatements");
					jumpStatementsKeyWordsElement = GetKeyWordsElement("JumpStatements");
					operatorStatementsKeyWordsElement = GetKeyWordsElement("OperatorStatements");
					selectionStatementsKeyWordsElement = GetKeyWordsElement("SelectionStatements");
					functionDefinitionKeyWordsElement = GetKeyWordsElement("FunctionDefinition");
					exceptionHandlingKeyWordsElement = GetKeyWordsElement("ExceptionHandlingStatements");
					withStatementKeyWordsElement = GetKeyWordsElement("WithStatement");
					passStatementKeyWordsElement = GetKeyWordsElement("PassStatement");
					classStatementKeyWordsElement = GetKeyWordsElement("ClassStatement");
					builtInStatementsKeyWordsElement = GetKeyWordsElement("BuiltInStatements");
					
					// Get mark previous.
					markPreviousElement = syntaxModeDocument.SelectSingleNode("//MarkPrevious") as XmlElement;
				}
				
				// Get line comment span.
				if (defaultRuleSet != null) {
					foreach (Span s in defaultRuleSet.Spans) {
						if (s.Name == "LineComment") {
							lineCommentSpan = s;
						} else if (s.Name == "String") {
							stringSpan = s;
						} else if (s.Name == "Char") {
							charSpan = s;
						} else if (s.Name == "DocComment") {
							docCommentSpan = s;
						} else if (s.Name == "SingleQuoteDocComment") {
							singleQuoteDocCommentSpan = s;
						}
					}
				}
			}
		}
		
		[Test]
		public void SyntaxModePathExists()
		{
			Assert.IsTrue(addin.Paths.ContainsKey(SyntaxModePath));
		}
		
		[Test]
		public void SyntaxModeCodonExists()
		{
			Assert.IsNotNull(syntaxModeCodon);
		}
		
		[Test]
		public void SyntaxModeFileExtension()
		{
			Assert.AreEqual(".py", syntaxModeCodon.Properties["extensions"]);
		}
		
		[Test]
		public void SyntaxModeLanguage()
		{
			Assert.AreEqual("Python", syntaxModeCodon.Properties["name"]);
		}
		
		[Test]
		public void SyntaxModeResourceName()
		{
			Assert.AreEqual("ICSharpCode.PythonBinding.Resources.Python.xshd", syntaxModeCodon.Properties["resource"]);
		}
		
		[Test]
		public void SyntaxModeResourceExists()
		{
			Assert.AreEqual("Python", highlightingStrategy.Name);
		}
		
		[Test]
		public void DefaultRuleSetExists()
		{
			Assert.IsNotNull(defaultRuleSet);
		}
		
		[Test]
		public void DefaultRuleSetIgnoreCase()
		{
			Assert.IsFalse(defaultRuleSet.IgnoreCase);
		}
		
		[Test]
		public void LineCommentSpanExists()
		{
			Assert.IsNotNull(lineCommentSpan);
		}
		
		[Test]
		public void LineCommentSpanBegin()
		{
			char[] begin = new char[] {'#'};
			Assert.AreEqual(begin, lineCommentSpan.Begin);
		}
		
		[Test]
		public void LineCommentSpanStopAtEndOfLine()
		{
			Assert.IsTrue(lineCommentSpan.StopEOL);
		}
		
		[Test]
		public void LineCommentSpanColor()
		{
			HighlightColor expectedColor = new HighlightColor(Color.Green, false, false);
			Assert.AreEqual(expectedColor.ToString(), lineCommentSpan.Color.ToString());
		}
		
		[Test]
		public void DocCommentStopAtEndOfLine()
		{
			Assert.IsFalse(docCommentSpan.StopEOL);
		}
		
		[Test]
		public void DocCommentBegin()
		{
			char[] begin = new char[] {'\"', '\"', '\"'};
			Assert.AreEqual(begin, docCommentSpan.Begin);
		}
		
		[Test]
		public void DocCommentEnd()
		{
			char[] begin = new char[] {'\"', '\"', '\"'};
			Assert.AreEqual(begin, docCommentSpan.End);
		}
		
		[Test]
		public void DocCommentSpanColor()
		{
			HighlightColor expectedColor = new HighlightColor(Color.Green, false, false);
			Assert.AreEqual(expectedColor.ToString(), docCommentSpan.Color.ToString());
		}
		
		[Test]
		public void DocCommentSpanOccursBeforeStringSpan()
		{
			Assert.IsTrue(SpanOccursBefore("DocComment", "String"));
		}
		
		bool SpanOccursBefore(string before, string after)
		{
			int beforeIndex = -1;
			int afterIndex = -1;
			
			for (int i = 0; i < defaultRuleSet.Spans.Count; ++i) {
				Span span = (Span)defaultRuleSet.Spans[i];
				if (span.Name == before) {
					beforeIndex = i;
				} else if (span.Name == after) {
					afterIndex = i;
				}
			}
			
			Assert.AreNotEqual(-1, beforeIndex);
			Assert.AreNotEqual(-1, afterIndex);
			
			return beforeIndex < afterIndex;
		}
		
		[Test]
		public void SingleQuoteDocCommentStopAtEndOfLine()
		{
			Assert.IsFalse(singleQuoteDocCommentSpan.StopEOL);
		}
		
		[Test]
		public void SingleQuoteDocCommentBegin()
		{
			char[] begin = new char[] {'\'', '\'', '\''};
			Assert.AreEqual(begin, singleQuoteDocCommentSpan.Begin);
		}
		
		[Test]
		public void SingleQuoteDocCommentEnd()
		{
			char[] begin = new char[] {'\'', '\'', '\''};
			Assert.AreEqual(begin, singleQuoteDocCommentSpan.End);
		}
		
		[Test]
		public void SingleQuoteDocCommentSpanColor()
		{
			HighlightColor expectedColor = new HighlightColor(Color.Green, false, false);
			Assert.AreEqual(expectedColor.ToString(), singleQuoteDocCommentSpan.Color.ToString());
		}
		
		[Test]
		public void SingleQuoteDocCommentSpanOccursBeforeCharSpan()
		{
			Assert.IsTrue(SpanOccursBefore("DocComment", "Char"));
		}
		
		[Test]
		public void ImportsKeyWordsExist()
		{
			Assert.IsNotNull(importsKeyWordsElement);
		}
		
		[Test]
		public void ImportsKeyWordsColor()
		{
			AssertAreHighlightColorsEqual(Color.Green, true, importsKeyWordsElement);
		}
		
		[Test]
		public void ImportsKeyWords()
		{
			string[] expectedKeyWords = new string[] { 
				"import", 
				"from" };
			Assert.AreEqual(expectedKeyWords, GetKeyWords(importsKeyWordsElement));
		}
				
		[Test]
		public void StringSpanBegin()
		{
			char[] expectedBegin = new char[] {'\"'};
			Assert.AreEqual(expectedBegin, stringSpan.Begin);
		}
		
		[Test]
		public void StringSpanEnd()
		{
			char[] expectedEnd = new char[] {'\"'};
			Assert.AreEqual(expectedEnd, stringSpan.End);
		}
		
		[Test]
		public void StringStopAtEndOfLine()
		{
			Assert.IsTrue(stringSpan.StopEOL);
		}
		
		[Test]
		public void StringSpanColor()
		{
			HighlightColor expectedColor = new HighlightColor(Color.Blue, false, false);
			Assert.AreEqual(expectedColor.ToString(), stringSpan.Color.ToString());
		}
		
		[Test]
		public void StringSpanEscapeCharacter()
		{
			Assert.AreEqual('\\', stringSpan.EscapeCharacter);
		}
		
		[Test]
		public void CharSpanBegin()
		{
			char[] expectedBegin = new char[] {'\''};
			Assert.AreEqual(expectedBegin, charSpan.Begin);
		}
		
		[Test]
		public void CharSpanEnd()
		{
			char[] expectedEnd = new char[] {'\''};
			Assert.AreEqual(expectedEnd, charSpan.End);
		}
		
		[Test]
		public void CharStopAtEndOfLine()
		{
			Assert.IsTrue(charSpan.StopEOL);
		}
		
		[Test]
		public void CharSpanColor()
		{
			HighlightColor expectedColor = new HighlightColor(Color.Magenta, false, false);
			Assert.AreEqual(expectedColor.ToString(), charSpan.Color.ToString());
		}
		
		[Test]
		public void CharSpanEscapeCharacter()
		{
			Assert.AreEqual('\\', charSpan.EscapeCharacter);
		}
		
		[Test]
		public void DigitsColor()
		{
			HighlightColor expectedColor = new HighlightColor(Color.DarkBlue, false, false);
			Assert.AreEqual(expectedColor.ToString(), highlightingStrategy.DigitColor.ToString());
		}
		
		[Test]
		public void Delimiters()
		{
			XmlElement delimiterElement = (XmlElement)syntaxModeDocument.SelectSingleNode("//Delimiters");
			
			string delimiters = "()[]{}@,:.`=;+-*/% &|^><";
			Assert.AreEqual(delimiters, delimiterElement.InnerText);
		}
		
		[Test]
		public void IterationStatements()
		{
			string[] expectedKeyWords = new string[] { 
				"for",
				"in",
				"while" };
			Assert.AreEqual(expectedKeyWords, GetKeyWords(iterationStatementsKeyWordsElement));
		}
		
		[Test]
		public void IterationStatementsColor()
		{
			AssertAreHighlightColorsEqual(Color.Blue, true, iterationStatementsKeyWordsElement);
		}
		
		[Test]
		public void JumpStatements()
		{
			string[] expectedKeyWords = new string[] { 
				"break",
				"continue",
				"yield",
				"return" };
			Assert.AreEqual(expectedKeyWords, GetKeyWords(jumpStatementsKeyWordsElement));
		}
		
		[Test]
		public void JumpStatementsColor()
		{
			AssertAreHighlightColorsEqual(Color.Navy, false, jumpStatementsKeyWordsElement);
		}
		
		[Test]
		public void WithStatement()
		{
			string[] expectedKeyWords = new string[] { "with" };
			Assert.AreEqual(expectedKeyWords, GetKeyWords(withStatementKeyWordsElement));
		}
		
		[Test]
		public void WithStatementColor()
		{
			AssertAreHighlightColorsEqual(Color.DarkViolet, false, withStatementKeyWordsElement);
		}
		
		[Test]
		public void PassStatement()
		{
			string[] expectedKeyWords = new string[] { "pass" };
			Assert.AreEqual(expectedKeyWords, GetKeyWords(passStatementKeyWordsElement));
		}
		
		[Test]
		public void PassStatementColor()
		{
			AssertAreHighlightColorsEqual(Color.Gray, false, passStatementKeyWordsElement);
		}
		
		[Test]
		public void OperatorStatements()
		{
			string[] expectedKeyWords = new string[] { 
				"and",
				"as",
				"is",
				"not",
				"or" };
			Assert.AreEqual(expectedKeyWords, GetKeyWords(operatorStatementsKeyWordsElement));
		}
		
		[Test]
		public void OperatorStatementsColor()
		{
			AssertAreHighlightColorsEqual(Color.DarkCyan, true, operatorStatementsKeyWordsElement);
		}

		[Test]
		public void MarkPreviousBeforeCharacter()
		{
			Assert.AreEqual("(", markPreviousElement.InnerText);
		}
		
		[Test]
		public void MarkPreviousColor()
		{
			AssertAreHighlightColorsEqual(Color.MidnightBlue, true, markPreviousElement);
		}		
		
		[Test]
		public void SelectionStatements()
		{
			string[] expectedKeyWords = new string[] { 
				"elif",
				"else",
				"if" };
			Assert.AreEqual(expectedKeyWords, GetKeyWords(selectionStatementsKeyWordsElement));
		}
		
		[Test]
		public void SelectionStatementsColor()
		{
			AssertAreHighlightColorsEqual(Color.Blue, true, selectionStatementsKeyWordsElement);
		}		

		[Test]
		public void FunctionDefinitionKeyWord()
		{
			string[] expectedKeyWords = new string[] { "def" };
			Assert.AreEqual(expectedKeyWords, GetKeyWords(functionDefinitionKeyWordsElement));
		}
		
		[Test]
		public void FunctionDefinitionColor()
		{
			AssertAreHighlightColorsEqual(Color.Blue, true, functionDefinitionKeyWordsElement);
		}
		
		[Test]
		public void ExceptionHandlingKeyWords()
		{
			string[] expectedKeyWords = new string[] { 
				"except",
				"finally",
				"raise",
				"try" };
			Assert.AreEqual(expectedKeyWords, GetKeyWords(exceptionHandlingKeyWordsElement));
		}

		[Test]
		public void ExceptionHandlingKeyWordsColor()
		{
			AssertAreHighlightColorsEqual(Color.Teal, true, exceptionHandlingKeyWordsElement);
		}
		
		[Test]
		public void ClassStatement()
		{
			string[] expectedKeyWords = new string[] { "class" };
			Assert.AreEqual(expectedKeyWords, GetKeyWords(classStatementKeyWordsElement));
		}

		[Test]
		public void ClassStatementColor()
		{
			AssertAreHighlightColorsEqual(Color.Blue, true, classStatementKeyWordsElement);
		}		
		
		[Test]
		public void BuiltInStatements()
		{
			string[] expectedKeyWords = new string[] { 
				"assert",
				"del",
				"exec",
				"global",
				"lambda",
				"print" };
			Assert.AreEqual(expectedKeyWords, GetKeyWords(builtInStatementsKeyWordsElement));
		}

		[Test]
		public void BuiltInStatementColor()
		{
			AssertAreHighlightColorsEqual(Color.MidnightBlue, true, builtInStatementsKeyWordsElement);
		}	
		
		[Test]
		public void LineCommentPropertyExists()
		{
			Assert.AreEqual("#", highlightingStrategy.Properties["LineComment"]);
		}
		
		/// <summary>
		/// Returns the KeyWords element from the Python syntax file 
		/// (Python.xshd) with the specified name.
		/// </summary>
		XmlElement GetKeyWordsElement(string name)
		{
			string xpath = String.Concat("//KeyWords[@name='", name, "']");
			return syntaxModeDocument.SelectSingleNode(xpath) as XmlElement;
		}
		
		/// <summary>
		/// Returns a list of Key/@word items from the specified KeyWords 
		/// element.
		/// </summary>
		string[] GetKeyWords(XmlElement element)
		{
			List<string> actualKeyWords = new List<string>();
			foreach (XmlNode node in element.SelectNodes("Key/@word")) {
				actualKeyWords.Add(node.Value);
			}
			return actualKeyWords.ToArray();
		}
		
		/// <summary>
		/// Compares the expected colour with the actual colour returned from 
		/// the xml element (typically a KeyWords element).
		/// </summary>
		void AssertAreHighlightColorsEqual(Color color, bool bold, XmlElement element)
		{
			HighlightColor expectedColor = new HighlightColor(color, bold, false);
			HighlightColor actualColor = new HighlightColor(element);
			Assert.AreEqual(expectedColor.ToString(), actualColor.ToString());
		}
		
		string GetFormsDesignerAddInFile()
		{
			return "<AddIn name        = \"Forms Designer\"" +
				   "    author      = \"\"" +
				   "    copyright   = \"prj:///doc/copyright.txt\"" +
				   "    description = \"\">" +
					"" +
					"<Manifest>" +
					"	<Identity name=\"ICSharpCode.FormsDesigner\"/>" +
					"</Manifest>" +
					"" +
					"<Runtime>" +
					"	<Import assembly=\"ICSharpCode.FormsDesigner\"/>" +
					"</Runtime>" +
					"</AddIn>";
		}
	}
}
