
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
	const int maxT = 238;

	const  bool   T            = true;
	const  bool   x            = false;
	

#line  12 "VBNET.ATG" 


/*

*/

	void VBNET() {

#line  262 "VBNET.ATG" 
		lexer.NextToken(); // get the first token
		compilationUnit = new CompilationUnit();
		BlockStart(compilationUnit);
		
		while (la.kind == 1 || la.kind == 22) {
			EndOfStmt();
		}
		while (la.kind == 174) {
			OptionStmt();
			while (la.kind == 1 || la.kind == 22) {
				EndOfStmt();
			}
		}
		while (la.kind == 138) {
			ImportsStmt();
			while (la.kind == 1 || la.kind == 22) {
				EndOfStmt();
			}
		}
		while (
#line  270 "VBNET.ATG" 
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
		} else SynErr(239);
	}

	void OptionStmt() {

#line  275 "VBNET.ATG" 
		INode node = null; bool val = true; 
		Expect(174);

#line  276 "VBNET.ATG" 
		Location startPos = t.Location; 
		if (la.kind == 122) {
			lexer.NextToken();
			if (la.kind == 171 || la.kind == 172) {
				OptionValue(
#line  278 "VBNET.ATG" 
ref val);
			}

#line  279 "VBNET.ATG" 
			node = new OptionDeclaration(OptionType.Explicit, val); 
		} else if (la.kind == 207) {
			lexer.NextToken();
			if (la.kind == 171 || la.kind == 172) {
				OptionValue(
#line  281 "VBNET.ATG" 
ref val);
			}

#line  282 "VBNET.ATG" 
			node = new OptionDeclaration(OptionType.Strict, val); 
		} else if (la.kind == 88) {
			lexer.NextToken();
			if (la.kind == 68) {
				lexer.NextToken();

#line  284 "VBNET.ATG" 
				node = new OptionDeclaration(OptionType.CompareBinary, val); 
			} else if (la.kind == 213) {
				lexer.NextToken();

#line  285 "VBNET.ATG" 
				node = new OptionDeclaration(OptionType.CompareText, val); 
			} else SynErr(240);
		} else if (la.kind == 140) {
			lexer.NextToken();
			if (la.kind == 171 || la.kind == 172) {
				OptionValue(
#line  288 "VBNET.ATG" 
ref val);
			}

#line  289 "VBNET.ATG" 
			node = new OptionDeclaration(OptionType.Infer, val); 
		} else SynErr(241);
		EndOfStmt();

#line  293 "VBNET.ATG" 
		if (node != null) {
		node.StartLocation = startPos;
		node.EndLocation   = t.Location;
		AddChild(node);
		}
		
	}

	void ImportsStmt() {

#line  316 "VBNET.ATG" 
		List<Using> usings = new List<Using>();
		
		Expect(138);

#line  320 "VBNET.ATG" 
		Location startPos = t.Location;
		Using u;
		
		ImportClause(
#line  323 "VBNET.ATG" 
out u);

#line  323 "VBNET.ATG" 
		if (u != null) { usings.Add(u); } 
		while (la.kind == 23) {
			lexer.NextToken();
			ImportClause(
#line  325 "VBNET.ATG" 
out u);

#line  325 "VBNET.ATG" 
			if (u != null) { usings.Add(u); } 
		}
		EndOfStmt();

#line  329 "VBNET.ATG" 
		UsingDeclaration usingDeclaration = new UsingDeclaration(usings);
		usingDeclaration.StartLocation = startPos;
		usingDeclaration.EndLocation   = t.Location;
		AddChild(usingDeclaration);
		
	}

	void GlobalAttributeSection() {
		Expect(41);

#line  2653 "VBNET.ATG" 
		Location startPos = t.Location; 
		if (la.kind == 66) {
			lexer.NextToken();
		} else if (la.kind == 156) {
			lexer.NextToken();
		} else SynErr(242);

#line  2655 "VBNET.ATG" 
		string attributeTarget = t.val != null ? t.val.ToLower(System.Globalization.CultureInfo.InvariantCulture) : null;
		List<ASTAttribute> attributes = new List<ASTAttribute>();
		ASTAttribute attribute;
		
		Expect(22);
		Attribute(
#line  2659 "VBNET.ATG" 
out attribute);

#line  2659 "VBNET.ATG" 
		attributes.Add(attribute); 
		while (
#line  2660 "VBNET.ATG" 
NotFinalComma()) {
			if (la.kind == 23) {
				lexer.NextToken();
				if (la.kind == 66) {
					lexer.NextToken();
				} else if (la.kind == 156) {
					lexer.NextToken();
				} else SynErr(243);
				Expect(22);
			}
			Attribute(
#line  2660 "VBNET.ATG" 
out attribute);

#line  2660 "VBNET.ATG" 
			attributes.Add(attribute); 
		}
		if (la.kind == 23) {
			lexer.NextToken();
		}
		Expect(40);
		EndOfStmt();

#line  2665 "VBNET.ATG" 
		AttributeSection section = new AttributeSection {
		AttributeTarget = attributeTarget,
		Attributes = attributes,
		StartLocation = startPos,
		EndLocation = t.EndLocation
		};
		AddChild(section);
		
	}

	void NamespaceMemberDecl() {

#line  362 "VBNET.ATG" 
		ModifierList m = new ModifierList();
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		string qualident;
		
		if (la.kind == 161) {
			lexer.NextToken();

#line  369 "VBNET.ATG" 
			Location startPos = t.Location;
			
			Qualident(
#line  371 "VBNET.ATG" 
out qualident);

#line  373 "VBNET.ATG" 
			INode node =  new NamespaceDeclaration(qualident);
			node.StartLocation = startPos;
			AddChild(node);
			BlockStart(node);
			
			EndOfStmt();
			NamespaceBody();

#line  381 "VBNET.ATG" 
			node.EndLocation = t.Location;
			BlockEnd();
			
		} else if (StartOf(2)) {
			while (la.kind == 41) {
				AttributeSection(
#line  385 "VBNET.ATG" 
out section);

#line  385 "VBNET.ATG" 
				attributes.Add(section); 
			}
			while (StartOf(3)) {
				TypeModifier(
#line  386 "VBNET.ATG" 
m);
			}
			NonModuleDeclaration(
#line  386 "VBNET.ATG" 
m, attributes);
		} else SynErr(244);
	}

	void OptionValue(
#line  301 "VBNET.ATG" 
ref bool val) {
		if (la.kind == 172) {
			lexer.NextToken();

#line  303 "VBNET.ATG" 
			val = true; 
		} else if (la.kind == 171) {
			lexer.NextToken();

#line  305 "VBNET.ATG" 
			val = false; 
		} else SynErr(245);
	}

	void ImportClause(
#line  336 "VBNET.ATG" 
out Using u) {

#line  338 "VBNET.ATG" 
		string qualident  = null;
		TypeReference aliasedType = null;
		u = null;
		
		if (StartOf(4)) {
			Qualident(
#line  343 "VBNET.ATG" 
out qualident);
			if (la.kind == 21) {
				lexer.NextToken();
				TypeName(
#line  344 "VBNET.ATG" 
out aliasedType);
			}

#line  346 "VBNET.ATG" 
			if (qualident != null && qualident.Length > 0) {
			if (aliasedType != null) {
				u = new Using(qualident, aliasedType);
			} else {
				u = new Using(qualident);
			}
			}
			
		} else if (la.kind == 10) {

#line  354 "VBNET.ATG" 
			string prefix = null; 
			lexer.NextToken();
			Identifier();

#line  355 "VBNET.ATG" 
			prefix = t.val; 
			Expect(21);
			Expect(3);

#line  355 "VBNET.ATG" 
			u = new Using(t.literalValue as string, prefix); 
			Expect(11);
		} else SynErr(246);
	}

	void Qualident(
#line  3409 "VBNET.ATG" 
out string qualident) {

#line  3411 "VBNET.ATG" 
		string name;
		qualidentBuilder.Length = 0; 
		
		Identifier();

#line  3415 "VBNET.ATG" 
		qualidentBuilder.Append(t.val); 
		while (
#line  3416 "VBNET.ATG" 
DotAndIdentOrKw()) {
			Expect(27);
			IdentifierOrKeyword(
#line  3416 "VBNET.ATG" 
out name);

#line  3416 "VBNET.ATG" 
			qualidentBuilder.Append('.'); qualidentBuilder.Append(name); 
		}

#line  3418 "VBNET.ATG" 
		qualident = qualidentBuilder.ToString(); 
	}

	void TypeName(
#line  2526 "VBNET.ATG" 
out TypeReference typeref) {

#line  2527 "VBNET.ATG" 
		ArrayList rank = null; 
		NonArrayTypeName(
#line  2529 "VBNET.ATG" 
out typeref, false);
		ArrayTypeModifiers(
#line  2533 "VBNET.ATG" 
out rank);

#line  2534 "VBNET.ATG" 
		if (rank != null && typeref != null) {
		typeref.RankSpecifier = (int[])rank.ToArray(typeof(int));
		}
		
	}

	void Identifier() {
		if (StartOf(5)) {
			IdentifierForFieldDeclaration();
		} else if (la.kind == 99) {
			lexer.NextToken();
		} else SynErr(247);
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
		Expect(114);
		Expect(161);
		EndOfStmt();
	}

	void AttributeSection(
#line  2728 "VBNET.ATG" 
out AttributeSection section) {

#line  2730 "VBNET.ATG" 
		string attributeTarget = "";List<ASTAttribute> attributes = new List<ASTAttribute>();
		ASTAttribute attribute;
		
		
		Expect(41);

#line  2734 "VBNET.ATG" 
		Location startPos = t.Location; 
		if (
#line  2735 "VBNET.ATG" 
IsLocalAttrTarget()) {
			if (la.kind == 120) {
				lexer.NextToken();

#line  2736 "VBNET.ATG" 
				attributeTarget = "event";
			} else if (la.kind == 195) {
				lexer.NextToken();

#line  2737 "VBNET.ATG" 
				attributeTarget = "return";
			} else {
				Identifier();

#line  2740 "VBNET.ATG" 
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
#line  2750 "VBNET.ATG" 
out attribute);

#line  2750 "VBNET.ATG" 
		attributes.Add(attribute); 
		while (
#line  2751 "VBNET.ATG" 
NotFinalComma()) {
			Expect(23);
			Attribute(
#line  2751 "VBNET.ATG" 
out attribute);

#line  2751 "VBNET.ATG" 
			attributes.Add(attribute); 
		}
		if (la.kind == 23) {
			lexer.NextToken();
		}
		Expect(40);

#line  2755 "VBNET.ATG" 
		section = new AttributeSection {
		AttributeTarget = attributeTarget,
		Attributes = attributes,
		StartLocation = startPos,
		EndLocation = t.EndLocation
		};
		
	}

	void TypeModifier(
#line  3493 "VBNET.ATG" 
ModifierList m) {
		switch (la.kind) {
		case 188: {
			lexer.NextToken();

#line  3494 "VBNET.ATG" 
			m.Add(Modifiers.Public, t.Location); 
			break;
		}
		case 187: {
			lexer.NextToken();

#line  3495 "VBNET.ATG" 
			m.Add(Modifiers.Protected, t.Location); 
			break;
		}
		case 126: {
			lexer.NextToken();

#line  3496 "VBNET.ATG" 
			m.Add(Modifiers.Internal, t.Location); 
			break;
		}
		case 185: {
			lexer.NextToken();

#line  3497 "VBNET.ATG" 
			m.Add(Modifiers.Private, t.Location); 
			break;
		}
		case 200: {
			lexer.NextToken();

#line  3498 "VBNET.ATG" 
			m.Add(Modifiers.Static, t.Location); 
			break;
		}
		case 199: {
			lexer.NextToken();

#line  3499 "VBNET.ATG" 
			m.Add(Modifiers.New, t.Location); 
			break;
		}
		case 157: {
			lexer.NextToken();

#line  3500 "VBNET.ATG" 
			m.Add(Modifiers.Abstract, t.Location); 
			break;
		}
		case 167: {
			lexer.NextToken();

#line  3501 "VBNET.ATG" 
			m.Add(Modifiers.Sealed, t.Location); 
			break;
		}
		case 183: {
			lexer.NextToken();

#line  3502 "VBNET.ATG" 
			m.Add(Modifiers.Partial, t.Location); 
			break;
		}
		default: SynErr(248); break;
		}
	}

	void NonModuleDeclaration(
#line  446 "VBNET.ATG" 
ModifierList m, List<AttributeSection> attributes) {

#line  448 "VBNET.ATG" 
		TypeReference typeRef = null;
		List<TypeReference> baseInterfaces = null;
		
		switch (la.kind) {
		case 85: {

#line  451 "VBNET.ATG" 
			m.Check(Modifiers.Classes); 
			lexer.NextToken();

#line  454 "VBNET.ATG" 
			TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
			newType.StartLocation = t.Location;
			AddChild(newType);
			BlockStart(newType);
			
			newType.Type       = ClassType.Class;
			
			Identifier();

#line  461 "VBNET.ATG" 
			newType.Name = t.val; 
			TypeParameterList(
#line  462 "VBNET.ATG" 
newType.Templates);
			EndOfStmt();

#line  464 "VBNET.ATG" 
			newType.BodyStartLocation = t.Location; 
			if (la.kind == 141) {
				ClassBaseType(
#line  465 "VBNET.ATG" 
out typeRef);

#line  465 "VBNET.ATG" 
				SafeAdd(newType, newType.BaseTypes, typeRef); 
			}
			while (la.kind == 137) {
				TypeImplementsClause(
#line  466 "VBNET.ATG" 
out baseInterfaces);

#line  466 "VBNET.ATG" 
				newType.BaseTypes.AddRange(baseInterfaces); 
			}
			ClassBody(
#line  467 "VBNET.ATG" 
newType);
			Expect(114);
			Expect(85);

#line  468 "VBNET.ATG" 
			newType.EndLocation = t.EndLocation; 
			EndOfStmt();

#line  471 "VBNET.ATG" 
			BlockEnd();
			
			break;
		}
		case 156: {
			lexer.NextToken();

#line  475 "VBNET.ATG" 
			m.Check(Modifiers.VBModules);
			TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
			AddChild(newType);
			BlockStart(newType);
			newType.StartLocation = m.GetDeclarationLocation(t.Location);
			newType.Type = ClassType.Module;
			
			Identifier();

#line  482 "VBNET.ATG" 
			newType.Name = t.val; 
			EndOfStmt();

#line  484 "VBNET.ATG" 
			newType.BodyStartLocation = t.Location; 
			ModuleBody(
#line  485 "VBNET.ATG" 
newType);

#line  487 "VBNET.ATG" 
			BlockEnd();
			
			break;
		}
		case 209: {
			lexer.NextToken();

#line  491 "VBNET.ATG" 
			m.Check(Modifiers.VBStructures);
			TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
			AddChild(newType);
			BlockStart(newType);
			newType.StartLocation = m.GetDeclarationLocation(t.Location);
			newType.Type = ClassType.Struct;
			
			Identifier();

#line  498 "VBNET.ATG" 
			newType.Name = t.val; 
			TypeParameterList(
#line  499 "VBNET.ATG" 
newType.Templates);
			EndOfStmt();

#line  501 "VBNET.ATG" 
			newType.BodyStartLocation = t.Location; 
			while (la.kind == 137) {
				TypeImplementsClause(
#line  502 "VBNET.ATG" 
out baseInterfaces);

#line  502 "VBNET.ATG" 
				newType.BaseTypes.AddRange(baseInterfaces);
			}
			StructureBody(
#line  503 "VBNET.ATG" 
newType);

#line  505 "VBNET.ATG" 
			BlockEnd();
			
			break;
		}
		case 116: {
			lexer.NextToken();

#line  510 "VBNET.ATG" 
			m.Check(Modifiers.VBEnums);
			TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
			newType.StartLocation = m.GetDeclarationLocation(t.Location);
			AddChild(newType);
			BlockStart(newType);
			
			newType.Type = ClassType.Enum;
			
			Identifier();

#line  518 "VBNET.ATG" 
			newType.Name = t.val; 
			if (la.kind == 64) {
				lexer.NextToken();
				NonArrayTypeName(
#line  519 "VBNET.ATG" 
out typeRef, false);

#line  519 "VBNET.ATG" 
				SafeAdd(newType, newType.BaseTypes, typeRef); 
			}
			EndOfStmt();

#line  521 "VBNET.ATG" 
			newType.BodyStartLocation = t.Location; 
			EnumBody(
#line  522 "VBNET.ATG" 
newType);

#line  524 "VBNET.ATG" 
			BlockEnd();
			
			break;
		}
		case 143: {
			lexer.NextToken();

#line  529 "VBNET.ATG" 
			m.Check(Modifiers.VBInterfacs);
			TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
			newType.StartLocation = m.GetDeclarationLocation(t.Location);
			AddChild(newType);
			BlockStart(newType);
			newType.Type = ClassType.Interface;
			
			Identifier();

#line  536 "VBNET.ATG" 
			newType.Name = t.val; 
			TypeParameterList(
#line  537 "VBNET.ATG" 
newType.Templates);
			EndOfStmt();

#line  539 "VBNET.ATG" 
			newType.BodyStartLocation = t.Location; 
			while (la.kind == 141) {
				InterfaceBase(
#line  540 "VBNET.ATG" 
out baseInterfaces);

#line  540 "VBNET.ATG" 
				newType.BaseTypes.AddRange(baseInterfaces); 
			}
			InterfaceBody(
#line  541 "VBNET.ATG" 
newType);

#line  543 "VBNET.ATG" 
			BlockEnd();
			
			break;
		}
		case 104: {
			lexer.NextToken();

#line  548 "VBNET.ATG" 
			m.Check(Modifiers.VBDelegates);
			DelegateDeclaration delegateDeclr = new DelegateDeclaration(m.Modifier, attributes);
			delegateDeclr.ReturnType = new TypeReference("System.Void", true);
			delegateDeclr.StartLocation = m.GetDeclarationLocation(t.Location);
			List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
			
			if (la.kind == 210) {
				lexer.NextToken();
				Identifier();

#line  555 "VBNET.ATG" 
				delegateDeclr.Name = t.val; 
				TypeParameterList(
#line  556 "VBNET.ATG" 
delegateDeclr.Templates);
				if (la.kind == 38) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
#line  557 "VBNET.ATG" 
p);
					}
					Expect(39);

#line  557 "VBNET.ATG" 
					delegateDeclr.Parameters = p; 
				}
			} else if (la.kind == 128) {
				lexer.NextToken();
				Identifier();

#line  559 "VBNET.ATG" 
				delegateDeclr.Name = t.val; 
				TypeParameterList(
#line  560 "VBNET.ATG" 
delegateDeclr.Templates);
				if (la.kind == 38) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
#line  561 "VBNET.ATG" 
p);
					}
					Expect(39);

#line  561 "VBNET.ATG" 
					delegateDeclr.Parameters = p; 
				}
				if (la.kind == 64) {
					lexer.NextToken();

#line  562 "VBNET.ATG" 
					TypeReference type; 
					TypeName(
#line  562 "VBNET.ATG" 
out type);

#line  562 "VBNET.ATG" 
					delegateDeclr.ReturnType = type; 
				}
			} else SynErr(249);

#line  564 "VBNET.ATG" 
			delegateDeclr.EndLocation = t.EndLocation; 
			EndOfStmt();

