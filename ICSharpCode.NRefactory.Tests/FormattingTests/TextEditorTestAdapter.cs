using System;
using System.Collections.Generic;
using System.Text;
using ICSharpCode.NRefactory.CSharp;
using System.IO;
using ICSharpCode.NRefactory.Editor;
using NUnit.Framework;
using ICSharpCode.NRefactory.CSharp.Refactoring;

namespace ICSharpCode.NRefactory.CSharp.FormattingTests
{
	public abstract class TestBase
	{
		/*public static string ApplyChanges (string text, List<TextReplaceAction> changes)
		{
			changes.Sort ((x, y) => y.Offset.CompareTo (x.Offset));
			StringBuilder b = new StringBuilder(text);
			foreach (var change in changes) {
				//Console.WriteLine ("---- apply:" + change);
//				Console.WriteLine (adapter.Text);
				if (change.Offset > b.Length)
					continue;
				b.Remove(change.Offset, change.RemovedChars);
				b.Insert(change.Offset, change.InsertedText);
			}
//			Console.WriteLine ("---result:");
//			Console.WriteLine (adapter.Text);
			return b.ToString();
		}*/
		
		protected static IDocument GetResult(CSharpFormattingOptions policy, string input, FormattingMode mode = FormattingMode.Intrusive)
		{
			input = NormalizeNewlines(input);
			var document = new StringBuilderDocument(input);
			var options = new TextEditorOptions();
			options.EolMarker = "\n";
			options.WrapLineLength = 80;
			var visitor = new CSharpFormatter (policy, options);
			visitor.FormattingMode = mode;
			var syntaxTree = new CSharpParser ().Parse (document, "test.cs");
			var changes = visitor.AnalyzeFormatting(document, syntaxTree);
			changes.ApplyChanges();
			return document;
		}
		
		protected static IDocument Test (CSharpFormattingOptions policy, string input, string expectedOutput, FormattingMode mode = FormattingMode.Intrusive)
		{
			expectedOutput = NormalizeNewlines(expectedOutput);
			IDocument doc = GetResult(policy, input, mode);
			if (expectedOutput != doc.Text) {
				Console.WriteLine ("expected:");
				Console.WriteLine (expectedOutput);
				Console.WriteLine ("got:");
				Console.WriteLine (doc.Text);
				for (int i = 0; i < expectedOutput.Length && i < doc.TextLength; i++) {
					if (expectedOutput [i] != doc.GetCharAt(i)) {
						Console.WriteLine (
							"i:"+i+" differ:"+ 
							expectedOutput[i].ToString ().Replace ("\n", "\\n").Replace ("\r", "\\r").Replace ("\t", "\\t") +
							" !=" + 
							doc.GetCharAt(i).ToString ().Replace ("\n", "\\n").Replace ("\r", "\\r").Replace ("\t", "\\t")
						);
						Console.WriteLine(">"+expectedOutput.Substring (i).Replace ("\n", "\\n").Replace ("\r", "\\r").Replace ("\t", "\\t"));
						Console.WriteLine(">"+doc.Text.Substring (i).Replace ("\n", "\\n").Replace ("\r", "\\r").Replace ("\t", "\\t"));
						break;
					}
				}
			}
			Assert.AreEqual (expectedOutput, doc.Text);
			return doc;
		}
		
		protected static string NormalizeNewlines(string input)
		{
			return input.Replace("\r\n", "\n");
		}

		protected static void Continue (CSharpFormattingOptions policy, IDocument document, string expectedOutput, FormattingMode formattingMode = FormattingMode.OnTheFly)
		{
			expectedOutput = NormalizeNewlines (expectedOutput);
			var options = new TextEditorOptions ();
			options.EolMarker = "\n";
			var formatter = new CSharpFormatter (policy, options);
			formatter.FormattingMode = formattingMode;
			string newText = formatter.Format (document);
			if (expectedOutput != newText) {
				Console.WriteLine (newText);
			}
			Assert.AreEqual (expectedOutput, newText);
		}

		
	}
}

