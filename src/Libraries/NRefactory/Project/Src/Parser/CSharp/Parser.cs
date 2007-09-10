
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
	const int maxT = 145;

	const  bool   T            = true;
	const  bool   x            = false;
	

#line  18 "cs.ATG" 


/*

*/

	void CS() {

#line  179 "cs.ATG" 
		lexer.NextToken(); /* get the first token */ 
		while (la.kind == 121) {
			UsingDirective();
		}
		while (
#line  182 "cs.ATG" 
IsGlobalAttrTarget()) {
			GlobalAttributeSection();
		}
		while (StartOf(1)) {
			NamespaceMemberDecl();
		}
		Expect(0);
	}

	void UsingDirective() {

#line  189 "cs.ATG" 
		string qualident = null; TypeReference aliasedType = null;
		
		Expect(121);

#line  192 "cs.ATG" 
		Location startPos = t.Location; 
		Qualident(
#line  193 "cs.ATG" 
out qualident);
		if (la.kind == 3) {
			lexer.NextToken();
			NonArrayType(
#line  194 "cs.ATG" 
out aliasedType);
		}
		Expect(11);

#line  196 "cs.ATG" 
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

#line  212 "cs.ATG" 
		Location startPos = t.Location; 
		Identifier();

#line  213 "cs.ATG" 
		if (t.val != "assembly") Error("global attribute target specifier (\"assembly\") expected");
		string attributeTarget = t.val;
		List<ASTAttribute> attributes = new List<ASTAttribute>();
		ASTAttribute attribute;
		
		Expect(9);
		Attribute(
#line  218 "cs.ATG" 
out attribute);

#line  218 "cs.ATG" 
		attributes.Add(attribute); 
		while (
#line  219 "cs.ATG" 
NotFinalComma()) {
			Expect(14);
			Attribute(
#line  219 "cs.ATG" 
out attribute);

#line  219 "cs.ATG" 
			attributes.Add(attribute); 
		}
		if (la.kind == 14) {
			lexer.NextToken();
		}
		Expect(19);

#line  221 "cs.ATG" 
		AttributeSection section = new AttributeSection(attributeTarget, attributes);
		section.StartLocation = startPos;
		section.EndLocation = t.EndLocation;
		compilationUnit.AddChild(section);
		
	}

	void NamespaceMemberDecl() {

#line  312 "cs.ATG" 
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		ModifierList m = new ModifierList();
		string qualident;
		
		if (la.kind == 88) {
			lexer.NextToken();

#line  318 "cs.ATG" 
			Location startPos = t.Location; 
			Qualident(
#line  319 "cs.ATG" 
out qualident);

#line  319 "cs.ATG" 
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

#line  328 "cs.ATG" 
			node.EndLocation   = t.EndLocation;
			compilationUnit.BlockEnd();
			
		} else if (StartOf(2)) {
			while (la.kind == 18) {
				AttributeSection(
#line  332 "cs.ATG" 
out section);

#line  332 "cs.ATG" 
				attributes.Add(section); 
			}
			while (StartOf(3)) {
				TypeModifier(
#line  333 "cs.ATG" 
m);
			}
			TypeDecl(
#line  334 "cs.ATG" 
m, attributes);
		} else SynErr(146);
	}

	void Qualident(
#line  456 "cs.ATG" 
out string qualident) {
		Identifier();

#line  458 "cs.ATG" 
		qualidentBuilder.Length = 0; qualidentBuilder.Append(t.val); 
		while (
#line  459 "cs.ATG" 
DotAndIdent()) {
			Expect(15);
			Identifier();

#line  459 "cs.ATG" 
			qualidentBuilder.Append('.');
			qualidentBuilder.Append(t.val); 
			
		}

#line  462 "cs.ATG" 
		qualident = qualidentBuilder.ToString(); 
	}

	void NonArrayType(
#line  569 "cs.ATG" 
out TypeReference type) {

#line  571 "cs.ATG" 
		string name;
		int pointer = 0;
		type = null;
		
		if (StartOf(4)) {
			ClassType(
#line  576 "cs.ATG" 
out type, false);
		} else if (StartOf(5)) {
			SimpleType(
#line  577 "cs.ATG" 
out name);

#line  577 "cs.ATG" 
			type = new TypeReference(name); 
		} else if (la.kind == 123) {
			lexer.NextToken();
			Expect(6);

#line  578 "cs.ATG" 
			pointer = 1; type = new TypeReference("void"); 
		} else SynErr(147);
		if (la.kind == 12) {
			NullableQuestionMark(
#line  581 "cs.ATG" 
ref type);
		}
		while (
#line  583 "cs.ATG" 
IsPointer()) {
			Expect(6);

#line  584 "cs.ATG" 
			++pointer; 
		}

#line  586 "cs.ATG" 
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
		case 141: {
			lexer.NextToken();
			break;
		}
		case 142: {
			lexer.NextToken();
			break;
		}
		case 143: {
			lexer.NextToken();
			break;
		}
		case 144: {
			lexer.NextToken();
			break;
		}
		default: SynErr(148); break;
		}
	}

	void Attribute(
#line  228 "cs.ATG" 
out ASTAttribute attribute) {

#line  229 "cs.ATG" 
		string qualident;
		string alias = null;
		
		if (
#line  233 "cs.ATG" 
IdentAndDoubleColon()) {
			Identifier();

#line  234 "cs.ATG" 
			alias = t.val; 
			Expect(10);
		}
		Qualident(
#line  237 "cs.ATG" 
out qualident);

#line  238 "cs.ATG" 
		List<Expression> positional = new List<Expression>();
		List<NamedArgumentExpression> named = new List<NamedArgumentExpression>();
		string name = (alias != null && alias != "global") ? alias + "." + qualident : qualident;
		
		if (la.kind == 20) {
			AttributeArguments(
#line  242 "cs.ATG" 
positional, named);
		}

#line  242 "cs.ATG" 
		attribute  = new ASTAttribute(name, positional, named);
	}

	void AttributeArguments(
#line  245 "cs.ATG" 
List<Expression> positional, List<NamedArgumentExpression> named) {

#line  247 "cs.ATG" 
		bool nameFound = false;
		string name = "";
		Expression expr;
		
		Expect(20);
		if (StartOf(6)) {
			if (
#line  255 "cs.ATG" 
IsAssignment()) {

#line  255 "cs.ATG" 
				nameFound = true; 
				Identifier();

#line  256 "cs.ATG" 
				name = t.val; 
				Expect(3);
			}
			Expr(
#line  258 "cs.ATG" 
out expr);

#line  258 "cs.ATG" 
			if (expr != null) {if(name == "") positional.Add(expr);
			else { named.Add(new NamedArgumentExpression(name, expr)); name = ""; }
			}
			
			while (la.kind == 14) {
				lexer.NextToken();
				if (
#line  266 "cs.ATG" 
IsAssignment()) {

#line  266 "cs.ATG" 
					nameFound = true; 
					Identifier();

#line  267 "cs.ATG" 
					name = t.val; 
					Expect(3);
				} else if (StartOf(6)) {

#line  269 "cs.ATG" 
					if (nameFound) Error("no positional argument after named argument"); 
				} else SynErr(149);
				Expr(
#line  270 "cs.ATG" 
out expr);

#line  270 "cs.ATG" 
				if (expr != null) { if(name == "") positional.Add(expr);
				else { named.Add(new NamedArgumentExpression(name, expr)); name = ""; }
				}
				
			}
		}
		Expect(21);
	}

	void Expr(
#line  1670 "cs.ATG" 
out Expression expr) {

#line  1671 "cs.ATG" 
		expr = null; Expression expr1 = null, expr2 = null; AssignmentOperatorType op; 
		UnaryExpr(
#line  1673 "cs.ATG" 
out expr);
		if (StartOf(7)) {
			AssignmentOperator(
#line  1676 "cs.ATG" 
out op);
			Expr(
#line  1676 "cs.ATG" 
out expr1);

#line  1676 "cs.ATG" 
			expr = new AssignmentExpression(expr, op, expr1); 
		} else if (
#line  1677 "cs.ATG" 
la.kind == Tokens.GreaterThan && Peek(1).kind == Tokens.GreaterEqual) {
			AssignmentOperator(
#line  1678 "cs.ATG" 
out op);
			Expr(
#line  1678 "cs.ATG" 
out expr1);

#line  1678 "cs.ATG" 
			expr = new AssignmentExpression(expr, op, expr1); 
		} else if (StartOf(8)) {
			ConditionalOrExpr(
#line  1680 "cs.ATG" 
ref expr);
			if (la.kind == 13) {
				lexer.NextToken();
				Expr(
#line  1681 "cs.ATG" 
out expr1);

#line  1681 "cs.ATG" 
				expr = new BinaryOperatorExpression(expr, BinaryOperatorType.NullCoalescing, expr1); 
			}
			if (la.kind == 12) {
				lexer.NextToken();
				Expr(
#line  1682 "cs.ATG" 
out expr1);
				Expect(9);
				Expr(
#line  1682 "cs.ATG" 
out expr2);

#line  1682 "cs.ATG" 
				expr = new ConditionalExpression(expr, expr1, expr2);  
			}
		} else SynErr(150);
	}

	void AttributeSection(
#line  279 "cs.ATG" 
out AttributeSection section) {

#line  281 "cs.ATG" 
		string attributeTarget = "";
		List<ASTAttribute> attributes = new List<ASTAttribute>();
		ASTAttribute attribute;
		
		
		Expect(18);

#line  287 "cs.ATG" 
		Location startPos = t.Location; 
		if (
#line  288 "cs.ATG" 
IsLocalAttrTarget()) {
			if (la.kind == 69) {
				lexer.NextToken();

#line  289 "cs.ATG" 
				attributeTarget = "event";
			} else if (la.kind == 101) {
				lexer.NextToken();

#line  290 "cs.ATG" 
				attributeTarget = "return";
			} else {
				Identifier();

#line  291 "cs.ATG" 
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
#line  301 "cs.ATG" 
out attribute);

#line  301 "cs.ATG" 
		attributes.Add(attribute); 
		while (
#line  302 "cs.ATG" 
NotFinalComma()) {
			Expect(14);
			Attribute(
#line  302 "cs.ATG" 
out attribute);

#line  302 "cs.ATG" 
			attributes.Add(attribute); 
		}
		if (la.kind == 14) {
			lexer.NextToken();
		}
		Expect(19);

#line  304 "cs.ATG" 
		section = new AttributeSection(attributeTarget, attributes);
		section.StartLocation = startPos;
		section.EndLocation = t.EndLocation;
		
	}

	void TypeModifier(
#line  656 "cs.ATG" 
ModifierList m) {
		switch (la.kind) {
		case 89: {
			lexer.NextToken();

#line  658 "cs.ATG" 
			m.Add(Modifiers.New, t.Location); 
			break;
		}
		case 98: {
			lexer.NextToken();

#line  659 "cs.ATG" 
			m.Add(Modifiers.Public, t.Location); 
			break;
		}
		case 97: {
			lexer.NextToken();

#line  660 "cs.ATG" 
			m.Add(Modifiers.Protected, t.Location); 
			break;
		}
		case 84: {
			lexer.NextToken();

#line  661 "cs.ATG" 
			m.Add(Modifiers.Internal, t.Location); 
			break;
		}
		case 96: {
			lexer.NextToken();

#line  662 "cs.ATG" 
			m.Add(Modifiers.Private, t.Location); 
			break;
		}
		case 119: {
			lexer.NextToken();

#line  663 "cs.ATG" 
			m.Add(Modifiers.Unsafe, t.Location); 
			break;
		}
		case 49: {
			lexer.NextToken();

#line  664 "cs.ATG" 
			m.Add(Modifiers.Abstract, t.Location); 
			break;
		}
		case 103: {
			lexer.NextToken();

#line  665 "cs.ATG" 
			m.Add(Modifiers.Sealed, t.Location); 
			break;
		}
		case 107: {
			lexer.NextToken();

#line  666 "cs.ATG" 
			m.Add(Modifiers.Static, t.Location); 
			break;
		}
		case 126: {
			lexer.NextToken();

#line  667 "cs.ATG" 
			m.Add(Modifiers.Partial, t.Location); 
			break;
		}
		default: SynErr(151); break;
		}
	}

	void TypeDecl(
#line  337 "cs.ATG" 
ModifierList m, List<AttributeSection> attributes) {

#line  339 "cs.ATG" 
		TypeReference type;
		List<TypeReference> names;
		List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
		string name;
		List<TemplateDefinition> templates;
		
		if (la.kind == 59) {

#line  345 "cs.ATG" 
			m.Check(Modifiers.Classes); 
			lexer.NextToken();

#line  346 "cs.ATG" 
			TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
			templates = newType.Templates;
			compilationUnit.AddChild(newType);
			compilationUnit.BlockStart(newType);
			newType.StartLocation = m.GetDeclarationLocation(t.Location);
			
			newType.Type = Types.Class;
			
			Identifier();

#line  354 "cs.ATG" 
			newType.Name = t.val; 
			if (la.kind == 23) {
				TypeParameterList(
#line  357 "cs.ATG" 
templates);
			}
			if (la.kind == 9) {
				ClassBase(
#line  359 "cs.ATG" 
out names);

#line  359 "cs.ATG" 
				newType.BaseTypes = names; 
			}
			while (la.kind == 127) {
				TypeParameterConstraintsClause(
#line  362 "cs.ATG" 
templates);
			}

#line  364 "cs.ATG" 
			newType.BodyStartLocation = t.EndLocation; 
			Expect(16);
			ClassBody();
			Expect(17);
			if (la.kind == 11) {
				lexer.NextToken();
			}

#line  366 "cs.ATG" 
			newType.EndLocation = t.Location; 
			compilationUnit.BlockEnd();
			
		} else if (StartOf(9)) {

#line  369 "cs.ATG" 
			m.Check(Modifiers.StructsInterfacesEnumsDelegates); 
			if (la.kind == 109) {
				lexer.NextToken();

#line  370 "cs.ATG" 
				TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
				templates = newType.Templates;
				newType.StartLocation = m.GetDeclarationLocation(t.Location);
				compilationUnit.AddChild(newType);
				compilationUnit.BlockStart(newType);
				newType.Type = Types.Struct; 
				
				Identifier();

#line  377 "cs.ATG" 
				newType.Name = t.val; 
				if (la.kind == 23) {
					TypeParameterList(
#line  380 "cs.ATG" 
templates);
				}
				if (la.kind == 9) {
					StructInterfaces(
#line  382 "cs.ATG" 
out names);

#line  382 "cs.ATG" 
					newType.BaseTypes = names; 
				}
				while (la.kind == 127) {
					TypeParameterConstraintsClause(
#line  385 "cs.ATG" 
templates);
				}

#line  388 "cs.ATG" 
				newType.BodyStartLocation = t.EndLocation; 
				StructBody();
				if (la.kind == 11) {
					lexer.NextToken();
				}

#line  390 "cs.ATG" 
				newType.EndLocation = t.Location; 
				compilationUnit.BlockEnd();
				
			} else if (la.kind == 83) {
				lexer.NextToken();

#line  394 "cs.ATG" 
				TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
				templates = newType.Templates;
				compilationUnit.AddChild(newType);
				compilationUnit.BlockStart(newType);
				newType.StartLocation = m.GetDeclarationLocation(t.Location);
				newType.Type = Types.Interface;
				
				Identifier();

#line  401 "cs.ATG" 
				newType.Name = t.val; 
				if (la.kind == 23) {
					TypeParameterList(
#line  404 "cs.ATG" 
templates);
				}
				if (la.kind == 9) {
					InterfaceBase(
#line  406 "cs.ATG" 
out names);

#line  406 "cs.ATG" 
					newType.BaseTypes = names; 
				}
				while (la.kind == 127) {
					TypeParameterConstraintsClause(
#line  409 "cs.ATG" 
templates);
				}

#line  411 "cs.ATG" 
				newType.BodyStartLocation = t.EndLocation; 
				InterfaceBody();
				if (la.kind == 11) {
					lexer.NextToken();
				}

#line  413 "cs.ATG" 
				newType.EndLocation = t.Location; 
				compilationUnit.BlockEnd();
				
			} else if (la.kind == 68) {
				lexer.NextToken();

#line  417 "cs.ATG" 
				TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
				compilationUnit.AddChild(newType);
				compilationUnit.BlockStart(newType);
				newType.StartLocation = m.GetDeclarationLocation(t.Location);
				newType.Type = Types.Enum;
				
				Identifier();

#line  423 "cs.ATG" 
				newType.Name = t.val; 
				if (la.kind == 9) {
					lexer.NextToken();
					IntegralType(
#line  424 "cs.ATG" 
out name);

#line  424 "cs.ATG" 
					newType.BaseTypes.Add(new TypeReference(name)); 
				}

#line  426 "cs.ATG" 
				newType.BodyStartLocation = t.EndLocation; 
				EnumBody();
				if (la.kind == 11) {
					lexer.NextToken();
				}

#line  428 "cs.ATG" 
				newType.EndLocation = t.Location; 
				compilationUnit.BlockEnd();
				
			} else {
				lexer.NextToken();

#line  432 "cs.ATG" 
				DelegateDeclaration delegateDeclr = new DelegateDeclaration(m.Modifier, attributes);
				templates = delegateDeclr.Templates;
				delegateDeclr.StartLocation = m.GetDeclarationLocation(t.Location);
				
				if (
#line  436 "cs.ATG" 
NotVoidPointer()) {
					Expect(123);

#line  436 "cs.ATG" 
					delegateDeclr.ReturnType = new TypeReference("void", 0, null); 
				} else if (StartOf(10)) {
					Type(
#line  437 "cs.ATG" 
out type);

#line  437 "cs.ATG" 
					delegateDeclr.ReturnType = type; 
				} else SynErr(152);
				Identifier();

#line  439 "cs.ATG" 
				delegateDeclr.Name = t.val; 
				if (la.kind == 23) {
					TypeParameterList(
#line  442 "cs.ATG" 
templates);
				}
				Expect(20);
				if (StartOf(11)) {
					FormalParameterList(
#line  444 "cs.ATG" 
p);

#line  444 "cs.ATG" 
					delegateDeclr.Parameters = p; 
				}
				Expect(21);
				while (la.kind == 127) {
					TypeParameterConstraintsClause(
#line  448 "cs.ATG" 
templates);
				}
				Expect(11);

#line  450 "cs.ATG" 
				delegateDeclr.EndLocation = t.Location;
				compilationUnit.AddChild(delegateDeclr);
				
			}
		} else SynErr(153);
	}

	void TypeParameterList(
#line  2217 "cs.ATG" 
List<TemplateDefinition> templates) {

#line  2219 "cs.ATG" 
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		
		Expect(23);
		while (la.kind == 18) {
			AttributeSection(
#line  2223 "cs.ATG" 
out section);

#line  2223 "cs.ATG" 
			attributes.Add(section); 
		}
		Identifier();

#line  2224 "cs.ATG" 
		templates.Add(new TemplateDefinition(t.val, attributes)); 
		while (la.kind == 14) {
			lexer.NextToken();
			while (la.kind == 18) {
				AttributeSection(
#line  2225 "cs.ATG" 
out section);

#line  2225 "cs.ATG" 
				attributes.Add(section); 
			}
			Identifier();

#line  2226 "cs.ATG" 
			templates.Add(new TemplateDefinition(t.val, attributes)); 
		}
		Expect(22);
	}

	void ClassBase(
#line  465 "cs.ATG" 
out List<TypeReference> names) {

#line  467 "cs.ATG" 
		TypeReference typeRef;
		names = new List<TypeReference>();
		
		Expect(9);
		ClassType(
#line  471 "cs.ATG" 
out typeRef, false);

#line  471 "cs.ATG" 
		if (typeRef != null) { names.Add(typeRef); } 
		while (la.kind == 14) {
			lexer.NextToken();
			TypeName(
#line  472 "cs.ATG" 
out typeRef, false);

#line  472 "cs.ATG" 
			if (typeRef != null) { names.Add(typeRef); } 
		}
	}

	void TypeParameterConstraintsClause(
#line  2230 "cs.ATG" 
List<TemplateDefinition> templates) {

#line  2231 "cs.ATG" 
		string name = ""; TypeReference type; 
		Expect(127);
		Identifier();

#line  2234 "cs.ATG" 
		name = t.val; 
		Expect(9);
		TypeParameterConstraintsClauseBase(
#line  2236 "cs.ATG" 
out type);

#line  2237 "cs.ATG" 
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
#line  2246 "cs.ATG" 
out type);

#line  2247 "cs.ATG" 
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

#line  476 "cs.ATG" 
		AttributeSection section; 
		while (StartOf(12)) {

#line  478 "cs.ATG" 
			List<AttributeSection> attributes = new List<AttributeSection>();
			ModifierList m = new ModifierList();
			
			while (la.kind == 18) {
				AttributeSection(
#line  481 "cs.ATG" 
out section);

#line  481 "cs.ATG" 
				attributes.Add(section); 
			}
			MemberModifiers(
#line  482 "cs.ATG" 
m);
			ClassMemberDecl(
#line  483 "cs.ATG" 
m, attributes);
		}
	}

	void StructInterfaces(
#line  487 "cs.ATG" 
out List<TypeReference> names) {

#line  489 "cs.ATG" 
		TypeReference typeRef;
		names = new List<TypeReference>();
		
		Expect(9);
		TypeName(
#line  493 "cs.ATG" 
out typeRef, false);

#line  493 "cs.ATG" 
		if (typeRef != null) { names.Add(typeRef); } 
		while (la.kind == 14) {
			lexer.NextToken();
			TypeName(
#line  494 "cs.ATG" 
out typeRef, false);

#line  494 "cs.ATG" 
			if (typeRef != null) { names.Add(typeRef); } 
		}
	}

	void StructBody() {

#line  498 "cs.ATG" 
		AttributeSection section; 
		Expect(16);
		while (StartOf(13)) {

#line  501 "cs.ATG" 
			List<AttributeSection> attributes = new List<AttributeSection>();
			ModifierList m = new ModifierList();
			
			while (la.kind == 18) {
				AttributeSection(
#line  504 "cs.ATG" 
out section);

#line  504 "cs.ATG" 
				attributes.Add(section); 
			}
			MemberModifiers(
#line  505 "cs.ATG" 
m);
			StructMemberDecl(
#line  506 "cs.ATG" 
m, attributes);
		}
		Expect(17);
	}

	void InterfaceBase(
#line  511 "cs.ATG" 
out List<TypeReference> names) {

#line  513 "cs.ATG" 
		TypeReference typeRef;
		names = new List<TypeReference>();
		
		Expect(9);
		TypeName(
#line  517 "cs.ATG" 
out typeRef, false);

#line  517 "cs.ATG" 
		if (typeRef != null) { names.Add(typeRef); } 
		while (la.kind == 14) {
			lexer.NextToken();
			TypeName(
#line  518 "cs.ATG" 
out typeRef, false);

#line  518 "cs.ATG" 
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
#line  678 "cs.ATG" 
out string name) {

#line  678 "cs.ATG" 
		name = ""; 
		switch (la.kind) {
		case 102: {
			lexer.NextToken();

#line  680 "cs.ATG" 
			name = "sbyte"; 
			break;
		}
		case 54: {
			lexer.NextToken();

#line  681 "cs.ATG" 
			name = "byte"; 
			break;
		}
		case 104: {
			lexer.NextToken();

#line  682 "cs.ATG" 
			name = "short"; 
			break;
		}
		case 120: {
			lexer.NextToken();

#line  683 "cs.ATG" 
			name = "ushort"; 
			break;
		}
		case 82: {
			lexer.NextToken();

#line  684 "cs.ATG" 
			name = "int"; 
			break;
		}
		case 116: {
			lexer.NextToken();

#line  685 "cs.ATG" 
			name = "uint"; 
			break;
		}
		case 87: {
			lexer.NextToken();

#line  686 "cs.ATG" 
			name = "long"; 
			break;
		}
		case 117: {
			lexer.NextToken();

#line  687 "cs.ATG" 
			name = "ulong"; 
			break;
		}
		case 57: {
			lexer.NextToken();

#line  688 "cs.ATG" 
			name = "char"; 
			break;
		}
		default: SynErr(154); break;
		}
	}

	void EnumBody() {

#line  527 "cs.ATG" 
		FieldDeclaration f; 
		Expect(16);
		if (StartOf(15)) {
			EnumMemberDecl(
#line  530 "cs.ATG" 
out f);

#line  530 "cs.ATG" 
			compilationUnit.AddChild(f); 
			while (
#line  531 "cs.ATG" 
NotFinalComma()) {
				Expect(14);
				EnumMemberDecl(
#line  532 "cs.ATG" 
out f);

#line  532 "cs.ATG" 
				compilationUnit.AddChild(f); 
			}
			if (la.kind == 14) {
				lexer.NextToken();
			}
		}
		Expect(17);
	}

	void Type(
#line  537 "cs.ATG" 
out TypeReference type) {
		TypeWithRestriction(
#line  539 "cs.ATG" 
out type, true, false);
	}

	void FormalParameterList(
#line  600 "cs.ATG" 
List<ParameterDeclarationExpression> parameter) {

#line  603 "cs.ATG" 
		ParameterDeclarationExpression p;
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		
		while (la.kind == 18) {
			AttributeSection(
#line  608 "cs.ATG" 
out section);

#line  608 "cs.ATG" 
			attributes.Add(section); 
		}
		if (StartOf(16)) {
			FixedParameter(
#line  610 "cs.ATG" 
out p);

#line  610 "cs.ATG" 
			bool paramsFound = false;
			p.Attributes = attributes;
			parameter.Add(p);
			
			while (la.kind == 14) {
				lexer.NextToken();

#line  615 "cs.ATG" 
				attributes = new List<AttributeSection>(); if (paramsFound) Error("params array must be at end of parameter list"); 
				while (la.kind == 18) {
					AttributeSection(
#line  616 "cs.ATG" 
out section);

#line  616 "cs.ATG" 
					attributes.Add(section); 
				}
				if (StartOf(16)) {
					FixedParameter(
#line  618 "cs.ATG" 
out p);

#line  618 "cs.ATG" 
					p.Attributes = attributes; parameter.Add(p); 
				} else if (la.kind == 95) {
					ParameterArray(
#line  619 "cs.ATG" 
out p);

#line  619 "cs.ATG" 
					paramsFound = true; p.Attributes = attributes; parameter.Add(p); 
				} else SynErr(155);
			}
		} else if (la.kind == 95) {
			ParameterArray(
#line  622 "cs.ATG" 
out p);

#line  622 "cs.ATG" 
			p.Attributes = attributes; parameter.Add(p); 
		} else SynErr(156);
	}

	void ClassType(
#line  670 "cs.ATG" 
out TypeReference typeRef, bool canBeUnbound) {

#line  671 "cs.ATG" 
		TypeReference r; typeRef = null; 
		if (StartOf(17)) {
			TypeName(
#line  673 "cs.ATG" 
out r, canBeUnbound);

#line  673 "cs.ATG" 
			typeRef = r; 
		} else if (la.kind == 91) {
			lexer.NextToken();

#line  674 "cs.ATG" 
			typeRef = new TypeReference("object"); 
		} else if (la.kind == 108) {
			lexer.NextToken();

#line  675 "cs.ATG" 
			typeRef = new TypeReference("string"); 
		} else SynErr(157);
	}

	void TypeName(
#line  2160 "cs.ATG" 
out TypeReference typeRef, bool canBeUnbound) {

#line  2161 "cs.ATG" 
		List<TypeReference> typeArguments = null;
		string alias = null;
		string qualident;
		
		if (
#line  2166 "cs.ATG" 
IdentAndDoubleColon()) {
			Identifier();

#line  2167 "cs.ATG" 
			alias = t.val; 
			Expect(10);
		}
		Qualident(
#line  2170 "cs.ATG" 
out qualident);
		if (la.kind == 23) {
			TypeArgumentList(
#line  2171 "cs.ATG" 
out typeArguments, canBeUnbound);
		}

#line  2173 "cs.ATG" 
		if (alias == null) {
		typeRef = new TypeReference(qualident, typeArguments);
		} else if (alias == "global") {
			typeRef = new TypeReference(qualident, typeArguments);
			typeRef.IsGlobal = true;
		} else {
			typeRef = new TypeReference(alias + "." + qualident, typeArguments);
		}
		
		while (
#line  2182 "cs.ATG" 
DotAndIdent()) {
			Expect(15);

#line  2183 "cs.ATG" 
			typeArguments = null; 
			Qualident(
#line  2184 "cs.ATG" 
out qualident);
			if (la.kind == 23) {
				TypeArgumentList(
#line  2185 "cs.ATG" 
out typeArguments, canBeUnbound);
			}

#line  2186 "cs.ATG" 
			typeRef = new InnerClassTypeReference(typeRef, qualident, typeArguments); 
		}
	}

	void MemberModifiers(
#line  691 "cs.ATG" 
ModifierList m) {
		while (StartOf(18)) {
			switch (la.kind) {
			case 49: {
				lexer.NextToken();

#line  694 "cs.ATG" 
				m.Add(Modifiers.Abstract, t.Location); 
				break;
			}
			case 71: {
				lexer.NextToken();

#line  695 "cs.ATG" 
				m.Add(Modifiers.Extern, t.Location); 
				break;
			}
			case 84: {
				lexer.NextToken();

#line  696 "cs.ATG" 
				m.Add(Modifiers.Internal, t.Location); 
				break;
			}
			case 89: {
				lexer.NextToken();

#line  697 "cs.ATG" 
				m.Add(Modifiers.New, t.Location); 
				break;
			}
			case 94: {
				lexer.NextToken();

#line  698 "cs.ATG" 
				m.Add(Modifiers.Override, t.Location); 
				break;
			}
			case 96: {
				lexer.NextToken();

#line  699 "cs.ATG" 
				m.Add(Modifiers.Private, t.Location); 
				break;
			}
			case 97: {
				lexer.NextToken();

#line  700 "cs.ATG" 
				m.Add(Modifiers.Protected, t.Location); 
				break;
			}
			case 98: {
				lexer.NextToken();

#line  701 "cs.ATG" 
				m.Add(Modifiers.Public, t.Location); 
				break;
			}
			case 99: {
				lexer.NextToken();

#line  702 "cs.ATG" 
				m.Add(Modifiers.ReadOnly, t.Location); 
				break;
			}
			case 103: {
				lexer.NextToken();

#line  703 "cs.ATG" 
				m.Add(Modifiers.Sealed, t.Location); 
				break;
			}
			case 107: {
				lexer.NextToken();

#line  704 "cs.ATG" 
				m.Add(Modifiers.Static, t.Location); 
				break;
			}
			case 74: {
				lexer.NextToken();

#line  705 "cs.ATG" 
				m.Add(Modifiers.Fixed, t.Location); 
				break;
			}
			case 119: {
				lexer.NextToken();

#line  706 "cs.ATG" 
				m.Add(Modifiers.Unsafe, t.Location); 
				break;
			}
			case 122: {
				lexer.NextToken();

#line  707 "cs.ATG" 
				m.Add(Modifiers.Virtual, t.Location); 
				break;
			}
			case 124: {
				lexer.NextToken();

#line  708 "cs.ATG" 
				m.Add(Modifiers.Volatile, t.Location); 
				break;
			}
			case 126: {
				lexer.NextToken();

#line  709 "cs.ATG" 
				m.Add(Modifiers.Partial, t.Location); 
				break;
			}
			}
		}
	}

	void ClassMemberDecl(
#line  1018 "cs.ATG" 
ModifierList m, List<AttributeSection> attributes) {

#line  1019 "cs.ATG" 
		Statement stmt = null; 
		if (StartOf(19)) {
			StructMemberDecl(
#line  1021 "cs.ATG" 
m, attributes);
		} else if (la.kind == 27) {

#line  1022 "cs.ATG" 
			m.Check(Modifiers.Destructors); Location startPos = t.Location; 
			lexer.NextToken();
			Identifier();

#line  1023 "cs.ATG" 
			DestructorDeclaration d = new DestructorDeclaration(t.val, m.Modifier, attributes); 
			d.Modifier = m.Modifier;
			d.StartLocation = m.GetDeclarationLocation(startPos);
			
			Expect(20);
			Expect(21);

#line  1027 "cs.ATG" 
			d.EndLocation = t.EndLocation; 
			if (la.kind == 16) {
				Block(
#line  1027 "cs.ATG" 
out stmt);
			} else if (la.kind == 11) {
				lexer.NextToken();
			} else SynErr(158);

#line  1028 "cs.ATG" 
			d.Body = (BlockStatement)stmt;
			compilationUnit.AddChild(d);
			
		} else SynErr(159);
	}

	void StructMemberDecl(
#line  713 "cs.ATG" 
ModifierList m, List<AttributeSection> attributes) {

#line  715 "cs.ATG" 
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

#line  726 "cs.ATG" 
			m.Check(Modifiers.Constants); 
			lexer.NextToken();

#line  727 "cs.ATG" 
			Location startPos = t.Location; 
			Type(
#line  728 "cs.ATG" 
out type);
			Identifier();

#line  728 "cs.ATG" 
			FieldDeclaration fd = new FieldDeclaration(attributes, type, m.Modifier | Modifiers.Const);
			fd.StartLocation = m.GetDeclarationLocation(startPos);
			VariableDeclaration f = new VariableDeclaration(t.val);
			fd.Fields.Add(f);
			
			Expect(3);
			Expr(
#line  733 "cs.ATG" 
out expr);

#line  733 "cs.ATG" 
			f.Initializer = expr; 
			while (la.kind == 14) {
				lexer.NextToken();
				Identifier();

#line  734 "cs.ATG" 
				f = new VariableDeclaration(t.val);
				fd.Fields.Add(f);
				
				Expect(3);
				Expr(
#line  737 "cs.ATG" 
out expr);

#line  737 "cs.ATG" 
				f.Initializer = expr; 
			}
			Expect(11);

#line  738 "cs.ATG" 
			fd.EndLocation = t.EndLocation; compilationUnit.AddChild(fd); 
		} else if (
#line  742 "cs.ATG" 
NotVoidPointer()) {

#line  742 "cs.ATG" 
			m.Check(Modifiers.PropertysEventsMethods); 
			Expect(123);

#line  743 "cs.ATG" 
			Location startPos = t.Location; 
			if (
#line  744 "cs.ATG" 
IsExplicitInterfaceImplementation()) {
				TypeName(
#line  745 "cs.ATG" 
out explicitInterface, false);

#line  746 "cs.ATG" 
				if (la.kind != Tokens.Dot || Peek(1).kind != Tokens.This) {
				qualident = TypeReference.StripLastIdentifierFromType(ref explicitInterface);
				 } 
			} else if (StartOf(17)) {
				Identifier();

#line  749 "cs.ATG" 
				qualident = t.val; 
			} else SynErr(160);
			if (la.kind == 23) {
				TypeParameterList(
#line  752 "cs.ATG" 
templates);
			}
			Expect(20);
			if (la.kind == 111) {
				lexer.NextToken();

#line  755 "cs.ATG" 
				isExtensionMethod = true; /* C# 3.0 */ 
			}
			if (StartOf(11)) {
				FormalParameterList(
#line  756 "cs.ATG" 
p);
			}
			Expect(21);

#line  757 "cs.ATG" 
			MethodDeclaration methodDeclaration = new MethodDeclaration {
			Name = qualident,
			Modifier = m.Modifier,
			TypeReference = new TypeReference("void"),
			Parameters = p,
			Attributes = attributes,
			StartLocation = m.GetDeclarationLocation(startPos),
			EndLocation = t.EndLocation,
			Templates = templates,
			IsExtensionMethod = isExtensionMethod
			};
			if (explicitInterface != null)
				methodDeclaration.InterfaceImplementations.Add(new InterfaceImplementation(explicitInterface, qualident));
			compilationUnit.AddChild(methodDeclaration);
			compilationUnit.BlockStart(methodDeclaration);
			
			while (la.kind == 127) {
				TypeParameterConstraintsClause(
#line  775 "cs.ATG" 
templates);
			}
			if (la.kind == 16) {
				Block(
#line  777 "cs.ATG" 
out stmt);
			} else if (la.kind == 11) {
				lexer.NextToken();
			} else SynErr(161);

#line  777 "cs.ATG" 
			compilationUnit.BlockEnd();
			methodDeclaration.Body  = (BlockStatement)stmt;
			
		} else if (la.kind == 69) {

#line  781 "cs.ATG" 
			m.Check(Modifiers.PropertysEventsMethods); 
			lexer.NextToken();

#line  783 "cs.ATG" 
			EventDeclaration eventDecl = new EventDeclaration {
			Modifier = m.Modifier, 
			Attributes = attributes,
			StartLocation = t.Location
			};
			compilationUnit.AddChild(eventDecl);
			compilationUnit.BlockStart(eventDecl);
			EventAddRegion addBlock = null;
			EventRemoveRegion removeBlock = null;
			
			Type(
#line  793 "cs.ATG" 
out type);

#line  793 "cs.ATG" 
			eventDecl.TypeReference = type; 
			if (
#line  794 "cs.ATG" 
IsExplicitInterfaceImplementation()) {
				TypeName(
#line  795 "cs.ATG" 
out explicitInterface, false);

#line  796 "cs.ATG" 
				qualident = TypeReference.StripLastIdentifierFromType(ref explicitInterface); 

#line  797 "cs.ATG" 
				eventDecl.InterfaceImplementations.Add(new InterfaceImplementation(explicitInterface, qualident)); 
			} else if (StartOf(17)) {
				Identifier();

#line  799 "cs.ATG" 
				qualident = t.val; 
			} else SynErr(162);

#line  801 "cs.ATG" 
			eventDecl.Name = qualident; eventDecl.EndLocation = t.EndLocation; 
			if (la.kind == 3) {
				lexer.NextToken();
				Expr(
#line  802 "cs.ATG" 
out expr);

#line  802 "cs.ATG" 
				eventDecl.Initializer = expr; 
			}
			if (la.kind == 16) {
				lexer.NextToken();

#line  803 "cs.ATG" 
				eventDecl.BodyStart = t.Location; 
				EventAccessorDecls(
#line  804 "cs.ATG" 
out addBlock, out removeBlock);
				Expect(17);

#line  805 "cs.ATG" 
				eventDecl.BodyEnd   = t.EndLocation; 
			}
			if (la.kind == 11) {
				lexer.NextToken();
			}

#line  808 "cs.ATG" 
			compilationUnit.BlockEnd();
			eventDecl.AddRegion = addBlock;
			eventDecl.RemoveRegion = removeBlock;
			
		} else if (
#line  814 "cs.ATG" 
IdentAndLPar()) {

#line  814 "cs.ATG" 
			m.Check(Modifiers.Constructors | Modifiers.StaticConstructors); 
			Identifier();

#line  815 "cs.ATG" 
			string name = t.val; Location startPos = t.Location; 
			Expect(20);
			if (StartOf(11)) {

#line  815 "cs.ATG" 
				m.Check(Modifiers.Constructors); 
				FormalParameterList(
#line  816 "cs.ATG" 
p);
			}
			Expect(21);

#line  818 "cs.ATG" 
			ConstructorInitializer init = null;  
			if (la.kind == 9) {

#line  819 "cs.ATG" 
				m.Check(Modifiers.Constructors); 
				ConstructorInitializer(
#line  820 "cs.ATG" 
out init);
			}

#line  822 "cs.ATG" 
			ConstructorDeclaration cd = new ConstructorDeclaration(name, m.Modifier, p, init, attributes); 
			cd.StartLocation = startPos;
			cd.EndLocation   = t.EndLocation;
			
			if (la.kind == 16) {
				Block(
#line  827 "cs.ATG" 
out stmt);
			} else if (la.kind == 11) {
				lexer.NextToken();
			} else SynErr(163);

#line  827 "cs.ATG" 
			cd.Body = (BlockStatement)stmt; compilationUnit.AddChild(cd); 
		} else if (la.kind == 70 || la.kind == 80) {

#line  830 "cs.ATG" 
			m.Check(Modifiers.Operators);
			if (m.isNone) Error("at least one modifier must be set"); 
			bool isImplicit = true;
			Location startPos = Location.Empty;
			
			if (la.kind == 80) {
				lexer.NextToken();

#line  835 "cs.ATG" 
				startPos = t.Location; 
			} else {
				lexer.NextToken();

#line  835 "cs.ATG" 
				isImplicit = false; startPos = t.Location; 
			}
			Expect(92);
			Type(
#line  836 "cs.ATG" 
out type);

#line  836 "cs.ATG" 
			TypeReference operatorType = type; 
			Expect(20);
			Type(
#line  837 "cs.ATG" 
out type);
			Identifier();

#line  837 "cs.ATG" 
			string varName = t.val; 
			Expect(21);

#line  838 "cs.ATG" 
			Location endPos = t.Location; 
			if (la.kind == 16) {
				Block(
#line  839 "cs.ATG" 
out stmt);
			} else if (la.kind == 11) {
				lexer.NextToken();

#line  839 "cs.ATG" 
				stmt = null; 
			} else SynErr(164);

#line  842 "cs.ATG" 
			List<ParameterDeclarationExpression> parameters = new List<ParameterDeclarationExpression>();
			parameters.Add(new ParameterDeclarationExpression(type, varName));
			OperatorDeclaration operatorDeclaration = new OperatorDeclaration {
				Modifier = m.Modifier,
				Attributes = attributes, 
				Parameters = parameters, 
				TypeReference = operatorType,
				ConversionType = isImplicit ? ConversionType.Implicit : ConversionType.Explicit,
				Body = (BlockStatement)stmt,
				StartLocation = m.GetDeclarationLocation(startPos),
				EndLocation = endPos
			};
			compilationUnit.AddChild(operatorDeclaration);
			
		} else if (StartOf(20)) {
			TypeDecl(
#line  859 "cs.ATG" 
m, attributes);
		} else if (StartOf(10)) {
			Type(
#line  861 "cs.ATG" 
out type);

#line  861 "cs.ATG" 
			Location startPos = t.Location;  
			if (la.kind == 92) {

#line  863 "cs.ATG" 
				OverloadableOperatorType op;
				m.Check(Modifiers.Operators);
				if (m.isNone) Error("at least one modifier must be set");
				
				lexer.NextToken();
				OverloadableOperator(
#line  867 "cs.ATG" 
out op);

#line  867 "cs.ATG" 
				TypeReference firstType, secondType = null; string secondName = null; 
				Expect(20);
				Type(
#line  868 "cs.ATG" 
out firstType);
				Identifier();

#line  868 "cs.ATG" 
				string firstName = t.val; 
				if (la.kind == 14) {
					lexer.NextToken();
					Type(
#line  869 "cs.ATG" 
out secondType);
					Identifier();

#line  869 "cs.ATG" 
					secondName = t.val; 
				} else if (la.kind == 21) {
				} else SynErr(165);

#line  877 "cs.ATG" 
				Location endPos = t.Location; 
				Expect(21);
				if (la.kind == 16) {
					Block(
#line  878 "cs.ATG" 
out stmt);
				} else if (la.kind == 11) {
					lexer.NextToken();
				} else SynErr(166);

#line  880 "cs.ATG" 
				List<ParameterDeclarationExpression> parameters = new List<ParameterDeclarationExpression>();
				parameters.Add(new ParameterDeclarationExpression(firstType, firstName));
				if (secondType != null) {
					parameters.Add(new ParameterDeclarationExpression(secondType, secondName));
				}
				OperatorDeclaration operatorDeclaration = new OperatorDeclaration {
					Modifier = m.Modifier,
					Attributes = attributes,
					Parameters = parameters,
					TypeReference = type,
					OverloadableOperator = op,
					Body = (BlockStatement)stmt,
					StartLocation = m.GetDeclarationLocation(startPos),
					EndLocation = endPos
				};
				compilationUnit.AddChild(operatorDeclaration);
				
			} else if (
#line  899 "cs.ATG" 
IsVarDecl()) {

#line  900 "cs.ATG" 
				m.Check(Modifiers.Fields);
				FieldDeclaration fd = new FieldDeclaration(attributes, type, m.Modifier);
				fd.StartLocation = m.GetDeclarationLocation(startPos); 
				
				if (
#line  904 "cs.ATG" 
m.Contains(Modifiers.Fixed)) {
					VariableDeclarator(
#line  905 "cs.ATG" 
variableDeclarators);
					Expect(18);
					Expr(
#line  907 "cs.ATG" 
out expr);

#line  907 "cs.ATG" 
					if (variableDeclarators.Count > 0)
					variableDeclarators[variableDeclarators.Count-1].FixedArrayInitialization = expr; 
					Expect(19);
					while (la.kind == 14) {
						lexer.NextToken();
						VariableDeclarator(
#line  911 "cs.ATG" 
variableDeclarators);
						Expect(18);
						Expr(
#line  913 "cs.ATG" 
out expr);

#line  913 "cs.ATG" 
						if (variableDeclarators.Count > 0)
						variableDeclarators[variableDeclarators.Count-1].FixedArrayInitialization = expr; 
						Expect(19);
					}
				} else if (StartOf(17)) {
					VariableDeclarator(
#line  918 "cs.ATG" 
variableDeclarators);
					while (la.kind == 14) {
						lexer.NextToken();
						VariableDeclarator(
#line  919 "cs.ATG" 
variableDeclarators);
					}
				} else SynErr(167);
				Expect(11);

#line  921 "cs.ATG" 
				fd.EndLocation = t.EndLocation; fd.Fields = variableDeclarators; compilationUnit.AddChild(fd); 
			} else if (la.kind == 111) {

#line  924 "cs.ATG" 
				m.Check(Modifiers.Indexers); 
				lexer.NextToken();
				Expect(18);
				FormalParameterList(
#line  925 "cs.ATG" 
p);
				Expect(19);

#line  925 "cs.ATG" 
				Location endLocation = t.EndLocation; 
				Expect(16);

#line  926 "cs.ATG" 
				IndexerDeclaration indexer = new IndexerDeclaration(type, p, m.Modifier, attributes);
				indexer.StartLocation = startPos;
				indexer.EndLocation   = endLocation;
				indexer.BodyStart     = t.Location;
				PropertyGetRegion getRegion;
				PropertySetRegion setRegion;
				
				AccessorDecls(
#line  933 "cs.ATG" 
out getRegion, out setRegion);
				Expect(17);

#line  934 "cs.ATG" 
				indexer.BodyEnd    = t.EndLocation;
				indexer.GetRegion = getRegion;
				indexer.SetRegion = setRegion;
				compilationUnit.AddChild(indexer);
				
			} else if (
#line  939 "cs.ATG" 
IsIdentifierToken(la)) {
				if (
#line  940 "cs.ATG" 
IsExplicitInterfaceImplementation()) {
					TypeName(
#line  941 "cs.ATG" 
out explicitInterface, false);

#line  942 "cs.ATG" 
					if (la.kind != Tokens.Dot || Peek(1).kind != Tokens.This) {
					qualident = TypeReference.StripLastIdentifierFromType(ref explicitInterface);
					 } 
				} else if (StartOf(17)) {
					Identifier();

#line  945 "cs.ATG" 
					qualident = t.val; 
				} else SynErr(168);

#line  947 "cs.ATG" 
				Location qualIdentEndLocation = t.EndLocation; 
				if (la.kind == 16 || la.kind == 20 || la.kind == 23) {
					if (la.kind == 20 || la.kind == 23) {

#line  951 "cs.ATG" 
						m.Check(Modifiers.PropertysEventsMethods); 
						if (la.kind == 23) {
							TypeParameterList(
#line  953 "cs.ATG" 
templates);
						}
						Expect(20);
						if (la.kind == 111) {
							lexer.NextToken();

#line  955 "cs.ATG" 
							isExtensionMethod = true; 
						}
						if (StartOf(11)) {
							FormalParameterList(
#line  956 "cs.ATG" 
p);
						}
						Expect(21);

#line  958 "cs.ATG" 
						MethodDeclaration methodDeclaration = new MethodDeclaration {
						Name = qualident,
						Modifier = m.Modifier,
						TypeReference = type,
						Parameters = p, 
						Attributes = attributes
						};
						if (explicitInterface != null)
							methodDeclaration.InterfaceImplementations.Add(new InterfaceImplementation(explicitInterface, qualident));
						methodDeclaration.StartLocation = m.GetDeclarationLocation(startPos);
						methodDeclaration.EndLocation   = t.EndLocation;
						methodDeclaration.IsExtensionMethod = isExtensionMethod;
						methodDeclaration.Templates = templates;
						compilationUnit.AddChild(methodDeclaration);
						                                      
						while (la.kind == 127) {
							TypeParameterConstraintsClause(
#line  973 "cs.ATG" 
templates);
						}
						if (la.kind == 16) {
							Block(
#line  974 "cs.ATG" 
out stmt);
						} else if (la.kind == 11) {
							lexer.NextToken();
						} else SynErr(169);

#line  974 "cs.ATG" 
						methodDeclaration.Body  = (BlockStatement)stmt; 
					} else {
						lexer.NextToken();

#line  977 "cs.ATG" 
						PropertyDeclaration pDecl = new PropertyDeclaration(qualident, type, m.Modifier, attributes); 
						if (explicitInterface != null)
						pDecl.InterfaceImplementations.Add(new InterfaceImplementation(explicitInterface, qualident));
						      pDecl.StartLocation = m.GetDeclarationLocation(startPos);
						      pDecl.EndLocation   = qualIdentEndLocation;
						      pDecl.BodyStart   = t.Location;
						      PropertyGetRegion getRegion;
						      PropertySetRegion setRegion;
						   
						AccessorDecls(
#line  986 "cs.ATG" 
out getRegion, out setRegion);
						Expect(17);

#line  988 "cs.ATG" 
						pDecl.GetRegion = getRegion;
						pDecl.SetRegion = setRegion;
						pDecl.BodyEnd = t.EndLocation;
						compilationUnit.AddChild(pDecl);
						
					}
				} else if (la.kind == 15) {

#line  996 "cs.ATG" 
					m.Check(Modifiers.Indexers); 
					lexer.NextToken();
					Expect(111);
					Expect(18);
					FormalParameterList(
#line  997 "cs.ATG" 
p);
					Expect(19);

#line  998 "cs.ATG" 
					IndexerDeclaration indexer = new IndexerDeclaration(type, p, m.Modifier, attributes);
					indexer.StartLocation = m.GetDeclarationLocation(startPos);
					indexer.EndLocation   = t.EndLocation;
					if (explicitInterface != null)
					indexer.InterfaceImplementations.Add(new InterfaceImplementation(explicitInterface, "this"));
					      PropertyGetRegion getRegion;
					      PropertySetRegion setRegion;
					    
					Expect(16);

#line  1006 "cs.ATG" 
					Location bodyStart = t.Location; 
					AccessorDecls(
#line  1007 "cs.ATG" 
out getRegion, out setRegion);
					Expect(17);

#line  1008 "cs.ATG" 
					indexer.BodyStart = bodyStart;
					indexer.BodyEnd   = t.EndLocation;
					indexer.GetRegion = getRegion;
					indexer.SetRegion = setRegion;
					compilationUnit.AddChild(indexer);
					
				} else SynErr(170);
			} else SynErr(171);
		} else SynErr(172);
	}

	void InterfaceMemberDecl() {

#line  1035 "cs.ATG" 
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
#line  1048 "cs.ATG" 
out section);