#line  567 "VBNET.ATG" 
			AddChild(delegateDeclr);
			
			break;
		}
		default: SynErr(250); break;
		}
	}

	void TypeParameterList(
#line  390 "VBNET.ATG" 
List<TemplateDefinition> templates) {

#line  392 "VBNET.ATG" 
		TemplateDefinition template;
		
		if (
#line  396 "VBNET.ATG" 
la.kind == Tokens.OpenParenthesis && Peek(1).kind == Tokens.Of) {
			lexer.NextToken();
			Expect(170);
			TypeParameter(
#line  397 "VBNET.ATG" 
out template);

#line  399 "VBNET.ATG" 
			if (template != null) templates.Add(template);
			
			while (la.kind == 23) {
				lexer.NextToken();
				TypeParameter(
#line  402 "VBNET.ATG" 
out template);

#line  404 "VBNET.ATG" 
				if (template != null) templates.Add(template);
				
			}
			Expect(39);
		}
	}

	void TypeParameter(
#line  412 "VBNET.ATG" 
out TemplateDefinition template) {
		Identifier();

#line  414 "VBNET.ATG" 
		template = new TemplateDefinition(t.val, null); 
		if (la.kind == 64) {
			TypeParameterConstraints(
#line  415 "VBNET.ATG" 
template);
		}
	}

	void TypeParameterConstraints(
#line  419 "VBNET.ATG" 
TemplateDefinition template) {

#line  421 "VBNET.ATG" 
		TypeReference constraint;
		
		Expect(64);
		if (la.kind == 36) {
			lexer.NextToken();
			TypeParameterConstraint(
#line  427 "VBNET.ATG" 
out constraint);

#line  427 "VBNET.ATG" 
			if (constraint != null) { template.Bases.Add(constraint); } 
			while (la.kind == 23) {
				lexer.NextToken();
				TypeParameterConstraint(
#line  430 "VBNET.ATG" 
out constraint);

#line  430 "VBNET.ATG" 
				if (constraint != null) { template.Bases.Add(constraint); } 
			}
			Expect(37);
		} else if (StartOf(7)) {
			TypeParameterConstraint(
#line  433 "VBNET.ATG" 
out constraint);

#line  433 "VBNET.ATG" 
			if (constraint != null) { template.Bases.Add(constraint); } 
		} else SynErr(251);
	}

	void TypeParameterConstraint(
#line  437 "VBNET.ATG" 
out TypeReference constraint) {

#line  438 "VBNET.ATG" 
		constraint = null; 
		if (la.kind == 85) {
			lexer.NextToken();

#line  439 "VBNET.ATG" 
			constraint = TypeReference.ClassConstraint; 
		} else if (la.kind == 209) {
			lexer.NextToken();

#line  440 "VBNET.ATG" 
			constraint = TypeReference.StructConstraint; 
		} else if (la.kind == 163) {
			lexer.NextToken();

#line  441 "VBNET.ATG" 
			constraint = TypeReference.NewConstraint; 
		} else if (StartOf(8)) {
			TypeName(
#line  442 "VBNET.ATG" 
out constraint);
		} else SynErr(252);
	}

	void ClassBaseType(
#line  787 "VBNET.ATG" 
out TypeReference typeRef) {

#line  789 "VBNET.ATG" 
		typeRef = null;
		
		Expect(141);
		TypeName(
#line  792 "VBNET.ATG" 
out typeRef);
		EndOfStmt();
	}

	void TypeImplementsClause(
#line  1604 "VBNET.ATG" 
out List<TypeReference> baseInterfaces) {

#line  1606 "VBNET.ATG" 
		baseInterfaces = new List<TypeReference>();
		TypeReference type = null;
		
		Expect(137);
		TypeName(
#line  1609 "VBNET.ATG" 
out type);

#line  1611 "VBNET.ATG" 
		if (type != null) baseInterfaces.Add(type);
		
		while (la.kind == 23) {
			lexer.NextToken();
			TypeName(
#line  1614 "VBNET.ATG" 
out type);

#line  1615 "VBNET.ATG" 
			if (type != null) baseInterfaces.Add(type); 
		}
		EndOfStmt();
	}

	void ClassBody(
#line  581 "VBNET.ATG" 
TypeDeclaration newType) {

#line  582 "VBNET.ATG" 
		AttributeSection section; 
		while (la.kind == 1 || la.kind == 22) {
			EndOfStmt();
		}
		while (StartOf(9)) {

#line  585 "VBNET.ATG" 
			List<AttributeSection> attributes = new List<AttributeSection>();
			ModifierList m = new ModifierList();
			
			while (la.kind == 41) {
				AttributeSection(
#line  588 "VBNET.ATG" 
out section);

#line  588 "VBNET.ATG" 
				attributes.Add(section); 
			}
			while (StartOf(10)) {
				MemberModifier(
#line  589 "VBNET.ATG" 
m);
			}
			ClassMemberDecl(
#line  590 "VBNET.ATG" 
m, attributes);
			while (la.kind == 1 || la.kind == 22) {
				EndOfStmt();
			}
		}
	}

	void ModuleBody(
#line  612 "VBNET.ATG" 
TypeDeclaration newType) {

#line  613 "VBNET.ATG" 
		AttributeSection section; 
		while (la.kind == 1 || la.kind == 22) {
			EndOfStmt();
		}
		while (StartOf(9)) {

#line  616 "VBNET.ATG" 
			List<AttributeSection> attributes = new List<AttributeSection>();
			ModifierList m = new ModifierList();
			
			while (la.kind == 41) {
				AttributeSection(
#line  619 "VBNET.ATG" 
out section);

#line  619 "VBNET.ATG" 
				attributes.Add(section); 
			}
			while (StartOf(10)) {
				MemberModifier(
#line  620 "VBNET.ATG" 
m);
			}
			ClassMemberDecl(
#line  621 "VBNET.ATG" 
m, attributes);
			while (la.kind == 1 || la.kind == 22) {
				EndOfStmt();
			}
		}
		Expect(114);
		Expect(156);

#line  624 "VBNET.ATG" 
		newType.EndLocation = t.EndLocation; 
		EndOfStmt();
	}

	void StructureBody(
#line  595 "VBNET.ATG" 
TypeDeclaration newType) {

#line  596 "VBNET.ATG" 
		AttributeSection section; 
		while (la.kind == 1 || la.kind == 22) {
			EndOfStmt();
		}
		while (StartOf(9)) {

#line  599 "VBNET.ATG" 
			List<AttributeSection> attributes = new List<AttributeSection>();
			ModifierList m = new ModifierList();
			
			while (la.kind == 41) {
				AttributeSection(
#line  602 "VBNET.ATG" 
out section);

#line  602 "VBNET.ATG" 
				attributes.Add(section); 
			}
			while (StartOf(10)) {
				MemberModifier(
#line  603 "VBNET.ATG" 
m);
			}
			StructureMemberDecl(
#line  604 "VBNET.ATG" 
m, attributes);
			while (la.kind == 1 || la.kind == 22) {
				EndOfStmt();
			}
		}
		Expect(114);
		Expect(209);

#line  607 "VBNET.ATG" 
		newType.EndLocation = t.EndLocation; 
		EndOfStmt();
	}

	void NonArrayTypeName(
#line  2552 "VBNET.ATG" 
out TypeReference typeref, bool canBeUnbound) {

#line  2554 "VBNET.ATG" 
		string name;
		typeref = null;
		bool isGlobal = false;
		
		if (StartOf(11)) {
			if (la.kind == 131) {
				lexer.NextToken();
				Expect(27);

#line  2559 "VBNET.ATG" 
				isGlobal = true; 
			}
			QualIdentAndTypeArguments(
#line  2560 "VBNET.ATG" 
out typeref, canBeUnbound);

#line  2561 "VBNET.ATG" 
			typeref.IsGlobal = isGlobal; 
			while (la.kind == 27) {
				lexer.NextToken();

#line  2562 "VBNET.ATG" 
				TypeReference nestedTypeRef; 
				QualIdentAndTypeArguments(
#line  2563 "VBNET.ATG" 
out nestedTypeRef, canBeUnbound);

#line  2564 "VBNET.ATG" 
				typeref = new InnerClassTypeReference(typeref, nestedTypeRef.Type, nestedTypeRef.GenericTypes); 
			}
		} else if (la.kind == 169) {
			lexer.NextToken();

#line  2567 "VBNET.ATG" 
			typeref = new TypeReference("System.Object", true); 
			if (la.kind == 34) {
				lexer.NextToken();

#line  2571 "VBNET.ATG" 
				List<TypeReference> typeArguments = new List<TypeReference>(1);
				if (typeref != null) typeArguments.Add(typeref);
				typeref = new TypeReference("System.Nullable", typeArguments) { IsKeyword = true };
				
			}
		} else if (StartOf(12)) {
			PrimitiveTypeName(
#line  2577 "VBNET.ATG" 
out name);

#line  2577 "VBNET.ATG" 
			typeref = new TypeReference(name, true); 
			if (la.kind == 34) {
				lexer.NextToken();

#line  2581 "VBNET.ATG" 
				List<TypeReference> typeArguments = new List<TypeReference>(1);
				if (typeref != null) typeArguments.Add(typeref);
				typeref = new TypeReference("System.Nullable", typeArguments) { IsKeyword = true };
				
			}
		} else SynErr(253);
	}

	void EnumBody(
#line  628 "VBNET.ATG" 
TypeDeclaration newType) {

#line  629 "VBNET.ATG" 
		FieldDeclaration f; 
		while (la.kind == 1 || la.kind == 22) {
			EndOfStmt();
		}
		while (StartOf(13)) {
			EnumMemberDecl(
#line  632 "VBNET.ATG" 
out f);

#line  634 "VBNET.ATG" 
			AddChild(f);
			
			while (la.kind == 1 || la.kind == 22) {
				EndOfStmt();
			}
		}
		Expect(114);
		Expect(116);

#line  638 "VBNET.ATG" 
		newType.EndLocation = t.EndLocation; 
		EndOfStmt();
	}

	void InterfaceBase(
#line  1589 "VBNET.ATG" 
out List<TypeReference> bases) {

#line  1591 "VBNET.ATG" 
		TypeReference type;
		bases = new List<TypeReference>();
		
		Expect(141);
		TypeName(
#line  1595 "VBNET.ATG" 
out type);

#line  1595 "VBNET.ATG" 
		if (type != null) bases.Add(type); 
		while (la.kind == 23) {
			lexer.NextToken();
			TypeName(
#line  1598 "VBNET.ATG" 
out type);

#line  1598 "VBNET.ATG" 
			if (type != null) bases.Add(type); 
		}
		EndOfStmt();
	}

	void InterfaceBody(
#line  642 "VBNET.ATG" 
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
		Expect(114);
		Expect(143);

#line  648 "VBNET.ATG" 
		newType.EndLocation = t.EndLocation; 
		EndOfStmt();
	}

	void FormalParameterList(
#line  2765 "VBNET.ATG" 
List<ParameterDeclarationExpression> parameter) {

#line  2766 "VBNET.ATG" 
		ParameterDeclarationExpression p; 
		FormalParameter(
#line  2768 "VBNET.ATG" 
out p);

#line  2768 "VBNET.ATG" 
		if (p != null) parameter.Add(p); 
		while (la.kind == 23) {
			lexer.NextToken();
			FormalParameter(
#line  2770 "VBNET.ATG" 
out p);

#line  2770 "VBNET.ATG" 
			if (p != null) parameter.Add(p); 
		}
	}

	void MemberModifier(
#line  3505 "VBNET.ATG" 
ModifierList m) {
		switch (la.kind) {
		case 157: {
			lexer.NextToken();

#line  3506 "VBNET.ATG" 
			m.Add(Modifiers.Abstract, t.Location);
			break;
		}
		case 103: {
			lexer.NextToken();

#line  3507 "VBNET.ATG" 
			m.Add(Modifiers.Default, t.Location);
			break;
		}
		case 126: {
			lexer.NextToken();

#line  3508 "VBNET.ATG" 
			m.Add(Modifiers.Internal, t.Location);
			break;
		}
		case 199: {
			lexer.NextToken();

#line  3509 "VBNET.ATG" 
			m.Add(Modifiers.New, t.Location);
			break;
		}
		case 181: {
			lexer.NextToken();

#line  3510 "VBNET.ATG" 
			m.Add(Modifiers.Override, t.Location);
			break;
		}
		case 158: {
			lexer.NextToken();

#line  3511 "VBNET.ATG" 
			m.Add(Modifiers.Abstract, t.Location);
			break;
		}
		case 185: {
			lexer.NextToken();

#line  3512 "VBNET.ATG" 
			m.Add(Modifiers.Private, t.Location);
			break;
		}
		case 187: {
			lexer.NextToken();

#line  3513 "VBNET.ATG" 
			m.Add(Modifiers.Protected, t.Location);
			break;
		}
		case 188: {
			lexer.NextToken();

#line  3514 "VBNET.ATG" 
			m.Add(Modifiers.Public, t.Location);
			break;
		}
		case 167: {
			lexer.NextToken();

#line  3515 "VBNET.ATG" 
			m.Add(Modifiers.Sealed, t.Location);
			break;
		}
		case 168: {
			lexer.NextToken();

#line  3516 "VBNET.ATG" 
			m.Add(Modifiers.Sealed, t.Location);
			break;
		}
		case 200: {
			lexer.NextToken();

#line  3517 "VBNET.ATG" 
			m.Add(Modifiers.Static, t.Location);
			break;
		}
		case 180: {
			lexer.NextToken();

#line  3518 "VBNET.ATG" 
			m.Add(Modifiers.Virtual, t.Location);
			break;
		}
		case 179: {
			lexer.NextToken();

#line  3519 "VBNET.ATG" 
			m.Add(Modifiers.Overloads, t.Location);
			break;
		}
		case 190: {
			lexer.NextToken();

#line  3520 "VBNET.ATG" 
			m.Add(Modifiers.ReadOnly, t.Location);
			break;
		}
		case 235: {
			lexer.NextToken();

#line  3521 "VBNET.ATG" 
			m.Add(Modifiers.WriteOnly, t.Location);
			break;
		}
		case 234: {
			lexer.NextToken();

#line  3522 "VBNET.ATG" 
			m.Add(Modifiers.WithEvents, t.Location);
			break;
		}
		case 106: {
			lexer.NextToken();

#line  3523 "VBNET.ATG" 
			m.Add(Modifiers.Dim, t.Location);
			break;
		}
		case 183: {
			lexer.NextToken();

#line  3524 "VBNET.ATG" 
			m.Add(Modifiers.Partial, t.Location);
			break;
		}
		default: SynErr(254); break;
		}
	}

	void ClassMemberDecl(
#line  783 "VBNET.ATG" 
ModifierList m, List<AttributeSection> attributes) {
		StructureMemberDecl(
#line  784 "VBNET.ATG" 
m, attributes);
	}

	void StructureMemberDecl(
#line  797 "VBNET.ATG" 
ModifierList m, List<AttributeSection> attributes) {

#line  799 "VBNET.ATG" 
		TypeReference type = null;
		List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
		Statement stmt = null;
		List<VariableDeclaration> variableDeclarators = new List<VariableDeclaration>();
		List<TemplateDefinition> templates = new List<TemplateDefinition>();
		
		switch (la.kind) {
		case 85: case 104: case 116: case 143: case 156: case 209: {
			NonModuleDeclaration(
#line  806 "VBNET.ATG" 
m, attributes);
			break;
		}
		case 210: {
			lexer.NextToken();

#line  810 "VBNET.ATG" 
			Location startPos = t.Location;
			
			if (StartOf(4)) {

#line  814 "VBNET.ATG" 
				string name = String.Empty;
				MethodDeclaration methodDeclaration; List<string> handlesClause = null;
				List<InterfaceImplementation> implementsClause = null;
				
				Identifier();

#line  820 "VBNET.ATG" 
				name = t.val;
				m.Check(Modifiers.VBMethods);
				
				TypeParameterList(
#line  823 "VBNET.ATG" 
templates);
				if (la.kind == 38) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
#line  824 "VBNET.ATG" 
p);
					}
					Expect(39);
				}
				if (la.kind == 135 || la.kind == 137) {
					if (la.kind == 137) {
						ImplementsClause(
#line  827 "VBNET.ATG" 
out implementsClause);
					} else {
						HandlesClause(
#line  829 "VBNET.ATG" 
out handlesClause);
					}
				}

#line  832 "VBNET.ATG" 
				Location endLocation = t.EndLocation; 
				if (
#line  835 "VBNET.ATG" 
IsMustOverride(m)) {
					EndOfStmt();

#line  838 "VBNET.ATG" 
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

#line  851 "VBNET.ATG" 
					methodDeclaration = new MethodDeclaration {
					Name = name, Modifier = m.Modifier, Parameters = p, Attributes = attributes,
					StartLocation = m.GetDeclarationLocation(startPos), EndLocation = endLocation,
					TypeReference = new TypeReference("System.Void", true),
					Templates = templates,
					HandlesClause = handlesClause,
					InterfaceImplementations = implementsClause
					};
					AddChild(methodDeclaration);
					

#line  862 "VBNET.ATG" 
					if (ParseMethodBodies) { 
					Block(
#line  863 "VBNET.ATG" 
out stmt);
					Expect(114);
					Expect(210);

#line  865 "VBNET.ATG" 
					} else {
					// don't parse method body
					lexer.SkipCurrentBlock(Tokens.Sub); stmt = new BlockStatement();
					  }
					

#line  871 "VBNET.ATG" 
					methodDeclaration.Body  = (BlockStatement)stmt; 

#line  872 "VBNET.ATG" 
					methodDeclaration.Body.EndLocation = t.EndLocation; 
					EndOfStmt();
				} else SynErr(255);
			} else if (la.kind == 163) {
				lexer.NextToken();
				if (la.kind == 38) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
#line  876 "VBNET.ATG" 
p);
					}
					Expect(39);
				}

#line  877 "VBNET.ATG" 
				m.Check(Modifiers.Constructors); 

#line  878 "VBNET.ATG" 
				Location constructorEndLocation = t.EndLocation; 
				Expect(1);

#line  881 "VBNET.ATG" 
				if (ParseMethodBodies) { 
				Block(
#line  882 "VBNET.ATG" 
out stmt);
				Expect(114);
				Expect(210);

#line  884 "VBNET.ATG" 
				} else {
				// don't parse method body
				lexer.SkipCurrentBlock(Tokens.Sub); stmt = new BlockStatement();
				  }
				

#line  890 "VBNET.ATG" 
				Location endLocation = t.EndLocation; 
				EndOfStmt();

#line  893 "VBNET.ATG" 
				ConstructorDeclaration cd = new ConstructorDeclaration("New", m.Modifier, p, attributes);
				cd.StartLocation = m.GetDeclarationLocation(startPos);
				cd.EndLocation   = constructorEndLocation;
				cd.Body = (BlockStatement)stmt;
				cd.Body.EndLocation   = endLocation;
				AddChild(cd);
				
			} else SynErr(256);
			break;
		}
		case 128: {
			lexer.NextToken();

#line  905 "VBNET.ATG" 
			m.Check(Modifiers.VBMethods);
			string name = String.Empty;
			Location startPos = t.Location;
			MethodDeclaration methodDeclaration;List<string> handlesClause = null;
			List<InterfaceImplementation> implementsClause = null;
			AttributeSection returnTypeAttributeSection = null;
			
			Identifier();

#line  912 "VBNET.ATG" 
			name = t.val; 
			TypeParameterList(
#line  913 "VBNET.ATG" 
templates);
			if (la.kind == 38) {
				lexer.NextToken();
				if (StartOf(6)) {
					FormalParameterList(
#line  914 "VBNET.ATG" 
p);
				}
				Expect(39);
			}
			if (la.kind == 64) {
				lexer.NextToken();
				while (la.kind == 41) {
					AttributeSection(
#line  916 "VBNET.ATG" 
out returnTypeAttributeSection);

#line  918 "VBNET.ATG" 
					if (returnTypeAttributeSection != null) {
					returnTypeAttributeSection.AttributeTarget = "return";
					attributes.Add(returnTypeAttributeSection);
					}
					
				}
				TypeName(
#line  924 "VBNET.ATG" 
out type);
			}

#line  926 "VBNET.ATG" 
			if(type == null) {
			type = new TypeReference("System.Object", true);
			}
			
			if (la.kind == 135 || la.kind == 137) {
				if (la.kind == 137) {
					ImplementsClause(
#line  932 "VBNET.ATG" 
out implementsClause);
				} else {
					HandlesClause(
#line  934 "VBNET.ATG" 
out handlesClause);
				}
			}

#line  937 "VBNET.ATG" 
			Location endLocation = t.EndLocation; 
			if (
#line  940 "VBNET.ATG" 
IsMustOverride(m)) {
				EndOfStmt();

#line  943 "VBNET.ATG" 
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

#line  958 "VBNET.ATG" 
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
#line  971 "VBNET.ATG" 
out stmt);
				Expect(114);
				Expect(128);

#line  973 "VBNET.ATG" 
				} else {
				// don't parse method body
				lexer.SkipCurrentBlock(Tokens.Function); stmt = new BlockStatement();
				}
				methodDeclaration.Body = (BlockStatement)stmt;
				methodDeclaration.Body.StartLocation = methodDeclaration.EndLocation;
				methodDeclaration.Body.EndLocation   = t.EndLocation;
				
				EndOfStmt();
			} else SynErr(257);
			break;
		}
		case 102: {
			lexer.NextToken();

#line  987 "VBNET.ATG" 
			m.Check(Modifiers.VBExternalMethods);
			Location startPos = t.Location;
			CharsetModifier charsetModifer = CharsetModifier.None;
			string library = String.Empty;
			string alias = null;
			string name = String.Empty;
			
			if (StartOf(15)) {
				Charset(
#line  994 "VBNET.ATG" 
out charsetModifer);
			}
			if (la.kind == 210) {
				lexer.NextToken();
				Identifier();

#line  997 "VBNET.ATG" 
				name = t.val; 
				Expect(150);
				Expect(3);

#line  998 "VBNET.ATG" 
				library = t.literalValue as string; 
				if (la.kind == 60) {
					lexer.NextToken();
					Expect(3);

#line  999 "VBNET.ATG" 
					alias = t.literalValue as string; 
				}
				if (la.kind == 38) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
#line  1000 "VBNET.ATG" 
p);
					}
					Expect(39);
				}
				EndOfStmt();

#line  1003 "VBNET.ATG" 
				DeclareDeclaration declareDeclaration = new DeclareDeclaration(name, m.Modifier, null, p, attributes, library, alias, charsetModifer);
				declareDeclaration.StartLocation = m.GetDeclarationLocation(startPos);
				declareDeclaration.EndLocation   = t.EndLocation;
				AddChild(declareDeclaration);
				
			} else if (la.kind == 128) {
				lexer.NextToken();
				Identifier();

#line  1010 "VBNET.ATG" 
				name = t.val; 
				Expect(150);
				Expect(3);

#line  1011 "VBNET.ATG" 
				library = t.literalValue as string; 
				if (la.kind == 60) {
					lexer.NextToken();
					Expect(3);

#line  1012 "VBNET.ATG" 
					alias = t.literalValue as string; 
				}
				if (la.kind == 38) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
#line  1013 "VBNET.ATG" 
p);
					}
					Expect(39);
				}
				if (la.kind == 64) {
					lexer.NextToken();
					TypeName(
#line  1014 "VBNET.ATG" 
out type);
				}
				EndOfStmt();

#line  1017 "VBNET.ATG" 
				DeclareDeclaration declareDeclaration = new DeclareDeclaration(name, m.Modifier, type, p, attributes, library, alias, charsetModifer);
				declareDeclaration.StartLocation = m.GetDeclarationLocation(startPos);
				declareDeclaration.EndLocation   = t.EndLocation;
				AddChild(declareDeclaration);
				
			} else SynErr(258);
			break;
		}
		case 120: {
			lexer.NextToken();

#line  1027 "VBNET.ATG" 
			m.Check(Modifiers.VBEvents);
			Location startPos = t.Location;
			EventDeclaration eventDeclaration;
			string name = String.Empty;
			List<InterfaceImplementation> implementsClause = null;
			
			Identifier();

#line  1033 "VBNET.ATG" 
			name= t.val; 
			if (la.kind == 64) {
				lexer.NextToken();
				TypeName(
#line  1035 "VBNET.ATG" 
out type);
			} else if (StartOf(16)) {
				if (la.kind == 38) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
#line  1037 "VBNET.ATG" 
p);
					}
					Expect(39);
				}
			} else SynErr(259);
			if (la.kind == 137) {
				ImplementsClause(
#line  1039 "VBNET.ATG" 
out implementsClause);
			}

#line  1041 "VBNET.ATG" 
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
		case 2: case 59: case 63: case 65: case 66: case 67: case 68: case 71: case 88: case 105: case 108: case 117: case 122: case 127: case 134: case 140: case 144: case 147: case 148: case 171: case 177: case 184: case 203: case 212: case 213: case 223: case 224: case 230: {

#line  1052 "VBNET.ATG" 
			m.Check(Modifiers.Fields);
			FieldDeclaration fd = new FieldDeclaration(attributes, null, m.Modifier);
			
			IdentifierForFieldDeclaration();

#line  1055 "VBNET.ATG" 
			string name = t.val; 

#line  1056 "VBNET.ATG" 
			fd.StartLocation = m.GetDeclarationLocation(t.Location); 
			VariableDeclaratorPartAfterIdentifier(
#line  1058 "VBNET.ATG" 
variableDeclarators, name);
			while (la.kind == 23) {
				lexer.NextToken();
				VariableDeclarator(
#line  1059 "VBNET.ATG" 
variableDeclarators);
			}
			EndOfStmt();

#line  1062 "VBNET.ATG" 
			fd.EndLocation = t.EndLocation;
			fd.Fields = variableDeclarators;
			AddChild(fd);
			
			break;
		}
		case 89: {

#line  1067 "VBNET.ATG" 
			m.Check(Modifiers.Fields); 
			lexer.NextToken();

#line  1068 "VBNET.ATG" 
			m.Add(Modifiers.Const, t.Location);  

#line  1070 "VBNET.ATG" 
			FieldDeclaration fd = new FieldDeclaration(attributes, type, m.Modifier);
			fd.StartLocation = m.GetDeclarationLocation(t.Location);
			List<VariableDeclaration> constantDeclarators = new List<VariableDeclaration>();
			
			ConstantDeclarator(
#line  1074 "VBNET.ATG" 
constantDeclarators);
			while (la.kind == 23) {
				lexer.NextToken();
				ConstantDeclarator(
#line  1075 "VBNET.ATG" 
constantDeclarators);
			}

#line  1077 "VBNET.ATG" 
			fd.Fields = constantDeclarators;
			fd.EndLocation = t.Location;
			
			EndOfStmt();

#line  1082 "VBNET.ATG" 
			fd.EndLocation = t.EndLocation;
			AddChild(fd);
			
			break;
		}
		case 186: {
			lexer.NextToken();

#line  1088 "VBNET.ATG" 
			m.Check(Modifiers.VBProperties);
			Location startPos = t.Location;
			List<InterfaceImplementation> implementsClause = null;
			AttributeSection returnTypeAttributeSection = null;
			Expression initializer = null;
			
			Identifier();

#line  1094 "VBNET.ATG" 
			string propertyName = t.val; 
			if (la.kind == 38) {
				lexer.NextToken();
				if (StartOf(6)) {
					FormalParameterList(
#line  1095 "VBNET.ATG" 
p);
				}
				Expect(39);
			}
			if (la.kind == 64) {
				lexer.NextToken();
				while (la.kind == 41) {
					AttributeSection(
#line  1098 "VBNET.ATG" 
out returnTypeAttributeSection);

#line  1100 "VBNET.ATG" 
					if (returnTypeAttributeSection != null) {
					returnTypeAttributeSection.AttributeTarget = "return";
					attributes.Add(returnTypeAttributeSection);
					}
					
				}
				if (
#line  1107 "VBNET.ATG" 
IsNewExpression()) {
					ObjectCreateExpression(
#line  1107 "VBNET.ATG" 
out initializer);

#line  1109 "VBNET.ATG" 
					if (initializer is ObjectCreateExpression) {
					type = ((ObjectCreateExpression)initializer).CreateType.Clone();
					} else {
						type = ((ArrayCreateExpression)initializer).CreateType.Clone();
					}
					
				} else if (StartOf(8)) {
					TypeName(
#line  1116 "VBNET.ATG" 
out type);
				} else SynErr(260);
			}
			if (la.kind == 21) {
				lexer.NextToken();
				Expr(
#line  1119 "VBNET.ATG" 
out initializer);
			}
			if (la.kind == 137) {
				ImplementsClause(
#line  1120 "VBNET.ATG" 
out implementsClause);
			}
			EndOfStmt();
			if (
#line  1124 "VBNET.ATG" 
IsMustOverride(m) || IsAutomaticProperty()) {

#line  1126 "VBNET.ATG" 
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

#line  1138 "VBNET.ATG" 
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
#line  1148 "VBNET.ATG" 
out getRegion, out setRegion);
				Expect(114);
				Expect(186);
				EndOfStmt();

#line  1152 "VBNET.ATG" 
				pDecl.GetRegion = getRegion;
				pDecl.SetRegion = setRegion;
				pDecl.BodyEnd = t.Location; // t = EndOfStmt; not "Property"
				AddChild(pDecl);
				
			} else SynErr(261);
			break;
		}
		case 99: {
			lexer.NextToken();

#line  1159 "VBNET.ATG" 
			Location startPos = t.Location; 
			Expect(120);

#line  1161 "VBNET.ATG" 
			m.Check(Modifiers.VBCustomEvents);
			EventAddRemoveRegion eventAccessorDeclaration;
			EventAddRegion addHandlerAccessorDeclaration = null;
			EventRemoveRegion removeHandlerAccessorDeclaration = null;
			EventRaiseRegion raiseEventAccessorDeclaration = null;
			List<InterfaceImplementation> implementsClause = null;
			
			Identifier();

#line  1168 "VBNET.ATG" 
			string customEventName = t.val; 
			Expect(64);
			TypeName(
#line  1169 "VBNET.ATG" 
out type);
			if (la.kind == 137) {
				ImplementsClause(
#line  1170 "VBNET.ATG" 
out implementsClause);
			}
			EndOfStmt();
			while (StartOf(18)) {
				EventAccessorDeclaration(
#line  1173 "VBNET.ATG" 
out eventAccessorDeclaration);

#line  1175 "VBNET.ATG" 
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
			Expect(114);
			Expect(120);
			EndOfStmt();

#line  1191 "VBNET.ATG" 
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
		case 162: case 173: case 232: {

#line  1217 "VBNET.ATG" 
			ConversionType opConversionType = ConversionType.None; 
			if (la.kind == 162 || la.kind == 232) {
				if (la.kind == 232) {
					lexer.NextToken();

#line  1218 "VBNET.ATG" 
					opConversionType = ConversionType.Implicit; 
				} else {
					lexer.NextToken();

#line  1219 "VBNET.ATG" 
					opConversionType = ConversionType.Explicit;
				}
			}
			Expect(173);

#line  1222 "VBNET.ATG" 
			m.Check(Modifiers.VBOperators);
			Location startPos = t.Location;
			TypeReference returnType = NullTypeReference.Instance;
			TypeReference operandType = NullTypeReference.Instance;
			string operandName;
			OverloadableOperatorType operatorType;
			AttributeSection section;
			List<ParameterDeclarationExpression> parameters = new List<ParameterDeclarationExpression>();
			
			OverloadableOperator(
#line  1231 "VBNET.ATG" 
out operatorType);
			Expect(38);
			if (la.kind == 73) {
				lexer.NextToken();
			}
			Identifier();

#line  1232 "VBNET.ATG" 
			operandName = t.val; 
			if (la.kind == 64) {
				lexer.NextToken();
				TypeName(
#line  1233 "VBNET.ATG" 
out operandType);
			}

#line  1234 "VBNET.ATG" 
			parameters.Add(new ParameterDeclarationExpression(operandType, operandName, ParameterModifiers.In)); 
			while (la.kind == 23) {
				lexer.NextToken();
				if (la.kind == 73) {
					lexer.NextToken();
				}
				Identifier();

#line  1238 "VBNET.ATG" 
				operandName = t.val; 
				if (la.kind == 64) {
					lexer.NextToken();
					TypeName(
#line  1239 "VBNET.ATG" 
out operandType);
				}

#line  1240 "VBNET.ATG" 
				parameters.Add(new ParameterDeclarationExpression(operandType, operandName, ParameterModifiers.In)); 
			}
			Expect(39);

#line  1243 "VBNET.ATG" 
			Location endPos = t.EndLocation; 
			if (la.kind == 64) {
				lexer.NextToken();
				while (la.kind == 41) {
					AttributeSection(
#line  1244 "VBNET.ATG" 
out section);

#line  1245 "VBNET.ATG" 
					if (section != null) {
					section.AttributeTarget = "return";
					attributes.Add(section);
					} 
				}
				TypeName(
#line  1249 "VBNET.ATG" 
out returnType);

#line  1249 "VBNET.ATG" 
				endPos = t.EndLocation; 
			}
			Expect(1);
			Block(
#line  1251 "VBNET.ATG" 
out stmt);
			Expect(114);
			Expect(173);
			EndOfStmt();

#line  1253 "VBNET.ATG" 
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
		default: SynErr(262); break;
		}
	}

	void EnumMemberDecl(
#line  765 "VBNET.ATG" 
out FieldDeclaration f) {

#line  767 "VBNET.ATG" 
		Expression expr = null;List<AttributeSection> attributes = new List<AttributeSection>();
		AttributeSection section = null;
		VariableDeclaration varDecl = null;
		
		while (la.kind == 41) {
			AttributeSection(
#line  771 "VBNET.ATG" 
out section);

#line  771 "VBNET.ATG" 
			attributes.Add(section); 
		}
		Identifier();

#line  774 "VBNET.ATG" 
		f = new FieldDeclaration(attributes);
		varDecl = new VariableDeclaration(t.val);
		f.Fields.Add(varDecl);
		f.StartLocation = varDecl.StartLocation = t.Location;
		
		if (la.kind == 21) {
			lexer.NextToken();
			Expr(
#line  779 "VBNET.ATG" 
out expr);

#line  779 "VBNET.ATG" 
			varDecl.Initializer = expr; 
		}
		EndOfStmt();
	}

	void InterfaceMemberDecl() {

#line  656 "VBNET.ATG" 
		TypeReference type =null;
		List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
		List<TemplateDefinition> templates = new List<TemplateDefinition>();
		AttributeSection section, returnTypeAttributeSection = null;
		ModifierList mod = new ModifierList();
		List<AttributeSection> attributes = new List<AttributeSection>();
		string name;
		
		if (StartOf(19)) {
			while (la.kind == 41) {
				AttributeSection(
#line  664 "VBNET.ATG" 
out section);

#line  664 "VBNET.ATG" 
				attributes.Add(section); 
			}
			while (StartOf(10)) {
				MemberModifier(
#line  667 "VBNET.ATG" 
mod);
			}
			if (la.kind == 120) {
				lexer.NextToken();

#line  671 "VBNET.ATG" 
				mod.Check(Modifiers.VBInterfaceEvents);
				Location startLocation = t.Location;
				
				Identifier();

#line  674 "VBNET.ATG" 
				name = t.val; 
				if (la.kind == 38) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
#line  675 "VBNET.ATG" 
p);
					}
					Expect(39);
				}
				if (la.kind == 64) {
					lexer.NextToken();
					TypeName(
#line  676 "VBNET.ATG" 
out type);
				}
				EndOfStmt();

#line  679 "VBNET.ATG" 
				EventDeclaration ed = new EventDeclaration {
				Name = name, TypeReference = type, Modifier = mod.Modifier,
				Parameters = p, Attributes = attributes,
				StartLocation = startLocation, EndLocation = t.EndLocation
				};
				AddChild(ed);
				
			} else if (la.kind == 210) {
				lexer.NextToken();

#line  689 "VBNET.ATG" 
				Location startLocation =  t.Location;
				mod.Check(Modifiers.VBInterfaceMethods);
				
				Identifier();

#line  692 "VBNET.ATG" 
				name = t.val; 
				TypeParameterList(
#line  693 "VBNET.ATG" 
templates);
				if (la.kind == 38) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
#line  694 "VBNET.ATG" 
p);
					}
					Expect(39);
				}
				EndOfStmt();

