
#line  1 "cs.ATG" 
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Parser.AST;
using ASTAttribute = ICSharpCode.NRefactory.Parser.AST.Attribute;
/*
  Parser.frame file for NRefactory.
 */
using System;
using System.Reflection;

namespace ICSharpCode.NRefactory.Parser.CSharp {



internal class Parser : AbstractParser
{
	const int maxT = 125;

	const  bool   T            = true;
	const  bool   x            = false;
	

#line  13 "cs.ATG" 
string        assemblyName     = null;
StringBuilder qualidentBuilder = new StringBuilder();

public string ContainingAssembly {
	set {
		assemblyName = value;
	}
}

Token t {
	get {
		return lexer.Token;
	}
}

Token la {
	get {
		return lexer.LookAhead;
	}
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
	Expression expr;
	Expr(out expr);
	return expr;
}
// Begin ISTypeCast
bool IsTypeCast()
{
	if (la.kind != Tokens.OpenParenthesis) {
		return false;
	}
	if (IsSimpleTypeCast()) {
		return true;
	}
	return GuessTypeCast();
}

// "(" ( typeKW [ "[" {","} "]" | "*" ] | void  ( "[" {","} "]" | "*" ) ) ")"
// only for built-in types, all others use GuessTypeCast!
bool IsSimpleTypeCast ()
{
	// assert: la.kind == _lpar
	lexer.StartPeek();
	Token pt = lexer.Peek();
	
	if (!IsTypeKWForTypeCast(ref pt)) {
		return false;
	}
	return pt.kind == Tokens.CloseParenthesis;
}

/* !!! Proceeds from current peek position !!! */
bool IsTypeKWForTypeCast(ref Token pt)
{
	if (Tokens.TypeKW[pt.kind]) {
		pt = lexer.Peek();
		if (pt.kind == Tokens.Times || pt.kind == Tokens.OpenSquareBracket) {
			return IsPointerOrDims(ref pt);
		} else {
		  return true;
		}
	} else if (pt.kind == Tokens.Void) {
		pt = lexer.Peek();
		return IsPointerOrDims(ref pt);
	}
	return false;
}

/* !!! Proceeds from current peek position !!! */
bool IsTypeNameOrKWForTypeCast(ref Token pt)
{
	if (IsTypeKWForTypeCast(ref pt))
		return true;
	else
		return IsTypeNameForTypeCast(ref pt);
}

// TypeName = ident [ "::" ident ] {  "." ident } ["<" TypeNameOrKW { "," TypeNameOrKW } ">" ] PointerOrDims
/* !!! Proceeds from current peek position !!! */
bool IsTypeNameForTypeCast(ref Token pt)
{
	// ident
	if (pt.kind != Tokens.Identifier) {
		return false;
	}	
	pt = Peek();
	// "::" ident
	if (pt.kind == Tokens.DoubleColon) {
		pt = Peek();
		if (pt.kind != Tokens.Identifier) {
			return false;
		}
		pt = Peek();
	}
	// { "." ident }
	while (pt.kind == Tokens.Dot) {
		pt = Peek();
		if (pt.kind != Tokens.Identifier) {
			return false;
		}
		pt = Peek();
	}
	if (pt.kind == Tokens.LessThan) {
		do {
			pt = Peek();
			if (!IsTypeNameOrKWForTypeCast(ref pt)) {
				return false;
			}
		} while (pt.kind == Tokens.Comma);
		if (pt.kind != Tokens.GreaterThan) {
			return false;
		}
		pt = Peek();
	}
	if (pt.kind == Tokens.Times || pt.kind == Tokens.OpenSquareBracket) {
		return IsPointerOrDims(ref pt);
	}
	return true;
}

// "(" TypeName ")" castFollower
bool GuessTypeCast ()
{
	// assert: la.kind == _lpar
	StartPeek();
	Token pt = Peek();

	if (!IsTypeNameForTypeCast(ref pt)) {
		return false;
	}
	
	// ")"
	if (pt.kind != Tokens.CloseParenthesis) {
		return false;
	}
	// check successor
	pt = Peek();
	return Tokens.CastFollower[pt.kind] || (Tokens.TypeKW[pt.kind] && lexer.Peek().kind == Tokens.Dot);
}
// END IsTypeCast

/* Checks whether the next sequences of tokens is a qualident *
 * and returns the qualident string                           */
/* !!! Proceeds from current peek position !!! */
bool IsQualident(ref Token pt, out string qualident)
{
	if (pt.kind == Tokens.Identifier) {
		qualidentBuilder.Length = 0; qualidentBuilder.Append(pt.val);
		pt = Peek();
	while (pt.kind == Tokens.Dot || pt.kind == Tokens.DoubleColon) {
			pt = Peek();
			if (pt.kind != Tokens.Identifier) {
				qualident = String.Empty;
				return false;
			}
			qualidentBuilder.Append('.');
			qualidentBuilder.Append(pt.val);
			pt = Peek();
		}
		qualident = qualidentBuilder.ToString();
		return true;
	}
	qualident = String.Empty;
	return false;
}

/* Skips generic type extensions */
/* !!! Proceeds from current peek position !!! */

/* skip: { "*" | "[" { "," } "]" } */
/* !!! Proceeds from current peek position !!! */
bool IsPointerOrDims (ref Token pt)
{
	for (;;) {
		if (pt.kind == Tokens.OpenSquareBracket) {
			do pt = Peek();
			while (pt.kind == Tokens.Comma);
			if (pt.kind != Tokens.CloseSquareBracket) return false;
		} else if (pt.kind != Tokens.Times) break;
		pt = Peek();
	}
	return true;
}

/* Return the n-th token after the current lookahead token */
void StartPeek()
{
	lexer.StartPeek();
}

Token Peek()
{
	return lexer.Peek();
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

/*-----------------------------------------------------------------*
 * Resolver routines to resolve LL(1) conflicts:                   *                                                  *
 * These resolution routine return a boolean value that indicates  *
 * whether the alternative at hand shall be choosen or not.        *
 * They are used in IF ( ... ) expressions.                        *       
 *-----------------------------------------------------------------*/

/* True, if ident is followed by "=" */
bool IdentAndAsgn ()
{
	return la.kind == Tokens.Identifier && Peek(1).kind == Tokens.Assign;
}

bool IsAssignment () { return IdentAndAsgn(); }

/* True, if ident is followed by ",", "=", or ";" */
bool IdentAndCommaOrAsgnOrSColon () {
	int peek = Peek(1).kind;
	return la.kind == Tokens.Identifier && 
	       (peek == Tokens.Comma || peek == Tokens.Assign || peek == Tokens.Semicolon);
}
bool IsVarDecl () { return IdentAndCommaOrAsgnOrSColon(); }

/* True, if the comma is not a trailing one, *
 * like the last one in: a, b, c,            */
bool NotFinalComma () {
	int peek = Peek(1).kind;
	return la.kind == Tokens.Comma &&
	       peek != Tokens.CloseCurlyBrace && peek != Tokens.CloseSquareBracket;
}

/* True, if "void" is followed by "*" */
bool NotVoidPointer () {
	return la.kind == Tokens.Void && Peek(1).kind != Tokens.Times;
}

/* True, if "checked" or "unchecked" are followed by "{" */
bool UnCheckedAndLBrace () {
	return la.kind == Tokens.Checked || la.kind == Tokens.Unchecked &&
	       Peek(1).kind == Tokens.OpenCurlyBrace;
}

/* True, if "." is followed by an ident */
bool DotAndIdent () {
	return la.kind == Tokens.Dot && Peek(1).kind == Tokens.Identifier;
}

/* True, if ident is followed by ":" */
bool IdentAndColon () {
	return la.kind == Tokens.Identifier && Peek(1).kind == Tokens.Colon;
}

bool IsLabel () { return IdentAndColon(); }

/* True, if ident is followed by "(" */
bool IdentAndLPar () {
	return la.kind == Tokens.Identifier && Peek(1).kind == Tokens.OpenParenthesis;
}

/* True, if "catch" is followed by "(" */
bool CatchAndLPar () {
	return la.kind == Tokens.Catch && Peek(1).kind == Tokens.OpenParenthesis;
}
bool IsTypedCatch () { return CatchAndLPar(); }

/* True, if "[" is followed by the ident "assembly" */
bool IsGlobalAttrTarget () {
	Token pt = Peek(1);
	return la.kind == Tokens.OpenSquareBracket && 
	       pt.kind == Tokens.Identifier && pt.val == "assembly";
}

/* True, if "[" is followed by "," or "]" */
bool LBrackAndCommaOrRBrack () {
	int peek = Peek(1).kind;
	return la.kind == Tokens.OpenSquareBracket &&
	       (peek == Tokens.Comma || peek == Tokens.CloseSquareBracket);
}

bool IsDims () { return LBrackAndCommaOrRBrack(); }

/* True, if "[" is followed by "," or "]" */
/* or if the current token is "*"         */
bool TimesOrLBrackAndCommaOrRBrack () {
	return la.kind == Tokens.Times || LBrackAndCommaOrRBrack();
}
bool IsPointerOrDims () { return TimesOrLBrackAndCommaOrRBrack(); }
bool IsPointer () { return la.kind == Tokens.Times; }


bool SkipGeneric(ref Token pt)
{
	if (pt.kind == Tokens.LessThan) {
		int braces = 1;
		while (braces != 0) {
			pt = Peek();
			if (pt.kind == Tokens.GreaterThan) {
				--braces;
			} else if (pt.kind == Tokens.LessThan) {
				++braces;
			} else if (pt.kind == Tokens.Semicolon || pt.kind == Tokens.OpenCurlyBrace || pt.kind == Tokens.CloseCurlyBrace || pt.kind == Tokens.EOF) {
				return false;
			}
		}
		pt = Peek();
	}
	return true;
}
bool SkipQuestionMark(ref Token pt)
{
	if (pt.kind == Tokens.Question) {
		pt = Peek();
	}
	return true;
}

/* True, if lookahead is a primitive type keyword, or */
/* if it is a type declaration followed by an ident   */
bool IsLocalVarDecl () {
	if (IsYieldStatement()) {
		return false;
	}
	if ((Tokens.TypeKW[la.kind] && Peek(1).kind != Tokens.Dot) || la.kind == Tokens.Void) {
		return true;
	}
	
	StartPeek();
	Token pt = la ;
	string ignore;
	
	return IsQualident(ref pt, out ignore) && SkipGeneric(ref pt) && SkipQuestionMark(ref pt) && IsPointerOrDims(ref pt) && 
	       pt.kind == Tokens.Identifier;
}

/* True if lookahead is type parameters (<...>) followed by the specified token */
bool IsGenericFollowedBy(int token)
{
	Token t = la;
	if (t.kind != Tokens.LessThan) return false;
	StartPeek();
	return SkipGeneric(ref t) && t.kind == token;
}

/* True, if lookahead ident is "where" */
bool IdentIsWhere () {
	return la.kind == Tokens.Identifier && la.val == "where";
}

/* True, if lookahead ident is "get" */
bool IdentIsGet () {
	return la.kind == Tokens.Identifier && la.val == "get";
}

/* True, if lookahead ident is "set" */
bool IdentIsSet () {
	return la.kind == Tokens.Identifier && la.val == "set";
}

/* True, if lookahead ident is "add" */
bool IdentIsAdd () {
	return la.kind == Tokens.Identifier && la.val == "add";
}

/* True, if lookahead ident is "remove" */
bool IdentIsRemove () {
	return la.kind == Tokens.Identifier && la.val == "remove";
}

bool IsNotYieldStatement () {
	return !IsYieldStatement();
}
/* True, if lookahead ident is "yield" and than follows a break or return */
bool IsYieldStatement () {
	return la.kind == Tokens.Identifier && la.val == "yield" && (Peek(1).kind == Tokens.Return || Peek(1).kind == Tokens.Break);
}

/* True, if lookahead is a local attribute target specifier, *
 * i.e. one of "event", "return", "field", "method",         *
 *             "module", "param", "property", or "type"      */
bool IsLocalAttrTarget () {
	int cur = la.kind;
	string val = la.val;

	return (cur == Tokens.Event || cur == Tokens.Return ||
	        (cur == Tokens.Identifier &&
	         (val == "field" || val == "method"   || val == "module" ||
	          val == "param" || val == "property" || val == "type"))) &&
	       Peek(1).kind == Tokens.Colon;
}

bool IsShiftRight() 
{
	Token next = Peek(1);
	// TODO : Add col test (seems not to work, lexer bug...) :  && la.col == next.col - 1
	return (la.kind == Tokens.GreaterThan && next.kind == Tokens.GreaterThan);
}

bool IsTypeReferenceExpression(Expression expr)
{
	if (expr is TypeReferenceExpression) return ((TypeReferenceExpression)expr).TypeReference.GenericTypes.Count == 0;
	while (expr is FieldReferenceExpression) {
		expr = ((FieldReferenceExpression)expr).TargetObject;
	}
	return expr is IdentifierExpression;
}

TypeReferenceExpression GetTypeReferenceExpression(Expression expr, List<TypeReference> genericTypes)
{
	TypeReferenceExpression	tre = expr as TypeReferenceExpression;
	if (tre != null) {
		return new TypeReferenceExpression(new TypeReference(tre.TypeReference.Type, tre.TypeReference.PointerNestingLevel, tre.TypeReference.RankSpecifier, genericTypes));
	}
	StringBuilder b = new StringBuilder();
	WriteFullTypeName(b, expr);
	return new TypeReferenceExpression(new TypeReference(b.ToString(), 0, null, genericTypes));
}

void WriteFullTypeName(StringBuilder b, Expression expr)
{
	FieldReferenceExpression fre = expr as FieldReferenceExpression;
	if (fre != null) {
		WriteFullTypeName(b, fre.TargetObject);
		b.Append('.');
		b.Append(fre.FieldName);
	} else if (expr is IdentifierExpression) {
		b.Append(((IdentifierExpression)expr).Identifier);
	}
}

/*------------------------------------------------------------------------*
 *----- LEXER TOKEN LIST  ------------------------------------------------*
 *------------------------------------------------------------------------*/

/* START AUTOGENERATED TOKENS SECTION */


/*

*/

	void CS() {

#line  604 "cs.ATG" 
		compilationUnit = new CompilationUnit(); 
		while (la.kind == 120) {
			UsingDirective();
		}
		while (
#line  607 "cs.ATG" 
IsGlobalAttrTarget()) {
			GlobalAttributeSection();
		}
		while (StartOf(1)) {
			NamespaceMemberDecl();
		}
		Expect(0);
	}

	void UsingDirective() {

#line  614 "cs.ATG" 
		string qualident = null; TypeReference aliasedType = null;
		
		Expect(120);

#line  617 "cs.ATG" 
		Point startPos = t.Location; 
		Qualident(
#line  618 "cs.ATG" 
out qualident);
		if (la.kind == 3) {
			lexer.NextToken();
			NonArrayType(
#line  619 "cs.ATG" 
out aliasedType);
		}
		Expect(11);

#line  621 "cs.ATG" 
		if (qualident != null && qualident.Length > 0) {
		 INode node;
		 if (aliasedType != null) {
		     node = new UsingDeclaration(qualident, aliasedType);
		 } else {
		     node = new UsingDeclaration(qualident);
		 }
		 node.StartLocation = startPos;
		 node.EndLocation   = t.EndLocation;
		 compilationUnit.AddChild(node);
		}
		
	}

	void GlobalAttributeSection() {
		Expect(18);

#line  637 "cs.ATG" 
		Point startPos = t.Location; 
		Expect(1);

#line  638 "cs.ATG" 
		if (t.val != "assembly") Error("global attribute target specifier (\"assembly\") expected");
		string attributeTarget = t.val;
		List<ASTAttribute> attributes = new List<ASTAttribute>();
		ASTAttribute attribute;
		
		Expect(9);
		Attribute(
#line  643 "cs.ATG" 
out attribute);

#line  643 "cs.ATG" 
		attributes.Add(attribute); 
		while (
#line  644 "cs.ATG" 
NotFinalComma()) {
			Expect(14);
			Attribute(
#line  644 "cs.ATG" 
out attribute);

#line  644 "cs.ATG" 
			attributes.Add(attribute); 
		}
		if (la.kind == 14) {
			lexer.NextToken();
		}
		Expect(19);

#line  646 "cs.ATG" 
		AttributeSection section = new AttributeSection(attributeTarget, attributes);
		section.StartLocation = startPos;
		section.EndLocation = t.EndLocation;
		compilationUnit.AddChild(section);
		
	}

	void NamespaceMemberDecl() {

#line  730 "cs.ATG" 
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		Modifiers m = new Modifiers();
		string qualident;
		
		if (la.kind == 87) {
			lexer.NextToken();

#line  736 "cs.ATG" 
			Point startPos = t.Location; 
			Qualident(
#line  737 "cs.ATG" 
out qualident);

#line  737 "cs.ATG" 
			INode node =  new NamespaceDeclaration(qualident);
			node.StartLocation = startPos;
			compilationUnit.AddChild(node);
			compilationUnit.BlockStart(node);
			
			Expect(16);
			while (la.kind == 120) {
				UsingDirective();
			}
			while (StartOf(1)) {
				NamespaceMemberDecl();
			}
			Expect(17);
			if (la.kind == 11) {
				lexer.NextToken();
			}

#line  746 "cs.ATG" 
			node.EndLocation   = t.EndLocation;
			compilationUnit.BlockEnd();
			
		} else if (StartOf(2)) {
			while (la.kind == 18) {
				AttributeSection(
#line  750 "cs.ATG" 
out section);

#line  750 "cs.ATG" 
				attributes.Add(section); 
			}
			while (StartOf(3)) {
				TypeModifier(
#line  751 "cs.ATG" 
m);
			}
			TypeDecl(
#line  752 "cs.ATG" 
m, attributes);
		} else SynErr(126);
	}

	void Qualident(
#line  870 "cs.ATG" 
out string qualident) {
		Expect(1);

#line  872 "cs.ATG" 
		qualidentBuilder.Length = 0; qualidentBuilder.Append(t.val); 
		while (
#line  873 "cs.ATG" 
DotAndIdent()) {
			Expect(15);
			Expect(1);

#line  873 "cs.ATG" 
			qualidentBuilder.Append('.');
			qualidentBuilder.Append(t.val); 
			
		}

#line  876 "cs.ATG" 
		qualident = qualidentBuilder.ToString(); 
	}

	void NonArrayType(
#line  973 "cs.ATG" 
out TypeReference type) {

#line  975 "cs.ATG" 
		string name;
		int pointer = 0;
		type = null;
		
		if (la.kind == 1 || la.kind == 90 || la.kind == 107) {
			ClassType(
#line  980 "cs.ATG" 
out type);
		} else if (StartOf(4)) {
			SimpleType(
#line  981 "cs.ATG" 
out name);

#line  981 "cs.ATG" 
			type = new TypeReference(name); 
			if (la.kind == 12) {
				NullableQuestionMark(
#line  982 "cs.ATG" 
ref type);
			}
		} else if (la.kind == 122) {
			lexer.NextToken();
			Expect(6);

#line  983 "cs.ATG" 
			pointer = 1; type = new TypeReference("void"); 
		} else SynErr(127);
		while (
#line  986 "cs.ATG" 
IsPointer()) {
			Expect(6);

#line  987 "cs.ATG" 
			++pointer; 
		}

#line  989 "cs.ATG" 
		if (type != null) { type.PointerNestingLevel = pointer; } 
	}

	void Attribute(
#line  653 "cs.ATG" 
out ASTAttribute attribute) {

#line  654 "cs.ATG" 
		string qualident; 
		Qualident(
#line  656 "cs.ATG" 
out qualident);

#line  656 "cs.ATG" 
		List<Expression> positional = new List<Expression>();
		List<NamedArgumentExpression> named = new List<NamedArgumentExpression>();
		string name = qualident;
		
		if (la.kind == 20) {
			AttributeArguments(
#line  660 "cs.ATG" 
positional, named);
		}

#line  660 "cs.ATG" 
		attribute  = new ICSharpCode.NRefactory.Parser.AST.Attribute(name, positional, named);
	}

	void AttributeArguments(
#line  663 "cs.ATG" 
List<Expression> positional, List<NamedArgumentExpression> named) {

#line  665 "cs.ATG" 
		bool nameFound = false;
		string name = "";
		Expression expr;
		
		Expect(20);
		if (StartOf(5)) {
			if (
#line  673 "cs.ATG" 
IsAssignment()) {

#line  673 "cs.ATG" 
				nameFound = true; 
				lexer.NextToken();

#line  674 "cs.ATG" 
				name = t.val; 
				Expect(3);
			}
			Expr(
#line  676 "cs.ATG" 
out expr);

#line  676 "cs.ATG" 
			if (expr != null) {if(name == "") positional.Add(expr);
			else { named.Add(new NamedArgumentExpression(name, expr)); name = ""; }
			}
			
			while (la.kind == 14) {
				lexer.NextToken();
				if (
#line  684 "cs.ATG" 
IsAssignment()) {

#line  684 "cs.ATG" 
					nameFound = true; 
					Expect(1);

#line  685 "cs.ATG" 
					name = t.val; 
					Expect(3);
				} else if (StartOf(5)) {

#line  687 "cs.ATG" 
					if (nameFound) Error("no positional argument after named argument"); 
				} else SynErr(128);
				Expr(
#line  688 "cs.ATG" 
out expr);

#line  688 "cs.ATG" 
				if (expr != null) { if(name == "") positional.Add(expr);
				else { named.Add(new NamedArgumentExpression(name, expr)); name = ""; }
				}
				
			}
		}
		Expect(21);
	}

	void Expr(
#line  1953 "cs.ATG" 
out Expression expr) {

#line  1954 "cs.ATG" 
		expr = null; Expression expr1 = null, expr2 = null; AssignmentOperatorType op; 
		UnaryExpr(
#line  1956 "cs.ATG" 
out expr);
		if (StartOf(6)) {
			AssignmentOperator(
#line  1959 "cs.ATG" 
out op);
			Expr(
#line  1959 "cs.ATG" 
out expr1);

#line  1959 "cs.ATG" 
			expr = new AssignmentExpression(expr, op, expr1); 
		} else if (
#line  1960 "cs.ATG" 
la.kind == Tokens.GreaterThan && Peek(1).kind == Tokens.GreaterEqual) {
			AssignmentOperator(
#line  1961 "cs.ATG" 
out op);
			Expr(
#line  1961 "cs.ATG" 
out expr1);

#line  1961 "cs.ATG" 
			expr = new AssignmentExpression(expr, op, expr1); 
		} else if (StartOf(7)) {
			ConditionalOrExpr(
#line  1963 "cs.ATG" 
ref expr);
			if (la.kind == 13) {
				lexer.NextToken();
				Expr(
#line  1964 "cs.ATG" 
out expr1);

#line  1964 "cs.ATG" 
				expr = new BinaryOperatorExpression(expr, BinaryOperatorType.NullCoalescing, expr1); 
			}
			if (la.kind == 12) {
				lexer.NextToken();
				Expr(
#line  1965 "cs.ATG" 
out expr1);
				Expect(9);
				Expr(
#line  1965 "cs.ATG" 
out expr2);

#line  1965 "cs.ATG" 
				expr = new ConditionalExpression(expr, expr1, expr2);  
			}
		} else SynErr(129);
	}

	void AttributeSection(
#line  697 "cs.ATG" 
out AttributeSection section) {

#line  699 "cs.ATG" 
		string attributeTarget = "";
		List<ASTAttribute> attributes = new List<ASTAttribute>();
		ASTAttribute attribute;
		
		
		Expect(18);

#line  705 "cs.ATG" 
		Point startPos = t.Location; 
		if (
#line  706 "cs.ATG" 
IsLocalAttrTarget()) {
			if (la.kind == 68) {
				lexer.NextToken();

#line  707 "cs.ATG" 
				attributeTarget = "event";
			} else if (la.kind == 100) {
				lexer.NextToken();

#line  708 "cs.ATG" 
				attributeTarget = "return";
			} else {
				lexer.NextToken();

#line  709 "cs.ATG" 
				if (t.val != "field"    || t.val != "method" ||
				  t.val != "module"   || t.val != "param"  ||
				  t.val != "property" || t.val != "type")
				Error("attribute target specifier (event, return, field," +
				      "method, module, param, property, or type) expected");
				attributeTarget = t.val;
				
			}
			Expect(9);
		}
		Attribute(
#line  719 "cs.ATG" 
out attribute);

#line  719 "cs.ATG" 
		attributes.Add(attribute); 
		while (
#line  720 "cs.ATG" 
NotFinalComma()) {
			Expect(14);
			Attribute(
#line  720 "cs.ATG" 
out attribute);

#line  720 "cs.ATG" 
			attributes.Add(attribute); 
		}
		if (la.kind == 14) {
			lexer.NextToken();
		}
		Expect(19);

#line  722 "cs.ATG" 
		section = new AttributeSection(attributeTarget, attributes);
		section.StartLocation = startPos;
		section.EndLocation = t.EndLocation;
		
	}

	void TypeModifier(
#line  1059 "cs.ATG" 
Modifiers m) {
		switch (la.kind) {
		case 88: {
			lexer.NextToken();

#line  1061 "cs.ATG" 
			m.Add(Modifier.New, t.Location); 
			break;
		}
		case 97: {
			lexer.NextToken();

#line  1062 "cs.ATG" 
			m.Add(Modifier.Public, t.Location); 
			break;
		}
		case 96: {
			lexer.NextToken();

#line  1063 "cs.ATG" 
			m.Add(Modifier.Protected, t.Location); 
			break;
		}
		case 83: {
			lexer.NextToken();

#line  1064 "cs.ATG" 
			m.Add(Modifier.Internal, t.Location); 
			break;
		}
		case 95: {
			lexer.NextToken();

#line  1065 "cs.ATG" 
			m.Add(Modifier.Private, t.Location); 
			break;
		}
		case 118: {
			lexer.NextToken();

#line  1066 "cs.ATG" 
			m.Add(Modifier.Unsafe, t.Location); 
			break;
		}
		case 48: {
			lexer.NextToken();

#line  1067 "cs.ATG" 
			m.Add(Modifier.Abstract, t.Location); 
			break;
		}
		case 102: {
			lexer.NextToken();

#line  1068 "cs.ATG" 
			m.Add(Modifier.Sealed, t.Location); 
			break;
		}
		case 106: {
			lexer.NextToken();

#line  1069 "cs.ATG" 
			m.Add(Modifier.Static, t.Location); 
			break;
		}
		case 1: {
			lexer.NextToken();

#line  1070 "cs.ATG" 
			if (t.val == "partial") { m.Add(Modifier.Partial, t.Location); } 
			break;
		}
		default: SynErr(130); break;
		}
	}

	void TypeDecl(
#line  755 "cs.ATG" 
Modifiers m, List<AttributeSection> attributes) {

#line  757 "cs.ATG" 
		TypeReference type;
		List<TypeReference> names;
		List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
		string name;
		List<TemplateDefinition> templates;
		
		if (la.kind == 58) {

#line  763 "cs.ATG" 
			m.Check(Modifier.Classes); 
			lexer.NextToken();

#line  764 "cs.ATG" 
			TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
			templates = newType.Templates;
			compilationUnit.AddChild(newType);
			compilationUnit.BlockStart(newType);
			newType.StartLocation = m.GetDeclarationLocation(t.Location);
			
			newType.Type = Types.Class;
			
			Expect(1);

#line  772 "cs.ATG" 
			newType.Name = t.val; 
			if (la.kind == 23) {
				TypeParameterList(
#line  775 "cs.ATG" 
templates);
			}
			if (la.kind == 9) {
				ClassBase(
#line  777 "cs.ATG" 
out names);

#line  777 "cs.ATG" 
				newType.BaseTypes = names; 
			}
			while (
#line  780 "cs.ATG" 
IdentIsWhere()) {
				TypeParameterConstraintsClause(
#line  780 "cs.ATG" 
templates);
			}
			ClassBody();
			if (la.kind == 11) {
				lexer.NextToken();
			}

#line  783 "cs.ATG" 
			newType.EndLocation = t.Location; 
			compilationUnit.BlockEnd();
			
		} else if (StartOf(8)) {

#line  786 "cs.ATG" 
			m.Check(Modifier.StructsInterfacesEnumsDelegates); 
			if (la.kind == 108) {
				lexer.NextToken();

#line  787 "cs.ATG" 
				TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
				templates = newType.Templates;
				newType.StartLocation = m.GetDeclarationLocation(t.Location);
				compilationUnit.AddChild(newType);
				compilationUnit.BlockStart(newType);
				newType.Type = Types.Struct; 
				
				Expect(1);

#line  794 "cs.ATG" 
				newType.Name = t.val; 
				if (la.kind == 23) {
					TypeParameterList(
#line  797 "cs.ATG" 
templates);
				}
				if (la.kind == 9) {
					StructInterfaces(
#line  799 "cs.ATG" 
out names);

#line  799 "cs.ATG" 
					newType.BaseTypes = names; 
				}
				while (
#line  802 "cs.ATG" 
IdentIsWhere()) {
					TypeParameterConstraintsClause(
#line  802 "cs.ATG" 
templates);
				}
				StructBody();
				if (la.kind == 11) {
					lexer.NextToken();
				}

#line  806 "cs.ATG" 
				newType.EndLocation = t.Location; 
				compilationUnit.BlockEnd();
				
			} else if (la.kind == 82) {
				lexer.NextToken();

#line  810 "cs.ATG" 
				TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
				templates = newType.Templates;
				compilationUnit.AddChild(newType);
				compilationUnit.BlockStart(newType);
				newType.StartLocation = m.GetDeclarationLocation(t.Location);
				newType.Type = Types.Interface;
				
				Expect(1);

#line  817 "cs.ATG" 
				newType.Name = t.val; 
				if (la.kind == 23) {
					TypeParameterList(
#line  820 "cs.ATG" 
templates);
				}
				if (la.kind == 9) {
					InterfaceBase(
#line  822 "cs.ATG" 
out names);

#line  822 "cs.ATG" 
					newType.BaseTypes = names; 
				}
				while (
#line  825 "cs.ATG" 
IdentIsWhere()) {
					TypeParameterConstraintsClause(
#line  825 "cs.ATG" 
templates);
				}
				InterfaceBody();
				if (la.kind == 11) {
					lexer.NextToken();
				}

#line  828 "cs.ATG" 
				newType.EndLocation = t.Location; 
				compilationUnit.BlockEnd();
				
			} else if (la.kind == 67) {
				lexer.NextToken();

#line  832 "cs.ATG" 
				TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
				compilationUnit.AddChild(newType);
				compilationUnit.BlockStart(newType);
				newType.StartLocation = m.GetDeclarationLocation(t.Location);
				newType.Type = Types.Enum;
				
				Expect(1);

#line  838 "cs.ATG" 
				newType.Name = t.val; 
				if (la.kind == 9) {
					lexer.NextToken();
					IntegralType(
#line  839 "cs.ATG" 
out name);

#line  839 "cs.ATG" 
					newType.BaseTypes.Add(new TypeReference(name)); 
				}
				EnumBody();
				if (la.kind == 11) {
					lexer.NextToken();
				}

#line  842 "cs.ATG" 
				newType.EndLocation = t.Location; 
				compilationUnit.BlockEnd();
				
			} else {
				lexer.NextToken();

#line  846 "cs.ATG" 
				DelegateDeclaration delegateDeclr = new DelegateDeclaration(m.Modifier, attributes);
				templates = delegateDeclr.Templates;
				delegateDeclr.StartLocation = m.GetDeclarationLocation(t.Location);
				
				if (
#line  850 "cs.ATG" 
NotVoidPointer()) {
					Expect(122);

#line  850 "cs.ATG" 
					delegateDeclr.ReturnType = new TypeReference("void", 0, null); 
				} else if (StartOf(9)) {
					Type(
#line  851 "cs.ATG" 
out type);

#line  851 "cs.ATG" 
					delegateDeclr.ReturnType = type; 
				} else SynErr(131);
				Expect(1);

#line  853 "cs.ATG" 
				delegateDeclr.Name = t.val; 
				if (la.kind == 23) {
					TypeParameterList(
#line  856 "cs.ATG" 
templates);
				}
				Expect(20);
				if (StartOf(10)) {
					FormalParameterList(
#line  858 "cs.ATG" 
p);

#line  858 "cs.ATG" 
					delegateDeclr.Parameters = p; 
				}
				Expect(21);
				while (
#line  862 "cs.ATG" 
IdentIsWhere()) {
					TypeParameterConstraintsClause(
#line  862 "cs.ATG" 
templates);
				}
				Expect(11);

#line  864 "cs.ATG" 
				delegateDeclr.EndLocation = t.Location;
				compilationUnit.AddChild(delegateDeclr);
				
			}
		} else SynErr(132);
	}

	void TypeParameterList(
#line  2348 "cs.ATG" 
List<TemplateDefinition> templates) {

#line  2350 "cs.ATG" 
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		
		Expect(23);
		while (la.kind == 18) {
			AttributeSection(
#line  2354 "cs.ATG" 
out section);

#line  2354 "cs.ATG" 
			attributes.Add(section); 
		}
		Expect(1);

#line  2355 "cs.ATG" 
		templates.Add(new TemplateDefinition(t.val, attributes)); 
		while (la.kind == 14) {
			lexer.NextToken();
			while (la.kind == 18) {
				AttributeSection(
#line  2356 "cs.ATG" 
out section);

#line  2356 "cs.ATG" 
				attributes.Add(section); 
			}
			Expect(1);

#line  2357 "cs.ATG" 
			templates.Add(new TemplateDefinition(t.val, attributes)); 
		}
		Expect(22);
	}

	void ClassBase(
#line  879 "cs.ATG" 
out List<TypeReference> names) {

#line  881 "cs.ATG" 
		TypeReference typeRef;
		names = new List<TypeReference>();
		
		Expect(9);
		ClassType(
#line  885 "cs.ATG" 
out typeRef);

#line  885 "cs.ATG" 
		if (typeRef != null) { names.Add(typeRef); } 
		while (la.kind == 14) {
			lexer.NextToken();
			TypeName(
#line  886 "cs.ATG" 
out typeRef);

#line  886 "cs.ATG" 
			if (typeRef != null) { names.Add(typeRef); } 
		}
	}

	void TypeParameterConstraintsClause(
#line  2361 "cs.ATG" 
List<TemplateDefinition> templates) {

#line  2362 "cs.ATG" 
		string name = ""; TypeReference type; 
		Expect(1);

#line  2364 "cs.ATG" 
		if (t.val != "where") Error("where expected"); 
		Expect(1);

#line  2365 "cs.ATG" 
		name = t.val; 
		Expect(9);
		TypeParameterConstraintsClauseBase(
#line  2367 "cs.ATG" 
out type);

#line  2368 "cs.ATG" 
		TemplateDefinition td = null;
		foreach (TemplateDefinition d in templates) {
			if (d.Name == name) {
				td = d;
				break;
			}
		}
		if ( td != null) { td.Bases.Add(type); }
		
		while (la.kind == 14) {
			lexer.NextToken();
			TypeParameterConstraintsClauseBase(
#line  2377 "cs.ATG" 
out type);

#line  2378 "cs.ATG" 
			td = null;
			foreach (TemplateDefinition d in templates) {
				if (d.Name == name) {
					td = d;
					break;
				}
			}
			if ( td != null) { td.Bases.Add(type); }
			
		}
	}

	void ClassBody() {

#line  890 "cs.ATG" 
		AttributeSection section; 
		Expect(16);
		while (StartOf(11)) {

#line  893 "cs.ATG" 
			List<AttributeSection> attributes = new List<AttributeSection>();
			Modifiers m = new Modifiers();
			
			while (la.kind == 18) {
				AttributeSection(
#line  896 "cs.ATG" 
out section);

#line  896 "cs.ATG" 
				attributes.Add(section); 
			}
			while (StartOf(12)) {
				MemberModifier(
#line  897 "cs.ATG" 
m);
			}
			ClassMemberDecl(
#line  898 "cs.ATG" 
m, attributes);
		}
		Expect(17);
	}

	void StructInterfaces(
#line  903 "cs.ATG" 
out List<TypeReference> names) {

#line  905 "cs.ATG" 
		TypeReference typeRef;
		names = new List<TypeReference>();
		
		Expect(9);
		TypeName(
#line  909 "cs.ATG" 
out typeRef);

#line  909 "cs.ATG" 
		if (typeRef != null) { names.Add(typeRef); } 
		while (la.kind == 14) {
			lexer.NextToken();
			TypeName(
#line  910 "cs.ATG" 
out typeRef);

#line  910 "cs.ATG" 
			if (typeRef != null) { names.Add(typeRef); } 
		}
	}

	void StructBody() {

#line  914 "cs.ATG" 
		AttributeSection section; 
		Expect(16);
		while (StartOf(13)) {

#line  917 "cs.ATG" 
			List<AttributeSection> attributes = new List<AttributeSection>();
			Modifiers m = new Modifiers();
			
			while (la.kind == 18) {
				AttributeSection(
#line  920 "cs.ATG" 
out section);

#line  920 "cs.ATG" 
				attributes.Add(section); 
			}
			while (StartOf(12)) {
				MemberModifier(
#line  921 "cs.ATG" 
m);
			}
			StructMemberDecl(
#line  922 "cs.ATG" 
m, attributes);
		}
		Expect(17);
	}

	void InterfaceBase(
#line  927 "cs.ATG" 
out List<TypeReference> names) {

#line  929 "cs.ATG" 
		TypeReference typeRef;
		names = new List<TypeReference>();
		
		Expect(9);
		TypeName(
#line  933 "cs.ATG" 
out typeRef);

#line  933 "cs.ATG" 
		if (typeRef != null) { names.Add(typeRef); } 
		while (la.kind == 14) {
			lexer.NextToken();
			TypeName(
#line  934 "cs.ATG" 
out typeRef);

#line  934 "cs.ATG" 
			if (typeRef != null) { names.Add(typeRef); } 
		}
	}

	void InterfaceBody() {
		Expect(16);
		while (StartOf(14)) {
			InterfaceMemberDecl();
		}
		Expect(17);
	}

	void IntegralType(
#line  1081 "cs.ATG" 
out string name) {

#line  1081 "cs.ATG" 
		name = ""; 
		switch (la.kind) {
		case 101: {
			lexer.NextToken();

#line  1083 "cs.ATG" 
			name = "sbyte"; 
			break;
		}
		case 53: {
			lexer.NextToken();

#line  1084 "cs.ATG" 
			name = "byte"; 
			break;
		}
		case 103: {
			lexer.NextToken();

#line  1085 "cs.ATG" 
			name = "short"; 
			break;
		}
		case 119: {
			lexer.NextToken();

#line  1086 "cs.ATG" 
			name = "ushort"; 
			break;
		}
		case 81: {
			lexer.NextToken();

#line  1087 "cs.ATG" 
			name = "int"; 
			break;
		}
		case 115: {
			lexer.NextToken();

#line  1088 "cs.ATG" 
			name = "uint"; 
			break;
		}
		case 86: {
			lexer.NextToken();

#line  1089 "cs.ATG" 
			name = "long"; 
			break;
		}
		case 116: {
			lexer.NextToken();

#line  1090 "cs.ATG" 
			name = "ulong"; 
			break;
		}
		case 56: {
			lexer.NextToken();

#line  1091 "cs.ATG" 
			name = "char"; 
			break;
		}
		default: SynErr(133); break;
		}
	}

	void EnumBody() {

#line  940 "cs.ATG" 
		FieldDeclaration f; 
		Expect(16);
		if (la.kind == 1 || la.kind == 18) {
			EnumMemberDecl(
#line  942 "cs.ATG" 
out f);

#line  942 "cs.ATG" 
			compilationUnit.AddChild(f); 
			while (
#line  943 "cs.ATG" 
NotFinalComma()) {
				Expect(14);
				EnumMemberDecl(
#line  943 "cs.ATG" 
out f);

#line  943 "cs.ATG" 
				compilationUnit.AddChild(f); 
			}
			if (la.kind == 14) {
				lexer.NextToken();
			}
		}
		Expect(17);
	}

	void Type(
#line  948 "cs.ATG" 
out TypeReference type) {

#line  950 "cs.ATG" 
		string name;
		int pointer = 0;
		type = null;
		
		if (la.kind == 1 || la.kind == 90 || la.kind == 107) {
			ClassType(
#line  955 "cs.ATG" 
out type);
		} else if (StartOf(4)) {
			SimpleType(
#line  956 "cs.ATG" 
out name);

#line  956 "cs.ATG" 
			type = new TypeReference(name); 
			if (la.kind == 12) {
				NullableQuestionMark(
#line  957 "cs.ATG" 
ref type);
			}
		} else if (la.kind == 122) {
			lexer.NextToken();
			Expect(6);

#line  958 "cs.ATG" 
			pointer = 1; type = new TypeReference("void"); 
		} else SynErr(134);

#line  959 "cs.ATG" 
		List<int> r = new List<int>(); 
		while (
#line  961 "cs.ATG" 
IsPointerOrDims()) {

#line  961 "cs.ATG" 
			int i = 0; 
			if (la.kind == 6) {
				lexer.NextToken();

#line  962 "cs.ATG" 
				++pointer; 
			} else if (la.kind == 18) {
				lexer.NextToken();
				while (la.kind == 14) {
					lexer.NextToken();

#line  963 "cs.ATG" 
					++i; 
				}
				Expect(19);

#line  963 "cs.ATG" 
				r.Add(i); 
			} else SynErr(135);
		}

#line  966 "cs.ATG" 
		if (type != null) {
		type.RankSpecifier = r.ToArray();
		type.PointerNestingLevel = pointer;
		  }
		
	}

	void FormalParameterList(
#line  1003 "cs.ATG" 
List<ParameterDeclarationExpression> parameter) {

#line  1006 "cs.ATG" 
		ParameterDeclarationExpression p;
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		
		while (la.kind == 18) {
			AttributeSection(
#line  1011 "cs.ATG" 
out section);

#line  1011 "cs.ATG" 
			attributes.Add(section); 
		}
		if (StartOf(15)) {
			FixedParameter(
#line  1013 "cs.ATG" 
out p);

#line  1013 "cs.ATG" 
			bool paramsFound = false;
			p.Attributes = attributes;
			parameter.Add(p);
			
			while (la.kind == 14) {
				lexer.NextToken();

#line  1018 "cs.ATG" 
				attributes = new List<AttributeSection>(); if (paramsFound) Error("params array must be at end of parameter list"); 
				while (la.kind == 18) {
					AttributeSection(
#line  1019 "cs.ATG" 
out section);

#line  1019 "cs.ATG" 
					attributes.Add(section); 
				}
				if (StartOf(15)) {
					FixedParameter(
#line  1021 "cs.ATG" 
out p);

#line  1021 "cs.ATG" 
					p.Attributes = attributes; parameter.Add(p); 
				} else if (la.kind == 94) {
					ParameterArray(
#line  1022 "cs.ATG" 
out p);

#line  1022 "cs.ATG" 
					paramsFound = true; p.Attributes = attributes; parameter.Add(p); 
				} else SynErr(136);
			}
		} else if (la.kind == 94) {
			ParameterArray(
#line  1025 "cs.ATG" 
out p);

#line  1025 "cs.ATG" 
			p.Attributes = attributes; parameter.Add(p); 
		} else SynErr(137);
	}

	void ClassType(
#line  1073 "cs.ATG" 
out TypeReference typeRef) {

#line  1074 "cs.ATG" 
		TypeReference r; typeRef = null; 
		if (la.kind == 1) {
			TypeName(
#line  1076 "cs.ATG" 
out r);

#line  1076 "cs.ATG" 
			typeRef = r; 
		} else if (la.kind == 90) {
			lexer.NextToken();

#line  1077 "cs.ATG" 
			typeRef = new TypeReference("object"); 
		} else if (la.kind == 107) {
			lexer.NextToken();

#line  1078 "cs.ATG" 
			typeRef = new TypeReference("string"); 
		} else SynErr(138);
	}

	void TypeName(
#line  2302 "cs.ATG" 
out TypeReference typeRef) {

#line  2303 "cs.ATG" 
		List<TypeReference> typeArguments = null;
		string alias = null;
		string qualident;
		
		if (
#line  2308 "cs.ATG" 
la.kind == Tokens.Identifier && Peek(1).kind == Tokens.DoubleColon) {
			lexer.NextToken();

#line  2309 "cs.ATG" 
			alias = t.val; 
			Expect(10);
		}
		Qualident(
#line  2312 "cs.ATG" 
out qualident);
		if (la.kind == 23) {
			TypeArgumentList(
#line  2313 "cs.ATG" 
out typeArguments);
		}

#line  2315 "cs.ATG" 
		if (alias == null) {
		typeRef = new TypeReference(qualident, typeArguments);
		} else if (alias == "global") {
			typeRef = new TypeReference(qualident, typeArguments);
			typeRef.IsGlobal = true;
		} else {
			typeRef = new TypeReference(alias + "." + qualident, typeArguments);
		}
		
		if (la.kind == 12) {
			NullableQuestionMark(
#line  2324 "cs.ATG" 
ref typeRef);
		}
	}

	void MemberModifier(
#line  1094 "cs.ATG" 
Modifiers m) {
		switch (la.kind) {
		case 48: {
			lexer.NextToken();

#line  1096 "cs.ATG" 
			m.Add(Modifier.Abstract, t.Location); 
			break;
		}
		case 70: {
			lexer.NextToken();

#line  1097 "cs.ATG" 
			m.Add(Modifier.Extern, t.Location); 
			break;
		}
		case 83: {
			lexer.NextToken();

#line  1098 "cs.ATG" 
			m.Add(Modifier.Internal, t.Location); 
			break;
		}
		case 88: {
			lexer.NextToken();

#line  1099 "cs.ATG" 
			m.Add(Modifier.New, t.Location); 
			break;
		}
		case 93: {
			lexer.NextToken();

#line  1100 "cs.ATG" 
			m.Add(Modifier.Override, t.Location); 
			break;
		}
		case 95: {
			lexer.NextToken();

#line  1101 "cs.ATG" 
			m.Add(Modifier.Private, t.Location); 
			break;
		}
		case 96: {
			lexer.NextToken();

#line  1102 "cs.ATG" 
			m.Add(Modifier.Protected, t.Location); 
			break;
		}
		case 97: {
			lexer.NextToken();

#line  1103 "cs.ATG" 
			m.Add(Modifier.Public, t.Location); 
			break;
		}
		case 98: {
			lexer.NextToken();

#line  1104 "cs.ATG" 
			m.Add(Modifier.Readonly, t.Location); 
			break;
		}
		case 102: {
			lexer.NextToken();

#line  1105 "cs.ATG" 
			m.Add(Modifier.Sealed, t.Location); 
			break;
		}
		case 106: {
			lexer.NextToken();

#line  1106 "cs.ATG" 
			m.Add(Modifier.Static, t.Location); 
			break;
		}
		case 118: {
			lexer.NextToken();

#line  1107 "cs.ATG" 
			m.Add(Modifier.Unsafe, t.Location); 
			break;
		}
		case 121: {
			lexer.NextToken();

#line  1108 "cs.ATG" 
			m.Add(Modifier.Virtual, t.Location); 
			break;
		}
		case 123: {
			lexer.NextToken();

#line  1109 "cs.ATG" 
			m.Add(Modifier.Volatile, t.Location); 
			break;
		}
		default: SynErr(139); break;
		}
	}

	void ClassMemberDecl(
#line  1350 "cs.ATG" 
Modifiers m, List<AttributeSection> attributes) {

#line  1351 "cs.ATG" 
		Statement stmt = null; 
		if (StartOf(16)) {
			StructMemberDecl(
#line  1353 "cs.ATG" 
m, attributes);
		} else if (la.kind == 27) {

#line  1354 "cs.ATG" 
			m.Check(Modifier.Destructors); Point startPos = t.Location; 
			lexer.NextToken();
			Expect(1);

#line  1355 "cs.ATG" 
			DestructorDeclaration d = new DestructorDeclaration(t.val, m.Modifier, attributes); 
			d.Modifier = m.Modifier;
			d.StartLocation = m.GetDeclarationLocation(startPos);
			
			Expect(20);
			Expect(21);

#line  1359 "cs.ATG" 
			d.EndLocation = t.EndLocation; 
			if (la.kind == 16) {
				Block(
#line  1359 "cs.ATG" 
out stmt);
			} else if (la.kind == 11) {
				lexer.NextToken();
			} else SynErr(140);

#line  1360 "cs.ATG" 
			d.Body = (BlockStatement)stmt;
			compilationUnit.AddChild(d);
			
		} else SynErr(141);
	}

	void StructMemberDecl(
#line  1112 "cs.ATG" 
Modifiers m, List<AttributeSection> attributes) {

#line  1114 "cs.ATG" 
		string qualident = null;
		TypeReference type;
		Expression expr;
		List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
		Statement stmt = null;
		List<VariableDeclaration> variableDeclarators = new List<VariableDeclaration>();
		List<TemplateDefinition> templates = new List<TemplateDefinition>();
		
		if (la.kind == 59) {

#line  1123 "cs.ATG" 
			m.Check(Modifier.Constants); 
			lexer.NextToken();

#line  1124 "cs.ATG" 
			Point startPos = t.Location; 
			Type(
#line  1125 "cs.ATG" 
out type);
			Expect(1);

#line  1125 "cs.ATG" 
			FieldDeclaration fd = new FieldDeclaration(attributes, type, m.Modifier | Modifier.Const);
			fd.StartLocation = m.GetDeclarationLocation(startPos);
			VariableDeclaration f = new VariableDeclaration(t.val);
			fd.Fields.Add(f);
			
			Expect(3);
			Expr(
#line  1130 "cs.ATG" 
out expr);

#line  1130 "cs.ATG" 
			f.Initializer = expr; 
			while (la.kind == 14) {
				lexer.NextToken();
				Expect(1);

#line  1131 "cs.ATG" 
				f = new VariableDeclaration(t.val);
				fd.Fields.Add(f);
				
				Expect(3);
				Expr(
#line  1134 "cs.ATG" 
out expr);

#line  1134 "cs.ATG" 
				f.Initializer = expr; 
			}
			Expect(11);

#line  1135 "cs.ATG" 
			fd.EndLocation = t.EndLocation; compilationUnit.AddChild(fd); 
		} else if (
#line  1138 "cs.ATG" 
NotVoidPointer()) {

#line  1138 "cs.ATG" 
			m.Check(Modifier.PropertysEventsMethods); 
			Expect(122);

#line  1139 "cs.ATG" 
			Point startPos = t.Location; 
			Qualident(
#line  1140 "cs.ATG" 
out qualident);
			if (la.kind == 23) {
				TypeParameterList(
#line  1142 "cs.ATG" 
templates);
			}
			Expect(20);
			if (StartOf(10)) {
				FormalParameterList(
#line  1145 "cs.ATG" 
p);
			}
			Expect(21);

#line  1145 "cs.ATG" 
			MethodDeclaration methodDeclaration = new MethodDeclaration(qualident,
			                                                           m.Modifier,
			                                                           new TypeReference("void"),
			                                                           p,
			                                                           attributes);
			methodDeclaration.StartLocation = m.GetDeclarationLocation(startPos);
			methodDeclaration.EndLocation   = t.EndLocation;
			methodDeclaration.Templates = templates;
			compilationUnit.AddChild(methodDeclaration);
			compilationUnit.BlockStart(methodDeclaration);
			
			while (
#line  1158 "cs.ATG" 
IdentIsWhere()) {
				TypeParameterConstraintsClause(
#line  1158 "cs.ATG" 
templates);
			}
			if (la.kind == 16) {
				Block(
#line  1160 "cs.ATG" 
out stmt);
			} else if (la.kind == 11) {
				lexer.NextToken();
			} else SynErr(142);

#line  1160 "cs.ATG" 
			compilationUnit.BlockEnd();
			methodDeclaration.Body  = (BlockStatement)stmt;
			
		} else if (la.kind == 68) {

#line  1164 "cs.ATG" 
			m.Check(Modifier.PropertysEventsMethods); 
			lexer.NextToken();

#line  1165 "cs.ATG" 
			EventDeclaration eventDecl = new EventDeclaration(m.Modifier, attributes);
			eventDecl.StartLocation = t.Location;
			compilationUnit.AddChild(eventDecl);
			compilationUnit.BlockStart(eventDecl);
			EventAddRegion addBlock = null;
			EventRemoveRegion removeBlock = null;
			
			Type(
#line  1172 "cs.ATG" 
out type);

#line  1172 "cs.ATG" 
			eventDecl.TypeReference = type; 
			if (
#line  1174 "cs.ATG" 
IsVarDecl()) {
				VariableDeclarator(
#line  1174 "cs.ATG" 
variableDeclarators);
				while (la.kind == 14) {
					lexer.NextToken();
					VariableDeclarator(
#line  1175 "cs.ATG" 
variableDeclarators);
				}
				Expect(11);

#line  1175 "cs.ATG" 
				eventDecl.VariableDeclarators = variableDeclarators; eventDecl.EndLocation = t.EndLocation;  
			} else if (la.kind == 1) {
				Qualident(
#line  1176 "cs.ATG" 
out qualident);

#line  1176 "cs.ATG" 
				eventDecl.Name = qualident; eventDecl.EndLocation = t.EndLocation;  
				Expect(16);

#line  1177 "cs.ATG" 
				eventDecl.BodyStart = t.Location; 
				EventAccessorDecls(
#line  1178 "cs.ATG" 
out addBlock, out removeBlock);
				Expect(17);

#line  1179 "cs.ATG" 
				eventDecl.BodyEnd   = t.EndLocation; 
			} else SynErr(143);

#line  1180 "cs.ATG" 
			compilationUnit.BlockEnd();
			
			eventDecl.AddRegion = addBlock;
			eventDecl.RemoveRegion = removeBlock;
			
		} else if (
#line  1187 "cs.ATG" 
IdentAndLPar()) {

#line  1187 "cs.ATG" 
			m.Check(Modifier.Constructors | Modifier.StaticConstructors); 
			Expect(1);

#line  1188 "cs.ATG" 
			string name = t.val; Point startPos = t.Location; 
			Expect(20);
			if (StartOf(10)) {

#line  1188 "cs.ATG" 
				m.Check(Modifier.Constructors); 
				FormalParameterList(
#line  1189 "cs.ATG" 
p);
			}
			Expect(21);

#line  1191 "cs.ATG" 
			ConstructorInitializer init = null;  
			if (la.kind == 9) {

#line  1192 "cs.ATG" 
				m.Check(Modifier.Constructors); 
				ConstructorInitializer(
#line  1193 "cs.ATG" 
out init);
			}

#line  1195 "cs.ATG" 
			ConstructorDeclaration cd = new ConstructorDeclaration(name, m.Modifier, p, init, attributes); 
			cd.StartLocation = startPos;
			cd.EndLocation   = t.EndLocation;
			
			if (la.kind == 16) {
				Block(
#line  1200 "cs.ATG" 
out stmt);
			} else if (la.kind == 11) {
				lexer.NextToken();
			} else SynErr(144);

#line  1200 "cs.ATG" 
			cd.Body = (BlockStatement)stmt; compilationUnit.AddChild(cd); 
		} else if (la.kind == 69 || la.kind == 79) {

#line  1203 "cs.ATG" 
			m.Check(Modifier.Operators);
			if (m.isNone) Error("at least one modifier must be set"); 
			bool isImplicit = true;
			Point startPos = Point.Empty;
			
			if (la.kind == 79) {
				lexer.NextToken();

#line  1208 "cs.ATG" 
				startPos = t.Location; 
			} else {
				lexer.NextToken();

#line  1208 "cs.ATG" 
				isImplicit = false; startPos = t.Location; 
			}
			Expect(91);
			Type(
#line  1209 "cs.ATG" 
out type);

#line  1209 "cs.ATG" 
			TypeReference operatorType = type; 
			Expect(20);
			Type(
#line  1210 "cs.ATG" 
out type);
			Expect(1);

#line  1210 "cs.ATG" 
			string varName = t.val; 
			Expect(21);

#line  1211 "cs.ATG" 
			Point endPos = t.Location; 
			if (la.kind == 16) {
				Block(
#line  1212 "cs.ATG" 
out stmt);
			} else if (la.kind == 11) {
				lexer.NextToken();

#line  1212 "cs.ATG" 
				stmt = null; 
			} else SynErr(145);

#line  1215 "cs.ATG" 
			List<ParameterDeclarationExpression> parameters = new List<ParameterDeclarationExpression>();
			parameters.Add(new ParameterDeclarationExpression(type, varName));
			OperatorDeclaration operatorDeclaration = new OperatorDeclaration(m.Modifier, 
			                                                                  attributes, 
			                                                                  parameters, 
			                                                                  operatorType,
			                                                                  isImplicit ? ConversionType.Implicit : ConversionType.Explicit
			                                                                  );
			operatorDeclaration.Body = (BlockStatement)stmt;
			operatorDeclaration.StartLocation = m.GetDeclarationLocation(startPos);
			operatorDeclaration.EndLocation = endPos;
			compilationUnit.AddChild(operatorDeclaration);
			
		} else if (StartOf(17)) {
			TypeDecl(
#line  1230 "cs.ATG" 
m, attributes);
		} else if (StartOf(9)) {
			Type(
#line  1231 "cs.ATG" 
out type);

#line  1231 "cs.ATG" 
			Point startPos = t.Location;  
			if (la.kind == 91) {

#line  1233 "cs.ATG" 
				OverloadableOperatorType op;
				m.Check(Modifier.Operators);
				if (m.isNone) Error("at least one modifier must be set");
				
				lexer.NextToken();
				OverloadableOperator(
#line  1237 "cs.ATG" 
out op);

#line  1237 "cs.ATG" 
				TypeReference firstType, secondType = null; string secondName = null; 
				Expect(20);
				Type(
#line  1238 "cs.ATG" 
out firstType);
				Expect(1);

#line  1238 "cs.ATG" 
				string firstName = t.val; 
				if (la.kind == 14) {
					lexer.NextToken();
					Type(
#line  1239 "cs.ATG" 
out secondType);
					Expect(1);

#line  1239 "cs.ATG" 
					secondName = t.val; 
				} else if (la.kind == 21) {
				} else SynErr(146);

#line  1247 "cs.ATG" 
				Point endPos = t.Location; 
				Expect(21);
				if (la.kind == 16) {
					Block(
#line  1248 "cs.ATG" 
out stmt);
				} else if (la.kind == 11) {
					lexer.NextToken();
				} else SynErr(147);

#line  1250 "cs.ATG" 
				List<ParameterDeclarationExpression> parameters = new List<ParameterDeclarationExpression>();
				parameters.Add(new ParameterDeclarationExpression(firstType, firstName));
				if (secondType != null) {
					parameters.Add(new ParameterDeclarationExpression(secondType, secondName));
				}
				OperatorDeclaration operatorDeclaration = new OperatorDeclaration(m.Modifier,
				                                                                  attributes,
				                                                                  parameters,
				                                                                  type,
				                                                                  op);
				operatorDeclaration.Body = (BlockStatement)stmt;
				operatorDeclaration.StartLocation = m.GetDeclarationLocation(startPos);
				operatorDeclaration.EndLocation = endPos;
				compilationUnit.AddChild(operatorDeclaration);
				
			} else if (
#line  1267 "cs.ATG" 
IsVarDecl()) {

#line  1267 "cs.ATG" 
				m.Check(Modifier.Fields); 
				FieldDeclaration fd = new FieldDeclaration(attributes, type, m.Modifier);
				fd.StartLocation = m.GetDeclarationLocation(startPos); 
				
				VariableDeclarator(
#line  1271 "cs.ATG" 
variableDeclarators);
				while (la.kind == 14) {
					lexer.NextToken();
					VariableDeclarator(
#line  1272 "cs.ATG" 
variableDeclarators);
				}
				Expect(11);

#line  1273 "cs.ATG" 
				fd.EndLocation = t.EndLocation; fd.Fields = variableDeclarators; compilationUnit.AddChild(fd); 
			} else if (la.kind == 110) {

#line  1276 "cs.ATG" 
				m.Check(Modifier.Indexers); 
				lexer.NextToken();
				Expect(18);
				FormalParameterList(
#line  1277 "cs.ATG" 
p);
				Expect(19);

#line  1277 "cs.ATG" 
				Point endLocation = t.EndLocation; 
				Expect(16);

#line  1278 "cs.ATG" 
				IndexerDeclaration indexer = new IndexerDeclaration(type, p, m.Modifier, attributes);
				indexer.StartLocation = startPos;
				indexer.EndLocation   = endLocation;
				indexer.BodyStart     = t.Location;
				PropertyGetRegion getRegion;
				PropertySetRegion setRegion;
				
				AccessorDecls(
#line  1285 "cs.ATG" 
out getRegion, out setRegion);
				Expect(17);

#line  1286 "cs.ATG" 
				indexer.BodyEnd    = t.EndLocation;
				indexer.GetRegion = getRegion;
				indexer.SetRegion = setRegion;
				compilationUnit.AddChild(indexer);
				
			} else if (la.kind == 1) {
				Qualident(
#line  1291 "cs.ATG" 
out qualident);

#line  1291 "cs.ATG" 
				Point qualIdentEndLocation = t.EndLocation; 
				if (la.kind == 16 || la.kind == 20 || la.kind == 23) {
					if (la.kind == 20 || la.kind == 23) {

#line  1294 "cs.ATG" 
						m.Check(Modifier.PropertysEventsMethods); 
						if (la.kind == 23) {
							TypeParameterList(
#line  1296 "cs.ATG" 
templates);
						}
						Expect(20);
						if (StartOf(10)) {
							FormalParameterList(
#line  1297 "cs.ATG" 
p);
						}
						Expect(21);

#line  1298 "cs.ATG" 
						MethodDeclaration methodDeclaration = new MethodDeclaration(qualident,
						                                                           m.Modifier, 
						                                                           type, 
						                                                           p, 
						                                                           attributes);
						methodDeclaration.StartLocation = m.GetDeclarationLocation(startPos);
						methodDeclaration.EndLocation   = t.EndLocation;
						methodDeclaration.Templates = templates;
						compilationUnit.AddChild(methodDeclaration);
						                                      
						while (
#line  1308 "cs.ATG" 
IdentIsWhere()) {
							TypeParameterConstraintsClause(
#line  1308 "cs.ATG" 
templates);
						}
						if (la.kind == 16) {
							Block(
#line  1309 "cs.ATG" 
out stmt);
						} else if (la.kind == 11) {
							lexer.NextToken();
						} else SynErr(148);

#line  1309 "cs.ATG" 
						methodDeclaration.Body  = (BlockStatement)stmt; 
					} else {
						lexer.NextToken();

#line  1312 "cs.ATG" 
						PropertyDeclaration pDecl = new PropertyDeclaration(qualident, type, m.Modifier, attributes); 
						pDecl.StartLocation = m.GetDeclarationLocation(startPos);
						pDecl.EndLocation   = qualIdentEndLocation;
						pDecl.BodyStart   = t.Location;
						PropertyGetRegion getRegion;
						PropertySetRegion setRegion;
						
						AccessorDecls(
#line  1319 "cs.ATG" 
out getRegion, out setRegion);
						Expect(17);

#line  1321 "cs.ATG" 
						pDecl.GetRegion = getRegion;
						pDecl.SetRegion = setRegion;
						pDecl.BodyEnd = t.EndLocation;
						compilationUnit.AddChild(pDecl);
						
					}
				} else if (la.kind == 15) {

#line  1329 "cs.ATG" 
					m.Check(Modifier.Indexers); 
					lexer.NextToken();
					Expect(110);
					Expect(18);
					FormalParameterList(
#line  1330 "cs.ATG" 
p);
					Expect(19);

#line  1331 "cs.ATG" 
					IndexerDeclaration indexer = new IndexerDeclaration(type, p, m.Modifier, attributes);
					indexer.StartLocation = m.GetDeclarationLocation(startPos);
					indexer.EndLocation   = t.EndLocation;
					indexer.NamespaceName = qualident;
					PropertyGetRegion getRegion;
					PropertySetRegion setRegion;
					
					Expect(16);

#line  1338 "cs.ATG" 
					Point bodyStart = t.Location; 
					AccessorDecls(
#line  1339 "cs.ATG" 
out getRegion, out setRegion);
					Expect(17);

#line  1340 "cs.ATG" 
					indexer.BodyStart = bodyStart;
					indexer.BodyEnd   = t.EndLocation;
					indexer.GetRegion = getRegion;
					indexer.SetRegion = setRegion;
					compilationUnit.AddChild(indexer);
					
				} else SynErr(149);
			} else SynErr(150);
		} else SynErr(151);
	}

	void InterfaceMemberDecl() {

#line  1367 "cs.ATG" 
		TypeReference type;
		
		AttributeSection section;
		Modifier mod = Modifier.None;
		List<AttributeSection> attributes = new List<AttributeSection>();
		List<ParameterDeclarationExpression> parameters = new List<ParameterDeclarationExpression>();
		string name;
		PropertyGetRegion getBlock;
		PropertySetRegion setBlock;
		Point startLocation = new Point(-1, -1);
		List<TemplateDefinition> templates = new List<TemplateDefinition>();
		
		while (la.kind == 18) {
			AttributeSection(
#line  1380 "cs.ATG" 
out section);

#line  1380 "cs.ATG" 
			attributes.Add(section); 
		}
		if (la.kind == 88) {
			lexer.NextToken();

#line  1381 "cs.ATG" 
			mod = Modifier.New; startLocation = t.Location; 
		}
		if (
#line  1384 "cs.ATG" 
NotVoidPointer()) {
			Expect(122);

#line  1384 "cs.ATG" 
			if (startLocation.X == -1) startLocation = t.Location; 
			Expect(1);

#line  1384 "cs.ATG" 
			name = t.val; 
			if (la.kind == 23) {
				TypeParameterList(
#line  1385 "cs.ATG" 
templates);
			}
			Expect(20);
			if (StartOf(10)) {
				FormalParameterList(
#line  1386 "cs.ATG" 
parameters);
			}
			Expect(21);
			while (
#line  1387 "cs.ATG" 
IdentIsWhere()) {
				TypeParameterConstraintsClause(
#line  1387 "cs.ATG" 
templates);
			}
			Expect(11);

#line  1389 "cs.ATG" 
			MethodDeclaration md = new MethodDeclaration(name, mod, new TypeReference("void"), parameters, attributes);
			md.StartLocation = startLocation;
			md.EndLocation = t.EndLocation;
			md.Templates = templates;
			compilationUnit.AddChild(md);
			
		} else if (StartOf(18)) {
			if (StartOf(9)) {
				Type(
#line  1396 "cs.ATG" 
out type);

#line  1396 "cs.ATG" 
				if (startLocation.X == -1) startLocation = t.Location; 
				if (la.kind == 1) {
					lexer.NextToken();

#line  1398 "cs.ATG" 
					name = t.val; Point qualIdentEndLocation = t.EndLocation; 
					if (la.kind == 20 || la.kind == 23) {
						if (la.kind == 23) {
							TypeParameterList(
#line  1402 "cs.ATG" 
templates);
						}
						Expect(20);
						if (StartOf(10)) {
							FormalParameterList(
#line  1403 "cs.ATG" 
parameters);
						}
						Expect(21);
						while (
#line  1405 "cs.ATG" 
IdentIsWhere()) {
							TypeParameterConstraintsClause(
#line  1405 "cs.ATG" 
templates);
						}
						Expect(11);

#line  1406 "cs.ATG" 
						MethodDeclaration md = new MethodDeclaration(name, mod, type, parameters, attributes);
						md.StartLocation = startLocation;
						md.EndLocation = t.EndLocation;
						md.Templates = templates;
						compilationUnit.AddChild(md);
						
					} else if (la.kind == 16) {

#line  1413 "cs.ATG" 
						PropertyDeclaration pd = new PropertyDeclaration(name, type, mod, attributes); compilationUnit.AddChild(pd); 
						lexer.NextToken();

#line  1414 "cs.ATG" 
						Point bodyStart = t.Location;
						InterfaceAccessors(
#line  1414 "cs.ATG" 
out getBlock, out setBlock);
						Expect(17);

#line  1414 "cs.ATG" 
						pd.GetRegion = getBlock; pd.SetRegion = setBlock; pd.StartLocation = startLocation; pd.EndLocation = qualIdentEndLocation; pd.BodyStart = bodyStart; pd.BodyEnd = t.EndLocation; 
					} else SynErr(152);
				} else if (la.kind == 110) {
					lexer.NextToken();
					Expect(18);
					FormalParameterList(
#line  1417 "cs.ATG" 
parameters);
					Expect(19);

#line  1417 "cs.ATG" 
					Point bracketEndLocation = t.EndLocation; 

#line  1417 "cs.ATG" 
					IndexerDeclaration id = new IndexerDeclaration(type, parameters, mod, attributes); compilationUnit.AddChild(id); 
					Expect(16);

#line  1418 "cs.ATG" 
					Point bodyStart = t.Location;
					InterfaceAccessors(
#line  1418 "cs.ATG" 
out getBlock, out setBlock);
					Expect(17);

#line  1418 "cs.ATG" 
					id.GetRegion = getBlock; id.SetRegion = setBlock; id.StartLocation = startLocation;  id.EndLocation = bracketEndLocation; id.BodyStart = bodyStart; id.BodyEnd = t.EndLocation;
				} else SynErr(153);
			} else {
				lexer.NextToken();

#line  1421 "cs.ATG" 
				if (startLocation.X == -1) startLocation = t.Location; 
				Type(
#line  1421 "cs.ATG" 
out type);
				Expect(1);

#line  1421 "cs.ATG" 
				EventDeclaration ed = new EventDeclaration(type, t.val, mod, attributes);
				compilationUnit.AddChild(ed);
				
				Expect(11);

#line  1424 "cs.ATG" 
				ed.StartLocation = startLocation; ed.EndLocation = t.EndLocation; 
			}
		} else SynErr(154);
	}

	void EnumMemberDecl(
#line  1429 "cs.ATG" 
out FieldDeclaration f) {

#line  1431 "cs.ATG" 
		Expression expr = null;
		List<AttributeSection> attributes = new List<AttributeSection>();
		AttributeSection section = null;
		VariableDeclaration varDecl = null;
		
		while (la.kind == 18) {
			AttributeSection(
#line  1437 "cs.ATG" 
out section);

#line  1437 "cs.ATG" 
			attributes.Add(section); 
		}
		Expect(1);

#line  1438 "cs.ATG" 
		f = new FieldDeclaration(attributes);
		varDecl         = new VariableDeclaration(t.val);
		f.Fields.Add(varDecl);
		f.StartLocation = t.Location;
		
		if (la.kind == 3) {
			lexer.NextToken();
			Expr(
#line  1443 "cs.ATG" 
out expr);

#line  1443 "cs.ATG" 
			varDecl.Initializer = expr; 
		}
	}

	void SimpleType(
#line  992 "cs.ATG" 
out string name) {

#line  993 "cs.ATG" 
		name = String.Empty; 
		if (StartOf(19)) {
			IntegralType(
#line  995 "cs.ATG" 
out name);
		} else if (la.kind == 74) {
			lexer.NextToken();

#line  996 "cs.ATG" 
			name = "float"; 
		} else if (la.kind == 65) {
			lexer.NextToken();

#line  997 "cs.ATG" 
			name = "double"; 
		} else if (la.kind == 61) {
			lexer.NextToken();

#line  998 "cs.ATG" 
			name = "decimal"; 
		} else if (la.kind == 51) {
			lexer.NextToken();

#line  999 "cs.ATG" 
			name = "bool"; 
		} else SynErr(155);
	}

	void NullableQuestionMark(
#line  2327 "cs.ATG" 
ref TypeReference typeRef) {

#line  2328 "cs.ATG" 
		List<TypeReference> typeArguments = new List<TypeReference>(1); 
		Expect(12);

#line  2332 "cs.ATG" 
		if (typeRef != null) typeArguments.Add(typeRef);
		typeRef = new TypeReference("System.Nullable", typeArguments);
		
	}

	void FixedParameter(
#line  1029 "cs.ATG" 
out ParameterDeclarationExpression p) {

#line  1031 "cs.ATG" 
		TypeReference type;
		ParamModifier mod = ParamModifier.In;
		System.Drawing.Point start = t.Location;
		
		if (la.kind == 92 || la.kind == 99) {
			if (la.kind == 99) {
				lexer.NextToken();

#line  1037 "cs.ATG" 
				mod = ParamModifier.Ref; 
			} else {
				lexer.NextToken();

#line  1038 "cs.ATG" 
				mod = ParamModifier.Out; 
			}
		}
		Type(
#line  1040 "cs.ATG" 
out type);
		Expect(1);

#line  1040 "cs.ATG" 
		p = new ParameterDeclarationExpression(type, t.val, mod); p.StartLocation = start; p.EndLocation = t.Location; 
	}

	void ParameterArray(
#line  1043 "cs.ATG" 
out ParameterDeclarationExpression p) {

#line  1044 "cs.ATG" 
		TypeReference type; 
		Expect(94);
		Type(
#line  1046 "cs.ATG" 
out type);
		Expect(1);

#line  1046 "cs.ATG" 
		p = new ParameterDeclarationExpression(type, t.val, ParamModifier.Params); 
	}

	void AccessorModifiers(
#line  1049 "cs.ATG" 
out Modifiers m) {

#line  1050 "cs.ATG" 
		m = new Modifiers(); 
		if (la.kind == 95) {
			lexer.NextToken();

#line  1052 "cs.ATG" 
			m.Add(Modifier.Private, t.Location); 
		} else if (la.kind == 96) {
			lexer.NextToken();

#line  1053 "cs.ATG" 
			m.Add(Modifier.Protected, t.Location); 
			if (la.kind == 83) {
				lexer.NextToken();

#line  1054 "cs.ATG" 
				m.Add(Modifier.Internal, t.Location); 
			}
		} else if (la.kind == 83) {
			lexer.NextToken();

#line  1055 "cs.ATG" 
			m.Add(Modifier.Internal, t.Location); 
			if (la.kind == 96) {
				lexer.NextToken();

#line  1056 "cs.ATG" 
				m.Add(Modifier.Protected, t.Location); 
			}
		} else SynErr(156);
	}

	void Block(
#line  1568 "cs.ATG" 
out Statement stmt) {
		Expect(16);

#line  1570 "cs.ATG" 
		BlockStatement blockStmt = new BlockStatement();
		blockStmt.StartLocation = t.EndLocation;
		compilationUnit.BlockStart(blockStmt);
		if (!parseMethodContents) lexer.SkipCurrentBlock();
		
		while (StartOf(20)) {
			Statement();
		}
		Expect(17);

#line  1577 "cs.ATG" 
		stmt = blockStmt;
		blockStmt.EndLocation = t.EndLocation;
		compilationUnit.BlockEnd();
		
	}

	void VariableDeclarator(
#line  1561 "cs.ATG" 
List<VariableDeclaration> fieldDeclaration) {

#line  1562 "cs.ATG" 
		Expression expr = null; 
		Expect(1);

#line  1564 "cs.ATG" 
		VariableDeclaration f = new VariableDeclaration(t.val); 
		if (la.kind == 3) {
			lexer.NextToken();
			VariableInitializer(
#line  1565 "cs.ATG" 
out expr);

#line  1565 "cs.ATG" 
			f.Initializer = expr; 
		}

#line  1565 "cs.ATG" 
		fieldDeclaration.Add(f); 
	}

	void EventAccessorDecls(
#line  1503 "cs.ATG" 
out EventAddRegion addBlock, out EventRemoveRegion removeBlock) {

#line  1504 "cs.ATG" 
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		Statement stmt;
		addBlock = null;
		removeBlock = null;
		
		while (la.kind == 18) {
			AttributeSection(
#line  1511 "cs.ATG" 
out section);

#line  1511 "cs.ATG" 
			attributes.Add(section); 
		}
		if (
#line  1513 "cs.ATG" 
IdentIsAdd()) {

#line  1513 "cs.ATG" 
			addBlock = new EventAddRegion(attributes); 
			AddAccessorDecl(
#line  1514 "cs.ATG" 
out stmt);

#line  1514 "cs.ATG" 
			attributes = new List<AttributeSection>(); addBlock.Block = (BlockStatement)stmt; 
			while (la.kind == 18) {
				AttributeSection(
#line  1515 "cs.ATG" 
out section);

#line  1515 "cs.ATG" 
				attributes.Add(section); 
			}
			RemoveAccessorDecl(
#line  1516 "cs.ATG" 
out stmt);

#line  1516 "cs.ATG" 
			removeBlock = new EventRemoveRegion(attributes); removeBlock.Block = (BlockStatement)stmt; 
		} else if (
#line  1517 "cs.ATG" 
IdentIsRemove()) {
			RemoveAccessorDecl(
#line  1518 "cs.ATG" 
out stmt);

#line  1518 "cs.ATG" 
			removeBlock = new EventRemoveRegion(attributes); removeBlock.Block = (BlockStatement)stmt; attributes = new List<AttributeSection>(); 
			while (la.kind == 18) {
				AttributeSection(
#line  1519 "cs.ATG" 
out section);

#line  1519 "cs.ATG" 
				attributes.Add(section); 
			}
			AddAccessorDecl(
#line  1520 "cs.ATG" 
out stmt);

#line  1520 "cs.ATG" 
			addBlock = new EventAddRegion(attributes); addBlock.Block = (BlockStatement)stmt; 
		} else if (la.kind == 1) {
			lexer.NextToken();

#line  1521 "cs.ATG" 
			Error("add or remove accessor declaration expected"); 
		} else SynErr(157);
	}

	void ConstructorInitializer(
#line  1599 "cs.ATG" 
out ConstructorInitializer ci) {

#line  1600 "cs.ATG" 
		Expression expr; ci = new ConstructorInitializer(); 
		Expect(9);
		if (la.kind == 50) {
			lexer.NextToken();

#line  1604 "cs.ATG" 
			ci.ConstructorInitializerType = ConstructorInitializerType.Base; 
		} else if (la.kind == 110) {
			lexer.NextToken();

#line  1605 "cs.ATG" 
			ci.ConstructorInitializerType = ConstructorInitializerType.This; 
		} else SynErr(158);
		Expect(20);
		if (StartOf(21)) {
			Argument(
#line  1608 "cs.ATG" 
out expr);

#line  1608 "cs.ATG" 
			if (expr != null) { ci.Arguments.Add(expr); } 
			while (la.kind == 14) {
				lexer.NextToken();
				Argument(
#line  1608 "cs.ATG" 
out expr);

#line  1608 "cs.ATG" 
				if (expr != null) { ci.Arguments.Add(expr); } 
			}
		}
		Expect(21);
	}

	void OverloadableOperator(
#line  1620 "cs.ATG" 
out OverloadableOperatorType op) {

#line  1621 "cs.ATG" 
		op = OverloadableOperatorType.None; 
		switch (la.kind) {
		case 4: {
			lexer.NextToken();

#line  1623 "cs.ATG" 
			op = OverloadableOperatorType.Add; 
			break;
		}
		case 5: {
			lexer.NextToken();

#line  1624 "cs.ATG" 
			op = OverloadableOperatorType.Subtract; 
			break;
		}
		case 24: {
			lexer.NextToken();

#line  1626 "cs.ATG" 
			op = OverloadableOperatorType.Not; 
			break;
		}
		case 27: {
			lexer.NextToken();

#line  1627 "cs.ATG" 
			op = OverloadableOperatorType.BitNot; 
			break;
		}
		case 31: {
			lexer.NextToken();

#line  1629 "cs.ATG" 
			op = OverloadableOperatorType.Increment; 
			break;
		}
		case 32: {
			lexer.NextToken();

#line  1630 "cs.ATG" 
			op = OverloadableOperatorType.Decrement; 
			break;
		}
		case 112: {
			lexer.NextToken();

#line  1632 "cs.ATG" 
			op = OverloadableOperatorType.True; 
			break;
		}
		case 71: {
			lexer.NextToken();

#line  1633 "cs.ATG" 
			op = OverloadableOperatorType.False; 
			break;
		}
		case 6: {
			lexer.NextToken();

#line  1635 "cs.ATG" 
			op = OverloadableOperatorType.Multiply; 
			break;
		}
		case 7: {
			lexer.NextToken();

#line  1636 "cs.ATG" 
			op = OverloadableOperatorType.Divide; 
			break;
		}
		case 8: {
			lexer.NextToken();

#line  1637 "cs.ATG" 
			op = OverloadableOperatorType.Modulus; 
			break;
		}
		case 28: {
			lexer.NextToken();

#line  1639 "cs.ATG" 
			op = OverloadableOperatorType.BitwiseAnd; 
			break;
		}
		case 29: {
			lexer.NextToken();

#line  1640 "cs.ATG" 
			op = OverloadableOperatorType.BitwiseOr; 
			break;
		}
		case 30: {
			lexer.NextToken();

#line  1641 "cs.ATG" 
			op = OverloadableOperatorType.ExclusiveOr; 
			break;
		}
		case 37: {
			lexer.NextToken();

#line  1643 "cs.ATG" 
			op = OverloadableOperatorType.ShiftLeft; 
			break;
		}
		case 33: {
			lexer.NextToken();

#line  1644 "cs.ATG" 
			op = OverloadableOperatorType.Equality; 
			break;
		}
		case 34: {
			lexer.NextToken();

#line  1645 "cs.ATG" 
			op = OverloadableOperatorType.InEquality; 
			break;
		}
		case 23: {
			lexer.NextToken();

#line  1646 "cs.ATG" 
			op = OverloadableOperatorType.LessThan; 
			break;
		}
		case 35: {
			lexer.NextToken();

#line  1647 "cs.ATG" 
			op = OverloadableOperatorType.GreaterThanOrEqual; 
			break;
		}
		case 36: {
			lexer.NextToken();

#line  1648 "cs.ATG" 
			op = OverloadableOperatorType.LessThanOrEqual; 
			break;
		}
		case 22: {
			lexer.NextToken();

#line  1649 "cs.ATG" 
			op = OverloadableOperatorType.GreaterThan; 
			if (la.kind == 22) {
				lexer.NextToken();

#line  1649 "cs.ATG" 
				op = OverloadableOperatorType.ShiftRight; 
			}
			break;
		}
		default: SynErr(159); break;
		}
	}

	void AccessorDecls(
#line  1447 "cs.ATG" 
out PropertyGetRegion getBlock, out PropertySetRegion setBlock) {

#line  1449 "cs.ATG" 
		List<AttributeSection> attributes = new List<AttributeSection>(); 
		AttributeSection section;
		getBlock = null;
		setBlock = null; 
		Modifiers modifiers = null;
		
		while (la.kind == 18) {
			AttributeSection(
#line  1456 "cs.ATG" 
out section);

#line  1456 "cs.ATG" 
			attributes.Add(section); 
		}
		if (la.kind == 83 || la.kind == 95 || la.kind == 96) {
			AccessorModifiers(
#line  1457 "cs.ATG" 
out modifiers);
		}
		if (
#line  1459 "cs.ATG" 
IdentIsGet()) {
			GetAccessorDecl(
#line  1460 "cs.ATG" 
out getBlock, attributes);

#line  1461 "cs.ATG" 
			if (modifiers != null) {getBlock.Modifier = modifiers.Modifier; } 
			if (StartOf(22)) {

#line  1462 "cs.ATG" 
				attributes = new List<AttributeSection>(); modifiers = null; 
				while (la.kind == 18) {
					AttributeSection(
#line  1463 "cs.ATG" 
out section);

#line  1463 "cs.ATG" 
					attributes.Add(section); 
				}
				if (la.kind == 83 || la.kind == 95 || la.kind == 96) {
					AccessorModifiers(
#line  1464 "cs.ATG" 
out modifiers);
				}
				SetAccessorDecl(
#line  1465 "cs.ATG" 
out setBlock, attributes);

#line  1466 "cs.ATG" 
				if (modifiers != null) {setBlock.Modifier = modifiers.Modifier; } 
			}
		} else if (
#line  1468 "cs.ATG" 
IdentIsSet()) {
			SetAccessorDecl(
#line  1469 "cs.ATG" 
out setBlock, attributes);

#line  1470 "cs.ATG" 
			if (modifiers != null) {setBlock.Modifier = modifiers.Modifier; } 
			if (StartOf(22)) {

#line  1471 "cs.ATG" 
				attributes = new List<AttributeSection>(); modifiers = null; 
				while (la.kind == 18) {
					AttributeSection(
#line  1472 "cs.ATG" 
out section);

#line  1472 "cs.ATG" 
					attributes.Add(section); 
				}
				if (la.kind == 83 || la.kind == 95 || la.kind == 96) {
					AccessorModifiers(
#line  1473 "cs.ATG" 
out modifiers);
				}
				GetAccessorDecl(
#line  1474 "cs.ATG" 
out getBlock, attributes);

#line  1475 "cs.ATG" 
				if (modifiers != null) {getBlock.Modifier = modifiers.Modifier; } 
			}
		} else if (la.kind == 1) {
			lexer.NextToken();

#line  1477 "cs.ATG" 
			Error("get or set accessor declaration expected"); 
		} else SynErr(160);
	}

	void InterfaceAccessors(
#line  1525 "cs.ATG" 
out PropertyGetRegion getBlock, out PropertySetRegion setBlock) {

#line  1527 "cs.ATG" 
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		getBlock = null; setBlock = null;
		PropertyGetSetRegion lastBlock = null;
		
		while (la.kind == 18) {
			AttributeSection(
#line  1533 "cs.ATG" 
out section);

#line  1533 "cs.ATG" 
			attributes.Add(section); 
		}

#line  1534 "cs.ATG" 
		Point startLocation = la.Location; 
		if (
#line  1536 "cs.ATG" 
IdentIsGet()) {
			Expect(1);

#line  1536 "cs.ATG" 
			getBlock = new PropertyGetRegion(null, attributes); 
		} else if (
#line  1537 "cs.ATG" 
IdentIsSet()) {
			Expect(1);

#line  1537 "cs.ATG" 
			setBlock = new PropertySetRegion(null, attributes); 
		} else if (la.kind == 1) {
			lexer.NextToken();

#line  1538 "cs.ATG" 
			Error("set or get expected"); 
		} else SynErr(161);
		Expect(11);

#line  1541 "cs.ATG" 
		if (getBlock != null) { getBlock.StartLocation = startLocation; getBlock.EndLocation = t.EndLocation; }
		if (setBlock != null) { setBlock.StartLocation = startLocation; setBlock.EndLocation = t.EndLocation; }
		attributes = new List<AttributeSection>(); 
		if (la.kind == 1 || la.kind == 18) {
			while (la.kind == 18) {
				AttributeSection(
#line  1545 "cs.ATG" 
out section);

#line  1545 "cs.ATG" 
				attributes.Add(section); 
			}

#line  1546 "cs.ATG" 
			startLocation = la.Location; 
			if (
#line  1548 "cs.ATG" 
IdentIsGet()) {
				Expect(1);

#line  1548 "cs.ATG" 
				if (getBlock != null) Error("get already declared");
				else { getBlock = new PropertyGetRegion(null, attributes); lastBlock = getBlock; }
				
			} else if (
#line  1551 "cs.ATG" 
IdentIsSet()) {
				Expect(1);

#line  1551 "cs.ATG" 
				if (setBlock != null) Error("set already declared");
				else { setBlock = new PropertySetRegion(null, attributes); lastBlock = setBlock; }
				
			} else if (la.kind == 1) {
				lexer.NextToken();

#line  1554 "cs.ATG" 
				Error("set or get expected"); 
			} else SynErr(162);
			Expect(11);

#line  1557 "cs.ATG" 
			if (lastBlock != null) { lastBlock.StartLocation = startLocation; lastBlock.EndLocation = t.EndLocation; } 
		}
	}

	void GetAccessorDecl(
#line  1481 "cs.ATG" 
out PropertyGetRegion getBlock, List<AttributeSection> attributes) {

#line  1482 "cs.ATG" 
		Statement stmt = null; 
		Expect(1);

#line  1485 "cs.ATG" 
		if (t.val != "get") Error("get expected"); 

#line  1486 "cs.ATG" 
		Point startLocation = t.Location; 
		if (la.kind == 16) {
			Block(
#line  1487 "cs.ATG" 
out stmt);
		} else if (la.kind == 11) {
			lexer.NextToken();
		} else SynErr(163);

#line  1488 "cs.ATG" 
		getBlock = new PropertyGetRegion((BlockStatement)stmt, attributes); 

#line  1489 "cs.ATG" 
		getBlock.StartLocation = startLocation; getBlock.EndLocation = t.EndLocation; 
	}

	void SetAccessorDecl(
#line  1492 "cs.ATG" 
out PropertySetRegion setBlock, List<AttributeSection> attributes) {

#line  1493 "cs.ATG" 
		Statement stmt = null; 
		Expect(1);

#line  1496 "cs.ATG" 
		if (t.val != "set") Error("set expected"); 

#line  1497 "cs.ATG" 
		Point startLocation = t.Location; 
		if (la.kind == 16) {
			Block(
#line  1498 "cs.ATG" 
out stmt);
		} else if (la.kind == 11) {
			lexer.NextToken();
		} else SynErr(164);

#line  1499 "cs.ATG" 
		setBlock = new PropertySetRegion((BlockStatement)stmt, attributes); 

#line  1500 "cs.ATG" 
		setBlock.StartLocation = startLocation; setBlock.EndLocation = t.EndLocation; 
	}

	void AddAccessorDecl(
#line  1583 "cs.ATG" 
out Statement stmt) {

#line  1584 "cs.ATG" 
		stmt = null;
		Expect(1);

#line  1587 "cs.ATG" 
		if (t.val != "add") Error("add expected"); 
		Block(
#line  1588 "cs.ATG" 
out stmt);
	}

	void RemoveAccessorDecl(
#line  1591 "cs.ATG" 
out Statement stmt) {

#line  1592 "cs.ATG" 
		stmt = null;
		Expect(1);

#line  1595 "cs.ATG" 
		if (t.val != "remove") Error("remove expected"); 
		Block(
#line  1596 "cs.ATG" 
out stmt);
	}

	void VariableInitializer(
#line  1612 "cs.ATG" 
out Expression initializerExpression) {

#line  1613 "cs.ATG" 
		TypeReference type = null; Expression expr = null; initializerExpression = null; 
		if (StartOf(5)) {
			Expr(
#line  1615 "cs.ATG" 
out initializerExpression);
		} else if (la.kind == 16) {
			ArrayInitializer(
#line  1616 "cs.ATG" 
out initializerExpression);
		} else if (la.kind == 105) {
			lexer.NextToken();
			Type(
#line  1617 "cs.ATG" 
out type);
			Expect(18);
			Expr(
#line  1617 "cs.ATG" 
out expr);
			Expect(19);

#line  1617 "cs.ATG" 
			initializerExpression = new StackAllocExpression(type, expr); 
		} else SynErr(165);
	}

	void Statement() {

#line  1722 "cs.ATG" 
		TypeReference type;
		Expression expr;
		Statement stmt = null;
		Point startPos = la.Location;
		
		if (
#line  1730 "cs.ATG" 
IsLabel()) {
			Expect(1);

#line  1730 "cs.ATG" 
			compilationUnit.AddChild(new LabelStatement(t.val)); 
			Expect(9);
			Statement();
		} else if (la.kind == 59) {
			lexer.NextToken();
			Type(
#line  1733 "cs.ATG" 
out type);

#line  1733 "cs.ATG" 
			LocalVariableDeclaration var = new LocalVariableDeclaration(type, Modifier.Const); string ident = null; var.StartLocation = t.Location; 
			Expect(1);

#line  1734 "cs.ATG" 
			ident = t.val; 
			Expect(3);
			Expr(
#line  1735 "cs.ATG" 
out expr);

#line  1735 "cs.ATG" 
			var.Variables.Add(new VariableDeclaration(ident, expr)); 
			while (la.kind == 14) {
				lexer.NextToken();
				Expect(1);

#line  1736 "cs.ATG" 
				ident = t.val; 
				Expect(3);
				Expr(
#line  1736 "cs.ATG" 
out expr);

#line  1736 "cs.ATG" 
				var.Variables.Add(new VariableDeclaration(ident, expr)); 
			}
			Expect(11);

#line  1737 "cs.ATG" 
			compilationUnit.AddChild(var); 
		} else if (
#line  1739 "cs.ATG" 
IsLocalVarDecl()) {
			LocalVariableDecl(
#line  1739 "cs.ATG" 
out stmt);
			Expect(11);

#line  1739 "cs.ATG" 
			compilationUnit.AddChild(stmt); 
		} else if (StartOf(23)) {
			EmbeddedStatement(
#line  1740 "cs.ATG" 
out stmt);

#line  1740 "cs.ATG" 
			compilationUnit.AddChild(stmt); 
		} else SynErr(166);

#line  1746 "cs.ATG" 
		if (stmt != null) {
		stmt.StartLocation = startPos;
		stmt.EndLocation = t.EndLocation;
		}
		
	}

	void Argument(
#line  1652 "cs.ATG" 
out Expression argumentexpr) {

#line  1654 "cs.ATG" 
		Expression expr;
		FieldDirection fd = FieldDirection.None;
		
		if (la.kind == 92 || la.kind == 99) {
			if (la.kind == 99) {
				lexer.NextToken();

#line  1659 "cs.ATG" 
				fd = FieldDirection.Ref; 
			} else {
				lexer.NextToken();

#line  1660 "cs.ATG" 
				fd = FieldDirection.Out; 
			}
		}
		Expr(
#line  1662 "cs.ATG" 
out expr);

#line  1662 "cs.ATG" 
		argumentexpr = fd != FieldDirection.None ? argumentexpr = new DirectionExpression(fd, expr) : expr; 
	}

	void ArrayInitializer(
#line  1682 "cs.ATG" 
out Expression outExpr) {

#line  1684 "cs.ATG" 
		Expression expr = null;
		ArrayInitializerExpression initializer = new ArrayInitializerExpression();
		
		Expect(16);
		if (StartOf(24)) {
			VariableInitializer(
#line  1689 "cs.ATG" 
out expr);

#line  1689 "cs.ATG" 
			if (expr != null) { initializer.CreateExpressions.Add(expr); } 
			while (
#line  1689 "cs.ATG" 
NotFinalComma()) {
				Expect(14);
				VariableInitializer(
#line  1689 "cs.ATG" 
out expr);

#line  1689 "cs.ATG" 
				if (expr != null) { initializer.CreateExpressions.Add(expr); } 
			}
			if (la.kind == 14) {
				lexer.NextToken();
			}
		}
		Expect(17);

#line  1690 "cs.ATG" 
		outExpr = initializer; 
	}

	void AssignmentOperator(
#line  1665 "cs.ATG" 
out AssignmentOperatorType op) {

#line  1666 "cs.ATG" 
		op = AssignmentOperatorType.None; 
		if (la.kind == 3) {
			lexer.NextToken();

#line  1668 "cs.ATG" 
			op = AssignmentOperatorType.Assign; 
		} else if (la.kind == 38) {
			lexer.NextToken();

#line  1669 "cs.ATG" 
			op = AssignmentOperatorType.Add; 
		} else if (la.kind == 39) {
			lexer.NextToken();

#line  1670 "cs.ATG" 
			op = AssignmentOperatorType.Subtract; 
		} else if (la.kind == 40) {
			lexer.NextToken();

#line  1671 "cs.ATG" 
			op = AssignmentOperatorType.Multiply; 
		} else if (la.kind == 41) {
			lexer.NextToken();

#line  1672 "cs.ATG" 
			op = AssignmentOperatorType.Divide; 
		} else if (la.kind == 42) {
			lexer.NextToken();

#line  1673 "cs.ATG" 
			op = AssignmentOperatorType.Modulus; 
		} else if (la.kind == 43) {
			lexer.NextToken();

#line  1674 "cs.ATG" 
			op = AssignmentOperatorType.BitwiseAnd; 
		} else if (la.kind == 44) {
			lexer.NextToken();

#line  1675 "cs.ATG" 
			op = AssignmentOperatorType.BitwiseOr; 
		} else if (la.kind == 45) {
			lexer.NextToken();

#line  1676 "cs.ATG" 
			op = AssignmentOperatorType.ExclusiveOr; 
		} else if (la.kind == 46) {
			lexer.NextToken();

#line  1677 "cs.ATG" 
			op = AssignmentOperatorType.ShiftLeft; 
		} else if (
#line  1678 "cs.ATG" 
la.kind == Tokens.GreaterThan && Peek(1).kind == Tokens.GreaterEqual) {
			Expect(22);
			Expect(35);

#line  1679 "cs.ATG" 
			op = AssignmentOperatorType.ShiftRight; 
		} else SynErr(167);
	}

	void LocalVariableDecl(
#line  1693 "cs.ATG" 
out Statement stmt) {

#line  1695 "cs.ATG" 
		TypeReference type;
		VariableDeclaration      var = null;
		LocalVariableDeclaration localVariableDeclaration; 
		
		Type(
#line  1700 "cs.ATG" 
out type);

#line  1700 "cs.ATG" 
		localVariableDeclaration = new LocalVariableDeclaration(type); localVariableDeclaration.StartLocation = t.Location; 
		LocalVariableDeclarator(
#line  1701 "cs.ATG" 
out var);

#line  1701 "cs.ATG" 
		localVariableDeclaration.Variables.Add(var); 
		while (la.kind == 14) {
			lexer.NextToken();
			LocalVariableDeclarator(
#line  1702 "cs.ATG" 
out var);

#line  1702 "cs.ATG" 
			localVariableDeclaration.Variables.Add(var); 
		}

#line  1703 "cs.ATG" 
		stmt = localVariableDeclaration; 
	}

	void LocalVariableDeclarator(
#line  1706 "cs.ATG" 
out VariableDeclaration var) {

#line  1707 "cs.ATG" 
		Expression expr = null; 
		Expect(1);

#line  1710 "cs.ATG" 
		var = new VariableDeclaration(t.val); 
		if (la.kind == 3) {
			lexer.NextToken();
			VariableInitializer(
#line  1710 "cs.ATG" 
out expr);

#line  1710 "cs.ATG" 
			var.Initializer = expr; 
		}
	}

	void EmbeddedStatement(
#line  1753 "cs.ATG" 
out Statement statement) {

#line  1755 "cs.ATG" 
		TypeReference type = null;
		Expression expr = null;
		Statement embeddedStatement = null;
		statement = null;
		
		if (la.kind == 16) {
			Block(
#line  1761 "cs.ATG" 
out statement);
		} else if (la.kind == 11) {
			lexer.NextToken();

#line  1763 "cs.ATG" 
			statement = new EmptyStatement(); 
		} else if (
#line  1765 "cs.ATG" 
UnCheckedAndLBrace()) {

#line  1765 "cs.ATG" 
			Statement block; bool isChecked = true; 
			if (la.kind == 57) {
				lexer.NextToken();
			} else if (la.kind == 117) {
				lexer.NextToken();

#line  1766 "cs.ATG" 
				isChecked = false;
			} else SynErr(168);
			Block(
#line  1767 "cs.ATG" 
out block);

#line  1767 "cs.ATG" 
			statement = isChecked ? (Statement)new CheckedStatement(block) : (Statement)new UncheckedStatement(block); 
		} else if (la.kind == 78) {
			lexer.NextToken();

#line  1769 "cs.ATG" 
			Statement elseStatement = null; 
			Expect(20);
			Expr(
#line  1770 "cs.ATG" 
out expr);
			Expect(21);
			EmbeddedStatement(
#line  1771 "cs.ATG" 
out embeddedStatement);
			if (la.kind == 66) {
				lexer.NextToken();
				EmbeddedStatement(
#line  1772 "cs.ATG" 
out elseStatement);
			}

#line  1773 "cs.ATG" 
			statement = elseStatement != null ? (Statement)new IfElseStatement(expr, embeddedStatement, elseStatement) :  (Statement)new IfElseStatement(expr, embeddedStatement); 
		} else if (la.kind == 109) {
			lexer.NextToken();

#line  1774 "cs.ATG" 
			ArrayList switchSections = new ArrayList(); SwitchSection switchSection; 
			Expect(20);
			Expr(
#line  1775 "cs.ATG" 
out expr);
			Expect(21);
			Expect(16);
			while (la.kind == 54 || la.kind == 62) {
				SwitchSection(
#line  1776 "cs.ATG" 
out switchSection);

#line  1776 "cs.ATG" 
				switchSections.Add(switchSection); 
			}
			Expect(17);

#line  1777 "cs.ATG" 
			statement = new SwitchStatement(expr, switchSections); 
		} else if (la.kind == 124) {
			lexer.NextToken();
			Expect(20);
			Expr(
#line  1779 "cs.ATG" 
out expr);
			Expect(21);
			EmbeddedStatement(
#line  1781 "cs.ATG" 
out embeddedStatement);

#line  1781 "cs.ATG" 
			statement = new DoLoopStatement(expr, embeddedStatement, ConditionType.While, ConditionPosition.Start);
		} else if (la.kind == 64) {
			lexer.NextToken();
			EmbeddedStatement(
#line  1782 "cs.ATG" 
out embeddedStatement);
			Expect(124);
			Expect(20);
			Expr(
#line  1783 "cs.ATG" 
out expr);
			Expect(21);
			Expect(11);

#line  1783 "cs.ATG" 
			statement = new DoLoopStatement(expr, embeddedStatement, ConditionType.While, ConditionPosition.End); 
		} else if (la.kind == 75) {
			lexer.NextToken();

#line  1784 "cs.ATG" 
			ArrayList initializer = null; ArrayList iterator = null; 
			Expect(20);
			if (StartOf(5)) {
				ForInitializer(
#line  1785 "cs.ATG" 
out initializer);
			}
			Expect(11);
			if (StartOf(5)) {
				Expr(
#line  1786 "cs.ATG" 
out expr);
			}
			Expect(11);
			if (StartOf(5)) {
				ForIterator(
#line  1787 "cs.ATG" 
out iterator);
			}
			Expect(21);
			EmbeddedStatement(
#line  1788 "cs.ATG" 
out embeddedStatement);

#line  1788 "cs.ATG" 
			statement = new ForStatement(initializer, expr, iterator, embeddedStatement); 
		} else if (la.kind == 76) {
			lexer.NextToken();
			Expect(20);
			Type(
#line  1789 "cs.ATG" 
out type);
			Expect(1);

#line  1789 "cs.ATG" 
			string varName = t.val; Point start = t.Location;
			Expect(80);
			Expr(
#line  1790 "cs.ATG" 
out expr);
			Expect(21);
			EmbeddedStatement(
#line  1791 "cs.ATG" 
out embeddedStatement);

#line  1791 "cs.ATG" 
			statement = new ForeachStatement(type, varName , expr, embeddedStatement); 
			statement.EndLocation = t.EndLocation;
			
		} else if (la.kind == 52) {
			lexer.NextToken();
			Expect(11);

#line  1795 "cs.ATG" 
			statement = new BreakStatement(); 
		} else if (la.kind == 60) {
			lexer.NextToken();
			Expect(11);

#line  1796 "cs.ATG" 
			statement = new ContinueStatement(); 
		} else if (la.kind == 77) {
			GotoStatement(
#line  1797 "cs.ATG" 
out statement);
		} else if (
#line  1798 "cs.ATG" 
IsYieldStatement()) {
			Expect(1);
			if (la.kind == 100) {
				lexer.NextToken();
				Expr(
#line  1798 "cs.ATG" 
out expr);

#line  1798 "cs.ATG" 
				statement = new YieldStatement(new ReturnStatement(expr)); 
			} else if (la.kind == 52) {
				lexer.NextToken();

#line  1799 "cs.ATG" 
				statement = new YieldStatement(new BreakStatement()); 
			} else SynErr(169);
			Expect(11);
		} else if (la.kind == 100) {
			lexer.NextToken();
			if (StartOf(5)) {
				Expr(
#line  1800 "cs.ATG" 
out expr);
			}
			Expect(11);

#line  1800 "cs.ATG" 
			statement = new ReturnStatement(expr); 
		} else if (la.kind == 111) {
			lexer.NextToken();
			if (StartOf(5)) {
				Expr(
#line  1801 "cs.ATG" 
out expr);
			}
			Expect(11);

#line  1801 "cs.ATG" 
			statement = new ThrowStatement(expr); 
		} else if (StartOf(5)) {
			StatementExpr(
#line  1803 "cs.ATG" 
out statement);
			Expect(11);
		} else if (la.kind == 113) {
			TryStatement(
#line  1805 "cs.ATG" 
out statement);
		} else if (la.kind == 85) {
			lexer.NextToken();
			Expect(20);
			Expr(
#line  1807 "cs.ATG" 
out expr);
			Expect(21);
			EmbeddedStatement(
#line  1808 "cs.ATG" 
out embeddedStatement);

#line  1808 "cs.ATG" 
			statement = new LockStatement(expr, embeddedStatement); 
		} else if (la.kind == 120) {

#line  1810 "cs.ATG" 
			Statement resourceAcquisitionStmt = null; 
			lexer.NextToken();
			Expect(20);
			ResourceAcquisition(
#line  1812 "cs.ATG" 
out resourceAcquisitionStmt);
			Expect(21);
			EmbeddedStatement(
#line  1813 "cs.ATG" 
out embeddedStatement);

#line  1813 "cs.ATG" 
			statement = new UsingStatement(resourceAcquisitionStmt, embeddedStatement); 
		} else if (la.kind == 118) {
			lexer.NextToken();
			Block(
#line  1815 "cs.ATG" 
out embeddedStatement);

#line  1815 "cs.ATG" 
			statement = new UnsafeStatement(embeddedStatement); 
		} else if (la.kind == 73) {
			lexer.NextToken();
			Expect(20);
			Type(
#line  1818 "cs.ATG" 
out type);

#line  1818 "cs.ATG" 
			if (type.PointerNestingLevel == 0) Error("can only fix pointer types");
			ArrayList pointerDeclarators = new ArrayList(1);
			
			Expect(1);

#line  1821 "cs.ATG" 
			string identifier = t.val; 
			Expect(3);
			Expr(
#line  1822 "cs.ATG" 
out expr);

#line  1822 "cs.ATG" 
			pointerDeclarators.Add(new VariableDeclaration(identifier, expr)); 
			while (la.kind == 14) {
				lexer.NextToken();
				Expect(1);

#line  1824 "cs.ATG" 
				identifier = t.val; 
				Expect(3);
				Expr(
#line  1825 "cs.ATG" 
out expr);

#line  1825 "cs.ATG" 
				pointerDeclarators.Add(new VariableDeclaration(identifier, expr)); 
			}
			Expect(21);
			EmbeddedStatement(
#line  1827 "cs.ATG" 
out embeddedStatement);

#line  1827 "cs.ATG" 
			statement = new FixedStatement(type, pointerDeclarators, embeddedStatement); 
		} else SynErr(170);
	}

	void SwitchSection(
#line  1849 "cs.ATG" 
out SwitchSection stmt) {

#line  1851 "cs.ATG" 
		SwitchSection switchSection = new SwitchSection();
		CaseLabel label;
		
		SwitchLabel(
#line  1855 "cs.ATG" 
out label);

#line  1855 "cs.ATG" 
		switchSection.SwitchLabels.Add(label); 
		while (la.kind == 54 || la.kind == 62) {
			SwitchLabel(
#line  1857 "cs.ATG" 
out label);

#line  1857 "cs.ATG" 
			switchSection.SwitchLabels.Add(label); 
		}

#line  1859 "cs.ATG" 
		compilationUnit.BlockStart(switchSection); 
		Statement();
		while (StartOf(20)) {
			Statement();
		}

#line  1862 "cs.ATG" 
		compilationUnit.BlockEnd();
		stmt = switchSection;
		
	}

	void ForInitializer(
#line  1830 "cs.ATG" 
out ArrayList initializer) {

#line  1832 "cs.ATG" 
		Statement stmt; 
		initializer = new ArrayList();
		
		if (
#line  1836 "cs.ATG" 
IsLocalVarDecl()) {
			LocalVariableDecl(
#line  1836 "cs.ATG" 
out stmt);

#line  1836 "cs.ATG" 
			initializer.Add(stmt);
		} else if (StartOf(5)) {
			StatementExpr(
#line  1837 "cs.ATG" 
out stmt);

#line  1837 "cs.ATG" 
			initializer.Add(stmt);
			while (la.kind == 14) {
				lexer.NextToken();
				StatementExpr(
#line  1837 "cs.ATG" 
out stmt);

#line  1837 "cs.ATG" 
				initializer.Add(stmt);
			}
		} else SynErr(171);
	}

	void ForIterator(
#line  1840 "cs.ATG" 
out ArrayList iterator) {

#line  1842 "cs.ATG" 
		Statement stmt; 
		iterator = new ArrayList();
		
		StatementExpr(
#line  1846 "cs.ATG" 
out stmt);

#line  1846 "cs.ATG" 
		iterator.Add(stmt);
		while (la.kind == 14) {
			lexer.NextToken();
			StatementExpr(
#line  1846 "cs.ATG" 
out stmt);

#line  1846 "cs.ATG" 
			iterator.Add(stmt); 
		}
	}

	void GotoStatement(
#line  1917 "cs.ATG" 
out Statement stmt) {

#line  1918 "cs.ATG" 
		Expression expr; stmt = null; 
		Expect(77);
		if (la.kind == 1) {
			lexer.NextToken();

#line  1922 "cs.ATG" 
			stmt = new GotoStatement(t.val); 
			Expect(11);
		} else if (la.kind == 54) {
			lexer.NextToken();
			Expr(
#line  1923 "cs.ATG" 
out expr);
			Expect(11);

#line  1923 "cs.ATG" 
			stmt = new GotoCaseStatement(expr); 
		} else if (la.kind == 62) {
			lexer.NextToken();
			Expect(11);

#line  1924 "cs.ATG" 
			stmt = new GotoCaseStatement(null); 
		} else SynErr(172);
	}

	void StatementExpr(
#line  1944 "cs.ATG" 
out Statement stmt) {

#line  1945 "cs.ATG" 
		Expression expr; 
		Expr(
#line  1947 "cs.ATG" 
out expr);

#line  1950 "cs.ATG" 
		stmt = new StatementExpression(expr); 
	}

	void TryStatement(
#line  1874 "cs.ATG" 
out Statement tryStatement) {

#line  1876 "cs.ATG" 
		Statement blockStmt = null, finallyStmt = null;
		ArrayList catchClauses = null;
		
		Expect(113);
		Block(
#line  1880 "cs.ATG" 
out blockStmt);
		if (la.kind == 55) {
			CatchClauses(
#line  1882 "cs.ATG" 
out catchClauses);
			if (la.kind == 72) {
				lexer.NextToken();
				Block(
#line  1882 "cs.ATG" 
out finallyStmt);
			}
		} else if (la.kind == 72) {
			lexer.NextToken();
			Block(
#line  1883 "cs.ATG" 
out finallyStmt);
		} else SynErr(173);

#line  1886 "cs.ATG" 
		tryStatement = new TryCatchStatement(blockStmt, catchClauses, finallyStmt);
			
	}

	void ResourceAcquisition(
#line  1928 "cs.ATG" 
out Statement stmt) {

#line  1930 "cs.ATG" 
		stmt = null;
		Expression expr;
		
		if (
#line  1935 "cs.ATG" 
IsLocalVarDecl()) {
			LocalVariableDecl(
#line  1935 "cs.ATG" 
out stmt);
		} else if (StartOf(5)) {
			Expr(
#line  1936 "cs.ATG" 
out expr);

#line  1940 "cs.ATG" 
			stmt = new StatementExpression(expr); 
		} else SynErr(174);
	}

	void SwitchLabel(
#line  1867 "cs.ATG" 
out CaseLabel label) {

#line  1868 "cs.ATG" 
		Expression expr = null; label = null; 
		if (la.kind == 54) {
			lexer.NextToken();
			Expr(
#line  1870 "cs.ATG" 
out expr);
			Expect(9);

#line  1870 "cs.ATG" 
			label =  new CaseLabel(expr); 
		} else if (la.kind == 62) {
			lexer.NextToken();
			Expect(9);

#line  1871 "cs.ATG" 
			label =  new CaseLabel(); 
		} else SynErr(175);
	}

	void CatchClauses(
#line  1891 "cs.ATG" 
out ArrayList catchClauses) {

#line  1893 "cs.ATG" 
		catchClauses = new ArrayList();
		
		Expect(55);

#line  1896 "cs.ATG" 
		string identifier;
		Statement stmt;
		TypeReference typeRef;
		
		if (la.kind == 16) {
			Block(
#line  1902 "cs.ATG" 
out stmt);

#line  1902 "cs.ATG" 
			catchClauses.Add(new CatchClause(stmt)); 
		} else if (la.kind == 20) {
			lexer.NextToken();
			ClassType(
#line  1904 "cs.ATG" 
out typeRef);

#line  1904 "cs.ATG" 
			identifier = null; 
			if (la.kind == 1) {
				lexer.NextToken();

#line  1905 "cs.ATG" 
				identifier = t.val; 
			}
			Expect(21);
			Block(
#line  1906 "cs.ATG" 
out stmt);

#line  1907 "cs.ATG" 
			catchClauses.Add(new CatchClause(typeRef, identifier, stmt)); 
			while (
#line  1908 "cs.ATG" 
IsTypedCatch()) {
				Expect(55);
				Expect(20);
				ClassType(
#line  1908 "cs.ATG" 
out typeRef);

#line  1908 "cs.ATG" 
				identifier = null; 
				if (la.kind == 1) {
					lexer.NextToken();

#line  1909 "cs.ATG" 
					identifier = t.val; 
				}
				Expect(21);
				Block(
#line  1910 "cs.ATG" 
out stmt);

#line  1911 "cs.ATG" 
				catchClauses.Add(new CatchClause(typeRef, identifier, stmt)); 
			}
			if (la.kind == 55) {
				lexer.NextToken();
				Block(
#line  1913 "cs.ATG" 
out stmt);

#line  1913 "cs.ATG" 
				catchClauses.Add(new CatchClause(stmt)); 
			}
		} else SynErr(176);
	}

	void UnaryExpr(
#line  1971 "cs.ATG" 
out Expression uExpr) {

#line  1973 "cs.ATG" 
		TypeReference type = null;
		Expression expr;
		ArrayList  expressions = new ArrayList();
		uExpr = null;
		
		while (StartOf(25) || 
#line  1997 "cs.ATG" 
IsTypeCast()) {
			if (la.kind == 4) {
				lexer.NextToken();

#line  1982 "cs.ATG" 
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.Plus)); 
			} else if (la.kind == 5) {
				lexer.NextToken();

#line  1983 "cs.ATG" 
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.Minus)); 
			} else if (la.kind == 24) {
				lexer.NextToken();

#line  1984 "cs.ATG" 
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.Not)); 
			} else if (la.kind == 27) {
				lexer.NextToken();

#line  1985 "cs.ATG" 
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.BitNot)); 
			} else if (la.kind == 6) {
				lexer.NextToken();

#line  1986 "cs.ATG" 
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.Star)); 
			} else if (la.kind == 31) {
				lexer.NextToken();

#line  1987 "cs.ATG" 
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.Increment)); 
			} else if (la.kind == 32) {
				lexer.NextToken();

#line  1988 "cs.ATG" 
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.Decrement)); 
			} else if (la.kind == 28) {
				lexer.NextToken();

#line  1989 "cs.ATG" 
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.BitWiseAnd)); 
			} else {
				Expect(20);
				Type(
#line  1997 "cs.ATG" 
out type);
				Expect(21);

#line  1997 "cs.ATG" 
				expressions.Add(new CastExpression(type)); 
			}
		}
		PrimaryExpr(
#line  2001 "cs.ATG" 
out expr);

