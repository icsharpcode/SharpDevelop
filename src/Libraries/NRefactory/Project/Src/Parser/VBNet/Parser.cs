
#line  1 "VBNET.ATG" 
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.Parser.VB;
using ASTAttribute = ICSharpCode.NRefactory.Ast.Attribute;
/*
  Parser.frame file for NRefactory.
 */
using System;
using System.Reflection;

namespace ICSharpCode.NRefactory.Parser.VB {



partial class Parser : AbstractParser
{
	const int maxT = 233;

	const  bool   T            = true;
	const  bool   x            = false;
	

#line  12 "VBNET.ATG" 


/*

*/

	void VBNET() {

#line  257 "VBNET.ATG" 
		lexer.NextToken(); // get the first token
		compilationUnit = new CompilationUnit();
		
		while (la.kind == 1 || la.kind == 20) {
			EndOfStmt();
		}
		while (la.kind == 170) {
			OptionStmt();
			while (la.kind == 1 || la.kind == 20) {
				EndOfStmt();
			}
		}
		while (la.kind == 134) {
			ImportsStmt();
			while (la.kind == 1 || la.kind == 20) {
				EndOfStmt();
			}
		}
		while (
#line  264 "VBNET.ATG" 
IsGlobalAttrTarget()) {
			GlobalAttributeSection();
			while (la.kind == 1 || la.kind == 20) {
				EndOfStmt();
			}
		}
		while (StartOf(1)) {
			NamespaceMemberDecl();
			while (la.kind == 1 || la.kind == 20) {
				EndOfStmt();
			}
		}
		Expect(0);
	}

	void EndOfStmt() {
		if (la.kind == 1) {
			lexer.NextToken();
		} else if (la.kind == 20) {
			lexer.NextToken();
		} else SynErr(234);
	}

	void OptionStmt() {

#line  269 "VBNET.ATG" 
		INode node = null; bool val = true; 
		Expect(170);

#line  270 "VBNET.ATG" 
		Location startPos = t.Location; 
		if (la.kind == 118) {
			lexer.NextToken();
			if (la.kind == 167 || la.kind == 168) {
				OptionValue(
#line  272 "VBNET.ATG" 
ref val);
			}

#line  273 "VBNET.ATG" 
			node = new OptionDeclaration(OptionType.Explicit, val); 
		} else if (la.kind == 203) {
			lexer.NextToken();
			if (la.kind == 167 || la.kind == 168) {
				OptionValue(
#line  275 "VBNET.ATG" 
ref val);
			}

#line  276 "VBNET.ATG" 
			node = new OptionDeclaration(OptionType.Strict, val); 
		} else if (la.kind == 84) {
			lexer.NextToken();
			if (la.kind == 64) {
				lexer.NextToken();

#line  278 "VBNET.ATG" 
				node = new OptionDeclaration(OptionType.CompareBinary, val); 
			} else if (la.kind == 209) {
				lexer.NextToken();

#line  279 "VBNET.ATG" 
				node = new OptionDeclaration(OptionType.CompareText, val); 
			} else SynErr(235);
		} else if (la.kind == 136) {
			lexer.NextToken();
			if (la.kind == 167 || la.kind == 168) {
				OptionValue(
#line  282 "VBNET.ATG" 
ref val);
			}

#line  283 "VBNET.ATG" 
			node = new OptionDeclaration(OptionType.Infer, val); 
		} else SynErr(236);
		EndOfStmt();

#line  287 "VBNET.ATG" 
		if (node != null) {
		node.StartLocation = startPos;
		node.EndLocation   = t.Location;
		compilationUnit.AddChild(node);
		}
		
	}

	void ImportsStmt() {

#line  310 "VBNET.ATG" 
		List<Using> usings = new List<Using>();
		
		Expect(134);

#line  314 "VBNET.ATG" 
		Location startPos = t.Location;
		Using u;
		
		ImportClause(
#line  317 "VBNET.ATG" 
out u);

#line  317 "VBNET.ATG" 
		if (u != null) { usings.Add(u); } 
		while (la.kind == 21) {
			lexer.NextToken();
			ImportClause(
#line  319 "VBNET.ATG" 
out u);

#line  319 "VBNET.ATG" 
			if (u != null) { usings.Add(u); } 
		}
		EndOfStmt();

#line  323 "VBNET.ATG" 
		UsingDeclaration usingDeclaration = new UsingDeclaration(usings);
		usingDeclaration.StartLocation = startPos;
		usingDeclaration.EndLocation   = t.Location;
		compilationUnit.AddChild(usingDeclaration);
		
	}

	void GlobalAttributeSection() {
		Expect(37);

#line  2585 "VBNET.ATG" 
		Location startPos = t.Location; 
		if (la.kind == 62) {
			lexer.NextToken();
		} else if (la.kind == 152) {
			lexer.NextToken();
		} else SynErr(237);

#line  2587 "VBNET.ATG" 
		string attributeTarget = t.val != null ? t.val.ToLower(System.Globalization.CultureInfo.InvariantCulture) : null;
		List<ASTAttribute> attributes = new List<ASTAttribute>();
		ASTAttribute attribute;
		
		Expect(20);
		Attribute(
#line  2591 "VBNET.ATG" 
out attribute);

#line  2591 "VBNET.ATG" 
		attributes.Add(attribute); 
		while (
#line  2592 "VBNET.ATG" 
NotFinalComma()) {
			if (la.kind == 21) {
				lexer.NextToken();
				if (la.kind == 62) {
					lexer.NextToken();
				} else if (la.kind == 152) {
					lexer.NextToken();
				} else SynErr(238);
				Expect(20);
			}
			Attribute(
#line  2592 "VBNET.ATG" 
out attribute);

#line  2592 "VBNET.ATG" 
			attributes.Add(attribute); 
		}
		if (la.kind == 21) {
			lexer.NextToken();
		}
		Expect(36);
		EndOfStmt();

#line  2597 "VBNET.ATG" 
		AttributeSection section = new AttributeSection {
		AttributeTarget = attributeTarget,
		Attributes = attributes,
		StartLocation = startPos,
		EndLocation = t.EndLocation
		};
		compilationUnit.AddChild(section);
		
	}

	void NamespaceMemberDecl() {

#line  356 "VBNET.ATG" 
		ModifierList m = new ModifierList();
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		string qualident;
		
		if (la.kind == 157) {
			lexer.NextToken();

#line  363 "VBNET.ATG" 
			Location startPos = t.Location;
			
			Qualident(
#line  365 "VBNET.ATG" 
out qualident);

#line  367 "VBNET.ATG" 
			INode node =  new NamespaceDeclaration(qualident);
			node.StartLocation = startPos;
			compilationUnit.AddChild(node);
			compilationUnit.BlockStart(node);
			
			EndOfStmt();
			NamespaceBody();

#line  375 "VBNET.ATG" 
			node.EndLocation = t.Location;
			compilationUnit.BlockEnd();
			
		} else if (StartOf(2)) {
			while (la.kind == 37) {
				AttributeSection(
#line  379 "VBNET.ATG" 
out section);

#line  379 "VBNET.ATG" 
				attributes.Add(section); 
			}
			while (StartOf(3)) {
				TypeModifier(
#line  380 "VBNET.ATG" 
m);
			}
			NonModuleDeclaration(
#line  380 "VBNET.ATG" 
m, attributes);
		} else SynErr(239);
	}

	void OptionValue(
#line  295 "VBNET.ATG" 
ref bool val) {
		if (la.kind == 168) {
			lexer.NextToken();

#line  297 "VBNET.ATG" 
			val = true; 
		} else if (la.kind == 167) {
			lexer.NextToken();

#line  299 "VBNET.ATG" 
			val = false; 
		} else SynErr(240);
	}

	void ImportClause(
#line  330 "VBNET.ATG" 
out Using u) {

#line  332 "VBNET.ATG" 
		string qualident  = null;
		TypeReference aliasedType = null;
		u = null;
		
		if (StartOf(4)) {
			Qualident(
#line  337 "VBNET.ATG" 
out qualident);
			if (la.kind == 19) {
				lexer.NextToken();
				TypeName(
#line  338 "VBNET.ATG" 
out aliasedType);
			}

#line  340 "VBNET.ATG" 
			if (qualident != null && qualident.Length > 0) {
			if (aliasedType != null) {
				u = new Using(qualident, aliasedType);
			} else {
				u = new Using(qualident);
			}
			}
			
		} else if (la.kind == 10) {

#line  348 "VBNET.ATG" 
			string prefix = null; 
			lexer.NextToken();
			Identifier();

#line  349 "VBNET.ATG" 
			prefix = t.val; 
			Expect(19);
			Expect(3);

#line  349 "VBNET.ATG" 
			u = new Using(t.literalValue as string, prefix); 
			Expect(11);
		} else SynErr(241);
	}

	void Qualident(
#line  3343 "VBNET.ATG" 
out string qualident) {

#line  3345 "VBNET.ATG" 
		string name;
		qualidentBuilder.Length = 0; 
		
		Identifier();

#line  3349 "VBNET.ATG" 
		qualidentBuilder.Append(t.val); 
		while (
#line  3350 "VBNET.ATG" 
DotAndIdentOrKw()) {
			Expect(25);
			IdentifierOrKeyword(
#line  3350 "VBNET.ATG" 
out name);

#line  3350 "VBNET.ATG" 
			qualidentBuilder.Append('.'); qualidentBuilder.Append(name); 
		}

#line  3352 "VBNET.ATG" 
		qualident = qualidentBuilder.ToString(); 
	}

	void TypeName(
#line  2458 "VBNET.ATG" 
out TypeReference typeref) {

#line  2459 "VBNET.ATG" 
		ArrayList rank = null; 
		NonArrayTypeName(
#line  2461 "VBNET.ATG" 
out typeref, false);
		ArrayTypeModifiers(
#line  2465 "VBNET.ATG" 
out rank);

#line  2466 "VBNET.ATG" 
		if (rank != null && typeref != null) {
		typeref.RankSpecifier = (int[])rank.ToArray(typeof(int));
		}
		
	}

	void Identifier() {
		if (StartOf(5)) {
			IdentifierForFieldDeclaration();
		} else if (la.kind == 95) {
			lexer.NextToken();
		} else SynErr(242);
	}

	void NamespaceBody() {
		while (la.kind == 1 || la.kind == 20) {
			EndOfStmt();
		}
		while (StartOf(1)) {
			NamespaceMemberDecl();
			while (la.kind == 1 || la.kind == 20) {
				EndOfStmt();
			}
		}
		Expect(110);
		Expect(157);
		EndOfStmt();
	}

	void AttributeSection(
#line  2660 "VBNET.ATG" 
out AttributeSection section) {

#line  2662 "VBNET.ATG" 
		string attributeTarget = "";List<ASTAttribute> attributes = new List<ASTAttribute>();
		ASTAttribute attribute;
		
		
		Expect(37);

#line  2666 "VBNET.ATG" 
		Location startPos = t.Location; 
		if (
#line  2667 "VBNET.ATG" 
IsLocalAttrTarget()) {
			if (la.kind == 116) {
				lexer.NextToken();

#line  2668 "VBNET.ATG" 
				attributeTarget = "event";
			} else if (la.kind == 191) {
				lexer.NextToken();

#line  2669 "VBNET.ATG" 
				attributeTarget = "return";
			} else {
				Identifier();

#line  2672 "VBNET.ATG" 
				string val = t.val.ToLower(System.Globalization.CultureInfo.InvariantCulture);
				if (val != "field"	|| val != "method" ||
					val != "module" || val != "param"  ||
					val != "property" || val != "type")
				Error("attribute target specifier (event, return, field," +
						"method, module, param, property, or type) expected");
				attributeTarget = t.val;
				
			}
			Expect(20);
		}
		Attribute(
#line  2682 "VBNET.ATG" 
out attribute);

#line  2682 "VBNET.ATG" 
		attributes.Add(attribute); 
		while (
#line  2683 "VBNET.ATG" 
NotFinalComma()) {
			Expect(21);
			Attribute(
#line  2683 "VBNET.ATG" 
out attribute);

#line  2683 "VBNET.ATG" 
			attributes.Add(attribute); 
		}
		if (la.kind == 21) {
			lexer.NextToken();
		}
		Expect(36);

#line  2687 "VBNET.ATG" 
		section = new AttributeSection {
		AttributeTarget = attributeTarget,
		Attributes = attributes,
		StartLocation = startPos,
		EndLocation = t.EndLocation
		};
		
	}

	void TypeModifier(
#line  3426 "VBNET.ATG" 
ModifierList m) {
		switch (la.kind) {
		case 184: {
			lexer.NextToken();

#line  3427 "VBNET.ATG" 
			m.Add(Modifiers.Public, t.Location); 
			break;
		}
		case 183: {
			lexer.NextToken();

#line  3428 "VBNET.ATG" 
			m.Add(Modifiers.Protected, t.Location); 
			break;
		}
		case 122: {
			lexer.NextToken();

#line  3429 "VBNET.ATG" 
			m.Add(Modifiers.Internal, t.Location); 
			break;
		}
		case 181: {
			lexer.NextToken();

#line  3430 "VBNET.ATG" 
			m.Add(Modifiers.Private, t.Location); 
			break;
		}
		case 196: {
			lexer.NextToken();

#line  3431 "VBNET.ATG" 
			m.Add(Modifiers.Static, t.Location); 
			break;
		}
		case 195: {
			lexer.NextToken();

#line  3432 "VBNET.ATG" 
			m.Add(Modifiers.New, t.Location); 
			break;
		}
		case 153: {
			lexer.NextToken();

#line  3433 "VBNET.ATG" 
			m.Add(Modifiers.Abstract, t.Location); 
			break;
		}
		case 163: {
			lexer.NextToken();

#line  3434 "VBNET.ATG" 
			m.Add(Modifiers.Sealed, t.Location); 
			break;
		}
		case 179: {
			lexer.NextToken();

#line  3435 "VBNET.ATG" 
			m.Add(Modifiers.Partial, t.Location); 
			break;
		}
		default: SynErr(243); break;
		}
	}

	void NonModuleDeclaration(
#line  440 "VBNET.ATG" 
ModifierList m, List<AttributeSection> attributes) {

#line  442 "VBNET.ATG" 
		TypeReference typeRef = null;
		List<TypeReference> baseInterfaces = null;
		
		switch (la.kind) {
		case 81: {

#line  445 "VBNET.ATG" 
			m.Check(Modifiers.Classes); 
			lexer.NextToken();

#line  448 "VBNET.ATG" 
			TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
			newType.StartLocation = t.Location;
			compilationUnit.AddChild(newType);
			compilationUnit.BlockStart(newType);
			
			newType.Type       = ClassType.Class;
			
			Identifier();

#line  455 "VBNET.ATG" 
			newType.Name = t.val; 
			TypeParameterList(
#line  456 "VBNET.ATG" 
newType.Templates);
			EndOfStmt();

#line  458 "VBNET.ATG" 
			newType.BodyStartLocation = t.Location; 
			if (la.kind == 137) {
				ClassBaseType(
#line  459 "VBNET.ATG" 
out typeRef);

#line  459 "VBNET.ATG" 
				SafeAdd(newType, newType.BaseTypes, typeRef); 
			}
			while (la.kind == 133) {
				TypeImplementsClause(
#line  460 "VBNET.ATG" 
out baseInterfaces);

#line  460 "VBNET.ATG" 
				newType.BaseTypes.AddRange(baseInterfaces); 
			}
			ClassBody(
#line  461 "VBNET.ATG" 
newType);
			Expect(110);
			Expect(81);

#line  462 "VBNET.ATG" 
			newType.EndLocation = t.EndLocation; 
			EndOfStmt();

#line  465 "VBNET.ATG" 
			compilationUnit.BlockEnd();
			
			break;
		}
		case 152: {
			lexer.NextToken();

#line  469 "VBNET.ATG" 
			m.Check(Modifiers.VBModules);
			TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
			compilationUnit.AddChild(newType);
			compilationUnit.BlockStart(newType);
			newType.StartLocation = m.GetDeclarationLocation(t.Location);
			newType.Type = ClassType.Module;
			
			Identifier();

#line  476 "VBNET.ATG" 
			newType.Name = t.val; 
			EndOfStmt();

#line  478 "VBNET.ATG" 
			newType.BodyStartLocation = t.Location; 
			ModuleBody(
#line  479 "VBNET.ATG" 
newType);

#line  481 "VBNET.ATG" 
			compilationUnit.BlockEnd();
			
			break;
		}
		case 205: {
			lexer.NextToken();

#line  485 "VBNET.ATG" 
			m.Check(Modifiers.VBStructures);
			TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
			compilationUnit.AddChild(newType);
			compilationUnit.BlockStart(newType);
			newType.StartLocation = m.GetDeclarationLocation(t.Location);
			newType.Type = ClassType.Struct;
			
			Identifier();

#line  492 "VBNET.ATG" 
			newType.Name = t.val; 
			TypeParameterList(
#line  493 "VBNET.ATG" 
newType.Templates);
			EndOfStmt();

#line  495 "VBNET.ATG" 
			newType.BodyStartLocation = t.Location; 
			while (la.kind == 133) {
				TypeImplementsClause(
#line  496 "VBNET.ATG" 
out baseInterfaces);

#line  496 "VBNET.ATG" 
				newType.BaseTypes.AddRange(baseInterfaces);
			}
			StructureBody(
#line  497 "VBNET.ATG" 
newType);

#line  499 "VBNET.ATG" 
			compilationUnit.BlockEnd();
			
			break;
		}
		case 112: {
			lexer.NextToken();

#line  504 "VBNET.ATG" 
			m.Check(Modifiers.VBEnums);
			TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
			newType.StartLocation = m.GetDeclarationLocation(t.Location);
			compilationUnit.AddChild(newType);
			compilationUnit.BlockStart(newType);
			
			newType.Type = ClassType.Enum;
			
			Identifier();

#line  512 "VBNET.ATG" 
			newType.Name = t.val; 
			if (la.kind == 60) {
				lexer.NextToken();
				NonArrayTypeName(
#line  513 "VBNET.ATG" 
out typeRef, false);

#line  513 "VBNET.ATG" 
				SafeAdd(newType, newType.BaseTypes, typeRef); 
			}
			EndOfStmt();

#line  515 "VBNET.ATG" 
			newType.BodyStartLocation = t.Location; 
			EnumBody(
#line  516 "VBNET.ATG" 
newType);

#line  518 "VBNET.ATG" 
			compilationUnit.BlockEnd();
			
			break;
		}
		case 139: {
			lexer.NextToken();

#line  523 "VBNET.ATG" 
			m.Check(Modifiers.VBInterfacs);
			TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
			newType.StartLocation = m.GetDeclarationLocation(t.Location);
			compilationUnit.AddChild(newType);
			compilationUnit.BlockStart(newType);
			newType.Type = ClassType.Interface;
			
			Identifier();

#line  530 "VBNET.ATG" 
			newType.Name = t.val; 
			TypeParameterList(
#line  531 "VBNET.ATG" 
newType.Templates);
			EndOfStmt();

#line  533 "VBNET.ATG" 
			newType.BodyStartLocation = t.Location; 
			while (la.kind == 137) {
				InterfaceBase(
#line  534 "VBNET.ATG" 
out baseInterfaces);

#line  534 "VBNET.ATG" 
				newType.BaseTypes.AddRange(baseInterfaces); 
			}
			InterfaceBody(
#line  535 "VBNET.ATG" 
newType);

#line  537 "VBNET.ATG" 
			compilationUnit.BlockEnd();
			
			break;
		}
		case 100: {
			lexer.NextToken();

#line  542 "VBNET.ATG" 
			m.Check(Modifiers.VBDelegates);
			DelegateDeclaration delegateDeclr = new DelegateDeclaration(m.Modifier, attributes);
			delegateDeclr.ReturnType = new TypeReference("System.Void", true);
			delegateDeclr.StartLocation = m.GetDeclarationLocation(t.Location);
			List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
			
			if (la.kind == 206) {
				lexer.NextToken();
				Identifier();

#line  549 "VBNET.ATG" 
				delegateDeclr.Name = t.val; 
				TypeParameterList(
#line  550 "VBNET.ATG" 
delegateDeclr.Templates);
				if (la.kind == 34) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
#line  551 "VBNET.ATG" 
p);
					}
					Expect(35);

#line  551 "VBNET.ATG" 
					delegateDeclr.Parameters = p; 
				}
			} else if (la.kind == 124) {
				lexer.NextToken();
				Identifier();

#line  553 "VBNET.ATG" 
				delegateDeclr.Name = t.val; 
				TypeParameterList(
#line  554 "VBNET.ATG" 
delegateDeclr.Templates);
				if (la.kind == 34) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
#line  555 "VBNET.ATG" 
p);
					}
					Expect(35);

#line  555 "VBNET.ATG" 
					delegateDeclr.Parameters = p; 
				}
				if (la.kind == 60) {
					lexer.NextToken();

#line  556 "VBNET.ATG" 
					TypeReference type; 
					TypeName(
#line  556 "VBNET.ATG" 
out type);

#line  556 "VBNET.ATG" 
					delegateDeclr.ReturnType = type; 
				}
			} else SynErr(244);

#line  558 "VBNET.ATG" 
			delegateDeclr.EndLocation = t.EndLocation; 
			EndOfStmt();

