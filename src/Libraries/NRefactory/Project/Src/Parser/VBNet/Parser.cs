
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
	const int maxT = 236;

	const  bool   T            = true;
	const  bool   x            = false;
	

#line  12 "VBNET.ATG" 


/*

*/

	void VBNET() {

#line  260 "VBNET.ATG" 
		lexer.NextToken(); // get the first token
		compilationUnit = new CompilationUnit();
		BlockStart(compilationUnit);
		
		while (la.kind == 1 || la.kind == 22) {
			EndOfStmt();
		}
		while (la.kind == 172) {
			OptionStmt();
			while (la.kind == 1 || la.kind == 22) {
				EndOfStmt();
			}
		}
		while (la.kind == 136) {
			ImportsStmt();
			while (la.kind == 1 || la.kind == 22) {
				EndOfStmt();
			}
		}
		while (
#line  268 "VBNET.ATG" 
IsGlobalAttrTarget()) {
			GlobalAttributeSection();
			while (la.kind == 1 || la.kind == 22) {
				EndOfStmt();
			}
		}
		while (StartOf(1)) {
			NamespaceMemberDecl();
			while (la.kind == 1 || la.kind == 22) {
				EndOfStmt();
			}
		}
		Expect(0);
	}

	void EndOfStmt() {
		if (la.kind == 1) {
			lexer.NextToken();
		} else if (la.kind == 22) {
			lexer.NextToken();
		} else SynErr(237);
	}

	void OptionStmt() {

#line  273 "VBNET.ATG" 
		INode node = null; bool val = true; 
		Expect(172);

#line  274 "VBNET.ATG" 
		Location startPos = t.Location; 
		if (la.kind == 120) {
			lexer.NextToken();
			if (la.kind == 169 || la.kind == 170) {
				OptionValue(
#line  276 "VBNET.ATG" 
ref val);
			}

#line  277 "VBNET.ATG" 
			node = new OptionDeclaration(OptionType.Explicit, val); 
		} else if (la.kind == 205) {
			lexer.NextToken();
			if (la.kind == 169 || la.kind == 170) {
				OptionValue(
#line  279 "VBNET.ATG" 
ref val);
			}

#line  280 "VBNET.ATG" 
			node = new OptionDeclaration(OptionType.Strict, val); 
		} else if (la.kind == 86) {
			lexer.NextToken();
			if (la.kind == 66) {
				lexer.NextToken();

#line  282 "VBNET.ATG" 
				node = new OptionDeclaration(OptionType.CompareBinary, val); 
			} else if (la.kind == 211) {
				lexer.NextToken();

#line  283 "VBNET.ATG" 
				node = new OptionDeclaration(OptionType.CompareText, val); 
			} else SynErr(238);
		} else if (la.kind == 138) {
			lexer.NextToken();
			if (la.kind == 169 || la.kind == 170) {
				OptionValue(
#line  286 "VBNET.ATG" 
ref val);
			}

#line  287 "VBNET.ATG" 
			node = new OptionDeclaration(OptionType.Infer, val); 
		} else SynErr(239);
		EndOfStmt();

#line  291 "VBNET.ATG" 
		if (node != null) {
		node.StartLocation = startPos;
		node.EndLocation   = t.Location;
		AddChild(node);
		}
		
	}

	void ImportsStmt() {

#line  314 "VBNET.ATG" 
		List<Using> usings = new List<Using>();
		
		Expect(136);

#line  318 "VBNET.ATG" 
		Location startPos = t.Location;
		Using u;
		
		ImportClause(
#line  321 "VBNET.ATG" 
out u);

#line  321 "VBNET.ATG" 
		if (u != null) { usings.Add(u); } 
		while (la.kind == 23) {
			lexer.NextToken();
			ImportClause(
#line  323 "VBNET.ATG" 
out u);

#line  323 "VBNET.ATG" 
			if (u != null) { usings.Add(u); } 
		}
		EndOfStmt();

#line  327 "VBNET.ATG" 
		UsingDeclaration usingDeclaration = new UsingDeclaration(usings);
		usingDeclaration.StartLocation = startPos;
		usingDeclaration.EndLocation   = t.Location;
		AddChild(usingDeclaration);
		
	}

	void GlobalAttributeSection() {
		Expect(39);

#line  2651 "VBNET.ATG" 
		Location startPos = t.Location; 
		if (la.kind == 64) {
			lexer.NextToken();
		} else if (la.kind == 154) {
			lexer.NextToken();
		} else SynErr(240);

#line  2653 "VBNET.ATG" 
		string attributeTarget = t.val != null ? t.val.ToLower(System.Globalization.CultureInfo.InvariantCulture) : null;
		List<ASTAttribute> attributes = new List<ASTAttribute>();
		ASTAttribute attribute;
		
		Expect(22);
		Attribute(
#line  2657 "VBNET.ATG" 
out attribute);

#line  2657 "VBNET.ATG" 
		attributes.Add(attribute); 
		while (
#line  2658 "VBNET.ATG" 
NotFinalComma()) {
			if (la.kind == 23) {
				lexer.NextToken();
				if (la.kind == 64) {
					lexer.NextToken();
				} else if (la.kind == 154) {
					lexer.NextToken();
				} else SynErr(241);
				Expect(22);
			}
			Attribute(
#line  2658 "VBNET.ATG" 
out attribute);

#line  2658 "VBNET.ATG" 
			attributes.Add(attribute); 
		}
		if (la.kind == 23) {
			lexer.NextToken();
		}
		Expect(38);
		EndOfStmt();

#line  2663 "VBNET.ATG" 
		AttributeSection section = new AttributeSection {
		AttributeTarget = attributeTarget,
		Attributes = attributes,
		StartLocation = startPos,
		EndLocation = t.EndLocation
		};
		AddChild(section);
		
	}

	void NamespaceMemberDecl() {

#line  360 "VBNET.ATG" 
		ModifierList m = new ModifierList();
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		string qualident;
		
		if (la.kind == 159) {
			lexer.NextToken();

#line  367 "VBNET.ATG" 
			Location startPos = t.Location;
			
			Qualident(
#line  369 "VBNET.ATG" 
out qualident);

#line  371 "VBNET.ATG" 
			INode node =  new NamespaceDeclaration(qualident);
			node.StartLocation = startPos;
			AddChild(node);
			BlockStart(node);
			
			EndOfStmt();
			NamespaceBody();

#line  379 "VBNET.ATG" 
			node.EndLocation = t.Location;
			BlockEnd();
			
		} else if (StartOf(2)) {
			while (la.kind == 39) {
				AttributeSection(
#line  383 "VBNET.ATG" 
out section);

#line  383 "VBNET.ATG" 
				attributes.Add(section); 
			}
			while (StartOf(3)) {
				TypeModifier(
#line  384 "VBNET.ATG" 
m);
			}
			NonModuleDeclaration(
#line  384 "VBNET.ATG" 
m, attributes);
		} else SynErr(242);
	}

	void OptionValue(
#line  299 "VBNET.ATG" 
ref bool val) {
		if (la.kind == 170) {
			lexer.NextToken();

#line  301 "VBNET.ATG" 
			val = true; 
		} else if (la.kind == 169) {
			lexer.NextToken();

#line  303 "VBNET.ATG" 
			val = false; 
		} else SynErr(243);
	}

	void ImportClause(
#line  334 "VBNET.ATG" 
out Using u) {

#line  336 "VBNET.ATG" 
		string qualident  = null;
		TypeReference aliasedType = null;
		u = null;
		
		if (StartOf(4)) {
			Qualident(
#line  341 "VBNET.ATG" 
out qualident);
			if (la.kind == 21) {
				lexer.NextToken();
				TypeName(
#line  342 "VBNET.ATG" 
out aliasedType);
			}

#line  344 "VBNET.ATG" 
			if (qualident != null && qualident.Length > 0) {
			if (aliasedType != null) {
				u = new Using(qualident, aliasedType);
			} else {
				u = new Using(qualident);
			}
			}
			
		} else if (la.kind == 10) {

#line  352 "VBNET.ATG" 
			string prefix = null; 
			lexer.NextToken();
			Identifier();

#line  353 "VBNET.ATG" 
			prefix = t.val; 
			Expect(21);
			Expect(3);

#line  353 "VBNET.ATG" 
			u = new Using(t.literalValue as string, prefix); 
			Expect(11);
		} else SynErr(244);
	}

	void Qualident(
#line  3407 "VBNET.ATG" 
out string qualident) {

#line  3409 "VBNET.ATG" 
		string name;
		qualidentBuilder.Length = 0; 
		
		Identifier();

#line  3413 "VBNET.ATG" 
		qualidentBuilder.Append(t.val); 
		while (
#line  3414 "VBNET.ATG" 
DotAndIdentOrKw()) {
			Expect(27);
			IdentifierOrKeyword(
#line  3414 "VBNET.ATG" 
out name);

#line  3414 "VBNET.ATG" 
			qualidentBuilder.Append('.'); qualidentBuilder.Append(name); 
		}

#line  3416 "VBNET.ATG" 
		qualident = qualidentBuilder.ToString(); 
	}

	void TypeName(
#line  2524 "VBNET.ATG" 
out TypeReference typeref) {

#line  2525 "VBNET.ATG" 
		ArrayList rank = null; 
		NonArrayTypeName(
#line  2527 "VBNET.ATG" 
out typeref, false);
		ArrayTypeModifiers(
#line  2531 "VBNET.ATG" 
out rank);

#line  2532 "VBNET.ATG" 
		if (rank != null && typeref != null) {
		typeref.RankSpecifier = (int[])rank.ToArray(typeof(int));
		}
		
	}

	void Identifier() {
		if (StartOf(5)) {
			IdentifierForFieldDeclaration();
		} else if (la.kind == 97) {
			lexer.NextToken();
		} else SynErr(245);
	}

	void NamespaceBody() {
		while (la.kind == 1 || la.kind == 22) {
			EndOfStmt();
		}
		while (StartOf(1)) {
			NamespaceMemberDecl();
			while (la.kind == 1 || la.kind == 22) {
				EndOfStmt();
			}
		}
		Expect(112);
		Expect(159);
		EndOfStmt();
	}

	void AttributeSection(
#line  2726 "VBNET.ATG" 
out AttributeSection section) {

#line  2728 "VBNET.ATG" 
		string attributeTarget = "";List<ASTAttribute> attributes = new List<ASTAttribute>();
		ASTAttribute attribute;
		
		
		Expect(39);

#line  2732 "VBNET.ATG" 
		Location startPos = t.Location; 
		if (
#line  2733 "VBNET.ATG" 
IsLocalAttrTarget()) {
			if (la.kind == 118) {
				lexer.NextToken();

#line  2734 "VBNET.ATG" 
				attributeTarget = "event";
			} else if (la.kind == 193) {
				lexer.NextToken();

#line  2735 "VBNET.ATG" 
				attributeTarget = "return";
			} else {
				Identifier();

#line  2738 "VBNET.ATG" 
				string val = t.val.ToLower(System.Globalization.CultureInfo.InvariantCulture);
				if (val != "field"	|| val != "method" ||
					val != "module" || val != "param"  ||
					val != "property" || val != "type")
				Error("attribute target specifier (event, return, field," +
						"method, module, param, property, or type) expected");
				attributeTarget = t.val;
				
			}
			Expect(22);
		}
		Attribute(
#line  2748 "VBNET.ATG" 
out attribute);

#line  2748 "VBNET.ATG" 
		attributes.Add(attribute); 
		while (
#line  2749 "VBNET.ATG" 
NotFinalComma()) {
			Expect(23);
			Attribute(
#line  2749 "VBNET.ATG" 
out attribute);

#line  2749 "VBNET.ATG" 
			attributes.Add(attribute); 
		}
		if (la.kind == 23) {
			lexer.NextToken();
		}
		Expect(38);

#line  2753 "VBNET.ATG" 
		section = new AttributeSection {
		AttributeTarget = attributeTarget,
		Attributes = attributes,
		StartLocation = startPos,
		EndLocation = t.EndLocation
		};
		
	}

	void TypeModifier(
#line  3490 "VBNET.ATG" 
ModifierList m) {
		switch (la.kind) {
		case 186: {
			lexer.NextToken();

#line  3491 "VBNET.ATG" 
			m.Add(Modifiers.Public, t.Location); 
			break;
		}
		case 185: {
			lexer.NextToken();

#line  3492 "VBNET.ATG" 
			m.Add(Modifiers.Protected, t.Location); 
			break;
		}
		case 124: {
			lexer.NextToken();

#line  3493 "VBNET.ATG" 
			m.Add(Modifiers.Internal, t.Location); 
			break;
		}
		case 183: {
			lexer.NextToken();

#line  3494 "VBNET.ATG" 
			m.Add(Modifiers.Private, t.Location); 
			break;
		}
		case 198: {
			lexer.NextToken();

#line  3495 "VBNET.ATG" 
			m.Add(Modifiers.Static, t.Location); 
			break;
		}
		case 197: {
			lexer.NextToken();

#line  3496 "VBNET.ATG" 
			m.Add(Modifiers.New, t.Location); 
			break;
		}
		case 155: {
			lexer.NextToken();

#line  3497 "VBNET.ATG" 
			m.Add(Modifiers.Abstract, t.Location); 
			break;
		}
		case 165: {
			lexer.NextToken();

#line  3498 "VBNET.ATG" 
			m.Add(Modifiers.Sealed, t.Location); 
			break;
		}
		case 181: {
			lexer.NextToken();

#line  3499 "VBNET.ATG" 
			m.Add(Modifiers.Partial, t.Location); 
			break;
		}
		default: SynErr(246); break;
		}
	}

	void NonModuleDeclaration(
#line  444 "VBNET.ATG" 
ModifierList m, List<AttributeSection> attributes) {

#line  446 "VBNET.ATG" 
		TypeReference typeRef = null;
		List<TypeReference> baseInterfaces = null;
		
		switch (la.kind) {
		case 83: {

#line  449 "VBNET.ATG" 
			m.Check(Modifiers.Classes); 
			lexer.NextToken();

#line  452 "VBNET.ATG" 
			TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
			newType.StartLocation = t.Location;
			AddChild(newType);
			BlockStart(newType);
			
			newType.Type       = ClassType.Class;
			
			Identifier();

#line  459 "VBNET.ATG" 
			newType.Name = t.val; 
			TypeParameterList(
#line  460 "VBNET.ATG" 
newType.Templates);
			EndOfStmt();

#line  462 "VBNET.ATG" 
			newType.BodyStartLocation = t.Location; 
			if (la.kind == 139) {
				ClassBaseType(
#line  463 "VBNET.ATG" 
out typeRef);

#line  463 "VBNET.ATG" 
				SafeAdd(newType, newType.BaseTypes, typeRef); 
			}
			while (la.kind == 135) {
				TypeImplementsClause(
#line  464 "VBNET.ATG" 
out baseInterfaces);

#line  464 "VBNET.ATG" 
				newType.BaseTypes.AddRange(baseInterfaces); 
			}
			ClassBody(
#line  465 "VBNET.ATG" 
newType);
			Expect(112);
			Expect(83);

#line  466 "VBNET.ATG" 
			newType.EndLocation = t.EndLocation; 
			EndOfStmt();

#line  469 "VBNET.ATG" 
			BlockEnd();
			
			break;
		}
		case 154: {
			lexer.NextToken();

#line  473 "VBNET.ATG" 
			m.Check(Modifiers.VBModules);
			TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
			AddChild(newType);
			BlockStart(newType);
			newType.StartLocation = m.GetDeclarationLocation(t.Location);
			newType.Type = ClassType.Module;
			
			Identifier();

#line  480 "VBNET.ATG" 
			newType.Name = t.val; 
			EndOfStmt();

#line  482 "VBNET.ATG" 
			newType.BodyStartLocation = t.Location; 
			ModuleBody(
#line  483 "VBNET.ATG" 
newType);

#line  485 "VBNET.ATG" 
			BlockEnd();
			
			break;
		}
		case 207: {
			lexer.NextToken();

#line  489 "VBNET.ATG" 
			m.Check(Modifiers.VBStructures);
			TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
			AddChild(newType);
			BlockStart(newType);
			newType.StartLocation = m.GetDeclarationLocation(t.Location);
			newType.Type = ClassType.Struct;
			
			Identifier();

#line  496 "VBNET.ATG" 
			newType.Name = t.val; 
			TypeParameterList(
#line  497 "VBNET.ATG" 
newType.Templates);
			EndOfStmt();

#line  499 "VBNET.ATG" 
			newType.BodyStartLocation = t.Location; 
			while (la.kind == 135) {
				TypeImplementsClause(
#line  500 "VBNET.ATG" 
out baseInterfaces);

#line  500 "VBNET.ATG" 
				newType.BaseTypes.AddRange(baseInterfaces);
			}
			StructureBody(
#line  501 "VBNET.ATG" 
newType);

#line  503 "VBNET.ATG" 
			BlockEnd();
			
			break;
		}
		case 114: {
			lexer.NextToken();

#line  508 "VBNET.ATG" 
			m.Check(Modifiers.VBEnums);
			TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
			newType.StartLocation = m.GetDeclarationLocation(t.Location);
			AddChild(newType);
			BlockStart(newType);
			
			newType.Type = ClassType.Enum;
			
			Identifier();

#line  516 "VBNET.ATG" 
			newType.Name = t.val; 
			if (la.kind == 62) {
				lexer.NextToken();
				NonArrayTypeName(
#line  517 "VBNET.ATG" 
out typeRef, false);

#line  517 "VBNET.ATG" 
				SafeAdd(newType, newType.BaseTypes, typeRef); 
			}
			EndOfStmt();

#line  519 "VBNET.ATG" 
			newType.BodyStartLocation = t.Location; 
			EnumBody(
#line  520 "VBNET.ATG" 
newType);

#line  522 "VBNET.ATG" 
			BlockEnd();
			
			break;
		}
		case 141: {
			lexer.NextToken();

#line  527 "VBNET.ATG" 
			m.Check(Modifiers.VBInterfacs);
			TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
			newType.StartLocation = m.GetDeclarationLocation(t.Location);
			AddChild(newType);
			BlockStart(newType);
			newType.Type = ClassType.Interface;
			
			Identifier();

#line  534 "VBNET.ATG" 
			newType.Name = t.val; 
			TypeParameterList(
#line  535 "VBNET.ATG" 
newType.Templates);
			EndOfStmt();

#line  537 "VBNET.ATG" 
			newType.BodyStartLocation = t.Location; 
			while (la.kind == 139) {
				InterfaceBase(
#line  538 "VBNET.ATG" 
out baseInterfaces);

#line  538 "VBNET.ATG" 
				newType.BaseTypes.AddRange(baseInterfaces); 
			}
			InterfaceBody(
#line  539 "VBNET.ATG" 
newType);

#line  541 "VBNET.ATG" 
			BlockEnd();
			
			break;
		}
		case 102: {
			lexer.NextToken();

#line  546 "VBNET.ATG" 
			m.Check(Modifiers.VBDelegates);
			DelegateDeclaration delegateDeclr = new DelegateDeclaration(m.Modifier, attributes);
			delegateDeclr.ReturnType = new TypeReference("System.Void", true);
			delegateDeclr.StartLocation = m.GetDeclarationLocation(t.Location);
			List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
			
			if (la.kind == 208) {
				lexer.NextToken();
				Identifier();

#line  553 "VBNET.ATG" 
				delegateDeclr.Name = t.val; 
				TypeParameterList(
#line  554 "VBNET.ATG" 
delegateDeclr.Templates);
				if (la.kind == 36) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
#line  555 "VBNET.ATG" 
p);
					}
					Expect(37);

#line  555 "VBNET.ATG" 
					delegateDeclr.Parameters = p; 
				}
			} else if (la.kind == 126) {
				lexer.NextToken();
				Identifier();

#line  557 "VBNET.ATG" 
				delegateDeclr.Name = t.val; 
				TypeParameterList(
#line  558 "VBNET.ATG" 
delegateDeclr.Templates);
				if (la.kind == 36) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
#line  559 "VBNET.ATG" 
p);
					}
					Expect(37);

#line  559 "VBNET.ATG" 
					delegateDeclr.Parameters = p; 
				}
				if (la.kind == 62) {
					lexer.NextToken();

#line  560 "VBNET.ATG" 
					TypeReference type; 
					TypeName(
#line  560 "VBNET.ATG" 
out type);

#line  560 "VBNET.ATG" 
					delegateDeclr.ReturnType = type; 
				}
			} else SynErr(247);

#line  562 "VBNET.ATG" 
			delegateDeclr.EndLocation = t.EndLocation; 
			EndOfStmt();

