
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
		if (t.val != "assembly" && t.val != "module") Error("global attribute target specifier (assembly or module) expected");
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

#line  311 "cs.ATG" 
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		ModifierList m = new ModifierList();
		string qualident;
		
		if (la.kind == 88) {
			lexer.NextToken();

#line  317 "cs.ATG" 
			Location startPos = t.Location; 
			Qualident(
#line  318 "cs.ATG" 
out qualident);

#line  318 "cs.ATG" 
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

#line  327 "cs.ATG" 
			node.EndLocation   = t.EndLocation;
			compilationUnit.BlockEnd();
			
		} else if (StartOf(2)) {
			while (la.kind == 18) {
				AttributeSection(
#line  331 "cs.ATG" 
out section);

#line  331 "cs.ATG" 
				attributes.Add(section); 
			}
			while (StartOf(3)) {
				TypeModifier(
#line  332 "cs.ATG" 
m);
			}
			TypeDecl(
#line  333 "cs.ATG" 
m, attributes);
		} else SynErr(146);
	}

	void Qualident(
#line  457 "cs.ATG" 
out string qualident) {
		Identifier();

#line  459 "cs.ATG" 
		qualidentBuilder.Length = 0; qualidentBuilder.Append(t.val); 
		while (
#line  460 "cs.ATG" 
DotAndIdent()) {
			Expect(15);
			Identifier();

#line  460 "cs.ATG" 
			qualidentBuilder.Append('.');
			qualidentBuilder.Append(t.val); 
			
		}

#line  463 "cs.ATG" 
		qualident = qualidentBuilder.ToString(); 
	}

	void NonArrayType(
#line  572 "cs.ATG" 
out TypeReference type) {

#line  574 "cs.ATG" 
		string name;
		int pointer = 0;
		type = null;
		
		if (StartOf(4)) {
			ClassType(
#line  579 "cs.ATG" 
out type, false);
		} else if (StartOf(5)) {
			SimpleType(
#line  580 "cs.ATG" 
out name);

#line  580 "cs.ATG" 
			type = new TypeReference(name); 
		} else if (la.kind == 123) {
			lexer.NextToken();
			Expect(6);

#line  581 "cs.ATG" 
			pointer = 1; type = new TypeReference("void"); 
		} else SynErr(147);
		if (la.kind == 12) {
			NullableQuestionMark(
#line  584 "cs.ATG" 
ref type);
		}
		while (
#line  586 "cs.ATG" 
IsPointer()) {
			Expect(6);

#line  587 "cs.ATG" 
			++pointer; 
		}

#line  589 "cs.ATG" 
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
#line  1715 "cs.ATG" 
out Expression expr) {

#line  1716 "cs.ATG" 
		expr = null; Expression expr1 = null, expr2 = null; AssignmentOperatorType op; 

#line  1718 "cs.ATG" 
		Location startLocation = la.Location; 
		UnaryExpr(
#line  1719 "cs.ATG" 
out expr);
		if (StartOf(7)) {
			AssignmentOperator(
#line  1722 "cs.ATG" 
out op);
			Expr(
#line  1722 "cs.ATG" 
out expr1);

#line  1722 "cs.ATG" 
			expr = new AssignmentExpression(expr, op, expr1); 
		} else if (
#line  1723 "cs.ATG" 
la.kind == Tokens.GreaterThan && Peek(1).kind == Tokens.GreaterEqual) {
			AssignmentOperator(
#line  1724 "cs.ATG" 
out op);
			Expr(
#line  1724 "cs.ATG" 
out expr1);

#line  1724 "cs.ATG" 
			expr = new AssignmentExpression(expr, op, expr1); 
		} else if (StartOf(8)) {
			ConditionalOrExpr(
#line  1726 "cs.ATG" 
ref expr);
			if (la.kind == 13) {
				lexer.NextToken();
				Expr(
#line  1727 "cs.ATG" 
out expr1);

#line  1727 "cs.ATG" 
				expr = new BinaryOperatorExpression(expr, BinaryOperatorType.NullCoalescing, expr1); 
			}
			if (la.kind == 12) {
				lexer.NextToken();
				Expr(
#line  1728 "cs.ATG" 
out expr1);
				Expect(9);
				Expr(
#line  1728 "cs.ATG" 
out expr2);

#line  1728 "cs.ATG" 
				expr = new ConditionalExpression(expr, expr1, expr2);  
			}
		} else SynErr(150);

#line  1731 "cs.ATG" 
		if (expr != null) {
		expr.StartLocation = startLocation;
		expr.EndLocation = t.EndLocation;
		}
		
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
				if (t.val != "field"   && t.val != "method" &&
				  t.val != "param" &&
				  t.val != "property" && t.val != "type")
				Error("attribute target specifier (field, event, method, param, property, return or type) expected");
				attributeTarget = t.val;
				
			}
			Expect(9);
		}
		Attribute(
#line  300 "cs.ATG" 
out attribute);

#line  300 "cs.ATG" 
		attributes.Add(attribute); 
		while (
#line  301 "cs.ATG" 
NotFinalComma()) {
			Expect(14);
			Attribute(
#line  301 "cs.ATG" 
out attribute);

#line  301 "cs.ATG" 
			attributes.Add(attribute); 
		}
		if (la.kind == 14) {
			lexer.NextToken();
		}
		Expect(19);

#line  303 "cs.ATG" 
		section = new AttributeSection(attributeTarget, attributes);
		section.StartLocation = startPos;
		section.EndLocation = t.EndLocation;
		
	}

	void TypeModifier(
#line  659 "cs.ATG" 
ModifierList m) {
		switch (la.kind) {
		case 89: {
			lexer.NextToken();

#line  661 "cs.ATG" 
			m.Add(Modifiers.New, t.Location); 
			break;
		}
		case 98: {
			lexer.NextToken();

#line  662 "cs.ATG" 
			m.Add(Modifiers.Public, t.Location); 
			break;
		}
		case 97: {
			lexer.NextToken();

#line  663 "cs.ATG" 
			m.Add(Modifiers.Protected, t.Location); 
			break;
		}
		case 84: {
			lexer.NextToken();

#line  664 "cs.ATG" 
			m.Add(Modifiers.Internal, t.Location); 
			break;
		}
		case 96: {
			lexer.NextToken();

#line  665 "cs.ATG" 
			m.Add(Modifiers.Private, t.Location); 
			break;
		}
		case 119: {
			lexer.NextToken();

#line  666 "cs.ATG" 
			m.Add(Modifiers.Unsafe, t.Location); 
			break;
		}
		case 49: {
			lexer.NextToken();

#line  667 "cs.ATG" 
			m.Add(Modifiers.Abstract, t.Location); 
			break;
		}
		case 103: {
			lexer.NextToken();

#line  668 "cs.ATG" 
			m.Add(Modifiers.Sealed, t.Location); 
			break;
		}
		case 107: {
			lexer.NextToken();

#line  669 "cs.ATG" 
			m.Add(Modifiers.Static, t.Location); 
			break;
		}
		case 126: {
			lexer.NextToken();

#line  670 "cs.ATG" 
			m.Add(Modifiers.Partial, t.Location); 
			break;
		}
		default: SynErr(151); break;
		}
	}

	void TypeDecl(
#line  336 "cs.ATG" 
ModifierList m, List<AttributeSection> attributes) {

#line  338 "cs.ATG" 
		TypeReference type;
		List<TypeReference> names;
		List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
		string name;
		List<TemplateDefinition> templates;
		
		if (la.kind == 59) {

#line  344 "cs.ATG" 
			m.Check(Modifiers.Classes); 
			lexer.NextToken();

#line  345 "cs.ATG" 
			TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
			templates = newType.Templates;
			compilationUnit.AddChild(newType);
			compilationUnit.BlockStart(newType);
			newType.StartLocation = m.GetDeclarationLocation(t.Location);
			
			newType.Type = Types.Class;
			
			Identifier();

#line  353 "cs.ATG" 
			newType.Name = t.val; 
			if (la.kind == 23) {
				TypeParameterList(
#line  356 "cs.ATG" 
templates);
			}
			if (la.kind == 9) {
				ClassBase(
#line  358 "cs.ATG" 
out names);

#line  358 "cs.ATG" 
				newType.BaseTypes = names; 
			}
			while (la.kind == 127) {
				TypeParameterConstraintsClause(
#line  361 "cs.ATG" 
templates);
			}

#line  363 "cs.ATG" 
			newType.BodyStartLocation = t.EndLocation; 
			Expect(16);
			ClassBody();
			Expect(17);
			if (la.kind == 11) {
				lexer.NextToken();
			}

#line  367 "cs.ATG" 
			newType.EndLocation = t.Location; 
			compilationUnit.BlockEnd();
			
		} else if (StartOf(9)) {

#line  370 "cs.ATG" 
			m.Check(Modifiers.StructsInterfacesEnumsDelegates); 
			if (la.kind == 109) {
				lexer.NextToken();

#line  371 "cs.ATG" 
				TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
				templates = newType.Templates;
				newType.StartLocation = m.GetDeclarationLocation(t.Location);
				compilationUnit.AddChild(newType);
				compilationUnit.BlockStart(newType);
				newType.Type = Types.Struct; 
				
				Identifier();

#line  378 "cs.ATG" 
				newType.Name = t.val; 
				if (la.kind == 23) {
					TypeParameterList(
#line  381 "cs.ATG" 
templates);
				}
				if (la.kind == 9) {
					StructInterfaces(
#line  383 "cs.ATG" 
out names);

#line  383 "cs.ATG" 
					newType.BaseTypes = names; 
				}
				while (la.kind == 127) {
					TypeParameterConstraintsClause(
#line  386 "cs.ATG" 
templates);
				}

#line  389 "cs.ATG" 
				newType.BodyStartLocation = t.EndLocation; 
				StructBody();
				if (la.kind == 11) {
					lexer.NextToken();
				}

#line  391 "cs.ATG" 
				newType.EndLocation = t.Location; 
				compilationUnit.BlockEnd();
				
			} else if (la.kind == 83) {
				lexer.NextToken();

#line  395 "cs.ATG" 
				TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
				templates = newType.Templates;
				compilationUnit.AddChild(newType);
				compilationUnit.BlockStart(newType);
				newType.StartLocation = m.GetDeclarationLocation(t.Location);
				newType.Type = Types.Interface;
				
				Identifier();

#line  402 "cs.ATG" 
				newType.Name = t.val; 
				if (la.kind == 23) {
					TypeParameterList(
#line  405 "cs.ATG" 
templates);
				}
				if (la.kind == 9) {
					InterfaceBase(
#line  407 "cs.ATG" 
out names);

#line  407 "cs.ATG" 
					newType.BaseTypes = names; 
				}
				while (la.kind == 127) {
					TypeParameterConstraintsClause(
#line  410 "cs.ATG" 
templates);
				}

#line  412 "cs.ATG" 
				newType.BodyStartLocation = t.EndLocation; 
				InterfaceBody();
				if (la.kind == 11) {
					lexer.NextToken();
				}

#line  414 "cs.ATG" 
				newType.EndLocation = t.Location; 
				compilationUnit.BlockEnd();
				
			} else if (la.kind == 68) {
				lexer.NextToken();

#line  418 "cs.ATG" 
				TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
				compilationUnit.AddChild(newType);
				compilationUnit.BlockStart(newType);
				newType.StartLocation = m.GetDeclarationLocation(t.Location);
				newType.Type = Types.Enum;
				
				Identifier();

#line  424 "cs.ATG" 
				newType.Name = t.val; 
				if (la.kind == 9) {
					lexer.NextToken();
					IntegralType(
#line  425 "cs.ATG" 
out name);

#line  425 "cs.ATG" 
					newType.BaseTypes.Add(new TypeReference(name)); 
				}

#line  427 "cs.ATG" 
				newType.BodyStartLocation = t.EndLocation; 
				EnumBody();
				if (la.kind == 11) {
					lexer.NextToken();
				}

#line  429 "cs.ATG" 
				newType.EndLocation = t.Location; 
				compilationUnit.BlockEnd();
				
			} else {
				lexer.NextToken();

#line  433 "cs.ATG" 
				DelegateDeclaration delegateDeclr = new DelegateDeclaration(m.Modifier, attributes);
				templates = delegateDeclr.Templates;
				delegateDeclr.StartLocation = m.GetDeclarationLocation(t.Location);
				
				if (
#line  437 "cs.ATG" 
NotVoidPointer()) {
					Expect(123);

#line  437 "cs.ATG" 
					delegateDeclr.ReturnType = new TypeReference("void", 0, null); 
				} else if (StartOf(10)) {
					Type(
#line  438 "cs.ATG" 
out type);

#line  438 "cs.ATG" 
					delegateDeclr.ReturnType = type; 
				} else SynErr(152);
				Identifier();

#line  440 "cs.ATG" 
				delegateDeclr.Name = t.val; 
				if (la.kind == 23) {
					TypeParameterList(
#line  443 "cs.ATG" 
templates);
				}
				Expect(20);
				if (StartOf(11)) {
					FormalParameterList(
#line  445 "cs.ATG" 
p);

#line  445 "cs.ATG" 
					delegateDeclr.Parameters = p; 
				}
				Expect(21);
				while (la.kind == 127) {
					TypeParameterConstraintsClause(
#line  449 "cs.ATG" 
templates);
				}
				Expect(11);

#line  451 "cs.ATG" 
				delegateDeclr.EndLocation = t.Location;
				compilationUnit.AddChild(delegateDeclr);
				
			}
		} else SynErr(153);
	}

	void TypeParameterList(
#line  2280 "cs.ATG" 
List<TemplateDefinition> templates) {

#line  2282 "cs.ATG" 
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		
		Expect(23);
		while (la.kind == 18) {
			AttributeSection(
#line  2286 "cs.ATG" 
out section);

#line  2286 "cs.ATG" 
			attributes.Add(section); 
		}
		Identifier();

#line  2287 "cs.ATG" 
		templates.Add(new TemplateDefinition(t.val, attributes)); 
		while (la.kind == 14) {
			lexer.NextToken();
			while (la.kind == 18) {
				AttributeSection(
#line  2288 "cs.ATG" 
out section);

#line  2288 "cs.ATG" 
				attributes.Add(section); 
			}
			Identifier();

#line  2289 "cs.ATG" 
			templates.Add(new TemplateDefinition(t.val, attributes)); 
		}
		Expect(22);
	}

	void ClassBase(
#line  466 "cs.ATG" 
out List<TypeReference> names) {

#line  468 "cs.ATG" 
		TypeReference typeRef;
		names = new List<TypeReference>();
		
		Expect(9);
		ClassType(
#line  472 "cs.ATG" 
out typeRef, false);

#line  472 "cs.ATG" 
		if (typeRef != null) { names.Add(typeRef); } 
		while (la.kind == 14) {
			lexer.NextToken();
			TypeName(
#line  473 "cs.ATG" 
out typeRef, false);

#line  473 "cs.ATG" 
			if (typeRef != null) { names.Add(typeRef); } 
		}
	}

	void TypeParameterConstraintsClause(
#line  2293 "cs.ATG" 
List<TemplateDefinition> templates) {

#line  2294 "cs.ATG" 
		string name = ""; TypeReference type; 
		Expect(127);
		Identifier();

#line  2297 "cs.ATG" 
		name = t.val; 
		Expect(9);
		TypeParameterConstraintsClauseBase(
#line  2299 "cs.ATG" 
out type);

#line  2300 "cs.ATG" 
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
#line  2309 "cs.ATG" 
out type);

#line  2310 "cs.ATG" 
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