#line  1048 "cs.ATG" 
			attributes.Add(section); 
		}
		if (la.kind == 89) {
			lexer.NextToken();

#line  1049 "cs.ATG" 
			mod = Modifiers.New; startLocation = t.Location; 
		}
		if (
#line  1052 "cs.ATG" 
NotVoidPointer()) {
			Expect(123);

#line  1052 "cs.ATG" 
			if (startLocation.X == -1) startLocation = t.Location; 
			Identifier();

#line  1053 "cs.ATG" 
			name = t.val; 
			if (la.kind == 23) {
				TypeParameterList(
#line  1054 "cs.ATG" 
templates);
			}
			Expect(20);
			if (StartOf(11)) {
				FormalParameterList(
#line  1055 "cs.ATG" 
parameters);
			}
			Expect(21);
			while (la.kind == 127) {
				TypeParameterConstraintsClause(
#line  1056 "cs.ATG" 
templates);
			}
			Expect(11);

#line  1058 "cs.ATG" 
			MethodDeclaration md = new MethodDeclaration {
			Name = name, Modifier = mod, TypeReference = new TypeReference("void"), 
			Parameters = parameters, Attributes = attributes, Templates = templates,
			StartLocation = startLocation, EndLocation = t.EndLocation
			};
			compilationUnit.AddChild(md);
			
		} else if (StartOf(21)) {
			if (StartOf(10)) {
				Type(
#line  1066 "cs.ATG" 
out type);

#line  1066 "cs.ATG" 
				if (startLocation.X == -1) startLocation = t.Location; 
				if (StartOf(17)) {
					Identifier();

#line  1068 "cs.ATG" 
					name = t.val; Location qualIdentEndLocation = t.EndLocation; 
					if (la.kind == 20 || la.kind == 23) {
						if (la.kind == 23) {
							TypeParameterList(
#line  1072 "cs.ATG" 
templates);
						}
						Expect(20);
						if (StartOf(11)) {
							FormalParameterList(
#line  1073 "cs.ATG" 
parameters);
						}
						Expect(21);
						while (la.kind == 127) {
							TypeParameterConstraintsClause(
#line  1075 "cs.ATG" 
templates);
						}
						Expect(11);

#line  1076 "cs.ATG" 
						MethodDeclaration md = new MethodDeclaration {
						Name = name, Modifier = mod, TypeReference = type,
						Parameters = parameters, Attributes = attributes, Templates = templates,
						StartLocation = startLocation, EndLocation = t.EndLocation
						};
						compilationUnit.AddChild(md);
						
					} else if (la.kind == 16) {

#line  1084 "cs.ATG" 
						PropertyDeclaration pd = new PropertyDeclaration(name, type, mod, attributes); compilationUnit.AddChild(pd); 
						lexer.NextToken();

#line  1085 "cs.ATG" 
						Location bodyStart = t.Location;
						InterfaceAccessors(
#line  1085 "cs.ATG" 
out getBlock, out setBlock);
						Expect(17);

#line  1085 "cs.ATG" 
						pd.GetRegion = getBlock; pd.SetRegion = setBlock; pd.StartLocation = startLocation; pd.EndLocation = qualIdentEndLocation; pd.BodyStart = bodyStart; pd.BodyEnd = t.EndLocation; 
					} else SynErr(173);
				} else if (la.kind == 111) {
					lexer.NextToken();
					Expect(18);
					FormalParameterList(
#line  1088 "cs.ATG" 
parameters);
					Expect(19);

#line  1088 "cs.ATG" 
					Location bracketEndLocation = t.EndLocation; 

#line  1088 "cs.ATG" 
					IndexerDeclaration id = new IndexerDeclaration(type, parameters, mod, attributes); compilationUnit.AddChild(id); 
					Expect(16);

#line  1089 "cs.ATG" 
					Location bodyStart = t.Location;
					InterfaceAccessors(
#line  1089 "cs.ATG" 
out getBlock, out setBlock);
					Expect(17);

#line  1089 "cs.ATG" 
					id.GetRegion = getBlock; id.SetRegion = setBlock; id.StartLocation = startLocation;  id.EndLocation = bracketEndLocation; id.BodyStart = bodyStart; id.BodyEnd = t.EndLocation;
				} else SynErr(174);
			} else {
				lexer.NextToken();

#line  1092 "cs.ATG" 
				if (startLocation.X == -1) startLocation = t.Location; 
				Type(
#line  1093 "cs.ATG" 
out type);
				Identifier();

#line  1094 "cs.ATG" 
				EventDeclaration ed = new EventDeclaration {
				TypeReference = type, Name = t.val, Modifier = mod, Attributes = attributes
				};
				compilationUnit.AddChild(ed);
				
				Expect(11);

#line  1099 "cs.ATG" 
				ed.StartLocation = startLocation; ed.EndLocation = t.EndLocation; 
			}
		} else SynErr(175);
	}

	void EnumMemberDecl(
#line  1104 "cs.ATG" 
out FieldDeclaration f) {

#line  1106 "cs.ATG" 
		Expression expr = null;
		List<AttributeSection> attributes = new List<AttributeSection>();
		AttributeSection section = null;
		VariableDeclaration varDecl = null;
		
		while (la.kind == 18) {
			AttributeSection(
#line  1112 "cs.ATG" 
out section);

#line  1112 "cs.ATG" 
			attributes.Add(section); 
		}
		Identifier();

#line  1113 "cs.ATG" 
		f = new FieldDeclaration(attributes);
		varDecl         = new VariableDeclaration(t.val);
		f.Fields.Add(varDecl);
		f.StartLocation = t.Location;
		
		if (la.kind == 3) {
			lexer.NextToken();
			Expr(
#line  1118 "cs.ATG" 
out expr);

#line  1118 "cs.ATG" 
			varDecl.Initializer = expr; 
		}
	}

	void TypeWithRestriction(
#line  542 "cs.ATG" 
out TypeReference type, bool allowNullable, bool canBeUnbound) {

#line  544 "cs.ATG" 
		string name;
		int pointer = 0;
		type = null;
		
		if (StartOf(4)) {
			ClassType(
#line  549 "cs.ATG" 
out type, canBeUnbound);
		} else if (StartOf(5)) {
			SimpleType(
#line  550 "cs.ATG" 
out name);

#line  550 "cs.ATG" 
			type = new TypeReference(name); 
		} else if (la.kind == 123) {
			lexer.NextToken();
			Expect(6);

#line  551 "cs.ATG" 
			pointer = 1; type = new TypeReference("void"); 
		} else SynErr(176);

#line  552 "cs.ATG" 
		List<int> r = new List<int>(); 
		if (
#line  554 "cs.ATG" 
allowNullable && la.kind == Tokens.Question) {
			NullableQuestionMark(
#line  554 "cs.ATG" 
ref type);
		}
		while (
#line  556 "cs.ATG" 
IsPointerOrDims()) {

#line  556 "cs.ATG" 
			int i = 0; 
			if (la.kind == 6) {
				lexer.NextToken();

#line  557 "cs.ATG" 
				++pointer; 
			} else if (la.kind == 18) {
				lexer.NextToken();
				while (la.kind == 14) {
					lexer.NextToken();

#line  558 "cs.ATG" 
					++i; 
				}
				Expect(19);

#line  558 "cs.ATG" 
				r.Add(i); 
			} else SynErr(177);
		}

#line  561 "cs.ATG" 
		if (type != null) {
		type.RankSpecifier = r.ToArray();
		type.PointerNestingLevel = pointer;
		  }
		
	}

	void SimpleType(
#line  589 "cs.ATG" 
out string name) {

#line  590 "cs.ATG" 
		name = String.Empty; 
		if (StartOf(22)) {
			IntegralType(
#line  592 "cs.ATG" 
out name);
		} else if (la.kind == 75) {
			lexer.NextToken();

#line  593 "cs.ATG" 
			name = "float"; 
		} else if (la.kind == 66) {
			lexer.NextToken();

#line  594 "cs.ATG" 
			name = "double"; 
		} else if (la.kind == 62) {
			lexer.NextToken();

#line  595 "cs.ATG" 
			name = "decimal"; 
		} else if (la.kind == 52) {
			lexer.NextToken();

#line  596 "cs.ATG" 
			name = "bool"; 
		} else SynErr(178);
	}

	void NullableQuestionMark(
#line  2191 "cs.ATG" 
ref TypeReference typeRef) {

#line  2192 "cs.ATG" 
		List<TypeReference> typeArguments = new List<TypeReference>(1); 
		Expect(12);

#line  2196 "cs.ATG" 
		if (typeRef != null) typeArguments.Add(typeRef);
		typeRef = new TypeReference("System.Nullable", typeArguments);
		
	}

	void FixedParameter(
#line  626 "cs.ATG" 
out ParameterDeclarationExpression p) {

#line  628 "cs.ATG" 
		TypeReference type;
		ParameterModifiers mod = ParameterModifiers.In;
		Location start = t.Location;
		
		if (la.kind == 93 || la.kind == 100) {
			if (la.kind == 100) {
				lexer.NextToken();

#line  634 "cs.ATG" 
				mod = ParameterModifiers.Ref; 
			} else {
				lexer.NextToken();

#line  635 "cs.ATG" 
				mod = ParameterModifiers.Out; 
			}
		}
		Type(
#line  637 "cs.ATG" 
out type);
		Identifier();

#line  637 "cs.ATG" 
		p = new ParameterDeclarationExpression(type, t.val, mod); p.StartLocation = start; p.EndLocation = t.Location; 
	}

	void ParameterArray(
#line  640 "cs.ATG" 
out ParameterDeclarationExpression p) {

#line  641 "cs.ATG" 
		TypeReference type; 
		Expect(95);
		Type(
#line  643 "cs.ATG" 
out type);
		Identifier();

#line  643 "cs.ATG" 
		p = new ParameterDeclarationExpression(type, t.val, ParameterModifiers.Params); 
	}

	void AccessorModifiers(
#line  646 "cs.ATG" 
out ModifierList m) {

#line  647 "cs.ATG" 
		m = new ModifierList(); 
		if (la.kind == 96) {
			lexer.NextToken();

#line  649 "cs.ATG" 
			m.Add(Modifiers.Private, t.Location); 
		} else if (la.kind == 97) {
			lexer.NextToken();

#line  650 "cs.ATG" 
			m.Add(Modifiers.Protected, t.Location); 
			if (la.kind == 84) {
				lexer.NextToken();

#line  651 "cs.ATG" 
				m.Add(Modifiers.Internal, t.Location); 
			}
		} else if (la.kind == 84) {
			lexer.NextToken();

#line  652 "cs.ATG" 
			m.Add(Modifiers.Internal, t.Location); 
			if (la.kind == 97) {
				lexer.NextToken();

#line  653 "cs.ATG" 
				m.Add(Modifiers.Protected, t.Location); 
			}
		} else SynErr(179);
	}

	void Block(
#line  1237 "cs.ATG" 
out Statement stmt) {
		Expect(16);

#line  1239 "cs.ATG" 
		BlockStatement blockStmt = new BlockStatement();
		blockStmt.StartLocation = t.Location;
		compilationUnit.BlockStart(blockStmt);
		if (!ParseMethodBodies) lexer.SkipCurrentBlock(0);
		
		while (StartOf(23)) {
			Statement();
		}
		Expect(17);

#line  1246 "cs.ATG" 
		stmt = blockStmt;
		blockStmt.EndLocation = t.EndLocation;
		compilationUnit.BlockEnd();
		
	}

	void EventAccessorDecls(
#line  1175 "cs.ATG" 
out EventAddRegion addBlock, out EventRemoveRegion removeBlock) {

#line  1176 "cs.ATG" 
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		Statement stmt;
		addBlock = null;
		removeBlock = null;
		
		while (la.kind == 18) {
			AttributeSection(
#line  1183 "cs.ATG" 
out section);

#line  1183 "cs.ATG" 
			attributes.Add(section); 
		}
		if (la.kind == 130) {

#line  1185 "cs.ATG" 
			addBlock = new EventAddRegion(attributes); 
			AddAccessorDecl(
#line  1186 "cs.ATG" 
out stmt);

#line  1186 "cs.ATG" 
			attributes = new List<AttributeSection>(); addBlock.Block = (BlockStatement)stmt; 
			while (la.kind == 18) {
				AttributeSection(
#line  1187 "cs.ATG" 
out section);

#line  1187 "cs.ATG" 
				attributes.Add(section); 
			}
			RemoveAccessorDecl(
#line  1188 "cs.ATG" 
out stmt);

#line  1188 "cs.ATG" 
			removeBlock = new EventRemoveRegion(attributes); removeBlock.Block = (BlockStatement)stmt; 
		} else if (la.kind == 131) {
			RemoveAccessorDecl(
#line  1190 "cs.ATG" 
out stmt);

#line  1190 "cs.ATG" 
			removeBlock = new EventRemoveRegion(attributes); removeBlock.Block = (BlockStatement)stmt; attributes = new List<AttributeSection>(); 
			while (la.kind == 18) {
				AttributeSection(
#line  1191 "cs.ATG" 
out section);

#line  1191 "cs.ATG" 
				attributes.Add(section); 
			}
			AddAccessorDecl(
#line  1192 "cs.ATG" 
out stmt);

#line  1192 "cs.ATG" 
			addBlock = new EventAddRegion(attributes); addBlock.Block = (BlockStatement)stmt; 
		} else SynErr(180);
	}

	void ConstructorInitializer(
#line  1266 "cs.ATG" 
out ConstructorInitializer ci) {

#line  1267 "cs.ATG" 
		Expression expr; ci = new ConstructorInitializer(); 
		Expect(9);
		if (la.kind == 51) {
			lexer.NextToken();

#line  1271 "cs.ATG" 
			ci.ConstructorInitializerType = ConstructorInitializerType.Base; 
		} else if (la.kind == 111) {
			lexer.NextToken();

#line  1272 "cs.ATG" 
			ci.ConstructorInitializerType = ConstructorInitializerType.This; 
		} else SynErr(181);
		Expect(20);
		if (StartOf(24)) {
			Argument(
#line  1275 "cs.ATG" 
out expr);

#line  1275 "cs.ATG" 
			if (expr != null) { ci.Arguments.Add(expr); } 
			while (la.kind == 14) {
				lexer.NextToken();
				Argument(
#line  1275 "cs.ATG" 
out expr);

#line  1275 "cs.ATG" 
				if (expr != null) { ci.Arguments.Add(expr); } 
			}
		}
		Expect(21);
	}

	void OverloadableOperator(
#line  1287 "cs.ATG" 
out OverloadableOperatorType op) {

#line  1288 "cs.ATG" 
		op = OverloadableOperatorType.None; 
		switch (la.kind) {
		case 4: {
			lexer.NextToken();

#line  1290 "cs.ATG" 
			op = OverloadableOperatorType.Add; 
			break;
		}
		case 5: {
			lexer.NextToken();

#line  1291 "cs.ATG" 
			op = OverloadableOperatorType.Subtract; 
			break;
		}
		case 24: {
			lexer.NextToken();

#line  1293 "cs.ATG" 
			op = OverloadableOperatorType.Not; 
			break;
		}
		case 27: {
			lexer.NextToken();

#line  1294 "cs.ATG" 
			op = OverloadableOperatorType.BitNot; 
			break;
		}
		case 31: {
			lexer.NextToken();

#line  1296 "cs.ATG" 
			op = OverloadableOperatorType.Increment; 
			break;
		}
		case 32: {
			lexer.NextToken();

#line  1297 "cs.ATG" 
			op = OverloadableOperatorType.Decrement; 
			break;
		}
		case 113: {
			lexer.NextToken();

#line  1299 "cs.ATG" 
			op = OverloadableOperatorType.IsTrue; 
			break;
		}
		case 72: {
			lexer.NextToken();

#line  1300 "cs.ATG" 
			op = OverloadableOperatorType.IsFalse; 
			break;
		}
		case 6: {
			lexer.NextToken();

#line  1302 "cs.ATG" 
			op = OverloadableOperatorType.Multiply; 
			break;
		}
		case 7: {
			lexer.NextToken();

#line  1303 "cs.ATG" 
			op = OverloadableOperatorType.Divide; 
			break;
		}
		case 8: {
			lexer.NextToken();

#line  1304 "cs.ATG" 
			op = OverloadableOperatorType.Modulus; 
			break;
		}
		case 28: {
			lexer.NextToken();

#line  1306 "cs.ATG" 
			op = OverloadableOperatorType.BitwiseAnd; 
			break;
		}
		case 29: {
			lexer.NextToken();

#line  1307 "cs.ATG" 
			op = OverloadableOperatorType.BitwiseOr; 
			break;
		}
		case 30: {
			lexer.NextToken();

#line  1308 "cs.ATG" 
			op = OverloadableOperatorType.ExclusiveOr; 
			break;
		}
		case 37: {
			lexer.NextToken();

#line  1310 "cs.ATG" 
			op = OverloadableOperatorType.ShiftLeft; 
			break;
		}
		case 33: {
			lexer.NextToken();

#line  1311 "cs.ATG" 
			op = OverloadableOperatorType.Equality; 
			break;
		}
		case 34: {
			lexer.NextToken();

#line  1312 "cs.ATG" 
			op = OverloadableOperatorType.InEquality; 
			break;
		}
		case 23: {
			lexer.NextToken();

#line  1313 "cs.ATG" 
			op = OverloadableOperatorType.LessThan; 
			break;
		}
		case 35: {
			lexer.NextToken();

#line  1314 "cs.ATG" 
			op = OverloadableOperatorType.GreaterThanOrEqual; 
			break;
		}
		case 36: {
			lexer.NextToken();

#line  1315 "cs.ATG" 
			op = OverloadableOperatorType.LessThanOrEqual; 
			break;
		}
		case 22: {
			lexer.NextToken();

#line  1316 "cs.ATG" 
			op = OverloadableOperatorType.GreaterThan; 
			if (la.kind == 22) {
				lexer.NextToken();

#line  1316 "cs.ATG" 
				op = OverloadableOperatorType.ShiftRight; 
			}
			break;
		}
		default: SynErr(182); break;
		}
	}

	void VariableDeclarator(
#line  1230 "cs.ATG" 
List<VariableDeclaration> fieldDeclaration) {

#line  1231 "cs.ATG" 
		Expression expr = null; 
		Identifier();

#line  1233 "cs.ATG" 
		VariableDeclaration f = new VariableDeclaration(t.val); 
		if (la.kind == 3) {
			lexer.NextToken();
			VariableInitializer(
#line  1234 "cs.ATG" 
out expr);

#line  1234 "cs.ATG" 
			f.Initializer = expr; 
		}

#line  1234 "cs.ATG" 
		fieldDeclaration.Add(f); 
	}

	void AccessorDecls(
#line  1122 "cs.ATG" 
out PropertyGetRegion getBlock, out PropertySetRegion setBlock) {

#line  1124 "cs.ATG" 
		List<AttributeSection> attributes = new List<AttributeSection>(); 
		AttributeSection section;
		getBlock = null;
		setBlock = null; 
		ModifierList modifiers = null;
		
		while (la.kind == 18) {
			AttributeSection(
#line  1131 "cs.ATG" 
out section);

#line  1131 "cs.ATG" 
			attributes.Add(section); 
		}
		if (la.kind == 84 || la.kind == 96 || la.kind == 97) {
			AccessorModifiers(
#line  1132 "cs.ATG" 
out modifiers);
		}
		if (la.kind == 128) {
			GetAccessorDecl(
#line  1134 "cs.ATG" 
out getBlock, attributes);

#line  1135 "cs.ATG" 
			if (modifiers != null) {getBlock.Modifier = modifiers.Modifier; } 
			if (StartOf(25)) {

#line  1136 "cs.ATG" 
				attributes = new List<AttributeSection>(); modifiers = null; 
				while (la.kind == 18) {
					AttributeSection(
#line  1137 "cs.ATG" 
out section);

#line  1137 "cs.ATG" 
					attributes.Add(section); 
				}
				if (la.kind == 84 || la.kind == 96 || la.kind == 97) {
					AccessorModifiers(
#line  1138 "cs.ATG" 
out modifiers);
				}
				SetAccessorDecl(
#line  1139 "cs.ATG" 
out setBlock, attributes);

#line  1140 "cs.ATG" 
				if (modifiers != null) {setBlock.Modifier = modifiers.Modifier; } 
			}
		} else if (la.kind == 129) {
			SetAccessorDecl(
#line  1143 "cs.ATG" 
out setBlock, attributes);

#line  1144 "cs.ATG" 
			if (modifiers != null) {setBlock.Modifier = modifiers.Modifier; } 
			if (StartOf(26)) {

#line  1145 "cs.ATG" 
				attributes = new List<AttributeSection>(); modifiers = null; 
				while (la.kind == 18) {
					AttributeSection(
#line  1146 "cs.ATG" 
out section);

#line  1146 "cs.ATG" 
					attributes.Add(section); 
				}
				if (la.kind == 84 || la.kind == 96 || la.kind == 97) {
					AccessorModifiers(
#line  1147 "cs.ATG" 
out modifiers);
				}
				GetAccessorDecl(
#line  1148 "cs.ATG" 
out getBlock, attributes);

#line  1149 "cs.ATG" 
				if (modifiers != null) {getBlock.Modifier = modifiers.Modifier; } 
			}
		} else if (StartOf(17)) {
			Identifier();

#line  1151 "cs.ATG" 
			Error("get or set accessor declaration expected"); 
		} else SynErr(183);
	}

	void InterfaceAccessors(
#line  1196 "cs.ATG" 
out PropertyGetRegion getBlock, out PropertySetRegion setBlock) {

#line  1198 "cs.ATG" 
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		getBlock = null; setBlock = null;
		PropertyGetSetRegion lastBlock = null;
		
		while (la.kind == 18) {
			AttributeSection(
#line  1204 "cs.ATG" 
out section);

#line  1204 "cs.ATG" 
			attributes.Add(section); 
		}

#line  1205 "cs.ATG" 
		Location startLocation = la.Location; 
		if (la.kind == 128) {
			lexer.NextToken();

#line  1207 "cs.ATG" 
			getBlock = new PropertyGetRegion(null, attributes); 
		} else if (la.kind == 129) {
			lexer.NextToken();

#line  1208 "cs.ATG" 
			setBlock = new PropertySetRegion(null, attributes); 
		} else SynErr(184);
		Expect(11);

#line  1211 "cs.ATG" 
		if (getBlock != null) { getBlock.StartLocation = startLocation; getBlock.EndLocation = t.EndLocation; }
		if (setBlock != null) { setBlock.StartLocation = startLocation; setBlock.EndLocation = t.EndLocation; }
		attributes = new List<AttributeSection>(); 
		if (la.kind == 18 || la.kind == 128 || la.kind == 129) {
			while (la.kind == 18) {
				AttributeSection(
#line  1215 "cs.ATG" 
out section);

#line  1215 "cs.ATG" 
				attributes.Add(section); 
			}

#line  1216 "cs.ATG" 
			startLocation = la.Location; 
			if (la.kind == 128) {
				lexer.NextToken();

#line  1218 "cs.ATG" 
				if (getBlock != null) Error("get already declared");
				                 else { getBlock = new PropertyGetRegion(null, attributes); lastBlock = getBlock; }
				              
			} else if (la.kind == 129) {
				lexer.NextToken();

#line  1221 "cs.ATG" 
				if (setBlock != null) Error("set already declared");
				                 else { setBlock = new PropertySetRegion(null, attributes); lastBlock = setBlock; }
				              
			} else SynErr(185);
			Expect(11);

#line  1226 "cs.ATG" 
			if (lastBlock != null) { lastBlock.StartLocation = startLocation; lastBlock.EndLocation = t.EndLocation; } 
		}
	}

	void GetAccessorDecl(
#line  1155 "cs.ATG" 
out PropertyGetRegion getBlock, List<AttributeSection> attributes) {

#line  1156 "cs.ATG" 
		Statement stmt = null; 
		Expect(128);

#line  1159 "cs.ATG" 
		Location startLocation = t.Location; 
		if (la.kind == 16) {
			Block(
#line  1160 "cs.ATG" 
out stmt);
		} else if (la.kind == 11) {
			lexer.NextToken();
		} else SynErr(186);

#line  1161 "cs.ATG" 
		getBlock = new PropertyGetRegion((BlockStatement)stmt, attributes); 

#line  1162 "cs.ATG" 
		getBlock.StartLocation = startLocation; getBlock.EndLocation = t.EndLocation; 
	}

	void SetAccessorDecl(
#line  1165 "cs.ATG" 
out PropertySetRegion setBlock, List<AttributeSection> attributes) {

#line  1166 "cs.ATG" 
		Statement stmt = null; 
		Expect(129);

#line  1169 "cs.ATG" 
		Location startLocation = t.Location; 
		if (la.kind == 16) {
			Block(
#line  1170 "cs.ATG" 
out stmt);
		} else if (la.kind == 11) {
			lexer.NextToken();
		} else SynErr(187);

#line  1171 "cs.ATG" 
		setBlock = new PropertySetRegion((BlockStatement)stmt, attributes); 

#line  1172 "cs.ATG" 
		setBlock.StartLocation = startLocation; setBlock.EndLocation = t.EndLocation; 
	}

	void AddAccessorDecl(
#line  1252 "cs.ATG" 
out Statement stmt) {

#line  1253 "cs.ATG" 
		stmt = null;
		Expect(130);
		Block(
#line  1256 "cs.ATG" 
out stmt);
	}

	void RemoveAccessorDecl(
#line  1259 "cs.ATG" 
out Statement stmt) {

#line  1260 "cs.ATG" 
		stmt = null;
		Expect(131);
		Block(
#line  1263 "cs.ATG" 
out stmt);
	}

	void VariableInitializer(
#line  1279 "cs.ATG" 
out Expression initializerExpression) {

#line  1280 "cs.ATG" 
		TypeReference type = null; Expression expr = null; initializerExpression = null; 
		if (StartOf(6)) {
			Expr(
#line  1282 "cs.ATG" 
out initializerExpression);
		} else if (la.kind == 16) {
			CollectionInitializer(
#line  1283 "cs.ATG" 
out initializerExpression);
		} else if (la.kind == 106) {
			lexer.NextToken();
			Type(
#line  1284 "cs.ATG" 
out type);
			Expect(18);
			Expr(
#line  1284 "cs.ATG" 
out expr);
			Expect(19);

#line  1284 "cs.ATG" 
			initializerExpression = new StackAllocExpression(type, expr); 
		} else SynErr(188);
	}

	void Statement() {

#line  1423 "cs.ATG" 
		TypeReference type;
		Expression expr;
		Statement stmt = null;
		Location startPos = la.Location;
		
		if (
#line  1431 "cs.ATG" 
IsLabel()) {
			Identifier();

#line  1431 "cs.ATG" 
			compilationUnit.AddChild(new LabelStatement(t.val)); 
			Expect(9);
			Statement();
		} else if (la.kind == 60) {
			lexer.NextToken();
			Type(
#line  1434 "cs.ATG" 
out type);

#line  1434 "cs.ATG" 
			LocalVariableDeclaration var = new LocalVariableDeclaration(type, Modifiers.Const); string ident = null; var.StartLocation = t.Location; 
			Identifier();

#line  1435 "cs.ATG" 
			ident = t.val; 
			Expect(3);
			Expr(
#line  1436 "cs.ATG" 
out expr);

#line  1436 "cs.ATG" 
			var.Variables.Add(new VariableDeclaration(ident, expr)); 
			while (la.kind == 14) {
				lexer.NextToken();
				Identifier();

#line  1437 "cs.ATG" 
				ident = t.val; 
				Expect(3);
				Expr(
#line  1437 "cs.ATG" 
out expr);

#line  1437 "cs.ATG" 
				var.Variables.Add(new VariableDeclaration(ident, expr)); 
			}
			Expect(11);

#line  1438 "cs.ATG" 
			compilationUnit.AddChild(var); 
		} else if (
#line  1440 "cs.ATG" 
IsLocalVarDecl()) {
			LocalVariableDecl(
#line  1440 "cs.ATG" 
out stmt);
			Expect(11);

#line  1440 "cs.ATG" 
			compilationUnit.AddChild(stmt); 
		} else if (StartOf(27)) {
			EmbeddedStatement(
#line  1441 "cs.ATG" 
out stmt);

#line  1441 "cs.ATG" 
			compilationUnit.AddChild(stmt); 
		} else SynErr(189);

#line  1447 "cs.ATG" 
		if (stmt != null) {
		stmt.StartLocation = startPos;
		stmt.EndLocation = t.EndLocation;
		}
		
	}

	void Argument(
#line  1319 "cs.ATG" 
out Expression argumentexpr) {

#line  1321 "cs.ATG" 
		Expression expr;
		FieldDirection fd = FieldDirection.None;
		
		if (la.kind == 93 || la.kind == 100) {
			if (la.kind == 100) {
				lexer.NextToken();

#line  1326 "cs.ATG" 
				fd = FieldDirection.Ref; 
			} else {
				lexer.NextToken();

#line  1327 "cs.ATG" 
				fd = FieldDirection.Out; 
			}
		}
		Expr(
#line  1329 "cs.ATG" 
out expr);

#line  1329 "cs.ATG" 
		argumentexpr = fd != FieldDirection.None ? argumentexpr = new DirectionExpression(fd, expr) : expr; 
	}

	void CollectionInitializer(
#line  1349 "cs.ATG" 
out Expression outExpr) {

#line  1351 "cs.ATG" 
		Expression expr = null;
		CollectionInitializerExpression initializer = new CollectionInitializerExpression();
		
		Expect(16);

#line  1355 "cs.ATG" 
		initializer.StartLocation = t.Location; 
		if (StartOf(28)) {
			VariableInitializer(
#line  1356 "cs.ATG" 
out expr);

#line  1357 "cs.ATG" 
			if (expr != null) { initializer.CreateExpressions.Add(expr); } 
			while (
#line  1358 "cs.ATG" 
NotFinalComma()) {
				Expect(14);
				VariableInitializer(
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
		initializer.EndLocation = t.Location; outExpr = initializer; 
	}

	void AssignmentOperator(
#line  1332 "cs.ATG" 
out AssignmentOperatorType op) {

#line  1333 "cs.ATG" 
		op = AssignmentOperatorType.None; 
		if (la.kind == 3) {
			lexer.NextToken();

#line  1335 "cs.ATG" 
			op = AssignmentOperatorType.Assign; 
		} else if (la.kind == 38) {
			lexer.NextToken();

#line  1336 "cs.ATG" 
			op = AssignmentOperatorType.Add; 
		} else if (la.kind == 39) {
			lexer.NextToken();

#line  1337 "cs.ATG" 
			op = AssignmentOperatorType.Subtract; 
		} else if (la.kind == 40) {
			lexer.NextToken();

#line  1338 "cs.ATG" 
			op = AssignmentOperatorType.Multiply; 
		} else if (la.kind == 41) {
			lexer.NextToken();

#line  1339 "cs.ATG" 
			op = AssignmentOperatorType.Divide; 
		} else if (la.kind == 42) {
			lexer.NextToken();

#line  1340 "cs.ATG" 
			op = AssignmentOperatorType.Modulus; 
		} else if (la.kind == 43) {
			lexer.NextToken();

#line  1341 "cs.ATG" 
			op = AssignmentOperatorType.BitwiseAnd; 
		} else if (la.kind == 44) {
			lexer.NextToken();

#line  1342 "cs.ATG" 
			op = AssignmentOperatorType.BitwiseOr; 
		} else if (la.kind == 45) {
			lexer.NextToken();

#line  1343 "cs.ATG" 
			op = AssignmentOperatorType.ExclusiveOr; 
		} else if (la.kind == 46) {
			lexer.NextToken();

#line  1344 "cs.ATG" 
			op = AssignmentOperatorType.ShiftLeft; 
		} else if (
#line  1345 "cs.ATG" 
la.kind == Tokens.GreaterThan && Peek(1).kind == Tokens.GreaterEqual) {
			Expect(22);
			Expect(35);

#line  1346 "cs.ATG" 
			op = AssignmentOperatorType.ShiftRight; 
		} else SynErr(190);
	}

	void CollectionOrObjectInitializer(
#line  1367 "cs.ATG" 
out Expression outExpr) {

#line  1369 "cs.ATG" 
		Expression expr = null;
		CollectionInitializerExpression initializer = new CollectionInitializerExpression();
		
		Expect(16);

#line  1373 "cs.ATG" 
		initializer.StartLocation = t.Location; 
		if (StartOf(28)) {
			ObjectPropertyInitializerOrVariableInitializer(
#line  1374 "cs.ATG" 
out expr);

#line  1375 "cs.ATG" 
			if (expr != null) { initializer.CreateExpressions.Add(expr); } 
			while (
#line  1376 "cs.ATG" 
NotFinalComma()) {
				Expect(14);
				ObjectPropertyInitializerOrVariableInitializer(
#line  1377 "cs.ATG" 
out expr);

#line  1378 "cs.ATG" 
				if (expr != null) { initializer.CreateExpressions.Add(expr); } 
			}
			if (la.kind == 14) {
				lexer.NextToken();
			}
		}
		Expect(17);

#line  1382 "cs.ATG" 
		initializer.EndLocation = t.Location; outExpr = initializer; 
	}

	void ObjectPropertyInitializerOrVariableInitializer(
#line  1385 "cs.ATG" 
out Expression expr) {

#line  1386 "cs.ATG" 
		expr = null; 
		if (
#line  1388 "cs.ATG" 
IdentAndAsgn()) {
			Identifier();

#line  1390 "cs.ATG" 
			NamedArgumentExpression nae = new NamedArgumentExpression(t.val, null);
			nae.StartLocation = t.Location;
			Expression r = null; 
			Expect(3);
			if (la.kind == 16) {
				CollectionOrObjectInitializer(
#line  1394 "cs.ATG" 
out r);
			} else if (StartOf(28)) {
				VariableInitializer(
#line  1395 "cs.ATG" 
out r);
			} else SynErr(191);

#line  1396 "cs.ATG" 
			nae.Expression = r; nae.EndLocation = t.EndLocation; expr = nae; 
		} else if (StartOf(28)) {
			VariableInitializer(
#line  1398 "cs.ATG" 
out expr);
		} else SynErr(192);
	}

	void LocalVariableDecl(
#line  1402 "cs.ATG" 
out Statement stmt) {

#line  1404 "cs.ATG" 
		TypeReference type;
		VariableDeclaration      var = null;
		LocalVariableDeclaration localVariableDeclaration; 
		
		Type(
#line  1409 "cs.ATG" 
out type);

#line  1409 "cs.ATG" 
		localVariableDeclaration = new LocalVariableDeclaration(type); localVariableDeclaration.StartLocation = t.Location; 
		LocalVariableDeclarator(
#line  1410 "cs.ATG" 
out var);

#line  1410 "cs.ATG" 
		localVariableDeclaration.Variables.Add(var); 
		while (la.kind == 14) {
			lexer.NextToken();
			LocalVariableDeclarator(
#line  1411 "cs.ATG" 
out var);

#line  1411 "cs.ATG" 
			localVariableDeclaration.Variables.Add(var); 
		}

#line  1412 "cs.ATG" 
		stmt = localVariableDeclaration; 
	}

	void LocalVariableDeclarator(
#line  1415 "cs.ATG" 
out VariableDeclaration var) {

#line  1416 "cs.ATG" 
		Expression expr = null; 
		Identifier();

#line  1418 "cs.ATG" 
		var = new VariableDeclaration(t.val); 
		if (la.kind == 3) {
			lexer.NextToken();
			VariableInitializer(
#line  1418 "cs.ATG" 
out expr);

#line  1418 "cs.ATG" 
			var.Initializer = expr; 
		}
	}

	void EmbeddedStatement(
#line  1454 "cs.ATG" 
out Statement statement) {

#line  1456 "cs.ATG" 
		TypeReference type = null;
		Expression expr = null;
		Statement embeddedStatement = null;
		statement = null;
		
		if (la.kind == 16) {
			Block(
#line  1462 "cs.ATG" 
out statement);
		} else if (la.kind == 11) {
			lexer.NextToken();

#line  1464 "cs.ATG" 
			statement = new EmptyStatement(); 
		} else if (
#line  1466 "cs.ATG" 
UnCheckedAndLBrace()) {

#line  1466 "cs.ATG" 
			Statement block; bool isChecked = true; 
			if (la.kind == 58) {
				lexer.NextToken();
			} else if (la.kind == 118) {
				lexer.NextToken();

#line  1467 "cs.ATG" 
				isChecked = false;
			} else SynErr(193);
			Block(
#line  1468 "cs.ATG" 
out block);

#line  1468 "cs.ATG" 
			statement = isChecked ? (Statement)new CheckedStatement(block) : (Statement)new UncheckedStatement(block); 
		} else if (la.kind == 79) {
			lexer.NextToken();

#line  1470 "cs.ATG" 
			Statement elseStatement = null; 
			Expect(20);
			Expr(
#line  1471 "cs.ATG" 
out expr);
			Expect(21);
			EmbeddedStatement(
#line  1472 "cs.ATG" 
out embeddedStatement);
			if (la.kind == 67) {
				lexer.NextToken();
				EmbeddedStatement(
#line  1473 "cs.ATG" 
out elseStatement);
			}

#line  1474 "cs.ATG" 
			statement = elseStatement != null ? new IfElseStatement(expr, embeddedStatement, elseStatement) : new IfElseStatement(expr, embeddedStatement); 

#line  1475 "cs.ATG" 
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

#line  1483 "cs.ATG" 
			List<SwitchSection> switchSections = new List<SwitchSection>(); 
			Expect(20);
			Expr(
#line  1484 "cs.ATG" 
out expr);
			Expect(21);
			Expect(16);
			SwitchSections(
#line  1485 "cs.ATG" 
switchSections);
			Expect(17);

#line  1486 "cs.ATG" 
			statement = new SwitchStatement(expr, switchSections); 
		} else if (la.kind == 125) {
			lexer.NextToken();
			Expect(20);
			Expr(
#line  1488 "cs.ATG" 
out expr);
			Expect(21);
			EmbeddedStatement(
#line  1490 "cs.ATG" 
out embeddedStatement);

#line  1490 "cs.ATG" 
			statement = new DoLoopStatement(expr, embeddedStatement, ConditionType.While, ConditionPosition.Start);
		} else if (la.kind == 65) {
			lexer.NextToken();
			EmbeddedStatement(
#line  1491 "cs.ATG" 
out embeddedStatement);
			Expect(125);
			Expect(20);
			Expr(
#line  1492 "cs.ATG" 
out expr);
			Expect(21);
			Expect(11);

#line  1492 "cs.ATG" 
			statement = new DoLoopStatement(expr, embeddedStatement, ConditionType.While, ConditionPosition.End); 
		} else if (la.kind == 76) {
			lexer.NextToken();

#line  1493 "cs.ATG" 
			List<Statement> initializer = null; List<Statement> iterator = null; 
			Expect(20);
			if (StartOf(6)) {
				ForInitializer(
#line  1494 "cs.ATG" 
out initializer);
			}
			Expect(11);
			if (StartOf(6)) {
				Expr(
#line  1495 "cs.ATG" 
out expr);
			}
			Expect(11);
			if (StartOf(6)) {
				ForIterator(
#line  1496 "cs.ATG" 
out iterator);
			}
			Expect(21);
			EmbeddedStatement(
#line  1497 "cs.ATG" 
out embeddedStatement);

#line  1497 "cs.ATG" 
			statement = new ForStatement(initializer, expr, iterator, embeddedStatement); 
		} else if (la.kind == 77) {
			lexer.NextToken();
			Expect(20);
			Type(
#line  1498 "cs.ATG" 
out type);
			Identifier();

#line  1498 "cs.ATG" 
			string varName = t.val; Location start = t.Location;
			Expect(81);
			Expr(
#line  1499 "cs.ATG" 
out expr);
			Expect(21);
			EmbeddedStatement(
#line  1500 "cs.ATG" 
out embeddedStatement);

#line  1500 "cs.ATG" 
			statement = new ForeachStatement(type, varName , expr, embeddedStatement); 
			statement.EndLocation = t.EndLocation;
			
		} else if (la.kind == 53) {
			lexer.NextToken();
			Expect(11);

#line  1504 "cs.ATG" 
			statement = new BreakStatement(); 
		} else if (la.kind == 61) {
			lexer.NextToken();
			Expect(11);

#line  1505 "cs.ATG" 
			statement = new ContinueStatement(); 
		} else if (la.kind == 78) {
			GotoStatement(
#line  1506 "cs.ATG" 
out statement);
		} else if (
#line  1507 "cs.ATG" 
IsYieldStatement()) {
			Expect(132);
			if (la.kind == 101) {
				lexer.NextToken();
				Expr(
#line  1507 "cs.ATG" 
out expr);

#line  1507 "cs.ATG" 
				statement = new YieldStatement(new ReturnStatement(expr)); 
			} else if (la.kind == 53) {
				lexer.NextToken();

#line  1508 "cs.ATG" 
				statement = new YieldStatement(new BreakStatement()); 
			} else SynErr(194);
			Expect(11);
		} else if (la.kind == 101) {
			lexer.NextToken();
			if (StartOf(6)) {
				Expr(
#line  1509 "cs.ATG" 
out expr);
			}
			Expect(11);

#line  1509 "cs.ATG" 
			statement = new ReturnStatement(expr); 
		} else if (la.kind == 112) {
			lexer.NextToken();
			if (StartOf(6)) {
				Expr(
#line  1510 "cs.ATG" 
out expr);
			}
			Expect(11);

#line  1510 "cs.ATG" 
			statement = new ThrowStatement(expr); 
		} else if (StartOf(6)) {
			StatementExpr(
#line  1513 "cs.ATG" 
out statement);
			Expect(11);
		} else if (la.kind == 114) {
			TryStatement(
#line  1515 "cs.ATG" 
out statement);
		} else if (la.kind == 86) {
			lexer.NextToken();
			Expect(20);
			Expr(
#line  1517 "cs.ATG" 
out expr);
			Expect(21);
			EmbeddedStatement(
#line  1518 "cs.ATG" 
out embeddedStatement);

#line  1518 "cs.ATG" 
			statement = new LockStatement(expr, embeddedStatement); 
		} else if (la.kind == 121) {

#line  1520 "cs.ATG" 
			Statement resourceAcquisitionStmt = null; 
			lexer.NextToken();
			Expect(20);
			ResourceAcquisition(
#line  1522 "cs.ATG" 
out resourceAcquisitionStmt);
			Expect(21);
			EmbeddedStatement(
#line  1523 "cs.ATG" 
out embeddedStatement);

#line  1523 "cs.ATG" 
			statement = new UsingStatement(resourceAcquisitionStmt, embeddedStatement); 
		} else if (la.kind == 119) {
			lexer.NextToken();
			Block(
#line  1525 "cs.ATG" 
out embeddedStatement);

#line  1525 "cs.ATG" 
			statement = new UnsafeStatement(embeddedStatement); 
		} else if (la.kind == 74) {
			lexer.NextToken();
			Expect(20);
			Type(
#line  1528 "cs.ATG" 
out type);

#line  1528 "cs.ATG" 
			if (type.PointerNestingLevel == 0) Error("can only fix pointer types");
			List<VariableDeclaration> pointerDeclarators = new List<VariableDeclaration>(1);
			
			Identifier();

#line  1531 "cs.ATG" 
			string identifier = t.val; 
			Expect(3);
			Expr(
#line  1532 "cs.ATG" 
out expr);

#line  1532 "cs.ATG" 
			pointerDeclarators.Add(new VariableDeclaration(identifier, expr)); 
			while (la.kind == 14) {
				lexer.NextToken();
				Identifier();

#line  1534 "cs.ATG" 
				identifier = t.val; 
				Expect(3);
				Expr(
#line  1535 "cs.ATG" 
out expr);

#line  1535 "cs.ATG" 
				pointerDeclarators.Add(new VariableDeclaration(identifier, expr)); 
			}
			Expect(21);
			EmbeddedStatement(
#line  1537 "cs.ATG" 
out embeddedStatement);

#line  1537 "cs.ATG" 
			statement = new FixedStatement(type, pointerDeclarators, embeddedStatement); 
		} else SynErr(195);
	}

	void SwitchSections(
#line  1559 "cs.ATG" 
List<SwitchSection> switchSections) {

#line  1561 "cs.ATG" 
		SwitchSection switchSection = new SwitchSection();
		CaseLabel label;
		
		SwitchLabel(
#line  1565 "cs.ATG" 
out label);

#line  1565 "cs.ATG" 
		if (label != null) { switchSection.SwitchLabels.Add(label); } 

#line  1566 "cs.ATG" 
		compilationUnit.BlockStart(switchSection); 
		while (StartOf(29)) {
			if (la.kind == 55 || la.kind == 63) {
				SwitchLabel(
#line  1568 "cs.ATG" 
out label);

#line  1569 "cs.ATG" 
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

#line  1581 "cs.ATG" 
		compilationUnit.BlockEnd(); switchSections.Add(switchSection); 
	}

	void ForInitializer(
#line  1540 "cs.ATG" 
out List<Statement> initializer) {

#line  1542 "cs.ATG" 
		Statement stmt; 
		initializer = new List<Statement>();
		
		if (
#line  1546 "cs.ATG" 
IsLocalVarDecl()) {
			LocalVariableDecl(
#line  1546 "cs.ATG" 
out stmt);

#line  1546 "cs.ATG" 
			initializer.Add(stmt);
		} else if (StartOf(6)) {
			StatementExpr(
#line  1547 "cs.ATG" 
out stmt);

#line  1547 "cs.ATG" 
			initializer.Add(stmt);
			while (la.kind == 14) {
				lexer.NextToken();
				StatementExpr(
#line  1547 "cs.ATG" 
out stmt);

#line  1547 "cs.ATG" 
				initializer.Add(stmt);
			}
		} else SynErr(196);
	}

	void ForIterator(
#line  1550 "cs.ATG" 
out List<Statement> iterator) {

#line  1552 "cs.ATG" 
		Statement stmt; 
		iterator = new List<Statement>();
		
		StatementExpr(
#line  1556 "cs.ATG" 
out stmt);

#line  1556 "cs.ATG" 
		iterator.Add(stmt);
		while (la.kind == 14) {
			lexer.NextToken();
			StatementExpr(
#line  1556 "cs.ATG" 
out stmt);

#line  1556 "cs.ATG" 
			iterator.Add(stmt); 
		}
	}

	void GotoStatement(
#line  1634 "cs.ATG" 
out Statement stmt) {

#line  1635 "cs.ATG" 
		Expression expr; stmt = null; 
		Expect(78);
		if (StartOf(17)) {
			Identifier();

#line  1639 "cs.ATG" 
			stmt = new GotoStatement(t.val); 
			Expect(11);
		} else if (la.kind == 55) {
			lexer.NextToken();
			Expr(
#line  1640 "cs.ATG" 
out expr);
			Expect(11);

#line  1640 "cs.ATG" 
			stmt = new GotoCaseStatement(expr); 
		} else if (la.kind == 63) {
			lexer.NextToken();
			Expect(11);

#line  1641 "cs.ATG" 
			stmt = new GotoCaseStatement(null); 
		} else SynErr(197);
	}

	void StatementExpr(
#line  1661 "cs.ATG" 
out Statement stmt) {

#line  1662 "cs.ATG" 
		Expression expr; 
		Expr(
#line  1664 "cs.ATG" 
out expr);

#line  1667 "cs.ATG" 
		stmt = new ExpressionStatement(expr); 
	}

	void TryStatement(
#line  1591 "cs.ATG" 
out Statement tryStatement) {

#line  1593 "cs.ATG" 
		Statement blockStmt = null, finallyStmt = null;
		List<CatchClause> catchClauses = null;
		
		Expect(114);
		Block(
#line  1597 "cs.ATG" 
out blockStmt);
		if (la.kind == 56) {
			CatchClauses(
#line  1599 "cs.ATG" 
out catchClauses);
			if (la.kind == 73) {
				lexer.NextToken();
				Block(
#line  1599 "cs.ATG" 
out finallyStmt);
			}
		} else if (la.kind == 73) {
			lexer.NextToken();
			Block(
#line  1600 "cs.ATG" 
out finallyStmt);
		} else SynErr(198);

#line  1603 "cs.ATG" 
		tryStatement = new TryCatchStatement(blockStmt, catchClauses, finallyStmt);
			
	}

	void ResourceAcquisition(
#line  1645 "cs.ATG" 
out Statement stmt) {

#line  1647 "cs.ATG" 
		stmt = null;
		Expression expr;
		
		if (
#line  1652 "cs.ATG" 
IsLocalVarDecl()) {
			LocalVariableDecl(
#line  1652 "cs.ATG" 
out stmt);
		} else if (StartOf(6)) {
			Expr(
#line  1653 "cs.ATG" 
out expr);

#line  1657 "cs.ATG" 
			stmt = new ExpressionStatement(expr); 
		} else SynErr(199);
	}

	void SwitchLabel(
#line  1584 "cs.ATG" 
out CaseLabel label) {

#line  1585 "cs.ATG" 
		Expression expr = null; label = null; 
		if (la.kind == 55) {
			lexer.NextToken();
			Expr(
#line  1587 "cs.ATG" 
out expr);
			Expect(9);

#line  1587 "cs.ATG" 
			label =  new CaseLabel(expr); 
		} else if (la.kind == 63) {
			lexer.NextToken();
			Expect(9);

#line  1588 "cs.ATG" 
			label =  new CaseLabel(); 
		} else SynErr(200);
	}

	void CatchClauses(
#line  1608 "cs.ATG" 
out List<CatchClause> catchClauses) {

#line  1610 "cs.ATG" 
		catchClauses = new List<CatchClause>();
		
		Expect(56);

#line  1613 "cs.ATG" 
		string identifier;
		Statement stmt;
		TypeReference typeRef;
		
		if (la.kind == 16) {
			Block(
#line  1619 "cs.ATG" 
out stmt);

#line  1619 "cs.ATG" 
			catchClauses.Add(new CatchClause(stmt)); 
		} else if (la.kind == 20) {
			lexer.NextToken();
			ClassType(
#line  1621 "cs.ATG" 
out typeRef, false);

#line  1621 "cs.ATG" 
			identifier = null; 
			if (StartOf(17)) {
				Identifier();

#line  1622 "cs.ATG" 
				identifier = t.val; 
			}
			Expect(21);
			Block(
#line  1623 "cs.ATG" 
out stmt);

#line  1624 "cs.ATG" 
			catchClauses.Add(new CatchClause(typeRef, identifier, stmt)); 
			while (
#line  1625 "cs.ATG" 
IsTypedCatch()) {
				Expect(56);
				Expect(20);
				ClassType(
#line  1625 "cs.ATG" 
out typeRef, false);

#line  1625 "cs.ATG" 
				identifier = null; 
				if (StartOf(17)) {
					Identifier();

#line  1626 "cs.ATG" 
					identifier = t.val; 
				}
				Expect(21);
				Block(
#line  1627 "cs.ATG" 
out stmt);

#line  1628 "cs.ATG" 
				catchClauses.Add(new CatchClause(typeRef, identifier, stmt)); 
			}
			if (la.kind == 56) {
				lexer.NextToken();
				Block(
#line  1630 "cs.ATG" 
out stmt);

#line  1630 "cs.ATG" 
				catchClauses.Add(new CatchClause(stmt)); 
			}
		} else SynErr(201);
	}

	void UnaryExpr(
#line  1688 "cs.ATG" 
out Expression uExpr) {

#line  1690 "cs.ATG" 
		TypeReference type = null;
		Expression expr = null;
		ArrayList expressions = new ArrayList();
		uExpr = null;
		
		while (StartOf(30) || 
#line  1712 "cs.ATG" 
IsTypeCast()) {
			if (la.kind == 4) {
				lexer.NextToken();

#line  1699 "cs.ATG" 
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.Plus)); 
			} else if (la.kind == 5) {
				lexer.NextToken();

#line  1700 "cs.ATG" 
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.Minus)); 
			} else if (la.kind == 24) {
				lexer.NextToken();

#line  1701 "cs.ATG" 
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.Not)); 
			} else if (la.kind == 27) {
				lexer.NextToken();

#line  1702 "cs.ATG" 
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.BitNot)); 
			} else if (la.kind == 6) {
				lexer.NextToken();

#line  1703 "cs.ATG" 
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.Star)); 
			} else if (la.kind == 31) {
				lexer.NextToken();

#line  1704 "cs.ATG" 
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.Increment)); 
			} else if (la.kind == 32) {
				lexer.NextToken();

#line  1705 "cs.ATG" 
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.Decrement)); 
			} else if (la.kind == 28) {
				lexer.NextToken();

#line  1706 "cs.ATG" 
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.BitWiseAnd)); 
			} else {
				Expect(20);
				Type(
#line  1712 "cs.ATG" 
out type);
				Expect(21);

#line  1712 "cs.ATG" 
				expressions.Add(new CastExpression(type)); 
			}
		}
		if (
#line  1717 "cs.ATG" 
LastExpressionIsUnaryMinus(expressions) && IsMostNegativeIntegerWithoutTypeSuffix()) {
			Expect(2);

#line  1720 "cs.ATG" 
			expressions.RemoveAt(expressions.Count - 1);
			if (t.literalValue is uint) {
				expr = new PrimitiveExpression(int.MinValue, int.MinValue.ToString());
			} else if (t.literalValue is ulong) {
				expr = new PrimitiveExpression(long.MinValue, long.MinValue.ToString());
			} else {
				throw new Exception("t.literalValue must be uint or ulong");
			}
			
		} else if (StartOf(31)) {
			PrimaryExpr(
#line  1729 "cs.ATG" 
out expr);
		} else SynErr(202);