#line  565 "VBNET.ATG" 
			AddChild(delegateDeclr);
			
			break;
		}
		default: SynErr(248); break;
		}
	}

	void TypeParameterList(
#line  388 "VBNET.ATG" 
List<TemplateDefinition> templates) {

#line  390 "VBNET.ATG" 
		TemplateDefinition template;
		
		if (
#line  394 "VBNET.ATG" 
la.kind == Tokens.OpenParenthesis && Peek(1).kind == Tokens.Of) {
			lexer.NextToken();
			Expect(168);
			TypeParameter(
#line  395 "VBNET.ATG" 
out template);

#line  397 "VBNET.ATG" 
			if (template != null) templates.Add(template);
			
			while (la.kind == 23) {
				lexer.NextToken();
				TypeParameter(
#line  400 "VBNET.ATG" 
out template);

#line  402 "VBNET.ATG" 
				if (template != null) templates.Add(template);
				
			}
			Expect(37);
		}
	}

	void TypeParameter(
#line  410 "VBNET.ATG" 
out TemplateDefinition template) {
		Identifier();

#line  412 "VBNET.ATG" 
		template = new TemplateDefinition(t.val, null); 
		if (la.kind == 62) {
			TypeParameterConstraints(
#line  413 "VBNET.ATG" 
template);
		}
	}

	void TypeParameterConstraints(
#line  417 "VBNET.ATG" 
TemplateDefinition template) {

#line  419 "VBNET.ATG" 
		TypeReference constraint;
		
		Expect(62);
		if (la.kind == 34) {
			lexer.NextToken();
			TypeParameterConstraint(
#line  425 "VBNET.ATG" 
out constraint);

#line  425 "VBNET.ATG" 
			if (constraint != null) { template.Bases.Add(constraint); } 
			while (la.kind == 23) {
				lexer.NextToken();
				TypeParameterConstraint(
#line  428 "VBNET.ATG" 
out constraint);

#line  428 "VBNET.ATG" 
				if (constraint != null) { template.Bases.Add(constraint); } 
			}
			Expect(35);
		} else if (StartOf(7)) {
			TypeParameterConstraint(
#line  431 "VBNET.ATG" 
out constraint);

#line  431 "VBNET.ATG" 
			if (constraint != null) { template.Bases.Add(constraint); } 
		} else SynErr(249);
	}

	void TypeParameterConstraint(
#line  435 "VBNET.ATG" 
out TypeReference constraint) {

#line  436 "VBNET.ATG" 
		constraint = null; 
		if (la.kind == 83) {
			lexer.NextToken();

#line  437 "VBNET.ATG" 
			constraint = TypeReference.ClassConstraint; 
		} else if (la.kind == 207) {
			lexer.NextToken();

#line  438 "VBNET.ATG" 
			constraint = TypeReference.StructConstraint; 
		} else if (la.kind == 161) {
			lexer.NextToken();

#line  439 "VBNET.ATG" 
			constraint = TypeReference.NewConstraint; 
		} else if (StartOf(8)) {
			TypeName(
#line  440 "VBNET.ATG" 
out constraint);
		} else SynErr(250);
	}

	void ClassBaseType(
#line  785 "VBNET.ATG" 
out TypeReference typeRef) {

#line  787 "VBNET.ATG" 
		typeRef = null;
		
		Expect(139);
		TypeName(
#line  790 "VBNET.ATG" 
out typeRef);
		EndOfStmt();
	}

	void TypeImplementsClause(
#line  1602 "VBNET.ATG" 
out List<TypeReference> baseInterfaces) {

#line  1604 "VBNET.ATG" 
		baseInterfaces = new List<TypeReference>();
		TypeReference type = null;
		
		Expect(135);
		TypeName(
#line  1607 "VBNET.ATG" 
out type);

#line  1609 "VBNET.ATG" 
		if (type != null) baseInterfaces.Add(type);
		
		while (la.kind == 23) {
			lexer.NextToken();
			TypeName(
#line  1612 "VBNET.ATG" 
out type);

#line  1613 "VBNET.ATG" 
			if (type != null) baseInterfaces.Add(type); 
		}
		EndOfStmt();
	}

	void ClassBody(
#line  579 "VBNET.ATG" 
TypeDeclaration newType) {

#line  580 "VBNET.ATG" 
		AttributeSection section; 
		while (la.kind == 1 || la.kind == 22) {
			EndOfStmt();
		}
		while (StartOf(9)) {

#line  583 "VBNET.ATG" 
			List<AttributeSection> attributes = new List<AttributeSection>();
			ModifierList m = new ModifierList();
			
			while (la.kind == 39) {
				AttributeSection(
#line  586 "VBNET.ATG" 
out section);

#line  586 "VBNET.ATG" 
				attributes.Add(section); 
			}
			while (StartOf(10)) {
				MemberModifier(
#line  587 "VBNET.ATG" 
m);
			}
			ClassMemberDecl(
#line  588 "VBNET.ATG" 
m, attributes);
			while (la.kind == 1 || la.kind == 22) {
				EndOfStmt();
			}
		}
	}

	void ModuleBody(
#line  610 "VBNET.ATG" 
TypeDeclaration newType) {

#line  611 "VBNET.ATG" 
		AttributeSection section; 
		while (la.kind == 1 || la.kind == 22) {
			EndOfStmt();
		}
		while (StartOf(9)) {

#line  614 "VBNET.ATG" 
			List<AttributeSection> attributes = new List<AttributeSection>();
			ModifierList m = new ModifierList();
			
			while (la.kind == 39) {
				AttributeSection(
#line  617 "VBNET.ATG" 
out section);

#line  617 "VBNET.ATG" 
				attributes.Add(section); 
			}
			while (StartOf(10)) {
				MemberModifier(
#line  618 "VBNET.ATG" 
m);
			}
			ClassMemberDecl(
#line  619 "VBNET.ATG" 
m, attributes);
			while (la.kind == 1 || la.kind == 22) {
				EndOfStmt();
			}
		}
		Expect(112);
		Expect(154);

#line  622 "VBNET.ATG" 
		newType.EndLocation = t.EndLocation; 
		EndOfStmt();
	}

	void StructureBody(
#line  593 "VBNET.ATG" 
TypeDeclaration newType) {

#line  594 "VBNET.ATG" 
		AttributeSection section; 
		while (la.kind == 1 || la.kind == 22) {
			EndOfStmt();
		}
		while (StartOf(9)) {

#line  597 "VBNET.ATG" 
			List<AttributeSection> attributes = new List<AttributeSection>();
			ModifierList m = new ModifierList();
			
			while (la.kind == 39) {
				AttributeSection(
#line  600 "VBNET.ATG" 
out section);

#line  600 "VBNET.ATG" 
				attributes.Add(section); 
			}
			while (StartOf(10)) {
				MemberModifier(
#line  601 "VBNET.ATG" 
m);
			}
			StructureMemberDecl(
#line  602 "VBNET.ATG" 
m, attributes);
			while (la.kind == 1 || la.kind == 22) {
				EndOfStmt();
			}
		}
		Expect(112);
		Expect(207);

#line  605 "VBNET.ATG" 
		newType.EndLocation = t.EndLocation; 
		EndOfStmt();
	}

	void NonArrayTypeName(
#line  2550 "VBNET.ATG" 
out TypeReference typeref, bool canBeUnbound) {

#line  2552 "VBNET.ATG" 
		string name;
		typeref = null;
		bool isGlobal = false;
		
		if (StartOf(11)) {
			if (la.kind == 129) {
				lexer.NextToken();
				Expect(27);

#line  2557 "VBNET.ATG" 
				isGlobal = true; 
			}
			QualIdentAndTypeArguments(
#line  2558 "VBNET.ATG" 
out typeref, canBeUnbound);

#line  2559 "VBNET.ATG" 
			typeref.IsGlobal = isGlobal; 
			while (la.kind == 27) {
				lexer.NextToken();

#line  2560 "VBNET.ATG" 
				TypeReference nestedTypeRef; 
				QualIdentAndTypeArguments(
#line  2561 "VBNET.ATG" 
out nestedTypeRef, canBeUnbound);

#line  2562 "VBNET.ATG" 
				typeref = new InnerClassTypeReference(typeref, nestedTypeRef.Type, nestedTypeRef.GenericTypes); 
			}
		} else if (la.kind == 167) {
			lexer.NextToken();

#line  2565 "VBNET.ATG" 
			typeref = new TypeReference("System.Object", true); 
			if (la.kind == 32) {
				lexer.NextToken();

#line  2569 "VBNET.ATG" 
				List<TypeReference> typeArguments = new List<TypeReference>(1);
				if (typeref != null) typeArguments.Add(typeref);
				typeref = new TypeReference("System.Nullable", typeArguments) { IsKeyword = true };
				
			}
		} else if (StartOf(12)) {
			PrimitiveTypeName(
#line  2575 "VBNET.ATG" 
out name);

#line  2575 "VBNET.ATG" 
			typeref = new TypeReference(name, true); 
			if (la.kind == 32) {
				lexer.NextToken();

#line  2579 "VBNET.ATG" 
				List<TypeReference> typeArguments = new List<TypeReference>(1);
				if (typeref != null) typeArguments.Add(typeref);
				typeref = new TypeReference("System.Nullable", typeArguments) { IsKeyword = true };
				
			}
		} else SynErr(251);
	}

	void EnumBody(
#line  626 "VBNET.ATG" 
TypeDeclaration newType) {

#line  627 "VBNET.ATG" 
		FieldDeclaration f; 
		while (la.kind == 1 || la.kind == 22) {
			EndOfStmt();
		}
		while (StartOf(13)) {
			EnumMemberDecl(
#line  630 "VBNET.ATG" 
out f);

#line  632 "VBNET.ATG" 
			AddChild(f);
			
			while (la.kind == 1 || la.kind == 22) {
				EndOfStmt();
			}
		}
		Expect(112);
		Expect(114);

#line  636 "VBNET.ATG" 
		newType.EndLocation = t.EndLocation; 
		EndOfStmt();
	}

	void InterfaceBase(
#line  1587 "VBNET.ATG" 
out List<TypeReference> bases) {

#line  1589 "VBNET.ATG" 
		TypeReference type;
		bases = new List<TypeReference>();
		
		Expect(139);
		TypeName(
#line  1593 "VBNET.ATG" 
out type);

#line  1593 "VBNET.ATG" 
		if (type != null) bases.Add(type); 
		while (la.kind == 23) {
			lexer.NextToken();
			TypeName(
#line  1596 "VBNET.ATG" 
out type);

#line  1596 "VBNET.ATG" 
			if (type != null) bases.Add(type); 
		}
		EndOfStmt();
	}

	void InterfaceBody(
#line  640 "VBNET.ATG" 
TypeDeclaration newType) {
		while (la.kind == 1 || la.kind == 22) {
			EndOfStmt();
		}
		while (StartOf(14)) {
			InterfaceMemberDecl();
			while (la.kind == 1 || la.kind == 22) {
				EndOfStmt();
			}
		}
		Expect(112);
		Expect(141);

#line  646 "VBNET.ATG" 
		newType.EndLocation = t.EndLocation; 
		EndOfStmt();
	}

	void FormalParameterList(
#line  2763 "VBNET.ATG" 
List<ParameterDeclarationExpression> parameter) {

#line  2764 "VBNET.ATG" 
		ParameterDeclarationExpression p; 
		FormalParameter(
#line  2766 "VBNET.ATG" 
out p);

#line  2766 "VBNET.ATG" 
		if (p != null) parameter.Add(p); 
		while (la.kind == 23) {
			lexer.NextToken();
			FormalParameter(
#line  2768 "VBNET.ATG" 
out p);

#line  2768 "VBNET.ATG" 
			if (p != null) parameter.Add(p); 
		}
	}

	void MemberModifier(
#line  3502 "VBNET.ATG" 
ModifierList m) {
		switch (la.kind) {
		case 155: {
			lexer.NextToken();

#line  3503 "VBNET.ATG" 
			m.Add(Modifiers.Abstract, t.Location);
			break;
		}
		case 101: {
			lexer.NextToken();

#line  3504 "VBNET.ATG" 
			m.Add(Modifiers.Default, t.Location);
			break;
		}
		case 124: {
			lexer.NextToken();

#line  3505 "VBNET.ATG" 
			m.Add(Modifiers.Internal, t.Location);
			break;
		}
		case 197: {
			lexer.NextToken();

#line  3506 "VBNET.ATG" 
			m.Add(Modifiers.New, t.Location);
			break;
		}
		case 179: {
			lexer.NextToken();

#line  3507 "VBNET.ATG" 
			m.Add(Modifiers.Override, t.Location);
			break;
		}
		case 156: {
			lexer.NextToken();

#line  3508 "VBNET.ATG" 
			m.Add(Modifiers.Abstract, t.Location);
			break;
		}
		case 183: {
			lexer.NextToken();

#line  3509 "VBNET.ATG" 
			m.Add(Modifiers.Private, t.Location);
			break;
		}
		case 185: {
			lexer.NextToken();

#line  3510 "VBNET.ATG" 
			m.Add(Modifiers.Protected, t.Location);
			break;
		}
		case 186: {
			lexer.NextToken();

#line  3511 "VBNET.ATG" 
			m.Add(Modifiers.Public, t.Location);
			break;
		}
		case 165: {
			lexer.NextToken();

#line  3512 "VBNET.ATG" 
			m.Add(Modifiers.Sealed, t.Location);
			break;
		}
		case 166: {
			lexer.NextToken();

#line  3513 "VBNET.ATG" 
			m.Add(Modifiers.Sealed, t.Location);
			break;
		}
		case 198: {
			lexer.NextToken();

#line  3514 "VBNET.ATG" 
			m.Add(Modifiers.Static, t.Location);
			break;
		}
		case 178: {
			lexer.NextToken();

#line  3515 "VBNET.ATG" 
			m.Add(Modifiers.Virtual, t.Location);
			break;
		}
		case 177: {
			lexer.NextToken();

#line  3516 "VBNET.ATG" 
			m.Add(Modifiers.Overloads, t.Location);
			break;
		}
		case 188: {
			lexer.NextToken();

#line  3517 "VBNET.ATG" 
			m.Add(Modifiers.ReadOnly, t.Location);
			break;
		}
		case 233: {
			lexer.NextToken();

#line  3518 "VBNET.ATG" 
			m.Add(Modifiers.WriteOnly, t.Location);
			break;
		}
		case 232: {
			lexer.NextToken();

#line  3519 "VBNET.ATG" 
			m.Add(Modifiers.WithEvents, t.Location);
			break;
		}
		case 104: {
			lexer.NextToken();

#line  3520 "VBNET.ATG" 
			m.Add(Modifiers.Dim, t.Location);
			break;
		}
		case 181: {
			lexer.NextToken();

#line  3521 "VBNET.ATG" 
			m.Add(Modifiers.Partial, t.Location);
			break;
		}
		default: SynErr(252); break;
		}
	}

	void ClassMemberDecl(
#line  781 "VBNET.ATG" 
ModifierList m, List<AttributeSection> attributes) {
		StructureMemberDecl(
#line  782 "VBNET.ATG" 
m, attributes);
	}

	void StructureMemberDecl(
#line  795 "VBNET.ATG" 
ModifierList m, List<AttributeSection> attributes) {

#line  797 "VBNET.ATG" 
		TypeReference type = null;
		List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
		Statement stmt = null;
		List<VariableDeclaration> variableDeclarators = new List<VariableDeclaration>();
		List<TemplateDefinition> templates = new List<TemplateDefinition>();
		
		switch (la.kind) {
		case 83: case 102: case 114: case 141: case 154: case 207: {
			NonModuleDeclaration(
#line  804 "VBNET.ATG" 
m, attributes);
			break;
		}
		case 208: {
			lexer.NextToken();

#line  808 "VBNET.ATG" 
			Location startPos = t.Location;
			
			if (StartOf(4)) {

#line  812 "VBNET.ATG" 
				string name = String.Empty;
				MethodDeclaration methodDeclaration; List<string> handlesClause = null;
				List<InterfaceImplementation> implementsClause = null;
				
				Identifier();

#line  818 "VBNET.ATG" 
				name = t.val;
				m.Check(Modifiers.VBMethods);
				
				TypeParameterList(
#line  821 "VBNET.ATG" 
templates);
				if (la.kind == 36) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
#line  822 "VBNET.ATG" 
p);
					}
					Expect(37);
				}
				if (la.kind == 133 || la.kind == 135) {
					if (la.kind == 135) {
						ImplementsClause(
#line  825 "VBNET.ATG" 
out implementsClause);
					} else {
						HandlesClause(
#line  827 "VBNET.ATG" 
out handlesClause);
					}
				}

#line  830 "VBNET.ATG" 
				Location endLocation = t.EndLocation; 
				if (
#line  833 "VBNET.ATG" 
IsMustOverride(m)) {
					EndOfStmt();

#line  836 "VBNET.ATG" 
					methodDeclaration = new MethodDeclaration {
					Name = name, Modifier = m.Modifier, Parameters = p, Attributes = attributes,
					StartLocation = m.GetDeclarationLocation(startPos), EndLocation = endLocation,
					TypeReference = new TypeReference("System.Void", true),
					Templates = templates,
					HandlesClause = handlesClause,
					InterfaceImplementations = implementsClause
					};
					AddChild(methodDeclaration);
					
				} else if (la.kind == 1) {
					lexer.NextToken();

#line  849 "VBNET.ATG" 
					methodDeclaration = new MethodDeclaration {
					Name = name, Modifier = m.Modifier, Parameters = p, Attributes = attributes,
					StartLocation = m.GetDeclarationLocation(startPos), EndLocation = endLocation,
					TypeReference = new TypeReference("System.Void", true),
					Templates = templates,
					HandlesClause = handlesClause,
					InterfaceImplementations = implementsClause
					};
					AddChild(methodDeclaration);
					

#line  860 "VBNET.ATG" 
					if (ParseMethodBodies) { 
					Block(
#line  861 "VBNET.ATG" 
out stmt);
					Expect(112);
					Expect(208);

#line  863 "VBNET.ATG" 
					} else {
					// don't parse method body
					lexer.SkipCurrentBlock(Tokens.Sub); stmt = new BlockStatement();
					  }
					

#line  869 "VBNET.ATG" 
					methodDeclaration.Body  = (BlockStatement)stmt; 

#line  870 "VBNET.ATG" 
					methodDeclaration.Body.EndLocation = t.EndLocation; 
					EndOfStmt();
				} else SynErr(253);
			} else if (la.kind == 161) {
				lexer.NextToken();
				if (la.kind == 36) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
#line  874 "VBNET.ATG" 
p);
					}
					Expect(37);
				}

#line  875 "VBNET.ATG" 
				m.Check(Modifiers.Constructors); 

#line  876 "VBNET.ATG" 
				Location constructorEndLocation = t.EndLocation; 
				Expect(1);

#line  879 "VBNET.ATG" 
				if (ParseMethodBodies) { 
				Block(
#line  880 "VBNET.ATG" 
out stmt);
				Expect(112);
				Expect(208);

#line  882 "VBNET.ATG" 
				} else {
				// don't parse method body
				lexer.SkipCurrentBlock(Tokens.Sub); stmt = new BlockStatement();
				  }
				

#line  888 "VBNET.ATG" 
				Location endLocation = t.EndLocation; 
				EndOfStmt();

#line  891 "VBNET.ATG" 
				ConstructorDeclaration cd = new ConstructorDeclaration("New", m.Modifier, p, attributes);
				cd.StartLocation = m.GetDeclarationLocation(startPos);
				cd.EndLocation   = constructorEndLocation;
				cd.Body = (BlockStatement)stmt;
				cd.Body.EndLocation   = endLocation;
				AddChild(cd);
				
			} else SynErr(254);
			break;
		}
		case 126: {
			lexer.NextToken();

#line  903 "VBNET.ATG" 
			m.Check(Modifiers.VBMethods);
			string name = String.Empty;
			Location startPos = t.Location;
			MethodDeclaration methodDeclaration;List<string> handlesClause = null;
			List<InterfaceImplementation> implementsClause = null;
			AttributeSection returnTypeAttributeSection = null;
			
			Identifier();

#line  910 "VBNET.ATG" 
			name = t.val; 
			TypeParameterList(
#line  911 "VBNET.ATG" 
templates);
			if (la.kind == 36) {
				lexer.NextToken();
				if (StartOf(6)) {
					FormalParameterList(
#line  912 "VBNET.ATG" 
p);
				}
				Expect(37);
			}
			if (la.kind == 62) {
				lexer.NextToken();
				while (la.kind == 39) {
					AttributeSection(
#line  914 "VBNET.ATG" 
out returnTypeAttributeSection);

#line  916 "VBNET.ATG" 
					if (returnTypeAttributeSection != null) {
					returnTypeAttributeSection.AttributeTarget = "return";
					attributes.Add(returnTypeAttributeSection);
					}
					
				}
				TypeName(
#line  922 "VBNET.ATG" 
out type);
			}

#line  924 "VBNET.ATG" 
			if(type == null) {
			type = new TypeReference("System.Object", true);
			}
			
			if (la.kind == 133 || la.kind == 135) {
				if (la.kind == 135) {
					ImplementsClause(
#line  930 "VBNET.ATG" 
out implementsClause);
				} else {
					HandlesClause(
#line  932 "VBNET.ATG" 
out handlesClause);
				}
			}

#line  935 "VBNET.ATG" 
			Location endLocation = t.EndLocation; 
			if (
#line  938 "VBNET.ATG" 
IsMustOverride(m)) {
				EndOfStmt();

#line  941 "VBNET.ATG" 
				methodDeclaration = new MethodDeclaration {
				Name = name, Modifier = m.Modifier, TypeReference = type,
				Parameters = p, Attributes = attributes,
				StartLocation = m.GetDeclarationLocation(startPos),
				EndLocation   = endLocation,
				HandlesClause = handlesClause,
				Templates     = templates,
				InterfaceImplementations = implementsClause
				};
				
				AddChild(methodDeclaration);
				
			} else if (la.kind == 1) {
				lexer.NextToken();

#line  956 "VBNET.ATG" 
				methodDeclaration = new MethodDeclaration {
				Name = name, Modifier = m.Modifier, TypeReference = type,
				Parameters = p, Attributes = attributes,
				StartLocation = m.GetDeclarationLocation(startPos),
				EndLocation   = endLocation,
				Templates     = templates,
				HandlesClause = handlesClause,
				InterfaceImplementations = implementsClause
				};
				
				AddChild(methodDeclaration);
				
				if (ParseMethodBodies) { 
				Block(
#line  969 "VBNET.ATG" 
out stmt);
				Expect(112);
				Expect(126);

#line  971 "VBNET.ATG" 
				} else {
				// don't parse method body
				lexer.SkipCurrentBlock(Tokens.Function); stmt = new BlockStatement();
				}
				methodDeclaration.Body = (BlockStatement)stmt;
				methodDeclaration.Body.StartLocation = methodDeclaration.EndLocation;
				methodDeclaration.Body.EndLocation   = t.EndLocation;
				
				EndOfStmt();
			} else SynErr(255);
			break;
		}
		case 100: {
			lexer.NextToken();

#line  985 "VBNET.ATG" 
			m.Check(Modifiers.VBExternalMethods);
			Location startPos = t.Location;
			CharsetModifier charsetModifer = CharsetModifier.None;
			string library = String.Empty;
			string alias = null;
			string name = String.Empty;
			
			if (StartOf(15)) {
				Charset(
#line  992 "VBNET.ATG" 
out charsetModifer);
			}
			if (la.kind == 208) {
				lexer.NextToken();
				Identifier();

#line  995 "VBNET.ATG" 
				name = t.val; 
				Expect(148);
				Expect(3);

#line  996 "VBNET.ATG" 
				library = t.literalValue as string; 
				if (la.kind == 58) {
					lexer.NextToken();
					Expect(3);

#line  997 "VBNET.ATG" 
					alias = t.literalValue as string; 
				}
				if (la.kind == 36) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
#line  998 "VBNET.ATG" 
p);
					}
					Expect(37);
				}
				EndOfStmt();

#line  1001 "VBNET.ATG" 
				DeclareDeclaration declareDeclaration = new DeclareDeclaration(name, m.Modifier, null, p, attributes, library, alias, charsetModifer);
				declareDeclaration.StartLocation = m.GetDeclarationLocation(startPos);
				declareDeclaration.EndLocation   = t.EndLocation;
				AddChild(declareDeclaration);
				
			} else if (la.kind == 126) {
				lexer.NextToken();
				Identifier();

#line  1008 "VBNET.ATG" 
				name = t.val; 
				Expect(148);
				Expect(3);

#line  1009 "VBNET.ATG" 
				library = t.literalValue as string; 
				if (la.kind == 58) {
					lexer.NextToken();
					Expect(3);

#line  1010 "VBNET.ATG" 
					alias = t.literalValue as string; 
				}
				if (la.kind == 36) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
#line  1011 "VBNET.ATG" 
p);
					}
					Expect(37);
				}
				if (la.kind == 62) {
					lexer.NextToken();
					TypeName(
#line  1012 "VBNET.ATG" 
out type);
				}
				EndOfStmt();

#line  1015 "VBNET.ATG" 
				DeclareDeclaration declareDeclaration = new DeclareDeclaration(name, m.Modifier, type, p, attributes, library, alias, charsetModifer);
				declareDeclaration.StartLocation = m.GetDeclarationLocation(startPos);
				declareDeclaration.EndLocation   = t.EndLocation;
				AddChild(declareDeclaration);
				
			} else SynErr(256);
			break;
		}
		case 118: {
			lexer.NextToken();

#line  1025 "VBNET.ATG" 
			m.Check(Modifiers.VBEvents);
			Location startPos = t.Location;
			EventDeclaration eventDeclaration;
			string name = String.Empty;
			List<InterfaceImplementation> implementsClause = null;
			
			Identifier();

#line  1031 "VBNET.ATG" 
			name= t.val; 
			if (la.kind == 62) {
				lexer.NextToken();
				TypeName(
#line  1033 "VBNET.ATG" 
out type);
			} else if (StartOf(16)) {
				if (la.kind == 36) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
#line  1035 "VBNET.ATG" 
p);
					}
					Expect(37);
				}
			} else SynErr(257);
			if (la.kind == 135) {
				ImplementsClause(
#line  1037 "VBNET.ATG" 
out implementsClause);
			}