#line  2001 "cs.ATG" 
		for (int i = 0; i < expressions.Count; ++i) {
		Expression nextExpression = i + 1 < expressions.Count ? (Expression)expressions[i + 1] : expr;
		if (expressions[i] is CastExpression) {
			((CastExpression)expressions[i]).Expression = nextExpression;
		} else {
			((UnaryOperatorExpression)expressions[i]).Expression = nextExpression;
		}
		}
		if (expressions.Count > 0) {
			uExpr = (Expression)expressions[0];
		} else {
			uExpr = expr;
		}
		
	}

	void ConditionalOrExpr(
#line  2180 "cs.ATG" 
ref Expression outExpr) {

#line  2181 "cs.ATG" 
		Expression expr;   
		ConditionalAndExpr(
#line  2183 "cs.ATG" 
ref outExpr);
		while (la.kind == 26) {
			lexer.NextToken();
			UnaryExpr(
#line  2183 "cs.ATG" 
out expr);
			ConditionalAndExpr(
#line  2183 "cs.ATG" 
ref expr);

#line  2183 "cs.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.LogicalOr, expr);  
		}
	}

	void PrimaryExpr(
#line  2018 "cs.ATG" 
out Expression pexpr) {

#line  2020 "cs.ATG" 
		TypeReference type = null;
		List<TypeReference> typeList = null;
		bool isArrayCreation = false;
		Expression expr;
		pexpr = null;
		
		if (la.kind == 112) {
			lexer.NextToken();

#line  2028 "cs.ATG" 
			pexpr = new PrimitiveExpression(true, "true");  
		} else if (la.kind == 71) {
			lexer.NextToken();

#line  2029 "cs.ATG" 
			pexpr = new PrimitiveExpression(false, "false"); 
		} else if (la.kind == 89) {
			lexer.NextToken();

#line  2030 "cs.ATG" 
			pexpr = new PrimitiveExpression(null, "null");  
		} else if (la.kind == 2) {
			lexer.NextToken();

#line  2031 "cs.ATG" 
			pexpr = new PrimitiveExpression(t.literalValue, t.val);  
		} else if (
#line  2032 "cs.ATG" 
la.kind == Tokens.Identifier && Peek(1).kind == Tokens.DoubleColon) {
			TypeName(
#line  2033 "cs.ATG" 
out type);

#line  2033 "cs.ATG" 
			pexpr = new TypeReferenceExpression(type); 
		} else if (la.kind == 1) {
			lexer.NextToken();

#line  2035 "cs.ATG" 
			pexpr = new IdentifierExpression(t.val); 
		} else if (la.kind == 20) {
			lexer.NextToken();
			Expr(
#line  2037 "cs.ATG" 
out expr);
			Expect(21);

#line  2037 "cs.ATG" 
			pexpr = new ParenthesizedExpression(expr); 
		} else if (StartOf(26)) {

#line  2039 "cs.ATG" 
			string val = null; 
			switch (la.kind) {
			case 51: {
				lexer.NextToken();

#line  2041 "cs.ATG" 
				val = "bool"; 
				break;
			}
			case 53: {
				lexer.NextToken();

#line  2042 "cs.ATG" 
				val = "byte"; 
				break;
			}
			case 56: {
				lexer.NextToken();

#line  2043 "cs.ATG" 
				val = "char"; 
				break;
			}
			case 61: {
				lexer.NextToken();

#line  2044 "cs.ATG" 
				val = "decimal"; 
				break;
			}
			case 65: {
				lexer.NextToken();

#line  2045 "cs.ATG" 
				val = "double"; 
				break;
			}
			case 74: {
				lexer.NextToken();

#line  2046 "cs.ATG" 
				val = "float"; 
				break;
			}
			case 81: {
				lexer.NextToken();

#line  2047 "cs.ATG" 
				val = "int"; 
				break;
			}
			case 86: {
				lexer.NextToken();

#line  2048 "cs.ATG" 
				val = "long"; 
				break;
			}
			case 90: {
				lexer.NextToken();

#line  2049 "cs.ATG" 
				val = "object"; 
				break;
			}
			case 101: {
				lexer.NextToken();

#line  2050 "cs.ATG" 
				val = "sbyte"; 
				break;
			}
			case 103: {
				lexer.NextToken();

#line  2051 "cs.ATG" 
				val = "short"; 
				break;
			}
			case 107: {
				lexer.NextToken();

#line  2052 "cs.ATG" 
				val = "string"; 
				break;
			}
			case 115: {
				lexer.NextToken();

#line  2053 "cs.ATG" 
				val = "uint"; 
				break;
			}
			case 116: {
				lexer.NextToken();

#line  2054 "cs.ATG" 
				val = "ulong"; 
				break;
			}
			case 119: {
				lexer.NextToken();

#line  2055 "cs.ATG" 
				val = "ushort"; 
				break;
			}
			}

#line  2056 "cs.ATG" 
			t.val = ""; 
			Expect(15);
			Expect(1);

#line  2056 "cs.ATG" 
			pexpr = new FieldReferenceExpression(new TypeReferenceExpression(val), t.val); 
		} else if (la.kind == 110) {
			lexer.NextToken();

#line  2058 "cs.ATG" 
			pexpr = new ThisReferenceExpression(); 
		} else if (la.kind == 50) {
			lexer.NextToken();

#line  2060 "cs.ATG" 
			Expression retExpr = new BaseReferenceExpression(); 
			if (la.kind == 15) {
				lexer.NextToken();
				Expect(1);

#line  2062 "cs.ATG" 
				retExpr = new FieldReferenceExpression(retExpr, t.val); 
			} else if (la.kind == 18) {
				lexer.NextToken();
				Expr(
#line  2063 "cs.ATG" 
out expr);

#line  2063 "cs.ATG" 
				ArrayList indices = new ArrayList(); if (expr != null) { indices.Add(expr); } 
				while (la.kind == 14) {
					lexer.NextToken();
					Expr(
#line  2064 "cs.ATG" 
out expr);

#line  2064 "cs.ATG" 
					if (expr != null) { indices.Add(expr); } 
				}
				Expect(19);

#line  2065 "cs.ATG" 
				retExpr = new IndexerExpression(retExpr, indices); 
			} else SynErr(177);

#line  2066 "cs.ATG" 
			pexpr = retExpr; 
		} else if (la.kind == 88) {
			lexer.NextToken();
			NonArrayType(
#line  2067 "cs.ATG" 
out type);

#line  2067 "cs.ATG" 
			ArrayList parameters = new ArrayList(); 
			if (la.kind == 20) {
				lexer.NextToken();

#line  2072 "cs.ATG" 
				ObjectCreateExpression oce = new ObjectCreateExpression(type, parameters); 
				if (StartOf(21)) {
					Argument(
#line  2073 "cs.ATG" 
out expr);

#line  2073 "cs.ATG" 
					if (expr != null) { parameters.Add(expr); } 
					while (la.kind == 14) {
						lexer.NextToken();
						Argument(
#line  2074 "cs.ATG" 
out expr);

#line  2074 "cs.ATG" 
						if (expr != null) { parameters.Add(expr); } 
					}
				}
				Expect(21);

#line  2076 "cs.ATG" 
				pexpr = oce; 
			} else if (la.kind == 18) {

#line  2078 "cs.ATG" 
				isArrayCreation = true; ArrayCreateExpression ace = new ArrayCreateExpression(type); pexpr = ace; 
				lexer.NextToken();

#line  2079 "cs.ATG" 
				int dims = 0; 
				ArrayList rank = new ArrayList(); 
				ArrayList parameterExpression = new ArrayList(); 
				if (StartOf(5)) {
					Expr(
#line  2083 "cs.ATG" 
out expr);

#line  2083 "cs.ATG" 
					if (expr != null) { parameterExpression.Add(expr); } 
					while (la.kind == 14) {
						lexer.NextToken();
						Expr(
#line  2085 "cs.ATG" 
out expr);

#line  2085 "cs.ATG" 
						if (expr != null) { parameterExpression.Add(expr); } 
					}
					Expect(19);

#line  2087 "cs.ATG" 
					parameters.Add(new ArrayCreationParameter(parameterExpression)); 
					ace.Parameters = parameters; 
					while (
#line  2090 "cs.ATG" 
IsDims()) {
						Expect(18);

#line  2090 "cs.ATG" 
						dims =0;
						while (la.kind == 14) {
							lexer.NextToken();

#line  2091 "cs.ATG" 
							dims++;
						}

#line  2091 "cs.ATG" 
						rank.Add(dims); 
						parameters.Add(new ArrayCreationParameter(dims)); 
						
						Expect(19);
					}

#line  2095 "cs.ATG" 
					if (rank.Count > 0) { 
					ace.Rank = (int[])rank.ToArray(typeof (int)); 
					} 
					
					if (la.kind == 16) {
						ArrayInitializer(
#line  2099 "cs.ATG" 
out expr);

#line  2099 "cs.ATG" 
						ace.ArrayInitializer = (ArrayInitializerExpression)expr; 
					}
				} else if (la.kind == 14 || la.kind == 19) {
					while (la.kind == 14) {
						lexer.NextToken();

#line  2101 "cs.ATG" 
						dims++;
					}

#line  2102 "cs.ATG" 
					parameters.Add(new ArrayCreationParameter(dims)); 
					
					Expect(19);
					while (
#line  2104 "cs.ATG" 
IsDims()) {
						Expect(18);

#line  2104 "cs.ATG" 
						dims =0;
						while (la.kind == 14) {
							lexer.NextToken();

#line  2104 "cs.ATG" 
							dims++;
						}

#line  2104 "cs.ATG" 
						parameters.Add(new ArrayCreationParameter(dims)); 
						Expect(19);
					}
					ArrayInitializer(
#line  2104 "cs.ATG" 
out expr);

#line  2104 "cs.ATG" 
					ace.ArrayInitializer = (ArrayInitializerExpression)expr; ace.Parameters = parameters; 
				} else SynErr(178);
			} else SynErr(179);
		} else if (la.kind == 114) {
			lexer.NextToken();
			Expect(20);
			if (
#line  2110 "cs.ATG" 
NotVoidPointer()) {
				Expect(122);

#line  2110 "cs.ATG" 
				type = new TypeReference("void"); 
			} else if (StartOf(9)) {
				Type(
#line  2111 "cs.ATG" 
out type);
			} else SynErr(180);
			Expect(21);

#line  2112 "cs.ATG" 
			pexpr = new TypeOfExpression(type); 
		} else if (
#line  2114 "cs.ATG" 
la.kind == Tokens.Default && Peek(1).kind == Tokens.OpenParenthesis) {
			Expect(62);
			Expect(20);
			Type(
#line  2116 "cs.ATG" 
out type);
			Expect(21);

#line  2116 "cs.ATG" 
			pexpr = new DefaultValueExpression(type); 
		} else if (la.kind == 104) {
			lexer.NextToken();
			Expect(20);
			Type(
#line  2117 "cs.ATG" 
out type);
			Expect(21);

#line  2117 "cs.ATG" 
			pexpr = new SizeOfExpression(type); 
		} else if (la.kind == 57) {
			lexer.NextToken();
			Expect(20);
			Expr(
#line  2118 "cs.ATG" 
out expr);
			Expect(21);

#line  2118 "cs.ATG" 
			pexpr = new CheckedExpression(expr); 
		} else if (la.kind == 117) {
			lexer.NextToken();
			Expect(20);
			Expr(
#line  2119 "cs.ATG" 
out expr);
			Expect(21);

#line  2119 "cs.ATG" 
			pexpr = new UncheckedExpression(expr); 
		} else if (la.kind == 63) {
			lexer.NextToken();
			AnonymousMethodExpr(
#line  2120 "cs.ATG" 
out expr);

#line  2120 "cs.ATG" 
			pexpr = expr; 
		} else SynErr(181);
		while (StartOf(27) || 
#line  2131 "cs.ATG" 
IsGenericFollowedBy(Tokens.Dot) && IsTypeReferenceExpression(pexpr) || 
#line  2139 "cs.ATG" 
IsGenericFollowedBy(Tokens.OpenParenthesis)) {
			if (la.kind == 31 || la.kind == 32) {
				if (la.kind == 31) {
					lexer.NextToken();

#line  2124 "cs.ATG" 
					pexpr = new UnaryOperatorExpression(pexpr, UnaryOperatorType.PostIncrement); 
				} else if (la.kind == 32) {
					lexer.NextToken();

#line  2125 "cs.ATG" 
					pexpr = new UnaryOperatorExpression(pexpr, UnaryOperatorType.PostDecrement); 
				} else SynErr(182);
			} else if (la.kind == 47) {
				lexer.NextToken();
				Expect(1);

#line  2128 "cs.ATG" 
				pexpr = new PointerReferenceExpression(pexpr, t.val); 
			} else if (la.kind == 15) {
				lexer.NextToken();
				Expect(1);

#line  2129 "cs.ATG" 
				pexpr = new FieldReferenceExpression(pexpr, t.val);
			} else if (
#line  2131 "cs.ATG" 
IsGenericFollowedBy(Tokens.Dot) && IsTypeReferenceExpression(pexpr)) {
				TypeArgumentList(
#line  2132 "cs.ATG" 
out typeList);
				Expect(15);
				Expect(1);

#line  2133 "cs.ATG" 
				pexpr = new FieldReferenceExpression(GetTypeReferenceExpression(pexpr, typeList), t.val);
			} else if (la.kind == 20) {
				lexer.NextToken();

#line  2135 "cs.ATG" 
				ArrayList parameters = new ArrayList(); 
				if (StartOf(21)) {
					Argument(
#line  2136 "cs.ATG" 
out expr);

#line  2136 "cs.ATG" 
					if (expr != null) {parameters.Add(expr);} 
					while (la.kind == 14) {
						lexer.NextToken();
						Argument(
#line  2137 "cs.ATG" 
out expr);

#line  2137 "cs.ATG" 
						if (expr != null) {parameters.Add(expr);} 
					}
				}
				Expect(21);

#line  2138 "cs.ATG" 
				pexpr = new InvocationExpression(pexpr, parameters); 
			} else if (
#line  2139 "cs.ATG" 
IsGenericFollowedBy(Tokens.OpenParenthesis)) {
				TypeArgumentList(
#line  2139 "cs.ATG" 
out typeList);
				Expect(20);

#line  2140 "cs.ATG" 
				ArrayList parameters = new ArrayList(); 
				if (StartOf(21)) {
					Argument(
#line  2141 "cs.ATG" 
out expr);

#line  2141 "cs.ATG" 
					if (expr != null) {parameters.Add(expr);} 
					while (la.kind == 14) {
						lexer.NextToken();
						Argument(
#line  2142 "cs.ATG" 
out expr);

#line  2142 "cs.ATG" 
						if (expr != null) {parameters.Add(expr);} 
					}
				}
				Expect(21);

#line  2143 "cs.ATG" 
				pexpr = new InvocationExpression(pexpr, parameters, typeList); 
			} else {

#line  2145 "cs.ATG" 
				if (isArrayCreation) Error("element access not allow on array creation");
				ArrayList indices = new ArrayList();
				
				lexer.NextToken();
				Expr(
#line  2148 "cs.ATG" 
out expr);

#line  2148 "cs.ATG" 
				if (expr != null) { indices.Add(expr); } 
				while (la.kind == 14) {
					lexer.NextToken();
					Expr(
#line  2149 "cs.ATG" 
out expr);

#line  2149 "cs.ATG" 
					if (expr != null) { indices.Add(expr); } 
				}
				Expect(19);

#line  2150 "cs.ATG" 
				pexpr = new IndexerExpression(pexpr, indices); 
			}
		}
	}

	void AnonymousMethodExpr(
#line  2154 "cs.ATG" 
out Expression outExpr) {

#line  2156 "cs.ATG" 
		AnonymousMethodExpression expr = new AnonymousMethodExpression();
		expr.StartLocation = t.Location;
		Statement stmt;
		List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
		outExpr = expr;
		
		if (la.kind == 20) {
			lexer.NextToken();
			if (StartOf(10)) {
				FormalParameterList(
#line  2165 "cs.ATG" 
p);

#line  2165 "cs.ATG" 
				expr.Parameters = p; 
			}
			Expect(21);
		}

#line  2170 "cs.ATG" 
		if (compilationUnit != null) { 
		Block(
#line  2171 "cs.ATG" 
out stmt);

#line  2171 "cs.ATG" 
		expr.Body  = (BlockStatement)stmt; 

#line  2172 "cs.ATG" 
		} else { 
		Expect(16);

#line  2174 "cs.ATG" 
		lexer.SkipCurrentBlock(); 
		Expect(17);

#line  2176 "cs.ATG" 
		} 

#line  2177 "cs.ATG" 
		expr.EndLocation = t.Location; 
	}

	void TypeArgumentList(
#line  2337 "cs.ATG" 
out List<TypeReference> types) {

#line  2339 "cs.ATG" 
		types = new List<TypeReference>();
		TypeReference type = null;
		
		Expect(23);
		Type(
#line  2343 "cs.ATG" 
out type);

#line  2343 "cs.ATG" 
		types.Add(type); 
		while (la.kind == 14) {
			lexer.NextToken();
			Type(
#line  2344 "cs.ATG" 
out type);

#line  2344 "cs.ATG" 
			types.Add(type); 
		}
		Expect(22);
	}

	void ConditionalAndExpr(
#line  2186 "cs.ATG" 
ref Expression outExpr) {

#line  2187 "cs.ATG" 
		Expression expr; 
		InclusiveOrExpr(
#line  2189 "cs.ATG" 
ref outExpr);
		while (la.kind == 25) {
			lexer.NextToken();
			UnaryExpr(
#line  2189 "cs.ATG" 
out expr);
			InclusiveOrExpr(
#line  2189 "cs.ATG" 
ref expr);

#line  2189 "cs.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.LogicalAnd, expr);  
		}
	}

	void InclusiveOrExpr(
#line  2192 "cs.ATG" 
ref Expression outExpr) {

#line  2193 "cs.ATG" 
		Expression expr; 
		ExclusiveOrExpr(
#line  2195 "cs.ATG" 
ref outExpr);
		while (la.kind == 29) {
			lexer.NextToken();
			UnaryExpr(
#line  2195 "cs.ATG" 
out expr);
			ExclusiveOrExpr(
#line  2195 "cs.ATG" 
ref expr);

#line  2195 "cs.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.BitwiseOr, expr);  
		}
	}

	void ExclusiveOrExpr(
#line  2198 "cs.ATG" 
ref Expression outExpr) {

#line  2199 "cs.ATG" 
		Expression expr; 
		AndExpr(
#line  2201 "cs.ATG" 
ref outExpr);
		while (la.kind == 30) {
			lexer.NextToken();
			UnaryExpr(
#line  2201 "cs.ATG" 
out expr);
			AndExpr(
#line  2201 "cs.ATG" 
ref expr);

#line  2201 "cs.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.ExclusiveOr, expr);  
		}
	}

	void AndExpr(
#line  2204 "cs.ATG" 
ref Expression outExpr) {

#line  2205 "cs.ATG" 
		Expression expr; 
		EqualityExpr(
#line  2207 "cs.ATG" 
ref outExpr);
		while (la.kind == 28) {
			lexer.NextToken();
			UnaryExpr(
#line  2207 "cs.ATG" 
out expr);
			EqualityExpr(
#line  2207 "cs.ATG" 
ref expr);

#line  2207 "cs.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.BitwiseAnd, expr);  
		}
	}

	void EqualityExpr(
#line  2210 "cs.ATG" 
ref Expression outExpr) {

#line  2212 "cs.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		RelationalExpr(
#line  2216 "cs.ATG" 
ref outExpr);
		while (la.kind == 33 || la.kind == 34) {
			if (la.kind == 34) {
				lexer.NextToken();

#line  2219 "cs.ATG" 
				op = BinaryOperatorType.InEquality; 
			} else {
				lexer.NextToken();

#line  2220 "cs.ATG" 
				op = BinaryOperatorType.Equality; 
			}
			UnaryExpr(
#line  2222 "cs.ATG" 
out expr);
			RelationalExpr(
#line  2222 "cs.ATG" 
ref expr);

#line  2222 "cs.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr);  
		}
	}

	void RelationalExpr(
#line  2226 "cs.ATG" 
ref Expression outExpr) {

#line  2228 "cs.ATG" 
		TypeReference type;
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		ShiftExpr(
#line  2233 "cs.ATG" 
ref outExpr);
		while (StartOf(28)) {
			if (StartOf(29)) {
				if (la.kind == 23) {
					lexer.NextToken();

#line  2236 "cs.ATG" 
					op = BinaryOperatorType.LessThan; 
				} else if (la.kind == 22) {
					lexer.NextToken();

#line  2237 "cs.ATG" 
					op = BinaryOperatorType.GreaterThan; 
				} else if (la.kind == 36) {
					lexer.NextToken();

#line  2238 "cs.ATG" 
					op = BinaryOperatorType.LessThanOrEqual; 
				} else if (la.kind == 35) {
					lexer.NextToken();

#line  2239 "cs.ATG" 
					op = BinaryOperatorType.GreaterThanOrEqual; 
				} else SynErr(183);
				UnaryExpr(
#line  2241 "cs.ATG" 
out expr);
				ShiftExpr(
#line  2241 "cs.ATG" 
ref expr);

#line  2241 "cs.ATG" 
				outExpr = new BinaryOperatorExpression(outExpr, op, expr);  
			} else {
				if (la.kind == 84) {
					lexer.NextToken();

#line  2244 "cs.ATG" 
					op = BinaryOperatorType.TypeCheck; 
				} else if (la.kind == 49) {
					lexer.NextToken();

#line  2245 "cs.ATG" 
					op = BinaryOperatorType.AsCast; 
				} else SynErr(184);
				Type(
#line  2247 "cs.ATG" 
out type);

#line  2247 "cs.ATG" 
				outExpr = new BinaryOperatorExpression(outExpr, op, new TypeReferenceExpression(type)); 
			}
		}
	}

	void ShiftExpr(
#line  2251 "cs.ATG" 
ref Expression outExpr) {

#line  2253 "cs.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		AdditiveExpr(
#line  2257 "cs.ATG" 
ref outExpr);
		while (la.kind == 37 || 
#line  2260 "cs.ATG" 
IsShiftRight()) {
			if (la.kind == 37) {
				lexer.NextToken();

#line  2259 "cs.ATG" 
				op = BinaryOperatorType.ShiftLeft; 
			} else {
				Expect(22);
				Expect(22);

#line  2261 "cs.ATG" 
				op = BinaryOperatorType.ShiftRight; 
			}
			UnaryExpr(
#line  2264 "cs.ATG" 
out expr);
			AdditiveExpr(
#line  2264 "cs.ATG" 
ref expr);

#line  2264 "cs.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr);  
		}
	}

	void AdditiveExpr(
#line  2268 "cs.ATG" 
ref Expression outExpr) {

#line  2270 "cs.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		MultiplicativeExpr(
#line  2274 "cs.ATG" 
ref outExpr);
		while (la.kind == 4 || la.kind == 5) {
			if (la.kind == 4) {
				lexer.NextToken();

#line  2277 "cs.ATG" 
				op = BinaryOperatorType.Add; 
			} else {
				lexer.NextToken();

#line  2278 "cs.ATG" 
				op = BinaryOperatorType.Subtract; 
			}
			UnaryExpr(
#line  2280 "cs.ATG" 
out expr);
			MultiplicativeExpr(
#line  2280 "cs.ATG" 
ref expr);

#line  2280 "cs.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr);  
		}
	}

	void MultiplicativeExpr(
#line  2284 "cs.ATG" 
ref Expression outExpr) {

#line  2286 "cs.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		while (la.kind == 6 || la.kind == 7 || la.kind == 8) {
			if (la.kind == 6) {
				lexer.NextToken();

#line  2292 "cs.ATG" 
				op = BinaryOperatorType.Multiply; 
			} else if (la.kind == 7) {
				lexer.NextToken();

#line  2293 "cs.ATG" 
				op = BinaryOperatorType.Divide; 
			} else {
				lexer.NextToken();

#line  2294 "cs.ATG" 
				op = BinaryOperatorType.Modulus; 
			}
			UnaryExpr(
#line  2296 "cs.ATG" 
out expr);

#line  2296 "cs.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr); 
		}
	}

	void TypeParameterConstraintsClauseBase(
#line  2389 "cs.ATG" 
out TypeReference type) {

#line  2390 "cs.ATG" 
		TypeReference t; type = null; 
		if (la.kind == 108) {
			lexer.NextToken();

#line  2392 "cs.ATG" 
			type = new TypeReference("struct"); 
		} else if (la.kind == 58) {
			lexer.NextToken();

#line  2393 "cs.ATG" 
			type = new TypeReference("struct"); 
		} else if (la.kind == 88) {
			lexer.NextToken();
			Expect(20);
			Expect(21);

#line  2394 "cs.ATG" 
			type = new TypeReference("struct"); 
		} else if (StartOf(9)) {
			Type(
#line  2395 "cs.ATG" 
out t);

#line  2395 "cs.ATG" 
			type = t; 
		} else SynErr(185);
	}


	public Parser(ILexer lexer) : base(lexer)
	{
	}
	
	public override void Parse()
	{
		CS();

	}
	
	protected void ExpectWeak(int n, int follow)
	{
		if (lexer.LookAhead.kind == n) {
			lexer.NextToken();
		} else {
			SynErr(n);
			while (!StartOf(follow)) {
				lexer.NextToken();
			}
		}
	}
	
	protected bool WeakSeparator(int n, int syFol, int repFol)
	{
		bool[] s = new bool[maxT + 1];
		
		if (lexer.LookAhead.kind == n) {
			lexer.NextToken();
			return true;
		} else if (StartOf(repFol)) {
			return false;
		} else {
			for (int i = 0; i <= maxT; i++) {
				s[i] = set[syFol, i] || set[repFol, i] || set[0, i];
			}
			SynErr(n);
			while (!s[lexer.LookAhead.kind]) {
				lexer.NextToken();
			}
			return StartOf(syFol);
		}
	}
	
	protected override void SynErr(int line, int col, int errorNumber)
	{
		errors.count++; 
		string s;
		switch (errorNumber) {
			case 0: s = "EOF expected"; break;
			case 1: s = "ident expected"; break;
			case 2: s = "Literal expected"; break;
			case 3: s = "\"=\" expected"; break;
			case 4: s = "\"+\" expected"; break;
			case 5: s = "\"-\" expected"; break;
			case 6: s = "\"*\" expected"; break;
			case 7: s = "\"/\" expected"; break;
			case 8: s = "\"%\" expected"; break;
			case 9: s = "\":\" expected"; break;
			case 10: s = "\"::\" expected"; break;
			case 11: s = "\";\" expected"; break;
			case 12: s = "\"?\" expected"; break;
			case 13: s = "\"??\" expected"; break;
			case 14: s = "\",\" expected"; break;
			case 15: s = "\".\" expected"; break;
			case 16: s = "\"{\" expected"; break;
			case 17: s = "\"}\" expected"; break;
			case 18: s = "\"[\" expected"; break;
			case 19: s = "\"]\" expected"; break;
			case 20: s = "\"(\" expected"; break;
			case 21: s = "\")\" expected"; break;
			case 22: s = "\">\" expected"; break;
			case 23: s = "\"<\" expected"; break;
			case 24: s = "\"!\" expected"; break;
			case 25: s = "\"&&\" expected"; break;
			case 26: s = "\"||\" expected"; break;
			case 27: s = "\"~\" expected"; break;
			case 28: s = "\"&\" expected"; break;
			case 29: s = "\"|\" expected"; break;
			case 30: s = "\"^\" expected"; break;
			case 31: s = "\"++\" expected"; break;
			case 32: s = "\"--\" expected"; break;
			case 33: s = "\"==\" expected"; break;
			case 34: s = "\"!=\" expected"; break;
			case 35: s = "\">=\" expected"; break;
			case 36: s = "\"<=\" expected"; break;
			case 37: s = "\"<<\" expected"; break;
			case 38: s = "\"+=\" expected"; break;
			case 39: s = "\"-=\" expected"; break;
			case 40: s = "\"*=\" expected"; break;
			case 41: s = "\"/=\" expected"; break;
			case 42: s = "\"%=\" expected"; break;
			case 43: s = "\"&=\" expected"; break;
			case 44: s = "\"|=\" expected"; break;
			case 45: s = "\"^=\" expected"; break;
			case 46: s = "\"<<=\" expected"; break;
			case 47: s = "\"->\" expected"; break;
			case 48: s = "\"abstract\" expected"; break;
			case 49: s = "\"as\" expected"; break;
			case 50: s = "\"base\" expected"; break;
			case 51: s = "\"bool\" expected"; break;
			case 52: s = "\"break\" expected"; break;
			case 53: s = "\"byte\" expected"; break;
			case 54: s = "\"case\" expected"; break;
			case 55: s = "\"catch\" expected"; break;
			case 56: s = "\"char\" expected"; break;
			case 57: s = "\"checked\" expected"; break;
			case 58: s = "\"class\" expected"; break;
			case 59: s = "\"const\" expected"; break;
			case 60: s = "\"continue\" expected"; break;
			case 61: s = "\"decimal\" expected"; break;
			case 62: s = "\"default\" expected"; break;
			case 63: s = "\"delegate\" expected"; break;
			case 64: s = "\"do\" expected"; break;
			case 65: s = "\"double\" expected"; break;
			case 66: s = "\"else\" expected"; break;
			case 67: s = "\"enum\" expected"; break;
			case 68: s = "\"event\" expected"; break;
			case 69: s = "\"explicit\" expected"; break;
			case 70: s = "\"extern\" expected"; break;
			case 71: s = "\"false\" expected"; break;
			case 72: s = "\"finally\" expected"; break;
			case 73: s = "\"fixed\" expected"; break;
			case 74: s = "\"float\" expected"; break;
			case 75: s = "\"for\" expected"; break;
			case 76: s = "\"foreach\" expected"; break;
			case 77: s = "\"goto\" expected"; break;
			case 78: s = "\"if\" expected"; break;
			case 79: s = "\"implicit\" expected"; break;
			case 80: s = "\"in\" expected"; break;
			case 81: s = "\"int\" expected"; break;
			case 82: s = "\"interface\" expected"; break;
			case 83: s = "\"internal\" expected"; break;
			case 84: s = "\"is\" expected"; break;
			case 85: s = "\"lock\" expected"; break;
			case 86: s = "\"long\" expected"; break;
			case 87: s = "\"namespace\" expected"; break;
			case 88: s = "\"new\" expected"; break;
			case 89: s = "\"null\" expected"; break;
			case 90: s = "\"object\" expected"; break;
			case 91: s = "\"operator\" expected"; break;
			case 92: s = "\"out\" expected"; break;
			case 93: s = "\"override\" expected"; break;
			case 94: s = "\"params\" expected"; break;
			case 95: s = "\"private\" expected"; break;
			case 96: s = "\"protected\" expected"; break;
			case 97: s = "\"public\" expected"; break;
			case 98: s = "\"readonly\" expected"; break;
			case 99: s = "\"ref\" expected"; break;
			case 100: s = "\"return\" expected"; break;
			case 101: s = "\"sbyte\" expected"; break;
			case 102: s = "\"sealed\" expected"; break;
			case 103: s = "\"short\" expected"; break;
			case 104: s = "\"sizeof\" expected"; break;
			case 105: s = "\"stackalloc\" expected"; break;
			case 106: s = "\"static\" expected"; break;
			case 107: s = "\"string\" expected"; break;
			case 108: s = "\"struct\" expected"; break;
			case 109: s = "\"switch\" expected"; break;
			case 110: s = "\"this\" expected"; break;
			case 111: s = "\"throw\" expected"; break;
			case 112: s = "\"true\" expected"; break;
			case 113: s = "\"try\" expected"; break;
			case 114: s = "\"typeof\" expected"; break;
			case 115: s = "\"uint\" expected"; break;
			case 116: s = "\"ulong\" expected"; break;
			case 117: s = "\"unchecked\" expected"; break;
			case 118: s = "\"unsafe\" expected"; break;
			case 119: s = "\"ushort\" expected"; break;
			case 120: s = "\"using\" expected"; break;
			case 121: s = "\"virtual\" expected"; break;
			case 122: s = "\"void\" expected"; break;
			case 123: s = "\"volatile\" expected"; break;
			case 124: s = "\"while\" expected"; break;
			case 125: s = "??? expected"; break;
			case 126: s = "invalid NamespaceMemberDecl"; break;
			case 127: s = "invalid NonArrayType"; break;
			case 128: s = "invalid AttributeArguments"; break;
			case 129: s = "invalid Expr"; break;
			case 130: s = "invalid TypeModifier"; break;
			case 131: s = "invalid TypeDecl"; break;
			case 132: s = "invalid TypeDecl"; break;
			case 133: s = "invalid IntegralType"; break;
			case 134: s = "invalid Type"; break;
			case 135: s = "invalid Type"; break;
			case 136: s = "invalid FormalParameterList"; break;
			case 137: s = "invalid FormalParameterList"; break;
			case 138: s = "invalid ClassType"; break;
			case 139: s = "invalid MemberModifier"; break;
			case 140: s = "invalid ClassMemberDecl"; break;
			case 141: s = "invalid ClassMemberDecl"; break;
			case 142: s = "invalid StructMemberDecl"; break;
			case 143: s = "invalid StructMemberDecl"; break;
			case 144: s = "invalid StructMemberDecl"; break;
			case 145: s = "invalid StructMemberDecl"; break;
			case 146: s = "invalid StructMemberDecl"; break;
			case 147: s = "invalid StructMemberDecl"; break;
			case 148: s = "invalid StructMemberDecl"; break;
			case 149: s = "invalid StructMemberDecl"; break;
			case 150: s = "invalid StructMemberDecl"; break;
			case 151: s = "invalid StructMemberDecl"; break;
			case 152: s = "invalid InterfaceMemberDecl"; break;
			case 153: s = "invalid InterfaceMemberDecl"; break;
			case 154: s = "invalid InterfaceMemberDecl"; break;
			case 155: s = "invalid SimpleType"; break;
			case 156: s = "invalid AccessorModifiers"; break;
			case 157: s = "invalid EventAccessorDecls"; break;
			case 158: s = "invalid ConstructorInitializer"; break;
			case 159: s = "invalid OverloadableOperator"; break;
			case 160: s = "invalid AccessorDecls"; break;
			case 161: s = "invalid InterfaceAccessors"; break;
			case 162: s = "invalid InterfaceAccessors"; break;
			case 163: s = "invalid GetAccessorDecl"; break;
			case 164: s = "invalid SetAccessorDecl"; break;
			case 165: s = "invalid VariableInitializer"; break;
			case 166: s = "invalid Statement"; break;
			case 167: s = "invalid AssignmentOperator"; break;
			case 168: s = "invalid EmbeddedStatement"; break;
			case 169: s = "invalid EmbeddedStatement"; break;
			case 170: s = "invalid EmbeddedStatement"; break;
			case 171: s = "invalid ForInitializer"; break;
			case 172: s = "invalid GotoStatement"; break;
			case 173: s = "invalid TryStatement"; break;
			case 174: s = "invalid ResourceAcquisition"; break;
			case 175: s = "invalid SwitchLabel"; break;
			case 176: s = "invalid CatchClauses"; break;
			case 177: s = "invalid PrimaryExpr"; break;
			case 178: s = "invalid PrimaryExpr"; break;
			case 179: s = "invalid PrimaryExpr"; break;
			case 180: s = "invalid PrimaryExpr"; break;
			case 181: s = "invalid PrimaryExpr"; break;
			case 182: s = "invalid PrimaryExpr"; break;
			case 183: s = "invalid RelationalExpr"; break;
			case 184: s = "invalid RelationalExpr"; break;
			case 185: s = "invalid TypeParameterConstraintsClauseBase"; break;

			default: s = "error " + errorNumber; break;
		}
		errors.Error(line, col, s);
	}
	
	protected bool StartOf(int s)
	{
		return set[s, lexer.LookAhead.kind];
	}
	
	static bool[,] set = {
	{T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,T, T,x,x,x, x,x,x,T, T,T,x,x, x,x,T,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, T,x,x,x, x,x,x,T, T,T,x,x, x,x,T,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,x,x,T, T,T,x,x, x,x,T,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,x,x, T,x,x,x, x,T,x,x, x,T,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,T, x,x,x,x, x,x,x},
	{x,T,T,x, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, T,x,x,T, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,T,x,x, T,T,x,x, x,T,x,T, x,T,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,T,x,x, x,x,T,x, T,T,T,x, x,x,x,x, x,x,x,x, x,T,x,T, T,x,x,T, x,x,T,x, T,x,T,T, T,T,x,T, x,x,x,x, x,x,x},
	{x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,x,x, T,T,T,T, T,T,x,T, T,T,T,x, x,T,x,T, x,T,T,T, x,T,T,x, T,T,T,x, x,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,x,x, T,x,x,x, x,T,x,x, x,T,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,T, x,x,x,T, x,x,x,x, x,x,x,T, T,x,x,T, x,x,T,x, x,x,x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,x,x, T,x,x,x, x,T,x,x, x,T,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,T,x, T,x,T,x, x,x,x,T, x,T,x,T, x,x,x,T, x,x,x,x, x,x,x,T, T,x,x,T, x,x,T,x, x,x,x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,T, x,T,x,x, T,x,T,T, x,T,x,T, x,T,x,T, T,T,T,x, x,x,T,x, x,x,x,T, x,T,T,T, x,x,T,x, T,x,T,x, x,T,x,T, T,T,T,x, x,T,T,T, x,x,T,T, T,x,x,x, x,x,x,T, T,x,T,T, x,T,T,T, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,T, T,T,T,x, x,x,T,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,T,x, x,T,x,T, x,x,x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,T, x,T,x,x, T,x,T,T, x,T,x,T, x,T,x,T, T,T,T,x, x,x,T,x, x,x,x,T, x,T,T,T, x,x,T,x, T,x,T,x, x,T,x,T, T,T,T,x, x,T,T,T, x,x,T,T, T,x,x,x, x,x,x,T, T,x,T,T, x,T,T,T, x,x,x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,x,x, T,x,x,x, x,T,x,x, x,T,x,x, T,x,x,x, x,x,T,x, x,x,x,x, x,T,x,x, x,x,T,x, T,x,T,x, x,x,x,x, x,x,x,x, x,T,x,T, x,x,x,T, x,x,x,x, x,x,x,T, T,x,x,T, x,x,T,x, x,x,x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,x,x, T,x,x,x, x,T,x,x, x,T,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,T,x, T,x,x,x, x,x,x,T, x,T,x,T, x,x,x,T, x,x,x,x, x,x,x,T, T,x,x,T, x,x,T,x, x,x,x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,x,x, T,x,T,T, x,T,x,T, x,T,x,T, T,T,x,x, x,x,T,x, x,x,x,T, x,T,T,x, x,x,T,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,T, x,x,x,T, T,x,x,x, x,x,x,T, T,x,x,T, x,x,T,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,x,x, T,x,x,x, x,T,x,x, x,T,x,x, T,x,x,x, x,x,T,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,T, x,x,x,T, x,x,x,x, x,x,x,T, T,x,x,T, x,x,T,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,T, x,x,x,x, x,x,x},
	{x,T,T,x, T,T,T,x, x,x,x,T, x,x,x,x, T,x,x,x, T,x,x,x, T,x,x,T, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,x,x, T,T,x,T, T,T,x,T, T,T,x,x, x,x,x,T, x,T,T,T, T,T,T,x, x,T,x,x, x,T,T,x, T,T,T,x, x,x,x,x, x,x,x,x, T,T,x,T, T,x,x,T, x,T,T,T, T,T,T,T, T,T,T,T, T,x,x,x, T,x,x},
	{x,T,T,x, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, T,x,x,T, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,T,x,x, T,T,x,x, x,T,x,T, x,T,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,T,x,x, x,x,T,x, T,T,T,x, T,x,x,x, x,x,x,T, x,T,x,T, T,x,x,T, x,x,T,x, T,x,T,T, T,T,x,T, x,x,x,x, x,x,x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,T,T,x, T,T,T,x, x,x,x,T, x,x,x,x, T,x,x,x, T,x,x,x, T,x,x,T, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,x,x, T,T,x,x, T,T,x,T, T,T,x,x, x,x,x,T, x,T,T,T, T,T,T,x, x,T,x,x, x,T,T,x, T,T,T,x, x,x,x,x, x,x,x,x, T,T,x,T, T,x,x,T, x,T,T,T, T,T,T,T, T,T,T,T, T,x,x,x, T,x,x},
	{x,T,T,x, T,T,T,x, x,x,x,x, x,x,x,x, T,x,x,x, T,x,x,x, T,x,x,T, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,T,x,x, T,T,x,x, x,T,x,T, x,T,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,T,x,x, x,x,T,x, T,T,T,x, x,x,x,x, x,x,x,x, x,T,x,T, T,T,x,T, x,x,T,x, T,x,T,T, T,T,x,T, x,x,x,x, x,x,x},
	{x,x,x,x, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,T, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,x,x, T,x,x,x, x,T,x,x, x,T,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,T, x,x,x,T, x,x,x,x, x,x,x,T, T,x,x,T, x,x,x,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x}

	};
} // end Parser

}