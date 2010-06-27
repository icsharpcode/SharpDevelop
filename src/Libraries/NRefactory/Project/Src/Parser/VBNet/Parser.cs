
//  1 "VBNET.ATG" 
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
	const int maxT = 239;

	const  bool   T            = true;
	const  bool   x            = false;
	

//  12 "VBNET.ATG" 


/*

*/

	void VBNET() {

//  263 "VBNET.ATG" 
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
//  271 "VBNET.ATG" 
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
		} else SynErr(240);
	}

	void OptionStmt() {

//  276 "VBNET.ATG" 
		INode node = null; bool val = true; 
		Expect(174);

//  277 "VBNET.ATG" 
		Location startPos = t.Location; 
		if (la.kind == 122) {
			lexer.NextToken();
			if (la.kind == 171 || la.kind == 172) {
				OptionValue(
//  279 "VBNET.ATG" 
ref val);
			}

//  280 "VBNET.ATG" 
			node = new OptionDeclaration(OptionType.Explicit, val); 
		} else if (la.kind == 208) {
			lexer.NextToken();
			if (la.kind == 171 || la.kind == 172) {
				OptionValue(
//  282 "VBNET.ATG" 
ref val);
			}

//  283 "VBNET.ATG" 
			node = new OptionDeclaration(OptionType.Strict, val); 
		} else if (la.kind == 88) {
			lexer.NextToken();
			if (la.kind == 68) {
				lexer.NextToken();

//  285 "VBNET.ATG" 
				node = new OptionDeclaration(OptionType.CompareBinary, val); 
			} else if (la.kind == 214) {
				lexer.NextToken();

//  286 "VBNET.ATG" 
				node = new OptionDeclaration(OptionType.CompareText, val); 
			} else SynErr(241);
		} else if (la.kind == 140) {
			lexer.NextToken();
			if (la.kind == 171 || la.kind == 172) {
				OptionValue(
//  289 "VBNET.ATG" 
ref val);
			}

//  290 "VBNET.ATG" 
			node = new OptionDeclaration(OptionType.Infer, val); 
		} else SynErr(242);
		EndOfStmt();

//  294 "VBNET.ATG" 
		if (node != null) {
		node.StartLocation = startPos;
		node.EndLocation   = t.Location;
		AddChild(node);
		}
		
	}

	void ImportsStmt() {

//  317 "VBNET.ATG" 
		List<Using> usings = new List<Using>();
		
		Expect(138);

//  321 "VBNET.ATG" 
		Location startPos = t.Location;
		Using u;
		
		ImportClause(
//  324 "VBNET.ATG" 
out u);

//  324 "VBNET.ATG" 
		if (u != null) { usings.Add(u); } 
		while (la.kind == 23) {
			lexer.NextToken();
			ImportClause(
//  326 "VBNET.ATG" 
out u);

//  326 "VBNET.ATG" 
			if (u != null) { usings.Add(u); } 
		}
		EndOfStmt();

//  330 "VBNET.ATG" 
		UsingDeclaration usingDeclaration = new UsingDeclaration(usings);
		usingDeclaration.StartLocation = startPos;
		usingDeclaration.EndLocation   = t.Location;
		AddChild(usingDeclaration);
		
	}

	void GlobalAttributeSection() {
		Expect(41);

//  2654 "VBNET.ATG" 
		Location startPos = t.Location; 
		if (la.kind == 66) {
			lexer.NextToken();
		} else if (la.kind == 156) {
			lexer.NextToken();
		} else SynErr(243);

//  2656 "VBNET.ATG" 
		string attributeTarget = t.val != null ? t.val.ToLower(System.Globalization.CultureInfo.InvariantCulture) : null;
		List<ASTAttribute> attributes = new List<ASTAttribute>();
		ASTAttribute attribute;
		
		Expect(22);
		Attribute(
//  2660 "VBNET.ATG" 
out attribute);

//  2660 "VBNET.ATG" 
		attributes.Add(attribute); 
		while (
//  2661 "VBNET.ATG" 
NotFinalComma()) {
			if (la.kind == 23) {
				lexer.NextToken();
				if (la.kind == 66) {
					lexer.NextToken();
				} else if (la.kind == 156) {
					lexer.NextToken();
				} else SynErr(244);
				Expect(22);
			}
			Attribute(
//  2661 "VBNET.ATG" 
out attribute);

//  2661 "VBNET.ATG" 
			attributes.Add(attribute); 
		}
		if (la.kind == 23) {
			lexer.NextToken();
		}
		Expect(40);
		EndOfStmt();

//  2666 "VBNET.ATG" 
		AttributeSection section = new AttributeSection {
		AttributeTarget = attributeTarget,
		Attributes = attributes,
		StartLocation = startPos,
		EndLocation = t.EndLocation
		};
		AddChild(section);
		
	}

	void NamespaceMemberDecl() {

//  363 "VBNET.ATG" 
		ModifierList m = new ModifierList();
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		string qualident;
		
		if (la.kind == 161) {
			lexer.NextToken();

//  370 "VBNET.ATG" 
			Location startPos = t.Location;
			
			Qualident(
//  372 "VBNET.ATG" 
out qualident);

//  374 "VBNET.ATG" 
			INode node =  new NamespaceDeclaration(qualident);
			node.StartLocation = startPos;
			AddChild(node);
			BlockStart(node);
			
			EndOfStmt();
			NamespaceBody();

//  382 "VBNET.ATG" 
			node.EndLocation = t.Location;
			BlockEnd();
			
		} else if (StartOf(2)) {
			while (la.kind == 41) {
				AttributeSection(
//  386 "VBNET.ATG" 
out section);

//  386 "VBNET.ATG" 
				attributes.Add(section); 
			}
			while (StartOf(3)) {
				TypeModifier(
//  387 "VBNET.ATG" 
m);
			}
			NonModuleDeclaration(
//  387 "VBNET.ATG" 
m, attributes);
		} else SynErr(245);
	}

	void OptionValue(
//  302 "VBNET.ATG" 
ref bool val) {
		if (la.kind == 172) {
			lexer.NextToken();

//  304 "VBNET.ATG" 
			val = true; 
		} else if (la.kind == 171) {
			lexer.NextToken();

//  306 "VBNET.ATG" 
			val = false; 
		} else SynErr(246);
	}

	void ImportClause(
//  337 "VBNET.ATG" 
out Using u) {

//  339 "VBNET.ATG" 
		string qualident  = null;
		TypeReference aliasedType = null;
		u = null;
		
		if (StartOf(4)) {
			Qualident(
//  344 "VBNET.ATG" 
out qualident);
			if (la.kind == 21) {
				lexer.NextToken();
				TypeName(
//  345 "VBNET.ATG" 
out aliasedType);
			}

//  347 "VBNET.ATG" 
			if (qualident != null && qualident.Length > 0) {
			if (aliasedType != null) {
				u = new Using(qualident, aliasedType);
			} else {
				u = new Using(qualident);
			}
			}
			
		} else if (la.kind == 10) {

//  355 "VBNET.ATG" 
			string prefix = null; 
			lexer.NextToken();
			Identifier();

//  356 "VBNET.ATG" 
			prefix = t.val; 
			Expect(21);
			Expect(3);

//  356 "VBNET.ATG" 
			u = new Using(t.literalValue as string, prefix); 
			Expect(11);
		} else SynErr(247);
	}

	void Qualident(
//  3410 "VBNET.ATG" 
out string qualident) {

//  3412 "VBNET.ATG" 
		string name;
		qualidentBuilder.Length = 0; 
		
		Identifier();

//  3416 "VBNET.ATG" 
		qualidentBuilder.Append(t.val); 
		while (
//  3417 "VBNET.ATG" 
DotAndIdentOrKw()) {
			Expect(27);
			IdentifierOrKeyword(
//  3417 "VBNET.ATG" 
out name);

//  3417 "VBNET.ATG" 
			qualidentBuilder.Append('.'); qualidentBuilder.Append(name); 
		}

//  3419 "VBNET.ATG" 
		qualident = qualidentBuilder.ToString(); 
	}

	void TypeName(
//  2527 "VBNET.ATG" 
out TypeReference typeref) {

//  2528 "VBNET.ATG" 
		ArrayList rank = null; 
		NonArrayTypeName(
//  2530 "VBNET.ATG" 
out typeref, false);
		ArrayTypeModifiers(
//  2534 "VBNET.ATG" 
out rank);

//  2535 "VBNET.ATG" 
		if (rank != null && typeref != null) {
		typeref.RankSpecifier = (int[])rank.ToArray(typeof(int));
		}
		
	}

	void Identifier() {
		if (StartOf(5)) {
			IdentifierForFieldDeclaration();
		} else if (la.kind == 99) {
			lexer.NextToken();
		} else SynErr(248);
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
//  2729 "VBNET.ATG" 
out AttributeSection section) {

//  2731 "VBNET.ATG" 
		string attributeTarget = "";List<ASTAttribute> attributes = new List<ASTAttribute>();
		ASTAttribute attribute;
		
		
		Expect(41);

//  2735 "VBNET.ATG" 
		Location startPos = t.Location; 
		if (
//  2736 "VBNET.ATG" 
IsLocalAttrTarget()) {
			if (la.kind == 120) {
				lexer.NextToken();

//  2737 "VBNET.ATG" 
				attributeTarget = "event";
			} else if (la.kind == 196) {
				lexer.NextToken();

//  2738 "VBNET.ATG" 
				attributeTarget = "return";
			} else {
				Identifier();

//  2741 "VBNET.ATG" 
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
//  2751 "VBNET.ATG" 
out attribute);

//  2751 "VBNET.ATG" 
		attributes.Add(attribute); 
		while (
//  2752 "VBNET.ATG" 
NotFinalComma()) {
			Expect(23);
			Attribute(
//  2752 "VBNET.ATG" 
out attribute);

//  2752 "VBNET.ATG" 
			attributes.Add(attribute); 
		}
		if (la.kind == 23) {
			lexer.NextToken();
		}
		Expect(40);

//  2756 "VBNET.ATG" 
		section = new AttributeSection {
		AttributeTarget = attributeTarget,
		Attributes = attributes,
		StartLocation = startPos,
		EndLocation = t.EndLocation
		};
		
	}

	void TypeModifier(
//  3495 "VBNET.ATG" 
ModifierList m) {
		switch (la.kind) {
		case 189: {
			lexer.NextToken();

//  3496 "VBNET.ATG" 
			m.Add(Modifiers.Public, t.Location); 
			break;
		}
		case 188: {
			lexer.NextToken();

//  3497 "VBNET.ATG" 
			m.Add(Modifiers.Protected, t.Location); 
			break;
		}
		case 126: {
			lexer.NextToken();

//  3498 "VBNET.ATG" 
			m.Add(Modifiers.Internal, t.Location); 
			break;
		}
		case 186: {
			lexer.NextToken();

//  3499 "VBNET.ATG" 
			m.Add(Modifiers.Private, t.Location); 
			break;
		}
		case 201: {
			lexer.NextToken();

//  3500 "VBNET.ATG" 
			m.Add(Modifiers.Static, t.Location); 
			break;
		}
		case 200: {
			lexer.NextToken();

//  3501 "VBNET.ATG" 
			m.Add(Modifiers.New, t.Location); 
			break;
		}
		case 157: {
			lexer.NextToken();

//  3502 "VBNET.ATG" 
			m.Add(Modifiers.Abstract, t.Location); 
			break;
		}
		case 167: {
			lexer.NextToken();

//  3503 "VBNET.ATG" 
			m.Add(Modifiers.Sealed, t.Location); 
			break;
		}
		case 184: {
			lexer.NextToken();

//  3504 "VBNET.ATG" 
			m.Add(Modifiers.Partial, t.Location); 
			break;
		}
		default: SynErr(249); break;
		}
	}

	void NonModuleDeclaration(
//  447 "VBNET.ATG" 
ModifierList m, List<AttributeSection> attributes) {

//  449 "VBNET.ATG" 
		TypeReference typeRef = null;
		List<TypeReference> baseInterfaces = null;
		
		switch (la.kind) {
		case 85: {

//  452 "VBNET.ATG" 
			m.Check(Modifiers.Classes); 
			lexer.NextToken();

//  455 "VBNET.ATG" 
			TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
			newType.StartLocation = t.Location;
			AddChild(newType);
			BlockStart(newType);
			
			newType.Type       = ClassType.Class;
			
			Identifier();

//  462 "VBNET.ATG" 
			newType.Name = t.val; 
			TypeParameterList(
//  463 "VBNET.ATG" 
newType.Templates);
			EndOfStmt();

//  465 "VBNET.ATG" 
			newType.BodyStartLocation = t.Location; 
			if (la.kind == 141) {
				ClassBaseType(
//  466 "VBNET.ATG" 
out typeRef);

//  466 "VBNET.ATG" 
				SafeAdd(newType, newType.BaseTypes, typeRef); 
			}
			while (la.kind == 137) {
				TypeImplementsClause(
//  467 "VBNET.ATG" 
out baseInterfaces);

//  467 "VBNET.ATG" 
				newType.BaseTypes.AddRange(baseInterfaces); 
			}
			ClassBody(
//  468 "VBNET.ATG" 
newType);
			Expect(114);
			Expect(85);

//  469 "VBNET.ATG" 
			newType.EndLocation = t.EndLocation; 
			EndOfStmt();

//  472 "VBNET.ATG" 
			BlockEnd();
			
			break;
		}
		case 156: {
			lexer.NextToken();

//  476 "VBNET.ATG" 
			m.Check(Modifiers.VBModules);
			TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
			AddChild(newType);
			BlockStart(newType);
			newType.StartLocation = m.GetDeclarationLocation(t.Location);
			newType.Type = ClassType.Module;
			
			Identifier();

//  483 "VBNET.ATG" 
			newType.Name = t.val; 
			EndOfStmt();

//  485 "VBNET.ATG" 
			newType.BodyStartLocation = t.Location; 
			ModuleBody(
//  486 "VBNET.ATG" 
newType);

//  488 "VBNET.ATG" 
			BlockEnd();
			
			break;
		}
		case 210: {
			lexer.NextToken();

//  492 "VBNET.ATG" 
			m.Check(Modifiers.VBStructures);
			TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
			AddChild(newType);
			BlockStart(newType);
			newType.StartLocation = m.GetDeclarationLocation(t.Location);
			newType.Type = ClassType.Struct;
			
			Identifier();

//  499 "VBNET.ATG" 
			newType.Name = t.val; 
			TypeParameterList(
//  500 "VBNET.ATG" 
newType.Templates);
			EndOfStmt();

//  502 "VBNET.ATG" 
			newType.BodyStartLocation = t.Location; 
			while (la.kind == 137) {
				TypeImplementsClause(
//  503 "VBNET.ATG" 
out baseInterfaces);

//  503 "VBNET.ATG" 
				newType.BaseTypes.AddRange(baseInterfaces);
			}
			StructureBody(
//  504 "VBNET.ATG" 
newType);

//  506 "VBNET.ATG" 
			BlockEnd();
			
			break;
		}
		case 116: {
			lexer.NextToken();

//  511 "VBNET.ATG" 
			m.Check(Modifiers.VBEnums);
			TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
			newType.StartLocation = m.GetDeclarationLocation(t.Location);
			AddChild(newType);
			BlockStart(newType);
			
			newType.Type = ClassType.Enum;
			
			Identifier();

//  519 "VBNET.ATG" 
			newType.Name = t.val; 
			if (la.kind == 64) {
				lexer.NextToken();
				NonArrayTypeName(
//  520 "VBNET.ATG" 
out typeRef, false);

//  520 "VBNET.ATG" 
				SafeAdd(newType, newType.BaseTypes, typeRef); 
			}
			EndOfStmt();

//  522 "VBNET.ATG" 
			newType.BodyStartLocation = t.Location; 
			EnumBody(
//  523 "VBNET.ATG" 
newType);

//  525 "VBNET.ATG" 
			BlockEnd();
			
			break;
		}
		case 143: {
			lexer.NextToken();

//  530 "VBNET.ATG" 
			m.Check(Modifiers.VBInterfacs);
			TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
			newType.StartLocation = m.GetDeclarationLocation(t.Location);
			AddChild(newType);
			BlockStart(newType);
			newType.Type = ClassType.Interface;
			
			Identifier();

//  537 "VBNET.ATG" 
			newType.Name = t.val; 
			TypeParameterList(
//  538 "VBNET.ATG" 
newType.Templates);
			EndOfStmt();

//  540 "VBNET.ATG" 
			newType.BodyStartLocation = t.Location; 
			while (la.kind == 141) {
				InterfaceBase(
//  541 "VBNET.ATG" 
out baseInterfaces);

//  541 "VBNET.ATG" 
				newType.BaseTypes.AddRange(baseInterfaces); 
			}
			InterfaceBody(
//  542 "VBNET.ATG" 
newType);

//  544 "VBNET.ATG" 
			BlockEnd();
			
			break;
		}
		case 104: {
			lexer.NextToken();

//  549 "VBNET.ATG" 
			m.Check(Modifiers.VBDelegates);
			DelegateDeclaration delegateDeclr = new DelegateDeclaration(m.Modifier, attributes);
			delegateDeclr.ReturnType = new TypeReference("System.Void", true);
			delegateDeclr.StartLocation = m.GetDeclarationLocation(t.Location);
			List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
			
			if (la.kind == 211) {
				lexer.NextToken();
				Identifier();

//  556 "VBNET.ATG" 
				delegateDeclr.Name = t.val; 
				TypeParameterList(
//  557 "VBNET.ATG" 
delegateDeclr.Templates);
				if (la.kind == 38) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
//  558 "VBNET.ATG" 
p);
					}
					Expect(39);

//  558 "VBNET.ATG" 
					delegateDeclr.Parameters = p; 
				}
			} else if (la.kind == 128) {
				lexer.NextToken();
				Identifier();

//  560 "VBNET.ATG" 
				delegateDeclr.Name = t.val; 
				TypeParameterList(
//  561 "VBNET.ATG" 
delegateDeclr.Templates);
				if (la.kind == 38) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
//  562 "VBNET.ATG" 
p);
					}
					Expect(39);

//  562 "VBNET.ATG" 
					delegateDeclr.Parameters = p; 
				}
				if (la.kind == 64) {
					lexer.NextToken();

//  563 "VBNET.ATG" 
					TypeReference type; 
					TypeName(
//  563 "VBNET.ATG" 
out type);

//  563 "VBNET.ATG" 
					delegateDeclr.ReturnType = type; 
				}
			} else SynErr(250);

//  565 "VBNET.ATG" 
			delegateDeclr.EndLocation = t.EndLocation; 
			EndOfStmt();

