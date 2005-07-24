
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
#line  814 "cs.ATG" 
out string qualident) {
		Expect(1);

#line  816 "cs.ATG" 
		qualidentBuilder.Length = 0; qualidentBuilder.Append(t.val); 
		while (
#line  817 "cs.ATG" 
DotAndIdent()) {
			Expect(14);
			Expect(1);

#line  817 "cs.ATG" 
			qualidentBuilder.Append('.');
			qualidentBuilder.Append(t.val); 
			
		}

#line  820 "cs.ATG" 
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
#line  1856 "cs.ATG" 
out Expression expr) {

#line  1857 "cs.ATG" 
		expr = null; Expression expr1 = null, expr2 = null; 
		UnaryExpr(
#line  1859 "cs.ATG" 
out expr);
		if (StartOf(5)) {
			ConditionalOrExpr(
#line  1862 "cs.ATG" 
ref expr);
			if (la.kind == 12) {
				lexer.NextToken();
				Expr(
#line  1862 "cs.ATG" 
out expr1);
				Expect(9);
				Expr(
#line  1862 "cs.ATG" 
out expr2);

#line  1862 "cs.ATG" 
				expr = new ConditionalExpression(expr, expr1, expr2);  
			}
		} else if (StartOf(6)) {

#line  1864 "cs.ATG" 
			AssignmentOperatorType op; Expression val; 
			AssignmentOperator(
#line  1864 "cs.ATG" 
out op);
			Expr(
#line  1864 "cs.ATG" 
out val);

#line  1864 "cs.ATG" 
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
#line  988 "cs.ATG" 
Modifiers m) {
		switch (la.kind) {
		case 87: {
			lexer.NextToken();

#line  990 "cs.ATG" 
			m.Add(Modifier.New); 
			break;
		}
		case 96: {
			lexer.NextToken();

#line  991 "cs.ATG" 
			m.Add(Modifier.Public); 
			break;
		}
		case 95: {
			lexer.NextToken();

#line  992 "cs.ATG" 
			m.Add(Modifier.Protected); 
			break;
		}
		case 82: {
			lexer.NextToken();

#line  993 "cs.ATG" 
			m.Add(Modifier.Internal); 
			break;
		}
		case 94: {
			lexer.NextToken();

#line  994 "cs.ATG" 
			m.Add(Modifier.Private); 
			break;
		}
		case 117: {
			lexer.NextToken();

#line  995 "cs.ATG" 
			m.Add(Modifier.Unsafe); 
			break;
		}
		case 47: {
			lexer.NextToken();

#line  996 "cs.ATG" 
			m.Add(Modifier.Abstract); 
			break;
		}
		case 101: {
			lexer.NextToken();

#line  997 "cs.ATG" 
			m.Add(Modifier.Sealed); 
			break;
		}
		case 105: {
			lexer.NextToken();

#line  998 "cs.ATG" 
			m.Add(Modifier.Static); 
			break;
		}
		case 1: {
			lexer.NextToken();

#line  999 "cs.ATG" 
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
			
			newType.Type = Types.Class;
			
			Expect(1);

#line  719 "cs.ATG" 
			newType.Name = t.val; 
			if (la.kind == 22) {
				TypeParameterList(
#line  722 "cs.ATG" 
templates);
			}
			if (la.kind == 9) {
				ClassBase(
#line  724 "cs.ATG" 
out names);

#line  724 "cs.ATG" 
				newType.BaseTypes = names; 
			}

#line  724 "cs.ATG" 
			newType.StartLocation = t.EndLocation; 
			while (
#line  727 "cs.ATG" 
IdentIsWhere()) {
				TypeParameterConstraintsClause(
#line  727 "cs.ATG" 
templates);
			}
			ClassBody();
			if (la.kind == 11) {
				lexer.NextToken();
			}

#line  730 "cs.ATG" 
			newType.EndLocation = t.Location; 
			compilationUnit.BlockEnd();
			
		} else if (StartOf(7)) {

#line  733 "cs.ATG" 
			m.Check(Modifier.StructsInterfacesEnumsDelegates); 
			if (la.kind == 107) {
				lexer.NextToken();

#line  734 "cs.ATG" 
				TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
				templates = newType.Templates;
				compilationUnit.AddChild(newType);
				compilationUnit.BlockStart(newType);
				newType.Type = Types.Struct; 
				
				Expect(1);

#line  740 "cs.ATG" 
				newType.Name = t.val; 
				if (la.kind == 22) {
					TypeParameterList(
#line  743 "cs.ATG" 
templates);
				}
				if (la.kind == 9) {
					StructInterfaces(
#line  745 "cs.ATG" 
out names);

#line  745 "cs.ATG" 
					newType.BaseTypes = names; 
				}

#line  745 "cs.ATG" 
				newType.StartLocation = t.EndLocation; 
				while (
#line  748 "cs.ATG" 
IdentIsWhere()) {
					TypeParameterConstraintsClause(
#line  748 "cs.ATG" 
templates);
				}
				StructBody();
				if (la.kind == 11) {
					lexer.NextToken();
				}

#line  752 "cs.ATG" 
				newType.EndLocation = t.Location; 
				compilationUnit.BlockEnd();
				
			} else if (la.kind == 81) {
				lexer.NextToken();

#line  756 "cs.ATG" 
				TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
				templates = newType.Templates;
				compilationUnit.AddChild(newType);
				compilationUnit.BlockStart(newType);
				newType.Type = Types.Interface;
				
				Expect(1);

#line  762 "cs.ATG" 
				newType.Name = t.val; 
				if (la.kind == 22) {
					TypeParameterList(
#line  765 "cs.ATG" 
templates);
				}
				if (la.kind == 9) {
					InterfaceBase(
#line  767 "cs.ATG" 
out names);

#line  767 "cs.ATG" 
					newType.BaseTypes = names; 
				}

#line  767 "cs.ATG" 
				newType.StartLocation = t.EndLocation; 
				while (
#line  770 "cs.ATG" 
IdentIsWhere()) {
					TypeParameterConstraintsClause(
#line  770 "cs.ATG" 
templates);
				}
				InterfaceBody();
				if (la.kind == 11) {
					lexer.NextToken();
				}

#line  773 "cs.ATG" 
				newType.EndLocation = t.Location; 
				compilationUnit.BlockEnd();
				
			} else if (la.kind == 66) {
				lexer.NextToken();

#line  777 "cs.ATG" 
				TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
				compilationUnit.AddChild(newType);
				compilationUnit.BlockStart(newType);
				newType.Type = Types.Enum;
				
				Expect(1);

#line  782 "cs.ATG" 
				newType.Name = t.val; 
				if (la.kind == 9) {
					lexer.NextToken();
					IntegralType(
#line  783 "cs.ATG" 
out name);

#line  783 "cs.ATG" 
					newType.BaseTypes.Add(new TypeReference(name)); 
				}

#line  784 "cs.ATG" 
				newType.StartLocation = t.EndLocation; 
				EnumBody();
				if (la.kind == 11) {
					lexer.NextToken();
				}

#line  786 "cs.ATG" 
				newType.EndLocation = t.Location; 
				compilationUnit.BlockEnd();
				
			} else {
				lexer.NextToken();

#line  790 "cs.ATG" 
				DelegateDeclaration delegateDeclr = new DelegateDeclaration(m.Modifier, attributes);
				templates = delegateDeclr.Templates;
				delegateDeclr.StartLocation = t.Location;
				
				if (
#line  794 "cs.ATG" 
NotVoidPointer()) {
					Expect(121);

#line  794 "cs.ATG" 
					delegateDeclr.ReturnType = new TypeReference("void", 0, null); 
				} else if (StartOf(8)) {
					Type(
#line  795 "cs.ATG" 
out type);

#line  795 "cs.ATG" 
					delegateDeclr.ReturnType = type; 
				} else SynErr(129);
				Expect(1);

#line  797 "cs.ATG" 
				delegateDeclr.Name = t.val; 
				if (la.kind == 22) {
					TypeParameterList(
#line  800 "cs.ATG" 
templates);
				}
				Expect(19);
				if (StartOf(9)) {
					FormalParameterList(
#line  802 "cs.ATG" 
p);

#line  802 "cs.ATG" 
					delegateDeclr.Parameters = p; 
				}
				Expect(20);
				while (
#line  806 "cs.ATG" 
IdentIsWhere()) {
					TypeParameterConstraintsClause(
#line  806 "cs.ATG" 
templates);
				}
				Expect(11);

#line  808 "cs.ATG" 
				delegateDeclr.EndLocation = t.Location;
				compilationUnit.AddChild(delegateDeclr);
				
			}
		} else SynErr(130);
	}

	void TypeParameterList(
#line  2239 "cs.ATG" 
List<TemplateDefinition> templates) {

#line  2241 "cs.ATG" 
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		
		Expect(22);
		while (la.kind == 17) {
			AttributeSection(
#line  2245 "cs.ATG" 
out section);

#line  2245 "cs.ATG" 
			attributes.Add(section); 
		}
		Expect(1);

#line  2246 "cs.ATG" 
		templates.Add(new TemplateDefinition(t.val, attributes)); 
		while (la.kind == 13) {
			lexer.NextToken();
			while (la.kind == 17) {
				AttributeSection(
#line  2247 "cs.ATG" 
out section);

#line  2247 "cs.ATG" 
				attributes.Add(section); 
			}
			Expect(1);

#line  2248 "cs.ATG" 
			templates.Add(new TemplateDefinition(t.val, attributes)); 
		}
		Expect(21);
	}

	void ClassBase(
#line  823 "cs.ATG" 
out List<TypeReference> names) {

#line  825 "cs.ATG" 
		TypeReference typeRef;
		names = new List<TypeReference>();
		
		Expect(9);
		ClassType(
#line  829 "cs.ATG" 
out typeRef);

#line  829 "cs.ATG" 
		if (typeRef != null) { names.Add(typeRef); } 
		while (la.kind == 13) {
			lexer.NextToken();
			TypeName(
#line  830 "cs.ATG" 
out typeRef);

#line  830 "cs.ATG" 
			if (typeRef != null) { names.Add(typeRef); } 
		}
	}

	void TypeParameterConstraintsClause(
#line  2252 "cs.ATG" 
List<TemplateDefinition> templates) {

#line  2253 "cs.ATG" 
		string name = ""; TypeReference type; 
		Expect(1);

#line  2255 "cs.ATG" 
		if (t.val != "where") Error("where expected"); 
		Expect(1);

#line  2256 "cs.ATG" 
		name = t.val; 
		Expect(9);
		TypeParameterConstraintsClauseBase(
#line  2258 "cs.ATG" 
out type);

#line  2259 "cs.ATG" 
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
#line  2268 "cs.ATG" 
out type);

#line  2269 "cs.ATG" 
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

#line  834 "cs.ATG" 
		AttributeSection section; 
		Expect(15);
		while (StartOf(10)) {

#line  837 "cs.ATG" 
			List<AttributeSection> attributes = new List<AttributeSection>();
			Modifiers m = new Modifiers();
			
			while (la.kind == 17) {
				AttributeSection(
#line  840 "cs.ATG" 
out section);

#line  840 "cs.ATG" 
				attributes.Add(section); 
			}
			while (StartOf(11)) {
				MemberModifier(
#line  841 "cs.ATG" 
m);
			}
			ClassMemberDecl(
#line  842 "cs.ATG" 
m, attributes);
		}
		Expect(16);
	}

	void StructInterfaces(
#line  847 "cs.ATG" 
out List<TypeReference> names) {

#line  849 "cs.ATG" 
		TypeReference typeRef;
		names = new List<TypeReference>();
		
		Expect(9);
		TypeName(
#line  853 "cs.ATG" 
out typeRef);

#line  853 "cs.ATG" 
		if (typeRef != null) { names.Add(typeRef); } 
		while (la.kind == 13) {
			lexer.NextToken();
			TypeName(
#line  854 "cs.ATG" 
out typeRef);

#line  854 "cs.ATG" 
			if (typeRef != null) { names.Add(typeRef); } 
		}
	}

	void StructBody() {

#line  858 "cs.ATG" 
		AttributeSection section; 
		Expect(15);
		while (StartOf(12)) {

#line  861 "cs.ATG" 
			List<AttributeSection> attributes = new List<AttributeSection>();
			Modifiers m = new Modifiers();
			
			while (la.kind == 17) {
				AttributeSection(
#line  864 "cs.ATG" 
out section);

#line  864 "cs.ATG" 
				attributes.Add(section); 
			}
			while (StartOf(11)) {
				MemberModifier(
#line  865 "cs.ATG" 
m);
			}
			StructMemberDecl(
#line  866 "cs.ATG" 
m, attributes);
		}
		Expect(16);
	}

	void InterfaceBase(
#line  871 "cs.ATG" 
out List<TypeReference> names) {

#line  873 "cs.ATG" 
		TypeReference typeRef;
		names = new List<TypeReference>();
		
		Expect(9);
		TypeName(
#line  877 "cs.ATG" 
out typeRef);

#line  877 "cs.ATG" 
		if (typeRef != null) { names.Add(typeRef); } 
		while (la.kind == 13) {
			lexer.NextToken();
			TypeName(
#line  878 "cs.ATG" 
out typeRef);

#line  878 "cs.ATG" 
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
#line  1010 "cs.ATG" 
out string name) {

#line  1010 "cs.ATG" 
		name = ""; 
		switch (la.kind) {
		case 100: {
			lexer.NextToken();

#line  1012 "cs.ATG" 
			name = "sbyte"; 
			break;
		}
		case 52: {
			lexer.NextToken();

#line  1013 "cs.ATG" 
			name = "byte"; 
			break;
		}
		case 102: {
			lexer.NextToken();

#line  1014 "cs.ATG" 
			name = "short"; 
			break;
		}
		case 118: {
			lexer.NextToken();

#line  1015 "cs.ATG" 
			name = "ushort"; 
			break;
		}
		case 80: {
			lexer.NextToken();

#line  1016 "cs.ATG" 
			name = "int"; 
			break;
		}
		case 114: {
			lexer.NextToken();

#line  1017 "cs.ATG" 
			name = "uint"; 
			break;
		}
		case 85: {
			lexer.NextToken();

#line  1018 "cs.ATG" 
			name = "long"; 
			break;
		}
		case 115: {
			lexer.NextToken();

#line  1019 "cs.ATG" 
			name = "ulong"; 
			break;
		}
		case 55: {
			lexer.NextToken();

#line  1020 "cs.ATG" 
			name = "char"; 
			break;
		}
		default: SynErr(131); break;
		}
	}

	void EnumBody() {

#line  884 "cs.ATG" 
		FieldDeclaration f; 
		Expect(15);
		if (la.kind == 1 || la.kind == 17) {
			EnumMemberDecl(
#line  886 "cs.ATG" 
out f);

#line  886 "cs.ATG" 
			compilationUnit.AddChild(f); 
			while (
#line  887 "cs.ATG" 
NotFinalComma()) {
				Expect(13);
				EnumMemberDecl(
#line  887 "cs.ATG" 
out f);

#line  887 "cs.ATG" 
				compilationUnit.AddChild(f); 
			}
			if (la.kind == 13) {
				lexer.NextToken();
			}
		}
		Expect(16);
	}

	void Type(
#line  892 "cs.ATG" 
out TypeReference type) {

#line  894 "cs.ATG" 
		string name;
		int pointer = 0;
		type = null;
		
		if (la.kind == 1 || la.kind == 89 || la.kind == 106) {
			ClassType(
#line  899 "cs.ATG" 
out type);
		} else if (StartOf(14)) {
			SimpleType(
#line  900 "cs.ATG" 
out name);

#line  900 "cs.ATG" 
			type = new TypeReference(name); 
		} else if (la.kind == 121) {
			lexer.NextToken();
			Expect(6);

#line  901 "cs.ATG" 
			pointer = 1; type = new TypeReference("void"); 
		} else SynErr(132);

#line  902 "cs.ATG" 
		List<int> r = new List<int>(); 
		while (
#line  904 "cs.ATG" 
IsPointerOrDims()) {

#line  904 "cs.ATG" 
			int i = 0; 
			if (la.kind == 6) {
				lexer.NextToken();

#line  905 "cs.ATG" 
				++pointer; 
			} else if (la.kind == 17) {
				lexer.NextToken();
				while (la.kind == 13) {
					lexer.NextToken();

#line  906 "cs.ATG" 
					++i; 
				}
				Expect(18);

#line  906 "cs.ATG" 
				r.Add(i); 
			} else SynErr(133);
		}

#line  909 "cs.ATG" 
		type.RankSpecifier = r.ToArray();
		type.PointerNestingLevel = pointer;
		
	}

	void FormalParameterList(
#line  943 "cs.ATG" 
List<ParameterDeclarationExpression> parameter) {

#line  946 "cs.ATG" 
		ParameterDeclarationExpression p;
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		
		while (la.kind == 17) {
			AttributeSection(
#line  951 "cs.ATG" 
out section);

#line  951 "cs.ATG" 
			attributes.Add(section); 
		}
		if (StartOf(15)) {
			FixedParameter(
#line  953 "cs.ATG" 
out p);

#line  953 "cs.ATG" 
			bool paramsFound = false;
			p.Attributes = attributes;
			parameter.Add(p);
			
			while (la.kind == 13) {
				lexer.NextToken();

#line  958 "cs.ATG" 
				attributes = new List<AttributeSection>(); if (paramsFound) Error("params array must be at end of parameter list"); 
				while (la.kind == 17) {
					AttributeSection(
#line  959 "cs.ATG" 
out section);

#line  959 "cs.ATG" 
					attributes.Add(section); 
				}
				if (StartOf(15)) {
					FixedParameter(
#line  961 "cs.ATG" 
out p);

#line  961 "cs.ATG" 
					p.Attributes = attributes; parameter.Add(p); 
				} else if (la.kind == 93) {
					ParameterArray(
#line  962 "cs.ATG" 
out p);

#line  962 "cs.ATG" 
					paramsFound = true; p.Attributes = attributes; parameter.Add(p); 
				} else SynErr(134);
			}
		} else if (la.kind == 93) {
			ParameterArray(
#line  965 "cs.ATG" 
out p);

#line  965 "cs.ATG" 
			p.Attributes = attributes; parameter.Add(p); 
		} else SynErr(135);
	}

	void ClassType(
#line  1002 "cs.ATG" 
out TypeReference typeRef) {

#line  1003 "cs.ATG" 
		TypeReference r; typeRef = null; 
		if (la.kind == 1) {
			TypeName(
#line  1005 "cs.ATG" 
out r);

#line  1005 "cs.ATG" 
			typeRef = r; 
		} else if (la.kind == 89) {
			lexer.NextToken();

#line  1006 "cs.ATG" 
			typeRef = new TypeReference("object"); 
		} else if (la.kind == 106) {
			lexer.NextToken();

#line  1007 "cs.ATG" 
			typeRef = new TypeReference("string"); 
		} else SynErr(136);
	}

	void TypeName(
#line  2204 "cs.ATG" 
out TypeReference typeRef) {

#line  2205 "cs.ATG" 
		List<TypeReference> typeArguments = null;
		string alias = null;
		string qualident;
		
		if (
#line  2210 "cs.ATG" 
la.kind == Tokens.Identifier && Peek(1).kind == Tokens.DoubleColon) {
			lexer.NextToken();

#line  2211 "cs.ATG" 
			alias = t.val; 
			Expect(10);
		}
		Qualident(
#line  2214 "cs.ATG" 
out qualident);
		if (la.kind == 22) {
			TypeArgumentList(
#line  2215 "cs.ATG" 
out typeArguments);
		}

#line  2217 "cs.ATG" 
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
#line  1023 "cs.ATG" 
Modifiers m) {
		switch (la.kind) {
		case 47: {
			lexer.NextToken();

#line  1025 "cs.ATG" 
			m.Add(Modifier.Abstract); 
			break;
		}
		case 69: {
			lexer.NextToken();

#line  1026 "cs.ATG" 
			m.Add(Modifier.Extern); 
			break;
		}
		case 82: {
			lexer.NextToken();

#line  1027 "cs.ATG" 
			m.Add(Modifier.Internal); 
			break;
		}
		case 87: {
			lexer.NextToken();

#line  1028 "cs.ATG" 
			m.Add(Modifier.New); 
			break;
		}
		case 92: {
			lexer.NextToken();

#line  1029 "cs.ATG" 
			m.Add(Modifier.Override); 
			break;
		}
		case 94: {
			lexer.NextToken();

#line  1030 "cs.ATG" 
			m.Add(Modifier.Private); 
			break;
		}
		case 95: {
			lexer.NextToken();

#line  1031 "cs.ATG" 
			m.Add(Modifier.Protected); 
			break;
		}
		case 96: {
			lexer.NextToken();

#line  1032 "cs.ATG" 
			m.Add(Modifier.Public); 
			break;
		}
		case 97: {
			lexer.NextToken();

#line  1033 "cs.ATG" 
			m.Add(Modifier.Readonly); 
			break;
		}
		case 101: {
			lexer.NextToken();

#line  1034 "cs.ATG" 
			m.Add(Modifier.Sealed); 
			break;
		}
		case 105: {
			lexer.NextToken();

#line  1035 "cs.ATG" 
			m.Add(Modifier.Static); 
			break;
		}
		case 117: {
			lexer.NextToken();

#line  1036 "cs.ATG" 
			m.Add(Modifier.Unsafe); 
			break;
		}
		case 120: {
			lexer.NextToken();

#line  1037 "cs.ATG" 
			m.Add(Modifier.Virtual); 
			break;
		}
		case 122: {
			lexer.NextToken();

#line  1038 "cs.ATG" 
			m.Add(Modifier.Volatile); 
			break;
		}
		default: SynErr(137); break;
		}
	}

	void ClassMemberDecl(
#line  1271 "cs.ATG" 
Modifiers m, List<AttributeSection> attributes) {

#line  1272 "cs.ATG" 
		Statement stmt = null; 
		if (StartOf(16)) {
			StructMemberDecl(
#line  1274 "cs.ATG" 
m, attributes);
		} else if (la.kind == 26) {

#line  1275 "cs.ATG" 
			m.Check(Modifier.Destructors); Point startPos = t.Location; 
			lexer.NextToken();
			Expect(1);

#line  1276 "cs.ATG" 
			DestructorDeclaration d = new DestructorDeclaration(t.val, m.Modifier, attributes); 
			d.Modifier = m.Modifier;
			d.StartLocation = startPos;
			
			Expect(19);
			Expect(20);

#line  1280 "cs.ATG" 
			d.EndLocation = t.EndLocation; 
			if (la.kind == 15) {
				Block(
#line  1280 "cs.ATG" 
out stmt);
			} else if (la.kind == 11) {
				lexer.NextToken();
			} else SynErr(138);

#line  1281 "cs.ATG" 
			d.Body = (BlockStatement)stmt;
			compilationUnit.AddChild(d);
			
		} else SynErr(139);
	}

	void StructMemberDecl(
#line  1041 "cs.ATG" 
Modifiers m, List<AttributeSection> attributes) {

#line  1043 "cs.ATG" 
		string qualident = null;
		TypeReference type;
		Expression expr;
		List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
		Statement stmt = null;
		List<VariableDeclaration> variableDeclarators = new List<VariableDeclaration>();
		List<TemplateDefinition> templates = new List<TemplateDefinition>();
		
		if (la.kind == 58) {

#line  1052 "cs.ATG" 
			m.Check(Modifier.Constants); 
			lexer.NextToken();

#line  1053 "cs.ATG" 
			Point startPos = t.Location; 
			Type(
#line  1054 "cs.ATG" 
out type);
			Expect(1);

#line  1054 "cs.ATG" 
			FieldDeclaration fd = new FieldDeclaration(attributes, type, m.Modifier | Modifier.Const);
			fd.StartLocation = startPos;
			VariableDeclaration f = new VariableDeclaration(t.val);
			fd.Fields.Add(f);
			
			Expect(3);
			Expr(
#line  1059 "cs.ATG" 
out expr);

#line  1059 "cs.ATG" 
			f.Initializer = expr; 
			while (la.kind == 13) {
				lexer.NextToken();
				Expect(1);

#line  1060 "cs.ATG" 
				f = new VariableDeclaration(t.val);
				fd.Fields.Add(f);
				
				Expect(3);
				Expr(
#line  1063 "cs.ATG" 
out expr);

#line  1063 "cs.ATG" 
				f.Initializer = expr; 
			}
			Expect(11);

#line  1064 "cs.ATG" 
			fd.EndLocation = t.EndLocation; compilationUnit.AddChild(fd); 
		} else if (
#line  1067 "cs.ATG" 
NotVoidPointer()) {

#line  1067 "cs.ATG" 
			m.Check(Modifier.PropertysEventsMethods); 
			Expect(121);

#line  1068 "cs.ATG" 
			Point startPos = t.Location; 
			Qualident(
#line  1069 "cs.ATG" 
out qualident);
			if (la.kind == 22) {
				TypeParameterList(
#line  1071 "cs.ATG" 
templates);
			}
			Expect(19);
			if (StartOf(9)) {
				FormalParameterList(
#line  1074 "cs.ATG" 
p);
			}
			Expect(20);

#line  1074 "cs.ATG" 
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
#line  1087 "cs.ATG" 
IdentIsWhere()) {
				TypeParameterConstraintsClause(
#line  1087 "cs.ATG" 
templates);
			}
			if (la.kind == 15) {
				Block(
#line  1089 "cs.ATG" 
out stmt);
			} else if (la.kind == 11) {
				lexer.NextToken();
			} else SynErr(140);

#line  1089 "cs.ATG" 
			compilationUnit.BlockEnd();
			methodDeclaration.Body  = (BlockStatement)stmt;
			
		} else if (la.kind == 67) {

#line  1093 "cs.ATG" 
			m.Check(Modifier.PropertysEventsMethods); 
			lexer.NextToken();

#line  1094 "cs.ATG" 
			EventDeclaration eventDecl = new EventDeclaration(m.Modifier, attributes);
			eventDecl.StartLocation = t.Location;
			compilationUnit.AddChild(eventDecl);
			compilationUnit.BlockStart(eventDecl);
			EventAddRegion addBlock = null;
			EventRemoveRegion removeBlock = null;
			
			Type(
#line  1101 "cs.ATG" 
out type);

#line  1101 "cs.ATG" 
			eventDecl.TypeReference = type; 
			if (
#line  1103 "cs.ATG" 
IsVarDecl()) {
				VariableDeclarator(
#line  1103 "cs.ATG" 
variableDeclarators);
				while (la.kind == 13) {
					lexer.NextToken();
					VariableDeclarator(
#line  1104 "cs.ATG" 
variableDeclarators);
				}
				Expect(11);

#line  1104 "cs.ATG" 
				eventDecl.VariableDeclarators = variableDeclarators; eventDecl.EndLocation = t.EndLocation;  
			} else if (la.kind == 1) {
				Qualident(
#line  1105 "cs.ATG" 
out qualident);

#line  1105 "cs.ATG" 
				eventDecl.Name = qualident; eventDecl.EndLocation = t.EndLocation;  
				Expect(15);

#line  1106 "cs.ATG" 
				eventDecl.BodyStart = t.Location; 
				EventAccessorDecls(
#line  1107 "cs.ATG" 
out addBlock, out removeBlock);
				Expect(16);

#line  1108 "cs.ATG" 
				eventDecl.BodyEnd   = t.EndLocation; 
			} else SynErr(141);

#line  1109 "cs.ATG" 
			compilationUnit.BlockEnd();
			
			eventDecl.AddRegion = addBlock;
			eventDecl.RemoveRegion = removeBlock;
			
		} else if (
#line  1116 "cs.ATG" 
IdentAndLPar()) {

#line  1116 "cs.ATG" 
			m.Check(Modifier.Constructors | Modifier.StaticConstructors); 
			Expect(1);

#line  1117 "cs.ATG" 
			string name = t.val; Point startPos = t.Location; 
			Expect(19);
			if (StartOf(9)) {

#line  1117 "cs.ATG" 
				m.Check(Modifier.Constructors); 
				FormalParameterList(
#line  1118 "cs.ATG" 
p);
			}
			Expect(20);

#line  1120 "cs.ATG" 
			ConstructorInitializer init = null;  
			if (la.kind == 9) {

#line  1121 "cs.ATG" 
				m.Check(Modifier.Constructors); 
				ConstructorInitializer(
#line  1122 "cs.ATG" 
out init);
			}

#line  1124 "cs.ATG" 
			ConstructorDeclaration cd = new ConstructorDeclaration(name, m.Modifier, p, init, attributes); 
			cd.StartLocation = startPos;
			cd.EndLocation   = t.EndLocation;
			
			if (la.kind == 15) {
				Block(
#line  1129 "cs.ATG" 
out stmt);
			} else if (la.kind == 11) {
				lexer.NextToken();
			} else SynErr(142);

#line  1129 "cs.ATG" 
			cd.Body = (BlockStatement)stmt; compilationUnit.AddChild(cd); 
		} else if (la.kind == 68 || la.kind == 78) {

#line  1132 "cs.ATG" 
			m.Check(Modifier.Operators);
			if (m.isNone) Error("at least one modifier must be set"); 
			bool isImplicit = true;
			
			if (la.kind == 78) {
				lexer.NextToken();
			} else {
				lexer.NextToken();

#line  1136 "cs.ATG" 
				isImplicit = false; 
			}
			Expect(90);
			Type(
#line  1137 "cs.ATG" 
out type);

#line  1137 "cs.ATG" 
			TypeReference operatorType = type; 
			Expect(19);
			Type(
#line  1138 "cs.ATG" 
out type);
			Expect(1);

#line  1138 "cs.ATG" 
			string varName = t.val; 
			Expect(20);
			if (la.kind == 15) {
				Block(
#line  1138 "cs.ATG" 
out stmt);
			} else if (la.kind == 11) {
				lexer.NextToken();

#line  1138 "cs.ATG" 
				stmt = null; 
			} else SynErr(143);

#line  1141 "cs.ATG" 
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
#line  1154 "cs.ATG" 
m, attributes);
		} else if (StartOf(8)) {
			Type(
#line  1155 "cs.ATG" 
out type);

#line  1155 "cs.ATG" 
			Point startPos = t.Location;  
			if (la.kind == 90) {

#line  1157 "cs.ATG" 
				OverloadableOperatorType op;
				m.Check(Modifier.Operators);
				if (m.isNone) Error("at least one modifier must be set");
				
				lexer.NextToken();
				OverloadableOperator(
#line  1161 "cs.ATG" 
out op);

#line  1161 "cs.ATG" 
				TypeReference firstType, secondType = null; string secondName = null; 
				Expect(19);
				Type(
#line  1162 "cs.ATG" 
out firstType);
				Expect(1);

#line  1162 "cs.ATG" 
				string firstName = t.val; 
				if (la.kind == 13) {
					lexer.NextToken();
					Type(
#line  1163 "cs.ATG" 
out secondType);
					Expect(1);

#line  1163 "cs.ATG" 
					secondName = t.val; 
				} else if (la.kind == 20) {
				} else SynErr(144);
				Expect(20);
				if (la.kind == 15) {
					Block(
#line  1171 "cs.ATG" 
out stmt);
				} else if (la.kind == 11) {
					lexer.NextToken();
				} else SynErr(145);

#line  1173 "cs.ATG" 
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
#line  1188 "cs.ATG" 
IsVarDecl()) {

#line  1188 "cs.ATG" 
				m.Check(Modifier.Fields); 
				FieldDeclaration fd = new FieldDeclaration(attributes, type, m.Modifier);
				fd.StartLocation = startPos; 
				
				VariableDeclarator(
#line  1192 "cs.ATG" 
variableDeclarators);
				while (la.kind == 13) {
					lexer.NextToken();
					VariableDeclarator(
#line  1193 "cs.ATG" 
variableDeclarators);
				}
				Expect(11);

#line  1194 "cs.ATG" 
				fd.EndLocation = t.EndLocation; fd.Fields = variableDeclarators; compilationUnit.AddChild(fd); 
			} else if (la.kind == 109) {

#line  1197 "cs.ATG" 
				m.Check(Modifier.Indexers); 
				lexer.NextToken();
				Expect(17);
				FormalParameterList(
#line  1198 "cs.ATG" 
p);
				Expect(18);

#line  1198 "cs.ATG" 
				Point endLocation = t.EndLocation; 
				Expect(15);

#line  1199 "cs.ATG" 
				IndexerDeclaration indexer = new IndexerDeclaration(type, p, m.Modifier, attributes);
				indexer.StartLocation = startPos;
				indexer.EndLocation   = endLocation;
				indexer.BodyStart     = t.Location;
				PropertyGetRegion getRegion;
				PropertySetRegion setRegion;
				
				AccessorDecls(
#line  1206 "cs.ATG" 
out getRegion, out setRegion);
				Expect(16);

#line  1207 "cs.ATG" 
				indexer.BodyEnd    = t.EndLocation;
				indexer.GetRegion = getRegion;
				indexer.SetRegion = setRegion;
				compilationUnit.AddChild(indexer);
				
			} else if (la.kind == 1) {
				Qualident(
#line  1212 "cs.ATG" 
out qualident);

#line  1212 "cs.ATG" 
				Point qualIdentEndLocation = t.EndLocation; 
				if (la.kind == 15 || la.kind == 19 || la.kind == 22) {
					if (la.kind == 19 || la.kind == 22) {

#line  1215 "cs.ATG" 
						m.Check(Modifier.PropertysEventsMethods); 
						if (la.kind == 22) {
							TypeParameterList(
#line  1217 "cs.ATG" 
templates);
						}
						Expect(19);
						if (StartOf(9)) {
							FormalParameterList(
#line  1218 "cs.ATG" 
p);
						}
						Expect(20);

#line  1219 "cs.ATG" 
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
#line  1229 "cs.ATG" 
IdentIsWhere()) {
							TypeParameterConstraintsClause(
#line  1229 "cs.ATG" 
templates);
						}
						if (la.kind == 15) {
							Block(
#line  1230 "cs.ATG" 
out stmt);
						} else if (la.kind == 11) {
							lexer.NextToken();
						} else SynErr(146);

#line  1230 "cs.ATG" 
						methodDeclaration.Body  = (BlockStatement)stmt; 
					} else {
						lexer.NextToken();

#line  1233 "cs.ATG" 
						PropertyDeclaration pDecl = new PropertyDeclaration(qualident, type, m.Modifier, attributes); 
						pDecl.StartLocation = startPos;
						pDecl.EndLocation   = qualIdentEndLocation;
						pDecl.BodyStart   = t.Location;
						PropertyGetRegion getRegion;
						PropertySetRegion setRegion;
						
						AccessorDecls(
#line  1240 "cs.ATG" 
out getRegion, out setRegion);
						Expect(16);

#line  1242 "cs.ATG" 
						pDecl.GetRegion = getRegion;
						pDecl.SetRegion = setRegion;
						pDecl.BodyEnd = t.EndLocation;
						compilationUnit.AddChild(pDecl);
						
					}
				} else if (la.kind == 14) {

#line  1250 "cs.ATG" 
					m.Check(Modifier.Indexers); 
					lexer.NextToken();
					Expect(109);
					Expect(17);
					FormalParameterList(
#line  1251 "cs.ATG" 
p);
					Expect(18);

#line  1252 "cs.ATG" 
					IndexerDeclaration indexer = new IndexerDeclaration(type, p, m.Modifier, attributes);
					indexer.StartLocation = startPos;
					indexer.EndLocation   = t.EndLocation;
					indexer.NamespaceName = qualident;
					PropertyGetRegion getRegion;
					PropertySetRegion setRegion;
					
					Expect(15);

#line  1259 "cs.ATG" 
					Point bodyStart = t.Location; 
					AccessorDecls(
#line  1260 "cs.ATG" 
out getRegion, out setRegion);
					Expect(16);

#line  1261 "cs.ATG" 
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

#line  1288 "cs.ATG" 
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
#line  1301 "cs.ATG" 
out section);

#line  1301 "cs.ATG" 
			attributes.Add(section); 
		}
		if (la.kind == 87) {
			lexer.NextToken();

#line  1302 "cs.ATG" 
			mod = Modifier.New; startLocation = t.Location; 
		}
		if (
#line  1305 "cs.ATG" 
NotVoidPointer()) {
			Expect(121);

#line  1305 "cs.ATG" 
			if (startLocation.X == -1) startLocation = t.Location; 
			Expect(1);

#line  1305 "cs.ATG" 
			name = t.val; 
			Expect(19);
			if (StartOf(9)) {
				FormalParameterList(
#line  1306 "cs.ATG" 
parameters);
			}
			Expect(20);
			Expect(11);

#line  1306 "cs.ATG" 
			MethodDeclaration md = new MethodDeclaration(name, mod, new TypeReference("void"), parameters, attributes);
			md.StartLocation = startLocation;
			md.EndLocation = t.EndLocation;
			compilationUnit.AddChild(md);
			
		} else if (StartOf(18)) {
			if (StartOf(8)) {
				Type(
#line  1312 "cs.ATG" 
out type);

#line  1312 "cs.ATG" 
				if (startLocation.X == -1) startLocation = t.Location; 
				if (la.kind == 1) {
					lexer.NextToken();

#line  1314 "cs.ATG" 
					name = t.val; Point qualIdentEndLocation = t.EndLocation; 
					if (la.kind == 19 || la.kind == 22) {
						if (la.kind == 22) {
							TypeParameterList(
#line  1318 "cs.ATG" 
templates);
						}
						Expect(19);
						if (StartOf(9)) {
							FormalParameterList(
#line  1319 "cs.ATG" 
parameters);
						}
						Expect(20);
						while (
#line  1321 "cs.ATG" 
IdentIsWhere()) {
							TypeParameterConstraintsClause(
#line  1321 "cs.ATG" 
templates);
						}
						Expect(11);

#line  1322 "cs.ATG" 
						MethodDeclaration md = new MethodDeclaration(name, mod, type, parameters, attributes);
						                                      md.StartLocation = startLocation;
						                                      md.EndLocation = t.EndLocation;
						                                      compilationUnit.AddChild(md);
						                                   
					} else if (la.kind == 15) {

#line  1328 "cs.ATG" 
						PropertyDeclaration pd = new PropertyDeclaration(name, type, mod, attributes); compilationUnit.AddChild(pd); 
						lexer.NextToken();

#line  1329 "cs.ATG" 
						Point bodyStart = t.Location;
						InterfaceAccessors(
#line  1329 "cs.ATG" 
out getBlock, out setBlock);
						Expect(16);

#line  1329 "cs.ATG" 
						pd.GetRegion = getBlock; pd.SetRegion = setBlock; pd.StartLocation = startLocation; pd.EndLocation = qualIdentEndLocation; pd.BodyStart = bodyStart; pd.BodyEnd = t.EndLocation; 
					} else SynErr(150);
				} else if (la.kind == 109) {
					lexer.NextToken();
					Expect(17);
					FormalParameterList(
#line  1332 "cs.ATG" 
parameters);
					Expect(18);

#line  1332 "cs.ATG" 
					Point bracketEndLocation = t.EndLocation; 

#line  1332 "cs.ATG" 
					IndexerDeclaration id = new IndexerDeclaration(type, parameters, mod, attributes); compilationUnit.AddChild(id); 
					Expect(15);

#line  1333 "cs.ATG" 
					Point bodyStart = t.Location;
					InterfaceAccessors(
#line  1333 "cs.ATG" 
out getBlock, out setBlock);
					Expect(16);

#line  1333 "cs.ATG" 
					id.GetRegion = getBlock; id.SetRegion = setBlock; id.StartLocation = startLocation;  id.EndLocation = bracketEndLocation; id.BodyStart = bodyStart; id.BodyEnd = t.EndLocation;
				} else SynErr(151);
			} else {
				lexer.NextToken();

#line  1336 "cs.ATG" 
				if (startLocation.X == -1) startLocation = t.Location; 
				Type(
#line  1336 "cs.ATG" 
out type);
				Expect(1);

#line  1336 "cs.ATG" 
				EventDeclaration ed = new EventDeclaration(type, t.val, mod, attributes);
				compilationUnit.AddChild(ed);
				
				Expect(11);

#line  1339 "cs.ATG" 
				ed.StartLocation = startLocation; ed.EndLocation = t.EndLocation; 
			}
		} else SynErr(152);
	}

	void EnumMemberDecl(
#line  1344 "cs.ATG" 
out FieldDeclaration f) {

#line  1346 "cs.ATG" 
		Expression expr = null;
		List<AttributeSection> attributes = new List<AttributeSection>();
		AttributeSection section = null;
		VariableDeclaration varDecl = null;
		
		while (la.kind == 17) {
			AttributeSection(
#line  1352 "cs.ATG" 
out section);

#line  1352 "cs.ATG" 
			attributes.Add(section); 
		}
		Expect(1);

#line  1353 "cs.ATG" 
		f = new FieldDeclaration(attributes);
		varDecl         = new VariableDeclaration(t.val);
		f.Fields.Add(varDecl);
		f.StartLocation = t.Location;
		
		if (la.kind == 3) {
			lexer.NextToken();
			Expr(
#line  1358 "cs.ATG" 
out expr);

#line  1358 "cs.ATG" 
			varDecl.Initializer = expr; 
		}
	}

	void SimpleType(
#line  932 "cs.ATG" 
out string name) {

#line  933 "cs.ATG" 
		name = String.Empty; 
		if (StartOf(19)) {
			IntegralType(
#line  935 "cs.ATG" 
out name);
		} else if (la.kind == 73) {
			lexer.NextToken();

#line  936 "cs.ATG" 
			name = "float"; 
		} else if (la.kind == 64) {
			lexer.NextToken();

#line  937 "cs.ATG" 
			name = "double"; 
		} else if (la.kind == 60) {
			lexer.NextToken();

#line  938 "cs.ATG" 
			name = "decimal"; 
		} else if (la.kind == 50) {
			lexer.NextToken();

#line  939 "cs.ATG" 
			name = "bool"; 
		} else SynErr(153);
	}

	void NonArrayType(
#line  914 "cs.ATG" 
out TypeReference type) {

#line  916 "cs.ATG" 
		string name;
		int pointer = 0;
		type = null;
		
		if (la.kind == 1 || la.kind == 89 || la.kind == 106) {
			ClassType(
#line  921 "cs.ATG" 
out type);
		} else if (StartOf(14)) {
			SimpleType(
#line  922 "cs.ATG" 
out name);

#line  922 "cs.ATG" 
			type = new TypeReference(name); 
		} else if (la.kind == 121) {
			lexer.NextToken();
			Expect(6);

#line  923 "cs.ATG" 
			pointer = 1; type = new TypeReference("void"); 
		} else SynErr(154);
		while (
#line  926 "cs.ATG" 
IsPointer()) {
			Expect(6);

#line  927 "cs.ATG" 
			++pointer; 
		}

#line  929 "cs.ATG" 
		type.PointerNestingLevel = pointer; 
	}

	void FixedParameter(
#line  969 "cs.ATG" 
out ParameterDeclarationExpression p) {

#line  971 "cs.ATG" 
		TypeReference type;
		ParamModifier mod = ParamModifier.In;
		
		if (la.kind == 91 || la.kind == 98) {
			if (la.kind == 98) {
				lexer.NextToken();

#line  976 "cs.ATG" 
				mod = ParamModifier.Ref; 
			} else {
				lexer.NextToken();

#line  977 "cs.ATG" 
				mod = ParamModifier.Out; 
			}
		}
		Type(
#line  979 "cs.ATG" 
out type);
		Expect(1);

#line  979 "cs.ATG" 
		p = new ParameterDeclarationExpression(type, t.val, mod); 
	}

	void ParameterArray(
#line  982 "cs.ATG" 
out ParameterDeclarationExpression p) {

#line  983 "cs.ATG" 
		TypeReference type; 
		Expect(93);
		Type(
#line  985 "cs.ATG" 
out type);
		Expect(1);

#line  985 "cs.ATG" 
		p = new ParameterDeclarationExpression(type, t.val, ParamModifier.Params); 
	}

	void Block(
#line  1462 "cs.ATG" 
out Statement stmt) {
		Expect(15);

#line  1464 "cs.ATG" 
		BlockStatement blockStmt = new BlockStatement();
		blockStmt.StartLocation = t.EndLocation;
		compilationUnit.BlockStart(blockStmt);
		if (!parseMethodContents) lexer.SkipCurrentBlock();
		
		while (StartOf(20)) {
			Statement();
		}
		Expect(16);

#line  1471 "cs.ATG" 
		stmt = blockStmt;
		blockStmt.EndLocation = t.EndLocation;
		compilationUnit.BlockEnd();
		
	}

	void VariableDeclarator(
#line  1455 "cs.ATG" 
List<VariableDeclaration> fieldDeclaration) {

#line  1456 "cs.ATG" 
		Expression expr = null; 
		Expect(1);

#line  1458 "cs.ATG" 
		VariableDeclaration f = new VariableDeclaration(t.val); 
		if (la.kind == 3) {
			lexer.NextToken();
			VariableInitializer(
#line  1459 "cs.ATG" 
out expr);

#line  1459 "cs.ATG" 
			f.Initializer = expr; 
		}

#line  1459 "cs.ATG" 
		fieldDeclaration.Add(f); 
	}

	void EventAccessorDecls(
#line  1404 "cs.ATG" 
out EventAddRegion addBlock, out EventRemoveRegion removeBlock) {

#line  1405 "cs.ATG" 
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		Statement stmt;
		addBlock = null;
		removeBlock = null;
		
		while (la.kind == 17) {
			AttributeSection(
#line  1412 "cs.ATG" 
out section);

#line  1412 "cs.ATG" 
			attributes.Add(section); 
		}
		if (
#line  1414 "cs.ATG" 
IdentIsAdd()) {

#line  1414 "cs.ATG" 
			addBlock = new EventAddRegion(attributes); 
			AddAccessorDecl(
#line  1415 "cs.ATG" 
out stmt);

#line  1415 "cs.ATG" 
			attributes = new List<AttributeSection>(); addBlock.Block = (BlockStatement)stmt; 
			while (la.kind == 17) {
				AttributeSection(
#line  1416 "cs.ATG" 
out section);

#line  1416 "cs.ATG" 
				attributes.Add(section); 
			}
			RemoveAccessorDecl(
#line  1417 "cs.ATG" 
out stmt);

#line  1417 "cs.ATG" 
			removeBlock = new EventRemoveRegion(attributes); removeBlock.Block = (BlockStatement)stmt; 
		} else if (
#line  1418 "cs.ATG" 
IdentIsRemove()) {
			RemoveAccessorDecl(
#line  1419 "cs.ATG" 
out stmt);

#line  1419 "cs.ATG" 
			removeBlock = new EventRemoveRegion(attributes); removeBlock.Block = (BlockStatement)stmt; attributes = new List<AttributeSection>(); 
			while (la.kind == 17) {
				AttributeSection(
#line  1420 "cs.ATG" 
out section);

#line  1420 "cs.ATG" 
				attributes.Add(section); 
			}
			AddAccessorDecl(
#line  1421 "cs.ATG" 
out stmt);

#line  1421 "cs.ATG" 
			addBlock = new EventAddRegion(attributes); addBlock.Block = (BlockStatement)stmt; 
		} else if (la.kind == 1) {
			lexer.NextToken();

#line  1422 "cs.ATG" 
			Error("add or remove accessor declaration expected"); 
		} else SynErr(155);
	}

	void ConstructorInitializer(
#line  1493 "cs.ATG" 
out ConstructorInitializer ci) {

#line  1494 "cs.ATG" 
		Expression expr; ci = new ConstructorInitializer(); 
		Expect(9);
		if (la.kind == 49) {
			lexer.NextToken();

#line  1498 "cs.ATG" 
			ci.ConstructorInitializerType = ConstructorInitializerType.Base; 
		} else if (la.kind == 109) {
			lexer.NextToken();

#line  1499 "cs.ATG" 
			ci.ConstructorInitializerType = ConstructorInitializerType.This; 
		} else SynErr(156);
		Expect(19);
		if (StartOf(21)) {
			Argument(
#line  1502 "cs.ATG" 
out expr);

#line  1502 "cs.ATG" 
			if (expr != null) { ci.Arguments.Add(expr); } 
			while (la.kind == 13) {
				lexer.NextToken();
				Argument(
#line  1502 "cs.ATG" 
out expr);

#line  1502 "cs.ATG" 
				if (expr != null) { ci.Arguments.Add(expr); } 
			}
		}
		Expect(20);
	}

	void OverloadableOperator(
#line  1514 "cs.ATG" 
out OverloadableOperatorType op) {

#line  1515 "cs.ATG" 
		op = OverloadableOperatorType.None; 
		switch (la.kind) {
		case 4: {
			lexer.NextToken();

#line  1517 "cs.ATG" 
			op = OverloadableOperatorType.Add; 
			break;
		}
		case 5: {
			lexer.NextToken();

#line  1518 "cs.ATG" 
			op = OverloadableOperatorType.Subtract; 
			break;
		}
		case 23: {
			lexer.NextToken();

#line  1520 "cs.ATG" 
			op = OverloadableOperatorType.Not; 
			break;
		}
		case 26: {
			lexer.NextToken();

#line  1521 "cs.ATG" 
			op = OverloadableOperatorType.BitNot; 
			break;
		}
		case 30: {
			lexer.NextToken();

#line  1523 "cs.ATG" 
			op = OverloadableOperatorType.Increment; 
			break;
		}
		case 31: {
			lexer.NextToken();

#line  1524 "cs.ATG" 
			op = OverloadableOperatorType.Decrement; 
			break;
		}
		case 111: {
			lexer.NextToken();

#line  1526 "cs.ATG" 
			op = OverloadableOperatorType.True; 
			break;
		}
		case 70: {
			lexer.NextToken();

#line  1527 "cs.ATG" 
			op = OverloadableOperatorType.False; 
			break;
		}
		case 6: {
			lexer.NextToken();

#line  1529 "cs.ATG" 
			op = OverloadableOperatorType.Multiply; 
			break;
		}
		case 7: {
			lexer.NextToken();

#line  1530 "cs.ATG" 
			op = OverloadableOperatorType.Divide; 
			break;
		}
		case 8: {
			lexer.NextToken();

#line  1531 "cs.ATG" 
			op = OverloadableOperatorType.Modulus; 
			break;
		}
		case 27: {
			lexer.NextToken();

#line  1533 "cs.ATG" 
			op = OverloadableOperatorType.BitwiseAnd; 
			break;
		}
		case 28: {
			lexer.NextToken();

#line  1534 "cs.ATG" 
			op = OverloadableOperatorType.BitwiseOr; 
			break;
		}
		case 29: {
			lexer.NextToken();

#line  1535 "cs.ATG" 
			op = OverloadableOperatorType.ExclusiveOr; 
			break;
		}
		case 36: {
			lexer.NextToken();

#line  1537 "cs.ATG" 
			op = OverloadableOperatorType.ShiftLeft; 
			break;
		}
		case 32: {
			lexer.NextToken();

#line  1538 "cs.ATG" 
			op = OverloadableOperatorType.Equality; 
			break;
		}
		case 33: {
			lexer.NextToken();

#line  1539 "cs.ATG" 
			op = OverloadableOperatorType.InEquality; 
			break;
		}
		case 22: {
			lexer.NextToken();

#line  1540 "cs.ATG" 
			op = OverloadableOperatorType.LessThan; 
			break;
		}
		case 34: {
			lexer.NextToken();

#line  1541 "cs.ATG" 
			op = OverloadableOperatorType.GreaterThanOrEqual; 
			break;
		}
		case 35: {
			lexer.NextToken();

#line  1542 "cs.ATG" 
			op = OverloadableOperatorType.LessThanOrEqual; 
			break;
		}
		case 21: {
			lexer.NextToken();

#line  1543 "cs.ATG" 
			op = OverloadableOperatorType.GreaterThan; 
			if (la.kind == 21) {
				lexer.NextToken();

#line  1543 "cs.ATG" 
				op = OverloadableOperatorType.ShiftRight; 
			}
			break;
		}
		default: SynErr(157); break;
		}
	}

	void AccessorDecls(
#line  1362 "cs.ATG" 
out PropertyGetRegion getBlock, out PropertySetRegion setBlock) {

#line  1364 "cs.ATG" 
		List<AttributeSection> attributes = new List<AttributeSection>(); 
		AttributeSection section;
		getBlock = null;
		setBlock = null; 
		
		while (la.kind == 17) {
			AttributeSection(
#line  1370 "cs.ATG" 
out section);

#line  1370 "cs.ATG" 
			attributes.Add(section); 
		}
		if (
#line  1372 "cs.ATG" 
IdentIsGet()) {
			GetAccessorDecl(
#line  1373 "cs.ATG" 
out getBlock, attributes);
			if (la.kind == 1 || la.kind == 17) {

#line  1374 "cs.ATG" 
				attributes = new List<AttributeSection>(); 
				while (la.kind == 17) {
					AttributeSection(
#line  1375 "cs.ATG" 
out section);

#line  1375 "cs.ATG" 
					attributes.Add(section); 
				}
				SetAccessorDecl(
#line  1376 "cs.ATG" 
out setBlock, attributes);
			}
		} else if (
#line  1378 "cs.ATG" 
IdentIsSet()) {
			SetAccessorDecl(
#line  1379 "cs.ATG" 
out setBlock, attributes);
			if (la.kind == 1 || la.kind == 17) {

#line  1380 "cs.ATG" 
				attributes = new List<AttributeSection>(); 
				while (la.kind == 17) {
					AttributeSection(
#line  1381 "cs.ATG" 
out section);

#line  1381 "cs.ATG" 
					attributes.Add(section); 
				}
				GetAccessorDecl(
#line  1382 "cs.ATG" 
out getBlock, attributes);
			}
		} else if (la.kind == 1) {
			lexer.NextToken();

#line  1384 "cs.ATG" 
			Error("get or set accessor declaration expected"); 
		} else SynErr(158);
	}

	void InterfaceAccessors(
#line  1426 "cs.ATG" 
out PropertyGetRegion getBlock, out PropertySetRegion setBlock) {

#line  1428 "cs.ATG" 
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		getBlock = null; setBlock = null;
		
		while (la.kind == 17) {
			AttributeSection(
#line  1433 "cs.ATG" 
out section);

#line  1433 "cs.ATG" 
			attributes.Add(section); 
		}
		if (
#line  1435 "cs.ATG" 
IdentIsGet()) {
			Expect(1);

#line  1435 "cs.ATG" 
			getBlock = new PropertyGetRegion(null, attributes); 
		} else if (
#line  1436 "cs.ATG" 
IdentIsSet()) {
			Expect(1);

#line  1436 "cs.ATG" 
			setBlock = new PropertySetRegion(null, attributes); 
		} else if (la.kind == 1) {
			lexer.NextToken();

#line  1437 "cs.ATG" 
			Error("set or get expected"); 
		} else SynErr(159);
		Expect(11);

#line  1439 "cs.ATG" 
		attributes = new List<AttributeSection>(); 
		if (la.kind == 1 || la.kind == 17) {
			while (la.kind == 17) {
				AttributeSection(
#line  1441 "cs.ATG" 
out section);

#line  1441 "cs.ATG" 
				attributes.Add(section); 
			}
			if (
#line  1443 "cs.ATG" 
IdentIsGet()) {
				Expect(1);

#line  1443 "cs.ATG" 
				if (getBlock != null) Error("get already declared");
				else getBlock = new PropertyGetRegion(null, attributes);
				
			} else if (
#line  1446 "cs.ATG" 
IdentIsSet()) {
				Expect(1);

#line  1446 "cs.ATG" 
				if (setBlock != null) Error("set already declared");
				else setBlock = new PropertySetRegion(null, attributes);
				
			} else if (la.kind == 1) {
				lexer.NextToken();

#line  1449 "cs.ATG" 
				Error("set or get expected"); 
			} else SynErr(160);
			Expect(11);
		}
	}

	void GetAccessorDecl(
#line  1388 "cs.ATG" 
out PropertyGetRegion getBlock, List<AttributeSection> attributes) {

#line  1389 "cs.ATG" 
		Statement stmt = null; 
		Expect(1);

#line  1392 "cs.ATG" 
		if (t.val != "get") Error("get expected"); 
		if (la.kind == 15) {
			Block(
#line  1393 "cs.ATG" 
out stmt);
		} else if (la.kind == 11) {
			lexer.NextToken();
		} else SynErr(161);

#line  1393 "cs.ATG" 
		getBlock = new PropertyGetRegion((BlockStatement)stmt, attributes); 
	}

	void SetAccessorDecl(
#line  1396 "cs.ATG" 
out PropertySetRegion setBlock, List<AttributeSection> attributes) {

#line  1397 "cs.ATG" 
		Statement stmt = null; 
		Expect(1);

#line  1400 "cs.ATG" 
		if (t.val != "set") Error("set expected"); 
		if (la.kind == 15) {
			Block(
#line  1401 "cs.ATG" 
out stmt);
		} else if (la.kind == 11) {
			lexer.NextToken();
		} else SynErr(162);

#line  1401 "cs.ATG" 
		setBlock = new PropertySetRegion((BlockStatement)stmt, attributes); 
	}

	void AddAccessorDecl(
#line  1477 "cs.ATG" 
out Statement stmt) {

#line  1478 "cs.ATG" 
		stmt = null;
		Expect(1);

#line  1481 "cs.ATG" 
		if (t.val != "add") Error("add expected"); 
		Block(
#line  1482 "cs.ATG" 
out stmt);
	}

	void RemoveAccessorDecl(
#line  1485 "cs.ATG" 
out Statement stmt) {

#line  1486 "cs.ATG" 
		stmt = null;
		Expect(1);

#line  1489 "cs.ATG" 
		if (t.val != "remove") Error("remove expected"); 
		Block(
#line  1490 "cs.ATG" 
out stmt);
	}

	void VariableInitializer(
#line  1506 "cs.ATG" 
out Expression initializerExpression) {

#line  1507 "cs.ATG" 
		TypeReference type = null; Expression expr = null; initializerExpression = null; 
		if (StartOf(4)) {
			Expr(
#line  1509 "cs.ATG" 
out initializerExpression);
		} else if (la.kind == 15) {
			ArrayInitializer(
#line  1510 "cs.ATG" 
out initializerExpression);
		} else if (la.kind == 104) {
			lexer.NextToken();
			Type(
#line  1511 "cs.ATG" 
out type);
			Expect(17);
			Expr(
#line  1511 "cs.ATG" 
out expr);
			Expect(18);

#line  1511 "cs.ATG" 
			initializerExpression = new StackAllocExpression(type, expr); 
		} else SynErr(163);
	}

	void Statement() {

#line  1615 "cs.ATG" 
		TypeReference type;
		Expression expr;
		Statement stmt = null;
		Point startPos = la.Location;
		
		if (
#line  1623 "cs.ATG" 
IsLabel()) {
			Expect(1);

#line  1623 "cs.ATG" 
			compilationUnit.AddChild(new LabelStatement(t.val)); 
			Expect(9);
			Statement();
		} else if (la.kind == 58) {
			lexer.NextToken();
			Type(
#line  1626 "cs.ATG" 
out type);

#line  1626 "cs.ATG" 
			LocalVariableDeclaration var = new LocalVariableDeclaration(type, Modifier.Const); string ident = null; var.StartLocation = t.Location; 
			Expect(1);

#line  1627 "cs.ATG" 
			ident = t.val; 
			Expect(3);
			Expr(
#line  1628 "cs.ATG" 
out expr);

#line  1628 "cs.ATG" 
			var.Variables.Add(new VariableDeclaration(ident, expr)); 
			while (la.kind == 13) {
				lexer.NextToken();
				Expect(1);

#line  1629 "cs.ATG" 
				ident = t.val; 
				Expect(3);
				Expr(
#line  1629 "cs.ATG" 
out expr);

#line  1629 "cs.ATG" 
				var.Variables.Add(new VariableDeclaration(ident, expr)); 
			}
			Expect(11);

#line  1630 "cs.ATG" 
			compilationUnit.AddChild(var); 
		} else if (
#line  1632 "cs.ATG" 
IsLocalVarDecl()) {
			LocalVariableDecl(
#line  1632 "cs.ATG" 
out stmt);
			Expect(11);

#line  1632 "cs.ATG" 
			compilationUnit.AddChild(stmt); 
		} else if (StartOf(22)) {
			EmbeddedStatement(
#line  1633 "cs.ATG" 
out stmt);

#line  1633 "cs.ATG" 
			compilationUnit.AddChild(stmt); 
		} else SynErr(164);

#line  1639 "cs.ATG" 
		if (stmt != null) {
		stmt.StartLocation = startPos;
		stmt.EndLocation = t.EndLocation;
		}
		
	}

	void Argument(
#line  1546 "cs.ATG" 
out Expression argumentexpr) {

#line  1548 "cs.ATG" 
		Expression expr;
		FieldDirection fd = FieldDirection.None;
		
		if (la.kind == 91 || la.kind == 98) {
			if (la.kind == 98) {
				lexer.NextToken();

#line  1553 "cs.ATG" 
				fd = FieldDirection.Ref; 
			} else {
				lexer.NextToken();

#line  1554 "cs.ATG" 
				fd = FieldDirection.Out; 
			}
		}
		Expr(
#line  1556 "cs.ATG" 
out expr);

#line  1556 "cs.ATG" 
		argumentexpr = fd != FieldDirection.None ? argumentexpr = new DirectionExpression(fd, expr) : expr; 
	}

	void ArrayInitializer(
#line  1575 "cs.ATG" 
out Expression outExpr) {

#line  1577 "cs.ATG" 
		Expression expr = null;
		ArrayInitializerExpression initializer = new ArrayInitializerExpression();
		
		Expect(15);
		if (StartOf(23)) {
			VariableInitializer(
#line  1582 "cs.ATG" 
out expr);

#line  1582 "cs.ATG" 
			if (expr != null) { initializer.CreateExpressions.Add(expr); } 
			while (
#line  1582 "cs.ATG" 
NotFinalComma()) {
				Expect(13);
				VariableInitializer(
#line  1582 "cs.ATG" 
out expr);

#line  1582 "cs.ATG" 
				if (expr != null) { initializer.CreateExpressions.Add(expr); } 
			}
			if (la.kind == 13) {
				lexer.NextToken();
			}
		}
		Expect(16);

#line  1583 "cs.ATG" 
		outExpr = initializer; 
	}

	void AssignmentOperator(
#line  1559 "cs.ATG" 
out AssignmentOperatorType op) {

#line  1560 "cs.ATG" 
		op = AssignmentOperatorType.None; 
		switch (la.kind) {
		case 3: {
			lexer.NextToken();

#line  1562 "cs.ATG" 
			op = AssignmentOperatorType.Assign; 
			break;
		}
		case 37: {
			lexer.NextToken();

#line  1563 "cs.ATG" 
			op = AssignmentOperatorType.Add; 
			break;
		}
		case 38: {
			lexer.NextToken();

#line  1564 "cs.ATG" 
			op = AssignmentOperatorType.Subtract; 
			break;
		}
		case 39: {
			lexer.NextToken();

#line  1565 "cs.ATG" 
			op = AssignmentOperatorType.Multiply; 
			break;
		}
		case 40: {
			lexer.NextToken();

#line  1566 "cs.ATG" 
			op = AssignmentOperatorType.Divide; 
			break;
		}
		case 41: {
			lexer.NextToken();

#line  1567 "cs.ATG" 
			op = AssignmentOperatorType.Modulus; 
			break;
		}
		case 42: {
			lexer.NextToken();

#line  1568 "cs.ATG" 
			op = AssignmentOperatorType.BitwiseAnd; 
			break;
		}
		case 43: {
			lexer.NextToken();

#line  1569 "cs.ATG" 
			op = AssignmentOperatorType.BitwiseOr; 
			break;
		}
		case 44: {
			lexer.NextToken();

#line  1570 "cs.ATG" 
			op = AssignmentOperatorType.ExclusiveOr; 
			break;
		}
		case 45: {
			lexer.NextToken();

#line  1571 "cs.ATG" 
			op = AssignmentOperatorType.ShiftLeft; 
			break;
		}
		case 21: {
			lexer.NextToken();
			Expect(34);

#line  1572 "cs.ATG" 
			op = AssignmentOperatorType.ShiftRight; 
			break;
		}
		default: SynErr(165); break;
		}
	}

	void LocalVariableDecl(
#line  1586 "cs.ATG" 
out Statement stmt) {

#line  1588 "cs.ATG" 
		TypeReference type;
		VariableDeclaration      var = null;
		LocalVariableDeclaration localVariableDeclaration; 
		
		Type(
#line  1593 "cs.ATG" 
out type);

#line  1593 "cs.ATG" 
		localVariableDeclaration = new LocalVariableDeclaration(type); localVariableDeclaration.StartLocation = t.Location; 
		LocalVariableDeclarator(
#line  1594 "cs.ATG" 
out var);

#line  1594 "cs.ATG" 
		localVariableDeclaration.Variables.Add(var); 
		while (la.kind == 13) {
			lexer.NextToken();
			LocalVariableDeclarator(
#line  1595 "cs.ATG" 
out var);

#line  1595 "cs.ATG" 
			localVariableDeclaration.Variables.Add(var); 
		}

#line  1596 "cs.ATG" 
		stmt = localVariableDeclaration; 
	}

	void LocalVariableDeclarator(
#line  1599 "cs.ATG" 
out VariableDeclaration var) {

#line  1600 "cs.ATG" 
		Expression expr = null; 
		Expect(1);

#line  1603 "cs.ATG" 
		var = new VariableDeclaration(t.val); 
		if (la.kind == 3) {
			lexer.NextToken();
			VariableInitializer(
#line  1603 "cs.ATG" 
out expr);

#line  1603 "cs.ATG" 
			var.Initializer = expr; 
		}
	}

	void EmbeddedStatement(
#line  1646 "cs.ATG" 
out Statement statement) {

#line  1648 "cs.ATG" 
		TypeReference type = null;
		Expression expr = null;
		Statement embeddedStatement = null;
		statement = null;
		
		if (la.kind == 15) {
			Block(
#line  1654 "cs.ATG" 
out statement);
		} else if (la.kind == 11) {
			lexer.NextToken();

#line  1656 "cs.ATG" 
			statement = new EmptyStatement(); 
		} else if (
#line  1658 "cs.ATG" 
UnCheckedAndLBrace()) {

#line  1658 "cs.ATG" 
			Statement block; bool isChecked = true; 
			if (la.kind == 56) {
				lexer.NextToken();
			} else if (la.kind == 116) {
				lexer.NextToken();

#line  1659 "cs.ATG" 
				isChecked = false;
			} else SynErr(166);
			Block(
#line  1660 "cs.ATG" 
out block);

#line  1660 "cs.ATG" 
			statement = isChecked ? (Statement)new CheckedStatement(block) : (Statement)new UncheckedStatement(block); 
		} else if (la.kind == 77) {
			lexer.NextToken();

#line  1662 "cs.ATG" 
			Statement elseStatement = null; 
			Expect(19);
			Expr(
#line  1663 "cs.ATG" 
out expr);
			Expect(20);
			EmbeddedStatement(
#line  1664 "cs.ATG" 
out embeddedStatement);
			if (la.kind == 65) {
				lexer.NextToken();
				EmbeddedStatement(
#line  1665 "cs.ATG" 
out elseStatement);
			}

#line  1666 "cs.ATG" 
			statement = elseStatement != null ? (Statement)new IfElseStatement(expr, embeddedStatement, elseStatement) :  (Statement)new IfElseStatement(expr, embeddedStatement); 
		} else if (la.kind == 108) {
			lexer.NextToken();

#line  1667 "cs.ATG" 
			ArrayList switchSections = new ArrayList(); SwitchSection switchSection; 
			Expect(19);
			Expr(
#line  1668 "cs.ATG" 
out expr);
			Expect(20);
			Expect(15);
			while (la.kind == 53 || la.kind == 61) {
				SwitchSection(
#line  1669 "cs.ATG" 
out switchSection);

#line  1669 "cs.ATG" 
				switchSections.Add(switchSection); 
			}
			Expect(16);

#line  1670 "cs.ATG" 
			statement = new SwitchStatement(expr, switchSections); 
		} else if (la.kind == 123) {
			lexer.NextToken();
			Expect(19);
			Expr(
#line  1672 "cs.ATG" 
out expr);
			Expect(20);
			EmbeddedStatement(
#line  1674 "cs.ATG" 
out embeddedStatement);

#line  1674 "cs.ATG" 
			statement = new DoLoopStatement(expr, embeddedStatement, ConditionType.While, ConditionPosition.Start);
		} else if (la.kind == 63) {
			lexer.NextToken();
			EmbeddedStatement(
#line  1675 "cs.ATG" 
out embeddedStatement);
			Expect(123);
			Expect(19);
			Expr(
#line  1676 "cs.ATG" 
out expr);
			Expect(20);
			Expect(11);

#line  1676 "cs.ATG" 
			statement = new DoLoopStatement(expr, embeddedStatement, ConditionType.While, ConditionPosition.End); 
		} else if (la.kind == 74) {
			lexer.NextToken();

#line  1677 "cs.ATG" 
			ArrayList initializer = null; ArrayList iterator = null; 
			Expect(19);
			if (StartOf(4)) {
				ForInitializer(
#line  1678 "cs.ATG" 
out initializer);
			}
			Expect(11);
			if (StartOf(4)) {
				Expr(
#line  1679 "cs.ATG" 
out expr);
			}
			Expect(11);
			if (StartOf(4)) {
				ForIterator(
#line  1680 "cs.ATG" 
out iterator);
			}
			Expect(20);
			EmbeddedStatement(
#line  1681 "cs.ATG" 
out embeddedStatement);

#line  1681 "cs.ATG" 
			statement = new ForStatement(initializer, expr, iterator, embeddedStatement); 
		} else if (la.kind == 75) {
			lexer.NextToken();
			Expect(19);
			Type(
#line  1682 "cs.ATG" 
out type);
			Expect(1);

#line  1682 "cs.ATG" 
			string varName = t.val; Point start = t.Location;
			Expect(79);
			Expr(
#line  1683 "cs.ATG" 
out expr);
			Expect(20);
			EmbeddedStatement(
#line  1684 "cs.ATG" 
out embeddedStatement);

#line  1684 "cs.ATG" 
			statement = new ForeachStatement(type, varName , expr, embeddedStatement); 
			statement.EndLocation = t.EndLocation;
			
		} else if (la.kind == 51) {
			lexer.NextToken();
			Expect(11);

#line  1688 "cs.ATG" 
			statement = new BreakStatement(); 
		} else if (la.kind == 59) {
			lexer.NextToken();
			Expect(11);

#line  1689 "cs.ATG" 
			statement = new ContinueStatement(); 
		} else if (la.kind == 76) {
			GotoStatement(
#line  1690 "cs.ATG" 
out statement);
		} else if (
#line  1691 "cs.ATG" 
IsYieldStatement()) {
			Expect(1);
			if (la.kind == 99) {
				lexer.NextToken();
				Expr(
#line  1691 "cs.ATG" 
out expr);

#line  1691 "cs.ATG" 
				statement = new YieldStatement(new ReturnStatement(expr)); 
			} else if (la.kind == 51) {
				lexer.NextToken();

#line  1692 "cs.ATG" 
				statement = new YieldStatement(new BreakStatement()); 
			} else SynErr(167);
			Expect(11);
		} else if (la.kind == 99) {
			lexer.NextToken();
			if (StartOf(4)) {
				Expr(
#line  1693 "cs.ATG" 
out expr);
			}
			Expect(11);

#line  1693 "cs.ATG" 
			statement = new ReturnStatement(expr); 
		} else if (la.kind == 110) {
			lexer.NextToken();
			if (StartOf(4)) {
				Expr(
#line  1694 "cs.ATG" 
out expr);
			}
			Expect(11);

#line  1694 "cs.ATG" 
			statement = new ThrowStatement(expr); 
		} else if (StartOf(4)) {
			StatementExpr(
#line  1696 "cs.ATG" 
out statement);
			Expect(11);
		} else if (la.kind == 112) {
			TryStatement(
#line  1698 "cs.ATG" 
out statement);
		} else if (la.kind == 84) {
			lexer.NextToken();
			Expect(19);
			Expr(
#line  1700 "cs.ATG" 
out expr);
			Expect(20);
			EmbeddedStatement(
#line  1701 "cs.ATG" 
out embeddedStatement);

#line  1701 "cs.ATG" 
			statement = new LockStatement(expr, embeddedStatement); 
		} else if (la.kind == 119) {

#line  1703 "cs.ATG" 
			Statement resourceAcquisitionStmt = null; 
			lexer.NextToken();
			Expect(19);
			ResourceAcquisition(
#line  1705 "cs.ATG" 
out resourceAcquisitionStmt);
			Expect(20);
			EmbeddedStatement(
#line  1706 "cs.ATG" 
out embeddedStatement);

#line  1706 "cs.ATG" 
			statement = new UsingStatement(resourceAcquisitionStmt, embeddedStatement); 
		} else if (la.kind == 117) {
			lexer.NextToken();
			Block(
#line  1708 "cs.ATG" 
out embeddedStatement);

#line  1708 "cs.ATG" 
			statement = new UnsafeStatement(embeddedStatement); 
		} else if (la.kind == 72) {
			lexer.NextToken();
			Expect(19);
			Type(
#line  1711 "cs.ATG" 
out type);

#line  1711 "cs.ATG" 
			if (type.PointerNestingLevel == 0) Error("can only fix pointer types");
			ArrayList pointerDeclarators = new ArrayList(1);
			
			Expect(1);

#line  1714 "cs.ATG" 
			string identifier = t.val; 
			Expect(3);
			Expr(
#line  1715 "cs.ATG" 
out expr);

#line  1715 "cs.ATG" 
			pointerDeclarators.Add(new VariableDeclaration(identifier, expr)); 
			while (la.kind == 13) {
				lexer.NextToken();
				Expect(1);

#line  1717 "cs.ATG" 
				identifier = t.val; 
				Expect(3);
				Expr(
#line  1718 "cs.ATG" 
out expr);

#line  1718 "cs.ATG" 
				pointerDeclarators.Add(new VariableDeclaration(identifier, expr)); 
			}
			Expect(20);
			EmbeddedStatement(
#line  1720 "cs.ATG" 
out embeddedStatement);

#line  1720 "cs.ATG" 
			statement = new FixedStatement(type, pointerDeclarators, embeddedStatement); 
		} else SynErr(168);
	}

	void SwitchSection(
#line  1742 "cs.ATG" 
out SwitchSection stmt) {

#line  1744 "cs.ATG" 
		SwitchSection switchSection = new SwitchSection();
		CaseLabel label;
		
		SwitchLabel(
#line  1748 "cs.ATG" 
out label);

#line  1748 "cs.ATG" 
		switchSection.SwitchLabels.Add(label); 
		while (la.kind == 53 || la.kind == 61) {
			SwitchLabel(
#line  1750 "cs.ATG" 
out label);

#line  1750 "cs.ATG" 
			switchSection.SwitchLabels.Add(label); 
		}

#line  1752 "cs.ATG" 
		compilationUnit.BlockStart(switchSection); 
		Statement();
		while (StartOf(20)) {
			Statement();
		}

#line  1755 "cs.ATG" 
		compilationUnit.BlockEnd();
		stmt = switchSection;
		
	}

	void ForInitializer(
#line  1723 "cs.ATG" 
out ArrayList initializer) {

#line  1725 "cs.ATG" 
		Statement stmt; 
		initializer = new ArrayList();
		
		if (
#line  1729 "cs.ATG" 
IsLocalVarDecl()) {
			LocalVariableDecl(
#line  1729 "cs.ATG" 
out stmt);

#line  1729 "cs.ATG" 
			initializer.Add(stmt);
		} else if (StartOf(4)) {
			StatementExpr(
#line  1730 "cs.ATG" 
out stmt);

#line  1730 "cs.ATG" 
			initializer.Add(stmt);
			while (la.kind == 13) {
				lexer.NextToken();
				StatementExpr(
#line  1730 "cs.ATG" 
out stmt);

#line  1730 "cs.ATG" 
				initializer.Add(stmt);
			}

#line  1730 "cs.ATG" 
			initializer.Add(stmt);
		} else SynErr(169);
	}

	void ForIterator(
#line  1733 "cs.ATG" 
out ArrayList iterator) {

#line  1735 "cs.ATG" 
		Statement stmt; 
		iterator = new ArrayList();
		
		StatementExpr(
#line  1739 "cs.ATG" 
out stmt);

#line  1739 "cs.ATG" 
		iterator.Add(stmt);
		while (la.kind == 13) {
			lexer.NextToken();
			StatementExpr(
#line  1739 "cs.ATG" 
out stmt);

#line  1739 "cs.ATG" 
			iterator.Add(stmt); 
		}
	}

	void GotoStatement(
#line  1810 "cs.ATG" 
out Statement stmt) {

#line  1811 "cs.ATG" 
		Expression expr; stmt = null; 
		Expect(76);
		if (la.kind == 1) {
			lexer.NextToken();

#line  1815 "cs.ATG" 
			stmt = new GotoStatement(t.val); 
			Expect(11);
		} else if (la.kind == 53) {
			lexer.NextToken();
			Expr(
#line  1816 "cs.ATG" 
out expr);
			Expect(11);

#line  1816 "cs.ATG" 
			stmt = new GotoCaseStatement(expr); 
		} else if (la.kind == 61) {
			lexer.NextToken();
			Expect(11);

#line  1817 "cs.ATG" 
			stmt = new GotoCaseStatement(null); 
		} else SynErr(170);
	}

	void StatementExpr(
#line  1837 "cs.ATG" 
out Statement stmt) {

#line  1842 "cs.ATG" 
		bool mustBeAssignment = la.kind == Tokens.Plus  || la.kind == Tokens.Minus ||
		                       la.kind == Tokens.Not   || la.kind == Tokens.BitwiseComplement ||
		                       la.kind == Tokens.Times || la.kind == Tokens.BitwiseAnd   || IsTypeCast();
		Expression expr = null;
		
		UnaryExpr(
#line  1848 "cs.ATG" 
out expr);
		if (StartOf(6)) {

#line  1851 "cs.ATG" 
			AssignmentOperatorType op; Expression val; 
			AssignmentOperator(
#line  1851 "cs.ATG" 
out op);
			Expr(
#line  1851 "cs.ATG" 
out val);

#line  1851 "cs.ATG" 
			expr = new AssignmentExpression(expr, op, val); 
		} else if (la.kind == 11 || la.kind == 13 || la.kind == 20) {

#line  1852 "cs.ATG" 
			if (mustBeAssignment) Error("error in assignment."); 
		} else SynErr(171);

#line  1853 "cs.ATG" 
		stmt = new StatementExpression(expr); 
	}

	void TryStatement(
#line  1767 "cs.ATG" 
out Statement tryStatement) {

#line  1769 "cs.ATG" 
		Statement blockStmt = null, finallyStmt = null;
		ArrayList catchClauses = null;
		
		Expect(112);
		Block(
#line  1773 "cs.ATG" 
out blockStmt);
		if (la.kind == 54) {
			CatchClauses(
#line  1775 "cs.ATG" 
out catchClauses);
			if (la.kind == 71) {
				lexer.NextToken();
				Block(
#line  1775 "cs.ATG" 
out finallyStmt);
			}
		} else if (la.kind == 71) {
			lexer.NextToken();
			Block(
#line  1776 "cs.ATG" 
out finallyStmt);
		} else SynErr(172);

#line  1779 "cs.ATG" 
		tryStatement = new TryCatchStatement(blockStmt, catchClauses, finallyStmt);
			
	}

	void ResourceAcquisition(
#line  1821 "cs.ATG" 
out Statement stmt) {

#line  1823 "cs.ATG" 
		stmt = null;
		Expression expr;
		
		if (
#line  1828 "cs.ATG" 
IsLocalVarDecl()) {
			LocalVariableDecl(
#line  1828 "cs.ATG" 
out stmt);
		} else if (StartOf(4)) {
			Expr(
#line  1829 "cs.ATG" 
out expr);

#line  1833 "cs.ATG" 
			stmt = new StatementExpression(expr); 
		} else SynErr(173);
	}

	void SwitchLabel(
#line  1760 "cs.ATG" 
out CaseLabel label) {

#line  1761 "cs.ATG" 
		Expression expr = null; label = null; 
		if (la.kind == 53) {
			lexer.NextToken();
			Expr(
#line  1763 "cs.ATG" 
out expr);
			Expect(9);

#line  1763 "cs.ATG" 
			label =  new CaseLabel(expr); 
		} else if (la.kind == 61) {
			lexer.NextToken();
			Expect(9);

#line  1764 "cs.ATG" 
			label =  new CaseLabel(); 
		} else SynErr(174);
	}

	void CatchClauses(
#line  1784 "cs.ATG" 
out ArrayList catchClauses) {

#line  1786 "cs.ATG" 
		catchClauses = new ArrayList();
		
		Expect(54);

#line  1789 "cs.ATG" 
		string identifier;
		Statement stmt;
		TypeReference typeRef;
		
		if (la.kind == 15) {
			Block(
#line  1795 "cs.ATG" 
out stmt);

#line  1795 "cs.ATG" 
			catchClauses.Add(new CatchClause(stmt)); 
		} else if (la.kind == 19) {
			lexer.NextToken();
			ClassType(
#line  1797 "cs.ATG" 
out typeRef);

#line  1797 "cs.ATG" 
			identifier = null; 
			if (la.kind == 1) {
				lexer.NextToken();

#line  1798 "cs.ATG" 
				identifier = t.val; 
			}
			Expect(20);
			Block(
#line  1799 "cs.ATG" 
out stmt);

#line  1800 "cs.ATG" 
			catchClauses.Add(new CatchClause(typeRef, identifier, stmt)); 
			while (
#line  1801 "cs.ATG" 
IsTypedCatch()) {
				Expect(54);
				Expect(19);
				ClassType(
#line  1801 "cs.ATG" 
out typeRef);

#line  1801 "cs.ATG" 
				identifier = null; 
				if (la.kind == 1) {
					lexer.NextToken();

#line  1802 "cs.ATG" 
					identifier = t.val; 
				}
				Expect(20);
				Block(
#line  1803 "cs.ATG" 
out stmt);

#line  1804 "cs.ATG" 
				catchClauses.Add(new CatchClause(typeRef, identifier, stmt)); 
			}
			if (la.kind == 54) {
				lexer.NextToken();
				Block(
#line  1806 "cs.ATG" 
out stmt);

#line  1806 "cs.ATG" 
				catchClauses.Add(new CatchClause(stmt)); 
			}
		} else SynErr(175);
	}

	void UnaryExpr(
#line  1869 "cs.ATG" 
out Expression uExpr) {

#line  1871 "cs.ATG" 
		TypeReference type = null;
		Expression expr;
		ArrayList  expressions = new ArrayList();
		uExpr = null;
		
		while (StartOf(24) || 
#line  1895 "cs.ATG" 
IsTypeCast()) {
			if (la.kind == 4) {
				lexer.NextToken();

#line  1880 "cs.ATG" 
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.Plus)); 
			} else if (la.kind == 5) {
				lexer.NextToken();

#line  1881 "cs.ATG" 
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.Minus)); 
			} else if (la.kind == 23) {
				lexer.NextToken();

#line  1882 "cs.ATG" 
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.Not)); 
			} else if (la.kind == 26) {
				lexer.NextToken();

#line  1883 "cs.ATG" 
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.BitNot)); 
			} else if (la.kind == 6) {
				lexer.NextToken();

#line  1884 "cs.ATG" 
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.Star)); 
			} else if (la.kind == 30) {
				lexer.NextToken();

#line  1885 "cs.ATG" 
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.Increment)); 
			} else if (la.kind == 31) {
				lexer.NextToken();

#line  1886 "cs.ATG" 
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.Decrement)); 
			} else if (la.kind == 27) {
				lexer.NextToken();

#line  1887 "cs.ATG" 
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.BitWiseAnd)); 
			} else {
				Expect(19);
				Type(
#line  1895 "cs.ATG" 
out type);
				Expect(20);

#line  1895 "cs.ATG" 
				expressions.Add(new CastExpression(type)); 
			}
		}
		PrimaryExpr(
#line  1899 "cs.ATG" 
out expr);

