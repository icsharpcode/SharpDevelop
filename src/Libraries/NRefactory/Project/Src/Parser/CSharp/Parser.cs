
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
	const int maxT = 147;

	const  bool   T            = true;
	const  bool   x            = false;
	

#line  18 "cs.ATG" 


/*

*/

	void CS() {

#line  182 "cs.ATG" 
		lexer.NextToken(); // get the first token
		compilationUnit = new CompilationUnit();
		BlockStart(compilationUnit);
		
		while (la.kind == 71) {
			ExternAliasDirective();
		}
		while (la.kind == 121) {
			UsingDirective();
		}
		while (
#line  189 "cs.ATG" 
IsGlobalAttrTarget()) {
			GlobalAttributeSection();
		}
		while (StartOf(1)) {
			NamespaceMemberDecl();
		}
		Expect(0);
	}

	void ExternAliasDirective() {

#line  362 "cs.ATG" 
		ExternAliasDirective ead = new ExternAliasDirective { StartLocation = la.Location }; 
		Expect(71);
		Identifier();

#line  365 "cs.ATG" 
		if (t.val != "alias") Error("Expected 'extern alias'."); 
		Identifier();

#line  366 "cs.ATG" 
		ead.Name = t.val; 
		Expect(11);

#line  367 "cs.ATG" 
		ead.EndLocation = t.EndLocation; 

#line  368 "cs.ATG" 
		AddChild(ead); 
	}

	void UsingDirective() {

#line  196 "cs.ATG" 
		string qualident = null; TypeReference aliasedType = null;
		string alias = null;
		
		Expect(121);

#line  200 "cs.ATG" 
		Location startPos = t.Location; 
		if (
#line  201 "cs.ATG" 
IdentAndDoubleColon()) {
			Identifier();

#line  201 "cs.ATG" 
			alias = t.val; 
			Expect(10);
		}
		Qualident(
#line  202 "cs.ATG" 
out qualident);
		if (la.kind == 3) {
			lexer.NextToken();
			NonArrayType(
#line  203 "cs.ATG" 
out aliasedType);
		}
		Expect(11);

#line  205 "cs.ATG" 
		if (qualident != null && qualident.Length > 0) {
		 string name = (alias != null && alias != "global") ? alias + "." + qualident : qualident;
		 INode node;
		 if (aliasedType != null) {
		     node = new UsingDeclaration(name, aliasedType);
		 } else {
		     node = new UsingDeclaration(name);
		 }
		 node.StartLocation = startPos;
		 node.EndLocation   = t.EndLocation;
		 AddChild(node);
		}
		
	}

	void GlobalAttributeSection() {
		Expect(18);

#line  222 "cs.ATG" 
		Location startPos = t.Location; 
		Identifier();

#line  223 "cs.ATG" 
		if (t.val != "assembly" && t.val != "module") Error("global attribute target specifier (assembly or module) expected");
		string attributeTarget = t.val;
		List<ASTAttribute> attributes = new List<ASTAttribute>();
		ASTAttribute attribute;
		
		Expect(9);
		Attribute(
#line  228 "cs.ATG" 
out attribute);

#line  228 "cs.ATG" 
		attributes.Add(attribute); 
		while (
#line  229 "cs.ATG" 
NotFinalComma()) {
			Expect(14);
			Attribute(
#line  229 "cs.ATG" 
out attribute);

#line  229 "cs.ATG" 
			attributes.Add(attribute); 
		}
		if (la.kind == 14) {
			lexer.NextToken();
		}
		Expect(19);

#line  231 "cs.ATG" 
		AttributeSection section = new AttributeSection {
		   AttributeTarget = attributeTarget,
		   Attributes = attributes,
		   StartLocation = startPos,
		   EndLocation = t.EndLocation
		};
		AddChild(section);
		
	}

	void NamespaceMemberDecl() {

#line  335 "cs.ATG" 
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		ModifierList m = new ModifierList();
		string qualident;
		
		if (la.kind == 88) {
			lexer.NextToken();

#line  341 "cs.ATG" 
			Location startPos = t.Location; 
			Qualident(
#line  342 "cs.ATG" 
out qualident);

#line  342 "cs.ATG" 
			INode node =  new NamespaceDeclaration(qualident);
			node.StartLocation = startPos;
			AddChild(node);
			BlockStart(node);
			
			Expect(16);
			while (la.kind == 71) {
				ExternAliasDirective();
			}
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

#line  352 "cs.ATG" 
			node.EndLocation   = t.EndLocation;
			BlockEnd();
			
		} else if (StartOf(2)) {
			while (la.kind == 18) {
				AttributeSection(
#line  356 "cs.ATG" 
out section);

#line  356 "cs.ATG" 
				attributes.Add(section); 
			}
			while (StartOf(3)) {
				TypeModifier(
#line  357 "cs.ATG" 
m);
			}
			TypeDecl(
#line  358 "cs.ATG" 
m, attributes);
		} else SynErr(148);
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
		case 145: {
			lexer.NextToken();
			break;
		}
		case 146: {
			lexer.NextToken();
			break;
		}
		default: SynErr(149); break;
		}
	}

	void Qualident(
#line  492 "cs.ATG" 
out string qualident) {
		Identifier();

#line  494 "cs.ATG" 
		qualidentBuilder.Length = 0; qualidentBuilder.Append(t.val); 
		while (
#line  495 "cs.ATG" 
DotAndIdent()) {
			Expect(15);
			Identifier();

#line  495 "cs.ATG" 
			qualidentBuilder.Append('.');
			qualidentBuilder.Append(t.val); 
			
		}

#line  498 "cs.ATG" 
		qualident = qualidentBuilder.ToString(); 
	}

	void NonArrayType(
#line  610 "cs.ATG" 
out TypeReference type) {

#line  612 "cs.ATG" 
		Location startPos = la.Location;
		string name;
		int pointer = 0;
		type = null;
		
		if (StartOf(4)) {
			ClassType(
#line  618 "cs.ATG" 
out type, false);
		} else if (StartOf(5)) {
			SimpleType(
#line  619 "cs.ATG" 
out name);

#line  619 "cs.ATG" 
			type = new TypeReference(name, true); 
		} else if (la.kind == 123) {
			lexer.NextToken();
			Expect(6);

#line  620 "cs.ATG" 
			pointer = 1; type = new TypeReference("System.Void", true); 
		} else SynErr(150);
		if (la.kind == 12) {
			NullableQuestionMark(
#line  623 "cs.ATG" 
ref type);
		}
		while (
#line  625 "cs.ATG" 
IsPointer()) {
			Expect(6);

#line  626 "cs.ATG" 
			++pointer; 
		}

#line  628 "cs.ATG" 
		if (type != null) {
		type.PointerNestingLevel = pointer; 
		type.EndLocation = t.EndLocation;
		type.StartLocation = startPos;
		} 
		
	}

	void Attribute(
#line  241 "cs.ATG" 
out ASTAttribute attribute) {

#line  242 "cs.ATG" 
		string qualident;
		string alias = null;
		

#line  246 "cs.ATG" 
		Location startPos = la.Location; 
		if (
#line  247 "cs.ATG" 
IdentAndDoubleColon()) {
			Identifier();

#line  248 "cs.ATG" 
			alias = t.val; 
			Expect(10);
		}
		Qualident(
#line  251 "cs.ATG" 
out qualident);

#line  252 "cs.ATG" 
		List<Expression> positional = new List<Expression>();
		List<NamedArgumentExpression> named = new List<NamedArgumentExpression>();
		string name = (alias != null && alias != "global") ? alias + "." + qualident : qualident;
		
		if (la.kind == 20) {
			AttributeArguments(
#line  256 "cs.ATG" 
positional, named);
		}

#line  257 "cs.ATG" 
		attribute = new ASTAttribute(name, positional, named); 
		attribute.StartLocation = startPos;
		attribute.EndLocation = t.EndLocation;
		
	}

	void AttributeArguments(
#line  263 "cs.ATG" 
List<Expression> positional, List<NamedArgumentExpression> named) {
		Expect(20);
		if (StartOf(6)) {
			AttributeArgument(
#line  267 "cs.ATG" 
positional, named);
			while (la.kind == 14) {
				lexer.NextToken();
				AttributeArgument(
#line  270 "cs.ATG" 
positional, named);
			}
		}
		Expect(21);
	}

	void AttributeArgument(
#line  276 "cs.ATG" 
List<Expression> positional, List<NamedArgumentExpression> named) {

#line  277 "cs.ATG" 
		string name = null; bool isNamed = false; Expression expr; Location startLocation = la.Location; 
		if (
#line  280 "cs.ATG" 
IsAssignment()) {

#line  280 "cs.ATG" 
			isNamed = true; 
			Identifier();

#line  281 "cs.ATG" 
			name = t.val; 
			Expect(3);
		} else if (
#line  284 "cs.ATG" 
IdentAndColon()) {
			Identifier();

#line  285 "cs.ATG" 
			name = t.val; 
			Expect(9);
		} else if (StartOf(6)) {
		} else SynErr(151);
		Expr(
#line  289 "cs.ATG" 
out expr);

#line  291 "cs.ATG" 
		if (expr != null) {
		if (isNamed) {
			named.Add(new NamedArgumentExpression(name, expr) { StartLocation = startLocation, EndLocation = t.EndLocation });
		} else {
			if (named.Count > 0)
				Error("positional argument after named argument is not allowed");
			if (name != null)
				expr = new NamedArgumentExpression(name, expr) { StartLocation = startLocation, EndLocation = t.EndLocation };
			positional.Add(expr);
		}
		}
		
	}

	void Expr(
#line  1808 "cs.ATG" 
out Expression expr) {

#line  1809 "cs.ATG" 
		expr = null; Expression expr1 = null, expr2 = null; AssignmentOperatorType op; 

#line  1811 "cs.ATG" 
		Location startLocation = la.Location; 
		UnaryExpr(
#line  1812 "cs.ATG" 
out expr);
		if (StartOf(7)) {
			AssignmentOperator(
#line  1815 "cs.ATG" 
out op);
			Expr(
#line  1815 "cs.ATG" 
out expr1);

#line  1815 "cs.ATG" 
			expr = new AssignmentExpression(expr, op, expr1); 
		} else if (
#line  1816 "cs.ATG" 
la.kind == Tokens.GreaterThan && Peek(1).kind == Tokens.GreaterEqual) {
			AssignmentOperator(
#line  1817 "cs.ATG" 
out op);
			Expr(
#line  1817 "cs.ATG" 
out expr1);

#line  1817 "cs.ATG" 
			expr = new AssignmentExpression(expr, op, expr1); 
		} else if (StartOf(8)) {
			ConditionalOrExpr(
#line  1819 "cs.ATG" 
ref expr);
			if (la.kind == 13) {
				lexer.NextToken();
				Expr(
#line  1820 "cs.ATG" 
out expr1);

#line  1820 "cs.ATG" 
				expr = new BinaryOperatorExpression(expr, BinaryOperatorType.NullCoalescing, expr1); 
			}
			if (la.kind == 12) {
				lexer.NextToken();
				Expr(
#line  1821 "cs.ATG" 
out expr1);
				Expect(9);
				Expr(
#line  1821 "cs.ATG" 
out expr2);

#line  1821 "cs.ATG" 
				expr = new ConditionalExpression(expr, expr1, expr2);  
			}
		} else SynErr(152);

#line  1824 "cs.ATG" 
		if (expr != null) {
		if (expr.StartLocation.IsEmpty)
			expr.StartLocation = startLocation;
		if (expr.EndLocation.IsEmpty)
			expr.EndLocation = t.EndLocation;
		}
		
	}

	void AttributeSection(
#line  305 "cs.ATG" 
out AttributeSection section) {

#line  307 "cs.ATG" 
		string attributeTarget = "";
		List<ASTAttribute> attributes = new List<ASTAttribute>();
		ASTAttribute attribute;
		
		
		Expect(18);

#line  313 "cs.ATG" 
		Location startPos = t.Location; 
		if (
#line  314 "cs.ATG" 
IsLocalAttrTarget()) {
			if (la.kind == 69) {
				lexer.NextToken();

#line  315 "cs.ATG" 
				attributeTarget = "event";
			} else if (la.kind == 101) {
				lexer.NextToken();

#line  316 "cs.ATG" 
				attributeTarget = "return";
			} else {
				Identifier();

#line  317 "cs.ATG" 
				attributeTarget = t.val; 
			}
			Expect(9);
		}
		Attribute(
#line  321 "cs.ATG" 
out attribute);

#line  321 "cs.ATG" 
		attributes.Add(attribute); 
		while (
#line  322 "cs.ATG" 
NotFinalComma()) {
			Expect(14);
			Attribute(
#line  322 "cs.ATG" 
out attribute);

#line  322 "cs.ATG" 
			attributes.Add(attribute); 
		}
		if (la.kind == 14) {
			lexer.NextToken();
		}
		Expect(19);

#line  324 "cs.ATG" 
		section = new AttributeSection {
		   AttributeTarget = attributeTarget,
		   Attributes = attributes,
		   StartLocation = startPos,
		   EndLocation = t.EndLocation
		};
		
	}

	void TypeModifier(
#line  695 "cs.ATG" 
ModifierList m) {
		switch (la.kind) {
		case 89: {
			lexer.NextToken();

#line  697 "cs.ATG" 
			m.Add(Modifiers.New, t.Location); 
			break;
		}
		case 98: {
			lexer.NextToken();

#line  698 "cs.ATG" 
			m.Add(Modifiers.Public, t.Location); 
			break;
		}
		case 97: {
			lexer.NextToken();

#line  699 "cs.ATG" 
			m.Add(Modifiers.Protected, t.Location); 
			break;
		}
		case 84: {
			lexer.NextToken();

#line  700 "cs.ATG" 
			m.Add(Modifiers.Internal, t.Location); 
			break;
		}
		case 96: {
			lexer.NextToken();

#line  701 "cs.ATG" 
			m.Add(Modifiers.Private, t.Location); 
			break;
		}
		case 119: {
			lexer.NextToken();

#line  702 "cs.ATG" 
			m.Add(Modifiers.Unsafe, t.Location); 
			break;
		}
		case 49: {
			lexer.NextToken();

#line  703 "cs.ATG" 
			m.Add(Modifiers.Abstract, t.Location); 
			break;
		}
		case 103: {
			lexer.NextToken();

#line  704 "cs.ATG" 
			m.Add(Modifiers.Sealed, t.Location); 
			break;
		}
		case 107: {
			lexer.NextToken();

#line  705 "cs.ATG" 
			m.Add(Modifiers.Static, t.Location); 
			break;
		}
		case 126: {
			lexer.NextToken();

#line  706 "cs.ATG" 
			m.Add(Modifiers.Partial, t.Location); 
			break;
		}
		default: SynErr(153); break;
		}
	}

	void TypeDecl(
#line  371 "cs.ATG" 
ModifierList m, List<AttributeSection> attributes) {

#line  373 "cs.ATG" 
		TypeReference type;
		List<TypeReference> names;
		List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
		string name;
		List<TemplateDefinition> templates;
		
		if (la.kind == 59) {

#line  379 "cs.ATG" 
			m.Check(Modifiers.Classes); 
			lexer.NextToken();

#line  380 "cs.ATG" 
			TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
			templates = newType.Templates;
			AddChild(newType);
			BlockStart(newType);
			newType.StartLocation = m.GetDeclarationLocation(t.Location);
			
			newType.Type = Types.Class;
			
			Identifier();

#line  388 "cs.ATG" 
			newType.Name = t.val; 
			if (la.kind == 23) {
				TypeParameterList(
#line  391 "cs.ATG" 
templates);
			}
			if (la.kind == 9) {
				ClassBase(
#line  393 "cs.ATG" 
out names);

#line  393 "cs.ATG" 
				newType.BaseTypes = names; 
			}
			while (la.kind == 127) {
				TypeParameterConstraintsClause(
#line  396 "cs.ATG" 
templates);
			}

#line  398 "cs.ATG" 
			newType.BodyStartLocation = t.EndLocation; 
			Expect(16);
			ClassBody();
			Expect(17);
			if (la.kind == 11) {
				lexer.NextToken();
			}

#line  402 "cs.ATG" 
			newType.EndLocation = t.EndLocation; 
			BlockEnd();
			
		} else if (StartOf(9)) {

#line  405 "cs.ATG" 
			m.Check(Modifiers.StructsInterfacesEnumsDelegates); 
			if (la.kind == 109) {
				lexer.NextToken();

#line  406 "cs.ATG" 
				TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
				templates = newType.Templates;
				newType.StartLocation = m.GetDeclarationLocation(t.Location);
				AddChild(newType);
				BlockStart(newType);
				newType.Type = Types.Struct; 
				
				Identifier();

#line  413 "cs.ATG" 
				newType.Name = t.val; 
				if (la.kind == 23) {
					TypeParameterList(
#line  416 "cs.ATG" 
templates);
				}
				if (la.kind == 9) {
					StructInterfaces(
#line  418 "cs.ATG" 
out names);

#line  418 "cs.ATG" 
					newType.BaseTypes = names; 
				}
				while (la.kind == 127) {
					TypeParameterConstraintsClause(
#line  421 "cs.ATG" 
templates);
				}

#line  424 "cs.ATG" 
				newType.BodyStartLocation = t.EndLocation; 
				StructBody();
				if (la.kind == 11) {
					lexer.NextToken();
				}

#line  426 "cs.ATG" 
				newType.EndLocation = t.EndLocation; 
				BlockEnd();
				
			} else if (la.kind == 83) {
				lexer.NextToken();

#line  430 "cs.ATG" 
				TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
				templates = newType.Templates;
				AddChild(newType);
				BlockStart(newType);
				newType.StartLocation = m.GetDeclarationLocation(t.Location);
				newType.Type = Types.Interface;
				
				Identifier();

#line  437 "cs.ATG" 
				newType.Name = t.val; 
				if (la.kind == 23) {
					TypeParameterList(
#line  440 "cs.ATG" 
templates);
				}
				if (la.kind == 9) {
					InterfaceBase(
#line  442 "cs.ATG" 
out names);

#line  442 "cs.ATG" 
					newType.BaseTypes = names; 
				}
				while (la.kind == 127) {
					TypeParameterConstraintsClause(
#line  445 "cs.ATG" 
templates);
				}

#line  447 "cs.ATG" 
				newType.BodyStartLocation = t.EndLocation; 
				InterfaceBody();
				if (la.kind == 11) {
					lexer.NextToken();
				}

#line  449 "cs.ATG" 
				newType.EndLocation = t.EndLocation; 
				BlockEnd();
				
			} else if (la.kind == 68) {
				lexer.NextToken();

#line  453 "cs.ATG" 
				TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
				AddChild(newType);
				BlockStart(newType);
				newType.StartLocation = m.GetDeclarationLocation(t.Location);
				newType.Type = Types.Enum;
				
				Identifier();

#line  459 "cs.ATG" 
				newType.Name = t.val; 
				if (la.kind == 9) {
					lexer.NextToken();
					IntegralType(
#line  460 "cs.ATG" 
out name);

#line  460 "cs.ATG" 
					newType.BaseTypes.Add(new TypeReference(name, true)); 
				}

#line  462 "cs.ATG" 
				newType.BodyStartLocation = t.EndLocation; 
				EnumBody();
				if (la.kind == 11) {
					lexer.NextToken();
				}

#line  464 "cs.ATG" 
				newType.EndLocation = t.EndLocation; 
				BlockEnd();
				
			} else {
				lexer.NextToken();

#line  468 "cs.ATG" 
				DelegateDeclaration delegateDeclr = new DelegateDeclaration(m.Modifier, attributes);
				templates = delegateDeclr.Templates;
				delegateDeclr.StartLocation = m.GetDeclarationLocation(t.Location);
				
				if (
#line  472 "cs.ATG" 
NotVoidPointer()) {
					Expect(123);

#line  472 "cs.ATG" 
					delegateDeclr.ReturnType = new TypeReference("System.Void", true); 
				} else if (StartOf(10)) {
					Type(
#line  473 "cs.ATG" 
out type);

#line  473 "cs.ATG" 
					delegateDeclr.ReturnType = type; 
				} else SynErr(154);
				Identifier();

#line  475 "cs.ATG" 
				delegateDeclr.Name = t.val; 
				if (la.kind == 23) {
					TypeParameterList(
#line  478 "cs.ATG" 
templates);
				}
				Expect(20);
				if (StartOf(11)) {
					FormalParameterList(
#line  480 "cs.ATG" 
p);

#line  480 "cs.ATG" 
					delegateDeclr.Parameters = p; 
				}
				Expect(21);
				while (la.kind == 127) {
					TypeParameterConstraintsClause(
#line  484 "cs.ATG" 
templates);
				}
				Expect(11);

#line  486 "cs.ATG" 
				delegateDeclr.EndLocation = t.EndLocation;
				AddChild(delegateDeclr);
				
			}
		} else SynErr(155);
	}

	void TypeParameterList(
#line  2397 "cs.ATG" 
List<TemplateDefinition> templates) {

#line  2399 "cs.ATG" 
		TemplateDefinition template;
		
		Expect(23);
		VariantTypeParameter(
#line  2403 "cs.ATG" 
out template);

#line  2403 "cs.ATG" 
		templates.Add(template); 
		while (la.kind == 14) {
			lexer.NextToken();
			VariantTypeParameter(
#line  2405 "cs.ATG" 
out template);

#line  2405 "cs.ATG" 
			templates.Add(template); 
		}
		Expect(22);
	}

	void ClassBase(
#line  501 "cs.ATG" 
out List<TypeReference> names) {

#line  503 "cs.ATG" 
		TypeReference typeRef;
		names = new List<TypeReference>();
		
		Expect(9);
		ClassType(
#line  507 "cs.ATG" 
out typeRef, false);

#line  507 "cs.ATG" 
		if (typeRef != null) { names.Add(typeRef); } 
		while (la.kind == 14) {
			lexer.NextToken();
			TypeName(
#line  508 "cs.ATG" 
out typeRef, false);

#line  508 "cs.ATG" 
			if (typeRef != null) { names.Add(typeRef); } 
		}
	}

	void TypeParameterConstraintsClause(
#line  2425 "cs.ATG" 
List<TemplateDefinition> templates) {

#line  2426 "cs.ATG" 
		string name = ""; TypeReference type; 
		Expect(127);
		Identifier();

#line  2429 "cs.ATG" 
		name = t.val; 
		Expect(9);
		TypeParameterConstraintsClauseBase(
#line  2431 "cs.ATG" 
out type);

#line  2432 "cs.ATG" 
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
#line  2441 "cs.ATG" 
out type);