//  568 "VBNET.ATG" 
			AddChild(delegateDeclr);
			
			break;
		}
		default: SynErr(251); break;
		}
	}

	void TypeParameterList(
//  391 "VBNET.ATG" 
List<TemplateDefinition> templates) {

//  393 "VBNET.ATG" 
		TemplateDefinition template;
		
		if (
//  397 "VBNET.ATG" 
la.kind == Tokens.OpenParenthesis && Peek(1).kind == Tokens.Of) {
			lexer.NextToken();
			Expect(170);
			TypeParameter(
//  398 "VBNET.ATG" 
out template);

//  400 "VBNET.ATG" 
			if (template != null) templates.Add(template);
			
			while (la.kind == 23) {
				lexer.NextToken();
				TypeParameter(
//  403 "VBNET.ATG" 
out template);

//  405 "VBNET.ATG" 
				if (template != null) templates.Add(template);
				
			}
			Expect(39);
		}
	}

	void TypeParameter(
//  413 "VBNET.ATG" 
out TemplateDefinition template) {
		Identifier();

//  415 "VBNET.ATG" 
		template = new TemplateDefinition(t.val, null); 
		if (la.kind == 64) {
			TypeParameterConstraints(
//  416 "VBNET.ATG" 
template);
		}
	}

	void TypeParameterConstraints(
//  420 "VBNET.ATG" 
TemplateDefinition template) {

//  422 "VBNET.ATG" 
		TypeReference constraint;
		
		Expect(64);
		if (la.kind == 36) {
			lexer.NextToken();
			TypeParameterConstraint(
//  428 "VBNET.ATG" 
out constraint);

//  428 "VBNET.ATG" 
			if (constraint != null) { template.Bases.Add(constraint); } 
			while (la.kind == 23) {
				lexer.NextToken();
				TypeParameterConstraint(
//  431 "VBNET.ATG" 
out constraint);

//  431 "VBNET.ATG" 
				if (constraint != null) { template.Bases.Add(constraint); } 
			}
			Expect(37);
		} else if (StartOf(7)) {
			TypeParameterConstraint(
//  434 "VBNET.ATG" 
out constraint);

//  434 "VBNET.ATG" 
			if (constraint != null) { template.Bases.Add(constraint); } 
		} else SynErr(252);
	}

	void TypeParameterConstraint(
//  438 "VBNET.ATG" 
out TypeReference constraint) {

//  439 "VBNET.ATG" 
		constraint = null; 
		if (la.kind == 85) {
			lexer.NextToken();

//  440 "VBNET.ATG" 
			constraint = TypeReference.ClassConstraint; 
		} else if (la.kind == 210) {
			lexer.NextToken();

//  441 "VBNET.ATG" 
			constraint = TypeReference.StructConstraint; 
		} else if (la.kind == 163) {
			lexer.NextToken();

//  442 "VBNET.ATG" 
			constraint = TypeReference.NewConstraint; 
		} else if (StartOf(8)) {
			TypeName(
//  443 "VBNET.ATG" 
out constraint);
		} else SynErr(253);
	}

	void ClassBaseType(
//  788 "VBNET.ATG" 
out TypeReference typeRef) {

//  790 "VBNET.ATG" 
		typeRef = null;
		
		Expect(141);
		TypeName(
//  793 "VBNET.ATG" 
out typeRef);
		EndOfStmt();
	}

	void TypeImplementsClause(
//  1605 "VBNET.ATG" 
out List<TypeReference> baseInterfaces) {

//  1607 "VBNET.ATG" 
		baseInterfaces = new List<TypeReference>();
		TypeReference type = null;
		
		Expect(137);
		TypeName(
//  1610 "VBNET.ATG" 
out type);

//  1612 "VBNET.ATG" 
		if (type != null) baseInterfaces.Add(type);
		
		while (la.kind == 23) {
			lexer.NextToken();
			TypeName(
//  1615 "VBNET.ATG" 
out type);

//  1616 "VBNET.ATG" 
			if (type != null) baseInterfaces.Add(type); 
		}
		EndOfStmt();
	}

	void ClassBody(
//  582 "VBNET.ATG" 
TypeDeclaration newType) {

//  583 "VBNET.ATG" 
		AttributeSection section; 
		while (la.kind == 1 || la.kind == 22) {
			EndOfStmt();
		}
		while (StartOf(9)) {

//  586 "VBNET.ATG" 
			List<AttributeSection> attributes = new List<AttributeSection>();
			ModifierList m = new ModifierList();
			
			while (la.kind == 41) {
				AttributeSection(
//  589 "VBNET.ATG" 
out section);

//  589 "VBNET.ATG" 
				attributes.Add(section); 
			}
			while (StartOf(10)) {
				MemberModifier(
//  590 "VBNET.ATG" 
m);
			}
			ClassMemberDecl(
//  591 "VBNET.ATG" 
m, attributes);
			while (la.kind == 1 || la.kind == 22) {
				EndOfStmt();
			}
		}
	}

	void ModuleBody(
//  613 "VBNET.ATG" 
TypeDeclaration newType) {

//  614 "VBNET.ATG" 
		AttributeSection section; 
		while (la.kind == 1 || la.kind == 22) {
			EndOfStmt();
		}
		while (StartOf(9)) {

//  617 "VBNET.ATG" 
			List<AttributeSection> attributes = new List<AttributeSection>();
			ModifierList m = new ModifierList();
			
			while (la.kind == 41) {
				AttributeSection(
//  620 "VBNET.ATG" 
out section);

//  620 "VBNET.ATG" 
				attributes.Add(section); 
			}
			while (StartOf(10)) {
				MemberModifier(
//  621 "VBNET.ATG" 
m);
			}
			ClassMemberDecl(
//  622 "VBNET.ATG" 
m, attributes);
			while (la.kind == 1 || la.kind == 22) {
				EndOfStmt();
			}
		}
		Expect(114);
		Expect(156);

//  625 "VBNET.ATG" 
		newType.EndLocation = t.EndLocation; 
		EndOfStmt();
	}

	void StructureBody(
//  596 "VBNET.ATG" 
TypeDeclaration newType) {

//  597 "VBNET.ATG" 
		AttributeSection section; 
		while (la.kind == 1 || la.kind == 22) {
			EndOfStmt();
		}
		while (StartOf(9)) {

//  600 "VBNET.ATG" 
			List<AttributeSection> attributes = new List<AttributeSection>();
			ModifierList m = new ModifierList();
			
			while (la.kind == 41) {
				AttributeSection(
//  603 "VBNET.ATG" 
out section);

//  603 "VBNET.ATG" 
				attributes.Add(section); 
			}
			while (StartOf(10)) {
				MemberModifier(
//  604 "VBNET.ATG" 
m);
			}
			StructureMemberDecl(
//  605 "VBNET.ATG" 
m, attributes);
			while (la.kind == 1 || la.kind == 22) {
				EndOfStmt();
			}
		}
		Expect(114);
		Expect(210);

//  608 "VBNET.ATG" 
		newType.EndLocation = t.EndLocation; 
		EndOfStmt();
	}

	void NonArrayTypeName(
//  2553 "VBNET.ATG" 
out TypeReference typeref, bool canBeUnbound) {

//  2555 "VBNET.ATG" 
		string name;
		typeref = null;
		bool isGlobal = false;
		
		if (StartOf(11)) {
			if (la.kind == 131) {
				lexer.NextToken();
				Expect(27);

//  2560 "VBNET.ATG" 
				isGlobal = true; 
			}
			QualIdentAndTypeArguments(
//  2561 "VBNET.ATG" 
out typeref, canBeUnbound);

//  2562 "VBNET.ATG" 
			typeref.IsGlobal = isGlobal; 
			while (la.kind == 27) {
				lexer.NextToken();

//  2563 "VBNET.ATG" 
				TypeReference nestedTypeRef; 
				QualIdentAndTypeArguments(
//  2564 "VBNET.ATG" 
out nestedTypeRef, canBeUnbound);

//  2565 "VBNET.ATG" 
				typeref = new InnerClassTypeReference(typeref, nestedTypeRef.Type, nestedTypeRef.GenericTypes); 
			}
		} else if (la.kind == 169) {
			lexer.NextToken();

//  2568 "VBNET.ATG" 
			typeref = new TypeReference("System.Object", true); 
			if (la.kind == 34) {
				lexer.NextToken();

//  2572 "VBNET.ATG" 
				List<TypeReference> typeArguments = new List<TypeReference>(1);
				if (typeref != null) typeArguments.Add(typeref);
				typeref = new TypeReference("System.Nullable", typeArguments) { IsKeyword = true };
				
			}
		} else if (StartOf(12)) {
			PrimitiveTypeName(
//  2578 "VBNET.ATG" 
out name);

//  2578 "VBNET.ATG" 
			typeref = new TypeReference(name, true); 
			if (la.kind == 34) {
				lexer.NextToken();

//  2582 "VBNET.ATG" 
				List<TypeReference> typeArguments = new List<TypeReference>(1);
				if (typeref != null) typeArguments.Add(typeref);
				typeref = new TypeReference("System.Nullable", typeArguments) { IsKeyword = true };
				
			}
		} else SynErr(254);
	}

	void EnumBody(
//  629 "VBNET.ATG" 
TypeDeclaration newType) {

//  630 "VBNET.ATG" 
		FieldDeclaration f; 
		while (la.kind == 1 || la.kind == 22) {
			EndOfStmt();
		}
		while (StartOf(13)) {
			EnumMemberDecl(
//  633 "VBNET.ATG" 
out f);

//  635 "VBNET.ATG" 
			AddChild(f);
			
			while (la.kind == 1 || la.kind == 22) {
				EndOfStmt();
			}
		}
		Expect(114);
		Expect(116);

//  639 "VBNET.ATG" 
		newType.EndLocation = t.EndLocation; 
		EndOfStmt();
	}

	void InterfaceBase(
//  1590 "VBNET.ATG" 
out List<TypeReference> bases) {

//  1592 "VBNET.ATG" 
		TypeReference type;
		bases = new List<TypeReference>();
		
		Expect(141);
		TypeName(
//  1596 "VBNET.ATG" 
out type);

//  1596 "VBNET.ATG" 
		if (type != null) bases.Add(type); 
		while (la.kind == 23) {
			lexer.NextToken();
			TypeName(
//  1599 "VBNET.ATG" 
out type);

//  1599 "VBNET.ATG" 
			if (type != null) bases.Add(type); 
		}
		EndOfStmt();
	}

	void InterfaceBody(
//  643 "VBNET.ATG" 
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

//  649 "VBNET.ATG" 
		newType.EndLocation = t.EndLocation; 
		EndOfStmt();
	}

	void FormalParameterList(
//  2766 "VBNET.ATG" 
List<ParameterDeclarationExpression> parameter) {

//  2767 "VBNET.ATG" 
		ParameterDeclarationExpression p; 
		FormalParameter(
//  2769 "VBNET.ATG" 
out p);

//  2769 "VBNET.ATG" 
		if (p != null) parameter.Add(p); 
		while (la.kind == 23) {
			lexer.NextToken();
			FormalParameter(
//  2771 "VBNET.ATG" 
out p);

//  2771 "VBNET.ATG" 
			if (p != null) parameter.Add(p); 
		}
	}

	void MemberModifier(
//  3507 "VBNET.ATG" 
ModifierList m) {
		switch (la.kind) {
		case 157: {
			lexer.NextToken();

//  3508 "VBNET.ATG" 
			m.Add(Modifiers.Abstract, t.Location);
			break;
		}
		case 103: {
			lexer.NextToken();

//  3509 "VBNET.ATG" 
			m.Add(Modifiers.Default, t.Location);
			break;
		}
		case 126: {
			lexer.NextToken();

//  3510 "VBNET.ATG" 
			m.Add(Modifiers.Internal, t.Location);
			break;
		}
		case 200: {
			lexer.NextToken();

//  3511 "VBNET.ATG" 
			m.Add(Modifiers.New, t.Location);
			break;
		}
		case 182: {
			lexer.NextToken();

//  3512 "VBNET.ATG" 
			m.Add(Modifiers.Override, t.Location);
			break;
		}
		case 158: {
			lexer.NextToken();

//  3513 "VBNET.ATG" 
			m.Add(Modifiers.Abstract, t.Location);
			break;
		}
		case 186: {
			lexer.NextToken();

//  3514 "VBNET.ATG" 
			m.Add(Modifiers.Private, t.Location);
			break;
		}
		case 188: {
			lexer.NextToken();

//  3515 "VBNET.ATG" 
			m.Add(Modifiers.Protected, t.Location);
			break;
		}
		case 189: {
			lexer.NextToken();

//  3516 "VBNET.ATG" 
			m.Add(Modifiers.Public, t.Location);
			break;
		}
		case 167: {
			lexer.NextToken();

//  3517 "VBNET.ATG" 
			m.Add(Modifiers.Sealed, t.Location);
			break;
		}
		case 168: {
			lexer.NextToken();

//  3518 "VBNET.ATG" 
			m.Add(Modifiers.Sealed, t.Location);
			break;
		}
		case 201: {
			lexer.NextToken();

//  3519 "VBNET.ATG" 
			m.Add(Modifiers.Static, t.Location);
			break;
		}
		case 181: {
			lexer.NextToken();

//  3520 "VBNET.ATG" 
			m.Add(Modifiers.Virtual, t.Location);
			break;
		}
		case 180: {
			lexer.NextToken();

//  3521 "VBNET.ATG" 
			m.Add(Modifiers.Overloads, t.Location);
			break;
		}
		case 191: {
			lexer.NextToken();

//  3522 "VBNET.ATG" 
			m.Add(Modifiers.ReadOnly, t.Location);
			break;
		}
		case 236: {
			lexer.NextToken();

//  3523 "VBNET.ATG" 
			m.Add(Modifiers.WriteOnly, t.Location);
			break;
		}
		case 235: {
			lexer.NextToken();

//  3524 "VBNET.ATG" 
			m.Add(Modifiers.WithEvents, t.Location);
			break;
		}
		case 106: {
			lexer.NextToken();

//  3525 "VBNET.ATG" 
			m.Add(Modifiers.Dim, t.Location);
			break;
		}
		case 184: {
			lexer.NextToken();

//  3526 "VBNET.ATG" 
			m.Add(Modifiers.Partial, t.Location);
			break;
		}
		default: SynErr(255); break;
		}
	}

	void ClassMemberDecl(
//  784 "VBNET.ATG" 
ModifierList m, List<AttributeSection> attributes) {
		StructureMemberDecl(
//  785 "VBNET.ATG" 
m, attributes);
	}

	void StructureMemberDecl(
//  798 "VBNET.ATG" 
ModifierList m, List<AttributeSection> attributes) {

//  800 "VBNET.ATG" 
		TypeReference type = null;
		List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
		Statement stmt = null;
		List<VariableDeclaration> variableDeclarators = new List<VariableDeclaration>();
		List<TemplateDefinition> templates = new List<TemplateDefinition>();
		
		switch (la.kind) {
		case 85: case 104: case 116: case 143: case 156: case 210: {
			NonModuleDeclaration(
//  807 "VBNET.ATG" 
m, attributes);
			break;
		}
		case 211: {
			lexer.NextToken();

//  811 "VBNET.ATG" 
			Location startPos = t.Location;
			
			if (StartOf(4)) {

//  815 "VBNET.ATG" 
				string name = String.Empty;
				MethodDeclaration methodDeclaration; List<string> handlesClause = null;
				List<InterfaceImplementation> implementsClause = null;
				
				Identifier();

//  821 "VBNET.ATG" 
				name = t.val;
				m.Check(Modifiers.VBMethods);
				
				TypeParameterList(
//  824 "VBNET.ATG" 
templates);
				if (la.kind == 38) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
//  825 "VBNET.ATG" 
p);
					}
					Expect(39);
				}
				if (la.kind == 135 || la.kind == 137) {
					if (la.kind == 137) {
						ImplementsClause(
//  828 "VBNET.ATG" 
out implementsClause);
					} else {
						HandlesClause(
//  830 "VBNET.ATG" 
out handlesClause);
					}
				}

//  833 "VBNET.ATG" 
				Location endLocation = t.EndLocation; 
				if (
//  836 "VBNET.ATG" 
IsMustOverride(m)) {
					EndOfStmt();

//  839 "VBNET.ATG" 
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

//  852 "VBNET.ATG" 
					methodDeclaration = new MethodDeclaration {
					Name = name, Modifier = m.Modifier, Parameters = p, Attributes = attributes,
					StartLocation = m.GetDeclarationLocation(startPos), EndLocation = endLocation,
					TypeReference = new TypeReference("System.Void", true),
					Templates = templates,
					HandlesClause = handlesClause,
					InterfaceImplementations = implementsClause
					};
					AddChild(methodDeclaration);
					

//  863 "VBNET.ATG" 
					if (ParseMethodBodies) { 
					Block(
//  864 "VBNET.ATG" 
out stmt);
					Expect(114);
					Expect(211);

//  866 "VBNET.ATG" 
					} else {
					// don't parse method body
					lexer.SkipCurrentBlock(Tokens.Sub); stmt = new BlockStatement();
					  }
					

//  872 "VBNET.ATG" 
					methodDeclaration.Body  = (BlockStatement)stmt; 

//  873 "VBNET.ATG" 
					methodDeclaration.Body.EndLocation = t.EndLocation; 
					EndOfStmt();
				} else SynErr(256);
			} else if (la.kind == 163) {
				lexer.NextToken();
				if (la.kind == 38) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
//  877 "VBNET.ATG" 
p);
					}
					Expect(39);
				}

//  878 "VBNET.ATG" 
				m.Check(Modifiers.Constructors); 

//  879 "VBNET.ATG" 
				Location constructorEndLocation = t.EndLocation; 
				Expect(1);

//  882 "VBNET.ATG" 
				if (ParseMethodBodies) { 
				Block(
//  883 "VBNET.ATG" 
out stmt);
				Expect(114);
				Expect(211);

//  885 "VBNET.ATG" 
				} else {
				// don't parse method body
				lexer.SkipCurrentBlock(Tokens.Sub); stmt = new BlockStatement();
				  }
				

//  891 "VBNET.ATG" 
				Location endLocation = t.EndLocation; 
				EndOfStmt();

//  894 "VBNET.ATG" 
				ConstructorDeclaration cd = new ConstructorDeclaration("New", m.Modifier, p, attributes);
				cd.StartLocation = m.GetDeclarationLocation(startPos);
				cd.EndLocation   = constructorEndLocation;
				cd.Body = (BlockStatement)stmt;
				cd.Body.EndLocation   = endLocation;
				AddChild(cd);
				
			} else SynErr(257);
			break;
		}
		case 128: {
			lexer.NextToken();

//  906 "VBNET.ATG" 
			m.Check(Modifiers.VBMethods);
			string name = String.Empty;
			Location startPos = t.Location;
			MethodDeclaration methodDeclaration;List<string> handlesClause = null;
			List<InterfaceImplementation> implementsClause = null;
			AttributeSection returnTypeAttributeSection = null;
			
			Identifier();

//  913 "VBNET.ATG" 
			name = t.val; 
			TypeParameterList(
//  914 "VBNET.ATG" 
templates);
			if (la.kind == 38) {
				lexer.NextToken();
				if (StartOf(6)) {
					FormalParameterList(
//  915 "VBNET.ATG" 
p);
				}
				Expect(39);
			}
			if (la.kind == 64) {
				lexer.NextToken();
				while (la.kind == 41) {
					AttributeSection(
//  917 "VBNET.ATG" 
out returnTypeAttributeSection);

//  919 "VBNET.ATG" 
					if (returnTypeAttributeSection != null) {
					returnTypeAttributeSection.AttributeTarget = "return";
					attributes.Add(returnTypeAttributeSection);
					}
					
				}
				TypeName(
//  925 "VBNET.ATG" 
out type);
			}

//  927 "VBNET.ATG" 
			if(type == null) {
			type = new TypeReference("System.Object", true);
			}
			
			if (la.kind == 135 || la.kind == 137) {
				if (la.kind == 137) {
					ImplementsClause(
//  933 "VBNET.ATG" 
out implementsClause);
				} else {
					HandlesClause(
//  935 "VBNET.ATG" 
out handlesClause);
				}
			}

//  938 "VBNET.ATG" 
			Location endLocation = t.EndLocation; 
			if (
//  941 "VBNET.ATG" 
IsMustOverride(m)) {
				EndOfStmt();

//  944 "VBNET.ATG" 
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

//  959 "VBNET.ATG" 
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
//  972 "VBNET.ATG" 
out stmt);
				Expect(114);
				Expect(128);

//  974 "VBNET.ATG" 
				} else {
				// don't parse method body
				lexer.SkipCurrentBlock(Tokens.Function); stmt = new BlockStatement();
				}
				methodDeclaration.Body = (BlockStatement)stmt;
				methodDeclaration.Body.StartLocation = methodDeclaration.EndLocation;
				methodDeclaration.Body.EndLocation   = t.EndLocation;
				
				EndOfStmt();
			} else SynErr(258);
			break;
		}
		case 102: {
			lexer.NextToken();

//  988 "VBNET.ATG" 
			m.Check(Modifiers.VBExternalMethods);
			Location startPos = t.Location;
			CharsetModifier charsetModifer = CharsetModifier.None;
			string library = String.Empty;
			string alias = null;
			string name = String.Empty;
			
			if (StartOf(15)) {
				Charset(
//  995 "VBNET.ATG" 
out charsetModifer);
			}
			if (la.kind == 211) {
				lexer.NextToken();
				Identifier();

//  998 "VBNET.ATG" 
				name = t.val; 
				Expect(150);
				Expect(3);

//  999 "VBNET.ATG" 
				library = t.literalValue as string; 
				if (la.kind == 60) {
					lexer.NextToken();
					Expect(3);

//  1000 "VBNET.ATG" 
					alias = t.literalValue as string; 
				}
				if (la.kind == 38) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
//  1001 "VBNET.ATG" 
p);
					}
					Expect(39);
				}
				EndOfStmt();

//  1004 "VBNET.ATG" 
				DeclareDeclaration declareDeclaration = new DeclareDeclaration(name, m.Modifier, null, p, attributes, library, alias, charsetModifer);
				declareDeclaration.StartLocation = m.GetDeclarationLocation(startPos);
				declareDeclaration.EndLocation   = t.EndLocation;
				AddChild(declareDeclaration);
				
			} else if (la.kind == 128) {
				lexer.NextToken();
				Identifier();

//  1011 "VBNET.ATG" 
				name = t.val; 
				Expect(150);
				Expect(3);

//  1012 "VBNET.ATG" 
				library = t.literalValue as string; 
				if (la.kind == 60) {
					lexer.NextToken();
					Expect(3);

//  1013 "VBNET.ATG" 
					alias = t.literalValue as string; 
				}
				if (la.kind == 38) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
//  1014 "VBNET.ATG" 
p);
					}
					Expect(39);
				}
				if (la.kind == 64) {
					lexer.NextToken();
					TypeName(
//  1015 "VBNET.ATG" 
out type);
				}
				EndOfStmt();

//  1018 "VBNET.ATG" 
				DeclareDeclaration declareDeclaration = new DeclareDeclaration(name, m.Modifier, type, p, attributes, library, alias, charsetModifer);
				declareDeclaration.StartLocation = m.GetDeclarationLocation(startPos);
				declareDeclaration.EndLocation   = t.EndLocation;
				AddChild(declareDeclaration);
				
			} else SynErr(259);
			break;
		}
		case 120: {
			lexer.NextToken();

//  1028 "VBNET.ATG" 
			m.Check(Modifiers.VBEvents);
			Location startPos = t.Location;
			EventDeclaration eventDeclaration;
			string name = String.Empty;
			List<InterfaceImplementation> implementsClause = null;
			
			Identifier();

//  1034 "VBNET.ATG" 
			name= t.val; 
			if (la.kind == 64) {
				lexer.NextToken();
				TypeName(
//  1036 "VBNET.ATG" 
out type);
			} else if (StartOf(16)) {
				if (la.kind == 38) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
//  1038 "VBNET.ATG" 
p);
					}
					Expect(39);
				}
			} else SynErr(260);
			if (la.kind == 137) {
				ImplementsClause(
//  1040 "VBNET.ATG" 
out implementsClause);
			}

