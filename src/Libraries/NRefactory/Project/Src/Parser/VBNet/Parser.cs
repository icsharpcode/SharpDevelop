
#line  1 "VBNET.ATG" 
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using ICSharpCode.NRefactory.Parser.AST;
using ICSharpCode.NRefactory.Parser.VB;
using ASTAttribute = ICSharpCode.NRefactory.Parser.AST.Attribute;
/*
  Parser.frame file for NRefactory.
 */
using System;
using System.Reflection;

namespace ICSharpCode.NRefactory.Parser.VB {



internal class Parser : AbstractParser
{
	const int maxT = 205;

	const  bool   T            = true;
	const  bool   x            = false;
	

#line  12 "VBNET.ATG" 
private string assemblyName = null;
private Stack withStatements;
private StringBuilder qualidentBuilder = new StringBuilder();

public string ContainingAssembly
{
	set { assemblyName = value; }
}
Token t
{
	get {
		return lexer.Token;
	}
}
Token la
{
	get {
		return lexer.LookAhead;
	}
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
	True, if ident is followed by "="
*/
bool IdentAndAsgn () {
	if(la.kind == Tokens.Identifier) {
		if(Peek(1).kind == Tokens.Assign) return true;
		if(Peek(1).kind == Tokens.Colon && Peek(2).kind == Tokens.Assign) return true;
	}
	return false;
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

bool IsAssignment ()
{
	return IdentAndAsgn();
}

bool IsMustOverride(Modifiers m)
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

/* START AUTOGENERATED TOKENS SECTION */


/*

*/

	void VBNET() {

#line  479 "VBNET.ATG" 
		compilationUnit = new CompilationUnit();
		withStatements = new Stack();
		
		while (la.kind == 1) {
			lexer.NextToken();
		}
		while (la.kind == 136) {
			OptionStmt();
		}
		while (la.kind == 108) {
			ImportsStmt();
		}
		while (
#line  485 "VBNET.ATG" 
IsGlobalAttrTarget()) {
			GlobalAttributeSection();
		}
		while (StartOf(1)) {
			NamespaceMemberDecl();
		}
		Expect(0);
	}

	void OptionStmt() {

#line  490 "VBNET.ATG" 
		INode node = null; bool val = true; 
		Expect(136);

#line  491 "VBNET.ATG" 
		Point startPos = t.Location; 
		if (la.kind == 95) {
			lexer.NextToken();
			if (la.kind == 134 || la.kind == 135) {
				OptionValue(
#line  493 "VBNET.ATG" 
ref val);
			}

#line  494 "VBNET.ATG" 
			node = new OptionDeclaration(OptionType.Explicit, val); 
		} else if (la.kind == 164) {
			lexer.NextToken();
			if (la.kind == 134 || la.kind == 135) {
				OptionValue(
#line  496 "VBNET.ATG" 
ref val);
			}

#line  497 "VBNET.ATG" 
			node = new OptionDeclaration(OptionType.Strict, val); 
		} else if (la.kind == 70) {
			lexer.NextToken();
			if (la.kind == 51) {
				lexer.NextToken();

#line  499 "VBNET.ATG" 
				node = new OptionDeclaration(OptionType.CompareBinary, val); 
			} else if (la.kind == 169) {
				lexer.NextToken();

#line  500 "VBNET.ATG" 
				node = new OptionDeclaration(OptionType.CompareText, val); 
			} else SynErr(206);
		} else SynErr(207);
		EndOfStmt();

#line  505 "VBNET.ATG" 
		if (node != null) {
		node.StartLocation = startPos;
		node.EndLocation   = t.Location;
		compilationUnit.AddChild(node);
		}
		
	}

	void ImportsStmt() {

#line  528 "VBNET.ATG" 
		List<Using> usings = new List<Using>();
		
		Expect(108);

#line  532 "VBNET.ATG" 
		Point startPos = t.Location;
		Using u;
		
		ImportClause(
#line  535 "VBNET.ATG" 
out u);

#line  535 "VBNET.ATG" 
		usings.Add(u); 
		while (la.kind == 12) {
			lexer.NextToken();
			ImportClause(
#line  537 "VBNET.ATG" 
out u);

#line  537 "VBNET.ATG" 
			usings.Add(u); 
		}
		EndOfStmt();

#line  541 "VBNET.ATG" 
		UsingDeclaration usingDeclaration = new UsingDeclaration(usings);
		usingDeclaration.StartLocation = startPos;
		usingDeclaration.EndLocation   = t.Location;
		compilationUnit.AddChild(usingDeclaration);
		
	}

	void GlobalAttributeSection() {

#line  2137 "VBNET.ATG" 
		Point startPos = t.Location; 
		Expect(27);
		if (la.kind == 49) {
			lexer.NextToken();
		} else if (la.kind == 121) {
			lexer.NextToken();
		} else SynErr(208);

#line  2139 "VBNET.ATG" 
		string attributeTarget = t.val.ToLower(System.Globalization.CultureInfo.InvariantCulture);
		List<ASTAttribute> attributes = new List<ASTAttribute>();
		ASTAttribute attribute;
		
		Expect(13);
		Attribute(
#line  2143 "VBNET.ATG" 
out attribute);

#line  2143 "VBNET.ATG" 
		attributes.Add(attribute); 
		while (
#line  2144 "VBNET.ATG" 
NotFinalComma()) {
			if (la.kind == 12) {
				lexer.NextToken();
				if (la.kind == 49) {
					lexer.NextToken();
				} else if (la.kind == 121) {
					lexer.NextToken();
				} else SynErr(209);
				Expect(13);
			}
			Attribute(
#line  2144 "VBNET.ATG" 
out attribute);

#line  2144 "VBNET.ATG" 
			attributes.Add(attribute); 
		}
		if (la.kind == 12) {
			lexer.NextToken();
		}
		Expect(26);
		EndOfStmt();

#line  2149 "VBNET.ATG" 
		AttributeSection section = new AttributeSection(attributeTarget, attributes);
		section.StartLocation = startPos;
		section.EndLocation = t.EndLocation;
		compilationUnit.AddChild(section);
		
	}

	void NamespaceMemberDecl() {

#line  570 "VBNET.ATG" 
		Modifiers m = new Modifiers();
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		string qualident;
		
		if (la.kind == 126) {
			lexer.NextToken();

#line  577 "VBNET.ATG" 
			Point startPos = t.Location;
			
			Qualident(
#line  579 "VBNET.ATG" 
out qualident);

#line  581 "VBNET.ATG" 
			INode node =  new NamespaceDeclaration(qualident);
			node.StartLocation = startPos;
			compilationUnit.AddChild(node);
			compilationUnit.BlockStart(node);
			
			Expect(1);
			NamespaceBody();

#line  589 "VBNET.ATG" 
			node.EndLocation = t.Location;
			compilationUnit.BlockEnd();
			
		} else if (StartOf(2)) {
			while (la.kind == 27) {
				AttributeSection(
#line  593 "VBNET.ATG" 
out section);

#line  593 "VBNET.ATG" 
				attributes.Add(section); 
			}
			while (StartOf(3)) {
				TypeModifier(
#line  594 "VBNET.ATG" 
m);
			}
			NonModuleDeclaration(
#line  594 "VBNET.ATG" 
m, attributes);
		} else SynErr(210);
	}

	void OptionValue(
#line  513 "VBNET.ATG" 
ref bool val) {
		if (la.kind == 135) {
			lexer.NextToken();

#line  515 "VBNET.ATG" 
			val = true; 
		} else if (la.kind == 134) {
			lexer.NextToken();

#line  517 "VBNET.ATG" 
			val = false; 
		} else SynErr(211);
	}

	void EndOfStmt() {
		if (la.kind == 1) {
			lexer.NextToken();
		} else if (la.kind == 13) {
			lexer.NextToken();
			if (la.kind == 1) {
				lexer.NextToken();
			}
		} else SynErr(212);
	}

	void ImportClause(
#line  548 "VBNET.ATG" 
out Using u) {

#line  550 "VBNET.ATG" 
		string qualident  = null;
		TypeReference aliasedType = null;
		u = null;
		
		Qualident(
#line  554 "VBNET.ATG" 
out qualident);
		if (la.kind == 11) {
			lexer.NextToken();
			TypeName(
#line  555 "VBNET.ATG" 
out aliasedType);
		}

#line  557 "VBNET.ATG" 
		if (qualident != null && qualident.Length > 0) {
		if (aliasedType != null) {
			u = new Using(qualident, aliasedType);
		} else {
			u = new Using(qualident);
		}
		}
		
	}

	void Qualident(
#line  2861 "VBNET.ATG" 
out string qualident) {

#line  2863 "VBNET.ATG" 
		string name;
		qualidentBuilder.Length = 0; 
		
		Identifier();

#line  2867 "VBNET.ATG" 
		qualidentBuilder.Append(t.val); 
		while (
#line  2868 "VBNET.ATG" 
DotAndIdentOrKw()) {
			Expect(10);
			IdentifierOrKeyword(
#line  2868 "VBNET.ATG" 
out name);

#line  2868 "VBNET.ATG" 
			qualidentBuilder.Append('.'); qualidentBuilder.Append(name); 
		}

#line  2870 "VBNET.ATG" 
		qualident = qualidentBuilder.ToString(); 
	}

	void TypeName(
#line  2030 "VBNET.ATG" 
out TypeReference typeref) {

#line  2031 "VBNET.ATG" 
		ArrayList rank = null; 
		NonArrayTypeName(
#line  2033 "VBNET.ATG" 
out typeref, false);
		ArrayTypeModifiers(
#line  2034 "VBNET.ATG" 
out rank);

#line  2035 "VBNET.ATG" 
		if (rank != null && typeref != null) {
		typeref.RankSpecifier = (int[])rank.ToArray(typeof(int));
		}
		
	}

	void NamespaceBody() {
		while (StartOf(1)) {
			NamespaceMemberDecl();
		}
		Expect(88);
		Expect(126);
		Expect(1);
	}

	void AttributeSection(
#line  2206 "VBNET.ATG" 
out AttributeSection section) {

#line  2208 "VBNET.ATG" 
		string attributeTarget = "";List<ASTAttribute> attributes = new List<ASTAttribute>();
		ASTAttribute attribute;
		
		
		Expect(27);

#line  2212 "VBNET.ATG" 
		Point startPos = t.Location; 
		if (
#line  2213 "VBNET.ATG" 
IsLocalAttrTarget()) {
			if (la.kind == 93) {
				lexer.NextToken();

#line  2214 "VBNET.ATG" 
				attributeTarget = "event";
			} else if (la.kind == 154) {
				lexer.NextToken();

#line  2215 "VBNET.ATG" 
				attributeTarget = "return";
			} else {
				Identifier();

#line  2218 "VBNET.ATG" 
				string val = t.val.ToLower(System.Globalization.CultureInfo.InvariantCulture);
				if (val != "field"	|| val != "method" ||
					val != "module" || val != "param"  ||
					val != "property" || val != "type")
				Error("attribute target specifier (event, return, field," +
						"method, module, param, property, or type) expected");
				attributeTarget = t.val;
				
			}
			Expect(13);
		}
		Attribute(
#line  2228 "VBNET.ATG" 
out attribute);

#line  2228 "VBNET.ATG" 
		attributes.Add(attribute); 
		while (
#line  2229 "VBNET.ATG" 
NotFinalComma()) {
			Expect(12);
			Attribute(
#line  2229 "VBNET.ATG" 
out attribute);

#line  2229 "VBNET.ATG" 
			attributes.Add(attribute); 
		}
		if (la.kind == 12) {
			lexer.NextToken();
		}
		Expect(26);

#line  2233 "VBNET.ATG" 
		section = new AttributeSection(attributeTarget, attributes);
		section.StartLocation = startPos;
		section.EndLocation = t.EndLocation;
		
	}

	void TypeModifier(
#line  2922 "VBNET.ATG" 
Modifiers m) {
		switch (la.kind) {
		case 148: {
			lexer.NextToken();

#line  2923 "VBNET.ATG" 
			m.Add(Modifier.Public, t.Location); 
			break;
		}
		case 147: {
			lexer.NextToken();

#line  2924 "VBNET.ATG" 
			m.Add(Modifier.Protected, t.Location); 
			break;
		}
		case 99: {
			lexer.NextToken();

#line  2925 "VBNET.ATG" 
			m.Add(Modifier.Internal, t.Location); 
			break;
		}
		case 145: {
			lexer.NextToken();

#line  2926 "VBNET.ATG" 
			m.Add(Modifier.Private, t.Location); 
			break;
		}
		case 158: {
			lexer.NextToken();

#line  2927 "VBNET.ATG" 
			m.Add(Modifier.Static, t.Location); 
			break;
		}
		case 157: {
			lexer.NextToken();

#line  2928 "VBNET.ATG" 
			m.Add(Modifier.New, t.Location); 
			break;
		}
		case 122: {
			lexer.NextToken();

#line  2929 "VBNET.ATG" 
			m.Add(Modifier.Abstract, t.Location); 
			break;
		}
		case 131: {
			lexer.NextToken();

#line  2930 "VBNET.ATG" 
			m.Add(Modifier.Sealed, t.Location); 
			break;
		}
		case 203: {
			lexer.NextToken();

#line  2931 "VBNET.ATG" 
			m.Add(Modifier.Partial, t.Location); 
			break;
		}
		default: SynErr(213); break;
		}
	}

	void NonModuleDeclaration(
#line  645 "VBNET.ATG" 
Modifiers m, List<AttributeSection> attributes) {

#line  647 "VBNET.ATG" 
		TypeReference typeRef = null;
		List<TypeReference> baseInterfaces = null;
		
		switch (la.kind) {
		case 67: {

#line  650 "VBNET.ATG" 
			m.Check(Modifier.Classes); 
			lexer.NextToken();

#line  653 "VBNET.ATG" 
			TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
			newType.StartLocation = t.Location;
			compilationUnit.AddChild(newType);
			compilationUnit.BlockStart(newType);
			
			newType.Type       = ClassType.Class;
			
			Identifier();

#line  660 "VBNET.ATG" 
			newType.Name = t.val; 
			TypeParameterList(
#line  661 "VBNET.ATG" 
newType.Templates);
			EndOfStmt();
			if (la.kind == 110) {
				ClassBaseType(
#line  663 "VBNET.ATG" 
out typeRef);

#line  663 "VBNET.ATG" 
				newType.BaseTypes.Add(typeRef); 
			}
			while (la.kind == 107) {
				TypeImplementsClause(
#line  664 "VBNET.ATG" 
out baseInterfaces);

#line  664 "VBNET.ATG" 
				newType.BaseTypes.AddRange(baseInterfaces); 
			}
			ClassBody(
#line  665 "VBNET.ATG" 
newType);

#line  667 "VBNET.ATG" 
			compilationUnit.BlockEnd();
			
			break;
		}
		case 121: {
			lexer.NextToken();

#line  671 "VBNET.ATG" 
			m.Check(Modifier.VBModules);
			TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
			compilationUnit.AddChild(newType);
			compilationUnit.BlockStart(newType);
			newType.StartLocation = m.GetDeclarationLocation(t.Location);
			newType.Type = ClassType.Module;
			
			Identifier();

#line  678 "VBNET.ATG" 
			newType.Name = t.val; 
			Expect(1);
			ModuleBody(
#line  680 "VBNET.ATG" 
newType);

#line  682 "VBNET.ATG" 
			compilationUnit.BlockEnd();
			
			break;
		}
		case 166: {
			lexer.NextToken();

#line  686 "VBNET.ATG" 
			m.Check(Modifier.VBStructures);
			TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
			compilationUnit.AddChild(newType);
			compilationUnit.BlockStart(newType);
			newType.StartLocation = m.GetDeclarationLocation(t.Location);
			newType.Type = ClassType.Struct;
			
			Identifier();

#line  693 "VBNET.ATG" 
			newType.Name = t.val; 
			TypeParameterList(
#line  694 "VBNET.ATG" 
newType.Templates);
			Expect(1);
			while (la.kind == 107) {
				TypeImplementsClause(
#line  695 "VBNET.ATG" 
out baseInterfaces);

#line  695 "VBNET.ATG" 
				newType.BaseTypes.AddRange(baseInterfaces);
			}
			StructureBody(
#line  696 "VBNET.ATG" 
newType);

#line  698 "VBNET.ATG" 
			compilationUnit.BlockEnd();
			
			break;
		}
		case 90: {
			lexer.NextToken();

#line  703 "VBNET.ATG" 
			m.Check(Modifier.VBEnums);
			TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
			newType.StartLocation = m.GetDeclarationLocation(t.Location);
			compilationUnit.AddChild(newType);
			compilationUnit.BlockStart(newType);
			
			newType.Type = ClassType.Enum;
			
			Identifier();

#line  711 "VBNET.ATG" 
			newType.Name = t.val; 
			if (la.kind == 48) {
				lexer.NextToken();
				NonArrayTypeName(
#line  712 "VBNET.ATG" 
out typeRef, false);

#line  712 "VBNET.ATG" 
				newType.BaseTypes.Add(typeRef); 
			}
			Expect(1);
			EnumBody(
#line  714 "VBNET.ATG" 
newType);

#line  716 "VBNET.ATG" 
			compilationUnit.BlockEnd();
			
			break;
		}
		case 112: {
			lexer.NextToken();

#line  721 "VBNET.ATG" 
			m.Check(Modifier.VBInterfacs);
			TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
			newType.StartLocation = m.GetDeclarationLocation(t.Location);
			compilationUnit.AddChild(newType);
			compilationUnit.BlockStart(newType);
			newType.Type = ClassType.Interface;
			
			Identifier();

#line  728 "VBNET.ATG" 
			newType.Name = t.val; 
			TypeParameterList(
#line  729 "VBNET.ATG" 
newType.Templates);
			EndOfStmt();
			while (la.kind == 110) {
				InterfaceBase(
#line  730 "VBNET.ATG" 
out baseInterfaces);

#line  730 "VBNET.ATG" 
				newType.BaseTypes.AddRange(baseInterfaces); 
			}
			InterfaceBody(
#line  731 "VBNET.ATG" 
newType);

#line  733 "VBNET.ATG" 
			compilationUnit.BlockEnd();
			
			break;
		}
		case 80: {
			lexer.NextToken();

#line  738 "VBNET.ATG" 
			m.Check(Modifier.VBDelegates);
			DelegateDeclaration delegateDeclr = new DelegateDeclaration(m.Modifier, attributes);
			delegateDeclr.ReturnType = new TypeReference("", "System.Void");
			delegateDeclr.StartLocation = m.GetDeclarationLocation(t.Location);
			List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
			
			if (la.kind == 167) {
				lexer.NextToken();
				Identifier();

#line  745 "VBNET.ATG" 
				delegateDeclr.Name = t.val; 
				TypeParameterList(
#line  746 "VBNET.ATG" 
delegateDeclr.Templates);
				if (la.kind == 24) {
					lexer.NextToken();
					if (StartOf(4)) {
						FormalParameterList(
#line  747 "VBNET.ATG" 
p);
					}
					Expect(25);

#line  747 "VBNET.ATG" 
					delegateDeclr.Parameters = p; 
				}
			} else if (la.kind == 100) {
				lexer.NextToken();
				Identifier();

#line  749 "VBNET.ATG" 
				delegateDeclr.Name = t.val; 
				TypeParameterList(
#line  750 "VBNET.ATG" 
delegateDeclr.Templates);
				if (la.kind == 24) {
					lexer.NextToken();
					if (StartOf(4)) {
						FormalParameterList(
#line  751 "VBNET.ATG" 
p);
					}
					Expect(25);

#line  751 "VBNET.ATG" 
					delegateDeclr.Parameters = p; 
				}
				if (la.kind == 48) {
					lexer.NextToken();

#line  752 "VBNET.ATG" 
					TypeReference type; 
					TypeName(
#line  752 "VBNET.ATG" 
out type);

#line  752 "VBNET.ATG" 
					delegateDeclr.ReturnType = type; 
				}
			} else SynErr(214);

#line  754 "VBNET.ATG" 
			delegateDeclr.EndLocation = t.EndLocation; 
			Expect(1);

#line  757 "VBNET.ATG" 
			compilationUnit.AddChild(delegateDeclr);
			
			break;
		}
		default: SynErr(215); break;
		}
	}

	void TypeParameterList(
#line  598 "VBNET.ATG" 
List<TemplateDefinition> templates) {

#line  600 "VBNET.ATG" 
		TemplateDefinition template;
		
		if (
#line  603 "VBNET.ATG" 
la.kind == Tokens.OpenParenthesis && Peek(1).kind == Tokens.Of) {
			lexer.NextToken();
			Expect(200);
			TypeParameter(
#line  604 "VBNET.ATG" 
out template);

#line  606 "VBNET.ATG" 
			if (template != null) templates.Add(template);
			
			while (la.kind == 12) {
				lexer.NextToken();
				TypeParameter(
#line  609 "VBNET.ATG" 
out template);

#line  611 "VBNET.ATG" 
				if (template != null) templates.Add(template);
				
			}
			Expect(25);
		}
	}

	void TypeParameter(
#line  619 "VBNET.ATG" 
out TemplateDefinition template) {
		Identifier();

#line  621 "VBNET.ATG" 
		template = new TemplateDefinition(t.val, null); 
		if (la.kind == 48) {
			TypeParameterConstraints(
#line  622 "VBNET.ATG" 
template);
		}
	}

	void Identifier() {
		switch (la.kind) {
		case 2: {
			lexer.NextToken();
			break;
		}
		case 169: {
			lexer.NextToken();
			break;
		}
		case 51: {
			lexer.NextToken();
			break;
		}
		case 70: {
			lexer.NextToken();
			break;
		}
		case 49: {
			lexer.NextToken();
			break;
		}
		case 47: {
			lexer.NextToken();
			break;
		}
		case 50: {
			lexer.NextToken();
			break;
		}
		case 144: {
			lexer.NextToken();
			break;
		}
		case 176: {
			lexer.NextToken();
			break;
		}
		case 177: {
			lexer.NextToken();
			break;
		}
		default: SynErr(216); break;
		}
	}

	void TypeParameterConstraints(
#line  626 "VBNET.ATG" 
TemplateDefinition template) {

#line  628 "VBNET.ATG" 
		TypeReference constraint;
		
		Expect(48);
		if (la.kind == 22) {
			lexer.NextToken();
			TypeName(
#line  634 "VBNET.ATG" 
out constraint);

#line  634 "VBNET.ATG" 
			if (constraint != null) { template.Bases.Add(constraint); } 
			while (la.kind == 12) {
				lexer.NextToken();
				TypeName(
#line  637 "VBNET.ATG" 
out constraint);

#line  637 "VBNET.ATG" 
				if (constraint != null) { template.Bases.Add(constraint); } 
			}
			Expect(23);
		} else if (StartOf(5)) {
			TypeName(
#line  640 "VBNET.ATG" 
out constraint);

#line  640 "VBNET.ATG" 
			if (constraint != null) { template.Bases.Add(constraint); } 
		} else SynErr(217);
	}

	void ClassBaseType(
#line  931 "VBNET.ATG" 
out TypeReference typeRef) {

#line  933 "VBNET.ATG" 
		typeRef = null;
		
		Expect(110);
		TypeName(
#line  936 "VBNET.ATG" 
out typeRef);
		EndOfStmt();
	}

	void TypeImplementsClause(
#line  1649 "VBNET.ATG" 
out List<TypeReference> baseInterfaces) {

#line  1651 "VBNET.ATG" 
		baseInterfaces = new List<TypeReference>();
		TypeReference type = null;
		
		Expect(107);
		TypeName(
#line  1654 "VBNET.ATG" 
out type);

#line  1656 "VBNET.ATG" 
		baseInterfaces.Add(type);
		
		while (la.kind == 12) {
			lexer.NextToken();
			TypeName(
#line  1659 "VBNET.ATG" 
out type);

#line  1660 "VBNET.ATG" 
			baseInterfaces.Add(type); 
		}
		EndOfStmt();
	}

	void ClassBody(
#line  767 "VBNET.ATG" 
TypeDeclaration newType) {

#line  768 "VBNET.ATG" 
		AttributeSection section; 
		while (StartOf(6)) {

#line  770 "VBNET.ATG" 
			List<AttributeSection> attributes = new List<AttributeSection>();
			Modifiers m = new Modifiers();
			
			while (la.kind == 27) {
				AttributeSection(
#line  773 "VBNET.ATG" 
out section);

#line  773 "VBNET.ATG" 
				attributes.Add(section); 
			}
			while (StartOf(7)) {
				MemberModifier(
#line  774 "VBNET.ATG" 
m);
			}
			ClassMemberDecl(
#line  775 "VBNET.ATG" 
m, attributes);
		}
		Expect(88);
		Expect(67);

#line  777 "VBNET.ATG" 
		newType.EndLocation = t.EndLocation; 
		Expect(1);
	}

	void ModuleBody(
#line  796 "VBNET.ATG" 
TypeDeclaration newType) {

#line  797 "VBNET.ATG" 
		AttributeSection section; 
		while (StartOf(6)) {

#line  799 "VBNET.ATG" 
			List<AttributeSection> attributes = new List<AttributeSection>();
			Modifiers m = new Modifiers();
			
			while (la.kind == 27) {
				AttributeSection(
#line  802 "VBNET.ATG" 
out section);

#line  802 "VBNET.ATG" 
				attributes.Add(section); 
			}
			while (StartOf(7)) {
				MemberModifier(
#line  803 "VBNET.ATG" 
m);
			}
			ClassMemberDecl(
#line  804 "VBNET.ATG" 
m, attributes);
		}
		Expect(88);
		Expect(121);

#line  806 "VBNET.ATG" 
		newType.EndLocation = t.EndLocation; 
		Expect(1);
	}

	void StructureBody(
#line  781 "VBNET.ATG" 
TypeDeclaration newType) {

#line  782 "VBNET.ATG" 
		AttributeSection section; 
		while (StartOf(6)) {

#line  784 "VBNET.ATG" 
			List<AttributeSection> attributes = new List<AttributeSection>();
			Modifiers m = new Modifiers();
			
			while (la.kind == 27) {
				AttributeSection(
#line  787 "VBNET.ATG" 
out section);

#line  787 "VBNET.ATG" 
				attributes.Add(section); 
			}
			while (StartOf(7)) {
				MemberModifier(
#line  788 "VBNET.ATG" 
m);
			}
			StructureMemberDecl(
#line  789 "VBNET.ATG" 
m, attributes);
		}
		Expect(88);
		Expect(166);

#line  791 "VBNET.ATG" 
		newType.EndLocation = t.EndLocation; 
		Expect(1);
	}

	void NonArrayTypeName(
#line  2053 "VBNET.ATG" 
out TypeReference typeref, bool canBeUnbound) {

#line  2055 "VBNET.ATG" 
		string name;
		typeref = null;
		bool isGlobal = false;
		
		if (StartOf(8)) {
			if (la.kind == 198) {
				lexer.NextToken();
				Expect(10);

#line  2060 "VBNET.ATG" 
				isGlobal = true; 
			}
			QualIdentAndTypeArguments(
#line  2061 "VBNET.ATG" 
out typeref, canBeUnbound);

#line  2062 "VBNET.ATG" 
			typeref.IsGlobal = isGlobal; 
			while (la.kind == 10) {
				lexer.NextToken();

#line  2063 "VBNET.ATG" 
				TypeReference nestedTypeRef; 
				QualIdentAndTypeArguments(
#line  2064 "VBNET.ATG" 
out nestedTypeRef, canBeUnbound);

#line  2065 "VBNET.ATG" 
				typeref = new InnerClassTypeReference(typeref, nestedTypeRef.Type, nestedTypeRef.GenericTypes); 
			}
		} else if (la.kind == 133) {
			lexer.NextToken();

#line  2068 "VBNET.ATG" 
			typeref = new TypeReference("System.Object"); 
		} else if (StartOf(9)) {
			PrimitiveTypeName(
#line  2069 "VBNET.ATG" 
out name);

#line  2069 "VBNET.ATG" 
			typeref = new TypeReference(name); 
		} else SynErr(218);
	}

	void EnumBody(
#line  810 "VBNET.ATG" 
TypeDeclaration newType) {

#line  811 "VBNET.ATG" 
		FieldDeclaration f; 
		while (StartOf(10)) {
			EnumMemberDecl(
#line  813 "VBNET.ATG" 
out f);

#line  813 "VBNET.ATG" 
			compilationUnit.AddChild(f); 
		}
		Expect(88);
		Expect(90);

#line  815 "VBNET.ATG" 
		newType.EndLocation = t.EndLocation; 
		Expect(1);
	}

	void InterfaceBase(
#line  1634 "VBNET.ATG" 
out List<TypeReference> bases) {

#line  1636 "VBNET.ATG" 
		TypeReference type;
		bases = new List<TypeReference>();
		
		Expect(110);
		TypeName(
#line  1640 "VBNET.ATG" 
out type);

#line  1640 "VBNET.ATG" 
		bases.Add(type); 
		while (la.kind == 12) {
			lexer.NextToken();
			TypeName(
#line  1643 "VBNET.ATG" 
out type);

#line  1643 "VBNET.ATG" 
			bases.Add(type); 
		}
		Expect(1);
	}

	void InterfaceBody(
#line  819 "VBNET.ATG" 
TypeDeclaration newType) {
		while (StartOf(11)) {
			InterfaceMemberDecl();
		}
		Expect(88);
		Expect(112);

#line  821 "VBNET.ATG" 
		newType.EndLocation = t.EndLocation; 
		Expect(1);
	}

	void FormalParameterList(
#line  2240 "VBNET.ATG" 
List<ParameterDeclarationExpression> parameter) {

#line  2242 "VBNET.ATG" 
		ParameterDeclarationExpression p;
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		
		while (la.kind == 27) {
			AttributeSection(
#line  2246 "VBNET.ATG" 
out section);

#line  2246 "VBNET.ATG" 
			attributes.Add(section); 
		}
		FormalParameter(
#line  2248 "VBNET.ATG" 
out p);

#line  2250 "VBNET.ATG" 
		bool paramsFound = false;
		p.Attributes = attributes;
		parameter.Add(p);
		
		while (la.kind == 12) {
			lexer.NextToken();

#line  2255 "VBNET.ATG" 
			if (paramsFound) Error("params array must be at end of parameter list"); 
			while (la.kind == 27) {
				AttributeSection(
#line  2256 "VBNET.ATG" 
out section);

#line  2256 "VBNET.ATG" 
				attributes.Add(section); 
			}
			FormalParameter(
#line  2258 "VBNET.ATG" 
out p);

#line  2258 "VBNET.ATG" 
			p.Attributes = attributes; parameter.Add(p); 
		}
	}

	void MemberModifier(
#line  2934 "VBNET.ATG" 
Modifiers m) {
		switch (la.kind) {
		case 122: {
			lexer.NextToken();

#line  2935 "VBNET.ATG" 
			m.Add(Modifier.Abstract, t.Location);
			break;
		}
		case 79: {
			lexer.NextToken();

#line  2936 "VBNET.ATG" 
			m.Add(Modifier.Default, t.Location);
			break;
		}
		case 99: {
			lexer.NextToken();

#line  2937 "VBNET.ATG" 
			m.Add(Modifier.Internal, t.Location);
			break;
		}
		case 157: {
			lexer.NextToken();

#line  2938 "VBNET.ATG" 
			m.Add(Modifier.New, t.Location);
			break;
		}
		case 142: {
			lexer.NextToken();

#line  2939 "VBNET.ATG" 
			m.Add(Modifier.Override, t.Location);
			break;
		}
		case 123: {
			lexer.NextToken();

#line  2940 "VBNET.ATG" 
			m.Add(Modifier.Abstract, t.Location);
			break;
		}
		case 145: {
			lexer.NextToken();

#line  2941 "VBNET.ATG" 
			m.Add(Modifier.Private, t.Location);
			break;
		}
		case 147: {
			lexer.NextToken();

#line  2942 "VBNET.ATG" 
			m.Add(Modifier.Protected, t.Location);
			break;
		}
		case 148: {
			lexer.NextToken();

#line  2943 "VBNET.ATG" 
			m.Add(Modifier.Public, t.Location);
			break;
		}
		case 131: {
			lexer.NextToken();

#line  2944 "VBNET.ATG" 
			m.Add(Modifier.Sealed, t.Location);
			break;
		}
		case 132: {
			lexer.NextToken();

#line  2945 "VBNET.ATG" 
			m.Add(Modifier.Sealed, t.Location);
			break;
		}
		case 158: {
			lexer.NextToken();

#line  2946 "VBNET.ATG" 
			m.Add(Modifier.Static, t.Location);
			break;
		}
		case 141: {
			lexer.NextToken();

#line  2947 "VBNET.ATG" 
			m.Add(Modifier.Virtual, t.Location);
			break;
		}
		case 140: {
			lexer.NextToken();

#line  2948 "VBNET.ATG" 
			m.Add(Modifier.Overloads, t.Location);
			break;
		}
		case 150: {
			lexer.NextToken();

#line  2949 "VBNET.ATG" 
			m.Add(Modifier.ReadOnly, t.Location);
			break;
		}
		case 184: {
			lexer.NextToken();

#line  2950 "VBNET.ATG" 
			m.Add(Modifier.WriteOnly, t.Location);
			break;
		}
		case 183: {
			lexer.NextToken();

#line  2951 "VBNET.ATG" 
			m.Add(Modifier.WithEvents, t.Location);
			break;
		}
		case 81: {
			lexer.NextToken();

#line  2952 "VBNET.ATG" 
			m.Add(Modifier.Dim, t.Location);
			break;
		}
		case 202: {
			lexer.NextToken();

#line  2953 "VBNET.ATG" 
			m.Add(Modifier.Widening, t.Location);
			break;
		}
		case 201: {
			lexer.NextToken();

#line  2954 "VBNET.ATG" 
			m.Add(Modifier.Narrowing, t.Location);
			break;
		}
		default: SynErr(219); break;
		}
	}

	void ClassMemberDecl(
#line  927 "VBNET.ATG" 
Modifiers m, List<AttributeSection> attributes) {
		StructureMemberDecl(
#line  928 "VBNET.ATG" 
m, attributes);
	}

	void StructureMemberDecl(
#line  941 "VBNET.ATG" 
Modifiers m, List<AttributeSection> attributes) {

#line  943 "VBNET.ATG" 
		TypeReference type = null;
		List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
		Statement stmt = null;
		List<VariableDeclaration> variableDeclarators = new List<VariableDeclaration>();
		List<TemplateDefinition> templates = new List<TemplateDefinition>();
		
		switch (la.kind) {
		case 67: case 80: case 90: case 112: case 121: case 166: {
			NonModuleDeclaration(
#line  949 "VBNET.ATG" 
m, attributes);
			break;
		}
		case 167: {
			lexer.NextToken();

#line  953 "VBNET.ATG" 
			Point startPos = t.Location;
			
			if (StartOf(12)) {

#line  957 "VBNET.ATG" 
				string name = String.Empty;
				MethodDeclaration methodDeclaration; ArrayList handlesClause = null;
				List<InterfaceImplementation> implementsClause = null;
				
				Identifier();

#line  963 "VBNET.ATG" 
				name = t.val;
				m.Check(Modifier.VBMethods);
				
				TypeParameterList(
#line  966 "VBNET.ATG" 
templates);
				if (la.kind == 24) {
					lexer.NextToken();
					if (StartOf(4)) {
						FormalParameterList(
#line  967 "VBNET.ATG" 
p);
					}
					Expect(25);
				}
				if (la.kind == 105 || la.kind == 107) {
					if (la.kind == 107) {
						ImplementsClause(
#line  970 "VBNET.ATG" 
out implementsClause);
					} else {
						HandlesClause(
#line  972 "VBNET.ATG" 
out handlesClause);
					}
				}

#line  975 "VBNET.ATG" 
				Point endLocation = t.EndLocation; 
				Expect(1);
				if (
#line  979 "VBNET.ATG" 
IsMustOverride(m)) {

#line  981 "VBNET.ATG" 
					methodDeclaration = new MethodDeclaration(name, m.Modifier,  null, p, attributes);
					methodDeclaration.StartLocation = m.GetDeclarationLocation(startPos);
					methodDeclaration.EndLocation   = endLocation;
					methodDeclaration.TypeReference = new TypeReference("", "System.Void");
					
					methodDeclaration.Templates = templates;
					methodDeclaration.HandlesClause = handlesClause;
					methodDeclaration.InterfaceImplementations = implementsClause;
					
					compilationUnit.AddChild(methodDeclaration);
					
				} else if (StartOf(13)) {

#line  994 "VBNET.ATG" 
					methodDeclaration = new MethodDeclaration(name, m.Modifier,  null, p, attributes);
					methodDeclaration.StartLocation = m.GetDeclarationLocation(startPos);
					methodDeclaration.EndLocation   = endLocation;
					methodDeclaration.TypeReference = new TypeReference("", "System.Void");
					
					methodDeclaration.Templates = templates;
					methodDeclaration.HandlesClause = handlesClause;
					methodDeclaration.InterfaceImplementations = implementsClause;
					
					compilationUnit.AddChild(methodDeclaration);
					compilationUnit.BlockStart(methodDeclaration);
					
					Block(
#line  1006 "VBNET.ATG" 
out stmt);

#line  1008 "VBNET.ATG" 
					compilationUnit.BlockEnd();
					methodDeclaration.Body  = (BlockStatement)stmt;
					
					Expect(88);
					Expect(167);

#line  1011 "VBNET.ATG" 
					methodDeclaration.Body.EndLocation = t.EndLocation; 
					Expect(1);
				} else SynErr(220);
			} else if (la.kind == 127) {
				lexer.NextToken();
				if (la.kind == 24) {
					lexer.NextToken();
					if (StartOf(4)) {
						FormalParameterList(
#line  1014 "VBNET.ATG" 
p);
					}
					Expect(25);
				}

#line  1015 "VBNET.ATG" 
				m.Check(Modifier.Constructors); 

#line  1016 "VBNET.ATG" 
				Point constructorEndLocation = t.EndLocation; 
				Expect(1);
				Block(
#line  1018 "VBNET.ATG" 
out stmt);
				Expect(88);
				Expect(167);

#line  1019 "VBNET.ATG" 
				Point endLocation = t.EndLocation; 
				Expect(1);

#line  1021 "VBNET.ATG" 
				ConstructorDeclaration cd = new ConstructorDeclaration("New", m.Modifier, p, attributes); 
				cd.StartLocation = m.GetDeclarationLocation(startPos);
				cd.EndLocation   = constructorEndLocation;
				cd.Body = (BlockStatement)stmt;
				cd.Body.EndLocation   = endLocation;
				compilationUnit.AddChild(cd);
				
			} else SynErr(221);
			break;
		}
		case 100: {
			lexer.NextToken();

#line  1033 "VBNET.ATG" 
			m.Check(Modifier.VBMethods);
			string name = String.Empty;
			Point startPos = t.Location;
			MethodDeclaration methodDeclaration;ArrayList handlesClause = null;
			List<InterfaceImplementation> implementsClause = null;
			AttributeSection returnTypeAttributeSection = null;
			
			Identifier();

#line  1040 "VBNET.ATG" 
			name = t.val; 
			TypeParameterList(
#line  1041 "VBNET.ATG" 
templates);
			if (la.kind == 24) {
				lexer.NextToken();
				if (StartOf(4)) {
					FormalParameterList(
#line  1042 "VBNET.ATG" 
p);
				}
				Expect(25);
			}
			if (la.kind == 48) {
				lexer.NextToken();
				while (la.kind == 27) {
					AttributeSection(
#line  1043 "VBNET.ATG" 
out returnTypeAttributeSection);
				}
				TypeName(
#line  1043 "VBNET.ATG" 
out type);
			}

#line  1045 "VBNET.ATG" 
			if(type == null) {
			type = new TypeReference("System.Object");
			}
			
			if (la.kind == 105 || la.kind == 107) {
				if (la.kind == 107) {
					ImplementsClause(
#line  1051 "VBNET.ATG" 
out implementsClause);
				} else {
					HandlesClause(
#line  1053 "VBNET.ATG" 
out handlesClause);
				}
			}
			Expect(1);
			if (
#line  1059 "VBNET.ATG" 
IsMustOverride(m)) {

#line  1061 "VBNET.ATG" 
				methodDeclaration = new MethodDeclaration(name, m.Modifier,  type, p, attributes);
				methodDeclaration.StartLocation = m.GetDeclarationLocation(startPos);
				methodDeclaration.EndLocation   = t.EndLocation;
				
				methodDeclaration.HandlesClause = handlesClause;
				methodDeclaration.Templates     = templates;
				methodDeclaration.InterfaceImplementations = implementsClause;
				methodDeclaration.ReturnTypeAttributeSection = returnTypeAttributeSection;
				compilationUnit.AddChild(methodDeclaration);
				
			} else if (StartOf(13)) {

#line  1073 "VBNET.ATG" 
				methodDeclaration = new MethodDeclaration(name, m.Modifier,  type, p, attributes);
				methodDeclaration.StartLocation = m.GetDeclarationLocation(startPos);
				methodDeclaration.EndLocation   = t.EndLocation;
				
				methodDeclaration.Templates     = templates;
				methodDeclaration.HandlesClause = handlesClause;
				methodDeclaration.InterfaceImplementations = implementsClause;
				methodDeclaration.ReturnTypeAttributeSection = returnTypeAttributeSection;
				
				compilationUnit.AddChild(methodDeclaration);
				compilationUnit.BlockStart(methodDeclaration);
				
				Block(
#line  1085 "VBNET.ATG" 
out stmt);

#line  1087 "VBNET.ATG" 
				compilationUnit.BlockEnd();
				methodDeclaration.Body  = (BlockStatement)stmt;
				
				Expect(88);
				Expect(100);

#line  1092 "VBNET.ATG" 
				methodDeclaration.Body.StartLocation = methodDeclaration.EndLocation;
				methodDeclaration.Body.EndLocation   = t.EndLocation;
				
				Expect(1);
			} else SynErr(222);
			break;
		}
		case 78: {
			lexer.NextToken();

#line  1101 "VBNET.ATG" 
			m.Check(Modifier.VBExternalMethods);
			Point startPos = t.Location;
			CharsetModifier charsetModifer = CharsetModifier.None;
			string library = String.Empty;
			string alias = null;
			string name = String.Empty;
			
			if (StartOf(14)) {
				Charset(
#line  1108 "VBNET.ATG" 
out charsetModifer);
			}
			if (la.kind == 167) {
				lexer.NextToken();
				Identifier();

#line  1111 "VBNET.ATG" 
				name = t.val; 
				Expect(115);
				Expect(3);

#line  1112 "VBNET.ATG" 
				library = t.val.ToString(); 
				if (la.kind == 44) {
					lexer.NextToken();
					Expect(3);

#line  1113 "VBNET.ATG" 
					alias = t.val.ToString(); 
				}
				if (la.kind == 24) {
					lexer.NextToken();
					if (StartOf(4)) {
						FormalParameterList(
#line  1114 "VBNET.ATG" 
p);
					}
					Expect(25);
				}
				Expect(1);

#line  1117 "VBNET.ATG" 
				DeclareDeclaration declareDeclaration = new DeclareDeclaration(name, m.Modifier, null, p, attributes, library, alias, charsetModifer);
				declareDeclaration.StartLocation = m.GetDeclarationLocation(startPos);
				declareDeclaration.EndLocation   = t.EndLocation;
				compilationUnit.AddChild(declareDeclaration);
				
			} else if (la.kind == 100) {
				lexer.NextToken();
				Identifier();

#line  1124 "VBNET.ATG" 
				name = t.val; 
				Expect(115);
				Expect(3);

#line  1125 "VBNET.ATG" 
				library = t.val; 
				if (la.kind == 44) {
					lexer.NextToken();
					Expect(3);

#line  1126 "VBNET.ATG" 
					alias = t.val; 
				}
				if (la.kind == 24) {
					lexer.NextToken();
					if (StartOf(4)) {
						FormalParameterList(
#line  1127 "VBNET.ATG" 
p);
					}
					Expect(25);
				}
				if (la.kind == 48) {
					lexer.NextToken();
					TypeName(
#line  1128 "VBNET.ATG" 
out type);
				}
				Expect(1);

#line  1131 "VBNET.ATG" 
				DeclareDeclaration declareDeclaration = new DeclareDeclaration(name, m.Modifier, type, p, attributes, library, alias, charsetModifer);
				declareDeclaration.StartLocation = m.GetDeclarationLocation(startPos);
				declareDeclaration.EndLocation   = t.EndLocation;
				compilationUnit.AddChild(declareDeclaration);
				
			} else SynErr(223);
			break;
		}
		case 93: {
			lexer.NextToken();

#line  1141 "VBNET.ATG" 
			m.Check(Modifier.VBEvents);
			Point startPos = t.Location;
			EventDeclaration eventDeclaration;
			string name = String.Empty;
			List<InterfaceImplementation> implementsClause = null;
			
			Identifier();

#line  1147 "VBNET.ATG" 
			name= t.val; 
			if (la.kind == 48) {
				lexer.NextToken();
				TypeName(
#line  1149 "VBNET.ATG" 
out type);
			} else if (la.kind == 1 || la.kind == 24 || la.kind == 107) {
				if (la.kind == 24) {
					lexer.NextToken();
					if (StartOf(4)) {
						FormalParameterList(
#line  1151 "VBNET.ATG" 
p);
					}
					Expect(25);
				}
			} else SynErr(224);
			if (la.kind == 107) {
				ImplementsClause(
#line  1153 "VBNET.ATG" 
out implementsClause);
			}

#line  1155 "VBNET.ATG" 
			eventDeclaration = new EventDeclaration(type, m.Modifier, p, attributes, name, implementsClause);
			eventDeclaration.StartLocation = m.GetDeclarationLocation(startPos);
			eventDeclaration.EndLocation = t.EndLocation;
			compilationUnit.AddChild(eventDeclaration);
			
			Expect(1);
			break;
		}
		case 2: case 47: case 49: case 50: case 51: case 70: case 144: case 169: case 176: case 177: {

#line  1162 "VBNET.ATG" 
			Point startPos = t.Location; 

#line  1164 "VBNET.ATG" 
			m.Check(Modifier.Fields);
			FieldDeclaration fd = new FieldDeclaration(attributes, type, m.Modifier);
			fd.StartLocation = m.GetDeclarationLocation(startPos); 
			
			VariableDeclarator(
#line  1168 "VBNET.ATG" 
variableDeclarators);
			while (la.kind == 12) {
				lexer.NextToken();
				VariableDeclarator(
#line  1169 "VBNET.ATG" 
variableDeclarators);
			}
			Expect(1);

#line  1172 "VBNET.ATG" 
			fd.EndLocation = t.EndLocation;
			fd.Fields = variableDeclarators;
			compilationUnit.AddChild(fd);
			
			break;
		}
		case 71: {

#line  1177 "VBNET.ATG" 
			m.Check(Modifier.Fields); 
			lexer.NextToken();

#line  1178 "VBNET.ATG" 
			m.Add(Modifier.Const, t.Location);  

#line  1180 "VBNET.ATG" 
			FieldDeclaration fd = new FieldDeclaration(attributes, type, m.Modifier);
			fd.StartLocation = m.GetDeclarationLocation(t.Location);
			List<VariableDeclaration> constantDeclarators = new List<VariableDeclaration>();
			
			ConstantDeclarator(
#line  1184 "VBNET.ATG" 
constantDeclarators);
			while (la.kind == 12) {
				lexer.NextToken();
				ConstantDeclarator(
#line  1185 "VBNET.ATG" 
constantDeclarators);
			}

#line  1187 "VBNET.ATG" 
			fd.Fields = constantDeclarators;
			fd.EndLocation = t.Location;
			
			Expect(1);

#line  1192 "VBNET.ATG" 
			fd.EndLocation = t.EndLocation;
			compilationUnit.AddChild(fd);
			
			break;
		}
		case 146: {
			lexer.NextToken();

#line  1198 "VBNET.ATG" 
			m.Check(Modifier.VBProperties);
			Point startPos = t.Location;
			List<InterfaceImplementation> implementsClause = null;
			
			Identifier();

#line  1202 "VBNET.ATG" 
			string propertyName = t.val; 
			if (la.kind == 24) {
				lexer.NextToken();
				if (StartOf(4)) {
					FormalParameterList(
#line  1203 "VBNET.ATG" 
p);
				}
				Expect(25);
			}
			if (la.kind == 48) {
				lexer.NextToken();
				TypeName(
#line  1204 "VBNET.ATG" 
out type);
			}

#line  1206 "VBNET.ATG" 
			if(type == null) {
			type = new TypeReference("System.Object");
			}
			
			if (la.kind == 107) {
				ImplementsClause(
#line  1210 "VBNET.ATG" 
out implementsClause);
			}
			Expect(1);
			if (
#line  1214 "VBNET.ATG" 
IsMustOverride(m)) {

#line  1216 "VBNET.ATG" 
				PropertyDeclaration pDecl = new PropertyDeclaration(propertyName, type, m.Modifier, attributes);
				pDecl.StartLocation = m.GetDeclarationLocation(startPos);
				pDecl.EndLocation   = t.Location;
				pDecl.TypeReference = type;
				pDecl.InterfaceImplementations = implementsClause;
				pDecl.Parameters = p;
				compilationUnit.AddChild(pDecl);
				
			} else if (la.kind == 27 || la.kind == 101 || la.kind == 156) {

#line  1226 "VBNET.ATG" 
				PropertyDeclaration pDecl = new PropertyDeclaration(propertyName, type, m.Modifier, attributes);
				pDecl.StartLocation = m.GetDeclarationLocation(startPos);
				pDecl.EndLocation   = t.Location;
				pDecl.BodyStart   = t.Location;
				pDecl.TypeReference = type;
				pDecl.InterfaceImplementations = implementsClause;
				pDecl.Parameters = p;
				PropertyGetRegion getRegion;
				PropertySetRegion setRegion;
				
				AccessorDecls(
#line  1236 "VBNET.ATG" 
out getRegion, out setRegion);
				Expect(88);
				Expect(146);
				Expect(1);

#line  1240 "VBNET.ATG" 
				pDecl.GetRegion = getRegion;
				pDecl.SetRegion = setRegion;
				pDecl.BodyEnd = t.EndLocation;
				compilationUnit.AddChild(pDecl);
				
			} else SynErr(225);
			break;
		}
		case 204: {
			lexer.NextToken();

#line  1247 "VBNET.ATG" 
			Point startPos = t.Location; 
			Expect(93);

#line  1249 "VBNET.ATG" 
			m.Check(Modifier.VBCustomEvents);
			EventAddRemoveRegion eventAccessorDeclaration;
			EventAddRegion addHandlerAccessorDeclaration = null;
			EventRemoveRegion removeHandlerAccessorDeclaration = null;
			EventRaiseRegion raiseEventAccessorDeclaration = null;
			List<InterfaceImplementation> implementsClause = null;
			
			Identifier();

#line  1256 "VBNET.ATG" 
			string customEventName = t.val; 
			Expect(48);
			TypeName(
#line  1257 "VBNET.ATG" 
out type);
			if (la.kind == 107) {
				ImplementsClause(
#line  1258 "VBNET.ATG" 
out implementsClause);
			}
			Expect(1);
			while (StartOf(15)) {
				EventAccessorDeclaration(
#line  1261 "VBNET.ATG" 
out eventAccessorDeclaration);

#line  1263 "VBNET.ATG" 
				if(eventAccessorDeclaration is EventAddRegion)
				{
					addHandlerAccessorDeclaration = (EventAddRegion)eventAccessorDeclaration;
				}
				else if(eventAccessorDeclaration is EventRemoveRegion)
				{
					removeHandlerAccessorDeclaration = (EventRemoveRegion)eventAccessorDeclaration;
				}
				else if(eventAccessorDeclaration is EventRaiseRegion)
				{
					raiseEventAccessorDeclaration = (EventRaiseRegion)eventAccessorDeclaration;
				}
				
			}
			Expect(88);
			Expect(93);
			Expect(1);

#line  1279 "VBNET.ATG" 
			if(addHandlerAccessorDeclaration == null)
			{
				Error("Need to provide AddHandler accessor.");
			}
			
			if(removeHandlerAccessorDeclaration == null)
			{
				Error("Need to provide RemoveHandler accessor.");
			}
			
			if(raiseEventAccessorDeclaration == null)
			{
				Error("Need to provide RaiseEvent accessor.");
			}
			
			EventDeclaration decl = new EventDeclaration(type, customEventName, m.Modifier, attributes);
			decl.StartLocation = m.GetDeclarationLocation(startPos);
			decl.EndLocation = t.EndLocation;
			decl.AddRegion = addHandlerAccessorDeclaration;
			decl.RemoveRegion = removeHandlerAccessorDeclaration;
			decl.RaiseRegion = raiseEventAccessorDeclaration;
			compilationUnit.AddChild(decl);
			
			break;
		}
		case 187: {
			lexer.NextToken();

#line  1305 "VBNET.ATG" 
			m.Check(Modifier.VBOperators);
			Point startPos = t.Location;
			TypeReference returnType = NullTypeReference.Instance;
			TypeReference operandType = NullTypeReference.Instance;
			string operandName;
			OverloadableOperatorType operatorType;
			AttributeSection section;
			List<ParameterDeclarationExpression> parameters = new List<ParameterDeclarationExpression>();
			List<AttributeSection> returnTypeAttributes = new List<AttributeSection>();
			
			OverloadableOperator(
#line  1315 "VBNET.ATG" 
out operatorType);
			Expect(24);
			if (la.kind == 55) {
				lexer.NextToken();
			}
			Identifier();

#line  1316 "VBNET.ATG" 
			operandName = t.val; 
			if (la.kind == 48) {
				lexer.NextToken();
				TypeName(
#line  1317 "VBNET.ATG" 
out operandType);
			}

#line  1318 "VBNET.ATG" 
			parameters.Add(new ParameterDeclarationExpression(operandType, operandName, ParamModifier.In)); 
			while (la.kind == 12) {
				lexer.NextToken();
				if (la.kind == 55) {
					lexer.NextToken();
				}
				Identifier();

#line  1322 "VBNET.ATG" 
				operandName = t.val; 
				if (la.kind == 48) {
					lexer.NextToken();
					TypeName(
#line  1323 "VBNET.ATG" 
out operandType);
				}

#line  1324 "VBNET.ATG" 
				parameters.Add(new ParameterDeclarationExpression(operandType, operandName, ParamModifier.In)); 
			}
			Expect(25);

#line  1327 "VBNET.ATG" 
			Point endPos = t.EndLocation; 
			if (la.kind == 48) {
				lexer.NextToken();
				while (la.kind == 27) {
					AttributeSection(
#line  1328 "VBNET.ATG" 
out section);

#line  1328 "VBNET.ATG" 
					returnTypeAttributes.Add(section); 
				}
				TypeName(
#line  1328 "VBNET.ATG" 
out returnType);

#line  1328 "VBNET.ATG" 
				endPos = t.EndLocation; 
				Expect(1);
			}
			Block(
#line  1329 "VBNET.ATG" 
out stmt);
			Expect(88);
			Expect(187);
			Expect(1);

#line  1331 "VBNET.ATG" 
			OperatorDeclaration operatorDeclaration = new OperatorDeclaration(m.Modifier, 
			                                                                 attributes, 
			                                                                 parameters, 
			                                                                 returnType,
			                                                                 operatorType
			                                                                 );
			operatorDeclaration.ConvertToType = returnType;
			operatorDeclaration.ReturnTypeAttributes = returnTypeAttributes;
			operatorDeclaration.Body = (BlockStatement)stmt;
			operatorDeclaration.StartLocation = m.GetDeclarationLocation(startPos);
			operatorDeclaration.EndLocation = endPos;
			operatorDeclaration.Body.StartLocation = startPos;
			operatorDeclaration.Body.EndLocation = t.Location;
			compilationUnit.AddChild(operatorDeclaration);
			
			break;
		}
		default: SynErr(226); break;
		}
	}

	void EnumMemberDecl(
#line  909 "VBNET.ATG" 
out FieldDeclaration f) {

#line  911 "VBNET.ATG" 
		Expression expr = null;List<AttributeSection> attributes = new List<AttributeSection>();
		AttributeSection section = null;
		VariableDeclaration varDecl = null;
		
		while (la.kind == 27) {
			AttributeSection(
#line  915 "VBNET.ATG" 
out section);

#line  915 "VBNET.ATG" 
			attributes.Add(section); 
		}
		Identifier();

#line  918 "VBNET.ATG" 
		f = new FieldDeclaration(attributes);
		varDecl = new VariableDeclaration(t.val);
		f.Fields.Add(varDecl);
		f.StartLocation = t.Location;
		
		if (la.kind == 11) {
			lexer.NextToken();
			Expr(
#line  923 "VBNET.ATG" 
out expr);

#line  923 "VBNET.ATG" 
			varDecl.Initializer = expr; 
		}
		Expect(1);
	}

	void InterfaceMemberDecl() {

#line  829 "VBNET.ATG" 
		TypeReference type =null;
		List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
		List<TemplateDefinition> templates = new List<TemplateDefinition>();
		AttributeSection section, returnTypeAttributeSection = null;
		Modifiers mod = new Modifiers();
		List<AttributeSection> attributes = new List<AttributeSection>();
		string name;
		
		if (StartOf(16)) {
			while (la.kind == 27) {
				AttributeSection(
#line  837 "VBNET.ATG" 
out section);

#line  837 "VBNET.ATG" 
				attributes.Add(section); 
			}
			while (StartOf(7)) {
				MemberModifier(
#line  840 "VBNET.ATG" 
mod);
			}
			if (la.kind == 93) {
				lexer.NextToken();

#line  843 "VBNET.ATG" 
				mod.Check(Modifier.VBInterfaceEvents); 
				Identifier();

#line  844 "VBNET.ATG" 
				name = t.val; 
				if (la.kind == 24) {
					lexer.NextToken();
					if (StartOf(4)) {
						FormalParameterList(
#line  845 "VBNET.ATG" 
p);
					}
					Expect(25);
				}
				if (la.kind == 48) {
					lexer.NextToken();
					TypeName(
#line  846 "VBNET.ATG" 
out type);
				}
				Expect(1);

#line  849 "VBNET.ATG" 
				EventDeclaration ed = new EventDeclaration(type, mod.Modifier, p, attributes, name, null);
				compilationUnit.AddChild(ed);
				ed.EndLocation = t.EndLocation;
				
			} else if (la.kind == 167) {
				lexer.NextToken();

#line  855 "VBNET.ATG" 
				mod.Check(Modifier.VBInterfaceMethods); 
				Identifier();

#line  856 "VBNET.ATG" 
				name = t.val; 
				TypeParameterList(
#line  857 "VBNET.ATG" 
templates);
				if (la.kind == 24) {
					lexer.NextToken();
					if (StartOf(4)) {
						FormalParameterList(
#line  858 "VBNET.ATG" 
p);
					}
					Expect(25);
				}
				Expect(1);

#line  861 "VBNET.ATG" 
				MethodDeclaration md = new MethodDeclaration(name, mod.Modifier, null, p, attributes);
				md.TypeReference = new TypeReference("", "System.Void");
				md.EndLocation = t.EndLocation;
				md.Templates = templates;
				compilationUnit.AddChild(md);
				
			} else if (la.kind == 100) {
				lexer.NextToken();

#line  869 "VBNET.ATG" 
				mod.Check(Modifier.VBInterfaceMethods); 
				Identifier();

#line  870 "VBNET.ATG" 
				name = t.val; 
				TypeParameterList(
#line  871 "VBNET.ATG" 
templates);
				if (la.kind == 24) {
					lexer.NextToken();
					if (StartOf(4)) {
						FormalParameterList(
#line  872 "VBNET.ATG" 
p);
					}
					Expect(25);
				}
				if (la.kind == 48) {
					lexer.NextToken();
					while (la.kind == 27) {
						AttributeSection(
#line  873 "VBNET.ATG" 
out returnTypeAttributeSection);
					}
					TypeName(
#line  873 "VBNET.ATG" 
out type);
				}

#line  875 "VBNET.ATG" 
				if(type == null) {
				type = new TypeReference("System.Object");
				}
				MethodDeclaration md = new MethodDeclaration(name, mod.Modifier, type, p, attributes);
				md.ReturnTypeAttributeSection = returnTypeAttributeSection;
				md.EndLocation = t.EndLocation;
				md.Templates = templates;
				compilationUnit.AddChild(md);
				
				Expect(1);
			} else if (la.kind == 146) {
				lexer.NextToken();

#line  887 "VBNET.ATG" 
				mod.Check(Modifier.VBInterfaceProperties); 
				Identifier();

#line  888 "VBNET.ATG" 
				name = t.val;  
				if (la.kind == 24) {
					lexer.NextToken();
					if (StartOf(4)) {
						FormalParameterList(
#line  889 "VBNET.ATG" 
p);
					}
					Expect(25);
				}
				if (la.kind == 48) {
					lexer.NextToken();
					TypeName(
#line  890 "VBNET.ATG" 
out type);
				}

#line  892 "VBNET.ATG" 
				if(type == null) {
				type = new TypeReference("System.Object");
				}
				
				Expect(1);

#line  898 "VBNET.ATG" 
				PropertyDeclaration pd = new PropertyDeclaration(name, type, mod.Modifier, attributes);
				pd.Parameters = p;
				pd.EndLocation = t.EndLocation;
				compilationUnit.AddChild(pd);
				
			} else SynErr(227);
		} else if (StartOf(17)) {
			NonModuleDeclaration(
#line  905 "VBNET.ATG" 
mod, attributes);
		} else SynErr(228);
	}

	void Expr(
#line  1695 "VBNET.ATG" 
out Expression expr) {
		ConditionalOrExpr(
#line  1697 "VBNET.ATG" 
out expr);
	}

	void ImplementsClause(
#line  1666 "VBNET.ATG" 
out List<InterfaceImplementation> baseInterfaces) {

#line  1668 "VBNET.ATG" 
		baseInterfaces = new List<InterfaceImplementation>();
		TypeReference type = null;
		string memberName = null;
		
		Expect(107);
		NonArrayTypeName(
#line  1673 "VBNET.ATG" 
out type, false);

#line  1674 "VBNET.ATG" 
		if (type != null) memberName = TypeReference.StripLastIdentifierFromType(ref type); 

#line  1675 "VBNET.ATG" 
		baseInterfaces.Add(new InterfaceImplementation(type, memberName)); 
		while (la.kind == 12) {
			lexer.NextToken();
			NonArrayTypeName(
#line  1677 "VBNET.ATG" 
out type, false);

#line  1678 "VBNET.ATG" 
			if (type != null) memberName = TypeReference.StripLastIdentifierFromType(ref type); 

#line  1679 "VBNET.ATG" 
			baseInterfaces.Add(new InterfaceImplementation(type, memberName)); 
		}
	}

	void HandlesClause(
#line  1624 "VBNET.ATG" 
out ArrayList handlesClause) {

#line  1626 "VBNET.ATG" 
		handlesClause = new ArrayList();
		string name;
		
		Expect(105);
		EventMemberSpecifier(
#line  1629 "VBNET.ATG" 
out name);

#line  1629 "VBNET.ATG" 
		handlesClause.Add(name); 
		while (la.kind == 12) {
			lexer.NextToken();
			EventMemberSpecifier(
#line  1630 "VBNET.ATG" 
out name);

#line  1630 "VBNET.ATG" 
			handlesClause.Add(name); 
		}
	}

	void Block(
#line  2296 "VBNET.ATG" 
out Statement stmt) {

#line  2299 "VBNET.ATG" 
		BlockStatement blockStmt = new BlockStatement();
		blockStmt.StartLocation = t.Location;
		compilationUnit.BlockStart(blockStmt);
		
		while (StartOf(18) || 
#line  2304 "VBNET.ATG" 
IsEndStmtAhead()) {
			if (
#line  2304 "VBNET.ATG" 
IsEndStmtAhead()) {
				Expect(88);
				EndOfStmt();

#line  2304 "VBNET.ATG" 
				compilationUnit.AddChild(new EndStatement()); 
			} else {
				Statement();
				EndOfStmt();
			}
		}

#line  2309 "VBNET.ATG" 
		stmt = blockStmt;
		blockStmt.EndLocation = t.EndLocation;
		compilationUnit.BlockEnd();
		
	}

	void Charset(
#line  1616 "VBNET.ATG" 
out CharsetModifier charsetModifier) {

#line  1617 "VBNET.ATG" 
		charsetModifier = CharsetModifier.None; 
		if (la.kind == 100 || la.kind == 167) {
		} else if (la.kind == 47) {
			lexer.NextToken();

#line  1618 "VBNET.ATG" 
			charsetModifier = CharsetModifier.ANSI; 
		} else if (la.kind == 50) {
			lexer.NextToken();

#line  1619 "VBNET.ATG" 
			charsetModifier = CharsetModifier.Auto; 
		} else if (la.kind == 176) {
			lexer.NextToken();

#line  1620 "VBNET.ATG" 
			charsetModifier = CharsetModifier.Unicode; 
		} else SynErr(229);
	}

	void VariableDeclarator(
#line  1518 "VBNET.ATG" 
List<VariableDeclaration> fieldDeclaration) {

#line  1520 "VBNET.ATG" 
		Expression expr = null;
		TypeReference type = null;
		ArrayList rank = null;
		List<Expression> dimension = null;
		
		Identifier();

#line  1525 "VBNET.ATG" 
		string name = t.val; 
		if (
#line  1526 "VBNET.ATG" 
IsSize() && !IsDims()) {
			ArrayInitializationModifier(
#line  1526 "VBNET.ATG" 
out dimension);
		}
		if (
#line  1527 "VBNET.ATG" 
IsDims()) {
			ArrayNameModifier(
#line  1527 "VBNET.ATG" 
out rank);
		}
		if (
#line  1529 "VBNET.ATG" 
IsObjectCreation()) {
			Expect(48);
			ObjectCreateExpression(
#line  1529 "VBNET.ATG" 
out expr);

#line  1531 "VBNET.ATG" 
			if (expr is ObjectCreateExpression) {
			type = ((ObjectCreateExpression)expr).CreateType;
			} else {
				type = ((ArrayCreateExpression)expr).CreateType;
			}
			
		} else if (StartOf(19)) {
			if (la.kind == 48) {
				lexer.NextToken();
				TypeName(
#line  1538 "VBNET.ATG" 
out type);
			}

#line  1540 "VBNET.ATG" 
			if (type != null && dimension != null) {
			if(type.RankSpecifier != null) {
				Error("array rank only allowed one time");
			} else {
				for (int i = 0; i < dimension.Count; i++)
					dimension[i] = Expression.AddInteger(dimension[i], 1);
				if (rank == null) {
					type.RankSpecifier = new int[] { dimension.Count - 1 };
				} else {
					rank.Insert(0, dimension.Count - 1);
					type.RankSpecifier = (int[])rank.ToArray(typeof(int));
				}
				expr = new ArrayCreateExpression(type, dimension);
			}
			} else if (type != null && rank != null) {
				if(type.RankSpecifier != null) {
					Error("array rank only allowed one time");
				} else {
					type.RankSpecifier = (int[])rank.ToArray(typeof(int));
				}
			}
			
			if (la.kind == 11) {
				lexer.NextToken();
				VariableInitializer(
#line  1562 "VBNET.ATG" 
out expr);
			}
		} else SynErr(230);

#line  1564 "VBNET.ATG" 
		fieldDeclaration.Add(new VariableDeclaration(name, expr, type)); 
	}

	void ConstantDeclarator(
#line  1501 "VBNET.ATG" 
List<VariableDeclaration> constantDeclaration) {

#line  1503 "VBNET.ATG" 
		Expression expr = null;
		TypeReference type = null;
		string name = String.Empty;
		
		Identifier();

#line  1507 "VBNET.ATG" 
		name = t.val; 
		if (la.kind == 48) {
			lexer.NextToken();
			TypeName(
#line  1508 "VBNET.ATG" 
out type);
		}
		Expect(11);
		Expr(
#line  1509 "VBNET.ATG" 
out expr);

#line  1511 "VBNET.ATG" 
		VariableDeclaration f = new VariableDeclaration(name, expr);
		f.TypeReference = type;
		constantDeclaration.Add(f);
		
	}

	void AccessorDecls(
#line  1443 "VBNET.ATG" 
out PropertyGetRegion getBlock, out PropertySetRegion setBlock) {

#line  1445 "VBNET.ATG" 
		List<AttributeSection> attributes = new List<AttributeSection>();
		AttributeSection section;
		getBlock = null;
		setBlock = null; 
		
		while (la.kind == 27) {
			AttributeSection(
#line  1450 "VBNET.ATG" 
out section);

#line  1450 "VBNET.ATG" 
			attributes.Add(section); 
		}
		if (la.kind == 101) {
			GetAccessorDecl(
#line  1452 "VBNET.ATG" 
out getBlock, attributes);
			if (la.kind == 27 || la.kind == 156) {

#line  1454 "VBNET.ATG" 
				attributes = new List<AttributeSection>(); 
				while (la.kind == 27) {
					AttributeSection(
#line  1455 "VBNET.ATG" 
out section);

#line  1455 "VBNET.ATG" 
					attributes.Add(section); 
				}
				SetAccessorDecl(
#line  1456 "VBNET.ATG" 
out setBlock, attributes);
			}
		} else if (la.kind == 156) {
			SetAccessorDecl(
#line  1459 "VBNET.ATG" 
out setBlock, attributes);
			if (la.kind == 27 || la.kind == 101) {

#line  1461 "VBNET.ATG" 
				attributes = new List<AttributeSection>(); 
				while (la.kind == 27) {
					AttributeSection(
#line  1462 "VBNET.ATG" 
out section);

#line  1462 "VBNET.ATG" 
					attributes.Add(section); 
				}
				GetAccessorDecl(
#line  1463 "VBNET.ATG" 
out getBlock, attributes);
			}
		} else SynErr(231);
	}

	void EventAccessorDeclaration(
#line  1406 "VBNET.ATG" 
out EventAddRemoveRegion eventAccessorDeclaration) {

#line  1408 "VBNET.ATG" 
		Statement stmt = null;
		List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		eventAccessorDeclaration = null;
		
		while (la.kind == 27) {
			AttributeSection(
#line  1414 "VBNET.ATG" 
out section);

#line  1414 "VBNET.ATG" 
			attributes.Add(section); 
		}
		if (la.kind == 42) {
			lexer.NextToken();
			if (la.kind == 24) {
				lexer.NextToken();
				if (StartOf(4)) {
					FormalParameterList(
#line  1416 "VBNET.ATG" 
p);
				}
				Expect(25);
			}
			Expect(1);
			Block(
#line  1417 "VBNET.ATG" 
out stmt);
			Expect(88);
			Expect(42);
			Expect(1);

#line  1419 "VBNET.ATG" 
			eventAccessorDeclaration = new EventAddRegion(attributes);
			eventAccessorDeclaration.Block = (BlockStatement)stmt;
			eventAccessorDeclaration.Parameters = p;
			
		} else if (la.kind == 152) {
			lexer.NextToken();
			if (la.kind == 24) {
				lexer.NextToken();
				if (StartOf(4)) {
					FormalParameterList(
#line  1424 "VBNET.ATG" 
p);
				}
				Expect(25);
			}
			Expect(1);
			Block(
#line  1425 "VBNET.ATG" 
out stmt);
			Expect(88);
			Expect(152);
			Expect(1);

#line  1427 "VBNET.ATG" 
			eventAccessorDeclaration = new EventRemoveRegion(attributes);
			eventAccessorDeclaration.Block = (BlockStatement)stmt;
			eventAccessorDeclaration.Parameters = p;
			
		} else if (la.kind == 149) {
			lexer.NextToken();
			if (la.kind == 24) {
				lexer.NextToken();
				if (StartOf(4)) {
					FormalParameterList(
#line  1432 "VBNET.ATG" 
p);
				}
				Expect(25);
			}
			Expect(1);
			Block(
#line  1433 "VBNET.ATG" 
out stmt);
			Expect(88);
			Expect(149);
			Expect(1);

#line  1435 "VBNET.ATG" 
			eventAccessorDeclaration = new EventRaiseRegion(attributes);
			eventAccessorDeclaration.Block = (BlockStatement)stmt;
			eventAccessorDeclaration.Parameters = p;
			
		} else SynErr(232);
	}

	void OverloadableOperator(
#line  1348 "VBNET.ATG" 
out OverloadableOperatorType operatorType) {

#line  1349 "VBNET.ATG" 
		operatorType = OverloadableOperatorType.None; 
		switch (la.kind) {
		case 14: {
			lexer.NextToken();

#line  1351 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Add; 
			break;
		}
		case 15: {
			lexer.NextToken();

#line  1353 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Subtract; 
			break;
		}
		case 16: {
			lexer.NextToken();

#line  1355 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Multiply; 
			break;
		}
		case 17: {
			lexer.NextToken();

#line  1357 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Divide; 
			break;
		}
		case 18: {
			lexer.NextToken();

#line  1359 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.DivideInteger; 
			break;
		}
		case 19: {
			lexer.NextToken();

#line  1361 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Concat; 
			break;
		}
		case 116: {
			lexer.NextToken();

#line  1363 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Like; 
			break;
		}
		case 120: {
			lexer.NextToken();

#line  1365 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Modulus; 
			break;
		}
		case 45: {
			lexer.NextToken();

#line  1367 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.BitwiseAnd; 
			break;
		}
		case 138: {
			lexer.NextToken();

#line  1369 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.BitwiseOr; 
			break;
		}
		case 185: {
			lexer.NextToken();

#line  1371 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.ExclusiveOr; 
			break;
		}
		case 20: {
			lexer.NextToken();

#line  1373 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Power; 
			break;
		}
		case 31: {
			lexer.NextToken();

#line  1375 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.ShiftLeft; 
			break;
		}
		case 32: {
			lexer.NextToken();

#line  1377 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.ShiftRight; 
			break;
		}
		case 11: {
			lexer.NextToken();

#line  1379 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Equality; 
			break;
		}
		case 28: {
			lexer.NextToken();

#line  1381 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.InEquality; 
			break;
		}
		case 27: {
			lexer.NextToken();

#line  1383 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.LessThan; 
			break;
		}
		case 30: {
			lexer.NextToken();

#line  1385 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.LessThanOrEqual; 
			break;
		}
		case 26: {
			lexer.NextToken();

#line  1387 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.GreaterThan; 
			break;
		}
		case 29: {
			lexer.NextToken();

#line  1389 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.GreaterThanOrEqual; 
			break;
		}
		case 75: {
			lexer.NextToken();

#line  1391 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.CType; 
			break;
		}
		case 2: case 47: case 49: case 50: case 51: case 70: case 144: case 169: case 176: case 177: {
			Identifier();

#line  1395 "VBNET.ATG" 
			string opName = t.val; 
			if (string.Equals(opName, "istrue", StringComparison.InvariantCultureIgnoreCase)) {
				operatorType = OverloadableOperatorType.IsTrue;
			} else if (string.Equals(opName, "isfalse", StringComparison.InvariantCultureIgnoreCase)) {
				operatorType = OverloadableOperatorType.IsFalse;
			} else {
				Error("Invalid operator. Possible operators are '+', '-', 'Not', 'IsTrue', 'IsFalse'.");
			}
			
			break;
		}
		default: SynErr(233); break;
		}
	}

	void GetAccessorDecl(
#line  1469 "VBNET.ATG" 
out PropertyGetRegion getBlock, List<AttributeSection> attributes) {

#line  1470 "VBNET.ATG" 
		Statement stmt = null; 
		Expect(101);

#line  1472 "VBNET.ATG" 
		Point startLocation = t.Location; 
		Expect(1);
		Block(
#line  1474 "VBNET.ATG" 
out stmt);

#line  1475 "VBNET.ATG" 
		getBlock = new PropertyGetRegion((BlockStatement)stmt, attributes); 
		Expect(88);
		Expect(101);

#line  1477 "VBNET.ATG" 
		getBlock.StartLocation = startLocation; getBlock.EndLocation = t.EndLocation; 
		Expect(1);
	}

	void SetAccessorDecl(
#line  1482 "VBNET.ATG" 
out PropertySetRegion setBlock, List<AttributeSection> attributes) {

#line  1484 "VBNET.ATG" 
		Statement stmt = null; List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
		
		Expect(156);

#line  1487 "VBNET.ATG" 
		Point startLocation = t.Location; 
		if (la.kind == 24) {
			lexer.NextToken();
			if (StartOf(4)) {
				FormalParameterList(
#line  1488 "VBNET.ATG" 
p);
			}
			Expect(25);
		}
		Expect(1);
		Block(
#line  1490 "VBNET.ATG" 
out stmt);

#line  1492 "VBNET.ATG" 
		setBlock = new PropertySetRegion((BlockStatement)stmt, attributes);
		setBlock.Parameters = p;
		
		Expect(88);
		Expect(156);

#line  1496 "VBNET.ATG" 
		setBlock.StartLocation = startLocation; setBlock.EndLocation = t.EndLocation; 
		Expect(1);
	}

	void ArrayInitializationModifier(
#line  1568 "VBNET.ATG" 
out List<Expression> arrayModifiers) {

#line  1570 "VBNET.ATG" 
		arrayModifiers = null;
		
		Expect(24);
		InitializationRankList(
#line  1572 "VBNET.ATG" 
out arrayModifiers);
		Expect(25);
	}

	void ArrayNameModifier(
#line  2089 "VBNET.ATG" 
out ArrayList arrayModifiers) {

#line  2091 "VBNET.ATG" 
		arrayModifiers = null;
		
		ArrayTypeModifiers(
#line  2093 "VBNET.ATG" 
out arrayModifiers);
	}

	void ObjectCreateExpression(
#line  1971 "VBNET.ATG" 
out Expression oce) {

#line  1973 "VBNET.ATG" 
		TypeReference type = null;
		Expression initializer = null;
		List<Expression> arguments = null;
		ArrayList dimensions = null;
		oce = null;
		
		Expect(127);
		NonArrayTypeName(
#line  1979 "VBNET.ATG" 
out type, false);
		if (la.kind == 24) {
			lexer.NextToken();
			if (StartOf(20)) {
				ArgumentList(
#line  1980 "VBNET.ATG" 
out arguments);
			}
			Expect(25);
		}
		if (la.kind == 22 || 
#line  1981 "VBNET.ATG" 
la.kind == Tokens.OpenParenthesis) {
			if (
#line  1981 "VBNET.ATG" 
la.kind == Tokens.OpenParenthesis) {
				ArrayTypeModifiers(
#line  1982 "VBNET.ATG" 
out dimensions);
				ArrayInitializer(
#line  1983 "VBNET.ATG" 
out initializer);
			} else {
				ArrayInitializer(
#line  1984 "VBNET.ATG" 
out initializer);
			}
		}

#line  1986 "VBNET.ATG" 
		if (initializer == null) {
		oce = new ObjectCreateExpression(type, arguments);
		} else {
			if (dimensions == null) dimensions = new ArrayList();
			dimensions.Insert(0, (arguments == null) ? 0 : Math.Max(arguments.Count - 1, 0));
			type.RankSpecifier = (int[])dimensions.ToArray(typeof(int));
			ArrayCreateExpression ace = new ArrayCreateExpression(type, initializer as ArrayInitializerExpression);
			ace.Arguments = arguments;
			oce = ace;
		}
		
	}

	void VariableInitializer(
#line  1588 "VBNET.ATG" 
out Expression initializerExpression) {

#line  1590 "VBNET.ATG" 
		initializerExpression = null;
		
		if (StartOf(21)) {
			Expr(
#line  1592 "VBNET.ATG" 
out initializerExpression);
		} else if (la.kind == 22) {
			ArrayInitializer(
#line  1593 "VBNET.ATG" 
out initializerExpression);
		} else SynErr(234);
	}

	void InitializationRankList(
#line  1576 "VBNET.ATG" 
out List<Expression> rank) {

#line  1578 "VBNET.ATG" 
		rank = null;
		Expression expr = null;
		
		Expr(
#line  1581 "VBNET.ATG" 
out expr);

#line  1581 "VBNET.ATG" 
		rank = new List<Expression>(); if (expr != null) { rank.Add(expr); } 
		while (la.kind == 12) {
			lexer.NextToken();
			Expr(
#line  1583 "VBNET.ATG" 
out expr);

#line  1583 "VBNET.ATG" 
			if (expr != null) { rank.Add(expr); } 
		}
	}

	void ArrayInitializer(
#line  1597 "VBNET.ATG" 
out Expression outExpr) {

#line  1599 "VBNET.ATG" 
		Expression expr = null;
		ArrayInitializerExpression initializer = new ArrayInitializerExpression();
		
		Expect(22);
		if (StartOf(22)) {
			VariableInitializer(
#line  1604 "VBNET.ATG" 
out expr);

#line  1606 "VBNET.ATG" 
			if (expr != null) { initializer.CreateExpressions.Add(expr); }
			
			while (
#line  1609 "VBNET.ATG" 
NotFinalComma()) {
				Expect(12);
				VariableInitializer(
#line  1609 "VBNET.ATG" 
out expr);

#line  1610 "VBNET.ATG" 
				if (expr != null) { initializer.CreateExpressions.Add(expr); } 
			}
		}
		Expect(23);

#line  1613 "VBNET.ATG" 
		outExpr = initializer; 
	}

	void EventMemberSpecifier(
#line  1683 "VBNET.ATG" 
out string name) {

#line  1684 "VBNET.ATG" 
		string type; name = String.Empty; 
		if (StartOf(12)) {
			Identifier();

#line  1685 "VBNET.ATG" 
			type = t.val; 
			Expect(10);
			Identifier();

#line  1687 "VBNET.ATG" 
			name = type + "." + t.val; 
		} else if (la.kind == 124) {
			lexer.NextToken();
			Expect(10);
			if (StartOf(12)) {
				Identifier();

#line  1690 "VBNET.ATG" 
				name = "MyBase." + t.val; 
			} else if (la.kind == 92) {
				lexer.NextToken();

#line  1691 "VBNET.ATG" 
				name = "MyBase.Error"; 
			} else SynErr(235);
		} else SynErr(236);
	}

	void ConditionalOrExpr(
#line  1850 "VBNET.ATG" 
out Expression outExpr) {

#line  1851 "VBNET.ATG" 
		Expression expr; 
		ConditionalAndExpr(
#line  1852 "VBNET.ATG" 
out outExpr);
		while (la.kind == 139) {
			lexer.NextToken();
			ConditionalAndExpr(
#line  1852 "VBNET.ATG" 
out expr);

#line  1852 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.LogicalOr, expr);  
		}
	}

	void UnaryExpr(
#line  1700 "VBNET.ATG" 
out Expression uExpr) {

#line  1702 "VBNET.ATG" 
		Expression expr;
		UnaryOperatorType uop = UnaryOperatorType.None;
		bool isUOp = false;
		
		while (la.kind == 14 || la.kind == 15 || la.kind == 16) {
			if (la.kind == 14) {
				lexer.NextToken();

#line  1706 "VBNET.ATG" 
				uop = UnaryOperatorType.Plus; isUOp = true; 
			} else if (la.kind == 15) {
				lexer.NextToken();

#line  1707 "VBNET.ATG" 
				uop = UnaryOperatorType.Minus; isUOp = true; 
			} else {
				lexer.NextToken();

#line  1709 "VBNET.ATG" 
				uop = UnaryOperatorType.Star;  isUOp = true;
			}
		}
		SimpleExpr(
#line  1711 "VBNET.ATG" 
out expr);

#line  1713 "VBNET.ATG" 
		if (isUOp) {
		uExpr = new UnaryOperatorExpression(expr, uop);
		} else {
			uExpr = expr;
		}
		
	}

	void SimpleExpr(
#line  1736 "VBNET.ATG" 
out Expression pexpr) {

#line  1738 "VBNET.ATG" 
		Expression expr;
		TypeReference type = null;
		string name = String.Empty;
		pexpr = null;
		
		if (StartOf(23)) {
			switch (la.kind) {
			case 3: {
				lexer.NextToken();

#line  1746 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(t.literalValue, t.val);  
				break;
			}
			case 4: {
				lexer.NextToken();

#line  1747 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(t.literalValue, t.val);  
				break;
			}
			case 7: {
				lexer.NextToken();

#line  1748 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(t.literalValue, t.val);  
				break;
			}
			case 6: {
				lexer.NextToken();

#line  1749 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(t.literalValue, t.val);  
				break;
			}
			case 5: {
				lexer.NextToken();

#line  1750 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(t.literalValue, t.val);  
				break;
			}
			case 9: {
				lexer.NextToken();

#line  1751 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(t.literalValue, t.val);  
				break;
			}
			case 8: {
				lexer.NextToken();

#line  1752 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(t.literalValue, t.val);  
				break;
			}
			case 173: {
				lexer.NextToken();

#line  1754 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(true, "true");  
				break;
			}
			case 96: {
				lexer.NextToken();

#line  1755 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(false, "false"); 
				break;
			}
			case 130: {
				lexer.NextToken();

#line  1756 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(null, "null");  
				break;
			}
			case 24: {
				lexer.NextToken();
				Expr(
#line  1757 "VBNET.ATG" 
out expr);
				Expect(25);

#line  1757 "VBNET.ATG" 
				pexpr = new ParenthesizedExpression(expr); 
				break;
			}
			case 2: case 47: case 49: case 50: case 51: case 70: case 144: case 169: case 176: case 177: {
				Identifier();

#line  1758 "VBNET.ATG" 
				pexpr = new IdentifierExpression(t.val); 
				break;
			}
			case 52: case 54: case 65: case 76: case 77: case 84: case 111: case 117: case 159: case 160: case 165: case 190: case 191: case 192: case 193: {

#line  1759 "VBNET.ATG" 
				string val = String.Empty; 
				PrimitiveTypeName(
#line  1760 "VBNET.ATG" 
out val);
				Expect(10);

#line  1761 "VBNET.ATG" 
				t.val = ""; 
				Identifier();

#line  1761 "VBNET.ATG" 
				pexpr = new FieldReferenceExpression(new TypeReferenceExpression(val), t.val); 
				break;
			}
			case 119: {
				lexer.NextToken();

#line  1762 "VBNET.ATG" 
				pexpr = new ThisReferenceExpression(); 
				break;
			}
			case 124: case 125: {

#line  1763 "VBNET.ATG" 
				Expression retExpr = null; 
				if (la.kind == 124) {
					lexer.NextToken();

#line  1764 "VBNET.ATG" 
					retExpr = new BaseReferenceExpression(); 
				} else if (la.kind == 125) {
					lexer.NextToken();

#line  1765 "VBNET.ATG" 
					retExpr = new ClassReferenceExpression(); 
				} else SynErr(237);
				Expect(10);
				IdentifierOrKeyword(
#line  1767 "VBNET.ATG" 
out name);

#line  1767 "VBNET.ATG" 
				pexpr = new FieldReferenceExpression(retExpr, name); 
				break;
			}
			case 198: {
				lexer.NextToken();
				Expect(10);
				Identifier();

#line  1769 "VBNET.ATG" 
				type = new TypeReference(t.val ?? ""); 

#line  1771 "VBNET.ATG" 
				type.IsGlobal = true; 

#line  1772 "VBNET.ATG" 
				pexpr = new TypeReferenceExpression(type); 
				break;
			}
			case 127: {
				ObjectCreateExpression(
#line  1773 "VBNET.ATG" 
out expr);

#line  1773 "VBNET.ATG" 
				pexpr = expr; 
				break;
			}
			case 75: case 82: case 199: {

#line  1775 "VBNET.ATG" 
				CastType castType = CastType.Cast; 
				if (la.kind == 82) {
					lexer.NextToken();
				} else if (la.kind == 75) {
					lexer.NextToken();

#line  1777 "VBNET.ATG" 
					castType = CastType.Conversion; 
				} else if (la.kind == 199) {
					lexer.NextToken();

#line  1778 "VBNET.ATG" 
					castType = CastType.TryCast; 
				} else SynErr(238);
				Expect(24);
				Expr(
#line  1780 "VBNET.ATG" 
out expr);
				Expect(12);
				TypeName(
#line  1780 "VBNET.ATG" 
out type);
				Expect(25);

#line  1781 "VBNET.ATG" 
				pexpr = new CastExpression(type, expr, castType); 
				break;
			}
			case 59: case 60: case 61: case 62: case 63: case 64: case 66: case 68: case 69: case 72: case 73: case 74: case 194: case 195: case 196: case 197: {
				CastTarget(
#line  1782 "VBNET.ATG" 
out type);
				Expect(24);
				Expr(
#line  1782 "VBNET.ATG" 
out expr);
				Expect(25);

#line  1782 "VBNET.ATG" 
				pexpr = new CastExpression(type, expr, CastType.PrimitiveConversion); 
				break;
			}
			case 43: {
				lexer.NextToken();
				Expr(
#line  1783 "VBNET.ATG" 
out expr);

#line  1783 "VBNET.ATG" 
				pexpr = new AddressOfExpression(expr); 
				break;
			}
			case 102: {
				lexer.NextToken();
				Expect(24);
				GetTypeTypeName(
#line  1784 "VBNET.ATG" 
out type);
				Expect(25);

#line  1784 "VBNET.ATG" 
				pexpr = new TypeOfExpression(type); 
				break;
			}
			case 175: {
				lexer.NextToken();
				SimpleExpr(
#line  1785 "VBNET.ATG" 
out expr);
				Expect(113);
				TypeName(
#line  1785 "VBNET.ATG" 
out type);

#line  1785 "VBNET.ATG" 
				pexpr = new TypeOfIsExpression(expr, type); 
				break;
			}
			}
			while (la.kind == 10 || la.kind == 24) {
				InvocationOrMemberReferenceExpression(
#line  1787 "VBNET.ATG" 
ref pexpr);
			}
		} else if (la.kind == 10) {
			lexer.NextToken();
			IdentifierOrKeyword(
#line  1790 "VBNET.ATG" 
out name);

#line  1790 "VBNET.ATG" 
			pexpr = new FieldReferenceExpression(pexpr, name);
			while (la.kind == 10 || la.kind == 24) {
				InvocationOrMemberReferenceExpression(
#line  1791 "VBNET.ATG" 
ref pexpr);
			}
		} else SynErr(239);
	}

	void AssignmentOperator(
#line  1721 "VBNET.ATG" 
out AssignmentOperatorType op) {

#line  1722 "VBNET.ATG" 
		op = AssignmentOperatorType.None; 
		switch (la.kind) {
		case 11: {
			lexer.NextToken();

#line  1723 "VBNET.ATG" 
			op = AssignmentOperatorType.Assign; 
			break;
		}
		case 41: {
			lexer.NextToken();

#line  1724 "VBNET.ATG" 
			op = AssignmentOperatorType.ConcatString; 
			break;
		}
		case 33: {
			lexer.NextToken();

#line  1725 "VBNET.ATG" 
			op = AssignmentOperatorType.Add; 
			break;
		}
		case 35: {
			lexer.NextToken();

#line  1726 "VBNET.ATG" 
			op = AssignmentOperatorType.Subtract; 
			break;
		}
		case 36: {
			lexer.NextToken();

#line  1727 "VBNET.ATG" 
			op = AssignmentOperatorType.Multiply; 
			break;
		}
		case 37: {
			lexer.NextToken();

#line  1728 "VBNET.ATG" 
			op = AssignmentOperatorType.Divide; 
			break;
		}
		case 38: {
			lexer.NextToken();

#line  1729 "VBNET.ATG" 
			op = AssignmentOperatorType.DivideInteger; 
			break;
		}
		case 34: {
			lexer.NextToken();

#line  1730 "VBNET.ATG" 
			op = AssignmentOperatorType.Power; 
			break;
		}
		case 39: {
			lexer.NextToken();

#line  1731 "VBNET.ATG" 
			op = AssignmentOperatorType.ShiftLeft; 
			break;
		}
		case 40: {
			lexer.NextToken();

#line  1732 "VBNET.ATG" 
			op = AssignmentOperatorType.ShiftRight; 
			break;
		}
		default: SynErr(240); break;
		}
	}

	void PrimitiveTypeName(
#line  2896 "VBNET.ATG" 
out string type) {

#line  2897 "VBNET.ATG" 
		type = String.Empty; 
		switch (la.kind) {
		case 52: {
			lexer.NextToken();

#line  2898 "VBNET.ATG" 
			type = "Boolean"; 
			break;
		}
		case 76: {
			lexer.NextToken();

#line  2899 "VBNET.ATG" 
			type = "Date"; 
			break;
		}
		case 65: {
			lexer.NextToken();

#line  2900 "VBNET.ATG" 
			type = "Char"; 
			break;
		}
		case 165: {
			lexer.NextToken();

#line  2901 "VBNET.ATG" 
			type = "String"; 
			break;
		}
		case 77: {
			lexer.NextToken();

#line  2902 "VBNET.ATG" 
			type = "Decimal"; 
			break;
		}
		case 54: {
			lexer.NextToken();

#line  2903 "VBNET.ATG" 
			type = "Byte"; 
			break;
		}
		case 159: {
			lexer.NextToken();

#line  2904 "VBNET.ATG" 
			type = "Short"; 
			break;
		}
		case 111: {
			lexer.NextToken();

#line  2905 "VBNET.ATG" 
			type = "Integer"; 
			break;
		}
		case 117: {
			lexer.NextToken();

#line  2906 "VBNET.ATG" 
			type = "Long"; 
			break;
		}
		case 160: {
			lexer.NextToken();

#line  2907 "VBNET.ATG" 
			type = "Single"; 
			break;
		}
		case 84: {
			lexer.NextToken();

#line  2908 "VBNET.ATG" 
			type = "Double"; 
			break;
		}
		case 191: {
			lexer.NextToken();

#line  2909 "VBNET.ATG" 
			type = "UInteger"; 
			break;
		}
		case 192: {
			lexer.NextToken();

#line  2910 "VBNET.ATG" 
			type = "ULong"; 
			break;
		}
		case 193: {
			lexer.NextToken();

#line  2911 "VBNET.ATG" 
			type = "UShort"; 
			break;
		}
		case 190: {
			lexer.NextToken();

#line  2912 "VBNET.ATG" 
			type = "SByte"; 
			break;
		}
		default: SynErr(241); break;
		}
	}

	void IdentifierOrKeyword(
#line  2889 "VBNET.ATG" 
out string name) {

#line  2891 "VBNET.ATG" 
		lexer.NextToken(); name = t.val;  
	}

	void CastTarget(
#line  1828 "VBNET.ATG" 
out TypeReference type) {

#line  1830 "VBNET.ATG" 
		type = null;
		
		switch (la.kind) {
		case 59: {
			lexer.NextToken();

#line  1832 "VBNET.ATG" 
			type = new TypeReference("System.Boolean"); 
			break;
		}
		case 60: {
			lexer.NextToken();

#line  1833 "VBNET.ATG" 
			type = new TypeReference("System.Byte"); 
			break;
		}
		case 194: {
			lexer.NextToken();

#line  1834 "VBNET.ATG" 
			type = new TypeReference("System.SByte"); 
			break;
		}
		case 61: {
			lexer.NextToken();

#line  1835 "VBNET.ATG" 
			type = new TypeReference("System.Char"); 
			break;
		}
		case 62: {
			lexer.NextToken();

#line  1836 "VBNET.ATG" 
			type = new TypeReference("System.DateTime"); 
			break;
		}
		case 64: {
			lexer.NextToken();

#line  1837 "VBNET.ATG" 
			type = new TypeReference("System.Decimal"); 
			break;
		}
		case 63: {
			lexer.NextToken();

#line  1838 "VBNET.ATG" 
			type = new TypeReference("System.Double"); 
			break;
		}
		case 72: {
			lexer.NextToken();

#line  1839 "VBNET.ATG" 
			type = new TypeReference("System.Int16"); 
			break;
		}
		case 66: {
			lexer.NextToken();

#line  1840 "VBNET.ATG" 
			type = new TypeReference("System.Int32"); 
			break;
		}
		case 68: {
			lexer.NextToken();

#line  1841 "VBNET.ATG" 
			type = new TypeReference("System.Int64"); 
			break;
		}
		case 195: {
			lexer.NextToken();

#line  1842 "VBNET.ATG" 
			type = new TypeReference("System.UInt16"); 
			break;
		}
		case 196: {
			lexer.NextToken();

#line  1843 "VBNET.ATG" 
			type = new TypeReference("System.UInt32"); 
			break;
		}
		case 197: {
			lexer.NextToken();

#line  1844 "VBNET.ATG" 
			type = new TypeReference("System.UInt64"); 
			break;
		}
		case 69: {
			lexer.NextToken();

#line  1845 "VBNET.ATG" 
			type = new TypeReference("System.Object"); 
			break;
		}
		case 73: {
			lexer.NextToken();

#line  1846 "VBNET.ATG" 
			type = new TypeReference("System.Single"); 
			break;
		}
		case 74: {
			lexer.NextToken();

#line  1847 "VBNET.ATG" 
			type = new TypeReference("System.String"); 
			break;
		}
		default: SynErr(242); break;
		}
	}

	void GetTypeTypeName(
#line  2041 "VBNET.ATG" 
out TypeReference typeref) {

#line  2042 "VBNET.ATG" 
		ArrayList rank = null; 
		NonArrayTypeName(
#line  2044 "VBNET.ATG" 
out typeref, true);
		ArrayTypeModifiers(
#line  2045 "VBNET.ATG" 
out rank);

#line  2046 "VBNET.ATG" 
		if (rank != null && typeref != null) {
		typeref.RankSpecifier = (int[])rank.ToArray(typeof(int));
		}
		
	}

	void InvocationOrMemberReferenceExpression(
#line  1795 "VBNET.ATG" 
ref Expression pexpr) {

#line  1796 "VBNET.ATG" 
		string name; 
		if (la.kind == 10) {
			lexer.NextToken();
			IdentifierOrKeyword(
#line  1798 "VBNET.ATG" 
out name);

#line  1798 "VBNET.ATG" 
			pexpr = new FieldReferenceExpression(pexpr, name); 
		} else if (la.kind == 24) {
			InvocationExpression(
#line  1799 "VBNET.ATG" 
ref pexpr);
		} else SynErr(243);
	}

	void InvocationExpression(
#line  1802 "VBNET.ATG" 
ref Expression pexpr) {

#line  1803 "VBNET.ATG" 
		List<TypeReference> typeParameters = new List<TypeReference>();
		List<Expression> parameters = null;
		TypeReference type; 
		Expect(24);

#line  1807 "VBNET.ATG" 
		Point start = t.Location; 
		if (la.kind == 200) {
			lexer.NextToken();
			TypeName(
#line  1809 "VBNET.ATG" 
out type);

#line  1809 "VBNET.ATG" 
			if (type != null) typeParameters.Add(type); 
			Expect(25);
			if (la.kind == 10) {
				lexer.NextToken();
				Identifier();

#line  1813 "VBNET.ATG" 
				pexpr = new FieldReferenceExpression(GetTypeReferenceExpression(pexpr, typeParameters), t.val); 
			} else if (la.kind == 24) {
				lexer.NextToken();
				ArgumentList(
#line  1815 "VBNET.ATG" 
out parameters);
				Expect(25);

#line  1817 "VBNET.ATG" 
				pexpr = new InvocationExpression(pexpr, parameters, typeParameters); 
			} else SynErr(244);
		} else if (StartOf(20)) {
			ArgumentList(
#line  1819 "VBNET.ATG" 
out parameters);
			Expect(25);

#line  1821 "VBNET.ATG" 
			pexpr = new InvocationExpression(pexpr, parameters, typeParameters); 
		} else SynErr(245);

#line  1823 "VBNET.ATG" 
		pexpr.StartLocation = start; pexpr.EndLocation = t.Location; 
	}

	void ArgumentList(
#line  2000 "VBNET.ATG" 
out List<Expression> arguments) {

#line  2002 "VBNET.ATG" 
		arguments = new List<Expression>();
		Expression expr = null;
		
		if (StartOf(21)) {
			Argument(
#line  2006 "VBNET.ATG" 
out expr);

#line  2006 "VBNET.ATG" 
			if (expr != null) { arguments.Add(expr); } 
			while (la.kind == 12) {
				lexer.NextToken();
				Argument(
#line  2009 "VBNET.ATG" 
out expr);

#line  2009 "VBNET.ATG" 
				if (expr != null) { arguments.Add(expr); } 
			}
		}
	}

	void ConditionalAndExpr(
#line  1855 "VBNET.ATG" 
out Expression outExpr) {

#line  1856 "VBNET.ATG" 
		Expression expr; 
		InclusiveOrExpr(
#line  1857 "VBNET.ATG" 
out outExpr);
		while (la.kind == 46) {
			lexer.NextToken();
			InclusiveOrExpr(
#line  1857 "VBNET.ATG" 
out expr);

#line  1857 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.LogicalAnd, expr);  
		}
	}

	void InclusiveOrExpr(
#line  1860 "VBNET.ATG" 
out Expression outExpr) {

#line  1861 "VBNET.ATG" 
		Expression expr; 
		ExclusiveOrExpr(
#line  1862 "VBNET.ATG" 
out outExpr);
		while (la.kind == 185) {
			lexer.NextToken();
			ExclusiveOrExpr(
#line  1862 "VBNET.ATG" 
out expr);

#line  1862 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.ExclusiveOr, expr);  
		}
	}

	void ExclusiveOrExpr(
#line  1865 "VBNET.ATG" 
out Expression outExpr) {

#line  1866 "VBNET.ATG" 
		Expression expr; 
		AndExpr(
#line  1867 "VBNET.ATG" 
out outExpr);
		while (la.kind == 138) {
			lexer.NextToken();
			AndExpr(
#line  1867 "VBNET.ATG" 
out expr);

#line  1867 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.BitwiseOr, expr);  
		}
	}

	void AndExpr(
#line  1870 "VBNET.ATG" 
out Expression outExpr) {

#line  1871 "VBNET.ATG" 
		Expression expr; 
		NotExpr(
#line  1872 "VBNET.ATG" 
out outExpr);
		while (la.kind == 45) {
			lexer.NextToken();
			NotExpr(
#line  1872 "VBNET.ATG" 
out expr);

#line  1872 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.BitwiseAnd, expr);  
		}
	}

	void NotExpr(
#line  1875 "VBNET.ATG" 
out Expression outExpr) {

#line  1876 "VBNET.ATG" 
		UnaryOperatorType uop = UnaryOperatorType.None; 
		while (la.kind == 129) {
			lexer.NextToken();

#line  1877 "VBNET.ATG" 
			uop = UnaryOperatorType.Not; 
		}
		EqualityExpr(
#line  1878 "VBNET.ATG" 
out outExpr);

#line  1879 "VBNET.ATG" 
		if (uop != UnaryOperatorType.None)
		outExpr = new UnaryOperatorExpression(outExpr, uop);
		
	}

	void EqualityExpr(
#line  1884 "VBNET.ATG" 
out Expression outExpr) {

#line  1886 "VBNET.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		RelationalExpr(
#line  1889 "VBNET.ATG" 
out outExpr);
		while (la.kind == 11 || la.kind == 28 || la.kind == 116) {
			if (la.kind == 28) {
				lexer.NextToken();

#line  1892 "VBNET.ATG" 
				op = BinaryOperatorType.InEquality; 
			} else if (la.kind == 11) {
				lexer.NextToken();

#line  1893 "VBNET.ATG" 
				op = BinaryOperatorType.Equality; 
			} else {
				lexer.NextToken();

#line  1894 "VBNET.ATG" 
				op = BinaryOperatorType.Like; 
			}
			RelationalExpr(
#line  1896 "VBNET.ATG" 
out expr);

#line  1896 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr);  
		}
	}

	void RelationalExpr(
#line  1900 "VBNET.ATG" 
out Expression outExpr) {

#line  1902 "VBNET.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		ShiftExpr(
#line  1905 "VBNET.ATG" 
out outExpr);
		while (StartOf(24)) {
			if (StartOf(25)) {
				if (la.kind == 27) {
					lexer.NextToken();

#line  1908 "VBNET.ATG" 
					op = BinaryOperatorType.LessThan; 
				} else if (la.kind == 26) {
					lexer.NextToken();

#line  1909 "VBNET.ATG" 
					op = BinaryOperatorType.GreaterThan; 
				} else if (la.kind == 30) {
					lexer.NextToken();

#line  1910 "VBNET.ATG" 
					op = BinaryOperatorType.LessThanOrEqual; 
				} else if (la.kind == 29) {
					lexer.NextToken();

#line  1911 "VBNET.ATG" 
					op = BinaryOperatorType.GreaterThanOrEqual; 
				} else SynErr(246);
				ShiftExpr(
#line  1913 "VBNET.ATG" 
out expr);

#line  1913 "VBNET.ATG" 
				outExpr = new BinaryOperatorExpression(outExpr, op, expr);  
			} else {
				if (la.kind == 113) {
					lexer.NextToken();

#line  1916 "VBNET.ATG" 
					op = BinaryOperatorType.ReferenceEquality; 
				} else if (la.kind == 189) {
					lexer.NextToken();

#line  1917 "VBNET.ATG" 
					op = BinaryOperatorType.ReferenceInequality; 
				} else SynErr(247);
				Expr(
#line  1918 "VBNET.ATG" 
out expr);

#line  1918 "VBNET.ATG" 
				outExpr = new BinaryOperatorExpression(outExpr, op, expr); 
			}
		}
	}