#line  477 "cs.ATG" 
		AttributeSection section; 
		while (StartOf(12)) {

#line  479 "cs.ATG" 
			List<AttributeSection> attributes = new List<AttributeSection>();
			ModifierList m = new ModifierList();
			
			while (!(StartOf(13))) {SynErr(154); lexer.NextToken(); }
			while (la.kind == 18) {
				AttributeSection(
#line  483 "cs.ATG" 
out section);

#line  483 "cs.ATG" 
				attributes.Add(section); 
			}
			MemberModifiers(
#line  484 "cs.ATG" 
m);
			ClassMemberDecl(
#line  485 "cs.ATG" 
m, attributes);
		}
	}

	void StructInterfaces(
#line  489 "cs.ATG" 
out List<TypeReference> names) {

#line  491 "cs.ATG" 
		TypeReference typeRef;
		names = new List<TypeReference>();
		
		Expect(9);
		TypeName(
#line  495 "cs.ATG" 
out typeRef, false);

#line  495 "cs.ATG" 
		if (typeRef != null) { names.Add(typeRef); } 
		while (la.kind == 14) {
			lexer.NextToken();
			TypeName(
#line  496 "cs.ATG" 
out typeRef, false);

#line  496 "cs.ATG" 
			if (typeRef != null) { names.Add(typeRef); } 
		}
	}

	void StructBody() {

#line  500 "cs.ATG" 
		AttributeSection section; 
		Expect(16);
		while (StartOf(14)) {

#line  503 "cs.ATG" 
			List<AttributeSection> attributes = new List<AttributeSection>();
			ModifierList m = new ModifierList();
			
			while (la.kind == 18) {
				AttributeSection(
#line  506 "cs.ATG" 
out section);

#line  506 "cs.ATG" 
				attributes.Add(section); 
			}
			MemberModifiers(
#line  507 "cs.ATG" 
m);
			StructMemberDecl(
#line  508 "cs.ATG" 
m, attributes);
		}
		Expect(17);
	}

	void InterfaceBase(
#line  513 "cs.ATG" 
out List<TypeReference> names) {

#line  515 "cs.ATG" 
		TypeReference typeRef;
		names = new List<TypeReference>();
		
		Expect(9);
		TypeName(
#line  519 "cs.ATG" 
out typeRef, false);

#line  519 "cs.ATG" 
		if (typeRef != null) { names.Add(typeRef); } 
		while (la.kind == 14) {
			lexer.NextToken();
			TypeName(
#line  520 "cs.ATG" 
out typeRef, false);

#line  520 "cs.ATG" 
			if (typeRef != null) { names.Add(typeRef); } 
		}
	}

	void InterfaceBody() {
		Expect(16);
		while (StartOf(15)) {
			while (!(StartOf(16))) {SynErr(155); lexer.NextToken(); }
			InterfaceMemberDecl();
		}
		Expect(17);
	}

	void IntegralType(
#line  681 "cs.ATG" 
out string name) {

#line  681 "cs.ATG" 
		name = ""; 
		switch (la.kind) {
		case 102: {
			lexer.NextToken();

#line  683 "cs.ATG" 
			name = "sbyte"; 
			break;
		}
		case 54: {
			lexer.NextToken();

#line  684 "cs.ATG" 
			name = "byte"; 
			break;
		}
		case 104: {
			lexer.NextToken();

#line  685 "cs.ATG" 
			name = "short"; 
			break;
		}
		case 120: {
			lexer.NextToken();

#line  686 "cs.ATG" 
			name = "ushort"; 
			break;
		}
		case 82: {
			lexer.NextToken();

#line  687 "cs.ATG" 
			name = "int"; 
			break;
		}
		case 116: {
			lexer.NextToken();

#line  688 "cs.ATG" 
			name = "uint"; 
			break;
		}
		case 87: {
			lexer.NextToken();

#line  689 "cs.ATG" 
			name = "long"; 
			break;
		}
		case 117: {
			lexer.NextToken();

#line  690 "cs.ATG" 
			name = "ulong"; 
			break;
		}
		case 57: {
			lexer.NextToken();

#line  691 "cs.ATG" 
			name = "char"; 
			break;
		}
		default: SynErr(156); break;
		}
	}

	void EnumBody() {

#line  529 "cs.ATG" 
		FieldDeclaration f; 
		Expect(16);
		if (StartOf(17)) {
			EnumMemberDecl(
#line  532 "cs.ATG" 
out f);

#line  532 "cs.ATG" 
			compilationUnit.AddChild(f); 
			while (
#line  533 "cs.ATG" 
NotFinalComma()) {
				Expect(14);
				EnumMemberDecl(
#line  534 "cs.ATG" 
out f);

#line  534 "cs.ATG" 
				compilationUnit.AddChild(f); 
			}
			if (la.kind == 14) {
				lexer.NextToken();
			}
		}
		Expect(17);
	}

	void Type(
#line  540 "cs.ATG" 
out TypeReference type) {
		TypeWithRestriction(
#line  542 "cs.ATG" 
out type, true, false);
	}

	void FormalParameterList(
#line  603 "cs.ATG" 
List<ParameterDeclarationExpression> parameter) {

#line  606 "cs.ATG" 
		ParameterDeclarationExpression p;
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		
		while (la.kind == 18) {
			AttributeSection(
#line  611 "cs.ATG" 
out section);

#line  611 "cs.ATG" 
			attributes.Add(section); 
		}
		if (StartOf(18)) {
			FixedParameter(
#line  613 "cs.ATG" 
out p);

#line  613 "cs.ATG" 
			bool paramsFound = false;
			p.Attributes = attributes;
			parameter.Add(p);
			
			while (la.kind == 14) {
				lexer.NextToken();

#line  618 "cs.ATG" 
				attributes = new List<AttributeSection>(); if (paramsFound) Error("params array must be at end of parameter list"); 
				while (la.kind == 18) {
					AttributeSection(
#line  619 "cs.ATG" 
out section);

#line  619 "cs.ATG" 
					attributes.Add(section); 
				}
				if (StartOf(18)) {
					FixedParameter(
#line  621 "cs.ATG" 
out p);

#line  621 "cs.ATG" 
					p.Attributes = attributes; parameter.Add(p); 
				} else if (la.kind == 95) {
					ParameterArray(
#line  622 "cs.ATG" 
out p);

#line  622 "cs.ATG" 
					paramsFound = true; p.Attributes = attributes; parameter.Add(p); 
				} else SynErr(157);
			}
		} else if (la.kind == 95) {
			ParameterArray(
#line  625 "cs.ATG" 
out p);

#line  625 "cs.ATG" 
			p.Attributes = attributes; parameter.Add(p); 
		} else SynErr(158);
	}

	void ClassType(
#line  673 "cs.ATG" 
out TypeReference typeRef, bool canBeUnbound) {

#line  674 "cs.ATG" 
		TypeReference r; typeRef = null; 
		if (StartOf(19)) {
			TypeName(
#line  676 "cs.ATG" 
out r, canBeUnbound);

#line  676 "cs.ATG" 
			typeRef = r; 
		} else if (la.kind == 91) {
			lexer.NextToken();

#line  677 "cs.ATG" 
			typeRef = new TypeReference("object"); 
		} else if (la.kind == 108) {
			lexer.NextToken();

#line  678 "cs.ATG" 
			typeRef = new TypeReference("string"); 
		} else SynErr(159);
	}

	void TypeName(
#line  2223 "cs.ATG" 
out TypeReference typeRef, bool canBeUnbound) {

#line  2224 "cs.ATG" 
		List<TypeReference> typeArguments = null;
		string alias = null;
		string qualident;
		
		if (
#line  2229 "cs.ATG" 
IdentAndDoubleColon()) {
			Identifier();

#line  2230 "cs.ATG" 
			alias = t.val; 
			Expect(10);
		}
		Qualident(
#line  2233 "cs.ATG" 
out qualident);
		if (la.kind == 23) {
			TypeArgumentList(
#line  2234 "cs.ATG" 
out typeArguments, canBeUnbound);
		}

#line  2236 "cs.ATG" 
		if (alias == null) {
		typeRef = new TypeReference(qualident, typeArguments);
		} else if (alias == "global") {
			typeRef = new TypeReference(qualident, typeArguments);
			typeRef.IsGlobal = true;
		} else {
			typeRef = new TypeReference(alias + "." + qualident, typeArguments);
		}
		
		while (
#line  2245 "cs.ATG" 
DotAndIdent()) {
			Expect(15);

#line  2246 "cs.ATG" 
			typeArguments = null; 
			Qualident(
#line  2247 "cs.ATG" 
out qualident);
			if (la.kind == 23) {
				TypeArgumentList(
#line  2248 "cs.ATG" 
out typeArguments, canBeUnbound);
			}

#line  2249 "cs.ATG" 
			typeRef = new InnerClassTypeReference(typeRef, qualident, typeArguments); 
		}
	}

	void MemberModifiers(
#line  694 "cs.ATG" 
ModifierList m) {
		while (StartOf(20)) {
			switch (la.kind) {
			case 49: {
				lexer.NextToken();

#line  697 "cs.ATG" 
				m.Add(Modifiers.Abstract, t.Location); 
				break;
			}
			case 71: {
				lexer.NextToken();

#line  698 "cs.ATG" 
				m.Add(Modifiers.Extern, t.Location); 
				break;
			}
			case 84: {
				lexer.NextToken();

#line  699 "cs.ATG" 
				m.Add(Modifiers.Internal, t.Location); 
				break;
			}
			case 89: {
				lexer.NextToken();

#line  700 "cs.ATG" 
				m.Add(Modifiers.New, t.Location); 
				break;
			}
			case 94: {
				lexer.NextToken();

#line  701 "cs.ATG" 
				m.Add(Modifiers.Override, t.Location); 
				break;
			}
			case 96: {
				lexer.NextToken();

#line  702 "cs.ATG" 
				m.Add(Modifiers.Private, t.Location); 
				break;
			}
			case 97: {
				lexer.NextToken();

#line  703 "cs.ATG" 
				m.Add(Modifiers.Protected, t.Location); 
				break;
			}
			case 98: {
				lexer.NextToken();

#line  704 "cs.ATG" 
				m.Add(Modifiers.Public, t.Location); 
				break;
			}
			case 99: {
				lexer.NextToken();

#line  705 "cs.ATG" 
				m.Add(Modifiers.ReadOnly, t.Location); 
				break;
			}
			case 103: {
				lexer.NextToken();

#line  706 "cs.ATG" 
				m.Add(Modifiers.Sealed, t.Location); 
				break;
			}
			case 107: {
				lexer.NextToken();

#line  707 "cs.ATG" 
				m.Add(Modifiers.Static, t.Location); 
				break;
			}
			case 74: {
				lexer.NextToken();

#line  708 "cs.ATG" 
				m.Add(Modifiers.Fixed, t.Location); 
				break;
			}
			case 119: {
				lexer.NextToken();

#line  709 "cs.ATG" 
				m.Add(Modifiers.Unsafe, t.Location); 
				break;
			}
			case 122: {
				lexer.NextToken();

#line  710 "cs.ATG" 
				m.Add(Modifiers.Virtual, t.Location); 
				break;
			}
			case 124: {
				lexer.NextToken();

#line  711 "cs.ATG" 
				m.Add(Modifiers.Volatile, t.Location); 
				break;
			}
			case 126: {
				lexer.NextToken();

#line  712 "cs.ATG" 
				m.Add(Modifiers.Partial, t.Location); 
				break;
			}
			}
		}
	}

	void ClassMemberDecl(
#line  1021 "cs.ATG" 
ModifierList m, List<AttributeSection> attributes) {

#line  1022 "cs.ATG" 
		Statement stmt = null; 
		if (StartOf(21)) {
			StructMemberDecl(
#line  1024 "cs.ATG" 
m, attributes);
		} else if (la.kind == 27) {

#line  1025 "cs.ATG" 
			m.Check(Modifiers.Destructors); Location startPos = t.Location; 
			lexer.NextToken();
			Identifier();

#line  1026 "cs.ATG" 
			DestructorDeclaration d = new DestructorDeclaration(t.val, m.Modifier, attributes); 
			d.Modifier = m.Modifier;
			d.StartLocation = m.GetDeclarationLocation(startPos);
			
			Expect(20);
			Expect(21);

#line  1030 "cs.ATG" 
			d.EndLocation = t.EndLocation; 
			if (la.kind == 16) {
				Block(
#line  1030 "cs.ATG" 
out stmt);
			} else if (la.kind == 11) {
				lexer.NextToken();
			} else SynErr(160);

#line  1031 "cs.ATG" 
			d.Body = (BlockStatement)stmt;
			compilationUnit.AddChild(d);
			
		} else SynErr(161);
	}

	void StructMemberDecl(
#line  716 "cs.ATG" 
ModifierList m, List<AttributeSection> attributes) {

#line  718 "cs.ATG" 
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

#line  729 "cs.ATG" 
			m.Check(Modifiers.Constants); 
			lexer.NextToken();

#line  730 "cs.ATG" 
			Location startPos = t.Location; 
			Type(
#line  731 "cs.ATG" 
out type);
			Identifier();

#line  731 "cs.ATG" 
			FieldDeclaration fd = new FieldDeclaration(attributes, type, m.Modifier | Modifiers.Const);
			fd.StartLocation = m.GetDeclarationLocation(startPos);
			VariableDeclaration f = new VariableDeclaration(t.val);
			fd.Fields.Add(f);
			
			Expect(3);
			Expr(
#line  736 "cs.ATG" 
out expr);

#line  736 "cs.ATG" 
			f.Initializer = expr; 
			while (la.kind == 14) {
				lexer.NextToken();
				Identifier();

#line  737 "cs.ATG" 
				f = new VariableDeclaration(t.val);
				fd.Fields.Add(f);
				
				Expect(3);
				Expr(
#line  740 "cs.ATG" 
out expr);

#line  740 "cs.ATG" 
				f.Initializer = expr; 
			}
			Expect(11);

#line  741 "cs.ATG" 
			fd.EndLocation = t.EndLocation; compilationUnit.AddChild(fd); 
		} else if (
#line  745 "cs.ATG" 
NotVoidPointer()) {

#line  745 "cs.ATG" 
			m.Check(Modifiers.PropertysEventsMethods); 
			Expect(123);

#line  746 "cs.ATG" 
			Location startPos = t.Location; 
			if (
#line  747 "cs.ATG" 
IsExplicitInterfaceImplementation()) {
				TypeName(
#line  748 "cs.ATG" 
out explicitInterface, false);

#line  749 "cs.ATG" 
				if (la.kind != Tokens.Dot || Peek(1).kind != Tokens.This) {
				qualident = TypeReference.StripLastIdentifierFromType(ref explicitInterface);
				 } 
			} else if (StartOf(19)) {
				Identifier();

#line  752 "cs.ATG" 
				qualident = t.val; 
			} else SynErr(162);
			if (la.kind == 23) {
				TypeParameterList(
#line  755 "cs.ATG" 
templates);
			}
			Expect(20);
			if (la.kind == 111) {
				lexer.NextToken();

#line  758 "cs.ATG" 
				isExtensionMethod = true; /* C# 3.0 */ 
			}
			if (StartOf(11)) {
				FormalParameterList(
#line  759 "cs.ATG" 
p);
			}
			Expect(21);

#line  760 "cs.ATG" 
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
#line  778 "cs.ATG" 
templates);
			}
			if (la.kind == 16) {
				Block(
#line  780 "cs.ATG" 
out stmt);
			} else if (la.kind == 11) {
				lexer.NextToken();
			} else SynErr(163);

#line  780 "cs.ATG" 
			compilationUnit.BlockEnd();
			methodDeclaration.Body  = (BlockStatement)stmt;
			
		} else if (la.kind == 69) {

#line  784 "cs.ATG" 
			m.Check(Modifiers.PropertysEventsMethods); 
			lexer.NextToken();

#line  786 "cs.ATG" 
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
#line  796 "cs.ATG" 
out type);

#line  796 "cs.ATG" 
			eventDecl.TypeReference = type; 
			if (
#line  797 "cs.ATG" 
IsExplicitInterfaceImplementation()) {
				TypeName(
#line  798 "cs.ATG" 
out explicitInterface, false);

#line  799 "cs.ATG" 
				qualident = TypeReference.StripLastIdentifierFromType(ref explicitInterface); 

#line  800 "cs.ATG" 
				eventDecl.InterfaceImplementations.Add(new InterfaceImplementation(explicitInterface, qualident)); 
			} else if (StartOf(19)) {
				Identifier();

#line  802 "cs.ATG" 
				qualident = t.val; 
			} else SynErr(164);

#line  804 "cs.ATG" 
			eventDecl.Name = qualident; eventDecl.EndLocation = t.EndLocation; 
			if (la.kind == 3) {
				lexer.NextToken();
				Expr(
#line  805 "cs.ATG" 
out expr);

#line  805 "cs.ATG" 
				eventDecl.Initializer = expr; 
			}
			if (la.kind == 16) {
				lexer.NextToken();

#line  806 "cs.ATG" 
				eventDecl.BodyStart = t.Location; 
				EventAccessorDecls(
#line  807 "cs.ATG" 
out addBlock, out removeBlock);
				Expect(17);

#line  808 "cs.ATG" 
				eventDecl.BodyEnd   = t.EndLocation; 
			}
			if (la.kind == 11) {
				lexer.NextToken();
			}

