
#line  1 "cs.ATG" 
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Ast;
using ASTAttribute = ICSharpCode.NRefactory.Ast.Attribute;
using Types = ICSharpCode.NRefactory.Ast.ClassType;
/*
  Parser.frame file for NRefactory.
 */
using System;
using System.Reflection;

namespace ICSharpCode.NRefactory.Parser.CSharp {



partial class Parser : AbstractParser
{
	const int maxT = 141;

	const  bool   T            = true;
	const  bool   x            = false;
	

#line  18 "cs.ATG" 


/*

*/

	void CS() {

#line  175 "cs.ATG" 
		lexer.NextToken(); /* get the first token */ 
		while (la.kind == 121) {
			UsingDirective();
		}
		while (
#line  178 "cs.ATG" 
IsGlobalAttrTarget()) {
			GlobalAttributeSection();
		}
		while (StartOf(1)) {
			NamespaceMemberDecl();
		}
		Expect(0);
	}

	void UsingDirective() {

#line  185 "cs.ATG" 
		string qualident = null; TypeReference aliasedType = null;
		
		Expect(121);

#line  188 "cs.ATG" 
		Location startPos = t.Location; 
		Qualident(
#line  189 "cs.ATG" 
out qualident);
		if (la.kind == 3) {
			lexer.NextToken();
			NonArrayType(
#line  190 "cs.ATG" 
out aliasedType);
		}
		Expect(11);

#line  192 "cs.ATG" 
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

#line  208 "cs.ATG" 
		Location startPos = t.Location; 
		Identifier();

#line  209 "cs.ATG" 
		if (t.val != "assembly") Error("global attribute target specifier (\"assembly\") expected");
		string attributeTarget = t.val;
		List<ASTAttribute> attributes = new List<ASTAttribute>();
		ASTAttribute attribute;
		
		Expect(9);
		Attribute(
#line  214 "cs.ATG" 
out attribute);

#line  214 "cs.ATG" 
		attributes.Add(attribute); 
		while (
#line  215 "cs.ATG" 
NotFinalComma()) {
			Expect(14);
			Attribute(
#line  215 "cs.ATG" 
out attribute);

#line  215 "cs.ATG" 
			attributes.Add(attribute); 
		}
		if (la.kind == 14) {
			lexer.NextToken();
		}
		Expect(19);

#line  217 "cs.ATG" 
		AttributeSection section = new AttributeSection(attributeTarget, attributes);
		section.StartLocation = startPos;
		section.EndLocation = t.EndLocation;
		compilationUnit.AddChild(section);
		
	}

	void NamespaceMemberDecl() {

#line  308 "cs.ATG" 
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		ModifierList m = new ModifierList();
		string qualident;
		
		if (la.kind == 88) {
			lexer.NextToken();

#line  314 "cs.ATG" 
			Location startPos = t.Location; 
			Qualident(
#line  315 "cs.ATG" 
out qualident);

#line  315 "cs.ATG" 
			INode node =  new NamespaceDeclaration(qualident);
			node.StartLocation = startPos;
			compilationUnit.AddChild(node);
			compilationUnit.BlockStart(node);
			
			Expect(16);
			while (la.kind == 121) {
				UsingDirective();
			}
			while (StartOf(1)) {
				NamespaceMemberDecl();
			}
			Expect(17);
			if (la.kind == 11) {
				lexer.NextToken();
			}

#line  324 "cs.ATG" 
			node.EndLocation   = t.EndLocation;
			compilationUnit.BlockEnd();
			
		} else if (StartOf(2)) {
			while (la.kind == 18) {
				AttributeSection(
#line  328 "cs.ATG" 
out section);

#line  328 "cs.ATG" 
				attributes.Add(section); 
			}
			while (StartOf(3)) {
				TypeModifier(
#line  329 "cs.ATG" 
m);
			}
			TypeDecl(
#line  330 "cs.ATG" 
m, attributes);
		} else SynErr(142);
	}

	void Qualident(
#line  452 "cs.ATG" 
out string qualident) {
		Identifier();

#line  454 "cs.ATG" 
		qualidentBuilder.Length = 0; qualidentBuilder.Append(t.val); 
		while (
#line  455 "cs.ATG" 
DotAndIdent()) {
			Expect(15);
			Identifier();

#line  455 "cs.ATG" 
			qualidentBuilder.Append('.');
			qualidentBuilder.Append(t.val); 
			
		}

#line  458 "cs.ATG" 
		qualident = qualidentBuilder.ToString(); 
	}

	void NonArrayType(
#line  567 "cs.ATG" 
out TypeReference type) {

#line  569 "cs.ATG" 
		string name;
		int pointer = 0;
		type = null;
		
		if (StartOf(4)) {
			ClassType(
#line  574 "cs.ATG" 
out type, false);
		} else if (StartOf(5)) {
			SimpleType(
#line  575 "cs.ATG" 
out name);

#line  575 "cs.ATG" 
			type = new TypeReference(name); 
		} else if (la.kind == 123) {
			lexer.NextToken();
			Expect(6);

#line  576 "cs.ATG" 
			pointer = 1; type = new TypeReference("void"); 
		} else SynErr(143);
		if (la.kind == 12) {
			NullableQuestionMark(
#line  579 "cs.ATG" 
ref type);
		}
		while (
#line  581 "cs.ATG" 
IsPointer()) {
			Expect(6);

#line  582 "cs.ATG" 
			++pointer; 
		}

#line  584 "cs.ATG" 
		if (type != null) { type.PointerNestingLevel = pointer; } 
	}

	void Identifier() {
		switch (la.kind) {
		case 1: {
			lexer.NextToken();
			break;
		}
		case 126: {
			lexer.NextToken();
			break;
		}
		case 127: {
			lexer.NextToken();
			break;
		}
		case 128: {
			lexer.NextToken();
			break;
		}
		case 129: {
			lexer.NextToken();
			break;
		}
		case 130: {
			lexer.NextToken();
			break;
		}
		case 131: {
			lexer.NextToken();
			break;
		}
		case 132: {
			lexer.NextToken();
			break;
		}
		case 133: {
			lexer.NextToken();
			break;
		}
		case 134: {
			lexer.NextToken();
			break;
		}
		case 135: {
			lexer.NextToken();
			break;
		}
		case 136: {
			lexer.NextToken();
			break;
		}
		case 137: {
			lexer.NextToken();
			break;
		}
		case 138: {
			lexer.NextToken();
			break;
		}
		case 139: {
			lexer.NextToken();
			break;
		}
		case 140: {
			lexer.NextToken();
			break;
		}
		default: SynErr(144); break;
		}
	}

	void Attribute(
#line  224 "cs.ATG" 
out ASTAttribute attribute) {

#line  225 "cs.ATG" 
		string qualident;
		string alias = null;
		
		if (
#line  229 "cs.ATG" 
IdentAndDoubleColon()) {
			Identifier();

#line  230 "cs.ATG" 
			alias = t.val; 
			Expect(10);
		}
		Qualident(
#line  233 "cs.ATG" 
out qualident);

#line  234 "cs.ATG" 
		List<Expression> positional = new List<Expression>();
		List<NamedArgumentExpression> named = new List<NamedArgumentExpression>();
		string name = (alias != null && alias != "global") ? alias + "." + qualident : qualident;
		
		if (la.kind == 20) {
			AttributeArguments(
#line  238 "cs.ATG" 
positional, named);
		}

#line  238 "cs.ATG" 
		attribute  = new ASTAttribute(name, positional, named);
	}

	void AttributeArguments(
#line  241 "cs.ATG" 
List<Expression> positional, List<NamedArgumentExpression> named) {

#line  243 "cs.ATG" 
		bool nameFound = false;
		string name = "";
		Expression expr;
		
		Expect(20);
		if (StartOf(6)) {
			if (
#line  251 "cs.ATG" 
IsAssignment()) {

#line  251 "cs.ATG" 
				nameFound = true; 
				Identifier();

#line  252 "cs.ATG" 
				name = t.val; 
				Expect(3);
			}
			Expr(
#line  254 "cs.ATG" 
out expr);

#line  254 "cs.ATG" 
			if (expr != null) {if(name == "") positional.Add(expr);
			else { named.Add(new NamedArgumentExpression(name, expr)); name = ""; }
			}
			
			while (la.kind == 14) {
				lexer.NextToken();
				if (
#line  262 "cs.ATG" 
IsAssignment()) {

#line  262 "cs.ATG" 
					nameFound = true; 
					Identifier();

#line  263 "cs.ATG" 
					name = t.val; 
					Expect(3);
				} else if (StartOf(6)) {

#line  265 "cs.ATG" 
					if (nameFound) Error("no positional argument after named argument"); 
				} else SynErr(145);
				Expr(
#line  266 "cs.ATG" 
out expr);

#line  266 "cs.ATG" 
				if (expr != null) { if(name == "") positional.Add(expr);
				else { named.Add(new NamedArgumentExpression(name, expr)); name = ""; }
				}
				
			}
		}
		Expect(21);
	}

	void Expr(
#line  1649 "cs.ATG" 
out Expression expr) {

#line  1650 "cs.ATG" 
		expr = null; Expression expr1 = null, expr2 = null; AssignmentOperatorType op; 
		UnaryExpr(
#line  1652 "cs.ATG" 
out expr);
		if (StartOf(7)) {
			AssignmentOperator(
#line  1655 "cs.ATG" 
out op);
			Expr(
#line  1655 "cs.ATG" 
out expr1);

#line  1655 "cs.ATG" 
			expr = new AssignmentExpression(expr, op, expr1); 
		} else if (
#line  1656 "cs.ATG" 
la.kind == Tokens.GreaterThan && Peek(1).kind == Tokens.GreaterEqual) {
			AssignmentOperator(
#line  1657 "cs.ATG" 
out op);
			Expr(
#line  1657 "cs.ATG" 
out expr1);

#line  1657 "cs.ATG" 
			expr = new AssignmentExpression(expr, op, expr1); 
		} else if (StartOf(8)) {
			ConditionalOrExpr(
#line  1659 "cs.ATG" 
ref expr);
			if (la.kind == 13) {
				lexer.NextToken();
				Expr(
#line  1660 "cs.ATG" 
out expr1);

#line  1660 "cs.ATG" 
				expr = new BinaryOperatorExpression(expr, BinaryOperatorType.NullCoalescing, expr1); 
			}
			if (la.kind == 12) {
				lexer.NextToken();
				Expr(
#line  1661 "cs.ATG" 
out expr1);
				Expect(9);
				Expr(
#line  1661 "cs.ATG" 
out expr2);

#line  1661 "cs.ATG" 
				expr = new ConditionalExpression(expr, expr1, expr2);  
			}
		} else SynErr(146);
	}

