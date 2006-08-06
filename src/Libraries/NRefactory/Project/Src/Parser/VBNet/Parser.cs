
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
	const int maxT = 205;

	const  bool   T            = true;
	const  bool   x            = false;
	

#line  12 "VBNET.ATG" 


/*

*/

	void VBNET() {

#line  230 "VBNET.ATG" 
		lexer.NextToken(); // get the first token
		compilationUnit = new CompilationUnit();
		
		while (la.kind == 1) {
			lexer.NextToken();
		}
		while (la.kind == 136) {
			OptionStmt();
		}
		while (la.kind == 108) {
			ImportsStmt();
		}
		while (
#line  236 "VBNET.ATG" 
IsGlobalAttrTarget()) {
			GlobalAttributeSection();
		}
		while (StartOf(1)) {
			NamespaceMemberDecl();
		}
		Expect(0);
	}

	void OptionStmt() {

#line  241 "VBNET.ATG" 
		INode node = null; bool val = true; 
		Expect(136);

#line  242 "VBNET.ATG" 
		Location startPos = t.Location; 
		if (la.kind == 95) {
			lexer.NextToken();
			if (la.kind == 134 || la.kind == 135) {
				OptionValue(
#line  244 "VBNET.ATG" 
ref val);
			}

#line  245 "VBNET.ATG" 
			node = new OptionDeclaration(OptionType.Explicit, val); 
		} else if (la.kind == 164) {
			lexer.NextToken();
			if (la.kind == 134 || la.kind == 135) {
				OptionValue(
#line  247 "VBNET.ATG" 
ref val);
			}

#line  248 "VBNET.ATG" 
			node = new OptionDeclaration(OptionType.Strict, val); 
		} else if (la.kind == 70) {
			lexer.NextToken();
			if (la.kind == 51) {
				lexer.NextToken();

#line  250 "VBNET.ATG" 
				node = new OptionDeclaration(OptionType.CompareBinary, val); 
			} else if (la.kind == 169) {
				lexer.NextToken();

#line  251 "VBNET.ATG" 
				node = new OptionDeclaration(OptionType.CompareText, val); 
			} else SynErr(206);
		} else SynErr(207);
		EndOfStmt();

#line  256 "VBNET.ATG" 
		if (node != null) {
		node.StartLocation = startPos;
		node.EndLocation   = t.Location;
		compilationUnit.AddChild(node);
		}
		
	}

	void ImportsStmt() {

#line  279 "VBNET.ATG" 
		List<Using> usings = new List<Using>();
		
		Expect(108);

#line  283 "VBNET.ATG" 
		Location startPos = t.Location;
		Using u;
		
		ImportClause(
#line  286 "VBNET.ATG" 
out u);

#line  286 "VBNET.ATG" 
		if (u != null) { usings.Add(u); } 
		while (la.kind == 12) {
			lexer.NextToken();
			ImportClause(
#line  288 "VBNET.ATG" 
out u);

#line  288 "VBNET.ATG" 
			if (u != null) { usings.Add(u); } 
		}
		EndOfStmt();

#line  292 "VBNET.ATG" 
		UsingDeclaration usingDeclaration = new UsingDeclaration(usings);
		usingDeclaration.StartLocation = startPos;
		usingDeclaration.EndLocation   = t.Location;
		compilationUnit.AddChild(usingDeclaration);
		
	}

	void GlobalAttributeSection() {

#line  2005 "VBNET.ATG" 
		Location startPos = t.Location; 
		Expect(27);
		if (la.kind == 49) {
			lexer.NextToken();
		} else if (la.kind == 121) {
			lexer.NextToken();
		} else SynErr(208);

#line  2007 "VBNET.ATG" 
		string attributeTarget = t.val.ToLower(System.Globalization.CultureInfo.InvariantCulture);
		List<ASTAttribute> attributes = new List<ASTAttribute>();
		ASTAttribute attribute;
		
		Expect(13);
		Attribute(
#line  2011 "VBNET.ATG" 
out attribute);

#line  2011 "VBNET.ATG" 
		attributes.Add(attribute); 
		while (
#line  2012 "VBNET.ATG" 
NotFinalComma()) {
			if (la.kind == 12) {
				lexer.NextToken();
				if (la.kind == 49) {
					lexer.NextToken();
				} else if (la.kind == 121) {
					lexer.NextToken();
				} else SynErr(209);
				Expect(13);
			}
			Attribute(
#line  2012 "VBNET.ATG" 
out attribute);

#line  2012 "VBNET.ATG" 
			attributes.Add(attribute); 
		}
		if (la.kind == 12) {
			lexer.NextToken();
		}
		Expect(26);
		EndOfStmt();

#line  2017 "VBNET.ATG" 
		AttributeSection section = new AttributeSection(attributeTarget, attributes);
		section.StartLocation = startPos;
		section.EndLocation = t.EndLocation;
		compilationUnit.AddChild(section);
		
	}

	void NamespaceMemberDecl() {

#line  321 "VBNET.ATG" 
		ModifierList m = new ModifierList();
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		string qualident;
		
		if (la.kind == 126) {
			lexer.NextToken();

#line  328 "VBNET.ATG" 
			Location startPos = t.Location;
			
			Qualident(
#line  330 "VBNET.ATG" 
out qualident);

#line  332 "VBNET.ATG" 
			INode node =  new NamespaceDeclaration(qualident);
			node.StartLocation = startPos;
			compilationUnit.AddChild(node);
			compilationUnit.BlockStart(node);
			
			Expect(1);
			NamespaceBody();

#line  340 "VBNET.ATG" 
			node.EndLocation = t.Location;
			compilationUnit.BlockEnd();
			
		} else if (StartOf(2)) {
			while (la.kind == 27) {
				AttributeSection(
#line  344 "VBNET.ATG" 
out section);

#line  344 "VBNET.ATG" 
				attributes.Add(section); 
			}
			while (StartOf(3)) {
				TypeModifier(
#line  345 "VBNET.ATG" 
m);
			}
			NonModuleDeclaration(
#line  345 "VBNET.ATG" 
m, attributes);
		} else SynErr(210);
	}

	void OptionValue(
#line  264 "VBNET.ATG" 
ref bool val) {
		if (la.kind == 135) {
			lexer.NextToken();

#line  266 "VBNET.ATG" 
			val = true; 
		} else if (la.kind == 134) {
			lexer.NextToken();

#line  268 "VBNET.ATG" 
			val = false; 
		} else SynErr(211);
	}

	void EndOfStmt() {
		if (la.kind == 1) {
			lexer.NextToken();
		} else if (la.kind == 13) {
			lexer.NextToken();
			if (la.kind == 1) {
				lexer.NextToken();
			}
		} else SynErr(212);
	}

	void ImportClause(
#line  299 "VBNET.ATG" 
out Using u) {

#line  301 "VBNET.ATG" 
		string qualident  = null;
		TypeReference aliasedType = null;
		u = null;
		
		Qualident(
#line  305 "VBNET.ATG" 
out qualident);
		if (la.kind == 11) {
			lexer.NextToken();
			TypeName(
#line  306 "VBNET.ATG" 
out aliasedType);
		}

#line  308 "VBNET.ATG" 
		if (qualident != null && qualident.Length > 0) {
		if (aliasedType != null) {
			u = new Using(qualident, aliasedType);
		} else {
			u = new Using(qualident);
		}
		}
		
	}

	void Qualident(
#line  2722 "VBNET.ATG" 
out string qualident) {

#line  2724 "VBNET.ATG" 
		string name;
		qualidentBuilder.Length = 0; 
		
		Identifier();

#line  2728 "VBNET.ATG" 
		qualidentBuilder.Append(t.val); 
		while (
#line  2729 "VBNET.ATG" 
DotAndIdentOrKw()) {
			Expect(10);
			IdentifierOrKeyword(
#line  2729 "VBNET.ATG" 
out name);

#line  2729 "VBNET.ATG" 
			qualidentBuilder.Append('.'); qualidentBuilder.Append(name); 
		}

#line  2731 "VBNET.ATG" 
		qualident = qualidentBuilder.ToString(); 
	}

	void TypeName(
#line  1898 "VBNET.ATG" 
out TypeReference typeref) {

#line  1899 "VBNET.ATG" 
		ArrayList rank = null; 
		NonArrayTypeName(
#line  1901 "VBNET.ATG" 
out typeref, false);
		ArrayTypeModifiers(
#line  1902 "VBNET.ATG" 
out rank);

#line  1903 "VBNET.ATG" 
		if (rank != null && typeref != null) {
		typeref.RankSpecifier = (int[])rank.ToArray(typeof(int));
		}
		
	}

	void NamespaceBody() {
		while (StartOf(1)) {
			NamespaceMemberDecl();
		}
		Expect(88);
		Expect(126);
		Expect(1);
	}

	void AttributeSection(
#line  2075 "VBNET.ATG" 
out AttributeSection section) {

#line  2077 "VBNET.ATG" 
		string attributeTarget = "";List<ASTAttribute> attributes = new List<ASTAttribute>();
		ASTAttribute attribute;
		
		
		Expect(27);

#line  2081 "VBNET.ATG" 
		Location startPos = t.Location; 
		if (
#line  2082 "VBNET.ATG" 
IsLocalAttrTarget()) {
			if (la.kind == 93) {
				lexer.NextToken();

#line  2083 "VBNET.ATG" 
				attributeTarget = "event";
			} else if (la.kind == 154) {
				lexer.NextToken();

#line  2084 "VBNET.ATG" 
				attributeTarget = "return";
			} else {
				Identifier();

#line  2087 "VBNET.ATG" 
				string val = t.val.ToLower(System.Globalization.CultureInfo.InvariantCulture);
				if (val != "field"	|| val != "method" ||
					val != "module" || val != "param"  ||
					val != "property" || val != "type")
				Error("attribute target specifier (event, return, field," +
						"method, module, param, property, or type) expected");
				attributeTarget = t.val;
				
			}
			Expect(13);
		}
		Attribute(
#line  2097 "VBNET.ATG" 
out attribute);

#line  2097 "VBNET.ATG" 
		attributes.Add(attribute); 
		while (
#line  2098 "VBNET.ATG" 
NotFinalComma()) {
			Expect(12);
			Attribute(
#line  2098 "VBNET.ATG" 
out attribute);

#line  2098 "VBNET.ATG" 
			attributes.Add(attribute); 
		}
		if (la.kind == 12) {
			lexer.NextToken();
		}
		Expect(26);

#line  2102 "VBNET.ATG" 
		section = new AttributeSection(attributeTarget, attributes);
		section.StartLocation = startPos;
		section.EndLocation = t.EndLocation;
		
	}

	void TypeModifier(
#line  2798 "VBNET.ATG" 
ModifierList m) {
		switch (la.kind) {
		case 148: {
			lexer.NextToken();

#line  2799 "VBNET.ATG" 
			m.Add(Modifiers.Public, t.Location); 
			break;
		}
		case 147: {
			lexer.NextToken();

#line  2800 "VBNET.ATG" 
			m.Add(Modifiers.Protected, t.Location); 
			break;
		}
		case 99: {
			lexer.NextToken();

#line  2801 "VBNET.ATG" 
			m.Add(Modifiers.Internal, t.Location); 
			break;
		}
		case 145: {
			lexer.NextToken();

#line  2802 "VBNET.ATG" 
			m.Add(Modifiers.Private, t.Location); 
			break;
		}
		case 158: {
			lexer.NextToken();

#line  2803 "VBNET.ATG" 
			m.Add(Modifiers.Static, t.Location); 
			break;
		}
		case 157: {
			lexer.NextToken();

#line  2804 "VBNET.ATG" 
			m.Add(Modifiers.New, t.Location); 
			break;
		}
		case 122: {
			lexer.NextToken();

#line  2805 "VBNET.ATG" 
			m.Add(Modifiers.Abstract, t.Location); 
			break;
		}
		case 131: {
			lexer.NextToken();

#line  2806 "VBNET.ATG" 
			m.Add(Modifiers.Sealed, t.Location); 
			break;
		}
		case 203: {
			lexer.NextToken();

#line  2807 "VBNET.ATG" 
			m.Add(Modifiers.Partial, t.Location); 
			break;
		}
		default: SynErr(213); break;
		}
	}

	void NonModuleDeclaration(
#line  396 "VBNET.ATG" 
ModifierList m, List<AttributeSection> attributes) {

#line  398 "VBNET.ATG" 
		TypeReference typeRef = null;
		List<TypeReference> baseInterfaces = null;
		
		switch (la.kind) {
		case 67: {

#line  401 "VBNET.ATG" 
			m.Check(Modifiers.Classes); 
			lexer.NextToken();

#line  404 "VBNET.ATG" 
			TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
			newType.StartLocation = t.Location;
			compilationUnit.AddChild(newType);
			compilationUnit.BlockStart(newType);
			
			newType.Type       = ClassType.Class;
			
			Identifier();

#line  411 "VBNET.ATG" 
			newType.Name = t.val; 
			TypeParameterList(
#line  412 "VBNET.ATG" 
newType.Templates);
			EndOfStmt();

#line  414 "VBNET.ATG" 
			newType.BodyStartLocation = t.Location; 
			if (la.kind == 110) {
				ClassBaseType(
#line  415 "VBNET.ATG" 
out typeRef);

#line  415 "VBNET.ATG" 
				newType.BaseTypes.Add(typeRef); 
			}
			while (la.kind == 107) {
				TypeImplementsClause(
#line  416 "VBNET.ATG" 
out baseInterfaces);

#line  416 "VBNET.ATG" 
				newType.BaseTypes.AddRange(baseInterfaces); 
			}
			ClassBody(
#line  417 "VBNET.ATG" 
newType);

#line  419 "VBNET.ATG" 
			compilationUnit.BlockEnd();
			
			break;
		}
		case 121: {
			lexer.NextToken();

#line  423 "VBNET.ATG" 
			m.Check(Modifiers.VBModules);
			TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
			compilationUnit.AddChild(newType);
			compilationUnit.BlockStart(newType);
			newType.StartLocation = m.GetDeclarationLocation(t.Location);
			newType.Type = ClassType.Module;
			
			Identifier();

#line  430 "VBNET.ATG" 
			newType.Name = t.val; 
			Expect(1);

#line  432 "VBNET.ATG" 
			newType.BodyStartLocation = t.Location; 
			ModuleBody(
#line  433 "VBNET.ATG" 
newType);

#line  435 "VBNET.ATG" 
			compilationUnit.BlockEnd();
			
			break;
		}
		case 166: {
			lexer.NextToken();

#line  439 "VBNET.ATG" 
			m.Check(Modifiers.VBStructures);
			TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
			compilationUnit.AddChild(newType);
			compilationUnit.BlockStart(newType);
			newType.StartLocation = m.GetDeclarationLocation(t.Location);
			newType.Type = ClassType.Struct;
			
			Identifier();

#line  446 "VBNET.ATG" 
			newType.Name = t.val; 
			TypeParameterList(
#line  447 "VBNET.ATG" 
newType.Templates);
			Expect(1);

#line  449 "VBNET.ATG" 
			newType.BodyStartLocation = t.Location; 
			while (la.kind == 107) {
				TypeImplementsClause(
#line  450 "VBNET.ATG" 
out baseInterfaces);

#line  450 "VBNET.ATG" 
				newType.BaseTypes.AddRange(baseInterfaces);
			}
			StructureBody(
#line  451 "VBNET.ATG" 
newType);

#line  453 "VBNET.ATG" 
			compilationUnit.BlockEnd();
			
			break;
		}
		case 90: {
			lexer.NextToken();

#line  458 "VBNET.ATG" 
			m.Check(Modifiers.VBEnums);
			TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
			newType.StartLocation = m.GetDeclarationLocation(t.Location);
			compilationUnit.AddChild(newType);
			compilationUnit.BlockStart(newType);
			
			newType.Type = ClassType.Enum;
			
			Identifier();

#line  466 "VBNET.ATG" 
			newType.Name = t.val; 
			if (la.kind == 48) {
				lexer.NextToken();
				NonArrayTypeName(
#line  467 "VBNET.ATG" 
out typeRef, false);

#line  467 "VBNET.ATG" 
				newType.BaseTypes.Add(typeRef); 
			}
			Expect(1);

#line  469 "VBNET.ATG" 
			newType.BodyStartLocation = t.Location; 
			EnumBody(
#line  470 "VBNET.ATG" 
newType);

#line  472 "VBNET.ATG" 
			compilationUnit.BlockEnd();
			
			break;
		}
		case 112: {
			lexer.NextToken();

#line  477 "VBNET.ATG" 
			m.Check(Modifiers.VBInterfacs);
			TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
			newType.StartLocation = m.GetDeclarationLocation(t.Location);
			compilationUnit.AddChild(newType);
			compilationUnit.BlockStart(newType);
			newType.Type = ClassType.Interface;
			
			Identifier();

#line  484 "VBNET.ATG" 
			newType.Name = t.val; 
			TypeParameterList(
#line  485 "VBNET.ATG" 
newType.Templates);
			EndOfStmt();

#line  487 "VBNET.ATG" 
			newType.BodyStartLocation = t.Location; 
			while (la.kind == 110) {
				InterfaceBase(
#line  488 "VBNET.ATG" 
out baseInterfaces);

#line  488 "VBNET.ATG" 
				newType.BaseTypes.AddRange(baseInterfaces); 
			}
			InterfaceBody(
#line  489 "VBNET.ATG" 
newType);

#line  491 "VBNET.ATG" 
			compilationUnit.BlockEnd();
			
			break;
		}
		case 80: {
			lexer.NextToken();

#line  496 "VBNET.ATG" 
			m.Check(Modifiers.VBDelegates);
			DelegateDeclaration delegateDeclr = new DelegateDeclaration(m.Modifier, attributes);
			delegateDeclr.ReturnType = new TypeReference("", "System.Void");
			delegateDeclr.StartLocation = m.GetDeclarationLocation(t.Location);
			List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
			
			if (la.kind == 167) {
				lexer.NextToken();
				Identifier();

#line  503 "VBNET.ATG" 
				delegateDeclr.Name = t.val; 
				TypeParameterList(
#line  504 "VBNET.ATG" 
delegateDeclr.Templates);
				if (la.kind == 24) {
					lexer.NextToken();
					if (StartOf(4)) {
						FormalParameterList(
#line  505 "VBNET.ATG" 
p);
					}
					Expect(25);

#line  505 "VBNET.ATG" 
					delegateDeclr.Parameters = p; 
				}
			} else if (la.kind == 100) {
				lexer.NextToken();
				Identifier();

#line  507 "VBNET.ATG" 
				delegateDeclr.Name = t.val; 
				TypeParameterList(
#line  508 "VBNET.ATG" 
delegateDeclr.Templates);
				if (la.kind == 24) {
					lexer.NextToken();
					if (StartOf(4)) {
						FormalParameterList(
#line  509 "VBNET.ATG" 
p);
					}
					Expect(25);

#line  509 "VBNET.ATG" 
					delegateDeclr.Parameters = p; 
				}
				if (la.kind == 48) {
					lexer.NextToken();

#line  510 "VBNET.ATG" 
					TypeReference type; 
					TypeName(
#line  510 "VBNET.ATG" 
out type);

#line  510 "VBNET.ATG" 
					delegateDeclr.ReturnType = type; 
				}
			} else SynErr(214);

#line  512 "VBNET.ATG" 
			delegateDeclr.EndLocation = t.EndLocation; 
			Expect(1);

#line  515 "VBNET.ATG" 
			compilationUnit.AddChild(delegateDeclr);
			
			break;
		}
		default: SynErr(215); break;
		}
	}

	void TypeParameterList(
#line  349 "VBNET.ATG" 
List<TemplateDefinition> templates) {

#line  351 "VBNET.ATG" 
		TemplateDefinition template;
		
		if (
#line  354 "VBNET.ATG" 
la.kind == Tokens.OpenParenthesis && Peek(1).kind == Tokens.Of) {
			lexer.NextToken();
			Expect(200);
			TypeParameter(
#line  355 "VBNET.ATG" 
out template);

#line  357 "VBNET.ATG" 
			if (template != null) templates.Add(template);
			
			while (la.kind == 12) {
				lexer.NextToken();
				TypeParameter(
#line  360 "VBNET.ATG" 
out template);

#line  362 "VBNET.ATG" 
				if (template != null) templates.Add(template);
				
			}
			Expect(25);
		}
	}

	void TypeParameter(
#line  370 "VBNET.ATG" 
out TemplateDefinition template) {
		Identifier();

#line  372 "VBNET.ATG" 
		template = new TemplateDefinition(t.val, null); 
		if (la.kind == 48) {
			TypeParameterConstraints(
#line  373 "VBNET.ATG" 
template);
		}
	}

	void Identifier() {
		switch (la.kind) {
		case 2: {
			lexer.NextToken();
			break;
		}
		case 169: {
			lexer.NextToken();
			break;
		}
		case 51: {
			lexer.NextToken();
			break;
		}
		case 70: {
			lexer.NextToken();
			break;
		}
		case 204: {
			lexer.NextToken();
			break;
		}
		case 49: {
			lexer.NextToken();
			break;
		}
		case 47: {
			lexer.NextToken();
			break;
		}
		case 50: {
			lexer.NextToken();
			break;
		}
		case 144: {
			lexer.NextToken();
			break;
		}
		case 176: {
			lexer.NextToken();
			break;
		}
		case 177: {
			lexer.NextToken();
			break;
		}
		default: SynErr(216); break;
		}
	}

	void TypeParameterConstraints(
#line  377 "VBNET.ATG" 
TemplateDefinition template) {

#line  379 "VBNET.ATG" 
		TypeReference constraint;
		
		Expect(48);
		if (la.kind == 22) {
			lexer.NextToken();
			TypeName(
#line  385 "VBNET.ATG" 
out constraint);

#line  385 "VBNET.ATG" 
			if (constraint != null) { template.Bases.Add(constraint); } 
			while (la.kind == 12) {
				lexer.NextToken();
				TypeName(
#line  388 "VBNET.ATG" 
out constraint);

#line  388 "VBNET.ATG" 
				if (constraint != null) { template.Bases.Add(constraint); } 
			}
			Expect(23);
		} else if (StartOf(5)) {
			TypeName(
#line  391 "VBNET.ATG" 
out constraint);

#line  391 "VBNET.ATG" 
			if (constraint != null) { template.Bases.Add(constraint); } 
		} else SynErr(217);
	}

	void ClassBaseType(
#line  692 "VBNET.ATG" 
out TypeReference typeRef) {

#line  694 "VBNET.ATG" 
		typeRef = null;
		
		Expect(110);
		TypeName(
#line  697 "VBNET.ATG" 
out typeRef);
		EndOfStmt();
	}

	void TypeImplementsClause(
#line  1470 "VBNET.ATG" 
out List<TypeReference> baseInterfaces) {

#line  1472 "VBNET.ATG" 
		baseInterfaces = new List<TypeReference>();
		TypeReference type = null;
		
		Expect(107);
		TypeName(
#line  1475 "VBNET.ATG" 
out type);

#line  1477 "VBNET.ATG" 
		baseInterfaces.Add(type);
		
		while (la.kind == 12) {
			lexer.NextToken();
			TypeName(
#line  1480 "VBNET.ATG" 
out type);

#line  1481 "VBNET.ATG" 
			baseInterfaces.Add(type); 
		}
		EndOfStmt();
	}

	void ClassBody(
#line  525 "VBNET.ATG" 
TypeDeclaration newType) {

#line  526 "VBNET.ATG" 
		AttributeSection section; 
		while (StartOf(6)) {

#line  528 "VBNET.ATG" 
			List<AttributeSection> attributes = new List<AttributeSection>();
			ModifierList m = new ModifierList();
			
			while (la.kind == 27) {
				AttributeSection(
#line  531 "VBNET.ATG" 
out section);

#line  531 "VBNET.ATG" 
				attributes.Add(section); 
			}
			while (StartOf(7)) {
				MemberModifier(
#line  532 "VBNET.ATG" 
m);
			}
			ClassMemberDecl(
#line  533 "VBNET.ATG" 
m, attributes);
		}
		Expect(88);
		Expect(67);

#line  535 "VBNET.ATG" 
		newType.EndLocation = t.EndLocation; 
		Expect(1);
	}

	void ModuleBody(
#line  554 "VBNET.ATG" 
TypeDeclaration newType) {

#line  555 "VBNET.ATG" 
		AttributeSection section; 
		while (StartOf(6)) {

#line  557 "VBNET.ATG" 
			List<AttributeSection> attributes = new List<AttributeSection>();
			ModifierList m = new ModifierList();
			
			while (la.kind == 27) {
				AttributeSection(
#line  560 "VBNET.ATG" 
out section);

#line  560 "VBNET.ATG" 
				attributes.Add(section); 
			}
			while (StartOf(7)) {
				MemberModifier(
#line  561 "VBNET.ATG" 
m);
			}
			ClassMemberDecl(
#line  562 "VBNET.ATG" 
m, attributes);
		}
		Expect(88);
		Expect(121);

#line  564 "VBNET.ATG" 
		newType.EndLocation = t.EndLocation; 
		Expect(1);
	}

	void StructureBody(
#line  539 "VBNET.ATG" 
TypeDeclaration newType) {

#line  540 "VBNET.ATG" 
		AttributeSection section; 
		while (StartOf(6)) {

#line  542 "VBNET.ATG" 
			List<AttributeSection> attributes = new List<AttributeSection>();
			ModifierList m = new ModifierList();
			
			while (la.kind == 27) {
				AttributeSection(
#line  545 "VBNET.ATG" 
out section);

#line  545 "VBNET.ATG" 
				attributes.Add(section); 
			}
			while (StartOf(7)) {
				MemberModifier(
#line  546 "VBNET.ATG" 
m);
			}
			StructureMemberDecl(
#line  547 "VBNET.ATG" 
m, attributes);
		}
		Expect(88);
		Expect(166);

#line  549 "VBNET.ATG" 
		newType.EndLocation = t.EndLocation; 
		Expect(1);
	}

	void NonArrayTypeName(
#line  1921 "VBNET.ATG" 
out TypeReference typeref, bool canBeUnbound) {

#line  1923 "VBNET.ATG" 
		string name;
		typeref = null;
		bool isGlobal = false;
		
		if (StartOf(8)) {
			if (la.kind == 198) {
				lexer.NextToken();
				Expect(10);

#line  1928 "VBNET.ATG" 
				isGlobal = true; 
			}
			QualIdentAndTypeArguments(
#line  1929 "VBNET.ATG" 
out typeref, canBeUnbound);

#line  1930 "VBNET.ATG" 
			typeref.IsGlobal = isGlobal; 
			while (la.kind == 10) {
				lexer.NextToken();

#line  1931 "VBNET.ATG" 
				TypeReference nestedTypeRef; 
				QualIdentAndTypeArguments(
#line  1932 "VBNET.ATG" 
out nestedTypeRef, canBeUnbound);

#line  1933 "VBNET.ATG" 
				typeref = new InnerClassTypeReference(typeref, nestedTypeRef.Type, nestedTypeRef.GenericTypes); 
			}
		} else if (la.kind == 133) {
			lexer.NextToken();

#line  1936 "VBNET.ATG" 
			typeref = new TypeReference("System.Object"); 
		} else if (StartOf(9)) {
			PrimitiveTypeName(
#line  1937 "VBNET.ATG" 
out name);

#line  1937 "VBNET.ATG" 
			typeref = new TypeReference(name); 
		} else SynErr(218);
	}

	void EnumBody(
#line  568 "VBNET.ATG" 
TypeDeclaration newType) {

#line  569 "VBNET.ATG" 
		FieldDeclaration f; 
		while (StartOf(10)) {
			EnumMemberDecl(
#line  571 "VBNET.ATG" 
out f);

#line  571 "VBNET.ATG" 
			compilationUnit.AddChild(f); 
		}
		Expect(88);
		Expect(90);

#line  573 "VBNET.ATG" 
		newType.EndLocation = t.EndLocation; 
		Expect(1);
	}

	void InterfaceBase(
#line  1455 "VBNET.ATG" 
out List<TypeReference> bases) {

#line  1457 "VBNET.ATG" 
		TypeReference type;
		bases = new List<TypeReference>();
		
		Expect(110);
		TypeName(
#line  1461 "VBNET.ATG" 
out type);

#line  1461 "VBNET.ATG" 
		bases.Add(type); 
		while (la.kind == 12) {
			lexer.NextToken();
			TypeName(
#line  1464 "VBNET.ATG" 
out type);

#line  1464 "VBNET.ATG" 
			bases.Add(type); 
		}
		Expect(1);
	}

	void InterfaceBody(
#line  577 "VBNET.ATG" 
TypeDeclaration newType) {
		while (StartOf(11)) {
			InterfaceMemberDecl();
		}
		Expect(88);
		Expect(112);

#line  579 "VBNET.ATG" 
		newType.EndLocation = t.EndLocation; 
		Expect(1);
	}

	void FormalParameterList(
#line  2109 "VBNET.ATG" 
List<ParameterDeclarationExpression> parameter) {

#line  2111 "VBNET.ATG" 
		ParameterDeclarationExpression p;
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		
		while (la.kind == 27) {
			AttributeSection(
#line  2115 "VBNET.ATG" 
out section);

#line  2115 "VBNET.ATG" 
			attributes.Add(section); 
		}
		FormalParameter(
#line  2117 "VBNET.ATG" 
out p);

#line  2119 "VBNET.ATG" 
		bool paramsFound = false;
		p.Attributes = attributes;
		parameter.Add(p);
		
		while (la.kind == 12) {
			lexer.NextToken();

#line  2124 "VBNET.ATG" 
			if (paramsFound) Error("params array must be at end of parameter list"); 
			while (la.kind == 27) {
				AttributeSection(
#line  2125 "VBNET.ATG" 
out section);

#line  2125 "VBNET.ATG" 
				attributes.Add(section); 
			}
			FormalParameter(
#line  2127 "VBNET.ATG" 
out p);

#line  2127 "VBNET.ATG" 
			p.Attributes = attributes; parameter.Add(p); 
		}
	}

	void MemberModifier(
#line  2810 "VBNET.ATG" 
ModifierList m) {
		switch (la.kind) {
		case 122: {
			lexer.NextToken();

#line  2811 "VBNET.ATG" 
			m.Add(Modifiers.Abstract, t.Location);
			break;
		}
		case 79: {
			lexer.NextToken();

#line  2812 "VBNET.ATG" 
			m.Add(Modifiers.Default, t.Location);
			break;
		}
		case 99: {
			lexer.NextToken();

#line  2813 "VBNET.ATG" 
			m.Add(Modifiers.Internal, t.Location);
			break;
		}
		case 157: {
			lexer.NextToken();

#line  2814 "VBNET.ATG" 
			m.Add(Modifiers.New, t.Location);
			break;
		}
		case 142: {
			lexer.NextToken();

#line  2815 "VBNET.ATG" 
			m.Add(Modifiers.Override, t.Location);
			break;
		}
		case 123: {
			lexer.NextToken();

#line  2816 "VBNET.ATG" 
			m.Add(Modifiers.Abstract, t.Location);
			break;
		}
		case 145: {
			lexer.NextToken();

#line  2817 "VBNET.ATG" 
			m.Add(Modifiers.Private, t.Location);
			break;
		}
		case 147: {
			lexer.NextToken();

#line  2818 "VBNET.ATG" 
			m.Add(Modifiers.Protected, t.Location);
			break;
		}
		case 148: {
			lexer.NextToken();

#line  2819 "VBNET.ATG" 
			m.Add(Modifiers.Public, t.Location);
			break;
		}
		case 131: {
			lexer.NextToken();

#line  2820 "VBNET.ATG" 
			m.Add(Modifiers.Sealed, t.Location);
			break;
		}
		case 132: {
			lexer.NextToken();

#line  2821 "VBNET.ATG" 
			m.Add(Modifiers.Sealed, t.Location);
			break;
		}
		case 158: {
			lexer.NextToken();

#line  2822 "VBNET.ATG" 
			m.Add(Modifiers.Static, t.Location);
			break;
		}
		case 141: {
			lexer.NextToken();

#line  2823 "VBNET.ATG" 
			m.Add(Modifiers.Virtual, t.Location);
			break;
		}
		case 140: {
			lexer.NextToken();

#line  2824 "VBNET.ATG" 
			m.Add(Modifiers.Overloads, t.Location);
			break;
		}
		case 150: {
			lexer.NextToken();

#line  2825 "VBNET.ATG" 
			m.Add(Modifiers.ReadOnly, t.Location);
			break;
		}
		case 184: {
			lexer.NextToken();

#line  2826 "VBNET.ATG" 
			m.Add(Modifiers.WriteOnly, t.Location);
			break;
		}
		case 183: {
			lexer.NextToken();

#line  2827 "VBNET.ATG" 
			m.Add(Modifiers.WithEvents, t.Location);
			break;
		}
		case 81: {
			lexer.NextToken();

#line  2828 "VBNET.ATG" 
			m.Add(Modifiers.Dim, t.Location);
			break;
		}
		default: SynErr(219); break;
		}
	}

	void ClassMemberDecl(
#line  688 "VBNET.ATG" 
ModifierList m, List<AttributeSection> attributes) {
		StructureMemberDecl(
#line  689 "VBNET.ATG" 
m, attributes);
	}

	void StructureMemberDecl(
#line  702 "VBNET.ATG" 
ModifierList m, List<AttributeSection> attributes) {

#line  704 "VBNET.ATG" 
		TypeReference type = null;
		List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
		Statement stmt = null;
		List<VariableDeclaration> variableDeclarators = new List<VariableDeclaration>();
		List<TemplateDefinition> templates = new List<TemplateDefinition>();
		
		switch (la.kind) {
		case 67: case 80: case 90: case 112: case 121: case 166: {
			NonModuleDeclaration(
#line  711 "VBNET.ATG" 
m, attributes);
			break;
		}
		case 167: {
			lexer.NextToken();

#line  715 "VBNET.ATG" 
			Location startPos = t.Location;
			
			if (StartOf(12)) {

#line  719 "VBNET.ATG" 
				string name = String.Empty;
				MethodDeclaration methodDeclaration; List<string> handlesClause = null;
				List<InterfaceImplementation> implementsClause = null;
				
				Identifier();

#line  725 "VBNET.ATG" 
				name = t.val;
				m.Check(Modifiers.VBMethods);
				
				TypeParameterList(
#line  728 "VBNET.ATG" 
templates);
				if (la.kind == 24) {
					lexer.NextToken();
					if (StartOf(4)) {
						FormalParameterList(
#line  729 "VBNET.ATG" 
p);
					}
					Expect(25);
				}
				if (la.kind == 105 || la.kind == 107) {
					if (la.kind == 107) {
						ImplementsClause(
#line  732 "VBNET.ATG" 
out implementsClause);
					} else {
						HandlesClause(
#line  734 "VBNET.ATG" 
out handlesClause);
					}
				}

#line  737 "VBNET.ATG" 
				Location endLocation = t.EndLocation; 
				Expect(1);
				if (
#line  741 "VBNET.ATG" 
IsMustOverride(m)) {

#line  743 "VBNET.ATG" 
					methodDeclaration = new MethodDeclaration(name, m.Modifier,  null, p, attributes);
					methodDeclaration.StartLocation = m.GetDeclarationLocation(startPos);
					methodDeclaration.EndLocation   = endLocation;
					methodDeclaration.TypeReference = new TypeReference("", "System.Void");
					
					methodDeclaration.Templates = templates;
					methodDeclaration.HandlesClause = handlesClause;
					methodDeclaration.InterfaceImplementations = implementsClause;
					
					compilationUnit.AddChild(methodDeclaration);
					
				} else if (StartOf(13)) {

#line  756 "VBNET.ATG" 
					methodDeclaration = new MethodDeclaration(name, m.Modifier,  null, p, attributes);
					methodDeclaration.StartLocation = m.GetDeclarationLocation(startPos);
					methodDeclaration.EndLocation   = endLocation;
					methodDeclaration.TypeReference = new TypeReference("", "System.Void");
					
					methodDeclaration.Templates = templates;
					methodDeclaration.HandlesClause = handlesClause;
					methodDeclaration.InterfaceImplementations = implementsClause;
					
					compilationUnit.AddChild(methodDeclaration);
					

#line  768 "VBNET.ATG" 
					if (ParseMethodBodies) { 
					Block(
#line  769 "VBNET.ATG" 
out stmt);
					Expect(88);
					Expect(167);

#line  771 "VBNET.ATG" 
					} else {
					// don't parse method body
					lexer.SkipCurrentBlock(Tokens.Sub); stmt = new BlockStatement();
					  }
					

#line  777 "VBNET.ATG" 
					methodDeclaration.Body  = (BlockStatement)stmt; 

#line  778 "VBNET.ATG" 
					methodDeclaration.Body.EndLocation = t.EndLocation; 
					Expect(1);
				} else SynErr(220);
			} else if (la.kind == 127) {
				lexer.NextToken();
				if (la.kind == 24) {
					lexer.NextToken();
					if (StartOf(4)) {
						FormalParameterList(
#line  781 "VBNET.ATG" 
p);
					}
					Expect(25);
				}

#line  782 "VBNET.ATG" 
				m.Check(Modifiers.Constructors); 

#line  783 "VBNET.ATG" 
				Location constructorEndLocation = t.EndLocation; 
				Expect(1);

#line  786 "VBNET.ATG" 
				if (ParseMethodBodies) { 
				Block(
#line  787 "VBNET.ATG" 
out stmt);
				Expect(88);
				Expect(167);

#line  789 "VBNET.ATG" 
				} else {
				// don't parse method body
				lexer.SkipCurrentBlock(Tokens.Sub); stmt = new BlockStatement();
				  }
				

#line  795 "VBNET.ATG" 
				Location endLocation = t.EndLocation; 
				Expect(1);

#line  797 "VBNET.ATG" 
				ConstructorDeclaration cd = new ConstructorDeclaration("New", m.Modifier, p, attributes); 
				cd.StartLocation = m.GetDeclarationLocation(startPos);
				cd.EndLocation   = constructorEndLocation;
				cd.Body = (BlockStatement)stmt;
				cd.Body.EndLocation   = endLocation;
				compilationUnit.AddChild(cd);
				
			} else SynErr(221);
			break;
		}
		case 100: {
			lexer.NextToken();

#line  809 "VBNET.ATG" 
			m.Check(Modifiers.VBMethods);
			string name = String.Empty;
			Location startPos = t.Location;
			MethodDeclaration methodDeclaration;List<string> handlesClause = null;
			List<InterfaceImplementation> implementsClause = null;
			AttributeSection returnTypeAttributeSection = null;
			
			Identifier();

#line  816 "VBNET.ATG" 
			name = t.val; 
			TypeParameterList(
#line  817 "VBNET.ATG" 
templates);
			if (la.kind == 24) {
				lexer.NextToken();
				if (StartOf(4)) {
					FormalParameterList(
#line  818 "VBNET.ATG" 
p);
				}
				Expect(25);
			}
			if (la.kind == 48) {
				lexer.NextToken();
				while (la.kind == 27) {
					AttributeSection(
#line  819 "VBNET.ATG" 
out returnTypeAttributeSection);
				}
				TypeName(
#line  819 "VBNET.ATG" 
out type);
			}

#line  821 "VBNET.ATG" 
			if(type == null) {
			type = new TypeReference("System.Object");
			}
			
			if (la.kind == 105 || la.kind == 107) {
				if (la.kind == 107) {
					ImplementsClause(
#line  827 "VBNET.ATG" 
out implementsClause);
				} else {
					HandlesClause(
#line  829 "VBNET.ATG" 
out handlesClause);
				}
			}
			Expect(1);
			if (
#line  835 "VBNET.ATG" 
IsMustOverride(m)) {

#line  837 "VBNET.ATG" 
				methodDeclaration = new MethodDeclaration(name, m.Modifier,  type, p, attributes);
				methodDeclaration.StartLocation = m.GetDeclarationLocation(startPos);
				methodDeclaration.EndLocation   = t.EndLocation;
				
				methodDeclaration.HandlesClause = handlesClause;
				methodDeclaration.Templates     = templates;
				methodDeclaration.InterfaceImplementations = implementsClause;
				if (returnTypeAttributeSection != null) {
					returnTypeAttributeSection.AttributeTarget = "return";
					methodDeclaration.Attributes.Add(returnTypeAttributeSection);
				}
				compilationUnit.AddChild(methodDeclaration);
				
			} else if (StartOf(13)) {

#line  852 "VBNET.ATG" 
				methodDeclaration = new MethodDeclaration(name, m.Modifier,  type, p, attributes);
				methodDeclaration.StartLocation = m.GetDeclarationLocation(startPos);
				methodDeclaration.EndLocation   = t.EndLocation;
				
				methodDeclaration.Templates     = templates;
				methodDeclaration.HandlesClause = handlesClause;
				methodDeclaration.InterfaceImplementations = implementsClause;
				if (returnTypeAttributeSection != null) {
					returnTypeAttributeSection.AttributeTarget = "return";
					methodDeclaration.Attributes.Add(returnTypeAttributeSection);
				}
				
				compilationUnit.AddChild(methodDeclaration);
				
				if (ParseMethodBodies) { 
				Block(
#line  867 "VBNET.ATG" 
out stmt);
				Expect(88);
				Expect(100);

#line  869 "VBNET.ATG" 
				} else {
				// don't parse method body
				lexer.SkipCurrentBlock(Tokens.Function); stmt = new BlockStatement();
				}
				methodDeclaration.Body = (BlockStatement)stmt;
				methodDeclaration.Body.StartLocation = methodDeclaration.EndLocation;
				methodDeclaration.Body.EndLocation   = t.EndLocation;
				
				Expect(1);
			} else SynErr(222);
			break;
		}
		case 78: {
			lexer.NextToken();

#line  883 "VBNET.ATG" 
			m.Check(Modifiers.VBExternalMethods);
			Location startPos = t.Location;
			CharsetModifier charsetModifer = CharsetModifier.None;
			string library = String.Empty;
			string alias = null;
			string name = String.Empty;
			
			if (StartOf(14)) {
				Charset(
#line  890 "VBNET.ATG" 
out charsetModifer);
			}
			if (la.kind == 167) {
				lexer.NextToken();
				Identifier();

#line  893 "VBNET.ATG" 
				name = t.val; 
				Expect(115);
				Expect(3);

#line  894 "VBNET.ATG" 
				library = t.literalValue.ToString(); 
				if (la.kind == 44) {
					lexer.NextToken();
					Expect(3);

#line  895 "VBNET.ATG" 
					alias = t.literalValue.ToString(); 
				}
				if (la.kind == 24) {
					lexer.NextToken();
					if (StartOf(4)) {
						FormalParameterList(
#line  896 "VBNET.ATG" 
p);
					}
					Expect(25);
				}
				Expect(1);

#line  899 "VBNET.ATG" 
				DeclareDeclaration declareDeclaration = new DeclareDeclaration(name, m.Modifier, null, p, attributes, library, alias, charsetModifer);
				declareDeclaration.StartLocation = m.GetDeclarationLocation(startPos);
				declareDeclaration.EndLocation   = t.EndLocation;
				compilationUnit.AddChild(declareDeclaration);
				
			} else if (la.kind == 100) {
				lexer.NextToken();
				Identifier();

#line  906 "VBNET.ATG" 
				name = t.val; 
				Expect(115);
				Expect(3);

#line  907 "VBNET.ATG" 
				library = t.literalValue.ToString(); 
				if (la.kind == 44) {
					lexer.NextToken();
					Expect(3);

#line  908 "VBNET.ATG" 
					alias = t.literalValue.ToString(); 
				}
				if (la.kind == 24) {
					lexer.NextToken();
					if (StartOf(4)) {
						FormalParameterList(
#line  909 "VBNET.ATG" 
p);
					}
					Expect(25);
				}
				if (la.kind == 48) {
					lexer.NextToken();
					TypeName(
#line  910 "VBNET.ATG" 
out type);
				}
				Expect(1);

#line  913 "VBNET.ATG" 
				DeclareDeclaration declareDeclaration = new DeclareDeclaration(name, m.Modifier, type, p, attributes, library, alias, charsetModifer);
				declareDeclaration.StartLocation = m.GetDeclarationLocation(startPos);
				declareDeclaration.EndLocation   = t.EndLocation;
				compilationUnit.AddChild(declareDeclaration);
				
			} else SynErr(223);
			break;
		}
		case 93: {
			lexer.NextToken();

#line  923 "VBNET.ATG" 
			m.Check(Modifiers.VBEvents);
			Location startPos = t.Location;
			EventDeclaration eventDeclaration;
			string name = String.Empty;
			List<InterfaceImplementation> implementsClause = null;
			
			Identifier();

#line  929 "VBNET.ATG" 
			name= t.val; 
			if (la.kind == 48) {
				lexer.NextToken();
				TypeName(
#line  931 "VBNET.ATG" 
out type);
			} else if (la.kind == 1 || la.kind == 24 || la.kind == 107) {
				if (la.kind == 24) {
					lexer.NextToken();
					if (StartOf(4)) {
						FormalParameterList(
#line  933 "VBNET.ATG" 
p);
					}
					Expect(25);
				}
			} else SynErr(224);
			if (la.kind == 107) {
				ImplementsClause(
#line  935 "VBNET.ATG" 
out implementsClause);
			}

#line  937 "VBNET.ATG" 
			eventDeclaration = new EventDeclaration(type, m.Modifier, p, attributes, name, implementsClause);
			eventDeclaration.StartLocation = m.GetDeclarationLocation(startPos);
			eventDeclaration.EndLocation = t.EndLocation;
			compilationUnit.AddChild(eventDeclaration);
			
			Expect(1);
			break;
		}
		case 2: case 47: case 49: case 50: case 51: case 70: case 144: case 169: case 176: case 177: {

#line  944 "VBNET.ATG" 
			Location startPos = t.Location; 

#line  946 "VBNET.ATG" 
			m.Check(Modifiers.Fields);
			FieldDeclaration fd = new FieldDeclaration(attributes, null, m.Modifier);
			fd.StartLocation = m.GetDeclarationLocation(startPos); 
			
			IdentifierForFieldDeclaration();

#line  950 "VBNET.ATG" 
			string name = t.val; 
			VariableDeclaratorPartAfterIdentifier(
#line  951 "VBNET.ATG" 
variableDeclarators, name);
			while (la.kind == 12) {
				lexer.NextToken();
				VariableDeclarator(
#line  952 "VBNET.ATG" 
variableDeclarators);
			}
			Expect(1);

#line  955 "VBNET.ATG" 
			fd.EndLocation = t.EndLocation;
			fd.Fields = variableDeclarators;
			compilationUnit.AddChild(fd);
			
			break;
		}
		case 71: {

#line  960 "VBNET.ATG" 
			m.Check(Modifiers.Fields); 
			lexer.NextToken();

#line  961 "VBNET.ATG" 
			m.Add(Modifiers.Const, t.Location);  

#line  963 "VBNET.ATG" 
			FieldDeclaration fd = new FieldDeclaration(attributes, type, m.Modifier);
			fd.StartLocation = m.GetDeclarationLocation(t.Location);
			List<VariableDeclaration> constantDeclarators = new List<VariableDeclaration>();
			
			ConstantDeclarator(
#line  967 "VBNET.ATG" 
constantDeclarators);
			while (la.kind == 12) {
				lexer.NextToken();
				ConstantDeclarator(
#line  968 "VBNET.ATG" 
constantDeclarators);
			}

#line  970 "VBNET.ATG" 
			fd.Fields = constantDeclarators;
			fd.EndLocation = t.Location;
			
			Expect(1);

#line  975 "VBNET.ATG" 
			fd.EndLocation = t.EndLocation;
			compilationUnit.AddChild(fd);
			
			break;
		}
		case 146: {
			lexer.NextToken();

#line  981 "VBNET.ATG" 
			m.Check(Modifiers.VBProperties);
			Location startPos = t.Location;
			List<InterfaceImplementation> implementsClause = null;
			
			Identifier();

#line  985 "VBNET.ATG" 
			string propertyName = t.val; 
			if (la.kind == 24) {
				lexer.NextToken();
				if (StartOf(4)) {
					FormalParameterList(
#line  986 "VBNET.ATG" 
p);
				}
				Expect(25);
			}
			if (la.kind == 48) {
				lexer.NextToken();
				TypeName(
#line  987 "VBNET.ATG" 
out type);
			}

#line  989 "VBNET.ATG" 
			if(type == null) {
			type = new TypeReference("System.Object");
			}
			
			if (la.kind == 107) {
				ImplementsClause(
#line  993 "VBNET.ATG" 
out implementsClause);
			}
			Expect(1);
			if (
#line  997 "VBNET.ATG" 
IsMustOverride(m)) {

#line  999 "VBNET.ATG" 
				PropertyDeclaration pDecl = new PropertyDeclaration(propertyName, type, m.Modifier, attributes);
				pDecl.StartLocation = m.GetDeclarationLocation(startPos);
				pDecl.EndLocation   = t.Location;
				pDecl.TypeReference = type;
				pDecl.InterfaceImplementations = implementsClause;
				pDecl.Parameters = p;
				compilationUnit.AddChild(pDecl);
				
			} else if (StartOf(15)) {

#line  1009 "VBNET.ATG" 
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
#line  1019 "VBNET.ATG" 
out getRegion, out setRegion);
				Expect(88);
				Expect(146);
				Expect(1);

#line  1023 "VBNET.ATG" 
				pDecl.GetRegion = getRegion;
				pDecl.SetRegion = setRegion;
				pDecl.BodyEnd = t.EndLocation;
				compilationUnit.AddChild(pDecl);
				
			} else SynErr(225);
			break;
		}
		case 204: {
			lexer.NextToken();

#line  1030 "VBNET.ATG" 
			Location startPos = t.Location; 
			Expect(93);

#line  1032 "VBNET.ATG" 
			m.Check(Modifiers.VBCustomEvents);
			EventAddRemoveRegion eventAccessorDeclaration;
			EventAddRegion addHandlerAccessorDeclaration = null;
			EventRemoveRegion removeHandlerAccessorDeclaration = null;
			EventRaiseRegion raiseEventAccessorDeclaration = null;
			List<InterfaceImplementation> implementsClause = null;
			
			Identifier();

#line  1039 "VBNET.ATG" 
			string customEventName = t.val; 
			Expect(48);
			TypeName(
#line  1040 "VBNET.ATG" 
out type);
			if (la.kind == 107) {
				ImplementsClause(
#line  1041 "VBNET.ATG" 
out implementsClause);
			}
			Expect(1);
			while (StartOf(16)) {
				EventAccessorDeclaration(
#line  1044 "VBNET.ATG" 
out eventAccessorDeclaration);

#line  1046 "VBNET.ATG" 
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
			Expect(88);
			Expect(93);
			Expect(1);

#line  1062 "VBNET.ATG" 
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
			
			EventDeclaration decl = new EventDeclaration(type, customEventName, m.Modifier, attributes, null);
			decl.StartLocation = m.GetDeclarationLocation(startPos);
			decl.EndLocation = t.EndLocation;
			decl.AddRegion = addHandlerAccessorDeclaration;
			decl.RemoveRegion = removeHandlerAccessorDeclaration;
			decl.RaiseRegion = raiseEventAccessorDeclaration;
			compilationUnit.AddChild(decl);
			
			break;
		}
		case 187: case 201: case 202: {

#line  1085 "VBNET.ATG" 
			ConversionType opConversionType = ConversionType.None; 
			if (la.kind == 201 || la.kind == 202) {
				if (la.kind == 202) {
					lexer.NextToken();

#line  1086 "VBNET.ATG" 
					opConversionType = ConversionType.Implicit; 
				} else {
					lexer.NextToken();

#line  1087 "VBNET.ATG" 
					opConversionType = ConversionType.Explicit;
				}
			}
			Expect(187);

#line  1090 "VBNET.ATG" 
			m.Check(Modifiers.VBOperators);
			Location startPos = t.Location;
			TypeReference returnType = NullTypeReference.Instance;
			TypeReference operandType = NullTypeReference.Instance;
			string operandName;
			OverloadableOperatorType operatorType;
			AttributeSection section;
			List<ParameterDeclarationExpression> parameters = new List<ParameterDeclarationExpression>();
			List<AttributeSection> returnTypeAttributes = new List<AttributeSection>();
			
			OverloadableOperator(
#line  1100 "VBNET.ATG" 
out operatorType);
			Expect(24);
			if (la.kind == 55) {
				lexer.NextToken();
			}
			Identifier();

#line  1101 "VBNET.ATG" 
			operandName = t.val; 
			if (la.kind == 48) {
				lexer.NextToken();
				TypeName(
#line  1102 "VBNET.ATG" 
out operandType);
			}

#line  1103 "VBNET.ATG" 
			parameters.Add(new ParameterDeclarationExpression(operandType, operandName, ParameterModifiers.In)); 
			while (la.kind == 12) {
				lexer.NextToken();
				if (la.kind == 55) {
					lexer.NextToken();
				}
				Identifier();

#line  1107 "VBNET.ATG" 
				operandName = t.val; 
				if (la.kind == 48) {
					lexer.NextToken();
					TypeName(
#line  1108 "VBNET.ATG" 
out operandType);
				}

#line  1109 "VBNET.ATG" 
				parameters.Add(new ParameterDeclarationExpression(operandType, operandName, ParameterModifiers.In)); 
			}
			Expect(25);

#line  1112 "VBNET.ATG" 
			Location endPos = t.EndLocation; 
			if (la.kind == 48) {
				lexer.NextToken();
				while (la.kind == 27) {
					AttributeSection(
#line  1113 "VBNET.ATG" 
out section);

#line  1113 "VBNET.ATG" 
					returnTypeAttributes.Add(section); 
				}
				TypeName(
#line  1113 "VBNET.ATG" 
out returnType);

#line  1113 "VBNET.ATG" 
				endPos = t.EndLocation; 
				Expect(1);
			}
			Block(
#line  1114 "VBNET.ATG" 
out stmt);
			Expect(88);
			Expect(187);
			Expect(1);

#line  1116 "VBNET.ATG" 
			OperatorDeclaration operatorDeclaration = new OperatorDeclaration(m.Modifier, 
			                                                                 attributes, 
			                                                                 parameters, 
			                                                                 returnType,
			                                                                 operatorType
			                                                                 );
			operatorDeclaration.ConversionType = opConversionType;
			operatorDeclaration.ReturnTypeAttributes = returnTypeAttributes;
			operatorDeclaration.Body = (BlockStatement)stmt;
			operatorDeclaration.StartLocation = m.GetDeclarationLocation(startPos);
			operatorDeclaration.EndLocation = endPos;
			operatorDeclaration.Body.StartLocation = startPos;
			operatorDeclaration.Body.EndLocation = t.Location;
			compilationUnit.AddChild(operatorDeclaration);
			
			break;
		}
		default: SynErr(226); break;
		}
	}

	void EnumMemberDecl(
#line  670 "VBNET.ATG" 
out FieldDeclaration f) {

#line  672 "VBNET.ATG" 
		Expression expr = null;List<AttributeSection> attributes = new List<AttributeSection>();
		AttributeSection section = null;
		VariableDeclaration varDecl = null;
		
		while (la.kind == 27) {
			AttributeSection(
#line  676 "VBNET.ATG" 
out section);

#line  676 "VBNET.ATG" 
			attributes.Add(section); 
		}
		Identifier();

#line  679 "VBNET.ATG" 
		f = new FieldDeclaration(attributes);
		varDecl = new VariableDeclaration(t.val);
		f.Fields.Add(varDecl);
		f.StartLocation = t.Location;
		
		if (la.kind == 11) {
			lexer.NextToken();
			Expr(
#line  684 "VBNET.ATG" 
out expr);

#line  684 "VBNET.ATG" 
			varDecl.Initializer = expr; 
		}
		Expect(1);
	}

	void InterfaceMemberDecl() {

#line  587 "VBNET.ATG" 
		TypeReference type =null;
		List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
		List<TemplateDefinition> templates = new List<TemplateDefinition>();
		AttributeSection section, returnTypeAttributeSection = null;
		ModifierList mod = new ModifierList();
		List<AttributeSection> attributes = new List<AttributeSection>();
		string name;
		
		if (StartOf(17)) {
			while (la.kind == 27) {
				AttributeSection(
#line  595 "VBNET.ATG" 
out section);

#line  595 "VBNET.ATG" 
				attributes.Add(section); 
			}
			while (StartOf(7)) {
				MemberModifier(
#line  598 "VBNET.ATG" 
mod);
			}
			if (la.kind == 93) {
				lexer.NextToken();

#line  601 "VBNET.ATG" 
				mod.Check(Modifiers.VBInterfaceEvents); 
				Identifier();

#line  602 "VBNET.ATG" 
				name = t.val; 
				if (la.kind == 24) {
					lexer.NextToken();
					if (StartOf(4)) {
						FormalParameterList(
#line  603 "VBNET.ATG" 
p);
					}
					Expect(25);
				}
				if (la.kind == 48) {
					lexer.NextToken();
					TypeName(
#line  604 "VBNET.ATG" 
out type);
				}
				Expect(1);

#line  607 "VBNET.ATG" 
				EventDeclaration ed = new EventDeclaration(type, mod.Modifier, p, attributes, name, null);
				compilationUnit.AddChild(ed);
				ed.EndLocation = t.EndLocation;
				
			} else if (la.kind == 167) {
				lexer.NextToken();

#line  613 "VBNET.ATG" 
				mod.Check(Modifiers.VBInterfaceMethods); 
				Identifier();

#line  614 "VBNET.ATG" 
				name = t.val; 
				TypeParameterList(
#line  615 "VBNET.ATG" 
templates);
				if (la.kind == 24) {
					lexer.NextToken();
					if (StartOf(4)) {
						FormalParameterList(
#line  616 "VBNET.ATG" 
p);
					}
					Expect(25);
				}
				Expect(1);

#line  619 "VBNET.ATG" 
				MethodDeclaration md = new MethodDeclaration(name, mod.Modifier, null, p, attributes);
				md.TypeReference = new TypeReference("", "System.Void");
				md.EndLocation = t.EndLocation;
				md.Templates = templates;
				compilationUnit.AddChild(md);
				
			} else if (la.kind == 100) {
				lexer.NextToken();

#line  627 "VBNET.ATG" 
				mod.Check(Modifiers.VBInterfaceMethods); 
				Identifier();

#line  628 "VBNET.ATG" 
				name = t.val; 
				TypeParameterList(
#line  629 "VBNET.ATG" 
templates);
				if (la.kind == 24) {
					lexer.NextToken();
					if (StartOf(4)) {
						FormalParameterList(
#line  630 "VBNET.ATG" 
p);
					}
					Expect(25);
				}
				if (la.kind == 48) {
					lexer.NextToken();
					while (la.kind == 27) {
						AttributeSection(
#line  631 "VBNET.ATG" 
out returnTypeAttributeSection);
					}
					TypeName(
#line  631 "VBNET.ATG" 
out type);
				}

#line  633 "VBNET.ATG" 
				if(type == null) {
				type = new TypeReference("System.Object");
				}
				MethodDeclaration md = new MethodDeclaration(name, mod.Modifier, type, p, attributes);
				if (returnTypeAttributeSection != null) {
					returnTypeAttributeSection.AttributeTarget = "return";
					md.Attributes.Add(returnTypeAttributeSection);
				}
				md.EndLocation = t.EndLocation;
				md.Templates = templates;
				compilationUnit.AddChild(md);
				
				Expect(1);
			} else if (la.kind == 146) {
				lexer.NextToken();

#line  648 "VBNET.ATG" 
				mod.Check(Modifiers.VBInterfaceProperties); 
				Identifier();

#line  649 "VBNET.ATG" 
				name = t.val;  
				if (la.kind == 24) {
					lexer.NextToken();
					if (StartOf(4)) {
						FormalParameterList(
#line  650 "VBNET.ATG" 
p);
					}
					Expect(25);
				}
				if (la.kind == 48) {
					lexer.NextToken();
					TypeName(
#line  651 "VBNET.ATG" 
out type);
				}

#line  653 "VBNET.ATG" 
				if(type == null) {
				type = new TypeReference("System.Object");
				}
				
				Expect(1);

#line  659 "VBNET.ATG" 
				PropertyDeclaration pd = new PropertyDeclaration(name, type, mod.Modifier, attributes);
				pd.Parameters = p;
				pd.EndLocation = t.EndLocation;
				compilationUnit.AddChild(pd);
				
			} else SynErr(227);
		} else if (StartOf(18)) {
			NonModuleDeclaration(
#line  666 "VBNET.ATG" 
mod, attributes);
		} else SynErr(228);
	}

	void Expr(
#line  1516 "VBNET.ATG" 
out Expression expr) {
		DisjunctionExpr(
#line  1518 "VBNET.ATG" 
out expr);
	}

	void ImplementsClause(
#line  1487 "VBNET.ATG" 
out List<InterfaceImplementation> baseInterfaces) {

#line  1489 "VBNET.ATG" 
		baseInterfaces = new List<InterfaceImplementation>();
		TypeReference type = null;
		string memberName = null;
		
		Expect(107);
		NonArrayTypeName(
#line  1494 "VBNET.ATG" 
out type, false);

#line  1495 "VBNET.ATG" 
		if (type != null) memberName = TypeReference.StripLastIdentifierFromType(ref type); 

#line  1496 "VBNET.ATG" 
		baseInterfaces.Add(new InterfaceImplementation(type, memberName)); 
		while (la.kind == 12) {
			lexer.NextToken();
			NonArrayTypeName(
#line  1498 "VBNET.ATG" 
out type, false);

#line  1499 "VBNET.ATG" 
			if (type != null) memberName = TypeReference.StripLastIdentifierFromType(ref type); 

#line  1500 "VBNET.ATG" 
			baseInterfaces.Add(new InterfaceImplementation(type, memberName)); 
		}
	}

	void HandlesClause(
#line  1445 "VBNET.ATG" 
out List<string> handlesClause) {

#line  1447 "VBNET.ATG" 
		handlesClause = new List<string>();
		string name;
		
		Expect(105);
		EventMemberSpecifier(
#line  1450 "VBNET.ATG" 
out name);

#line  1450 "VBNET.ATG" 
		handlesClause.Add(name); 
		while (la.kind == 12) {
			lexer.NextToken();
			EventMemberSpecifier(
#line  1451 "VBNET.ATG" 
out name);

#line  1451 "VBNET.ATG" 
			handlesClause.Add(name); 
		}
	}

	void Block(
#line  2165 "VBNET.ATG" 
out Statement stmt) {

#line  2168 "VBNET.ATG" 
		BlockStatement blockStmt = new BlockStatement();
		blockStmt.StartLocation = t.Location;
		compilationUnit.BlockStart(blockStmt);
		
		while (StartOf(19) || 
#line  2173 "VBNET.ATG" 
IsEndStmtAhead()) {
			if (
#line  2173 "VBNET.ATG" 
IsEndStmtAhead()) {
				Expect(88);
				EndOfStmt();

#line  2173 "VBNET.ATG" 
				compilationUnit.AddChild(new EndStatement()); 
			} else {
				Statement();
				EndOfStmt();
			}
		}

#line  2178 "VBNET.ATG" 
		stmt = blockStmt;
		blockStmt.EndLocation = t.EndLocation;
		compilationUnit.BlockEnd();
		
	}

	void Charset(
#line  1437 "VBNET.ATG" 
out CharsetModifier charsetModifier) {

#line  1438 "VBNET.ATG" 
		charsetModifier = CharsetModifier.None; 
		if (la.kind == 100 || la.kind == 167) {
		} else if (la.kind == 47) {
			lexer.NextToken();

#line  1439 "VBNET.ATG" 
			charsetModifier = CharsetModifier.Ansi; 
		} else if (la.kind == 50) {
			lexer.NextToken();

#line  1440 "VBNET.ATG" 
			charsetModifier = CharsetModifier.Auto; 
		} else if (la.kind == 176) {
			lexer.NextToken();

#line  1441 "VBNET.ATG" 
			charsetModifier = CharsetModifier.Unicode; 
		} else SynErr(229);
	}

	void IdentifierForFieldDeclaration() {
		switch (la.kind) {
		case 2: {
			lexer.NextToken();
			break;
		}
		case 169: {
			lexer.NextToken();
			break;
		}
		case 51: {
			lexer.NextToken();
			break;
		}
		case 70: {
			lexer.NextToken();
			break;
		}
		case 49: {
			lexer.NextToken();
			break;
		}
		case 47: {
			lexer.NextToken();
			break;
		}
		case 50: {
			lexer.NextToken();
			break;
		}
		case 144: {
			lexer.NextToken();
			break;
		}
		case 176: {
			lexer.NextToken();
			break;
		}
		case 177: {
			lexer.NextToken();
			break;
		}
		default: SynErr(230); break;
		}
	}

	void VariableDeclaratorPartAfterIdentifier(
#line  1317 "VBNET.ATG" 
List<VariableDeclaration> fieldDeclaration, string name) {

#line  1319 "VBNET.ATG" 
		Expression expr = null;
		TypeReference type = null;
		ArrayList rank = null;
		List<Expression> dimension = null;
		
		if (
#line  1324 "VBNET.ATG" 
IsSize() && !IsDims()) {
			ArrayInitializationModifier(
#line  1324 "VBNET.ATG" 
out dimension);
		}
		if (
#line  1325 "VBNET.ATG" 
IsDims()) {
			ArrayNameModifier(
#line  1325 "VBNET.ATG" 
out rank);
		}
		if (
#line  1327 "VBNET.ATG" 
IsObjectCreation()) {
			Expect(48);
			ObjectCreateExpression(
#line  1327 "VBNET.ATG" 
out expr);

#line  1329 "VBNET.ATG" 
			if (expr is ObjectCreateExpression) {
			type = ((ObjectCreateExpression)expr).CreateType;
			} else {
				type = ((ArrayCreateExpression)expr).CreateType;
			}
			
		} else if (StartOf(20)) {
			if (la.kind == 48) {
				lexer.NextToken();
				TypeName(
#line  1336 "VBNET.ATG" 
out type);

#line  1338 "VBNET.ATG" 
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

#line  1350 "VBNET.ATG" 
			if (type == null && (dimension != null || rank != null)) {
			type = new TypeReference("");
			}
			if (dimension != null) {
				if(type.RankSpecifier != null) {
					Error("array rank only allowed one time");
				} else {
					for (int i = 0; i < dimension.Count; i++)
						dimension[i] = Expression.AddInteger(dimension[i], 1);
					if (rank == null) {
						type.RankSpecifier = new int[] { dimension.Count - 1 };
					} else {
						rank.Insert(0, dimension.Count - 1);
						type.RankSpecifier = (int[])rank.ToArray(typeof(int));
					}
					expr = new ArrayCreateExpression(type, dimension);
				}
			} else if (rank != null) {
				if(type.RankSpecifier != null) {
					Error("array rank only allowed one time");
				} else {
					type.RankSpecifier = (int[])rank.ToArray(typeof(int));
				}
			}
			
			if (la.kind == 11) {
				lexer.NextToken();
				VariableInitializer(
#line  1375 "VBNET.ATG" 
out expr);
			}
		} else SynErr(231);

#line  1377 "VBNET.ATG" 
		fieldDeclaration.Add(new VariableDeclaration(name, expr, type)); 
	}

	void VariableDeclarator(
#line  1311 "VBNET.ATG" 
List<VariableDeclaration> fieldDeclaration) {
		Identifier();

#line  1313 "VBNET.ATG" 
		string name = t.val; 
		VariableDeclaratorPartAfterIdentifier(
#line  1314 "VBNET.ATG" 
fieldDeclaration, name);
	}

	void ConstantDeclarator(
#line  1294 "VBNET.ATG" 
List<VariableDeclaration> constantDeclaration) {

#line  1296 "VBNET.ATG" 
		Expression expr = null;
		TypeReference type = null;
		string name = String.Empty;
		
		Identifier();

#line  1300 "VBNET.ATG" 
		name = t.val; 
		if (la.kind == 48) {
			lexer.NextToken();
			TypeName(
#line  1301 "VBNET.ATG" 
out type);
		}
		Expect(11);
		Expr(
#line  1302 "VBNET.ATG" 
out expr);

#line  1304 "VBNET.ATG" 
		VariableDeclaration f = new VariableDeclaration(name, expr);
		f.TypeReference = type;
		constantDeclaration.Add(f);
		
	}

	void AccessorDecls(
#line  1228 "VBNET.ATG" 
out PropertyGetRegion getBlock, out PropertySetRegion setBlock) {

#line  1230 "VBNET.ATG" 
		List<AttributeSection> attributes = new List<AttributeSection>();
		AttributeSection section;
		getBlock = null;
		setBlock = null; 
		
		while (la.kind == 27) {
			AttributeSection(
#line  1235 "VBNET.ATG" 
out section);

#line  1235 "VBNET.ATG" 
			attributes.Add(section); 
		}
		if (StartOf(21)) {
			GetAccessorDecl(
#line  1237 "VBNET.ATG" 
out getBlock, attributes);
			if (StartOf(22)) {

#line  1239 "VBNET.ATG" 
				attributes = new List<AttributeSection>(); 
				while (la.kind == 27) {
					AttributeSection(
#line  1240 "VBNET.ATG" 
out section);

#line  1240 "VBNET.ATG" 
					attributes.Add(section); 
				}
				SetAccessorDecl(
#line  1241 "VBNET.ATG" 
out setBlock, attributes);
			}
		} else if (StartOf(23)) {
			SetAccessorDecl(
#line  1244 "VBNET.ATG" 
out setBlock, attributes);
			if (StartOf(24)) {

#line  1246 "VBNET.ATG" 
				attributes = new List<AttributeSection>(); 
				while (la.kind == 27) {
					AttributeSection(
#line  1247 "VBNET.ATG" 
out section);

#line  1247 "VBNET.ATG" 
					attributes.Add(section); 
				}
				GetAccessorDecl(
#line  1248 "VBNET.ATG" 
out getBlock, attributes);
			}
		} else SynErr(232);
	}

	void EventAccessorDeclaration(
#line  1191 "VBNET.ATG" 
out EventAddRemoveRegion eventAccessorDeclaration) {

#line  1193 "VBNET.ATG" 
		Statement stmt = null;
		List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		eventAccessorDeclaration = null;
		
		while (la.kind == 27) {
			AttributeSection(
#line  1199 "VBNET.ATG" 
out section);

#line  1199 "VBNET.ATG" 
			attributes.Add(section); 
		}
		if (la.kind == 42) {
			lexer.NextToken();
			if (la.kind == 24) {
				lexer.NextToken();
				if (StartOf(4)) {
					FormalParameterList(
#line  1201 "VBNET.ATG" 
p);
				}
				Expect(25);
			}
			Expect(1);
			Block(
#line  1202 "VBNET.ATG" 
out stmt);
			Expect(88);
			Expect(42);
			Expect(1);

#line  1204 "VBNET.ATG" 
			eventAccessorDeclaration = new EventAddRegion(attributes);
			eventAccessorDeclaration.Block = (BlockStatement)stmt;
			eventAccessorDeclaration.Parameters = p;
			
		} else if (la.kind == 152) {
			lexer.NextToken();
			if (la.kind == 24) {
				lexer.NextToken();
				if (StartOf(4)) {
					FormalParameterList(
#line  1209 "VBNET.ATG" 
p);
				}
				Expect(25);
			}
			Expect(1);
			Block(
#line  1210 "VBNET.ATG" 
out stmt);
			Expect(88);
			Expect(152);
			Expect(1);

#line  1212 "VBNET.ATG" 
			eventAccessorDeclaration = new EventRemoveRegion(attributes);
			eventAccessorDeclaration.Block = (BlockStatement)stmt;
			eventAccessorDeclaration.Parameters = p;
			
		} else if (la.kind == 149) {
			lexer.NextToken();
			if (la.kind == 24) {
				lexer.NextToken();
				if (StartOf(4)) {
					FormalParameterList(
#line  1217 "VBNET.ATG" 
p);
				}
				Expect(25);
			}
			Expect(1);
			Block(
#line  1218 "VBNET.ATG" 
out stmt);
			Expect(88);
			Expect(149);
			Expect(1);

#line  1220 "VBNET.ATG" 
			eventAccessorDeclaration = new EventRaiseRegion(attributes);
			eventAccessorDeclaration.Block = (BlockStatement)stmt;
			eventAccessorDeclaration.Parameters = p;
			
		} else SynErr(233);
	}

	void OverloadableOperator(
#line  1133 "VBNET.ATG" 
out OverloadableOperatorType operatorType) {

#line  1134 "VBNET.ATG" 
		operatorType = OverloadableOperatorType.None; 
		switch (la.kind) {
		case 14: {
			lexer.NextToken();

#line  1136 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Add; 
			break;
		}
		case 15: {
			lexer.NextToken();

#line  1138 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Subtract; 
			break;
		}
		case 16: {
			lexer.NextToken();

#line  1140 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Multiply; 
			break;
		}
		case 17: {
			lexer.NextToken();

#line  1142 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Divide; 
			break;
		}
		case 18: {
			lexer.NextToken();

#line  1144 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.DivideInteger; 
			break;
		}
		case 19: {
			lexer.NextToken();

#line  1146 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Concat; 
			break;
		}
		case 116: {
			lexer.NextToken();

#line  1148 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Like; 
			break;
		}
		case 120: {
			lexer.NextToken();

#line  1150 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Modulus; 
			break;
		}
		case 45: {
			lexer.NextToken();

#line  1152 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.BitwiseAnd; 
			break;
		}
		case 138: {
			lexer.NextToken();

#line  1154 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.BitwiseOr; 
			break;
		}
		case 185: {
			lexer.NextToken();

#line  1156 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.ExclusiveOr; 
			break;
		}
		case 20: {
			lexer.NextToken();

#line  1158 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Power; 
			break;
		}
		case 31: {
			lexer.NextToken();

#line  1160 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.ShiftLeft; 
			break;
		}
		case 32: {
			lexer.NextToken();

#line  1162 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.ShiftRight; 
			break;
		}
		case 11: {
			lexer.NextToken();

#line  1164 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Equality; 
			break;
		}
		case 28: {
			lexer.NextToken();

#line  1166 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.InEquality; 
			break;
		}
		case 27: {
			lexer.NextToken();

#line  1168 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.LessThan; 
			break;
		}
		case 30: {
			lexer.NextToken();

#line  1170 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.LessThanOrEqual; 
			break;
		}
		case 26: {
			lexer.NextToken();

#line  1172 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.GreaterThan; 
			break;
		}
		case 29: {
			lexer.NextToken();

#line  1174 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.GreaterThanOrEqual; 
			break;
		}
		case 75: {
			lexer.NextToken();

#line  1176 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.CType; 
			break;
		}
		case 2: case 47: case 49: case 50: case 51: case 70: case 144: case 169: case 176: case 177: case 204: {
			Identifier();

#line  1180 "VBNET.ATG" 
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
		default: SynErr(234); break;
		}
	}

	void GetAccessorDecl(
#line  1254 "VBNET.ATG" 
out PropertyGetRegion getBlock, List<AttributeSection> attributes) {

#line  1255 "VBNET.ATG" 
		Statement stmt = null; Modifiers m; 
		PropertyAccessorAccessModifier(
#line  1257 "VBNET.ATG" 
out m);
		Expect(101);

#line  1259 "VBNET.ATG" 
		Location startLocation = t.Location; 
		Expect(1);
		Block(
#line  1261 "VBNET.ATG" 
out stmt);

#line  1262 "VBNET.ATG" 
		getBlock = new PropertyGetRegion((BlockStatement)stmt, attributes); 
		Expect(88);
		Expect(101);

#line  1264 "VBNET.ATG" 
		getBlock.Modifier = m; 

#line  1265 "VBNET.ATG" 
		getBlock.StartLocation = startLocation; getBlock.EndLocation = t.EndLocation; 
		Expect(1);
	}

	void SetAccessorDecl(
#line  1270 "VBNET.ATG" 
out PropertySetRegion setBlock, List<AttributeSection> attributes) {

#line  1272 "VBNET.ATG" 
		Statement stmt = null;
		List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
		Modifiers m;
		
		PropertyAccessorAccessModifier(
#line  1277 "VBNET.ATG" 
out m);
		Expect(156);

#line  1279 "VBNET.ATG" 
		Location startLocation = t.Location; 
		if (la.kind == 24) {
			lexer.NextToken();
			if (StartOf(4)) {
				FormalParameterList(
#line  1280 "VBNET.ATG" 
p);
			}
			Expect(25);
		}
		Expect(1);
		Block(
#line  1282 "VBNET.ATG" 
out stmt);

#line  1284 "VBNET.ATG" 
		setBlock = new PropertySetRegion((BlockStatement)stmt, attributes);
		setBlock.Modifier = m;
		setBlock.Parameters = p;
		
		Expect(88);
		Expect(156);

#line  1289 "VBNET.ATG" 
		setBlock.StartLocation = startLocation; setBlock.EndLocation = t.EndLocation; 
		Expect(1);
	}

	void PropertyAccessorAccessModifier(
#line  2831 "VBNET.ATG" 
out Modifiers m) {

#line  2832 "VBNET.ATG" 
		m = Modifiers.None; 
		while (StartOf(25)) {
			if (la.kind == 148) {
				lexer.NextToken();

#line  2834 "VBNET.ATG" 
				m |= Modifiers.Public; 
			} else if (la.kind == 147) {
				lexer.NextToken();

#line  2835 "VBNET.ATG" 
				m |= Modifiers.Protected; 
			} else if (la.kind == 99) {
				lexer.NextToken();

#line  2836 "VBNET.ATG" 
				m |= Modifiers.Internal; 
			} else {
				lexer.NextToken();

#line  2837 "VBNET.ATG" 
				m |= Modifiers.Private; 
			}
		}
	}

	void ArrayInitializationModifier(
#line  1381 "VBNET.ATG" 
out List<Expression> arrayModifiers) {

#line  1383 "VBNET.ATG" 
		arrayModifiers = null;
		
		Expect(24);
		InitializationRankList(
#line  1385 "VBNET.ATG" 
out arrayModifiers);
		Expect(25);
	}

	void ArrayNameModifier(
#line  1957 "VBNET.ATG" 
out ArrayList arrayModifiers) {

#line  1959 "VBNET.ATG" 
		arrayModifiers = null;
		
		ArrayTypeModifiers(
#line  1961 "VBNET.ATG" 
out arrayModifiers);
	}

	void ObjectCreateExpression(
#line  1810 "VBNET.ATG" 
out Expression oce) {

#line  1812 "VBNET.ATG" 
		TypeReference type = null;
		Expression initializer = null;
		List<Expression> arguments = null;
		ArrayList dimensions = null;
		oce = null;
		bool canBeNormal; bool canBeReDim;
		
		Expect(127);
		NonArrayTypeName(
#line  1819 "VBNET.ATG" 
out type, false);
		if (la.kind == 24) {
			lexer.NextToken();
			NormalOrReDimArgumentList(
#line  1820 "VBNET.ATG" 
out arguments, out canBeNormal, out canBeReDim);
			Expect(25);
			if (la.kind == 22 || 
#line  1821 "VBNET.ATG" 
la.kind == Tokens.OpenParenthesis) {
				if (
#line  1821 "VBNET.ATG" 
la.kind == Tokens.OpenParenthesis) {
					ArrayTypeModifiers(
#line  1822 "VBNET.ATG" 
out dimensions);
					ArrayInitializer(
#line  1823 "VBNET.ATG" 
out initializer);
				} else {
					ArrayInitializer(
#line  1824 "VBNET.ATG" 
out initializer);
				}
			}

#line  1826 "VBNET.ATG" 
			if (canBeReDim && !canBeNormal && initializer == null) initializer = new ArrayInitializerExpression(); 
		}

#line  1829 "VBNET.ATG" 
		if (type == null) type = new TypeReference("Object"); // fallback type on parser errors
		if (initializer == null) {
			oce = new ObjectCreateExpression(type, arguments);
		} else {
			if (dimensions == null) dimensions = new ArrayList();
			dimensions.Insert(0, (arguments == null) ? 0 : Math.Max(arguments.Count - 1, 0));
			type.RankSpecifier = (int[])dimensions.ToArray(typeof(int));
			ArrayCreateExpression ace = new ArrayCreateExpression(type, initializer as ArrayInitializerExpression);
			ace.Arguments = arguments;
			oce = ace;
		}
		
	}

	void VariableInitializer(
#line  1409 "VBNET.ATG" 
out Expression initializerExpression) {

#line  1411 "VBNET.ATG" 
		initializerExpression = null;
		
		if (StartOf(26)) {
			Expr(
#line  1413 "VBNET.ATG" 
out initializerExpression);
		} else if (la.kind == 22) {
			ArrayInitializer(
#line  1414 "VBNET.ATG" 
out initializerExpression);
		} else SynErr(235);
	}

	void InitializationRankList(
#line  1389 "VBNET.ATG" 
out List<Expression> rank) {

#line  1391 "VBNET.ATG" 
		rank = new List<Expression>();
		Expression expr = null;
		
		Expr(
#line  1394 "VBNET.ATG" 
out expr);
		if (la.kind == 172) {
			lexer.NextToken();

#line  1395 "VBNET.ATG" 
			EnsureIsZero(expr); 
			Expr(
#line  1396 "VBNET.ATG" 
out expr);
		}

#line  1398 "VBNET.ATG" 
		if (expr != null) { rank.Add(expr); } 
		while (la.kind == 12) {
			lexer.NextToken();
			Expr(
#line  1400 "VBNET.ATG" 
out expr);
			if (la.kind == 172) {
				lexer.NextToken();

#line  1401 "VBNET.ATG" 
				EnsureIsZero(expr); 
				Expr(
#line  1402 "VBNET.ATG" 
out expr);
			}

#line  1404 "VBNET.ATG" 
			if (expr != null) { rank.Add(expr); } 
		}
	}

	void ArrayInitializer(
#line  1418 "VBNET.ATG" 
out Expression outExpr) {

#line  1420 "VBNET.ATG" 
		Expression expr = null;
		ArrayInitializerExpression initializer = new ArrayInitializerExpression();
		
		Expect(22);
		if (StartOf(27)) {
			VariableInitializer(
#line  1425 "VBNET.ATG" 
out expr);

#line  1427 "VBNET.ATG" 
			if (expr != null) { initializer.CreateExpressions.Add(expr); }
			
			while (
#line  1430 "VBNET.ATG" 
NotFinalComma()) {
				Expect(12);
				VariableInitializer(
#line  1430 "VBNET.ATG" 
out expr);

#line  1431 "VBNET.ATG" 
				if (expr != null) { initializer.CreateExpressions.Add(expr); } 
			}
		}
		Expect(23);

#line  1434 "VBNET.ATG" 
		outExpr = initializer; 
	}

	void EventMemberSpecifier(
#line  1504 "VBNET.ATG" 
out string name) {

#line  1505 "VBNET.ATG" 
		string type; name = String.Empty; 
		if (StartOf(12)) {
			Identifier();

#line  1506 "VBNET.ATG" 
			type = t.val; 
			Expect(10);
			Identifier();

#line  1508 "VBNET.ATG" 
			name = type + "." + t.val; 
		} else if (la.kind == 124) {
			lexer.NextToken();
			Expect(10);
			if (StartOf(12)) {
				Identifier();

#line  1511 "VBNET.ATG" 
				name = "MyBase." + t.val; 
			} else if (la.kind == 92) {
				lexer.NextToken();

#line  1512 "VBNET.ATG" 
				name = "MyBase.Error"; 
			} else SynErr(236);
		} else SynErr(237);
	}

	void DisjunctionExpr(
#line  1654 "VBNET.ATG" 
out Expression outExpr) {

#line  1656 "VBNET.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		ConjunctionExpr(
#line  1659 "VBNET.ATG" 
out outExpr);
		while (la.kind == 138 || la.kind == 139 || la.kind == 185) {
			if (la.kind == 138) {
				lexer.NextToken();

#line  1662 "VBNET.ATG" 
				op = BinaryOperatorType.BitwiseOr; 
			} else if (la.kind == 139) {
				lexer.NextToken();

#line  1663 "VBNET.ATG" 
				op = BinaryOperatorType.LogicalOr; 
			} else {
				lexer.NextToken();

#line  1664 "VBNET.ATG" 
				op = BinaryOperatorType.ExclusiveOr; 
			}
			ConjunctionExpr(
#line  1666 "VBNET.ATG" 
out expr);

#line  1666 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr);  
		}
	}

	void AssignmentOperator(
#line  1521 "VBNET.ATG" 
out AssignmentOperatorType op) {

#line  1522 "VBNET.ATG" 
		op = AssignmentOperatorType.None; 
		switch (la.kind) {
		case 11: {
			lexer.NextToken();

#line  1523 "VBNET.ATG" 
			op = AssignmentOperatorType.Assign; 
			break;
		}
		case 41: {
			lexer.NextToken();

#line  1524 "VBNET.ATG" 
			op = AssignmentOperatorType.ConcatString; 
			break;
		}
		case 33: {
			lexer.NextToken();

#line  1525 "VBNET.ATG" 
			op = AssignmentOperatorType.Add; 
			break;
		}
		case 35: {
			lexer.NextToken();

#line  1526 "VBNET.ATG" 
			op = AssignmentOperatorType.Subtract; 
			break;
		}
		case 36: {
			lexer.NextToken();

#line  1527 "VBNET.ATG" 
			op = AssignmentOperatorType.Multiply; 
			break;
		}
		case 37: {
			lexer.NextToken();

#line  1528 "VBNET.ATG" 
			op = AssignmentOperatorType.Divide; 
			break;
		}
		case 38: {
			lexer.NextToken();

#line  1529 "VBNET.ATG" 
			op = AssignmentOperatorType.DivideInteger; 
			break;
		}
		case 34: {
			lexer.NextToken();

#line  1530 "VBNET.ATG" 
			op = AssignmentOperatorType.Power; 
			break;
		}
		case 39: {
			lexer.NextToken();

#line  1531 "VBNET.ATG" 
			op = AssignmentOperatorType.ShiftLeft; 
			break;
		}
		case 40: {
			lexer.NextToken();

#line  1532 "VBNET.ATG" 
			op = AssignmentOperatorType.ShiftRight; 
			break;
		}
		default: SynErr(238); break;
		}
	}

	void SimpleExpr(
#line  1536 "VBNET.ATG" 
out Expression pexpr) {
		SimpleNonInvocationExpression(
#line  1538 "VBNET.ATG" 
out pexpr);
		while (la.kind == 10 || la.kind == 24) {
			if (la.kind == 10) {

#line  1539 "VBNET.ATG" 
				string name; 
				lexer.NextToken();
				IdentifierOrKeyword(
#line  1540 "VBNET.ATG" 
out name);

#line  1540 "VBNET.ATG" 
				pexpr = new FieldReferenceExpression(pexpr, name); 
			} else {
				InvocationExpression(
#line  1541 "VBNET.ATG" 
ref pexpr);
			}
		}
	}

	void SimpleNonInvocationExpression(
#line  1545 "VBNET.ATG" 
out Expression pexpr) {

#line  1547 "VBNET.ATG" 
		Expression expr;
		TypeReference type = null;
		string name = String.Empty;
		pexpr = null;
		
		if (StartOf(28)) {
			switch (la.kind) {
			case 3: {
				lexer.NextToken();

#line  1555 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(t.literalValue, t.val);  
				break;
			}
			case 4: {
				lexer.NextToken();

#line  1556 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(t.literalValue, t.val);  
				break;
			}
			case 7: {
				lexer.NextToken();

#line  1557 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(t.literalValue, t.val);  
				break;
			}
			case 6: {
				lexer.NextToken();

#line  1558 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(t.literalValue, t.val);  
				break;
			}
			case 5: {
				lexer.NextToken();

#line  1559 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(t.literalValue, t.val);  
				break;
			}
			case 9: {
				lexer.NextToken();

#line  1560 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(t.literalValue, t.val);  
				break;
			}
			case 8: {
				lexer.NextToken();

#line  1561 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(t.literalValue, t.val);  
				break;
			}
			case 173: {
				lexer.NextToken();

#line  1563 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(true, "true");  
				break;
			}
			case 96: {
				lexer.NextToken();

#line  1564 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(false, "false"); 
				break;
			}
			case 130: {
				lexer.NextToken();

#line  1565 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(null, "null");  
				break;
			}
			case 24: {
				lexer.NextToken();
				Expr(
#line  1566 "VBNET.ATG" 
out expr);
				Expect(25);

#line  1566 "VBNET.ATG" 
				pexpr = new ParenthesizedExpression(expr); 
				break;
			}
			case 2: case 47: case 49: case 50: case 51: case 70: case 144: case 169: case 176: case 177: case 204: {
				Identifier();

#line  1567 "VBNET.ATG" 
				pexpr = new IdentifierExpression(t.val); 
				break;
			}
			case 52: case 54: case 65: case 76: case 77: case 84: case 111: case 117: case 133: case 159: case 160: case 165: case 190: case 191: case 192: case 193: {

#line  1568 "VBNET.ATG" 
				string val = String.Empty; 
				if (StartOf(9)) {
					PrimitiveTypeName(
#line  1569 "VBNET.ATG" 
out val);
				} else if (la.kind == 133) {
					lexer.NextToken();

#line  1569 "VBNET.ATG" 
					val = "Object"; 
				} else SynErr(239);
				Expect(10);

#line  1570 "VBNET.ATG" 
				t.val = ""; 
				Identifier();

#line  1570 "VBNET.ATG" 
				pexpr = new FieldReferenceExpression(new TypeReferenceExpression(val), t.val); 
				break;
			}
			case 119: {
				lexer.NextToken();

#line  1571 "VBNET.ATG" 
				pexpr = new ThisReferenceExpression(); 
				break;
			}
			case 124: case 125: {

#line  1572 "VBNET.ATG" 
				Expression retExpr = null; 
				if (la.kind == 124) {
					lexer.NextToken();

#line  1573 "VBNET.ATG" 
					retExpr = new BaseReferenceExpression(); 
				} else if (la.kind == 125) {
					lexer.NextToken();

#line  1574 "VBNET.ATG" 
					retExpr = new ClassReferenceExpression(); 
				} else SynErr(240);
				Expect(10);
				IdentifierOrKeyword(
#line  1576 "VBNET.ATG" 
out name);

#line  1576 "VBNET.ATG" 
				pexpr = new FieldReferenceExpression(retExpr, name); 
				break;
			}
			case 198: {
				lexer.NextToken();
				Expect(10);
				Identifier();

#line  1578 "VBNET.ATG" 
				type = new TypeReference(t.val ?? ""); 

#line  1580 "VBNET.ATG" 
				type.IsGlobal = true; 

#line  1581 "VBNET.ATG" 
				pexpr = new TypeReferenceExpression(type); 
				break;
			}
			case 127: {
				ObjectCreateExpression(
#line  1582 "VBNET.ATG" 
out expr);

#line  1582 "VBNET.ATG" 
				pexpr = expr; 
				break;
			}
			case 75: case 82: case 199: {

#line  1584 "VBNET.ATG" 
				CastType castType = CastType.Cast; 
				if (la.kind == 82) {
					lexer.NextToken();
				} else if (la.kind == 75) {
					lexer.NextToken();

#line  1586 "VBNET.ATG" 
					castType = CastType.Conversion; 
				} else if (la.kind == 199) {
					lexer.NextToken();

#line  1587 "VBNET.ATG" 
					castType = CastType.TryCast; 
				} else SynErr(241);
				Expect(24);
				Expr(
#line  1589 "VBNET.ATG" 
out expr);
				Expect(12);
				TypeName(
#line  1589 "VBNET.ATG" 
out type);
				Expect(25);

#line  1590 "VBNET.ATG" 
				pexpr = new CastExpression(type, expr, castType); 
				break;
			}
			case 59: case 60: case 61: case 62: case 63: case 64: case 66: case 68: case 69: case 72: case 73: case 74: case 194: case 195: case 196: case 197: {
				CastTarget(
#line  1591 "VBNET.ATG" 
out type);
				Expect(24);
				Expr(
#line  1591 "VBNET.ATG" 
out expr);
				Expect(25);

#line  1591 "VBNET.ATG" 
				pexpr = new CastExpression(type, expr, CastType.PrimitiveConversion); 
				break;
			}
			case 43: {
				lexer.NextToken();
				Expr(
#line  1592 "VBNET.ATG" 
out expr);

#line  1592 "VBNET.ATG" 
				pexpr = new AddressOfExpression(expr); 
				break;
			}
			case 102: {
				lexer.NextToken();
				Expect(24);
				GetTypeTypeName(
#line  1593 "VBNET.ATG" 
out type);
				Expect(25);

#line  1593 "VBNET.ATG" 
				pexpr = new TypeOfExpression(type); 
				break;
			}
			case 175: {
				lexer.NextToken();
				SimpleExpr(
#line  1594 "VBNET.ATG" 
out expr);
				Expect(113);
				TypeName(
#line  1594 "VBNET.ATG" 
out type);

#line  1594 "VBNET.ATG" 
				pexpr = new TypeOfIsExpression(expr, type); 
				break;
			}
			}
		} else if (la.kind == 10) {
			lexer.NextToken();
			IdentifierOrKeyword(
#line  1598 "VBNET.ATG" 
out name);

#line  1598 "VBNET.ATG" 
			pexpr = new FieldReferenceExpression(null, name);
		} else SynErr(242);
	}

	void IdentifierOrKeyword(
#line  2765 "VBNET.ATG" 
out string name) {

#line  2767 "VBNET.ATG" 
		lexer.NextToken(); name = t.val;  
	}

	void InvocationExpression(
#line  1602 "VBNET.ATG" 
ref Expression pexpr) {

#line  1603 "VBNET.ATG" 
		List<TypeReference> typeParameters = new List<TypeReference>();
		List<Expression> parameters = null;
		TypeReference type; 
		Expect(24);

#line  1607 "VBNET.ATG" 
		Location start = t.Location; 
		if (la.kind == 200) {
			lexer.NextToken();
			TypeName(
#line  1609 "VBNET.ATG" 
out type);

#line  1609 "VBNET.ATG" 
			if (type != null) typeParameters.Add(type); 
			while (la.kind == 12) {
				lexer.NextToken();
				TypeName(
#line  1612 "VBNET.ATG" 
out type);

#line  1612 "VBNET.ATG" 
				if (type != null) typeParameters.Add(type); 
			}
			Expect(25);
			if (la.kind == 10) {
				lexer.NextToken();
				Identifier();

#line  1617 "VBNET.ATG" 
				pexpr = new FieldReferenceExpression(GetTypeReferenceExpression(pexpr, typeParameters), t.val); 
			} else if (la.kind == 24) {
				lexer.NextToken();
				ArgumentList(
#line  1619 "VBNET.ATG" 
out parameters);
				Expect(25);

#line  1621 "VBNET.ATG" 
				pexpr = new InvocationExpression(pexpr, parameters, typeParameters); 
			} else SynErr(243);
		} else if (StartOf(29)) {
			ArgumentList(
#line  1623 "VBNET.ATG" 
out parameters);
			Expect(25);

#line  1625 "VBNET.ATG" 
			pexpr = new InvocationExpression(pexpr, parameters, typeParameters); 
		} else SynErr(244);

#line  1627 "VBNET.ATG" 
		pexpr.StartLocation = start; pexpr.EndLocation = t.Location; 
	}

	void PrimitiveTypeName(
#line  2772 "VBNET.ATG" 
out string type) {

#line  2773 "VBNET.ATG" 
		type = String.Empty; 
		switch (la.kind) {
		case 52: {
			lexer.NextToken();

#line  2774 "VBNET.ATG" 
			type = "Boolean"; 
			break;
		}
		case 76: {
			lexer.NextToken();

#line  2775 "VBNET.ATG" 
			type = "Date"; 
			break;
		}
		case 65: {
			lexer.NextToken();

#line  2776 "VBNET.ATG" 
			type = "Char"; 
			break;
		}
		case 165: {
			lexer.NextToken();

#line  2777 "VBNET.ATG" 
			type = "String"; 
			break;
		}
		case 77: {
			lexer.NextToken();

#line  2778 "VBNET.ATG" 
			type = "Decimal"; 
			break;
		}
		case 54: {
			lexer.NextToken();

#line  2779 "VBNET.ATG" 
			type = "Byte"; 
			break;
		}
		case 159: {
			lexer.NextToken();

#line  2780 "VBNET.ATG" 
			type = "Short"; 
			break;
		}
		case 111: {
			lexer.NextToken();

#line  2781 "VBNET.ATG" 
			type = "Integer"; 
			break;
		}
		case 117: {
			lexer.NextToken();

#line  2782 "VBNET.ATG" 
			type = "Long"; 
			break;
		}
		case 160: {
			lexer.NextToken();

#line  2783 "VBNET.ATG" 
			type = "Single"; 
			break;
		}
		case 84: {
			lexer.NextToken();

#line  2784 "VBNET.ATG" 
			type = "Double"; 
			break;
		}
		case 191: {
			lexer.NextToken();

#line  2785 "VBNET.ATG" 
			type = "UInteger"; 
			break;
		}
		case 192: {
			lexer.NextToken();

#line  2786 "VBNET.ATG" 
			type = "ULong"; 
			break;
		}
		case 193: {
			lexer.NextToken();

#line  2787 "VBNET.ATG" 
			type = "UShort"; 
			break;
		}
		case 190: {
			lexer.NextToken();

#line  2788 "VBNET.ATG" 
			type = "SByte"; 
			break;
		}
		default: SynErr(245); break;
		}
	}

	void CastTarget(
#line  1632 "VBNET.ATG" 
out TypeReference type) {

#line  1634 "VBNET.ATG" 
		type = null;
		
		switch (la.kind) {
		case 59: {
			lexer.NextToken();

#line  1636 "VBNET.ATG" 
			type = new TypeReference("System.Boolean"); 
			break;
		}
		case 60: {
			lexer.NextToken();

#line  1637 "VBNET.ATG" 
			type = new TypeReference("System.Byte"); 
			break;
		}
		case 194: {
			lexer.NextToken();

#line  1638 "VBNET.ATG" 
			type = new TypeReference("System.SByte"); 
			break;
		}
		case 61: {
			lexer.NextToken();

#line  1639 "VBNET.ATG" 
			type = new TypeReference("System.Char"); 
			break;
		}
		case 62: {
			lexer.NextToken();

#line  1640 "VBNET.ATG" 
			type = new TypeReference("System.DateTime"); 
			break;
		}
		case 64: {
			lexer.NextToken();

#line  1641 "VBNET.ATG" 
			type = new TypeReference("System.Decimal"); 
			break;
		}
		case 63: {
			lexer.NextToken();

#line  1642 "VBNET.ATG" 
			type = new TypeReference("System.Double"); 
			break;
		}
		case 72: {
			lexer.NextToken();

#line  1643 "VBNET.ATG" 
			type = new TypeReference("System.Int16"); 
			break;
		}
		case 66: {
			lexer.NextToken();

#line  1644 "VBNET.ATG" 
			type = new TypeReference("System.Int32"); 
			break;
		}
		case 68: {
			lexer.NextToken();

#line  1645 "VBNET.ATG" 
			type = new TypeReference("System.Int64"); 
			break;
		}
		case 195: {
			lexer.NextToken();

#line  1646 "VBNET.ATG" 
			type = new TypeReference("System.UInt16"); 
			break;
		}
		case 196: {
			lexer.NextToken();

#line  1647 "VBNET.ATG" 
			type = new TypeReference("System.UInt32"); 
			break;
		}
		case 197: {
			lexer.NextToken();

#line  1648 "VBNET.ATG" 
			type = new TypeReference("System.UInt64"); 
			break;
		}
		case 69: {
			lexer.NextToken();

#line  1649 "VBNET.ATG" 
			type = new TypeReference("System.Object"); 
			break;
		}
		case 73: {
			lexer.NextToken();

#line  1650 "VBNET.ATG" 
			type = new TypeReference("System.Single"); 
			break;
		}
		case 74: {
			lexer.NextToken();

#line  1651 "VBNET.ATG" 
			type = new TypeReference("System.String"); 
			break;
		}
		default: SynErr(246); break;
		}
	}

	void GetTypeTypeName(
#line  1909 "VBNET.ATG" 
out TypeReference typeref) {

#line  1910 "VBNET.ATG" 
		ArrayList rank = null; 
		NonArrayTypeName(
#line  1912 "VBNET.ATG" 
out typeref, true);
		ArrayTypeModifiers(
#line  1913 "VBNET.ATG" 
out rank);

#line  1914 "VBNET.ATG" 
		if (rank != null && typeref != null) {
		typeref.RankSpecifier = (int[])rank.ToArray(typeof(int));
		}
		
	}

	void ArgumentList(
#line  1844 "VBNET.ATG" 
out List<Expression> arguments) {

#line  1846 "VBNET.ATG" 
		arguments = new List<Expression>();
		Expression expr = null;
		
		if (StartOf(26)) {
			Argument(
#line  1849 "VBNET.ATG" 
out expr);
		}
		while (la.kind == 12) {
			lexer.NextToken();

#line  1850 "VBNET.ATG" 
			arguments.Add(expr ?? Expression.Null); expr = null; 
			if (StartOf(26)) {
				Argument(
#line  1851 "VBNET.ATG" 
out expr);
			}

#line  1852 "VBNET.ATG" 
			if (expr == null) expr = Expression.Null; 
		}

#line  1854 "VBNET.ATG" 
		if (expr != null) arguments.Add(expr); 
	}

	void ConjunctionExpr(
#line  1670 "VBNET.ATG" 
out Expression outExpr) {

#line  1672 "VBNET.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		NotExpr(
#line  1675 "VBNET.ATG" 
out outExpr);
		while (la.kind == 45 || la.kind == 46) {
			if (la.kind == 45) {
				lexer.NextToken();

#line  1678 "VBNET.ATG" 
				op = BinaryOperatorType.BitwiseAnd; 
			} else {
				lexer.NextToken();

#line  1679 "VBNET.ATG" 
				op = BinaryOperatorType.LogicalAnd; 
			}
			NotExpr(
#line  1681 "VBNET.ATG" 
out expr);

#line  1681 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr);  
		}
	}

	void NotExpr(
#line  1685 "VBNET.ATG" 
out Expression outExpr) {

#line  1686 "VBNET.ATG" 
		UnaryOperatorType uop = UnaryOperatorType.None; 
		while (la.kind == 129) {
			lexer.NextToken();

#line  1687 "VBNET.ATG" 
			uop = UnaryOperatorType.Not; 
		}
		ComparisonExpr(
#line  1688 "VBNET.ATG" 
out outExpr);

#line  1689 "VBNET.ATG" 
		if (uop != UnaryOperatorType.None)
		outExpr = new UnaryOperatorExpression(outExpr, uop);
		
	}

	void ComparisonExpr(
#line  1694 "VBNET.ATG" 
out Expression outExpr) {

#line  1696 "VBNET.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		ShiftExpr(
#line  1699 "VBNET.ATG" 
out outExpr);
		while (StartOf(30)) {
			switch (la.kind) {
			case 27: {
				lexer.NextToken();

#line  1702 "VBNET.ATG" 
				op = BinaryOperatorType.LessThan; 
				break;
			}
			case 26: {
				lexer.NextToken();

#line  1703 "VBNET.ATG" 
				op = BinaryOperatorType.GreaterThan; 
				break;
			}
			case 30: {
				lexer.NextToken();

#line  1704 "VBNET.ATG" 
				op = BinaryOperatorType.LessThanOrEqual; 
				break;
			}
			case 29: {
				lexer.NextToken();

#line  1705 "VBNET.ATG" 
				op = BinaryOperatorType.GreaterThanOrEqual; 
				break;
			}
			case 28: {
				lexer.NextToken();

#line  1706 "VBNET.ATG" 
				op = BinaryOperatorType.InEquality; 
				break;
			}
			case 11: {
				lexer.NextToken();

#line  1707 "VBNET.ATG" 
				op = BinaryOperatorType.Equality; 
				break;
			}
			case 116: {
				lexer.NextToken();

#line  1708 "VBNET.ATG" 
				op = BinaryOperatorType.Like; 
				break;
			}
			case 113: {
				lexer.NextToken();

#line  1709 "VBNET.ATG" 
				op = BinaryOperatorType.ReferenceEquality; 
				break;
			}
			case 189: {
				lexer.NextToken();

#line  1710 "VBNET.ATG" 
				op = BinaryOperatorType.ReferenceInequality; 
				break;
			}
			}
			if (StartOf(31)) {
				ShiftExpr(
#line  1713 "VBNET.ATG" 
out expr);

#line  1713 "VBNET.ATG" 
				outExpr = new BinaryOperatorExpression(outExpr, op, expr);  
			} else if (la.kind == 129) {
				lexer.NextToken();
				ShiftExpr(
#line  1716 "VBNET.ATG" 
out expr);

#line  1716 "VBNET.ATG" 
				outExpr = new BinaryOperatorExpression(outExpr, op, new UnaryOperatorExpression(expr, UnaryOperatorType.Not));  
			} else SynErr(247);
		}
	}

	void ShiftExpr(
#line  1721 "VBNET.ATG" 
out Expression outExpr) {

#line  1723 "VBNET.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		ConcatenationExpr(
#line  1726 "VBNET.ATG" 
out outExpr);
		while (la.kind == 31 || la.kind == 32) {
			if (la.kind == 31) {
				lexer.NextToken();

#line  1729 "VBNET.ATG" 
				op = BinaryOperatorType.ShiftLeft; 
			} else {
				lexer.NextToken();

#line  1730 "VBNET.ATG" 
				op = BinaryOperatorType.ShiftRight; 
			}
			ConcatenationExpr(
#line  1732 "VBNET.ATG" 
out expr);

#line  1732 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr);  
		}
	}

	void ConcatenationExpr(
#line  1736 "VBNET.ATG" 
out Expression outExpr) {

#line  1737 "VBNET.ATG" 
		Expression expr; 
		AdditiveExpr(
#line  1739 "VBNET.ATG" 
out outExpr);
		while (la.kind == 19) {
			lexer.NextToken();
			AdditiveExpr(
#line  1739 "VBNET.ATG" 
out expr);

#line  1739 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.Concat, expr);  
		}
	}

	void AdditiveExpr(
#line  1742 "VBNET.ATG" 
out Expression outExpr) {

#line  1744 "VBNET.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		ModuloExpr(
#line  1747 "VBNET.ATG" 
out outExpr);
		while (la.kind == 14 || la.kind == 15) {
			if (la.kind == 14) {
				lexer.NextToken();

#line  1750 "VBNET.ATG" 
				op = BinaryOperatorType.Add; 
			} else {
				lexer.NextToken();

#line  1751 "VBNET.ATG" 
				op = BinaryOperatorType.Subtract; 
			}
			ModuloExpr(
#line  1753 "VBNET.ATG" 
out expr);

#line  1753 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr);  
		}
	}

	void ModuloExpr(
#line  1757 "VBNET.ATG" 
out Expression outExpr) {

#line  1758 "VBNET.ATG" 
		Expression expr; 
		IntegerDivisionExpr(
#line  1760 "VBNET.ATG" 
out outExpr);
		while (la.kind == 120) {
			lexer.NextToken();
			IntegerDivisionExpr(
#line  1760 "VBNET.ATG" 
out expr);

#line  1760 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.Modulus, expr);  
		}
	}

	void IntegerDivisionExpr(
#line  1763 "VBNET.ATG" 
out Expression outExpr) {

#line  1764 "VBNET.ATG" 
		Expression expr; 
		MultiplicativeExpr(
#line  1766 "VBNET.ATG" 
out outExpr);
		while (la.kind == 18) {
			lexer.NextToken();
			MultiplicativeExpr(
#line  1766 "VBNET.ATG" 
out expr);

#line  1766 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.DivideInteger, expr);  
		}
	}

	void MultiplicativeExpr(
#line  1769 "VBNET.ATG" 
out Expression outExpr) {

#line  1771 "VBNET.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		UnaryExpr(
#line  1774 "VBNET.ATG" 
out outExpr);
		while (la.kind == 16 || la.kind == 17) {
			if (la.kind == 16) {
				lexer.NextToken();

#line  1777 "VBNET.ATG" 
				op = BinaryOperatorType.Multiply; 
			} else {
				lexer.NextToken();

#line  1778 "VBNET.ATG" 
				op = BinaryOperatorType.Divide; 
			}
			UnaryExpr(
#line  1780 "VBNET.ATG" 
out expr);

#line  1780 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr); 
		}
	}

	void UnaryExpr(
#line  1784 "VBNET.ATG" 
out Expression uExpr) {

#line  1786 "VBNET.ATG" 
		Expression expr;
		UnaryOperatorType uop = UnaryOperatorType.None;
		bool isUOp = false;
		
		while (la.kind == 14 || la.kind == 15 || la.kind == 16) {
			if (la.kind == 14) {
				lexer.NextToken();

#line  1790 "VBNET.ATG" 
				uop = UnaryOperatorType.Plus; isUOp = true; 
			} else if (la.kind == 15) {
				lexer.NextToken();

#line  1791 "VBNET.ATG" 
				uop = UnaryOperatorType.Minus; isUOp = true; 
			} else {
				lexer.NextToken();

#line  1792 "VBNET.ATG" 
				uop = UnaryOperatorType.Star;  isUOp = true;
			}
		}
		ExponentiationExpr(
#line  1794 "VBNET.ATG" 
out expr);

#line  1796 "VBNET.ATG" 
		if (isUOp) {
		uExpr = new UnaryOperatorExpression(expr, uop);
		} else {
			uExpr = expr;
		}
		
	}

	void ExponentiationExpr(
#line  1804 "VBNET.ATG" 
out Expression outExpr) {

#line  1805 "VBNET.ATG" 
		Expression expr; 
		SimpleExpr(
#line  1807 "VBNET.ATG" 
out outExpr);
		while (la.kind == 20) {
			lexer.NextToken();
			SimpleExpr(
#line  1807 "VBNET.ATG" 
out expr);

#line  1807 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.Power, expr);  
		}
	}

	void NormalOrReDimArgumentList(
#line  1858 "VBNET.ATG" 
out List<Expression> arguments, out bool canBeNormal, out bool canBeRedim) {

#line  1860 "VBNET.ATG" 
		arguments = new List<Expression>();
		canBeNormal = true; canBeRedim = !IsNamedAssign();
		Expression expr = null;
		
		if (StartOf(26)) {
			Argument(
#line  1865 "VBNET.ATG" 
out expr);
			if (la.kind == 172) {
				lexer.NextToken();

#line  1866 "VBNET.ATG" 
				EnsureIsZero(expr); canBeNormal = false; 
				Expr(
#line  1867 "VBNET.ATG" 
out expr);
			}
		}
		while (la.kind == 12) {
			lexer.NextToken();

#line  1870 "VBNET.ATG" 
			if (expr == null) canBeRedim = false; 

#line  1871 "VBNET.ATG" 
			arguments.Add(expr ?? Expression.Null); expr = null; 

#line  1872 "VBNET.ATG" 
			canBeRedim &= !IsNamedAssign(); 
			if (StartOf(26)) {
				Argument(
#line  1873 "VBNET.ATG" 
out expr);
				if (la.kind == 172) {
					lexer.NextToken();

#line  1874 "VBNET.ATG" 
					EnsureIsZero(expr); canBeNormal = false; 
					Expr(
#line  1875 "VBNET.ATG" 
out expr);
				}
			}

#line  1877 "VBNET.ATG" 
			if (expr == null) { canBeRedim = false; expr = Expression.Null; } 
		}

#line  1879 "VBNET.ATG" 
		if (expr != null) arguments.Add(expr); else canBeRedim = false; 
	}

	void ArrayTypeModifiers(
#line  1966 "VBNET.ATG" 
out ArrayList arrayModifiers) {

#line  1968 "VBNET.ATG" 
		arrayModifiers = new ArrayList();
		int i = 0;
		
		while (
#line  1971 "VBNET.ATG" 
IsDims()) {
			Expect(24);
			if (la.kind == 12 || la.kind == 25) {
				RankList(
#line  1973 "VBNET.ATG" 
out i);
			}

#line  1975 "VBNET.ATG" 
			arrayModifiers.Add(i);
			
			Expect(25);
		}

#line  1980 "VBNET.ATG" 
		if(arrayModifiers.Count == 0) {
		 arrayModifiers = null;
		}
		
	}

	void Argument(
#line  1883 "VBNET.ATG" 
out Expression argumentexpr) {

#line  1885 "VBNET.ATG" 
		Expression expr;
		argumentexpr = null;
		string name;
		
		if (
#line  1889 "VBNET.ATG" 
IsNamedAssign()) {
			Identifier();

#line  1889 "VBNET.ATG" 
			name = t.val;  
			Expect(13);
			Expect(11);
			Expr(
#line  1889 "VBNET.ATG" 
out expr);

#line  1891 "VBNET.ATG" 
			argumentexpr = new NamedArgumentExpression(name, expr);
			
		} else if (StartOf(26)) {
			Expr(
#line  1894 "VBNET.ATG" 
out argumentexpr);
		} else SynErr(248);
	}

	void QualIdentAndTypeArguments(
#line  1940 "VBNET.ATG" 
out TypeReference typeref, bool canBeUnbound) {

#line  1941 "VBNET.ATG" 
		string name; typeref = null; 
		Qualident(
#line  1943 "VBNET.ATG" 
out name);

#line  1944 "VBNET.ATG" 
		typeref = new TypeReference(name); 
		if (
#line  1945 "VBNET.ATG" 
la.kind == Tokens.OpenParenthesis && Peek(1).kind == Tokens.Of) {
			lexer.NextToken();
			Expect(200);
			if (
#line  1947 "VBNET.ATG" 
canBeUnbound && (la.kind == Tokens.CloseParenthesis || la.kind == Tokens.Comma)) {

#line  1948 "VBNET.ATG" 
				typeref.GenericTypes.Add(NullTypeReference.Instance); 
				while (la.kind == 12) {
					lexer.NextToken();

#line  1949 "VBNET.ATG" 
					typeref.GenericTypes.Add(NullTypeReference.Instance); 
				}
			} else if (StartOf(5)) {
				TypeArgumentList(
#line  1950 "VBNET.ATG" 
typeref.GenericTypes);
			} else SynErr(249);
			Expect(25);
		}
	}

	void TypeArgumentList(
#line  1993 "VBNET.ATG" 
List<TypeReference> typeArguments) {

#line  1995 "VBNET.ATG" 
		TypeReference typeref;
		
		TypeName(
#line  1997 "VBNET.ATG" 
out typeref);

#line  1997 "VBNET.ATG" 
		if (typeref != null) typeArguments.Add(typeref); 
		while (la.kind == 12) {
			lexer.NextToken();
			TypeName(
#line  2000 "VBNET.ATG" 
out typeref);

#line  2000 "VBNET.ATG" 
			if (typeref != null) typeArguments.Add(typeref); 
		}
	}

	void RankList(
#line  1987 "VBNET.ATG" 
out int i) {

#line  1988 "VBNET.ATG" 
		i = 0; 
		while (la.kind == 12) {
			lexer.NextToken();

#line  1989 "VBNET.ATG" 
			++i; 
		}
	}

	void Attribute(
#line  2025 "VBNET.ATG" 
out ASTAttribute attribute) {

#line  2026 "VBNET.ATG" 
		string name;
		List<Expression> positional = new List<Expression>();
		List<NamedArgumentExpression> named = new List<NamedArgumentExpression>();
		
		if (la.kind == 198) {
			lexer.NextToken();
			Expect(10);
		}
		Qualident(
#line  2031 "VBNET.ATG" 
out name);
		if (la.kind == 24) {
			AttributeArguments(
#line  2032 "VBNET.ATG" 
positional, named);
		}

#line  2033 "VBNET.ATG" 
		attribute  = new ASTAttribute(name, positional, named); 
	}

	void AttributeArguments(
#line  2037 "VBNET.ATG" 
List<Expression> positional, List<NamedArgumentExpression> named) {

#line  2039 "VBNET.ATG" 
		bool nameFound = false;
		string name = "";
		Expression expr;
		
		Expect(24);
		if (
#line  2045 "VBNET.ATG" 
IsNotClosingParenthesis()) {
			if (
#line  2047 "VBNET.ATG" 
IsNamedAssign()) {

#line  2047 "VBNET.ATG" 
				nameFound = true; 
				IdentifierOrKeyword(
#line  2048 "VBNET.ATG" 
out name);
				if (la.kind == 13) {
					lexer.NextToken();
				}
				Expect(11);
			}
			Expr(
#line  2050 "VBNET.ATG" 
out expr);

#line  2052 "VBNET.ATG" 
			if (expr != null) {
			if (string.IsNullOrEmpty(name)) { positional.Add(expr); }
			else { named.Add(new NamedArgumentExpression(name, expr)); name = ""; }
			}
			
			while (la.kind == 12) {
				lexer.NextToken();
				if (
#line  2060 "VBNET.ATG" 
IsNamedAssign()) {

#line  2060 "VBNET.ATG" 
					nameFound = true; 
					IdentifierOrKeyword(
#line  2061 "VBNET.ATG" 
out name);
					if (la.kind == 13) {
						lexer.NextToken();
					}
					Expect(11);
				} else if (StartOf(26)) {

#line  2063 "VBNET.ATG" 
					if (nameFound) Error("no positional argument after named argument"); 
				} else SynErr(250);
				Expr(
#line  2064 "VBNET.ATG" 
out expr);

#line  2064 "VBNET.ATG" 
				if (expr != null) { if(name == "") positional.Add(expr);
				else { named.Add(new NamedArgumentExpression(name, expr)); name = ""; }
				}
				
			}
		}
		Expect(25);
	}

	void FormalParameter(
#line  2133 "VBNET.ATG" 
out ParameterDeclarationExpression p) {

#line  2135 "VBNET.ATG" 
		TypeReference type = null;
		ParamModifierList mod = new ParamModifierList(this);
		Expression expr = null;
		p = null;ArrayList arrayModifiers = null;
		
		while (StartOf(32)) {
			ParameterModifier(
#line  2140 "VBNET.ATG" 
mod);
		}
		Identifier();

#line  2141 "VBNET.ATG" 
		string parameterName = t.val; 
		if (
#line  2142 "VBNET.ATG" 
IsDims()) {
			ArrayTypeModifiers(
#line  2142 "VBNET.ATG" 
out arrayModifiers);
		}
		if (la.kind == 48) {
			lexer.NextToken();
			TypeName(
#line  2143 "VBNET.ATG" 
out type);
		}

#line  2145 "VBNET.ATG" 
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
		
		if (la.kind == 11) {
			lexer.NextToken();
			Expr(
#line  2157 "VBNET.ATG" 
out expr);
		}

#line  2159 "VBNET.ATG" 
		mod.Check();
		p = new ParameterDeclarationExpression(type, parameterName, mod.Modifier, expr);
		
	}

	void ParameterModifier(
#line  2791 "VBNET.ATG" 
ParamModifierList m) {
		if (la.kind == 55) {
			lexer.NextToken();

#line  2792 "VBNET.ATG" 
			m.Add(ParameterModifiers.In); 
		} else if (la.kind == 53) {
			lexer.NextToken();

#line  2793 "VBNET.ATG" 
			m.Add(ParameterModifiers.Ref); 
		} else if (la.kind == 137) {
			lexer.NextToken();

#line  2794 "VBNET.ATG" 
			m.Add(ParameterModifiers.Optional); 
		} else if (la.kind == 143) {
			lexer.NextToken();

#line  2795 "VBNET.ATG" 
			m.Add(ParameterModifiers.Params); 
		} else SynErr(251);
	}

	void Statement() {

#line  2186 "VBNET.ATG" 
		Statement stmt = null;
		Location startPos = la.Location;
		string label = String.Empty;
		
		
		if (la.kind == 1 || la.kind == 13) {
		} else if (
#line  2192 "VBNET.ATG" 
IsLabel()) {
			LabelName(
#line  2192 "VBNET.ATG" 
out label);

#line  2194 "VBNET.ATG" 
			compilationUnit.AddChild(new LabelStatement(t.val));
			
			Expect(13);
			Statement();
		} else if (StartOf(33)) {
			EmbeddedStatement(
#line  2197 "VBNET.ATG" 
out stmt);

#line  2197 "VBNET.ATG" 
			compilationUnit.AddChild(stmt); 
		} else if (StartOf(34)) {
			LocalDeclarationStatement(
#line  2198 "VBNET.ATG" 
out stmt);

#line  2198 "VBNET.ATG" 
			compilationUnit.AddChild(stmt); 
		} else SynErr(252);

#line  2201 "VBNET.ATG" 
		if (stmt != null) {
		stmt.StartLocation = startPos;
		stmt.EndLocation = t.Location;
		}
		
	}

	void LabelName(
#line  2577 "VBNET.ATG" 
out string name) {

#line  2579 "VBNET.ATG" 
		name = String.Empty;
		
		if (StartOf(12)) {
			Identifier();

#line  2581 "VBNET.ATG" 
			name = t.val; 
		} else if (la.kind == 5) {
			lexer.NextToken();

#line  2582 "VBNET.ATG" 
			name = t.val; 
		} else SynErr(253);
	}

	void EmbeddedStatement(
#line  2240 "VBNET.ATG" 
out Statement statement) {

#line  2242 "VBNET.ATG" 
		Statement embeddedStatement = null;
		statement = null;
		Expression expr = null;
		string name = String.Empty;
		List<Expression> p = null;
		
		switch (la.kind) {
		case 94: {
			lexer.NextToken();

#line  2248 "VBNET.ATG" 
			ExitType exitType = ExitType.None; 
			switch (la.kind) {
			case 167: {
				lexer.NextToken();

#line  2250 "VBNET.ATG" 
				exitType = ExitType.Sub; 
				break;
			}
			case 100: {
				lexer.NextToken();

#line  2252 "VBNET.ATG" 
				exitType = ExitType.Function; 
				break;
			}
			case 146: {
				lexer.NextToken();

#line  2254 "VBNET.ATG" 
				exitType = ExitType.Property; 
				break;
			}
			case 83: {
				lexer.NextToken();

#line  2256 "VBNET.ATG" 
				exitType = ExitType.Do; 
				break;
			}
			case 98: {
				lexer.NextToken();

#line  2258 "VBNET.ATG" 
				exitType = ExitType.For; 
				break;
			}
			case 174: {
				lexer.NextToken();

#line  2260 "VBNET.ATG" 
				exitType = ExitType.Try; 
				break;
			}
			case 181: {
				lexer.NextToken();

#line  2262 "VBNET.ATG" 
				exitType = ExitType.While; 
				break;
			}
			case 155: {
				lexer.NextToken();

#line  2264 "VBNET.ATG" 
				exitType = ExitType.Select; 
				break;
			}
			default: SynErr(254); break;
			}

#line  2266 "VBNET.ATG" 
			statement = new ExitStatement(exitType); 
			break;
		}
		case 174: {
			TryStatement(
#line  2267 "VBNET.ATG" 
out statement);
			break;
		}
		case 186: {
			lexer.NextToken();

#line  2268 "VBNET.ATG" 
			ContinueType continueType = ContinueType.None; 
			if (la.kind == 83 || la.kind == 98 || la.kind == 181) {
				if (la.kind == 83) {
					lexer.NextToken();

#line  2268 "VBNET.ATG" 
					continueType = ContinueType.Do; 
				} else if (la.kind == 98) {
					lexer.NextToken();

#line  2268 "VBNET.ATG" 
					continueType = ContinueType.For; 
				} else {
					lexer.NextToken();

#line  2268 "VBNET.ATG" 
					continueType = ContinueType.While; 
				}
			}

#line  2268 "VBNET.ATG" 
			statement = new ContinueStatement(continueType); 
			break;
		}
		case 171: {
			lexer.NextToken();
			if (StartOf(26)) {
				Expr(
#line  2270 "VBNET.ATG" 
out expr);
			}

#line  2270 "VBNET.ATG" 
			statement = new ThrowStatement(expr); 
			break;
		}
		case 154: {
			lexer.NextToken();
			if (StartOf(26)) {
				Expr(
#line  2272 "VBNET.ATG" 
out expr);
			}

#line  2272 "VBNET.ATG" 
			statement = new ReturnStatement(expr); 
			break;
		}
		case 168: {
			lexer.NextToken();
			Expr(
#line  2274 "VBNET.ATG" 
out expr);
			EndOfStmt();
			Block(
#line  2274 "VBNET.ATG" 
out embeddedStatement);
			Expect(88);
			Expect(168);

#line  2275 "VBNET.ATG" 
			statement = new LockStatement(expr, embeddedStatement); 
			break;
		}
		case 149: {
			lexer.NextToken();
			Identifier();

#line  2277 "VBNET.ATG" 
			name = t.val; 
			if (la.kind == 24) {
				lexer.NextToken();
				if (StartOf(29)) {
					ArgumentList(
#line  2278 "VBNET.ATG" 
out p);
				}
				Expect(25);
			}

#line  2279 "VBNET.ATG" 
			statement = new RaiseEventStatement(name, p); 
			break;
		}
		case 182: {
			WithStatement(
#line  2281 "VBNET.ATG" 
out statement);
			break;
		}
		case 42: {
			lexer.NextToken();

#line  2283 "VBNET.ATG" 
			Expression handlerExpr = null; 
			Expr(
#line  2284 "VBNET.ATG" 
out expr);
			Expect(12);
			Expr(
#line  2284 "VBNET.ATG" 
out handlerExpr);

#line  2286 "VBNET.ATG" 
			statement = new AddHandlerStatement(expr, handlerExpr);
			
			break;
		}
		case 152: {
			lexer.NextToken();

#line  2289 "VBNET.ATG" 
			Expression handlerExpr = null; 
			Expr(
#line  2290 "VBNET.ATG" 
out expr);
			Expect(12);
			Expr(
#line  2290 "VBNET.ATG" 
out handlerExpr);

#line  2292 "VBNET.ATG" 
			statement = new RemoveHandlerStatement(expr, handlerExpr);
			
			break;
		}
		case 181: {
			lexer.NextToken();
			Expr(
#line  2295 "VBNET.ATG" 
out expr);
			EndOfStmt();
			Block(
#line  2296 "VBNET.ATG" 
out embeddedStatement);
			Expect(88);
			Expect(181);

#line  2298 "VBNET.ATG" 
			statement = new DoLoopStatement(expr, embeddedStatement, ConditionType.While, ConditionPosition.Start);
			
			break;
		}
		case 83: {
			lexer.NextToken();

#line  2303 "VBNET.ATG" 
			ConditionType conditionType = ConditionType.None;
			
			if (la.kind == 177 || la.kind == 181) {
				WhileOrUntil(
#line  2306 "VBNET.ATG" 
out conditionType);
				Expr(
#line  2306 "VBNET.ATG" 
out expr);
				EndOfStmt();
				Block(
#line  2307 "VBNET.ATG" 
out embeddedStatement);
				Expect(118);

#line  2310 "VBNET.ATG" 
				statement = new DoLoopStatement(expr, 
				                               embeddedStatement, 
				                               conditionType == ConditionType.While ? ConditionType.DoWhile : conditionType, 
				                               ConditionPosition.Start);
				
			} else if (la.kind == 1 || la.kind == 13) {
				EndOfStmt();
				Block(
#line  2317 "VBNET.ATG" 
out embeddedStatement);
				Expect(118);
				if (la.kind == 177 || la.kind == 181) {
					WhileOrUntil(
#line  2318 "VBNET.ATG" 
out conditionType);
					Expr(
#line  2318 "VBNET.ATG" 
out expr);
				}

#line  2320 "VBNET.ATG" 
				statement = new DoLoopStatement(expr, embeddedStatement, conditionType, ConditionPosition.End);
				
			} else SynErr(255);
			break;
		}
		case 98: {
			lexer.NextToken();

#line  2325 "VBNET.ATG" 
			Expression group = null;
			TypeReference typeReference;
			string        typeName;
			Location startLocation = t.Location;
			
			if (la.kind == 85) {
				lexer.NextToken();
				LoopControlVariable(
#line  2332 "VBNET.ATG" 
out typeReference, out typeName);
				Expect(109);
				Expr(
#line  2333 "VBNET.ATG" 
out group);
				EndOfStmt();
				Block(
#line  2334 "VBNET.ATG" 
out embeddedStatement);
				Expect(128);
				if (StartOf(26)) {
					Expr(
#line  2335 "VBNET.ATG" 
out expr);
				}

#line  2337 "VBNET.ATG" 
				statement = new ForeachStatement(typeReference, 
				                                typeName,
				                                group, 
				                                embeddedStatement, 
				                                expr);
				statement.StartLocation = startLocation;
				statement.EndLocation   = t.EndLocation;
				
				
			} else if (StartOf(12)) {

#line  2348 "VBNET.ATG" 
				Expression start = null;
				Expression end = null;
				Expression step = null;
				Expression nextExpr = null;List<Expression> nextExpressions = null;
				
				LoopControlVariable(
#line  2353 "VBNET.ATG" 
out typeReference, out typeName);
				Expect(11);
				Expr(
#line  2354 "VBNET.ATG" 
out start);
				Expect(172);
				Expr(
#line  2354 "VBNET.ATG" 
out end);
				if (la.kind == 162) {
					lexer.NextToken();
					Expr(
#line  2354 "VBNET.ATG" 
out step);
				}
				EndOfStmt();
				Block(
#line  2355 "VBNET.ATG" 
out embeddedStatement);
				Expect(128);
				if (StartOf(26)) {
					Expr(
#line  2358 "VBNET.ATG" 
out nextExpr);

#line  2358 "VBNET.ATG" 
					nextExpressions = new List<Expression>(); nextExpressions.Add(nextExpr); 
					while (la.kind == 12) {
						lexer.NextToken();
						Expr(
#line  2359 "VBNET.ATG" 
out nextExpr);

#line  2359 "VBNET.ATG" 
						nextExpressions.Add(nextExpr); 
					}
				}

#line  2362 "VBNET.ATG" 
				statement = new ForNextStatement(typeReference, typeName, start, end, step, embeddedStatement, nextExpressions);
				
			} else SynErr(256);
			break;
		}
		case 92: {
			lexer.NextToken();
			Expr(
#line  2366 "VBNET.ATG" 
out expr);

#line  2366 "VBNET.ATG" 
			statement = new ErrorStatement(expr); 
			break;
		}
		case 151: {
			lexer.NextToken();

#line  2368 "VBNET.ATG" 
			bool isPreserve = false; 
			if (la.kind == 144) {
				lexer.NextToken();

#line  2368 "VBNET.ATG" 
				isPreserve = true; 
			}
			ReDimClause(
#line  2369 "VBNET.ATG" 
out expr);

#line  2371 "VBNET.ATG" 
			ReDimStatement reDimStatement = new ReDimStatement(isPreserve);
			statement = reDimStatement;
			InvocationExpression redimClause = expr as InvocationExpression;
			if (redimClause != null) { reDimStatement.ReDimClauses.Add(redimClause); }
			
			while (la.kind == 12) {
				lexer.NextToken();
				ReDimClause(
#line  2376 "VBNET.ATG" 
out expr);

#line  2377 "VBNET.ATG" 
				redimClause = expr as InvocationExpression; 

#line  2378 "VBNET.ATG" 
				if (redimClause != null) { reDimStatement.ReDimClauses.Add(redimClause); } 
			}
			break;
		}
		case 91: {
			lexer.NextToken();
			Expr(
#line  2382 "VBNET.ATG" 
out expr);

#line  2383 "VBNET.ATG" 
			List<Expression> arrays = new List<Expression>();
			if (expr != null) { arrays.Add(expr);}
			EraseStatement eraseStatement = new EraseStatement(arrays);
			
			
			while (la.kind == 12) {
				lexer.NextToken();
				Expr(
#line  2388 "VBNET.ATG" 
out expr);

#line  2388 "VBNET.ATG" 
				if (expr != null) { arrays.Add(expr); }
			}

#line  2389 "VBNET.ATG" 
			statement = eraseStatement; 
			break;
		}
		case 163: {
			lexer.NextToken();

#line  2391 "VBNET.ATG" 
			statement = new StopStatement(); 
			break;
		}
		case 106: {
			lexer.NextToken();
			Expr(
#line  2393 "VBNET.ATG" 
out expr);
			if (la.kind == 170) {
				lexer.NextToken();
			}
			if (
#line  2395 "VBNET.ATG" 
IsEndStmtAhead()) {
				Expect(88);

#line  2395 "VBNET.ATG" 
				statement = new IfElseStatement(expr, new EndStatement()); 
			} else if (la.kind == 1 || la.kind == 13) {
				EndOfStmt();
				Block(
#line  2398 "VBNET.ATG" 
out embeddedStatement);

#line  2400 "VBNET.ATG" 
				IfElseStatement ifStatement = new IfElseStatement(expr, embeddedStatement);
				
				while (la.kind == 87 || 
#line  2404 "VBNET.ATG" 
IsElseIf()) {
					if (
#line  2404 "VBNET.ATG" 
IsElseIf()) {
						Expect(86);
						Expect(106);
					} else {
						lexer.NextToken();
					}

#line  2407 "VBNET.ATG" 
					Expression condition = null; Statement block = null; 
					Expr(
#line  2408 "VBNET.ATG" 
out condition);
					if (la.kind == 170) {
						lexer.NextToken();
					}
					EndOfStmt();
					Block(
#line  2409 "VBNET.ATG" 
out block);

#line  2411 "VBNET.ATG" 
					ifStatement.ElseIfSections.Add(new ElseIfSection(condition, block));
					
				}
				if (la.kind == 86) {
					lexer.NextToken();
					EndOfStmt();
					Block(
#line  2416 "VBNET.ATG" 
out embeddedStatement);

#line  2418 "VBNET.ATG" 
					ifStatement.FalseStatement.Add(embeddedStatement);
					
				}
				Expect(88);
				Expect(106);

#line  2422 "VBNET.ATG" 
				statement = ifStatement;
				
			} else if (StartOf(33)) {
				EmbeddedStatement(
#line  2425 "VBNET.ATG" 
out embeddedStatement);

#line  2427 "VBNET.ATG" 
				IfElseStatement ifStatement = new IfElseStatement(expr, embeddedStatement);
				
				while (la.kind == 13) {
					lexer.NextToken();
					EmbeddedStatement(
#line  2429 "VBNET.ATG" 
out embeddedStatement);

#line  2429 "VBNET.ATG" 
					ifStatement.TrueStatement.Add(embeddedStatement); 
				}
				if (la.kind == 86) {
					lexer.NextToken();
					if (StartOf(33)) {
						EmbeddedStatement(
#line  2431 "VBNET.ATG" 
out embeddedStatement);
					}

#line  2433 "VBNET.ATG" 
					ifStatement.FalseStatement.Add(embeddedStatement);
					
					while (la.kind == 13) {
						lexer.NextToken();
						EmbeddedStatement(
#line  2436 "VBNET.ATG" 
out embeddedStatement);

#line  2437 "VBNET.ATG" 
						ifStatement.FalseStatement.Add(embeddedStatement); 
					}
				}

#line  2440 "VBNET.ATG" 
				statement = ifStatement; 
			} else SynErr(257);
			break;
		}
		case 155: {
			lexer.NextToken();
			if (la.kind == 57) {
				lexer.NextToken();
			}
			Expr(
#line  2443 "VBNET.ATG" 
out expr);
			EndOfStmt();

#line  2444 "VBNET.ATG" 
			List<SwitchSection> selectSections = new List<SwitchSection>();
			Statement block = null;
			
			while (la.kind == 57) {

#line  2448 "VBNET.ATG" 
				List<CaseLabel> caseClauses = null; 
				lexer.NextToken();
				CaseClauses(
#line  2449 "VBNET.ATG" 
out caseClauses);
				if (
#line  2449 "VBNET.ATG" 
IsNotStatementSeparator()) {
					lexer.NextToken();
				}
				EndOfStmt();

#line  2451 "VBNET.ATG" 
				SwitchSection selectSection = new SwitchSection(caseClauses);
				
				Block(
#line  2453 "VBNET.ATG" 
out block);

#line  2455 "VBNET.ATG" 
				selectSection.Children = block.Children;
				selectSections.Add(selectSection);
				
			}

#line  2459 "VBNET.ATG" 
			statement = new SwitchStatement(expr, selectSections); 
			Expect(88);
			Expect(155);
			break;
		}
		case 135: {

#line  2461 "VBNET.ATG" 
			OnErrorStatement onErrorStatement = null; 
			OnErrorStatement(
#line  2462 "VBNET.ATG" 
out onErrorStatement);

#line  2462 "VBNET.ATG" 
			statement = onErrorStatement; 
			break;
		}
		case 104: {

#line  2463 "VBNET.ATG" 
			GotoStatement goToStatement = null; 
			GotoStatement(
#line  2464 "VBNET.ATG" 
out goToStatement);

#line  2464 "VBNET.ATG" 
			statement = goToStatement; 
			break;
		}
		case 153: {

#line  2465 "VBNET.ATG" 
			ResumeStatement resumeStatement = null; 
			ResumeStatement(
#line  2466 "VBNET.ATG" 
out resumeStatement);

#line  2466 "VBNET.ATG" 
			statement = resumeStatement; 
			break;
		}
		case 2: case 3: case 4: case 5: case 6: case 7: case 8: case 9: case 10: case 24: case 43: case 47: case 49: case 50: case 51: case 52: case 54: case 59: case 60: case 61: case 62: case 63: case 64: case 65: case 66: case 68: case 69: case 70: case 72: case 73: case 74: case 75: case 76: case 77: case 82: case 84: case 96: case 102: case 111: case 117: case 119: case 124: case 125: case 127: case 130: case 133: case 144: case 159: case 160: case 165: case 169: case 173: case 175: case 176: case 177: case 190: case 191: case 192: case 193: case 194: case 195: case 196: case 197: case 198: case 199: case 204: {

#line  2469 "VBNET.ATG" 
			Expression val = null;
			AssignmentOperatorType op;
			
			bool mustBeAssignment = la.kind == Tokens.Plus  || la.kind == Tokens.Minus ||
			                        la.kind == Tokens.Not   || la.kind == Tokens.Times;
			
			SimpleExpr(
#line  2475 "VBNET.ATG" 
out expr);
			if (StartOf(35)) {
				AssignmentOperator(
#line  2477 "VBNET.ATG" 
out op);
				Expr(
#line  2477 "VBNET.ATG" 
out val);

#line  2477 "VBNET.ATG" 
				expr = new AssignmentExpression(expr, op, val); 
			} else if (la.kind == 1 || la.kind == 13 || la.kind == 86) {

#line  2478 "VBNET.ATG" 
				if (mustBeAssignment) Error("error in assignment."); 
			} else SynErr(258);

#line  2481 "VBNET.ATG" 
			// a field reference expression that stands alone is a
			// invocation expression without parantheses and arguments
			if(expr is FieldReferenceExpression || expr is IdentifierExpression) {
				expr = new InvocationExpression(expr);
			}
			statement = new ExpressionStatement(expr);
			
			break;
		}
		case 56: {
			lexer.NextToken();
			SimpleExpr(
#line  2488 "VBNET.ATG" 
out expr);

#line  2488 "VBNET.ATG" 
			statement = new ExpressionStatement(expr); 
			break;
		}
		case 188: {
			lexer.NextToken();

#line  2490 "VBNET.ATG" 
			Statement block;  
			if (
#line  2491 "VBNET.ATG" 
Peek(1).kind == Tokens.As) {

#line  2492 "VBNET.ATG" 
				LocalVariableDeclaration resourceAquisition = new LocalVariableDeclaration(Modifiers.None); 
				VariableDeclarator(
#line  2493 "VBNET.ATG" 
resourceAquisition.Variables);
				while (la.kind == 12) {
					lexer.NextToken();
					VariableDeclarator(
#line  2495 "VBNET.ATG" 
resourceAquisition.Variables);
				}
				Block(
#line  2497 "VBNET.ATG" 
out block);

#line  2498 "VBNET.ATG" 
				statement = new UsingStatement(resourceAquisition, block); 
			} else if (StartOf(26)) {
				Expr(
#line  2499 "VBNET.ATG" 
out expr);
				Block(
#line  2500 "VBNET.ATG" 
out block);

#line  2501 "VBNET.ATG" 
				statement = new UsingStatement(new ExpressionStatement(expr), block); 
			} else SynErr(259);
			Expect(88);
			Expect(188);
			break;
		}
		default: SynErr(260); break;
		}
	}

	void LocalDeclarationStatement(
#line  2209 "VBNET.ATG" 
out Statement statement) {

#line  2211 "VBNET.ATG" 
		ModifierList m = new ModifierList();
		LocalVariableDeclaration localVariableDeclaration;
		bool dimfound = false;
		
		while (la.kind == 71 || la.kind == 81 || la.kind == 161) {
			if (la.kind == 71) {
				lexer.NextToken();

#line  2217 "VBNET.ATG" 
				m.Add(Modifiers.Const, t.Location); 
			} else if (la.kind == 161) {
				lexer.NextToken();

#line  2218 "VBNET.ATG" 
				m.Add(Modifiers.Static, t.Location); 
			} else {
				lexer.NextToken();

#line  2219 "VBNET.ATG" 
				dimfound = true; 
			}
		}

#line  2222 "VBNET.ATG" 
		if(dimfound && (m.Modifier & Modifiers.Const) != 0) {
		Error("Dim is not allowed on constants.");
		}
		
		if(m.isNone && dimfound == false) {
			Error("Const, Dim or Static expected");
		}
		
		localVariableDeclaration = new LocalVariableDeclaration(m.Modifier);
		localVariableDeclaration.StartLocation = t.Location;
		
		VariableDeclarator(
#line  2233 "VBNET.ATG" 
localVariableDeclaration.Variables);
		while (la.kind == 12) {
			lexer.NextToken();
			VariableDeclarator(
#line  2234 "VBNET.ATG" 
localVariableDeclaration.Variables);
		}

#line  2236 "VBNET.ATG" 
		statement = localVariableDeclaration;
		
	}

	void TryStatement(
#line  2688 "VBNET.ATG" 
out Statement tryStatement) {

#line  2690 "VBNET.ATG" 
		Statement blockStmt = null, finallyStmt = null;List<CatchClause> catchClauses = null;
		
		Expect(174);
		EndOfStmt();
		Block(
#line  2693 "VBNET.ATG" 
out blockStmt);
		if (la.kind == 58 || la.kind == 88 || la.kind == 97) {
			CatchClauses(
#line  2694 "VBNET.ATG" 
out catchClauses);
		}
		if (la.kind == 97) {
			lexer.NextToken();
			EndOfStmt();
			Block(
#line  2695 "VBNET.ATG" 
out finallyStmt);
		}
		Expect(88);
		Expect(174);

#line  2698 "VBNET.ATG" 
		tryStatement = new TryCatchStatement(blockStmt, catchClauses, finallyStmt);
		
	}

	void WithStatement(
#line  2668 "VBNET.ATG" 
out Statement withStatement) {

#line  2670 "VBNET.ATG" 
		Statement blockStmt = null;
		Expression expr = null;
		
		Expect(182);

#line  2673 "VBNET.ATG" 
		Location start = t.Location; 
		Expr(
#line  2674 "VBNET.ATG" 
out expr);
		EndOfStmt();

#line  2676 "VBNET.ATG" 
		withStatement = new WithStatement(expr);
		withStatement.StartLocation = start;
		
		Block(
#line  2679 "VBNET.ATG" 
out blockStmt);

#line  2681 "VBNET.ATG" 
		((WithStatement)withStatement).Body = (BlockStatement)blockStmt;
		
		Expect(88);
		Expect(182);

#line  2684 "VBNET.ATG" 
		withStatement.EndLocation = t.Location; 
	}

	void WhileOrUntil(
#line  2661 "VBNET.ATG" 
out ConditionType conditionType) {

#line  2662 "VBNET.ATG" 
		conditionType = ConditionType.None; 
		if (la.kind == 181) {
			lexer.NextToken();

#line  2663 "VBNET.ATG" 
			conditionType = ConditionType.While; 
		} else if (la.kind == 177) {
			lexer.NextToken();

#line  2664 "VBNET.ATG" 
			conditionType = ConditionType.Until; 
		} else SynErr(261);
	}

	void LoopControlVariable(
#line  2507 "VBNET.ATG" 
out TypeReference type, out string name) {

#line  2508 "VBNET.ATG" 
		ArrayList arrayModifiers = null;
		type = null;
		
		Qualident(
#line  2512 "VBNET.ATG" 
out name);
		if (
#line  2513 "VBNET.ATG" 
IsDims()) {
			ArrayTypeModifiers(
#line  2513 "VBNET.ATG" 
out arrayModifiers);
		}
		if (la.kind == 48) {
			lexer.NextToken();
			TypeName(
#line  2514 "VBNET.ATG" 
out type);

#line  2514 "VBNET.ATG" 
			if (name.IndexOf('.') > 0) { Error("No type def for 'for each' member indexer allowed."); } 
		}

#line  2516 "VBNET.ATG" 
		if (type != null) {
		if(type.RankSpecifier != null && arrayModifiers != null) {
			Error("array rank only allowed one time");
		} else if (arrayModifiers != null) {
			type.RankSpecifier = (int[])arrayModifiers.ToArray(typeof(int));
		}
		}
		
	}

	void ReDimClause(
#line  2586 "VBNET.ATG" 
out Expression expr) {
		SimpleNonInvocationExpression(
#line  2588 "VBNET.ATG" 
out expr);
		ReDimClauseInternal(
#line  2589 "VBNET.ATG" 
ref expr);
	}

	void CaseClauses(
#line  2621 "VBNET.ATG" 
out List<CaseLabel> caseClauses) {

#line  2623 "VBNET.ATG" 
		caseClauses = new List<CaseLabel>();
		CaseLabel caseClause = null;
		
		CaseClause(
#line  2626 "VBNET.ATG" 
out caseClause);

#line  2626 "VBNET.ATG" 
		if (caseClause != null) { caseClauses.Add(caseClause); } 
		while (la.kind == 12) {
			lexer.NextToken();
			CaseClause(
#line  2627 "VBNET.ATG" 
out caseClause);

#line  2627 "VBNET.ATG" 
			if (caseClause != null) { caseClauses.Add(caseClause); } 
		}
	}

	void OnErrorStatement(
#line  2527 "VBNET.ATG" 
out OnErrorStatement stmt) {

#line  2529 "VBNET.ATG" 
		stmt = null;
		GotoStatement goToStatement = null;
		
		Expect(135);
		Expect(92);
		if (
#line  2535 "VBNET.ATG" 
IsNegativeLabelName()) {
			Expect(104);
			Expect(15);
			Expect(5);

#line  2537 "VBNET.ATG" 
			long intLabel = Int64.Parse(t.val);
			if(intLabel != 1) {
				Error("invalid label in on error statement.");
			}
			stmt = new OnErrorStatement(new GotoStatement((intLabel * -1).ToString()));
			
		} else if (la.kind == 104) {
			GotoStatement(
#line  2543 "VBNET.ATG" 
out goToStatement);

#line  2545 "VBNET.ATG" 
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
			
		} else if (la.kind == 153) {
			lexer.NextToken();
			Expect(128);

#line  2559 "VBNET.ATG" 
			stmt = new OnErrorStatement(new ResumeStatement(true));
			
		} else SynErr(262);
	}

	void GotoStatement(
#line  2565 "VBNET.ATG" 
out GotoStatement goToStatement) {

#line  2567 "VBNET.ATG" 
		string label = String.Empty;
		
		Expect(104);
		LabelName(
#line  2570 "VBNET.ATG" 
out label);

#line  2572 "VBNET.ATG" 
		goToStatement = new GotoStatement(label);
		
	}

	void ResumeStatement(
#line  2610 "VBNET.ATG" 
out ResumeStatement resumeStatement) {

#line  2612 "VBNET.ATG" 
		resumeStatement = null;
		string label = String.Empty;
		
		if (
#line  2615 "VBNET.ATG" 
IsResumeNext()) {
			Expect(153);
			Expect(128);

#line  2616 "VBNET.ATG" 
			resumeStatement = new ResumeStatement(true); 
		} else if (la.kind == 153) {
			lexer.NextToken();
			if (StartOf(36)) {
				LabelName(
#line  2617 "VBNET.ATG" 
out label);
			}

#line  2617 "VBNET.ATG" 
			resumeStatement = new ResumeStatement(label); 
		} else SynErr(263);
	}

	void ReDimClauseInternal(
#line  2592 "VBNET.ATG" 
ref Expression expr) {

#line  2593 "VBNET.ATG" 
		List<Expression> arguments; bool canBeNormal; bool canBeRedim; string name; 
		while (la.kind == 10 || 
#line  2596 "VBNET.ATG" 
la.kind == Tokens.OpenParenthesis && Peek(1).kind == Tokens.Of) {
			if (la.kind == 10) {
				lexer.NextToken();
				IdentifierOrKeyword(
#line  2595 "VBNET.ATG" 
out name);

#line  2595 "VBNET.ATG" 
				expr = new FieldReferenceExpression(expr, name); 
			} else {
				InvocationExpression(
#line  2597 "VBNET.ATG" 
ref expr);
			}
		}
		Expect(24);
		NormalOrReDimArgumentList(
#line  2600 "VBNET.ATG" 
out arguments, out canBeNormal, out canBeRedim);
		Expect(25);

#line  2602 "VBNET.ATG" 
		expr = new InvocationExpression(expr, arguments);
		if (canBeRedim == false || canBeNormal && (la.kind == Tokens.Dot || la.kind == Tokens.OpenParenthesis)) {
			ReDimClauseInternal(ref expr);
		}
		
	}

	void CaseClause(
#line  2631 "VBNET.ATG" 
out CaseLabel caseClause) {

#line  2633 "VBNET.ATG" 
		Expression expr = null;
		Expression sexpr = null;
		BinaryOperatorType op = BinaryOperatorType.None;
		caseClause = null;
		
		if (la.kind == 86) {
			lexer.NextToken();

#line  2639 "VBNET.ATG" 
			caseClause = new CaseLabel(); 
		} else if (StartOf(37)) {
			if (la.kind == 113) {
				lexer.NextToken();
			}
			switch (la.kind) {
			case 27: {
				lexer.NextToken();

#line  2643 "VBNET.ATG" 
				op = BinaryOperatorType.LessThan; 
				break;
			}
			case 26: {
				lexer.NextToken();

#line  2644 "VBNET.ATG" 
				op = BinaryOperatorType.GreaterThan; 
				break;
			}
			case 30: {
				lexer.NextToken();

#line  2645 "VBNET.ATG" 
				op = BinaryOperatorType.LessThanOrEqual; 
				break;
			}
			case 29: {
				lexer.NextToken();

#line  2646 "VBNET.ATG" 
				op = BinaryOperatorType.GreaterThanOrEqual; 
				break;
			}
			case 11: {
				lexer.NextToken();

#line  2647 "VBNET.ATG" 
				op = BinaryOperatorType.Equality; 
				break;
			}
			case 28: {
				lexer.NextToken();

#line  2648 "VBNET.ATG" 
				op = BinaryOperatorType.InEquality; 
				break;
			}
			default: SynErr(264); break;
			}
			Expr(
#line  2650 "VBNET.ATG" 
out expr);

#line  2652 "VBNET.ATG" 
			caseClause = new CaseLabel(op, expr);
			
		} else if (StartOf(26)) {
			Expr(
#line  2654 "VBNET.ATG" 
out expr);
			if (la.kind == 172) {
				lexer.NextToken();
				Expr(
#line  2654 "VBNET.ATG" 
out sexpr);
			}

#line  2656 "VBNET.ATG" 
			caseClause = new CaseLabel(expr, sexpr);
			
		} else SynErr(265);
	}

	void CatchClauses(
#line  2703 "VBNET.ATG" 
out List<CatchClause> catchClauses) {

#line  2705 "VBNET.ATG" 
		catchClauses = new List<CatchClause>();
		TypeReference type = null;
		Statement blockStmt = null;
		Expression expr = null;
		string name = String.Empty;
		
		while (la.kind == 58) {
			lexer.NextToken();
			if (StartOf(12)) {
				Identifier();

#line  2713 "VBNET.ATG" 
				name = t.val; 
				if (la.kind == 48) {
					lexer.NextToken();
					TypeName(
#line  2713 "VBNET.ATG" 
out type);
				}
			}
			if (la.kind == 180) {
				lexer.NextToken();
				Expr(
#line  2714 "VBNET.ATG" 
out expr);
			}
			EndOfStmt();
			Block(
#line  2716 "VBNET.ATG" 
out blockStmt);

#line  2717 "VBNET.ATG" 
			catchClauses.Add(new CatchClause(type, name, blockStmt, expr)); 
		}
	}


	
	public override void Parse()
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
			case 10: s = "\".\" expected"; break;
			case 11: s = "\"=\" expected"; break;
			case 12: s = "\",\" expected"; break;
			case 13: s = "\":\" expected"; break;
			case 14: s = "\"+\" expected"; break;
			case 15: s = "\"-\" expected"; break;
			case 16: s = "\"*\" expected"; break;
			case 17: s = "\"/\" expected"; break;
			case 18: s = "\"\\\\\" expected"; break;
			case 19: s = "\"&\" expected"; break;
			case 20: s = "\"^\" expected"; break;
			case 21: s = "\"?\" expected"; break;
			case 22: s = "\"{\" expected"; break;
			case 23: s = "\"}\" expected"; break;
			case 24: s = "\"(\" expected"; break;
			case 25: s = "\")\" expected"; break;
			case 26: s = "\">\" expected"; break;
			case 27: s = "\"<\" expected"; break;
			case 28: s = "\"<>\" expected"; break;
			case 29: s = "\">=\" expected"; break;
			case 30: s = "\"<=\" expected"; break;
			case 31: s = "\"<<\" expected"; break;
			case 32: s = "\">>\" expected"; break;
			case 33: s = "\"+=\" expected"; break;
			case 34: s = "\"^=\" expected"; break;
			case 35: s = "\"-=\" expected"; break;
			case 36: s = "\"*=\" expected"; break;
			case 37: s = "\"/=\" expected"; break;
			case 38: s = "\"\\\\=\" expected"; break;
			case 39: s = "\"<<=\" expected"; break;
			case 40: s = "\">>=\" expected"; break;
			case 41: s = "\"&=\" expected"; break;
			case 42: s = "\"AddHandler\" expected"; break;
			case 43: s = "\"AddressOf\" expected"; break;
			case 44: s = "\"Alias\" expected"; break;
			case 45: s = "\"And\" expected"; break;
			case 46: s = "\"AndAlso\" expected"; break;
			case 47: s = "\"Ansi\" expected"; break;
			case 48: s = "\"As\" expected"; break;
			case 49: s = "\"Assembly\" expected"; break;
			case 50: s = "\"Auto\" expected"; break;
			case 51: s = "\"Binary\" expected"; break;
			case 52: s = "\"Boolean\" expected"; break;
			case 53: s = "\"ByRef\" expected"; break;
			case 54: s = "\"Byte\" expected"; break;
			case 55: s = "\"ByVal\" expected"; break;
			case 56: s = "\"Call\" expected"; break;
			case 57: s = "\"Case\" expected"; break;
			case 58: s = "\"Catch\" expected"; break;
			case 59: s = "\"CBool\" expected"; break;
			case 60: s = "\"CByte\" expected"; break;
			case 61: s = "\"CChar\" expected"; break;
			case 62: s = "\"CDate\" expected"; break;
			case 63: s = "\"CDbl\" expected"; break;
			case 64: s = "\"CDec\" expected"; break;
			case 65: s = "\"Char\" expected"; break;
			case 66: s = "\"CInt\" expected"; break;
			case 67: s = "\"Class\" expected"; break;
			case 68: s = "\"CLng\" expected"; break;
			case 69: s = "\"CObj\" expected"; break;
			case 70: s = "\"Compare\" expected"; break;
			case 71: s = "\"Const\" expected"; break;
			case 72: s = "\"CShort\" expected"; break;
			case 73: s = "\"CSng\" expected"; break;
			case 74: s = "\"CStr\" expected"; break;
			case 75: s = "\"CType\" expected"; break;
			case 76: s = "\"Date\" expected"; break;
			case 77: s = "\"Decimal\" expected"; break;
			case 78: s = "\"Declare\" expected"; break;
			case 79: s = "\"Default\" expected"; break;
			case 80: s = "\"Delegate\" expected"; break;
			case 81: s = "\"Dim\" expected"; break;
			case 82: s = "\"DirectCast\" expected"; break;
			case 83: s = "\"Do\" expected"; break;
			case 84: s = "\"Double\" expected"; break;
			case 85: s = "\"Each\" expected"; break;
			case 86: s = "\"Else\" expected"; break;
			case 87: s = "\"ElseIf\" expected"; break;
			case 88: s = "\"End\" expected"; break;
			case 89: s = "\"EndIf\" expected"; break;
			case 90: s = "\"Enum\" expected"; break;
			case 91: s = "\"Erase\" expected"; break;
			case 92: s = "\"Error\" expected"; break;
			case 93: s = "\"Event\" expected"; break;
			case 94: s = "\"Exit\" expected"; break;
			case 95: s = "\"Explicit\" expected"; break;
			case 96: s = "\"False\" expected"; break;
			case 97: s = "\"Finally\" expected"; break;
			case 98: s = "\"For\" expected"; break;
			case 99: s = "\"Friend\" expected"; break;
			case 100: s = "\"Function\" expected"; break;
			case 101: s = "\"Get\" expected"; break;
			case 102: s = "\"GetType\" expected"; break;
			case 103: s = "\"GoSub\" expected"; break;
			case 104: s = "\"GoTo\" expected"; break;
			case 105: s = "\"Handles\" expected"; break;
			case 106: s = "\"If\" expected"; break;
			case 107: s = "\"Implements\" expected"; break;
			case 108: s = "\"Imports\" expected"; break;
			case 109: s = "\"In\" expected"; break;
			case 110: s = "\"Inherits\" expected"; break;
			case 111: s = "\"Integer\" expected"; break;
			case 112: s = "\"Interface\" expected"; break;
			case 113: s = "\"Is\" expected"; break;
			case 114: s = "\"Let\" expected"; break;
			case 115: s = "\"Lib\" expected"; break;
			case 116: s = "\"Like\" expected"; break;
			case 117: s = "\"Long\" expected"; break;
			case 118: s = "\"Loop\" expected"; break;
			case 119: s = "\"Me\" expected"; break;
			case 120: s = "\"Mod\" expected"; break;
			case 121: s = "\"Module\" expected"; break;
			case 122: s = "\"MustInherit\" expected"; break;
			case 123: s = "\"MustOverride\" expected"; break;
			case 124: s = "\"MyBase\" expected"; break;
			case 125: s = "\"MyClass\" expected"; break;
			case 126: s = "\"Namespace\" expected"; break;
			case 127: s = "\"New\" expected"; break;
			case 128: s = "\"Next\" expected"; break;
			case 129: s = "\"Not\" expected"; break;
			case 130: s = "\"Nothing\" expected"; break;
			case 131: s = "\"NotInheritable\" expected"; break;
			case 132: s = "\"NotOverridable\" expected"; break;
			case 133: s = "\"Object\" expected"; break;
			case 134: s = "\"Off\" expected"; break;
			case 135: s = "\"On\" expected"; break;
			case 136: s = "\"Option\" expected"; break;
			case 137: s = "\"Optional\" expected"; break;
			case 138: s = "\"Or\" expected"; break;
			case 139: s = "\"OrElse\" expected"; break;
			case 140: s = "\"Overloads\" expected"; break;
			case 141: s = "\"Overridable\" expected"; break;
			case 142: s = "\"Overrides\" expected"; break;
			case 143: s = "\"ParamArray\" expected"; break;
			case 144: s = "\"Preserve\" expected"; break;
			case 145: s = "\"Private\" expected"; break;
			case 146: s = "\"Property\" expected"; break;
			case 147: s = "\"Protected\" expected"; break;
			case 148: s = "\"Public\" expected"; break;
			case 149: s = "\"RaiseEvent\" expected"; break;
			case 150: s = "\"ReadOnly\" expected"; break;
			case 151: s = "\"ReDim\" expected"; break;
			case 152: s = "\"RemoveHandler\" expected"; break;
			case 153: s = "\"Resume\" expected"; break;
			case 154: s = "\"Return\" expected"; break;
			case 155: s = "\"Select\" expected"; break;
			case 156: s = "\"Set\" expected"; break;
			case 157: s = "\"Shadows\" expected"; break;
			case 158: s = "\"Shared\" expected"; break;
			case 159: s = "\"Short\" expected"; break;
			case 160: s = "\"Single\" expected"; break;
			case 161: s = "\"Static\" expected"; break;
			case 162: s = "\"Step\" expected"; break;
			case 163: s = "\"Stop\" expected"; break;
			case 164: s = "\"Strict\" expected"; break;
			case 165: s = "\"String\" expected"; break;
			case 166: s = "\"Structure\" expected"; break;
			case 167: s = "\"Sub\" expected"; break;
			case 168: s = "\"SyncLock\" expected"; break;
			case 169: s = "\"Text\" expected"; break;
			case 170: s = "\"Then\" expected"; break;
			case 171: s = "\"Throw\" expected"; break;
			case 172: s = "\"To\" expected"; break;
			case 173: s = "\"True\" expected"; break;
			case 174: s = "\"Try\" expected"; break;
			case 175: s = "\"TypeOf\" expected"; break;
			case 176: s = "\"Unicode\" expected"; break;
			case 177: s = "\"Until\" expected"; break;
			case 178: s = "\"Variant\" expected"; break;
			case 179: s = "\"Wend\" expected"; break;
			case 180: s = "\"When\" expected"; break;
			case 181: s = "\"While\" expected"; break;
			case 182: s = "\"With\" expected"; break;
			case 183: s = "\"WithEvents\" expected"; break;
			case 184: s = "\"WriteOnly\" expected"; break;
			case 185: s = "\"Xor\" expected"; break;
			case 186: s = "\"Continue\" expected"; break;
			case 187: s = "\"Operator\" expected"; break;
			case 188: s = "\"Using\" expected"; break;
			case 189: s = "\"IsNot\" expected"; break;
			case 190: s = "\"SByte\" expected"; break;
			case 191: s = "\"UInteger\" expected"; break;
			case 192: s = "\"ULong\" expected"; break;
			case 193: s = "\"UShort\" expected"; break;
			case 194: s = "\"CSByte\" expected"; break;
			case 195: s = "\"CUShort\" expected"; break;
			case 196: s = "\"CUInt\" expected"; break;
			case 197: s = "\"CULng\" expected"; break;
			case 198: s = "\"Global\" expected"; break;
			case 199: s = "\"TryCast\" expected"; break;
			case 200: s = "\"Of\" expected"; break;
			case 201: s = "\"Narrowing\" expected"; break;
			case 202: s = "\"Widening\" expected"; break;
			case 203: s = "\"Partial\" expected"; break;
			case 204: s = "\"Custom\" expected"; break;
			case 205: s = "??? expected"; break;
			case 206: s = "invalid OptionStmt"; break;
			case 207: s = "invalid OptionStmt"; break;
			case 208: s = "invalid GlobalAttributeSection"; break;
			case 209: s = "invalid GlobalAttributeSection"; break;
			case 210: s = "invalid NamespaceMemberDecl"; break;
			case 211: s = "invalid OptionValue"; break;
			case 212: s = "invalid EndOfStmt"; break;
			case 213: s = "invalid TypeModifier"; break;
			case 214: s = "invalid NonModuleDeclaration"; break;
			case 215: s = "invalid NonModuleDeclaration"; break;
			case 216: s = "invalid Identifier"; break;
			case 217: s = "invalid TypeParameterConstraints"; break;
			case 218: s = "invalid NonArrayTypeName"; break;
			case 219: s = "invalid MemberModifier"; break;
			case 220: s = "invalid StructureMemberDecl"; break;
			case 221: s = "invalid StructureMemberDecl"; break;
			case 222: s = "invalid StructureMemberDecl"; break;
			case 223: s = "invalid StructureMemberDecl"; break;
			case 224: s = "invalid StructureMemberDecl"; break;
			case 225: s = "invalid StructureMemberDecl"; break;
			case 226: s = "invalid StructureMemberDecl"; break;
			case 227: s = "invalid InterfaceMemberDecl"; break;
			case 228: s = "invalid InterfaceMemberDecl"; break;
			case 229: s = "invalid Charset"; break;
			case 230: s = "invalid IdentifierForFieldDeclaration"; break;
			case 231: s = "invalid VariableDeclaratorPartAfterIdentifier"; break;
			case 232: s = "invalid AccessorDecls"; break;
			case 233: s = "invalid EventAccessorDeclaration"; break;
			case 234: s = "invalid OverloadableOperator"; break;
			case 235: s = "invalid VariableInitializer"; break;
			case 236: s = "invalid EventMemberSpecifier"; break;
			case 237: s = "invalid EventMemberSpecifier"; break;
			case 238: s = "invalid AssignmentOperator"; break;
			case 239: s = "invalid SimpleNonInvocationExpression"; break;
			case 240: s = "invalid SimpleNonInvocationExpression"; break;
			case 241: s = "invalid SimpleNonInvocationExpression"; break;
			case 242: s = "invalid SimpleNonInvocationExpression"; break;
			case 243: s = "invalid InvocationExpression"; break;
			case 244: s = "invalid InvocationExpression"; break;
			case 245: s = "invalid PrimitiveTypeName"; break;
			case 246: s = "invalid CastTarget"; break;
			case 247: s = "invalid ComparisonExpr"; break;
			case 248: s = "invalid Argument"; break;
			case 249: s = "invalid QualIdentAndTypeArguments"; break;
			case 250: s = "invalid AttributeArguments"; break;
			case 251: s = "invalid ParameterModifier"; break;
			case 252: s = "invalid Statement"; break;
			case 253: s = "invalid LabelName"; break;
			case 254: s = "invalid EmbeddedStatement"; break;
			case 255: s = "invalid EmbeddedStatement"; break;
			case 256: s = "invalid EmbeddedStatement"; break;
			case 257: s = "invalid EmbeddedStatement"; break;
			case 258: s = "invalid EmbeddedStatement"; break;
			case 259: s = "invalid EmbeddedStatement"; break;
			case 260: s = "invalid EmbeddedStatement"; break;
			case 261: s = "invalid WhileOrUntil"; break;
			case 262: s = "invalid OnErrorStatement"; break;
			case 263: s = "invalid ResumeStatement"; break;
			case 264: s = "invalid CaseClause"; break;
			case 265: s = "invalid CaseClause"; break;

			default: s = "error " + errorNumber; break;
		}
		this.Errors.Error(line, col, s);
	}
	
	private bool StartOf(int s)
	{
		return set[s, lexer.LookAhead.kind];
	}
	
	static bool[,] set = {
	{T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,T,T,x, x,x,T,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, T,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, T,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, T,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,T, x,T,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,T, T,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,x, T,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,T,x,x, x,T,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,x,x, x,x,T,x, x,x,x,x, T,x,x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,T, x,x,x,x, x,x,T,T, T,T,x,x, x,x,x,x, x,x,T,x, x,T,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,T,T,T, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, T,T,T,x, T,T,T,T, T,x,T,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,T,T, x,T,x,x, x,x,x,x, T,T,x,x, x,x,x,T, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, T,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, T,T,T,x, x,T,x,T, T,x,T,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, T,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,T, T,T,x,x, x,x,x,x, x,x,T,x, x,T,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,T,T,T, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, T,T,T,x, x,T,T,T, T,x,T,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x},
	{x,T,T,T, T,T,T,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,T, x,T,T,T, T,x,T,x, T,x,x,T, T,T,T,T, T,T,T,x, T,T,T,T, T,T,T,T, T,T,x,x, x,T,T,T, T,x,x,x, T,x,x,T, T,x,T,x, T,x,T,x, x,x,T,x, T,x,T,x, x,x,x,T, x,x,x,x, x,T,x,T, x,x,x,x, T,T,x,T, x,x,T,x, x,T,x,T, x,x,x,x, x,x,x,x, T,x,x,x, x,T,x,T, T,T,T,T, x,x,x,T, T,T,x,T, x,T,x,x, T,T,x,T, x,T,T,T, T,T,x,x, x,T,T,x, x,x,T,x, T,x,T,T, T,T,T,T, T,T,T,T, x,x,x,x, T,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, T,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, T,T,T,x, x,T,T,T, T,x,T,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,T,T,T, T,T,T,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,T, x,T,T,T, T,x,T,x, T,x,x,T, T,T,T,T, T,T,T,x, T,T,T,T, T,T,T,T, T,T,x,x, x,T,T,T, T,x,x,x, x,x,x,T, T,x,T,x, T,x,T,x, x,x,T,x, T,x,T,x, x,x,x,T, x,x,x,x, x,T,x,T, x,x,x,x, T,T,x,T, x,x,T,x, x,T,x,T, x,x,x,x, x,x,x,x, T,x,x,x, x,T,x,T, T,T,T,T, x,x,x,T, T,T,x,T, x,T,x,x, T,T,x,T, x,T,T,T, T,T,x,x, x,T,T,x, x,x,T,x, T,x,T,T, T,T,T,T, T,T,T,T, x,x,x,x, T,x,x},
	{x,T,T,T, T,T,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,T, T,T,T,T, T,x,T,x, T,x,x,T, T,T,T,T, T,T,T,x, T,T,T,T, T,T,T,T, T,T,x,x, x,T,T,T, T,x,x,x, T,x,x,T, T,x,T,x, T,x,T,x, x,x,T,x, T,x,T,x, x,x,x,T, x,x,x,x, x,T,x,T, x,x,x,x, T,T,x,T, x,x,T,x, x,T,x,T, x,x,x,x, x,x,x,x, T,x,x,x, x,T,x,T, T,T,T,T, x,x,x,T, T,T,x,T, x,T,x,x, T,T,x,T, x,T,T,T, T,T,x,x, x,T,T,x, x,x,T,x, T,x,T,T, T,T,T,T, T,T,T,T, x,x,x,x, T,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, T,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, T,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,T,T, T,T,T,T, T,T,T,x, x,x,T,T, T,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,T,T,T, T,x,T,x, x,x,x,T, T,T,T,T, T,T,T,x, T,T,T,x, T,T,T,T, T,T,x,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,T, x,x,x,x, T,T,x,T, x,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,T,x,x, x,T,x,x, x,T,x,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,T,T, T,T,T,T, x,x,x,x, T,x,x},
	{x,x,T,T, T,T,T,T, T,T,T,x, x,x,T,T, T,x,x,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,T,T,T, T,x,T,x, x,x,x,T, T,T,T,T, T,T,T,x, T,T,T,x, T,T,T,T, T,T,x,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,T, x,x,x,x, T,T,x,T, x,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,T,x,x, x,T,x,x, x,T,x,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,T,T, T,T,T,T, x,x,x,x, T,x,x},
	{x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,T,T,T, T,x,T,x, x,x,x,T, T,T,T,T, T,T,T,x, T,T,T,x, T,T,T,T, T,T,x,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,T, x,x,x,x, T,T,x,T, x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,T,x,x, x,T,x,x, x,T,x,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,T,T, T,T,T,T, x,x,x,x, T,x,x},
	{x,x,T,T, T,T,T,T, T,T,T,x, T,x,T,T, T,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,T,T,T, T,x,T,x, x,x,x,T, T,T,T,T, T,T,T,x, T,T,T,x, T,T,T,T, T,T,x,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,T, x,x,x,x, T,T,x,T, x,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,T,x,x, x,T,x,x, x,T,x,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,T,T, T,T,T,T, x,x,x,x, T,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,T,T, T,T,T,T, T,T,T,x, x,x,T,T, T,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,T,T,T, T,x,T,x, x,x,x,T, T,T,T,T, T,T,T,x, T,T,T,x, T,T,T,T, T,T,x,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,T, x,x,x,x, T,T,x,T, x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,T,x,x, x,T,x,x, x,T,x,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,T,T, T,T,T,T, x,x,x,x, T,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,T, x,T,T,T, T,x,T,x, T,x,x,T, T,T,T,T, T,T,T,x, T,T,T,x, T,T,T,T, T,T,x,x, x,x,T,T, T,x,x,x, x,x,x,T, T,x,T,x, T,x,T,x, x,x,T,x, T,x,T,x, x,x,x,T, x,x,x,x, x,T,x,T, x,x,x,x, T,T,x,T, x,x,T,x, x,T,x,T, x,x,x,x, x,x,x,x, T,x,x,x, x,T,x,T, T,T,T,T, x,x,x,T, T,x,x,T, x,T,x,x, T,T,x,T, x,T,T,T, T,T,x,x, x,T,T,x, x,x,T,x, T,x,T,T, T,T,T,T, T,T,T,T, x,x,x,x, T,x,x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,T,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x}

	};
} // end Parser

}