#line  561 "VBNET.ATG" 
			compilationUnit.AddChild(delegateDeclr);
			
			break;
		}
		default: SynErr(245); break;
		}
	}

	void TypeParameterList(
#line  384 "VBNET.ATG" 
List<TemplateDefinition> templates) {

#line  386 "VBNET.ATG" 
		TemplateDefinition template;
		
		if (
#line  390 "VBNET.ATG" 
la.kind == Tokens.OpenParenthesis && Peek(1).kind == Tokens.Of) {
			lexer.NextToken();
			Expect(166);
			TypeParameter(
#line  391 "VBNET.ATG" 
out template);

#line  393 "VBNET.ATG" 
			if (template != null) templates.Add(template);
			
			while (la.kind == 21) {
				lexer.NextToken();
				TypeParameter(
#line  396 "VBNET.ATG" 
out template);

#line  398 "VBNET.ATG" 
				if (template != null) templates.Add(template);
				
			}
			Expect(35);
		}
	}

	void TypeParameter(
#line  406 "VBNET.ATG" 
out TemplateDefinition template) {
		Identifier();

#line  408 "VBNET.ATG" 
		template = new TemplateDefinition(t.val, null); 
		if (la.kind == 60) {
			TypeParameterConstraints(
#line  409 "VBNET.ATG" 
template);
		}
	}

	void TypeParameterConstraints(
#line  413 "VBNET.ATG" 
TemplateDefinition template) {

#line  415 "VBNET.ATG" 
		TypeReference constraint;
		
		Expect(60);
		if (la.kind == 32) {
			lexer.NextToken();
			TypeParameterConstraint(
#line  421 "VBNET.ATG" 
out constraint);

#line  421 "VBNET.ATG" 
			if (constraint != null) { template.Bases.Add(constraint); } 
			while (la.kind == 21) {
				lexer.NextToken();
				TypeParameterConstraint(
#line  424 "VBNET.ATG" 
out constraint);

#line  424 "VBNET.ATG" 
				if (constraint != null) { template.Bases.Add(constraint); } 
			}
			Expect(33);
		} else if (StartOf(7)) {
			TypeParameterConstraint(
#line  427 "VBNET.ATG" 
out constraint);

#line  427 "VBNET.ATG" 
			if (constraint != null) { template.Bases.Add(constraint); } 
		} else SynErr(246);
	}

	void TypeParameterConstraint(
#line  431 "VBNET.ATG" 
out TypeReference constraint) {

#line  432 "VBNET.ATG" 
		constraint = null; 
		if (la.kind == 81) {
			lexer.NextToken();

#line  433 "VBNET.ATG" 
			constraint = TypeReference.ClassConstraint; 
		} else if (la.kind == 205) {
			lexer.NextToken();

#line  434 "VBNET.ATG" 
			constraint = TypeReference.StructConstraint; 
		} else if (la.kind == 159) {
			lexer.NextToken();

#line  435 "VBNET.ATG" 
			constraint = TypeReference.NewConstraint; 
		} else if (StartOf(8)) {
			TypeName(
#line  436 "VBNET.ATG" 
out constraint);
		} else SynErr(247);
	}

	void ClassBaseType(
#line  781 "VBNET.ATG" 
out TypeReference typeRef) {

#line  783 "VBNET.ATG" 
		typeRef = null;
		
		Expect(137);
		TypeName(
#line  786 "VBNET.ATG" 
out typeRef);
		EndOfStmt();
	}

	void TypeImplementsClause(
#line  1598 "VBNET.ATG" 
out List<TypeReference> baseInterfaces) {

#line  1600 "VBNET.ATG" 
		baseInterfaces = new List<TypeReference>();
		TypeReference type = null;
		
		Expect(133);
		TypeName(
#line  1603 "VBNET.ATG" 
out type);

#line  1605 "VBNET.ATG" 
		if (type != null) baseInterfaces.Add(type);
		
		while (la.kind == 21) {
			lexer.NextToken();
			TypeName(
#line  1608 "VBNET.ATG" 
out type);

#line  1609 "VBNET.ATG" 
			if (type != null) baseInterfaces.Add(type); 
		}
		EndOfStmt();
	}

	void ClassBody(
#line  575 "VBNET.ATG" 
TypeDeclaration newType) {

#line  576 "VBNET.ATG" 
		AttributeSection section; 
		while (la.kind == 1 || la.kind == 20) {
			EndOfStmt();
		}
		while (StartOf(9)) {

#line  579 "VBNET.ATG" 
			List<AttributeSection> attributes = new List<AttributeSection>();
			ModifierList m = new ModifierList();
			
			while (la.kind == 37) {
				AttributeSection(
#line  582 "VBNET.ATG" 
out section);

#line  582 "VBNET.ATG" 
				attributes.Add(section); 
			}
			while (StartOf(10)) {
				MemberModifier(
#line  583 "VBNET.ATG" 
m);
			}
			ClassMemberDecl(
#line  584 "VBNET.ATG" 
m, attributes);
			while (la.kind == 1 || la.kind == 20) {
				EndOfStmt();
			}
		}
	}

	void ModuleBody(
#line  606 "VBNET.ATG" 
TypeDeclaration newType) {

#line  607 "VBNET.ATG" 
		AttributeSection section; 
		while (la.kind == 1 || la.kind == 20) {
			EndOfStmt();
		}
		while (StartOf(9)) {

#line  610 "VBNET.ATG" 
			List<AttributeSection> attributes = new List<AttributeSection>();
			ModifierList m = new ModifierList();
			
			while (la.kind == 37) {
				AttributeSection(
#line  613 "VBNET.ATG" 
out section);

#line  613 "VBNET.ATG" 
				attributes.Add(section); 
			}
			while (StartOf(10)) {
				MemberModifier(
#line  614 "VBNET.ATG" 
m);
			}
			ClassMemberDecl(
#line  615 "VBNET.ATG" 
m, attributes);
			while (la.kind == 1 || la.kind == 20) {
				EndOfStmt();
			}
		}
		Expect(110);
		Expect(152);

#line  618 "VBNET.ATG" 
		newType.EndLocation = t.EndLocation; 
		EndOfStmt();
	}

	void StructureBody(
#line  589 "VBNET.ATG" 
TypeDeclaration newType) {

#line  590 "VBNET.ATG" 
		AttributeSection section; 
		while (la.kind == 1 || la.kind == 20) {
			EndOfStmt();
		}
		while (StartOf(9)) {

#line  593 "VBNET.ATG" 
			List<AttributeSection> attributes = new List<AttributeSection>();
			ModifierList m = new ModifierList();
			
			while (la.kind == 37) {
				AttributeSection(
#line  596 "VBNET.ATG" 
out section);

#line  596 "VBNET.ATG" 
				attributes.Add(section); 
			}
			while (StartOf(10)) {
				MemberModifier(
#line  597 "VBNET.ATG" 
m);
			}
			StructureMemberDecl(
#line  598 "VBNET.ATG" 
m, attributes);
			while (la.kind == 1 || la.kind == 20) {
				EndOfStmt();
			}
		}
		Expect(110);
		Expect(205);

#line  601 "VBNET.ATG" 
		newType.EndLocation = t.EndLocation; 
		EndOfStmt();
	}

	void NonArrayTypeName(
#line  2484 "VBNET.ATG" 
out TypeReference typeref, bool canBeUnbound) {

#line  2486 "VBNET.ATG" 
		string name;
		typeref = null;
		bool isGlobal = false;
		
		if (StartOf(11)) {
			if (la.kind == 127) {
				lexer.NextToken();
				Expect(25);

#line  2491 "VBNET.ATG" 
				isGlobal = true; 
			}
			QualIdentAndTypeArguments(
#line  2492 "VBNET.ATG" 
out typeref, canBeUnbound);

#line  2493 "VBNET.ATG" 
			typeref.IsGlobal = isGlobal; 
			while (la.kind == 25) {
				lexer.NextToken();

#line  2494 "VBNET.ATG" 
				TypeReference nestedTypeRef; 
				QualIdentAndTypeArguments(
#line  2495 "VBNET.ATG" 
out nestedTypeRef, canBeUnbound);

#line  2496 "VBNET.ATG" 
				typeref = new InnerClassTypeReference(typeref, nestedTypeRef.Type, nestedTypeRef.GenericTypes); 
			}
		} else if (la.kind == 165) {
			lexer.NextToken();

#line  2499 "VBNET.ATG" 
			typeref = new TypeReference("System.Object", true); 
			if (la.kind == 30) {
				lexer.NextToken();

#line  2503 "VBNET.ATG" 
				List<TypeReference> typeArguments = new List<TypeReference>(1);
				if (typeref != null) typeArguments.Add(typeref);
				typeref = new TypeReference("System.Nullable", typeArguments) { IsKeyword = true };
				
			}
		} else if (StartOf(12)) {
			PrimitiveTypeName(
#line  2509 "VBNET.ATG" 
out name);

#line  2509 "VBNET.ATG" 
			typeref = new TypeReference(name, true); 
			if (la.kind == 30) {
				lexer.NextToken();

#line  2513 "VBNET.ATG" 
				List<TypeReference> typeArguments = new List<TypeReference>(1);
				if (typeref != null) typeArguments.Add(typeref);
				typeref = new TypeReference("System.Nullable", typeArguments) { IsKeyword = true };
				
			}
		} else SynErr(248);
	}

	void EnumBody(
#line  622 "VBNET.ATG" 
TypeDeclaration newType) {

#line  623 "VBNET.ATG" 
		FieldDeclaration f; 
		while (la.kind == 1 || la.kind == 20) {
			EndOfStmt();
		}
		while (StartOf(13)) {
			EnumMemberDecl(
#line  626 "VBNET.ATG" 
out f);

#line  628 "VBNET.ATG" 
			compilationUnit.AddChild(f);
			
			while (la.kind == 1 || la.kind == 20) {
				EndOfStmt();
			}
		}
		Expect(110);
		Expect(112);

#line  632 "VBNET.ATG" 
		newType.EndLocation = t.EndLocation; 
		EndOfStmt();
	}

	void InterfaceBase(
#line  1583 "VBNET.ATG" 
out List<TypeReference> bases) {

#line  1585 "VBNET.ATG" 
		TypeReference type;
		bases = new List<TypeReference>();
		
		Expect(137);
		TypeName(
#line  1589 "VBNET.ATG" 
out type);

#line  1589 "VBNET.ATG" 
		if (type != null) bases.Add(type); 
		while (la.kind == 21) {
			lexer.NextToken();
			TypeName(
#line  1592 "VBNET.ATG" 
out type);

#line  1592 "VBNET.ATG" 
			if (type != null) bases.Add(type); 
		}
		EndOfStmt();
	}

	void InterfaceBody(
#line  636 "VBNET.ATG" 
TypeDeclaration newType) {
		while (la.kind == 1 || la.kind == 20) {
			EndOfStmt();
		}
		while (StartOf(14)) {
			InterfaceMemberDecl();
			while (la.kind == 1 || la.kind == 20) {
				EndOfStmt();
			}
		}
		Expect(110);
		Expect(139);

#line  642 "VBNET.ATG" 
		newType.EndLocation = t.EndLocation; 
		EndOfStmt();
	}

	void FormalParameterList(
#line  2697 "VBNET.ATG" 
List<ParameterDeclarationExpression> parameter) {

#line  2698 "VBNET.ATG" 
		ParameterDeclarationExpression p; 
		FormalParameter(
#line  2700 "VBNET.ATG" 
out p);

#line  2700 "VBNET.ATG" 
		if (p != null) parameter.Add(p); 
		while (la.kind == 21) {
			lexer.NextToken();
			FormalParameter(
#line  2702 "VBNET.ATG" 
out p);

#line  2702 "VBNET.ATG" 
			if (p != null) parameter.Add(p); 
		}
	}

	void MemberModifier(
#line  3438 "VBNET.ATG" 
ModifierList m) {
		switch (la.kind) {
		case 153: {
			lexer.NextToken();

#line  3439 "VBNET.ATG" 
			m.Add(Modifiers.Abstract, t.Location);
			break;
		}
		case 99: {
			lexer.NextToken();

#line  3440 "VBNET.ATG" 
			m.Add(Modifiers.Default, t.Location);
			break;
		}
		case 122: {
			lexer.NextToken();

#line  3441 "VBNET.ATG" 
			m.Add(Modifiers.Internal, t.Location);
			break;
		}
		case 195: {
			lexer.NextToken();

#line  3442 "VBNET.ATG" 
			m.Add(Modifiers.New, t.Location);
			break;
		}
		case 177: {
			lexer.NextToken();

#line  3443 "VBNET.ATG" 
			m.Add(Modifiers.Override, t.Location);
			break;
		}
		case 154: {
			lexer.NextToken();

#line  3444 "VBNET.ATG" 
			m.Add(Modifiers.Abstract, t.Location);
			break;
		}
		case 181: {
			lexer.NextToken();

#line  3445 "VBNET.ATG" 
			m.Add(Modifiers.Private, t.Location);
			break;
		}
		case 183: {
			lexer.NextToken();

#line  3446 "VBNET.ATG" 
			m.Add(Modifiers.Protected, t.Location);
			break;
		}
		case 184: {
			lexer.NextToken();

#line  3447 "VBNET.ATG" 
			m.Add(Modifiers.Public, t.Location);
			break;
		}
		case 163: {
			lexer.NextToken();

#line  3448 "VBNET.ATG" 
			m.Add(Modifiers.Sealed, t.Location);
			break;
		}
		case 164: {
			lexer.NextToken();

#line  3449 "VBNET.ATG" 
			m.Add(Modifiers.Sealed, t.Location);
			break;
		}
		case 196: {
			lexer.NextToken();

#line  3450 "VBNET.ATG" 
			m.Add(Modifiers.Static, t.Location);
			break;
		}
		case 176: {
			lexer.NextToken();

#line  3451 "VBNET.ATG" 
			m.Add(Modifiers.Virtual, t.Location);
			break;
		}
		case 175: {
			lexer.NextToken();

#line  3452 "VBNET.ATG" 
			m.Add(Modifiers.Overloads, t.Location);
			break;
		}
		case 186: {
			lexer.NextToken();

#line  3453 "VBNET.ATG" 
			m.Add(Modifiers.ReadOnly, t.Location);
			break;
		}
		case 231: {
			lexer.NextToken();

#line  3454 "VBNET.ATG" 
			m.Add(Modifiers.WriteOnly, t.Location);
			break;
		}
		case 230: {
			lexer.NextToken();

#line  3455 "VBNET.ATG" 
			m.Add(Modifiers.WithEvents, t.Location);
			break;
		}
		case 102: {
			lexer.NextToken();

#line  3456 "VBNET.ATG" 
			m.Add(Modifiers.Dim, t.Location);
			break;
		}
		case 179: {
			lexer.NextToken();

#line  3457 "VBNET.ATG" 
			m.Add(Modifiers.Partial, t.Location);
			break;
		}
		default: SynErr(249); break;
		}
	}

	void ClassMemberDecl(
#line  777 "VBNET.ATG" 
ModifierList m, List<AttributeSection> attributes) {
		StructureMemberDecl(
#line  778 "VBNET.ATG" 
m, attributes);
	}

	void StructureMemberDecl(
#line  791 "VBNET.ATG" 
ModifierList m, List<AttributeSection> attributes) {

#line  793 "VBNET.ATG" 
		TypeReference type = null;
		List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
		Statement stmt = null;
		List<VariableDeclaration> variableDeclarators = new List<VariableDeclaration>();
		List<TemplateDefinition> templates = new List<TemplateDefinition>();
		
		switch (la.kind) {
		case 81: case 100: case 112: case 139: case 152: case 205: {
			NonModuleDeclaration(
#line  800 "VBNET.ATG" 
m, attributes);
			break;
		}
		case 206: {
			lexer.NextToken();

#line  804 "VBNET.ATG" 
			Location startPos = t.Location;
			
			if (StartOf(4)) {

#line  808 "VBNET.ATG" 
				string name = String.Empty;
				MethodDeclaration methodDeclaration; List<string> handlesClause = null;
				List<InterfaceImplementation> implementsClause = null;
				
				Identifier();

#line  814 "VBNET.ATG" 
				name = t.val;
				m.Check(Modifiers.VBMethods);
				
				TypeParameterList(
#line  817 "VBNET.ATG" 
templates);
				if (la.kind == 34) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
#line  818 "VBNET.ATG" 
p);
					}
					Expect(35);
				}
				if (la.kind == 131 || la.kind == 133) {
					if (la.kind == 133) {
						ImplementsClause(
#line  821 "VBNET.ATG" 
out implementsClause);
					} else {
						HandlesClause(
#line  823 "VBNET.ATG" 
out handlesClause);
					}
				}

#line  826 "VBNET.ATG" 
				Location endLocation = t.EndLocation; 
				if (
#line  829 "VBNET.ATG" 
IsMustOverride(m)) {
					EndOfStmt();

#line  832 "VBNET.ATG" 
					methodDeclaration = new MethodDeclaration {
					Name = name, Modifier = m.Modifier, Parameters = p, Attributes = attributes,
					StartLocation = m.GetDeclarationLocation(startPos), EndLocation = endLocation,
					TypeReference = new TypeReference("System.Void", true),
					Templates = templates,
					HandlesClause = handlesClause,
					InterfaceImplementations = implementsClause
					};
					compilationUnit.AddChild(methodDeclaration);
					
				} else if (la.kind == 1) {
					lexer.NextToken();

#line  845 "VBNET.ATG" 
					methodDeclaration = new MethodDeclaration {
					Name = name, Modifier = m.Modifier, Parameters = p, Attributes = attributes,
					StartLocation = m.GetDeclarationLocation(startPos), EndLocation = endLocation,
					TypeReference = new TypeReference("System.Void", true),
					Templates = templates,
					HandlesClause = handlesClause,
					InterfaceImplementations = implementsClause
					};
					compilationUnit.AddChild(methodDeclaration);
					

#line  856 "VBNET.ATG" 
					if (ParseMethodBodies) { 
					Block(
#line  857 "VBNET.ATG" 
out stmt);
					Expect(110);
					Expect(206);

#line  859 "VBNET.ATG" 
					} else {
					// don't parse method body
					lexer.SkipCurrentBlock(Tokens.Sub); stmt = new BlockStatement();
					  }
					

#line  865 "VBNET.ATG" 
					methodDeclaration.Body  = (BlockStatement)stmt; 

#line  866 "VBNET.ATG" 
					methodDeclaration.Body.EndLocation = t.EndLocation; 
					EndOfStmt();
				} else SynErr(250);
			} else if (la.kind == 159) {
				lexer.NextToken();
				if (la.kind == 34) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
#line  870 "VBNET.ATG" 
p);
					}
					Expect(35);
				}

#line  871 "VBNET.ATG" 
				m.Check(Modifiers.Constructors); 

#line  872 "VBNET.ATG" 
				Location constructorEndLocation = t.EndLocation; 
				Expect(1);

#line  875 "VBNET.ATG" 
				if (ParseMethodBodies) { 
				Block(
#line  876 "VBNET.ATG" 
out stmt);
				Expect(110);
				Expect(206);

#line  878 "VBNET.ATG" 
				} else {
				// don't parse method body
				lexer.SkipCurrentBlock(Tokens.Sub); stmt = new BlockStatement();
				  }
				

#line  884 "VBNET.ATG" 
				Location endLocation = t.EndLocation; 
				EndOfStmt();

#line  887 "VBNET.ATG" 
				ConstructorDeclaration cd = new ConstructorDeclaration("New", m.Modifier, p, attributes);
				cd.StartLocation = m.GetDeclarationLocation(startPos);
				cd.EndLocation   = constructorEndLocation;
				cd.Body = (BlockStatement)stmt;
				cd.Body.EndLocation   = endLocation;
				compilationUnit.AddChild(cd);
				
			} else SynErr(251);
			break;
		}
		case 124: {
			lexer.NextToken();

#line  899 "VBNET.ATG" 
			m.Check(Modifiers.VBMethods);
			string name = String.Empty;
			Location startPos = t.Location;
			MethodDeclaration methodDeclaration;List<string> handlesClause = null;
			List<InterfaceImplementation> implementsClause = null;
			AttributeSection returnTypeAttributeSection = null;
			
			Identifier();

#line  906 "VBNET.ATG" 
			name = t.val; 
			TypeParameterList(
#line  907 "VBNET.ATG" 
templates);
			if (la.kind == 34) {
				lexer.NextToken();
				if (StartOf(6)) {
					FormalParameterList(
#line  908 "VBNET.ATG" 
p);
				}
				Expect(35);
			}
			if (la.kind == 60) {
				lexer.NextToken();
				while (la.kind == 37) {
					AttributeSection(
#line  910 "VBNET.ATG" 
out returnTypeAttributeSection);

#line  912 "VBNET.ATG" 
					if (returnTypeAttributeSection != null) {
					returnTypeAttributeSection.AttributeTarget = "return";
					attributes.Add(returnTypeAttributeSection);
					}
					
				}
				TypeName(
#line  918 "VBNET.ATG" 
out type);
			}

#line  920 "VBNET.ATG" 
			if(type == null) {
			type = new TypeReference("System.Object", true);
			}
			
			if (la.kind == 131 || la.kind == 133) {
				if (la.kind == 133) {
					ImplementsClause(
#line  926 "VBNET.ATG" 
out implementsClause);
				} else {
					HandlesClause(
#line  928 "VBNET.ATG" 
out handlesClause);
				}
			}

#line  931 "VBNET.ATG" 
			Location endLocation = t.EndLocation; 
			if (
#line  934 "VBNET.ATG" 
IsMustOverride(m)) {
				EndOfStmt();

#line  937 "VBNET.ATG" 
				methodDeclaration = new MethodDeclaration {
				Name = name, Modifier = m.Modifier, TypeReference = type,
				Parameters = p, Attributes = attributes,
				StartLocation = m.GetDeclarationLocation(startPos),
				EndLocation   = endLocation,
				HandlesClause = handlesClause,
				Templates     = templates,
				InterfaceImplementations = implementsClause
				};
				
				compilationUnit.AddChild(methodDeclaration);
				
			} else if (la.kind == 1) {
				lexer.NextToken();

#line  952 "VBNET.ATG" 
				methodDeclaration = new MethodDeclaration {
				Name = name, Modifier = m.Modifier, TypeReference = type,
				Parameters = p, Attributes = attributes,
				StartLocation = m.GetDeclarationLocation(startPos),
				EndLocation   = endLocation,
				Templates     = templates,
				HandlesClause = handlesClause,
				InterfaceImplementations = implementsClause
				};
				
				compilationUnit.AddChild(methodDeclaration);
				
				if (ParseMethodBodies) { 
				Block(
#line  965 "VBNET.ATG" 
out stmt);
				Expect(110);
				Expect(124);

#line  967 "VBNET.ATG" 
				} else {
				// don't parse method body
				lexer.SkipCurrentBlock(Tokens.Function); stmt = new BlockStatement();
				}
				methodDeclaration.Body = (BlockStatement)stmt;
				methodDeclaration.Body.StartLocation = methodDeclaration.EndLocation;
				methodDeclaration.Body.EndLocation   = t.EndLocation;
				
				EndOfStmt();
			} else SynErr(252);
			break;
		}
		case 98: {
			lexer.NextToken();

#line  981 "VBNET.ATG" 
			m.Check(Modifiers.VBExternalMethods);
			Location startPos = t.Location;
			CharsetModifier charsetModifer = CharsetModifier.None;
			string library = String.Empty;
			string alias = null;
			string name = String.Empty;
			
			if (StartOf(15)) {
				Charset(
#line  988 "VBNET.ATG" 
out charsetModifer);
			}
			if (la.kind == 206) {
				lexer.NextToken();
				Identifier();

#line  991 "VBNET.ATG" 
				name = t.val; 
				Expect(146);
				Expect(3);

#line  992 "VBNET.ATG" 
				library = t.literalValue as string; 
				if (la.kind == 56) {
					lexer.NextToken();
					Expect(3);

#line  993 "VBNET.ATG" 
					alias = t.literalValue as string; 
				}
				if (la.kind == 34) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
#line  994 "VBNET.ATG" 
p);
					}
					Expect(35);
				}
				EndOfStmt();

#line  997 "VBNET.ATG" 
				DeclareDeclaration declareDeclaration = new DeclareDeclaration(name, m.Modifier, null, p, attributes, library, alias, charsetModifer);
				declareDeclaration.StartLocation = m.GetDeclarationLocation(startPos);
				declareDeclaration.EndLocation   = t.EndLocation;
				compilationUnit.AddChild(declareDeclaration);
				
			} else if (la.kind == 124) {
				lexer.NextToken();
				Identifier();

#line  1004 "VBNET.ATG" 
				name = t.val; 
				Expect(146);
				Expect(3);

#line  1005 "VBNET.ATG" 
				library = t.literalValue as string; 
				if (la.kind == 56) {
					lexer.NextToken();
					Expect(3);

#line  1006 "VBNET.ATG" 
					alias = t.literalValue as string; 
				}
				if (la.kind == 34) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
#line  1007 "VBNET.ATG" 
p);
					}
					Expect(35);
				}
				if (la.kind == 60) {
					lexer.NextToken();
					TypeName(
#line  1008 "VBNET.ATG" 
out type);
				}
				EndOfStmt();

#line  1011 "VBNET.ATG" 
				DeclareDeclaration declareDeclaration = new DeclareDeclaration(name, m.Modifier, type, p, attributes, library, alias, charsetModifer);
				declareDeclaration.StartLocation = m.GetDeclarationLocation(startPos);
				declareDeclaration.EndLocation   = t.EndLocation;
				compilationUnit.AddChild(declareDeclaration);
				
			} else SynErr(253);
			break;
		}
		case 116: {
			lexer.NextToken();

#line  1021 "VBNET.ATG" 
			m.Check(Modifiers.VBEvents);
			Location startPos = t.Location;
			EventDeclaration eventDeclaration;
			string name = String.Empty;
			List<InterfaceImplementation> implementsClause = null;
			
			Identifier();

#line  1027 "VBNET.ATG" 
			name= t.val; 
			if (la.kind == 60) {
				lexer.NextToken();
				TypeName(
#line  1029 "VBNET.ATG" 
out type);
			} else if (StartOf(16)) {
				if (la.kind == 34) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
#line  1031 "VBNET.ATG" 
p);
					}
					Expect(35);
				}
			} else SynErr(254);
			if (la.kind == 133) {
				ImplementsClause(
#line  1033 "VBNET.ATG" 
out implementsClause);
			}

