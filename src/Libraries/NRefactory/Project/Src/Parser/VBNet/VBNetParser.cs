/*
 * Created by SharpDevelop.
 * User: Daniel Grunwald
 * Date: 16.07.2006
 * Time: 15:55
 */

using System;
using System.Text;
using System.Collections.Generic;
using ICSharpCode.NRefactory.Ast;

namespace ICSharpCode.NRefactory.Parser.VB
{
	internal sealed partial class Parser : AbstractParser
	{
		Lexer lexer;
		
		public Parser(ILexer lexer) : base(lexer)
		{
			this.lexer = (Lexer)lexer;
		}
		
		private StringBuilder qualidentBuilder = new StringBuilder();

		Token t
		{
			[System.Diagnostics.DebuggerStepThrough]
			get {
				return lexer.Token;
			}
		}
		Token la
		{
			[System.Diagnostics.DebuggerStepThrough]
			get {
				return lexer.LookAhead;
			}
		}

		Token Peek (int n)
		{
			lexer.StartPeek();
			Token x = la;
			while (n > 0) {
				x = lexer.Peek();
				n--;
			}
			return x;
		}

		public void Error(string s)
		{
			if (errDist >= minErrDist) {
				errors.Error(la.line, la.col, s);
			}
			errDist = 0;
		}

		public override Expression ParseExpression()
		{
			lexer.NextToken();
			Expression expr;
			Expr(out expr);
			return expr;
		}

		bool LeaveBlock()
		{
			int peek = Peek(1).kind;
			return Tokens.BlockSucc[la.kind] && (la.kind != Tokens.End || peek == Tokens.EOL || peek == Tokens.Colon);
		}

		/* True, if "." is followed by an ident */
		bool DotAndIdentOrKw () {
			int peek = Peek(1).kind;
			return la.kind == Tokens.Dot && (peek == Tokens.Identifier || peek >= Tokens.AddHandler);
		}

		bool IsEndStmtAhead()
		{
			int peek = Peek(1).kind;
			return la.kind == Tokens.End && (peek == Tokens.EOL || peek == Tokens.Colon);
		}

		bool IsNotClosingParenthesis() {
			return la.kind != Tokens.CloseParenthesis;
		}

		/*
	True, if ident is followed by "=" or by ":" and "="
		 */
		bool IsNamedAssign() {
//	if(Peek(1).kind == Tokens.Assign) return true; // removed: not in the lang spec
			if(Peek(1).kind == Tokens.Colon && Peek(2).kind == Tokens.Assign) return true;
			return false;
		}

		bool IsObjectCreation() {
			return la.kind == Tokens.As && Peek(1).kind == Tokens.New;
		}

		/*
	True, if "<" is followed by the ident "assembly" or "module"
		 */
		bool IsGlobalAttrTarget () {
			Token pt = Peek(1);
			return la.kind == Tokens.LessThan && ( string.Equals(pt.val, "assembly", StringComparison.InvariantCultureIgnoreCase) || string.Equals(pt.val, "module", StringComparison.InvariantCultureIgnoreCase));
		}

		/*
	True if the next token is a "(" and is followed by "," or ")"
		 */
		bool IsDims()
		{
			int peek = Peek(1).kind;
			return la.kind == Tokens.OpenParenthesis
				&& (peek == Tokens.Comma || peek == Tokens.CloseParenthesis);
		}

		bool IsSize()
		{
			return la.kind == Tokens.OpenParenthesis;
		}

		/*
	True, if the comma is not a trailing one,
	like the last one in: a, b, c,
		 */
		bool NotFinalComma() {
			int peek = Peek(1).kind;
			return la.kind == Tokens.Comma &&
				peek != Tokens.CloseCurlyBrace;
		}

		/*
	True, if the next token is "Else" and this one
	if followed by "If"
		 */
		bool IsElseIf()
		{
			int peek = Peek(1).kind;
			return la.kind == Tokens.Else && peek == Tokens.If;
		}

		/*
	True if the next token is goto and this one is
	followed by minus ("-") (this is allowd in in
	error clauses)
		 */
		bool IsNegativeLabelName()
		{
			int peek = Peek(1).kind;
			return la.kind == Tokens.GoTo && peek == Tokens.Minus;
		}

		/*
	True if the next statement is a "Resume next" statement
		 */
		bool IsResumeNext()
		{
			int peek = Peek(1).kind;
			return la.kind == Tokens.Resume && peek == Tokens.Next;
		}

		/*
	True, if ident/literal integer is followed by ":"
		 */
		bool IsLabel()
		{
			return (la.kind == Tokens.Identifier || la.kind == Tokens.LiteralInteger)
				&& Peek(1).kind == Tokens.Colon;
		}

		bool IsNotStatementSeparator()
		{
			return la.kind == Tokens.Colon && Peek(1).kind == Tokens.EOL;
		}

		static bool IsMustOverride(Modifiers m)
		{
			return m.Contains(Modifier.Abstract);
		}

		TypeReferenceExpression GetTypeReferenceExpression(Expression expr, List<TypeReference> genericTypes)
		{
			TypeReferenceExpression	tre = expr as TypeReferenceExpression;
			if (tre != null) {
				return new TypeReferenceExpression(new TypeReference(tre.TypeReference.Type, tre.TypeReference.PointerNestingLevel, tre.TypeReference.RankSpecifier, genericTypes));
			}
			StringBuilder b = new StringBuilder();
			if (!WriteFullTypeName(b, expr)) {
				// there is some TypeReferenceExpression hidden in the expression
				while (expr is FieldReferenceExpression) {
					expr = ((FieldReferenceExpression)expr).TargetObject;
				}
				tre = expr as TypeReferenceExpression;
				if (tre != null) {
					TypeReference typeRef = tre.TypeReference;
					if (typeRef.GenericTypes.Count == 0) {
						typeRef = typeRef.Clone();
						typeRef.Type += "." + b.ToString();
						typeRef.GenericTypes.AddRange(genericTypes);
					} else {
						typeRef = new InnerClassTypeReference(typeRef, b.ToString(), genericTypes);
					}
					return new TypeReferenceExpression(typeRef);
				}
			}
			return new TypeReferenceExpression(new TypeReference(b.ToString(), 0, null, genericTypes));
		}

		/* Writes the type name represented through the expression into the string builder. */
		/* Returns true when the expression was converted successfully, returns false when */
		/* There was an unknown expression (e.g. TypeReferenceExpression) in it */
		bool WriteFullTypeName(StringBuilder b, Expression expr)
		{
			FieldReferenceExpression fre = expr as FieldReferenceExpression;
			if (fre != null) {
				bool result = WriteFullTypeName(b, fre.TargetObject);
				if (b.Length > 0) b.Append('.');
				b.Append(fre.FieldName);
				return result;
			} else if (expr is IdentifierExpression) {
				b.Append(((IdentifierExpression)expr).Identifier);
				return true;
			} else {
				return false;
			}
		}

		/*
		True, if lookahead is a local attribute target specifier,
		i.e. one of "event", "return", "field", "method",
		"module", "param", "property", or "type"
		 */
		bool IsLocalAttrTarget() {
			// TODO
			return false;
		}
	}
}