//  1042 "VBNET.ATG" 
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
		case 2: case 59: case 63: case 65: case 66: case 67: case 68: case 71: case 88: case 105: case 108: case 117: case 122: case 127: case 134: case 140: case 144: case 147: case 148: case 171: case 177: case 179: case 185: case 204: case 213: case 214: case 224: case 225: case 231: {

//  1053 "VBNET.ATG" 
			m.Check(Modifiers.Fields);
			FieldDeclaration fd = new FieldDeclaration(attributes, null, m.Modifier);
			
			IdentifierForFieldDeclaration();

//  1056 "VBNET.ATG" 
			string name = t.val; 

//  1057 "VBNET.ATG" 
			fd.StartLocation = m.GetDeclarationLocation(t.Location); 
			VariableDeclaratorPartAfterIdentifier(
//  1059 "VBNET.ATG" 
variableDeclarators, name);
			while (la.kind == 23) {
				lexer.NextToken();
				VariableDeclarator(
//  1060 "VBNET.ATG" 
variableDeclarators);
			}
			EndOfStmt();

//  1063 "VBNET.ATG" 
			fd.EndLocation = t.EndLocation;
			fd.Fields = variableDeclarators;
			AddChild(fd);
			
			break;
		}
		case 89: {

//  1068 "VBNET.ATG" 
			m.Check(Modifiers.Fields); 
			lexer.NextToken();

//  1069 "VBNET.ATG" 
			m.Add(Modifiers.Const, t.Location);  

//  1071 "VBNET.ATG" 
			FieldDeclaration fd = new FieldDeclaration(attributes, type, m.Modifier);
			fd.StartLocation = m.GetDeclarationLocation(t.Location);
			List<VariableDeclaration> constantDeclarators = new List<VariableDeclaration>();
			
			ConstantDeclarator(
//  1075 "VBNET.ATG" 
constantDeclarators);
			while (la.kind == 23) {
				lexer.NextToken();
				ConstantDeclarator(
//  1076 "VBNET.ATG" 
constantDeclarators);
			}

//  1078 "VBNET.ATG" 
			fd.Fields = constantDeclarators;
			fd.EndLocation = t.Location;
			
			EndOfStmt();

//  1083 "VBNET.ATG" 
			fd.EndLocation = t.EndLocation;
			AddChild(fd);
			
			break;
		}
		case 187: {
			lexer.NextToken();

//  1089 "VBNET.ATG" 
			m.Check(Modifiers.VBProperties);
			Location startPos = t.Location;
			List<InterfaceImplementation> implementsClause = null;
			AttributeSection returnTypeAttributeSection = null;
			Expression initializer = null;
			
			Identifier();

//  1095 "VBNET.ATG" 
			string propertyName = t.val; 
			if (la.kind == 38) {
				lexer.NextToken();
				if (StartOf(6)) {
					FormalParameterList(
//  1096 "VBNET.ATG" 
p);
				}
				Expect(39);
			}
			if (la.kind == 64) {
				lexer.NextToken();
				while (la.kind == 41) {
					AttributeSection(
//  1099 "VBNET.ATG" 
out returnTypeAttributeSection);

//  1101 "VBNET.ATG" 
					if (returnTypeAttributeSection != null) {
					returnTypeAttributeSection.AttributeTarget = "return";
					attributes.Add(returnTypeAttributeSection);
					}
					
				}
				if (
//  1108 "VBNET.ATG" 
IsNewExpression()) {
					ObjectCreateExpression(
//  1108 "VBNET.ATG" 
out initializer);

//  1110 "VBNET.ATG" 
					if (initializer is ObjectCreateExpression) {
					type = ((ObjectCreateExpression)initializer).CreateType.Clone();
					} else {
						type = ((ArrayCreateExpression)initializer).CreateType.Clone();
					}
					
				} else if (StartOf(8)) {
					TypeName(
//  1117 "VBNET.ATG" 
out type);
				} else SynErr(261);
			}
			if (la.kind == 21) {
				lexer.NextToken();
				Expr(
//  1120 "VBNET.ATG" 
out initializer);
			}
			if (la.kind == 137) {
				ImplementsClause(
//  1121 "VBNET.ATG" 
out implementsClause);
			}
			EndOfStmt();
			if (
//  1125 "VBNET.ATG" 
IsMustOverride(m) || IsAutomaticProperty()) {

//  1127 "VBNET.ATG" 
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

//  1139 "VBNET.ATG" 
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
//  1149 "VBNET.ATG" 
out getRegion, out setRegion);
				Expect(114);
				Expect(187);
				EndOfStmt();

//  1153 "VBNET.ATG" 
				pDecl.GetRegion = getRegion;
				pDecl.SetRegion = setRegion;
				pDecl.BodyEnd = t.Location; // t = EndOfStmt; not "Property"
				AddChild(pDecl);
				
			} else SynErr(262);
			break;
		}
		case 99: {
			lexer.NextToken();

//  1160 "VBNET.ATG" 
			Location startPos = t.Location; 
			Expect(120);

//  1162 "VBNET.ATG" 
			m.Check(Modifiers.VBCustomEvents);
			EventAddRemoveRegion eventAccessorDeclaration;
			EventAddRegion addHandlerAccessorDeclaration = null;
			EventRemoveRegion removeHandlerAccessorDeclaration = null;
			EventRaiseRegion raiseEventAccessorDeclaration = null;
			List<InterfaceImplementation> implementsClause = null;
			
			Identifier();

//  1169 "VBNET.ATG" 
			string customEventName = t.val; 
			Expect(64);
			TypeName(
//  1170 "VBNET.ATG" 
out type);
			if (la.kind == 137) {
				ImplementsClause(
//  1171 "VBNET.ATG" 
out implementsClause);
			}
			EndOfStmt();
			while (StartOf(18)) {
				EventAccessorDeclaration(
//  1174 "VBNET.ATG" 
out eventAccessorDeclaration);

//  1176 "VBNET.ATG" 
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

//  1192 "VBNET.ATG" 
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
		case 162: case 173: case 233: {

//  1218 "VBNET.ATG" 
			ConversionType opConversionType = ConversionType.None; 
			if (la.kind == 162 || la.kind == 233) {
				if (la.kind == 233) {
					lexer.NextToken();

//  1219 "VBNET.ATG" 
					opConversionType = ConversionType.Implicit; 
				} else {
					lexer.NextToken();

//  1220 "VBNET.ATG" 
					opConversionType = ConversionType.Explicit;
				}
			}
			Expect(173);

//  1223 "VBNET.ATG" 
			m.Check(Modifiers.VBOperators);
			Location startPos = t.Location;
			TypeReference returnType = NullTypeReference.Instance;
			TypeReference operandType = NullTypeReference.Instance;
			string operandName;
			OverloadableOperatorType operatorType;
			AttributeSection section;
			List<ParameterDeclarationExpression> parameters = new List<ParameterDeclarationExpression>();
			
			OverloadableOperator(
//  1232 "VBNET.ATG" 
out operatorType);
			Expect(38);
			if (la.kind == 73) {
				lexer.NextToken();
			}
			Identifier();

//  1233 "VBNET.ATG" 
			operandName = t.val; 
			if (la.kind == 64) {
				lexer.NextToken();
				TypeName(
//  1234 "VBNET.ATG" 
out operandType);
			}

//  1235 "VBNET.ATG" 
			parameters.Add(new ParameterDeclarationExpression(operandType, operandName, ParameterModifiers.In)); 
			while (la.kind == 23) {
				lexer.NextToken();
				if (la.kind == 73) {
					lexer.NextToken();
				}
				Identifier();

//  1239 "VBNET.ATG" 
				operandName = t.val; 
				if (la.kind == 64) {
					lexer.NextToken();
					TypeName(
//  1240 "VBNET.ATG" 
out operandType);
				}

//  1241 "VBNET.ATG" 
				parameters.Add(new ParameterDeclarationExpression(operandType, operandName, ParameterModifiers.In)); 
			}
			Expect(39);

//  1244 "VBNET.ATG" 
			Location endPos = t.EndLocation; 
			if (la.kind == 64) {
				lexer.NextToken();
				while (la.kind == 41) {
					AttributeSection(
//  1245 "VBNET.ATG" 
out section);

//  1246 "VBNET.ATG" 
					if (section != null) {
					section.AttributeTarget = "return";
					attributes.Add(section);
					} 
				}
				TypeName(
//  1250 "VBNET.ATG" 
out returnType);

//  1250 "VBNET.ATG" 
				endPos = t.EndLocation; 
			}
			Expect(1);
			Block(
//  1252 "VBNET.ATG" 
out stmt);
			Expect(114);
			Expect(173);
			EndOfStmt();

//  1254 "VBNET.ATG" 
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
		default: SynErr(263); break;
		}
	}

	void EnumMemberDecl(
//  766 "VBNET.ATG" 
out FieldDeclaration f) {

//  768 "VBNET.ATG" 
		Expression expr = null;List<AttributeSection> attributes = new List<AttributeSection>();
		AttributeSection section = null;
		VariableDeclaration varDecl = null;
		
		while (la.kind == 41) {
			AttributeSection(
//  772 "VBNET.ATG" 
out section);

//  772 "VBNET.ATG" 
			attributes.Add(section); 
		}
		Identifier();

//  775 "VBNET.ATG" 
		f = new FieldDeclaration(attributes);
		varDecl = new VariableDeclaration(t.val);
		f.Fields.Add(varDecl);
		f.StartLocation = varDecl.StartLocation = t.Location;
		
		if (la.kind == 21) {
			lexer.NextToken();
			Expr(
//  780 "VBNET.ATG" 
out expr);

//  780 "VBNET.ATG" 
			varDecl.Initializer = expr; 
		}
		EndOfStmt();
	}

	void InterfaceMemberDecl() {

//  657 "VBNET.ATG" 
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
//  665 "VBNET.ATG" 
out section);

//  665 "VBNET.ATG" 
				attributes.Add(section); 
			}
			while (StartOf(10)) {
				MemberModifier(
//  668 "VBNET.ATG" 
mod);
			}
			if (la.kind == 120) {
				lexer.NextToken();

//  672 "VBNET.ATG" 
				mod.Check(Modifiers.VBInterfaceEvents);
				Location startLocation = t.Location;
				
				Identifier();

//  675 "VBNET.ATG" 
				name = t.val; 
				if (la.kind == 38) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
//  676 "VBNET.ATG" 
p);
					}
					Expect(39);
				}
				if (la.kind == 64) {
					lexer.NextToken();
					TypeName(
//  677 "VBNET.ATG" 
out type);
				}
				EndOfStmt();

//  680 "VBNET.ATG" 
				EventDeclaration ed = new EventDeclaration {
				Name = name, TypeReference = type, Modifier = mod.Modifier,
				Parameters = p, Attributes = attributes,
				StartLocation = startLocation, EndLocation = t.EndLocation
				};
				AddChild(ed);
				
			} else if (la.kind == 211) {
				lexer.NextToken();

//  690 "VBNET.ATG" 
				Location startLocation =  t.Location;
				mod.Check(Modifiers.VBInterfaceMethods);
				
				Identifier();

//  693 "VBNET.ATG" 
				name = t.val; 
				TypeParameterList(
//  694 "VBNET.ATG" 
templates);
				if (la.kind == 38) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
//  695 "VBNET.ATG" 
p);
					}
					Expect(39);
				}
				EndOfStmt();

//  698 "VBNET.ATG" 
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

//  713 "VBNET.ATG" 
				mod.Check(Modifiers.VBInterfaceMethods);
				Location startLocation = t.Location;
				
				Identifier();

//  716 "VBNET.ATG" 
				name = t.val; 
				TypeParameterList(
//  717 "VBNET.ATG" 
templates);
				if (la.kind == 38) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
//  718 "VBNET.ATG" 
p);
					}
					Expect(39);
				}
				if (la.kind == 64) {
					lexer.NextToken();
					while (la.kind == 41) {
						AttributeSection(
//  719 "VBNET.ATG" 
out returnTypeAttributeSection);
					}
					TypeName(
//  719 "VBNET.ATG" 
out type);
				}

//  721 "VBNET.ATG" 
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
			} else if (la.kind == 187) {
				lexer.NextToken();

//  741 "VBNET.ATG" 
				Location startLocation = t.Location;
				mod.Check(Modifiers.VBInterfaceProperties);
				
				Identifier();

//  744 "VBNET.ATG" 
				name = t.val;  
				if (la.kind == 38) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
//  745 "VBNET.ATG" 
p);
					}
					Expect(39);
				}
				if (la.kind == 64) {
					lexer.NextToken();
					TypeName(
//  746 "VBNET.ATG" 
out type);
				}

//  748 "VBNET.ATG" 
				if(type == null) {
				type = new TypeReference("System.Object", true);
				}
				
				EndOfStmt();

//  754 "VBNET.ATG" 
				PropertyDeclaration pd = new PropertyDeclaration(name, type, mod.Modifier, attributes);
				pd.Parameters = p;
				pd.EndLocation = t.EndLocation;
				pd.StartLocation = startLocation;
				AddChild(pd);
				
			} else SynErr(264);
		} else if (StartOf(20)) {
			NonModuleDeclaration(
//  762 "VBNET.ATG" 
mod, attributes);
		} else SynErr(265);
	}

	void Expr(
//  1649 "VBNET.ATG" 
out Expression expr) {

//  1650 "VBNET.ATG" 
		expr = null; 
		if (
//  1651 "VBNET.ATG" 
IsQueryExpression() ) {
			QueryExpr(
//  1652 "VBNET.ATG" 
out expr);
		} else if (la.kind == 128 || la.kind == 211) {
			LambdaExpr(
//  1653 "VBNET.ATG" 
out expr);
		} else if (StartOf(21)) {
			DisjunctionExpr(
//  1654 "VBNET.ATG" 
out expr);
		} else SynErr(266);
	}

	void ImplementsClause(
//  1622 "VBNET.ATG" 
out List<InterfaceImplementation> baseInterfaces) {

//  1624 "VBNET.ATG" 
		baseInterfaces = new List<InterfaceImplementation>();
		TypeReference type = null;
		string memberName = null;
		
		Expect(137);
		NonArrayTypeName(
//  1629 "VBNET.ATG" 
out type, false);

//  1630 "VBNET.ATG" 
		if (type != null) memberName = TypeReference.StripLastIdentifierFromType(ref type); 

//  1631 "VBNET.ATG" 
		baseInterfaces.Add(new InterfaceImplementation(type, memberName)); 
		while (la.kind == 23) {
			lexer.NextToken();
			NonArrayTypeName(
//  1633 "VBNET.ATG" 
out type, false);

//  1634 "VBNET.ATG" 
			if (type != null) memberName = TypeReference.StripLastIdentifierFromType(ref type); 

//  1635 "VBNET.ATG" 
			baseInterfaces.Add(new InterfaceImplementation(type, memberName)); 
		}
	}

	void HandlesClause(
//  1580 "VBNET.ATG" 
out List<string> handlesClause) {

//  1582 "VBNET.ATG" 
		handlesClause = new List<string>();
		string name;
		
		Expect(135);
		EventMemberSpecifier(
//  1585 "VBNET.ATG" 
out name);

//  1585 "VBNET.ATG" 
		if (name != null) handlesClause.Add(name); 
		while (la.kind == 23) {
			lexer.NextToken();
			EventMemberSpecifier(
//  1586 "VBNET.ATG" 
out name);

//  1586 "VBNET.ATG" 
			if (name != null) handlesClause.Add(name); 
		}
	}

	void Block(
//  2811 "VBNET.ATG" 
out Statement stmt) {

//  2814 "VBNET.ATG" 
		BlockStatement blockStmt = new BlockStatement();
		/* in snippet parsing mode, t might be null */
		if (t != null) blockStmt.StartLocation = t.EndLocation;
		BlockStart(blockStmt);
		
		while (StartOf(22) || 
//  2820 "VBNET.ATG" 
IsEndStmtAhead()) {
			if (
//  2820 "VBNET.ATG" 
IsEndStmtAhead()) {
				Expect(114);
				EndOfStmt();

//  2820 "VBNET.ATG" 
				AddChild(new EndStatement()); 
			} else {
				Statement();
				EndOfStmt();
			}
		}

//  2825 "VBNET.ATG" 
		stmt = blockStmt;
		if (t != null) blockStmt.EndLocation = t.EndLocation;
		BlockEnd();
		
	}

	void Charset(
//  1572 "VBNET.ATG" 
out CharsetModifier charsetModifier) {

//  1573 "VBNET.ATG" 
		charsetModifier = CharsetModifier.None; 
		if (la.kind == 128 || la.kind == 211) {
		} else if (la.kind == 63) {
			lexer.NextToken();

//  1574 "VBNET.ATG" 
			charsetModifier = CharsetModifier.Ansi; 
		} else if (la.kind == 67) {
			lexer.NextToken();

//  1575 "VBNET.ATG" 
			charsetModifier = CharsetModifier.Auto; 
		} else if (la.kind == 224) {
			lexer.NextToken();

//  1576 "VBNET.ATG" 
			charsetModifier = CharsetModifier.Unicode; 
		} else SynErr(267);
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
		case 179: {
			lexer.NextToken();
			break;
		}
		case 177: {
			lexer.NextToken();
			break;
		}
		case 185: {
			lexer.NextToken();
			break;
		}
		case 204: {
			lexer.NextToken();
			break;
		}
		case 213: {
			lexer.NextToken();
			break;
		}
		case 214: {
			lexer.NextToken();
			break;
		}
		case 224: {
			lexer.NextToken();
			break;
		}
		case 225: {
			lexer.NextToken();
			break;
		}
		case 231: {
			lexer.NextToken();
			break;
		}
		default: SynErr(268); break;
		}
	}

	void VariableDeclaratorPartAfterIdentifier(
//  1457 "VBNET.ATG" 
List<VariableDeclaration> fieldDeclaration, string name) {

//  1459 "VBNET.ATG" 
		Expression expr = null;
		TypeReference type = null;
		ArrayList rank = null;
		List<Expression> dimension = null;
		Location startLocation = t.Location;
		
		if (
//  1465 "VBNET.ATG" 
IsSize() && !IsDims()) {
			ArrayInitializationModifier(
//  1465 "VBNET.ATG" 
out dimension);
		}
		if (
//  1466 "VBNET.ATG" 
IsDims()) {
			ArrayNameModifier(
//  1466 "VBNET.ATG" 
out rank);
		}
		if (
//  1468 "VBNET.ATG" 
IsObjectCreation()) {
			Expect(64);
			ObjectCreateExpression(
//  1468 "VBNET.ATG" 
out expr);

//  1470 "VBNET.ATG" 
			if (expr is ObjectCreateExpression) {
			type = ((ObjectCreateExpression)expr).CreateType.Clone();
			} else {
				type = ((ArrayCreateExpression)expr).CreateType.Clone();
			}
			
		} else if (StartOf(23)) {
			if (la.kind == 64) {
				lexer.NextToken();
				TypeName(
//  1477 "VBNET.ATG" 
out type);

//  1479 "VBNET.ATG" 
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

//  1491 "VBNET.ATG" 
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
//  1514 "VBNET.ATG" 
out expr);
			}
		} else SynErr(269);