	void AttributeSection(
#line  275 "cs.ATG" 
out AttributeSection section) {

#line  277 "cs.ATG" 
		string attributeTarget = "";
		List<ASTAttribute> attributes = new List<ASTAttribute>();
		ASTAttribute attribute;
		
		
		Expect(18);

#line  283 "cs.ATG" 
		Location startPos = t.Location; 
		if (
#line  284 "cs.ATG" 
IsLocalAttrTarget()) {
			if (la.kind == 69) {
				lexer.NextToken();

#line  285 "cs.ATG" 
				attributeTarget = "event";
			} else if (la.kind == 101) {
				lexer.NextToken();

#line  286 "cs.ATG" 
				attributeTarget = "return";
			} else {
				Identifier();

#line  287 "cs.ATG" 
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
#line  297 "cs.ATG" 
out attribute);

#line  297 "cs.ATG" 
		attributes.Add(attribute); 
		while (
#line  298 "cs.ATG" 
NotFinalComma()) {
			Expect(14);
			Attribute(
#line  298 "cs.ATG" 
out attribute);

#line  298 "cs.ATG" 
			attributes.Add(attribute); 
		}
		if (la.kind == 14) {
			lexer.NextToken();
		}
		Expect(19);

#line  300 "cs.ATG" 
		section = new AttributeSection(attributeTarget, attributes);
		section.StartLocation = startPos;
		section.EndLocation = t.EndLocation;
		
	}

	void TypeModifier(
#line  654 "cs.ATG" 
ModifierList m) {
		switch (la.kind) {
		case 89: {
			lexer.NextToken();

#line  656 "cs.ATG" 
			m.Add(Modifiers.New, t.Location); 
			break;
		}
		case 98: {
			lexer.NextToken();

#line  657 "cs.ATG" 
			m.Add(Modifiers.Public, t.Location); 
			break;
		}
		case 97: {
			lexer.NextToken();

#line  658 "cs.ATG" 
			m.Add(Modifiers.Protected, t.Location); 
			break;
		}
		case 84: {
			lexer.NextToken();

#line  659 "cs.ATG" 
			m.Add(Modifiers.Internal, t.Location); 
			break;
		}
		case 96: {
			lexer.NextToken();

#line  660 "cs.ATG" 
			m.Add(Modifiers.Private, t.Location); 
			break;
		}
		case 119: {
			lexer.NextToken();

#line  661 "cs.ATG" 
			m.Add(Modifiers.Unsafe, t.Location); 
			break;
		}
		case 49: {
			lexer.NextToken();

#line  662 "cs.ATG" 
			m.Add(Modifiers.Abstract, t.Location); 
			break;
		}
		case 103: {
			lexer.NextToken();

#line  663 "cs.ATG" 
			m.Add(Modifiers.Sealed, t.Location); 
			break;
		}
		case 107: {
			lexer.NextToken();

#line  664 "cs.ATG" 
			m.Add(Modifiers.Static, t.Location); 
			break;
		}
		case 126: {
			lexer.NextToken();

#line  665 "cs.ATG" 
			m.Add(Modifiers.Partial, t.Location); 
			break;
		}
		default: SynErr(147); break;
		}
	}

	void TypeDecl(
#line  333 "cs.ATG" 
ModifierList m, List<AttributeSection> attributes) {

#line  335 "cs.ATG" 
		TypeReference type;
		List<TypeReference> names;
		List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
		string name;
		List<TemplateDefinition> templates;
		
		if (la.kind == 59) {

#line  341 "cs.ATG" 
			m.Check(Modifiers.Classes); 
			lexer.NextToken();

#line  342 "cs.ATG" 
			TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
			templates = newType.Templates;
			compilationUnit.AddChild(newType);
			compilationUnit.BlockStart(newType);
			newType.StartLocation = m.GetDeclarationLocation(t.Location);
			
			newType.Type = Types.Class;
			
			Identifier();

#line  350 "cs.ATG" 
			newType.Name = t.val; 
			if (la.kind == 23) {
				TypeParameterList(
#line  353 "cs.ATG" 
templates);
			}
			if (la.kind == 9) {
				ClassBase(
#line  355 "cs.ATG" 
out names);

#line  355 "cs.ATG" 
				newType.BaseTypes = names; 
			}
			while (la.kind == 127) {
				TypeParameterConstraintsClause(
#line  358 "cs.ATG" 
templates);
			}

#line  360 "cs.ATG" 
			newType.BodyStartLocation = t.EndLocation; 
			ClassBody();
			if (la.kind == 11) {
				lexer.NextToken();
			}

#line  362 "cs.ATG" 
			newType.EndLocation = t.Location; 
			compilationUnit.BlockEnd();
			
		} else if (StartOf(9)) {

#line  365 "cs.ATG" 
			m.Check(Modifiers.StructsInterfacesEnumsDelegates); 
			if (la.kind == 109) {
				lexer.NextToken();

#line  366 "cs.ATG" 
				TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
				templates = newType.Templates;
				newType.StartLocation = m.GetDeclarationLocation(t.Location);
				compilationUnit.AddChild(newType);
				compilationUnit.BlockStart(newType);
				newType.Type = Types.Struct; 
				
				Identifier();

#line  373 "cs.ATG" 
				newType.Name = t.val; 
				if (la.kind == 23) {
					TypeParameterList(
#line  376 "cs.ATG" 
templates);
				}
				if (la.kind == 9) {
					StructInterfaces(
#line  378 "cs.ATG" 
out names);

#line  378 "cs.ATG" 
					newType.BaseTypes = names; 
				}
				while (la.kind == 127) {
					TypeParameterConstraintsClause(
#line  381 "cs.ATG" 
templates);
				}

#line  384 "cs.ATG" 
				newType.BodyStartLocation = t.EndLocation; 
				StructBody();
				if (la.kind == 11) {
					lexer.NextToken();
				}

#line  386 "cs.ATG" 
				newType.EndLocation = t.Location; 
				compilationUnit.BlockEnd();
				
			} else if (la.kind == 83) {
				lexer.NextToken();

#line  390 "cs.ATG" 
				TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
				templates = newType.Templates;
				compilationUnit.AddChild(newType);
				compilationUnit.BlockStart(newType);
				newType.StartLocation = m.GetDeclarationLocation(t.Location);
				newType.Type = Types.Interface;
				
				Identifier();

#line  397 "cs.ATG" 
				newType.Name = t.val; 
				if (la.kind == 23) {
					TypeParameterList(
#line  400 "cs.ATG" 
templates);
				}
				if (la.kind == 9) {
					InterfaceBase(
#line  402 "cs.ATG" 
out names);

#line  402 "cs.ATG" 
					newType.BaseTypes = names; 
				}
				while (la.kind == 127) {
					TypeParameterConstraintsClause(
#line  405 "cs.ATG" 
templates);
				}

#line  407 "cs.ATG" 
				newType.BodyStartLocation = t.EndLocation; 
				InterfaceBody();
				if (la.kind == 11) {
					lexer.NextToken();
				}

#line  409 "cs.ATG" 
				newType.EndLocation = t.Location; 
				compilationUnit.BlockEnd();
				
			} else if (la.kind == 68) {
				lexer.NextToken();

#line  413 "cs.ATG" 
				TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
				compilationUnit.AddChild(newType);
				compilationUnit.BlockStart(newType);
				newType.StartLocation = m.GetDeclarationLocation(t.Location);
				newType.Type = Types.Enum;
				
				Identifier();

#line  419 "cs.ATG" 
				newType.Name = t.val; 
				if (la.kind == 9) {
					lexer.NextToken();
					IntegralType(
#line  420 "cs.ATG" 
out name);

#line  420 "cs.ATG" 
					newType.BaseTypes.Add(new TypeReference(name)); 
				}

#line  422 "cs.ATG" 
				newType.BodyStartLocation = t.EndLocation; 
				EnumBody();
				if (la.kind == 11) {
					lexer.NextToken();
				}

#line  424 "cs.ATG" 
				newType.EndLocation = t.Location; 
				compilationUnit.BlockEnd();
				
			} else {
				lexer.NextToken();

#line  428 "cs.ATG" 
				DelegateDeclaration delegateDeclr = new DelegateDeclaration(m.Modifier, attributes);
				templates = delegateDeclr.Templates;
				delegateDeclr.StartLocation = m.GetDeclarationLocation(t.Location);
				
				if (
#line  432 "cs.ATG" 
NotVoidPointer()) {
					Expect(123);

#line  432 "cs.ATG" 
					delegateDeclr.ReturnType = new TypeReference("void", 0, null); 
				} else if (StartOf(10)) {
					Type(
#line  433 "cs.ATG" 
out type);

#line  433 "cs.ATG" 
					delegateDeclr.ReturnType = type; 
				} else SynErr(148);
				Identifier();

#line  435 "cs.ATG" 
				delegateDeclr.Name = t.val; 
				if (la.kind == 23) {
					TypeParameterList(
#line  438 "cs.ATG" 
templates);
				}
				Expect(20);
				if (StartOf(11)) {
					FormalParameterList(
#line  440 "cs.ATG" 
p);

#line  440 "cs.ATG" 
					delegateDeclr.Parameters = p; 
				}
				Expect(21);
				while (la.kind == 127) {
					TypeParameterConstraintsClause(
#line  444 "cs.ATG" 
templates);
				}
				Expect(11);

#line  446 "cs.ATG" 
				delegateDeclr.EndLocation = t.Location;
				compilationUnit.AddChild(delegateDeclr);
				
			}
		} else SynErr(149);
	}

	void TypeParameterList(
#line  2153 "cs.ATG" 
List<TemplateDefinition> templates) {

#line  2155 "cs.ATG" 
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		
		Expect(23);
		while (la.kind == 18) {
			AttributeSection(
#line  2159 "cs.ATG" 
out section);

#line  2159 "cs.ATG" 
			attributes.Add(section); 
		}
		Identifier();

#line  2160 "cs.ATG" 
		templates.Add(new TemplateDefinition(t.val, attributes)); 
		while (la.kind == 14) {
			lexer.NextToken();
			while (la.kind == 18) {
				AttributeSection(
#line  2161 "cs.ATG" 
out section);

#line  2161 "cs.ATG" 
				attributes.Add(section); 
			}
			Identifier();

#line  2162 "cs.ATG" 
			templates.Add(new TemplateDefinition(t.val, attributes)); 
		}
		Expect(22);
	}

	void ClassBase(
#line  461 "cs.ATG" 
out List<TypeReference> names) {

#line  463 "cs.ATG" 
		TypeReference typeRef;
		names = new List<TypeReference>();
		
		Expect(9);
		ClassType(
#line  467 "cs.ATG" 
out typeRef, false);

#line  467 "cs.ATG" 
		if (typeRef != null) { names.Add(typeRef); } 
		while (la.kind == 14) {
			lexer.NextToken();
			TypeName(
#line  468 "cs.ATG" 
out typeRef, false);

#line  468 "cs.ATG" 
			if (typeRef != null) { names.Add(typeRef); } 
		}
	}

	void TypeParameterConstraintsClause(
#line  2166 "cs.ATG" 
List<TemplateDefinition> templates) {

#line  2167 "cs.ATG" 
		string name = ""; TypeReference type; 
		Expect(127);
		Identifier();

#line  2170 "cs.ATG" 
		name = t.val; 
		Expect(9);
		TypeParameterConstraintsClauseBase(
#line  2172 "cs.ATG" 
out type);

#line  2173 "cs.ATG" 
		TemplateDefinition td = null;
		foreach (TemplateDefinition d in templates) {
			if (d.Name == name) {
				td = d;
				break;
			}
		}
		if ( td != null && type != null) { td.Bases.Add(type); }
		
		while (la.kind == 14) {
			lexer.NextToken();
			TypeParameterConstraintsClauseBase(
#line  2182 "cs.ATG" 
out type);

#line  2183 "cs.ATG" 
			td = null;
			foreach (TemplateDefinition d in templates) {
				if (d.Name == name) {
					td = d;
					break;
				}
			}
			if ( td != null && type != null) { td.Bases.Add(type); }
			
		}
	}

	void ClassBody() {

#line  472 "cs.ATG" 
		AttributeSection section; 
		Expect(16);
		while (StartOf(12)) {

#line  475 "cs.ATG" 
			List<AttributeSection> attributes = new List<AttributeSection>();
			ModifierList m = new ModifierList();
			
			while (la.kind == 18) {
				AttributeSection(
#line  478 "cs.ATG" 
out section);

#line  478 "cs.ATG" 
				attributes.Add(section); 
			}
			MemberModifiers(
#line  479 "cs.ATG" 
m);
			ClassMemberDecl(
#line  480 "cs.ATG" 
m, attributes);
		}
		Expect(17);
	}

	void StructInterfaces(
#line  485 "cs.ATG" 
out List<TypeReference> names) {

#line  487 "cs.ATG" 
		TypeReference typeRef;
		names = new List<TypeReference>();
		
		Expect(9);
		TypeName(
#line  491 "cs.ATG" 
out typeRef, false);

#line  491 "cs.ATG" 
		if (typeRef != null) { names.Add(typeRef); } 
		while (la.kind == 14) {
			lexer.NextToken();
			TypeName(
#line  492 "cs.ATG" 
out typeRef, false);

#line  492 "cs.ATG" 
			if (typeRef != null) { names.Add(typeRef); } 
		}
	}

	void StructBody() {

#line  496 "cs.ATG" 
		AttributeSection section; 
		Expect(16);
		while (StartOf(13)) {

#line  499 "cs.ATG" 
			List<AttributeSection> attributes = new List<AttributeSection>();
			ModifierList m = new ModifierList();
			
			while (la.kind == 18) {
				AttributeSection(
#line  502 "cs.ATG" 
out section);

#line  502 "cs.ATG" 
				attributes.Add(section); 
			}
			MemberModifiers(
#line  503 "cs.ATG" 
m);
			StructMemberDecl(
#line  504 "cs.ATG" 
m, attributes);
		}
		Expect(17);
	}

	void InterfaceBase(
#line  509 "cs.ATG" 
out List<TypeReference> names) {

#line  511 "cs.ATG" 
		TypeReference typeRef;
		names = new List<TypeReference>();
		
		Expect(9);
		TypeName(
#line  515 "cs.ATG" 
out typeRef, false);

#line  515 "cs.ATG" 
		if (typeRef != null) { names.Add(typeRef); } 
		while (la.kind == 14) {
			lexer.NextToken();
			TypeName(
#line  516 "cs.ATG" 
out typeRef, false);

#line  516 "cs.ATG" 
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
#line  676 "cs.ATG" 
out string name) {

#line  676 "cs.ATG" 
		name = ""; 
		switch (la.kind) {
		case 102: {
			lexer.NextToken();

#line  678 "cs.ATG" 
			name = "sbyte"; 
			break;
		}
		case 54: {
			lexer.NextToken();

#line  679 "cs.ATG" 
			name = "byte"; 
			break;
		}
		case 104: {
			lexer.NextToken();

#line  680 "cs.ATG" 
			name = "short"; 
			break;
		}
		case 120: {
			lexer.NextToken();

#line  681 "cs.ATG" 
			name = "ushort"; 
			break;
		}
		case 82: {
			lexer.NextToken();

#line  682 "cs.ATG" 
			name = "int"; 
			break;
		}
		case 116: {
			lexer.NextToken();

#line  683 "cs.ATG" 
			name = "uint"; 
			break;
		}
		case 87: {
			lexer.NextToken();

#line  684 "cs.ATG" 
			name = "long"; 
			break;
		}
		case 117: {
			lexer.NextToken();

#line  685 "cs.ATG" 
			name = "ulong"; 
			break;
		}
		case 57: {
			lexer.NextToken();

#line  686 "cs.ATG" 
			name = "char"; 
			break;
		}
		default: SynErr(150); break;
		}
	}

	void EnumBody() {

#line  525 "cs.ATG" 
		FieldDeclaration f; 
		Expect(16);
		if (StartOf(15)) {
			EnumMemberDecl(
#line  528 "cs.ATG" 
out f);

#line  528 "cs.ATG" 
			compilationUnit.AddChild(f); 
			while (
#line  529 "cs.ATG" 
NotFinalComma()) {
				Expect(14);
				EnumMemberDecl(
#line  530 "cs.ATG" 
out f);

#line  530 "cs.ATG" 
				compilationUnit.AddChild(f); 
			}
			if (la.kind == 14) {
				lexer.NextToken();
			}
		}
		Expect(17);
	}

	void Type(
#line  535 "cs.ATG" 
out TypeReference type) {
		TypeWithRestriction(
#line  537 "cs.ATG" 
out type, true, false);
	}

	void FormalParameterList(
#line  598 "cs.ATG" 
List<ParameterDeclarationExpression> parameter) {

#line  601 "cs.ATG" 
		ParameterDeclarationExpression p;
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		
		while (la.kind == 18) {
			AttributeSection(
#line  606 "cs.ATG" 
out section);

#line  606 "cs.ATG" 
			attributes.Add(section); 
		}
		if (StartOf(16)) {
			FixedParameter(
#line  608 "cs.ATG" 
out p);

#line  608 "cs.ATG" 
			bool paramsFound = false;
			p.Attributes = attributes;
			parameter.Add(p);
			
			while (la.kind == 14) {
				lexer.NextToken();

#line  613 "cs.ATG" 
				attributes = new List<AttributeSection>(); if (paramsFound) Error("params array must be at end of parameter list"); 
				while (la.kind == 18) {
					AttributeSection(
#line  614 "cs.ATG" 
out section);

#line  614 "cs.ATG" 
					attributes.Add(section); 
				}
				if (StartOf(16)) {
					FixedParameter(
#line  616 "cs.ATG" 
out p);

#line  616 "cs.ATG" 
					p.Attributes = attributes; parameter.Add(p); 
				} else if (la.kind == 95) {
					ParameterArray(
#line  617 "cs.ATG" 
out p);

#line  617 "cs.ATG" 
					paramsFound = true; p.Attributes = attributes; parameter.Add(p); 
				} else SynErr(151);
			}
		} else if (la.kind == 95) {
			ParameterArray(
#line  620 "cs.ATG" 
out p);

#line  620 "cs.ATG" 
			p.Attributes = attributes; parameter.Add(p); 
		} else SynErr(152);
	}

	void ClassType(
#line  668 "cs.ATG" 
out TypeReference typeRef, bool canBeUnbound) {

#line  669 "cs.ATG" 
		TypeReference r; typeRef = null; 
		if (StartOf(17)) {
			TypeName(
#line  671 "cs.ATG" 
out r, canBeUnbound);

#line  671 "cs.ATG" 
			typeRef = r; 
		} else if (la.kind == 91) {
			lexer.NextToken();

#line  672 "cs.ATG" 
			typeRef = new TypeReference("object"); 
		} else if (la.kind == 108) {
			lexer.NextToken();

#line  673 "cs.ATG" 
			typeRef = new TypeReference("string"); 
		} else SynErr(153);
	}

	void TypeName(
#line  2096 "cs.ATG" 
out TypeReference typeRef, bool canBeUnbound) {

#line  2097 "cs.ATG" 
		List<TypeReference> typeArguments = null;
		string alias = null;
		string qualident;
		
		if (
#line  2102 "cs.ATG" 
IdentAndDoubleColon()) {
			Identifier();

#line  2103 "cs.ATG" 
			alias = t.val; 
			Expect(10);
		}
		Qualident(
#line  2106 "cs.ATG" 
out qualident);
		if (la.kind == 23) {
			TypeArgumentList(
#line  2107 "cs.ATG" 
out typeArguments, canBeUnbound);
		}

#line  2109 "cs.ATG" 
		if (alias == null) {
		typeRef = new TypeReference(qualident, typeArguments);
		} else if (alias == "global") {
			typeRef = new TypeReference(qualident, typeArguments);
			typeRef.IsGlobal = true;
		} else {
			typeRef = new TypeReference(alias + "." + qualident, typeArguments);
		}
		
		while (
#line  2118 "cs.ATG" 
DotAndIdent()) {
			Expect(15);

#line  2119 "cs.ATG" 
			typeArguments = null; 
			Qualident(
#line  2120 "cs.ATG" 
out qualident);
			if (la.kind == 23) {
				TypeArgumentList(
#line  2121 "cs.ATG" 
out typeArguments, canBeUnbound);
			}

#line  2122 "cs.ATG" 
			typeRef = new InnerClassTypeReference(typeRef, qualident, typeArguments); 
		}
	}

	void MemberModifiers(
#line  689 "cs.ATG" 
ModifierList m) {
		while (StartOf(18)) {
			switch (la.kind) {
			case 49: {
				lexer.NextToken();

#line  692 "cs.ATG" 
				m.Add(Modifiers.Abstract, t.Location); 
				break;
			}
			case 71: {
				lexer.NextToken();

#line  693 "cs.ATG" 
				m.Add(Modifiers.Extern, t.Location); 
				break;
			}
			case 84: {
				lexer.NextToken();

#line  694 "cs.ATG" 
				m.Add(Modifiers.Internal, t.Location); 
				break;
			}
			case 89: {
				lexer.NextToken();

#line  695 "cs.ATG" 
				m.Add(Modifiers.New, t.Location); 
				break;
			}
			case 94: {
				lexer.NextToken();

#line  696 "cs.ATG" 
				m.Add(Modifiers.Override, t.Location); 
				break;
			}
			case 96: {
				lexer.NextToken();

#line  697 "cs.ATG" 
				m.Add(Modifiers.Private, t.Location); 
				break;
			}
			case 97: {
				lexer.NextToken();

#line  698 "cs.ATG" 
				m.Add(Modifiers.Protected, t.Location); 
				break;
			}
			case 98: {
				lexer.NextToken();

#line  699 "cs.ATG" 
				m.Add(Modifiers.Public, t.Location); 
				break;
			}
			case 99: {
				lexer.NextToken();

#line  700 "cs.ATG" 
				m.Add(Modifiers.ReadOnly, t.Location); 
				break;
			}
			case 103: {
				lexer.NextToken();

#line  701 "cs.ATG" 
				m.Add(Modifiers.Sealed, t.Location); 
				break;
			}
			case 107: {
				lexer.NextToken();

#line  702 "cs.ATG" 
				m.Add(Modifiers.Static, t.Location); 
				break;
			}
			case 74: {
				lexer.NextToken();

#line  703 "cs.ATG" 
				m.Add(Modifiers.Fixed, t.Location); 
				break;
			}
			case 119: {
				lexer.NextToken();

#line  704 "cs.ATG" 
				m.Add(Modifiers.Unsafe, t.Location); 
				break;
			}
			case 122: {
				lexer.NextToken();

#line  705 "cs.ATG" 
				m.Add(Modifiers.Virtual, t.Location); 
				break;
			}
			case 124: {
				lexer.NextToken();

#line  706 "cs.ATG" 
				m.Add(Modifiers.Volatile, t.Location); 
				break;
			}
			case 126: {
				lexer.NextToken();

#line  707 "cs.ATG" 
				m.Add(Modifiers.Partial, t.Location); 
				break;
			}
			}
		}
	}

	void ClassMemberDecl(
#line  1005 "cs.ATG" 
ModifierList m, List<AttributeSection> attributes) {

#line  1006 "cs.ATG" 
		Statement stmt = null; 
		if (StartOf(19)) {
			StructMemberDecl(
#line  1008 "cs.ATG" 
m, attributes);
		} else if (la.kind == 27) {

#line  1009 "cs.ATG" 
			m.Check(Modifiers.Destructors); Location startPos = t.Location; 
			lexer.NextToken();
			Identifier();

#line  1010 "cs.ATG" 
			DestructorDeclaration d = new DestructorDeclaration(t.val, m.Modifier, attributes); 
			d.Modifier = m.Modifier;
			d.StartLocation = m.GetDeclarationLocation(startPos);
			
			Expect(20);
			Expect(21);

#line  1014 "cs.ATG" 
			d.EndLocation = t.EndLocation; 
			if (la.kind == 16) {
				Block(
#line  1014 "cs.ATG" 
out stmt);
			} else if (la.kind == 11) {
				lexer.NextToken();
			} else SynErr(154);

#line  1015 "cs.ATG" 
			d.Body = (BlockStatement)stmt;
			compilationUnit.AddChild(d);
			
		} else SynErr(155);
	}

	void StructMemberDecl(
#line  711 "cs.ATG" 
ModifierList m, List<AttributeSection> attributes) {

#line  713 "cs.ATG" 
		string qualident = null;
		TypeReference type;
		Expression expr;
		List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
		Statement stmt = null;
		List<VariableDeclaration> variableDeclarators = new List<VariableDeclaration>();
		List<TemplateDefinition> templates = new List<TemplateDefinition>();
		TypeReference explicitInterface = null;
		bool isExtensionMethod = false;
		
		if (la.kind == 60) {

#line  724 "cs.ATG" 
			m.Check(Modifiers.Constants); 
			lexer.NextToken();

#line  725 "cs.ATG" 
			Location startPos = t.Location; 
			Type(
#line  726 "cs.ATG" 
out type);
			Identifier();

#line  726 "cs.ATG" 
			FieldDeclaration fd = new FieldDeclaration(attributes, type, m.Modifier | Modifiers.Const);
			fd.StartLocation = m.GetDeclarationLocation(startPos);
			VariableDeclaration f = new VariableDeclaration(t.val);
			fd.Fields.Add(f);
			
			Expect(3);
			Expr(
#line  731 "cs.ATG" 
out expr);

#line  731 "cs.ATG" 
			f.Initializer = expr; 
			while (la.kind == 14) {
				lexer.NextToken();
				Identifier();

#line  732 "cs.ATG" 
				f = new VariableDeclaration(t.val);
				fd.Fields.Add(f);
				
				Expect(3);
				Expr(
#line  735 "cs.ATG" 
out expr);

#line  735 "cs.ATG" 
				f.Initializer = expr; 
			}
			Expect(11);

#line  736 "cs.ATG" 
			fd.EndLocation = t.EndLocation; compilationUnit.AddChild(fd); 
		} else if (
#line  740 "cs.ATG" 
NotVoidPointer()) {

#line  740 "cs.ATG" 
			m.Check(Modifiers.PropertysEventsMethods); 
			Expect(123);

#line  741 "cs.ATG" 
			Location startPos = t.Location; 
			if (
#line  742 "cs.ATG" 
IsExplicitInterfaceImplementation()) {
				TypeName(
#line  743 "cs.ATG" 
out explicitInterface, false);

#line  744 "cs.ATG" 
				if (la.kind != Tokens.Dot || Peek(1).kind != Tokens.This) {
				qualident = TypeReference.StripLastIdentifierFromType(ref explicitInterface);
				 } 
			} else if (StartOf(17)) {
				Identifier();

#line  747 "cs.ATG" 
				qualident = t.val; 
			} else SynErr(156);
			if (la.kind == 23) {
				TypeParameterList(
#line  750 "cs.ATG" 
templates);
			}
			Expect(20);
			if (la.kind == 111) {
				lexer.NextToken();

#line  753 "cs.ATG" 
				isExtensionMethod = true; /* C# 3.0 */ 
			}
			if (StartOf(11)) {
				FormalParameterList(
#line  754 "cs.ATG" 
p);
			}
			Expect(21);

#line  755 "cs.ATG" 
			MethodDeclaration methodDeclaration = new MethodDeclaration(qualident,
			                                                         m.Modifier,
			                                                         new TypeReference("void"),
			                                                         p,
			                                                         attributes);
			methodDeclaration.StartLocation = m.GetDeclarationLocation(startPos);
			methodDeclaration.EndLocation   = t.EndLocation;
			methodDeclaration.Templates = templates;
			methodDeclaration.IsExtensionMethod = isExtensionMethod;
			if (explicitInterface != null)
			methodDeclaration.InterfaceImplementations.Add(new InterfaceImplementation(explicitInterface, qualident));
			compilationUnit.AddChild(methodDeclaration);
			compilationUnit.BlockStart(methodDeclaration);
			
			while (la.kind == 127) {
				TypeParameterConstraintsClause(
#line  771 "cs.ATG" 
templates);
			}
			if (la.kind == 16) {
				Block(
#line  773 "cs.ATG" 
out stmt);
			} else if (la.kind == 11) {
				lexer.NextToken();
			} else SynErr(157);

#line  773 "cs.ATG" 
			compilationUnit.BlockEnd();
			methodDeclaration.Body  = (BlockStatement)stmt;
			
		} else if (la.kind == 69) {

#line  777 "cs.ATG" 
			m.Check(Modifiers.PropertysEventsMethods); 
			lexer.NextToken();

#line  778 "cs.ATG" 
			EventDeclaration eventDecl = new EventDeclaration(null, null, m.Modifier, attributes, null);
			eventDecl.StartLocation = t.Location;
			compilationUnit.AddChild(eventDecl);
			compilationUnit.BlockStart(eventDecl);
			EventAddRegion addBlock = null;
			EventRemoveRegion removeBlock = null;
			
			Type(
#line  785 "cs.ATG" 
out type);

#line  785 "cs.ATG" 
			eventDecl.TypeReference = type; 
			if (
#line  786 "cs.ATG" 
IsExplicitInterfaceImplementation()) {
				TypeName(
#line  787 "cs.ATG" 
out explicitInterface, false);

#line  788 "cs.ATG" 
				qualident = TypeReference.StripLastIdentifierFromType(ref explicitInterface); 

#line  789 "cs.ATG" 
				eventDecl.InterfaceImplementations.Add(new InterfaceImplementation(explicitInterface, qualident)); 
			} else if (StartOf(17)) {
				Identifier();

#line  791 "cs.ATG" 
				qualident = t.val; 
			} else SynErr(158);

#line  793 "cs.ATG" 
			eventDecl.Name = qualident; eventDecl.EndLocation = t.EndLocation; 
			if (la.kind == 3) {
				lexer.NextToken();
				Expr(
#line  794 "cs.ATG" 
out expr);

#line  794 "cs.ATG" 
				eventDecl.Initializer = expr; 
			}
			if (la.kind == 16) {
				lexer.NextToken();

#line  795 "cs.ATG" 
				eventDecl.BodyStart = t.Location; 
				EventAccessorDecls(
#line  796 "cs.ATG" 
out addBlock, out removeBlock);
				Expect(17);

#line  797 "cs.ATG" 
				eventDecl.BodyEnd   = t.EndLocation; 
			}
			if (la.kind == 11) {
				lexer.NextToken();
			}

#line  800 "cs.ATG" 
			compilationUnit.BlockEnd();
			eventDecl.AddRegion = addBlock;
			eventDecl.RemoveRegion = removeBlock;
			
		} else if (
#line  806 "cs.ATG" 
IdentAndLPar()) {

#line  806 "cs.ATG" 
			m.Check(Modifiers.Constructors | Modifiers.StaticConstructors); 
			Identifier();

#line  807 "cs.ATG" 
			string name = t.val; Location startPos = t.Location; 
			Expect(20);
			if (StartOf(11)) {

#line  807 "cs.ATG" 
				m.Check(Modifiers.Constructors); 
				FormalParameterList(
#line  808 "cs.ATG" 
p);
			}
			Expect(21);

#line  810 "cs.ATG" 
			ConstructorInitializer init = null;  
			if (la.kind == 9) {

#line  811 "cs.ATG" 
				m.Check(Modifiers.Constructors); 
				ConstructorInitializer(
#line  812 "cs.ATG" 
out init);
			}

#line  814 "cs.ATG" 
			ConstructorDeclaration cd = new ConstructorDeclaration(name, m.Modifier, p, init, attributes); 
			cd.StartLocation = startPos;
			cd.EndLocation   = t.EndLocation;
			
			if (la.kind == 16) {
				Block(
#line  819 "cs.ATG" 
out stmt);
			} else if (la.kind == 11) {
				lexer.NextToken();
			} else SynErr(159);

#line  819 "cs.ATG" 
			cd.Body = (BlockStatement)stmt; compilationUnit.AddChild(cd); 
		} else if (la.kind == 70 || la.kind == 80) {

#line  822 "cs.ATG" 
			m.Check(Modifiers.Operators);
			if (m.isNone) Error("at least one modifier must be set"); 
			bool isImplicit = true;
			Location startPos = Location.Empty;
			
			if (la.kind == 80) {
				lexer.NextToken();

#line  827 "cs.ATG" 
				startPos = t.Location; 
			} else {
				lexer.NextToken();

#line  827 "cs.ATG" 
				isImplicit = false; startPos = t.Location; 
			}
			Expect(92);
			Type(
#line  828 "cs.ATG" 
out type);

#line  828 "cs.ATG" 
			TypeReference operatorType = type; 
			Expect(20);
			Type(
#line  829 "cs.ATG" 
out type);
			Identifier();

#line  829 "cs.ATG" 
			string varName = t.val; 
			Expect(21);

#line  830 "cs.ATG" 
			Location endPos = t.Location; 
			if (la.kind == 16) {
				Block(
#line  831 "cs.ATG" 
out stmt);
			} else if (la.kind == 11) {
				lexer.NextToken();

#line  831 "cs.ATG" 
				stmt = null; 
			} else SynErr(160);

#line  834 "cs.ATG" 
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
			
		} else if (StartOf(20)) {
			TypeDecl(
#line  850 "cs.ATG" 
m, attributes);
		} else if (StartOf(10)) {
			Type(
#line  852 "cs.ATG" 
out type);

#line  852 "cs.ATG" 
			Location startPos = t.Location;  
			if (la.kind == 92) {

#line  854 "cs.ATG" 
				OverloadableOperatorType op;
				m.Check(Modifiers.Operators);
				if (m.isNone) Error("at least one modifier must be set");
				
				lexer.NextToken();
				OverloadableOperator(
#line  858 "cs.ATG" 
out op);

#line  858 "cs.ATG" 
				TypeReference firstType, secondType = null; string secondName = null; 
				Expect(20);
				Type(
#line  859 "cs.ATG" 
out firstType);
				Identifier();

#line  859 "cs.ATG" 
				string firstName = t.val; 
				if (la.kind == 14) {
					lexer.NextToken();
					Type(
#line  860 "cs.ATG" 
out secondType);
					Identifier();

#line  860 "cs.ATG" 
					secondName = t.val; 
				} else if (la.kind == 21) {
				} else SynErr(161);

#line  868 "cs.ATG" 
				Location endPos = t.Location; 
				Expect(21);
				if (la.kind == 16) {
					Block(
#line  869 "cs.ATG" 
out stmt);
				} else if (la.kind == 11) {
					lexer.NextToken();
				} else SynErr(162);

#line  871 "cs.ATG" 
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
#line  888 "cs.ATG" 
IsVarDecl()) {

#line  889 "cs.ATG" 
				m.Check(Modifiers.Fields);
				FieldDeclaration fd = new FieldDeclaration(attributes, type, m.Modifier);
				fd.StartLocation = m.GetDeclarationLocation(startPos); 
				
				if (
#line  893 "cs.ATG" 
m.Contains(Modifiers.Fixed)) {
					VariableDeclarator(
#line  894 "cs.ATG" 
variableDeclarators);
					Expect(18);
					Expr(
#line  896 "cs.ATG" 
out expr);

#line  896 "cs.ATG" 
					if (variableDeclarators.Count > 0)
					variableDeclarators[variableDeclarators.Count-1].FixedArrayInitialization = expr; 
					Expect(19);
					while (la.kind == 14) {
						lexer.NextToken();
						VariableDeclarator(
#line  900 "cs.ATG" 
variableDeclarators);
						Expect(18);
						Expr(
#line  902 "cs.ATG" 
out expr);

#line  902 "cs.ATG" 
						if (variableDeclarators.Count > 0)
						variableDeclarators[variableDeclarators.Count-1].FixedArrayInitialization = expr; 
						Expect(19);
					}
				} else if (StartOf(17)) {
					VariableDeclarator(
#line  907 "cs.ATG" 
variableDeclarators);
					while (la.kind == 14) {
						lexer.NextToken();
						VariableDeclarator(
#line  908 "cs.ATG" 
variableDeclarators);
					}
				} else SynErr(163);
				Expect(11);

#line  910 "cs.ATG" 
				fd.EndLocation = t.EndLocation; fd.Fields = variableDeclarators; compilationUnit.AddChild(fd); 
			} else if (la.kind == 111) {

#line  913 "cs.ATG" 
				m.Check(Modifiers.Indexers); 
				lexer.NextToken();
				Expect(18);
				FormalParameterList(
#line  914 "cs.ATG" 
p);
				Expect(19);

#line  914 "cs.ATG" 
				Location endLocation = t.EndLocation; 
				Expect(16);

#line  915 "cs.ATG" 
				IndexerDeclaration indexer = new IndexerDeclaration(type, p, m.Modifier, attributes);
				indexer.StartLocation = startPos;
				indexer.EndLocation   = endLocation;
				indexer.BodyStart     = t.Location;
				PropertyGetRegion getRegion;
				PropertySetRegion setRegion;
				
				AccessorDecls(
#line  922 "cs.ATG" 
out getRegion, out setRegion);
				Expect(17);

#line  923 "cs.ATG" 
				indexer.BodyEnd    = t.EndLocation;
				indexer.GetRegion = getRegion;
				indexer.SetRegion = setRegion;
				compilationUnit.AddChild(indexer);
				
			} else if (
#line  928 "cs.ATG" 
IsIdentifierToken(la)) {
				if (
#line  929 "cs.ATG" 
IsExplicitInterfaceImplementation()) {
					TypeName(
#line  930 "cs.ATG" 
out explicitInterface, false);

#line  931 "cs.ATG" 
					if (la.kind != Tokens.Dot || Peek(1).kind != Tokens.This) {
					qualident = TypeReference.StripLastIdentifierFromType(ref explicitInterface);
					 } 
				} else if (StartOf(17)) {
					Identifier();

#line  934 "cs.ATG" 
					qualident = t.val; 
				} else SynErr(164);

#line  936 "cs.ATG" 
				Location qualIdentEndLocation = t.EndLocation; 
				if (la.kind == 16 || la.kind == 20 || la.kind == 23) {
					if (la.kind == 20 || la.kind == 23) {

#line  940 "cs.ATG" 
						m.Check(Modifiers.PropertysEventsMethods); 
						if (la.kind == 23) {
							TypeParameterList(
#line  942 "cs.ATG" 
templates);
						}
						Expect(20);
						if (la.kind == 111) {
							lexer.NextToken();

#line  944 "cs.ATG" 
							isExtensionMethod = true; 
						}
						if (StartOf(11)) {
							FormalParameterList(
#line  945 "cs.ATG" 
p);
						}
						Expect(21);

#line  947 "cs.ATG" 
						MethodDeclaration methodDeclaration = new MethodDeclaration(qualident,
						                                                           m.Modifier, 
						                                                           type, 
						                                                           p, 
						                                                           attributes);
						if (explicitInterface != null)
							methodDeclaration.InterfaceImplementations.Add(new InterfaceImplementation(explicitInterface, qualident));
						methodDeclaration.StartLocation = m.GetDeclarationLocation(startPos);
						methodDeclaration.EndLocation   = t.EndLocation;
						methodDeclaration.IsExtensionMethod = isExtensionMethod;
						methodDeclaration.Templates = templates;
						compilationUnit.AddChild(methodDeclaration);
						                                      
						while (la.kind == 127) {
							TypeParameterConstraintsClause(
#line  960 "cs.ATG" 
templates);
						}
						if (la.kind == 16) {
							Block(
#line  961 "cs.ATG" 
out stmt);
						} else if (la.kind == 11) {
							lexer.NextToken();
						} else SynErr(165);

#line  961 "cs.ATG" 
						methodDeclaration.Body  = (BlockStatement)stmt; 
					} else {
						lexer.NextToken();

#line  964 "cs.ATG" 
						PropertyDeclaration pDecl = new PropertyDeclaration(qualident, type, m.Modifier, attributes); 
						if (explicitInterface != null)
						pDecl.InterfaceImplementations.Add(new InterfaceImplementation(explicitInterface, qualident));
						      pDecl.StartLocation = m.GetDeclarationLocation(startPos);
						      pDecl.EndLocation   = qualIdentEndLocation;
						      pDecl.BodyStart   = t.Location;
						      PropertyGetRegion getRegion;
						      PropertySetRegion setRegion;
						   
						AccessorDecls(
#line  973 "cs.ATG" 
out getRegion, out setRegion);
						Expect(17);

#line  975 "cs.ATG" 
						pDecl.GetRegion = getRegion;
						pDecl.SetRegion = setRegion;
						pDecl.BodyEnd = t.EndLocation;
						compilationUnit.AddChild(pDecl);
						
					}
				} else if (la.kind == 15) {

#line  983 "cs.ATG" 
					m.Check(Modifiers.Indexers); 
					lexer.NextToken();
					Expect(111);
					Expect(18);
					FormalParameterList(
#line  984 "cs.ATG" 
p);
					Expect(19);

#line  985 "cs.ATG" 
					IndexerDeclaration indexer = new IndexerDeclaration(type, p, m.Modifier, attributes);
					indexer.StartLocation = m.GetDeclarationLocation(startPos);
					indexer.EndLocation   = t.EndLocation;
					if (explicitInterface != null)
					indexer.InterfaceImplementations.Add(new InterfaceImplementation(explicitInterface, "this"));
					      PropertyGetRegion getRegion;
					      PropertySetRegion setRegion;
					    
					Expect(16);

#line  993 "cs.ATG" 
					Location bodyStart = t.Location; 
					AccessorDecls(
#line  994 "cs.ATG" 
out getRegion, out setRegion);
					Expect(17);

#line  995 "cs.ATG" 
					indexer.BodyStart = bodyStart;
					indexer.BodyEnd   = t.EndLocation;
					indexer.GetRegion = getRegion;
					indexer.SetRegion = setRegion;
					compilationUnit.AddChild(indexer);
					
				} else SynErr(166);
			} else SynErr(167);
		} else SynErr(168);
	}

	void InterfaceMemberDecl() {

#line  1022 "cs.ATG" 
		TypeReference type;
		
		AttributeSection section;
		Modifiers mod = Modifiers.None;
		List<AttributeSection> attributes = new List<AttributeSection>();
		List<ParameterDeclarationExpression> parameters = new List<ParameterDeclarationExpression>();
		string name;
		PropertyGetRegion getBlock;
		PropertySetRegion setBlock;
		Location startLocation = new Location(-1, -1);
		List<TemplateDefinition> templates = new List<TemplateDefinition>();
		
		while (la.kind == 18) {
			AttributeSection(
#line  1035 "cs.ATG" 
out section);

#line  1035 "cs.ATG" 
			attributes.Add(section); 
		}
		if (la.kind == 89) {
			lexer.NextToken();

#line  1036 "cs.ATG" 
			mod = Modifiers.New; startLocation = t.Location; 
		}
		if (
#line  1039 "cs.ATG" 
NotVoidPointer()) {
			Expect(123);

#line  1039 "cs.ATG" 
			if (startLocation.X == -1) startLocation = t.Location; 
			Identifier();

#line  1040 "cs.ATG" 
			name = t.val; 
			if (la.kind == 23) {
				TypeParameterList(
#line  1041 "cs.ATG" 
templates);
			}
			Expect(20);
			if (StartOf(11)) {
				FormalParameterList(
#line  1042 "cs.ATG" 
parameters);
			}
			Expect(21);
			while (la.kind == 127) {
				TypeParameterConstraintsClause(
#line  1043 "cs.ATG" 
templates);
			}
			Expect(11);

#line  1045 "cs.ATG" 
			MethodDeclaration md = new MethodDeclaration(name, mod, new TypeReference("void"), parameters, attributes);
			md.StartLocation = startLocation;
			md.EndLocation = t.EndLocation;
			md.Templates = templates;
			compilationUnit.AddChild(md);
			
		} else if (StartOf(21)) {
			if (StartOf(10)) {
				Type(
#line  1052 "cs.ATG" 
out type);

#line  1052 "cs.ATG" 
				if (startLocation.X == -1) startLocation = t.Location; 
				if (StartOf(17)) {
					Identifier();

#line  1054 "cs.ATG" 
					name = t.val; Location qualIdentEndLocation = t.EndLocation; 
					if (la.kind == 20 || la.kind == 23) {
						if (la.kind == 23) {
							TypeParameterList(
#line  1058 "cs.ATG" 
templates);
						}
						Expect(20);
						if (StartOf(11)) {
							FormalParameterList(
#line  1059 "cs.ATG" 
parameters);
						}
						Expect(21);
						while (la.kind == 127) {
							TypeParameterConstraintsClause(
#line  1061 "cs.ATG" 
templates);
						}
						Expect(11);

#line  1062 "cs.ATG" 
						MethodDeclaration md = new MethodDeclaration(name, mod, type, parameters, attributes);
						md.StartLocation = startLocation;
						md.EndLocation = t.EndLocation;
						md.Templates = templates;
						compilationUnit.AddChild(md);
						
					} else if (la.kind == 16) {

#line  1069 "cs.ATG" 
						PropertyDeclaration pd = new PropertyDeclaration(name, type, mod, attributes); compilationUnit.AddChild(pd); 
						lexer.NextToken();

#line  1070 "cs.ATG" 
						Location bodyStart = t.Location;
						InterfaceAccessors(
#line  1070 "cs.ATG" 
out getBlock, out setBlock);
						Expect(17);

#line  1070 "cs.ATG" 
						pd.GetRegion = getBlock; pd.SetRegion = setBlock; pd.StartLocation = startLocation; pd.EndLocation = qualIdentEndLocation; pd.BodyStart = bodyStart; pd.BodyEnd = t.EndLocation; 
					} else SynErr(169);
				} else if (la.kind == 111) {
					lexer.NextToken();
					Expect(18);
					FormalParameterList(
#line  1073 "cs.ATG" 
parameters);
					Expect(19);

#line  1073 "cs.ATG" 
					Location bracketEndLocation = t.EndLocation; 

#line  1073 "cs.ATG" 
					IndexerDeclaration id = new IndexerDeclaration(type, parameters, mod, attributes); compilationUnit.AddChild(id); 
					Expect(16);

#line  1074 "cs.ATG" 
					Location bodyStart = t.Location;
					InterfaceAccessors(
#line  1074 "cs.ATG" 
out getBlock, out setBlock);
					Expect(17);

#line  1074 "cs.ATG" 
					id.GetRegion = getBlock; id.SetRegion = setBlock; id.StartLocation = startLocation;  id.EndLocation = bracketEndLocation; id.BodyStart = bodyStart; id.BodyEnd = t.EndLocation;
				} else SynErr(170);
			} else {
				lexer.NextToken();

#line  1077 "cs.ATG" 
				if (startLocation.X == -1) startLocation = t.Location; 
				Type(
#line  1078 "cs.ATG" 
out type);
				Identifier();

#line  1078 "cs.ATG" 
				EventDeclaration ed = new EventDeclaration(type, t.val, mod, attributes, null);
				compilationUnit.AddChild(ed);
				
				Expect(11);

#line  1081 "cs.ATG" 
				ed.StartLocation = startLocation; ed.EndLocation = t.EndLocation; 
			}
		} else SynErr(171);
	}

	void EnumMemberDecl(
#line  1086 "cs.ATG" 
out FieldDeclaration f) {

#line  1088 "cs.ATG" 
		Expression expr = null;
		List<AttributeSection> attributes = new List<AttributeSection>();
		AttributeSection section = null;
		VariableDeclaration varDecl = null;
		
		while (la.kind == 18) {
			AttributeSection(
#line  1094 "cs.ATG" 
out section);

#line  1094 "cs.ATG" 
			attributes.Add(section); 
		}
		Identifier();

#line  1095 "cs.ATG" 
		f = new FieldDeclaration(attributes);
		varDecl         = new VariableDeclaration(t.val);
		f.Fields.Add(varDecl);
		f.StartLocation = t.Location;
		
		if (la.kind == 3) {
			lexer.NextToken();
			Expr(
#line  1100 "cs.ATG" 
out expr);

#line  1100 "cs.ATG" 
			varDecl.Initializer = expr; 
		}
	}

	void TypeWithRestriction(
#line  540 "cs.ATG" 
out TypeReference type, bool allowNullable, bool canBeUnbound) {

#line  542 "cs.ATG" 
		string name;
		int pointer = 0;
		type = null;
		
		if (StartOf(4)) {
			ClassType(
#line  547 "cs.ATG" 
out type, canBeUnbound);
		} else if (StartOf(5)) {
			SimpleType(
#line  548 "cs.ATG" 
out name);

#line  548 "cs.ATG" 
			type = new TypeReference(name); 
		} else if (la.kind == 123) {
			lexer.NextToken();
			Expect(6);

#line  549 "cs.ATG" 
			pointer = 1; type = new TypeReference("void"); 
		} else SynErr(172);

#line  550 "cs.ATG" 
		List<int> r = new List<int>(); 
		if (
#line  552 "cs.ATG" 
allowNullable && la.kind == Tokens.Question) {
			NullableQuestionMark(
#line  552 "cs.ATG" 
ref type);
		}
		while (
#line  554 "cs.ATG" 
IsPointerOrDims()) {

#line  554 "cs.ATG" 
			int i = 0; 
			if (la.kind == 6) {
				lexer.NextToken();

#line  555 "cs.ATG" 
				++pointer; 
			} else if (la.kind == 18) {
				lexer.NextToken();
				while (la.kind == 14) {
					lexer.NextToken();

#line  556 "cs.ATG" 
					++i; 
				}
				Expect(19);

#line  556 "cs.ATG" 
				r.Add(i); 
			} else SynErr(173);
		}

#line  559 "cs.ATG" 
		if (type != null) {
		type.RankSpecifier = r.ToArray();
		type.PointerNestingLevel = pointer;
		  }
		
	}

	void SimpleType(
#line  587 "cs.ATG" 
out string name) {

#line  588 "cs.ATG" 
		name = String.Empty; 
		if (StartOf(22)) {
			IntegralType(
#line  590 "cs.ATG" 
out name);
		} else if (la.kind == 75) {
			lexer.NextToken();

#line  591 "cs.ATG" 
			name = "float"; 
		} else if (la.kind == 66) {
			lexer.NextToken();

#line  592 "cs.ATG" 
			name = "double"; 
		} else if (la.kind == 62) {
			lexer.NextToken();

#line  593 "cs.ATG" 
			name = "decimal"; 
		} else if (la.kind == 52) {
			lexer.NextToken();

#line  594 "cs.ATG" 
			name = "bool"; 
		} else SynErr(174);
	}

	void NullableQuestionMark(
#line  2127 "cs.ATG" 
ref TypeReference typeRef) {

#line  2128 "cs.ATG" 
		List<TypeReference> typeArguments = new List<TypeReference>(1); 
		Expect(12);

#line  2132 "cs.ATG" 
		if (typeRef != null) typeArguments.Add(typeRef);
		typeRef = new TypeReference("System.Nullable", typeArguments);
		
	}

	void FixedParameter(
#line  624 "cs.ATG" 
out ParameterDeclarationExpression p) {

#line  626 "cs.ATG" 
		TypeReference type;
		ParameterModifiers mod = ParameterModifiers.In;
		Location start = t.Location;
		
		if (la.kind == 93 || la.kind == 100) {
			if (la.kind == 100) {
				lexer.NextToken();

#line  632 "cs.ATG" 
				mod = ParameterModifiers.Ref; 
			} else {
				lexer.NextToken();

#line  633 "cs.ATG" 
				mod = ParameterModifiers.Out; 
			}
		}
		Type(
#line  635 "cs.ATG" 
out type);
		Identifier();

#line  635 "cs.ATG" 
		p = new ParameterDeclarationExpression(type, t.val, mod); p.StartLocation = start; p.EndLocation = t.Location; 
	}

	void ParameterArray(
#line  638 "cs.ATG" 
out ParameterDeclarationExpression p) {

#line  639 "cs.ATG" 
		TypeReference type; 
		Expect(95);
		Type(
#line  641 "cs.ATG" 
out type);
		Identifier();

#line  641 "cs.ATG" 
		p = new ParameterDeclarationExpression(type, t.val, ParameterModifiers.Params); 
	}

	void AccessorModifiers(
#line  644 "cs.ATG" 
out ModifierList m) {

#line  645 "cs.ATG" 
		m = new ModifierList(); 
		if (la.kind == 96) {
			lexer.NextToken();

#line  647 "cs.ATG" 
			m.Add(Modifiers.Private, t.Location); 
		} else if (la.kind == 97) {
			lexer.NextToken();

#line  648 "cs.ATG" 
			m.Add(Modifiers.Protected, t.Location); 
			if (la.kind == 84) {
				lexer.NextToken();

#line  649 "cs.ATG" 
				m.Add(Modifiers.Internal, t.Location); 
			}
		} else if (la.kind == 84) {
			lexer.NextToken();

#line  650 "cs.ATG" 
			m.Add(Modifiers.Internal, t.Location); 
			if (la.kind == 97) {
				lexer.NextToken();

#line  651 "cs.ATG" 
				m.Add(Modifiers.Protected, t.Location); 
			}
		} else SynErr(175);
	}

	void Block(
#line  1219 "cs.ATG" 
out Statement stmt) {
		Expect(16);

#line  1221 "cs.ATG" 
		BlockStatement blockStmt = new BlockStatement();
		blockStmt.StartLocation = t.Location;
		compilationUnit.BlockStart(blockStmt);
		if (!ParseMethodBodies) lexer.SkipCurrentBlock(0);
		
		while (StartOf(23)) {
			Statement();
		}
		Expect(17);

#line  1228 "cs.ATG" 
		stmt = blockStmt;
		blockStmt.EndLocation = t.EndLocation;
		compilationUnit.BlockEnd();
		
	}

	void EventAccessorDecls(
#line  1157 "cs.ATG" 
out EventAddRegion addBlock, out EventRemoveRegion removeBlock) {

#line  1158 "cs.ATG" 
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		Statement stmt;
		addBlock = null;
		removeBlock = null;
		
		while (la.kind == 18) {
			AttributeSection(
#line  1165 "cs.ATG" 
out section);

#line  1165 "cs.ATG" 
			attributes.Add(section); 
		}
		if (la.kind == 130) {

#line  1167 "cs.ATG" 
			addBlock = new EventAddRegion(attributes); 
			AddAccessorDecl(
#line  1168 "cs.ATG" 
out stmt);

#line  1168 "cs.ATG" 
			attributes = new List<AttributeSection>(); addBlock.Block = (BlockStatement)stmt; 
			while (la.kind == 18) {
				AttributeSection(
#line  1169 "cs.ATG" 
out section);

#line  1169 "cs.ATG" 
				attributes.Add(section); 
			}
			RemoveAccessorDecl(
#line  1170 "cs.ATG" 
out stmt);

#line  1170 "cs.ATG" 
			removeBlock = new EventRemoveRegion(attributes); removeBlock.Block = (BlockStatement)stmt; 
		} else if (la.kind == 131) {
			RemoveAccessorDecl(
#line  1172 "cs.ATG" 
out stmt);

#line  1172 "cs.ATG" 
			removeBlock = new EventRemoveRegion(attributes); removeBlock.Block = (BlockStatement)stmt; attributes = new List<AttributeSection>(); 
			while (la.kind == 18) {
				AttributeSection(
#line  1173 "cs.ATG" 
out section);

#line  1173 "cs.ATG" 
				attributes.Add(section); 
			}
			AddAccessorDecl(
#line  1174 "cs.ATG" 
out stmt);

#line  1174 "cs.ATG" 
			addBlock = new EventAddRegion(attributes); addBlock.Block = (BlockStatement)stmt; 
		} else SynErr(176);
	}

	void ConstructorInitializer(
#line  1248 "cs.ATG" 
out ConstructorInitializer ci) {

#line  1249 "cs.ATG" 
		Expression expr; ci = new ConstructorInitializer(); 
		Expect(9);
		if (la.kind == 51) {
			lexer.NextToken();

#line  1253 "cs.ATG" 
			ci.ConstructorInitializerType = ConstructorInitializerType.Base; 
		} else if (la.kind == 111) {
			lexer.NextToken();

#line  1254 "cs.ATG" 
			ci.ConstructorInitializerType = ConstructorInitializerType.This; 
		} else SynErr(177);
		Expect(20);
		if (StartOf(24)) {
			Argument(
#line  1257 "cs.ATG" 
out expr);

#line  1257 "cs.ATG" 
			if (expr != null) { ci.Arguments.Add(expr); } 
			while (la.kind == 14) {
				lexer.NextToken();
				Argument(
#line  1257 "cs.ATG" 
out expr);

#line  1257 "cs.ATG" 
				if (expr != null) { ci.Arguments.Add(expr); } 
			}
		}
		Expect(21);
	}

	void OverloadableOperator(
#line  1269 "cs.ATG" 
out OverloadableOperatorType op) {

#line  1270 "cs.ATG" 
		op = OverloadableOperatorType.None; 
		switch (la.kind) {
		case 4: {
			lexer.NextToken();

#line  1272 "cs.ATG" 
			op = OverloadableOperatorType.Add; 
			break;
		}
		case 5: {
			lexer.NextToken();

#line  1273 "cs.ATG" 
			op = OverloadableOperatorType.Subtract; 
			break;
		}
		case 24: {
			lexer.NextToken();

#line  1275 "cs.ATG" 
			op = OverloadableOperatorType.Not; 
			break;
		}
		case 27: {
			lexer.NextToken();

#line  1276 "cs.ATG" 
			op = OverloadableOperatorType.BitNot; 
			break;
		}
		case 31: {
			lexer.NextToken();

#line  1278 "cs.ATG" 
			op = OverloadableOperatorType.Increment; 
			break;
		}
		case 32: {
			lexer.NextToken();

#line  1279 "cs.ATG" 
			op = OverloadableOperatorType.Decrement; 
			break;
		}
		case 113: {
			lexer.NextToken();

#line  1281 "cs.ATG" 
			op = OverloadableOperatorType.IsTrue; 
			break;
		}
		case 72: {
			lexer.NextToken();

#line  1282 "cs.ATG" 
			op = OverloadableOperatorType.IsFalse; 
			break;
		}
		case 6: {
			lexer.NextToken();

#line  1284 "cs.ATG" 
			op = OverloadableOperatorType.Multiply; 
			break;
		}
		case 7: {
			lexer.NextToken();

#line  1285 "cs.ATG" 
			op = OverloadableOperatorType.Divide; 
			break;
		}
		case 8: {
			lexer.NextToken();

#line  1286 "cs.ATG" 
			op = OverloadableOperatorType.Modulus; 
			break;
		}
		case 28: {
			lexer.NextToken();

#line  1288 "cs.ATG" 
			op = OverloadableOperatorType.BitwiseAnd; 
			break;
		}
		case 29: {
			lexer.NextToken();

#line  1289 "cs.ATG" 
			op = OverloadableOperatorType.BitwiseOr; 
			break;
		}
		case 30: {
			lexer.NextToken();

#line  1290 "cs.ATG" 
			op = OverloadableOperatorType.ExclusiveOr; 
			break;
		}
		case 37: {
			lexer.NextToken();

#line  1292 "cs.ATG" 
			op = OverloadableOperatorType.ShiftLeft; 
			break;
		}
		case 33: {
			lexer.NextToken();

#line  1293 "cs.ATG" 
			op = OverloadableOperatorType.Equality; 
			break;
		}
		case 34: {
			lexer.NextToken();

#line  1294 "cs.ATG" 
			op = OverloadableOperatorType.InEquality; 
			break;
		}
		case 23: {
			lexer.NextToken();

#line  1295 "cs.ATG" 
			op = OverloadableOperatorType.LessThan; 
			break;
		}
		case 35: {
			lexer.NextToken();

#line  1296 "cs.ATG" 
			op = OverloadableOperatorType.GreaterThanOrEqual; 
			break;
		}
		case 36: {
			lexer.NextToken();

#line  1297 "cs.ATG" 
			op = OverloadableOperatorType.LessThanOrEqual; 
			break;
		}
		case 22: {
			lexer.NextToken();

#line  1298 "cs.ATG" 
			op = OverloadableOperatorType.GreaterThan; 
			if (la.kind == 22) {
				lexer.NextToken();

#line  1298 "cs.ATG" 
				op = OverloadableOperatorType.ShiftRight; 
			}
			break;
		}
		default: SynErr(178); break;
		}
	}

	void VariableDeclarator(
#line  1212 "cs.ATG" 
List<VariableDeclaration> fieldDeclaration) {

#line  1213 "cs.ATG" 
		Expression expr = null; 
		Identifier();

#line  1215 "cs.ATG" 
		VariableDeclaration f = new VariableDeclaration(t.val); 
		if (la.kind == 3) {
			lexer.NextToken();
			VariableInitializer(
#line  1216 "cs.ATG" 
out expr);

#line  1216 "cs.ATG" 
			f.Initializer = expr; 
		}

#line  1216 "cs.ATG" 
		fieldDeclaration.Add(f); 
	}

	void AccessorDecls(
#line  1104 "cs.ATG" 
out PropertyGetRegion getBlock, out PropertySetRegion setBlock) {

#line  1106 "cs.ATG" 
		List<AttributeSection> attributes = new List<AttributeSection>(); 
		AttributeSection section;
		getBlock = null;
		setBlock = null; 
		ModifierList modifiers = null;
		
		while (la.kind == 18) {
			AttributeSection(
#line  1113 "cs.ATG" 
out section);

#line  1113 "cs.ATG" 
			attributes.Add(section); 
		}
		if (la.kind == 84 || la.kind == 96 || la.kind == 97) {
			AccessorModifiers(
#line  1114 "cs.ATG" 
out modifiers);
		}
		if (la.kind == 128) {
			GetAccessorDecl(
#line  1116 "cs.ATG" 
out getBlock, attributes);

#line  1117 "cs.ATG" 
			if (modifiers != null) {getBlock.Modifier = modifiers.Modifier; } 
			if (StartOf(25)) {

#line  1118 "cs.ATG" 
				attributes = new List<AttributeSection>(); modifiers = null; 
				while (la.kind == 18) {
					AttributeSection(
#line  1119 "cs.ATG" 
out section);

#line  1119 "cs.ATG" 
					attributes.Add(section); 
				}
				if (la.kind == 84 || la.kind == 96 || la.kind == 97) {
					AccessorModifiers(
#line  1120 "cs.ATG" 
out modifiers);
				}
				SetAccessorDecl(
#line  1121 "cs.ATG" 
out setBlock, attributes);

#line  1122 "cs.ATG" 
				if (modifiers != null) {setBlock.Modifier = modifiers.Modifier; } 
			}
		} else if (la.kind == 129) {
			SetAccessorDecl(
#line  1125 "cs.ATG" 
out setBlock, attributes);

#line  1126 "cs.ATG" 
			if (modifiers != null) {setBlock.Modifier = modifiers.Modifier; } 
			if (StartOf(26)) {

#line  1127 "cs.ATG" 
				attributes = new List<AttributeSection>(); modifiers = null; 
				while (la.kind == 18) {
					AttributeSection(
#line  1128 "cs.ATG" 
out section);

#line  1128 "cs.ATG" 
					attributes.Add(section); 
				}
				if (la.kind == 84 || la.kind == 96 || la.kind == 97) {
					AccessorModifiers(
#line  1129 "cs.ATG" 
out modifiers);
				}
				GetAccessorDecl(
#line  1130 "cs.ATG" 
out getBlock, attributes);

#line  1131 "cs.ATG" 
				if (modifiers != null) {getBlock.Modifier = modifiers.Modifier; } 
			}
		} else if (StartOf(17)) {
			Identifier();

#line  1133 "cs.ATG" 
			Error("get or set accessor declaration expected"); 
		} else SynErr(179);
	}

	void InterfaceAccessors(
#line  1178 "cs.ATG" 
out PropertyGetRegion getBlock, out PropertySetRegion setBlock) {

#line  1180 "cs.ATG" 
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		getBlock = null; setBlock = null;
		PropertyGetSetRegion lastBlock = null;
		
		while (la.kind == 18) {
			AttributeSection(
#line  1186 "cs.ATG" 
out section);

#line  1186 "cs.ATG" 
			attributes.Add(section); 
		}

#line  1187 "cs.ATG" 
		Location startLocation = la.Location; 
		if (la.kind == 128) {
			lexer.NextToken();

#line  1189 "cs.ATG" 
			getBlock = new PropertyGetRegion(null, attributes); 
		} else if (la.kind == 129) {
			lexer.NextToken();

#line  1190 "cs.ATG" 
			setBlock = new PropertySetRegion(null, attributes); 
		} else SynErr(180);
		Expect(11);

#line  1193 "cs.ATG" 
		if (getBlock != null) { getBlock.StartLocation = startLocation; getBlock.EndLocation = t.EndLocation; }
		if (setBlock != null) { setBlock.StartLocation = startLocation; setBlock.EndLocation = t.EndLocation; }
		attributes = new List<AttributeSection>(); 
		if (la.kind == 18 || la.kind == 128 || la.kind == 129) {
			while (la.kind == 18) {
				AttributeSection(
#line  1197 "cs.ATG" 
out section);

#line  1197 "cs.ATG" 
				attributes.Add(section); 
			}

#line  1198 "cs.ATG" 
			startLocation = la.Location; 
			if (la.kind == 128) {
				lexer.NextToken();

#line  1200 "cs.ATG" 
				if (getBlock != null) Error("get already declared");
				                 else { getBlock = new PropertyGetRegion(null, attributes); lastBlock = getBlock; }
				              
			} else if (la.kind == 129) {
				lexer.NextToken();

#line  1203 "cs.ATG" 
				if (setBlock != null) Error("set already declared");
				                 else { setBlock = new PropertySetRegion(null, attributes); lastBlock = setBlock; }
				              
			} else SynErr(181);
			Expect(11);

#line  1208 "cs.ATG" 
			if (lastBlock != null) { lastBlock.StartLocation = startLocation; lastBlock.EndLocation = t.EndLocation; } 
		}
	}

	void GetAccessorDecl(
#line  1137 "cs.ATG" 
out PropertyGetRegion getBlock, List<AttributeSection> attributes) {

#line  1138 "cs.ATG" 
		Statement stmt = null; 
		Expect(128);

#line  1141 "cs.ATG" 
		Location startLocation = t.Location; 
		if (la.kind == 16) {
			Block(
#line  1142 "cs.ATG" 
out stmt);
		} else if (la.kind == 11) {
			lexer.NextToken();
		} else SynErr(182);

#line  1143 "cs.ATG" 
		getBlock = new PropertyGetRegion((BlockStatement)stmt, attributes); 

#line  1144 "cs.ATG" 
		getBlock.StartLocation = startLocation; getBlock.EndLocation = t.EndLocation; 
	}

	void SetAccessorDecl(
#line  1147 "cs.ATG" 
out PropertySetRegion setBlock, List<AttributeSection> attributes) {

#line  1148 "cs.ATG" 
		Statement stmt = null; 
		Expect(129);

#line  1151 "cs.ATG" 
		Location startLocation = t.Location; 
		if (la.kind == 16) {
			Block(
#line  1152 "cs.ATG" 
out stmt);
		} else if (la.kind == 11) {
			lexer.NextToken();
		} else SynErr(183);

#line  1153 "cs.ATG" 
		setBlock = new PropertySetRegion((BlockStatement)stmt, attributes); 

#line  1154 "cs.ATG" 
		setBlock.StartLocation = startLocation; setBlock.EndLocation = t.EndLocation; 
	}

	void AddAccessorDecl(
#line  1234 "cs.ATG" 
out Statement stmt) {

#line  1235 "cs.ATG" 
		stmt = null;
		Expect(130);
		Block(
#line  1238 "cs.ATG" 
out stmt);
	}

	void RemoveAccessorDecl(
#line  1241 "cs.ATG" 
out Statement stmt) {

#line  1242 "cs.ATG" 
		stmt = null;
		Expect(131);
		Block(
#line  1245 "cs.ATG" 
out stmt);
	}

	void VariableInitializer(
#line  1261 "cs.ATG" 
out Expression initializerExpression) {

#line  1262 "cs.ATG" 
		TypeReference type = null; Expression expr = null; initializerExpression = null; 
		if (StartOf(6)) {
			Expr(
#line  1264 "cs.ATG" 
out initializerExpression);
		} else if (la.kind == 16) {
			CollectionInitializer(
#line  1265 "cs.ATG" 
out initializerExpression);
		} else if (la.kind == 106) {
			lexer.NextToken();
			Type(
#line  1266 "cs.ATG" 
out type);
			Expect(18);
			Expr(
#line  1266 "cs.ATG" 
out expr);
			Expect(19);

#line  1266 "cs.ATG" 
			initializerExpression = new StackAllocExpression(type, expr); 
		} else SynErr(184);
	}

	void Statement() {

#line  1402 "cs.ATG" 
		TypeReference type;
		Expression expr;
		Statement stmt = null;
		Location startPos = la.Location;
		
		if (
#line  1410 "cs.ATG" 
IsLabel()) {
			Identifier();

#line  1410 "cs.ATG" 
			compilationUnit.AddChild(new LabelStatement(t.val)); 
			Expect(9);
			Statement();
		} else if (la.kind == 60) {
			lexer.NextToken();
			Type(
#line  1413 "cs.ATG" 
out type);

#line  1413 "cs.ATG" 
			LocalVariableDeclaration var = new LocalVariableDeclaration(type, Modifiers.Const); string ident = null; var.StartLocation = t.Location; 
			Identifier();

#line  1414 "cs.ATG" 
			ident = t.val; 
			Expect(3);
			Expr(
#line  1415 "cs.ATG" 
out expr);

#line  1415 "cs.ATG" 
			var.Variables.Add(new VariableDeclaration(ident, expr)); 
			while (la.kind == 14) {
				lexer.NextToken();
				Identifier();

#line  1416 "cs.ATG" 
				ident = t.val; 
				Expect(3);
				Expr(
#line  1416 "cs.ATG" 
out expr);

#line  1416 "cs.ATG" 
				var.Variables.Add(new VariableDeclaration(ident, expr)); 
			}
			Expect(11);

#line  1417 "cs.ATG" 
			compilationUnit.AddChild(var); 
		} else if (
#line  1419 "cs.ATG" 
IsLocalVarDecl()) {
			LocalVariableDecl(
#line  1419 "cs.ATG" 
out stmt);
			Expect(11);

#line  1419 "cs.ATG" 
			compilationUnit.AddChild(stmt); 
		} else if (StartOf(27)) {
			EmbeddedStatement(
#line  1420 "cs.ATG" 
out stmt);

#line  1420 "cs.ATG" 
			compilationUnit.AddChild(stmt); 
		} else SynErr(185);

#line  1426 "cs.ATG" 
		if (stmt != null) {
		stmt.StartLocation = startPos;
		stmt.EndLocation = t.EndLocation;
		}
		
	}

	void Argument(
#line  1301 "cs.ATG" 
out Expression argumentexpr) {

#line  1303 "cs.ATG" 
		Expression expr;
		FieldDirection fd = FieldDirection.None;
		
		if (la.kind == 93 || la.kind == 100) {
			if (la.kind == 100) {
				lexer.NextToken();

#line  1308 "cs.ATG" 
				fd = FieldDirection.Ref; 
			} else {
				lexer.NextToken();

#line  1309 "cs.ATG" 
				fd = FieldDirection.Out; 
			}
		}
		Expr(
#line  1311 "cs.ATG" 
out expr);

#line  1311 "cs.ATG" 
		argumentexpr = fd != FieldDirection.None ? argumentexpr = new DirectionExpression(fd, expr) : expr; 
	}

	void CollectionInitializer(
#line  1331 "cs.ATG" 
out Expression outExpr) {

#line  1333 "cs.ATG" 
		Expression expr = null;
		CollectionInitializerExpression initializer = new CollectionInitializerExpression();
		
		Expect(16);
		if (StartOf(28)) {
			VariableInitializer(
#line  1338 "cs.ATG" 
out expr);

#line  1339 "cs.ATG" 
			if (expr != null) { initializer.CreateExpressions.Add(expr); } 
			while (
#line  1340 "cs.ATG" 
NotFinalComma()) {
				Expect(14);
				VariableInitializer(
#line  1341 "cs.ATG" 
out expr);

#line  1342 "cs.ATG" 
				if (expr != null) { initializer.CreateExpressions.Add(expr); } 
			}
			if (la.kind == 14) {
				lexer.NextToken();
			}
		}
		Expect(17);

#line  1346 "cs.ATG" 
		outExpr = initializer; 
	}

	void AssignmentOperator(
#line  1314 "cs.ATG" 
out AssignmentOperatorType op) {

#line  1315 "cs.ATG" 
		op = AssignmentOperatorType.None; 
		if (la.kind == 3) {
			lexer.NextToken();

#line  1317 "cs.ATG" 
			op = AssignmentOperatorType.Assign; 
		} else if (la.kind == 38) {
			lexer.NextToken();

#line  1318 "cs.ATG" 
			op = AssignmentOperatorType.Add; 
		} else if (la.kind == 39) {
			lexer.NextToken();

#line  1319 "cs.ATG" 
			op = AssignmentOperatorType.Subtract; 
		} else if (la.kind == 40) {
			lexer.NextToken();

#line  1320 "cs.ATG" 
			op = AssignmentOperatorType.Multiply; 
		} else if (la.kind == 41) {
			lexer.NextToken();

#line  1321 "cs.ATG" 
			op = AssignmentOperatorType.Divide; 
		} else if (la.kind == 42) {
			lexer.NextToken();

#line  1322 "cs.ATG" 
			op = AssignmentOperatorType.Modulus; 
		} else if (la.kind == 43) {
			lexer.NextToken();

#line  1323 "cs.ATG" 
			op = AssignmentOperatorType.BitwiseAnd; 
		} else if (la.kind == 44) {
			lexer.NextToken();

#line  1324 "cs.ATG" 
			op = AssignmentOperatorType.BitwiseOr; 
		} else if (la.kind == 45) {
			lexer.NextToken();

#line  1325 "cs.ATG" 
			op = AssignmentOperatorType.ExclusiveOr; 
		} else if (la.kind == 46) {
			lexer.NextToken();

#line  1326 "cs.ATG" 
			op = AssignmentOperatorType.ShiftLeft; 
		} else if (
#line  1327 "cs.ATG" 
la.kind == Tokens.GreaterThan && Peek(1).kind == Tokens.GreaterEqual) {
			Expect(22);
			Expect(35);

#line  1328 "cs.ATG" 
			op = AssignmentOperatorType.ShiftRight; 
		} else SynErr(186);
	}

	void CollectionOrObjectInitializer(
#line  1349 "cs.ATG" 
out Expression outExpr) {

#line  1351 "cs.ATG" 
		Expression expr = null;
		CollectionInitializerExpression initializer = new CollectionInitializerExpression();
		
		Expect(16);
		if (StartOf(28)) {
			ObjectPropertyInitializerOrVariableInitializer(
#line  1356 "cs.ATG" 
out expr);

#line  1357 "cs.ATG" 
			if (expr != null) { initializer.CreateExpressions.Add(expr); } 
			while (
#line  1358 "cs.ATG" 
NotFinalComma()) {
				Expect(14);
				ObjectPropertyInitializerOrVariableInitializer(
#line  1359 "cs.ATG" 
out expr);

#line  1360 "cs.ATG" 
				if (expr != null) { initializer.CreateExpressions.Add(expr); } 
			}
			if (la.kind == 14) {
				lexer.NextToken();
			}
		}
		Expect(17);

#line  1364 "cs.ATG" 
		outExpr = initializer; 
	}

	void ObjectPropertyInitializerOrVariableInitializer(
#line  1367 "cs.ATG" 
out Expression expr) {

#line  1368 "cs.ATG" 
		expr = null; 
		if (
#line  1370 "cs.ATG" 
IdentAndAsgn()) {
			Identifier();

#line  1372 "cs.ATG" 
			IdentifierExpression l = new IdentifierExpression(t.val);
			l.StartLocation = t.Location; l.EndLocation = t.EndLocation;
			Expression r = null; 
			Expect(3);
			VariableInitializer(
#line  1375 "cs.ATG" 
out r);

#line  1376 "cs.ATG" 
			expr = new AssignmentExpression(l, AssignmentOperatorType.Assign, r); 
		} else if (StartOf(28)) {
			VariableInitializer(
#line  1377 "cs.ATG" 
out expr);
		} else SynErr(187);
	}

	void LocalVariableDecl(
#line  1381 "cs.ATG" 
out Statement stmt) {

#line  1383 "cs.ATG" 
		TypeReference type;
		VariableDeclaration      var = null;
		LocalVariableDeclaration localVariableDeclaration; 
		
		Type(
#line  1388 "cs.ATG" 
out type);

#line  1388 "cs.ATG" 
		localVariableDeclaration = new LocalVariableDeclaration(type); localVariableDeclaration.StartLocation = t.Location; 
		LocalVariableDeclarator(
#line  1389 "cs.ATG" 
out var);

#line  1389 "cs.ATG" 
		localVariableDeclaration.Variables.Add(var); 
		while (la.kind == 14) {
			lexer.NextToken();
			LocalVariableDeclarator(
#line  1390 "cs.ATG" 
out var);

#line  1390 "cs.ATG" 
			localVariableDeclaration.Variables.Add(var); 
		}

#line  1391 "cs.ATG" 
		stmt = localVariableDeclaration; 
	}

	void LocalVariableDeclarator(
#line  1394 "cs.ATG" 
out VariableDeclaration var) {

#line  1395 "cs.ATG" 
		Expression expr = null; 
		Identifier();

#line  1397 "cs.ATG" 
		var = new VariableDeclaration(t.val); 
		if (la.kind == 3) {
			lexer.NextToken();
			VariableInitializer(
#line  1397 "cs.ATG" 
out expr);

#line  1397 "cs.ATG" 
			var.Initializer = expr; 
		}
	}

	void EmbeddedStatement(
#line  1433 "cs.ATG" 
out Statement statement) {

#line  1435 "cs.ATG" 
		TypeReference type = null;
		Expression expr = null;
		Statement embeddedStatement = null;
		statement = null;
		
		if (la.kind == 16) {
			Block(
#line  1441 "cs.ATG" 
out statement);
		} else if (la.kind == 11) {
			lexer.NextToken();

#line  1443 "cs.ATG" 
			statement = new EmptyStatement(); 
		} else if (
#line  1445 "cs.ATG" 
UnCheckedAndLBrace()) {

#line  1445 "cs.ATG" 
			Statement block; bool isChecked = true; 
			if (la.kind == 58) {
				lexer.NextToken();
			} else if (la.kind == 118) {
				lexer.NextToken();

#line  1446 "cs.ATG" 
				isChecked = false;
			} else SynErr(188);
			Block(
#line  1447 "cs.ATG" 
out block);

#line  1447 "cs.ATG" 
			statement = isChecked ? (Statement)new CheckedStatement(block) : (Statement)new UncheckedStatement(block); 
		} else if (la.kind == 79) {
			lexer.NextToken();

#line  1449 "cs.ATG" 
			Statement elseStatement = null; 
			Expect(20);
			Expr(
#line  1450 "cs.ATG" 
out expr);
			Expect(21);
			EmbeddedStatement(
#line  1451 "cs.ATG" 
out embeddedStatement);
			if (la.kind == 67) {
				lexer.NextToken();
				EmbeddedStatement(
#line  1452 "cs.ATG" 
out elseStatement);
			}

#line  1453 "cs.ATG" 
			statement = elseStatement != null ? new IfElseStatement(expr, embeddedStatement, elseStatement) : new IfElseStatement(expr, embeddedStatement); 

#line  1454 "cs.ATG" 
			if (elseStatement is IfElseStatement && (elseStatement as IfElseStatement).TrueStatement.Count == 1) {
			/* else if-section (otherwise we would have a BlockStatment) */
			(statement as IfElseStatement).ElseIfSections.Add(
			             new ElseIfSection((elseStatement as IfElseStatement).Condition,
			                               (elseStatement as IfElseStatement).TrueStatement[0]));
			(statement as IfElseStatement).ElseIfSections.AddRange((elseStatement as IfElseStatement).ElseIfSections);
			(statement as IfElseStatement).FalseStatement = (elseStatement as IfElseStatement).FalseStatement;
			} 
		} else if (la.kind == 110) {
			lexer.NextToken();

#line  1462 "cs.ATG" 
			List<SwitchSection> switchSections = new List<SwitchSection>(); 
			Expect(20);
			Expr(
#line  1463 "cs.ATG" 
out expr);
			Expect(21);
			Expect(16);
			SwitchSections(
#line  1464 "cs.ATG" 
switchSections);
			Expect(17);

#line  1465 "cs.ATG" 
			statement = new SwitchStatement(expr, switchSections); 
		} else if (la.kind == 125) {
			lexer.NextToken();
			Expect(20);
			Expr(
#line  1467 "cs.ATG" 
out expr);
			Expect(21);
			EmbeddedStatement(
#line  1469 "cs.ATG" 
out embeddedStatement);

#line  1469 "cs.ATG" 
			statement = new DoLoopStatement(expr, embeddedStatement, ConditionType.While, ConditionPosition.Start);
		} else if (la.kind == 65) {
			lexer.NextToken();
			EmbeddedStatement(
#line  1470 "cs.ATG" 
out embeddedStatement);
			Expect(125);
			Expect(20);
			Expr(
#line  1471 "cs.ATG" 
out expr);
			Expect(21);
			Expect(11);

#line  1471 "cs.ATG" 
			statement = new DoLoopStatement(expr, embeddedStatement, ConditionType.While, ConditionPosition.End); 
		} else if (la.kind == 76) {
			lexer.NextToken();

#line  1472 "cs.ATG" 
			List<Statement> initializer = null; List<Statement> iterator = null; 
			Expect(20);
			if (StartOf(6)) {
				ForInitializer(
#line  1473 "cs.ATG" 
out initializer);
			}
			Expect(11);
			if (StartOf(6)) {
				Expr(
#line  1474 "cs.ATG" 
out expr);
			}
			Expect(11);
			if (StartOf(6)) {
				ForIterator(
#line  1475 "cs.ATG" 
out iterator);
			}
			Expect(21);
			EmbeddedStatement(
#line  1476 "cs.ATG" 
out embeddedStatement);

#line  1476 "cs.ATG" 
			statement = new ForStatement(initializer, expr, iterator, embeddedStatement); 
		} else if (la.kind == 77) {
			lexer.NextToken();
			Expect(20);
			Type(
#line  1477 "cs.ATG" 
out type);
			Identifier();

#line  1477 "cs.ATG" 
			string varName = t.val; Location start = t.Location;
			Expect(81);
			Expr(
#line  1478 "cs.ATG" 
out expr);
			Expect(21);
			EmbeddedStatement(
#line  1479 "cs.ATG" 
out embeddedStatement);

#line  1479 "cs.ATG" 
			statement = new ForeachStatement(type, varName , expr, embeddedStatement); 
			statement.EndLocation = t.EndLocation;
			
		} else if (la.kind == 53) {
			lexer.NextToken();
			Expect(11);

#line  1483 "cs.ATG" 
			statement = new BreakStatement(); 
		} else if (la.kind == 61) {
			lexer.NextToken();
			Expect(11);

#line  1484 "cs.ATG" 
			statement = new ContinueStatement(); 
		} else if (la.kind == 78) {
			GotoStatement(
#line  1485 "cs.ATG" 
out statement);
		} else if (
#line  1486 "cs.ATG" 
IsYieldStatement()) {
			Expect(132);
			if (la.kind == 101) {
				lexer.NextToken();
				Expr(
#line  1486 "cs.ATG" 
out expr);

#line  1486 "cs.ATG" 
				statement = new YieldStatement(new ReturnStatement(expr)); 
			} else if (la.kind == 53) {
				lexer.NextToken();

#line  1487 "cs.ATG" 
				statement = new YieldStatement(new BreakStatement()); 
			} else SynErr(189);
			Expect(11);
		} else if (la.kind == 101) {
			lexer.NextToken();
			if (StartOf(6)) {
				Expr(
#line  1488 "cs.ATG" 
out expr);
			}
			Expect(11);

#line  1488 "cs.ATG" 
			statement = new ReturnStatement(expr); 
		} else if (la.kind == 112) {
			lexer.NextToken();
			if (StartOf(6)) {
				Expr(
#line  1489 "cs.ATG" 
out expr);
			}
			Expect(11);

#line  1489 "cs.ATG" 
			statement = new ThrowStatement(expr); 
		} else if (StartOf(6)) {
			StatementExpr(
#line  1492 "cs.ATG" 
out statement);
			Expect(11);
		} else if (la.kind == 114) {
			TryStatement(
#line  1494 "cs.ATG" 
out statement);
		} else if (la.kind == 86) {
			lexer.NextToken();
			Expect(20);
			Expr(
#line  1496 "cs.ATG" 
out expr);
			Expect(21);
			EmbeddedStatement(
#line  1497 "cs.ATG" 
out embeddedStatement);

#line  1497 "cs.ATG" 
			statement = new LockStatement(expr, embeddedStatement); 
		} else if (la.kind == 121) {

#line  1499 "cs.ATG" 
			Statement resourceAcquisitionStmt = null; 
			lexer.NextToken();
			Expect(20);
			ResourceAcquisition(
#line  1501 "cs.ATG" 
out resourceAcquisitionStmt);
			Expect(21);
			EmbeddedStatement(
#line  1502 "cs.ATG" 
out embeddedStatement);

#line  1502 "cs.ATG" 
			statement = new UsingStatement(resourceAcquisitionStmt, embeddedStatement); 
		} else if (la.kind == 119) {
			lexer.NextToken();
			Block(
#line  1504 "cs.ATG" 
out embeddedStatement);

#line  1504 "cs.ATG" 
			statement = new UnsafeStatement(embeddedStatement); 
		} else if (la.kind == 74) {
			lexer.NextToken();
			Expect(20);
			Type(
#line  1507 "cs.ATG" 
out type);

#line  1507 "cs.ATG" 
			if (type.PointerNestingLevel == 0) Error("can only fix pointer types");
			List<VariableDeclaration> pointerDeclarators = new List<VariableDeclaration>(1);
			
			Identifier();

#line  1510 "cs.ATG" 
			string identifier = t.val; 
			Expect(3);
			Expr(
#line  1511 "cs.ATG" 
out expr);

#line  1511 "cs.ATG" 
			pointerDeclarators.Add(new VariableDeclaration(identifier, expr)); 
			while (la.kind == 14) {
				lexer.NextToken();
				Identifier();

#line  1513 "cs.ATG" 
				identifier = t.val; 
				Expect(3);
				Expr(
#line  1514 "cs.ATG" 
out expr);

#line  1514 "cs.ATG" 
				pointerDeclarators.Add(new VariableDeclaration(identifier, expr)); 
			}
			Expect(21);
			EmbeddedStatement(
#line  1516 "cs.ATG" 
out embeddedStatement);

#line  1516 "cs.ATG" 
			statement = new FixedStatement(type, pointerDeclarators, embeddedStatement); 
		} else SynErr(190);
	}

	void SwitchSections(
#line  1538 "cs.ATG" 
List<SwitchSection> switchSections) {

#line  1540 "cs.ATG" 
		SwitchSection switchSection = new SwitchSection();
		CaseLabel label;
		
		SwitchLabel(
#line  1544 "cs.ATG" 
out label);

#line  1544 "cs.ATG" 
		if (label != null) { switchSection.SwitchLabels.Add(label); } 

#line  1545 "cs.ATG" 
		compilationUnit.BlockStart(switchSection); 
		while (StartOf(29)) {
			if (la.kind == 55 || la.kind == 63) {
				SwitchLabel(
#line  1547 "cs.ATG" 
out label);

#line  1548 "cs.ATG" 
				if (label != null) {
				if (switchSection.Children.Count > 0) {
					// open new section
					compilationUnit.BlockEnd(); switchSections.Add(switchSection);
					switchSection = new SwitchSection();
					compilationUnit.BlockStart(switchSection);
				}
				switchSection.SwitchLabels.Add(label);
				}
				
			} else {
				Statement();
			}
		}

#line  1560 "cs.ATG" 
		compilationUnit.BlockEnd(); switchSections.Add(switchSection); 
	}

	void ForInitializer(
#line  1519 "cs.ATG" 
out List<Statement> initializer) {

#line  1521 "cs.ATG" 
		Statement stmt; 
		initializer = new List<Statement>();
		
		if (
#line  1525 "cs.ATG" 
IsLocalVarDecl()) {
			LocalVariableDecl(
#line  1525 "cs.ATG" 
out stmt);

#line  1525 "cs.ATG" 
			initializer.Add(stmt);
		} else if (StartOf(6)) {
			StatementExpr(
#line  1526 "cs.ATG" 
out stmt);

#line  1526 "cs.ATG" 
			initializer.Add(stmt);
			while (la.kind == 14) {
				lexer.NextToken();
				StatementExpr(
#line  1526 "cs.ATG" 
out stmt);

#line  1526 "cs.ATG" 
				initializer.Add(stmt);
			}
		} else SynErr(191);
	}

	void ForIterator(
#line  1529 "cs.ATG" 
out List<Statement> iterator) {

#line  1531 "cs.ATG" 
		Statement stmt; 
		iterator = new List<Statement>();
		
		StatementExpr(
#line  1535 "cs.ATG" 
out stmt);

#line  1535 "cs.ATG" 
		iterator.Add(stmt);
		while (la.kind == 14) {
			lexer.NextToken();
			StatementExpr(
#line  1535 "cs.ATG" 
out stmt);

#line  1535 "cs.ATG" 
			iterator.Add(stmt); 
		}
	}

	void GotoStatement(
#line  1613 "cs.ATG" 
out Statement stmt) {

#line  1614 "cs.ATG" 
		Expression expr; stmt = null; 
		Expect(78);
		if (StartOf(17)) {
			Identifier();

#line  1618 "cs.ATG" 
			stmt = new GotoStatement(t.val); 
			Expect(11);
		} else if (la.kind == 55) {
			lexer.NextToken();
			Expr(
#line  1619 "cs.ATG" 
out expr);
			Expect(11);

#line  1619 "cs.ATG" 
			stmt = new GotoCaseStatement(expr); 
		} else if (la.kind == 63) {
			lexer.NextToken();
			Expect(11);

#line  1620 "cs.ATG" 
			stmt = new GotoCaseStatement(null); 
		} else SynErr(192);
	}

	void StatementExpr(
#line  1640 "cs.ATG" 
out Statement stmt) {

#line  1641 "cs.ATG" 
		Expression expr; 
		Expr(
#line  1643 "cs.ATG" 
out expr);

#line  1646 "cs.ATG" 
		stmt = new ExpressionStatement(expr); 
	}

	void TryStatement(
#line  1570 "cs.ATG" 
out Statement tryStatement) {

#line  1572 "cs.ATG" 
		Statement blockStmt = null, finallyStmt = null;
		List<CatchClause> catchClauses = null;
		
		Expect(114);
		Block(
#line  1576 "cs.ATG" 
out blockStmt);
		if (la.kind == 56) {
			CatchClauses(
#line  1578 "cs.ATG" 
out catchClauses);
			if (la.kind == 73) {
				lexer.NextToken();
				Block(
#line  1578 "cs.ATG" 
out finallyStmt);
			}
		} else if (la.kind == 73) {
			lexer.NextToken();
			Block(
#line  1579 "cs.ATG" 
out finallyStmt);
		} else SynErr(193);

#line  1582 "cs.ATG" 
		tryStatement = new TryCatchStatement(blockStmt, catchClauses, finallyStmt);
			
	}

	void ResourceAcquisition(
#line  1624 "cs.ATG" 
out Statement stmt) {

#line  1626 "cs.ATG" 
		stmt = null;
		Expression expr;
		
		if (
#line  1631 "cs.ATG" 
IsLocalVarDecl()) {
			LocalVariableDecl(
#line  1631 "cs.ATG" 
out stmt);
		} else if (StartOf(6)) {
			Expr(
#line  1632 "cs.ATG" 
out expr);

#line  1636 "cs.ATG" 
			stmt = new ExpressionStatement(expr); 
		} else SynErr(194);
	}

	void SwitchLabel(
#line  1563 "cs.ATG" 
out CaseLabel label) {

#line  1564 "cs.ATG" 
		Expression expr = null; label = null; 
		if (la.kind == 55) {
			lexer.NextToken();
			Expr(
#line  1566 "cs.ATG" 
out expr);
			Expect(9);

#line  1566 "cs.ATG" 
			label =  new CaseLabel(expr); 
		} else if (la.kind == 63) {
			lexer.NextToken();
			Expect(9);

#line  1567 "cs.ATG" 
			label =  new CaseLabel(); 
		} else SynErr(195);
	}

	void CatchClauses(
#line  1587 "cs.ATG" 
out List<CatchClause> catchClauses) {

#line  1589 "cs.ATG" 
		catchClauses = new List<CatchClause>();
		
		Expect(56);

#line  1592 "cs.ATG" 
		string identifier;
		Statement stmt;
		TypeReference typeRef;
		
		if (la.kind == 16) {
			Block(
#line  1598 "cs.ATG" 
out stmt);

#line  1598 "cs.ATG" 
			catchClauses.Add(new CatchClause(stmt)); 
		} else if (la.kind == 20) {
			lexer.NextToken();
			ClassType(
#line  1600 "cs.ATG" 
out typeRef, false);

#line  1600 "cs.ATG" 
			identifier = null; 
			if (StartOf(17)) {
				Identifier();

#line  1601 "cs.ATG" 
				identifier = t.val; 
			}
			Expect(21);
			Block(
#line  1602 "cs.ATG" 
out stmt);

#line  1603 "cs.ATG" 
			catchClauses.Add(new CatchClause(typeRef, identifier, stmt)); 
			while (
#line  1604 "cs.ATG" 
IsTypedCatch()) {
				Expect(56);
				Expect(20);
				ClassType(
#line  1604 "cs.ATG" 
out typeRef, false);

#line  1604 "cs.ATG" 
				identifier = null; 
				if (StartOf(17)) {
					Identifier();

#line  1605 "cs.ATG" 
					identifier = t.val; 
				}
				Expect(21);
				Block(
#line  1606 "cs.ATG" 
out stmt);

#line  1607 "cs.ATG" 
				catchClauses.Add(new CatchClause(typeRef, identifier, stmt)); 
			}
			if (la.kind == 56) {
				lexer.NextToken();
				Block(
#line  1609 "cs.ATG" 
out stmt);

#line  1609 "cs.ATG" 
				catchClauses.Add(new CatchClause(stmt)); 
			}
		} else SynErr(196);
	}

	void UnaryExpr(
#line  1667 "cs.ATG" 
out Expression uExpr) {

#line  1669 "cs.ATG" 
		TypeReference type = null;
		Expression expr;
		ArrayList  expressions = new ArrayList();
		uExpr = null;
		
		while (StartOf(30) || 
#line  1691 "cs.ATG" 
IsTypeCast()) {
			if (la.kind == 4) {
				lexer.NextToken();

#line  1678 "cs.ATG" 
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.Plus)); 
			} else if (la.kind == 5) {
				lexer.NextToken();

#line  1679 "cs.ATG" 
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.Minus)); 
			} else if (la.kind == 24) {
				lexer.NextToken();

#line  1680 "cs.ATG" 
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.Not)); 
			} else if (la.kind == 27) {
				lexer.NextToken();

#line  1681 "cs.ATG" 
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.BitNot)); 
			} else if (la.kind == 6) {
				lexer.NextToken();

#line  1682 "cs.ATG" 
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.Star)); 
			} else if (la.kind == 31) {
				lexer.NextToken();

#line  1683 "cs.ATG" 
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.Increment)); 
			} else if (la.kind == 32) {
				lexer.NextToken();

#line  1684 "cs.ATG" 
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.Decrement)); 
			} else if (la.kind == 28) {
				lexer.NextToken();

#line  1685 "cs.ATG" 
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.BitWiseAnd)); 
			} else {
				Expect(20);
				Type(
#line  1691 "cs.ATG" 
out type);
				Expect(21);

#line  1691 "cs.ATG" 
				expressions.Add(new CastExpression(type)); 
			}
		}
		PrimaryExpr(
#line  1695 "cs.ATG" 
out expr);

