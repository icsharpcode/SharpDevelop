
#line  1 "VBNET.ATG" 
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
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
	const int maxT = 242;

	const  bool   T            = true;
	const  bool   x            = false;
	

#line  13 "VBNET.ATG" 


/*

*/

	void VBNET() {

#line  267 "VBNET.ATG" 
		lexer.NextToken(); // get the first token
		compilationUnit = new CompilationUnit();
		BlockStart(compilationUnit);
		
		while (la.kind == 1 || la.kind == 21) {
			EndOfStmt();
		}
		while (la.kind == 176) {
			OptionStmt();
			while (la.kind == 1 || la.kind == 21) {
				EndOfStmt();
			}
		}
		while (la.kind == 139) {
			ImportsStmt();
			while (la.kind == 1 || la.kind == 21) {
				EndOfStmt();
			}
		}
		while (
#line  275 "VBNET.ATG" 
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
		while (!(la.kind == 0 || la.kind == 1 || la.kind == 21)) {SynErr(243); lexer.NextToken(); }
		if (la.kind == 1) {
			lexer.NextToken();
		} else if (la.kind == 21) {
			lexer.NextToken();
		} else SynErr(244);
	}

	void OptionStmt() {

#line  280 "VBNET.ATG" 
		INode node = null; bool val = true; 
		Expect(176);

#line  281 "VBNET.ATG" 
		Location startPos = t.Location; 
		if (la.kind == 123) {
			lexer.NextToken();
			if (la.kind == 173 || la.kind == 174) {
				OptionValue(
#line  283 "VBNET.ATG" 
ref val);
			}

#line  284 "VBNET.ATG" 
			node = new OptionDeclaration(OptionType.Explicit, val); 
		} else if (la.kind == 210) {
			lexer.NextToken();
			if (la.kind == 173 || la.kind == 174) {
				OptionValue(
#line  286 "VBNET.ATG" 
ref val);
			}

#line  287 "VBNET.ATG" 
			node = new OptionDeclaration(OptionType.Strict, val); 
		} else if (la.kind == 89) {
			lexer.NextToken();
			if (la.kind == 69) {
				lexer.NextToken();

#line  289 "VBNET.ATG" 
				node = new OptionDeclaration(OptionType.CompareBinary, val); 
			} else if (la.kind == 216) {
				lexer.NextToken();

#line  290 "VBNET.ATG" 
				node = new OptionDeclaration(OptionType.CompareText, val); 
			} else SynErr(245);
		} else if (la.kind == 141) {
			lexer.NextToken();
			if (la.kind == 173 || la.kind == 174) {
				OptionValue(
#line  293 "VBNET.ATG" 
ref val);
			}

#line  294 "VBNET.ATG" 
			node = new OptionDeclaration(OptionType.Infer, val); 
		} else SynErr(246);
		EndOfStmt();

#line  298 "VBNET.ATG" 
		if (node != null) {
		node.StartLocation = startPos;
		node.EndLocation   = t.Location;
		AddChild(node);
		}
		
	}

	void ImportsStmt() {

#line  319 "VBNET.ATG" 
		List<Using> usings = new List<Using>();
		
		Expect(139);

#line  323 "VBNET.ATG" 
		Location startPos = t.Location;
		Using u;
		
		ImportClause(
#line  326 "VBNET.ATG" 
out u);

#line  326 "VBNET.ATG" 
		if (u != null) { usings.Add(u); } 
		while (la.kind == 22) {
			lexer.NextToken();
			ImportClause(
#line  328 "VBNET.ATG" 
out u);

#line  328 "VBNET.ATG" 
			if (u != null) { usings.Add(u); } 
		}
		EndOfStmt();

#line  332 "VBNET.ATG" 
		UsingDeclaration usingDeclaration = new UsingDeclaration(usings);
		usingDeclaration.StartLocation = startPos;
		usingDeclaration.EndLocation   = t.Location;
		AddChild(usingDeclaration);
		
	}

	void GlobalAttributeSection() {
		Expect(40);

#line  2830 "VBNET.ATG" 
		Location startPos = t.Location; 
		if (la.kind == 65) {
			lexer.NextToken();
		} else if (la.kind == 158) {
			lexer.NextToken();
		} else SynErr(247);

#line  2832 "VBNET.ATG" 
		string attributeTarget = t.val != null ? t.val.ToLower(System.Globalization.CultureInfo.InvariantCulture) : null;
		List<ASTAttribute> attributes = new List<ASTAttribute>();
		ASTAttribute attribute;
		
		Expect(21);
		Attribute(
#line  2836 "VBNET.ATG" 
out attribute);

#line  2836 "VBNET.ATG" 
		attributes.Add(attribute); 
		while (
#line  2837 "VBNET.ATG" 
NotFinalComma()) {
			if (la.kind == 22) {
				lexer.NextToken();
				if (la.kind == 65) {
					lexer.NextToken();
				} else if (la.kind == 158) {
					lexer.NextToken();
				} else SynErr(248);
				Expect(21);
			}
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
		EndOfStmt();

#line  2842 "VBNET.ATG" 
		AttributeSection section = new AttributeSection {
		AttributeTarget = attributeTarget,
		Attributes = attributes,
		StartLocation = startPos,
		EndLocation = t.EndLocation
		};
		AddChild(section);
		
	}

	void NamespaceMemberDecl() {

#line  365 "VBNET.ATG" 
		ModifierList m = new ModifierList();
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		string qualident;
		
		if (la.kind == 163) {
			lexer.NextToken();

#line  372 "VBNET.ATG" 
			Location startPos = t.Location;
			
			Qualident(
#line  374 "VBNET.ATG" 
out qualident);

#line  376 "VBNET.ATG" 
			INode node =  new NamespaceDeclaration(qualident);
			node.StartLocation = startPos;
			AddChild(node);
			BlockStart(node);
			
			EndOfStmt();
			NamespaceBody();

#line  384 "VBNET.ATG" 
			node.EndLocation = t.Location;
			BlockEnd();
			
		} else if (StartOf(2)) {
			while (la.kind == 40) {
				AttributeSection(
#line  388 "VBNET.ATG" 
out section);

#line  388 "VBNET.ATG" 
				attributes.Add(section); 
			}
			while (StartOf(3)) {
				TypeModifier(
#line  389 "VBNET.ATG" 
m);
			}
			NonModuleDeclaration(
#line  389 "VBNET.ATG" 
m, attributes);
		} else SynErr(249);
	}

	void OptionValue(
#line  306 "VBNET.ATG" 
ref bool val) {
		if (la.kind == 174) {
			lexer.NextToken();

#line  308 "VBNET.ATG" 
			val = true; 
		} else if (la.kind == 173) {
			lexer.NextToken();

#line  310 "VBNET.ATG" 
			val = false; 
		} else SynErr(250);
	}

	void ImportClause(
#line  339 "VBNET.ATG" 
out Using u) {

#line  341 "VBNET.ATG" 
		string qualident  = null;
		TypeReference aliasedType = null;
		u = null;
		
		if (StartOf(4)) {
			Qualident(
#line  346 "VBNET.ATG" 
out qualident);
			if (la.kind == 20) {
				lexer.NextToken();
				TypeName(
#line  347 "VBNET.ATG" 
out aliasedType);
			}

#line  349 "VBNET.ATG" 
			if (qualident != null && qualident.Length > 0) {
			if (aliasedType != null) {
				u = new Using(qualident, aliasedType);
			} else {
				u = new Using(qualident);
			}
			}
			
		} else if (la.kind == 10) {

#line  357 "VBNET.ATG" 
			string prefix = null; 
			lexer.NextToken();
			Identifier();

#line  358 "VBNET.ATG" 
			prefix = t.val; 
			Expect(20);
			Expect(3);

#line  358 "VBNET.ATG" 
			u = new Using(t.literalValue as string, prefix); 
			Expect(11);
		} else SynErr(251);
	}

	void Qualident(
#line  3633 "VBNET.ATG" 
out string qualident) {

#line  3635 "VBNET.ATG" 
		string name;
		qualidentBuilder.Length = 0; 
		
		Identifier();

#line  3639 "VBNET.ATG" 
		qualidentBuilder.Append(t.val); 
		while (
#line  3640 "VBNET.ATG" 
DotAndIdentOrKw()) {
			Expect(26);
			IdentifierOrKeyword(
#line  3640 "VBNET.ATG" 
out name);

#line  3640 "VBNET.ATG" 
			qualidentBuilder.Append('.'); qualidentBuilder.Append(name); 
		}

#line  3642 "VBNET.ATG" 
		qualident = qualidentBuilder.ToString(); 
	}

	void TypeName(
#line  2701 "VBNET.ATG" 
out TypeReference typeref) {

#line  2702 "VBNET.ATG" 
		ArrayList rank = null; Location startLocation = la.Location; 
		NonArrayTypeName(
#line  2704 "VBNET.ATG" 
out typeref, false);
		ArrayTypeModifiers(
#line  2705 "VBNET.ATG" 
out rank);

#line  2707 "VBNET.ATG" 
		if (typeref != null) {
		if (rank != null) {
			typeref.RankSpecifier = (int[])rank.ToArray(typeof(int));
		}
		typeref.StartLocation = startLocation;
		typeref.EndLocation = t.EndLocation;
		}
		
	}

	void Identifier() {
		if (StartOf(5)) {
			IdentifierForFieldDeclaration();
		} else if (la.kind == 100) {
			lexer.NextToken();
		} else if (la.kind == 66) {
			lexer.NextToken();
		} else if (la.kind == 148) {
			lexer.NextToken();
		} else SynErr(252);
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
		Expect(115);
		Expect(163);
		EndOfStmt();
	}

	void AttributeSection(
#line  2908 "VBNET.ATG" 
out AttributeSection section) {

#line  2910 "VBNET.ATG" 
		string attributeTarget = "";
		List<ASTAttribute> attributes = new List<ASTAttribute>();
		ASTAttribute attribute;
		Location startLocation = la.Location;
		
		Expect(40);
		if (
#line  2916 "VBNET.ATG" 
IsLocalAttrTarget()) {
			if (la.kind == 121) {
				lexer.NextToken();

#line  2917 "VBNET.ATG" 
				attributeTarget = "event";
			} else if (la.kind == 198) {
				lexer.NextToken();

#line  2918 "VBNET.ATG" 
				attributeTarget = "return";
			} else {
				Identifier();

#line  2921 "VBNET.ATG" 
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
#line  2931 "VBNET.ATG" 
out attribute);

#line  2931 "VBNET.ATG" 
		attributes.Add(attribute); 
		while (
#line  2932 "VBNET.ATG" 
NotFinalComma()) {
			Expect(22);
			Attribute(
#line  2932 "VBNET.ATG" 
out attribute);

#line  2932 "VBNET.ATG" 
			attributes.Add(attribute); 
		}
		if (la.kind == 22) {
			lexer.NextToken();
		}
		Expect(39);

#line  2936 "VBNET.ATG" 
		section = new AttributeSection {
		AttributeTarget = attributeTarget,
		Attributes = attributes,
		StartLocation = startLocation,
		EndLocation = t.EndLocation
		};
		
	}

	void TypeModifier(
#line  3722 "VBNET.ATG" 
ModifierList m) {
		switch (la.kind) {
		case 191: {
			lexer.NextToken();

#line  3723 "VBNET.ATG" 
			m.Add(Modifiers.Public, t.Location); 
			break;
		}
		case 190: {
			lexer.NextToken();

#line  3724 "VBNET.ATG" 
			m.Add(Modifiers.Protected, t.Location); 
			break;
		}
		case 127: {
			lexer.NextToken();

#line  3725 "VBNET.ATG" 
			m.Add(Modifiers.Internal, t.Location); 
			break;
		}
		case 188: {
			lexer.NextToken();

#line  3726 "VBNET.ATG" 
			m.Add(Modifiers.Private, t.Location); 
			break;
		}
		case 203: {
			lexer.NextToken();

#line  3727 "VBNET.ATG" 
			m.Add(Modifiers.Static, t.Location); 
			break;
		}
		case 202: {
			lexer.NextToken();

#line  3728 "VBNET.ATG" 
			m.Add(Modifiers.New, t.Location); 
			break;
		}
		case 159: {
			lexer.NextToken();

#line  3729 "VBNET.ATG" 
			m.Add(Modifiers.Abstract, t.Location); 
			break;
		}
		case 169: {
			lexer.NextToken();

#line  3730 "VBNET.ATG" 
			m.Add(Modifiers.Sealed, t.Location); 
			break;
		}
		case 186: {
			lexer.NextToken();

#line  3731 "VBNET.ATG" 
			m.Add(Modifiers.Partial, t.Location); 
			break;
		}
		default: SynErr(253); break;
		}
	}

	void NonModuleDeclaration(
#line  459 "VBNET.ATG" 
ModifierList m, List<AttributeSection> attributes) {

#line  461 "VBNET.ATG" 
		TypeReference typeRef = null;
		List<TypeReference> baseInterfaces = null;
		
		switch (la.kind) {
		case 86: {

#line  464 "VBNET.ATG" 
			m.Check(Modifiers.Classes); 
			lexer.NextToken();

#line  467 "VBNET.ATG" 
			TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
			newType.StartLocation = t.Location;
			AddChild(newType);
			BlockStart(newType);
			
			newType.Type       = ClassType.Class;
			
			Identifier();

#line  474 "VBNET.ATG" 
			newType.Name = t.val; 
			TypeParameterList(
#line  475 "VBNET.ATG" 
newType.Templates);
			EndOfStmt();

#line  477 "VBNET.ATG" 
			newType.BodyStartLocation = t.Location; 
			if (la.kind == 142) {
				ClassBaseType(
#line  478 "VBNET.ATG" 
out typeRef);

#line  478 "VBNET.ATG" 
				SafeAdd(newType, newType.BaseTypes, typeRef); 
			}
			while (la.kind == 138) {
				TypeImplementsClause(
#line  479 "VBNET.ATG" 
out baseInterfaces);

#line  479 "VBNET.ATG" 
				newType.BaseTypes.AddRange(baseInterfaces); 
			}
			ClassBody(
#line  480 "VBNET.ATG" 
newType);
			Expect(115);
			Expect(86);

#line  481 "VBNET.ATG" 
			newType.EndLocation = t.EndLocation; 
			EndOfStmt();

#line  484 "VBNET.ATG" 
			BlockEnd();
			
			break;
		}
		case 158: {
			lexer.NextToken();

#line  488 "VBNET.ATG" 
			m.Check(Modifiers.VBModules);
			TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
			AddChild(newType);
			BlockStart(newType);
			newType.StartLocation = m.GetDeclarationLocation(t.Location);
			newType.Type = ClassType.Module;
			
			Identifier();

#line  495 "VBNET.ATG" 
			newType.Name = t.val; 
			EndOfStmt();

#line  497 "VBNET.ATG" 
			newType.BodyStartLocation = t.Location; 
			ModuleBody(
#line  498 "VBNET.ATG" 
newType);

#line  500 "VBNET.ATG" 
			BlockEnd();
			
			break;
		}
		case 212: {
			lexer.NextToken();

#line  504 "VBNET.ATG" 
			m.Check(Modifiers.VBStructures);
			TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
			AddChild(newType);
			BlockStart(newType);
			newType.StartLocation = m.GetDeclarationLocation(t.Location);
			newType.Type = ClassType.Struct;
			
			Identifier();

#line  511 "VBNET.ATG" 
			newType.Name = t.val; 
			TypeParameterList(
#line  512 "VBNET.ATG" 
newType.Templates);
			EndOfStmt();

#line  514 "VBNET.ATG" 
			newType.BodyStartLocation = t.Location; 
			while (la.kind == 138) {
				TypeImplementsClause(
#line  515 "VBNET.ATG" 
out baseInterfaces);

#line  515 "VBNET.ATG" 
				newType.BaseTypes.AddRange(baseInterfaces);
			}
			StructureBody(
#line  516 "VBNET.ATG" 
newType);

#line  518 "VBNET.ATG" 
			BlockEnd();
			
			break;
		}
		case 117: {
			lexer.NextToken();

#line  523 "VBNET.ATG" 
			m.Check(Modifiers.VBEnums);
			TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
			newType.StartLocation = m.GetDeclarationLocation(t.Location);
			AddChild(newType);
			BlockStart(newType);
			
			newType.Type = ClassType.Enum;
			
			Identifier();

#line  531 "VBNET.ATG" 
			newType.Name = t.val; 
			if (la.kind == 63) {
				lexer.NextToken();
				NonArrayTypeName(
#line  532 "VBNET.ATG" 
out typeRef, false);

#line  532 "VBNET.ATG" 
				SafeAdd(newType, newType.BaseTypes, typeRef); 
			}
			EndOfStmt();

#line  534 "VBNET.ATG" 
			newType.BodyStartLocation = t.Location; 
			EnumBody(
#line  535 "VBNET.ATG" 
newType);

#line  537 "VBNET.ATG" 
			BlockEnd();
			
			break;
		}
		case 144: {
			lexer.NextToken();

#line  542 "VBNET.ATG" 
			m.Check(Modifiers.VBInterfacs);
			TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
			newType.StartLocation = m.GetDeclarationLocation(t.Location);
			AddChild(newType);
			BlockStart(newType);
			newType.Type = ClassType.Interface;
			
			Identifier();

#line  549 "VBNET.ATG" 
			newType.Name = t.val; 
			TypeParameterList(
#line  550 "VBNET.ATG" 
newType.Templates);
			EndOfStmt();

#line  552 "VBNET.ATG" 
			newType.BodyStartLocation = t.Location; 
			while (la.kind == 142) {
				InterfaceBase(
#line  553 "VBNET.ATG" 
out baseInterfaces);

#line  553 "VBNET.ATG" 
				newType.BaseTypes.AddRange(baseInterfaces); 
			}
			InterfaceBody(
#line  554 "VBNET.ATG" 
newType);

#line  556 "VBNET.ATG" 
			BlockEnd();
			
			break;
		}
		case 105: {
			lexer.NextToken();

#line  561 "VBNET.ATG" 
			m.Check(Modifiers.VBDelegates);
			DelegateDeclaration delegateDeclr = new DelegateDeclaration(m.Modifier, attributes);
			delegateDeclr.ReturnType = new TypeReference("System.Void", true);
			delegateDeclr.StartLocation = m.GetDeclarationLocation(t.Location);
			List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
			
			if (la.kind == 213) {
				lexer.NextToken();
				Identifier();

#line  568 "VBNET.ATG" 
				delegateDeclr.Name = t.val; 
				TypeParameterList(
#line  569 "VBNET.ATG" 
delegateDeclr.Templates);
				if (la.kind == 37) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
#line  570 "VBNET.ATG" 
p);
					}
					Expect(38);

#line  570 "VBNET.ATG" 
					delegateDeclr.Parameters = p; 
				}
			} else if (la.kind == 129) {
				lexer.NextToken();
				Identifier();

#line  572 "VBNET.ATG" 
				delegateDeclr.Name = t.val; 
				TypeParameterList(
#line  573 "VBNET.ATG" 
delegateDeclr.Templates);
				if (la.kind == 37) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
#line  574 "VBNET.ATG" 
p);
					}
					Expect(38);

#line  574 "VBNET.ATG" 
					delegateDeclr.Parameters = p; 
				}
				if (la.kind == 63) {
					lexer.NextToken();

#line  575 "VBNET.ATG" 
					TypeReference type; 
					TypeName(
#line  575 "VBNET.ATG" 
out type);

#line  575 "VBNET.ATG" 
					delegateDeclr.ReturnType = type; 
				}
			} else SynErr(254);

#line  577 "VBNET.ATG" 
			delegateDeclr.EndLocation = t.EndLocation; 
			EndOfStmt();

