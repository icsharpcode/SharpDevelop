
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
	const int maxT = 235;

	const  bool   T            = true;
	const  bool   x            = false;
	

#line  12 "VBNET.ATG" 


/*

*/

	void VBNET() {

#line  259 "VBNET.ATG" 
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
#line  267 "VBNET.ATG" 
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
		} else SynErr(236);
	}

	void OptionStmt() {

#line  272 "VBNET.ATG" 
		INode node = null; bool val = true; 
		Expect(172);

#line  273 "VBNET.ATG" 
		Location startPos = t.Location; 
		if (la.kind == 120) {
			lexer.NextToken();
			if (la.kind == 169 || la.kind == 170) {
				OptionValue(
#line  275 "VBNET.ATG" 
ref val);
			}

#line  276 "VBNET.ATG" 
			node = new OptionDeclaration(OptionType.Explicit, val); 
		} else if (la.kind == 205) {
			lexer.NextToken();
			if (la.kind == 169 || la.kind == 170) {
				OptionValue(
#line  278 "VBNET.ATG" 
ref val);
			}

#line  279 "VBNET.ATG" 
			node = new OptionDeclaration(OptionType.Strict, val); 
		} else if (la.kind == 86) {
			lexer.NextToken();
			if (la.kind == 66) {
				lexer.NextToken();

#line  281 "VBNET.ATG" 
				node = new OptionDeclaration(OptionType.CompareBinary, val); 
			} else if (la.kind == 211) {
				lexer.NextToken();

#line  282 "VBNET.ATG" 
				node = new OptionDeclaration(OptionType.CompareText, val); 
			} else SynErr(237);
		} else if (la.kind == 138) {
			lexer.NextToken();
			if (la.kind == 169 || la.kind == 170) {
				OptionValue(
#line  285 "VBNET.ATG" 
ref val);
			}

#line  286 "VBNET.ATG" 
			node = new OptionDeclaration(OptionType.Infer, val); 
		} else SynErr(238);
		EndOfStmt();

#line  290 "VBNET.ATG" 
		if (node != null) {
		node.StartLocation = startPos;
		node.EndLocation   = t.Location;
		AddChild(node);
		}
		
	}

	void ImportsStmt() {

#line  313 "VBNET.ATG" 
		List<Using> usings = new List<Using>();
		
		Expect(136);

#line  317 "VBNET.ATG" 
		Location startPos = t.Location;
		Using u;
		
		ImportClause(
#line  320 "VBNET.ATG" 
out u);

#line  320 "VBNET.ATG" 
		if (u != null) { usings.Add(u); } 
		while (la.kind == 23) {
			lexer.NextToken();
			ImportClause(
#line  322 "VBNET.ATG" 
out u);

#line  322 "VBNET.ATG" 
			if (u != null) { usings.Add(u); } 
		}
		EndOfStmt();

#line  326 "VBNET.ATG" 
		UsingDeclaration usingDeclaration = new UsingDeclaration(usings);
		usingDeclaration.StartLocation = startPos;
		usingDeclaration.EndLocation   = t.Location;
		AddChild(usingDeclaration);
		
	}

	void GlobalAttributeSection() {
		Expect(39);

#line  2650 "VBNET.ATG" 
		Location startPos = t.Location; 
		if (la.kind == 64) {
			lexer.NextToken();
		} else if (la.kind == 154) {
			lexer.NextToken();
		} else SynErr(239);

#line  2652 "VBNET.ATG" 
		string attributeTarget = t.val != null ? t.val.ToLower(System.Globalization.CultureInfo.InvariantCulture) : null;
		List<ASTAttribute> attributes = new List<ASTAttribute>();
		ASTAttribute attribute;
		
		Expect(22);
		Attribute(
#line  2656 "VBNET.ATG" 
out attribute);

#line  2656 "VBNET.ATG" 
		attributes.Add(attribute); 
		while (
#line  2657 "VBNET.ATG" 
NotFinalComma()) {
			if (la.kind == 23) {
				lexer.NextToken();
				if (la.kind == 64) {
					lexer.NextToken();
				} else if (la.kind == 154) {
					lexer.NextToken();
				} else SynErr(240);
				Expect(22);
			}
			Attribute(
#line  2657 "VBNET.ATG" 
out attribute);

#line  2657 "VBNET.ATG" 
			attributes.Add(attribute); 
		}
		if (la.kind == 23) {
			lexer.NextToken();
		}
		Expect(38);
		EndOfStmt();

#line  2662 "VBNET.ATG" 
		AttributeSection section = new AttributeSection {
		AttributeTarget = attributeTarget,
		Attributes = attributes,
		StartLocation = startPos,
		EndLocation = t.EndLocation
		};
		AddChild(section);
		
	}

	void NamespaceMemberDecl() {

#line  359 "VBNET.ATG" 
		ModifierList m = new ModifierList();
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		string qualident;
		
		if (la.kind == 159) {
			lexer.NextToken();

#line  366 "VBNET.ATG" 
			Location startPos = t.Location;
			
			Qualident(
#line  368 "VBNET.ATG" 
out qualident);

#line  370 "VBNET.ATG" 
			INode node =  new NamespaceDeclaration(qualident);
			node.StartLocation = startPos;
			AddChild(node);
			BlockStart(node);
			
			EndOfStmt();
			NamespaceBody();

#line  378 "VBNET.ATG" 
			node.EndLocation = t.Location;
			BlockEnd();
			
		} else if (StartOf(2)) {
			while (la.kind == 39) {
				AttributeSection(
#line  382 "VBNET.ATG" 
out section);

#line  382 "VBNET.ATG" 
				attributes.Add(section); 
			}
			while (StartOf(3)) {
				TypeModifier(
#line  383 "VBNET.ATG" 
m);
			}
			NonModuleDeclaration(
#line  383 "VBNET.ATG" 
m, attributes);
		} else SynErr(241);
	}

	void OptionValue(
#line  298 "VBNET.ATG" 
ref bool val) {
		if (la.kind == 170) {
			lexer.NextToken();

#line  300 "VBNET.ATG" 
			val = true; 
		} else if (la.kind == 169) {
			lexer.NextToken();

#line  302 "VBNET.ATG" 
			val = false; 
		} else SynErr(242);
	}

	void ImportClause(
#line  333 "VBNET.ATG" 
out Using u) {

#line  335 "VBNET.ATG" 
		string qualident  = null;
		TypeReference aliasedType = null;
		u = null;
		
		if (StartOf(4)) {
			Qualident(
#line  340 "VBNET.ATG" 
out qualident);
			if (la.kind == 21) {
				lexer.NextToken();
				TypeName(
#line  341 "VBNET.ATG" 
out aliasedType);
			}

#line  343 "VBNET.ATG" 
			if (qualident != null && qualident.Length > 0) {
			if (aliasedType != null) {
				u = new Using(qualident, aliasedType);
			} else {
				u = new Using(qualident);
			}
			}
			
		} else if (la.kind == 10) {

#line  351 "VBNET.ATG" 
			string prefix = null; 
			lexer.NextToken();
			Identifier();

#line  352 "VBNET.ATG" 
			prefix = t.val; 
			Expect(21);
			Expect(3);

#line  352 "VBNET.ATG" 
			u = new Using(t.literalValue as string, prefix); 
			Expect(11);
		} else SynErr(243);
	}

	void Qualident(
#line  3408 "VBNET.ATG" 
out string qualident) {

#line  3410 "VBNET.ATG" 
		string name;
		qualidentBuilder.Length = 0; 
		
		Identifier();

#line  3414 "VBNET.ATG" 
		qualidentBuilder.Append(t.val); 
		while (
#line  3415 "VBNET.ATG" 
DotAndIdentOrKw()) {
			Expect(27);
			IdentifierOrKeyword(
#line  3415 "VBNET.ATG" 
out name);

#line  3415 "VBNET.ATG" 
			qualidentBuilder.Append('.'); qualidentBuilder.Append(name); 
		}

#line  3417 "VBNET.ATG" 
		qualident = qualidentBuilder.ToString(); 
	}

	void TypeName(
#line  2523 "VBNET.ATG" 
out TypeReference typeref) {

#line  2524 "VBNET.ATG" 
		ArrayList rank = null; 
		NonArrayTypeName(
#line  2526 "VBNET.ATG" 
out typeref, false);
		ArrayTypeModifiers(
#line  2530 "VBNET.ATG" 
out rank);

#line  2531 "VBNET.ATG" 
		if (rank != null && typeref != null) {
		typeref.RankSpecifier = (int[])rank.ToArray(typeof(int));
		}
		
	}

	void Identifier() {
		if (StartOf(5)) {
			IdentifierForFieldDeclaration();
		} else if (la.kind == 97) {
			lexer.NextToken();
		} else SynErr(244);
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
#line  2725 "VBNET.ATG" 
out AttributeSection section) {

#line  2727 "VBNET.ATG" 
		string attributeTarget = "";List<ASTAttribute> attributes = new List<ASTAttribute>();
		ASTAttribute attribute;
		
		
		Expect(39);

#line  2731 "VBNET.ATG" 
		Location startPos = t.Location; 
		if (
#line  2732 "VBNET.ATG" 
IsLocalAttrTarget()) {
			if (la.kind == 118) {
				lexer.NextToken();

#line  2733 "VBNET.ATG" 
				attributeTarget = "event";
			} else if (la.kind == 193) {
				lexer.NextToken();

#line  2734 "VBNET.ATG" 
				attributeTarget = "return";
			} else {
				Identifier();

#line  2737 "VBNET.ATG" 
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
#line  2747 "VBNET.ATG" 
out attribute);

#line  2747 "VBNET.ATG" 
		attributes.Add(attribute); 
		while (
#line  2748 "VBNET.ATG" 
NotFinalComma()) {
			Expect(23);
			Attribute(
#line  2748 "VBNET.ATG" 
out attribute);

#line  2748 "VBNET.ATG" 
			attributes.Add(attribute); 
		}
		if (la.kind == 23) {
			lexer.NextToken();
		}
		Expect(38);

#line  2752 "VBNET.ATG" 
		section = new AttributeSection {
		AttributeTarget = attributeTarget,
		Attributes = attributes,
		StartLocation = startPos,
		EndLocation = t.EndLocation
		};
		
	}

	void TypeModifier(
#line  3491 "VBNET.ATG" 
ModifierList m) {
		switch (la.kind) {
		case 186: {
			lexer.NextToken();

#line  3492 "VBNET.ATG" 
			m.Add(Modifiers.Public, t.Location); 
			break;
		}
		case 185: {
			lexer.NextToken();

#line  3493 "VBNET.ATG" 
			m.Add(Modifiers.Protected, t.Location); 
			break;
		}
		case 124: {
			lexer.NextToken();

#line  3494 "VBNET.ATG" 
			m.Add(Modifiers.Internal, t.Location); 
			break;
		}
		case 183: {
			lexer.NextToken();

#line  3495 "VBNET.ATG" 
			m.Add(Modifiers.Private, t.Location); 
			break;
		}
		case 198: {
			lexer.NextToken();

#line  3496 "VBNET.ATG" 
			m.Add(Modifiers.Static, t.Location); 
			break;
		}
		case 197: {
			lexer.NextToken();

#line  3497 "VBNET.ATG" 
			m.Add(Modifiers.New, t.Location); 
			break;
		}
		case 155: {
			lexer.NextToken();

#line  3498 "VBNET.ATG" 
			m.Add(Modifiers.Abstract, t.Location); 
			break;
		}
		case 165: {
			lexer.NextToken();

#line  3499 "VBNET.ATG" 
			m.Add(Modifiers.Sealed, t.Location); 
			break;
		}
		case 181: {
			lexer.NextToken();

#line  3500 "VBNET.ATG" 
			m.Add(Modifiers.Partial, t.Location); 
			break;
		}
		default: SynErr(245); break;
		}
	}

	void NonModuleDeclaration(
#line  443 "VBNET.ATG" 
ModifierList m, List<AttributeSection> attributes) {

#line  445 "VBNET.ATG" 
		TypeReference typeRef = null;
		List<TypeReference> baseInterfaces = null;
		
		switch (la.kind) {
		case 83: {

#line  448 "VBNET.ATG" 
			m.Check(Modifiers.Classes); 
			lexer.NextToken();

#line  451 "VBNET.ATG" 
			TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
			newType.StartLocation = t.Location;
			AddChild(newType);
			BlockStart(newType);
			
			newType.Type       = ClassType.Class;
			
			Identifier();

#line  458 "VBNET.ATG" 
			newType.Name = t.val; 
			TypeParameterList(
#line  459 "VBNET.ATG" 
newType.Templates);
			EndOfStmt();

#line  461 "VBNET.ATG" 
			newType.BodyStartLocation = t.Location; 
			if (la.kind == 139) {
				ClassBaseType(
#line  462 "VBNET.ATG" 
out typeRef);

#line  462 "VBNET.ATG" 
				SafeAdd(newType, newType.BaseTypes, typeRef); 
			}
			while (la.kind == 135) {
				TypeImplementsClause(
#line  463 "VBNET.ATG" 
out baseInterfaces);

#line  463 "VBNET.ATG" 
				newType.BaseTypes.AddRange(baseInterfaces); 
			}
			ClassBody(
#line  464 "VBNET.ATG" 
newType);
			Expect(112);
			Expect(83);

#line  465 "VBNET.ATG" 
			newType.EndLocation = t.EndLocation; 
			EndOfStmt();

#line  468 "VBNET.ATG" 
			BlockEnd();
			
			break;
		}
		case 154: {
			lexer.NextToken();

#line  472 "VBNET.ATG" 
			m.Check(Modifiers.VBModules);
			TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
			AddChild(newType);
			BlockStart(newType);
			newType.StartLocation = m.GetDeclarationLocation(t.Location);
			newType.Type = ClassType.Module;
			
			Identifier();

#line  479 "VBNET.ATG" 
			newType.Name = t.val; 
			EndOfStmt();

#line  481 "VBNET.ATG" 
			newType.BodyStartLocation = t.Location; 
			ModuleBody(
#line  482 "VBNET.ATG" 
newType);

#line  484 "VBNET.ATG" 
			BlockEnd();
			
			break;
		}
		case 207: {
			lexer.NextToken();

#line  488 "VBNET.ATG" 
			m.Check(Modifiers.VBStructures);
			TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
			AddChild(newType);
			BlockStart(newType);
			newType.StartLocation = m.GetDeclarationLocation(t.Location);
			newType.Type = ClassType.Struct;
			
			Identifier();

#line  495 "VBNET.ATG" 
			newType.Name = t.val; 
			TypeParameterList(
#line  496 "VBNET.ATG" 
newType.Templates);
			EndOfStmt();

#line  498 "VBNET.ATG" 
			newType.BodyStartLocation = t.Location; 
			while (la.kind == 135) {
				TypeImplementsClause(
#line  499 "VBNET.ATG" 
out baseInterfaces);

#line  499 "VBNET.ATG" 
				newType.BaseTypes.AddRange(baseInterfaces);
			}
			StructureBody(
#line  500 "VBNET.ATG" 
newType);

#line  502 "VBNET.ATG" 
			BlockEnd();
			
			break;
		}
		case 114: {
			lexer.NextToken();

#line  507 "VBNET.ATG" 
			m.Check(Modifiers.VBEnums);
			TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
			newType.StartLocation = m.GetDeclarationLocation(t.Location);
			AddChild(newType);
			BlockStart(newType);
			
			newType.Type = ClassType.Enum;
			
			Identifier();

#line  515 "VBNET.ATG" 
			newType.Name = t.val; 
			if (la.kind == 62) {
				lexer.NextToken();
				NonArrayTypeName(
#line  516 "VBNET.ATG" 
out typeRef, false);

#line  516 "VBNET.ATG" 
				SafeAdd(newType, newType.BaseTypes, typeRef); 
			}
			EndOfStmt();

#line  518 "VBNET.ATG" 
			newType.BodyStartLocation = t.Location; 
			EnumBody(
#line  519 "VBNET.ATG" 
newType);

#line  521 "VBNET.ATG" 
			BlockEnd();
			
			break;
		}
		case 141: {
			lexer.NextToken();

#line  526 "VBNET.ATG" 
			m.Check(Modifiers.VBInterfacs);
			TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
			newType.StartLocation = m.GetDeclarationLocation(t.Location);
			AddChild(newType);
			BlockStart(newType);
			newType.Type = ClassType.Interface;
			
			Identifier();

#line  533 "VBNET.ATG" 
			newType.Name = t.val; 
			TypeParameterList(
#line  534 "VBNET.ATG" 
newType.Templates);
			EndOfStmt();

#line  536 "VBNET.ATG" 
			newType.BodyStartLocation = t.Location; 
			while (la.kind == 139) {
				InterfaceBase(
#line  537 "VBNET.ATG" 
out baseInterfaces);

#line  537 "VBNET.ATG" 
				newType.BaseTypes.AddRange(baseInterfaces); 
			}
			InterfaceBody(
#line  538 "VBNET.ATG" 
newType);

#line  540 "VBNET.ATG" 
			BlockEnd();
			
			break;
		}
		case 102: {
			lexer.NextToken();

#line  545 "VBNET.ATG" 
			m.Check(Modifiers.VBDelegates);
			DelegateDeclaration delegateDeclr = new DelegateDeclaration(m.Modifier, attributes);
			delegateDeclr.ReturnType = new TypeReference("System.Void", true);
			delegateDeclr.StartLocation = m.GetDeclarationLocation(t.Location);
			List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
			
			if (la.kind == 208) {
				lexer.NextToken();
				Identifier();

#line  552 "VBNET.ATG" 
				delegateDeclr.Name = t.val; 
				TypeParameterList(
#line  553 "VBNET.ATG" 
delegateDeclr.Templates);
				if (la.kind == 36) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
#line  554 "VBNET.ATG" 
p);
					}
					Expect(37);

#line  554 "VBNET.ATG" 
					delegateDeclr.Parameters = p; 
				}
			} else if (la.kind == 126) {
				lexer.NextToken();
				Identifier();

#line  556 "VBNET.ATG" 
				delegateDeclr.Name = t.val; 
				TypeParameterList(
#line  557 "VBNET.ATG" 
delegateDeclr.Templates);
				if (la.kind == 36) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
#line  558 "VBNET.ATG" 
p);
					}
					Expect(37);

#line  558 "VBNET.ATG" 
					delegateDeclr.Parameters = p; 
				}
				if (la.kind == 62) {
					lexer.NextToken();

#line  559 "VBNET.ATG" 
					TypeReference type; 
					TypeName(
#line  559 "VBNET.ATG" 
out type);

#line  559 "VBNET.ATG" 
					delegateDeclr.ReturnType = type; 
				}
			} else SynErr(246);

#line  561 "VBNET.ATG" 
			delegateDeclr.EndLocation = t.EndLocation; 
			EndOfStmt();