	void ShiftExpr(
#line  1922 "VBNET.ATG" 
out Expression outExpr) {

#line  1924 "VBNET.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		AdditiveExpr(
#line  1927 "VBNET.ATG" 
out outExpr);
		while (la.kind == 31 || la.kind == 32) {
			if (la.kind == 31) {
				lexer.NextToken();

#line  1930 "VBNET.ATG" 
				op = BinaryOperatorType.ShiftLeft; 
			} else {
				lexer.NextToken();

#line  1931 "VBNET.ATG" 
				op = BinaryOperatorType.ShiftRight; 
			}
			AdditiveExpr(
#line  1933 "VBNET.ATG" 
out expr);

#line  1933 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr);  
		}
	}

	void AdditiveExpr(
#line  1937 "VBNET.ATG" 
out Expression outExpr) {

#line  1939 "VBNET.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		MultiplicativeExpr(
#line  1942 "VBNET.ATG" 
out outExpr);
		while (la.kind == 14 || la.kind == 15 || la.kind == 19) {
			if (la.kind == 14) {
				lexer.NextToken();

#line  1945 "VBNET.ATG" 
				op = BinaryOperatorType.Add; 
			} else if (la.kind == 15) {
				lexer.NextToken();

#line  1946 "VBNET.ATG" 
				op = BinaryOperatorType.Subtract; 
			} else {
				lexer.NextToken();

#line  1947 "VBNET.ATG" 
				op = BinaryOperatorType.Concat; 
			}
			MultiplicativeExpr(
#line  1949 "VBNET.ATG" 
out expr);

#line  1949 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr);  
		}
	}

	void MultiplicativeExpr(
#line  1953 "VBNET.ATG" 
out Expression outExpr) {

#line  1955 "VBNET.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		UnaryExpr(
#line  1958 "VBNET.ATG" 
out outExpr);
		while (StartOf(26)) {
			if (la.kind == 16) {
				lexer.NextToken();

#line  1961 "VBNET.ATG" 
				op = BinaryOperatorType.Multiply; 
			} else if (la.kind == 17) {
				lexer.NextToken();

#line  1962 "VBNET.ATG" 
				op = BinaryOperatorType.Divide; 
			} else if (la.kind == 18) {
				lexer.NextToken();

#line  1963 "VBNET.ATG" 
				op = BinaryOperatorType.DivideInteger; 
			} else if (la.kind == 120) {
				lexer.NextToken();

#line  1964 "VBNET.ATG" 
				op = BinaryOperatorType.Modulus; 
			} else {
				lexer.NextToken();

#line  1965 "VBNET.ATG" 
				op = BinaryOperatorType.Power; 
			}
			UnaryExpr(
#line  1967 "VBNET.ATG" 
out expr);

#line  1967 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr); 
		}
	}

	void ArrayTypeModifiers(
#line  2098 "VBNET.ATG" 
out ArrayList arrayModifiers) {

#line  2100 "VBNET.ATG" 
		arrayModifiers = new ArrayList();
		int i = 0;
		
		while (
#line  2103 "VBNET.ATG" 
IsDims()) {
			Expect(24);
			if (la.kind == 12 || la.kind == 25) {
				RankList(
#line  2105 "VBNET.ATG" 
out i);
			}

#line  2107 "VBNET.ATG" 
			arrayModifiers.Add(i);
			
			Expect(25);
		}

#line  2112 "VBNET.ATG" 
		if(arrayModifiers.Count == 0) {
		 arrayModifiers = null;
		}
		
	}

	void Argument(
#line  2015 "VBNET.ATG" 
out Expression argumentexpr) {

#line  2017 "VBNET.ATG" 
		Expression expr;
		argumentexpr = null;
		string name;
		
		if (
#line  2021 "VBNET.ATG" 
IsNamedAssign()) {
			Identifier();

#line  2021 "VBNET.ATG" 
			name = t.val;  
			Expect(13);
			Expect(11);
			Expr(
#line  2021 "VBNET.ATG" 
out expr);

#line  2023 "VBNET.ATG" 
			argumentexpr = new NamedArgumentExpression(name, expr);
			
		} else if (StartOf(21)) {
			Expr(
#line  2026 "VBNET.ATG" 
out argumentexpr);
		} else SynErr(248);
	}

	void QualIdentAndTypeArguments(
#line  2072 "VBNET.ATG" 
out TypeReference typeref, bool canBeUnbound) {

#line  2073 "VBNET.ATG" 
		string name; typeref = null; 
		Qualident(
#line  2075 "VBNET.ATG" 
out name);

#line  2076 "VBNET.ATG" 
		typeref = new TypeReference(name); 
		if (
#line  2077 "VBNET.ATG" 
la.kind == Tokens.OpenParenthesis && Peek(1).kind == Tokens.Of) {
			lexer.NextToken();
			Expect(200);
			if (
#line  2079 "VBNET.ATG" 
canBeUnbound && (la.kind == Tokens.CloseParenthesis || la.kind == Tokens.Comma)) {

#line  2080 "VBNET.ATG" 
				typeref.GenericTypes.Add(NullTypeReference.Instance); 
				while (la.kind == 12) {
					lexer.NextToken();

#line  2081 "VBNET.ATG" 
					typeref.GenericTypes.Add(NullTypeReference.Instance); 
				}
			} else if (StartOf(5)) {
				TypeArgumentList(
#line  2082 "VBNET.ATG" 
typeref.GenericTypes);
			} else SynErr(249);
			Expect(25);
		}
	}

	void TypeArgumentList(
#line  2125 "VBNET.ATG" 
List<TypeReference> typeArguments) {

#line  2127 "VBNET.ATG" 
		TypeReference typeref;
		
		TypeName(
#line  2129 "VBNET.ATG" 
out typeref);

#line  2129 "VBNET.ATG" 
		if (typeref != null) typeArguments.Add(typeref); 
		while (la.kind == 12) {
			lexer.NextToken();
			TypeName(
#line  2132 "VBNET.ATG" 
out typeref);

#line  2132 "VBNET.ATG" 
			if (typeref != null) typeArguments.Add(typeref); 
		}
	}

	void RankList(
#line  2119 "VBNET.ATG" 
out int i) {

#line  2120 "VBNET.ATG" 
		i = 0; 
		while (la.kind == 12) {
			lexer.NextToken();

#line  2121 "VBNET.ATG" 
			++i; 
		}
	}

	void Attribute(
#line  2157 "VBNET.ATG" 
out ICSharpCode.NRefactory.Parser.AST.Attribute attribute) {

#line  2158 "VBNET.ATG" 
		string name;
		List<Expression> positional = new List<Expression>();
		List<NamedArgumentExpression> named = new List<NamedArgumentExpression>();
		
		if (la.kind == 198) {
			lexer.NextToken();
			Expect(10);
		}
		Qualident(
#line  2163 "VBNET.ATG" 
out name);
		if (la.kind == 24) {
			AttributeArguments(
#line  2164 "VBNET.ATG" 
positional, named);
		}

#line  2165 "VBNET.ATG" 
		attribute  = new ICSharpCode.NRefactory.Parser.AST.Attribute(name, positional, named); 
	}

	void AttributeArguments(
#line  2169 "VBNET.ATG" 
List<Expression> positional, List<NamedArgumentExpression> named) {

#line  2171 "VBNET.ATG" 
		bool nameFound = false;
		string name = "";
		Expression expr;
		
		Expect(24);
		if (
#line  2177 "VBNET.ATG" 
IsNotClosingParenthesis()) {
			if (
#line  2179 "VBNET.ATG" 
IsNamedAssign()) {

#line  2179 "VBNET.ATG" 
				nameFound = true; 
				IdentifierOrKeyword(
#line  2180 "VBNET.ATG" 
out name);
				if (la.kind == 13) {
					lexer.NextToken();
				}
				Expect(11);
			}
			Expr(
#line  2182 "VBNET.ATG" 
out expr);

#line  2184 "VBNET.ATG" 
			if (expr != null) { if(name == "") positional.Add(expr);
			else { named.Add(new NamedArgumentExpression(name, expr)); name = ""; }
			}
			
			while (la.kind == 12) {
				lexer.NextToken();
				if (
#line  2191 "VBNET.ATG" 
IsNamedAssign()) {

#line  2191 "VBNET.ATG" 
					nameFound = true; 
					IdentifierOrKeyword(
#line  2192 "VBNET.ATG" 
out name);
					if (la.kind == 13) {
						lexer.NextToken();
					}
					Expect(11);
				} else if (StartOf(21)) {

#line  2194 "VBNET.ATG" 
					if (nameFound) Error("no positional argument after named argument"); 
				} else SynErr(250);
				Expr(
#line  2195 "VBNET.ATG" 
out expr);

#line  2195 "VBNET.ATG" 
				if (expr != null) { if(name == "") positional.Add(expr);
				else { named.Add(new NamedArgumentExpression(name, expr)); name = ""; }
				}
				
			}
		}
		Expect(25);
	}

	void FormalParameter(
#line  2264 "VBNET.ATG" 
out ParameterDeclarationExpression p) {

#line  2266 "VBNET.ATG" 
		TypeReference type = null;
		ParamModifiers mod = new ParamModifiers(this);
		Expression expr = null;
		p = null;ArrayList arrayModifiers = null;
		
		while (StartOf(27)) {
			ParameterModifier(
#line  2271 "VBNET.ATG" 
mod);
		}
		Identifier();

#line  2272 "VBNET.ATG" 
		string parameterName = t.val; 
		if (
#line  2273 "VBNET.ATG" 
IsDims()) {
			ArrayTypeModifiers(
#line  2273 "VBNET.ATG" 
out arrayModifiers);
		}
		if (la.kind == 48) {
			lexer.NextToken();
			TypeName(
#line  2274 "VBNET.ATG" 
out type);
		}

#line  2276 "VBNET.ATG" 
		if(type != null) {
		if (arrayModifiers != null) {
			if (type.RankSpecifier != null) {
				Error("array rank only allowed one time");
			} else {
				type.RankSpecifier = (int[])arrayModifiers.ToArray(typeof(int));
			}
		}
		} else {
			type = new TypeReference("System.Object", arrayModifiers == null ? null : (int[])arrayModifiers.ToArray(typeof(int)));
		}
		
		if (la.kind == 11) {
			lexer.NextToken();
			Expr(
#line  2288 "VBNET.ATG" 
out expr);
		}

#line  2290 "VBNET.ATG" 
		mod.Check();
		p = new ParameterDeclarationExpression(type, parameterName, mod.Modifier, expr);
		
	}

	void ParameterModifier(
#line  2915 "VBNET.ATG" 
ParamModifiers m) {
		if (la.kind == 55) {
			lexer.NextToken();

#line  2916 "VBNET.ATG" 
			m.Add(ParamModifier.In); 
		} else if (la.kind == 53) {
			lexer.NextToken();

#line  2917 "VBNET.ATG" 
			m.Add(ParamModifier.Ref); 
		} else if (la.kind == 137) {
			lexer.NextToken();

#line  2918 "VBNET.ATG" 
			m.Add(ParamModifier.Optional); 
		} else if (la.kind == 143) {
			lexer.NextToken();

#line  2919 "VBNET.ATG" 
			m.Add(ParamModifier.Params); 
		} else SynErr(251);
	}

	void Statement() {

#line  2317 "VBNET.ATG" 
		Statement stmt = null;
		Point startPos = la.Location;
		string label = String.Empty;
		
		
		if (la.kind == 1 || la.kind == 13) {
		} else if (
#line  2323 "VBNET.ATG" 
IsLabel()) {
			LabelName(
#line  2323 "VBNET.ATG" 
out label);

#line  2325 "VBNET.ATG" 
			compilationUnit.AddChild(new LabelStatement(t.val));
			
			Expect(13);
			Statement();
		} else if (StartOf(28)) {
			EmbeddedStatement(
#line  2328 "VBNET.ATG" 
out stmt);

#line  2328 "VBNET.ATG" 
			compilationUnit.AddChild(stmt); 
		} else if (StartOf(29)) {
			LocalDeclarationStatement(
#line  2329 "VBNET.ATG" 
out stmt);

#line  2329 "VBNET.ATG" 
			compilationUnit.AddChild(stmt); 
		} else SynErr(252);

#line  2332 "VBNET.ATG" 
		if (stmt != null) {
		stmt.StartLocation = startPos;
		stmt.EndLocation = t.Location;
		}
		
	}

	void LabelName(
#line  2715 "VBNET.ATG" 
out string name) {

#line  2717 "VBNET.ATG" 
		name = String.Empty;
		
		if (StartOf(12)) {
			Identifier();

#line  2719 "VBNET.ATG" 
			name = t.val; 
		} else if (la.kind == 5) {
			lexer.NextToken();

#line  2720 "VBNET.ATG" 
			name = t.val; 
		} else SynErr(253);
	}

	void EmbeddedStatement(
#line  2371 "VBNET.ATG" 
out Statement statement) {

#line  2373 "VBNET.ATG" 
		Statement embeddedStatement = null;
		statement = null;
		Expression expr = null;
		string name = String.Empty;
		List<Expression> p = null;
		
		switch (la.kind) {
		case 94: {
			lexer.NextToken();

#line  2379 "VBNET.ATG" 
			ExitType exitType = ExitType.None; 
			switch (la.kind) {
			case 167: {
				lexer.NextToken();

#line  2381 "VBNET.ATG" 
				exitType = ExitType.Sub; 
				break;
			}
			case 100: {
				lexer.NextToken();

#line  2383 "VBNET.ATG" 
				exitType = ExitType.Function; 
				break;
			}
			case 146: {
				lexer.NextToken();

#line  2385 "VBNET.ATG" 
				exitType = ExitType.Property; 
				break;
			}
			case 83: {
				lexer.NextToken();

#line  2387 "VBNET.ATG" 
				exitType = ExitType.Do; 
				break;
			}
			case 98: {
				lexer.NextToken();

#line  2389 "VBNET.ATG" 
				exitType = ExitType.For; 
				break;
			}
			case 174: {
				lexer.NextToken();

#line  2391 "VBNET.ATG" 
				exitType = ExitType.Try; 
				break;
			}
			case 181: {
				lexer.NextToken();

#line  2393 "VBNET.ATG" 
				exitType = ExitType.While; 
				break;
			}
			case 155: {
				lexer.NextToken();

#line  2395 "VBNET.ATG" 
				exitType = ExitType.Select; 
				break;
			}
			default: SynErr(254); break;
			}

#line  2397 "VBNET.ATG" 
			statement = new ExitStatement(exitType); 
			break;
		}
		case 174: {
			TryStatement(
#line  2398 "VBNET.ATG" 
out statement);
			break;
		}
		case 186: {
			lexer.NextToken();

#line  2399 "VBNET.ATG" 
			ContinueType continueType = ContinueType.None; 
			if (la.kind == 83 || la.kind == 98 || la.kind == 181) {
				if (la.kind == 83) {
					lexer.NextToken();

#line  2399 "VBNET.ATG" 
					continueType = ContinueType.Do; 
				} else if (la.kind == 98) {
					lexer.NextToken();

#line  2399 "VBNET.ATG" 
					continueType = ContinueType.For; 
				} else {
					lexer.NextToken();

#line  2399 "VBNET.ATG" 
					continueType = ContinueType.While; 
				}
			}

#line  2399 "VBNET.ATG" 
			statement = new ContinueStatement(continueType); 
			break;
		}
		case 171: {
			lexer.NextToken();
			if (StartOf(21)) {
				Expr(
#line  2401 "VBNET.ATG" 
out expr);
			}

#line  2401 "VBNET.ATG" 
			statement = new ThrowStatement(expr); 
			break;
		}
		case 154: {
			lexer.NextToken();
			if (StartOf(21)) {
				Expr(
#line  2403 "VBNET.ATG" 
out expr);
			}

#line  2403 "VBNET.ATG" 
			statement = new ReturnStatement(expr); 
			break;
		}
		case 168: {
			lexer.NextToken();
			Expr(
#line  2405 "VBNET.ATG" 
out expr);
			EndOfStmt();
			Block(
#line  2405 "VBNET.ATG" 
out embeddedStatement);
			Expect(88);
			Expect(168);

#line  2406 "VBNET.ATG" 
			statement = new LockStatement(expr, embeddedStatement); 
			break;
		}
		case 149: {
			lexer.NextToken();
			Identifier();

#line  2408 "VBNET.ATG" 
			name = t.val; 
			if (la.kind == 24) {
				lexer.NextToken();
				if (StartOf(20)) {
					ArgumentList(
#line  2409 "VBNET.ATG" 
out p);
				}
				Expect(25);
			}

#line  2410 "VBNET.ATG" 
			statement = new RaiseEventStatement(name, p); 
			break;
		}
		case 182: {
			WithStatement(
#line  2412 "VBNET.ATG" 
out statement);
			break;
		}
		case 42: {
			lexer.NextToken();

#line  2414 "VBNET.ATG" 
			Expression handlerExpr = null; 
			Expr(
#line  2415 "VBNET.ATG" 
out expr);
			Expect(12);
			Expr(
#line  2415 "VBNET.ATG" 
out handlerExpr);

#line  2417 "VBNET.ATG" 
			statement = new AddHandlerStatement(expr, handlerExpr);
			
			break;
		}
		case 152: {
			lexer.NextToken();

#line  2420 "VBNET.ATG" 
			Expression handlerExpr = null; 
			Expr(
#line  2421 "VBNET.ATG" 
out expr);
			Expect(12);
			Expr(
#line  2421 "VBNET.ATG" 
out handlerExpr);

#line  2423 "VBNET.ATG" 
			statement = new RemoveHandlerStatement(expr, handlerExpr);
			
			break;
		}
		case 181: {
			lexer.NextToken();
			Expr(
#line  2426 "VBNET.ATG" 
out expr);
			EndOfStmt();
			Block(
#line  2427 "VBNET.ATG" 
out embeddedStatement);
			Expect(88);
			Expect(181);

#line  2429 "VBNET.ATG" 
			statement = new DoLoopStatement(expr, embeddedStatement, ConditionType.While, ConditionPosition.Start);
			
			break;
		}
		case 83: {
			lexer.NextToken();

#line  2434 "VBNET.ATG" 
			ConditionType conditionType = ConditionType.None;
			
			if (la.kind == 177 || la.kind == 181) {
				WhileOrUntil(
#line  2437 "VBNET.ATG" 
out conditionType);
				Expr(
#line  2437 "VBNET.ATG" 
out expr);
				EndOfStmt();
				Block(
#line  2438 "VBNET.ATG" 
out embeddedStatement);
				Expect(118);

#line  2441 "VBNET.ATG" 
				statement = new DoLoopStatement(expr, 
				                               embeddedStatement, 
				                               conditionType == ConditionType.While ? ConditionType.DoWhile : conditionType, 
				                               ConditionPosition.Start);
				
			} else if (la.kind == 1 || la.kind == 13) {
				EndOfStmt();
				Block(
#line  2448 "VBNET.ATG" 
out embeddedStatement);
				Expect(118);
				if (la.kind == 177 || la.kind == 181) {
					WhileOrUntil(
#line  2449 "VBNET.ATG" 
out conditionType);
					Expr(
#line  2449 "VBNET.ATG" 
out expr);
				}

#line  2451 "VBNET.ATG" 
				statement = new DoLoopStatement(expr, embeddedStatement, conditionType, ConditionPosition.End);
				
			} else SynErr(255);
			break;
		}
		case 98: {
			lexer.NextToken();

#line  2456 "VBNET.ATG" 
			Expression group = null;
			TypeReference typeReference;
			string        typeName;
			Point startLocation = t.Location;
			
			if (la.kind == 85) {
				lexer.NextToken();
				LoopControlVariable(
#line  2463 "VBNET.ATG" 
out typeReference, out typeName);
				Expect(109);
				Expr(
#line  2464 "VBNET.ATG" 
out group);
				EndOfStmt();
				Block(
#line  2465 "VBNET.ATG" 
out embeddedStatement);
				Expect(128);
				if (StartOf(21)) {
					Expr(
#line  2466 "VBNET.ATG" 
out expr);
				}

#line  2468 "VBNET.ATG" 
				statement = new ForeachStatement(typeReference, 
				                                typeName,
				                                group, 
				                                embeddedStatement, 
				                                expr);
				statement.StartLocation = startLocation;
				statement.EndLocation   = t.EndLocation;
				
				
			} else if (StartOf(12)) {

#line  2479 "VBNET.ATG" 
				Expression start = null;
				Expression end = null;
				Expression step = null;
				Expression nextExpr = null;ArrayList nextExpressions = null;
				
				LoopControlVariable(
#line  2484 "VBNET.ATG" 
out typeReference, out typeName);
				Expect(11);
				Expr(
#line  2485 "VBNET.ATG" 
out start);
				Expect(172);
				Expr(
#line  2485 "VBNET.ATG" 
out end);
				if (la.kind == 162) {
					lexer.NextToken();
					Expr(
#line  2485 "VBNET.ATG" 
out step);
				}
				EndOfStmt();
				Block(
#line  2486 "VBNET.ATG" 
out embeddedStatement);
				Expect(128);
				if (StartOf(21)) {
					Expr(
#line  2489 "VBNET.ATG" 
out nextExpr);

#line  2489 "VBNET.ATG" 
					nextExpressions = new ArrayList(); nextExpressions.Add(nextExpr); 
					while (la.kind == 12) {
						lexer.NextToken();
						Expr(
#line  2490 "VBNET.ATG" 
out nextExpr);

#line  2490 "VBNET.ATG" 
						nextExpressions.Add(nextExpr); 
					}
				}

#line  2493 "VBNET.ATG" 
				statement = new ForNextStatement(typeReference, typeName, start, end, step, embeddedStatement, nextExpressions);
				
			} else SynErr(256);
			break;
		}
		case 92: {
			lexer.NextToken();
			Expr(
#line  2497 "VBNET.ATG" 
out expr);

#line  2497 "VBNET.ATG" 
			statement = new ErrorStatement(expr); 
			break;
		}
		case 151: {
			lexer.NextToken();

#line  2499 "VBNET.ATG" 
			Expression redimclause = null; bool isPreserve = false; 
			if (la.kind == 144) {
				lexer.NextToken();

#line  2499 "VBNET.ATG" 
				isPreserve = true; 
			}
			Expr(
#line  2500 "VBNET.ATG" 
out redimclause);

#line  2502 "VBNET.ATG" 
			ReDimStatement reDimStatement = new ReDimStatement(isPreserve);
			statement = reDimStatement;
			reDimStatement.ReDimClauses.Add(redimclause as InvocationExpression);
			
			while (la.kind == 12) {
				lexer.NextToken();
				Expr(
#line  2506 "VBNET.ATG" 
out redimclause);

#line  2506 "VBNET.ATG" 
				reDimStatement.ReDimClauses.Add(redimclause as InvocationExpression); 
			}
			break;
		}
		case 91: {
			lexer.NextToken();
			Expr(
#line  2509 "VBNET.ATG" 
out expr);

#line  2510 "VBNET.ATG" 
			ArrayList arrays = new ArrayList();
			if (expr != null) { arrays.Add(expr);}
			EraseStatement eraseStatement = new EraseStatement(arrays);
			
			
			while (la.kind == 12) {
				lexer.NextToken();
				Expr(
#line  2515 "VBNET.ATG" 
out expr);

#line  2515 "VBNET.ATG" 
				if (expr != null) { arrays.Add(expr); }
			}

#line  2516 "VBNET.ATG" 
			statement = eraseStatement; 
			break;
		}
		case 163: {
			lexer.NextToken();

#line  2518 "VBNET.ATG" 
			statement = new StopStatement(); 
			break;
		}
		case 106: {
			lexer.NextToken();
			Expr(
#line  2520 "VBNET.ATG" 
out expr);
			if (la.kind == 170) {
				lexer.NextToken();
			}
			if (
#line  2522 "VBNET.ATG" 
IsEndStmtAhead()) {
				Expect(88);

#line  2522 "VBNET.ATG" 
				statement = new IfElseStatement(expr, new EndStatement()); 
			} else if (la.kind == 1 || la.kind == 13) {
				EndOfStmt();
				Block(
#line  2525 "VBNET.ATG" 
out embeddedStatement);

#line  2527 "VBNET.ATG" 
				IfElseStatement ifStatement = new IfElseStatement(expr, embeddedStatement);
				
				while (la.kind == 87 || 
#line  2531 "VBNET.ATG" 
IsElseIf()) {
					if (
#line  2531 "VBNET.ATG" 
IsElseIf()) {
						Expect(86);
						Expect(106);
					} else {
						lexer.NextToken();
					}

#line  2534 "VBNET.ATG" 
					Expression condition = null; Statement block = null; 
					Expr(
#line  2535 "VBNET.ATG" 
out condition);
					if (la.kind == 170) {
						lexer.NextToken();
					}
					EndOfStmt();
					Block(
#line  2536 "VBNET.ATG" 
out block);

#line  2538 "VBNET.ATG" 
					ifStatement.ElseIfSections.Add(new ElseIfSection(condition, block));
					
				}
				if (la.kind == 86) {
					lexer.NextToken();
					EndOfStmt();
					Block(
#line  2543 "VBNET.ATG" 
out embeddedStatement);

#line  2545 "VBNET.ATG" 
					ifStatement.FalseStatement.Add(embeddedStatement);
					
				}
				Expect(88);
				Expect(106);

#line  2549 "VBNET.ATG" 
				statement = ifStatement;
				
			} else if (StartOf(28)) {
				EmbeddedStatement(
#line  2552 "VBNET.ATG" 
out embeddedStatement);

#line  2554 "VBNET.ATG" 
				IfElseStatement ifStatement = new IfElseStatement(expr, embeddedStatement);
				
				while (la.kind == 13) {
					lexer.NextToken();
					EmbeddedStatement(
#line  2556 "VBNET.ATG" 
out embeddedStatement);

#line  2556 "VBNET.ATG" 
					ifStatement.TrueStatement.Add(embeddedStatement); 
				}
				if (la.kind == 86) {
					lexer.NextToken();
					if (StartOf(28)) {
						EmbeddedStatement(
#line  2558 "VBNET.ATG" 
out embeddedStatement);
					}

#line  2560 "VBNET.ATG" 
					ifStatement.FalseStatement.Add(embeddedStatement);
					
					while (la.kind == 13) {
						lexer.NextToken();
						EmbeddedStatement(
#line  2563 "VBNET.ATG" 
out embeddedStatement);

#line  2564 "VBNET.ATG" 
						ifStatement.FalseStatement.Add(embeddedStatement); 
					}
				}

#line  2567 "VBNET.ATG" 
				statement = ifStatement; 
			} else SynErr(257);
			break;
		}
		case 155: {
			lexer.NextToken();
			if (la.kind == 57) {
				lexer.NextToken();
			}
			Expr(
#line  2570 "VBNET.ATG" 
out expr);
			EndOfStmt();

#line  2571 "VBNET.ATG" 
			List<SwitchSection> selectSections = new List<SwitchSection>();
			Statement block = null;
			
			while (la.kind == 57) {

#line  2575 "VBNET.ATG" 
				List<CaseLabel> caseClauses = null; 
				lexer.NextToken();
				CaseClauses(
#line  2576 "VBNET.ATG" 
out caseClauses);
				if (
#line  2576 "VBNET.ATG" 
IsNotStatementSeparator()) {
					lexer.NextToken();
				}
				EndOfStmt();

#line  2578 "VBNET.ATG" 
				SwitchSection selectSection = new SwitchSection(caseClauses);
				
				Block(
#line  2580 "VBNET.ATG" 
out block);

#line  2582 "VBNET.ATG" 
				selectSection.Children = block.Children;
				selectSections.Add(selectSection);
				
			}

#line  2586 "VBNET.ATG" 
			statement = new SwitchStatement(expr, selectSections); 
			Expect(88);
			Expect(155);
			break;
		}
		case 135: {

#line  2588 "VBNET.ATG" 
			OnErrorStatement onErrorStatement = null; 
			OnErrorStatement(
#line  2589 "VBNET.ATG" 
out onErrorStatement);

#line  2589 "VBNET.ATG" 
			statement = onErrorStatement; 
			break;
		}
		case 104: {

#line  2590 "VBNET.ATG" 
			GotoStatement goToStatement = null; 
			GotoStatement(
#line  2591 "VBNET.ATG" 
out goToStatement);

#line  2591 "VBNET.ATG" 
			statement = goToStatement; 
			break;
		}
		case 153: {

#line  2592 "VBNET.ATG" 
			ResumeStatement resumeStatement = null; 
			ResumeStatement(
#line  2593 "VBNET.ATG" 
out resumeStatement);

#line  2593 "VBNET.ATG" 
			statement = resumeStatement; 
			break;
		}
		case 2: case 3: case 4: case 5: case 6: case 7: case 8: case 9: case 10: case 14: case 15: case 16: case 24: case 43: case 47: case 49: case 50: case 51: case 52: case 54: case 59: case 60: case 61: case 62: case 63: case 64: case 65: case 66: case 68: case 69: case 70: case 72: case 73: case 74: case 75: case 76: case 77: case 82: case 84: case 96: case 102: case 111: case 117: case 119: case 124: case 125: case 127: case 130: case 144: case 159: case 160: case 165: case 169: case 173: case 175: case 176: case 177: case 190: case 191: case 192: case 193: case 194: case 195: case 196: case 197: case 198: case 199: {

#line  2596 "VBNET.ATG" 
			Expression val = null;
			AssignmentOperatorType op;
			
			bool mustBeAssignment = la.kind == Tokens.Plus  || la.kind == Tokens.Minus ||
			                        la.kind == Tokens.Not   || la.kind == Tokens.Times;
			
			UnaryExpr(
#line  2602 "VBNET.ATG" 
out expr);
			if (StartOf(30)) {
				AssignmentOperator(
#line  2604 "VBNET.ATG" 
out op);
				Expr(
#line  2604 "VBNET.ATG" 
out val);

#line  2604 "VBNET.ATG" 
				expr = new AssignmentExpression(expr, op, val); 
			} else if (la.kind == 1 || la.kind == 13 || la.kind == 86) {

#line  2605 "VBNET.ATG" 
				if (mustBeAssignment) Error("error in assignment."); 
			} else SynErr(258);

#line  2608 "VBNET.ATG" 
			// a field reference expression that stands alone is a
			// invocation expression without parantheses and arguments
			if(expr is FieldReferenceExpression || expr is IdentifierExpression) {
				expr = new InvocationExpression(expr);
			}
			statement = new StatementExpression(expr);
			
			break;
		}
		case 56: {
			lexer.NextToken();
			UnaryExpr(
#line  2615 "VBNET.ATG" 
out expr);

#line  2615 "VBNET.ATG" 
			statement = new StatementExpression(expr); 
			break;
		}
		case 188: {
			lexer.NextToken();
			Identifier();

#line  2617 "VBNET.ATG" 
			string resourcename = t.val, typeName; 
			Statement resourceAquisition = null, block = null;
			
			Expect(48);
			if (la.kind == 127) {
				lexer.NextToken();
				Qualident(
#line  2620 "VBNET.ATG" 
out typeName);

#line  2621 "VBNET.ATG" 
				List<Expression> initializer = null; 
				if (la.kind == 24) {
					lexer.NextToken();
					if (StartOf(20)) {
						ArgumentList(
#line  2622 "VBNET.ATG" 
out initializer);
					}
					Expect(25);
				}

#line  2624 "VBNET.ATG" 
				resourceAquisition =  new LocalVariableDeclaration(new VariableDeclaration(resourcename, new ArrayInitializerExpression(initializer), new TypeReference(typeName)));
				
				
			} else if (StartOf(12)) {
				Qualident(
#line  2627 "VBNET.ATG" 
out typeName);
				Expect(11);
				Expr(
#line  2627 "VBNET.ATG" 
out expr);

#line  2629 "VBNET.ATG" 
				resourceAquisition =  new LocalVariableDeclaration(new VariableDeclaration(resourcename, expr, new TypeReference(typeName)));
				
			} else SynErr(259);
			Block(
#line  2632 "VBNET.ATG" 
out block);
			Expect(88);
			Expect(188);

#line  2634 "VBNET.ATG" 
			statement = new UsingStatement(resourceAquisition, block); 
			break;
		}
		default: SynErr(260); break;
		}
	}

	void LocalDeclarationStatement(
#line  2340 "VBNET.ATG" 
out Statement statement) {

#line  2342 "VBNET.ATG" 
		Modifiers m = new Modifiers();
		LocalVariableDeclaration localVariableDeclaration;
		bool dimfound = false;
		
		while (la.kind == 71 || la.kind == 81 || la.kind == 161) {
			if (la.kind == 71) {
				lexer.NextToken();

#line  2348 "VBNET.ATG" 
				m.Add(Modifier.Const, t.Location); 
			} else if (la.kind == 161) {
				lexer.NextToken();

#line  2349 "VBNET.ATG" 
				m.Add(Modifier.Static, t.Location); 
			} else {
				lexer.NextToken();

#line  2350 "VBNET.ATG" 
				dimfound = true; 
			}
		}

#line  2353 "VBNET.ATG" 
		if(dimfound && (m.Modifier & Modifier.Const) != 0) {
		Error("Dim is not allowed on constants.");
		}
		
		if(m.isNone && dimfound == false) {
			Error("Const, Dim or Static expected");
		}
		
		localVariableDeclaration = new LocalVariableDeclaration(m.Modifier);
		localVariableDeclaration.StartLocation = t.Location;
		
		VariableDeclarator(
#line  2364 "VBNET.ATG" 
localVariableDeclaration.Variables);
		while (la.kind == 12) {
			lexer.NextToken();
			VariableDeclarator(
#line  2365 "VBNET.ATG" 
localVariableDeclaration.Variables);
		}

#line  2367 "VBNET.ATG" 
		statement = localVariableDeclaration;
		
	}

	void TryStatement(
#line  2827 "VBNET.ATG" 
out Statement tryStatement) {

#line  2829 "VBNET.ATG" 
		Statement blockStmt = null, finallyStmt = null;List<CatchClause> catchClauses = null;
		
		Expect(174);
		EndOfStmt();
		Block(
#line  2832 "VBNET.ATG" 
out blockStmt);
		if (la.kind == 58 || la.kind == 88 || la.kind == 97) {
			CatchClauses(
#line  2833 "VBNET.ATG" 
out catchClauses);
		}
		if (la.kind == 97) {
			lexer.NextToken();
			EndOfStmt();
			Block(
#line  2834 "VBNET.ATG" 
out finallyStmt);
		}
		Expect(88);
		Expect(174);

#line  2837 "VBNET.ATG" 
		tryStatement = new TryCatchStatement(blockStmt, catchClauses, finallyStmt);
		
	}

	void WithStatement(
#line  2805 "VBNET.ATG" 
out Statement withStatement) {

#line  2807 "VBNET.ATG" 
		Statement blockStmt = null;
		Expression expr = null;
		
		Expect(182);

#line  2810 "VBNET.ATG" 
		Point start = t.Location; 
		Expr(
#line  2811 "VBNET.ATG" 
out expr);
		EndOfStmt();

#line  2813 "VBNET.ATG" 
		withStatement = new WithStatement(expr);
		withStatement.StartLocation = start;
		withStatements.Push(withStatement);
		
		Block(
#line  2817 "VBNET.ATG" 
out blockStmt);

#line  2819 "VBNET.ATG" 
		((WithStatement)withStatement).Body = (BlockStatement)blockStmt;
		withStatements.Pop();
		
		Expect(88);
		Expect(182);

#line  2823 "VBNET.ATG" 
		withStatement.EndLocation = t.Location; 
	}

	void WhileOrUntil(
#line  2798 "VBNET.ATG" 
out ConditionType conditionType) {

#line  2799 "VBNET.ATG" 
		conditionType = ConditionType.None; 
		if (la.kind == 181) {
			lexer.NextToken();

#line  2800 "VBNET.ATG" 
			conditionType = ConditionType.While; 
		} else if (la.kind == 177) {
			lexer.NextToken();

#line  2801 "VBNET.ATG" 
			conditionType = ConditionType.Until; 
		} else SynErr(261);
	}

	void LoopControlVariable(
#line  2639 "VBNET.ATG" 
out TypeReference type, out string name) {

#line  2640 "VBNET.ATG" 
		ArrayList arrayModifiers = null;
		type = null;
		
		Qualident(
#line  2644 "VBNET.ATG" 
out name);
		if (
#line  2645 "VBNET.ATG" 
IsDims()) {
			ArrayTypeModifiers(
#line  2645 "VBNET.ATG" 
out arrayModifiers);
		}
		if (la.kind == 48) {
			lexer.NextToken();
			TypeName(
#line  2646 "VBNET.ATG" 
out type);

#line  2646 "VBNET.ATG" 
			if (name.IndexOf('.') > 0) { Error("No type def for 'for each' member indexer allowed."); } 
		}

#line  2648 "VBNET.ATG" 
		if (type != null) {
		if(type.RankSpecifier != null && arrayModifiers != null) {
			Error("array rank only allowed one time");
		} else if (arrayModifiers != null) {
			type.RankSpecifier = (int[])arrayModifiers.ToArray(typeof(int));
		}
		} else {
			if (arrayModifiers != null) {
				type = new TypeReference("Integer", (int[])arrayModifiers.ToArray(typeof(int)));
			} else {
				type = new TypeReference("Integer");
			}
		}
		
	}

	void CaseClauses(
#line  2758 "VBNET.ATG" 
out List<CaseLabel> caseClauses) {

#line  2760 "VBNET.ATG" 
		caseClauses = new List<CaseLabel>();
		CaseLabel caseClause = null;
		
		CaseClause(
#line  2763 "VBNET.ATG" 
out caseClause);

#line  2763 "VBNET.ATG" 
		caseClauses.Add(caseClause); 
		while (la.kind == 12) {
			lexer.NextToken();
			CaseClause(
#line  2764 "VBNET.ATG" 
out caseClause);

#line  2764 "VBNET.ATG" 
			caseClauses.Add(caseClause); 
		}
	}

	void OnErrorStatement(
#line  2665 "VBNET.ATG" 
out OnErrorStatement stmt) {

#line  2667 "VBNET.ATG" 
		stmt = null;
		GotoStatement goToStatement = null;
		
		Expect(135);
		Expect(92);
		if (
#line  2673 "VBNET.ATG" 
IsNegativeLabelName()) {
			Expect(104);
			Expect(15);
			Expect(5);

#line  2675 "VBNET.ATG" 
			long intLabel = Int64.Parse(t.val);
			if(intLabel != 1) {
				Error("invalid label in on error statement.");
			}
			stmt = new OnErrorStatement(new GotoStatement((intLabel * -1).ToString()));
			
		} else if (la.kind == 104) {
			GotoStatement(
#line  2681 "VBNET.ATG" 
out goToStatement);

#line  2683 "VBNET.ATG" 
			string val = goToStatement.Label;
			
			// if value is numeric, make sure that is 0
			try {
				long intLabel = Int64.Parse(val);
				if(intLabel != 0) {
					Error("invalid label in on error statement.");
				}
			} catch {
			}
			stmt = new OnErrorStatement(goToStatement);
			
		} else if (la.kind == 153) {
			lexer.NextToken();
			Expect(128);

#line  2697 "VBNET.ATG" 
			stmt = new OnErrorStatement(new ResumeStatement(true));
			
		} else SynErr(262);
	}

	void GotoStatement(
#line  2703 "VBNET.ATG" 
out ICSharpCode.NRefactory.Parser.AST.GotoStatement goToStatement) {

#line  2705 "VBNET.ATG" 
		string label = String.Empty;
		
		Expect(104);
		LabelName(
#line  2708 "VBNET.ATG" 
out label);

#line  2710 "VBNET.ATG" 
		goToStatement = new ICSharpCode.NRefactory.Parser.AST.GotoStatement(label);
		
	}

	void ResumeStatement(
#line  2747 "VBNET.ATG" 
out ResumeStatement resumeStatement) {

#line  2749 "VBNET.ATG" 
		resumeStatement = null;
		string label = String.Empty;
		
		if (
#line  2752 "VBNET.ATG" 
IsResumeNext()) {
			Expect(153);
			Expect(128);

#line  2753 "VBNET.ATG" 
			resumeStatement = new ResumeStatement(true); 
		} else if (la.kind == 153) {
			lexer.NextToken();
			if (StartOf(31)) {
				LabelName(
#line  2754 "VBNET.ATG" 
out label);
			}

#line  2754 "VBNET.ATG" 
			resumeStatement = new ResumeStatement(label); 
		} else SynErr(263);
	}

	void CaseClause(
#line  2768 "VBNET.ATG" 
out CaseLabel caseClause) {

#line  2770 "VBNET.ATG" 
		Expression expr = null;
		Expression sexpr = null;
		BinaryOperatorType op = BinaryOperatorType.None;
		caseClause = null;
		
		if (la.kind == 86) {
			lexer.NextToken();

#line  2776 "VBNET.ATG" 
			caseClause = new CaseLabel(); 
		} else if (StartOf(32)) {
			if (la.kind == 113) {
				lexer.NextToken();
			}
			switch (la.kind) {
			case 27: {
				lexer.NextToken();

#line  2780 "VBNET.ATG" 
				op = BinaryOperatorType.LessThan; 
				break;
			}
			case 26: {
				lexer.NextToken();

#line  2781 "VBNET.ATG" 
				op = BinaryOperatorType.GreaterThan; 
				break;
			}
			case 30: {
				lexer.NextToken();

#line  2782 "VBNET.ATG" 
				op = BinaryOperatorType.LessThanOrEqual; 
				break;
			}
			case 29: {
				lexer.NextToken();

#line  2783 "VBNET.ATG" 
				op = BinaryOperatorType.GreaterThanOrEqual; 
				break;
			}
			case 11: {
				lexer.NextToken();

#line  2784 "VBNET.ATG" 
				op = BinaryOperatorType.Equality; 
				break;
			}
			case 28: {
				lexer.NextToken();

#line  2785 "VBNET.ATG" 
				op = BinaryOperatorType.InEquality; 
				break;
			}
			default: SynErr(264); break;
			}
			Expr(
#line  2787 "VBNET.ATG" 
out expr);

#line  2789 "VBNET.ATG" 
			caseClause = new CaseLabel(op, expr);
			
		} else if (StartOf(21)) {
			Expr(
#line  2791 "VBNET.ATG" 
out expr);
			if (la.kind == 172) {
				lexer.NextToken();
				Expr(
#line  2791 "VBNET.ATG" 
out sexpr);
			}

#line  2793 "VBNET.ATG" 
			caseClause = new CaseLabel(expr, sexpr);
			
		} else SynErr(265);
	}

	void CatchClauses(
#line  2842 "VBNET.ATG" 
out List<CatchClause> catchClauses) {

#line  2844 "VBNET.ATG" 
		catchClauses = new List<CatchClause>();
		TypeReference type = null;
		Statement blockStmt = null;
		Expression expr = null;
		string name = String.Empty;
		
		while (la.kind == 58) {
			lexer.NextToken();
			if (StartOf(12)) {
				Identifier();

#line  2852 "VBNET.ATG" 
				name = t.val; 
				if (la.kind == 48) {
					lexer.NextToken();
					TypeName(
#line  2852 "VBNET.ATG" 
out type);
				}
			}
			if (la.kind == 180) {
				lexer.NextToken();
				Expr(
#line  2853 "VBNET.ATG" 
out expr);
			}
			EndOfStmt();
			Block(
#line  2855 "VBNET.ATG" 
out blockStmt);

#line  2856 "VBNET.ATG" 
			catchClauses.Add(new CatchClause(type, name, blockStmt, expr)); 
		}
	}


	public Parser(ILexer lexer) : base(lexer)
	{
	}
	
	public override void Parse()
	{
		VBNET();

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
			case 1: s = "EOL expected"; break;
			case 2: s = "ident expected"; break;
			case 3: s = "LiteralString expected"; break;
			case 4: s = "LiteralCharacter expected"; break;
			case 5: s = "LiteralInteger expected"; break;
			case 6: s = "LiteralDouble expected"; break;
			case 7: s = "LiteralSingle expected"; break;
			case 8: s = "LiteralDecimal expected"; break;
			case 9: s = "LiteralDate expected"; break;
			case 10: s = "\".\" expected"; break;
			case 11: s = "\"=\" expected"; break;
			case 12: s = "\",\" expected"; break;
			case 13: s = "\":\" expected"; break;
			case 14: s = "\"+\" expected"; break;
			case 15: s = "\"-\" expected"; break;
			case 16: s = "\"*\" expected"; break;
			case 17: s = "\"/\" expected"; break;
			case 18: s = "\"\\\\\" expected"; break;
			case 19: s = "\"&\" expected"; break;
			case 20: s = "\"^\" expected"; break;
			case 21: s = "\"?\" expected"; break;
			case 22: s = "\"{\" expected"; break;
			case 23: s = "\"}\" expected"; break;
			case 24: s = "\"(\" expected"; break;
			case 25: s = "\")\" expected"; break;
			case 26: s = "\">\" expected"; break;
			case 27: s = "\"<\" expected"; break;
			case 28: s = "\"<>\" expected"; break;
			case 29: s = "\">=\" expected"; break;
			case 30: s = "\"<=\" expected"; break;
			case 31: s = "\"<<\" expected"; break;
			case 32: s = "\">>\" expected"; break;
			case 33: s = "\"+=\" expected"; break;
			case 34: s = "\"^=\" expected"; break;
			case 35: s = "\"-=\" expected"; break;
			case 36: s = "\"*=\" expected"; break;
			case 37: s = "\"/=\" expected"; break;
			case 38: s = "\"\\\\=\" expected"; break;
			case 39: s = "\"<<=\" expected"; break;
			case 40: s = "\">>=\" expected"; break;
			case 41: s = "\"&=\" expected"; break;
			case 42: s = "\"AddHandler\" expected"; break;
			case 43: s = "\"AddressOf\" expected"; break;
			case 44: s = "\"Alias\" expected"; break;
			case 45: s = "\"And\" expected"; break;
			case 46: s = "\"AndAlso\" expected"; break;
			case 47: s = "\"Ansi\" expected"; break;
			case 48: s = "\"As\" expected"; break;
			case 49: s = "\"Assembly\" expected"; break;
			case 50: s = "\"Auto\" expected"; break;
			case 51: s = "\"Binary\" expected"; break;
			case 52: s = "\"Boolean\" expected"; break;
			case 53: s = "\"ByRef\" expected"; break;
			case 54: s = "\"Byte\" expected"; break;
			case 55: s = "\"ByVal\" expected"; break;
			case 56: s = "\"Call\" expected"; break;
			case 57: s = "\"Case\" expected"; break;
			case 58: s = "\"Catch\" expected"; break;
			case 59: s = "\"CBool\" expected"; break;
			case 60: s = "\"CByte\" expected"; break;
			case 61: s = "\"CChar\" expected"; break;
			case 62: s = "\"CDate\" expected"; break;
			case 63: s = "\"CDbl\" expected"; break;
			case 64: s = "\"CDec\" expected"; break;
			case 65: s = "\"Char\" expected"; break;
			case 66: s = "\"CInt\" expected"; break;
			case 67: s = "\"Class\" expected"; break;
			case 68: s = "\"CLng\" expected"; break;
			case 69: s = "\"CObj\" expected"; break;
			case 70: s = "\"Compare\" expected"; break;
			case 71: s = "\"Const\" expected"; break;
			case 72: s = "\"CShort\" expected"; break;
			case 73: s = "\"CSng\" expected"; break;
			case 74: s = "\"CStr\" expected"; break;
			case 75: s = "\"CType\" expected"; break;
			case 76: s = "\"Date\" expected"; break;
			case 77: s = "\"Decimal\" expected"; break;
			case 78: s = "\"Declare\" expected"; break;
			case 79: s = "\"Default\" expected"; break;
			case 80: s = "\"Delegate\" expected"; break;
			case 81: s = "\"Dim\" expected"; break;
			case 82: s = "\"DirectCast\" expected"; break;
			case 83: s = "\"Do\" expected"; break;
			case 84: s = "\"Double\" expected"; break;
			case 85: s = "\"Each\" expected"; break;
			case 86: s = "\"Else\" expected"; break;
			case 87: s = "\"ElseIf\" expected"; break;
			case 88: s = "\"End\" expected"; break;
			case 89: s = "\"EndIf\" expected"; break;
			case 90: s = "\"Enum\" expected"; break;
			case 91: s = "\"Erase\" expected"; break;
			case 92: s = "\"Error\" expected"; break;
			case 93: s = "\"Event\" expected"; break;
			case 94: s = "\"Exit\" expected"; break;
			case 95: s = "\"Explicit\" expected"; break;
			case 96: s = "\"False\" expected"; break;
			case 97: s = "\"Finally\" expected"; break;
			case 98: s = "\"For\" expected"; break;
			case 99: s = "\"Friend\" expected"; break;
			case 100: s = "\"Function\" expected"; break;
			case 101: s = "\"Get\" expected"; break;
			case 102: s = "\"GetType\" expected"; break;
			case 103: s = "\"GoSub\" expected"; break;
			case 104: s = "\"GoTo\" expected"; break;
			case 105: s = "\"Handles\" expected"; break;
			case 106: s = "\"If\" expected"; break;
			case 107: s = "\"Implements\" expected"; break;
			case 108: s = "\"Imports\" expected"; break;
			case 109: s = "\"In\" expected"; break;
			case 110: s = "\"Inherits\" expected"; break;
			case 111: s = "\"Integer\" expected"; break;
			case 112: s = "\"Interface\" expected"; break;
			case 113: s = "\"Is\" expected"; break;
			case 114: s = "\"Let\" expected"; break;
			case 115: s = "\"Lib\" expected"; break;
			case 116: s = "\"Like\" expected"; break;
			case 117: s = "\"Long\" expected"; break;
			case 118: s = "\"Loop\" expected"; break;
			case 119: s = "\"Me\" expected"; break;
			case 120: s = "\"Mod\" expected"; break;
			case 121: s = "\"Module\" expected"; break;
			case 122: s = "\"MustInherit\" expected"; break;
			case 123: s = "\"MustOverride\" expected"; break;
			case 124: s = "\"MyBase\" expected"; break;
			case 125: s = "\"MyClass\" expected"; break;
			case 126: s = "\"Namespace\" expected"; break;
			case 127: s = "\"New\" expected"; break;
			case 128: s = "\"Next\" expected"; break;
			case 129: s = "\"Not\" expected"; break;
			case 130: s = "\"Nothing\" expected"; break;
			case 131: s = "\"NotInheritable\" expected"; break;
			case 132: s = "\"NotOverridable\" expected"; break;
			case 133: s = "\"Object\" expected"; break;
			case 134: s = "\"Off\" expected"; break;
			case 135: s = "\"On\" expected"; break;
			case 136: s = "\"Option\" expected"; break;
			case 137: s = "\"Optional\" expected"; break;
			case 138: s = "\"Or\" expected"; break;
			case 139: s = "\"OrElse\" expected"; break;
			case 140: s = "\"Overloads\" expected"; break;
			case 141: s = "\"Overridable\" expected"; break;
			case 142: s = "\"Overrides\" expected"; break;
			case 143: s = "\"ParamArray\" expected"; break;
			case 144: s = "\"Preserve\" expected"; break;
			case 145: s = "\"Private\" expected"; break;
			case 146: s = "\"Property\" expected"; break;
			case 147: s = "\"Protected\" expected"; break;
			case 148: s = "\"Public\" expected"; break;
			case 149: s = "\"RaiseEvent\" expected"; break;
			case 150: s = "\"ReadOnly\" expected"; break;
			case 151: s = "\"ReDim\" expected"; break;
			case 152: s = "\"RemoveHandler\" expected"; break;
			case 153: s = "\"Resume\" expected"; break;
			case 154: s = "\"Return\" expected"; break;
			case 155: s = "\"Select\" expected"; break;
			case 156: s = "\"Set\" expected"; break;
			case 157: s = "\"Shadows\" expected"; break;
			case 158: s = "\"Shared\" expected"; break;
			case 159: s = "\"Short\" expected"; break;
			case 160: s = "\"Single\" expected"; break;
			case 161: s = "\"Static\" expected"; break;
			case 162: s = "\"Step\" expected"; break;
			case 163: s = "\"Stop\" expected"; break;
			case 164: s = "\"Strict\" expected"; break;
			case 165: s = "\"String\" expected"; break;
			case 166: s = "\"Structure\" expected"; break;
			case 167: s = "\"Sub\" expected"; break;
			case 168: s = "\"SyncLock\" expected"; break;
			case 169: s = "\"Text\" expected"; break;
			case 170: s = "\"Then\" expected"; break;
			case 171: s = "\"Throw\" expected"; break;
			case 172: s = "\"To\" expected"; break;
			case 173: s = "\"True\" expected"; break;
			case 174: s = "\"Try\" expected"; break;
			case 175: s = "\"TypeOf\" expected"; break;
			case 176: s = "\"Unicode\" expected"; break;
			case 177: s = "\"Until\" expected"; break;
			case 178: s = "\"Variant\" expected"; break;
			case 179: s = "\"Wend\" expected"; break;
			case 180: s = "\"When\" expected"; break;
			case 181: s = "\"While\" expected"; break;
			case 182: s = "\"With\" expected"; break;
			case 183: s = "\"WithEvents\" expected"; break;
			case 184: s = "\"WriteOnly\" expected"; break;
			case 185: s = "\"Xor\" expected"; break;
			case 186: s = "\"Continue\" expected"; break;
			case 187: s = "\"Operator\" expected"; break;
			case 188: s = "\"Using\" expected"; break;
			case 189: s = "\"IsNot\" expected"; break;
			case 190: s = "\"SByte\" expected"; break;
			case 191: s = "\"UInteger\" expected"; break;
			case 192: s = "\"ULong\" expected"; break;
			case 193: s = "\"UShort\" expected"; break;
			case 194: s = "\"CSByte\" expected"; break;
			case 195: s = "\"CUShort\" expected"; break;
			case 196: s = "\"CUInt\" expected"; break;
			case 197: s = "\"CULng\" expected"; break;
			case 198: s = "\"Global\" expected"; break;
			case 199: s = "\"TryCast\" expected"; break;
			case 200: s = "\"Of\" expected"; break;
			case 201: s = "\"Narrowing\" expected"; break;
			case 202: s = "\"Widening\" expected"; break;
			case 203: s = "\"Partial\" expected"; break;
			case 204: s = "\"Custom\" expected"; break;
			case 205: s = "??? expected"; break;
			case 206: s = "invalid OptionStmt"; break;
			case 207: s = "invalid OptionStmt"; break;
			case 208: s = "invalid GlobalAttributeSection"; break;
			case 209: s = "invalid GlobalAttributeSection"; break;
			case 210: s = "invalid NamespaceMemberDecl"; break;
			case 211: s = "invalid OptionValue"; break;
			case 212: s = "invalid EndOfStmt"; break;
			case 213: s = "invalid TypeModifier"; break;
			case 214: s = "invalid NonModuleDeclaration"; break;
			case 215: s = "invalid NonModuleDeclaration"; break;
			case 216: s = "invalid Identifier"; break;
			case 217: s = "invalid TypeParameterConstraints"; break;
			case 218: s = "invalid NonArrayTypeName"; break;
			case 219: s = "invalid MemberModifier"; break;
			case 220: s = "invalid StructureMemberDecl"; break;
			case 221: s = "invalid StructureMemberDecl"; break;
			case 222: s = "invalid StructureMemberDecl"; break;
			case 223: s = "invalid StructureMemberDecl"; break;
			case 224: s = "invalid StructureMemberDecl"; break;
			case 225: s = "invalid StructureMemberDecl"; break;
			case 226: s = "invalid StructureMemberDecl"; break;
			case 227: s = "invalid InterfaceMemberDecl"; break;
			case 228: s = "invalid InterfaceMemberDecl"; break;
			case 229: s = "invalid Charset"; break;
			case 230: s = "invalid VariableDeclarator"; break;
			case 231: s = "invalid AccessorDecls"; break;
			case 232: s = "invalid EventAccessorDeclaration"; break;
			case 233: s = "invalid OverloadableOperator"; break;
			case 234: s = "invalid VariableInitializer"; break;
			case 235: s = "invalid EventMemberSpecifier"; break;
			case 236: s = "invalid EventMemberSpecifier"; break;
			case 237: s = "invalid SimpleExpr"; break;
			case 238: s = "invalid SimpleExpr"; break;
			case 239: s = "invalid SimpleExpr"; break;
			case 240: s = "invalid AssignmentOperator"; break;
			case 241: s = "invalid PrimitiveTypeName"; break;
			case 242: s = "invalid CastTarget"; break;
			case 243: s = "invalid InvocationOrMemberReferenceExpression"; break;
			case 244: s = "invalid InvocationExpression"; break;
			case 245: s = "invalid InvocationExpression"; break;
			case 246: s = "invalid RelationalExpr"; break;
			case 247: s = "invalid RelationalExpr"; break;
			case 248: s = "invalid Argument"; break;
			case 249: s = "invalid QualIdentAndTypeArguments"; break;
			case 250: s = "invalid AttributeArguments"; break;
			case 251: s = "invalid ParameterModifier"; break;
			case 252: s = "invalid Statement"; break;
			case 253: s = "invalid LabelName"; break;
			case 254: s = "invalid EmbeddedStatement"; break;
			case 255: s = "invalid EmbeddedStatement"; break;
			case 256: s = "invalid EmbeddedStatement"; break;
			case 257: s = "invalid EmbeddedStatement"; break;
			case 258: s = "invalid EmbeddedStatement"; break;
			case 259: s = "invalid EmbeddedStatement"; break;
			case 260: s = "invalid EmbeddedStatement"; break;
			case 261: s = "invalid WhileOrUntil"; break;
			case 262: s = "invalid OnErrorStatement"; break;
			case 263: s = "invalid ResumeStatement"; break;
			case 264: s = "invalid CaseClause"; break;
			case 265: s = "invalid CaseClause"; break;

			default: s = "error " + errorNumber; break;
		}
		errors.Error(line, col, s);
	}
	
	protected bool StartOf(int s)
	{
		return set[s, lexer.LookAhead.kind];
	}
	
	static bool[,] set = {
	{T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,T,T,x, x,x,T,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, T,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, T,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, T,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,T, x,T,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,T, T,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,x, T,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,T,x,x, x,T,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,x,x, x,x,T,x, x,x,x,x, x,x,x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,T, x,x,x,x, x,x,T,T, T,T,x,x, x,x,x,x, x,x,T,x, x,T,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,T,T,T, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, T,T,T,x, T,T,T,T, T,x,T,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,T,T, x,T,x,x, x,x,x,x, T,T,x,x, x,x,x,T, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, T,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, T,T,T,x, x,T,x,T, T,x,T,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,T, T,T,x,x, x,x,x,x, x,x,T,x, x,T,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,T,T,T, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, T,T,T,x, x,T,T,T, T,x,T,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,T,T,T, T,T,T,T, T,T,T,x, x,T,T,T, T,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,T, x,T,T,T, T,x,T,x, T,x,x,T, T,T,T,T, T,T,T,x, T,T,T,T, T,T,T,T, T,T,x,x, x,T,T,T, T,x,x,x, T,x,x,T, T,x,T,x, T,x,T,x, x,x,T,x, T,x,T,x, x,x,x,T, x,x,x,x, x,T,x,T, x,x,x,x, T,T,x,T, x,x,T,x, x,x,x,T, x,x,x,x, x,x,x,x, T,x,x,x, x,T,x,T, T,T,T,T, x,x,x,T, T,T,x,T, x,T,x,x, T,T,x,T, x,T,T,T, T,T,x,x, x,T,T,x, x,x,T,x, T,x,T,T, T,T,T,T, T,T,T,T, x,x,x,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, T,T,T,x, x,T,T,T, T,x,T,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,T,T,T, T,T,T,T, T,T,T,x, x,T,T,T, T,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,T, x,T,T,T, T,x,T,x, T,x,x,T, T,T,T,T, T,T,T,x, T,T,T,T, T,T,T,T, T,T,x,x, x,T,T,T, T,x,x,x, x,x,x,T, T,x,T,x, T,x,T,x, x,x,T,x, T,x,T,x, x,x,x,T, x,x,x,x, x,T,x,T, x,x,x,x, T,T,x,T, x,x,T,x, x,x,x,T, x,x,x,x, x,x,x,x, T,x,x,x, x,T,x,T, T,T,T,T, x,x,x,T, T,T,x,T, x,T,x,x, T,T,x,T, x,T,T,T, T,T,x,x, x,T,T,x, x,x,T,x, T,x,T,T, T,T,T,T, T,T,T,T, x,x,x,x, x,x,x},
	{x,T,x,x, x,x,x,x, x,x,x,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,T,T, T,T,T,T, T,T,T,x, x,x,T,T, T,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,T,T,T, T,x,T,x, x,x,x,T, T,T,T,T, T,T,T,x, T,T,T,x, T,T,T,T, T,T,x,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,T, x,x,x,x, T,T,x,T, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,T,x,x, x,T,x,x, x,T,x,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,T,T, T,T,T,T, x,x,x,x, x,x,x},
	{x,x,T,T, T,T,T,T, T,T,T,x, x,x,T,T, T,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,T,T,T, T,x,T,x, x,x,x,T, T,T,T,T, T,T,T,x, T,T,T,x, T,T,T,T, T,T,x,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,T, x,x,x,x, T,T,x,T, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,T,x,x, x,T,x,x, x,T,x,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,T,T, T,T,T,T, x,x,x,x, x,x,x},
	{x,x,T,T, T,T,T,T, T,T,T,x, x,x,T,T, T,x,x,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,T,T,T, T,x,T,x, x,x,x,T, T,T,T,T, T,T,T,x, T,T,T,x, T,T,T,T, T,T,x,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,T, x,x,x,x, T,T,x,T, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,T,x,x, x,T,x,x, x,T,x,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,T,T, T,T,T,T, x,x,x,x, x,x,x},
	{x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,T,T,T, T,x,T,x, x,x,x,T, T,T,T,T, T,T,T,x, T,T,T,x, T,T,T,T, T,T,x,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,T, x,x,x,x, T,T,x,T, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,T,x,x, x,T,x,x, x,T,x,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,T,T, T,T,T,T, x,x,x,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,T,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,T,T, T,T,T,T, T,T,T,x, x,x,T,T, T,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,T, x,T,T,T, T,x,T,x, T,x,x,T, T,T,T,T, T,T,T,x, T,T,T,x, T,T,T,T, T,T,x,x, x,x,T,T, T,x,x,x, x,x,x,T, T,x,T,x, T,x,T,x, x,x,T,x, T,x,T,x, x,x,x,T, x,x,x,x, x,T,x,T, x,x,x,x, T,T,x,T, x,x,T,x, x,x,x,T, x,x,x,x, x,x,x,x, T,x,x,x, x,T,x,T, T,T,T,T, x,x,x,T, T,x,x,T, x,T,x,x, T,T,x,T, x,T,T,T, T,T,x,x, x,T,T,x, x,x,T,x, T,x,T,T, T,T,T,T, T,T,T,T, x,x,x,x, x,x,x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,T,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x}

	};
} // end Parser

}