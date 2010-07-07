// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com" />
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ICSharpCode.NRefactory.Parser.VB
{
	public partial class ExpressionFinder
	{
		Stack<Block> stack = new Stack<Block>();
		StringBuilder output = new StringBuilder();
		
		void PopContext()
		{
			if (stack.Count > 0) {
				string indent = new string('\t', stack.Count - 1);
				var item = stack.Pop();
				item.isClosed = true;
				Print(indent + "exit " + item.context);
			} else {
				Print("empty stack");
			}
		}
		
		void PushContext(Context context, Token la, Token t)
		{
			string indent = new string('\t', stack.Count);
			Location l = la == null ? Location.Empty : la.Location;
			
			stack.Push(new Block() { context = context, lastExpressionStart = l });
			Print(indent + "enter " + context);
		}
		
		void SetContext(Context context, Token la, Token t)
		{
			PopContext();
			PushContext(context, la, t);
		}
		
		void ApplyToken(Token token)
		{
			//Console.WriteLine(token);
			
			if (stack.Count == 0 || token == null)
				return;
			
			Block current = stack.Peek();
//
//			switch (token.kind) {
//				case Tokens.EOL:
//				case Tokens.Colon:
//					current.lastExpressionStart = token.EndLocation;
//					break;
//				default:
//					if (Tokens.IdentifierTokens[token.Kind]) {
//						if (lastToken != Tokens.Dot) {
//							if (Tokens.IdentifierTokens[lastToken]) {
//								current.context = Context.Default;
//							}
//							current.lastExpressionStart = token.Location;
//						}
//					} else if (Tokens.SimpleTypeName[token.Kind] || Tokens.ExpressionStart[token.Kind] || token.Kind == Tokens.Literal) {
//						current.lastExpressionStart = token.Location;
//					} else {
//						current.lastExpressionStart = Location.Empty;
//						current.context = Context.Default;
//					}
//			}
		}
		
		void Print(string text)
		{
			//Console.WriteLine(text);
			output.AppendLine(text);
		}
		
		public void SetContext(SnippetType type)
		{
			switch (type) {
				case SnippetType.Expression:
					currentState = startOfExpression;
					break;
			}
			
			Advance();
		}
		
		public string Output {
			get { return output.ToString(); }
		}
		
		public string Stacktrace {
			get {
				string text = "";
				
				foreach (Block b in stack) {
					text += b.ToString() + "\n";
				}
				
				return text;
			}
		}
		
		public Block CurrentBlock {
			get { return stack.Any() ? stack.Peek() : Block.Default; }
		}
		
		public bool NextTokenIsPotentialStartOfXmlMode {
			get { return nextTokenIsPotentialStartOfXmlMode; }
		}
		
		public bool ReadXmlIdentifier {
			get { return readXmlIdentifier; }
			set { readXmlIdentifier = value; }
		}
		
		public bool NextTokenIsStartOfImportsOrAccessExpression {
			get { return nextTokenIsStartOfImportsOrAccessExpression; }
		}
		
		public bool WasQualifierTokenAtStart {
			get { return wasQualifierTokenAtStart; }
		}
		
		public List<Token> Errors {
			get { return errors; }
		}
	}
	
	public enum Context
	{
		Global,
		Type,
		Member,
		IdentifierExpected,
		Body,
		Xml,
		Attribute,
		Query,
		Expression,
		Debug,
		Default
	}
	
	public class Block
	{
		public static readonly Block Default = new Block() {
			context = Context.Global,
			lastExpressionStart = Location.Empty
		};
		
		public Context context;
		public Location lastExpressionStart;
		public bool isClosed;
		
		public override string ToString()
		{
			return string.Format("[Block Context={0}, LastExpressionStart={1}, IsClosed={2}]", context, lastExpressionStart, isClosed);
		}
	}
}