#line  1731 "cs.ATG" 
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
#line  2031 "cs.ATG" 
ref Expression outExpr) {

#line  2032 "cs.ATG" 
		Expression expr;   
		ConditionalAndExpr(
#line  2034 "cs.ATG" 
ref outExpr);
		while (la.kind == 26) {
			lexer.NextToken();
			UnaryExpr(
#line  2034 "cs.ATG" 
out expr);
			ConditionalAndExpr(
#line  2034 "cs.ATG" 
ref expr);

#line  2034 "cs.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.LogicalOr, expr);  
		}
	}

	void PrimaryExpr(
#line  1748 "cs.ATG" 
out Expression pexpr) {

#line  1750 "cs.ATG" 
		TypeReference type = null;
		Expression expr;
		pexpr = null;
		
		if (la.kind == 113) {
			lexer.NextToken();

#line  1756 "cs.ATG" 
			pexpr = new PrimitiveExpression(true, "true");  
		} else if (la.kind == 72) {
			lexer.NextToken();

#line  1757 "cs.ATG" 
			pexpr = new PrimitiveExpression(false, "false"); 
		} else if (la.kind == 90) {
			lexer.NextToken();

#line  1758 "cs.ATG" 
			pexpr = new PrimitiveExpression(null, "null");  
		} else if (la.kind == 2) {
			lexer.NextToken();

#line  1759 "cs.ATG" 
			pexpr = new PrimitiveExpression(t.literalValue, t.val);  
		} else if (
#line  1760 "cs.ATG" 
StartOfQueryExpression()) {
			QueryExpression(
#line  1761 "cs.ATG" 
out pexpr);
		} else if (
#line  1762 "cs.ATG" 
IdentAndDoubleColon()) {
			Identifier();

#line  1763 "cs.ATG" 
			type = new TypeReference(t.val); 
			Expect(10);

#line  1764 "cs.ATG" 
			pexpr = new TypeReferenceExpression(type); 
			Identifier();

#line  1765 "cs.ATG" 
			if (type.Type == "global") { type.IsGlobal = true; type.Type = (t.val ?? "?"); } else type.Type += "." + (t.val ?? "?"); 
		} else if (StartOf(17)) {
			Identifier();

#line  1768 "cs.ATG" 
			pexpr = new IdentifierExpression(t.val); 

#line  1769 "cs.ATG" 
			pexpr.StartLocation = t.Location; pexpr.EndLocation = t.EndLocation; 
			if (la.kind == 48 || 
#line  1772 "cs.ATG" 
IsGenericInSimpleNameOrMemberAccess()) {
				if (la.kind == 48) {
					ShortedLambdaExpression(
#line  1771 "cs.ATG" 
(IdentifierExpression)pexpr, out pexpr);
				} else {

#line  1773 "cs.ATG" 
					List<TypeReference> typeList; 
					TypeArgumentList(
#line  1774 "cs.ATG" 
out typeList, false);

#line  1775 "cs.ATG" 
					((IdentifierExpression)pexpr).TypeArguments = typeList; 
				}
			}
		} else if (
#line  1777 "cs.ATG" 
IsLambdaExpression()) {
			LambdaExpression(
#line  1778 "cs.ATG" 
out pexpr);
		} else if (la.kind == 20) {
			lexer.NextToken();
			Expr(
#line  1780 "cs.ATG" 
out expr);
			Expect(21);

#line  1780 "cs.ATG" 
			pexpr = new ParenthesizedExpression(expr); 
		} else if (StartOf(32)) {

#line  1782 "cs.ATG" 
			string val = null; 
			switch (la.kind) {
			case 52: {
				lexer.NextToken();

#line  1784 "cs.ATG" 
				val = "bool"; 
				break;
			}
			case 54: {
				lexer.NextToken();

#line  1785 "cs.ATG" 
				val = "byte"; 
				break;
			}
			case 57: {
				lexer.NextToken();

#line  1786 "cs.ATG" 
				val = "char"; 
				break;
			}
			case 62: {
				lexer.NextToken();

#line  1787 "cs.ATG" 
				val = "decimal"; 
				break;
			}
			case 66: {
				lexer.NextToken();

#line  1788 "cs.ATG" 
				val = "double"; 
				break;
			}
			case 75: {
				lexer.NextToken();

#line  1789 "cs.ATG" 
				val = "float"; 
				break;
			}
			case 82: {
				lexer.NextToken();

#line  1790 "cs.ATG" 
				val = "int"; 
				break;
			}
			case 87: {
				lexer.NextToken();

#line  1791 "cs.ATG" 
				val = "long"; 
				break;
			}
			case 91: {
				lexer.NextToken();

#line  1792 "cs.ATG" 
				val = "object"; 
				break;
			}
			case 102: {
				lexer.NextToken();

#line  1793 "cs.ATG" 
				val = "sbyte"; 
				break;
			}
			case 104: {
				lexer.NextToken();

#line  1794 "cs.ATG" 
				val = "short"; 
				break;
			}
			case 108: {
				lexer.NextToken();

#line  1795 "cs.ATG" 
				val = "string"; 
				break;
			}
			case 116: {
				lexer.NextToken();

#line  1796 "cs.ATG" 
				val = "uint"; 
				break;
			}
			case 117: {
				lexer.NextToken();

#line  1797 "cs.ATG" 
				val = "ulong"; 
				break;
			}
			case 120: {
				lexer.NextToken();

#line  1798 "cs.ATG" 
				val = "ushort"; 
				break;
			}
			}
			MemberAccess(
#line  1800 "cs.ATG" 
out pexpr, new TypeReferenceExpression(val));
		} else if (la.kind == 111) {
			lexer.NextToken();

#line  1802 "cs.ATG" 
			pexpr = new ThisReferenceExpression(); 
		} else if (la.kind == 51) {
			lexer.NextToken();

#line  1804 "cs.ATG" 
			Expression retExpr = new BaseReferenceExpression(); 
			if (la.kind == 15) {
				MemberAccess(
#line  1806 "cs.ATG" 
out retExpr, retExpr);
			} else if (la.kind == 18) {
				lexer.NextToken();
				Expr(
#line  1807 "cs.ATG" 
out expr);

#line  1807 "cs.ATG" 
				List<Expression> indices = new List<Expression>(); if (expr != null) { indices.Add(expr); } 
				while (la.kind == 14) {
					lexer.NextToken();
					Expr(
#line  1808 "cs.ATG" 
out expr);

#line  1808 "cs.ATG" 
					if (expr != null) { indices.Add(expr); } 
				}
				Expect(19);

#line  1809 "cs.ATG" 
				retExpr = new IndexerExpression(retExpr, indices); 
			} else SynErr(203);

#line  1810 "cs.ATG" 
			pexpr = retExpr; 
		} else if (la.kind == 89) {
			NewExpression(
#line  1813 "cs.ATG" 
out expr);

#line  1813 "cs.ATG" 
			pexpr = expr; 
		} else if (la.kind == 115) {
			lexer.NextToken();
			Expect(20);
			if (
#line  1817 "cs.ATG" 
NotVoidPointer()) {
				Expect(123);

#line  1817 "cs.ATG" 
				type = new TypeReference("void"); 
			} else if (StartOf(10)) {
				TypeWithRestriction(
#line  1818 "cs.ATG" 
out type, true, true);
			} else SynErr(204);
			Expect(21);

#line  1819 "cs.ATG" 
			pexpr = new TypeOfExpression(type); 
		} else if (la.kind == 63) {
			lexer.NextToken();
			Expect(20);
			Type(
#line  1821 "cs.ATG" 
out type);
			Expect(21);

#line  1821 "cs.ATG" 
			pexpr = new DefaultValueExpression(type); 
		} else if (la.kind == 105) {
			lexer.NextToken();
			Expect(20);
			Type(
#line  1822 "cs.ATG" 
out type);
			Expect(21);

#line  1822 "cs.ATG" 
			pexpr = new SizeOfExpression(type); 
		} else if (la.kind == 58) {
			lexer.NextToken();
			Expect(20);
			Expr(
#line  1823 "cs.ATG" 
out expr);
			Expect(21);

#line  1823 "cs.ATG" 
			pexpr = new CheckedExpression(expr); 
		} else if (la.kind == 118) {
			lexer.NextToken();
			Expect(20);
			Expr(
#line  1824 "cs.ATG" 
out expr);
			Expect(21);

#line  1824 "cs.ATG" 
			pexpr = new UncheckedExpression(expr); 
		} else if (la.kind == 64) {
			lexer.NextToken();
			AnonymousMethodExpr(
#line  1825 "cs.ATG" 
out expr);

#line  1825 "cs.ATG" 
			pexpr = expr; 
		} else SynErr(205);
		while (StartOf(33)) {
			if (la.kind == 31 || la.kind == 32) {
				if (la.kind == 31) {
					lexer.NextToken();

#line  1829 "cs.ATG" 
					pexpr = new UnaryOperatorExpression(pexpr, UnaryOperatorType.PostIncrement); 
				} else if (la.kind == 32) {
					lexer.NextToken();

#line  1830 "cs.ATG" 
					pexpr = new UnaryOperatorExpression(pexpr, UnaryOperatorType.PostDecrement); 
				} else SynErr(206);
			} else if (la.kind == 47) {
				PointerMemberAccess(
#line  1833 "cs.ATG" 
out pexpr, pexpr);
			} else if (la.kind == 15) {
				MemberAccess(
#line  1834 "cs.ATG" 
out pexpr, pexpr);
			} else if (la.kind == 20) {
				lexer.NextToken();

#line  1837 "cs.ATG" 
				List<Expression> parameters = new List<Expression>(); 
				if (StartOf(24)) {
					Argument(
#line  1838 "cs.ATG" 
out expr);

#line  1838 "cs.ATG" 
					if (expr != null) {parameters.Add(expr);} 
					while (la.kind == 14) {
						lexer.NextToken();
						Argument(
#line  1839 "cs.ATG" 
out expr);

#line  1839 "cs.ATG" 
						if (expr != null) {parameters.Add(expr);} 
					}
				}
				Expect(21);

#line  1840 "cs.ATG" 
				pexpr = new InvocationExpression(pexpr, parameters); 
			} else {

#line  1843 "cs.ATG" 
				List<Expression> indices = new List<Expression>();
				
				lexer.NextToken();
				Expr(
#line  1845 "cs.ATG" 
out expr);

#line  1845 "cs.ATG" 
				if (expr != null) { indices.Add(expr); } 
				while (la.kind == 14) {
					lexer.NextToken();
					Expr(
#line  1846 "cs.ATG" 
out expr);

#line  1846 "cs.ATG" 
					if (expr != null) { indices.Add(expr); } 
				}
				Expect(19);

#line  1847 "cs.ATG" 
				pexpr = new IndexerExpression(pexpr, indices); 
			}
		}
	}

	void QueryExpression(
#line  2267 "cs.ATG" 
out Expression outExpr) {

#line  2268 "cs.ATG" 
		QueryExpression q = new QueryExpression(); outExpr = q; q.StartLocation = la.Location; 
		QueryExpressionFromClause fromClause;
		
		QueryExpressionFromClause(
#line  2272 "cs.ATG" 
out fromClause);

#line  2272 "cs.ATG" 
		q.FromClause = fromClause; 
		QueryExpressionBody(
#line  2273 "cs.ATG" 
q);

#line  2274 "cs.ATG" 
		q.EndLocation = t.EndLocation; 
	}

	void ShortedLambdaExpression(
#line  1956 "cs.ATG" 
IdentifierExpression ident, out Expression pexpr) {

#line  1957 "cs.ATG" 
		LambdaExpression lambda = new LambdaExpression(); pexpr = lambda; 
		Expect(48);

#line  1962 "cs.ATG" 
		lambda.StartLocation = ident.StartLocation;
		lambda.Parameters.Add(new ParameterDeclarationExpression(null, ident.Identifier));
		lambda.Parameters[0].StartLocation = ident.StartLocation;
		lambda.Parameters[0].EndLocation = ident.EndLocation;
		
		LambdaExpressionBody(
#line  1967 "cs.ATG" 
lambda);
	}

	void TypeArgumentList(
#line  2201 "cs.ATG" 
out List<TypeReference> types, bool canBeUnbound) {

#line  2203 "cs.ATG" 
		types = new List<TypeReference>();
		TypeReference type = null;
		
		Expect(23);
		if (
#line  2208 "cs.ATG" 
canBeUnbound && (la.kind == Tokens.GreaterThan || la.kind == Tokens.Comma)) {

#line  2209 "cs.ATG" 
			types.Add(TypeReference.Null); 
			while (la.kind == 14) {
				lexer.NextToken();

#line  2210 "cs.ATG" 
				types.Add(TypeReference.Null); 
			}
		} else if (StartOf(10)) {
			Type(
#line  2211 "cs.ATG" 
out type);

#line  2211 "cs.ATG" 
			if (type != null) { types.Add(type); } 
			while (la.kind == 14) {
				lexer.NextToken();
				Type(
#line  2212 "cs.ATG" 
out type);

#line  2212 "cs.ATG" 
				if (type != null) { types.Add(type); } 
			}
		} else SynErr(207);
		Expect(22);
	}

	void LambdaExpression(
#line  1936 "cs.ATG" 
out Expression outExpr) {

#line  1938 "cs.ATG" 
		LambdaExpression lambda = new LambdaExpression();
		lambda.StartLocation = la.Location;
		ParameterDeclarationExpression p;
		outExpr = lambda;
		
		Expect(20);
		if (StartOf(10)) {
			LambdaExpressionParameter(
#line  1946 "cs.ATG" 
out p);

#line  1946 "cs.ATG" 
			if (p != null) lambda.Parameters.Add(p); 
			while (la.kind == 14) {
				lexer.NextToken();
				LambdaExpressionParameter(
#line  1948 "cs.ATG" 
out p);

#line  1948 "cs.ATG" 
				if (p != null) lambda.Parameters.Add(p); 
			}
		}
		Expect(21);
		Expect(48);
		LambdaExpressionBody(
#line  1953 "cs.ATG" 
lambda);
	}

	void MemberAccess(
#line  1851 "cs.ATG" 
out Expression expr, Expression target) {

#line  1852 "cs.ATG" 
		List<TypeReference> typeList; 

#line  1854 "cs.ATG" 
		if (ShouldConvertTargetExpressionToTypeReference(target)) {
		TypeReference type = GetTypeReferenceFromExpression(target);
		if (type != null) {
			target = new TypeReferenceExpression(type);
		}
		}
		t.val = ""; // required for TypeReferenceExpressionTests.StandaloneIntReferenceExpression hack
		
		Expect(15);
		Identifier();

#line  1864 "cs.ATG" 
		expr = new MemberReferenceExpression(target, t.val); 
		if (
#line  1865 "cs.ATG" 
IsGenericInSimpleNameOrMemberAccess()) {
			TypeArgumentList(
#line  1866 "cs.ATG" 
out typeList, false);

#line  1867 "cs.ATG" 
			((MemberReferenceExpression)expr).TypeArguments = typeList; 
		}
	}

	void NewExpression(
#line  1883 "cs.ATG" 
out Expression pexpr) {

#line  1884 "cs.ATG" 
		pexpr = null;
		List<Expression> parameters = new List<Expression>();
		TypeReference type = null;
		Expression expr;
		
		Expect(89);
		if (StartOf(10)) {
			NonArrayType(
#line  1891 "cs.ATG" 
out type);
		}
		if (la.kind == 16 || la.kind == 20) {
			if (la.kind == 20) {

#line  1897 "cs.ATG" 
				ObjectCreateExpression oce = new ObjectCreateExpression(type, parameters); 
				lexer.NextToken();

#line  1898 "cs.ATG" 
				if (type == null) Error("Cannot use an anonymous type with arguments for the constructor"); 
				if (StartOf(24)) {
					Argument(
#line  1899 "cs.ATG" 
out expr);

#line  1899 "cs.ATG" 
					if (expr != null) { parameters.Add(expr); } 
					while (la.kind == 14) {
						lexer.NextToken();
						Argument(
#line  1900 "cs.ATG" 
out expr);

#line  1900 "cs.ATG" 
						if (expr != null) { parameters.Add(expr); } 
					}
				}
				Expect(21);

#line  1902 "cs.ATG" 
				pexpr = oce; 
				if (la.kind == 16) {
					CollectionOrObjectInitializer(
#line  1903 "cs.ATG" 
out expr);

#line  1903 "cs.ATG" 
					oce.ObjectInitializer = (CollectionInitializerExpression)expr; 
				}
			} else {

#line  1904 "cs.ATG" 
				ObjectCreateExpression oce = new ObjectCreateExpression(type, parameters); 
				CollectionOrObjectInitializer(
#line  1905 "cs.ATG" 
out expr);

#line  1905 "cs.ATG" 
				oce.ObjectInitializer = (CollectionInitializerExpression)expr; 

#line  1906 "cs.ATG" 
				pexpr = oce; 
			}
		} else if (la.kind == 18) {
			lexer.NextToken();

#line  1911 "cs.ATG" 
			ArrayCreateExpression ace = new ArrayCreateExpression(type);
			/* we must not change RankSpecifier on the null type reference*/
			if (ace.CreateType.IsNull) { ace.CreateType = new TypeReference(""); }
			pexpr = ace;
			int dims = 0; List<int> ranks = new List<int>();
			
			if (la.kind == 14 || la.kind == 19) {
				while (la.kind == 14) {
					lexer.NextToken();

#line  1918 "cs.ATG" 
					dims += 1; 
				}
				Expect(19);

#line  1919 "cs.ATG" 
				ranks.Add(dims); dims = 0; 
				while (la.kind == 18) {
					lexer.NextToken();
					while (la.kind == 14) {
						lexer.NextToken();

#line  1920 "cs.ATG" 
						++dims; 
					}
					Expect(19);

#line  1920 "cs.ATG" 
					ranks.Add(dims); dims = 0; 
				}

#line  1921 "cs.ATG" 
				ace.CreateType.RankSpecifier = ranks.ToArray(); 
				CollectionInitializer(
#line  1922 "cs.ATG" 
out expr);

#line  1922 "cs.ATG" 
				ace.ArrayInitializer = (CollectionInitializerExpression)expr; 
			} else if (StartOf(6)) {
				Expr(
#line  1923 "cs.ATG" 
out expr);

#line  1923 "cs.ATG" 
				if (expr != null) parameters.Add(expr); 
				while (la.kind == 14) {
					lexer.NextToken();

#line  1924 "cs.ATG" 
					dims += 1; 
					Expr(
#line  1925 "cs.ATG" 
out expr);

#line  1925 "cs.ATG" 
					if (expr != null) parameters.Add(expr); 
				}
				Expect(19);

#line  1927 "cs.ATG" 
				ranks.Add(dims); ace.Arguments = parameters; dims = 0; 
				while (la.kind == 18) {
					lexer.NextToken();
					while (la.kind == 14) {
						lexer.NextToken();

#line  1928 "cs.ATG" 
						++dims; 
					}
					Expect(19);

#line  1928 "cs.ATG" 
					ranks.Add(dims); dims = 0; 
				}

#line  1929 "cs.ATG" 
				ace.CreateType.RankSpecifier = ranks.ToArray(); 
				if (la.kind == 16) {
					CollectionInitializer(
#line  1930 "cs.ATG" 
out expr);

#line  1930 "cs.ATG" 
					ace.ArrayInitializer = (CollectionInitializerExpression)expr; 
				}
			} else SynErr(208);
		} else SynErr(209);
	}

	void AnonymousMethodExpr(
#line  1998 "cs.ATG" 
out Expression outExpr) {

#line  2000 "cs.ATG" 
		AnonymousMethodExpression expr = new AnonymousMethodExpression();
		expr.StartLocation = t.Location;
		BlockStatement stmt;
		List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
		outExpr = expr;
		
		if (la.kind == 20) {
			lexer.NextToken();
			if (StartOf(11)) {
				FormalParameterList(
#line  2009 "cs.ATG" 
p);

#line  2009 "cs.ATG" 
				expr.Parameters = p; 
			}
			Expect(21);

#line  2011 "cs.ATG" 
			expr.HasParameterList = true; 
		}
		BlockInsideExpression(
#line  2013 "cs.ATG" 
out stmt);

#line  2013 "cs.ATG" 
		expr.Body  = stmt; 

#line  2014 "cs.ATG" 
		expr.EndLocation = t.Location; 
	}

	void PointerMemberAccess(
#line  1871 "cs.ATG" 
out Expression expr, Expression target) {

#line  1872 "cs.ATG" 
		List<TypeReference> typeList; 
		Expect(47);
		Identifier();

#line  1876 "cs.ATG" 
		expr = new PointerReferenceExpression(target, t.val); 
		if (
#line  1877 "cs.ATG" 
IsGenericInSimpleNameOrMemberAccess()) {
			TypeArgumentList(
#line  1878 "cs.ATG" 
out typeList, false);

#line  1879 "cs.ATG" 
			((MemberReferenceExpression)expr).TypeArguments = typeList; 
		}
	}

	void LambdaExpressionParameter(
#line  1970 "cs.ATG" 
out ParameterDeclarationExpression p) {

#line  1971 "cs.ATG" 
		Location start = la.Location; p = null;
		TypeReference type;
		
		if (
#line  1975 "cs.ATG" 
Peek(1).kind == Tokens.Comma || Peek(1).kind == Tokens.CloseParenthesis) {
			Identifier();

#line  1977 "cs.ATG" 
			p = new ParameterDeclarationExpression(null, t.val);
			p.StartLocation = start; p.EndLocation = t.EndLocation;
			
		} else if (StartOf(10)) {
			Type(
#line  1980 "cs.ATG" 
out type);
			Identifier();

#line  1982 "cs.ATG" 
			p = new ParameterDeclarationExpression(type, t.val);
			p.StartLocation = start; p.EndLocation = t.EndLocation;
			
		} else SynErr(210);
	}

	void LambdaExpressionBody(
#line  1988 "cs.ATG" 
LambdaExpression lambda) {

#line  1989 "cs.ATG" 
		Expression expr; BlockStatement stmt; 
		if (la.kind == 16) {
			BlockInsideExpression(
#line  1992 "cs.ATG" 
out stmt);

#line  1992 "cs.ATG" 
			lambda.StatementBody = stmt; 
		} else if (StartOf(6)) {
			Expr(
#line  1993 "cs.ATG" 
out expr);

#line  1993 "cs.ATG" 
			lambda.ExpressionBody = expr; 
		} else SynErr(211);

#line  1995 "cs.ATG" 
		lambda.EndLocation = t.EndLocation; 
	}

	void BlockInsideExpression(
#line  2017 "cs.ATG" 
out BlockStatement outStmt) {

#line  2018 "cs.ATG" 
		Statement stmt = null; outStmt = null; 

#line  2022 "cs.ATG" 
		if (compilationUnit != null) { 
		Block(
#line  2023 "cs.ATG" 
out stmt);

#line  2023 "cs.ATG" 
		outStmt = (BlockStatement)stmt; 

#line  2024 "cs.ATG" 
		} else { 
		Expect(16);

#line  2026 "cs.ATG" 
		lexer.SkipCurrentBlock(0); 
		Expect(17);

#line  2028 "cs.ATG" 
		} 
	}

	void ConditionalAndExpr(
#line  2037 "cs.ATG" 
ref Expression outExpr) {

#line  2038 "cs.ATG" 
		Expression expr; 
		InclusiveOrExpr(
#line  2040 "cs.ATG" 
ref outExpr);
		while (la.kind == 25) {
			lexer.NextToken();
			UnaryExpr(
#line  2040 "cs.ATG" 
out expr);
			InclusiveOrExpr(
#line  2040 "cs.ATG" 
ref expr);

#line  2040 "cs.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.LogicalAnd, expr);  
		}
	}

	void InclusiveOrExpr(
#line  2043 "cs.ATG" 
ref Expression outExpr) {

#line  2044 "cs.ATG" 
		Expression expr; 
		ExclusiveOrExpr(
#line  2046 "cs.ATG" 
ref outExpr);
		while (la.kind == 29) {
			lexer.NextToken();
			UnaryExpr(
#line  2046 "cs.ATG" 
out expr);
			ExclusiveOrExpr(
#line  2046 "cs.ATG" 
ref expr);

#line  2046 "cs.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.BitwiseOr, expr);  
		}
	}

	void ExclusiveOrExpr(
#line  2049 "cs.ATG" 
ref Expression outExpr) {

#line  2050 "cs.ATG" 
		Expression expr; 
		AndExpr(
#line  2052 "cs.ATG" 
ref outExpr);
		while (la.kind == 30) {
			lexer.NextToken();
			UnaryExpr(
#line  2052 "cs.ATG" 
out expr);
			AndExpr(
#line  2052 "cs.ATG" 
ref expr);

#line  2052 "cs.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.ExclusiveOr, expr);  
		}
	}

	void AndExpr(
#line  2055 "cs.ATG" 
ref Expression outExpr) {

#line  2056 "cs.ATG" 
		Expression expr; 
		EqualityExpr(
#line  2058 "cs.ATG" 
ref outExpr);
		while (la.kind == 28) {
			lexer.NextToken();
			UnaryExpr(
#line  2058 "cs.ATG" 
out expr);
			EqualityExpr(
#line  2058 "cs.ATG" 
ref expr);

#line  2058 "cs.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.BitwiseAnd, expr);  
		}
	}

	void EqualityExpr(
#line  2061 "cs.ATG" 
ref Expression outExpr) {

#line  2063 "cs.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		RelationalExpr(
#line  2067 "cs.ATG" 
ref outExpr);
		while (la.kind == 33 || la.kind == 34) {
			if (la.kind == 34) {
				lexer.NextToken();

#line  2070 "cs.ATG" 
				op = BinaryOperatorType.InEquality; 
			} else {
				lexer.NextToken();

#line  2071 "cs.ATG" 
				op = BinaryOperatorType.Equality; 
			}
			UnaryExpr(
#line  2073 "cs.ATG" 
out expr);
			RelationalExpr(
#line  2073 "cs.ATG" 
ref expr);

#line  2073 "cs.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr);  
		}
	}

	void RelationalExpr(
#line  2077 "cs.ATG" 
ref Expression outExpr) {

#line  2079 "cs.ATG" 
		TypeReference type;
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		ShiftExpr(
#line  2084 "cs.ATG" 
ref outExpr);
		while (StartOf(34)) {
			if (StartOf(35)) {
				if (la.kind == 23) {
					lexer.NextToken();

#line  2086 "cs.ATG" 
					op = BinaryOperatorType.LessThan; 
				} else if (la.kind == 22) {
					lexer.NextToken();

#line  2087 "cs.ATG" 
					op = BinaryOperatorType.GreaterThan; 
				} else if (la.kind == 36) {
					lexer.NextToken();

#line  2088 "cs.ATG" 
					op = BinaryOperatorType.LessThanOrEqual; 
				} else if (la.kind == 35) {
					lexer.NextToken();

#line  2089 "cs.ATG" 
					op = BinaryOperatorType.GreaterThanOrEqual; 
				} else SynErr(212);
				UnaryExpr(
#line  2091 "cs.ATG" 
out expr);
				ShiftExpr(
#line  2092 "cs.ATG" 
ref expr);

#line  2093 "cs.ATG" 
				outExpr = new BinaryOperatorExpression(outExpr, op, expr); 
			} else {
				if (la.kind == 85) {
					lexer.NextToken();
					TypeWithRestriction(
#line  2096 "cs.ATG" 
out type, false, false);
					if (
#line  2097 "cs.ATG" 
la.kind == Tokens.Question && !IsPossibleExpressionStart(Peek(1).kind)) {
						NullableQuestionMark(
#line  2098 "cs.ATG" 
ref type);
					}

#line  2099 "cs.ATG" 
					outExpr = new TypeOfIsExpression(outExpr, type); 
				} else if (la.kind == 50) {
					lexer.NextToken();
					TypeWithRestriction(
#line  2101 "cs.ATG" 
out type, false, false);
					if (
#line  2102 "cs.ATG" 
la.kind == Tokens.Question && !IsPossibleExpressionStart(Peek(1).kind)) {
						NullableQuestionMark(
#line  2103 "cs.ATG" 
ref type);
					}

#line  2104 "cs.ATG" 
					outExpr = new CastExpression(type, outExpr, CastType.TryCast); 
				} else SynErr(213);
			}
		}
	}

	void ShiftExpr(
#line  2109 "cs.ATG" 
ref Expression outExpr) {

#line  2111 "cs.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		AdditiveExpr(
#line  2115 "cs.ATG" 
ref outExpr);
		while (la.kind == 37 || 
#line  2118 "cs.ATG" 
IsShiftRight()) {
			if (la.kind == 37) {
				lexer.NextToken();

#line  2117 "cs.ATG" 
				op = BinaryOperatorType.ShiftLeft; 
			} else {
				Expect(22);
				Expect(22);

#line  2119 "cs.ATG" 
				op = BinaryOperatorType.ShiftRight; 
			}
			UnaryExpr(
#line  2122 "cs.ATG" 
out expr);
			AdditiveExpr(
#line  2122 "cs.ATG" 
ref expr);

#line  2122 "cs.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr);  
		}
	}

	void AdditiveExpr(
#line  2126 "cs.ATG" 
ref Expression outExpr) {

#line  2128 "cs.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		MultiplicativeExpr(
#line  2132 "cs.ATG" 
ref outExpr);
		while (la.kind == 4 || la.kind == 5) {
			if (la.kind == 4) {
				lexer.NextToken();

#line  2135 "cs.ATG" 
				op = BinaryOperatorType.Add; 
			} else {
				lexer.NextToken();

#line  2136 "cs.ATG" 
				op = BinaryOperatorType.Subtract; 
			}
			UnaryExpr(
#line  2138 "cs.ATG" 
out expr);
			MultiplicativeExpr(
#line  2138 "cs.ATG" 
ref expr);

#line  2138 "cs.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr);  
		}
	}

	void MultiplicativeExpr(
#line  2142 "cs.ATG" 
ref Expression outExpr) {

#line  2144 "cs.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		while (la.kind == 6 || la.kind == 7 || la.kind == 8) {
			if (la.kind == 6) {
				lexer.NextToken();

#line  2150 "cs.ATG" 
				op = BinaryOperatorType.Multiply; 
			} else if (la.kind == 7) {
				lexer.NextToken();

#line  2151 "cs.ATG" 
				op = BinaryOperatorType.Divide; 
			} else {
				lexer.NextToken();

#line  2152 "cs.ATG" 
				op = BinaryOperatorType.Modulus; 
			}
			UnaryExpr(
#line  2154 "cs.ATG" 
out expr);

#line  2154 "cs.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr); 
		}
	}

	void TypeParameterConstraintsClauseBase(
#line  2258 "cs.ATG" 
out TypeReference type) {

#line  2259 "cs.ATG" 
		TypeReference t; type = null; 
		if (la.kind == 109) {
			lexer.NextToken();

#line  2261 "cs.ATG" 
			type = TypeReference.StructConstraint; 
		} else if (la.kind == 59) {
			lexer.NextToken();

#line  2262 "cs.ATG" 
			type = TypeReference.ClassConstraint; 
		} else if (la.kind == 89) {
			lexer.NextToken();
			Expect(20);
			Expect(21);

#line  2263 "cs.ATG" 
			type = TypeReference.NewConstraint; 
		} else if (StartOf(10)) {
			Type(
#line  2264 "cs.ATG" 
out t);

#line  2264 "cs.ATG" 
			type = t; 
		} else SynErr(214);
	}

	void QueryExpressionFromClause(
#line  2277 "cs.ATG" 
out QueryExpressionFromClause fc) {

#line  2278 "cs.ATG" 
		fc = new QueryExpressionFromClause(); fc.StartLocation = la.Location; 
		
		Expect(137);
		QueryExpressionFromOrJoinClause(
#line  2282 "cs.ATG" 
fc);

#line  2283 "cs.ATG" 
		fc.EndLocation = t.EndLocation; 
	}

	void QueryExpressionBody(
#line  2313 "cs.ATG" 
QueryExpression q) {

#line  2314 "cs.ATG" 
		QueryExpressionFromClause fromClause;     QueryExpressionWhereClause whereClause;
		QueryExpressionLetClause letClause;       QueryExpressionJoinClause joinClause;
		QueryExpressionSelectClause selectClause; QueryExpressionGroupClause groupClause;
		QueryExpressionIntoClause intoClause;
		
		while (StartOf(36)) {
			if (la.kind == 137) {
				QueryExpressionFromClause(
#line  2320 "cs.ATG" 
out fromClause);

#line  2320 "cs.ATG" 
				SafeAdd<QueryExpressionClause>(q, q.FromLetWhereClauses, fromClause); 
			} else if (la.kind == 127) {
				QueryExpressionWhereClause(
#line  2321 "cs.ATG" 
out whereClause);

#line  2321 "cs.ATG" 
				SafeAdd<QueryExpressionClause>(q, q.FromLetWhereClauses, whereClause); 
			} else if (la.kind == 141) {
				QueryExpressionLetClause(
#line  2322 "cs.ATG" 
out letClause);

#line  2322 "cs.ATG" 
				SafeAdd<QueryExpressionClause>(q, q.FromLetWhereClauses, letClause); 
			} else {
				QueryExpressionJoinClause(
#line  2323 "cs.ATG" 
out joinClause);

#line  2323 "cs.ATG" 
				SafeAdd<QueryExpressionClause>(q, q.FromLetWhereClauses, joinClause); 
			}
		}
		if (la.kind == 140) {
			QueryExpressionOrderByClause(
#line  2325 "cs.ATG" 
q);
		}
		if (la.kind == 133) {
			QueryExpressionSelectClause(
#line  2326 "cs.ATG" 
out selectClause);

#line  2326 "cs.ATG" 
			q.SelectOrGroupClause = selectClause; 
		} else if (la.kind == 134) {
			QueryExpressionGroupClause(
#line  2327 "cs.ATG" 
out groupClause);

#line  2327 "cs.ATG" 
			q.SelectOrGroupClause = groupClause; 
		} else SynErr(215);
		if (la.kind == 136) {
			QueryExpressionIntoClause(
#line  2329 "cs.ATG" 
out intoClause);

#line  2329 "cs.ATG" 
			q.IntoClause = intoClause; 
		}
	}

	void QueryExpressionFromOrJoinClause(
#line  2303 "cs.ATG" 
QueryExpressionFromOrJoinClause fjc) {

#line  2304 "cs.ATG" 
		TypeReference type; Expression expr; 

#line  2306 "cs.ATG" 
		fjc.Type = null; 
		Identifier();

#line  2308 "cs.ATG" 
		fjc.Identifier = t.val; 
		Expect(81);
		Expr(
#line  2310 "cs.ATG" 
out expr);

#line  2310 "cs.ATG" 
		fjc.InExpression = expr; 
	}

	void QueryExpressionJoinClause(
#line  2286 "cs.ATG" 
out QueryExpressionJoinClause jc) {

#line  2287 "cs.ATG" 
		jc = new QueryExpressionJoinClause(); jc.StartLocation = la.Location; 
		Expression expr;
		
		Expect(142);
		QueryExpressionFromOrJoinClause(
#line  2292 "cs.ATG" 
jc);
		Expect(143);
		Expr(
#line  2294 "cs.ATG" 
out expr);

#line  2294 "cs.ATG" 
		jc.OnExpression = expr; 
		Expect(144);
		Expr(
#line  2296 "cs.ATG" 
out expr);

#line  2296 "cs.ATG" 
		jc.EqualsExpression = expr; 
		if (la.kind == 136) {
			lexer.NextToken();
			Identifier();

#line  2298 "cs.ATG" 
			jc.IntoIdentifier = t.val; 
		}

#line  2300 "cs.ATG" 
		jc.EndLocation = t.EndLocation; 
	}

	void QueryExpressionWhereClause(
#line  2332 "cs.ATG" 
out QueryExpressionWhereClause wc) {

#line  2333 "cs.ATG" 
		Expression expr; wc = new QueryExpressionWhereClause(); wc.StartLocation = la.Location; 
		Expect(127);
		Expr(
#line  2336 "cs.ATG" 
out expr);

#line  2336 "cs.ATG" 
		wc.Condition = expr; 

#line  2337 "cs.ATG" 
		wc.EndLocation = t.EndLocation; 
	}

	void QueryExpressionLetClause(
#line  2340 "cs.ATG" 
out QueryExpressionLetClause wc) {

#line  2341 "cs.ATG" 
		Expression expr; wc = new QueryExpressionLetClause(); wc.StartLocation = la.Location; 
		Expect(141);
		Identifier();

#line  2344 "cs.ATG" 
		wc.Identifier = t.val; 
		Expect(3);
		Expr(
#line  2346 "cs.ATG" 
out expr);

#line  2346 "cs.ATG" 
		wc.Expression = expr; 

#line  2347 "cs.ATG" 
		wc.EndLocation = t.EndLocation; 
	}

	void QueryExpressionOrderByClause(
#line  2350 "cs.ATG" 
QueryExpression q) {

#line  2351 "cs.ATG" 
		QueryExpressionOrdering ordering; 
		Expect(140);
		QueryExpressionOrderingClause(
#line  2354 "cs.ATG" 
out ordering);

#line  2354 "cs.ATG" 
		SafeAdd(q, q.Orderings, ordering); 
		while (la.kind == 14) {
			lexer.NextToken();
			QueryExpressionOrderingClause(
#line  2356 "cs.ATG" 
out ordering);

#line  2356 "cs.ATG" 
			SafeAdd(q, q.Orderings, ordering); 
		}
	}

	void QueryExpressionSelectClause(
#line  2370 "cs.ATG" 
out QueryExpressionSelectClause sc) {

#line  2371 "cs.ATG" 
		Expression expr; sc = new QueryExpressionSelectClause(); sc.StartLocation = la.Location; 
		Expect(133);
		Expr(
#line  2374 "cs.ATG" 
out expr);

#line  2374 "cs.ATG" 
		sc.Projection = expr; 

#line  2375 "cs.ATG" 
		sc.EndLocation = t.EndLocation; 
	}

	void QueryExpressionGroupClause(
#line  2378 "cs.ATG" 
out QueryExpressionGroupClause gc) {

#line  2379 "cs.ATG" 
		Expression expr; gc = new QueryExpressionGroupClause(); gc.StartLocation = la.Location; 
		Expect(134);
		Expr(
#line  2382 "cs.ATG" 
out expr);

#line  2382 "cs.ATG" 
		gc.Projection = expr; 
		Expect(135);
		Expr(
#line  2384 "cs.ATG" 
out expr);

#line  2384 "cs.ATG" 
		gc.GroupBy = expr; 

#line  2385 "cs.ATG" 
		gc.EndLocation = t.EndLocation; 
	}

	void QueryExpressionIntoClause(
#line  2388 "cs.ATG" 
out QueryExpressionIntoClause ic) {

#line  2389 "cs.ATG" 
		ic = new QueryExpressionIntoClause(); ic.StartLocation = la.Location; 
		Expect(136);
		Identifier();

#line  2392 "cs.ATG" 
		ic.IntoIdentifier = t.val; 

#line  2393 "cs.ATG" 
		ic.ContinuedQuery = new QueryExpression(); 

#line  2394 "cs.ATG" 
		ic.ContinuedQuery.StartLocation = la.Location; 
		QueryExpressionBody(
#line  2395 "cs.ATG" 
ic.ContinuedQuery);

#line  2396 "cs.ATG" 
		ic.ContinuedQuery.EndLocation = t.EndLocation; 

#line  2397 "cs.ATG" 
		ic.EndLocation = t.EndLocation; 
	}

	void QueryExpressionOrderingClause(
#line  2360 "cs.ATG" 
out QueryExpressionOrdering ordering) {

#line  2361 "cs.ATG" 
		Expression expr; ordering = new QueryExpressionOrdering(); ordering.StartLocation = la.Location; 
		Expr(
#line  2363 "cs.ATG" 
out expr);

#line  2363 "cs.ATG" 
		ordering.Criteria = expr; 
		if (la.kind == 138 || la.kind == 139) {
			if (la.kind == 138) {
				lexer.NextToken();

#line  2364 "cs.ATG" 
				ordering.Direction = QueryExpressionOrderingDirection.Ascending; 
			} else {
				lexer.NextToken();

#line  2365 "cs.ATG" 
				ordering.Direction = QueryExpressionOrderingDirection.Descending; 
			}
		}

#line  2367 "cs.ATG" 
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
			case 141: s = "\"let\" expected"; break;
			case 142: s = "\"join\" expected"; break;
			case 143: s = "\"on\" expected"; break;
			case 144: s = "\"equals\" expected"; break;
			case 145: s = "??? expected"; break;
			case 146: s = "invalid NamespaceMemberDecl"; break;
			case 147: s = "invalid NonArrayType"; break;
			case 148: s = "invalid Identifier"; break;
			case 149: s = "invalid AttributeArguments"; break;
			case 150: s = "invalid Expr"; break;
			case 151: s = "invalid TypeModifier"; break;
			case 152: s = "invalid TypeDecl"; break;
			case 153: s = "invalid TypeDecl"; break;
			case 154: s = "invalid IntegralType"; break;
			case 155: s = "invalid FormalParameterList"; break;
			case 156: s = "invalid FormalParameterList"; break;
			case 157: s = "invalid ClassType"; break;
			case 158: s = "invalid ClassMemberDecl"; break;
			case 159: s = "invalid ClassMemberDecl"; break;
			case 160: s = "invalid StructMemberDecl"; break;
			case 161: s = "invalid StructMemberDecl"; break;
			case 162: s = "invalid StructMemberDecl"; break;
			case 163: s = "invalid StructMemberDecl"; break;
			case 164: s = "invalid StructMemberDecl"; break;
			case 165: s = "invalid StructMemberDecl"; break;
			case 166: s = "invalid StructMemberDecl"; break;
			case 167: s = "invalid StructMemberDecl"; break;
			case 168: s = "invalid StructMemberDecl"; break;
			case 169: s = "invalid StructMemberDecl"; break;
			case 170: s = "invalid StructMemberDecl"; break;
			case 171: s = "invalid StructMemberDecl"; break;
			case 172: s = "invalid StructMemberDecl"; break;
			case 173: s = "invalid InterfaceMemberDecl"; break;
			case 174: s = "invalid InterfaceMemberDecl"; break;
			case 175: s = "invalid InterfaceMemberDecl"; break;
			case 176: s = "invalid TypeWithRestriction"; break;
			case 177: s = "invalid TypeWithRestriction"; break;
			case 178: s = "invalid SimpleType"; break;
			case 179: s = "invalid AccessorModifiers"; break;
			case 180: s = "invalid EventAccessorDecls"; break;
			case 181: s = "invalid ConstructorInitializer"; break;
			case 182: s = "invalid OverloadableOperator"; break;
			case 183: s = "invalid AccessorDecls"; break;
			case 184: s = "invalid InterfaceAccessors"; break;
			case 185: s = "invalid InterfaceAccessors"; break;
			case 186: s = "invalid GetAccessorDecl"; break;
			case 187: s = "invalid SetAccessorDecl"; break;
			case 188: s = "invalid VariableInitializer"; break;
			case 189: s = "invalid Statement"; break;
			case 190: s = "invalid AssignmentOperator"; break;
			case 191: s = "invalid ObjectPropertyInitializerOrVariableInitializer"; break;
			case 192: s = "invalid ObjectPropertyInitializerOrVariableInitializer"; break;
			case 193: s = "invalid EmbeddedStatement"; break;
			case 194: s = "invalid EmbeddedStatement"; break;
			case 195: s = "invalid EmbeddedStatement"; break;
			case 196: s = "invalid ForInitializer"; break;
			case 197: s = "invalid GotoStatement"; break;
			case 198: s = "invalid TryStatement"; break;
			case 199: s = "invalid ResourceAcquisition"; break;
			case 200: s = "invalid SwitchLabel"; break;
			case 201: s = "invalid CatchClauses"; break;
			case 202: s = "invalid UnaryExpr"; break;
			case 203: s = "invalid PrimaryExpr"; break;
			case 204: s = "invalid PrimaryExpr"; break;
			case 205: s = "invalid PrimaryExpr"; break;
			case 206: s = "invalid PrimaryExpr"; break;
			case 207: s = "invalid TypeArgumentList"; break;
			case 208: s = "invalid NewExpression"; break;
			case 209: s = "invalid NewExpression"; break;
			case 210: s = "invalid LambdaExpressionParameter"; break;
			case 211: s = "invalid LambdaExpressionBody"; break;
			case 212: s = "invalid RelationalExpr"; break;
			case 213: s = "invalid RelationalExpr"; break;
			case 214: s = "invalid TypeParameterConstraintsClauseBase"; break;
			case 215: s = "invalid QueryExpressionBody"; break;

			default: s = "error " + errorNumber; break;
		}
		this.Errors.Error(line, col, s);
	}
	
	private bool StartOf(int s)
	{
		return set[s, lexer.LookAhead.kind];
	}
	
	static bool[,] set = {
	{T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, T,T,x,x, x,x,x,x, T,T,T,x, x,x,x,T, x,x,x,T, x,T,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,T,x,x, x,x,x,x, T,T,T,x, x,x,x,T, x,x,x,T, x,T,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, T,T,T,x, x,x,x,T, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,x, x,T,x,x, x,x,T,x, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,T,T,x, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, T,x,x,T, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,T,x, x,T,T,x, x,x,T,T, T,x,T,x, x,x,x,x, T,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,T,T,T, x,x,x,x, x,x,x,x, x,x,T,x, T,T,x,x, T,x,x,T, x,T,x,T, T,T,T,x, T,x,x,x, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
	{x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,T,x,T, T,T,T,T, T,T,x,T, T,T,T,T, T,T,T,T, T,T,T,T, x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, x,T,T,x, T,x,T,x, x,T,x,T, T,x,T,x, T,x,T,x, T,T,T,T, x,x,T,T, x,x,x,x, T,x,T,T, T,T,x,T, x,T,x,T, x,x,T,x, T,T,T,T, x,x,T,T, T,x,x,T, T,T,x,x, x,x,x,x, T,T,x,T, T,x,T,T, T,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,x, x,T,x,x, x,x,T,x, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,T, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,x, T,x,x,x, x,x,x,x, T,T,x,x, T,x,x,T, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,x, x,T,x,x, x,x,T,x, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,T, x,T,x,T, x,x,x,x, T,x,T,x, T,x,x,x, T,x,x,x, x,x,x,x, T,T,x,x, T,x,x,T, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, T,x,T,x, x,T,x,T, T,x,T,x, T,x,T,x, T,T,T,T, x,x,T,T, x,x,x,x, T,x,T,T, T,x,x,T, x,T,x,T, x,x,T,x, T,T,T,T, x,x,T,T, T,x,x,T, T,T,x,x, x,x,x,x, T,T,x,T, T,x,T,T, T,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, T,x,T,x, x,T,x,T, T,x,T,x, T,x,T,x, T,T,T,T, x,x,T,T, x,x,x,x, T,x,T,T, T,x,x,T, x,T,x,T, x,x,T,x, T,T,T,T, x,x,T,T, T,x,x,T, T,T,x,x, x,x,x,x, T,T,x,T, T,x,T,T, T,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,x, x,T,x,x, x,x,T,x, x,x,T,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,T,x,T, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,x, T,x,x,x, x,x,x,x, T,T,x,x, T,x,x,T, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,x, x,T,x,x, x,x,T,x, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,T, x,T,x,x, x,x,x,x, T,x,T,x, T,x,x,x, T,x,x,x, x,x,x,x, T,T,x,x, T,x,x,T, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,x, T,x,x,x, x,T,x,x, x,x,T,x, T,T,T,T, x,x,x,T, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,x, T,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,x, x,T,x,T, T,x,T,x, T,x,T,x, T,T,T,x, x,x,x,T, x,x,x,x, T,x,T,T, x,x,x,T, x,x,x,T, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,x, T,T,x,x, x,x,x,x, T,T,x,x, T,x,x,T, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,x, x,T,x,x, x,x,T,x, x,x,T,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,T, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,x, T,x,x,x, x,x,x,x, T,T,x,x, T,x,x,T, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,T,T,x, T,T,T,x, x,x,x,T, x,x,x,x, T,x,x,x, T,x,x,x, T,x,x,T, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,T,x, x,T,T,x, T,T,T,T, T,T,T,x, x,x,x,x, T,x,T,T, T,T,T,T, x,x,T,x, x,x,T,T, x,T,T,T, x,x,x,x, x,x,x,x, x,T,T,x, T,T,x,x, T,x,T,T, T,T,T,T, T,T,T,T, T,T,x,x, x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
	{x,T,T,x, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, T,x,x,T, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,T,x, x,T,T,x, x,x,T,T, T,x,T,x, x,x,x,x, T,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,T,T,T, x,T,x,x, x,x,x,x, T,x,T,x, T,T,x,x, T,x,x,T, x,T,x,T, T,T,T,x, T,x,x,x, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,T,T,x, T,T,T,x, x,x,x,T, x,x,x,x, T,x,x,x, T,x,x,x, T,x,x,T, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,T,x, x,T,T,x, x,T,T,T, T,T,T,x, x,x,x,x, T,x,T,T, T,T,T,T, x,x,T,x, x,x,T,T, x,T,T,T, x,x,x,x, x,x,x,x, x,T,T,x, T,T,x,x, T,x,T,T, T,T,T,T, T,T,T,T, T,T,x,x, x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
	{x,T,T,x, T,T,T,x, x,x,x,x, x,x,x,x, T,x,x,x, T,x,x,x, T,x,x,T, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,T,x, x,T,T,x, x,x,T,T, T,x,T,x, x,x,x,x, T,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,T,T,T, x,x,x,x, x,x,x,x, x,x,T,x, T,T,T,x, T,x,x,T, x,T,x,T, T,T,T,x, T,x,x,x, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
	{x,T,T,x, T,T,T,x, x,x,x,T, x,x,x,x, T,x,x,x, T,x,x,x, T,x,x,T, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,T,T, x,T,T,x, T,T,T,T, T,T,T,x, x,x,x,x, T,x,T,T, T,T,T,T, x,x,T,x, x,x,T,T, x,T,T,T, x,x,x,x, x,x,x,x, x,T,T,x, T,T,x,x, T,x,T,T, T,T,T,T, T,T,T,T, T,T,x,x, x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
	{x,x,x,x, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,T, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,T,x, x,T,T,x, x,x,T,T, T,x,T,x, x,x,x,x, T,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,T,T,T, x,x,x,x, x,x,x,x, x,x,T,x, T,T,x,x, T,x,x,T, x,T,x,T, T,T,T,x, T,x,x,x, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,x, x,T,x,x, x,x,T,x, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,T, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,x, T,x,x,x, x,x,x,x, T,T,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,T,x,x, x,T,T,x, x,x,x}

	};
} // end Parser

}