#line  1039 "VBNET.ATG" 
			eventDeclaration = new EventDeclaration {
			Name = name, TypeReference = type, Modifier = m.Modifier, 
			Parameters = p, Attributes = attributes, InterfaceImplementations = implementsClause,
			StartLocation = m.GetDeclarationLocation(startPos),
			EndLocation = t.EndLocation
			};
			AddChild(eventDeclaration);
			
			EndOfStmt();
			break;
		}
		case 2: case 57: case 61: case 63: case 64: case 65: case 66: case 69: case 86: case 103: case 106: case 115: case 120: case 125: case 132: case 138: case 142: case 145: case 169: case 175: case 182: case 201: case 210: case 211: case 221: case 222: case 228: {

#line  1050 "VBNET.ATG" 
			m.Check(Modifiers.Fields);
			FieldDeclaration fd = new FieldDeclaration(attributes, null, m.Modifier);
			
			IdentifierForFieldDeclaration();

#line  1053 "VBNET.ATG" 
			string name = t.val; 

#line  1054 "VBNET.ATG" 
			fd.StartLocation = m.GetDeclarationLocation(t.Location); 
			VariableDeclaratorPartAfterIdentifier(
#line  1056 "VBNET.ATG" 
variableDeclarators, name);
			while (la.kind == 23) {
				lexer.NextToken();
				VariableDeclarator(
#line  1057 "VBNET.ATG" 
variableDeclarators);
			}
			EndOfStmt();

#line  1060 "VBNET.ATG" 
			fd.EndLocation = t.EndLocation;
			fd.Fields = variableDeclarators;
			AddChild(fd);
			
			break;
		}
		case 87: {

#line  1065 "VBNET.ATG" 
			m.Check(Modifiers.Fields); 
			lexer.NextToken();

#line  1066 "VBNET.ATG" 
			m.Add(Modifiers.Const, t.Location);  

#line  1068 "VBNET.ATG" 
			FieldDeclaration fd = new FieldDeclaration(attributes, type, m.Modifier);
			fd.StartLocation = m.GetDeclarationLocation(t.Location);
			List<VariableDeclaration> constantDeclarators = new List<VariableDeclaration>();
			
			ConstantDeclarator(
#line  1072 "VBNET.ATG" 
constantDeclarators);
			while (la.kind == 23) {
				lexer.NextToken();
				ConstantDeclarator(
#line  1073 "VBNET.ATG" 
constantDeclarators);
			}

#line  1075 "VBNET.ATG" 
			fd.Fields = constantDeclarators;
			fd.EndLocation = t.Location;
			
			EndOfStmt();

#line  1080 "VBNET.ATG" 
			fd.EndLocation = t.EndLocation;
			AddChild(fd);
			
			break;
		}
		case 184: {
			lexer.NextToken();

#line  1086 "VBNET.ATG" 
			m.Check(Modifiers.VBProperties);
			Location startPos = t.Location;
			List<InterfaceImplementation> implementsClause = null;
			AttributeSection returnTypeAttributeSection = null;
			Expression initializer = null;
			
			Identifier();

#line  1092 "VBNET.ATG" 
			string propertyName = t.val; 
			if (la.kind == 36) {
				lexer.NextToken();
				if (StartOf(6)) {
					FormalParameterList(
#line  1093 "VBNET.ATG" 
p);
				}
				Expect(37);
			}
			if (la.kind == 62) {
				lexer.NextToken();
				while (la.kind == 39) {
					AttributeSection(
#line  1096 "VBNET.ATG" 
out returnTypeAttributeSection);

#line  1098 "VBNET.ATG" 
					if (returnTypeAttributeSection != null) {
					returnTypeAttributeSection.AttributeTarget = "return";
					attributes.Add(returnTypeAttributeSection);
					}
					
				}
				if (
#line  1105 "VBNET.ATG" 
IsNewExpression()) {
					ObjectCreateExpression(
#line  1105 "VBNET.ATG" 
out initializer);

#line  1107 "VBNET.ATG" 
					if (initializer is ObjectCreateExpression) {
					type = ((ObjectCreateExpression)initializer).CreateType.Clone();
					} else {
						type = ((ArrayCreateExpression)initializer).CreateType.Clone();
					}
					
				} else if (StartOf(8)) {
					TypeName(
#line  1114 "VBNET.ATG" 
out type);
				} else SynErr(258);
			}
			if (la.kind == 21) {
				lexer.NextToken();
				Expr(
#line  1117 "VBNET.ATG" 
out initializer);
			}
			if (la.kind == 135) {
				ImplementsClause(
#line  1118 "VBNET.ATG" 
out implementsClause);
			}
			EndOfStmt();
			if (
#line  1122 "VBNET.ATG" 
IsMustOverride(m) || IsAutomaticProperty()) {

#line  1124 "VBNET.ATG" 
				PropertyDeclaration pDecl = new PropertyDeclaration(propertyName, type, m.Modifier, attributes);
				pDecl.StartLocation = m.GetDeclarationLocation(startPos);
				pDecl.EndLocation   = t.Location;
				pDecl.TypeReference = type;
				pDecl.InterfaceImplementations = implementsClause;
				pDecl.Parameters = p;
				if (initializer != null)
					pDecl.Initializer = initializer;
				AddChild(pDecl);
				
			} else if (StartOf(17)) {

#line  1136 "VBNET.ATG" 
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
#line  1146 "VBNET.ATG" 
out getRegion, out setRegion);
				Expect(112);
				Expect(184);
				EndOfStmt();

#line  1150 "VBNET.ATG" 
				pDecl.GetRegion = getRegion;
				pDecl.SetRegion = setRegion;
				pDecl.BodyEnd = t.Location; // t = EndOfStmt; not "Property"
				AddChild(pDecl);
				
			} else SynErr(259);
			break;
		}
		case 97: {
			lexer.NextToken();

#line  1157 "VBNET.ATG" 
			Location startPos = t.Location; 
			Expect(118);

#line  1159 "VBNET.ATG" 
			m.Check(Modifiers.VBCustomEvents);
			EventAddRemoveRegion eventAccessorDeclaration;
			EventAddRegion addHandlerAccessorDeclaration = null;
			EventRemoveRegion removeHandlerAccessorDeclaration = null;
			EventRaiseRegion raiseEventAccessorDeclaration = null;
			List<InterfaceImplementation> implementsClause = null;
			
			Identifier();

#line  1166 "VBNET.ATG" 
			string customEventName = t.val; 
			Expect(62);
			TypeName(
#line  1167 "VBNET.ATG" 
out type);
			if (la.kind == 135) {
				ImplementsClause(
#line  1168 "VBNET.ATG" 
out implementsClause);
			}
			EndOfStmt();
			while (StartOf(18)) {
				EventAccessorDeclaration(
#line  1171 "VBNET.ATG" 
out eventAccessorDeclaration);

#line  1173 "VBNET.ATG" 
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
			Expect(112);
			Expect(118);
			EndOfStmt();

#line  1189 "VBNET.ATG" 
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
			AddChild(decl);
			
			break;
		}
		case 160: case 171: case 230: {

#line  1215 "VBNET.ATG" 
			ConversionType opConversionType = ConversionType.None; 
			if (la.kind == 160 || la.kind == 230) {
				if (la.kind == 230) {
					lexer.NextToken();

#line  1216 "VBNET.ATG" 
					opConversionType = ConversionType.Implicit; 
				} else {
					lexer.NextToken();

#line  1217 "VBNET.ATG" 
					opConversionType = ConversionType.Explicit;
				}
			}
			Expect(171);

#line  1220 "VBNET.ATG" 
			m.Check(Modifiers.VBOperators);
			Location startPos = t.Location;
			TypeReference returnType = NullTypeReference.Instance;
			TypeReference operandType = NullTypeReference.Instance;
			string operandName;
			OverloadableOperatorType operatorType;
			AttributeSection section;
			List<ParameterDeclarationExpression> parameters = new List<ParameterDeclarationExpression>();
			
			OverloadableOperator(
#line  1229 "VBNET.ATG" 
out operatorType);
			Expect(36);
			if (la.kind == 71) {
				lexer.NextToken();
			}
			Identifier();

#line  1230 "VBNET.ATG" 
			operandName = t.val; 
			if (la.kind == 62) {
				lexer.NextToken();
				TypeName(
#line  1231 "VBNET.ATG" 
out operandType);
			}

#line  1232 "VBNET.ATG" 
			parameters.Add(new ParameterDeclarationExpression(operandType, operandName, ParameterModifiers.In)); 
			while (la.kind == 23) {
				lexer.NextToken();
				if (la.kind == 71) {
					lexer.NextToken();
				}
				Identifier();

#line  1236 "VBNET.ATG" 
				operandName = t.val; 
				if (la.kind == 62) {
					lexer.NextToken();
					TypeName(
#line  1237 "VBNET.ATG" 
out operandType);
				}

#line  1238 "VBNET.ATG" 
				parameters.Add(new ParameterDeclarationExpression(operandType, operandName, ParameterModifiers.In)); 
			}
			Expect(37);

#line  1241 "VBNET.ATG" 
			Location endPos = t.EndLocation; 
			if (la.kind == 62) {
				lexer.NextToken();
				while (la.kind == 39) {
					AttributeSection(
#line  1242 "VBNET.ATG" 
out section);

#line  1243 "VBNET.ATG" 
					if (section != null) {
					section.AttributeTarget = "return";
					attributes.Add(section);
					} 
				}
				TypeName(
#line  1247 "VBNET.ATG" 
out returnType);

#line  1247 "VBNET.ATG" 
				endPos = t.EndLocation; 
			}
			Expect(1);
			Block(
#line  1249 "VBNET.ATG" 
out stmt);
			Expect(112);
			Expect(171);
			EndOfStmt();

#line  1251 "VBNET.ATG" 
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
			AddChild(operatorDeclaration);
			
			break;
		}
		default: SynErr(260); break;
		}
	}

	void EnumMemberDecl(
#line  763 "VBNET.ATG" 
out FieldDeclaration f) {

#line  765 "VBNET.ATG" 
		Expression expr = null;List<AttributeSection> attributes = new List<AttributeSection>();
		AttributeSection section = null;
		VariableDeclaration varDecl = null;
		
		while (la.kind == 39) {
			AttributeSection(
#line  769 "VBNET.ATG" 
out section);

#line  769 "VBNET.ATG" 
			attributes.Add(section); 
		}
		Identifier();

#line  772 "VBNET.ATG" 
		f = new FieldDeclaration(attributes);
		varDecl = new VariableDeclaration(t.val);
		f.Fields.Add(varDecl);
		f.StartLocation = varDecl.StartLocation = t.Location;
		
		if (la.kind == 21) {
			lexer.NextToken();
			Expr(
#line  777 "VBNET.ATG" 
out expr);

#line  777 "VBNET.ATG" 
			varDecl.Initializer = expr; 
		}
		EndOfStmt();
	}

	void InterfaceMemberDecl() {

#line  654 "VBNET.ATG" 
		TypeReference type =null;
		List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
		List<TemplateDefinition> templates = new List<TemplateDefinition>();
		AttributeSection section, returnTypeAttributeSection = null;
		ModifierList mod = new ModifierList();
		List<AttributeSection> attributes = new List<AttributeSection>();
		string name;
		
		if (StartOf(19)) {
			while (la.kind == 39) {
				AttributeSection(
#line  662 "VBNET.ATG" 
out section);

#line  662 "VBNET.ATG" 
				attributes.Add(section); 
			}
			while (StartOf(10)) {
				MemberModifier(
#line  665 "VBNET.ATG" 
mod);
			}
			if (la.kind == 118) {
				lexer.NextToken();

#line  669 "VBNET.ATG" 
				mod.Check(Modifiers.VBInterfaceEvents);
				Location startLocation = t.Location;
				
				Identifier();

#line  672 "VBNET.ATG" 
				name = t.val; 
				if (la.kind == 36) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
#line  673 "VBNET.ATG" 
p);
					}
					Expect(37);
				}
				if (la.kind == 62) {
					lexer.NextToken();
					TypeName(
#line  674 "VBNET.ATG" 
out type);
				}
				EndOfStmt();

#line  677 "VBNET.ATG" 
				EventDeclaration ed = new EventDeclaration {
				Name = name, TypeReference = type, Modifier = mod.Modifier,
				Parameters = p, Attributes = attributes,
				StartLocation = startLocation, EndLocation = t.EndLocation
				};
				AddChild(ed);
				
			} else if (la.kind == 208) {
				lexer.NextToken();

#line  687 "VBNET.ATG" 
				Location startLocation =  t.Location;
				mod.Check(Modifiers.VBInterfaceMethods);
				
				Identifier();

#line  690 "VBNET.ATG" 
				name = t.val; 
				TypeParameterList(
#line  691 "VBNET.ATG" 
templates);
				if (la.kind == 36) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
#line  692 "VBNET.ATG" 
p);
					}
					Expect(37);
				}
				EndOfStmt();

#line  695 "VBNET.ATG" 
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
				AddChild(md);
				
			} else if (la.kind == 126) {
				lexer.NextToken();

#line  710 "VBNET.ATG" 
				mod.Check(Modifiers.VBInterfaceMethods);
				Location startLocation = t.Location;
				
				Identifier();

#line  713 "VBNET.ATG" 
				name = t.val; 
				TypeParameterList(
#line  714 "VBNET.ATG" 
templates);
				if (la.kind == 36) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
#line  715 "VBNET.ATG" 
p);
					}
					Expect(37);
				}
				if (la.kind == 62) {
					lexer.NextToken();
					while (la.kind == 39) {
						AttributeSection(
#line  716 "VBNET.ATG" 
out returnTypeAttributeSection);
					}
					TypeName(
#line  716 "VBNET.ATG" 
out type);
				}

#line  718 "VBNET.ATG" 
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
				AddChild(md);
				
				EndOfStmt();
			} else if (la.kind == 184) {
				lexer.NextToken();

#line  738 "VBNET.ATG" 
				Location startLocation = t.Location;
				mod.Check(Modifiers.VBInterfaceProperties);
				
				Identifier();

#line  741 "VBNET.ATG" 
				name = t.val;  
				if (la.kind == 36) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
#line  742 "VBNET.ATG" 
p);
					}
					Expect(37);
				}
				if (la.kind == 62) {
					lexer.NextToken();
					TypeName(
#line  743 "VBNET.ATG" 
out type);
				}

#line  745 "VBNET.ATG" 
				if(type == null) {
				type = new TypeReference("System.Object", true);
				}
				
				EndOfStmt();