#line  2442 "cs.ATG" 
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

#line  512 "cs.ATG" 
		AttributeSection section; 
		while (StartOf(12)) {

#line  514 "cs.ATG" 
			List<AttributeSection> attributes = new List<AttributeSection>();
			ModifierList m = new ModifierList();
			
			while (!(StartOf(13))) {SynErr(156); lexer.NextToken(); }
			while (la.kind == 18) {
				AttributeSection(
#line  518 "cs.ATG" 
out section);

#line  518 "cs.ATG" 
				attributes.Add(section); 
			}
			MemberModifiers(
#line  519 "cs.ATG" 
m);
			ClassMemberDecl(
#line  520 "cs.ATG" 
m, attributes);
		}
	}

	void StructInterfaces(
#line  524 "cs.ATG" 
out List<TypeReference> names) {

#line  526 "cs.ATG" 
		TypeReference typeRef;
		names = new List<TypeReference>();
		
		Expect(9);
		TypeName(
#line  530 "cs.ATG" 
out typeRef, false);

#line  530 "cs.ATG" 
		if (typeRef != null) { names.Add(typeRef); } 
		while (la.kind == 14) {
			lexer.NextToken();
			TypeName(
#line  531 "cs.ATG" 
out typeRef, false);

#line  531 "cs.ATG" 
			if (typeRef != null) { names.Add(typeRef); } 
		}
	}

	void StructBody() {

#line  535 "cs.ATG" 
		AttributeSection section; 
		Expect(16);
		while (StartOf(14)) {

#line  538 "cs.ATG" 
			List<AttributeSection> attributes = new List<AttributeSection>();
			ModifierList m = new ModifierList();
			
			while (la.kind == 18) {
				AttributeSection(
#line  541 "cs.ATG" 
out section);

#line  541 "cs.ATG" 
				attributes.Add(section); 
			}
			MemberModifiers(
#line  542 "cs.ATG" 
m);
			StructMemberDecl(
#line  543 "cs.ATG" 
m, attributes);
		}
		Expect(17);
	}

	void InterfaceBase(
#line  548 "cs.ATG" 
out List<TypeReference> names) {

#line  550 "cs.ATG" 
		TypeReference typeRef;
		names = new List<TypeReference>();
		
		Expect(9);
		TypeName(
#line  554 "cs.ATG" 
out typeRef, false);

#line  554 "cs.ATG" 
		if (typeRef != null) { names.Add(typeRef); } 
		while (la.kind == 14) {
			lexer.NextToken();
			TypeName(
#line  555 "cs.ATG" 
out typeRef, false);

#line  555 "cs.ATG" 
			if (typeRef != null) { names.Add(typeRef); } 
		}
	}

	void InterfaceBody() {
		Expect(16);
		while (StartOf(15)) {
			while (!(StartOf(16))) {SynErr(157); lexer.NextToken(); }
			InterfaceMemberDecl();
		}
		Expect(17);
	}

	void IntegralType(
#line  717 "cs.ATG" 
out string name) {

#line  717 "cs.ATG" 
		name = ""; 
		switch (la.kind) {
		case 102: {
			lexer.NextToken();

#line  719 "cs.ATG" 
			name = "System.SByte"; 
			break;
		}
		case 54: {
			lexer.NextToken();

#line  720 "cs.ATG" 
			name = "System.Byte"; 
			break;
		}
		case 104: {
			lexer.NextToken();

#line  721 "cs.ATG" 
			name = "System.Int16"; 
			break;
		}
		case 120: {
			lexer.NextToken();

#line  722 "cs.ATG" 
			name = "System.UInt16"; 
			break;
		}
		case 82: {
			lexer.NextToken();

#line  723 "cs.ATG" 
			name = "System.Int32"; 
			break;
		}
		case 116: {
			lexer.NextToken();

#line  724 "cs.ATG" 
			name = "System.UInt32"; 
			break;
		}
		case 87: {
			lexer.NextToken();

#line  725 "cs.ATG" 
			name = "System.Int64"; 
			break;
		}
		case 117: {
			lexer.NextToken();

#line  726 "cs.ATG" 
			name = "System.UInt64"; 
			break;
		}
		case 57: {
			lexer.NextToken();

#line  727 "cs.ATG" 
			name = "System.Char"; 
			break;
		}
		default: SynErr(158); break;
		}
	}

	void EnumBody() {

#line  564 "cs.ATG" 
		FieldDeclaration f; 
		Expect(16);
		if (StartOf(17)) {
			EnumMemberDecl(
#line  567 "cs.ATG" 
out f);

#line  567 "cs.ATG" 
			AddChild(f); 
			while (
#line  568 "cs.ATG" 
NotFinalComma()) {
				Expect(14);
				EnumMemberDecl(
#line  569 "cs.ATG" 
out f);

#line  569 "cs.ATG" 
				AddChild(f); 
			}
			if (la.kind == 14) {
				lexer.NextToken();
			}
		}
		Expect(17);
	}

	void Type(
#line  575 "cs.ATG" 
out TypeReference type) {
		TypeWithRestriction(
#line  577 "cs.ATG" 
out type, true, false);
	}

	void FormalParameterList(
#line  647 "cs.ATG" 
List<ParameterDeclarationExpression> parameter) {

#line  650 "cs.ATG" 
		ParameterDeclarationExpression p;
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		
		while (la.kind == 18) {
			AttributeSection(
#line  655 "cs.ATG" 
out section);

#line  655 "cs.ATG" 
			attributes.Add(section); 
		}
		FixedParameter(
#line  656 "cs.ATG" 
out p);

#line  656 "cs.ATG" 
		p.Attributes = attributes;
		parameter.Add(p);
		
		while (la.kind == 14) {
			lexer.NextToken();

#line  660 "cs.ATG" 
			attributes = new List<AttributeSection>(); 
			while (la.kind == 18) {
				AttributeSection(
#line  661 "cs.ATG" 
out section);

#line  661 "cs.ATG" 
				attributes.Add(section); 
			}
			FixedParameter(
#line  662 "cs.ATG" 
out p);

#line  662 "cs.ATG" 
			p.Attributes = attributes; parameter.Add(p); 
		}
	}

	void ClassType(
#line  709 "cs.ATG" 
out TypeReference typeRef, bool canBeUnbound) {

#line  710 "cs.ATG" 
		TypeReference r; typeRef = null; 
		if (StartOf(18)) {
			TypeName(
#line  712 "cs.ATG" 
out r, canBeUnbound);

#line  712 "cs.ATG" 
			typeRef = r; 
		} else if (la.kind == 91) {
			lexer.NextToken();

#line  713 "cs.ATG" 
			typeRef = new TypeReference("System.Object", true); typeRef.StartLocation = t.Location; typeRef.EndLocation = t.EndLocation; 
		} else if (la.kind == 108) {
			lexer.NextToken();

#line  714 "cs.ATG" 
			typeRef = new TypeReference("System.String", true); typeRef.StartLocation = t.Location; typeRef.EndLocation = t.EndLocation; 
		} else SynErr(159);
	}

	void TypeName(
#line  2338 "cs.ATG" 
out TypeReference typeRef, bool canBeUnbound) {

#line  2339 "cs.ATG" 
		List<TypeReference> typeArguments = null;
		string alias = null;
		string qualident;
		Location startLocation = la.Location;
		
		if (
#line  2345 "cs.ATG" 
IdentAndDoubleColon()) {
			Identifier();

#line  2346 "cs.ATG" 
			alias = t.val; 
			Expect(10);
		}
		Qualident(
#line  2349 "cs.ATG" 
out qualident);
		if (la.kind == 23) {
			TypeArgumentList(
#line  2350 "cs.ATG" 
out typeArguments, canBeUnbound);
		}

#line  2352 "cs.ATG" 
		if (alias == null) {
		typeRef = new TypeReference(qualident, typeArguments);
		} else if (alias == "global") {
			typeRef = new TypeReference(qualident, typeArguments);
			typeRef.IsGlobal = true;
		} else {
			typeRef = new TypeReference(alias + "." + qualident, typeArguments);
		}
		
		while (
#line  2361 "cs.ATG" 
DotAndIdent()) {
			Expect(15);

#line  2362 "cs.ATG" 
			typeArguments = null; 
			Qualident(
#line  2363 "cs.ATG" 
out qualident);
			if (la.kind == 23) {
				TypeArgumentList(
#line  2364 "cs.ATG" 
out typeArguments, canBeUnbound);
			}

#line  2365 "cs.ATG" 
			typeRef = new InnerClassTypeReference(typeRef, qualident, typeArguments); 
		}

#line  2367 "cs.ATG" 
		typeRef.StartLocation = startLocation; typeRef.EndLocation = t.EndLocation; 
	}

	void MemberModifiers(
#line  730 "cs.ATG" 
ModifierList m) {
		while (StartOf(19)) {
			switch (la.kind) {
			case 49: {
				lexer.NextToken();

#line  733 "cs.ATG" 
				m.Add(Modifiers.Abstract, t.Location); 
				break;
			}
			case 71: {
				lexer.NextToken();

#line  734 "cs.ATG" 
				m.Add(Modifiers.Extern, t.Location); 
				break;
			}
			case 84: {
				lexer.NextToken();

#line  735 "cs.ATG" 
				m.Add(Modifiers.Internal, t.Location); 
				break;
			}
			case 89: {
				lexer.NextToken();

#line  736 "cs.ATG" 
				m.Add(Modifiers.New, t.Location); 
				break;
			}
			case 94: {
				lexer.NextToken();

#line  737 "cs.ATG" 
				m.Add(Modifiers.Override, t.Location); 
				break;
			}
			case 96: {
				lexer.NextToken();

#line  738 "cs.ATG" 
				m.Add(Modifiers.Private, t.Location); 
				break;
			}
			case 97: {
				lexer.NextToken();

#line  739 "cs.ATG" 
				m.Add(Modifiers.Protected, t.Location); 
				break;
			}
			case 98: {
				lexer.NextToken();

#line  740 "cs.ATG" 
				m.Add(Modifiers.Public, t.Location); 
				break;
			}
			case 99: {
				lexer.NextToken();

#line  741 "cs.ATG" 
				m.Add(Modifiers.ReadOnly, t.Location); 
				break;
			}
			case 103: {
				lexer.NextToken();

#line  742 "cs.ATG" 
				m.Add(Modifiers.Sealed, t.Location); 
				break;
			}
			case 107: {
				lexer.NextToken();

#line  743 "cs.ATG" 
				m.Add(Modifiers.Static, t.Location); 
				break;
			}
			case 74: {
				lexer.NextToken();

#line  744 "cs.ATG" 
				m.Add(Modifiers.Fixed, t.Location); 
				break;
			}
			case 119: {
				lexer.NextToken();

#line  745 "cs.ATG" 
				m.Add(Modifiers.Unsafe, t.Location); 
				break;
			}
			case 122: {
				lexer.NextToken();

#line  746 "cs.ATG" 
				m.Add(Modifiers.Virtual, t.Location); 
				break;
			}
			case 124: {
				lexer.NextToken();

#line  747 "cs.ATG" 
				m.Add(Modifiers.Volatile, t.Location); 
				break;
			}
			case 126: {
				lexer.NextToken();

#line  748 "cs.ATG" 
				m.Add(Modifiers.Partial, t.Location); 
				break;
			}
			case 145: {
				lexer.NextToken();

#line  749 "cs.ATG" 
				m.Add(Modifiers.Async, t.Location); 
				break;
			}
			}
		}
	}

	void ClassMemberDecl(
#line  1084 "cs.ATG" 
ModifierList m, List<AttributeSection> attributes) {

#line  1085 "cs.ATG" 
		BlockStatement stmt = null; 
		if (StartOf(20)) {
			StructMemberDecl(
#line  1087 "cs.ATG" 
m, attributes);
		} else if (la.kind == 27) {

#line  1088 "cs.ATG" 
			m.Check(Modifiers.Destructors); Location startPos = la.Location; 
			lexer.NextToken();
			Identifier();

#line  1089 "cs.ATG" 
			DestructorDeclaration d = new DestructorDeclaration(t.val, m.Modifier, attributes); 
			d.Modifier = m.Modifier;
			d.StartLocation = m.GetDeclarationLocation(startPos);
			
			Expect(20);
			Expect(21);

#line  1093 "cs.ATG" 
			d.EndLocation = t.EndLocation; 
			if (la.kind == 16) {
				Block(
#line  1093 "cs.ATG" 
out stmt);
			} else if (la.kind == 11) {
				lexer.NextToken();
			} else SynErr(160);

#line  1094 "cs.ATG" 
			d.Body = stmt;
			AddChild(d);
			
		} else SynErr(161);
	}

	void StructMemberDecl(
#line  753 "cs.ATG" 
ModifierList m, List<AttributeSection> attributes) {

#line  755 "cs.ATG" 
		string qualident = null;
		TypeReference type;
		Expression expr;
		List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
		BlockStatement stmt = null;
		List<TemplateDefinition> templates = new List<TemplateDefinition>();
		TypeReference explicitInterface = null;
		bool isExtensionMethod = false;
		
		if (la.kind == 60) {

#line  765 "cs.ATG" 
			m.Check(Modifiers.Constants); 
			lexer.NextToken();

#line  766 "cs.ATG" 
			Location startPos = t.Location; 
			Type(
#line  767 "cs.ATG" 
out type);
			Identifier();

#line  767 "cs.ATG" 
			FieldDeclaration fd = new FieldDeclaration(attributes, type, m.Modifier | Modifiers.Const);
			fd.StartLocation = m.GetDeclarationLocation(startPos);
			VariableDeclaration f = new VariableDeclaration(t.val);
			f.StartLocation = t.Location;
			f.TypeReference = type;
			SafeAdd(fd, fd.Fields, f);
			
			Expect(3);
			Expr(
#line  774 "cs.ATG" 
out expr);

#line  774 "cs.ATG" 
			f.Initializer = expr; 
			while (la.kind == 14) {
				lexer.NextToken();
				Identifier();

#line  775 "cs.ATG" 
				f = new VariableDeclaration(t.val);
				f.StartLocation = t.Location;
				f.TypeReference = type;
				SafeAdd(fd, fd.Fields, f);
				
				Expect(3);
				Expr(
#line  780 "cs.ATG" 
out expr);

#line  780 "cs.ATG" 
				f.EndLocation = t.EndLocation; f.Initializer = expr; 
			}
			Expect(11);

#line  781 "cs.ATG" 
			fd.EndLocation = t.EndLocation; AddChild(fd); 
		} else if (
#line  785 "cs.ATG" 
NotVoidPointer()) {

#line  785 "cs.ATG" 
			m.Check(Modifiers.PropertysEventsMethods); 
			Expect(123);

#line  786 "cs.ATG" 
			Location startPos = t.Location; 
			if (
#line  787 "cs.ATG" 
IsExplicitInterfaceImplementation()) {
				TypeName(
#line  788 "cs.ATG" 
out explicitInterface, false);

#line  789 "cs.ATG" 
				if (la.kind != Tokens.Dot || Peek(1).kind != Tokens.This) {
				qualident = TypeReference.StripLastIdentifierFromType(ref explicitInterface);
				 } 
			} else if (StartOf(18)) {
				Identifier();

#line  792 "cs.ATG" 
				qualident = t.val; 
			} else SynErr(162);
			if (la.kind == 23) {
				TypeParameterList(
#line  795 "cs.ATG" 
templates);
			}
			Expect(20);
			if (la.kind == 111) {
				lexer.NextToken();

#line  798 "cs.ATG" 
				isExtensionMethod = true; /* C# 3.0 */ 
			}
			if (StartOf(11)) {
				FormalParameterList(
#line  799 "cs.ATG" 
p);
			}
			Expect(21);

#line  800 "cs.ATG" 
			MethodDeclaration methodDeclaration = new MethodDeclaration {
			Name = qualident,
			Modifier = m.Modifier,
			TypeReference = new TypeReference("System.Void", true),
			Parameters = p,
			Attributes = attributes,
			StartLocation = m.GetDeclarationLocation(startPos),
			EndLocation = t.EndLocation,
			Templates = templates,
			IsExtensionMethod = isExtensionMethod
			};
			if (explicitInterface != null)
				SafeAdd(methodDeclaration, methodDeclaration.InterfaceImplementations, new InterfaceImplementation(explicitInterface, qualident));
			AddChild(methodDeclaration);
			BlockStart(methodDeclaration);
			
			while (la.kind == 127) {
				TypeParameterConstraintsClause(
#line  818 "cs.ATG" 
templates);
			}
			if (la.kind == 16) {
				Block(
#line  820 "cs.ATG" 
out stmt);
			} else if (la.kind == 11) {
				lexer.NextToken();
			} else SynErr(163);

#line  820 "cs.ATG" 
			BlockEnd();
			methodDeclaration.Body  = (BlockStatement)stmt;
			
		} else if (la.kind == 69) {

#line  824 "cs.ATG" 
			m.Check(Modifiers.PropertysEventsMethods); 
			lexer.NextToken();

#line  826 "cs.ATG" 
			EventDeclaration eventDecl = new EventDeclaration {
			Modifier = m.Modifier, 
			Attributes = attributes,
			StartLocation = t.Location
			};
			AddChild(eventDecl);
			BlockStart(eventDecl);
			EventAddRegion addBlock = null;
			EventRemoveRegion removeBlock = null;
			
			Type(
#line  836 "cs.ATG" 
out type);

#line  836 "cs.ATG" 
			eventDecl.TypeReference = type; 
			if (
#line  837 "cs.ATG" 
IsExplicitInterfaceImplementation()) {
				TypeName(
#line  838 "cs.ATG" 
out explicitInterface, false);

#line  839 "cs.ATG" 
				qualident = TypeReference.StripLastIdentifierFromType(ref explicitInterface); 

#line  840 "cs.ATG" 
				eventDecl.InterfaceImplementations.Add(new InterfaceImplementation(explicitInterface, qualident)); 
			} else if (StartOf(18)) {
				Identifier();

#line  842 "cs.ATG" 
				qualident = t.val; 
				if (la.kind == 3) {
					lexer.NextToken();
					Expr(
#line  843 "cs.ATG" 
out expr);

#line  843 "cs.ATG" 
					eventDecl.Initializer = expr; 
				}
				while (la.kind == 14) {
					lexer.NextToken();

#line  847 "cs.ATG" 
					eventDecl.Name = qualident; eventDecl.EndLocation = t.EndLocation; BlockEnd(); 

#line  849 "cs.ATG" 
					eventDecl = new EventDeclaration {
					  Modifier = eventDecl.Modifier,
					  Attributes = eventDecl.Attributes,
					  StartLocation = eventDecl.StartLocation,
					  TypeReference = eventDecl.TypeReference.Clone()
					};
					AddChild(eventDecl);
					BlockStart(eventDecl);
					
					Identifier();

#line  858 "cs.ATG" 
					qualident = t.val; 
					if (la.kind == 3) {
						lexer.NextToken();
						Expr(
#line  859 "cs.ATG" 
out expr);

#line  859 "cs.ATG" 
						eventDecl.Initializer = expr; 
					}
				}
			} else SynErr(164);

#line  862 "cs.ATG" 
			eventDecl.Name = qualident; eventDecl.EndLocation = t.EndLocation; 
			if (la.kind == 16) {
				lexer.NextToken();

#line  863 "cs.ATG" 
				eventDecl.BodyStart = t.Location; 
				EventAccessorDecls(
#line  864 "cs.ATG" 
out addBlock, out removeBlock);
				Expect(17);

#line  865 "cs.ATG" 
				eventDecl.BodyEnd   = t.EndLocation; 
			}
			if (la.kind == 11) {
				lexer.NextToken();
			}

#line  868 "cs.ATG" 
			BlockEnd();
			eventDecl.AddRegion = addBlock;
			eventDecl.RemoveRegion = removeBlock;
			
		} else if (
#line  874 "cs.ATG" 
IdentAndLPar()) {

#line  874 "cs.ATG" 
			m.Check(Modifiers.Constructors | Modifiers.StaticConstructors); 
			Identifier();

#line  875 "cs.ATG" 
			string name = t.val; Location startPos = t.Location; 
			Expect(20);
			if (StartOf(11)) {

#line  875 "cs.ATG" 
				m.Check(Modifiers.Constructors); 
				FormalParameterList(
#line  876 "cs.ATG" 
p);
			}
			Expect(21);

#line  878 "cs.ATG" 
			ConstructorInitializer init = null;  
			if (la.kind == 9) {

#line  879 "cs.ATG" 
				m.Check(Modifiers.Constructors); 
				ConstructorInitializer(
#line  880 "cs.ATG" 
out init);
			}

#line  882 "cs.ATG" 
			ConstructorDeclaration cd = new ConstructorDeclaration(name, m.Modifier, p, init, attributes);
			cd.StartLocation = startPos;
			cd.EndLocation   = t.EndLocation;
			
			if (la.kind == 16) {
				Block(
#line  887 "cs.ATG" 
out stmt);
			} else if (la.kind == 11) {
				lexer.NextToken();
			} else SynErr(165);

#line  887 "cs.ATG" 
			cd.Body = (BlockStatement)stmt; AddChild(cd); 
		} else if (la.kind == 70 || la.kind == 80) {

#line  890 "cs.ATG" 
			m.Check(Modifiers.Operators);
			if (m.isNone) Error("at least one modifier must be set"); 
			bool isImplicit = true;
			Location startPos = Location.Empty;
			
			if (la.kind == 80) {
				lexer.NextToken();

#line  895 "cs.ATG" 
				startPos = t.Location; 
			} else {
				lexer.NextToken();

#line  895 "cs.ATG" 
				isImplicit = false; startPos = t.Location; 
			}
			Expect(92);
			Type(
#line  896 "cs.ATG" 
out type);

#line  896 "cs.ATG" 
			TypeReference operatorType = type; 
			Expect(20);
			Type(
#line  897 "cs.ATG" 
out type);
			Identifier();

#line  897 "cs.ATG" 
			string varName = t.val; 
			Expect(21);

#line  898 "cs.ATG" 
			Location endPos = t.Location; 
			if (la.kind == 16) {
				Block(
#line  899 "cs.ATG" 
out stmt);
			} else if (la.kind == 11) {
				lexer.NextToken();

#line  899 "cs.ATG" 
				stmt = null; 
			} else SynErr(166);

#line  902 "cs.ATG" 
			List<ParameterDeclarationExpression> parameters = new List<ParameterDeclarationExpression>();
			parameters.Add(new ParameterDeclarationExpression(type, varName));
			OperatorDeclaration operatorDeclaration = new OperatorDeclaration {
				Name = (isImplicit ? "op_Implicit" : "op_Explicit"),
				Modifier = m.Modifier,
				Attributes = attributes, 
				Parameters = parameters, 
				TypeReference = operatorType,
				ConversionType = isImplicit ? ConversionType.Implicit : ConversionType.Explicit,
				Body = (BlockStatement)stmt,
				StartLocation = m.GetDeclarationLocation(startPos),
				EndLocation = endPos
			};
			AddChild(operatorDeclaration);
			
		} else if (StartOf(21)) {
			TypeDecl(
#line  920 "cs.ATG" 
m, attributes);
		} else if (StartOf(10)) {
			Type(
#line  922 "cs.ATG" 
out type);

#line  922 "cs.ATG" 
			Location startPos = t.Location;  
			if (la.kind == 92) {

#line  924 "cs.ATG" 
				OverloadableOperatorType op;
				m.Check(Modifiers.Operators);
				if (m.isNone) Error("at least one modifier must be set");
				
				lexer.NextToken();
				OverloadableOperator(
#line  928 "cs.ATG" 
out op);

#line  928 "cs.ATG" 
				TypeReference firstType, secondType = null; string secondName = null; 
				Expect(20);

#line  929 "cs.ATG" 
				Location firstStart = la.Location, secondStart = Location.Empty, secondEnd = Location.Empty; 
				Type(
#line  929 "cs.ATG" 
out firstType);
				Identifier();

#line  929 "cs.ATG" 
				string firstName = t.val; Location firstEnd = t.EndLocation; 
				if (la.kind == 14) {
					lexer.NextToken();

#line  930 "cs.ATG" 
					secondStart = la.Location; 
					Type(
#line  930 "cs.ATG" 
out secondType);
					Identifier();

#line  930 "cs.ATG" 
					secondName = t.val; secondEnd = t.EndLocation; 
				} else if (la.kind == 21) {
				} else SynErr(167);

#line  938 "cs.ATG" 
				Location endPos = t.Location; 
				Expect(21);
				if (la.kind == 16) {
					Block(
#line  939 "cs.ATG" 
out stmt);
				} else if (la.kind == 11) {
					lexer.NextToken();
				} else SynErr(168);

#line  941 "cs.ATG" 
				if (op == OverloadableOperatorType.Add && secondType == null)
				op = OverloadableOperatorType.UnaryPlus;
				if (op == OverloadableOperatorType.Subtract && secondType == null)
					op = OverloadableOperatorType.UnaryMinus;
				OperatorDeclaration operatorDeclaration = new OperatorDeclaration {
					Modifier = m.Modifier,
					Attributes = attributes,
					TypeReference = type,
					OverloadableOperator = op,
					Name = GetReflectionNameForOperator(op),
					Body = (BlockStatement)stmt,
					StartLocation = m.GetDeclarationLocation(startPos),
					EndLocation = endPos
				};
				SafeAdd(operatorDeclaration, operatorDeclaration.Parameters, new ParameterDeclarationExpression(firstType, firstName) { StartLocation = firstStart, EndLocation = firstEnd });
				if (secondType != null) {
					SafeAdd(operatorDeclaration, operatorDeclaration.Parameters, new ParameterDeclarationExpression(secondType, secondName) { StartLocation = secondStart, EndLocation = secondEnd });
				}
				AddChild(operatorDeclaration);
				
			} else if (
#line  963 "cs.ATG" 
IsVarDecl()) {

#line  964 "cs.ATG" 
				m.Check(Modifiers.Fields);
				FieldDeclaration fd = new FieldDeclaration(attributes, type, m.Modifier);
				fd.StartLocation = m.GetDeclarationLocation(startPos); 
				
				if (
#line  968 "cs.ATG" 
m.Contains(Modifiers.Fixed)) {
					VariableDeclarator(
#line  969 "cs.ATG" 
fd);
					Expect(18);
					Expr(
#line  971 "cs.ATG" 
out expr);

#line  971 "cs.ATG" 
					if (fd.Fields.Count > 0)
					fd.Fields[fd.Fields.Count-1].FixedArrayInitialization = expr; 
					Expect(19);
					while (la.kind == 14) {
						lexer.NextToken();
						VariableDeclarator(
#line  975 "cs.ATG" 
fd);
						Expect(18);
						Expr(
#line  977 "cs.ATG" 
out expr);

#line  977 "cs.ATG" 
						if (fd.Fields.Count > 0)
						fd.Fields[fd.Fields.Count-1].FixedArrayInitialization = expr; 
						Expect(19);
					}
				} else if (StartOf(18)) {
					VariableDeclarator(
#line  982 "cs.ATG" 
fd);
					while (la.kind == 14) {
						lexer.NextToken();
						VariableDeclarator(
#line  983 "cs.ATG" 
fd);
					}
				} else SynErr(169);
				Expect(11);

#line  985 "cs.ATG" 
				fd.EndLocation = t.EndLocation; AddChild(fd); 
			} else if (la.kind == 111) {

#line  988 "cs.ATG" 
				m.Check(Modifiers.Indexers); 
				lexer.NextToken();
				Expect(18);
				FormalParameterList(
#line  989 "cs.ATG" 
p);
				Expect(19);

#line  989 "cs.ATG" 
				Location endLocation = t.EndLocation; 
				Expect(16);

#line  990 "cs.ATG" 
				PropertyDeclaration indexer = new PropertyDeclaration(m.Modifier | Modifiers.Default, attributes, "Item", p);
				indexer.StartLocation = startPos;
				indexer.EndLocation   = endLocation;
				indexer.BodyStart     = t.Location;
				indexer.TypeReference = type;
				PropertyGetRegion getRegion;
				PropertySetRegion setRegion;
				
				AccessorDecls(
#line  998 "cs.ATG" 
out getRegion, out setRegion);
				Expect(17);

#line  999 "cs.ATG" 
				indexer.BodyEnd    = t.EndLocation;
				indexer.GetRegion = getRegion;
				indexer.SetRegion = setRegion;
				AddChild(indexer);
				
			} else if (
#line  1004 "cs.ATG" 
IsIdentifierToken(la)) {
				if (
#line  1005 "cs.ATG" 
IsExplicitInterfaceImplementation()) {
					TypeName(
#line  1006 "cs.ATG" 
out explicitInterface, false);

#line  1007 "cs.ATG" 
					if (la.kind != Tokens.Dot || Peek(1).kind != Tokens.This) {
					qualident = TypeReference.StripLastIdentifierFromType(ref explicitInterface);
					 } 
				} else if (StartOf(18)) {
					Identifier();

#line  1010 "cs.ATG" 
					qualident = t.val; 
				} else SynErr(170);

#line  1012 "cs.ATG" 
				Location qualIdentEndLocation = t.EndLocation; 
				if (la.kind == 16 || la.kind == 20 || la.kind == 23) {
					if (la.kind == 20 || la.kind == 23) {

#line  1016 "cs.ATG" 
						m.Check(Modifiers.PropertysEventsMethods); 
						if (la.kind == 23) {
							TypeParameterList(
#line  1018 "cs.ATG" 
templates);
						}
						Expect(20);
						if (la.kind == 111) {
							lexer.NextToken();

#line  1020 "cs.ATG" 
							isExtensionMethod = true; 
						}
						if (StartOf(11)) {
							FormalParameterList(
#line  1021 "cs.ATG" 
p);
						}
						Expect(21);

#line  1023 "cs.ATG" 
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
						AddChild(methodDeclaration);
						                                      
						while (la.kind == 127) {
							TypeParameterConstraintsClause(
#line  1038 "cs.ATG" 
templates);
						}
						if (la.kind == 16) {
							Block(
#line  1039 "cs.ATG" 
out stmt);
						} else if (la.kind == 11) {
							lexer.NextToken();
						} else SynErr(171);

#line  1039 "cs.ATG" 
						methodDeclaration.Body  = (BlockStatement)stmt; 
					} else {
						lexer.NextToken();

#line  1042 "cs.ATG" 
						PropertyDeclaration pDecl = new PropertyDeclaration(qualident, type, m.Modifier, attributes); 
						if (explicitInterface != null)
						pDecl.InterfaceImplementations.Add(new InterfaceImplementation(explicitInterface, qualident));
						      pDecl.StartLocation = m.GetDeclarationLocation(startPos);
						      pDecl.EndLocation   = qualIdentEndLocation;
						      pDecl.BodyStart   = t.Location;
						      PropertyGetRegion getRegion;
						      PropertySetRegion setRegion;
						   
						AccessorDecls(
#line  1051 "cs.ATG" 
out getRegion, out setRegion);
						Expect(17);

#line  1053 "cs.ATG" 
						pDecl.GetRegion = getRegion;
						pDecl.SetRegion = setRegion;
						pDecl.BodyEnd = t.EndLocation;
						AddChild(pDecl);
						
					}
				} else if (la.kind == 15) {

#line  1061 "cs.ATG" 
					m.Check(Modifiers.Indexers); 
					lexer.NextToken();
					Expect(111);
					Expect(18);
					FormalParameterList(
#line  1062 "cs.ATG" 
p);
					Expect(19);

#line  1063 "cs.ATG" 
					PropertyDeclaration indexer = new PropertyDeclaration(m.Modifier | Modifiers.Default, attributes, "Item", p);
					indexer.StartLocation = m.GetDeclarationLocation(startPos);
					indexer.EndLocation   = t.EndLocation;
					indexer.TypeReference = type;
					if (explicitInterface != null)
					SafeAdd(indexer, indexer.InterfaceImplementations, new InterfaceImplementation(explicitInterface, "this"));
					      PropertyGetRegion getRegion;
					      PropertySetRegion setRegion;
					    
					Expect(16);

#line  1072 "cs.ATG" 
					Location bodyStart = t.Location; 
					AccessorDecls(
#line  1073 "cs.ATG" 
out getRegion, out setRegion);
					Expect(17);

#line  1074 "cs.ATG" 
					indexer.BodyStart = bodyStart;
					indexer.BodyEnd   = t.EndLocation;
					indexer.GetRegion = getRegion;
					indexer.SetRegion = setRegion;
					AddChild(indexer);
					
				} else SynErr(172);
			} else SynErr(173);
		} else SynErr(174);
	}

	void InterfaceMemberDecl() {

#line  1101 "cs.ATG" 
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
#line  1114 "cs.ATG" 
out section);