#line  811 "cs.ATG" 
			compilationUnit.BlockEnd();
			eventDecl.AddRegion = addBlock;
			eventDecl.RemoveRegion = removeBlock;
			
		} else if (
#line  817 "cs.ATG" 
IdentAndLPar()) {

#line  817 "cs.ATG" 
			m.Check(Modifiers.Constructors | Modifiers.StaticConstructors); 
			Identifier();

#line  818 "cs.ATG" 
			string name = t.val; Location startPos = t.Location; 
			Expect(20);
			if (StartOf(11)) {

#line  818 "cs.ATG" 
				m.Check(Modifiers.Constructors); 
				FormalParameterList(
#line  819 "cs.ATG" 
p);
			}
			Expect(21);

#line  821 "cs.ATG" 
			ConstructorInitializer init = null;  
			if (la.kind == 9) {

#line  822 "cs.ATG" 
				m.Check(Modifiers.Constructors); 
				ConstructorInitializer(
#line  823 "cs.ATG" 
out init);
			}

#line  825 "cs.ATG" 
			ConstructorDeclaration cd = new ConstructorDeclaration(name, m.Modifier, p, init, attributes); 
			cd.StartLocation = startPos;
			cd.EndLocation   = t.EndLocation;
			
			if (la.kind == 16) {
				Block(
#line  830 "cs.ATG" 
out stmt);
			} else if (la.kind == 11) {
				lexer.NextToken();
			} else SynErr(165);

#line  830 "cs.ATG" 
			cd.Body = (BlockStatement)stmt; compilationUnit.AddChild(cd); 
		} else if (la.kind == 70 || la.kind == 80) {

#line  833 "cs.ATG" 
			m.Check(Modifiers.Operators);
			if (m.isNone) Error("at least one modifier must be set"); 
			bool isImplicit = true;
			Location startPos = Location.Empty;
			
			if (la.kind == 80) {
				lexer.NextToken();

#line  838 "cs.ATG" 
				startPos = t.Location; 
			} else {
				lexer.NextToken();

#line  838 "cs.ATG" 
				isImplicit = false; startPos = t.Location; 
			}
			Expect(92);
			Type(
#line  839 "cs.ATG" 
out type);

#line  839 "cs.ATG" 
			TypeReference operatorType = type; 
			Expect(20);
			Type(
#line  840 "cs.ATG" 
out type);
			Identifier();

#line  840 "cs.ATG" 
			string varName = t.val; 
			Expect(21);

#line  841 "cs.ATG" 
			Location endPos = t.Location; 
			if (la.kind == 16) {
				Block(
#line  842 "cs.ATG" 
out stmt);
			} else if (la.kind == 11) {
				lexer.NextToken();

#line  842 "cs.ATG" 
				stmt = null; 
			} else SynErr(166);

#line  845 "cs.ATG" 
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
			
		} else if (StartOf(22)) {
			TypeDecl(
#line  862 "cs.ATG" 
m, attributes);
		} else if (StartOf(10)) {
			Type(
#line  864 "cs.ATG" 
out type);

#line  864 "cs.ATG" 
			Location startPos = t.Location;  
			if (la.kind == 92) {

#line  866 "cs.ATG" 
				OverloadableOperatorType op;
				m.Check(Modifiers.Operators);
				if (m.isNone) Error("at least one modifier must be set");
				
				lexer.NextToken();
				OverloadableOperator(
#line  870 "cs.ATG" 
out op);

#line  870 "cs.ATG" 
				TypeReference firstType, secondType = null; string secondName = null; 
				Expect(20);
				Type(
#line  871 "cs.ATG" 
out firstType);
				Identifier();

#line  871 "cs.ATG" 
				string firstName = t.val; 
				if (la.kind == 14) {
					lexer.NextToken();
					Type(
#line  872 "cs.ATG" 
out secondType);
					Identifier();

#line  872 "cs.ATG" 
					secondName = t.val; 
				} else if (la.kind == 21) {
				} else SynErr(167);

#line  880 "cs.ATG" 
				Location endPos = t.Location; 
				Expect(21);
				if (la.kind == 16) {
					Block(
#line  881 "cs.ATG" 
out stmt);
				} else if (la.kind == 11) {
					lexer.NextToken();
				} else SynErr(168);

#line  883 "cs.ATG" 
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
#line  902 "cs.ATG" 
IsVarDecl()) {

#line  903 "cs.ATG" 
				m.Check(Modifiers.Fields);
				FieldDeclaration fd = new FieldDeclaration(attributes, type, m.Modifier);
				fd.StartLocation = m.GetDeclarationLocation(startPos); 
				
				if (
#line  907 "cs.ATG" 
m.Contains(Modifiers.Fixed)) {
					VariableDeclarator(
#line  908 "cs.ATG" 
variableDeclarators);
					Expect(18);
					Expr(
#line  910 "cs.ATG" 
out expr);

#line  910 "cs.ATG" 
					if (variableDeclarators.Count > 0)
					variableDeclarators[variableDeclarators.Count-1].FixedArrayInitialization = expr; 
					Expect(19);
					while (la.kind == 14) {
						lexer.NextToken();
						VariableDeclarator(
#line  914 "cs.ATG" 
variableDeclarators);
						Expect(18);
						Expr(
#line  916 "cs.ATG" 
out expr);

#line  916 "cs.ATG" 
						if (variableDeclarators.Count > 0)
						variableDeclarators[variableDeclarators.Count-1].FixedArrayInitialization = expr; 
						Expect(19);
					}
				} else if (StartOf(19)) {
					VariableDeclarator(
#line  921 "cs.ATG" 
variableDeclarators);
					while (la.kind == 14) {
						lexer.NextToken();
						VariableDeclarator(
#line  922 "cs.ATG" 
variableDeclarators);
					}
				} else SynErr(169);
				Expect(11);

#line  924 "cs.ATG" 
				fd.EndLocation = t.EndLocation; fd.Fields = variableDeclarators; compilationUnit.AddChild(fd); 
			} else if (la.kind == 111) {

#line  927 "cs.ATG" 
				m.Check(Modifiers.Indexers); 
				lexer.NextToken();
				Expect(18);
				FormalParameterList(
#line  928 "cs.ATG" 
p);
				Expect(19);

#line  928 "cs.ATG" 
				Location endLocation = t.EndLocation; 
				Expect(16);

#line  929 "cs.ATG" 
				IndexerDeclaration indexer = new IndexerDeclaration(type, p, m.Modifier, attributes);
				indexer.StartLocation = startPos;
				indexer.EndLocation   = endLocation;
				indexer.BodyStart     = t.Location;
				PropertyGetRegion getRegion;
				PropertySetRegion setRegion;
				
				AccessorDecls(
#line  936 "cs.ATG" 
out getRegion, out setRegion);
				Expect(17);

#line  937 "cs.ATG" 
				indexer.BodyEnd    = t.EndLocation;
				indexer.GetRegion = getRegion;
				indexer.SetRegion = setRegion;
				compilationUnit.AddChild(indexer);
				
			} else if (
#line  942 "cs.ATG" 
IsIdentifierToken(la)) {
				if (
#line  943 "cs.ATG" 
IsExplicitInterfaceImplementation()) {
					TypeName(
#line  944 "cs.ATG" 
out explicitInterface, false);

#line  945 "cs.ATG" 
					if (la.kind != Tokens.Dot || Peek(1).kind != Tokens.This) {
					qualident = TypeReference.StripLastIdentifierFromType(ref explicitInterface);
					 } 
				} else if (StartOf(19)) {
					Identifier();

#line  948 "cs.ATG" 
					qualident = t.val; 
				} else SynErr(170);

#line  950 "cs.ATG" 
				Location qualIdentEndLocation = t.EndLocation; 
				if (la.kind == 16 || la.kind == 20 || la.kind == 23) {
					if (la.kind == 20 || la.kind == 23) {

#line  954 "cs.ATG" 
						m.Check(Modifiers.PropertysEventsMethods); 
						if (la.kind == 23) {
							TypeParameterList(
#line  956 "cs.ATG" 
templates);
						}
						Expect(20);
						if (la.kind == 111) {
							lexer.NextToken();

#line  958 "cs.ATG" 
							isExtensionMethod = true; 
						}
						if (StartOf(11)) {
							FormalParameterList(
#line  959 "cs.ATG" 
p);
						}
						Expect(21);

#line  961 "cs.ATG" 
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
#line  976 "cs.ATG" 
templates);
						}
						if (la.kind == 16) {
							Block(
#line  977 "cs.ATG" 
out stmt);
						} else if (la.kind == 11) {
							lexer.NextToken();
						} else SynErr(171);

#line  977 "cs.ATG" 
						methodDeclaration.Body  = (BlockStatement)stmt; 
					} else {
						lexer.NextToken();

#line  980 "cs.ATG" 
						PropertyDeclaration pDecl = new PropertyDeclaration(qualident, type, m.Modifier, attributes); 
						if (explicitInterface != null)
						pDecl.InterfaceImplementations.Add(new InterfaceImplementation(explicitInterface, qualident));
						      pDecl.StartLocation = m.GetDeclarationLocation(startPos);
						      pDecl.EndLocation   = qualIdentEndLocation;
						      pDecl.BodyStart   = t.Location;
						      PropertyGetRegion getRegion;
						      PropertySetRegion setRegion;
						   
						AccessorDecls(
#line  989 "cs.ATG" 
out getRegion, out setRegion);
						Expect(17);

#line  991 "cs.ATG" 
						pDecl.GetRegion = getRegion;
						pDecl.SetRegion = setRegion;
						pDecl.BodyEnd = t.EndLocation;
						compilationUnit.AddChild(pDecl);
						
					}
				} else if (la.kind == 15) {

#line  999 "cs.ATG" 
					m.Check(Modifiers.Indexers); 
					lexer.NextToken();
					Expect(111);
					Expect(18);
					FormalParameterList(
#line  1000 "cs.ATG" 
p);
					Expect(19);

#line  1001 "cs.ATG" 
					IndexerDeclaration indexer = new IndexerDeclaration(type, p, m.Modifier, attributes);
					indexer.StartLocation = m.GetDeclarationLocation(startPos);
					indexer.EndLocation   = t.EndLocation;
					if (explicitInterface != null)
					indexer.InterfaceImplementations.Add(new InterfaceImplementation(explicitInterface, "this"));
					      PropertyGetRegion getRegion;
					      PropertySetRegion setRegion;
					    
					Expect(16);

#line  1009 "cs.ATG" 
					Location bodyStart = t.Location; 
					AccessorDecls(
#line  1010 "cs.ATG" 
out getRegion, out setRegion);
					Expect(17);

#line  1011 "cs.ATG" 
					indexer.BodyStart = bodyStart;
					indexer.BodyEnd   = t.EndLocation;
					indexer.GetRegion = getRegion;
					indexer.SetRegion = setRegion;
					compilationUnit.AddChild(indexer);
					
				} else SynErr(172);
			} else SynErr(173);
		} else SynErr(174);
	}

	void InterfaceMemberDecl() {

#line  1038 "cs.ATG" 
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
#line  1051 "cs.ATG" 
out section);

