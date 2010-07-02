
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
		
		while (la.kind == 1 || la.kind == 21) {
			EndOfStmt();
		}
		while (la.kind == 173) {
			OptionStmt();
			while (la.kind == 1 || la.kind == 21) {
				EndOfStmt();
			}
		}
		while (la.kind == 137) {
			ImportsStmt();
			while (la.kind == 1 || la.kind == 21) {
				EndOfStmt();
			}
		}
		while (
#line  270 "VBNET.ATG" 
IsGlobalAttrTarget()) {
			GlobalAttributeSection();
			while (la.kind == 1 || la.kind == 21) {
				EndOfStmt();
			}
		}
		while (StartOf(1)) {
			NamespaceMemberDecl();
			while (la.kind == 1 || la.kind == 21) {
				EndOfStmt();
			}
		}
		Expect(0);
	}

	void EndOfStmt() {
		if (la.kind == 1) {
			lexer.NextToken();
		} else if (la.kind == 21) {
			lexer.NextToken();
		} else SynErr(239);
	}

	void OptionStmt() {

#line  275 "VBNET.ATG" 
		INode node = null; bool val = true; 
		Expect(173);

#line  276 "VBNET.ATG" 
		Location startPos = t.Location; 
		if (la.kind == 121) {
			lexer.NextToken();
			if (la.kind == 170 || la.kind == 171) {
				OptionValue(
#line  278 "VBNET.ATG" 
ref val);
			}

#line  279 "VBNET.ATG" 
			node = new OptionDeclaration(OptionType.Explicit, val); 
		} else if (la.kind == 207) {
			lexer.NextToken();
			if (la.kind == 170 || la.kind == 171) {
				OptionValue(
#line  281 "VBNET.ATG" 
ref val);
			}

#line  282 "VBNET.ATG" 
			node = new OptionDeclaration(OptionType.Strict, val); 
		} else if (la.kind == 87) {
			lexer.NextToken();
			if (la.kind == 67) {
				lexer.NextToken();

#line  284 "VBNET.ATG" 
				node = new OptionDeclaration(OptionType.CompareBinary, val); 
			} else if (la.kind == 213) {
				lexer.NextToken();

#line  285 "VBNET.ATG" 
				node = new OptionDeclaration(OptionType.CompareText, val); 
			} else SynErr(240);
		} else if (la.kind == 139) {
			lexer.NextToken();
			if (la.kind == 170 || la.kind == 171) {
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
		
		Expect(137);

#line  320 "VBNET.ATG" 
		Location startPos = t.Location;
		Using u;
		
		ImportClause(
#line  323 "VBNET.ATG" 
out u);

#line  323 "VBNET.ATG" 
		if (u != null) { usings.Add(u); } 
		while (la.kind == 22) {
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
		Expect(40);

#line  2739 "VBNET.ATG" 
		Location startPos = t.Location; 
		if (la.kind == 65) {
			lexer.NextToken();
		} else if (la.kind == 155) {
			lexer.NextToken();
		} else SynErr(242);

#line  2741 "VBNET.ATG" 
		string attributeTarget = t.val != null ? t.val.ToLower(System.Globalization.CultureInfo.InvariantCulture) : null;
		List<ASTAttribute> attributes = new List<ASTAttribute>();
		ASTAttribute attribute;
		
		Expect(21);
		Attribute(
#line  2745 "VBNET.ATG" 
out attribute);

#line  2745 "VBNET.ATG" 
		attributes.Add(attribute); 
		while (
#line  2746 "VBNET.ATG" 
NotFinalComma()) {
			if (la.kind == 22) {
				lexer.NextToken();
				if (la.kind == 65) {
					lexer.NextToken();
				} else if (la.kind == 155) {
					lexer.NextToken();
				} else SynErr(243);
				Expect(21);
			}
			Attribute(
#line  2746 "VBNET.ATG" 
out attribute);

#line  2746 "VBNET.ATG" 
			attributes.Add(attribute); 
		}
		if (la.kind == 22) {
			lexer.NextToken();
		}
		Expect(39);
		EndOfStmt();

#line  2751 "VBNET.ATG" 
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
		
		if (la.kind == 160) {
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
			while (la.kind == 40) {
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
		if (la.kind == 171) {
			lexer.NextToken();

#line  303 "VBNET.ATG" 
			val = true; 
		} else if (la.kind == 170) {
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
			if (la.kind == 20) {
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
			Expect(20);
			Expect(3);

#line  355 "VBNET.ATG" 
			u = new Using(t.literalValue as string, prefix); 
			Expect(11);
		} else SynErr(246);
	}

	void Qualident(
#line  3495 "VBNET.ATG" 
out string qualident) {

#line  3497 "VBNET.ATG" 
		string name;
		qualidentBuilder.Length = 0; 
		
		Identifier();

#line  3501 "VBNET.ATG" 
		qualidentBuilder.Append(t.val); 
		while (
#line  3502 "VBNET.ATG" 
DotAndIdentOrKw()) {
			Expect(26);
			IdentifierOrKeyword(
#line  3502 "VBNET.ATG" 
out name);

#line  3502 "VBNET.ATG" 
			qualidentBuilder.Append('.'); qualidentBuilder.Append(name); 
		}

#line  3504 "VBNET.ATG" 
		qualident = qualidentBuilder.ToString(); 
	}

	void TypeName(
#line  2612 "VBNET.ATG" 
out TypeReference typeref) {

#line  2613 "VBNET.ATG" 
		ArrayList rank = null; 
		NonArrayTypeName(
#line  2615 "VBNET.ATG" 
out typeref, false);
		ArrayTypeModifiers(
#line  2619 "VBNET.ATG" 
out rank);

#line  2620 "VBNET.ATG" 
		if (rank != null && typeref != null) {
		typeref.RankSpecifier = (int[])rank.ToArray(typeof(int));
		}
		
	}

	void Identifier() {
		if (StartOf(5)) {
			IdentifierForFieldDeclaration();
		} else if (la.kind == 98) {
			lexer.NextToken();
		} else SynErr(247);
	}

	void NamespaceBody() {
		while (la.kind == 1 || la.kind == 21) {
			EndOfStmt();
		}
		while (StartOf(1)) {
			NamespaceMemberDecl();
			while (la.kind == 1 || la.kind == 21) {
				EndOfStmt();
			}
		}
		Expect(113);
		Expect(160);
		EndOfStmt();
	}

	void AttributeSection(
#line  2814 "VBNET.ATG" 
out AttributeSection section) {

#line  2816 "VBNET.ATG" 
		string attributeTarget = "";List<ASTAttribute> attributes = new List<ASTAttribute>();
		ASTAttribute attribute;
		
		
		Expect(40);

#line  2820 "VBNET.ATG" 
		Location startPos = t.Location; 
		if (
#line  2821 "VBNET.ATG" 
IsLocalAttrTarget()) {
			if (la.kind == 119) {
				lexer.NextToken();

#line  2822 "VBNET.ATG" 
				attributeTarget = "event";
			} else if (la.kind == 195) {
				lexer.NextToken();

#line  2823 "VBNET.ATG" 
				attributeTarget = "return";
			} else {
				Identifier();

#line  2826 "VBNET.ATG" 
				string val = t.val.ToLower(System.Globalization.CultureInfo.InvariantCulture);
				if (val != "field"	|| val != "method" ||
					val != "module" || val != "param"  ||
					val != "property" || val != "type")
				Error("attribute target specifier (event, return, field," +
						"method, module, param, property, or type) expected");
				attributeTarget = t.val;
				
			}
			Expect(21);
		}
		Attribute(
#line  2836 "VBNET.ATG" 
out attribute);

#line  2836 "VBNET.ATG" 
		attributes.Add(attribute); 
		while (
#line  2837 "VBNET.ATG" 
NotFinalComma()) {
			Expect(22);
			Attribute(
#line  2837 "VBNET.ATG" 
out attribute);

#line  2837 "VBNET.ATG" 
			attributes.Add(attribute); 
		}
		if (la.kind == 22) {
			lexer.NextToken();
		}
		Expect(39);

#line  2841 "VBNET.ATG" 
		section = new AttributeSection {
		AttributeTarget = attributeTarget,
		Attributes = attributes,
		StartLocation = startPos,
		EndLocation = t.EndLocation
		};
		
	}

	void TypeModifier(
#line  3580 "VBNET.ATG" 
ModifierList m) {
		switch (la.kind) {
		case 188: {
			lexer.NextToken();

#line  3581 "VBNET.ATG" 
			m.Add(Modifiers.Public, t.Location); 
			break;
		}
		case 187: {
			lexer.NextToken();

#line  3582 "VBNET.ATG" 
			m.Add(Modifiers.Protected, t.Location); 
			break;
		}
		case 125: {
			lexer.NextToken();

#line  3583 "VBNET.ATG" 
			m.Add(Modifiers.Internal, t.Location); 
			break;
		}
		case 185: {
			lexer.NextToken();

#line  3584 "VBNET.ATG" 
			m.Add(Modifiers.Private, t.Location); 
			break;
		}
		case 200: {
			lexer.NextToken();

#line  3585 "VBNET.ATG" 
			m.Add(Modifiers.Static, t.Location); 
			break;
		}
		case 199: {
			lexer.NextToken();

#line  3586 "VBNET.ATG" 
			m.Add(Modifiers.New, t.Location); 
			break;
		}
		case 156: {
			lexer.NextToken();

#line  3587 "VBNET.ATG" 
			m.Add(Modifiers.Abstract, t.Location); 
			break;
		}
		case 166: {
			lexer.NextToken();

#line  3588 "VBNET.ATG" 
			m.Add(Modifiers.Sealed, t.Location); 
			break;
		}
		case 183: {
			lexer.NextToken();

#line  3589 "VBNET.ATG" 
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
		case 84: {

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
			if (la.kind == 140) {
				ClassBaseType(
#line  465 "VBNET.ATG" 
out typeRef);

#line  465 "VBNET.ATG" 
				SafeAdd(newType, newType.BaseTypes, typeRef); 
			}
			while (la.kind == 136) {
				TypeImplementsClause(
#line  466 "VBNET.ATG" 
out baseInterfaces);

#line  466 "VBNET.ATG" 
				newType.BaseTypes.AddRange(baseInterfaces); 
			}
			ClassBody(
#line  467 "VBNET.ATG" 
newType);
			Expect(113);
			Expect(84);

#line  468 "VBNET.ATG" 
			newType.EndLocation = t.EndLocation; 
			EndOfStmt();

#line  471 "VBNET.ATG" 
			BlockEnd();
			
			break;
		}
		case 155: {
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
			while (la.kind == 136) {
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
		case 115: {
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
			if (la.kind == 63) {
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
		case 142: {
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
			while (la.kind == 140) {
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
		case 103: {
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
				if (la.kind == 37) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
#line  557 "VBNET.ATG" 
p);
					}
					Expect(38);

#line  557 "VBNET.ATG" 
					delegateDeclr.Parameters = p; 
				}
			} else if (la.kind == 127) {
				lexer.NextToken();
				Identifier();

#line  559 "VBNET.ATG" 
				delegateDeclr.Name = t.val; 
				TypeParameterList(
#line  560 "VBNET.ATG" 
delegateDeclr.Templates);
				if (la.kind == 37) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
#line  561 "VBNET.ATG" 
p);
					}
					Expect(38);

#line  561 "VBNET.ATG" 
					delegateDeclr.Parameters = p; 
				}
				if (la.kind == 63) {
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
			Expect(169);
			TypeParameter(
#line  397 "VBNET.ATG" 
out template);

#line  399 "VBNET.ATG" 
			if (template != null) templates.Add(template);
			
			while (la.kind == 22) {
				lexer.NextToken();
				TypeParameter(
#line  402 "VBNET.ATG" 
out template);

#line  404 "VBNET.ATG" 
				if (template != null) templates.Add(template);
				
			}
			Expect(38);
		}
	}

	void TypeParameter(
#line  412 "VBNET.ATG" 
out TemplateDefinition template) {
		Identifier();

#line  414 "VBNET.ATG" 
		template = new TemplateDefinition(t.val, null); 
		if (la.kind == 63) {
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
		
		Expect(63);
		if (la.kind == 35) {
			lexer.NextToken();
			TypeParameterConstraint(
#line  427 "VBNET.ATG" 
out constraint);

#line  427 "VBNET.ATG" 
			if (constraint != null) { template.Bases.Add(constraint); } 
			while (la.kind == 22) {
				lexer.NextToken();
				TypeParameterConstraint(
#line  430 "VBNET.ATG" 
out constraint);

#line  430 "VBNET.ATG" 
				if (constraint != null) { template.Bases.Add(constraint); } 
			}
			Expect(36);
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
		if (la.kind == 84) {
			lexer.NextToken();

#line  439 "VBNET.ATG" 
			constraint = TypeReference.ClassConstraint; 
		} else if (la.kind == 209) {
			lexer.NextToken();

#line  440 "VBNET.ATG" 
			constraint = TypeReference.StructConstraint; 
		} else if (la.kind == 162) {
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
		
		Expect(140);
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
		
		Expect(136);
		TypeName(
#line  1609 "VBNET.ATG" 
out type);

#line  1611 "VBNET.ATG" 
		if (type != null) baseInterfaces.Add(type);
		
		while (la.kind == 22) {
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
		while (la.kind == 1 || la.kind == 21) {
			EndOfStmt();
		}
		while (StartOf(9)) {

#line  585 "VBNET.ATG" 
			List<AttributeSection> attributes = new List<AttributeSection>();
			ModifierList m = new ModifierList();
			
			while (la.kind == 40) {
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
			while (la.kind == 1 || la.kind == 21) {
				EndOfStmt();
			}
		}
	}

	void ModuleBody(
#line  612 "VBNET.ATG" 
TypeDeclaration newType) {

#line  613 "VBNET.ATG" 
		AttributeSection section; 
		while (la.kind == 1 || la.kind == 21) {
			EndOfStmt();
		}
		while (StartOf(9)) {

#line  616 "VBNET.ATG" 
			List<AttributeSection> attributes = new List<AttributeSection>();
			ModifierList m = new ModifierList();
			
			while (la.kind == 40) {
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
			while (la.kind == 1 || la.kind == 21) {
				EndOfStmt();
			}
		}
		Expect(113);
		Expect(155);

#line  624 "VBNET.ATG" 
		newType.EndLocation = t.EndLocation; 
		EndOfStmt();
	}

	void StructureBody(
#line  595 "VBNET.ATG" 
TypeDeclaration newType) {

#line  596 "VBNET.ATG" 
		AttributeSection section; 
		while (la.kind == 1 || la.kind == 21) {
			EndOfStmt();
		}
		while (StartOf(9)) {

#line  599 "VBNET.ATG" 
			List<AttributeSection> attributes = new List<AttributeSection>();
			ModifierList m = new ModifierList();
			
			while (la.kind == 40) {
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
			while (la.kind == 1 || la.kind == 21) {
				EndOfStmt();
			}
		}
		Expect(113);
		Expect(209);

#line  607 "VBNET.ATG" 
		newType.EndLocation = t.EndLocation; 
		EndOfStmt();
	}

	void NonArrayTypeName(
#line  2638 "VBNET.ATG" 
out TypeReference typeref, bool canBeUnbound) {

#line  2640 "VBNET.ATG" 
		string name;
		typeref = null;
		bool isGlobal = false;
		
		if (StartOf(11)) {
			if (la.kind == 130) {
				lexer.NextToken();
				Expect(26);

#line  2645 "VBNET.ATG" 
				isGlobal = true; 
			}
			QualIdentAndTypeArguments(
#line  2646 "VBNET.ATG" 
out typeref, canBeUnbound);

#line  2647 "VBNET.ATG" 
			typeref.IsGlobal = isGlobal; 
			while (la.kind == 26) {
				lexer.NextToken();

#line  2648 "VBNET.ATG" 
				TypeReference nestedTypeRef; 
				QualIdentAndTypeArguments(
#line  2649 "VBNET.ATG" 
out nestedTypeRef, canBeUnbound);

#line  2650 "VBNET.ATG" 
				typeref = new InnerClassTypeReference(typeref, nestedTypeRef.Type, nestedTypeRef.GenericTypes); 
			}
		} else if (la.kind == 168) {
			lexer.NextToken();

#line  2653 "VBNET.ATG" 
			typeref = new TypeReference("System.Object", true); 
			if (la.kind == 33) {
				lexer.NextToken();

#line  2657 "VBNET.ATG" 
				List<TypeReference> typeArguments = new List<TypeReference>(1);
				if (typeref != null) typeArguments.Add(typeref);
				typeref = new TypeReference("System.Nullable", typeArguments) { IsKeyword = true };
				
			}
		} else if (StartOf(12)) {
			PrimitiveTypeName(
#line  2663 "VBNET.ATG" 
out name);

#line  2663 "VBNET.ATG" 
			typeref = new TypeReference(name, true); 
			if (la.kind == 33) {
				lexer.NextToken();

#line  2667 "VBNET.ATG" 
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
		while (la.kind == 1 || la.kind == 21) {
			EndOfStmt();
		}
		while (StartOf(13)) {
			EnumMemberDecl(
#line  632 "VBNET.ATG" 
out f);

#line  634 "VBNET.ATG" 
			AddChild(f);
			
			while (la.kind == 1 || la.kind == 21) {
				EndOfStmt();
			}
		}
		Expect(113);
		Expect(115);

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
		
		Expect(140);
		TypeName(
#line  1595 "VBNET.ATG" 
out type);

#line  1595 "VBNET.ATG" 
		if (type != null) bases.Add(type); 
		while (la.kind == 22) {
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
		while (la.kind == 1 || la.kind == 21) {
			EndOfStmt();
		}
		while (StartOf(14)) {
			InterfaceMemberDecl();
			while (la.kind == 1 || la.kind == 21) {
				EndOfStmt();
			}
		}
		Expect(113);
		Expect(142);

#line  648 "VBNET.ATG" 
		newType.EndLocation = t.EndLocation; 
		EndOfStmt();
	}

	void FormalParameterList(
#line  2851 "VBNET.ATG" 
List<ParameterDeclarationExpression> parameter) {

#line  2852 "VBNET.ATG" 
		ParameterDeclarationExpression p; 
		FormalParameter(
#line  2854 "VBNET.ATG" 
out p);

#line  2854 "VBNET.ATG" 
		if (p != null) parameter.Add(p); 
		while (la.kind == 22) {
			lexer.NextToken();
			FormalParameter(
#line  2856 "VBNET.ATG" 
out p);

#line  2856 "VBNET.ATG" 
			if (p != null) parameter.Add(p); 
		}
	}

	void MemberModifier(
#line  3592 "VBNET.ATG" 
ModifierList m) {
		switch (la.kind) {
		case 156: {
			lexer.NextToken();

#line  3593 "VBNET.ATG" 
			m.Add(Modifiers.Abstract, t.Location);
			break;
		}
		case 102: {
			lexer.NextToken();

#line  3594 "VBNET.ATG" 
			m.Add(Modifiers.Default, t.Location);
			break;
		}
		case 125: {
			lexer.NextToken();

#line  3595 "VBNET.ATG" 
			m.Add(Modifiers.Internal, t.Location);
			break;
		}
		case 199: {
			lexer.NextToken();

#line  3596 "VBNET.ATG" 
			m.Add(Modifiers.New, t.Location);
			break;
		}
		case 181: {
			lexer.NextToken();

#line  3597 "VBNET.ATG" 
			m.Add(Modifiers.Override, t.Location);
			break;
		}
		case 157: {
			lexer.NextToken();

#line  3598 "VBNET.ATG" 
			m.Add(Modifiers.Abstract, t.Location);
			break;
		}
		case 185: {
			lexer.NextToken();

#line  3599 "VBNET.ATG" 
			m.Add(Modifiers.Private, t.Location);
			break;
		}
		case 187: {
			lexer.NextToken();

#line  3600 "VBNET.ATG" 
			m.Add(Modifiers.Protected, t.Location);
			break;
		}
		case 188: {
			lexer.NextToken();

#line  3601 "VBNET.ATG" 
			m.Add(Modifiers.Public, t.Location);
			break;
		}
		case 166: {
			lexer.NextToken();

#line  3602 "VBNET.ATG" 
			m.Add(Modifiers.Sealed, t.Location);
			break;
		}
		case 167: {
			lexer.NextToken();

#line  3603 "VBNET.ATG" 
			m.Add(Modifiers.Sealed, t.Location);
			break;
		}
		case 200: {
			lexer.NextToken();

#line  3604 "VBNET.ATG" 
			m.Add(Modifiers.Static, t.Location);
			break;
		}
		case 180: {
			lexer.NextToken();

#line  3605 "VBNET.ATG" 
			m.Add(Modifiers.Virtual, t.Location);
			break;
		}
		case 179: {
			lexer.NextToken();

#line  3606 "VBNET.ATG" 
			m.Add(Modifiers.Overloads, t.Location);
			break;
		}
		case 190: {
			lexer.NextToken();

#line  3607 "VBNET.ATG" 
			m.Add(Modifiers.ReadOnly, t.Location);
			break;
		}
		case 235: {
			lexer.NextToken();

#line  3608 "VBNET.ATG" 
			m.Add(Modifiers.WriteOnly, t.Location);
			break;
		}
		case 234: {
			lexer.NextToken();

#line  3609 "VBNET.ATG" 
			m.Add(Modifiers.WithEvents, t.Location);
			break;
		}
		case 105: {
			lexer.NextToken();

#line  3610 "VBNET.ATG" 
			m.Add(Modifiers.Dim, t.Location);
			break;
		}
		case 183: {
			lexer.NextToken();

#line  3611 "VBNET.ATG" 
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
		case 84: case 103: case 115: case 142: case 155: case 209: {
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
				if (la.kind == 37) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
#line  824 "VBNET.ATG" 
p);
					}
					Expect(38);
				}
				if (la.kind == 134 || la.kind == 136) {
					if (la.kind == 136) {
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
					Expect(113);
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
			} else if (la.kind == 162) {
				lexer.NextToken();
				if (la.kind == 37) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
#line  876 "VBNET.ATG" 
p);
					}
					Expect(38);
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
				Expect(113);
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
		case 127: {
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
			if (la.kind == 37) {
				lexer.NextToken();
				if (StartOf(6)) {
					FormalParameterList(
#line  914 "VBNET.ATG" 
p);
				}
				Expect(38);
			}
			if (la.kind == 63) {
				lexer.NextToken();
				while (la.kind == 40) {
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
			
			if (la.kind == 134 || la.kind == 136) {
				if (la.kind == 136) {
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
				Expect(113);
				Expect(127);

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
		case 101: {
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
				Expect(149);
				Expect(3);

#line  998 "VBNET.ATG" 
				library = t.literalValue as string; 
				if (la.kind == 59) {
					lexer.NextToken();
					Expect(3);

#line  999 "VBNET.ATG" 
					alias = t.literalValue as string; 
				}
				if (la.kind == 37) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
#line  1000 "VBNET.ATG" 
p);
					}
					Expect(38);
				}
				EndOfStmt();

#line  1003 "VBNET.ATG" 
				DeclareDeclaration declareDeclaration = new DeclareDeclaration(name, m.Modifier, null, p, attributes, library, alias, charsetModifer);
				declareDeclaration.StartLocation = m.GetDeclarationLocation(startPos);
				declareDeclaration.EndLocation   = t.EndLocation;
				AddChild(declareDeclaration);
				
			} else if (la.kind == 127) {
				lexer.NextToken();
				Identifier();

#line  1010 "VBNET.ATG" 
				name = t.val; 
				Expect(149);
				Expect(3);

#line  1011 "VBNET.ATG" 
				library = t.literalValue as string; 
				if (la.kind == 59) {
					lexer.NextToken();
					Expect(3);

#line  1012 "VBNET.ATG" 
					alias = t.literalValue as string; 
				}
				if (la.kind == 37) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
#line  1013 "VBNET.ATG" 
p);
					}
					Expect(38);
				}
				if (la.kind == 63) {
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
		case 119: {
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
			if (la.kind == 63) {
				lexer.NextToken();
				TypeName(
#line  1035 "VBNET.ATG" 
out type);
			} else if (StartOf(16)) {
				if (la.kind == 37) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
#line  1037 "VBNET.ATG" 
p);
					}
					Expect(38);
				}
			} else SynErr(259);
			if (la.kind == 136) {
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
		case 2: case 58: case 62: case 64: case 65: case 66: case 67: case 70: case 87: case 104: case 107: case 116: case 121: case 126: case 133: case 139: case 143: case 146: case 147: case 170: case 176: case 178: case 184: case 203: case 212: case 213: case 223: case 224: case 230: {

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
			while (la.kind == 22) {
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
		case 88: {

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
			while (la.kind == 22) {
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
			if (la.kind == 37) {
				lexer.NextToken();
				if (StartOf(6)) {
					FormalParameterList(
#line  1095 "VBNET.ATG" 
p);
				}
				Expect(38);
			}
			if (la.kind == 63) {
				lexer.NextToken();
				while (la.kind == 40) {
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
			if (la.kind == 20) {
				lexer.NextToken();
				Expr(
#line  1119 "VBNET.ATG" 
out initializer);
			}
			if (la.kind == 136) {
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
				Expect(113);
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
		case 98: {
			lexer.NextToken();

#line  1159 "VBNET.ATG" 
			Location startPos = t.Location; 
			Expect(119);

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
			Expect(63);
			TypeName(
#line  1169 "VBNET.ATG" 
out type);
			if (la.kind == 136) {
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
			Expect(113);
			Expect(119);
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
		case 161: case 172: case 232: {

#line  1217 "VBNET.ATG" 
			ConversionType opConversionType = ConversionType.None; 
			if (la.kind == 161 || la.kind == 232) {
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
			Expect(172);

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
			Expect(37);
			if (la.kind == 72) {
				lexer.NextToken();
			}
			Identifier();

#line  1232 "VBNET.ATG" 
			operandName = t.val; 
			if (la.kind == 63) {
				lexer.NextToken();
				TypeName(
#line  1233 "VBNET.ATG" 
out operandType);
			}

#line  1234 "VBNET.ATG" 
			parameters.Add(new ParameterDeclarationExpression(operandType, operandName, ParameterModifiers.In)); 
			while (la.kind == 22) {
				lexer.NextToken();
				if (la.kind == 72) {
					lexer.NextToken();
				}
				Identifier();

#line  1238 "VBNET.ATG" 
				operandName = t.val; 
				if (la.kind == 63) {
					lexer.NextToken();
					TypeName(
#line  1239 "VBNET.ATG" 
out operandType);
				}

#line  1240 "VBNET.ATG" 
				parameters.Add(new ParameterDeclarationExpression(operandType, operandName, ParameterModifiers.In)); 
			}
			Expect(38);

#line  1243 "VBNET.ATG" 
			Location endPos = t.EndLocation; 
			if (la.kind == 63) {
				lexer.NextToken();
				while (la.kind == 40) {
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
			Expect(113);
			Expect(172);
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
		
		while (la.kind == 40) {
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
		
		if (la.kind == 20) {
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
			while (la.kind == 40) {
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
			if (la.kind == 119) {
				lexer.NextToken();

#line  671 "VBNET.ATG" 
				mod.Check(Modifiers.VBInterfaceEvents);
				Location startLocation = t.Location;
				
				Identifier();

#line  674 "VBNET.ATG" 
				name = t.val; 
				if (la.kind == 37) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
#line  675 "VBNET.ATG" 
p);
					}
					Expect(38);
				}
				if (la.kind == 63) {
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
				if (la.kind == 37) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
#line  694 "VBNET.ATG" 
p);
					}
					Expect(38);
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
				
			} else if (la.kind == 127) {
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
				if (la.kind == 37) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
#line  717 "VBNET.ATG" 
p);
					}
					Expect(38);
				}
				if (la.kind == 63) {
					lexer.NextToken();
					while (la.kind == 40) {
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
				if (la.kind == 37) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
#line  744 "VBNET.ATG" 
p);
					}
					Expect(38);
				}
				if (la.kind == 63) {
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
		} else if (la.kind == 127 || la.kind == 210) {
			LambdaExpr(
#line  1652 "VBNET.ATG" 
out expr);
		} else if (StartOf(21)) {
			DisjunctionExpr(
#line  1653 "VBNET.ATG" 
out expr);
		} else if (StartOf(22)) {
			XmlLiteralExpression(
#line  1654 "VBNET.ATG" 
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
		
		Expect(136);
		NonArrayTypeName(
#line  1628 "VBNET.ATG" 
out type, false);

#line  1629 "VBNET.ATG" 
		if (type != null) memberName = TypeReference.StripLastIdentifierFromType(ref type); 

#line  1630 "VBNET.ATG" 
		baseInterfaces.Add(new InterfaceImplementation(type, memberName)); 
		while (la.kind == 22) {
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
		
		Expect(134);
		EventMemberSpecifier(
#line  1584 "VBNET.ATG" 
out name);

#line  1584 "VBNET.ATG" 
		if (name != null) handlesClause.Add(name); 
		while (la.kind == 22) {
			lexer.NextToken();
			EventMemberSpecifier(
#line  1585 "VBNET.ATG" 
out name);

#line  1585 "VBNET.ATG" 
			if (name != null) handlesClause.Add(name); 
		}
	}

	void Block(
#line  2896 "VBNET.ATG" 
out Statement stmt) {

#line  2899 "VBNET.ATG" 
		BlockStatement blockStmt = new BlockStatement();
		/* in snippet parsing mode, t might be null */
		if (t != null) blockStmt.StartLocation = t.EndLocation;
		BlockStart(blockStmt);
		
		while (StartOf(23) || 
#line  2905 "VBNET.ATG" 
IsEndStmtAhead()) {
			if (
#line  2905 "VBNET.ATG" 
IsEndStmtAhead()) {
				Expect(113);
				EndOfStmt();

#line  2905 "VBNET.ATG" 
				AddChild(new EndStatement()); 
			} else {
				Statement();
				EndOfStmt();
			}
		}

#line  2910 "VBNET.ATG" 
		stmt = blockStmt;
		if (t != null) blockStmt.EndLocation = t.EndLocation;
		BlockEnd();
		
	}

	void Charset(
#line  1571 "VBNET.ATG" 
out CharsetModifier charsetModifier) {

#line  1572 "VBNET.ATG" 
		charsetModifier = CharsetModifier.None; 
		if (la.kind == 127 || la.kind == 210) {
		} else if (la.kind == 62) {
			lexer.NextToken();

#line  1573 "VBNET.ATG" 
			charsetModifier = CharsetModifier.Ansi; 
		} else if (la.kind == 66) {
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
		case 58: {
			lexer.NextToken();
			break;
		}
		case 62: {
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
		case 67: {
			lexer.NextToken();
			break;
		}
		case 70: {
			lexer.NextToken();
			break;
		}
		case 87: {
			lexer.NextToken();
			break;
		}
		case 104: {
			lexer.NextToken();
			break;
		}
		case 107: {
			lexer.NextToken();
			break;
		}
		case 116: {
			lexer.NextToken();
			break;
		}
		case 121: {
			lexer.NextToken();
			break;
		}
		case 126: {
			lexer.NextToken();
			break;
		}
		case 133: {
			lexer.NextToken();
			break;
		}
		case 139: {
			lexer.NextToken();
			break;
		}
		case 143: {
			lexer.NextToken();
			break;
		}
		case 146: {
			lexer.NextToken();
			break;
		}
		case 147: {
			lexer.NextToken();
			break;
		}
		case 170: {
			lexer.NextToken();
			break;
		}
		case 176: {
			lexer.NextToken();
			break;
		}
		case 178: {
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
			Expect(63);
			ObjectCreateExpression(
#line  1467 "VBNET.ATG" 
out expr);

#line  1469 "VBNET.ATG" 
			if (expr is ObjectCreateExpression) {
			type = ((ObjectCreateExpression)expr).CreateType.Clone();
			} else {
				type = ((ArrayCreateExpression)expr).CreateType.Clone();
			}
			
		} else if (StartOf(24)) {
			if (la.kind == 63) {
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
			
			if (la.kind == 20) {
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
		if (la.kind == 63) {
			lexer.NextToken();
			TypeName(
#line  1439 "VBNET.ATG" 
out type);
		}
		Expect(20);
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
#line  2064 "VBNET.ATG" 
out Expression oce) {

#line  2066 "VBNET.ATG" 
		TypeReference type = null;
		CollectionInitializerExpression initializer = null;
		List<Expression> arguments = null;
		ArrayList dimensions = null;
		oce = null;
		bool canBeNormal; bool canBeReDim;
		
		Expect(162);
		if (StartOf(8)) {
			NonArrayTypeName(
#line  2074 "VBNET.ATG" 
out type, false);
			if (la.kind == 37) {
				lexer.NextToken();
				NormalOrReDimArgumentList(
#line  2075 "VBNET.ATG" 
out arguments, out canBeNormal, out canBeReDim);
				Expect(38);
				if (la.kind == 35 || 
#line  2076 "VBNET.ATG" 
la.kind == Tokens.OpenParenthesis) {
					if (
#line  2076 "VBNET.ATG" 
la.kind == Tokens.OpenParenthesis) {
						ArrayTypeModifiers(
#line  2077 "VBNET.ATG" 
out dimensions);
						CollectionInitializer(
#line  2078 "VBNET.ATG" 
out initializer);
					} else {
						CollectionInitializer(
#line  2079 "VBNET.ATG" 
out initializer);
					}
				}

#line  2081 "VBNET.ATG" 
				if (canBeReDim && !canBeNormal && initializer == null) initializer = new CollectionInitializerExpression(); 
			}
		}

#line  2085 "VBNET.ATG" 
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
		
		if (la.kind == 126 || la.kind == 233) {
			if (la.kind == 233) {

#line  2100 "VBNET.ATG" 
				MemberInitializerExpression memberInitializer = null;
				
				lexer.NextToken();

#line  2104 "VBNET.ATG" 
				CollectionInitializerExpression memberInitializers = new CollectionInitializerExpression();
				memberInitializers.StartLocation = la.Location;
				
				Expect(35);
				MemberInitializer(
#line  2108 "VBNET.ATG" 
out memberInitializer);

#line  2109 "VBNET.ATG" 
				memberInitializers.CreateExpressions.Add(memberInitializer); 
				while (la.kind == 22) {
					lexer.NextToken();
					MemberInitializer(
#line  2111 "VBNET.ATG" 
out memberInitializer);

#line  2112 "VBNET.ATG" 
					memberInitializers.CreateExpressions.Add(memberInitializer); 
				}
				Expect(36);

#line  2116 "VBNET.ATG" 
				memberInitializers.EndLocation = t.Location;
				if(oce is ObjectCreateExpression)
				{
					((ObjectCreateExpression)oce).ObjectInitializer = memberInitializers;
				}
				
			} else {
				lexer.NextToken();
				CollectionInitializer(
#line  2126 "VBNET.ATG" 
out initializer);

#line  2128 "VBNET.ATG" 
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
		
		while (la.kind == 40) {
			AttributeSection(
#line  1372 "VBNET.ATG" 
out section);

#line  1372 "VBNET.ATG" 
			attributes.Add(section); 
		}
		if (StartOf(25)) {
			GetAccessorDecl(
#line  1374 "VBNET.ATG" 
out getBlock, attributes);
			if (StartOf(26)) {

#line  1376 "VBNET.ATG" 
				attributes = new List<AttributeSection>(); 
				while (la.kind == 40) {
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
		} else if (StartOf(27)) {
			SetAccessorDecl(
#line  1381 "VBNET.ATG" 
out setBlock, attributes);
			if (StartOf(28)) {

#line  1383 "VBNET.ATG" 
				attributes = new List<AttributeSection>(); 
				while (la.kind == 40) {
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
		
		while (la.kind == 40) {
			AttributeSection(
#line  1336 "VBNET.ATG" 
out section);

#line  1336 "VBNET.ATG" 
			attributes.Add(section); 
		}
		if (la.kind == 56) {
			lexer.NextToken();
			if (la.kind == 37) {
				lexer.NextToken();
				if (StartOf(6)) {
					FormalParameterList(
#line  1338 "VBNET.ATG" 
p);
				}
				Expect(38);
			}
			Expect(1);
			Block(
#line  1339 "VBNET.ATG" 
out stmt);
			Expect(113);
			Expect(56);
			EndOfStmt();

#line  1341 "VBNET.ATG" 
			eventAccessorDeclaration = new EventAddRegion(attributes);
			eventAccessorDeclaration.Block = (BlockStatement)stmt;
			eventAccessorDeclaration.Parameters = p;
			
		} else if (la.kind == 193) {
			lexer.NextToken();
			if (la.kind == 37) {
				lexer.NextToken();
				if (StartOf(6)) {
					FormalParameterList(
#line  1346 "VBNET.ATG" 
p);
				}
				Expect(38);
			}
			Expect(1);
			Block(
#line  1347 "VBNET.ATG" 
out stmt);
			Expect(113);
			Expect(193);
			EndOfStmt();

#line  1349 "VBNET.ATG" 
			eventAccessorDeclaration = new EventRemoveRegion(attributes);
			eventAccessorDeclaration.Block = (BlockStatement)stmt;
			eventAccessorDeclaration.Parameters = p;
			
		} else if (la.kind == 189) {
			lexer.NextToken();
			if (la.kind == 37) {
				lexer.NextToken();
				if (StartOf(6)) {
					FormalParameterList(
#line  1354 "VBNET.ATG" 
p);
				}
				Expect(38);
			}
			Expect(1);
			Block(
#line  1355 "VBNET.ATG" 
out stmt);
			Expect(113);
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
		case 31: {
			lexer.NextToken();

#line  1273 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Add; 
			break;
		}
		case 30: {
			lexer.NextToken();

#line  1275 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Subtract; 
			break;
		}
		case 34: {
			lexer.NextToken();

#line  1277 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Multiply; 
			break;
		}
		case 24: {
			lexer.NextToken();

#line  1279 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Divide; 
			break;
		}
		case 25: {
			lexer.NextToken();

#line  1281 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.DivideInteger; 
			break;
		}
		case 23: {
			lexer.NextToken();

#line  1283 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Concat; 
			break;
		}
		case 150: {
			lexer.NextToken();

#line  1285 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Like; 
			break;
		}
		case 154: {
			lexer.NextToken();

#line  1287 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Modulus; 
			break;
		}
		case 60: {
			lexer.NextToken();

#line  1289 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.BitwiseAnd; 
			break;
		}
		case 175: {
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
		case 32: {
			lexer.NextToken();

#line  1295 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Power; 
			break;
		}
		case 44: {
			lexer.NextToken();

#line  1297 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.ShiftLeft; 
			break;
		}
		case 45: {
			lexer.NextToken();

#line  1299 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.ShiftRight; 
			break;
		}
		case 20: {
			lexer.NextToken();

#line  1301 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Equality; 
			break;
		}
		case 41: {
			lexer.NextToken();

#line  1303 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.InEquality; 
			break;
		}
		case 40: {
			lexer.NextToken();

#line  1305 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.LessThan; 
			break;
		}
		case 43: {
			lexer.NextToken();

#line  1307 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.LessThanOrEqual; 
			break;
		}
		case 39: {
			lexer.NextToken();

#line  1309 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.GreaterThan; 
			break;
		}
		case 42: {
			lexer.NextToken();

#line  1311 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.GreaterThanOrEqual; 
			break;
		}
		case 94: {
			lexer.NextToken();

#line  1313 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.CType; 
			break;
		}
		case 2: case 58: case 62: case 64: case 65: case 66: case 67: case 70: case 87: case 98: case 104: case 107: case 116: case 121: case 126: case 133: case 139: case 143: case 146: case 147: case 170: case 176: case 178: case 184: case 203: case 212: case 213: case 223: case 224: case 230: {
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
		Expect(128);

#line  1396 "VBNET.ATG" 
		Location startLocation = t.Location; 
		Expect(1);
		Block(
#line  1398 "VBNET.ATG" 
out stmt);

#line  1399 "VBNET.ATG" 
		getBlock = new PropertyGetRegion((BlockStatement)stmt, attributes); 
		Expect(113);
		Expect(128);

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
		if (la.kind == 37) {
			lexer.NextToken();
			if (StartOf(6)) {
				FormalParameterList(
#line  1417 "VBNET.ATG" 
p);
			}
			Expect(38);
		}
		Expect(1);
		Block(
#line  1419 "VBNET.ATG" 
out stmt);

#line  1421 "VBNET.ATG" 
		setBlock = new PropertySetRegion((BlockStatement)stmt, attributes);
		setBlock.Modifier = m;
		setBlock.Parameters = p;
		
		Expect(113);
		Expect(198);

#line  1426 "VBNET.ATG" 
		setBlock.StartLocation = startLocation; setBlock.EndLocation = t.EndLocation; 
		EndOfStmt();
	}

	void PropertyAccessorAccessModifier(
#line  3614 "VBNET.ATG" 
out Modifiers m) {

#line  3615 "VBNET.ATG" 
		m = Modifiers.None; 
		while (StartOf(29)) {
			if (la.kind == 188) {
				lexer.NextToken();

#line  3617 "VBNET.ATG" 
				m |= Modifiers.Public; 
			} else if (la.kind == 187) {
				lexer.NextToken();

#line  3618 "VBNET.ATG" 
				m |= Modifiers.Protected; 
			} else if (la.kind == 125) {
				lexer.NextToken();

#line  3619 "VBNET.ATG" 
				m |= Modifiers.Internal; 
			} else {
				lexer.NextToken();

#line  3620 "VBNET.ATG" 
				m |= Modifiers.Private; 
			}
		}
	}

	void ArrayInitializationModifier(
#line  1524 "VBNET.ATG" 
out List<Expression> arrayModifiers) {

#line  1526 "VBNET.ATG" 
		arrayModifiers = null;
		
		Expect(37);
		InitializationRankList(
#line  1528 "VBNET.ATG" 
out arrayModifiers);
		Expect(38);
	}

	void ArrayNameModifier(
#line  2691 "VBNET.ATG" 
out ArrayList arrayModifiers) {

#line  2693 "VBNET.ATG" 
		arrayModifiers = null;
		
		ArrayTypeModifiers(
#line  2695 "VBNET.ATG" 
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
		while (la.kind == 22) {
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
		
		Expect(35);
		if (StartOf(30)) {
			Expr(
#line  1559 "VBNET.ATG" 
out expr);

#line  1561 "VBNET.ATG" 
			if (expr != null) { initializer.CreateExpressions.Add(expr); }
			
			while (
#line  1564 "VBNET.ATG" 
NotFinalComma()) {
				Expect(22);
				Expr(
#line  1564 "VBNET.ATG" 
out expr);

#line  1565 "VBNET.ATG" 
				if (expr != null) { initializer.CreateExpressions.Add(expr); } 
			}
		}
		Expect(36);

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
		} else if (la.kind == 158) {
			lexer.NextToken();
		} else if (la.kind == 153) {
			lexer.NextToken();
		} else SynErr(272);

#line  1642 "VBNET.ATG" 
		name = t.val; 
		Expect(26);
		IdentifierOrKeyword(
#line  1644 "VBNET.ATG" 
out eventName);

#line  1645 "VBNET.ATG" 
		name = name + "." + eventName; 
	}

	void IdentifierOrKeyword(
#line  3547 "VBNET.ATG" 
out string name) {
		lexer.NextToken();

#line  3549 "VBNET.ATG" 
		name = t.val;  
	}

	void QueryExpr(
#line  2215 "VBNET.ATG" 
out Expression expr) {

#line  2217 "VBNET.ATG" 
		QueryExpressionVB qexpr = new QueryExpressionVB();
		qexpr.StartLocation = la.Location;
		expr = qexpr;
		
		FromOrAggregateQueryOperator(
#line  2221 "VBNET.ATG" 
qexpr.Clauses);
		while (StartOf(31)) {
			QueryOperator(
#line  2222 "VBNET.ATG" 
qexpr.Clauses);
		}

#line  2224 "VBNET.ATG" 
		qexpr.EndLocation = t.EndLocation;
		
	}

	void LambdaExpr(
#line  2135 "VBNET.ATG" 
out Expression expr) {

#line  2137 "VBNET.ATG" 
		LambdaExpression lambda = null;
		
		if (la.kind == 210) {
			SubLambdaExpression(
#line  2139 "VBNET.ATG" 
out lambda);
		} else if (la.kind == 127) {
			FunctionLambdaExpression(
#line  2140 "VBNET.ATG" 
out lambda);
		} else SynErr(273);

#line  2141 "VBNET.ATG" 
		expr = lambda; 
	}

	void DisjunctionExpr(
#line  1908 "VBNET.ATG" 
out Expression outExpr) {

#line  1910 "VBNET.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		ConjunctionExpr(
#line  1913 "VBNET.ATG" 
out outExpr);
		while (la.kind == 175 || la.kind == 177 || la.kind == 236) {
			if (la.kind == 175) {
				lexer.NextToken();

#line  1916 "VBNET.ATG" 
				op = BinaryOperatorType.BitwiseOr; 
			} else if (la.kind == 177) {
				lexer.NextToken();

#line  1917 "VBNET.ATG" 
				op = BinaryOperatorType.LogicalOr; 
			} else {
				lexer.NextToken();

#line  1918 "VBNET.ATG" 
				op = BinaryOperatorType.ExclusiveOr; 
			}
			ConjunctionExpr(
#line  1920 "VBNET.ATG" 
out expr);

#line  1920 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr);  
		}
	}

	void XmlLiteralExpression(
#line  1772 "VBNET.ATG" 
out Expression pexpr) {

#line  1774 "VBNET.ATG" 
		XmlLiteralExpression expr = new XmlLiteralExpression();
		List<XmlExpression> exprs = expr.Expressions;
		XmlExpression currentExpression = null;
		pexpr = expr;
		
		if (StartOf(32)) {
			XmlContentExpression(
#line  1781 "VBNET.ATG" 
exprs);
			while (StartOf(32)) {
				XmlContentExpression(
#line  1781 "VBNET.ATG" 
exprs);
			}
			if (la.kind == 10) {
				XmlElement(
#line  1781 "VBNET.ATG" 
out currentExpression);

#line  1781 "VBNET.ATG" 
				exprs.Add(currentExpression); 
				while (StartOf(32)) {
					XmlContentExpression(
#line  1781 "VBNET.ATG" 
exprs);
				}
			}
		} else if (la.kind == 10) {
			XmlElement(
#line  1783 "VBNET.ATG" 
out currentExpression);

#line  1783 "VBNET.ATG" 
			exprs.Add(currentExpression); 
			while (StartOf(32)) {
				XmlContentExpression(
#line  1783 "VBNET.ATG" 
exprs);
			}
		} else SynErr(274);
	}

	void AssignmentOperator(
#line  1657 "VBNET.ATG" 
out AssignmentOperatorType op) {

#line  1658 "VBNET.ATG" 
		op = AssignmentOperatorType.None; 
		switch (la.kind) {
		case 20: {
			lexer.NextToken();

#line  1659 "VBNET.ATG" 
			op = AssignmentOperatorType.Assign; 
			break;
		}
		case 54: {
			lexer.NextToken();

#line  1660 "VBNET.ATG" 
			op = AssignmentOperatorType.ConcatString; 
			break;
		}
		case 46: {
			lexer.NextToken();

#line  1661 "VBNET.ATG" 
			op = AssignmentOperatorType.Add; 
			break;
		}
		case 48: {
			lexer.NextToken();

#line  1662 "VBNET.ATG" 
			op = AssignmentOperatorType.Subtract; 
			break;
		}
		case 49: {
			lexer.NextToken();

#line  1663 "VBNET.ATG" 
			op = AssignmentOperatorType.Multiply; 
			break;
		}
		case 50: {
			lexer.NextToken();

#line  1664 "VBNET.ATG" 
			op = AssignmentOperatorType.Divide; 
			break;
		}
		case 51: {
			lexer.NextToken();

#line  1665 "VBNET.ATG" 
			op = AssignmentOperatorType.DivideInteger; 
			break;
		}
		case 47: {
			lexer.NextToken();

#line  1666 "VBNET.ATG" 
			op = AssignmentOperatorType.Power; 
			break;
		}
		case 52: {
			lexer.NextToken();

#line  1667 "VBNET.ATG" 
			op = AssignmentOperatorType.ShiftLeft; 
			break;
		}
		case 53: {
			lexer.NextToken();

#line  1668 "VBNET.ATG" 
			op = AssignmentOperatorType.ShiftRight; 
			break;
		}
		default: SynErr(275); break;
		}
	}

	void SimpleExpr(
#line  1672 "VBNET.ATG" 
out Expression pexpr) {

#line  1673 "VBNET.ATG" 
		string name; 
		SimpleNonInvocationExpression(
#line  1675 "VBNET.ATG" 
out pexpr);
		while (StartOf(33)) {
			if (la.kind == 26) {
				lexer.NextToken();
				if (la.kind == 10) {
					lexer.NextToken();
					IdentifierOrKeyword(
#line  1678 "VBNET.ATG" 
out name);
					Expect(11);

#line  1679 "VBNET.ATG" 
					pexpr = new XmlMemberAccessExpression(pexpr, XmlAxisType.Element, name, true); 
				} else if (StartOf(34)) {
					IdentifierOrKeyword(
#line  1680 "VBNET.ATG" 
out name);

#line  1681 "VBNET.ATG" 
					pexpr = new MemberReferenceExpression(pexpr, name); 
				} else SynErr(276);
				if (
#line  1683 "VBNET.ATG" 
la.kind == Tokens.OpenParenthesis && Peek(1).kind == Tokens.Of) {
					lexer.NextToken();
					Expect(169);
					TypeArgumentList(
#line  1684 "VBNET.ATG" 
((MemberReferenceExpression)pexpr).TypeArguments);
					Expect(38);
				}
			} else if (la.kind == 29) {
				lexer.NextToken();
				IdentifierOrKeyword(
#line  1686 "VBNET.ATG" 
out name);

#line  1686 "VBNET.ATG" 
				pexpr = new BinaryOperatorExpression(pexpr, BinaryOperatorType.DictionaryAccess, new PrimitiveExpression(name, name)); 
			} else if (la.kind == 27 || la.kind == 28) {

#line  1687 "VBNET.ATG" 
				XmlAxisType type = XmlAxisType.Attribute; bool isXmlName = false; 
				if (la.kind == 28) {
					lexer.NextToken();
				} else if (la.kind == 27) {
					lexer.NextToken();

#line  1688 "VBNET.ATG" 
					type = XmlAxisType.Descendents; 
				} else SynErr(277);
				if (la.kind == 10) {
					lexer.NextToken();

#line  1688 "VBNET.ATG" 
					isXmlName = true; 
				}
				IdentifierOrKeyword(
#line  1688 "VBNET.ATG" 
out name);
				if (la.kind == 11) {
					lexer.NextToken();
				}

#line  1689 "VBNET.ATG" 
				pexpr = new XmlMemberAccessExpression(pexpr, type, name, isXmlName); 
			} else {
				InvocationExpression(
#line  1690 "VBNET.ATG" 
ref pexpr);
			}
		}
	}

	void SimpleNonInvocationExpression(
#line  1694 "VBNET.ATG" 
out Expression pexpr) {

#line  1696 "VBNET.ATG" 
		Expression expr;
		CollectionInitializerExpression cie;
		TypeReference type = null;
		string name = String.Empty;
		pexpr = null;
		
		if (StartOf(35)) {
			switch (la.kind) {
			case 3: {
				lexer.NextToken();

#line  1705 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(t.literalValue, t.val) { LiteralFormat = t.literalFormat };  
				break;
			}
			case 4: {
				lexer.NextToken();

#line  1706 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(t.literalValue, t.val) { LiteralFormat = t.literalFormat };  
				break;
			}
			case 7: {
				lexer.NextToken();

#line  1707 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(t.literalValue, t.val) { LiteralFormat = t.literalFormat };  
				break;
			}
			case 6: {
				lexer.NextToken();

#line  1708 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(t.literalValue, t.val) { LiteralFormat = t.literalFormat };  
				break;
			}
			case 5: {
				lexer.NextToken();

#line  1709 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(t.literalValue, t.val) { LiteralFormat = t.literalFormat };  
				break;
			}
			case 9: {
				lexer.NextToken();

#line  1710 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(t.literalValue, t.val) { LiteralFormat = t.literalFormat };  
				break;
			}
			case 8: {
				lexer.NextToken();

#line  1711 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(t.literalValue, t.val) { LiteralFormat = t.literalFormat };  
				break;
			}
			case 217: {
				lexer.NextToken();

#line  1713 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(true, "true");  
				break;
			}
			case 122: {
				lexer.NextToken();

#line  1714 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(false, "false"); 
				break;
			}
			case 165: {
				lexer.NextToken();

#line  1715 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(null, "null");  
				break;
			}
			case 37: {
				lexer.NextToken();
				Expr(
#line  1716 "VBNET.ATG" 
out expr);
				Expect(38);

#line  1716 "VBNET.ATG" 
				pexpr = new ParenthesizedExpression(expr); 
				break;
			}
			case 2: case 58: case 62: case 64: case 65: case 66: case 67: case 70: case 87: case 98: case 104: case 107: case 116: case 121: case 126: case 133: case 139: case 143: case 146: case 147: case 170: case 176: case 178: case 184: case 203: case 212: case 213: case 223: case 224: case 230: {
				Identifier();

#line  1718 "VBNET.ATG" 
				pexpr = new IdentifierExpression(t.val);
				pexpr.StartLocation = t.Location; pexpr.EndLocation = t.EndLocation;
				
				if (
#line  1721 "VBNET.ATG" 
la.kind == Tokens.OpenParenthesis && Peek(1).kind == Tokens.Of) {
					lexer.NextToken();
					Expect(169);
					TypeArgumentList(
#line  1722 "VBNET.ATG" 
((IdentifierExpression)pexpr).TypeArguments);
					Expect(38);
				}
				break;
			}
			case 68: case 71: case 82: case 99: case 100: case 109: case 141: case 151: case 168: case 196: case 201: case 202: case 208: case 221: case 222: case 225: {

#line  1724 "VBNET.ATG" 
				string val = String.Empty; 
				if (StartOf(12)) {
					PrimitiveTypeName(
#line  1725 "VBNET.ATG" 
out val);
				} else if (la.kind == 168) {
					lexer.NextToken();

#line  1725 "VBNET.ATG" 
					val = "System.Object"; 
				} else SynErr(278);

#line  1726 "VBNET.ATG" 
				pexpr = new TypeReferenceExpression(new TypeReference(val, true)); 
				break;
			}
			case 153: {
				lexer.NextToken();

#line  1727 "VBNET.ATG" 
				pexpr = new ThisReferenceExpression(); 
				break;
			}
			case 158: case 159: {

#line  1728 "VBNET.ATG" 
				Expression retExpr = null; 
				if (la.kind == 158) {
					lexer.NextToken();

#line  1729 "VBNET.ATG" 
					retExpr = new BaseReferenceExpression(); 
				} else if (la.kind == 159) {
					lexer.NextToken();

#line  1730 "VBNET.ATG" 
					retExpr = new ClassReferenceExpression(); 
				} else SynErr(279);
				Expect(26);
				IdentifierOrKeyword(
#line  1732 "VBNET.ATG" 
out name);

#line  1732 "VBNET.ATG" 
				pexpr = new MemberReferenceExpression(retExpr, name); 
				break;
			}
			case 130: {
				lexer.NextToken();
				Expect(26);
				Identifier();

#line  1734 "VBNET.ATG" 
				type = new TypeReference(t.val ?? ""); 

#line  1736 "VBNET.ATG" 
				type.IsGlobal = true; 

#line  1737 "VBNET.ATG" 
				pexpr = new TypeReferenceExpression(type); 
				break;
			}
			case 162: {
				ObjectCreateExpression(
#line  1738 "VBNET.ATG" 
out expr);

#line  1738 "VBNET.ATG" 
				pexpr = expr; 
				break;
			}
			case 35: {
				CollectionInitializer(
#line  1739 "VBNET.ATG" 
out cie);

#line  1739 "VBNET.ATG" 
				pexpr = cie; 
				break;
			}
			case 94: case 106: case 219: {

#line  1741 "VBNET.ATG" 
				CastType castType = CastType.Cast; 
				if (la.kind == 106) {
					lexer.NextToken();
				} else if (la.kind == 94) {
					lexer.NextToken();

#line  1743 "VBNET.ATG" 
					castType = CastType.Conversion; 
				} else if (la.kind == 219) {
					lexer.NextToken();

#line  1744 "VBNET.ATG" 
					castType = CastType.TryCast; 
				} else SynErr(280);
				Expect(37);
				Expr(
#line  1746 "VBNET.ATG" 
out expr);
				Expect(22);
				TypeName(
#line  1746 "VBNET.ATG" 
out type);
				Expect(38);

#line  1747 "VBNET.ATG" 
				pexpr = new CastExpression(type, expr, castType); 
				break;
			}
			case 76: case 77: case 78: case 79: case 80: case 81: case 83: case 85: case 86: case 90: case 91: case 92: case 93: case 95: case 96: case 97: {
				CastTarget(
#line  1748 "VBNET.ATG" 
out type);
				Expect(37);
				Expr(
#line  1748 "VBNET.ATG" 
out expr);
				Expect(38);

#line  1748 "VBNET.ATG" 
				pexpr = new CastExpression(type, expr, CastType.PrimitiveConversion); 
				break;
			}
			case 57: {
				lexer.NextToken();
				Expr(
#line  1749 "VBNET.ATG" 
out expr);

#line  1749 "VBNET.ATG" 
				pexpr = new AddressOfExpression(expr); 
				break;
			}
			case 129: {
				lexer.NextToken();
				Expect(37);
				GetTypeTypeName(
#line  1750 "VBNET.ATG" 
out type);
				Expect(38);

#line  1750 "VBNET.ATG" 
				pexpr = new TypeOfExpression(type); 
				break;
			}
			case 220: {
				lexer.NextToken();
				SimpleExpr(
#line  1751 "VBNET.ATG" 
out expr);
				Expect(144);
				TypeName(
#line  1751 "VBNET.ATG" 
out type);

#line  1751 "VBNET.ATG" 
				pexpr = new TypeOfIsExpression(expr, type); 
				break;
			}
			case 135: {
				ConditionalExpression(
#line  1752 "VBNET.ATG" 
out pexpr);
				break;
			}
			}
		} else if (la.kind == 26 || la.kind == 27 || la.kind == 28) {
			if (la.kind == 26) {
				lexer.NextToken();
				if (la.kind == 10) {
					lexer.NextToken();
					IdentifierOrKeyword(
#line  1758 "VBNET.ATG" 
out name);
					Expect(11);

#line  1759 "VBNET.ATG" 
					pexpr = new XmlMemberAccessExpression(null, XmlAxisType.Element, name, true); 
				} else if (StartOf(34)) {
					IdentifierOrKeyword(
#line  1760 "VBNET.ATG" 
out name);

#line  1761 "VBNET.ATG" 
					pexpr = new MemberReferenceExpression(null, name); 
				} else SynErr(281);
			} else {

#line  1763 "VBNET.ATG" 
				XmlAxisType axisType = XmlAxisType.Element; bool isXmlIdentifier = false; 
				if (la.kind == 27) {
					lexer.NextToken();

#line  1764 "VBNET.ATG" 
					axisType = XmlAxisType.Descendents; 
				} else if (la.kind == 28) {
					lexer.NextToken();

#line  1764 "VBNET.ATG" 
					axisType = XmlAxisType.Attribute; 
				} else SynErr(282);
				if (la.kind == 10) {
					lexer.NextToken();

#line  1765 "VBNET.ATG" 
					isXmlIdentifier = true; 
				}
				IdentifierOrKeyword(
#line  1765 "VBNET.ATG" 
out name);
				if (la.kind == 11) {
					lexer.NextToken();
				}

#line  1766 "VBNET.ATG" 
				pexpr = new XmlMemberAccessExpression(null, axisType, name, isXmlIdentifier); 
			}
		} else SynErr(283);
	}

	void TypeArgumentList(
#line  2727 "VBNET.ATG" 
List<TypeReference> typeArguments) {

#line  2729 "VBNET.ATG" 
		TypeReference typeref;
		
		TypeName(
#line  2731 "VBNET.ATG" 
out typeref);

#line  2731 "VBNET.ATG" 
		if (typeref != null) typeArguments.Add(typeref); 
		while (la.kind == 22) {
			lexer.NextToken();
			TypeName(
#line  2734 "VBNET.ATG" 
out typeref);

#line  2734 "VBNET.ATG" 
			if (typeref != null) typeArguments.Add(typeref); 
		}
	}

	void InvocationExpression(
#line  1872 "VBNET.ATG" 
ref Expression pexpr) {

#line  1873 "VBNET.ATG" 
		List<Expression> parameters = null; 
		Expect(37);

#line  1875 "VBNET.ATG" 
		Location start = t.Location; 
		ArgumentList(
#line  1876 "VBNET.ATG" 
out parameters);
		Expect(38);

#line  1879 "VBNET.ATG" 
		pexpr = new InvocationExpression(pexpr, parameters);
		

#line  1881 "VBNET.ATG" 
		pexpr.StartLocation = start; pexpr.EndLocation = t.Location; 
	}

	void PrimitiveTypeName(
#line  3554 "VBNET.ATG" 
out string type) {

#line  3555 "VBNET.ATG" 
		type = String.Empty; 
		switch (la.kind) {
		case 68: {
			lexer.NextToken();

#line  3556 "VBNET.ATG" 
			type = "System.Boolean"; 
			break;
		}
		case 99: {
			lexer.NextToken();

#line  3557 "VBNET.ATG" 
			type = "System.DateTime"; 
			break;
		}
		case 82: {
			lexer.NextToken();

#line  3558 "VBNET.ATG" 
			type = "System.Char"; 
			break;
		}
		case 208: {
			lexer.NextToken();

#line  3559 "VBNET.ATG" 
			type = "System.String"; 
			break;
		}
		case 100: {
			lexer.NextToken();

#line  3560 "VBNET.ATG" 
			type = "System.Decimal"; 
			break;
		}
		case 71: {
			lexer.NextToken();

#line  3561 "VBNET.ATG" 
			type = "System.Byte"; 
			break;
		}
		case 201: {
			lexer.NextToken();

#line  3562 "VBNET.ATG" 
			type = "System.Int16"; 
			break;
		}
		case 141: {
			lexer.NextToken();

#line  3563 "VBNET.ATG" 
			type = "System.Int32"; 
			break;
		}
		case 151: {
			lexer.NextToken();

#line  3564 "VBNET.ATG" 
			type = "System.Int64"; 
			break;
		}
		case 202: {
			lexer.NextToken();

#line  3565 "VBNET.ATG" 
			type = "System.Single"; 
			break;
		}
		case 109: {
			lexer.NextToken();

#line  3566 "VBNET.ATG" 
			type = "System.Double"; 
			break;
		}
		case 221: {
			lexer.NextToken();

#line  3567 "VBNET.ATG" 
			type = "System.UInt32"; 
			break;
		}
		case 222: {
			lexer.NextToken();

#line  3568 "VBNET.ATG" 
			type = "System.UInt64"; 
			break;
		}
		case 225: {
			lexer.NextToken();

#line  3569 "VBNET.ATG" 
			type = "System.UInt16"; 
			break;
		}
		case 196: {
			lexer.NextToken();

#line  3570 "VBNET.ATG" 
			type = "System.SByte"; 
			break;
		}
		default: SynErr(284); break;
		}
	}

	void CastTarget(
#line  1886 "VBNET.ATG" 
out TypeReference type) {

#line  1888 "VBNET.ATG" 
		type = null;
		
		switch (la.kind) {
		case 76: {
			lexer.NextToken();

#line  1890 "VBNET.ATG" 
			type = new TypeReference("System.Boolean", true); 
			break;
		}
		case 77: {
			lexer.NextToken();

#line  1891 "VBNET.ATG" 
			type = new TypeReference("System.Byte", true); 
			break;
		}
		case 90: {
			lexer.NextToken();

#line  1892 "VBNET.ATG" 
			type = new TypeReference("System.SByte", true); 
			break;
		}
		case 78: {
			lexer.NextToken();

#line  1893 "VBNET.ATG" 
			type = new TypeReference("System.Char", true); 
			break;
		}
		case 79: {
			lexer.NextToken();

#line  1894 "VBNET.ATG" 
			type = new TypeReference("System.DateTime", true); 
			break;
		}
		case 81: {
			lexer.NextToken();

#line  1895 "VBNET.ATG" 
			type = new TypeReference("System.Decimal", true); 
			break;
		}
		case 80: {
			lexer.NextToken();

#line  1896 "VBNET.ATG" 
			type = new TypeReference("System.Double", true); 
			break;
		}
		case 91: {
			lexer.NextToken();

#line  1897 "VBNET.ATG" 
			type = new TypeReference("System.Int16", true); 
			break;
		}
		case 83: {
			lexer.NextToken();

#line  1898 "VBNET.ATG" 
			type = new TypeReference("System.Int32", true); 
			break;
		}
		case 85: {
			lexer.NextToken();

#line  1899 "VBNET.ATG" 
			type = new TypeReference("System.Int64", true); 
			break;
		}
		case 97: {
			lexer.NextToken();

#line  1900 "VBNET.ATG" 
			type = new TypeReference("System.UInt16", true); 
			break;
		}
		case 95: {
			lexer.NextToken();

#line  1901 "VBNET.ATG" 
			type = new TypeReference("System.UInt32", true); 
			break;
		}
		case 96: {
			lexer.NextToken();

#line  1902 "VBNET.ATG" 
			type = new TypeReference("System.UInt64", true); 
			break;
		}
		case 86: {
			lexer.NextToken();

#line  1903 "VBNET.ATG" 
			type = new TypeReference("System.Object", true); 
			break;
		}
		case 92: {
			lexer.NextToken();

#line  1904 "VBNET.ATG" 
			type = new TypeReference("System.Single", true); 
			break;
		}
		case 93: {
			lexer.NextToken();

#line  1905 "VBNET.ATG" 
			type = new TypeReference("System.String", true); 
			break;
		}
		default: SynErr(285); break;
		}
	}

	void GetTypeTypeName(
#line  2626 "VBNET.ATG" 
out TypeReference typeref) {

#line  2627 "VBNET.ATG" 
		ArrayList rank = null; 
		NonArrayTypeName(
#line  2629 "VBNET.ATG" 
out typeref, true);
		ArrayTypeModifiers(
#line  2630 "VBNET.ATG" 
out rank);

#line  2631 "VBNET.ATG" 
		if (rank != null && typeref != null) {
		typeref.RankSpecifier = (int[])rank.ToArray(typeof(int));
		}
		
	}

	void ConditionalExpression(
#line  1838 "VBNET.ATG" 
out Expression expr) {

#line  1840 "VBNET.ATG" 
		ConditionalExpression conditionalExpression = new ConditionalExpression();
		BinaryOperatorExpression binaryOperatorExpression = new BinaryOperatorExpression();
		conditionalExpression.StartLocation = binaryOperatorExpression.StartLocation = la.Location;
		
		Expression condition = null;
		Expression trueExpr = null;
		Expression falseExpr = null;
		
		Expect(135);
		Expect(37);
		Expr(
#line  1849 "VBNET.ATG" 
out condition);
		Expect(22);
		Expr(
#line  1849 "VBNET.ATG" 
out trueExpr);
		if (la.kind == 22) {
			lexer.NextToken();
			Expr(
#line  1849 "VBNET.ATG" 
out falseExpr);
		}
		Expect(38);

#line  1851 "VBNET.ATG" 
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

	void XmlContentExpression(
#line  1787 "VBNET.ATG" 
List<XmlExpression> exprs) {

#line  1788 "VBNET.ATG" 
		XmlContentExpression expr = null; 
		if (la.kind == 16) {
			lexer.NextToken();

#line  1790 "VBNET.ATG" 
			expr = new XmlContentExpression(t.val, XmlContentType.Text); 
		} else if (la.kind == 18) {
			lexer.NextToken();

#line  1791 "VBNET.ATG" 
			expr = new XmlContentExpression(t.val, XmlContentType.CData); 
		} else if (la.kind == 17) {
			lexer.NextToken();

#line  1792 "VBNET.ATG" 
			expr = new XmlContentExpression(t.val, XmlContentType.Comment); 
		} else if (la.kind == 19) {
			lexer.NextToken();

#line  1793 "VBNET.ATG" 
			expr = new XmlContentExpression(t.val, XmlContentType.ProcessingInstruction); 
		} else SynErr(286);

#line  1796 "VBNET.ATG" 
		expr.StartLocation = t.Location;
		expr.EndLocation = t.EndLocation;
		exprs.Add(expr);
		
	}

	void XmlElement(
#line  1816 "VBNET.ATG" 
out XmlExpression expr) {

#line  1817 "VBNET.ATG" 
		XmlElementExpression el = new XmlElementExpression(); 
		Expect(10);

#line  1820 "VBNET.ATG" 
		el.StartLocation = t.Location; 
		if (la.kind == 12) {
			lexer.NextToken();

#line  1821 "VBNET.ATG" 
			Expression innerExpression; 
			Expr(
#line  1821 "VBNET.ATG" 
out innerExpression);
			Expect(13);

#line  1822 "VBNET.ATG" 
			el.NameExpression = new XmlEmbeddedExpression() { InlineVBExpression = innerExpression }; 
		} else if (StartOf(4)) {
			Identifier();

#line  1823 "VBNET.ATG" 
			el.XmlName = t.val; 
		} else SynErr(287);
		while (StartOf(36)) {
			XmlAttribute(
#line  1823 "VBNET.ATG" 
el.Attributes);
		}
		if (la.kind == 14) {
			lexer.NextToken();

#line  1824 "VBNET.ATG" 
			el.EndLocation = t.EndLocation; 
		} else if (la.kind == 11) {
			lexer.NextToken();
			while (StartOf(37)) {

#line  1824 "VBNET.ATG" 
				XmlExpression child; 
				XmlNestedContent(
#line  1824 "VBNET.ATG" 
out child);

#line  1824 "VBNET.ATG" 
				el.Children.Add(child); 
			}
			Expect(15);
			while (StartOf(38)) {
				lexer.NextToken();
			}
			Expect(11);

#line  1824 "VBNET.ATG" 
			el.EndLocation = t.EndLocation; 
		} else SynErr(288);

#line  1826 "VBNET.ATG" 
		expr = el; 
	}

	void XmlNestedContent(
#line  1802 "VBNET.ATG" 
out XmlExpression expr) {

#line  1803 "VBNET.ATG" 
		XmlExpression tmpExpr = null; 
		switch (la.kind) {
		case 16: {
			lexer.NextToken();

#line  1806 "VBNET.ATG" 
			tmpExpr = new XmlContentExpression(t.val, XmlContentType.Text); 
			break;
		}
		case 18: {
			lexer.NextToken();

#line  1807 "VBNET.ATG" 
			tmpExpr = new XmlContentExpression(t.val, XmlContentType.CData); 
			break;
		}
		case 17: {
			lexer.NextToken();

#line  1808 "VBNET.ATG" 
			tmpExpr = new XmlContentExpression(t.val, XmlContentType.Comment); 
			break;
		}
		case 19: {
			lexer.NextToken();

#line  1809 "VBNET.ATG" 
			tmpExpr = new XmlContentExpression(t.val, XmlContentType.ProcessingInstruction); 
			break;
		}
		case 12: {
			lexer.NextToken();

#line  1810 "VBNET.ATG" 
			Expression innerExpression; 
			Expr(
#line  1810 "VBNET.ATG" 
out innerExpression);
			Expect(13);

#line  1810 "VBNET.ATG" 
			tmpExpr = new XmlEmbeddedExpression() { InlineVBExpression = innerExpression }; 
			break;
		}
		case 10: {
			XmlElement(
#line  1811 "VBNET.ATG" 
out tmpExpr);
			break;
		}
		default: SynErr(289); break;
		}

#line  1813 "VBNET.ATG" 
		expr = tmpExpr; 
	}

	void XmlAttribute(
#line  1829 "VBNET.ATG" 
List<XmlExpression> attrs) {
		if (StartOf(4)) {
			Identifier();

#line  1830 "VBNET.ATG" 
			string name = t.val; 
			Expect(20);

#line  1831 "VBNET.ATG" 
			string literalValue = null; Expression expressionValue = null; 
			if (la.kind == 3) {
				lexer.NextToken();

#line  1832 "VBNET.ATG" 
				literalValue = t.literalValue.ToString(); 
			} else if (la.kind == 12) {
				lexer.NextToken();
				Expr(
#line  1832 "VBNET.ATG" 
out expressionValue);
				Expect(13);
			} else SynErr(290);

#line  1833 "VBNET.ATG" 
			attrs.Add(new XmlAttribute() { Name = name, ExpressionValue = expressionValue, LiteralValue = literalValue }); 
		} else if (la.kind == 12) {
			lexer.NextToken();

#line  1834 "VBNET.ATG" 
			Expression innerExpression; 
			Expr(
#line  1834 "VBNET.ATG" 
out innerExpression);
			Expect(13);

#line  1835 "VBNET.ATG" 
			attrs.Add(new XmlEmbeddedExpression() { InlineVBExpression = innerExpression }); 
		} else SynErr(291);
	}

	void ArgumentList(
#line  2558 "VBNET.ATG" 
out List<Expression> arguments) {

#line  2560 "VBNET.ATG" 
		arguments = new List<Expression>();
		Expression expr = null;
		
		if (StartOf(30)) {
			Argument(
#line  2563 "VBNET.ATG" 
out expr);
		}
		while (la.kind == 22) {
			lexer.NextToken();

#line  2564 "VBNET.ATG" 
			arguments.Add(expr ?? Expression.Null); expr = null; 
			if (StartOf(30)) {
				Argument(
#line  2565 "VBNET.ATG" 
out expr);
			}

#line  2566 "VBNET.ATG" 
			if (expr == null) expr = Expression.Null; 
		}

#line  2568 "VBNET.ATG" 
		if (expr != null) arguments.Add(expr); 
	}

	void ConjunctionExpr(
#line  1924 "VBNET.ATG" 
out Expression outExpr) {

#line  1926 "VBNET.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		NotExpr(
#line  1929 "VBNET.ATG" 
out outExpr);
		while (la.kind == 60 || la.kind == 61) {
			if (la.kind == 60) {
				lexer.NextToken();

#line  1932 "VBNET.ATG" 
				op = BinaryOperatorType.BitwiseAnd; 
			} else {
				lexer.NextToken();

#line  1933 "VBNET.ATG" 
				op = BinaryOperatorType.LogicalAnd; 
			}
			NotExpr(
#line  1935 "VBNET.ATG" 
out expr);

#line  1935 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr);  
		}
	}

	void NotExpr(
#line  1939 "VBNET.ATG" 
out Expression outExpr) {

#line  1940 "VBNET.ATG" 
		UnaryOperatorType uop = UnaryOperatorType.None; 
		while (la.kind == 164) {
			lexer.NextToken();

#line  1941 "VBNET.ATG" 
			uop = UnaryOperatorType.Not; 
		}
		ComparisonExpr(
#line  1942 "VBNET.ATG" 
out outExpr);

#line  1943 "VBNET.ATG" 
		if (uop != UnaryOperatorType.None)
		outExpr = new UnaryOperatorExpression(outExpr, uop);
		
	}

	void ComparisonExpr(
#line  1948 "VBNET.ATG" 
out Expression outExpr) {

#line  1950 "VBNET.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		ShiftExpr(
#line  1953 "VBNET.ATG" 
out outExpr);
		while (StartOf(39)) {
			switch (la.kind) {
			case 40: {
				lexer.NextToken();

#line  1956 "VBNET.ATG" 
				op = BinaryOperatorType.LessThan; 
				break;
			}
			case 39: {
				lexer.NextToken();

#line  1957 "VBNET.ATG" 
				op = BinaryOperatorType.GreaterThan; 
				break;
			}
			case 43: {
				lexer.NextToken();

#line  1958 "VBNET.ATG" 
				op = BinaryOperatorType.LessThanOrEqual; 
				break;
			}
			case 42: {
				lexer.NextToken();

#line  1959 "VBNET.ATG" 
				op = BinaryOperatorType.GreaterThanOrEqual; 
				break;
			}
			case 41: {
				lexer.NextToken();

#line  1960 "VBNET.ATG" 
				op = BinaryOperatorType.InEquality; 
				break;
			}
			case 20: {
				lexer.NextToken();

#line  1961 "VBNET.ATG" 
				op = BinaryOperatorType.Equality; 
				break;
			}
			case 150: {
				lexer.NextToken();

#line  1962 "VBNET.ATG" 
				op = BinaryOperatorType.Like; 
				break;
			}
			case 144: {
				lexer.NextToken();

#line  1963 "VBNET.ATG" 
				op = BinaryOperatorType.ReferenceEquality; 
				break;
			}
			case 145: {
				lexer.NextToken();

#line  1964 "VBNET.ATG" 
				op = BinaryOperatorType.ReferenceInequality; 
				break;
			}
			}
			if (StartOf(40)) {
				ShiftExpr(
#line  1967 "VBNET.ATG" 
out expr);

#line  1967 "VBNET.ATG" 
				outExpr = new BinaryOperatorExpression(outExpr, op, expr);  
			} else if (la.kind == 164) {
				lexer.NextToken();
				ShiftExpr(
#line  1970 "VBNET.ATG" 
out expr);

#line  1970 "VBNET.ATG" 
				outExpr = new BinaryOperatorExpression(outExpr, op, new UnaryOperatorExpression(expr, UnaryOperatorType.Not));  
			} else SynErr(292);
		}
	}

	void ShiftExpr(
#line  1975 "VBNET.ATG" 
out Expression outExpr) {

#line  1977 "VBNET.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		ConcatenationExpr(
#line  1980 "VBNET.ATG" 
out outExpr);
		while (la.kind == 44 || la.kind == 45) {
			if (la.kind == 44) {
				lexer.NextToken();

#line  1983 "VBNET.ATG" 
				op = BinaryOperatorType.ShiftLeft; 
			} else {
				lexer.NextToken();

#line  1984 "VBNET.ATG" 
				op = BinaryOperatorType.ShiftRight; 
			}
			ConcatenationExpr(
#line  1986 "VBNET.ATG" 
out expr);

#line  1986 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr);  
		}
	}

	void ConcatenationExpr(
#line  1990 "VBNET.ATG" 
out Expression outExpr) {

#line  1991 "VBNET.ATG" 
		Expression expr; 
		AdditiveExpr(
#line  1993 "VBNET.ATG" 
out outExpr);
		while (la.kind == 23) {
			lexer.NextToken();
			AdditiveExpr(
#line  1993 "VBNET.ATG" 
out expr);

#line  1993 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.Concat, expr);  
		}
	}

	void AdditiveExpr(
#line  1996 "VBNET.ATG" 
out Expression outExpr) {

#line  1998 "VBNET.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		ModuloExpr(
#line  2001 "VBNET.ATG" 
out outExpr);
		while (la.kind == 30 || la.kind == 31) {
			if (la.kind == 31) {
				lexer.NextToken();

#line  2004 "VBNET.ATG" 
				op = BinaryOperatorType.Add; 
			} else {
				lexer.NextToken();

#line  2005 "VBNET.ATG" 
				op = BinaryOperatorType.Subtract; 
			}
			ModuloExpr(
#line  2007 "VBNET.ATG" 
out expr);

#line  2007 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr);  
		}
	}

	void ModuloExpr(
#line  2011 "VBNET.ATG" 
out Expression outExpr) {

#line  2012 "VBNET.ATG" 
		Expression expr; 
		IntegerDivisionExpr(
#line  2014 "VBNET.ATG" 
out outExpr);
		while (la.kind == 154) {
			lexer.NextToken();
			IntegerDivisionExpr(
#line  2014 "VBNET.ATG" 
out expr);

#line  2014 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.Modulus, expr);  
		}
	}

	void IntegerDivisionExpr(
#line  2017 "VBNET.ATG" 
out Expression outExpr) {

#line  2018 "VBNET.ATG" 
		Expression expr; 
		MultiplicativeExpr(
#line  2020 "VBNET.ATG" 
out outExpr);
		while (la.kind == 25) {
			lexer.NextToken();
			MultiplicativeExpr(
#line  2020 "VBNET.ATG" 
out expr);

#line  2020 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.DivideInteger, expr);  
		}
	}

	void MultiplicativeExpr(
#line  2023 "VBNET.ATG" 
out Expression outExpr) {

#line  2025 "VBNET.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		UnaryExpr(
#line  2028 "VBNET.ATG" 
out outExpr);
		while (la.kind == 24 || la.kind == 34) {
			if (la.kind == 34) {
				lexer.NextToken();

#line  2031 "VBNET.ATG" 
				op = BinaryOperatorType.Multiply; 
			} else {
				lexer.NextToken();

#line  2032 "VBNET.ATG" 
				op = BinaryOperatorType.Divide; 
			}
			UnaryExpr(
#line  2034 "VBNET.ATG" 
out expr);

#line  2034 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr); 
		}
	}

	void UnaryExpr(
#line  2038 "VBNET.ATG" 
out Expression uExpr) {

#line  2040 "VBNET.ATG" 
		Expression expr;
		UnaryOperatorType uop = UnaryOperatorType.None;
		bool isUOp = false;
		
		while (la.kind == 30 || la.kind == 31 || la.kind == 34) {
			if (la.kind == 31) {
				lexer.NextToken();

#line  2044 "VBNET.ATG" 
				uop = UnaryOperatorType.Plus; isUOp = true; 
			} else if (la.kind == 30) {
				lexer.NextToken();

#line  2045 "VBNET.ATG" 
				uop = UnaryOperatorType.Minus; isUOp = true; 
			} else {
				lexer.NextToken();

#line  2046 "VBNET.ATG" 
				uop = UnaryOperatorType.Dereference;  isUOp = true;
			}
		}
		ExponentiationExpr(
#line  2048 "VBNET.ATG" 
out expr);

#line  2050 "VBNET.ATG" 
		if (isUOp) {
		uExpr = new UnaryOperatorExpression(expr, uop);
		} else {
			uExpr = expr;
		}
		
	}

	void ExponentiationExpr(
#line  2058 "VBNET.ATG" 
out Expression outExpr) {

#line  2059 "VBNET.ATG" 
		Expression expr; 
		SimpleExpr(
#line  2061 "VBNET.ATG" 
out outExpr);
		while (la.kind == 32) {
			lexer.NextToken();
			SimpleExpr(
#line  2061 "VBNET.ATG" 
out expr);

#line  2061 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.Power, expr);  
		}
	}

	void NormalOrReDimArgumentList(
#line  2572 "VBNET.ATG" 
out List<Expression> arguments, out bool canBeNormal, out bool canBeRedim) {

#line  2574 "VBNET.ATG" 
		arguments = new List<Expression>();
		canBeNormal = true; canBeRedim = !IsNamedAssign();
		Expression expr = null;
		
		if (StartOf(30)) {
			Argument(
#line  2579 "VBNET.ATG" 
out expr);
			if (la.kind == 216) {
				lexer.NextToken();

#line  2580 "VBNET.ATG" 
				EnsureIsZero(expr); canBeNormal = false; 
				Expr(
#line  2581 "VBNET.ATG" 
out expr);
			}
		}
		while (la.kind == 22) {
			lexer.NextToken();

#line  2584 "VBNET.ATG" 
			if (expr == null) canBeRedim = false; 

#line  2585 "VBNET.ATG" 
			arguments.Add(expr ?? Expression.Null); expr = null; 

#line  2586 "VBNET.ATG" 
			canBeRedim &= !IsNamedAssign(); 
			if (StartOf(30)) {
				Argument(
#line  2587 "VBNET.ATG" 
out expr);
				if (la.kind == 216) {
					lexer.NextToken();

#line  2588 "VBNET.ATG" 
					EnsureIsZero(expr); canBeNormal = false; 
					Expr(
#line  2589 "VBNET.ATG" 
out expr);
				}
			}

#line  2591 "VBNET.ATG" 
			if (expr == null) { canBeRedim = false; expr = Expression.Null; } 
		}

#line  2593 "VBNET.ATG" 
		if (expr != null) arguments.Add(expr); else canBeRedim = false; 
	}

	void ArrayTypeModifiers(
#line  2700 "VBNET.ATG" 
out ArrayList arrayModifiers) {

#line  2702 "VBNET.ATG" 
		arrayModifiers = new ArrayList();
		int i = 0;
		
		while (
#line  2705 "VBNET.ATG" 
IsDims()) {
			Expect(37);
			if (la.kind == 22 || la.kind == 38) {
				RankList(
#line  2707 "VBNET.ATG" 
out i);
			}

#line  2709 "VBNET.ATG" 
			arrayModifiers.Add(i);
			
			Expect(38);
		}

#line  2714 "VBNET.ATG" 
		if(arrayModifiers.Count == 0) {
		 arrayModifiers = null;
		}
		
	}

	void MemberInitializer(
#line  2539 "VBNET.ATG" 
out MemberInitializerExpression memberInitializer) {

#line  2541 "VBNET.ATG" 
		memberInitializer = new MemberInitializerExpression();
		memberInitializer.StartLocation = la.Location;
		Expression initExpr = null;
		bool isKey = false;
		string name = null;
		
		if (la.kind == 147) {
			lexer.NextToken();

#line  2547 "VBNET.ATG" 
			isKey = true; 
		}
		Expect(26);
		IdentifierOrKeyword(
#line  2548 "VBNET.ATG" 
out name);
		Expect(20);
		Expr(
#line  2548 "VBNET.ATG" 
out initExpr);

#line  2550 "VBNET.ATG" 
		memberInitializer.Name = name;
		memberInitializer.Expression = initExpr;
		memberInitializer.IsKey = isKey;
		memberInitializer.EndLocation = t.EndLocation;
		
	}

	void SubLambdaExpression(
#line  2144 "VBNET.ATG" 
out LambdaExpression lambda) {

#line  2146 "VBNET.ATG" 
		lambda = new LambdaExpression();
		lambda.ReturnType = new TypeReference("System.Void", true);
		Expression inner = null;
		Statement statement = null;
		lambda.StartLocation = la.Location;
		
		Expect(210);
		if (la.kind == 37) {
			lexer.NextToken();
			if (StartOf(6)) {
				FormalParameterList(
#line  2153 "VBNET.ATG" 
lambda.Parameters);
			}
			Expect(38);
		}
		if (StartOf(41)) {
			if (StartOf(30)) {
				Expr(
#line  2156 "VBNET.ATG" 
out inner);

#line  2158 "VBNET.ATG" 
				lambda.ExpressionBody = inner;
				lambda.EndLocation = t.EndLocation; // la.Location?
				
			} else {
				EmbeddedStatement(
#line  2162 "VBNET.ATG" 
out statement);

#line  2164 "VBNET.ATG" 
				lambda.StatementBody = statement;
				lambda.EndLocation = t.EndLocation;
				
			}
		} else if (la.kind == 1) {
			lexer.NextToken();
			Block(
#line  2170 "VBNET.ATG" 
out statement);
			Expect(113);
			Expect(210);

#line  2173 "VBNET.ATG" 
			lambda.StatementBody = statement;
			lambda.EndLocation = t.EndLocation;
			
		} else SynErr(293);
	}

	void FunctionLambdaExpression(
#line  2179 "VBNET.ATG" 
out LambdaExpression lambda) {

#line  2181 "VBNET.ATG" 
		lambda = new LambdaExpression();
		TypeReference typeRef = null;
		Expression inner = null;
		Statement statement = null;
		lambda.StartLocation = la.Location;
		
		Expect(127);
		if (la.kind == 37) {
			lexer.NextToken();
			if (StartOf(6)) {
				FormalParameterList(
#line  2188 "VBNET.ATG" 
lambda.Parameters);
			}
			Expect(38);
		}
		if (la.kind == 63) {
			lexer.NextToken();
			TypeName(
#line  2189 "VBNET.ATG" 
out typeRef);

#line  2189 "VBNET.ATG" 
			lambda.ReturnType = typeRef; 
		}
		if (StartOf(41)) {
			if (StartOf(30)) {
				Expr(
#line  2192 "VBNET.ATG" 
out inner);

#line  2194 "VBNET.ATG" 
				lambda.ExpressionBody = inner;
				lambda.EndLocation = t.EndLocation; // la.Location?
				
			} else {
				EmbeddedStatement(
#line  2198 "VBNET.ATG" 
out statement);

#line  2200 "VBNET.ATG" 
				lambda.StatementBody = statement;
				lambda.EndLocation = t.EndLocation;
				
			}
		} else if (la.kind == 1) {
			lexer.NextToken();
			Block(
#line  2206 "VBNET.ATG" 
out statement);
			Expect(113);
			Expect(127);

#line  2209 "VBNET.ATG" 
			lambda.StatementBody = statement;
			lambda.EndLocation = t.EndLocation;
			
		} else SynErr(294);
	}

	void EmbeddedStatement(
#line  2971 "VBNET.ATG" 
out Statement statement) {

#line  2973 "VBNET.ATG" 
		Statement embeddedStatement = null;
		statement = null;
		Expression expr = null;
		string name = String.Empty;
		List<Expression> p = null;
		
		if (la.kind == 120) {
			lexer.NextToken();

#line  2979 "VBNET.ATG" 
			ExitType exitType = ExitType.None; 
			switch (la.kind) {
			case 210: {
				lexer.NextToken();

#line  2981 "VBNET.ATG" 
				exitType = ExitType.Sub; 
				break;
			}
			case 127: {
				lexer.NextToken();

#line  2983 "VBNET.ATG" 
				exitType = ExitType.Function; 
				break;
			}
			case 186: {
				lexer.NextToken();

#line  2985 "VBNET.ATG" 
				exitType = ExitType.Property; 
				break;
			}
			case 108: {
				lexer.NextToken();

#line  2987 "VBNET.ATG" 
				exitType = ExitType.Do; 
				break;
			}
			case 124: {
				lexer.NextToken();

#line  2989 "VBNET.ATG" 
				exitType = ExitType.For; 
				break;
			}
			case 218: {
				lexer.NextToken();

#line  2991 "VBNET.ATG" 
				exitType = ExitType.Try; 
				break;
			}
			case 231: {
				lexer.NextToken();

#line  2993 "VBNET.ATG" 
				exitType = ExitType.While; 
				break;
			}
			case 197: {
				lexer.NextToken();

#line  2995 "VBNET.ATG" 
				exitType = ExitType.Select; 
				break;
			}
			default: SynErr(295); break;
			}

#line  2997 "VBNET.ATG" 
			statement = new ExitStatement(exitType); 
		} else if (la.kind == 218) {
			TryStatement(
#line  2998 "VBNET.ATG" 
out statement);
		} else if (la.kind == 89) {
			lexer.NextToken();

#line  2999 "VBNET.ATG" 
			ContinueType continueType = ContinueType.None; 
			if (la.kind == 108 || la.kind == 124 || la.kind == 231) {
				if (la.kind == 108) {
					lexer.NextToken();

#line  2999 "VBNET.ATG" 
					continueType = ContinueType.Do; 
				} else if (la.kind == 124) {
					lexer.NextToken();

#line  2999 "VBNET.ATG" 
					continueType = ContinueType.For; 
				} else {
					lexer.NextToken();

#line  2999 "VBNET.ATG" 
					continueType = ContinueType.While; 
				}
			}

#line  2999 "VBNET.ATG" 
			statement = new ContinueStatement(continueType); 
		} else if (la.kind == 215) {
			lexer.NextToken();
			if (StartOf(30)) {
				Expr(
#line  3001 "VBNET.ATG" 
out expr);
			}

#line  3001 "VBNET.ATG" 
			statement = new ThrowStatement(expr); 
		} else if (la.kind == 195) {
			lexer.NextToken();
			if (StartOf(30)) {
				Expr(
#line  3003 "VBNET.ATG" 
out expr);
			}

#line  3003 "VBNET.ATG" 
			statement = new ReturnStatement(expr); 
		} else if (la.kind == 211) {
			lexer.NextToken();
			Expr(
#line  3005 "VBNET.ATG" 
out expr);
			EndOfStmt();
			Block(
#line  3005 "VBNET.ATG" 
out embeddedStatement);
			Expect(113);
			Expect(211);

#line  3006 "VBNET.ATG" 
			statement = new LockStatement(expr, embeddedStatement); 
		} else if (la.kind == 189) {
			lexer.NextToken();
			Identifier();

#line  3008 "VBNET.ATG" 
			name = t.val; 
			if (la.kind == 37) {
				lexer.NextToken();
				if (StartOf(42)) {
					ArgumentList(
#line  3009 "VBNET.ATG" 
out p);
				}
				Expect(38);
			}

#line  3011 "VBNET.ATG" 
			statement = new RaiseEventStatement(name, p);
			
		} else if (la.kind == 233) {
			WithStatement(
#line  3014 "VBNET.ATG" 
out statement);
		} else if (la.kind == 56) {
			lexer.NextToken();

#line  3016 "VBNET.ATG" 
			Expression handlerExpr = null; 
			Expr(
#line  3017 "VBNET.ATG" 
out expr);
			Expect(22);
			Expr(
#line  3017 "VBNET.ATG" 
out handlerExpr);

#line  3019 "VBNET.ATG" 
			statement = new AddHandlerStatement(expr, handlerExpr);
			
		} else if (la.kind == 193) {
			lexer.NextToken();

#line  3022 "VBNET.ATG" 
			Expression handlerExpr = null; 
			Expr(
#line  3023 "VBNET.ATG" 
out expr);
			Expect(22);
			Expr(
#line  3023 "VBNET.ATG" 
out handlerExpr);

#line  3025 "VBNET.ATG" 
			statement = new RemoveHandlerStatement(expr, handlerExpr);
			
		} else if (la.kind == 231) {
			lexer.NextToken();
			Expr(
#line  3028 "VBNET.ATG" 
out expr);
			EndOfStmt();
			Block(
#line  3029 "VBNET.ATG" 
out embeddedStatement);
			Expect(113);
			Expect(231);

#line  3031 "VBNET.ATG" 
			statement = new DoLoopStatement(expr, embeddedStatement, ConditionType.While, ConditionPosition.Start);
			
		} else if (la.kind == 108) {
			lexer.NextToken();

#line  3036 "VBNET.ATG" 
			ConditionType conditionType = ConditionType.None;
			
			if (la.kind == 224 || la.kind == 231) {
				WhileOrUntil(
#line  3039 "VBNET.ATG" 
out conditionType);
				Expr(
#line  3039 "VBNET.ATG" 
out expr);
				EndOfStmt();
				Block(
#line  3040 "VBNET.ATG" 
out embeddedStatement);
				Expect(152);

#line  3043 "VBNET.ATG" 
				statement = new DoLoopStatement(expr, 
				                               embeddedStatement, 
				                               conditionType == ConditionType.While ? ConditionType.DoWhile : conditionType, 
				                               ConditionPosition.Start);
				
			} else if (la.kind == 1 || la.kind == 21) {
				EndOfStmt();
				Block(
#line  3050 "VBNET.ATG" 
out embeddedStatement);
				Expect(152);
				if (la.kind == 224 || la.kind == 231) {
					WhileOrUntil(
#line  3051 "VBNET.ATG" 
out conditionType);
					Expr(
#line  3051 "VBNET.ATG" 
out expr);
				}

#line  3053 "VBNET.ATG" 
				statement = new DoLoopStatement(expr, embeddedStatement, conditionType, ConditionPosition.End);
				
			} else SynErr(296);
		} else if (la.kind == 124) {
			lexer.NextToken();

#line  3058 "VBNET.ATG" 
			Expression group = null;
			TypeReference typeReference;
			string        typeName;
			Location startLocation = t.Location;
			
			if (la.kind == 110) {
				lexer.NextToken();
				LoopControlVariable(
#line  3065 "VBNET.ATG" 
out typeReference, out typeName);
				Expect(138);
				Expr(
#line  3066 "VBNET.ATG" 
out group);
				EndOfStmt();
				Block(
#line  3067 "VBNET.ATG" 
out embeddedStatement);
				Expect(163);
				if (StartOf(30)) {
					Expr(
#line  3068 "VBNET.ATG" 
out expr);
				}

#line  3070 "VBNET.ATG" 
				statement = new ForeachStatement(typeReference, 
				                                typeName,
				                                group, 
				                                embeddedStatement, 
				                                expr);
				statement.StartLocation = startLocation;
				statement.EndLocation   = t.EndLocation;
				
				
			} else if (StartOf(43)) {

#line  3081 "VBNET.ATG" 
				Expression start = null;
				Expression end = null;
				Expression step = null;
				Expression variableExpr = null;
				Expression nextExpr = null;
				List<Expression> nextExpressions = null;
				
				if (
#line  3088 "VBNET.ATG" 
IsLoopVariableDeclaration()) {
					LoopControlVariable(
#line  3089 "VBNET.ATG" 
out typeReference, out typeName);
				} else {

#line  3091 "VBNET.ATG" 
					typeReference = null; typeName = null; 
					SimpleExpr(
#line  3092 "VBNET.ATG" 
out variableExpr);
				}
				Expect(20);
				Expr(
#line  3094 "VBNET.ATG" 
out start);
				Expect(216);
				Expr(
#line  3094 "VBNET.ATG" 
out end);
				if (la.kind == 205) {
					lexer.NextToken();
					Expr(
#line  3094 "VBNET.ATG" 
out step);
				}
				EndOfStmt();
				Block(
#line  3095 "VBNET.ATG" 
out embeddedStatement);
				Expect(163);
				if (StartOf(30)) {
					Expr(
#line  3098 "VBNET.ATG" 
out nextExpr);

#line  3100 "VBNET.ATG" 
					nextExpressions = new List<Expression>();
					nextExpressions.Add(nextExpr);
					
					while (la.kind == 22) {
						lexer.NextToken();
						Expr(
#line  3103 "VBNET.ATG" 
out nextExpr);

#line  3103 "VBNET.ATG" 
						nextExpressions.Add(nextExpr); 
					}
				}

#line  3106 "VBNET.ATG" 
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
				
			} else SynErr(297);
		} else if (la.kind == 118) {
			lexer.NextToken();
			Expr(
#line  3119 "VBNET.ATG" 
out expr);

#line  3119 "VBNET.ATG" 
			statement = new ErrorStatement(expr); 
		} else if (la.kind == 191) {
			lexer.NextToken();

#line  3121 "VBNET.ATG" 
			bool isPreserve = false; 
			if (la.kind == 184) {
				lexer.NextToken();

#line  3121 "VBNET.ATG" 
				isPreserve = true; 
			}
			ReDimClause(
#line  3122 "VBNET.ATG" 
out expr);

#line  3124 "VBNET.ATG" 
			ReDimStatement reDimStatement = new ReDimStatement(isPreserve);
			statement = reDimStatement;
			SafeAdd(reDimStatement, reDimStatement.ReDimClauses, expr as InvocationExpression);
			
			while (la.kind == 22) {
				lexer.NextToken();
				ReDimClause(
#line  3128 "VBNET.ATG" 
out expr);

#line  3129 "VBNET.ATG" 
				SafeAdd(reDimStatement, reDimStatement.ReDimClauses, expr as InvocationExpression); 
			}
		} else if (la.kind == 117) {
			lexer.NextToken();
			Expr(
#line  3133 "VBNET.ATG" 
out expr);

#line  3135 "VBNET.ATG" 
			EraseStatement eraseStatement = new EraseStatement();
			if (expr != null) { SafeAdd(eraseStatement, eraseStatement.Expressions, expr);}
			
			while (la.kind == 22) {
				lexer.NextToken();
				Expr(
#line  3138 "VBNET.ATG" 
out expr);

#line  3138 "VBNET.ATG" 
				if (expr != null) { SafeAdd(eraseStatement, eraseStatement.Expressions, expr); }
			}

#line  3139 "VBNET.ATG" 
			statement = eraseStatement; 
		} else if (la.kind == 206) {
			lexer.NextToken();

#line  3141 "VBNET.ATG" 
			statement = new StopStatement(); 
		} else if (
#line  3143 "VBNET.ATG" 
la.kind == Tokens.If) {
			Expect(135);

#line  3144 "VBNET.ATG" 
			Location ifStartLocation = t.Location; 
			Expr(
#line  3144 "VBNET.ATG" 
out expr);
			if (la.kind == 214) {
				lexer.NextToken();
			}
			if (la.kind == 1 || la.kind == 21) {
				EndOfStmt();
				Block(
#line  3147 "VBNET.ATG" 
out embeddedStatement);

#line  3149 "VBNET.ATG" 
				IfElseStatement ifStatement = new IfElseStatement(expr, embeddedStatement);
				ifStatement.StartLocation = ifStartLocation;
				Location elseIfStart;
				
				while (la.kind == 112 || 
#line  3155 "VBNET.ATG" 
IsElseIf()) {
					if (
#line  3155 "VBNET.ATG" 
IsElseIf()) {
						Expect(111);

#line  3155 "VBNET.ATG" 
						elseIfStart = t.Location; 
						Expect(135);
					} else {
						lexer.NextToken();

#line  3156 "VBNET.ATG" 
						elseIfStart = t.Location; 
					}

#line  3158 "VBNET.ATG" 
					Expression condition = null; Statement block = null; 
					Expr(
#line  3159 "VBNET.ATG" 
out condition);
					if (la.kind == 214) {
						lexer.NextToken();
					}
					EndOfStmt();
					Block(
#line  3160 "VBNET.ATG" 
out block);

#line  3162 "VBNET.ATG" 
					ElseIfSection elseIfSection = new ElseIfSection(condition, block);
					elseIfSection.StartLocation = elseIfStart;
					elseIfSection.EndLocation = t.Location;
					elseIfSection.Parent = ifStatement;
					ifStatement.ElseIfSections.Add(elseIfSection);
					
				}
				if (la.kind == 111) {
					lexer.NextToken();
					if (la.kind == 1 || la.kind == 21) {
						EndOfStmt();
					}
					Block(
#line  3171 "VBNET.ATG" 
out embeddedStatement);

#line  3173 "VBNET.ATG" 
					ifStatement.FalseStatement.Add(embeddedStatement);
					
				}
				Expect(113);
				Expect(135);

#line  3177 "VBNET.ATG" 
				ifStatement.EndLocation = t.Location;
				statement = ifStatement;
				
			} else if (StartOf(44)) {

#line  3182 "VBNET.ATG" 
				IfElseStatement ifStatement = new IfElseStatement(expr);
				ifStatement.StartLocation = ifStartLocation;
				
				SingleLineStatementList(
#line  3185 "VBNET.ATG" 
ifStatement.TrueStatement);
				if (la.kind == 111) {
					lexer.NextToken();
					if (StartOf(44)) {
						SingleLineStatementList(
#line  3188 "VBNET.ATG" 
ifStatement.FalseStatement);
					}
				}

#line  3190 "VBNET.ATG" 
				ifStatement.EndLocation = t.Location; statement = ifStatement; 
			} else SynErr(298);
		} else if (la.kind == 197) {
			lexer.NextToken();
			if (la.kind == 74) {
				lexer.NextToken();
			}
			Expr(
#line  3193 "VBNET.ATG" 
out expr);
			EndOfStmt();

#line  3194 "VBNET.ATG" 
			List<SwitchSection> selectSections = new List<SwitchSection>();
			Statement block = null;
			
			while (la.kind == 74) {

#line  3198 "VBNET.ATG" 
				List<CaseLabel> caseClauses = null; Location caseLocation = la.Location; 
				lexer.NextToken();
				CaseClauses(
#line  3199 "VBNET.ATG" 
out caseClauses);
				if (
#line  3199 "VBNET.ATG" 
IsNotStatementSeparator()) {
					lexer.NextToken();
				}
				EndOfStmt();

#line  3201 "VBNET.ATG" 
				SwitchSection selectSection = new SwitchSection(caseClauses);
				selectSection.StartLocation = caseLocation;
				
				Block(
#line  3204 "VBNET.ATG" 
out block);

#line  3206 "VBNET.ATG" 
				selectSection.Children = block.Children;
				selectSection.EndLocation = t.EndLocation;
				selectSections.Add(selectSection);
				
			}

#line  3212 "VBNET.ATG" 
			statement = new SwitchStatement(expr, selectSections);
			
			Expect(113);
			Expect(197);
		} else if (la.kind == 171) {

#line  3215 "VBNET.ATG" 
			OnErrorStatement onErrorStatement = null; 
			OnErrorStatement(
#line  3216 "VBNET.ATG" 
out onErrorStatement);

#line  3216 "VBNET.ATG" 
			statement = onErrorStatement; 
		} else if (la.kind == 132) {

#line  3217 "VBNET.ATG" 
			GotoStatement goToStatement = null; 
			GotoStatement(
#line  3218 "VBNET.ATG" 
out goToStatement);

#line  3218 "VBNET.ATG" 
			statement = goToStatement; 
		} else if (la.kind == 194) {

#line  3219 "VBNET.ATG" 
			ResumeStatement resumeStatement = null; 
			ResumeStatement(
#line  3220 "VBNET.ATG" 
out resumeStatement);

#line  3220 "VBNET.ATG" 
			statement = resumeStatement; 
		} else if (StartOf(43)) {

#line  3223 "VBNET.ATG" 
			Expression val = null;
			AssignmentOperatorType op;
			
			bool mustBeAssignment = la.kind == Tokens.Plus  || la.kind == Tokens.Minus ||
			                        la.kind == Tokens.Not   || la.kind == Tokens.Times;
			
			SimpleExpr(
#line  3229 "VBNET.ATG" 
out expr);
			if (StartOf(45)) {
				AssignmentOperator(
#line  3231 "VBNET.ATG" 
out op);
				Expr(
#line  3231 "VBNET.ATG" 
out val);

#line  3231 "VBNET.ATG" 
				expr = new AssignmentExpression(expr, op, val); 
			} else if (StartOf(46)) {

#line  3232 "VBNET.ATG" 
				if (mustBeAssignment) Error("error in assignment."); 
			} else SynErr(299);

#line  3235 "VBNET.ATG" 
			// a field reference expression that stands alone is a
			// invocation expression without parantheses and arguments
			if(expr is MemberReferenceExpression || expr is IdentifierExpression) {
				expr = new InvocationExpression(expr);
			}
			statement = new ExpressionStatement(expr);
			
		} else if (la.kind == 73) {
			lexer.NextToken();
			SimpleExpr(
#line  3242 "VBNET.ATG" 
out expr);

#line  3242 "VBNET.ATG" 
			statement = new ExpressionStatement(expr); 
		} else if (la.kind == 226) {
			lexer.NextToken();

#line  3244 "VBNET.ATG" 
			Statement block;  
			if (
#line  3245 "VBNET.ATG" 
Peek(1).kind == Tokens.As) {

#line  3246 "VBNET.ATG" 
				LocalVariableDeclaration resourceAquisition = new LocalVariableDeclaration(Modifiers.None); 
				VariableDeclarator(
#line  3247 "VBNET.ATG" 
resourceAquisition.Variables);
				while (la.kind == 22) {
					lexer.NextToken();
					VariableDeclarator(
#line  3249 "VBNET.ATG" 
resourceAquisition.Variables);
				}
				Block(
#line  3251 "VBNET.ATG" 
out block);

#line  3253 "VBNET.ATG" 
				statement = new UsingStatement(resourceAquisition, block);
				
			} else if (StartOf(30)) {
				Expr(
#line  3255 "VBNET.ATG" 
out expr);
				Block(
#line  3256 "VBNET.ATG" 
out block);

#line  3257 "VBNET.ATG" 
				statement = new UsingStatement(new ExpressionStatement(expr), block); 
			} else SynErr(300);
			Expect(113);
			Expect(226);
		} else if (StartOf(47)) {
			LocalDeclarationStatement(
#line  3260 "VBNET.ATG" 
out statement);
		} else SynErr(301);
	}

	void FromOrAggregateQueryOperator(
#line  2228 "VBNET.ATG" 
List<QueryExpressionClause> middleClauses) {

#line  2230 "VBNET.ATG" 
		QueryExpressionFromClause fromClause = null;
		QueryExpressionAggregateClause aggregateClause = null;
		
		if (la.kind == 126) {
			FromQueryOperator(
#line  2233 "VBNET.ATG" 
out fromClause);

#line  2234 "VBNET.ATG" 
			middleClauses.Add(fromClause); 
		} else if (la.kind == 58) {
			AggregateQueryOperator(
#line  2235 "VBNET.ATG" 
out aggregateClause);

#line  2236 "VBNET.ATG" 
			middleClauses.Add(aggregateClause); 
		} else SynErr(302);
	}

	void QueryOperator(
#line  2239 "VBNET.ATG" 
List<QueryExpressionClause> middleClauses) {

#line  2241 "VBNET.ATG" 
		QueryExpressionJoinVBClause joinClause = null;
		QueryExpressionGroupVBClause groupByClause = null;
		QueryExpressionPartitionVBClause partitionClause = null;
		QueryExpressionGroupJoinVBClause groupJoinClause = null;
		QueryExpressionFromClause fromClause = null;
		QueryExpressionAggregateClause aggregateClause = null;
		
		if (la.kind == 126) {
			FromQueryOperator(
#line  2248 "VBNET.ATG" 
out fromClause);

#line  2249 "VBNET.ATG" 
			middleClauses.Add(fromClause); 
		} else if (la.kind == 58) {
			AggregateQueryOperator(
#line  2250 "VBNET.ATG" 
out aggregateClause);

#line  2251 "VBNET.ATG" 
			middleClauses.Add(aggregateClause); 
		} else if (la.kind == 197) {
			SelectQueryOperator(
#line  2252 "VBNET.ATG" 
middleClauses);
		} else if (la.kind == 107) {
			DistinctQueryOperator(
#line  2253 "VBNET.ATG" 
middleClauses);
		} else if (la.kind == 230) {
			WhereQueryOperator(
#line  2254 "VBNET.ATG" 
middleClauses);
		} else if (la.kind == 176) {
			OrderByQueryOperator(
#line  2255 "VBNET.ATG" 
middleClauses);
		} else if (la.kind == 203 || la.kind == 212) {
			PartitionQueryOperator(
#line  2256 "VBNET.ATG" 
out partitionClause);

#line  2257 "VBNET.ATG" 
			middleClauses.Add(partitionClause); 
		} else if (la.kind == 148) {
			LetQueryOperator(
#line  2258 "VBNET.ATG" 
middleClauses);
		} else if (la.kind == 146) {
			JoinQueryOperator(
#line  2259 "VBNET.ATG" 
out joinClause);

#line  2260 "VBNET.ATG" 
			middleClauses.Add(joinClause); 
		} else if (
#line  2261 "VBNET.ATG" 
la.kind == Tokens.Group && Peek(1).kind == Tokens.Join) {
			GroupJoinQueryOperator(
#line  2261 "VBNET.ATG" 
out groupJoinClause);

#line  2262 "VBNET.ATG" 
			middleClauses.Add(groupJoinClause); 
		} else if (la.kind == 133) {
			GroupByQueryOperator(
#line  2263 "VBNET.ATG" 
out groupByClause);

#line  2264 "VBNET.ATG" 
			middleClauses.Add(groupByClause); 
		} else SynErr(303);
	}

	void FromQueryOperator(
#line  2339 "VBNET.ATG" 
out QueryExpressionFromClause fromClause) {

#line  2341 "VBNET.ATG" 
		fromClause = new QueryExpressionFromClause();
		fromClause.StartLocation = la.Location;
		
		Expect(126);
		CollectionRangeVariableDeclarationList(
#line  2344 "VBNET.ATG" 
fromClause.Sources);

#line  2346 "VBNET.ATG" 
		fromClause.EndLocation = t.EndLocation;
		
	}

	void AggregateQueryOperator(
#line  2408 "VBNET.ATG" 
out QueryExpressionAggregateClause aggregateClause) {

#line  2410 "VBNET.ATG" 
		aggregateClause = new QueryExpressionAggregateClause();
		aggregateClause.IntoVariables = new List<ExpressionRangeVariable>();
		aggregateClause.StartLocation = la.Location;
		CollectionRangeVariable source;
		
		Expect(58);
		CollectionRangeVariableDeclaration(
#line  2415 "VBNET.ATG" 
out source);

#line  2417 "VBNET.ATG" 
		aggregateClause.Source = source;
		
		while (StartOf(31)) {
			QueryOperator(
#line  2420 "VBNET.ATG" 
aggregateClause.MiddleClauses);
		}
		Expect(143);
		ExpressionRangeVariableDeclarationList(
#line  2422 "VBNET.ATG" 
aggregateClause.IntoVariables);

#line  2424 "VBNET.ATG" 
		aggregateClause.EndLocation = t.EndLocation;
		
	}

	void SelectQueryOperator(
#line  2350 "VBNET.ATG" 
List<QueryExpressionClause> middleClauses) {

#line  2352 "VBNET.ATG" 
		QueryExpressionSelectVBClause selectClause = new QueryExpressionSelectVBClause();
		selectClause.StartLocation = la.Location;
		
		Expect(197);
		ExpressionRangeVariableDeclarationList(
#line  2355 "VBNET.ATG" 
selectClause.Variables);

#line  2357 "VBNET.ATG" 
		selectClause.EndLocation = t.Location;
		middleClauses.Add(selectClause);
		
	}

	void DistinctQueryOperator(
#line  2362 "VBNET.ATG" 
List<QueryExpressionClause> middleClauses) {

#line  2364 "VBNET.ATG" 
		QueryExpressionDistinctClause distinctClause = new QueryExpressionDistinctClause();
		distinctClause.StartLocation = la.Location;
		
		Expect(107);

#line  2369 "VBNET.ATG" 
		distinctClause.EndLocation = t.EndLocation;
		middleClauses.Add(distinctClause);
		
	}

	void WhereQueryOperator(
#line  2374 "VBNET.ATG" 
List<QueryExpressionClause> middleClauses) {

#line  2376 "VBNET.ATG" 
		QueryExpressionWhereClause whereClause = new QueryExpressionWhereClause();
		whereClause.StartLocation = la.Location;
		Expression operand = null;
		
		Expect(230);
		Expr(
#line  2380 "VBNET.ATG" 
out operand);

#line  2382 "VBNET.ATG" 
		whereClause.Condition = operand;
		whereClause.EndLocation = t.EndLocation;
		
		middleClauses.Add(whereClause);
		
	}

	void OrderByQueryOperator(
#line  2267 "VBNET.ATG" 
List<QueryExpressionClause> middleClauses) {

#line  2269 "VBNET.ATG" 
		QueryExpressionOrderClause orderClause = new QueryExpressionOrderClause();
		orderClause.StartLocation = la.Location;
		List<QueryExpressionOrdering> orderings = null;
		
		Expect(176);
		Expect(70);
		OrderExpressionList(
#line  2273 "VBNET.ATG" 
out orderings);

#line  2275 "VBNET.ATG" 
		orderClause.Orderings = orderings;
		orderClause.EndLocation = t.EndLocation;
		middleClauses.Add(orderClause);
		
	}

	void PartitionQueryOperator(
#line  2389 "VBNET.ATG" 
out QueryExpressionPartitionVBClause partitionClause) {

#line  2391 "VBNET.ATG" 
		partitionClause = new QueryExpressionPartitionVBClause();
		partitionClause.StartLocation = la.Location;
		Expression expr = null;
		
		if (la.kind == 212) {
			lexer.NextToken();

#line  2396 "VBNET.ATG" 
			partitionClause.PartitionType = QueryExpressionPartitionType.Take; 
			if (la.kind == 231) {
				lexer.NextToken();

#line  2397 "VBNET.ATG" 
				partitionClause.PartitionType = QueryExpressionPartitionType.TakeWhile; 
			}
		} else if (la.kind == 203) {
			lexer.NextToken();

#line  2398 "VBNET.ATG" 
			partitionClause.PartitionType = QueryExpressionPartitionType.Skip; 
			if (la.kind == 231) {
				lexer.NextToken();

#line  2399 "VBNET.ATG" 
				partitionClause.PartitionType = QueryExpressionPartitionType.SkipWhile; 
			}
		} else SynErr(304);
		Expr(
#line  2401 "VBNET.ATG" 
out expr);

#line  2403 "VBNET.ATG" 
		partitionClause.Expression = expr;
		partitionClause.EndLocation = t.EndLocation;
		
	}

	void LetQueryOperator(
#line  2428 "VBNET.ATG" 
List<QueryExpressionClause> middleClauses) {

#line  2430 "VBNET.ATG" 
		QueryExpressionLetVBClause letClause = new QueryExpressionLetVBClause();
		letClause.StartLocation = la.Location;
		
		Expect(148);
		ExpressionRangeVariableDeclarationList(
#line  2433 "VBNET.ATG" 
letClause.Variables);

#line  2435 "VBNET.ATG" 
		letClause.EndLocation = t.EndLocation;
		middleClauses.Add(letClause);
		
	}

	void JoinQueryOperator(
#line  2472 "VBNET.ATG" 
out QueryExpressionJoinVBClause joinClause) {

#line  2474 "VBNET.ATG" 
		joinClause = new QueryExpressionJoinVBClause();
		joinClause.StartLocation = la.Location;
		CollectionRangeVariable joinVariable = null;
		QueryExpressionJoinVBClause subJoin = null;
		QueryExpressionJoinConditionVB condition = null;
		
		
		Expect(146);
		CollectionRangeVariableDeclaration(
#line  2481 "VBNET.ATG" 
out joinVariable);

#line  2482 "VBNET.ATG" 
		joinClause.JoinVariable = joinVariable; 
		if (la.kind == 146) {
			JoinQueryOperator(
#line  2484 "VBNET.ATG" 
out subJoin);

#line  2485 "VBNET.ATG" 
			joinClause.SubJoin = subJoin; 
		}
		Expect(171);
		JoinCondition(
#line  2488 "VBNET.ATG" 
out condition);

#line  2489 "VBNET.ATG" 
		SafeAdd(joinClause, joinClause.Conditions, condition); 
		while (la.kind == 60) {
			lexer.NextToken();
			JoinCondition(
#line  2491 "VBNET.ATG" 
out condition);

#line  2492 "VBNET.ATG" 
			SafeAdd(joinClause, joinClause.Conditions, condition); 
		}

#line  2495 "VBNET.ATG" 
		joinClause.EndLocation = t.EndLocation;
		
	}

	void GroupJoinQueryOperator(
#line  2325 "VBNET.ATG" 
out QueryExpressionGroupJoinVBClause groupJoinClause) {

#line  2327 "VBNET.ATG" 
		groupJoinClause = new QueryExpressionGroupJoinVBClause();
		groupJoinClause.StartLocation = la.Location;
		QueryExpressionJoinVBClause joinClause = null;
		
		Expect(133);
		JoinQueryOperator(
#line  2331 "VBNET.ATG" 
out joinClause);
		Expect(143);
		ExpressionRangeVariableDeclarationList(
#line  2332 "VBNET.ATG" 
groupJoinClause.IntoVariables);

#line  2334 "VBNET.ATG" 
		groupJoinClause.JoinClause = joinClause;
		groupJoinClause.EndLocation = t.EndLocation;
		
	}

	void GroupByQueryOperator(
#line  2312 "VBNET.ATG" 
out QueryExpressionGroupVBClause groupByClause) {

#line  2314 "VBNET.ATG" 
		groupByClause = new QueryExpressionGroupVBClause();
		groupByClause.StartLocation = la.Location;
		
		Expect(133);
		ExpressionRangeVariableDeclarationList(
#line  2317 "VBNET.ATG" 
groupByClause.GroupVariables);
		Expect(70);
		ExpressionRangeVariableDeclarationList(
#line  2318 "VBNET.ATG" 
groupByClause.ByVariables);
		Expect(143);
		ExpressionRangeVariableDeclarationList(
#line  2319 "VBNET.ATG" 
groupByClause.IntoVariables);

#line  2321 "VBNET.ATG" 
		groupByClause.EndLocation = t.EndLocation;
		
	}

	void OrderExpressionList(
#line  2281 "VBNET.ATG" 
out List<QueryExpressionOrdering> orderings) {

#line  2283 "VBNET.ATG" 
		orderings = new List<QueryExpressionOrdering>();
		QueryExpressionOrdering ordering = null;
		
		OrderExpression(
#line  2286 "VBNET.ATG" 
out ordering);

#line  2287 "VBNET.ATG" 
		orderings.Add(ordering); 
		while (la.kind == 22) {
			lexer.NextToken();
			OrderExpression(
#line  2289 "VBNET.ATG" 
out ordering);

#line  2290 "VBNET.ATG" 
			orderings.Add(ordering); 
		}
	}

	void OrderExpression(
#line  2294 "VBNET.ATG" 
out QueryExpressionOrdering ordering) {

#line  2296 "VBNET.ATG" 
		ordering = new QueryExpressionOrdering();
		ordering.StartLocation = la.Location;
		ordering.Direction = QueryExpressionOrderingDirection.None;
		Expression orderExpr = null;
		
		Expr(
#line  2301 "VBNET.ATG" 
out orderExpr);

#line  2303 "VBNET.ATG" 
		ordering.Criteria = orderExpr;
		
		if (la.kind == 64 || la.kind == 104) {
			if (la.kind == 64) {
				lexer.NextToken();

#line  2306 "VBNET.ATG" 
				ordering.Direction = QueryExpressionOrderingDirection.Ascending; 
			} else {
				lexer.NextToken();

#line  2307 "VBNET.ATG" 
				ordering.Direction = QueryExpressionOrderingDirection.Descending; 
			}
		}

#line  2309 "VBNET.ATG" 
		ordering.EndLocation = t.EndLocation; 
	}

	void ExpressionRangeVariableDeclarationList(
#line  2440 "VBNET.ATG" 
List<ExpressionRangeVariable> variables) {

#line  2442 "VBNET.ATG" 
		ExpressionRangeVariable variable = null;
		
		ExpressionRangeVariableDeclaration(
#line  2444 "VBNET.ATG" 
out variable);

#line  2445 "VBNET.ATG" 
		variables.Add(variable); 
		while (la.kind == 22) {
			lexer.NextToken();
			ExpressionRangeVariableDeclaration(
#line  2446 "VBNET.ATG" 
out variable);

#line  2446 "VBNET.ATG" 
			variables.Add(variable); 
		}
	}

	void CollectionRangeVariableDeclarationList(
#line  2499 "VBNET.ATG" 
List<CollectionRangeVariable> rangeVariables) {

#line  2500 "VBNET.ATG" 
		CollectionRangeVariable variableDeclaration; 
		CollectionRangeVariableDeclaration(
#line  2502 "VBNET.ATG" 
out variableDeclaration);

#line  2503 "VBNET.ATG" 
		rangeVariables.Add(variableDeclaration); 
		while (la.kind == 22) {
			lexer.NextToken();
			CollectionRangeVariableDeclaration(
#line  2504 "VBNET.ATG" 
out variableDeclaration);

#line  2504 "VBNET.ATG" 
			rangeVariables.Add(variableDeclaration); 
		}
	}

	void CollectionRangeVariableDeclaration(
#line  2507 "VBNET.ATG" 
out CollectionRangeVariable rangeVariable) {

#line  2509 "VBNET.ATG" 
		rangeVariable = new CollectionRangeVariable();
		rangeVariable.StartLocation = la.Location;
		TypeReference typeName = null;
		Expression inExpr = null;
		
		Identifier();

#line  2514 "VBNET.ATG" 
		rangeVariable.Identifier = t.val; 
		if (la.kind == 63) {
			lexer.NextToken();
			TypeName(
#line  2515 "VBNET.ATG" 
out typeName);

#line  2515 "VBNET.ATG" 
			rangeVariable.Type = typeName; 
		}
		Expect(138);
		Expr(
#line  2516 "VBNET.ATG" 
out inExpr);

#line  2518 "VBNET.ATG" 
		rangeVariable.Expression = inExpr;
		rangeVariable.EndLocation = t.EndLocation;
		
	}

	void ExpressionRangeVariableDeclaration(
#line  2449 "VBNET.ATG" 
out ExpressionRangeVariable variable) {

#line  2451 "VBNET.ATG" 
		variable = new ExpressionRangeVariable();
		variable.StartLocation = la.Location;
		Expression rhs = null;
		TypeReference typeName = null;
		
		if (
#line  2457 "VBNET.ATG" 
IsIdentifiedExpressionRange()) {
			Identifier();

#line  2458 "VBNET.ATG" 
			variable.Identifier = t.val; 
			if (la.kind == 63) {
				lexer.NextToken();
				TypeName(
#line  2460 "VBNET.ATG" 
out typeName);

#line  2461 "VBNET.ATG" 
				variable.Type = typeName; 
			}
			Expect(20);
		}
		Expr(
#line  2465 "VBNET.ATG" 
out rhs);

#line  2467 "VBNET.ATG" 
		variable.Expression = rhs;
		variable.EndLocation = t.EndLocation;
		
	}

	void JoinCondition(
#line  2523 "VBNET.ATG" 
out QueryExpressionJoinConditionVB condition) {

#line  2525 "VBNET.ATG" 
		condition = new QueryExpressionJoinConditionVB();
		condition.StartLocation = la.Location;
		
		Expression lhs = null;
		Expression rhs = null;
		
		Expr(
#line  2531 "VBNET.ATG" 
out lhs);
		Expect(116);
		Expr(
#line  2531 "VBNET.ATG" 
out rhs);

#line  2533 "VBNET.ATG" 
		condition.LeftSide = lhs;
		condition.RightSide = rhs;
		condition.EndLocation = t.EndLocation;
		
	}

	void Argument(
#line  2597 "VBNET.ATG" 
out Expression argumentexpr) {

#line  2599 "VBNET.ATG" 
		Expression expr;
		argumentexpr = null;
		string name;
		
		if (
#line  2603 "VBNET.ATG" 
IsNamedAssign()) {
			Identifier();

#line  2603 "VBNET.ATG" 
			name = t.val;  
			Expect(55);
			Expr(
#line  2603 "VBNET.ATG" 
out expr);

#line  2605 "VBNET.ATG" 
			argumentexpr = new NamedArgumentExpression(name, expr);
			
		} else if (StartOf(30)) {
			Expr(
#line  2608 "VBNET.ATG" 
out argumentexpr);
		} else SynErr(305);
	}

	void QualIdentAndTypeArguments(
#line  2674 "VBNET.ATG" 
out TypeReference typeref, bool canBeUnbound) {

#line  2675 "VBNET.ATG" 
		string name; typeref = null; 
		Qualident(
#line  2677 "VBNET.ATG" 
out name);

#line  2678 "VBNET.ATG" 
		typeref = new TypeReference(name); 
		if (
#line  2679 "VBNET.ATG" 
la.kind == Tokens.OpenParenthesis && Peek(1).kind == Tokens.Of) {
			lexer.NextToken();
			Expect(169);
			if (
#line  2681 "VBNET.ATG" 
canBeUnbound && (la.kind == Tokens.CloseParenthesis || la.kind == Tokens.Comma)) {

#line  2682 "VBNET.ATG" 
				typeref.GenericTypes.Add(NullTypeReference.Instance); 
				while (la.kind == 22) {
					lexer.NextToken();

#line  2683 "VBNET.ATG" 
					typeref.GenericTypes.Add(NullTypeReference.Instance); 
				}
			} else if (StartOf(8)) {
				TypeArgumentList(
#line  2684 "VBNET.ATG" 
typeref.GenericTypes);
			} else SynErr(306);
			Expect(38);
		}
	}

	void RankList(
#line  2721 "VBNET.ATG" 
out int i) {

#line  2722 "VBNET.ATG" 
		i = 0; 
		while (la.kind == 22) {
			lexer.NextToken();

#line  2723 "VBNET.ATG" 
			++i; 
		}
	}

	void Attribute(
#line  2762 "VBNET.ATG" 
out ASTAttribute attribute) {

#line  2763 "VBNET.ATG" 
		string name;
		List<Expression> positional = new List<Expression>();
		List<NamedArgumentExpression> named = new List<NamedArgumentExpression>();
		
		if (la.kind == 130) {
			lexer.NextToken();
			Expect(26);
		}
		Qualident(
#line  2768 "VBNET.ATG" 
out name);
		if (la.kind == 37) {
			AttributeArguments(
#line  2769 "VBNET.ATG" 
positional, named);
		}

#line  2771 "VBNET.ATG" 
		attribute  = new ASTAttribute(name, positional, named);
		
	}

	void AttributeArguments(
#line  2776 "VBNET.ATG" 
List<Expression> positional, List<NamedArgumentExpression> named) {

#line  2778 "VBNET.ATG" 
		bool nameFound = false;
		string name = "";
		Expression expr;
		
		Expect(37);
		if (
#line  2784 "VBNET.ATG" 
IsNotClosingParenthesis()) {
			if (
#line  2786 "VBNET.ATG" 
IsNamedAssign()) {

#line  2786 "VBNET.ATG" 
				nameFound = true; 
				IdentifierOrKeyword(
#line  2787 "VBNET.ATG" 
out name);
				if (la.kind == 55) {
					lexer.NextToken();
				} else if (la.kind == 20) {
					lexer.NextToken();
				} else SynErr(307);
			}
			Expr(
#line  2789 "VBNET.ATG" 
out expr);

#line  2791 "VBNET.ATG" 
			if (expr != null) {
			if (string.IsNullOrEmpty(name)) { positional.Add(expr); }
			else { named.Add(new NamedArgumentExpression(name, expr)); name = ""; }
			}
			
			while (la.kind == 22) {
				lexer.NextToken();
				if (
#line  2799 "VBNET.ATG" 
IsNamedAssign()) {

#line  2799 "VBNET.ATG" 
					nameFound = true; 
					IdentifierOrKeyword(
#line  2800 "VBNET.ATG" 
out name);
					if (la.kind == 55) {
						lexer.NextToken();
					} else if (la.kind == 20) {
						lexer.NextToken();
					} else SynErr(308);
				} else if (StartOf(30)) {

#line  2802 "VBNET.ATG" 
					if (nameFound) Error("no positional argument after named argument"); 
				} else SynErr(309);
				Expr(
#line  2803 "VBNET.ATG" 
out expr);

#line  2803 "VBNET.ATG" 
				if (expr != null) { if(name == "") positional.Add(expr);
				else { named.Add(new NamedArgumentExpression(name, expr)); name = ""; }
				}
				
			}
		}
		Expect(38);
	}

	void FormalParameter(
#line  2860 "VBNET.ATG" 
out ParameterDeclarationExpression p) {

#line  2862 "VBNET.ATG" 
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		TypeReference type = null;
		ParamModifierList mod = new ParamModifierList(this);
		Expression expr = null;
		p = null;
		ArrayList arrayModifiers = null;
		
		while (la.kind == 40) {
			AttributeSection(
#line  2871 "VBNET.ATG" 
out section);

#line  2871 "VBNET.ATG" 
			attributes.Add(section); 
		}
		while (StartOf(48)) {
			ParameterModifier(
#line  2872 "VBNET.ATG" 
mod);
		}
		Identifier();

#line  2873 "VBNET.ATG" 
		string parameterName = t.val; 
		if (
#line  2874 "VBNET.ATG" 
IsDims()) {
			ArrayTypeModifiers(
#line  2874 "VBNET.ATG" 
out arrayModifiers);
		}
		if (la.kind == 63) {
			lexer.NextToken();
			TypeName(
#line  2875 "VBNET.ATG" 
out type);
		}

#line  2877 "VBNET.ATG" 
		if(type != null) {
		if (arrayModifiers != null) {
			if (type.RankSpecifier != null) {
				Error("array rank only allowed one time");
			} else {
				type.RankSpecifier = (int[])arrayModifiers.ToArray(typeof(int));
			}
		}
		}
		
		if (la.kind == 20) {
			lexer.NextToken();
			Expr(
#line  2887 "VBNET.ATG" 
out expr);
		}

#line  2889 "VBNET.ATG" 
		mod.Check();
		p = new ParameterDeclarationExpression(type, parameterName, mod.Modifier, expr);
		p.Attributes = attributes;
		
	}

	void ParameterModifier(
#line  3573 "VBNET.ATG" 
ParamModifierList m) {
		if (la.kind == 72) {
			lexer.NextToken();

#line  3574 "VBNET.ATG" 
			m.Add(ParameterModifiers.In); 
		} else if (la.kind == 69) {
			lexer.NextToken();

#line  3575 "VBNET.ATG" 
			m.Add(ParameterModifiers.Ref); 
		} else if (la.kind == 174) {
			lexer.NextToken();

#line  3576 "VBNET.ATG" 
			m.Add(ParameterModifiers.Optional); 
		} else if (la.kind == 182) {
			lexer.NextToken();

#line  3577 "VBNET.ATG" 
			m.Add(ParameterModifiers.Params); 
		} else SynErr(310);
	}

	void Statement() {

#line  2918 "VBNET.ATG" 
		Statement stmt = null;
		Location startPos = la.Location;
		string label = String.Empty;
		
		
		if (la.kind == 1 || la.kind == 21) {
		} else if (
#line  2924 "VBNET.ATG" 
IsLabel()) {
			LabelName(
#line  2924 "VBNET.ATG" 
out label);

#line  2926 "VBNET.ATG" 
			AddChild(new LabelStatement(t.val));
			
			Expect(21);
			Statement();
		} else if (StartOf(49)) {
			EmbeddedStatement(
#line  2929 "VBNET.ATG" 
out stmt);

#line  2929 "VBNET.ATG" 
			AddChild(stmt); 
		} else SynErr(311);

#line  2932 "VBNET.ATG" 
		if (stmt != null) {
		stmt.StartLocation = startPos;
		stmt.EndLocation = t.Location;
		}
		
	}

	void LabelName(
#line  3347 "VBNET.ATG" 
out string name) {

#line  3349 "VBNET.ATG" 
		name = String.Empty;
		
		if (StartOf(4)) {
			Identifier();

#line  3351 "VBNET.ATG" 
			name = t.val; 
		} else if (la.kind == 5) {
			lexer.NextToken();

#line  3352 "VBNET.ATG" 
			name = t.val; 
		} else SynErr(312);
	}

	void LocalDeclarationStatement(
#line  2940 "VBNET.ATG" 
out Statement statement) {

#line  2942 "VBNET.ATG" 
		ModifierList m = new ModifierList();
		LocalVariableDeclaration localVariableDeclaration;
		bool dimfound = false;
		
		while (la.kind == 88 || la.kind == 105 || la.kind == 204) {
			if (la.kind == 88) {
				lexer.NextToken();

#line  2948 "VBNET.ATG" 
				m.Add(Modifiers.Const, t.Location); 
			} else if (la.kind == 204) {
				lexer.NextToken();

#line  2949 "VBNET.ATG" 
				m.Add(Modifiers.Static, t.Location); 
			} else {
				lexer.NextToken();

#line  2950 "VBNET.ATG" 
				dimfound = true; 
			}
		}

#line  2953 "VBNET.ATG" 
		if(dimfound && (m.Modifier & Modifiers.Const) != 0) {
		Error("Dim is not allowed on constants.");
		}
		
		if(m.isNone && dimfound == false) {
			Error("Const, Dim or Static expected");
		}
		
		localVariableDeclaration = new LocalVariableDeclaration(m.Modifier);
		localVariableDeclaration.StartLocation = t.Location;
		
		VariableDeclarator(
#line  2964 "VBNET.ATG" 
localVariableDeclaration.Variables);
		while (la.kind == 22) {
			lexer.NextToken();
			VariableDeclarator(
#line  2965 "VBNET.ATG" 
localVariableDeclaration.Variables);
		}

#line  2967 "VBNET.ATG" 
		statement = localVariableDeclaration;
		
	}

	void TryStatement(
#line  3461 "VBNET.ATG" 
out Statement tryStatement) {

#line  3463 "VBNET.ATG" 
		Statement blockStmt = null, finallyStmt = null;List<CatchClause> catchClauses = null;
		
		Expect(218);
		EndOfStmt();
		Block(
#line  3466 "VBNET.ATG" 
out blockStmt);
		if (la.kind == 75 || la.kind == 113 || la.kind == 123) {
			CatchClauses(
#line  3467 "VBNET.ATG" 
out catchClauses);
		}
		if (la.kind == 123) {
			lexer.NextToken();
			EndOfStmt();
			Block(
#line  3468 "VBNET.ATG" 
out finallyStmt);
		}
		Expect(113);
		Expect(218);

#line  3471 "VBNET.ATG" 
		tryStatement = new TryCatchStatement(blockStmt, catchClauses, finallyStmt);
		
	}

	void WithStatement(
#line  3441 "VBNET.ATG" 
out Statement withStatement) {

#line  3443 "VBNET.ATG" 
		Statement blockStmt = null;
		Expression expr = null;
		
		Expect(233);

#line  3446 "VBNET.ATG" 
		Location start = t.Location; 
		Expr(
#line  3447 "VBNET.ATG" 
out expr);
		EndOfStmt();

#line  3449 "VBNET.ATG" 
		withStatement = new WithStatement(expr);
		withStatement.StartLocation = start;
		
		Block(
#line  3452 "VBNET.ATG" 
out blockStmt);

#line  3454 "VBNET.ATG" 
		((WithStatement)withStatement).Body = (BlockStatement)blockStmt;
		
		Expect(113);
		Expect(233);

#line  3457 "VBNET.ATG" 
		withStatement.EndLocation = t.Location; 
	}

	void WhileOrUntil(
#line  3434 "VBNET.ATG" 
out ConditionType conditionType) {

#line  3435 "VBNET.ATG" 
		conditionType = ConditionType.None; 
		if (la.kind == 231) {
			lexer.NextToken();

#line  3436 "VBNET.ATG" 
			conditionType = ConditionType.While; 
		} else if (la.kind == 224) {
			lexer.NextToken();

#line  3437 "VBNET.ATG" 
			conditionType = ConditionType.Until; 
		} else SynErr(313);
	}

	void LoopControlVariable(
#line  3277 "VBNET.ATG" 
out TypeReference type, out string name) {

#line  3278 "VBNET.ATG" 
		ArrayList arrayModifiers = null;
		type = null;
		
		Qualident(
#line  3282 "VBNET.ATG" 
out name);
		if (
#line  3283 "VBNET.ATG" 
IsDims()) {
			ArrayTypeModifiers(
#line  3283 "VBNET.ATG" 
out arrayModifiers);
		}
		if (la.kind == 63) {
			lexer.NextToken();
			TypeName(
#line  3284 "VBNET.ATG" 
out type);

#line  3284 "VBNET.ATG" 
			if (name.IndexOf('.') > 0) { Error("No type def for 'for each' member indexer allowed."); } 
		}

#line  3286 "VBNET.ATG" 
		if (type != null) {
		if(type.RankSpecifier != null && arrayModifiers != null) {
			Error("array rank only allowed one time");
		} else if (arrayModifiers != null) {
			type.RankSpecifier = (int[])arrayModifiers.ToArray(typeof(int));
		}
		}
		
	}

	void ReDimClause(
#line  3356 "VBNET.ATG" 
out Expression expr) {
		SimpleNonInvocationExpression(
#line  3358 "VBNET.ATG" 
out expr);
		ReDimClauseInternal(
#line  3359 "VBNET.ATG" 
ref expr);
	}

	void SingleLineStatementList(
#line  3263 "VBNET.ATG" 
List<Statement> list) {

#line  3264 "VBNET.ATG" 
		Statement embeddedStatement = null; 
		if (la.kind == 113) {
			lexer.NextToken();

#line  3266 "VBNET.ATG" 
			embeddedStatement = new EndStatement(); 
		} else if (StartOf(49)) {
			EmbeddedStatement(
#line  3267 "VBNET.ATG" 
out embeddedStatement);
		} else SynErr(314);

#line  3268 "VBNET.ATG" 
		if (embeddedStatement != null) list.Add(embeddedStatement); 
		while (la.kind == 21) {
			lexer.NextToken();
			while (la.kind == 21) {
				lexer.NextToken();
			}
			if (la.kind == 113) {
				lexer.NextToken();

#line  3270 "VBNET.ATG" 
				embeddedStatement = new EndStatement(); 
			} else if (StartOf(49)) {
				EmbeddedStatement(
#line  3271 "VBNET.ATG" 
out embeddedStatement);
			} else SynErr(315);

#line  3272 "VBNET.ATG" 
			if (embeddedStatement != null) list.Add(embeddedStatement); 
		}
	}

	void CaseClauses(
#line  3394 "VBNET.ATG" 
out List<CaseLabel> caseClauses) {

#line  3396 "VBNET.ATG" 
		caseClauses = new List<CaseLabel>();
		CaseLabel caseClause = null;
		
		CaseClause(
#line  3399 "VBNET.ATG" 
out caseClause);

#line  3399 "VBNET.ATG" 
		if (caseClause != null) { caseClauses.Add(caseClause); } 
		while (la.kind == 22) {
			lexer.NextToken();
			CaseClause(
#line  3400 "VBNET.ATG" 
out caseClause);

#line  3400 "VBNET.ATG" 
			if (caseClause != null) { caseClauses.Add(caseClause); } 
		}
	}

	void OnErrorStatement(
#line  3297 "VBNET.ATG" 
out OnErrorStatement stmt) {

#line  3299 "VBNET.ATG" 
		stmt = null;
		GotoStatement goToStatement = null;
		
		Expect(171);
		Expect(118);
		if (
#line  3305 "VBNET.ATG" 
IsNegativeLabelName()) {
			Expect(132);
			Expect(30);
			Expect(5);

#line  3307 "VBNET.ATG" 
			long intLabel = Int64.Parse(t.val);
			if(intLabel != 1) {
				Error("invalid label in on error statement.");
			}
			stmt = new OnErrorStatement(new GotoStatement((intLabel * -1).ToString()));
			
		} else if (la.kind == 132) {
			GotoStatement(
#line  3313 "VBNET.ATG" 
out goToStatement);

#line  3315 "VBNET.ATG" 
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
			Expect(163);

#line  3329 "VBNET.ATG" 
			stmt = new OnErrorStatement(new ResumeStatement(true));
			
		} else SynErr(316);
	}

	void GotoStatement(
#line  3335 "VBNET.ATG" 
out GotoStatement goToStatement) {

#line  3337 "VBNET.ATG" 
		string label = String.Empty;
		
		Expect(132);
		LabelName(
#line  3340 "VBNET.ATG" 
out label);

#line  3342 "VBNET.ATG" 
		goToStatement = new GotoStatement(label);
		
	}

	void ResumeStatement(
#line  3383 "VBNET.ATG" 
out ResumeStatement resumeStatement) {

#line  3385 "VBNET.ATG" 
		resumeStatement = null;
		string label = String.Empty;
		
		if (
#line  3388 "VBNET.ATG" 
IsResumeNext()) {
			Expect(194);
			Expect(163);

#line  3389 "VBNET.ATG" 
			resumeStatement = new ResumeStatement(true); 
		} else if (la.kind == 194) {
			lexer.NextToken();
			if (StartOf(50)) {
				LabelName(
#line  3390 "VBNET.ATG" 
out label);
			}

#line  3390 "VBNET.ATG" 
			resumeStatement = new ResumeStatement(label); 
		} else SynErr(317);
	}

	void ReDimClauseInternal(
#line  3362 "VBNET.ATG" 
ref Expression expr) {

#line  3363 "VBNET.ATG" 
		List<Expression> arguments; bool canBeNormal; bool canBeRedim; string name; 
		while (la.kind == 26 || 
#line  3366 "VBNET.ATG" 
la.kind == Tokens.OpenParenthesis && Peek(1).kind == Tokens.Of) {
			if (la.kind == 26) {
				lexer.NextToken();
				IdentifierOrKeyword(
#line  3365 "VBNET.ATG" 
out name);

#line  3365 "VBNET.ATG" 
				expr = new MemberReferenceExpression(expr, name); 
			} else {
				InvocationExpression(
#line  3367 "VBNET.ATG" 
ref expr);
			}
		}
		Expect(37);
		NormalOrReDimArgumentList(
#line  3370 "VBNET.ATG" 
out arguments, out canBeNormal, out canBeRedim);
		Expect(38);

#line  3372 "VBNET.ATG" 
		expr = new InvocationExpression(expr, arguments);
		if (canBeRedim == false || canBeNormal && (la.kind == Tokens.Dot || la.kind == Tokens.OpenParenthesis)) {
			if (this.Errors.Count == 0) {
				// don't recurse on parse errors - could result in endless recursion
				ReDimClauseInternal(ref expr);
			}
		}
		
	}

	void CaseClause(
#line  3404 "VBNET.ATG" 
out CaseLabel caseClause) {

#line  3406 "VBNET.ATG" 
		Expression expr = null;
		Expression sexpr = null;
		BinaryOperatorType op = BinaryOperatorType.None;
		caseClause = null;
		
		if (la.kind == 111) {
			lexer.NextToken();

#line  3412 "VBNET.ATG" 
			caseClause = new CaseLabel(); 
		} else if (StartOf(51)) {
			if (la.kind == 144) {
				lexer.NextToken();
			}
			switch (la.kind) {
			case 40: {
				lexer.NextToken();

#line  3416 "VBNET.ATG" 
				op = BinaryOperatorType.LessThan; 
				break;
			}
			case 39: {
				lexer.NextToken();

#line  3417 "VBNET.ATG" 
				op = BinaryOperatorType.GreaterThan; 
				break;
			}
			case 43: {
				lexer.NextToken();

#line  3418 "VBNET.ATG" 
				op = BinaryOperatorType.LessThanOrEqual; 
				break;
			}
			case 42: {
				lexer.NextToken();

#line  3419 "VBNET.ATG" 
				op = BinaryOperatorType.GreaterThanOrEqual; 
				break;
			}
			case 20: {
				lexer.NextToken();

#line  3420 "VBNET.ATG" 
				op = BinaryOperatorType.Equality; 
				break;
			}
			case 41: {
				lexer.NextToken();

#line  3421 "VBNET.ATG" 
				op = BinaryOperatorType.InEquality; 
				break;
			}
			default: SynErr(318); break;
			}
			Expr(
#line  3423 "VBNET.ATG" 
out expr);

#line  3425 "VBNET.ATG" 
			caseClause = new CaseLabel(op, expr);
			
		} else if (StartOf(30)) {
			Expr(
#line  3427 "VBNET.ATG" 
out expr);
			if (la.kind == 216) {
				lexer.NextToken();
				Expr(
#line  3427 "VBNET.ATG" 
out sexpr);
			}

#line  3429 "VBNET.ATG" 
			caseClause = new CaseLabel(expr, sexpr);
			
		} else SynErr(319);
	}

	void CatchClauses(
#line  3476 "VBNET.ATG" 
out List<CatchClause> catchClauses) {

#line  3478 "VBNET.ATG" 
		catchClauses = new List<CatchClause>();
		TypeReference type = null;
		Statement blockStmt = null;
		Expression expr = null;
		string name = String.Empty;
		
		while (la.kind == 75) {
			lexer.NextToken();
			if (StartOf(4)) {
				Identifier();

#line  3486 "VBNET.ATG" 
				name = t.val; 
				if (la.kind == 63) {
					lexer.NextToken();
					TypeName(
#line  3486 "VBNET.ATG" 
out type);
				}
			}
			if (la.kind == 229) {
				lexer.NextToken();
				Expr(
#line  3487 "VBNET.ATG" 
out expr);
			}
			EndOfStmt();
			Block(
#line  3489 "VBNET.ATG" 
out blockStmt);

#line  3490 "VBNET.ATG" 
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
			case 19: s = "XmlProcessingInstruction expected"; break;
			case 20: s = "\"=\" expected"; break;
			case 21: s = "\":\" expected"; break;
			case 22: s = "\",\" expected"; break;
			case 23: s = "\"&\" expected"; break;
			case 24: s = "\"/\" expected"; break;
			case 25: s = "\"\\\\\" expected"; break;
			case 26: s = "\".\" expected"; break;
			case 27: s = "\"...\" expected"; break;
			case 28: s = "\".@\" expected"; break;
			case 29: s = "\"!\" expected"; break;
			case 30: s = "\"-\" expected"; break;
			case 31: s = "\"+\" expected"; break;
			case 32: s = "\"^\" expected"; break;
			case 33: s = "\"?\" expected"; break;
			case 34: s = "\"*\" expected"; break;
			case 35: s = "\"{\" expected"; break;
			case 36: s = "\"}\" expected"; break;
			case 37: s = "\"(\" expected"; break;
			case 38: s = "\")\" expected"; break;
			case 39: s = "\">\" expected"; break;
			case 40: s = "\"<\" expected"; break;
			case 41: s = "\"<>\" expected"; break;
			case 42: s = "\">=\" expected"; break;
			case 43: s = "\"<=\" expected"; break;
			case 44: s = "\"<<\" expected"; break;
			case 45: s = "\">>\" expected"; break;
			case 46: s = "\"+=\" expected"; break;
			case 47: s = "\"^=\" expected"; break;
			case 48: s = "\"-=\" expected"; break;
			case 49: s = "\"*=\" expected"; break;
			case 50: s = "\"/=\" expected"; break;
			case 51: s = "\"\\\\=\" expected"; break;
			case 52: s = "\"<<=\" expected"; break;
			case 53: s = "\">>=\" expected"; break;
			case 54: s = "\"&=\" expected"; break;
			case 55: s = "\":=\" expected"; break;
			case 56: s = "\"AddHandler\" expected"; break;
			case 57: s = "\"AddressOf\" expected"; break;
			case 58: s = "\"Aggregate\" expected"; break;
			case 59: s = "\"Alias\" expected"; break;
			case 60: s = "\"And\" expected"; break;
			case 61: s = "\"AndAlso\" expected"; break;
			case 62: s = "\"Ansi\" expected"; break;
			case 63: s = "\"As\" expected"; break;
			case 64: s = "\"Ascending\" expected"; break;
			case 65: s = "\"Assembly\" expected"; break;
			case 66: s = "\"Auto\" expected"; break;
			case 67: s = "\"Binary\" expected"; break;
			case 68: s = "\"Boolean\" expected"; break;
			case 69: s = "\"ByRef\" expected"; break;
			case 70: s = "\"By\" expected"; break;
			case 71: s = "\"Byte\" expected"; break;
			case 72: s = "\"ByVal\" expected"; break;
			case 73: s = "\"Call\" expected"; break;
			case 74: s = "\"Case\" expected"; break;
			case 75: s = "\"Catch\" expected"; break;
			case 76: s = "\"CBool\" expected"; break;
			case 77: s = "\"CByte\" expected"; break;
			case 78: s = "\"CChar\" expected"; break;
			case 79: s = "\"CDate\" expected"; break;
			case 80: s = "\"CDbl\" expected"; break;
			case 81: s = "\"CDec\" expected"; break;
			case 82: s = "\"Char\" expected"; break;
			case 83: s = "\"CInt\" expected"; break;
			case 84: s = "\"Class\" expected"; break;
			case 85: s = "\"CLng\" expected"; break;
			case 86: s = "\"CObj\" expected"; break;
			case 87: s = "\"Compare\" expected"; break;
			case 88: s = "\"Const\" expected"; break;
			case 89: s = "\"Continue\" expected"; break;
			case 90: s = "\"CSByte\" expected"; break;
			case 91: s = "\"CShort\" expected"; break;
			case 92: s = "\"CSng\" expected"; break;
			case 93: s = "\"CStr\" expected"; break;
			case 94: s = "\"CType\" expected"; break;
			case 95: s = "\"CUInt\" expected"; break;
			case 96: s = "\"CULng\" expected"; break;
			case 97: s = "\"CUShort\" expected"; break;
			case 98: s = "\"Custom\" expected"; break;
			case 99: s = "\"Date\" expected"; break;
			case 100: s = "\"Decimal\" expected"; break;
			case 101: s = "\"Declare\" expected"; break;
			case 102: s = "\"Default\" expected"; break;
			case 103: s = "\"Delegate\" expected"; break;
			case 104: s = "\"Descending\" expected"; break;
			case 105: s = "\"Dim\" expected"; break;
			case 106: s = "\"DirectCast\" expected"; break;
			case 107: s = "\"Distinct\" expected"; break;
			case 108: s = "\"Do\" expected"; break;
			case 109: s = "\"Double\" expected"; break;
			case 110: s = "\"Each\" expected"; break;
			case 111: s = "\"Else\" expected"; break;
			case 112: s = "\"ElseIf\" expected"; break;
			case 113: s = "\"End\" expected"; break;
			case 114: s = "\"EndIf\" expected"; break;
			case 115: s = "\"Enum\" expected"; break;
			case 116: s = "\"Equals\" expected"; break;
			case 117: s = "\"Erase\" expected"; break;
			case 118: s = "\"Error\" expected"; break;
			case 119: s = "\"Event\" expected"; break;
			case 120: s = "\"Exit\" expected"; break;
			case 121: s = "\"Explicit\" expected"; break;
			case 122: s = "\"False\" expected"; break;
			case 123: s = "\"Finally\" expected"; break;
			case 124: s = "\"For\" expected"; break;
			case 125: s = "\"Friend\" expected"; break;
			case 126: s = "\"From\" expected"; break;
			case 127: s = "\"Function\" expected"; break;
			case 128: s = "\"Get\" expected"; break;
			case 129: s = "\"GetType\" expected"; break;
			case 130: s = "\"Global\" expected"; break;
			case 131: s = "\"GoSub\" expected"; break;
			case 132: s = "\"GoTo\" expected"; break;
			case 133: s = "\"Group\" expected"; break;
			case 134: s = "\"Handles\" expected"; break;
			case 135: s = "\"If\" expected"; break;
			case 136: s = "\"Implements\" expected"; break;
			case 137: s = "\"Imports\" expected"; break;
			case 138: s = "\"In\" expected"; break;
			case 139: s = "\"Infer\" expected"; break;
			case 140: s = "\"Inherits\" expected"; break;
			case 141: s = "\"Integer\" expected"; break;
			case 142: s = "\"Interface\" expected"; break;
			case 143: s = "\"Into\" expected"; break;
			case 144: s = "\"Is\" expected"; break;
			case 145: s = "\"IsNot\" expected"; break;
			case 146: s = "\"Join\" expected"; break;
			case 147: s = "\"Key\" expected"; break;
			case 148: s = "\"Let\" expected"; break;
			case 149: s = "\"Lib\" expected"; break;
			case 150: s = "\"Like\" expected"; break;
			case 151: s = "\"Long\" expected"; break;
			case 152: s = "\"Loop\" expected"; break;
			case 153: s = "\"Me\" expected"; break;
			case 154: s = "\"Mod\" expected"; break;
			case 155: s = "\"Module\" expected"; break;
			case 156: s = "\"MustInherit\" expected"; break;
			case 157: s = "\"MustOverride\" expected"; break;
			case 158: s = "\"MyBase\" expected"; break;
			case 159: s = "\"MyClass\" expected"; break;
			case 160: s = "\"Namespace\" expected"; break;
			case 161: s = "\"Narrowing\" expected"; break;
			case 162: s = "\"New\" expected"; break;
			case 163: s = "\"Next\" expected"; break;
			case 164: s = "\"Not\" expected"; break;
			case 165: s = "\"Nothing\" expected"; break;
			case 166: s = "\"NotInheritable\" expected"; break;
			case 167: s = "\"NotOverridable\" expected"; break;
			case 168: s = "\"Object\" expected"; break;
			case 169: s = "\"Of\" expected"; break;
			case 170: s = "\"Off\" expected"; break;
			case 171: s = "\"On\" expected"; break;
			case 172: s = "\"Operator\" expected"; break;
			case 173: s = "\"Option\" expected"; break;
			case 174: s = "\"Optional\" expected"; break;
			case 175: s = "\"Or\" expected"; break;
			case 176: s = "\"Order\" expected"; break;
			case 177: s = "\"OrElse\" expected"; break;
			case 178: s = "\"Out\" expected"; break;
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
			case 274: s = "invalid XmlLiteralExpression"; break;
			case 275: s = "invalid AssignmentOperator"; break;
			case 276: s = "invalid SimpleExpr"; break;
			case 277: s = "invalid SimpleExpr"; break;
			case 278: s = "invalid SimpleNonInvocationExpression"; break;
			case 279: s = "invalid SimpleNonInvocationExpression"; break;
			case 280: s = "invalid SimpleNonInvocationExpression"; break;
			case 281: s = "invalid SimpleNonInvocationExpression"; break;
			case 282: s = "invalid SimpleNonInvocationExpression"; break;
			case 283: s = "invalid SimpleNonInvocationExpression"; break;
			case 284: s = "invalid PrimitiveTypeName"; break;
			case 285: s = "invalid CastTarget"; break;
			case 286: s = "invalid XmlContentExpression"; break;
			case 287: s = "invalid XmlElement"; break;
			case 288: s = "invalid XmlElement"; break;
			case 289: s = "invalid XmlNestedContent"; break;
			case 290: s = "invalid XmlAttribute"; break;
			case 291: s = "invalid XmlAttribute"; break;
			case 292: s = "invalid ComparisonExpr"; break;
			case 293: s = "invalid SubLambdaExpression"; break;
			case 294: s = "invalid FunctionLambdaExpression"; break;
			case 295: s = "invalid EmbeddedStatement"; break;
			case 296: s = "invalid EmbeddedStatement"; break;
			case 297: s = "invalid EmbeddedStatement"; break;
			case 298: s = "invalid EmbeddedStatement"; break;
			case 299: s = "invalid EmbeddedStatement"; break;
			case 300: s = "invalid EmbeddedStatement"; break;
			case 301: s = "invalid EmbeddedStatement"; break;
			case 302: s = "invalid FromOrAggregateQueryOperator"; break;
			case 303: s = "invalid QueryOperator"; break;
			case 304: s = "invalid PartitionQueryOperator"; break;
			case 305: s = "invalid Argument"; break;
			case 306: s = "invalid QualIdentAndTypeArguments"; break;
			case 307: s = "invalid AttributeArguments"; break;
			case 308: s = "invalid AttributeArguments"; break;
			case 309: s = "invalid AttributeArguments"; break;
			case 310: s = "invalid ParameterModifier"; break;
			case 311: s = "invalid Statement"; break;
			case 312: s = "invalid LabelName"; break;
			case 313: s = "invalid WhileOrUntil"; break;
			case 314: s = "invalid SingleLineStatementList"; break;
			case 315: s = "invalid SingleLineStatementList"; break;
			case 316: s = "invalid OnErrorStatement"; break;
			case 317: s = "invalid ResumeStatement"; break;
			case 318: s = "invalid CaseClause"; break;
			case 319: s = "invalid CaseClause"; break;

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
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,x,T, T,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,x,T, T,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,x,T, T,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,T,x, T,T,T,T, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, T,x,x,T, x,x,x,x, x,x,x,x, T,x,x,x, x,T,x,x, x,x,T,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,T, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, T,x,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,T,x, T,T,T,T, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,T, x,x,x,x, x,x,x,x, T,x,x,x, x,T,x,x, x,x,T,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,T, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, T,x,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,T,x, T,T,T,T, x,T,T,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, T,x,x,T, x,x,x,x, x,x,x,x, T,x,x,x, x,T,x,x, x,x,T,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,T, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,T,x, T,x,T,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,T,x, T,T,T,T, T,x,T,T, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,T, x,x,x,x, x,x,x,x, x,x,T,T, T,x,x,x, T,x,x,T, x,T,x,x, x,x,x,x, T,x,x,x, x,T,x,x, x,x,T,x, x,x,T,x, x,T,x,x, x,x,x,T, x,T,x,T, x,x,T,T, x,x,x,T, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, T,x,T,x, x,x,x,x, T,x,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,T,T,T, x,x,x,x, T,T,x,x, T,T,x,x, x,x,x,x, x,T,T,T, T,T,x,x, x,x,T,x, x,x,x,x, x,x,x,x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,T,x, T,T,T,T, T,x,T,T, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,T,T, T,x,x,x, T,x,x,T, x,T,x,x, x,x,x,x, T,x,x,x, x,T,x,x, x,x,T,x, x,x,T,x, x,T,x,x, x,x,x,T, x,T,x,T, x,x,T,T, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,x, x,x,x,x, T,x,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,T,T,T, x,x,x,x, T,x,x,x, T,T,x,x, x,x,x,x, x,T,T,T, T,T,x,x, x,x,T,x, x,x,x,x, x,x,x,x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,T,x, T,T,T,T, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,T, T,x,x,x, x,x,x,x, x,x,T,x, x,T,T,T, T,T,x,T, x,x,x,x, x,x,x,T, T,x,x,T, x,T,x,x, x,T,T,T, x,x,x,x, x,T,x,x, x,x,x,T, x,x,T,T, x,x,T,T, x,x,x,x, x,x,x,T, T,T,x,x, x,T,x,x, x,x,T,T, x,x,T,x, T,x,x,x, T,x,T,T, T,T,x,T, T,T,T,T, T,x,T,x, x,x,x,x, x,x,x,T, T,x,x,T, x,x,x,x, x,T,T,x, T,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,T,x, T,x,T,T, x,x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,T, T,T,x,T, x,T,x,T, T,x,T,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,T,x, T,T,T,T, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, T,x,x,T, x,x,x,x, x,x,x,x, T,x,x,x, x,T,x,x, x,x,T,x, x,x,T,x, x,T,x,x, x,x,x,T, x,x,x,T, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, T,x,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,T, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,T,x, T,T,T,T, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, T,x,x,T, x,x,x,x, x,x,x,x, T,x,x,x, x,T,x,x, x,x,T,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,T, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, T,x,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,T,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,x,x,x, x,T,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,T, T,T,x,T, x,T,T,T, T,x,T,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, T,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,T, T,T,x,T, x,T,T,T, T,x,T,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
	{x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,x,T,T, x,x,T,T, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,T,x, T,T,T,T, T,x,T,T, x,x,x,x, T,T,T,T, T,T,T,T, x,T,T,T, x,x,T,T, T,T,T,T, T,T,T,T, T,x,x,x, T,x,T,T, x,T,x,x, x,x,x,x, T,x,x,x, x,T,T,x, x,x,T,x, x,T,T,x, x,T,x,T, x,x,x,T, x,T,x,T, x,x,T,T, x,x,x,T, x,T,x,x, x,x,T,T, x,x,T,x, T,T,x,x, T,x,T,x, x,x,x,x, T,x,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,T,T,T, x,x,x,x, T,x,x,x, T,T,x,x, x,T,x,T, T,T,T,T, T,T,x,x, x,x,T,x, x,x,x,x, x,x,x,x},
	{x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, T,T,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
	{x,T,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,T,T, T,x,x,x, x,x,x,T, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,T,x, x,x,T,x, T,T,T,T, T,x,T,T, x,T,x,x, T,T,T,T, T,T,T,T, x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x,x, T,T,T,T, T,T,x,x, x,x,x,x, T,T,T,x, T,T,T,x, T,x,T,x, x,T,T,x, T,T,x,T, x,x,x,T, x,T,x,T, x,x,T,T, x,x,x,T, x,T,x,x, x,x,T,T, x,x,T,x, x,T,x,x, T,x,T,T, x,x,x,x, T,x,T,x, x,x,x,x, T,x,x,x, x,T,x,T, x,T,T,T, T,T,x,x, x,T,T,T, T,x,T,x, T,x,x,T, T,T,x,T, x,T,T,T, T,T,T,T, T,T,T,x, x,x,T,T, x,T,x,x, x,x,x,x},
	{x,T,T,T, T,T,T,T, T,T,x,x, x,T,x,x, x,x,x,x, T,T,T,T, T,T,T,T, T,T,T,T, T,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, T,T,T,x, T,T,T,T, T,T,T,T, T,x,T,T, x,T,x,x, T,T,T,T, T,T,T,T, x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x,x, T,T,T,T, T,T,x,T, x,T,x,x, T,T,T,x, T,T,T,x, T,x,T,x, x,T,T,x, T,T,x,T, T,x,x,T, x,T,x,T, T,T,T,T, T,x,T,T, x,T,T,x, x,x,T,T, x,x,T,x, x,T,x,x, T,x,T,T, x,x,x,T, T,T,T,x, x,x,x,x, T,x,x,x, x,T,x,T, x,T,T,T, T,T,x,x, x,T,T,T, T,T,T,x, T,x,x,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, x,x,T,T, x,T,x,x, T,x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, T,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, T,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
	{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, T,T,T,T, x,x,x,x, x,x,T,T, T,x,T,T, x,x,T,T, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,T,x, T,T,T,T, T,x,T,T, x,x,x,x, T,T,T,T, T,T,T,T, x,T,T,T, x,x,T,T, T,T,T,T, T,T,T,T, T,x,x,x, T,x,T,T, x,T,x,x, x,x,x,x, T,x,x,x, x,T,T,x, x,x,T,T, x,T,T,x, x,T,x,T, x,x,x,T, x,T,x,T, x,x,T,T, x,x,x,T, x,T,x,x, x,x,T,T, x,x,T,x, T,T,x,x, T,x,T,x, x,x,x,x, T,x,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,T,T,T, x,x,x,x, T,x,T,x, T,T,x,x, x,T,x,T, T,T,T,T, T,T,x,x, x,x,T,x, x,x,x,x, x,x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
	{x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x},
	{x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,T,x, T,T,T,T, T,x,T,T, x,x,x,x, T,T,T,T, T,T,T,T, x,T,T,T, x,x,T,T, T,T,T,T, T,T,T,T, T,x,x,x, T,x,T,T, x,T,x,x, x,x,x,x, T,x,x,x, x,T,T,x, x,x,T,x, x,T,T,x, x,T,x,T, x,x,x,T, x,T,x,T, x,x,T,T, x,x,x,T, x,T,x,x, x,x,T,T, x,x,T,x, x,T,x,x, T,x,T,x, x,x,x,x, T,x,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,T,T,T, x,x,x,x, T,x,x,x, T,T,x,x, x,T,x,T, T,T,T,T, T,T,x,x, x,x,T,x, x,x,x,x, x,x,x,x},
	{x,x,T,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,T,x, T,T,T,T, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, T,x,x,T, x,x,x,x, x,x,x,x, T,x,x,x, x,T,x,x, x,x,T,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,T, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, T,x,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x},
	{x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,x, T,T,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
	{x,T,T,T, T,T,T,T, T,T,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
	{x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,x,T,T, x,x,T,T, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,T,x, T,T,T,T, T,x,T,T, x,x,x,x, T,T,T,T, T,T,T,T, x,T,T,T, x,x,T,T, T,T,T,T, T,T,T,T, T,x,x,x, T,x,T,T, x,T,x,x, x,x,x,x, T,x,x,x, x,T,T,x, x,x,T,x, x,T,T,x, x,T,x,T, x,x,x,T, x,T,x,T, x,x,T,T, x,x,x,T, x,T,x,x, x,x,T,T, x,x,T,x, x,T,x,x, T,x,T,x, x,x,x,x, T,x,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,T,T,T, x,x,x,x, T,x,x,x, T,T,x,x, x,T,x,T, T,T,T,T, T,T,x,x, x,x,T,x, x,x,x,x, x,x,x,x},
	{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, T,T,T,T, x,x,x,x, x,x,T,T, T,x,T,T, x,x,T,T, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,T,x, x,x,T,x, T,T,T,T, T,x,T,T, x,T,x,x, T,T,T,T, T,T,T,T, x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x,x, T,T,T,T, T,T,x,x, x,x,x,x, T,T,T,x, T,T,T,x, T,x,T,T, x,T,T,x, T,T,x,T, x,x,x,T, x,T,x,T, x,x,T,T, x,x,x,T, x,T,x,x, x,x,T,T, x,x,T,x, T,T,x,x, T,x,T,T, x,x,x,x, T,x,T,x, x,x,x,x, T,x,x,x, x,T,x,T, x,T,T,T, T,T,x,x, x,T,T,T, T,x,T,x, T,x,T,T, T,T,x,T, x,T,T,T, T,T,T,T, T,T,T,x, x,x,T,T, x,T,x,x, x,x,x,x},
	{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, T,T,T,T, x,x,T,x, x,x,T,T, T,x,T,T, x,x,T,T, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,T,x, T,T,T,T, T,x,T,T, x,x,x,x, T,T,T,T, T,T,T,T, x,T,T,T, x,x,T,T, T,T,T,T, T,T,T,T, T,x,x,x, T,x,T,T, x,T,x,x, x,x,x,x, T,x,x,x, x,T,T,x, x,x,T,T, x,T,T,x, x,T,x,T, x,x,x,T, x,T,x,T, x,x,T,T, x,x,x,T, x,T,x,x, x,x,T,T, x,x,T,x, T,T,x,x, T,x,T,x, x,x,x,x, T,x,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,T,T,T, x,x,x,x, T,x,T,x, T,T,x,x, x,T,x,T, T,T,T,T, T,T,x,x, x,x,T,x, x,x,x,x, x,x,x,x},
	{x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,x,x,x, x,x,x,T, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,T,x, T,T,T,T, T,x,T,T, x,x,x,x, T,T,T,T, T,T,T,T, x,T,T,T, x,x,T,T, T,T,T,T, T,T,T,T, T,x,x,x, T,x,T,T, x,T,x,x, x,x,x,x, T,x,x,x, x,T,T,x, x,x,T,x, x,T,T,x, x,T,x,T, x,x,x,T, x,T,x,T, x,x,T,T, x,x,x,T, x,T,x,x, x,x,T,T, x,x,T,x, x,T,x,x, T,x,T,x, x,x,x,x, T,x,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,T,T,T, x,x,x,x, T,x,x,x, T,T,x,x, x,T,x,T, T,T,T,T, T,T,x,x, x,x,T,x, x,x,x,x, x,x,x,x},
	{x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,x,x,x, x,x,x,T, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,T,x, x,x,T,x, T,T,T,T, T,x,T,T, x,T,x,x, T,T,T,T, T,T,T,T, x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x,x, T,T,T,T, T,T,x,x, x,T,x,x, T,T,T,x, T,T,T,x, T,x,T,x, x,T,T,x, T,T,x,T, x,x,x,T, x,T,x,T, x,x,T,T, x,x,x,T, x,T,x,x, x,x,T,T, x,x,T,x, x,T,x,x, T,x,T,T, x,x,x,x, T,x,T,x, x,x,x,x, T,x,x,x, x,T,x,T, x,T,T,T, T,T,x,x, x,T,T,T, T,x,T,x, T,x,x,T, T,T,x,T, x,T,T,T, T,T,T,T, T,T,T,x, x,x,T,T, x,T,x,x, x,x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
	{x,T,T,T, T,T,T,T, T,T,x,x, x,T,x,x, x,x,x,x, T,T,T,T, T,T,T,T, T,T,T,T, T,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, T,T,T,x, T,T,T,x, T,T,T,T, T,x,T,T, x,T,x,x, T,T,T,T, T,T,T,T, x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x,x, T,T,T,T, T,T,x,T, x,T,x,x, T,T,T,x, T,T,T,x, T,x,T,x, x,T,T,x, T,T,x,T, T,x,x,T, x,T,x,T, T,T,T,T, T,x,T,T, x,T,T,x, x,x,T,T, x,x,T,x, x,T,x,x, T,x,T,T, x,x,x,T, T,T,T,x, x,x,x,x, T,x,x,x, x,T,x,T, x,T,T,T, T,T,x,x, x,T,T,T, T,T,T,x, T,x,x,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, x,x,T,T, x,T,x,x, T,x,x,x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,T,x, T,T,T,T, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, T,T,x,T, x,x,x,x, x,x,x,x, T,x,x,x, x,T,x,x, x,x,T,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,T, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, T,x,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
	{x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,x,x,x, x,x,x,T, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,T,x, x,x,T,x, T,T,T,T, T,x,T,T, x,T,x,x, T,T,T,T, T,T,T,T, x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x,x, T,T,T,T, T,T,x,x, x,x,x,x, T,T,T,x, T,T,T,x, T,x,T,x, x,T,T,x, T,T,x,T, x,x,x,T, x,T,x,T, x,x,T,T, x,x,x,T, x,T,x,x, x,x,T,T, x,x,T,x, x,T,x,x, T,x,T,T, x,x,x,x, T,x,T,x, x,x,x,x, T,x,x,x, x,T,x,T, x,T,T,T, T,T,x,x, x,T,T,T, T,x,T,x, T,x,x,T, T,T,x,T, x,T,T,T, T,T,T,T, T,T,T,x, x,x,T,T, x,T,x,x, x,x,x,x},
	{x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,T,x, T,T,T,T, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, T,x,x,T, x,x,x,x, x,x,x,x, T,x,x,x, x,T,x,x, x,x,T,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,T, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, T,x,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x}

	};
} // end Parser

}