#line  564 "VBNET.ATG" 
			AddChild(delegateDeclr);
			
			break;
		}
		default: SynErr(247); break;
		}
	}

	void TypeParameterList(
#line  387 "VBNET.ATG" 
List<TemplateDefinition> templates) {

#line  389 "VBNET.ATG" 
		TemplateDefinition template;
		
		if (
#line  393 "VBNET.ATG" 
la.kind == Tokens.OpenParenthesis && Peek(1).kind == Tokens.Of) {
			lexer.NextToken();
			Expect(168);
			TypeParameter(
#line  394 "VBNET.ATG" 
out template);

#line  396 "VBNET.ATG" 
			if (template != null) templates.Add(template);
			
			while (la.kind == 23) {
				lexer.NextToken();
				TypeParameter(
#line  399 "VBNET.ATG" 
out template);

#line  401 "VBNET.ATG" 
				if (template != null) templates.Add(template);
				
			}
			Expect(37);
		}
	}

	void TypeParameter(
#line  409 "VBNET.ATG" 
out TemplateDefinition template) {
		Identifier();

#line  411 "VBNET.ATG" 
		template = new TemplateDefinition(t.val, null); 
		if (la.kind == 62) {
			TypeParameterConstraints(
#line  412 "VBNET.ATG" 
template);
		}
	}

	void TypeParameterConstraints(
#line  416 "VBNET.ATG" 
TemplateDefinition template) {

#line  418 "VBNET.ATG" 
		TypeReference constraint;
		
		Expect(62);
		if (la.kind == 34) {
			lexer.NextToken();
			TypeParameterConstraint(
#line  424 "VBNET.ATG" 
out constraint);

#line  424 "VBNET.ATG" 
			if (constraint != null) { template.Bases.Add(constraint); } 
			while (la.kind == 23) {
				lexer.NextToken();
				TypeParameterConstraint(
#line  427 "VBNET.ATG" 
out constraint);

#line  427 "VBNET.ATG" 
				if (constraint != null) { template.Bases.Add(constraint); } 
			}
			Expect(35);
		} else if (StartOf(7)) {
			TypeParameterConstraint(
#line  430 "VBNET.ATG" 
out constraint);

#line  430 "VBNET.ATG" 
			if (constraint != null) { template.Bases.Add(constraint); } 
		} else SynErr(248);
	}

	void TypeParameterConstraint(
#line  434 "VBNET.ATG" 
out TypeReference constraint) {

#line  435 "VBNET.ATG" 
		constraint = null; 
		if (la.kind == 83) {
			lexer.NextToken();

#line  436 "VBNET.ATG" 
			constraint = TypeReference.ClassConstraint; 
		} else if (la.kind == 207) {
			lexer.NextToken();

#line  437 "VBNET.ATG" 
			constraint = TypeReference.StructConstraint; 
		} else if (la.kind == 161) {
			lexer.NextToken();

#line  438 "VBNET.ATG" 
			constraint = TypeReference.NewConstraint; 
		} else if (StartOf(8)) {
			TypeName(
#line  439 "VBNET.ATG" 
out constraint);
		} else SynErr(249);
	}

	void ClassBaseType(
#line  784 "VBNET.ATG" 
out TypeReference typeRef) {

#line  786 "VBNET.ATG" 
		typeRef = null;
		
		Expect(139);
		TypeName(
#line  789 "VBNET.ATG" 
out typeRef);
		EndOfStmt();
	}

	void TypeImplementsClause(
#line  1601 "VBNET.ATG" 
out List<TypeReference> baseInterfaces) {

#line  1603 "VBNET.ATG" 
		baseInterfaces = new List<TypeReference>();
		TypeReference type = null;
		
		Expect(135);
		TypeName(
#line  1606 "VBNET.ATG" 
out type);

#line  1608 "VBNET.ATG" 
		if (type != null) baseInterfaces.Add(type);
		
		while (la.kind == 23) {
			lexer.NextToken();
			TypeName(
#line  1611 "VBNET.ATG" 
out type);

#line  1612 "VBNET.ATG" 
			if (type != null) baseInterfaces.Add(type); 
		}
		EndOfStmt();
	}

	void ClassBody(
#line  578 "VBNET.ATG" 
TypeDeclaration newType) {

#line  579 "VBNET.ATG" 
		AttributeSection section; 
		while (la.kind == 1 || la.kind == 22) {
			EndOfStmt();
		}
		while (StartOf(9)) {

#line  582 "VBNET.ATG" 
			List<AttributeSection> attributes = new List<AttributeSection>();
			ModifierList m = new ModifierList();
			
			while (la.kind == 39) {
				AttributeSection(
#line  585 "VBNET.ATG" 
out section);

#line  585 "VBNET.ATG" 
				attributes.Add(section); 
			}
			while (StartOf(10)) {
				MemberModifier(
#line  586 "VBNET.ATG" 
m);
			}
			ClassMemberDecl(
#line  587 "VBNET.ATG" 
m, attributes);
			while (la.kind == 1 || la.kind == 22) {
				EndOfStmt();
			}
		}
	}

	void ModuleBody(
#line  609 "VBNET.ATG" 
TypeDeclaration newType) {

#line  610 "VBNET.ATG" 
		AttributeSection section; 
		while (la.kind == 1 || la.kind == 22) {
			EndOfStmt();
		}
		while (StartOf(9)) {

#line  613 "VBNET.ATG" 
			List<AttributeSection> attributes = new List<AttributeSection>();
			ModifierList m = new ModifierList();
			
			while (la.kind == 39) {
				AttributeSection(
#line  616 "VBNET.ATG" 
out section);

#line  616 "VBNET.ATG" 
				attributes.Add(section); 
			}
			while (StartOf(10)) {
				MemberModifier(
#line  617 "VBNET.ATG" 
m);
			}
			ClassMemberDecl(
#line  618 "VBNET.ATG" 
m, attributes);
			while (la.kind == 1 || la.kind == 22) {
				EndOfStmt();
			}
		}
		Expect(112);
		Expect(154);

#line  621 "VBNET.ATG" 
		newType.EndLocation = t.EndLocation; 
		EndOfStmt();
	}

	void StructureBody(
#line  592 "VBNET.ATG" 
TypeDeclaration newType) {

#line  593 "VBNET.ATG" 
		AttributeSection section; 
		while (la.kind == 1 || la.kind == 22) {
			EndOfStmt();
		}
		while (StartOf(9)) {

#line  596 "VBNET.ATG" 
			List<AttributeSection> attributes = new List<AttributeSection>();
			ModifierList m = new ModifierList();
			
			while (la.kind == 39) {
				AttributeSection(
#line  599 "VBNET.ATG" 
out section);

#line  599 "VBNET.ATG" 
				attributes.Add(section); 
			}
			while (StartOf(10)) {
				MemberModifier(
#line  600 "VBNET.ATG" 
m);
			}
			StructureMemberDecl(
#line  601 "VBNET.ATG" 
m, attributes);
			while (la.kind == 1 || la.kind == 22) {
				EndOfStmt();
			}
		}
		Expect(112);
		Expect(207);

#line  604 "VBNET.ATG" 
		newType.EndLocation = t.EndLocation; 
		EndOfStmt();
	}

	void NonArrayTypeName(
#line  2549 "VBNET.ATG" 
out TypeReference typeref, bool canBeUnbound) {

#line  2551 "VBNET.ATG" 
		string name;
		typeref = null;
		bool isGlobal = false;
		
		if (StartOf(11)) {
			if (la.kind == 129) {
				lexer.NextToken();
				Expect(27);

#line  2556 "VBNET.ATG" 
				isGlobal = true; 
			}
			QualIdentAndTypeArguments(
#line  2557 "VBNET.ATG" 
out typeref, canBeUnbound);

#line  2558 "VBNET.ATG" 
			typeref.IsGlobal = isGlobal; 
			while (la.kind == 27) {
				lexer.NextToken();

#line  2559 "VBNET.ATG" 
				TypeReference nestedTypeRef; 
				QualIdentAndTypeArguments(
#line  2560 "VBNET.ATG" 
out nestedTypeRef, canBeUnbound);

#line  2561 "VBNET.ATG" 
				typeref = new InnerClassTypeReference(typeref, nestedTypeRef.Type, nestedTypeRef.GenericTypes); 
			}
		} else if (la.kind == 167) {
			lexer.NextToken();

#line  2564 "VBNET.ATG" 
			typeref = new TypeReference("System.Object", true); 
			if (la.kind == 32) {
				lexer.NextToken();

#line  2568 "VBNET.ATG" 
				List<TypeReference> typeArguments = new List<TypeReference>(1);
				if (typeref != null) typeArguments.Add(typeref);
				typeref = new TypeReference("System.Nullable", typeArguments) { IsKeyword = true };
				
			}
		} else if (StartOf(12)) {
			PrimitiveTypeName(
#line  2574 "VBNET.ATG" 
out name);

#line  2574 "VBNET.ATG" 
			typeref = new TypeReference(name, true); 
			if (la.kind == 32) {
				lexer.NextToken();

#line  2578 "VBNET.ATG" 
				List<TypeReference> typeArguments = new List<TypeReference>(1);
				if (typeref != null) typeArguments.Add(typeref);
				typeref = new TypeReference("System.Nullable", typeArguments) { IsKeyword = true };
				
			}
		} else SynErr(250);
	}

	void EnumBody(
#line  625 "VBNET.ATG" 
TypeDeclaration newType) {

#line  626 "VBNET.ATG" 
		FieldDeclaration f; 
		while (la.kind == 1 || la.kind == 22) {
			EndOfStmt();
		}
		while (StartOf(13)) {
			EnumMemberDecl(
#line  629 "VBNET.ATG" 
out f);

#line  631 "VBNET.ATG" 
			AddChild(f);
			
			while (la.kind == 1 || la.kind == 22) {
				EndOfStmt();
			}
		}
		Expect(112);
		Expect(114);

#line  635 "VBNET.ATG" 
		newType.EndLocation = t.EndLocation; 
		EndOfStmt();
	}

	void InterfaceBase(
#line  1586 "VBNET.ATG" 
out List<TypeReference> bases) {

#line  1588 "VBNET.ATG" 
		TypeReference type;
		bases = new List<TypeReference>();
		
		Expect(139);
		TypeName(
#line  1592 "VBNET.ATG" 
out type);

#line  1592 "VBNET.ATG" 
		if (type != null) bases.Add(type); 
		while (la.kind == 23) {
			lexer.NextToken();
			TypeName(
#line  1595 "VBNET.ATG" 
out type);

#line  1595 "VBNET.ATG" 
			if (type != null) bases.Add(type); 
		}
		EndOfStmt();
	}

	void InterfaceBody(
#line  639 "VBNET.ATG" 
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

#line  645 "VBNET.ATG" 
		newType.EndLocation = t.EndLocation; 
		EndOfStmt();
	}

	void FormalParameterList(
#line  2762 "VBNET.ATG" 
List<ParameterDeclarationExpression> parameter) {

#line  2763 "VBNET.ATG" 
		ParameterDeclarationExpression p; 
		FormalParameter(
#line  2765 "VBNET.ATG" 
out p);

#line  2765 "VBNET.ATG" 
		if (p != null) parameter.Add(p); 
		while (la.kind == 23) {
			lexer.NextToken();
			FormalParameter(
#line  2767 "VBNET.ATG" 
out p);

#line  2767 "VBNET.ATG" 
			if (p != null) parameter.Add(p); 
		}
	}

	void MemberModifier(
#line  3503 "VBNET.ATG" 
ModifierList m) {
		switch (la.kind) {
		case 155: {
			lexer.NextToken();

#line  3504 "VBNET.ATG" 
			m.Add(Modifiers.Abstract, t.Location);
			break;
		}
		case 101: {
			lexer.NextToken();

#line  3505 "VBNET.ATG" 
			m.Add(Modifiers.Default, t.Location);
			break;
		}
		case 124: {
			lexer.NextToken();

#line  3506 "VBNET.ATG" 
			m.Add(Modifiers.Internal, t.Location);
			break;
		}
		case 197: {
			lexer.NextToken();

#line  3507 "VBNET.ATG" 
			m.Add(Modifiers.New, t.Location);
			break;
		}
		case 179: {
			lexer.NextToken();

#line  3508 "VBNET.ATG" 
			m.Add(Modifiers.Override, t.Location);
			break;
		}
		case 156: {
			lexer.NextToken();

#line  3509 "VBNET.ATG" 
			m.Add(Modifiers.Abstract, t.Location);
			break;
		}
		case 183: {
			lexer.NextToken();

#line  3510 "VBNET.ATG" 
			m.Add(Modifiers.Private, t.Location);
			break;
		}
		case 185: {
			lexer.NextToken();

#line  3511 "VBNET.ATG" 
			m.Add(Modifiers.Protected, t.Location);
			break;
		}
		case 186: {
			lexer.NextToken();

#line  3512 "VBNET.ATG" 
			m.Add(Modifiers.Public, t.Location);
			break;
		}
		case 165: {
			lexer.NextToken();

#line  3513 "VBNET.ATG" 
			m.Add(Modifiers.Sealed, t.Location);
			break;
		}
		case 166: {
			lexer.NextToken();

#line  3514 "VBNET.ATG" 
			m.Add(Modifiers.Sealed, t.Location);
			break;
		}
		case 198: {
			lexer.NextToken();

#line  3515 "VBNET.ATG" 
			m.Add(Modifiers.Static, t.Location);
			break;
		}
		case 178: {
			lexer.NextToken();

#line  3516 "VBNET.ATG" 
			m.Add(Modifiers.Virtual, t.Location);
			break;
		}
		case 177: {
			lexer.NextToken();

#line  3517 "VBNET.ATG" 
			m.Add(Modifiers.Overloads, t.Location);
			break;
		}
		case 188: {
			lexer.NextToken();

#line  3518 "VBNET.ATG" 
			m.Add(Modifiers.ReadOnly, t.Location);
			break;
		}
		case 233: {
			lexer.NextToken();

#line  3519 "VBNET.ATG" 
			m.Add(Modifiers.WriteOnly, t.Location);
			break;
		}
		case 232: {
			lexer.NextToken();

#line  3520 "VBNET.ATG" 
			m.Add(Modifiers.WithEvents, t.Location);
			break;
		}
		case 104: {
			lexer.NextToken();

#line  3521 "VBNET.ATG" 
			m.Add(Modifiers.Dim, t.Location);
			break;
		}
		case 181: {
			lexer.NextToken();

#line  3522 "VBNET.ATG" 
			m.Add(Modifiers.Partial, t.Location);
			break;
		}
		default: SynErr(251); break;
		}
	}

	void ClassMemberDecl(
#line  780 "VBNET.ATG" 
ModifierList m, List<AttributeSection> attributes) {
		StructureMemberDecl(
#line  781 "VBNET.ATG" 
m, attributes);
	}

	void StructureMemberDecl(
#line  794 "VBNET.ATG" 
ModifierList m, List<AttributeSection> attributes) {

#line  796 "VBNET.ATG" 
		TypeReference type = null;
		List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
		Statement stmt = null;
		List<VariableDeclaration> variableDeclarators = new List<VariableDeclaration>();
		List<TemplateDefinition> templates = new List<TemplateDefinition>();
		
		switch (la.kind) {
		case 83: case 102: case 114: case 141: case 154: case 207: {
			NonModuleDeclaration(
#line  803 "VBNET.ATG" 
m, attributes);
			break;
		}
		case 208: {
			lexer.NextToken();

#line  807 "VBNET.ATG" 
			Location startPos = t.Location;
			
			if (StartOf(4)) {

#line  811 "VBNET.ATG" 
				string name = String.Empty;
				MethodDeclaration methodDeclaration; List<string> handlesClause = null;
				List<InterfaceImplementation> implementsClause = null;
				
				Identifier();

#line  817 "VBNET.ATG" 
				name = t.val;
				m.Check(Modifiers.VBMethods);
				
				TypeParameterList(
#line  820 "VBNET.ATG" 
templates);
				if (la.kind == 36) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
#line  821 "VBNET.ATG" 
p);
					}
					Expect(37);
				}
				if (la.kind == 133 || la.kind == 135) {
					if (la.kind == 135) {
						ImplementsClause(
#line  824 "VBNET.ATG" 
out implementsClause);
					} else {
						HandlesClause(
#line  826 "VBNET.ATG" 
out handlesClause);
					}
				}

#line  829 "VBNET.ATG" 
				Location endLocation = t.EndLocation; 
				if (
#line  832 "VBNET.ATG" 
IsMustOverride(m)) {
					EndOfStmt();

#line  835 "VBNET.ATG" 
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

#line  848 "VBNET.ATG" 
					methodDeclaration = new MethodDeclaration {
					Name = name, Modifier = m.Modifier, Parameters = p, Attributes = attributes,
					StartLocation = m.GetDeclarationLocation(startPos), EndLocation = endLocation,
					TypeReference = new TypeReference("System.Void", true),
					Templates = templates,
					HandlesClause = handlesClause,
					InterfaceImplementations = implementsClause
					};
					AddChild(methodDeclaration);
					

#line  859 "VBNET.ATG" 
					if (ParseMethodBodies) { 
					Block(
#line  860 "VBNET.ATG" 
out stmt);
					Expect(112);
					Expect(208);

#line  862 "VBNET.ATG" 
					} else {
					// don't parse method body
					lexer.SkipCurrentBlock(Tokens.Sub); stmt = new BlockStatement();
					  }
					

#line  868 "VBNET.ATG" 
					methodDeclaration.Body  = (BlockStatement)stmt; 

#line  869 "VBNET.ATG" 
					methodDeclaration.Body.EndLocation = t.EndLocation; 
					EndOfStmt();
				} else SynErr(252);
			} else if (la.kind == 161) {
				lexer.NextToken();
				if (la.kind == 36) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
#line  873 "VBNET.ATG" 
p);
					}
					Expect(37);
				}

#line  874 "VBNET.ATG" 
				m.Check(Modifiers.Constructors); 

#line  875 "VBNET.ATG" 
				Location constructorEndLocation = t.EndLocation; 
				Expect(1);

#line  878 "VBNET.ATG" 
				if (ParseMethodBodies) { 
				Block(
#line  879 "VBNET.ATG" 
out stmt);
				Expect(112);
				Expect(208);

#line  881 "VBNET.ATG" 
				} else {
				// don't parse method body
				lexer.SkipCurrentBlock(Tokens.Sub); stmt = new BlockStatement();
				  }
				

#line  887 "VBNET.ATG" 
				Location endLocation = t.EndLocation; 
				EndOfStmt();

#line  890 "VBNET.ATG" 
				ConstructorDeclaration cd = new ConstructorDeclaration("New", m.Modifier, p, attributes);
				cd.StartLocation = m.GetDeclarationLocation(startPos);
				cd.EndLocation   = constructorEndLocation;
				cd.Body = (BlockStatement)stmt;
				cd.Body.EndLocation   = endLocation;
				AddChild(cd);
				
			} else SynErr(253);
			break;
		}
		case 126: {
			lexer.NextToken();

#line  902 "VBNET.ATG" 
			m.Check(Modifiers.VBMethods);
			string name = String.Empty;
			Location startPos = t.Location;
			MethodDeclaration methodDeclaration;List<string> handlesClause = null;
			List<InterfaceImplementation> implementsClause = null;
			AttributeSection returnTypeAttributeSection = null;
			
			Identifier();

#line  909 "VBNET.ATG" 
			name = t.val; 
			TypeParameterList(
#line  910 "VBNET.ATG" 
templates);
			if (la.kind == 36) {
				lexer.NextToken();
				if (StartOf(6)) {
					FormalParameterList(
#line  911 "VBNET.ATG" 
p);
				}
				Expect(37);
			}
			if (la.kind == 62) {
				lexer.NextToken();
				while (la.kind == 39) {
					AttributeSection(
#line  913 "VBNET.ATG" 
out returnTypeAttributeSection);

#line  915 "VBNET.ATG" 
					if (returnTypeAttributeSection != null) {
					returnTypeAttributeSection.AttributeTarget = "return";
					attributes.Add(returnTypeAttributeSection);
					}
					
				}
				TypeName(
#line  921 "VBNET.ATG" 
out type);
			}

#line  923 "VBNET.ATG" 
			if(type == null) {
			type = new TypeReference("System.Object", true);
			}
			
			if (la.kind == 133 || la.kind == 135) {
				if (la.kind == 135) {
					ImplementsClause(
#line  929 "VBNET.ATG" 
out implementsClause);
				} else {
					HandlesClause(
#line  931 "VBNET.ATG" 
out handlesClause);
				}
			}

#line  934 "VBNET.ATG" 
			Location endLocation = t.EndLocation; 
			if (
#line  937 "VBNET.ATG" 
IsMustOverride(m)) {
				EndOfStmt();

#line  940 "VBNET.ATG" 
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

#line  955 "VBNET.ATG" 
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
#line  968 "VBNET.ATG" 
out stmt);
				Expect(112);
				Expect(126);

#line  970 "VBNET.ATG" 
				} else {
				// don't parse method body
				lexer.SkipCurrentBlock(Tokens.Function); stmt = new BlockStatement();
				}
				methodDeclaration.Body = (BlockStatement)stmt;
				methodDeclaration.Body.StartLocation = methodDeclaration.EndLocation;
				methodDeclaration.Body.EndLocation   = t.EndLocation;
				
				EndOfStmt();
			} else SynErr(254);
			break;
		}
		case 100: {
			lexer.NextToken();

#line  984 "VBNET.ATG" 
			m.Check(Modifiers.VBExternalMethods);
			Location startPos = t.Location;
			CharsetModifier charsetModifer = CharsetModifier.None;
			string library = String.Empty;
			string alias = null;
			string name = String.Empty;
			
			if (StartOf(15)) {
				Charset(
#line  991 "VBNET.ATG" 
out charsetModifer);
			}
			if (la.kind == 208) {
				lexer.NextToken();
				Identifier();

#line  994 "VBNET.ATG" 
				name = t.val; 
				Expect(148);
				Expect(3);

#line  995 "VBNET.ATG" 
				library = t.literalValue as string; 
				if (la.kind == 58) {
					lexer.NextToken();
					Expect(3);

#line  996 "VBNET.ATG" 
					alias = t.literalValue as string; 
				}
				if (la.kind == 36) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
#line  997 "VBNET.ATG" 
p);
					}
					Expect(37);
				}
				EndOfStmt();

#line  1000 "VBNET.ATG" 
				DeclareDeclaration declareDeclaration = new DeclareDeclaration(name, m.Modifier, null, p, attributes, library, alias, charsetModifer);
				declareDeclaration.StartLocation = m.GetDeclarationLocation(startPos);
				declareDeclaration.EndLocation   = t.EndLocation;
				AddChild(declareDeclaration);
				
			} else if (la.kind == 126) {
				lexer.NextToken();
				Identifier();

#line  1007 "VBNET.ATG" 
				name = t.val; 
				Expect(148);
				Expect(3);

#line  1008 "VBNET.ATG" 
				library = t.literalValue as string; 
				if (la.kind == 58) {
					lexer.NextToken();
					Expect(3);

#line  1009 "VBNET.ATG" 
					alias = t.literalValue as string; 
				}
				if (la.kind == 36) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
#line  1010 "VBNET.ATG" 
p);
					}
					Expect(37);
				}
				if (la.kind == 62) {
					lexer.NextToken();
					TypeName(
#line  1011 "VBNET.ATG" 
out type);
				}
				EndOfStmt();

#line  1014 "VBNET.ATG" 
				DeclareDeclaration declareDeclaration = new DeclareDeclaration(name, m.Modifier, type, p, attributes, library, alias, charsetModifer);
				declareDeclaration.StartLocation = m.GetDeclarationLocation(startPos);
				declareDeclaration.EndLocation   = t.EndLocation;
				AddChild(declareDeclaration);
				
			} else SynErr(255);
			break;
		}
		case 118: {
			lexer.NextToken();

#line  1024 "VBNET.ATG" 
			m.Check(Modifiers.VBEvents);
			Location startPos = t.Location;
			EventDeclaration eventDeclaration;
			string name = String.Empty;
			List<InterfaceImplementation> implementsClause = null;
			
			Identifier();

#line  1030 "VBNET.ATG" 
			name= t.val; 
			if (la.kind == 62) {
				lexer.NextToken();
				TypeName(
#line  1032 "VBNET.ATG" 
out type);
			} else if (StartOf(16)) {
				if (la.kind == 36) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
#line  1034 "VBNET.ATG" 
p);
					}
					Expect(37);
				}
			} else SynErr(256);
			if (la.kind == 135) {
				ImplementsClause(
#line  1036 "VBNET.ATG" 
out implementsClause);
			}

#line  1038 "VBNET.ATG" 
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

#line  1049 "VBNET.ATG" 
			m.Check(Modifiers.Fields);
			FieldDeclaration fd = new FieldDeclaration(attributes, null, m.Modifier);
			
			IdentifierForFieldDeclaration();

#line  1052 "VBNET.ATG" 
			string name = t.val; 

#line  1053 "VBNET.ATG" 
			fd.StartLocation = m.GetDeclarationLocation(t.Location); 
			VariableDeclaratorPartAfterIdentifier(
#line  1055 "VBNET.ATG" 
variableDeclarators, name);
			while (la.kind == 23) {
				lexer.NextToken();
				VariableDeclarator(
#line  1056 "VBNET.ATG" 
variableDeclarators);
			}
			EndOfStmt();