#line  1035 "VBNET.ATG" 
			eventDeclaration = new EventDeclaration {
			Name = name, TypeReference = type, Modifier = m.Modifier, 
			Parameters = p, Attributes = attributes, InterfaceImplementations = implementsClause,
			StartLocation = m.GetDeclarationLocation(startPos),
			EndLocation = t.EndLocation
			};
			compilationUnit.AddChild(eventDeclaration);
			
			EndOfStmt();
			break;
		}
		case 2: case 55: case 59: case 61: case 62: case 63: case 64: case 67: case 84: case 101: case 104: case 113: case 118: case 123: case 130: case 136: case 140: case 143: case 167: case 173: case 180: case 199: case 208: case 209: case 219: case 220: case 226: {

#line  1046 "VBNET.ATG" 
			m.Check(Modifiers.Fields);
			FieldDeclaration fd = new FieldDeclaration(attributes, null, m.Modifier);
			
			IdentifierForFieldDeclaration();

#line  1049 "VBNET.ATG" 
			string name = t.val; 

#line  1050 "VBNET.ATG" 
			fd.StartLocation = m.GetDeclarationLocation(t.Location); 
			VariableDeclaratorPartAfterIdentifier(
#line  1052 "VBNET.ATG" 
variableDeclarators, name);
			while (la.kind == 21) {
				lexer.NextToken();
				VariableDeclarator(
#line  1053 "VBNET.ATG" 
variableDeclarators);
			}
			EndOfStmt();

#line  1056 "VBNET.ATG" 
			fd.EndLocation = t.EndLocation;
			fd.Fields = variableDeclarators;
			compilationUnit.AddChild(fd);
			
			break;
		}
		case 85: {

#line  1061 "VBNET.ATG" 
			m.Check(Modifiers.Fields); 
			lexer.NextToken();

#line  1062 "VBNET.ATG" 
			m.Add(Modifiers.Const, t.Location);  

#line  1064 "VBNET.ATG" 
			FieldDeclaration fd = new FieldDeclaration(attributes, type, m.Modifier);
			fd.StartLocation = m.GetDeclarationLocation(t.Location);
			List<VariableDeclaration> constantDeclarators = new List<VariableDeclaration>();
			
			ConstantDeclarator(
#line  1068 "VBNET.ATG" 
constantDeclarators);
			while (la.kind == 21) {
				lexer.NextToken();
				ConstantDeclarator(
#line  1069 "VBNET.ATG" 
constantDeclarators);
			}

#line  1071 "VBNET.ATG" 
			fd.Fields = constantDeclarators;
			fd.EndLocation = t.Location;
			
			EndOfStmt();

#line  1076 "VBNET.ATG" 
			fd.EndLocation = t.EndLocation;
			compilationUnit.AddChild(fd);
			
			break;
		}
		case 182: {
			lexer.NextToken();

#line  1082 "VBNET.ATG" 
			m.Check(Modifiers.VBProperties);
			Location startPos = t.Location;
			List<InterfaceImplementation> implementsClause = null;
			AttributeSection returnTypeAttributeSection = null;
			Expression initializer = null;
			
			Identifier();

#line  1088 "VBNET.ATG" 
			string propertyName = t.val; 
			if (la.kind == 34) {
				lexer.NextToken();
				if (StartOf(6)) {
					FormalParameterList(
#line  1089 "VBNET.ATG" 
p);
				}
				Expect(35);
			}
			if (la.kind == 60) {
				lexer.NextToken();
				while (la.kind == 37) {
					AttributeSection(
#line  1092 "VBNET.ATG" 
out returnTypeAttributeSection);

#line  1094 "VBNET.ATG" 
					if (returnTypeAttributeSection != null) {
					returnTypeAttributeSection.AttributeTarget = "return";
					attributes.Add(returnTypeAttributeSection);
					}
					
				}
				if (
#line  1101 "VBNET.ATG" 
IsNewExpression()) {
					ObjectCreateExpression(
#line  1101 "VBNET.ATG" 
out initializer);

#line  1103 "VBNET.ATG" 
					if (initializer is ObjectCreateExpression) {
					type = ((ObjectCreateExpression)initializer).CreateType.Clone();
					} else {
						type = ((ArrayCreateExpression)initializer).CreateType.Clone();
					}
					
				} else if (StartOf(8)) {
					TypeName(
#line  1110 "VBNET.ATG" 
out type);
				} else SynErr(255);
			}
			if (la.kind == 19) {
				lexer.NextToken();
				Expr(
#line  1113 "VBNET.ATG" 
out initializer);
			}
			if (la.kind == 133) {
				ImplementsClause(
#line  1114 "VBNET.ATG" 
out implementsClause);
			}
			EndOfStmt();
			if (
#line  1118 "VBNET.ATG" 
IsMustOverride(m) || IsAutomaticProperty()) {

#line  1120 "VBNET.ATG" 
				PropertyDeclaration pDecl = new PropertyDeclaration(propertyName, type, m.Modifier, attributes);
				pDecl.StartLocation = m.GetDeclarationLocation(startPos);
				pDecl.EndLocation   = t.Location;
				pDecl.TypeReference = type;
				pDecl.InterfaceImplementations = implementsClause;
				pDecl.Parameters = p;
				if (initializer != null)
					pDecl.Initializer = initializer;
				compilationUnit.AddChild(pDecl);
				
			} else if (StartOf(17)) {

#line  1132 "VBNET.ATG" 
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
#line  1142 "VBNET.ATG" 
out getRegion, out setRegion);
				Expect(110);
				Expect(182);
				EndOfStmt();

#line  1146 "VBNET.ATG" 
				pDecl.GetRegion = getRegion;
				pDecl.SetRegion = setRegion;
				pDecl.BodyEnd = t.Location; // t = EndOfStmt; not "Property"
				compilationUnit.AddChild(pDecl);
				
			} else SynErr(256);
			break;
		}
		case 95: {
			lexer.NextToken();

#line  1153 "VBNET.ATG" 
			Location startPos = t.Location; 
			Expect(116);

#line  1155 "VBNET.ATG" 
			m.Check(Modifiers.VBCustomEvents);
			EventAddRemoveRegion eventAccessorDeclaration;
			EventAddRegion addHandlerAccessorDeclaration = null;
			EventRemoveRegion removeHandlerAccessorDeclaration = null;
			EventRaiseRegion raiseEventAccessorDeclaration = null;
			List<InterfaceImplementation> implementsClause = null;
			
			Identifier();

#line  1162 "VBNET.ATG" 
			string customEventName = t.val; 
			Expect(60);
			TypeName(
#line  1163 "VBNET.ATG" 
out type);
			if (la.kind == 133) {
				ImplementsClause(
#line  1164 "VBNET.ATG" 
out implementsClause);
			}
			EndOfStmt();
			while (StartOf(18)) {
				EventAccessorDeclaration(
#line  1167 "VBNET.ATG" 
out eventAccessorDeclaration);

#line  1169 "VBNET.ATG" 
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
			Expect(110);
			Expect(116);
			EndOfStmt();

#line  1185 "VBNET.ATG" 
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
			
			EventDeclaration decl = new EventDeclaration {
				TypeReference = type, Name = customEventName, Modifier = m.Modifier,
				Attributes = attributes,
				StartLocation = m.GetDeclarationLocation(startPos),
				EndLocation = t.EndLocation,
				AddRegion = addHandlerAccessorDeclaration,
				RemoveRegion = removeHandlerAccessorDeclaration,
				RaiseRegion = raiseEventAccessorDeclaration
			};
			compilationUnit.AddChild(decl);
			
			break;
		}
		case 158: case 169: case 228: {

#line  1211 "VBNET.ATG" 
			ConversionType opConversionType = ConversionType.None; 
			if (la.kind == 158 || la.kind == 228) {
				if (la.kind == 228) {
					lexer.NextToken();

#line  1212 "VBNET.ATG" 
					opConversionType = ConversionType.Implicit; 
				} else {
					lexer.NextToken();

#line  1213 "VBNET.ATG" 
					opConversionType = ConversionType.Explicit;
				}
			}
			Expect(169);

#line  1216 "VBNET.ATG" 
			m.Check(Modifiers.VBOperators);
			Location startPos = t.Location;
			TypeReference returnType = NullTypeReference.Instance;
			TypeReference operandType = NullTypeReference.Instance;
			string operandName;
			OverloadableOperatorType operatorType;
			AttributeSection section;
			List<ParameterDeclarationExpression> parameters = new List<ParameterDeclarationExpression>();
			
			OverloadableOperator(
#line  1225 "VBNET.ATG" 
out operatorType);
			Expect(34);
			if (la.kind == 69) {
				lexer.NextToken();
			}
			Identifier();

#line  1226 "VBNET.ATG" 
			operandName = t.val; 
			if (la.kind == 60) {
				lexer.NextToken();
				TypeName(
#line  1227 "VBNET.ATG" 
out operandType);
			}

#line  1228 "VBNET.ATG" 
			parameters.Add(new ParameterDeclarationExpression(operandType, operandName, ParameterModifiers.In)); 
			while (la.kind == 21) {
				lexer.NextToken();
				if (la.kind == 69) {
					lexer.NextToken();
				}
				Identifier();

#line  1232 "VBNET.ATG" 
				operandName = t.val; 
				if (la.kind == 60) {
					lexer.NextToken();
					TypeName(
#line  1233 "VBNET.ATG" 
out operandType);
				}

#line  1234 "VBNET.ATG" 
				parameters.Add(new ParameterDeclarationExpression(operandType, operandName, ParameterModifiers.In)); 
			}
			Expect(35);

#line  1237 "VBNET.ATG" 
			Location endPos = t.EndLocation; 
			if (la.kind == 60) {
				lexer.NextToken();
				while (la.kind == 37) {
					AttributeSection(
#line  1238 "VBNET.ATG" 
out section);

#line  1239 "VBNET.ATG" 
					if (section != null) {
					section.AttributeTarget = "return";
					attributes.Add(section);
					} 
				}
				TypeName(
#line  1243 "VBNET.ATG" 
out returnType);

#line  1243 "VBNET.ATG" 
				endPos = t.EndLocation; 
			}
			Expect(1);
			Block(
#line  1245 "VBNET.ATG" 
out stmt);
			Expect(110);
			Expect(169);
			EndOfStmt();

#line  1247 "VBNET.ATG" 
			OperatorDeclaration operatorDeclaration = new OperatorDeclaration {
			Modifier = m.Modifier,
			Attributes = attributes,
			Parameters = parameters,
			TypeReference = returnType,
			OverloadableOperator = operatorType,
			ConversionType = opConversionType,
			Body = (BlockStatement)stmt,
			StartLocation = m.GetDeclarationLocation(startPos),
			EndLocation = endPos
			};
			operatorDeclaration.Body.StartLocation = startPos;
			operatorDeclaration.Body.EndLocation = t.Location;
			compilationUnit.AddChild(operatorDeclaration);
			
			break;
		}
		default: SynErr(257); break;
		}
	}

	void EnumMemberDecl(
#line  759 "VBNET.ATG" 
out FieldDeclaration f) {

#line  761 "VBNET.ATG" 
		Expression expr = null;List<AttributeSection> attributes = new List<AttributeSection>();
		AttributeSection section = null;
		VariableDeclaration varDecl = null;
		
		while (la.kind == 37) {
			AttributeSection(
#line  765 "VBNET.ATG" 
out section);

#line  765 "VBNET.ATG" 
			attributes.Add(section); 
		}
		Identifier();

#line  768 "VBNET.ATG" 
		f = new FieldDeclaration(attributes);
		varDecl = new VariableDeclaration(t.val);
		f.Fields.Add(varDecl);
		f.StartLocation = varDecl.StartLocation = t.Location;
		
		if (la.kind == 19) {
			lexer.NextToken();
			Expr(
#line  773 "VBNET.ATG" 
out expr);

#line  773 "VBNET.ATG" 
			varDecl.Initializer = expr; 
		}
		EndOfStmt();
	}

	void InterfaceMemberDecl() {

#line  650 "VBNET.ATG" 
		TypeReference type =null;
		List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
		List<TemplateDefinition> templates = new List<TemplateDefinition>();
		AttributeSection section, returnTypeAttributeSection = null;
		ModifierList mod = new ModifierList();
		List<AttributeSection> attributes = new List<AttributeSection>();
		string name;
		
		if (StartOf(19)) {
			while (la.kind == 37) {
				AttributeSection(
#line  658 "VBNET.ATG" 
out section);

#line  658 "VBNET.ATG" 
				attributes.Add(section); 
			}
			while (StartOf(10)) {
				MemberModifier(
#line  661 "VBNET.ATG" 
mod);
			}
			if (la.kind == 116) {
				lexer.NextToken();

#line  665 "VBNET.ATG" 
				mod.Check(Modifiers.VBInterfaceEvents);
				Location startLocation = t.Location;
				
				Identifier();

#line  668 "VBNET.ATG" 
				name = t.val; 
				if (la.kind == 34) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
#line  669 "VBNET.ATG" 
p);
					}
					Expect(35);
				}
				if (la.kind == 60) {
					lexer.NextToken();
					TypeName(
#line  670 "VBNET.ATG" 
out type);
				}
				EndOfStmt();

#line  673 "VBNET.ATG" 
				EventDeclaration ed = new EventDeclaration {
				Name = name, TypeReference = type, Modifier = mod.Modifier,
				Parameters = p, Attributes = attributes,
				StartLocation = startLocation, EndLocation = t.EndLocation
				};
				compilationUnit.AddChild(ed);
				
			} else if (la.kind == 206) {
				lexer.NextToken();

#line  683 "VBNET.ATG" 
				Location startLocation =  t.Location;
				mod.Check(Modifiers.VBInterfaceMethods);
				
				Identifier();

#line  686 "VBNET.ATG" 
				name = t.val; 
				TypeParameterList(
#line  687 "VBNET.ATG" 
templates);
				if (la.kind == 34) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
#line  688 "VBNET.ATG" 
p);
					}
					Expect(35);
				}
				EndOfStmt();

#line  691 "VBNET.ATG" 
				MethodDeclaration md = new MethodDeclaration {
				Name = name, 
				Modifier = mod.Modifier, 
				Parameters = p,
				Attributes = attributes,
				TypeReference = new TypeReference("System.Void", true),
				StartLocation = startLocation,
				EndLocation = t.EndLocation,
				Templates = templates
				};
				compilationUnit.AddChild(md);
				
			} else if (la.kind == 124) {
				lexer.NextToken();

#line  706 "VBNET.ATG" 
				mod.Check(Modifiers.VBInterfaceMethods);
				Location startLocation = t.Location;
				
				Identifier();

#line  709 "VBNET.ATG" 
				name = t.val; 
				TypeParameterList(
#line  710 "VBNET.ATG" 
templates);
				if (la.kind == 34) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
#line  711 "VBNET.ATG" 
p);
					}
					Expect(35);
				}
				if (la.kind == 60) {
					lexer.NextToken();
					while (la.kind == 37) {
						AttributeSection(
#line  712 "VBNET.ATG" 
out returnTypeAttributeSection);
					}
					TypeName(
#line  712 "VBNET.ATG" 
out type);
				}

#line  714 "VBNET.ATG" 
				if(type == null) {
				type = new TypeReference("System.Object", true);
				}
				MethodDeclaration md = new MethodDeclaration {
					Name = name, Modifier = mod.Modifier, 
					TypeReference = type, Parameters = p, Attributes = attributes
				};
				if (returnTypeAttributeSection != null) {
					returnTypeAttributeSection.AttributeTarget = "return";
					md.Attributes.Add(returnTypeAttributeSection);
				}
				md.StartLocation = startLocation;
				md.EndLocation = t.EndLocation;
				md.Templates = templates;
				compilationUnit.AddChild(md);
				
				EndOfStmt();
			} else if (la.kind == 182) {
				lexer.NextToken();

#line  734 "VBNET.ATG" 
				Location startLocation = t.Location;
				mod.Check(Modifiers.VBInterfaceProperties);
				
				Identifier();

#line  737 "VBNET.ATG" 
				name = t.val;  
				if (la.kind == 34) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
#line  738 "VBNET.ATG" 
p);
					}
					Expect(35);
				}
				if (la.kind == 60) {
					lexer.NextToken();
					TypeName(
#line  739 "VBNET.ATG" 
out type);
				}

#line  741 "VBNET.ATG" 
				if(type == null) {
				type = new TypeReference("System.Object", true);
				}
				
				EndOfStmt();