#line  697 "VBNET.ATG" 
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
				
			} else if (la.kind == 128) {
				lexer.NextToken();

#line  712 "VBNET.ATG" 
				mod.Check(Modifiers.VBInterfaceMethods);
				Location startLocation = t.Location;
				
				Identifier();

#line  715 "VBNET.ATG" 
				name = t.val; 
				TypeParameterList(
#line  716 "VBNET.ATG" 
templates);
				if (la.kind == 38) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
#line  717 "VBNET.ATG" 
p);
					}
					Expect(39);
				}
				if (la.kind == 64) {
					lexer.NextToken();
					while (la.kind == 41) {
						AttributeSection(
#line  718 "VBNET.ATG" 
out returnTypeAttributeSection);
					}
					TypeName(
#line  718 "VBNET.ATG" 
out type);
				}

#line  720 "VBNET.ATG" 
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
			} else if (la.kind == 186) {
				lexer.NextToken();

#line  740 "VBNET.ATG" 
				Location startLocation = t.Location;
				mod.Check(Modifiers.VBInterfaceProperties);
				
				Identifier();

#line  743 "VBNET.ATG" 
				name = t.val;  
				if (la.kind == 38) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
#line  744 "VBNET.ATG" 
p);
					}
					Expect(39);
				}
				if (la.kind == 64) {
					lexer.NextToken();
					TypeName(
#line  745 "VBNET.ATG" 
out type);
				}

#line  747 "VBNET.ATG" 
				if(type == null) {
				type = new TypeReference("System.Object", true);
				}
				
				EndOfStmt();