#line  580 "VBNET.ATG" 
			AddChild(delegateDeclr);
			
			break;
		}
		default: SynErr(255); break;
		}
	}

	void TypeParameterList(
#line  393 "VBNET.ATG" 
List<TemplateDefinition> templates) {

#line  395 "VBNET.ATG" 
		TemplateDefinition template;
		
		if (
#line  399 "VBNET.ATG" 
la.kind == Tokens.OpenParenthesis && Peek(1).kind == Tokens.Of) {
			lexer.NextToken();
			Expect(172);
			TypeParameter(
#line  400 "VBNET.ATG" 
out template);

#line  402 "VBNET.ATG" 
			if (template != null) templates.Add(template);
			
			while (la.kind == 22) {
				lexer.NextToken();
				TypeParameter(
#line  405 "VBNET.ATG" 
out template);

#line  407 "VBNET.ATG" 
				if (template != null) templates.Add(template);
				
			}
			Expect(38);
		}
	}

	void TypeParameter(
#line  415 "VBNET.ATG" 
out TemplateDefinition template) {

#line  416 "VBNET.ATG" 
		VarianceModifier modifier = VarianceModifier.Invariant; Location startLocation = la.Location; 
		if (la.kind == 140 || la.kind == 181) {
			if (la.kind == 140) {
				lexer.NextToken();

#line  419 "VBNET.ATG" 
				modifier = VarianceModifier.Contravariant; 
			} else {
				lexer.NextToken();

#line  419 "VBNET.ATG" 
				modifier = VarianceModifier.Covariant; 
			}
		}
		Identifier();

#line  419 "VBNET.ATG" 
		template = new TemplateDefinition(t.val, null) { VarianceModifier = modifier }; 
		if (la.kind == 63) {
			TypeParameterConstraints(
#line  420 "VBNET.ATG" 
template);
		}

#line  423 "VBNET.ATG" 
		if (template != null) {
		template.StartLocation = startLocation;
		template.EndLocation = t.EndLocation;
		}
		
	}

	void TypeParameterConstraints(
#line  431 "VBNET.ATG" 
TemplateDefinition template) {

#line  433 "VBNET.ATG" 
		TypeReference constraint;
		
		Expect(63);
		if (la.kind == 35) {
			lexer.NextToken();
			TypeParameterConstraint(
#line  439 "VBNET.ATG" 
out constraint);

#line  439 "VBNET.ATG" 
			if (constraint != null) { template.Bases.Add(constraint); } 
			while (la.kind == 22) {
				lexer.NextToken();
				TypeParameterConstraint(
#line  442 "VBNET.ATG" 
out constraint);

#line  442 "VBNET.ATG" 
				if (constraint != null) { template.Bases.Add(constraint); } 
			}
			Expect(36);
		} else if (StartOf(7)) {
			TypeParameterConstraint(
#line  445 "VBNET.ATG" 
out constraint);

#line  445 "VBNET.ATG" 
			if (constraint != null) { template.Bases.Add(constraint); } 
		} else SynErr(256);
	}

	void TypeParameterConstraint(
#line  449 "VBNET.ATG" 
out TypeReference constraint) {

#line  450 "VBNET.ATG" 
		constraint = null; Location startLocation = la.Location; 
		if (la.kind == 86) {
			lexer.NextToken();

#line  452 "VBNET.ATG" 
			constraint = TypeReference.ClassConstraint; 
		} else if (la.kind == 212) {
			lexer.NextToken();

#line  453 "VBNET.ATG" 
			constraint = TypeReference.StructConstraint; 
		} else if (la.kind == 165) {
			lexer.NextToken();

#line  454 "VBNET.ATG" 
			constraint = TypeReference.NewConstraint; 
		} else if (StartOf(8)) {
			TypeName(
#line  455 "VBNET.ATG" 
out constraint);
		} else SynErr(257);
	}

	void ClassBaseType(
#line  801 "VBNET.ATG" 
out TypeReference typeRef) {

#line  803 "VBNET.ATG" 
		typeRef = null;
		
		Expect(142);
		TypeName(
#line  806 "VBNET.ATG" 
out typeRef);
		EndOfStmt();
	}

	void TypeImplementsClause(
#line  1624 "VBNET.ATG" 
out List<TypeReference> baseInterfaces) {

#line  1626 "VBNET.ATG" 
		baseInterfaces = new List<TypeReference>();
		TypeReference type = null;
		
		Expect(138);
		TypeName(
#line  1629 "VBNET.ATG" 
out type);

#line  1631 "VBNET.ATG" 
		if (type != null) baseInterfaces.Add(type);
		
		while (la.kind == 22) {
			lexer.NextToken();
			TypeName(
#line  1634 "VBNET.ATG" 
out type);

#line  1635 "VBNET.ATG" 
			if (type != null) baseInterfaces.Add(type); 
		}
		EndOfStmt();
	}

	void ClassBody(
#line  594 "VBNET.ATG" 
TypeDeclaration newType) {

#line  595 "VBNET.ATG" 
		AttributeSection section; 
		while (la.kind == 1 || la.kind == 21) {
			EndOfStmt();
		}
		while (StartOf(9)) {

#line  598 "VBNET.ATG" 
			List<AttributeSection> attributes = new List<AttributeSection>();
			ModifierList m = new ModifierList();
			
			while (la.kind == 40) {
				AttributeSection(
#line  601 "VBNET.ATG" 
out section);

#line  601 "VBNET.ATG" 
				attributes.Add(section); 
			}
			while (StartOf(10)) {
				MemberModifier(
#line  602 "VBNET.ATG" 
m);
			}
			ClassMemberDecl(
#line  603 "VBNET.ATG" 
m, attributes);
			while (la.kind == 1 || la.kind == 21) {
				EndOfStmt();
			}
		}
	}

	void ModuleBody(
#line  625 "VBNET.ATG" 
TypeDeclaration newType) {

#line  626 "VBNET.ATG" 
		AttributeSection section; 
		while (la.kind == 1 || la.kind == 21) {
			EndOfStmt();
		}
		while (StartOf(9)) {

#line  629 "VBNET.ATG" 
			List<AttributeSection> attributes = new List<AttributeSection>();
			ModifierList m = new ModifierList();
			
			while (la.kind == 40) {
				AttributeSection(
#line  632 "VBNET.ATG" 
out section);

#line  632 "VBNET.ATG" 
				attributes.Add(section); 
			}
			while (StartOf(10)) {
				MemberModifier(
#line  633 "VBNET.ATG" 
m);
			}
			ClassMemberDecl(
#line  634 "VBNET.ATG" 
m, attributes);
			while (la.kind == 1 || la.kind == 21) {
				EndOfStmt();
			}
		}
		Expect(115);
		Expect(158);

#line  637 "VBNET.ATG" 
		newType.EndLocation = t.EndLocation; 
		EndOfStmt();
	}

	void StructureBody(
#line  608 "VBNET.ATG" 
TypeDeclaration newType) {

#line  609 "VBNET.ATG" 
		AttributeSection section; 
		while (la.kind == 1 || la.kind == 21) {
			EndOfStmt();
		}
		while (StartOf(9)) {

#line  612 "VBNET.ATG" 
			List<AttributeSection> attributes = new List<AttributeSection>();
			ModifierList m = new ModifierList();
			
			while (la.kind == 40) {
				AttributeSection(
#line  615 "VBNET.ATG" 
out section);

#line  615 "VBNET.ATG" 
				attributes.Add(section); 
			}
			while (StartOf(10)) {
				MemberModifier(
#line  616 "VBNET.ATG" 
m);
			}
			StructureMemberDecl(
#line  617 "VBNET.ATG" 
m, attributes);
			while (la.kind == 1 || la.kind == 21) {
				EndOfStmt();
			}
		}
		Expect(115);
		Expect(212);

#line  620 "VBNET.ATG" 
		newType.EndLocation = t.EndLocation; 
		EndOfStmt();
	}

	void NonArrayTypeName(
#line  2729 "VBNET.ATG" 
out TypeReference typeref, bool canBeUnbound) {

#line  2731 "VBNET.ATG" 
		string name;
		typeref = null;
		bool isGlobal = false;
		
		if (StartOf(11)) {
			if (la.kind == 132) {
				lexer.NextToken();
				Expect(26);

#line  2736 "VBNET.ATG" 
				isGlobal = true; 
			}
			QualIdentAndTypeArguments(
#line  2737 "VBNET.ATG" 
out typeref, canBeUnbound);

#line  2738 "VBNET.ATG" 
			typeref.IsGlobal = isGlobal; 
			while (la.kind == 26) {
				lexer.NextToken();

#line  2739 "VBNET.ATG" 
				TypeReference nestedTypeRef; 
				QualIdentAndTypeArguments(
#line  2740 "VBNET.ATG" 
out nestedTypeRef, canBeUnbound);

#line  2741 "VBNET.ATG" 
				typeref = new InnerClassTypeReference(typeref, nestedTypeRef.Type, nestedTypeRef.GenericTypes); 
			}
		} else if (la.kind == 171) {
			lexer.NextToken();

#line  2744 "VBNET.ATG" 
			typeref = new TypeReference("System.Object", true); 
			if (la.kind == 33) {
				lexer.NextToken();

#line  2748 "VBNET.ATG" 
				List<TypeReference> typeArguments = new List<TypeReference>(1);
				if (typeref != null) typeArguments.Add(typeref);
				typeref = new TypeReference("System.Nullable", typeArguments) { IsKeyword = true };
				
			}
		} else if (StartOf(12)) {
			PrimitiveTypeName(
#line  2754 "VBNET.ATG" 
out name);

#line  2754 "VBNET.ATG" 
			typeref = new TypeReference(name, true); 
			if (la.kind == 33) {
				lexer.NextToken();

#line  2758 "VBNET.ATG" 
				List<TypeReference> typeArguments = new List<TypeReference>(1);
				if (typeref != null) typeArguments.Add(typeref);
				typeref = new TypeReference("System.Nullable", typeArguments) { IsKeyword = true };
				
			}
		} else SynErr(258);
	}

	void EnumBody(
#line  641 "VBNET.ATG" 
TypeDeclaration newType) {

#line  642 "VBNET.ATG" 
		FieldDeclaration f; 
		while (la.kind == 1 || la.kind == 21) {
			EndOfStmt();
		}
		while (StartOf(13)) {
			EnumMemberDecl(
#line  645 "VBNET.ATG" 
out f);

#line  647 "VBNET.ATG" 
			AddChild(f);
			
			while (la.kind == 1 || la.kind == 21) {
				EndOfStmt();
			}
		}
		Expect(115);
		Expect(117);

#line  651 "VBNET.ATG" 
		newType.EndLocation = t.EndLocation; 
		EndOfStmt();
	}

	void InterfaceBase(
#line  1609 "VBNET.ATG" 
out List<TypeReference> bases) {

#line  1611 "VBNET.ATG" 
		TypeReference type;
		bases = new List<TypeReference>();
		
		Expect(142);
		TypeName(
#line  1615 "VBNET.ATG" 
out type);

#line  1615 "VBNET.ATG" 
		if (type != null) bases.Add(type); 
		while (la.kind == 22) {
			lexer.NextToken();
			TypeName(
#line  1618 "VBNET.ATG" 
out type);

#line  1618 "VBNET.ATG" 
			if (type != null) bases.Add(type); 
		}
		EndOfStmt();
	}

	void InterfaceBody(
#line  655 "VBNET.ATG" 
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
		Expect(115);
		Expect(144);

#line  661 "VBNET.ATG" 
		newType.EndLocation = t.EndLocation; 
		EndOfStmt();
	}

	void FormalParameterList(
#line  2946 "VBNET.ATG" 
List<ParameterDeclarationExpression> parameter) {

#line  2947 "VBNET.ATG" 
		ParameterDeclarationExpression p; 
		FormalParameter(
#line  2949 "VBNET.ATG" 
out p);

#line  2949 "VBNET.ATG" 
		if (p != null) parameter.Add(p); 
		while (la.kind == 22) {
			lexer.NextToken();
			FormalParameter(
#line  2951 "VBNET.ATG" 
out p);

#line  2951 "VBNET.ATG" 
			if (p != null) parameter.Add(p); 
		}
	}

	void MemberModifier(
#line  3734 "VBNET.ATG" 
ModifierList m) {
		switch (la.kind) {
		case 159: {
			lexer.NextToken();

#line  3735 "VBNET.ATG" 
			m.Add(Modifiers.Abstract, t.Location);
			break;
		}
		case 104: {
			lexer.NextToken();

#line  3736 "VBNET.ATG" 
			m.Add(Modifiers.Default, t.Location);
			break;
		}
		case 127: {
			lexer.NextToken();

#line  3737 "VBNET.ATG" 
			m.Add(Modifiers.Internal, t.Location);
			break;
		}
		case 202: {
			lexer.NextToken();

#line  3738 "VBNET.ATG" 
			m.Add(Modifiers.New, t.Location);
			break;
		}
		case 184: {
			lexer.NextToken();

#line  3739 "VBNET.ATG" 
			m.Add(Modifiers.Override, t.Location);
			break;
		}
		case 160: {
			lexer.NextToken();

#line  3740 "VBNET.ATG" 
			m.Add(Modifiers.Abstract, t.Location);
			break;
		}
		case 188: {
			lexer.NextToken();

#line  3741 "VBNET.ATG" 
			m.Add(Modifiers.Private, t.Location);
			break;
		}
		case 190: {
			lexer.NextToken();

#line  3742 "VBNET.ATG" 
			m.Add(Modifiers.Protected, t.Location);
			break;
		}
		case 191: {
			lexer.NextToken();

#line  3743 "VBNET.ATG" 
			m.Add(Modifiers.Public, t.Location);
			break;
		}
		case 169: {
			lexer.NextToken();

#line  3744 "VBNET.ATG" 
			m.Add(Modifiers.Sealed, t.Location);
			break;
		}
		case 170: {
			lexer.NextToken();

#line  3745 "VBNET.ATG" 
			m.Add(Modifiers.Sealed, t.Location);
			break;
		}
		case 203: {
			lexer.NextToken();

#line  3746 "VBNET.ATG" 
			m.Add(Modifiers.Static, t.Location);
			break;
		}
		case 183: {
			lexer.NextToken();

#line  3747 "VBNET.ATG" 
			m.Add(Modifiers.Virtual, t.Location);
			break;
		}
		case 182: {
			lexer.NextToken();

#line  3748 "VBNET.ATG" 
			m.Add(Modifiers.Overloads, t.Location);
			break;
		}
		case 193: {
			lexer.NextToken();

#line  3749 "VBNET.ATG" 
			m.Add(Modifiers.ReadOnly, t.Location);
			break;
		}
		case 238: {
			lexer.NextToken();

#line  3750 "VBNET.ATG" 
			m.Add(Modifiers.WriteOnly, t.Location);
			break;
		}
		case 237: {
			lexer.NextToken();

#line  3751 "VBNET.ATG" 
			m.Add(Modifiers.WithEvents, t.Location);
			break;
		}
		case 107: {
			lexer.NextToken();

#line  3752 "VBNET.ATG" 
			m.Add(Modifiers.Dim, t.Location);
			break;
		}
		case 186: {
			lexer.NextToken();

#line  3753 "VBNET.ATG" 
			m.Add(Modifiers.Partial, t.Location);
			break;
		}
		case 66: {
			lexer.NextToken();

#line  3754 "VBNET.ATG" 
			m.Add(Modifiers.Async, t.Location);
			break;
		}
		case 148: {
			lexer.NextToken();

#line  3755 "VBNET.ATG" 
			m.Add(Modifiers.Iterator, t.Location);
			break;
		}
		default: SynErr(259); break;
		}
	}

	void ClassMemberDecl(
#line  797 "VBNET.ATG" 
ModifierList m, List<AttributeSection> attributes) {
		StructureMemberDecl(
#line  798 "VBNET.ATG" 
m, attributes);
	}

	void StructureMemberDecl(
#line  811 "VBNET.ATG" 
ModifierList m, List<AttributeSection> attributes) {

#line  813 "VBNET.ATG" 
		TypeReference type = null;
		List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
		Statement stmt = null;
		List<VariableDeclaration> variableDeclarators = new List<VariableDeclaration>();
		List<TemplateDefinition> templates = new List<TemplateDefinition>();
		
		switch (la.kind) {
		case 86: case 105: case 117: case 144: case 158: case 212: {
			NonModuleDeclaration(
#line  820 "VBNET.ATG" 
m, attributes);
			break;
		}
		case 213: {
			lexer.NextToken();

#line  824 "VBNET.ATG" 
			Location startPos = t.Location;
			
			if (StartOf(4)) {

#line  828 "VBNET.ATG" 
				string name = String.Empty;
				MethodDeclaration methodDeclaration; List<string> handlesClause = null;
				List<InterfaceImplementation> implementsClause = null;
				
				Identifier();

#line  834 "VBNET.ATG" 
				name = t.val;
				m.Check(Modifiers.VBMethods);
				
				TypeParameterList(
#line  837 "VBNET.ATG" 
templates);
				if (la.kind == 37) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
#line  838 "VBNET.ATG" 
p);
					}
					Expect(38);
				}
				if (la.kind == 136 || la.kind == 138) {
					if (la.kind == 138) {
						ImplementsClause(
#line  841 "VBNET.ATG" 
out implementsClause);
					} else {
						HandlesClause(
#line  843 "VBNET.ATG" 
out handlesClause);
					}
				}

#line  846 "VBNET.ATG" 
				Location endLocation = t.EndLocation; 
				if (
#line  849 "VBNET.ATG" 
IsMustOverride(m)) {
					EndOfStmt();

#line  852 "VBNET.ATG" 
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

#line  865 "VBNET.ATG" 
					methodDeclaration = new MethodDeclaration {
					Name = name, Modifier = m.Modifier, Parameters = p, Attributes = attributes,
					StartLocation = m.GetDeclarationLocation(startPos), EndLocation = endLocation,
					TypeReference = new TypeReference("System.Void", true),
					Templates = templates,
					HandlesClause = handlesClause,
					InterfaceImplementations = implementsClause
					};
					AddChild(methodDeclaration);
					

#line  876 "VBNET.ATG" 
					if (ParseMethodBodies) { 
					Block(
#line  877 "VBNET.ATG" 
out stmt);
					Expect(115);
					Expect(213);

#line  879 "VBNET.ATG" 
					} else {
					// don't parse method body
					lexer.SkipCurrentBlock(Tokens.Sub); stmt = new BlockStatement();
					  }
					

#line  885 "VBNET.ATG" 
					methodDeclaration.Body  = (BlockStatement)stmt; 

#line  886 "VBNET.ATG" 
					methodDeclaration.Body.EndLocation = t.EndLocation; 
					EndOfStmt();
				} else SynErr(260);
			} else if (la.kind == 165) {
				lexer.NextToken();
				if (la.kind == 37) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
#line  890 "VBNET.ATG" 
p);
					}
					Expect(38);
				}

#line  891 "VBNET.ATG" 
				m.Check(Modifiers.Constructors); 

#line  892 "VBNET.ATG" 
				Location constructorEndLocation = t.EndLocation; 
				Expect(1);

#line  895 "VBNET.ATG" 
				if (ParseMethodBodies) { 
				Block(
#line  896 "VBNET.ATG" 
out stmt);
				Expect(115);
				Expect(213);

#line  898 "VBNET.ATG" 
				} else {
				// don't parse method body
				lexer.SkipCurrentBlock(Tokens.Sub); stmt = new BlockStatement();
				  }
				

#line  904 "VBNET.ATG" 
				Location endLocation = t.EndLocation; 
				EndOfStmt();

#line  907 "VBNET.ATG" 
				ConstructorDeclaration cd = new ConstructorDeclaration("New", m.Modifier, p, attributes);
				cd.StartLocation = m.GetDeclarationLocation(startPos);
				cd.EndLocation   = constructorEndLocation;
				cd.Body = (BlockStatement)stmt;
				cd.Body.EndLocation   = endLocation;
				AddChild(cd);
				
			} else SynErr(261);
			break;
		}
		case 129: {
			lexer.NextToken();

#line  919 "VBNET.ATG" 
			m.Check(Modifiers.VBMethods);
			string name = String.Empty;
			Location startPos = t.Location;
			MethodDeclaration methodDeclaration;List<string> handlesClause = null;
			List<InterfaceImplementation> implementsClause = null;
			AttributeSection returnTypeAttributeSection = null;
			
			Identifier();

#line  926 "VBNET.ATG" 
			name = t.val; 
			TypeParameterList(
#line  927 "VBNET.ATG" 
templates);
			if (la.kind == 37) {
				lexer.NextToken();
				if (StartOf(6)) {
					FormalParameterList(
#line  928 "VBNET.ATG" 
p);
				}
				Expect(38);
			}
			if (la.kind == 63) {
				lexer.NextToken();
				while (la.kind == 40) {
					AttributeSection(
#line  930 "VBNET.ATG" 
out returnTypeAttributeSection);

#line  932 "VBNET.ATG" 
					if (returnTypeAttributeSection != null) {
					returnTypeAttributeSection.AttributeTarget = "return";
					attributes.Add(returnTypeAttributeSection);
					}
					
				}
				TypeName(
#line  938 "VBNET.ATG" 
out type);
			}

#line  940 "VBNET.ATG" 
			if(type == null) {
			type = new TypeReference("System.Object", true);
			}
			
			if (la.kind == 136 || la.kind == 138) {
				if (la.kind == 138) {
					ImplementsClause(
#line  946 "VBNET.ATG" 
out implementsClause);
				} else {
					HandlesClause(
#line  948 "VBNET.ATG" 
out handlesClause);
				}
			}

#line  951 "VBNET.ATG" 
			Location endLocation = t.EndLocation; 
			if (
#line  954 "VBNET.ATG" 
IsMustOverride(m)) {
				EndOfStmt();

#line  957 "VBNET.ATG" 
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

#line  972 "VBNET.ATG" 
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
#line  985 "VBNET.ATG" 
out stmt);
				Expect(115);
				Expect(129);

#line  987 "VBNET.ATG" 
				} else {
				// don't parse method body
				lexer.SkipCurrentBlock(Tokens.Function); stmt = new BlockStatement();
				}
				methodDeclaration.Body = (BlockStatement)stmt;
				methodDeclaration.Body.StartLocation = methodDeclaration.EndLocation;
				methodDeclaration.Body.EndLocation   = t.EndLocation;
				
				EndOfStmt();
			} else SynErr(262);
			break;
		}
		case 103: {
			lexer.NextToken();

#line  1001 "VBNET.ATG" 
			m.Check(Modifiers.VBExternalMethods);
			Location startPos = t.Location;
			CharsetModifier charsetModifer = CharsetModifier.None;
			string library = String.Empty;
			string alias = null;
			string name = String.Empty;
			
			if (StartOf(15)) {
				Charset(
#line  1008 "VBNET.ATG" 
out charsetModifer);
			}
			if (la.kind == 213) {
				lexer.NextToken();
				Identifier();

#line  1011 "VBNET.ATG" 
				name = t.val; 
				Expect(152);
				Expect(3);

#line  1012 "VBNET.ATG" 
				library = t.literalValue as string; 
				if (la.kind == 59) {
					lexer.NextToken();
					Expect(3);

#line  1013 "VBNET.ATG" 
					alias = t.literalValue as string; 
				}
				if (la.kind == 37) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
#line  1014 "VBNET.ATG" 
p);
					}
					Expect(38);
				}
				EndOfStmt();

#line  1017 "VBNET.ATG" 
				DeclareDeclaration declareDeclaration = new DeclareDeclaration(name, m.Modifier, null, p, attributes, library, alias, charsetModifer);
				declareDeclaration.StartLocation = m.GetDeclarationLocation(startPos);
				declareDeclaration.EndLocation   = t.EndLocation;
				AddChild(declareDeclaration);
				
			} else if (la.kind == 129) {
				lexer.NextToken();
				Identifier();

#line  1024 "VBNET.ATG" 
				name = t.val; 
				Expect(152);
				Expect(3);

#line  1025 "VBNET.ATG" 
				library = t.literalValue as string; 
				if (la.kind == 59) {
					lexer.NextToken();
					Expect(3);

#line  1026 "VBNET.ATG" 
					alias = t.literalValue as string; 
				}
				if (la.kind == 37) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
#line  1027 "VBNET.ATG" 
p);
					}
					Expect(38);
				}
				if (la.kind == 63) {
					lexer.NextToken();
					TypeName(
#line  1028 "VBNET.ATG" 
out type);
				}
				EndOfStmt();

#line  1031 "VBNET.ATG" 
				DeclareDeclaration declareDeclaration = new DeclareDeclaration(name, m.Modifier, type, p, attributes, library, alias, charsetModifer);
				declareDeclaration.StartLocation = m.GetDeclarationLocation(startPos);
				declareDeclaration.EndLocation   = t.EndLocation;
				AddChild(declareDeclaration);
				
			} else SynErr(263);
			break;
		}
		case 121: {
			lexer.NextToken();

#line  1041 "VBNET.ATG" 
			m.Check(Modifiers.VBEvents);
			Location startPos = t.Location;
			EventDeclaration eventDeclaration;
			string name = String.Empty;
			List<InterfaceImplementation> implementsClause = null;
			
			Identifier();

#line  1047 "VBNET.ATG" 
			name= t.val; 
			if (la.kind == 63) {
				lexer.NextToken();
				TypeName(
#line  1049 "VBNET.ATG" 
out type);
			} else if (StartOf(16)) {
				if (la.kind == 37) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
#line  1051 "VBNET.ATG" 
p);
					}
					Expect(38);
				}
			} else SynErr(264);
			if (la.kind == 138) {
				ImplementsClause(
#line  1053 "VBNET.ATG" 
out implementsClause);
			}

#line  1055 "VBNET.ATG" 
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
		case 2: case 58: case 62: case 64: case 65: case 67: case 68: case 69: case 72: case 89: case 106: case 109: case 118: case 123: case 128: case 135: case 141: case 145: case 149: case 150: case 173: case 179: case 181: case 187: case 206: case 215: case 216: case 226: case 227: case 233: case 240: {

#line  1066 "VBNET.ATG" 
			m.Check(Modifiers.Fields);
			FieldDeclaration fd = new FieldDeclaration(attributes, null, m.Modifier);
			
			IdentifierForFieldDeclaration();

#line  1069 "VBNET.ATG" 
			string name = t.val; 

#line  1070 "VBNET.ATG" 
			fd.StartLocation = m.GetDeclarationLocation(t.Location); 
			VariableDeclaratorPartAfterIdentifier(
#line  1072 "VBNET.ATG" 
variableDeclarators, name);
			while (la.kind == 22) {
				lexer.NextToken();
				VariableDeclarator(
#line  1073 "VBNET.ATG" 
variableDeclarators);
			}
			EndOfStmt();

#line  1076 "VBNET.ATG" 
			fd.EndLocation = t.EndLocation;
			fd.Fields = variableDeclarators;
			AddChild(fd);
			
			break;
		}
		case 90: {

#line  1081 "VBNET.ATG" 
			m.Check(Modifiers.Fields); 
			lexer.NextToken();

#line  1082 "VBNET.ATG" 
			m.Add(Modifiers.Const, t.Location);  

#line  1084 "VBNET.ATG" 
			FieldDeclaration fd = new FieldDeclaration(attributes, type, m.Modifier);
			fd.StartLocation = m.GetDeclarationLocation(t.Location);
			List<VariableDeclaration> constantDeclarators = new List<VariableDeclaration>();
			
			ConstantDeclarator(
#line  1088 "VBNET.ATG" 
constantDeclarators);
			while (la.kind == 22) {
				lexer.NextToken();
				ConstantDeclarator(
#line  1089 "VBNET.ATG" 
constantDeclarators);
			}

#line  1091 "VBNET.ATG" 
			fd.Fields = constantDeclarators;
			fd.EndLocation = t.Location;
			
			EndOfStmt();

#line  1096 "VBNET.ATG" 
			fd.EndLocation = t.EndLocation;
			AddChild(fd);
			
			break;
		}
		case 189: {
			lexer.NextToken();

#line  1102 "VBNET.ATG" 
			m.Check(Modifiers.VBProperties);
			Location startPos = t.Location;
			List<InterfaceImplementation> implementsClause = null;
			AttributeSection returnTypeAttributeSection = null;
			Expression initializer = null;
			
			Identifier();

#line  1108 "VBNET.ATG" 
			string propertyName = t.val; 
			if (la.kind == 37) {
				lexer.NextToken();
				if (StartOf(6)) {
					FormalParameterList(
#line  1109 "VBNET.ATG" 
p);
				}
				Expect(38);
			}
			if (la.kind == 63) {
				lexer.NextToken();
				while (la.kind == 40) {
					AttributeSection(
#line  1112 "VBNET.ATG" 
out returnTypeAttributeSection);

#line  1114 "VBNET.ATG" 
					if (returnTypeAttributeSection != null) {
					returnTypeAttributeSection.AttributeTarget = "return";
					attributes.Add(returnTypeAttributeSection);
					}
					
				}
				if (
#line  1121 "VBNET.ATG" 
IsNewExpression()) {
					ObjectCreateExpression(
#line  1121 "VBNET.ATG" 
out initializer);

#line  1123 "VBNET.ATG" 
					if (initializer is ObjectCreateExpression) {
					type = ((ObjectCreateExpression)initializer).CreateType.Clone();
					} else {
						type = ((ArrayCreateExpression)initializer).CreateType.Clone();
					}
					
				} else if (StartOf(8)) {
					TypeName(
#line  1130 "VBNET.ATG" 
out type);
				} else SynErr(265);
			}
			if (la.kind == 20) {
				lexer.NextToken();
				Expr(
#line  1133 "VBNET.ATG" 
out initializer);
			}
			if (la.kind == 138) {
				ImplementsClause(
#line  1134 "VBNET.ATG" 
out implementsClause);
			}
			EndOfStmt();
			if (
#line  1138 "VBNET.ATG" 
IsMustOverride(m) || IsAutomaticProperty()) {

#line  1140 "VBNET.ATG" 
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

#line  1152 "VBNET.ATG" 
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
#line  1162 "VBNET.ATG" 
out getRegion, out setRegion);
				Expect(115);
				Expect(189);
				EndOfStmt();

#line  1166 "VBNET.ATG" 
				pDecl.GetRegion = getRegion;
				pDecl.SetRegion = setRegion;
				pDecl.BodyEnd = t.Location; // t = EndOfStmt; not "Property"
				AddChild(pDecl);
				
			} else SynErr(266);
			break;
		}
		case 100: {
			lexer.NextToken();

#line  1173 "VBNET.ATG" 
			Location startPos = t.Location; 
			Expect(121);

#line  1175 "VBNET.ATG" 
			m.Check(Modifiers.VBCustomEvents);
			EventAddRemoveRegion eventAccessorDeclaration;
			EventAddRegion addHandlerAccessorDeclaration = null;
			EventRemoveRegion removeHandlerAccessorDeclaration = null;
			EventRaiseRegion raiseEventAccessorDeclaration = null;
			List<InterfaceImplementation> implementsClause = null;
			
			Identifier();

#line  1182 "VBNET.ATG" 
			string customEventName = t.val; 
			Expect(63);
			TypeName(
#line  1183 "VBNET.ATG" 
out type);
			if (la.kind == 138) {
				ImplementsClause(
#line  1184 "VBNET.ATG" 
out implementsClause);
			}
			EndOfStmt();
			while (StartOf(18)) {
				EventAccessorDeclaration(
#line  1187 "VBNET.ATG" 
out eventAccessorDeclaration);

#line  1189 "VBNET.ATG" 
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
			Expect(115);
			Expect(121);
			EndOfStmt();

#line  1205 "VBNET.ATG" 
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
		case 164: case 175: case 235: {

#line  1231 "VBNET.ATG" 
			ConversionType opConversionType = ConversionType.None; 
			if (la.kind == 164 || la.kind == 235) {
				if (la.kind == 235) {
					lexer.NextToken();

#line  1232 "VBNET.ATG" 
					opConversionType = ConversionType.Implicit; 
				} else {
					lexer.NextToken();

#line  1233 "VBNET.ATG" 
					opConversionType = ConversionType.Explicit;
				}
			}
			Expect(175);

#line  1236 "VBNET.ATG" 
			m.Check(Modifiers.VBOperators);
			Location startPos = t.Location;
			TypeReference returnType = NullTypeReference.Instance;
			TypeReference operandType = NullTypeReference.Instance;
			OverloadableOperatorType operatorType;
			AttributeSection section;
			ParameterDeclarationExpression param;
			List<ParameterDeclarationExpression> parameters = new List<ParameterDeclarationExpression>();
			
			OverloadableOperator(
#line  1245 "VBNET.ATG" 
out operatorType);
			Expect(37);
			FormalParameter(
#line  1247 "VBNET.ATG" 
out param);

#line  1248 "VBNET.ATG" 
			if (param != null) parameters.Add(param); 
			if (la.kind == 22) {
				lexer.NextToken();
				FormalParameter(
#line  1250 "VBNET.ATG" 
out param);

#line  1251 "VBNET.ATG" 
				if (param != null) parameters.Add(param); 
			}
			Expect(38);

#line  1254 "VBNET.ATG" 
			Location endPos = t.EndLocation; 
			if (la.kind == 63) {
				lexer.NextToken();
				while (la.kind == 40) {
					AttributeSection(
#line  1255 "VBNET.ATG" 
out section);

#line  1256 "VBNET.ATG" 
					if (section != null) {
					section.AttributeTarget = "return";
					attributes.Add(section);
					} 
				}
				TypeName(
#line  1260 "VBNET.ATG" 
out returnType);

#line  1260 "VBNET.ATG" 
				endPos = t.EndLocation; 
			}
			Expect(1);
			Block(
#line  1262 "VBNET.ATG" 
out stmt);
			Expect(115);
			Expect(175);
			EndOfStmt();

#line  1264 "VBNET.ATG" 
			OperatorDeclaration operatorDeclaration = new OperatorDeclaration {
			Modifier = m.Modifier,
			Attributes = attributes,
			Parameters = parameters,
			TypeReference = returnType,
			OverloadableOperator = operatorType,
			Name = GetReflectionNameForOperator(operatorType, opConversionType),
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
		default: SynErr(267); break;
		}
	}

	void EnumMemberDecl(
#line  778 "VBNET.ATG" 
out FieldDeclaration f) {

#line  780 "VBNET.ATG" 
		Expression expr = null;List<AttributeSection> attributes = new List<AttributeSection>();
		AttributeSection section = null;
		VariableDeclaration varDecl = null;
		
		while (la.kind == 40) {
			AttributeSection(
#line  784 "VBNET.ATG" 
out section);

#line  784 "VBNET.ATG" 
			attributes.Add(section); 
		}
		Identifier();

#line  787 "VBNET.ATG" 
		f = new FieldDeclaration(attributes);
		varDecl = new VariableDeclaration(t.val);
		f.Fields.Add(varDecl);
		f.StartLocation = varDecl.StartLocation = t.Location;
		
		if (la.kind == 20) {
			lexer.NextToken();
			Expr(
#line  792 "VBNET.ATG" 
out expr);

#line  792 "VBNET.ATG" 
			varDecl.Initializer = expr; 
		}

#line  793 "VBNET.ATG" 
		f.EndLocation = varDecl.EndLocation = t.EndLocation; 
		EndOfStmt();
	}

	void InterfaceMemberDecl() {

#line  669 "VBNET.ATG" 
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
#line  677 "VBNET.ATG" 
out section);

#line  677 "VBNET.ATG" 
				attributes.Add(section); 
			}
			while (StartOf(10)) {
				MemberModifier(
#line  680 "VBNET.ATG" 
mod);
			}
			if (la.kind == 121) {
				lexer.NextToken();

#line  684 "VBNET.ATG" 
				mod.Check(Modifiers.VBInterfaceEvents);
				Location startLocation = t.Location;
				
				Identifier();

#line  687 "VBNET.ATG" 
				name = t.val; 
				if (la.kind == 37) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
#line  688 "VBNET.ATG" 
p);
					}
					Expect(38);
				}
				if (la.kind == 63) {
					lexer.NextToken();
					TypeName(
#line  689 "VBNET.ATG" 
out type);
				}
				EndOfStmt();