#line  1695 "cs.ATG" 
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
#line  1967 "cs.ATG" 
ref Expression outExpr) {

#line  1968 "cs.ATG" 
		Expression expr;   
		ConditionalAndExpr(
#line  1970 "cs.ATG" 
ref outExpr);
		while (la.kind == 26) {
			lexer.NextToken();
			UnaryExpr(
#line  1970 "cs.ATG" 
out expr);
			ConditionalAndExpr(
#line  1970 "cs.ATG" 
ref expr);

#line  1970 "cs.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.LogicalOr, expr);  
		}
	}

	void PrimaryExpr(
#line  1712 "cs.ATG" 
out Expression pexpr) {

#line  1714 "cs.ATG" 
		TypeReference type = null;
		List<TypeReference> typeList = null;
		Expression expr;
		pexpr = null;
		
		if (la.kind == 113) {
			lexer.NextToken();

#line  1721 "cs.ATG" 
			pexpr = new PrimitiveExpression(true, "true");  
		} else if (la.kind == 72) {
			lexer.NextToken();

#line  1722 "cs.ATG" 
			pexpr = new PrimitiveExpression(false, "false"); 
		} else if (la.kind == 90) {
			lexer.NextToken();

#line  1723 "cs.ATG" 
			pexpr = new PrimitiveExpression(null, "null");  
		} else if (la.kind == 2) {
			lexer.NextToken();

#line  1724 "cs.ATG" 
			pexpr = new PrimitiveExpression(t.literalValue, t.val);  
		} else if (
#line  1725 "cs.ATG" 
StartOfQueryExpression()) {
			QueryExpression(
#line  1726 "cs.ATG" 
out pexpr);
		} else if (
#line  1727 "cs.ATG" 
IdentAndDoubleColon()) {
			Identifier();

#line  1728 "cs.ATG" 
			type = new TypeReference(t.val); 
			Expect(10);

#line  1729 "cs.ATG" 
			pexpr = new TypeReferenceExpression(type); 
			Identifier();

#line  1730 "cs.ATG" 
			if (type.Type == "global") { type.IsGlobal = true; type.Type = (t.val ?? "?"); } else type.Type += "." + (t.val ?? "?"); 
		} else if (StartOf(17)) {
			Identifier();

#line  1733 "cs.ATG" 
			pexpr = new IdentifierExpression(t.val); 

#line  1734 "cs.ATG" 
			pexpr.StartLocation = t.Location; pexpr.EndLocation = t.EndLocation; 
			if (la.kind == 48) {
				ShortedLambdaExpression(
#line  1736 "cs.ATG" 
(IdentifierExpression)pexpr, out pexpr);
			}
		} else if (
#line  1737 "cs.ATG" 
IsLambdaExpression()) {
			LambdaExpression(
#line  1738 "cs.ATG" 
out pexpr);
		} else if (la.kind == 20) {
			lexer.NextToken();
			Expr(
#line  1740 "cs.ATG" 
out expr);
			Expect(21);

#line  1740 "cs.ATG" 
			pexpr = new ParenthesizedExpression(expr); 
		} else if (StartOf(31)) {

#line  1742 "cs.ATG" 
			string val = null; 
			switch (la.kind) {
			case 52: {
				lexer.NextToken();

#line  1744 "cs.ATG" 
				val = "bool"; 
				break;
			}
			case 54: {
				lexer.NextToken();

#line  1745 "cs.ATG" 
				val = "byte"; 
				break;
			}
			case 57: {
				lexer.NextToken();

#line  1746 "cs.ATG" 
				val = "char"; 
				break;
			}
			case 62: {
				lexer.NextToken();

#line  1747 "cs.ATG" 
				val = "decimal"; 
				break;
			}
			case 66: {
				lexer.NextToken();

#line  1748 "cs.ATG" 
				val = "double"; 
				break;
			}
			case 75: {
				lexer.NextToken();

#line  1749 "cs.ATG" 
				val = "float"; 
				break;
			}
			case 82: {
				lexer.NextToken();

#line  1750 "cs.ATG" 
				val = "int"; 
				break;
			}
			case 87: {
				lexer.NextToken();

#line  1751 "cs.ATG" 
				val = "long"; 
				break;
			}
			case 91: {
				lexer.NextToken();

#line  1752 "cs.ATG" 
				val = "object"; 
				break;
			}
			case 102: {
				lexer.NextToken();

#line  1753 "cs.ATG" 
				val = "sbyte"; 
				break;
			}
			case 104: {
				lexer.NextToken();

#line  1754 "cs.ATG" 
				val = "short"; 
				break;
			}
			case 108: {
				lexer.NextToken();

#line  1755 "cs.ATG" 
				val = "string"; 
				break;
			}
			case 116: {
				lexer.NextToken();

#line  1756 "cs.ATG" 
				val = "uint"; 
				break;
			}
			case 117: {
				lexer.NextToken();

#line  1757 "cs.ATG" 
				val = "ulong"; 
				break;
			}
			case 120: {
				lexer.NextToken();

#line  1758 "cs.ATG" 
				val = "ushort"; 
				break;
			}
			}

#line  1759 "cs.ATG" 
			t.val = ""; 
			Expect(15);
			Identifier();

#line  1759 "cs.ATG" 
			pexpr = new FieldReferenceExpression(new TypeReferenceExpression(val), t.val); 
		} else if (la.kind == 111) {
			lexer.NextToken();

#line  1761 "cs.ATG" 
			pexpr = new ThisReferenceExpression(); 
		} else if (la.kind == 51) {
			lexer.NextToken();

#line  1763 "cs.ATG" 
			Expression retExpr = new BaseReferenceExpression(); 
			if (la.kind == 15) {
				lexer.NextToken();
				Identifier();

#line  1765 "cs.ATG" 
				retExpr = new FieldReferenceExpression(retExpr, t.val); 
			} else if (la.kind == 18) {
				lexer.NextToken();
				Expr(
#line  1766 "cs.ATG" 
out expr);

#line  1766 "cs.ATG" 
				List<Expression> indices = new List<Expression>(); if (expr != null) { indices.Add(expr); } 
				while (la.kind == 14) {
					lexer.NextToken();
					Expr(
#line  1767 "cs.ATG" 
out expr);

#line  1767 "cs.ATG" 
					if (expr != null) { indices.Add(expr); } 
				}
				Expect(19);

#line  1768 "cs.ATG" 
				retExpr = new IndexerExpression(retExpr, indices); 
			} else SynErr(197);

#line  1769 "cs.ATG" 
			pexpr = retExpr; 
		} else if (la.kind == 89) {
			NewExpression(
#line  1772 "cs.ATG" 
out expr);

#line  1772 "cs.ATG" 
			pexpr = expr; 
		} else if (la.kind == 115) {
			lexer.NextToken();
			Expect(20);
			if (
#line  1776 "cs.ATG" 
NotVoidPointer()) {
				Expect(123);

#line  1776 "cs.ATG" 
				type = new TypeReference("void"); 
			} else if (StartOf(10)) {
				TypeWithRestriction(
#line  1777 "cs.ATG" 
out type, true, true);
			} else SynErr(198);
			Expect(21);

#line  1778 "cs.ATG" 
			pexpr = new TypeOfExpression(type); 
		} else if (la.kind == 63) {
			lexer.NextToken();
			Expect(20);
			Type(
#line  1780 "cs.ATG" 
out type);
			Expect(21);

#line  1780 "cs.ATG" 
			pexpr = new DefaultValueExpression(type); 
		} else if (la.kind == 105) {
			lexer.NextToken();
			Expect(20);
			Type(
#line  1781 "cs.ATG" 
out type);
			Expect(21);

#line  1781 "cs.ATG" 
			pexpr = new SizeOfExpression(type); 
		} else if (la.kind == 58) {
			lexer.NextToken();
			Expect(20);
			Expr(
#line  1782 "cs.ATG" 
out expr);
			Expect(21);

#line  1782 "cs.ATG" 
			pexpr = new CheckedExpression(expr); 
		} else if (la.kind == 118) {
			lexer.NextToken();
			Expect(20);
			Expr(
#line  1783 "cs.ATG" 
out expr);
			Expect(21);

#line  1783 "cs.ATG" 
			pexpr = new UncheckedExpression(expr); 
		} else if (la.kind == 64) {
			lexer.NextToken();
			AnonymousMethodExpr(
#line  1784 "cs.ATG" 
out expr);

#line  1784 "cs.ATG" 
			pexpr = expr; 
		} else SynErr(199);
		while (StartOf(32) || 
#line  1795 "cs.ATG" 
IsGenericFollowedBy(Tokens.Dot) && IsTypeReferenceExpression(pexpr) || 
#line  1804 "cs.ATG" 
IsGenericFollowedBy(Tokens.OpenParenthesis)) {
			if (la.kind == 31 || la.kind == 32) {
				if (la.kind == 31) {
					lexer.NextToken();

#line  1788 "cs.ATG" 
					pexpr = new UnaryOperatorExpression(pexpr, UnaryOperatorType.PostIncrement); 
				} else if (la.kind == 32) {
					lexer.NextToken();

#line  1789 "cs.ATG" 
					pexpr = new UnaryOperatorExpression(pexpr, UnaryOperatorType.PostDecrement); 
				} else SynErr(200);
			} else if (la.kind == 47) {
				lexer.NextToken();
				Identifier();

#line  1792 "cs.ATG" 
				pexpr = new PointerReferenceExpression(pexpr, t.val); 
			} else if (la.kind == 15) {
				lexer.NextToken();
				Identifier();

#line  1793 "cs.ATG" 
				pexpr = new FieldReferenceExpression(pexpr, t.val);
			} else if (
#line  1795 "cs.ATG" 
IsGenericFollowedBy(Tokens.Dot) && IsTypeReferenceExpression(pexpr)) {
				TypeArgumentList(
#line  1796 "cs.ATG" 
out typeList, false);
				Expect(15);
				Identifier();

#line  1798 "cs.ATG" 
				pexpr = new FieldReferenceExpression(GetTypeReferenceExpression(pexpr, typeList), t.val);
			} else if (la.kind == 20) {
				lexer.NextToken();

#line  1800 "cs.ATG" 
				List<Expression> parameters = new List<Expression>(); 
				if (StartOf(24)) {
					Argument(
#line  1801 "cs.ATG" 
out expr);

#line  1801 "cs.ATG" 
					if (expr != null) {parameters.Add(expr);} 
					while (la.kind == 14) {
						lexer.NextToken();
						Argument(
#line  1802 "cs.ATG" 
out expr);

#line  1802 "cs.ATG" 
						if (expr != null) {parameters.Add(expr);} 
					}
				}
				Expect(21);

#line  1803 "cs.ATG" 
				pexpr = new InvocationExpression(pexpr, parameters); 
			} else if (
#line  1804 "cs.ATG" 
IsGenericFollowedBy(Tokens.OpenParenthesis)) {
				TypeArgumentList(
#line  1804 "cs.ATG" 
out typeList, false);
				Expect(20);

#line  1805 "cs.ATG" 
				List<Expression> parameters = new List<Expression>(); 
				if (StartOf(24)) {
					Argument(
#line  1806 "cs.ATG" 
out expr);

#line  1806 "cs.ATG" 
					if (expr != null) {parameters.Add(expr);} 
					while (la.kind == 14) {
						lexer.NextToken();
						Argument(
#line  1807 "cs.ATG" 
out expr);

#line  1807 "cs.ATG" 
						if (expr != null) {parameters.Add(expr);} 
					}
				}
				Expect(21);

#line  1808 "cs.ATG" 
				pexpr = new InvocationExpression(pexpr, parameters, typeList); 
			} else {

#line  1811 "cs.ATG" 
				List<Expression> indices = new List<Expression>();
				
				lexer.NextToken();
				Expr(
#line  1813 "cs.ATG" 
out expr);

#line  1813 "cs.ATG" 
				if (expr != null) { indices.Add(expr); } 
				while (la.kind == 14) {
					lexer.NextToken();
					Expr(
#line  1814 "cs.ATG" 
out expr);

#line  1814 "cs.ATG" 
					if (expr != null) { indices.Add(expr); } 
				}
				Expect(19);

#line  1815 "cs.ATG" 
				pexpr = new IndexerExpression(pexpr, indices); 
			}
		}
	}

	void QueryExpression(
#line  2203 "cs.ATG" 
out Expression outExpr) {

#line  2204 "cs.ATG" 
		QueryExpression q = new QueryExpression(); outExpr = q; q.StartLocation = la.Location; 
		QueryExpressionFromClause fromClause;
		
		QueryExpressionFromClause(
#line  2208 "cs.ATG" 
out fromClause);

#line  2208 "cs.ATG" 
		q.FromClause = fromClause; 
		QueryExpressionBody(
#line  2209 "cs.ATG" 
q);

#line  2210 "cs.ATG" 
		q.EndLocation = t.EndLocation; 
	}

	void ShortedLambdaExpression(
#line  1892 "cs.ATG" 
IdentifierExpression ident, out Expression pexpr) {

#line  1893 "cs.ATG" 
		LambdaExpression lambda = new LambdaExpression(); pexpr = lambda; 
		Expect(48);

#line  1898 "cs.ATG" 
		lambda.StartLocation = ident.StartLocation;
		lambda.Parameters.Add(new ParameterDeclarationExpression(null, ident.Identifier));
		lambda.Parameters[0].StartLocation = ident.StartLocation;
		lambda.Parameters[0].EndLocation = ident.EndLocation;
		
		LambdaExpressionBody(
#line  1903 "cs.ATG" 
lambda);
	}

	void LambdaExpression(
#line  1872 "cs.ATG" 
out Expression outExpr) {

#line  1874 "cs.ATG" 
		LambdaExpression lambda = new LambdaExpression();
		lambda.StartLocation = la.Location;
		ParameterDeclarationExpression p;
		outExpr = lambda;
		
		Expect(20);
		if (StartOf(10)) {
			LambdaExpressionParameter(
#line  1882 "cs.ATG" 
out p);

#line  1882 "cs.ATG" 
			if (p != null) lambda.Parameters.Add(p); 
			while (la.kind == 14) {
				lexer.NextToken();
				LambdaExpressionParameter(
#line  1884 "cs.ATG" 
out p);

#line  1884 "cs.ATG" 
				if (p != null) lambda.Parameters.Add(p); 
			}
		}
		Expect(21);
		Expect(48);
		LambdaExpressionBody(
#line  1889 "cs.ATG" 
lambda);
	}

	void NewExpression(
#line  1819 "cs.ATG" 
out Expression pexpr) {

#line  1820 "cs.ATG" 
		pexpr = null;
		List<Expression> parameters = new List<Expression>();
		TypeReference type = null;
		Expression expr;
		
		Expect(89);
		if (StartOf(10)) {
			NonArrayType(
#line  1827 "cs.ATG" 
out type);
		}
		if (la.kind == 16 || la.kind == 20) {
			if (la.kind == 20) {

#line  1833 "cs.ATG" 
				ObjectCreateExpression oce = new ObjectCreateExpression(type, parameters); 
				lexer.NextToken();

#line  1834 "cs.ATG" 
				if (type == null) Error("Cannot use an anonymous type with arguments for the constructor"); 
				if (StartOf(24)) {
					Argument(
#line  1835 "cs.ATG" 
out expr);

#line  1835 "cs.ATG" 
					if (expr != null) { parameters.Add(expr); } 
					while (la.kind == 14) {
						lexer.NextToken();
						Argument(
#line  1836 "cs.ATG" 
out expr);

#line  1836 "cs.ATG" 
						if (expr != null) { parameters.Add(expr); } 
					}
				}
				Expect(21);

#line  1838 "cs.ATG" 
				pexpr = oce; 
				if (la.kind == 16) {
					CollectionOrObjectInitializer(
#line  1839 "cs.ATG" 
out expr);

#line  1839 "cs.ATG" 
					oce.ObjectInitializer = (CollectionInitializerExpression)expr; 
				}
			} else {

#line  1840 "cs.ATG" 
				ObjectCreateExpression oce = new ObjectCreateExpression(type, parameters); 
				CollectionOrObjectInitializer(
#line  1841 "cs.ATG" 
out expr);

#line  1841 "cs.ATG" 
				oce.ObjectInitializer = (CollectionInitializerExpression)expr; 

#line  1842 "cs.ATG" 
				pexpr = oce; 
			}
		} else if (la.kind == 18) {
			lexer.NextToken();

#line  1847 "cs.ATG" 
			ArrayCreateExpression ace = new ArrayCreateExpression(type);
			/* we must not change RankSpecifier on the null type reference*/
			if (ace.CreateType.IsNull) { ace.CreateType = new TypeReference(""); }
			pexpr = ace;
			int dims = 0; List<int> ranks = new List<int>();
			
			if (la.kind == 14 || la.kind == 19) {
				while (la.kind == 14) {
					lexer.NextToken();

#line  1854 "cs.ATG" 
					dims += 1; 
				}
				Expect(19);

#line  1855 "cs.ATG" 
				ranks.Add(dims); dims = 0; 
				while (la.kind == 18) {
					lexer.NextToken();
					while (la.kind == 14) {
						lexer.NextToken();

#line  1856 "cs.ATG" 
						++dims; 
					}
					Expect(19);

#line  1856 "cs.ATG" 
					ranks.Add(dims); dims = 0; 
				}

#line  1857 "cs.ATG" 
				ace.CreateType.RankSpecifier = ranks.ToArray(); 
				CollectionInitializer(
#line  1858 "cs.ATG" 
out expr);

#line  1858 "cs.ATG" 
				ace.ArrayInitializer = (CollectionInitializerExpression)expr; 
			} else if (StartOf(6)) {
				Expr(
#line  1859 "cs.ATG" 
out expr);

#line  1859 "cs.ATG" 
				if (expr != null) parameters.Add(expr); 
				while (la.kind == 14) {
					lexer.NextToken();

#line  1860 "cs.ATG" 
					dims += 1; 
					Expr(
#line  1861 "cs.ATG" 
out expr);

#line  1861 "cs.ATG" 
					if (expr != null) parameters.Add(expr); 
				}
				Expect(19);

#line  1863 "cs.ATG" 
				ranks.Add(dims); ace.Arguments = parameters; dims = 0; 
				while (la.kind == 18) {
					lexer.NextToken();
					while (la.kind == 14) {
						lexer.NextToken();

#line  1864 "cs.ATG" 
						++dims; 
					}
					Expect(19);

#line  1864 "cs.ATG" 
					ranks.Add(dims); dims = 0; 
				}

#line  1865 "cs.ATG" 
				ace.CreateType.RankSpecifier = ranks.ToArray(); 
				if (la.kind == 16) {
					CollectionInitializer(
#line  1866 "cs.ATG" 
out expr);

#line  1866 "cs.ATG" 
					ace.ArrayInitializer = (CollectionInitializerExpression)expr; 
				}
			} else SynErr(201);
		} else SynErr(202);
	}

	void AnonymousMethodExpr(
#line  1934 "cs.ATG" 
out Expression outExpr) {

#line  1936 "cs.ATG" 
		AnonymousMethodExpression expr = new AnonymousMethodExpression();
		expr.StartLocation = t.Location;
		BlockStatement stmt;
		List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
		outExpr = expr;
		
		if (la.kind == 20) {
			lexer.NextToken();
			if (StartOf(11)) {
				FormalParameterList(
#line  1945 "cs.ATG" 
p);

#line  1945 "cs.ATG" 
				expr.Parameters = p; 
			}
			Expect(21);

#line  1947 "cs.ATG" 
			expr.HasParameterList = true; 
		}
		BlockInsideExpression(
#line  1949 "cs.ATG" 
out stmt);

#line  1949 "cs.ATG" 
		expr.Body  = stmt; 

#line  1950 "cs.ATG" 
		expr.EndLocation = t.Location; 
	}

	void TypeArgumentList(
#line  2137 "cs.ATG" 
out List<TypeReference> types, bool canBeUnbound) {

#line  2139 "cs.ATG" 
		types = new List<TypeReference>();
		TypeReference type = null;
		
		Expect(23);
		if (
#line  2144 "cs.ATG" 
canBeUnbound && (la.kind == Tokens.GreaterThan || la.kind == Tokens.Comma)) {

#line  2145 "cs.ATG" 
			types.Add(TypeReference.Null); 
			while (la.kind == 14) {
				lexer.NextToken();

#line  2146 "cs.ATG" 
				types.Add(TypeReference.Null); 
			}
		} else if (StartOf(10)) {
			Type(
#line  2147 "cs.ATG" 
out type);

#line  2147 "cs.ATG" 
			if (type != null) { types.Add(type); } 
			while (la.kind == 14) {
				lexer.NextToken();
				Type(
#line  2148 "cs.ATG" 
out type);

#line  2148 "cs.ATG" 
				if (type != null) { types.Add(type); } 
			}
		} else SynErr(203);
		Expect(22);
	}

	void LambdaExpressionParameter(
#line  1906 "cs.ATG" 
out ParameterDeclarationExpression p) {

#line  1907 "cs.ATG" 
		Location start = la.Location; p = null;
		TypeReference type;
		
		if (
#line  1911 "cs.ATG" 
Peek(1).kind == Tokens.Comma || Peek(1).kind == Tokens.CloseParenthesis) {
			Identifier();

#line  1913 "cs.ATG" 
			p = new ParameterDeclarationExpression(null, t.val);
			p.StartLocation = start; p.EndLocation = t.EndLocation;
			
		} else if (StartOf(10)) {
			Type(
#line  1916 "cs.ATG" 
out type);
			Identifier();

#line  1918 "cs.ATG" 
			p = new ParameterDeclarationExpression(type, t.val);
			p.StartLocation = start; p.EndLocation = t.EndLocation;
			
		} else SynErr(204);
	}

	void LambdaExpressionBody(
#line  1924 "cs.ATG" 
LambdaExpression lambda) {

#line  1925 "cs.ATG" 
		Expression expr; BlockStatement stmt; 
		if (la.kind == 16) {
			BlockInsideExpression(
#line  1928 "cs.ATG" 
out stmt);

#line  1928 "cs.ATG" 
			lambda.StatementBody = stmt; 
		} else if (StartOf(6)) {
			Expr(
#line  1929 "cs.ATG" 
out expr);

#line  1929 "cs.ATG" 
			lambda.ExpressionBody = expr; 
		} else SynErr(205);

#line  1931 "cs.ATG" 
		lambda.EndLocation = t.EndLocation; 
	}

	void BlockInsideExpression(
#line  1953 "cs.ATG" 
out BlockStatement outStmt) {

#line  1954 "cs.ATG" 
		Statement stmt = null; outStmt = null; 

#line  1958 "cs.ATG" 
		if (compilationUnit != null) { 
		Block(
#line  1959 "cs.ATG" 
out stmt);

#line  1959 "cs.ATG" 
		outStmt = (BlockStatement)stmt; 

#line  1960 "cs.ATG" 
		} else { 
		Expect(16);

#line  1962 "cs.ATG" 
		lexer.SkipCurrentBlock(0); 
		Expect(17);

#line  1964 "cs.ATG" 
		} 
	}

	void ConditionalAndExpr(
#line  1973 "cs.ATG" 
ref Expression outExpr) {

#line  1974 "cs.ATG" 
		Expression expr; 
		InclusiveOrExpr(
#line  1976 "cs.ATG" 
ref outExpr);
		while (la.kind == 25) {
			lexer.NextToken();
			UnaryExpr(
#line  1976 "cs.ATG" 
out expr);
			InclusiveOrExpr(
#line  1976 "cs.ATG" 
ref expr);

#line  1976 "cs.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.LogicalAnd, expr);  
		}
	}

	void InclusiveOrExpr(
#line  1979 "cs.ATG" 
ref Expression outExpr) {

#line  1980 "cs.ATG" 
		Expression expr; 
		ExclusiveOrExpr(
#line  1982 "cs.ATG" 
ref outExpr);
		while (la.kind == 29) {
			lexer.NextToken();
			UnaryExpr(
#line  1982 "cs.ATG" 
out expr);
			ExclusiveOrExpr(
#line  1982 "cs.ATG" 
ref expr);

#line  1982 "cs.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.BitwiseOr, expr);  
		}
	}

	void ExclusiveOrExpr(
#line  1985 "cs.ATG" 
ref Expression outExpr) {

#line  1986 "cs.ATG" 
		Expression expr; 
		AndExpr(
#line  1988 "cs.ATG" 
ref outExpr);
		while (la.kind == 30) {
			lexer.NextToken();
			UnaryExpr(
#line  1988 "cs.ATG" 
out expr);
			AndExpr(
#line  1988 "cs.ATG" 
ref expr);

#line  1988 "cs.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.ExclusiveOr, expr);  
		}
	}

	void AndExpr(
#line  1991 "cs.ATG" 
ref Expression outExpr) {

#line  1992 "cs.ATG" 
		Expression expr; 
		EqualityExpr(
#line  1994 "cs.ATG" 
ref outExpr);
		while (la.kind == 28) {
			lexer.NextToken();
			UnaryExpr(
#line  1994 "cs.ATG" 
out expr);
			EqualityExpr(
#line  1994 "cs.ATG" 
ref expr);

#line  1994 "cs.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.BitwiseAnd, expr);  
		}
	}

	void EqualityExpr(
#line  1997 "cs.ATG" 
ref Expression outExpr) {

#line  1999 "cs.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		RelationalExpr(
#line  2003 "cs.ATG" 
ref outExpr);
		while (la.kind == 33 || la.kind == 34) {
			if (la.kind == 34) {
				lexer.NextToken();

#line  2006 "cs.ATG" 
				op = BinaryOperatorType.InEquality; 
			} else {
				lexer.NextToken();

#line  2007 "cs.ATG" 
				op = BinaryOperatorType.Equality; 
			}
			UnaryExpr(
#line  2009 "cs.ATG" 
out expr);
			RelationalExpr(
#line  2009 "cs.ATG" 
ref expr);

#line  2009 "cs.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr);  
		}
	}

	void RelationalExpr(
#line  2013 "cs.ATG" 
ref Expression outExpr) {

#line  2015 "cs.ATG" 
		TypeReference type;
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		ShiftExpr(
#line  2020 "cs.ATG" 
ref outExpr);
		while (StartOf(33)) {
			if (StartOf(34)) {
				if (la.kind == 23) {
					lexer.NextToken();

#line  2022 "cs.ATG" 
					op = BinaryOperatorType.LessThan; 
				} else if (la.kind == 22) {
					lexer.NextToken();

#line  2023 "cs.ATG" 
					op = BinaryOperatorType.GreaterThan; 
				} else if (la.kind == 36) {
					lexer.NextToken();

#line  2024 "cs.ATG" 
					op = BinaryOperatorType.LessThanOrEqual; 
				} else if (la.kind == 35) {
					lexer.NextToken();

#line  2025 "cs.ATG" 
					op = BinaryOperatorType.GreaterThanOrEqual; 
				} else SynErr(206);
				UnaryExpr(
#line  2027 "cs.ATG" 
out expr);
				ShiftExpr(
#line  2028 "cs.ATG" 
ref expr);

#line  2029 "cs.ATG" 
				outExpr = new BinaryOperatorExpression(outExpr, op, expr); 
			} else {
				if (la.kind == 85) {
					lexer.NextToken();
					TypeWithRestriction(
#line  2032 "cs.ATG" 
out type, false, false);
					if (
#line  2033 "cs.ATG" 
la.kind == Tokens.Question && Tokens.CastFollower[Peek(1).kind] == false) {
						NullableQuestionMark(
#line  2034 "cs.ATG" 
ref type);
					}

#line  2035 "cs.ATG" 
					outExpr = new TypeOfIsExpression(outExpr, type); 
				} else if (la.kind == 50) {
					lexer.NextToken();
					TypeWithRestriction(
#line  2037 "cs.ATG" 
out type, false, false);
					if (
#line  2038 "cs.ATG" 
la.kind == Tokens.Question && Tokens.CastFollower[Peek(1).kind] == false) {
						NullableQuestionMark(
#line  2039 "cs.ATG" 
ref type);
					}

#line  2040 "cs.ATG" 
					outExpr = new CastExpression(type, outExpr, CastType.TryCast); 
				} else SynErr(207);
			}
		}
	}

	void ShiftExpr(
#line  2045 "cs.ATG" 
ref Expression outExpr) {

#line  2047 "cs.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		AdditiveExpr(
#line  2051 "cs.ATG" 
ref outExpr);
		while (la.kind == 37 || 
#line  2054 "cs.ATG" 
IsShiftRight()) {
			if (la.kind == 37) {
				lexer.NextToken();

#line  2053 "cs.ATG" 
				op = BinaryOperatorType.ShiftLeft; 
			} else {
				Expect(22);
				Expect(22);

#line  2055 "cs.ATG" 
				op = BinaryOperatorType.ShiftRight; 
			}
			UnaryExpr(
#line  2058 "cs.ATG" 
out expr);
			AdditiveExpr(
#line  2058 "cs.ATG" 
ref expr);

#line  2058 "cs.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr);  
		}
	}

	void AdditiveExpr(
#line  2062 "cs.ATG" 
ref Expression outExpr) {

#line  2064 "cs.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		MultiplicativeExpr(
#line  2068 "cs.ATG" 
ref outExpr);
		while (la.kind == 4 || la.kind == 5) {
			if (la.kind == 4) {
				lexer.NextToken();

#line  2071 "cs.ATG" 
				op = BinaryOperatorType.Add; 
			} else {
				lexer.NextToken();

#line  2072 "cs.ATG" 
				op = BinaryOperatorType.Subtract; 
			}
			UnaryExpr(
#line  2074 "cs.ATG" 
out expr);
			MultiplicativeExpr(
#line  2074 "cs.ATG" 
ref expr);

#line  2074 "cs.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr);  
		}
	}

	void MultiplicativeExpr(
#line  2078 "cs.ATG" 
ref Expression outExpr) {

#line  2080 "cs.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		while (la.kind == 6 || la.kind == 7 || la.kind == 8) {
			if (la.kind == 6) {
				lexer.NextToken();

#line  2086 "cs.ATG" 
				op = BinaryOperatorType.Multiply; 
			} else if (la.kind == 7) {
				lexer.NextToken();

#line  2087 "cs.ATG" 
				op = BinaryOperatorType.Divide; 
			} else {
				lexer.NextToken();

#line  2088 "cs.ATG" 
				op = BinaryOperatorType.Modulus; 
			}
			UnaryExpr(
#line  2090 "cs.ATG" 
out expr);

#line  2090 "cs.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr); 
		}
	}

	void TypeParameterConstraintsClauseBase(
#line  2194 "cs.ATG" 
out TypeReference type) {

#line  2195 "cs.ATG" 
		TypeReference t; type = null; 
		if (la.kind == 109) {
			lexer.NextToken();

#line  2197 "cs.ATG" 
			type = TypeReference.StructConstraint; 
		} else if (la.kind == 59) {
			lexer.NextToken();

#line  2198 "cs.ATG" 
			type = TypeReference.ClassConstraint; 
		} else if (la.kind == 89) {
			lexer.NextToken();
			Expect(20);
			Expect(21);

#line  2199 "cs.ATG" 
			type = TypeReference.NewConstraint; 
		} else if (StartOf(10)) {
			Type(
#line  2200 "cs.ATG" 
out t);

#line  2200 "cs.ATG" 
			type = t; 
		} else SynErr(208);
	}

	void QueryExpressionFromClause(
#line  2213 "cs.ATG" 
out QueryExpressionFromClause fc) {

#line  2214 "cs.ATG" 
		fc = new QueryExpressionFromClause(); fc.StartLocation = la.Location; 
		QueryExpressionFromGenerator fg;
		
		Expect(137);
		QueryExpressionFromGenerator(
#line  2219 "cs.ATG" 
out fg);

#line  2219 "cs.ATG" 
		SafeAdd(fc, fc.Generators, fg); 
		while (la.kind == 14) {
			lexer.NextToken();
			QueryExpressionFromGenerator(
#line  2221 "cs.ATG" 
out fg);

#line  2221 "cs.ATG" 
			SafeAdd(fc, fc.Generators, fg); 
		}

#line  2223 "cs.ATG" 
		fc.EndLocation = t.EndLocation; 
	}

	void QueryExpressionBody(
#line  2235 "cs.ATG" 
QueryExpression q) {

#line  2236 "cs.ATG" 
		QueryExpressionFromClause fromClause;     QueryExpressionWhereClause whereClause;
		QueryExpressionSelectClause selectClause; QueryExpressionGroupClause groupClause;
		QueryExpressionIntoClause intoClause;
		
		while (la.kind == 127 || la.kind == 137) {
			if (la.kind == 137) {
				QueryExpressionFromClause(
#line  2241 "cs.ATG" 
out fromClause);

#line  2241 "cs.ATG" 
				SafeAdd<QueryExpressionClause>(q, q.FromOrWhereClauses, fromClause); 
			} else {
				QueryExpressionWhereClause(
#line  2242 "cs.ATG" 
out whereClause);

#line  2242 "cs.ATG" 
				SafeAdd<QueryExpressionClause>(q, q.FromOrWhereClauses, whereClause); 
			}
		}
		if (la.kind == 140) {
			QueryExpressionOrderByClause(
#line  2244 "cs.ATG" 
q);
		}
		if (la.kind == 133) {
			QueryExpressionSelectClause(
#line  2245 "cs.ATG" 
out selectClause);

#line  2245 "cs.ATG" 
			q.SelectOrGroupClause = selectClause; 
		} else if (la.kind == 134) {
			QueryExpressionGroupClause(
#line  2246 "cs.ATG" 
out groupClause);

#line  2246 "cs.ATG" 
			q.SelectOrGroupClause = groupClause; 
		} else SynErr(209);
		if (la.kind == 136) {
			QueryExpressionIntoClause(
#line  2248 "cs.ATG" 
out intoClause);

#line  2248 "cs.ATG" 
			q.IntoClause = intoClause; 
		}
	}

	void QueryExpressionFromGenerator(
#line  2226 "cs.ATG" 
out QueryExpressionFromGenerator fg) {

#line  2227 "cs.ATG" 
		Expression expr; fg = new QueryExpressionFromGenerator(); fg.StartLocation = la.Location; 
		Identifier();

#line  2229 "cs.ATG" 
		fg.Identifier = t.val; 
		Expect(81);
		Expr(
#line  2231 "cs.ATG" 
out expr);

#line  2231 "cs.ATG" 
		fg.InExpression = expr; 

#line  2232 "cs.ATG" 
		fg.EndLocation = t.EndLocation; 
	}

	void QueryExpressionWhereClause(
#line  2251 "cs.ATG" 
out QueryExpressionWhereClause wc) {

#line  2252 "cs.ATG" 
		Expression expr; wc = new QueryExpressionWhereClause(); wc.StartLocation = la.Location; 
		Expect(127);
		Expr(
#line  2255 "cs.ATG" 
out expr);

#line  2255 "cs.ATG" 
		wc.Condition = expr; 

#line  2256 "cs.ATG" 
		wc.EndLocation = t.EndLocation; 
	}

	void QueryExpressionOrderByClause(
#line  2259 "cs.ATG" 
QueryExpression q) {

#line  2260 "cs.ATG" 
		QueryExpressionOrdering ordering; 
		Expect(140);
		QueryExpressionOrderingClause(
#line  2263 "cs.ATG" 
out ordering);

#line  2263 "cs.ATG" 
		SafeAdd(q, q.Orderings, ordering); 
		while (la.kind == 14) {
			lexer.NextToken();
			QueryExpressionOrderingClause(
#line  2265 "cs.ATG" 
out ordering);

#line  2265 "cs.ATG" 
			SafeAdd(q, q.Orderings, ordering); 
		}
	}

	void QueryExpressionSelectClause(
#line  2279 "cs.ATG" 
out QueryExpressionSelectClause sc) {

#line  2280 "cs.ATG" 
		Expression expr; sc = new QueryExpressionSelectClause(); sc.StartLocation = la.Location; 
		Expect(133);
		Expr(
#line  2283 "cs.ATG" 
out expr);

#line  2283 "cs.ATG" 
		sc.Projection = expr; 

#line  2284 "cs.ATG" 
		sc.EndLocation = t.EndLocation; 
	}

	void QueryExpressionGroupClause(
#line  2287 "cs.ATG" 
out QueryExpressionGroupClause gc) {

#line  2288 "cs.ATG" 
		Expression expr; gc = new QueryExpressionGroupClause(); gc.StartLocation = la.Location; 
		Expect(134);
		Expr(
#line  2291 "cs.ATG" 
out expr);

#line  2291 "cs.ATG" 
		gc.Projection = expr; 
		Expect(135);
		Expr(
#line  2293 "cs.ATG" 
out expr);

#line  2293 "cs.ATG" 
		gc.GroupBy = expr; 

#line  2294 "cs.ATG" 
		gc.EndLocation = t.EndLocation; 
	}

	void QueryExpressionIntoClause(
#line  2297 "cs.ATG" 
out QueryExpressionIntoClause ic) {

#line  2298 "cs.ATG" 
		ic = new QueryExpressionIntoClause(); ic.StartLocation = la.Location; 
		Expect(136);
		Identifier();

#line  2301 "cs.ATG" 
		ic.IntoIdentifier = t.val; 

#line  2302 "cs.ATG" 
		ic.ContinuedQuery = new QueryExpression(); 

#line  2303 "cs.ATG" 
		ic.ContinuedQuery.StartLocation = la.Location; 
		QueryExpressionBody(
#line  2304 "cs.ATG" 
ic.ContinuedQuery);

#line  2305 "cs.ATG" 
		ic.ContinuedQuery.EndLocation = t.EndLocation; 

#line  2306 "cs.ATG" 
		ic.EndLocation = t.EndLocation; 
	}

	void QueryExpressionOrderingClause(
#line  2269 "cs.ATG" 
out QueryExpressionOrdering ordering) {

#line  2270 "cs.ATG" 
		Expression expr; ordering = new QueryExpressionOrdering(); ordering.StartLocation = la.Location; 
		Expr(
#line  2272 "cs.ATG" 
out expr);

#line  2272 "cs.ATG" 
		ordering.Criteria = expr; 
		if (la.kind == 138 || la.kind == 139) {
			if (la.kind == 138) {
				lexer.NextToken();

#line  2273 "cs.ATG" 
				ordering.Direction = QueryExpressionOrderingDirection.Ascending; 
			} else {
				lexer.NextToken();

#line  2274 "cs.ATG" 
				ordering.Direction = QueryExpressionOrderingDirection.Descending; 
			}
		}

#line  2276 "cs.ATG" 
		ordering.EndLocation = t.EndLocation; 
	}


	
	public override void Parse()
	{
		CS();

	}
	
	protected override void SynErr(int line, int col, int errorNumber)
	{
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
			case 48: s = "\"=>\" expected"; break;
			case 49: s = "\"abstract\" expected"; break;
			case 50: s = "\"as\" expected"; break;
			case 51: s = "\"base\" expected"; break;
			case 52: s = "\"bool\" expected"; break;
			case 53: s = "\"break\" expected"; break;
			case 54: s = "\"byte\" expected"; break;
			case 55: s = "\"case\" expected"; break;
			case 56: s = "\"catch\" expected"; break;
			case 57: s = "\"char\" expected"; break;
			case 58: s = "\"checked\" expected"; break;
			case 59: s = "\"class\" expected"; break;
			case 60: s = "\"const\" expected"; break;
			case 61: s = "\"continue\" expected"; break;
			case 62: s = "\"decimal\" expected"; break;
			case 63: s = "\"default\" expected"; break;
			case 64: s = "\"delegate\" expected"; break;
			case 65: s = "\"do\" expected"; break;
			case 66: s = "\"double\" expected"; break;
			case 67: s = "\"else\" expected"; break;
			case 68: s = "\"enum\" expected"; break;
			case 69: s = "\"event\" expected"; break;
			case 70: s = "\"explicit\" expected"; break;
			case 71: s = "\"extern\" expected"; break;
			case 72: s = "\"false\" expected"; break;
			case 73: s = "\"finally\" expected"; break;
			case 74: s = "\"fixed\" expected"; break;
			case 75: s = "\"float\" expected"; break;
			case 76: s = "\"for\" expected"; break;
			case 77: s = "\"foreach\" expected"; break;
			case 78: s = "\"goto\" expected"; break;
			case 79: s = "\"if\" expected"; break;
			case 80: s = "\"implicit\" expected"; break;
			case 81: s = "\"in\" expected"; break;
			case 82: s = "\"int\" expected"; break;
			case 83: s = "\"interface\" expected"; break;
			case 84: s = "\"internal\" expected"; break;
			case 85: s = "\"is\" expected"; break;
			case 86: s = "\"lock\" expected"; break;
			case 87: s = "\"long\" expected"; break;
			case 88: s = "\"namespace\" expected"; break;
			case 89: s = "\"new\" expected"; break;
			case 90: s = "\"null\" expected"; break;
			case 91: s = "\"object\" expected"; break;
			case 92: s = "\"operator\" expected"; break;
			case 93: s = "\"out\" expected"; break;
			case 94: s = "\"override\" expected"; break;
			case 95: s = "\"params\" expected"; break;
			case 96: s = "\"private\" expected"; break;
			case 97: s = "\"protected\" expected"; break;
			case 98: s = "\"public\" expected"; break;
			case 99: s = "\"readonly\" expected"; break;
			case 100: s = "\"ref\" expected"; break;
			case 101: s = "\"return\" expected"; break;
			case 102: s = "\"sbyte\" expected"; break;
			case 103: s = "\"sealed\" expected"; break;
			case 104: s = "\"short\" expected"; break;
			case 105: s = "\"sizeof\" expected"; break;
			case 106: s = "\"stackalloc\" expected"; break;
			case 107: s = "\"static\" expected"; break;
			case 108: s = "\"string\" expected"; break;
			case 109: s = "\"struct\" expected"; break;
			case 110: s = "\"switch\" expected"; break;
			case 111: s = "\"this\" expected"; break;
			case 112: s = "\"throw\" expected"; break;
			case 113: s = "\"true\" expected"; break;
			case 114: s = "\"try\" expected"; break;
			case 115: s = "\"typeof\" expected"; break;
			case 116: s = "\"uint\" expected"; break;
			case 117: s = "\"ulong\" expected"; break;
			case 118: s = "\"unchecked\" expected"; break;
			case 119: s = "\"unsafe\" expected"; break;
			case 120: s = "\"ushort\" expected"; break;
			case 121: s = "\"using\" expected"; break;
			case 122: s = "\"virtual\" expected"; break;
			case 123: s = "\"void\" expected"; break;
			case 124: s = "\"volatile\" expected"; break;
			case 125: s = "\"while\" expected"; break;
			case 126: s = "\"partial\" expected"; break;
			case 127: s = "\"where\" expected"; break;
			case 128: s = "\"get\" expected"; break;
			case 129: s = "\"set\" expected"; break;
			case 130: s = "\"add\" expected"; break;
			case 131: s = "\"remove\" expected"; break;
			case 132: s = "\"yield\" expected"; break;
			case 133: s = "\"select\" expected"; break;
			case 134: s = "\"group\" expected"; break;
			case 135: s = "\"by\" expected"; break;
			case 136: s = "\"into\" expected"; break;
			case 137: s = "\"from\" expected"; break;
			case 138: s = "\"ascending\" expected"; break;
			case 139: s = "\"descending\" expected"; break;
			case 140: s = "\"orderby\" expected"; break;
			case 141: s = "??? expected"; break;
			case 142: s = "invalid NamespaceMemberDecl"; break;
			case 143: s = "invalid NonArrayType"; break;
			case 144: s = "invalid Identifier"; break;
			case 145: s = "invalid AttributeArguments"; break;
			case 146: s = "invalid Expr"; break;
			case 147: s = "invalid TypeModifier"; break;
			case 148: s = "invalid TypeDecl"; break;
			case 149: s = "invalid TypeDecl"; break;
			case 150: s = "invalid IntegralType"; break;
			case 151: s = "invalid FormalParameterList"; break;
			case 152: s = "invalid FormalParameterList"; break;
			case 153: s = "invalid ClassType"; break;
			case 154: s = "invalid ClassMemberDecl"; break;
			case 155: s = "invalid ClassMemberDecl"; break;
			case 156: s = "invalid StructMemberDecl"; break;
			case 157: s = "invalid StructMemberDecl"; break;
			case 158: s = "invalid StructMemberDecl"; break;
			case 159: s = "invalid StructMemberDecl"; break;
			case 160: s = "invalid StructMemberDecl"; break;
			case 161: s = "invalid StructMemberDecl"; break;
			case 162: s = "invalid StructMemberDecl"; break;
			case 163: s = "invalid StructMemberDecl"; break;
			case 164: s = "invalid StructMemberDecl"; break;
			case 165: s = "invalid StructMemberDecl"; break;
			case 166: s = "invalid StructMemberDecl"; break;
			case 167: s = "invalid StructMemberDecl"; break;
			case 168: s = "invalid StructMemberDecl"; break;
			case 169: s = "invalid InterfaceMemberDecl"; break;
			case 170: s = "invalid InterfaceMemberDecl"; break;
			case 171: s = "invalid InterfaceMemberDecl"; break;
			case 172: s = "invalid TypeWithRestriction"; break;
			case 173: s = "invalid TypeWithRestriction"; break;
			case 174: s = "invalid SimpleType"; break;
			case 175: s = "invalid AccessorModifiers"; break;
			case 176: s = "invalid EventAccessorDecls"; break;
			case 177: s = "invalid ConstructorInitializer"; break;
			case 178: s = "invalid OverloadableOperator"; break;
			case 179: s = "invalid AccessorDecls"; break;
			case 180: s = "invalid InterfaceAccessors"; break;
			case 181: s = "invalid InterfaceAccessors"; break;
			case 182: s = "invalid GetAccessorDecl"; break;
			case 183: s = "invalid SetAccessorDecl"; break;
			case 184: s = "invalid VariableInitializer"; break;
			case 185: s = "invalid Statement"; break;
			case 186: s = "invalid AssignmentOperator"; break;
			case 187: s = "invalid ObjectPropertyInitializerOrVariableInitializer"; break;
			case 188: s = "invalid EmbeddedStatement"; break;
			case 189: s = "invalid EmbeddedStatement"; break;
			case 190: s = "invalid EmbeddedStatement"; break;
			case 191: s = "invalid ForInitializer"; break;
			case 192: s = "invalid GotoStatement"; break;
			case 193: s = "invalid TryStatement"; break;
			case 194: s = "invalid ResourceAcquisition"; break;
			case 195: s = "invalid SwitchLabel"; break;
			case 196: s = "invalid CatchClauses"; break;
			case 197: s = "invalid PrimaryExpr"; break;
			case 198: s = "invalid PrimaryExpr"; break;
			case 199: s = "invalid PrimaryExpr"; break;
			case 200: s = "invalid PrimaryExpr"; break;
			case 201: s = "invalid NewExpression"; break;
			case 202: s = "invalid NewExpression"; break;
			case 203: s = "invalid TypeArgumentList"; break;
			case 204: s = "invalid LambdaExpressionParameter"; break;
			case 205: s = "invalid LambdaExpressionBody"; break;
			case 206: s = "invalid RelationalExpr"; break;
			case 207: s = "invalid RelationalExpr"; break;
			case 208: s = "invalid TypeParameterConstraintsClauseBase"; break;
			case 209: s = "invalid QueryExpressionBody"; break;

			default: s = "error " + errorNumber; break;
		}
		this.Errors.Error(line, col, s);
	}
	
	private bool StartOf(int s)
	{
		return set[s, lexer.LookAhead.kind];
	}
	
	static bool[,] set = {
	{T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, T,T,x,x, x,x,x,x, T,T,T,x, x,x,x,T, x,x,x,T, x,T,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,T,x,x, x,x,x,x, T,T,T,x, x,x,x,T, x,x,x,T, x,T,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, T,T,T,x, x,x,x,T, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,x, x,T,x,x, x,x,T,x, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,T,T,x, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, T,x,x,T, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,T,x, x,T,T,x, x,x,T,T, T,x,T,x, x,x,x,x, T,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,T,T,T, x,x,x,x, x,x,x,x, x,x,T,x, T,T,x,x, T,x,x,T, x,T,x,T, T,T,T,x, T,x,x,x, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
	{x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,T,x,T, T,T,T,T, T,T,x,T, T,T,T,T, T,T,T,T, T,T,T,T, x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, x,T,T,x, T,x,T,x, x,T,x,T, T,x,T,x, T,x,T,x, T,T,T,T, x,x,T,T, x,x,x,x, T,x,T,T, T,T,x,T, x,T,x,T, x,x,T,x, T,T,T,T, x,x,T,T, T,x,x,T, T,T,x,x, x,x,x,x, T,T,x,T, T,x,T,T, T,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,x, x,T,x,x, x,x,T,x, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,T, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,x, T,x,x,x, x,x,x,x, T,T,x,x, T,x,x,T, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,x, x,T,x,x, x,x,T,x, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,T, x,T,x,T, x,x,x,x, T,x,T,x, T,x,x,x, T,x,x,x, x,x,x,x, T,T,x,x, T,x,x,T, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, T,x,T,x, x,T,x,T, T,x,T,x, T,x,T,x, T,T,T,T, x,x,T,T, x,x,x,x, T,x,T,T, T,x,x,T, x,T,x,T, x,x,T,x, T,T,T,T, x,x,T,T, T,x,x,T, T,T,x,x, x,x,x,x, T,T,x,T, T,x,T,T, T,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, T,x,T,x, x,T,x,T, T,x,T,x, T,x,T,x, T,T,T,T, x,x,T,T, x,x,x,x, T,x,T,T, T,x,x,T, x,T,x,T, x,x,T,x, T,T,T,T, x,x,T,T, T,x,x,T, T,T,x,x, x,x,x,x, T,T,x,T, T,x,T,T, T,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,x, x,T,x,x, x,x,T,x, x,x,T,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,T,x,T, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,x, T,x,x,x, x,x,x,x, T,T,x,x, T,x,x,T, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,x, x,T,x,x, x,x,T,x, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,T, x,T,x,x, x,x,x,x, T,x,T,x, T,x,x,x, T,x,x,x, x,x,x,x, T,T,x,x, T,x,x,T, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,x, T,x,x,x, x,T,x,x, x,x,T,x, T,T,T,T, x,x,x,T, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,x, T,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,x, x,T,x,T, T,x,T,x, T,x,T,x, T,T,T,x, x,x,x,T, x,x,x,x, T,x,T,T, x,x,x,T, x,x,x,T, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,x, T,T,x,x, x,x,x,x, T,T,x,x, T,x,x,T, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,x, x,T,x,x, x,x,T,x, x,x,T,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,T, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,x, T,x,x,x, x,x,x,x, T,T,x,x, T,x,x,T, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,T,T,x, T,T,T,x, x,x,x,T, x,x,x,x, T,x,x,x, T,x,x,x, T,x,x,T, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,T,x, x,T,T,x, T,T,T,T, T,T,T,x, x,x,x,x, T,x,T,T, T,T,T,T, x,x,T,x, x,x,T,T, x,T,T,T, x,x,x,x, x,x,x,x, x,T,T,x, T,T,x,x, T,x,T,T, T,T,T,T, T,T,T,T, T,T,x,x, x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
	{x,T,T,x, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, T,x,x,T, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,T,x, x,T,T,x, x,x,T,T, T,x,T,x, x,x,x,x, T,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,T,T,T, x,T,x,x, x,x,x,x, T,x,T,x, T,T,x,x, T,x,x,T, x,T,x,T, T,T,T,x, T,x,x,x, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,T,T,x, T,T,T,x, x,x,x,T, x,x,x,x, T,x,x,x, T,x,x,x, T,x,x,T, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,T,x, x,T,T,x, x,T,T,T, T,T,T,x, x,x,x,x, T,x,T,T, T,T,T,T, x,x,T,x, x,x,T,T, x,T,T,T, x,x,x,x, x,x,x,x, x,T,T,x, T,T,x,x, T,x,T,T, T,T,T,T, T,T,T,T, T,T,x,x, x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
	{x,T,T,x, T,T,T,x, x,x,x,x, x,x,x,x, T,x,x,x, T,x,x,x, T,x,x,T, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,T,x, x,T,T,x, x,x,T,T, T,x,T,x, x,x,x,x, T,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,T,T,T, x,x,x,x, x,x,x,x, x,x,T,x, T,T,T,x, T,x,x,T, x,T,x,T, T,T,T,x, T,x,x,x, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
	{x,T,T,x, T,T,T,x, x,x,x,T, x,x,x,x, T,x,x,x, T,x,x,x, T,x,x,T, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,T,T, x,T,T,x, T,T,T,T, T,T,T,x, x,x,x,x, T,x,T,T, T,T,T,T, x,x,T,x, x,x,T,T, x,T,T,T, x,x,x,x, x,x,x,x, x,T,T,x, T,T,x,x, T,x,T,T, T,T,T,T, T,T,T,T, T,T,x,x, x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
	{x,x,x,x, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,T, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,x, x,T,x,x, x,x,T,x, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,T, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,x, T,x,x,x, x,x,x,x, T,T,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x}

	};
} // end Parser

}