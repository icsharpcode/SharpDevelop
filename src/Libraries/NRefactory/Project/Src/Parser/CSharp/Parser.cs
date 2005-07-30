
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
	const int maxT = 124;

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
bool IsSimpleTypeCast ()
{
	// assert: la.kind == _lpar
	lexer.StartPeek();
	Token pt1 = lexer.Peek();
	Token pt  = lexer.Peek();
	
	if (Tokens.TypeKW[pt1.kind]) {
		if (pt.kind == Tokens.Times || pt.kind == Tokens.OpenSquareBracket) {
			return IsPointerOrDims(ref pt) && pt.kind == Tokens.CloseParenthesis;
		} else {
		  return pt.kind == Tokens.CloseParenthesis;
		}
	} else if (pt1.kind == Tokens.Void) {
		return IsPointerOrDims(ref pt) && pt.kind == Tokens.CloseParenthesis;
	}
	
	return false;
}

// "(" NonGenericTypeName ")" castFollower
// NonGenericTypeName = ident [ "::" ident ] {  "." ident }
bool GuessTypeCast ()
{
	// assert: la.kind == _lpar
	StartPeek();
	Token pt = Peek();

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

/* True, if "[" is followed by "," or "]" *
 * or if the current token is "*"         */
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
		return true;
	}
	return true;
}
/* True, if lookahead is a primitive type keyword, or *
 * if it is a type declaration followed by an ident   */
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
	
	return IsQualident(ref pt, out ignore) && SkipGeneric(ref pt) && IsPointerOrDims(ref pt) && 
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