#line  692 "VBNET.ATG" 
				EventDeclaration ed = new EventDeclaration {
				Name = name, TypeReference = type, Modifier = mod.Modifier,
				Parameters = p, Attributes = attributes,
				StartLocation = startLocation, EndLocation = t.EndLocation
				};
				AddChild(ed);
				
			} else if (la.kind == 213) {
				lexer.NextToken();

#line  702 "VBNET.ATG" 
				Location startLocation =  t.Location;
				mod.Check(Modifiers.VBInterfaceMethods);
				
				Identifier();

#line  705 "VBNET.ATG" 
				name = t.val; 
				TypeParameterList(
#line  706 "VBNET.ATG" 
templates);
				if (la.kind == 37) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
#line  707 "VBNET.ATG" 
p);
					}
					Expect(38);
				}
				EndOfStmt();

#line  710 "VBNET.ATG" 
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
				
			} else if (la.kind == 129) {
				lexer.NextToken();

#line  725 "VBNET.ATG" 
				mod.Check(Modifiers.VBInterfaceMethods);
				Location startLocation = t.Location;
				
				Identifier();

#line  728 "VBNET.ATG" 
				name = t.val; 
				TypeParameterList(
#line  729 "VBNET.ATG" 
templates);
				if (la.kind == 37) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
#line  730 "VBNET.ATG" 
p);
					}
					Expect(38);
				}
				if (la.kind == 63) {
					lexer.NextToken();
					while (la.kind == 40) {
						AttributeSection(
#line  731 "VBNET.ATG" 
out returnTypeAttributeSection);
					}
					TypeName(
#line  731 "VBNET.ATG" 
out type);
				}