#line  751 "VBNET.ATG" 
				PropertyDeclaration pd = new PropertyDeclaration(name, type, mod.Modifier, attributes);
				pd.Parameters = p;
				pd.EndLocation = t.EndLocation;
				pd.StartLocation = startLocation;
				AddChild(pd);
				
			} else SynErr(261);
		} else if (StartOf(20)) {
			NonModuleDeclaration(
#line  759 "VBNET.ATG" 
mod, attributes);
		} else SynErr(262);
	}

	void Expr(
#line  1646 "VBNET.ATG" 
out Expression expr) {

#line  1647 "VBNET.ATG" 
		expr = null; 
		if (
#line  1648 "VBNET.ATG" 
IsQueryExpression() ) {
			QueryExpr(
#line  1649 "VBNET.ATG" 
out expr);
		} else if (la.kind == 126 || la.kind == 208) {
			LambdaExpr(
#line  1650 "VBNET.ATG" 
out expr);
		} else if (StartOf(21)) {
			DisjunctionExpr(
#line  1651 "VBNET.ATG" 
out expr);
		} else SynErr(263);
	}

	void ImplementsClause(
#line  1619 "VBNET.ATG" 
out List<InterfaceImplementation> baseInterfaces) {

#line  1621 "VBNET.ATG" 
		baseInterfaces = new List<InterfaceImplementation>();
		TypeReference type = null;
		string memberName = null;
		
		Expect(135);
		NonArrayTypeName(
#line  1626 "VBNET.ATG" 
out type, false);

#line  1627 "VBNET.ATG" 
		if (type != null) memberName = TypeReference.StripLastIdentifierFromType(ref type); 

#line  1628 "VBNET.ATG" 
		baseInterfaces.Add(new InterfaceImplementation(type, memberName)); 
		while (la.kind == 23) {
			lexer.NextToken();
			NonArrayTypeName(
#line  1630 "VBNET.ATG" 
out type, false);

#line  1631 "VBNET.ATG" 
			if (type != null) memberName = TypeReference.StripLastIdentifierFromType(ref type); 

#line  1632 "VBNET.ATG" 
			baseInterfaces.Add(new InterfaceImplementation(type, memberName)); 
		}
	}

	void HandlesClause(
#line  1577 "VBNET.ATG" 
out List<string> handlesClause) {

#line  1579 "VBNET.ATG" 
		handlesClause = new List<string>();
		string name;
		
		Expect(133);
		EventMemberSpecifier(
#line  1582 "VBNET.ATG" 
out name);

#line  1582 "VBNET.ATG" 
		if (name != null) handlesClause.Add(name); 
		while (la.kind == 23) {
			lexer.NextToken();
			EventMemberSpecifier(
#line  1583 "VBNET.ATG" 
out name);

#line  1583 "VBNET.ATG" 
			if (name != null) handlesClause.Add(name); 
		}
	}

	void Block(
#line  2808 "VBNET.ATG" 
out Statement stmt) {

#line  2811 "VBNET.ATG" 
		BlockStatement blockStmt = new BlockStatement();
		/* in snippet parsing mode, t might be null */
		if (t != null) blockStmt.StartLocation = t.EndLocation;
		BlockStart(blockStmt);
		
		while (StartOf(22) || 
#line  2817 "VBNET.ATG" 
IsEndStmtAhead()) {
			if (
#line  2817 "VBNET.ATG" 
IsEndStmtAhead()) {
				Expect(112);
				EndOfStmt();

#line  2817 "VBNET.ATG" 
				AddChild(new EndStatement()); 
			} else {
				Statement();
				EndOfStmt();
			}
		}

#line  2822 "VBNET.ATG" 
		stmt = blockStmt;
		if (t != null) blockStmt.EndLocation = t.EndLocation;
		BlockEnd();
		
	}

	void Charset(
#line  1569 "VBNET.ATG" 
out CharsetModifier charsetModifier) {

#line  1570 "VBNET.ATG" 
		charsetModifier = CharsetModifier.None; 
		if (la.kind == 126 || la.kind == 208) {
		} else if (la.kind == 61) {
			lexer.NextToken();

#line  1571 "VBNET.ATG" 
			charsetModifier = CharsetModifier.Ansi; 
		} else if (la.kind == 65) {
			lexer.NextToken();

#line  1572 "VBNET.ATG" 
			charsetModifier = CharsetModifier.Auto; 
		} else if (la.kind == 221) {
			lexer.NextToken();

#line  1573 "VBNET.ATG" 
			charsetModifier = CharsetModifier.Unicode; 
		} else SynErr(264);
	}

	void IdentifierForFieldDeclaration() {
		switch (la.kind) {
		case 2: {
			lexer.NextToken();
			break;
		}
		case 57: {
			lexer.NextToken();
			break;
		}
		case 61: {
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
		case 65: {
			lexer.NextToken();
			break;
		}
		case 66: {
			lexer.NextToken();
			break;
		}
		case 69: {
			lexer.NextToken();
			break;
		}
		case 86: {
			lexer.NextToken();
			break;
		}
		case 103: {
			lexer.NextToken();
			break;
		}
		case 106: {
			lexer.NextToken();
			break;
		}
		case 115: {
			lexer.NextToken();
			break;
		}
		case 120: {
			lexer.NextToken();
			break;
		}
		case 125: {
			lexer.NextToken();
			break;
		}
		case 132: {
			lexer.NextToken();
			break;
		}
		case 138: {
			lexer.NextToken();
			break;
		}
		case 142: {
			lexer.NextToken();
			break;
		}
		case 145: {
			lexer.NextToken();
			break;
		}
		case 169: {
			lexer.NextToken();
			break;
		}
		case 175: {
			lexer.NextToken();
			break;
		}
		case 182: {
			lexer.NextToken();
			break;
		}
		case 201: {
			lexer.NextToken();
			break;
		}
		case 210: {
			lexer.NextToken();
			break;
		}
		case 211: {
			lexer.NextToken();
			break;
		}
		case 221: {
			lexer.NextToken();
			break;
		}
		case 222: {
			lexer.NextToken();
			break;
		}
		case 228: {
			lexer.NextToken();
			break;
		}
		default: SynErr(265); break;
		}
	}

	void VariableDeclaratorPartAfterIdentifier(
#line  1454 "VBNET.ATG" 
List<VariableDeclaration> fieldDeclaration, string name) {

#line  1456 "VBNET.ATG" 
		Expression expr = null;
		TypeReference type = null;
		ArrayList rank = null;
		List<Expression> dimension = null;
		Location startLocation = t.Location;
		
		if (
#line  1462 "VBNET.ATG" 
IsSize() && !IsDims()) {
			ArrayInitializationModifier(
#line  1462 "VBNET.ATG" 
out dimension);
		}
		if (
#line  1463 "VBNET.ATG" 
IsDims()) {
			ArrayNameModifier(
#line  1463 "VBNET.ATG" 
out rank);
		}
		if (
#line  1465 "VBNET.ATG" 
IsObjectCreation()) {
			Expect(62);
			ObjectCreateExpression(
#line  1465 "VBNET.ATG" 
out expr);

#line  1467 "VBNET.ATG" 
			if (expr is ObjectCreateExpression) {
			type = ((ObjectCreateExpression)expr).CreateType.Clone();
			} else {
				type = ((ArrayCreateExpression)expr).CreateType.Clone();
			}
			
		} else if (StartOf(23)) {
			if (la.kind == 62) {
				lexer.NextToken();
				TypeName(
#line  1474 "VBNET.ATG" 
out type);

#line  1476 "VBNET.ATG" 
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

#line  1488 "VBNET.ATG" 
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
			
			if (la.kind == 21) {
				lexer.NextToken();
				Expr(
#line  1511 "VBNET.ATG" 
out expr);
			}
		} else SynErr(266);

#line  1514 "VBNET.ATG" 
		VariableDeclaration varDecl = new VariableDeclaration(name, expr, type);
		varDecl.StartLocation = startLocation;
		varDecl.EndLocation = t.Location;
		fieldDeclaration.Add(varDecl);
		
	}

	void VariableDeclarator(
#line  1448 "VBNET.ATG" 
List<VariableDeclaration> fieldDeclaration) {
		Identifier();

#line  1450 "VBNET.ATG" 
		string name = t.val; 
		VariableDeclaratorPartAfterIdentifier(
#line  1451 "VBNET.ATG" 
fieldDeclaration, name);
	}

	void ConstantDeclarator(
#line  1429 "VBNET.ATG" 
List<VariableDeclaration> constantDeclaration) {

#line  1431 "VBNET.ATG" 
		Expression expr = null;
		TypeReference type = null;
		string name = String.Empty;
		Location location;
		
		Identifier();

#line  1436 "VBNET.ATG" 
		name = t.val; location = t.Location; 
		if (la.kind == 62) {
			lexer.NextToken();
			TypeName(
#line  1437 "VBNET.ATG" 
out type);
		}
		Expect(21);
		Expr(
#line  1438 "VBNET.ATG" 
out expr);

#line  1440 "VBNET.ATG" 
		VariableDeclaration f = new VariableDeclaration(name, expr);
		f.TypeReference = type;
		f.StartLocation = location;
		constantDeclaration.Add(f);
		
	}

	void ObjectCreateExpression(
#line  1976 "VBNET.ATG" 
out Expression oce) {

#line  1978 "VBNET.ATG" 
		TypeReference type = null;
		CollectionInitializerExpression initializer = null;
		List<Expression> arguments = null;
		ArrayList dimensions = null;
		oce = null;
		bool canBeNormal; bool canBeReDim;
		
		Expect(161);
		if (StartOf(8)) {
			NonArrayTypeName(
#line  1986 "VBNET.ATG" 
out type, false);
			if (la.kind == 36) {
				lexer.NextToken();
				NormalOrReDimArgumentList(
#line  1987 "VBNET.ATG" 
out arguments, out canBeNormal, out canBeReDim);
				Expect(37);
				if (la.kind == 34 || 
#line  1988 "VBNET.ATG" 
la.kind == Tokens.OpenParenthesis) {
					if (
#line  1988 "VBNET.ATG" 
la.kind == Tokens.OpenParenthesis) {
						ArrayTypeModifiers(
#line  1989 "VBNET.ATG" 
out dimensions);
						CollectionInitializer(
#line  1990 "VBNET.ATG" 
out initializer);
					} else {
						CollectionInitializer(
#line  1991 "VBNET.ATG" 
out initializer);
					}
				}

#line  1993 "VBNET.ATG" 
				if (canBeReDim && !canBeNormal && initializer == null) initializer = new CollectionInitializerExpression(); 
			}
		}

#line  1997 "VBNET.ATG" 
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
		
		if (la.kind == 125 || la.kind == 231) {
			if (la.kind == 231) {

#line  2012 "VBNET.ATG" 
				MemberInitializerExpression memberInitializer = null;
				
				lexer.NextToken();

#line  2016 "VBNET.ATG" 
				CollectionInitializerExpression memberInitializers = new CollectionInitializerExpression();
				memberInitializers.StartLocation = la.Location;
				
				Expect(34);
				MemberInitializer(
#line  2020 "VBNET.ATG" 
out memberInitializer);

#line  2021 "VBNET.ATG" 
				memberInitializers.CreateExpressions.Add(memberInitializer); 
				while (la.kind == 23) {
					lexer.NextToken();
					MemberInitializer(
#line  2023 "VBNET.ATG" 
out memberInitializer);

#line  2024 "VBNET.ATG" 
					memberInitializers.CreateExpressions.Add(memberInitializer); 
				}
				Expect(35);

#line  2028 "VBNET.ATG" 
				memberInitializers.EndLocation = t.Location;
				if(oce is ObjectCreateExpression)
				{
					((ObjectCreateExpression)oce).ObjectInitializer = memberInitializers;
				}
				
			} else {
				lexer.NextToken();
				CollectionInitializer(
#line  2038 "VBNET.ATG" 
out initializer);

#line  2040 "VBNET.ATG" 
				if(oce is ObjectCreateExpression)
				((ObjectCreateExpression)oce).ObjectInitializer = initializer;
				
			}
		}
	}

	void AccessorDecls(
#line  1363 "VBNET.ATG" 
out PropertyGetRegion getBlock, out PropertySetRegion setBlock) {

#line  1365 "VBNET.ATG" 
		List<AttributeSection> attributes = new List<AttributeSection>();
		AttributeSection section;
		getBlock = null;
		setBlock = null; 
		
		while (la.kind == 39) {
			AttributeSection(
#line  1370 "VBNET.ATG" 
out section);

#line  1370 "VBNET.ATG" 
			attributes.Add(section); 
		}
		if (StartOf(24)) {
			GetAccessorDecl(
#line  1372 "VBNET.ATG" 
out getBlock, attributes);
			if (StartOf(25)) {

#line  1374 "VBNET.ATG" 
				attributes = new List<AttributeSection>(); 
				while (la.kind == 39) {
					AttributeSection(
#line  1375 "VBNET.ATG" 
out section);

#line  1375 "VBNET.ATG" 
					attributes.Add(section); 
				}
				SetAccessorDecl(
#line  1376 "VBNET.ATG" 
out setBlock, attributes);
			}
		} else if (StartOf(26)) {
			SetAccessorDecl(
#line  1379 "VBNET.ATG" 
out setBlock, attributes);
			if (StartOf(27)) {

#line  1381 "VBNET.ATG" 
				attributes = new List<AttributeSection>(); 
				while (la.kind == 39) {
					AttributeSection(
#line  1382 "VBNET.ATG" 
out section);

#line  1382 "VBNET.ATG" 
					attributes.Add(section); 
				}
				GetAccessorDecl(
#line  1383 "VBNET.ATG" 
out getBlock, attributes);
			}
		} else SynErr(267);
	}

	void EventAccessorDeclaration(
#line  1326 "VBNET.ATG" 
out EventAddRemoveRegion eventAccessorDeclaration) {

#line  1328 "VBNET.ATG" 
		Statement stmt = null;
		List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		eventAccessorDeclaration = null;
		
		while (la.kind == 39) {
			AttributeSection(
#line  1334 "VBNET.ATG" 
out section);

#line  1334 "VBNET.ATG" 
			attributes.Add(section); 
		}
		if (la.kind == 55) {
			lexer.NextToken();
			if (la.kind == 36) {
				lexer.NextToken();
				if (StartOf(6)) {
					FormalParameterList(
#line  1336 "VBNET.ATG" 
p);
				}
				Expect(37);
			}
			Expect(1);
			Block(
#line  1337 "VBNET.ATG" 
out stmt);
			Expect(112);
			Expect(55);
			EndOfStmt();

#line  1339 "VBNET.ATG" 
			eventAccessorDeclaration = new EventAddRegion(attributes);
			eventAccessorDeclaration.Block = (BlockStatement)stmt;
			eventAccessorDeclaration.Parameters = p;
			
		} else if (la.kind == 191) {
			lexer.NextToken();
			if (la.kind == 36) {
				lexer.NextToken();
				if (StartOf(6)) {
					FormalParameterList(
#line  1344 "VBNET.ATG" 
p);
				}
				Expect(37);
			}
			Expect(1);
			Block(
#line  1345 "VBNET.ATG" 
out stmt);
			Expect(112);
			Expect(191);
			EndOfStmt();

#line  1347 "VBNET.ATG" 
			eventAccessorDeclaration = new EventRemoveRegion(attributes);
			eventAccessorDeclaration.Block = (BlockStatement)stmt;
			eventAccessorDeclaration.Parameters = p;
			
		} else if (la.kind == 187) {
			lexer.NextToken();
			if (la.kind == 36) {
				lexer.NextToken();
				if (StartOf(6)) {
					FormalParameterList(
#line  1352 "VBNET.ATG" 
p);
				}
				Expect(37);
			}
			Expect(1);
			Block(
#line  1353 "VBNET.ATG" 
out stmt);
			Expect(112);
			Expect(187);
			EndOfStmt();

#line  1355 "VBNET.ATG" 
			eventAccessorDeclaration = new EventRaiseRegion(attributes);
			eventAccessorDeclaration.Block = (BlockStatement)stmt;
			eventAccessorDeclaration.Parameters = p;
			
		} else SynErr(268);
	}

	void OverloadableOperator(
#line  1268 "VBNET.ATG" 
out OverloadableOperatorType operatorType) {

#line  1269 "VBNET.ATG" 
		operatorType = OverloadableOperatorType.None; 
		switch (la.kind) {
		case 30: {
			lexer.NextToken();

#line  1271 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Add; 
			break;
		}
		case 29: {
			lexer.NextToken();

#line  1273 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Subtract; 
			break;
		}
		case 33: {
			lexer.NextToken();

#line  1275 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Multiply; 
			break;
		}
		case 25: {
			lexer.NextToken();

#line  1277 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Divide; 
			break;
		}
		case 26: {
			lexer.NextToken();

#line  1279 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.DivideInteger; 
			break;
		}
		case 24: {
			lexer.NextToken();

#line  1281 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Concat; 
			break;
		}
		case 149: {
			lexer.NextToken();

#line  1283 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Like; 
			break;
		}
		case 153: {
			lexer.NextToken();

#line  1285 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Modulus; 
			break;
		}
		case 59: {
			lexer.NextToken();

#line  1287 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.BitwiseAnd; 
			break;
		}
		case 174: {
			lexer.NextToken();

#line  1289 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.BitwiseOr; 
			break;
		}
		case 234: {
			lexer.NextToken();

#line  1291 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.ExclusiveOr; 
			break;
		}
		case 31: {
			lexer.NextToken();

#line  1293 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Power; 
			break;
		}
		case 43: {
			lexer.NextToken();

#line  1295 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.ShiftLeft; 
			break;
		}
		case 44: {
			lexer.NextToken();

#line  1297 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.ShiftRight; 
			break;
		}
		case 21: {
			lexer.NextToken();

#line  1299 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Equality; 
			break;
		}
		case 40: {
			lexer.NextToken();

#line  1301 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.InEquality; 
			break;
		}
		case 39: {
			lexer.NextToken();

#line  1303 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.LessThan; 
			break;
		}
		case 42: {
			lexer.NextToken();

#line  1305 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.LessThanOrEqual; 
			break;
		}
		case 38: {
			lexer.NextToken();

#line  1307 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.GreaterThan; 
			break;
		}
		case 41: {
			lexer.NextToken();

#line  1309 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.GreaterThanOrEqual; 
			break;
		}
		case 93: {
			lexer.NextToken();

#line  1311 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.CType; 
			break;
		}
		case 2: case 57: case 61: case 63: case 64: case 65: case 66: case 69: case 86: case 97: case 103: case 106: case 115: case 120: case 125: case 132: case 138: case 142: case 145: case 169: case 175: case 182: case 201: case 210: case 211: case 221: case 222: case 228: {
			Identifier();

#line  1315 "VBNET.ATG" 
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
		default: SynErr(269); break;
		}
	}

	void GetAccessorDecl(
#line  1389 "VBNET.ATG" 
out PropertyGetRegion getBlock, List<AttributeSection> attributes) {

#line  1390 "VBNET.ATG" 
		Statement stmt = null; Modifiers m; 
		PropertyAccessorAccessModifier(
#line  1392 "VBNET.ATG" 
out m);
		Expect(127);

#line  1394 "VBNET.ATG" 
		Location startLocation = t.Location; 
		Expect(1);
		Block(
#line  1396 "VBNET.ATG" 
out stmt);

#line  1397 "VBNET.ATG" 
		getBlock = new PropertyGetRegion((BlockStatement)stmt, attributes); 
		Expect(112);
		Expect(127);

#line  1399 "VBNET.ATG" 
		getBlock.Modifier = m; 

#line  1400 "VBNET.ATG" 
		getBlock.StartLocation = startLocation; getBlock.EndLocation = t.EndLocation; 
		EndOfStmt();
	}

	void SetAccessorDecl(
#line  1405 "VBNET.ATG" 
out PropertySetRegion setBlock, List<AttributeSection> attributes) {

#line  1407 "VBNET.ATG" 
		Statement stmt = null;
		List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
		Modifiers m;
		
		PropertyAccessorAccessModifier(
#line  1412 "VBNET.ATG" 
out m);
		Expect(196);

#line  1414 "VBNET.ATG" 
		Location startLocation = t.Location; 
		if (la.kind == 36) {
			lexer.NextToken();
			if (StartOf(6)) {
				FormalParameterList(
#line  1415 "VBNET.ATG" 
p);
			}
			Expect(37);
		}
		Expect(1);
		Block(
#line  1417 "VBNET.ATG" 
out stmt);

#line  1419 "VBNET.ATG" 
		setBlock = new PropertySetRegion((BlockStatement)stmt, attributes);
		setBlock.Modifier = m;
		setBlock.Parameters = p;
		
		Expect(112);
		Expect(196);

#line  1424 "VBNET.ATG" 
		setBlock.StartLocation = startLocation; setBlock.EndLocation = t.EndLocation; 
		EndOfStmt();
	}

	void PropertyAccessorAccessModifier(
#line  3524 "VBNET.ATG" 
out Modifiers m) {

#line  3525 "VBNET.ATG" 
		m = Modifiers.None; 
		while (StartOf(28)) {
			if (la.kind == 186) {
				lexer.NextToken();

#line  3527 "VBNET.ATG" 
				m |= Modifiers.Public; 
			} else if (la.kind == 185) {
				lexer.NextToken();

#line  3528 "VBNET.ATG" 
				m |= Modifiers.Protected; 
			} else if (la.kind == 124) {
				lexer.NextToken();

#line  3529 "VBNET.ATG" 
				m |= Modifiers.Internal; 
			} else {
				lexer.NextToken();

#line  3530 "VBNET.ATG" 
				m |= Modifiers.Private; 
			}
		}
	}

	void ArrayInitializationModifier(
#line  1522 "VBNET.ATG" 
out List<Expression> arrayModifiers) {

#line  1524 "VBNET.ATG" 
		arrayModifiers = null;
		
		Expect(36);
		InitializationRankList(
#line  1526 "VBNET.ATG" 
out arrayModifiers);
		Expect(37);
	}

	void ArrayNameModifier(
#line  2603 "VBNET.ATG" 
out ArrayList arrayModifiers) {

#line  2605 "VBNET.ATG" 
		arrayModifiers = null;
		
		ArrayTypeModifiers(
#line  2607 "VBNET.ATG" 
out arrayModifiers);
	}

	void InitializationRankList(
#line  1530 "VBNET.ATG" 
out List<Expression> rank) {

#line  1532 "VBNET.ATG" 
		rank = new List<Expression>();
		Expression expr = null;
		
		Expr(
#line  1535 "VBNET.ATG" 
out expr);
		if (la.kind == 214) {
			lexer.NextToken();

#line  1536 "VBNET.ATG" 
			EnsureIsZero(expr); 
			Expr(
#line  1537 "VBNET.ATG" 
out expr);
		}

#line  1539 "VBNET.ATG" 
		if (expr != null) { rank.Add(expr); } 
		while (la.kind == 23) {
			lexer.NextToken();
			Expr(
#line  1541 "VBNET.ATG" 
out expr);
			if (la.kind == 214) {
				lexer.NextToken();

#line  1542 "VBNET.ATG" 
				EnsureIsZero(expr); 
				Expr(
#line  1543 "VBNET.ATG" 
out expr);
			}

#line  1545 "VBNET.ATG" 
			if (expr != null) { rank.Add(expr); } 
		}
	}

	void CollectionInitializer(
#line  1550 "VBNET.ATG" 
out CollectionInitializerExpression outExpr) {

#line  1552 "VBNET.ATG" 
		Expression expr = null;
		CollectionInitializerExpression initializer = new CollectionInitializerExpression();
		
		Expect(34);
		if (StartOf(29)) {
			Expr(
#line  1557 "VBNET.ATG" 
out expr);

#line  1559 "VBNET.ATG" 
			if (expr != null) { initializer.CreateExpressions.Add(expr); }
			
			while (
#line  1562 "VBNET.ATG" 
NotFinalComma()) {
				Expect(23);
				Expr(
#line  1562 "VBNET.ATG" 
out expr);

#line  1563 "VBNET.ATG" 
				if (expr != null) { initializer.CreateExpressions.Add(expr); } 
			}
		}
		Expect(35);

#line  1566 "VBNET.ATG" 
		outExpr = initializer; 
	}

	void EventMemberSpecifier(
#line  1636 "VBNET.ATG" 
out string name) {

#line  1637 "VBNET.ATG" 
		string eventName; 
		if (StartOf(4)) {
			Identifier();
		} else if (la.kind == 157) {
			lexer.NextToken();
		} else if (la.kind == 152) {
			lexer.NextToken();
		} else SynErr(270);

#line  1640 "VBNET.ATG" 
		name = t.val; 
		Expect(27);
		IdentifierOrKeyword(
#line  1642 "VBNET.ATG" 
out eventName);

#line  1643 "VBNET.ATG" 
		name = name + "." + eventName; 
	}

	void IdentifierOrKeyword(
#line  3457 "VBNET.ATG" 
out string name) {
		lexer.NextToken();

#line  3459 "VBNET.ATG" 
		name = t.val;  
	}

	void QueryExpr(
#line  2127 "VBNET.ATG" 
out Expression expr) {

#line  2129 "VBNET.ATG" 
		QueryExpressionVB qexpr = new QueryExpressionVB();
		qexpr.StartLocation = la.Location;
		expr = qexpr;
		
		FromOrAggregateQueryOperator(
#line  2133 "VBNET.ATG" 
qexpr.Clauses);
		while (StartOf(30)) {
			QueryOperator(
#line  2134 "VBNET.ATG" 
qexpr.Clauses);
		}

#line  2136 "VBNET.ATG" 
		qexpr.EndLocation = t.EndLocation;
		
	}

	void LambdaExpr(
#line  2047 "VBNET.ATG" 
out Expression expr) {

#line  2049 "VBNET.ATG" 
		LambdaExpression lambda = null;
		
		if (la.kind == 208) {
			SubLambdaExpression(
#line  2051 "VBNET.ATG" 
out lambda);
		} else if (la.kind == 126) {
			FunctionLambdaExpression(
#line  2052 "VBNET.ATG" 
out lambda);
		} else SynErr(271);

#line  2053 "VBNET.ATG" 
		expr = lambda; 
	}

	void DisjunctionExpr(
#line  1820 "VBNET.ATG" 
out Expression outExpr) {

#line  1822 "VBNET.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		ConjunctionExpr(
#line  1825 "VBNET.ATG" 
out outExpr);
		while (la.kind == 174 || la.kind == 176 || la.kind == 234) {
			if (la.kind == 174) {
				lexer.NextToken();

#line  1828 "VBNET.ATG" 
				op = BinaryOperatorType.BitwiseOr; 
			} else if (la.kind == 176) {
				lexer.NextToken();

#line  1829 "VBNET.ATG" 
				op = BinaryOperatorType.LogicalOr; 
			} else {
				lexer.NextToken();

#line  1830 "VBNET.ATG" 
				op = BinaryOperatorType.ExclusiveOr; 
			}
			ConjunctionExpr(
#line  1832 "VBNET.ATG" 
out expr);

#line  1832 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr);  
		}
	}

	void AssignmentOperator(
#line  1654 "VBNET.ATG" 
out AssignmentOperatorType op) {

#line  1655 "VBNET.ATG" 
		op = AssignmentOperatorType.None; 
		switch (la.kind) {
		case 21: {
			lexer.NextToken();

#line  1656 "VBNET.ATG" 
			op = AssignmentOperatorType.Assign; 
			break;
		}
		case 53: {
			lexer.NextToken();

#line  1657 "VBNET.ATG" 
			op = AssignmentOperatorType.ConcatString; 
			break;
		}
		case 45: {
			lexer.NextToken();

#line  1658 "VBNET.ATG" 
			op = AssignmentOperatorType.Add; 
			break;
		}
		case 47: {
			lexer.NextToken();

#line  1659 "VBNET.ATG" 
			op = AssignmentOperatorType.Subtract; 
			break;
		}
		case 48: {
			lexer.NextToken();

#line  1660 "VBNET.ATG" 
			op = AssignmentOperatorType.Multiply; 
			break;
		}
		case 49: {
			lexer.NextToken();

#line  1661 "VBNET.ATG" 
			op = AssignmentOperatorType.Divide; 
			break;
		}
		case 50: {
			lexer.NextToken();

#line  1662 "VBNET.ATG" 
			op = AssignmentOperatorType.DivideInteger; 
			break;
		}
		case 46: {
			lexer.NextToken();

#line  1663 "VBNET.ATG" 
			op = AssignmentOperatorType.Power; 
			break;
		}
		case 51: {
			lexer.NextToken();

#line  1664 "VBNET.ATG" 
			op = AssignmentOperatorType.ShiftLeft; 
			break;
		}
		case 52: {
			lexer.NextToken();

#line  1665 "VBNET.ATG" 
			op = AssignmentOperatorType.ShiftRight; 
			break;
		}
		default: SynErr(272); break;
		}
	}

	void SimpleExpr(
#line  1669 "VBNET.ATG" 
out Expression pexpr) {

#line  1670 "VBNET.ATG" 
		string name; 
		SimpleNonInvocationExpression(
#line  1672 "VBNET.ATG" 
out pexpr);
		while (la.kind == 27 || la.kind == 28 || la.kind == 36) {
			if (la.kind == 27) {
				lexer.NextToken();
				IdentifierOrKeyword(
#line  1674 "VBNET.ATG" 
out name);

#line  1675 "VBNET.ATG" 
				pexpr = new MemberReferenceExpression(pexpr, name); 
				if (
#line  1676 "VBNET.ATG" 
la.kind == Tokens.OpenParenthesis && Peek(1).kind == Tokens.Of) {
					lexer.NextToken();
					Expect(168);
					TypeArgumentList(
#line  1677 "VBNET.ATG" 
((MemberReferenceExpression)pexpr).TypeArguments);
					Expect(37);
				}
			} else if (la.kind == 28) {
				lexer.NextToken();
				IdentifierOrKeyword(
#line  1679 "VBNET.ATG" 
out name);

#line  1679 "VBNET.ATG" 
				pexpr = new BinaryOperatorExpression(pexpr, BinaryOperatorType.DictionaryAccess, new PrimitiveExpression(name, name)); 
			} else {
				InvocationExpression(
#line  1680 "VBNET.ATG" 
ref pexpr);
			}
		}
	}

	void SimpleNonInvocationExpression(
#line  1684 "VBNET.ATG" 
out Expression pexpr) {

#line  1686 "VBNET.ATG" 
		Expression expr;
		CollectionInitializerExpression cie;
		TypeReference type = null;
		string name = String.Empty;
		pexpr = null;
		
		if (StartOf(31)) {
			switch (la.kind) {
			case 3: {
				lexer.NextToken();

#line  1695 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(t.literalValue, t.val) { LiteralFormat = t.literalFormat };  
				break;
			}
			case 4: {
				lexer.NextToken();

#line  1696 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(t.literalValue, t.val) { LiteralFormat = t.literalFormat };  
				break;
			}
			case 7: {
				lexer.NextToken();

#line  1697 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(t.literalValue, t.val) { LiteralFormat = t.literalFormat };  
				break;
			}
			case 6: {
				lexer.NextToken();

#line  1698 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(t.literalValue, t.val) { LiteralFormat = t.literalFormat };  
				break;
			}
			case 5: {
				lexer.NextToken();

#line  1699 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(t.literalValue, t.val) { LiteralFormat = t.literalFormat };  
				break;
			}
			case 9: {
				lexer.NextToken();

#line  1700 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(t.literalValue, t.val) { LiteralFormat = t.literalFormat };  
				break;
			}
			case 8: {
				lexer.NextToken();

#line  1701 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(t.literalValue, t.val) { LiteralFormat = t.literalFormat };  
				break;
			}
			case 215: {
				lexer.NextToken();

#line  1703 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(true, "true");  
				break;
			}
			case 121: {
				lexer.NextToken();

#line  1704 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(false, "false"); 
				break;
			}
			case 164: {
				lexer.NextToken();

#line  1705 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(null, "null");  
				break;
			}
			case 36: {
				lexer.NextToken();
				Expr(
#line  1706 "VBNET.ATG" 
out expr);
				Expect(37);

#line  1706 "VBNET.ATG" 
				pexpr = new ParenthesizedExpression(expr); 
				break;
			}
			case 2: case 57: case 61: case 63: case 64: case 65: case 66: case 69: case 86: case 97: case 103: case 106: case 115: case 120: case 125: case 132: case 138: case 142: case 145: case 169: case 175: case 182: case 201: case 210: case 211: case 221: case 222: case 228: {
				Identifier();

#line  1708 "VBNET.ATG" 
				pexpr = new IdentifierExpression(t.val);
				pexpr.StartLocation = t.Location; pexpr.EndLocation = t.EndLocation;
				
				if (
#line  1711 "VBNET.ATG" 
la.kind == Tokens.OpenParenthesis && Peek(1).kind == Tokens.Of) {
					lexer.NextToken();
					Expect(168);
					TypeArgumentList(
#line  1712 "VBNET.ATG" 
((IdentifierExpression)pexpr).TypeArguments);
					Expect(37);
				}
				break;
			}
			case 67: case 70: case 81: case 98: case 99: case 108: case 140: case 150: case 167: case 194: case 199: case 200: case 206: case 219: case 220: case 223: {

#line  1714 "VBNET.ATG" 
				string val = String.Empty; 
				if (StartOf(12)) {
					PrimitiveTypeName(
#line  1715 "VBNET.ATG" 
out val);
				} else if (la.kind == 167) {
					lexer.NextToken();

#line  1715 "VBNET.ATG" 
					val = "System.Object"; 
				} else SynErr(273);

#line  1716 "VBNET.ATG" 
				pexpr = new TypeReferenceExpression(new TypeReference(val, true)); 
				break;
			}
			case 152: {
				lexer.NextToken();

#line  1717 "VBNET.ATG" 
				pexpr = new ThisReferenceExpression(); 
				break;
			}
			case 157: case 158: {

#line  1718 "VBNET.ATG" 
				Expression retExpr = null; 
				if (la.kind == 157) {
					lexer.NextToken();

#line  1719 "VBNET.ATG" 
					retExpr = new BaseReferenceExpression(); 
				} else if (la.kind == 158) {
					lexer.NextToken();

#line  1720 "VBNET.ATG" 
					retExpr = new ClassReferenceExpression(); 
				} else SynErr(274);
				Expect(27);
				IdentifierOrKeyword(
#line  1722 "VBNET.ATG" 
out name);

#line  1722 "VBNET.ATG" 
				pexpr = new MemberReferenceExpression(retExpr, name); 
				break;
			}
			case 129: {
				lexer.NextToken();
				Expect(27);
				Identifier();

#line  1724 "VBNET.ATG" 
				type = new TypeReference(t.val ?? ""); 

#line  1726 "VBNET.ATG" 
				type.IsGlobal = true; 

#line  1727 "VBNET.ATG" 
				pexpr = new TypeReferenceExpression(type); 
				break;
			}
			case 161: {
				ObjectCreateExpression(
#line  1728 "VBNET.ATG" 
out expr);

#line  1728 "VBNET.ATG" 
				pexpr = expr; 
				break;
			}
			case 34: {
				CollectionInitializer(
#line  1729 "VBNET.ATG" 
out cie);

#line  1729 "VBNET.ATG" 
				pexpr = cie; 
				break;
			}
			case 93: case 105: case 217: {

#line  1731 "VBNET.ATG" 
				CastType castType = CastType.Cast; 
				if (la.kind == 105) {
					lexer.NextToken();
				} else if (la.kind == 93) {
					lexer.NextToken();

#line  1733 "VBNET.ATG" 
					castType = CastType.Conversion; 
				} else if (la.kind == 217) {
					lexer.NextToken();

#line  1734 "VBNET.ATG" 
					castType = CastType.TryCast; 
				} else SynErr(275);
				Expect(36);
				Expr(
#line  1736 "VBNET.ATG" 
out expr);
				Expect(23);
				TypeName(
#line  1736 "VBNET.ATG" 
out type);
				Expect(37);

#line  1737 "VBNET.ATG" 
				pexpr = new CastExpression(type, expr, castType); 
				break;
			}
			case 75: case 76: case 77: case 78: case 79: case 80: case 82: case 84: case 85: case 89: case 90: case 91: case 92: case 94: case 95: case 96: {
				CastTarget(
#line  1738 "VBNET.ATG" 
out type);
				Expect(36);
				Expr(
#line  1738 "VBNET.ATG" 
out expr);
				Expect(37);

#line  1738 "VBNET.ATG" 
				pexpr = new CastExpression(type, expr, CastType.PrimitiveConversion); 
				break;
			}
			case 56: {
				lexer.NextToken();
				Expr(
#line  1739 "VBNET.ATG" 
out expr);

#line  1739 "VBNET.ATG" 
				pexpr = new AddressOfExpression(expr); 
				break;
			}
			case 128: {
				lexer.NextToken();
				Expect(36);
				GetTypeTypeName(
#line  1740 "VBNET.ATG" 
out type);
				Expect(37);

#line  1740 "VBNET.ATG" 
				pexpr = new TypeOfExpression(type); 
				break;
			}
			case 218: {
				lexer.NextToken();
				SimpleExpr(
#line  1741 "VBNET.ATG" 
out expr);
				Expect(143);
				TypeName(
#line  1741 "VBNET.ATG" 
out type);

#line  1741 "VBNET.ATG" 
				pexpr = new TypeOfIsExpression(expr, type); 
				break;
			}
			case 134: {
				ConditionalExpression(
#line  1742 "VBNET.ATG" 
out pexpr);
				break;
			}
			}
		} else if (la.kind == 27) {
			lexer.NextToken();
			IdentifierOrKeyword(
#line  1746 "VBNET.ATG" 
out name);

#line  1746 "VBNET.ATG" 
			pexpr = new MemberReferenceExpression(null, name);
		} else SynErr(276);
	}

	void TypeArgumentList(
#line  2639 "VBNET.ATG" 
List<TypeReference> typeArguments) {

#line  2641 "VBNET.ATG" 
		TypeReference typeref;
		
		TypeName(
#line  2643 "VBNET.ATG" 
out typeref);

#line  2643 "VBNET.ATG" 
		if (typeref != null) typeArguments.Add(typeref); 
		while (la.kind == 23) {
			lexer.NextToken();
			TypeName(
#line  2646 "VBNET.ATG" 
out typeref);

#line  2646 "VBNET.ATG" 
			if (typeref != null) typeArguments.Add(typeref); 
		}
	}

	void InvocationExpression(
#line  1784 "VBNET.ATG" 
ref Expression pexpr) {

#line  1785 "VBNET.ATG" 
		List<Expression> parameters = null; 
		Expect(36);

#line  1787 "VBNET.ATG" 
		Location start = t.Location; 
		ArgumentList(
#line  1788 "VBNET.ATG" 
out parameters);
		Expect(37);

#line  1791 "VBNET.ATG" 
		pexpr = new InvocationExpression(pexpr, parameters);
		

#line  1793 "VBNET.ATG" 
		pexpr.StartLocation = start; pexpr.EndLocation = t.Location; 
	}

	void PrimitiveTypeName(
#line  3464 "VBNET.ATG" 
out string type) {

#line  3465 "VBNET.ATG" 
		type = String.Empty; 
		switch (la.kind) {
		case 67: {
			lexer.NextToken();

#line  3466 "VBNET.ATG" 
			type = "System.Boolean"; 
			break;
		}
		case 98: {
			lexer.NextToken();

#line  3467 "VBNET.ATG" 
			type = "System.DateTime"; 
			break;
		}
		case 81: {
			lexer.NextToken();

#line  3468 "VBNET.ATG" 
			type = "System.Char"; 
			break;
		}
		case 206: {
			lexer.NextToken();

#line  3469 "VBNET.ATG" 
			type = "System.String"; 
			break;
		}
		case 99: {
			lexer.NextToken();

#line  3470 "VBNET.ATG" 
			type = "System.Decimal"; 
			break;
		}
		case 70: {
			lexer.NextToken();

#line  3471 "VBNET.ATG" 
			type = "System.Byte"; 
			break;
		}
		case 199: {
			lexer.NextToken();

#line  3472 "VBNET.ATG" 
			type = "System.Int16"; 
			break;
		}
		case 140: {
			lexer.NextToken();

#line  3473 "VBNET.ATG" 
			type = "System.Int32"; 
			break;
		}
		case 150: {
			lexer.NextToken();

#line  3474 "VBNET.ATG" 
			type = "System.Int64"; 
			break;
		}
		case 200: {
			lexer.NextToken();

#line  3475 "VBNET.ATG" 
			type = "System.Single"; 
			break;
		}
		case 108: {
			lexer.NextToken();

#line  3476 "VBNET.ATG" 
			type = "System.Double"; 
			break;
		}
		case 219: {
			lexer.NextToken();

#line  3477 "VBNET.ATG" 
			type = "System.UInt32"; 
			break;
		}
		case 220: {
			lexer.NextToken();

#line  3478 "VBNET.ATG" 
			type = "System.UInt64"; 
			break;
		}
		case 223: {
			lexer.NextToken();

#line  3479 "VBNET.ATG" 
			type = "System.UInt16"; 
			break;
		}
		case 194: {
			lexer.NextToken();

#line  3480 "VBNET.ATG" 
			type = "System.SByte"; 
			break;
		}
		default: SynErr(277); break;
		}
	}

	void CastTarget(
#line  1798 "VBNET.ATG" 
out TypeReference type) {

#line  1800 "VBNET.ATG" 
		type = null;
		
		switch (la.kind) {
		case 75: {
			lexer.NextToken();

#line  1802 "VBNET.ATG" 
			type = new TypeReference("System.Boolean", true); 
			break;
		}
		case 76: {
			lexer.NextToken();

#line  1803 "VBNET.ATG" 
			type = new TypeReference("System.Byte", true); 
			break;
		}
		case 89: {
			lexer.NextToken();

#line  1804 "VBNET.ATG" 
			type = new TypeReference("System.SByte", true); 
			break;
		}
		case 77: {
			lexer.NextToken();

#line  1805 "VBNET.ATG" 
			type = new TypeReference("System.Char", true); 
			break;
		}
		case 78: {
			lexer.NextToken();

#line  1806 "VBNET.ATG" 
			type = new TypeReference("System.DateTime", true); 
			break;
		}
		case 80: {
			lexer.NextToken();

#line  1807 "VBNET.ATG" 
			type = new TypeReference("System.Decimal", true); 
			break;
		}
		case 79: {
			lexer.NextToken();

#line  1808 "VBNET.ATG" 
			type = new TypeReference("System.Double", true); 
			break;
		}
		case 90: {
			lexer.NextToken();

#line  1809 "VBNET.ATG" 
			type = new TypeReference("System.Int16", true); 
			break;
		}
		case 82: {
			lexer.NextToken();

#line  1810 "VBNET.ATG" 
			type = new TypeReference("System.Int32", true); 
			break;
		}
		case 84: {
			lexer.NextToken();

#line  1811 "VBNET.ATG" 
			type = new TypeReference("System.Int64", true); 
			break;
		}
		case 96: {
			lexer.NextToken();

#line  1812 "VBNET.ATG" 
			type = new TypeReference("System.UInt16", true); 
			break;
		}
		case 94: {
			lexer.NextToken();

#line  1813 "VBNET.ATG" 
			type = new TypeReference("System.UInt32", true); 
			break;
		}
		case 95: {
			lexer.NextToken();

#line  1814 "VBNET.ATG" 
			type = new TypeReference("System.UInt64", true); 
			break;
		}
		case 85: {
			lexer.NextToken();

#line  1815 "VBNET.ATG" 
			type = new TypeReference("System.Object", true); 
			break;
		}
		case 91: {
			lexer.NextToken();

#line  1816 "VBNET.ATG" 
			type = new TypeReference("System.Single", true); 
			break;
		}
		case 92: {
			lexer.NextToken();

#line  1817 "VBNET.ATG" 
			type = new TypeReference("System.String", true); 
			break;
		}
		default: SynErr(278); break;
		}
	}

	void GetTypeTypeName(
#line  2538 "VBNET.ATG" 
out TypeReference typeref) {

#line  2539 "VBNET.ATG" 
		ArrayList rank = null; 
		NonArrayTypeName(
#line  2541 "VBNET.ATG" 
out typeref, true);
		ArrayTypeModifiers(
#line  2542 "VBNET.ATG" 
out rank);

#line  2543 "VBNET.ATG" 
		if (rank != null && typeref != null) {
		typeref.RankSpecifier = (int[])rank.ToArray(typeof(int));
		}
		
	}

	void ConditionalExpression(
#line  1750 "VBNET.ATG" 
out Expression expr) {

#line  1752 "VBNET.ATG" 
		ConditionalExpression conditionalExpression = new ConditionalExpression();
		BinaryOperatorExpression binaryOperatorExpression = new BinaryOperatorExpression();
		conditionalExpression.StartLocation = binaryOperatorExpression.StartLocation = la.Location;
		
		Expression condition = null;
		Expression trueExpr = null;
		Expression falseExpr = null;
		
		Expect(134);
		Expect(36);
		Expr(
#line  1761 "VBNET.ATG" 
out condition);
		Expect(23);
		Expr(
#line  1761 "VBNET.ATG" 
out trueExpr);
		if (la.kind == 23) {
			lexer.NextToken();
			Expr(
#line  1761 "VBNET.ATG" 
out falseExpr);
		}
		Expect(37);

#line  1763 "VBNET.ATG" 
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
#line  2470 "VBNET.ATG" 
out List<Expression> arguments) {

#line  2472 "VBNET.ATG" 
		arguments = new List<Expression>();
		Expression expr = null;
		
		if (StartOf(29)) {
			Argument(
#line  2475 "VBNET.ATG" 
out expr);
		}
		while (la.kind == 23) {
			lexer.NextToken();

#line  2476 "VBNET.ATG" 
			arguments.Add(expr ?? Expression.Null); expr = null; 
			if (StartOf(29)) {
				Argument(
#line  2477 "VBNET.ATG" 
out expr);
			}

#line  2478 "VBNET.ATG" 
			if (expr == null) expr = Expression.Null; 
		}

#line  2480 "VBNET.ATG" 
		if (expr != null) arguments.Add(expr); 
	}

	void ConjunctionExpr(
#line  1836 "VBNET.ATG" 
out Expression outExpr) {

#line  1838 "VBNET.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		NotExpr(
#line  1841 "VBNET.ATG" 
out outExpr);
		while (la.kind == 59 || la.kind == 60) {
			if (la.kind == 59) {
				lexer.NextToken();

#line  1844 "VBNET.ATG" 
				op = BinaryOperatorType.BitwiseAnd; 
			} else {
				lexer.NextToken();

#line  1845 "VBNET.ATG" 
				op = BinaryOperatorType.LogicalAnd; 
			}
			NotExpr(
#line  1847 "VBNET.ATG" 
out expr);

#line  1847 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr);  
		}
	}

	void NotExpr(
#line  1851 "VBNET.ATG" 
out Expression outExpr) {

#line  1852 "VBNET.ATG" 
		UnaryOperatorType uop = UnaryOperatorType.None; 
		while (la.kind == 163) {
			lexer.NextToken();

#line  1853 "VBNET.ATG" 
			uop = UnaryOperatorType.Not; 
		}
		ComparisonExpr(
#line  1854 "VBNET.ATG" 
out outExpr);

#line  1855 "VBNET.ATG" 
		if (uop != UnaryOperatorType.None)
		outExpr = new UnaryOperatorExpression(outExpr, uop);
		
	}

	void ComparisonExpr(
#line  1860 "VBNET.ATG" 
out Expression outExpr) {

#line  1862 "VBNET.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		ShiftExpr(
#line  1865 "VBNET.ATG" 
out outExpr);
		while (StartOf(32)) {
			switch (la.kind) {
			case 39: {
				lexer.NextToken();

#line  1868 "VBNET.ATG" 
				op = BinaryOperatorType.LessThan; 
				break;
			}
			case 38: {
				lexer.NextToken();

#line  1869 "VBNET.ATG" 
				op = BinaryOperatorType.GreaterThan; 
				break;
			}
			case 42: {
				lexer.NextToken();

#line  1870 "VBNET.ATG" 
				op = BinaryOperatorType.LessThanOrEqual; 
				break;
			}
			case 41: {
				lexer.NextToken();

#line  1871 "VBNET.ATG" 
				op = BinaryOperatorType.GreaterThanOrEqual; 
				break;
			}
			case 40: {
				lexer.NextToken();

#line  1872 "VBNET.ATG" 
				op = BinaryOperatorType.InEquality; 
				break;
			}
			case 21: {
				lexer.NextToken();

#line  1873 "VBNET.ATG" 
				op = BinaryOperatorType.Equality; 
				break;
			}
			case 149: {
				lexer.NextToken();

#line  1874 "VBNET.ATG" 
				op = BinaryOperatorType.Like; 
				break;
			}
			case 143: {
				lexer.NextToken();

#line  1875 "VBNET.ATG" 
				op = BinaryOperatorType.ReferenceEquality; 
				break;
			}
			case 144: {
				lexer.NextToken();

#line  1876 "VBNET.ATG" 
				op = BinaryOperatorType.ReferenceInequality; 
				break;
			}
			}
			if (StartOf(33)) {
				ShiftExpr(
#line  1879 "VBNET.ATG" 
out expr);

#line  1879 "VBNET.ATG" 
				outExpr = new BinaryOperatorExpression(outExpr, op, expr);  
			} else if (la.kind == 163) {
				lexer.NextToken();
				ShiftExpr(
#line  1882 "VBNET.ATG" 
out expr);

#line  1882 "VBNET.ATG" 
				outExpr = new BinaryOperatorExpression(outExpr, op, new UnaryOperatorExpression(expr, UnaryOperatorType.Not));  
			} else SynErr(279);
		}
	}

	void ShiftExpr(
#line  1887 "VBNET.ATG" 
out Expression outExpr) {

#line  1889 "VBNET.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		ConcatenationExpr(
#line  1892 "VBNET.ATG" 
out outExpr);
		while (la.kind == 43 || la.kind == 44) {
			if (la.kind == 43) {
				lexer.NextToken();

#line  1895 "VBNET.ATG" 
				op = BinaryOperatorType.ShiftLeft; 
			} else {
				lexer.NextToken();

#line  1896 "VBNET.ATG" 
				op = BinaryOperatorType.ShiftRight; 
			}
			ConcatenationExpr(
#line  1898 "VBNET.ATG" 
out expr);

#line  1898 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr);  
		}
	}

	void ConcatenationExpr(
#line  1902 "VBNET.ATG" 
out Expression outExpr) {

#line  1903 "VBNET.ATG" 
		Expression expr; 
		AdditiveExpr(
#line  1905 "VBNET.ATG" 
out outExpr);
		while (la.kind == 24) {
			lexer.NextToken();
			AdditiveExpr(
#line  1905 "VBNET.ATG" 
out expr);

#line  1905 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.Concat, expr);  
		}
	}

	void AdditiveExpr(
#line  1908 "VBNET.ATG" 
out Expression outExpr) {

#line  1910 "VBNET.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		ModuloExpr(
#line  1913 "VBNET.ATG" 
out outExpr);
		while (la.kind == 29 || la.kind == 30) {
			if (la.kind == 30) {
				lexer.NextToken();

#line  1916 "VBNET.ATG" 
				op = BinaryOperatorType.Add; 
			} else {
				lexer.NextToken();

#line  1917 "VBNET.ATG" 
				op = BinaryOperatorType.Subtract; 
			}
			ModuloExpr(
#line  1919 "VBNET.ATG" 
out expr);

#line  1919 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr);  
		}
	}

	void ModuloExpr(
#line  1923 "VBNET.ATG" 
out Expression outExpr) {

#line  1924 "VBNET.ATG" 
		Expression expr; 
		IntegerDivisionExpr(
#line  1926 "VBNET.ATG" 
out outExpr);
		while (la.kind == 153) {
			lexer.NextToken();
			IntegerDivisionExpr(
#line  1926 "VBNET.ATG" 
out expr);

#line  1926 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.Modulus, expr);  
		}
	}

	void IntegerDivisionExpr(
#line  1929 "VBNET.ATG" 
out Expression outExpr) {

#line  1930 "VBNET.ATG" 
		Expression expr; 
		MultiplicativeExpr(
#line  1932 "VBNET.ATG" 
out outExpr);
		while (la.kind == 26) {
			lexer.NextToken();
			MultiplicativeExpr(
#line  1932 "VBNET.ATG" 
out expr);

#line  1932 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.DivideInteger, expr);  
		}
	}

	void MultiplicativeExpr(
#line  1935 "VBNET.ATG" 
out Expression outExpr) {

#line  1937 "VBNET.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		UnaryExpr(
#line  1940 "VBNET.ATG" 
out outExpr);
		while (la.kind == 25 || la.kind == 33) {
			if (la.kind == 33) {
				lexer.NextToken();

#line  1943 "VBNET.ATG" 
				op = BinaryOperatorType.Multiply; 
			} else {
				lexer.NextToken();

#line  1944 "VBNET.ATG" 
				op = BinaryOperatorType.Divide; 
			}
			UnaryExpr(
#line  1946 "VBNET.ATG" 
out expr);

#line  1946 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr); 
		}
	}

	void UnaryExpr(
#line  1950 "VBNET.ATG" 
out Expression uExpr) {

#line  1952 "VBNET.ATG" 
		Expression expr;
		UnaryOperatorType uop = UnaryOperatorType.None;
		bool isUOp = false;
		
		while (la.kind == 29 || la.kind == 30 || la.kind == 33) {
			if (la.kind == 30) {
				lexer.NextToken();

#line  1956 "VBNET.ATG" 
				uop = UnaryOperatorType.Plus; isUOp = true; 
			} else if (la.kind == 29) {
				lexer.NextToken();

#line  1957 "VBNET.ATG" 
				uop = UnaryOperatorType.Minus; isUOp = true; 
			} else {
				lexer.NextToken();

#line  1958 "VBNET.ATG" 
				uop = UnaryOperatorType.Dereference;  isUOp = true;
			}
		}
		ExponentiationExpr(
#line  1960 "VBNET.ATG" 
out expr);

#line  1962 "VBNET.ATG" 
		if (isUOp) {
		uExpr = new UnaryOperatorExpression(expr, uop);
		} else {
			uExpr = expr;
		}
		
	}

	void ExponentiationExpr(
#line  1970 "VBNET.ATG" 
out Expression outExpr) {

#line  1971 "VBNET.ATG" 
		Expression expr; 
		SimpleExpr(
#line  1973 "VBNET.ATG" 
out outExpr);
		while (la.kind == 31) {
			lexer.NextToken();
			SimpleExpr(
#line  1973 "VBNET.ATG" 
out expr);

#line  1973 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.Power, expr);  
		}
	}

	void NormalOrReDimArgumentList(
#line  2484 "VBNET.ATG" 
out List<Expression> arguments, out bool canBeNormal, out bool canBeRedim) {

#line  2486 "VBNET.ATG" 
		arguments = new List<Expression>();
		canBeNormal = true; canBeRedim = !IsNamedAssign();
		Expression expr = null;
		
		if (StartOf(29)) {
			Argument(
#line  2491 "VBNET.ATG" 
out expr);
			if (la.kind == 214) {
				lexer.NextToken();

#line  2492 "VBNET.ATG" 
				EnsureIsZero(expr); canBeNormal = false; 
				Expr(
#line  2493 "VBNET.ATG" 
out expr);
			}
		}
		while (la.kind == 23) {
			lexer.NextToken();

#line  2496 "VBNET.ATG" 
			if (expr == null) canBeRedim = false; 

#line  2497 "VBNET.ATG" 
			arguments.Add(expr ?? Expression.Null); expr = null; 

#line  2498 "VBNET.ATG" 
			canBeRedim &= !IsNamedAssign(); 
			if (StartOf(29)) {
				Argument(
#line  2499 "VBNET.ATG" 
out expr);
				if (la.kind == 214) {
					lexer.NextToken();

#line  2500 "VBNET.ATG" 
					EnsureIsZero(expr); canBeNormal = false; 
					Expr(
#line  2501 "VBNET.ATG" 
out expr);
				}
			}

#line  2503 "VBNET.ATG" 
			if (expr == null) { canBeRedim = false; expr = Expression.Null; } 
		}

#line  2505 "VBNET.ATG" 
		if (expr != null) arguments.Add(expr); else canBeRedim = false; 
	}

	void ArrayTypeModifiers(
#line  2612 "VBNET.ATG" 
out ArrayList arrayModifiers) {

#line  2614 "VBNET.ATG" 
		arrayModifiers = new ArrayList();
		int i = 0;
		
		while (
#line  2617 "VBNET.ATG" 
IsDims()) {
			Expect(36);
			if (la.kind == 23 || la.kind == 37) {
				RankList(
#line  2619 "VBNET.ATG" 
out i);
			}

#line  2621 "VBNET.ATG" 
			arrayModifiers.Add(i);
			
			Expect(37);
		}

#line  2626 "VBNET.ATG" 
		if(arrayModifiers.Count == 0) {
		 arrayModifiers = null;
		}
		
	}

	void MemberInitializer(
#line  2451 "VBNET.ATG" 
out MemberInitializerExpression memberInitializer) {

#line  2453 "VBNET.ATG" 
		memberInitializer = new MemberInitializerExpression();
		memberInitializer.StartLocation = la.Location;
		Expression initExpr = null;
		bool isKey = false;
		string name = null;
		
		Expect(27);
		IdentifierOrKeyword(
#line  2460 "VBNET.ATG" 
out name);
		Expect(21);
		Expr(
#line  2460 "VBNET.ATG" 
out initExpr);

#line  2462 "VBNET.ATG" 
		memberInitializer.Name = name;
		memberInitializer.Expression = initExpr;
		memberInitializer.IsKey = isKey;
		memberInitializer.EndLocation = t.EndLocation;
		
	}

	void SubLambdaExpression(
#line  2056 "VBNET.ATG" 
out LambdaExpression lambda) {

#line  2058 "VBNET.ATG" 
		lambda = new LambdaExpression();
		lambda.ReturnType = new TypeReference("System.Void", true);
		Expression inner = null;
		Statement statement = null;
		lambda.StartLocation = la.Location;
		
		Expect(208);
		if (la.kind == 36) {
			lexer.NextToken();
			if (StartOf(6)) {
				FormalParameterList(
#line  2065 "VBNET.ATG" 
lambda.Parameters);
			}
			Expect(37);
		}
		if (StartOf(34)) {
			if (StartOf(29)) {
				Expr(
#line  2068 "VBNET.ATG" 
out inner);

#line  2070 "VBNET.ATG" 
				lambda.ExpressionBody = inner;
				lambda.EndLocation = t.EndLocation; // la.Location?
				
			} else {
				EmbeddedStatement(
#line  2074 "VBNET.ATG" 
out statement);

#line  2076 "VBNET.ATG" 
				lambda.StatementBody = statement;
				lambda.EndLocation = t.EndLocation;
				
			}
		} else if (la.kind == 1) {
			lexer.NextToken();
			Block(
#line  2082 "VBNET.ATG" 
out statement);
			Expect(112);
			Expect(208);

#line  2085 "VBNET.ATG" 
			lambda.StatementBody = statement;
			lambda.EndLocation = t.EndLocation;
			
		} else SynErr(280);
	}

	void FunctionLambdaExpression(
#line  2091 "VBNET.ATG" 
out LambdaExpression lambda) {

#line  2093 "VBNET.ATG" 
		lambda = new LambdaExpression();
		TypeReference typeRef = null;
		Expression inner = null;
		Statement statement = null;
		lambda.StartLocation = la.Location;
		
		Expect(126);
		if (la.kind == 36) {
			lexer.NextToken();
			if (StartOf(6)) {
				FormalParameterList(
#line  2100 "VBNET.ATG" 
lambda.Parameters);
			}
			Expect(37);
		}
		if (la.kind == 62) {
			lexer.NextToken();
			TypeName(
#line  2101 "VBNET.ATG" 
out typeRef);

#line  2101 "VBNET.ATG" 
			lambda.ReturnType = typeRef; 
		}
		if (StartOf(34)) {
			if (StartOf(29)) {
				Expr(
#line  2104 "VBNET.ATG" 
out inner);

#line  2106 "VBNET.ATG" 
				lambda.ExpressionBody = inner;
				lambda.EndLocation = t.EndLocation; // la.Location?
				
			} else {
				EmbeddedStatement(
#line  2110 "VBNET.ATG" 
out statement);

#line  2112 "VBNET.ATG" 
				lambda.StatementBody = statement;
				lambda.EndLocation = t.EndLocation;
				
			}
		} else if (la.kind == 1) {
			lexer.NextToken();
			Block(
#line  2118 "VBNET.ATG" 
out statement);
			Expect(112);
			Expect(126);

#line  2121 "VBNET.ATG" 
			lambda.StatementBody = statement;
			lambda.EndLocation = t.EndLocation;
			
		} else SynErr(281);
	}

	void EmbeddedStatement(
#line  2883 "VBNET.ATG" 
out Statement statement) {

#line  2885 "VBNET.ATG" 
		Statement embeddedStatement = null;
		statement = null;
		Expression expr = null;
		string name = String.Empty;
		List<Expression> p = null;
		
		if (la.kind == 119) {
			lexer.NextToken();

#line  2891 "VBNET.ATG" 
			ExitType exitType = ExitType.None; 
			switch (la.kind) {
			case 208: {
				lexer.NextToken();

#line  2893 "VBNET.ATG" 
				exitType = ExitType.Sub; 
				break;
			}
			case 126: {
				lexer.NextToken();

#line  2895 "VBNET.ATG" 
				exitType = ExitType.Function; 
				break;
			}
			case 184: {
				lexer.NextToken();

#line  2897 "VBNET.ATG" 
				exitType = ExitType.Property; 
				break;
			}
			case 107: {
				lexer.NextToken();

#line  2899 "VBNET.ATG" 
				exitType = ExitType.Do; 
				break;
			}
			case 123: {
				lexer.NextToken();

#line  2901 "VBNET.ATG" 
				exitType = ExitType.For; 
				break;
			}
			case 216: {
				lexer.NextToken();

#line  2903 "VBNET.ATG" 
				exitType = ExitType.Try; 
				break;
			}
			case 229: {
				lexer.NextToken();

#line  2905 "VBNET.ATG" 
				exitType = ExitType.While; 
				break;
			}
			case 195: {
				lexer.NextToken();

#line  2907 "VBNET.ATG" 
				exitType = ExitType.Select; 
				break;
			}
			default: SynErr(282); break;
			}

#line  2909 "VBNET.ATG" 
			statement = new ExitStatement(exitType); 
		} else if (la.kind == 216) {
			TryStatement(
#line  2910 "VBNET.ATG" 
out statement);
		} else if (la.kind == 88) {
			lexer.NextToken();

#line  2911 "VBNET.ATG" 
			ContinueType continueType = ContinueType.None; 
			if (la.kind == 107 || la.kind == 123 || la.kind == 229) {
				if (la.kind == 107) {
					lexer.NextToken();

#line  2911 "VBNET.ATG" 
					continueType = ContinueType.Do; 
				} else if (la.kind == 123) {
					lexer.NextToken();

#line  2911 "VBNET.ATG" 
					continueType = ContinueType.For; 
				} else {
					lexer.NextToken();

#line  2911 "VBNET.ATG" 
					continueType = ContinueType.While; 
				}
			}

#line  2911 "VBNET.ATG" 
			statement = new ContinueStatement(continueType); 
		} else if (la.kind == 213) {
			lexer.NextToken();
			if (StartOf(29)) {
				Expr(
#line  2913 "VBNET.ATG" 
out expr);
			}

#line  2913 "VBNET.ATG" 
			statement = new ThrowStatement(expr); 
		} else if (la.kind == 193) {
			lexer.NextToken();
			if (StartOf(29)) {
				Expr(
#line  2915 "VBNET.ATG" 
out expr);
			}

#line  2915 "VBNET.ATG" 
			statement = new ReturnStatement(expr); 
		} else if (la.kind == 209) {
			lexer.NextToken();
			Expr(
#line  2917 "VBNET.ATG" 
out expr);
			EndOfStmt();
			Block(
#line  2917 "VBNET.ATG" 
out embeddedStatement);
			Expect(112);
			Expect(209);

#line  2918 "VBNET.ATG" 
			statement = new LockStatement(expr, embeddedStatement); 
		} else if (la.kind == 187) {
			lexer.NextToken();
			Identifier();

#line  2920 "VBNET.ATG" 
			name = t.val; 
			if (la.kind == 36) {
				lexer.NextToken();
				if (StartOf(35)) {
					ArgumentList(
#line  2921 "VBNET.ATG" 
out p);
				}
				Expect(37);
			}

#line  2923 "VBNET.ATG" 
			statement = new RaiseEventStatement(name, p);
			
		} else if (la.kind == 231) {
			WithStatement(
#line  2926 "VBNET.ATG" 
out statement);
		} else if (la.kind == 55) {
			lexer.NextToken();

#line  2928 "VBNET.ATG" 
			Expression handlerExpr = null; 
			Expr(
#line  2929 "VBNET.ATG" 
out expr);
			Expect(23);
			Expr(
#line  2929 "VBNET.ATG" 
out handlerExpr);

#line  2931 "VBNET.ATG" 
			statement = new AddHandlerStatement(expr, handlerExpr);
			
		} else if (la.kind == 191) {
			lexer.NextToken();

#line  2934 "VBNET.ATG" 
			Expression handlerExpr = null; 
			Expr(
#line  2935 "VBNET.ATG" 
out expr);
			Expect(23);
			Expr(
#line  2935 "VBNET.ATG" 
out handlerExpr);

#line  2937 "VBNET.ATG" 
			statement = new RemoveHandlerStatement(expr, handlerExpr);
			
		} else if (la.kind == 229) {
			lexer.NextToken();
			Expr(
#line  2940 "VBNET.ATG" 
out expr);
			EndOfStmt();
			Block(
#line  2941 "VBNET.ATG" 
out embeddedStatement);
			Expect(112);
			Expect(229);

#line  2943 "VBNET.ATG" 
			statement = new DoLoopStatement(expr, embeddedStatement, ConditionType.While, ConditionPosition.Start);
			
		} else if (la.kind == 107) {
			lexer.NextToken();

#line  2948 "VBNET.ATG" 
			ConditionType conditionType = ConditionType.None;
			
			if (la.kind == 222 || la.kind == 229) {
				WhileOrUntil(
#line  2951 "VBNET.ATG" 
out conditionType);
				Expr(
#line  2951 "VBNET.ATG" 
out expr);
				EndOfStmt();
				Block(
#line  2952 "VBNET.ATG" 
out embeddedStatement);
				Expect(151);

#line  2955 "VBNET.ATG" 
				statement = new DoLoopStatement(expr, 
				                               embeddedStatement, 
				                               conditionType == ConditionType.While ? ConditionType.DoWhile : conditionType, 
				                               ConditionPosition.Start);
				
			} else if (la.kind == 1 || la.kind == 22) {
				EndOfStmt();
				Block(
#line  2962 "VBNET.ATG" 
out embeddedStatement);
				Expect(151);
				if (la.kind == 222 || la.kind == 229) {
					WhileOrUntil(
#line  2963 "VBNET.ATG" 
out conditionType);
					Expr(
#line  2963 "VBNET.ATG" 
out expr);
				}

#line  2965 "VBNET.ATG" 
				statement = new DoLoopStatement(expr, embeddedStatement, conditionType, ConditionPosition.End);
				
			} else SynErr(283);
		} else if (la.kind == 123) {
			lexer.NextToken();

#line  2970 "VBNET.ATG" 
			Expression group = null;
			TypeReference typeReference;
			string        typeName;
			Location startLocation = t.Location;
			
			if (la.kind == 109) {
				lexer.NextToken();
				LoopControlVariable(
#line  2977 "VBNET.ATG" 
out typeReference, out typeName);
				Expect(137);
				Expr(
#line  2978 "VBNET.ATG" 
out group);
				EndOfStmt();
				Block(
#line  2979 "VBNET.ATG" 
out embeddedStatement);
				Expect(162);
				if (StartOf(29)) {
					Expr(
#line  2980 "VBNET.ATG" 
out expr);
				}

#line  2982 "VBNET.ATG" 
				statement = new ForeachStatement(typeReference, 
				                                typeName,
				                                group, 
				                                embeddedStatement, 
				                                expr);
				statement.StartLocation = startLocation;
				statement.EndLocation   = t.EndLocation;
				
				
			} else if (StartOf(36)) {

#line  2993 "VBNET.ATG" 
				Expression start = null;
				Expression end = null;
				Expression step = null;
				Expression variableExpr = null;
				Expression nextExpr = null;
				List<Expression> nextExpressions = null;
				
				if (
#line  3000 "VBNET.ATG" 
IsLoopVariableDeclaration()) {
					LoopControlVariable(
#line  3001 "VBNET.ATG" 
out typeReference, out typeName);
				} else {

#line  3003 "VBNET.ATG" 
					typeReference = null; typeName = null; 
					SimpleExpr(
#line  3004 "VBNET.ATG" 
out variableExpr);
				}
				Expect(21);
				Expr(
#line  3006 "VBNET.ATG" 
out start);
				Expect(214);
				Expr(
#line  3006 "VBNET.ATG" 
out end);
				if (la.kind == 203) {
					lexer.NextToken();
					Expr(
#line  3006 "VBNET.ATG" 
out step);
				}
				EndOfStmt();
				Block(
#line  3007 "VBNET.ATG" 
out embeddedStatement);
				Expect(162);
				if (StartOf(29)) {
					Expr(
#line  3010 "VBNET.ATG" 
out nextExpr);

#line  3012 "VBNET.ATG" 
					nextExpressions = new List<Expression>();
					nextExpressions.Add(nextExpr);
					
					while (la.kind == 23) {
						lexer.NextToken();
						Expr(
#line  3015 "VBNET.ATG" 
out nextExpr);

#line  3015 "VBNET.ATG" 
						nextExpressions.Add(nextExpr); 
					}
				}

#line  3018 "VBNET.ATG" 
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
				
			} else SynErr(284);
		} else if (la.kind == 117) {
			lexer.NextToken();
			Expr(
#line  3031 "VBNET.ATG" 
out expr);

#line  3031 "VBNET.ATG" 
			statement = new ErrorStatement(expr); 
		} else if (la.kind == 189) {
			lexer.NextToken();

#line  3033 "VBNET.ATG" 
			bool isPreserve = false; 
			if (la.kind == 182) {
				lexer.NextToken();

#line  3033 "VBNET.ATG" 
				isPreserve = true; 
			}
			ReDimClause(
#line  3034 "VBNET.ATG" 
out expr);

#line  3036 "VBNET.ATG" 
			ReDimStatement reDimStatement = new ReDimStatement(isPreserve);
			statement = reDimStatement;
			SafeAdd(reDimStatement, reDimStatement.ReDimClauses, expr as InvocationExpression);
			
			while (la.kind == 23) {
				lexer.NextToken();
				ReDimClause(
#line  3040 "VBNET.ATG" 
out expr);

#line  3041 "VBNET.ATG" 
				SafeAdd(reDimStatement, reDimStatement.ReDimClauses, expr as InvocationExpression); 
			}
		} else if (la.kind == 116) {
			lexer.NextToken();
			Expr(
#line  3045 "VBNET.ATG" 
out expr);

#line  3047 "VBNET.ATG" 
			EraseStatement eraseStatement = new EraseStatement();
			if (expr != null) { SafeAdd(eraseStatement, eraseStatement.Expressions, expr);}
			
			while (la.kind == 23) {
				lexer.NextToken();
				Expr(
#line  3050 "VBNET.ATG" 
out expr);

#line  3050 "VBNET.ATG" 
				if (expr != null) { SafeAdd(eraseStatement, eraseStatement.Expressions, expr); }
			}

#line  3051 "VBNET.ATG" 
			statement = eraseStatement; 
		} else if (la.kind == 204) {
			lexer.NextToken();

#line  3053 "VBNET.ATG" 
			statement = new StopStatement(); 
		} else if (
#line  3055 "VBNET.ATG" 
la.kind == Tokens.If) {
			Expect(134);

#line  3056 "VBNET.ATG" 
			Location ifStartLocation = t.Location; 
			Expr(
#line  3056 "VBNET.ATG" 
out expr);
			if (la.kind == 212) {
				lexer.NextToken();
			}
			if (la.kind == 1 || la.kind == 22) {
				EndOfStmt();
				Block(
#line  3059 "VBNET.ATG" 
out embeddedStatement);

#line  3061 "VBNET.ATG" 
				IfElseStatement ifStatement = new IfElseStatement(expr, embeddedStatement);
				ifStatement.StartLocation = ifStartLocation;
				Location elseIfStart;
				
				while (la.kind == 111 || 
#line  3067 "VBNET.ATG" 
IsElseIf()) {
					if (
#line  3067 "VBNET.ATG" 
IsElseIf()) {
						Expect(110);

#line  3067 "VBNET.ATG" 
						elseIfStart = t.Location; 
						Expect(134);
					} else {
						lexer.NextToken();

#line  3068 "VBNET.ATG" 
						elseIfStart = t.Location; 
					}

#line  3070 "VBNET.ATG" 
					Expression condition = null; Statement block = null; 
					Expr(
#line  3071 "VBNET.ATG" 
out condition);
					if (la.kind == 212) {
						lexer.NextToken();
					}
					EndOfStmt();
					Block(
#line  3072 "VBNET.ATG" 
out block);

#line  3074 "VBNET.ATG" 
					ElseIfSection elseIfSection = new ElseIfSection(condition, block);
					elseIfSection.StartLocation = elseIfStart;
					elseIfSection.EndLocation = t.Location;
					elseIfSection.Parent = ifStatement;
					ifStatement.ElseIfSections.Add(elseIfSection);
					
				}
				if (la.kind == 110) {
					lexer.NextToken();
					if (la.kind == 1 || la.kind == 22) {
						EndOfStmt();
					}
					Block(
#line  3083 "VBNET.ATG" 
out embeddedStatement);

#line  3085 "VBNET.ATG" 
					ifStatement.FalseStatement.Add(embeddedStatement);
					
				}
				Expect(112);
				Expect(134);

#line  3089 "VBNET.ATG" 
				ifStatement.EndLocation = t.Location;
				statement = ifStatement;
				
			} else if (StartOf(37)) {

#line  3094 "VBNET.ATG" 
				IfElseStatement ifStatement = new IfElseStatement(expr);
				ifStatement.StartLocation = ifStartLocation;
				
				SingleLineStatementList(
#line  3097 "VBNET.ATG" 
ifStatement.TrueStatement);
				if (la.kind == 110) {
					lexer.NextToken();
					if (StartOf(37)) {
						SingleLineStatementList(
#line  3100 "VBNET.ATG" 
ifStatement.FalseStatement);
					}
				}

#line  3102 "VBNET.ATG" 
				ifStatement.EndLocation = t.Location; statement = ifStatement; 
			} else SynErr(285);
		} else if (la.kind == 195) {
			lexer.NextToken();
			if (la.kind == 73) {
				lexer.NextToken();
			}
			Expr(
#line  3105 "VBNET.ATG" 
out expr);
			EndOfStmt();

#line  3106 "VBNET.ATG" 
			List<SwitchSection> selectSections = new List<SwitchSection>();
			Statement block = null;
			
			while (la.kind == 73) {

#line  3110 "VBNET.ATG" 
				List<CaseLabel> caseClauses = null; Location caseLocation = la.Location; 
				lexer.NextToken();
				CaseClauses(
#line  3111 "VBNET.ATG" 
out caseClauses);
				if (
#line  3111 "VBNET.ATG" 
IsNotStatementSeparator()) {
					lexer.NextToken();
				}
				EndOfStmt();

#line  3113 "VBNET.ATG" 
				SwitchSection selectSection = new SwitchSection(caseClauses);
				selectSection.StartLocation = caseLocation;
				
				Block(
#line  3116 "VBNET.ATG" 
out block);

#line  3118 "VBNET.ATG" 
				selectSection.Children = block.Children;
				selectSection.EndLocation = t.EndLocation;
				selectSections.Add(selectSection);
				
			}

#line  3124 "VBNET.ATG" 
			statement = new SwitchStatement(expr, selectSections);
			
			Expect(112);
			Expect(195);
		} else if (la.kind == 170) {

#line  3127 "VBNET.ATG" 
			OnErrorStatement onErrorStatement = null; 
			OnErrorStatement(
#line  3128 "VBNET.ATG" 
out onErrorStatement);

#line  3128 "VBNET.ATG" 
			statement = onErrorStatement; 
		} else if (la.kind == 131) {

#line  3129 "VBNET.ATG" 
			GotoStatement goToStatement = null; 
			GotoStatement(
#line  3130 "VBNET.ATG" 
out goToStatement);

#line  3130 "VBNET.ATG" 
			statement = goToStatement; 
		} else if (la.kind == 192) {

#line  3131 "VBNET.ATG" 
			ResumeStatement resumeStatement = null; 
			ResumeStatement(
#line  3132 "VBNET.ATG" 
out resumeStatement);

#line  3132 "VBNET.ATG" 
			statement = resumeStatement; 
		} else if (StartOf(36)) {

#line  3135 "VBNET.ATG" 
			Expression val = null;
			AssignmentOperatorType op;
			
			bool mustBeAssignment = la.kind == Tokens.Plus  || la.kind == Tokens.Minus ||
			                        la.kind == Tokens.Not   || la.kind == Tokens.Times;
			
			SimpleExpr(
#line  3141 "VBNET.ATG" 
out expr);
			if (StartOf(38)) {
				AssignmentOperator(
#line  3143 "VBNET.ATG" 
out op);
				Expr(
#line  3143 "VBNET.ATG" 
out val);

#line  3143 "VBNET.ATG" 
				expr = new AssignmentExpression(expr, op, val); 
			} else if (StartOf(39)) {

#line  3144 "VBNET.ATG" 
				if (mustBeAssignment) Error("error in assignment."); 
			} else SynErr(286);

#line  3147 "VBNET.ATG" 
			// a field reference expression that stands alone is a
			// invocation expression without parantheses and arguments
			if(expr is MemberReferenceExpression || expr is IdentifierExpression) {
				expr = new InvocationExpression(expr);
			}
			statement = new ExpressionStatement(expr);
			
		} else if (la.kind == 72) {
			lexer.NextToken();
			SimpleExpr(
#line  3154 "VBNET.ATG" 
out expr);

#line  3154 "VBNET.ATG" 
			statement = new ExpressionStatement(expr); 
		} else if (la.kind == 224) {
			lexer.NextToken();

#line  3156 "VBNET.ATG" 
			Statement block;  
			if (
#line  3157 "VBNET.ATG" 
Peek(1).kind == Tokens.As) {

#line  3158 "VBNET.ATG" 
				LocalVariableDeclaration resourceAquisition = new LocalVariableDeclaration(Modifiers.None); 
				VariableDeclarator(
#line  3159 "VBNET.ATG" 
resourceAquisition.Variables);
				while (la.kind == 23) {
					lexer.NextToken();
					VariableDeclarator(
#line  3161 "VBNET.ATG" 
resourceAquisition.Variables);
				}
				Block(
#line  3163 "VBNET.ATG" 
out block);

#line  3165 "VBNET.ATG" 
				statement = new UsingStatement(resourceAquisition, block);
				
			} else if (StartOf(29)) {
				Expr(
#line  3167 "VBNET.ATG" 
out expr);
				Block(
#line  3168 "VBNET.ATG" 
out block);

#line  3169 "VBNET.ATG" 
				statement = new UsingStatement(new ExpressionStatement(expr), block); 
			} else SynErr(287);
			Expect(112);
			Expect(224);
		} else if (StartOf(40)) {
			LocalDeclarationStatement(
#line  3172 "VBNET.ATG" 
out statement);
		} else SynErr(288);
	}

	void FromOrAggregateQueryOperator(
#line  2140 "VBNET.ATG" 
List<QueryExpressionClause> middleClauses) {

#line  2142 "VBNET.ATG" 
		QueryExpressionFromClause fromClause = null;
		QueryExpressionAggregateClause aggregateClause = null;
		
		if (la.kind == 125) {
			FromQueryOperator(
#line  2145 "VBNET.ATG" 
out fromClause);

#line  2146 "VBNET.ATG" 
			middleClauses.Add(fromClause); 
		} else if (la.kind == 57) {
			AggregateQueryOperator(
#line  2147 "VBNET.ATG" 
out aggregateClause);

#line  2148 "VBNET.ATG" 
			middleClauses.Add(aggregateClause); 
		} else SynErr(289);
	}

	void QueryOperator(
#line  2151 "VBNET.ATG" 
List<QueryExpressionClause> middleClauses) {

#line  2153 "VBNET.ATG" 
		QueryExpressionJoinVBClause joinClause = null;
		QueryExpressionGroupVBClause groupByClause = null;
		QueryExpressionPartitionVBClause partitionClause = null;
		QueryExpressionGroupJoinVBClause groupJoinClause = null;
		QueryExpressionFromClause fromClause = null;
		QueryExpressionAggregateClause aggregateClause = null;
		
		if (la.kind == 125) {
			FromQueryOperator(
#line  2160 "VBNET.ATG" 
out fromClause);

#line  2161 "VBNET.ATG" 
			middleClauses.Add(fromClause); 
		} else if (la.kind == 57) {
			AggregateQueryOperator(
#line  2162 "VBNET.ATG" 
out aggregateClause);

#line  2163 "VBNET.ATG" 
			middleClauses.Add(aggregateClause); 
		} else if (la.kind == 195) {
			SelectQueryOperator(
#line  2164 "VBNET.ATG" 
middleClauses);
		} else if (la.kind == 106) {
			DistinctQueryOperator(
#line  2165 "VBNET.ATG" 
middleClauses);
		} else if (la.kind == 228) {
			WhereQueryOperator(
#line  2166 "VBNET.ATG" 
middleClauses);
		} else if (la.kind == 175) {
			OrderByQueryOperator(
#line  2167 "VBNET.ATG" 
middleClauses);
		} else if (la.kind == 201 || la.kind == 210) {
			PartitionQueryOperator(
#line  2168 "VBNET.ATG" 
out partitionClause);

#line  2169 "VBNET.ATG" 
			middleClauses.Add(partitionClause); 
		} else if (la.kind == 147) {
			LetQueryOperator(
#line  2170 "VBNET.ATG" 
middleClauses);
		} else if (la.kind == 145) {
			JoinQueryOperator(
#line  2171 "VBNET.ATG" 
out joinClause);

#line  2172 "VBNET.ATG" 
			middleClauses.Add(joinClause); 
		} else if (
#line  2173 "VBNET.ATG" 
la.kind == Tokens.Group && Peek(1).kind == Tokens.Join) {
			GroupJoinQueryOperator(
#line  2173 "VBNET.ATG" 
out groupJoinClause);

#line  2174 "VBNET.ATG" 
			middleClauses.Add(groupJoinClause); 
		} else if (la.kind == 132) {
			GroupByQueryOperator(
#line  2175 "VBNET.ATG" 
out groupByClause);

#line  2176 "VBNET.ATG" 
			middleClauses.Add(groupByClause); 
		} else SynErr(290);
	}

	void FromQueryOperator(
#line  2251 "VBNET.ATG" 
out QueryExpressionFromClause fromClause) {

#line  2253 "VBNET.ATG" 
		fromClause = new QueryExpressionFromClause();
		fromClause.StartLocation = la.Location;
		
		Expect(125);
		CollectionRangeVariableDeclarationList(
#line  2256 "VBNET.ATG" 
fromClause.Sources);

#line  2258 "VBNET.ATG" 
		fromClause.EndLocation = t.EndLocation;
		
	}

	void AggregateQueryOperator(
#line  2320 "VBNET.ATG" 
out QueryExpressionAggregateClause aggregateClause) {

#line  2322 "VBNET.ATG" 
		aggregateClause = new QueryExpressionAggregateClause();
		aggregateClause.IntoVariables = new List<ExpressionRangeVariable>();
		aggregateClause.StartLocation = la.Location;
		CollectionRangeVariable source;
		
		Expect(57);
		CollectionRangeVariableDeclaration(
#line  2327 "VBNET.ATG" 
out source);

#line  2329 "VBNET.ATG" 
		aggregateClause.Source = source;
		
		while (StartOf(30)) {
			QueryOperator(
#line  2332 "VBNET.ATG" 
aggregateClause.MiddleClauses);
		}
		Expect(142);
		ExpressionRangeVariableDeclarationList(
#line  2334 "VBNET.ATG" 
aggregateClause.IntoVariables);

#line  2336 "VBNET.ATG" 
		aggregateClause.EndLocation = t.EndLocation;
		
	}

	void SelectQueryOperator(
#line  2262 "VBNET.ATG" 
List<QueryExpressionClause> middleClauses) {

#line  2264 "VBNET.ATG" 
		QueryExpressionSelectVBClause selectClause = new QueryExpressionSelectVBClause();
		selectClause.StartLocation = la.Location;
		
		Expect(195);
		ExpressionRangeVariableDeclarationList(
#line  2267 "VBNET.ATG" 
selectClause.Variables);

#line  2269 "VBNET.ATG" 
		selectClause.EndLocation = t.Location;
		middleClauses.Add(selectClause);
		
	}

	void DistinctQueryOperator(
#line  2274 "VBNET.ATG" 
List<QueryExpressionClause> middleClauses) {

#line  2276 "VBNET.ATG" 
		QueryExpressionDistinctClause distinctClause = new QueryExpressionDistinctClause();
		distinctClause.StartLocation = la.Location;
		
		Expect(106);

#line  2281 "VBNET.ATG" 
		distinctClause.EndLocation = t.EndLocation;
		middleClauses.Add(distinctClause);
		
	}

	void WhereQueryOperator(
#line  2286 "VBNET.ATG" 
List<QueryExpressionClause> middleClauses) {

#line  2288 "VBNET.ATG" 
		QueryExpressionWhereClause whereClause = new QueryExpressionWhereClause();
		whereClause.StartLocation = la.Location;
		Expression operand = null;
		
		Expect(228);
		Expr(
#line  2292 "VBNET.ATG" 
out operand);

#line  2294 "VBNET.ATG" 
		whereClause.Condition = operand;
		whereClause.EndLocation = t.EndLocation;
		
		middleClauses.Add(whereClause);
		
	}

	void OrderByQueryOperator(
#line  2179 "VBNET.ATG" 
List<QueryExpressionClause> middleClauses) {

#line  2181 "VBNET.ATG" 
		QueryExpressionOrderClause orderClause = new QueryExpressionOrderClause();
		orderClause.StartLocation = la.Location;
		List<QueryExpressionOrdering> orderings = null;
		
		Expect(175);
		Expect(69);
		OrderExpressionList(
#line  2185 "VBNET.ATG" 
out orderings);

#line  2187 "VBNET.ATG" 
		orderClause.Orderings = orderings;
		orderClause.EndLocation = t.EndLocation;
		middleClauses.Add(orderClause);
		
	}

	void PartitionQueryOperator(
#line  2301 "VBNET.ATG" 
out QueryExpressionPartitionVBClause partitionClause) {

#line  2303 "VBNET.ATG" 
		partitionClause = new QueryExpressionPartitionVBClause();
		partitionClause.StartLocation = la.Location;
		Expression expr = null;
		
		if (la.kind == 210) {
			lexer.NextToken();

#line  2308 "VBNET.ATG" 
			partitionClause.PartitionType = QueryExpressionPartitionType.Take; 
			if (la.kind == 229) {
				lexer.NextToken();

#line  2309 "VBNET.ATG" 
				partitionClause.PartitionType = QueryExpressionPartitionType.TakeWhile; 
			}
		} else if (la.kind == 201) {
			lexer.NextToken();

#line  2310 "VBNET.ATG" 
			partitionClause.PartitionType = QueryExpressionPartitionType.Skip; 
			if (la.kind == 229) {
				lexer.NextToken();

#line  2311 "VBNET.ATG" 
				partitionClause.PartitionType = QueryExpressionPartitionType.SkipWhile; 
			}
		} else SynErr(291);
		Expr(
#line  2313 "VBNET.ATG" 
out expr);

#line  2315 "VBNET.ATG" 
		partitionClause.Expression = expr;
		partitionClause.EndLocation = t.EndLocation;
		
	}

	void LetQueryOperator(
#line  2340 "VBNET.ATG" 
List<QueryExpressionClause> middleClauses) {

#line  2342 "VBNET.ATG" 
		QueryExpressionLetVBClause letClause = new QueryExpressionLetVBClause();
		letClause.StartLocation = la.Location;
		
		Expect(147);
		ExpressionRangeVariableDeclarationList(
#line  2345 "VBNET.ATG" 
letClause.Variables);

#line  2347 "VBNET.ATG" 
		letClause.EndLocation = t.EndLocation;
		middleClauses.Add(letClause);
		
	}

	void JoinQueryOperator(
#line  2384 "VBNET.ATG" 
out QueryExpressionJoinVBClause joinClause) {

#line  2386 "VBNET.ATG" 
		joinClause = new QueryExpressionJoinVBClause();
		joinClause.StartLocation = la.Location;
		CollectionRangeVariable joinVariable = null;
		QueryExpressionJoinVBClause subJoin = null;
		QueryExpressionJoinConditionVB condition = null;
		
		
		Expect(145);
		CollectionRangeVariableDeclaration(
#line  2393 "VBNET.ATG" 
out joinVariable);

#line  2394 "VBNET.ATG" 
		joinClause.JoinVariable = joinVariable; 
		if (la.kind == 145) {
			JoinQueryOperator(
#line  2396 "VBNET.ATG" 
out subJoin);

#line  2397 "VBNET.ATG" 
			joinClause.SubJoin = subJoin; 
		}
		Expect(170);
		JoinCondition(
#line  2400 "VBNET.ATG" 
out condition);

#line  2401 "VBNET.ATG" 
		SafeAdd(joinClause, joinClause.Conditions, condition); 
		while (la.kind == 59) {
			lexer.NextToken();
			JoinCondition(
#line  2403 "VBNET.ATG" 
out condition);

#line  2404 "VBNET.ATG" 
			SafeAdd(joinClause, joinClause.Conditions, condition); 
		}

#line  2407 "VBNET.ATG" 
		joinClause.EndLocation = t.EndLocation;
		
	}

	void GroupJoinQueryOperator(
#line  2237 "VBNET.ATG" 
out QueryExpressionGroupJoinVBClause groupJoinClause) {

#line  2239 "VBNET.ATG" 
		groupJoinClause = new QueryExpressionGroupJoinVBClause();
		groupJoinClause.StartLocation = la.Location;
		QueryExpressionJoinVBClause joinClause = null;
		
		Expect(132);
		JoinQueryOperator(
#line  2243 "VBNET.ATG" 
out joinClause);
		Expect(142);
		ExpressionRangeVariableDeclarationList(
#line  2244 "VBNET.ATG" 
groupJoinClause.IntoVariables);

#line  2246 "VBNET.ATG" 
		groupJoinClause.JoinClause = joinClause;
		groupJoinClause.EndLocation = t.EndLocation;
		
	}

	void GroupByQueryOperator(
#line  2224 "VBNET.ATG" 
out QueryExpressionGroupVBClause groupByClause) {

#line  2226 "VBNET.ATG" 
		groupByClause = new QueryExpressionGroupVBClause();
		groupByClause.StartLocation = la.Location;
		
		Expect(132);
		ExpressionRangeVariableDeclarationList(
#line  2229 "VBNET.ATG" 
groupByClause.GroupVariables);
		Expect(69);
		ExpressionRangeVariableDeclarationList(
#line  2230 "VBNET.ATG" 
groupByClause.ByVariables);
		Expect(142);
		ExpressionRangeVariableDeclarationList(
#line  2231 "VBNET.ATG" 
groupByClause.IntoVariables);

#line  2233 "VBNET.ATG" 
		groupByClause.EndLocation = t.EndLocation;
		
	}

	void OrderExpressionList(
#line  2193 "VBNET.ATG" 
out List<QueryExpressionOrdering> orderings) {

#line  2195 "VBNET.ATG" 
		orderings = new List<QueryExpressionOrdering>();
		QueryExpressionOrdering ordering = null;
		
		OrderExpression(
#line  2198 "VBNET.ATG" 
out ordering);

#line  2199 "VBNET.ATG" 
		orderings.Add(ordering); 
		while (la.kind == 23) {
			lexer.NextToken();
			OrderExpression(
#line  2201 "VBNET.ATG" 
out ordering);

#line  2202 "VBNET.ATG" 
			orderings.Add(ordering); 
		}
	}

	void OrderExpression(
#line  2206 "VBNET.ATG" 
out QueryExpressionOrdering ordering) {

#line  2208 "VBNET.ATG" 
		ordering = new QueryExpressionOrdering();
		ordering.StartLocation = la.Location;
		ordering.Direction = QueryExpressionOrderingDirection.None;
		Expression orderExpr = null;
		
		Expr(
#line  2213 "VBNET.ATG" 
out orderExpr);

#line  2215 "VBNET.ATG" 
		ordering.Criteria = orderExpr;
		
		if (la.kind == 63 || la.kind == 103) {
			if (la.kind == 63) {
				lexer.NextToken();

#line  2218 "VBNET.ATG" 
				ordering.Direction = QueryExpressionOrderingDirection.Ascending; 
			} else {
				lexer.NextToken();

#line  2219 "VBNET.ATG" 
				ordering.Direction = QueryExpressionOrderingDirection.Descending; 
			}
		}

#line  2221 "VBNET.ATG" 
		ordering.EndLocation = t.EndLocation; 
	}

	void ExpressionRangeVariableDeclarationList(
#line  2352 "VBNET.ATG" 
List<ExpressionRangeVariable> variables) {

#line  2354 "VBNET.ATG" 
		ExpressionRangeVariable variable = null;
		
		ExpressionRangeVariableDeclaration(
#line  2356 "VBNET.ATG" 
out variable);

#line  2357 "VBNET.ATG" 
		variables.Add(variable); 
		while (la.kind == 23) {
			lexer.NextToken();
			ExpressionRangeVariableDeclaration(
#line  2358 "VBNET.ATG" 
out variable);

#line  2358 "VBNET.ATG" 
			variables.Add(variable); 
		}
	}

	void CollectionRangeVariableDeclarationList(
#line  2411 "VBNET.ATG" 
List<CollectionRangeVariable> rangeVariables) {

#line  2412 "VBNET.ATG" 
		CollectionRangeVariable variableDeclaration; 
		CollectionRangeVariableDeclaration(
#line  2414 "VBNET.ATG" 
out variableDeclaration);

#line  2415 "VBNET.ATG" 
		rangeVariables.Add(variableDeclaration); 
		while (la.kind == 23) {
			lexer.NextToken();
			CollectionRangeVariableDeclaration(
#line  2416 "VBNET.ATG" 
out variableDeclaration);

#line  2416 "VBNET.ATG" 
			rangeVariables.Add(variableDeclaration); 
		}
	}

	void CollectionRangeVariableDeclaration(
#line  2419 "VBNET.ATG" 
out CollectionRangeVariable rangeVariable) {

#line  2421 "VBNET.ATG" 
		rangeVariable = new CollectionRangeVariable();
		rangeVariable.StartLocation = la.Location;
		TypeReference typeName = null;
		Expression inExpr = null;
		
		Identifier();

#line  2426 "VBNET.ATG" 
		rangeVariable.Identifier = t.val; 
		if (la.kind == 62) {
			lexer.NextToken();
			TypeName(
#line  2427 "VBNET.ATG" 
out typeName);

#line  2427 "VBNET.ATG" 
			rangeVariable.Type = typeName; 
		}
		Expect(137);
		Expr(
#line  2428 "VBNET.ATG" 
out inExpr);

#line  2430 "VBNET.ATG" 
		rangeVariable.Expression = inExpr;
		rangeVariable.EndLocation = t.EndLocation;
		
	}

	void ExpressionRangeVariableDeclaration(
#line  2361 "VBNET.ATG" 
out ExpressionRangeVariable variable) {

#line  2363 "VBNET.ATG" 
		variable = new ExpressionRangeVariable();
		variable.StartLocation = la.Location;
		Expression rhs = null;
		TypeReference typeName = null;
		
		if (
#line  2369 "VBNET.ATG" 
IsIdentifiedExpressionRange()) {
			Identifier();

#line  2370 "VBNET.ATG" 
			variable.Identifier = t.val; 
			if (la.kind == 62) {
				lexer.NextToken();
				TypeName(
#line  2372 "VBNET.ATG" 
out typeName);

#line  2373 "VBNET.ATG" 
				variable.Type = typeName; 
			}
			Expect(21);
		}
		Expr(
#line  2377 "VBNET.ATG" 
out rhs);

#line  2379 "VBNET.ATG" 
		variable.Expression = rhs;
		variable.EndLocation = t.EndLocation;
		
	}

	void JoinCondition(
#line  2435 "VBNET.ATG" 
out QueryExpressionJoinConditionVB condition) {

#line  2437 "VBNET.ATG" 
		condition = new QueryExpressionJoinConditionVB();
		condition.StartLocation = la.Location;
		
		Expression lhs = null;
		Expression rhs = null;
		
		Expr(
#line  2443 "VBNET.ATG" 
out lhs);
		Expect(115);
		Expr(
#line  2443 "VBNET.ATG" 
out rhs);

#line  2445 "VBNET.ATG" 
		condition.LeftSide = lhs;
		condition.RightSide = rhs;
		condition.EndLocation = t.EndLocation;
		
	}

	void Argument(
#line  2509 "VBNET.ATG" 
out Expression argumentexpr) {

#line  2511 "VBNET.ATG" 
		Expression expr;
		argumentexpr = null;
		string name;
		
		if (
#line  2515 "VBNET.ATG" 
IsNamedAssign()) {
			Identifier();

#line  2515 "VBNET.ATG" 
			name = t.val;  
			Expect(54);
			Expr(
#line  2515 "VBNET.ATG" 
out expr);

#line  2517 "VBNET.ATG" 
			argumentexpr = new NamedArgumentExpression(name, expr);
			
		} else if (StartOf(29)) {
			Expr(
#line  2520 "VBNET.ATG" 
out argumentexpr);
		} else SynErr(292);
	}

	void QualIdentAndTypeArguments(
#line  2586 "VBNET.ATG" 
out TypeReference typeref, bool canBeUnbound) {

#line  2587 "VBNET.ATG" 
		string name; typeref = null; 
		Qualident(
#line  2589 "VBNET.ATG" 
out name);

#line  2590 "VBNET.ATG" 
		typeref = new TypeReference(name); 
		if (
#line  2591 "VBNET.ATG" 
la.kind == Tokens.OpenParenthesis && Peek(1).kind == Tokens.Of) {
			lexer.NextToken();
			Expect(168);
			if (
#line  2593 "VBNET.ATG" 
canBeUnbound && (la.kind == Tokens.CloseParenthesis || la.kind == Tokens.Comma)) {

#line  2594 "VBNET.ATG" 
				typeref.GenericTypes.Add(NullTypeReference.Instance); 
				while (la.kind == 23) {
					lexer.NextToken();

#line  2595 "VBNET.ATG" 
					typeref.GenericTypes.Add(NullTypeReference.Instance); 
				}
			} else if (StartOf(8)) {
				TypeArgumentList(
#line  2596 "VBNET.ATG" 
typeref.GenericTypes);
			} else SynErr(293);
			Expect(37);
		}
	}

	void RankList(
#line  2633 "VBNET.ATG" 
out int i) {

#line  2634 "VBNET.ATG" 
		i = 0; 
		while (la.kind == 23) {
			lexer.NextToken();

#line  2635 "VBNET.ATG" 
			++i; 
		}
	}

	void Attribute(
#line  2674 "VBNET.ATG" 
out ASTAttribute attribute) {

#line  2675 "VBNET.ATG" 
		string name;
		List<Expression> positional = new List<Expression>();
		List<NamedArgumentExpression> named = new List<NamedArgumentExpression>();
		
		if (la.kind == 129) {
			lexer.NextToken();
			Expect(27);
		}
		Qualident(
#line  2680 "VBNET.ATG" 
out name);
		if (la.kind == 36) {
			AttributeArguments(
#line  2681 "VBNET.ATG" 
positional, named);
		}

#line  2683 "VBNET.ATG" 
		attribute  = new ASTAttribute(name, positional, named);
		
	}

	void AttributeArguments(
#line  2688 "VBNET.ATG" 
List<Expression> positional, List<NamedArgumentExpression> named) {

#line  2690 "VBNET.ATG" 
		bool nameFound = false;
		string name = "";
		Expression expr;
		
		Expect(36);
		if (
#line  2696 "VBNET.ATG" 
IsNotClosingParenthesis()) {
			if (
#line  2698 "VBNET.ATG" 
IsNamedAssign()) {

#line  2698 "VBNET.ATG" 
				nameFound = true; 
				IdentifierOrKeyword(
#line  2699 "VBNET.ATG" 
out name);
				if (la.kind == 54) {
					lexer.NextToken();
				} else if (la.kind == 21) {
					lexer.NextToken();
				} else SynErr(294);
			}
			Expr(
#line  2701 "VBNET.ATG" 
out expr);

#line  2703 "VBNET.ATG" 
			if (expr != null) {
			if (string.IsNullOrEmpty(name)) { positional.Add(expr); }
			else { named.Add(new NamedArgumentExpression(name, expr)); name = ""; }
			}
			
			while (la.kind == 23) {
				lexer.NextToken();
				if (
#line  2711 "VBNET.ATG" 
IsNamedAssign()) {

#line  2711 "VBNET.ATG" 
					nameFound = true; 
					IdentifierOrKeyword(
#line  2712 "VBNET.ATG" 
out name);
					if (la.kind == 54) {
						lexer.NextToken();
					} else if (la.kind == 21) {
						lexer.NextToken();
					} else SynErr(295);
				} else if (StartOf(29)) {

#line  2714 "VBNET.ATG" 
					if (nameFound) Error("no positional argument after named argument"); 
				} else SynErr(296);
				Expr(
#line  2715 "VBNET.ATG" 
out expr);

#line  2715 "VBNET.ATG" 
				if (expr != null) { if(name == "") positional.Add(expr);
				else { named.Add(new NamedArgumentExpression(name, expr)); name = ""; }
				}
				
			}
		}
		Expect(37);
	}

	void FormalParameter(
#line  2772 "VBNET.ATG" 
out ParameterDeclarationExpression p) {

#line  2774 "VBNET.ATG" 
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		TypeReference type = null;
		ParamModifierList mod = new ParamModifierList(this);
		Expression expr = null;
		p = null;
		ArrayList arrayModifiers = null;
		
		while (la.kind == 39) {
			AttributeSection(
#line  2783 "VBNET.ATG" 
out section);

#line  2783 "VBNET.ATG" 
			attributes.Add(section); 
		}
		while (StartOf(41)) {
			ParameterModifier(
#line  2784 "VBNET.ATG" 
mod);
		}
		Identifier();

#line  2785 "VBNET.ATG" 
		string parameterName = t.val; 
		if (
#line  2786 "VBNET.ATG" 
IsDims()) {
			ArrayTypeModifiers(
#line  2786 "VBNET.ATG" 
out arrayModifiers);
		}
		if (la.kind == 62) {
			lexer.NextToken();
			TypeName(
#line  2787 "VBNET.ATG" 
out type);
		}

#line  2789 "VBNET.ATG" 
		if(type != null) {
		if (arrayModifiers != null) {
			if (type.RankSpecifier != null) {
				Error("array rank only allowed one time");
			} else {
				type.RankSpecifier = (int[])arrayModifiers.ToArray(typeof(int));
			}
		}
		}
		
		if (la.kind == 21) {
			lexer.NextToken();
			Expr(
#line  2799 "VBNET.ATG" 
out expr);
		}

#line  2801 "VBNET.ATG" 
		mod.Check();
		p = new ParameterDeclarationExpression(type, parameterName, mod.Modifier, expr);
		p.Attributes = attributes;
		
	}

	void ParameterModifier(
#line  3483 "VBNET.ATG" 
ParamModifierList m) {
		if (la.kind == 71) {
			lexer.NextToken();

#line  3484 "VBNET.ATG" 
			m.Add(ParameterModifiers.In); 
		} else if (la.kind == 68) {
			lexer.NextToken();

#line  3485 "VBNET.ATG" 
			m.Add(ParameterModifiers.Ref); 
		} else if (la.kind == 173) {
			lexer.NextToken();

#line  3486 "VBNET.ATG" 
			m.Add(ParameterModifiers.Optional); 
		} else if (la.kind == 180) {
			lexer.NextToken();

#line  3487 "VBNET.ATG" 
			m.Add(ParameterModifiers.Params); 
		} else SynErr(297);
	}

	void Statement() {

#line  2830 "VBNET.ATG" 
		Statement stmt = null;
		Location startPos = la.Location;
		string label = String.Empty;
		
		
		if (la.kind == 1 || la.kind == 22) {
		} else if (
#line  2836 "VBNET.ATG" 
IsLabel()) {
			LabelName(
#line  2836 "VBNET.ATG" 
out label);

#line  2838 "VBNET.ATG" 
			AddChild(new LabelStatement(t.val));
			
			Expect(22);
			Statement();
		} else if (StartOf(42)) {
			EmbeddedStatement(
#line  2841 "VBNET.ATG" 
out stmt);

#line  2841 "VBNET.ATG" 
			AddChild(stmt); 
		} else SynErr(298);

#line  2844 "VBNET.ATG" 
		if (stmt != null) {
		stmt.StartLocation = startPos;
		stmt.EndLocation = t.Location;
		}
		
	}

	void LabelName(
#line  3259 "VBNET.ATG" 
out string name) {

#line  3261 "VBNET.ATG" 
		name = String.Empty;
		
		if (StartOf(4)) {
			Identifier();

#line  3263 "VBNET.ATG" 
			name = t.val; 
		} else if (la.kind == 5) {
			lexer.NextToken();

#line  3264 "VBNET.ATG" 
			name = t.val; 
		} else SynErr(299);
	}

	void LocalDeclarationStatement(
#line  2852 "VBNET.ATG" 
out Statement statement) {

#line  2854 "VBNET.ATG" 
		ModifierList m = new ModifierList();
		LocalVariableDeclaration localVariableDeclaration;
		bool dimfound = false;
		
		while (la.kind == 87 || la.kind == 104 || la.kind == 202) {
			if (la.kind == 87) {
				lexer.NextToken();

#line  2860 "VBNET.ATG" 
				m.Add(Modifiers.Const, t.Location); 
			} else if (la.kind == 202) {
				lexer.NextToken();

#line  2861 "VBNET.ATG" 
				m.Add(Modifiers.Static, t.Location); 
			} else {
				lexer.NextToken();

#line  2862 "VBNET.ATG" 
				dimfound = true; 
			}
		}

#line  2865 "VBNET.ATG" 
		if(dimfound && (m.Modifier & Modifiers.Const) != 0) {
		Error("Dim is not allowed on constants.");
		}
		
		if(m.isNone && dimfound == false) {
			Error("Const, Dim or Static expected");
		}
		
		localVariableDeclaration = new LocalVariableDeclaration(m.Modifier);
		localVariableDeclaration.StartLocation = t.Location;
		
		VariableDeclarator(
#line  2876 "VBNET.ATG" 
localVariableDeclaration.Variables);
		while (la.kind == 23) {
			lexer.NextToken();
			VariableDeclarator(
#line  2877 "VBNET.ATG" 
localVariableDeclaration.Variables);
		}

#line  2879 "VBNET.ATG" 
		statement = localVariableDeclaration;
		
	}

	void TryStatement(
#line  3373 "VBNET.ATG" 
out Statement tryStatement) {

#line  3375 "VBNET.ATG" 
		Statement blockStmt = null, finallyStmt = null;List<CatchClause> catchClauses = null;
		
		Expect(216);
		EndOfStmt();
		Block(
#line  3378 "VBNET.ATG" 
out blockStmt);
		if (la.kind == 74 || la.kind == 112 || la.kind == 122) {
			CatchClauses(
#line  3379 "VBNET.ATG" 
out catchClauses);
		}
		if (la.kind == 122) {
			lexer.NextToken();
			EndOfStmt();
			Block(
#line  3380 "VBNET.ATG" 
out finallyStmt);
		}
		Expect(112);
		Expect(216);

#line  3383 "VBNET.ATG" 
		tryStatement = new TryCatchStatement(blockStmt, catchClauses, finallyStmt);
		
	}

	void WithStatement(
#line  3353 "VBNET.ATG" 
out Statement withStatement) {

#line  3355 "VBNET.ATG" 
		Statement blockStmt = null;
		Expression expr = null;
		
		Expect(231);

#line  3358 "VBNET.ATG" 
		Location start = t.Location; 
		Expr(
#line  3359 "VBNET.ATG" 
out expr);
		EndOfStmt();

#line  3361 "VBNET.ATG" 
		withStatement = new WithStatement(expr);
		withStatement.StartLocation = start;
		
		Block(
#line  3364 "VBNET.ATG" 
out blockStmt);

#line  3366 "VBNET.ATG" 
		((WithStatement)withStatement).Body = (BlockStatement)blockStmt;
		
		Expect(112);
		Expect(231);

#line  3369 "VBNET.ATG" 
		withStatement.EndLocation = t.Location; 
	}

	void WhileOrUntil(
#line  3346 "VBNET.ATG" 
out ConditionType conditionType) {

#line  3347 "VBNET.ATG" 
		conditionType = ConditionType.None; 
		if (la.kind == 229) {
			lexer.NextToken();

#line  3348 "VBNET.ATG" 
			conditionType = ConditionType.While; 
		} else if (la.kind == 222) {
			lexer.NextToken();

#line  3349 "VBNET.ATG" 
			conditionType = ConditionType.Until; 
		} else SynErr(300);
	}

	void LoopControlVariable(
#line  3189 "VBNET.ATG" 
out TypeReference type, out string name) {

#line  3190 "VBNET.ATG" 
		ArrayList arrayModifiers = null;
		type = null;
		
		Qualident(
#line  3194 "VBNET.ATG" 
out name);
		if (
#line  3195 "VBNET.ATG" 
IsDims()) {
			ArrayTypeModifiers(
#line  3195 "VBNET.ATG" 
out arrayModifiers);
		}
		if (la.kind == 62) {
			lexer.NextToken();
			TypeName(
#line  3196 "VBNET.ATG" 
out type);

#line  3196 "VBNET.ATG" 
			if (name.IndexOf('.') > 0) { Error("No type def for 'for each' member indexer allowed."); } 
		}

#line  3198 "VBNET.ATG" 
		if (type != null) {
		if(type.RankSpecifier != null && arrayModifiers != null) {
			Error("array rank only allowed one time");
		} else if (arrayModifiers != null) {
			type.RankSpecifier = (int[])arrayModifiers.ToArray(typeof(int));
		}
		}
		
	}

	void ReDimClause(
#line  3268 "VBNET.ATG" 
out Expression expr) {
		SimpleNonInvocationExpression(
#line  3270 "VBNET.ATG" 
out expr);
		ReDimClauseInternal(
#line  3271 "VBNET.ATG" 
ref expr);
	}

	void SingleLineStatementList(
#line  3175 "VBNET.ATG" 
List<Statement> list) {

#line  3176 "VBNET.ATG" 
		Statement embeddedStatement = null; 
		if (la.kind == 112) {
			lexer.NextToken();

#line  3178 "VBNET.ATG" 
			embeddedStatement = new EndStatement(); 
		} else if (StartOf(42)) {
			EmbeddedStatement(
#line  3179 "VBNET.ATG" 
out embeddedStatement);
		} else SynErr(301);

#line  3180 "VBNET.ATG" 
		if (embeddedStatement != null) list.Add(embeddedStatement); 
		while (la.kind == 22) {
			lexer.NextToken();
			while (la.kind == 22) {
				lexer.NextToken();
			}
			if (la.kind == 112) {
				lexer.NextToken();

#line  3182 "VBNET.ATG" 
				embeddedStatement = new EndStatement(); 
			} else if (StartOf(42)) {
				EmbeddedStatement(
#line  3183 "VBNET.ATG" 
out embeddedStatement);
			} else SynErr(302);

#line  3184 "VBNET.ATG" 
			if (embeddedStatement != null) list.Add(embeddedStatement); 
		}
	}

	void CaseClauses(
#line  3306 "VBNET.ATG" 
out List<CaseLabel> caseClauses) {

#line  3308 "VBNET.ATG" 
		caseClauses = new List<CaseLabel>();
		CaseLabel caseClause = null;
		
		CaseClause(
#line  3311 "VBNET.ATG" 
out caseClause);

#line  3311 "VBNET.ATG" 
		if (caseClause != null) { caseClauses.Add(caseClause); } 
		while (la.kind == 23) {
			lexer.NextToken();
			CaseClause(
#line  3312 "VBNET.ATG" 
out caseClause);

#line  3312 "VBNET.ATG" 
			if (caseClause != null) { caseClauses.Add(caseClause); } 
		}
	}

	void OnErrorStatement(
#line  3209 "VBNET.ATG" 
out OnErrorStatement stmt) {

#line  3211 "VBNET.ATG" 
		stmt = null;
		GotoStatement goToStatement = null;
		
		Expect(170);
		Expect(117);
		if (
#line  3217 "VBNET.ATG" 
IsNegativeLabelName()) {
			Expect(131);
			Expect(29);
			Expect(5);

#line  3219 "VBNET.ATG" 
			long intLabel = Int64.Parse(t.val);
			if(intLabel != 1) {
				Error("invalid label in on error statement.");
			}
			stmt = new OnErrorStatement(new GotoStatement((intLabel * -1).ToString()));
			
		} else if (la.kind == 131) {
			GotoStatement(
#line  3225 "VBNET.ATG" 
out goToStatement);

#line  3227 "VBNET.ATG" 
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
			
		} else if (la.kind == 192) {
			lexer.NextToken();
			Expect(162);

#line  3241 "VBNET.ATG" 
			stmt = new OnErrorStatement(new ResumeStatement(true));
			
		} else SynErr(303);
	}

	void GotoStatement(
#line  3247 "VBNET.ATG" 
out GotoStatement goToStatement) {

#line  3249 "VBNET.ATG" 
		string label = String.Empty;
		
		Expect(131);
		LabelName(
#line  3252 "VBNET.ATG" 
out label);

#line  3254 "VBNET.ATG" 
		goToStatement = new GotoStatement(label);
		
	}

	void ResumeStatement(
#line  3295 "VBNET.ATG" 
out ResumeStatement resumeStatement) {

#line  3297 "VBNET.ATG" 
		resumeStatement = null;
		string label = String.Empty;
		
		if (
#line  3300 "VBNET.ATG" 
IsResumeNext()) {
			Expect(192);
			Expect(162);

#line  3301 "VBNET.ATG" 
			resumeStatement = new ResumeStatement(true); 
		} else if (la.kind == 192) {
			lexer.NextToken();
			if (StartOf(43)) {
				LabelName(
#line  3302 "VBNET.ATG" 
out label);
			}

#line  3302 "VBNET.ATG" 
			resumeStatement = new ResumeStatement(label); 
		} else SynErr(304);
	}

	void ReDimClauseInternal(
#line  3274 "VBNET.ATG" 
ref Expression expr) {

#line  3275 "VBNET.ATG" 
		List<Expression> arguments; bool canBeNormal; bool canBeRedim; string name; 
		while (la.kind == 27 || 
#line  3278 "VBNET.ATG" 
la.kind == Tokens.OpenParenthesis && Peek(1).kind == Tokens.Of) {
			if (la.kind == 27) {
				lexer.NextToken();
				IdentifierOrKeyword(
#line  3277 "VBNET.ATG" 
out name);

#line  3277 "VBNET.ATG" 
				expr = new MemberReferenceExpression(expr, name); 
			} else {
				InvocationExpression(
#line  3279 "VBNET.ATG" 
ref expr);
			}
		}
		Expect(36);
		NormalOrReDimArgumentList(
#line  3282 "VBNET.ATG" 
out arguments, out canBeNormal, out canBeRedim);
		Expect(37);

#line  3284 "VBNET.ATG" 
		expr = new InvocationExpression(expr, arguments);
		if (canBeRedim == false || canBeNormal && (la.kind == Tokens.Dot || la.kind == Tokens.OpenParenthesis)) {
			if (this.Errors.Count == 0) {
				// don't recurse on parse errors - could result in endless recursion
				ReDimClauseInternal(ref expr);
			}
		}
		
	}

	void CaseClause(
#line  3316 "VBNET.ATG" 
out CaseLabel caseClause) {

#line  3318 "VBNET.ATG" 
		Expression expr = null;
		Expression sexpr = null;
		BinaryOperatorType op = BinaryOperatorType.None;
		caseClause = null;
		
		if (la.kind == 110) {
			lexer.NextToken();

#line  3324 "VBNET.ATG" 
			caseClause = new CaseLabel(); 
		} else if (StartOf(44)) {
			if (la.kind == 143) {
				lexer.NextToken();
			}
			switch (la.kind) {
			case 39: {
				lexer.NextToken();

#line  3328 "VBNET.ATG" 
				op = BinaryOperatorType.LessThan; 
				break;
			}
			case 38: {
				lexer.NextToken();

#line  3329 "VBNET.ATG" 
				op = BinaryOperatorType.GreaterThan; 
				break;
			}
			case 42: {
				lexer.NextToken();

#line  3330 "VBNET.ATG" 
				op = BinaryOperatorType.LessThanOrEqual; 
				break;
			}
			case 41: {
				lexer.NextToken();

#line  3331 "VBNET.ATG" 
				op = BinaryOperatorType.GreaterThanOrEqual; 
				break;
			}
			case 21: {
				lexer.NextToken();

#line  3332 "VBNET.ATG" 
				op = BinaryOperatorType.Equality; 
				break;
			}
			case 40: {
				lexer.NextToken();

#line  3333 "VBNET.ATG" 
				op = BinaryOperatorType.InEquality; 
				break;
			}
			default: SynErr(305); break;
			}
			Expr(
#line  3335 "VBNET.ATG" 
out expr);

#line  3337 "VBNET.ATG" 
			caseClause = new CaseLabel(op, expr);
			
		} else if (StartOf(29)) {
			Expr(
#line  3339 "VBNET.ATG" 
out expr);
			if (la.kind == 214) {
				lexer.NextToken();
				Expr(
#line  3339 "VBNET.ATG" 
out sexpr);
			}

#line  3341 "VBNET.ATG" 
			caseClause = new CaseLabel(expr, sexpr);
			
		} else SynErr(306);
	}

	void CatchClauses(
#line  3388 "VBNET.ATG" 
out List<CatchClause> catchClauses) {

#line  3390 "VBNET.ATG" 
		catchClauses = new List<CatchClause>();
		TypeReference type = null;
		Statement blockStmt = null;
		Expression expr = null;
		string name = String.Empty;
		
		while (la.kind == 74) {
			lexer.NextToken();
			if (StartOf(4)) {
				Identifier();

#line  3398 "VBNET.ATG" 
				name = t.val; 
				if (la.kind == 62) {
					lexer.NextToken();
					TypeName(
#line  3398 "VBNET.ATG" 
out type);
				}
			}
			if (la.kind == 227) {
				lexer.NextToken();
				Expr(
#line  3399 "VBNET.ATG" 
out expr);
			}
			EndOfStmt();
			Block(
#line  3401 "VBNET.ATG" 
out blockStmt);

#line  3402 "VBNET.ATG" 
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
			case 19: s = "XmlProcessingInstructionStart expected"; break;
			case 20: s = "XmlProcessingInstructionEnd expected"; break;
			case 21: s = "\"=\" expected"; break;
			case 22: s = "\":\" expected"; break;
			case 23: s = "\",\" expected"; break;
			case 24: s = "\"&\" expected"; break;
			case 25: s = "\"/\" expected"; break;
			case 26: s = "\"\\\\\" expected"; break;
			case 27: s = "\".\" expected"; break;
			case 28: s = "\"!\" expected"; break;
			case 29: s = "\"-\" expected"; break;
			case 30: s = "\"+\" expected"; break;
			case 31: s = "\"^\" expected"; break;
			case 32: s = "\"?\" expected"; break;
			case 33: s = "\"*\" expected"; break;
			case 34: s = "\"{\" expected"; break;
			case 35: s = "\"}\" expected"; break;
			case 36: s = "\"(\" expected"; break;
			case 37: s = "\")\" expected"; break;
			case 38: s = "\">\" expected"; break;
			case 39: s = "\"<\" expected"; break;
			case 40: s = "\"<>\" expected"; break;
			case 41: s = "\">=\" expected"; break;
			case 42: s = "\"<=\" expected"; break;
			case 43: s = "\"<<\" expected"; break;
			case 44: s = "\">>\" expected"; break;
			case 45: s = "\"+=\" expected"; break;
			case 46: s = "\"^=\" expected"; break;
			case 47: s = "\"-=\" expected"; break;
			case 48: s = "\"*=\" expected"; break;
			case 49: s = "\"/=\" expected"; break;
			case 50: s = "\"\\\\=\" expected"; break;
			case 51: s = "\"<<=\" expected"; break;
			case 52: s = "\">>=\" expected"; break;
			case 53: s = "\"&=\" expected"; break;
			case 54: s = "\":=\" expected"; break;
			case 55: s = "\"AddHandler\" expected"; break;
			case 56: s = "\"AddressOf\" expected"; break;
			case 57: s = "\"Aggregate\" expected"; break;
			case 58: s = "\"Alias\" expected"; break;
			case 59: s = "\"And\" expected"; break;
			case 60: s = "\"AndAlso\" expected"; break;
			case 61: s = "\"Ansi\" expected"; break;
			case 62: s = "\"As\" expected"; break;
			case 63: s = "\"Ascending\" expected"; break;
			case 64: s = "\"Assembly\" expected"; break;
			case 65: s = "\"Auto\" expected"; break;
			case 66: s = "\"Binary\" expected"; break;
			case 67: s = "\"Boolean\" expected"; break;
			case 68: s = "\"ByRef\" expected"; break;
			case 69: s = "\"By\" expected"; break;
			case 70: s = "\"Byte\" expected"; break;
			case 71: s = "\"ByVal\" expected"; break;
			case 72: s = "\"Call\" expected"; break;
			case 73: s = "\"Case\" expected"; break;
			case 74: s = "\"Catch\" expected"; break;
			case 75: s = "\"CBool\" expected"; break;
			case 76: s = "\"CByte\" expected"; break;
			case 77: s = "\"CChar\" expected"; break;
			case 78: s = "\"CDate\" expected"; break;
			case 79: s = "\"CDbl\" expected"; break;
			case 80: s = "\"CDec\" expected"; break;
			case 81: s = "\"Char\" expected"; break;
			case 82: s = "\"CInt\" expected"; break;
			case 83: s = "\"Class\" expected"; break;
			case 84: s = "\"CLng\" expected"; break;
			case 85: s = "\"CObj\" expected"; break;
			case 86: s = "\"Compare\" expected"; break;
			case 87: s = "\"Const\" expected"; break;
			case 88: s = "\"Continue\" expected"; break;
			case 89: s = "\"CSByte\" expected"; break;
			case 90: s = "\"CShort\" expected"; break;
			case 91: s = "\"CSng\" expected"; break;
			case 92: s = "\"CStr\" expected"; break;
			case 93: s = "\"CType\" expected"; break;
			case 94: s = "\"CUInt\" expected"; break;
			case 95: s = "\"CULng\" expected"; break;
			case 96: s = "\"CUShort\" expected"; break;
			case 97: s = "\"Custom\" expected"; break;
			case 98: s = "\"Date\" expected"; break;
			case 99: s = "\"Decimal\" expected"; break;
			case 100: s = "\"Declare\" expected"; break;
			case 101: s = "\"Default\" expected"; break;
			case 102: s = "\"Delegate\" expected"; break;
			case 103: s = "\"Descending\" expected"; break;
			case 104: s = "\"Dim\" expected"; break;
			case 105: s = "\"DirectCast\" expected"; break;
			case 106: s = "\"Distinct\" expected"; break;
			case 107: s = "\"Do\" expected"; break;
			case 108: s = "\"Double\" expected"; break;
			case 109: s = "\"Each\" expected"; break;
			case 110: s = "\"Else\" expected"; break;
			case 111: s = "\"ElseIf\" expected"; break;
			case 112: s = "\"End\" expected"; break;
			case 113: s = "\"EndIf\" expected"; break;
			case 114: s = "\"Enum\" expected"; break;
			case 115: s = "\"Equals\" expected"; break;
			case 116: s = "\"Erase\" expected"; break;
			case 117: s = "\"Error\" expected"; break;
			case 118: s = "\"Event\" expected"; break;
			case 119: s = "\"Exit\" expected"; break;
			case 120: s = "\"Explicit\" expected"; break;
			case 121: s = "\"False\" expected"; break;
			case 122: s = "\"Finally\" expected"; break;
			case 123: s = "\"For\" expected"; break;
			case 124: s = "\"Friend\" expected"; break;
			case 125: s = "\"From\" expected"; break;
			case 126: s = "\"Function\" expected"; break;
			case 127: s = "\"Get\" expected"; break;
			case 128: s = "\"GetType\" expected"; break;
			case 129: s = "\"Global\" expected"; break;
			case 130: s = "\"GoSub\" expected"; break;
			case 131: s = "\"GoTo\" expected"; break;
			case 132: s = "\"Group\" expected"; break;
			case 133: s = "\"Handles\" expected"; break;
			case 134: s = "\"If\" expected"; break;
			case 135: s = "\"Implements\" expected"; break;
			case 136: s = "\"Imports\" expected"; break;
			case 137: s = "\"In\" expected"; break;
			case 138: s = "\"Infer\" expected"; break;
			case 139: s = "\"Inherits\" expected"; break;
			case 140: s = "\"Integer\" expected"; break;
			case 141: s = "\"Interface\" expected"; break;
			case 142: s = "\"Into\" expected"; break;
			case 143: s = "\"Is\" expected"; break;
			case 144: s = "\"IsNot\" expected"; break;
			case 145: s = "\"Join\" expected"; break;
			case 146: s = "\"Key\" expected"; break;
			case 147: s = "\"Let\" expected"; break;
			case 148: s = "\"Lib\" expected"; break;
			case 149: s = "\"Like\" expected"; break;
			case 150: s = "\"Long\" expected"; break;
			case 151: s = "\"Loop\" expected"; break;
			case 152: s = "\"Me\" expected"; break;
			case 153: s = "\"Mod\" expected"; break;
			case 154: s = "\"Module\" expected"; break;
			case 155: s = "\"MustInherit\" expected"; break;
			case 156: s = "\"MustOverride\" expected"; break;
			case 157: s = "\"MyBase\" expected"; break;
			case 158: s = "\"MyClass\" expected"; break;
			case 159: s = "\"Namespace\" expected"; break;
			case 160: s = "\"Narrowing\" expected"; break;
			case 161: s = "\"New\" expected"; break;
			case 162: s = "\"Next\" expected"; break;
			case 163: s = "\"Not\" expected"; break;
			case 164: s = "\"Nothing\" expected"; break;
			case 165: s = "\"NotInheritable\" expected"; break;
			case 166: s = "\"NotOverridable\" expected"; break;
			case 167: s = "\"Object\" expected"; break;
			case 168: s = "\"Of\" expected"; break;
			case 169: s = "\"Off\" expected"; break;
			case 170: s = "\"On\" expected"; break;
			case 171: s = "\"Operator\" expected"; break;
			case 172: s = "\"Option\" expected"; break;
			case 173: s = "\"Optional\" expected"; break;
			case 174: s = "\"Or\" expected"; break;
			case 175: s = "\"Order\" expected"; break;
			case 176: s = "\"OrElse\" expected"; break;
			case 177: s = "\"Overloads\" expected"; break;
			case 178: s = "\"Overridable\" expected"; break;
			case 179: s = "\"Overrides\" expected"; break;
			case 180: s = "\"ParamArray\" expected"; break;
			case 181: s = "\"Partial\" expected"; break;
			case 182: s = "\"Preserve\" expected"; break;
			case 183: s = "\"Private\" expected"; break;
			case 184: s = "\"Property\" expected"; break;
			case 185: s = "\"Protected\" expected"; break;
			case 186: s = "\"Public\" expected"; break;
			case 187: s = "\"RaiseEvent\" expected"; break;
			case 188: s = "\"ReadOnly\" expected"; break;
			case 189: s = "\"ReDim\" expected"; break;
			case 190: s = "\"Rem\" expected"; break;
			case 191: s = "\"RemoveHandler\" expected"; break;
			case 192: s = "\"Resume\" expected"; break;
			case 193: s = "\"Return\" expected"; break;
			case 194: s = "\"SByte\" expected"; break;
			case 195: s = "\"Select\" expected"; break;
			case 196: s = "\"Set\" expected"; break;
			case 197: s = "\"Shadows\" expected"; break;
			case 198: s = "\"Shared\" expected"; break;
			case 199: s = "\"Short\" expected"; break;
			case 200: s = "\"Single\" expected"; break;
			case 201: s = "\"Skip\" expected"; break;
			case 202: s = "\"Static\" expected"; break;
			case 203: s = "\"Step\" expected"; break;
			case 204: s = "\"Stop\" expected"; break;
			case 205: s = "\"Strict\" expected"; break;
			case 206: s = "\"String\" expected"; break;
			case 207: s = "\"Structure\" expected"; break;
			case 208: s = "\"Sub\" expected"; break;
			case 209: s = "\"SyncLock\" expected"; break;
			case 210: s = "\"Take\" expected"; break;
			case 211: s = "\"Text\" expected"; break;
			case 212: s = "\"Then\" expected"; break;
			case 213: s = "\"Throw\" expected"; break;
			case 214: s = "\"To\" expected"; break;
			case 215: s = "\"True\" expected"; break;
			case 216: s = "\"Try\" expected"; break;
			case 217: s = "\"TryCast\" expected"; break;
			case 218: s = "\"TypeOf\" expected"; break;
			case 219: s = "\"UInteger\" expected"; break;
			case 220: s = "\"ULong\" expected"; break;
			case 221: s = "\"Unicode\" expected"; break;
			case 222: s = "\"Until\" expected"; break;
			case 223: s = "\"UShort\" expected"; break;
			case 224: s = "\"Using\" expected"; break;
			case 225: s = "\"Variant\" expected"; break;
			case 226: s = "\"Wend\" expected"; break;
			case 227: s = "\"When\" expected"; break;
			case 228: s = "\"Where\" expected"; break;
			case 229: s = "\"While\" expected"; break;
			case 230: s = "\"Widening\" expected"; break;
			case 231: s = "\"With\" expected"; break;
			case 232: s = "\"WithEvents\" expected"; break;
			case 233: s = "\"WriteOnly\" expected"; break;
			case 234: s = "\"Xor\" expected"; break;
			case 235: s = "\"GetXmlNamespace\" expected"; break;
			case 236: s = "??? expected"; break;
			case 237: s = "invalid EndOfStmt"; break;
			case 238: s = "invalid OptionStmt"; break;
			case 239: s = "invalid OptionStmt"; break;
			case 240: s = "invalid GlobalAttributeSection"; break;
			case 241: s = "invalid GlobalAttributeSection"; break;
			case 242: s = "invalid NamespaceMemberDecl"; break;
			case 243: s = "invalid OptionValue"; break;
			case 244: s = "invalid ImportClause"; break;
			case 245: s = "invalid Identifier"; break;
			case 246: s = "invalid TypeModifier"; break;
			case 247: s = "invalid NonModuleDeclaration"; break;
			case 248: s = "invalid NonModuleDeclaration"; break;
			case 249: s = "invalid TypeParameterConstraints"; break;
			case 250: s = "invalid TypeParameterConstraint"; break;
			case 251: s = "invalid NonArrayTypeName"; break;
			case 252: s = "invalid MemberModifier"; break;
			case 253: s = "invalid StructureMemberDecl"; break;
			case 254: s = "invalid StructureMemberDecl"; break;
			case 255: s = "invalid StructureMemberDecl"; break;
			case 256: s = "invalid StructureMemberDecl"; break;
			case 257: s = "invalid StructureMemberDecl"; break;
			case 258: s = "invalid StructureMemberDecl"; break;
			case 259: s = "invalid StructureMemberDecl"; break;
			case 260: s = "invalid StructureMemberDecl"; break;
			case 261: s = "invalid InterfaceMemberDecl"; break;
			case 262: s = "invalid InterfaceMemberDecl"; break;
			case 263: s = "invalid Expr"; break;
			case 264: s = "invalid Charset"; break;
			case 265: s = "invalid IdentifierForFieldDeclaration"; break;
			case 266: s = "invalid VariableDeclaratorPartAfterIdentifier"; break;
			case 267: s = "invalid AccessorDecls"; break;
			case 268: s = "invalid EventAccessorDeclaration"; break;
			case 269: s = "invalid OverloadableOperator"; break;
			case 270: s = "invalid EventMemberSpecifier"; break;
			case 271: s = "invalid LambdaExpr"; break;
			case 272: s = "invalid AssignmentOperator"; break;
			case 273: s = "invalid SimpleNonInvocationExpression"; break;
			case 274: s = "invalid SimpleNonInvocationExpression"; break;
			case 275: s = "invalid SimpleNonInvocationExpression"; break;
			case 276: s = "invalid SimpleNonInvocationExpression"; break;
			case 277: s = "invalid PrimitiveTypeName"; break;
			case 278: s = "invalid CastTarget"; break;
			case 279: s = "invalid ComparisonExpr"; break;
			case 280: s = "invalid SubLambdaExpression"; break;
			case 281: s = "invalid FunctionLambdaExpression"; break;
			case 282: s = "invalid EmbeddedStatement"; break;
			case 283: s = "invalid EmbeddedStatement"; break;
			case 284: s = "invalid EmbeddedStatement"; break;
			case 285: s = "invalid EmbeddedStatement"; break;
			case 286: s = "invalid EmbeddedStatement"; break;
			case 287: s = "invalid EmbeddedStatement"; break;
			case 288: s = "invalid EmbeddedStatement"; break;
			case 289: s = "invalid FromOrAggregateQueryOperator"; break;
			case 290: s = "invalid QueryOperator"; break;
			case 291: s = "invalid PartitionQueryOperator"; break;
			case 292: s = "invalid Argument"; break;
			case 293: s = "invalid QualIdentAndTypeArguments"; break;
			case 294: s = "invalid AttributeArguments"; break;
			case 295: s = "invalid AttributeArguments"; break;
			case 296: s = "invalid AttributeArguments"; break;
			case 297: s = "invalid ParameterModifier"; break;
			case 298: s = "invalid Statement"; break;
			case 299: s = "invalid LabelName"; break;
			case 300: s = "invalid WhileOrUntil"; break;
			case 301: s = "invalid SingleLineStatementList"; break;
			case 302: s = "invalid SingleLineStatementList"; break;
			case 303: s = "invalid OnErrorStatement"; break;
			case 304: s = "invalid ResumeStatement"; break;
			case 305: s = "invalid CaseClause"; break;
			case 306: s = "invalid CaseClause"; break;

			default: s = "error " + errorNumber; break;
		}
		this.Errors.Error(line, col, s);
	}
	
	private bool StartOf(int s)
	{
		return set[s, lexer.LookAhead.kind];
	}
	
	static bool[,] set = {
	{T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,T, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, x,T,T,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, x,T,T,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, x,T,T,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, T,T,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, x,x,x,x, T,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,T, x,T,T,x, x,x,x,x, x,x,x,x, x,T,x,T, x,x,T,x, x,x,x,x, x,x,x,x, x,T,T,T, x,x,x,T, x,x,T,x, T,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,T,x,x, T,x,x,x, x,x,T,x, T,x,T,x, x,T,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, T,T,x,x, x,x,T,T, x,x,T,T, x,x,x,x, x,x,x,T, T,T,T,T, x,x,x,x, T,x,x,x, x,x,x,x, x,x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,T, x,T,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,T,T, x,x,x,T, x,x,T,x, T,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,T,x,x, T,x,x,x, x,x,T,x, T,x,T,x, x,T,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, T,T,x,x, x,x,T,x, x,x,T,T, x,x,x,x, x,x,x,T, T,T,T,T, x,x,x,x, T,x,x,x, x,x,x,x, x,x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,T, x,x,x,x, x,x,x,x, x,T,x,x, T,T,T,T, T,x,T,x, x,x,x,x, x,x,T,T, x,x,T,x, T,x,x,x, T,T,T,x, x,x,x,x, T,x,x,x, x,x,T,x, x,T,T,x, x,T,x,x, x,x,x,x, x,x,T,T, T,x,x,x, T,x,x,x, x,T,T,x, x,T,x,T, x,x,x,T, x,T,T,T, x,T,T,T, T,T,T,x, T,x,x,x, x,x,x,x, x,T,T,x, x,T,x,x, x,x,x,T, T,x,T,T, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, T,x,T,x, T,T,x,x, x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, x,T,T,T, x,T,x,T, x,T,T,x, T,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,T,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, T,x,x,x, x,x,x,x, x,x,T,x, x,x,T,x, x,x,x,x, T,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, x,T,T,T, x,T,x,T, T,T,T,x, T,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, T,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, x,T,T,T, x,T,x,T, T,T,T,x, T,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
	{x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,x, x,T,T,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,T,x,T, T,T,T,T, x,T,T,x, x,x,x,T, T,T,T,T, T,T,T,x, T,T,T,x, x,T,T,T, T,T,T,T, T,T,T,T, x,x,x,T, x,T,T,x, T,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,x,x, T,T,x,x, T,x,T,x, x,x,T,x, T,x,T,x, x,T,x,x, x,x,T,x, T,x,x,x, x,T,T,x, x,T,x,T, T,x,x,T, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, T,T,x,x, x,x,T,x, x,x,T,T, x,x,x,T, x,T,T,T, T,T,T,T, x,x,x,x, T,x,x,x, x,x,x,x, x,x},
	{x,T,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,x,x, x,T,x,T, T,T,T,T, x,T,T,x, T,x,x,T, T,T,T,T, T,T,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, x,x,x,T, T,T,T,T, T,x,x,x, x,x,x,T, T,T,x,T, T,T,x,T, x,T,x,x, T,T,x,T, T,x,T,x, x,x,T,x, T,x,T,x, x,T,x,x, x,x,T,x, T,x,x,x, x,T,T,x, x,T,x,x, T,x,x,T, x,T,T,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,T,x,T, T,T,T,T, x,x,x,T, T,T,T,x, T,x,T,x, x,T,T,T, x,T,x,T, T,T,T,T, T,T,T,T, T,x,x,x, T,T,x,T, x,x,x,x, x,x},
	{x,T,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,T,T,T, T,T,T,T, T,T,T,T, x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,T, T,T,x,T, T,T,T,T, T,T,T,T, x,T,T,x, T,x,x,T, T,T,T,T, T,T,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, x,x,x,T, T,T,T,T, T,x,T,x, T,x,x,T, T,T,x,T, T,T,x,T, x,T,x,x, T,T,x,T, T,x,T,T, x,x,T,x, T,x,T,T, T,T,x,T, x,T,T,x, T,T,x,x, x,T,T,x, x,T,x,x, T,x,x,T, x,T,T,x, x,x,T,T, T,x,x,x, x,x,T,x, x,x,x,T, x,T,x,T, T,T,T,T, x,x,x,T, T,T,T,T, T,x,T,x, x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x,x, T,T,x,T, x,x,T,x, x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
	{x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,x, x,T,T,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,T,x,T, T,T,T,T, x,T,T,x, x,x,x,T, T,T,T,T, T,T,T,x, T,T,T,x, x,T,T,T, T,T,T,T, T,T,T,T, x,x,x,T, x,T,T,x, T,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,T,x, T,T,x,x, T,x,T,x, x,x,T,x, T,x,T,x, x,T,x,x, x,x,T,x, T,x,x,x, x,T,T,x, x,T,x,T, T,x,x,T, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, T,T,x,x, x,x,T,x, T,x,T,T, x,x,x,T, x,T,T,T, T,T,T,T, x,x,x,x, T,x,x,x, x,x,x,x, x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x},
	{x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,T,x,T, T,T,T,T, x,T,T,x, x,x,x,T, T,T,T,T, T,T,T,x, T,T,T,x, x,T,T,T, T,T,T,T, T,T,T,T, x,x,x,T, x,T,T,x, T,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,x,x, T,T,x,x, T,x,T,x, x,x,T,x, T,x,T,x, x,T,x,x, x,x,T,x, T,x,x,x, x,T,T,x, x,T,x,x, T,x,x,T, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, T,T,x,x, x,x,T,x, x,x,T,T, x,x,x,T, x,T,T,T, T,T,T,T, x,x,x,x, T,x,x,x, x,x,x,x, x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
	{x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,x, x,T,T,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,T,x,T, T,T,T,T, x,T,T,x, x,x,x,T, T,T,T,T, T,T,T,x, T,T,T,x, x,T,T,T, T,T,T,T, T,T,T,T, x,x,x,T, x,T,T,x, T,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,x,x, T,T,x,x, T,x,T,x, x,x,T,x, T,x,T,x, x,T,x,x, x,x,T,x, T,x,x,x, x,T,T,x, x,T,x,x, T,x,x,T, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, T,T,x,x, x,x,T,x, x,x,T,T, x,x,x,T, x,T,T,T, T,T,T,T, x,x,x,x, T,x,x,x, x,x,x,x, x,x},
	{x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,x, x,T,T,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,x,x, x,T,x,T, T,T,T,T, x,T,T,x, T,x,x,T, T,T,T,T, T,T,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, x,x,x,T, T,T,T,T, T,x,x,x, x,x,x,T, T,T,x,T, T,T,x,T, x,T,T,x, T,T,x,T, T,x,T,x, x,x,T,x, T,x,T,x, x,T,x,x, x,x,T,x, T,x,x,x, x,T,T,x, x,T,x,T, T,x,x,T, x,T,T,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,T,x,T, T,T,T,T, x,x,x,T, T,T,T,x, T,x,T,x, T,T,T,T, x,T,x,T, T,T,T,T, T,T,T,T, T,x,x,x, T,T,x,T, x,x,x,x, x,x},
	{x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,T,T,x, x,T,T,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,T,x,T, T,T,T,T, x,T,T,x, x,x,x,T, T,T,T,T, T,T,T,x, T,T,T,x, x,T,T,T, T,T,T,T, T,T,T,T, x,x,x,T, x,T,T,x, T,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,T,x, T,T,x,x, T,x,T,x, x,x,T,x, T,x,T,x, x,T,x,x, x,x,T,x, T,x,x,x, x,T,T,x, x,T,x,T, T,x,x,T, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, T,T,x,x, x,x,T,x, T,x,T,T, x,x,x,T, x,T,T,T, T,T,T,T, x,x,x,x, T,x,x,x, x,x,x,x, x,x},
	{x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,T,x,T, T,T,T,T, x,T,T,x, x,x,x,T, T,T,T,T, T,T,T,x, T,T,T,x, x,T,T,T, T,T,T,T, T,T,T,T, x,x,x,T, x,T,T,x, T,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,x,x, T,T,x,x, T,x,T,x, x,x,T,x, T,x,T,x, x,T,x,x, x,x,T,x, T,x,x,x, x,T,T,x, x,T,x,x, T,x,x,T, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, T,T,x,x, x,x,T,x, x,x,T,T, x,x,x,T, x,T,T,T, T,T,T,T, x,x,x,x, T,x,x,x, x,x,x,x, x,x},
	{x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,x,x, x,T,x,T, T,T,T,T, x,T,T,x, T,x,x,T, T,T,T,T, T,T,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, x,x,x,T, T,T,T,T, T,x,x,x, T,x,x,T, T,T,x,T, T,T,x,T, x,T,x,x, T,T,x,T, T,x,T,x, x,x,T,x, T,x,T,x, x,T,x,x, x,x,T,x, T,x,x,x, x,T,T,x, x,T,x,x, T,x,x,T, x,T,T,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,T,x,T, T,T,T,T, x,x,x,T, T,T,T,x, T,x,T,x, x,T,T,T, x,T,x,T, T,T,T,T, T,T,T,T, T,x,x,x, T,T,x,T, x,x,x,x, x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
	{x,T,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,T,T,T, T,T,T,T, T,T,T,T, x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,T, T,T,x,T, T,T,x,T, T,T,T,T, x,T,T,x, T,x,x,T, T,T,T,T, T,T,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, x,x,x,T, T,T,T,T, T,x,T,x, T,x,x,T, T,T,x,T, T,T,x,T, x,T,x,x, T,T,x,T, T,x,T,T, x,x,T,x, T,x,T,T, T,T,x,T, x,T,T,x, T,T,x,x, x,T,T,x, x,T,x,x, T,x,x,T, x,T,T,x, x,x,T,T, T,x,x,x, x,x,T,x, x,x,x,T, x,T,x,T, T,T,T,T, x,x,x,T, T,T,T,T, T,x,T,x, x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x,x, T,T,x,T, x,x,T,x, x,x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, T,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
	{x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,x,x, x,T,x,T, T,T,T,T, x,T,T,x, T,x,x,T, T,T,T,T, T,T,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, x,x,x,T, T,T,T,T, T,x,x,x, x,x,x,T, T,T,x,T, T,T,x,T, x,T,x,x, T,T,x,T, T,x,T,x, x,x,T,x, T,x,T,x, x,T,x,x, x,x,T,x, T,x,x,x, x,T,T,x, x,T,x,x, T,x,x,T, x,T,T,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,T,x,T, T,T,T,T, x,x,x,T, T,T,T,x, T,x,T,x, x,T,T,T, x,T,x,T, T,T,T,T, T,T,T,T, T,x,x,x, T,T,x,T, x,x,x,x, x,x},
	{x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x}

	};
} // end Parser

}