#line  1051 "cs.ATG" 
			attributes.Add(section); 
		}
		if (la.kind == 89) {
			lexer.NextToken();

#line  1052 "cs.ATG" 
			mod = Modifiers.New; startLocation = t.Location; 
		}
		if (
#line  1055 "cs.ATG" 
NotVoidPointer()) {
			Expect(123);

#line  1055 "cs.ATG" 
			if (startLocation.X == -1) startLocation = t.Location; 
			Identifier();

#line  1056 "cs.ATG" 
			name = t.val; 
			if (la.kind == 23) {
				TypeParameterList(
#line  1057 "cs.ATG" 
templates);
			}
			Expect(20);
			if (StartOf(11)) {
				FormalParameterList(
#line  1058 "cs.ATG" 
parameters);
			}
			Expect(21);
			while (la.kind == 127) {
				TypeParameterConstraintsClause(
#line  1059 "cs.ATG" 
templates);
			}
			Expect(11);

#line  1061 "cs.ATG" 
			MethodDeclaration md = new MethodDeclaration {
			Name = name, Modifier = mod, TypeReference = new TypeReference("void"), 
			Parameters = parameters, Attributes = attributes, Templates = templates,
			StartLocation = startLocation, EndLocation = t.EndLocation
			};
			compilationUnit.AddChild(md);
			
		} else if (StartOf(23)) {
			if (StartOf(10)) {
				Type(
#line  1069 "cs.ATG" 
out type);

#line  1069 "cs.ATG" 
				if (startLocation.X == -1) startLocation = t.Location; 
				if (StartOf(19)) {
					Identifier();

#line  1071 "cs.ATG" 
					name = t.val; Location qualIdentEndLocation = t.EndLocation; 
					if (la.kind == 20 || la.kind == 23) {
						if (la.kind == 23) {
							TypeParameterList(
#line  1075 "cs.ATG" 
templates);
						}
						Expect(20);
						if (StartOf(11)) {
							FormalParameterList(
#line  1076 "cs.ATG" 
parameters);
						}
						Expect(21);
						while (la.kind == 127) {
							TypeParameterConstraintsClause(
#line  1078 "cs.ATG" 
templates);
						}
						Expect(11);

#line  1079 "cs.ATG" 
						MethodDeclaration md = new MethodDeclaration {
						Name = name, Modifier = mod, TypeReference = type,
						Parameters = parameters, Attributes = attributes, Templates = templates,
						StartLocation = startLocation, EndLocation = t.EndLocation
						};
						compilationUnit.AddChild(md);
						
					} else if (la.kind == 16) {

#line  1087 "cs.ATG" 
						PropertyDeclaration pd = new PropertyDeclaration(name, type, mod, attributes); compilationUnit.AddChild(pd); 
						lexer.NextToken();

#line  1088 "cs.ATG" 
						Location bodyStart = t.Location;
						InterfaceAccessors(
#line  1088 "cs.ATG" 
out getBlock, out setBlock);
						Expect(17);

#line  1088 "cs.ATG" 
						pd.GetRegion = getBlock; pd.SetRegion = setBlock; pd.StartLocation = startLocation; pd.EndLocation = qualIdentEndLocation; pd.BodyStart = bodyStart; pd.BodyEnd = t.EndLocation; 
					} else SynErr(175);
				} else if (la.kind == 111) {
					lexer.NextToken();
					Expect(18);
					FormalParameterList(
#line  1091 "cs.ATG" 
parameters);
					Expect(19);

#line  1091 "cs.ATG" 
					Location bracketEndLocation = t.EndLocation; 

#line  1091 "cs.ATG" 
					IndexerDeclaration id = new IndexerDeclaration(type, parameters, mod, attributes); compilationUnit.AddChild(id); 
					Expect(16);

#line  1092 "cs.ATG" 
					Location bodyStart = t.Location;
					InterfaceAccessors(
#line  1092 "cs.ATG" 
out getBlock, out setBlock);
					Expect(17);

#line  1092 "cs.ATG" 
					id.GetRegion = getBlock; id.SetRegion = setBlock; id.StartLocation = startLocation;  id.EndLocation = bracketEndLocation; id.BodyStart = bodyStart; id.BodyEnd = t.EndLocation;
				} else SynErr(176);
			} else {
				lexer.NextToken();

#line  1095 "cs.ATG" 
				if (startLocation.X == -1) startLocation = t.Location; 
				Type(
#line  1096 "cs.ATG" 
out type);
				Identifier();

#line  1097 "cs.ATG" 
				EventDeclaration ed = new EventDeclaration {
				TypeReference = type, Name = t.val, Modifier = mod, Attributes = attributes
				};
				compilationUnit.AddChild(ed);
				
				Expect(11);

#line  1102 "cs.ATG" 
				ed.StartLocation = startLocation; ed.EndLocation = t.EndLocation; 
			}
		} else SynErr(177);
	}

	void EnumMemberDecl(
#line  1107 "cs.ATG" 
out FieldDeclaration f) {

#line  1109 "cs.ATG" 
		Expression expr = null;
		List<AttributeSection> attributes = new List<AttributeSection>();
		AttributeSection section = null;
		VariableDeclaration varDecl = null;
		
		while (la.kind == 18) {
			AttributeSection(
#line  1115 "cs.ATG" 
out section);

#line  1115 "cs.ATG" 
			attributes.Add(section); 
		}
		Identifier();

#line  1116 "cs.ATG" 
		f = new FieldDeclaration(attributes);
		varDecl         = new VariableDeclaration(t.val);
		f.Fields.Add(varDecl);
		f.StartLocation = t.Location;
		
		if (la.kind == 3) {
			lexer.NextToken();
			Expr(
#line  1121 "cs.ATG" 
out expr);

#line  1121 "cs.ATG" 
			varDecl.Initializer = expr; 
		}
	}

	void TypeWithRestriction(
#line  545 "cs.ATG" 
out TypeReference type, bool allowNullable, bool canBeUnbound) {

#line  547 "cs.ATG" 
		string name;
		int pointer = 0;
		type = null;
		
		if (StartOf(4)) {
			ClassType(
#line  552 "cs.ATG" 
out type, canBeUnbound);
		} else if (StartOf(5)) {
			SimpleType(
#line  553 "cs.ATG" 
out name);

#line  553 "cs.ATG" 
			type = new TypeReference(name); 
		} else if (la.kind == 123) {
			lexer.NextToken();
			Expect(6);

#line  554 "cs.ATG" 
			pointer = 1; type = new TypeReference("void"); 
		} else SynErr(178);

#line  555 "cs.ATG" 
		List<int> r = new List<int>(); 
		if (
#line  557 "cs.ATG" 
allowNullable && la.kind == Tokens.Question) {
			NullableQuestionMark(
#line  557 "cs.ATG" 
ref type);
		}
		while (
#line  559 "cs.ATG" 
IsPointerOrDims()) {

#line  559 "cs.ATG" 
			int i = 0; 
			if (la.kind == 6) {
				lexer.NextToken();

#line  560 "cs.ATG" 
				++pointer; 
			} else if (la.kind == 18) {
				lexer.NextToken();
				while (la.kind == 14) {
					lexer.NextToken();

#line  561 "cs.ATG" 
					++i; 
				}
				Expect(19);

#line  561 "cs.ATG" 
				r.Add(i); 
			} else SynErr(179);
		}

#line  564 "cs.ATG" 
		if (type != null) {
		type.RankSpecifier = r.ToArray();
		type.PointerNestingLevel = pointer;
		  }
		
	}

	void SimpleType(
#line  592 "cs.ATG" 
out string name) {

#line  593 "cs.ATG" 
		name = String.Empty; 
		if (StartOf(24)) {
			IntegralType(
#line  595 "cs.ATG" 
out name);
		} else if (la.kind == 75) {
			lexer.NextToken();

#line  596 "cs.ATG" 
			name = "float"; 
		} else if (la.kind == 66) {
			lexer.NextToken();

#line  597 "cs.ATG" 
			name = "double"; 
		} else if (la.kind == 62) {
			lexer.NextToken();

#line  598 "cs.ATG" 
			name = "decimal"; 
		} else if (la.kind == 52) {
			lexer.NextToken();

#line  599 "cs.ATG" 
			name = "bool"; 
		} else SynErr(180);
	}

	void NullableQuestionMark(
#line  2254 "cs.ATG" 
ref TypeReference typeRef) {

#line  2255 "cs.ATG" 
		List<TypeReference> typeArguments = new List<TypeReference>(1); 
		Expect(12);

#line  2259 "cs.ATG" 
		if (typeRef != null) typeArguments.Add(typeRef);
		typeRef = new TypeReference("System.Nullable", typeArguments);
		
	}

	void FixedParameter(
#line  629 "cs.ATG" 
out ParameterDeclarationExpression p) {

#line  631 "cs.ATG" 
		TypeReference type;
		ParameterModifiers mod = ParameterModifiers.In;
		Location start = t.Location;
		
		if (la.kind == 93 || la.kind == 100) {
			if (la.kind == 100) {
				lexer.NextToken();

#line  637 "cs.ATG" 
				mod = ParameterModifiers.Ref; 
			} else {
				lexer.NextToken();

#line  638 "cs.ATG" 
				mod = ParameterModifiers.Out; 
			}
		}
		Type(
#line  640 "cs.ATG" 
out type);
		Identifier();

#line  640 "cs.ATG" 
		p = new ParameterDeclarationExpression(type, t.val, mod); p.StartLocation = start; p.EndLocation = t.Location; 
	}

	void ParameterArray(
#line  643 "cs.ATG" 
out ParameterDeclarationExpression p) {

#line  644 "cs.ATG" 
		TypeReference type; 
		Expect(95);
		Type(
#line  646 "cs.ATG" 
out type);
		Identifier();

#line  646 "cs.ATG" 
		p = new ParameterDeclarationExpression(type, t.val, ParameterModifiers.Params); 
	}

	void AccessorModifiers(
#line  649 "cs.ATG" 
out ModifierList m) {

#line  650 "cs.ATG" 
		m = new ModifierList(); 
		if (la.kind == 96) {
			lexer.NextToken();

#line  652 "cs.ATG" 
			m.Add(Modifiers.Private, t.Location); 
		} else if (la.kind == 97) {
			lexer.NextToken();

#line  653 "cs.ATG" 
			m.Add(Modifiers.Protected, t.Location); 
			if (la.kind == 84) {
				lexer.NextToken();

#line  654 "cs.ATG" 
				m.Add(Modifiers.Internal, t.Location); 
			}
		} else if (la.kind == 84) {
			lexer.NextToken();

#line  655 "cs.ATG" 
			m.Add(Modifiers.Internal, t.Location); 
			if (la.kind == 97) {
				lexer.NextToken();

#line  656 "cs.ATG" 
				m.Add(Modifiers.Protected, t.Location); 
			}
		} else SynErr(181);
	}

	void Block(
#line  1240 "cs.ATG" 
out Statement stmt) {
		Expect(16);

#line  1242 "cs.ATG" 
		BlockStatement blockStmt = new BlockStatement();
		blockStmt.StartLocation = t.Location;
		compilationUnit.BlockStart(blockStmt);
		if (!ParseMethodBodies) lexer.SkipCurrentBlock(0);
		
		while (StartOf(25)) {
			Statement();
		}
		Expect(17);

#line  1249 "cs.ATG" 
		stmt = blockStmt;
		blockStmt.EndLocation = t.EndLocation;
		compilationUnit.BlockEnd();
		
	}

	void EventAccessorDecls(
#line  1178 "cs.ATG" 
out EventAddRegion addBlock, out EventRemoveRegion removeBlock) {

#line  1179 "cs.ATG" 
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		Statement stmt;
		addBlock = null;
		removeBlock = null;
		
		while (la.kind == 18) {
			AttributeSection(
#line  1186 "cs.ATG" 
out section);

#line  1186 "cs.ATG" 
			attributes.Add(section); 
		}
		if (la.kind == 130) {

#line  1188 "cs.ATG" 
			addBlock = new EventAddRegion(attributes); 
			AddAccessorDecl(
#line  1189 "cs.ATG" 
out stmt);

#line  1189 "cs.ATG" 
			attributes = new List<AttributeSection>(); addBlock.Block = (BlockStatement)stmt; 
			while (la.kind == 18) {
				AttributeSection(
#line  1190 "cs.ATG" 
out section);

#line  1190 "cs.ATG" 
				attributes.Add(section); 
			}
			RemoveAccessorDecl(
#line  1191 "cs.ATG" 
out stmt);

#line  1191 "cs.ATG" 
			removeBlock = new EventRemoveRegion(attributes); removeBlock.Block = (BlockStatement)stmt; 
		} else if (la.kind == 131) {
			RemoveAccessorDecl(
#line  1193 "cs.ATG" 
out stmt);

#line  1193 "cs.ATG" 
			removeBlock = new EventRemoveRegion(attributes); removeBlock.Block = (BlockStatement)stmt; attributes = new List<AttributeSection>(); 
			while (la.kind == 18) {
				AttributeSection(
#line  1194 "cs.ATG" 
out section);

#line  1194 "cs.ATG" 
				attributes.Add(section); 
			}
			AddAccessorDecl(
#line  1195 "cs.ATG" 
out stmt);

#line  1195 "cs.ATG" 
			addBlock = new EventAddRegion(attributes); addBlock.Block = (BlockStatement)stmt; 
		} else SynErr(182);
	}

	void ConstructorInitializer(
#line  1269 "cs.ATG" 
out ConstructorInitializer ci) {

#line  1270 "cs.ATG" 
		Expression expr; ci = new ConstructorInitializer(); 
		Expect(9);
		if (la.kind == 51) {
			lexer.NextToken();

#line  1274 "cs.ATG" 
			ci.ConstructorInitializerType = ConstructorInitializerType.Base; 
		} else if (la.kind == 111) {
			lexer.NextToken();

#line  1275 "cs.ATG" 
			ci.ConstructorInitializerType = ConstructorInitializerType.This; 
		} else SynErr(183);
		Expect(20);
		if (StartOf(26)) {
			Argument(
#line  1278 "cs.ATG" 
out expr);

#line  1278 "cs.ATG" 
			if (expr != null) { ci.Arguments.Add(expr); } 
			while (la.kind == 14) {
				lexer.NextToken();
				Argument(
#line  1278 "cs.ATG" 
out expr);

#line  1278 "cs.ATG" 
				if (expr != null) { ci.Arguments.Add(expr); } 
			}
		}
		Expect(21);
	}

	void OverloadableOperator(
#line  1290 "cs.ATG" 
out OverloadableOperatorType op) {

#line  1291 "cs.ATG" 
		op = OverloadableOperatorType.None; 
		switch (la.kind) {
		case 4: {
			lexer.NextToken();

#line  1293 "cs.ATG" 
			op = OverloadableOperatorType.Add; 
			break;
		}
		case 5: {
			lexer.NextToken();

#line  1294 "cs.ATG" 
			op = OverloadableOperatorType.Subtract; 
			break;
		}
		case 24: {
			lexer.NextToken();

#line  1296 "cs.ATG" 
			op = OverloadableOperatorType.Not; 
			break;
		}
		case 27: {
			lexer.NextToken();

#line  1297 "cs.ATG" 
			op = OverloadableOperatorType.BitNot; 
			break;
		}
		case 31: {
			lexer.NextToken();

#line  1299 "cs.ATG" 
			op = OverloadableOperatorType.Increment; 
			break;
		}
		case 32: {
			lexer.NextToken();

#line  1300 "cs.ATG" 
			op = OverloadableOperatorType.Decrement; 
			break;
		}
		case 113: {
			lexer.NextToken();

#line  1302 "cs.ATG" 
			op = OverloadableOperatorType.IsTrue; 
			break;
		}
		case 72: {
			lexer.NextToken();

#line  1303 "cs.ATG" 
			op = OverloadableOperatorType.IsFalse; 
			break;
		}
		case 6: {
			lexer.NextToken();

#line  1305 "cs.ATG" 
			op = OverloadableOperatorType.Multiply; 
			break;
		}
		case 7: {
			lexer.NextToken();

#line  1306 "cs.ATG" 
			op = OverloadableOperatorType.Divide; 
			break;
		}
		case 8: {
			lexer.NextToken();

#line  1307 "cs.ATG" 
			op = OverloadableOperatorType.Modulus; 
			break;
		}
		case 28: {
			lexer.NextToken();

#line  1309 "cs.ATG" 
			op = OverloadableOperatorType.BitwiseAnd; 
			break;
		}
		case 29: {
			lexer.NextToken();

#line  1310 "cs.ATG" 
			op = OverloadableOperatorType.BitwiseOr; 
			break;
		}
		case 30: {
			lexer.NextToken();

#line  1311 "cs.ATG" 
			op = OverloadableOperatorType.ExclusiveOr; 
			break;
		}
		case 37: {
			lexer.NextToken();

#line  1313 "cs.ATG" 
			op = OverloadableOperatorType.ShiftLeft; 
			break;
		}
		case 33: {
			lexer.NextToken();

#line  1314 "cs.ATG" 
			op = OverloadableOperatorType.Equality; 
			break;
		}
		case 34: {
			lexer.NextToken();

#line  1315 "cs.ATG" 
			op = OverloadableOperatorType.InEquality; 
			break;
		}
		case 23: {
			lexer.NextToken();

#line  1316 "cs.ATG" 
			op = OverloadableOperatorType.LessThan; 
			break;
		}
		case 35: {
			lexer.NextToken();

#line  1317 "cs.ATG" 
			op = OverloadableOperatorType.GreaterThanOrEqual; 
			break;
		}
		case 36: {
			lexer.NextToken();

#line  1318 "cs.ATG" 
			op = OverloadableOperatorType.LessThanOrEqual; 
			break;
		}
		case 22: {
			lexer.NextToken();

#line  1319 "cs.ATG" 
			op = OverloadableOperatorType.GreaterThan; 
			if (la.kind == 22) {
				lexer.NextToken();

#line  1319 "cs.ATG" 
				op = OverloadableOperatorType.ShiftRight; 
			}
			break;
		}
		default: SynErr(184); break;
		}
	}

	void VariableDeclarator(
#line  1233 "cs.ATG" 
List<VariableDeclaration> fieldDeclaration) {

#line  1234 "cs.ATG" 
		Expression expr = null; 
		Identifier();

#line  1236 "cs.ATG" 
		VariableDeclaration f = new VariableDeclaration(t.val); 
		if (la.kind == 3) {
			lexer.NextToken();
			VariableInitializer(
#line  1237 "cs.ATG" 
out expr);

#line  1237 "cs.ATG" 
			f.Initializer = expr; 
		}

#line  1237 "cs.ATG" 
		fieldDeclaration.Add(f); 
	}

	void AccessorDecls(
#line  1125 "cs.ATG" 
out PropertyGetRegion getBlock, out PropertySetRegion setBlock) {

#line  1127 "cs.ATG" 
		List<AttributeSection> attributes = new List<AttributeSection>(); 
		AttributeSection section;
		getBlock = null;
		setBlock = null; 
		ModifierList modifiers = null;
		
		while (la.kind == 18) {
			AttributeSection(
#line  1134 "cs.ATG" 
out section);

#line  1134 "cs.ATG" 
			attributes.Add(section); 
		}
		if (la.kind == 84 || la.kind == 96 || la.kind == 97) {
			AccessorModifiers(
#line  1135 "cs.ATG" 
out modifiers);
		}
		if (la.kind == 128) {
			GetAccessorDecl(
#line  1137 "cs.ATG" 
out getBlock, attributes);

#line  1138 "cs.ATG" 
			if (modifiers != null) {getBlock.Modifier = modifiers.Modifier; } 
			if (StartOf(27)) {

#line  1139 "cs.ATG" 
				attributes = new List<AttributeSection>(); modifiers = null; 
				while (la.kind == 18) {
					AttributeSection(
#line  1140 "cs.ATG" 
out section);

#line  1140 "cs.ATG" 
					attributes.Add(section); 
				}
				if (la.kind == 84 || la.kind == 96 || la.kind == 97) {
					AccessorModifiers(
#line  1141 "cs.ATG" 
out modifiers);
				}
				SetAccessorDecl(
#line  1142 "cs.ATG" 
out setBlock, attributes);

#line  1143 "cs.ATG" 
				if (modifiers != null) {setBlock.Modifier = modifiers.Modifier; } 
			}
		} else if (la.kind == 129) {
			SetAccessorDecl(
#line  1146 "cs.ATG" 
out setBlock, attributes);

#line  1147 "cs.ATG" 
			if (modifiers != null) {setBlock.Modifier = modifiers.Modifier; } 
			if (StartOf(28)) {

#line  1148 "cs.ATG" 
				attributes = new List<AttributeSection>(); modifiers = null; 
				while (la.kind == 18) {
					AttributeSection(
#line  1149 "cs.ATG" 
out section);

#line  1149 "cs.ATG" 
					attributes.Add(section); 
				}
				if (la.kind == 84 || la.kind == 96 || la.kind == 97) {
					AccessorModifiers(
#line  1150 "cs.ATG" 
out modifiers);
				}
				GetAccessorDecl(
#line  1151 "cs.ATG" 
out getBlock, attributes);

#line  1152 "cs.ATG" 
				if (modifiers != null) {getBlock.Modifier = modifiers.Modifier; } 
			}
		} else if (StartOf(19)) {
			Identifier();

#line  1154 "cs.ATG" 
			Error("get or set accessor declaration expected"); 
		} else SynErr(185);
	}

	void InterfaceAccessors(
#line  1199 "cs.ATG" 
out PropertyGetRegion getBlock, out PropertySetRegion setBlock) {

#line  1201 "cs.ATG" 
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		getBlock = null; setBlock = null;
		PropertyGetSetRegion lastBlock = null;
		
		while (la.kind == 18) {
			AttributeSection(
#line  1207 "cs.ATG" 
out section);

#line  1207 "cs.ATG" 
			attributes.Add(section); 
		}

#line  1208 "cs.ATG" 
		Location startLocation = la.Location; 
		if (la.kind == 128) {
			lexer.NextToken();

#line  1210 "cs.ATG" 
			getBlock = new PropertyGetRegion(null, attributes); 
		} else if (la.kind == 129) {
			lexer.NextToken();

#line  1211 "cs.ATG" 
			setBlock = new PropertySetRegion(null, attributes); 
		} else SynErr(186);
		Expect(11);

#line  1214 "cs.ATG" 
		if (getBlock != null) { getBlock.StartLocation = startLocation; getBlock.EndLocation = t.EndLocation; }
		if (setBlock != null) { setBlock.StartLocation = startLocation; setBlock.EndLocation = t.EndLocation; }
		attributes = new List<AttributeSection>(); 
		if (la.kind == 18 || la.kind == 128 || la.kind == 129) {
			while (la.kind == 18) {
				AttributeSection(
#line  1218 "cs.ATG" 
out section);

#line  1218 "cs.ATG" 
				attributes.Add(section); 
			}

#line  1219 "cs.ATG" 
			startLocation = la.Location; 
			if (la.kind == 128) {
				lexer.NextToken();

#line  1221 "cs.ATG" 
				if (getBlock != null) Error("get already declared");
				                 else { getBlock = new PropertyGetRegion(null, attributes); lastBlock = getBlock; }
				              
			} else if (la.kind == 129) {
				lexer.NextToken();

#line  1224 "cs.ATG" 
				if (setBlock != null) Error("set already declared");
				                 else { setBlock = new PropertySetRegion(null, attributes); lastBlock = setBlock; }
				              
			} else SynErr(187);
			Expect(11);

#line  1229 "cs.ATG" 
			if (lastBlock != null) { lastBlock.StartLocation = startLocation; lastBlock.EndLocation = t.EndLocation; } 
		}
	}

	void GetAccessorDecl(
#line  1158 "cs.ATG" 
out PropertyGetRegion getBlock, List<AttributeSection> attributes) {

#line  1159 "cs.ATG" 
		Statement stmt = null; 
		Expect(128);

#line  1162 "cs.ATG" 
		Location startLocation = t.Location; 
		if (la.kind == 16) {
			Block(
#line  1163 "cs.ATG" 
out stmt);
		} else if (la.kind == 11) {
			lexer.NextToken();
		} else SynErr(188);

#line  1164 "cs.ATG" 
		getBlock = new PropertyGetRegion((BlockStatement)stmt, attributes); 

#line  1165 "cs.ATG" 
		getBlock.StartLocation = startLocation; getBlock.EndLocation = t.EndLocation; 
	}

	void SetAccessorDecl(
#line  1168 "cs.ATG" 
out PropertySetRegion setBlock, List<AttributeSection> attributes) {

#line  1169 "cs.ATG" 
		Statement stmt = null; 
		Expect(129);

#line  1172 "cs.ATG" 
		Location startLocation = t.Location; 
		if (la.kind == 16) {
			Block(
#line  1173 "cs.ATG" 
out stmt);
		} else if (la.kind == 11) {
			lexer.NextToken();
		} else SynErr(189);

#line  1174 "cs.ATG" 
		setBlock = new PropertySetRegion((BlockStatement)stmt, attributes); 

#line  1175 "cs.ATG" 
		setBlock.StartLocation = startLocation; setBlock.EndLocation = t.EndLocation; 
	}

	void AddAccessorDecl(
#line  1255 "cs.ATG" 
out Statement stmt) {

#line  1256 "cs.ATG" 
		stmt = null;
		Expect(130);
		Block(
#line  1259 "cs.ATG" 
out stmt);
	}

	void RemoveAccessorDecl(
#line  1262 "cs.ATG" 
out Statement stmt) {

#line  1263 "cs.ATG" 
		stmt = null;
		Expect(131);
		Block(
#line  1266 "cs.ATG" 
out stmt);
	}

	void VariableInitializer(
#line  1282 "cs.ATG" 
out Expression initializerExpression) {

#line  1283 "cs.ATG" 
		TypeReference type = null; Expression expr = null; initializerExpression = null; 
		if (StartOf(6)) {
			Expr(
#line  1285 "cs.ATG" 
out initializerExpression);
		} else if (la.kind == 16) {
			CollectionInitializer(
#line  1286 "cs.ATG" 
out initializerExpression);
		} else if (la.kind == 106) {
			lexer.NextToken();
			Type(
#line  1287 "cs.ATG" 
out type);
			Expect(18);
			Expr(
#line  1287 "cs.ATG" 
out expr);
			Expect(19);

#line  1287 "cs.ATG" 
			initializerExpression = new StackAllocExpression(type, expr); 
		} else SynErr(190);
	}

	void Statement() {

#line  1428 "cs.ATG" 
		TypeReference type;
		Expression expr;
		Statement stmt = null;
		Location startPos = la.Location;
		
		while (!(StartOf(29))) {SynErr(191); lexer.NextToken(); }
		if (
#line  1437 "cs.ATG" 
IsLabel()) {
			Identifier();

#line  1437 "cs.ATG" 
			compilationUnit.AddChild(new LabelStatement(t.val)); 
			Expect(9);
			Statement();
		} else if (la.kind == 60) {
			lexer.NextToken();
			Type(
#line  1440 "cs.ATG" 
out type);

#line  1440 "cs.ATG" 
			LocalVariableDeclaration var = new LocalVariableDeclaration(type, Modifiers.Const); string ident = null; var.StartLocation = t.Location; 
			Identifier();

#line  1441 "cs.ATG" 
			ident = t.val; 
			Expect(3);
			Expr(
#line  1442 "cs.ATG" 
out expr);

#line  1442 "cs.ATG" 
			var.Variables.Add(new VariableDeclaration(ident, expr)); 
			while (la.kind == 14) {
				lexer.NextToken();
				Identifier();

#line  1443 "cs.ATG" 
				ident = t.val; 
				Expect(3);
				Expr(
#line  1443 "cs.ATG" 
out expr);

#line  1443 "cs.ATG" 
				var.Variables.Add(new VariableDeclaration(ident, expr)); 
			}
			Expect(11);

#line  1444 "cs.ATG" 
			compilationUnit.AddChild(var); 
		} else if (
#line  1447 "cs.ATG" 
IsLocalVarDecl()) {
			LocalVariableDecl(
#line  1447 "cs.ATG" 
out stmt);
			Expect(11);

#line  1447 "cs.ATG" 
			compilationUnit.AddChild(stmt); 
		} else if (StartOf(30)) {
			EmbeddedStatement(
#line  1449 "cs.ATG" 
out stmt);

#line  1449 "cs.ATG" 
			compilationUnit.AddChild(stmt); 
		} else SynErr(192);

#line  1455 "cs.ATG" 
		if (stmt != null) {
		stmt.StartLocation = startPos;
		stmt.EndLocation = t.EndLocation;
		}
		
	}

	void Argument(
#line  1322 "cs.ATG" 
out Expression argumentexpr) {

#line  1324 "cs.ATG" 
		Expression expr;
		FieldDirection fd = FieldDirection.None;
		
		if (la.kind == 93 || la.kind == 100) {
			if (la.kind == 100) {
				lexer.NextToken();

#line  1329 "cs.ATG" 
				fd = FieldDirection.Ref; 
			} else {
				lexer.NextToken();

#line  1330 "cs.ATG" 
				fd = FieldDirection.Out; 
			}
		}
		Expr(
#line  1332 "cs.ATG" 
out expr);

#line  1333 "cs.ATG" 
		argumentexpr = fd != FieldDirection.None ? argumentexpr = new DirectionExpression(fd, expr) : expr; 
	}

	void CollectionInitializer(
#line  1353 "cs.ATG" 
out Expression outExpr) {

#line  1355 "cs.ATG" 
		Expression expr = null;
		CollectionInitializerExpression initializer = new CollectionInitializerExpression();
		
		Expect(16);

#line  1359 "cs.ATG" 
		initializer.StartLocation = t.Location; 
		if (StartOf(31)) {
			VariableInitializer(
#line  1360 "cs.ATG" 
out expr);

#line  1361 "cs.ATG" 
			if (expr != null) { initializer.CreateExpressions.Add(expr); } 
			while (
#line  1362 "cs.ATG" 
NotFinalComma()) {
				Expect(14);
				VariableInitializer(
#line  1363 "cs.ATG" 
out expr);

#line  1364 "cs.ATG" 
				if (expr != null) { initializer.CreateExpressions.Add(expr); } 
			}
			if (la.kind == 14) {
				lexer.NextToken();
			}
		}
		Expect(17);

#line  1368 "cs.ATG" 
		initializer.EndLocation = t.Location; outExpr = initializer; 
	}

	void AssignmentOperator(
#line  1336 "cs.ATG" 
out AssignmentOperatorType op) {

#line  1337 "cs.ATG" 
		op = AssignmentOperatorType.None; 
		if (la.kind == 3) {
			lexer.NextToken();

#line  1339 "cs.ATG" 
			op = AssignmentOperatorType.Assign; 
		} else if (la.kind == 38) {
			lexer.NextToken();

#line  1340 "cs.ATG" 
			op = AssignmentOperatorType.Add; 
		} else if (la.kind == 39) {
			lexer.NextToken();

#line  1341 "cs.ATG" 
			op = AssignmentOperatorType.Subtract; 
		} else if (la.kind == 40) {
			lexer.NextToken();

#line  1342 "cs.ATG" 
			op = AssignmentOperatorType.Multiply; 
		} else if (la.kind == 41) {
			lexer.NextToken();

#line  1343 "cs.ATG" 
			op = AssignmentOperatorType.Divide; 
		} else if (la.kind == 42) {
			lexer.NextToken();

#line  1344 "cs.ATG" 
			op = AssignmentOperatorType.Modulus; 
		} else if (la.kind == 43) {
			lexer.NextToken();

#line  1345 "cs.ATG" 
			op = AssignmentOperatorType.BitwiseAnd; 
		} else if (la.kind == 44) {
			lexer.NextToken();

#line  1346 "cs.ATG" 
			op = AssignmentOperatorType.BitwiseOr; 
		} else if (la.kind == 45) {
			lexer.NextToken();

#line  1347 "cs.ATG" 
			op = AssignmentOperatorType.ExclusiveOr; 
		} else if (la.kind == 46) {
			lexer.NextToken();

#line  1348 "cs.ATG" 
			op = AssignmentOperatorType.ShiftLeft; 
		} else if (
#line  1349 "cs.ATG" 
la.kind == Tokens.GreaterThan && Peek(1).kind == Tokens.GreaterEqual) {
			Expect(22);
			Expect(35);

#line  1350 "cs.ATG" 
			op = AssignmentOperatorType.ShiftRight; 
		} else SynErr(193);
	}

	void CollectionOrObjectInitializer(
#line  1371 "cs.ATG" 
out Expression outExpr) {

#line  1373 "cs.ATG" 
		Expression expr = null;
		CollectionInitializerExpression initializer = new CollectionInitializerExpression();
		
		Expect(16);

#line  1377 "cs.ATG" 
		initializer.StartLocation = t.Location; 
		if (StartOf(31)) {
			ObjectPropertyInitializerOrVariableInitializer(
#line  1378 "cs.ATG" 
out expr);

#line  1379 "cs.ATG" 
			if (expr != null) { initializer.CreateExpressions.Add(expr); } 
			while (
#line  1380 "cs.ATG" 
NotFinalComma()) {
				Expect(14);
				ObjectPropertyInitializerOrVariableInitializer(
#line  1381 "cs.ATG" 
out expr);

#line  1382 "cs.ATG" 
				if (expr != null) { initializer.CreateExpressions.Add(expr); } 
			}
			if (la.kind == 14) {
				lexer.NextToken();
			}
		}
		Expect(17);

#line  1386 "cs.ATG" 
		initializer.EndLocation = t.Location; outExpr = initializer; 
	}

	void ObjectPropertyInitializerOrVariableInitializer(
#line  1389 "cs.ATG" 
out Expression expr) {

#line  1390 "cs.ATG" 
		expr = null; 
		if (
#line  1392 "cs.ATG" 
IdentAndAsgn()) {
			Identifier();

#line  1394 "cs.ATG" 
			NamedArgumentExpression nae = new NamedArgumentExpression(t.val, null);
			nae.StartLocation = t.Location;
			Expression r = null; 
			Expect(3);
			if (la.kind == 16) {
				CollectionOrObjectInitializer(
#line  1398 "cs.ATG" 
out r);
			} else if (StartOf(31)) {
				VariableInitializer(
#line  1399 "cs.ATG" 
out r);
			} else SynErr(194);

#line  1400 "cs.ATG" 
			nae.Expression = r; nae.EndLocation = t.EndLocation; expr = nae; 
		} else if (StartOf(31)) {
			VariableInitializer(
#line  1402 "cs.ATG" 
out expr);
		} else SynErr(195);
	}

	void LocalVariableDecl(
#line  1406 "cs.ATG" 
out Statement stmt) {

#line  1408 "cs.ATG" 
		TypeReference type;
		VariableDeclaration      var = null;
		LocalVariableDeclaration localVariableDeclaration; 
		
		Type(
#line  1413 "cs.ATG" 
out type);

#line  1413 "cs.ATG" 
		localVariableDeclaration = new LocalVariableDeclaration(type); localVariableDeclaration.StartLocation = t.Location; 
		LocalVariableDeclarator(
#line  1414 "cs.ATG" 
out var);

#line  1414 "cs.ATG" 
		localVariableDeclaration.Variables.Add(var); 
		while (la.kind == 14) {
			lexer.NextToken();
			LocalVariableDeclarator(
#line  1415 "cs.ATG" 
out var);

#line  1415 "cs.ATG" 
			localVariableDeclaration.Variables.Add(var); 
		}

#line  1416 "cs.ATG" 
		stmt = localVariableDeclaration; 
	}

	void LocalVariableDeclarator(
#line  1419 "cs.ATG" 
out VariableDeclaration var) {

#line  1420 "cs.ATG" 
		Expression expr = null; 
		Identifier();

#line  1422 "cs.ATG" 
		var = new VariableDeclaration(t.val); 
		if (la.kind == 3) {
			lexer.NextToken();
			VariableInitializer(
#line  1423 "cs.ATG" 
out expr);

#line  1423 "cs.ATG" 
			var.Initializer = expr; 
		}
	}

	void EmbeddedStatement(
#line  1462 "cs.ATG" 
out Statement statement) {

#line  1464 "cs.ATG" 
		TypeReference type = null;
		Expression expr = null;
		Statement embeddedStatement = null;
		statement = null;
		

#line  1470 "cs.ATG" 
		Location startLocation = la.Location; 
		if (la.kind == 16) {
			Block(
#line  1472 "cs.ATG" 
out statement);
		} else if (la.kind == 11) {
			lexer.NextToken();

#line  1475 "cs.ATG" 
			statement = new EmptyStatement(); 
		} else if (
#line  1478 "cs.ATG" 
UnCheckedAndLBrace()) {

#line  1478 "cs.ATG" 
			Statement block; bool isChecked = true; 
			if (la.kind == 58) {
				lexer.NextToken();
			} else if (la.kind == 118) {
				lexer.NextToken();

#line  1479 "cs.ATG" 
				isChecked = false;
			} else SynErr(196);
			Block(
#line  1480 "cs.ATG" 
out block);

#line  1480 "cs.ATG" 
			statement = isChecked ? (Statement)new CheckedStatement(block) : (Statement)new UncheckedStatement(block); 
		} else if (la.kind == 79) {
			IfStatement(
#line  1483 "cs.ATG" 
out statement);
		} else if (la.kind == 110) {
			lexer.NextToken();

#line  1485 "cs.ATG" 
			List<SwitchSection> switchSections = new List<SwitchSection>(); 
			Expect(20);
			Expr(
#line  1486 "cs.ATG" 
out expr);
			Expect(21);
			Expect(16);
			SwitchSections(
#line  1487 "cs.ATG" 
switchSections);
			Expect(17);

#line  1488 "cs.ATG" 
			statement = new SwitchStatement(expr, switchSections); 
		} else if (la.kind == 125) {
			lexer.NextToken();
			Expect(20);
			Expr(
#line  1491 "cs.ATG" 
out expr);
			Expect(21);
			EmbeddedStatement(
#line  1492 "cs.ATG" 
out embeddedStatement);

#line  1493 "cs.ATG" 
			statement = new DoLoopStatement(expr, embeddedStatement, ConditionType.While, ConditionPosition.Start);
		} else if (la.kind == 65) {
			lexer.NextToken();
			EmbeddedStatement(
#line  1495 "cs.ATG" 
out embeddedStatement);
			Expect(125);
			Expect(20);
			Expr(
#line  1496 "cs.ATG" 
out expr);
			Expect(21);
			Expect(11);

#line  1497 "cs.ATG" 
			statement = new DoLoopStatement(expr, embeddedStatement, ConditionType.While, ConditionPosition.End); 
		} else if (la.kind == 76) {
			lexer.NextToken();

#line  1499 "cs.ATG" 
			List<Statement> initializer = null; List<Statement> iterator = null; 
			Expect(20);
			if (StartOf(6)) {
				ForInitializer(
#line  1500 "cs.ATG" 
out initializer);
			}
			Expect(11);
			if (StartOf(6)) {
				Expr(
#line  1501 "cs.ATG" 
out expr);
			}
			Expect(11);
			if (StartOf(6)) {
				ForIterator(
#line  1502 "cs.ATG" 
out iterator);
			}
			Expect(21);
			EmbeddedStatement(
#line  1503 "cs.ATG" 
out embeddedStatement);

#line  1503 "cs.ATG" 
			statement = new ForStatement(initializer, expr, iterator, embeddedStatement); 
		} else if (la.kind == 77) {
			lexer.NextToken();
			Expect(20);
			Type(
#line  1505 "cs.ATG" 
out type);
			Identifier();

#line  1505 "cs.ATG" 
			string varName = t.val; 
			Expect(81);
			Expr(
#line  1506 "cs.ATG" 
out expr);
			Expect(21);
			EmbeddedStatement(
#line  1507 "cs.ATG" 
out embeddedStatement);

#line  1508 "cs.ATG" 
			statement = new ForeachStatement(type, varName , expr, embeddedStatement); 
		} else if (la.kind == 53) {
			lexer.NextToken();
			Expect(11);

#line  1511 "cs.ATG" 
			statement = new BreakStatement(); 
		} else if (la.kind == 61) {
			lexer.NextToken();
			Expect(11);

#line  1512 "cs.ATG" 
			statement = new ContinueStatement(); 
		} else if (la.kind == 78) {
			GotoStatement(
#line  1513 "cs.ATG" 
out statement);
		} else if (
#line  1515 "cs.ATG" 
IsYieldStatement()) {
			Expect(132);
			if (la.kind == 101) {
				lexer.NextToken();
				Expr(
#line  1516 "cs.ATG" 
out expr);

#line  1516 "cs.ATG" 
				statement = new YieldStatement(new ReturnStatement(expr)); 
			} else if (la.kind == 53) {
				lexer.NextToken();

#line  1517 "cs.ATG" 
				statement = new YieldStatement(new BreakStatement()); 
			} else SynErr(197);
			Expect(11);
		} else if (la.kind == 101) {
			lexer.NextToken();
			if (StartOf(6)) {
				Expr(
#line  1520 "cs.ATG" 
out expr);
			}
			Expect(11);

#line  1520 "cs.ATG" 
			statement = new ReturnStatement(expr); 
		} else if (la.kind == 112) {
			lexer.NextToken();
			if (StartOf(6)) {
				Expr(
#line  1521 "cs.ATG" 
out expr);
			}
			Expect(11);

#line  1521 "cs.ATG" 
			statement = new ThrowStatement(expr); 
		} else if (StartOf(6)) {
			StatementExpr(
#line  1524 "cs.ATG" 
out statement);
			while (!(la.kind == 0 || la.kind == 11)) {SynErr(198); lexer.NextToken(); }
			Expect(11);
		} else if (la.kind == 114) {
			TryStatement(
#line  1527 "cs.ATG" 
out statement);
		} else if (la.kind == 86) {
			lexer.NextToken();
			Expect(20);
			Expr(
#line  1530 "cs.ATG" 
out expr);
			Expect(21);
			EmbeddedStatement(
#line  1531 "cs.ATG" 
out embeddedStatement);

#line  1531 "cs.ATG" 
			statement = new LockStatement(expr, embeddedStatement); 
		} else if (la.kind == 121) {

#line  1534 "cs.ATG" 
			Statement resourceAcquisitionStmt = null; 
			lexer.NextToken();
			Expect(20);
			ResourceAcquisition(
#line  1536 "cs.ATG" 
out resourceAcquisitionStmt);
			Expect(21);
			EmbeddedStatement(
#line  1537 "cs.ATG" 
out embeddedStatement);

#line  1537 "cs.ATG" 
			statement = new UsingStatement(resourceAcquisitionStmt, embeddedStatement); 
		} else if (la.kind == 119) {
			lexer.NextToken();
			Block(
#line  1540 "cs.ATG" 
out embeddedStatement);

#line  1540 "cs.ATG" 
			statement = new UnsafeStatement(embeddedStatement); 
		} else if (la.kind == 74) {
			lexer.NextToken();
			Expect(20);
			Type(
#line  1543 "cs.ATG" 
out type);

#line  1543 "cs.ATG" 
			if (type.PointerNestingLevel == 0) Error("can only fix pointer types");
			List<VariableDeclaration> pointerDeclarators = new List<VariableDeclaration>(1);
			
			Identifier();

#line  1546 "cs.ATG" 
			string identifier = t.val; 
			Expect(3);
			Expr(
#line  1547 "cs.ATG" 
out expr);

#line  1547 "cs.ATG" 
			pointerDeclarators.Add(new VariableDeclaration(identifier, expr)); 
			while (la.kind == 14) {
				lexer.NextToken();
				Identifier();

#line  1549 "cs.ATG" 
				identifier = t.val; 
				Expect(3);
				Expr(
#line  1550 "cs.ATG" 
out expr);

#line  1550 "cs.ATG" 
				pointerDeclarators.Add(new VariableDeclaration(identifier, expr)); 
			}
			Expect(21);
			EmbeddedStatement(
#line  1552 "cs.ATG" 
out embeddedStatement);

#line  1552 "cs.ATG" 
			statement = new FixedStatement(type, pointerDeclarators, embeddedStatement); 
		} else SynErr(199);

#line  1554 "cs.ATG" 
		if (statement != null) {
		statement.StartLocation = startLocation;
		statement.EndLocation = t.EndLocation;
		}
		
	}

	void IfStatement(
#line  1561 "cs.ATG" 
out Statement statement) {

#line  1563 "cs.ATG" 
		Expression expr = null;
		Statement embeddedStatement = null;
		statement = null;
		
		Expect(79);
		Expect(20);
		Expr(
#line  1569 "cs.ATG" 
out expr);
		Expect(21);
		EmbeddedStatement(
#line  1570 "cs.ATG" 
out embeddedStatement);

#line  1571 "cs.ATG" 
		Statement elseStatement = null; 
		if (la.kind == 67) {
			lexer.NextToken();
			EmbeddedStatement(
#line  1572 "cs.ATG" 
out elseStatement);
		}

#line  1573 "cs.ATG" 
		statement = elseStatement != null ? new IfElseStatement(expr, embeddedStatement, elseStatement) : new IfElseStatement(expr, embeddedStatement); 

#line  1574 "cs.ATG" 
		if (elseStatement is IfElseStatement && (elseStatement as IfElseStatement).TrueStatement.Count == 1) {
		/* else if-section (otherwise we would have a BlockStatment) */
		(statement as IfElseStatement).ElseIfSections.Add(
		             new ElseIfSection((elseStatement as IfElseStatement).Condition,
		                               (elseStatement as IfElseStatement).TrueStatement[0]));
		(statement as IfElseStatement).ElseIfSections.AddRange((elseStatement as IfElseStatement).ElseIfSections);
		(statement as IfElseStatement).FalseStatement = (elseStatement as IfElseStatement).FalseStatement;
		}
		
	}

	void SwitchSections(
#line  1604 "cs.ATG" 
List<SwitchSection> switchSections) {

#line  1606 "cs.ATG" 
		SwitchSection switchSection = new SwitchSection();
		CaseLabel label;
		
		SwitchLabel(
#line  1610 "cs.ATG" 
out label);

#line  1610 "cs.ATG" 
		if (label != null) { switchSection.SwitchLabels.Add(label); } 

#line  1611 "cs.ATG" 
		compilationUnit.BlockStart(switchSection); 
		while (StartOf(32)) {
			if (la.kind == 55 || la.kind == 63) {
				SwitchLabel(
#line  1613 "cs.ATG" 
out label);

#line  1614 "cs.ATG" 
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

#line  1626 "cs.ATG" 
		compilationUnit.BlockEnd(); switchSections.Add(switchSection); 
	}

	void ForInitializer(
#line  1585 "cs.ATG" 
out List<Statement> initializer) {

#line  1587 "cs.ATG" 
		Statement stmt; 
		initializer = new List<Statement>();
		
		if (
#line  1591 "cs.ATG" 
IsLocalVarDecl()) {
			LocalVariableDecl(
#line  1591 "cs.ATG" 
out stmt);

#line  1591 "cs.ATG" 
			initializer.Add(stmt);
		} else if (StartOf(6)) {
			StatementExpr(
#line  1592 "cs.ATG" 
out stmt);

#line  1592 "cs.ATG" 
			initializer.Add(stmt);
			while (la.kind == 14) {
				lexer.NextToken();
				StatementExpr(
#line  1592 "cs.ATG" 
out stmt);

#line  1592 "cs.ATG" 
				initializer.Add(stmt);
			}
		} else SynErr(200);
	}

	void ForIterator(
#line  1595 "cs.ATG" 
out List<Statement> iterator) {

#line  1597 "cs.ATG" 
		Statement stmt; 
		iterator = new List<Statement>();
		
		StatementExpr(
#line  1601 "cs.ATG" 
out stmt);

#line  1601 "cs.ATG" 
		iterator.Add(stmt);
		while (la.kind == 14) {
			lexer.NextToken();
			StatementExpr(
#line  1601 "cs.ATG" 
out stmt);

#line  1601 "cs.ATG" 
			iterator.Add(stmt); 
		}
	}

	void GotoStatement(
#line  1679 "cs.ATG" 
out Statement stmt) {

#line  1680 "cs.ATG" 
		Expression expr; stmt = null; 
		Expect(78);
		if (StartOf(19)) {
			Identifier();

#line  1684 "cs.ATG" 
			stmt = new GotoStatement(t.val); 
			Expect(11);
		} else if (la.kind == 55) {
			lexer.NextToken();
			Expr(
#line  1685 "cs.ATG" 
out expr);
			Expect(11);

#line  1685 "cs.ATG" 
			stmt = new GotoCaseStatement(expr); 
		} else if (la.kind == 63) {
			lexer.NextToken();
			Expect(11);

#line  1686 "cs.ATG" 
			stmt = new GotoCaseStatement(null); 
		} else SynErr(201);
	}

	void StatementExpr(
#line  1706 "cs.ATG" 
out Statement stmt) {

#line  1707 "cs.ATG" 
		Expression expr; 
		Expr(
#line  1709 "cs.ATG" 
out expr);

#line  1712 "cs.ATG" 
		stmt = new ExpressionStatement(expr); 
	}

	void TryStatement(
#line  1636 "cs.ATG" 
out Statement tryStatement) {

#line  1638 "cs.ATG" 
		Statement blockStmt = null, finallyStmt = null;
		List<CatchClause> catchClauses = null;
		
		Expect(114);
		Block(
#line  1642 "cs.ATG" 
out blockStmt);
		if (la.kind == 56) {
			CatchClauses(
#line  1644 "cs.ATG" 
out catchClauses);
			if (la.kind == 73) {
				lexer.NextToken();
				Block(
#line  1644 "cs.ATG" 
out finallyStmt);
			}
		} else if (la.kind == 73) {
			lexer.NextToken();
			Block(
#line  1645 "cs.ATG" 
out finallyStmt);
		} else SynErr(202);

#line  1648 "cs.ATG" 
		tryStatement = new TryCatchStatement(blockStmt, catchClauses, finallyStmt);
			
	}

	void ResourceAcquisition(
#line  1690 "cs.ATG" 
out Statement stmt) {

#line  1692 "cs.ATG" 
		stmt = null;
		Expression expr;
		
		if (
#line  1697 "cs.ATG" 
IsLocalVarDecl()) {
			LocalVariableDecl(
#line  1697 "cs.ATG" 
out stmt);
		} else if (StartOf(6)) {
			Expr(
#line  1698 "cs.ATG" 
out expr);

#line  1702 "cs.ATG" 
			stmt = new ExpressionStatement(expr); 
		} else SynErr(203);
	}

	void SwitchLabel(
#line  1629 "cs.ATG" 
out CaseLabel label) {

#line  1630 "cs.ATG" 
		Expression expr = null; label = null; 
		if (la.kind == 55) {
			lexer.NextToken();
			Expr(
#line  1632 "cs.ATG" 
out expr);
			Expect(9);

#line  1632 "cs.ATG" 
			label =  new CaseLabel(expr); 
		} else if (la.kind == 63) {
			lexer.NextToken();
			Expect(9);

#line  1633 "cs.ATG" 
			label =  new CaseLabel(); 
		} else SynErr(204);
	}

	void CatchClauses(
#line  1653 "cs.ATG" 
out List<CatchClause> catchClauses) {

#line  1655 "cs.ATG" 
		catchClauses = new List<CatchClause>();
		
		Expect(56);

#line  1658 "cs.ATG" 
		string identifier;
		Statement stmt;
		TypeReference typeRef;
		
		if (la.kind == 16) {
			Block(
#line  1664 "cs.ATG" 
out stmt);

#line  1664 "cs.ATG" 
			catchClauses.Add(new CatchClause(stmt)); 
		} else if (la.kind == 20) {
			lexer.NextToken();
			ClassType(
#line  1666 "cs.ATG" 
out typeRef, false);

#line  1666 "cs.ATG" 
			identifier = null; 
			if (StartOf(19)) {
				Identifier();

#line  1667 "cs.ATG" 
				identifier = t.val; 
			}
			Expect(21);
			Block(
#line  1668 "cs.ATG" 
out stmt);

#line  1669 "cs.ATG" 
			catchClauses.Add(new CatchClause(typeRef, identifier, stmt)); 
			while (
#line  1670 "cs.ATG" 
IsTypedCatch()) {
				Expect(56);
				Expect(20);
				ClassType(
#line  1670 "cs.ATG" 
out typeRef, false);

#line  1670 "cs.ATG" 
				identifier = null; 
				if (StartOf(19)) {
					Identifier();

#line  1671 "cs.ATG" 
					identifier = t.val; 
				}
				Expect(21);
				Block(
#line  1672 "cs.ATG" 
out stmt);

#line  1673 "cs.ATG" 
				catchClauses.Add(new CatchClause(typeRef, identifier, stmt)); 
			}
			if (la.kind == 56) {
				lexer.NextToken();
				Block(
#line  1675 "cs.ATG" 
out stmt);

#line  1675 "cs.ATG" 
				catchClauses.Add(new CatchClause(stmt)); 
			}
		} else SynErr(205);
	}

	void UnaryExpr(
#line  1739 "cs.ATG" 
out Expression uExpr) {

#line  1741 "cs.ATG" 
		TypeReference type = null;
		Expression expr = null;
		ArrayList expressions = new ArrayList();
		uExpr = null;
		
		while (StartOf(33) || 
#line  1763 "cs.ATG" 
IsTypeCast()) {
			if (la.kind == 4) {
				lexer.NextToken();

#line  1750 "cs.ATG" 
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.Plus)); 
			} else if (la.kind == 5) {
				lexer.NextToken();

#line  1751 "cs.ATG" 
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.Minus)); 
			} else if (la.kind == 24) {
				lexer.NextToken();

#line  1752 "cs.ATG" 
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.Not)); 
			} else if (la.kind == 27) {
				lexer.NextToken();

#line  1753 "cs.ATG" 
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.BitNot)); 
			} else if (la.kind == 6) {
				lexer.NextToken();

#line  1754 "cs.ATG" 
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.Star)); 
			} else if (la.kind == 31) {
				lexer.NextToken();

#line  1755 "cs.ATG" 
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.Increment)); 
			} else if (la.kind == 32) {
				lexer.NextToken();

#line  1756 "cs.ATG" 
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.Decrement)); 
			} else if (la.kind == 28) {
				lexer.NextToken();

#line  1757 "cs.ATG" 
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.BitWiseAnd)); 
			} else {
				Expect(20);
				Type(
#line  1763 "cs.ATG" 
out type);
				Expect(21);

#line  1763 "cs.ATG" 
				expressions.Add(new CastExpression(type)); 
			}
		}
		if (
#line  1768 "cs.ATG" 
LastExpressionIsUnaryMinus(expressions) && IsMostNegativeIntegerWithoutTypeSuffix()) {
			Expect(2);

#line  1771 "cs.ATG" 
			expressions.RemoveAt(expressions.Count - 1);
			if (t.literalValue is uint) {
				expr = new PrimitiveExpression(int.MinValue, int.MinValue.ToString());
			} else if (t.literalValue is ulong) {
				expr = new PrimitiveExpression(long.MinValue, long.MinValue.ToString());
			} else {
				throw new Exception("t.literalValue must be uint or ulong");
			}
			
		} else if (StartOf(34)) {
			PrimaryExpr(
#line  1780 "cs.ATG" 
out expr);
		} else SynErr(206);