#line  753 "VBNET.ATG" 
				PropertyDeclaration pd = new PropertyDeclaration(name, type, mod.Modifier, attributes);
				pd.Parameters = p;
				pd.EndLocation = t.EndLocation;
				pd.StartLocation = startLocation;
				AddChild(pd);
				
			} else SynErr(263);
		} else if (StartOf(20)) {
			NonModuleDeclaration(
#line  761 "VBNET.ATG" 
mod, attributes);
		} else SynErr(264);
	}

	void Expr(
#line  1648 "VBNET.ATG" 
out Expression expr) {

#line  1649 "VBNET.ATG" 
		expr = null; 
		if (
#line  1650 "VBNET.ATG" 
IsQueryExpression() ) {
			QueryExpr(
#line  1651 "VBNET.ATG" 
out expr);
		} else if (la.kind == 128 || la.kind == 210) {
			LambdaExpr(
#line  1652 "VBNET.ATG" 
out expr);
		} else if (StartOf(21)) {
			DisjunctionExpr(
#line  1653 "VBNET.ATG" 
out expr);
		} else SynErr(265);
	}

	void ImplementsClause(
#line  1621 "VBNET.ATG" 
out List<InterfaceImplementation> baseInterfaces) {

#line  1623 "VBNET.ATG" 
		baseInterfaces = new List<InterfaceImplementation>();
		TypeReference type = null;
		string memberName = null;
		
		Expect(137);
		NonArrayTypeName(
#line  1628 "VBNET.ATG" 
out type, false);

#line  1629 "VBNET.ATG" 
		if (type != null) memberName = TypeReference.StripLastIdentifierFromType(ref type); 

#line  1630 "VBNET.ATG" 
		baseInterfaces.Add(new InterfaceImplementation(type, memberName)); 
		while (la.kind == 23) {
			lexer.NextToken();
			NonArrayTypeName(
#line  1632 "VBNET.ATG" 
out type, false);

#line  1633 "VBNET.ATG" 
			if (type != null) memberName = TypeReference.StripLastIdentifierFromType(ref type); 

#line  1634 "VBNET.ATG" 
			baseInterfaces.Add(new InterfaceImplementation(type, memberName)); 
		}
	}

	void HandlesClause(
#line  1579 "VBNET.ATG" 
out List<string> handlesClause) {

#line  1581 "VBNET.ATG" 
		handlesClause = new List<string>();
		string name;
		
		Expect(135);
		EventMemberSpecifier(
#line  1584 "VBNET.ATG" 
out name);

#line  1584 "VBNET.ATG" 
		if (name != null) handlesClause.Add(name); 
		while (la.kind == 23) {
			lexer.NextToken();
			EventMemberSpecifier(
#line  1585 "VBNET.ATG" 
out name);

#line  1585 "VBNET.ATG" 
			if (name != null) handlesClause.Add(name); 
		}
	}

	void Block(
#line  2810 "VBNET.ATG" 
out Statement stmt) {

#line  2813 "VBNET.ATG" 
		BlockStatement blockStmt = new BlockStatement();
		/* in snippet parsing mode, t might be null */
		if (t != null) blockStmt.StartLocation = t.EndLocation;
		BlockStart(blockStmt);
		
		while (StartOf(22) || 
#line  2819 "VBNET.ATG" 
IsEndStmtAhead()) {
			if (
#line  2819 "VBNET.ATG" 
IsEndStmtAhead()) {
				Expect(114);
				EndOfStmt();

#line  2819 "VBNET.ATG" 
				AddChild(new EndStatement()); 
			} else {
				Statement();
				EndOfStmt();
			}
		}

#line  2824 "VBNET.ATG" 
		stmt = blockStmt;
		if (t != null) blockStmt.EndLocation = t.EndLocation;
		BlockEnd();
		
	}

	void Charset(
#line  1571 "VBNET.ATG" 
out CharsetModifier charsetModifier) {

#line  1572 "VBNET.ATG" 
		charsetModifier = CharsetModifier.None; 
		if (la.kind == 128 || la.kind == 210) {
		} else if (la.kind == 63) {
			lexer.NextToken();

#line  1573 "VBNET.ATG" 
			charsetModifier = CharsetModifier.Ansi; 
		} else if (la.kind == 67) {
			lexer.NextToken();

#line  1574 "VBNET.ATG" 
			charsetModifier = CharsetModifier.Auto; 
		} else if (la.kind == 223) {
			lexer.NextToken();

#line  1575 "VBNET.ATG" 
			charsetModifier = CharsetModifier.Unicode; 
		} else SynErr(266);
	}

	void IdentifierForFieldDeclaration() {
		switch (la.kind) {
		case 2: {
			lexer.NextToken();
			break;
		}
		case 59: {
			lexer.NextToken();
			break;
		}
		case 63: {
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
		case 67: {
			lexer.NextToken();
			break;
		}
		case 68: {
			lexer.NextToken();
			break;
		}
		case 71: {
			lexer.NextToken();
			break;
		}
		case 88: {
			lexer.NextToken();
			break;
		}
		case 105: {
			lexer.NextToken();
			break;
		}
		case 108: {
			lexer.NextToken();
			break;
		}
		case 117: {
			lexer.NextToken();
			break;
		}
		case 122: {
			lexer.NextToken();
			break;
		}
		case 127: {
			lexer.NextToken();
			break;
		}
		case 134: {
			lexer.NextToken();
			break;
		}
		case 140: {
			lexer.NextToken();
			break;
		}
		case 144: {
			lexer.NextToken();
			break;
		}
		case 147: {
			lexer.NextToken();
			break;
		}
		case 148: {
			lexer.NextToken();
			break;
		}
		case 171: {
			lexer.NextToken();
			break;
		}
		case 177: {
			lexer.NextToken();
			break;
		}
		case 184: {
			lexer.NextToken();
			break;
		}
		case 203: {
			lexer.NextToken();
			break;
		}
		case 212: {
			lexer.NextToken();
			break;
		}
		case 213: {
			lexer.NextToken();
			break;
		}
		case 223: {
			lexer.NextToken();
			break;
		}
		case 224: {
			lexer.NextToken();
			break;
		}
		case 230: {
			lexer.NextToken();
			break;
		}
		default: SynErr(267); break;
		}
	}

	void VariableDeclaratorPartAfterIdentifier(
#line  1456 "VBNET.ATG" 
List<VariableDeclaration> fieldDeclaration, string name) {

#line  1458 "VBNET.ATG" 
		Expression expr = null;
		TypeReference type = null;
		ArrayList rank = null;
		List<Expression> dimension = null;
		Location startLocation = t.Location;
		
		if (
#line  1464 "VBNET.ATG" 
IsSize() && !IsDims()) {
			ArrayInitializationModifier(
#line  1464 "VBNET.ATG" 
out dimension);
		}
		if (
#line  1465 "VBNET.ATG" 
IsDims()) {
			ArrayNameModifier(
#line  1465 "VBNET.ATG" 
out rank);
		}
		if (
#line  1467 "VBNET.ATG" 
IsObjectCreation()) {
			Expect(64);
			ObjectCreateExpression(
#line  1467 "VBNET.ATG" 
out expr);

#line  1469 "VBNET.ATG" 
			if (expr is ObjectCreateExpression) {
			type = ((ObjectCreateExpression)expr).CreateType.Clone();
			} else {
				type = ((ArrayCreateExpression)expr).CreateType.Clone();
			}
			
		} else if (StartOf(23)) {
			if (la.kind == 64) {
				lexer.NextToken();
				TypeName(
#line  1476 "VBNET.ATG" 
out type);

#line  1478 "VBNET.ATG" 
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

#line  1490 "VBNET.ATG" 
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
#line  1513 "VBNET.ATG" 
out expr);
			}
		} else SynErr(268);

#line  1516 "VBNET.ATG" 
		VariableDeclaration varDecl = new VariableDeclaration(name, expr, type);
		varDecl.StartLocation = startLocation;
		varDecl.EndLocation = t.Location;
		fieldDeclaration.Add(varDecl);
		
	}

	void VariableDeclarator(
#line  1450 "VBNET.ATG" 
List<VariableDeclaration> fieldDeclaration) {
		Identifier();

#line  1452 "VBNET.ATG" 
		string name = t.val; 
		VariableDeclaratorPartAfterIdentifier(
#line  1453 "VBNET.ATG" 
fieldDeclaration, name);
	}

	void ConstantDeclarator(
#line  1431 "VBNET.ATG" 
List<VariableDeclaration> constantDeclaration) {

#line  1433 "VBNET.ATG" 
		Expression expr = null;
		TypeReference type = null;
		string name = String.Empty;
		Location location;
		
		Identifier();

#line  1438 "VBNET.ATG" 
		name = t.val; location = t.Location; 
		if (la.kind == 64) {
			lexer.NextToken();
			TypeName(
#line  1439 "VBNET.ATG" 
out type);
		}
		Expect(21);
		Expr(
#line  1440 "VBNET.ATG" 
out expr);

#line  1442 "VBNET.ATG" 
		VariableDeclaration f = new VariableDeclaration(name, expr);
		f.TypeReference = type;
		f.StartLocation = location;
		constantDeclaration.Add(f);
		
	}

	void ObjectCreateExpression(
#line  1978 "VBNET.ATG" 
out Expression oce) {

#line  1980 "VBNET.ATG" 
		TypeReference type = null;
		CollectionInitializerExpression initializer = null;
		List<Expression> arguments = null;
		ArrayList dimensions = null;
		oce = null;
		bool canBeNormal; bool canBeReDim;
		
		Expect(163);
		if (StartOf(8)) {
			NonArrayTypeName(
#line  1988 "VBNET.ATG" 
out type, false);
			if (la.kind == 38) {
				lexer.NextToken();
				NormalOrReDimArgumentList(
#line  1989 "VBNET.ATG" 
out arguments, out canBeNormal, out canBeReDim);
				Expect(39);
				if (la.kind == 36 || 
#line  1990 "VBNET.ATG" 
la.kind == Tokens.OpenParenthesis) {
					if (
#line  1990 "VBNET.ATG" 
la.kind == Tokens.OpenParenthesis) {
						ArrayTypeModifiers(
#line  1991 "VBNET.ATG" 
out dimensions);
						CollectionInitializer(
#line  1992 "VBNET.ATG" 
out initializer);
					} else {
						CollectionInitializer(
#line  1993 "VBNET.ATG" 
out initializer);
					}
				}

#line  1995 "VBNET.ATG" 
				if (canBeReDim && !canBeNormal && initializer == null) initializer = new CollectionInitializerExpression(); 
			}
		}

#line  1999 "VBNET.ATG" 
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
		
		if (la.kind == 127 || la.kind == 233) {
			if (la.kind == 233) {

#line  2014 "VBNET.ATG" 
				MemberInitializerExpression memberInitializer = null;
				
				lexer.NextToken();

#line  2018 "VBNET.ATG" 
				CollectionInitializerExpression memberInitializers = new CollectionInitializerExpression();
				memberInitializers.StartLocation = la.Location;
				
				Expect(36);
				MemberInitializer(
#line  2022 "VBNET.ATG" 
out memberInitializer);

#line  2023 "VBNET.ATG" 
				memberInitializers.CreateExpressions.Add(memberInitializer); 
				while (la.kind == 23) {
					lexer.NextToken();
					MemberInitializer(
#line  2025 "VBNET.ATG" 
out memberInitializer);

#line  2026 "VBNET.ATG" 
					memberInitializers.CreateExpressions.Add(memberInitializer); 
				}
				Expect(37);

#line  2030 "VBNET.ATG" 
				memberInitializers.EndLocation = t.Location;
				if(oce is ObjectCreateExpression)
				{
					((ObjectCreateExpression)oce).ObjectInitializer = memberInitializers;
				}
				
			} else {
				lexer.NextToken();
				CollectionInitializer(
#line  2040 "VBNET.ATG" 
out initializer);

#line  2042 "VBNET.ATG" 
				if(oce is ObjectCreateExpression)
				((ObjectCreateExpression)oce).ObjectInitializer = initializer;
				
			}
		}
	}

	void AccessorDecls(
#line  1365 "VBNET.ATG" 
out PropertyGetRegion getBlock, out PropertySetRegion setBlock) {

#line  1367 "VBNET.ATG" 
		List<AttributeSection> attributes = new List<AttributeSection>();
		AttributeSection section;
		getBlock = null;
		setBlock = null; 
		
		while (la.kind == 41) {
			AttributeSection(
#line  1372 "VBNET.ATG" 
out section);

#line  1372 "VBNET.ATG" 
			attributes.Add(section); 
		}
		if (StartOf(24)) {
			GetAccessorDecl(
#line  1374 "VBNET.ATG" 
out getBlock, attributes);
			if (StartOf(25)) {

#line  1376 "VBNET.ATG" 
				attributes = new List<AttributeSection>(); 
				while (la.kind == 41) {
					AttributeSection(
#line  1377 "VBNET.ATG" 
out section);

#line  1377 "VBNET.ATG" 
					attributes.Add(section); 
				}
				SetAccessorDecl(
#line  1378 "VBNET.ATG" 
out setBlock, attributes);
			}
		} else if (StartOf(26)) {
			SetAccessorDecl(
#line  1381 "VBNET.ATG" 
out setBlock, attributes);
			if (StartOf(27)) {

#line  1383 "VBNET.ATG" 
				attributes = new List<AttributeSection>(); 
				while (la.kind == 41) {
					AttributeSection(
#line  1384 "VBNET.ATG" 
out section);

#line  1384 "VBNET.ATG" 
					attributes.Add(section); 
				}
				GetAccessorDecl(
#line  1385 "VBNET.ATG" 
out getBlock, attributes);
			}
		} else SynErr(269);
	}

	void EventAccessorDeclaration(
#line  1328 "VBNET.ATG" 
out EventAddRemoveRegion eventAccessorDeclaration) {

#line  1330 "VBNET.ATG" 
		Statement stmt = null;
		List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		eventAccessorDeclaration = null;
		
		while (la.kind == 41) {
			AttributeSection(
#line  1336 "VBNET.ATG" 
out section);

#line  1336 "VBNET.ATG" 
			attributes.Add(section); 
		}
		if (la.kind == 57) {
			lexer.NextToken();
			if (la.kind == 38) {
				lexer.NextToken();
				if (StartOf(6)) {
					FormalParameterList(
#line  1338 "VBNET.ATG" 
p);
				}
				Expect(39);
			}
			Expect(1);
			Block(
#line  1339 "VBNET.ATG" 
out stmt);
			Expect(114);
			Expect(57);
			EndOfStmt();

#line  1341 "VBNET.ATG" 
			eventAccessorDeclaration = new EventAddRegion(attributes);
			eventAccessorDeclaration.Block = (BlockStatement)stmt;
			eventAccessorDeclaration.Parameters = p;
			
		} else if (la.kind == 193) {
			lexer.NextToken();
			if (la.kind == 38) {
				lexer.NextToken();
				if (StartOf(6)) {
					FormalParameterList(
#line  1346 "VBNET.ATG" 
p);
				}
				Expect(39);
			}
			Expect(1);
			Block(
#line  1347 "VBNET.ATG" 
out stmt);
			Expect(114);
			Expect(193);
			EndOfStmt();

#line  1349 "VBNET.ATG" 
			eventAccessorDeclaration = new EventRemoveRegion(attributes);
			eventAccessorDeclaration.Block = (BlockStatement)stmt;
			eventAccessorDeclaration.Parameters = p;
			
		} else if (la.kind == 189) {
			lexer.NextToken();
			if (la.kind == 38) {
				lexer.NextToken();
				if (StartOf(6)) {
					FormalParameterList(
#line  1354 "VBNET.ATG" 
p);
				}
				Expect(39);
			}
			Expect(1);
			Block(
#line  1355 "VBNET.ATG" 
out stmt);
			Expect(114);
			Expect(189);
			EndOfStmt();

#line  1357 "VBNET.ATG" 
			eventAccessorDeclaration = new EventRaiseRegion(attributes);
			eventAccessorDeclaration.Block = (BlockStatement)stmt;
			eventAccessorDeclaration.Parameters = p;
			
		} else SynErr(270);
	}

	void OverloadableOperator(
#line  1270 "VBNET.ATG" 
out OverloadableOperatorType operatorType) {

#line  1271 "VBNET.ATG" 
		operatorType = OverloadableOperatorType.None; 
		switch (la.kind) {
		case 32: {
			lexer.NextToken();

#line  1273 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Add; 
			break;
		}
		case 31: {
			lexer.NextToken();

#line  1275 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Subtract; 
			break;
		}
		case 35: {
			lexer.NextToken();

#line  1277 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Multiply; 
			break;
		}
		case 25: {
			lexer.NextToken();

#line  1279 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Divide; 
			break;
		}
		case 26: {
			lexer.NextToken();

#line  1281 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.DivideInteger; 
			break;
		}
		case 24: {
			lexer.NextToken();

#line  1283 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Concat; 
			break;
		}
		case 151: {
			lexer.NextToken();

#line  1285 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Like; 
			break;
		}
		case 155: {
			lexer.NextToken();

#line  1287 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Modulus; 
			break;
		}
		case 61: {
			lexer.NextToken();

#line  1289 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.BitwiseAnd; 
			break;
		}
		case 176: {
			lexer.NextToken();

#line  1291 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.BitwiseOr; 
			break;
		}
		case 236: {
			lexer.NextToken();

#line  1293 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.ExclusiveOr; 
			break;
		}
		case 33: {
			lexer.NextToken();

#line  1295 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Power; 
			break;
		}
		case 45: {
			lexer.NextToken();

#line  1297 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.ShiftLeft; 
			break;
		}
		case 46: {
			lexer.NextToken();

#line  1299 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.ShiftRight; 
			break;
		}
		case 21: {
			lexer.NextToken();

#line  1301 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Equality; 
			break;
		}
		case 42: {
			lexer.NextToken();

#line  1303 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.InEquality; 
			break;
		}
		case 41: {
			lexer.NextToken();

#line  1305 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.LessThan; 
			break;
		}
		case 44: {
			lexer.NextToken();

#line  1307 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.LessThanOrEqual; 
			break;
		}
		case 40: {
			lexer.NextToken();

#line  1309 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.GreaterThan; 
			break;
		}
		case 43: {
			lexer.NextToken();

#line  1311 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.GreaterThanOrEqual; 
			break;
		}
		case 95: {
			lexer.NextToken();

#line  1313 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.CType; 
			break;
		}
		case 2: case 59: case 63: case 65: case 66: case 67: case 68: case 71: case 88: case 99: case 105: case 108: case 117: case 122: case 127: case 134: case 140: case 144: case 147: case 148: case 171: case 177: case 184: case 203: case 212: case 213: case 223: case 224: case 230: {
			Identifier();

#line  1317 "VBNET.ATG" 
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
		default: SynErr(271); break;
		}
	}

	void GetAccessorDecl(
#line  1391 "VBNET.ATG" 
out PropertyGetRegion getBlock, List<AttributeSection> attributes) {

#line  1392 "VBNET.ATG" 
		Statement stmt = null; Modifiers m; 
		PropertyAccessorAccessModifier(
#line  1394 "VBNET.ATG" 
out m);
		Expect(129);

#line  1396 "VBNET.ATG" 
		Location startLocation = t.Location; 
		Expect(1);
		Block(
#line  1398 "VBNET.ATG" 
out stmt);

#line  1399 "VBNET.ATG" 
		getBlock = new PropertyGetRegion((BlockStatement)stmt, attributes); 
		Expect(114);
		Expect(129);

#line  1401 "VBNET.ATG" 
		getBlock.Modifier = m; 

#line  1402 "VBNET.ATG" 
		getBlock.StartLocation = startLocation; getBlock.EndLocation = t.EndLocation; 
		EndOfStmt();
	}

	void SetAccessorDecl(
#line  1407 "VBNET.ATG" 
out PropertySetRegion setBlock, List<AttributeSection> attributes) {

#line  1409 "VBNET.ATG" 
		Statement stmt = null;
		List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
		Modifiers m;
		
		PropertyAccessorAccessModifier(
#line  1414 "VBNET.ATG" 
out m);
		Expect(198);

#line  1416 "VBNET.ATG" 
		Location startLocation = t.Location; 
		if (la.kind == 38) {
			lexer.NextToken();
			if (StartOf(6)) {
				FormalParameterList(
#line  1417 "VBNET.ATG" 
p);
			}
			Expect(39);
		}
		Expect(1);
		Block(
#line  1419 "VBNET.ATG" 
out stmt);

#line  1421 "VBNET.ATG" 
		setBlock = new PropertySetRegion((BlockStatement)stmt, attributes);
		setBlock.Modifier = m;
		setBlock.Parameters = p;
		
		Expect(114);
		Expect(198);

#line  1426 "VBNET.ATG" 
		setBlock.StartLocation = startLocation; setBlock.EndLocation = t.EndLocation; 
		EndOfStmt();
	}

	void PropertyAccessorAccessModifier(
#line  3527 "VBNET.ATG" 
out Modifiers m) {

#line  3528 "VBNET.ATG" 
		m = Modifiers.None; 
		while (StartOf(28)) {
			if (la.kind == 188) {
				lexer.NextToken();

#line  3530 "VBNET.ATG" 
				m |= Modifiers.Public; 
			} else if (la.kind == 187) {
				lexer.NextToken();

#line  3531 "VBNET.ATG" 
				m |= Modifiers.Protected; 
			} else if (la.kind == 126) {
				lexer.NextToken();

#line  3532 "VBNET.ATG" 
				m |= Modifiers.Internal; 
			} else {
				lexer.NextToken();

#line  3533 "VBNET.ATG" 
				m |= Modifiers.Private; 
			}
		}
	}

	void ArrayInitializationModifier(
#line  1524 "VBNET.ATG" 
out List<Expression> arrayModifiers) {

#line  1526 "VBNET.ATG" 
		arrayModifiers = null;
		
		Expect(38);
		InitializationRankList(
#line  1528 "VBNET.ATG" 
out arrayModifiers);
		Expect(39);
	}

	void ArrayNameModifier(
#line  2605 "VBNET.ATG" 
out ArrayList arrayModifiers) {

#line  2607 "VBNET.ATG" 
		arrayModifiers = null;
		
		ArrayTypeModifiers(
#line  2609 "VBNET.ATG" 
out arrayModifiers);
	}

	void InitializationRankList(
#line  1532 "VBNET.ATG" 
out List<Expression> rank) {

#line  1534 "VBNET.ATG" 
		rank = new List<Expression>();
		Expression expr = null;
		
		Expr(
#line  1537 "VBNET.ATG" 
out expr);
		if (la.kind == 216) {
			lexer.NextToken();

#line  1538 "VBNET.ATG" 
			EnsureIsZero(expr); 
			Expr(
#line  1539 "VBNET.ATG" 
out expr);
		}

#line  1541 "VBNET.ATG" 
		if (expr != null) { rank.Add(expr); } 
		while (la.kind == 23) {
			lexer.NextToken();
			Expr(
#line  1543 "VBNET.ATG" 
out expr);
			if (la.kind == 216) {
				lexer.NextToken();

#line  1544 "VBNET.ATG" 
				EnsureIsZero(expr); 
				Expr(
#line  1545 "VBNET.ATG" 
out expr);
			}

#line  1547 "VBNET.ATG" 
			if (expr != null) { rank.Add(expr); } 
		}
	}

	void CollectionInitializer(
#line  1552 "VBNET.ATG" 
out CollectionInitializerExpression outExpr) {

#line  1554 "VBNET.ATG" 
		Expression expr = null;
		CollectionInitializerExpression initializer = new CollectionInitializerExpression();
		
		Expect(36);
		if (StartOf(29)) {
			Expr(
#line  1559 "VBNET.ATG" 
out expr);

#line  1561 "VBNET.ATG" 
			if (expr != null) { initializer.CreateExpressions.Add(expr); }
			
			while (
#line  1564 "VBNET.ATG" 
NotFinalComma()) {
				Expect(23);
				Expr(
#line  1564 "VBNET.ATG" 
out expr);

#line  1565 "VBNET.ATG" 
				if (expr != null) { initializer.CreateExpressions.Add(expr); } 
			}
		}
		Expect(37);

#line  1568 "VBNET.ATG" 
		outExpr = initializer; 
	}

	void EventMemberSpecifier(
#line  1638 "VBNET.ATG" 
out string name) {

#line  1639 "VBNET.ATG" 
		string eventName; 
		if (StartOf(4)) {
			Identifier();
		} else if (la.kind == 159) {
			lexer.NextToken();
		} else if (la.kind == 154) {
			lexer.NextToken();
		} else SynErr(272);

#line  1642 "VBNET.ATG" 
		name = t.val; 
		Expect(27);
		IdentifierOrKeyword(
#line  1644 "VBNET.ATG" 
out eventName);

#line  1645 "VBNET.ATG" 
		name = name + "." + eventName; 
	}

	void IdentifierOrKeyword(
#line  3460 "VBNET.ATG" 
out string name) {
		lexer.NextToken();

#line  3462 "VBNET.ATG" 
		name = t.val;  
	}

	void QueryExpr(
#line  2129 "VBNET.ATG" 
out Expression expr) {

#line  2131 "VBNET.ATG" 
		QueryExpressionVB qexpr = new QueryExpressionVB();
		qexpr.StartLocation = la.Location;
		expr = qexpr;
		
		FromOrAggregateQueryOperator(
#line  2135 "VBNET.ATG" 
qexpr.Clauses);
		while (StartOf(30)) {
			QueryOperator(
#line  2136 "VBNET.ATG" 
qexpr.Clauses);
		}

#line  2138 "VBNET.ATG" 
		qexpr.EndLocation = t.EndLocation;
		
	}

	void LambdaExpr(
#line  2049 "VBNET.ATG" 
out Expression expr) {

#line  2051 "VBNET.ATG" 
		LambdaExpression lambda = null;
		
		if (la.kind == 210) {
			SubLambdaExpression(
#line  2053 "VBNET.ATG" 
out lambda);
		} else if (la.kind == 128) {
			FunctionLambdaExpression(
#line  2054 "VBNET.ATG" 
out lambda);
		} else SynErr(273);

#line  2055 "VBNET.ATG" 
		expr = lambda; 
	}

	void DisjunctionExpr(
#line  1822 "VBNET.ATG" 
out Expression outExpr) {

#line  1824 "VBNET.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		ConjunctionExpr(
#line  1827 "VBNET.ATG" 
out outExpr);
		while (la.kind == 176 || la.kind == 178 || la.kind == 236) {
			if (la.kind == 176) {
				lexer.NextToken();

#line  1830 "VBNET.ATG" 
				op = BinaryOperatorType.BitwiseOr; 
			} else if (la.kind == 178) {
				lexer.NextToken();

#line  1831 "VBNET.ATG" 
				op = BinaryOperatorType.LogicalOr; 
			} else {
				lexer.NextToken();

#line  1832 "VBNET.ATG" 
				op = BinaryOperatorType.ExclusiveOr; 
			}
			ConjunctionExpr(
#line  1834 "VBNET.ATG" 
out expr);

#line  1834 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr);  
		}
	}

	void AssignmentOperator(
#line  1656 "VBNET.ATG" 
out AssignmentOperatorType op) {

#line  1657 "VBNET.ATG" 
		op = AssignmentOperatorType.None; 
		switch (la.kind) {
		case 21: {
			lexer.NextToken();

#line  1658 "VBNET.ATG" 
			op = AssignmentOperatorType.Assign; 
			break;
		}
		case 55: {
			lexer.NextToken();

#line  1659 "VBNET.ATG" 
			op = AssignmentOperatorType.ConcatString; 
			break;
		}
		case 47: {
			lexer.NextToken();

#line  1660 "VBNET.ATG" 
			op = AssignmentOperatorType.Add; 
			break;
		}
		case 49: {
			lexer.NextToken();

#line  1661 "VBNET.ATG" 
			op = AssignmentOperatorType.Subtract; 
			break;
		}
		case 50: {
			lexer.NextToken();

#line  1662 "VBNET.ATG" 
			op = AssignmentOperatorType.Multiply; 
			break;
		}
		case 51: {
			lexer.NextToken();

#line  1663 "VBNET.ATG" 
			op = AssignmentOperatorType.Divide; 
			break;
		}
		case 52: {
			lexer.NextToken();

#line  1664 "VBNET.ATG" 
			op = AssignmentOperatorType.DivideInteger; 
			break;
		}
		case 48: {
			lexer.NextToken();

#line  1665 "VBNET.ATG" 
			op = AssignmentOperatorType.Power; 
			break;
		}
		case 53: {
			lexer.NextToken();

#line  1666 "VBNET.ATG" 
			op = AssignmentOperatorType.ShiftLeft; 
			break;
		}
		case 54: {
			lexer.NextToken();

#line  1667 "VBNET.ATG" 
			op = AssignmentOperatorType.ShiftRight; 
			break;
		}
		default: SynErr(274); break;
		}
	}

	void SimpleExpr(
#line  1671 "VBNET.ATG" 
out Expression pexpr) {

#line  1672 "VBNET.ATG" 
		string name; 
		SimpleNonInvocationExpression(
#line  1674 "VBNET.ATG" 
out pexpr);
		while (la.kind == 27 || la.kind == 30 || la.kind == 38) {
			if (la.kind == 27) {
				lexer.NextToken();
				IdentifierOrKeyword(
#line  1676 "VBNET.ATG" 
out name);

#line  1677 "VBNET.ATG" 
				pexpr = new MemberReferenceExpression(pexpr, name); 
				if (
#line  1678 "VBNET.ATG" 
la.kind == Tokens.OpenParenthesis && Peek(1).kind == Tokens.Of) {
					lexer.NextToken();
					Expect(170);
					TypeArgumentList(
#line  1679 "VBNET.ATG" 
((MemberReferenceExpression)pexpr).TypeArguments);
					Expect(39);
				}
			} else if (la.kind == 30) {
				lexer.NextToken();
				IdentifierOrKeyword(
#line  1681 "VBNET.ATG" 
out name);

#line  1681 "VBNET.ATG" 
				pexpr = new BinaryOperatorExpression(pexpr, BinaryOperatorType.DictionaryAccess, new PrimitiveExpression(name, name)); 
			} else {
				InvocationExpression(
#line  1682 "VBNET.ATG" 
ref pexpr);
			}
		}
	}

	void SimpleNonInvocationExpression(
#line  1686 "VBNET.ATG" 
out Expression pexpr) {

#line  1688 "VBNET.ATG" 
		Expression expr;
		CollectionInitializerExpression cie;
		TypeReference type = null;
		string name = String.Empty;
		pexpr = null;
		
		if (StartOf(31)) {
			switch (la.kind) {
			case 3: {
				lexer.NextToken();

#line  1697 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(t.literalValue, t.val) { LiteralFormat = t.literalFormat };  
				break;
			}
			case 4: {
				lexer.NextToken();

#line  1698 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(t.literalValue, t.val) { LiteralFormat = t.literalFormat };  
				break;
			}
			case 7: {
				lexer.NextToken();

#line  1699 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(t.literalValue, t.val) { LiteralFormat = t.literalFormat };  
				break;
			}
			case 6: {
				lexer.NextToken();

#line  1700 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(t.literalValue, t.val) { LiteralFormat = t.literalFormat };  
				break;
			}
			case 5: {
				lexer.NextToken();

#line  1701 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(t.literalValue, t.val) { LiteralFormat = t.literalFormat };  
				break;
			}
			case 9: {
				lexer.NextToken();

#line  1702 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(t.literalValue, t.val) { LiteralFormat = t.literalFormat };  
				break;
			}
			case 8: {
				lexer.NextToken();

#line  1703 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(t.literalValue, t.val) { LiteralFormat = t.literalFormat };  
				break;
			}
			case 217: {
				lexer.NextToken();

#line  1705 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(true, "true");  
				break;
			}
			case 123: {
				lexer.NextToken();

#line  1706 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(false, "false"); 
				break;
			}
			case 166: {
				lexer.NextToken();

#line  1707 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(null, "null");  
				break;
			}
			case 38: {
				lexer.NextToken();
				Expr(
#line  1708 "VBNET.ATG" 
out expr);
				Expect(39);

#line  1708 "VBNET.ATG" 
				pexpr = new ParenthesizedExpression(expr); 
				break;
			}
			case 2: case 59: case 63: case 65: case 66: case 67: case 68: case 71: case 88: case 99: case 105: case 108: case 117: case 122: case 127: case 134: case 140: case 144: case 147: case 148: case 171: case 177: case 184: case 203: case 212: case 213: case 223: case 224: case 230: {
				Identifier();

#line  1710 "VBNET.ATG" 
				pexpr = new IdentifierExpression(t.val);
				pexpr.StartLocation = t.Location; pexpr.EndLocation = t.EndLocation;
				
				if (
#line  1713 "VBNET.ATG" 
la.kind == Tokens.OpenParenthesis && Peek(1).kind == Tokens.Of) {
					lexer.NextToken();
					Expect(170);
					TypeArgumentList(
#line  1714 "VBNET.ATG" 
((IdentifierExpression)pexpr).TypeArguments);
					Expect(39);
				}
				break;
			}
			case 69: case 72: case 83: case 100: case 101: case 110: case 142: case 152: case 169: case 196: case 201: case 202: case 208: case 221: case 222: case 225: {

#line  1716 "VBNET.ATG" 
				string val = String.Empty; 
				if (StartOf(12)) {
					PrimitiveTypeName(
#line  1717 "VBNET.ATG" 
out val);
				} else if (la.kind == 169) {
					lexer.NextToken();

#line  1717 "VBNET.ATG" 
					val = "System.Object"; 
				} else SynErr(275);

#line  1718 "VBNET.ATG" 
				pexpr = new TypeReferenceExpression(new TypeReference(val, true)); 
				break;
			}
			case 154: {
				lexer.NextToken();

#line  1719 "VBNET.ATG" 
				pexpr = new ThisReferenceExpression(); 
				break;
			}
			case 159: case 160: {

#line  1720 "VBNET.ATG" 
				Expression retExpr = null; 
				if (la.kind == 159) {
					lexer.NextToken();

#line  1721 "VBNET.ATG" 
					retExpr = new BaseReferenceExpression(); 
				} else if (la.kind == 160) {
					lexer.NextToken();

#line  1722 "VBNET.ATG" 
					retExpr = new ClassReferenceExpression(); 
				} else SynErr(276);
				Expect(27);
				IdentifierOrKeyword(
#line  1724 "VBNET.ATG" 
out name);

#line  1724 "VBNET.ATG" 
				pexpr = new MemberReferenceExpression(retExpr, name); 
				break;
			}
			case 131: {
				lexer.NextToken();
				Expect(27);
				Identifier();

#line  1726 "VBNET.ATG" 
				type = new TypeReference(t.val ?? ""); 

#line  1728 "VBNET.ATG" 
				type.IsGlobal = true; 

#line  1729 "VBNET.ATG" 
				pexpr = new TypeReferenceExpression(type); 
				break;
			}
			case 163: {
				ObjectCreateExpression(
#line  1730 "VBNET.ATG" 
out expr);

#line  1730 "VBNET.ATG" 
				pexpr = expr; 
				break;
			}
			case 36: {
				CollectionInitializer(
#line  1731 "VBNET.ATG" 
out cie);

#line  1731 "VBNET.ATG" 
				pexpr = cie; 
				break;
			}
			case 95: case 107: case 219: {

#line  1733 "VBNET.ATG" 
				CastType castType = CastType.Cast; 
				if (la.kind == 107) {
					lexer.NextToken();
				} else if (la.kind == 95) {
					lexer.NextToken();

#line  1735 "VBNET.ATG" 
					castType = CastType.Conversion; 
				} else if (la.kind == 219) {
					lexer.NextToken();

#line  1736 "VBNET.ATG" 
					castType = CastType.TryCast; 
				} else SynErr(277);
				Expect(38);
				Expr(
#line  1738 "VBNET.ATG" 
out expr);
				Expect(23);
				TypeName(
#line  1738 "VBNET.ATG" 
out type);
				Expect(39);

#line  1739 "VBNET.ATG" 
				pexpr = new CastExpression(type, expr, castType); 
				break;
			}
			case 77: case 78: case 79: case 80: case 81: case 82: case 84: case 86: case 87: case 91: case 92: case 93: case 94: case 96: case 97: case 98: {
				CastTarget(
#line  1740 "VBNET.ATG" 
out type);
				Expect(38);
				Expr(
#line  1740 "VBNET.ATG" 
out expr);
				Expect(39);

#line  1740 "VBNET.ATG" 
				pexpr = new CastExpression(type, expr, CastType.PrimitiveConversion); 
				break;
			}
			case 58: {
				lexer.NextToken();
				Expr(
#line  1741 "VBNET.ATG" 
out expr);

#line  1741 "VBNET.ATG" 
				pexpr = new AddressOfExpression(expr); 
				break;
			}
			case 130: {
				lexer.NextToken();
				Expect(38);
				GetTypeTypeName(
#line  1742 "VBNET.ATG" 
out type);
				Expect(39);

#line  1742 "VBNET.ATG" 
				pexpr = new TypeOfExpression(type); 
				break;
			}
			case 220: {
				lexer.NextToken();
				SimpleExpr(
#line  1743 "VBNET.ATG" 
out expr);
				Expect(145);
				TypeName(
#line  1743 "VBNET.ATG" 
out type);

#line  1743 "VBNET.ATG" 
				pexpr = new TypeOfIsExpression(expr, type); 
				break;
			}
			case 136: {
				ConditionalExpression(
#line  1744 "VBNET.ATG" 
out pexpr);
				break;
			}
			}
		} else if (la.kind == 27) {
			lexer.NextToken();
			IdentifierOrKeyword(
#line  1748 "VBNET.ATG" 
out name);

#line  1748 "VBNET.ATG" 
			pexpr = new MemberReferenceExpression(null, name);
		} else SynErr(278);
	}

	void TypeArgumentList(
#line  2641 "VBNET.ATG" 
List<TypeReference> typeArguments) {

#line  2643 "VBNET.ATG" 
		TypeReference typeref;
		
		TypeName(
#line  2645 "VBNET.ATG" 
out typeref);

#line  2645 "VBNET.ATG" 
		if (typeref != null) typeArguments.Add(typeref); 
		while (la.kind == 23) {
			lexer.NextToken();
			TypeName(
#line  2648 "VBNET.ATG" 
out typeref);

#line  2648 "VBNET.ATG" 
			if (typeref != null) typeArguments.Add(typeref); 
		}
	}

	void InvocationExpression(
#line  1786 "VBNET.ATG" 
ref Expression pexpr) {

#line  1787 "VBNET.ATG" 
		List<Expression> parameters = null; 
		Expect(38);

#line  1789 "VBNET.ATG" 
		Location start = t.Location; 
		ArgumentList(
#line  1790 "VBNET.ATG" 
out parameters);
		Expect(39);

#line  1793 "VBNET.ATG" 
		pexpr = new InvocationExpression(pexpr, parameters);
		

#line  1795 "VBNET.ATG" 
		pexpr.StartLocation = start; pexpr.EndLocation = t.Location; 
	}

	void PrimitiveTypeName(
#line  3467 "VBNET.ATG" 
out string type) {

#line  3468 "VBNET.ATG" 
		type = String.Empty; 
		switch (la.kind) {
		case 69: {
			lexer.NextToken();

#line  3469 "VBNET.ATG" 
			type = "System.Boolean"; 
			break;
		}
		case 100: {
			lexer.NextToken();

#line  3470 "VBNET.ATG" 
			type = "System.DateTime"; 
			break;
		}
		case 83: {
			lexer.NextToken();

#line  3471 "VBNET.ATG" 
			type = "System.Char"; 
			break;
		}
		case 208: {
			lexer.NextToken();

#line  3472 "VBNET.ATG" 
			type = "System.String"; 
			break;
		}
		case 101: {
			lexer.NextToken();

#line  3473 "VBNET.ATG" 
			type = "System.Decimal"; 
			break;
		}
		case 72: {
			lexer.NextToken();

#line  3474 "VBNET.ATG" 
			type = "System.Byte"; 
			break;
		}
		case 201: {
			lexer.NextToken();

#line  3475 "VBNET.ATG" 
			type = "System.Int16"; 
			break;
		}
		case 142: {
			lexer.NextToken();

#line  3476 "VBNET.ATG" 
			type = "System.Int32"; 
			break;
		}
		case 152: {
			lexer.NextToken();

#line  3477 "VBNET.ATG" 
			type = "System.Int64"; 
			break;
		}
		case 202: {
			lexer.NextToken();

#line  3478 "VBNET.ATG" 
			type = "System.Single"; 
			break;
		}
		case 110: {
			lexer.NextToken();

#line  3479 "VBNET.ATG" 
			type = "System.Double"; 
			break;
		}
		case 221: {
			lexer.NextToken();

#line  3480 "VBNET.ATG" 
			type = "System.UInt32"; 
			break;
		}
		case 222: {
			lexer.NextToken();

#line  3481 "VBNET.ATG" 
			type = "System.UInt64"; 
			break;
		}
		case 225: {
			lexer.NextToken();

#line  3482 "VBNET.ATG" 
			type = "System.UInt16"; 
			break;
		}
		case 196: {
			lexer.NextToken();

#line  3483 "VBNET.ATG" 
			type = "System.SByte"; 
			break;
		}
		default: SynErr(279); break;
		}
	}

	void CastTarget(
#line  1800 "VBNET.ATG" 
out TypeReference type) {

#line  1802 "VBNET.ATG" 
		type = null;
		
		switch (la.kind) {
		case 77: {
			lexer.NextToken();

#line  1804 "VBNET.ATG" 
			type = new TypeReference("System.Boolean", true); 
			break;
		}
		case 78: {
			lexer.NextToken();

#line  1805 "VBNET.ATG" 
			type = new TypeReference("System.Byte", true); 
			break;
		}
		case 91: {
			lexer.NextToken();

#line  1806 "VBNET.ATG" 
			type = new TypeReference("System.SByte", true); 
			break;
		}
		case 79: {
			lexer.NextToken();

#line  1807 "VBNET.ATG" 
			type = new TypeReference("System.Char", true); 
			break;
		}
		case 80: {
			lexer.NextToken();

#line  1808 "VBNET.ATG" 
			type = new TypeReference("System.DateTime", true); 
			break;
		}
		case 82: {
			lexer.NextToken();

#line  1809 "VBNET.ATG" 
			type = new TypeReference("System.Decimal", true); 
			break;
		}
		case 81: {
			lexer.NextToken();

#line  1810 "VBNET.ATG" 
			type = new TypeReference("System.Double", true); 
			break;
		}
		case 92: {
			lexer.NextToken();

#line  1811 "VBNET.ATG" 
			type = new TypeReference("System.Int16", true); 
			break;
		}
		case 84: {
			lexer.NextToken();

#line  1812 "VBNET.ATG" 
			type = new TypeReference("System.Int32", true); 
			break;
		}
		case 86: {
			lexer.NextToken();

#line  1813 "VBNET.ATG" 
			type = new TypeReference("System.Int64", true); 
			break;
		}
		case 98: {
			lexer.NextToken();

#line  1814 "VBNET.ATG" 
			type = new TypeReference("System.UInt16", true); 
			break;
		}
		case 96: {
			lexer.NextToken();

#line  1815 "VBNET.ATG" 
			type = new TypeReference("System.UInt32", true); 
			break;
		}
		case 97: {
			lexer.NextToken();

#line  1816 "VBNET.ATG" 
			type = new TypeReference("System.UInt64", true); 
			break;
		}
		case 87: {
			lexer.NextToken();

#line  1817 "VBNET.ATG" 
			type = new TypeReference("System.Object", true); 
			break;
		}
		case 93: {
			lexer.NextToken();

#line  1818 "VBNET.ATG" 
			type = new TypeReference("System.Single", true); 
			break;
		}
		case 94: {
			lexer.NextToken();

#line  1819 "VBNET.ATG" 
			type = new TypeReference("System.String", true); 
			break;
		}
		default: SynErr(280); break;
		}
	}

	void GetTypeTypeName(
#line  2540 "VBNET.ATG" 
out TypeReference typeref) {

#line  2541 "VBNET.ATG" 
		ArrayList rank = null; 
		NonArrayTypeName(
#line  2543 "VBNET.ATG" 
out typeref, true);
		ArrayTypeModifiers(
#line  2544 "VBNET.ATG" 
out rank);

#line  2545 "VBNET.ATG" 
		if (rank != null && typeref != null) {
		typeref.RankSpecifier = (int[])rank.ToArray(typeof(int));
		}
		
	}

	void ConditionalExpression(
#line  1752 "VBNET.ATG" 
out Expression expr) {

#line  1754 "VBNET.ATG" 
		ConditionalExpression conditionalExpression = new ConditionalExpression();
		BinaryOperatorExpression binaryOperatorExpression = new BinaryOperatorExpression();
		conditionalExpression.StartLocation = binaryOperatorExpression.StartLocation = la.Location;
		
		Expression condition = null;
		Expression trueExpr = null;
		Expression falseExpr = null;
		
		Expect(136);
		Expect(38);
		Expr(
#line  1763 "VBNET.ATG" 
out condition);
		Expect(23);
		Expr(
#line  1763 "VBNET.ATG" 
out trueExpr);
		if (la.kind == 23) {
			lexer.NextToken();
			Expr(
#line  1763 "VBNET.ATG" 
out falseExpr);
		}
		Expect(39);

#line  1765 "VBNET.ATG" 
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
#line  2472 "VBNET.ATG" 
out List<Expression> arguments) {

#line  2474 "VBNET.ATG" 
		arguments = new List<Expression>();
		Expression expr = null;
		
		if (StartOf(29)) {
			Argument(
#line  2477 "VBNET.ATG" 
out expr);
		}
		while (la.kind == 23) {
			lexer.NextToken();

#line  2478 "VBNET.ATG" 
			arguments.Add(expr ?? Expression.Null); expr = null; 
			if (StartOf(29)) {
				Argument(
#line  2479 "VBNET.ATG" 
out expr);
			}

#line  2480 "VBNET.ATG" 
			if (expr == null) expr = Expression.Null; 
		}

#line  2482 "VBNET.ATG" 
		if (expr != null) arguments.Add(expr); 
	}

	void ConjunctionExpr(
#line  1838 "VBNET.ATG" 
out Expression outExpr) {

#line  1840 "VBNET.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		NotExpr(
#line  1843 "VBNET.ATG" 
out outExpr);
		while (la.kind == 61 || la.kind == 62) {
			if (la.kind == 61) {
				lexer.NextToken();

#line  1846 "VBNET.ATG" 
				op = BinaryOperatorType.BitwiseAnd; 
			} else {
				lexer.NextToken();

#line  1847 "VBNET.ATG" 
				op = BinaryOperatorType.LogicalAnd; 
			}
			NotExpr(
#line  1849 "VBNET.ATG" 
out expr);

#line  1849 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr);  
		}
	}

	void NotExpr(
#line  1853 "VBNET.ATG" 
out Expression outExpr) {

#line  1854 "VBNET.ATG" 
		UnaryOperatorType uop = UnaryOperatorType.None; 
		while (la.kind == 165) {
			lexer.NextToken();

#line  1855 "VBNET.ATG" 
			uop = UnaryOperatorType.Not; 
		}
		ComparisonExpr(
#line  1856 "VBNET.ATG" 
out outExpr);

#line  1857 "VBNET.ATG" 
		if (uop != UnaryOperatorType.None)
		outExpr = new UnaryOperatorExpression(outExpr, uop);
		
	}

	void ComparisonExpr(
#line  1862 "VBNET.ATG" 
out Expression outExpr) {

#line  1864 "VBNET.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		ShiftExpr(
#line  1867 "VBNET.ATG" 
out outExpr);
		while (StartOf(32)) {
			switch (la.kind) {
			case 41: {
				lexer.NextToken();

#line  1870 "VBNET.ATG" 
				op = BinaryOperatorType.LessThan; 
				break;
			}
			case 40: {
				lexer.NextToken();

#line  1871 "VBNET.ATG" 
				op = BinaryOperatorType.GreaterThan; 
				break;
			}
			case 44: {
				lexer.NextToken();

#line  1872 "VBNET.ATG" 
				op = BinaryOperatorType.LessThanOrEqual; 
				break;
			}
			case 43: {
				lexer.NextToken();

#line  1873 "VBNET.ATG" 
				op = BinaryOperatorType.GreaterThanOrEqual; 
				break;
			}
			case 42: {
				lexer.NextToken();

#line  1874 "VBNET.ATG" 
				op = BinaryOperatorType.InEquality; 
				break;
			}
			case 21: {
				lexer.NextToken();

#line  1875 "VBNET.ATG" 
				op = BinaryOperatorType.Equality; 
				break;
			}
			case 151: {
				lexer.NextToken();

#line  1876 "VBNET.ATG" 
				op = BinaryOperatorType.Like; 
				break;
			}
			case 145: {
				lexer.NextToken();

#line  1877 "VBNET.ATG" 
				op = BinaryOperatorType.ReferenceEquality; 
				break;
			}
			case 146: {
				lexer.NextToken();

#line  1878 "VBNET.ATG" 
				op = BinaryOperatorType.ReferenceInequality; 
				break;
			}
			}
			if (StartOf(33)) {
				ShiftExpr(
#line  1881 "VBNET.ATG" 
out expr);

#line  1881 "VBNET.ATG" 
				outExpr = new BinaryOperatorExpression(outExpr, op, expr);  
			} else if (la.kind == 165) {
				lexer.NextToken();
				ShiftExpr(
#line  1884 "VBNET.ATG" 
out expr);

#line  1884 "VBNET.ATG" 
				outExpr = new BinaryOperatorExpression(outExpr, op, new UnaryOperatorExpression(expr, UnaryOperatorType.Not));  
			} else SynErr(281);
		}
	}

	void ShiftExpr(
#line  1889 "VBNET.ATG" 
out Expression outExpr) {

#line  1891 "VBNET.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		ConcatenationExpr(
#line  1894 "VBNET.ATG" 
out outExpr);
		while (la.kind == 45 || la.kind == 46) {
			if (la.kind == 45) {
				lexer.NextToken();

#line  1897 "VBNET.ATG" 
				op = BinaryOperatorType.ShiftLeft; 
			} else {
				lexer.NextToken();

#line  1898 "VBNET.ATG" 
				op = BinaryOperatorType.ShiftRight; 
			}
			ConcatenationExpr(
#line  1900 "VBNET.ATG" 
out expr);

#line  1900 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr);  
		}
	}

	void ConcatenationExpr(
#line  1904 "VBNET.ATG" 
out Expression outExpr) {

#line  1905 "VBNET.ATG" 
		Expression expr; 
		AdditiveExpr(
#line  1907 "VBNET.ATG" 
out outExpr);
		while (la.kind == 24) {
			lexer.NextToken();
			AdditiveExpr(
#line  1907 "VBNET.ATG" 
out expr);

#line  1907 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.Concat, expr);  
		}
	}

	void AdditiveExpr(
#line  1910 "VBNET.ATG" 
out Expression outExpr) {

#line  1912 "VBNET.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		ModuloExpr(
#line  1915 "VBNET.ATG" 
out outExpr);
		while (la.kind == 31 || la.kind == 32) {
			if (la.kind == 32) {
				lexer.NextToken();

#line  1918 "VBNET.ATG" 
				op = BinaryOperatorType.Add; 
			} else {
				lexer.NextToken();

#line  1919 "VBNET.ATG" 
				op = BinaryOperatorType.Subtract; 
			}
			ModuloExpr(
#line  1921 "VBNET.ATG" 
out expr);

#line  1921 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr);  
		}
	}

	void ModuloExpr(
#line  1925 "VBNET.ATG" 
out Expression outExpr) {

#line  1926 "VBNET.ATG" 
		Expression expr; 
		IntegerDivisionExpr(
#line  1928 "VBNET.ATG" 
out outExpr);
		while (la.kind == 155) {
			lexer.NextToken();
			IntegerDivisionExpr(
#line  1928 "VBNET.ATG" 
out expr);

#line  1928 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.Modulus, expr);  
		}
	}

	void IntegerDivisionExpr(
#line  1931 "VBNET.ATG" 
out Expression outExpr) {

#line  1932 "VBNET.ATG" 
		Expression expr; 
		MultiplicativeExpr(
#line  1934 "VBNET.ATG" 
out outExpr);
		while (la.kind == 26) {
			lexer.NextToken();
			MultiplicativeExpr(
#line  1934 "VBNET.ATG" 
out expr);

#line  1934 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.DivideInteger, expr);  
		}
	}

	void MultiplicativeExpr(
#line  1937 "VBNET.ATG" 
out Expression outExpr) {

#line  1939 "VBNET.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		UnaryExpr(
#line  1942 "VBNET.ATG" 
out outExpr);
		while (la.kind == 25 || la.kind == 35) {
			if (la.kind == 35) {
				lexer.NextToken();

#line  1945 "VBNET.ATG" 
				op = BinaryOperatorType.Multiply; 
			} else {
				lexer.NextToken();

#line  1946 "VBNET.ATG" 
				op = BinaryOperatorType.Divide; 
			}
			UnaryExpr(
#line  1948 "VBNET.ATG" 
out expr);

#line  1948 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr); 
		}
	}

	void UnaryExpr(
#line  1952 "VBNET.ATG" 
out Expression uExpr) {

#line  1954 "VBNET.ATG" 
		Expression expr;
		UnaryOperatorType uop = UnaryOperatorType.None;
		bool isUOp = false;
		
		while (la.kind == 31 || la.kind == 32 || la.kind == 35) {
			if (la.kind == 32) {
				lexer.NextToken();

#line  1958 "VBNET.ATG" 
				uop = UnaryOperatorType.Plus; isUOp = true; 
			} else if (la.kind == 31) {
				lexer.NextToken();

#line  1959 "VBNET.ATG" 
				uop = UnaryOperatorType.Minus; isUOp = true; 
			} else {
				lexer.NextToken();

#line  1960 "VBNET.ATG" 
				uop = UnaryOperatorType.Dereference;  isUOp = true;
			}
		}
		ExponentiationExpr(
#line  1962 "VBNET.ATG" 
out expr);

#line  1964 "VBNET.ATG" 
		if (isUOp) {
		uExpr = new UnaryOperatorExpression(expr, uop);
		} else {
			uExpr = expr;
		}
		
	}

	void ExponentiationExpr(
#line  1972 "VBNET.ATG" 
out Expression outExpr) {

#line  1973 "VBNET.ATG" 
		Expression expr; 
		SimpleExpr(
#line  1975 "VBNET.ATG" 
out outExpr);
		while (la.kind == 33) {
			lexer.NextToken();
			SimpleExpr(
#line  1975 "VBNET.ATG" 
out expr);

#line  1975 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.Power, expr);  
		}
	}

	void NormalOrReDimArgumentList(
#line  2486 "VBNET.ATG" 
out List<Expression> arguments, out bool canBeNormal, out bool canBeRedim) {

#line  2488 "VBNET.ATG" 
		arguments = new List<Expression>();
		canBeNormal = true; canBeRedim = !IsNamedAssign();
		Expression expr = null;
		
		if (StartOf(29)) {
			Argument(
#line  2493 "VBNET.ATG" 
out expr);
			if (la.kind == 216) {
				lexer.NextToken();

#line  2494 "VBNET.ATG" 
				EnsureIsZero(expr); canBeNormal = false; 
				Expr(
#line  2495 "VBNET.ATG" 
out expr);
			}
		}
		while (la.kind == 23) {
			lexer.NextToken();

#line  2498 "VBNET.ATG" 
			if (expr == null) canBeRedim = false; 

#line  2499 "VBNET.ATG" 
			arguments.Add(expr ?? Expression.Null); expr = null; 

#line  2500 "VBNET.ATG" 
			canBeRedim &= !IsNamedAssign(); 
			if (StartOf(29)) {
				Argument(
#line  2501 "VBNET.ATG" 
out expr);
				if (la.kind == 216) {
					lexer.NextToken();

#line  2502 "VBNET.ATG" 
					EnsureIsZero(expr); canBeNormal = false; 
					Expr(
#line  2503 "VBNET.ATG" 
out expr);
				}
			}

#line  2505 "VBNET.ATG" 
			if (expr == null) { canBeRedim = false; expr = Expression.Null; } 
		}

#line  2507 "VBNET.ATG" 
		if (expr != null) arguments.Add(expr); else canBeRedim = false; 
	}

	void ArrayTypeModifiers(
#line  2614 "VBNET.ATG" 
out ArrayList arrayModifiers) {

#line  2616 "VBNET.ATG" 
		arrayModifiers = new ArrayList();
		int i = 0;
		
		while (
#line  2619 "VBNET.ATG" 
IsDims()) {
			Expect(38);
			if (la.kind == 23 || la.kind == 39) {
				RankList(
#line  2621 "VBNET.ATG" 
out i);
			}

#line  2623 "VBNET.ATG" 
			arrayModifiers.Add(i);
			
			Expect(39);
		}

#line  2628 "VBNET.ATG" 
		if(arrayModifiers.Count == 0) {
		 arrayModifiers = null;
		}
		
	}

	void MemberInitializer(
#line  2453 "VBNET.ATG" 
out MemberInitializerExpression memberInitializer) {

#line  2455 "VBNET.ATG" 
		memberInitializer = new MemberInitializerExpression();
		memberInitializer.StartLocation = la.Location;
		Expression initExpr = null;
		bool isKey = false;
		string name = null;
		
		if (la.kind == 148) {
			lexer.NextToken();

#line  2461 "VBNET.ATG" 
			isKey = true; 
		}
		Expect(27);
		IdentifierOrKeyword(
#line  2462 "VBNET.ATG" 
out name);
		Expect(21);
		Expr(
#line  2462 "VBNET.ATG" 
out initExpr);

#line  2464 "VBNET.ATG" 
		memberInitializer.Name = name;
		memberInitializer.Expression = initExpr;
		memberInitializer.IsKey = isKey;
		memberInitializer.EndLocation = t.EndLocation;
		
	}

	void SubLambdaExpression(
#line  2058 "VBNET.ATG" 
out LambdaExpression lambda) {

#line  2060 "VBNET.ATG" 
		lambda = new LambdaExpression();
		lambda.ReturnType = new TypeReference("System.Void", true);
		Expression inner = null;
		Statement statement = null;
		lambda.StartLocation = la.Location;
		
		Expect(210);
		if (la.kind == 38) {
			lexer.NextToken();
			if (StartOf(6)) {
				FormalParameterList(
#line  2067 "VBNET.ATG" 
lambda.Parameters);
			}
			Expect(39);
		}
		if (StartOf(34)) {
			if (StartOf(29)) {
				Expr(
#line  2070 "VBNET.ATG" 
out inner);

#line  2072 "VBNET.ATG" 
				lambda.ExpressionBody = inner;
				lambda.EndLocation = t.EndLocation; // la.Location?
				
			} else {
				EmbeddedStatement(
#line  2076 "VBNET.ATG" 
out statement);

#line  2078 "VBNET.ATG" 
				lambda.StatementBody = statement;
				lambda.EndLocation = t.EndLocation;
				
			}
		} else if (la.kind == 1) {
			lexer.NextToken();
			Block(
#line  2084 "VBNET.ATG" 
out statement);
			Expect(114);
			Expect(210);

#line  2087 "VBNET.ATG" 
			lambda.StatementBody = statement;
			lambda.EndLocation = t.EndLocation;
			
		} else SynErr(282);
	}

	void FunctionLambdaExpression(
#line  2093 "VBNET.ATG" 
out LambdaExpression lambda) {

#line  2095 "VBNET.ATG" 
		lambda = new LambdaExpression();
		TypeReference typeRef = null;
		Expression inner = null;
		Statement statement = null;
		lambda.StartLocation = la.Location;
		
		Expect(128);
		if (la.kind == 38) {
			lexer.NextToken();
			if (StartOf(6)) {
				FormalParameterList(
#line  2102 "VBNET.ATG" 
lambda.Parameters);
			}
			Expect(39);
		}
		if (la.kind == 64) {
			lexer.NextToken();
			TypeName(
#line  2103 "VBNET.ATG" 
out typeRef);

#line  2103 "VBNET.ATG" 
			lambda.ReturnType = typeRef; 
		}
		if (StartOf(34)) {
			if (StartOf(29)) {
				Expr(
#line  2106 "VBNET.ATG" 
out inner);

#line  2108 "VBNET.ATG" 
				lambda.ExpressionBody = inner;
				lambda.EndLocation = t.EndLocation; // la.Location?
				
			} else {
				EmbeddedStatement(
#line  2112 "VBNET.ATG" 
out statement);

#line  2114 "VBNET.ATG" 
				lambda.StatementBody = statement;
				lambda.EndLocation = t.EndLocation;
				
			}
		} else if (la.kind == 1) {
			lexer.NextToken();
			Block(
#line  2120 "VBNET.ATG" 
out statement);
			Expect(114);
			Expect(128);

#line  2123 "VBNET.ATG" 
			lambda.StatementBody = statement;
			lambda.EndLocation = t.EndLocation;
			
		} else SynErr(283);
	}

	void EmbeddedStatement(
#line  2885 "VBNET.ATG" 
out Statement statement) {

#line  2887 "VBNET.ATG" 
		Statement embeddedStatement = null;
		statement = null;
		Expression expr = null;
		string name = String.Empty;
		List<Expression> p = null;
		
		if (la.kind == 121) {
			lexer.NextToken();

#line  2893 "VBNET.ATG" 
			ExitType exitType = ExitType.None; 
			switch (la.kind) {
			case 210: {
				lexer.NextToken();

#line  2895 "VBNET.ATG" 
				exitType = ExitType.Sub; 
				break;
			}
			case 128: {
				lexer.NextToken();

#line  2897 "VBNET.ATG" 
				exitType = ExitType.Function; 
				break;
			}
			case 186: {
				lexer.NextToken();

#line  2899 "VBNET.ATG" 
				exitType = ExitType.Property; 
				break;
			}
			case 109: {
				lexer.NextToken();

#line  2901 "VBNET.ATG" 
				exitType = ExitType.Do; 
				break;
			}
			case 125: {
				lexer.NextToken();

#line  2903 "VBNET.ATG" 
				exitType = ExitType.For; 
				break;
			}
			case 218: {
				lexer.NextToken();

#line  2905 "VBNET.ATG" 
				exitType = ExitType.Try; 
				break;
			}
			case 231: {
				lexer.NextToken();

#line  2907 "VBNET.ATG" 
				exitType = ExitType.While; 
				break;
			}
			case 197: {
				lexer.NextToken();

#line  2909 "VBNET.ATG" 
				exitType = ExitType.Select; 
				break;
			}
			default: SynErr(284); break;
			}

#line  2911 "VBNET.ATG" 
			statement = new ExitStatement(exitType); 
		} else if (la.kind == 218) {
			TryStatement(
#line  2912 "VBNET.ATG" 
out statement);
		} else if (la.kind == 90) {
			lexer.NextToken();

#line  2913 "VBNET.ATG" 
			ContinueType continueType = ContinueType.None; 
			if (la.kind == 109 || la.kind == 125 || la.kind == 231) {
				if (la.kind == 109) {
					lexer.NextToken();

#line  2913 "VBNET.ATG" 
					continueType = ContinueType.Do; 
				} else if (la.kind == 125) {
					lexer.NextToken();

#line  2913 "VBNET.ATG" 
					continueType = ContinueType.For; 
				} else {
					lexer.NextToken();

#line  2913 "VBNET.ATG" 
					continueType = ContinueType.While; 
				}
			}

#line  2913 "VBNET.ATG" 
			statement = new ContinueStatement(continueType); 
		} else if (la.kind == 215) {
			lexer.NextToken();
			if (StartOf(29)) {
				Expr(
#line  2915 "VBNET.ATG" 
out expr);
			}

#line  2915 "VBNET.ATG" 
			statement = new ThrowStatement(expr); 
		} else if (la.kind == 195) {
			lexer.NextToken();
			if (StartOf(29)) {
				Expr(
#line  2917 "VBNET.ATG" 
out expr);
			}

#line  2917 "VBNET.ATG" 
			statement = new ReturnStatement(expr); 
		} else if (la.kind == 211) {
			lexer.NextToken();
			Expr(
#line  2919 "VBNET.ATG" 
out expr);
			EndOfStmt();
			Block(
#line  2919 "VBNET.ATG" 
out embeddedStatement);
			Expect(114);
			Expect(211);

#line  2920 "VBNET.ATG" 
			statement = new LockStatement(expr, embeddedStatement); 
		} else if (la.kind == 189) {
			lexer.NextToken();
			Identifier();

#line  2922 "VBNET.ATG" 
			name = t.val; 
			if (la.kind == 38) {
				lexer.NextToken();
				if (StartOf(35)) {
					ArgumentList(
#line  2923 "VBNET.ATG" 
out p);
				}
				Expect(39);
			}

#line  2925 "VBNET.ATG" 
			statement = new RaiseEventStatement(name, p);
			
		} else if (la.kind == 233) {
			WithStatement(
#line  2928 "VBNET.ATG" 
out statement);
		} else if (la.kind == 57) {
			lexer.NextToken();

#line  2930 "VBNET.ATG" 
			Expression handlerExpr = null; 
			Expr(
#line  2931 "VBNET.ATG" 
out expr);
			Expect(23);
			Expr(
#line  2931 "VBNET.ATG" 
out handlerExpr);

#line  2933 "VBNET.ATG" 
			statement = new AddHandlerStatement(expr, handlerExpr);
			
		} else if (la.kind == 193) {
			lexer.NextToken();

#line  2936 "VBNET.ATG" 
			Expression handlerExpr = null; 
			Expr(
#line  2937 "VBNET.ATG" 
out expr);
			Expect(23);
			Expr(
#line  2937 "VBNET.ATG" 
out handlerExpr);

#line  2939 "VBNET.ATG" 
			statement = new RemoveHandlerStatement(expr, handlerExpr);
			
		} else if (la.kind == 231) {
			lexer.NextToken();
			Expr(
#line  2942 "VBNET.ATG" 
out expr);
			EndOfStmt();
			Block(
#line  2943 "VBNET.ATG" 
out embeddedStatement);
			Expect(114);
			Expect(231);

#line  2945 "VBNET.ATG" 
			statement = new DoLoopStatement(expr, embeddedStatement, ConditionType.While, ConditionPosition.Start);
			
		} else if (la.kind == 109) {
			lexer.NextToken();

#line  2950 "VBNET.ATG" 
			ConditionType conditionType = ConditionType.None;
			
			if (la.kind == 224 || la.kind == 231) {
				WhileOrUntil(
#line  2953 "VBNET.ATG" 
out conditionType);
				Expr(
#line  2953 "VBNET.ATG" 
out expr);
				EndOfStmt();
				Block(
#line  2954 "VBNET.ATG" 
out embeddedStatement);
				Expect(153);

#line  2957 "VBNET.ATG" 
				statement = new DoLoopStatement(expr, 
				                               embeddedStatement, 
				                               conditionType == ConditionType.While ? ConditionType.DoWhile : conditionType, 
				                               ConditionPosition.Start);
				
			} else if (la.kind == 1 || la.kind == 22) {
				EndOfStmt();
				Block(
#line  2964 "VBNET.ATG" 
out embeddedStatement);
				Expect(153);
				if (la.kind == 224 || la.kind == 231) {
					WhileOrUntil(
#line  2965 "VBNET.ATG" 
out conditionType);
					Expr(
#line  2965 "VBNET.ATG" 
out expr);
				}

#line  2967 "VBNET.ATG" 
				statement = new DoLoopStatement(expr, embeddedStatement, conditionType, ConditionPosition.End);
				
			} else SynErr(285);
		} else if (la.kind == 125) {
			lexer.NextToken();

#line  2972 "VBNET.ATG" 
			Expression group = null;
			TypeReference typeReference;
			string        typeName;
			Location startLocation = t.Location;
			
			if (la.kind == 111) {
				lexer.NextToken();
				LoopControlVariable(
#line  2979 "VBNET.ATG" 
out typeReference, out typeName);
				Expect(139);
				Expr(
#line  2980 "VBNET.ATG" 
out group);
				EndOfStmt();
				Block(
#line  2981 "VBNET.ATG" 
out embeddedStatement);
				Expect(164);
				if (StartOf(29)) {
					Expr(
#line  2982 "VBNET.ATG" 
out expr);
				}

#line  2984 "VBNET.ATG" 
				statement = new ForeachStatement(typeReference, 
				                                typeName,
				                                group, 
				                                embeddedStatement, 
				                                expr);
				statement.StartLocation = startLocation;
				statement.EndLocation   = t.EndLocation;
				
				
			} else if (StartOf(36)) {

#line  2995 "VBNET.ATG" 
				Expression start = null;
				Expression end = null;
				Expression step = null;
				Expression variableExpr = null;
				Expression nextExpr = null;
				List<Expression> nextExpressions = null;
				
				if (
#line  3002 "VBNET.ATG" 
IsLoopVariableDeclaration()) {
					LoopControlVariable(
#line  3003 "VBNET.ATG" 
out typeReference, out typeName);
				} else {

#line  3005 "VBNET.ATG" 
					typeReference = null; typeName = null; 
					SimpleExpr(
#line  3006 "VBNET.ATG" 
out variableExpr);
				}
				Expect(21);
				Expr(
#line  3008 "VBNET.ATG" 
out start);
				Expect(216);
				Expr(
#line  3008 "VBNET.ATG" 
out end);
				if (la.kind == 205) {
					lexer.NextToken();
					Expr(
#line  3008 "VBNET.ATG" 
out step);
				}
				EndOfStmt();
				Block(
#line  3009 "VBNET.ATG" 
out embeddedStatement);
				Expect(164);
				if (StartOf(29)) {
					Expr(
#line  3012 "VBNET.ATG" 
out nextExpr);

#line  3014 "VBNET.ATG" 
					nextExpressions = new List<Expression>();
					nextExpressions.Add(nextExpr);
					
					while (la.kind == 23) {
						lexer.NextToken();
						Expr(
#line  3017 "VBNET.ATG" 
out nextExpr);

#line  3017 "VBNET.ATG" 
						nextExpressions.Add(nextExpr); 
					}
				}

#line  3020 "VBNET.ATG" 
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
				
			} else SynErr(286);
		} else if (la.kind == 119) {
			lexer.NextToken();
			Expr(
#line  3033 "VBNET.ATG" 
out expr);

#line  3033 "VBNET.ATG" 
			statement = new ErrorStatement(expr); 
		} else if (la.kind == 191) {
			lexer.NextToken();

#line  3035 "VBNET.ATG" 
			bool isPreserve = false; 
			if (la.kind == 184) {
				lexer.NextToken();

#line  3035 "VBNET.ATG" 
				isPreserve = true; 
			}
			ReDimClause(
#line  3036 "VBNET.ATG" 
out expr);

#line  3038 "VBNET.ATG" 
			ReDimStatement reDimStatement = new ReDimStatement(isPreserve);
			statement = reDimStatement;
			SafeAdd(reDimStatement, reDimStatement.ReDimClauses, expr as InvocationExpression);
			
			while (la.kind == 23) {
				lexer.NextToken();
				ReDimClause(
#line  3042 "VBNET.ATG" 
out expr);

#line  3043 "VBNET.ATG" 
				SafeAdd(reDimStatement, reDimStatement.ReDimClauses, expr as InvocationExpression); 
			}
		} else if (la.kind == 118) {
			lexer.NextToken();
			Expr(
#line  3047 "VBNET.ATG" 
out expr);

#line  3049 "VBNET.ATG" 
			EraseStatement eraseStatement = new EraseStatement();
			if (expr != null) { SafeAdd(eraseStatement, eraseStatement.Expressions, expr);}
			
			while (la.kind == 23) {
				lexer.NextToken();
				Expr(
#line  3052 "VBNET.ATG" 
out expr);

#line  3052 "VBNET.ATG" 
				if (expr != null) { SafeAdd(eraseStatement, eraseStatement.Expressions, expr); }
			}

#line  3053 "VBNET.ATG" 
			statement = eraseStatement; 
		} else if (la.kind == 206) {
			lexer.NextToken();

#line  3055 "VBNET.ATG" 
			statement = new StopStatement(); 
		} else if (
#line  3057 "VBNET.ATG" 
la.kind == Tokens.If) {
			Expect(136);

#line  3058 "VBNET.ATG" 
			Location ifStartLocation = t.Location; 
			Expr(
#line  3058 "VBNET.ATG" 
out expr);
			if (la.kind == 214) {
				lexer.NextToken();
			}
			if (la.kind == 1 || la.kind == 22) {
				EndOfStmt();
				Block(
#line  3061 "VBNET.ATG" 
out embeddedStatement);

#line  3063 "VBNET.ATG" 
				IfElseStatement ifStatement = new IfElseStatement(expr, embeddedStatement);
				ifStatement.StartLocation = ifStartLocation;
				Location elseIfStart;
				
				while (la.kind == 113 || 
#line  3069 "VBNET.ATG" 
IsElseIf()) {
					if (
#line  3069 "VBNET.ATG" 
IsElseIf()) {
						Expect(112);

#line  3069 "VBNET.ATG" 
						elseIfStart = t.Location; 
						Expect(136);
					} else {
						lexer.NextToken();

#line  3070 "VBNET.ATG" 
						elseIfStart = t.Location; 
					}

#line  3072 "VBNET.ATG" 
					Expression condition = null; Statement block = null; 
					Expr(
#line  3073 "VBNET.ATG" 
out condition);
					if (la.kind == 214) {
						lexer.NextToken();
					}
					EndOfStmt();
					Block(
#line  3074 "VBNET.ATG" 
out block);

#line  3076 "VBNET.ATG" 
					ElseIfSection elseIfSection = new ElseIfSection(condition, block);
					elseIfSection.StartLocation = elseIfStart;
					elseIfSection.EndLocation = t.Location;
					elseIfSection.Parent = ifStatement;
					ifStatement.ElseIfSections.Add(elseIfSection);
					
				}
				if (la.kind == 112) {
					lexer.NextToken();
					if (la.kind == 1 || la.kind == 22) {
						EndOfStmt();
					}
					Block(
#line  3085 "VBNET.ATG" 
out embeddedStatement);

#line  3087 "VBNET.ATG" 
					ifStatement.FalseStatement.Add(embeddedStatement);
					
				}
				Expect(114);
				Expect(136);

#line  3091 "VBNET.ATG" 
				ifStatement.EndLocation = t.Location;
				statement = ifStatement;
				
			} else if (StartOf(37)) {

#line  3096 "VBNET.ATG" 
				IfElseStatement ifStatement = new IfElseStatement(expr);
				ifStatement.StartLocation = ifStartLocation;
				
				SingleLineStatementList(
#line  3099 "VBNET.ATG" 
ifStatement.TrueStatement);
				if (la.kind == 112) {
					lexer.NextToken();
					if (StartOf(37)) {
						SingleLineStatementList(
#line  3102 "VBNET.ATG" 
ifStatement.FalseStatement);
					}
				}

#line  3104 "VBNET.ATG" 
				ifStatement.EndLocation = t.Location; statement = ifStatement; 
			} else SynErr(287);
		} else if (la.kind == 197) {
			lexer.NextToken();
			if (la.kind == 75) {
				lexer.NextToken();
			}
			Expr(
#line  3107 "VBNET.ATG" 
out expr);
			EndOfStmt();

#line  3108 "VBNET.ATG" 
			List<SwitchSection> selectSections = new List<SwitchSection>();
			Statement block = null;
			
			while (la.kind == 75) {

#line  3112 "VBNET.ATG" 
				List<CaseLabel> caseClauses = null; Location caseLocation = la.Location; 
				lexer.NextToken();
				CaseClauses(
#line  3113 "VBNET.ATG" 
out caseClauses);
				if (
#line  3113 "VBNET.ATG" 
IsNotStatementSeparator()) {
					lexer.NextToken();
				}
				EndOfStmt();

#line  3115 "VBNET.ATG" 
				SwitchSection selectSection = new SwitchSection(caseClauses);
				selectSection.StartLocation = caseLocation;
				
				Block(
#line  3118 "VBNET.ATG" 
out block);

#line  3120 "VBNET.ATG" 
				selectSection.Children = block.Children;
				selectSection.EndLocation = t.EndLocation;
				selectSections.Add(selectSection);
				
			}

#line  3126 "VBNET.ATG" 
			statement = new SwitchStatement(expr, selectSections);
			
			Expect(114);
			Expect(197);
		} else if (la.kind == 172) {

#line  3129 "VBNET.ATG" 
			OnErrorStatement onErrorStatement = null; 
			OnErrorStatement(
#line  3130 "VBNET.ATG" 
out onErrorStatement);

#line  3130 "VBNET.ATG" 
			statement = onErrorStatement; 
		} else if (la.kind == 133) {

#line  3131 "VBNET.ATG" 
			GotoStatement goToStatement = null; 
			GotoStatement(
#line  3132 "VBNET.ATG" 
out goToStatement);

#line  3132 "VBNET.ATG" 
			statement = goToStatement; 
		} else if (la.kind == 194) {

#line  3133 "VBNET.ATG" 
			ResumeStatement resumeStatement = null; 
			ResumeStatement(
#line  3134 "VBNET.ATG" 
out resumeStatement);

#line  3134 "VBNET.ATG" 
			statement = resumeStatement; 
		} else if (StartOf(36)) {

#line  3137 "VBNET.ATG" 
			Expression val = null;
			AssignmentOperatorType op;
			
			bool mustBeAssignment = la.kind == Tokens.Plus  || la.kind == Tokens.Minus ||
			                        la.kind == Tokens.Not   || la.kind == Tokens.Times;
			
			SimpleExpr(
#line  3143 "VBNET.ATG" 
out expr);
			if (StartOf(38)) {
				AssignmentOperator(
#line  3145 "VBNET.ATG" 
out op);
				Expr(
#line  3145 "VBNET.ATG" 
out val);

#line  3145 "VBNET.ATG" 
				expr = new AssignmentExpression(expr, op, val); 
			} else if (StartOf(39)) {

#line  3146 "VBNET.ATG" 
				if (mustBeAssignment) Error("error in assignment."); 
			} else SynErr(288);

#line  3149 "VBNET.ATG" 
			// a field reference expression that stands alone is a
			// invocation expression without parantheses and arguments
			if(expr is MemberReferenceExpression || expr is IdentifierExpression) {
				expr = new InvocationExpression(expr);
			}
			statement = new ExpressionStatement(expr);
			
		} else if (la.kind == 74) {
			lexer.NextToken();
			SimpleExpr(
#line  3156 "VBNET.ATG" 
out expr);

#line  3156 "VBNET.ATG" 
			statement = new ExpressionStatement(expr); 
		} else if (la.kind == 226) {
			lexer.NextToken();

#line  3158 "VBNET.ATG" 
			Statement block;  
			if (
#line  3159 "VBNET.ATG" 
Peek(1).kind == Tokens.As) {

#line  3160 "VBNET.ATG" 
				LocalVariableDeclaration resourceAquisition = new LocalVariableDeclaration(Modifiers.None); 
				VariableDeclarator(
#line  3161 "VBNET.ATG" 
resourceAquisition.Variables);
				while (la.kind == 23) {
					lexer.NextToken();
					VariableDeclarator(
#line  3163 "VBNET.ATG" 
resourceAquisition.Variables);
				}
				Block(
#line  3165 "VBNET.ATG" 
out block);

#line  3167 "VBNET.ATG" 
				statement = new UsingStatement(resourceAquisition, block);
				
			} else if (StartOf(29)) {
				Expr(
#line  3169 "VBNET.ATG" 
out expr);
				Block(
#line  3170 "VBNET.ATG" 
out block);

#line  3171 "VBNET.ATG" 
				statement = new UsingStatement(new ExpressionStatement(expr), block); 
			} else SynErr(289);
			Expect(114);
			Expect(226);
		} else if (StartOf(40)) {
			LocalDeclarationStatement(
#line  3174 "VBNET.ATG" 
out statement);
		} else SynErr(290);
	}

	void FromOrAggregateQueryOperator(
#line  2142 "VBNET.ATG" 
List<QueryExpressionClause> middleClauses) {

#line  2144 "VBNET.ATG" 
		QueryExpressionFromClause fromClause = null;
		QueryExpressionAggregateClause aggregateClause = null;
		
		if (la.kind == 127) {
			FromQueryOperator(
#line  2147 "VBNET.ATG" 
out fromClause);

#line  2148 "VBNET.ATG" 
			middleClauses.Add(fromClause); 
		} else if (la.kind == 59) {
			AggregateQueryOperator(
#line  2149 "VBNET.ATG" 
out aggregateClause);

#line  2150 "VBNET.ATG" 
			middleClauses.Add(aggregateClause); 
		} else SynErr(291);
	}

	void QueryOperator(
#line  2153 "VBNET.ATG" 
List<QueryExpressionClause> middleClauses) {

#line  2155 "VBNET.ATG" 
		QueryExpressionJoinVBClause joinClause = null;
		QueryExpressionGroupVBClause groupByClause = null;
		QueryExpressionPartitionVBClause partitionClause = null;
		QueryExpressionGroupJoinVBClause groupJoinClause = null;
		QueryExpressionFromClause fromClause = null;
		QueryExpressionAggregateClause aggregateClause = null;
		
		if (la.kind == 127) {
			FromQueryOperator(
#line  2162 "VBNET.ATG" 
out fromClause);

#line  2163 "VBNET.ATG" 
			middleClauses.Add(fromClause); 
		} else if (la.kind == 59) {
			AggregateQueryOperator(
#line  2164 "VBNET.ATG" 
out aggregateClause);

#line  2165 "VBNET.ATG" 
			middleClauses.Add(aggregateClause); 
		} else if (la.kind == 197) {
			SelectQueryOperator(
#line  2166 "VBNET.ATG" 
middleClauses);
		} else if (la.kind == 108) {
			DistinctQueryOperator(
#line  2167 "VBNET.ATG" 
middleClauses);
		} else if (la.kind == 230) {
			WhereQueryOperator(
#line  2168 "VBNET.ATG" 
middleClauses);
		} else if (la.kind == 177) {
			OrderByQueryOperator(
#line  2169 "VBNET.ATG" 
middleClauses);
		} else if (la.kind == 203 || la.kind == 212) {
			PartitionQueryOperator(
#line  2170 "VBNET.ATG" 
out partitionClause);

#line  2171 "VBNET.ATG" 
			middleClauses.Add(partitionClause); 
		} else if (la.kind == 149) {
			LetQueryOperator(
#line  2172 "VBNET.ATG" 
middleClauses);
		} else if (la.kind == 147) {
			JoinQueryOperator(
#line  2173 "VBNET.ATG" 
out joinClause);

#line  2174 "VBNET.ATG" 
			middleClauses.Add(joinClause); 
		} else if (
#line  2175 "VBNET.ATG" 
la.kind == Tokens.Group && Peek(1).kind == Tokens.Join) {
			GroupJoinQueryOperator(
#line  2175 "VBNET.ATG" 
out groupJoinClause);

#line  2176 "VBNET.ATG" 
			middleClauses.Add(groupJoinClause); 
		} else if (la.kind == 134) {
			GroupByQueryOperator(
#line  2177 "VBNET.ATG" 
out groupByClause);

#line  2178 "VBNET.ATG" 
			middleClauses.Add(groupByClause); 
		} else SynErr(292);
	}

	void FromQueryOperator(
#line  2253 "VBNET.ATG" 
out QueryExpressionFromClause fromClause) {

#line  2255 "VBNET.ATG" 
		fromClause = new QueryExpressionFromClause();
		fromClause.StartLocation = la.Location;
		
		Expect(127);
		CollectionRangeVariableDeclarationList(
#line  2258 "VBNET.ATG" 
fromClause.Sources);

#line  2260 "VBNET.ATG" 
		fromClause.EndLocation = t.EndLocation;
		
	}

	void AggregateQueryOperator(
#line  2322 "VBNET.ATG" 
out QueryExpressionAggregateClause aggregateClause) {

#line  2324 "VBNET.ATG" 
		aggregateClause = new QueryExpressionAggregateClause();
		aggregateClause.IntoVariables = new List<ExpressionRangeVariable>();
		aggregateClause.StartLocation = la.Location;
		CollectionRangeVariable source;
		
		Expect(59);
		CollectionRangeVariableDeclaration(
#line  2329 "VBNET.ATG" 
out source);

#line  2331 "VBNET.ATG" 
		aggregateClause.Source = source;
		
		while (StartOf(30)) {
			QueryOperator(
#line  2334 "VBNET.ATG" 
aggregateClause.MiddleClauses);
		}
		Expect(144);
		ExpressionRangeVariableDeclarationList(
#line  2336 "VBNET.ATG" 
aggregateClause.IntoVariables);

#line  2338 "VBNET.ATG" 
		aggregateClause.EndLocation = t.EndLocation;
		
	}

	void SelectQueryOperator(
#line  2264 "VBNET.ATG" 
List<QueryExpressionClause> middleClauses) {

#line  2266 "VBNET.ATG" 
		QueryExpressionSelectVBClause selectClause = new QueryExpressionSelectVBClause();
		selectClause.StartLocation = la.Location;
		
		Expect(197);
		ExpressionRangeVariableDeclarationList(
#line  2269 "VBNET.ATG" 
selectClause.Variables);

#line  2271 "VBNET.ATG" 
		selectClause.EndLocation = t.Location;
		middleClauses.Add(selectClause);
		
	}

	void DistinctQueryOperator(
#line  2276 "VBNET.ATG" 
List<QueryExpressionClause> middleClauses) {

#line  2278 "VBNET.ATG" 
		QueryExpressionDistinctClause distinctClause = new QueryExpressionDistinctClause();
		distinctClause.StartLocation = la.Location;
		
		Expect(108);

#line  2283 "VBNET.ATG" 
		distinctClause.EndLocation = t.EndLocation;
		middleClauses.Add(distinctClause);
		
	}

	void WhereQueryOperator(
#line  2288 "VBNET.ATG" 
List<QueryExpressionClause> middleClauses) {

#line  2290 "VBNET.ATG" 
		QueryExpressionWhereClause whereClause = new QueryExpressionWhereClause();
		whereClause.StartLocation = la.Location;
		Expression operand = null;
		
		Expect(230);
		Expr(
#line  2294 "VBNET.ATG" 
out operand);

#line  2296 "VBNET.ATG" 
		whereClause.Condition = operand;
		whereClause.EndLocation = t.EndLocation;
		
		middleClauses.Add(whereClause);
		
	}

	void OrderByQueryOperator(
#line  2181 "VBNET.ATG" 
List<QueryExpressionClause> middleClauses) {

#line  2183 "VBNET.ATG" 
		QueryExpressionOrderClause orderClause = new QueryExpressionOrderClause();
		orderClause.StartLocation = la.Location;
		List<QueryExpressionOrdering> orderings = null;
		
		Expect(177);
		Expect(71);
		OrderExpressionList(
#line  2187 "VBNET.ATG" 
out orderings);

#line  2189 "VBNET.ATG" 
		orderClause.Orderings = orderings;
		orderClause.EndLocation = t.EndLocation;
		middleClauses.Add(orderClause);
		
	}

	void PartitionQueryOperator(
#line  2303 "VBNET.ATG" 
out QueryExpressionPartitionVBClause partitionClause) {

#line  2305 "VBNET.ATG" 
		partitionClause = new QueryExpressionPartitionVBClause();
		partitionClause.StartLocation = la.Location;
		Expression expr = null;
		
		if (la.kind == 212) {
			lexer.NextToken();

#line  2310 "VBNET.ATG" 
			partitionClause.PartitionType = QueryExpressionPartitionType.Take; 
			if (la.kind == 231) {
				lexer.NextToken();

#line  2311 "VBNET.ATG" 
				partitionClause.PartitionType = QueryExpressionPartitionType.TakeWhile; 
			}
		} else if (la.kind == 203) {
			lexer.NextToken();

#line  2312 "VBNET.ATG" 
			partitionClause.PartitionType = QueryExpressionPartitionType.Skip; 
			if (la.kind == 231) {
				lexer.NextToken();

#line  2313 "VBNET.ATG" 
				partitionClause.PartitionType = QueryExpressionPartitionType.SkipWhile; 
			}
		} else SynErr(293);
		Expr(
#line  2315 "VBNET.ATG" 
out expr);

#line  2317 "VBNET.ATG" 
		partitionClause.Expression = expr;
		partitionClause.EndLocation = t.EndLocation;
		
	}

	void LetQueryOperator(
#line  2342 "VBNET.ATG" 
List<QueryExpressionClause> middleClauses) {

#line  2344 "VBNET.ATG" 
		QueryExpressionLetVBClause letClause = new QueryExpressionLetVBClause();
		letClause.StartLocation = la.Location;
		
		Expect(149);
		ExpressionRangeVariableDeclarationList(
#line  2347 "VBNET.ATG" 
letClause.Variables);

#line  2349 "VBNET.ATG" 
		letClause.EndLocation = t.EndLocation;
		middleClauses.Add(letClause);
		
	}

	void JoinQueryOperator(
#line  2386 "VBNET.ATG" 
out QueryExpressionJoinVBClause joinClause) {

#line  2388 "VBNET.ATG" 
		joinClause = new QueryExpressionJoinVBClause();
		joinClause.StartLocation = la.Location;
		CollectionRangeVariable joinVariable = null;
		QueryExpressionJoinVBClause subJoin = null;
		QueryExpressionJoinConditionVB condition = null;
		
		
		Expect(147);
		CollectionRangeVariableDeclaration(
#line  2395 "VBNET.ATG" 
out joinVariable);

#line  2396 "VBNET.ATG" 
		joinClause.JoinVariable = joinVariable; 
		if (la.kind == 147) {
			JoinQueryOperator(
#line  2398 "VBNET.ATG" 
out subJoin);

#line  2399 "VBNET.ATG" 
			joinClause.SubJoin = subJoin; 
		}
		Expect(172);
		JoinCondition(
#line  2402 "VBNET.ATG" 
out condition);

#line  2403 "VBNET.ATG" 
		SafeAdd(joinClause, joinClause.Conditions, condition); 
		while (la.kind == 61) {
			lexer.NextToken();
			JoinCondition(
#line  2405 "VBNET.ATG" 
out condition);

#line  2406 "VBNET.ATG" 
			SafeAdd(joinClause, joinClause.Conditions, condition); 
		}

#line  2409 "VBNET.ATG" 
		joinClause.EndLocation = t.EndLocation;
		
	}

	void GroupJoinQueryOperator(
#line  2239 "VBNET.ATG" 
out QueryExpressionGroupJoinVBClause groupJoinClause) {

#line  2241 "VBNET.ATG" 
		groupJoinClause = new QueryExpressionGroupJoinVBClause();
		groupJoinClause.StartLocation = la.Location;
		QueryExpressionJoinVBClause joinClause = null;
		
		Expect(134);
		JoinQueryOperator(
#line  2245 "VBNET.ATG" 
out joinClause);
		Expect(144);
		ExpressionRangeVariableDeclarationList(
#line  2246 "VBNET.ATG" 
groupJoinClause.IntoVariables);

#line  2248 "VBNET.ATG" 
		groupJoinClause.JoinClause = joinClause;
		groupJoinClause.EndLocation = t.EndLocation;
		
	}

	void GroupByQueryOperator(
#line  2226 "VBNET.ATG" 
out QueryExpressionGroupVBClause groupByClause) {

#line  2228 "VBNET.ATG" 
		groupByClause = new QueryExpressionGroupVBClause();
		groupByClause.StartLocation = la.Location;
		
		Expect(134);
		ExpressionRangeVariableDeclarationList(
#line  2231 "VBNET.ATG" 
groupByClause.GroupVariables);
		Expect(71);
		ExpressionRangeVariableDeclarationList(
#line  2232 "VBNET.ATG" 
groupByClause.ByVariables);
		Expect(144);
		ExpressionRangeVariableDeclarationList(
#line  2233 "VBNET.ATG" 
groupByClause.IntoVariables);

#line  2235 "VBNET.ATG" 
		groupByClause.EndLocation = t.EndLocation;
		
	}

	void OrderExpressionList(
#line  2195 "VBNET.ATG" 
out List<QueryExpressionOrdering> orderings) {

#line  2197 "VBNET.ATG" 
		orderings = new List<QueryExpressionOrdering>();
		QueryExpressionOrdering ordering = null;
		
		OrderExpression(
#line  2200 "VBNET.ATG" 
out ordering);

#line  2201 "VBNET.ATG" 
		orderings.Add(ordering); 
		while (la.kind == 23) {
			lexer.NextToken();
			OrderExpression(
#line  2203 "VBNET.ATG" 
out ordering);

#line  2204 "VBNET.ATG" 
			orderings.Add(ordering); 
		}
	}

	void OrderExpression(
#line  2208 "VBNET.ATG" 
out QueryExpressionOrdering ordering) {

#line  2210 "VBNET.ATG" 
		ordering = new QueryExpressionOrdering();
		ordering.StartLocation = la.Location;
		ordering.Direction = QueryExpressionOrderingDirection.None;
		Expression orderExpr = null;
		
		Expr(
#line  2215 "VBNET.ATG" 
out orderExpr);

#line  2217 "VBNET.ATG" 
		ordering.Criteria = orderExpr;
		
		if (la.kind == 65 || la.kind == 105) {
			if (la.kind == 65) {
				lexer.NextToken();

#line  2220 "VBNET.ATG" 
				ordering.Direction = QueryExpressionOrderingDirection.Ascending; 
			} else {
				lexer.NextToken();

#line  2221 "VBNET.ATG" 
				ordering.Direction = QueryExpressionOrderingDirection.Descending; 
			}
		}

#line  2223 "VBNET.ATG" 
		ordering.EndLocation = t.EndLocation; 
	}

	void ExpressionRangeVariableDeclarationList(
#line  2354 "VBNET.ATG" 
List<ExpressionRangeVariable> variables) {

#line  2356 "VBNET.ATG" 
		ExpressionRangeVariable variable = null;
		
		ExpressionRangeVariableDeclaration(
#line  2358 "VBNET.ATG" 
out variable);

#line  2359 "VBNET.ATG" 
		variables.Add(variable); 
		while (la.kind == 23) {
			lexer.NextToken();
			ExpressionRangeVariableDeclaration(
#line  2360 "VBNET.ATG" 
out variable);

#line  2360 "VBNET.ATG" 
			variables.Add(variable); 
		}
	}

	void CollectionRangeVariableDeclarationList(
#line  2413 "VBNET.ATG" 
List<CollectionRangeVariable> rangeVariables) {

#line  2414 "VBNET.ATG" 
		CollectionRangeVariable variableDeclaration; 
		CollectionRangeVariableDeclaration(
#line  2416 "VBNET.ATG" 
out variableDeclaration);

#line  2417 "VBNET.ATG" 
		rangeVariables.Add(variableDeclaration); 
		while (la.kind == 23) {
			lexer.NextToken();
			CollectionRangeVariableDeclaration(
#line  2418 "VBNET.ATG" 
out variableDeclaration);

#line  2418 "VBNET.ATG" 
			rangeVariables.Add(variableDeclaration); 
		}
	}

	void CollectionRangeVariableDeclaration(
#line  2421 "VBNET.ATG" 
out CollectionRangeVariable rangeVariable) {

#line  2423 "VBNET.ATG" 
		rangeVariable = new CollectionRangeVariable();
		rangeVariable.StartLocation = la.Location;
		TypeReference typeName = null;
		Expression inExpr = null;
		
		Identifier();

#line  2428 "VBNET.ATG" 
		rangeVariable.Identifier = t.val; 
		if (la.kind == 64) {
			lexer.NextToken();
			TypeName(
#line  2429 "VBNET.ATG" 
out typeName);

#line  2429 "VBNET.ATG" 
			rangeVariable.Type = typeName; 
		}
		Expect(139);
		Expr(
#line  2430 "VBNET.ATG" 
out inExpr);

#line  2432 "VBNET.ATG" 
		rangeVariable.Expression = inExpr;
		rangeVariable.EndLocation = t.EndLocation;
		
	}

	void ExpressionRangeVariableDeclaration(
#line  2363 "VBNET.ATG" 
out ExpressionRangeVariable variable) {

#line  2365 "VBNET.ATG" 
		variable = new ExpressionRangeVariable();
		variable.StartLocation = la.Location;
		Expression rhs = null;
		TypeReference typeName = null;
		
		if (
#line  2371 "VBNET.ATG" 
IsIdentifiedExpressionRange()) {
			Identifier();

#line  2372 "VBNET.ATG" 
			variable.Identifier = t.val; 
			if (la.kind == 64) {
				lexer.NextToken();
				TypeName(
#line  2374 "VBNET.ATG" 
out typeName);

#line  2375 "VBNET.ATG" 
				variable.Type = typeName; 
			}
			Expect(21);
		}
		Expr(
#line  2379 "VBNET.ATG" 
out rhs);

#line  2381 "VBNET.ATG" 
		variable.Expression = rhs;
		variable.EndLocation = t.EndLocation;
		
	}

	void JoinCondition(
#line  2437 "VBNET.ATG" 
out QueryExpressionJoinConditionVB condition) {

#line  2439 "VBNET.ATG" 
		condition = new QueryExpressionJoinConditionVB();
		condition.StartLocation = la.Location;
		
		Expression lhs = null;
		Expression rhs = null;
		
		Expr(
#line  2445 "VBNET.ATG" 
out lhs);
		Expect(117);
		Expr(
#line  2445 "VBNET.ATG" 
out rhs);

#line  2447 "VBNET.ATG" 
		condition.LeftSide = lhs;
		condition.RightSide = rhs;
		condition.EndLocation = t.EndLocation;
		
	}

	void Argument(
#line  2511 "VBNET.ATG" 
out Expression argumentexpr) {

#line  2513 "VBNET.ATG" 
		Expression expr;
		argumentexpr = null;
		string name;
		
		if (
#line  2517 "VBNET.ATG" 
IsNamedAssign()) {
			Identifier();

#line  2517 "VBNET.ATG" 
			name = t.val;  
			Expect(56);
			Expr(
#line  2517 "VBNET.ATG" 
out expr);

#line  2519 "VBNET.ATG" 
			argumentexpr = new NamedArgumentExpression(name, expr);
			
		} else if (StartOf(29)) {
			Expr(
#line  2522 "VBNET.ATG" 
out argumentexpr);
		} else SynErr(294);
	}

	void QualIdentAndTypeArguments(
#line  2588 "VBNET.ATG" 
out TypeReference typeref, bool canBeUnbound) {

#line  2589 "VBNET.ATG" 
		string name; typeref = null; 
		Qualident(
#line  2591 "VBNET.ATG" 
out name);

#line  2592 "VBNET.ATG" 
		typeref = new TypeReference(name); 
		if (
#line  2593 "VBNET.ATG" 
la.kind == Tokens.OpenParenthesis && Peek(1).kind == Tokens.Of) {
			lexer.NextToken();
			Expect(170);
			if (
#line  2595 "VBNET.ATG" 
canBeUnbound && (la.kind == Tokens.CloseParenthesis || la.kind == Tokens.Comma)) {

#line  2596 "VBNET.ATG" 
				typeref.GenericTypes.Add(NullTypeReference.Instance); 
				while (la.kind == 23) {
					lexer.NextToken();

#line  2597 "VBNET.ATG" 
					typeref.GenericTypes.Add(NullTypeReference.Instance); 
				}
			} else if (StartOf(8)) {
				TypeArgumentList(
#line  2598 "VBNET.ATG" 
typeref.GenericTypes);
			} else SynErr(295);
			Expect(39);
		}
	}

	void RankList(
#line  2635 "VBNET.ATG" 
out int i) {

#line  2636 "VBNET.ATG" 
		i = 0; 
		while (la.kind == 23) {
			lexer.NextToken();

#line  2637 "VBNET.ATG" 
			++i; 
		}
	}

	void Attribute(
#line  2676 "VBNET.ATG" 
out ASTAttribute attribute) {

#line  2677 "VBNET.ATG" 
		string name;
		List<Expression> positional = new List<Expression>();
		List<NamedArgumentExpression> named = new List<NamedArgumentExpression>();
		
		if (la.kind == 131) {
			lexer.NextToken();
			Expect(27);
		}
		Qualident(
#line  2682 "VBNET.ATG" 
out name);
		if (la.kind == 38) {
			AttributeArguments(
#line  2683 "VBNET.ATG" 
positional, named);
		}

#line  2685 "VBNET.ATG" 
		attribute  = new ASTAttribute(name, positional, named);
		
	}

	void AttributeArguments(
#line  2690 "VBNET.ATG" 
List<Expression> positional, List<NamedArgumentExpression> named) {

#line  2692 "VBNET.ATG" 
		bool nameFound = false;
		string name = "";
		Expression expr;
		
		Expect(38);
		if (
#line  2698 "VBNET.ATG" 
IsNotClosingParenthesis()) {
			if (
#line  2700 "VBNET.ATG" 
IsNamedAssign()) {

#line  2700 "VBNET.ATG" 
				nameFound = true; 
				IdentifierOrKeyword(
#line  2701 "VBNET.ATG" 
out name);
				if (la.kind == 56) {
					lexer.NextToken();
				} else if (la.kind == 21) {
					lexer.NextToken();
				} else SynErr(296);
			}
			Expr(
#line  2703 "VBNET.ATG" 
out expr);

#line  2705 "VBNET.ATG" 
			if (expr != null) {
			if (string.IsNullOrEmpty(name)) { positional.Add(expr); }
			else { named.Add(new NamedArgumentExpression(name, expr)); name = ""; }
			}
			
			while (la.kind == 23) {
				lexer.NextToken();
				if (
#line  2713 "VBNET.ATG" 
IsNamedAssign()) {

#line  2713 "VBNET.ATG" 
					nameFound = true; 
					IdentifierOrKeyword(
#line  2714 "VBNET.ATG" 
out name);
					if (la.kind == 56) {
						lexer.NextToken();
					} else if (la.kind == 21) {
						lexer.NextToken();
					} else SynErr(297);
				} else if (StartOf(29)) {

#line  2716 "VBNET.ATG" 
					if (nameFound) Error("no positional argument after named argument"); 
				} else SynErr(298);
				Expr(
#line  2717 "VBNET.ATG" 
out expr);

#line  2717 "VBNET.ATG" 
				if (expr != null) { if(name == "") positional.Add(expr);
				else { named.Add(new NamedArgumentExpression(name, expr)); name = ""; }
				}
				
			}
		}
		Expect(39);
	}

	void FormalParameter(
#line  2774 "VBNET.ATG" 
out ParameterDeclarationExpression p) {

#line  2776 "VBNET.ATG" 
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		TypeReference type = null;
		ParamModifierList mod = new ParamModifierList(this);
		Expression expr = null;
		p = null;
		ArrayList arrayModifiers = null;
		
		while (la.kind == 41) {
			AttributeSection(
#line  2785 "VBNET.ATG" 
out section);

#line  2785 "VBNET.ATG" 
			attributes.Add(section); 
		}
		while (StartOf(41)) {
			ParameterModifier(
#line  2786 "VBNET.ATG" 
mod);
		}
		Identifier();

#line  2787 "VBNET.ATG" 
		string parameterName = t.val; 
		if (
#line  2788 "VBNET.ATG" 
IsDims()) {
			ArrayTypeModifiers(
#line  2788 "VBNET.ATG" 
out arrayModifiers);
		}
		if (la.kind == 64) {
			lexer.NextToken();
			TypeName(
#line  2789 "VBNET.ATG" 
out type);
		}

#line  2791 "VBNET.ATG" 
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
#line  2801 "VBNET.ATG" 
out expr);
		}