#line  747 "VBNET.ATG" 
				PropertyDeclaration pd = new PropertyDeclaration(name, type, mod.Modifier, attributes);
				pd.Parameters = p;
				pd.EndLocation = t.EndLocation;
				pd.StartLocation = startLocation;
				compilationUnit.AddChild(pd);
				
			} else SynErr(258);
		} else if (StartOf(20)) {
			NonModuleDeclaration(
#line  755 "VBNET.ATG" 
mod, attributes);
		} else SynErr(259);
	}

	void Expr(
#line  1642 "VBNET.ATG" 
out Expression expr) {

#line  1643 "VBNET.ATG" 
		expr = null; 
		if (
#line  1644 "VBNET.ATG" 
IsQueryExpression() ) {
			QueryExpr(
#line  1645 "VBNET.ATG" 
out expr);
		} else if (la.kind == 124) {
			LambdaExpr(
#line  1646 "VBNET.ATG" 
out expr);
		} else if (StartOf(21)) {
			DisjunctionExpr(
#line  1647 "VBNET.ATG" 
out expr);
		} else SynErr(260);
	}

	void ImplementsClause(
#line  1615 "VBNET.ATG" 
out List<InterfaceImplementation> baseInterfaces) {

#line  1617 "VBNET.ATG" 
		baseInterfaces = new List<InterfaceImplementation>();
		TypeReference type = null;
		string memberName = null;
		
		Expect(133);
		NonArrayTypeName(
#line  1622 "VBNET.ATG" 
out type, false);

#line  1623 "VBNET.ATG" 
		if (type != null) memberName = TypeReference.StripLastIdentifierFromType(ref type); 

#line  1624 "VBNET.ATG" 
		baseInterfaces.Add(new InterfaceImplementation(type, memberName)); 
		while (la.kind == 21) {
			lexer.NextToken();
			NonArrayTypeName(
#line  1626 "VBNET.ATG" 
out type, false);

#line  1627 "VBNET.ATG" 
			if (type != null) memberName = TypeReference.StripLastIdentifierFromType(ref type); 

#line  1628 "VBNET.ATG" 
			baseInterfaces.Add(new InterfaceImplementation(type, memberName)); 
		}
	}

	void HandlesClause(
#line  1573 "VBNET.ATG" 
out List<string> handlesClause) {

#line  1575 "VBNET.ATG" 
		handlesClause = new List<string>();
		string name;
		
		Expect(131);
		EventMemberSpecifier(
#line  1578 "VBNET.ATG" 
out name);

#line  1578 "VBNET.ATG" 
		if (name != null) handlesClause.Add(name); 
		while (la.kind == 21) {
			lexer.NextToken();
			EventMemberSpecifier(
#line  1579 "VBNET.ATG" 
out name);

#line  1579 "VBNET.ATG" 
			if (name != null) handlesClause.Add(name); 
		}
	}

	void Block(
#line  2744 "VBNET.ATG" 
out  Statement stmt) {

#line  2747 "VBNET.ATG" 
		BlockStatement blockStmt = new BlockStatement();
		/* in snippet parsing mode, t might be null */
		if (t != null) blockStmt.StartLocation = t.EndLocation;
		compilationUnit.BlockStart(blockStmt);
		
		while (StartOf(22) || 
#line  2753 "VBNET.ATG" 
IsEndStmtAhead()) {
			if (
#line  2753 "VBNET.ATG" 
IsEndStmtAhead()) {
				Expect(110);
				EndOfStmt();

#line  2753 "VBNET.ATG" 
				compilationUnit.AddChild(new EndStatement()); 
			} else {
				Statement();
				EndOfStmt();
			}
		}

#line  2758 "VBNET.ATG" 
		stmt = blockStmt;
		if (t != null) blockStmt.EndLocation = t.EndLocation;
		compilationUnit.BlockEnd();
		
	}

	void Charset(
#line  1565 "VBNET.ATG" 
out CharsetModifier charsetModifier) {

#line  1566 "VBNET.ATG" 
		charsetModifier = CharsetModifier.None; 
		if (la.kind == 124 || la.kind == 206) {
		} else if (la.kind == 59) {
			lexer.NextToken();

#line  1567 "VBNET.ATG" 
			charsetModifier = CharsetModifier.Ansi; 
		} else if (la.kind == 63) {
			lexer.NextToken();

#line  1568 "VBNET.ATG" 
			charsetModifier = CharsetModifier.Auto; 
		} else if (la.kind == 219) {
			lexer.NextToken();

#line  1569 "VBNET.ATG" 
			charsetModifier = CharsetModifier.Unicode; 
		} else SynErr(261);
	}

	void IdentifierForFieldDeclaration() {
		switch (la.kind) {
		case 2: {
			lexer.NextToken();
			break;
		}
		case 55: {
			lexer.NextToken();
			break;
		}
		case 59: {
			lexer.NextToken();
			break;
		}
		case 61: {
			lexer.NextToken();
			break;
		}
		case 62: {
			lexer.NextToken();
			break;
		}
		case 63: {
			lexer.NextToken();
			break;
		}
		case 64: {
			lexer.NextToken();
			break;
		}
		case 67: {
			lexer.NextToken();
			break;
		}
		case 84: {
			lexer.NextToken();
			break;
		}
		case 101: {
			lexer.NextToken();
			break;
		}
		case 104: {
			lexer.NextToken();
			break;
		}
		case 113: {
			lexer.NextToken();
			break;
		}
		case 118: {
			lexer.NextToken();
			break;
		}
		case 123: {
			lexer.NextToken();
			break;
		}
		case 130: {
			lexer.NextToken();
			break;
		}
		case 136: {
			lexer.NextToken();
			break;
		}
		case 140: {
			lexer.NextToken();
			break;
		}
		case 143: {
			lexer.NextToken();
			break;
		}
		case 167: {
			lexer.NextToken();
			break;
		}
		case 173: {
			lexer.NextToken();
			break;
		}
		case 180: {
			lexer.NextToken();
			break;
		}
		case 199: {
			lexer.NextToken();
			break;
		}
		case 208: {
			lexer.NextToken();
			break;
		}
		case 209: {
			lexer.NextToken();
			break;
		}
		case 219: {
			lexer.NextToken();
			break;
		}
		case 220: {
			lexer.NextToken();
			break;
		}
		case 226: {
			lexer.NextToken();
			break;
		}
		default: SynErr(262); break;
		}
	}

	void VariableDeclaratorPartAfterIdentifier(
#line  1450 "VBNET.ATG" 
List<VariableDeclaration> fieldDeclaration, string name) {

#line  1452 "VBNET.ATG" 
		Expression expr = null;
		TypeReference type = null;
		ArrayList rank = null;
		List<Expression> dimension = null;
		Location startLocation = t.Location;
		
		if (
#line  1458 "VBNET.ATG" 
IsSize() && !IsDims()) {
			ArrayInitializationModifier(
#line  1458 "VBNET.ATG" 
out dimension);
		}
		if (
#line  1459 "VBNET.ATG" 
IsDims()) {
			ArrayNameModifier(
#line  1459 "VBNET.ATG" 
out rank);
		}
		if (
#line  1461 "VBNET.ATG" 
IsObjectCreation()) {
			Expect(60);
			ObjectCreateExpression(
#line  1461 "VBNET.ATG" 
out expr);

#line  1463 "VBNET.ATG" 
			if (expr is ObjectCreateExpression) {
			type = ((ObjectCreateExpression)expr).CreateType.Clone();
			} else {
				type = ((ArrayCreateExpression)expr).CreateType.Clone();
			}
			
		} else if (StartOf(23)) {
			if (la.kind == 60) {
				lexer.NextToken();
				TypeName(
#line  1470 "VBNET.ATG" 
out type);

#line  1472 "VBNET.ATG" 
				if (type != null) {
				for (int i = fieldDeclaration.Count - 1; i >= 0; i--) {
					VariableDeclaration vd = fieldDeclaration[i];
					if (vd.TypeReference.Type.Length > 0) break;
					TypeReference newType = type.Clone();
					newType.RankSpecifier = vd.TypeReference.RankSpecifier;
					vd.TypeReference = newType;
				}
				}
				 
			}

#line  1484 "VBNET.ATG" 
			if (type == null && (dimension != null || rank != null)) {
			type = new TypeReference("");
			}
			if (dimension != null) {
				if(type.RankSpecifier != null) {
					Error("array rank only allowed one time");
				} else {
					if (rank == null) {
						type.RankSpecifier = new int[] { dimension.Count - 1 };
					} else {
						rank.Insert(0, dimension.Count - 1);
						type.RankSpecifier = (int[])rank.ToArray(typeof(int));
					}
					expr = new ArrayCreateExpression(type.Clone(), dimension);
				}
			} else if (rank != null) {
				if(type.RankSpecifier != null) {
					Error("array rank only allowed one time");
				} else {
					type.RankSpecifier = (int[])rank.ToArray(typeof(int));
				}
			}
			
			if (la.kind == 19) {
				lexer.NextToken();
				Expr(
#line  1507 "VBNET.ATG" 
out expr);
			}
		} else SynErr(263);

#line  1510 "VBNET.ATG" 
		VariableDeclaration varDecl = new VariableDeclaration(name, expr, type);
		varDecl.StartLocation = startLocation;
		varDecl.EndLocation = t.Location;
		fieldDeclaration.Add(varDecl);
		
	}

	void VariableDeclarator(
#line  1444 "VBNET.ATG" 
List<VariableDeclaration> fieldDeclaration) {
		Identifier();

#line  1446 "VBNET.ATG" 
		string name = t.val; 
		VariableDeclaratorPartAfterIdentifier(
#line  1447 "VBNET.ATG" 
fieldDeclaration, name);
	}

	void ConstantDeclarator(
#line  1425 "VBNET.ATG" 
List<VariableDeclaration> constantDeclaration) {

#line  1427 "VBNET.ATG" 
		Expression expr = null;
		TypeReference type = null;
		string name = String.Empty;
		Location location;
		
		Identifier();

#line  1432 "VBNET.ATG" 
		name = t.val; location = t.Location; 
		if (la.kind == 60) {
			lexer.NextToken();
			TypeName(
#line  1433 "VBNET.ATG" 
out type);
		}
		Expect(19);
		Expr(
#line  1434 "VBNET.ATG" 
out expr);

#line  1436 "VBNET.ATG" 
		VariableDeclaration f = new VariableDeclaration(name, expr);
		f.TypeReference = type;
		f.StartLocation = location;
		constantDeclaration.Add(f);
		
	}

	void ObjectCreateExpression(
#line  1972 "VBNET.ATG" 
out Expression oce) {

#line  1974 "VBNET.ATG" 
		TypeReference type = null;
		CollectionInitializerExpression initializer = null;
		List<Expression> arguments = null;
		ArrayList dimensions = null;
		oce = null;
		bool canBeNormal; bool canBeReDim;
		
		Expect(159);
		if (StartOf(8)) {
			NonArrayTypeName(
#line  1982 "VBNET.ATG" 
out type, false);
			if (la.kind == 34) {
				lexer.NextToken();
				NormalOrReDimArgumentList(
#line  1983 "VBNET.ATG" 
out arguments, out canBeNormal, out canBeReDim);
				Expect(35);
				if (la.kind == 32 || 
#line  1984 "VBNET.ATG" 
la.kind == Tokens.OpenParenthesis) {
					if (
#line  1984 "VBNET.ATG" 
la.kind == Tokens.OpenParenthesis) {
						ArrayTypeModifiers(
#line  1985 "VBNET.ATG" 
out dimensions);
						CollectionInitializer(
#line  1986 "VBNET.ATG" 
out initializer);
					} else {
						CollectionInitializer(
#line  1987 "VBNET.ATG" 
out initializer);
					}
				}

#line  1989 "VBNET.ATG" 
				if (canBeReDim && !canBeNormal && initializer == null) initializer = new CollectionInitializerExpression(); 
			}
		}

#line  1993 "VBNET.ATG" 
		if (initializer == null) {
		oce = new ObjectCreateExpression(type, arguments);
		} else {
			if (dimensions == null) dimensions = new ArrayList();
			dimensions.Insert(0, (arguments == null) ? 0 : Math.Max(arguments.Count - 1, 0));
			type.RankSpecifier = (int[])dimensions.ToArray(typeof(int));
			ArrayCreateExpression ace = new ArrayCreateExpression(type, initializer);
			ace.Arguments = arguments;
			oce = ace;
		}
		
		if (la.kind == 123 || la.kind == 229) {
			if (la.kind == 229) {

#line  2008 "VBNET.ATG" 
				MemberInitializerExpression memberInitializer = null;
				
				lexer.NextToken();

#line  2012 "VBNET.ATG" 
				CollectionInitializerExpression memberInitializers = new CollectionInitializerExpression();
				memberInitializers.StartLocation = la.Location;
				
				Expect(32);
				MemberInitializer(
#line  2016 "VBNET.ATG" 
out memberInitializer);

#line  2017 "VBNET.ATG" 
				memberInitializers.CreateExpressions.Add(memberInitializer); 
				while (la.kind == 21) {
					lexer.NextToken();
					MemberInitializer(
#line  2019 "VBNET.ATG" 
out memberInitializer);

#line  2020 "VBNET.ATG" 
					memberInitializers.CreateExpressions.Add(memberInitializer); 
				}
				Expect(33);

#line  2024 "VBNET.ATG" 
				memberInitializers.EndLocation = t.Location;
				if(oce is ObjectCreateExpression)
				{
					((ObjectCreateExpression)oce).ObjectInitializer = memberInitializers;
				}
				
			} else {
				lexer.NextToken();
				CollectionInitializer(
#line  2034 "VBNET.ATG" 
out initializer);

#line  2036 "VBNET.ATG" 
				if(oce is ObjectCreateExpression)
				((ObjectCreateExpression)oce).ObjectInitializer = initializer;
				
			}
		}
	}

	void AccessorDecls(
#line  1359 "VBNET.ATG" 
out PropertyGetRegion getBlock, out PropertySetRegion setBlock) {

#line  1361 "VBNET.ATG" 
		List<AttributeSection> attributes = new List<AttributeSection>();
		AttributeSection section;
		getBlock = null;
		setBlock = null; 
		
		while (la.kind == 37) {
			AttributeSection(
#line  1366 "VBNET.ATG" 
out section);

#line  1366 "VBNET.ATG" 
			attributes.Add(section); 
		}
		if (StartOf(24)) {
			GetAccessorDecl(
#line  1368 "VBNET.ATG" 
out getBlock, attributes);
			if (StartOf(25)) {

#line  1370 "VBNET.ATG" 
				attributes = new List<AttributeSection>(); 
				while (la.kind == 37) {
					AttributeSection(
#line  1371 "VBNET.ATG" 
out section);

#line  1371 "VBNET.ATG" 
					attributes.Add(section); 
				}
				SetAccessorDecl(
#line  1372 "VBNET.ATG" 
out setBlock, attributes);
			}
		} else if (StartOf(26)) {
			SetAccessorDecl(
#line  1375 "VBNET.ATG" 
out setBlock, attributes);
			if (StartOf(27)) {

#line  1377 "VBNET.ATG" 
				attributes = new List<AttributeSection>(); 
				while (la.kind == 37) {
					AttributeSection(
#line  1378 "VBNET.ATG" 
out section);

#line  1378 "VBNET.ATG" 
					attributes.Add(section); 
				}
				GetAccessorDecl(
#line  1379 "VBNET.ATG" 
out getBlock, attributes);
			}
		} else SynErr(264);
	}

	void EventAccessorDeclaration(
#line  1322 "VBNET.ATG" 
out EventAddRemoveRegion eventAccessorDeclaration) {

#line  1324 "VBNET.ATG" 
		Statement stmt = null;
		List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		eventAccessorDeclaration = null;
		
		while (la.kind == 37) {
			AttributeSection(
#line  1330 "VBNET.ATG" 
out section);

#line  1330 "VBNET.ATG" 
			attributes.Add(section); 
		}
		if (la.kind == 53) {
			lexer.NextToken();
			if (la.kind == 34) {
				lexer.NextToken();
				if (StartOf(6)) {
					FormalParameterList(
#line  1332 "VBNET.ATG" 
p);
				}
				Expect(35);
			}
			Expect(1);
			Block(
#line  1333 "VBNET.ATG" 
out stmt);
			Expect(110);
			Expect(53);
			EndOfStmt();

#line  1335 "VBNET.ATG" 
			eventAccessorDeclaration = new EventAddRegion(attributes);
			eventAccessorDeclaration.Block = (BlockStatement)stmt;
			eventAccessorDeclaration.Parameters = p;
			
		} else if (la.kind == 189) {
			lexer.NextToken();
			if (la.kind == 34) {
				lexer.NextToken();
				if (StartOf(6)) {
					FormalParameterList(
#line  1340 "VBNET.ATG" 
p);
				}
				Expect(35);
			}
			Expect(1);
			Block(
#line  1341 "VBNET.ATG" 
out stmt);
			Expect(110);
			Expect(189);
			EndOfStmt();

#line  1343 "VBNET.ATG" 
			eventAccessorDeclaration = new EventRemoveRegion(attributes);
			eventAccessorDeclaration.Block = (BlockStatement)stmt;
			eventAccessorDeclaration.Parameters = p;
			
		} else if (la.kind == 185) {
			lexer.NextToken();
			if (la.kind == 34) {
				lexer.NextToken();
				if (StartOf(6)) {
					FormalParameterList(
#line  1348 "VBNET.ATG" 
p);
				}
				Expect(35);
			}
			Expect(1);
			Block(
#line  1349 "VBNET.ATG" 
out stmt);
			Expect(110);
			Expect(185);
			EndOfStmt();

#line  1351 "VBNET.ATG" 
			eventAccessorDeclaration = new EventRaiseRegion(attributes);
			eventAccessorDeclaration.Block = (BlockStatement)stmt;
			eventAccessorDeclaration.Parameters = p;
			
		} else SynErr(265);
	}

	void OverloadableOperator(
#line  1264 "VBNET.ATG" 
out OverloadableOperatorType operatorType) {

#line  1265 "VBNET.ATG" 
		operatorType = OverloadableOperatorType.None; 
		switch (la.kind) {
		case 28: {
			lexer.NextToken();

#line  1267 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Add; 
			break;
		}
		case 27: {
			lexer.NextToken();

#line  1269 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Subtract; 
			break;
		}
		case 31: {
			lexer.NextToken();

#line  1271 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Multiply; 
			break;
		}
		case 23: {
			lexer.NextToken();

#line  1273 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Divide; 
			break;
		}
		case 24: {
			lexer.NextToken();

#line  1275 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.DivideInteger; 
			break;
		}
		case 22: {
			lexer.NextToken();

#line  1277 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Concat; 
			break;
		}
		case 147: {
			lexer.NextToken();

#line  1279 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Like; 
			break;
		}
		case 151: {
			lexer.NextToken();

#line  1281 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Modulus; 
			break;
		}
		case 57: {
			lexer.NextToken();

#line  1283 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.BitwiseAnd; 
			break;
		}
		case 172: {
			lexer.NextToken();

#line  1285 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.BitwiseOr; 
			break;
		}
		case 232: {
			lexer.NextToken();

#line  1287 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.ExclusiveOr; 
			break;
		}
		case 29: {
			lexer.NextToken();

#line  1289 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Power; 
			break;
		}
		case 41: {
			lexer.NextToken();

#line  1291 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.ShiftLeft; 
			break;
		}
		case 42: {
			lexer.NextToken();

#line  1293 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.ShiftRight; 
			break;
		}
		case 19: {
			lexer.NextToken();

#line  1295 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Equality; 
			break;
		}
		case 38: {
			lexer.NextToken();

#line  1297 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.InEquality; 
			break;
		}
		case 37: {
			lexer.NextToken();

#line  1299 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.LessThan; 
			break;
		}
		case 40: {
			lexer.NextToken();

#line  1301 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.LessThanOrEqual; 
			break;
		}
		case 36: {
			lexer.NextToken();

#line  1303 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.GreaterThan; 
			break;
		}
		case 39: {
			lexer.NextToken();

#line  1305 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.GreaterThanOrEqual; 
			break;
		}
		case 91: {
			lexer.NextToken();

#line  1307 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.CType; 
			break;
		}
		case 2: case 55: case 59: case 61: case 62: case 63: case 64: case 67: case 84: case 95: case 101: case 104: case 113: case 118: case 123: case 130: case 136: case 140: case 143: case 167: case 173: case 180: case 199: case 208: case 209: case 219: case 220: case 226: {
			Identifier();

#line  1311 "VBNET.ATG" 
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
		default: SynErr(266); break;
		}
	}

	void GetAccessorDecl(
#line  1385 "VBNET.ATG" 
out PropertyGetRegion getBlock, List<AttributeSection> attributes) {

#line  1386 "VBNET.ATG" 
		Statement stmt = null; Modifiers m; 
		PropertyAccessorAccessModifier(
#line  1388 "VBNET.ATG" 
out m);
		Expect(125);

#line  1390 "VBNET.ATG" 
		Location startLocation = t.Location; 
		Expect(1);
		Block(
#line  1392 "VBNET.ATG" 
out stmt);

#line  1393 "VBNET.ATG" 
		getBlock = new PropertyGetRegion((BlockStatement)stmt, attributes); 
		Expect(110);
		Expect(125);

#line  1395 "VBNET.ATG" 
		getBlock.Modifier = m; 

#line  1396 "VBNET.ATG" 
		getBlock.StartLocation = startLocation; getBlock.EndLocation = t.EndLocation; 
		EndOfStmt();
	}

	void SetAccessorDecl(
#line  1401 "VBNET.ATG" 
out PropertySetRegion setBlock, List<AttributeSection> attributes) {

#line  1403 "VBNET.ATG" 
		Statement stmt = null;
		List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
		Modifiers m;
		
		PropertyAccessorAccessModifier(
#line  1408 "VBNET.ATG" 
out m);
		Expect(194);

#line  1410 "VBNET.ATG" 
		Location startLocation = t.Location; 
		if (la.kind == 34) {
			lexer.NextToken();
			if (StartOf(6)) {
				FormalParameterList(
#line  1411 "VBNET.ATG" 
p);
			}
			Expect(35);
		}
		Expect(1);
		Block(
#line  1413 "VBNET.ATG" 
out stmt);

#line  1415 "VBNET.ATG" 
		setBlock = new PropertySetRegion((BlockStatement)stmt, attributes);
		setBlock.Modifier = m;
		setBlock.Parameters = p;
		
		Expect(110);
		Expect(194);

#line  1420 "VBNET.ATG" 
		setBlock.StartLocation = startLocation; setBlock.EndLocation = t.EndLocation; 
		EndOfStmt();
	}

	void PropertyAccessorAccessModifier(
#line  3460 "VBNET.ATG" 
out Modifiers m) {

#line  3461 "VBNET.ATG" 
		m = Modifiers.None; 
		while (StartOf(28)) {
			if (la.kind == 184) {
				lexer.NextToken();

#line  3463 "VBNET.ATG" 
				m |= Modifiers.Public; 
			} else if (la.kind == 183) {
				lexer.NextToken();

#line  3464 "VBNET.ATG" 
				m |= Modifiers.Protected; 
			} else if (la.kind == 122) {
				lexer.NextToken();

#line  3465 "VBNET.ATG" 
				m |= Modifiers.Internal; 
			} else {
				lexer.NextToken();

#line  3466 "VBNET.ATG" 
				m |= Modifiers.Private; 
			}
		}
	}

	void ArrayInitializationModifier(
#line  1518 "VBNET.ATG" 
out List<Expression> arrayModifiers) {

#line  1520 "VBNET.ATG" 
		arrayModifiers = null;
		
		Expect(34);
		InitializationRankList(
#line  1522 "VBNET.ATG" 
out arrayModifiers);
		Expect(35);
	}

	void ArrayNameModifier(
#line  2537 "VBNET.ATG" 
out ArrayList arrayModifiers) {

#line  2539 "VBNET.ATG" 
		arrayModifiers = null;
		
		ArrayTypeModifiers(
#line  2541 "VBNET.ATG" 
out arrayModifiers);
	}

	void InitializationRankList(
#line  1526 "VBNET.ATG" 
out List<Expression> rank) {

#line  1528 "VBNET.ATG" 
		rank = new List<Expression>();
		Expression expr = null;
		
		Expr(
#line  1531 "VBNET.ATG" 
out expr);
		if (la.kind == 212) {
			lexer.NextToken();

#line  1532 "VBNET.ATG" 
			EnsureIsZero(expr); 
			Expr(
#line  1533 "VBNET.ATG" 
out expr);
		}

#line  1535 "VBNET.ATG" 
		if (expr != null) { rank.Add(expr); } 
		while (la.kind == 21) {
			lexer.NextToken();
			Expr(
#line  1537 "VBNET.ATG" 
out expr);
			if (la.kind == 212) {
				lexer.NextToken();

#line  1538 "VBNET.ATG" 
				EnsureIsZero(expr); 
				Expr(
#line  1539 "VBNET.ATG" 
out expr);
			}

#line  1541 "VBNET.ATG" 
			if (expr != null) { rank.Add(expr); } 
		}
	}

	void CollectionInitializer(
#line  1546 "VBNET.ATG" 
out CollectionInitializerExpression outExpr) {

#line  1548 "VBNET.ATG" 
		Expression expr = null;
		CollectionInitializerExpression initializer = new CollectionInitializerExpression();
		
		Expect(32);
		if (StartOf(29)) {
			Expr(
#line  1553 "VBNET.ATG" 
out expr);

#line  1555 "VBNET.ATG" 
			if (expr != null) { initializer.CreateExpressions.Add(expr); }
			
			while (
#line  1558 "VBNET.ATG" 
NotFinalComma()) {
				Expect(21);
				Expr(
#line  1558 "VBNET.ATG" 
out expr);

#line  1559 "VBNET.ATG" 
				if (expr != null) { initializer.CreateExpressions.Add(expr); } 
			}
		}
		Expect(33);

#line  1562 "VBNET.ATG" 
		outExpr = initializer; 
	}

	void EventMemberSpecifier(
#line  1632 "VBNET.ATG" 
out string name) {

#line  1633 "VBNET.ATG" 
		string eventName; 
		if (StartOf(4)) {
			Identifier();
		} else if (la.kind == 155) {
			lexer.NextToken();
		} else if (la.kind == 150) {
			lexer.NextToken();
		} else SynErr(267);

#line  1636 "VBNET.ATG" 
		name = t.val; 
		Expect(25);
		IdentifierOrKeyword(
#line  1638 "VBNET.ATG" 
out eventName);

#line  1639 "VBNET.ATG" 
		name = name + "." + eventName; 
	}

	void IdentifierOrKeyword(
#line  3393 "VBNET.ATG" 
out string name) {
		lexer.NextToken();

#line  3395 "VBNET.ATG" 
		name = t.val;  
	}

	void QueryExpr(
#line  2061 "VBNET.ATG" 
out Expression expr) {

#line  2063 "VBNET.ATG" 
		QueryExpressionVB qexpr = new QueryExpressionVB();
		qexpr.StartLocation = la.Location;
		expr = qexpr;
		
		FromOrAggregateQueryOperator(
#line  2067 "VBNET.ATG" 
qexpr.Clauses);
		while (StartOf(30)) {
			QueryOperator(
#line  2068 "VBNET.ATG" 
qexpr.Clauses);
		}

#line  2070 "VBNET.ATG" 
		qexpr.EndLocation = t.EndLocation;
		
	}

	void LambdaExpr(
#line  2043 "VBNET.ATG" 
out Expression expr) {

#line  2045 "VBNET.ATG" 
		Expression inner = null;
		LambdaExpression lambda = new LambdaExpression();
		lambda.StartLocation = la.Location;
		
		Expect(124);
		if (la.kind == 34) {
			lexer.NextToken();
			if (StartOf(6)) {
				FormalParameterList(
#line  2051 "VBNET.ATG" 
lambda.Parameters);
			}
			Expect(35);
		}
		Expr(
#line  2052 "VBNET.ATG" 
out inner);

#line  2054 "VBNET.ATG" 
		lambda.ExpressionBody = inner;
		lambda.EndLocation = t.EndLocation; // la.Location?
		
		expr = lambda;
		
	}

	void DisjunctionExpr(
#line  1816 "VBNET.ATG" 
out Expression outExpr) {

#line  1818 "VBNET.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		ConjunctionExpr(
#line  1821 "VBNET.ATG" 
out outExpr);
		while (la.kind == 172 || la.kind == 174 || la.kind == 232) {
			if (la.kind == 172) {
				lexer.NextToken();

#line  1824 "VBNET.ATG" 
				op = BinaryOperatorType.BitwiseOr; 
			} else if (la.kind == 174) {
				lexer.NextToken();

#line  1825 "VBNET.ATG" 
				op = BinaryOperatorType.LogicalOr; 
			} else {
				lexer.NextToken();

#line  1826 "VBNET.ATG" 
				op = BinaryOperatorType.ExclusiveOr; 
			}
			ConjunctionExpr(
#line  1828 "VBNET.ATG" 
out expr);

#line  1828 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr);  
		}
	}

	void AssignmentOperator(
#line  1650 "VBNET.ATG" 
out AssignmentOperatorType op) {

#line  1651 "VBNET.ATG" 
		op = AssignmentOperatorType.None; 
		switch (la.kind) {
		case 19: {
			lexer.NextToken();

#line  1652 "VBNET.ATG" 
			op = AssignmentOperatorType.Assign; 
			break;
		}
		case 51: {
			lexer.NextToken();

#line  1653 "VBNET.ATG" 
			op = AssignmentOperatorType.ConcatString; 
			break;
		}
		case 43: {
			lexer.NextToken();

#line  1654 "VBNET.ATG" 
			op = AssignmentOperatorType.Add; 
			break;
		}
		case 45: {
			lexer.NextToken();

#line  1655 "VBNET.ATG" 
			op = AssignmentOperatorType.Subtract; 
			break;
		}
		case 46: {
			lexer.NextToken();

#line  1656 "VBNET.ATG" 
			op = AssignmentOperatorType.Multiply; 
			break;
		}
		case 47: {
			lexer.NextToken();

#line  1657 "VBNET.ATG" 
			op = AssignmentOperatorType.Divide; 
			break;
		}
		case 48: {
			lexer.NextToken();

#line  1658 "VBNET.ATG" 
			op = AssignmentOperatorType.DivideInteger; 
			break;
		}
		case 44: {
			lexer.NextToken();

#line  1659 "VBNET.ATG" 
			op = AssignmentOperatorType.Power; 
			break;
		}
		case 49: {
			lexer.NextToken();

#line  1660 "VBNET.ATG" 
			op = AssignmentOperatorType.ShiftLeft; 
			break;
		}
		case 50: {
			lexer.NextToken();

#line  1661 "VBNET.ATG" 
			op = AssignmentOperatorType.ShiftRight; 
			break;
		}
		default: SynErr(268); break;
		}
	}

	void SimpleExpr(
#line  1665 "VBNET.ATG" 
out Expression pexpr) {

#line  1666 "VBNET.ATG" 
		string name; 
		SimpleNonInvocationExpression(
#line  1668 "VBNET.ATG" 
out pexpr);
		while (la.kind == 25 || la.kind == 26 || la.kind == 34) {
			if (la.kind == 25) {
				lexer.NextToken();
				IdentifierOrKeyword(
#line  1670 "VBNET.ATG" 
out name);

#line  1671 "VBNET.ATG" 
				pexpr = new MemberReferenceExpression(pexpr, name); 
				if (
#line  1672 "VBNET.ATG" 
la.kind == Tokens.OpenParenthesis && Peek(1).kind == Tokens.Of) {
					lexer.NextToken();
					Expect(166);
					TypeArgumentList(
#line  1673 "VBNET.ATG" 
((MemberReferenceExpression)pexpr).TypeArguments);
					Expect(35);
				}
			} else if (la.kind == 26) {
				lexer.NextToken();
				IdentifierOrKeyword(
#line  1675 "VBNET.ATG" 
out name);

#line  1675 "VBNET.ATG" 
				pexpr = new BinaryOperatorExpression(pexpr, BinaryOperatorType.DictionaryAccess, new PrimitiveExpression(name, name)); 
			} else {
				InvocationExpression(
#line  1676 "VBNET.ATG" 
ref pexpr);
			}
		}
	}

	void SimpleNonInvocationExpression(
#line  1680 "VBNET.ATG" 
out Expression pexpr) {

#line  1682 "VBNET.ATG" 
		Expression expr;
		CollectionInitializerExpression cie;
		TypeReference type = null;
		string name = String.Empty;
		pexpr = null;
		
		if (StartOf(31)) {
			switch (la.kind) {
			case 3: {
				lexer.NextToken();

#line  1691 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(t.literalValue, t.val) { LiteralFormat = t.literalFormat };  
				break;
			}
			case 4: {
				lexer.NextToken();

#line  1692 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(t.literalValue, t.val) { LiteralFormat = t.literalFormat };  
				break;
			}
			case 7: {
				lexer.NextToken();

#line  1693 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(t.literalValue, t.val) { LiteralFormat = t.literalFormat };  
				break;
			}
			case 6: {
				lexer.NextToken();

#line  1694 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(t.literalValue, t.val) { LiteralFormat = t.literalFormat };  
				break;
			}
			case 5: {
				lexer.NextToken();

#line  1695 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(t.literalValue, t.val) { LiteralFormat = t.literalFormat };  
				break;
			}
			case 9: {
				lexer.NextToken();

#line  1696 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(t.literalValue, t.val) { LiteralFormat = t.literalFormat };  
				break;
			}
			case 8: {
				lexer.NextToken();

#line  1697 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(t.literalValue, t.val) { LiteralFormat = t.literalFormat };  
				break;
			}
			case 213: {
				lexer.NextToken();

#line  1699 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(true, "true");  
				break;
			}
			case 119: {
				lexer.NextToken();

#line  1700 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(false, "false"); 
				break;
			}
			case 162: {
				lexer.NextToken();

#line  1701 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(null, "null");  
				break;
			}
			case 34: {
				lexer.NextToken();
				Expr(
#line  1702 "VBNET.ATG" 
out expr);
				Expect(35);

#line  1702 "VBNET.ATG" 
				pexpr = new ParenthesizedExpression(expr); 
				break;
			}
			case 2: case 55: case 59: case 61: case 62: case 63: case 64: case 67: case 84: case 95: case 101: case 104: case 113: case 118: case 123: case 130: case 136: case 140: case 143: case 167: case 173: case 180: case 199: case 208: case 209: case 219: case 220: case 226: {
				Identifier();

#line  1704 "VBNET.ATG" 
				pexpr = new IdentifierExpression(t.val);
				pexpr.StartLocation = t.Location; pexpr.EndLocation = t.EndLocation;
				
				if (
#line  1707 "VBNET.ATG" 
la.kind == Tokens.OpenParenthesis && Peek(1).kind == Tokens.Of) {
					lexer.NextToken();
					Expect(166);
					TypeArgumentList(
#line  1708 "VBNET.ATG" 
((IdentifierExpression)pexpr).TypeArguments);
					Expect(35);
				}
				break;
			}
			case 65: case 68: case 79: case 96: case 97: case 106: case 138: case 148: case 165: case 192: case 197: case 198: case 204: case 217: case 218: case 221: {

#line  1710 "VBNET.ATG" 
				string val = String.Empty; 
				if (StartOf(12)) {
					PrimitiveTypeName(
#line  1711 "VBNET.ATG" 
out val);
				} else if (la.kind == 165) {
					lexer.NextToken();

#line  1711 "VBNET.ATG" 
					val = "System.Object"; 
				} else SynErr(269);

#line  1712 "VBNET.ATG" 
				pexpr = new TypeReferenceExpression(new TypeReference(val, true)); 
				break;
			}
			case 150: {
				lexer.NextToken();

#line  1713 "VBNET.ATG" 
				pexpr = new ThisReferenceExpression(); 
				break;
			}
			case 155: case 156: {

#line  1714 "VBNET.ATG" 
				Expression retExpr = null; 
				if (la.kind == 155) {
					lexer.NextToken();

#line  1715 "VBNET.ATG" 
					retExpr = new BaseReferenceExpression(); 
				} else if (la.kind == 156) {
					lexer.NextToken();

#line  1716 "VBNET.ATG" 
					retExpr = new ClassReferenceExpression(); 
				} else SynErr(270);
				Expect(25);
				IdentifierOrKeyword(
#line  1718 "VBNET.ATG" 
out name);

#line  1718 "VBNET.ATG" 
				pexpr = new MemberReferenceExpression(retExpr, name); 
				break;
			}
			case 127: {
				lexer.NextToken();
				Expect(25);
				Identifier();

#line  1720 "VBNET.ATG" 
				type = new TypeReference(t.val ?? ""); 

#line  1722 "VBNET.ATG" 
				type.IsGlobal = true; 

#line  1723 "VBNET.ATG" 
				pexpr = new TypeReferenceExpression(type); 
				break;
			}
			case 159: {
				ObjectCreateExpression(
#line  1724 "VBNET.ATG" 
out expr);

#line  1724 "VBNET.ATG" 
				pexpr = expr; 
				break;
			}
			case 32: {
				CollectionInitializer(
#line  1725 "VBNET.ATG" 
out cie);

#line  1725 "VBNET.ATG" 
				pexpr = cie; 
				break;
			}
			case 91: case 103: case 215: {

#line  1727 "VBNET.ATG" 
				CastType castType = CastType.Cast; 
				if (la.kind == 103) {
					lexer.NextToken();
				} else if (la.kind == 91) {
					lexer.NextToken();

#line  1729 "VBNET.ATG" 
					castType = CastType.Conversion; 
				} else if (la.kind == 215) {
					lexer.NextToken();

#line  1730 "VBNET.ATG" 
					castType = CastType.TryCast; 
				} else SynErr(271);
				Expect(34);
				Expr(
#line  1732 "VBNET.ATG" 
out expr);
				Expect(21);
				TypeName(
#line  1732 "VBNET.ATG" 
out type);
				Expect(35);

#line  1733 "VBNET.ATG" 
				pexpr = new CastExpression(type, expr, castType); 
				break;
			}
			case 73: case 74: case 75: case 76: case 77: case 78: case 80: case 82: case 83: case 87: case 88: case 89: case 90: case 92: case 93: case 94: {
				CastTarget(
#line  1734 "VBNET.ATG" 
out type);
				Expect(34);
				Expr(
#line  1734 "VBNET.ATG" 
out expr);
				Expect(35);

#line  1734 "VBNET.ATG" 
				pexpr = new CastExpression(type, expr, CastType.PrimitiveConversion); 
				break;
			}
			case 54: {
				lexer.NextToken();
				Expr(
#line  1735 "VBNET.ATG" 
out expr);

#line  1735 "VBNET.ATG" 
				pexpr = new AddressOfExpression(expr); 
				break;
			}
			case 126: {
				lexer.NextToken();
				Expect(34);
				GetTypeTypeName(
#line  1736 "VBNET.ATG" 
out type);
				Expect(35);

#line  1736 "VBNET.ATG" 
				pexpr = new TypeOfExpression(type); 
				break;
			}
			case 216: {
				lexer.NextToken();
				SimpleExpr(
#line  1737 "VBNET.ATG" 
out expr);
				Expect(141);
				TypeName(
#line  1737 "VBNET.ATG" 
out type);

#line  1737 "VBNET.ATG" 
				pexpr = new TypeOfIsExpression(expr, type); 
				break;
			}
			case 132: {
				ConditionalExpression(
#line  1738 "VBNET.ATG" 
out pexpr);
				break;
			}
			}
		} else if (la.kind == 25) {
			lexer.NextToken();
			IdentifierOrKeyword(
#line  1742 "VBNET.ATG" 
out name);

#line  1742 "VBNET.ATG" 
			pexpr = new MemberReferenceExpression(null, name);
		} else SynErr(272);
	}

	void TypeArgumentList(
#line  2573 "VBNET.ATG" 
List<TypeReference> typeArguments) {

#line  2575 "VBNET.ATG" 
		TypeReference typeref;
		
		TypeName(
#line  2577 "VBNET.ATG" 
out typeref);

#line  2577 "VBNET.ATG" 
		if (typeref != null) typeArguments.Add(typeref); 
		while (la.kind == 21) {
			lexer.NextToken();
			TypeName(
#line  2580 "VBNET.ATG" 
out typeref);

#line  2580 "VBNET.ATG" 
			if (typeref != null) typeArguments.Add(typeref); 
		}
	}

	void InvocationExpression(
#line  1780 "VBNET.ATG" 
ref Expression pexpr) {

#line  1781 "VBNET.ATG" 
		List<Expression> parameters = null; 
		Expect(34);

#line  1783 "VBNET.ATG" 
		Location start = t.Location; 
		ArgumentList(
#line  1784 "VBNET.ATG" 
out parameters);
		Expect(35);

#line  1787 "VBNET.ATG" 
		pexpr = new InvocationExpression(pexpr, parameters);
		

#line  1789 "VBNET.ATG" 
		pexpr.StartLocation = start; pexpr.EndLocation = t.Location; 
	}

	void PrimitiveTypeName(
#line  3400 "VBNET.ATG" 
out string type) {

#line  3401 "VBNET.ATG" 
		type = String.Empty; 
		switch (la.kind) {
		case 65: {
			lexer.NextToken();

#line  3402 "VBNET.ATG" 
			type = "System.Boolean"; 
			break;
		}
		case 96: {
			lexer.NextToken();

#line  3403 "VBNET.ATG" 
			type = "System.DateTime"; 
			break;
		}
		case 79: {
			lexer.NextToken();

#line  3404 "VBNET.ATG" 
			type = "System.Char"; 
			break;
		}
		case 204: {
			lexer.NextToken();

#line  3405 "VBNET.ATG" 
			type = "System.String"; 
			break;
		}
		case 97: {
			lexer.NextToken();

#line  3406 "VBNET.ATG" 
			type = "System.Decimal"; 
			break;
		}
		case 68: {
			lexer.NextToken();

#line  3407 "VBNET.ATG" 
			type = "System.Byte"; 
			break;
		}
		case 197: {
			lexer.NextToken();

#line  3408 "VBNET.ATG" 
			type = "System.Int16"; 
			break;
		}
		case 138: {
			lexer.NextToken();

#line  3409 "VBNET.ATG" 
			type = "System.Int32"; 
			break;
		}
		case 148: {
			lexer.NextToken();

#line  3410 "VBNET.ATG" 
			type = "System.Int64"; 
			break;
		}
		case 198: {
			lexer.NextToken();

#line  3411 "VBNET.ATG" 
			type = "System.Single"; 
			break;
		}
		case 106: {
			lexer.NextToken();

#line  3412 "VBNET.ATG" 
			type = "System.Double"; 
			break;
		}
		case 217: {
			lexer.NextToken();

#line  3413 "VBNET.ATG" 
			type = "System.UInt32"; 
			break;
		}
		case 218: {
			lexer.NextToken();

#line  3414 "VBNET.ATG" 
			type = "System.UInt64"; 
			break;
		}
		case 221: {
			lexer.NextToken();

#line  3415 "VBNET.ATG" 
			type = "System.UInt16"; 
			break;
		}
		case 192: {
			lexer.NextToken();

#line  3416 "VBNET.ATG" 
			type = "System.SByte"; 
			break;
		}
		default: SynErr(273); break;
		}
	}

	void CastTarget(
#line  1794 "VBNET.ATG" 
out TypeReference type) {

#line  1796 "VBNET.ATG" 
		type = null;
		
		switch (la.kind) {
		case 73: {
			lexer.NextToken();

#line  1798 "VBNET.ATG" 
			type = new TypeReference("System.Boolean", true); 
			break;
		}
		case 74: {
			lexer.NextToken();

#line  1799 "VBNET.ATG" 
			type = new TypeReference("System.Byte", true); 
			break;
		}
		case 87: {
			lexer.NextToken();

#line  1800 "VBNET.ATG" 
			type = new TypeReference("System.SByte", true); 
			break;
		}
		case 75: {
			lexer.NextToken();

#line  1801 "VBNET.ATG" 
			type = new TypeReference("System.Char", true); 
			break;
		}
		case 76: {
			lexer.NextToken();

#line  1802 "VBNET.ATG" 
			type = new TypeReference("System.DateTime", true); 
			break;
		}
		case 78: {
			lexer.NextToken();

#line  1803 "VBNET.ATG" 
			type = new TypeReference("System.Decimal", true); 
			break;
		}
		case 77: {
			lexer.NextToken();

#line  1804 "VBNET.ATG" 
			type = new TypeReference("System.Double", true); 
			break;
		}
		case 88: {
			lexer.NextToken();

#line  1805 "VBNET.ATG" 
			type = new TypeReference("System.Int16", true); 
			break;
		}
		case 80: {
			lexer.NextToken();

#line  1806 "VBNET.ATG" 
			type = new TypeReference("System.Int32", true); 
			break;
		}
		case 82: {
			lexer.NextToken();

#line  1807 "VBNET.ATG" 
			type = new TypeReference("System.Int64", true); 
			break;
		}
		case 94: {
			lexer.NextToken();

#line  1808 "VBNET.ATG" 
			type = new TypeReference("System.UInt16", true); 
			break;
		}
		case 92: {
			lexer.NextToken();

#line  1809 "VBNET.ATG" 
			type = new TypeReference("System.UInt32", true); 
			break;
		}
		case 93: {
			lexer.NextToken();

#line  1810 "VBNET.ATG" 
			type = new TypeReference("System.UInt64", true); 
			break;
		}
		case 83: {
			lexer.NextToken();

#line  1811 "VBNET.ATG" 
			type = new TypeReference("System.Object", true); 
			break;
		}
		case 89: {
			lexer.NextToken();

#line  1812 "VBNET.ATG" 
			type = new TypeReference("System.Single", true); 
			break;
		}
		case 90: {
			lexer.NextToken();

#line  1813 "VBNET.ATG" 
			type = new TypeReference("System.String", true); 
			break;
		}
		default: SynErr(274); break;
		}
	}

	void GetTypeTypeName(
#line  2472 "VBNET.ATG" 
out TypeReference typeref) {

#line  2473 "VBNET.ATG" 
		ArrayList rank = null; 
		NonArrayTypeName(
#line  2475 "VBNET.ATG" 
out typeref, true);
		ArrayTypeModifiers(
#line  2476 "VBNET.ATG" 
out rank);

#line  2477 "VBNET.ATG" 
		if (rank != null && typeref != null) {
		typeref.RankSpecifier = (int[])rank.ToArray(typeof(int));
		}
		
	}

	void ConditionalExpression(
#line  1746 "VBNET.ATG" 
out Expression expr) {

#line  1748 "VBNET.ATG" 
		ConditionalExpression conditionalExpression = new ConditionalExpression();
		BinaryOperatorExpression binaryOperatorExpression = new BinaryOperatorExpression();
		conditionalExpression.StartLocation = binaryOperatorExpression.StartLocation = la.Location;
		
		Expression condition = null;
		Expression trueExpr = null;
		Expression falseExpr = null;
		
		Expect(132);
		Expect(34);
		Expr(
#line  1757 "VBNET.ATG" 
out condition);
		Expect(21);
		Expr(
#line  1757 "VBNET.ATG" 
out trueExpr);
		if (la.kind == 21) {
			lexer.NextToken();
			Expr(
#line  1757 "VBNET.ATG" 
out falseExpr);
		}
		Expect(35);

#line  1759 "VBNET.ATG" 
		if(falseExpr != null)
		{
			conditionalExpression.Condition = condition;
			conditionalExpression.TrueExpression = trueExpr;
			conditionalExpression.FalseExpression = falseExpr;
			conditionalExpression.EndLocation = t.EndLocation;
			
			expr = conditionalExpression;
		}
		else
		{
			binaryOperatorExpression.Left = condition;
			binaryOperatorExpression.Right = trueExpr;
			binaryOperatorExpression.Op = BinaryOperatorType.NullCoalescing;
			binaryOperatorExpression.EndLocation = t.EndLocation;
			
			expr = binaryOperatorExpression;
		}
		
	}

	void ArgumentList(
#line  2404 "VBNET.ATG" 
out List<Expression> arguments) {

#line  2406 "VBNET.ATG" 
		arguments = new List<Expression>();
		Expression expr = null;
		
		if (StartOf(29)) {
			Argument(
#line  2409 "VBNET.ATG" 
out expr);
		}
		while (la.kind == 21) {
			lexer.NextToken();

#line  2410 "VBNET.ATG" 
			arguments.Add(expr ?? Expression.Null); expr = null; 
			if (StartOf(29)) {
				Argument(
#line  2411 "VBNET.ATG" 
out expr);
			}

#line  2412 "VBNET.ATG" 
			if (expr == null) expr = Expression.Null; 
		}

#line  2414 "VBNET.ATG" 
		if (expr != null) arguments.Add(expr); 
	}

	void ConjunctionExpr(
#line  1832 "VBNET.ATG" 
out Expression outExpr) {

#line  1834 "VBNET.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		NotExpr(
#line  1837 "VBNET.ATG" 
out outExpr);
		while (la.kind == 57 || la.kind == 58) {
			if (la.kind == 57) {
				lexer.NextToken();

#line  1840 "VBNET.ATG" 
				op = BinaryOperatorType.BitwiseAnd; 
			} else {
				lexer.NextToken();

#line  1841 "VBNET.ATG" 
				op = BinaryOperatorType.LogicalAnd; 
			}
			NotExpr(
#line  1843 "VBNET.ATG" 
out expr);

#line  1843 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr);  
		}
	}

	void NotExpr(
#line  1847 "VBNET.ATG" 
out Expression outExpr) {

#line  1848 "VBNET.ATG" 
		UnaryOperatorType uop = UnaryOperatorType.None; 
		while (la.kind == 161) {
			lexer.NextToken();

#line  1849 "VBNET.ATG" 
			uop = UnaryOperatorType.Not; 
		}
		ComparisonExpr(
#line  1850 "VBNET.ATG" 
out outExpr);

#line  1851 "VBNET.ATG" 
		if (uop != UnaryOperatorType.None)
		outExpr = new UnaryOperatorExpression(outExpr, uop);
		
	}

	void ComparisonExpr(
#line  1856 "VBNET.ATG" 
out Expression outExpr) {

#line  1858 "VBNET.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		ShiftExpr(
#line  1861 "VBNET.ATG" 
out outExpr);
		while (StartOf(32)) {
			switch (la.kind) {
			case 37: {
				lexer.NextToken();

#line  1864 "VBNET.ATG" 
				op = BinaryOperatorType.LessThan; 
				break;
			}
			case 36: {
				lexer.NextToken();

#line  1865 "VBNET.ATG" 
				op = BinaryOperatorType.GreaterThan; 
				break;
			}
			case 40: {
				lexer.NextToken();

#line  1866 "VBNET.ATG" 
				op = BinaryOperatorType.LessThanOrEqual; 
				break;
			}
			case 39: {
				lexer.NextToken();

#line  1867 "VBNET.ATG" 
				op = BinaryOperatorType.GreaterThanOrEqual; 
				break;
			}
			case 38: {
				lexer.NextToken();

#line  1868 "VBNET.ATG" 
				op = BinaryOperatorType.InEquality; 
				break;
			}
			case 19: {
				lexer.NextToken();

#line  1869 "VBNET.ATG" 
				op = BinaryOperatorType.Equality; 
				break;
			}
			case 147: {
				lexer.NextToken();

#line  1870 "VBNET.ATG" 
				op = BinaryOperatorType.Like; 
				break;
			}
			case 141: {
				lexer.NextToken();

#line  1871 "VBNET.ATG" 
				op = BinaryOperatorType.ReferenceEquality; 
				break;
			}
			case 142: {
				lexer.NextToken();

#line  1872 "VBNET.ATG" 
				op = BinaryOperatorType.ReferenceInequality; 
				break;
			}
			}
			if (StartOf(33)) {
				ShiftExpr(
#line  1875 "VBNET.ATG" 
out expr);

#line  1875 "VBNET.ATG" 
				outExpr = new BinaryOperatorExpression(outExpr, op, expr);  
			} else if (la.kind == 161) {
				lexer.NextToken();
				ShiftExpr(
#line  1878 "VBNET.ATG" 
out expr);

#line  1878 "VBNET.ATG" 
				outExpr = new BinaryOperatorExpression(outExpr, op, new UnaryOperatorExpression(expr, UnaryOperatorType.Not));  
			} else SynErr(275);
		}
	}

	void ShiftExpr(
#line  1883 "VBNET.ATG" 
out Expression outExpr) {

#line  1885 "VBNET.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		ConcatenationExpr(
#line  1888 "VBNET.ATG" 
out outExpr);
		while (la.kind == 41 || la.kind == 42) {
			if (la.kind == 41) {
				lexer.NextToken();

#line  1891 "VBNET.ATG" 
				op = BinaryOperatorType.ShiftLeft; 
			} else {
				lexer.NextToken();

#line  1892 "VBNET.ATG" 
				op = BinaryOperatorType.ShiftRight; 
			}
			ConcatenationExpr(
#line  1894 "VBNET.ATG" 
out expr);

#line  1894 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr);  
		}
	}

	void ConcatenationExpr(
#line  1898 "VBNET.ATG" 
out Expression outExpr) {

#line  1899 "VBNET.ATG" 
		Expression expr; 
		AdditiveExpr(
#line  1901 "VBNET.ATG" 
out outExpr);
		while (la.kind == 22) {
			lexer.NextToken();
			AdditiveExpr(
#line  1901 "VBNET.ATG" 
out expr);

#line  1901 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.Concat, expr);  
		}
	}

	void AdditiveExpr(
#line  1904 "VBNET.ATG" 
out Expression outExpr) {

#line  1906 "VBNET.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		ModuloExpr(
#line  1909 "VBNET.ATG" 
out outExpr);
		while (la.kind == 27 || la.kind == 28) {
			if (la.kind == 28) {
				lexer.NextToken();

#line  1912 "VBNET.ATG" 
				op = BinaryOperatorType.Add; 
			} else {
				lexer.NextToken();

#line  1913 "VBNET.ATG" 
				op = BinaryOperatorType.Subtract; 
			}
			ModuloExpr(
#line  1915 "VBNET.ATG" 
out expr);

#line  1915 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr);  
		}
	}

	void ModuloExpr(
#line  1919 "VBNET.ATG" 
out Expression outExpr) {

#line  1920 "VBNET.ATG" 
		Expression expr; 
		IntegerDivisionExpr(
#line  1922 "VBNET.ATG" 
out outExpr);
		while (la.kind == 151) {
			lexer.NextToken();
			IntegerDivisionExpr(
#line  1922 "VBNET.ATG" 
out expr);

#line  1922 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.Modulus, expr);  
		}
	}

	void IntegerDivisionExpr(
#line  1925 "VBNET.ATG" 
out Expression outExpr) {

#line  1926 "VBNET.ATG" 
		Expression expr; 
		MultiplicativeExpr(
#line  1928 "VBNET.ATG" 
out outExpr);
		while (la.kind == 24) {
			lexer.NextToken();
			MultiplicativeExpr(
#line  1928 "VBNET.ATG" 
out expr);

#line  1928 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.DivideInteger, expr);  
		}
	}

	void MultiplicativeExpr(
#line  1931 "VBNET.ATG" 
out Expression outExpr) {

#line  1933 "VBNET.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		UnaryExpr(
#line  1936 "VBNET.ATG" 
out outExpr);
		while (la.kind == 23 || la.kind == 31) {
			if (la.kind == 31) {
				lexer.NextToken();

#line  1939 "VBNET.ATG" 
				op = BinaryOperatorType.Multiply; 
			} else {
				lexer.NextToken();

#line  1940 "VBNET.ATG" 
				op = BinaryOperatorType.Divide; 
			}
			UnaryExpr(
#line  1942 "VBNET.ATG" 
out expr);

#line  1942 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr); 
		}
	}

	void UnaryExpr(
#line  1946 "VBNET.ATG" 
out Expression uExpr) {

#line  1948 "VBNET.ATG" 
		Expression expr;
		UnaryOperatorType uop = UnaryOperatorType.None;
		bool isUOp = false;
		
		while (la.kind == 27 || la.kind == 28 || la.kind == 31) {
			if (la.kind == 28) {
				lexer.NextToken();

#line  1952 "VBNET.ATG" 
				uop = UnaryOperatorType.Plus; isUOp = true; 
			} else if (la.kind == 27) {
				lexer.NextToken();

#line  1953 "VBNET.ATG" 
				uop = UnaryOperatorType.Minus; isUOp = true; 
			} else {
				lexer.NextToken();

#line  1954 "VBNET.ATG" 
				uop = UnaryOperatorType.Dereference;  isUOp = true;
			}
		}
		ExponentiationExpr(
#line  1956 "VBNET.ATG" 
out expr);

#line  1958 "VBNET.ATG" 
		if (isUOp) {
		uExpr = new UnaryOperatorExpression(expr, uop);
		} else {
			uExpr = expr;
		}
		
	}

	void ExponentiationExpr(
#line  1966 "VBNET.ATG" 
out Expression outExpr) {

#line  1967 "VBNET.ATG" 
		Expression expr; 
		SimpleExpr(
#line  1969 "VBNET.ATG" 
out outExpr);
		while (la.kind == 29) {
			lexer.NextToken();
			SimpleExpr(
#line  1969 "VBNET.ATG" 
out expr);

#line  1969 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.Power, expr);  
		}
	}

	void NormalOrReDimArgumentList(
#line  2418 "VBNET.ATG" 
out List<Expression> arguments, out bool canBeNormal, out bool canBeRedim) {

#line  2420 "VBNET.ATG" 
		arguments = new List<Expression>();
		canBeNormal = true; canBeRedim = !IsNamedAssign();
		Expression expr = null;
		
		if (StartOf(29)) {
			Argument(
#line  2425 "VBNET.ATG" 
out expr);
			if (la.kind == 212) {
				lexer.NextToken();

#line  2426 "VBNET.ATG" 
				EnsureIsZero(expr); canBeNormal = false; 
				Expr(
#line  2427 "VBNET.ATG" 
out expr);
			}
		}
		while (la.kind == 21) {
			lexer.NextToken();

#line  2430 "VBNET.ATG" 
			if (expr == null) canBeRedim = false; 

#line  2431 "VBNET.ATG" 
			arguments.Add(expr ?? Expression.Null); expr = null; 

#line  2432 "VBNET.ATG" 
			canBeRedim &= !IsNamedAssign(); 
			if (StartOf(29)) {
				Argument(
#line  2433 "VBNET.ATG" 
out expr);
				if (la.kind == 212) {
					lexer.NextToken();

#line  2434 "VBNET.ATG" 
					EnsureIsZero(expr); canBeNormal = false; 
					Expr(
#line  2435 "VBNET.ATG" 
out expr);
				}
			}

#line  2437 "VBNET.ATG" 
			if (expr == null) { canBeRedim = false; expr = Expression.Null; } 
		}

#line  2439 "VBNET.ATG" 
		if (expr != null) arguments.Add(expr); else canBeRedim = false; 
	}

	void ArrayTypeModifiers(
#line  2546 "VBNET.ATG" 
out ArrayList arrayModifiers) {

#line  2548 "VBNET.ATG" 
		arrayModifiers = new ArrayList();
		int i = 0;
		
		while (
#line  2551 "VBNET.ATG" 
IsDims()) {
			Expect(34);
			if (la.kind == 21 || la.kind == 35) {
				RankList(
#line  2553 "VBNET.ATG" 
out i);
			}

#line  2555 "VBNET.ATG" 
			arrayModifiers.Add(i);
			
			Expect(35);
		}

#line  2560 "VBNET.ATG" 
		if(arrayModifiers.Count == 0) {
		 arrayModifiers = null;
		}
		
	}

	void MemberInitializer(
#line  2385 "VBNET.ATG" 
out MemberInitializerExpression memberInitializer) {

#line  2387 "VBNET.ATG" 
		memberInitializer = new MemberInitializerExpression();
		memberInitializer.StartLocation = la.Location;
		Expression initExpr = null;
		bool isKey = false;
		string name = null;
		
		Expect(25);
		IdentifierOrKeyword(
#line  2394 "VBNET.ATG" 
out name);
		Expect(19);
		Expr(
#line  2394 "VBNET.ATG" 
out initExpr);

#line  2396 "VBNET.ATG" 
		memberInitializer.Name = name;
		memberInitializer.Expression = initExpr;
		memberInitializer.IsKey = isKey;
		memberInitializer.EndLocation = t.EndLocation;
		
	}

	void FromOrAggregateQueryOperator(
#line  2074 "VBNET.ATG" 
List<QueryExpressionClause> middleClauses) {

#line  2076 "VBNET.ATG" 
		QueryExpressionFromClause fromClause = null;
		QueryExpressionAggregateClause aggregateClause = null;
		
		if (la.kind == 123) {
			FromQueryOperator(
#line  2079 "VBNET.ATG" 
out fromClause);

#line  2080 "VBNET.ATG" 
			middleClauses.Add(fromClause); 
		} else if (la.kind == 55) {
			AggregateQueryOperator(
#line  2081 "VBNET.ATG" 
out aggregateClause);

#line  2082 "VBNET.ATG" 
			middleClauses.Add(aggregateClause); 
		} else SynErr(276);
	}

	void QueryOperator(
#line  2085 "VBNET.ATG" 
List<QueryExpressionClause> middleClauses) {

#line  2087 "VBNET.ATG" 
		QueryExpressionJoinVBClause joinClause = null;
		QueryExpressionGroupVBClause groupByClause = null;
		QueryExpressionPartitionVBClause partitionClause = null;
		QueryExpressionGroupJoinVBClause groupJoinClause = null;
		QueryExpressionFromClause fromClause = null;
		QueryExpressionAggregateClause aggregateClause = null;
		
		if (la.kind == 123) {
			FromQueryOperator(
#line  2094 "VBNET.ATG" 
out fromClause);

#line  2095 "VBNET.ATG" 
			middleClauses.Add(fromClause); 
		} else if (la.kind == 55) {
			AggregateQueryOperator(
#line  2096 "VBNET.ATG" 
out aggregateClause);

#line  2097 "VBNET.ATG" 
			middleClauses.Add(aggregateClause); 
		} else if (la.kind == 193) {
			SelectQueryOperator(
#line  2098 "VBNET.ATG" 
middleClauses);
		} else if (la.kind == 104) {
			DistinctQueryOperator(
#line  2099 "VBNET.ATG" 
middleClauses);
		} else if (la.kind == 226) {
			WhereQueryOperator(
#line  2100 "VBNET.ATG" 
middleClauses);
		} else if (la.kind == 173) {
			OrderByQueryOperator(
#line  2101 "VBNET.ATG" 
middleClauses);
		} else if (la.kind == 199 || la.kind == 208) {
			PartitionQueryOperator(
#line  2102 "VBNET.ATG" 
out partitionClause);

#line  2103 "VBNET.ATG" 
			middleClauses.Add(partitionClause); 
		} else if (la.kind == 145) {
			LetQueryOperator(
#line  2104 "VBNET.ATG" 
middleClauses);
		} else if (la.kind == 143) {
			JoinQueryOperator(
#line  2105 "VBNET.ATG" 
out joinClause);

#line  2106 "VBNET.ATG" 
			middleClauses.Add(joinClause); 
		} else if (
#line  2107 "VBNET.ATG" 
la.kind == Tokens.Group && Peek(1).kind == Tokens.Join) {
			GroupJoinQueryOperator(
#line  2107 "VBNET.ATG" 
out groupJoinClause);

#line  2108 "VBNET.ATG" 
			middleClauses.Add(groupJoinClause); 
		} else if (la.kind == 130) {
			GroupByQueryOperator(
#line  2109 "VBNET.ATG" 
out groupByClause);

#line  2110 "VBNET.ATG" 
			middleClauses.Add(groupByClause); 
		} else SynErr(277);
	}

	void FromQueryOperator(
#line  2185 "VBNET.ATG" 
out QueryExpressionFromClause fromClause) {

#line  2187 "VBNET.ATG" 
		fromClause = new QueryExpressionFromClause();
		fromClause.StartLocation = la.Location;
		
		Expect(123);
		CollectionRangeVariableDeclarationList(
#line  2190 "VBNET.ATG" 
fromClause.Sources);

#line  2192 "VBNET.ATG" 
		fromClause.EndLocation = t.EndLocation;
		
	}

	void AggregateQueryOperator(
#line  2254 "VBNET.ATG" 
out QueryExpressionAggregateClause aggregateClause) {

#line  2256 "VBNET.ATG" 
		aggregateClause = new QueryExpressionAggregateClause();
		aggregateClause.IntoVariables = new List<ExpressionRangeVariable>();
		aggregateClause.StartLocation = la.Location;
		CollectionRangeVariable source;
		
		Expect(55);
		CollectionRangeVariableDeclaration(
#line  2261 "VBNET.ATG" 
out source);

#line  2263 "VBNET.ATG" 
		aggregateClause.Source = source;
		
		while (StartOf(30)) {
			QueryOperator(
#line  2266 "VBNET.ATG" 
aggregateClause.MiddleClauses);
		}
		Expect(140);
		ExpressionRangeVariableDeclarationList(
#line  2268 "VBNET.ATG" 
aggregateClause.IntoVariables);

#line  2270 "VBNET.ATG" 
		aggregateClause.EndLocation = t.EndLocation;
		
	}

	void SelectQueryOperator(
#line  2196 "VBNET.ATG" 
List<QueryExpressionClause> middleClauses) {

#line  2198 "VBNET.ATG" 
		QueryExpressionSelectVBClause selectClause = new QueryExpressionSelectVBClause();
		selectClause.StartLocation = la.Location;
		
		Expect(193);
		ExpressionRangeVariableDeclarationList(
#line  2201 "VBNET.ATG" 
selectClause.Variables);

#line  2203 "VBNET.ATG" 
		selectClause.EndLocation = t.Location;
		middleClauses.Add(selectClause);
		
	}

	void DistinctQueryOperator(
#line  2208 "VBNET.ATG" 
List<QueryExpressionClause> middleClauses) {

#line  2210 "VBNET.ATG" 
		QueryExpressionDistinctClause distinctClause = new QueryExpressionDistinctClause();
		distinctClause.StartLocation = la.Location;
		
		Expect(104);

#line  2215 "VBNET.ATG" 
		distinctClause.EndLocation = t.EndLocation;
		middleClauses.Add(distinctClause);
		
	}

	void WhereQueryOperator(
#line  2220 "VBNET.ATG" 
List<QueryExpressionClause> middleClauses) {

#line  2222 "VBNET.ATG" 
		QueryExpressionWhereClause whereClause = new QueryExpressionWhereClause();
		whereClause.StartLocation = la.Location;
		Expression operand = null;
		
		Expect(226);
		Expr(
#line  2226 "VBNET.ATG" 
out operand);

#line  2228 "VBNET.ATG" 
		whereClause.Condition = operand;
		whereClause.EndLocation = t.EndLocation;
		
		middleClauses.Add(whereClause);
		
	}

	void OrderByQueryOperator(
#line  2113 "VBNET.ATG" 
List<QueryExpressionClause> middleClauses) {

#line  2115 "VBNET.ATG" 
		QueryExpressionOrderClause orderClause = new QueryExpressionOrderClause();
		orderClause.StartLocation = la.Location;
		List<QueryExpressionOrdering> orderings = null;
		
		Expect(173);
		Expect(67);
		OrderExpressionList(
#line  2119 "VBNET.ATG" 
out orderings);

#line  2121 "VBNET.ATG" 
		orderClause.Orderings = orderings;
		orderClause.EndLocation = t.EndLocation;
		middleClauses.Add(orderClause);
		
	}

	void PartitionQueryOperator(
#line  2235 "VBNET.ATG" 
out QueryExpressionPartitionVBClause partitionClause) {

#line  2237 "VBNET.ATG" 
		partitionClause = new QueryExpressionPartitionVBClause();
		partitionClause.StartLocation = la.Location;
		Expression expr = null;
		
		if (la.kind == 208) {
			lexer.NextToken();

#line  2242 "VBNET.ATG" 
			partitionClause.PartitionType = QueryExpressionPartitionType.Take; 
			if (la.kind == 227) {
				lexer.NextToken();

#line  2243 "VBNET.ATG" 
				partitionClause.PartitionType = QueryExpressionPartitionType.TakeWhile; 
			}
		} else if (la.kind == 199) {
			lexer.NextToken();

#line  2244 "VBNET.ATG" 
			partitionClause.PartitionType = QueryExpressionPartitionType.Skip; 
			if (la.kind == 227) {
				lexer.NextToken();

#line  2245 "VBNET.ATG" 
				partitionClause.PartitionType = QueryExpressionPartitionType.SkipWhile; 
			}
		} else SynErr(278);
		Expr(
#line  2247 "VBNET.ATG" 
out expr);

#line  2249 "VBNET.ATG" 
		partitionClause.Expression = expr;
		partitionClause.EndLocation = t.EndLocation;
		
	}

	void LetQueryOperator(
#line  2274 "VBNET.ATG" 
List<QueryExpressionClause> middleClauses) {

#line  2276 "VBNET.ATG" 
		QueryExpressionLetVBClause letClause = new QueryExpressionLetVBClause();
		letClause.StartLocation = la.Location;
		
		Expect(145);
		ExpressionRangeVariableDeclarationList(
#line  2279 "VBNET.ATG" 
letClause.Variables);

#line  2281 "VBNET.ATG" 
		letClause.EndLocation = t.EndLocation;
		middleClauses.Add(letClause);
		
	}

	void JoinQueryOperator(
#line  2318 "VBNET.ATG" 
out QueryExpressionJoinVBClause joinClause) {

#line  2320 "VBNET.ATG" 
		joinClause = new QueryExpressionJoinVBClause();
		joinClause.StartLocation = la.Location;
		CollectionRangeVariable joinVariable = null;
		QueryExpressionJoinVBClause subJoin = null;
		QueryExpressionJoinConditionVB condition = null;
		
		
		Expect(143);
		CollectionRangeVariableDeclaration(
#line  2327 "VBNET.ATG" 
out joinVariable);

#line  2328 "VBNET.ATG" 
		joinClause.JoinVariable = joinVariable; 
		if (la.kind == 143) {
			JoinQueryOperator(
#line  2330 "VBNET.ATG" 
out subJoin);

#line  2331 "VBNET.ATG" 
			joinClause.SubJoin = subJoin; 
		}
		Expect(168);
		JoinCondition(
#line  2334 "VBNET.ATG" 
out condition);

#line  2335 "VBNET.ATG" 
		SafeAdd(joinClause, joinClause.Conditions, condition); 
		while (la.kind == 57) {
			lexer.NextToken();
			JoinCondition(
#line  2337 "VBNET.ATG" 
out condition);

#line  2338 "VBNET.ATG" 
			SafeAdd(joinClause, joinClause.Conditions, condition); 
		}

#line  2341 "VBNET.ATG" 
		joinClause.EndLocation = t.EndLocation;
		
	}

	void GroupJoinQueryOperator(
#line  2171 "VBNET.ATG" 
out QueryExpressionGroupJoinVBClause groupJoinClause) {

#line  2173 "VBNET.ATG" 
		groupJoinClause = new QueryExpressionGroupJoinVBClause();
		groupJoinClause.StartLocation = la.Location;
		QueryExpressionJoinVBClause joinClause = null;
		
		Expect(130);
		JoinQueryOperator(
#line  2177 "VBNET.ATG" 
out joinClause);
		Expect(140);
		ExpressionRangeVariableDeclarationList(
#line  2178 "VBNET.ATG" 
groupJoinClause.IntoVariables);

#line  2180 "VBNET.ATG" 
		groupJoinClause.JoinClause = joinClause;
		groupJoinClause.EndLocation = t.EndLocation;
		
	}

	void GroupByQueryOperator(
#line  2158 "VBNET.ATG" 
out QueryExpressionGroupVBClause groupByClause) {

#line  2160 "VBNET.ATG" 
		groupByClause = new QueryExpressionGroupVBClause();
		groupByClause.StartLocation = la.Location;
		
		Expect(130);
		ExpressionRangeVariableDeclarationList(
#line  2163 "VBNET.ATG" 
groupByClause.GroupVariables);
		Expect(67);
		ExpressionRangeVariableDeclarationList(
#line  2164 "VBNET.ATG" 
groupByClause.ByVariables);
		Expect(140);
		ExpressionRangeVariableDeclarationList(
#line  2165 "VBNET.ATG" 
groupByClause.IntoVariables);

#line  2167 "VBNET.ATG" 
		groupByClause.EndLocation = t.EndLocation;
		
	}

	void OrderExpressionList(
#line  2127 "VBNET.ATG" 
out List<QueryExpressionOrdering> orderings) {

#line  2129 "VBNET.ATG" 
		orderings = new List<QueryExpressionOrdering>();
		QueryExpressionOrdering ordering = null;
		
		OrderExpression(
#line  2132 "VBNET.ATG" 
out ordering);

#line  2133 "VBNET.ATG" 
		orderings.Add(ordering); 
		while (la.kind == 21) {
			lexer.NextToken();
			OrderExpression(
#line  2135 "VBNET.ATG" 
out ordering);

#line  2136 "VBNET.ATG" 
			orderings.Add(ordering); 
		}
	}

	void OrderExpression(
#line  2140 "VBNET.ATG" 
out QueryExpressionOrdering ordering) {

#line  2142 "VBNET.ATG" 
		ordering = new QueryExpressionOrdering();
		ordering.StartLocation = la.Location;
		ordering.Direction = QueryExpressionOrderingDirection.None;
		Expression orderExpr = null;
		
		Expr(
#line  2147 "VBNET.ATG" 
out orderExpr);

#line  2149 "VBNET.ATG" 
		ordering.Criteria = orderExpr;
		
		if (la.kind == 61 || la.kind == 101) {
			if (la.kind == 61) {
				lexer.NextToken();

#line  2152 "VBNET.ATG" 
				ordering.Direction = QueryExpressionOrderingDirection.Ascending; 
			} else {
				lexer.NextToken();

#line  2153 "VBNET.ATG" 
				ordering.Direction = QueryExpressionOrderingDirection.Descending; 
			}
		}

#line  2155 "VBNET.ATG" 
		ordering.EndLocation = t.EndLocation; 
	}

	void ExpressionRangeVariableDeclarationList(
#line  2286 "VBNET.ATG" 
List<ExpressionRangeVariable> variables) {

#line  2288 "VBNET.ATG" 
		ExpressionRangeVariable variable = null;
		
		ExpressionRangeVariableDeclaration(
#line  2290 "VBNET.ATG" 
out variable);

#line  2291 "VBNET.ATG" 
		variables.Add(variable); 
		while (la.kind == 21) {
			lexer.NextToken();
			ExpressionRangeVariableDeclaration(
#line  2292 "VBNET.ATG" 
out variable);

#line  2292 "VBNET.ATG" 
			variables.Add(variable); 
		}
	}

	void CollectionRangeVariableDeclarationList(
#line  2345 "VBNET.ATG" 
List<CollectionRangeVariable> rangeVariables) {

#line  2346 "VBNET.ATG" 
		CollectionRangeVariable variableDeclaration; 
		CollectionRangeVariableDeclaration(
#line  2348 "VBNET.ATG" 
out variableDeclaration);

#line  2349 "VBNET.ATG" 
		rangeVariables.Add(variableDeclaration); 
		while (la.kind == 21) {
			lexer.NextToken();
			CollectionRangeVariableDeclaration(
#line  2350 "VBNET.ATG" 
out variableDeclaration);

#line  2350 "VBNET.ATG" 
			rangeVariables.Add(variableDeclaration); 
		}
	}

	void CollectionRangeVariableDeclaration(
#line  2353 "VBNET.ATG" 
out CollectionRangeVariable rangeVariable) {

#line  2355 "VBNET.ATG" 
		rangeVariable = new CollectionRangeVariable();
		rangeVariable.StartLocation = la.Location;
		TypeReference typeName = null;
		Expression inExpr = null;
		
		Identifier();

#line  2360 "VBNET.ATG" 
		rangeVariable.Identifier = t.val; 
		if (la.kind == 60) {
			lexer.NextToken();
			TypeName(
#line  2361 "VBNET.ATG" 
out typeName);

#line  2361 "VBNET.ATG" 
			rangeVariable.Type = typeName; 
		}
		Expect(135);
		Expr(
#line  2362 "VBNET.ATG" 
out inExpr);

#line  2364 "VBNET.ATG" 
		rangeVariable.Expression = inExpr;
		rangeVariable.EndLocation = t.EndLocation;
		
	}

	void ExpressionRangeVariableDeclaration(
#line  2295 "VBNET.ATG" 
out ExpressionRangeVariable variable) {

#line  2297 "VBNET.ATG" 
		variable = new ExpressionRangeVariable();
		variable.StartLocation = la.Location;
		Expression rhs = null;
		TypeReference typeName = null;
		
		if (
#line  2303 "VBNET.ATG" 
IsIdentifiedExpressionRange()) {
			Identifier();

#line  2304 "VBNET.ATG" 
			variable.Identifier = t.val; 
			if (la.kind == 60) {
				lexer.NextToken();
				TypeName(
#line  2306 "VBNET.ATG" 
out typeName);

#line  2307 "VBNET.ATG" 
				variable.Type = typeName; 
			}
			Expect(19);
		}
		Expr(
#line  2311 "VBNET.ATG" 
out rhs);

#line  2313 "VBNET.ATG" 
		variable.Expression = rhs;
		variable.EndLocation = t.EndLocation;
		
	}

	void JoinCondition(
#line  2369 "VBNET.ATG" 
out QueryExpressionJoinConditionVB condition) {

#line  2371 "VBNET.ATG" 
		condition = new QueryExpressionJoinConditionVB();
		condition.StartLocation = la.Location;
		
		Expression lhs = null;
		Expression rhs = null;
		
		Expr(
#line  2377 "VBNET.ATG" 
out lhs);
		Expect(113);
		Expr(
#line  2377 "VBNET.ATG" 
out rhs);

#line  2379 "VBNET.ATG" 
		condition.LeftSide = lhs;
		condition.RightSide = rhs;
		condition.EndLocation = t.EndLocation;
		
	}

	void Argument(
#line  2443 "VBNET.ATG" 
out Expression argumentexpr) {

#line  2445 "VBNET.ATG" 
		Expression expr;
		argumentexpr = null;
		string name;
		
		if (
#line  2449 "VBNET.ATG" 
IsNamedAssign()) {
			Identifier();

#line  2449 "VBNET.ATG" 
			name = t.val;  
			Expect(52);
			Expr(
#line  2449 "VBNET.ATG" 
out expr);

#line  2451 "VBNET.ATG" 
			argumentexpr = new NamedArgumentExpression(name, expr);
			
		} else if (StartOf(29)) {
			Expr(
#line  2454 "VBNET.ATG" 
out argumentexpr);
		} else SynErr(279);
	}

	void QualIdentAndTypeArguments(
#line  2520 "VBNET.ATG" 
out TypeReference typeref, bool canBeUnbound) {

#line  2521 "VBNET.ATG" 
		string name; typeref = null; 
		Qualident(
#line  2523 "VBNET.ATG" 
out name);

#line  2524 "VBNET.ATG" 
		typeref = new TypeReference(name); 
		if (
#line  2525 "VBNET.ATG" 
la.kind == Tokens.OpenParenthesis && Peek(1).kind == Tokens.Of) {
			lexer.NextToken();
			Expect(166);
			if (
#line  2527 "VBNET.ATG" 
canBeUnbound && (la.kind == Tokens.CloseParenthesis || la.kind == Tokens.Comma)) {

#line  2528 "VBNET.ATG" 
				typeref.GenericTypes.Add(NullTypeReference.Instance); 
				while (la.kind == 21) {
					lexer.NextToken();

#line  2529 "VBNET.ATG" 
					typeref.GenericTypes.Add(NullTypeReference.Instance); 
				}
			} else if (StartOf(8)) {
				TypeArgumentList(
#line  2530 "VBNET.ATG" 
typeref.GenericTypes);
			} else SynErr(280);
			Expect(35);
		}
	}

	void RankList(
#line  2567 "VBNET.ATG" 
out int i) {

#line  2568 "VBNET.ATG" 
		i = 0; 
		while (la.kind == 21) {
			lexer.NextToken();

#line  2569 "VBNET.ATG" 
			++i; 
		}
	}

	void Attribute(
#line  2608 "VBNET.ATG" 
out ASTAttribute attribute) {

#line  2609 "VBNET.ATG" 
		string name;
		List<Expression> positional = new List<Expression>();
		List<NamedArgumentExpression> named = new List<NamedArgumentExpression>();
		
		if (la.kind == 127) {
			lexer.NextToken();
			Expect(25);
		}
		Qualident(
#line  2614 "VBNET.ATG" 
out name);
		if (la.kind == 34) {
			AttributeArguments(
#line  2615 "VBNET.ATG" 
positional, named);
		}

#line  2617 "VBNET.ATG" 
		attribute  = new ASTAttribute(name, positional, named);
		
	}

	void AttributeArguments(
#line  2622 "VBNET.ATG" 
List<Expression> positional, List<NamedArgumentExpression> named) {

#line  2624 "VBNET.ATG" 
		bool nameFound = false;
		string name = "";
		Expression expr;
		
		Expect(34);
		if (
#line  2630 "VBNET.ATG" 
IsNotClosingParenthesis()) {
			if (
#line  2632 "VBNET.ATG" 
IsNamedAssign()) {

#line  2632 "VBNET.ATG" 
				nameFound = true; 
				IdentifierOrKeyword(
#line  2633 "VBNET.ATG" 
out name);
				if (la.kind == 52) {
					lexer.NextToken();
				} else if (la.kind == 19) {
					lexer.NextToken();
				} else SynErr(281);
			}
			Expr(
#line  2635 "VBNET.ATG" 
out expr);

#line  2637 "VBNET.ATG" 
			if (expr != null) {
			if (string.IsNullOrEmpty(name)) { positional.Add(expr); }
			else { named.Add(new NamedArgumentExpression(name, expr)); name = ""; }
			}
			
			while (la.kind == 21) {
				lexer.NextToken();
				if (
#line  2645 "VBNET.ATG" 
IsNamedAssign()) {

#line  2645 "VBNET.ATG" 
					nameFound = true; 
					IdentifierOrKeyword(
#line  2646 "VBNET.ATG" 
out name);
					if (la.kind == 52) {
						lexer.NextToken();
					} else if (la.kind == 19) {
						lexer.NextToken();
					} else SynErr(282);
				} else if (StartOf(29)) {

#line  2648 "VBNET.ATG" 
					if (nameFound) Error("no positional argument after named argument"); 
				} else SynErr(283);
				Expr(
#line  2649 "VBNET.ATG" 
out expr);

#line  2649 "VBNET.ATG" 
				if (expr != null) { if(name == "") positional.Add(expr);
				else { named.Add(new NamedArgumentExpression(name, expr)); name = ""; }
				}
				
			}
		}
		Expect(35);
	}

	void FormalParameter(
#line  2706 "VBNET.ATG" 
out ParameterDeclarationExpression p) {

#line  2708 "VBNET.ATG" 
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		TypeReference type = null;
		ParamModifierList mod = new ParamModifierList(this);
		Expression expr = null;
		p = null;
		ArrayList arrayModifiers = null;
		
		while (la.kind == 37) {
			AttributeSection(
#line  2717 "VBNET.ATG" 
out section);

#line  2717 "VBNET.ATG" 
			attributes.Add(section); 
		}
		while (StartOf(34)) {
			ParameterModifier(
#line  2718 "VBNET.ATG" 
mod);
		}
		Identifier();

#line  2719 "VBNET.ATG" 
		string parameterName = t.val; 
		if (
#line  2720 "VBNET.ATG" 
IsDims()) {
			ArrayTypeModifiers(
#line  2720 "VBNET.ATG" 
out arrayModifiers);
		}
		if (la.kind == 60) {
			lexer.NextToken();
			TypeName(
#line  2721 "VBNET.ATG" 
out type);
		}

#line  2723 "VBNET.ATG" 
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
		
		if (la.kind == 19) {
			lexer.NextToken();
			Expr(
#line  2735 "VBNET.ATG" 
out expr);
		}

#line  2737 "VBNET.ATG" 
		mod.Check();
		p = new ParameterDeclarationExpression(type, parameterName, mod.Modifier, expr);
		p.Attributes = attributes;
		
	}

	void ParameterModifier(
#line  3419 "VBNET.ATG" 
ParamModifierList m) {
		if (la.kind == 69) {
			lexer.NextToken();

#line  3420 "VBNET.ATG" 
			m.Add(ParameterModifiers.In); 
		} else if (la.kind == 66) {
			lexer.NextToken();

#line  3421 "VBNET.ATG" 
			m.Add(ParameterModifiers.Ref); 
		} else if (la.kind == 171) {
			lexer.NextToken();

#line  3422 "VBNET.ATG" 
			m.Add(ParameterModifiers.Optional); 
		} else if (la.kind == 178) {
			lexer.NextToken();

#line  3423 "VBNET.ATG" 
			m.Add(ParameterModifiers.Params); 
		} else SynErr(284);
	}

	void Statement() {

#line  2766 "VBNET.ATG" 
		Statement stmt = null;
		Location startPos = la.Location;
		string label = String.Empty;
		
		
		if (la.kind == 1 || la.kind == 20) {
		} else if (
#line  2772 "VBNET.ATG" 
IsLabel()) {
			LabelName(
#line  2772 "VBNET.ATG" 
out label);

#line  2774 "VBNET.ATG" 
			compilationUnit.AddChild(new LabelStatement(t.val));
			
			Expect(20);
			Statement();
		} else if (StartOf(35)) {
			EmbeddedStatement(
#line  2777 "VBNET.ATG" 
out stmt);

#line  2777 "VBNET.ATG" 
			compilationUnit.AddChild(stmt); 
		} else SynErr(285);

#line  2780 "VBNET.ATG" 
		if (stmt != null) {
		stmt.StartLocation = startPos;
		stmt.EndLocation = t.Location;
		}
		
	}

	void LabelName(
#line  3195 "VBNET.ATG" 
out string name) {

#line  3197 "VBNET.ATG" 
		name = String.Empty;
		
		if (StartOf(4)) {
			Identifier();

#line  3199 "VBNET.ATG" 
			name = t.val; 
		} else if (la.kind == 5) {
			lexer.NextToken();

#line  3200 "VBNET.ATG" 
			name = t.val; 
		} else SynErr(286);
	}

	void EmbeddedStatement(
#line  2819 "VBNET.ATG" 
out Statement statement) {

#line  2821 "VBNET.ATG" 
		Statement embeddedStatement = null;
		statement = null;
		Expression expr = null;
		string name = String.Empty;
		List<Expression> p = null;
		
		if (la.kind == 117) {
			lexer.NextToken();

#line  2827 "VBNET.ATG" 
			ExitType exitType = ExitType.None; 
			switch (la.kind) {
			case 206: {
				lexer.NextToken();

#line  2829 "VBNET.ATG" 
				exitType = ExitType.Sub; 
				break;
			}
			case 124: {
				lexer.NextToken();

#line  2831 "VBNET.ATG" 
				exitType = ExitType.Function; 
				break;
			}
			case 182: {
				lexer.NextToken();

#line  2833 "VBNET.ATG" 
				exitType = ExitType.Property; 
				break;
			}
			case 105: {
				lexer.NextToken();

#line  2835 "VBNET.ATG" 
				exitType = ExitType.Do; 
				break;
			}
			case 121: {
				lexer.NextToken();

#line  2837 "VBNET.ATG" 
				exitType = ExitType.For; 
				break;
			}
			case 214: {
				lexer.NextToken();

#line  2839 "VBNET.ATG" 
				exitType = ExitType.Try; 
				break;
			}
			case 227: {
				lexer.NextToken();

#line  2841 "VBNET.ATG" 
				exitType = ExitType.While; 
				break;
			}
			case 193: {
				lexer.NextToken();

#line  2843 "VBNET.ATG" 
				exitType = ExitType.Select; 
				break;
			}
			default: SynErr(287); break;
			}

#line  2845 "VBNET.ATG" 
			statement = new ExitStatement(exitType); 
		} else if (la.kind == 214) {
			TryStatement(
#line  2846 "VBNET.ATG" 
out statement);
		} else if (la.kind == 86) {
			lexer.NextToken();

#line  2847 "VBNET.ATG" 
			ContinueType continueType = ContinueType.None; 
			if (la.kind == 105 || la.kind == 121 || la.kind == 227) {
				if (la.kind == 105) {
					lexer.NextToken();

#line  2847 "VBNET.ATG" 
					continueType = ContinueType.Do; 
				} else if (la.kind == 121) {
					lexer.NextToken();

#line  2847 "VBNET.ATG" 
					continueType = ContinueType.For; 
				} else {
					lexer.NextToken();

#line  2847 "VBNET.ATG" 
					continueType = ContinueType.While; 
				}
			}

#line  2847 "VBNET.ATG" 
			statement = new ContinueStatement(continueType); 
		} else if (la.kind == 211) {
			lexer.NextToken();
			if (StartOf(29)) {
				Expr(
#line  2849 "VBNET.ATG" 
out expr);
			}

#line  2849 "VBNET.ATG" 
			statement = new ThrowStatement(expr); 
		} else if (la.kind == 191) {
			lexer.NextToken();
			if (StartOf(29)) {
				Expr(
#line  2851 "VBNET.ATG" 
out expr);
			}

#line  2851 "VBNET.ATG" 
			statement = new ReturnStatement(expr); 
		} else if (la.kind == 207) {
			lexer.NextToken();
			Expr(
#line  2853 "VBNET.ATG" 
out expr);
			EndOfStmt();
			Block(
#line  2853 "VBNET.ATG" 
out embeddedStatement);
			Expect(110);
			Expect(207);

#line  2854 "VBNET.ATG" 
			statement = new LockStatement(expr, embeddedStatement); 
		} else if (la.kind == 185) {
			lexer.NextToken();
			Identifier();

#line  2856 "VBNET.ATG" 
			name = t.val; 
			if (la.kind == 34) {
				lexer.NextToken();
				if (StartOf(36)) {
					ArgumentList(
#line  2857 "VBNET.ATG" 
out p);
				}
				Expect(35);
			}

#line  2859 "VBNET.ATG" 
			statement = new RaiseEventStatement(name, p);
			
		} else if (la.kind == 229) {
			WithStatement(
#line  2862 "VBNET.ATG" 
out statement);
		} else if (la.kind == 53) {
			lexer.NextToken();

#line  2864 "VBNET.ATG" 
			Expression handlerExpr = null; 
			Expr(
#line  2865 "VBNET.ATG" 
out expr);
			Expect(21);
			Expr(
#line  2865 "VBNET.ATG" 
out handlerExpr);

#line  2867 "VBNET.ATG" 
			statement = new AddHandlerStatement(expr, handlerExpr);
			
		} else if (la.kind == 189) {
			lexer.NextToken();

#line  2870 "VBNET.ATG" 
			Expression handlerExpr = null; 
			Expr(
#line  2871 "VBNET.ATG" 
out expr);
			Expect(21);
			Expr(
#line  2871 "VBNET.ATG" 
out handlerExpr);

#line  2873 "VBNET.ATG" 
			statement = new RemoveHandlerStatement(expr, handlerExpr);
			
		} else if (la.kind == 227) {
			lexer.NextToken();
			Expr(
#line  2876 "VBNET.ATG" 
out expr);
			EndOfStmt();
			Block(
#line  2877 "VBNET.ATG" 
out embeddedStatement);
			Expect(110);
			Expect(227);

#line  2879 "VBNET.ATG" 
			statement = new DoLoopStatement(expr, embeddedStatement, ConditionType.While, ConditionPosition.Start);
			
		} else if (la.kind == 105) {
			lexer.NextToken();

#line  2884 "VBNET.ATG" 
			ConditionType conditionType = ConditionType.None;
			
			if (la.kind == 220 || la.kind == 227) {
				WhileOrUntil(
#line  2887 "VBNET.ATG" 
out conditionType);
				Expr(
#line  2887 "VBNET.ATG" 
out expr);
				EndOfStmt();
				Block(
#line  2888 "VBNET.ATG" 
out embeddedStatement);
				Expect(149);

#line  2891 "VBNET.ATG" 
				statement = new DoLoopStatement(expr, 
				                               embeddedStatement, 
				                               conditionType == ConditionType.While ? ConditionType.DoWhile : conditionType, 
				                               ConditionPosition.Start);
				
			} else if (la.kind == 1 || la.kind == 20) {
				EndOfStmt();
				Block(
#line  2898 "VBNET.ATG" 
out embeddedStatement);
				Expect(149);
				if (la.kind == 220 || la.kind == 227) {
					WhileOrUntil(
#line  2899 "VBNET.ATG" 
out conditionType);
					Expr(
#line  2899 "VBNET.ATG" 
out expr);
				}

#line  2901 "VBNET.ATG" 
				statement = new DoLoopStatement(expr, embeddedStatement, conditionType, ConditionPosition.End);
				
			} else SynErr(288);
		} else if (la.kind == 121) {
			lexer.NextToken();

#line  2906 "VBNET.ATG" 
			Expression group = null;
			TypeReference typeReference;
			string        typeName;
			Location startLocation = t.Location;
			
			if (la.kind == 107) {
				lexer.NextToken();
				LoopControlVariable(
#line  2913 "VBNET.ATG" 
out typeReference, out typeName);
				Expect(135);
				Expr(
#line  2914 "VBNET.ATG" 
out group);
				EndOfStmt();
				Block(
#line  2915 "VBNET.ATG" 
out embeddedStatement);
				Expect(160);
				if (StartOf(29)) {
					Expr(
#line  2916 "VBNET.ATG" 
out expr);
				}

#line  2918 "VBNET.ATG" 
				statement = new ForeachStatement(typeReference, 
				                                typeName,
				                                group, 
				                                embeddedStatement, 
				                                expr);
				statement.StartLocation = startLocation;
				statement.EndLocation   = t.EndLocation;
				
				
			} else if (StartOf(37)) {

#line  2929 "VBNET.ATG" 
				Expression start = null;
				Expression end = null;
				Expression step = null;
				Expression variableExpr = null;
				Expression nextExpr = null;
				List<Expression> nextExpressions = null;
				
				if (
#line  2936 "VBNET.ATG" 
IsLoopVariableDeclaration()) {
					LoopControlVariable(
#line  2937 "VBNET.ATG" 
out typeReference, out typeName);
				} else {

#line  2939 "VBNET.ATG" 
					typeReference = null; typeName = null; 
					SimpleExpr(
#line  2940 "VBNET.ATG" 
out variableExpr);
				}
				Expect(19);
				Expr(
#line  2942 "VBNET.ATG" 
out start);
				Expect(212);
				Expr(
#line  2942 "VBNET.ATG" 
out end);
				if (la.kind == 201) {
					lexer.NextToken();
					Expr(
#line  2942 "VBNET.ATG" 
out step);
				}
				EndOfStmt();
				Block(
#line  2943 "VBNET.ATG" 
out embeddedStatement);
				Expect(160);
				if (StartOf(29)) {
					Expr(
#line  2946 "VBNET.ATG" 
out nextExpr);

#line  2948 "VBNET.ATG" 
					nextExpressions = new List<Expression>();
					nextExpressions.Add(nextExpr);
					
					while (la.kind == 21) {
						lexer.NextToken();
						Expr(
#line  2951 "VBNET.ATG" 
out nextExpr);

#line  2951 "VBNET.ATG" 
						nextExpressions.Add(nextExpr); 
					}
				}

#line  2954 "VBNET.ATG" 
				statement = new ForNextStatement {
				TypeReference = typeReference,
				VariableName = typeName, 
				LoopVariableExpression = variableExpr,
				Start = start, 
				End = end, 
				Step = step, 
				EmbeddedStatement = embeddedStatement, 
				NextExpressions = nextExpressions
				};
				
			} else SynErr(289);
		} else if (la.kind == 115) {
			lexer.NextToken();
			Expr(
#line  2967 "VBNET.ATG" 
out expr);

#line  2967 "VBNET.ATG" 
			statement = new ErrorStatement(expr); 
		} else if (la.kind == 187) {
			lexer.NextToken();

#line  2969 "VBNET.ATG" 
			bool isPreserve = false; 
			if (la.kind == 180) {
				lexer.NextToken();

#line  2969 "VBNET.ATG" 
				isPreserve = true; 
			}
			ReDimClause(
#line  2970 "VBNET.ATG" 
out expr);

#line  2972 "VBNET.ATG" 
			ReDimStatement reDimStatement = new ReDimStatement(isPreserve);
			statement = reDimStatement;
			SafeAdd(reDimStatement, reDimStatement.ReDimClauses, expr as InvocationExpression);
			
			while (la.kind == 21) {
				lexer.NextToken();
				ReDimClause(
#line  2976 "VBNET.ATG" 
out expr);

#line  2977 "VBNET.ATG" 
				SafeAdd(reDimStatement, reDimStatement.ReDimClauses, expr as InvocationExpression); 
			}
		} else if (la.kind == 114) {
			lexer.NextToken();
			Expr(
#line  2981 "VBNET.ATG" 
out expr);

#line  2983 "VBNET.ATG" 
			EraseStatement eraseStatement = new EraseStatement();
			if (expr != null) { SafeAdd(eraseStatement, eraseStatement.Expressions, expr);}
			
			while (la.kind == 21) {
				lexer.NextToken();
				Expr(
#line  2986 "VBNET.ATG" 
out expr);

#line  2986 "VBNET.ATG" 
				if (expr != null) { SafeAdd(eraseStatement, eraseStatement.Expressions, expr); }
			}

#line  2987 "VBNET.ATG" 
			statement = eraseStatement; 
		} else if (la.kind == 202) {
			lexer.NextToken();

#line  2989 "VBNET.ATG" 
			statement = new StopStatement(); 
		} else if (
#line  2991 "VBNET.ATG" 
la.kind == Tokens.If) {
			Expect(132);

#line  2992 "VBNET.ATG" 
			Location ifStartLocation = t.Location; 
			Expr(
#line  2992 "VBNET.ATG" 
out expr);
			if (la.kind == 210) {
				lexer.NextToken();
			}
			if (la.kind == 1 || la.kind == 20) {
				EndOfStmt();
				Block(
#line  2995 "VBNET.ATG" 
out embeddedStatement);

#line  2997 "VBNET.ATG" 
				IfElseStatement ifStatement = new IfElseStatement(expr, embeddedStatement);
				ifStatement.StartLocation = ifStartLocation;
				Location elseIfStart;
				
				while (la.kind == 109 || 
#line  3003 "VBNET.ATG" 
IsElseIf()) {
					if (
#line  3003 "VBNET.ATG" 
IsElseIf()) {
						Expect(108);

#line  3003 "VBNET.ATG" 
						elseIfStart = t.Location; 
						Expect(132);
					} else {
						lexer.NextToken();

#line  3004 "VBNET.ATG" 
						elseIfStart = t.Location; 
					}

#line  3006 "VBNET.ATG" 
					Expression condition = null; Statement block = null; 
					Expr(
#line  3007 "VBNET.ATG" 
out condition);
					if (la.kind == 210) {
						lexer.NextToken();
					}
					EndOfStmt();
					Block(
#line  3008 "VBNET.ATG" 
out block);

#line  3010 "VBNET.ATG" 
					ElseIfSection elseIfSection = new ElseIfSection(condition, block);
					elseIfSection.StartLocation = elseIfStart;
					elseIfSection.EndLocation = t.Location;
					elseIfSection.Parent = ifStatement;
					ifStatement.ElseIfSections.Add(elseIfSection);
					
				}
				if (la.kind == 108) {
					lexer.NextToken();
					if (la.kind == 1 || la.kind == 20) {
						EndOfStmt();
					}
					Block(
#line  3019 "VBNET.ATG" 
out embeddedStatement);

#line  3021 "VBNET.ATG" 
					ifStatement.FalseStatement.Add(embeddedStatement);
					
				}
				Expect(110);
				Expect(132);

#line  3025 "VBNET.ATG" 
				ifStatement.EndLocation = t.Location;
				statement = ifStatement;
				
			} else if (StartOf(38)) {

#line  3030 "VBNET.ATG" 
				IfElseStatement ifStatement = new IfElseStatement(expr);
				ifStatement.StartLocation = ifStartLocation;
				
				SingleLineStatementList(
#line  3033 "VBNET.ATG" 
ifStatement.TrueStatement);
				if (la.kind == 108) {
					lexer.NextToken();
					if (StartOf(38)) {
						SingleLineStatementList(
#line  3036 "VBNET.ATG" 
ifStatement.FalseStatement);
					}
				}

#line  3038 "VBNET.ATG" 
				ifStatement.EndLocation = t.Location; statement = ifStatement; 
			} else SynErr(290);
		} else if (la.kind == 193) {
			lexer.NextToken();
			if (la.kind == 71) {
				lexer.NextToken();
			}
			Expr(
#line  3041 "VBNET.ATG" 
out expr);
			EndOfStmt();

#line  3042 "VBNET.ATG" 
			List<SwitchSection> selectSections = new List<SwitchSection>();
			Statement block = null;
			
			while (la.kind == 71) {

#line  3046 "VBNET.ATG" 
				List<CaseLabel> caseClauses = null; Location caseLocation = la.Location; 
				lexer.NextToken();
				CaseClauses(
#line  3047 "VBNET.ATG" 
out caseClauses);
				if (
#line  3047 "VBNET.ATG" 
IsNotStatementSeparator()) {
					lexer.NextToken();
				}
				EndOfStmt();

#line  3049 "VBNET.ATG" 
				SwitchSection selectSection = new SwitchSection(caseClauses);
				selectSection.StartLocation = caseLocation;
				
				Block(
#line  3052 "VBNET.ATG" 
out block);

#line  3054 "VBNET.ATG" 
				selectSection.Children = block.Children;
				selectSection.EndLocation = t.EndLocation;
				selectSections.Add(selectSection);
				
			}

#line  3060 "VBNET.ATG" 
			statement = new SwitchStatement(expr, selectSections);
			
			Expect(110);
			Expect(193);
		} else if (la.kind == 168) {

#line  3063 "VBNET.ATG" 
			OnErrorStatement onErrorStatement = null; 
			OnErrorStatement(
#line  3064 "VBNET.ATG" 
out onErrorStatement);

#line  3064 "VBNET.ATG" 
			statement = onErrorStatement; 
		} else if (la.kind == 129) {

#line  3065 "VBNET.ATG" 
			GotoStatement goToStatement = null; 
			GotoStatement(
#line  3066 "VBNET.ATG" 
out goToStatement);

#line  3066 "VBNET.ATG" 
			statement = goToStatement; 
		} else if (la.kind == 190) {

#line  3067 "VBNET.ATG" 
			ResumeStatement resumeStatement = null; 
			ResumeStatement(
#line  3068 "VBNET.ATG" 
out resumeStatement);

#line  3068 "VBNET.ATG" 
			statement = resumeStatement; 
		} else if (StartOf(37)) {

#line  3071 "VBNET.ATG" 
			Expression val = null;
			AssignmentOperatorType op;
			
			bool mustBeAssignment = la.kind == Tokens.Plus  || la.kind == Tokens.Minus ||
			                        la.kind == Tokens.Not   || la.kind == Tokens.Times;
			
			SimpleExpr(
#line  3077 "VBNET.ATG" 
out expr);
			if (StartOf(39)) {
				AssignmentOperator(
#line  3079 "VBNET.ATG" 
out op);
				Expr(
#line  3079 "VBNET.ATG" 
out val);

#line  3079 "VBNET.ATG" 
				expr = new AssignmentExpression(expr, op, val); 
			} else if (la.kind == 1 || la.kind == 20 || la.kind == 108) {

#line  3080 "VBNET.ATG" 
				if (mustBeAssignment) Error("error in assignment."); 
			} else SynErr(291);

#line  3083 "VBNET.ATG" 
			// a field reference expression that stands alone is a
			// invocation expression without parantheses and arguments
			if(expr is MemberReferenceExpression || expr is IdentifierExpression) {
				expr = new InvocationExpression(expr);
			}
			statement = new ExpressionStatement(expr);
			
		} else if (la.kind == 70) {
			lexer.NextToken();
			SimpleExpr(
#line  3090 "VBNET.ATG" 
out expr);

#line  3090 "VBNET.ATG" 
			statement = new ExpressionStatement(expr); 
		} else if (la.kind == 222) {
			lexer.NextToken();

#line  3092 "VBNET.ATG" 
			Statement block;  
			if (
#line  3093 "VBNET.ATG" 
Peek(1).kind == Tokens.As) {

#line  3094 "VBNET.ATG" 
				LocalVariableDeclaration resourceAquisition = new LocalVariableDeclaration(Modifiers.None); 
				VariableDeclarator(
#line  3095 "VBNET.ATG" 
resourceAquisition.Variables);
				while (la.kind == 21) {
					lexer.NextToken();
					VariableDeclarator(
#line  3097 "VBNET.ATG" 
resourceAquisition.Variables);
				}
				Block(
#line  3099 "VBNET.ATG" 
out block);

#line  3101 "VBNET.ATG" 
				statement = new UsingStatement(resourceAquisition, block);
				
			} else if (StartOf(29)) {
				Expr(
#line  3103 "VBNET.ATG" 
out expr);
				Block(
#line  3104 "VBNET.ATG" 
out block);

#line  3105 "VBNET.ATG" 
				statement = new UsingStatement(new ExpressionStatement(expr), block); 
			} else SynErr(292);
			Expect(110);
			Expect(222);
		} else if (StartOf(40)) {
			LocalDeclarationStatement(
#line  3108 "VBNET.ATG" 
out statement);
		} else SynErr(293);
	}

	void LocalDeclarationStatement(
#line  2788 "VBNET.ATG" 
out Statement statement) {

#line  2790 "VBNET.ATG" 
		ModifierList m = new ModifierList();
		LocalVariableDeclaration localVariableDeclaration;
		bool dimfound = false;
		
		while (la.kind == 85 || la.kind == 102 || la.kind == 200) {
			if (la.kind == 85) {
				lexer.NextToken();

#line  2796 "VBNET.ATG" 
				m.Add(Modifiers.Const, t.Location); 
			} else if (la.kind == 200) {
				lexer.NextToken();

#line  2797 "VBNET.ATG" 
				m.Add(Modifiers.Static, t.Location); 
			} else {
				lexer.NextToken();

#line  2798 "VBNET.ATG" 
				dimfound = true; 
			}
		}

#line  2801 "VBNET.ATG" 
		if(dimfound && (m.Modifier & Modifiers.Const) != 0) {
		Error("Dim is not allowed on constants.");
		}
		
		if(m.isNone && dimfound == false) {
			Error("Const, Dim or Static expected");
		}
		
		localVariableDeclaration = new LocalVariableDeclaration(m.Modifier);
		localVariableDeclaration.StartLocation = t.Location;
		
		VariableDeclarator(
#line  2812 "VBNET.ATG" 
localVariableDeclaration.Variables);
		while (la.kind == 21) {
			lexer.NextToken();
			VariableDeclarator(
#line  2813 "VBNET.ATG" 
localVariableDeclaration.Variables);
		}

#line  2815 "VBNET.ATG" 
		statement = localVariableDeclaration;
		
	}

	void TryStatement(
#line  3309 "VBNET.ATG" 
out Statement tryStatement) {

#line  3311 "VBNET.ATG" 
		Statement blockStmt = null, finallyStmt = null;List<CatchClause> catchClauses = null;
		
		Expect(214);
		EndOfStmt();
		Block(
#line  3314 "VBNET.ATG" 
out blockStmt);
		if (la.kind == 72 || la.kind == 110 || la.kind == 120) {
			CatchClauses(
#line  3315 "VBNET.ATG" 
out catchClauses);
		}
		if (la.kind == 120) {
			lexer.NextToken();
			EndOfStmt();
			Block(
#line  3316 "VBNET.ATG" 
out finallyStmt);
		}
		Expect(110);
		Expect(214);

#line  3319 "VBNET.ATG" 
		tryStatement = new TryCatchStatement(blockStmt, catchClauses, finallyStmt);
		
	}

	void WithStatement(
#line  3289 "VBNET.ATG" 
out Statement withStatement) {

#line  3291 "VBNET.ATG" 
		Statement blockStmt = null;
		Expression expr = null;
		
		Expect(229);

#line  3294 "VBNET.ATG" 
		Location start = t.Location; 
		Expr(
#line  3295 "VBNET.ATG" 
out expr);
		EndOfStmt();

#line  3297 "VBNET.ATG" 
		withStatement = new WithStatement(expr);
		withStatement.StartLocation = start;
		
		Block(
#line  3300 "VBNET.ATG" 
out blockStmt);

#line  3302 "VBNET.ATG" 
		((WithStatement)withStatement).Body = (BlockStatement)blockStmt;
		
		Expect(110);
		Expect(229);

#line  3305 "VBNET.ATG" 
		withStatement.EndLocation = t.Location; 
	}

	void WhileOrUntil(
#line  3282 "VBNET.ATG" 
out ConditionType conditionType) {

#line  3283 "VBNET.ATG" 
		conditionType = ConditionType.None; 
		if (la.kind == 227) {
			lexer.NextToken();

#line  3284 "VBNET.ATG" 
			conditionType = ConditionType.While; 
		} else if (la.kind == 220) {
			lexer.NextToken();

#line  3285 "VBNET.ATG" 
			conditionType = ConditionType.Until; 
		} else SynErr(294);
	}

	void LoopControlVariable(
#line  3125 "VBNET.ATG" 
out TypeReference type, out string name) {

#line  3126 "VBNET.ATG" 
		ArrayList arrayModifiers = null;
		type = null;
		
		Qualident(
#line  3130 "VBNET.ATG" 
out name);
		if (
#line  3131 "VBNET.ATG" 
IsDims()) {
			ArrayTypeModifiers(
#line  3131 "VBNET.ATG" 
out arrayModifiers);
		}
		if (la.kind == 60) {
			lexer.NextToken();
			TypeName(
#line  3132 "VBNET.ATG" 
out type);

#line  3132 "VBNET.ATG" 
			if (name.IndexOf('.') > 0) { Error("No type def for 'for each' member indexer allowed."); } 
		}

#line  3134 "VBNET.ATG" 
		if (type != null) {
		if(type.RankSpecifier != null && arrayModifiers != null) {
			Error("array rank only allowed one time");
		} else if (arrayModifiers != null) {
			type.RankSpecifier = (int[])arrayModifiers.ToArray(typeof(int));
		}
		}
		
	}

	void ReDimClause(
#line  3204 "VBNET.ATG" 
out Expression expr) {
		SimpleNonInvocationExpression(
#line  3206 "VBNET.ATG" 
out expr);
		ReDimClauseInternal(
#line  3207 "VBNET.ATG" 
ref expr);
	}

	void SingleLineStatementList(
#line  3111 "VBNET.ATG" 
List<Statement> list) {

#line  3112 "VBNET.ATG" 
		Statement embeddedStatement = null; 
		if (la.kind == 110) {
			lexer.NextToken();

#line  3114 "VBNET.ATG" 
			embeddedStatement = new EndStatement(); 
		} else if (StartOf(35)) {
			EmbeddedStatement(
#line  3115 "VBNET.ATG" 
out embeddedStatement);
		} else SynErr(295);

#line  3116 "VBNET.ATG" 
		if (embeddedStatement != null) list.Add(embeddedStatement); 
		while (la.kind == 20) {
			lexer.NextToken();
			while (la.kind == 20) {
				lexer.NextToken();
			}
			if (la.kind == 110) {
				lexer.NextToken();

#line  3118 "VBNET.ATG" 
				embeddedStatement = new EndStatement(); 
			} else if (StartOf(35)) {
				EmbeddedStatement(
#line  3119 "VBNET.ATG" 
out embeddedStatement);
			} else SynErr(296);

#line  3120 "VBNET.ATG" 
			if (embeddedStatement != null) list.Add(embeddedStatement); 
		}
	}

	void CaseClauses(
#line  3242 "VBNET.ATG" 
out List<CaseLabel> caseClauses) {

#line  3244 "VBNET.ATG" 
		caseClauses = new List<CaseLabel>();
		CaseLabel caseClause = null;
		
		CaseClause(
#line  3247 "VBNET.ATG" 
out caseClause);

#line  3247 "VBNET.ATG" 
		if (caseClause != null) { caseClauses.Add(caseClause); } 
		while (la.kind == 21) {
			lexer.NextToken();
			CaseClause(
#line  3248 "VBNET.ATG" 
out caseClause);

#line  3248 "VBNET.ATG" 
			if (caseClause != null) { caseClauses.Add(caseClause); } 
		}
	}

	void OnErrorStatement(
#line  3145 "VBNET.ATG" 
out OnErrorStatement stmt) {

#line  3147 "VBNET.ATG" 
		stmt = null;
		GotoStatement goToStatement = null;
		
		Expect(168);
		Expect(115);
		if (
#line  3153 "VBNET.ATG" 
IsNegativeLabelName()) {
			Expect(129);
			Expect(27);
			Expect(5);

#line  3155 "VBNET.ATG" 
			long intLabel = Int64.Parse(t.val);
			if(intLabel != 1) {
				Error("invalid label in on error statement.");
			}
			stmt = new OnErrorStatement(new GotoStatement((intLabel * -1).ToString()));
			
		} else if (la.kind == 129) {
			GotoStatement(
#line  3161 "VBNET.ATG" 
out goToStatement);

#line  3163 "VBNET.ATG" 
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
			
		} else if (la.kind == 190) {
			lexer.NextToken();
			Expect(160);

#line  3177 "VBNET.ATG" 
			stmt = new OnErrorStatement(new ResumeStatement(true));
			
		} else SynErr(297);
	}

	void GotoStatement(
#line  3183 "VBNET.ATG" 
out GotoStatement goToStatement) {

#line  3185 "VBNET.ATG" 
		string label = String.Empty;
		
		Expect(129);
		LabelName(
#line  3188 "VBNET.ATG" 
out label);

#line  3190 "VBNET.ATG" 
		goToStatement = new GotoStatement(label);
		
	}

	void ResumeStatement(
#line  3231 "VBNET.ATG" 
out ResumeStatement resumeStatement) {

#line  3233 "VBNET.ATG" 
		resumeStatement = null;
		string label = String.Empty;
		
		if (
#line  3236 "VBNET.ATG" 
IsResumeNext()) {
			Expect(190);
			Expect(160);

#line  3237 "VBNET.ATG" 
			resumeStatement = new ResumeStatement(true); 
		} else if (la.kind == 190) {
			lexer.NextToken();
			if (StartOf(41)) {
				LabelName(
#line  3238 "VBNET.ATG" 
out label);
			}

#line  3238 "VBNET.ATG" 
			resumeStatement = new ResumeStatement(label); 
		} else SynErr(298);
	}

	void ReDimClauseInternal(
#line  3210 "VBNET.ATG" 
ref Expression expr) {

#line  3211 "VBNET.ATG" 
		List<Expression> arguments; bool canBeNormal; bool canBeRedim; string name; 
		while (la.kind == 25 || 
#line  3214 "VBNET.ATG" 
la.kind == Tokens.OpenParenthesis && Peek(1).kind == Tokens.Of) {
			if (la.kind == 25) {
				lexer.NextToken();
				IdentifierOrKeyword(
#line  3213 "VBNET.ATG" 
out name);

#line  3213 "VBNET.ATG" 
				expr = new MemberReferenceExpression(expr, name); 
			} else {
				InvocationExpression(
#line  3215 "VBNET.ATG" 
ref expr);
			}
		}
		Expect(34);
		NormalOrReDimArgumentList(
#line  3218 "VBNET.ATG" 
out arguments, out canBeNormal, out canBeRedim);
		Expect(35);

#line  3220 "VBNET.ATG" 
		expr = new InvocationExpression(expr, arguments);
		if (canBeRedim == false || canBeNormal && (la.kind == Tokens.Dot || la.kind == Tokens.OpenParenthesis)) {
			if (this.Errors.Count == 0) {
				// don't recurse on parse errors - could result in endless recursion
				ReDimClauseInternal(ref expr);
			}
		}
		
	}

	void CaseClause(
#line  3252 "VBNET.ATG" 
out CaseLabel caseClause) {

#line  3254 "VBNET.ATG" 
		Expression expr = null;
		Expression sexpr = null;
		BinaryOperatorType op = BinaryOperatorType.None;
		caseClause = null;
		
		if (la.kind == 108) {
			lexer.NextToken();

#line  3260 "VBNET.ATG" 
			caseClause = new CaseLabel(); 
		} else if (StartOf(42)) {
			if (la.kind == 141) {
				lexer.NextToken();
			}
			switch (la.kind) {
			case 37: {
				lexer.NextToken();

#line  3264 "VBNET.ATG" 
				op = BinaryOperatorType.LessThan; 
				break;
			}
			case 36: {
				lexer.NextToken();

#line  3265 "VBNET.ATG" 
				op = BinaryOperatorType.GreaterThan; 
				break;
			}
			case 40: {
				lexer.NextToken();

#line  3266 "VBNET.ATG" 
				op = BinaryOperatorType.LessThanOrEqual; 
				break;
			}
			case 39: {
				lexer.NextToken();

#line  3267 "VBNET.ATG" 
				op = BinaryOperatorType.GreaterThanOrEqual; 
				break;
			}
			case 19: {
				lexer.NextToken();

#line  3268 "VBNET.ATG" 
				op = BinaryOperatorType.Equality; 
				break;
			}
			case 38: {
				lexer.NextToken();

#line  3269 "VBNET.ATG" 
				op = BinaryOperatorType.InEquality; 
				break;
			}
			default: SynErr(299); break;
			}
			Expr(
#line  3271 "VBNET.ATG" 
out expr);

#line  3273 "VBNET.ATG" 
			caseClause = new CaseLabel(op, expr);
			
		} else if (StartOf(29)) {
			Expr(
#line  3275 "VBNET.ATG" 
out expr);
			if (la.kind == 212) {
				lexer.NextToken();
				Expr(
#line  3275 "VBNET.ATG" 
out sexpr);
			}

#line  3277 "VBNET.ATG" 
			caseClause = new CaseLabel(expr, sexpr);
			
		} else SynErr(300);
	}

	void CatchClauses(
#line  3324 "VBNET.ATG" 
out List<CatchClause> catchClauses) {

#line  3326 "VBNET.ATG" 
		catchClauses = new List<CatchClause>();
		TypeReference type = null;
		Statement blockStmt = null;
		Expression expr = null;
		string name = String.Empty;
		
		while (la.kind == 72) {
			lexer.NextToken();
			if (StartOf(4)) {
				Identifier();

#line  3334 "VBNET.ATG" 
				name = t.val; 
				if (la.kind == 60) {
					lexer.NextToken();
					TypeName(
#line  3334 "VBNET.ATG" 
out type);
				}
			}
			if (la.kind == 225) {
				lexer.NextToken();
				Expr(
#line  3335 "VBNET.ATG" 
out expr);
			}
			EndOfStmt();
			Block(
#line  3337 "VBNET.ATG" 
out blockStmt);

#line  3338 "VBNET.ATG" 
			catchClauses.Add(new CatchClause(type, name, blockStmt, expr)); 
		}
	}


	
	void ParseRoot()
	{
		VBNET();

	}
	
	protected override void SynErr(int line, int col, int errorNumber)
	{
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
			case 10: s = "XmlOpenTag expected"; break;
			case 11: s = "XmlCloseTag expected"; break;
			case 12: s = "XmlStartInlineVB expected"; break;
			case 13: s = "XmlEndInlineVB expected"; break;
			case 14: s = "XmlCloseTagEmptyElement expected"; break;
			case 15: s = "XmlOpenEndTag expected"; break;
			case 16: s = "XmlContent expected"; break;
			case 17: s = "XmlComment expected"; break;
			case 18: s = "XmlCData expected"; break;
			case 19: s = "\"=\" expected"; break;
			case 20: s = "\":\" expected"; break;
			case 21: s = "\",\" expected"; break;
			case 22: s = "\"&\" expected"; break;
			case 23: s = "\"/\" expected"; break;
			case 24: s = "\"\\\\\" expected"; break;
			case 25: s = "\".\" expected"; break;
			case 26: s = "\"!\" expected"; break;
			case 27: s = "\"-\" expected"; break;
			case 28: s = "\"+\" expected"; break;
			case 29: s = "\"^\" expected"; break;
			case 30: s = "\"?\" expected"; break;
			case 31: s = "\"*\" expected"; break;
			case 32: s = "\"{\" expected"; break;
			case 33: s = "\"}\" expected"; break;
			case 34: s = "\"(\" expected"; break;
			case 35: s = "\")\" expected"; break;
			case 36: s = "\">\" expected"; break;
			case 37: s = "\"<\" expected"; break;
			case 38: s = "\"<>\" expected"; break;
			case 39: s = "\">=\" expected"; break;
			case 40: s = "\"<=\" expected"; break;
			case 41: s = "\"<<\" expected"; break;
			case 42: s = "\">>\" expected"; break;
			case 43: s = "\"+=\" expected"; break;
			case 44: s = "\"^=\" expected"; break;
			case 45: s = "\"-=\" expected"; break;
			case 46: s = "\"*=\" expected"; break;
			case 47: s = "\"/=\" expected"; break;
			case 48: s = "\"\\\\=\" expected"; break;
			case 49: s = "\"<<=\" expected"; break;
			case 50: s = "\">>=\" expected"; break;
			case 51: s = "\"&=\" expected"; break;
			case 52: s = "\":=\" expected"; break;
			case 53: s = "\"AddHandler\" expected"; break;
			case 54: s = "\"AddressOf\" expected"; break;
			case 55: s = "\"Aggregate\" expected"; break;
			case 56: s = "\"Alias\" expected"; break;
			case 57: s = "\"And\" expected"; break;
			case 58: s = "\"AndAlso\" expected"; break;
			case 59: s = "\"Ansi\" expected"; break;
			case 60: s = "\"As\" expected"; break;
			case 61: s = "\"Ascending\" expected"; break;
			case 62: s = "\"Assembly\" expected"; break;
			case 63: s = "\"Auto\" expected"; break;
			case 64: s = "\"Binary\" expected"; break;
			case 65: s = "\"Boolean\" expected"; break;
			case 66: s = "\"ByRef\" expected"; break;
			case 67: s = "\"By\" expected"; break;
			case 68: s = "\"Byte\" expected"; break;
			case 69: s = "\"ByVal\" expected"; break;
			case 70: s = "\"Call\" expected"; break;
			case 71: s = "\"Case\" expected"; break;
			case 72: s = "\"Catch\" expected"; break;
			case 73: s = "\"CBool\" expected"; break;
			case 74: s = "\"CByte\" expected"; break;
			case 75: s = "\"CChar\" expected"; break;
			case 76: s = "\"CDate\" expected"; break;
			case 77: s = "\"CDbl\" expected"; break;
			case 78: s = "\"CDec\" expected"; break;
			case 79: s = "\"Char\" expected"; break;
			case 80: s = "\"CInt\" expected"; break;
			case 81: s = "\"Class\" expected"; break;
			case 82: s = "\"CLng\" expected"; break;
			case 83: s = "\"CObj\" expected"; break;
			case 84: s = "\"Compare\" expected"; break;
			case 85: s = "\"Const\" expected"; break;
			case 86: s = "\"Continue\" expected"; break;
			case 87: s = "\"CSByte\" expected"; break;
			case 88: s = "\"CShort\" expected"; break;
			case 89: s = "\"CSng\" expected"; break;
			case 90: s = "\"CStr\" expected"; break;
			case 91: s = "\"CType\" expected"; break;
			case 92: s = "\"CUInt\" expected"; break;
			case 93: s = "\"CULng\" expected"; break;
			case 94: s = "\"CUShort\" expected"; break;
			case 95: s = "\"Custom\" expected"; break;
			case 96: s = "\"Date\" expected"; break;
			case 97: s = "\"Decimal\" expected"; break;
			case 98: s = "\"Declare\" expected"; break;
			case 99: s = "\"Default\" expected"; break;
			case 100: s = "\"Delegate\" expected"; break;
			case 101: s = "\"Descending\" expected"; break;
			case 102: s = "\"Dim\" expected"; break;
			case 103: s = "\"DirectCast\" expected"; break;
			case 104: s = "\"Distinct\" expected"; break;
			case 105: s = "\"Do\" expected"; break;
			case 106: s = "\"Double\" expected"; break;
			case 107: s = "\"Each\" expected"; break;
			case 108: s = "\"Else\" expected"; break;
			case 109: s = "\"ElseIf\" expected"; break;
			case 110: s = "\"End\" expected"; break;
			case 111: s = "\"EndIf\" expected"; break;
			case 112: s = "\"Enum\" expected"; break;
			case 113: s = "\"Equals\" expected"; break;
			case 114: s = "\"Erase\" expected"; break;
			case 115: s = "\"Error\" expected"; break;
			case 116: s = "\"Event\" expected"; break;
			case 117: s = "\"Exit\" expected"; break;
			case 118: s = "\"Explicit\" expected"; break;
			case 119: s = "\"False\" expected"; break;
			case 120: s = "\"Finally\" expected"; break;
			case 121: s = "\"For\" expected"; break;
			case 122: s = "\"Friend\" expected"; break;
			case 123: s = "\"From\" expected"; break;
			case 124: s = "\"Function\" expected"; break;
			case 125: s = "\"Get\" expected"; break;
			case 126: s = "\"GetType\" expected"; break;
			case 127: s = "\"Global\" expected"; break;
			case 128: s = "\"GoSub\" expected"; break;
			case 129: s = "\"GoTo\" expected"; break;
			case 130: s = "\"Group\" expected"; break;
			case 131: s = "\"Handles\" expected"; break;
			case 132: s = "\"If\" expected"; break;
			case 133: s = "\"Implements\" expected"; break;
			case 134: s = "\"Imports\" expected"; break;
			case 135: s = "\"In\" expected"; break;
			case 136: s = "\"Infer\" expected"; break;
			case 137: s = "\"Inherits\" expected"; break;
			case 138: s = "\"Integer\" expected"; break;
			case 139: s = "\"Interface\" expected"; break;
			case 140: s = "\"Into\" expected"; break;
			case 141: s = "\"Is\" expected"; break;
			case 142: s = "\"IsNot\" expected"; break;
			case 143: s = "\"Join\" expected"; break;
			case 144: s = "\"Key\" expected"; break;
			case 145: s = "\"Let\" expected"; break;
			case 146: s = "\"Lib\" expected"; break;
			case 147: s = "\"Like\" expected"; break;
			case 148: s = "\"Long\" expected"; break;
			case 149: s = "\"Loop\" expected"; break;
			case 150: s = "\"Me\" expected"; break;
			case 151: s = "\"Mod\" expected"; break;
			case 152: s = "\"Module\" expected"; break;
			case 153: s = "\"MustInherit\" expected"; break;
			case 154: s = "\"MustOverride\" expected"; break;
			case 155: s = "\"MyBase\" expected"; break;
			case 156: s = "\"MyClass\" expected"; break;
			case 157: s = "\"Namespace\" expected"; break;
			case 158: s = "\"Narrowing\" expected"; break;
			case 159: s = "\"New\" expected"; break;
			case 160: s = "\"Next\" expected"; break;
			case 161: s = "\"Not\" expected"; break;
			case 162: s = "\"Nothing\" expected"; break;
			case 163: s = "\"NotInheritable\" expected"; break;
			case 164: s = "\"NotOverridable\" expected"; break;
			case 165: s = "\"Object\" expected"; break;
			case 166: s = "\"Of\" expected"; break;
			case 167: s = "\"Off\" expected"; break;
			case 168: s = "\"On\" expected"; break;
			case 169: s = "\"Operator\" expected"; break;
			case 170: s = "\"Option\" expected"; break;
			case 171: s = "\"Optional\" expected"; break;
			case 172: s = "\"Or\" expected"; break;
			case 173: s = "\"Order\" expected"; break;
			case 174: s = "\"OrElse\" expected"; break;
			case 175: s = "\"Overloads\" expected"; break;
			case 176: s = "\"Overridable\" expected"; break;
			case 177: s = "\"Overrides\" expected"; break;
			case 178: s = "\"ParamArray\" expected"; break;
			case 179: s = "\"Partial\" expected"; break;
			case 180: s = "\"Preserve\" expected"; break;
			case 181: s = "\"Private\" expected"; break;
			case 182: s = "\"Property\" expected"; break;
			case 183: s = "\"Protected\" expected"; break;
			case 184: s = "\"Public\" expected"; break;
			case 185: s = "\"RaiseEvent\" expected"; break;
			case 186: s = "\"ReadOnly\" expected"; break;
			case 187: s = "\"ReDim\" expected"; break;
			case 188: s = "\"Rem\" expected"; break;
			case 189: s = "\"RemoveHandler\" expected"; break;
			case 190: s = "\"Resume\" expected"; break;
			case 191: s = "\"Return\" expected"; break;
			case 192: s = "\"SByte\" expected"; break;
			case 193: s = "\"Select\" expected"; break;
			case 194: s = "\"Set\" expected"; break;
			case 195: s = "\"Shadows\" expected"; break;
			case 196: s = "\"Shared\" expected"; break;
			case 197: s = "\"Short\" expected"; break;
			case 198: s = "\"Single\" expected"; break;
			case 199: s = "\"Skip\" expected"; break;
			case 200: s = "\"Static\" expected"; break;
			case 201: s = "\"Step\" expected"; break;
			case 202: s = "\"Stop\" expected"; break;
			case 203: s = "\"Strict\" expected"; break;
			case 204: s = "\"String\" expected"; break;
			case 205: s = "\"Structure\" expected"; break;
			case 206: s = "\"Sub\" expected"; break;
			case 207: s = "\"SyncLock\" expected"; break;
			case 208: s = "\"Take\" expected"; break;
			case 209: s = "\"Text\" expected"; break;
			case 210: s = "\"Then\" expected"; break;
			case 211: s = "\"Throw\" expected"; break;
			case 212: s = "\"To\" expected"; break;
			case 213: s = "\"True\" expected"; break;
			case 214: s = "\"Try\" expected"; break;
			case 215: s = "\"TryCast\" expected"; break;
			case 216: s = "\"TypeOf\" expected"; break;
			case 217: s = "\"UInteger\" expected"; break;
			case 218: s = "\"ULong\" expected"; break;
			case 219: s = "\"Unicode\" expected"; break;
			case 220: s = "\"Until\" expected"; break;
			case 221: s = "\"UShort\" expected"; break;
			case 222: s = "\"Using\" expected"; break;
			case 223: s = "\"Variant\" expected"; break;
			case 224: s = "\"Wend\" expected"; break;
			case 225: s = "\"When\" expected"; break;
			case 226: s = "\"Where\" expected"; break;
			case 227: s = "\"While\" expected"; break;
			case 228: s = "\"Widening\" expected"; break;
			case 229: s = "\"With\" expected"; break;
			case 230: s = "\"WithEvents\" expected"; break;
			case 231: s = "\"WriteOnly\" expected"; break;
			case 232: s = "\"Xor\" expected"; break;
			case 233: s = "??? expected"; break;
			case 234: s = "invalid EndOfStmt"; break;
			case 235: s = "invalid OptionStmt"; break;
			case 236: s = "invalid OptionStmt"; break;
			case 237: s = "invalid GlobalAttributeSection"; break;
			case 238: s = "invalid GlobalAttributeSection"; break;
			case 239: s = "invalid NamespaceMemberDecl"; break;
			case 240: s = "invalid OptionValue"; break;
			case 241: s = "invalid ImportClause"; break;
			case 242: s = "invalid Identifier"; break;
			case 243: s = "invalid TypeModifier"; break;
			case 244: s = "invalid NonModuleDeclaration"; break;
			case 245: s = "invalid NonModuleDeclaration"; break;
			case 246: s = "invalid TypeParameterConstraints"; break;
			case 247: s = "invalid TypeParameterConstraint"; break;
			case 248: s = "invalid NonArrayTypeName"; break;
			case 249: s = "invalid MemberModifier"; break;
			case 250: s = "invalid StructureMemberDecl"; break;
			case 251: s = "invalid StructureMemberDecl"; break;
			case 252: s = "invalid StructureMemberDecl"; break;
			case 253: s = "invalid StructureMemberDecl"; break;
			case 254: s = "invalid StructureMemberDecl"; break;
			case 255: s = "invalid StructureMemberDecl"; break;
			case 256: s = "invalid StructureMemberDecl"; break;
			case 257: s = "invalid StructureMemberDecl"; break;
			case 258: s = "invalid InterfaceMemberDecl"; break;
			case 259: s = "invalid InterfaceMemberDecl"; break;
			case 260: s = "invalid Expr"; break;
			case 261: s = "invalid Charset"; break;
			case 262: s = "invalid IdentifierForFieldDeclaration"; break;
			case 263: s = "invalid VariableDeclaratorPartAfterIdentifier"; break;
			case 264: s = "invalid AccessorDecls"; break;
			case 265: s = "invalid EventAccessorDeclaration"; break;
			case 266: s = "invalid OverloadableOperator"; break;
			case 267: s = "invalid EventMemberSpecifier"; break;
			case 268: s = "invalid AssignmentOperator"; break;
			case 269: s = "invalid SimpleNonInvocationExpression"; break;
			case 270: s = "invalid SimpleNonInvocationExpression"; break;
			case 271: s = "invalid SimpleNonInvocationExpression"; break;
			case 272: s = "invalid SimpleNonInvocationExpression"; break;
			case 273: s = "invalid PrimitiveTypeName"; break;
			case 274: s = "invalid CastTarget"; break;
			case 275: s = "invalid ComparisonExpr"; break;
			case 276: s = "invalid FromOrAggregateQueryOperator"; break;
			case 277: s = "invalid QueryOperator"; break;
			case 278: s = "invalid PartitionQueryOperator"; break;
			case 279: s = "invalid Argument"; break;
			case 280: s = "invalid QualIdentAndTypeArguments"; break;
			case 281: s = "invalid AttributeArguments"; break;
			case 282: s = "invalid AttributeArguments"; break;
			case 283: s = "invalid AttributeArguments"; break;
			case 284: s = "invalid ParameterModifier"; break;
			case 285: s = "invalid Statement"; break;
			case 286: s = "invalid LabelName"; break;
			case 287: s = "invalid EmbeddedStatement"; break;
			case 288: s = "invalid EmbeddedStatement"; break;
			case 289: s = "invalid EmbeddedStatement"; break;
			case 290: s = "invalid EmbeddedStatement"; break;
			case 291: s = "invalid EmbeddedStatement"; break;
			case 292: s = "invalid EmbeddedStatement"; break;
			case 293: s = "invalid EmbeddedStatement"; break;
			case 294: s = "invalid WhileOrUntil"; break;
			case 295: s = "invalid SingleLineStatementList"; break;
			case 296: s = "invalid SingleLineStatementList"; break;
			case 297: s = "invalid OnErrorStatement"; break;
			case 298: s = "invalid ResumeStatement"; break;
			case 299: s = "invalid CaseClause"; break;
			case 300: s = "invalid CaseClause"; break;

			default: s = "error " + errorNumber; break;
		}
		this.Errors.Error(line, col, s);
	}
	
	private bool StartOf(int s)
	{
		return set[s, lexer.LookAhead.kind];
	}
	
	static bool[,] set = {
	{T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,x,T, T,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,x,T, T,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,x,T, T,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,T,T,T, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,x, T,x,x,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, T,x,x,x, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,T,T,T, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, T,x,x,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, T,x,x,x, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,T,T,T, T,x,T,T, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,x, T,x,x,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, T,x,x,x, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,T,x,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,T,T,T, T,T,x,T, T,x,x,x, x,x,x,x, x,x,x,T, x,T,x,x, T,x,x,x, x,x,x,x, x,x,x,T, T,T,x,x, x,T,x,x, T,x,T,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,T, x,x,x,T, x,x,T,x, x,x,x,x, T,x,T,x, T,x,x,T, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,T, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,T,T,T, x,x,x,x, T,T,x,x, T,T,x,x, x,x,x,x, x,T,T,T, T,T,x,x, x,x,T,x, x,x,x,x, x,x,x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,T,T,T, T,T,x,T, T,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,T, T,T,x,x, x,T,x,x, T,x,T,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,T, x,x,x,T, x,x,T,x, x,x,x,x, T,x,T,x, T,x,x,T, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,T,T,T, x,x,x,x, T,x,x,x, T,T,x,x, x,x,x,x, x,T,T,T, T,T,x,x, x,x,T,x, x,x,x,x, x,x,x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,T,T,T, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, T,T,x,x, x,x,x,x, x,x,x,T, x,x,T,T, T,T,T,x, T,x,x,x, x,x,x,x, T,T,x,x, T,x,T,x, x,x,T,T, T,x,x,x, x,x,T,x, x,x,x,x, T,x,x,T, T,x,x,T, x,x,x,x, x,x,x,x, T,T,T,x, x,x,T,x, x,x,x,T, T,x,x,T, x,T,x,x, x,T,x,T, T,T,x,T, T,T,T,T, T,x,T,x, x,x,x,x, x,x,x,T, T,x,x,T, x,x,x,x, x,T,T,x, T,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,T,x, T,x,T,T, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,T, T,T,x,T, x,T,x,T, T,x,T,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,T,T,T, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,x, T,x,x,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,T, x,x,x,T, x,x,T,x, x,x,x,x, T,x,x,x, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, T,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,T,T,T, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,x, T,x,x,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, T,x,x,x, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,T,x, x,x,x,x, x,x,x,x, T,x,x,x, T,x,x,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, T,T,T,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,T, T,T,x,T, x,T,T,T, T,x,T,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, T,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,T, T,T,x,T, x,T,T,T, T,x,T,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, T,x,x,T, T,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,T, x,T,T,T, T,T,x,T, T,x,x,x, x,T,T,T, T,T,T,T, T,x,T,T, T,x,x,T, T,T,T,T, T,T,T,T, T,T,x,x, x,T,x,T, T,x,T,x, x,x,x,x, x,T,x,x, x,x,T,T, x,x,x,T, x,x,T,T, x,x,T,x, T,x,x,x, T,x,T,x, T,x,x,T, x,x,x,x, T,x,T,x, x,x,x,T, T,x,x,T, x,T,T,x, x,T,x,T, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,T,T,T, x,x,x,x, T,x,x,x, T,T,x,x, x,T,x,T, T,T,T,T, T,T,x,x, x,x,T,x, x,x,x,x, x,x,x},
	{x,T,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, T,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,T, x,x,x,T, x,T,T,T, T,T,x,T, T,x,T,x, x,T,T,T, T,T,T,T, T,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,x, x,T,T,T, T,T,T,x, x,x,x,x, x,T,T,T, x,T,T,T, x,T,x,T, x,x,T,T, x,T,T,x, T,x,x,x, T,x,T,x, T,x,x,T, x,x,x,x, T,x,T,x, x,x,x,T, T,x,x,T, x,x,T,x, x,T,x,T, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,T,x,T, x,T,T,T, T,T,x,x, x,T,T,T, T,x,T,x, T,x,x,T, T,T,x,T, x,T,T,T, T,T,T,T, T,T,T,x, x,x,T,T, x,T,x,x, x,x,x},
	{x,T,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,T, T,T,x,x, x,T,x,x, x,x,x,x, T,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,T, x,x,x,T, T,T,T,T, T,T,x,T, T,x,T,x, x,T,T,T, T,T,T,T, T,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,x, x,T,T,T, T,T,T,x, T,x,T,x, x,T,T,T, x,T,T,T, x,T,x,T, x,x,T,T, x,T,T,x, T,x,x,x, T,x,T,x, T,x,x,T, x,x,x,x, T,x,T,x, x,x,x,T, T,x,x,T, x,x,T,x, x,T,x,T, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,T,x,T, x,T,T,T, T,T,x,x, x,T,T,T, T,x,T,x, T,x,x,T, T,T,x,T, x,T,T,T, T,T,T,T, T,T,T,x, x,x,T,T, x,T,x,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, T,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, T,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, T,x,x,T, T,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,T, x,T,T,T, T,T,x,T, T,x,x,x, x,T,T,T, T,T,T,T, T,x,T,T, T,x,x,T, T,T,T,T, T,T,T,T, T,T,x,x, x,T,x,T, T,x,T,x, x,x,x,x, x,T,x,x, x,x,T,T, x,x,x,T, T,x,T,T, x,x,T,x, T,x,x,x, T,x,T,x, T,x,x,T, x,x,x,x, T,x,T,x, x,x,x,T, T,x,x,T, x,T,T,x, x,T,x,T, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,T,T,T, x,x,x,x, T,x,x,x, T,T,x,x, x,T,x,T, T,T,T,T, T,T,x,x, x,x,T,x, x,x,x,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x},
	{x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,T, x,T,T,T, T,T,x,T, T,x,x,x, x,T,T,T, T,T,T,T, T,x,T,T, T,x,x,T, T,T,T,T, T,T,T,T, T,T,x,x, x,T,x,T, T,x,T,x, x,x,x,x, x,T,x,x, x,x,T,T, x,x,x,T, x,x,T,T, x,x,T,x, T,x,x,x, T,x,T,x, T,x,x,T, x,x,x,x, T,x,T,x, x,x,x,T, T,x,x,T, x,x,T,x, x,T,x,T, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,T,T,T, x,x,x,x, T,x,x,x, T,T,x,x, x,T,x,T, T,T,T,T, T,T,x,x, x,x,T,x, x,x,x,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,T,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, T,x,x,T, T,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,T, x,T,T,T, T,T,x,T, T,x,x,x, x,T,T,T, T,T,T,T, T,x,T,T, T,x,x,T, T,T,T,T, T,T,T,T, T,T,x,x, x,T,x,T, T,x,T,x, x,x,x,x, x,T,x,x, x,x,T,T, x,x,x,T, x,x,T,T, x,x,T,x, T,x,x,x, T,x,T,x, T,x,x,T, x,x,x,x, T,x,T,x, x,x,x,T, T,x,x,T, x,x,T,x, x,T,x,T, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,T,T,T, x,x,x,x, T,x,x,x, T,T,x,x, x,T,x,T, T,T,T,T, T,T,x,x, x,x,T,x, x,x,x,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, T,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,T, x,x,x,T, x,T,T,T, T,T,x,T, T,x,T,x, x,T,T,T, T,T,T,T, T,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,x, x,T,T,T, T,T,T,x, x,x,x,x, x,T,T,T, x,T,T,T, x,T,x,T, x,x,T,T, x,T,T,x, T,x,x,x, T,x,T,x, T,x,x,T, x,x,x,x, T,x,T,x, x,x,x,T, T,x,x,T, x,x,T,x, x,T,x,T, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,T,x,T, x,T,T,T, T,T,x,x, x,T,T,T, T,x,T,x, T,x,x,T, T,T,x,T, x,T,T,T, T,T,T,T, T,T,T,x, x,x,T,T, x,T,x,x, x,x,x},
	{x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,x,x,T, T,x,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,T, x,T,T,T, T,T,x,T, T,x,x,x, x,T,T,T, T,T,T,T, T,x,T,T, T,x,x,T, T,T,T,T, T,T,T,T, T,T,x,x, x,T,x,T, T,x,T,x, x,x,x,x, x,T,x,x, x,x,T,T, x,x,x,T, T,x,T,T, x,x,T,x, T,x,x,x, T,x,T,x, T,x,x,T, x,x,x,x, T,x,T,x, x,x,x,T, T,x,x,T, x,T,T,x, x,T,x,T, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,T,T,T, x,x,x,x, T,x,x,x, T,T,x,x, x,T,x,T, T,T,T,T, T,T,x,x, x,x,T,x, x,x,x,x, x,x,x},
	{x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, T,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,T, x,T,T,T, T,T,x,T, T,x,x,x, x,T,T,T, T,T,T,T, T,x,T,T, T,x,x,T, T,T,T,T, T,T,T,T, T,T,x,x, x,T,x,T, T,x,T,x, x,x,x,x, x,T,x,x, x,x,T,T, x,x,x,T, x,x,T,T, x,x,T,x, T,x,x,x, T,x,T,x, T,x,x,T, x,x,x,x, T,x,T,x, x,x,x,T, T,x,x,T, x,x,T,x, x,T,x,T, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,T,T,T, x,x,x,x, T,x,x,x, T,T,x,x, x,T,x,T, T,T,T,T, T,T,x,x, x,x,T,x, x,x,x,x, x,x,x},
	{x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, T,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,T, x,x,x,T, x,T,T,T, T,T,x,T, T,x,T,x, x,T,T,T, T,T,T,T, T,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,x, x,T,T,T, T,T,T,x, x,x,T,x, x,T,T,T, x,T,T,T, x,T,x,T, x,x,T,T, x,T,T,x, T,x,x,x, T,x,T,x, T,x,x,T, x,x,x,x, T,x,T,x, x,x,x,T, T,x,x,T, x,x,T,x, x,T,x,T, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,T,x,T, x,T,T,T, T,T,x,x, x,T,T,T, T,x,T,x, T,x,x,T, T,T,x,T, x,T,T,T, T,T,T,T, T,T,T,x, x,x,T,T, x,T,x,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,T,T, T,T,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,T,T,T, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,T,x, T,x,x,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, T,x,x,x, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x},
	{x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,T,T,T, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,x, T,x,x,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, T,x,x,x, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,T,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x}

	};
} // end Parser

}