#line  1782 "cs.ATG" 
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
		while (la.kind == 26) {
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
#line  1799 "cs.ATG" 
out Expression pexpr) {

#line  1801 "cs.ATG" 
		TypeReference type = null;
		Expression expr;
		pexpr = null;
		

#line  1806 "cs.ATG" 
		Location startLocation = la.Location; 
		if (la.kind == 113) {
			lexer.NextToken();

#line  1808 "cs.ATG" 
			pexpr = new PrimitiveExpression(true, "true");  
		} else if (la.kind == 72) {
			lexer.NextToken();

#line  1809 "cs.ATG" 
			pexpr = new PrimitiveExpression(false, "false"); 
		} else if (la.kind == 90) {
			lexer.NextToken();

#line  1810 "cs.ATG" 
			pexpr = new PrimitiveExpression(null, "null");  
		} else if (la.kind == 2) {
			lexer.NextToken();

#line  1811 "cs.ATG" 
			pexpr = new PrimitiveExpression(t.literalValue, t.val);  
		} else if (
#line  1812 "cs.ATG" 
StartOfQueryExpression()) {
			QueryExpression(
#line  1813 "cs.ATG" 
out pexpr);
		} else if (
#line  1814 "cs.ATG" 
IdentAndDoubleColon()) {
			Identifier();

#line  1815 "cs.ATG" 
			type = new TypeReference(t.val); 
			Expect(10);

#line  1816 "cs.ATG" 
			pexpr = new TypeReferenceExpression(type); 
			Identifier();

#line  1817 "cs.ATG" 
			if (type.Type == "global") { type.IsGlobal = true; type.Type = (t.val ?? "?"); } else type.Type += "." + (t.val ?? "?"); 
		} else if (StartOf(19)) {
			Identifier();

#line  1821 "cs.ATG" 
			pexpr = new IdentifierExpression(t.val); 
			if (la.kind == 48 || 
#line  1824 "cs.ATG" 
IsGenericInSimpleNameOrMemberAccess()) {
				if (la.kind == 48) {
					ShortedLambdaExpression(
#line  1823 "cs.ATG" 
(IdentifierExpression)pexpr, out pexpr);
				} else {

#line  1825 "cs.ATG" 
					List<TypeReference> typeList; 
					TypeArgumentList(
#line  1826 "cs.ATG" 
out typeList, false);

#line  1827 "cs.ATG" 
					((IdentifierExpression)pexpr).TypeArguments = typeList; 
				}
			}
		} else if (
#line  1829 "cs.ATG" 
IsLambdaExpression()) {
			LambdaExpression(
#line  1830 "cs.ATG" 
out pexpr);
		} else if (la.kind == 20) {
			lexer.NextToken();
			Expr(
#line  1833 "cs.ATG" 
out expr);
			Expect(21);

#line  1833 "cs.ATG" 
			pexpr = new ParenthesizedExpression(expr); 
		} else if (StartOf(35)) {

#line  1836 "cs.ATG" 
			string val = null; 
			switch (la.kind) {
			case 52: {
				lexer.NextToken();

#line  1837 "cs.ATG" 
				val = "bool"; 
				break;
			}
			case 54: {
				lexer.NextToken();

#line  1838 "cs.ATG" 
				val = "byte"; 
				break;
			}
			case 57: {
				lexer.NextToken();

#line  1839 "cs.ATG" 
				val = "char"; 
				break;
			}
			case 62: {
				lexer.NextToken();

#line  1840 "cs.ATG" 
				val = "decimal"; 
				break;
			}
			case 66: {
				lexer.NextToken();

#line  1841 "cs.ATG" 
				val = "double"; 
				break;
			}
			case 75: {
				lexer.NextToken();

#line  1842 "cs.ATG" 
				val = "float"; 
				break;
			}
			case 82: {
				lexer.NextToken();

#line  1843 "cs.ATG" 
				val = "int"; 
				break;
			}
			case 87: {
				lexer.NextToken();

#line  1844 "cs.ATG" 
				val = "long"; 
				break;
			}
			case 91: {
				lexer.NextToken();

#line  1845 "cs.ATG" 
				val = "object"; 
				break;
			}
			case 102: {
				lexer.NextToken();

#line  1846 "cs.ATG" 
				val = "sbyte"; 
				break;
			}
			case 104: {
				lexer.NextToken();

#line  1847 "cs.ATG" 
				val = "short"; 
				break;
			}
			case 108: {
				lexer.NextToken();

#line  1848 "cs.ATG" 
				val = "string"; 
				break;
			}
			case 116: {
				lexer.NextToken();

#line  1849 "cs.ATG" 
				val = "uint"; 
				break;
			}
			case 117: {
				lexer.NextToken();

#line  1850 "cs.ATG" 
				val = "ulong"; 
				break;
			}
			case 120: {
				lexer.NextToken();

#line  1851 "cs.ATG" 
				val = "ushort"; 
				break;
			}
			}
			MemberAccess(
#line  1853 "cs.ATG" 
out pexpr, new TypeReferenceExpression(val));
		} else if (la.kind == 111) {
			lexer.NextToken();

#line  1856 "cs.ATG" 
			pexpr = new ThisReferenceExpression(); 
		} else if (la.kind == 51) {
			lexer.NextToken();

#line  1858 "cs.ATG" 
			pexpr = new BaseReferenceExpression(); 
		} else if (la.kind == 89) {
			NewExpression(
#line  1861 "cs.ATG" 
out pexpr);
		} else if (la.kind == 115) {
			lexer.NextToken();
			Expect(20);
			if (
#line  1865 "cs.ATG" 
NotVoidPointer()) {
				Expect(123);

#line  1865 "cs.ATG" 
				type = new TypeReference("void"); 
			} else if (StartOf(10)) {
				TypeWithRestriction(
#line  1866 "cs.ATG" 
out type, true, true);
			} else SynErr(207);
			Expect(21);

#line  1868 "cs.ATG" 
			pexpr = new TypeOfExpression(type); 
		} else if (la.kind == 63) {
			lexer.NextToken();
			Expect(20);
			Type(
#line  1870 "cs.ATG" 
out type);
			Expect(21);

#line  1870 "cs.ATG" 
			pexpr = new DefaultValueExpression(type); 
		} else if (la.kind == 105) {
			lexer.NextToken();
			Expect(20);
			Type(
#line  1871 "cs.ATG" 
out type);
			Expect(21);

#line  1871 "cs.ATG" 
			pexpr = new SizeOfExpression(type); 
		} else if (la.kind == 58) {
			lexer.NextToken();
			Expect(20);
			Expr(
#line  1872 "cs.ATG" 
out expr);
			Expect(21);

#line  1872 "cs.ATG" 
			pexpr = new CheckedExpression(expr); 
		} else if (la.kind == 118) {
			lexer.NextToken();
			Expect(20);
			Expr(
#line  1873 "cs.ATG" 
out expr);
			Expect(21);

#line  1873 "cs.ATG" 
			pexpr = new UncheckedExpression(expr); 
		} else if (la.kind == 64) {
			lexer.NextToken();
			AnonymousMethodExpr(
#line  1874 "cs.ATG" 
out expr);

#line  1874 "cs.ATG" 
			pexpr = expr; 
		} else SynErr(208);

#line  1876 "cs.ATG" 
		if (pexpr != null) {
		pexpr.StartLocation = startLocation;
		pexpr.EndLocation = t.EndLocation;
		}
		
		while (StartOf(36)) {
			if (la.kind == 31 || la.kind == 32) {

#line  1882 "cs.ATG" 
				startLocation = la.Location; 
				if (la.kind == 31) {
					lexer.NextToken();

#line  1884 "cs.ATG" 
					pexpr = new UnaryOperatorExpression(pexpr, UnaryOperatorType.PostIncrement); 
				} else if (la.kind == 32) {
					lexer.NextToken();

#line  1885 "cs.ATG" 
					pexpr = new UnaryOperatorExpression(pexpr, UnaryOperatorType.PostDecrement); 
				} else SynErr(209);
			} else if (la.kind == 47) {
				PointerMemberAccess(
#line  1888 "cs.ATG" 
out pexpr, pexpr);
			} else if (la.kind == 15) {
				MemberAccess(
#line  1889 "cs.ATG" 
out pexpr, pexpr);
			} else if (la.kind == 20) {
				lexer.NextToken();

#line  1892 "cs.ATG" 
				List<Expression> parameters = new List<Expression>(); 
				if (StartOf(26)) {
					Argument(
#line  1893 "cs.ATG" 
out expr);

#line  1893 "cs.ATG" 
					if (expr != null) {parameters.Add(expr);} 
					while (la.kind == 14) {
						lexer.NextToken();
						Argument(
#line  1894 "cs.ATG" 
out expr);

#line  1894 "cs.ATG" 
						if (expr != null) {parameters.Add(expr);} 
					}
				}
				Expect(21);

#line  1897 "cs.ATG" 
				pexpr = new InvocationExpression(pexpr, parameters); 
			} else {

#line  1900 "cs.ATG" 
				List<Expression> indices = new List<Expression>();
				
				lexer.NextToken();
				Expr(
#line  1902 "cs.ATG" 
out expr);

#line  1902 "cs.ATG" 
				if (expr != null) { indices.Add(expr); } 
				while (la.kind == 14) {
					lexer.NextToken();
					Expr(
#line  1903 "cs.ATG" 
out expr);

#line  1903 "cs.ATG" 
					if (expr != null) { indices.Add(expr); } 
				}
				Expect(19);

#line  1904 "cs.ATG" 
				pexpr = new IndexerExpression(pexpr, indices); 

#line  1906 "cs.ATG" 
				if (pexpr != null) {
				pexpr.StartLocation = startLocation;
				pexpr.EndLocation = t.EndLocation;
				}
				
			}
		}
	}

	void QueryExpression(
#line  2330 "cs.ATG" 
out Expression outExpr) {

#line  2331 "cs.ATG" 
		QueryExpression q = new QueryExpression(); outExpr = q; q.StartLocation = la.Location; 
		QueryExpressionFromClause fromClause;
		
		QueryExpressionFromClause(
#line  2335 "cs.ATG" 
out fromClause);

#line  2335 "cs.ATG" 
		q.FromClause = fromClause; 
		QueryExpressionBody(
#line  2336 "cs.ATG" 
q);

#line  2337 "cs.ATG" 
		q.EndLocation = t.EndLocation; 
	}

	void ShortedLambdaExpression(
#line  2019 "cs.ATG" 
IdentifierExpression ident, out Expression pexpr) {

#line  2020 "cs.ATG" 
		LambdaExpression lambda = new LambdaExpression(); pexpr = lambda; 
		Expect(48);

#line  2025 "cs.ATG" 
		lambda.StartLocation = ident.StartLocation;
		lambda.Parameters.Add(new ParameterDeclarationExpression(null, ident.Identifier));
		lambda.Parameters[0].StartLocation = ident.StartLocation;
		lambda.Parameters[0].EndLocation = ident.EndLocation;
		
		LambdaExpressionBody(
#line  2030 "cs.ATG" 
lambda);
	}

	void TypeArgumentList(
#line  2264 "cs.ATG" 
out List<TypeReference> types, bool canBeUnbound) {

#line  2266 "cs.ATG" 
		types = new List<TypeReference>();
		TypeReference type = null;
		
		Expect(23);
		if (
#line  2271 "cs.ATG" 
canBeUnbound && (la.kind == Tokens.GreaterThan || la.kind == Tokens.Comma)) {

#line  2272 "cs.ATG" 
			types.Add(TypeReference.Null); 
			while (la.kind == 14) {
				lexer.NextToken();

#line  2273 "cs.ATG" 
				types.Add(TypeReference.Null); 
			}
		} else if (StartOf(10)) {
			Type(
#line  2274 "cs.ATG" 
out type);

#line  2274 "cs.ATG" 
			if (type != null) { types.Add(type); } 
			while (la.kind == 14) {
				lexer.NextToken();
				Type(
#line  2275 "cs.ATG" 
out type);

#line  2275 "cs.ATG" 
				if (type != null) { types.Add(type); } 
			}
		} else SynErr(210);
		Expect(22);
	}

	void LambdaExpression(
#line  1999 "cs.ATG" 
out Expression outExpr) {

#line  2001 "cs.ATG" 
		LambdaExpression lambda = new LambdaExpression();
		lambda.StartLocation = la.Location;
		ParameterDeclarationExpression p;
		outExpr = lambda;
		
		Expect(20);
		if (StartOf(10)) {
			LambdaExpressionParameter(
#line  2009 "cs.ATG" 
out p);

#line  2009 "cs.ATG" 
			if (p != null) lambda.Parameters.Add(p); 
			while (la.kind == 14) {
				lexer.NextToken();
				LambdaExpressionParameter(
#line  2011 "cs.ATG" 
out p);

#line  2011 "cs.ATG" 
				if (p != null) lambda.Parameters.Add(p); 
			}
		}
		Expect(21);
		Expect(48);
		LambdaExpressionBody(
#line  2016 "cs.ATG" 
lambda);
	}

	void MemberAccess(
#line  1914 "cs.ATG" 
out Expression expr, Expression target) {

#line  1915 "cs.ATG" 
		List<TypeReference> typeList; 

#line  1917 "cs.ATG" 
		if (ShouldConvertTargetExpressionToTypeReference(target)) {
		TypeReference type = GetTypeReferenceFromExpression(target);
		if (type != null) {
			target = new TypeReferenceExpression(type);
		}
		}
		t.val = ""; // required for TypeReferenceExpressionTests.StandaloneIntReferenceExpression hack
		
		Expect(15);
		Identifier();

#line  1927 "cs.ATG" 
		expr = new MemberReferenceExpression(target, t.val); 
		if (
#line  1928 "cs.ATG" 
IsGenericInSimpleNameOrMemberAccess()) {
			TypeArgumentList(
#line  1929 "cs.ATG" 
out typeList, false);

#line  1930 "cs.ATG" 
			((MemberReferenceExpression)expr).TypeArguments = typeList; 
		}
	}

	void NewExpression(
#line  1946 "cs.ATG" 
out Expression pexpr) {

#line  1947 "cs.ATG" 
		pexpr = null;
		List<Expression> parameters = new List<Expression>();
		TypeReference type = null;
		Expression expr;
		
		Expect(89);
		if (StartOf(10)) {
			NonArrayType(
#line  1954 "cs.ATG" 
out type);
		}
		if (la.kind == 16 || la.kind == 20) {
			if (la.kind == 20) {

#line  1960 "cs.ATG" 
				ObjectCreateExpression oce = new ObjectCreateExpression(type, parameters); 
				lexer.NextToken();

#line  1961 "cs.ATG" 
				if (type == null) Error("Cannot use an anonymous type with arguments for the constructor"); 
				if (StartOf(26)) {
					Argument(
#line  1962 "cs.ATG" 
out expr);

#line  1962 "cs.ATG" 
					if (expr != null) { parameters.Add(expr); } 
					while (la.kind == 14) {
						lexer.NextToken();
						Argument(
#line  1963 "cs.ATG" 
out expr);

#line  1963 "cs.ATG" 
						if (expr != null) { parameters.Add(expr); } 
					}
				}
				Expect(21);

#line  1965 "cs.ATG" 
				pexpr = oce; 
				if (la.kind == 16) {
					CollectionOrObjectInitializer(
#line  1966 "cs.ATG" 
out expr);

#line  1966 "cs.ATG" 
					oce.ObjectInitializer = (CollectionInitializerExpression)expr; 
				}
			} else {

#line  1967 "cs.ATG" 
				ObjectCreateExpression oce = new ObjectCreateExpression(type, parameters); 
				CollectionOrObjectInitializer(
#line  1968 "cs.ATG" 
out expr);

#line  1968 "cs.ATG" 
				oce.ObjectInitializer = (CollectionInitializerExpression)expr; 

#line  1969 "cs.ATG" 
				pexpr = oce; 
			}
		} else if (la.kind == 18) {
			lexer.NextToken();

#line  1974 "cs.ATG" 
			ArrayCreateExpression ace = new ArrayCreateExpression(type);
			/* we must not change RankSpecifier on the null type reference*/
			if (ace.CreateType.IsNull) { ace.CreateType = new TypeReference(""); }
			pexpr = ace;
			int dims = 0; List<int> ranks = new List<int>();
			
			if (la.kind == 14 || la.kind == 19) {
				while (la.kind == 14) {
					lexer.NextToken();

#line  1981 "cs.ATG" 
					dims += 1; 
				}
				Expect(19);

#line  1982 "cs.ATG" 
				ranks.Add(dims); dims = 0; 
				while (la.kind == 18) {
					lexer.NextToken();
					while (la.kind == 14) {
						lexer.NextToken();

#line  1983 "cs.ATG" 
						++dims; 
					}
					Expect(19);

#line  1983 "cs.ATG" 
					ranks.Add(dims); dims = 0; 
				}

#line  1984 "cs.ATG" 
				ace.CreateType.RankSpecifier = ranks.ToArray(); 
				CollectionInitializer(
#line  1985 "cs.ATG" 
out expr);

#line  1985 "cs.ATG" 
				ace.ArrayInitializer = (CollectionInitializerExpression)expr; 
			} else if (StartOf(6)) {
				Expr(
#line  1986 "cs.ATG" 
out expr);

#line  1986 "cs.ATG" 
				if (expr != null) parameters.Add(expr); 
				while (la.kind == 14) {
					lexer.NextToken();

#line  1987 "cs.ATG" 
					dims += 1; 
					Expr(
#line  1988 "cs.ATG" 
out expr);

#line  1988 "cs.ATG" 
					if (expr != null) parameters.Add(expr); 
				}
				Expect(19);

#line  1990 "cs.ATG" 
				ranks.Add(dims); ace.Arguments = parameters; dims = 0; 
				while (la.kind == 18) {
					lexer.NextToken();
					while (la.kind == 14) {
						lexer.NextToken();

#line  1991 "cs.ATG" 
						++dims; 
					}
					Expect(19);

#line  1991 "cs.ATG" 
					ranks.Add(dims); dims = 0; 
				}

#line  1992 "cs.ATG" 
				ace.CreateType.RankSpecifier = ranks.ToArray(); 
				if (la.kind == 16) {
					CollectionInitializer(
#line  1993 "cs.ATG" 
out expr);

#line  1993 "cs.ATG" 
					ace.ArrayInitializer = (CollectionInitializerExpression)expr; 
				}
			} else SynErr(211);
		} else SynErr(212);
	}

	void AnonymousMethodExpr(
#line  2061 "cs.ATG" 
out Expression outExpr) {

#line  2063 "cs.ATG" 
		AnonymousMethodExpression expr = new AnonymousMethodExpression();
		expr.StartLocation = t.Location;
		BlockStatement stmt;
		List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
		outExpr = expr;
		
		if (la.kind == 20) {
			lexer.NextToken();
			if (StartOf(11)) {
				FormalParameterList(
#line  2072 "cs.ATG" 
p);

#line  2072 "cs.ATG" 
				expr.Parameters = p; 
			}
			Expect(21);

#line  2074 "cs.ATG" 
			expr.HasParameterList = true; 
		}
		BlockInsideExpression(
#line  2076 "cs.ATG" 
out stmt);

#line  2076 "cs.ATG" 
		expr.Body  = stmt; 

#line  2077 "cs.ATG" 
		expr.EndLocation = t.Location; 
	}

	void PointerMemberAccess(
#line  1934 "cs.ATG" 
out Expression expr, Expression target) {

#line  1935 "cs.ATG" 
		List<TypeReference> typeList; 
		Expect(47);
		Identifier();

#line  1939 "cs.ATG" 
		expr = new PointerReferenceExpression(target, t.val); 
		if (
#line  1940 "cs.ATG" 
IsGenericInSimpleNameOrMemberAccess()) {
			TypeArgumentList(
#line  1941 "cs.ATG" 
out typeList, false);

#line  1942 "cs.ATG" 
			((MemberReferenceExpression)expr).TypeArguments = typeList; 
		}
	}

	void LambdaExpressionParameter(
#line  2033 "cs.ATG" 
out ParameterDeclarationExpression p) {

#line  2034 "cs.ATG" 
		Location start = la.Location; p = null;
		TypeReference type;
		
		if (
#line  2038 "cs.ATG" 
Peek(1).kind == Tokens.Comma || Peek(1).kind == Tokens.CloseParenthesis) {
			Identifier();

#line  2040 "cs.ATG" 
			p = new ParameterDeclarationExpression(null, t.val);
			p.StartLocation = start; p.EndLocation = t.EndLocation;
			
		} else if (StartOf(10)) {
			Type(
#line  2043 "cs.ATG" 
out type);
			Identifier();

#line  2045 "cs.ATG" 
			p = new ParameterDeclarationExpression(type, t.val);
			p.StartLocation = start; p.EndLocation = t.EndLocation;
			
		} else SynErr(213);
	}

	void LambdaExpressionBody(
#line  2051 "cs.ATG" 
LambdaExpression lambda) {

#line  2052 "cs.ATG" 
		Expression expr; BlockStatement stmt; 
		if (la.kind == 16) {
			BlockInsideExpression(
#line  2055 "cs.ATG" 
out stmt);

#line  2055 "cs.ATG" 
			lambda.StatementBody = stmt; 
		} else if (StartOf(6)) {
			Expr(
#line  2056 "cs.ATG" 
out expr);

#line  2056 "cs.ATG" 
			lambda.ExpressionBody = expr; 
		} else SynErr(214);

#line  2058 "cs.ATG" 
		lambda.EndLocation = t.EndLocation; 
	}

	void BlockInsideExpression(
#line  2080 "cs.ATG" 
out BlockStatement outStmt) {

#line  2081 "cs.ATG" 
		Statement stmt = null; outStmt = null; 

#line  2085 "cs.ATG" 
		if (compilationUnit != null) { 
		Block(
#line  2086 "cs.ATG" 
out stmt);

#line  2086 "cs.ATG" 
		outStmt = (BlockStatement)stmt; 

#line  2087 "cs.ATG" 
		} else { 
		Expect(16);

#line  2089 "cs.ATG" 
		lexer.SkipCurrentBlock(0); 
		Expect(17);

#line  2091 "cs.ATG" 
		} 
	}

	void ConditionalAndExpr(
#line  2100 "cs.ATG" 
ref Expression outExpr) {

#line  2101 "cs.ATG" 
		Expression expr; 
		InclusiveOrExpr(
#line  2103 "cs.ATG" 
ref outExpr);
		while (la.kind == 25) {
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
		while (la.kind == 29) {
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
		while (la.kind == 30) {
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
		while (la.kind == 28) {
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
		while (la.kind == 33 || la.kind == 34) {
			if (la.kind == 34) {
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
		while (StartOf(37)) {
			if (StartOf(38)) {
				if (la.kind == 23) {
					lexer.NextToken();

#line  2149 "cs.ATG" 
					op = BinaryOperatorType.LessThan; 
				} else if (la.kind == 22) {
					lexer.NextToken();

#line  2150 "cs.ATG" 
					op = BinaryOperatorType.GreaterThan; 
				} else if (la.kind == 36) {
					lexer.NextToken();

#line  2151 "cs.ATG" 
					op = BinaryOperatorType.LessThanOrEqual; 
				} else if (la.kind == 35) {
					lexer.NextToken();

#line  2152 "cs.ATG" 
					op = BinaryOperatorType.GreaterThanOrEqual; 
				} else SynErr(215);
				UnaryExpr(
#line  2154 "cs.ATG" 
out expr);
				ShiftExpr(
#line  2155 "cs.ATG" 
ref expr);

#line  2156 "cs.ATG" 
				outExpr = new BinaryOperatorExpression(outExpr, op, expr); 
			} else {
				if (la.kind == 85) {
					lexer.NextToken();
					TypeWithRestriction(
#line  2159 "cs.ATG" 
out type, false, false);
					if (
#line  2160 "cs.ATG" 
la.kind == Tokens.Question && !IsPossibleExpressionStart(Peek(1).kind)) {
						NullableQuestionMark(
#line  2161 "cs.ATG" 
ref type);
					}

#line  2162 "cs.ATG" 
					outExpr = new TypeOfIsExpression(outExpr, type); 
				} else if (la.kind == 50) {
					lexer.NextToken();
					TypeWithRestriction(
#line  2164 "cs.ATG" 
out type, false, false);
					if (
#line  2165 "cs.ATG" 
la.kind == Tokens.Question && !IsPossibleExpressionStart(Peek(1).kind)) {
						NullableQuestionMark(
#line  2166 "cs.ATG" 
ref type);
					}

#line  2167 "cs.ATG" 
					outExpr = new CastExpression(type, outExpr, CastType.TryCast); 
				} else SynErr(216);
			}
		}
	}

	void ShiftExpr(
#line  2172 "cs.ATG" 
ref Expression outExpr) {

#line  2174 "cs.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		AdditiveExpr(
#line  2178 "cs.ATG" 
ref outExpr);
		while (la.kind == 37 || 
#line  2181 "cs.ATG" 
IsShiftRight()) {
			if (la.kind == 37) {
				lexer.NextToken();

#line  2180 "cs.ATG" 
				op = BinaryOperatorType.ShiftLeft; 
			} else {
				Expect(22);
				Expect(22);

#line  2182 "cs.ATG" 
				op = BinaryOperatorType.ShiftRight; 
			}
			UnaryExpr(
#line  2185 "cs.ATG" 
out expr);
			AdditiveExpr(
#line  2185 "cs.ATG" 
ref expr);

#line  2185 "cs.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr);  
		}
	}

	void AdditiveExpr(
#line  2189 "cs.ATG" 
ref Expression outExpr) {

#line  2191 "cs.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		MultiplicativeExpr(
#line  2195 "cs.ATG" 
ref outExpr);
		while (la.kind == 4 || la.kind == 5) {
			if (la.kind == 4) {
				lexer.NextToken();

#line  2198 "cs.ATG" 
				op = BinaryOperatorType.Add; 
			} else {
				lexer.NextToken();

#line  2199 "cs.ATG" 
				op = BinaryOperatorType.Subtract; 
			}
			UnaryExpr(
#line  2201 "cs.ATG" 
out expr);
			MultiplicativeExpr(
#line  2201 "cs.ATG" 
ref expr);

#line  2201 "cs.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr);  
		}
	}

	void MultiplicativeExpr(
#line  2205 "cs.ATG" 
ref Expression outExpr) {

#line  2207 "cs.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		while (la.kind == 6 || la.kind == 7 || la.kind == 8) {
			if (la.kind == 6) {
				lexer.NextToken();

#line  2213 "cs.ATG" 
				op = BinaryOperatorType.Multiply; 
			} else if (la.kind == 7) {
				lexer.NextToken();

#line  2214 "cs.ATG" 
				op = BinaryOperatorType.Divide; 
			} else {
				lexer.NextToken();

#line  2215 "cs.ATG" 
				op = BinaryOperatorType.Modulus; 
			}
			UnaryExpr(
#line  2217 "cs.ATG" 
out expr);

#line  2217 "cs.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr); 
		}
	}

	void TypeParameterConstraintsClauseBase(
#line  2321 "cs.ATG" 
out TypeReference type) {

#line  2322 "cs.ATG" 
		TypeReference t; type = null; 
		if (la.kind == 109) {
			lexer.NextToken();

#line  2324 "cs.ATG" 
			type = TypeReference.StructConstraint; 
		} else if (la.kind == 59) {
			lexer.NextToken();

#line  2325 "cs.ATG" 
			type = TypeReference.ClassConstraint; 
		} else if (la.kind == 89) {
			lexer.NextToken();
			Expect(20);
			Expect(21);

#line  2326 "cs.ATG" 
			type = TypeReference.NewConstraint; 
		} else if (StartOf(10)) {
			Type(
#line  2327 "cs.ATG" 
out t);

#line  2327 "cs.ATG" 
			type = t; 
		} else SynErr(217);
	}

	void QueryExpressionFromClause(
#line  2340 "cs.ATG" 
out QueryExpressionFromClause fc) {

#line  2341 "cs.ATG" 
		fc = new QueryExpressionFromClause(); fc.StartLocation = la.Location; 
		
		Expect(137);
		QueryExpressionFromOrJoinClause(
#line  2345 "cs.ATG" 
fc);

#line  2346 "cs.ATG" 
		fc.EndLocation = t.EndLocation; 
	}

	void QueryExpressionBody(
#line  2376 "cs.ATG" 
QueryExpression q) {

#line  2377 "cs.ATG" 
		QueryExpressionFromClause fromClause;     QueryExpressionWhereClause whereClause;
		QueryExpressionLetClause letClause;       QueryExpressionJoinClause joinClause;
		QueryExpressionSelectClause selectClause; QueryExpressionGroupClause groupClause;
		QueryExpressionIntoClause intoClause;
		
		while (StartOf(39)) {
			if (la.kind == 137) {
				QueryExpressionFromClause(
#line  2383 "cs.ATG" 
out fromClause);

#line  2383 "cs.ATG" 
				SafeAdd<QueryExpressionClause>(q, q.FromLetWhereClauses, fromClause); 
			} else if (la.kind == 127) {
				QueryExpressionWhereClause(
#line  2384 "cs.ATG" 
out whereClause);

#line  2384 "cs.ATG" 
				SafeAdd<QueryExpressionClause>(q, q.FromLetWhereClauses, whereClause); 
			} else if (la.kind == 141) {
				QueryExpressionLetClause(
#line  2385 "cs.ATG" 
out letClause);

#line  2385 "cs.ATG" 
				SafeAdd<QueryExpressionClause>(q, q.FromLetWhereClauses, letClause); 
			} else {
				QueryExpressionJoinClause(
#line  2386 "cs.ATG" 
out joinClause);

#line  2386 "cs.ATG" 
				SafeAdd<QueryExpressionClause>(q, q.FromLetWhereClauses, joinClause); 
			}
		}
		if (la.kind == 140) {
			QueryExpressionOrderByClause(
#line  2388 "cs.ATG" 
q);
		}
		if (la.kind == 133) {
			QueryExpressionSelectClause(
#line  2389 "cs.ATG" 
out selectClause);

#line  2389 "cs.ATG" 
			q.SelectOrGroupClause = selectClause; 
		} else if (la.kind == 134) {
			QueryExpressionGroupClause(
#line  2390 "cs.ATG" 
out groupClause);

#line  2390 "cs.ATG" 
			q.SelectOrGroupClause = groupClause; 
		} else SynErr(218);
		if (la.kind == 136) {
			QueryExpressionIntoClause(
#line  2392 "cs.ATG" 
out intoClause);

#line  2392 "cs.ATG" 
			q.IntoClause = intoClause; 
		}
	}

	void QueryExpressionFromOrJoinClause(
#line  2366 "cs.ATG" 
QueryExpressionFromOrJoinClause fjc) {

#line  2367 "cs.ATG" 
		TypeReference type; Expression expr; 

#line  2369 "cs.ATG" 
		fjc.Type = null; 
		if (
#line  2370 "cs.ATG" 
IsLocalVarDecl()) {
			Type(
#line  2370 "cs.ATG" 
out type);

#line  2370 "cs.ATG" 
			fjc.Type = type; 
		}
		Identifier();

#line  2371 "cs.ATG" 
		fjc.Identifier = t.val; 
		Expect(81);
		Expr(
#line  2373 "cs.ATG" 
out expr);

#line  2373 "cs.ATG" 
		fjc.InExpression = expr; 
	}

	void QueryExpressionJoinClause(
#line  2349 "cs.ATG" 
out QueryExpressionJoinClause jc) {

#line  2350 "cs.ATG" 
		jc = new QueryExpressionJoinClause(); jc.StartLocation = la.Location; 
		Expression expr;
		
		Expect(142);
		QueryExpressionFromOrJoinClause(
#line  2355 "cs.ATG" 
jc);
		Expect(143);
		Expr(
#line  2357 "cs.ATG" 
out expr);

#line  2357 "cs.ATG" 
		jc.OnExpression = expr; 
		Expect(144);
		Expr(
#line  2359 "cs.ATG" 
out expr);

#line  2359 "cs.ATG" 
		jc.EqualsExpression = expr; 
		if (la.kind == 136) {
			lexer.NextToken();
			Identifier();

#line  2361 "cs.ATG" 
			jc.IntoIdentifier = t.val; 
		}

#line  2363 "cs.ATG" 
		jc.EndLocation = t.EndLocation; 
	}

	void QueryExpressionWhereClause(
#line  2395 "cs.ATG" 
out QueryExpressionWhereClause wc) {

#line  2396 "cs.ATG" 
		Expression expr; wc = new QueryExpressionWhereClause(); wc.StartLocation = la.Location; 
		Expect(127);
		Expr(
#line  2399 "cs.ATG" 
out expr);

#line  2399 "cs.ATG" 
		wc.Condition = expr; 

#line  2400 "cs.ATG" 
		wc.EndLocation = t.EndLocation; 
	}

	void QueryExpressionLetClause(
#line  2403 "cs.ATG" 
out QueryExpressionLetClause wc) {

#line  2404 "cs.ATG" 
		Expression expr; wc = new QueryExpressionLetClause(); wc.StartLocation = la.Location; 
		Expect(141);
		Identifier();

#line  2407 "cs.ATG" 
		wc.Identifier = t.val; 
		Expect(3);
		Expr(
#line  2409 "cs.ATG" 
out expr);

#line  2409 "cs.ATG" 
		wc.Expression = expr; 

#line  2410 "cs.ATG" 
		wc.EndLocation = t.EndLocation; 
	}

	void QueryExpressionOrderByClause(
#line  2413 "cs.ATG" 
QueryExpression q) {

#line  2414 "cs.ATG" 
		QueryExpressionOrdering ordering; 
		Expect(140);
		QueryExpressionOrderingClause(
#line  2417 "cs.ATG" 
out ordering);

#line  2417 "cs.ATG" 
		SafeAdd(q, q.Orderings, ordering); 
		while (la.kind == 14) {
			lexer.NextToken();
			QueryExpressionOrderingClause(
#line  2419 "cs.ATG" 
out ordering);

#line  2419 "cs.ATG" 
			SafeAdd(q, q.Orderings, ordering); 
		}
	}

	void QueryExpressionSelectClause(
#line  2433 "cs.ATG" 
out QueryExpressionSelectClause sc) {

#line  2434 "cs.ATG" 
		Expression expr; sc = new QueryExpressionSelectClause(); sc.StartLocation = la.Location; 
		Expect(133);
		Expr(
#line  2437 "cs.ATG" 
out expr);

#line  2437 "cs.ATG" 
		sc.Projection = expr; 

#line  2438 "cs.ATG" 
		sc.EndLocation = t.EndLocation; 
	}

	void QueryExpressionGroupClause(
#line  2441 "cs.ATG" 
out QueryExpressionGroupClause gc) {

#line  2442 "cs.ATG" 
		Expression expr; gc = new QueryExpressionGroupClause(); gc.StartLocation = la.Location; 
		Expect(134);
		Expr(
#line  2445 "cs.ATG" 
out expr);

#line  2445 "cs.ATG" 
		gc.Projection = expr; 
		Expect(135);
		Expr(
#line  2447 "cs.ATG" 
out expr);

#line  2447 "cs.ATG" 
		gc.GroupBy = expr; 

#line  2448 "cs.ATG" 
		gc.EndLocation = t.EndLocation; 
	}

	void QueryExpressionIntoClause(
#line  2451 "cs.ATG" 
out QueryExpressionIntoClause ic) {

#line  2452 "cs.ATG" 
		ic = new QueryExpressionIntoClause(); ic.StartLocation = la.Location; 
		Expect(136);
		Identifier();

#line  2455 "cs.ATG" 
		ic.IntoIdentifier = t.val; 

#line  2456 "cs.ATG" 
		ic.ContinuedQuery = new QueryExpression(); 

#line  2457 "cs.ATG" 
		ic.ContinuedQuery.StartLocation = la.Location; 
		QueryExpressionBody(
#line  2458 "cs.ATG" 
ic.ContinuedQuery);

#line  2459 "cs.ATG" 
		ic.ContinuedQuery.EndLocation = t.EndLocation; 

#line  2460 "cs.ATG" 
		ic.EndLocation = t.EndLocation; 
	}

	void QueryExpressionOrderingClause(
#line  2423 "cs.ATG" 
out QueryExpressionOrdering ordering) {

#line  2424 "cs.ATG" 
		Expression expr; ordering = new QueryExpressionOrdering(); ordering.StartLocation = la.Location; 
		Expr(
#line  2426 "cs.ATG" 
out expr);

#line  2426 "cs.ATG" 
		ordering.Criteria = expr; 
		if (la.kind == 138 || la.kind == 139) {
			if (la.kind == 138) {
				lexer.NextToken();

#line  2427 "cs.ATG" 
				ordering.Direction = QueryExpressionOrderingDirection.Ascending; 
			} else {
				lexer.NextToken();

#line  2428 "cs.ATG" 
				ordering.Direction = QueryExpressionOrderingDirection.Descending; 
			}
		}

#line  2430 "cs.ATG" 
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
			case 154: s = "this symbol not expected in ClassBody"; break;
			case 155: s = "this symbol not expected in InterfaceBody"; break;
			case 156: s = "invalid IntegralType"; break;
			case 157: s = "invalid FormalParameterList"; break;
			case 158: s = "invalid FormalParameterList"; break;
			case 159: s = "invalid ClassType"; break;
			case 160: s = "invalid ClassMemberDecl"; break;
			case 161: s = "invalid ClassMemberDecl"; break;
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
			case 173: s = "invalid StructMemberDecl"; break;
			case 174: s = "invalid StructMemberDecl"; break;
			case 175: s = "invalid InterfaceMemberDecl"; break;
			case 176: s = "invalid InterfaceMemberDecl"; break;
			case 177: s = "invalid InterfaceMemberDecl"; break;
			case 178: s = "invalid TypeWithRestriction"; break;
			case 179: s = "invalid TypeWithRestriction"; break;
			case 180: s = "invalid SimpleType"; break;
			case 181: s = "invalid AccessorModifiers"; break;
			case 182: s = "invalid EventAccessorDecls"; break;
			case 183: s = "invalid ConstructorInitializer"; break;
			case 184: s = "invalid OverloadableOperator"; break;
			case 185: s = "invalid AccessorDecls"; break;
			case 186: s = "invalid InterfaceAccessors"; break;
			case 187: s = "invalid InterfaceAccessors"; break;
			case 188: s = "invalid GetAccessorDecl"; break;
			case 189: s = "invalid SetAccessorDecl"; break;
			case 190: s = "invalid VariableInitializer"; break;
			case 191: s = "this symbol not expected in Statement"; break;
			case 192: s = "invalid Statement"; break;
			case 193: s = "invalid AssignmentOperator"; break;
			case 194: s = "invalid ObjectPropertyInitializerOrVariableInitializer"; break;
			case 195: s = "invalid ObjectPropertyInitializerOrVariableInitializer"; break;
			case 196: s = "invalid EmbeddedStatement"; break;
			case 197: s = "invalid EmbeddedStatement"; break;
			case 198: s = "this symbol not expected in EmbeddedStatement"; break;
			case 199: s = "invalid EmbeddedStatement"; break;
			case 200: s = "invalid ForInitializer"; break;
			case 201: s = "invalid GotoStatement"; break;
			case 202: s = "invalid TryStatement"; break;
			case 203: s = "invalid ResourceAcquisition"; break;
			case 204: s = "invalid SwitchLabel"; break;
			case 205: s = "invalid CatchClauses"; break;
			case 206: s = "invalid UnaryExpr"; break;
			case 207: s = "invalid PrimaryExpr"; break;
			case 208: s = "invalid PrimaryExpr"; break;
			case 209: s = "invalid PrimaryExpr"; break;
			case 210: s = "invalid TypeArgumentList"; break;
			case 211: s = "invalid NewExpression"; break;
			case 212: s = "invalid NewExpression"; break;
			case 213: s = "invalid LambdaExpressionParameter"; break;
			case 214: s = "invalid LambdaExpressionBody"; break;
			case 215: s = "invalid RelationalExpr"; break;
			case 216: s = "invalid RelationalExpr"; break;
			case 217: s = "invalid TypeParameterConstraintsClauseBase"; break;
			case 218: s = "invalid QueryExpressionBody"; break;

			default: s = "error " + errorNumber; break;
		}
		this.Errors.Error(line, col, s);
	}
	
	private bool StartOf(int s)
	{
		return set[s, lexer.LookAhead.kind];
	}
	
	static bool[,] set = {
	{T,T,T,x, T,T,T,x, x,x,x,T, x,x,x,x, T,x,T,x, T,x,x,x, T,x,x,T, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, T,T,T,x, x,T,T,T, T,T,T,T, T,T,T,x, T,T,T,T, T,x,T,T, T,T,T,T, T,x,T,T, T,x,T,T, x,T,T,T, x,x,T,x, T,T,T,T, x,T,T,T, T,T,x,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
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
	{T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, T,x,T,x, x,T,x,T, T,x,T,x, T,x,T,x, T,T,T,T, x,x,T,T, x,x,x,x, T,x,T,T, T,x,x,T, x,T,x,T, x,x,T,x, T,T,T,T, x,x,T,T, T,x,x,T, T,T,x,x, x,x,x,x, T,T,x,T, T,x,T,T, T,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, T,x,T,x, x,T,x,T, T,x,T,x, T,x,T,x, T,T,T,T, x,x,T,T, x,x,x,x, T,x,T,T, T,x,x,T, x,T,x,T, x,x,T,x, T,T,T,T, x,x,T,T, T,x,x,T, T,T,x,x, x,x,x,x, T,T,x,T, T,x,T,T, T,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,x, x,T,x,x, x,x,T,x, x,x,T,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,T,x,T, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,x, T,x,x,x, x,x,x,x, T,T,x,x, T,x,x,T, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
	{T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,x, x,T,x,x, x,x,T,x, x,x,T,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,T,x,T, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,x, T,x,x,x, x,x,x,x, T,T,x,x, T,x,x,T, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
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
	{T,T,T,x, T,T,T,x, x,x,x,T, x,x,x,x, T,x,x,x, T,x,x,x, T,x,x,T, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,T,x, x,T,T,x, T,T,T,T, T,T,T,x, x,x,x,x, T,x,T,T, T,T,T,T, x,x,T,x, x,x,T,T, x,T,T,T, x,x,x,x, x,x,x,x, x,T,T,x, T,T,x,x, T,x,T,T, T,T,T,T, T,T,T,T, T,T,x,x, x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
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