//  1517 "VBNET.ATG" 
		VariableDeclaration varDecl = new VariableDeclaration(name, expr, type);
		varDecl.StartLocation = startLocation;
		varDecl.EndLocation = t.Location;
		fieldDeclaration.Add(varDecl);
		
	}

	void VariableDeclarator(
//  1451 "VBNET.ATG" 
List<VariableDeclaration> fieldDeclaration) {
		Identifier();

//  1453 "VBNET.ATG" 
		string name = t.val; 
		VariableDeclaratorPartAfterIdentifier(
//  1454 "VBNET.ATG" 
fieldDeclaration, name);
	}

	void ConstantDeclarator(
//  1432 "VBNET.ATG" 
List<VariableDeclaration> constantDeclaration) {

//  1434 "VBNET.ATG" 
		Expression expr = null;
		TypeReference type = null;
		string name = String.Empty;
		Location location;
		
		Identifier();

//  1439 "VBNET.ATG" 
		name = t.val; location = t.Location; 
		if (la.kind == 64) {
			lexer.NextToken();
			TypeName(
//  1440 "VBNET.ATG" 
out type);
		}
		Expect(21);
		Expr(
//  1441 "VBNET.ATG" 
out expr);

//  1443 "VBNET.ATG" 
		VariableDeclaration f = new VariableDeclaration(name, expr);
		f.TypeReference = type;
		f.StartLocation = location;
		constantDeclaration.Add(f);
		
	}

	void ObjectCreateExpression(
//  1979 "VBNET.ATG" 
out Expression oce) {

//  1981 "VBNET.ATG" 
		TypeReference type = null;
		CollectionInitializerExpression initializer = null;
		List<Expression> arguments = null;
		ArrayList dimensions = null;
		oce = null;
		bool canBeNormal; bool canBeReDim;
		
		Expect(163);
		if (StartOf(8)) {
			NonArrayTypeName(
//  1989 "VBNET.ATG" 
out type, false);
			if (la.kind == 38) {
				lexer.NextToken();
				NormalOrReDimArgumentList(
//  1990 "VBNET.ATG" 
out arguments, out canBeNormal, out canBeReDim);
				Expect(39);
				if (la.kind == 36 || 
//  1991 "VBNET.ATG" 
la.kind == Tokens.OpenParenthesis) {
					if (
//  1991 "VBNET.ATG" 
la.kind == Tokens.OpenParenthesis) {
						ArrayTypeModifiers(
//  1992 "VBNET.ATG" 
out dimensions);
						CollectionInitializer(
//  1993 "VBNET.ATG" 
out initializer);
					} else {
						CollectionInitializer(
//  1994 "VBNET.ATG" 
out initializer);
					}
				}

//  1996 "VBNET.ATG" 
				if (canBeReDim && !canBeNormal && initializer == null) initializer = new CollectionInitializerExpression(); 
			}
		}

//  2000 "VBNET.ATG" 
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
		
		if (la.kind == 127 || la.kind == 234) {
			if (la.kind == 234) {

//  2015 "VBNET.ATG" 
				MemberInitializerExpression memberInitializer = null;
				
				lexer.NextToken();

//  2019 "VBNET.ATG" 
				CollectionInitializerExpression memberInitializers = new CollectionInitializerExpression();
				memberInitializers.StartLocation = la.Location;
				
				Expect(36);
				MemberInitializer(
//  2023 "VBNET.ATG" 
out memberInitializer);

//  2024 "VBNET.ATG" 
				memberInitializers.CreateExpressions.Add(memberInitializer); 
				while (la.kind == 23) {
					lexer.NextToken();
					MemberInitializer(
//  2026 "VBNET.ATG" 
out memberInitializer);

//  2027 "VBNET.ATG" 
					memberInitializers.CreateExpressions.Add(memberInitializer); 
				}
				Expect(37);

//  2031 "VBNET.ATG" 
				memberInitializers.EndLocation = t.Location;
				if(oce is ObjectCreateExpression)
				{
					((ObjectCreateExpression)oce).ObjectInitializer = memberInitializers;
				}
				
			} else {
				lexer.NextToken();
				CollectionInitializer(
//  2041 "VBNET.ATG" 
out initializer);

//  2043 "VBNET.ATG" 
				if(oce is ObjectCreateExpression)
				((ObjectCreateExpression)oce).ObjectInitializer = initializer;
				
			}
		}
	}

	void AccessorDecls(
//  1366 "VBNET.ATG" 
out PropertyGetRegion getBlock, out PropertySetRegion setBlock) {

//  1368 "VBNET.ATG" 
		List<AttributeSection> attributes = new List<AttributeSection>();
		AttributeSection section;
		getBlock = null;
		setBlock = null; 
		
		while (la.kind == 41) {
			AttributeSection(
//  1373 "VBNET.ATG" 
out section);

//  1373 "VBNET.ATG" 
			attributes.Add(section); 
		}
		if (StartOf(24)) {
			GetAccessorDecl(
//  1375 "VBNET.ATG" 
out getBlock, attributes);
			if (StartOf(25)) {

//  1377 "VBNET.ATG" 
				attributes = new List<AttributeSection>(); 
				while (la.kind == 41) {
					AttributeSection(
//  1378 "VBNET.ATG" 
out section);

//  1378 "VBNET.ATG" 
					attributes.Add(section); 
				}
				SetAccessorDecl(
//  1379 "VBNET.ATG" 
out setBlock, attributes);
			}
		} else if (StartOf(26)) {
			SetAccessorDecl(
//  1382 "VBNET.ATG" 
out setBlock, attributes);
			if (StartOf(27)) {

//  1384 "VBNET.ATG" 
				attributes = new List<AttributeSection>(); 
				while (la.kind == 41) {
					AttributeSection(
//  1385 "VBNET.ATG" 
out section);

//  1385 "VBNET.ATG" 
					attributes.Add(section); 
				}
				GetAccessorDecl(
//  1386 "VBNET.ATG" 
out getBlock, attributes);
			}
		} else SynErr(270);
	}

	void EventAccessorDeclaration(
//  1329 "VBNET.ATG" 
out EventAddRemoveRegion eventAccessorDeclaration) {

//  1331 "VBNET.ATG" 
		Statement stmt = null;
		List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		eventAccessorDeclaration = null;
		
		while (la.kind == 41) {
			AttributeSection(
//  1337 "VBNET.ATG" 
out section);

//  1337 "VBNET.ATG" 
			attributes.Add(section); 
		}
		if (la.kind == 57) {
			lexer.NextToken();
			if (la.kind == 38) {
				lexer.NextToken();
				if (StartOf(6)) {
					FormalParameterList(
//  1339 "VBNET.ATG" 
p);
				}
				Expect(39);
			}
			Expect(1);
			Block(
//  1340 "VBNET.ATG" 
out stmt);
			Expect(114);
			Expect(57);
			EndOfStmt();

//  1342 "VBNET.ATG" 
			eventAccessorDeclaration = new EventAddRegion(attributes);
			eventAccessorDeclaration.Block = (BlockStatement)stmt;
			eventAccessorDeclaration.Parameters = p;
			
		} else if (la.kind == 194) {
			lexer.NextToken();
			if (la.kind == 38) {
				lexer.NextToken();
				if (StartOf(6)) {
					FormalParameterList(
//  1347 "VBNET.ATG" 
p);
				}
				Expect(39);
			}
			Expect(1);
			Block(
//  1348 "VBNET.ATG" 
out stmt);
			Expect(114);
			Expect(194);
			EndOfStmt();

//  1350 "VBNET.ATG" 
			eventAccessorDeclaration = new EventRemoveRegion(attributes);
			eventAccessorDeclaration.Block = (BlockStatement)stmt;
			eventAccessorDeclaration.Parameters = p;
			
		} else if (la.kind == 190) {
			lexer.NextToken();
			if (la.kind == 38) {
				lexer.NextToken();
				if (StartOf(6)) {
					FormalParameterList(
//  1355 "VBNET.ATG" 
p);
				}
				Expect(39);
			}
			Expect(1);
			Block(
//  1356 "VBNET.ATG" 
out stmt);
			Expect(114);
			Expect(190);
			EndOfStmt();

//  1358 "VBNET.ATG" 
			eventAccessorDeclaration = new EventRaiseRegion(attributes);
			eventAccessorDeclaration.Block = (BlockStatement)stmt;
			eventAccessorDeclaration.Parameters = p;
			
		} else SynErr(271);
	}

	void OverloadableOperator(
//  1271 "VBNET.ATG" 
out OverloadableOperatorType operatorType) {

//  1272 "VBNET.ATG" 
		operatorType = OverloadableOperatorType.None; 
		switch (la.kind) {
		case 32: {
			lexer.NextToken();

//  1274 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Add; 
			break;
		}
		case 31: {
			lexer.NextToken();

//  1276 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Subtract; 
			break;
		}
		case 35: {
			lexer.NextToken();

//  1278 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Multiply; 
			break;
		}
		case 25: {
			lexer.NextToken();

//  1280 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Divide; 
			break;
		}
		case 26: {
			lexer.NextToken();

//  1282 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.DivideInteger; 
			break;
		}
		case 24: {
			lexer.NextToken();

//  1284 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Concat; 
			break;
		}
		case 151: {
			lexer.NextToken();

//  1286 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Like; 
			break;
		}
		case 155: {
			lexer.NextToken();

//  1288 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Modulus; 
			break;
		}
		case 61: {
			lexer.NextToken();

//  1290 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.BitwiseAnd; 
			break;
		}
		case 176: {
			lexer.NextToken();

//  1292 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.BitwiseOr; 
			break;
		}
		case 237: {
			lexer.NextToken();

//  1294 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.ExclusiveOr; 
			break;
		}
		case 33: {
			lexer.NextToken();

//  1296 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Power; 
			break;
		}
		case 45: {
			lexer.NextToken();

//  1298 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.ShiftLeft; 
			break;
		}
		case 46: {
			lexer.NextToken();

//  1300 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.ShiftRight; 
			break;
		}
		case 21: {
			lexer.NextToken();

//  1302 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Equality; 
			break;
		}
		case 42: {
			lexer.NextToken();

//  1304 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.InEquality; 
			break;
		}
		case 41: {
			lexer.NextToken();

//  1306 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.LessThan; 
			break;
		}
		case 44: {
			lexer.NextToken();

//  1308 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.LessThanOrEqual; 
			break;
		}
		case 40: {
			lexer.NextToken();

//  1310 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.GreaterThan; 
			break;
		}
		case 43: {
			lexer.NextToken();

//  1312 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.GreaterThanOrEqual; 
			break;
		}
		case 95: {
			lexer.NextToken();

//  1314 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.CType; 
			break;
		}
		case 2: case 59: case 63: case 65: case 66: case 67: case 68: case 71: case 88: case 99: case 105: case 108: case 117: case 122: case 127: case 134: case 140: case 144: case 147: case 148: case 171: case 177: case 179: case 185: case 204: case 213: case 214: case 224: case 225: case 231: {
			Identifier();

//  1318 "VBNET.ATG" 
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
		default: SynErr(272); break;
		}
	}

	void GetAccessorDecl(
//  1392 "VBNET.ATG" 
out PropertyGetRegion getBlock, List<AttributeSection> attributes) {

//  1393 "VBNET.ATG" 
		Statement stmt = null; Modifiers m; 
		PropertyAccessorAccessModifier(
//  1395 "VBNET.ATG" 
out m);
		Expect(129);

//  1397 "VBNET.ATG" 
		Location startLocation = t.Location; 
		Expect(1);
		Block(
//  1399 "VBNET.ATG" 
out stmt);

//  1400 "VBNET.ATG" 
		getBlock = new PropertyGetRegion((BlockStatement)stmt, attributes); 
		Expect(114);
		Expect(129);

//  1402 "VBNET.ATG" 
		getBlock.Modifier = m; 

//  1403 "VBNET.ATG" 
		getBlock.StartLocation = startLocation; getBlock.EndLocation = t.EndLocation; 
		EndOfStmt();
	}

	void SetAccessorDecl(
//  1408 "VBNET.ATG" 
out PropertySetRegion setBlock, List<AttributeSection> attributes) {

//  1410 "VBNET.ATG" 
		Statement stmt = null;
		List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
		Modifiers m;
		
		PropertyAccessorAccessModifier(
//  1415 "VBNET.ATG" 
out m);
		Expect(199);

//  1417 "VBNET.ATG" 
		Location startLocation = t.Location; 
		if (la.kind == 38) {
			lexer.NextToken();
			if (StartOf(6)) {
				FormalParameterList(
//  1418 "VBNET.ATG" 
p);
			}
			Expect(39);
		}
		Expect(1);
		Block(
//  1420 "VBNET.ATG" 
out stmt);

//  1422 "VBNET.ATG" 
		setBlock = new PropertySetRegion((BlockStatement)stmt, attributes);
		setBlock.Modifier = m;
		setBlock.Parameters = p;
		
		Expect(114);
		Expect(199);

//  1427 "VBNET.ATG" 
		setBlock.StartLocation = startLocation; setBlock.EndLocation = t.EndLocation; 
		EndOfStmt();
	}

	void PropertyAccessorAccessModifier(
//  3529 "VBNET.ATG" 
out Modifiers m) {

//  3530 "VBNET.ATG" 
		m = Modifiers.None; 
		while (StartOf(28)) {
			if (la.kind == 189) {
				lexer.NextToken();

//  3532 "VBNET.ATG" 
				m |= Modifiers.Public; 
			} else if (la.kind == 188) {
				lexer.NextToken();

//  3533 "VBNET.ATG" 
				m |= Modifiers.Protected; 
			} else if (la.kind == 126) {
				lexer.NextToken();

//  3534 "VBNET.ATG" 
				m |= Modifiers.Internal; 
			} else {
				lexer.NextToken();

//  3535 "VBNET.ATG" 
				m |= Modifiers.Private; 
			}
		}
	}

	void ArrayInitializationModifier(
//  1525 "VBNET.ATG" 
out List<Expression> arrayModifiers) {

//  1527 "VBNET.ATG" 
		arrayModifiers = null;
		
		Expect(38);
		InitializationRankList(
//  1529 "VBNET.ATG" 
out arrayModifiers);
		Expect(39);
	}

	void ArrayNameModifier(
//  2606 "VBNET.ATG" 
out ArrayList arrayModifiers) {

//  2608 "VBNET.ATG" 
		arrayModifiers = null;
		
		ArrayTypeModifiers(
//  2610 "VBNET.ATG" 
out arrayModifiers);
	}

	void InitializationRankList(
//  1533 "VBNET.ATG" 
out List<Expression> rank) {

//  1535 "VBNET.ATG" 
		rank = new List<Expression>();
		Expression expr = null;
		
		Expr(
//  1538 "VBNET.ATG" 
out expr);
		if (la.kind == 217) {
			lexer.NextToken();

//  1539 "VBNET.ATG" 
			EnsureIsZero(expr); 
			Expr(
//  1540 "VBNET.ATG" 
out expr);
		}

//  1542 "VBNET.ATG" 
		if (expr != null) { rank.Add(expr); } 
		while (la.kind == 23) {
			lexer.NextToken();
			Expr(
//  1544 "VBNET.ATG" 
out expr);
			if (la.kind == 217) {
				lexer.NextToken();

//  1545 "VBNET.ATG" 
				EnsureIsZero(expr); 
				Expr(
//  1546 "VBNET.ATG" 
out expr);
			}

//  1548 "VBNET.ATG" 
			if (expr != null) { rank.Add(expr); } 
		}
	}

	void CollectionInitializer(
//  1553 "VBNET.ATG" 
out CollectionInitializerExpression outExpr) {

//  1555 "VBNET.ATG" 
		Expression expr = null;
		CollectionInitializerExpression initializer = new CollectionInitializerExpression();
		
		Expect(36);
		if (StartOf(29)) {
			Expr(
//  1560 "VBNET.ATG" 
out expr);

//  1562 "VBNET.ATG" 
			if (expr != null) { initializer.CreateExpressions.Add(expr); }
			
			while (
//  1565 "VBNET.ATG" 
NotFinalComma()) {
				Expect(23);
				Expr(
//  1565 "VBNET.ATG" 
out expr);

//  1566 "VBNET.ATG" 
				if (expr != null) { initializer.CreateExpressions.Add(expr); } 
			}
		}
		Expect(37);

//  1569 "VBNET.ATG" 
		outExpr = initializer; 
	}

	void EventMemberSpecifier(
//  1639 "VBNET.ATG" 
out string name) {

//  1640 "VBNET.ATG" 
		string eventName; 
		if (StartOf(4)) {
			Identifier();
		} else if (la.kind == 159) {
			lexer.NextToken();
		} else if (la.kind == 154) {
			lexer.NextToken();
		} else SynErr(273);

//  1643 "VBNET.ATG" 
		name = t.val; 
		Expect(27);
		IdentifierOrKeyword(
//  1645 "VBNET.ATG" 
out eventName);

//  1646 "VBNET.ATG" 
		name = name + "." + eventName; 
	}

	void IdentifierOrKeyword(
//  3462 "VBNET.ATG" 
out string name) {
		lexer.NextToken();

//  3464 "VBNET.ATG" 
		name = t.val;  
	}

	void QueryExpr(
//  2130 "VBNET.ATG" 
out Expression expr) {

//  2132 "VBNET.ATG" 
		QueryExpressionVB qexpr = new QueryExpressionVB();
		qexpr.StartLocation = la.Location;
		expr = qexpr;
		
		FromOrAggregateQueryOperator(
//  2136 "VBNET.ATG" 
qexpr.Clauses);
		while (StartOf(30)) {
			QueryOperator(
//  2137 "VBNET.ATG" 
qexpr.Clauses);
		}

//  2139 "VBNET.ATG" 
		qexpr.EndLocation = t.EndLocation;
		
	}

	void LambdaExpr(
//  2050 "VBNET.ATG" 
out Expression expr) {

//  2052 "VBNET.ATG" 
		LambdaExpression lambda = null;
		
		if (la.kind == 211) {
			SubLambdaExpression(
//  2054 "VBNET.ATG" 
out lambda);
		} else if (la.kind == 128) {
			FunctionLambdaExpression(
//  2055 "VBNET.ATG" 
out lambda);
		} else SynErr(274);

//  2056 "VBNET.ATG" 
		expr = lambda; 
	}

	void DisjunctionExpr(
//  1823 "VBNET.ATG" 
out Expression outExpr) {

//  1825 "VBNET.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		ConjunctionExpr(
//  1828 "VBNET.ATG" 
out outExpr);
		while (la.kind == 176 || la.kind == 178 || la.kind == 237) {
			if (la.kind == 176) {
				lexer.NextToken();

//  1831 "VBNET.ATG" 
				op = BinaryOperatorType.BitwiseOr; 
			} else if (la.kind == 178) {
				lexer.NextToken();

//  1832 "VBNET.ATG" 
				op = BinaryOperatorType.LogicalOr; 
			} else {
				lexer.NextToken();

//  1833 "VBNET.ATG" 
				op = BinaryOperatorType.ExclusiveOr; 
			}
			ConjunctionExpr(
//  1835 "VBNET.ATG" 
out expr);

//  1835 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr);  
		}
	}

	void AssignmentOperator(
//  1657 "VBNET.ATG" 
out AssignmentOperatorType op) {

//  1658 "VBNET.ATG" 
		op = AssignmentOperatorType.None; 
		switch (la.kind) {
		case 21: {
			lexer.NextToken();

//  1659 "VBNET.ATG" 
			op = AssignmentOperatorType.Assign; 
			break;
		}
		case 55: {
			lexer.NextToken();

//  1660 "VBNET.ATG" 
			op = AssignmentOperatorType.ConcatString; 
			break;
		}
		case 47: {
			lexer.NextToken();

//  1661 "VBNET.ATG" 
			op = AssignmentOperatorType.Add; 
			break;
		}
		case 49: {
			lexer.NextToken();

//  1662 "VBNET.ATG" 
			op = AssignmentOperatorType.Subtract; 
			break;
		}
		case 50: {
			lexer.NextToken();

//  1663 "VBNET.ATG" 
			op = AssignmentOperatorType.Multiply; 
			break;
		}
		case 51: {
			lexer.NextToken();

//  1664 "VBNET.ATG" 
			op = AssignmentOperatorType.Divide; 
			break;
		}
		case 52: {
			lexer.NextToken();

//  1665 "VBNET.ATG" 
			op = AssignmentOperatorType.DivideInteger; 
			break;
		}
		case 48: {
			lexer.NextToken();

//  1666 "VBNET.ATG" 
			op = AssignmentOperatorType.Power; 
			break;
		}
		case 53: {
			lexer.NextToken();

//  1667 "VBNET.ATG" 
			op = AssignmentOperatorType.ShiftLeft; 
			break;
		}
		case 54: {
			lexer.NextToken();

//  1668 "VBNET.ATG" 
			op = AssignmentOperatorType.ShiftRight; 
			break;
		}
		default: SynErr(275); break;
		}
	}

	void SimpleExpr(
//  1672 "VBNET.ATG" 
out Expression pexpr) {

//  1673 "VBNET.ATG" 
		string name; 
		SimpleNonInvocationExpression(
//  1675 "VBNET.ATG" 
out pexpr);
		while (la.kind == 27 || la.kind == 30 || la.kind == 38) {
			if (la.kind == 27) {
				lexer.NextToken();
				IdentifierOrKeyword(
//  1677 "VBNET.ATG" 
out name);

//  1678 "VBNET.ATG" 
				pexpr = new MemberReferenceExpression(pexpr, name); 
				if (
//  1679 "VBNET.ATG" 
la.kind == Tokens.OpenParenthesis && Peek(1).kind == Tokens.Of) {
					lexer.NextToken();
					Expect(170);
					TypeArgumentList(
//  1680 "VBNET.ATG" 
((MemberReferenceExpression)pexpr).TypeArguments);
					Expect(39);
				}
			} else if (la.kind == 30) {
				lexer.NextToken();
				IdentifierOrKeyword(
//  1682 "VBNET.ATG" 
out name);

//  1682 "VBNET.ATG" 
				pexpr = new BinaryOperatorExpression(pexpr, BinaryOperatorType.DictionaryAccess, new PrimitiveExpression(name, name)); 
			} else {
				InvocationExpression(
//  1683 "VBNET.ATG" 
ref pexpr);
			}
		}
	}

	void SimpleNonInvocationExpression(
//  1687 "VBNET.ATG" 
out Expression pexpr) {

//  1689 "VBNET.ATG" 
		Expression expr;
		CollectionInitializerExpression cie;
		TypeReference type = null;
		string name = String.Empty;
		pexpr = null;
		
		if (StartOf(31)) {
			switch (la.kind) {
			case 3: {
				lexer.NextToken();

//  1698 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(t.literalValue, t.val) { LiteralFormat = t.literalFormat };  
				break;
			}
			case 4: {
				lexer.NextToken();

//  1699 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(t.literalValue, t.val) { LiteralFormat = t.literalFormat };  
				break;
			}
			case 7: {
				lexer.NextToken();

//  1700 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(t.literalValue, t.val) { LiteralFormat = t.literalFormat };  
				break;
			}
			case 6: {
				lexer.NextToken();

//  1701 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(t.literalValue, t.val) { LiteralFormat = t.literalFormat };  
				break;
			}
			case 5: {
				lexer.NextToken();

//  1702 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(t.literalValue, t.val) { LiteralFormat = t.literalFormat };  
				break;
			}
			case 9: {
				lexer.NextToken();

//  1703 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(t.literalValue, t.val) { LiteralFormat = t.literalFormat };  
				break;
			}
			case 8: {
				lexer.NextToken();

//  1704 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(t.literalValue, t.val) { LiteralFormat = t.literalFormat };  
				break;
			}
			case 218: {
				lexer.NextToken();

//  1706 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(true, "true");  
				break;
			}
			case 123: {
				lexer.NextToken();

//  1707 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(false, "false"); 
				break;
			}
			case 166: {
				lexer.NextToken();

//  1708 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(null, "null");  
				break;
			}
			case 38: {
				lexer.NextToken();
				Expr(
//  1709 "VBNET.ATG" 
out expr);
				Expect(39);

//  1709 "VBNET.ATG" 
				pexpr = new ParenthesizedExpression(expr); 
				break;
			}
			case 2: case 59: case 63: case 65: case 66: case 67: case 68: case 71: case 88: case 99: case 105: case 108: case 117: case 122: case 127: case 134: case 140: case 144: case 147: case 148: case 171: case 177: case 179: case 185: case 204: case 213: case 214: case 224: case 225: case 231: {
				Identifier();

//  1711 "VBNET.ATG" 
				pexpr = new IdentifierExpression(t.val);
				pexpr.StartLocation = t.Location; pexpr.EndLocation = t.EndLocation;
				
				if (
//  1714 "VBNET.ATG" 
la.kind == Tokens.OpenParenthesis && Peek(1).kind == Tokens.Of) {
					lexer.NextToken();
					Expect(170);
					TypeArgumentList(
//  1715 "VBNET.ATG" 
((IdentifierExpression)pexpr).TypeArguments);
					Expect(39);
				}
				break;
			}
			case 69: case 72: case 83: case 100: case 101: case 110: case 142: case 152: case 169: case 197: case 202: case 203: case 209: case 222: case 223: case 226: {

//  1717 "VBNET.ATG" 
				string val = String.Empty; 
				if (StartOf(12)) {
					PrimitiveTypeName(
//  1718 "VBNET.ATG" 
out val);
				} else if (la.kind == 169) {
					lexer.NextToken();

//  1718 "VBNET.ATG" 
					val = "System.Object"; 
				} else SynErr(276);

//  1719 "VBNET.ATG" 
				pexpr = new TypeReferenceExpression(new TypeReference(val, true)); 
				break;
			}
			case 154: {
				lexer.NextToken();

//  1720 "VBNET.ATG" 
				pexpr = new ThisReferenceExpression(); 
				break;
			}
			case 159: case 160: {

//  1721 "VBNET.ATG" 
				Expression retExpr = null; 
				if (la.kind == 159) {
					lexer.NextToken();

//  1722 "VBNET.ATG" 
					retExpr = new BaseReferenceExpression(); 
				} else if (la.kind == 160) {
					lexer.NextToken();

//  1723 "VBNET.ATG" 
					retExpr = new ClassReferenceExpression(); 
				} else SynErr(277);
				Expect(27);
				IdentifierOrKeyword(
//  1725 "VBNET.ATG" 
out name);

//  1725 "VBNET.ATG" 
				pexpr = new MemberReferenceExpression(retExpr, name); 
				break;
			}
			case 131: {
				lexer.NextToken();
				Expect(27);
				Identifier();

//  1727 "VBNET.ATG" 
				type = new TypeReference(t.val ?? ""); 

//  1729 "VBNET.ATG" 
				type.IsGlobal = true; 

//  1730 "VBNET.ATG" 
				pexpr = new TypeReferenceExpression(type); 
				break;
			}
			case 163: {
				ObjectCreateExpression(
//  1731 "VBNET.ATG" 
out expr);

//  1731 "VBNET.ATG" 
				pexpr = expr; 
				break;
			}
			case 36: {
				CollectionInitializer(
//  1732 "VBNET.ATG" 
out cie);

//  1732 "VBNET.ATG" 
				pexpr = cie; 
				break;
			}
			case 95: case 107: case 220: {

//  1734 "VBNET.ATG" 
				CastType castType = CastType.Cast; 
				if (la.kind == 107) {
					lexer.NextToken();
				} else if (la.kind == 95) {
					lexer.NextToken();

//  1736 "VBNET.ATG" 
					castType = CastType.Conversion; 
				} else if (la.kind == 220) {
					lexer.NextToken();

//  1737 "VBNET.ATG" 
					castType = CastType.TryCast; 
				} else SynErr(278);
				Expect(38);
				Expr(
//  1739 "VBNET.ATG" 
out expr);
				Expect(23);
				TypeName(
//  1739 "VBNET.ATG" 
out type);
				Expect(39);

//  1740 "VBNET.ATG" 
				pexpr = new CastExpression(type, expr, castType); 
				break;
			}
			case 77: case 78: case 79: case 80: case 81: case 82: case 84: case 86: case 87: case 91: case 92: case 93: case 94: case 96: case 97: case 98: {
				CastTarget(
//  1741 "VBNET.ATG" 
out type);
				Expect(38);
				Expr(
//  1741 "VBNET.ATG" 
out expr);
				Expect(39);

//  1741 "VBNET.ATG" 
				pexpr = new CastExpression(type, expr, CastType.PrimitiveConversion); 
				break;
			}
			case 58: {
				lexer.NextToken();
				Expr(
//  1742 "VBNET.ATG" 
out expr);

//  1742 "VBNET.ATG" 
				pexpr = new AddressOfExpression(expr); 
				break;
			}
			case 130: {
				lexer.NextToken();
				Expect(38);
				GetTypeTypeName(
//  1743 "VBNET.ATG" 
out type);
				Expect(39);

//  1743 "VBNET.ATG" 
				pexpr = new TypeOfExpression(type); 
				break;
			}
			case 221: {
				lexer.NextToken();
				SimpleExpr(
//  1744 "VBNET.ATG" 
out expr);
				Expect(145);
				TypeName(
//  1744 "VBNET.ATG" 
out type);

//  1744 "VBNET.ATG" 
				pexpr = new TypeOfIsExpression(expr, type); 
				break;
			}
			case 136: {
				ConditionalExpression(
//  1745 "VBNET.ATG" 
out pexpr);
				break;
			}
			}
		} else if (la.kind == 27) {
			lexer.NextToken();
			IdentifierOrKeyword(
//  1749 "VBNET.ATG" 
out name);

//  1749 "VBNET.ATG" 
			pexpr = new MemberReferenceExpression(null, name);
		} else SynErr(279);
	}

	void TypeArgumentList(
//  2642 "VBNET.ATG" 
List<TypeReference> typeArguments) {

//  2644 "VBNET.ATG" 
		TypeReference typeref;
		
		TypeName(
//  2646 "VBNET.ATG" 
out typeref);

//  2646 "VBNET.ATG" 
		if (typeref != null) typeArguments.Add(typeref); 
		while (la.kind == 23) {
			lexer.NextToken();
			TypeName(
//  2649 "VBNET.ATG" 
out typeref);

//  2649 "VBNET.ATG" 
			if (typeref != null) typeArguments.Add(typeref); 
		}
	}

	void InvocationExpression(
//  1787 "VBNET.ATG" 
ref Expression pexpr) {

//  1788 "VBNET.ATG" 
		List<Expression> parameters = null; 
		Expect(38);

//  1790 "VBNET.ATG" 
		Location start = t.Location; 
		ArgumentList(
//  1791 "VBNET.ATG" 
out parameters);
		Expect(39);

//  1794 "VBNET.ATG" 
		pexpr = new InvocationExpression(pexpr, parameters);
		

//  1796 "VBNET.ATG" 
		pexpr.StartLocation = start; pexpr.EndLocation = t.Location; 
	}

	void PrimitiveTypeName(
//  3469 "VBNET.ATG" 
out string type) {

//  3470 "VBNET.ATG" 
		type = String.Empty; 
		switch (la.kind) {
		case 69: {
			lexer.NextToken();

//  3471 "VBNET.ATG" 
			type = "System.Boolean"; 
			break;
		}
		case 100: {
			lexer.NextToken();

//  3472 "VBNET.ATG" 
			type = "System.DateTime"; 
			break;
		}
		case 83: {
			lexer.NextToken();

//  3473 "VBNET.ATG" 
			type = "System.Char"; 
			break;
		}
		case 209: {
			lexer.NextToken();

//  3474 "VBNET.ATG" 
			type = "System.String"; 
			break;
		}
		case 101: {
			lexer.NextToken();

//  3475 "VBNET.ATG" 
			type = "System.Decimal"; 
			break;
		}
		case 72: {
			lexer.NextToken();

//  3476 "VBNET.ATG" 
			type = "System.Byte"; 
			break;
		}
		case 202: {
			lexer.NextToken();

//  3477 "VBNET.ATG" 
			type = "System.Int16"; 
			break;
		}
		case 142: {
			lexer.NextToken();

//  3478 "VBNET.ATG" 
			type = "System.Int32"; 
			break;
		}
		case 152: {
			lexer.NextToken();

//  3479 "VBNET.ATG" 
			type = "System.Int64"; 
			break;
		}
		case 203: {
			lexer.NextToken();

//  3480 "VBNET.ATG" 
			type = "System.Single"; 
			break;
		}
		case 110: {
			lexer.NextToken();

//  3481 "VBNET.ATG" 
			type = "System.Double"; 
			break;
		}
		case 222: {
			lexer.NextToken();

//  3482 "VBNET.ATG" 
			type = "System.UInt32"; 
			break;
		}
		case 223: {
			lexer.NextToken();

//  3483 "VBNET.ATG" 
			type = "System.UInt64"; 
			break;
		}
		case 226: {
			lexer.NextToken();

//  3484 "VBNET.ATG" 
			type = "System.UInt16"; 
			break;
		}
		case 197: {
			lexer.NextToken();

//  3485 "VBNET.ATG" 
			type = "System.SByte"; 
			break;
		}
		default: SynErr(280); break;
		}
	}

	void CastTarget(
//  1801 "VBNET.ATG" 
out TypeReference type) {

//  1803 "VBNET.ATG" 
		type = null;
		
		switch (la.kind) {
		case 77: {
			lexer.NextToken();

//  1805 "VBNET.ATG" 
			type = new TypeReference("System.Boolean", true); 
			break;
		}
		case 78: {
			lexer.NextToken();

//  1806 "VBNET.ATG" 
			type = new TypeReference("System.Byte", true); 
			break;
		}
		case 91: {
			lexer.NextToken();

//  1807 "VBNET.ATG" 
			type = new TypeReference("System.SByte", true); 
			break;
		}
		case 79: {
			lexer.NextToken();

//  1808 "VBNET.ATG" 
			type = new TypeReference("System.Char", true); 
			break;
		}
		case 80: {
			lexer.NextToken();

//  1809 "VBNET.ATG" 
			type = new TypeReference("System.DateTime", true); 
			break;
		}
		case 82: {
			lexer.NextToken();

//  1810 "VBNET.ATG" 
			type = new TypeReference("System.Decimal", true); 
			break;
		}
		case 81: {
			lexer.NextToken();

//  1811 "VBNET.ATG" 
			type = new TypeReference("System.Double", true); 
			break;
		}
		case 92: {
			lexer.NextToken();

//  1812 "VBNET.ATG" 
			type = new TypeReference("System.Int16", true); 
			break;
		}
		case 84: {
			lexer.NextToken();

//  1813 "VBNET.ATG" 
			type = new TypeReference("System.Int32", true); 
			break;
		}
		case 86: {
			lexer.NextToken();

//  1814 "VBNET.ATG" 
			type = new TypeReference("System.Int64", true); 
			break;
		}
		case 98: {
			lexer.NextToken();

//  1815 "VBNET.ATG" 
			type = new TypeReference("System.UInt16", true); 
			break;
		}
		case 96: {
			lexer.NextToken();

//  1816 "VBNET.ATG" 
			type = new TypeReference("System.UInt32", true); 
			break;
		}
		case 97: {
			lexer.NextToken();

//  1817 "VBNET.ATG" 
			type = new TypeReference("System.UInt64", true); 
			break;
		}
		case 87: {
			lexer.NextToken();

//  1818 "VBNET.ATG" 
			type = new TypeReference("System.Object", true); 
			break;
		}
		case 93: {
			lexer.NextToken();

//  1819 "VBNET.ATG" 
			type = new TypeReference("System.Single", true); 
			break;
		}
		case 94: {
			lexer.NextToken();

//  1820 "VBNET.ATG" 
			type = new TypeReference("System.String", true); 
			break;
		}
		default: SynErr(281); break;
		}
	}

	void GetTypeTypeName(
//  2541 "VBNET.ATG" 
out TypeReference typeref) {

//  2542 "VBNET.ATG" 
		ArrayList rank = null; 
		NonArrayTypeName(
//  2544 "VBNET.ATG" 
out typeref, true);
		ArrayTypeModifiers(
//  2545 "VBNET.ATG" 
out rank);

//  2546 "VBNET.ATG" 
		if (rank != null && typeref != null) {
		typeref.RankSpecifier = (int[])rank.ToArray(typeof(int));
		}
		
	}

	void ConditionalExpression(
//  1753 "VBNET.ATG" 
out Expression expr) {

//  1755 "VBNET.ATG" 
		ConditionalExpression conditionalExpression = new ConditionalExpression();
		BinaryOperatorExpression binaryOperatorExpression = new BinaryOperatorExpression();
		conditionalExpression.StartLocation = binaryOperatorExpression.StartLocation = la.Location;
		
		Expression condition = null;
		Expression trueExpr = null;
		Expression falseExpr = null;
		
		Expect(136);
		Expect(38);
		Expr(
//  1764 "VBNET.ATG" 
out condition);
		Expect(23);
		Expr(
//  1764 "VBNET.ATG" 
out trueExpr);
		if (la.kind == 23) {
			lexer.NextToken();
			Expr(
//  1764 "VBNET.ATG" 
out falseExpr);
		}
		Expect(39);

//  1766 "VBNET.ATG" 
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
//  2473 "VBNET.ATG" 
out List<Expression> arguments) {

//  2475 "VBNET.ATG" 
		arguments = new List<Expression>();
		Expression expr = null;
		
		if (StartOf(29)) {
			Argument(
//  2478 "VBNET.ATG" 
out expr);
		}
		while (la.kind == 23) {
			lexer.NextToken();

//  2479 "VBNET.ATG" 
			arguments.Add(expr ?? Expression.Null); expr = null; 
			if (StartOf(29)) {
				Argument(
//  2480 "VBNET.ATG" 
out expr);
			}

//  2481 "VBNET.ATG" 
			if (expr == null) expr = Expression.Null; 
		}

//  2483 "VBNET.ATG" 
		if (expr != null) arguments.Add(expr); 
	}

	void ConjunctionExpr(
//  1839 "VBNET.ATG" 
out Expression outExpr) {

//  1841 "VBNET.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		NotExpr(
//  1844 "VBNET.ATG" 
out outExpr);
		while (la.kind == 61 || la.kind == 62) {
			if (la.kind == 61) {
				lexer.NextToken();

//  1847 "VBNET.ATG" 
				op = BinaryOperatorType.BitwiseAnd; 
			} else {
				lexer.NextToken();

//  1848 "VBNET.ATG" 
				op = BinaryOperatorType.LogicalAnd; 
			}
			NotExpr(
//  1850 "VBNET.ATG" 
out expr);

//  1850 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr);  
		}
	}

	void NotExpr(
//  1854 "VBNET.ATG" 
out Expression outExpr) {

//  1855 "VBNET.ATG" 
		UnaryOperatorType uop = UnaryOperatorType.None; 
		while (la.kind == 165) {
			lexer.NextToken();

//  1856 "VBNET.ATG" 
			uop = UnaryOperatorType.Not; 
		}
		ComparisonExpr(
//  1857 "VBNET.ATG" 
out outExpr);

//  1858 "VBNET.ATG" 
		if (uop != UnaryOperatorType.None)
		outExpr = new UnaryOperatorExpression(outExpr, uop);
		
	}

	void ComparisonExpr(
//  1863 "VBNET.ATG" 
out Expression outExpr) {

//  1865 "VBNET.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		ShiftExpr(
//  1868 "VBNET.ATG" 
out outExpr);
		while (StartOf(32)) {
			switch (la.kind) {
			case 41: {
				lexer.NextToken();

//  1871 "VBNET.ATG" 
				op = BinaryOperatorType.LessThan; 
				break;
			}
			case 40: {
				lexer.NextToken();

//  1872 "VBNET.ATG" 
				op = BinaryOperatorType.GreaterThan; 
				break;
			}
			case 44: {
				lexer.NextToken();

//  1873 "VBNET.ATG" 
				op = BinaryOperatorType.LessThanOrEqual; 
				break;
			}
			case 43: {
				lexer.NextToken();

//  1874 "VBNET.ATG" 
				op = BinaryOperatorType.GreaterThanOrEqual; 
				break;
			}
			case 42: {
				lexer.NextToken();

//  1875 "VBNET.ATG" 
				op = BinaryOperatorType.InEquality; 
				break;
			}
			case 21: {
				lexer.NextToken();

//  1876 "VBNET.ATG" 
				op = BinaryOperatorType.Equality; 
				break;
			}
			case 151: {
				lexer.NextToken();

//  1877 "VBNET.ATG" 
				op = BinaryOperatorType.Like; 
				break;
			}
			case 145: {
				lexer.NextToken();

//  1878 "VBNET.ATG" 
				op = BinaryOperatorType.ReferenceEquality; 
				break;
			}
			case 146: {
				lexer.NextToken();

//  1879 "VBNET.ATG" 
				op = BinaryOperatorType.ReferenceInequality; 
				break;
			}
			}
			if (StartOf(33)) {
				ShiftExpr(
//  1882 "VBNET.ATG" 
out expr);

//  1882 "VBNET.ATG" 
				outExpr = new BinaryOperatorExpression(outExpr, op, expr);  
			} else if (la.kind == 165) {
				lexer.NextToken();
				ShiftExpr(
//  1885 "VBNET.ATG" 
out expr);

//  1885 "VBNET.ATG" 
				outExpr = new BinaryOperatorExpression(outExpr, op, new UnaryOperatorExpression(expr, UnaryOperatorType.Not));  
			} else SynErr(282);
		}
	}

	void ShiftExpr(
//  1890 "VBNET.ATG" 
out Expression outExpr) {

//  1892 "VBNET.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		ConcatenationExpr(
//  1895 "VBNET.ATG" 
out outExpr);
		while (la.kind == 45 || la.kind == 46) {
			if (la.kind == 45) {
				lexer.NextToken();

//  1898 "VBNET.ATG" 
				op = BinaryOperatorType.ShiftLeft; 
			} else {
				lexer.NextToken();

//  1899 "VBNET.ATG" 
				op = BinaryOperatorType.ShiftRight; 
			}
			ConcatenationExpr(
//  1901 "VBNET.ATG" 
out expr);

//  1901 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr);  
		}
	}

	void ConcatenationExpr(
//  1905 "VBNET.ATG" 
out Expression outExpr) {

//  1906 "VBNET.ATG" 
		Expression expr; 
		AdditiveExpr(
//  1908 "VBNET.ATG" 
out outExpr);
		while (la.kind == 24) {
			lexer.NextToken();
			AdditiveExpr(
//  1908 "VBNET.ATG" 
out expr);

//  1908 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.Concat, expr);  
		}
	}

	void AdditiveExpr(
//  1911 "VBNET.ATG" 
out Expression outExpr) {

//  1913 "VBNET.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		ModuloExpr(
//  1916 "VBNET.ATG" 
out outExpr);
		while (la.kind == 31 || la.kind == 32) {
			if (la.kind == 32) {
				lexer.NextToken();

//  1919 "VBNET.ATG" 
				op = BinaryOperatorType.Add; 
			} else {
				lexer.NextToken();

//  1920 "VBNET.ATG" 
				op = BinaryOperatorType.Subtract; 
			}
			ModuloExpr(
//  1922 "VBNET.ATG" 
out expr);

//  1922 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr);  
		}
	}

	void ModuloExpr(
//  1926 "VBNET.ATG" 
out Expression outExpr) {

//  1927 "VBNET.ATG" 
		Expression expr; 
		IntegerDivisionExpr(
//  1929 "VBNET.ATG" 
out outExpr);
		while (la.kind == 155) {
			lexer.NextToken();
			IntegerDivisionExpr(
//  1929 "VBNET.ATG" 
out expr);

//  1929 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.Modulus, expr);  
		}
	}

	void IntegerDivisionExpr(
//  1932 "VBNET.ATG" 
out Expression outExpr) {

//  1933 "VBNET.ATG" 
		Expression expr; 
		MultiplicativeExpr(
//  1935 "VBNET.ATG" 
out outExpr);
		while (la.kind == 26) {
			lexer.NextToken();
			MultiplicativeExpr(
//  1935 "VBNET.ATG" 
out expr);

//  1935 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.DivideInteger, expr);  
		}
	}

	void MultiplicativeExpr(
//  1938 "VBNET.ATG" 
out Expression outExpr) {

//  1940 "VBNET.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		UnaryExpr(
//  1943 "VBNET.ATG" 
out outExpr);
		while (la.kind == 25 || la.kind == 35) {
			if (la.kind == 35) {
				lexer.NextToken();

//  1946 "VBNET.ATG" 
				op = BinaryOperatorType.Multiply; 
			} else {
				lexer.NextToken();

//  1947 "VBNET.ATG" 
				op = BinaryOperatorType.Divide; 
			}
			UnaryExpr(
//  1949 "VBNET.ATG" 
out expr);

//  1949 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr); 
		}
	}

	void UnaryExpr(
//  1953 "VBNET.ATG" 
out Expression uExpr) {

//  1955 "VBNET.ATG" 
		Expression expr;
		UnaryOperatorType uop = UnaryOperatorType.None;
		bool isUOp = false;
		
		while (la.kind == 31 || la.kind == 32 || la.kind == 35) {
			if (la.kind == 32) {
				lexer.NextToken();

//  1959 "VBNET.ATG" 
				uop = UnaryOperatorType.Plus; isUOp = true; 
			} else if (la.kind == 31) {
				lexer.NextToken();

//  1960 "VBNET.ATG" 
				uop = UnaryOperatorType.Minus; isUOp = true; 
			} else {
				lexer.NextToken();

//  1961 "VBNET.ATG" 
				uop = UnaryOperatorType.Dereference;  isUOp = true;
			}
		}
		ExponentiationExpr(
//  1963 "VBNET.ATG" 
out expr);

//  1965 "VBNET.ATG" 
		if (isUOp) {
		uExpr = new UnaryOperatorExpression(expr, uop);
		} else {
			uExpr = expr;
		}
		
	}

	void ExponentiationExpr(
//  1973 "VBNET.ATG" 
out Expression outExpr) {

//  1974 "VBNET.ATG" 
		Expression expr; 
		SimpleExpr(
//  1976 "VBNET.ATG" 
out outExpr);
		while (la.kind == 33) {
			lexer.NextToken();
			SimpleExpr(
//  1976 "VBNET.ATG" 
out expr);

//  1976 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.Power, expr);  
		}
	}

	void NormalOrReDimArgumentList(
//  2487 "VBNET.ATG" 
out List<Expression> arguments, out bool canBeNormal, out bool canBeRedim) {

//  2489 "VBNET.ATG" 
		arguments = new List<Expression>();
		canBeNormal = true; canBeRedim = !IsNamedAssign();
		Expression expr = null;
		
		if (StartOf(29)) {
			Argument(
//  2494 "VBNET.ATG" 
out expr);
			if (la.kind == 217) {
				lexer.NextToken();

//  2495 "VBNET.ATG" 
				EnsureIsZero(expr); canBeNormal = false; 
				Expr(
//  2496 "VBNET.ATG" 
out expr);
			}
		}
		while (la.kind == 23) {
			lexer.NextToken();

//  2499 "VBNET.ATG" 
			if (expr == null) canBeRedim = false; 

//  2500 "VBNET.ATG" 
			arguments.Add(expr ?? Expression.Null); expr = null; 

//  2501 "VBNET.ATG" 
			canBeRedim &= !IsNamedAssign(); 
			if (StartOf(29)) {
				Argument(
//  2502 "VBNET.ATG" 
out expr);
				if (la.kind == 217) {
					lexer.NextToken();

//  2503 "VBNET.ATG" 
					EnsureIsZero(expr); canBeNormal = false; 
					Expr(
//  2504 "VBNET.ATG" 
out expr);
				}
			}

//  2506 "VBNET.ATG" 
			if (expr == null) { canBeRedim = false; expr = Expression.Null; } 
		}

//  2508 "VBNET.ATG" 
		if (expr != null) arguments.Add(expr); else canBeRedim = false; 
	}

	void ArrayTypeModifiers(
//  2615 "VBNET.ATG" 
out ArrayList arrayModifiers) {

//  2617 "VBNET.ATG" 
		arrayModifiers = new ArrayList();
		int i = 0;
		
		while (
//  2620 "VBNET.ATG" 
IsDims()) {
			Expect(38);
			if (la.kind == 23 || la.kind == 39) {
				RankList(
//  2622 "VBNET.ATG" 
out i);
			}

//  2624 "VBNET.ATG" 
			arrayModifiers.Add(i);
			
			Expect(39);
		}

//  2629 "VBNET.ATG" 
		if(arrayModifiers.Count == 0) {
		 arrayModifiers = null;
		}
		
	}

	void MemberInitializer(
//  2454 "VBNET.ATG" 
out MemberInitializerExpression memberInitializer) {

//  2456 "VBNET.ATG" 
		memberInitializer = new MemberInitializerExpression();
		memberInitializer.StartLocation = la.Location;
		Expression initExpr = null;
		bool isKey = false;
		string name = null;
		
		if (la.kind == 148) {
			lexer.NextToken();

//  2462 "VBNET.ATG" 
			isKey = true; 
		}
		Expect(27);
		IdentifierOrKeyword(
//  2463 "VBNET.ATG" 
out name);
		Expect(21);
		Expr(
//  2463 "VBNET.ATG" 
out initExpr);

//  2465 "VBNET.ATG" 
		memberInitializer.Name = name;
		memberInitializer.Expression = initExpr;
		memberInitializer.IsKey = isKey;
		memberInitializer.EndLocation = t.EndLocation;
		
	}

	void SubLambdaExpression(
//  2059 "VBNET.ATG" 
out LambdaExpression lambda) {

//  2061 "VBNET.ATG" 
		lambda = new LambdaExpression();
		lambda.ReturnType = new TypeReference("System.Void", true);
		Expression inner = null;
		Statement statement = null;
		lambda.StartLocation = la.Location;
		
		Expect(211);
		if (la.kind == 38) {
			lexer.NextToken();
			if (StartOf(6)) {
				FormalParameterList(
//  2068 "VBNET.ATG" 
lambda.Parameters);
			}
			Expect(39);
		}
		if (StartOf(34)) {
			if (StartOf(29)) {
				Expr(
//  2071 "VBNET.ATG" 
out inner);

//  2073 "VBNET.ATG" 
				lambda.ExpressionBody = inner;
				lambda.EndLocation = t.EndLocation; // la.Location?
				
			} else {
				EmbeddedStatement(
//  2077 "VBNET.ATG" 
out statement);

//  2079 "VBNET.ATG" 
				lambda.StatementBody = statement;
				lambda.EndLocation = t.EndLocation;
				
			}
		} else if (la.kind == 1) {
			lexer.NextToken();
			Block(
//  2085 "VBNET.ATG" 
out statement);
			Expect(114);
			Expect(211);

//  2088 "VBNET.ATG" 
			lambda.StatementBody = statement;
			lambda.EndLocation = t.EndLocation;
			
		} else SynErr(283);
	}

	void FunctionLambdaExpression(
//  2094 "VBNET.ATG" 
out LambdaExpression lambda) {

//  2096 "VBNET.ATG" 
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
//  2103 "VBNET.ATG" 
lambda.Parameters);
			}
			Expect(39);
		}
		if (la.kind == 64) {
			lexer.NextToken();
			TypeName(
//  2104 "VBNET.ATG" 
out typeRef);

//  2104 "VBNET.ATG" 
			lambda.ReturnType = typeRef; 
		}
		if (StartOf(34)) {
			if (StartOf(29)) {
				Expr(
//  2107 "VBNET.ATG" 
out inner);

//  2109 "VBNET.ATG" 
				lambda.ExpressionBody = inner;
				lambda.EndLocation = t.EndLocation; // la.Location?
				
			} else {
				EmbeddedStatement(
//  2113 "VBNET.ATG" 
out statement);

//  2115 "VBNET.ATG" 
				lambda.StatementBody = statement;
				lambda.EndLocation = t.EndLocation;
				
			}
		} else if (la.kind == 1) {
			lexer.NextToken();
			Block(
//  2121 "VBNET.ATG" 
out statement);
			Expect(114);
			Expect(128);

//  2124 "VBNET.ATG" 
			lambda.StatementBody = statement;
			lambda.EndLocation = t.EndLocation;
			
		} else SynErr(284);
	}

	void EmbeddedStatement(
//  2886 "VBNET.ATG" 
out Statement statement) {

//  2888 "VBNET.ATG" 
		Statement embeddedStatement = null;
		statement = null;
		Expression expr = null;
		string name = String.Empty;
		List<Expression> p = null;
		
		if (la.kind == 121) {
			lexer.NextToken();

//  2894 "VBNET.ATG" 
			ExitType exitType = ExitType.None; 
			switch (la.kind) {
			case 211: {
				lexer.NextToken();

//  2896 "VBNET.ATG" 
				exitType = ExitType.Sub; 
				break;
			}
			case 128: {
				lexer.NextToken();

//  2898 "VBNET.ATG" 
				exitType = ExitType.Function; 
				break;
			}
			case 187: {
				lexer.NextToken();

//  2900 "VBNET.ATG" 
				exitType = ExitType.Property; 
				break;
			}
			case 109: {
				lexer.NextToken();

//  2902 "VBNET.ATG" 
				exitType = ExitType.Do; 
				break;
			}
			case 125: {
				lexer.NextToken();

//  2904 "VBNET.ATG" 
				exitType = ExitType.For; 
				break;
			}
			case 219: {
				lexer.NextToken();

//  2906 "VBNET.ATG" 
				exitType = ExitType.Try; 
				break;
			}
			case 232: {
				lexer.NextToken();

//  2908 "VBNET.ATG" 
				exitType = ExitType.While; 
				break;
			}
			case 198: {
				lexer.NextToken();

//  2910 "VBNET.ATG" 
				exitType = ExitType.Select; 
				break;
			}
			default: SynErr(285); break;
			}

//  2912 "VBNET.ATG" 
			statement = new ExitStatement(exitType); 
		} else if (la.kind == 219) {
			TryStatement(
//  2913 "VBNET.ATG" 
out statement);
		} else if (la.kind == 90) {
			lexer.NextToken();

//  2914 "VBNET.ATG" 
			ContinueType continueType = ContinueType.None; 
			if (la.kind == 109 || la.kind == 125 || la.kind == 232) {
				if (la.kind == 109) {
					lexer.NextToken();

//  2914 "VBNET.ATG" 
					continueType = ContinueType.Do; 
				} else if (la.kind == 125) {
					lexer.NextToken();

//  2914 "VBNET.ATG" 
					continueType = ContinueType.For; 
				} else {
					lexer.NextToken();

//  2914 "VBNET.ATG" 
					continueType = ContinueType.While; 
				}
			}

//  2914 "VBNET.ATG" 
			statement = new ContinueStatement(continueType); 
		} else if (la.kind == 216) {
			lexer.NextToken();
			if (StartOf(29)) {
				Expr(
//  2916 "VBNET.ATG" 
out expr);
			}

//  2916 "VBNET.ATG" 
			statement = new ThrowStatement(expr); 
		} else if (la.kind == 196) {
			lexer.NextToken();
			if (StartOf(29)) {
				Expr(
//  2918 "VBNET.ATG" 
out expr);
			}

//  2918 "VBNET.ATG" 
			statement = new ReturnStatement(expr); 
		} else if (la.kind == 212) {
			lexer.NextToken();
			Expr(
//  2920 "VBNET.ATG" 
out expr);
			EndOfStmt();
			Block(
//  2920 "VBNET.ATG" 
out embeddedStatement);
			Expect(114);
			Expect(212);

//  2921 "VBNET.ATG" 
			statement = new LockStatement(expr, embeddedStatement); 
		} else if (la.kind == 190) {
			lexer.NextToken();
			Identifier();

//  2923 "VBNET.ATG" 
			name = t.val; 
			if (la.kind == 38) {
				lexer.NextToken();
				if (StartOf(35)) {
					ArgumentList(
//  2924 "VBNET.ATG" 
out p);
				}
				Expect(39);
			}

//  2926 "VBNET.ATG" 
			statement = new RaiseEventStatement(name, p);
			
		} else if (la.kind == 234) {
			WithStatement(
//  2929 "VBNET.ATG" 
out statement);
		} else if (la.kind == 57) {
			lexer.NextToken();

//  2931 "VBNET.ATG" 
			Expression handlerExpr = null; 
			Expr(
//  2932 "VBNET.ATG" 
out expr);
			Expect(23);
			Expr(
//  2932 "VBNET.ATG" 
out handlerExpr);

//  2934 "VBNET.ATG" 
			statement = new AddHandlerStatement(expr, handlerExpr);
			
		} else if (la.kind == 194) {
			lexer.NextToken();

//  2937 "VBNET.ATG" 
			Expression handlerExpr = null; 
			Expr(
//  2938 "VBNET.ATG" 
out expr);
			Expect(23);
			Expr(
//  2938 "VBNET.ATG" 
out handlerExpr);

//  2940 "VBNET.ATG" 
			statement = new RemoveHandlerStatement(expr, handlerExpr);
			
		} else if (la.kind == 232) {
			lexer.NextToken();
			Expr(
//  2943 "VBNET.ATG" 
out expr);
			EndOfStmt();
			Block(
//  2944 "VBNET.ATG" 
out embeddedStatement);
			Expect(114);
			Expect(232);

//  2946 "VBNET.ATG" 
			statement = new DoLoopStatement(expr, embeddedStatement, ConditionType.While, ConditionPosition.Start);
			
		} else if (la.kind == 109) {
			lexer.NextToken();

//  2951 "VBNET.ATG" 
			ConditionType conditionType = ConditionType.None;
			
			if (la.kind == 225 || la.kind == 232) {
				WhileOrUntil(
//  2954 "VBNET.ATG" 
out conditionType);
				Expr(
//  2954 "VBNET.ATG" 
out expr);
				EndOfStmt();
				Block(
//  2955 "VBNET.ATG" 
out embeddedStatement);
				Expect(153);

//  2958 "VBNET.ATG" 
				statement = new DoLoopStatement(expr, 
				                               embeddedStatement, 
				                               conditionType == ConditionType.While ? ConditionType.DoWhile : conditionType, 
				                               ConditionPosition.Start);
				
			} else if (la.kind == 1 || la.kind == 22) {
				EndOfStmt();
				Block(
//  2965 "VBNET.ATG" 
out embeddedStatement);
				Expect(153);
				if (la.kind == 225 || la.kind == 232) {
					WhileOrUntil(
//  2966 "VBNET.ATG" 
out conditionType);
					Expr(
//  2966 "VBNET.ATG" 
out expr);
				}

//  2968 "VBNET.ATG" 
				statement = new DoLoopStatement(expr, embeddedStatement, conditionType, ConditionPosition.End);
				
			} else SynErr(286);
		} else if (la.kind == 125) {
			lexer.NextToken();

//  2973 "VBNET.ATG" 
			Expression group = null;
			TypeReference typeReference;
			string        typeName;
			Location startLocation = t.Location;
			
			if (la.kind == 111) {
				lexer.NextToken();
				LoopControlVariable(
//  2980 "VBNET.ATG" 
out typeReference, out typeName);
				Expect(139);
				Expr(
//  2981 "VBNET.ATG" 
out group);
				EndOfStmt();
				Block(
//  2982 "VBNET.ATG" 
out embeddedStatement);
				Expect(164);
				if (StartOf(29)) {
					Expr(
//  2983 "VBNET.ATG" 
out expr);
				}

//  2985 "VBNET.ATG" 
				statement = new ForeachStatement(typeReference, 
				                                typeName,
				                                group, 
				                                embeddedStatement, 
				                                expr);
				statement.StartLocation = startLocation;
				statement.EndLocation   = t.EndLocation;
				
				
			} else if (StartOf(36)) {

//  2996 "VBNET.ATG" 
				Expression start = null;
				Expression end = null;
				Expression step = null;
				Expression variableExpr = null;
				Expression nextExpr = null;
				List<Expression> nextExpressions = null;
				
				if (
//  3003 "VBNET.ATG" 
IsLoopVariableDeclaration()) {
					LoopControlVariable(
//  3004 "VBNET.ATG" 
out typeReference, out typeName);
				} else {

//  3006 "VBNET.ATG" 
					typeReference = null; typeName = null; 
					SimpleExpr(
//  3007 "VBNET.ATG" 
out variableExpr);
				}
				Expect(21);
				Expr(
//  3009 "VBNET.ATG" 
out start);
				Expect(217);
				Expr(
//  3009 "VBNET.ATG" 
out end);
				if (la.kind == 206) {
					lexer.NextToken();
					Expr(
//  3009 "VBNET.ATG" 
out step);
				}
				EndOfStmt();
				Block(
//  3010 "VBNET.ATG" 
out embeddedStatement);
				Expect(164);
				if (StartOf(29)) {
					Expr(
//  3013 "VBNET.ATG" 
out nextExpr);

//  3015 "VBNET.ATG" 
					nextExpressions = new List<Expression>();
					nextExpressions.Add(nextExpr);
					
					while (la.kind == 23) {
						lexer.NextToken();
						Expr(
//  3018 "VBNET.ATG" 
out nextExpr);

//  3018 "VBNET.ATG" 
						nextExpressions.Add(nextExpr); 
					}
				}

//  3021 "VBNET.ATG" 
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
				
			} else SynErr(287);
		} else if (la.kind == 119) {
			lexer.NextToken();
			Expr(
//  3034 "VBNET.ATG" 
out expr);

//  3034 "VBNET.ATG" 
			statement = new ErrorStatement(expr); 
		} else if (la.kind == 192) {
			lexer.NextToken();

//  3036 "VBNET.ATG" 
			bool isPreserve = false; 
			if (la.kind == 185) {
				lexer.NextToken();

//  3036 "VBNET.ATG" 
				isPreserve = true; 
			}
			ReDimClause(
//  3037 "VBNET.ATG" 
out expr);

//  3039 "VBNET.ATG" 
			ReDimStatement reDimStatement = new ReDimStatement(isPreserve);
			statement = reDimStatement;
			SafeAdd(reDimStatement, reDimStatement.ReDimClauses, expr as InvocationExpression);
			
			while (la.kind == 23) {
				lexer.NextToken();
				ReDimClause(
//  3043 "VBNET.ATG" 
out expr);

//  3044 "VBNET.ATG" 
				SafeAdd(reDimStatement, reDimStatement.ReDimClauses, expr as InvocationExpression); 
			}
		} else if (la.kind == 118) {
			lexer.NextToken();
			Expr(
//  3048 "VBNET.ATG" 
out expr);

//  3050 "VBNET.ATG" 
			EraseStatement eraseStatement = new EraseStatement();
			if (expr != null) { SafeAdd(eraseStatement, eraseStatement.Expressions, expr);}
			
			while (la.kind == 23) {
				lexer.NextToken();
				Expr(
//  3053 "VBNET.ATG" 
out expr);

//  3053 "VBNET.ATG" 
				if (expr != null) { SafeAdd(eraseStatement, eraseStatement.Expressions, expr); }
			}

//  3054 "VBNET.ATG" 
			statement = eraseStatement; 
		} else if (la.kind == 207) {
			lexer.NextToken();

//  3056 "VBNET.ATG" 
			statement = new StopStatement(); 
		} else if (
//  3058 "VBNET.ATG" 
la.kind == Tokens.If) {
			Expect(136);

//  3059 "VBNET.ATG" 
			Location ifStartLocation = t.Location; 
			Expr(
//  3059 "VBNET.ATG" 
out expr);
			if (la.kind == 215) {
				lexer.NextToken();
			}
			if (la.kind == 1 || la.kind == 22) {
				EndOfStmt();
				Block(
//  3062 "VBNET.ATG" 
out embeddedStatement);

//  3064 "VBNET.ATG" 
				IfElseStatement ifStatement = new IfElseStatement(expr, embeddedStatement);
				ifStatement.StartLocation = ifStartLocation;
				Location elseIfStart;
				
				while (la.kind == 113 || 
//  3070 "VBNET.ATG" 
IsElseIf()) {
					if (
//  3070 "VBNET.ATG" 
IsElseIf()) {
						Expect(112);

//  3070 "VBNET.ATG" 
						elseIfStart = t.Location; 
						Expect(136);
					} else {
						lexer.NextToken();

//  3071 "VBNET.ATG" 
						elseIfStart = t.Location; 
					}

//  3073 "VBNET.ATG" 
					Expression condition = null; Statement block = null; 
					Expr(
//  3074 "VBNET.ATG" 
out condition);
					if (la.kind == 215) {
						lexer.NextToken();
					}
					EndOfStmt();
					Block(
//  3075 "VBNET.ATG" 
out block);

//  3077 "VBNET.ATG" 
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
//  3086 "VBNET.ATG" 
out embeddedStatement);

//  3088 "VBNET.ATG" 
					ifStatement.FalseStatement.Add(embeddedStatement);
					
				}
				Expect(114);
				Expect(136);

//  3092 "VBNET.ATG" 
				ifStatement.EndLocation = t.Location;
				statement = ifStatement;
				
			} else if (StartOf(37)) {

//  3097 "VBNET.ATG" 
				IfElseStatement ifStatement = new IfElseStatement(expr);
				ifStatement.StartLocation = ifStartLocation;
				
				SingleLineStatementList(
//  3100 "VBNET.ATG" 
ifStatement.TrueStatement);
				if (la.kind == 112) {
					lexer.NextToken();
					if (StartOf(37)) {
						SingleLineStatementList(
//  3103 "VBNET.ATG" 
ifStatement.FalseStatement);
					}
				}

//  3105 "VBNET.ATG" 
				ifStatement.EndLocation = t.Location; statement = ifStatement; 
			} else SynErr(288);
		} else if (la.kind == 198) {
			lexer.NextToken();
			if (la.kind == 75) {
				lexer.NextToken();
			}
			Expr(
//  3108 "VBNET.ATG" 
out expr);
			EndOfStmt();

//  3109 "VBNET.ATG" 
			List<SwitchSection> selectSections = new List<SwitchSection>();
			Statement block = null;
			
			while (la.kind == 75) {

//  3113 "VBNET.ATG" 
				List<CaseLabel> caseClauses = null; Location caseLocation = la.Location; 
				lexer.NextToken();
				CaseClauses(
//  3114 "VBNET.ATG" 
out caseClauses);
				if (
//  3114 "VBNET.ATG" 
IsNotStatementSeparator()) {
					lexer.NextToken();
				}
				EndOfStmt();

//  3116 "VBNET.ATG" 
				SwitchSection selectSection = new SwitchSection(caseClauses);
				selectSection.StartLocation = caseLocation;
				
				Block(
//  3119 "VBNET.ATG" 
out block);

//  3121 "VBNET.ATG" 
				selectSection.Children = block.Children;
				selectSection.EndLocation = t.EndLocation;
				selectSections.Add(selectSection);
				
			}

//  3127 "VBNET.ATG" 
			statement = new SwitchStatement(expr, selectSections);
			
			Expect(114);
			Expect(198);
		} else if (la.kind == 172) {

//  3130 "VBNET.ATG" 
			OnErrorStatement onErrorStatement = null; 
			OnErrorStatement(
//  3131 "VBNET.ATG" 
out onErrorStatement);

//  3131 "VBNET.ATG" 
			statement = onErrorStatement; 
		} else if (la.kind == 133) {

//  3132 "VBNET.ATG" 
			GotoStatement goToStatement = null; 
			GotoStatement(
//  3133 "VBNET.ATG" 
out goToStatement);

//  3133 "VBNET.ATG" 
			statement = goToStatement; 
		} else if (la.kind == 195) {

//  3134 "VBNET.ATG" 
			ResumeStatement resumeStatement = null; 
			ResumeStatement(
//  3135 "VBNET.ATG" 
out resumeStatement);

//  3135 "VBNET.ATG" 
			statement = resumeStatement; 
		} else if (StartOf(36)) {

//  3138 "VBNET.ATG" 
			Expression val = null;
			AssignmentOperatorType op;
			
			bool mustBeAssignment = la.kind == Tokens.Plus  || la.kind == Tokens.Minus ||
			                        la.kind == Tokens.Not   || la.kind == Tokens.Times;
			
			SimpleExpr(
//  3144 "VBNET.ATG" 
out expr);
			if (StartOf(38)) {
				AssignmentOperator(
//  3146 "VBNET.ATG" 
out op);
				Expr(
//  3146 "VBNET.ATG" 
out val);

//  3146 "VBNET.ATG" 
				expr = new AssignmentExpression(expr, op, val); 
			} else if (StartOf(39)) {

//  3147 "VBNET.ATG" 
				if (mustBeAssignment) Error("error in assignment."); 
			} else SynErr(289);

//  3150 "VBNET.ATG" 
			// a field reference expression that stands alone is a
			// invocation expression without parantheses and arguments
			if(expr is MemberReferenceExpression || expr is IdentifierExpression) {
				expr = new InvocationExpression(expr);
			}
			statement = new ExpressionStatement(expr);
			
		} else if (la.kind == 74) {
			lexer.NextToken();
			SimpleExpr(
//  3157 "VBNET.ATG" 
out expr);

//  3157 "VBNET.ATG" 
			statement = new ExpressionStatement(expr); 
		} else if (la.kind == 227) {
			lexer.NextToken();

//  3159 "VBNET.ATG" 
			Statement block;  
			if (
//  3160 "VBNET.ATG" 
Peek(1).kind == Tokens.As) {

//  3161 "VBNET.ATG" 
				LocalVariableDeclaration resourceAquisition = new LocalVariableDeclaration(Modifiers.None); 
				VariableDeclarator(
//  3162 "VBNET.ATG" 
resourceAquisition.Variables);
				while (la.kind == 23) {
					lexer.NextToken();
					VariableDeclarator(
//  3164 "VBNET.ATG" 
resourceAquisition.Variables);
				}
				Block(
//  3166 "VBNET.ATG" 
out block);

//  3168 "VBNET.ATG" 
				statement = new UsingStatement(resourceAquisition, block);
				
			} else if (StartOf(29)) {
				Expr(
//  3170 "VBNET.ATG" 
out expr);
				Block(
//  3171 "VBNET.ATG" 
out block);

//  3172 "VBNET.ATG" 
				statement = new UsingStatement(new ExpressionStatement(expr), block); 
			} else SynErr(290);
			Expect(114);
			Expect(227);
		} else if (StartOf(40)) {
			LocalDeclarationStatement(
//  3175 "VBNET.ATG" 
out statement);
		} else SynErr(291);
	}

	void FromOrAggregateQueryOperator(
//  2143 "VBNET.ATG" 
List<QueryExpressionClause> middleClauses) {

//  2145 "VBNET.ATG" 
		QueryExpressionFromClause fromClause = null;
		QueryExpressionAggregateClause aggregateClause = null;
		
		if (la.kind == 127) {
			FromQueryOperator(
//  2148 "VBNET.ATG" 
out fromClause);

//  2149 "VBNET.ATG" 
			middleClauses.Add(fromClause); 
		} else if (la.kind == 59) {
			AggregateQueryOperator(
//  2150 "VBNET.ATG" 
out aggregateClause);

//  2151 "VBNET.ATG" 
			middleClauses.Add(aggregateClause); 
		} else SynErr(292);
	}

	void QueryOperator(
//  2154 "VBNET.ATG" 
List<QueryExpressionClause> middleClauses) {

//  2156 "VBNET.ATG" 
		QueryExpressionJoinVBClause joinClause = null;
		QueryExpressionGroupVBClause groupByClause = null;
		QueryExpressionPartitionVBClause partitionClause = null;
		QueryExpressionGroupJoinVBClause groupJoinClause = null;
		QueryExpressionFromClause fromClause = null;
		QueryExpressionAggregateClause aggregateClause = null;
		
		if (la.kind == 127) {
			FromQueryOperator(
//  2163 "VBNET.ATG" 
out fromClause);

//  2164 "VBNET.ATG" 
			middleClauses.Add(fromClause); 
		} else if (la.kind == 59) {
			AggregateQueryOperator(
//  2165 "VBNET.ATG" 
out aggregateClause);

//  2166 "VBNET.ATG" 
			middleClauses.Add(aggregateClause); 
		} else if (la.kind == 198) {
			SelectQueryOperator(
//  2167 "VBNET.ATG" 
middleClauses);
		} else if (la.kind == 108) {
			DistinctQueryOperator(
//  2168 "VBNET.ATG" 
middleClauses);
		} else if (la.kind == 231) {
			WhereQueryOperator(
//  2169 "VBNET.ATG" 
middleClauses);
		} else if (la.kind == 177) {
			OrderByQueryOperator(
//  2170 "VBNET.ATG" 
middleClauses);
		} else if (la.kind == 204 || la.kind == 213) {
			PartitionQueryOperator(
//  2171 "VBNET.ATG" 
out partitionClause);

//  2172 "VBNET.ATG" 
			middleClauses.Add(partitionClause); 
		} else if (la.kind == 149) {
			LetQueryOperator(
//  2173 "VBNET.ATG" 
middleClauses);
		} else if (la.kind == 147) {
			JoinQueryOperator(
//  2174 "VBNET.ATG" 
out joinClause);

//  2175 "VBNET.ATG" 
			middleClauses.Add(joinClause); 
		} else if (
//  2176 "VBNET.ATG" 
la.kind == Tokens.Group && Peek(1).kind == Tokens.Join) {
			GroupJoinQueryOperator(
//  2176 "VBNET.ATG" 
out groupJoinClause);

//  2177 "VBNET.ATG" 
			middleClauses.Add(groupJoinClause); 
		} else if (la.kind == 134) {
			GroupByQueryOperator(
//  2178 "VBNET.ATG" 
out groupByClause);

//  2179 "VBNET.ATG" 
			middleClauses.Add(groupByClause); 
		} else SynErr(293);
	}

	void FromQueryOperator(
//  2254 "VBNET.ATG" 
out QueryExpressionFromClause fromClause) {

//  2256 "VBNET.ATG" 
		fromClause = new QueryExpressionFromClause();
		fromClause.StartLocation = la.Location;
		
		Expect(127);
		CollectionRangeVariableDeclarationList(
//  2259 "VBNET.ATG" 
fromClause.Sources);

//  2261 "VBNET.ATG" 
		fromClause.EndLocation = t.EndLocation;
		
	}

	void AggregateQueryOperator(
//  2323 "VBNET.ATG" 
out QueryExpressionAggregateClause aggregateClause) {

//  2325 "VBNET.ATG" 
		aggregateClause = new QueryExpressionAggregateClause();
		aggregateClause.IntoVariables = new List<ExpressionRangeVariable>();
		aggregateClause.StartLocation = la.Location;
		CollectionRangeVariable source;
		
		Expect(59);
		CollectionRangeVariableDeclaration(
//  2330 "VBNET.ATG" 
out source);

//  2332 "VBNET.ATG" 
		aggregateClause.Source = source;
		
		while (StartOf(30)) {
			QueryOperator(
//  2335 "VBNET.ATG" 
aggregateClause.MiddleClauses);
		}
		Expect(144);
		ExpressionRangeVariableDeclarationList(
//  2337 "VBNET.ATG" 
aggregateClause.IntoVariables);

//  2339 "VBNET.ATG" 
		aggregateClause.EndLocation = t.EndLocation;
		
	}

	void SelectQueryOperator(
//  2265 "VBNET.ATG" 
List<QueryExpressionClause> middleClauses) {

//  2267 "VBNET.ATG" 
		QueryExpressionSelectVBClause selectClause = new QueryExpressionSelectVBClause();
		selectClause.StartLocation = la.Location;
		
		Expect(198);
		ExpressionRangeVariableDeclarationList(
//  2270 "VBNET.ATG" 
selectClause.Variables);

//  2272 "VBNET.ATG" 
		selectClause.EndLocation = t.Location;
		middleClauses.Add(selectClause);
		
	}

	void DistinctQueryOperator(
//  2277 "VBNET.ATG" 
List<QueryExpressionClause> middleClauses) {

//  2279 "VBNET.ATG" 
		QueryExpressionDistinctClause distinctClause = new QueryExpressionDistinctClause();
		distinctClause.StartLocation = la.Location;
		
		Expect(108);

//  2284 "VBNET.ATG" 
		distinctClause.EndLocation = t.EndLocation;
		middleClauses.Add(distinctClause);
		
	}

	void WhereQueryOperator(
//  2289 "VBNET.ATG" 
List<QueryExpressionClause> middleClauses) {

//  2291 "VBNET.ATG" 
		QueryExpressionWhereClause whereClause = new QueryExpressionWhereClause();
		whereClause.StartLocation = la.Location;
		Expression operand = null;
		
		Expect(231);
		Expr(
//  2295 "VBNET.ATG" 
out operand);

//  2297 "VBNET.ATG" 
		whereClause.Condition = operand;
		whereClause.EndLocation = t.EndLocation;
		
		middleClauses.Add(whereClause);
		
	}

	void OrderByQueryOperator(
//  2182 "VBNET.ATG" 
List<QueryExpressionClause> middleClauses) {

//  2184 "VBNET.ATG" 
		QueryExpressionOrderClause orderClause = new QueryExpressionOrderClause();
		orderClause.StartLocation = la.Location;
		List<QueryExpressionOrdering> orderings = null;
		
		Expect(177);
		Expect(71);
		OrderExpressionList(
//  2188 "VBNET.ATG" 
out orderings);

//  2190 "VBNET.ATG" 
		orderClause.Orderings = orderings;
		orderClause.EndLocation = t.EndLocation;
		middleClauses.Add(orderClause);
		
	}

	void PartitionQueryOperator(
//  2304 "VBNET.ATG" 
out QueryExpressionPartitionVBClause partitionClause) {

//  2306 "VBNET.ATG" 
		partitionClause = new QueryExpressionPartitionVBClause();
		partitionClause.StartLocation = la.Location;
		Expression expr = null;
		
		if (la.kind == 213) {
			lexer.NextToken();

//  2311 "VBNET.ATG" 
			partitionClause.PartitionType = QueryExpressionPartitionType.Take; 
			if (la.kind == 232) {
				lexer.NextToken();

//  2312 "VBNET.ATG" 
				partitionClause.PartitionType = QueryExpressionPartitionType.TakeWhile; 
			}
		} else if (la.kind == 204) {
			lexer.NextToken();

//  2313 "VBNET.ATG" 
			partitionClause.PartitionType = QueryExpressionPartitionType.Skip; 
			if (la.kind == 232) {
				lexer.NextToken();

//  2314 "VBNET.ATG" 
				partitionClause.PartitionType = QueryExpressionPartitionType.SkipWhile; 
			}
		} else SynErr(294);
		Expr(
//  2316 "VBNET.ATG" 
out expr);

//  2318 "VBNET.ATG" 
		partitionClause.Expression = expr;
		partitionClause.EndLocation = t.EndLocation;
		
	}

	void LetQueryOperator(
//  2343 "VBNET.ATG" 
List<QueryExpressionClause> middleClauses) {

//  2345 "VBNET.ATG" 
		QueryExpressionLetVBClause letClause = new QueryExpressionLetVBClause();
		letClause.StartLocation = la.Location;
		
		Expect(149);
		ExpressionRangeVariableDeclarationList(
//  2348 "VBNET.ATG" 
letClause.Variables);

//  2350 "VBNET.ATG" 
		letClause.EndLocation = t.EndLocation;
		middleClauses.Add(letClause);
		
	}

	void JoinQueryOperator(
//  2387 "VBNET.ATG" 
out QueryExpressionJoinVBClause joinClause) {

//  2389 "VBNET.ATG" 
		joinClause = new QueryExpressionJoinVBClause();
		joinClause.StartLocation = la.Location;
		CollectionRangeVariable joinVariable = null;
		QueryExpressionJoinVBClause subJoin = null;
		QueryExpressionJoinConditionVB condition = null;
		
		
		Expect(147);
		CollectionRangeVariableDeclaration(
//  2396 "VBNET.ATG" 
out joinVariable);

//  2397 "VBNET.ATG" 
		joinClause.JoinVariable = joinVariable; 
		if (la.kind == 147) {
			JoinQueryOperator(
//  2399 "VBNET.ATG" 
out subJoin);

//  2400 "VBNET.ATG" 
			joinClause.SubJoin = subJoin; 
		}
		Expect(172);
		JoinCondition(
//  2403 "VBNET.ATG" 
out condition);

//  2404 "VBNET.ATG" 
		SafeAdd(joinClause, joinClause.Conditions, condition); 
		while (la.kind == 61) {
			lexer.NextToken();
			JoinCondition(
//  2406 "VBNET.ATG" 
out condition);

//  2407 "VBNET.ATG" 
			SafeAdd(joinClause, joinClause.Conditions, condition); 
		}

//  2410 "VBNET.ATG" 
		joinClause.EndLocation = t.EndLocation;
		
	}

	void GroupJoinQueryOperator(
//  2240 "VBNET.ATG" 
out QueryExpressionGroupJoinVBClause groupJoinClause) {

//  2242 "VBNET.ATG" 
		groupJoinClause = new QueryExpressionGroupJoinVBClause();
		groupJoinClause.StartLocation = la.Location;
		QueryExpressionJoinVBClause joinClause = null;
		
		Expect(134);
		JoinQueryOperator(
//  2246 "VBNET.ATG" 
out joinClause);
		Expect(144);
		ExpressionRangeVariableDeclarationList(
//  2247 "VBNET.ATG" 
groupJoinClause.IntoVariables);

//  2249 "VBNET.ATG" 
		groupJoinClause.JoinClause = joinClause;
		groupJoinClause.EndLocation = t.EndLocation;
		
	}

	void GroupByQueryOperator(
//  2227 "VBNET.ATG" 
out QueryExpressionGroupVBClause groupByClause) {

//  2229 "VBNET.ATG" 
		groupByClause = new QueryExpressionGroupVBClause();
		groupByClause.StartLocation = la.Location;
		
		Expect(134);
		ExpressionRangeVariableDeclarationList(
//  2232 "VBNET.ATG" 
groupByClause.GroupVariables);
		Expect(71);
		ExpressionRangeVariableDeclarationList(
//  2233 "VBNET.ATG" 
groupByClause.ByVariables);
		Expect(144);
		ExpressionRangeVariableDeclarationList(
//  2234 "VBNET.ATG" 
groupByClause.IntoVariables);

//  2236 "VBNET.ATG" 
		groupByClause.EndLocation = t.EndLocation;
		
	}

	void OrderExpressionList(
//  2196 "VBNET.ATG" 
out List<QueryExpressionOrdering> orderings) {

//  2198 "VBNET.ATG" 
		orderings = new List<QueryExpressionOrdering>();
		QueryExpressionOrdering ordering = null;
		
		OrderExpression(
//  2201 "VBNET.ATG" 
out ordering);

//  2202 "VBNET.ATG" 
		orderings.Add(ordering); 
		while (la.kind == 23) {
			lexer.NextToken();
			OrderExpression(
//  2204 "VBNET.ATG" 
out ordering);

//  2205 "VBNET.ATG" 
			orderings.Add(ordering); 
		}
	}

	void OrderExpression(
//  2209 "VBNET.ATG" 
out QueryExpressionOrdering ordering) {

//  2211 "VBNET.ATG" 
		ordering = new QueryExpressionOrdering();
		ordering.StartLocation = la.Location;
		ordering.Direction = QueryExpressionOrderingDirection.None;
		Expression orderExpr = null;
		
		Expr(
//  2216 "VBNET.ATG" 
out orderExpr);

//  2218 "VBNET.ATG" 
		ordering.Criteria = orderExpr;
		
		if (la.kind == 65 || la.kind == 105) {
			if (la.kind == 65) {
				lexer.NextToken();

//  2221 "VBNET.ATG" 
				ordering.Direction = QueryExpressionOrderingDirection.Ascending; 
			} else {
				lexer.NextToken();

//  2222 "VBNET.ATG" 
				ordering.Direction = QueryExpressionOrderingDirection.Descending; 
			}
		}

//  2224 "VBNET.ATG" 
		ordering.EndLocation = t.EndLocation; 
	}

	void ExpressionRangeVariableDeclarationList(
//  2355 "VBNET.ATG" 
List<ExpressionRangeVariable> variables) {

//  2357 "VBNET.ATG" 
		ExpressionRangeVariable variable = null;
		
		ExpressionRangeVariableDeclaration(
//  2359 "VBNET.ATG" 
out variable);

//  2360 "VBNET.ATG" 
		variables.Add(variable); 
		while (la.kind == 23) {
			lexer.NextToken();
			ExpressionRangeVariableDeclaration(
//  2361 "VBNET.ATG" 
out variable);

//  2361 "VBNET.ATG" 
			variables.Add(variable); 
		}
	}

	void CollectionRangeVariableDeclarationList(
//  2414 "VBNET.ATG" 
List<CollectionRangeVariable> rangeVariables) {

//  2415 "VBNET.ATG" 
		CollectionRangeVariable variableDeclaration; 
		CollectionRangeVariableDeclaration(
//  2417 "VBNET.ATG" 
out variableDeclaration);

//  2418 "VBNET.ATG" 
		rangeVariables.Add(variableDeclaration); 
		while (la.kind == 23) {
			lexer.NextToken();
			CollectionRangeVariableDeclaration(
//  2419 "VBNET.ATG" 
out variableDeclaration);

//  2419 "VBNET.ATG" 
			rangeVariables.Add(variableDeclaration); 
		}
	}

	void CollectionRangeVariableDeclaration(
//  2422 "VBNET.ATG" 
out CollectionRangeVariable rangeVariable) {

//  2424 "VBNET.ATG" 
		rangeVariable = new CollectionRangeVariable();
		rangeVariable.StartLocation = la.Location;
		TypeReference typeName = null;
		Expression inExpr = null;
		
		Identifier();

//  2429 "VBNET.ATG" 
		rangeVariable.Identifier = t.val; 
		if (la.kind == 64) {
			lexer.NextToken();
			TypeName(
//  2430 "VBNET.ATG" 
out typeName);

//  2430 "VBNET.ATG" 
			rangeVariable.Type = typeName; 
		}
		Expect(139);
		Expr(
//  2431 "VBNET.ATG" 
out inExpr);

//  2433 "VBNET.ATG" 
		rangeVariable.Expression = inExpr;
		rangeVariable.EndLocation = t.EndLocation;
		
	}

	void ExpressionRangeVariableDeclaration(
//  2364 "VBNET.ATG" 
out ExpressionRangeVariable variable) {

//  2366 "VBNET.ATG" 
		variable = new ExpressionRangeVariable();
		variable.StartLocation = la.Location;
		Expression rhs = null;
		TypeReference typeName = null;
		
		if (
//  2372 "VBNET.ATG" 
IsIdentifiedExpressionRange()) {
			Identifier();

//  2373 "VBNET.ATG" 
			variable.Identifier = t.val; 
			if (la.kind == 64) {
				lexer.NextToken();
				TypeName(
//  2375 "VBNET.ATG" 
out typeName);

//  2376 "VBNET.ATG" 
				variable.Type = typeName; 
			}
			Expect(21);
		}
		Expr(
//  2380 "VBNET.ATG" 
out rhs);

//  2382 "VBNET.ATG" 
		variable.Expression = rhs;
		variable.EndLocation = t.EndLocation;
		
	}

	void JoinCondition(
//  2438 "VBNET.ATG" 
out QueryExpressionJoinConditionVB condition) {

//  2440 "VBNET.ATG" 
		condition = new QueryExpressionJoinConditionVB();
		condition.StartLocation = la.Location;
		
		Expression lhs = null;
		Expression rhs = null;
		
		Expr(
//  2446 "VBNET.ATG" 
out lhs);
		Expect(117);
		Expr(
//  2446 "VBNET.ATG" 
out rhs);

//  2448 "VBNET.ATG" 
		condition.LeftSide = lhs;
		condition.RightSide = rhs;
		condition.EndLocation = t.EndLocation;
		
	}

	void Argument(
//  2512 "VBNET.ATG" 
out Expression argumentexpr) {

//  2514 "VBNET.ATG" 
		Expression expr;
		argumentexpr = null;
		string name;
		
		if (
//  2518 "VBNET.ATG" 
IsNamedAssign()) {
			Identifier();

//  2518 "VBNET.ATG" 
			name = t.val;  
			Expect(56);
			Expr(
//  2518 "VBNET.ATG" 
out expr);

//  2520 "VBNET.ATG" 
			argumentexpr = new NamedArgumentExpression(name, expr);
			
		} else if (StartOf(29)) {
			Expr(
//  2523 "VBNET.ATG" 
out argumentexpr);
		} else SynErr(295);
	}

	void QualIdentAndTypeArguments(
//  2589 "VBNET.ATG" 
out TypeReference typeref, bool canBeUnbound) {

//  2590 "VBNET.ATG" 
		string name; typeref = null; 
		Qualident(
//  2592 "VBNET.ATG" 
out name);

//  2593 "VBNET.ATG" 
		typeref = new TypeReference(name); 
		if (
//  2594 "VBNET.ATG" 
la.kind == Tokens.OpenParenthesis && Peek(1).kind == Tokens.Of) {
			lexer.NextToken();
			Expect(170);
			if (
//  2596 "VBNET.ATG" 
canBeUnbound && (la.kind == Tokens.CloseParenthesis || la.kind == Tokens.Comma)) {

//  2597 "VBNET.ATG" 
				typeref.GenericTypes.Add(NullTypeReference.Instance); 
				while (la.kind == 23) {
					lexer.NextToken();

//  2598 "VBNET.ATG" 
					typeref.GenericTypes.Add(NullTypeReference.Instance); 
				}
			} else if (StartOf(8)) {
				TypeArgumentList(
//  2599 "VBNET.ATG" 
typeref.GenericTypes);
			} else SynErr(296);
			Expect(39);
		}
	}

	void RankList(
//  2636 "VBNET.ATG" 
out int i) {

//  2637 "VBNET.ATG" 
		i = 0; 
		while (la.kind == 23) {
			lexer.NextToken();

//  2638 "VBNET.ATG" 
			++i; 
		}
	}

	void Attribute(
//  2677 "VBNET.ATG" 
out ASTAttribute attribute) {

//  2678 "VBNET.ATG" 
		string name;
		List<Expression> positional = new List<Expression>();
		List<NamedArgumentExpression> named = new List<NamedArgumentExpression>();
		
		if (la.kind == 131) {
			lexer.NextToken();
			Expect(27);
		}
		Qualident(
//  2683 "VBNET.ATG" 
out name);
		if (la.kind == 38) {
			AttributeArguments(
//  2684 "VBNET.ATG" 
positional, named);
		}

//  2686 "VBNET.ATG" 
		attribute  = new ASTAttribute(name, positional, named);
		
	}

	void AttributeArguments(
//  2691 "VBNET.ATG" 
List<Expression> positional, List<NamedArgumentExpression> named) {

//  2693 "VBNET.ATG" 
		bool nameFound = false;
		string name = "";
		Expression expr;
		
		Expect(38);
		if (
//  2699 "VBNET.ATG" 
IsNotClosingParenthesis()) {
			if (
//  2701 "VBNET.ATG" 
IsNamedAssign()) {

//  2701 "VBNET.ATG" 
				nameFound = true; 
				IdentifierOrKeyword(
//  2702 "VBNET.ATG" 
out name);
				if (la.kind == 56) {
					lexer.NextToken();
				} else if (la.kind == 21) {
					lexer.NextToken();
				} else SynErr(297);
			}
			Expr(
//  2704 "VBNET.ATG" 
out expr);

//  2706 "VBNET.ATG" 
			if (expr != null) {
			if (string.IsNullOrEmpty(name)) { positional.Add(expr); }
			else { named.Add(new NamedArgumentExpression(name, expr)); name = ""; }
			}
			
			while (la.kind == 23) {
				lexer.NextToken();
				if (
//  2714 "VBNET.ATG" 
IsNamedAssign()) {

//  2714 "VBNET.ATG" 
					nameFound = true; 
					IdentifierOrKeyword(
//  2715 "VBNET.ATG" 
out name);
					if (la.kind == 56) {
						lexer.NextToken();
					} else if (la.kind == 21) {
						lexer.NextToken();
					} else SynErr(298);
				} else if (StartOf(29)) {

//  2717 "VBNET.ATG" 
					if (nameFound) Error("no positional argument after named argument"); 
				} else SynErr(299);
				Expr(
//  2718 "VBNET.ATG" 
out expr);

//  2718 "VBNET.ATG" 
				if (expr != null) { if(name == "") positional.Add(expr);
				else { named.Add(new NamedArgumentExpression(name, expr)); name = ""; }
				}
				
			}
		}
		Expect(39);
	}

	void FormalParameter(
//  2775 "VBNET.ATG" 
out ParameterDeclarationExpression p) {

//  2777 "VBNET.ATG" 
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		TypeReference type = null;
		ParamModifierList mod = new ParamModifierList(this);
		Expression expr = null;
		p = null;
		ArrayList arrayModifiers = null;
		
		while (la.kind == 41) {
			AttributeSection(
//  2786 "VBNET.ATG" 
out section);

//  2786 "VBNET.ATG" 
			attributes.Add(section); 
		}
		while (StartOf(41)) {
			ParameterModifier(
//  2787 "VBNET.ATG" 
mod);
		}
		Identifier();

//  2788 "VBNET.ATG" 
		string parameterName = t.val; 
		if (
//  2789 "VBNET.ATG" 
IsDims()) {
			ArrayTypeModifiers(
//  2789 "VBNET.ATG" 
out arrayModifiers);
		}
		if (la.kind == 64) {
			lexer.NextToken();
			TypeName(
//  2790 "VBNET.ATG" 
out type);
		}

//  2792 "VBNET.ATG" 
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
//  2802 "VBNET.ATG" 
out expr);
		}