#line  1059 "VBNET.ATG" 
			fd.EndLocation = t.EndLocation;
			fd.Fields = variableDeclarators;
			AddChild(fd);
			
			break;
		}
		case 87: {

#line  1064 "VBNET.ATG" 
			m.Check(Modifiers.Fields); 
			lexer.NextToken();

#line  1065 "VBNET.ATG" 
			m.Add(Modifiers.Const, t.Location);  

#line  1067 "VBNET.ATG" 
			FieldDeclaration fd = new FieldDeclaration(attributes, type, m.Modifier);
			fd.StartLocation = m.GetDeclarationLocation(t.Location);
			List<VariableDeclaration> constantDeclarators = new List<VariableDeclaration>();
			
			ConstantDeclarator(
#line  1071 "VBNET.ATG" 
constantDeclarators);
			while (la.kind == 23) {
				lexer.NextToken();
				ConstantDeclarator(
#line  1072 "VBNET.ATG" 
constantDeclarators);
			}

#line  1074 "VBNET.ATG" 
			fd.Fields = constantDeclarators;
			fd.EndLocation = t.Location;
			
			EndOfStmt();

#line  1079 "VBNET.ATG" 
			fd.EndLocation = t.EndLocation;
			AddChild(fd);
			
			break;
		}
		case 184: {
			lexer.NextToken();

#line  1085 "VBNET.ATG" 
			m.Check(Modifiers.VBProperties);
			Location startPos = t.Location;
			List<InterfaceImplementation> implementsClause = null;
			AttributeSection returnTypeAttributeSection = null;
			Expression initializer = null;
			
			Identifier();

#line  1091 "VBNET.ATG" 
			string propertyName = t.val; 
			if (la.kind == 36) {
				lexer.NextToken();
				if (StartOf(6)) {
					FormalParameterList(
#line  1092 "VBNET.ATG" 
p);
				}
				Expect(37);
			}
			if (la.kind == 62) {
				lexer.NextToken();
				while (la.kind == 39) {
					AttributeSection(
#line  1095 "VBNET.ATG" 
out returnTypeAttributeSection);

#line  1097 "VBNET.ATG" 
					if (returnTypeAttributeSection != null) {
					returnTypeAttributeSection.AttributeTarget = "return";
					attributes.Add(returnTypeAttributeSection);
					}
					
				}
				if (
#line  1104 "VBNET.ATG" 
IsNewExpression()) {
					ObjectCreateExpression(
#line  1104 "VBNET.ATG" 
out initializer);

#line  1106 "VBNET.ATG" 
					if (initializer is ObjectCreateExpression) {
					type = ((ObjectCreateExpression)initializer).CreateType.Clone();
					} else {
						type = ((ArrayCreateExpression)initializer).CreateType.Clone();
					}
					
				} else if (StartOf(8)) {
					TypeName(
#line  1113 "VBNET.ATG" 
out type);
				} else SynErr(257);
			}
			if (la.kind == 21) {
				lexer.NextToken();
				Expr(
#line  1116 "VBNET.ATG" 
out initializer);
			}
			if (la.kind == 135) {
				ImplementsClause(
#line  1117 "VBNET.ATG" 
out implementsClause);
			}
			EndOfStmt();
			if (
#line  1121 "VBNET.ATG" 
IsMustOverride(m) || IsAutomaticProperty()) {

#line  1123 "VBNET.ATG" 
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

#line  1135 "VBNET.ATG" 
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
#line  1145 "VBNET.ATG" 
out getRegion, out setRegion);
				Expect(112);
				Expect(184);
				EndOfStmt();

#line  1149 "VBNET.ATG" 
				pDecl.GetRegion = getRegion;
				pDecl.SetRegion = setRegion;
				pDecl.BodyEnd = t.Location; // t = EndOfStmt; not "Property"
				AddChild(pDecl);
				
			} else SynErr(258);
			break;
		}
		case 97: {
			lexer.NextToken();

#line  1156 "VBNET.ATG" 
			Location startPos = t.Location; 
			Expect(118);

#line  1158 "VBNET.ATG" 
			m.Check(Modifiers.VBCustomEvents);
			EventAddRemoveRegion eventAccessorDeclaration;
			EventAddRegion addHandlerAccessorDeclaration = null;
			EventRemoveRegion removeHandlerAccessorDeclaration = null;
			EventRaiseRegion raiseEventAccessorDeclaration = null;
			List<InterfaceImplementation> implementsClause = null;
			
			Identifier();

#line  1165 "VBNET.ATG" 
			string customEventName = t.val; 
			Expect(62);
			TypeName(
#line  1166 "VBNET.ATG" 
out type);
			if (la.kind == 135) {
				ImplementsClause(
#line  1167 "VBNET.ATG" 
out implementsClause);
			}
			EndOfStmt();
			while (StartOf(18)) {
				EventAccessorDeclaration(
#line  1170 "VBNET.ATG" 
out eventAccessorDeclaration);

#line  1172 "VBNET.ATG" 
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

#line  1188 "VBNET.ATG" 
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

#line  1214 "VBNET.ATG" 
			ConversionType opConversionType = ConversionType.None; 
			if (la.kind == 160 || la.kind == 230) {
				if (la.kind == 230) {
					lexer.NextToken();

#line  1215 "VBNET.ATG" 
					opConversionType = ConversionType.Implicit; 
				} else {
					lexer.NextToken();

#line  1216 "VBNET.ATG" 
					opConversionType = ConversionType.Explicit;
				}
			}
			Expect(171);

#line  1219 "VBNET.ATG" 
			m.Check(Modifiers.VBOperators);
			Location startPos = t.Location;
			TypeReference returnType = NullTypeReference.Instance;
			TypeReference operandType = NullTypeReference.Instance;
			string operandName;
			OverloadableOperatorType operatorType;
			AttributeSection section;
			List<ParameterDeclarationExpression> parameters = new List<ParameterDeclarationExpression>();
			
			OverloadableOperator(
#line  1228 "VBNET.ATG" 
out operatorType);
			Expect(36);
			if (la.kind == 71) {
				lexer.NextToken();
			}
			Identifier();

#line  1229 "VBNET.ATG" 
			operandName = t.val; 
			if (la.kind == 62) {
				lexer.NextToken();
				TypeName(
#line  1230 "VBNET.ATG" 
out operandType);
			}

#line  1231 "VBNET.ATG" 
			parameters.Add(new ParameterDeclarationExpression(operandType, operandName, ParameterModifiers.In)); 
			while (la.kind == 23) {
				lexer.NextToken();
				if (la.kind == 71) {
					lexer.NextToken();
				}
				Identifier();

#line  1235 "VBNET.ATG" 
				operandName = t.val; 
				if (la.kind == 62) {
					lexer.NextToken();
					TypeName(
#line  1236 "VBNET.ATG" 
out operandType);
				}

#line  1237 "VBNET.ATG" 
				parameters.Add(new ParameterDeclarationExpression(operandType, operandName, ParameterModifiers.In)); 
			}
			Expect(37);

#line  1240 "VBNET.ATG" 
			Location endPos = t.EndLocation; 
			if (la.kind == 62) {
				lexer.NextToken();
				while (la.kind == 39) {
					AttributeSection(
#line  1241 "VBNET.ATG" 
out section);

#line  1242 "VBNET.ATG" 
					if (section != null) {
					section.AttributeTarget = "return";
					attributes.Add(section);
					} 
				}
				TypeName(
#line  1246 "VBNET.ATG" 
out returnType);

#line  1246 "VBNET.ATG" 
				endPos = t.EndLocation; 
			}
			Expect(1);
			Block(
#line  1248 "VBNET.ATG" 
out stmt);
			Expect(112);
			Expect(171);
			EndOfStmt();

#line  1250 "VBNET.ATG" 
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
		default: SynErr(259); break;
		}
	}

	void EnumMemberDecl(
#line  762 "VBNET.ATG" 
out FieldDeclaration f) {

#line  764 "VBNET.ATG" 
		Expression expr = null;List<AttributeSection> attributes = new List<AttributeSection>();
		AttributeSection section = null;
		VariableDeclaration varDecl = null;
		
		while (la.kind == 39) {
			AttributeSection(
#line  768 "VBNET.ATG" 
out section);

#line  768 "VBNET.ATG" 
			attributes.Add(section); 
		}
		Identifier();

#line  771 "VBNET.ATG" 
		f = new FieldDeclaration(attributes);
		varDecl = new VariableDeclaration(t.val);
		f.Fields.Add(varDecl);
		f.StartLocation = varDecl.StartLocation = t.Location;
		
		if (la.kind == 21) {
			lexer.NextToken();
			Expr(
#line  776 "VBNET.ATG" 
out expr);

#line  776 "VBNET.ATG" 
			varDecl.Initializer = expr; 
		}
		EndOfStmt();
	}

	void InterfaceMemberDecl() {

#line  653 "VBNET.ATG" 
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
#line  661 "VBNET.ATG" 
out section);

#line  661 "VBNET.ATG" 
				attributes.Add(section); 
			}
			while (StartOf(10)) {
				MemberModifier(
#line  664 "VBNET.ATG" 
mod);
			}
			if (la.kind == 118) {
				lexer.NextToken();

#line  668 "VBNET.ATG" 
				mod.Check(Modifiers.VBInterfaceEvents);
				Location startLocation = t.Location;
				
				Identifier();

#line  671 "VBNET.ATG" 
				name = t.val; 
				if (la.kind == 36) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
#line  672 "VBNET.ATG" 
p);
					}
					Expect(37);
				}
				if (la.kind == 62) {
					lexer.NextToken();
					TypeName(
#line  673 "VBNET.ATG" 
out type);
				}
				EndOfStmt();

#line  676 "VBNET.ATG" 
				EventDeclaration ed = new EventDeclaration {
				Name = name, TypeReference = type, Modifier = mod.Modifier,
				Parameters = p, Attributes = attributes,
				StartLocation = startLocation, EndLocation = t.EndLocation
				};
				AddChild(ed);
				
			} else if (la.kind == 208) {
				lexer.NextToken();

#line  686 "VBNET.ATG" 
				Location startLocation =  t.Location;
				mod.Check(Modifiers.VBInterfaceMethods);
				
				Identifier();

#line  689 "VBNET.ATG" 
				name = t.val; 
				TypeParameterList(
#line  690 "VBNET.ATG" 
templates);
				if (la.kind == 36) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
#line  691 "VBNET.ATG" 
p);
					}
					Expect(37);
				}
				EndOfStmt();

#line  694 "VBNET.ATG" 
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

#line  709 "VBNET.ATG" 
				mod.Check(Modifiers.VBInterfaceMethods);
				Location startLocation = t.Location;
				
				Identifier();

#line  712 "VBNET.ATG" 
				name = t.val; 
				TypeParameterList(
#line  713 "VBNET.ATG" 
templates);
				if (la.kind == 36) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
#line  714 "VBNET.ATG" 
p);
					}
					Expect(37);
				}
				if (la.kind == 62) {
					lexer.NextToken();
					while (la.kind == 39) {
						AttributeSection(
#line  715 "VBNET.ATG" 
out returnTypeAttributeSection);
					}
					TypeName(
#line  715 "VBNET.ATG" 
out type);
				}

#line  717 "VBNET.ATG" 
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

#line  737 "VBNET.ATG" 
				Location startLocation = t.Location;
				mod.Check(Modifiers.VBInterfaceProperties);
				
				Identifier();

#line  740 "VBNET.ATG" 
				name = t.val;  
				if (la.kind == 36) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
#line  741 "VBNET.ATG" 
p);
					}
					Expect(37);
				}
				if (la.kind == 62) {
					lexer.NextToken();
					TypeName(
#line  742 "VBNET.ATG" 
out type);
				}

#line  744 "VBNET.ATG" 
				if(type == null) {
				type = new TypeReference("System.Object", true);
				}
				
				EndOfStmt();

#line  750 "VBNET.ATG" 
				PropertyDeclaration pd = new PropertyDeclaration(name, type, mod.Modifier, attributes);
				pd.Parameters = p;
				pd.EndLocation = t.EndLocation;
				pd.StartLocation = startLocation;
				AddChild(pd);
				
			} else SynErr(260);
		} else if (StartOf(20)) {
			NonModuleDeclaration(
#line  758 "VBNET.ATG" 
mod, attributes);
		} else SynErr(261);
	}

	void Expr(
#line  1645 "VBNET.ATG" 
out Expression expr) {

#line  1646 "VBNET.ATG" 
		expr = null; 
		if (
#line  1647 "VBNET.ATG" 
IsQueryExpression() ) {
			QueryExpr(
#line  1648 "VBNET.ATG" 
out expr);
		} else if (la.kind == 126 || la.kind == 208) {
			LambdaExpr(
#line  1649 "VBNET.ATG" 
out expr);
		} else if (StartOf(21)) {
			DisjunctionExpr(
#line  1650 "VBNET.ATG" 
out expr);
		} else SynErr(262);
	}

	void ImplementsClause(
#line  1618 "VBNET.ATG" 
out List<InterfaceImplementation> baseInterfaces) {

#line  1620 "VBNET.ATG" 
		baseInterfaces = new List<InterfaceImplementation>();
		TypeReference type = null;
		string memberName = null;
		
		Expect(135);
		NonArrayTypeName(
#line  1625 "VBNET.ATG" 
out type, false);

#line  1626 "VBNET.ATG" 
		if (type != null) memberName = TypeReference.StripLastIdentifierFromType(ref type); 

#line  1627 "VBNET.ATG" 
		baseInterfaces.Add(new InterfaceImplementation(type, memberName)); 
		while (la.kind == 23) {
			lexer.NextToken();
			NonArrayTypeName(
#line  1629 "VBNET.ATG" 
out type, false);

#line  1630 "VBNET.ATG" 
			if (type != null) memberName = TypeReference.StripLastIdentifierFromType(ref type); 

#line  1631 "VBNET.ATG" 
			baseInterfaces.Add(new InterfaceImplementation(type, memberName)); 
		}
	}

	void HandlesClause(
#line  1576 "VBNET.ATG" 
out List<string> handlesClause) {

#line  1578 "VBNET.ATG" 
		handlesClause = new List<string>();
		string name;
		
		Expect(133);
		EventMemberSpecifier(
#line  1581 "VBNET.ATG" 
out name);

#line  1581 "VBNET.ATG" 
		if (name != null) handlesClause.Add(name); 
		while (la.kind == 23) {
			lexer.NextToken();
			EventMemberSpecifier(
#line  1582 "VBNET.ATG" 
out name);

#line  1582 "VBNET.ATG" 
			if (name != null) handlesClause.Add(name); 
		}
	}

	void Block(
#line  2809 "VBNET.ATG" 
out Statement stmt) {

#line  2812 "VBNET.ATG" 
		BlockStatement blockStmt = new BlockStatement();
		/* in snippet parsing mode, t might be null */
		if (t != null) blockStmt.StartLocation = t.EndLocation;
		BlockStart(blockStmt);
		
		while (StartOf(22) || 
#line  2818 "VBNET.ATG" 
IsEndStmtAhead()) {
			if (
#line  2818 "VBNET.ATG" 
IsEndStmtAhead()) {
				Expect(112);
				EndOfStmt();

#line  2818 "VBNET.ATG" 
				AddChild(new EndStatement()); 
			} else {
				Statement();
				EndOfStmt();
			}
		}

#line  2823 "VBNET.ATG" 
		stmt = blockStmt;
		if (t != null) blockStmt.EndLocation = t.EndLocation;
		BlockEnd();
		
	}

	void Charset(
#line  1568 "VBNET.ATG" 
out CharsetModifier charsetModifier) {

#line  1569 "VBNET.ATG" 
		charsetModifier = CharsetModifier.None; 
		if (la.kind == 126 || la.kind == 208) {
		} else if (la.kind == 61) {
			lexer.NextToken();

#line  1570 "VBNET.ATG" 
			charsetModifier = CharsetModifier.Ansi; 
		} else if (la.kind == 65) {
			lexer.NextToken();

#line  1571 "VBNET.ATG" 
			charsetModifier = CharsetModifier.Auto; 
		} else if (la.kind == 221) {
			lexer.NextToken();

#line  1572 "VBNET.ATG" 
			charsetModifier = CharsetModifier.Unicode; 
		} else SynErr(263);
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
		default: SynErr(264); break;
		}
	}

	void VariableDeclaratorPartAfterIdentifier(
#line  1453 "VBNET.ATG" 
List<VariableDeclaration> fieldDeclaration, string name) {

#line  1455 "VBNET.ATG" 
		Expression expr = null;
		TypeReference type = null;
		ArrayList rank = null;
		List<Expression> dimension = null;
		Location startLocation = t.Location;
		
		if (
#line  1461 "VBNET.ATG" 
IsSize() && !IsDims()) {
			ArrayInitializationModifier(
#line  1461 "VBNET.ATG" 
out dimension);
		}
		if (
#line  1462 "VBNET.ATG" 
IsDims()) {
			ArrayNameModifier(
#line  1462 "VBNET.ATG" 
out rank);
		}
		if (
#line  1464 "VBNET.ATG" 
IsObjectCreation()) {
			Expect(62);
			ObjectCreateExpression(
#line  1464 "VBNET.ATG" 
out expr);

#line  1466 "VBNET.ATG" 
			if (expr is ObjectCreateExpression) {
			type = ((ObjectCreateExpression)expr).CreateType.Clone();
			} else {
				type = ((ArrayCreateExpression)expr).CreateType.Clone();
			}
			
		} else if (StartOf(23)) {
			if (la.kind == 62) {
				lexer.NextToken();
				TypeName(
#line  1473 "VBNET.ATG" 
out type);

#line  1475 "VBNET.ATG" 
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

#line  1487 "VBNET.ATG" 
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
#line  1510 "VBNET.ATG" 
out expr);
			}
		} else SynErr(265);