#line  552 "cs.ATG" 
		compilationUnit = new CompilationUnit(); 
		while (la.kind == 119) {
			UsingDirective();
		}
		while (
#line  555 "cs.ATG" 
IsGlobalAttrTarget()) {
			GlobalAttributeSection();
		}
		while (StartOf(1)) {
			NamespaceMemberDecl();
		}
		Expect(0);
	}

	void UsingDirective() {

#line  562 "cs.ATG" 
		string qualident = null, aliasident = null;
		
		Expect(119);

#line  565 "cs.ATG" 
		Point startPos = t.Location; 
		if (
#line  566 "cs.ATG" 
IsAssignment()) {
			lexer.NextToken();

#line  566 "cs.ATG" 
			aliasident = t.val; 
			Expect(3);
		}
		Qualident(
#line  567 "cs.ATG" 
out qualident);
		Expect(11);

#line  569 "cs.ATG" 
		if (qualident != null && qualident.Length > 0) {
		 INode node;
		 if (aliasident != null) {
		     node = new UsingDeclaration(aliasident, qualident);
		 } else {
		     node = new UsingDeclaration(qualident);
		 }
		 node.StartLocation = startPos;
		 node.EndLocation   = t.EndLocation;
		 compilationUnit.AddChild(node);
		}
		
	}

	void GlobalAttributeSection() {
		Expect(17);

#line  585 "cs.ATG" 
		Point startPos = t.Location; 
		Expect(1);

#line  586 "cs.ATG" 
		if (t.val != "assembly") Error("global attribute target specifier (\"assembly\") expected");
		string attributeTarget = t.val;
		List<ASTAttribute> attributes = new List<ASTAttribute>();
		ASTAttribute attribute;
		
		Expect(9);
		Attribute(
#line  591 "cs.ATG" 
out attribute);

#line  591 "cs.ATG" 
		attributes.Add(attribute); 
		while (
#line  592 "cs.ATG" 
NotFinalComma()) {
			Expect(13);
			Attribute(
#line  592 "cs.ATG" 
out attribute);

#line  592 "cs.ATG" 
			attributes.Add(attribute); 
		}
		if (la.kind == 13) {
			lexer.NextToken();
		}
		Expect(18);

#line  594 "cs.ATG" 
		AttributeSection section = new AttributeSection(attributeTarget, attributes);
		section.StartLocation = startPos;
		section.EndLocation = t.EndLocation;
		compilationUnit.AddChild(section);
		
	}

	void NamespaceMemberDecl() {

#line  678 "cs.ATG" 
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		Modifiers m = new Modifiers();
		string qualident;
		
		if (la.kind == 86) {
			lexer.NextToken();

#line  684 "cs.ATG" 
			Point startPos = t.Location; 
			Qualident(
#line  685 "cs.ATG" 
out qualident);

#line  685 "cs.ATG" 
			INode node =  new NamespaceDeclaration(qualident);
			node.StartLocation = startPos;
			compilationUnit.AddChild(node);
			compilationUnit.BlockStart(node);
			
			Expect(15);
			while (la.kind == 119) {
				UsingDirective();
			}
			while (StartOf(1)) {
				NamespaceMemberDecl();
			}
			Expect(16);
			if (la.kind == 11) {
				lexer.NextToken();
			}

#line  694 "cs.ATG" 
			node.EndLocation   = t.EndLocation;
			compilationUnit.BlockEnd();
			
		} else if (StartOf(2)) {
			while (la.kind == 17) {
				AttributeSection(
#line  698 "cs.ATG" 
out section);

#line  698 "cs.ATG" 
				attributes.Add(section); 
			}
			while (StartOf(3)) {
				TypeModifier(
#line  699 "cs.ATG" 
m);
			}
			TypeDecl(
#line  700 "cs.ATG" 
m, attributes);
		} else SynErr(125);
	}

	void Qualident(
#line  818 "cs.ATG" 
out string qualident) {
		Expect(1);

#line  820 "cs.ATG" 
		qualidentBuilder.Length = 0; qualidentBuilder.Append(t.val); 
		while (
#line  821 "cs.ATG" 
DotAndIdent()) {
			Expect(14);
			Expect(1);

#line  821 "cs.ATG" 
			qualidentBuilder.Append('.');
			qualidentBuilder.Append(t.val); 
			
		}

#line  824 "cs.ATG" 
		qualident = qualidentBuilder.ToString(); 
	}

	void Attribute(
#line  601 "cs.ATG" 
out ASTAttribute attribute) {

#line  602 "cs.ATG" 
		string qualident; 
		Qualident(
#line  604 "cs.ATG" 
out qualident);

#line  604 "cs.ATG" 
		List<Expression> positional = new List<Expression>();
		List<NamedArgumentExpression> named = new List<NamedArgumentExpression>();
		string name = qualident;
		
		if (la.kind == 19) {
			AttributeArguments(
#line  608 "cs.ATG" 
positional, named);
		}

#line  608 "cs.ATG" 
		attribute  = new ICSharpCode.NRefactory.Parser.AST.Attribute(name, positional, named);
	}

	void AttributeArguments(
#line  611 "cs.ATG" 
List<Expression> positional, List<NamedArgumentExpression> named) {

#line  613 "cs.ATG" 
		bool nameFound = false;
		string name = "";
		Expression expr;
		
		Expect(19);
		if (StartOf(4)) {
			if (
#line  621 "cs.ATG" 
IsAssignment()) {

#line  621 "cs.ATG" 
				nameFound = true; 
				lexer.NextToken();

#line  622 "cs.ATG" 
				name = t.val; 
				Expect(3);
			}
			Expr(
#line  624 "cs.ATG" 
out expr);

#line  624 "cs.ATG" 
			if (expr != null) {if(name == "") positional.Add(expr);
			else { named.Add(new NamedArgumentExpression(name, expr)); name = ""; }
			}
			
			while (la.kind == 13) {
				lexer.NextToken();
				if (
#line  632 "cs.ATG" 
IsAssignment()) {

#line  632 "cs.ATG" 
					nameFound = true; 
					Expect(1);

#line  633 "cs.ATG" 
					name = t.val; 
					Expect(3);
				} else if (StartOf(4)) {

#line  635 "cs.ATG" 
					if (nameFound) Error("no positional argument after named argument"); 
				} else SynErr(126);
				Expr(
#line  636 "cs.ATG" 
out expr);

#line  636 "cs.ATG" 
				if (expr != null) { if(name == "") positional.Add(expr);
				else { named.Add(new NamedArgumentExpression(name, expr)); name = ""; }
				}
				
			}
		}
		Expect(20);
	}

	void Expr(
#line  1868 "cs.ATG" 
out Expression expr) {

#line  1869 "cs.ATG" 
		expr = null; Expression expr1 = null, expr2 = null; 
		UnaryExpr(
#line  1871 "cs.ATG" 
out expr);
		if (StartOf(5)) {
			ConditionalOrExpr(
#line  1874 "cs.ATG" 
ref expr);
			if (la.kind == 12) {
				lexer.NextToken();
				Expr(
#line  1874 "cs.ATG" 
out expr1);
				Expect(9);
				Expr(
#line  1874 "cs.ATG" 
out expr2);

#line  1874 "cs.ATG" 
				expr = new ConditionalExpression(expr, expr1, expr2);  
			}
		} else if (StartOf(6)) {

#line  1876 "cs.ATG" 
			AssignmentOperatorType op; Expression val; 
			AssignmentOperator(
#line  1876 "cs.ATG" 
out op);
			Expr(
#line  1876 "cs.ATG" 
out val);

#line  1876 "cs.ATG" 
			expr = new AssignmentExpression(expr, op, val); 
		} else SynErr(127);
	}

	void AttributeSection(
#line  645 "cs.ATG" 
out AttributeSection section) {

#line  647 "cs.ATG" 
		string attributeTarget = "";
		List<ASTAttribute> attributes = new List<ASTAttribute>();
		ASTAttribute attribute;
		
		
		Expect(17);

#line  653 "cs.ATG" 
		Point startPos = t.Location; 
		if (
#line  654 "cs.ATG" 
IsLocalAttrTarget()) {
			if (la.kind == 67) {
				lexer.NextToken();

#line  655 "cs.ATG" 
				attributeTarget = "event";
			} else if (la.kind == 99) {
				lexer.NextToken();

#line  656 "cs.ATG" 
				attributeTarget = "return";
			} else {
				lexer.NextToken();

#line  657 "cs.ATG" 
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
#line  667 "cs.ATG" 
out attribute);

#line  667 "cs.ATG" 
		attributes.Add(attribute); 
		while (
#line  668 "cs.ATG" 
NotFinalComma()) {
			Expect(13);
			Attribute(
#line  668 "cs.ATG" 
out attribute);

#line  668 "cs.ATG" 
			attributes.Add(attribute); 
		}
		if (la.kind == 13) {
			lexer.NextToken();
		}
		Expect(18);

#line  670 "cs.ATG" 
		section = new AttributeSection(attributeTarget, attributes);
		section.StartLocation = startPos;
		section.EndLocation = t.EndLocation;
		
	}

	void TypeModifier(
#line  994 "cs.ATG" 
Modifiers m) {
		switch (la.kind) {
		case 87: {
			lexer.NextToken();

#line  996 "cs.ATG" 
			m.Add(Modifier.New); 
			break;
		}
		case 96: {
			lexer.NextToken();

#line  997 "cs.ATG" 
			m.Add(Modifier.Public); 
			break;
		}
		case 95: {
			lexer.NextToken();

#line  998 "cs.ATG" 
			m.Add(Modifier.Protected); 
			break;
		}
		case 82: {
			lexer.NextToken();

#line  999 "cs.ATG" 
			m.Add(Modifier.Internal); 
			break;
		}
		case 94: {
			lexer.NextToken();

#line  1000 "cs.ATG" 
			m.Add(Modifier.Private); 
			break;
		}
		case 117: {
			lexer.NextToken();

#line  1001 "cs.ATG" 
			m.Add(Modifier.Unsafe); 
			break;
		}
		case 47: {
			lexer.NextToken();

#line  1002 "cs.ATG" 
			m.Add(Modifier.Abstract); 
			break;
		}
		case 101: {
			lexer.NextToken();

#line  1003 "cs.ATG" 
			m.Add(Modifier.Sealed); 
			break;
		}
		case 105: {
			lexer.NextToken();

#line  1004 "cs.ATG" 
			m.Add(Modifier.Static); 
			break;
		}
		case 1: {
			lexer.NextToken();

#line  1005 "cs.ATG" 
			if (t.val == "partial") { m.Add(Modifier.Partial); } 
			break;
		}
		default: SynErr(128); break;
		}
	}

	void TypeDecl(
#line  703 "cs.ATG" 
Modifiers m, List<AttributeSection> attributes) {

#line  705 "cs.ATG" 
		TypeReference type;
		List<TypeReference> names;
		List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
		string name;
		List<TemplateDefinition> templates;
		
		if (la.kind == 57) {

#line  711 "cs.ATG" 
			m.Check(Modifier.Classes); 
			lexer.NextToken();

#line  712 "cs.ATG" 
			TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
			templates = newType.Templates;
			compilationUnit.AddChild(newType);
			compilationUnit.BlockStart(newType);
			newType.StartLocation = t.Location;
			
			newType.Type = Types.Class;
			
			Expect(1);

#line  720 "cs.ATG" 
			newType.Name = t.val; 
			if (la.kind == 22) {
				TypeParameterList(
#line  723 "cs.ATG" 
templates);
			}
			if (la.kind == 9) {
				ClassBase(
#line  725 "cs.ATG" 
out names);

#line  725 "cs.ATG" 
				newType.BaseTypes = names; 
			}
			while (
#line  728 "cs.ATG" 
IdentIsWhere()) {
				TypeParameterConstraintsClause(
#line  728 "cs.ATG" 
templates);
			}
			ClassBody();
			if (la.kind == 11) {
				lexer.NextToken();
			}

#line  731 "cs.ATG" 
			newType.EndLocation = t.Location; 
			compilationUnit.BlockEnd();
			
		} else if (StartOf(7)) {

#line  734 "cs.ATG" 
			m.Check(Modifier.StructsInterfacesEnumsDelegates); 
			if (la.kind == 107) {
				lexer.NextToken();

#line  735 "cs.ATG" 
				TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
				templates = newType.Templates;
				newType.StartLocation = t.Location;
				compilationUnit.AddChild(newType);
				compilationUnit.BlockStart(newType);
				newType.Type = Types.Struct; 
				
				Expect(1);

#line  742 "cs.ATG" 
				newType.Name = t.val; 
				if (la.kind == 22) {
					TypeParameterList(
#line  745 "cs.ATG" 
templates);
				}
				if (la.kind == 9) {
					StructInterfaces(
#line  747 "cs.ATG" 
out names);

#line  747 "cs.ATG" 
					newType.BaseTypes = names; 
				}
				while (
#line  750 "cs.ATG" 
IdentIsWhere()) {
					TypeParameterConstraintsClause(
#line  750 "cs.ATG" 
templates);
				}
				StructBody();
				if (la.kind == 11) {
					lexer.NextToken();
				}

#line  754 "cs.ATG" 
				newType.EndLocation = t.Location; 
				compilationUnit.BlockEnd();
				
			} else if (la.kind == 81) {
				lexer.NextToken();

#line  758 "cs.ATG" 
				TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
				templates = newType.Templates;
				compilationUnit.AddChild(newType);
				compilationUnit.BlockStart(newType);
				newType.StartLocation = t.Location;
				newType.Type = Types.Interface;
				
				Expect(1);

#line  765 "cs.ATG" 
				newType.Name = t.val; 
				if (la.kind == 22) {
					TypeParameterList(
#line  768 "cs.ATG" 
templates);
				}
				if (la.kind == 9) {
					InterfaceBase(
#line  770 "cs.ATG" 
out names);

#line  770 "cs.ATG" 
					newType.BaseTypes = names; 
				}
				while (
#line  773 "cs.ATG" 
IdentIsWhere()) {
					TypeParameterConstraintsClause(
#line  773 "cs.ATG" 
templates);
				}
				InterfaceBody();
				if (la.kind == 11) {
					lexer.NextToken();
				}

#line  776 "cs.ATG" 
				newType.EndLocation = t.Location; 
				compilationUnit.BlockEnd();
				
			} else if (la.kind == 66) {
				lexer.NextToken();

#line  780 "cs.ATG" 
				TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
				compilationUnit.AddChild(newType);
				compilationUnit.BlockStart(newType);
				newType.StartLocation = t.Location;
				newType.Type = Types.Enum;
				
				Expect(1);

#line  786 "cs.ATG" 
				newType.Name = t.val; 
				if (la.kind == 9) {
					lexer.NextToken();
					IntegralType(
#line  787 "cs.ATG" 
out name);

#line  787 "cs.ATG" 
					newType.BaseTypes.Add(new TypeReference(name)); 
				}
				EnumBody();
				if (la.kind == 11) {
					lexer.NextToken();
				}

#line  790 "cs.ATG" 
				newType.EndLocation = t.Location; 
				compilationUnit.BlockEnd();
				
			} else {
				lexer.NextToken();

#line  794 "cs.ATG" 
				DelegateDeclaration delegateDeclr = new DelegateDeclaration(m.Modifier, attributes);
				templates = delegateDeclr.Templates;
				delegateDeclr.StartLocation = t.Location;
				
				if (
#line  798 "cs.ATG" 
NotVoidPointer()) {
					Expect(121);

#line  798 "cs.ATG" 
					delegateDeclr.ReturnType = new TypeReference("void", 0, null); 
				} else if (StartOf(8)) {
					Type(
#line  799 "cs.ATG" 
out type);

#line  799 "cs.ATG" 
					delegateDeclr.ReturnType = type; 
				} else SynErr(129);
				Expect(1);

#line  801 "cs.ATG" 
				delegateDeclr.Name = t.val; 
				if (la.kind == 22) {
					TypeParameterList(
#line  804 "cs.ATG" 
templates);
				}
				Expect(19);
				if (StartOf(9)) {
					FormalParameterList(
#line  806 "cs.ATG" 
p);

#line  806 "cs.ATG" 
					delegateDeclr.Parameters = p; 
				}
				Expect(20);
				while (
#line  810 "cs.ATG" 
IdentIsWhere()) {
					TypeParameterConstraintsClause(
#line  810 "cs.ATG" 
templates);
				}
				Expect(11);

#line  812 "cs.ATG" 
				delegateDeclr.EndLocation = t.Location;
				compilationUnit.AddChild(delegateDeclr);
				
			}
		} else SynErr(130);
	}

	void TypeParameterList(
#line  2251 "cs.ATG" 
List<TemplateDefinition> templates) {

#line  2253 "cs.ATG" 
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		
		Expect(22);
		while (la.kind == 17) {
			AttributeSection(
#line  2257 "cs.ATG" 
out section);

#line  2257 "cs.ATG" 
			attributes.Add(section); 
		}
		Expect(1);

#line  2258 "cs.ATG" 
		templates.Add(new TemplateDefinition(t.val, attributes)); 
		while (la.kind == 13) {
			lexer.NextToken();
			while (la.kind == 17) {
				AttributeSection(
#line  2259 "cs.ATG" 
out section);

#line  2259 "cs.ATG" 
				attributes.Add(section); 
			}
			Expect(1);

#line  2260 "cs.ATG" 
			templates.Add(new TemplateDefinition(t.val, attributes)); 
		}
		Expect(21);
	}

	void ClassBase(
#line  827 "cs.ATG" 
out List<TypeReference> names) {

#line  829 "cs.ATG" 
		TypeReference typeRef;
		names = new List<TypeReference>();
		
		Expect(9);
		ClassType(
#line  833 "cs.ATG" 
out typeRef);

#line  833 "cs.ATG" 
		if (typeRef != null) { names.Add(typeRef); } 
		while (la.kind == 13) {
			lexer.NextToken();
			TypeName(
#line  834 "cs.ATG" 
out typeRef);

#line  834 "cs.ATG" 
			if (typeRef != null) { names.Add(typeRef); } 
		}
	}

	void TypeParameterConstraintsClause(
#line  2264 "cs.ATG" 
List<TemplateDefinition> templates) {

#line  2265 "cs.ATG" 
		string name = ""; TypeReference type; 
		Expect(1);

#line  2267 "cs.ATG" 
		if (t.val != "where") Error("where expected"); 
		Expect(1);

#line  2268 "cs.ATG" 
		name = t.val; 
		Expect(9);
		TypeParameterConstraintsClauseBase(
#line  2270 "cs.ATG" 
out type);

#line  2271 "cs.ATG" 
		TemplateDefinition td = null;
		foreach (TemplateDefinition d in templates) {
			if (d.Name == name) {
				td = d;
				break;
			}
		}
		if ( td != null) { td.Bases.Add(type); }
		
		while (la.kind == 13) {
			lexer.NextToken();
			TypeParameterConstraintsClauseBase(
#line  2280 "cs.ATG" 
out type);

#line  2281 "cs.ATG" 
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

#line  838 "cs.ATG" 
		AttributeSection section; 
		Expect(15);
		while (StartOf(10)) {

#line  841 "cs.ATG" 
			List<AttributeSection> attributes = new List<AttributeSection>();
			Modifiers m = new Modifiers();
			
			while (la.kind == 17) {
				AttributeSection(
#line  844 "cs.ATG" 
out section);

#line  844 "cs.ATG" 
				attributes.Add(section); 
			}
			while (StartOf(11)) {
				MemberModifier(
#line  845 "cs.ATG" 
m);
			}
			ClassMemberDecl(
#line  846 "cs.ATG" 
m, attributes);
		}
		Expect(16);
	}

	void StructInterfaces(
#line  851 "cs.ATG" 
out List<TypeReference> names) {

#line  853 "cs.ATG" 
		TypeReference typeRef;
		names = new List<TypeReference>();
		
		Expect(9);
		TypeName(
#line  857 "cs.ATG" 
out typeRef);

#line  857 "cs.ATG" 
		if (typeRef != null) { names.Add(typeRef); } 
		while (la.kind == 13) {
			lexer.NextToken();
			TypeName(
#line  858 "cs.ATG" 
out typeRef);

#line  858 "cs.ATG" 
			if (typeRef != null) { names.Add(typeRef); } 
		}
	}

	void StructBody() {

#line  862 "cs.ATG" 
		AttributeSection section; 
		Expect(15);
		while (StartOf(12)) {

#line  865 "cs.ATG" 
			List<AttributeSection> attributes = new List<AttributeSection>();
			Modifiers m = new Modifiers();
			
			while (la.kind == 17) {
				AttributeSection(
#line  868 "cs.ATG" 
out section);

#line  868 "cs.ATG" 
				attributes.Add(section); 
			}
			while (StartOf(11)) {
				MemberModifier(
#line  869 "cs.ATG" 
m);
			}
			StructMemberDecl(
#line  870 "cs.ATG" 
m, attributes);
		}
		Expect(16);
	}

	void InterfaceBase(
#line  875 "cs.ATG" 
out List<TypeReference> names) {

#line  877 "cs.ATG" 
		TypeReference typeRef;
		names = new List<TypeReference>();
		
		Expect(9);
		TypeName(
#line  881 "cs.ATG" 
out typeRef);

#line  881 "cs.ATG" 
		if (typeRef != null) { names.Add(typeRef); } 
		while (la.kind == 13) {
			lexer.NextToken();
			TypeName(
#line  882 "cs.ATG" 
out typeRef);

#line  882 "cs.ATG" 
			if (typeRef != null) { names.Add(typeRef); } 
		}
	}

	void InterfaceBody() {
		Expect(15);
		while (StartOf(13)) {
			InterfaceMemberDecl();
		}
		Expect(16);
	}

	void IntegralType(
#line  1016 "cs.ATG" 
out string name) {

#line  1016 "cs.ATG" 
		name = ""; 
		switch (la.kind) {
		case 100: {
			lexer.NextToken();

#line  1018 "cs.ATG" 
			name = "sbyte"; 
			break;
		}
		case 52: {
			lexer.NextToken();

#line  1019 "cs.ATG" 
			name = "byte"; 
			break;
		}
		case 102: {
			lexer.NextToken();

#line  1020 "cs.ATG" 
			name = "short"; 
			break;
		}
		case 118: {
			lexer.NextToken();

#line  1021 "cs.ATG" 
			name = "ushort"; 
			break;
		}
		case 80: {
			lexer.NextToken();

#line  1022 "cs.ATG" 
			name = "int"; 
			break;
		}
		case 114: {
			lexer.NextToken();

#line  1023 "cs.ATG" 
			name = "uint"; 
			break;
		}
		case 85: {
			lexer.NextToken();

#line  1024 "cs.ATG" 
			name = "long"; 
			break;
		}
		case 115: {
			lexer.NextToken();

#line  1025 "cs.ATG" 
			name = "ulong"; 
			break;
		}
		case 55: {
			lexer.NextToken();

#line  1026 "cs.ATG" 
			name = "char"; 
			break;
		}
		default: SynErr(131); break;
		}
	}

	void EnumBody() {

#line  888 "cs.ATG" 
		FieldDeclaration f; 
		Expect(15);
		if (la.kind == 1 || la.kind == 17) {
			EnumMemberDecl(
#line  890 "cs.ATG" 
out f);

#line  890 "cs.ATG" 
			compilationUnit.AddChild(f); 
			while (
#line  891 "cs.ATG" 
NotFinalComma()) {
				Expect(13);
				EnumMemberDecl(
#line  891 "cs.ATG" 
out f);

#line  891 "cs.ATG" 
				compilationUnit.AddChild(f); 
			}
			if (la.kind == 13) {
				lexer.NextToken();
			}
		}
		Expect(16);
	}

	void Type(
#line  896 "cs.ATG" 
out TypeReference type) {

#line  898 "cs.ATG" 
		string name;
		int pointer = 0;
		type = null;
		
		if (la.kind == 1 || la.kind == 89 || la.kind == 106) {
			ClassType(
#line  903 "cs.ATG" 
out type);
		} else if (StartOf(14)) {
			SimpleType(
#line  904 "cs.ATG" 
out name);

#line  904 "cs.ATG" 
			type = new TypeReference(name); 
		} else if (la.kind == 121) {
			lexer.NextToken();
			Expect(6);

#line  905 "cs.ATG" 
			pointer = 1; type = new TypeReference("void"); 
		} else SynErr(132);

#line  906 "cs.ATG" 
		List<int> r = new List<int>(); 
		while (
#line  908 "cs.ATG" 
IsPointerOrDims()) {

#line  908 "cs.ATG" 
			int i = 0; 
			if (la.kind == 6) {
				lexer.NextToken();

#line  909 "cs.ATG" 
				++pointer; 
			} else if (la.kind == 17) {
				lexer.NextToken();
				while (la.kind == 13) {
					lexer.NextToken();

#line  910 "cs.ATG" 
					++i; 
				}
				Expect(18);

#line  910 "cs.ATG" 
				r.Add(i); 
			} else SynErr(133);
		}

#line  913 "cs.ATG" 
		if (type != null) {
		type.RankSpecifier = r.ToArray();
		type.PointerNestingLevel = pointer;
		  }
		
	}

	void FormalParameterList(
#line  949 "cs.ATG" 
List<ParameterDeclarationExpression> parameter) {

#line  952 "cs.ATG" 
		ParameterDeclarationExpression p;
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		
		while (la.kind == 17) {
			AttributeSection(
#line  957 "cs.ATG" 
out section);

#line  957 "cs.ATG" 
			attributes.Add(section); 
		}
		if (StartOf(15)) {
			FixedParameter(
#line  959 "cs.ATG" 
out p);

#line  959 "cs.ATG" 
			bool paramsFound = false;
			p.Attributes = attributes;
			parameter.Add(p);
			
			while (la.kind == 13) {
				lexer.NextToken();

#line  964 "cs.ATG" 
				attributes = new List<AttributeSection>(); if (paramsFound) Error("params array must be at end of parameter list"); 
				while (la.kind == 17) {
					AttributeSection(
#line  965 "cs.ATG" 
out section);

#line  965 "cs.ATG" 
					attributes.Add(section); 
				}
				if (StartOf(15)) {
					FixedParameter(
#line  967 "cs.ATG" 
out p);

#line  967 "cs.ATG" 
					p.Attributes = attributes; parameter.Add(p); 
				} else if (la.kind == 93) {
					ParameterArray(
#line  968 "cs.ATG" 
out p);

#line  968 "cs.ATG" 
					paramsFound = true; p.Attributes = attributes; parameter.Add(p); 
				} else SynErr(134);
			}
		} else if (la.kind == 93) {
			ParameterArray(
#line  971 "cs.ATG" 
out p);

#line  971 "cs.ATG" 
			p.Attributes = attributes; parameter.Add(p); 
		} else SynErr(135);
	}

	void ClassType(
#line  1008 "cs.ATG" 
out TypeReference typeRef) {

#line  1009 "cs.ATG" 
		TypeReference r; typeRef = null; 
		if (la.kind == 1) {
			TypeName(
#line  1011 "cs.ATG" 
out r);

#line  1011 "cs.ATG" 
			typeRef = r; 
		} else if (la.kind == 89) {
			lexer.NextToken();

#line  1012 "cs.ATG" 
			typeRef = new TypeReference("object"); 
		} else if (la.kind == 106) {
			lexer.NextToken();

#line  1013 "cs.ATG" 
			typeRef = new TypeReference("string"); 
		} else SynErr(136);
	}

	void TypeName(
#line  2216 "cs.ATG" 
out TypeReference typeRef) {

#line  2217 "cs.ATG" 
		List<TypeReference> typeArguments = null;
		string alias = null;
		string qualident;
		
		if (
#line  2222 "cs.ATG" 
la.kind == Tokens.Identifier && Peek(1).kind == Tokens.DoubleColon) {
			lexer.NextToken();

#line  2223 "cs.ATG" 
			alias = t.val; 
			Expect(10);
		}
		Qualident(
#line  2226 "cs.ATG" 
out qualident);
		if (la.kind == 22) {
			TypeArgumentList(
#line  2227 "cs.ATG" 
out typeArguments);
		}

#line  2229 "cs.ATG" 
		if (alias == null) {
		typeRef = new TypeReference(qualident, typeArguments);
		} else if (alias == "global") {
			typeRef = new TypeReference(qualident, typeArguments);
			typeRef.IsGlobal = true;
		} else {
			typeRef = new TypeReference(alias + "." + qualident, typeArguments);
		}
		
	}

	void MemberModifier(
#line  1029 "cs.ATG" 
Modifiers m) {
		switch (la.kind) {
		case 47: {
			lexer.NextToken();

#line  1031 "cs.ATG" 
			m.Add(Modifier.Abstract); 
			break;
		}
		case 69: {
			lexer.NextToken();

#line  1032 "cs.ATG" 
			m.Add(Modifier.Extern); 
			break;
		}
		case 82: {
			lexer.NextToken();

#line  1033 "cs.ATG" 
			m.Add(Modifier.Internal); 
			break;
		}
		case 87: {
			lexer.NextToken();

#line  1034 "cs.ATG" 
			m.Add(Modifier.New); 
			break;
		}
		case 92: {
			lexer.NextToken();

#line  1035 "cs.ATG" 
			m.Add(Modifier.Override); 
			break;
		}
		case 94: {
			lexer.NextToken();

#line  1036 "cs.ATG" 
			m.Add(Modifier.Private); 
			break;
		}
		case 95: {
			lexer.NextToken();

#line  1037 "cs.ATG" 
			m.Add(Modifier.Protected); 
			break;
		}
		case 96: {
			lexer.NextToken();

#line  1038 "cs.ATG" 
			m.Add(Modifier.Public); 
			break;
		}
		case 97: {
			lexer.NextToken();

#line  1039 "cs.ATG" 
			m.Add(Modifier.Readonly); 
			break;
		}
		case 101: {
			lexer.NextToken();

#line  1040 "cs.ATG" 
			m.Add(Modifier.Sealed); 
			break;
		}
		case 105: {
			lexer.NextToken();

#line  1041 "cs.ATG" 
			m.Add(Modifier.Static); 
			break;
		}
		case 117: {
			lexer.NextToken();

#line  1042 "cs.ATG" 
			m.Add(Modifier.Unsafe); 
			break;
		}
		case 120: {
			lexer.NextToken();

#line  1043 "cs.ATG" 
			m.Add(Modifier.Virtual); 
			break;
		}
		case 122: {
			lexer.NextToken();

#line  1044 "cs.ATG" 
			m.Add(Modifier.Volatile); 
			break;
		}
		default: SynErr(137); break;
		}
	}

	void ClassMemberDecl(
#line  1277 "cs.ATG" 
Modifiers m, List<AttributeSection> attributes) {

#line  1278 "cs.ATG" 
		Statement stmt = null; 
		if (StartOf(16)) {
			StructMemberDecl(
#line  1280 "cs.ATG" 
m, attributes);
		} else if (la.kind == 26) {

#line  1281 "cs.ATG" 
			m.Check(Modifier.Destructors); Point startPos = t.Location; 
			lexer.NextToken();
			Expect(1);

#line  1282 "cs.ATG" 
			DestructorDeclaration d = new DestructorDeclaration(t.val, m.Modifier, attributes); 
			d.Modifier = m.Modifier;
			d.StartLocation = startPos;
			
			Expect(19);
			Expect(20);

#line  1286 "cs.ATG" 
			d.EndLocation = t.EndLocation; 
			if (la.kind == 15) {
				Block(
#line  1286 "cs.ATG" 
out stmt);
			} else if (la.kind == 11) {
				lexer.NextToken();
			} else SynErr(138);

#line  1287 "cs.ATG" 
			d.Body = (BlockStatement)stmt;
			compilationUnit.AddChild(d);
			
		} else SynErr(139);
	}

	void StructMemberDecl(
#line  1047 "cs.ATG" 
Modifiers m, List<AttributeSection> attributes) {

#line  1049 "cs.ATG" 
		string qualident = null;
		TypeReference type;
		Expression expr;
		List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
		Statement stmt = null;
		List<VariableDeclaration> variableDeclarators = new List<VariableDeclaration>();
		List<TemplateDefinition> templates = new List<TemplateDefinition>();
		
		if (la.kind == 58) {

#line  1058 "cs.ATG" 
			m.Check(Modifier.Constants); 
			lexer.NextToken();

#line  1059 "cs.ATG" 
			Point startPos = t.Location; 
			Type(
#line  1060 "cs.ATG" 
out type);
			Expect(1);

#line  1060 "cs.ATG" 
			FieldDeclaration fd = new FieldDeclaration(attributes, type, m.Modifier | Modifier.Const);
			fd.StartLocation = startPos;
			VariableDeclaration f = new VariableDeclaration(t.val);
			fd.Fields.Add(f);
			
			Expect(3);
			Expr(
#line  1065 "cs.ATG" 
out expr);

#line  1065 "cs.ATG" 
			f.Initializer = expr; 
			while (la.kind == 13) {
				lexer.NextToken();
				Expect(1);

#line  1066 "cs.ATG" 
				f = new VariableDeclaration(t.val);
				fd.Fields.Add(f);
				
				Expect(3);
				Expr(
#line  1069 "cs.ATG" 
out expr);

#line  1069 "cs.ATG" 
				f.Initializer = expr; 
			}
			Expect(11);

#line  1070 "cs.ATG" 
			fd.EndLocation = t.EndLocation; compilationUnit.AddChild(fd); 
		} else if (
#line  1073 "cs.ATG" 
NotVoidPointer()) {

#line  1073 "cs.ATG" 
			m.Check(Modifier.PropertysEventsMethods); 
			Expect(121);

#line  1074 "cs.ATG" 
			Point startPos = t.Location; 
			Qualident(
#line  1075 "cs.ATG" 
out qualident);
			if (la.kind == 22) {
				TypeParameterList(
#line  1077 "cs.ATG" 
templates);
			}
			Expect(19);
			if (StartOf(9)) {
				FormalParameterList(
#line  1080 "cs.ATG" 
p);
			}
			Expect(20);

#line  1080 "cs.ATG" 
			MethodDeclaration methodDeclaration = new MethodDeclaration(qualident,
			                                                           m.Modifier,
			                                                           new TypeReference("void"),
			                                                           p,
			                                                           attributes);
			methodDeclaration.StartLocation = startPos;
			methodDeclaration.EndLocation   = t.EndLocation;
			methodDeclaration.Templates = templates;
			compilationUnit.AddChild(methodDeclaration);
			compilationUnit.BlockStart(methodDeclaration);
			
			while (
#line  1093 "cs.ATG" 
IdentIsWhere()) {
				TypeParameterConstraintsClause(
#line  1093 "cs.ATG" 
templates);
			}
			if (la.kind == 15) {
				Block(
#line  1095 "cs.ATG" 
out stmt);
			} else if (la.kind == 11) {
				lexer.NextToken();
			} else SynErr(140);

#line  1095 "cs.ATG" 
			compilationUnit.BlockEnd();
			methodDeclaration.Body  = (BlockStatement)stmt;
			
		} else if (la.kind == 67) {

#line  1099 "cs.ATG" 
			m.Check(Modifier.PropertysEventsMethods); 
			lexer.NextToken();

#line  1100 "cs.ATG" 
			EventDeclaration eventDecl = new EventDeclaration(m.Modifier, attributes);
			eventDecl.StartLocation = t.Location;
			compilationUnit.AddChild(eventDecl);
			compilationUnit.BlockStart(eventDecl);
			EventAddRegion addBlock = null;
			EventRemoveRegion removeBlock = null;
			
			Type(
#line  1107 "cs.ATG" 
out type);

#line  1107 "cs.ATG" 
			eventDecl.TypeReference = type; 
			if (
#line  1109 "cs.ATG" 
IsVarDecl()) {
				VariableDeclarator(
#line  1109 "cs.ATG" 
variableDeclarators);
				while (la.kind == 13) {
					lexer.NextToken();
					VariableDeclarator(
#line  1110 "cs.ATG" 
variableDeclarators);
				}
				Expect(11);

#line  1110 "cs.ATG" 
				eventDecl.VariableDeclarators = variableDeclarators; eventDecl.EndLocation = t.EndLocation;  
			} else if (la.kind == 1) {
				Qualident(
#line  1111 "cs.ATG" 
out qualident);

#line  1111 "cs.ATG" 
				eventDecl.Name = qualident; eventDecl.EndLocation = t.EndLocation;  
				Expect(15);

#line  1112 "cs.ATG" 
				eventDecl.BodyStart = t.Location; 
				EventAccessorDecls(
#line  1113 "cs.ATG" 
out addBlock, out removeBlock);
				Expect(16);

#line  1114 "cs.ATG" 
				eventDecl.BodyEnd   = t.EndLocation; 
			} else SynErr(141);

#line  1115 "cs.ATG" 
			compilationUnit.BlockEnd();
			
			eventDecl.AddRegion = addBlock;
			eventDecl.RemoveRegion = removeBlock;
			
		} else if (
#line  1122 "cs.ATG" 
IdentAndLPar()) {

#line  1122 "cs.ATG" 
			m.Check(Modifier.Constructors | Modifier.StaticConstructors); 
			Expect(1);

#line  1123 "cs.ATG" 
			string name = t.val; Point startPos = t.Location; 
			Expect(19);
			if (StartOf(9)) {

#line  1123 "cs.ATG" 
				m.Check(Modifier.Constructors); 
				FormalParameterList(
#line  1124 "cs.ATG" 
p);
			}
			Expect(20);

#line  1126 "cs.ATG" 
			ConstructorInitializer init = null;  
			if (la.kind == 9) {

#line  1127 "cs.ATG" 
				m.Check(Modifier.Constructors); 
				ConstructorInitializer(
#line  1128 "cs.ATG" 
out init);
			}

#line  1130 "cs.ATG" 
			ConstructorDeclaration cd = new ConstructorDeclaration(name, m.Modifier, p, init, attributes); 
			cd.StartLocation = startPos;
			cd.EndLocation   = t.EndLocation;
			
			if (la.kind == 15) {
				Block(
#line  1135 "cs.ATG" 
out stmt);
			} else if (la.kind == 11) {
				lexer.NextToken();
			} else SynErr(142);

#line  1135 "cs.ATG" 
			cd.Body = (BlockStatement)stmt; compilationUnit.AddChild(cd); 
		} else if (la.kind == 68 || la.kind == 78) {

#line  1138 "cs.ATG" 
			m.Check(Modifier.Operators);
			if (m.isNone) Error("at least one modifier must be set"); 
			bool isImplicit = true;
			
			if (la.kind == 78) {
				lexer.NextToken();
			} else {
				lexer.NextToken();

#line  1142 "cs.ATG" 
				isImplicit = false; 
			}
			Expect(90);
			Type(
#line  1143 "cs.ATG" 
out type);

#line  1143 "cs.ATG" 
			TypeReference operatorType = type; 
			Expect(19);
			Type(
#line  1144 "cs.ATG" 
out type);
			Expect(1);

#line  1144 "cs.ATG" 
			string varName = t.val; 
			Expect(20);
			if (la.kind == 15) {
				Block(
#line  1144 "cs.ATG" 
out stmt);
			} else if (la.kind == 11) {
				lexer.NextToken();

#line  1144 "cs.ATG" 
				stmt = null; 
			} else SynErr(143);

#line  1147 "cs.ATG" 
			List<ParameterDeclarationExpression> parameters = new List<ParameterDeclarationExpression>();
			parameters.Add(new ParameterDeclarationExpression(type, varName));
			OperatorDeclaration operatorDeclaration = new OperatorDeclaration(m.Modifier, 
			                                                                  attributes, 
			                                                                  parameters, 
			                                                                  operatorType,
			                                                                  isImplicit ? ConversionType.Implicit : ConversionType.Explicit
			                                                                  );
			operatorDeclaration.Body = (BlockStatement)stmt;
			compilationUnit.AddChild(operatorDeclaration);
			
		} else if (StartOf(17)) {
			TypeDecl(
#line  1160 "cs.ATG" 
m, attributes);
		} else if (StartOf(8)) {
			Type(
#line  1161 "cs.ATG" 
out type);

#line  1161 "cs.ATG" 
			Point startPos = t.Location;  
			if (la.kind == 90) {

#line  1163 "cs.ATG" 
				OverloadableOperatorType op;
				m.Check(Modifier.Operators);
				if (m.isNone) Error("at least one modifier must be set");
				
				lexer.NextToken();
				OverloadableOperator(
#line  1167 "cs.ATG" 
out op);

#line  1167 "cs.ATG" 
				TypeReference firstType, secondType = null; string secondName = null; 
				Expect(19);
				Type(
#line  1168 "cs.ATG" 
out firstType);
				Expect(1);

#line  1168 "cs.ATG" 
				string firstName = t.val; 
				if (la.kind == 13) {
					lexer.NextToken();
					Type(
#line  1169 "cs.ATG" 
out secondType);
					Expect(1);

#line  1169 "cs.ATG" 
					secondName = t.val; 
				} else if (la.kind == 20) {
				} else SynErr(144);
				Expect(20);
				if (la.kind == 15) {
					Block(
#line  1177 "cs.ATG" 
out stmt);
				} else if (la.kind == 11) {
					lexer.NextToken();
				} else SynErr(145);

#line  1179 "cs.ATG" 
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
				compilationUnit.AddChild(operatorDeclaration);
				
			} else if (
#line  1194 "cs.ATG" 
IsVarDecl()) {

#line  1194 "cs.ATG" 
				m.Check(Modifier.Fields); 
				FieldDeclaration fd = new FieldDeclaration(attributes, type, m.Modifier);
				fd.StartLocation = startPos; 
				
				VariableDeclarator(
#line  1198 "cs.ATG" 
variableDeclarators);
				while (la.kind == 13) {
					lexer.NextToken();
					VariableDeclarator(
#line  1199 "cs.ATG" 
variableDeclarators);
				}
				Expect(11);

#line  1200 "cs.ATG" 
				fd.EndLocation = t.EndLocation; fd.Fields = variableDeclarators; compilationUnit.AddChild(fd); 
			} else if (la.kind == 109) {

#line  1203 "cs.ATG" 
				m.Check(Modifier.Indexers); 
				lexer.NextToken();
				Expect(17);
				FormalParameterList(
#line  1204 "cs.ATG" 
p);
				Expect(18);

#line  1204 "cs.ATG" 
				Point endLocation = t.EndLocation; 
				Expect(15);

#line  1205 "cs.ATG" 
				IndexerDeclaration indexer = new IndexerDeclaration(type, p, m.Modifier, attributes);
				indexer.StartLocation = startPos;
				indexer.EndLocation   = endLocation;
				indexer.BodyStart     = t.Location;
				PropertyGetRegion getRegion;
				PropertySetRegion setRegion;
				
				AccessorDecls(
#line  1212 "cs.ATG" 
out getRegion, out setRegion);
				Expect(16);

#line  1213 "cs.ATG" 
				indexer.BodyEnd    = t.EndLocation;
				indexer.GetRegion = getRegion;
				indexer.SetRegion = setRegion;
				compilationUnit.AddChild(indexer);
				
			} else if (la.kind == 1) {
				Qualident(
#line  1218 "cs.ATG" 
out qualident);

#line  1218 "cs.ATG" 
				Point qualIdentEndLocation = t.EndLocation; 
				if (la.kind == 15 || la.kind == 19 || la.kind == 22) {
					if (la.kind == 19 || la.kind == 22) {

#line  1221 "cs.ATG" 
						m.Check(Modifier.PropertysEventsMethods); 
						if (la.kind == 22) {
							TypeParameterList(
#line  1223 "cs.ATG" 
templates);
						}
						Expect(19);
						if (StartOf(9)) {
							FormalParameterList(
#line  1224 "cs.ATG" 
p);
						}
						Expect(20);

#line  1225 "cs.ATG" 
						MethodDeclaration methodDeclaration = new MethodDeclaration(qualident,
						                                                           m.Modifier, 
						                                                           type, 
						                                                           p, 
						                                                           attributes);
						methodDeclaration.StartLocation = startPos;
						methodDeclaration.EndLocation   = t.EndLocation;
						methodDeclaration.Templates = templates;
						compilationUnit.AddChild(methodDeclaration);
						                                      
						while (
#line  1235 "cs.ATG" 
IdentIsWhere()) {
							TypeParameterConstraintsClause(
#line  1235 "cs.ATG" 
templates);
						}
						if (la.kind == 15) {
							Block(
#line  1236 "cs.ATG" 
out stmt);
						} else if (la.kind == 11) {
							lexer.NextToken();
						} else SynErr(146);

#line  1236 "cs.ATG" 
						methodDeclaration.Body  = (BlockStatement)stmt; 
					} else {
						lexer.NextToken();

#line  1239 "cs.ATG" 
						PropertyDeclaration pDecl = new PropertyDeclaration(qualident, type, m.Modifier, attributes); 
						pDecl.StartLocation = startPos;
						pDecl.EndLocation   = qualIdentEndLocation;
						pDecl.BodyStart   = t.Location;
						PropertyGetRegion getRegion;
						PropertySetRegion setRegion;
						
						AccessorDecls(
#line  1246 "cs.ATG" 
out getRegion, out setRegion);
						Expect(16);

#line  1248 "cs.ATG" 
						pDecl.GetRegion = getRegion;
						pDecl.SetRegion = setRegion;
						pDecl.BodyEnd = t.EndLocation;
						compilationUnit.AddChild(pDecl);
						
					}
				} else if (la.kind == 14) {

#line  1256 "cs.ATG" 
					m.Check(Modifier.Indexers); 
					lexer.NextToken();
					Expect(109);
					Expect(17);
					FormalParameterList(
#line  1257 "cs.ATG" 
p);
					Expect(18);

#line  1258 "cs.ATG" 
					IndexerDeclaration indexer = new IndexerDeclaration(type, p, m.Modifier, attributes);
					indexer.StartLocation = startPos;
					indexer.EndLocation   = t.EndLocation;
					indexer.NamespaceName = qualident;
					PropertyGetRegion getRegion;
					PropertySetRegion setRegion;
					
					Expect(15);

#line  1265 "cs.ATG" 
					Point bodyStart = t.Location; 
					AccessorDecls(
#line  1266 "cs.ATG" 
out getRegion, out setRegion);
					Expect(16);

#line  1267 "cs.ATG" 
					indexer.BodyStart = bodyStart;
					indexer.BodyEnd   = t.EndLocation;
					indexer.GetRegion = getRegion;
					indexer.SetRegion = setRegion;
					compilationUnit.AddChild(indexer);
					
				} else SynErr(147);
			} else SynErr(148);
		} else SynErr(149);
	}

	void InterfaceMemberDecl() {

#line  1294 "cs.ATG" 
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
		
		while (la.kind == 17) {
			AttributeSection(
#line  1307 "cs.ATG" 
out section);

#line  1307 "cs.ATG" 
			attributes.Add(section); 
		}
		if (la.kind == 87) {
			lexer.NextToken();

#line  1308 "cs.ATG" 
			mod = Modifier.New; startLocation = t.Location; 
		}
		if (
#line  1311 "cs.ATG" 
NotVoidPointer()) {
			Expect(121);

#line  1311 "cs.ATG" 
			if (startLocation.X == -1) startLocation = t.Location; 
			Expect(1);

#line  1311 "cs.ATG" 
			name = t.val; 
			if (la.kind == 22) {
				TypeParameterList(
#line  1312 "cs.ATG" 
templates);
			}
			Expect(19);
			if (StartOf(9)) {
				FormalParameterList(
#line  1313 "cs.ATG" 
parameters);
			}
			Expect(20);
			while (
#line  1314 "cs.ATG" 
IdentIsWhere()) {
				TypeParameterConstraintsClause(
#line  1314 "cs.ATG" 
templates);
			}
			Expect(11);

#line  1316 "cs.ATG" 
			MethodDeclaration md = new MethodDeclaration(name, mod, new TypeReference("void"), parameters, attributes);
			md.StartLocation = startLocation;
			md.EndLocation = t.EndLocation;
			md.Templates = templates;
			compilationUnit.AddChild(md);
			
		} else if (StartOf(18)) {
			if (StartOf(8)) {
				Type(
#line  1323 "cs.ATG" 
out type);

#line  1323 "cs.ATG" 
				if (startLocation.X == -1) startLocation = t.Location; 
				if (la.kind == 1) {
					lexer.NextToken();

#line  1325 "cs.ATG" 
					name = t.val; Point qualIdentEndLocation = t.EndLocation; 
					if (la.kind == 19 || la.kind == 22) {
						if (la.kind == 22) {
							TypeParameterList(
#line  1329 "cs.ATG" 
templates);
						}
						Expect(19);
						if (StartOf(9)) {
							FormalParameterList(
#line  1330 "cs.ATG" 
parameters);
						}
						Expect(20);
						while (
#line  1332 "cs.ATG" 
IdentIsWhere()) {
							TypeParameterConstraintsClause(
#line  1332 "cs.ATG" 
templates);
						}
						Expect(11);

#line  1333 "cs.ATG" 
						MethodDeclaration md = new MethodDeclaration(name, mod, type, parameters, attributes);
						md.StartLocation = startLocation;
						md.EndLocation = t.EndLocation;
						md.Templates = templates;
						compilationUnit.AddChild(md);
						
					} else if (la.kind == 15) {

#line  1340 "cs.ATG" 
						PropertyDeclaration pd = new PropertyDeclaration(name, type, mod, attributes); compilationUnit.AddChild(pd); 
						lexer.NextToken();

#line  1341 "cs.ATG" 
						Point bodyStart = t.Location;
						InterfaceAccessors(
#line  1341 "cs.ATG" 
out getBlock, out setBlock);
						Expect(16);

#line  1341 "cs.ATG" 
						pd.GetRegion = getBlock; pd.SetRegion = setBlock; pd.StartLocation = startLocation; pd.EndLocation = qualIdentEndLocation; pd.BodyStart = bodyStart; pd.BodyEnd = t.EndLocation; 
					} else SynErr(150);
				} else if (la.kind == 109) {
					lexer.NextToken();
					Expect(17);
					FormalParameterList(
#line  1344 "cs.ATG" 
parameters);
					Expect(18);

#line  1344 "cs.ATG" 
					Point bracketEndLocation = t.EndLocation; 

#line  1344 "cs.ATG" 
					IndexerDeclaration id = new IndexerDeclaration(type, parameters, mod, attributes); compilationUnit.AddChild(id); 
					Expect(15);

#line  1345 "cs.ATG" 
					Point bodyStart = t.Location;
					InterfaceAccessors(
#line  1345 "cs.ATG" 
out getBlock, out setBlock);
					Expect(16);

#line  1345 "cs.ATG" 
					id.GetRegion = getBlock; id.SetRegion = setBlock; id.StartLocation = startLocation;  id.EndLocation = bracketEndLocation; id.BodyStart = bodyStart; id.BodyEnd = t.EndLocation;
				} else SynErr(151);
			} else {
				lexer.NextToken();

#line  1348 "cs.ATG" 
				if (startLocation.X == -1) startLocation = t.Location; 
				Type(
#line  1348 "cs.ATG" 
out type);
				Expect(1);

#line  1348 "cs.ATG" 
				EventDeclaration ed = new EventDeclaration(type, t.val, mod, attributes);
				compilationUnit.AddChild(ed);
				
				Expect(11);

#line  1351 "cs.ATG" 
				ed.StartLocation = startLocation; ed.EndLocation = t.EndLocation; 
			}
		} else SynErr(152);
	}

	void EnumMemberDecl(
#line  1356 "cs.ATG" 
out FieldDeclaration f) {

#line  1358 "cs.ATG" 
		Expression expr = null;
		List<AttributeSection> attributes = new List<AttributeSection>();
		AttributeSection section = null;
		VariableDeclaration varDecl = null;
		
		while (la.kind == 17) {
			AttributeSection(
#line  1364 "cs.ATG" 
out section);

#line  1364 "cs.ATG" 
			attributes.Add(section); 
		}
		Expect(1);

#line  1365 "cs.ATG" 
		f = new FieldDeclaration(attributes);
		varDecl         = new VariableDeclaration(t.val);
		f.Fields.Add(varDecl);
		f.StartLocation = t.Location;
		
		if (la.kind == 3) {
			lexer.NextToken();
			Expr(
#line  1370 "cs.ATG" 
out expr);

#line  1370 "cs.ATG" 
			varDecl.Initializer = expr; 
		}
	}

	void SimpleType(
#line  938 "cs.ATG" 
out string name) {

#line  939 "cs.ATG" 
		name = String.Empty; 
		if (StartOf(19)) {
			IntegralType(
#line  941 "cs.ATG" 
out name);
		} else if (la.kind == 73) {
			lexer.NextToken();

#line  942 "cs.ATG" 
			name = "float"; 
		} else if (la.kind == 64) {
			lexer.NextToken();

#line  943 "cs.ATG" 
			name = "double"; 
		} else if (la.kind == 60) {
			lexer.NextToken();

#line  944 "cs.ATG" 
			name = "decimal"; 
		} else if (la.kind == 50) {
			lexer.NextToken();

#line  945 "cs.ATG" 
			name = "bool"; 
		} else SynErr(153);
	}

	void NonArrayType(
#line  920 "cs.ATG" 
out TypeReference type) {

#line  922 "cs.ATG" 
		string name;
		int pointer = 0;
		type = null;
		
		if (la.kind == 1 || la.kind == 89 || la.kind == 106) {
			ClassType(
#line  927 "cs.ATG" 
out type);
		} else if (StartOf(14)) {
			SimpleType(
#line  928 "cs.ATG" 
out name);

#line  928 "cs.ATG" 
			type = new TypeReference(name); 
		} else if (la.kind == 121) {
			lexer.NextToken();
			Expect(6);

#line  929 "cs.ATG" 
			pointer = 1; type = new TypeReference("void"); 
		} else SynErr(154);
		while (
#line  932 "cs.ATG" 
IsPointer()) {
			Expect(6);

#line  933 "cs.ATG" 
			++pointer; 
		}

#line  935 "cs.ATG" 
		if (type != null) { type.PointerNestingLevel = pointer; } 
	}

	void FixedParameter(
#line  975 "cs.ATG" 
out ParameterDeclarationExpression p) {

#line  977 "cs.ATG" 
		TypeReference type;
		ParamModifier mod = ParamModifier.In;
		
		if (la.kind == 91 || la.kind == 98) {
			if (la.kind == 98) {
				lexer.NextToken();

#line  982 "cs.ATG" 
				mod = ParamModifier.Ref; 
			} else {
				lexer.NextToken();

#line  983 "cs.ATG" 
				mod = ParamModifier.Out; 
			}
		}
		Type(
#line  985 "cs.ATG" 
out type);
		Expect(1);

#line  985 "cs.ATG" 
		p = new ParameterDeclarationExpression(type, t.val, mod); 
	}

	void ParameterArray(
#line  988 "cs.ATG" 
out ParameterDeclarationExpression p) {

#line  989 "cs.ATG" 
		TypeReference type; 
		Expect(93);
		Type(
#line  991 "cs.ATG" 
out type);
		Expect(1);

#line  991 "cs.ATG" 
		p = new ParameterDeclarationExpression(type, t.val, ParamModifier.Params); 
	}

	void Block(
#line  1474 "cs.ATG" 
out Statement stmt) {
		Expect(15);

#line  1476 "cs.ATG" 
		BlockStatement blockStmt = new BlockStatement();
		blockStmt.StartLocation = t.EndLocation;
		compilationUnit.BlockStart(blockStmt);
		if (!parseMethodContents) lexer.SkipCurrentBlock();
		
		while (StartOf(20)) {
			Statement();
		}
		Expect(16);

#line  1483 "cs.ATG" 
		stmt = blockStmt;
		blockStmt.EndLocation = t.EndLocation;
		compilationUnit.BlockEnd();
		
	}

	void VariableDeclarator(
#line  1467 "cs.ATG" 
List<VariableDeclaration> fieldDeclaration) {

#line  1468 "cs.ATG" 
		Expression expr = null; 
		Expect(1);

#line  1470 "cs.ATG" 
		VariableDeclaration f = new VariableDeclaration(t.val); 
		if (la.kind == 3) {
			lexer.NextToken();
			VariableInitializer(
#line  1471 "cs.ATG" 
out expr);

#line  1471 "cs.ATG" 
			f.Initializer = expr; 
		}

#line  1471 "cs.ATG" 
		fieldDeclaration.Add(f); 
	}

	void EventAccessorDecls(
#line  1416 "cs.ATG" 
out EventAddRegion addBlock, out EventRemoveRegion removeBlock) {

#line  1417 "cs.ATG" 
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		Statement stmt;
		addBlock = null;
		removeBlock = null;
		
		while (la.kind == 17) {
			AttributeSection(
#line  1424 "cs.ATG" 
out section);

#line  1424 "cs.ATG" 
			attributes.Add(section); 
		}
		if (
#line  1426 "cs.ATG" 
IdentIsAdd()) {

#line  1426 "cs.ATG" 
			addBlock = new EventAddRegion(attributes); 
			AddAccessorDecl(
#line  1427 "cs.ATG" 
out stmt);

#line  1427 "cs.ATG" 
			attributes = new List<AttributeSection>(); addBlock.Block = (BlockStatement)stmt; 
			while (la.kind == 17) {
				AttributeSection(
#line  1428 "cs.ATG" 
out section);

#line  1428 "cs.ATG" 
				attributes.Add(section); 
			}
			RemoveAccessorDecl(
#line  1429 "cs.ATG" 
out stmt);

#line  1429 "cs.ATG" 
			removeBlock = new EventRemoveRegion(attributes); removeBlock.Block = (BlockStatement)stmt; 
		} else if (
#line  1430 "cs.ATG" 
IdentIsRemove()) {
			RemoveAccessorDecl(
#line  1431 "cs.ATG" 
out stmt);

#line  1431 "cs.ATG" 
			removeBlock = new EventRemoveRegion(attributes); removeBlock.Block = (BlockStatement)stmt; attributes = new List<AttributeSection>(); 
			while (la.kind == 17) {
				AttributeSection(
#line  1432 "cs.ATG" 
out section);

#line  1432 "cs.ATG" 
				attributes.Add(section); 
			}
			AddAccessorDecl(
#line  1433 "cs.ATG" 
out stmt);

#line  1433 "cs.ATG" 
			addBlock = new EventAddRegion(attributes); addBlock.Block = (BlockStatement)stmt; 
		} else if (la.kind == 1) {
			lexer.NextToken();

#line  1434 "cs.ATG" 
			Error("add or remove accessor declaration expected"); 
		} else SynErr(155);
	}

	void ConstructorInitializer(
#line  1505 "cs.ATG" 
out ConstructorInitializer ci) {

#line  1506 "cs.ATG" 
		Expression expr; ci = new ConstructorInitializer(); 
		Expect(9);
		if (la.kind == 49) {
			lexer.NextToken();

#line  1510 "cs.ATG" 
			ci.ConstructorInitializerType = ConstructorInitializerType.Base; 
		} else if (la.kind == 109) {
			lexer.NextToken();

#line  1511 "cs.ATG" 
			ci.ConstructorInitializerType = ConstructorInitializerType.This; 
		} else SynErr(156);
		Expect(19);
		if (StartOf(21)) {
			Argument(
#line  1514 "cs.ATG" 
out expr);

#line  1514 "cs.ATG" 
			if (expr != null) { ci.Arguments.Add(expr); } 
			while (la.kind == 13) {
				lexer.NextToken();
				Argument(
#line  1514 "cs.ATG" 
out expr);

#line  1514 "cs.ATG" 
				if (expr != null) { ci.Arguments.Add(expr); } 
			}
		}
		Expect(20);
	}

	void OverloadableOperator(
#line  1526 "cs.ATG" 
out OverloadableOperatorType op) {

#line  1527 "cs.ATG" 
		op = OverloadableOperatorType.None; 
		switch (la.kind) {
		case 4: {
			lexer.NextToken();

#line  1529 "cs.ATG" 
			op = OverloadableOperatorType.Add; 
			break;
		}
		case 5: {
			lexer.NextToken();

#line  1530 "cs.ATG" 
			op = OverloadableOperatorType.Subtract; 
			break;
		}
		case 23: {
			lexer.NextToken();

#line  1532 "cs.ATG" 
			op = OverloadableOperatorType.Not; 
			break;
		}
		case 26: {
			lexer.NextToken();

#line  1533 "cs.ATG" 
			op = OverloadableOperatorType.BitNot; 
			break;
		}
		case 30: {
			lexer.NextToken();

#line  1535 "cs.ATG" 
			op = OverloadableOperatorType.Increment; 
			break;
		}
		case 31: {
			lexer.NextToken();

#line  1536 "cs.ATG" 
			op = OverloadableOperatorType.Decrement; 
			break;
		}
		case 111: {
			lexer.NextToken();

#line  1538 "cs.ATG" 
			op = OverloadableOperatorType.True; 
			break;
		}
		case 70: {
			lexer.NextToken();

#line  1539 "cs.ATG" 
			op = OverloadableOperatorType.False; 
			break;
		}
		case 6: {
			lexer.NextToken();

#line  1541 "cs.ATG" 
			op = OverloadableOperatorType.Multiply; 
			break;
		}
		case 7: {
			lexer.NextToken();

#line  1542 "cs.ATG" 
			op = OverloadableOperatorType.Divide; 
			break;
		}
		case 8: {
			lexer.NextToken();

#line  1543 "cs.ATG" 
			op = OverloadableOperatorType.Modulus; 
			break;
		}
		case 27: {
			lexer.NextToken();

#line  1545 "cs.ATG" 
			op = OverloadableOperatorType.BitwiseAnd; 
			break;
		}
		case 28: {
			lexer.NextToken();

#line  1546 "cs.ATG" 
			op = OverloadableOperatorType.BitwiseOr; 
			break;
		}
		case 29: {
			lexer.NextToken();

#line  1547 "cs.ATG" 
			op = OverloadableOperatorType.ExclusiveOr; 
			break;
		}
		case 36: {
			lexer.NextToken();

#line  1549 "cs.ATG" 
			op = OverloadableOperatorType.ShiftLeft; 
			break;
		}
		case 32: {
			lexer.NextToken();

#line  1550 "cs.ATG" 
			op = OverloadableOperatorType.Equality; 
			break;
		}
		case 33: {
			lexer.NextToken();

#line  1551 "cs.ATG" 
			op = OverloadableOperatorType.InEquality; 
			break;
		}
		case 22: {
			lexer.NextToken();

#line  1552 "cs.ATG" 
			op = OverloadableOperatorType.LessThan; 
			break;
		}
		case 34: {
			lexer.NextToken();

#line  1553 "cs.ATG" 
			op = OverloadableOperatorType.GreaterThanOrEqual; 
			break;
		}
		case 35: {
			lexer.NextToken();

#line  1554 "cs.ATG" 
			op = OverloadableOperatorType.LessThanOrEqual; 
			break;
		}
		case 21: {
			lexer.NextToken();

#line  1555 "cs.ATG" 
			op = OverloadableOperatorType.GreaterThan; 
			if (la.kind == 21) {
				lexer.NextToken();

#line  1555 "cs.ATG" 
				op = OverloadableOperatorType.ShiftRight; 
			}
			break;
		}
		default: SynErr(157); break;
		}
	}

	void AccessorDecls(
#line  1374 "cs.ATG" 
out PropertyGetRegion getBlock, out PropertySetRegion setBlock) {

#line  1376 "cs.ATG" 
		List<AttributeSection> attributes = new List<AttributeSection>(); 
		AttributeSection section;
		getBlock = null;
		setBlock = null; 
		
		while (la.kind == 17) {
			AttributeSection(
#line  1382 "cs.ATG" 
out section);

#line  1382 "cs.ATG" 
			attributes.Add(section); 
		}
		if (
#line  1384 "cs.ATG" 
IdentIsGet()) {
			GetAccessorDecl(
#line  1385 "cs.ATG" 
out getBlock, attributes);
			if (la.kind == 1 || la.kind == 17) {

#line  1386 "cs.ATG" 
				attributes = new List<AttributeSection>(); 
				while (la.kind == 17) {
					AttributeSection(
#line  1387 "cs.ATG" 
out section);

#line  1387 "cs.ATG" 
					attributes.Add(section); 
				}
				SetAccessorDecl(
#line  1388 "cs.ATG" 
out setBlock, attributes);
			}
		} else if (
#line  1390 "cs.ATG" 
IdentIsSet()) {
			SetAccessorDecl(
#line  1391 "cs.ATG" 
out setBlock, attributes);
			if (la.kind == 1 || la.kind == 17) {

#line  1392 "cs.ATG" 
				attributes = new List<AttributeSection>(); 
				while (la.kind == 17) {
					AttributeSection(
#line  1393 "cs.ATG" 
out section);

#line  1393 "cs.ATG" 
					attributes.Add(section); 
				}
				GetAccessorDecl(
#line  1394 "cs.ATG" 
out getBlock, attributes);
			}
		} else if (la.kind == 1) {
			lexer.NextToken();

#line  1396 "cs.ATG" 
			Error("get or set accessor declaration expected"); 
		} else SynErr(158);
	}

	void InterfaceAccessors(
#line  1438 "cs.ATG" 
out PropertyGetRegion getBlock, out PropertySetRegion setBlock) {

#line  1440 "cs.ATG" 
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		getBlock = null; setBlock = null;
		
		while (la.kind == 17) {
			AttributeSection(
#line  1445 "cs.ATG" 
out section);

#line  1445 "cs.ATG" 
			attributes.Add(section); 
		}
		if (
#line  1447 "cs.ATG" 
IdentIsGet()) {
			Expect(1);

#line  1447 "cs.ATG" 
			getBlock = new PropertyGetRegion(null, attributes); 
		} else if (
#line  1448 "cs.ATG" 
IdentIsSet()) {
			Expect(1);

#line  1448 "cs.ATG" 
			setBlock = new PropertySetRegion(null, attributes); 
		} else if (la.kind == 1) {
			lexer.NextToken();

#line  1449 "cs.ATG" 
			Error("set or get expected"); 
		} else SynErr(159);
		Expect(11);

#line  1451 "cs.ATG" 
		attributes = new List<AttributeSection>(); 
		if (la.kind == 1 || la.kind == 17) {
			while (la.kind == 17) {
				AttributeSection(
#line  1453 "cs.ATG" 
out section);

#line  1453 "cs.ATG" 
				attributes.Add(section); 
			}
			if (
#line  1455 "cs.ATG" 
IdentIsGet()) {
				Expect(1);

#line  1455 "cs.ATG" 
				if (getBlock != null) Error("get already declared");
				else getBlock = new PropertyGetRegion(null, attributes);
				
			} else if (
#line  1458 "cs.ATG" 
IdentIsSet()) {
				Expect(1);

#line  1458 "cs.ATG" 
				if (setBlock != null) Error("set already declared");
				else setBlock = new PropertySetRegion(null, attributes);
				
			} else if (la.kind == 1) {
				lexer.NextToken();

#line  1461 "cs.ATG" 
				Error("set or get expected"); 
			} else SynErr(160);
			Expect(11);
		}
	}

	void GetAccessorDecl(
#line  1400 "cs.ATG" 
out PropertyGetRegion getBlock, List<AttributeSection> attributes) {

#line  1401 "cs.ATG" 
		Statement stmt = null; 
		Expect(1);

#line  1404 "cs.ATG" 
		if (t.val != "get") Error("get expected"); 
		if (la.kind == 15) {
			Block(
#line  1405 "cs.ATG" 
out stmt);
		} else if (la.kind == 11) {
			lexer.NextToken();
		} else SynErr(161);

#line  1405 "cs.ATG" 
		getBlock = new PropertyGetRegion((BlockStatement)stmt, attributes); 
	}

	void SetAccessorDecl(
#line  1408 "cs.ATG" 
out PropertySetRegion setBlock, List<AttributeSection> attributes) {

#line  1409 "cs.ATG" 
		Statement stmt = null; 
		Expect(1);

#line  1412 "cs.ATG" 
		if (t.val != "set") Error("set expected"); 
		if (la.kind == 15) {
			Block(
#line  1413 "cs.ATG" 
out stmt);
		} else if (la.kind == 11) {
			lexer.NextToken();
		} else SynErr(162);

#line  1413 "cs.ATG" 
		setBlock = new PropertySetRegion((BlockStatement)stmt, attributes); 
	}

	void AddAccessorDecl(
#line  1489 "cs.ATG" 
out Statement stmt) {

#line  1490 "cs.ATG" 
		stmt = null;
		Expect(1);

#line  1493 "cs.ATG" 
		if (t.val != "add") Error("add expected"); 
		Block(
#line  1494 "cs.ATG" 
out stmt);
	}

	void RemoveAccessorDecl(
#line  1497 "cs.ATG" 
out Statement stmt) {

#line  1498 "cs.ATG" 
		stmt = null;
		Expect(1);

#line  1501 "cs.ATG" 
		if (t.val != "remove") Error("remove expected"); 
		Block(
#line  1502 "cs.ATG" 
out stmt);
	}

	void VariableInitializer(
#line  1518 "cs.ATG" 
out Expression initializerExpression) {

#line  1519 "cs.ATG" 
		TypeReference type = null; Expression expr = null; initializerExpression = null; 
		if (StartOf(4)) {
			Expr(
#line  1521 "cs.ATG" 
out initializerExpression);
		} else if (la.kind == 15) {
			ArrayInitializer(
#line  1522 "cs.ATG" 
out initializerExpression);
		} else if (la.kind == 104) {
			lexer.NextToken();
			Type(
#line  1523 "cs.ATG" 
out type);
			Expect(17);
			Expr(
#line  1523 "cs.ATG" 
out expr);
			Expect(18);

#line  1523 "cs.ATG" 
			initializerExpression = new StackAllocExpression(type, expr); 
		} else SynErr(163);
	}

	void Statement() {

#line  1627 "cs.ATG" 
		TypeReference type;
		Expression expr;
		Statement stmt = null;
		Point startPos = la.Location;
		
		if (
#line  1635 "cs.ATG" 
IsLabel()) {
			Expect(1);

#line  1635 "cs.ATG" 
			compilationUnit.AddChild(new LabelStatement(t.val)); 
			Expect(9);
			Statement();
		} else if (la.kind == 58) {
			lexer.NextToken();
			Type(
#line  1638 "cs.ATG" 
out type);

#line  1638 "cs.ATG" 
			LocalVariableDeclaration var = new LocalVariableDeclaration(type, Modifier.Const); string ident = null; var.StartLocation = t.Location; 
			Expect(1);

#line  1639 "cs.ATG" 
			ident = t.val; 
			Expect(3);
			Expr(
#line  1640 "cs.ATG" 
out expr);

#line  1640 "cs.ATG" 
			var.Variables.Add(new VariableDeclaration(ident, expr)); 
			while (la.kind == 13) {
				lexer.NextToken();
				Expect(1);

#line  1641 "cs.ATG" 
				ident = t.val; 
				Expect(3);
				Expr(
#line  1641 "cs.ATG" 
out expr);

#line  1641 "cs.ATG" 
				var.Variables.Add(new VariableDeclaration(ident, expr)); 
			}
			Expect(11);

#line  1642 "cs.ATG" 
			compilationUnit.AddChild(var); 
		} else if (
#line  1644 "cs.ATG" 
IsLocalVarDecl()) {
			LocalVariableDecl(
#line  1644 "cs.ATG" 
out stmt);
			Expect(11);

#line  1644 "cs.ATG" 
			compilationUnit.AddChild(stmt); 
		} else if (StartOf(22)) {
			EmbeddedStatement(
#line  1645 "cs.ATG" 
out stmt);

#line  1645 "cs.ATG" 
			compilationUnit.AddChild(stmt); 
		} else SynErr(164);

#line  1651 "cs.ATG" 
		if (stmt != null) {
		stmt.StartLocation = startPos;
		stmt.EndLocation = t.EndLocation;
		}
		
	}

	void Argument(
#line  1558 "cs.ATG" 
out Expression argumentexpr) {

#line  1560 "cs.ATG" 
		Expression expr;
		FieldDirection fd = FieldDirection.None;
		
		if (la.kind == 91 || la.kind == 98) {
			if (la.kind == 98) {
				lexer.NextToken();

#line  1565 "cs.ATG" 
				fd = FieldDirection.Ref; 
			} else {
				lexer.NextToken();

#line  1566 "cs.ATG" 
				fd = FieldDirection.Out; 
			}
		}
		Expr(
#line  1568 "cs.ATG" 
out expr);

#line  1568 "cs.ATG" 
		argumentexpr = fd != FieldDirection.None ? argumentexpr = new DirectionExpression(fd, expr) : expr; 
	}

	void ArrayInitializer(
#line  1587 "cs.ATG" 
out Expression outExpr) {

#line  1589 "cs.ATG" 
		Expression expr = null;
		ArrayInitializerExpression initializer = new ArrayInitializerExpression();
		
		Expect(15);
		if (StartOf(23)) {
			VariableInitializer(
#line  1594 "cs.ATG" 
out expr);

#line  1594 "cs.ATG" 
			if (expr != null) { initializer.CreateExpressions.Add(expr); } 
			while (
#line  1594 "cs.ATG" 
NotFinalComma()) {
				Expect(13);
				VariableInitializer(
#line  1594 "cs.ATG" 
out expr);

#line  1594 "cs.ATG" 
				if (expr != null) { initializer.CreateExpressions.Add(expr); } 
			}
			if (la.kind == 13) {
				lexer.NextToken();
			}
		}
		Expect(16);

#line  1595 "cs.ATG" 
		outExpr = initializer; 
	}

	void AssignmentOperator(
#line  1571 "cs.ATG" 
out AssignmentOperatorType op) {

#line  1572 "cs.ATG" 
		op = AssignmentOperatorType.None; 
		switch (la.kind) {
		case 3: {
			lexer.NextToken();

#line  1574 "cs.ATG" 
			op = AssignmentOperatorType.Assign; 
			break;
		}
		case 37: {
			lexer.NextToken();

#line  1575 "cs.ATG" 
			op = AssignmentOperatorType.Add; 
			break;
		}
		case 38: {
			lexer.NextToken();

#line  1576 "cs.ATG" 
			op = AssignmentOperatorType.Subtract; 
			break;
		}
		case 39: {
			lexer.NextToken();

#line  1577 "cs.ATG" 
			op = AssignmentOperatorType.Multiply; 
			break;
		}
		case 40: {
			lexer.NextToken();

#line  1578 "cs.ATG" 
			op = AssignmentOperatorType.Divide; 
			break;
		}
		case 41: {
			lexer.NextToken();

#line  1579 "cs.ATG" 
			op = AssignmentOperatorType.Modulus; 
			break;
		}
		case 42: {
			lexer.NextToken();

#line  1580 "cs.ATG" 
			op = AssignmentOperatorType.BitwiseAnd; 
			break;
		}
		case 43: {
			lexer.NextToken();

#line  1581 "cs.ATG" 
			op = AssignmentOperatorType.BitwiseOr; 
			break;
		}
		case 44: {
			lexer.NextToken();

#line  1582 "cs.ATG" 
			op = AssignmentOperatorType.ExclusiveOr; 
			break;
		}
		case 45: {
			lexer.NextToken();

#line  1583 "cs.ATG" 
			op = AssignmentOperatorType.ShiftLeft; 
			break;
		}
		case 21: {
			lexer.NextToken();
			Expect(34);

#line  1584 "cs.ATG" 
			op = AssignmentOperatorType.ShiftRight; 
			break;
		}
		default: SynErr(165); break;
		}
	}

	void LocalVariableDecl(
#line  1598 "cs.ATG" 
out Statement stmt) {

#line  1600 "cs.ATG" 
		TypeReference type;
		VariableDeclaration      var = null;
		LocalVariableDeclaration localVariableDeclaration; 
		
		Type(
#line  1605 "cs.ATG" 
out type);

#line  1605 "cs.ATG" 
		localVariableDeclaration = new LocalVariableDeclaration(type); localVariableDeclaration.StartLocation = t.Location; 
		LocalVariableDeclarator(
#line  1606 "cs.ATG" 
out var);

#line  1606 "cs.ATG" 
		localVariableDeclaration.Variables.Add(var); 
		while (la.kind == 13) {
			lexer.NextToken();
			LocalVariableDeclarator(
#line  1607 "cs.ATG" 
out var);

#line  1607 "cs.ATG" 
			localVariableDeclaration.Variables.Add(var); 
		}

#line  1608 "cs.ATG" 
		stmt = localVariableDeclaration; 
	}

	void LocalVariableDeclarator(
#line  1611 "cs.ATG" 
out VariableDeclaration var) {

#line  1612 "cs.ATG" 
		Expression expr = null; 
		Expect(1);

#line  1615 "cs.ATG" 
		var = new VariableDeclaration(t.val); 
		if (la.kind == 3) {
			lexer.NextToken();
			VariableInitializer(
#line  1615 "cs.ATG" 
out expr);

#line  1615 "cs.ATG" 
			var.Initializer = expr; 
		}
	}

	void EmbeddedStatement(
#line  1658 "cs.ATG" 
out Statement statement) {

#line  1660 "cs.ATG" 
		TypeReference type = null;
		Expression expr = null;
		Statement embeddedStatement = null;
		statement = null;
		
		if (la.kind == 15) {
			Block(
#line  1666 "cs.ATG" 
out statement);
		} else if (la.kind == 11) {
			lexer.NextToken();

#line  1668 "cs.ATG" 
			statement = new EmptyStatement(); 
		} else if (
#line  1670 "cs.ATG" 
UnCheckedAndLBrace()) {

#line  1670 "cs.ATG" 
			Statement block; bool isChecked = true; 
			if (la.kind == 56) {
				lexer.NextToken();
			} else if (la.kind == 116) {
				lexer.NextToken();

#line  1671 "cs.ATG" 
				isChecked = false;
			} else SynErr(166);
			Block(
#line  1672 "cs.ATG" 
out block);

#line  1672 "cs.ATG" 
			statement = isChecked ? (Statement)new CheckedStatement(block) : (Statement)new UncheckedStatement(block); 
		} else if (la.kind == 77) {
			lexer.NextToken();

#line  1674 "cs.ATG" 
			Statement elseStatement = null; 
			Expect(19);
			Expr(
#line  1675 "cs.ATG" 
out expr);
			Expect(20);
			EmbeddedStatement(
#line  1676 "cs.ATG" 
out embeddedStatement);
			if (la.kind == 65) {
				lexer.NextToken();
				EmbeddedStatement(
#line  1677 "cs.ATG" 
out elseStatement);
			}

#line  1678 "cs.ATG" 
			statement = elseStatement != null ? (Statement)new IfElseStatement(expr, embeddedStatement, elseStatement) :  (Statement)new IfElseStatement(expr, embeddedStatement); 
		} else if (la.kind == 108) {
			lexer.NextToken();

#line  1679 "cs.ATG" 
			ArrayList switchSections = new ArrayList(); SwitchSection switchSection; 
			Expect(19);
			Expr(
#line  1680 "cs.ATG" 
out expr);
			Expect(20);
			Expect(15);
			while (la.kind == 53 || la.kind == 61) {
				SwitchSection(
#line  1681 "cs.ATG" 
out switchSection);

#line  1681 "cs.ATG" 
				switchSections.Add(switchSection); 
			}
			Expect(16);

#line  1682 "cs.ATG" 
			statement = new SwitchStatement(expr, switchSections); 
		} else if (la.kind == 123) {
			lexer.NextToken();
			Expect(19);
			Expr(
#line  1684 "cs.ATG" 
out expr);
			Expect(20);
			EmbeddedStatement(
#line  1686 "cs.ATG" 
out embeddedStatement);

#line  1686 "cs.ATG" 
			statement = new DoLoopStatement(expr, embeddedStatement, ConditionType.While, ConditionPosition.Start);
		} else if (la.kind == 63) {
			lexer.NextToken();
			EmbeddedStatement(
#line  1687 "cs.ATG" 
out embeddedStatement);
			Expect(123);
			Expect(19);
			Expr(
#line  1688 "cs.ATG" 
out expr);
			Expect(20);
			Expect(11);

#line  1688 "cs.ATG" 
			statement = new DoLoopStatement(expr, embeddedStatement, ConditionType.While, ConditionPosition.End); 
		} else if (la.kind == 74) {
			lexer.NextToken();

#line  1689 "cs.ATG" 
			ArrayList initializer = null; ArrayList iterator = null; 
			Expect(19);
			if (StartOf(4)) {
				ForInitializer(
#line  1690 "cs.ATG" 
out initializer);
			}
			Expect(11);
			if (StartOf(4)) {
				Expr(
#line  1691 "cs.ATG" 
out expr);
			}
			Expect(11);
			if (StartOf(4)) {
				ForIterator(
#line  1692 "cs.ATG" 
out iterator);
			}
			Expect(20);
			EmbeddedStatement(
#line  1693 "cs.ATG" 
out embeddedStatement);

#line  1693 "cs.ATG" 
			statement = new ForStatement(initializer, expr, iterator, embeddedStatement); 
		} else if (la.kind == 75) {
			lexer.NextToken();
			Expect(19);
			Type(
#line  1694 "cs.ATG" 
out type);
			Expect(1);

#line  1694 "cs.ATG" 
			string varName = t.val; Point start = t.Location;
			Expect(79);
			Expr(
#line  1695 "cs.ATG" 
out expr);
			Expect(20);
			EmbeddedStatement(
#line  1696 "cs.ATG" 
out embeddedStatement);

#line  1696 "cs.ATG" 
			statement = new ForeachStatement(type, varName , expr, embeddedStatement); 
			statement.EndLocation = t.EndLocation;
			
		} else if (la.kind == 51) {
			lexer.NextToken();
			Expect(11);

#line  1700 "cs.ATG" 
			statement = new BreakStatement(); 
		} else if (la.kind == 59) {
			lexer.NextToken();
			Expect(11);

#line  1701 "cs.ATG" 
			statement = new ContinueStatement(); 
		} else if (la.kind == 76) {
			GotoStatement(
#line  1702 "cs.ATG" 
out statement);
		} else if (
#line  1703 "cs.ATG" 
IsYieldStatement()) {
			Expect(1);
			if (la.kind == 99) {
				lexer.NextToken();
				Expr(
#line  1703 "cs.ATG" 
out expr);

#line  1703 "cs.ATG" 
				statement = new YieldStatement(new ReturnStatement(expr)); 
			} else if (la.kind == 51) {
				lexer.NextToken();

#line  1704 "cs.ATG" 
				statement = new YieldStatement(new BreakStatement()); 
			} else SynErr(167);
			Expect(11);
		} else if (la.kind == 99) {
			lexer.NextToken();
			if (StartOf(4)) {
				Expr(
#line  1705 "cs.ATG" 
out expr);
			}
			Expect(11);

#line  1705 "cs.ATG" 
			statement = new ReturnStatement(expr); 
		} else if (la.kind == 110) {
			lexer.NextToken();
			if (StartOf(4)) {
				Expr(
#line  1706 "cs.ATG" 
out expr);
			}
			Expect(11);

#line  1706 "cs.ATG" 
			statement = new ThrowStatement(expr); 
		} else if (StartOf(4)) {
			StatementExpr(
#line  1708 "cs.ATG" 
out statement);
			Expect(11);
		} else if (la.kind == 112) {
			TryStatement(
#line  1710 "cs.ATG" 
out statement);
		} else if (la.kind == 84) {
			lexer.NextToken();
			Expect(19);
			Expr(
#line  1712 "cs.ATG" 
out expr);
			Expect(20);
			EmbeddedStatement(
#line  1713 "cs.ATG" 
out embeddedStatement);

#line  1713 "cs.ATG" 
			statement = new LockStatement(expr, embeddedStatement); 
		} else if (la.kind == 119) {

#line  1715 "cs.ATG" 
			Statement resourceAcquisitionStmt = null; 
			lexer.NextToken();
			Expect(19);
			ResourceAcquisition(
#line  1717 "cs.ATG" 
out resourceAcquisitionStmt);
			Expect(20);
			EmbeddedStatement(
#line  1718 "cs.ATG" 
out embeddedStatement);

#line  1718 "cs.ATG" 
			statement = new UsingStatement(resourceAcquisitionStmt, embeddedStatement); 
		} else if (la.kind == 117) {
			lexer.NextToken();
			Block(
#line  1720 "cs.ATG" 
out embeddedStatement);

#line  1720 "cs.ATG" 
			statement = new UnsafeStatement(embeddedStatement); 
		} else if (la.kind == 72) {
			lexer.NextToken();
			Expect(19);
			Type(
#line  1723 "cs.ATG" 
out type);

#line  1723 "cs.ATG" 
			if (type.PointerNestingLevel == 0) Error("can only fix pointer types");
			ArrayList pointerDeclarators = new ArrayList(1);
			
			Expect(1);

#line  1726 "cs.ATG" 
			string identifier = t.val; 
			Expect(3);
			Expr(
#line  1727 "cs.ATG" 
out expr);

#line  1727 "cs.ATG" 
			pointerDeclarators.Add(new VariableDeclaration(identifier, expr)); 
			while (la.kind == 13) {
				lexer.NextToken();
				Expect(1);

#line  1729 "cs.ATG" 
				identifier = t.val; 
				Expect(3);
				Expr(
#line  1730 "cs.ATG" 
out expr);

#line  1730 "cs.ATG" 
				pointerDeclarators.Add(new VariableDeclaration(identifier, expr)); 
			}
			Expect(20);
			EmbeddedStatement(
#line  1732 "cs.ATG" 
out embeddedStatement);

#line  1732 "cs.ATG" 
			statement = new FixedStatement(type, pointerDeclarators, embeddedStatement); 
		} else SynErr(168);
	}

	void SwitchSection(
#line  1754 "cs.ATG" 
out SwitchSection stmt) {

#line  1756 "cs.ATG" 
		SwitchSection switchSection = new SwitchSection();
		CaseLabel label;
		
		SwitchLabel(
#line  1760 "cs.ATG" 
out label);

#line  1760 "cs.ATG" 
		switchSection.SwitchLabels.Add(label); 
		while (la.kind == 53 || la.kind == 61) {
			SwitchLabel(
#line  1762 "cs.ATG" 
out label);

#line  1762 "cs.ATG" 
			switchSection.SwitchLabels.Add(label); 
		}

#line  1764 "cs.ATG" 
		compilationUnit.BlockStart(switchSection); 
		Statement();
		while (StartOf(20)) {
			Statement();
		}

#line  1767 "cs.ATG" 
		compilationUnit.BlockEnd();
		stmt = switchSection;
		
	}

	void ForInitializer(
#line  1735 "cs.ATG" 
out ArrayList initializer) {

#line  1737 "cs.ATG" 
		Statement stmt; 
		initializer = new ArrayList();
		
		if (
#line  1741 "cs.ATG" 
IsLocalVarDecl()) {
			LocalVariableDecl(
#line  1741 "cs.ATG" 
out stmt);

#line  1741 "cs.ATG" 
			initializer.Add(stmt);
		} else if (StartOf(4)) {
			StatementExpr(
#line  1742 "cs.ATG" 
out stmt);

#line  1742 "cs.ATG" 
			initializer.Add(stmt);
			while (la.kind == 13) {
				lexer.NextToken();
				StatementExpr(
#line  1742 "cs.ATG" 
out stmt);

#line  1742 "cs.ATG" 
				initializer.Add(stmt);
			}

#line  1742 "cs.ATG" 
			initializer.Add(stmt);
		} else SynErr(169);
	}

	void ForIterator(
#line  1745 "cs.ATG" 
out ArrayList iterator) {

#line  1747 "cs.ATG" 
		Statement stmt; 
		iterator = new ArrayList();
		
		StatementExpr(
#line  1751 "cs.ATG" 
out stmt);

#line  1751 "cs.ATG" 
		iterator.Add(stmt);
		while (la.kind == 13) {
			lexer.NextToken();
			StatementExpr(
#line  1751 "cs.ATG" 
out stmt);

#line  1751 "cs.ATG" 
			iterator.Add(stmt); 
		}
	}

	void GotoStatement(
#line  1822 "cs.ATG" 
out Statement stmt) {

#line  1823 "cs.ATG" 
		Expression expr; stmt = null; 
		Expect(76);
		if (la.kind == 1) {
			lexer.NextToken();

#line  1827 "cs.ATG" 
			stmt = new GotoStatement(t.val); 
			Expect(11);
		} else if (la.kind == 53) {
			lexer.NextToken();
			Expr(
#line  1828 "cs.ATG" 
out expr);
			Expect(11);

#line  1828 "cs.ATG" 
			stmt = new GotoCaseStatement(expr); 
		} else if (la.kind == 61) {
			lexer.NextToken();
			Expect(11);

#line  1829 "cs.ATG" 
			stmt = new GotoCaseStatement(null); 
		} else SynErr(170);
	}

	void StatementExpr(
#line  1849 "cs.ATG" 
out Statement stmt) {

#line  1854 "cs.ATG" 
		bool mustBeAssignment = la.kind == Tokens.Plus  || la.kind == Tokens.Minus ||
		                       la.kind == Tokens.Not   || la.kind == Tokens.BitwiseComplement ||
		                       la.kind == Tokens.Times || la.kind == Tokens.BitwiseAnd   || IsTypeCast();
		Expression expr = null;
		
		UnaryExpr(
#line  1860 "cs.ATG" 
out expr);
		if (StartOf(6)) {

#line  1863 "cs.ATG" 
			AssignmentOperatorType op; Expression val; 
			AssignmentOperator(
#line  1863 "cs.ATG" 
out op);
			Expr(
#line  1863 "cs.ATG" 
out val);

#line  1863 "cs.ATG" 
			expr = new AssignmentExpression(expr, op, val); 
		} else if (la.kind == 11 || la.kind == 13 || la.kind == 20) {

#line  1864 "cs.ATG" 
			if (mustBeAssignment) Error("error in assignment."); 
		} else SynErr(171);

#line  1865 "cs.ATG" 
		stmt = new StatementExpression(expr); 
	}

	void TryStatement(
#line  1779 "cs.ATG" 
out Statement tryStatement) {

#line  1781 "cs.ATG" 
		Statement blockStmt = null, finallyStmt = null;
		ArrayList catchClauses = null;
		
		Expect(112);
		Block(
#line  1785 "cs.ATG" 
out blockStmt);
		if (la.kind == 54) {
			CatchClauses(
#line  1787 "cs.ATG" 
out catchClauses);
			if (la.kind == 71) {
				lexer.NextToken();
				Block(
#line  1787 "cs.ATG" 
out finallyStmt);
			}
		} else if (la.kind == 71) {
			lexer.NextToken();
			Block(
#line  1788 "cs.ATG" 
out finallyStmt);
		} else SynErr(172);

#line  1791 "cs.ATG" 
		tryStatement = new TryCatchStatement(blockStmt, catchClauses, finallyStmt);
			
	}

	void ResourceAcquisition(
#line  1833 "cs.ATG" 
out Statement stmt) {

#line  1835 "cs.ATG" 
		stmt = null;
		Expression expr;
		
		if (
#line  1840 "cs.ATG" 
IsLocalVarDecl()) {
			LocalVariableDecl(
#line  1840 "cs.ATG" 
out stmt);
		} else if (StartOf(4)) {
			Expr(
#line  1841 "cs.ATG" 
out expr);

#line  1845 "cs.ATG" 
			stmt = new StatementExpression(expr); 
		} else SynErr(173);
	}

	void SwitchLabel(
#line  1772 "cs.ATG" 
out CaseLabel label) {

#line  1773 "cs.ATG" 
		Expression expr = null; label = null; 
		if (la.kind == 53) {
			lexer.NextToken();
			Expr(
#line  1775 "cs.ATG" 
out expr);
			Expect(9);

#line  1775 "cs.ATG" 
			label =  new CaseLabel(expr); 
		} else if (la.kind == 61) {
			lexer.NextToken();
			Expect(9);

#line  1776 "cs.ATG" 
			label =  new CaseLabel(); 
		} else SynErr(174);
	}

	void CatchClauses(
#line  1796 "cs.ATG" 
out ArrayList catchClauses) {

#line  1798 "cs.ATG" 
		catchClauses = new ArrayList();
		
		Expect(54);

#line  1801 "cs.ATG" 
		string identifier;
		Statement stmt;
		TypeReference typeRef;
		
		if (la.kind == 15) {
			Block(
#line  1807 "cs.ATG" 
out stmt);

#line  1807 "cs.ATG" 
			catchClauses.Add(new CatchClause(stmt)); 
		} else if (la.kind == 19) {
			lexer.NextToken();
			ClassType(
#line  1809 "cs.ATG" 
out typeRef);

#line  1809 "cs.ATG" 
			identifier = null; 
			if (la.kind == 1) {
				lexer.NextToken();

#line  1810 "cs.ATG" 
				identifier = t.val; 
			}
			Expect(20);
			Block(
#line  1811 "cs.ATG" 
out stmt);

#line  1812 "cs.ATG" 
			catchClauses.Add(new CatchClause(typeRef, identifier, stmt)); 
			while (
#line  1813 "cs.ATG" 
IsTypedCatch()) {
				Expect(54);
				Expect(19);
				ClassType(
#line  1813 "cs.ATG" 
out typeRef);

#line  1813 "cs.ATG" 
				identifier = null; 
				if (la.kind == 1) {
					lexer.NextToken();

#line  1814 "cs.ATG" 
					identifier = t.val; 
				}
				Expect(20);
				Block(
#line  1815 "cs.ATG" 
out stmt);

#line  1816 "cs.ATG" 
				catchClauses.Add(new CatchClause(typeRef, identifier, stmt)); 
			}
			if (la.kind == 54) {
				lexer.NextToken();
				Block(
#line  1818 "cs.ATG" 
out stmt);

#line  1818 "cs.ATG" 
				catchClauses.Add(new CatchClause(stmt)); 
			}
		} else SynErr(175);
	}

	void UnaryExpr(
#line  1881 "cs.ATG" 
out Expression uExpr) {

#line  1883 "cs.ATG" 
		TypeReference type = null;
		Expression expr;
		ArrayList  expressions = new ArrayList();
		uExpr = null;
		
		while (StartOf(24) || 
#line  1907 "cs.ATG" 
IsTypeCast()) {
			if (la.kind == 4) {
				lexer.NextToken();

#line  1892 "cs.ATG" 
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.Plus)); 
			} else if (la.kind == 5) {
				lexer.NextToken();

#line  1893 "cs.ATG" 
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.Minus)); 
			} else if (la.kind == 23) {
				lexer.NextToken();

#line  1894 "cs.ATG" 
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.Not)); 
			} else if (la.kind == 26) {
				lexer.NextToken();

#line  1895 "cs.ATG" 
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.BitNot)); 
			} else if (la.kind == 6) {
				lexer.NextToken();

#line  1896 "cs.ATG" 
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.Star)); 
			} else if (la.kind == 30) {
				lexer.NextToken();

#line  1897 "cs.ATG" 
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.Increment)); 
			} else if (la.kind == 31) {
				lexer.NextToken();

#line  1898 "cs.ATG" 
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.Decrement)); 
			} else if (la.kind == 27) {
				lexer.NextToken();

#line  1899 "cs.ATG" 
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.BitWiseAnd)); 
			} else {
				Expect(19);
				Type(
#line  1907 "cs.ATG" 
out type);
				Expect(20);

#line  1907 "cs.ATG" 
				expressions.Add(new CastExpression(type)); 
			}
		}
		PrimaryExpr(
#line  1911 "cs.ATG" 
out expr);