//  2804 "VBNET.ATG" 
		mod.Check();
		p = new ParameterDeclarationExpression(type, parameterName, mod.Modifier, expr);
		p.Attributes = attributes;
		
	}

	void ParameterModifier(
//  3488 "VBNET.ATG" 
ParamModifierList m) {
		if (la.kind == 73) {
			lexer.NextToken();

//  3489 "VBNET.ATG" 
			m.Add(ParameterModifiers.In); 
		} else if (la.kind == 70) {
			lexer.NextToken();

//  3490 "VBNET.ATG" 
			m.Add(ParameterModifiers.Ref); 
		} else if (la.kind == 175) {
			lexer.NextToken();

//  3491 "VBNET.ATG" 
			m.Add(ParameterModifiers.Optional); 
		} else if (la.kind == 183) {
			lexer.NextToken();

//  3492 "VBNET.ATG" 
			m.Add(ParameterModifiers.Params); 
		} else SynErr(300);
	}

	void Statement() {

//  2833 "VBNET.ATG" 
		Statement stmt = null;
		Location startPos = la.Location;
		string label = String.Empty;
		
		
		if (la.kind == 1 || la.kind == 22) {
		} else if (
//  2839 "VBNET.ATG" 
IsLabel()) {
			LabelName(
//  2839 "VBNET.ATG" 
out label);

//  2841 "VBNET.ATG" 
			AddChild(new LabelStatement(t.val));
			
			Expect(22);
			Statement();
		} else if (StartOf(42)) {
			EmbeddedStatement(
//  2844 "VBNET.ATG" 
out stmt);

//  2844 "VBNET.ATG" 
			AddChild(stmt); 
		} else SynErr(301);

//  2847 "VBNET.ATG" 
		if (stmt != null) {
		stmt.StartLocation = startPos;
		stmt.EndLocation = t.Location;
		}
		
	}

	void LabelName(
//  3262 "VBNET.ATG" 
out string name) {

//  3264 "VBNET.ATG" 
		name = String.Empty;
		
		if (StartOf(4)) {
			Identifier();

//  3266 "VBNET.ATG" 
			name = t.val; 
		} else if (la.kind == 5) {
			lexer.NextToken();

//  3267 "VBNET.ATG" 
			name = t.val; 
		} else SynErr(302);
	}

	void LocalDeclarationStatement(
//  2855 "VBNET.ATG" 
out Statement statement) {

//  2857 "VBNET.ATG" 
		ModifierList m = new ModifierList();
		LocalVariableDeclaration localVariableDeclaration;
		bool dimfound = false;
		
		while (la.kind == 89 || la.kind == 106 || la.kind == 205) {
			if (la.kind == 89) {
				lexer.NextToken();

//  2863 "VBNET.ATG" 
				m.Add(Modifiers.Const, t.Location); 
			} else if (la.kind == 205) {
				lexer.NextToken();

//  2864 "VBNET.ATG" 
				m.Add(Modifiers.Static, t.Location); 
			} else {
				lexer.NextToken();

//  2865 "VBNET.ATG" 
				dimfound = true; 
			}
		}

//  2868 "VBNET.ATG" 
		if(dimfound && (m.Modifier & Modifiers.Const) != 0) {
		Error("Dim is not allowed on constants.");
		}
		
		if(m.isNone && dimfound == false) {
			Error("Const, Dim or Static expected");
		}
		
		localVariableDeclaration = new LocalVariableDeclaration(m.Modifier);
		localVariableDeclaration.StartLocation = t.Location;
		
		VariableDeclarator(
//  2879 "VBNET.ATG" 
localVariableDeclaration.Variables);
		while (la.kind == 23) {
			lexer.NextToken();
			VariableDeclarator(
//  2880 "VBNET.ATG" 
localVariableDeclaration.Variables);
		}

//  2882 "VBNET.ATG" 
		statement = localVariableDeclaration;
		
	}

	void TryStatement(
//  3376 "VBNET.ATG" 
out Statement tryStatement) {

//  3378 "VBNET.ATG" 
		Statement blockStmt = null, finallyStmt = null;List<CatchClause> catchClauses = null;
		
		Expect(219);
		EndOfStmt();
		Block(
//  3381 "VBNET.ATG" 
out blockStmt);
		if (la.kind == 76 || la.kind == 114 || la.kind == 124) {
			CatchClauses(
//  3382 "VBNET.ATG" 
out catchClauses);
		}
		if (la.kind == 124) {
			lexer.NextToken();
			EndOfStmt();
			Block(
//  3383 "VBNET.ATG" 
out finallyStmt);
		}
		Expect(114);
		Expect(219);

//  3386 "VBNET.ATG" 
		tryStatement = new TryCatchStatement(blockStmt, catchClauses, finallyStmt);
		
	}

	void WithStatement(
//  3356 "VBNET.ATG" 
out Statement withStatement) {

//  3358 "VBNET.ATG" 
		Statement blockStmt = null;
		Expression expr = null;
		
		Expect(234);

//  3361 "VBNET.ATG" 
		Location start = t.Location; 
		Expr(
//  3362 "VBNET.ATG" 
out expr);
		EndOfStmt();

//  3364 "VBNET.ATG" 
		withStatement = new WithStatement(expr);
		withStatement.StartLocation = start;
		
		Block(
//  3367 "VBNET.ATG" 
out blockStmt);

//  3369 "VBNET.ATG" 
		((WithStatement)withStatement).Body = (BlockStatement)blockStmt;
		
		Expect(114);
		Expect(234);

//  3372 "VBNET.ATG" 
		withStatement.EndLocation = t.Location; 
	}

	void WhileOrUntil(
//  3349 "VBNET.ATG" 
out ConditionType conditionType) {

//  3350 "VBNET.ATG" 
		conditionType = ConditionType.None; 
		if (la.kind == 232) {
			lexer.NextToken();

//  3351 "VBNET.ATG" 
			conditionType = ConditionType.While; 
		} else if (la.kind == 225) {
			lexer.NextToken();

//  3352 "VBNET.ATG" 
			conditionType = ConditionType.Until; 
		} else SynErr(303);
	}

	void LoopControlVariable(
//  3192 "VBNET.ATG" 
out TypeReference type, out string name) {

//  3193 "VBNET.ATG" 
		ArrayList arrayModifiers = null;
		type = null;
		
		Qualident(
//  3197 "VBNET.ATG" 
out name);
		if (
//  3198 "VBNET.ATG" 
IsDims()) {
			ArrayTypeModifiers(
//  3198 "VBNET.ATG" 
out arrayModifiers);
		}
		if (la.kind == 64) {
			lexer.NextToken();
			TypeName(
//  3199 "VBNET.ATG" 
out type);

//  3199 "VBNET.ATG" 
			if (name.IndexOf('.') > 0) { Error("No type def for 'for each' member indexer allowed."); } 
		}

//  3201 "VBNET.ATG" 
		if (type != null) {
		if(type.RankSpecifier != null && arrayModifiers != null) {
			Error("array rank only allowed one time");
		} else if (arrayModifiers != null) {
			type.RankSpecifier = (int[])arrayModifiers.ToArray(typeof(int));
		}
		}
		
	}

	void ReDimClause(
//  3271 "VBNET.ATG" 
out Expression expr) {
		SimpleNonInvocationExpression(
//  3273 "VBNET.ATG" 
out expr);
		ReDimClauseInternal(
//  3274 "VBNET.ATG" 
ref expr);
	}

	void SingleLineStatementList(
//  3178 "VBNET.ATG" 
List<Statement> list) {

//  3179 "VBNET.ATG" 
		Statement embeddedStatement = null; 
		if (la.kind == 114) {
			lexer.NextToken();

//  3181 "VBNET.ATG" 
			embeddedStatement = new EndStatement(); 
		} else if (StartOf(42)) {
			EmbeddedStatement(
//  3182 "VBNET.ATG" 
out embeddedStatement);
		} else SynErr(304);

//  3183 "VBNET.ATG" 
		if (embeddedStatement != null) list.Add(embeddedStatement); 
		while (la.kind == 22) {
			lexer.NextToken();
			while (la.kind == 22) {
				lexer.NextToken();
			}
			if (la.kind == 114) {
				lexer.NextToken();

//  3185 "VBNET.ATG" 
				embeddedStatement = new EndStatement(); 
			} else if (StartOf(42)) {
				EmbeddedStatement(
//  3186 "VBNET.ATG" 
out embeddedStatement);
			} else SynErr(305);

//  3187 "VBNET.ATG" 
			if (embeddedStatement != null) list.Add(embeddedStatement); 
		}
	}

	void CaseClauses(
//  3309 "VBNET.ATG" 
out List<CaseLabel> caseClauses) {

//  3311 "VBNET.ATG" 
		caseClauses = new List<CaseLabel>();
		CaseLabel caseClause = null;
		
		CaseClause(
//  3314 "VBNET.ATG" 
out caseClause);

//  3314 "VBNET.ATG" 
		if (caseClause != null) { caseClauses.Add(caseClause); } 
		while (la.kind == 23) {
			lexer.NextToken();
			CaseClause(
//  3315 "VBNET.ATG" 
out caseClause);

//  3315 "VBNET.ATG" 
			if (caseClause != null) { caseClauses.Add(caseClause); } 
		}
	}

	void OnErrorStatement(
//  3212 "VBNET.ATG" 
out OnErrorStatement stmt) {

//  3214 "VBNET.ATG" 
		stmt = null;
		GotoStatement goToStatement = null;
		
		Expect(172);
		Expect(119);
		if (
//  3220 "VBNET.ATG" 
IsNegativeLabelName()) {
			Expect(133);
			Expect(31);
			Expect(5);

//  3222 "VBNET.ATG" 
			long intLabel = Int64.Parse(t.val);
			if(intLabel != 1) {
				Error("invalid label in on error statement.");
			}
			stmt = new OnErrorStatement(new GotoStatement((intLabel * -1).ToString()));
			
		} else if (la.kind == 133) {
			GotoStatement(
//  3228 "VBNET.ATG" 
out goToStatement);

//  3230 "VBNET.ATG" 
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
			
		} else if (la.kind == 195) {
			lexer.NextToken();
			Expect(164);

//  3244 "VBNET.ATG" 
			stmt = new OnErrorStatement(new ResumeStatement(true));
			
		} else SynErr(306);
	}

	void GotoStatement(
//  3250 "VBNET.ATG" 
out GotoStatement goToStatement) {

//  3252 "VBNET.ATG" 
		string label = String.Empty;
		
		Expect(133);
		LabelName(
//  3255 "VBNET.ATG" 
out label);

//  3257 "VBNET.ATG" 
		goToStatement = new GotoStatement(label);
		
	}

	void ResumeStatement(
//  3298 "VBNET.ATG" 
out ResumeStatement resumeStatement) {

//  3300 "VBNET.ATG" 
		resumeStatement = null;
		string label = String.Empty;
		
		if (
//  3303 "VBNET.ATG" 
IsResumeNext()) {
			Expect(195);
			Expect(164);

//  3304 "VBNET.ATG" 
			resumeStatement = new ResumeStatement(true); 
		} else if (la.kind == 195) {
			lexer.NextToken();
			if (StartOf(43)) {
				LabelName(
//  3305 "VBNET.ATG" 
out label);
			}

//  3305 "VBNET.ATG" 
			resumeStatement = new ResumeStatement(label); 
		} else SynErr(307);
	}

	void ReDimClauseInternal(
//  3277 "VBNET.ATG" 
ref Expression expr) {

//  3278 "VBNET.ATG" 
		List<Expression> arguments; bool canBeNormal; bool canBeRedim; string name; 
		while (la.kind == 27 || 
//  3281 "VBNET.ATG" 
la.kind == Tokens.OpenParenthesis && Peek(1).kind == Tokens.Of) {
			if (la.kind == 27) {
				lexer.NextToken();
				IdentifierOrKeyword(
//  3280 "VBNET.ATG" 
out name);

//  3280 "VBNET.ATG" 
				expr = new MemberReferenceExpression(expr, name); 
			} else {
				InvocationExpression(
//  3282 "VBNET.ATG" 
ref expr);
			}
		}
		Expect(38);
		NormalOrReDimArgumentList(
//  3285 "VBNET.ATG" 
out arguments, out canBeNormal, out canBeRedim);
		Expect(39);

//  3287 "VBNET.ATG" 
		expr = new InvocationExpression(expr, arguments);
		if (canBeRedim == false || canBeNormal && (la.kind == Tokens.Dot || la.kind == Tokens.OpenParenthesis)) {
			if (this.Errors.Count == 0) {
				// don't recurse on parse errors - could result in endless recursion
				ReDimClauseInternal(ref expr);
			}
		}
		
	}

	void CaseClause(
//  3319 "VBNET.ATG" 
out CaseLabel caseClause) {

//  3321 "VBNET.ATG" 
		Expression expr = null;
		Expression sexpr = null;
		BinaryOperatorType op = BinaryOperatorType.None;
		caseClause = null;
		
		if (la.kind == 112) {
			lexer.NextToken();

//  3327 "VBNET.ATG" 
			caseClause = new CaseLabel(); 
		} else if (StartOf(44)) {
			if (la.kind == 145) {
				lexer.NextToken();
			}
			switch (la.kind) {
			case 41: {
				lexer.NextToken();

//  3331 "VBNET.ATG" 
				op = BinaryOperatorType.LessThan; 
				break;
			}
			case 40: {
				lexer.NextToken();

//  3332 "VBNET.ATG" 
				op = BinaryOperatorType.GreaterThan; 
				break;
			}
			case 44: {
				lexer.NextToken();

//  3333 "VBNET.ATG" 
				op = BinaryOperatorType.LessThanOrEqual; 
				break;
			}
			case 43: {
				lexer.NextToken();

//  3334 "VBNET.ATG" 
				op = BinaryOperatorType.GreaterThanOrEqual; 
				break;
			}
			case 21: {
				lexer.NextToken();

//  3335 "VBNET.ATG" 
				op = BinaryOperatorType.Equality; 
				break;
			}
			case 42: {
				lexer.NextToken();

//  3336 "VBNET.ATG" 
				op = BinaryOperatorType.InEquality; 
				break;
			}
			default: SynErr(308); break;
			}
			Expr(
//  3338 "VBNET.ATG" 
out expr);

//  3340 "VBNET.ATG" 
			caseClause = new CaseLabel(op, expr);
			
		} else if (StartOf(29)) {
			Expr(
//  3342 "VBNET.ATG" 
out expr);
			if (la.kind == 217) {
				lexer.NextToken();
				Expr(
//  3342 "VBNET.ATG" 
out sexpr);
			}

//  3344 "VBNET.ATG" 
			caseClause = new CaseLabel(expr, sexpr);
			
		} else SynErr(309);
	}

	void CatchClauses(
//  3391 "VBNET.ATG" 
out List<CatchClause> catchClauses) {

//  3393 "VBNET.ATG" 
		catchClauses = new List<CatchClause>();
		TypeReference type = null;
		Statement blockStmt = null;
		Expression expr = null;
		string name = String.Empty;
		
		while (la.kind == 76) {
			lexer.NextToken();
			if (StartOf(4)) {
				Identifier();

//  3401 "VBNET.ATG" 
				name = t.val; 
				if (la.kind == 64) {
					lexer.NextToken();
					TypeName(
//  3401 "VBNET.ATG" 
out type);
				}
			}
			if (la.kind == 230) {
				lexer.NextToken();
				Expr(
//  3402 "VBNET.ATG" 
out expr);
			}
			EndOfStmt();
			Block(
//  3404 "VBNET.ATG" 
out blockStmt);

//  3405 "VBNET.ATG" 
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
			case 179: s = "\"Out\" expected"; break;
			case 180: s = "\"Overloads\" expected"; break;
			case 181: s = "\"Overridable\" expected"; break;
			case 182: s = "\"Overrides\" expected"; break;
			case 183: s = "\"ParamArray\" expected"; break;
			case 184: s = "\"Partial\" expected"; break;
			case 185: s = "\"Preserve\" expected"; break;
			case 186: s = "\"Private\" expected"; break;
			case 187: s = "\"Property\" expected"; break;
			case 188: s = "\"Protected\" expected"; break;
			case 189: s = "\"Public\" expected"; break;
			case 190: s = "\"RaiseEvent\" expected"; break;
			case 191: s = "\"ReadOnly\" expected"; break;
			case 192: s = "\"ReDim\" expected"; break;
			case 193: s = "\"Rem\" expected"; break;
			case 194: s = "\"RemoveHandler\" expected"; break;
			case 195: s = "\"Resume\" expected"; break;
			case 196: s = "\"Return\" expected"; break;
			case 197: s = "\"SByte\" expected"; break;
			case 198: s = "\"Select\" expected"; break;
			case 199: s = "\"Set\" expected"; break;
			case 200: s = "\"Shadows\" expected"; break;
			case 201: s = "\"Shared\" expected"; break;
			case 202: s = "\"Short\" expected"; break;
			case 203: s = "\"Single\" expected"; break;
			case 204: s = "\"Skip\" expected"; break;
			case 205: s = "\"Static\" expected"; break;
			case 206: s = "\"Step\" expected"; break;
			case 207: s = "\"Stop\" expected"; break;
			case 208: s = "\"Strict\" expected"; break;
			case 209: s = "\"String\" expected"; break;
			case 210: s = "\"Structure\" expected"; break;
			case 211: s = "\"Sub\" expected"; break;
			case 212: s = "\"SyncLock\" expected"; break;
			case 213: s = "\"Take\" expected"; break;
			case 214: s = "\"Text\" expected"; break;
			case 215: s = "\"Then\" expected"; break;
			case 216: s = "\"Throw\" expected"; break;
			case 217: s = "\"To\" expected"; break;
			case 218: s = "\"True\" expected"; break;
			case 219: s = "\"Try\" expected"; break;
			case 220: s = "\"TryCast\" expected"; break;
			case 221: s = "\"TypeOf\" expected"; break;
			case 222: s = "\"UInteger\" expected"; break;
			case 223: s = "\"ULong\" expected"; break;
			case 224: s = "\"Unicode\" expected"; break;
			case 225: s = "\"Until\" expected"; break;
			case 226: s = "\"UShort\" expected"; break;
			case 227: s = "\"Using\" expected"; break;
			case 228: s = "\"Variant\" expected"; break;
			case 229: s = "\"Wend\" expected"; break;
			case 230: s = "\"When\" expected"; break;
			case 231: s = "\"Where\" expected"; break;
			case 232: s = "\"While\" expected"; break;
			case 233: s = "\"Widening\" expected"; break;
			case 234: s = "\"With\" expected"; break;
			case 235: s = "\"WithEvents\" expected"; break;
			case 236: s = "\"WriteOnly\" expected"; break;
			case 237: s = "\"Xor\" expected"; break;
			case 238: s = "\"GetXmlNamespace\" expected"; break;
			case 239: s = "??? expected"; break;
			case 240: s = "invalid EndOfStmt"; break;
			case 241: s = "invalid OptionStmt"; break;
			case 242: s = "invalid OptionStmt"; break;
			case 243: s = "invalid GlobalAttributeSection"; break;
			case 244: s = "invalid GlobalAttributeSection"; break;
			case 245: s = "invalid NamespaceMemberDecl"; break;
			case 246: s = "invalid OptionValue"; break;
			case 247: s = "invalid ImportClause"; break;
			case 248: s = "invalid Identifier"; break;
			case 249: s = "invalid TypeModifier"; break;
			case 250: s = "invalid NonModuleDeclaration"; break;
			case 251: s = "invalid NonModuleDeclaration"; break;
			case 252: s = "invalid TypeParameterConstraints"; break;
			case 253: s = "invalid TypeParameterConstraint"; break;
			case 254: s = "invalid NonArrayTypeName"; break;
			case 255: s = "invalid MemberModifier"; break;
			case 256: s = "invalid StructureMemberDecl"; break;
			case 257: s = "invalid StructureMemberDecl"; break;
			case 258: s = "invalid StructureMemberDecl"; break;
			case 259: s = "invalid StructureMemberDecl"; break;
			case 260: s = "invalid StructureMemberDecl"; break;
			case 261: s = "invalid StructureMemberDecl"; break;
			case 262: s = "invalid StructureMemberDecl"; break;
			case 263: s = "invalid StructureMemberDecl"; break;
			case 264: s = "invalid InterfaceMemberDecl"; break;
			case 265: s = "invalid InterfaceMemberDecl"; break;
			case 266: s = "invalid Expr"; break;
			case 267: s = "invalid Charset"; break;
			case 268: s = "invalid IdentifierForFieldDeclaration"; break;
			case 269: s = "invalid VariableDeclaratorPartAfterIdentifier"; break;
			case 270: s = "invalid AccessorDecls"; break;
			case 271: s = "invalid EventAccessorDeclaration"; break;
			case 272: s = "invalid OverloadableOperator"; break;
			case 273: s = "invalid EventMemberSpecifier"; break;
			case 274: s = "invalid LambdaExpr"; break;
			case 275: s = "invalid AssignmentOperator"; break;
			case 276: s = "invalid SimpleNonInvocationExpression"; break;
			case 277: s = "invalid SimpleNonInvocationExpression"; break;
			case 278: s = "invalid SimpleNonInvocationExpression"; break;
			case 279: s = "invalid SimpleNonInvocationExpression"; break;
			case 280: s = "invalid PrimitiveTypeName"; break;
			case 281: s = "invalid CastTarget"; break;
			case 282: s = "invalid ComparisonExpr"; break;
			case 283: s = "invalid SubLambdaExpression"; break;
			case 284: s = "invalid FunctionLambdaExpression"; break;
			case 285: s = "invalid EmbeddedStatement"; break;
			case 286: s = "invalid EmbeddedStatement"; break;
			case 287: s = "invalid EmbeddedStatement"; break;
			case 288: s = "invalid EmbeddedStatement"; break;
			case 289: s = "invalid EmbeddedStatement"; break;
			case 290: s = "invalid EmbeddedStatement"; break;
			case 291: s = "invalid EmbeddedStatement"; break;
			case 292: s = "invalid FromOrAggregateQueryOperator"; break;
			case 293: s = "invalid QueryOperator"; break;
			case 294: s = "invalid PartitionQueryOperator"; break;
			case 295: s = "invalid Argument"; break;
			case 296: s = "invalid QualIdentAndTypeArguments"; break;
			case 297: s = "invalid AttributeArguments"; break;
			case 298: s = "invalid AttributeArguments"; break;
			case 299: s = "invalid AttributeArguments"; break;
			case 300: s = "invalid ParameterModifier"; break;
			case 301: s = "invalid Statement"; break;
			case 302: s = "invalid LabelName"; break;
			case 303: s = "invalid WhileOrUntil"; break;
			case 304: s = "invalid SingleLineStatementList"; break;
			case 305: s = "invalid SingleLineStatementList"; break;
			case 306: s = "invalid OnErrorStatement"; break;
			case 307: s = "invalid ResumeStatement"; break;
			case 308: s = "invalid CaseClause"; break;
			case 309: s = "invalid CaseClause"; break;

			default: s = "error " + errorNumber; break;
		}
		this.Errors.Error(line, col, s);
	}
	
	private bool StartOf(int s)
	{
		return set[s, lexer.LookAhead.kind];
	}
	
	static bool[,] set = {
	{T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,x, T,T,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,x, T,T,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,x, T,T,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,T,T,T, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,x, T,x,x,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, T,x,x,x, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,T, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,T,T,T, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, T,x,x,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, T,x,x,x, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,T, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,T,T,T, T,x,T,T, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,x, T,x,x,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, T,x,x,x, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,T,x,T, x,x,x,T, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,T,T,T, T,T,x,T, T,x,x,x, x,x,x,x, x,x,x,T, x,T,x,x, T,x,x,x, x,x,x,x, x,x,x,T, T,T,x,x, x,T,x,x, T,x,T,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,T, x,x,x,T, x,x,T,x, x,x,x,x, T,x,T,x, T,x,x,T, T,x,x,x, T,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,T, x,x,x,x, x,T,x,T, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,T,T, T,x,x,x, x,T,T,x, x,T,T,x, x,x,x,x, x,x,T,T, T,T,T,x, x,x,x,T, x,x,x,x, x,x,x,x, x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,T,T,T, T,T,x,T, T,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,T, T,T,x,x, x,T,x,x, T,x,T,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,T, x,x,x,T, x,x,T,x, x,x,x,x, T,x,T,x, T,x,x,T, T,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, x,x,x,x, x,T,x,T, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,T,T, T,x,x,x, x,T,x,x, x,T,T,x, x,x,x,x, x,x,T,T, T,T,T,x, x,x,x,T, x,x,x,x, x,x,x,x, x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,T,T,T, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, T,T,x,x, x,x,x,x, x,x,x,T, x,x,T,T, T,T,T,x, T,x,x,x, x,x,x,x, T,T,x,x, T,x,T,x, x,x,T,T, T,x,x,x, x,x,T,x, x,x,x,x, T,x,x,T, T,x,x,T, T,x,x,x, x,x,x,x, T,T,T,x, x,x,T,x, x,x,x,T, T,x,x,T, x,T,x,x, x,T,x,T, T,T,T,x, T,T,T,T, T,T,x,T, x,x,x,x, x,x,x,x, T,T,x,x, T,x,x,x, x,x,T,T, x,T,T,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,T, x,T,x,T, T,x,x,x, x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, T,T,T,x, T,x,T,x, T,T,x,T, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,T,T,T, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,x, T,x,x,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,T, x,x,x,T, x,x,T,x, x,x,x,x, T,x,x,x, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,T, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, T,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,T,T, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,T,T,T, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,x, T,x,x,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, T,x,x,x, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,T, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,T,x, x,x,x,x, x,x,x,x, T,x,x,x, T,x,x,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, T,T,T,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, T,T,T,x, T,x,T,T, T,T,x,T, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,T,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, T,T,T,x, T,x,T,T, T,T,x,T, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
	{x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, T,x,x,T, T,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,T, x,T,T,T, T,T,x,T, T,x,x,x, x,T,T,T, T,T,T,T, T,x,T,T, T,x,x,T, T,T,T,T, T,T,T,T, T,T,x,x, x,T,x,T, T,x,T,x, x,x,x,x, x,T,x,x, x,x,T,T, x,x,x,T, x,x,T,T, x,x,T,x, T,x,x,x, T,x,T,x, T,x,x,T, T,x,x,x, T,x,T,x, x,x,x,T, T,x,x,T, x,T,T,x, x,T,x,T, x,x,x,x, x,T,x,T, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,T,T, T,x,x,x, x,T,x,x, x,T,T,x, x,x,T,x, T,T,T,T, T,T,T,x, x,x,x,T, x,x,x,x, x,x,x,x, x},
	{x,T,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,x, x,x,x,x, T,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,T, x,x,x,T, x,T,T,T, T,T,x,T, T,x,T,x, x,T,T,T, T,T,T,T, T,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,x, x,T,T,T, T,T,T,x, x,x,x,x, x,T,T,T, x,T,T,T, x,T,x,T, x,x,T,T, x,T,T,x, T,x,x,x, T,x,T,x, T,x,x,T, T,x,x,x, T,x,T,x, x,x,x,T, T,x,x,T, x,x,T,x, x,T,x,T, T,x,x,x, x,T,x,T, x,x,x,x, x,T,x,x, x,x,T,x, T,x,T,T, T,T,T,x, x,x,T,T, T,T,x,T, x,T,x,x, T,T,T,x, T,x,T,T, T,T,T,T, T,T,T,T, x,x,x,T, T,x,T,x, x,x,x,x, x},
	{x,T,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,T,T,T, T,T,T,T, x,x,T,T, T,T,x,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, x,T,T,T, x,T,T,T, T,T,T,T, T,T,x,T, T,x,T,x, x,T,T,T, T,T,T,T, T,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,x, x,T,T,T, T,T,T,x, T,x,T,x, x,T,T,T, x,T,T,T, x,T,x,T, x,x,T,T, x,T,T,x, T,T,x,x, T,x,T,x, T,T,T,T, T,T,x,T, T,x,T,T, x,x,x,T, T,x,x,T, x,x,T,x, x,T,x,T, T,x,x,x, T,T,T,T, x,x,x,x, x,T,x,x, x,x,T,x, T,x,T,T, T,T,T,x, x,x,T,T, T,T,T,T, x,T,x,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, x,x,x,T, T,x,T,x, x,T,x,x, x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,T,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,T,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
	{x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, T,x,x,T, T,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,T, x,T,T,T, T,T,x,T, T,x,x,x, x,T,T,T, T,T,T,T, T,x,T,T, T,x,x,T, T,T,T,T, T,T,T,T, T,T,x,x, x,T,x,T, T,x,T,x, x,x,x,x, x,T,x,x, x,x,T,T, x,x,x,T, T,x,T,T, x,x,T,x, T,x,x,x, T,x,T,x, T,x,x,T, T,x,x,x, T,x,T,x, x,x,x,T, T,x,x,T, x,T,T,x, x,T,x,T, x,x,x,x, x,T,x,T, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,T,T, T,x,x,x, x,T,x,T, x,T,T,x, x,x,T,x, T,T,T,T, T,T,T,x, x,x,x,T, x,x,x,x, x,x,x,x, x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x},
	{x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,T, x,T,T,T, T,T,x,T, T,x,x,x, x,T,T,T, T,T,T,T, T,x,T,T, T,x,x,T, T,T,T,T, T,T,T,T, T,T,x,x, x,T,x,T, T,x,T,x, x,x,x,x, x,T,x,x, x,x,T,T, x,x,x,T, x,x,T,T, x,x,T,x, T,x,x,x, T,x,T,x, T,x,x,T, T,x,x,x, T,x,T,x, x,x,x,T, T,x,x,T, x,x,T,x, x,T,x,T, x,x,x,x, x,T,x,T, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,T,T, T,x,x,x, x,T,x,x, x,T,T,x, x,x,T,x, T,T,T,T, T,T,T,x, x,x,x,T, x,x,x,x, x,x,x,x, x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,T,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
	{x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, T,x,x,T, T,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,T, x,T,T,T, T,T,x,T, T,x,x,x, x,T,T,T, T,T,T,T, T,x,T,T, T,x,x,T, T,T,T,T, T,T,T,T, T,T,x,x, x,T,x,T, T,x,T,x, x,x,x,x, x,T,x,x, x,x,T,T, x,x,x,T, x,x,T,T, x,x,T,x, T,x,x,x, T,x,T,x, T,x,x,T, T,x,x,x, T,x,T,x, x,x,x,T, T,x,x,T, x,x,T,x, x,T,x,T, x,x,x,x, x,T,x,T, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,T,T, T,x,x,x, x,T,x,x, x,T,T,x, x,x,T,x, T,T,T,T, T,T,T,x, x,x,x,T, x,x,x,x, x,x,x,x, x},
	{x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, T,x,x,T, T,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,T, x,x,x,T, x,T,T,T, T,T,x,T, T,x,T,x, x,T,T,T, T,T,T,T, T,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,x, x,T,T,T, T,T,T,x, x,x,x,x, x,T,T,T, x,T,T,T, x,T,x,T, T,x,T,T, x,T,T,x, T,x,x,x, T,x,T,x, T,x,x,T, T,x,x,x, T,x,T,x, x,x,x,T, T,x,x,T, x,T,T,x, x,T,x,T, T,x,x,x, x,T,x,T, x,x,x,x, x,T,x,x, x,x,T,x, T,x,T,T, T,T,T,x, x,x,T,T, T,T,x,T, x,T,x,T, T,T,T,x, T,x,T,T, T,T,T,T, T,T,T,T, x,x,x,T, T,x,T,x, x,x,x,x, x},
	{x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,x,x,T, T,x,x,T, T,x,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,T, x,T,T,T, T,T,x,T, T,x,x,x, x,T,T,T, T,T,T,T, T,x,T,T, T,x,x,T, T,T,T,T, T,T,T,T, T,T,x,x, x,T,x,T, T,x,T,x, x,x,x,x, x,T,x,x, x,x,T,T, x,x,x,T, T,x,T,T, x,x,T,x, T,x,x,x, T,x,T,x, T,x,x,T, T,x,x,x, T,x,T,x, x,x,x,T, T,x,x,T, x,T,T,x, x,T,x,T, x,x,x,x, x,T,x,T, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,T,T, T,x,x,x, x,T,x,T, x,T,T,x, x,x,T,x, T,T,T,T, T,T,T,x, x,x,x,T, x,x,x,x, x,x,x,x, x},
	{x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,T, x,T,T,T, T,T,x,T, T,x,x,x, x,T,T,T, T,T,T,T, T,x,T,T, T,x,x,T, T,T,T,T, T,T,T,T, T,T,x,x, x,T,x,T, T,x,T,x, x,x,x,x, x,T,x,x, x,x,T,T, x,x,x,T, x,x,T,T, x,x,T,x, T,x,x,x, T,x,T,x, T,x,x,T, T,x,x,x, T,x,T,x, x,x,x,T, T,x,x,T, x,x,T,x, x,T,x,T, x,x,x,x, x,T,x,T, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,T,T, T,x,x,x, x,T,x,x, x,T,T,x, x,x,T,x, T,T,T,T, T,T,T,x, x,x,x,T, x,x,x,x, x,x,x,x, x},
	{x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,T, x,x,x,T, x,T,T,T, T,T,x,T, T,x,T,x, x,T,T,T, T,T,T,T, T,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,x, x,T,T,T, T,T,T,x, x,x,T,x, x,T,T,T, x,T,T,T, x,T,x,T, x,x,T,T, x,T,T,x, T,x,x,x, T,x,T,x, T,x,x,T, T,x,x,x, T,x,T,x, x,x,x,T, T,x,x,T, x,x,T,x, x,T,x,T, T,x,x,x, x,T,x,T, x,x,x,x, x,T,x,x, x,x,T,x, T,x,T,T, T,T,T,x, x,x,T,T, T,T,x,T, x,T,x,x, T,T,T,x, T,x,T,T, T,T,T,T, T,T,T,T, x,x,x,T, T,x,T,x, x,x,x,x, x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,T,T, T,T,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
	{x,T,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,T,T,T, T,T,T,T, x,x,T,T, T,T,x,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, x,T,T,T, x,T,T,T, x,T,T,T, T,T,x,T, T,x,T,x, x,T,T,T, T,T,T,T, T,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,x, x,T,T,T, T,T,T,x, T,x,T,x, x,T,T,T, x,T,T,T, x,T,x,T, x,x,T,T, x,T,T,x, T,T,x,x, T,x,T,x, T,T,T,T, T,T,x,T, T,x,T,T, x,x,x,T, T,x,x,T, x,x,T,x, x,T,x,T, T,x,x,x, T,T,T,T, x,x,x,x, x,T,x,x, x,x,T,x, T,x,T,T, T,T,T,x, x,x,T,T, T,T,T,T, x,T,x,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, x,x,x,T, T,x,T,x, x,T,x,x, x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,T,T,T, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,T,x, T,x,x,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, T,x,x,x, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,T, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
	{x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,T, x,x,x,T, x,T,T,T, T,T,x,T, T,x,T,x, x,T,T,T, T,T,T,T, T,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,x, x,T,T,T, T,T,T,x, x,x,x,x, x,T,T,T, x,T,T,T, x,T,x,T, x,x,T,T, x,T,T,x, T,x,x,x, T,x,T,x, T,x,x,T, T,x,x,x, T,x,T,x, x,x,x,T, T,x,x,T, x,x,T,x, x,T,x,T, T,x,x,x, x,T,x,T, x,x,x,x, x,T,x,x, x,x,T,x, T,x,T,T, T,T,T,x, x,x,T,T, T,T,x,T, x,T,x,x, T,T,T,x, T,x,T,T, T,T,T,T, T,T,T,T, x,x,x,T, T,x,T,x, x,x,x,x, x},
	{x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,T,T,T, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,x, T,x,x,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, T,x,x,x, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,T, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,T,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x}

	};
} // end Parser

}