#line  1899 "cs.ATG" 
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
#line  2082 "cs.ATG" 
ref Expression outExpr) {

#line  2083 "cs.ATG" 
		Expression expr;   
		ConditionalAndExpr(
#line  2085 "cs.ATG" 
ref outExpr);
		while (la.kind == 25) {
			lexer.NextToken();
			UnaryExpr(
#line  2085 "cs.ATG" 
out expr);
			ConditionalAndExpr(
#line  2085 "cs.ATG" 
ref expr);

#line  2085 "cs.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.LogicalOr, expr);  
		}
	}

	void PrimaryExpr(
#line  1916 "cs.ATG" 
out Expression pexpr) {

#line  1918 "cs.ATG" 
		TypeReference type = null;
		List<TypeReference> typeList = null;
		bool isArrayCreation = false;
		Expression expr;
		pexpr = null;
		
		if (la.kind == 111) {
			lexer.NextToken();

#line  1926 "cs.ATG" 
			pexpr = new PrimitiveExpression(true, "true");  
		} else if (la.kind == 70) {
			lexer.NextToken();

#line  1927 "cs.ATG" 
			pexpr = new PrimitiveExpression(false, "false"); 
		} else if (la.kind == 88) {
			lexer.NextToken();

#line  1928 "cs.ATG" 
			pexpr = new PrimitiveExpression(null, "null");  
		} else if (la.kind == 2) {
			lexer.NextToken();

#line  1929 "cs.ATG" 
			pexpr = new PrimitiveExpression(t.literalValue, t.val);  
		} else if (
#line  1930 "cs.ATG" 
la.kind == Tokens.Identifier && Peek(1).kind == Tokens.DoubleColon) {
			TypeName(
#line  1931 "cs.ATG" 
out type);

#line  1931 "cs.ATG" 
			pexpr = new TypeReferenceExpression(type); 
		} else if (la.kind == 1) {
			lexer.NextToken();

#line  1933 "cs.ATG" 
			pexpr = new IdentifierExpression(t.val); 
		} else if (la.kind == 19) {
			lexer.NextToken();
			Expr(
#line  1935 "cs.ATG" 
out expr);
			Expect(20);

#line  1935 "cs.ATG" 
			pexpr = new ParenthesizedExpression(expr); 
		} else if (StartOf(25)) {

#line  1937 "cs.ATG" 
			string val = null; 
			switch (la.kind) {
			case 50: {
				lexer.NextToken();

#line  1939 "cs.ATG" 
				val = "bool"; 
				break;
			}
			case 52: {
				lexer.NextToken();

#line  1940 "cs.ATG" 
				val = "byte"; 
				break;
			}
			case 55: {
				lexer.NextToken();

#line  1941 "cs.ATG" 
				val = "char"; 
				break;
			}
			case 60: {
				lexer.NextToken();

#line  1942 "cs.ATG" 
				val = "decimal"; 
				break;
			}
			case 64: {
				lexer.NextToken();

#line  1943 "cs.ATG" 
				val = "double"; 
				break;
			}
			case 73: {
				lexer.NextToken();

#line  1944 "cs.ATG" 
				val = "float"; 
				break;
			}
			case 80: {
				lexer.NextToken();

#line  1945 "cs.ATG" 
				val = "int"; 
				break;
			}
			case 85: {
				lexer.NextToken();

#line  1946 "cs.ATG" 
				val = "long"; 
				break;
			}
			case 89: {
				lexer.NextToken();

#line  1947 "cs.ATG" 
				val = "object"; 
				break;
			}
			case 100: {
				lexer.NextToken();

#line  1948 "cs.ATG" 
				val = "sbyte"; 
				break;
			}
			case 102: {
				lexer.NextToken();

#line  1949 "cs.ATG" 
				val = "short"; 
				break;
			}
			case 106: {
				lexer.NextToken();

#line  1950 "cs.ATG" 
				val = "string"; 
				break;
			}
			case 114: {
				lexer.NextToken();

#line  1951 "cs.ATG" 
				val = "uint"; 
				break;
			}
			case 115: {
				lexer.NextToken();

#line  1952 "cs.ATG" 
				val = "ulong"; 
				break;
			}
			case 118: {
				lexer.NextToken();

#line  1953 "cs.ATG" 
				val = "ushort"; 
				break;
			}
			}

#line  1954 "cs.ATG" 
			t.val = ""; 
			Expect(14);
			Expect(1);

#line  1954 "cs.ATG" 
			pexpr = new FieldReferenceExpression(new TypeReferenceExpression(val), t.val); 
		} else if (la.kind == 109) {
			lexer.NextToken();

#line  1956 "cs.ATG" 
			pexpr = new ThisReferenceExpression(); 
		} else if (la.kind == 49) {
			lexer.NextToken();

#line  1958 "cs.ATG" 
			Expression retExpr = new BaseReferenceExpression(); 
			if (la.kind == 14) {
				lexer.NextToken();
				Expect(1);

#line  1960 "cs.ATG" 
				retExpr = new FieldReferenceExpression(retExpr, t.val); 
			} else if (la.kind == 17) {
				lexer.NextToken();
				Expr(
#line  1961 "cs.ATG" 
out expr);

#line  1961 "cs.ATG" 
				ArrayList indices = new ArrayList(); if (expr != null) { indices.Add(expr); } 
				while (la.kind == 13) {
					lexer.NextToken();
					Expr(
#line  1962 "cs.ATG" 
out expr);

#line  1962 "cs.ATG" 
					if (expr != null) { indices.Add(expr); } 
				}
				Expect(18);

#line  1963 "cs.ATG" 
				retExpr = new IndexerExpression(retExpr, indices); 
			} else SynErr(176);

#line  1964 "cs.ATG" 
			pexpr = retExpr; 
		} else if (la.kind == 87) {
			lexer.NextToken();
			NonArrayType(
#line  1965 "cs.ATG" 
out type);

#line  1965 "cs.ATG" 
			ArrayList parameters = new ArrayList(); 
			if (la.kind == 19) {
				lexer.NextToken();

#line  1970 "cs.ATG" 
				ObjectCreateExpression oce = new ObjectCreateExpression(type, parameters); 
				if (StartOf(21)) {
					Argument(
#line  1971 "cs.ATG" 
out expr);

#line  1971 "cs.ATG" 
					if (expr != null) { parameters.Add(expr); } 
					while (la.kind == 13) {
						lexer.NextToken();
						Argument(
#line  1972 "cs.ATG" 
out expr);

#line  1972 "cs.ATG" 
						if (expr != null) { parameters.Add(expr); } 
					}
				}
				Expect(20);

#line  1974 "cs.ATG" 
				pexpr = oce; 
			} else if (la.kind == 17) {

#line  1976 "cs.ATG" 
				isArrayCreation = true; ArrayCreateExpression ace = new ArrayCreateExpression(type); pexpr = ace; 
				lexer.NextToken();

#line  1977 "cs.ATG" 
				int dims = 0; 
				ArrayList rank = new ArrayList(); 
				ArrayList parameterExpression = new ArrayList(); 
				if (StartOf(4)) {
					Expr(
#line  1981 "cs.ATG" 
out expr);

#line  1981 "cs.ATG" 
					if (expr != null) { parameterExpression.Add(expr); } 
					while (la.kind == 13) {
						lexer.NextToken();
						Expr(
#line  1983 "cs.ATG" 
out expr);

#line  1983 "cs.ATG" 
						if (expr != null) { parameterExpression.Add(expr); } 
					}
					Expect(18);

#line  1985 "cs.ATG" 
					parameters.Add(new ArrayCreationParameter(parameterExpression)); 
					ace.Parameters = parameters; 
					while (
#line  1988 "cs.ATG" 
IsDims()) {
						Expect(17);

#line  1988 "cs.ATG" 
						dims =0;
						while (la.kind == 13) {
							lexer.NextToken();

#line  1989 "cs.ATG" 
							dims++;
						}

#line  1989 "cs.ATG" 
						rank.Add(dims); 
						parameters.Add(new ArrayCreationParameter(dims)); 
						
						Expect(18);
					}

#line  1993 "cs.ATG" 
					if (rank.Count > 0) { 
					ace.Rank = (int[])rank.ToArray(typeof (int)); 
					} 
					
					if (la.kind == 15) {
						ArrayInitializer(
#line  1997 "cs.ATG" 
out expr);

#line  1997 "cs.ATG" 
						ace.ArrayInitializer = (ArrayInitializerExpression)expr; 
					}
				} else if (la.kind == 13 || la.kind == 18) {
					while (la.kind == 13) {
						lexer.NextToken();

#line  1999 "cs.ATG" 
						dims++;
					}

#line  2000 "cs.ATG" 
					parameters.Add(new ArrayCreationParameter(dims)); 
					
					Expect(18);
					while (
#line  2002 "cs.ATG" 
IsDims()) {
						Expect(17);

#line  2002 "cs.ATG" 
						dims =0;
						while (la.kind == 13) {
							lexer.NextToken();

#line  2002 "cs.ATG" 
							dims++;
						}

#line  2002 "cs.ATG" 
						parameters.Add(new ArrayCreationParameter(dims)); 
						Expect(18);
					}
					ArrayInitializer(
#line  2002 "cs.ATG" 
out expr);

#line  2002 "cs.ATG" 
					ace.ArrayInitializer = (ArrayInitializerExpression)expr; ace.Parameters = parameters; 
				} else SynErr(177);
			} else SynErr(178);
		} else if (la.kind == 113) {
			lexer.NextToken();
			Expect(19);
			if (
#line  2008 "cs.ATG" 
NotVoidPointer()) {
				Expect(121);

#line  2008 "cs.ATG" 
				type = new TypeReference("void"); 
			} else if (StartOf(8)) {
				Type(
#line  2009 "cs.ATG" 
out type);
			} else SynErr(179);
			Expect(20);

#line  2010 "cs.ATG" 
			pexpr = new TypeOfExpression(type); 
		} else if (la.kind == 103) {
			lexer.NextToken();
			Expect(19);
			Type(
#line  2011 "cs.ATG" 
out type);
			Expect(20);

#line  2011 "cs.ATG" 
			pexpr = new SizeOfExpression(type); 
		} else if (la.kind == 56) {
			lexer.NextToken();
			Expect(19);
			Expr(
#line  2012 "cs.ATG" 
out expr);
			Expect(20);

#line  2012 "cs.ATG" 
			pexpr = new CheckedExpression(expr); 
		} else if (la.kind == 116) {
			lexer.NextToken();
			Expect(19);
			Expr(
#line  2013 "cs.ATG" 
out expr);
			Expect(20);

#line  2013 "cs.ATG" 
			pexpr = new UncheckedExpression(expr); 
		} else if (la.kind == 62) {
			lexer.NextToken();
			AnonymousMethodExpr(
#line  2014 "cs.ATG" 
out expr);

#line  2014 "cs.ATG" 
			pexpr = expr; 
		} else SynErr(180);
		while (StartOf(26) || 
#line  2035 "cs.ATG" 
IsGenericFollowedBy(Tokens.Dot) && IsTypeReferenceExpression(pexpr) || 
#line  2043 "cs.ATG" 
IsGenericFollowedBy(Tokens.OpenParenthesis)) {
			if (la.kind == 30 || la.kind == 31) {
				if (la.kind == 30) {
					lexer.NextToken();

#line  2018 "cs.ATG" 
					pexpr = new UnaryOperatorExpression(pexpr, UnaryOperatorType.PostIncrement); 
				} else if (la.kind == 31) {
					lexer.NextToken();

#line  2019 "cs.ATG" 
					pexpr = new UnaryOperatorExpression(pexpr, UnaryOperatorType.PostDecrement); 
				} else SynErr(181);
			} else if (la.kind == 46) {
				lexer.NextToken();
				Expect(1);

#line  2022 "cs.ATG" 
				pexpr = new PointerReferenceExpression(pexpr, t.val); 
			} else if (la.kind == 14) {
				lexer.NextToken();
				Expect(1);

#line  2023 "cs.ATG" 
				pexpr = new FieldReferenceExpression(pexpr, t.val);
			} else if (
#line  2035 "cs.ATG" 
IsGenericFollowedBy(Tokens.Dot) && IsTypeReferenceExpression(pexpr)) {
				TypeArgumentList(
#line  2036 "cs.ATG" 
out typeList);
				Expect(14);
				Expect(1);

#line  2037 "cs.ATG" 
				pexpr = new FieldReferenceExpression(GetTypeReferenceExpression(pexpr, typeList), t.val);
			} else if (la.kind == 19) {
				lexer.NextToken();

#line  2039 "cs.ATG" 
				ArrayList parameters = new ArrayList(); 
				if (StartOf(21)) {
					Argument(
#line  2040 "cs.ATG" 
out expr);

#line  2040 "cs.ATG" 
					if (expr != null) {parameters.Add(expr);} 
					while (la.kind == 13) {
						lexer.NextToken();
						Argument(
#line  2041 "cs.ATG" 
out expr);

#line  2041 "cs.ATG" 
						if (expr != null) {parameters.Add(expr);} 
					}
				}
				Expect(20);

#line  2042 "cs.ATG" 
				pexpr = new InvocationExpression(pexpr, parameters); 
			} else if (
#line  2043 "cs.ATG" 
IsGenericFollowedBy(Tokens.OpenParenthesis)) {
				TypeArgumentList(
#line  2043 "cs.ATG" 
out typeList);
				Expect(19);

#line  2044 "cs.ATG" 
				ArrayList parameters = new ArrayList(); 
				if (StartOf(21)) {
					Argument(
#line  2045 "cs.ATG" 
out expr);

#line  2045 "cs.ATG" 
					if (expr != null) {parameters.Add(expr);} 
					while (la.kind == 13) {
						lexer.NextToken();
						Argument(
#line  2046 "cs.ATG" 
out expr);

#line  2046 "cs.ATG" 
						if (expr != null) {parameters.Add(expr);} 
					}
				}
				Expect(20);

#line  2047 "cs.ATG" 
				pexpr = new InvocationExpression(pexpr, parameters, typeList); 
			} else {

#line  2049 "cs.ATG" 
				if (isArrayCreation) Error("element access not allow on array creation");
				ArrayList indices = new ArrayList();
				
				lexer.NextToken();
				Expr(
#line  2052 "cs.ATG" 
out expr);

#line  2052 "cs.ATG" 
				if (expr != null) { indices.Add(expr); } 
				while (la.kind == 13) {
					lexer.NextToken();
					Expr(
#line  2053 "cs.ATG" 
out expr);

#line  2053 "cs.ATG" 
					if (expr != null) { indices.Add(expr); } 
				}
				Expect(18);

#line  2054 "cs.ATG" 
				pexpr = new IndexerExpression(pexpr, indices); 
			}
		}
	}

	void AnonymousMethodExpr(
#line  2058 "cs.ATG" 
out Expression outExpr) {

#line  2060 "cs.ATG" 
		AnonymousMethodExpression expr = new AnonymousMethodExpression();
		expr.StartLocation = t.Location;
		Statement stmt;
		List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
		outExpr = expr;
		
		Expect(19);
		if (StartOf(9)) {
			FormalParameterList(
#line  2068 "cs.ATG" 
p);

#line  2068 "cs.ATG" 
			expr.Parameters = p; 
		}
		Expect(20);

#line  2072 "cs.ATG" 
		if (compilationUnit != null) { 
		Block(
#line  2073 "cs.ATG" 
out stmt);

#line  2073 "cs.ATG" 
		expr.Body  = (BlockStatement)stmt; 

#line  2074 "cs.ATG" 
		} else { 
		Expect(15);

#line  2076 "cs.ATG" 
		lexer.SkipCurrentBlock(); 
		Expect(16);

#line  2078 "cs.ATG" 
		} 

#line  2079 "cs.ATG" 
		expr.EndLocation = t.Location; 
	}

	void TypeArgumentList(
#line  2228 "cs.ATG" 
out List<TypeReference> types) {

#line  2230 "cs.ATG" 
		types = new List<TypeReference>();
		TypeReference type = null;
		
		Expect(22);
		Type(
#line  2234 "cs.ATG" 
out type);

#line  2234 "cs.ATG" 
		types.Add(type); 
		while (la.kind == 13) {
			lexer.NextToken();
			Type(
#line  2235 "cs.ATG" 
out type);

#line  2235 "cs.ATG" 
			types.Add(type); 
		}
		Expect(21);
	}

	void ConditionalAndExpr(
#line  2088 "cs.ATG" 
ref Expression outExpr) {

#line  2089 "cs.ATG" 
		Expression expr; 
		InclusiveOrExpr(
#line  2091 "cs.ATG" 
ref outExpr);
		while (la.kind == 24) {
			lexer.NextToken();
			UnaryExpr(
#line  2091 "cs.ATG" 
out expr);
			InclusiveOrExpr(
#line  2091 "cs.ATG" 
ref expr);

#line  2091 "cs.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.LogicalAnd, expr);  
		}
	}

	void InclusiveOrExpr(
#line  2094 "cs.ATG" 
ref Expression outExpr) {

#line  2095 "cs.ATG" 
		Expression expr; 
		ExclusiveOrExpr(
#line  2097 "cs.ATG" 
ref outExpr);
		while (la.kind == 28) {
			lexer.NextToken();
			UnaryExpr(
#line  2097 "cs.ATG" 
out expr);
			ExclusiveOrExpr(
#line  2097 "cs.ATG" 
ref expr);

#line  2097 "cs.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.BitwiseOr, expr);  
		}
	}

	void ExclusiveOrExpr(
#line  2100 "cs.ATG" 
ref Expression outExpr) {

#line  2101 "cs.ATG" 
		Expression expr; 
		AndExpr(
#line  2103 "cs.ATG" 
ref outExpr);
		while (la.kind == 29) {
			lexer.NextToken();
			UnaryExpr(
#line  2103 "cs.ATG" 
out expr);
			AndExpr(
#line  2103 "cs.ATG" 
ref expr);

#line  2103 "cs.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.ExclusiveOr, expr);  
		}
	}

	void AndExpr(
#line  2106 "cs.ATG" 
ref Expression outExpr) {

#line  2107 "cs.ATG" 
		Expression expr; 
		EqualityExpr(
#line  2109 "cs.ATG" 
ref outExpr);
		while (la.kind == 27) {
			lexer.NextToken();
			UnaryExpr(
#line  2109 "cs.ATG" 
out expr);
			EqualityExpr(
#line  2109 "cs.ATG" 
ref expr);

#line  2109 "cs.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.BitwiseAnd, expr);  
		}
	}

	void EqualityExpr(
#line  2112 "cs.ATG" 
ref Expression outExpr) {

#line  2114 "cs.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		RelationalExpr(
#line  2118 "cs.ATG" 
ref outExpr);
		while (la.kind == 32 || la.kind == 33) {
			if (la.kind == 33) {
				lexer.NextToken();

#line  2121 "cs.ATG" 
				op = BinaryOperatorType.InEquality; 
			} else {
				lexer.NextToken();

#line  2122 "cs.ATG" 
				op = BinaryOperatorType.Equality; 
			}
			UnaryExpr(
#line  2124 "cs.ATG" 
out expr);
			RelationalExpr(
#line  2124 "cs.ATG" 
ref expr);

#line  2124 "cs.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr);  
		}
	}

	void RelationalExpr(
#line  2128 "cs.ATG" 
ref Expression outExpr) {

#line  2130 "cs.ATG" 
		TypeReference type;
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		ShiftExpr(
#line  2135 "cs.ATG" 
ref outExpr);
		while (StartOf(27)) {
			if (StartOf(28)) {
				if (la.kind == 22) {
					lexer.NextToken();

#line  2138 "cs.ATG" 
					op = BinaryOperatorType.LessThan; 
				} else if (la.kind == 21) {
					lexer.NextToken();

#line  2139 "cs.ATG" 
					op = BinaryOperatorType.GreaterThan; 
				} else if (la.kind == 35) {
					lexer.NextToken();

#line  2140 "cs.ATG" 
					op = BinaryOperatorType.LessThanOrEqual; 
				} else if (la.kind == 34) {
					lexer.NextToken();

#line  2141 "cs.ATG" 
					op = BinaryOperatorType.GreaterThanOrEqual; 
				} else SynErr(182);
				UnaryExpr(
#line  2143 "cs.ATG" 
out expr);
				ShiftExpr(
#line  2143 "cs.ATG" 
ref expr);

#line  2143 "cs.ATG" 
				outExpr = new BinaryOperatorExpression(outExpr, op, expr);  
			} else {
				if (la.kind == 83) {
					lexer.NextToken();

#line  2146 "cs.ATG" 
					op = BinaryOperatorType.TypeCheck; 
				} else if (la.kind == 48) {
					lexer.NextToken();

#line  2147 "cs.ATG" 
					op = BinaryOperatorType.AsCast; 
				} else SynErr(183);
				Type(
#line  2149 "cs.ATG" 
out type);

#line  2149 "cs.ATG" 
				outExpr = new BinaryOperatorExpression(outExpr, op, new TypeReferenceExpression(type)); 
			}
		}
	}

	void ShiftExpr(
#line  2153 "cs.ATG" 
ref Expression outExpr) {

#line  2155 "cs.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		AdditiveExpr(
#line  2159 "cs.ATG" 
ref outExpr);
		while (la.kind == 36 || 
#line  2162 "cs.ATG" 
IsShiftRight()) {
			if (la.kind == 36) {
				lexer.NextToken();

#line  2161 "cs.ATG" 
				op = BinaryOperatorType.ShiftLeft; 
			} else {
				Expect(21);
				Expect(21);

#line  2163 "cs.ATG" 
				op = BinaryOperatorType.ShiftRight; 
			}
			UnaryExpr(
#line  2166 "cs.ATG" 
out expr);
			AdditiveExpr(
#line  2166 "cs.ATG" 
ref expr);

#line  2166 "cs.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr);  
		}
	}

	void AdditiveExpr(
#line  2170 "cs.ATG" 
ref Expression outExpr) {

#line  2172 "cs.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		MultiplicativeExpr(
#line  2176 "cs.ATG" 
ref outExpr);
		while (la.kind == 4 || la.kind == 5) {
			if (la.kind == 4) {
				lexer.NextToken();

#line  2179 "cs.ATG" 
				op = BinaryOperatorType.Add; 
			} else {
				lexer.NextToken();

#line  2180 "cs.ATG" 
				op = BinaryOperatorType.Subtract; 
			}
			UnaryExpr(
#line  2182 "cs.ATG" 
out expr);
			MultiplicativeExpr(
#line  2182 "cs.ATG" 
ref expr);

#line  2182 "cs.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr);  
		}
	}

	void MultiplicativeExpr(
#line  2186 "cs.ATG" 
ref Expression outExpr) {

#line  2188 "cs.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		while (la.kind == 6 || la.kind == 7 || la.kind == 8) {
			if (la.kind == 6) {
				lexer.NextToken();

#line  2194 "cs.ATG" 
				op = BinaryOperatorType.Multiply; 
			} else if (la.kind == 7) {
				lexer.NextToken();

#line  2195 "cs.ATG" 
				op = BinaryOperatorType.Divide; 
			} else {
				lexer.NextToken();

#line  2196 "cs.ATG" 
				op = BinaryOperatorType.Modulus; 
			}
			UnaryExpr(
#line  2198 "cs.ATG" 
out expr);

#line  2198 "cs.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr); 
		}
	}

	void TypeParameterConstraintsClauseBase(
#line  2280 "cs.ATG" 
out TypeReference type) {

#line  2281 "cs.ATG" 
		TypeReference t; type = null; 
		if (la.kind == 107) {
			lexer.NextToken();

#line  2283 "cs.ATG" 
			type = new TypeReference("struct"); 
		} else if (la.kind == 57) {
			lexer.NextToken();

#line  2284 "cs.ATG" 
			type = new TypeReference("struct"); 
		} else if (la.kind == 87) {
			lexer.NextToken();
			Expect(19);
			Expect(20);

#line  2285 "cs.ATG" 
			type = new TypeReference("struct"); 
		} else if (StartOf(8)) {
			Type(
#line  2286 "cs.ATG" 
out t);

#line  2286 "cs.ATG" 
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