#line  1114 "cs.ATG" 
			attributes.Add(section); 
		}
		if (la.kind == 89) {
			lexer.NextToken();

#line  1115 "cs.ATG" 
			mod = Modifiers.New; startLocation = t.Location; 
		}
		if (
#line  1118 "cs.ATG" 
NotVoidPointer()) {
			Expect(123);

#line  1118 "cs.ATG" 
			if (startLocation.IsEmpty) startLocation = t.Location; 
			Identifier();

#line  1119 "cs.ATG" 
			name = t.val; 
			if (la.kind == 23) {
				TypeParameterList(
#line  1120 "cs.ATG" 
templates);
			}
			Expect(20);
			if (StartOf(11)) {
				FormalParameterList(
#line  1121 "cs.ATG" 
parameters);
			}
			Expect(21);
			while (la.kind == 127) {
				TypeParameterConstraintsClause(
#line  1122 "cs.ATG" 
templates);
			}
			Expect(11);

#line  1124 "cs.ATG" 
			MethodDeclaration md = new MethodDeclaration {
			Name = name, Modifier = mod, TypeReference = new TypeReference("System.Void", true), 
			Parameters = parameters, Attributes = attributes, Templates = templates,
			StartLocation = startLocation, EndLocation = t.EndLocation
			};
			AddChild(md);
			
		} else if (StartOf(22)) {
			if (StartOf(10)) {
				Type(
#line  1132 "cs.ATG" 
out type);

#line  1132 "cs.ATG" 
				if (startLocation.IsEmpty) startLocation = t.Location; 
				if (StartOf(18)) {
					Identifier();

#line  1134 "cs.ATG" 
					name = t.val; Location qualIdentEndLocation = t.EndLocation; 
					if (la.kind == 20 || la.kind == 23) {
						if (la.kind == 23) {
							TypeParameterList(
#line  1138 "cs.ATG" 
templates);
						}
						Expect(20);
						if (StartOf(11)) {
							FormalParameterList(
#line  1139 "cs.ATG" 
parameters);
						}
						Expect(21);
						while (la.kind == 127) {
							TypeParameterConstraintsClause(
#line  1141 "cs.ATG" 
templates);
						}
						Expect(11);

#line  1142 "cs.ATG" 
						MethodDeclaration md = new MethodDeclaration {
						Name = name, Modifier = mod, TypeReference = type,
						Parameters = parameters, Attributes = attributes, Templates = templates,
						StartLocation = startLocation, EndLocation = t.EndLocation
						};
						AddChild(md);
						
					} else if (la.kind == 16) {

#line  1151 "cs.ATG" 
						PropertyDeclaration pd = new PropertyDeclaration(name, type, mod, attributes);
						AddChild(pd); 
						lexer.NextToken();

#line  1154 "cs.ATG" 
						Location bodyStart = t.Location;
						InterfaceAccessors(
#line  1155 "cs.ATG" 
out getBlock, out setBlock);
						Expect(17);

#line  1156 "cs.ATG" 
						pd.GetRegion = getBlock; pd.SetRegion = setBlock; pd.StartLocation = startLocation; pd.EndLocation = qualIdentEndLocation; pd.BodyStart = bodyStart; pd.BodyEnd = t.EndLocation; 
					} else SynErr(175);
				} else if (la.kind == 111) {
					lexer.NextToken();
					Expect(18);
					FormalParameterList(
#line  1159 "cs.ATG" 
parameters);
					Expect(19);

#line  1160 "cs.ATG" 
					Location bracketEndLocation = t.EndLocation; 

#line  1161 "cs.ATG" 
					PropertyDeclaration id = new PropertyDeclaration(mod | Modifiers.Default, attributes, "Item", parameters);
					id.TypeReference = type;
					  AddChild(id); 
					Expect(16);

#line  1164 "cs.ATG" 
					Location bodyStart = t.Location;
					InterfaceAccessors(
#line  1165 "cs.ATG" 
out getBlock, out setBlock);
					Expect(17);

#line  1167 "cs.ATG" 
					id.GetRegion = getBlock; id.SetRegion = setBlock; id.StartLocation = startLocation;  id.EndLocation = bracketEndLocation; id.BodyStart = bodyStart; id.BodyEnd = t.EndLocation;
				} else SynErr(176);
			} else {
				lexer.NextToken();

#line  1170 "cs.ATG" 
				if (startLocation.IsEmpty) startLocation = t.Location; 
				Type(
#line  1171 "cs.ATG" 
out type);
				Identifier();

#line  1172 "cs.ATG" 
				EventDeclaration ed = new EventDeclaration {
				TypeReference = type, Name = t.val, Modifier = mod, Attributes = attributes
				};
				AddChild(ed);
				
				Expect(11);

#line  1178 "cs.ATG" 
				ed.StartLocation = startLocation; ed.EndLocation = t.EndLocation; 
			}
		} else SynErr(177);
	}

	void EnumMemberDecl(
#line  1183 "cs.ATG" 
out FieldDeclaration f) {

#line  1185 "cs.ATG" 
		Expression expr = null;
		List<AttributeSection> attributes = new List<AttributeSection>();
		AttributeSection section = null;
		VariableDeclaration varDecl = null;
		
		while (la.kind == 18) {
			AttributeSection(
#line  1191 "cs.ATG" 
out section);

#line  1191 "cs.ATG" 
			attributes.Add(section); 
		}
		Identifier();

#line  1192 "cs.ATG" 
		f = new FieldDeclaration(attributes);
		varDecl         = new VariableDeclaration(t.val);
		f.Fields.Add(varDecl);
		f.StartLocation = t.Location;
		f.EndLocation = t.EndLocation;
		
		if (la.kind == 3) {
			lexer.NextToken();
			Expr(
#line  1198 "cs.ATG" 
out expr);

#line  1198 "cs.ATG" 
			varDecl.Initializer = expr; 
		}
	}

	void TypeWithRestriction(
#line  580 "cs.ATG" 
out TypeReference type, bool allowNullable, bool canBeUnbound) {

#line  582 "cs.ATG" 
		Location startPos = la.Location;
		string name;
		int pointer = 0;
		type = null;
		
		if (StartOf(4)) {
			ClassType(
#line  588 "cs.ATG" 
out type, canBeUnbound);
		} else if (StartOf(5)) {
			SimpleType(
#line  589 "cs.ATG" 
out name);

#line  589 "cs.ATG" 
			type = new TypeReference(name, true); type.StartLocation = startPos; type.EndLocation = t.EndLocation; 
		} else if (la.kind == 123) {
			lexer.NextToken();
			Expect(6);

#line  590 "cs.ATG" 
			pointer = 1; type = new TypeReference("System.Void", true); type.StartLocation = startPos; type.EndLocation = t.EndLocation; 
		} else SynErr(178);

#line  591 "cs.ATG" 
		List<int> r = new List<int>(); 
		if (
#line  593 "cs.ATG" 
allowNullable && la.kind == Tokens.Question) {
			NullableQuestionMark(
#line  593 "cs.ATG" 
ref type);
		}
		while (
#line  595 "cs.ATG" 
IsPointerOrDims()) {

#line  595 "cs.ATG" 
			int i = 0; 
			if (la.kind == 6) {
				lexer.NextToken();

#line  596 "cs.ATG" 
				++pointer; 
			} else if (la.kind == 18) {
				lexer.NextToken();
				while (la.kind == 14) {
					lexer.NextToken();

#line  597 "cs.ATG" 
					++i; 
				}
				Expect(19);

#line  597 "cs.ATG" 
				r.Add(i); 
			} else SynErr(179);
		}

#line  600 "cs.ATG" 
		if (type != null) {
		type.RankSpecifier = r.ToArray();
		type.PointerNestingLevel = pointer;
		type.EndLocation = t.EndLocation;
		type.StartLocation = startPos;
		}
		
	}

	void SimpleType(
#line  636 "cs.ATG" 
out string name) {

#line  637 "cs.ATG" 
		name = String.Empty; 
		if (StartOf(23)) {
			IntegralType(
#line  639 "cs.ATG" 
out name);
		} else if (la.kind == 75) {
			lexer.NextToken();

#line  640 "cs.ATG" 
			name = "System.Single"; 
		} else if (la.kind == 66) {
			lexer.NextToken();

#line  641 "cs.ATG" 
			name = "System.Double"; 
		} else if (la.kind == 62) {
			lexer.NextToken();

#line  642 "cs.ATG" 
			name = "System.Decimal"; 
		} else if (la.kind == 52) {
			lexer.NextToken();

#line  643 "cs.ATG" 
			name = "System.Boolean"; 
		} else SynErr(180);
	}

	void NullableQuestionMark(
#line  2371 "cs.ATG" 
ref TypeReference typeRef) {

#line  2372 "cs.ATG" 
		List<TypeReference> typeArguments = new List<TypeReference>(1); 
		Expect(12);

#line  2376 "cs.ATG" 
		if (typeRef != null) typeArguments.Add(typeRef);
		typeRef = new TypeReference("System.Nullable", typeArguments) { IsKeyword = true };
		
	}

	void FixedParameter(
#line  666 "cs.ATG" 
out ParameterDeclarationExpression p) {

#line  668 "cs.ATG" 
		TypeReference type;
		ParameterModifiers mod = ParameterModifiers.In;
		Location start = la.Location;
		Expression expr;
		
		if (la.kind == 93 || la.kind == 95 || la.kind == 100) {
			if (la.kind == 100) {
				lexer.NextToken();

#line  675 "cs.ATG" 
				mod = ParameterModifiers.Ref; 
			} else if (la.kind == 93) {
				lexer.NextToken();

#line  676 "cs.ATG" 
				mod = ParameterModifiers.Out; 
			} else {
				lexer.NextToken();

#line  677 "cs.ATG" 
				mod = ParameterModifiers.Params; 
			}
		}
		Type(
#line  679 "cs.ATG" 
out type);
		Identifier();

#line  680 "cs.ATG" 
		p = new ParameterDeclarationExpression(type, t.val, mod); 
		if (la.kind == 3) {
			lexer.NextToken();
			Expr(
#line  681 "cs.ATG" 
out expr);

#line  681 "cs.ATG" 
			p.DefaultValue = expr; p.ParamModifier |= ParameterModifiers.Optional; 
		}

#line  682 "cs.ATG" 
		p.StartLocation = start; p.EndLocation = t.EndLocation; 
	}

	void AccessorModifiers(
#line  685 "cs.ATG" 
out ModifierList m) {

#line  686 "cs.ATG" 
		m = new ModifierList(); 
		if (la.kind == 96) {
			lexer.NextToken();

#line  688 "cs.ATG" 
			m.Add(Modifiers.Private, t.Location); 
		} else if (la.kind == 97) {
			lexer.NextToken();

#line  689 "cs.ATG" 
			m.Add(Modifiers.Protected, t.Location); 
			if (la.kind == 84) {
				lexer.NextToken();

#line  690 "cs.ATG" 
				m.Add(Modifiers.Internal, t.Location); 
			}
		} else if (la.kind == 84) {
			lexer.NextToken();

#line  691 "cs.ATG" 
			m.Add(Modifiers.Internal, t.Location); 
			if (la.kind == 97) {
				lexer.NextToken();

#line  692 "cs.ATG" 
				m.Add(Modifiers.Protected, t.Location); 
			}
		} else SynErr(181);
	}

	void Block(
#line  1318 "cs.ATG" 
out BlockStatement stmt) {
		Expect(16);

#line  1320 "cs.ATG" 
		BlockStatement blockStmt = new BlockStatement();
		blockStmt.StartLocation = t.Location;
		BlockStart(blockStmt);
		if (!ParseMethodBodies) lexer.SkipCurrentBlock(0);
		
		while (StartOf(24)) {
			Statement();
		}
		while (!(la.kind == 0 || la.kind == 17)) {SynErr(182); lexer.NextToken(); }
		Expect(17);

#line  1328 "cs.ATG" 
		stmt = blockStmt;
		blockStmt.EndLocation = t.EndLocation;
		BlockEnd();
		
	}

	void EventAccessorDecls(
#line  1255 "cs.ATG" 
out EventAddRegion addBlock, out EventRemoveRegion removeBlock) {

#line  1256 "cs.ATG" 
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		BlockStatement stmt;
		addBlock = null;
		removeBlock = null;
		
		while (la.kind == 18) {
			AttributeSection(
#line  1263 "cs.ATG" 
out section);

#line  1263 "cs.ATG" 
			attributes.Add(section); 
		}
		if (la.kind == 130) {

#line  1265 "cs.ATG" 
			addBlock = new EventAddRegion(attributes); 
			AddAccessorDecl(
#line  1266 "cs.ATG" 
out stmt);

#line  1266 "cs.ATG" 
			attributes = new List<AttributeSection>(); addBlock.Block = stmt; 
			while (la.kind == 18) {
				AttributeSection(
#line  1267 "cs.ATG" 
out section);

#line  1267 "cs.ATG" 
				attributes.Add(section); 
			}
			RemoveAccessorDecl(
#line  1268 "cs.ATG" 
out stmt);

#line  1268 "cs.ATG" 
			removeBlock = new EventRemoveRegion(attributes); removeBlock.Block = stmt; 
		} else if (la.kind == 131) {
			RemoveAccessorDecl(
#line  1270 "cs.ATG" 
out stmt);

#line  1270 "cs.ATG" 
			removeBlock = new EventRemoveRegion(attributes); removeBlock.Block = stmt; attributes = new List<AttributeSection>(); 
			while (la.kind == 18) {
				AttributeSection(
#line  1271 "cs.ATG" 
out section);

#line  1271 "cs.ATG" 
				attributes.Add(section); 
			}
			AddAccessorDecl(
#line  1272 "cs.ATG" 
out stmt);

#line  1272 "cs.ATG" 
			addBlock = new EventAddRegion(attributes); addBlock.Block = stmt; 
		} else SynErr(183);
	}

	void ConstructorInitializer(
#line  1348 "cs.ATG" 
out ConstructorInitializer ci) {

#line  1349 "cs.ATG" 
		Expression expr; ci = new ConstructorInitializer(); 
		Expect(9);
		if (la.kind == 51) {
			lexer.NextToken();

#line  1353 "cs.ATG" 
			ci.ConstructorInitializerType = ConstructorInitializerType.Base; 
		} else if (la.kind == 111) {
			lexer.NextToken();

#line  1354 "cs.ATG" 
			ci.ConstructorInitializerType = ConstructorInitializerType.This; 
		} else SynErr(184);
		Expect(20);
		if (StartOf(25)) {
			Argument(
#line  1357 "cs.ATG" 
out expr);

#line  1357 "cs.ATG" 
			SafeAdd(ci, ci.Arguments, expr); 
			while (la.kind == 14) {
				lexer.NextToken();
				Argument(
#line  1358 "cs.ATG" 
out expr);

#line  1358 "cs.ATG" 
				SafeAdd(ci, ci.Arguments, expr); 
			}
		}
		Expect(21);
	}

	void OverloadableOperator(
#line  1371 "cs.ATG" 
out OverloadableOperatorType op) {

#line  1372 "cs.ATG" 
		op = OverloadableOperatorType.None; 
		switch (la.kind) {
		case 4: {
			lexer.NextToken();

#line  1374 "cs.ATG" 
			op = OverloadableOperatorType.Add; 
			break;
		}
		case 5: {
			lexer.NextToken();

#line  1375 "cs.ATG" 
			op = OverloadableOperatorType.Subtract; 
			break;
		}
		case 24: {
			lexer.NextToken();

#line  1377 "cs.ATG" 
			op = OverloadableOperatorType.Not; 
			break;
		}
		case 27: {
			lexer.NextToken();

#line  1378 "cs.ATG" 
			op = OverloadableOperatorType.BitNot; 
			break;
		}
		case 31: {
			lexer.NextToken();

#line  1380 "cs.ATG" 
			op = OverloadableOperatorType.Increment; 
			break;
		}
		case 32: {
			lexer.NextToken();

#line  1381 "cs.ATG" 
			op = OverloadableOperatorType.Decrement; 
			break;
		}
		case 113: {
			lexer.NextToken();

#line  1383 "cs.ATG" 
			op = OverloadableOperatorType.IsTrue; 
			break;
		}
		case 72: {
			lexer.NextToken();

#line  1384 "cs.ATG" 
			op = OverloadableOperatorType.IsFalse; 
			break;
		}
		case 6: {
			lexer.NextToken();

#line  1386 "cs.ATG" 
			op = OverloadableOperatorType.Multiply; 
			break;
		}
		case 7: {
			lexer.NextToken();

#line  1387 "cs.ATG" 
			op = OverloadableOperatorType.Divide; 
			break;
		}
		case 8: {
			lexer.NextToken();

#line  1388 "cs.ATG" 
			op = OverloadableOperatorType.Modulus; 
			break;
		}
		case 28: {
			lexer.NextToken();

#line  1390 "cs.ATG" 
			op = OverloadableOperatorType.BitwiseAnd; 
			break;
		}
		case 29: {
			lexer.NextToken();

#line  1391 "cs.ATG" 
			op = OverloadableOperatorType.BitwiseOr; 
			break;
		}
		case 30: {
			lexer.NextToken();

#line  1392 "cs.ATG" 
			op = OverloadableOperatorType.ExclusiveOr; 
			break;
		}
		case 37: {
			lexer.NextToken();

#line  1394 "cs.ATG" 
			op = OverloadableOperatorType.ShiftLeft; 
			break;
		}
		case 33: {
			lexer.NextToken();

#line  1395 "cs.ATG" 
			op = OverloadableOperatorType.Equality; 
			break;
		}
		case 34: {
			lexer.NextToken();

#line  1396 "cs.ATG" 
			op = OverloadableOperatorType.InEquality; 
			break;
		}
		case 23: {
			lexer.NextToken();

#line  1397 "cs.ATG" 
			op = OverloadableOperatorType.LessThan; 
			break;
		}
		case 35: {
			lexer.NextToken();

#line  1398 "cs.ATG" 
			op = OverloadableOperatorType.GreaterThanOrEqual; 
			break;
		}
		case 36: {
			lexer.NextToken();

#line  1399 "cs.ATG" 
			op = OverloadableOperatorType.LessThanOrEqual; 
			break;
		}
		case 22: {
			lexer.NextToken();

#line  1400 "cs.ATG" 
			op = OverloadableOperatorType.GreaterThan; 
			if (la.kind == 22) {
				lexer.NextToken();

#line  1400 "cs.ATG" 
				op = OverloadableOperatorType.ShiftRight; 
			}
			break;
		}
		default: SynErr(185); break;
		}
	}

	void VariableDeclarator(
#line  1310 "cs.ATG" 
FieldDeclaration parentFieldDeclaration) {

#line  1311 "cs.ATG" 
		Expression expr = null; 
		Identifier();

#line  1313 "cs.ATG" 
		VariableDeclaration f = new VariableDeclaration(t.val); f.StartLocation = t.Location; 
		if (la.kind == 3) {
			lexer.NextToken();
			VariableInitializer(
#line  1314 "cs.ATG" 
out expr);

#line  1314 "cs.ATG" 
			f.Initializer = expr; 
		}

#line  1315 "cs.ATG" 
		f.EndLocation = t.EndLocation; SafeAdd(parentFieldDeclaration, parentFieldDeclaration.Fields, f); 
	}

	void AccessorDecls(
#line  1202 "cs.ATG" 
out PropertyGetRegion getBlock, out PropertySetRegion setBlock) {

#line  1204 "cs.ATG" 
		List<AttributeSection> attributes = new List<AttributeSection>(); 
		AttributeSection section;
		getBlock = null;
		setBlock = null; 
		ModifierList modifiers = null;
		
		while (la.kind == 18) {
			AttributeSection(
#line  1211 "cs.ATG" 
out section);

#line  1211 "cs.ATG" 
			attributes.Add(section); 
		}
		if (la.kind == 84 || la.kind == 96 || la.kind == 97) {
			AccessorModifiers(
#line  1212 "cs.ATG" 
out modifiers);
		}
		if (la.kind == 128) {
			GetAccessorDecl(
#line  1214 "cs.ATG" 
out getBlock, attributes);

#line  1215 "cs.ATG" 
			if (modifiers != null) {getBlock.Modifier = modifiers.Modifier; } 
			if (StartOf(26)) {

#line  1216 "cs.ATG" 
				attributes = new List<AttributeSection>(); modifiers = null; 
				while (la.kind == 18) {
					AttributeSection(
#line  1217 "cs.ATG" 
out section);

#line  1217 "cs.ATG" 
					attributes.Add(section); 
				}
				if (la.kind == 84 || la.kind == 96 || la.kind == 97) {
					AccessorModifiers(
#line  1218 "cs.ATG" 
out modifiers);
				}
				SetAccessorDecl(
#line  1219 "cs.ATG" 
out setBlock, attributes);

#line  1220 "cs.ATG" 
				if (modifiers != null) {setBlock.Modifier = modifiers.Modifier; } 
			}
		} else if (la.kind == 129) {
			SetAccessorDecl(
#line  1223 "cs.ATG" 
out setBlock, attributes);

#line  1224 "cs.ATG" 
			if (modifiers != null) {setBlock.Modifier = modifiers.Modifier; } 
			if (StartOf(27)) {

#line  1225 "cs.ATG" 
				attributes = new List<AttributeSection>(); modifiers = null; 
				while (la.kind == 18) {
					AttributeSection(
#line  1226 "cs.ATG" 
out section);

#line  1226 "cs.ATG" 
					attributes.Add(section); 
				}
				if (la.kind == 84 || la.kind == 96 || la.kind == 97) {
					AccessorModifiers(
#line  1227 "cs.ATG" 
out modifiers);
				}
				GetAccessorDecl(
#line  1228 "cs.ATG" 
out getBlock, attributes);

#line  1229 "cs.ATG" 
				if (modifiers != null) {getBlock.Modifier = modifiers.Modifier; } 
			}
		} else if (StartOf(18)) {
			Identifier();

#line  1231 "cs.ATG" 
			Error("get or set accessor declaration expected"); 
		} else SynErr(186);
	}

	void InterfaceAccessors(
#line  1276 "cs.ATG" 
out PropertyGetRegion getBlock, out PropertySetRegion setBlock) {

#line  1278 "cs.ATG" 
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		getBlock = null; setBlock = null;
		PropertyGetSetRegion lastBlock = null;
		
		while (la.kind == 18) {
			AttributeSection(
#line  1284 "cs.ATG" 
out section);

#line  1284 "cs.ATG" 
			attributes.Add(section); 
		}

#line  1285 "cs.ATG" 
		Location startLocation = la.Location; 
		if (la.kind == 128) {
			lexer.NextToken();

#line  1287 "cs.ATG" 
			getBlock = new PropertyGetRegion(null, attributes); 
		} else if (la.kind == 129) {
			lexer.NextToken();

#line  1288 "cs.ATG" 
			setBlock = new PropertySetRegion(null, attributes); 
		} else SynErr(187);
		Expect(11);

#line  1291 "cs.ATG" 
		if (getBlock != null) { getBlock.StartLocation = startLocation; getBlock.EndLocation = t.EndLocation; }
		if (setBlock != null) { setBlock.StartLocation = startLocation; setBlock.EndLocation = t.EndLocation; }
		attributes = new List<AttributeSection>(); 
		if (la.kind == 18 || la.kind == 128 || la.kind == 129) {
			while (la.kind == 18) {
				AttributeSection(
#line  1295 "cs.ATG" 
out section);

#line  1295 "cs.ATG" 
				attributes.Add(section); 
			}

#line  1296 "cs.ATG" 
			startLocation = la.Location; 
			if (la.kind == 128) {
				lexer.NextToken();

#line  1298 "cs.ATG" 
				if (getBlock != null) Error("get already declared");
				                 else { getBlock = new PropertyGetRegion(null, attributes); lastBlock = getBlock; }
				              
			} else if (la.kind == 129) {
				lexer.NextToken();

#line  1301 "cs.ATG" 
				if (setBlock != null) Error("set already declared");
				                 else { setBlock = new PropertySetRegion(null, attributes); lastBlock = setBlock; }
				              
			} else SynErr(188);
			Expect(11);

#line  1306 "cs.ATG" 
			if (lastBlock != null) { lastBlock.StartLocation = startLocation; lastBlock.EndLocation = t.EndLocation; } 
		}
	}

	void GetAccessorDecl(
#line  1235 "cs.ATG" 
out PropertyGetRegion getBlock, List<AttributeSection> attributes) {

#line  1236 "cs.ATG" 
		BlockStatement stmt = null; 
		Expect(128);

#line  1239 "cs.ATG" 
		Location startLocation = t.Location; 
		if (la.kind == 16) {
			Block(
#line  1240 "cs.ATG" 
out stmt);
		} else if (la.kind == 11) {
			lexer.NextToken();
		} else SynErr(189);

#line  1241 "cs.ATG" 
		getBlock = new PropertyGetRegion(stmt, attributes); 

#line  1242 "cs.ATG" 
		getBlock.StartLocation = startLocation; getBlock.EndLocation = t.EndLocation; 
	}

	void SetAccessorDecl(
#line  1245 "cs.ATG" 
out PropertySetRegion setBlock, List<AttributeSection> attributes) {

#line  1246 "cs.ATG" 
		BlockStatement stmt = null; 
		Expect(129);

#line  1249 "cs.ATG" 
		Location startLocation = t.Location; 
		if (la.kind == 16) {
			Block(
#line  1250 "cs.ATG" 
out stmt);
		} else if (la.kind == 11) {
			lexer.NextToken();
		} else SynErr(190);

#line  1251 "cs.ATG" 
		setBlock = new PropertySetRegion(stmt, attributes); 

#line  1252 "cs.ATG" 
		setBlock.StartLocation = startLocation; setBlock.EndLocation = t.EndLocation; 
	}

	void AddAccessorDecl(
#line  1334 "cs.ATG" 
out BlockStatement stmt) {

#line  1335 "cs.ATG" 
		stmt = null;
		Expect(130);
		Block(
#line  1338 "cs.ATG" 
out stmt);
	}

	void RemoveAccessorDecl(
#line  1341 "cs.ATG" 
out BlockStatement stmt) {

#line  1342 "cs.ATG" 
		stmt = null;
		Expect(131);
		Block(
#line  1345 "cs.ATG" 
out stmt);
	}

	void VariableInitializer(
#line  1363 "cs.ATG" 
out Expression initializerExpression) {

#line  1364 "cs.ATG" 
		TypeReference type = null; Expression expr = null; initializerExpression = null; 
		if (StartOf(6)) {
			Expr(
#line  1366 "cs.ATG" 
out initializerExpression);
		} else if (la.kind == 16) {
			CollectionInitializer(
#line  1367 "cs.ATG" 
out initializerExpression);
		} else if (la.kind == 106) {
			lexer.NextToken();
			Type(
#line  1368 "cs.ATG" 
out type);
			Expect(18);
			Expr(
#line  1368 "cs.ATG" 
out expr);
			Expect(19);

#line  1368 "cs.ATG" 
			initializerExpression = new StackAllocExpression(type, expr); 
		} else SynErr(191);
	}

	void Statement() {

#line  1525 "cs.ATG" 
		Statement stmt = null;
		Location startPos = la.Location;
		
		while (!(StartOf(28))) {SynErr(192); lexer.NextToken(); }
		if (
#line  1532 "cs.ATG" 
IsLabel()) {
			Identifier();

#line  1532 "cs.ATG" 
			AddChild(new LabelStatement(t.val)); 
			Expect(9);
			Statement();
		} else if (la.kind == 60) {
			lexer.NextToken();
			LocalVariableDecl(
#line  1536 "cs.ATG" 
out stmt);

#line  1537 "cs.ATG" 
			if (stmt != null) { ((LocalVariableDeclaration)stmt).Modifier |= Modifiers.Const; } 
			Expect(11);

#line  1538 "cs.ATG" 
			AddChild(stmt); 
		} else if (
#line  1540 "cs.ATG" 
IsLocalVarDecl()) {
			LocalVariableDecl(
#line  1540 "cs.ATG" 
out stmt);
			Expect(11);

#line  1540 "cs.ATG" 
			AddChild(stmt); 
		} else if (StartOf(29)) {
			EmbeddedStatement(
#line  1542 "cs.ATG" 
out stmt);

#line  1542 "cs.ATG" 
			AddChild(stmt); 
		} else SynErr(193);

#line  1548 "cs.ATG" 
		if (stmt != null) {
		stmt.StartLocation = startPos;
		stmt.EndLocation = t.EndLocation;
		}
		
	}

	void Argument(
#line  1403 "cs.ATG" 
out Expression argumentexpr) {

#line  1404 "cs.ATG" 
		argumentexpr = null; 
		if (
#line  1406 "cs.ATG" 
IdentAndColon()) {

#line  1407 "cs.ATG" 
			Token ident; Expression expr; 
			Identifier();

#line  1408 "cs.ATG" 
			ident = t; 
			Expect(9);
			ArgumentValue(
#line  1410 "cs.ATG" 
out expr);

#line  1411 "cs.ATG" 
			argumentexpr = new NamedArgumentExpression(ident.val, expr) { StartLocation = ident.Location, EndLocation = t.EndLocation }; 
		} else if (StartOf(25)) {
			ArgumentValue(
#line  1413 "cs.ATG" 
out argumentexpr);
		} else SynErr(194);
	}

	void CollectionInitializer(
#line  1447 "cs.ATG" 
out Expression outExpr) {

#line  1449 "cs.ATG" 
		Expression expr = null;
		CollectionInitializerExpression initializer = new CollectionInitializerExpression();
		
		Expect(16);

#line  1453 "cs.ATG" 
		initializer.StartLocation = t.Location; 
		if (StartOf(30)) {
			VariableInitializer(
#line  1454 "cs.ATG" 
out expr);

#line  1455 "cs.ATG" 
			SafeAdd(initializer, initializer.CreateExpressions, expr); 
			while (
#line  1456 "cs.ATG" 
NotFinalComma()) {
				Expect(14);
				VariableInitializer(
#line  1457 "cs.ATG" 
out expr);

#line  1458 "cs.ATG" 
				SafeAdd(initializer, initializer.CreateExpressions, expr); 
			}
			if (la.kind == 14) {
				lexer.NextToken();
			}
		}
		Expect(17);

#line  1462 "cs.ATG" 
		initializer.EndLocation = t.Location; outExpr = initializer; 
	}

	void ArgumentValue(
#line  1416 "cs.ATG" 
out Expression argumentexpr) {

#line  1418 "cs.ATG" 
		Expression expr;
		FieldDirection fd = FieldDirection.None;
		
		if (la.kind == 93 || la.kind == 100) {
			if (la.kind == 100) {
				lexer.NextToken();

#line  1423 "cs.ATG" 
				fd = FieldDirection.Ref; 
			} else {
				lexer.NextToken();

#line  1424 "cs.ATG" 
				fd = FieldDirection.Out; 
			}
		}
		Expr(
#line  1426 "cs.ATG" 
out expr);

#line  1427 "cs.ATG" 
		argumentexpr = fd != FieldDirection.None ? argumentexpr = new DirectionExpression(fd, expr) : expr; 
	}

	void AssignmentOperator(
#line  1430 "cs.ATG" 
out AssignmentOperatorType op) {

#line  1431 "cs.ATG" 
		op = AssignmentOperatorType.None; 
		if (la.kind == 3) {
			lexer.NextToken();

#line  1433 "cs.ATG" 
			op = AssignmentOperatorType.Assign; 
		} else if (la.kind == 38) {
			lexer.NextToken();

#line  1434 "cs.ATG" 
			op = AssignmentOperatorType.Add; 
		} else if (la.kind == 39) {
			lexer.NextToken();

#line  1435 "cs.ATG" 
			op = AssignmentOperatorType.Subtract; 
		} else if (la.kind == 40) {
			lexer.NextToken();

#line  1436 "cs.ATG" 
			op = AssignmentOperatorType.Multiply; 
		} else if (la.kind == 41) {
			lexer.NextToken();

#line  1437 "cs.ATG" 
			op = AssignmentOperatorType.Divide; 
		} else if (la.kind == 42) {
			lexer.NextToken();

#line  1438 "cs.ATG" 
			op = AssignmentOperatorType.Modulus; 
		} else if (la.kind == 43) {
			lexer.NextToken();

#line  1439 "cs.ATG" 
			op = AssignmentOperatorType.BitwiseAnd; 
		} else if (la.kind == 44) {
			lexer.NextToken();

#line  1440 "cs.ATG" 
			op = AssignmentOperatorType.BitwiseOr; 
		} else if (la.kind == 45) {
			lexer.NextToken();

#line  1441 "cs.ATG" 
			op = AssignmentOperatorType.ExclusiveOr; 
		} else if (la.kind == 46) {
			lexer.NextToken();

#line  1442 "cs.ATG" 
			op = AssignmentOperatorType.ShiftLeft; 
		} else if (
#line  1443 "cs.ATG" 
la.kind == Tokens.GreaterThan && Peek(1).kind == Tokens.GreaterEqual) {
			Expect(22);
			Expect(35);

#line  1444 "cs.ATG" 
			op = AssignmentOperatorType.ShiftRight; 
		} else SynErr(195);
	}

	void CollectionOrObjectInitializer(
#line  1465 "cs.ATG" 
out Expression outExpr) {

#line  1467 "cs.ATG" 
		Expression expr = null;
		CollectionInitializerExpression initializer = new CollectionInitializerExpression();
		
		Expect(16);

#line  1471 "cs.ATG" 
		initializer.StartLocation = t.Location; 
		if (StartOf(30)) {
			ObjectPropertyInitializerOrVariableInitializer(
#line  1472 "cs.ATG" 
out expr);

#line  1473 "cs.ATG" 
			SafeAdd(initializer, initializer.CreateExpressions, expr); 
			while (
#line  1474 "cs.ATG" 
NotFinalComma()) {
				Expect(14);
				ObjectPropertyInitializerOrVariableInitializer(
#line  1475 "cs.ATG" 
out expr);

#line  1476 "cs.ATG" 
				SafeAdd(initializer, initializer.CreateExpressions, expr); 
			}
			if (la.kind == 14) {
				lexer.NextToken();
			}
		}
		Expect(17);

#line  1480 "cs.ATG" 
		initializer.EndLocation = t.Location; outExpr = initializer; 
	}

	void ObjectPropertyInitializerOrVariableInitializer(
#line  1483 "cs.ATG" 
out Expression expr) {

#line  1484 "cs.ATG" 
		expr = null; 
		if (
#line  1486 "cs.ATG" 
IdentAndAsgn()) {
			Identifier();

#line  1488 "cs.ATG" 
			MemberInitializerExpression mie = new MemberInitializerExpression(t.val, null);
			mie.StartLocation = t.Location;
			mie.IsKey = true;
			Expression r = null; 
			Expect(3);
			if (la.kind == 16) {
				CollectionOrObjectInitializer(
#line  1493 "cs.ATG" 
out r);
			} else if (StartOf(30)) {
				VariableInitializer(
#line  1494 "cs.ATG" 
out r);
			} else SynErr(196);

#line  1495 "cs.ATG" 
			mie.Expression = r; mie.EndLocation = t.EndLocation; expr = mie; 
		} else if (StartOf(30)) {
			VariableInitializer(
#line  1497 "cs.ATG" 
out expr);
		} else SynErr(197);
	}

	void LocalVariableDecl(
#line  1501 "cs.ATG" 
out Statement stmt) {

#line  1503 "cs.ATG" 
		TypeReference type;
		VariableDeclaration      var = null;
		LocalVariableDeclaration localVariableDeclaration; 
		Location startPos = la.Location;
		
		Type(
#line  1509 "cs.ATG" 
out type);

#line  1509 "cs.ATG" 
		localVariableDeclaration = new LocalVariableDeclaration(type); localVariableDeclaration.StartLocation = startPos; 
		LocalVariableDeclarator(
#line  1510 "cs.ATG" 
out var);

#line  1510 "cs.ATG" 
		SafeAdd(localVariableDeclaration, localVariableDeclaration.Variables, var); 
		while (la.kind == 14) {
			lexer.NextToken();
			LocalVariableDeclarator(
#line  1511 "cs.ATG" 
out var);

#line  1511 "cs.ATG" 
			SafeAdd(localVariableDeclaration, localVariableDeclaration.Variables, var); 
		}

#line  1512 "cs.ATG" 
		stmt = localVariableDeclaration; stmt.EndLocation = t.EndLocation; 
	}

	void LocalVariableDeclarator(
#line  1515 "cs.ATG" 
out VariableDeclaration var) {

#line  1516 "cs.ATG" 
		Expression expr = null; 
		Identifier();

#line  1518 "cs.ATG" 
		var = new VariableDeclaration(t.val); var.StartLocation = t.Location; 
		if (la.kind == 3) {
			lexer.NextToken();
			VariableInitializer(
#line  1519 "cs.ATG" 
out expr);

#line  1519 "cs.ATG" 
			var.Initializer = expr; 
		}

#line  1520 "cs.ATG" 
		var.EndLocation = t.EndLocation; 
	}

	void EmbeddedStatement(
#line  1555 "cs.ATG" 
out Statement statement) {

#line  1557 "cs.ATG" 
		TypeReference type = null;
		Expression expr = null;
		Statement embeddedStatement = null;
		BlockStatement block = null;
		statement = null;
		

#line  1564 "cs.ATG" 
		Location startLocation = la.Location; 
		if (la.kind == 16) {
			Block(
#line  1566 "cs.ATG" 
out block);

#line  1566 "cs.ATG" 
			statement = block; 
		} else if (la.kind == 11) {
			lexer.NextToken();

#line  1569 "cs.ATG" 
			statement = new EmptyStatement(); 
		} else if (
#line  1572 "cs.ATG" 
UnCheckedAndLBrace()) {

#line  1572 "cs.ATG" 
			bool isChecked = true; 
			if (la.kind == 58) {
				lexer.NextToken();
			} else if (la.kind == 118) {
				lexer.NextToken();

#line  1573 "cs.ATG" 
				isChecked = false;
			} else SynErr(198);
			Block(
#line  1574 "cs.ATG" 
out block);

#line  1574 "cs.ATG" 
			statement = isChecked ? (Statement)new CheckedStatement(block) : (Statement)new UncheckedStatement(block); 
		} else if (la.kind == 79) {
			IfStatement(
#line  1577 "cs.ATG" 
out statement);
		} else if (la.kind == 110) {
			lexer.NextToken();

#line  1579 "cs.ATG" 
			List<SwitchSection> switchSections = new List<SwitchSection>(); 
			Expect(20);
			Expr(
#line  1580 "cs.ATG" 
out expr);
			Expect(21);
			Expect(16);
			SwitchSections(
#line  1581 "cs.ATG" 
switchSections);
			Expect(17);

#line  1583 "cs.ATG" 
			statement = new SwitchStatement(expr, switchSections); 
		} else if (la.kind == 125) {
			lexer.NextToken();
			Expect(20);
			Expr(
#line  1586 "cs.ATG" 
out expr);
			Expect(21);
			EmbeddedStatement(
#line  1587 "cs.ATG" 
out embeddedStatement);

#line  1588 "cs.ATG" 
			statement = new DoLoopStatement(expr, embeddedStatement, ConditionType.While, ConditionPosition.Start);
		} else if (la.kind == 65) {
			lexer.NextToken();
			EmbeddedStatement(
#line  1590 "cs.ATG" 
out embeddedStatement);
			Expect(125);
			Expect(20);
			Expr(
#line  1591 "cs.ATG" 
out expr);
			Expect(21);
			Expect(11);

#line  1592 "cs.ATG" 
			statement = new DoLoopStatement(expr, embeddedStatement, ConditionType.While, ConditionPosition.End); 
		} else if (la.kind == 76) {
			lexer.NextToken();

#line  1594 "cs.ATG" 
			List<Statement> initializer = null; List<Statement> iterator = null; 
			Expect(20);
			if (StartOf(6)) {
				ForInitializer(
#line  1595 "cs.ATG" 
out initializer);
			}
			Expect(11);
			if (StartOf(6)) {
				Expr(
#line  1596 "cs.ATG" 
out expr);
			}
			Expect(11);
			if (StartOf(6)) {
				ForIterator(
#line  1597 "cs.ATG" 
out iterator);
			}
			Expect(21);
			EmbeddedStatement(
#line  1598 "cs.ATG" 
out embeddedStatement);

#line  1599 "cs.ATG" 
			statement = new ForStatement(initializer, expr, iterator, embeddedStatement); 
		} else if (la.kind == 77) {
			lexer.NextToken();
			Expect(20);
			Type(
#line  1601 "cs.ATG" 
out type);
			Identifier();

#line  1601 "cs.ATG" 
			string varName = t.val; 
			Expect(81);
			Expr(
#line  1602 "cs.ATG" 
out expr);
			Expect(21);
			EmbeddedStatement(
#line  1603 "cs.ATG" 
out embeddedStatement);

#line  1604 "cs.ATG" 
			statement = new ForeachStatement(type, varName , expr, embeddedStatement); 
		} else if (la.kind == 53) {
			lexer.NextToken();
			Expect(11);

#line  1607 "cs.ATG" 
			statement = new BreakStatement(); 
		} else if (la.kind == 61) {
			lexer.NextToken();
			Expect(11);

#line  1608 "cs.ATG" 
			statement = new ContinueStatement(); 
		} else if (la.kind == 78) {
			GotoStatement(
#line  1609 "cs.ATG" 
out statement);
		} else if (
#line  1611 "cs.ATG" 
IsYieldStatement()) {
			Expect(132);
			if (la.kind == 101) {
				lexer.NextToken();
				Expr(
#line  1612 "cs.ATG" 
out expr);

#line  1612 "cs.ATG" 
				statement = new YieldStatement(new ReturnStatement(expr)); 
			} else if (la.kind == 53) {
				lexer.NextToken();

#line  1613 "cs.ATG" 
				statement = new YieldStatement(new BreakStatement()); 
			} else SynErr(199);
			Expect(11);
		} else if (la.kind == 101) {
			lexer.NextToken();
			if (StartOf(6)) {
				Expr(
#line  1616 "cs.ATG" 
out expr);
			}
			Expect(11);

#line  1616 "cs.ATG" 
			statement = new ReturnStatement(expr); 
		} else if (la.kind == 112) {
			lexer.NextToken();
			if (StartOf(6)) {
				Expr(
#line  1617 "cs.ATG" 
out expr);
			}
			Expect(11);

#line  1617 "cs.ATG" 
			statement = new ThrowStatement(expr); 
		} else if (StartOf(6)) {
			StatementExpr(
#line  1620 "cs.ATG" 
out statement);
			while (!(la.kind == 0 || la.kind == 11)) {SynErr(200); lexer.NextToken(); }
			Expect(11);
		} else if (la.kind == 114) {
			TryStatement(
#line  1623 "cs.ATG" 
out statement);
		} else if (la.kind == 86) {
			lexer.NextToken();
			Expect(20);
			Expr(
#line  1626 "cs.ATG" 
out expr);
			Expect(21);
			EmbeddedStatement(
#line  1627 "cs.ATG" 
out embeddedStatement);

#line  1627 "cs.ATG" 
			statement = new LockStatement(expr, embeddedStatement); 
		} else if (la.kind == 121) {

#line  1630 "cs.ATG" 
			Statement resourceAcquisitionStmt = null; 
			lexer.NextToken();
			Expect(20);
			ResourceAcquisition(
#line  1632 "cs.ATG" 
out resourceAcquisitionStmt);
			Expect(21);
			EmbeddedStatement(
#line  1633 "cs.ATG" 
out embeddedStatement);

#line  1633 "cs.ATG" 
			statement = new UsingStatement(resourceAcquisitionStmt, embeddedStatement); 
		} else if (la.kind == 119) {
			lexer.NextToken();
			Block(
#line  1636 "cs.ATG" 
out block);

#line  1636 "cs.ATG" 
			statement = new UnsafeStatement(block); 
		} else if (la.kind == 74) {

#line  1638 "cs.ATG" 
			Statement pointerDeclarationStmt = null; 
			lexer.NextToken();
			Expect(20);
			ResourceAcquisition(
#line  1640 "cs.ATG" 
out pointerDeclarationStmt);
			Expect(21);
			EmbeddedStatement(
#line  1641 "cs.ATG" 
out embeddedStatement);

#line  1641 "cs.ATG" 
			statement = new FixedStatement(pointerDeclarationStmt, embeddedStatement); 
		} else SynErr(201);

#line  1643 "cs.ATG" 
		if (statement != null) {
		statement.StartLocation = startLocation;
		statement.EndLocation = t.EndLocation;
		}
		
	}

	void IfStatement(
#line  1650 "cs.ATG" 
out Statement statement) {

#line  1652 "cs.ATG" 
		Expression expr = null;
		Statement embeddedStatement = null;
		statement = null;
		
		Expect(79);
		Expect(20);
		Expr(
#line  1658 "cs.ATG" 
out expr);
		Expect(21);
		EmbeddedStatement(
#line  1659 "cs.ATG" 
out embeddedStatement);

#line  1660 "cs.ATG" 
		Statement elseStatement = null; 
		if (la.kind == 67) {
			lexer.NextToken();
			EmbeddedStatement(
#line  1661 "cs.ATG" 
out elseStatement);
		}

#line  1662 "cs.ATG" 
		statement = elseStatement != null ? new IfElseStatement(expr, embeddedStatement, elseStatement) : new IfElseStatement(expr, embeddedStatement); 

#line  1663 "cs.ATG" 
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
#line  1693 "cs.ATG" 
List<SwitchSection> switchSections) {

#line  1695 "cs.ATG" 
		SwitchSection switchSection = new SwitchSection();
		CaseLabel label;
		
		SwitchLabel(
#line  1699 "cs.ATG" 
out label);

#line  1699 "cs.ATG" 
		SafeAdd(switchSection, switchSection.SwitchLabels, label); 

#line  1700 "cs.ATG" 
		BlockStart(switchSection); 
		while (StartOf(31)) {
			if (la.kind == 55 || la.kind == 63) {
				SwitchLabel(
#line  1702 "cs.ATG" 
out label);

#line  1703 "cs.ATG" 
				if (label != null) {
				if (switchSection.Children.Count > 0) {
					// open new section
					BlockEnd(); switchSections.Add(switchSection);
					switchSection = new SwitchSection();
					BlockStart(switchSection);
				}
				SafeAdd(switchSection, switchSection.SwitchLabels, label);
				}
				
			} else {
				Statement();
			}
		}

#line  1715 "cs.ATG" 
		BlockEnd(); switchSections.Add(switchSection); 
	}

	void ForInitializer(
#line  1674 "cs.ATG" 
out List<Statement> initializer) {

#line  1676 "cs.ATG" 
		Statement stmt; 
		initializer = new List<Statement>();
		
		if (
#line  1680 "cs.ATG" 
IsLocalVarDecl()) {
			LocalVariableDecl(
#line  1680 "cs.ATG" 
out stmt);

#line  1680 "cs.ATG" 
			initializer.Add(stmt);
		} else if (StartOf(6)) {
			StatementExpr(
#line  1681 "cs.ATG" 
out stmt);

#line  1681 "cs.ATG" 
			initializer.Add(stmt);
			while (la.kind == 14) {
				lexer.NextToken();
				StatementExpr(
#line  1681 "cs.ATG" 
out stmt);

#line  1681 "cs.ATG" 
				initializer.Add(stmt);
			}
		} else SynErr(202);
	}

	void ForIterator(
#line  1684 "cs.ATG" 
out List<Statement> iterator) {

#line  1686 "cs.ATG" 
		Statement stmt; 
		iterator = new List<Statement>();
		
		StatementExpr(
#line  1690 "cs.ATG" 
out stmt);

#line  1690 "cs.ATG" 
		iterator.Add(stmt);
		while (la.kind == 14) {
			lexer.NextToken();
			StatementExpr(
#line  1690 "cs.ATG" 
out stmt);

#line  1690 "cs.ATG" 
			iterator.Add(stmt); 
		}
	}

	void GotoStatement(
#line  1772 "cs.ATG" 
out Statement stmt) {

#line  1773 "cs.ATG" 
		Expression expr; stmt = null; 
		Expect(78);
		if (StartOf(18)) {
			Identifier();

#line  1777 "cs.ATG" 
			stmt = new GotoStatement(t.val); 
			Expect(11);
		} else if (la.kind == 55) {
			lexer.NextToken();
			Expr(
#line  1778 "cs.ATG" 
out expr);
			Expect(11);

#line  1778 "cs.ATG" 
			stmt = new GotoCaseStatement(expr); 
		} else if (la.kind == 63) {
			lexer.NextToken();
			Expect(11);

#line  1779 "cs.ATG" 
			stmt = new GotoCaseStatement(null); 
		} else SynErr(203);
	}

	void StatementExpr(
#line  1799 "cs.ATG" 
out Statement stmt) {

#line  1800 "cs.ATG" 
		Expression expr; 
		Expr(
#line  1802 "cs.ATG" 
out expr);

#line  1805 "cs.ATG" 
		stmt = new ExpressionStatement(expr); 
	}

	void TryStatement(
#line  1725 "cs.ATG" 
out Statement tryStatement) {

#line  1727 "cs.ATG" 
		BlockStatement blockStmt = null, finallyStmt = null;
		CatchClause catchClause = null;
		List<CatchClause> catchClauses = new List<CatchClause>();
		
		Expect(114);
		Block(
#line  1732 "cs.ATG" 
out blockStmt);
		while (la.kind == 56) {
			CatchClause(
#line  1734 "cs.ATG" 
out catchClause);

#line  1735 "cs.ATG" 
			if (catchClause != null) catchClauses.Add(catchClause); 
		}
		if (la.kind == 73) {
			lexer.NextToken();
			Block(
#line  1737 "cs.ATG" 
out finallyStmt);
		}

#line  1739 "cs.ATG" 
		tryStatement = new TryCatchStatement(blockStmt, catchClauses, finallyStmt);
		if (catchClauses != null) {
			foreach (CatchClause cc in catchClauses) cc.Parent = tryStatement;
		}
		
	}

	void ResourceAcquisition(
#line  1783 "cs.ATG" 
out Statement stmt) {

#line  1785 "cs.ATG" 
		stmt = null;
		Expression expr;
		
		if (
#line  1790 "cs.ATG" 
IsLocalVarDecl()) {
			LocalVariableDecl(
#line  1790 "cs.ATG" 
out stmt);
		} else if (StartOf(6)) {
			Expr(
#line  1791 "cs.ATG" 
out expr);

#line  1795 "cs.ATG" 
			stmt = new ExpressionStatement(expr); 
		} else SynErr(204);
	}

	void SwitchLabel(
#line  1718 "cs.ATG" 
out CaseLabel label) {

#line  1719 "cs.ATG" 
		Expression expr = null; label = null; 
		if (la.kind == 55) {
			lexer.NextToken();
			Expr(
#line  1721 "cs.ATG" 
out expr);
			Expect(9);

#line  1721 "cs.ATG" 
			label =  new CaseLabel(expr); 
		} else if (la.kind == 63) {
			lexer.NextToken();
			Expect(9);

#line  1722 "cs.ATG" 
			label =  new CaseLabel(); 
		} else SynErr(205);
	}

	void CatchClause(
#line  1746 "cs.ATG" 
out CatchClause catchClause) {
		Expect(56);

#line  1748 "cs.ATG" 
		string identifier;
		BlockStatement stmt;
		TypeReference typeRef;
		Location startPos = t.Location;
		catchClause = null;
		
		if (la.kind == 16) {
			Block(
#line  1756 "cs.ATG" 
out stmt);

#line  1756 "cs.ATG" 
			catchClause = new CatchClause(stmt);  
		} else if (la.kind == 20) {
			lexer.NextToken();
			ClassType(
#line  1759 "cs.ATG" 
out typeRef, false);

#line  1759 "cs.ATG" 
			identifier = null; 
			if (StartOf(18)) {
				Identifier();

#line  1760 "cs.ATG" 
				identifier = t.val; 
			}
			Expect(21);
			Block(
#line  1761 "cs.ATG" 
out stmt);

#line  1762 "cs.ATG" 
			catchClause = new CatchClause(typeRef, identifier, stmt); 
		} else SynErr(206);

#line  1765 "cs.ATG" 
		if (catchClause != null) {
		catchClause.StartLocation = startPos;
		catchClause.EndLocation = t.Location;
		}
		
	}

	void UnaryExpr(
#line  1834 "cs.ATG" 
out Expression uExpr) {

#line  1836 "cs.ATG" 
		TypeReference type = null;
		Expression expr = null;
		ArrayList expressions = new ArrayList();
		uExpr = null;
		
		while (StartOf(32) || 
#line  1859 "cs.ATG" 
IsTypeCast()) {
			if (la.kind == 4) {
				lexer.NextToken();

#line  1845 "cs.ATG" 
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.Plus) { StartLocation = t.Location }); 
			} else if (la.kind == 5) {
				lexer.NextToken();

#line  1846 "cs.ATG" 
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.Minus) { StartLocation = t.Location }); 
			} else if (la.kind == 24) {
				lexer.NextToken();

#line  1847 "cs.ATG" 
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.Not) { StartLocation = t.Location }); 
			} else if (la.kind == 27) {
				lexer.NextToken();

#line  1848 "cs.ATG" 
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.BitNot) { StartLocation = t.Location }); 
			} else if (la.kind == 6) {
				lexer.NextToken();

#line  1849 "cs.ATG" 
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.Dereference) { StartLocation = t.Location }); 
			} else if (la.kind == 31) {
				lexer.NextToken();

#line  1850 "cs.ATG" 
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.Increment) { StartLocation = t.Location }); 
			} else if (la.kind == 32) {
				lexer.NextToken();

#line  1851 "cs.ATG" 
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.Decrement) { StartLocation = t.Location }); 
			} else if (la.kind == 28) {
				lexer.NextToken();

#line  1852 "cs.ATG" 
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.AddressOf) { StartLocation = t.Location }); 
			} else if (la.kind == 146) {
				lexer.NextToken();

#line  1853 "cs.ATG" 
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.Await) { StartLocation = t.Location }); 
			} else {
				Expect(20);
				Type(
#line  1859 "cs.ATG" 
out type);
				Expect(21);

#line  1859 "cs.ATG" 
				expressions.Add(new CastExpression(type) { StartLocation = t.Location }); 
			}
		}
		if (
#line  1864 "cs.ATG" 
LastExpressionIsUnaryMinus(expressions) && IsMostNegativeIntegerWithoutTypeSuffix()) {
			Expect(2);

#line  1867 "cs.ATG" 
			expressions.RemoveAt(expressions.Count - 1);
			if (t.literalValue is uint) {
				expr = new PrimitiveExpression(int.MinValue, int.MinValue.ToString());
			} else if (t.literalValue is ulong) {
				expr = new PrimitiveExpression(long.MinValue, long.MinValue.ToString());
			} else {
				throw new Exception("t.literalValue must be uint or ulong");
			}
			
		} else if (StartOf(33)) {
			PrimaryExpr(
#line  1876 "cs.ATG" 
out expr);
		} else SynErr(207);