#line  733 "VBNET.ATG" 
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
			} else if (la.kind == 189) {
				lexer.NextToken();

#line  753 "VBNET.ATG" 
				Location startLocation = t.Location;
				mod.Check(Modifiers.VBInterfaceProperties);
				
				Identifier();

#line  756 "VBNET.ATG" 
				name = t.val;  
				if (la.kind == 37) {
					lexer.NextToken();
					if (StartOf(6)) {
						FormalParameterList(
#line  757 "VBNET.ATG" 
p);
					}
					Expect(38);
				}
				if (la.kind == 63) {
					lexer.NextToken();
					TypeName(
#line  758 "VBNET.ATG" 
out type);
				}

#line  760 "VBNET.ATG" 
				if(type == null) {
				type = new TypeReference("System.Object", true);
				}
				
				EndOfStmt();

#line  766 "VBNET.ATG" 
				PropertyDeclaration pd = new PropertyDeclaration(name, type, mod.Modifier, attributes);
				pd.Parameters = p;
				pd.EndLocation = t.EndLocation;
				pd.StartLocation = startLocation;
				AddChild(pd);
				
			} else SynErr(268);
		} else if (StartOf(20)) {
			NonModuleDeclaration(
#line  774 "VBNET.ATG" 
mod, attributes);
		} else SynErr(269);
	}

	void Expr(
#line  1668 "VBNET.ATG" 
out Expression expr) {

#line  1669 "VBNET.ATG" 
		expr = null; Location startLocation = la.Location; 
		if (
#line  1672 "VBNET.ATG" 
IsQueryExpression() ) {
			QueryExpr(
#line  1673 "VBNET.ATG" 
out expr);
		} else if (StartOf(21)) {
			LambdaExpr(
#line  1674 "VBNET.ATG" 
out expr);
		} else if (StartOf(22)) {
			DisjunctionExpr(
#line  1675 "VBNET.ATG" 
out expr);
		} else SynErr(270);

#line  1678 "VBNET.ATG" 
		if (expr != null) {
		expr.StartLocation = startLocation;
		expr.EndLocation = t.EndLocation;
		}
		
	}

	void ImplementsClause(
#line  1641 "VBNET.ATG" 
out List<InterfaceImplementation> baseInterfaces) {

#line  1643 "VBNET.ATG" 
		baseInterfaces = new List<InterfaceImplementation>();
		TypeReference type = null;
		string memberName = null;
		
		Expect(138);
		NonArrayTypeName(
#line  1648 "VBNET.ATG" 
out type, false);

#line  1649 "VBNET.ATG" 
		if (type != null) memberName = TypeReference.StripLastIdentifierFromType(ref type); 

#line  1650 "VBNET.ATG" 
		baseInterfaces.Add(new InterfaceImplementation(type, memberName)); 
		while (la.kind == 22) {
			lexer.NextToken();
			NonArrayTypeName(
#line  1652 "VBNET.ATG" 
out type, false);

#line  1653 "VBNET.ATG" 
			if (type != null) memberName = TypeReference.StripLastIdentifierFromType(ref type); 

#line  1654 "VBNET.ATG" 
			baseInterfaces.Add(new InterfaceImplementation(type, memberName)); 
		}
	}

	void HandlesClause(
#line  1599 "VBNET.ATG" 
out List<string> handlesClause) {

#line  1601 "VBNET.ATG" 
		handlesClause = new List<string>();
		string name;
		
		Expect(136);
		EventMemberSpecifier(
#line  1604 "VBNET.ATG" 
out name);

#line  1604 "VBNET.ATG" 
		if (name != null) handlesClause.Add(name); 
		while (la.kind == 22) {
			lexer.NextToken();
			EventMemberSpecifier(
#line  1605 "VBNET.ATG" 
out name);

#line  1605 "VBNET.ATG" 
			if (name != null) handlesClause.Add(name); 
		}
	}

	void Block(
#line  2994 "VBNET.ATG" 
out Statement stmt) {

#line  2997 "VBNET.ATG" 
		BlockStatement blockStmt = new BlockStatement();
		/* in snippet parsing mode, t might be null */
		if (t != null) blockStmt.StartLocation = t.EndLocation;
		BlockStart(blockStmt);
		
		while (StartOf(23) || 
#line  3003 "VBNET.ATG" 
IsEndStmtAhead()) {
			if (
#line  3003 "VBNET.ATG" 
IsEndStmtAhead()) {

#line  3004 "VBNET.ATG" 
				Token first = la; 
				Expect(115);
				EndOfStmt();

#line  3007 "VBNET.ATG" 
				AddChild(new EndStatement() {
				StartLocation = first.Location,
				EndLocation = first.EndLocation }
				);
				
			} else {
				Statement();
				EndOfStmt();
			}
		}

#line  3016 "VBNET.ATG" 
		stmt = blockStmt;
		// Use start of 'End'/'Next' keyword, not the end of the previous statement
		// this is necessary to have local variables available for code completion (CodeCompletionTests.TestLocalVariablesAvailableAtEndOfForLoop)
		if (la != null) blockStmt.EndLocation = la.Location;
		BlockEnd();
		
	}

	void Charset(
#line  1591 "VBNET.ATG" 
out CharsetModifier charsetModifier) {

#line  1592 "VBNET.ATG" 
		charsetModifier = CharsetModifier.None; 
		if (la.kind == 129 || la.kind == 213) {
		} else if (la.kind == 62) {
			lexer.NextToken();

#line  1593 "VBNET.ATG" 
			charsetModifier = CharsetModifier.Ansi; 
		} else if (la.kind == 67) {
			lexer.NextToken();

#line  1594 "VBNET.ATG" 
			charsetModifier = CharsetModifier.Auto; 
		} else if (la.kind == 226) {
			lexer.NextToken();

#line  1595 "VBNET.ATG" 
			charsetModifier = CharsetModifier.Unicode; 
		} else SynErr(271);
	}

	void IdentifierForFieldDeclaration() {
		switch (la.kind) {
		case 2: {
			lexer.NextToken();
			break;
		}
		case 68: {
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
		case 67: {
			lexer.NextToken();
			break;
		}
		case 69: {
			lexer.NextToken();
			break;
		}
		case 72: {
			lexer.NextToken();
			break;
		}
		case 89: {
			lexer.NextToken();
			break;
		}
		case 106: {
			lexer.NextToken();
			break;
		}
		case 109: {
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
		case 128: {
			lexer.NextToken();
			break;
		}
		case 135: {
			lexer.NextToken();
			break;
		}
		case 141: {
			lexer.NextToken();
			break;
		}
		case 145: {
			lexer.NextToken();
			break;
		}
		case 149: {
			lexer.NextToken();
			break;
		}
		case 150: {
			lexer.NextToken();
			break;
		}
		case 173: {
			lexer.NextToken();
			break;
		}
		case 179: {
			lexer.NextToken();
			break;
		}
		case 181: {
			lexer.NextToken();
			break;
		}
		case 187: {
			lexer.NextToken();
			break;
		}
		case 206: {
			lexer.NextToken();
			break;
		}
		case 215: {
			lexer.NextToken();
			break;
		}
		case 216: {
			lexer.NextToken();
			break;
		}
		case 226: {
			lexer.NextToken();
			break;
		}
		case 227: {
			lexer.NextToken();
			break;
		}
		case 233: {
			lexer.NextToken();
			break;
		}
		case 240: {
			lexer.NextToken();
			break;
		}
		default: SynErr(272); break;
		}
	}

	void VariableDeclaratorPartAfterIdentifier(
#line  1470 "VBNET.ATG" 
List<VariableDeclaration> fieldDeclaration, string name) {

#line  1472 "VBNET.ATG" 
		Expression expr = null;
		TypeReference type = null;
		ArrayList rank = null;
		List<Expression> dimension = null;
		Location startLocation = t.Location;
		
		if (
#line  1478 "VBNET.ATG" 
IsSize() && !IsDims()) {
			ArrayInitializationModifier(
#line  1478 "VBNET.ATG" 
out dimension);
		}
		if (
#line  1479 "VBNET.ATG" 
IsDims()) {
			ArrayNameModifier(
#line  1479 "VBNET.ATG" 
out rank);
		}
		if (
#line  1481 "VBNET.ATG" 
IsObjectCreation()) {
			Expect(63);
			ObjectCreateExpression(
#line  1481 "VBNET.ATG" 
out expr);

#line  1483 "VBNET.ATG" 
			if (expr is ObjectCreateExpression) {
			type = ((ObjectCreateExpression)expr).CreateType.Clone();
			} else {
				type = ((ArrayCreateExpression)expr).CreateType.Clone();
			}
			
		} else if (StartOf(24)) {
			if (la.kind == 63) {
				lexer.NextToken();
				TypeName(
#line  1490 "VBNET.ATG" 
out type);

#line  1492 "VBNET.ATG" 
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

#line  1504 "VBNET.ATG" 
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
#line  1527 "VBNET.ATG" 
out expr);
			}
		} else SynErr(273);

#line  1530 "VBNET.ATG" 
		VariableDeclaration varDecl = new VariableDeclaration(name, expr, type);
		varDecl.StartLocation = startLocation;
		varDecl.EndLocation = t.Location;
		fieldDeclaration.Add(varDecl);
		
	}

	void VariableDeclarator(
#line  1464 "VBNET.ATG" 
List<VariableDeclaration> fieldDeclaration) {
		Identifier();

#line  1466 "VBNET.ATG" 
		string name = t.val; 
		VariableDeclaratorPartAfterIdentifier(
#line  1467 "VBNET.ATG" 
fieldDeclaration, name);
	}

	void ConstantDeclarator(
#line  1445 "VBNET.ATG" 
List<VariableDeclaration> constantDeclaration) {

#line  1447 "VBNET.ATG" 
		Expression expr = null;
		TypeReference type = null;
		string name = String.Empty;
		Location location;
		
		Identifier();

#line  1452 "VBNET.ATG" 
		name = t.val; location = t.Location; 
		if (la.kind == 63) {
			lexer.NextToken();
			TypeName(
#line  1453 "VBNET.ATG" 
out type);
		}
		Expect(20);
		Expr(
#line  1454 "VBNET.ATG" 
out expr);

#line  1456 "VBNET.ATG" 
		VariableDeclaration f = new VariableDeclaration(name, expr);
		f.TypeReference = type;
		f.StartLocation = location;
		constantDeclaration.Add(f);
		
	}

	void ObjectCreateExpression(
#line  2130 "VBNET.ATG" 
out Expression oce) {

#line  2132 "VBNET.ATG" 
		TypeReference type = null;
		CollectionInitializerExpression initializer = null;
		List<Expression> arguments = null;
		ArrayList dimensions = null;
		oce = null;
		Location startLocation = la.Location;
		bool canBeNormal; bool canBeReDim;
		
		Expect(165);
		if (StartOf(8)) {
			NonArrayTypeName(
#line  2141 "VBNET.ATG" 
out type, false);
			if (la.kind == 37) {
				lexer.NextToken();
				NormalOrReDimArgumentList(
#line  2142 "VBNET.ATG" 
out arguments, out canBeNormal, out canBeReDim);
				Expect(38);
				if (la.kind == 35 || 
#line  2143 "VBNET.ATG" 
la.kind == Tokens.OpenParenthesis) {
					if (
#line  2143 "VBNET.ATG" 
la.kind == Tokens.OpenParenthesis) {
						ArrayTypeModifiers(
#line  2144 "VBNET.ATG" 
out dimensions);
						CollectionInitializer(
#line  2145 "VBNET.ATG" 
out initializer);
					} else {
						CollectionInitializer(
#line  2146 "VBNET.ATG" 
out initializer);
					}
				}

#line  2148 "VBNET.ATG" 
				if (canBeReDim && !canBeNormal && initializer == null) initializer = new CollectionInitializerExpression(); 
			}
		}

#line  2152 "VBNET.ATG" 
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
		
		if (la.kind == 128 || la.kind == 236) {
			if (la.kind == 236) {

#line  2167 "VBNET.ATG" 
				MemberInitializerExpression memberInitializer = null;
				Expression anonymousMember = null;
				
				lexer.NextToken();

#line  2172 "VBNET.ATG" 
				CollectionInitializerExpression memberInitializers = new CollectionInitializerExpression();
				memberInitializers.StartLocation = la.Location;
				
				Expect(35);
				if (la.kind == 26 || la.kind == 150) {
					MemberInitializer(
#line  2177 "VBNET.ATG" 
out memberInitializer);

#line  2178 "VBNET.ATG" 
					memberInitializers.CreateExpressions.Add(memberInitializer); 
				} else if (StartOf(25)) {
					Expr(
#line  2179 "VBNET.ATG" 
out anonymousMember);

#line  2180 "VBNET.ATG" 
					memberInitializers.CreateExpressions.Add(anonymousMember); 
				} else SynErr(274);
				while (la.kind == 22) {
					lexer.NextToken();
					if (la.kind == 26 || la.kind == 150) {
						MemberInitializer(
#line  2184 "VBNET.ATG" 
out memberInitializer);

#line  2185 "VBNET.ATG" 
						memberInitializers.CreateExpressions.Add(memberInitializer); 
					} else if (StartOf(25)) {
						Expr(
#line  2186 "VBNET.ATG" 
out anonymousMember);

#line  2187 "VBNET.ATG" 
						memberInitializers.CreateExpressions.Add(anonymousMember); 
					} else SynErr(275);
				}
				Expect(36);

#line  2192 "VBNET.ATG" 
				memberInitializers.EndLocation = t.Location;
				if(oce is ObjectCreateExpression)
				{
					((ObjectCreateExpression)oce).ObjectInitializer = memberInitializers;
				}
				
			} else {
				lexer.NextToken();
				CollectionInitializer(
#line  2202 "VBNET.ATG" 
out initializer);

#line  2204 "VBNET.ATG" 
				if(oce is ObjectCreateExpression)
				((ObjectCreateExpression)oce).ObjectInitializer = initializer;
				
			}
		}

#line  2210 "VBNET.ATG" 
		if (oce != null) {
		oce.StartLocation = startLocation;
		oce.EndLocation = t.EndLocation;
		}
		
	}

	void AccessorDecls(
#line  1379 "VBNET.ATG" 
out PropertyGetRegion getBlock, out PropertySetRegion setBlock) {

#line  1381 "VBNET.ATG" 
		List<AttributeSection> attributes = new List<AttributeSection>();
		AttributeSection section;
		getBlock = null;
		setBlock = null; 
		
		while (la.kind == 40) {
			AttributeSection(
#line  1386 "VBNET.ATG" 
out section);

#line  1386 "VBNET.ATG" 
			attributes.Add(section); 
		}
		if (StartOf(26)) {
			GetAccessorDecl(
#line  1388 "VBNET.ATG" 
out getBlock, attributes);
			if (StartOf(27)) {

#line  1390 "VBNET.ATG" 
				attributes = new List<AttributeSection>(); 
				while (la.kind == 40) {
					AttributeSection(
#line  1391 "VBNET.ATG" 
out section);

#line  1391 "VBNET.ATG" 
					attributes.Add(section); 
				}
				SetAccessorDecl(
#line  1392 "VBNET.ATG" 
out setBlock, attributes);
			}
		} else if (StartOf(28)) {
			SetAccessorDecl(
#line  1395 "VBNET.ATG" 
out setBlock, attributes);
			if (StartOf(29)) {

#line  1397 "VBNET.ATG" 
				attributes = new List<AttributeSection>(); 
				while (la.kind == 40) {
					AttributeSection(
#line  1398 "VBNET.ATG" 
out section);

#line  1398 "VBNET.ATG" 
					attributes.Add(section); 
				}
				GetAccessorDecl(
#line  1399 "VBNET.ATG" 
out getBlock, attributes);
			}
		} else SynErr(276);
	}

	void EventAccessorDeclaration(
#line  1342 "VBNET.ATG" 
out EventAddRemoveRegion eventAccessorDeclaration) {

#line  1344 "VBNET.ATG" 
		Statement stmt = null;
		List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		eventAccessorDeclaration = null;
		
		while (la.kind == 40) {
			AttributeSection(
#line  1350 "VBNET.ATG" 
out section);

#line  1350 "VBNET.ATG" 
			attributes.Add(section); 
		}
		if (la.kind == 56) {
			lexer.NextToken();
			if (la.kind == 37) {
				lexer.NextToken();
				if (StartOf(6)) {
					FormalParameterList(
#line  1352 "VBNET.ATG" 
p);
				}
				Expect(38);
			}
			Expect(1);
			Block(
#line  1353 "VBNET.ATG" 
out stmt);
			Expect(115);
			Expect(56);
			EndOfStmt();

#line  1355 "VBNET.ATG" 
			eventAccessorDeclaration = new EventAddRegion(attributes);
			eventAccessorDeclaration.Block = (BlockStatement)stmt;
			eventAccessorDeclaration.Parameters = p;
			
		} else if (la.kind == 196) {
			lexer.NextToken();
			if (la.kind == 37) {
				lexer.NextToken();
				if (StartOf(6)) {
					FormalParameterList(
#line  1360 "VBNET.ATG" 
p);
				}
				Expect(38);
			}
			Expect(1);
			Block(
#line  1361 "VBNET.ATG" 
out stmt);
			Expect(115);
			Expect(196);
			EndOfStmt();

#line  1363 "VBNET.ATG" 
			eventAccessorDeclaration = new EventRemoveRegion(attributes);
			eventAccessorDeclaration.Block = (BlockStatement)stmt;
			eventAccessorDeclaration.Parameters = p;
			
		} else if (la.kind == 192) {
			lexer.NextToken();
			if (la.kind == 37) {
				lexer.NextToken();
				if (StartOf(6)) {
					FormalParameterList(
#line  1368 "VBNET.ATG" 
p);
				}
				Expect(38);
			}
			Expect(1);
			Block(
#line  1369 "VBNET.ATG" 
out stmt);
			Expect(115);
			Expect(192);
			EndOfStmt();

#line  1371 "VBNET.ATG" 
			eventAccessorDeclaration = new EventRaiseRegion(attributes);
			eventAccessorDeclaration.Block = (BlockStatement)stmt;
			eventAccessorDeclaration.Parameters = p;
			
		} else SynErr(277);
	}

	void OverloadableOperator(
#line  1282 "VBNET.ATG" 
out OverloadableOperatorType operatorType) {

#line  1283 "VBNET.ATG" 
		operatorType = OverloadableOperatorType.None; 
		switch (la.kind) {
		case 31: {
			lexer.NextToken();

#line  1285 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Add; 
			break;
		}
		case 30: {
			lexer.NextToken();

#line  1287 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Subtract; 
			break;
		}
		case 34: {
			lexer.NextToken();

#line  1289 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Multiply; 
			break;
		}
		case 24: {
			lexer.NextToken();

#line  1291 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Divide; 
			break;
		}
		case 25: {
			lexer.NextToken();

#line  1293 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.DivideInteger; 
			break;
		}
		case 23: {
			lexer.NextToken();

#line  1295 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Concat; 
			break;
		}
		case 153: {
			lexer.NextToken();

#line  1297 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Like; 
			break;
		}
		case 157: {
			lexer.NextToken();

#line  1299 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Modulus; 
			break;
		}
		case 60: {
			lexer.NextToken();

#line  1301 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.BitwiseAnd; 
			break;
		}
		case 178: {
			lexer.NextToken();

#line  1303 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.BitwiseOr; 
			break;
		}
		case 239: {
			lexer.NextToken();

#line  1305 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.ExclusiveOr; 
			break;
		}
		case 167: {
			lexer.NextToken();

#line  1307 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.BitNot; 
			break;
		}
		case 32: {
			lexer.NextToken();

#line  1309 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Power; 
			break;
		}
		case 44: {
			lexer.NextToken();

#line  1311 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.ShiftLeft; 
			break;
		}
		case 45: {
			lexer.NextToken();

#line  1313 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.ShiftRight; 
			break;
		}
		case 20: {
			lexer.NextToken();

#line  1315 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Equality; 
			break;
		}
		case 41: {
			lexer.NextToken();

#line  1317 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.InEquality; 
			break;
		}
		case 40: {
			lexer.NextToken();

#line  1319 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.LessThan; 
			break;
		}
		case 43: {
			lexer.NextToken();

#line  1321 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.LessThanOrEqual; 
			break;
		}
		case 39: {
			lexer.NextToken();

#line  1323 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.GreaterThan; 
			break;
		}
		case 42: {
			lexer.NextToken();

#line  1325 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.GreaterThanOrEqual; 
			break;
		}
		case 96: {
			lexer.NextToken();

#line  1327 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.CType; 
			break;
		}
		case 2: case 58: case 62: case 64: case 65: case 66: case 67: case 68: case 69: case 72: case 89: case 100: case 106: case 109: case 118: case 123: case 128: case 135: case 141: case 145: case 148: case 149: case 150: case 173: case 179: case 181: case 187: case 206: case 215: case 216: case 226: case 227: case 233: case 240: {
			Identifier();

#line  1331 "VBNET.ATG" 
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
		default: SynErr(278); break;
		}
	}

	void FormalParameter(
#line  2955 "VBNET.ATG" 
out ParameterDeclarationExpression p) {

#line  2957 "VBNET.ATG" 
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		TypeReference type = null;
		ParamModifierList mod = new ParamModifierList(this);
		Expression expr = null;
		p = null;
		ArrayList arrayModifiers = null;
		Location startLocation = la.Location;
		
		while (la.kind == 40) {
			AttributeSection(
#line  2967 "VBNET.ATG" 
out section);

#line  2967 "VBNET.ATG" 
			attributes.Add(section); 
		}
		while (StartOf(30)) {
			ParameterModifier(
#line  2968 "VBNET.ATG" 
mod);
		}
		Identifier();

#line  2969 "VBNET.ATG" 
		string parameterName = t.val; 
		if (
#line  2970 "VBNET.ATG" 
IsDims()) {
			ArrayTypeModifiers(
#line  2970 "VBNET.ATG" 
out arrayModifiers);
		}
		if (la.kind == 63) {
			lexer.NextToken();
			TypeName(
#line  2971 "VBNET.ATG" 
out type);
		}

#line  2973 "VBNET.ATG" 
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
#line  2983 "VBNET.ATG" 
out expr);
		}

#line  2985 "VBNET.ATG" 
		mod.Check();
		p = new ParameterDeclarationExpression(type, parameterName, mod.Modifier, expr);
		p.Attributes = attributes;
		p.StartLocation = startLocation;
		p.EndLocation = t.EndLocation;
		
	}

	void GetAccessorDecl(
#line  1405 "VBNET.ATG" 
out PropertyGetRegion getBlock, List<AttributeSection> attributes) {

#line  1406 "VBNET.ATG" 
		Statement stmt = null; Modifiers m; 
		PropertyAccessorAccessModifier(
#line  1408 "VBNET.ATG" 
out m);
		Expect(130);

#line  1410 "VBNET.ATG" 
		Location startLocation = t.Location; 
		Expect(1);
		Block(
#line  1412 "VBNET.ATG" 
out stmt);

#line  1413 "VBNET.ATG" 
		getBlock = new PropertyGetRegion((BlockStatement)stmt, attributes); 
		Expect(115);
		Expect(130);

#line  1415 "VBNET.ATG" 
		getBlock.Modifier = m; 

#line  1416 "VBNET.ATG" 
		getBlock.StartLocation = startLocation; getBlock.EndLocation = t.EndLocation; 
		EndOfStmt();
	}

	void SetAccessorDecl(
#line  1421 "VBNET.ATG" 
out PropertySetRegion setBlock, List<AttributeSection> attributes) {

#line  1423 "VBNET.ATG" 
		Statement stmt = null;
		List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
		Modifiers m;
		
		PropertyAccessorAccessModifier(
#line  1428 "VBNET.ATG" 
out m);
		Expect(201);

#line  1430 "VBNET.ATG" 
		Location startLocation = t.Location; 
		if (la.kind == 37) {
			lexer.NextToken();
			if (StartOf(6)) {
				FormalParameterList(
#line  1431 "VBNET.ATG" 
p);
			}
			Expect(38);
		}
		Expect(1);
		Block(
#line  1433 "VBNET.ATG" 
out stmt);

#line  1435 "VBNET.ATG" 
		setBlock = new PropertySetRegion((BlockStatement)stmt, attributes);
		setBlock.Modifier = m;
		setBlock.Parameters = p;
		
		Expect(115);
		Expect(201);

#line  1440 "VBNET.ATG" 
		setBlock.StartLocation = startLocation; setBlock.EndLocation = t.EndLocation; 
		EndOfStmt();
	}

	void PropertyAccessorAccessModifier(
#line  3758 "VBNET.ATG" 
out Modifiers m) {

#line  3759 "VBNET.ATG" 
		m = Modifiers.None; 
		while (StartOf(31)) {
			if (la.kind == 191) {
				lexer.NextToken();

#line  3761 "VBNET.ATG" 
				m |= Modifiers.Public; 
			} else if (la.kind == 190) {
				lexer.NextToken();

#line  3762 "VBNET.ATG" 
				m |= Modifiers.Protected; 
			} else if (la.kind == 127) {
				lexer.NextToken();

#line  3763 "VBNET.ATG" 
				m |= Modifiers.Internal; 
			} else {
				lexer.NextToken();

#line  3764 "VBNET.ATG" 
				m |= Modifiers.Private; 
			}
		}
	}

	void ArrayInitializationModifier(
#line  1538 "VBNET.ATG" 
out List<Expression> arrayModifiers) {

#line  1540 "VBNET.ATG" 
		arrayModifiers = null;
		
		Expect(37);
		InitializationRankList(
#line  1542 "VBNET.ATG" 
out arrayModifiers);
		Expect(38);
	}

	void ArrayNameModifier(
#line  2782 "VBNET.ATG" 
out ArrayList arrayModifiers) {

#line  2784 "VBNET.ATG" 
		arrayModifiers = null;
		
		ArrayTypeModifiers(
#line  2786 "VBNET.ATG" 
out arrayModifiers);
	}

	void InitializationRankList(
#line  1546 "VBNET.ATG" 
out List<Expression> rank) {

#line  1548 "VBNET.ATG" 
		rank = new List<Expression>();
		Expression expr = null;
		
		Expr(
#line  1551 "VBNET.ATG" 
out expr);
		if (la.kind == 219) {
			lexer.NextToken();

#line  1552 "VBNET.ATG" 
			EnsureIsZero(expr); 
			Expr(
#line  1553 "VBNET.ATG" 
out expr);
		}

#line  1555 "VBNET.ATG" 
		if (expr != null) { rank.Add(expr); } 
		while (la.kind == 22) {
			lexer.NextToken();
			Expr(
#line  1557 "VBNET.ATG" 
out expr);
			if (la.kind == 219) {
				lexer.NextToken();

#line  1558 "VBNET.ATG" 
				EnsureIsZero(expr); 
				Expr(
#line  1559 "VBNET.ATG" 
out expr);
			}

#line  1561 "VBNET.ATG" 
			if (expr != null) { rank.Add(expr); } 
		}
	}

	void CollectionInitializer(
#line  1566 "VBNET.ATG" 
out CollectionInitializerExpression outExpr) {

#line  1568 "VBNET.ATG" 
		Expression expr = null;
		CollectionInitializerExpression initializer = new CollectionInitializerExpression();
		Location startLocation = la.Location;
		
		Expect(35);
		if (StartOf(25)) {
			Expr(
#line  1574 "VBNET.ATG" 
out expr);

#line  1576 "VBNET.ATG" 
			if (expr != null) { initializer.CreateExpressions.Add(expr); }
			
			while (
#line  1579 "VBNET.ATG" 
NotFinalComma()) {
				Expect(22);
				Expr(
#line  1579 "VBNET.ATG" 
out expr);

#line  1580 "VBNET.ATG" 
				if (expr != null) { initializer.CreateExpressions.Add(expr); } 
			}
		}
		Expect(36);

#line  1585 "VBNET.ATG" 
		outExpr = initializer;
		outExpr.StartLocation = startLocation;
		outExpr.EndLocation = t.EndLocation;
		
	}

	void EventMemberSpecifier(
#line  1658 "VBNET.ATG" 
out string name) {

#line  1659 "VBNET.ATG" 
		string eventName; 
		if (StartOf(4)) {
			Identifier();
		} else if (la.kind == 161) {
			lexer.NextToken();
		} else if (la.kind == 156) {
			lexer.NextToken();
		} else SynErr(279);

#line  1662 "VBNET.ATG" 
		name = t.val; 
		Expect(26);
		IdentifierOrKeyword(
#line  1664 "VBNET.ATG" 
out eventName);

#line  1665 "VBNET.ATG" 
		name = name + "." + eventName; 
	}

	void IdentifierOrKeyword(
#line  3689 "VBNET.ATG" 
out string name) {
		lexer.NextToken();

#line  3691 "VBNET.ATG" 
		name = t.val;  
	}

	void QueryExpr(
#line  2303 "VBNET.ATG" 
out Expression expr) {

#line  2305 "VBNET.ATG" 
		QueryExpressionVB qexpr = new QueryExpressionVB();
		qexpr.StartLocation = la.Location;
		expr = qexpr;
		
		FromOrAggregateQueryOperator(
#line  2309 "VBNET.ATG" 
qexpr.Clauses);
		while (StartOf(32)) {
			QueryOperator(
#line  2310 "VBNET.ATG" 
qexpr.Clauses);
		}

#line  2312 "VBNET.ATG" 
		qexpr.EndLocation = t.EndLocation;
		
	}

	void LambdaExpr(
#line  2217 "VBNET.ATG" 
out Expression expr) {

#line  2219 "VBNET.ATG" 
		LambdaExpression lambda = null;
		
		if (la.kind == 66 || la.kind == 148 || la.kind == 213) {
			SubLambdaExpression(
#line  2221 "VBNET.ATG" 
out lambda);
		} else if (la.kind == 66 || la.kind == 129 || la.kind == 148) {
			FunctionLambdaExpression(
#line  2222 "VBNET.ATG" 
out lambda);
		} else SynErr(280);

#line  2223 "VBNET.ATG" 
		expr = lambda; 
	}

	void DisjunctionExpr(
#line  1967 "VBNET.ATG" 
out Expression outExpr) {

#line  1969 "VBNET.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		Location startLocation = la.Location;
		
		ConjunctionExpr(
#line  1973 "VBNET.ATG" 
out outExpr);
		while (la.kind == 178 || la.kind == 180 || la.kind == 239) {
			if (la.kind == 178) {
				lexer.NextToken();

#line  1976 "VBNET.ATG" 
				op = BinaryOperatorType.BitwiseOr; 
			} else if (la.kind == 180) {
				lexer.NextToken();

#line  1977 "VBNET.ATG" 
				op = BinaryOperatorType.LogicalOr; 
			} else {
				lexer.NextToken();

#line  1978 "VBNET.ATG" 
				op = BinaryOperatorType.ExclusiveOr; 
			}
			ConjunctionExpr(
#line  1980 "VBNET.ATG" 
out expr);

#line  1980 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr) { StartLocation = startLocation, EndLocation = t.EndLocation };  
		}
	}

	void AssignmentOperator(
#line  1685 "VBNET.ATG" 
out AssignmentOperatorType op) {

#line  1686 "VBNET.ATG" 
		op = AssignmentOperatorType.None; 
		switch (la.kind) {
		case 20: {
			lexer.NextToken();

#line  1687 "VBNET.ATG" 
			op = AssignmentOperatorType.Assign; 
			break;
		}
		case 54: {
			lexer.NextToken();

#line  1688 "VBNET.ATG" 
			op = AssignmentOperatorType.ConcatString; 
			break;
		}
		case 46: {
			lexer.NextToken();

#line  1689 "VBNET.ATG" 
			op = AssignmentOperatorType.Add; 
			break;
		}
		case 48: {
			lexer.NextToken();

#line  1690 "VBNET.ATG" 
			op = AssignmentOperatorType.Subtract; 
			break;
		}
		case 49: {
			lexer.NextToken();

#line  1691 "VBNET.ATG" 
			op = AssignmentOperatorType.Multiply; 
			break;
		}
		case 50: {
			lexer.NextToken();

#line  1692 "VBNET.ATG" 
			op = AssignmentOperatorType.Divide; 
			break;
		}
		case 51: {
			lexer.NextToken();

#line  1693 "VBNET.ATG" 
			op = AssignmentOperatorType.DivideInteger; 
			break;
		}
		case 47: {
			lexer.NextToken();

#line  1694 "VBNET.ATG" 
			op = AssignmentOperatorType.Power; 
			break;
		}
		case 52: {
			lexer.NextToken();

#line  1695 "VBNET.ATG" 
			op = AssignmentOperatorType.ShiftLeft; 
			break;
		}
		case 53: {
			lexer.NextToken();

#line  1696 "VBNET.ATG" 
			op = AssignmentOperatorType.ShiftRight; 
			break;
		}
		default: SynErr(281); break;
		}
	}

	void SimpleExpr(
#line  1700 "VBNET.ATG" 
out Expression pexpr) {

#line  1701 "VBNET.ATG" 
		string name; Location startLocation = la.Location; 
		SimpleNonInvocationExpression(
#line  1704 "VBNET.ATG" 
out pexpr);
		while (StartOf(33)) {
			if (la.kind == 26) {
				lexer.NextToken();
				if (la.kind == 10) {
					lexer.NextToken();
					IdentifierOrKeyword(
#line  1707 "VBNET.ATG" 
out name);
					Expect(11);

#line  1708 "VBNET.ATG" 
					pexpr = new XmlMemberAccessExpression(pexpr, XmlAxisType.Element, name, true); 
				} else if (StartOf(34)) {
					IdentifierOrKeyword(
#line  1709 "VBNET.ATG" 
out name);

#line  1710 "VBNET.ATG" 
					pexpr = new MemberReferenceExpression(pexpr, name) { StartLocation = startLocation, EndLocation = t.EndLocation }; 
				} else SynErr(282);
				if (
#line  1712 "VBNET.ATG" 
la.kind == Tokens.OpenParenthesis && Peek(1).kind == Tokens.Of) {
					lexer.NextToken();
					Expect(172);
					TypeArgumentList(
#line  1713 "VBNET.ATG" 
((MemberReferenceExpression)pexpr).TypeArguments);
					Expect(38);
				}
			} else if (la.kind == 29) {
				lexer.NextToken();
				IdentifierOrKeyword(
#line  1715 "VBNET.ATG" 
out name);

#line  1715 "VBNET.ATG" 
				pexpr = new BinaryOperatorExpression(pexpr, BinaryOperatorType.DictionaryAccess, new PrimitiveExpression(name, name) { StartLocation = t.Location, EndLocation = t.EndLocation }); 
			} else if (la.kind == 27 || la.kind == 28) {

#line  1716 "VBNET.ATG" 
				XmlAxisType type = XmlAxisType.Attribute; bool isXmlName = false; 
				if (la.kind == 28) {
					lexer.NextToken();
				} else if (la.kind == 27) {
					lexer.NextToken();

#line  1717 "VBNET.ATG" 
					type = XmlAxisType.Descendents; 
				} else SynErr(283);
				if (la.kind == 10) {
					lexer.NextToken();

#line  1717 "VBNET.ATG" 
					isXmlName = true; 
				}
				IdentifierOrKeyword(
#line  1717 "VBNET.ATG" 
out name);
				if (la.kind == 11) {
					lexer.NextToken();
				}

#line  1718 "VBNET.ATG" 
				pexpr = new XmlMemberAccessExpression(pexpr, type, name, isXmlName); 
			} else {
				InvocationExpression(
#line  1719 "VBNET.ATG" 
ref pexpr);
			}
		}

#line  1723 "VBNET.ATG" 
		if (pexpr != null) {
		pexpr.StartLocation = startLocation;
		pexpr.EndLocation = t.EndLocation;
		}
		
	}

	void SimpleNonInvocationExpression(
#line  1730 "VBNET.ATG" 
out Expression pexpr) {

#line  1732 "VBNET.ATG" 
		Expression expr;
		CollectionInitializerExpression cie;
		TypeReference type = null;
		string name = String.Empty;
		Location startLocation = la.Location;
		pexpr = null;
		
		if (StartOf(35)) {
			switch (la.kind) {
			case 3: {
				lexer.NextToken();

#line  1742 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(t.literalValue, t.val) { LiteralFormat = t.literalFormat };  
				break;
			}
			case 4: {
				lexer.NextToken();

#line  1743 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(t.literalValue, t.val) { LiteralFormat = t.literalFormat };  
				break;
			}
			case 7: {
				lexer.NextToken();

#line  1744 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(t.literalValue, t.val) { LiteralFormat = t.literalFormat };  
				break;
			}
			case 6: {
				lexer.NextToken();

#line  1745 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(t.literalValue, t.val) { LiteralFormat = t.literalFormat };  
				break;
			}
			case 5: {
				lexer.NextToken();

#line  1746 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(t.literalValue, t.val) { LiteralFormat = t.literalFormat };  
				break;
			}
			case 9: {
				lexer.NextToken();

#line  1747 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(t.literalValue, t.val) { LiteralFormat = t.literalFormat };  
				break;
			}
			case 8: {
				lexer.NextToken();

#line  1748 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(t.literalValue, t.val) { LiteralFormat = t.literalFormat };  
				break;
			}
			case 220: {
				lexer.NextToken();

#line  1750 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(true, "true");  
				break;
			}
			case 124: {
				lexer.NextToken();

#line  1751 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(false, "false"); 
				break;
			}
			case 168: {
				lexer.NextToken();

#line  1752 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(null, "null");  
				break;
			}
			case 37: {
				lexer.NextToken();
				Expr(
#line  1753 "VBNET.ATG" 
out expr);
				Expect(38);

#line  1753 "VBNET.ATG" 
				pexpr = new ParenthesizedExpression(expr); 
				break;
			}
			case 2: case 58: case 62: case 64: case 65: case 66: case 67: case 68: case 69: case 72: case 89: case 100: case 106: case 109: case 118: case 123: case 128: case 135: case 141: case 145: case 148: case 149: case 150: case 173: case 179: case 181: case 187: case 206: case 215: case 216: case 226: case 227: case 233: case 240: {
				Identifier();

#line  1755 "VBNET.ATG" 
				pexpr = new IdentifierExpression(t.val);
				pexpr.StartLocation = t.Location; pexpr.EndLocation = t.EndLocation;
				
				if (
#line  1758 "VBNET.ATG" 
la.kind == Tokens.OpenParenthesis && Peek(1).kind == Tokens.Of) {
					lexer.NextToken();
					Expect(172);
					TypeArgumentList(
#line  1759 "VBNET.ATG" 
((IdentifierExpression)pexpr).TypeArguments);
					Expect(38);
				}
				break;
			}
			case 70: case 73: case 84: case 101: case 102: case 111: case 143: case 154: case 171: case 199: case 204: case 205: case 211: case 224: case 225: case 228: {

#line  1761 "VBNET.ATG" 
				string val = String.Empty; 
				if (StartOf(12)) {
					PrimitiveTypeName(
#line  1762 "VBNET.ATG" 
out val);
				} else if (la.kind == 171) {
					lexer.NextToken();

#line  1762 "VBNET.ATG" 
					val = "System.Object"; 
				} else SynErr(284);

#line  1763 "VBNET.ATG" 
				pexpr = new TypeReferenceExpression(new TypeReference(val, true)); 
				break;
			}
			case 156: {
				lexer.NextToken();

#line  1764 "VBNET.ATG" 
				pexpr = new ThisReferenceExpression(); 
				break;
			}
			case 161: case 162: {

#line  1765 "VBNET.ATG" 
				Expression retExpr = null; 
				if (la.kind == 161) {
					lexer.NextToken();

#line  1766 "VBNET.ATG" 
					retExpr = new BaseReferenceExpression() { StartLocation = t.Location, EndLocation = t.EndLocation }; 
				} else if (la.kind == 162) {
					lexer.NextToken();

#line  1767 "VBNET.ATG" 
					retExpr = new ClassReferenceExpression() { StartLocation = t.Location, EndLocation = t.EndLocation }; 
				} else SynErr(285);
				Expect(26);
				IdentifierOrKeyword(
#line  1769 "VBNET.ATG" 
out name);

#line  1769 "VBNET.ATG" 
				pexpr = new MemberReferenceExpression(retExpr, name) { StartLocation = startLocation, EndLocation = t.EndLocation }; 
				break;
			}
			case 132: {
				lexer.NextToken();
				Expect(26);
				Identifier();

#line  1771 "VBNET.ATG" 
				type = new TypeReference(t.val ?? ""); 

#line  1773 "VBNET.ATG" 
				type.IsGlobal = true; 

#line  1774 "VBNET.ATG" 
				pexpr = new TypeReferenceExpression(type); 
				break;
			}
			case 165: {
				ObjectCreateExpression(
#line  1775 "VBNET.ATG" 
out expr);

#line  1775 "VBNET.ATG" 
				pexpr = expr; 
				break;
			}
			case 35: {
				CollectionInitializer(
#line  1776 "VBNET.ATG" 
out cie);

#line  1776 "VBNET.ATG" 
				pexpr = cie; 
				break;
			}
			case 96: case 108: case 222: {

#line  1778 "VBNET.ATG" 
				CastType castType = CastType.Cast; 
				if (la.kind == 108) {
					lexer.NextToken();
				} else if (la.kind == 96) {
					lexer.NextToken();

#line  1780 "VBNET.ATG" 
					castType = CastType.Conversion; 
				} else if (la.kind == 222) {
					lexer.NextToken();

#line  1781 "VBNET.ATG" 
					castType = CastType.TryCast; 
				} else SynErr(286);
				Expect(37);
				Expr(
#line  1783 "VBNET.ATG" 
out expr);
				Expect(22);
				TypeName(
#line  1783 "VBNET.ATG" 
out type);
				Expect(38);

#line  1784 "VBNET.ATG" 
				pexpr = new CastExpression(type, expr, castType); 
				break;
			}
			case 78: case 79: case 80: case 81: case 82: case 83: case 85: case 87: case 88: case 92: case 93: case 94: case 95: case 97: case 98: case 99: {
				CastTarget(
#line  1785 "VBNET.ATG" 
out type);
				Expect(37);
				Expr(
#line  1785 "VBNET.ATG" 
out expr);
				Expect(38);

#line  1785 "VBNET.ATG" 
				pexpr = new CastExpression(type, expr, CastType.PrimitiveConversion); 
				break;
			}
			case 57: {
				lexer.NextToken();
				Expr(
#line  1786 "VBNET.ATG" 
out expr);

#line  1786 "VBNET.ATG" 
				pexpr = new AddressOfExpression(expr); 
				break;
			}
			case 131: {
				lexer.NextToken();
				Expect(37);
				GetTypeTypeName(
#line  1787 "VBNET.ATG" 
out type);
				Expect(38);

#line  1787 "VBNET.ATG" 
				pexpr = new TypeOfExpression(type); 
				break;
			}
			case 223: {
				lexer.NextToken();
				SimpleExpr(
#line  1788 "VBNET.ATG" 
out expr);
				Expect(146);
				TypeName(
#line  1788 "VBNET.ATG" 
out type);

#line  1788 "VBNET.ATG" 
				pexpr = new TypeOfIsExpression(expr, type); 
				break;
			}
			case 137: {
				ConditionalExpression(
#line  1789 "VBNET.ATG" 
out pexpr);
				break;
			}
			case 10: case 16: case 17: case 18: case 19: {
				XmlLiteralExpression(
#line  1790 "VBNET.ATG" 
out pexpr);
				break;
			}
			}
		} else if (StartOf(36)) {
			if (la.kind == 26) {
				lexer.NextToken();
				if (la.kind == 10) {
					lexer.NextToken();
					IdentifierOrKeyword(
#line  1796 "VBNET.ATG" 
out name);
					Expect(11);

#line  1797 "VBNET.ATG" 
					pexpr = new XmlMemberAccessExpression(null, XmlAxisType.Element, name, true) { StartLocation = startLocation, EndLocation = t.EndLocation }; 
				} else if (StartOf(34)) {
					IdentifierOrKeyword(
#line  1798 "VBNET.ATG" 
out name);

#line  1799 "VBNET.ATG" 
					pexpr = new MemberReferenceExpression(null, name) { StartLocation = startLocation, EndLocation = t.EndLocation }; 
				} else SynErr(287);
			} else if (la.kind == 29) {
				lexer.NextToken();
				IdentifierOrKeyword(
#line  1801 "VBNET.ATG" 
out name);

#line  1801 "VBNET.ATG" 
				pexpr = new BinaryOperatorExpression(null, BinaryOperatorType.DictionaryAccess, new PrimitiveExpression(name, name) { StartLocation = t.Location, EndLocation = t.EndLocation }); 
			} else {

#line  1802 "VBNET.ATG" 
				XmlAxisType axisType = XmlAxisType.Element; bool isXmlIdentifier = false; 
				if (la.kind == 27) {
					lexer.NextToken();

#line  1803 "VBNET.ATG" 
					axisType = XmlAxisType.Descendents; 
				} else if (la.kind == 28) {
					lexer.NextToken();

#line  1803 "VBNET.ATG" 
					axisType = XmlAxisType.Attribute; 
				} else SynErr(288);
				if (la.kind == 10) {
					lexer.NextToken();

#line  1804 "VBNET.ATG" 
					isXmlIdentifier = true; 
				}
				IdentifierOrKeyword(
#line  1804 "VBNET.ATG" 
out name);
				if (la.kind == 11) {
					lexer.NextToken();
				}

#line  1805 "VBNET.ATG" 
				pexpr = new XmlMemberAccessExpression(null, axisType, name, isXmlIdentifier); 
			}
		} else SynErr(289);

#line  1810 "VBNET.ATG" 
		if (pexpr != null) {
		pexpr.StartLocation = startLocation;
		pexpr.EndLocation = t.EndLocation;
		}
		
	}

	void TypeArgumentList(
#line  2818 "VBNET.ATG" 
List<TypeReference> typeArguments) {

#line  2820 "VBNET.ATG" 
		TypeReference typeref;
		
		TypeName(
#line  2822 "VBNET.ATG" 
out typeref);

#line  2822 "VBNET.ATG" 
		if (typeref != null) typeArguments.Add(typeref); 
		while (la.kind == 22) {
			lexer.NextToken();
			TypeName(
#line  2825 "VBNET.ATG" 
out typeref);

#line  2825 "VBNET.ATG" 
			if (typeref != null) typeArguments.Add(typeref); 
		}
	}

	void InvocationExpression(
#line  1931 "VBNET.ATG" 
ref Expression pexpr) {

#line  1932 "VBNET.ATG" 
		List<Expression> parameters = null; 
		Expect(37);

#line  1934 "VBNET.ATG" 
		Location start = t.Location; 
		ArgumentList(
#line  1935 "VBNET.ATG" 
out parameters);
		Expect(38);

#line  1938 "VBNET.ATG" 
		pexpr = new InvocationExpression(pexpr, parameters);
		

#line  1940 "VBNET.ATG" 
		pexpr.StartLocation = start; pexpr.EndLocation = t.Location; 
	}

	void PrimitiveTypeName(
#line  3696 "VBNET.ATG" 
out string type) {

#line  3697 "VBNET.ATG" 
		type = String.Empty; 
		switch (la.kind) {
		case 70: {
			lexer.NextToken();

#line  3698 "VBNET.ATG" 
			type = "System.Boolean"; 
			break;
		}
		case 101: {
			lexer.NextToken();

#line  3699 "VBNET.ATG" 
			type = "System.DateTime"; 
			break;
		}
		case 84: {
			lexer.NextToken();

#line  3700 "VBNET.ATG" 
			type = "System.Char"; 
			break;
		}
		case 211: {
			lexer.NextToken();

#line  3701 "VBNET.ATG" 
			type = "System.String"; 
			break;
		}
		case 102: {
			lexer.NextToken();

#line  3702 "VBNET.ATG" 
			type = "System.Decimal"; 
			break;
		}
		case 73: {
			lexer.NextToken();

#line  3703 "VBNET.ATG" 
			type = "System.Byte"; 
			break;
		}
		case 204: {
			lexer.NextToken();

#line  3704 "VBNET.ATG" 
			type = "System.Int16"; 
			break;
		}
		case 143: {
			lexer.NextToken();

#line  3705 "VBNET.ATG" 
			type = "System.Int32"; 
			break;
		}
		case 154: {
			lexer.NextToken();

#line  3706 "VBNET.ATG" 
			type = "System.Int64"; 
			break;
		}
		case 205: {
			lexer.NextToken();

#line  3707 "VBNET.ATG" 
			type = "System.Single"; 
			break;
		}
		case 111: {
			lexer.NextToken();

#line  3708 "VBNET.ATG" 
			type = "System.Double"; 
			break;
		}
		case 224: {
			lexer.NextToken();

#line  3709 "VBNET.ATG" 
			type = "System.UInt32"; 
			break;
		}
		case 225: {
			lexer.NextToken();

#line  3710 "VBNET.ATG" 
			type = "System.UInt64"; 
			break;
		}
		case 228: {
			lexer.NextToken();

#line  3711 "VBNET.ATG" 
			type = "System.UInt16"; 
			break;
		}
		case 199: {
			lexer.NextToken();

#line  3712 "VBNET.ATG" 
			type = "System.SByte"; 
			break;
		}
		default: SynErr(290); break;
		}
	}

	void CastTarget(
#line  1945 "VBNET.ATG" 
out TypeReference type) {

#line  1947 "VBNET.ATG" 
		type = null;
		
		switch (la.kind) {
		case 78: {
			lexer.NextToken();

#line  1949 "VBNET.ATG" 
			type = new TypeReference("System.Boolean", true); 
			break;
		}
		case 79: {
			lexer.NextToken();

#line  1950 "VBNET.ATG" 
			type = new TypeReference("System.Byte", true); 
			break;
		}
		case 92: {
			lexer.NextToken();

#line  1951 "VBNET.ATG" 
			type = new TypeReference("System.SByte", true); 
			break;
		}
		case 80: {
			lexer.NextToken();

#line  1952 "VBNET.ATG" 
			type = new TypeReference("System.Char", true); 
			break;
		}
		case 81: {
			lexer.NextToken();

#line  1953 "VBNET.ATG" 
			type = new TypeReference("System.DateTime", true); 
			break;
		}
		case 83: {
			lexer.NextToken();

#line  1954 "VBNET.ATG" 
			type = new TypeReference("System.Decimal", true); 
			break;
		}
		case 82: {
			lexer.NextToken();

#line  1955 "VBNET.ATG" 
			type = new TypeReference("System.Double", true); 
			break;
		}
		case 93: {
			lexer.NextToken();

#line  1956 "VBNET.ATG" 
			type = new TypeReference("System.Int16", true); 
			break;
		}
		case 85: {
			lexer.NextToken();

#line  1957 "VBNET.ATG" 
			type = new TypeReference("System.Int32", true); 
			break;
		}
		case 87: {
			lexer.NextToken();

#line  1958 "VBNET.ATG" 
			type = new TypeReference("System.Int64", true); 
			break;
		}
		case 99: {
			lexer.NextToken();

#line  1959 "VBNET.ATG" 
			type = new TypeReference("System.UInt16", true); 
			break;
		}
		case 97: {
			lexer.NextToken();

#line  1960 "VBNET.ATG" 
			type = new TypeReference("System.UInt32", true); 
			break;
		}
		case 98: {
			lexer.NextToken();

#line  1961 "VBNET.ATG" 
			type = new TypeReference("System.UInt64", true); 
			break;
		}
		case 88: {
			lexer.NextToken();

#line  1962 "VBNET.ATG" 
			type = new TypeReference("System.Object", true); 
			break;
		}
		case 94: {
			lexer.NextToken();

#line  1963 "VBNET.ATG" 
			type = new TypeReference("System.Single", true); 
			break;
		}
		case 95: {
			lexer.NextToken();

#line  1964 "VBNET.ATG" 
			type = new TypeReference("System.String", true); 
			break;
		}
		default: SynErr(291); break;
		}
	}

	void GetTypeTypeName(
#line  2717 "VBNET.ATG" 
out TypeReference typeref) {

#line  2718 "VBNET.ATG" 
		ArrayList rank = null; 
		NonArrayTypeName(
#line  2720 "VBNET.ATG" 
out typeref, true);
		ArrayTypeModifiers(
#line  2721 "VBNET.ATG" 
out rank);

#line  2722 "VBNET.ATG" 
		if (rank != null && typeref != null) {
		typeref.RankSpecifier = (int[])rank.ToArray(typeof(int));
		}
		
	}

	void ConditionalExpression(
#line  1897 "VBNET.ATG" 
out Expression expr) {

#line  1899 "VBNET.ATG" 
		ConditionalExpression conditionalExpression = new ConditionalExpression();
		BinaryOperatorExpression binaryOperatorExpression = new BinaryOperatorExpression();
		conditionalExpression.StartLocation = binaryOperatorExpression.StartLocation = la.Location;
		
		Expression condition = null;
		Expression trueExpr = null;
		Expression falseExpr = null;
		
		Expect(137);
		Expect(37);
		Expr(
#line  1908 "VBNET.ATG" 
out condition);
		Expect(22);
		Expr(
#line  1908 "VBNET.ATG" 
out trueExpr);
		if (la.kind == 22) {
			lexer.NextToken();
			Expr(
#line  1908 "VBNET.ATG" 
out falseExpr);
		}
		Expect(38);

#line  1910 "VBNET.ATG" 
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

	void XmlLiteralExpression(
#line  1817 "VBNET.ATG" 
out Expression pexpr) {

#line  1819 "VBNET.ATG" 
		List<XmlExpression> exprs = new List<XmlExpression>();
		XmlExpression currentExpression = null;
		
		if (StartOf(37)) {
			XmlContentExpression(
#line  1824 "VBNET.ATG" 
exprs);
			while (StartOf(37)) {
				XmlContentExpression(
#line  1824 "VBNET.ATG" 
exprs);
			}
			if (la.kind == 10) {
				XmlElement(
#line  1824 "VBNET.ATG" 
out currentExpression);

#line  1824 "VBNET.ATG" 
				exprs.Add(currentExpression); 
				while (StartOf(37)) {
					XmlContentExpression(
#line  1824 "VBNET.ATG" 
exprs);
				}
			}
		} else if (la.kind == 10) {
			XmlElement(
#line  1826 "VBNET.ATG" 
out currentExpression);

#line  1826 "VBNET.ATG" 
			exprs.Add(currentExpression); 
			while (StartOf(37)) {
				XmlContentExpression(
#line  1826 "VBNET.ATG" 
exprs);
			}
		} else SynErr(292);

#line  1829 "VBNET.ATG" 
		if (exprs.Count > 1) {
		pexpr = new XmlDocumentExpression() { Expressions = exprs };
		} else {
			pexpr = exprs[0];
		}
		
	}

	void XmlContentExpression(
#line  1837 "VBNET.ATG" 
List<XmlExpression> exprs) {

#line  1838 "VBNET.ATG" 
		XmlContentExpression expr = null; 
		if (la.kind == 16) {
			lexer.NextToken();

#line  1840 "VBNET.ATG" 
			expr = new XmlContentExpression(t.val, XmlContentType.Text); 
		} else if (la.kind == 18) {
			lexer.NextToken();

#line  1841 "VBNET.ATG" 
			expr = new XmlContentExpression(t.val, XmlContentType.CData); 
		} else if (la.kind == 17) {
			lexer.NextToken();

#line  1842 "VBNET.ATG" 
			expr = new XmlContentExpression(t.val, XmlContentType.Comment); 
		} else if (la.kind == 19) {
			lexer.NextToken();

#line  1843 "VBNET.ATG" 
			expr = new XmlContentExpression(t.val, XmlContentType.ProcessingInstruction); 
		} else SynErr(293);

#line  1846 "VBNET.ATG" 
		expr.StartLocation = t.Location;
		expr.EndLocation = t.EndLocation;
		exprs.Add(expr);
		
	}

	void XmlElement(
#line  1872 "VBNET.ATG" 
out XmlExpression expr) {

#line  1873 "VBNET.ATG" 
		XmlElementExpression el = new XmlElementExpression(); 
		Expect(10);

#line  1876 "VBNET.ATG" 
		el.StartLocation = t.Location; 
		if (la.kind == 12) {
			lexer.NextToken();

#line  1877 "VBNET.ATG" 
			Expression innerExpression; 
			Expr(
#line  1877 "VBNET.ATG" 
out innerExpression);
			Expect(13);

#line  1878 "VBNET.ATG" 
			el.NameExpression = new XmlEmbeddedExpression() { InlineVBExpression = innerExpression }; 
		} else if (StartOf(4)) {
			Identifier();

#line  1879 "VBNET.ATG" 
			el.XmlName = t.val; 
		} else SynErr(294);
		while (StartOf(38)) {
			XmlAttribute(
#line  1879 "VBNET.ATG" 
el.Attributes);
		}
		if (la.kind == 14) {
			lexer.NextToken();

#line  1880 "VBNET.ATG" 
			el.EndLocation = t.EndLocation; 
		} else if (la.kind == 11) {
			lexer.NextToken();
			while (StartOf(39)) {

#line  1880 "VBNET.ATG" 
				XmlExpression child; 
				XmlNestedContent(
#line  1880 "VBNET.ATG" 
out child);

#line  1880 "VBNET.ATG" 
				el.Children.Add(child); 
			}
			Expect(15);
			while (StartOf(40)) {
				lexer.NextToken();
			}
			Expect(11);

#line  1880 "VBNET.ATG" 
			el.EndLocation = t.EndLocation; 
		} else SynErr(295);

#line  1882 "VBNET.ATG" 
		expr = el; 
	}

	void XmlNestedContent(
#line  1852 "VBNET.ATG" 
out XmlExpression expr) {

#line  1853 "VBNET.ATG" 
		XmlExpression tmpExpr = null; Location start = la.Location; 
		switch (la.kind) {
		case 16: {
			lexer.NextToken();

#line  1856 "VBNET.ATG" 
			tmpExpr = new XmlContentExpression(t.val, XmlContentType.Text); 
			break;
		}
		case 18: {
			lexer.NextToken();

#line  1857 "VBNET.ATG" 
			tmpExpr = new XmlContentExpression(t.val, XmlContentType.CData); 
			break;
		}
		case 17: {
			lexer.NextToken();

#line  1858 "VBNET.ATG" 
			tmpExpr = new XmlContentExpression(t.val, XmlContentType.Comment); 
			break;
		}
		case 19: {
			lexer.NextToken();

#line  1859 "VBNET.ATG" 
			tmpExpr = new XmlContentExpression(t.val, XmlContentType.ProcessingInstruction); 
			break;
		}
		case 12: {
			lexer.NextToken();

#line  1860 "VBNET.ATG" 
			Expression innerExpression; 
			Expr(
#line  1860 "VBNET.ATG" 
out innerExpression);
			Expect(13);

#line  1860 "VBNET.ATG" 
			tmpExpr = new XmlEmbeddedExpression() { InlineVBExpression = innerExpression }; 
			break;
		}
		case 10: {
			XmlElement(
#line  1861 "VBNET.ATG" 
out tmpExpr);
			break;
		}
		default: SynErr(296); break;
		}

#line  1864 "VBNET.ATG" 
		if (tmpExpr.StartLocation.IsEmpty)
		tmpExpr.StartLocation = start;
		if (tmpExpr.EndLocation.IsEmpty)
			tmpExpr.EndLocation = t.EndLocation;
		expr = tmpExpr;
		
	}

	void XmlAttribute(
#line  1885 "VBNET.ATG" 
List<XmlExpression> attrs) {

#line  1886 "VBNET.ATG" 
		Location start = la.Location; 
		if (StartOf(4)) {
			Identifier();

#line  1888 "VBNET.ATG" 
			string name = t.val; 
			Expect(20);

#line  1889 "VBNET.ATG" 
			string literalValue = null; Expression expressionValue = null; bool useDoubleQuotes = false; 
			if (la.kind == 3) {
				lexer.NextToken();

#line  1890 "VBNET.ATG" 
				literalValue = t.literalValue.ToString(); useDoubleQuotes = t.val[0] == '"'; 
			} else if (la.kind == 12) {
				lexer.NextToken();
				Expr(
#line  1890 "VBNET.ATG" 
out expressionValue);
				Expect(13);
			} else SynErr(297);

#line  1891 "VBNET.ATG" 
			attrs.Add(new XmlAttributeExpression() { Name = name, ExpressionValue = expressionValue, LiteralValue = literalValue, UseDoubleQuotes = useDoubleQuotes, StartLocation = start, EndLocation = t.EndLocation }); 
		} else if (la.kind == 12) {
			lexer.NextToken();

#line  1893 "VBNET.ATG" 
			Expression innerExpression; 
			Expr(
#line  1893 "VBNET.ATG" 
out innerExpression);
			Expect(13);

#line  1894 "VBNET.ATG" 
			attrs.Add(new XmlEmbeddedExpression() { InlineVBExpression = innerExpression, StartLocation = start, EndLocation = t.EndLocation }); 
		} else SynErr(298);
	}

	void ArgumentList(
#line  2646 "VBNET.ATG" 
out List<Expression> arguments) {

#line  2648 "VBNET.ATG" 
		arguments = new List<Expression>();
		Expression expr = null;
		
		if (StartOf(25)) {
			Argument(
#line  2651 "VBNET.ATG" 
out expr);
		}
		while (la.kind == 22) {
			lexer.NextToken();

#line  2652 "VBNET.ATG" 
			arguments.Add(expr ?? Expression.Null); expr = null; 
			if (StartOf(25)) {
				Argument(
#line  2653 "VBNET.ATG" 
out expr);
			}

#line  2654 "VBNET.ATG" 
			if (expr == null) expr = Expression.Null; 
		}

#line  2656 "VBNET.ATG" 
		if (expr != null) arguments.Add(expr); 
	}

	void ConjunctionExpr(
#line  1984 "VBNET.ATG" 
out Expression outExpr) {

#line  1986 "VBNET.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		Location startLocation = la.Location;
		
		NotExpr(
#line  1990 "VBNET.ATG" 
out outExpr);
		while (la.kind == 60 || la.kind == 61) {
			if (la.kind == 60) {
				lexer.NextToken();

#line  1993 "VBNET.ATG" 
				op = BinaryOperatorType.BitwiseAnd; 
			} else {
				lexer.NextToken();

#line  1994 "VBNET.ATG" 
				op = BinaryOperatorType.LogicalAnd; 
			}
			NotExpr(
#line  1996 "VBNET.ATG" 
out expr);

#line  1996 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr) { StartLocation = startLocation, EndLocation = t.EndLocation };  
		}
	}

	void NotExpr(
#line  2000 "VBNET.ATG" 
out Expression outExpr) {

#line  2001 "VBNET.ATG" 
		UnaryOperatorType uop = UnaryOperatorType.None; 
		while (la.kind == 167) {
			lexer.NextToken();

#line  2002 "VBNET.ATG" 
			uop = UnaryOperatorType.Not; 
		}
		ComparisonExpr(
#line  2003 "VBNET.ATG" 
out outExpr);

#line  2004 "VBNET.ATG" 
		if (uop != UnaryOperatorType.None)
		outExpr = new UnaryOperatorExpression(outExpr, uop);
		
	}

	void ComparisonExpr(
#line  2009 "VBNET.ATG" 
out Expression outExpr) {

#line  2011 "VBNET.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		Location startLocation = la.Location;
		
		ShiftExpr(
#line  2015 "VBNET.ATG" 
out outExpr);
		while (StartOf(41)) {
			switch (la.kind) {
			case 40: {
				lexer.NextToken();

#line  2018 "VBNET.ATG" 
				op = BinaryOperatorType.LessThan; 
				break;
			}
			case 39: {
				lexer.NextToken();

#line  2019 "VBNET.ATG" 
				op = BinaryOperatorType.GreaterThan; 
				break;
			}
			case 43: {
				lexer.NextToken();

#line  2020 "VBNET.ATG" 
				op = BinaryOperatorType.LessThanOrEqual; 
				break;
			}
			case 42: {
				lexer.NextToken();

#line  2021 "VBNET.ATG" 
				op = BinaryOperatorType.GreaterThanOrEqual; 
				break;
			}
			case 41: {
				lexer.NextToken();

#line  2022 "VBNET.ATG" 
				op = BinaryOperatorType.InEquality; 
				break;
			}
			case 20: {
				lexer.NextToken();

#line  2023 "VBNET.ATG" 
				op = BinaryOperatorType.Equality; 
				break;
			}
			case 153: {
				lexer.NextToken();

#line  2024 "VBNET.ATG" 
				op = BinaryOperatorType.Like; 
				break;
			}
			case 146: {
				lexer.NextToken();

#line  2025 "VBNET.ATG" 
				op = BinaryOperatorType.ReferenceEquality; 
				break;
			}
			case 147: {
				lexer.NextToken();

#line  2026 "VBNET.ATG" 
				op = BinaryOperatorType.ReferenceInequality; 
				break;
			}
			}
			if (StartOf(42)) {
				ShiftExpr(
#line  2029 "VBNET.ATG" 
out expr);

#line  2029 "VBNET.ATG" 
				outExpr = new BinaryOperatorExpression(outExpr, op, expr) { StartLocation = startLocation, EndLocation = t.EndLocation };  
			} else if (la.kind == 167) {

#line  2030 "VBNET.ATG" 
				Location startLocation2 = la.Location; 
				lexer.NextToken();
				ShiftExpr(
#line  2032 "VBNET.ATG" 
out expr);

#line  2032 "VBNET.ATG" 
				outExpr = new BinaryOperatorExpression(outExpr, op, new UnaryOperatorExpression(expr, UnaryOperatorType.Not) { StartLocation = startLocation2, EndLocation = t.EndLocation }) { StartLocation = startLocation, EndLocation = t.EndLocation };  
			} else SynErr(299);
		}
	}

	void ShiftExpr(
#line  2037 "VBNET.ATG" 
out Expression outExpr) {

#line  2039 "VBNET.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		Location startLocation = la.Location;
		
		ConcatenationExpr(
#line  2043 "VBNET.ATG" 
out outExpr);
		while (la.kind == 44 || la.kind == 45) {
			if (la.kind == 44) {
				lexer.NextToken();

#line  2046 "VBNET.ATG" 
				op = BinaryOperatorType.ShiftLeft; 
			} else {
				lexer.NextToken();

#line  2047 "VBNET.ATG" 
				op = BinaryOperatorType.ShiftRight; 
			}
			ConcatenationExpr(
#line  2049 "VBNET.ATG" 
out expr);

#line  2049 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr) { StartLocation = startLocation, EndLocation = t.EndLocation };  
		}
	}

	void ConcatenationExpr(
#line  2053 "VBNET.ATG" 
out Expression outExpr) {

#line  2054 "VBNET.ATG" 
		Expression expr; Location startLocation = la.Location; 
		AdditiveExpr(
#line  2056 "VBNET.ATG" 
out outExpr);
		while (la.kind == 23) {
			lexer.NextToken();
			AdditiveExpr(
#line  2056 "VBNET.ATG" 
out expr);

#line  2056 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.Concat, expr) { StartLocation = startLocation, EndLocation = t.EndLocation };  
		}
	}

	void AdditiveExpr(
#line  2059 "VBNET.ATG" 
out Expression outExpr) {

#line  2061 "VBNET.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		Location startLocation = la.Location;
		
		ModuloExpr(
#line  2065 "VBNET.ATG" 
out outExpr);
		while (la.kind == 30 || la.kind == 31) {
			if (la.kind == 31) {
				lexer.NextToken();

#line  2068 "VBNET.ATG" 
				op = BinaryOperatorType.Add; 
			} else {
				lexer.NextToken();

#line  2069 "VBNET.ATG" 
				op = BinaryOperatorType.Subtract; 
			}
			ModuloExpr(
#line  2071 "VBNET.ATG" 
out expr);

#line  2071 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr) { StartLocation = startLocation, EndLocation = t.EndLocation };  
		}
	}

	void ModuloExpr(
#line  2075 "VBNET.ATG" 
out Expression outExpr) {

#line  2076 "VBNET.ATG" 
		Expression expr; Location startLocation = la.Location; 
		IntegerDivisionExpr(
#line  2078 "VBNET.ATG" 
out outExpr);
		while (la.kind == 157) {
			lexer.NextToken();
			IntegerDivisionExpr(
#line  2078 "VBNET.ATG" 
out expr);

#line  2078 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.Modulus, expr) { StartLocation = startLocation, EndLocation = t.EndLocation };  
		}
	}

	void IntegerDivisionExpr(
#line  2081 "VBNET.ATG" 
out Expression outExpr) {

#line  2082 "VBNET.ATG" 
		Expression expr; Location startLocation = la.Location; 
		MultiplicativeExpr(
#line  2084 "VBNET.ATG" 
out outExpr);
		while (la.kind == 25) {
			lexer.NextToken();
			MultiplicativeExpr(
#line  2084 "VBNET.ATG" 
out expr);

#line  2084 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.DivideInteger, expr) { StartLocation = startLocation, EndLocation = t.EndLocation };  
		}
	}

	void MultiplicativeExpr(
#line  2087 "VBNET.ATG" 
out Expression outExpr) {

#line  2089 "VBNET.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		Location startLocation = la.Location;
		
		UnaryExpr(
#line  2093 "VBNET.ATG" 
out outExpr);
		while (la.kind == 24 || la.kind == 34) {
			if (la.kind == 34) {
				lexer.NextToken();

#line  2096 "VBNET.ATG" 
				op = BinaryOperatorType.Multiply; 
			} else {
				lexer.NextToken();

#line  2097 "VBNET.ATG" 
				op = BinaryOperatorType.Divide; 
			}
			UnaryExpr(
#line  2099 "VBNET.ATG" 
out expr);

#line  2099 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr) { StartLocation = startLocation, EndLocation = t.EndLocation }; 
		}
	}

	void UnaryExpr(
#line  2103 "VBNET.ATG" 
out Expression uExpr) {

#line  2105 "VBNET.ATG" 
		Expression expr;
		UnaryOperatorType uop = UnaryOperatorType.None;
		Location startLocation = la.Location;
		bool isUOp = false;
		
		while (la.kind == 30 || la.kind == 31 || la.kind == 34) {
			if (la.kind == 31) {
				lexer.NextToken();

#line  2110 "VBNET.ATG" 
				uop = UnaryOperatorType.Plus; isUOp = true; 
			} else if (la.kind == 30) {
				lexer.NextToken();

#line  2111 "VBNET.ATG" 
				uop = UnaryOperatorType.Minus; isUOp = true; 
			} else {
				lexer.NextToken();

#line  2112 "VBNET.ATG" 
				uop = UnaryOperatorType.Dereference;  isUOp = true;
			}
		}
		ExponentiationExpr(
#line  2114 "VBNET.ATG" 
out expr);

#line  2116 "VBNET.ATG" 
		if (isUOp) {
		uExpr = new UnaryOperatorExpression(expr, uop) { StartLocation = startLocation, EndLocation = t.EndLocation };
		} else {
			uExpr = expr;
		}
		
	}

	void ExponentiationExpr(
#line  2124 "VBNET.ATG" 
out Expression outExpr) {

#line  2125 "VBNET.ATG" 
		Expression expr; Location startLocation = la.Location; 
		SimpleExpr(
#line  2127 "VBNET.ATG" 
out outExpr);
		while (la.kind == 32) {
			lexer.NextToken();
			SimpleExpr(
#line  2127 "VBNET.ATG" 
out expr);

#line  2127 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.Power, expr) { StartLocation = startLocation, EndLocation = t.EndLocation };  
		}
	}

	void NormalOrReDimArgumentList(
#line  2660 "VBNET.ATG" 
out List<Expression> arguments, out bool canBeNormal, out bool canBeRedim) {

#line  2662 "VBNET.ATG" 
		arguments = new List<Expression>();
		canBeNormal = true; canBeRedim = !IsNamedAssign();
		Expression expr = null;
		
		if (StartOf(25)) {
			Argument(
#line  2667 "VBNET.ATG" 
out expr);
			if (la.kind == 219) {
				lexer.NextToken();

#line  2668 "VBNET.ATG" 
				EnsureIsZero(expr); canBeNormal = false; 
				Expr(
#line  2669 "VBNET.ATG" 
out expr);
			}
		}
		while (la.kind == 22) {
			lexer.NextToken();

#line  2672 "VBNET.ATG" 
			if (expr == null) canBeRedim = false; 

#line  2673 "VBNET.ATG" 
			arguments.Add(expr ?? Expression.Null); expr = null; 

#line  2674 "VBNET.ATG" 
			canBeRedim &= !IsNamedAssign(); 
			if (StartOf(25)) {
				Argument(
#line  2675 "VBNET.ATG" 
out expr);
				if (la.kind == 219) {
					lexer.NextToken();

#line  2676 "VBNET.ATG" 
					EnsureIsZero(expr); canBeNormal = false; 
					Expr(
#line  2677 "VBNET.ATG" 
out expr);
				}
			}

#line  2679 "VBNET.ATG" 
			if (expr == null) { canBeRedim = false; expr = Expression.Null; } 
		}

#line  2681 "VBNET.ATG" 
		if (expr != null) arguments.Add(expr); else canBeRedim = false; 
	}

	void ArrayTypeModifiers(
#line  2791 "VBNET.ATG" 
out ArrayList arrayModifiers) {

#line  2793 "VBNET.ATG" 
		arrayModifiers = new ArrayList();
		int i = 0;
		
		while (
#line  2796 "VBNET.ATG" 
IsDims()) {
			Expect(37);
			if (la.kind == 22 || la.kind == 38) {
				RankList(
#line  2798 "VBNET.ATG" 
out i);
			}

#line  2800 "VBNET.ATG" 
			arrayModifiers.Add(i);
			
			Expect(38);
		}

#line  2805 "VBNET.ATG" 
		if(arrayModifiers.Count == 0) {
		 arrayModifiers = null;
		}
		
	}

	void MemberInitializer(
#line  2627 "VBNET.ATG" 
out MemberInitializerExpression memberInitializer) {

#line  2629 "VBNET.ATG" 
		memberInitializer = new MemberInitializerExpression();
		memberInitializer.StartLocation = la.Location;
		Expression initExpr = null;
		bool isKey = false;
		string name = null;
		
		if (la.kind == 150) {
			lexer.NextToken();

#line  2635 "VBNET.ATG" 
			isKey = true; 
		}
		Expect(26);
		IdentifierOrKeyword(
#line  2636 "VBNET.ATG" 
out name);
		Expect(20);
		Expr(
#line  2636 "VBNET.ATG" 
out initExpr);

#line  2638 "VBNET.ATG" 
		memberInitializer.Name = name;
		memberInitializer.Expression = initExpr;
		memberInitializer.IsKey = isKey;
		memberInitializer.EndLocation = t.EndLocation;
		
	}

	void SubLambdaExpression(
#line  2226 "VBNET.ATG" 
out LambdaExpression lambda) {

#line  2228 "VBNET.ATG" 
		lambda = new LambdaExpression();
		lambda.ReturnType = new TypeReference("System.Void", true);
		Expression inner = null;
		Statement statement = null;
		lambda.StartLocation = la.Location;
		
		while (la.kind == 66 || la.kind == 148) {
			if (la.kind == 66) {
				lexer.NextToken();

#line  2235 "VBNET.ATG" 
				lambda.IsAsync = true; 
			} else {
				lexer.NextToken();

#line  2235 "VBNET.ATG" 
				lambda.IsIterator = true; 
			}
		}
		Expect(213);
		if (la.kind == 37) {
			lexer.NextToken();
			if (StartOf(6)) {
				FormalParameterList(
#line  2235 "VBNET.ATG" 
lambda.Parameters);
			}
			Expect(38);
		}
		if (StartOf(43)) {
			if (StartOf(25)) {
				Expr(
#line  2238 "VBNET.ATG" 
out inner);

#line  2240 "VBNET.ATG" 
				lambda.ExpressionBody = inner;
				lambda.EndLocation = t.EndLocation; // la.Location?
				lambda.ExtendedEndLocation = la.Location;
				
			} else {
				EmbeddedStatement(
#line  2245 "VBNET.ATG" 
out statement);

#line  2247 "VBNET.ATG" 
				lambda.StatementBody = statement;
				lambda.EndLocation = t.EndLocation;
				lambda.ExtendedEndLocation = la.Location;
				
			}
		} else if (la.kind == 1) {
			lexer.NextToken();
			Block(
#line  2254 "VBNET.ATG" 
out statement);
			Expect(115);
			Expect(213);

#line  2257 "VBNET.ATG" 
			lambda.StatementBody = statement;
			lambda.EndLocation = t.EndLocation;
			lambda.ExtendedEndLocation = la.Location;
			
		} else SynErr(300);
	}

	void FunctionLambdaExpression(
#line  2264 "VBNET.ATG" 
out LambdaExpression lambda) {

#line  2266 "VBNET.ATG" 
		lambda = new LambdaExpression();
		TypeReference typeRef = null;
		Expression inner = null;
		Statement statement = null;
		lambda.StartLocation = la.Location;
		
		while (la.kind == 66 || la.kind == 148) {
			if (la.kind == 66) {
				lexer.NextToken();

#line  2273 "VBNET.ATG" 
				lambda.IsAsync = true; 
			} else {
				lexer.NextToken();

#line  2273 "VBNET.ATG" 
				lambda.IsIterator = true; 
			}
		}
		Expect(129);
		if (la.kind == 37) {
			lexer.NextToken();
			if (StartOf(6)) {
				FormalParameterList(
#line  2273 "VBNET.ATG" 
lambda.Parameters);
			}
			Expect(38);
		}
		if (la.kind == 63) {
			lexer.NextToken();
			TypeName(
#line  2274 "VBNET.ATG" 
out typeRef);

#line  2274 "VBNET.ATG" 
			lambda.ReturnType = typeRef; 
		}
		if (StartOf(43)) {
			if (StartOf(25)) {
				Expr(
#line  2277 "VBNET.ATG" 
out inner);

#line  2279 "VBNET.ATG" 
				lambda.ExpressionBody = inner;
				lambda.EndLocation = t.EndLocation;
				lambda.ExtendedEndLocation = la.Location;
				
			} else {
				EmbeddedStatement(
#line  2284 "VBNET.ATG" 
out statement);

#line  2286 "VBNET.ATG" 
				lambda.StatementBody = statement;
				lambda.EndLocation = t.EndLocation;
				lambda.ExtendedEndLocation = la.Location;
				
			}
		} else if (la.kind == 1) {
			lexer.NextToken();
			Block(
#line  2293 "VBNET.ATG" 
out statement);
			Expect(115);
			Expect(129);

#line  2296 "VBNET.ATG" 
			lambda.StatementBody = statement;
			lambda.EndLocation = t.EndLocation;
			lambda.ExtendedEndLocation = la.Location;
			
		} else SynErr(301);
	}

	void EmbeddedStatement(
#line  3079 "VBNET.ATG" 
out Statement statement) {

#line  3081 "VBNET.ATG" 
		Statement embeddedStatement = null;
		statement = null;
		Expression expr = null;
		string name = String.Empty;
		List<Expression> p = null;
		Location startLocation = la.Location;
		
		if (la.kind == 122) {
			lexer.NextToken();

#line  3089 "VBNET.ATG" 
			ExitType exitType = ExitType.None; 
			switch (la.kind) {
			case 213: {
				lexer.NextToken();

#line  3091 "VBNET.ATG" 
				exitType = ExitType.Sub; 
				break;
			}
			case 129: {
				lexer.NextToken();

#line  3093 "VBNET.ATG" 
				exitType = ExitType.Function; 
				break;
			}
			case 189: {
				lexer.NextToken();

#line  3095 "VBNET.ATG" 
				exitType = ExitType.Property; 
				break;
			}
			case 110: {
				lexer.NextToken();

#line  3097 "VBNET.ATG" 
				exitType = ExitType.Do; 
				break;
			}
			case 126: {
				lexer.NextToken();

#line  3099 "VBNET.ATG" 
				exitType = ExitType.For; 
				break;
			}
			case 221: {
				lexer.NextToken();

#line  3101 "VBNET.ATG" 
				exitType = ExitType.Try; 
				break;
			}
			case 234: {
				lexer.NextToken();

#line  3103 "VBNET.ATG" 
				exitType = ExitType.While; 
				break;
			}
			case 200: {
				lexer.NextToken();

#line  3105 "VBNET.ATG" 
				exitType = ExitType.Select; 
				break;
			}
			default: SynErr(302); break;
			}

#line  3107 "VBNET.ATG" 
			statement = new ExitStatement(exitType); 
		} else if (la.kind == 221) {
			TryStatement(
#line  3108 "VBNET.ATG" 
out statement);
		} else if (la.kind == 91) {
			lexer.NextToken();

#line  3109 "VBNET.ATG" 
			ContinueType continueType = ContinueType.None; 
			if (la.kind == 110 || la.kind == 126 || la.kind == 234) {
				if (la.kind == 110) {
					lexer.NextToken();

#line  3109 "VBNET.ATG" 
					continueType = ContinueType.Do; 
				} else if (la.kind == 126) {
					lexer.NextToken();

#line  3109 "VBNET.ATG" 
					continueType = ContinueType.For; 
				} else {
					lexer.NextToken();

#line  3109 "VBNET.ATG" 
					continueType = ContinueType.While; 
				}
			}

#line  3109 "VBNET.ATG" 
			statement = new ContinueStatement(continueType); 
		} else if (la.kind == 218) {
			lexer.NextToken();
			if (StartOf(25)) {
				Expr(
#line  3111 "VBNET.ATG" 
out expr);
			}

#line  3111 "VBNET.ATG" 
			statement = new ThrowStatement(expr); 
		} else if (la.kind == 198) {
			lexer.NextToken();
			if (StartOf(25)) {
				Expr(
#line  3113 "VBNET.ATG" 
out expr);
			}

#line  3113 "VBNET.ATG" 
			statement = new ReturnStatement(expr); 
		} else if (la.kind == 240) {
			lexer.NextToken();
			if (StartOf(25)) {
				Expr(
#line  3114 "VBNET.ATG" 
out expr);
			}

#line  3114 "VBNET.ATG" 
			statement = new YieldStatement(new ReturnStatement(expr)); 
		} else if (la.kind == 214) {
			lexer.NextToken();
			Expr(
#line  3116 "VBNET.ATG" 
out expr);
			EndOfStmt();
			Block(
#line  3116 "VBNET.ATG" 
out embeddedStatement);
			Expect(115);
			Expect(214);

#line  3117 "VBNET.ATG" 
			statement = new LockStatement(expr, embeddedStatement); 
		} else if (la.kind == 192) {
			lexer.NextToken();
			Identifier();

#line  3119 "VBNET.ATG" 
			name = t.val; 
			if (la.kind == 37) {
				lexer.NextToken();
				if (StartOf(44)) {
					ArgumentList(
#line  3120 "VBNET.ATG" 
out p);
				}
				Expect(38);
			}

#line  3122 "VBNET.ATG" 
			statement = new RaiseEventStatement(name, p);
			
		} else if (la.kind == 236) {
			WithStatement(
#line  3125 "VBNET.ATG" 
out statement);
		} else if (la.kind == 56) {
			lexer.NextToken();

#line  3127 "VBNET.ATG" 
			Expression handlerExpr = null; 
			Expr(
#line  3128 "VBNET.ATG" 
out expr);
			Expect(22);
			Expr(
#line  3128 "VBNET.ATG" 
out handlerExpr);

#line  3130 "VBNET.ATG" 
			statement = new AddHandlerStatement(expr, handlerExpr);
			
		} else if (la.kind == 196) {
			lexer.NextToken();

#line  3133 "VBNET.ATG" 
			Expression handlerExpr = null; 
			Expr(
#line  3134 "VBNET.ATG" 
out expr);
			Expect(22);
			Expr(
#line  3134 "VBNET.ATG" 
out handlerExpr);

#line  3136 "VBNET.ATG" 
			statement = new RemoveHandlerStatement(expr, handlerExpr);
			
		} else if (la.kind == 234) {
			lexer.NextToken();
			Expr(
#line  3139 "VBNET.ATG" 
out expr);
			EndOfStmt();
			Block(
#line  3140 "VBNET.ATG" 
out embeddedStatement);
			Expect(115);
			Expect(234);

#line  3142 "VBNET.ATG" 
			statement = new DoLoopStatement(expr, embeddedStatement, ConditionType.While, ConditionPosition.Start);
			
		} else if (la.kind == 110) {
			lexer.NextToken();

#line  3147 "VBNET.ATG" 
			ConditionType conditionType = ConditionType.None;
			
			if (la.kind == 227 || la.kind == 234) {
				WhileOrUntil(
#line  3150 "VBNET.ATG" 
out conditionType);
				Expr(
#line  3150 "VBNET.ATG" 
out expr);
				EndOfStmt();
				Block(
#line  3151 "VBNET.ATG" 
out embeddedStatement);
				Expect(155);

#line  3154 "VBNET.ATG" 
				statement = new DoLoopStatement(expr, 
				                               embeddedStatement, 
				                               conditionType == ConditionType.While ? ConditionType.DoWhile : conditionType, 
				                               ConditionPosition.Start);
				
			} else if (la.kind == 1 || la.kind == 21) {
				EndOfStmt();
				Block(
#line  3161 "VBNET.ATG" 
out embeddedStatement);
				Expect(155);
				if (la.kind == 227 || la.kind == 234) {
					WhileOrUntil(
#line  3162 "VBNET.ATG" 
out conditionType);
					Expr(
#line  3162 "VBNET.ATG" 
out expr);
				}

#line  3164 "VBNET.ATG" 
				statement = new DoLoopStatement(expr, embeddedStatement, conditionType, ConditionPosition.End);
				
			} else SynErr(303);
		} else if (la.kind == 126) {
			lexer.NextToken();

#line  3169 "VBNET.ATG" 
			Expression group = null;
			TypeReference typeReference;
			string        typeName;
			
			if (la.kind == 112) {
				lexer.NextToken();
				LoopControlVariable(
#line  3175 "VBNET.ATG" 
out typeReference, out typeName);
				Expect(140);
				Expr(
#line  3176 "VBNET.ATG" 
out group);
				EndOfStmt();
				Block(
#line  3177 "VBNET.ATG" 
out embeddedStatement);
				Expect(166);
				if (StartOf(25)) {
					Expr(
#line  3178 "VBNET.ATG" 
out expr);
				}

#line  3180 "VBNET.ATG" 
				statement = new ForeachStatement(typeReference, 
				                                typeName,
				                                group, 
				                                embeddedStatement, 
				                                expr);
				statement.StartLocation = startLocation;
				statement.EndLocation   = t.EndLocation;
				
				
			} else if (StartOf(45)) {

#line  3191 "VBNET.ATG" 
				Expression start = null;
				Expression end = null;
				Expression step = null;
				Expression variableExpr = null;
				Expression nextExpr = null;
				List<Expression> nextExpressions = null;
				
				if (
#line  3198 "VBNET.ATG" 
IsLoopVariableDeclaration()) {
					LoopControlVariable(
#line  3199 "VBNET.ATG" 
out typeReference, out typeName);
				} else {

#line  3201 "VBNET.ATG" 
					typeReference = null; typeName = null; 
					SimpleExpr(
#line  3202 "VBNET.ATG" 
out variableExpr);
				}
				Expect(20);
				Expr(
#line  3204 "VBNET.ATG" 
out start);
				Expect(219);
				Expr(
#line  3204 "VBNET.ATG" 
out end);
				if (la.kind == 208) {
					lexer.NextToken();
					Expr(
#line  3204 "VBNET.ATG" 
out step);
				}
				EndOfStmt();
				Block(
#line  3205 "VBNET.ATG" 
out embeddedStatement);
				Expect(166);
				if (StartOf(25)) {
					Expr(
#line  3208 "VBNET.ATG" 
out nextExpr);

#line  3210 "VBNET.ATG" 
					nextExpressions = new List<Expression>();
					nextExpressions.Add(nextExpr);
					
					while (la.kind == 22) {
						lexer.NextToken();
						Expr(
#line  3213 "VBNET.ATG" 
out nextExpr);

#line  3213 "VBNET.ATG" 
						nextExpressions.Add(nextExpr); 
					}
				}

#line  3216 "VBNET.ATG" 
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
				
			} else SynErr(304);
		} else if (la.kind == 120) {
			lexer.NextToken();
			Expr(
#line  3229 "VBNET.ATG" 
out expr);

#line  3229 "VBNET.ATG" 
			statement = new ErrorStatement(expr); 
		} else if (la.kind == 194) {
			lexer.NextToken();

#line  3231 "VBNET.ATG" 
			bool isPreserve = false; 
			if (la.kind == 187) {
				lexer.NextToken();

#line  3231 "VBNET.ATG" 
				isPreserve = true; 
			}
			ReDimClause(
#line  3232 "VBNET.ATG" 
out expr);

#line  3234 "VBNET.ATG" 
			ReDimStatement reDimStatement = new ReDimStatement(isPreserve);
			statement = reDimStatement;
			SafeAdd(reDimStatement, reDimStatement.ReDimClauses, expr as InvocationExpression);
			
			while (la.kind == 22) {
				lexer.NextToken();
				ReDimClause(
#line  3238 "VBNET.ATG" 
out expr);

#line  3239 "VBNET.ATG" 
				SafeAdd(reDimStatement, reDimStatement.ReDimClauses, expr as InvocationExpression); 
			}
		} else if (la.kind == 119) {
			lexer.NextToken();
			Expr(
#line  3243 "VBNET.ATG" 
out expr);

#line  3245 "VBNET.ATG" 
			EraseStatement eraseStatement = new EraseStatement();
			if (expr != null) { SafeAdd(eraseStatement, eraseStatement.Expressions, expr);}
			
			while (la.kind == 22) {
				lexer.NextToken();
				Expr(
#line  3248 "VBNET.ATG" 
out expr);

#line  3248 "VBNET.ATG" 
				if (expr != null) { SafeAdd(eraseStatement, eraseStatement.Expressions, expr); }
			}

#line  3249 "VBNET.ATG" 
			statement = eraseStatement; 
		} else if (la.kind == 209) {
			lexer.NextToken();

#line  3251 "VBNET.ATG" 
			statement = new StopStatement(); 
		} else if (
#line  3253 "VBNET.ATG" 
la.kind == Tokens.If) {
			Expect(137);

#line  3254 "VBNET.ATG" 
			Location ifStartLocation = t.Location; 
			Expr(
#line  3254 "VBNET.ATG" 
out expr);
			if (la.kind == 217) {
				lexer.NextToken();
			}
			if (la.kind == 1 || la.kind == 21) {
				EndOfStmt();
				Block(
#line  3257 "VBNET.ATG" 
out embeddedStatement);

#line  3259 "VBNET.ATG" 
				IfElseStatement ifStatement = new IfElseStatement(expr, embeddedStatement);
				ifStatement.StartLocation = ifStartLocation;
				Location elseIfStart;
				
				while (la.kind == 114 || 
#line  3265 "VBNET.ATG" 
IsElseIf()) {
					if (
#line  3265 "VBNET.ATG" 
IsElseIf()) {
						Expect(113);

#line  3265 "VBNET.ATG" 
						elseIfStart = t.Location; 
						Expect(137);
					} else {
						lexer.NextToken();

#line  3266 "VBNET.ATG" 
						elseIfStart = t.Location; 
					}

#line  3268 "VBNET.ATG" 
					Expression condition = null; Statement block = null; 
					Expr(
#line  3269 "VBNET.ATG" 
out condition);
					if (la.kind == 217) {
						lexer.NextToken();
					}
					EndOfStmt();
					Block(
#line  3270 "VBNET.ATG" 
out block);

#line  3272 "VBNET.ATG" 
					ElseIfSection elseIfSection = new ElseIfSection(condition, block);
					elseIfSection.StartLocation = elseIfStart;
					elseIfSection.EndLocation = t.Location;
					elseIfSection.Parent = ifStatement;
					ifStatement.ElseIfSections.Add(elseIfSection);
					
				}
				if (la.kind == 113) {
					lexer.NextToken();
					if (la.kind == 1 || la.kind == 21) {
						EndOfStmt();
					}
					Block(
#line  3281 "VBNET.ATG" 
out embeddedStatement);

#line  3283 "VBNET.ATG" 
					ifStatement.FalseStatement.Add(embeddedStatement);
					
				}
				Expect(115);
				Expect(137);

#line  3287 "VBNET.ATG" 
				ifStatement.EndLocation = t.Location;
				statement = ifStatement;
				
			} else if (StartOf(46)) {

#line  3292 "VBNET.ATG" 
				IfElseStatement ifStatement = new IfElseStatement(expr);
				ifStatement.StartLocation = ifStartLocation;
				
				SingleLineStatementList(
#line  3295 "VBNET.ATG" 
ifStatement.TrueStatement);
				if (la.kind == 113) {
					lexer.NextToken();
					if (StartOf(46)) {
						SingleLineStatementList(
#line  3298 "VBNET.ATG" 
ifStatement.FalseStatement);
					}
				}

#line  3300 "VBNET.ATG" 
				ifStatement.EndLocation = t.Location; statement = ifStatement; 
			} else SynErr(305);
		} else if (la.kind == 200) {
			lexer.NextToken();
			if (la.kind == 76) {
				lexer.NextToken();
			}
			Expr(
#line  3303 "VBNET.ATG" 
out expr);
			EndOfStmt();

#line  3304 "VBNET.ATG" 
			List<SwitchSection> selectSections = new List<SwitchSection>();
			Statement block = null;
			
			while (la.kind == 76) {

#line  3308 "VBNET.ATG" 
				List<CaseLabel> caseClauses = null; Location caseLocation = la.Location; 
				lexer.NextToken();
				CaseClauses(
#line  3309 "VBNET.ATG" 
out caseClauses);
				if (
#line  3309 "VBNET.ATG" 
IsNotStatementSeparator()) {
					lexer.NextToken();
				}
				EndOfStmt();

#line  3311 "VBNET.ATG" 
				SwitchSection selectSection = new SwitchSection(caseClauses);
				selectSection.StartLocation = caseLocation;
				
				Block(
#line  3314 "VBNET.ATG" 
out block);

#line  3316 "VBNET.ATG" 
				selectSection.Children = block.Children;
				selectSection.EndLocation = t.EndLocation;
				selectSections.Add(selectSection);
				
			}

#line  3322 "VBNET.ATG" 
			statement = new SwitchStatement(expr, selectSections);
			
			Expect(115);
			Expect(200);
		} else if (la.kind == 174) {

#line  3325 "VBNET.ATG" 
			OnErrorStatement onErrorStatement = null; 
			OnErrorStatement(
#line  3326 "VBNET.ATG" 
out onErrorStatement);

#line  3326 "VBNET.ATG" 
			statement = onErrorStatement; 
		} else if (la.kind == 134) {

#line  3327 "VBNET.ATG" 
			GotoStatement goToStatement = null; 
			GotoStatement(
#line  3328 "VBNET.ATG" 
out goToStatement);

#line  3328 "VBNET.ATG" 
			statement = goToStatement; 
		} else if (la.kind == 197) {

#line  3329 "VBNET.ATG" 
			ResumeStatement resumeStatement = null; 
			ResumeStatement(
#line  3330 "VBNET.ATG" 
out resumeStatement);

#line  3330 "VBNET.ATG" 
			statement = resumeStatement; 
		} else if (StartOf(45)) {

#line  3333 "VBNET.ATG" 
			Expression val = null;
			AssignmentOperatorType op;
			Location startLoc = la.Location;
			
			bool mustBeAssignment = la.kind == Tokens.Plus  || la.kind == Tokens.Minus ||
			                        la.kind == Tokens.Not   || la.kind == Tokens.Times;
			
			SimpleExpr(
#line  3340 "VBNET.ATG" 
out expr);
			if (StartOf(47)) {
				AssignmentOperator(
#line  3342 "VBNET.ATG" 
out op);
				Expr(
#line  3342 "VBNET.ATG" 
out val);

#line  3344 "VBNET.ATG" 
				expr = new AssignmentExpression(expr, op, val);
				expr.StartLocation = startLoc;
				expr.EndLocation = t.EndLocation;
				
			} else if (StartOf(48)) {

#line  3348 "VBNET.ATG" 
				if (mustBeAssignment) Error("error in assignment."); 
			} else SynErr(306);

#line  3351 "VBNET.ATG" 
			// a field reference expression that stands alone is a
			// invocation expression without parantheses and arguments
			if(expr is MemberReferenceExpression || expr is IdentifierExpression) {
				Location endLocation = expr.EndLocation;
				expr = new InvocationExpression(expr);
				expr.StartLocation = startLoc;
				expr.EndLocation = endLocation;
			}
			statement = new ExpressionStatement(expr);
			
		} else if (la.kind == 75) {
			lexer.NextToken();
			SimpleExpr(
#line  3361 "VBNET.ATG" 
out expr);

#line  3361 "VBNET.ATG" 
			statement = new ExpressionStatement(expr); 
		} else if (la.kind == 229) {
			lexer.NextToken();

#line  3363 "VBNET.ATG" 
			Statement block;  
			if (
#line  3364 "VBNET.ATG" 
Peek(1).kind == Tokens.As) {

#line  3365 "VBNET.ATG" 
				LocalVariableDeclaration resourceAquisition = new LocalVariableDeclaration(Modifiers.None); 
				VariableDeclarator(
#line  3366 "VBNET.ATG" 
resourceAquisition.Variables);
				while (la.kind == 22) {
					lexer.NextToken();
					VariableDeclarator(
#line  3368 "VBNET.ATG" 
resourceAquisition.Variables);
				}
				Block(
#line  3370 "VBNET.ATG" 
out block);

#line  3372 "VBNET.ATG" 
				statement = new UsingStatement(resourceAquisition, block);
				
			} else if (StartOf(25)) {
				Expr(
#line  3374 "VBNET.ATG" 
out expr);
				Block(
#line  3375 "VBNET.ATG" 
out block);

#line  3376 "VBNET.ATG" 
				statement = new UsingStatement(new ExpressionStatement(expr), block); 
			} else SynErr(307);
			Expect(115);
			Expect(229);
		} else if (StartOf(49)) {
			LocalDeclarationStatement(
#line  3379 "VBNET.ATG" 
out statement);
		} else SynErr(308);

#line  3382 "VBNET.ATG" 
		if (statement != null) {
		statement.StartLocation = startLocation;
		statement.EndLocation = t.EndLocation;
		}
		
	}

	void FromOrAggregateQueryOperator(
#line  2316 "VBNET.ATG" 
List<QueryExpressionClause> middleClauses) {

#line  2318 "VBNET.ATG" 
		QueryExpressionFromClause fromClause = null;
		QueryExpressionAggregateClause aggregateClause = null;
		
		if (la.kind == 128) {
			FromQueryOperator(
#line  2321 "VBNET.ATG" 
out fromClause);

#line  2322 "VBNET.ATG" 
			middleClauses.Add(fromClause); 
		} else if (la.kind == 58) {
			AggregateQueryOperator(
#line  2323 "VBNET.ATG" 
out aggregateClause);

#line  2324 "VBNET.ATG" 
			middleClauses.Add(aggregateClause); 
		} else SynErr(309);
	}

	void QueryOperator(
#line  2327 "VBNET.ATG" 
List<QueryExpressionClause> middleClauses) {

#line  2329 "VBNET.ATG" 
		QueryExpressionJoinVBClause joinClause = null;
		QueryExpressionGroupVBClause groupByClause = null;
		QueryExpressionPartitionVBClause partitionClause = null;
		QueryExpressionGroupJoinVBClause groupJoinClause = null;
		QueryExpressionFromClause fromClause = null;
		QueryExpressionAggregateClause aggregateClause = null;
		
		if (la.kind == 128) {
			FromQueryOperator(
#line  2336 "VBNET.ATG" 
out fromClause);

#line  2337 "VBNET.ATG" 
			middleClauses.Add(fromClause); 
		} else if (la.kind == 58) {
			AggregateQueryOperator(
#line  2338 "VBNET.ATG" 
out aggregateClause);

#line  2339 "VBNET.ATG" 
			middleClauses.Add(aggregateClause); 
		} else if (la.kind == 200) {
			SelectQueryOperator(
#line  2340 "VBNET.ATG" 
middleClauses);
		} else if (la.kind == 109) {
			DistinctQueryOperator(
#line  2341 "VBNET.ATG" 
middleClauses);
		} else if (la.kind == 233) {
			WhereQueryOperator(
#line  2342 "VBNET.ATG" 
middleClauses);
		} else if (la.kind == 179) {
			OrderByQueryOperator(
#line  2343 "VBNET.ATG" 
middleClauses);
		} else if (la.kind == 206 || la.kind == 215) {
			PartitionQueryOperator(
#line  2344 "VBNET.ATG" 
out partitionClause);

#line  2345 "VBNET.ATG" 
			middleClauses.Add(partitionClause); 
		} else if (la.kind == 151) {
			LetQueryOperator(
#line  2346 "VBNET.ATG" 
middleClauses);
		} else if (la.kind == 149) {
			JoinQueryOperator(
#line  2347 "VBNET.ATG" 
out joinClause);

#line  2348 "VBNET.ATG" 
			middleClauses.Add(joinClause); 
		} else if (
#line  2349 "VBNET.ATG" 
la.kind == Tokens.Group && Peek(1).kind == Tokens.Join) {
			GroupJoinQueryOperator(
#line  2349 "VBNET.ATG" 
out groupJoinClause);

#line  2350 "VBNET.ATG" 
			middleClauses.Add(groupJoinClause); 
		} else if (la.kind == 135) {
			GroupByQueryOperator(
#line  2351 "VBNET.ATG" 
out groupByClause);

#line  2352 "VBNET.ATG" 
			middleClauses.Add(groupByClause); 
		} else SynErr(310);
	}

	void FromQueryOperator(
#line  2427 "VBNET.ATG" 
out QueryExpressionFromClause fromClause) {

#line  2429 "VBNET.ATG" 
		fromClause = new QueryExpressionFromClause();
		fromClause.StartLocation = la.Location;
		
		Expect(128);
		CollectionRangeVariableDeclarationList(
#line  2432 "VBNET.ATG" 
fromClause.Sources);

#line  2434 "VBNET.ATG" 
		fromClause.EndLocation = t.EndLocation;
		
	}

	void AggregateQueryOperator(
#line  2496 "VBNET.ATG" 
out QueryExpressionAggregateClause aggregateClause) {

#line  2498 "VBNET.ATG" 
		aggregateClause = new QueryExpressionAggregateClause();
		aggregateClause.IntoVariables = new List<ExpressionRangeVariable>();
		aggregateClause.StartLocation = la.Location;
		CollectionRangeVariable source;
		
		Expect(58);
		CollectionRangeVariableDeclaration(
#line  2503 "VBNET.ATG" 
out source);

#line  2505 "VBNET.ATG" 
		aggregateClause.Source = source;
		
		while (StartOf(32)) {
			QueryOperator(
#line  2508 "VBNET.ATG" 
aggregateClause.MiddleClauses);
		}
		Expect(145);
		ExpressionRangeVariableDeclarationList(
#line  2510 "VBNET.ATG" 
aggregateClause.IntoVariables);

#line  2512 "VBNET.ATG" 
		aggregateClause.EndLocation = t.EndLocation;
		
	}

	void SelectQueryOperator(
#line  2438 "VBNET.ATG" 
List<QueryExpressionClause> middleClauses) {

#line  2440 "VBNET.ATG" 
		QueryExpressionSelectVBClause selectClause = new QueryExpressionSelectVBClause();
		selectClause.StartLocation = la.Location;
		
		Expect(200);
		ExpressionRangeVariableDeclarationList(
#line  2443 "VBNET.ATG" 
selectClause.Variables);

#line  2445 "VBNET.ATG" 
		selectClause.EndLocation = t.Location;
		middleClauses.Add(selectClause);
		
	}

	void DistinctQueryOperator(
#line  2450 "VBNET.ATG" 
List<QueryExpressionClause> middleClauses) {

#line  2452 "VBNET.ATG" 
		QueryExpressionDistinctClause distinctClause = new QueryExpressionDistinctClause();
		distinctClause.StartLocation = la.Location;
		
		Expect(109);

#line  2457 "VBNET.ATG" 
		distinctClause.EndLocation = t.EndLocation;
		middleClauses.Add(distinctClause);
		
	}

	void WhereQueryOperator(
#line  2462 "VBNET.ATG" 
List<QueryExpressionClause> middleClauses) {

#line  2464 "VBNET.ATG" 
		QueryExpressionWhereClause whereClause = new QueryExpressionWhereClause();
		whereClause.StartLocation = la.Location;
		Expression operand = null;
		
		Expect(233);
		Expr(
#line  2468 "VBNET.ATG" 
out operand);

#line  2470 "VBNET.ATG" 
		whereClause.Condition = operand;
		whereClause.EndLocation = t.EndLocation;
		
		middleClauses.Add(whereClause);
		
	}

	void OrderByQueryOperator(
#line  2355 "VBNET.ATG" 
List<QueryExpressionClause> middleClauses) {

#line  2357 "VBNET.ATG" 
		QueryExpressionOrderClause orderClause = new QueryExpressionOrderClause();
		orderClause.StartLocation = la.Location;
		List<QueryExpressionOrdering> orderings = null;
		
		Expect(179);
		Expect(72);
		OrderExpressionList(
#line  2361 "VBNET.ATG" 
out orderings);

#line  2363 "VBNET.ATG" 
		orderClause.Orderings = orderings;
		orderClause.EndLocation = t.EndLocation;
		middleClauses.Add(orderClause);
		
	}

	void PartitionQueryOperator(
#line  2477 "VBNET.ATG" 
out QueryExpressionPartitionVBClause partitionClause) {

#line  2479 "VBNET.ATG" 
		partitionClause = new QueryExpressionPartitionVBClause();
		partitionClause.StartLocation = la.Location;
		Expression expr = null;
		
		if (la.kind == 215) {
			lexer.NextToken();

#line  2484 "VBNET.ATG" 
			partitionClause.PartitionType = QueryExpressionPartitionType.Take; 
			if (la.kind == 234) {
				lexer.NextToken();

#line  2485 "VBNET.ATG" 
				partitionClause.PartitionType = QueryExpressionPartitionType.TakeWhile; 
			}
		} else if (la.kind == 206) {
			lexer.NextToken();

#line  2486 "VBNET.ATG" 
			partitionClause.PartitionType = QueryExpressionPartitionType.Skip; 
			if (la.kind == 234) {
				lexer.NextToken();

#line  2487 "VBNET.ATG" 
				partitionClause.PartitionType = QueryExpressionPartitionType.SkipWhile; 
			}
		} else SynErr(311);
		Expr(
#line  2489 "VBNET.ATG" 
out expr);

#line  2491 "VBNET.ATG" 
		partitionClause.Expression = expr;
		partitionClause.EndLocation = t.EndLocation;
		
	}

	void LetQueryOperator(
#line  2516 "VBNET.ATG" 
List<QueryExpressionClause> middleClauses) {

#line  2518 "VBNET.ATG" 
		QueryExpressionLetVBClause letClause = new QueryExpressionLetVBClause();
		letClause.StartLocation = la.Location;
		
		Expect(151);
		ExpressionRangeVariableDeclarationList(
#line  2521 "VBNET.ATG" 
letClause.Variables);

#line  2523 "VBNET.ATG" 
		letClause.EndLocation = t.EndLocation;
		middleClauses.Add(letClause);
		
	}

	void JoinQueryOperator(
#line  2560 "VBNET.ATG" 
out QueryExpressionJoinVBClause joinClause) {

#line  2562 "VBNET.ATG" 
		joinClause = new QueryExpressionJoinVBClause();
		joinClause.StartLocation = la.Location;
		CollectionRangeVariable joinVariable = null;
		QueryExpressionJoinVBClause subJoin = null;
		QueryExpressionJoinConditionVB condition = null;
		
		
		Expect(149);
		CollectionRangeVariableDeclaration(
#line  2569 "VBNET.ATG" 
out joinVariable);

#line  2570 "VBNET.ATG" 
		joinClause.JoinVariable = joinVariable; 
		if (la.kind == 149) {
			JoinQueryOperator(
#line  2572 "VBNET.ATG" 
out subJoin);

#line  2573 "VBNET.ATG" 
			joinClause.SubJoin = subJoin; 
		}
		Expect(174);
		JoinCondition(
#line  2576 "VBNET.ATG" 
out condition);

#line  2577 "VBNET.ATG" 
		SafeAdd(joinClause, joinClause.Conditions, condition); 
		while (la.kind == 60) {
			lexer.NextToken();
			JoinCondition(
#line  2579 "VBNET.ATG" 
out condition);

#line  2580 "VBNET.ATG" 
			SafeAdd(joinClause, joinClause.Conditions, condition); 
		}

#line  2583 "VBNET.ATG" 
		joinClause.EndLocation = t.EndLocation;
		
	}

	void GroupJoinQueryOperator(
#line  2413 "VBNET.ATG" 
out QueryExpressionGroupJoinVBClause groupJoinClause) {

#line  2415 "VBNET.ATG" 
		groupJoinClause = new QueryExpressionGroupJoinVBClause();
		groupJoinClause.StartLocation = la.Location;
		QueryExpressionJoinVBClause joinClause = null;
		
		Expect(135);
		JoinQueryOperator(
#line  2419 "VBNET.ATG" 
out joinClause);
		Expect(145);
		ExpressionRangeVariableDeclarationList(
#line  2420 "VBNET.ATG" 
groupJoinClause.IntoVariables);

#line  2422 "VBNET.ATG" 
		groupJoinClause.JoinClause = joinClause;
		groupJoinClause.EndLocation = t.EndLocation;
		
	}

	void GroupByQueryOperator(
#line  2400 "VBNET.ATG" 
out QueryExpressionGroupVBClause groupByClause) {

#line  2402 "VBNET.ATG" 
		groupByClause = new QueryExpressionGroupVBClause();
		groupByClause.StartLocation = la.Location;
		
		Expect(135);
		ExpressionRangeVariableDeclarationList(
#line  2405 "VBNET.ATG" 
groupByClause.GroupVariables);
		Expect(72);
		ExpressionRangeVariableDeclarationList(
#line  2406 "VBNET.ATG" 
groupByClause.ByVariables);
		Expect(145);
		ExpressionRangeVariableDeclarationList(
#line  2407 "VBNET.ATG" 
groupByClause.IntoVariables);

#line  2409 "VBNET.ATG" 
		groupByClause.EndLocation = t.EndLocation;
		
	}

	void OrderExpressionList(
#line  2369 "VBNET.ATG" 
out List<QueryExpressionOrdering> orderings) {

#line  2371 "VBNET.ATG" 
		orderings = new List<QueryExpressionOrdering>();
		QueryExpressionOrdering ordering = null;
		
		OrderExpression(
#line  2374 "VBNET.ATG" 
out ordering);

#line  2375 "VBNET.ATG" 
		orderings.Add(ordering); 
		while (la.kind == 22) {
			lexer.NextToken();
			OrderExpression(
#line  2377 "VBNET.ATG" 
out ordering);

#line  2378 "VBNET.ATG" 
			orderings.Add(ordering); 
		}
	}

	void OrderExpression(
#line  2382 "VBNET.ATG" 
out QueryExpressionOrdering ordering) {

#line  2384 "VBNET.ATG" 
		ordering = new QueryExpressionOrdering();
		ordering.StartLocation = la.Location;
		ordering.Direction = QueryExpressionOrderingDirection.None;
		Expression orderExpr = null;
		
		Expr(
#line  2389 "VBNET.ATG" 
out orderExpr);

#line  2391 "VBNET.ATG" 
		ordering.Criteria = orderExpr;
		
		if (la.kind == 64 || la.kind == 106) {
			if (la.kind == 64) {
				lexer.NextToken();

#line  2394 "VBNET.ATG" 
				ordering.Direction = QueryExpressionOrderingDirection.Ascending; 
			} else {
				lexer.NextToken();

#line  2395 "VBNET.ATG" 
				ordering.Direction = QueryExpressionOrderingDirection.Descending; 
			}
		}

#line  2397 "VBNET.ATG" 
		ordering.EndLocation = t.EndLocation; 
	}

	void ExpressionRangeVariableDeclarationList(
#line  2528 "VBNET.ATG" 
List<ExpressionRangeVariable> variables) {

#line  2530 "VBNET.ATG" 
		ExpressionRangeVariable variable = null;
		
		ExpressionRangeVariableDeclaration(
#line  2532 "VBNET.ATG" 
out variable);

#line  2533 "VBNET.ATG" 
		variables.Add(variable); 
		while (la.kind == 22) {
			lexer.NextToken();
			ExpressionRangeVariableDeclaration(
#line  2534 "VBNET.ATG" 
out variable);

#line  2534 "VBNET.ATG" 
			variables.Add(variable); 
		}
	}

	void CollectionRangeVariableDeclarationList(
#line  2587 "VBNET.ATG" 
List<CollectionRangeVariable> rangeVariables) {

#line  2588 "VBNET.ATG" 
		CollectionRangeVariable variableDeclaration; 
		CollectionRangeVariableDeclaration(
#line  2590 "VBNET.ATG" 
out variableDeclaration);

#line  2591 "VBNET.ATG" 
		rangeVariables.Add(variableDeclaration); 
		while (la.kind == 22) {
			lexer.NextToken();
			CollectionRangeVariableDeclaration(
#line  2592 "VBNET.ATG" 
out variableDeclaration);

#line  2592 "VBNET.ATG" 
			rangeVariables.Add(variableDeclaration); 
		}
	}

	void CollectionRangeVariableDeclaration(
#line  2595 "VBNET.ATG" 
out CollectionRangeVariable rangeVariable) {

#line  2597 "VBNET.ATG" 
		rangeVariable = new CollectionRangeVariable();
		rangeVariable.StartLocation = la.Location;
		TypeReference typeName = null;
		Expression inExpr = null;
		
		Identifier();

#line  2602 "VBNET.ATG" 
		rangeVariable.Identifier = t.val; 
		if (la.kind == 63) {
			lexer.NextToken();
			TypeName(
#line  2603 "VBNET.ATG" 
out typeName);

#line  2603 "VBNET.ATG" 
			rangeVariable.Type = typeName; 
		}
		Expect(140);
		Expr(
#line  2604 "VBNET.ATG" 
out inExpr);

#line  2606 "VBNET.ATG" 
		rangeVariable.Expression = inExpr;
		rangeVariable.EndLocation = t.EndLocation;
		
	}

	void ExpressionRangeVariableDeclaration(
#line  2537 "VBNET.ATG" 
out ExpressionRangeVariable variable) {

#line  2539 "VBNET.ATG" 
		variable = new ExpressionRangeVariable();
		variable.StartLocation = la.Location;
		Expression rhs = null;
		TypeReference typeName = null;
		
		if (
#line  2545 "VBNET.ATG" 
IsIdentifiedExpressionRange()) {
			Identifier();

#line  2546 "VBNET.ATG" 
			variable.Identifier = t.val; 
			if (la.kind == 63) {
				lexer.NextToken();
				TypeName(
#line  2548 "VBNET.ATG" 
out typeName);

#line  2549 "VBNET.ATG" 
				variable.Type = typeName; 
			}
			Expect(20);
		}
		Expr(
#line  2553 "VBNET.ATG" 
out rhs);

#line  2555 "VBNET.ATG" 
		variable.Expression = rhs;
		variable.EndLocation = t.EndLocation;
		
	}

	void JoinCondition(
#line  2611 "VBNET.ATG" 
out QueryExpressionJoinConditionVB condition) {

#line  2613 "VBNET.ATG" 
		condition = new QueryExpressionJoinConditionVB();
		condition.StartLocation = la.Location;
		
		Expression lhs = null;
		Expression rhs = null;
		
		Expr(
#line  2619 "VBNET.ATG" 
out lhs);
		Expect(118);
		Expr(
#line  2619 "VBNET.ATG" 
out rhs);

#line  2621 "VBNET.ATG" 
		condition.LeftSide = lhs;
		condition.RightSide = rhs;
		condition.EndLocation = t.EndLocation;
		
	}

	void Argument(
#line  2685 "VBNET.ATG" 
out Expression argumentexpr) {

#line  2687 "VBNET.ATG" 
		Expression expr;
		argumentexpr = null;
		string name;
		Location startLocation = la.Location;
		
		if (
#line  2692 "VBNET.ATG" 
IsNamedAssign()) {
			Identifier();

#line  2692 "VBNET.ATG" 
			name = t.val;  
			Expect(55);
			Expr(
#line  2692 "VBNET.ATG" 
out expr);

#line  2694 "VBNET.ATG" 
			argumentexpr = new NamedArgumentExpression(name, expr) { StartLocation = startLocation, EndLocation = t.EndLocation };
			
		} else if (StartOf(25)) {
			Expr(
#line  2697 "VBNET.ATG" 
out argumentexpr);
		} else SynErr(312);
	}

	void QualIdentAndTypeArguments(
#line  2765 "VBNET.ATG" 
out TypeReference typeref, bool canBeUnbound) {

#line  2766 "VBNET.ATG" 
		string name; typeref = null; 
		Qualident(
#line  2768 "VBNET.ATG" 
out name);

#line  2769 "VBNET.ATG" 
		typeref = new TypeReference(name); 
		if (
#line  2770 "VBNET.ATG" 
la.kind == Tokens.OpenParenthesis && Peek(1).kind == Tokens.Of) {
			lexer.NextToken();
			Expect(172);
			if (
#line  2772 "VBNET.ATG" 
canBeUnbound && (la.kind == Tokens.CloseParenthesis || la.kind == Tokens.Comma)) {

#line  2773 "VBNET.ATG" 
				typeref.GenericTypes.Add(NullTypeReference.Instance); 
				while (la.kind == 22) {
					lexer.NextToken();

#line  2774 "VBNET.ATG" 
					typeref.GenericTypes.Add(NullTypeReference.Instance); 
				}
			} else if (StartOf(8)) {
				TypeArgumentList(
#line  2775 "VBNET.ATG" 
typeref.GenericTypes);
			} else SynErr(313);
			Expect(38);
		}
	}

	void RankList(
#line  2812 "VBNET.ATG" 
out int i) {

#line  2813 "VBNET.ATG" 
		i = 0; 
		while (la.kind == 22) {
			lexer.NextToken();

#line  2814 "VBNET.ATG" 
			++i; 
		}
	}

	void Attribute(
#line  2853 "VBNET.ATG" 
out ASTAttribute attribute) {

#line  2855 "VBNET.ATG" 
		string name;
		List<Expression> positional = new List<Expression>();
		List<NamedArgumentExpression> named = new List<NamedArgumentExpression>();
		Location startLocation = la.Location;
		
		if (la.kind == 132) {
			lexer.NextToken();
			Expect(26);
		}
		Qualident(
#line  2861 "VBNET.ATG" 
out name);
		if (la.kind == 37) {
			AttributeArguments(
#line  2862 "VBNET.ATG" 
positional, named);
		}

#line  2864 "VBNET.ATG" 
		attribute  = new ASTAttribute(name, positional, named) { StartLocation = startLocation, EndLocation = t.EndLocation };
		
	}

	void AttributeArguments(
#line  2869 "VBNET.ATG" 
List<Expression> positional, List<NamedArgumentExpression> named) {

#line  2871 "VBNET.ATG" 
		bool nameFound = false;
		string name = "";
		Expression expr;
		
		Expect(37);
		if (
#line  2877 "VBNET.ATG" 
IsNotClosingParenthesis()) {

#line  2878 "VBNET.ATG" 
			Location startLocation = la.Location; 
			if (
#line  2880 "VBNET.ATG" 
IsNamedAssign()) {

#line  2880 "VBNET.ATG" 
				nameFound = true; 
				IdentifierOrKeyword(
#line  2881 "VBNET.ATG" 
out name);
				if (la.kind == 55) {
					lexer.NextToken();
				} else if (la.kind == 20) {
					lexer.NextToken();
				} else SynErr(314);
			}
			Expr(
#line  2883 "VBNET.ATG" 
out expr);

#line  2885 "VBNET.ATG" 
			if (expr != null) {
			if (string.IsNullOrEmpty(name)) { positional.Add(expr); }
			else { named.Add(new NamedArgumentExpression(name, expr) { StartLocation = startLocation, EndLocation = t.EndLocation }); name = ""; }
			}
			
			while (la.kind == 22) {
				lexer.NextToken();
				if (
#line  2893 "VBNET.ATG" 
IsNamedAssign()) {

#line  2893 "VBNET.ATG" 
					nameFound = true; 
					IdentifierOrKeyword(
#line  2894 "VBNET.ATG" 
out name);
					if (la.kind == 55) {
						lexer.NextToken();
					} else if (la.kind == 20) {
						lexer.NextToken();
					} else SynErr(315);
				} else if (StartOf(25)) {

#line  2896 "VBNET.ATG" 
					if (nameFound) Error("no positional argument after named argument"); 
				} else SynErr(316);
				Expr(
#line  2897 "VBNET.ATG" 
out expr);

#line  2897 "VBNET.ATG" 
				if (expr != null) { if(name == "") positional.Add(expr);
				else { named.Add(new NamedArgumentExpression(name, expr) { StartLocation = startLocation, EndLocation = t.EndLocation }); name = ""; }
				}
				
			}
		}
		Expect(38);
	}

	void ParameterModifier(
#line  3715 "VBNET.ATG" 
ParamModifierList m) {
		if (la.kind == 74) {
			lexer.NextToken();

#line  3716 "VBNET.ATG" 
			m.Add(ParameterModifiers.In); 
		} else if (la.kind == 71) {
			lexer.NextToken();

#line  3717 "VBNET.ATG" 
			m.Add(ParameterModifiers.Ref); 
		} else if (la.kind == 177) {
			lexer.NextToken();

#line  3718 "VBNET.ATG" 
			m.Add(ParameterModifiers.Optional); 
		} else if (la.kind == 185) {
			lexer.NextToken();

#line  3719 "VBNET.ATG" 
			m.Add(ParameterModifiers.Params); 
		} else SynErr(317);
	}

	void Statement() {

#line  3026 "VBNET.ATG" 
		Statement stmt = null;
		Location startPos = la.Location;
		string label = String.Empty;
		
		
		if (la.kind == 1 || la.kind == 21) {
		} else if (
#line  3032 "VBNET.ATG" 
IsLabel()) {
			LabelName(
#line  3032 "VBNET.ATG" 
out label);

#line  3034 "VBNET.ATG" 
			AddChild(new LabelStatement(t.val));
			
			Expect(21);
			Statement();
		} else if (StartOf(50)) {
			EmbeddedStatement(
#line  3037 "VBNET.ATG" 
out stmt);

#line  3037 "VBNET.ATG" 
			AddChild(stmt); 
		} else SynErr(318);

#line  3040 "VBNET.ATG" 
		if (stmt != null) {
		stmt.StartLocation = startPos;
		stmt.EndLocation = t.Location;
		}
		
	}

	void LabelName(
#line  3481 "VBNET.ATG" 
out string name) {

#line  3483 "VBNET.ATG" 
		name = String.Empty;
		
		if (StartOf(4)) {
			Identifier();

#line  3485 "VBNET.ATG" 
			name = t.val; 
		} else if (la.kind == 5) {
			lexer.NextToken();

#line  3486 "VBNET.ATG" 
			name = t.val; 
		} else SynErr(319);
	}

	void LocalDeclarationStatement(
#line  3048 "VBNET.ATG" 
out Statement statement) {

#line  3050 "VBNET.ATG" 
		ModifierList m = new ModifierList();
		LocalVariableDeclaration localVariableDeclaration;
		bool dimfound = false;
		
		while (la.kind == 90 || la.kind == 107 || la.kind == 207) {
			if (la.kind == 90) {
				lexer.NextToken();

#line  3056 "VBNET.ATG" 
				m.Add(Modifiers.Const, t.Location); 
			} else if (la.kind == 207) {
				lexer.NextToken();

#line  3057 "VBNET.ATG" 
				m.Add(Modifiers.Static, t.Location); 
			} else {
				lexer.NextToken();

#line  3058 "VBNET.ATG" 
				dimfound = true; 
			}
		}

#line  3061 "VBNET.ATG" 
		if(dimfound && (m.Modifier & Modifiers.Const) != 0) {
		Error("Dim is not allowed on constants.");
		}
		
		if(m.isNone && dimfound == false) {
			Error("Const, Dim or Static expected");
		}
		
		localVariableDeclaration = new LocalVariableDeclaration(m.Modifier);
		localVariableDeclaration.StartLocation = t.Location;
		
		VariableDeclarator(
#line  3072 "VBNET.ATG" 
localVariableDeclaration.Variables);
		while (la.kind == 22) {
			lexer.NextToken();
			VariableDeclarator(
#line  3073 "VBNET.ATG" 
localVariableDeclaration.Variables);
		}

#line  3075 "VBNET.ATG" 
		statement = localVariableDeclaration;
		
	}

	void TryStatement(
#line  3599 "VBNET.ATG" 
out Statement tryStatement) {

#line  3601 "VBNET.ATG" 
		Statement blockStmt = null, finallyStmt = null;List<CatchClause> catchClauses = null;
		
		Expect(221);
		EndOfStmt();
		Block(
#line  3604 "VBNET.ATG" 
out blockStmt);
		if (la.kind == 77 || la.kind == 115 || la.kind == 125) {
			CatchClauses(
#line  3605 "VBNET.ATG" 
out catchClauses);
		}
		if (la.kind == 125) {
			lexer.NextToken();
			EndOfStmt();
			Block(
#line  3606 "VBNET.ATG" 
out finallyStmt);
		}
		Expect(115);
		Expect(221);

#line  3609 "VBNET.ATG" 
		tryStatement = new TryCatchStatement(blockStmt, catchClauses, finallyStmt);
		
	}

	void WithStatement(
#line  3579 "VBNET.ATG" 
out Statement withStatement) {

#line  3581 "VBNET.ATG" 
		Statement blockStmt = null;
		Expression expr = null;
		
		Expect(236);

#line  3584 "VBNET.ATG" 
		Location start = t.Location; 
		Expr(
#line  3585 "VBNET.ATG" 
out expr);
		EndOfStmt();

#line  3587 "VBNET.ATG" 
		withStatement = new WithStatement(expr);
		withStatement.StartLocation = start;
		
		Block(
#line  3590 "VBNET.ATG" 
out blockStmt);

#line  3592 "VBNET.ATG" 
		((WithStatement)withStatement).Body = (BlockStatement)blockStmt;
		
		Expect(115);
		Expect(236);

#line  3595 "VBNET.ATG" 
		withStatement.EndLocation = t.Location; 
	}

	void WhileOrUntil(
#line  3572 "VBNET.ATG" 
out ConditionType conditionType) {

#line  3573 "VBNET.ATG" 
		conditionType = ConditionType.None; 
		if (la.kind == 234) {
			lexer.NextToken();

#line  3574 "VBNET.ATG" 
			conditionType = ConditionType.While; 
		} else if (la.kind == 227) {
			lexer.NextToken();

#line  3575 "VBNET.ATG" 
			conditionType = ConditionType.Until; 
		} else SynErr(320);
	}

	void LoopControlVariable(
#line  3403 "VBNET.ATG" 
out TypeReference type, out string name) {

#line  3404 "VBNET.ATG" 
		ArrayList arrayModifiers = null;
		type = null;
		
		Qualident(
#line  3408 "VBNET.ATG" 
out name);
		if (
#line  3409 "VBNET.ATG" 
IsDims()) {
			ArrayTypeModifiers(
#line  3409 "VBNET.ATG" 
out arrayModifiers);
		}
		if (la.kind == 63) {
			lexer.NextToken();
			TypeName(
#line  3410 "VBNET.ATG" 
out type);

#line  3410 "VBNET.ATG" 
			if (name.IndexOf('.') > 0) { Error("No type def for 'for each' member indexer allowed."); } 
		}

#line  3412 "VBNET.ATG" 
		if (type != null) {
		if(type.RankSpecifier != null && arrayModifiers != null) {
			Error("array rank only allowed one time");
		} else if (arrayModifiers != null) {
			type.RankSpecifier = (int[])arrayModifiers.ToArray(typeof(int));
		}
		}
		
	}

	void ReDimClause(
#line  3490 "VBNET.ATG" 
out Expression expr) {
		SimpleNonInvocationExpression(
#line  3492 "VBNET.ATG" 
out expr);
		ReDimClauseInternal(
#line  3493 "VBNET.ATG" 
ref expr);
	}

	void SingleLineStatementList(
#line  3389 "VBNET.ATG" 
List<Statement> list) {

#line  3390 "VBNET.ATG" 
		Statement embeddedStatement = null; 
		if (la.kind == 115) {
			lexer.NextToken();

#line  3392 "VBNET.ATG" 
			embeddedStatement = new EndStatement() { StartLocation = t.Location, EndLocation = t.EndLocation }; 
		} else if (StartOf(50)) {
			EmbeddedStatement(
#line  3393 "VBNET.ATG" 
out embeddedStatement);
		} else SynErr(321);

#line  3394 "VBNET.ATG" 
		if (embeddedStatement != null) list.Add(embeddedStatement); 
		while (la.kind == 21) {
			lexer.NextToken();
			while (la.kind == 21) {
				lexer.NextToken();
			}
			if (la.kind == 115) {
				lexer.NextToken();

#line  3396 "VBNET.ATG" 
				embeddedStatement = new EndStatement() { StartLocation = t.Location, EndLocation = t.EndLocation }; 
			} else if (StartOf(50)) {
				EmbeddedStatement(
#line  3397 "VBNET.ATG" 
out embeddedStatement);
			} else SynErr(322);

#line  3398 "VBNET.ATG" 
			if (embeddedStatement != null) list.Add(embeddedStatement); 
		}
	}

	void CaseClauses(
#line  3532 "VBNET.ATG" 
out List<CaseLabel> caseClauses) {

#line  3534 "VBNET.ATG" 
		caseClauses = new List<CaseLabel>();
		CaseLabel caseClause = null;
		
		CaseClause(
#line  3537 "VBNET.ATG" 
out caseClause);

#line  3537 "VBNET.ATG" 
		if (caseClause != null) { caseClauses.Add(caseClause); } 
		while (la.kind == 22) {
			lexer.NextToken();
			CaseClause(
#line  3538 "VBNET.ATG" 
out caseClause);

#line  3538 "VBNET.ATG" 
			if (caseClause != null) { caseClauses.Add(caseClause); } 
		}
	}

	void OnErrorStatement(
#line  3423 "VBNET.ATG" 
out OnErrorStatement stmt) {

#line  3425 "VBNET.ATG" 
		stmt = null;
		Location startLocation = la.Location;
		GotoStatement goToStatement = null;
		
		Expect(174);
		Expect(120);
		if (
#line  3432 "VBNET.ATG" 
IsNegativeLabelName()) {
			Expect(134);
			Expect(30);
			Expect(5);

#line  3434 "VBNET.ATG" 
			long intLabel = Int64.Parse(t.val);
			if(intLabel != 1) {
				Error("invalid label in on error statement.");
			}
			stmt = new OnErrorStatement(new GotoStatement((intLabel * -1).ToString()));
			
		} else if (la.kind == 134) {
			GotoStatement(
#line  3440 "VBNET.ATG" 
out goToStatement);

#line  3442 "VBNET.ATG" 
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
			
		} else if (la.kind == 197) {
			lexer.NextToken();
			Expect(166);

#line  3456 "VBNET.ATG" 
			stmt = new OnErrorStatement(new ResumeStatement(true));
			
		} else SynErr(323);

#line  3460 "VBNET.ATG" 
		if (stmt != null) {
		stmt.StartLocation = startLocation;
		stmt.EndLocation = t.EndLocation;
		}
		
	}

	void GotoStatement(
#line  3468 "VBNET.ATG" 
out GotoStatement goToStatement) {

#line  3469 "VBNET.ATG" 
		string label = String.Empty; Location startLocation = la.Location; 
		Expect(134);
		LabelName(
#line  3471 "VBNET.ATG" 
out label);

#line  3473 "VBNET.ATG" 
		goToStatement = new GotoStatement(label) {
		StartLocation = startLocation,
		EndLocation = t.EndLocation
		};
		
	}

	void ResumeStatement(
#line  3521 "VBNET.ATG" 
out ResumeStatement resumeStatement) {

#line  3523 "VBNET.ATG" 
		resumeStatement = null;
		string label = String.Empty;
		
		if (
#line  3526 "VBNET.ATG" 
IsResumeNext()) {
			Expect(197);
			Expect(166);

#line  3527 "VBNET.ATG" 
			resumeStatement = new ResumeStatement(true); 
		} else if (la.kind == 197) {
			lexer.NextToken();
			if (StartOf(51)) {
				LabelName(
#line  3528 "VBNET.ATG" 
out label);
			}

#line  3528 "VBNET.ATG" 
			resumeStatement = new ResumeStatement(label); 
		} else SynErr(324);
	}

	void ReDimClauseInternal(
#line  3496 "VBNET.ATG" 
ref Expression expr) {

#line  3497 "VBNET.ATG" 
		List<Expression> arguments; bool canBeNormal; bool canBeRedim; string name; Location startLocation = la.Location; 
		while (la.kind == 26 || 
#line  3500 "VBNET.ATG" 
la.kind == Tokens.OpenParenthesis && Peek(1).kind == Tokens.Of) {
			if (la.kind == 26) {
				lexer.NextToken();
				IdentifierOrKeyword(
#line  3499 "VBNET.ATG" 
out name);

#line  3499 "VBNET.ATG" 
				expr = new MemberReferenceExpression(expr, name) { StartLocation = startLocation, EndLocation = t.EndLocation }; 
			} else {
				InvocationExpression(
#line  3501 "VBNET.ATG" 
ref expr);

#line  3503 "VBNET.ATG" 
				expr.StartLocation = startLocation;
				expr.EndLocation = t.EndLocation;
				
			}
		}
		Expect(37);
		NormalOrReDimArgumentList(
#line  3508 "VBNET.ATG" 
out arguments, out canBeNormal, out canBeRedim);
		Expect(38);

#line  3510 "VBNET.ATG" 
		expr = new InvocationExpression(expr, arguments);
		if (canBeRedim == false || canBeNormal && (la.kind == Tokens.Dot || la.kind == Tokens.OpenParenthesis)) {
			if (this.Errors.Count == 0) {
				// don't recurse on parse errors - could result in endless recursion
				ReDimClauseInternal(ref expr);
			}
		}
		
	}

	void CaseClause(
#line  3542 "VBNET.ATG" 
out CaseLabel caseClause) {

#line  3544 "VBNET.ATG" 
		Expression expr = null;
		Expression sexpr = null;
		BinaryOperatorType op = BinaryOperatorType.None;
		caseClause = null;
		
		if (la.kind == 113) {
			lexer.NextToken();

#line  3550 "VBNET.ATG" 
			caseClause = new CaseLabel(); 
		} else if (StartOf(52)) {
			if (la.kind == 146) {
				lexer.NextToken();
			}
			switch (la.kind) {
			case 40: {
				lexer.NextToken();

#line  3554 "VBNET.ATG" 
				op = BinaryOperatorType.LessThan; 
				break;
			}
			case 39: {
				lexer.NextToken();

#line  3555 "VBNET.ATG" 
				op = BinaryOperatorType.GreaterThan; 
				break;
			}
			case 43: {
				lexer.NextToken();

#line  3556 "VBNET.ATG" 
				op = BinaryOperatorType.LessThanOrEqual; 
				break;
			}
			case 42: {
				lexer.NextToken();

#line  3557 "VBNET.ATG" 
				op = BinaryOperatorType.GreaterThanOrEqual; 
				break;
			}
			case 20: {
				lexer.NextToken();

#line  3558 "VBNET.ATG" 
				op = BinaryOperatorType.Equality; 
				break;
			}
			case 41: {
				lexer.NextToken();

#line  3559 "VBNET.ATG" 
				op = BinaryOperatorType.InEquality; 
				break;
			}
			default: SynErr(325); break;
			}
			Expr(
#line  3561 "VBNET.ATG" 
out expr);

#line  3563 "VBNET.ATG" 
			caseClause = new CaseLabel(op, expr);
			
		} else if (StartOf(25)) {
			Expr(
#line  3565 "VBNET.ATG" 
out expr);
			if (la.kind == 219) {
				lexer.NextToken();
				Expr(
#line  3565 "VBNET.ATG" 
out sexpr);
			}

#line  3567 "VBNET.ATG" 
			caseClause = new CaseLabel(expr, sexpr);
			
		} else SynErr(326);
	}

	void CatchClauses(
#line  3614 "VBNET.ATG" 
out List<CatchClause> catchClauses) {

#line  3616 "VBNET.ATG" 
		catchClauses = new List<CatchClause>();
		TypeReference type = null;
		Statement blockStmt = null;
		Expression expr = null;
		string name = String.Empty;
		
		while (la.kind == 77) {
			lexer.NextToken();
			if (StartOf(4)) {
				Identifier();

#line  3624 "VBNET.ATG" 
				name = t.val; 
				if (la.kind == 63) {
					lexer.NextToken();
					TypeName(
#line  3624 "VBNET.ATG" 
out type);
				}
			}
			if (la.kind == 232) {
				lexer.NextToken();
				Expr(
#line  3625 "VBNET.ATG" 
out expr);
			}
			EndOfStmt();
			Block(
#line  3627 "VBNET.ATG" 
out blockStmt);

#line  3628 "VBNET.ATG" 
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
			case 66: s = "\"Async\" expected"; break;
			case 67: s = "\"Auto\" expected"; break;
			case 68: s = "\"Await\" expected"; break;
			case 69: s = "\"Binary\" expected"; break;
			case 70: s = "\"Boolean\" expected"; break;
			case 71: s = "\"ByRef\" expected"; break;
			case 72: s = "\"By\" expected"; break;
			case 73: s = "\"Byte\" expected"; break;
			case 74: s = "\"ByVal\" expected"; break;
			case 75: s = "\"Call\" expected"; break;
			case 76: s = "\"Case\" expected"; break;
			case 77: s = "\"Catch\" expected"; break;
			case 78: s = "\"CBool\" expected"; break;
			case 79: s = "\"CByte\" expected"; break;
			case 80: s = "\"CChar\" expected"; break;
			case 81: s = "\"CDate\" expected"; break;
			case 82: s = "\"CDbl\" expected"; break;
			case 83: s = "\"CDec\" expected"; break;
			case 84: s = "\"Char\" expected"; break;
			case 85: s = "\"CInt\" expected"; break;
			case 86: s = "\"Class\" expected"; break;
			case 87: s = "\"CLng\" expected"; break;
			case 88: s = "\"CObj\" expected"; break;
			case 89: s = "\"Compare\" expected"; break;
			case 90: s = "\"Const\" expected"; break;
			case 91: s = "\"Continue\" expected"; break;
			case 92: s = "\"CSByte\" expected"; break;
			case 93: s = "\"CShort\" expected"; break;
			case 94: s = "\"CSng\" expected"; break;
			case 95: s = "\"CStr\" expected"; break;
			case 96: s = "\"CType\" expected"; break;
			case 97: s = "\"CUInt\" expected"; break;
			case 98: s = "\"CULng\" expected"; break;
			case 99: s = "\"CUShort\" expected"; break;
			case 100: s = "\"Custom\" expected"; break;
			case 101: s = "\"Date\" expected"; break;
			case 102: s = "\"Decimal\" expected"; break;
			case 103: s = "\"Declare\" expected"; break;
			case 104: s = "\"Default\" expected"; break;
			case 105: s = "\"Delegate\" expected"; break;
			case 106: s = "\"Descending\" expected"; break;
			case 107: s = "\"Dim\" expected"; break;
			case 108: s = "\"DirectCast\" expected"; break;
			case 109: s = "\"Distinct\" expected"; break;
			case 110: s = "\"Do\" expected"; break;
			case 111: s = "\"Double\" expected"; break;
			case 112: s = "\"Each\" expected"; break;
			case 113: s = "\"Else\" expected"; break;
			case 114: s = "\"ElseIf\" expected"; break;
			case 115: s = "\"End\" expected"; break;
			case 116: s = "\"EndIf\" expected"; break;
			case 117: s = "\"Enum\" expected"; break;
			case 118: s = "\"Equals\" expected"; break;
			case 119: s = "\"Erase\" expected"; break;
			case 120: s = "\"Error\" expected"; break;
			case 121: s = "\"Event\" expected"; break;
			case 122: s = "\"Exit\" expected"; break;
			case 123: s = "\"Explicit\" expected"; break;
			case 124: s = "\"False\" expected"; break;
			case 125: s = "\"Finally\" expected"; break;
			case 126: s = "\"For\" expected"; break;
			case 127: s = "\"Friend\" expected"; break;
			case 128: s = "\"From\" expected"; break;
			case 129: s = "\"Function\" expected"; break;
			case 130: s = "\"Get\" expected"; break;
			case 131: s = "\"GetType\" expected"; break;
			case 132: s = "\"Global\" expected"; break;
			case 133: s = "\"GoSub\" expected"; break;
			case 134: s = "\"GoTo\" expected"; break;
			case 135: s = "\"Group\" expected"; break;
			case 136: s = "\"Handles\" expected"; break;
			case 137: s = "\"If\" expected"; break;
			case 138: s = "\"Implements\" expected"; break;
			case 139: s = "\"Imports\" expected"; break;
			case 140: s = "\"In\" expected"; break;
			case 141: s = "\"Infer\" expected"; break;
			case 142: s = "\"Inherits\" expected"; break;
			case 143: s = "\"Integer\" expected"; break;
			case 144: s = "\"Interface\" expected"; break;
			case 145: s = "\"Into\" expected"; break;
			case 146: s = "\"Is\" expected"; break;
			case 147: s = "\"IsNot\" expected"; break;
			case 148: s = "\"Iterator\" expected"; break;
			case 149: s = "\"Join\" expected"; break;
			case 150: s = "\"Key\" expected"; break;
			case 151: s = "\"Let\" expected"; break;
			case 152: s = "\"Lib\" expected"; break;
			case 153: s = "\"Like\" expected"; break;
			case 154: s = "\"Long\" expected"; break;
			case 155: s = "\"Loop\" expected"; break;
			case 156: s = "\"Me\" expected"; break;
			case 157: s = "\"Mod\" expected"; break;
			case 158: s = "\"Module\" expected"; break;
			case 159: s = "\"MustInherit\" expected"; break;
			case 160: s = "\"MustOverride\" expected"; break;
			case 161: s = "\"MyBase\" expected"; break;
			case 162: s = "\"MyClass\" expected"; break;
			case 163: s = "\"Namespace\" expected"; break;
			case 164: s = "\"Narrowing\" expected"; break;
			case 165: s = "\"New\" expected"; break;
			case 166: s = "\"Next\" expected"; break;
			case 167: s = "\"Not\" expected"; break;
			case 168: s = "\"Nothing\" expected"; break;
			case 169: s = "\"NotInheritable\" expected"; break;
			case 170: s = "\"NotOverridable\" expected"; break;
			case 171: s = "\"Object\" expected"; break;
			case 172: s = "\"Of\" expected"; break;
			case 173: s = "\"Off\" expected"; break;
			case 174: s = "\"On\" expected"; break;
			case 175: s = "\"Operator\" expected"; break;
			case 176: s = "\"Option\" expected"; break;
			case 177: s = "\"Optional\" expected"; break;
			case 178: s = "\"Or\" expected"; break;
			case 179: s = "\"Order\" expected"; break;
			case 180: s = "\"OrElse\" expected"; break;
			case 181: s = "\"Out\" expected"; break;
			case 182: s = "\"Overloads\" expected"; break;
			case 183: s = "\"Overridable\" expected"; break;
			case 184: s = "\"Overrides\" expected"; break;
			case 185: s = "\"ParamArray\" expected"; break;
			case 186: s = "\"Partial\" expected"; break;
			case 187: s = "\"Preserve\" expected"; break;
			case 188: s = "\"Private\" expected"; break;
			case 189: s = "\"Property\" expected"; break;
			case 190: s = "\"Protected\" expected"; break;
			case 191: s = "\"Public\" expected"; break;
			case 192: s = "\"RaiseEvent\" expected"; break;
			case 193: s = "\"ReadOnly\" expected"; break;
			case 194: s = "\"ReDim\" expected"; break;
			case 195: s = "\"Rem\" expected"; break;
			case 196: s = "\"RemoveHandler\" expected"; break;
			case 197: s = "\"Resume\" expected"; break;
			case 198: s = "\"Return\" expected"; break;
			case 199: s = "\"SByte\" expected"; break;
			case 200: s = "\"Select\" expected"; break;
			case 201: s = "\"Set\" expected"; break;
			case 202: s = "\"Shadows\" expected"; break;
			case 203: s = "\"Shared\" expected"; break;
			case 204: s = "\"Short\" expected"; break;
			case 205: s = "\"Single\" expected"; break;
			case 206: s = "\"Skip\" expected"; break;
			case 207: s = "\"Static\" expected"; break;
			case 208: s = "\"Step\" expected"; break;
			case 209: s = "\"Stop\" expected"; break;
			case 210: s = "\"Strict\" expected"; break;
			case 211: s = "\"String\" expected"; break;
			case 212: s = "\"Structure\" expected"; break;
			case 213: s = "\"Sub\" expected"; break;
			case 214: s = "\"SyncLock\" expected"; break;
			case 215: s = "\"Take\" expected"; break;
			case 216: s = "\"Text\" expected"; break;
			case 217: s = "\"Then\" expected"; break;
			case 218: s = "\"Throw\" expected"; break;
			case 219: s = "\"To\" expected"; break;
			case 220: s = "\"True\" expected"; break;
			case 221: s = "\"Try\" expected"; break;
			case 222: s = "\"TryCast\" expected"; break;
			case 223: s = "\"TypeOf\" expected"; break;
			case 224: s = "\"UInteger\" expected"; break;
			case 225: s = "\"ULong\" expected"; break;
			case 226: s = "\"Unicode\" expected"; break;
			case 227: s = "\"Until\" expected"; break;
			case 228: s = "\"UShort\" expected"; break;
			case 229: s = "\"Using\" expected"; break;
			case 230: s = "\"Variant\" expected"; break;
			case 231: s = "\"Wend\" expected"; break;
			case 232: s = "\"When\" expected"; break;
			case 233: s = "\"Where\" expected"; break;
			case 234: s = "\"While\" expected"; break;
			case 235: s = "\"Widening\" expected"; break;
			case 236: s = "\"With\" expected"; break;
			case 237: s = "\"WithEvents\" expected"; break;
			case 238: s = "\"WriteOnly\" expected"; break;
			case 239: s = "\"Xor\" expected"; break;
			case 240: s = "\"Yield\" expected"; break;
			case 241: s = "\"GetXmlNamespace\" expected"; break;
			case 242: s = "??? expected"; break;
			case 243: s = "this symbol not expected in EndOfStmt"; break;
			case 244: s = "invalid EndOfStmt"; break;
			case 245: s = "invalid OptionStmt"; break;
			case 246: s = "invalid OptionStmt"; break;
			case 247: s = "invalid GlobalAttributeSection"; break;
			case 248: s = "invalid GlobalAttributeSection"; break;
			case 249: s = "invalid NamespaceMemberDecl"; break;
			case 250: s = "invalid OptionValue"; break;
			case 251: s = "invalid ImportClause"; break;
			case 252: s = "invalid Identifier"; break;
			case 253: s = "invalid TypeModifier"; break;
			case 254: s = "invalid NonModuleDeclaration"; break;
			case 255: s = "invalid NonModuleDeclaration"; break;
			case 256: s = "invalid TypeParameterConstraints"; break;
			case 257: s = "invalid TypeParameterConstraint"; break;
			case 258: s = "invalid NonArrayTypeName"; break;
			case 259: s = "invalid MemberModifier"; break;
			case 260: s = "invalid StructureMemberDecl"; break;
			case 261: s = "invalid StructureMemberDecl"; break;
			case 262: s = "invalid StructureMemberDecl"; break;
			case 263: s = "invalid StructureMemberDecl"; break;
			case 264: s = "invalid StructureMemberDecl"; break;
			case 265: s = "invalid StructureMemberDecl"; break;
			case 266: s = "invalid StructureMemberDecl"; break;
			case 267: s = "invalid StructureMemberDecl"; break;
			case 268: s = "invalid InterfaceMemberDecl"; break;
			case 269: s = "invalid InterfaceMemberDecl"; break;
			case 270: s = "invalid Expr"; break;
			case 271: s = "invalid Charset"; break;
			case 272: s = "invalid IdentifierForFieldDeclaration"; break;
			case 273: s = "invalid VariableDeclaratorPartAfterIdentifier"; break;
			case 274: s = "invalid ObjectCreateExpression"; break;
			case 275: s = "invalid ObjectCreateExpression"; break;
			case 276: s = "invalid AccessorDecls"; break;
			case 277: s = "invalid EventAccessorDeclaration"; break;
			case 278: s = "invalid OverloadableOperator"; break;
			case 279: s = "invalid EventMemberSpecifier"; break;
			case 280: s = "invalid LambdaExpr"; break;
			case 281: s = "invalid AssignmentOperator"; break;
			case 282: s = "invalid SimpleExpr"; break;
			case 283: s = "invalid SimpleExpr"; break;
			case 284: s = "invalid SimpleNonInvocationExpression"; break;
			case 285: s = "invalid SimpleNonInvocationExpression"; break;
			case 286: s = "invalid SimpleNonInvocationExpression"; break;
			case 287: s = "invalid SimpleNonInvocationExpression"; break;
			case 288: s = "invalid SimpleNonInvocationExpression"; break;
			case 289: s = "invalid SimpleNonInvocationExpression"; break;
			case 290: s = "invalid PrimitiveTypeName"; break;
			case 291: s = "invalid CastTarget"; break;
			case 292: s = "invalid XmlLiteralExpression"; break;
			case 293: s = "invalid XmlContentExpression"; break;
			case 294: s = "invalid XmlElement"; break;
			case 295: s = "invalid XmlElement"; break;
			case 296: s = "invalid XmlNestedContent"; break;
			case 297: s = "invalid XmlAttribute"; break;
			case 298: s = "invalid XmlAttribute"; break;
			case 299: s = "invalid ComparisonExpr"; break;
			case 300: s = "invalid SubLambdaExpression"; break;
			case 301: s = "invalid FunctionLambdaExpression"; break;
			case 302: s = "invalid EmbeddedStatement"; break;
			case 303: s = "invalid EmbeddedStatement"; break;
			case 304: s = "invalid EmbeddedStatement"; break;
			case 305: s = "invalid EmbeddedStatement"; break;
			case 306: s = "invalid EmbeddedStatement"; break;
			case 307: s = "invalid EmbeddedStatement"; break;
			case 308: s = "invalid EmbeddedStatement"; break;
			case 309: s = "invalid FromOrAggregateQueryOperator"; break;
			case 310: s = "invalid QueryOperator"; break;
			case 311: s = "invalid PartitionQueryOperator"; break;
			case 312: s = "invalid Argument"; break;
			case 313: s = "invalid QualIdentAndTypeArguments"; break;
			case 314: s = "invalid AttributeArguments"; break;
			case 315: s = "invalid AttributeArguments"; break;
			case 316: s = "invalid AttributeArguments"; break;
			case 317: s = "invalid ParameterModifier"; break;
			case 318: s = "invalid Statement"; break;
			case 319: s = "invalid LabelName"; break;
			case 320: s = "invalid WhileOrUntil"; break;
			case 321: s = "invalid SingleLineStatementList"; break;
			case 322: s = "invalid SingleLineStatementList"; break;
			case 323: s = "invalid OnErrorStatement"; break;
			case 324: s = "invalid ResumeStatement"; break;
			case 325: s = "invalid CaseClause"; break;
			case 326: s = "invalid CaseClause"; break;

			default: s = "error " + errorNumber; break;
		}
		this.Errors.Error(line, col, s);
	}
	
	private bool StartOf(int s)
	{
		return set[s, lexer.LookAhead.kind];
	}
	
	static bool[,] set = {
	{T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,T, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,x,T,T, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,x,T,T, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,x,T,T, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,T,x, T,T,T,T, T,T,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,T,x,x, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,x, T,x,x,x, x,x,x,T, x,x,x,x, x,T,x,x, x,T,x,x, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,T,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,T,x, T,T,x,T, T,T,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,T,x,x, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,x, T,x,x,x, x,x,x,T, x,x,x,x, x,T,x,x, x,T,x,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,T,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,T,x, T,T,T,T, T,T,x,T, T,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,T,x,x, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,x, T,x,x,x, x,x,x,T, x,x,x,x, x,T,x,x, x,T,x,x, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, x,T,x,x, x,T,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,T,x, T,T,T,T, T,T,T,x, T,T,x,x, x,x,x,x, x,x,x,x, T,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, T,T,T,x, x,x,T,x, x,T,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,x, T,x,x,x, T,x,x,T, x,x,x,x, x,T,x,T, x,T,x,x, T,T,T,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,T,x,x, x,x,x,T, x,T,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, T,T,T,x, x,x,x,T, T,x,x,T, T,x,x,x, x,x,x,x, T,T,T,T, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,T,x, T,T,T,T, T,T,T,x, T,T,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, T,T,T,x, x,x,T,x, x,T,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,x, T,x,x,x, T,x,x,T, x,x,x,x, x,T,x,T, x,T,x,x, T,T,T,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,x,x, x,x,x,T, x,T,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, T,T,T,x, x,x,x,T, x,x,x,T, T,x,x,x, x,x,x,x, T,T,T,T, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,T,x, T,T,T,T, T,T,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,T,T,x, x,x,x,x, x,x,x,x, T,x,x,T, T,T,T,T, x,T,x,x, x,x,x,x, x,T,T,x, x,T,x,T, x,x,x,T, T,T,x,x, x,x,x,T, x,x,x,x, x,T,x,x, T,T,x,x, T,T,T,x, x,x,x,x, x,x,T,T, T,x,x,x, T,x,x,x, x,T,T,x, x,T,x,T, x,x,x,T, x,T,T,T, T,x,T,T, T,T,T,T, x,T,x,x, x,x,x,x, x,x,T,T, x,x,T,x, x,x,x,x, T,T,x,T, T,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,T,x,T, x,T,T,x, T,x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,T,T, T,x,T,x, T,x,T,T, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,T,x, T,T,T,T, T,T,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,T,x,x, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,x, T,x,x,x, T,x,x,T, x,x,x,x, x,T,x,x, x,T,x,x, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,T,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,T,x, T,T,T,T, T,T,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,T,x,x, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,x, T,x,x,x, x,x,x,T, x,x,x,x, x,T,x,x, x,T,x,x, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,T,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,T, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,x, x,x,x,T, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, T,x,x,x, x,x,x,x, x,x,T,T, T,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,T,T, T,x,T,x, T,T,T,T, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,T, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,T,T, T,x,T,x, T,T,T,T, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
	{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, T,T,T,T, x,x,x,x, x,x,T,T, T,T,T,T, x,x,T,T, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,T,x, T,T,T,T, T,T,T,x, T,T,x,x, x,x,T,T, T,T,T,T, T,T,x,T, T,T,x,x, T,T,T,T, T,T,T,T, T,T,T,x, x,x,T,x, T,T,x,T, x,x,x,x, x,x,T,x, x,x,x,T, T,x,x,x, T,x,x,T, T,x,x,T, x,T,x,x, x,T,x,T, x,T,x,x, T,T,T,x, x,x,T,x, T,x,x,x, x,T,T,x, x,T,x,T, T,x,x,T, x,T,x,x, x,x,x,T, x,T,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, T,T,T,x, x,x,x,T, x,x,x,T, T,x,x,x, T,x,T,T, T,T,T,T, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x},
	{x,T,T,T, T,T,T,T, T,T,T,x, x,x,x,x, T,T,T,T, x,T,x,x, x,x,T,T, T,T,x,x, x,x,x,T, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,T,x, x,x,T,x, T,T,T,T, T,T,T,x, T,T,x,T, x,x,T,T, T,T,T,T, T,T,x,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, x,x,T,T, T,T,T,T, x,x,x,x, x,x,T,T, T,x,T,T, T,x,T,x, T,x,x,T, T,x,T,T, x,T,x,x, x,T,x,T, x,T,x,x, T,T,T,x, x,x,T,x, T,x,x,x, x,T,T,x, x,T,x,x, T,x,x,T, x,T,T,x, x,x,x,T, x,T,x,x, x,x,x,T, x,x,x,x, T,x,T,x, T,T,T,T, T,x,x,x, T,T,T,T, x,T,x,T, x,x,T,T, T,x,T,x, T,T,T,T, T,T,T,T, T,T,x,x, x,T,T,x, T,x,x,x, T,x,x,x},
	{x,T,T,T, T,T,T,T, T,T,T,x, x,T,x,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, T,T,T,x, T,T,T,T, T,T,T,T, T,T,T,x, T,T,x,T, x,x,T,T, T,T,T,T, T,T,x,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, x,x,T,T, T,T,T,T, x,T,x,T, x,x,T,T, T,x,T,T, T,x,T,x, T,x,x,T, T,x,T,T, x,T,T,x, x,T,x,T, x,T,T,T, T,T,T,T, x,T,T,x, T,T,x,x, x,T,T,x, x,T,x,x, T,x,x,T, x,T,T,x, x,x,T,T, T,T,x,x, x,x,x,T, x,x,x,x, T,x,T,x, T,T,T,T, T,x,x,x, T,T,T,T, T,T,x,T, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,x, x,T,T,x, T,x,x,T, T,x,x,x},
	{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, T,T,T,T, x,x,x,x, x,x,T,T, T,T,T,T, x,x,T,T, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,T,x, T,T,T,T, T,T,T,x, T,T,x,x, x,x,T,T, T,T,T,T, T,T,x,T, T,T,x,x, T,T,T,T, T,T,T,T, T,T,T,x, x,x,T,x, T,T,x,T, x,x,x,x, x,x,T,x, x,x,x,T, T,x,x,x, T,T,x,T, T,x,x,T, x,T,x,x, x,T,x,T, x,T,x,x, T,T,T,x, x,x,T,x, T,x,x,x, x,T,T,x, x,T,x,T, T,x,x,T, x,T,x,x, x,x,x,T, x,T,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, T,T,T,x, x,x,x,T, x,T,x,T, T,x,x,x, T,x,T,T, T,T,T,T, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,T, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,T, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
	{x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x},
	{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, T,T,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,T,x, T,T,T,T, T,T,T,x, T,T,x,x, x,x,T,T, T,T,T,T, T,T,x,T, T,T,x,x, T,T,T,T, T,T,T,T, T,T,T,x, x,x,T,x, T,T,x,T, x,x,x,x, x,x,T,x, x,x,x,T, T,x,x,x, T,x,x,T, T,x,x,T, x,T,x,x, x,T,x,T, x,T,x,x, T,T,T,x, x,x,T,x, T,x,x,x, x,T,T,x, x,T,x,x, T,x,x,T, x,T,x,x, x,x,x,T, x,T,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, T,T,T,x, x,x,x,T, x,x,x,T, T,x,x,x, T,x,T,T, T,T,T,T, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
	{x,x,T,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,T,x, T,T,T,T, T,T,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,T,x,x, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,x, T,x,x,x, x,x,x,T, x,x,x,x, x,T,x,x, x,T,x,x, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,T,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x},
	{x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,x, T,T,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
	{x,T,T,T, T,T,T,T, T,T,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
	{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, T,T,T,T, x,x,x,x, x,x,T,T, T,T,T,T, x,x,T,T, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,T,x, T,T,T,T, T,T,T,x, T,T,x,x, x,x,T,T, T,T,T,T, T,T,x,T, T,T,x,x, T,T,T,T, T,T,T,T, T,T,T,x, x,x,T,x, T,T,x,T, x,x,x,x, x,x,T,x, x,x,x,T, T,x,x,x, T,x,x,T, T,x,x,T, x,T,x,x, x,T,x,T, x,T,x,x, T,T,T,x, x,x,T,x, T,x,x,x, x,T,T,x, x,T,x,x, T,x,x,T, x,T,x,x, x,x,x,T, x,T,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, T,T,T,x, x,x,x,T, x,x,x,T, T,x,x,x, T,x,T,T, T,T,T,T, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x},
	{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, T,T,T,T, x,x,x,x, x,x,T,T, T,T,T,T, x,x,T,T, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,T,x, x,x,T,x, T,T,T,T, T,T,T,x, T,T,x,T, x,x,T,T, T,T,T,T, T,T,x,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, x,x,T,T, T,T,T,T, x,x,x,x, x,x,T,T, T,x,T,T, T,x,T,x, T,T,x,T, T,x,T,T, x,T,x,x, x,T,x,T, x,T,x,x, T,T,T,x, x,x,T,x, T,x,x,x, x,T,T,x, x,T,x,T, T,x,x,T, x,T,T,x, x,x,x,T, x,T,x,x, x,x,x,T, x,x,x,x, T,x,T,x, T,T,T,T, T,x,x,x, T,T,T,T, x,T,x,T, x,T,T,T, T,x,T,x, T,T,T,T, T,T,T,T, T,T,x,x, x,T,T,x, T,x,x,x, T,x,x,x},
	{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, T,T,T,T, x,x,T,x, x,x,T,T, T,T,T,T, x,x,T,T, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,T,x, T,T,T,T, T,T,T,x, T,T,x,x, x,x,T,T, T,T,T,T, T,T,x,T, T,T,x,x, T,T,T,T, T,T,T,T, T,T,T,x, x,x,T,x, T,T,x,T, x,x,x,x, x,x,T,x, x,x,x,T, T,x,x,x, T,T,x,T, T,x,x,T, x,T,x,x, x,T,x,T, x,T,x,x, T,T,T,x, x,x,T,x, T,x,x,x, x,T,T,x, x,T,x,T, T,x,x,T, x,T,x,x, x,x,x,T, x,T,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, T,T,T,x, x,x,x,T, x,T,x,T, T,x,x,x, T,x,T,T, T,T,T,T, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x},
	{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, T,T,T,T, x,x,x,x, x,x,T,T, T,T,x,x, x,x,x,T, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,T,x, T,T,T,T, T,T,T,x, T,T,x,x, x,x,T,T, T,T,T,T, T,T,x,T, T,T,x,x, T,T,T,T, T,T,T,T, T,T,T,x, x,x,T,x, T,T,x,T, x,x,x,x, x,x,T,x, x,x,x,T, T,x,x,x, T,x,x,T, T,x,x,T, x,T,x,x, x,T,x,T, x,T,x,x, T,T,T,x, x,x,T,x, T,x,x,x, x,T,T,x, x,T,x,x, T,x,x,T, x,T,x,x, x,x,x,T, x,T,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, T,T,T,x, x,x,x,T, x,x,x,T, T,x,x,x, T,x,T,T, T,T,T,T, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x},
	{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, T,T,T,T, x,x,x,x, x,x,T,T, T,T,x,x, x,x,x,T, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,T,x, x,x,T,x, T,T,T,T, T,T,T,x, T,T,x,T, x,x,T,T, T,T,T,T, T,T,x,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, x,x,T,T, T,T,T,T, x,x,x,T, x,x,T,T, T,x,T,T, T,x,T,x, T,x,x,T, T,x,T,T, x,T,x,x, x,T,x,T, x,T,x,x, T,T,T,x, x,x,T,x, T,x,x,x, x,T,T,x, x,T,x,x, T,x,x,T, x,T,T,x, x,x,x,T, x,T,x,x, x,x,x,T, x,x,x,x, T,x,T,x, T,T,T,T, T,x,x,x, T,T,T,T, x,T,x,T, x,x,T,T, T,x,T,x, T,T,T,T, T,T,T,T, T,T,x,x, x,T,T,x, T,x,x,x, T,x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
	{x,T,T,T, T,T,T,T, T,T,T,x, x,T,x,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, T,T,T,x, T,T,T,x, T,T,T,T, T,T,T,x, T,T,x,T, x,x,T,T, T,T,T,T, T,T,x,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, x,x,T,T, T,T,T,T, x,T,x,T, x,x,T,T, T,x,T,T, T,x,T,x, T,x,x,T, T,x,T,T, x,T,T,x, x,T,x,T, x,T,T,T, T,T,T,T, x,T,T,x, T,T,x,x, x,T,T,x, x,T,x,x, T,x,x,T, x,T,T,x, x,x,T,T, T,T,x,x, x,x,x,T, x,x,x,x, T,x,T,x, T,T,T,T, T,x,x,x, T,T,T,T, T,T,x,T, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,x, x,T,T,x, T,x,x,T, T,x,x,x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,T,x, T,T,T,T, T,T,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,T,T, x,T,x,x, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,x, T,x,x,x, x,x,x,T, x,x,x,x, x,T,x,x, x,T,x,x, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,T,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x},
	{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, T,T,T,T, x,x,x,x, x,x,T,T, T,T,x,x, x,x,x,T, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,T,x, x,x,T,x, T,T,T,T, T,T,T,x, T,T,x,T, x,x,T,T, T,T,T,T, T,T,x,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, x,x,T,T, T,T,T,T, x,x,x,x, x,x,T,T, T,x,T,T, T,x,T,x, T,x,x,T, T,x,T,T, x,T,x,x, x,T,x,T, x,T,x,x, T,T,T,x, x,x,T,x, T,x,x,x, x,T,T,x, x,T,x,x, T,x,x,T, x,T,T,x, x,x,x,T, x,T,x,x, x,x,x,T, x,x,x,x, T,x,T,x, T,T,T,T, T,x,x,x, T,T,T,T, x,T,x,T, x,x,T,T, T,x,T,x, T,T,T,T, T,T,T,T, T,T,x,x, x,T,T,x, T,x,x,x, T,x,x,x},
	{x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,T,x, T,T,T,T, T,T,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,T,x,x, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,x, T,x,x,x, x,x,x,T, x,x,x,x, x,T,x,x, x,T,x,x, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,T,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x}

	};
} // end Parser

}