#line  1911 "cs.ATG" 
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
#line  2094 "cs.ATG" 
ref Expression outExpr) {

#line  2095 "cs.ATG" 
		Expression expr;   
		ConditionalAndExpr(
#line  2097 "cs.ATG" 
ref outExpr);
		while (la.kind == 25) {
			lexer.NextToken();
			UnaryExpr(
#line  2097 "cs.ATG" 
out expr);
			ConditionalAndExpr(
#line  2097 "cs.ATG" 
ref expr);

#line  2097 "cs.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.LogicalOr, expr);  
		}
	}

	void PrimaryExpr(
#line  1928 "cs.ATG" 
out Expression pexpr) {

#line  1930 "cs.ATG" 
		TypeReference type = null;
		List<TypeReference> typeList = null;
		bool isArrayCreation = false;
		Expression expr;
		pexpr = null;
		
		if (la.kind == 111) {
			lexer.NextToken();

#line  1938 "cs.ATG" 
			pexpr = new PrimitiveExpression(true, "true");  
		} else if (la.kind == 70) {
			lexer.NextToken();

#line  1939 "cs.ATG" 
			pexpr = new PrimitiveExpression(false, "false"); 
		} else if (la.kind == 88) {
			lexer.NextToken();

#line  1940 "cs.ATG" 
			pexpr = new PrimitiveExpression(null, "null");  
		} else if (la.kind == 2) {
			lexer.NextToken();

#line  1941 "cs.ATG" 
			pexpr = new PrimitiveExpression(t.literalValue, t.val);  
		} else if (
#line  1942 "cs.ATG" 
la.kind == Tokens.Identifier && Peek(1).kind == Tokens.DoubleColon) {
			TypeName(
#line  1943 "cs.ATG" 
out type);

#line  1943 "cs.ATG" 
			pexpr = new TypeReferenceExpression(type); 
		} else if (la.kind == 1) {
			lexer.NextToken();

#line  1945 "cs.ATG" 
			pexpr = new IdentifierExpression(t.val); 
		} else if (la.kind == 19) {
			lexer.NextToken();
			Expr(
#line  1947 "cs.ATG" 
out expr);
			Expect(20);

#line  1947 "cs.ATG" 
			pexpr = new ParenthesizedExpression(expr); 
		} else if (StartOf(25)) {

#line  1949 "cs.ATG" 
			string val = null; 
			switch (la.kind) {
			case 50: {
				lexer.NextToken();

#line  1951 "cs.ATG" 
				val = "bool"; 
				break;
			}
			case 52: {
				lexer.NextToken();

#line  1952 "cs.ATG" 
				val = "byte"; 
				break;
			}
			case 55: {
				lexer.NextToken();

#line  1953 "cs.ATG" 
				val = "char"; 
				break;
			}
			case 60: {
				lexer.NextToken();

#line  1954 "cs.ATG" 
				val = "decimal"; 
				break;
			}
			case 64: {
				lexer.NextToken();

#line  1955 "cs.ATG" 
				val = "double"; 
				break;
			}
			case 73: {
				lexer.NextToken();

#line  1956 "cs.ATG" 
				val = "float"; 
				break;
			}
			case 80: {
				lexer.NextToken();

#line  1957 "cs.ATG" 
				val = "int"; 
				break;
			}
			case 85: {
				lexer.NextToken();

#line  1958 "cs.ATG" 
				val = "long"; 
				break;
			}
			case 89: {
				lexer.NextToken();

#line  1959 "cs.ATG" 
				val = "object"; 
				break;
			}
			case 100: {
				lexer.NextToken();

#line  1960 "cs.ATG" 
				val = "sbyte"; 
				break;
			}
			case 102: {
				lexer.NextToken();

#line  1961 "cs.ATG" 
				val = "short"; 
				break;
			}
			case 106: {
				lexer.NextToken();

#line  1962 "cs.ATG" 
				val = "string"; 
				break;
			}
			case 114: {
				lexer.NextToken();

#line  1963 "cs.ATG" 
				val = "uint"; 
				break;
			}
			case 115: {
				lexer.NextToken();

#line  1964 "cs.ATG" 
				val = "ulong"; 
				break;
			}
			case 118: {
				lexer.NextToken();

#line  1965 "cs.ATG" 
				val = "ushort"; 
				break;
			}
			}

#line  1966 "cs.ATG" 
			t.val = ""; 
			Expect(14);
			Expect(1);

#line  1966 "cs.ATG" 
			pexpr = new FieldReferenceExpression(new TypeReferenceExpression(val), t.val); 
		} else if (la.kind == 109) {
			lexer.NextToken();

#line  1968 "cs.ATG" 
			pexpr = new ThisReferenceExpression(); 
		} else if (la.kind == 49) {
			lexer.NextToken();

#line  1970 "cs.ATG" 
			Expression retExpr = new BaseReferenceExpression(); 
			if (la.kind == 14) {
				lexer.NextToken();
				Expect(1);

#line  1972 "cs.ATG" 
				retExpr = new FieldReferenceExpression(retExpr, t.val); 
			} else if (la.kind == 17) {
				lexer.NextToken();
				Expr(
#line  1973 "cs.ATG" 
out expr);

#line  1973 "cs.ATG" 
				ArrayList indices = new ArrayList(); if (expr != null) { indices.Add(expr); } 
				while (la.kind == 13) {
					lexer.NextToken();
					Expr(
#line  1974 "cs.ATG" 
out expr);

#line  1974 "cs.ATG" 
					if (expr != null) { indices.Add(expr); } 
				}
				Expect(18);

#line  1975 "cs.ATG" 
				retExpr = new IndexerExpression(retExpr, indices); 
			} else SynErr(176);

#line  1976 "cs.ATG" 
			pexpr = retExpr; 
		} else if (la.kind == 87) {
			lexer.NextToken();
			NonArrayType(
#line  1977 "cs.ATG" 
out type);

#line  1977 "cs.ATG" 
			ArrayList parameters = new ArrayList(); 
			if (la.kind == 19) {
				lexer.NextToken();

#line  1982 "cs.ATG" 
				ObjectCreateExpression oce = new ObjectCreateExpression(type, parameters); 
				if (StartOf(21)) {
					Argument(
#line  1983 "cs.ATG" 
out expr);

#line  1983 "cs.ATG" 
					if (expr != null) { parameters.Add(expr); } 
					while (la.kind == 13) {
						lexer.NextToken();
						Argument(
#line  1984 "cs.ATG" 
out expr);

#line  1984 "cs.ATG" 
						if (expr != null) { parameters.Add(expr); } 
					}
				}
				Expect(20);

#line  1986 "cs.ATG" 
				pexpr = oce; 
			} else if (la.kind == 17) {

#line  1988 "cs.ATG" 
				isArrayCreation = true; ArrayCreateExpression ace = new ArrayCreateExpression(type); pexpr = ace; 
				lexer.NextToken();

#line  1989 "cs.ATG" 
				int dims = 0; 
				ArrayList rank = new ArrayList(); 
				ArrayList parameterExpression = new ArrayList(); 
				if (StartOf(4)) {
					Expr(
#line  1993 "cs.ATG" 
out expr);

#line  1993 "cs.ATG" 
					if (expr != null) { parameterExpression.Add(expr); } 
					while (la.kind == 13) {
						lexer.NextToken();
						Expr(
#line  1995 "cs.ATG" 
out expr);

#line  1995 "cs.ATG" 
						if (expr != null) { parameterExpression.Add(expr); } 
					}
					Expect(18);

#line  1997 "cs.ATG" 
					parameters.Add(new ArrayCreationParameter(parameterExpression)); 
					ace.Parameters = parameters; 
					while (
#line  2000 "cs.ATG" 
IsDims()) {
						Expect(17);

#line  2000 "cs.ATG" 
						dims =0;
						while (la.kind == 13) {
							lexer.NextToken();

#line  2001 "cs.ATG" 
							dims++;
						}

#line  2001 "cs.ATG" 
						rank.Add(dims); 
						parameters.Add(new ArrayCreationParameter(dims)); 
						
						Expect(18);
					}

#line  2005 "cs.ATG" 
					if (rank.Count > 0) { 
					ace.Rank = (int[])rank.ToArray(typeof (int)); 
					} 
					
					if (la.kind == 15) {
						ArrayInitializer(
#line  2009 "cs.ATG" 
out expr);

#line  2009 "cs.ATG" 
						ace.ArrayInitializer = (ArrayInitializerExpression)expr; 
					}
				} else if (la.kind == 13 || la.kind == 18) {
					while (la.kind == 13) {
						lexer.NextToken();

#line  2011 "cs.ATG" 
						dims++;
					}

#line  2012 "cs.ATG" 
					parameters.Add(new ArrayCreationParameter(dims)); 
					
					Expect(18);
					while (
#line  2014 "cs.ATG" 
IsDims()) {
						Expect(17);

#line  2014 "cs.ATG" 
						dims =0;
						while (la.kind == 13) {
							lexer.NextToken();

#line  2014 "cs.ATG" 
							dims++;
						}

#line  2014 "cs.ATG" 
						parameters.Add(new ArrayCreationParameter(dims)); 
						Expect(18);
					}
					ArrayInitializer(
#line  2014 "cs.ATG" 
out expr);

#line  2014 "cs.ATG" 
					ace.ArrayInitializer = (ArrayInitializerExpression)expr; ace.Parameters = parameters; 
				} else SynErr(177);
			} else SynErr(178);
		} else if (la.kind == 113) {
			lexer.NextToken();
			Expect(19);
			if (
#line  2020 "cs.ATG" 
NotVoidPointer()) {
				Expect(121);

#line  2020 "cs.ATG" 
				type = new TypeReference("void"); 
			} else if (StartOf(8)) {
				Type(
#line  2021 "cs.ATG" 
out type);
			} else SynErr(179);
			Expect(20);

#line  2022 "cs.ATG" 
			pexpr = new TypeOfExpression(type); 
		} else if (la.kind == 103) {
			lexer.NextToken();
			Expect(19);
			Type(
#line  2023 "cs.ATG" 
out type);
			Expect(20);

#line  2023 "cs.ATG" 
			pexpr = new SizeOfExpression(type); 
		} else if (la.kind == 56) {
			lexer.NextToken();
			Expect(19);
			Expr(
#line  2024 "cs.ATG" 
out expr);
			Expect(20);

#line  2024 "cs.ATG" 
			pexpr = new CheckedExpression(expr); 
		} else if (la.kind == 116) {
			lexer.NextToken();
			Expect(19);
			Expr(
#line  2025 "cs.ATG" 
out expr);
			Expect(20);

#line  2025 "cs.ATG" 
			pexpr = new UncheckedExpression(expr); 
		} else if (la.kind == 62) {
			lexer.NextToken();
			AnonymousMethodExpr(
#line  2026 "cs.ATG" 
out expr);

#line  2026 "cs.ATG" 
			pexpr = expr; 
		} else SynErr(180);
		while (StartOf(26) || 
#line  2047 "cs.ATG" 
IsGenericFollowedBy(Tokens.Dot) && IsTypeReferenceExpression(pexpr) || 
#line  2055 "cs.ATG" 
IsGenericFollowedBy(Tokens.OpenParenthesis)) {
			if (la.kind == 30 || la.kind == 31) {
				if (la.kind == 30) {
					lexer.NextToken();

#line  2030 "cs.ATG" 
					pexpr = new UnaryOperatorExpression(pexpr, UnaryOperatorType.PostIncrement); 
				} else if (la.kind == 31) {
					lexer.NextToken();

#line  2031 "cs.ATG" 
					pexpr = new UnaryOperatorExpression(pexpr, UnaryOperatorType.PostDecrement); 
				} else SynErr(181);
			} else if (la.kind == 46) {
				lexer.NextToken();
				Expect(1);

#line  2034 "cs.ATG" 
				pexpr = new PointerReferenceExpression(pexpr, t.val); 
			} else if (la.kind == 14) {
				lexer.NextToken();
				Expect(1);

#line  2035 "cs.ATG" 
				pexpr = new FieldReferenceExpression(pexpr, t.val);
			} else if (
#line  2047 "cs.ATG" 
IsGenericFollowedBy(Tokens.Dot) && IsTypeReferenceExpression(pexpr)) {
				TypeArgumentList(
#line  2048 "cs.ATG" 
out typeList);
				Expect(14);
				Expect(1);

#line  2049 "cs.ATG" 
				pexpr = new FieldReferenceExpression(GetTypeReferenceExpression(pexpr, typeList), t.val);
			} else if (la.kind == 19) {
				lexer.NextToken();

#line  2051 "cs.ATG" 
				ArrayList parameters = new ArrayList(); 
				if (StartOf(21)) {
					Argument(
#line  2052 "cs.ATG" 
out expr);

#line  2052 "cs.ATG" 
					if (expr != null) {parameters.Add(expr);} 
					while (la.kind == 13) {
						lexer.NextToken();
						Argument(
#line  2053 "cs.ATG" 
out expr);

#line  2053 "cs.ATG" 
						if (expr != null) {parameters.Add(expr);} 
					}
				}
				Expect(20);

#line  2054 "cs.ATG" 
				pexpr = new InvocationExpression(pexpr, parameters); 
			} else if (
#line  2055 "cs.ATG" 
IsGenericFollowedBy(Tokens.OpenParenthesis)) {
				TypeArgumentList(
#line  2055 "cs.ATG" 
out typeList);
				Expect(19);

#line  2056 "cs.ATG" 
				ArrayList parameters = new ArrayList(); 
				if (StartOf(21)) {
					Argument(
#line  2057 "cs.ATG" 
out expr);

#line  2057 "cs.ATG" 
					if (expr != null) {parameters.Add(expr);} 
					while (la.kind == 13) {
						lexer.NextToken();
						Argument(
#line  2058 "cs.ATG" 
out expr);

#line  2058 "cs.ATG" 
						if (expr != null) {parameters.Add(expr);} 
					}
				}
				Expect(20);

#line  2059 "cs.ATG" 
				pexpr = new InvocationExpression(pexpr, parameters, typeList); 
			} else {

#line  2061 "cs.ATG" 
				if (isArrayCreation) Error("element access not allow on array creation");
				ArrayList indices = new ArrayList();
				
				lexer.NextToken();
				Expr(
#line  2064 "cs.ATG" 
out expr);

#line  2064 "cs.ATG" 
				if (expr != null) { indices.Add(expr); } 
				while (la.kind == 13) {
					lexer.NextToken();
					Expr(
#line  2065 "cs.ATG" 
out expr);

#line  2065 "cs.ATG" 
					if (expr != null) { indices.Add(expr); } 
				}
				Expect(18);

#line  2066 "cs.ATG" 
				pexpr = new IndexerExpression(pexpr, indices); 
			}
		}
	}

	void AnonymousMethodExpr(
#line  2070 "cs.ATG" 
out Expression outExpr) {

#line  2072 "cs.ATG" 
		AnonymousMethodExpression expr = new AnonymousMethodExpression();
		expr.StartLocation = t.Location;
		Statement stmt;
		List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
		outExpr = expr;
		
		Expect(19);
		if (StartOf(9)) {
			FormalParameterList(
#line  2080 "cs.ATG" 
p);

#line  2080 "cs.ATG" 
			expr.Parameters = p; 
		}
		Expect(20);

#line  2084 "cs.ATG" 
		if (compilationUnit != null) { 
		Block(
#line  2085 "cs.ATG" 
out stmt);

#line  2085 "cs.ATG" 
		expr.Body  = (BlockStatement)stmt; 

#line  2086 "cs.ATG" 
		} else { 
		Expect(15);

#line  2088 "cs.ATG" 
		lexer.SkipCurrentBlock(); 
		Expect(16);

#line  2090 "cs.ATG" 
		} 

#line  2091 "cs.ATG" 
		expr.EndLocation = t.Location; 
	}

	void TypeArgumentList(
#line  2240 "cs.ATG" 
out List<TypeReference> types) {

#line  2242 "cs.ATG" 
		types = new List<TypeReference>();
		TypeReference type = null;
		
		Expect(22);
		Type(
#line  2246 "cs.ATG" 
out type);

#line  2246 "cs.ATG" 
		types.Add(type); 
		while (la.kind == 13) {
			lexer.NextToken();
			Type(
#line  2247 "cs.ATG" 
out type);

#line  2247 "cs.ATG" 
			types.Add(type); 
		}
		Expect(21);
	}

	void ConditionalAndExpr(
#line  2100 "cs.ATG" 
ref Expression outExpr) {

#line  2101 "cs.ATG" 
		Expression expr; 
		InclusiveOrExpr(
#line  2103 "cs.ATG" 
ref outExpr);
		while (la.kind == 24) {
			lexer.NextToken();
			UnaryExpr(
#line  2103 "cs.ATG" 
out expr);
			InclusiveOrExpr(
#line  2103 "cs.ATG" 
ref expr);

#line  2103 "cs.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.LogicalAnd, expr);  
		}
	}

	void InclusiveOrExpr(
#line  2106 "cs.ATG" 
ref Expression outExpr) {

#line  2107 "cs.ATG" 
		Expression expr; 
		ExclusiveOrExpr(
#line  2109 "cs.ATG" 
ref outExpr);
		while (la.kind == 28) {
			lexer.NextToken();
			UnaryExpr(
#line  2109 "cs.ATG" 
out expr);
			ExclusiveOrExpr(
#line  2109 "cs.ATG" 
ref expr);

#line  2109 "cs.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.BitwiseOr, expr);  
		}
	}

	void ExclusiveOrExpr(
#line  2112 "cs.ATG" 
ref Expression outExpr) {

#line  2113 "cs.ATG" 
		Expression expr; 
		AndExpr(
#line  2115 "cs.ATG" 
ref outExpr);
		while (la.kind == 29) {
			lexer.NextToken();
			UnaryExpr(
#line  2115 "cs.ATG" 
out expr);
			AndExpr(
#line  2115 "cs.ATG" 
ref expr);

#line  2115 "cs.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.ExclusiveOr, expr);  
		}
	}

	void AndExpr(
#line  2118 "cs.ATG" 
ref Expression outExpr) {

#line  2119 "cs.ATG" 
		Expression expr; 
		EqualityExpr(
#line  2121 "cs.ATG" 
ref outExpr);
		while (la.kind == 27) {
			lexer.NextToken();
			UnaryExpr(
#line  2121 "cs.ATG" 
out expr);
			EqualityExpr(
#line  2121 "cs.ATG" 
ref expr);

#line  2121 "cs.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.BitwiseAnd, expr);  
		}
	}

	void EqualityExpr(
#line  2124 "cs.ATG" 
ref Expression outExpr) {

#line  2126 "cs.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		RelationalExpr(
#line  2130 "cs.ATG" 
ref outExpr);
		while (la.kind == 32 || la.kind == 33) {
			if (la.kind == 33) {
				lexer.NextToken();

#line  2133 "cs.ATG" 
				op = BinaryOperatorType.InEquality; 
			} else {
				lexer.NextToken();

#line  2134 "cs.ATG" 
				op = BinaryOperatorType.Equality; 
			}
			UnaryExpr(
#line  2136 "cs.ATG" 
out expr);
			RelationalExpr(
#line  2136 "cs.ATG" 
ref expr);

#line  2136 "cs.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr);  
		}
	}

	void RelationalExpr(
#line  2140 "cs.ATG" 
ref Expression outExpr) {

#line  2142 "cs.ATG" 
		TypeReference type;
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		ShiftExpr(
#line  2147 "cs.ATG" 
ref outExpr);
		while (StartOf(27)) {
			if (StartOf(28)) {
				if (la.kind == 22) {
					lexer.NextToken();

#line  2150 "cs.ATG" 
					op = BinaryOperatorType.LessThan; 
				} else if (la.kind == 21) {
					lexer.NextToken();

#line  2151 "cs.ATG" 
					op = BinaryOperatorType.GreaterThan; 
				} else if (la.kind == 35) {
					lexer.NextToken();

#line  2152 "cs.ATG" 
					op = BinaryOperatorType.LessThanOrEqual; 
				} else if (la.kind == 34) {
					lexer.NextToken();

#line  2153 "cs.ATG" 
					op = BinaryOperatorType.GreaterThanOrEqual; 
				} else SynErr(182);
				UnaryExpr(
#line  2155 "cs.ATG" 
out expr);
				ShiftExpr(
#line  2155 "cs.ATG" 
ref expr);

#line  2155 "cs.ATG" 
				outExpr = new BinaryOperatorExpression(outExpr, op, expr);  
			} else {
				if (la.kind == 83) {
					lexer.NextToken();

#line  2158 "cs.ATG" 
					op = BinaryOperatorType.TypeCheck; 
				} else if (la.kind == 48) {
					lexer.NextToken();

#line  2159 "cs.ATG" 
					op = BinaryOperatorType.AsCast; 
				} else SynErr(183);
				Type(
#line  2161 "cs.ATG" 
out type);

#line  2161 "cs.ATG" 
				outExpr = new BinaryOperatorExpression(outExpr, op, new TypeReferenceExpression(type)); 
			}
		}
	}

	void ShiftExpr(
#line  2165 "cs.ATG" 
ref Expression outExpr) {

#line  2167 "cs.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		AdditiveExpr(
#line  2171 "cs.ATG" 
ref outExpr);
		while (la.kind == 36 || 
#line  2174 "cs.ATG" 
IsShiftRight()) {
			if (la.kind == 36) {
				lexer.NextToken();

#line  2173 "cs.ATG" 
				op = BinaryOperatorType.ShiftLeft; 
			} else {
				Expect(21);
				Expect(21);

#line  2175 "cs.ATG" 
				op = BinaryOperatorType.ShiftRight; 
			}
			UnaryExpr(
#line  2178 "cs.ATG" 
out expr);
			AdditiveExpr(
#line  2178 "cs.ATG" 
ref expr);

#line  2178 "cs.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr);  
		}
	}

	void AdditiveExpr(
#line  2182 "cs.ATG" 
ref Expression outExpr) {

#line  2184 "cs.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		MultiplicativeExpr(
#line  2188 "cs.ATG" 
ref outExpr);
		while (la.kind == 4 || la.kind == 5) {
			if (la.kind == 4) {
				lexer.NextToken();

#line  2191 "cs.ATG" 
				op = BinaryOperatorType.Add; 
			} else {
				lexer.NextToken();

#line  2192 "cs.ATG" 
				op = BinaryOperatorType.Subtract; 
			}
			UnaryExpr(
#line  2194 "cs.ATG" 
out expr);
			MultiplicativeExpr(
#line  2194 "cs.ATG" 
ref expr);

#line  2194 "cs.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr);  
		}
	}

	void MultiplicativeExpr(
#line  2198 "cs.ATG" 
ref Expression outExpr) {

#line  2200 "cs.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		while (la.kind == 6 || la.kind == 7 || la.kind == 8) {
			if (la.kind == 6) {
				lexer.NextToken();

#line  2206 "cs.ATG" 
				op = BinaryOperatorType.Multiply; 
			} else if (la.kind == 7) {
				lexer.NextToken();

#line  2207 "cs.ATG" 
				op = BinaryOperatorType.Divide; 
			} else {
				lexer.NextToken();

#line  2208 "cs.ATG" 
				op = BinaryOperatorType.Modulus; 
			}
			UnaryExpr(
#line  2210 "cs.ATG" 
out expr);

#line  2210 "cs.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr); 
		}
	}

	void TypeParameterConstraintsClauseBase(
#line  2292 "cs.ATG" 
out TypeReference type) {

#line  2293 "cs.ATG" 
		TypeReference t; type = null; 
		if (la.kind == 107) {
			lexer.NextToken();

#line  2295 "cs.ATG" 
			type = new TypeReference("struct"); 
		} else if (la.kind == 57) {
			lexer.NextToken();

#line  2296 "cs.ATG" 
			type = new TypeReference("struct"); 
		} else if (la.kind == 87) {
			lexer.NextToken();
			Expect(19);
			Expect(20);

#line  2297 "cs.ATG" 
			type = new TypeReference("struct"); 
		} else if (StartOf(8)) {
			Type(
#line  2298 "cs.ATG" 
out t);

#line  2298 "cs.ATG" 
			type = t; 
		} else SynErr(184);
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
			case 13: s = "\",\" expected"; break;
			case 14: s = "\".\" expected"; break;
			case 15: s = "\"{\" expected"; break;
			case 16: s = "\"}\" expected"; break;
			case 17: s = "\"[\" expected"; break;
			case 18: s = "\"]\" expected"; break;
			case 19: s = "\"(\" expected"; break;
			case 20: s = "\")\" expected"; break;
			case 21: s = "\">\" expected"; break;
			case 22: s = "\"<\" expected"; break;
			case 23: s = "\"!\" expected"; break;
			case 24: s = "\"&&\" expected"; break;
			case 25: s = "\"||\" expected"; break;
			case 26: s = "\"~\" expected"; break;
			case 27: s = "\"&\" expected"; break;
			case 28: s = "\"|\" expected"; break;
			case 29: s = "\"^\" expected"; break;
			case 30: s = "\"++\" expected"; break;
			case 31: s = "\"--\" expected"; break;
			case 32: s = "\"==\" expected"; break;
			case 33: s = "\"!=\" expected"; break;
			case 34: s = "\">=\" expected"; break;
			case 35: s = "\"<=\" expected"; break;
			case 36: s = "\"<<\" expected"; break;
			case 37: s = "\"+=\" expected"; break;
			case 38: s = "\"-=\" expected"; break;
			case 39: s = "\"*=\" expected"; break;
			case 40: s = "\"/=\" expected"; break;
			case 41: s = "\"%=\" expected"; break;
			case 42: s = "\"&=\" expected"; break;
			case 43: s = "\"|=\" expected"; break;
			case 44: s = "\"^=\" expected"; break;
			case 45: s = "\"<<=\" expected"; break;
			case 46: s = "\"->\" expected"; break;
			case 47: s = "\"abstract\" expected"; break;
			case 48: s = "\"as\" expected"; break;
			case 49: s = "\"base\" expected"; break;
			case 50: s = "\"bool\" expected"; break;
			case 51: s = "\"break\" expected"; break;
			case 52: s = "\"byte\" expected"; break;
			case 53: s = "\"case\" expected"; break;
			case 54: s = "\"catch\" expected"; break;
			case 55: s = "\"char\" expected"; break;
			case 56: s = "\"checked\" expected"; break;
			case 57: s = "\"class\" expected"; break;
			case 58: s = "\"const\" expected"; break;
			case 59: s = "\"continue\" expected"; break;
			case 60: s = "\"decimal\" expected"; break;
			case 61: s = "\"default\" expected"; break;
			case 62: s = "\"delegate\" expected"; break;
			case 63: s = "\"do\" expected"; break;
			case 64: s = "\"double\" expected"; break;
			case 65: s = "\"else\" expected"; break;
			case 66: s = "\"enum\" expected"; break;
			case 67: s = "\"event\" expected"; break;
			case 68: s = "\"explicit\" expected"; break;
			case 69: s = "\"extern\" expected"; break;
			case 70: s = "\"false\" expected"; break;
			case 71: s = "\"finally\" expected"; break;
			case 72: s = "\"fixed\" expected"; break;
			case 73: s = "\"float\" expected"; break;
			case 74: s = "\"for\" expected"; break;
			case 75: s = "\"foreach\" expected"; break;
			case 76: s = "\"goto\" expected"; break;
			case 77: s = "\"if\" expected"; break;
			case 78: s = "\"implicit\" expected"; break;
			case 79: s = "\"in\" expected"; break;
			case 80: s = "\"int\" expected"; break;
			case 81: s = "\"interface\" expected"; break;
			case 82: s = "\"internal\" expected"; break;
			case 83: s = "\"is\" expected"; break;
			case 84: s = "\"lock\" expected"; break;
			case 85: s = "\"long\" expected"; break;
			case 86: s = "\"namespace\" expected"; break;
			case 87: s = "\"new\" expected"; break;
			case 88: s = "\"null\" expected"; break;
			case 89: s = "\"object\" expected"; break;
			case 90: s = "\"operator\" expected"; break;
			case 91: s = "\"out\" expected"; break;
			case 92: s = "\"override\" expected"; break;
			case 93: s = "\"params\" expected"; break;
			case 94: s = "\"private\" expected"; break;
			case 95: s = "\"protected\" expected"; break;
			case 96: s = "\"public\" expected"; break;
			case 97: s = "\"readonly\" expected"; break;
			case 98: s = "\"ref\" expected"; break;
			case 99: s = "\"return\" expected"; break;
			case 100: s = "\"sbyte\" expected"; break;
			case 101: s = "\"sealed\" expected"; break;
			case 102: s = "\"short\" expected"; break;
			case 103: s = "\"sizeof\" expected"; break;
			case 104: s = "\"stackalloc\" expected"; break;
			case 105: s = "\"static\" expected"; break;
			case 106: s = "\"string\" expected"; break;
			case 107: s = "\"struct\" expected"; break;
			case 108: s = "\"switch\" expected"; break;
			case 109: s = "\"this\" expected"; break;
			case 110: s = "\"throw\" expected"; break;
			case 111: s = "\"true\" expected"; break;
			case 112: s = "\"try\" expected"; break;
			case 113: s = "\"typeof\" expected"; break;
			case 114: s = "\"uint\" expected"; break;
			case 115: s = "\"ulong\" expected"; break;
			case 116: s = "\"unchecked\" expected"; break;
			case 117: s = "\"unsafe\" expected"; break;
			case 118: s = "\"ushort\" expected"; break;
			case 119: s = "\"using\" expected"; break;
			case 120: s = "\"virtual\" expected"; break;
			case 121: s = "\"void\" expected"; break;
			case 122: s = "\"volatile\" expected"; break;
			case 123: s = "\"while\" expected"; break;
			case 124: s = "??? expected"; break;
			case 125: s = "invalid NamespaceMemberDecl"; break;
			case 126: s = "invalid AttributeArguments"; break;
			case 127: s = "invalid Expr"; break;
			case 128: s = "invalid TypeModifier"; break;
			case 129: s = "invalid TypeDecl"; break;
			case 130: s = "invalid TypeDecl"; break;
			case 131: s = "invalid IntegralType"; break;
			case 132: s = "invalid Type"; break;
			case 133: s = "invalid Type"; break;
			case 134: s = "invalid FormalParameterList"; break;
			case 135: s = "invalid FormalParameterList"; break;
			case 136: s = "invalid ClassType"; break;
			case 137: s = "invalid MemberModifier"; break;
			case 138: s = "invalid ClassMemberDecl"; break;
			case 139: s = "invalid ClassMemberDecl"; break;
			case 140: s = "invalid StructMemberDecl"; break;
			case 141: s = "invalid StructMemberDecl"; break;
			case 142: s = "invalid StructMemberDecl"; break;
			case 143: s = "invalid StructMemberDecl"; break;
			case 144: s = "invalid StructMemberDecl"; break;
			case 145: s = "invalid StructMemberDecl"; break;
			case 146: s = "invalid StructMemberDecl"; break;
			case 147: s = "invalid StructMemberDecl"; break;
			case 148: s = "invalid StructMemberDecl"; break;
			case 149: s = "invalid StructMemberDecl"; break;
			case 150: s = "invalid InterfaceMemberDecl"; break;
			case 151: s = "invalid InterfaceMemberDecl"; break;
			case 152: s = "invalid InterfaceMemberDecl"; break;
			case 153: s = "invalid SimpleType"; break;
			case 154: s = "invalid NonArrayType"; break;
			case 155: s = "invalid EventAccessorDecls"; break;
			case 156: s = "invalid ConstructorInitializer"; break;
			case 157: s = "invalid OverloadableOperator"; break;
			case 158: s = "invalid AccessorDecls"; break;
			case 159: s = "invalid InterfaceAccessors"; break;
			case 160: s = "invalid InterfaceAccessors"; break;
			case 161: s = "invalid GetAccessorDecl"; break;
			case 162: s = "invalid SetAccessorDecl"; break;
			case 163: s = "invalid VariableInitializer"; break;
			case 164: s = "invalid Statement"; break;
			case 165: s = "invalid AssignmentOperator"; break;
			case 166: s = "invalid EmbeddedStatement"; break;
			case 167: s = "invalid EmbeddedStatement"; break;
			case 168: s = "invalid EmbeddedStatement"; break;
			case 169: s = "invalid ForInitializer"; break;
			case 170: s = "invalid GotoStatement"; break;
			case 171: s = "invalid StatementExpr"; break;
			case 172: s = "invalid TryStatement"; break;
			case 173: s = "invalid ResourceAcquisition"; break;
			case 174: s = "invalid SwitchLabel"; break;
			case 175: s = "invalid CatchClauses"; break;
			case 176: s = "invalid PrimaryExpr"; break;
			case 177: s = "invalid PrimaryExpr"; break;
			case 178: s = "invalid PrimaryExpr"; break;
			case 179: s = "invalid PrimaryExpr"; break;
			case 180: s = "invalid PrimaryExpr"; break;
			case 181: s = "invalid PrimaryExpr"; break;
			case 182: s = "invalid RelationalExpr"; break;
			case 183: s = "invalid RelationalExpr"; break;
			case 184: s = "invalid TypeParameterConstraintsClauseBase"; break;

			default: s = "error " + errorNumber; break;
		}
		errors.Error(line, col, s);
	}
	
	protected bool StartOf(int s)
	{
		return set[s, lexer.LookAhead.kind];
	}
	
	static bool[,] set = {
	{T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,T,T, x,x,x,x, x,x,T,T, T,x,x,x, x,T,x,x, x,T,x,T, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,T, x,x,x,x, x,x,T,T, T,x,x,x, x,T,x,x, x,T,x,T, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,x, x,x,T,T, T,x,x,x, x,T,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x},
	{x,T,T,x, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,x,T,T, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, T,x,x,T, T,x,x,x, T,x,T,x, T,x,x,x, x,x,T,x, x,T,x,x, x,x,x,x, T,x,x,x, x,T,x,T, T,T,x,x, x,x,x,x, x,x,x,x, T,x,T,T, x,x,T,x, x,T,x,T, x,T,T,T, T,x,T,x, x,x,x,x, x,x},
	{x,x,x,x, T,T,T,T, T,T,x,T, T,T,x,x, T,x,T,x, T,T,T,x, T,T,x,T, T,T,x,x, T,T,T,T, T,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
	{x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,T, x,x,x,x, T,x,x,x, T,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,T,x,x, x,T,x,x, x,x,x,x, x,x,x,x, T,x,T,x, x,x,T,x, x,x,x,x, x,x,T,T, x,x,T,x, x,T,x,x, x,x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,T, x,x,x,x, T,x,x,x, T,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,T,x,x, x,T,x,T, x,T,x,x, x,x,T,x, T,x,T,x, x,x,T,x, x,x,x,x, x,x,T,T, x,x,T,x, x,T,x,x, x,x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,x, T,x,x,T, x,T,T,x, T,x,T,x, T,x,T,T, T,T,x,x, x,T,x,x, x,x,T,x, T,T,T,x, x,T,x,T, x,T,x,x, T,x,T,T, T,T,x,x, T,T,T,x, x,T,T,T, x,x,x,x, x,x,T,T, x,T,T,x, T,T,T,x, x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,x, T,x,T,T, T,T,x,x, x,T,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,T,x,x, T,x,T,x, x,x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,x, T,x,x,T, x,T,T,x, T,x,T,x, T,x,T,T, T,T,x,x, x,T,x,x, x,x,T,x, T,T,T,x, x,T,x,T, x,T,x,x, T,x,T,T, T,T,x,x, T,T,T,x, x,T,T,T, x,x,x,x, x,x,T,T, x,T,T,x, T,T,T,x, x,x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,T, x,x,x,x, T,x,x,x, T,x,x,T, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,T,x,T, x,T,x,x, x,x,x,x, x,x,x,x, T,x,T,x, x,x,T,x, x,x,x,x, x,x,T,T, x,x,T,x, x,T,x,x, x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,T, x,x,x,x, T,x,x,x, T,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,T,x, x,x,x,x, x,x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,T, x,x,x,x, T,x,x,x, T,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,T,x,x, x,T,x,T, x,x,x,x, x,x,T,x, T,x,T,x, x,x,T,x, x,x,x,x, x,x,T,T, x,x,T,x, x,T,x,x, x,x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,T, x,T,T,x, T,x,T,x, T,x,T,T, T,x,x,x, x,T,x,x, x,x,T,x, T,T,x,x, x,T,x,x, x,T,x,x, x,x,x,x, x,x,x,x, T,x,T,x, x,x,T,T, x,x,x,x, x,x,T,T, x,x,T,x, x,T,x,x, x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,T, x,x,x,x, T,x,x,x, T,x,x,T, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,T,x,x, x,T,x,x, x,x,x,x, x,x,x,x, T,x,T,x, x,x,T,x, x,x,x,x, x,x,T,T, x,x,T,x, x,T,x,x, x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,T,x, x,x,x,x, x,x},
	{x,T,T,x, T,T,T,x, x,x,x,T, x,x,x,T, x,x,x,T, x,x,x,T, x,x,T,T, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,T, T,x,x,T, T,x,T,T, T,x,T,T, T,x,x,x, x,x,T,x, T,T,T,T, T,T,x,x, T,x,x,x, T,T,x,T, T,T,x,x, x,x,x,x, x,x,x,T, T,x,T,T, x,x,T,x, T,T,T,T, T,T,T,T, T,T,T,T, x,x,x,T, x,x},
	{x,T,T,x, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,x,T,T, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, T,x,x,T, T,x,x,x, T,x,T,x, T,x,x,x, x,x,T,x, x,T,x,x, x,x,x,x, T,x,x,x, x,T,x,T, T,T,x,T, x,x,x,x, x,x,T,x, T,x,T,T, x,x,T,x, x,T,x,T, x,T,T,T, T,x,T,x, x,x,x,x, x,x},
	{x,T,T,x, T,T,T,x, x,x,x,T, x,x,x,T, x,x,x,T, x,x,x,T, x,x,T,T, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,T, T,x,x,T, T,x,x,T, T,x,T,T, T,x,x,x, x,x,T,x, T,T,T,T, T,T,x,x, T,x,x,x, T,T,x,T, T,T,x,x, x,x,x,x, x,x,x,T, T,x,T,T, x,x,T,x, T,T,T,T, T,T,T,T, T,T,T,T, x,x,x,T, x,x},
	{x,T,T,x, T,T,T,x, x,x,x,x, x,x,x,T, x,x,x,T, x,x,x,T, x,x,T,T, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, T,x,x,T, T,x,x,x, T,x,T,x, T,x,x,x, x,x,T,x, x,T,x,x, x,x,x,x, T,x,x,x, x,T,x,T, T,T,x,x, x,x,x,x, x,x,x,x, T,x,T,T, T,x,T,x, x,T,x,T, x,T,T,T, T,x,T,x, x,x,x,x, x,x},
	{x,x,x,x, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,T, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,T, x,x,x,x, T,x,x,x, T,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,T,x,x, x,T,x,x, x,x,x,x, x,x,x,x, T,x,T,x, x,x,T,x, x,x,x,x, x,x,T,T, x,x,T,x, x,x,x,x, x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,T,x,T, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x}

	};
} // end Parser

}