#line  1878 "cs.ATG" 
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
#line  2204 "cs.ATG" 
ref Expression outExpr) {

#line  2205 "cs.ATG" 
		Expression expr; Location startLocation = la.Location; 
		ConditionalAndExpr(
#line  2207 "cs.ATG" 
ref outExpr);
		while (la.kind == 26) {
			lexer.NextToken();
			UnaryExpr(
#line  2207 "cs.ATG" 
out expr);
			ConditionalAndExpr(
#line  2207 "cs.ATG" 
ref expr);

#line  2207 "cs.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.LogicalOr, expr) { StartLocation = startLocation, EndLocation = t.EndLocation };  
		}
	}

	void PrimaryExpr(
#line  1895 "cs.ATG" 
out Expression pexpr) {

#line  1897 "cs.ATG" 
		TypeReference type = null;
		Expression expr;
		pexpr = null;
		

#line  1902 "cs.ATG" 
		Location startLocation = la.Location; 
		if (la.kind == 113) {
			lexer.NextToken();

#line  1904 "cs.ATG" 
			pexpr = new PrimitiveExpression(true, "true");  
		} else if (la.kind == 72) {
			lexer.NextToken();

#line  1905 "cs.ATG" 
			pexpr = new PrimitiveExpression(false, "false"); 
		} else if (la.kind == 90) {
			lexer.NextToken();

#line  1906 "cs.ATG" 
			pexpr = new PrimitiveExpression(null, "null");  
		} else if (la.kind == 2) {
			lexer.NextToken();

#line  1907 "cs.ATG" 
			pexpr = new PrimitiveExpression(t.literalValue, t.val) { LiteralFormat = t.literalFormat };  
		} else if (
#line  1908 "cs.ATG" 
StartOfQueryExpression()) {
			QueryExpression(
#line  1909 "cs.ATG" 
out pexpr);
		} else if (
#line  1910 "cs.ATG" 
IdentAndDoubleColon()) {
			Identifier();

#line  1911 "cs.ATG" 
			type = new TypeReference(t.val); 
			Expect(10);

#line  1912 "cs.ATG" 
			pexpr = new TypeReferenceExpression(type); 
			Identifier();

#line  1913 "cs.ATG" 
			if (type.Type == "global") { type.IsGlobal = true; type.Type = t.val ?? "?"; } else type.Type += "." + (t.val ?? "?"); 
		} else if (la.kind == 64) {
			lexer.NextToken();
			AnonymousMethodExpr(
#line  1915 "cs.ATG" 
out expr);

#line  1915 "cs.ATG" 
			pexpr = expr; 
		} else if (
#line  1916 "cs.ATG" 
la.kind == Tokens.Async && Peek(1).kind == Tokens.Delegate) {
			Expect(145);
			Expect(64);
			AnonymousMethodExpr(
#line  1917 "cs.ATG" 
out expr);

#line  1917 "cs.ATG" 
			pexpr = expr; 

#line  1918 "cs.ATG" 
			((AnonymousMethodExpression)expr).IsAsync = true; 
		} else if (
#line  1920 "cs.ATG" 
la.kind == Tokens.Async && Peek(1).kind == Tokens.OpenParenthesis) {
			Expect(145);
			LambdaExpression(
#line  1922 "cs.ATG" 
out pexpr);

#line  1923 "cs.ATG" 
			((LambdaExpression)pexpr).IsAsync = true; 
		} else if (
#line  1925 "cs.ATG" 
la.kind == Tokens.Async && IsIdentifierToken(Peek(1))) {
			Expect(145);
			Identifier();

#line  1927 "cs.ATG" 
			pexpr = new IdentifierExpression(t.val); 
			ShortedLambdaExpression(
#line  1928 "cs.ATG" 
(IdentifierExpression)pexpr, out pexpr);

#line  1929 "cs.ATG" 
			((LambdaExpression)pexpr).IsAsync = true; 
		} else if (StartOf(18)) {
			Identifier();

#line  1933 "cs.ATG" 
			pexpr = new IdentifierExpression(t.val); 
			if (la.kind == 48 || 
#line  1936 "cs.ATG" 
IsGenericInSimpleNameOrMemberAccess()) {
				if (la.kind == 48) {
					ShortedLambdaExpression(
#line  1935 "cs.ATG" 
(IdentifierExpression)pexpr, out pexpr);
				} else {

#line  1937 "cs.ATG" 
					List<TypeReference> typeList; 
					TypeArgumentList(
#line  1938 "cs.ATG" 
out typeList, false);

#line  1939 "cs.ATG" 
					((IdentifierExpression)pexpr).TypeArguments = typeList; 
				}
			}
		} else if (
#line  1942 "cs.ATG" 
IsLambdaExpression()) {
			LambdaExpression(
#line  1943 "cs.ATG" 
out pexpr);
		} else if (la.kind == 20) {
			lexer.NextToken();
			Expr(
#line  1946 "cs.ATG" 
out expr);
			Expect(21);

#line  1946 "cs.ATG" 
			pexpr = new ParenthesizedExpression(expr); 
		} else if (StartOf(34)) {

#line  1949 "cs.ATG" 
			string val = null; 
			switch (la.kind) {
			case 52: {
				lexer.NextToken();

#line  1950 "cs.ATG" 
				val = "System.Boolean"; 
				break;
			}
			case 54: {
				lexer.NextToken();

#line  1951 "cs.ATG" 
				val = "System.Byte"; 
				break;
			}
			case 57: {
				lexer.NextToken();

#line  1952 "cs.ATG" 
				val = "System.Char"; 
				break;
			}
			case 62: {
				lexer.NextToken();

#line  1953 "cs.ATG" 
				val = "System.Decimal"; 
				break;
			}
			case 66: {
				lexer.NextToken();

#line  1954 "cs.ATG" 
				val = "System.Double"; 
				break;
			}
			case 75: {
				lexer.NextToken();

#line  1955 "cs.ATG" 
				val = "System.Single"; 
				break;
			}
			case 82: {
				lexer.NextToken();

#line  1956 "cs.ATG" 
				val = "System.Int32"; 
				break;
			}
			case 87: {
				lexer.NextToken();

#line  1957 "cs.ATG" 
				val = "System.Int64"; 
				break;
			}
			case 91: {
				lexer.NextToken();

#line  1958 "cs.ATG" 
				val = "System.Object"; 
				break;
			}
			case 102: {
				lexer.NextToken();

#line  1959 "cs.ATG" 
				val = "System.SByte"; 
				break;
			}
			case 104: {
				lexer.NextToken();

#line  1960 "cs.ATG" 
				val = "System.Int16"; 
				break;
			}
			case 108: {
				lexer.NextToken();

#line  1961 "cs.ATG" 
				val = "System.String"; 
				break;
			}
			case 116: {
				lexer.NextToken();

#line  1962 "cs.ATG" 
				val = "System.UInt32"; 
				break;
			}
			case 117: {
				lexer.NextToken();

#line  1963 "cs.ATG" 
				val = "System.UInt64"; 
				break;
			}
			case 120: {
				lexer.NextToken();

#line  1964 "cs.ATG" 
				val = "System.UInt16"; 
				break;
			}
			case 123: {
				lexer.NextToken();

#line  1965 "cs.ATG" 
				val = "System.Void"; 
				break;
			}
			}

#line  1967 "cs.ATG" 
			pexpr = new TypeReferenceExpression(new TypeReference(val, true)) { StartLocation = t.Location, EndLocation = t.EndLocation }; 
		} else if (la.kind == 111) {
			lexer.NextToken();

#line  1970 "cs.ATG" 
			pexpr = new ThisReferenceExpression(); pexpr.StartLocation = t.Location; pexpr.EndLocation = t.EndLocation; 
		} else if (la.kind == 51) {
			lexer.NextToken();

#line  1972 "cs.ATG" 
			pexpr = new BaseReferenceExpression(); pexpr.StartLocation = t.Location; pexpr.EndLocation = t.EndLocation; 
		} else if (la.kind == 89) {
			NewExpression(
#line  1975 "cs.ATG" 
out pexpr);
		} else if (la.kind == 115) {
			lexer.NextToken();
			Expect(20);
			if (
#line  1979 "cs.ATG" 
NotVoidPointer()) {
				Expect(123);

#line  1979 "cs.ATG" 
				type = new TypeReference("System.Void", true); 
			} else if (StartOf(10)) {
				TypeWithRestriction(
#line  1980 "cs.ATG" 
out type, true, true);
			} else SynErr(208);
			Expect(21);

#line  1982 "cs.ATG" 
			pexpr = new TypeOfExpression(type); 
		} else if (la.kind == 63) {
			lexer.NextToken();
			Expect(20);
			Type(
#line  1984 "cs.ATG" 
out type);
			Expect(21);

#line  1984 "cs.ATG" 
			pexpr = new DefaultValueExpression(type); 
		} else if (la.kind == 105) {
			lexer.NextToken();
			Expect(20);
			Type(
#line  1985 "cs.ATG" 
out type);
			Expect(21);

#line  1985 "cs.ATG" 
			pexpr = new SizeOfExpression(type); 
		} else if (la.kind == 58) {
			lexer.NextToken();
			Expect(20);
			Expr(
#line  1986 "cs.ATG" 
out expr);
			Expect(21);

#line  1986 "cs.ATG" 
			pexpr = new CheckedExpression(expr); 
		} else if (la.kind == 118) {
			lexer.NextToken();
			Expect(20);
			Expr(
#line  1987 "cs.ATG" 
out expr);
			Expect(21);

#line  1987 "cs.ATG" 
			pexpr = new UncheckedExpression(expr); 
		} else SynErr(209);

#line  1989 "cs.ATG" 
		if (pexpr != null) {
		if (pexpr.StartLocation.IsEmpty)
			pexpr.StartLocation = startLocation;
		if (pexpr.EndLocation.IsEmpty)
			pexpr.EndLocation = t.EndLocation;
		}
		
		while (StartOf(35)) {

#line  1997 "cs.ATG" 
			startLocation = la.Location; 
			switch (la.kind) {
			case 31: {
				lexer.NextToken();

#line  1999 "cs.ATG" 
				pexpr = new UnaryOperatorExpression(pexpr, UnaryOperatorType.PostIncrement); 
				break;
			}
			case 32: {
				lexer.NextToken();

#line  2001 "cs.ATG" 
				pexpr = new UnaryOperatorExpression(pexpr, UnaryOperatorType.PostDecrement); 
				break;
			}
			case 47: {
				PointerMemberAccess(
#line  2003 "cs.ATG" 
out pexpr, pexpr);
				break;
			}
			case 15: {
				MemberAccess(
#line  2004 "cs.ATG" 
out pexpr, pexpr);
				break;
			}
			case 20: {
				lexer.NextToken();

#line  2008 "cs.ATG" 
				List<Expression> parameters = new List<Expression>(); 

#line  2009 "cs.ATG" 
				pexpr = new InvocationExpression(pexpr, parameters); 
				if (StartOf(25)) {
					Argument(
#line  2010 "cs.ATG" 
out expr);

#line  2010 "cs.ATG" 
					SafeAdd(pexpr, parameters, expr); 
					while (la.kind == 14) {
						lexer.NextToken();
						Argument(
#line  2011 "cs.ATG" 
out expr);

#line  2011 "cs.ATG" 
						SafeAdd(pexpr, parameters, expr); 
					}
				}
				Expect(21);
				break;
			}
			case 18: {

#line  2017 "cs.ATG" 
				List<Expression> indices = new List<Expression>();
				pexpr = new IndexerExpression(pexpr, indices);
				
				lexer.NextToken();
				Expr(
#line  2020 "cs.ATG" 
out expr);

#line  2020 "cs.ATG" 
				SafeAdd(pexpr, indices, expr); 
				while (la.kind == 14) {
					lexer.NextToken();
					Expr(
#line  2021 "cs.ATG" 
out expr);

#line  2021 "cs.ATG" 
					SafeAdd(pexpr, indices, expr); 
				}
				Expect(19);
				break;
			}
			}

#line  2024 "cs.ATG" 
			if (pexpr != null) {
			if (pexpr.StartLocation.IsEmpty)
				pexpr.StartLocation = startLocation;
			if (pexpr.EndLocation.IsEmpty)
				pexpr.EndLocation = t.EndLocation;
			}
			
		}
	}

	void QueryExpression(
#line  2462 "cs.ATG" 
out Expression outExpr) {

#line  2463 "cs.ATG" 
		QueryExpression q = new QueryExpression(); outExpr = q; q.StartLocation = la.Location; 
		QueryExpressionFromClause fromClause;
		
		QueryExpressionFromClause(
#line  2467 "cs.ATG" 
out fromClause);

#line  2467 "cs.ATG" 
		q.FromClause = fromClause; 
		QueryExpressionBody(
#line  2468 "cs.ATG" 
ref q);

#line  2469 "cs.ATG" 
		q.EndLocation = t.EndLocation; 
		outExpr = q; /* set outExpr to q again if QueryExpressionBody changed it (can happen with 'into' clauses) */ 
		
	}

	void AnonymousMethodExpr(
#line  2185 "cs.ATG" 
out Expression outExpr) {

#line  2187 "cs.ATG" 
		AnonymousMethodExpression expr = new AnonymousMethodExpression();
		expr.StartLocation = t.Location;
		BlockStatement stmt;
		List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
		outExpr = expr;
		
		if (la.kind == 20) {
			lexer.NextToken();
			if (StartOf(11)) {
				FormalParameterList(
#line  2196 "cs.ATG" 
p);

#line  2196 "cs.ATG" 
				expr.Parameters = p; 
			}
			Expect(21);

#line  2198 "cs.ATG" 
			expr.HasParameterList = true; 
		}
		Block(
#line  2200 "cs.ATG" 
out stmt);

#line  2200 "cs.ATG" 
		expr.Body = stmt; 

#line  2201 "cs.ATG" 
		expr.EndLocation = t.Location; 
	}

	void LambdaExpression(
#line  2118 "cs.ATG" 
out Expression outExpr) {

#line  2120 "cs.ATG" 
		LambdaExpression lambda = new LambdaExpression();
		lambda.StartLocation = la.Location;
		ParameterDeclarationExpression p;
		outExpr = lambda;
		
		Expect(20);
		if (StartOf(36)) {
			LambdaExpressionParameter(
#line  2128 "cs.ATG" 
out p);

#line  2128 "cs.ATG" 
			SafeAdd(lambda, lambda.Parameters, p); 
			while (la.kind == 14) {
				lexer.NextToken();
				LambdaExpressionParameter(
#line  2130 "cs.ATG" 
out p);

#line  2130 "cs.ATG" 
				SafeAdd(lambda, lambda.Parameters, p); 
			}
		}
		Expect(21);
		Expect(48);
		LambdaExpressionBody(
#line  2135 "cs.ATG" 
lambda);
	}

	void ShortedLambdaExpression(
#line  2138 "cs.ATG" 
IdentifierExpression ident, out Expression pexpr) {

#line  2139 "cs.ATG" 
		LambdaExpression lambda = new LambdaExpression(); pexpr = lambda; 
		Expect(48);

#line  2144 "cs.ATG" 
		lambda.StartLocation = ident.StartLocation;
		SafeAdd(lambda, lambda.Parameters, new ParameterDeclarationExpression(null, ident.Identifier));
		lambda.Parameters[0].StartLocation = ident.StartLocation;
		lambda.Parameters[0].EndLocation = ident.EndLocation;
		
		LambdaExpressionBody(
#line  2149 "cs.ATG" 
lambda);
	}

	void TypeArgumentList(
#line  2381 "cs.ATG" 
out List<TypeReference> types, bool canBeUnbound) {

#line  2383 "cs.ATG" 
		types = new List<TypeReference>();
		TypeReference type = null;
		
		Expect(23);
		if (
#line  2388 "cs.ATG" 
canBeUnbound && (la.kind == Tokens.GreaterThan || la.kind == Tokens.Comma)) {

#line  2389 "cs.ATG" 
			types.Add(TypeReference.Null); 
			while (la.kind == 14) {
				lexer.NextToken();

#line  2390 "cs.ATG" 
				types.Add(TypeReference.Null); 
			}
		} else if (StartOf(10)) {
			Type(
#line  2391 "cs.ATG" 
out type);

#line  2391 "cs.ATG" 
			if (type != null) { types.Add(type); } 
			while (la.kind == 14) {
				lexer.NextToken();
				Type(
#line  2392 "cs.ATG" 
out type);

#line  2392 "cs.ATG" 
				if (type != null) { types.Add(type); } 
			}
		} else SynErr(210);
		Expect(22);
	}

	void NewExpression(
#line  2065 "cs.ATG" 
out Expression pexpr) {

#line  2066 "cs.ATG" 
		pexpr = null;
		List<Expression> parameters = new List<Expression>();
		TypeReference type = null;
		Expression expr;
		
		Expect(89);
		if (StartOf(10)) {
			NonArrayType(
#line  2073 "cs.ATG" 
out type);
		}
		if (la.kind == 16 || la.kind == 20) {
			if (la.kind == 20) {

#line  2079 "cs.ATG" 
				ObjectCreateExpression oce = new ObjectCreateExpression(type, parameters); 
				lexer.NextToken();

#line  2080 "cs.ATG" 
				if (type == null) Error("Cannot use an anonymous type with arguments for the constructor"); 
				if (StartOf(25)) {
					Argument(
#line  2081 "cs.ATG" 
out expr);

#line  2081 "cs.ATG" 
					SafeAdd(oce, parameters, expr); 
					while (la.kind == 14) {
						lexer.NextToken();
						Argument(
#line  2082 "cs.ATG" 
out expr);

#line  2082 "cs.ATG" 
						SafeAdd(oce, parameters, expr); 
					}
				}
				Expect(21);

#line  2084 "cs.ATG" 
				pexpr = oce; 
				if (la.kind == 16) {
					CollectionOrObjectInitializer(
#line  2085 "cs.ATG" 
out expr);

#line  2085 "cs.ATG" 
					oce.ObjectInitializer = (CollectionInitializerExpression)expr; 
				}
			} else {

#line  2086 "cs.ATG" 
				ObjectCreateExpression oce = new ObjectCreateExpression(type, parameters); 
				CollectionOrObjectInitializer(
#line  2087 "cs.ATG" 
out expr);

#line  2087 "cs.ATG" 
				oce.ObjectInitializer = (CollectionInitializerExpression)expr; 

#line  2088 "cs.ATG" 
				pexpr = oce; 
			}
		} else if (la.kind == 18) {
			lexer.NextToken();

#line  2093 "cs.ATG" 
			ArrayCreateExpression ace = new ArrayCreateExpression(type);
			/* we must not change RankSpecifier on the null type reference*/
			if (ace.CreateType.IsNull) { ace.CreateType = new TypeReference(""); }
			pexpr = ace;
			int dims = 0; List<int> ranks = new List<int>();
			
			if (la.kind == 14 || la.kind == 19) {
				while (la.kind == 14) {
					lexer.NextToken();

#line  2100 "cs.ATG" 
					dims += 1; 
				}
				Expect(19);

#line  2101 "cs.ATG" 
				ranks.Add(dims); dims = 0; 
				while (la.kind == 18) {
					lexer.NextToken();
					while (la.kind == 14) {
						lexer.NextToken();

#line  2102 "cs.ATG" 
						++dims; 
					}
					Expect(19);

#line  2102 "cs.ATG" 
					ranks.Add(dims); dims = 0; 
				}

#line  2103 "cs.ATG" 
				ace.CreateType.RankSpecifier = ranks.ToArray(); 
				CollectionInitializer(
#line  2104 "cs.ATG" 
out expr);

#line  2104 "cs.ATG" 
				ace.ArrayInitializer = (CollectionInitializerExpression)expr; 
			} else if (StartOf(6)) {
				Expr(
#line  2105 "cs.ATG" 
out expr);

#line  2105 "cs.ATG" 
				if (expr != null) parameters.Add(expr); 
				while (la.kind == 14) {
					lexer.NextToken();

#line  2106 "cs.ATG" 
					dims += 1; 
					Expr(
#line  2107 "cs.ATG" 
out expr);

#line  2107 "cs.ATG" 
					if (expr != null) parameters.Add(expr); 
				}
				Expect(19);

#line  2109 "cs.ATG" 
				ranks.Add(dims); ace.Arguments = parameters; dims = 0; 
				while (la.kind == 18) {
					lexer.NextToken();
					while (la.kind == 14) {
						lexer.NextToken();

#line  2110 "cs.ATG" 
						++dims; 
					}
					Expect(19);

#line  2110 "cs.ATG" 
					ranks.Add(dims); dims = 0; 
				}

#line  2111 "cs.ATG" 
				ace.CreateType.RankSpecifier = ranks.ToArray(); 
				if (la.kind == 16) {
					CollectionInitializer(
#line  2112 "cs.ATG" 
out expr);

#line  2112 "cs.ATG" 
					ace.ArrayInitializer = (CollectionInitializerExpression)expr; 
				}
			} else SynErr(211);
		} else SynErr(212);
	}

	void PointerMemberAccess(
#line  2053 "cs.ATG" 
out Expression expr, Expression target) {

#line  2054 "cs.ATG" 
		List<TypeReference> typeList; 
		Expect(47);
		Identifier();

#line  2058 "cs.ATG" 
		expr = new PointerReferenceExpression(target, t.val); expr.StartLocation = t.Location; expr.EndLocation = t.EndLocation; 
		if (
#line  2059 "cs.ATG" 
IsGenericInSimpleNameOrMemberAccess()) {
			TypeArgumentList(
#line  2060 "cs.ATG" 
out typeList, false);

#line  2061 "cs.ATG" 
			((MemberReferenceExpression)expr).TypeArguments = typeList; 
		}
	}

	void MemberAccess(
#line  2034 "cs.ATG" 
out Expression expr, Expression target) {

#line  2035 "cs.ATG" 
		List<TypeReference> typeList; 

#line  2037 "cs.ATG" 
		if (ShouldConvertTargetExpressionToTypeReference(target)) {
		TypeReference type = GetTypeReferenceFromExpression(target);
		if (type != null) {
			target = new TypeReferenceExpression(type) { StartLocation = t.Location, EndLocation = t.EndLocation };
		}
		}
		
		Expect(15);

#line  2044 "cs.ATG" 
		Location startLocation = t.Location; 
		Identifier();

#line  2046 "cs.ATG" 
		expr = new MemberReferenceExpression(target, t.val); expr.StartLocation = startLocation; expr.EndLocation = t.EndLocation; 
		if (
#line  2047 "cs.ATG" 
IsGenericInSimpleNameOrMemberAccess()) {
			TypeArgumentList(
#line  2048 "cs.ATG" 
out typeList, false);

#line  2049 "cs.ATG" 
			((MemberReferenceExpression)expr).TypeArguments = typeList; 
		}
	}

	void LambdaExpressionParameter(
#line  2152 "cs.ATG" 
out ParameterDeclarationExpression p) {

#line  2153 "cs.ATG" 
		Location start = la.Location; p = null;
		TypeReference type;
		ParameterModifiers mod = ParameterModifiers.In;
		
		if (
#line  2158 "cs.ATG" 
Peek(1).kind == Tokens.Comma || Peek(1).kind == Tokens.CloseParenthesis) {
			Identifier();

#line  2160 "cs.ATG" 
			p = new ParameterDeclarationExpression(null, t.val);
			p.StartLocation = start; p.EndLocation = t.EndLocation;
			
		} else if (StartOf(36)) {
			if (la.kind == 93 || la.kind == 100) {
				if (la.kind == 100) {
					lexer.NextToken();

#line  2163 "cs.ATG" 
					mod = ParameterModifiers.Ref; 
				} else {
					lexer.NextToken();

#line  2164 "cs.ATG" 
					mod = ParameterModifiers.Out; 
				}
			}
			Type(
#line  2166 "cs.ATG" 
out type);
			Identifier();

#line  2168 "cs.ATG" 
			p = new ParameterDeclarationExpression(type, t.val, mod);
			p.StartLocation = start; p.EndLocation = t.EndLocation;
			
		} else SynErr(213);
	}

	void LambdaExpressionBody(
#line  2174 "cs.ATG" 
LambdaExpression lambda) {

#line  2175 "cs.ATG" 
		Expression expr; BlockStatement stmt; 
		if (la.kind == 16) {
			Block(
#line  2178 "cs.ATG" 
out stmt);

#line  2178 "cs.ATG" 
			lambda.StatementBody = stmt; 
		} else if (StartOf(6)) {
			Expr(
#line  2179 "cs.ATG" 
out expr);

#line  2179 "cs.ATG" 
			lambda.ExpressionBody = expr; 
		} else SynErr(214);

#line  2181 "cs.ATG" 
		lambda.EndLocation = t.EndLocation; 

#line  2182 "cs.ATG" 
		lambda.ExtendedEndLocation = la.Location; 
	}

	void ConditionalAndExpr(
#line  2210 "cs.ATG" 
ref Expression outExpr) {

#line  2211 "cs.ATG" 
		Expression expr; Location startLocation = la.Location; 
		InclusiveOrExpr(
#line  2213 "cs.ATG" 
ref outExpr);
		while (la.kind == 25) {
			lexer.NextToken();
			UnaryExpr(
#line  2213 "cs.ATG" 
out expr);
			InclusiveOrExpr(
#line  2213 "cs.ATG" 
ref expr);

#line  2213 "cs.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.LogicalAnd, expr) { StartLocation = startLocation, EndLocation = t.EndLocation };  
		}
	}

	void InclusiveOrExpr(
#line  2216 "cs.ATG" 
ref Expression outExpr) {

#line  2217 "cs.ATG" 
		Expression expr; Location startLocation = la.Location; 
		ExclusiveOrExpr(
#line  2219 "cs.ATG" 
ref outExpr);
		while (la.kind == 29) {
			lexer.NextToken();
			UnaryExpr(
#line  2219 "cs.ATG" 
out expr);
			ExclusiveOrExpr(
#line  2219 "cs.ATG" 
ref expr);

#line  2219 "cs.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.BitwiseOr, expr) { StartLocation = startLocation, EndLocation = t.EndLocation };  
		}
	}

	void ExclusiveOrExpr(
#line  2222 "cs.ATG" 
ref Expression outExpr) {

#line  2223 "cs.ATG" 
		Expression expr; Location startLocation = la.Location; 
		AndExpr(
#line  2225 "cs.ATG" 
ref outExpr);
		while (la.kind == 30) {
			lexer.NextToken();
			UnaryExpr(
#line  2225 "cs.ATG" 
out expr);
			AndExpr(
#line  2225 "cs.ATG" 
ref expr);

#line  2225 "cs.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.ExclusiveOr, expr) { StartLocation = startLocation, EndLocation = t.EndLocation };  
		}
	}

	void AndExpr(
#line  2228 "cs.ATG" 
ref Expression outExpr) {

#line  2229 "cs.ATG" 
		Expression expr; Location startLocation = la.Location; 
		EqualityExpr(
#line  2231 "cs.ATG" 
ref outExpr);
		while (la.kind == 28) {
			lexer.NextToken();
			UnaryExpr(
#line  2231 "cs.ATG" 
out expr);
			EqualityExpr(
#line  2231 "cs.ATG" 
ref expr);

#line  2231 "cs.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.BitwiseAnd, expr) { StartLocation = startLocation, EndLocation = t.EndLocation };  
		}
	}

	void EqualityExpr(
#line  2234 "cs.ATG" 
ref Expression outExpr) {

#line  2236 "cs.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		Location startLocation = la.Location;
		
		RelationalExpr(
#line  2241 "cs.ATG" 
ref outExpr);
		while (la.kind == 33 || la.kind == 34) {
			if (la.kind == 34) {
				lexer.NextToken();

#line  2244 "cs.ATG" 
				op = BinaryOperatorType.InEquality; 
			} else {
				lexer.NextToken();

#line  2245 "cs.ATG" 
				op = BinaryOperatorType.Equality; 
			}
			UnaryExpr(
#line  2247 "cs.ATG" 
out expr);
			RelationalExpr(
#line  2247 "cs.ATG" 
ref expr);

#line  2247 "cs.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr) { StartLocation = startLocation, EndLocation = t.EndLocation };  
		}
	}

	void RelationalExpr(
#line  2251 "cs.ATG" 
ref Expression outExpr) {

#line  2253 "cs.ATG" 
		TypeReference type;
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		Location startLocation = la.Location;
		
		ShiftExpr(
#line  2259 "cs.ATG" 
ref outExpr);
		while (StartOf(37)) {
			if (StartOf(38)) {
				if (la.kind == 23) {
					lexer.NextToken();

#line  2261 "cs.ATG" 
					op = BinaryOperatorType.LessThan; 
				} else if (la.kind == 22) {
					lexer.NextToken();

#line  2262 "cs.ATG" 
					op = BinaryOperatorType.GreaterThan; 
				} else if (la.kind == 36) {
					lexer.NextToken();

#line  2263 "cs.ATG" 
					op = BinaryOperatorType.LessThanOrEqual; 
				} else if (la.kind == 35) {
					lexer.NextToken();

#line  2264 "cs.ATG" 
					op = BinaryOperatorType.GreaterThanOrEqual; 
				} else SynErr(215);
				UnaryExpr(
#line  2266 "cs.ATG" 
out expr);
				ShiftExpr(
#line  2267 "cs.ATG" 
ref expr);

#line  2268 "cs.ATG" 
				outExpr = new BinaryOperatorExpression(outExpr, op, expr) { StartLocation = startLocation, EndLocation = t.EndLocation }; 
			} else {
				if (la.kind == 85) {
					lexer.NextToken();
					TypeWithRestriction(
#line  2271 "cs.ATG" 
out type, false, false);
					if (
#line  2272 "cs.ATG" 
la.kind == Tokens.Question && !IsPossibleExpressionStart(Peek(1).kind)) {
						NullableQuestionMark(
#line  2273 "cs.ATG" 
ref type);
					}

#line  2274 "cs.ATG" 
					outExpr = new TypeOfIsExpression(outExpr, type)  { StartLocation = startLocation, EndLocation = t.EndLocation }; 
				} else if (la.kind == 50) {
					lexer.NextToken();
					TypeWithRestriction(
#line  2276 "cs.ATG" 
out type, false, false);
					if (
#line  2277 "cs.ATG" 
la.kind == Tokens.Question && !IsPossibleExpressionStart(Peek(1).kind)) {
						NullableQuestionMark(
#line  2278 "cs.ATG" 
ref type);
					}

#line  2279 "cs.ATG" 
					outExpr = new CastExpression(type, outExpr, CastType.TryCast) { StartLocation = startLocation, EndLocation = t.EndLocation }; 
				} else SynErr(216);
			}
		}
	}

	void ShiftExpr(
#line  2284 "cs.ATG" 
ref Expression outExpr) {

#line  2286 "cs.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		Location startLocation = la.Location;
		
		AdditiveExpr(
#line  2291 "cs.ATG" 
ref outExpr);
		while (la.kind == 37 || 
#line  2294 "cs.ATG" 
IsShiftRight()) {
			if (la.kind == 37) {
				lexer.NextToken();

#line  2293 "cs.ATG" 
				op = BinaryOperatorType.ShiftLeft; 
			} else {
				Expect(22);
				Expect(22);

#line  2295 "cs.ATG" 
				op = BinaryOperatorType.ShiftRight; 
			}
			UnaryExpr(
#line  2298 "cs.ATG" 
out expr);
			AdditiveExpr(
#line  2298 "cs.ATG" 
ref expr);

#line  2298 "cs.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr) { StartLocation = startLocation, EndLocation = t.EndLocation };  
		}
	}

	void AdditiveExpr(
#line  2302 "cs.ATG" 
ref Expression outExpr) {

#line  2304 "cs.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		Location startLocation = la.Location;
		
		MultiplicativeExpr(
#line  2309 "cs.ATG" 
ref outExpr);
		while (la.kind == 4 || la.kind == 5) {
			if (la.kind == 4) {
				lexer.NextToken();

#line  2312 "cs.ATG" 
				op = BinaryOperatorType.Add; 
			} else {
				lexer.NextToken();

#line  2313 "cs.ATG" 
				op = BinaryOperatorType.Subtract; 
			}
			UnaryExpr(
#line  2315 "cs.ATG" 
out expr);
			MultiplicativeExpr(
#line  2315 "cs.ATG" 
ref expr);

#line  2315 "cs.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr) { StartLocation = startLocation, EndLocation = t.EndLocation };  
		}
	}

	void MultiplicativeExpr(
#line  2319 "cs.ATG" 
ref Expression outExpr) {

#line  2321 "cs.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		Location startLocation = la.Location;
		
		while (la.kind == 6 || la.kind == 7 || la.kind == 8) {
			if (la.kind == 6) {
				lexer.NextToken();

#line  2328 "cs.ATG" 
				op = BinaryOperatorType.Multiply; 
			} else if (la.kind == 7) {
				lexer.NextToken();

#line  2329 "cs.ATG" 
				op = BinaryOperatorType.Divide; 
			} else {
				lexer.NextToken();

#line  2330 "cs.ATG" 
				op = BinaryOperatorType.Modulus; 
			}
			UnaryExpr(
#line  2332 "cs.ATG" 
out expr);

#line  2332 "cs.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr) { StartLocation = startLocation, EndLocation = t.EndLocation }; 
		}
	}

	void VariantTypeParameter(
#line  2410 "cs.ATG" 
out TemplateDefinition typeParameter) {

#line  2412 "cs.ATG" 
		typeParameter = new TemplateDefinition();
		AttributeSection section;
		
		while (la.kind == 18) {
			AttributeSection(
#line  2416 "cs.ATG" 
out section);

#line  2416 "cs.ATG" 
			typeParameter.Attributes.Add(section); 
		}
		if (la.kind == 81 || la.kind == 93) {
			if (la.kind == 81) {
				lexer.NextToken();

#line  2418 "cs.ATG" 
				typeParameter.VarianceModifier = VarianceModifier.Contravariant; 
			} else {
				lexer.NextToken();

#line  2419 "cs.ATG" 
				typeParameter.VarianceModifier = VarianceModifier.Covariant; 
			}
		}
		Identifier();

#line  2421 "cs.ATG" 
		typeParameter.Name = t.val; typeParameter.StartLocation = t.Location; 

#line  2422 "cs.ATG" 
		typeParameter.EndLocation = t.EndLocation; 
	}

	void TypeParameterConstraintsClauseBase(
#line  2453 "cs.ATG" 
out TypeReference type) {

#line  2454 "cs.ATG" 
		TypeReference t; type = null; 
		if (la.kind == 109) {
			lexer.NextToken();

#line  2456 "cs.ATG" 
			type = TypeReference.StructConstraint; 
		} else if (la.kind == 59) {
			lexer.NextToken();

#line  2457 "cs.ATG" 
			type = TypeReference.ClassConstraint; 
		} else if (la.kind == 89) {
			lexer.NextToken();
			Expect(20);
			Expect(21);

#line  2458 "cs.ATG" 
			type = TypeReference.NewConstraint; 
		} else if (StartOf(10)) {
			Type(
#line  2459 "cs.ATG" 
out t);

#line  2459 "cs.ATG" 
			type = t; 
		} else SynErr(217);
	}

	void QueryExpressionFromClause(
#line  2474 "cs.ATG" 
out QueryExpressionFromClause fc) {

#line  2475 "cs.ATG" 
		fc = new QueryExpressionFromClause();
		fc.StartLocation = la.Location;
		CollectionRangeVariable variable;
		
		Expect(137);
		QueryExpressionFromOrJoinClause(
#line  2481 "cs.ATG" 
out variable);

#line  2482 "cs.ATG" 
		fc.EndLocation = t.EndLocation;
		fc.Sources.Add(variable);
		
	}

	void QueryExpressionBody(
#line  2518 "cs.ATG" 
ref QueryExpression q) {

#line  2519 "cs.ATG" 
		QueryExpressionFromClause fromClause;     QueryExpressionWhereClause whereClause;
		QueryExpressionLetClause letClause;       QueryExpressionJoinClause joinClause;
		QueryExpressionOrderClause orderClause;
		QueryExpressionSelectClause selectClause; QueryExpressionGroupClause groupClause;
		
		while (StartOf(39)) {
			if (la.kind == 137) {
				QueryExpressionFromClause(
#line  2525 "cs.ATG" 
out fromClause);

#line  2525 "cs.ATG" 
				SafeAdd<QueryExpressionClause>(q, q.MiddleClauses, fromClause); 
			} else if (la.kind == 127) {
				QueryExpressionWhereClause(
#line  2526 "cs.ATG" 
out whereClause);

#line  2526 "cs.ATG" 
				SafeAdd<QueryExpressionClause>(q, q.MiddleClauses, whereClause); 
			} else if (la.kind == 141) {
				QueryExpressionLetClause(
#line  2527 "cs.ATG" 
out letClause);

#line  2527 "cs.ATG" 
				SafeAdd<QueryExpressionClause>(q, q.MiddleClauses, letClause); 
			} else if (la.kind == 142) {
				QueryExpressionJoinClause(
#line  2528 "cs.ATG" 
out joinClause);

#line  2528 "cs.ATG" 
				SafeAdd<QueryExpressionClause>(q, q.MiddleClauses, joinClause); 
			} else {
				QueryExpressionOrderByClause(
#line  2529 "cs.ATG" 
out orderClause);

#line  2529 "cs.ATG" 
				SafeAdd<QueryExpressionClause>(q, q.MiddleClauses, orderClause); 
			}
		}
		if (la.kind == 133) {
			QueryExpressionSelectClause(
#line  2531 "cs.ATG" 
out selectClause);

#line  2531 "cs.ATG" 
			q.SelectOrGroupClause = selectClause; 
		} else if (la.kind == 134) {
			QueryExpressionGroupClause(
#line  2532 "cs.ATG" 
out groupClause);

#line  2532 "cs.ATG" 
			q.SelectOrGroupClause = groupClause; 
		} else SynErr(218);
		if (la.kind == 136) {
			QueryExpressionIntoClause(
#line  2534 "cs.ATG" 
ref q);
		}
	}

	void QueryExpressionFromOrJoinClause(
#line  2508 "cs.ATG" 
out CollectionRangeVariable variable) {

#line  2509 "cs.ATG" 
		TypeReference type; Expression expr; variable = new CollectionRangeVariable(); 

#line  2511 "cs.ATG" 
		variable.Type = null; 
		if (
#line  2512 "cs.ATG" 
IsLocalVarDecl()) {
			Type(
#line  2512 "cs.ATG" 
out type);

#line  2512 "cs.ATG" 
			variable.Type = type; 
		}
		Identifier();

#line  2513 "cs.ATG" 
		variable.Identifier = t.val; 
		Expect(81);
		Expr(
#line  2515 "cs.ATG" 
out expr);

#line  2515 "cs.ATG" 
		variable.Expression = expr; 
	}

	void QueryExpressionJoinClause(
#line  2487 "cs.ATG" 
out QueryExpressionJoinClause jc) {

#line  2488 "cs.ATG" 
		jc = new QueryExpressionJoinClause(); jc.StartLocation = la.Location; 
		Expression expr;
		CollectionRangeVariable variable;
		
		Expect(142);
		QueryExpressionFromOrJoinClause(
#line  2494 "cs.ATG" 
out variable);
		Expect(143);
		Expr(
#line  2496 "cs.ATG" 
out expr);

#line  2496 "cs.ATG" 
		jc.OnExpression = expr; 
		Expect(144);
		Expr(
#line  2498 "cs.ATG" 
out expr);

#line  2498 "cs.ATG" 
		jc.EqualsExpression = expr; 
		if (la.kind == 136) {
			lexer.NextToken();
			Identifier();

#line  2500 "cs.ATG" 
			jc.IntoIdentifier = t.val; 
		}

#line  2503 "cs.ATG" 
		jc.EndLocation = t.EndLocation;
		jc.Source = variable;
		
	}

	void QueryExpressionWhereClause(
#line  2537 "cs.ATG" 
out QueryExpressionWhereClause wc) {

#line  2538 "cs.ATG" 
		Expression expr; wc = new QueryExpressionWhereClause(); wc.StartLocation = la.Location; 
		Expect(127);
		Expr(
#line  2541 "cs.ATG" 
out expr);

#line  2541 "cs.ATG" 
		wc.Condition = expr; 

#line  2542 "cs.ATG" 
		wc.EndLocation = t.EndLocation; 
	}

	void QueryExpressionLetClause(
#line  2545 "cs.ATG" 
out QueryExpressionLetClause wc) {

#line  2546 "cs.ATG" 
		Expression expr; wc = new QueryExpressionLetClause(); wc.StartLocation = la.Location; 
		Expect(141);
		Identifier();

#line  2549 "cs.ATG" 
		wc.Identifier = t.val; 
		Expect(3);
		Expr(
#line  2551 "cs.ATG" 
out expr);

#line  2551 "cs.ATG" 
		wc.Expression = expr; 

#line  2552 "cs.ATG" 
		wc.EndLocation = t.EndLocation; 
	}

	void QueryExpressionOrderByClause(
#line  2555 "cs.ATG" 
out QueryExpressionOrderClause oc) {

#line  2556 "cs.ATG" 
		QueryExpressionOrdering ordering; oc = new QueryExpressionOrderClause(); oc.StartLocation = la.Location; 
		Expect(140);
		QueryExpressionOrdering(
#line  2559 "cs.ATG" 
out ordering);

#line  2559 "cs.ATG" 
		SafeAdd(oc, oc.Orderings, ordering); 
		while (la.kind == 14) {
			lexer.NextToken();
			QueryExpressionOrdering(
#line  2561 "cs.ATG" 
out ordering);

#line  2561 "cs.ATG" 
			SafeAdd(oc, oc.Orderings, ordering); 
		}

#line  2563 "cs.ATG" 
		oc.EndLocation = t.EndLocation; 
	}

	void QueryExpressionSelectClause(
#line  2576 "cs.ATG" 
out QueryExpressionSelectClause sc) {

#line  2577 "cs.ATG" 
		Expression expr; sc = new QueryExpressionSelectClause(); sc.StartLocation = la.Location; 
		Expect(133);
		Expr(
#line  2580 "cs.ATG" 
out expr);

#line  2580 "cs.ATG" 
		sc.Projection = expr; 

#line  2581 "cs.ATG" 
		sc.EndLocation = t.EndLocation; 
	}

	void QueryExpressionGroupClause(
#line  2584 "cs.ATG" 
out QueryExpressionGroupClause gc) {

#line  2585 "cs.ATG" 
		Expression expr; gc = new QueryExpressionGroupClause(); gc.StartLocation = la.Location; 
		Expect(134);
		Expr(
#line  2588 "cs.ATG" 
out expr);

#line  2588 "cs.ATG" 
		gc.Projection = expr; 
		Expect(135);
		Expr(
#line  2590 "cs.ATG" 
out expr);

#line  2590 "cs.ATG" 
		gc.GroupBy = expr; 

#line  2591 "cs.ATG" 
		gc.EndLocation = t.EndLocation; 
	}

	void QueryExpressionIntoClause(
#line  2594 "cs.ATG" 
ref QueryExpression q) {

#line  2595 "cs.ATG" 
		QueryExpression firstQuery = q;
		QueryExpression continuedQuery = new QueryExpression(); 
		continuedQuery.StartLocation = q.StartLocation;
		firstQuery.EndLocation = la.Location;
		continuedQuery.FromClause = new QueryExpressionFromClause();
		CollectionRangeVariable fromVariable = new CollectionRangeVariable();
		continuedQuery.FromClause.Sources.Add(fromVariable);
		fromVariable.StartLocation = la.Location;
		// nest firstQuery inside continuedQuery.
		fromVariable.Expression = firstQuery;
		continuedQuery.IsQueryContinuation = true;
		q = continuedQuery;
		
		Expect(136);
		Identifier();

#line  2610 "cs.ATG" 
		fromVariable.Identifier = t.val; 

#line  2611 "cs.ATG" 
		continuedQuery.FromClause.EndLocation = t.EndLocation; 
		QueryExpressionBody(
#line  2612 "cs.ATG" 
ref q);
	}

	void QueryExpressionOrdering(
#line  2566 "cs.ATG" 
out QueryExpressionOrdering ordering) {

#line  2567 "cs.ATG" 
		Expression expr; ordering = new QueryExpressionOrdering(); ordering.StartLocation = la.Location; 
		Expr(
#line  2569 "cs.ATG" 
out expr);

#line  2569 "cs.ATG" 
		ordering.Criteria = expr; 
		if (la.kind == 138 || la.kind == 139) {
			if (la.kind == 138) {
				lexer.NextToken();

#line  2570 "cs.ATG" 
				ordering.Direction = QueryExpressionOrderingDirection.Ascending; 
			} else {
				lexer.NextToken();

#line  2571 "cs.ATG" 
				ordering.Direction = QueryExpressionOrderingDirection.Descending; 
			}
		}

#line  2573 "cs.ATG" 
		ordering.EndLocation = t.EndLocation; 
	}


	
	void ParseRoot()
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
			case 145: s = "\"async\" expected"; break;
			case 146: s = "\"await\" expected"; break;
			case 147: s = "??? expected"; break;
			case 148: s = "invalid NamespaceMemberDecl"; break;
			case 149: s = "invalid Identifier"; break;
			case 150: s = "invalid NonArrayType"; break;
			case 151: s = "invalid AttributeArgument"; break;
			case 152: s = "invalid Expr"; break;
			case 153: s = "invalid TypeModifier"; break;
			case 154: s = "invalid TypeDecl"; break;
			case 155: s = "invalid TypeDecl"; break;
			case 156: s = "this symbol not expected in ClassBody"; break;
			case 157: s = "this symbol not expected in InterfaceBody"; break;
			case 158: s = "invalid IntegralType"; break;
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
			case 182: s = "this symbol not expected in Block"; break;
			case 183: s = "invalid EventAccessorDecls"; break;
			case 184: s = "invalid ConstructorInitializer"; break;
			case 185: s = "invalid OverloadableOperator"; break;
			case 186: s = "invalid AccessorDecls"; break;
			case 187: s = "invalid InterfaceAccessors"; break;
			case 188: s = "invalid InterfaceAccessors"; break;
			case 189: s = "invalid GetAccessorDecl"; break;
			case 190: s = "invalid SetAccessorDecl"; break;
			case 191: s = "invalid VariableInitializer"; break;
			case 192: s = "this symbol not expected in Statement"; break;
			case 193: s = "invalid Statement"; break;
			case 194: s = "invalid Argument"; break;
			case 195: s = "invalid AssignmentOperator"; break;
			case 196: s = "invalid ObjectPropertyInitializerOrVariableInitializer"; break;
			case 197: s = "invalid ObjectPropertyInitializerOrVariableInitializer"; break;
			case 198: s = "invalid EmbeddedStatement"; break;
			case 199: s = "invalid EmbeddedStatement"; break;
			case 200: s = "this symbol not expected in EmbeddedStatement"; break;
			case 201: s = "invalid EmbeddedStatement"; break;
			case 202: s = "invalid ForInitializer"; break;
			case 203: s = "invalid GotoStatement"; break;
			case 204: s = "invalid ResourceAcquisition"; break;
			case 205: s = "invalid SwitchLabel"; break;
			case 206: s = "invalid CatchClause"; break;
			case 207: s = "invalid UnaryExpr"; break;
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
	{T,T,T,x, T,T,T,x, x,x,x,T, x,x,x,x, T,T,T,x, T,x,x,x, T,x,x,T, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, T,T,T,x, x,T,T,T, T,T,T,T, T,T,T,x, T,T,T,T, T,x,T,T, T,T,T,T, T,x,T,T, T,x,T,T, x,T,T,T, x,x,T,x, T,T,T,T, x,T,T,T, T,T,x,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, T,T,x,x, x,x,x,x, T,T,T,x, x,x,x,T, x,x,x,T, x,T,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,T,x,x, x,x,x,x, T,T,T,x, x,x,x,T, x,x,x,T, x,T,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, T,T,T,x, x,x,x,T, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,x, x,T,x,x, x,x,T,x, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
	{x,T,T,x, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, T,x,x,T, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,T,x, x,T,T,x, x,x,T,T, T,x,T,x, x,x,x,x, T,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,T,T,T, x,x,x,x, x,x,x,x, x,x,T,x, T,T,x,x, T,x,x,T, x,T,x,T, T,T,T,x, T,x,x,T, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, x},
	{x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
	{x,T,x,T, T,T,T,T, T,T,x,T, T,T,T,T, T,T,T,T, T,T,T,T, x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, x,T,T,x, T,x,T,x, x,T,x,T, T,x,T,x, T,x,T,x, T,T,T,T, x,x,T,T, x,x,x,x, T,x,T,T, T,T,x,T, x,T,x,T, x,x,T,x, T,T,T,T, x,x,T,T, T,x,x,T, T,T,x,x, x,x,x,x, T,T,x,T, T,x,T,T, T,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,x, x,T,x,x, x,x,T,x, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,T, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,x, T,x,x,x, x,x,x,x, T,T,x,x, T,x,x,T, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,x, x,T,x,x, x,x,T,x, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,T, x,T,x,T, x,x,x,x, T,x,T,x, T,x,x,x, T,x,x,x, x,x,x,x, T,T,x,x, T,x,x,T, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, T,x,T,x, x,T,x,T, T,x,T,x, T,x,T,x, T,T,T,T, x,x,T,T, x,x,x,x, T,x,T,T, T,x,x,T, x,T,x,T, x,x,T,x, T,T,T,T, x,x,T,T, T,x,x,T, T,T,x,x, x,x,x,x, T,T,x,T, T,x,T,T, T,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, x},
	{T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, T,x,T,x, x,T,x,T, T,x,T,x, T,x,T,x, T,T,T,T, x,x,T,T, x,x,x,x, T,x,T,T, T,x,x,T, x,T,x,T, x,x,T,x, T,T,T,T, x,x,T,T, T,x,x,T, T,T,x,x, x,x,x,x, T,T,x,T, T,x,T,T, T,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, T,x,T,x, x,T,x,T, T,x,T,x, T,x,T,x, T,T,T,T, x,x,T,T, x,x,x,x, T,x,T,T, T,x,x,T, x,T,x,T, x,x,T,x, T,T,T,T, x,x,T,T, T,x,x,T, T,T,x,x, x,x,x,x, T,T,x,T, T,x,T,T, T,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,x, x,T,x,x, x,x,T,x, x,x,T,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,T,x,T, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,x, T,x,x,x, x,x,x,x, T,T,x,x, T,x,x,T, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, x},
	{T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,x, x,T,x,x, x,x,T,x, x,x,T,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,T,x,T, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,x, T,x,x,x, x,x,x,x, T,T,x,x, T,x,x,T, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,x, T,x,x,x, x,T,x,x, x,x,T,x, T,T,T,T, x,x,x,T, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,x, T,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,x, x,T,x,T, T,x,T,x, T,x,T,x, T,T,T,x, x,x,x,T, x,x,x,x, T,x,T,T, x,x,x,T, x,x,x,T, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,x, T,T,x,x, x,x,x,x, T,T,x,x, T,x,x,T, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,x, x,T,x,x, x,x,T,x, x,x,T,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,T, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,x, T,x,x,x, x,x,x,x, T,T,x,x, T,x,x,T, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
	{x,T,T,x, T,T,T,x, x,x,x,T, x,x,x,x, T,x,x,x, T,x,x,x, T,x,x,T, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,T,x, x,T,T,x, T,T,T,T, T,T,T,x, x,x,x,x, T,x,T,T, T,T,T,T, x,x,T,x, x,x,T,T, x,T,T,T, x,x,x,x, x,x,x,x, x,T,T,x, T,T,x,x, T,x,T,T, T,T,T,T, T,T,T,T, T,T,x,T, x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, x},
	{x,T,T,x, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, T,x,x,T, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,T,x, x,T,T,x, x,x,T,T, T,x,T,x, x,x,x,x, T,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,T,T,T, x,T,x,x, x,x,x,x, T,x,T,x, T,T,x,x, T,x,x,T, x,T,x,T, T,T,T,x, T,x,x,T, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
	{T,T,T,x, T,T,T,x, x,x,x,T, x,x,x,x, T,x,x,x, T,x,x,x, T,x,x,T, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,T,x, x,T,T,x, T,T,T,T, T,T,T,x, x,x,x,x, T,x,T,T, T,T,T,T, x,x,T,x, x,x,T,T, x,T,T,T, x,x,x,x, x,x,x,x, x,T,T,x, T,T,x,x, T,x,T,T, T,T,T,T, T,T,T,T, T,T,x,T, x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, x},
	{x,T,T,x, T,T,T,x, x,x,x,T, x,x,x,x, T,x,x,x, T,x,x,x, T,x,x,T, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,T,x, x,T,T,x, x,T,T,T, T,T,T,x, x,x,x,x, T,x,T,T, T,T,T,T, x,x,T,x, x,x,T,T, x,T,T,T, x,x,x,x, x,x,x,x, x,T,T,x, T,T,x,x, T,x,T,T, T,T,T,T, T,T,T,T, T,T,x,T, x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, x},
	{x,T,T,x, T,T,T,x, x,x,x,x, x,x,x,x, T,x,x,x, T,x,x,x, T,x,x,T, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,T,x, x,T,T,x, x,x,T,T, T,x,T,x, x,x,x,x, T,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,T,T,T, x,x,x,x, x,x,x,x, x,x,T,x, T,T,T,x, T,x,x,T, x,T,x,T, T,T,T,x, T,x,x,T, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, x},
	{x,T,T,x, T,T,T,x, x,x,x,T, x,x,x,x, T,x,x,x, T,x,x,x, T,x,x,T, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,T,T, x,T,T,x, T,T,T,T, T,T,T,x, x,x,x,x, T,x,T,T, T,T,T,T, x,x,T,x, x,x,T,T, x,T,T,T, x,x,x,x, x,x,x,x, x,T,T,x, T,T,x,x, T,x,T,T, T,T,T,T, T,T,T,T, T,T,x,T, x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, x},
	{x,x,x,x, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,T, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x},
	{x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,T,x, x,T,T,x, x,x,T,T, T,x,T,x, x,x,x,x, T,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,T,T,T, x,x,x,x, x,x,x,x, x,x,T,x, T,T,x,x, T,x,x,T, x,T,x,T, T,T,T,x, T,x,x,T, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,x, x,T,x,x, x,x,T,x, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,T, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,x, T,x,x,x, x,x,x,x, T,T,x,x, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,x, x,T,x,x, x,x,T,x, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,T, x,T,x,x, x,x,x,x, T,x,T,x, T,x,x,x, T,x,x,x, x,x,x,x, T,T,x,x, T,x,x,T, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,T,x,x, T,T,T,x, x,x,x,x, x}

	};
} // end Parser

}