#line  1513 "VBNET.ATG" 
		VariableDeclaration varDecl = new VariableDeclaration(name, expr, type);
		varDecl.StartLocation = startLocation;
		varDecl.EndLocation = t.Location;
		fieldDeclaration.Add(varDecl);
		
	}

	void VariableDeclarator(
#line  1447 "VBNET.ATG" 
List<VariableDeclaration> fieldDeclaration) {
		Identifier();

#line  1449 "VBNET.ATG" 
		string name = t.val; 
		VariableDeclaratorPartAfterIdentifier(
#line  1450 "VBNET.ATG" 
fieldDeclaration, name);
	}

	void ConstantDeclarator(
#line  1428 "VBNET.ATG" 
List<VariableDeclaration> constantDeclaration) {

#line  1430 "VBNET.ATG" 
		Expression expr = null;
		TypeReference type = null;
		string name = String.Empty;
		Location location;
		
		Identifier();

#line  1435 "VBNET.ATG" 
		name = t.val; location = t.Location; 
		if (la.kind == 62) {
			lexer.NextToken();
			TypeName(
#line  1436 "VBNET.ATG" 
out type);
		}
		Expect(21);
		Expr(
#line  1437 "VBNET.ATG" 
out expr);

#line  1439 "VBNET.ATG" 
		VariableDeclaration f = new VariableDeclaration(name, expr);
		f.TypeReference = type;
		f.StartLocation = location;
		constantDeclaration.Add(f);
		
	}

	void ObjectCreateExpression(
#line  1975 "VBNET.ATG" 
out Expression oce) {

#line  1977 "VBNET.ATG" 
		TypeReference type = null;
		CollectionInitializerExpression initializer = null;
		List<Expression> arguments = null;
		ArrayList dimensions = null;
		oce = null;
		bool canBeNormal; bool canBeReDim;
		
		Expect(161);
		if (StartOf(8)) {
			NonArrayTypeName(
#line  1985 "VBNET.ATG" 
out type, false);
			if (la.kind == 36) {
				lexer.NextToken();
				NormalOrReDimArgumentList(
#line  1986 "VBNET.ATG" 
out arguments, out canBeNormal, out canBeReDim);
				Expect(37);
				if (la.kind == 34 || 
#line  1987 "VBNET.ATG" 
la.kind == Tokens.OpenParenthesis) {
					if (
#line  1987 "VBNET.ATG" 
la.kind == Tokens.OpenParenthesis) {
						ArrayTypeModifiers(
#line  1988 "VBNET.ATG" 
out dimensions);
						CollectionInitializer(
#line  1989 "VBNET.ATG" 
out initializer);
					} else {
						CollectionInitializer(
#line  1990 "VBNET.ATG" 
out initializer);
					}
				}

#line  1992 "VBNET.ATG" 
				if (canBeReDim && !canBeNormal && initializer == null) initializer = new CollectionInitializerExpression(); 
			}
		}

#line  1996 "VBNET.ATG" 
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

#line  2011 "VBNET.ATG" 
				MemberInitializerExpression memberInitializer = null;
				
				lexer.NextToken();

#line  2015 "VBNET.ATG" 
				CollectionInitializerExpression memberInitializers = new CollectionInitializerExpression();
				memberInitializers.StartLocation = la.Location;
				
				Expect(34);
				MemberInitializer(
#line  2019 "VBNET.ATG" 
out memberInitializer);

#line  2020 "VBNET.ATG" 
				memberInitializers.CreateExpressions.Add(memberInitializer); 
				while (la.kind == 23) {
					lexer.NextToken();
					MemberInitializer(
#line  2022 "VBNET.ATG" 
out memberInitializer);

#line  2023 "VBNET.ATG" 
					memberInitializers.CreateExpressions.Add(memberInitializer); 
				}
				Expect(35);

#line  2027 "VBNET.ATG" 
				memberInitializers.EndLocation = t.Location;
				if(oce is ObjectCreateExpression)
				{
					((ObjectCreateExpression)oce).ObjectInitializer = memberInitializers;
				}
				
			} else {
				lexer.NextToken();
				CollectionInitializer(
#line  2037 "VBNET.ATG" 
out initializer);

#line  2039 "VBNET.ATG" 
				if(oce is ObjectCreateExpression)
				((ObjectCreateExpression)oce).ObjectInitializer = initializer;
				
			}
		}
	}

	void AccessorDecls(
#line  1362 "VBNET.ATG" 
out PropertyGetRegion getBlock, out PropertySetRegion setBlock) {

#line  1364 "VBNET.ATG" 
		List<AttributeSection> attributes = new List<AttributeSection>();
		AttributeSection section;
		getBlock = null;
		setBlock = null; 
		
		while (la.kind == 39) {
			AttributeSection(
#line  1369 "VBNET.ATG" 
out section);

#line  1369 "VBNET.ATG" 
			attributes.Add(section); 
		}
		if (StartOf(24)) {
			GetAccessorDecl(
#line  1371 "VBNET.ATG" 
out getBlock, attributes);
			if (StartOf(25)) {

#line  1373 "VBNET.ATG" 
				attributes = new List<AttributeSection>(); 
				while (la.kind == 39) {
					AttributeSection(
#line  1374 "VBNET.ATG" 
out section);

#line  1374 "VBNET.ATG" 
					attributes.Add(section); 
				}
				SetAccessorDecl(
#line  1375 "VBNET.ATG" 
out setBlock, attributes);
			}
		} else if (StartOf(26)) {
			SetAccessorDecl(
#line  1378 "VBNET.ATG" 
out setBlock, attributes);
			if (StartOf(27)) {

#line  1380 "VBNET.ATG" 
				attributes = new List<AttributeSection>(); 
				while (la.kind == 39) {
					AttributeSection(
#line  1381 "VBNET.ATG" 
out section);

#line  1381 "VBNET.ATG" 
					attributes.Add(section); 
				}
				GetAccessorDecl(
#line  1382 "VBNET.ATG" 
out getBlock, attributes);
			}
		} else SynErr(266);
	}

	void EventAccessorDeclaration(
#line  1325 "VBNET.ATG" 
out EventAddRemoveRegion eventAccessorDeclaration) {

#line  1327 "VBNET.ATG" 
		Statement stmt = null;
		List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		eventAccessorDeclaration = null;
		
		while (la.kind == 39) {
			AttributeSection(
#line  1333 "VBNET.ATG" 
out section);

#line  1333 "VBNET.ATG" 
			attributes.Add(section); 
		}
		if (la.kind == 55) {
			lexer.NextToken();
			if (la.kind == 36) {
				lexer.NextToken();
				if (StartOf(6)) {
					FormalParameterList(
#line  1335 "VBNET.ATG" 
p);
				}
				Expect(37);
			}
			Expect(1);
			Block(
#line  1336 "VBNET.ATG" 
out stmt);
			Expect(112);
			Expect(55);
			EndOfStmt();

#line  1338 "VBNET.ATG" 
			eventAccessorDeclaration = new EventAddRegion(attributes);
			eventAccessorDeclaration.Block = (BlockStatement)stmt;
			eventAccessorDeclaration.Parameters = p;
			
		} else if (la.kind == 191) {
			lexer.NextToken();
			if (la.kind == 36) {
				lexer.NextToken();
				if (StartOf(6)) {
					FormalParameterList(
#line  1343 "VBNET.ATG" 
p);
				}
				Expect(37);
			}
			Expect(1);
			Block(
#line  1344 "VBNET.ATG" 
out stmt);
			Expect(112);
			Expect(191);
			EndOfStmt();

#line  1346 "VBNET.ATG" 
			eventAccessorDeclaration = new EventRemoveRegion(attributes);
			eventAccessorDeclaration.Block = (BlockStatement)stmt;
			eventAccessorDeclaration.Parameters = p;
			
		} else if (la.kind == 187) {
			lexer.NextToken();
			if (la.kind == 36) {
				lexer.NextToken();
				if (StartOf(6)) {
					FormalParameterList(
#line  1351 "VBNET.ATG" 
p);
				}
				Expect(37);
			}
			Expect(1);
			Block(
#line  1352 "VBNET.ATG" 
out stmt);
			Expect(112);
			Expect(187);
			EndOfStmt();

#line  1354 "VBNET.ATG" 
			eventAccessorDeclaration = new EventRaiseRegion(attributes);
			eventAccessorDeclaration.Block = (BlockStatement)stmt;
			eventAccessorDeclaration.Parameters = p;
			
		} else SynErr(267);
	}

	void OverloadableOperator(
#line  1267 "VBNET.ATG" 
out OverloadableOperatorType operatorType) {

#line  1268 "VBNET.ATG" 
		operatorType = OverloadableOperatorType.None; 
		switch (la.kind) {
		case 30: {
			lexer.NextToken();

#line  1270 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Add; 
			break;
		}
		case 29: {
			lexer.NextToken();

#line  1272 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Subtract; 
			break;
		}
		case 33: {
			lexer.NextToken();

#line  1274 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Multiply; 
			break;
		}
		case 25: {
			lexer.NextToken();

#line  1276 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Divide; 
			break;
		}
		case 26: {
			lexer.NextToken();

#line  1278 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.DivideInteger; 
			break;
		}
		case 24: {
			lexer.NextToken();

#line  1280 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Concat; 
			break;
		}
		case 149: {
			lexer.NextToken();

#line  1282 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Like; 
			break;
		}
		case 153: {
			lexer.NextToken();

#line  1284 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Modulus; 
			break;
		}
		case 59: {
			lexer.NextToken();

#line  1286 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.BitwiseAnd; 
			break;
		}
		case 174: {
			lexer.NextToken();

#line  1288 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.BitwiseOr; 
			break;
		}
		case 234: {
			lexer.NextToken();

#line  1290 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.ExclusiveOr; 
			break;
		}
		case 31: {
			lexer.NextToken();

#line  1292 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Power; 
			break;
		}
		case 43: {
			lexer.NextToken();

#line  1294 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.ShiftLeft; 
			break;
		}
		case 44: {
			lexer.NextToken();

#line  1296 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.ShiftRight; 
			break;
		}
		case 21: {
			lexer.NextToken();

#line  1298 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Equality; 
			break;
		}
		case 40: {
			lexer.NextToken();

#line  1300 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.InEquality; 
			break;
		}
		case 39: {
			lexer.NextToken();

#line  1302 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.LessThan; 
			break;
		}
		case 42: {
			lexer.NextToken();

#line  1304 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.LessThanOrEqual; 
			break;
		}
		case 38: {
			lexer.NextToken();

#line  1306 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.GreaterThan; 
			break;
		}
		case 41: {
			lexer.NextToken();

#line  1308 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.GreaterThanOrEqual; 
			break;
		}
		case 93: {
			lexer.NextToken();

#line  1310 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.CType; 
			break;
		}
		case 2: case 57: case 61: case 63: case 64: case 65: case 66: case 69: case 86: case 97: case 103: case 106: case 115: case 120: case 125: case 132: case 138: case 142: case 145: case 169: case 175: case 182: case 201: case 210: case 211: case 221: case 222: case 228: {
			Identifier();

#line  1314 "VBNET.ATG" 
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
		default: SynErr(268); break;
		}
	}

	void GetAccessorDecl(
#line  1388 "VBNET.ATG" 
out PropertyGetRegion getBlock, List<AttributeSection> attributes) {

#line  1389 "VBNET.ATG" 
		Statement stmt = null; Modifiers m; 
		PropertyAccessorAccessModifier(
#line  1391 "VBNET.ATG" 
out m);
		Expect(127);

#line  1393 "VBNET.ATG" 
		Location startLocation = t.Location; 
		Expect(1);
		Block(
#line  1395 "VBNET.ATG" 
out stmt);

#line  1396 "VBNET.ATG" 
		getBlock = new PropertyGetRegion((BlockStatement)stmt, attributes); 
		Expect(112);
		Expect(127);

#line  1398 "VBNET.ATG" 
		getBlock.Modifier = m; 

#line  1399 "VBNET.ATG" 
		getBlock.StartLocation = startLocation; getBlock.EndLocation = t.EndLocation; 
		EndOfStmt();
	}

	void SetAccessorDecl(
#line  1404 "VBNET.ATG" 
out PropertySetRegion setBlock, List<AttributeSection> attributes) {

#line  1406 "VBNET.ATG" 
		Statement stmt = null;
		List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
		Modifiers m;
		
		PropertyAccessorAccessModifier(
#line  1411 "VBNET.ATG" 
out m);
		Expect(196);

#line  1413 "VBNET.ATG" 
		Location startLocation = t.Location; 
		if (la.kind == 36) {
			lexer.NextToken();
			if (StartOf(6)) {
				FormalParameterList(
#line  1414 "VBNET.ATG" 
p);
			}
			Expect(37);
		}
		Expect(1);
		Block(
#line  1416 "VBNET.ATG" 
out stmt);

#line  1418 "VBNET.ATG" 
		setBlock = new PropertySetRegion((BlockStatement)stmt, attributes);
		setBlock.Modifier = m;
		setBlock.Parameters = p;
		
		Expect(112);
		Expect(196);

#line  1423 "VBNET.ATG" 
		setBlock.StartLocation = startLocation; setBlock.EndLocation = t.EndLocation; 
		EndOfStmt();
	}

	void PropertyAccessorAccessModifier(
#line  3525 "VBNET.ATG" 
out Modifiers m) {

#line  3526 "VBNET.ATG" 
		m = Modifiers.None; 
		while (StartOf(28)) {
			if (la.kind == 186) {
				lexer.NextToken();

#line  3528 "VBNET.ATG" 
				m |= Modifiers.Public; 
			} else if (la.kind == 185) {
				lexer.NextToken();

#line  3529 "VBNET.ATG" 
				m |= Modifiers.Protected; 
			} else if (la.kind == 124) {
				lexer.NextToken();

#line  3530 "VBNET.ATG" 
				m |= Modifiers.Internal; 
			} else {
				lexer.NextToken();

#line  3531 "VBNET.ATG" 
				m |= Modifiers.Private; 
			}
		}
	}

	void ArrayInitializationModifier(
#line  1521 "VBNET.ATG" 
out List<Expression> arrayModifiers) {

#line  1523 "VBNET.ATG" 
		arrayModifiers = null;
		
		Expect(36);
		InitializationRankList(
#line  1525 "VBNET.ATG" 
out arrayModifiers);
		Expect(37);
	}

	void ArrayNameModifier(
#line  2602 "VBNET.ATG" 
out ArrayList arrayModifiers) {

#line  2604 "VBNET.ATG" 
		arrayModifiers = null;
		
		ArrayTypeModifiers(
#line  2606 "VBNET.ATG" 
out arrayModifiers);
	}

	void InitializationRankList(
#line  1529 "VBNET.ATG" 
out List<Expression> rank) {

#line  1531 "VBNET.ATG" 
		rank = new List<Expression>();
		Expression expr = null;
		
		Expr(
#line  1534 "VBNET.ATG" 
out expr);
		if (la.kind == 214) {
			lexer.NextToken();

#line  1535 "VBNET.ATG" 
			EnsureIsZero(expr); 
			Expr(
#line  1536 "VBNET.ATG" 
out expr);
		}

#line  1538 "VBNET.ATG" 
		if (expr != null) { rank.Add(expr); } 
		while (la.kind == 23) {
			lexer.NextToken();
			Expr(
#line  1540 "VBNET.ATG" 
out expr);
			if (la.kind == 214) {
				lexer.NextToken();

#line  1541 "VBNET.ATG" 
				EnsureIsZero(expr); 
				Expr(
#line  1542 "VBNET.ATG" 
out expr);
			}

#line  1544 "VBNET.ATG" 
			if (expr != null) { rank.Add(expr); } 
		}
	}

	void CollectionInitializer(
#line  1549 "VBNET.ATG" 
out CollectionInitializerExpression outExpr) {

#line  1551 "VBNET.ATG" 
		Expression expr = null;
		CollectionInitializerExpression initializer = new CollectionInitializerExpression();
		
		Expect(34);
		if (StartOf(29)) {
			Expr(
#line  1556 "VBNET.ATG" 
out expr);

#line  1558 "VBNET.ATG" 
			if (expr != null) { initializer.CreateExpressions.Add(expr); }
			
			while (
#line  1561 "VBNET.ATG" 
NotFinalComma()) {
				Expect(23);
				Expr(
#line  1561 "VBNET.ATG" 
out expr);

#line  1562 "VBNET.ATG" 
				if (expr != null) { initializer.CreateExpressions.Add(expr); } 
			}
		}
		Expect(35);

#line  1565 "VBNET.ATG" 
		outExpr = initializer; 
	}

	void EventMemberSpecifier(
#line  1635 "VBNET.ATG" 
out string name) {

#line  1636 "VBNET.ATG" 
		string eventName; 
		if (StartOf(4)) {
			Identifier();
		} else if (la.kind == 157) {
			lexer.NextToken();
		} else if (la.kind == 152) {
			lexer.NextToken();
		} else SynErr(269);

#line  1639 "VBNET.ATG" 
		name = t.val; 
		Expect(27);
		IdentifierOrKeyword(
#line  1641 "VBNET.ATG" 
out eventName);

#line  1642 "VBNET.ATG" 
		name = name + "." + eventName; 
	}

	void IdentifierOrKeyword(
#line  3458 "VBNET.ATG" 
out string name) {
		lexer.NextToken();

#line  3460 "VBNET.ATG" 
		name = t.val;  
	}

	void QueryExpr(
#line  2126 "VBNET.ATG" 
out Expression expr) {

#line  2128 "VBNET.ATG" 
		QueryExpressionVB qexpr = new QueryExpressionVB();
		qexpr.StartLocation = la.Location;
		expr = qexpr;
		
		FromOrAggregateQueryOperator(
#line  2132 "VBNET.ATG" 
qexpr.Clauses);
		while (StartOf(30)) {
			QueryOperator(
#line  2133 "VBNET.ATG" 
qexpr.Clauses);
		}

#line  2135 "VBNET.ATG" 
		qexpr.EndLocation = t.EndLocation;
		
	}

	void LambdaExpr(
#line  2046 "VBNET.ATG" 
out Expression expr) {

#line  2048 "VBNET.ATG" 
		LambdaExpression lambda = null;
		
		if (la.kind == 208) {
			SubLambdaExpression(
#line  2050 "VBNET.ATG" 
out lambda);
		} else if (la.kind == 126) {
			FunctionLambdaExpression(
#line  2051 "VBNET.ATG" 
out lambda);
		} else SynErr(270);

#line  2052 "VBNET.ATG" 
		expr = lambda; 
	}

	void DisjunctionExpr(
#line  1819 "VBNET.ATG" 
out Expression outExpr) {

#line  1821 "VBNET.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		ConjunctionExpr(
#line  1824 "VBNET.ATG" 
out outExpr);
		while (la.kind == 174 || la.kind == 176 || la.kind == 234) {
			if (la.kind == 174) {
				lexer.NextToken();

#line  1827 "VBNET.ATG" 
				op = BinaryOperatorType.BitwiseOr; 
			} else if (la.kind == 176) {
				lexer.NextToken();

#line  1828 "VBNET.ATG" 
				op = BinaryOperatorType.LogicalOr; 
			} else {
				lexer.NextToken();

#line  1829 "VBNET.ATG" 
				op = BinaryOperatorType.ExclusiveOr; 
			}
			ConjunctionExpr(
#line  1831 "VBNET.ATG" 
out expr);

#line  1831 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr);  
		}
	}

	void AssignmentOperator(
#line  1653 "VBNET.ATG" 
out AssignmentOperatorType op) {

#line  1654 "VBNET.ATG" 
		op = AssignmentOperatorType.None; 
		switch (la.kind) {
		case 21: {
			lexer.NextToken();

#line  1655 "VBNET.ATG" 
			op = AssignmentOperatorType.Assign; 
			break;
		}
		case 53: {
			lexer.NextToken();

#line  1656 "VBNET.ATG" 
			op = AssignmentOperatorType.ConcatString; 
			break;
		}
		case 45: {
			lexer.NextToken();

#line  1657 "VBNET.ATG" 
			op = AssignmentOperatorType.Add; 
			break;
		}
		case 47: {
			lexer.NextToken();

#line  1658 "VBNET.ATG" 
			op = AssignmentOperatorType.Subtract; 
			break;
		}
		case 48: {
			lexer.NextToken();

#line  1659 "VBNET.ATG" 
			op = AssignmentOperatorType.Multiply; 
			break;
		}
		case 49: {
			lexer.NextToken();

#line  1660 "VBNET.ATG" 
			op = AssignmentOperatorType.Divide; 
			break;
		}
		case 50: {
			lexer.NextToken();

#line  1661 "VBNET.ATG" 
			op = AssignmentOperatorType.DivideInteger; 
			break;
		}
		case 46: {
			lexer.NextToken();

#line  1662 "VBNET.ATG" 
			op = AssignmentOperatorType.Power; 
			break;
		}
		case 51: {
			lexer.NextToken();

#line  1663 "VBNET.ATG" 
			op = AssignmentOperatorType.ShiftLeft; 
			break;
		}
		case 52: {
			lexer.NextToken();

#line  1664 "VBNET.ATG" 
			op = AssignmentOperatorType.ShiftRight; 
			break;
		}
		default: SynErr(271); break;
		}
	}

	void SimpleExpr(
#line  1668 "VBNET.ATG" 
out Expression pexpr) {

#line  1669 "VBNET.ATG" 
		string name; 
		SimpleNonInvocationExpression(
#line  1671 "VBNET.ATG" 
out pexpr);
		while (la.kind == 27 || la.kind == 28 || la.kind == 36) {
			if (la.kind == 27) {
				lexer.NextToken();
				IdentifierOrKeyword(
#line  1673 "VBNET.ATG" 
out name);

#line  1674 "VBNET.ATG" 
				pexpr = new MemberReferenceExpression(pexpr, name); 
				if (
#line  1675 "VBNET.ATG" 
la.kind == Tokens.OpenParenthesis && Peek(1).kind == Tokens.Of) {
					lexer.NextToken();
					Expect(168);
					TypeArgumentList(
#line  1676 "VBNET.ATG" 
((MemberReferenceExpression)pexpr).TypeArguments);
					Expect(37);
				}
			} else if (la.kind == 28) {
				lexer.NextToken();
				IdentifierOrKeyword(
#line  1678 "VBNET.ATG" 
out name);

#line  1678 "VBNET.ATG" 
				pexpr = new BinaryOperatorExpression(pexpr, BinaryOperatorType.DictionaryAccess, new PrimitiveExpression(name, name)); 
			} else {
				InvocationExpression(
#line  1679 "VBNET.ATG" 
ref pexpr);
			}
		}
	}

	void SimpleNonInvocationExpression(
#line  1683 "VBNET.ATG" 
out Expression pexpr) {

#line  1685 "VBNET.ATG" 
		Expression expr;
		CollectionInitializerExpression cie;
		TypeReference type = null;
		string name = String.Empty;
		pexpr = null;
		
		if (StartOf(31)) {
			switch (la.kind) {
			case 3: {
				lexer.NextToken();

#line  1694 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(t.literalValue, t.val) { LiteralFormat = t.literalFormat };  
				break;
			}
			case 4: {
				lexer.NextToken();

#line  1695 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(t.literalValue, t.val) { LiteralFormat = t.literalFormat };  
				break;
			}
			case 7: {
				lexer.NextToken();

#line  1696 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(t.literalValue, t.val) { LiteralFormat = t.literalFormat };  
				break;
			}
			case 6: {
				lexer.NextToken();

#line  1697 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(t.literalValue, t.val) { LiteralFormat = t.literalFormat };  
				break;
			}
			case 5: {
				lexer.NextToken();

#line  1698 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(t.literalValue, t.val) { LiteralFormat = t.literalFormat };  
				break;
			}
			case 9: {
				lexer.NextToken();

#line  1699 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(t.literalValue, t.val) { LiteralFormat = t.literalFormat };  
				break;
			}
			case 8: {
				lexer.NextToken();

#line  1700 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(t.literalValue, t.val) { LiteralFormat = t.literalFormat };  
				break;
			}
			case 215: {
				lexer.NextToken();

#line  1702 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(true, "true");  
				break;
			}
			case 121: {
				lexer.NextToken();

#line  1703 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(false, "false"); 
				break;
			}
			case 164: {
				lexer.NextToken();

#line  1704 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(null, "null");  
				break;
			}
			case 36: {
				lexer.NextToken();
				Expr(
#line  1705 "VBNET.ATG" 
out expr);
				Expect(37);

#line  1705 "VBNET.ATG" 
				pexpr = new ParenthesizedExpression(expr); 
				break;
			}
			case 2: case 57: case 61: case 63: case 64: case 65: case 66: case 69: case 86: case 97: case 103: case 106: case 115: case 120: case 125: case 132: case 138: case 142: case 145: case 169: case 175: case 182: case 201: case 210: case 211: case 221: case 222: case 228: {
				Identifier();

#line  1707 "VBNET.ATG" 
				pexpr = new IdentifierExpression(t.val);
				pexpr.StartLocation = t.Location; pexpr.EndLocation = t.EndLocation;
				
				if (
#line  1710 "VBNET.ATG" 
la.kind == Tokens.OpenParenthesis && Peek(1).kind == Tokens.Of) {
					lexer.NextToken();
					Expect(168);
					TypeArgumentList(
#line  1711 "VBNET.ATG" 
((IdentifierExpression)pexpr).TypeArguments);
					Expect(37);
				}
				break;
			}
			case 67: case 70: case 81: case 98: case 99: case 108: case 140: case 150: case 167: case 194: case 199: case 200: case 206: case 219: case 220: case 223: {

#line  1713 "VBNET.ATG" 
				string val = String.Empty; 
				if (StartOf(12)) {
					PrimitiveTypeName(
#line  1714 "VBNET.ATG" 
out val);
				} else if (la.kind == 167) {
					lexer.NextToken();

#line  1714 "VBNET.ATG" 
					val = "System.Object"; 
				} else SynErr(272);

#line  1715 "VBNET.ATG" 
				pexpr = new TypeReferenceExpression(new TypeReference(val, true)); 
				break;
			}
			case 152: {
				lexer.NextToken();

#line  1716 "VBNET.ATG" 
				pexpr = new ThisReferenceExpression(); 
				break;
			}
			case 157: case 158: {

#line  1717 "VBNET.ATG" 
				Expression retExpr = null; 
				if (la.kind == 157) {
					lexer.NextToken();

#line  1718 "VBNET.ATG" 
					retExpr = new BaseReferenceExpression(); 
				} else if (la.kind == 158) {
					lexer.NextToken();

#line  1719 "VBNET.ATG" 
					retExpr = new ClassReferenceExpression(); 
				} else SynErr(273);
				Expect(27);
				IdentifierOrKeyword(
#line  1721 "VBNET.ATG" 
out name);

#line  1721 "VBNET.ATG" 
				pexpr = new MemberReferenceExpression(retExpr, name); 
				break;
			}
			case 129: {
				lexer.NextToken();
				Expect(27);
				Identifier();

#line  1723 "VBNET.ATG" 
				type = new TypeReference(t.val ?? ""); 

#line  1725 "VBNET.ATG" 
				type.IsGlobal = true; 

#line  1726 "VBNET.ATG" 
				pexpr = new TypeReferenceExpression(type); 
				break;
			}
			case 161: {
				ObjectCreateExpression(
#line  1727 "VBNET.ATG" 
out expr);

#line  1727 "VBNET.ATG" 
				pexpr = expr; 
				break;
			}
			case 34: {
				CollectionInitializer(
#line  1728 "VBNET.ATG" 
out cie);

#line  1728 "VBNET.ATG" 
				pexpr = cie; 
				break;
			}
			case 93: case 105: case 217: {

#line  1730 "VBNET.ATG" 
				CastType castType = CastType.Cast; 
				if (la.kind == 105) {
					lexer.NextToken();
				} else if (la.kind == 93) {
					lexer.NextToken();

#line  1732 "VBNET.ATG" 
					castType = CastType.Conversion; 
				} else if (la.kind == 217) {
					lexer.NextToken();

#line  1733 "VBNET.ATG" 
					castType = CastType.TryCast; 
				} else SynErr(274);
				Expect(36);
				Expr(
#line  1735 "VBNET.ATG" 
out expr);
				Expect(23);
				TypeName(
#line  1735 "VBNET.ATG" 
out type);
				Expect(37);

#line  1736 "VBNET.ATG" 
				pexpr = new CastExpression(type, expr, castType); 
				break;
			}
			case 75: case 76: case 77: case 78: case 79: case 80: case 82: case 84: case 85: case 89: case 90: case 91: case 92: case 94: case 95: case 96: {
				CastTarget(
#line  1737 "VBNET.ATG" 
out type);
				Expect(36);
				Expr(
#line  1737 "VBNET.ATG" 
out expr);
				Expect(37);

#line  1737 "VBNET.ATG" 
				pexpr = new CastExpression(type, expr, CastType.PrimitiveConversion); 
				break;
			}
			case 56: {
				lexer.NextToken();
				Expr(
#line  1738 "VBNET.ATG" 
out expr);

#line  1738 "VBNET.ATG" 
				pexpr = new AddressOfExpression(expr); 
				break;
			}
			case 128: {
				lexer.NextToken();
				Expect(36);
				GetTypeTypeName(
#line  1739 "VBNET.ATG" 
out type);
				Expect(37);

#line  1739 "VBNET.ATG" 
				pexpr = new TypeOfExpression(type); 
				break;
			}
			case 218: {
				lexer.NextToken();
				SimpleExpr(
#line  1740 "VBNET.ATG" 
out expr);
				Expect(143);
				TypeName(
#line  1740 "VBNET.ATG" 
out type);

#line  1740 "VBNET.ATG" 
				pexpr = new TypeOfIsExpression(expr, type); 
				break;
			}
			case 134: {
				ConditionalExpression(
#line  1741 "VBNET.ATG" 
out pexpr);
				break;
			}
			}
		} else if (la.kind == 27) {
			lexer.NextToken();
			IdentifierOrKeyword(
#line  1745 "VBNET.ATG" 
out name);

#line  1745 "VBNET.ATG" 
			pexpr = new MemberReferenceExpression(null, name);
		} else SynErr(275);
	}

	void TypeArgumentList(
#line  2638 "VBNET.ATG" 
List<TypeReference> typeArguments) {

#line  2640 "VBNET.ATG" 
		TypeReference typeref;
		
		TypeName(
#line  2642 "VBNET.ATG" 
out typeref);

#line  2642 "VBNET.ATG" 
		if (typeref != null) typeArguments.Add(typeref); 
		while (la.kind == 23) {
			lexer.NextToken();
			TypeName(
#line  2645 "VBNET.ATG" 
out typeref);

#line  2645 "VBNET.ATG" 
			if (typeref != null) typeArguments.Add(typeref); 
		}
	}

	void InvocationExpression(
#line  1783 "VBNET.ATG" 
ref Expression pexpr) {

#line  1784 "VBNET.ATG" 
		List<Expression> parameters = null; 
		Expect(36);

#line  1786 "VBNET.ATG" 
		Location start = t.Location; 
		ArgumentList(
#line  1787 "VBNET.ATG" 
out parameters);
		Expect(37);

#line  1790 "VBNET.ATG" 
		pexpr = new InvocationExpression(pexpr, parameters);
		

#line  1792 "VBNET.ATG" 
		pexpr.StartLocation = start; pexpr.EndLocation = t.Location; 
	}

	void PrimitiveTypeName(
#line  3465 "VBNET.ATG" 
out string type) {

#line  3466 "VBNET.ATG" 
		type = String.Empty; 
		switch (la.kind) {
		case 67: {
			lexer.NextToken();

#line  3467 "VBNET.ATG" 
			type = "System.Boolean"; 
			break;
		}
		case 98: {
			lexer.NextToken();

#line  3468 "VBNET.ATG" 
			type = "System.DateTime"; 
			break;
		}
		case 81: {
			lexer.NextToken();

#line  3469 "VBNET.ATG" 
			type = "System.Char"; 
			break;
		}
		case 206: {
			lexer.NextToken();

#line  3470 "VBNET.ATG" 
			type = "System.String"; 
			break;
		}
		case 99: {
			lexer.NextToken();

#line  3471 "VBNET.ATG" 
			type = "System.Decimal"; 
			break;
		}
		case 70: {
			lexer.NextToken();

#line  3472 "VBNET.ATG" 
			type = "System.Byte"; 
			break;
		}
		case 199: {
			lexer.NextToken();

#line  3473 "VBNET.ATG" 
			type = "System.Int16"; 
			break;
		}
		case 140: {
			lexer.NextToken();

#line  3474 "VBNET.ATG" 
			type = "System.Int32"; 
			break;
		}
		case 150: {
			lexer.NextToken();

#line  3475 "VBNET.ATG" 
			type = "System.Int64"; 
			break;
		}
		case 200: {
			lexer.NextToken();

#line  3476 "VBNET.ATG" 
			type = "System.Single"; 
			break;
		}
		case 108: {
			lexer.NextToken();

#line  3477 "VBNET.ATG" 
			type = "System.Double"; 
			break;
		}
		case 219: {
			lexer.NextToken();

#line  3478 "VBNET.ATG" 
			type = "System.UInt32"; 
			break;
		}
		case 220: {
			lexer.NextToken();

#line  3479 "VBNET.ATG" 
			type = "System.UInt64"; 
			break;
		}
		case 223: {
			lexer.NextToken();

#line  3480 "VBNET.ATG" 
			type = "System.UInt16"; 
			break;
		}
		case 194: {
			lexer.NextToken();

#line  3481 "VBNET.ATG" 
			type = "System.SByte"; 
			break;
		}
		default: SynErr(276); break;
		}
	}

	void CastTarget(
#line  1797 "VBNET.ATG" 
out TypeReference type) {

#line  1799 "VBNET.ATG" 
		type = null;
		
		switch (la.kind) {
		case 75: {
			lexer.NextToken();

#line  1801 "VBNET.ATG" 
			type = new TypeReference("System.Boolean", true); 
			break;
		}
		case 76: {
			lexer.NextToken();

#line  1802 "VBNET.ATG" 
			type = new TypeReference("System.Byte", true); 
			break;
		}
		case 89: {
			lexer.NextToken();

#line  1803 "VBNET.ATG" 
			type = new TypeReference("System.SByte", true); 
			break;
		}
		case 77: {
			lexer.NextToken();

#line  1804 "VBNET.ATG" 
			type = new TypeReference("System.Char", true); 
			break;
		}
		case 78: {
			lexer.NextToken();

#line  1805 "VBNET.ATG" 
			type = new TypeReference("System.DateTime", true); 
			break;
		}
		case 80: {
			lexer.NextToken();

#line  1806 "VBNET.ATG" 
			type = new TypeReference("System.Decimal", true); 
			break;
		}
		case 79: {
			lexer.NextToken();

#line  1807 "VBNET.ATG" 
			type = new TypeReference("System.Double", true); 
			break;
		}
		case 90: {
			lexer.NextToken();

#line  1808 "VBNET.ATG" 
			type = new TypeReference("System.Int16", true); 
			break;
		}
		case 82: {
			lexer.NextToken();

#line  1809 "VBNET.ATG" 
			type = new TypeReference("System.Int32", true); 
			break;
		}
		case 84: {
			lexer.NextToken();

#line  1810 "VBNET.ATG" 
			type = new TypeReference("System.Int64", true); 
			break;
		}
		case 96: {
			lexer.NextToken();

#line  1811 "VBNET.ATG" 
			type = new TypeReference("System.UInt16", true); 
			break;
		}
		case 94: {
			lexer.NextToken();

#line  1812 "VBNET.ATG" 
			type = new TypeReference("System.UInt32", true); 
			break;
		}
		case 95: {
			lexer.NextToken();

#line  1813 "VBNET.ATG" 
			type = new TypeReference("System.UInt64", true); 
			break;
		}
		case 85: {
			lexer.NextToken();

#line  1814 "VBNET.ATG" 
			type = new TypeReference("System.Object", true); 
			break;
		}
		case 91: {
			lexer.NextToken();

#line  1815 "VBNET.ATG" 
			type = new TypeReference("System.Single", true); 
			break;
		}
		case 92: {
			lexer.NextToken();

#line  1816 "VBNET.ATG" 
			type = new TypeReference("System.String", true); 
			break;
		}
		default: SynErr(277); break;
		}
	}

	void GetTypeTypeName(
#line  2537 "VBNET.ATG" 
out TypeReference typeref) {

#line  2538 "VBNET.ATG" 
		ArrayList rank = null; 
		NonArrayTypeName(
#line  2540 "VBNET.ATG" 
out typeref, true);
		ArrayTypeModifiers(
#line  2541 "VBNET.ATG" 
out rank);

#line  2542 "VBNET.ATG" 
		if (rank != null && typeref != null) {
		typeref.RankSpecifier = (int[])rank.ToArray(typeof(int));
		}
		
	}

	void ConditionalExpression(
#line  1749 "VBNET.ATG" 
out Expression expr) {

#line  1751 "VBNET.ATG" 
		ConditionalExpression conditionalExpression = new ConditionalExpression();
		BinaryOperatorExpression binaryOperatorExpression = new BinaryOperatorExpression();
		conditionalExpression.StartLocation = binaryOperatorExpression.StartLocation = la.Location;
		
		Expression condition = null;
		Expression trueExpr = null;
		Expression falseExpr = null;
		
		Expect(134);
		Expect(36);
		Expr(
#line  1760 "VBNET.ATG" 
out condition);
		Expect(23);
		Expr(
#line  1760 "VBNET.ATG" 
out trueExpr);
		if (la.kind == 23) {
			lexer.NextToken();
			Expr(
#line  1760 "VBNET.ATG" 
out falseExpr);
		}
		Expect(37);

#line  1762 "VBNET.ATG" 
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
#line  2469 "VBNET.ATG" 
out List<Expression> arguments) {

#line  2471 "VBNET.ATG" 
		arguments = new List<Expression>();
		Expression expr = null;
		
		if (StartOf(29)) {
			Argument(
#line  2474 "VBNET.ATG" 
out expr);
		}
		while (la.kind == 23) {
			lexer.NextToken();

#line  2475 "VBNET.ATG" 
			arguments.Add(expr ?? Expression.Null); expr = null; 
			if (StartOf(29)) {
				Argument(
#line  2476 "VBNET.ATG" 
out expr);
			}

#line  2477 "VBNET.ATG" 
			if (expr == null) expr = Expression.Null; 
		}

#line  2479 "VBNET.ATG" 
		if (expr != null) arguments.Add(expr); 
	}

	void ConjunctionExpr(
#line  1835 "VBNET.ATG" 
out Expression outExpr) {

#line  1837 "VBNET.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		NotExpr(
#line  1840 "VBNET.ATG" 
out outExpr);
		while (la.kind == 59 || la.kind == 60) {
			if (la.kind == 59) {
				lexer.NextToken();

#line  1843 "VBNET.ATG" 
				op = BinaryOperatorType.BitwiseAnd; 
			} else {
				lexer.NextToken();

#line  1844 "VBNET.ATG" 
				op = BinaryOperatorType.LogicalAnd; 
			}
			NotExpr(
#line  1846 "VBNET.ATG" 
out expr);

#line  1846 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr);  
		}
	}

	void NotExpr(
#line  1850 "VBNET.ATG" 
out Expression outExpr) {

#line  1851 "VBNET.ATG" 
		UnaryOperatorType uop = UnaryOperatorType.None; 
		while (la.kind == 163) {
			lexer.NextToken();

#line  1852 "VBNET.ATG" 
			uop = UnaryOperatorType.Not; 
		}
		ComparisonExpr(
#line  1853 "VBNET.ATG" 
out outExpr);

#line  1854 "VBNET.ATG" 
		if (uop != UnaryOperatorType.None)
		outExpr = new UnaryOperatorExpression(outExpr, uop);
		
	}

	void ComparisonExpr(
#line  1859 "VBNET.ATG" 
out Expression outExpr) {

#line  1861 "VBNET.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		ShiftExpr(
#line  1864 "VBNET.ATG" 
out outExpr);
		while (StartOf(32)) {
			switch (la.kind) {
			case 39: {
				lexer.NextToken();

#line  1867 "VBNET.ATG" 
				op = BinaryOperatorType.LessThan; 
				break;
			}
			case 38: {
				lexer.NextToken();

#line  1868 "VBNET.ATG" 
				op = BinaryOperatorType.GreaterThan; 
				break;
			}
			case 42: {
				lexer.NextToken();

#line  1869 "VBNET.ATG" 
				op = BinaryOperatorType.LessThanOrEqual; 
				break;
			}
			case 41: {
				lexer.NextToken();

#line  1870 "VBNET.ATG" 
				op = BinaryOperatorType.GreaterThanOrEqual; 
				break;
			}
			case 40: {
				lexer.NextToken();

#line  1871 "VBNET.ATG" 
				op = BinaryOperatorType.InEquality; 
				break;
			}
			case 21: {
				lexer.NextToken();

#line  1872 "VBNET.ATG" 
				op = BinaryOperatorType.Equality; 
				break;
			}
			case 149: {
				lexer.NextToken();

#line  1873 "VBNET.ATG" 
				op = BinaryOperatorType.Like; 
				break;
			}
			case 143: {
				lexer.NextToken();

#line  1874 "VBNET.ATG" 
				op = BinaryOperatorType.ReferenceEquality; 
				break;
			}
			case 144: {
				lexer.NextToken();

#line  1875 "VBNET.ATG" 
				op = BinaryOperatorType.ReferenceInequality; 
				break;
			}
			}
			if (StartOf(33)) {
				ShiftExpr(
#line  1878 "VBNET.ATG" 
out expr);

#line  1878 "VBNET.ATG" 
				outExpr = new BinaryOperatorExpression(outExpr, op, expr);  
			} else if (la.kind == 163) {
				lexer.NextToken();
				ShiftExpr(
#line  1881 "VBNET.ATG" 
out expr);

#line  1881 "VBNET.ATG" 
				outExpr = new BinaryOperatorExpression(outExpr, op, new UnaryOperatorExpression(expr, UnaryOperatorType.Not));  
			} else SynErr(278);
		}
	}

	void ShiftExpr(
#line  1886 "VBNET.ATG" 
out Expression outExpr) {

#line  1888 "VBNET.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		ConcatenationExpr(
#line  1891 "VBNET.ATG" 
out outExpr);
		while (la.kind == 43 || la.kind == 44) {
			if (la.kind == 43) {
				lexer.NextToken();

#line  1894 "VBNET.ATG" 
				op = BinaryOperatorType.ShiftLeft; 
			} else {
				lexer.NextToken();

#line  1895 "VBNET.ATG" 
				op = BinaryOperatorType.ShiftRight; 
			}
			ConcatenationExpr(
#line  1897 "VBNET.ATG" 
out expr);

#line  1897 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr);  
		}
	}

	void ConcatenationExpr(
#line  1901 "VBNET.ATG" 
out Expression outExpr) {

#line  1902 "VBNET.ATG" 
		Expression expr; 
		AdditiveExpr(
#line  1904 "VBNET.ATG" 
out outExpr);
		while (la.kind == 24) {
			lexer.NextToken();
			AdditiveExpr(
#line  1904 "VBNET.ATG" 
out expr);

#line  1904 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.Concat, expr);  
		}
	}

	void AdditiveExpr(
#line  1907 "VBNET.ATG" 
out Expression outExpr) {

#line  1909 "VBNET.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		ModuloExpr(
#line  1912 "VBNET.ATG" 
out outExpr);
		while (la.kind == 29 || la.kind == 30) {
			if (la.kind == 30) {
				lexer.NextToken();

#line  1915 "VBNET.ATG" 
				op = BinaryOperatorType.Add; 
			} else {
				lexer.NextToken();

#line  1916 "VBNET.ATG" 
				op = BinaryOperatorType.Subtract; 
			}
			ModuloExpr(
#line  1918 "VBNET.ATG" 
out expr);

#line  1918 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr);  
		}
	}

	void ModuloExpr(
#line  1922 "VBNET.ATG" 
out Expression outExpr) {

#line  1923 "VBNET.ATG" 
		Expression expr; 
		IntegerDivisionExpr(
#line  1925 "VBNET.ATG" 
out outExpr);
		while (la.kind == 153) {
			lexer.NextToken();
			IntegerDivisionExpr(
#line  1925 "VBNET.ATG" 
out expr);

#line  1925 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.Modulus, expr);  
		}
	}

	void IntegerDivisionExpr(
#line  1928 "VBNET.ATG" 
out Expression outExpr) {

#line  1929 "VBNET.ATG" 
		Expression expr; 
		MultiplicativeExpr(
#line  1931 "VBNET.ATG" 
out outExpr);
		while (la.kind == 26) {
			lexer.NextToken();
			MultiplicativeExpr(
#line  1931 "VBNET.ATG" 
out expr);

#line  1931 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.DivideInteger, expr);  
		}
	}

	void MultiplicativeExpr(
#line  1934 "VBNET.ATG" 
out Expression outExpr) {

#line  1936 "VBNET.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		UnaryExpr(
#line  1939 "VBNET.ATG" 
out outExpr);
		while (la.kind == 25 || la.kind == 33) {
			if (la.kind == 33) {
				lexer.NextToken();

#line  1942 "VBNET.ATG" 
				op = BinaryOperatorType.Multiply; 
			} else {
				lexer.NextToken();

#line  1943 "VBNET.ATG" 
				op = BinaryOperatorType.Divide; 
			}
			UnaryExpr(
#line  1945 "VBNET.ATG" 
out expr);

#line  1945 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr); 
		}
	}

	void UnaryExpr(
#line  1949 "VBNET.ATG" 
out Expression uExpr) {

#line  1951 "VBNET.ATG" 
		Expression expr;
		UnaryOperatorType uop = UnaryOperatorType.None;
		bool isUOp = false;
		
		while (la.kind == 29 || la.kind == 30 || la.kind == 33) {
			if (la.kind == 30) {
				lexer.NextToken();

#line  1955 "VBNET.ATG" 
				uop = UnaryOperatorType.Plus; isUOp = true; 
			} else if (la.kind == 29) {
				lexer.NextToken();

#line  1956 "VBNET.ATG" 
				uop = UnaryOperatorType.Minus; isUOp = true; 
			} else {
				lexer.NextToken();

#line  1957 "VBNET.ATG" 
				uop = UnaryOperatorType.Dereference;  isUOp = true;
			}
		}
		ExponentiationExpr(
#line  1959 "VBNET.ATG" 
out expr);

#line  1961 "VBNET.ATG" 
		if (isUOp) {
		uExpr = new UnaryOperatorExpression(expr, uop);
		} else {
			uExpr = expr;
		}
		
	}

	void ExponentiationExpr(
#line  1969 "VBNET.ATG" 
out Expression outExpr) {

#line  1970 "VBNET.ATG" 
		Expression expr; 
		SimpleExpr(
#line  1972 "VBNET.ATG" 
out outExpr);
		while (la.kind == 31) {
			lexer.NextToken();
			SimpleExpr(
#line  1972 "VBNET.ATG" 
out expr);

#line  1972 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.Power, expr);  
		}
	}

	void NormalOrReDimArgumentList(
#line  2483 "VBNET.ATG" 
out List<Expression> arguments, out bool canBeNormal, out bool canBeRedim) {

#line  2485 "VBNET.ATG" 
		arguments = new List<Expression>();
		canBeNormal = true; canBeRedim = !IsNamedAssign();
		Expression expr = null;
		
		if (StartOf(29)) {
			Argument(
#line  2490 "VBNET.ATG" 
out expr);
			if (la.kind == 214) {
				lexer.NextToken();

#line  2491 "VBNET.ATG" 
				EnsureIsZero(expr); canBeNormal = false; 
				Expr(
#line  2492 "VBNET.ATG" 
out expr);
			}
		}
		while (la.kind == 23) {
			lexer.NextToken();

#line  2495 "VBNET.ATG" 
			if (expr == null) canBeRedim = false; 

#line  2496 "VBNET.ATG" 
			arguments.Add(expr ?? Expression.Null); expr = null; 

#line  2497 "VBNET.ATG" 
			canBeRedim &= !IsNamedAssign(); 
			if (StartOf(29)) {
				Argument(
#line  2498 "VBNET.ATG" 
out expr);
				if (la.kind == 214) {
					lexer.NextToken();

#line  2499 "VBNET.ATG" 
					EnsureIsZero(expr); canBeNormal = false; 
					Expr(
#line  2500 "VBNET.ATG" 
out expr);
				}
			}

#line  2502 "VBNET.ATG" 
			if (expr == null) { canBeRedim = false; expr = Expression.Null; } 
		}

#line  2504 "VBNET.ATG" 
		if (expr != null) arguments.Add(expr); else canBeRedim = false; 
	}

	void ArrayTypeModifiers(
#line  2611 "VBNET.ATG" 
out ArrayList arrayModifiers) {

#line  2613 "VBNET.ATG" 
		arrayModifiers = new ArrayList();
		int i = 0;
		
		while (
#line  2616 "VBNET.ATG" 
IsDims()) {
			Expect(36);
			if (la.kind == 23 || la.kind == 37) {
				RankList(
#line  2618 "VBNET.ATG" 
out i);
			}

#line  2620 "VBNET.ATG" 
			arrayModifiers.Add(i);
			
			Expect(37);
		}

#line  2625 "VBNET.ATG" 
		if(arrayModifiers.Count == 0) {
		 arrayModifiers = null;
		}
		
	}

	void MemberInitializer(
#line  2450 "VBNET.ATG" 
out MemberInitializerExpression memberInitializer) {

#line  2452 "VBNET.ATG" 
		memberInitializer = new MemberInitializerExpression();
		memberInitializer.StartLocation = la.Location;
		Expression initExpr = null;
		bool isKey = false;
		string name = null;
		
		Expect(27);
		IdentifierOrKeyword(
#line  2459 "VBNET.ATG" 
out name);
		Expect(21);
		Expr(
#line  2459 "VBNET.ATG" 
out initExpr);

#line  2461 "VBNET.ATG" 
		memberInitializer.Name = name;
		memberInitializer.Expression = initExpr;
		memberInitializer.IsKey = isKey;
		memberInitializer.EndLocation = t.EndLocation;
		
	}

	void SubLambdaExpression(
#line  2055 "VBNET.ATG" 
out LambdaExpression lambda) {

#line  2057 "VBNET.ATG" 
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
#line  2064 "VBNET.ATG" 
lambda.Parameters);
			}
			Expect(37);
		}
		if (StartOf(34)) {
			if (StartOf(29)) {
				Expr(
#line  2067 "VBNET.ATG" 
out inner);

#line  2069 "VBNET.ATG" 
				lambda.ExpressionBody = inner;
				lambda.EndLocation = t.EndLocation; // la.Location?
				
			} else {
				EmbeddedStatement(
#line  2073 "VBNET.ATG" 
out statement);

#line  2075 "VBNET.ATG" 
				lambda.StatementBody = statement;
				lambda.EndLocation = t.EndLocation;
				
			}
		} else if (la.kind == 1) {
			lexer.NextToken();
			Block(
#line  2081 "VBNET.ATG" 
out statement);
			Expect(112);
			Expect(208);

#line  2084 "VBNET.ATG" 
			lambda.StatementBody = statement;
			lambda.EndLocation = t.EndLocation;
			
		} else SynErr(279);
	}

	void FunctionLambdaExpression(
#line  2090 "VBNET.ATG" 
out LambdaExpression lambda) {

#line  2092 "VBNET.ATG" 
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
#line  2099 "VBNET.ATG" 
lambda.Parameters);
			}
			Expect(37);
		}
		if (la.kind == 62) {
			lexer.NextToken();
			TypeName(
#line  2100 "VBNET.ATG" 
out typeRef);

#line  2100 "VBNET.ATG" 
			lambda.ReturnType = typeRef; 
		}
		if (StartOf(34)) {
			if (StartOf(29)) {
				Expr(
#line  2103 "VBNET.ATG" 
out inner);

#line  2105 "VBNET.ATG" 
				lambda.ExpressionBody = inner;
				lambda.EndLocation = t.EndLocation; // la.Location?
				
			} else {
				EmbeddedStatement(
#line  2109 "VBNET.ATG" 
out statement);

#line  2111 "VBNET.ATG" 
				lambda.StatementBody = statement;
				lambda.EndLocation = t.EndLocation;
				
			}
		} else if (la.kind == 1) {
			lexer.NextToken();
			Block(
#line  2117 "VBNET.ATG" 
out statement);
			Expect(112);
			Expect(126);

#line  2120 "VBNET.ATG" 
			lambda.StatementBody = statement;
			lambda.EndLocation = t.EndLocation;
			
		} else SynErr(280);
	}

	void EmbeddedStatement(
#line  2884 "VBNET.ATG" 
out Statement statement) {

#line  2886 "VBNET.ATG" 
		Statement embeddedStatement = null;
		statement = null;
		Expression expr = null;
		string name = String.Empty;
		List<Expression> p = null;
		
		if (la.kind == 119) {
			lexer.NextToken();

#line  2892 "VBNET.ATG" 
			ExitType exitType = ExitType.None; 
			switch (la.kind) {
			case 208: {
				lexer.NextToken();

#line  2894 "VBNET.ATG" 
				exitType = ExitType.Sub; 
				break;
			}
			case 126: {
				lexer.NextToken();

#line  2896 "VBNET.ATG" 
				exitType = ExitType.Function; 
				break;
			}
			case 184: {
				lexer.NextToken();

#line  2898 "VBNET.ATG" 
				exitType = ExitType.Property; 
				break;
			}
			case 107: {
				lexer.NextToken();

#line  2900 "VBNET.ATG" 
				exitType = ExitType.Do; 
				break;
			}
			case 123: {
				lexer.NextToken();

#line  2902 "VBNET.ATG" 
				exitType = ExitType.For; 
				break;
			}
			case 216: {
				lexer.NextToken();

#line  2904 "VBNET.ATG" 
				exitType = ExitType.Try; 
				break;
			}
			case 229: {
				lexer.NextToken();

#line  2906 "VBNET.ATG" 
				exitType = ExitType.While; 
				break;
			}
			case 195: {
				lexer.NextToken();

#line  2908 "VBNET.ATG" 
				exitType = ExitType.Select; 
				break;
			}
			default: SynErr(281); break;
			}

#line  2910 "VBNET.ATG" 
			statement = new ExitStatement(exitType); 
		} else if (la.kind == 216) {
			TryStatement(
#line  2911 "VBNET.ATG" 
out statement);
		} else if (la.kind == 88) {
			lexer.NextToken();

#line  2912 "VBNET.ATG" 
			ContinueType continueType = ContinueType.None; 
			if (la.kind == 107 || la.kind == 123 || la.kind == 229) {
				if (la.kind == 107) {
					lexer.NextToken();

#line  2912 "VBNET.ATG" 
					continueType = ContinueType.Do; 
				} else if (la.kind == 123) {
					lexer.NextToken();

#line  2912 "VBNET.ATG" 
					continueType = ContinueType.For; 
				} else {
					lexer.NextToken();

#line  2912 "VBNET.ATG" 
					continueType = ContinueType.While; 
				}
			}

#line  2912 "VBNET.ATG" 
			statement = new ContinueStatement(continueType); 
		} else if (la.kind == 213) {
			lexer.NextToken();
			if (StartOf(29)) {
				Expr(
#line  2914 "VBNET.ATG" 
out expr);
			}

#line  2914 "VBNET.ATG" 
			statement = new ThrowStatement(expr); 
		} else if (la.kind == 193) {
			lexer.NextToken();
			if (StartOf(29)) {
				Expr(
#line  2916 "VBNET.ATG" 
out expr);
			}

#line  2916 "VBNET.ATG" 
			statement = new ReturnStatement(expr); 
		} else if (la.kind == 209) {
			lexer.NextToken();
			Expr(
#line  2918 "VBNET.ATG" 
out expr);
			EndOfStmt();
			Block(
#line  2918 "VBNET.ATG" 
out embeddedStatement);
			Expect(112);
			Expect(209);

#line  2919 "VBNET.ATG" 
			statement = new LockStatement(expr, embeddedStatement); 
		} else if (la.kind == 187) {
			lexer.NextToken();
			Identifier();

#line  2921 "VBNET.ATG" 
			name = t.val; 
			if (la.kind == 36) {
				lexer.NextToken();
				if (StartOf(35)) {
					ArgumentList(
#line  2922 "VBNET.ATG" 
out p);
				}
				Expect(37);
			}

#line  2924 "VBNET.ATG" 
			statement = new RaiseEventStatement(name, p);
			
		} else if (la.kind == 231) {
			WithStatement(
#line  2927 "VBNET.ATG" 
out statement);
		} else if (la.kind == 55) {
			lexer.NextToken();

#line  2929 "VBNET.ATG" 
			Expression handlerExpr = null; 
			Expr(
#line  2930 "VBNET.ATG" 
out expr);
			Expect(23);
			Expr(
#line  2930 "VBNET.ATG" 
out handlerExpr);

#line  2932 "VBNET.ATG" 
			statement = new AddHandlerStatement(expr, handlerExpr);
			
		} else if (la.kind == 191) {
			lexer.NextToken();

#line  2935 "VBNET.ATG" 
			Expression handlerExpr = null; 
			Expr(
#line  2936 "VBNET.ATG" 
out expr);
			Expect(23);
			Expr(
#line  2936 "VBNET.ATG" 
out handlerExpr);

#line  2938 "VBNET.ATG" 
			statement = new RemoveHandlerStatement(expr, handlerExpr);
			
		} else if (la.kind == 229) {
			lexer.NextToken();
			Expr(
#line  2941 "VBNET.ATG" 
out expr);
			EndOfStmt();
			Block(
#line  2942 "VBNET.ATG" 
out embeddedStatement);
			Expect(112);
			Expect(229);

#line  2944 "VBNET.ATG" 
			statement = new DoLoopStatement(expr, embeddedStatement, ConditionType.While, ConditionPosition.Start);
			
		} else if (la.kind == 107) {
			lexer.NextToken();

#line  2949 "VBNET.ATG" 
			ConditionType conditionType = ConditionType.None;
			
			if (la.kind == 222 || la.kind == 229) {
				WhileOrUntil(
#line  2952 "VBNET.ATG" 
out conditionType);
				Expr(
#line  2952 "VBNET.ATG" 
out expr);
				EndOfStmt();
				Block(
#line  2953 "VBNET.ATG" 
out embeddedStatement);
				Expect(151);

#line  2956 "VBNET.ATG" 
				statement = new DoLoopStatement(expr, 
				                               embeddedStatement, 
				                               conditionType == ConditionType.While ? ConditionType.DoWhile : conditionType, 
				                               ConditionPosition.Start);
				
			} else if (la.kind == 1 || la.kind == 22) {
				EndOfStmt();
				Block(
#line  2963 "VBNET.ATG" 
out embeddedStatement);
				Expect(151);
				if (la.kind == 222 || la.kind == 229) {
					WhileOrUntil(
#line  2964 "VBNET.ATG" 
out conditionType);
					Expr(
#line  2964 "VBNET.ATG" 
out expr);
				}

#line  2966 "VBNET.ATG" 
				statement = new DoLoopStatement(expr, embeddedStatement, conditionType, ConditionPosition.End);
				
			} else SynErr(282);
		} else if (la.kind == 123) {
			lexer.NextToken();

#line  2971 "VBNET.ATG" 
			Expression group = null;
			TypeReference typeReference;
			string        typeName;
			Location startLocation = t.Location;
			
			if (la.kind == 109) {
				lexer.NextToken();
				LoopControlVariable(
#line  2978 "VBNET.ATG" 
out typeReference, out typeName);
				Expect(137);
				Expr(
#line  2979 "VBNET.ATG" 
out group);
				EndOfStmt();
				Block(
#line  2980 "VBNET.ATG" 
out embeddedStatement);
				Expect(162);
				if (StartOf(29)) {
					Expr(
#line  2981 "VBNET.ATG" 
out expr);
				}

#line  2983 "VBNET.ATG" 
				statement = new ForeachStatement(typeReference, 
				                                typeName,
				                                group, 
				                                embeddedStatement, 
				                                expr);
				statement.StartLocation = startLocation;
				statement.EndLocation   = t.EndLocation;
				
				
			} else if (StartOf(36)) {

#line  2994 "VBNET.ATG" 
				Expression start = null;
				Expression end = null;
				Expression step = null;
				Expression variableExpr = null;
				Expression nextExpr = null;
				List<Expression> nextExpressions = null;
				
				if (
#line  3001 "VBNET.ATG" 
IsLoopVariableDeclaration()) {
					LoopControlVariable(
#line  3002 "VBNET.ATG" 
out typeReference, out typeName);
				} else {

#line  3004 "VBNET.ATG" 
					typeReference = null; typeName = null; 
					SimpleExpr(
#line  3005 "VBNET.ATG" 
out variableExpr);
				}
				Expect(21);
				Expr(
#line  3007 "VBNET.ATG" 
out start);
				Expect(214);
				Expr(
#line  3007 "VBNET.ATG" 
out end);
				if (la.kind == 203) {
					lexer.NextToken();
					Expr(
#line  3007 "VBNET.ATG" 
out step);
				}
				EndOfStmt();
				Block(
#line  3008 "VBNET.ATG" 
out embeddedStatement);
				Expect(162);
				if (StartOf(29)) {
					Expr(
#line  3011 "VBNET.ATG" 
out nextExpr);

#line  3013 "VBNET.ATG" 
					nextExpressions = new List<Expression>();
					nextExpressions.Add(nextExpr);
					
					while (la.kind == 23) {
						lexer.NextToken();
						Expr(
#line  3016 "VBNET.ATG" 
out nextExpr);

#line  3016 "VBNET.ATG" 
						nextExpressions.Add(nextExpr); 
					}
				}

#line  3019 "VBNET.ATG" 
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
				
			} else SynErr(283);
		} else if (la.kind == 117) {
			lexer.NextToken();
			Expr(
#line  3032 "VBNET.ATG" 
out expr);

#line  3032 "VBNET.ATG" 
			statement = new ErrorStatement(expr); 
		} else if (la.kind == 189) {
			lexer.NextToken();

#line  3034 "VBNET.ATG" 
			bool isPreserve = false; 
			if (la.kind == 182) {
				lexer.NextToken();

#line  3034 "VBNET.ATG" 
				isPreserve = true; 
			}
			ReDimClause(
#line  3035 "VBNET.ATG" 
out expr);

#line  3037 "VBNET.ATG" 
			ReDimStatement reDimStatement = new ReDimStatement(isPreserve);
			statement = reDimStatement;
			SafeAdd(reDimStatement, reDimStatement.ReDimClauses, expr as InvocationExpression);
			
			while (la.kind == 23) {
				lexer.NextToken();
				ReDimClause(
#line  3041 "VBNET.ATG" 
out expr);

#line  3042 "VBNET.ATG" 
				SafeAdd(reDimStatement, reDimStatement.ReDimClauses, expr as InvocationExpression); 
			}
		} else if (la.kind == 116) {
			lexer.NextToken();
			Expr(
#line  3046 "VBNET.ATG" 
out expr);

#line  3048 "VBNET.ATG" 
			EraseStatement eraseStatement = new EraseStatement();
			if (expr != null) { SafeAdd(eraseStatement, eraseStatement.Expressions, expr);}
			
			while (la.kind == 23) {
				lexer.NextToken();
				Expr(
#line  3051 "VBNET.ATG" 
out expr);

#line  3051 "VBNET.ATG" 
				if (expr != null) { SafeAdd(eraseStatement, eraseStatement.Expressions, expr); }
			}

#line  3052 "VBNET.ATG" 
			statement = eraseStatement; 
		} else if (la.kind == 204) {
			lexer.NextToken();

#line  3054 "VBNET.ATG" 
			statement = new StopStatement(); 
		} else if (
#line  3056 "VBNET.ATG" 
la.kind == Tokens.If) {
			Expect(134);

#line  3057 "VBNET.ATG" 
			Location ifStartLocation = t.Location; 
			Expr(
#line  3057 "VBNET.ATG" 
out expr);
			if (la.kind == 212) {
				lexer.NextToken();
			}
			if (la.kind == 1 || la.kind == 22) {
				EndOfStmt();
				Block(
#line  3060 "VBNET.ATG" 
out embeddedStatement);

#line  3062 "VBNET.ATG" 
				IfElseStatement ifStatement = new IfElseStatement(expr, embeddedStatement);
				ifStatement.StartLocation = ifStartLocation;
				Location elseIfStart;
				
				while (la.kind == 111 || 
#line  3068 "VBNET.ATG" 
IsElseIf()) {
					if (
#line  3068 "VBNET.ATG" 
IsElseIf()) {
						Expect(110);

#line  3068 "VBNET.ATG" 
						elseIfStart = t.Location; 
						Expect(134);
					} else {
						lexer.NextToken();

#line  3069 "VBNET.ATG" 
						elseIfStart = t.Location; 
					}

#line  3071 "VBNET.ATG" 
					Expression condition = null; Statement block = null; 
					Expr(
#line  3072 "VBNET.ATG" 
out condition);
					if (la.kind == 212) {
						lexer.NextToken();
					}
					EndOfStmt();
					Block(
#line  3073 "VBNET.ATG" 
out block);

#line  3075 "VBNET.ATG" 
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
#line  3084 "VBNET.ATG" 
out embeddedStatement);

#line  3086 "VBNET.ATG" 
					ifStatement.FalseStatement.Add(embeddedStatement);
					
				}
				Expect(112);
				Expect(134);

#line  3090 "VBNET.ATG" 
				ifStatement.EndLocation = t.Location;
				statement = ifStatement;
				
			} else if (StartOf(37)) {

#line  3095 "VBNET.ATG" 
				IfElseStatement ifStatement = new IfElseStatement(expr);
				ifStatement.StartLocation = ifStartLocation;
				
				SingleLineStatementList(
#line  3098 "VBNET.ATG" 
ifStatement.TrueStatement);
				if (la.kind == 110) {
					lexer.NextToken();
					if (StartOf(37)) {
						SingleLineStatementList(
#line  3101 "VBNET.ATG" 
ifStatement.FalseStatement);
					}
				}

#line  3103 "VBNET.ATG" 
				ifStatement.EndLocation = t.Location; statement = ifStatement; 
			} else SynErr(284);
		} else if (la.kind == 195) {
			lexer.NextToken();
			if (la.kind == 73) {
				lexer.NextToken();
			}
			Expr(
#line  3106 "VBNET.ATG" 
out expr);
			EndOfStmt();

#line  3107 "VBNET.ATG" 
			List<SwitchSection> selectSections = new List<SwitchSection>();
			Statement block = null;
			
			while (la.kind == 73) {

#line  3111 "VBNET.ATG" 
				List<CaseLabel> caseClauses = null; Location caseLocation = la.Location; 
				lexer.NextToken();
				CaseClauses(
#line  3112 "VBNET.ATG" 
out caseClauses);
				if (
#line  3112 "VBNET.ATG" 
IsNotStatementSeparator()) {
					lexer.NextToken();
				}
				EndOfStmt();

#line  3114 "VBNET.ATG" 
				SwitchSection selectSection = new SwitchSection(caseClauses);
				selectSection.StartLocation = caseLocation;
				
				Block(
#line  3117 "VBNET.ATG" 
out block);

#line  3119 "VBNET.ATG" 
				selectSection.Children = block.Children;
				selectSection.EndLocation = t.EndLocation;
				selectSections.Add(selectSection);
				
			}

#line  3125 "VBNET.ATG" 
			statement = new SwitchStatement(expr, selectSections);
			
			Expect(112);
			Expect(195);
		} else if (la.kind == 170) {

#line  3128 "VBNET.ATG" 
			OnErrorStatement onErrorStatement = null; 
			OnErrorStatement(
#line  3129 "VBNET.ATG" 
out onErrorStatement);

#line  3129 "VBNET.ATG" 
			statement = onErrorStatement; 
		} else if (la.kind == 131) {

#line  3130 "VBNET.ATG" 
			GotoStatement goToStatement = null; 
			GotoStatement(
#line  3131 "VBNET.ATG" 
out goToStatement);

#line  3131 "VBNET.ATG" 
			statement = goToStatement; 
		} else if (la.kind == 192) {

#line  3132 "VBNET.ATG" 
			ResumeStatement resumeStatement = null; 
			ResumeStatement(
#line  3133 "VBNET.ATG" 
out resumeStatement);

#line  3133 "VBNET.ATG" 
			statement = resumeStatement; 
		} else if (StartOf(36)) {

#line  3136 "VBNET.ATG" 
			Expression val = null;
			AssignmentOperatorType op;
			
			bool mustBeAssignment = la.kind == Tokens.Plus  || la.kind == Tokens.Minus ||
			                        la.kind == Tokens.Not   || la.kind == Tokens.Times;
			
			SimpleExpr(
#line  3142 "VBNET.ATG" 
out expr);
			if (StartOf(38)) {
				AssignmentOperator(
#line  3144 "VBNET.ATG" 
out op);
				Expr(
#line  3144 "VBNET.ATG" 
out val);

#line  3144 "VBNET.ATG" 
				expr = new AssignmentExpression(expr, op, val); 
			} else if (StartOf(39)) {

#line  3145 "VBNET.ATG" 
				if (mustBeAssignment) Error("error in assignment."); 
			} else SynErr(285);

#line  3148 "VBNET.ATG" 
			// a field reference expression that stands alone is a
			// invocation expression without parantheses and arguments
			if(expr is MemberReferenceExpression || expr is IdentifierExpression) {
				expr = new InvocationExpression(expr);
			}
			statement = new ExpressionStatement(expr);
			
		} else if (la.kind == 72) {
			lexer.NextToken();
			SimpleExpr(
#line  3155 "VBNET.ATG" 
out expr);

#line  3155 "VBNET.ATG" 
			statement = new ExpressionStatement(expr); 
		} else if (la.kind == 224) {
			lexer.NextToken();

#line  3157 "VBNET.ATG" 
			Statement block;  
			if (
#line  3158 "VBNET.ATG" 
Peek(1).kind == Tokens.As) {

#line  3159 "VBNET.ATG" 
				LocalVariableDeclaration resourceAquisition = new LocalVariableDeclaration(Modifiers.None); 
				VariableDeclarator(
#line  3160 "VBNET.ATG" 
resourceAquisition.Variables);
				while (la.kind == 23) {
					lexer.NextToken();
					VariableDeclarator(
#line  3162 "VBNET.ATG" 
resourceAquisition.Variables);
				}
				Block(
#line  3164 "VBNET.ATG" 
out block);

#line  3166 "VBNET.ATG" 
				statement = new UsingStatement(resourceAquisition, block);
				
			} else if (StartOf(29)) {
				Expr(
#line  3168 "VBNET.ATG" 
out expr);
				Block(
#line  3169 "VBNET.ATG" 
out block);

#line  3170 "VBNET.ATG" 
				statement = new UsingStatement(new ExpressionStatement(expr), block); 
			} else SynErr(286);
			Expect(112);
			Expect(224);
		} else if (StartOf(40)) {
			LocalDeclarationStatement(
#line  3173 "VBNET.ATG" 
out statement);
		} else SynErr(287);
	}

	void FromOrAggregateQueryOperator(
#line  2139 "VBNET.ATG" 
List<QueryExpressionClause> middleClauses) {

#line  2141 "VBNET.ATG" 
		QueryExpressionFromClause fromClause = null;
		QueryExpressionAggregateClause aggregateClause = null;
		
		if (la.kind == 125) {
			FromQueryOperator(
#line  2144 "VBNET.ATG" 
out fromClause);

#line  2145 "VBNET.ATG" 
			middleClauses.Add(fromClause); 
		} else if (la.kind == 57) {
			AggregateQueryOperator(
#line  2146 "VBNET.ATG" 
out aggregateClause);

#line  2147 "VBNET.ATG" 
			middleClauses.Add(aggregateClause); 
		} else SynErr(288);
	}

	void QueryOperator(
#line  2150 "VBNET.ATG" 
List<QueryExpressionClause> middleClauses) {

#line  2152 "VBNET.ATG" 
		QueryExpressionJoinVBClause joinClause = null;
		QueryExpressionGroupVBClause groupByClause = null;
		QueryExpressionPartitionVBClause partitionClause = null;
		QueryExpressionGroupJoinVBClause groupJoinClause = null;
		QueryExpressionFromClause fromClause = null;
		QueryExpressionAggregateClause aggregateClause = null;
		
		if (la.kind == 125) {
			FromQueryOperator(
#line  2159 "VBNET.ATG" 
out fromClause);

#line  2160 "VBNET.ATG" 
			middleClauses.Add(fromClause); 
		} else if (la.kind == 57) {
			AggregateQueryOperator(
#line  2161 "VBNET.ATG" 
out aggregateClause);

#line  2162 "VBNET.ATG" 
			middleClauses.Add(aggregateClause); 
		} else if (la.kind == 195) {
			SelectQueryOperator(
#line  2163 "VBNET.ATG" 
middleClauses);
		} else if (la.kind == 106) {
			DistinctQueryOperator(
#line  2164 "VBNET.ATG" 
middleClauses);
		} else if (la.kind == 228) {
			WhereQueryOperator(
#line  2165 "VBNET.ATG" 
middleClauses);
		} else if (la.kind == 175) {
			OrderByQueryOperator(
#line  2166 "VBNET.ATG" 
middleClauses);
		} else if (la.kind == 201 || la.kind == 210) {
			PartitionQueryOperator(
#line  2167 "VBNET.ATG" 
out partitionClause);

#line  2168 "VBNET.ATG" 
			middleClauses.Add(partitionClause); 
		} else if (la.kind == 147) {
			LetQueryOperator(
#line  2169 "VBNET.ATG" 
middleClauses);
		} else if (la.kind == 145) {
			JoinQueryOperator(
#line  2170 "VBNET.ATG" 
out joinClause);

#line  2171 "VBNET.ATG" 
			middleClauses.Add(joinClause); 
		} else if (
#line  2172 "VBNET.ATG" 
la.kind == Tokens.Group && Peek(1).kind == Tokens.Join) {
			GroupJoinQueryOperator(
#line  2172 "VBNET.ATG" 
out groupJoinClause);

#line  2173 "VBNET.ATG" 
			middleClauses.Add(groupJoinClause); 
		} else if (la.kind == 132) {
			GroupByQueryOperator(
#line  2174 "VBNET.ATG" 
out groupByClause);

#line  2175 "VBNET.ATG" 
			middleClauses.Add(groupByClause); 
		} else SynErr(289);
	}

	void FromQueryOperator(
#line  2250 "VBNET.ATG" 
out QueryExpressionFromClause fromClause) {

#line  2252 "VBNET.ATG" 
		fromClause = new QueryExpressionFromClause();
		fromClause.StartLocation = la.Location;
		
		Expect(125);
		CollectionRangeVariableDeclarationList(
#line  2255 "VBNET.ATG" 
fromClause.Sources);

#line  2257 "VBNET.ATG" 
		fromClause.EndLocation = t.EndLocation;
		
	}

	void AggregateQueryOperator(
#line  2319 "VBNET.ATG" 
out QueryExpressionAggregateClause aggregateClause) {

#line  2321 "VBNET.ATG" 
		aggregateClause = new QueryExpressionAggregateClause();
		aggregateClause.IntoVariables = new List<ExpressionRangeVariable>();
		aggregateClause.StartLocation = la.Location;
		CollectionRangeVariable source;
		
		Expect(57);
		CollectionRangeVariableDeclaration(
#line  2326 "VBNET.ATG" 
out source);

#line  2328 "VBNET.ATG" 
		aggregateClause.Source = source;
		
		while (StartOf(30)) {
			QueryOperator(
#line  2331 "VBNET.ATG" 
aggregateClause.MiddleClauses);
		}
		Expect(142);
		ExpressionRangeVariableDeclarationList(
#line  2333 "VBNET.ATG" 
aggregateClause.IntoVariables);

#line  2335 "VBNET.ATG" 
		aggregateClause.EndLocation = t.EndLocation;
		
	}

	void SelectQueryOperator(
#line  2261 "VBNET.ATG" 
List<QueryExpressionClause> middleClauses) {

#line  2263 "VBNET.ATG" 
		QueryExpressionSelectVBClause selectClause = new QueryExpressionSelectVBClause();
		selectClause.StartLocation = la.Location;
		
		Expect(195);
		ExpressionRangeVariableDeclarationList(
#line  2266 "VBNET.ATG" 
selectClause.Variables);

#line  2268 "VBNET.ATG" 
		selectClause.EndLocation = t.Location;
		middleClauses.Add(selectClause);
		
	}

	void DistinctQueryOperator(
#line  2273 "VBNET.ATG" 
List<QueryExpressionClause> middleClauses) {

#line  2275 "VBNET.ATG" 
		QueryExpressionDistinctClause distinctClause = new QueryExpressionDistinctClause();
		distinctClause.StartLocation = la.Location;
		
		Expect(106);

#line  2280 "VBNET.ATG" 
		distinctClause.EndLocation = t.EndLocation;
		middleClauses.Add(distinctClause);
		
	}

	void WhereQueryOperator(
#line  2285 "VBNET.ATG" 
List<QueryExpressionClause> middleClauses) {

#line  2287 "VBNET.ATG" 
		QueryExpressionWhereClause whereClause = new QueryExpressionWhereClause();
		whereClause.StartLocation = la.Location;
		Expression operand = null;
		
		Expect(228);
		Expr(
#line  2291 "VBNET.ATG" 
out operand);

#line  2293 "VBNET.ATG" 
		whereClause.Condition = operand;
		whereClause.EndLocation = t.EndLocation;
		
		middleClauses.Add(whereClause);
		
	}

	void OrderByQueryOperator(
#line  2178 "VBNET.ATG" 
List<QueryExpressionClause> middleClauses) {

#line  2180 "VBNET.ATG" 
		QueryExpressionOrderClause orderClause = new QueryExpressionOrderClause();
		orderClause.StartLocation = la.Location;
		List<QueryExpressionOrdering> orderings = null;
		
		Expect(175);
		Expect(69);
		OrderExpressionList(
#line  2184 "VBNET.ATG" 
out orderings);

#line  2186 "VBNET.ATG" 
		orderClause.Orderings = orderings;
		orderClause.EndLocation = t.EndLocation;
		middleClauses.Add(orderClause);
		
	}

	void PartitionQueryOperator(
#line  2300 "VBNET.ATG" 
out QueryExpressionPartitionVBClause partitionClause) {

#line  2302 "VBNET.ATG" 
		partitionClause = new QueryExpressionPartitionVBClause();
		partitionClause.StartLocation = la.Location;
		Expression expr = null;
		
		if (la.kind == 210) {
			lexer.NextToken();

#line  2307 "VBNET.ATG" 
			partitionClause.PartitionType = QueryExpressionPartitionType.Take; 
			if (la.kind == 229) {
				lexer.NextToken();

#line  2308 "VBNET.ATG" 
				partitionClause.PartitionType = QueryExpressionPartitionType.TakeWhile; 
			}
		} else if (la.kind == 201) {
			lexer.NextToken();

#line  2309 "VBNET.ATG" 
			partitionClause.PartitionType = QueryExpressionPartitionType.Skip; 
			if (la.kind == 229) {
				lexer.NextToken();

#line  2310 "VBNET.ATG" 
				partitionClause.PartitionType = QueryExpressionPartitionType.SkipWhile; 
			}
		} else SynErr(290);
		Expr(
#line  2312 "VBNET.ATG" 
out expr);

#line  2314 "VBNET.ATG" 
		partitionClause.Expression = expr;
		partitionClause.EndLocation = t.EndLocation;
		
	}

	void LetQueryOperator(
#line  2339 "VBNET.ATG" 
List<QueryExpressionClause> middleClauses) {

#line  2341 "VBNET.ATG" 
		QueryExpressionLetVBClause letClause = new QueryExpressionLetVBClause();
		letClause.StartLocation = la.Location;
		
		Expect(147);
		ExpressionRangeVariableDeclarationList(
#line  2344 "VBNET.ATG" 
letClause.Variables);

#line  2346 "VBNET.ATG" 
		letClause.EndLocation = t.EndLocation;
		middleClauses.Add(letClause);
		
	}

	void JoinQueryOperator(
#line  2383 "VBNET.ATG" 
out QueryExpressionJoinVBClause joinClause) {

#line  2385 "VBNET.ATG" 
		joinClause = new QueryExpressionJoinVBClause();
		joinClause.StartLocation = la.Location;
		CollectionRangeVariable joinVariable = null;
		QueryExpressionJoinVBClause subJoin = null;
		QueryExpressionJoinConditionVB condition = null;
		
		
		Expect(145);
		CollectionRangeVariableDeclaration(
#line  2392 "VBNET.ATG" 
out joinVariable);

#line  2393 "VBNET.ATG" 
		joinClause.JoinVariable = joinVariable; 
		if (la.kind == 145) {
			JoinQueryOperator(
#line  2395 "VBNET.ATG" 
out subJoin);

#line  2396 "VBNET.ATG" 
			joinClause.SubJoin = subJoin; 
		}
		Expect(170);
		JoinCondition(
#line  2399 "VBNET.ATG" 
out condition);

#line  2400 "VBNET.ATG" 
		SafeAdd(joinClause, joinClause.Conditions, condition); 
		while (la.kind == 59) {
			lexer.NextToken();
			JoinCondition(
#line  2402 "VBNET.ATG" 
out condition);

#line  2403 "VBNET.ATG" 
			SafeAdd(joinClause, joinClause.Conditions, condition); 
		}

#line  2406 "VBNET.ATG" 
		joinClause.EndLocation = t.EndLocation;
		
	}

	void GroupJoinQueryOperator(
#line  2236 "VBNET.ATG" 
out QueryExpressionGroupJoinVBClause groupJoinClause) {

#line  2238 "VBNET.ATG" 
		groupJoinClause = new QueryExpressionGroupJoinVBClause();
		groupJoinClause.StartLocation = la.Location;
		QueryExpressionJoinVBClause joinClause = null;
		
		Expect(132);
		JoinQueryOperator(
#line  2242 "VBNET.ATG" 
out joinClause);
		Expect(142);
		ExpressionRangeVariableDeclarationList(
#line  2243 "VBNET.ATG" 
groupJoinClause.IntoVariables);

#line  2245 "VBNET.ATG" 
		groupJoinClause.JoinClause = joinClause;
		groupJoinClause.EndLocation = t.EndLocation;
		
	}

	void GroupByQueryOperator(
#line  2223 "VBNET.ATG" 
out QueryExpressionGroupVBClause groupByClause) {

#line  2225 "VBNET.ATG" 
		groupByClause = new QueryExpressionGroupVBClause();
		groupByClause.StartLocation = la.Location;
		
		Expect(132);
		ExpressionRangeVariableDeclarationList(
#line  2228 "VBNET.ATG" 
groupByClause.GroupVariables);
		Expect(69);
		ExpressionRangeVariableDeclarationList(
#line  2229 "VBNET.ATG" 
groupByClause.ByVariables);
		Expect(142);
		ExpressionRangeVariableDeclarationList(
#line  2230 "VBNET.ATG" 
groupByClause.IntoVariables);

#line  2232 "VBNET.ATG" 
		groupByClause.EndLocation = t.EndLocation;
		
	}

	void OrderExpressionList(
#line  2192 "VBNET.ATG" 
out List<QueryExpressionOrdering> orderings) {

#line  2194 "VBNET.ATG" 
		orderings = new List<QueryExpressionOrdering>();
		QueryExpressionOrdering ordering = null;
		
		OrderExpression(
#line  2197 "VBNET.ATG" 
out ordering);

#line  2198 "VBNET.ATG" 
		orderings.Add(ordering); 
		while (la.kind == 23) {
			lexer.NextToken();
			OrderExpression(
#line  2200 "VBNET.ATG" 
out ordering);

#line  2201 "VBNET.ATG" 
			orderings.Add(ordering); 
		}
	}

	void OrderExpression(
#line  2205 "VBNET.ATG" 
out QueryExpressionOrdering ordering) {

#line  2207 "VBNET.ATG" 
		ordering = new QueryExpressionOrdering();
		ordering.StartLocation = la.Location;
		ordering.Direction = QueryExpressionOrderingDirection.None;
		Expression orderExpr = null;
		
		Expr(
#line  2212 "VBNET.ATG" 
out orderExpr);

#line  2214 "VBNET.ATG" 
		ordering.Criteria = orderExpr;
		
		if (la.kind == 63 || la.kind == 103) {
			if (la.kind == 63) {
				lexer.NextToken();

#line  2217 "VBNET.ATG" 
				ordering.Direction = QueryExpressionOrderingDirection.Ascending; 
			} else {
				lexer.NextToken();

#line  2218 "VBNET.ATG" 
				ordering.Direction = QueryExpressionOrderingDirection.Descending; 
			}
		}

#line  2220 "VBNET.ATG" 
		ordering.EndLocation = t.EndLocation; 
	}

	void ExpressionRangeVariableDeclarationList(
#line  2351 "VBNET.ATG" 
List<ExpressionRangeVariable> variables) {

#line  2353 "VBNET.ATG" 
		ExpressionRangeVariable variable = null;
		
		ExpressionRangeVariableDeclaration(
#line  2355 "VBNET.ATG" 
out variable);

#line  2356 "VBNET.ATG" 
		variables.Add(variable); 
		while (la.kind == 23) {
			lexer.NextToken();
			ExpressionRangeVariableDeclaration(
#line  2357 "VBNET.ATG" 
out variable);

#line  2357 "VBNET.ATG" 
			variables.Add(variable); 
		}
	}

	void CollectionRangeVariableDeclarationList(
#line  2410 "VBNET.ATG" 
List<CollectionRangeVariable> rangeVariables) {

#line  2411 "VBNET.ATG" 
		CollectionRangeVariable variableDeclaration; 
		CollectionRangeVariableDeclaration(
#line  2413 "VBNET.ATG" 
out variableDeclaration);

#line  2414 "VBNET.ATG" 
		rangeVariables.Add(variableDeclaration); 
		while (la.kind == 23) {
			lexer.NextToken();
			CollectionRangeVariableDeclaration(
#line  2415 "VBNET.ATG" 
out variableDeclaration);

#line  2415 "VBNET.ATG" 
			rangeVariables.Add(variableDeclaration); 
		}
	}

	void CollectionRangeVariableDeclaration(
#line  2418 "VBNET.ATG" 
out CollectionRangeVariable rangeVariable) {

#line  2420 "VBNET.ATG" 
		rangeVariable = new CollectionRangeVariable();
		rangeVariable.StartLocation = la.Location;
		TypeReference typeName = null;
		Expression inExpr = null;
		
		Identifier();

#line  2425 "VBNET.ATG" 
		rangeVariable.Identifier = t.val; 
		if (la.kind == 62) {
			lexer.NextToken();
			TypeName(
#line  2426 "VBNET.ATG" 
out typeName);

#line  2426 "VBNET.ATG" 
			rangeVariable.Type = typeName; 
		}
		Expect(137);
		Expr(
#line  2427 "VBNET.ATG" 
out inExpr);

#line  2429 "VBNET.ATG" 
		rangeVariable.Expression = inExpr;
		rangeVariable.EndLocation = t.EndLocation;
		
	}

	void ExpressionRangeVariableDeclaration(
#line  2360 "VBNET.ATG" 
out ExpressionRangeVariable variable) {

#line  2362 "VBNET.ATG" 
		variable = new ExpressionRangeVariable();
		variable.StartLocation = la.Location;
		Expression rhs = null;
		TypeReference typeName = null;
		
		if (
#line  2368 "VBNET.ATG" 
IsIdentifiedExpressionRange()) {
			Identifier();

#line  2369 "VBNET.ATG" 
			variable.Identifier = t.val; 
			if (la.kind == 62) {
				lexer.NextToken();
				TypeName(
#line  2371 "VBNET.ATG" 
out typeName);

#line  2372 "VBNET.ATG" 
				variable.Type = typeName; 
			}
			Expect(21);
		}
		Expr(
#line  2376 "VBNET.ATG" 
out rhs);

#line  2378 "VBNET.ATG" 
		variable.Expression = rhs;
		variable.EndLocation = t.EndLocation;
		
	}

	void JoinCondition(
#line  2434 "VBNET.ATG" 
out QueryExpressionJoinConditionVB condition) {

#line  2436 "VBNET.ATG" 
		condition = new QueryExpressionJoinConditionVB();
		condition.StartLocation = la.Location;
		
		Expression lhs = null;
		Expression rhs = null;
		
		Expr(
#line  2442 "VBNET.ATG" 
out lhs);
		Expect(115);
		Expr(
#line  2442 "VBNET.ATG" 
out rhs);

#line  2444 "VBNET.ATG" 
		condition.LeftSide = lhs;
		condition.RightSide = rhs;
		condition.EndLocation = t.EndLocation;
		
	}

	void Argument(
#line  2508 "VBNET.ATG" 
out Expression argumentexpr) {

#line  2510 "VBNET.ATG" 
		Expression expr;
		argumentexpr = null;
		string name;
		
		if (
#line  2514 "VBNET.ATG" 
IsNamedAssign()) {
			Identifier();

#line  2514 "VBNET.ATG" 
			name = t.val;  
			Expect(54);
			Expr(
#line  2514 "VBNET.ATG" 
out expr);

#line  2516 "VBNET.ATG" 
			argumentexpr = new NamedArgumentExpression(name, expr);
			
		} else if (StartOf(29)) {
			Expr(
#line  2519 "VBNET.ATG" 
out argumentexpr);
		} else SynErr(291);
	}

	void QualIdentAndTypeArguments(
#line  2585 "VBNET.ATG" 
out TypeReference typeref, bool canBeUnbound) {

#line  2586 "VBNET.ATG" 
		string name; typeref = null; 
		Qualident(
#line  2588 "VBNET.ATG" 
out name);

#line  2589 "VBNET.ATG" 
		typeref = new TypeReference(name); 
		if (
#line  2590 "VBNET.ATG" 
la.kind == Tokens.OpenParenthesis && Peek(1).kind == Tokens.Of) {
			lexer.NextToken();
			Expect(168);
			if (
#line  2592 "VBNET.ATG" 
canBeUnbound && (la.kind == Tokens.CloseParenthesis || la.kind == Tokens.Comma)) {

#line  2593 "VBNET.ATG" 
				typeref.GenericTypes.Add(NullTypeReference.Instance); 
				while (la.kind == 23) {
					lexer.NextToken();

#line  2594 "VBNET.ATG" 
					typeref.GenericTypes.Add(NullTypeReference.Instance); 
				}
			} else if (StartOf(8)) {
				TypeArgumentList(
#line  2595 "VBNET.ATG" 
typeref.GenericTypes);
			} else SynErr(292);
			Expect(37);
		}
	}

	void RankList(
#line  2632 "VBNET.ATG" 
out int i) {

#line  2633 "VBNET.ATG" 
		i = 0; 
		while (la.kind == 23) {
			lexer.NextToken();

#line  2634 "VBNET.ATG" 
			++i; 
		}
	}

	void Attribute(
#line  2673 "VBNET.ATG" 
out ASTAttribute attribute) {

#line  2674 "VBNET.ATG" 
		string name;
		List<Expression> positional = new List<Expression>();
		List<NamedArgumentExpression> named = new List<NamedArgumentExpression>();
		
		if (la.kind == 129) {
			lexer.NextToken();
			Expect(27);
		}
		Qualident(
#line  2679 "VBNET.ATG" 
out name);
		if (la.kind == 36) {
			AttributeArguments(
#line  2680 "VBNET.ATG" 
positional, named);
		}

#line  2682 "VBNET.ATG" 
		attribute  = new ASTAttribute(name, positional, named);
		
	}

	void AttributeArguments(
#line  2687 "VBNET.ATG" 
List<Expression> positional, List<NamedArgumentExpression> named) {

#line  2689 "VBNET.ATG" 
		bool nameFound = false;
		string name = "";
		Expression expr;
		
		Expect(36);
		if (
#line  2695 "VBNET.ATG" 
IsNotClosingParenthesis()) {
			if (
#line  2697 "VBNET.ATG" 
IsNamedAssign()) {

#line  2697 "VBNET.ATG" 
				nameFound = true; 
				IdentifierOrKeyword(
#line  2698 "VBNET.ATG" 
out name);
				if (la.kind == 54) {
					lexer.NextToken();
				} else if (la.kind == 21) {
					lexer.NextToken();
				} else SynErr(293);
			}
			Expr(
#line  2700 "VBNET.ATG" 
out expr);

#line  2702 "VBNET.ATG" 
			if (expr != null) {
			if (string.IsNullOrEmpty(name)) { positional.Add(expr); }
			else { named.Add(new NamedArgumentExpression(name, expr)); name = ""; }
			}
			
			while (la.kind == 23) {
				lexer.NextToken();
				if (
#line  2710 "VBNET.ATG" 
IsNamedAssign()) {

#line  2710 "VBNET.ATG" 
					nameFound = true; 
					IdentifierOrKeyword(
#line  2711 "VBNET.ATG" 
out name);
					if (la.kind == 54) {
						lexer.NextToken();
					} else if (la.kind == 21) {
						lexer.NextToken();
					} else SynErr(294);
				} else if (StartOf(29)) {

#line  2713 "VBNET.ATG" 
					if (nameFound) Error("no positional argument after named argument"); 
				} else SynErr(295);
				Expr(
#line  2714 "VBNET.ATG" 
out expr);

#line  2714 "VBNET.ATG" 
				if (expr != null) { if(name == "") positional.Add(expr);
				else { named.Add(new NamedArgumentExpression(name, expr)); name = ""; }
				}
				
			}
		}
		Expect(37);
	}

	void FormalParameter(
#line  2771 "VBNET.ATG" 
out ParameterDeclarationExpression p) {

#line  2773 "VBNET.ATG" 
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		TypeReference type = null;
		ParamModifierList mod = new ParamModifierList(this);
		Expression expr = null;
		p = null;
		ArrayList arrayModifiers = null;
		
		while (la.kind == 39) {
			AttributeSection(
#line  2782 "VBNET.ATG" 
out section);

#line  2782 "VBNET.ATG" 
			attributes.Add(section); 
		}
		while (StartOf(41)) {
			ParameterModifier(
#line  2783 "VBNET.ATG" 
mod);
		}
		Identifier();

#line  2784 "VBNET.ATG" 
		string parameterName = t.val; 
		if (
#line  2785 "VBNET.ATG" 
IsDims()) {
			ArrayTypeModifiers(
#line  2785 "VBNET.ATG" 
out arrayModifiers);
		}
		if (la.kind == 62) {
			lexer.NextToken();
			TypeName(
#line  2786 "VBNET.ATG" 
out type);
		}

#line  2788 "VBNET.ATG" 
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
		
		if (la.kind == 21) {
			lexer.NextToken();
			Expr(
#line  2800 "VBNET.ATG" 
out expr);
		}

#line  2802 "VBNET.ATG" 
		mod.Check();
		p = new ParameterDeclarationExpression(type, parameterName, mod.Modifier, expr);
		p.Attributes = attributes;
		
	}

	void ParameterModifier(
#line  3484 "VBNET.ATG" 
ParamModifierList m) {
		if (la.kind == 71) {
			lexer.NextToken();

#line  3485 "VBNET.ATG" 
			m.Add(ParameterModifiers.In); 
		} else if (la.kind == 68) {
			lexer.NextToken();

#line  3486 "VBNET.ATG" 
			m.Add(ParameterModifiers.Ref); 
		} else if (la.kind == 173) {
			lexer.NextToken();

#line  3487 "VBNET.ATG" 
			m.Add(ParameterModifiers.Optional); 
		} else if (la.kind == 180) {
			lexer.NextToken();

#line  3488 "VBNET.ATG" 
			m.Add(ParameterModifiers.Params); 
		} else SynErr(296);
	}

	void Statement() {

#line  2831 "VBNET.ATG" 
		Statement stmt = null;
		Location startPos = la.Location;
		string label = String.Empty;
		
		
		if (la.kind == 1 || la.kind == 22) {
		} else if (
#line  2837 "VBNET.ATG" 
IsLabel()) {
			LabelName(
#line  2837 "VBNET.ATG" 
out label);

#line  2839 "VBNET.ATG" 
			AddChild(new LabelStatement(t.val));
			
			Expect(22);
			Statement();
		} else if (StartOf(42)) {
			EmbeddedStatement(
#line  2842 "VBNET.ATG" 
out stmt);

#line  2842 "VBNET.ATG" 
			AddChild(stmt); 
		} else SynErr(297);

#line  2845 "VBNET.ATG" 
		if (stmt != null) {
		stmt.StartLocation = startPos;
		stmt.EndLocation = t.Location;
		}
		
	}

	void LabelName(
#line  3260 "VBNET.ATG" 
out string name) {

#line  3262 "VBNET.ATG" 
		name = String.Empty;
		
		if (StartOf(4)) {
			Identifier();

#line  3264 "VBNET.ATG" 
			name = t.val; 
		} else if (la.kind == 5) {
			lexer.NextToken();

#line  3265 "VBNET.ATG" 
			name = t.val; 
		} else SynErr(298);
	}

	void LocalDeclarationStatement(
#line  2853 "VBNET.ATG" 
out Statement statement) {

#line  2855 "VBNET.ATG" 
		ModifierList m = new ModifierList();
		LocalVariableDeclaration localVariableDeclaration;
		bool dimfound = false;
		
		while (la.kind == 87 || la.kind == 104 || la.kind == 202) {
			if (la.kind == 87) {
				lexer.NextToken();

#line  2861 "VBNET.ATG" 
				m.Add(Modifiers.Const, t.Location); 
			} else if (la.kind == 202) {
				lexer.NextToken();

#line  2862 "VBNET.ATG" 
				m.Add(Modifiers.Static, t.Location); 
			} else {
				lexer.NextToken();

#line  2863 "VBNET.ATG" 
				dimfound = true; 
			}
		}

#line  2866 "VBNET.ATG" 
		if(dimfound && (m.Modifier & Modifiers.Const) != 0) {
		Error("Dim is not allowed on constants.");
		}
		
		if(m.isNone && dimfound == false) {
			Error("Const, Dim or Static expected");
		}
		
		localVariableDeclaration = new LocalVariableDeclaration(m.Modifier);
		localVariableDeclaration.StartLocation = t.Location;
		
		VariableDeclarator(
#line  2877 "VBNET.ATG" 
localVariableDeclaration.Variables);
		while (la.kind == 23) {
			lexer.NextToken();
			VariableDeclarator(
#line  2878 "VBNET.ATG" 
localVariableDeclaration.Variables);
		}

#line  2880 "VBNET.ATG" 
		statement = localVariableDeclaration;
		
	}

	void TryStatement(
#line  3374 "VBNET.ATG" 
out Statement tryStatement) {

#line  3376 "VBNET.ATG" 
		Statement blockStmt = null, finallyStmt = null;List<CatchClause> catchClauses = null;
		
		Expect(216);
		EndOfStmt();
		Block(
#line  3379 "VBNET.ATG" 
out blockStmt);
		if (la.kind == 74 || la.kind == 112 || la.kind == 122) {
			CatchClauses(
#line  3380 "VBNET.ATG" 
out catchClauses);
		}
		if (la.kind == 122) {
			lexer.NextToken();
			EndOfStmt();
			Block(
#line  3381 "VBNET.ATG" 
out finallyStmt);
		}
		Expect(112);
		Expect(216);

#line  3384 "VBNET.ATG" 
		tryStatement = new TryCatchStatement(blockStmt, catchClauses, finallyStmt);
		
	}

	void WithStatement(
#line  3354 "VBNET.ATG" 
out Statement withStatement) {

#line  3356 "VBNET.ATG" 
		Statement blockStmt = null;
		Expression expr = null;
		
		Expect(231);

#line  3359 "VBNET.ATG" 
		Location start = t.Location; 
		Expr(
#line  3360 "VBNET.ATG" 
out expr);
		EndOfStmt();

#line  3362 "VBNET.ATG" 
		withStatement = new WithStatement(expr);
		withStatement.StartLocation = start;
		
		Block(
#line  3365 "VBNET.ATG" 
out blockStmt);

#line  3367 "VBNET.ATG" 
		((WithStatement)withStatement).Body = (BlockStatement)blockStmt;
		
		Expect(112);
		Expect(231);

#line  3370 "VBNET.ATG" 
		withStatement.EndLocation = t.Location; 
	}

	void WhileOrUntil(
#line  3347 "VBNET.ATG" 
out ConditionType conditionType) {

#line  3348 "VBNET.ATG" 
		conditionType = ConditionType.None; 
		if (la.kind == 229) {
			lexer.NextToken();

#line  3349 "VBNET.ATG" 
			conditionType = ConditionType.While; 
		} else if (la.kind == 222) {
			lexer.NextToken();

#line  3350 "VBNET.ATG" 
			conditionType = ConditionType.Until; 
		} else SynErr(299);
	}

	void LoopControlVariable(
#line  3190 "VBNET.ATG" 
out TypeReference type, out string name) {

#line  3191 "VBNET.ATG" 
		ArrayList arrayModifiers = null;
		type = null;
		
		Qualident(
#line  3195 "VBNET.ATG" 
out name);
		if (
#line  3196 "VBNET.ATG" 
IsDims()) {
			ArrayTypeModifiers(
#line  3196 "VBNET.ATG" 
out arrayModifiers);
		}
		if (la.kind == 62) {
			lexer.NextToken();
			TypeName(
#line  3197 "VBNET.ATG" 
out type);

#line  3197 "VBNET.ATG" 
			if (name.IndexOf('.') > 0) { Error("No type def for 'for each' member indexer allowed."); } 
		}

#line  3199 "VBNET.ATG" 
		if (type != null) {
		if(type.RankSpecifier != null && arrayModifiers != null) {
			Error("array rank only allowed one time");
		} else if (arrayModifiers != null) {
			type.RankSpecifier = (int[])arrayModifiers.ToArray(typeof(int));
		}
		}
		
	}

	void ReDimClause(
#line  3269 "VBNET.ATG" 
out Expression expr) {
		SimpleNonInvocationExpression(
#line  3271 "VBNET.ATG" 
out expr);
		ReDimClauseInternal(
#line  3272 "VBNET.ATG" 
ref expr);
	}

	void SingleLineStatementList(
#line  3176 "VBNET.ATG" 
List<Statement> list) {

#line  3177 "VBNET.ATG" 
		Statement embeddedStatement = null; 
		if (la.kind == 112) {
			lexer.NextToken();

#line  3179 "VBNET.ATG" 
			embeddedStatement = new EndStatement(); 
		} else if (StartOf(42)) {
			EmbeddedStatement(
#line  3180 "VBNET.ATG" 
out embeddedStatement);
		} else SynErr(300);

#line  3181 "VBNET.ATG" 
		if (embeddedStatement != null) list.Add(embeddedStatement); 
		while (la.kind == 22) {
			lexer.NextToken();
			while (la.kind == 22) {
				lexer.NextToken();
			}
			if (la.kind == 112) {
				lexer.NextToken();

#line  3183 "VBNET.ATG" 
				embeddedStatement = new EndStatement(); 
			} else if (StartOf(42)) {
				EmbeddedStatement(
#line  3184 "VBNET.ATG" 
out embeddedStatement);
			} else SynErr(301);

#line  3185 "VBNET.ATG" 
			if (embeddedStatement != null) list.Add(embeddedStatement); 
		}
	}

	void CaseClauses(
#line  3307 "VBNET.ATG" 
out List<CaseLabel> caseClauses) {

#line  3309 "VBNET.ATG" 
		caseClauses = new List<CaseLabel>();
		CaseLabel caseClause = null;
		
		CaseClause(
#line  3312 "VBNET.ATG" 
out caseClause);

#line  3312 "VBNET.ATG" 
		if (caseClause != null) { caseClauses.Add(caseClause); } 
		while (la.kind == 23) {
			lexer.NextToken();
			CaseClause(
#line  3313 "VBNET.ATG" 
out caseClause);

#line  3313 "VBNET.ATG" 
			if (caseClause != null) { caseClauses.Add(caseClause); } 
		}
	}

	void OnErrorStatement(
#line  3210 "VBNET.ATG" 
out OnErrorStatement stmt) {

#line  3212 "VBNET.ATG" 
		stmt = null;
		GotoStatement goToStatement = null;
		
		Expect(170);
		Expect(117);
		if (
#line  3218 "VBNET.ATG" 
IsNegativeLabelName()) {
			Expect(131);
			Expect(29);
			Expect(5);

#line  3220 "VBNET.ATG" 
			long intLabel = Int64.Parse(t.val);
			if(intLabel != 1) {
				Error("invalid label in on error statement.");
			}
			stmt = new OnErrorStatement(new GotoStatement((intLabel * -1).ToString()));
			
		} else if (la.kind == 131) {
			GotoStatement(
#line  3226 "VBNET.ATG" 
out goToStatement);

#line  3228 "VBNET.ATG" 
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

#line  3242 "VBNET.ATG" 
			stmt = new OnErrorStatement(new ResumeStatement(true));
			
		} else SynErr(302);
	}

	void GotoStatement(
#line  3248 "VBNET.ATG" 
out GotoStatement goToStatement) {

#line  3250 "VBNET.ATG" 
		string label = String.Empty;
		
		Expect(131);
		LabelName(
#line  3253 "VBNET.ATG" 
out label);

#line  3255 "VBNET.ATG" 
		goToStatement = new GotoStatement(label);
		
	}

	void ResumeStatement(
#line  3296 "VBNET.ATG" 
out ResumeStatement resumeStatement) {

#line  3298 "VBNET.ATG" 
		resumeStatement = null;
		string label = String.Empty;
		
		if (
#line  3301 "VBNET.ATG" 
IsResumeNext()) {
			Expect(192);
			Expect(162);

#line  3302 "VBNET.ATG" 
			resumeStatement = new ResumeStatement(true); 
		} else if (la.kind == 192) {
			lexer.NextToken();
			if (StartOf(43)) {
				LabelName(
#line  3303 "VBNET.ATG" 
out label);
			}

#line  3303 "VBNET.ATG" 
			resumeStatement = new ResumeStatement(label); 
		} else SynErr(303);
	}

	void ReDimClauseInternal(
#line  3275 "VBNET.ATG" 
ref Expression expr) {

#line  3276 "VBNET.ATG" 
		List<Expression> arguments; bool canBeNormal; bool canBeRedim; string name; 
		while (la.kind == 27 || 
#line  3279 "VBNET.ATG" 
la.kind == Tokens.OpenParenthesis && Peek(1).kind == Tokens.Of) {
			if (la.kind == 27) {
				lexer.NextToken();
				IdentifierOrKeyword(
#line  3278 "VBNET.ATG" 
out name);

#line  3278 "VBNET.ATG" 
				expr = new MemberReferenceExpression(expr, name); 
			} else {
				InvocationExpression(
#line  3280 "VBNET.ATG" 
ref expr);
			}
		}
		Expect(36);
		NormalOrReDimArgumentList(
#line  3283 "VBNET.ATG" 
out arguments, out canBeNormal, out canBeRedim);
		Expect(37);

#line  3285 "VBNET.ATG" 
		expr = new InvocationExpression(expr, arguments);
		if (canBeRedim == false || canBeNormal && (la.kind == Tokens.Dot || la.kind == Tokens.OpenParenthesis)) {
			if (this.Errors.Count == 0) {
				// don't recurse on parse errors - could result in endless recursion
				ReDimClauseInternal(ref expr);
			}
		}
		
	}

	void CaseClause(
#line  3317 "VBNET.ATG" 
out CaseLabel caseClause) {

#line  3319 "VBNET.ATG" 
		Expression expr = null;
		Expression sexpr = null;
		BinaryOperatorType op = BinaryOperatorType.None;
		caseClause = null;
		
		if (la.kind == 110) {
			lexer.NextToken();

#line  3325 "VBNET.ATG" 
			caseClause = new CaseLabel(); 
		} else if (StartOf(44)) {
			if (la.kind == 143) {
				lexer.NextToken();
			}
			switch (la.kind) {
			case 39: {
				lexer.NextToken();

#line  3329 "VBNET.ATG" 
				op = BinaryOperatorType.LessThan; 
				break;
			}
			case 38: {
				lexer.NextToken();

#line  3330 "VBNET.ATG" 
				op = BinaryOperatorType.GreaterThan; 
				break;
			}
			case 42: {
				lexer.NextToken();

#line  3331 "VBNET.ATG" 
				op = BinaryOperatorType.LessThanOrEqual; 
				break;
			}
			case 41: {
				lexer.NextToken();

#line  3332 "VBNET.ATG" 
				op = BinaryOperatorType.GreaterThanOrEqual; 
				break;
			}
			case 21: {
				lexer.NextToken();

#line  3333 "VBNET.ATG" 
				op = BinaryOperatorType.Equality; 
				break;
			}
			case 40: {
				lexer.NextToken();

#line  3334 "VBNET.ATG" 
				op = BinaryOperatorType.InEquality; 
				break;
			}
			default: SynErr(304); break;
			}
			Expr(
#line  3336 "VBNET.ATG" 
out expr);

#line  3338 "VBNET.ATG" 
			caseClause = new CaseLabel(op, expr);
			
		} else if (StartOf(29)) {
			Expr(
#line  3340 "VBNET.ATG" 
out expr);
			if (la.kind == 214) {
				lexer.NextToken();
				Expr(
#line  3340 "VBNET.ATG" 
out sexpr);
			}

#line  3342 "VBNET.ATG" 
			caseClause = new CaseLabel(expr, sexpr);
			
		} else SynErr(305);
	}

	void CatchClauses(
#line  3389 "VBNET.ATG" 
out List<CatchClause> catchClauses) {

#line  3391 "VBNET.ATG" 
		catchClauses = new List<CatchClause>();
		TypeReference type = null;
		Statement blockStmt = null;
		Expression expr = null;
		string name = String.Empty;
		
		while (la.kind == 74) {
			lexer.NextToken();
			if (StartOf(4)) {
				Identifier();

#line  3399 "VBNET.ATG" 
				name = t.val; 
				if (la.kind == 62) {
					lexer.NextToken();
					TypeName(
#line  3399 "VBNET.ATG" 
out type);
				}
			}
			if (la.kind == 227) {
				lexer.NextToken();
				Expr(
#line  3400 "VBNET.ATG" 
out expr);
			}
			EndOfStmt();
			Block(
#line  3402 "VBNET.ATG" 
out blockStmt);

#line  3403 "VBNET.ATG" 
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
			case 235: s = "??? expected"; break;
			case 236: s = "invalid EndOfStmt"; break;
			case 237: s = "invalid OptionStmt"; break;
			case 238: s = "invalid OptionStmt"; break;
			case 239: s = "invalid GlobalAttributeSection"; break;
			case 240: s = "invalid GlobalAttributeSection"; break;
			case 241: s = "invalid NamespaceMemberDecl"; break;
			case 242: s = "invalid OptionValue"; break;
			case 243: s = "invalid ImportClause"; break;
			case 244: s = "invalid Identifier"; break;
			case 245: s = "invalid TypeModifier"; break;
			case 246: s = "invalid NonModuleDeclaration"; break;
			case 247: s = "invalid NonModuleDeclaration"; break;
			case 248: s = "invalid TypeParameterConstraints"; break;
			case 249: s = "invalid TypeParameterConstraint"; break;
			case 250: s = "invalid NonArrayTypeName"; break;
			case 251: s = "invalid MemberModifier"; break;
			case 252: s = "invalid StructureMemberDecl"; break;
			case 253: s = "invalid StructureMemberDecl"; break;
			case 254: s = "invalid StructureMemberDecl"; break;
			case 255: s = "invalid StructureMemberDecl"; break;
			case 256: s = "invalid StructureMemberDecl"; break;
			case 257: s = "invalid StructureMemberDecl"; break;
			case 258: s = "invalid StructureMemberDecl"; break;
			case 259: s = "invalid StructureMemberDecl"; break;
			case 260: s = "invalid InterfaceMemberDecl"; break;
			case 261: s = "invalid InterfaceMemberDecl"; break;
			case 262: s = "invalid Expr"; break;
			case 263: s = "invalid Charset"; break;
			case 264: s = "invalid IdentifierForFieldDeclaration"; break;
			case 265: s = "invalid VariableDeclaratorPartAfterIdentifier"; break;
			case 266: s = "invalid AccessorDecls"; break;
			case 267: s = "invalid EventAccessorDeclaration"; break;
			case 268: s = "invalid OverloadableOperator"; break;
			case 269: s = "invalid EventMemberSpecifier"; break;
			case 270: s = "invalid LambdaExpr"; break;
			case 271: s = "invalid AssignmentOperator"; break;
			case 272: s = "invalid SimpleNonInvocationExpression"; break;
			case 273: s = "invalid SimpleNonInvocationExpression"; break;
			case 274: s = "invalid SimpleNonInvocationExpression"; break;
			case 275: s = "invalid SimpleNonInvocationExpression"; break;
			case 276: s = "invalid PrimitiveTypeName"; break;
			case 277: s = "invalid CastTarget"; break;
			case 278: s = "invalid ComparisonExpr"; break;
			case 279: s = "invalid SubLambdaExpression"; break;
			case 280: s = "invalid FunctionLambdaExpression"; break;
			case 281: s = "invalid EmbeddedStatement"; break;
			case 282: s = "invalid EmbeddedStatement"; break;
			case 283: s = "invalid EmbeddedStatement"; break;
			case 284: s = "invalid EmbeddedStatement"; break;
			case 285: s = "invalid EmbeddedStatement"; break;
			case 286: s = "invalid EmbeddedStatement"; break;
			case 287: s = "invalid EmbeddedStatement"; break;
			case 288: s = "invalid FromOrAggregateQueryOperator"; break;
			case 289: s = "invalid QueryOperator"; break;
			case 290: s = "invalid PartitionQueryOperator"; break;
			case 291: s = "invalid Argument"; break;
			case 292: s = "invalid QualIdentAndTypeArguments"; break;
			case 293: s = "invalid AttributeArguments"; break;
			case 294: s = "invalid AttributeArguments"; break;
			case 295: s = "invalid AttributeArguments"; break;
			case 296: s = "invalid ParameterModifier"; break;
			case 297: s = "invalid Statement"; break;
			case 298: s = "invalid LabelName"; break;
			case 299: s = "invalid WhileOrUntil"; break;
			case 300: s = "invalid SingleLineStatementList"; break;
			case 301: s = "invalid SingleLineStatementList"; break;
			case 302: s = "invalid OnErrorStatement"; break;
			case 303: s = "invalid ResumeStatement"; break;
			case 304: s = "invalid CaseClause"; break;
			case 305: s = "invalid CaseClause"; break;

			default: s = "error " + errorNumber; break;
		}
		this.Errors.Error(line, col, s);
	}
	
	private bool StartOf(int s)
	{
		return set[s, lexer.LookAhead.kind];
	}
	
	static bool[,] set = {
	{T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,T, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, x,T,T,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, x,T,T,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, x,T,T,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, T,T,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, x,x,x,x, T,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,T, x,T,T,x, x,x,x,x, x,x,x,x, x,T,x,T, x,x,T,x, x,x,x,x, x,x,x,x, x,T,T,T, x,x,x,T, x,x,T,x, T,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,T,x,x, T,x,x,x, x,x,T,x, T,x,T,x, x,T,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, T,T,x,x, x,x,T,T, x,x,T,T, x,x,x,x, x,x,x,T, T,T,T,T, x,x,x,x, T,x,x,x, x,x,x,x, x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,T, x,T,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,T,T, x,x,x,T, x,x,T,x, T,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,T,x,x, T,x,x,x, x,x,T,x, T,x,T,x, x,T,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, T,T,x,x, x,x,T,x, x,x,T,T, x,x,x,x, x,x,x,T, T,T,T,T, x,x,x,x, T,x,x,x, x,x,x,x, x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,T, x,x,x,x, x,x,x,x, x,T,x,x, T,T,T,T, T,x,T,x, x,x,x,x, x,x,T,T, x,x,T,x, T,x,x,x, T,T,T,x, x,x,x,x, T,x,x,x, x,x,T,x, x,T,T,x, x,T,x,x, x,x,x,x, x,x,T,T, T,x,x,x, T,x,x,x, x,T,T,x, x,T,x,T, x,x,x,T, x,T,T,T, x,T,T,T, T,T,T,x, T,x,x,x, x,x,x,x, x,T,T,x, x,T,x,x, x,x,x,T, T,x,T,T, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, T,x,T,x, T,T,x,x, x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, x,T,T,T, x,T,x,T, x,T,T,x, T,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,T,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, T,x,x,x, x,x,x,x, x,x,T,x, x,x,T,x, x,x,x,x, T,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, x,T,T,T, x,T,x,T, T,T,T,x, T,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, T,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, x,T,T,T, x,T,x,T, T,T,T,x, T,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
	{x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,x, x,T,T,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,T,x,T, T,T,T,T, x,T,T,x, x,x,x,T, T,T,T,T, T,T,T,x, T,T,T,x, x,T,T,T, T,T,T,T, T,T,T,T, x,x,x,T, x,T,T,x, T,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,x,x, T,T,x,x, T,x,T,x, x,x,T,x, T,x,T,x, x,T,x,x, x,x,T,x, T,x,x,x, x,T,T,x, x,T,x,T, T,x,x,T, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, T,T,x,x, x,x,T,x, x,x,T,T, x,x,x,T, x,T,T,T, T,T,T,T, x,x,x,x, T,x,x,x, x,x,x,x, x},
	{x,T,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,x,x, x,T,x,T, T,T,T,T, x,T,T,x, T,x,x,T, T,T,T,T, T,T,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, x,x,x,T, T,T,T,T, T,x,x,x, x,x,x,T, T,T,x,T, T,T,x,T, x,T,x,x, T,T,x,T, T,x,T,x, x,x,T,x, T,x,T,x, x,T,x,x, x,x,T,x, T,x,x,x, x,T,T,x, x,T,x,x, T,x,x,T, x,T,T,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,T,x,T, T,T,T,T, x,x,x,T, T,T,T,x, T,x,T,x, x,T,T,T, x,T,x,T, T,T,T,T, T,T,T,T, T,x,x,x, T,T,x,T, x,x,x,x, x},
	{x,T,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,T,T,T, T,T,T,T, T,T,T,T, x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,T, T,T,x,T, T,T,T,T, T,T,T,T, x,T,T,x, T,x,x,T, T,T,T,T, T,T,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, x,x,x,T, T,T,T,T, T,x,T,x, T,x,x,T, T,T,x,T, T,T,x,T, x,T,x,x, T,T,x,T, T,x,T,T, x,x,T,x, T,x,T,T, T,T,x,T, x,T,T,x, T,T,x,x, x,T,T,x, x,T,x,x, T,x,x,T, x,T,T,x, x,x,T,T, T,x,x,x, x,x,T,x, x,x,x,T, x,T,x,T, T,T,T,T, x,x,x,T, T,T,T,T, T,x,T,x, x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x,x, T,T,x,T, x,x,T,x, x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
	{x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,x, x,T,T,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,T,x,T, T,T,T,T, x,T,T,x, x,x,x,T, T,T,T,T, T,T,T,x, T,T,T,x, x,T,T,T, T,T,T,T, T,T,T,T, x,x,x,T, x,T,T,x, T,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,T,x, T,T,x,x, T,x,T,x, x,x,T,x, T,x,T,x, x,T,x,x, x,x,T,x, T,x,x,x, x,T,T,x, x,T,x,T, T,x,x,T, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, T,T,x,x, x,x,T,x, T,x,T,T, x,x,x,T, x,T,T,T, T,T,T,T, x,x,x,x, T,x,x,x, x,x,x,x, x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x},
	{x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,T,x,T, T,T,T,T, x,T,T,x, x,x,x,T, T,T,T,T, T,T,T,x, T,T,T,x, x,T,T,T, T,T,T,T, T,T,T,T, x,x,x,T, x,T,T,x, T,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,x,x, T,T,x,x, T,x,T,x, x,x,T,x, T,x,T,x, x,T,x,x, x,x,T,x, T,x,x,x, x,T,T,x, x,T,x,x, T,x,x,T, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, T,T,x,x, x,x,T,x, x,x,T,T, x,x,x,T, x,T,T,T, T,T,T,T, x,x,x,x, T,x,x,x, x,x,x,x, x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
	{x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,x, x,T,T,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,T,x,T, T,T,T,T, x,T,T,x, x,x,x,T, T,T,T,T, T,T,T,x, T,T,T,x, x,T,T,T, T,T,T,T, T,T,T,T, x,x,x,T, x,T,T,x, T,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,x,x, T,T,x,x, T,x,T,x, x,x,T,x, T,x,T,x, x,T,x,x, x,x,T,x, T,x,x,x, x,T,T,x, x,T,x,x, T,x,x,T, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, T,T,x,x, x,x,T,x, x,x,T,T, x,x,x,T, x,T,T,T, T,T,T,T, x,x,x,x, T,x,x,x, x,x,x,x, x},
	{x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,x, x,T,T,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,x,x, x,T,x,T, T,T,T,T, x,T,T,x, T,x,x,T, T,T,T,T, T,T,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, x,x,x,T, T,T,T,T, T,x,x,x, x,x,x,T, T,T,x,T, T,T,x,T, x,T,T,x, T,T,x,T, T,x,T,x, x,x,T,x, T,x,T,x, x,T,x,x, x,x,T,x, T,x,x,x, x,T,T,x, x,T,x,T, T,x,x,T, x,T,T,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,T,x,T, T,T,T,T, x,x,x,T, T,T,T,x, T,x,T,x, T,T,T,T, x,T,x,T, T,T,T,T, T,T,T,T, T,x,x,x, T,T,x,T, x,x,x,x, x},
	{x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,T,T,x, x,T,T,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,T,x,T, T,T,T,T, x,T,T,x, x,x,x,T, T,T,T,T, T,T,T,x, T,T,T,x, x,T,T,T, T,T,T,T, T,T,T,T, x,x,x,T, x,T,T,x, T,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,T,x, T,T,x,x, T,x,T,x, x,x,T,x, T,x,T,x, x,T,x,x, x,x,T,x, T,x,x,x, x,T,T,x, x,T,x,T, T,x,x,T, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, T,T,x,x, x,x,T,x, T,x,T,T, x,x,x,T, x,T,T,T, T,T,T,T, x,x,x,x, T,x,x,x, x,x,x,x, x},
	{x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,T,x,T, T,T,T,T, x,T,T,x, x,x,x,T, T,T,T,T, T,T,T,x, T,T,T,x, x,T,T,T, T,T,T,T, T,T,T,T, x,x,x,T, x,T,T,x, T,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,x,x, T,T,x,x, T,x,T,x, x,x,T,x, T,x,T,x, x,T,x,x, x,x,T,x, T,x,x,x, x,T,T,x, x,T,x,x, T,x,x,T, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, T,T,x,x, x,x,T,x, x,x,T,T, x,x,x,T, x,T,T,T, T,T,T,T, x,x,x,x, T,x,x,x, x,x,x,x, x},
	{x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,x,x, x,T,x,T, T,T,T,T, x,T,T,x, T,x,x,T, T,T,T,T, T,T,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, x,x,x,T, T,T,T,T, T,x,x,x, T,x,x,T, T,T,x,T, T,T,x,T, x,T,x,x, T,T,x,T, T,x,T,x, x,x,T,x, T,x,T,x, x,T,x,x, x,x,T,x, T,x,x,x, x,T,T,x, x,T,x,x, T,x,x,T, x,T,T,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,T,x,T, T,T,T,T, x,x,x,T, T,T,T,x, T,x,T,x, x,T,T,T, x,T,x,T, T,T,T,T, T,T,T,T, T,x,x,x, T,T,x,T, x,x,x,x, x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
	{x,T,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,T,T,T, T,T,T,T, T,T,T,T, x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,T, T,T,x,T, T,T,x,T, T,T,T,T, x,T,T,x, T,x,x,T, T,T,T,T, T,T,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, x,x,x,T, T,T,T,T, T,x,T,x, T,x,x,T, T,T,x,T, T,T,x,T, x,T,x,x, T,T,x,T, T,x,T,T, x,x,T,x, T,x,T,T, T,T,x,T, x,T,T,x, T,T,x,x, x,T,T,x, x,T,x,x, T,x,x,T, x,T,T,x, x,x,T,T, T,x,x,x, x,x,T,x, x,x,x,T, x,T,x,T, T,T,T,T, x,x,x,T, T,T,T,T, T,x,T,x, x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x,x, T,T,x,T, x,x,T,x, x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, T,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
	{x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,x,x, x,T,x,T, T,T,T,T, x,T,T,x, T,x,x,T, T,T,T,T, T,T,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, x,x,x,T, T,T,T,T, T,x,x,x, x,x,x,T, T,T,x,T, T,T,x,T, x,T,x,x, T,T,x,T, T,x,T,x, x,x,T,x, T,x,T,x, x,T,x,x, x,x,T,x, T,x,x,x, x,T,T,x, x,T,x,x, T,x,x,T, x,T,T,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,T,x,T, T,T,T,T, x,x,x,T, T,T,T,x, T,x,T,x, x,T,T,T, x,T,x,T, T,T,T,T, T,T,T,T, T,x,x,x, T,T,x,T, x,x,x,x, x},
	{x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x}

	};
} // end Parser

}