#line  2803 "VBNET.ATG" 
		mod.Check();
		p = new ParameterDeclarationExpression(type, parameterName, mod.Modifier, expr);
		p.Attributes = attributes;
		
	}

	void ParameterModifier(
#line  3486 "VBNET.ATG" 
ParamModifierList m) {
		if (la.kind == 73) {
			lexer.NextToken();

#line  3487 "VBNET.ATG" 
			m.Add(ParameterModifiers.In); 
		} else if (la.kind == 70) {
			lexer.NextToken();

#line  3488 "VBNET.ATG" 
			m.Add(ParameterModifiers.Ref); 
		} else if (la.kind == 175) {
			lexer.NextToken();

#line  3489 "VBNET.ATG" 
			m.Add(ParameterModifiers.Optional); 
		} else if (la.kind == 182) {
			lexer.NextToken();

#line  3490 "VBNET.ATG" 
			m.Add(ParameterModifiers.Params); 
		} else SynErr(299);
	}

	void Statement() {

#line  2832 "VBNET.ATG" 
		Statement stmt = null;
		Location startPos = la.Location;
		string label = String.Empty;
		
		
		if (la.kind == 1 || la.kind == 22) {
		} else if (
#line  2838 "VBNET.ATG" 
IsLabel()) {
			LabelName(
#line  2838 "VBNET.ATG" 
out label);

#line  2840 "VBNET.ATG" 
			AddChild(new LabelStatement(t.val));
			
			Expect(22);
			Statement();
		} else if (StartOf(42)) {
			EmbeddedStatement(
#line  2843 "VBNET.ATG" 
out stmt);

#line  2843 "VBNET.ATG" 
			AddChild(stmt); 
		} else SynErr(300);

#line  2846 "VBNET.ATG" 
		if (stmt != null) {
		stmt.StartLocation = startPos;
		stmt.EndLocation = t.Location;
		}
		
	}

	void LabelName(
#line  3261 "VBNET.ATG" 
out string name) {

#line  3263 "VBNET.ATG" 
		name = String.Empty;
		
		if (StartOf(4)) {
			Identifier();

#line  3265 "VBNET.ATG" 
			name = t.val; 
		} else if (la.kind == 5) {
			lexer.NextToken();

#line  3266 "VBNET.ATG" 
			name = t.val; 
		} else SynErr(301);
	}

	void LocalDeclarationStatement(
#line  2854 "VBNET.ATG" 
out Statement statement) {

#line  2856 "VBNET.ATG" 
		ModifierList m = new ModifierList();
		LocalVariableDeclaration localVariableDeclaration;
		bool dimfound = false;
		
		while (la.kind == 89 || la.kind == 106 || la.kind == 204) {
			if (la.kind == 89) {
				lexer.NextToken();

#line  2862 "VBNET.ATG" 
				m.Add(Modifiers.Const, t.Location); 
			} else if (la.kind == 204) {
				lexer.NextToken();

#line  2863 "VBNET.ATG" 
				m.Add(Modifiers.Static, t.Location); 
			} else {
				lexer.NextToken();

#line  2864 "VBNET.ATG" 
				dimfound = true; 
			}
		}

#line  2867 "VBNET.ATG" 
		if(dimfound && (m.Modifier & Modifiers.Const) != 0) {
		Error("Dim is not allowed on constants.");
		}
		
		if(m.isNone && dimfound == false) {
			Error("Const, Dim or Static expected");
		}
		
		localVariableDeclaration = new LocalVariableDeclaration(m.Modifier);
		localVariableDeclaration.StartLocation = t.Location;
		
		VariableDeclarator(
#line  2878 "VBNET.ATG" 
localVariableDeclaration.Variables);
		while (la.kind == 23) {
			lexer.NextToken();
			VariableDeclarator(
#line  2879 "VBNET.ATG" 
localVariableDeclaration.Variables);
		}

#line  2881 "VBNET.ATG" 
		statement = localVariableDeclaration;
		
	}

	void TryStatement(
#line  3375 "VBNET.ATG" 
out Statement tryStatement) {

#line  3377 "VBNET.ATG" 
		Statement blockStmt = null, finallyStmt = null;List<CatchClause> catchClauses = null;
		
		Expect(218);
		EndOfStmt();
		Block(
#line  3380 "VBNET.ATG" 
out blockStmt);
		if (la.kind == 76 || la.kind == 114 || la.kind == 124) {
			CatchClauses(
#line  3381 "VBNET.ATG" 
out catchClauses);
		}
		if (la.kind == 124) {
			lexer.NextToken();
			EndOfStmt();
			Block(
#line  3382 "VBNET.ATG" 
out finallyStmt);
		}
		Expect(114);
		Expect(218);

#line  3385 "VBNET.ATG" 
		tryStatement = new TryCatchStatement(blockStmt, catchClauses, finallyStmt);
		
	}

	void WithStatement(
#line  3355 "VBNET.ATG" 
out Statement withStatement) {

#line  3357 "VBNET.ATG" 
		Statement blockStmt = null;
		Expression expr = null;
		
		Expect(233);

#line  3360 "VBNET.ATG" 
		Location start = t.Location; 
		Expr(
#line  3361 "VBNET.ATG" 
out expr);
		EndOfStmt();

#line  3363 "VBNET.ATG" 
		withStatement = new WithStatement(expr);
		withStatement.StartLocation = start;
		
		Block(
#line  3366 "VBNET.ATG" 
out blockStmt);

#line  3368 "VBNET.ATG" 
		((WithStatement)withStatement).Body = (BlockStatement)blockStmt;
		
		Expect(114);
		Expect(233);

#line  3371 "VBNET.ATG" 
		withStatement.EndLocation = t.Location; 
	}

	void WhileOrUntil(
#line  3348 "VBNET.ATG" 
out ConditionType conditionType) {

#line  3349 "VBNET.ATG" 
		conditionType = ConditionType.None; 
		if (la.kind == 231) {
			lexer.NextToken();

#line  3350 "VBNET.ATG" 
			conditionType = ConditionType.While; 
		} else if (la.kind == 224) {
			lexer.NextToken();

#line  3351 "VBNET.ATG" 
			conditionType = ConditionType.Until; 
		} else SynErr(302);
	}

	void LoopControlVariable(
#line  3191 "VBNET.ATG" 
out TypeReference type, out string name) {

#line  3192 "VBNET.ATG" 
		ArrayList arrayModifiers = null;
		type = null;
		
		Qualident(
#line  3196 "VBNET.ATG" 
out name);
		if (
#line  3197 "VBNET.ATG" 
IsDims()) {
			ArrayTypeModifiers(
#line  3197 "VBNET.ATG" 
out arrayModifiers);
		}
		if (la.kind == 64) {
			lexer.NextToken();
			TypeName(
#line  3198 "VBNET.ATG" 
out type);

#line  3198 "VBNET.ATG" 
			if (name.IndexOf('.') > 0) { Error("No type def for 'for each' member indexer allowed."); } 
		}

#line  3200 "VBNET.ATG" 
		if (type != null) {
		if(type.RankSpecifier != null && arrayModifiers != null) {
			Error("array rank only allowed one time");
		} else if (arrayModifiers != null) {
			type.RankSpecifier = (int[])arrayModifiers.ToArray(typeof(int));
		}
		}
		
	}

	void ReDimClause(
#line  3270 "VBNET.ATG" 
out Expression expr) {
		SimpleNonInvocationExpression(
#line  3272 "VBNET.ATG" 
out expr);
		ReDimClauseInternal(
#line  3273 "VBNET.ATG" 
ref expr);
	}

	void SingleLineStatementList(
#line  3177 "VBNET.ATG" 
List<Statement> list) {

#line  3178 "VBNET.ATG" 
		Statement embeddedStatement = null; 
		if (la.kind == 114) {
			lexer.NextToken();

#line  3180 "VBNET.ATG" 
			embeddedStatement = new EndStatement(); 
		} else if (StartOf(42)) {
			EmbeddedStatement(
#line  3181 "VBNET.ATG" 
out embeddedStatement);
		} else SynErr(303);

#line  3182 "VBNET.ATG" 
		if (embeddedStatement != null) list.Add(embeddedStatement); 
		while (la.kind == 22) {
			lexer.NextToken();
			while (la.kind == 22) {
				lexer.NextToken();
			}
			if (la.kind == 114) {
				lexer.NextToken();

#line  3184 "VBNET.ATG" 
				embeddedStatement = new EndStatement(); 
			} else if (StartOf(42)) {
				EmbeddedStatement(
#line  3185 "VBNET.ATG" 
out embeddedStatement);
			} else SynErr(304);

#line  3186 "VBNET.ATG" 
			if (embeddedStatement != null) list.Add(embeddedStatement); 
		}
	}

	void CaseClauses(
#line  3308 "VBNET.ATG" 
out List<CaseLabel> caseClauses) {

#line  3310 "VBNET.ATG" 
		caseClauses = new List<CaseLabel>();
		CaseLabel caseClause = null;
		
		CaseClause(
#line  3313 "VBNET.ATG" 
out caseClause);

#line  3313 "VBNET.ATG" 
		if (caseClause != null) { caseClauses.Add(caseClause); } 
		while (la.kind == 23) {
			lexer.NextToken();
			CaseClause(
#line  3314 "VBNET.ATG" 
out caseClause);

#line  3314 "VBNET.ATG" 
			if (caseClause != null) { caseClauses.Add(caseClause); } 
		}
	}

	void OnErrorStatement(
#line  3211 "VBNET.ATG" 
out OnErrorStatement stmt) {

#line  3213 "VBNET.ATG" 
		stmt = null;
		GotoStatement goToStatement = null;
		
		Expect(172);
		Expect(119);
		if (
#line  3219 "VBNET.ATG" 
IsNegativeLabelName()) {
			Expect(133);
			Expect(31);
			Expect(5);

#line  3221 "VBNET.ATG" 
			long intLabel = Int64.Parse(t.val);
			if(intLabel != 1) {
				Error("invalid label in on error statement.");
			}
			stmt = new OnErrorStatement(new GotoStatement((intLabel * -1).ToString()));
			
		} else if (la.kind == 133) {
			GotoStatement(
#line  3227 "VBNET.ATG" 
out goToStatement);

#line  3229 "VBNET.ATG" 
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
			
		} else if (la.kind == 194) {
			lexer.NextToken();
			Expect(164);

#line  3243 "VBNET.ATG" 
			stmt = new OnErrorStatement(new ResumeStatement(true));
			
		} else SynErr(305);
	}

	void GotoStatement(
#line  3249 "VBNET.ATG" 
out GotoStatement goToStatement) {

#line  3251 "VBNET.ATG" 
		string label = String.Empty;
		
		Expect(133);
		LabelName(
#line  3254 "VBNET.ATG" 
out label);

#line  3256 "VBNET.ATG" 
		goToStatement = new GotoStatement(label);
		
	}

	void ResumeStatement(
#line  3297 "VBNET.ATG" 
out ResumeStatement resumeStatement) {

#line  3299 "VBNET.ATG" 
		resumeStatement = null;
		string label = String.Empty;
		
		if (
#line  3302 "VBNET.ATG" 
IsResumeNext()) {
			Expect(194);
			Expect(164);

#line  3303 "VBNET.ATG" 
			resumeStatement = new ResumeStatement(true); 
		} else if (la.kind == 194) {
			lexer.NextToken();
			if (StartOf(43)) {
				LabelName(
#line  3304 "VBNET.ATG" 
out label);
			}

#line  3304 "VBNET.ATG" 
			resumeStatement = new ResumeStatement(label); 
		} else SynErr(306);
	}

	void ReDimClauseInternal(
#line  3276 "VBNET.ATG" 
ref Expression expr) {

#line  3277 "VBNET.ATG" 
		List<Expression> arguments; bool canBeNormal; bool canBeRedim; string name; 
		while (la.kind == 27 || 
#line  3280 "VBNET.ATG" 
la.kind == Tokens.OpenParenthesis && Peek(1).kind == Tokens.Of) {
			if (la.kind == 27) {
				lexer.NextToken();
				IdentifierOrKeyword(
#line  3279 "VBNET.ATG" 
out name);

#line  3279 "VBNET.ATG" 
				expr = new MemberReferenceExpression(expr, name); 
			} else {
				InvocationExpression(
#line  3281 "VBNET.ATG" 
ref expr);
			}
		}
		Expect(38);
		NormalOrReDimArgumentList(
#line  3284 "VBNET.ATG" 
out arguments, out canBeNormal, out canBeRedim);
		Expect(39);

#line  3286 "VBNET.ATG" 
		expr = new InvocationExpression(expr, arguments);
		if (canBeRedim == false || canBeNormal && (la.kind == Tokens.Dot || la.kind == Tokens.OpenParenthesis)) {
			if (this.Errors.Count == 0) {
				// don't recurse on parse errors - could result in endless recursion
				ReDimClauseInternal(ref expr);
			}
		}
		
	}

	void CaseClause(
#line  3318 "VBNET.ATG" 
out CaseLabel caseClause) {

#line  3320 "VBNET.ATG" 
		Expression expr = null;
		Expression sexpr = null;
		BinaryOperatorType op = BinaryOperatorType.None;
		caseClause = null;
		
		if (la.kind == 112) {
			lexer.NextToken();

#line  3326 "VBNET.ATG" 
			caseClause = new CaseLabel(); 
		} else if (StartOf(44)) {
			if (la.kind == 145) {
				lexer.NextToken();
			}
			switch (la.kind) {
			case 41: {
				lexer.NextToken();

#line  3330 "VBNET.ATG" 
				op = BinaryOperatorType.LessThan; 
				break;
			}
			case 40: {
				lexer.NextToken();

#line  3331 "VBNET.ATG" 
				op = BinaryOperatorType.GreaterThan; 
				break;
			}
			case 44: {
				lexer.NextToken();

#line  3332 "VBNET.ATG" 
				op = BinaryOperatorType.LessThanOrEqual; 
				break;
			}
			case 43: {
				lexer.NextToken();

#line  3333 "VBNET.ATG" 
				op = BinaryOperatorType.GreaterThanOrEqual; 
				break;
			}
			case 21: {
				lexer.NextToken();

#line  3334 "VBNET.ATG" 
				op = BinaryOperatorType.Equality; 
				break;
			}
			case 42: {
				lexer.NextToken();

#line  3335 "VBNET.ATG" 
				op = BinaryOperatorType.InEquality; 
				break;
			}
			default: SynErr(307); break;
			}
			Expr(
#line  3337 "VBNET.ATG" 
out expr);

#line  3339 "VBNET.ATG" 
			caseClause = new CaseLabel(op, expr);
			
		} else if (StartOf(29)) {
			Expr(
#line  3341 "VBNET.ATG" 
out expr);
			if (la.kind == 216) {
				lexer.NextToken();
				Expr(
#line  3341 "VBNET.ATG" 
out sexpr);
			}

#line  3343 "VBNET.ATG" 
			caseClause = new CaseLabel(expr, sexpr);
			
		} else SynErr(308);
	}

	void CatchClauses(
#line  3390 "VBNET.ATG" 
out List<CatchClause> catchClauses) {

#line  3392 "VBNET.ATG" 
		catchClauses = new List<CatchClause>();
		TypeReference type = null;
		Statement blockStmt = null;
		Expression expr = null;
		string name = String.Empty;
		
		while (la.kind == 76) {
			lexer.NextToken();
			if (StartOf(4)) {
				Identifier();

#line  3400 "VBNET.ATG" 
				name = t.val; 
				if (la.kind == 64) {
					lexer.NextToken();
					TypeName(
#line  3400 "VBNET.ATG" 
out type);
				}
			}
			if (la.kind == 229) {
				lexer.NextToken();
				Expr(
#line  3401 "VBNET.ATG" 
out expr);
			}
			EndOfStmt();
			Block(
#line  3403 "VBNET.ATG" 
out blockStmt);

#line  3404 "VBNET.ATG" 
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
			case 28: s = "\"...\" expected"; break;
			case 29: s = "\".@\" expected"; break;
			case 30: s = "\"!\" expected"; break;
			case 31: s = "\"-\" expected"; break;
			case 32: s = "\"+\" expected"; break;
			case 33: s = "\"^\" expected"; break;
			case 34: s = "\"?\" expected"; break;
			case 35: s = "\"*\" expected"; break;
			case 36: s = "\"{\" expected"; break;
			case 37: s = "\"}\" expected"; break;
			case 38: s = "\"(\" expected"; break;
			case 39: s = "\")\" expected"; break;
			case 40: s = "\">\" expected"; break;
			case 41: s = "\"<\" expected"; break;
			case 42: s = "\"<>\" expected"; break;
			case 43: s = "\">=\" expected"; break;
			case 44: s = "\"<=\" expected"; break;
			case 45: s = "\"<<\" expected"; break;
			case 46: s = "\">>\" expected"; break;
			case 47: s = "\"+=\" expected"; break;
			case 48: s = "\"^=\" expected"; break;
			case 49: s = "\"-=\" expected"; break;
			case 50: s = "\"*=\" expected"; break;
			case 51: s = "\"/=\" expected"; break;
			case 52: s = "\"\\\\=\" expected"; break;
			case 53: s = "\"<<=\" expected"; break;
			case 54: s = "\">>=\" expected"; break;
			case 55: s = "\"&=\" expected"; break;
			case 56: s = "\":=\" expected"; break;
			case 57: s = "\"AddHandler\" expected"; break;
			case 58: s = "\"AddressOf\" expected"; break;
			case 59: s = "\"Aggregate\" expected"; break;
			case 60: s = "\"Alias\" expected"; break;
			case 61: s = "\"And\" expected"; break;
			case 62: s = "\"AndAlso\" expected"; break;
			case 63: s = "\"Ansi\" expected"; break;
			case 64: s = "\"As\" expected"; break;
			case 65: s = "\"Ascending\" expected"; break;
			case 66: s = "\"Assembly\" expected"; break;
			case 67: s = "\"Auto\" expected"; break;
			case 68: s = "\"Binary\" expected"; break;
			case 69: s = "\"Boolean\" expected"; break;
			case 70: s = "\"ByRef\" expected"; break;
			case 71: s = "\"By\" expected"; break;
			case 72: s = "\"Byte\" expected"; break;
			case 73: s = "\"ByVal\" expected"; break;
			case 74: s = "\"Call\" expected"; break;
			case 75: s = "\"Case\" expected"; break;
			case 76: s = "\"Catch\" expected"; break;
			case 77: s = "\"CBool\" expected"; break;
			case 78: s = "\"CByte\" expected"; break;
			case 79: s = "\"CChar\" expected"; break;
			case 80: s = "\"CDate\" expected"; break;
			case 81: s = "\"CDbl\" expected"; break;
			case 82: s = "\"CDec\" expected"; break;
			case 83: s = "\"Char\" expected"; break;
			case 84: s = "\"CInt\" expected"; break;
			case 85: s = "\"Class\" expected"; break;
			case 86: s = "\"CLng\" expected"; break;
			case 87: s = "\"CObj\" expected"; break;
			case 88: s = "\"Compare\" expected"; break;
			case 89: s = "\"Const\" expected"; break;
			case 90: s = "\"Continue\" expected"; break;
			case 91: s = "\"CSByte\" expected"; break;
			case 92: s = "\"CShort\" expected"; break;
			case 93: s = "\"CSng\" expected"; break;
			case 94: s = "\"CStr\" expected"; break;
			case 95: s = "\"CType\" expected"; break;
			case 96: s = "\"CUInt\" expected"; break;
			case 97: s = "\"CULng\" expected"; break;
			case 98: s = "\"CUShort\" expected"; break;
			case 99: s = "\"Custom\" expected"; break;
			case 100: s = "\"Date\" expected"; break;
			case 101: s = "\"Decimal\" expected"; break;
			case 102: s = "\"Declare\" expected"; break;
			case 103: s = "\"Default\" expected"; break;
			case 104: s = "\"Delegate\" expected"; break;
			case 105: s = "\"Descending\" expected"; break;
			case 106: s = "\"Dim\" expected"; break;
			case 107: s = "\"DirectCast\" expected"; break;
			case 108: s = "\"Distinct\" expected"; break;
			case 109: s = "\"Do\" expected"; break;
			case 110: s = "\"Double\" expected"; break;
			case 111: s = "\"Each\" expected"; break;
			case 112: s = "\"Else\" expected"; break;
			case 113: s = "\"ElseIf\" expected"; break;
			case 114: s = "\"End\" expected"; break;
			case 115: s = "\"EndIf\" expected"; break;
			case 116: s = "\"Enum\" expected"; break;
			case 117: s = "\"Equals\" expected"; break;
			case 118: s = "\"Erase\" expected"; break;
			case 119: s = "\"Error\" expected"; break;
			case 120: s = "\"Event\" expected"; break;
			case 121: s = "\"Exit\" expected"; break;
			case 122: s = "\"Explicit\" expected"; break;
			case 123: s = "\"False\" expected"; break;
			case 124: s = "\"Finally\" expected"; break;
			case 125: s = "\"For\" expected"; break;
			case 126: s = "\"Friend\" expected"; break;
			case 127: s = "\"From\" expected"; break;
			case 128: s = "\"Function\" expected"; break;
			case 129: s = "\"Get\" expected"; break;
			case 130: s = "\"GetType\" expected"; break;
			case 131: s = "\"Global\" expected"; break;
			case 132: s = "\"GoSub\" expected"; break;
			case 133: s = "\"GoTo\" expected"; break;
			case 134: s = "\"Group\" expected"; break;
			case 135: s = "\"Handles\" expected"; break;
			case 136: s = "\"If\" expected"; break;
			case 137: s = "\"Implements\" expected"; break;
			case 138: s = "\"Imports\" expected"; break;
			case 139: s = "\"In\" expected"; break;
			case 140: s = "\"Infer\" expected"; break;
			case 141: s = "\"Inherits\" expected"; break;
			case 142: s = "\"Integer\" expected"; break;
			case 143: s = "\"Interface\" expected"; break;
			case 144: s = "\"Into\" expected"; break;
			case 145: s = "\"Is\" expected"; break;
			case 146: s = "\"IsNot\" expected"; break;
			case 147: s = "\"Join\" expected"; break;
			case 148: s = "\"Key\" expected"; break;
			case 149: s = "\"Let\" expected"; break;
			case 150: s = "\"Lib\" expected"; break;
			case 151: s = "\"Like\" expected"; break;
			case 152: s = "\"Long\" expected"; break;
			case 153: s = "\"Loop\" expected"; break;
			case 154: s = "\"Me\" expected"; break;
			case 155: s = "\"Mod\" expected"; break;
			case 156: s = "\"Module\" expected"; break;
			case 157: s = "\"MustInherit\" expected"; break;
			case 158: s = "\"MustOverride\" expected"; break;
			case 159: s = "\"MyBase\" expected"; break;
			case 160: s = "\"MyClass\" expected"; break;
			case 161: s = "\"Namespace\" expected"; break;
			case 162: s = "\"Narrowing\" expected"; break;
			case 163: s = "\"New\" expected"; break;
			case 164: s = "\"Next\" expected"; break;
			case 165: s = "\"Not\" expected"; break;
			case 166: s = "\"Nothing\" expected"; break;
			case 167: s = "\"NotInheritable\" expected"; break;
			case 168: s = "\"NotOverridable\" expected"; break;
			case 169: s = "\"Object\" expected"; break;
			case 170: s = "\"Of\" expected"; break;
			case 171: s = "\"Off\" expected"; break;
			case 172: s = "\"On\" expected"; break;
			case 173: s = "\"Operator\" expected"; break;
			case 174: s = "\"Option\" expected"; break;
			case 175: s = "\"Optional\" expected"; break;
			case 176: s = "\"Or\" expected"; break;
			case 177: s = "\"Order\" expected"; break;
			case 178: s = "\"OrElse\" expected"; break;
			case 179: s = "\"Overloads\" expected"; break;
			case 180: s = "\"Overridable\" expected"; break;
			case 181: s = "\"Overrides\" expected"; break;
			case 182: s = "\"ParamArray\" expected"; break;
			case 183: s = "\"Partial\" expected"; break;
			case 184: s = "\"Preserve\" expected"; break;
			case 185: s = "\"Private\" expected"; break;
			case 186: s = "\"Property\" expected"; break;
			case 187: s = "\"Protected\" expected"; break;
			case 188: s = "\"Public\" expected"; break;
			case 189: s = "\"RaiseEvent\" expected"; break;
			case 190: s = "\"ReadOnly\" expected"; break;
			case 191: s = "\"ReDim\" expected"; break;
			case 192: s = "\"Rem\" expected"; break;
			case 193: s = "\"RemoveHandler\" expected"; break;
			case 194: s = "\"Resume\" expected"; break;
			case 195: s = "\"Return\" expected"; break;
			case 196: s = "\"SByte\" expected"; break;
			case 197: s = "\"Select\" expected"; break;
			case 198: s = "\"Set\" expected"; break;
			case 199: s = "\"Shadows\" expected"; break;
			case 200: s = "\"Shared\" expected"; break;
			case 201: s = "\"Short\" expected"; break;
			case 202: s = "\"Single\" expected"; break;
			case 203: s = "\"Skip\" expected"; break;
			case 204: s = "\"Static\" expected"; break;
			case 205: s = "\"Step\" expected"; break;
			case 206: s = "\"Stop\" expected"; break;
			case 207: s = "\"Strict\" expected"; break;
			case 208: s = "\"String\" expected"; break;
			case 209: s = "\"Structure\" expected"; break;
			case 210: s = "\"Sub\" expected"; break;
			case 211: s = "\"SyncLock\" expected"; break;
			case 212: s = "\"Take\" expected"; break;
			case 213: s = "\"Text\" expected"; break;
			case 214: s = "\"Then\" expected"; break;
			case 215: s = "\"Throw\" expected"; break;
			case 216: s = "\"To\" expected"; break;
			case 217: s = "\"True\" expected"; break;
			case 218: s = "\"Try\" expected"; break;
			case 219: s = "\"TryCast\" expected"; break;
			case 220: s = "\"TypeOf\" expected"; break;
			case 221: s = "\"UInteger\" expected"; break;
			case 222: s = "\"ULong\" expected"; break;
			case 223: s = "\"Unicode\" expected"; break;
			case 224: s = "\"Until\" expected"; break;
			case 225: s = "\"UShort\" expected"; break;
			case 226: s = "\"Using\" expected"; break;
			case 227: s = "\"Variant\" expected"; break;
			case 228: s = "\"Wend\" expected"; break;
			case 229: s = "\"When\" expected"; break;
			case 230: s = "\"Where\" expected"; break;
			case 231: s = "\"While\" expected"; break;
			case 232: s = "\"Widening\" expected"; break;
			case 233: s = "\"With\" expected"; break;
			case 234: s = "\"WithEvents\" expected"; break;
			case 235: s = "\"WriteOnly\" expected"; break;
			case 236: s = "\"Xor\" expected"; break;
			case 237: s = "\"GetXmlNamespace\" expected"; break;
			case 238: s = "??? expected"; break;
			case 239: s = "invalid EndOfStmt"; break;
			case 240: s = "invalid OptionStmt"; break;
			case 241: s = "invalid OptionStmt"; break;
			case 242: s = "invalid GlobalAttributeSection"; break;
			case 243: s = "invalid GlobalAttributeSection"; break;
			case 244: s = "invalid NamespaceMemberDecl"; break;
			case 245: s = "invalid OptionValue"; break;
			case 246: s = "invalid ImportClause"; break;
			case 247: s = "invalid Identifier"; break;
			case 248: s = "invalid TypeModifier"; break;
			case 249: s = "invalid NonModuleDeclaration"; break;
			case 250: s = "invalid NonModuleDeclaration"; break;
			case 251: s = "invalid TypeParameterConstraints"; break;
			case 252: s = "invalid TypeParameterConstraint"; break;
			case 253: s = "invalid NonArrayTypeName"; break;
			case 254: s = "invalid MemberModifier"; break;
			case 255: s = "invalid StructureMemberDecl"; break;
			case 256: s = "invalid StructureMemberDecl"; break;
			case 257: s = "invalid StructureMemberDecl"; break;
			case 258: s = "invalid StructureMemberDecl"; break;
			case 259: s = "invalid StructureMemberDecl"; break;
			case 260: s = "invalid StructureMemberDecl"; break;
			case 261: s = "invalid StructureMemberDecl"; break;
			case 262: s = "invalid StructureMemberDecl"; break;
			case 263: s = "invalid InterfaceMemberDecl"; break;
			case 264: s = "invalid InterfaceMemberDecl"; break;
			case 265: s = "invalid Expr"; break;
			case 266: s = "invalid Charset"; break;
			case 267: s = "invalid IdentifierForFieldDeclaration"; break;
			case 268: s = "invalid VariableDeclaratorPartAfterIdentifier"; break;
			case 269: s = "invalid AccessorDecls"; break;
			case 270: s = "invalid EventAccessorDeclaration"; break;
			case 271: s = "invalid OverloadableOperator"; break;
			case 272: s = "invalid EventMemberSpecifier"; break;
			case 273: s = "invalid LambdaExpr"; break;
			case 274: s = "invalid AssignmentOperator"; break;
			case 275: s = "invalid SimpleNonInvocationExpression"; break;
			case 276: s = "invalid SimpleNonInvocationExpression"; break;
			case 277: s = "invalid SimpleNonInvocationExpression"; break;
			case 278: s = "invalid SimpleNonInvocationExpression"; break;
			case 279: s = "invalid PrimitiveTypeName"; break;
			case 280: s = "invalid CastTarget"; break;
			case 281: s = "invalid ComparisonExpr"; break;
			case 282: s = "invalid SubLambdaExpression"; break;
			case 283: s = "invalid FunctionLambdaExpression"; break;
			case 284: s = "invalid EmbeddedStatement"; break;
			case 285: s = "invalid EmbeddedStatement"; break;
			case 286: s = "invalid EmbeddedStatement"; break;
			case 287: s = "invalid EmbeddedStatement"; break;
			case 288: s = "invalid EmbeddedStatement"; break;
			case 289: s = "invalid EmbeddedStatement"; break;
			case 290: s = "invalid EmbeddedStatement"; break;
			case 291: s = "invalid FromOrAggregateQueryOperator"; break;
			case 292: s = "invalid QueryOperator"; break;
			case 293: s = "invalid PartitionQueryOperator"; break;
			case 294: s = "invalid Argument"; break;
			case 295: s = "invalid QualIdentAndTypeArguments"; break;
			case 296: s = "invalid AttributeArguments"; break;
			case 297: s = "invalid AttributeArguments"; break;
			case 298: s = "invalid AttributeArguments"; break;
			case 299: s = "invalid ParameterModifier"; break;
			case 300: s = "invalid Statement"; break;
			case 301: s = "invalid LabelName"; break;
			case 302: s = "invalid WhileOrUntil"; break;
			case 303: s = "invalid SingleLineStatementList"; break;
			case 304: s = "invalid SingleLineStatementList"; break;
			case 305: s = "invalid OnErrorStatement"; break;
			case 306: s = "invalid ResumeStatement"; break;
			case 307: s = "invalid CaseClause"; break;
			case 308: s = "invalid CaseClause"; break;

			default: s = "error " + errorNumber; break;
		}
		this.Errors.Error(line, col, s);
	}
	
	private bool StartOf(int s)
	{
		return set[s, lexer.LookAhead.kind];
	}
	
	static bool[,] set = {
	{T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,x,T, T,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,x,T, T,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,x,T, T,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,T,T,T, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,x, T,x,x,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, T,x,x,x, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,T,T,T, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, T,x,x,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, T,x,x,x, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,T,T,T, T,x,T,T, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,x, T,x,x,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, T,x,x,x, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,T,x,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,T,T,T, T,T,x,T, T,x,x,x, x,x,x,x, x,x,x,T, x,T,x,x, T,x,x,x, x,x,x,x, x,x,x,T, T,T,x,x, x,T,x,x, T,x,T,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,T, x,x,x,T, x,x,T,x, x,x,x,x, T,x,T,x, T,x,x,T, T,x,x,x, T,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,T, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,T,T,T, x,x,x,x, T,T,x,x, T,T,x,x, x,x,x,x, x,T,T,T, T,T,x,x, x,x,T,x, x,x,x,x, x,x,x,x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,T,T,T, T,T,x,T, T,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,T, T,T,x,x, x,T,x,x, T,x,T,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,T, x,x,x,T, x,x,T,x, x,x,x,x, T,x,T,x, T,x,x,T, T,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,T,T,T, x,x,x,x, T,x,x,x, T,T,x,x, x,x,x,x, x,T,T,T, T,T,x,x, x,x,T,x, x,x,x,x, x,x,x,x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,T,T,T, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, T,T,x,x, x,x,x,x, x,x,x,T, x,x,T,T, T,T,T,x, T,x,x,x, x,x,x,x, T,T,x,x, T,x,T,x, x,x,T,T, T,x,x,x, x,x,T,x, x,x,x,x, T,x,x,T, T,x,x,T, T,x,x,x, x,x,x,x, T,T,T,x, x,x,T,x, x,x,x,T, T,x,x,T, x,T,x,x, x,T,x,T, T,T,x,T, T,T,T,T, T,x,T,x, x,x,x,x, x,x,x,T, T,x,x,T, x,x,x,x, x,T,T,x, T,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,T,x, T,x,T,T, x,x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,T, T,T,x,T, x,T,x,T, T,x,T,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,T,T,T, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,x, T,x,x,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,T, x,x,x,T, x,x,T,x, x,x,x,x, T,x,x,x, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, T,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,T,T,T, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,x, T,x,x,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, T,x,x,x, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,T,x, x,x,x,x, x,x,x,x, T,x,x,x, T,x,x,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, T,T,T,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,T, T,T,x,T, x,T,T,T, T,x,T,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, T,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,T, T,T,x,T, x,T,T,T, T,x,T,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
	{x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, T,x,x,T, T,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,T, x,T,T,T, T,T,x,T, T,x,x,x, x,T,T,T, T,T,T,T, T,x,T,T, T,x,x,T, T,T,T,T, T,T,T,T, T,T,x,x, x,T,x,T, T,x,T,x, x,x,x,x, x,T,x,x, x,x,T,T, x,x,x,T, x,x,T,T, x,x,T,x, T,x,x,x, T,x,T,x, T,x,x,T, T,x,x,x, T,x,T,x, x,x,x,T, T,x,x,T, x,T,T,x, x,T,x,T, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,T,T,T, x,x,x,x, T,x,x,x, T,T,x,x, x,T,x,T, T,T,T,T, T,T,x,x, x,x,T,x, x,x,x,x, x,x,x,x},
	{x,T,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,x, x,x,x,x, T,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,T, x,x,x,T, x,T,T,T, T,T,x,T, T,x,T,x, x,T,T,T, T,T,T,T, T,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,x, x,T,T,T, T,T,T,x, x,x,x,x, x,T,T,T, x,T,T,T, x,T,x,T, x,x,T,T, x,T,T,x, T,x,x,x, T,x,T,x, T,x,x,T, T,x,x,x, T,x,T,x, x,x,x,T, T,x,x,T, x,x,T,x, x,T,x,T, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,T,x,T, x,T,T,T, T,T,x,x, x,T,T,T, T,x,T,x, T,x,x,T, T,T,x,T, x,T,T,T, T,T,T,T, T,T,T,x, x,x,T,T, x,T,x,x, x,x,x,x},
	{x,T,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,T,T,T, T,T,T,T, x,x,T,T, T,T,x,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, x,T,T,T, x,T,T,T, T,T,T,T, T,T,x,T, T,x,T,x, x,T,T,T, T,T,T,T, T,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,x, x,T,T,T, T,T,T,x, T,x,T,x, x,T,T,T, x,T,T,T, x,T,x,T, x,x,T,T, x,T,T,x, T,T,x,x, T,x,T,x, T,T,T,T, T,T,x,T, T,x,T,T, x,x,x,T, T,x,x,T, x,x,T,x, x,T,x,T, T,x,x,x, T,T,T,x, x,x,x,x, T,x,x,x, x,T,x,T, x,T,T,T, T,T,x,x, x,T,T,T, T,T,T,x, T,x,x,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, x,x,T,T, x,T,x,x, T,x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, T,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, T,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
	{x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, T,x,x,T, T,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,T, x,T,T,T, T,T,x,T, T,x,x,x, x,T,T,T, T,T,T,T, T,x,T,T, T,x,x,T, T,T,T,T, T,T,T,T, T,T,x,x, x,T,x,T, T,x,T,x, x,x,x,x, x,T,x,x, x,x,T,T, x,x,x,T, T,x,T,T, x,x,T,x, T,x,x,x, T,x,T,x, T,x,x,T, T,x,x,x, T,x,T,x, x,x,x,T, T,x,x,T, x,T,T,x, x,T,x,T, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,T,T,T, x,x,x,x, T,x,T,x, T,T,x,x, x,T,x,T, T,T,T,T, T,T,x,x, x,x,T,x, x,x,x,x, x,x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x},
	{x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,T, x,T,T,T, T,T,x,T, T,x,x,x, x,T,T,T, T,T,T,T, T,x,T,T, T,x,x,T, T,T,T,T, T,T,T,T, T,T,x,x, x,T,x,T, T,x,T,x, x,x,x,x, x,T,x,x, x,x,T,T, x,x,x,T, x,x,T,T, x,x,T,x, T,x,x,x, T,x,T,x, T,x,x,T, T,x,x,x, T,x,T,x, x,x,x,T, T,x,x,T, x,x,T,x, x,T,x,T, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,T,T,T, x,x,x,x, T,x,x,x, T,T,x,x, x,T,x,T, T,T,T,T, T,T,x,x, x,x,T,x, x,x,x,x, x,x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,T,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
	{x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, T,x,x,T, T,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,T, x,T,T,T, T,T,x,T, T,x,x,x, x,T,T,T, T,T,T,T, T,x,T,T, T,x,x,T, T,T,T,T, T,T,T,T, T,T,x,x, x,T,x,T, T,x,T,x, x,x,x,x, x,T,x,x, x,x,T,T, x,x,x,T, x,x,T,T, x,x,T,x, T,x,x,x, T,x,T,x, T,x,x,T, T,x,x,x, T,x,T,x, x,x,x,T, T,x,x,T, x,x,T,x, x,T,x,T, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,T,T,T, x,x,x,x, T,x,x,x, T,T,x,x, x,T,x,T, T,T,T,T, T,T,x,x, x,x,T,x, x,x,x,x, x,x,x,x},
	{x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, T,x,x,T, T,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,T, x,x,x,T, x,T,T,T, T,T,x,T, T,x,T,x, x,T,T,T, T,T,T,T, T,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,x, x,T,T,T, T,T,T,x, x,x,x,x, x,T,T,T, x,T,T,T, x,T,x,T, T,x,T,T, x,T,T,x, T,x,x,x, T,x,T,x, T,x,x,T, T,x,x,x, T,x,T,x, x,x,x,T, T,x,x,T, x,T,T,x, x,T,x,T, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,T,x,T, x,T,T,T, T,T,x,x, x,T,T,T, T,x,T,x, T,x,T,T, T,T,x,T, x,T,T,T, T,T,T,T, T,T,T,x, x,x,T,T, x,T,x,x, x,x,x,x},
	{x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,x,x,T, T,x,x,T, T,x,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,T, x,T,T,T, T,T,x,T, T,x,x,x, x,T,T,T, T,T,T,T, T,x,T,T, T,x,x,T, T,T,T,T, T,T,T,T, T,T,x,x, x,T,x,T, T,x,T,x, x,x,x,x, x,T,x,x, x,x,T,T, x,x,x,T, T,x,T,T, x,x,T,x, T,x,x,x, T,x,T,x, T,x,x,T, T,x,x,x, T,x,T,x, x,x,x,T, T,x,x,T, x,T,T,x, x,T,x,T, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,T,T,T, x,x,x,x, T,x,T,x, T,T,x,x, x,T,x,T, T,T,T,T, T,T,x,x, x,x,T,x, x,x,x,x, x,x,x,x},
	{x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,T, x,T,T,T, T,T,x,T, T,x,x,x, x,T,T,T, T,T,T,T, T,x,T,T, T,x,x,T, T,T,T,T, T,T,T,T, T,T,x,x, x,T,x,T, T,x,T,x, x,x,x,x, x,T,x,x, x,x,T,T, x,x,x,T, x,x,T,T, x,x,T,x, T,x,x,x, T,x,T,x, T,x,x,T, T,x,x,x, T,x,T,x, x,x,x,T, T,x,x,T, x,x,T,x, x,T,x,T, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,T,T,T, x,x,x,x, T,x,x,x, T,T,x,x, x,T,x,T, T,T,T,T, T,T,x,x, x,x,T,x, x,x,x,x, x,x,x,x},
	{x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,T, x,x,x,T, x,T,T,T, T,T,x,T, T,x,T,x, x,T,T,T, T,T,T,T, T,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,x, x,T,T,T, T,T,T,x, x,x,T,x, x,T,T,T, x,T,T,T, x,T,x,T, x,x,T,T, x,T,T,x, T,x,x,x, T,x,T,x, T,x,x,T, T,x,x,x, T,x,T,x, x,x,x,T, T,x,x,T, x,x,T,x, x,T,x,T, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,T,x,T, x,T,T,T, T,T,x,x, x,T,T,T, T,x,T,x, T,x,x,T, T,T,x,T, x,T,T,T, T,T,T,T, T,T,T,x, x,x,T,T, x,T,x,x, x,x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,T,T, T,T,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
	{x,T,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,T,T,T, T,T,T,T, x,x,T,T, T,T,x,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, x,T,T,T, x,T,T,T, x,T,T,T, T,T,x,T, T,x,T,x, x,T,T,T, T,T,T,T, T,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,x, x,T,T,T, T,T,T,x, T,x,T,x, x,T,T,T, x,T,T,T, x,T,x,T, x,x,T,T, x,T,T,x, T,T,x,x, T,x,T,x, T,T,T,T, T,T,x,T, T,x,T,T, x,x,x,T, T,x,x,T, x,x,T,x, x,T,x,T, T,x,x,x, T,T,T,x, x,x,x,x, T,x,x,x, x,T,x,T, x,T,T,T, T,T,x,x, x,T,T,T, T,T,T,x, T,x,x,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, x,x,T,T, x,T,x,x, T,x,x,x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,T,T,T, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,T,x, T,x,x,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, T,x,x,x, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
	{x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,T, x,x,x,T, x,T,T,T, T,T,x,T, T,x,T,x, x,T,T,T, T,T,T,T, T,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,x, x,T,T,T, T,T,T,x, x,x,x,x, x,T,T,T, x,T,T,T, x,T,x,T, x,x,T,T, x,T,T,x, T,x,x,x, T,x,T,x, T,x,x,T, T,x,x,x, T,x,T,x, x,x,x,T, T,x,x,T, x,x,T,x, x,T,x,T, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,T,x,T, x,T,T,T, T,T,x,x, x,T,T,T, T,x,T,x, T,x,x,T, T,T,x,T, x,T,T,T, T,T,T,T, T,T,T,x, x,x,T,T, x,T,x,x, x,x,x,x},
	{x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,T,T,T, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,x, T,x,x,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, T,x,x,x, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,T,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x}

	};
} // end Parser

}