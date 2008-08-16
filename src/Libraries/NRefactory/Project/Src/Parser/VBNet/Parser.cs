
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
	const int maxT = 208;

	const  bool   T            = true;
	const  bool   x            = false;
	

#line  12 "VBNET.ATG" 


/*

*/

	void VBNET() {

#line  232 "VBNET.ATG" 
		lexer.NextToken(); // get the first token
		compilationUnit = new CompilationUnit();
		
		while (la.kind == 1 || la.kind == 11) {
			EndOfStmt();
		}
		while (la.kind == 149) {
			OptionStmt();
			while (la.kind == 1 || la.kind == 11) {
				EndOfStmt();
			}
		}
		while (la.kind == 116) {
			ImportsStmt();
			while (la.kind == 1 || la.kind == 11) {
				EndOfStmt();
			}
		}
		while (
#line  239 "VBNET.ATG" 
IsGlobalAttrTarget()) {
			GlobalAttributeSection();
			while (la.kind == 1 || la.kind == 11) {
				EndOfStmt();
			}
		}
		while (StartOf(1)) {
			NamespaceMemberDecl();
			while (la.kind == 1 || la.kind == 11) {
				EndOfStmt();
			}
		}
		Expect(0);
	}

	void EndOfStmt() {
		if (la.kind == 1) {
			lexer.NextToken();
		} else if (la.kind == 11) {
			lexer.NextToken();
		} else SynErr(209);
	}

	void OptionStmt() {

#line  244 "VBNET.ATG" 
		INode node = null; bool val = true; 
		Expect(149);

#line  245 "VBNET.ATG" 
		Location startPos = t.Location; 
		if (la.kind == 102) {
			lexer.NextToken();
			if (la.kind == 146 || la.kind == 147) {
				OptionValue(
#line  247 "VBNET.ATG" 
ref val);
			}

#line  248 "VBNET.ATG" 
			node = new OptionDeclaration(OptionType.Explicit, val); 
		} else if (la.kind == 180) {
			lexer.NextToken();
			if (la.kind == 146 || la.kind == 147) {
				OptionValue(
#line  250 "VBNET.ATG" 
ref val);
			}

#line  251 "VBNET.ATG" 
			node = new OptionDeclaration(OptionType.Strict, val); 
		} else if (la.kind == 71) {
			lexer.NextToken();
			if (la.kind == 52) {
				lexer.NextToken();

#line  253 "VBNET.ATG" 
				node = new OptionDeclaration(OptionType.CompareBinary, val); 
			} else if (la.kind == 185) {
				lexer.NextToken();

#line  254 "VBNET.ATG" 
				node = new OptionDeclaration(OptionType.CompareText, val); 
			} else SynErr(210);
		} else if (la.kind == 118) {
			lexer.NextToken();
			if (la.kind == 146 || la.kind == 147) {
				OptionValue(
#line  257 "VBNET.ATG" 
ref val);
			}

#line  258 "VBNET.ATG" 
			node = new OptionDeclaration(OptionType.Infer, val); 
		} else SynErr(211);
		EndOfStmt();

#line  262 "VBNET.ATG" 
		if (node != null) {
		node.StartLocation = startPos;
		node.EndLocation   = t.Location;
		compilationUnit.AddChild(node);
		}
		
	}

	void ImportsStmt() {

#line  285 "VBNET.ATG" 
		List<Using> usings = new List<Using>();
		
		Expect(116);

#line  289 "VBNET.ATG" 
		Location startPos = t.Location;
		Using u;
		
		ImportClause(
#line  292 "VBNET.ATG" 
out u);

#line  292 "VBNET.ATG" 
		if (u != null) { usings.Add(u); } 
		while (la.kind == 12) {
			lexer.NextToken();
			ImportClause(
#line  294 "VBNET.ATG" 
out u);

#line  294 "VBNET.ATG" 
			if (u != null) { usings.Add(u); } 
		}
		EndOfStmt();

#line  298 "VBNET.ATG" 
		UsingDeclaration usingDeclaration = new UsingDeclaration(usings);
		usingDeclaration.StartLocation = startPos;
		usingDeclaration.EndLocation   = t.Location;
		compilationUnit.AddChild(usingDeclaration);
		
	}

	void GlobalAttributeSection() {
		Expect(28);

#line  2181 "VBNET.ATG" 
		Location startPos = t.Location; 
		if (la.kind == 50) {
			lexer.NextToken();
		} else if (la.kind == 131) {
			lexer.NextToken();
		} else SynErr(212);

#line  2183 "VBNET.ATG" 
		string attributeTarget = t.val != null ? t.val.ToLower(System.Globalization.CultureInfo.InvariantCulture) : null;
		List<ASTAttribute> attributes = new List<ASTAttribute>();
		ASTAttribute attribute;
		
		Expect(11);
		Attribute(
#line  2187 "VBNET.ATG" 
out attribute);

#line  2187 "VBNET.ATG" 
		attributes.Add(attribute); 
		while (
#line  2188 "VBNET.ATG" 
NotFinalComma()) {
			if (la.kind == 12) {
				lexer.NextToken();
				if (la.kind == 50) {
					lexer.NextToken();
				} else if (la.kind == 131) {
					lexer.NextToken();
				} else SynErr(213);
				Expect(11);
			}
			Attribute(
#line  2188 "VBNET.ATG" 
out attribute);

#line  2188 "VBNET.ATG" 
			attributes.Add(attribute); 
		}
		if (la.kind == 12) {
			lexer.NextToken();
		}
		Expect(27);
		EndOfStmt();

#line  2193 "VBNET.ATG" 
		AttributeSection section = new AttributeSection {
		AttributeTarget = attributeTarget,
		Attributes = attributes,
		StartLocation = startPos,
		EndLocation = t.EndLocation
		};
		compilationUnit.AddChild(section);
		
	}

	void NamespaceMemberDecl() {

#line  327 "VBNET.ATG" 
		ModifierList m = new ModifierList();
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		string qualident;
		
		if (la.kind == 136) {
			lexer.NextToken();

#line  334 "VBNET.ATG" 
			Location startPos = t.Location;
			
			Qualident(
#line  336 "VBNET.ATG" 
out qualident);

#line  338 "VBNET.ATG" 
			INode node =  new NamespaceDeclaration(qualident);
			node.StartLocation = startPos;
			compilationUnit.AddChild(node);
			compilationUnit.BlockStart(node);
			
			EndOfStmt();
			NamespaceBody();

#line  346 "VBNET.ATG" 
			node.EndLocation = t.Location;
			compilationUnit.BlockEnd();
			
		} else if (StartOf(2)) {
			while (la.kind == 28) {
				AttributeSection(
#line  350 "VBNET.ATG" 
out section);

#line  350 "VBNET.ATG" 
				attributes.Add(section); 
			}
			while (StartOf(3)) {
				TypeModifier(
#line  351 "VBNET.ATG" 
m);
			}
			NonModuleDeclaration(
#line  351 "VBNET.ATG" 
m, attributes);
		} else SynErr(214);
	}

	void OptionValue(
#line  270 "VBNET.ATG" 
ref bool val) {
		if (la.kind == 147) {
			lexer.NextToken();

#line  272 "VBNET.ATG" 
			val = true; 
		} else if (la.kind == 146) {
			lexer.NextToken();

#line  274 "VBNET.ATG" 
			val = false; 
		} else SynErr(215);
	}

	void ImportClause(
#line  305 "VBNET.ATG" 
out Using u) {

#line  307 "VBNET.ATG" 
		string qualident  = null;
		TypeReference aliasedType = null;
		u = null;
		
		Qualident(
#line  311 "VBNET.ATG" 
out qualident);
		if (la.kind == 10) {
			lexer.NextToken();
			TypeName(
#line  312 "VBNET.ATG" 
out aliasedType);
		}

#line  314 "VBNET.ATG" 
		if (qualident != null && qualident.Length > 0) {
		if (aliasedType != null) {
			u = new Using(qualident, aliasedType);
		} else {
			u = new Using(qualident);
		}
		}
		
	}

	void Qualident(
#line  2934 "VBNET.ATG" 
out string qualident) {

#line  2936 "VBNET.ATG" 
		string name;
		qualidentBuilder.Length = 0; 
		
		Identifier();

#line  2940 "VBNET.ATG" 
		qualidentBuilder.Append(t.val); 
		while (
#line  2941 "VBNET.ATG" 
DotAndIdentOrKw()) {
			Expect(16);
			IdentifierOrKeyword(
#line  2941 "VBNET.ATG" 
out name);

#line  2941 "VBNET.ATG" 
			qualidentBuilder.Append('.'); qualidentBuilder.Append(name); 
		}

#line  2943 "VBNET.ATG" 
		qualident = qualidentBuilder.ToString(); 
	}

	void TypeName(
#line  2054 "VBNET.ATG" 
out TypeReference typeref) {

#line  2055 "VBNET.ATG" 
		ArrayList rank = null; 
		NonArrayTypeName(
#line  2057 "VBNET.ATG" 
out typeref, false);
		ArrayTypeModifiers(
#line  2061 "VBNET.ATG" 
out rank);

#line  2062 "VBNET.ATG" 
		if (rank != null && typeref != null) {
		typeref.RankSpecifier = (int[])rank.ToArray(typeof(int));
		}
		
	}

	void NamespaceBody() {
		while (la.kind == 1 || la.kind == 11) {
			EndOfStmt();
		}
		while (StartOf(1)) {
			NamespaceMemberDecl();
			while (la.kind == 1 || la.kind == 11) {
				EndOfStmt();
			}
		}
		Expect(95);
		Expect(136);
		EndOfStmt();
	}

	void AttributeSection(
#line  2258 "VBNET.ATG" 
out AttributeSection section) {

#line  2260 "VBNET.ATG" 
		string attributeTarget = "";List<ASTAttribute> attributes = new List<ASTAttribute>();
		ASTAttribute attribute;
		
		
		Expect(28);

#line  2264 "VBNET.ATG" 
		Location startPos = t.Location; 
		if (
#line  2265 "VBNET.ATG" 
IsLocalAttrTarget()) {
			if (la.kind == 100) {
				lexer.NextToken();

#line  2266 "VBNET.ATG" 
				attributeTarget = "event";
			} else if (la.kind == 169) {
				lexer.NextToken();

#line  2267 "VBNET.ATG" 
				attributeTarget = "return";
			} else {
				Identifier();

#line  2270 "VBNET.ATG" 
				string val = t.val.ToLower(System.Globalization.CultureInfo.InvariantCulture);
				if (val != "field"	|| val != "method" ||
					val != "module" || val != "param"  ||
					val != "property" || val != "type")
				Error("attribute target specifier (event, return, field," +
						"method, module, param, property, or type) expected");
				attributeTarget = t.val;
				
			}
			Expect(11);
		}
		Attribute(
#line  2280 "VBNET.ATG" 
out attribute);

#line  2280 "VBNET.ATG" 
		attributes.Add(attribute); 
		while (
#line  2281 "VBNET.ATG" 
NotFinalComma()) {
			Expect(12);
			Attribute(
#line  2281 "VBNET.ATG" 
out attribute);

#line  2281 "VBNET.ATG" 
			attributes.Add(attribute); 
		}
		if (la.kind == 12) {
			lexer.NextToken();
		}
		Expect(27);

#line  2285 "VBNET.ATG" 
		section = new AttributeSection {
		AttributeTarget = attributeTarget,
		Attributes = attributes,
		StartLocation = startPos,
		EndLocation = t.EndLocation
		};
		
	}

	void TypeModifier(
#line  3014 "VBNET.ATG" 
ModifierList m) {
		switch (la.kind) {
		case 162: {
			lexer.NextToken();

#line  3015 "VBNET.ATG" 
			m.Add(Modifiers.Public, t.Location); 
			break;
		}
		case 161: {
			lexer.NextToken();

#line  3016 "VBNET.ATG" 
			m.Add(Modifiers.Protected, t.Location); 
			break;
		}
		case 106: {
			lexer.NextToken();

#line  3017 "VBNET.ATG" 
			m.Add(Modifiers.Internal, t.Location); 
			break;
		}
		case 159: {
			lexer.NextToken();

#line  3018 "VBNET.ATG" 
			m.Add(Modifiers.Private, t.Location); 
			break;
		}
		case 174: {
			lexer.NextToken();

#line  3019 "VBNET.ATG" 
			m.Add(Modifiers.Static, t.Location); 
			break;
		}
		case 173: {
			lexer.NextToken();

#line  3020 "VBNET.ATG" 
			m.Add(Modifiers.New, t.Location); 
			break;
		}
		case 132: {
			lexer.NextToken();

#line  3021 "VBNET.ATG" 
			m.Add(Modifiers.Abstract, t.Location); 
			break;
		}
		case 142: {
			lexer.NextToken();

#line  3022 "VBNET.ATG" 
			m.Add(Modifiers.Sealed, t.Location); 
			break;
		}
		case 157: {
			lexer.NextToken();

#line  3023 "VBNET.ATG" 
			m.Add(Modifiers.Partial, t.Location); 
			break;
		}
		default: SynErr(216); break;
		}
	}

	void NonModuleDeclaration(
#line  410 "VBNET.ATG" 
ModifierList m, List<AttributeSection> attributes) {

#line  412 "VBNET.ATG" 
		TypeReference typeRef = null;
		List<TypeReference> baseInterfaces = null;
		
		switch (la.kind) {
		case 68: {

#line  415 "VBNET.ATG" 
			m.Check(Modifiers.Classes); 
			lexer.NextToken();

#line  418 "VBNET.ATG" 
			TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
			newType.StartLocation = t.Location;
			compilationUnit.AddChild(newType);
			compilationUnit.BlockStart(newType);
			
			newType.Type       = ClassType.Class;
			
			Identifier();

#line  425 "VBNET.ATG" 
			newType.Name = t.val; 
			TypeParameterList(
#line  426 "VBNET.ATG" 
newType.Templates);
			EndOfStmt();

#line  428 "VBNET.ATG" 
			newType.BodyStartLocation = t.Location; 
			if (la.kind == 119) {
				ClassBaseType(
#line  429 "VBNET.ATG" 
out typeRef);

#line  429 "VBNET.ATG" 
				newType.BaseTypes.Add(typeRef); 
			}
			while (la.kind == 115) {
				TypeImplementsClause(
#line  430 "VBNET.ATG" 
out baseInterfaces);

#line  430 "VBNET.ATG" 
				newType.BaseTypes.AddRange(baseInterfaces); 
			}
			ClassBody(
#line  431 "VBNET.ATG" 
newType);
			Expect(95);
			Expect(68);

#line  432 "VBNET.ATG" 
			newType.EndLocation = t.EndLocation; 
			EndOfStmt();

#line  435 "VBNET.ATG" 
			compilationUnit.BlockEnd();
			
			break;
		}
		case 131: {
			lexer.NextToken();

#line  439 "VBNET.ATG" 
			m.Check(Modifiers.VBModules);
			TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
			compilationUnit.AddChild(newType);
			compilationUnit.BlockStart(newType);
			newType.StartLocation = m.GetDeclarationLocation(t.Location);
			newType.Type = ClassType.Module;
			
			Identifier();

#line  446 "VBNET.ATG" 
			newType.Name = t.val; 
			EndOfStmt();

#line  448 "VBNET.ATG" 
			newType.BodyStartLocation = t.Location; 
			ModuleBody(
#line  449 "VBNET.ATG" 
newType);

#line  451 "VBNET.ATG" 
			compilationUnit.BlockEnd();
			
			break;
		}
		case 182: {
			lexer.NextToken();

#line  455 "VBNET.ATG" 
			m.Check(Modifiers.VBStructures);
			TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
			compilationUnit.AddChild(newType);
			compilationUnit.BlockStart(newType);
			newType.StartLocation = m.GetDeclarationLocation(t.Location);
			newType.Type = ClassType.Struct;
			
			Identifier();

#line  462 "VBNET.ATG" 
			newType.Name = t.val; 
			TypeParameterList(
#line  463 "VBNET.ATG" 
newType.Templates);
			EndOfStmt();

#line  465 "VBNET.ATG" 
			newType.BodyStartLocation = t.Location; 
			while (la.kind == 115) {
				TypeImplementsClause(
#line  466 "VBNET.ATG" 
out baseInterfaces);

#line  466 "VBNET.ATG" 
				newType.BaseTypes.AddRange(baseInterfaces);
			}
			StructureBody(
#line  467 "VBNET.ATG" 
newType);

#line  469 "VBNET.ATG" 
			compilationUnit.BlockEnd();
			
			break;
		}
		case 97: {
			lexer.NextToken();

#line  474 "VBNET.ATG" 
			m.Check(Modifiers.VBEnums);
			TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
			newType.StartLocation = m.GetDeclarationLocation(t.Location);
			compilationUnit.AddChild(newType);
			compilationUnit.BlockStart(newType);
			
			newType.Type = ClassType.Enum;
			
			Identifier();

#line  482 "VBNET.ATG" 
			newType.Name = t.val; 
			if (la.kind == 49) {
				lexer.NextToken();
				NonArrayTypeName(
#line  483 "VBNET.ATG" 
out typeRef, false);

#line  483 "VBNET.ATG" 
				newType.BaseTypes.Add(typeRef); 
			}
			EndOfStmt();

#line  485 "VBNET.ATG" 
			newType.BodyStartLocation = t.Location; 
			EnumBody(
#line  486 "VBNET.ATG" 
newType);

#line  488 "VBNET.ATG" 
			compilationUnit.BlockEnd();
			
			break;
		}
		case 121: {
			lexer.NextToken();

#line  493 "VBNET.ATG" 
			m.Check(Modifiers.VBInterfacs);
			TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
			newType.StartLocation = m.GetDeclarationLocation(t.Location);
			compilationUnit.AddChild(newType);
			compilationUnit.BlockStart(newType);
			newType.Type = ClassType.Interface;
			
			Identifier();

#line  500 "VBNET.ATG" 
			newType.Name = t.val; 
			TypeParameterList(
#line  501 "VBNET.ATG" 
newType.Templates);
			EndOfStmt();

#line  503 "VBNET.ATG" 
			newType.BodyStartLocation = t.Location; 
			while (la.kind == 119) {
				InterfaceBase(
#line  504 "VBNET.ATG" 
out baseInterfaces);

#line  504 "VBNET.ATG" 
				newType.BaseTypes.AddRange(baseInterfaces); 
			}
			InterfaceBody(
#line  505 "VBNET.ATG" 
newType);

#line  507 "VBNET.ATG" 
			compilationUnit.BlockEnd();
			
			break;
		}
		case 87: {
			lexer.NextToken();

#line  512 "VBNET.ATG" 
			m.Check(Modifiers.VBDelegates);
			DelegateDeclaration delegateDeclr = new DelegateDeclaration(m.Modifier, attributes);
			delegateDeclr.ReturnType = new TypeReference("", "System.Void");
			delegateDeclr.StartLocation = m.GetDeclarationLocation(t.Location);
			List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
			
			if (la.kind == 183) {
				lexer.NextToken();
				Identifier();

#line  519 "VBNET.ATG" 
				delegateDeclr.Name = t.val; 
				TypeParameterList(
#line  520 "VBNET.ATG" 
delegateDeclr.Templates);
				if (la.kind == 25) {
					lexer.NextToken();
					if (StartOf(4)) {
						FormalParameterList(
#line  521 "VBNET.ATG" 
p);
					}
					Expect(26);

#line  521 "VBNET.ATG" 
					delegateDeclr.Parameters = p; 
				}
			} else if (la.kind == 107) {
				lexer.NextToken();
				Identifier();

#line  523 "VBNET.ATG" 
				delegateDeclr.Name = t.val; 
				TypeParameterList(
#line  524 "VBNET.ATG" 
delegateDeclr.Templates);
				if (la.kind == 25) {
					lexer.NextToken();
					if (StartOf(4)) {
						FormalParameterList(
#line  525 "VBNET.ATG" 
p);
					}
					Expect(26);

#line  525 "VBNET.ATG" 
					delegateDeclr.Parameters = p; 
				}
				if (la.kind == 49) {
					lexer.NextToken();

#line  526 "VBNET.ATG" 
					TypeReference type; 
					TypeName(
#line  526 "VBNET.ATG" 
out type);

#line  526 "VBNET.ATG" 
					delegateDeclr.ReturnType = type; 
				}
			} else SynErr(217);

#line  528 "VBNET.ATG" 
			delegateDeclr.EndLocation = t.EndLocation; 
			EndOfStmt();

#line  531 "VBNET.ATG" 
			compilationUnit.AddChild(delegateDeclr);
			
			break;
		}
		default: SynErr(218); break;
		}
	}

	void TypeParameterList(
#line  355 "VBNET.ATG" 
List<TemplateDefinition> templates) {

#line  357 "VBNET.ATG" 
		TemplateDefinition template;
		
		if (
#line  360 "VBNET.ATG" 
la.kind == Tokens.OpenParenthesis && Peek(1).kind == Tokens.Of) {
			lexer.NextToken();
			Expect(145);
			TypeParameter(
#line  361 "VBNET.ATG" 
out template);

#line  363 "VBNET.ATG" 
			if (template != null) templates.Add(template);
			
			while (la.kind == 12) {
				lexer.NextToken();
				TypeParameter(
#line  366 "VBNET.ATG" 
out template);

#line  368 "VBNET.ATG" 
				if (template != null) templates.Add(template);
				
			}
			Expect(26);
		}
	}

	void TypeParameter(
#line  376 "VBNET.ATG" 
out TemplateDefinition template) {
		Identifier();

#line  378 "VBNET.ATG" 
		template = new TemplateDefinition(t.val, null); 
		if (la.kind == 49) {
			TypeParameterConstraints(
#line  379 "VBNET.ATG" 
template);
		}
	}

	void Identifier() {
		switch (la.kind) {
		case 2: {
			lexer.NextToken();
			break;
		}
		case 185: {
			lexer.NextToken();
			break;
		}
		case 52: {
			lexer.NextToken();
			break;
		}
		case 71: {
			lexer.NextToken();
			break;
		}
		case 82: {
			lexer.NextToken();
			break;
		}
		case 50: {
			lexer.NextToken();
			break;
		}
		case 48: {
			lexer.NextToken();
			break;
		}
		case 51: {
			lexer.NextToken();
			break;
		}
		case 158: {
			lexer.NextToken();
			break;
		}
		case 195: {
			lexer.NextToken();
			break;
		}
		case 196: {
			lexer.NextToken();
			break;
		}
		case 146: {
			lexer.NextToken();
			break;
		}
		case 102: {
			lexer.NextToken();
			break;
		}
		default: SynErr(219); break;
		}
	}

	void TypeParameterConstraints(
#line  383 "VBNET.ATG" 
TemplateDefinition template) {

#line  385 "VBNET.ATG" 
		TypeReference constraint;
		
		Expect(49);
		if (la.kind == 23) {
			lexer.NextToken();
			TypeParameterConstraint(
#line  391 "VBNET.ATG" 
out constraint);

#line  391 "VBNET.ATG" 
			if (constraint != null) { template.Bases.Add(constraint); } 
			while (la.kind == 12) {
				lexer.NextToken();
				TypeParameterConstraint(
#line  394 "VBNET.ATG" 
out constraint);

#line  394 "VBNET.ATG" 
				if (constraint != null) { template.Bases.Add(constraint); } 
			}
			Expect(24);
		} else if (StartOf(5)) {
			TypeParameterConstraint(
#line  397 "VBNET.ATG" 
out constraint);

#line  397 "VBNET.ATG" 
			if (constraint != null) { template.Bases.Add(constraint); } 
		} else SynErr(220);
	}

	void TypeParameterConstraint(
#line  401 "VBNET.ATG" 
out TypeReference constraint) {

#line  402 "VBNET.ATG" 
		constraint = null; 
		if (la.kind == 68) {
			lexer.NextToken();

#line  403 "VBNET.ATG" 
			constraint = TypeReference.ClassConstraint; 
		} else if (la.kind == 182) {
			lexer.NextToken();

#line  404 "VBNET.ATG" 
			constraint = TypeReference.StructConstraint; 
		} else if (la.kind == 138) {
			lexer.NextToken();

#line  405 "VBNET.ATG" 
			constraint = TypeReference.NewConstraint; 
		} else if (StartOf(6)) {
			TypeName(
#line  406 "VBNET.ATG" 
out constraint);
		} else SynErr(221);
	}

	void ClassBaseType(
#line  752 "VBNET.ATG" 
out TypeReference typeRef) {

#line  754 "VBNET.ATG" 
		typeRef = null;
		
		Expect(119);
		TypeName(
#line  757 "VBNET.ATG" 
out typeRef);
		EndOfStmt();
	}

	void TypeImplementsClause(
#line  1552 "VBNET.ATG" 
out List<TypeReference> baseInterfaces) {

#line  1554 "VBNET.ATG" 
		baseInterfaces = new List<TypeReference>();
		TypeReference type = null;
		
		Expect(115);
		TypeName(
#line  1557 "VBNET.ATG" 
out type);

#line  1559 "VBNET.ATG" 
		baseInterfaces.Add(type);
		
		while (la.kind == 12) {
			lexer.NextToken();
			TypeName(
#line  1562 "VBNET.ATG" 
out type);

#line  1563 "VBNET.ATG" 
			baseInterfaces.Add(type); 
		}
		EndOfStmt();
	}

	void ClassBody(
#line  545 "VBNET.ATG" 
TypeDeclaration newType) {

#line  546 "VBNET.ATG" 
		AttributeSection section; 
		while (la.kind == 1 || la.kind == 11) {
			EndOfStmt();
		}
		while (StartOf(7)) {

#line  549 "VBNET.ATG" 
			List<AttributeSection> attributes = new List<AttributeSection>();
			ModifierList m = new ModifierList();
			
			while (la.kind == 28) {
				AttributeSection(
#line  552 "VBNET.ATG" 
out section);

#line  552 "VBNET.ATG" 
				attributes.Add(section); 
			}
			while (StartOf(8)) {
				MemberModifier(
#line  553 "VBNET.ATG" 
m);
			}
			ClassMemberDecl(
#line  554 "VBNET.ATG" 
m, attributes);
			while (la.kind == 1 || la.kind == 11) {
				EndOfStmt();
			}
		}
	}

	void ModuleBody(
#line  576 "VBNET.ATG" 
TypeDeclaration newType) {

#line  577 "VBNET.ATG" 
		AttributeSection section; 
		while (la.kind == 1 || la.kind == 11) {
			EndOfStmt();
		}
		while (StartOf(7)) {

#line  580 "VBNET.ATG" 
			List<AttributeSection> attributes = new List<AttributeSection>();
			ModifierList m = new ModifierList();
			
			while (la.kind == 28) {
				AttributeSection(
#line  583 "VBNET.ATG" 
out section);

#line  583 "VBNET.ATG" 
				attributes.Add(section); 
			}
			while (StartOf(8)) {
				MemberModifier(
#line  584 "VBNET.ATG" 
m);
			}
			ClassMemberDecl(
#line  585 "VBNET.ATG" 
m, attributes);
			while (la.kind == 1 || la.kind == 11) {
				EndOfStmt();
			}
		}
		Expect(95);
		Expect(131);

#line  588 "VBNET.ATG" 
		newType.EndLocation = t.EndLocation; 
		EndOfStmt();
	}

	void StructureBody(
#line  559 "VBNET.ATG" 
TypeDeclaration newType) {

#line  560 "VBNET.ATG" 
		AttributeSection section; 
		while (la.kind == 1 || la.kind == 11) {
			EndOfStmt();
		}
		while (StartOf(7)) {

#line  563 "VBNET.ATG" 
			List<AttributeSection> attributes = new List<AttributeSection>();
			ModifierList m = new ModifierList();
			
			while (la.kind == 28) {
				AttributeSection(
#line  566 "VBNET.ATG" 
out section);

#line  566 "VBNET.ATG" 
				attributes.Add(section); 
			}
			while (StartOf(8)) {
				MemberModifier(
#line  567 "VBNET.ATG" 
m);
			}
			StructureMemberDecl(
#line  568 "VBNET.ATG" 
m, attributes);
			while (la.kind == 1 || la.kind == 11) {
				EndOfStmt();
			}
		}
		Expect(95);
		Expect(182);

#line  571 "VBNET.ATG" 
		newType.EndLocation = t.EndLocation; 
		EndOfStmt();
	}

	void NonArrayTypeName(
#line  2080 "VBNET.ATG" 
out TypeReference typeref, bool canBeUnbound) {

#line  2082 "VBNET.ATG" 
		string name;
		typeref = null;
		bool isGlobal = false;
		
		if (StartOf(9)) {
			if (la.kind == 110) {
				lexer.NextToken();
				Expect(16);

#line  2087 "VBNET.ATG" 
				isGlobal = true; 
			}
			QualIdentAndTypeArguments(
#line  2088 "VBNET.ATG" 
out typeref, canBeUnbound);

#line  2089 "VBNET.ATG" 
			typeref.IsGlobal = isGlobal; 
			while (la.kind == 16) {
				lexer.NextToken();

#line  2090 "VBNET.ATG" 
				TypeReference nestedTypeRef; 
				QualIdentAndTypeArguments(
#line  2091 "VBNET.ATG" 
out nestedTypeRef, canBeUnbound);

#line  2092 "VBNET.ATG" 
				typeref = new InnerClassTypeReference(typeref, nestedTypeRef.Type, nestedTypeRef.GenericTypes); 
			}
		} else if (la.kind == 144) {
			lexer.NextToken();

#line  2095 "VBNET.ATG" 
			typeref = new TypeReference("System.Object"); 
			if (la.kind == 21) {
				lexer.NextToken();

#line  2099 "VBNET.ATG" 
				List<TypeReference> typeArguments = new List<TypeReference>(1);
				if (typeref != null) typeArguments.Add(typeref);
				typeref = new TypeReference("System.Nullable", typeArguments);
				
			}
		} else if (StartOf(10)) {
			PrimitiveTypeName(
#line  2105 "VBNET.ATG" 
out name);

#line  2105 "VBNET.ATG" 
			typeref = new TypeReference(name); 
			if (la.kind == 21) {
				lexer.NextToken();

#line  2109 "VBNET.ATG" 
				List<TypeReference> typeArguments = new List<TypeReference>(1);
				if (typeref != null) typeArguments.Add(typeref);
				typeref = new TypeReference("System.Nullable", typeArguments);
				
			}
		} else SynErr(222);
	}

	void EnumBody(
#line  592 "VBNET.ATG" 
TypeDeclaration newType) {

#line  593 "VBNET.ATG" 
		FieldDeclaration f; 
		while (la.kind == 1 || la.kind == 11) {
			EndOfStmt();
		}
		while (StartOf(11)) {
			EnumMemberDecl(
#line  596 "VBNET.ATG" 
out f);

#line  598 "VBNET.ATG" 
			SetParent(f.Fields, f);
			compilationUnit.AddChild(f);
			
			while (la.kind == 1 || la.kind == 11) {
				EndOfStmt();
			}
		}
		Expect(95);
		Expect(97);

#line  603 "VBNET.ATG" 
		newType.EndLocation = t.EndLocation; 
		EndOfStmt();
	}

	void InterfaceBase(
#line  1537 "VBNET.ATG" 
out List<TypeReference> bases) {

#line  1539 "VBNET.ATG" 
		TypeReference type;
		bases = new List<TypeReference>();
		
		Expect(119);
		TypeName(
#line  1543 "VBNET.ATG" 
out type);

#line  1543 "VBNET.ATG" 
		bases.Add(type); 
		while (la.kind == 12) {
			lexer.NextToken();
			TypeName(
#line  1546 "VBNET.ATG" 
out type);

#line  1546 "VBNET.ATG" 
			bases.Add(type); 
		}
		EndOfStmt();
	}

	void InterfaceBody(
#line  607 "VBNET.ATG" 
TypeDeclaration newType) {
		while (la.kind == 1 || la.kind == 11) {
			EndOfStmt();
		}
		while (StartOf(12)) {
			InterfaceMemberDecl();
			while (la.kind == 1 || la.kind == 11) {
				EndOfStmt();
			}
		}
		Expect(95);
		Expect(121);

#line  613 "VBNET.ATG" 
		newType.EndLocation = t.EndLocation; 
		EndOfStmt();
	}

	void FormalParameterList(
#line  2295 "VBNET.ATG" 
List<ParameterDeclarationExpression> parameter) {

#line  2296 "VBNET.ATG" 
		ParameterDeclarationExpression p; 
		FormalParameter(
#line  2298 "VBNET.ATG" 
out p);

#line  2298 "VBNET.ATG" 
		if (p != null) parameter.Add(p); 
		while (la.kind == 12) {
			lexer.NextToken();
			FormalParameter(
#line  2300 "VBNET.ATG" 
out p);

#line  2300 "VBNET.ATG" 
			if (p != null) parameter.Add(p); 
		}
	}

	void MemberModifier(
#line  3026 "VBNET.ATG" 
ModifierList m) {
		switch (la.kind) {
		case 132: {
			lexer.NextToken();

#line  3027 "VBNET.ATG" 
			m.Add(Modifiers.Abstract, t.Location);
			break;
		}
		case 86: {
			lexer.NextToken();

#line  3028 "VBNET.ATG" 
			m.Add(Modifiers.Default, t.Location);
			break;
		}
		case 106: {
			lexer.NextToken();

#line  3029 "VBNET.ATG" 
			m.Add(Modifiers.Internal, t.Location);
			break;
		}
		case 173: {
			lexer.NextToken();

#line  3030 "VBNET.ATG" 
			m.Add(Modifiers.New, t.Location);
			break;
		}
		case 155: {
			lexer.NextToken();

#line  3031 "VBNET.ATG" 
			m.Add(Modifiers.Override, t.Location);
			break;
		}
		case 133: {
			lexer.NextToken();

#line  3032 "VBNET.ATG" 
			m.Add(Modifiers.Abstract, t.Location);
			break;
		}
		case 159: {
			lexer.NextToken();

#line  3033 "VBNET.ATG" 
			m.Add(Modifiers.Private, t.Location);
			break;
		}
		case 161: {
			lexer.NextToken();

#line  3034 "VBNET.ATG" 
			m.Add(Modifiers.Protected, t.Location);
			break;
		}
		case 162: {
			lexer.NextToken();

#line  3035 "VBNET.ATG" 
			m.Add(Modifiers.Public, t.Location);
			break;
		}
		case 142: {
			lexer.NextToken();

#line  3036 "VBNET.ATG" 
			m.Add(Modifiers.Sealed, t.Location);
			break;
		}
		case 143: {
			lexer.NextToken();

#line  3037 "VBNET.ATG" 
			m.Add(Modifiers.Sealed, t.Location);
			break;
		}
		case 174: {
			lexer.NextToken();

#line  3038 "VBNET.ATG" 
			m.Add(Modifiers.Static, t.Location);
			break;
		}
		case 154: {
			lexer.NextToken();

#line  3039 "VBNET.ATG" 
			m.Add(Modifiers.Virtual, t.Location);
			break;
		}
		case 153: {
			lexer.NextToken();

#line  3040 "VBNET.ATG" 
			m.Add(Modifiers.Overloads, t.Location);
			break;
		}
		case 164: {
			lexer.NextToken();

#line  3041 "VBNET.ATG" 
			m.Add(Modifiers.ReadOnly, t.Location);
			break;
		}
		case 206: {
			lexer.NextToken();

#line  3042 "VBNET.ATG" 
			m.Add(Modifiers.WriteOnly, t.Location);
			break;
		}
		case 205: {
			lexer.NextToken();

#line  3043 "VBNET.ATG" 
			m.Add(Modifiers.WithEvents, t.Location);
			break;
		}
		case 88: {
			lexer.NextToken();

#line  3044 "VBNET.ATG" 
			m.Add(Modifiers.Dim, t.Location);
			break;
		}
		case 157: {
			lexer.NextToken();

#line  3045 "VBNET.ATG" 
			m.Add(Modifiers.Partial, t.Location);
			break;
		}
		default: SynErr(223); break;
		}
	}

	void ClassMemberDecl(
#line  748 "VBNET.ATG" 
ModifierList m, List<AttributeSection> attributes) {
		StructureMemberDecl(
#line  749 "VBNET.ATG" 
m, attributes);
	}

	void StructureMemberDecl(
#line  762 "VBNET.ATG" 
ModifierList m, List<AttributeSection> attributes) {

#line  764 "VBNET.ATG" 
		TypeReference type = null;
		List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
		Statement stmt = null;
		List<VariableDeclaration> variableDeclarators = new List<VariableDeclaration>();
		List<TemplateDefinition> templates = new List<TemplateDefinition>();
		
		switch (la.kind) {
		case 68: case 87: case 97: case 121: case 131: case 182: {
			NonModuleDeclaration(
#line  771 "VBNET.ATG" 
m, attributes);
			break;
		}
		case 183: {
			lexer.NextToken();

#line  775 "VBNET.ATG" 
			Location startPos = t.Location;
			
			if (StartOf(13)) {

#line  779 "VBNET.ATG" 
				string name = String.Empty;
				MethodDeclaration methodDeclaration; List<string> handlesClause = null;
				List<InterfaceImplementation> implementsClause = null;
				
				Identifier();

#line  785 "VBNET.ATG" 
				name = t.val;
				m.Check(Modifiers.VBMethods);
				
				TypeParameterList(
#line  788 "VBNET.ATG" 
templates);
				if (la.kind == 25) {
					lexer.NextToken();
					if (StartOf(4)) {
						FormalParameterList(
#line  789 "VBNET.ATG" 
p);
					}
					Expect(26);
				}
				if (la.kind == 113 || la.kind == 115) {
					if (la.kind == 115) {
						ImplementsClause(
#line  792 "VBNET.ATG" 
out implementsClause);
					} else {
						HandlesClause(
#line  794 "VBNET.ATG" 
out handlesClause);
					}
				}

#line  797 "VBNET.ATG" 
				Location endLocation = t.EndLocation; 
				if (
#line  800 "VBNET.ATG" 
IsMustOverride(m)) {
					EndOfStmt();

#line  803 "VBNET.ATG" 
					methodDeclaration = new MethodDeclaration {
					Name = name, Modifier = m.Modifier, Parameters = p, Attributes = attributes,
					StartLocation = m.GetDeclarationLocation(startPos), EndLocation = endLocation,
					TypeReference = new TypeReference("", "System.Void"),
					Templates = templates,
					HandlesClause = handlesClause,
					InterfaceImplementations = implementsClause
					};
					compilationUnit.AddChild(methodDeclaration);
					
				} else if (la.kind == 1) {
					lexer.NextToken();

#line  816 "VBNET.ATG" 
					methodDeclaration = new MethodDeclaration {
					Name = name, Modifier = m.Modifier, Parameters = p, Attributes = attributes,
					StartLocation = m.GetDeclarationLocation(startPos), EndLocation = endLocation,
					TypeReference = new TypeReference("", "System.Void"),
					Templates = templates,
					HandlesClause = handlesClause,
					InterfaceImplementations = implementsClause
					};
					compilationUnit.AddChild(methodDeclaration);
					

#line  827 "VBNET.ATG" 
					if (ParseMethodBodies) { 
					Block(
#line  828 "VBNET.ATG" 
out stmt);
					Expect(95);
					Expect(183);

#line  830 "VBNET.ATG" 
					} else {
					// don't parse method body
					lexer.SkipCurrentBlock(Tokens.Sub); stmt = new BlockStatement();
					  }
					

#line  836 "VBNET.ATG" 
					methodDeclaration.Body  = (BlockStatement)stmt; 

#line  837 "VBNET.ATG" 
					methodDeclaration.Body.EndLocation = t.EndLocation; 
					EndOfStmt();
				} else SynErr(224);
			} else if (la.kind == 138) {
				lexer.NextToken();
				if (la.kind == 25) {
					lexer.NextToken();
					if (StartOf(4)) {
						FormalParameterList(
#line  841 "VBNET.ATG" 
p);
					}
					Expect(26);
				}

#line  842 "VBNET.ATG" 
				m.Check(Modifiers.Constructors); 

#line  843 "VBNET.ATG" 
				Location constructorEndLocation = t.EndLocation; 
				Expect(1);

#line  846 "VBNET.ATG" 
				if (ParseMethodBodies) { 
				Block(
#line  847 "VBNET.ATG" 
out stmt);
				Expect(95);
				Expect(183);

#line  849 "VBNET.ATG" 
				} else {
				// don't parse method body
				lexer.SkipCurrentBlock(Tokens.Sub); stmt = new BlockStatement();
				  }
				

#line  855 "VBNET.ATG" 
				Location endLocation = t.EndLocation; 
				EndOfStmt();

#line  858 "VBNET.ATG" 
				ConstructorDeclaration cd = new ConstructorDeclaration("New", m.Modifier, p, attributes); 
				cd.StartLocation = m.GetDeclarationLocation(startPos);
				cd.EndLocation   = constructorEndLocation;
				cd.Body = (BlockStatement)stmt;
				cd.Body.EndLocation   = endLocation;
				compilationUnit.AddChild(cd);
				
			} else SynErr(225);
			break;
		}
		case 107: {
			lexer.NextToken();

#line  870 "VBNET.ATG" 
			m.Check(Modifiers.VBMethods);
			string name = String.Empty;
			Location startPos = t.Location;
			MethodDeclaration methodDeclaration;List<string> handlesClause = null;
			List<InterfaceImplementation> implementsClause = null;
			AttributeSection returnTypeAttributeSection = null;
			
			Identifier();

#line  877 "VBNET.ATG" 
			name = t.val; 
			TypeParameterList(
#line  878 "VBNET.ATG" 
templates);
			if (la.kind == 25) {
				lexer.NextToken();
				if (StartOf(4)) {
					FormalParameterList(
#line  879 "VBNET.ATG" 
p);
				}
				Expect(26);
			}
			if (la.kind == 49) {
				lexer.NextToken();
				while (la.kind == 28) {
					AttributeSection(
#line  880 "VBNET.ATG" 
out returnTypeAttributeSection);
				}
				TypeName(
#line  880 "VBNET.ATG" 
out type);
			}

#line  882 "VBNET.ATG" 
			if(type == null) {
			type = new TypeReference("System.Object");
			}
			
			if (la.kind == 113 || la.kind == 115) {
				if (la.kind == 115) {
					ImplementsClause(
#line  888 "VBNET.ATG" 
out implementsClause);
				} else {
					HandlesClause(
#line  890 "VBNET.ATG" 
out handlesClause);
				}
			}
			if (
#line  895 "VBNET.ATG" 
IsMustOverride(m)) {
				EndOfStmt();

#line  898 "VBNET.ATG" 
				methodDeclaration = new MethodDeclaration {
				Name = name, Modifier = m.Modifier, TypeReference = type,
				Parameters = p, Attributes = attributes,
				StartLocation = m.GetDeclarationLocation(startPos),
				EndLocation   = t.EndLocation,
				HandlesClause = handlesClause,
				Templates     = templates,
				InterfaceImplementations = implementsClause
				};
				if (returnTypeAttributeSection != null) {
					returnTypeAttributeSection.AttributeTarget = "return";
					methodDeclaration.Attributes.Add(returnTypeAttributeSection);
				}
				compilationUnit.AddChild(methodDeclaration);
				
			} else if (la.kind == 1) {
				lexer.NextToken();

#line  916 "VBNET.ATG" 
				methodDeclaration = new MethodDeclaration {
				Name = name, Modifier = m.Modifier, TypeReference = type,
				Parameters = p, Attributes = attributes,
				StartLocation = m.GetDeclarationLocation(startPos),
				EndLocation   = t.EndLocation,
				Templates     = templates,
				HandlesClause = handlesClause,
				InterfaceImplementations = implementsClause
				};
				if (returnTypeAttributeSection != null) {
					returnTypeAttributeSection.AttributeTarget = "return";
					methodDeclaration.Attributes.Add(returnTypeAttributeSection);
				}
				
				compilationUnit.AddChild(methodDeclaration);
				
				if (ParseMethodBodies) { 
				Block(
#line  933 "VBNET.ATG" 
out stmt);
				Expect(95);
				Expect(107);

#line  935 "VBNET.ATG" 
				} else {
				// don't parse method body
				lexer.SkipCurrentBlock(Tokens.Function); stmt = new BlockStatement();
				}
				methodDeclaration.Body = (BlockStatement)stmt;
				methodDeclaration.Body.StartLocation = methodDeclaration.EndLocation;
				methodDeclaration.Body.EndLocation   = t.EndLocation;
				
				EndOfStmt();
			} else SynErr(226);
			break;
		}
		case 85: {
			lexer.NextToken();

#line  949 "VBNET.ATG" 
			m.Check(Modifiers.VBExternalMethods);
			Location startPos = t.Location;
			CharsetModifier charsetModifer = CharsetModifier.None;
			string library = String.Empty;
			string alias = null;
			string name = String.Empty;
			
			if (StartOf(14)) {
				Charset(
#line  956 "VBNET.ATG" 
out charsetModifer);
			}
			if (la.kind == 183) {
				lexer.NextToken();
				Identifier();

#line  959 "VBNET.ATG" 
				name = t.val; 
				Expect(125);
				Expect(3);

#line  960 "VBNET.ATG" 
				library = t.literalValue as string; 
				if (la.kind == 45) {
					lexer.NextToken();
					Expect(3);

#line  961 "VBNET.ATG" 
					alias = t.literalValue as string; 
				}
				if (la.kind == 25) {
					lexer.NextToken();
					if (StartOf(4)) {
						FormalParameterList(
#line  962 "VBNET.ATG" 
p);
					}
					Expect(26);
				}
				EndOfStmt();

#line  965 "VBNET.ATG" 
				DeclareDeclaration declareDeclaration = new DeclareDeclaration(name, m.Modifier, null, p, attributes, library, alias, charsetModifer);
				declareDeclaration.StartLocation = m.GetDeclarationLocation(startPos);
				declareDeclaration.EndLocation   = t.EndLocation;
				compilationUnit.AddChild(declareDeclaration);
				
			} else if (la.kind == 107) {
				lexer.NextToken();
				Identifier();

#line  972 "VBNET.ATG" 
				name = t.val; 
				Expect(125);
				Expect(3);

#line  973 "VBNET.ATG" 
				library = t.literalValue as string; 
				if (la.kind == 45) {
					lexer.NextToken();
					Expect(3);

#line  974 "VBNET.ATG" 
					alias = t.literalValue as string; 
				}
				if (la.kind == 25) {
					lexer.NextToken();
					if (StartOf(4)) {
						FormalParameterList(
#line  975 "VBNET.ATG" 
p);
					}
					Expect(26);
				}
				if (la.kind == 49) {
					lexer.NextToken();
					TypeName(
#line  976 "VBNET.ATG" 
out type);
				}
				EndOfStmt();

#line  979 "VBNET.ATG" 
				DeclareDeclaration declareDeclaration = new DeclareDeclaration(name, m.Modifier, type, p, attributes, library, alias, charsetModifer);
				declareDeclaration.StartLocation = m.GetDeclarationLocation(startPos);
				declareDeclaration.EndLocation   = t.EndLocation;
				compilationUnit.AddChild(declareDeclaration);
				
			} else SynErr(227);
			break;
		}
		case 100: {
			lexer.NextToken();

#line  989 "VBNET.ATG" 
			m.Check(Modifiers.VBEvents);
			Location startPos = t.Location;
			EventDeclaration eventDeclaration;
			string name = String.Empty;
			List<InterfaceImplementation> implementsClause = null;
			
			Identifier();

#line  995 "VBNET.ATG" 
			name= t.val; 
			if (la.kind == 49) {
				lexer.NextToken();
				TypeName(
#line  997 "VBNET.ATG" 
out type);
			} else if (StartOf(15)) {
				if (la.kind == 25) {
					lexer.NextToken();
					if (StartOf(4)) {
						FormalParameterList(
#line  999 "VBNET.ATG" 
p);
					}
					Expect(26);
				}
			} else SynErr(228);
			if (la.kind == 115) {
				ImplementsClause(
#line  1001 "VBNET.ATG" 
out implementsClause);
			}

#line  1003 "VBNET.ATG" 
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
		case 2: case 48: case 50: case 51: case 52: case 71: case 102: case 118: case 146: case 158: case 185: case 195: case 196: {

#line  1013 "VBNET.ATG" 
			Location startPos = t.Location; 

#line  1015 "VBNET.ATG" 
			m.Check(Modifiers.Fields);
			FieldDeclaration fd = new FieldDeclaration(attributes, null, m.Modifier);
			fd.StartLocation = m.GetDeclarationLocation(startPos); 
			
			IdentifierForFieldDeclaration();

#line  1019 "VBNET.ATG" 
			string name = t.val; 
			VariableDeclaratorPartAfterIdentifier(
#line  1020 "VBNET.ATG" 
variableDeclarators, name);
			while (la.kind == 12) {
				lexer.NextToken();
				VariableDeclarator(
#line  1021 "VBNET.ATG" 
variableDeclarators);
			}
			EndOfStmt();

#line  1024 "VBNET.ATG" 
			fd.EndLocation = t.EndLocation;
			fd.Fields = variableDeclarators;
			SetParent(variableDeclarators, fd);
			compilationUnit.AddChild(fd);
			
			break;
		}
		case 72: {

#line  1030 "VBNET.ATG" 
			m.Check(Modifiers.Fields); 
			lexer.NextToken();

#line  1031 "VBNET.ATG" 
			m.Add(Modifiers.Const, t.Location);  

#line  1033 "VBNET.ATG" 
			FieldDeclaration fd = new FieldDeclaration(attributes, type, m.Modifier);
			fd.StartLocation = m.GetDeclarationLocation(t.Location);
			List<VariableDeclaration> constantDeclarators = new List<VariableDeclaration>();
			
			ConstantDeclarator(
#line  1037 "VBNET.ATG" 
constantDeclarators);
			while (la.kind == 12) {
				lexer.NextToken();
				ConstantDeclarator(
#line  1038 "VBNET.ATG" 
constantDeclarators);
			}

#line  1040 "VBNET.ATG" 
			fd.Fields = constantDeclarators;
			fd.EndLocation = t.Location;
			
			EndOfStmt();

#line  1045 "VBNET.ATG" 
			fd.EndLocation = t.EndLocation;
			compilationUnit.AddChild(fd);
			
			break;
		}
		case 160: {
			lexer.NextToken();

#line  1051 "VBNET.ATG" 
			m.Check(Modifiers.VBProperties);
			Location startPos = t.Location;
			List<InterfaceImplementation> implementsClause = null;
			
			Identifier();

#line  1055 "VBNET.ATG" 
			string propertyName = t.val; 
			if (la.kind == 25) {
				lexer.NextToken();
				if (StartOf(4)) {
					FormalParameterList(
#line  1056 "VBNET.ATG" 
p);
				}
				Expect(26);
			}
			if (la.kind == 49) {
				lexer.NextToken();
				TypeName(
#line  1057 "VBNET.ATG" 
out type);
			}

#line  1059 "VBNET.ATG" 
			if(type == null) {
			type = new TypeReference("System.Object");
			}
			
			if (la.kind == 115) {
				ImplementsClause(
#line  1063 "VBNET.ATG" 
out implementsClause);
			}
			EndOfStmt();
			if (
#line  1067 "VBNET.ATG" 
IsMustOverride(m)) {

#line  1069 "VBNET.ATG" 
				PropertyDeclaration pDecl = new PropertyDeclaration(propertyName, type, m.Modifier, attributes);
				pDecl.StartLocation = m.GetDeclarationLocation(startPos);
				pDecl.EndLocation   = t.Location;
				pDecl.TypeReference = type;
				pDecl.InterfaceImplementations = implementsClause;
				pDecl.Parameters = p;
				compilationUnit.AddChild(pDecl);
				
			} else if (StartOf(16)) {

#line  1079 "VBNET.ATG" 
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
#line  1089 "VBNET.ATG" 
out getRegion, out setRegion);
				Expect(95);
				Expect(160);
				EndOfStmt();

#line  1093 "VBNET.ATG" 
				pDecl.GetRegion = getRegion;
				pDecl.SetRegion = setRegion;
				pDecl.BodyEnd = t.EndLocation;
				compilationUnit.AddChild(pDecl);
				
			} else SynErr(229);
			break;
		}
		case 82: {
			lexer.NextToken();

#line  1100 "VBNET.ATG" 
			Location startPos = t.Location; 
			Expect(100);

#line  1102 "VBNET.ATG" 
			m.Check(Modifiers.VBCustomEvents);
			EventAddRemoveRegion eventAccessorDeclaration;
			EventAddRegion addHandlerAccessorDeclaration = null;
			EventRemoveRegion removeHandlerAccessorDeclaration = null;
			EventRaiseRegion raiseEventAccessorDeclaration = null;
			List<InterfaceImplementation> implementsClause = null;
			
			Identifier();

#line  1109 "VBNET.ATG" 
			string customEventName = t.val; 
			Expect(49);
			TypeName(
#line  1110 "VBNET.ATG" 
out type);
			if (la.kind == 115) {
				ImplementsClause(
#line  1111 "VBNET.ATG" 
out implementsClause);
			}
			EndOfStmt();
			while (StartOf(17)) {
				EventAccessorDeclaration(
#line  1114 "VBNET.ATG" 
out eventAccessorDeclaration);

#line  1116 "VBNET.ATG" 
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
			Expect(95);
			Expect(100);
			EndOfStmt();

#line  1132 "VBNET.ATG" 
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
		case 137: case 148: case 203: {

#line  1158 "VBNET.ATG" 
			ConversionType opConversionType = ConversionType.None; 
			if (la.kind == 137 || la.kind == 203) {
				if (la.kind == 203) {
					lexer.NextToken();

#line  1159 "VBNET.ATG" 
					opConversionType = ConversionType.Implicit; 
				} else {
					lexer.NextToken();

#line  1160 "VBNET.ATG" 
					opConversionType = ConversionType.Explicit;
				}
			}
			Expect(148);

#line  1163 "VBNET.ATG" 
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
#line  1173 "VBNET.ATG" 
out operatorType);
			Expect(25);
			if (la.kind == 56) {
				lexer.NextToken();
			}
			Identifier();

#line  1174 "VBNET.ATG" 
			operandName = t.val; 
			if (la.kind == 49) {
				lexer.NextToken();
				TypeName(
#line  1175 "VBNET.ATG" 
out operandType);
			}

#line  1176 "VBNET.ATG" 
			parameters.Add(new ParameterDeclarationExpression(operandType, operandName, ParameterModifiers.In)); 
			while (la.kind == 12) {
				lexer.NextToken();
				if (la.kind == 56) {
					lexer.NextToken();
				}
				Identifier();

#line  1180 "VBNET.ATG" 
				operandName = t.val; 
				if (la.kind == 49) {
					lexer.NextToken();
					TypeName(
#line  1181 "VBNET.ATG" 
out operandType);
				}

#line  1182 "VBNET.ATG" 
				parameters.Add(new ParameterDeclarationExpression(operandType, operandName, ParameterModifiers.In)); 
			}
			Expect(26);

#line  1185 "VBNET.ATG" 
			Location endPos = t.EndLocation; 
			if (la.kind == 49) {
				lexer.NextToken();
				while (la.kind == 28) {
					AttributeSection(
#line  1186 "VBNET.ATG" 
out section);

#line  1186 "VBNET.ATG" 
					returnTypeAttributes.Add(section); 
				}
				TypeName(
#line  1186 "VBNET.ATG" 
out returnType);

#line  1186 "VBNET.ATG" 
				endPos = t.EndLocation; 
			}
			Expect(1);
			Block(
#line  1188 "VBNET.ATG" 
out stmt);
			Expect(95);
			Expect(148);
			EndOfStmt();

#line  1190 "VBNET.ATG" 
			OperatorDeclaration operatorDeclaration = new OperatorDeclaration {
			Modifier = m.Modifier,
			Attributes = attributes,
			Parameters = parameters,
			TypeReference = returnType,
			OverloadableOperator = operatorType,
			ConversionType = opConversionType,
			ReturnTypeAttributes = returnTypeAttributes,
			Body = (BlockStatement)stmt,
			StartLocation = m.GetDeclarationLocation(startPos),
			EndLocation = endPos
			};
			operatorDeclaration.Body.StartLocation = startPos;
			operatorDeclaration.Body.EndLocation = t.Location;
			compilationUnit.AddChild(operatorDeclaration);
			
			break;
		}
		default: SynErr(230); break;
		}
	}

	void EnumMemberDecl(
#line  730 "VBNET.ATG" 
out FieldDeclaration f) {

#line  732 "VBNET.ATG" 
		Expression expr = null;List<AttributeSection> attributes = new List<AttributeSection>();
		AttributeSection section = null;
		VariableDeclaration varDecl = null;
		
		while (la.kind == 28) {
			AttributeSection(
#line  736 "VBNET.ATG" 
out section);

#line  736 "VBNET.ATG" 
			attributes.Add(section); 
		}
		Identifier();

#line  739 "VBNET.ATG" 
		f = new FieldDeclaration(attributes);
		varDecl = new VariableDeclaration(t.val);
		f.Fields.Add(varDecl);
		f.StartLocation = varDecl.StartLocation = t.Location;
		
		if (la.kind == 10) {
			lexer.NextToken();
			Expr(
#line  744 "VBNET.ATG" 
out expr);

#line  744 "VBNET.ATG" 
			varDecl.Initializer = expr; 
		}
		EndOfStmt();
	}

	void InterfaceMemberDecl() {

#line  621 "VBNET.ATG" 
		TypeReference type =null;
		List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
		List<TemplateDefinition> templates = new List<TemplateDefinition>();
		AttributeSection section, returnTypeAttributeSection = null;
		ModifierList mod = new ModifierList();
		List<AttributeSection> attributes = new List<AttributeSection>();
		string name;
		
		if (StartOf(18)) {
			while (la.kind == 28) {
				AttributeSection(
#line  629 "VBNET.ATG" 
out section);

#line  629 "VBNET.ATG" 
				attributes.Add(section); 
			}
			while (StartOf(8)) {
				MemberModifier(
#line  632 "VBNET.ATG" 
mod);
			}
			if (la.kind == 100) {
				lexer.NextToken();

#line  636 "VBNET.ATG" 
				mod.Check(Modifiers.VBInterfaceEvents);
				Location startLocation = t.Location;
				
				Identifier();

#line  639 "VBNET.ATG" 
				name = t.val; 
				if (la.kind == 25) {
					lexer.NextToken();
					if (StartOf(4)) {
						FormalParameterList(
#line  640 "VBNET.ATG" 
p);
					}
					Expect(26);
				}
				if (la.kind == 49) {
					lexer.NextToken();
					TypeName(
#line  641 "VBNET.ATG" 
out type);
				}
				EndOfStmt();

#line  644 "VBNET.ATG" 
				EventDeclaration ed = new EventDeclaration {
				Name = name, TypeReference = type, Modifier = mod.Modifier,
				Parameters = p, Attributes = attributes,
				StartLocation = startLocation, EndLocation = t.EndLocation
				};
				compilationUnit.AddChild(ed);
				
			} else if (la.kind == 183) {
				lexer.NextToken();

#line  654 "VBNET.ATG" 
				Location startLocation =  t.Location;
				mod.Check(Modifiers.VBInterfaceMethods);
				
				Identifier();

#line  657 "VBNET.ATG" 
				name = t.val; 
				TypeParameterList(
#line  658 "VBNET.ATG" 
templates);
				if (la.kind == 25) {
					lexer.NextToken();
					if (StartOf(4)) {
						FormalParameterList(
#line  659 "VBNET.ATG" 
p);
					}
					Expect(26);
				}
				EndOfStmt();

#line  662 "VBNET.ATG" 
				MethodDeclaration md = new MethodDeclaration {
				Name = name, 
				Modifier = mod.Modifier, 
				Parameters = p,
				Attributes = attributes,
				TypeReference = new TypeReference("", "System.Void"),
				StartLocation = startLocation,
				EndLocation = t.EndLocation,
				Templates = templates
				};
				compilationUnit.AddChild(md);
				
			} else if (la.kind == 107) {
				lexer.NextToken();

#line  677 "VBNET.ATG" 
				mod.Check(Modifiers.VBInterfaceMethods);
				Location startLocation = t.Location;
				
				Identifier();

#line  680 "VBNET.ATG" 
				name = t.val; 
				TypeParameterList(
#line  681 "VBNET.ATG" 
templates);
				if (la.kind == 25) {
					lexer.NextToken();
					if (StartOf(4)) {
						FormalParameterList(
#line  682 "VBNET.ATG" 
p);
					}
					Expect(26);
				}
				if (la.kind == 49) {
					lexer.NextToken();
					while (la.kind == 28) {
						AttributeSection(
#line  683 "VBNET.ATG" 
out returnTypeAttributeSection);
					}
					TypeName(
#line  683 "VBNET.ATG" 
out type);
				}

#line  685 "VBNET.ATG" 
				if(type == null) {
				type = new TypeReference("System.Object");
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
			} else if (la.kind == 160) {
				lexer.NextToken();

#line  705 "VBNET.ATG" 
				Location startLocation = t.Location;
				mod.Check(Modifiers.VBInterfaceProperties);
				
				Identifier();

#line  708 "VBNET.ATG" 
				name = t.val;  
				if (la.kind == 25) {
					lexer.NextToken();
					if (StartOf(4)) {
						FormalParameterList(
#line  709 "VBNET.ATG" 
p);
					}
					Expect(26);
				}
				if (la.kind == 49) {
					lexer.NextToken();
					TypeName(
#line  710 "VBNET.ATG" 
out type);
				}

#line  712 "VBNET.ATG" 
				if(type == null) {
				type = new TypeReference("System.Object");
				}
				
				EndOfStmt();

#line  718 "VBNET.ATG" 
				PropertyDeclaration pd = new PropertyDeclaration(name, type, mod.Modifier, attributes);
				pd.Parameters = p;
				pd.EndLocation = t.EndLocation;
				pd.StartLocation = startLocation;
				compilationUnit.AddChild(pd);
				
			} else SynErr(231);
		} else if (StartOf(19)) {
			NonModuleDeclaration(
#line  726 "VBNET.ATG" 
mod, attributes);
		} else SynErr(232);
	}

	void Expr(
#line  1596 "VBNET.ATG" 
out Expression expr) {
		DisjunctionExpr(
#line  1598 "VBNET.ATG" 
out expr);
	}

	void ImplementsClause(
#line  1569 "VBNET.ATG" 
out List<InterfaceImplementation> baseInterfaces) {

#line  1571 "VBNET.ATG" 
		baseInterfaces = new List<InterfaceImplementation>();
		TypeReference type = null;
		string memberName = null;
		
		Expect(115);
		NonArrayTypeName(
#line  1576 "VBNET.ATG" 
out type, false);

#line  1577 "VBNET.ATG" 
		if (type != null) memberName = TypeReference.StripLastIdentifierFromType(ref type); 

#line  1578 "VBNET.ATG" 
		baseInterfaces.Add(new InterfaceImplementation(type, memberName)); 
		while (la.kind == 12) {
			lexer.NextToken();
			NonArrayTypeName(
#line  1580 "VBNET.ATG" 
out type, false);

#line  1581 "VBNET.ATG" 
			if (type != null) memberName = TypeReference.StripLastIdentifierFromType(ref type); 

#line  1582 "VBNET.ATG" 
			baseInterfaces.Add(new InterfaceImplementation(type, memberName)); 
		}
	}

	void HandlesClause(
#line  1527 "VBNET.ATG" 
out List<string> handlesClause) {

#line  1529 "VBNET.ATG" 
		handlesClause = new List<string>();
		string name;
		
		Expect(113);
		EventMemberSpecifier(
#line  1532 "VBNET.ATG" 
out name);

#line  1532 "VBNET.ATG" 
		handlesClause.Add(name); 
		while (la.kind == 12) {
			lexer.NextToken();
			EventMemberSpecifier(
#line  1533 "VBNET.ATG" 
out name);

#line  1533 "VBNET.ATG" 
			handlesClause.Add(name); 
		}
	}

	void Block(
#line  2342 "VBNET.ATG" 
out Statement stmt) {

#line  2345 "VBNET.ATG" 
		BlockStatement blockStmt = new BlockStatement();
		/* in snippet parsing mode, t might be null */
		if (t != null) blockStmt.StartLocation = t.Location;
		compilationUnit.BlockStart(blockStmt);
		
		while (StartOf(20) || 
#line  2351 "VBNET.ATG" 
IsEndStmtAhead()) {
			if (
#line  2351 "VBNET.ATG" 
IsEndStmtAhead()) {
				Expect(95);
				EndOfStmt();

#line  2351 "VBNET.ATG" 
				compilationUnit.AddChild(new EndStatement()); 
			} else {
				Statement();
				EndOfStmt();
			}
		}

#line  2356 "VBNET.ATG" 
		stmt = blockStmt;
		if (t != null) blockStmt.EndLocation = t.EndLocation;
		compilationUnit.BlockEnd();
		
	}

	void Charset(
#line  1519 "VBNET.ATG" 
out CharsetModifier charsetModifier) {

#line  1520 "VBNET.ATG" 
		charsetModifier = CharsetModifier.None; 
		if (la.kind == 107 || la.kind == 183) {
		} else if (la.kind == 48) {
			lexer.NextToken();

#line  1521 "VBNET.ATG" 
			charsetModifier = CharsetModifier.Ansi; 
		} else if (la.kind == 51) {
			lexer.NextToken();

#line  1522 "VBNET.ATG" 
			charsetModifier = CharsetModifier.Auto; 
		} else if (la.kind == 195) {
			lexer.NextToken();

#line  1523 "VBNET.ATG" 
			charsetModifier = CharsetModifier.Unicode; 
		} else SynErr(233);
	}

	void IdentifierForFieldDeclaration() {
		switch (la.kind) {
		case 2: {
			lexer.NextToken();
			break;
		}
		case 185: {
			lexer.NextToken();
			break;
		}
		case 52: {
			lexer.NextToken();
			break;
		}
		case 71: {
			lexer.NextToken();
			break;
		}
		case 50: {
			lexer.NextToken();
			break;
		}
		case 48: {
			lexer.NextToken();
			break;
		}
		case 51: {
			lexer.NextToken();
			break;
		}
		case 158: {
			lexer.NextToken();
			break;
		}
		case 195: {
			lexer.NextToken();
			break;
		}
		case 196: {
			lexer.NextToken();
			break;
		}
		case 146: {
			lexer.NextToken();
			break;
		}
		case 102: {
			lexer.NextToken();
			break;
		}
		case 118: {
			lexer.NextToken();
			break;
		}
		default: SynErr(234); break;
		}
	}

	void VariableDeclaratorPartAfterIdentifier(
#line  1394 "VBNET.ATG" 
List<VariableDeclaration> fieldDeclaration, string name) {

#line  1396 "VBNET.ATG" 
		Expression expr = null;
		TypeReference type = null;
		ArrayList rank = null;
		List<Expression> dimension = null;
		Location startLocation = t.Location;
		
		if (
#line  1402 "VBNET.ATG" 
IsSize() && !IsDims()) {
			ArrayInitializationModifier(
#line  1402 "VBNET.ATG" 
out dimension);
		}
		if (
#line  1403 "VBNET.ATG" 
IsDims()) {
			ArrayNameModifier(
#line  1403 "VBNET.ATG" 
out rank);
		}
		if (
#line  1405 "VBNET.ATG" 
IsObjectCreation()) {
			Expect(49);
			ObjectCreateExpression(
#line  1405 "VBNET.ATG" 
out expr);

#line  1407 "VBNET.ATG" 
			if (expr is ObjectCreateExpression) {
			type = ((ObjectCreateExpression)expr).CreateType;
			} else {
				type = ((ArrayCreateExpression)expr).CreateType;
			}
			
		} else if (StartOf(21)) {
			if (la.kind == 49) {
				lexer.NextToken();
				TypeName(
#line  1414 "VBNET.ATG" 
out type);

#line  1416 "VBNET.ATG" 
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

#line  1428 "VBNET.ATG" 
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
					expr = new ArrayCreateExpression(type, dimension);
					SetParent(dimension, expr);
				}
			} else if (rank != null) {
				if(type.RankSpecifier != null) {
					Error("array rank only allowed one time");
				} else {
					type.RankSpecifier = (int[])rank.ToArray(typeof(int));
				}
			}
			
			if (la.kind == 10) {
				lexer.NextToken();
				VariableInitializer(
#line  1452 "VBNET.ATG" 
out expr);
			}
		} else SynErr(235);

#line  1455 "VBNET.ATG" 
		VariableDeclaration varDecl = new VariableDeclaration(name, expr, type);
		varDecl.StartLocation = startLocation;
		varDecl.EndLocation = t.Location;
		fieldDeclaration.Add(varDecl);
		
	}

	void VariableDeclarator(
#line  1388 "VBNET.ATG" 
List<VariableDeclaration> fieldDeclaration) {
		Identifier();

#line  1390 "VBNET.ATG" 
		string name = t.val; 
		VariableDeclaratorPartAfterIdentifier(
#line  1391 "VBNET.ATG" 
fieldDeclaration, name);
	}

	void ConstantDeclarator(
#line  1369 "VBNET.ATG" 
List<VariableDeclaration> constantDeclaration) {

#line  1371 "VBNET.ATG" 
		Expression expr = null;
		TypeReference type = null;
		string name = String.Empty;
		Location location;
		
		Identifier();

#line  1376 "VBNET.ATG" 
		name = t.val; location = t.Location; 
		if (la.kind == 49) {
			lexer.NextToken();
			TypeName(
#line  1377 "VBNET.ATG" 
out type);
		}
		Expect(10);
		Expr(
#line  1378 "VBNET.ATG" 
out expr);

#line  1380 "VBNET.ATG" 
		VariableDeclaration f = new VariableDeclaration(name, expr);
		f.TypeReference = type;
		f.StartLocation = location;
		constantDeclaration.Add(f);
		
	}

	void AccessorDecls(
#line  1303 "VBNET.ATG" 
out PropertyGetRegion getBlock, out PropertySetRegion setBlock) {

#line  1305 "VBNET.ATG" 
		List<AttributeSection> attributes = new List<AttributeSection>();
		AttributeSection section;
		getBlock = null;
		setBlock = null; 
		
		while (la.kind == 28) {
			AttributeSection(
#line  1310 "VBNET.ATG" 
out section);

#line  1310 "VBNET.ATG" 
			attributes.Add(section); 
		}
		if (StartOf(22)) {
			GetAccessorDecl(
#line  1312 "VBNET.ATG" 
out getBlock, attributes);
			if (StartOf(23)) {

#line  1314 "VBNET.ATG" 
				attributes = new List<AttributeSection>(); 
				while (la.kind == 28) {
					AttributeSection(
#line  1315 "VBNET.ATG" 
out section);

#line  1315 "VBNET.ATG" 
					attributes.Add(section); 
				}
				SetAccessorDecl(
#line  1316 "VBNET.ATG" 
out setBlock, attributes);
			}
		} else if (StartOf(24)) {
			SetAccessorDecl(
#line  1319 "VBNET.ATG" 
out setBlock, attributes);
			if (StartOf(25)) {

#line  1321 "VBNET.ATG" 
				attributes = new List<AttributeSection>(); 
				while (la.kind == 28) {
					AttributeSection(
#line  1322 "VBNET.ATG" 
out section);

#line  1322 "VBNET.ATG" 
					attributes.Add(section); 
				}
				GetAccessorDecl(
#line  1323 "VBNET.ATG" 
out getBlock, attributes);
			}
		} else SynErr(236);
	}

	void EventAccessorDeclaration(
#line  1266 "VBNET.ATG" 
out EventAddRemoveRegion eventAccessorDeclaration) {

#line  1268 "VBNET.ATG" 
		Statement stmt = null;
		List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		eventAccessorDeclaration = null;
		
		while (la.kind == 28) {
			AttributeSection(
#line  1274 "VBNET.ATG" 
out section);

#line  1274 "VBNET.ATG" 
			attributes.Add(section); 
		}
		if (la.kind == 43) {
			lexer.NextToken();
			if (la.kind == 25) {
				lexer.NextToken();
				if (StartOf(4)) {
					FormalParameterList(
#line  1276 "VBNET.ATG" 
p);
				}
				Expect(26);
			}
			Expect(1);
			Block(
#line  1277 "VBNET.ATG" 
out stmt);
			Expect(95);
			Expect(43);
			EndOfStmt();

#line  1279 "VBNET.ATG" 
			eventAccessorDeclaration = new EventAddRegion(attributes);
			eventAccessorDeclaration.Block = (BlockStatement)stmt;
			eventAccessorDeclaration.Parameters = p;
			
		} else if (la.kind == 167) {
			lexer.NextToken();
			if (la.kind == 25) {
				lexer.NextToken();
				if (StartOf(4)) {
					FormalParameterList(
#line  1284 "VBNET.ATG" 
p);
				}
				Expect(26);
			}
			Expect(1);
			Block(
#line  1285 "VBNET.ATG" 
out stmt);
			Expect(95);
			Expect(167);
			EndOfStmt();

#line  1287 "VBNET.ATG" 
			eventAccessorDeclaration = new EventRemoveRegion(attributes);
			eventAccessorDeclaration.Block = (BlockStatement)stmt;
			eventAccessorDeclaration.Parameters = p;
			
		} else if (la.kind == 163) {
			lexer.NextToken();
			if (la.kind == 25) {
				lexer.NextToken();
				if (StartOf(4)) {
					FormalParameterList(
#line  1292 "VBNET.ATG" 
p);
				}
				Expect(26);
			}
			Expect(1);
			Block(
#line  1293 "VBNET.ATG" 
out stmt);
			Expect(95);
			Expect(163);
			EndOfStmt();

#line  1295 "VBNET.ATG" 
			eventAccessorDeclaration = new EventRaiseRegion(attributes);
			eventAccessorDeclaration.Block = (BlockStatement)stmt;
			eventAccessorDeclaration.Parameters = p;
			
		} else SynErr(237);
	}

	void OverloadableOperator(
#line  1208 "VBNET.ATG" 
out OverloadableOperatorType operatorType) {

#line  1209 "VBNET.ATG" 
		operatorType = OverloadableOperatorType.None; 
		switch (la.kind) {
		case 19: {
			lexer.NextToken();

#line  1211 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Add; 
			break;
		}
		case 18: {
			lexer.NextToken();

#line  1213 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Subtract; 
			break;
		}
		case 22: {
			lexer.NextToken();

#line  1215 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Multiply; 
			break;
		}
		case 14: {
			lexer.NextToken();

#line  1217 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Divide; 
			break;
		}
		case 15: {
			lexer.NextToken();

#line  1219 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.DivideInteger; 
			break;
		}
		case 13: {
			lexer.NextToken();

#line  1221 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Concat; 
			break;
		}
		case 126: {
			lexer.NextToken();

#line  1223 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Like; 
			break;
		}
		case 130: {
			lexer.NextToken();

#line  1225 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Modulus; 
			break;
		}
		case 46: {
			lexer.NextToken();

#line  1227 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.BitwiseAnd; 
			break;
		}
		case 151: {
			lexer.NextToken();

#line  1229 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.BitwiseOr; 
			break;
		}
		case 207: {
			lexer.NextToken();

#line  1231 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.ExclusiveOr; 
			break;
		}
		case 20: {
			lexer.NextToken();

#line  1233 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Power; 
			break;
		}
		case 32: {
			lexer.NextToken();

#line  1235 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.ShiftLeft; 
			break;
		}
		case 33: {
			lexer.NextToken();

#line  1237 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.ShiftRight; 
			break;
		}
		case 10: {
			lexer.NextToken();

#line  1239 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.Equality; 
			break;
		}
		case 29: {
			lexer.NextToken();

#line  1241 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.InEquality; 
			break;
		}
		case 28: {
			lexer.NextToken();

#line  1243 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.LessThan; 
			break;
		}
		case 31: {
			lexer.NextToken();

#line  1245 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.LessThanOrEqual; 
			break;
		}
		case 27: {
			lexer.NextToken();

#line  1247 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.GreaterThan; 
			break;
		}
		case 30: {
			lexer.NextToken();

#line  1249 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.GreaterThanOrEqual; 
			break;
		}
		case 78: {
			lexer.NextToken();

#line  1251 "VBNET.ATG" 
			operatorType = OverloadableOperatorType.CType; 
			break;
		}
		case 2: case 48: case 50: case 51: case 52: case 71: case 82: case 102: case 146: case 158: case 185: case 195: case 196: {
			Identifier();

#line  1255 "VBNET.ATG" 
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
		default: SynErr(238); break;
		}
	}

	void GetAccessorDecl(
#line  1329 "VBNET.ATG" 
out PropertyGetRegion getBlock, List<AttributeSection> attributes) {

#line  1330 "VBNET.ATG" 
		Statement stmt = null; Modifiers m; 
		PropertyAccessorAccessModifier(
#line  1332 "VBNET.ATG" 
out m);
		Expect(108);

#line  1334 "VBNET.ATG" 
		Location startLocation = t.Location; 
		Expect(1);
		Block(
#line  1336 "VBNET.ATG" 
out stmt);

#line  1337 "VBNET.ATG" 
		getBlock = new PropertyGetRegion((BlockStatement)stmt, attributes); 
		Expect(95);
		Expect(108);

#line  1339 "VBNET.ATG" 
		getBlock.Modifier = m; 

#line  1340 "VBNET.ATG" 
		getBlock.StartLocation = startLocation; getBlock.EndLocation = t.EndLocation; 
		EndOfStmt();
	}

	void SetAccessorDecl(
#line  1345 "VBNET.ATG" 
out PropertySetRegion setBlock, List<AttributeSection> attributes) {

#line  1347 "VBNET.ATG" 
		Statement stmt = null;
		List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
		Modifiers m;
		
		PropertyAccessorAccessModifier(
#line  1352 "VBNET.ATG" 
out m);
		Expect(172);

#line  1354 "VBNET.ATG" 
		Location startLocation = t.Location; 
		if (la.kind == 25) {
			lexer.NextToken();
			if (StartOf(4)) {
				FormalParameterList(
#line  1355 "VBNET.ATG" 
p);
			}
			Expect(26);
		}
		Expect(1);
		Block(
#line  1357 "VBNET.ATG" 
out stmt);

#line  1359 "VBNET.ATG" 
		setBlock = new PropertySetRegion((BlockStatement)stmt, attributes);
		setBlock.Modifier = m;
		setBlock.Parameters = p;
		
		Expect(95);
		Expect(172);

#line  1364 "VBNET.ATG" 
		setBlock.StartLocation = startLocation; setBlock.EndLocation = t.EndLocation; 
		EndOfStmt();
	}

	void PropertyAccessorAccessModifier(
#line  3048 "VBNET.ATG" 
out Modifiers m) {

#line  3049 "VBNET.ATG" 
		m = Modifiers.None; 
		while (StartOf(26)) {
			if (la.kind == 162) {
				lexer.NextToken();

#line  3051 "VBNET.ATG" 
				m |= Modifiers.Public; 
			} else if (la.kind == 161) {
				lexer.NextToken();

#line  3052 "VBNET.ATG" 
				m |= Modifiers.Protected; 
			} else if (la.kind == 106) {
				lexer.NextToken();

#line  3053 "VBNET.ATG" 
				m |= Modifiers.Internal; 
			} else {
				lexer.NextToken();

#line  3054 "VBNET.ATG" 
				m |= Modifiers.Private; 
			}
		}
	}

	void ArrayInitializationModifier(
#line  1463 "VBNET.ATG" 
out List<Expression> arrayModifiers) {

#line  1465 "VBNET.ATG" 
		arrayModifiers = null;
		
		Expect(25);
		InitializationRankList(
#line  1467 "VBNET.ATG" 
out arrayModifiers);
		Expect(26);
	}

	void ArrayNameModifier(
#line  2133 "VBNET.ATG" 
out ArrayList arrayModifiers) {

#line  2135 "VBNET.ATG" 
		arrayModifiers = null;
		
		ArrayTypeModifiers(
#line  2137 "VBNET.ATG" 
out arrayModifiers);
	}

	void ObjectCreateExpression(
#line  1922 "VBNET.ATG" 
out Expression oce) {

#line  1924 "VBNET.ATG" 
		TypeReference type = null;
		Expression initializer = null;
		List<Expression> arguments = null;
		ArrayList dimensions = null;
		oce = null;
		bool canBeNormal; bool canBeReDim;
		
		Expect(138);
		if (StartOf(6)) {
			NonArrayTypeName(
#line  1932 "VBNET.ATG" 
out type, false);
			if (la.kind == 25) {
				lexer.NextToken();
				NormalOrReDimArgumentList(
#line  1933 "VBNET.ATG" 
out arguments, out canBeNormal, out canBeReDim);
				Expect(26);
				if (la.kind == 23 || 
#line  1934 "VBNET.ATG" 
la.kind == Tokens.OpenParenthesis) {
					if (
#line  1934 "VBNET.ATG" 
la.kind == Tokens.OpenParenthesis) {
						ArrayTypeModifiers(
#line  1935 "VBNET.ATG" 
out dimensions);
						CollectionInitializer(
#line  1936 "VBNET.ATG" 
out initializer);
					} else {
						CollectionInitializer(
#line  1937 "VBNET.ATG" 
out initializer);
					}
				}

#line  1939 "VBNET.ATG" 
				if (canBeReDim && !canBeNormal && initializer == null) initializer = new CollectionInitializerExpression(); 
			}
		}

#line  1943 "VBNET.ATG" 
		if (initializer == null) {
		oce = new ObjectCreateExpression(type, arguments);
		SetParent(arguments, oce);
		} else {
			if (dimensions == null) dimensions = new ArrayList();
			dimensions.Insert(0, (arguments == null) ? 0 : Math.Max(arguments.Count - 1, 0));
			type.RankSpecifier = (int[])dimensions.ToArray(typeof(int));
			ArrayCreateExpression ace = new ArrayCreateExpression(type, initializer as CollectionInitializerExpression);
			ace.Arguments = arguments;
			SetParent(arguments, ace);
			oce = ace;
		}
		
		if (la.kind == 204) {

#line  1959 "VBNET.ATG" 
			NamedArgumentExpression memberInitializer = null;
			
			lexer.NextToken();

#line  1963 "VBNET.ATG" 
			CollectionInitializerExpression memberInitializers = new CollectionInitializerExpression();
			memberInitializers.StartLocation = la.Location;
			
			Expect(23);
			MemberInitializer(
#line  1967 "VBNET.ATG" 
out memberInitializer);

#line  1968 "VBNET.ATG" 
			memberInitializers.CreateExpressions.Add(memberInitializer); 
			while (la.kind == 12) {
				lexer.NextToken();
				MemberInitializer(
#line  1970 "VBNET.ATG" 
out memberInitializer);

#line  1971 "VBNET.ATG" 
				memberInitializers.CreateExpressions.Add(memberInitializer); 
			}
			Expect(24);

#line  1975 "VBNET.ATG" 
			memberInitializers.EndLocation = t.Location;
			if(oce is ObjectCreateExpression)
			{
				((ObjectCreateExpression)oce).ObjectInitializer = memberInitializers;
			}
			
		}
	}

	void VariableInitializer(
#line  1491 "VBNET.ATG" 
out Expression initializerExpression) {

#line  1493 "VBNET.ATG" 
		initializerExpression = null;
		
		if (StartOf(27)) {
			Expr(
#line  1495 "VBNET.ATG" 
out initializerExpression);
		} else if (la.kind == 23) {
			CollectionInitializer(
#line  1496 "VBNET.ATG" 
out initializerExpression);
		} else SynErr(239);
	}

	void InitializationRankList(
#line  1471 "VBNET.ATG" 
out List<Expression> rank) {

#line  1473 "VBNET.ATG" 
		rank = new List<Expression>();
		Expression expr = null;
		
		Expr(
#line  1476 "VBNET.ATG" 
out expr);
		if (la.kind == 188) {
			lexer.NextToken();

#line  1477 "VBNET.ATG" 
			EnsureIsZero(expr); 
			Expr(
#line  1478 "VBNET.ATG" 
out expr);
		}

#line  1480 "VBNET.ATG" 
		if (expr != null) { rank.Add(expr); } 
		while (la.kind == 12) {
			lexer.NextToken();
			Expr(
#line  1482 "VBNET.ATG" 
out expr);
			if (la.kind == 188) {
				lexer.NextToken();

#line  1483 "VBNET.ATG" 
				EnsureIsZero(expr); 
				Expr(
#line  1484 "VBNET.ATG" 
out expr);
			}

#line  1486 "VBNET.ATG" 
			if (expr != null) { rank.Add(expr); } 
		}
	}

	void CollectionInitializer(
#line  1500 "VBNET.ATG" 
out Expression outExpr) {

#line  1502 "VBNET.ATG" 
		Expression expr = null;
		CollectionInitializerExpression initializer = new CollectionInitializerExpression();
		
		Expect(23);
		if (StartOf(28)) {
			VariableInitializer(
#line  1507 "VBNET.ATG" 
out expr);

#line  1509 "VBNET.ATG" 
			if (expr != null) { initializer.CreateExpressions.Add(expr); }
			
			while (
#line  1512 "VBNET.ATG" 
NotFinalComma()) {
				Expect(12);
				VariableInitializer(
#line  1512 "VBNET.ATG" 
out expr);

#line  1513 "VBNET.ATG" 
				if (expr != null) { initializer.CreateExpressions.Add(expr); } 
			}
		}
		Expect(24);

#line  1516 "VBNET.ATG" 
		outExpr = initializer; 
	}

	void EventMemberSpecifier(
#line  1586 "VBNET.ATG" 
out string name) {

#line  1587 "VBNET.ATG" 
		string eventName; 
		if (StartOf(13)) {
			Identifier();
		} else if (la.kind == 134) {
			lexer.NextToken();
		} else if (la.kind == 129) {
			lexer.NextToken();
		} else SynErr(240);

#line  1590 "VBNET.ATG" 
		name = t.val; 
		Expect(16);
		IdentifierOrKeyword(
#line  1592 "VBNET.ATG" 
out eventName);

#line  1593 "VBNET.ATG" 
		name = name + "." + eventName; 
	}

	void IdentifierOrKeyword(
#line  2981 "VBNET.ATG" 
out string name) {

#line  2983 "VBNET.ATG" 
		lexer.NextToken(); name = t.val;  
	}

	void DisjunctionExpr(
#line  1766 "VBNET.ATG" 
out Expression outExpr) {

#line  1768 "VBNET.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		ConjunctionExpr(
#line  1771 "VBNET.ATG" 
out outExpr);
		while (la.kind == 151 || la.kind == 152 || la.kind == 207) {
			if (la.kind == 151) {
				lexer.NextToken();

#line  1774 "VBNET.ATG" 
				op = BinaryOperatorType.BitwiseOr; 
			} else if (la.kind == 152) {
				lexer.NextToken();

#line  1775 "VBNET.ATG" 
				op = BinaryOperatorType.LogicalOr; 
			} else {
				lexer.NextToken();

#line  1776 "VBNET.ATG" 
				op = BinaryOperatorType.ExclusiveOr; 
			}
			ConjunctionExpr(
#line  1778 "VBNET.ATG" 
out expr);

#line  1778 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr);  
		}
	}

	void AssignmentOperator(
#line  1601 "VBNET.ATG" 
out AssignmentOperatorType op) {

#line  1602 "VBNET.ATG" 
		op = AssignmentOperatorType.None; 
		switch (la.kind) {
		case 10: {
			lexer.NextToken();

#line  1603 "VBNET.ATG" 
			op = AssignmentOperatorType.Assign; 
			break;
		}
		case 42: {
			lexer.NextToken();

#line  1604 "VBNET.ATG" 
			op = AssignmentOperatorType.ConcatString; 
			break;
		}
		case 34: {
			lexer.NextToken();

#line  1605 "VBNET.ATG" 
			op = AssignmentOperatorType.Add; 
			break;
		}
		case 36: {
			lexer.NextToken();

#line  1606 "VBNET.ATG" 
			op = AssignmentOperatorType.Subtract; 
			break;
		}
		case 37: {
			lexer.NextToken();

#line  1607 "VBNET.ATG" 
			op = AssignmentOperatorType.Multiply; 
			break;
		}
		case 38: {
			lexer.NextToken();

#line  1608 "VBNET.ATG" 
			op = AssignmentOperatorType.Divide; 
			break;
		}
		case 39: {
			lexer.NextToken();

#line  1609 "VBNET.ATG" 
			op = AssignmentOperatorType.DivideInteger; 
			break;
		}
		case 35: {
			lexer.NextToken();

#line  1610 "VBNET.ATG" 
			op = AssignmentOperatorType.Power; 
			break;
		}
		case 40: {
			lexer.NextToken();

#line  1611 "VBNET.ATG" 
			op = AssignmentOperatorType.ShiftLeft; 
			break;
		}
		case 41: {
			lexer.NextToken();

#line  1612 "VBNET.ATG" 
			op = AssignmentOperatorType.ShiftRight; 
			break;
		}
		default: SynErr(241); break;
		}
	}

	void SimpleExpr(
#line  1616 "VBNET.ATG" 
out Expression pexpr) {

#line  1617 "VBNET.ATG" 
		string name; 
		SimpleNonInvocationExpression(
#line  1619 "VBNET.ATG" 
out pexpr);
		while (la.kind == 16 || la.kind == 17 || la.kind == 25) {
			if (la.kind == 16) {
				lexer.NextToken();
				IdentifierOrKeyword(
#line  1621 "VBNET.ATG" 
out name);

#line  1622 "VBNET.ATG" 
				pexpr = new MemberReferenceExpression(pexpr, name); 
				if (
#line  1623 "VBNET.ATG" 
la.kind == Tokens.OpenParenthesis && Peek(1).kind == Tokens.Of) {
					lexer.NextToken();
					Expect(145);
					TypeArgumentList(
#line  1624 "VBNET.ATG" 
((MemberReferenceExpression)pexpr).TypeArguments);
					Expect(26);
				}
			} else if (la.kind == 17) {
				lexer.NextToken();
				IdentifierOrKeyword(
#line  1626 "VBNET.ATG" 
out name);

#line  1626 "VBNET.ATG" 
				pexpr = new BinaryOperatorExpression(pexpr, BinaryOperatorType.DictionaryAccess, new PrimitiveExpression(name, name)); 
			} else {
				InvocationExpression(
#line  1627 "VBNET.ATG" 
ref pexpr);
			}
		}
	}

	void SimpleNonInvocationExpression(
#line  1631 "VBNET.ATG" 
out Expression pexpr) {

#line  1633 "VBNET.ATG" 
		Expression expr;
		TypeReference type = null;
		string name = String.Empty;
		pexpr = null;
		
		if (StartOf(29) || 
#line  1687 "VBNET.ATG" 
la.kind == Tokens.If) {
			if (la.kind == 3) {
				lexer.NextToken();

#line  1641 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(t.literalValue, t.val) { LiteralFormat = t.literalFormat };  
			} else if (la.kind == 4) {
				lexer.NextToken();

#line  1642 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(t.literalValue, t.val) { LiteralFormat = t.literalFormat };  
			} else if (la.kind == 7) {
				lexer.NextToken();

#line  1643 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(t.literalValue, t.val) { LiteralFormat = t.literalFormat };  
			} else if (la.kind == 6) {
				lexer.NextToken();

#line  1644 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(t.literalValue, t.val) { LiteralFormat = t.literalFormat };  
			} else if (la.kind == 5) {
				lexer.NextToken();

#line  1645 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(t.literalValue, t.val) { LiteralFormat = t.literalFormat };  
			} else if (la.kind == 9) {
				lexer.NextToken();

#line  1646 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(t.literalValue, t.val) { LiteralFormat = t.literalFormat };  
			} else if (la.kind == 8) {
				lexer.NextToken();

#line  1647 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(t.literalValue, t.val) { LiteralFormat = t.literalFormat };  
			} else if (la.kind == 189) {
				lexer.NextToken();

#line  1649 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(true, "true");  
			} else if (la.kind == 103) {
				lexer.NextToken();

#line  1650 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(false, "false"); 
			} else if (la.kind == 141) {
				lexer.NextToken();

#line  1651 "VBNET.ATG" 
				pexpr = new PrimitiveExpression(null, "null");  
			} else if (la.kind == 25) {
				lexer.NextToken();
				Expr(
#line  1652 "VBNET.ATG" 
out expr);
				Expect(26);

#line  1652 "VBNET.ATG" 
				pexpr = new ParenthesizedExpression(expr); 
			} else if (StartOf(13)) {
				Identifier();

#line  1654 "VBNET.ATG" 
				pexpr = new IdentifierExpression(t.val);
				pexpr.StartLocation = t.Location; pexpr.EndLocation = t.EndLocation;
				
				if (
#line  1657 "VBNET.ATG" 
la.kind == Tokens.OpenParenthesis && Peek(1).kind == Tokens.Of) {
					lexer.NextToken();
					Expect(145);
					TypeArgumentList(
#line  1658 "VBNET.ATG" 
((IdentifierExpression)pexpr).TypeArguments);
					Expect(26);
				}
			} else if (StartOf(30)) {

#line  1660 "VBNET.ATG" 
				string val = String.Empty; 
				if (StartOf(10)) {
					PrimitiveTypeName(
#line  1661 "VBNET.ATG" 
out val);
				} else if (la.kind == 144) {
					lexer.NextToken();

#line  1661 "VBNET.ATG" 
					val = "Object"; 
				} else SynErr(242);
				Expect(16);

#line  1662 "VBNET.ATG" 
				t.val = ""; 
				Identifier();

#line  1662 "VBNET.ATG" 
				pexpr = new MemberReferenceExpression(new TypeReferenceExpression(val), t.val); 
			} else if (la.kind == 129) {
				lexer.NextToken();

#line  1663 "VBNET.ATG" 
				pexpr = new ThisReferenceExpression(); 
			} else if (la.kind == 134 || la.kind == 135) {

#line  1664 "VBNET.ATG" 
				Expression retExpr = null; 
				if (la.kind == 134) {
					lexer.NextToken();

#line  1665 "VBNET.ATG" 
					retExpr = new BaseReferenceExpression(); 
				} else if (la.kind == 135) {
					lexer.NextToken();

#line  1666 "VBNET.ATG" 
					retExpr = new ClassReferenceExpression(); 
				} else SynErr(243);
				Expect(16);
				IdentifierOrKeyword(
#line  1668 "VBNET.ATG" 
out name);

#line  1668 "VBNET.ATG" 
				pexpr = new MemberReferenceExpression(retExpr, name); 
			} else if (la.kind == 110) {
				lexer.NextToken();
				Expect(16);
				Identifier();

#line  1670 "VBNET.ATG" 
				type = new TypeReference(t.val ?? ""); 

#line  1672 "VBNET.ATG" 
				type.IsGlobal = true; 

#line  1673 "VBNET.ATG" 
				pexpr = new TypeReferenceExpression(type); 
			} else if (la.kind == 138) {
				ObjectCreateExpression(
#line  1674 "VBNET.ATG" 
out expr);

#line  1674 "VBNET.ATG" 
				pexpr = expr; 
			} else if (la.kind == 78 || la.kind == 89 || la.kind == 191) {

#line  1676 "VBNET.ATG" 
				CastType castType = CastType.Cast; 
				if (la.kind == 89) {
					lexer.NextToken();
				} else if (la.kind == 78) {
					lexer.NextToken();

#line  1678 "VBNET.ATG" 
					castType = CastType.Conversion; 
				} else if (la.kind == 191) {
					lexer.NextToken();

#line  1679 "VBNET.ATG" 
					castType = CastType.TryCast; 
				} else SynErr(244);
				Expect(25);
				Expr(
#line  1681 "VBNET.ATG" 
out expr);
				Expect(12);
				TypeName(
#line  1681 "VBNET.ATG" 
out type);
				Expect(26);

#line  1682 "VBNET.ATG" 
				pexpr = new CastExpression(type, expr, castType); 
			} else if (StartOf(31)) {
				CastTarget(
#line  1683 "VBNET.ATG" 
out type);
				Expect(25);
				Expr(
#line  1683 "VBNET.ATG" 
out expr);
				Expect(26);

#line  1683 "VBNET.ATG" 
				pexpr = new CastExpression(type, expr, CastType.PrimitiveConversion); 
			} else if (la.kind == 44) {
				lexer.NextToken();
				Expr(
#line  1684 "VBNET.ATG" 
out expr);

#line  1684 "VBNET.ATG" 
				pexpr = new AddressOfExpression(expr); 
			} else if (la.kind == 109) {
				lexer.NextToken();
				Expect(25);
				GetTypeTypeName(
#line  1685 "VBNET.ATG" 
out type);
				Expect(26);

#line  1685 "VBNET.ATG" 
				pexpr = new TypeOfExpression(type); 
			} else if (la.kind == 192) {
				lexer.NextToken();
				SimpleExpr(
#line  1686 "VBNET.ATG" 
out expr);
				Expect(122);
				TypeName(
#line  1686 "VBNET.ATG" 
out type);

#line  1686 "VBNET.ATG" 
				pexpr = new TypeOfIsExpression(expr, type); 
			} else {
				ConditionalExpression(
#line  1687 "VBNET.ATG" 
out pexpr);
			}
		} else if (la.kind == 16) {
			lexer.NextToken();
			IdentifierOrKeyword(
#line  1691 "VBNET.ATG" 
out name);

#line  1691 "VBNET.ATG" 
			pexpr = new MemberReferenceExpression(null, name);
		} else SynErr(245);
	}

	void TypeArgumentList(
#line  2169 "VBNET.ATG" 
List<TypeReference> typeArguments) {

#line  2171 "VBNET.ATG" 
		TypeReference typeref;
		
		TypeName(
#line  2173 "VBNET.ATG" 
out typeref);

#line  2173 "VBNET.ATG" 
		if (typeref != null) typeArguments.Add(typeref); 
		while (la.kind == 12) {
			lexer.NextToken();
			TypeName(
#line  2176 "VBNET.ATG" 
out typeref);

#line  2176 "VBNET.ATG" 
			if (typeref != null) typeArguments.Add(typeref); 
		}
	}

	void InvocationExpression(
#line  1729 "VBNET.ATG" 
ref Expression pexpr) {

#line  1730 "VBNET.ATG" 
		List<Expression> parameters = null; 
		Expect(25);

#line  1732 "VBNET.ATG" 
		Location start = t.Location; 
		ArgumentList(
#line  1733 "VBNET.ATG" 
out parameters);
		Expect(26);

#line  1736 "VBNET.ATG" 
		pexpr = new InvocationExpression(pexpr, parameters);
		SetParent(parameters, pexpr);
		

#line  1739 "VBNET.ATG" 
		pexpr.StartLocation = start; pexpr.EndLocation = t.Location; 
	}

	void PrimitiveTypeName(
#line  2988 "VBNET.ATG" 
out string type) {

#line  2989 "VBNET.ATG" 
		type = String.Empty; 
		switch (la.kind) {
		case 53: {
			lexer.NextToken();

#line  2990 "VBNET.ATG" 
			type = "Boolean"; 
			break;
		}
		case 83: {
			lexer.NextToken();

#line  2991 "VBNET.ATG" 
			type = "Date"; 
			break;
		}
		case 66: {
			lexer.NextToken();

#line  2992 "VBNET.ATG" 
			type = "Char"; 
			break;
		}
		case 181: {
			lexer.NextToken();

#line  2993 "VBNET.ATG" 
			type = "String"; 
			break;
		}
		case 84: {
			lexer.NextToken();

#line  2994 "VBNET.ATG" 
			type = "Decimal"; 
			break;
		}
		case 55: {
			lexer.NextToken();

#line  2995 "VBNET.ATG" 
			type = "Byte"; 
			break;
		}
		case 175: {
			lexer.NextToken();

#line  2996 "VBNET.ATG" 
			type = "Short"; 
			break;
		}
		case 120: {
			lexer.NextToken();

#line  2997 "VBNET.ATG" 
			type = "Integer"; 
			break;
		}
		case 127: {
			lexer.NextToken();

#line  2998 "VBNET.ATG" 
			type = "Long"; 
			break;
		}
		case 176: {
			lexer.NextToken();

#line  2999 "VBNET.ATG" 
			type = "Single"; 
			break;
		}
		case 91: {
			lexer.NextToken();

#line  3000 "VBNET.ATG" 
			type = "Double"; 
			break;
		}
		case 193: {
			lexer.NextToken();

#line  3001 "VBNET.ATG" 
			type = "UInteger"; 
			break;
		}
		case 194: {
			lexer.NextToken();

#line  3002 "VBNET.ATG" 
			type = "ULong"; 
			break;
		}
		case 197: {
			lexer.NextToken();

#line  3003 "VBNET.ATG" 
			type = "UShort"; 
			break;
		}
		case 170: {
			lexer.NextToken();

#line  3004 "VBNET.ATG" 
			type = "SByte"; 
			break;
		}
		default: SynErr(246); break;
		}
	}

	void CastTarget(
#line  1744 "VBNET.ATG" 
out TypeReference type) {

#line  1746 "VBNET.ATG" 
		type = null;
		
		switch (la.kind) {
		case 60: {
			lexer.NextToken();

#line  1748 "VBNET.ATG" 
			type = new TypeReference("System.Boolean"); 
			break;
		}
		case 61: {
			lexer.NextToken();

#line  1749 "VBNET.ATG" 
			type = new TypeReference("System.Byte"); 
			break;
		}
		case 74: {
			lexer.NextToken();

#line  1750 "VBNET.ATG" 
			type = new TypeReference("System.SByte"); 
			break;
		}
		case 62: {
			lexer.NextToken();

#line  1751 "VBNET.ATG" 
			type = new TypeReference("System.Char"); 
			break;
		}
		case 63: {
			lexer.NextToken();

#line  1752 "VBNET.ATG" 
			type = new TypeReference("System.DateTime"); 
			break;
		}
		case 65: {
			lexer.NextToken();

#line  1753 "VBNET.ATG" 
			type = new TypeReference("System.Decimal"); 
			break;
		}
		case 64: {
			lexer.NextToken();

#line  1754 "VBNET.ATG" 
			type = new TypeReference("System.Double"); 
			break;
		}
		case 75: {
			lexer.NextToken();

#line  1755 "VBNET.ATG" 
			type = new TypeReference("System.Int16"); 
			break;
		}
		case 67: {
			lexer.NextToken();

#line  1756 "VBNET.ATG" 
			type = new TypeReference("System.Int32"); 
			break;
		}
		case 69: {
			lexer.NextToken();

#line  1757 "VBNET.ATG" 
			type = new TypeReference("System.Int64"); 
			break;
		}
		case 81: {
			lexer.NextToken();

#line  1758 "VBNET.ATG" 
			type = new TypeReference("System.UInt16"); 
			break;
		}
		case 79: {
			lexer.NextToken();

#line  1759 "VBNET.ATG" 
			type = new TypeReference("System.UInt32"); 
			break;
		}
		case 80: {
			lexer.NextToken();

#line  1760 "VBNET.ATG" 
			type = new TypeReference("System.UInt64"); 
			break;
		}
		case 70: {
			lexer.NextToken();

#line  1761 "VBNET.ATG" 
			type = new TypeReference("System.Object"); 
			break;
		}
		case 76: {
			lexer.NextToken();

#line  1762 "VBNET.ATG" 
			type = new TypeReference("System.Single"); 
			break;
		}
		case 77: {
			lexer.NextToken();

#line  1763 "VBNET.ATG" 
			type = new TypeReference("System.String"); 
			break;
		}
		default: SynErr(247); break;
		}
	}

	void GetTypeTypeName(
#line  2068 "VBNET.ATG" 
out TypeReference typeref) {

#line  2069 "VBNET.ATG" 
		ArrayList rank = null; 
		NonArrayTypeName(
#line  2071 "VBNET.ATG" 
out typeref, true);
		ArrayTypeModifiers(
#line  2072 "VBNET.ATG" 
out rank);

#line  2073 "VBNET.ATG" 
		if (rank != null && typeref != null) {
		typeref.RankSpecifier = (int[])rank.ToArray(typeof(int));
		}
		
	}

	void ConditionalExpression(
#line  1695 "VBNET.ATG" 
out Expression expr) {

#line  1697 "VBNET.ATG" 
		ConditionalExpression conditionalExpression = new ConditionalExpression();
		BinaryOperatorExpression binaryOperatorExpression = new BinaryOperatorExpression();
		conditionalExpression.StartLocation = binaryOperatorExpression.StartLocation = la.Location;
		
		Expression condition = null;
		Expression trueExpr = null;
		Expression falseExpr = null;
		
		Expect(114);
		Expect(25);
		Expr(
#line  1706 "VBNET.ATG" 
out condition);
		Expect(12);
		Expr(
#line  1706 "VBNET.ATG" 
out trueExpr);
		if (la.kind == 12) {
			lexer.NextToken();
			Expr(
#line  1706 "VBNET.ATG" 
out falseExpr);
		}
		Expect(26);

#line  1708 "VBNET.ATG" 
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
#line  2000 "VBNET.ATG" 
out List<Expression> arguments) {

#line  2002 "VBNET.ATG" 
		arguments = new List<Expression>();
		Expression expr = null;
		
		if (StartOf(27)) {
			Argument(
#line  2005 "VBNET.ATG" 
out expr);
		}
		while (la.kind == 12) {
			lexer.NextToken();

#line  2006 "VBNET.ATG" 
			arguments.Add(expr ?? Expression.Null); expr = null; 
			if (StartOf(27)) {
				Argument(
#line  2007 "VBNET.ATG" 
out expr);
			}

#line  2008 "VBNET.ATG" 
			if (expr == null) expr = Expression.Null; 
		}

#line  2010 "VBNET.ATG" 
		if (expr != null) arguments.Add(expr); 
	}

	void ConjunctionExpr(
#line  1782 "VBNET.ATG" 
out Expression outExpr) {

#line  1784 "VBNET.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		NotExpr(
#line  1787 "VBNET.ATG" 
out outExpr);
		while (la.kind == 46 || la.kind == 47) {
			if (la.kind == 46) {
				lexer.NextToken();

#line  1790 "VBNET.ATG" 
				op = BinaryOperatorType.BitwiseAnd; 
			} else {
				lexer.NextToken();

#line  1791 "VBNET.ATG" 
				op = BinaryOperatorType.LogicalAnd; 
			}
			NotExpr(
#line  1793 "VBNET.ATG" 
out expr);

#line  1793 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr);  
		}
	}

	void NotExpr(
#line  1797 "VBNET.ATG" 
out Expression outExpr) {

#line  1798 "VBNET.ATG" 
		UnaryOperatorType uop = UnaryOperatorType.None; 
		while (la.kind == 140) {
			lexer.NextToken();

#line  1799 "VBNET.ATG" 
			uop = UnaryOperatorType.Not; 
		}
		ComparisonExpr(
#line  1800 "VBNET.ATG" 
out outExpr);

#line  1801 "VBNET.ATG" 
		if (uop != UnaryOperatorType.None)
		outExpr = new UnaryOperatorExpression(outExpr, uop);
		
	}

	void ComparisonExpr(
#line  1806 "VBNET.ATG" 
out Expression outExpr) {

#line  1808 "VBNET.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		ShiftExpr(
#line  1811 "VBNET.ATG" 
out outExpr);
		while (StartOf(32)) {
			switch (la.kind) {
			case 28: {
				lexer.NextToken();

#line  1814 "VBNET.ATG" 
				op = BinaryOperatorType.LessThan; 
				break;
			}
			case 27: {
				lexer.NextToken();

#line  1815 "VBNET.ATG" 
				op = BinaryOperatorType.GreaterThan; 
				break;
			}
			case 31: {
				lexer.NextToken();

#line  1816 "VBNET.ATG" 
				op = BinaryOperatorType.LessThanOrEqual; 
				break;
			}
			case 30: {
				lexer.NextToken();

#line  1817 "VBNET.ATG" 
				op = BinaryOperatorType.GreaterThanOrEqual; 
				break;
			}
			case 29: {
				lexer.NextToken();

#line  1818 "VBNET.ATG" 
				op = BinaryOperatorType.InEquality; 
				break;
			}
			case 10: {
				lexer.NextToken();

#line  1819 "VBNET.ATG" 
				op = BinaryOperatorType.Equality; 
				break;
			}
			case 126: {
				lexer.NextToken();

#line  1820 "VBNET.ATG" 
				op = BinaryOperatorType.Like; 
				break;
			}
			case 122: {
				lexer.NextToken();

#line  1821 "VBNET.ATG" 
				op = BinaryOperatorType.ReferenceEquality; 
				break;
			}
			case 123: {
				lexer.NextToken();

#line  1822 "VBNET.ATG" 
				op = BinaryOperatorType.ReferenceInequality; 
				break;
			}
			}
			if (StartOf(33)) {
				ShiftExpr(
#line  1825 "VBNET.ATG" 
out expr);

#line  1825 "VBNET.ATG" 
				outExpr = new BinaryOperatorExpression(outExpr, op, expr);  
			} else if (la.kind == 140) {
				lexer.NextToken();
				ShiftExpr(
#line  1828 "VBNET.ATG" 
out expr);

#line  1828 "VBNET.ATG" 
				outExpr = new BinaryOperatorExpression(outExpr, op, new UnaryOperatorExpression(expr, UnaryOperatorType.Not));  
			} else SynErr(248);
		}
	}

	void ShiftExpr(
#line  1833 "VBNET.ATG" 
out Expression outExpr) {

#line  1835 "VBNET.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		ConcatenationExpr(
#line  1838 "VBNET.ATG" 
out outExpr);
		while (la.kind == 32 || la.kind == 33) {
			if (la.kind == 32) {
				lexer.NextToken();

#line  1841 "VBNET.ATG" 
				op = BinaryOperatorType.ShiftLeft; 
			} else {
				lexer.NextToken();

#line  1842 "VBNET.ATG" 
				op = BinaryOperatorType.ShiftRight; 
			}
			ConcatenationExpr(
#line  1844 "VBNET.ATG" 
out expr);

#line  1844 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr);  
		}
	}

	void ConcatenationExpr(
#line  1848 "VBNET.ATG" 
out Expression outExpr) {

#line  1849 "VBNET.ATG" 
		Expression expr; 
		AdditiveExpr(
#line  1851 "VBNET.ATG" 
out outExpr);
		while (la.kind == 13) {
			lexer.NextToken();
			AdditiveExpr(
#line  1851 "VBNET.ATG" 
out expr);

#line  1851 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.Concat, expr);  
		}
	}

	void AdditiveExpr(
#line  1854 "VBNET.ATG" 
out Expression outExpr) {

#line  1856 "VBNET.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		ModuloExpr(
#line  1859 "VBNET.ATG" 
out outExpr);
		while (la.kind == 18 || la.kind == 19) {
			if (la.kind == 19) {
				lexer.NextToken();

#line  1862 "VBNET.ATG" 
				op = BinaryOperatorType.Add; 
			} else {
				lexer.NextToken();

#line  1863 "VBNET.ATG" 
				op = BinaryOperatorType.Subtract; 
			}
			ModuloExpr(
#line  1865 "VBNET.ATG" 
out expr);

#line  1865 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr);  
		}
	}

	void ModuloExpr(
#line  1869 "VBNET.ATG" 
out Expression outExpr) {

#line  1870 "VBNET.ATG" 
		Expression expr; 
		IntegerDivisionExpr(
#line  1872 "VBNET.ATG" 
out outExpr);
		while (la.kind == 130) {
			lexer.NextToken();
			IntegerDivisionExpr(
#line  1872 "VBNET.ATG" 
out expr);

#line  1872 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.Modulus, expr);  
		}
	}

	void IntegerDivisionExpr(
#line  1875 "VBNET.ATG" 
out Expression outExpr) {

#line  1876 "VBNET.ATG" 
		Expression expr; 
		MultiplicativeExpr(
#line  1878 "VBNET.ATG" 
out outExpr);
		while (la.kind == 15) {
			lexer.NextToken();
			MultiplicativeExpr(
#line  1878 "VBNET.ATG" 
out expr);

#line  1878 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.DivideInteger, expr);  
		}
	}

	void MultiplicativeExpr(
#line  1881 "VBNET.ATG" 
out Expression outExpr) {

#line  1883 "VBNET.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		UnaryExpr(
#line  1886 "VBNET.ATG" 
out outExpr);
		while (la.kind == 14 || la.kind == 22) {
			if (la.kind == 22) {
				lexer.NextToken();

#line  1889 "VBNET.ATG" 
				op = BinaryOperatorType.Multiply; 
			} else {
				lexer.NextToken();

#line  1890 "VBNET.ATG" 
				op = BinaryOperatorType.Divide; 
			}
			UnaryExpr(
#line  1892 "VBNET.ATG" 
out expr);

#line  1892 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr); 
		}
	}

	void UnaryExpr(
#line  1896 "VBNET.ATG" 
out Expression uExpr) {

#line  1898 "VBNET.ATG" 
		Expression expr;
		UnaryOperatorType uop = UnaryOperatorType.None;
		bool isUOp = false;
		
		while (la.kind == 18 || la.kind == 19 || la.kind == 22) {
			if (la.kind == 19) {
				lexer.NextToken();

#line  1902 "VBNET.ATG" 
				uop = UnaryOperatorType.Plus; isUOp = true; 
			} else if (la.kind == 18) {
				lexer.NextToken();

#line  1903 "VBNET.ATG" 
				uop = UnaryOperatorType.Minus; isUOp = true; 
			} else {
				lexer.NextToken();

#line  1904 "VBNET.ATG" 
				uop = UnaryOperatorType.Dereference;  isUOp = true;
			}
		}
		ExponentiationExpr(
#line  1906 "VBNET.ATG" 
out expr);

#line  1908 "VBNET.ATG" 
		if (isUOp) {
		uExpr = new UnaryOperatorExpression(expr, uop);
		} else {
			uExpr = expr;
		}
		
	}

	void ExponentiationExpr(
#line  1916 "VBNET.ATG" 
out Expression outExpr) {

#line  1917 "VBNET.ATG" 
		Expression expr; 
		SimpleExpr(
#line  1919 "VBNET.ATG" 
out outExpr);
		while (la.kind == 20) {
			lexer.NextToken();
			SimpleExpr(
#line  1919 "VBNET.ATG" 
out expr);

#line  1919 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.Power, expr);  
		}
	}

	void NormalOrReDimArgumentList(
#line  2014 "VBNET.ATG" 
out List<Expression> arguments, out bool canBeNormal, out bool canBeRedim) {

#line  2016 "VBNET.ATG" 
		arguments = new List<Expression>();
		canBeNormal = true; canBeRedim = !IsNamedAssign();
		Expression expr = null;
		
		if (StartOf(27)) {
			Argument(
#line  2021 "VBNET.ATG" 
out expr);
			if (la.kind == 188) {
				lexer.NextToken();

#line  2022 "VBNET.ATG" 
				EnsureIsZero(expr); canBeNormal = false; 
				Expr(
#line  2023 "VBNET.ATG" 
out expr);
			}
		}
		while (la.kind == 12) {
			lexer.NextToken();

#line  2026 "VBNET.ATG" 
			if (expr == null) canBeRedim = false; 

#line  2027 "VBNET.ATG" 
			arguments.Add(expr ?? Expression.Null); expr = null; 

#line  2028 "VBNET.ATG" 
			canBeRedim &= !IsNamedAssign(); 
			if (StartOf(27)) {
				Argument(
#line  2029 "VBNET.ATG" 
out expr);
				if (la.kind == 188) {
					lexer.NextToken();

#line  2030 "VBNET.ATG" 
					EnsureIsZero(expr); canBeNormal = false; 
					Expr(
#line  2031 "VBNET.ATG" 
out expr);
				}
			}

#line  2033 "VBNET.ATG" 
			if (expr == null) { canBeRedim = false; expr = Expression.Null; } 
		}

#line  2035 "VBNET.ATG" 
		if (expr != null) arguments.Add(expr); else canBeRedim = false; 
	}

	void ArrayTypeModifiers(
#line  2142 "VBNET.ATG" 
out ArrayList arrayModifiers) {

#line  2144 "VBNET.ATG" 
		arrayModifiers = new ArrayList();
		int i = 0;
		
		while (
#line  2147 "VBNET.ATG" 
IsDims()) {
			Expect(25);
			if (la.kind == 12 || la.kind == 26) {
				RankList(
#line  2149 "VBNET.ATG" 
out i);
			}

#line  2151 "VBNET.ATG" 
			arrayModifiers.Add(i);
			
			Expect(26);
		}

#line  2156 "VBNET.ATG" 
		if(arrayModifiers.Count == 0) {
		 arrayModifiers = null;
		}
		
	}

	void MemberInitializer(
#line  1984 "VBNET.ATG" 
out NamedArgumentExpression memberInitializer) {

#line  1986 "VBNET.ATG" 
		memberInitializer = new NamedArgumentExpression();
		memberInitializer.StartLocation = la.Location;
		Expression initExpr = null;
		string name = null;
		
		Expect(16);
		IdentifierOrKeyword(
#line  1991 "VBNET.ATG" 
out name);
		Expect(10);
		Expr(
#line  1991 "VBNET.ATG" 
out initExpr);

#line  1993 "VBNET.ATG" 
		memberInitializer.Name = name;
		memberInitializer.Expression = initExpr;
		memberInitializer.EndLocation = t.EndLocation;
		
	}

	void Argument(
#line  2039 "VBNET.ATG" 
out Expression argumentexpr) {

#line  2041 "VBNET.ATG" 
		Expression expr;
		argumentexpr = null;
		string name;
		
		if (
#line  2045 "VBNET.ATG" 
IsNamedAssign()) {
			Identifier();

#line  2045 "VBNET.ATG" 
			name = t.val;  
			Expect(11);
			Expect(10);
			Expr(
#line  2045 "VBNET.ATG" 
out expr);

#line  2047 "VBNET.ATG" 
			argumentexpr = new NamedArgumentExpression(name, expr);
			
		} else if (StartOf(27)) {
			Expr(
#line  2050 "VBNET.ATG" 
out argumentexpr);
		} else SynErr(249);
	}

	void QualIdentAndTypeArguments(
#line  2116 "VBNET.ATG" 
out TypeReference typeref, bool canBeUnbound) {

#line  2117 "VBNET.ATG" 
		string name; typeref = null; 
		Qualident(
#line  2119 "VBNET.ATG" 
out name);

#line  2120 "VBNET.ATG" 
		typeref = new TypeReference(name); 
		if (
#line  2121 "VBNET.ATG" 
la.kind == Tokens.OpenParenthesis && Peek(1).kind == Tokens.Of) {
			lexer.NextToken();
			Expect(145);
			if (
#line  2123 "VBNET.ATG" 
canBeUnbound && (la.kind == Tokens.CloseParenthesis || la.kind == Tokens.Comma)) {

#line  2124 "VBNET.ATG" 
				typeref.GenericTypes.Add(NullTypeReference.Instance); 
				while (la.kind == 12) {
					lexer.NextToken();

#line  2125 "VBNET.ATG" 
					typeref.GenericTypes.Add(NullTypeReference.Instance); 
				}
			} else if (StartOf(6)) {
				TypeArgumentList(
#line  2126 "VBNET.ATG" 
typeref.GenericTypes);
			} else SynErr(250);
			Expect(26);
		}
	}

	void RankList(
#line  2163 "VBNET.ATG" 
out int i) {

#line  2164 "VBNET.ATG" 
		i = 0; 
		while (la.kind == 12) {
			lexer.NextToken();

#line  2165 "VBNET.ATG" 
			++i; 
		}
	}

	void Attribute(
#line  2204 "VBNET.ATG" 
out ASTAttribute attribute) {

#line  2205 "VBNET.ATG" 
		string name;
		List<Expression> positional = new List<Expression>();
		List<NamedArgumentExpression> named = new List<NamedArgumentExpression>();
		
		if (la.kind == 110) {
			lexer.NextToken();
			Expect(16);
		}
		Qualident(
#line  2210 "VBNET.ATG" 
out name);
		if (la.kind == 25) {
			AttributeArguments(
#line  2211 "VBNET.ATG" 
positional, named);
		}

#line  2213 "VBNET.ATG" 
		attribute  = new ASTAttribute(name, positional, named);
		SetParent(positional, attribute);			
		SetParent(named, attribute);			
		
	}

	void AttributeArguments(
#line  2220 "VBNET.ATG" 
List<Expression> positional, List<NamedArgumentExpression> named) {

#line  2222 "VBNET.ATG" 
		bool nameFound = false;
		string name = "";
		Expression expr;
		
		Expect(25);
		if (
#line  2228 "VBNET.ATG" 
IsNotClosingParenthesis()) {
			if (
#line  2230 "VBNET.ATG" 
IsNamedAssign()) {

#line  2230 "VBNET.ATG" 
				nameFound = true; 
				IdentifierOrKeyword(
#line  2231 "VBNET.ATG" 
out name);
				if (la.kind == 11) {
					lexer.NextToken();
				}
				Expect(10);
			}
			Expr(
#line  2233 "VBNET.ATG" 
out expr);

#line  2235 "VBNET.ATG" 
			if (expr != null) {
			if (string.IsNullOrEmpty(name)) { positional.Add(expr); }
			else { named.Add(new NamedArgumentExpression(name, expr)); name = ""; }
			}
			
			while (la.kind == 12) {
				lexer.NextToken();
				if (
#line  2243 "VBNET.ATG" 
IsNamedAssign()) {

#line  2243 "VBNET.ATG" 
					nameFound = true; 
					IdentifierOrKeyword(
#line  2244 "VBNET.ATG" 
out name);
					if (la.kind == 11) {
						lexer.NextToken();
					}
					Expect(10);
				} else if (StartOf(27)) {

#line  2246 "VBNET.ATG" 
					if (nameFound) Error("no positional argument after named argument"); 
				} else SynErr(251);
				Expr(
#line  2247 "VBNET.ATG" 
out expr);

#line  2247 "VBNET.ATG" 
				if (expr != null) { if(name == "") positional.Add(expr);
				else { named.Add(new NamedArgumentExpression(name, expr)); name = ""; }
				}
				
			}
		}
		Expect(26);
	}

	void FormalParameter(
#line  2304 "VBNET.ATG" 
out ParameterDeclarationExpression p) {

#line  2306 "VBNET.ATG" 
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		TypeReference type = null;
		ParamModifierList mod = new ParamModifierList(this);
		Expression expr = null;
		p = null;
		ArrayList arrayModifiers = null;
		
		while (la.kind == 28) {
			AttributeSection(
#line  2315 "VBNET.ATG" 
out section);

#line  2315 "VBNET.ATG" 
			attributes.Add(section); 
		}
		while (StartOf(34)) {
			ParameterModifier(
#line  2316 "VBNET.ATG" 
mod);
		}
		Identifier();

#line  2317 "VBNET.ATG" 
		string parameterName = t.val; 
		if (
#line  2318 "VBNET.ATG" 
IsDims()) {
			ArrayTypeModifiers(
#line  2318 "VBNET.ATG" 
out arrayModifiers);
		}
		if (la.kind == 49) {
			lexer.NextToken();
			TypeName(
#line  2319 "VBNET.ATG" 
out type);
		}

#line  2321 "VBNET.ATG" 
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
		
		if (la.kind == 10) {
			lexer.NextToken();
			Expr(
#line  2333 "VBNET.ATG" 
out expr);
		}

#line  2335 "VBNET.ATG" 
		mod.Check();
		p = new ParameterDeclarationExpression(type, parameterName, mod.Modifier, expr);
		p.Attributes = attributes;
		
	}

	void ParameterModifier(
#line  3007 "VBNET.ATG" 
ParamModifierList m) {
		if (la.kind == 56) {
			lexer.NextToken();

#line  3008 "VBNET.ATG" 
			m.Add(ParameterModifiers.In); 
		} else if (la.kind == 54) {
			lexer.NextToken();

#line  3009 "VBNET.ATG" 
			m.Add(ParameterModifiers.Ref); 
		} else if (la.kind == 150) {
			lexer.NextToken();

#line  3010 "VBNET.ATG" 
			m.Add(ParameterModifiers.Optional); 
		} else if (la.kind == 156) {
			lexer.NextToken();

#line  3011 "VBNET.ATG" 
			m.Add(ParameterModifiers.Params); 
		} else SynErr(252);
	}

	void Statement() {

#line  2364 "VBNET.ATG" 
		Statement stmt = null;
		Location startPos = la.Location;
		string label = String.Empty;
		
		
		if (la.kind == 1 || la.kind == 11) {
		} else if (
#line  2370 "VBNET.ATG" 
IsLabel()) {
			LabelName(
#line  2370 "VBNET.ATG" 
out label);

#line  2372 "VBNET.ATG" 
			compilationUnit.AddChild(new LabelStatement(t.val));
			
			Expect(11);
			Statement();
		} else if (StartOf(35)) {
			EmbeddedStatement(
#line  2375 "VBNET.ATG" 
out stmt);

#line  2375 "VBNET.ATG" 
			compilationUnit.AddChild(stmt); 
		} else if (StartOf(36)) {
			LocalDeclarationStatement(
#line  2376 "VBNET.ATG" 
out stmt);

#line  2376 "VBNET.ATG" 
			compilationUnit.AddChild(stmt); 
		} else SynErr(253);

#line  2379 "VBNET.ATG" 
		if (stmt != null) {
		stmt.StartLocation = startPos;
		stmt.EndLocation = t.Location;
		}
		
	}

	void LabelName(
#line  2785 "VBNET.ATG" 
out string name) {

#line  2787 "VBNET.ATG" 
		name = String.Empty;
		
		if (StartOf(13)) {
			Identifier();

#line  2789 "VBNET.ATG" 
			name = t.val; 
		} else if (la.kind == 5) {
			lexer.NextToken();

#line  2790 "VBNET.ATG" 
			name = t.val; 
		} else SynErr(254);
	}

	void EmbeddedStatement(
#line  2419 "VBNET.ATG" 
out Statement statement) {

#line  2421 "VBNET.ATG" 
		Statement embeddedStatement = null;
		statement = null;
		Expression expr = null;
		string name = String.Empty;
		List<Expression> p = null;
		
		switch (la.kind) {
		case 101: {
			lexer.NextToken();

#line  2427 "VBNET.ATG" 
			ExitType exitType = ExitType.None; 
			switch (la.kind) {
			case 183: {
				lexer.NextToken();

#line  2429 "VBNET.ATG" 
				exitType = ExitType.Sub; 
				break;
			}
			case 107: {
				lexer.NextToken();

#line  2431 "VBNET.ATG" 
				exitType = ExitType.Function; 
				break;
			}
			case 160: {
				lexer.NextToken();

#line  2433 "VBNET.ATG" 
				exitType = ExitType.Property; 
				break;
			}
			case 90: {
				lexer.NextToken();

#line  2435 "VBNET.ATG" 
				exitType = ExitType.Do; 
				break;
			}
			case 105: {
				lexer.NextToken();

#line  2437 "VBNET.ATG" 
				exitType = ExitType.For; 
				break;
			}
			case 190: {
				lexer.NextToken();

#line  2439 "VBNET.ATG" 
				exitType = ExitType.Try; 
				break;
			}
			case 202: {
				lexer.NextToken();

#line  2441 "VBNET.ATG" 
				exitType = ExitType.While; 
				break;
			}
			case 171: {
				lexer.NextToken();

#line  2443 "VBNET.ATG" 
				exitType = ExitType.Select; 
				break;
			}
			default: SynErr(255); break;
			}

#line  2445 "VBNET.ATG" 
			statement = new ExitStatement(exitType); 
			break;
		}
		case 190: {
			TryStatement(
#line  2446 "VBNET.ATG" 
out statement);
			break;
		}
		case 73: {
			lexer.NextToken();

#line  2447 "VBNET.ATG" 
			ContinueType continueType = ContinueType.None; 
			if (la.kind == 90 || la.kind == 105 || la.kind == 202) {
				if (la.kind == 90) {
					lexer.NextToken();

#line  2447 "VBNET.ATG" 
					continueType = ContinueType.Do; 
				} else if (la.kind == 105) {
					lexer.NextToken();

#line  2447 "VBNET.ATG" 
					continueType = ContinueType.For; 
				} else {
					lexer.NextToken();

#line  2447 "VBNET.ATG" 
					continueType = ContinueType.While; 
				}
			}

#line  2447 "VBNET.ATG" 
			statement = new ContinueStatement(continueType); 
			break;
		}
		case 187: {
			lexer.NextToken();
			if (StartOf(27)) {
				Expr(
#line  2449 "VBNET.ATG" 
out expr);
			}

#line  2449 "VBNET.ATG" 
			statement = new ThrowStatement(expr); 
			break;
		}
		case 169: {
			lexer.NextToken();
			if (StartOf(27)) {
				Expr(
#line  2451 "VBNET.ATG" 
out expr);
			}

#line  2451 "VBNET.ATG" 
			statement = new ReturnStatement(expr); 
			break;
		}
		case 184: {
			lexer.NextToken();
			Expr(
#line  2453 "VBNET.ATG" 
out expr);
			EndOfStmt();
			Block(
#line  2453 "VBNET.ATG" 
out embeddedStatement);
			Expect(95);
			Expect(184);

#line  2454 "VBNET.ATG" 
			statement = new LockStatement(expr, embeddedStatement); 
			break;
		}
		case 163: {
			lexer.NextToken();
			Identifier();

#line  2456 "VBNET.ATG" 
			name = t.val; 
			if (la.kind == 25) {
				lexer.NextToken();
				if (StartOf(37)) {
					ArgumentList(
#line  2457 "VBNET.ATG" 
out p);
				}
				Expect(26);
			}

#line  2459 "VBNET.ATG" 
			statement = new RaiseEventStatement(name, p);
			SetParent(p, statement);
			
			break;
		}
		case 204: {
			WithStatement(
#line  2463 "VBNET.ATG" 
out statement);
			break;
		}
		case 43: {
			lexer.NextToken();

#line  2465 "VBNET.ATG" 
			Expression handlerExpr = null; 
			Expr(
#line  2466 "VBNET.ATG" 
out expr);
			Expect(12);
			Expr(
#line  2466 "VBNET.ATG" 
out handlerExpr);

#line  2468 "VBNET.ATG" 
			statement = new AddHandlerStatement(expr, handlerExpr);
			
			break;
		}
		case 167: {
			lexer.NextToken();

#line  2471 "VBNET.ATG" 
			Expression handlerExpr = null; 
			Expr(
#line  2472 "VBNET.ATG" 
out expr);
			Expect(12);
			Expr(
#line  2472 "VBNET.ATG" 
out handlerExpr);

#line  2474 "VBNET.ATG" 
			statement = new RemoveHandlerStatement(expr, handlerExpr);
			
			break;
		}
		case 202: {
			lexer.NextToken();
			Expr(
#line  2477 "VBNET.ATG" 
out expr);
			EndOfStmt();
			Block(
#line  2478 "VBNET.ATG" 
out embeddedStatement);
			Expect(95);
			Expect(202);

#line  2480 "VBNET.ATG" 
			statement = new DoLoopStatement(expr, embeddedStatement, ConditionType.While, ConditionPosition.Start);
			
			break;
		}
		case 90: {
			lexer.NextToken();

#line  2485 "VBNET.ATG" 
			ConditionType conditionType = ConditionType.None;
			
			if (la.kind == 196 || la.kind == 202) {
				WhileOrUntil(
#line  2488 "VBNET.ATG" 
out conditionType);
				Expr(
#line  2488 "VBNET.ATG" 
out expr);
				EndOfStmt();
				Block(
#line  2489 "VBNET.ATG" 
out embeddedStatement);
				Expect(128);

#line  2492 "VBNET.ATG" 
				statement = new DoLoopStatement(expr, 
				                               embeddedStatement, 
				                               conditionType == ConditionType.While ? ConditionType.DoWhile : conditionType, 
				                               ConditionPosition.Start);
				
			} else if (la.kind == 1 || la.kind == 11) {
				EndOfStmt();
				Block(
#line  2499 "VBNET.ATG" 
out embeddedStatement);
				Expect(128);
				if (la.kind == 196 || la.kind == 202) {
					WhileOrUntil(
#line  2500 "VBNET.ATG" 
out conditionType);
					Expr(
#line  2500 "VBNET.ATG" 
out expr);
				}

#line  2502 "VBNET.ATG" 
				statement = new DoLoopStatement(expr, embeddedStatement, conditionType, ConditionPosition.End);
				
			} else SynErr(256);
			break;
		}
		case 105: {
			lexer.NextToken();

#line  2507 "VBNET.ATG" 
			Expression group = null;
			TypeReference typeReference;
			string        typeName;
			Location startLocation = t.Location;
			
			if (la.kind == 92) {
				lexer.NextToken();
				LoopControlVariable(
#line  2514 "VBNET.ATG" 
out typeReference, out typeName);
				Expect(117);
				Expr(
#line  2515 "VBNET.ATG" 
out group);
				EndOfStmt();
				Block(
#line  2516 "VBNET.ATG" 
out embeddedStatement);
				Expect(139);
				if (StartOf(27)) {
					Expr(
#line  2517 "VBNET.ATG" 
out expr);
				}

#line  2519 "VBNET.ATG" 
				statement = new ForeachStatement(typeReference, 
				                                typeName,
				                                group, 
				                                embeddedStatement, 
				                                expr);
				statement.StartLocation = startLocation;
				statement.EndLocation   = t.EndLocation;
				
				
			} else if (StartOf(13)) {

#line  2530 "VBNET.ATG" 
				Expression start = null;
				Expression end = null;
				Expression step = null;
				Expression nextExpr = null;List<Expression> nextExpressions = null;
				
				LoopControlVariable(
#line  2535 "VBNET.ATG" 
out typeReference, out typeName);
				Expect(10);
				Expr(
#line  2536 "VBNET.ATG" 
out start);
				Expect(188);
				Expr(
#line  2536 "VBNET.ATG" 
out end);
				if (la.kind == 178) {
					lexer.NextToken();
					Expr(
#line  2536 "VBNET.ATG" 
out step);
				}
				EndOfStmt();
				Block(
#line  2537 "VBNET.ATG" 
out embeddedStatement);
				Expect(139);
				if (StartOf(27)) {
					Expr(
#line  2540 "VBNET.ATG" 
out nextExpr);

#line  2542 "VBNET.ATG" 
					nextExpressions = new List<Expression>();
					nextExpressions.Add(nextExpr);
					
					while (la.kind == 12) {
						lexer.NextToken();
						Expr(
#line  2545 "VBNET.ATG" 
out nextExpr);

#line  2545 "VBNET.ATG" 
						nextExpressions.Add(nextExpr); 
					}
				}

#line  2548 "VBNET.ATG" 
				statement = new ForNextStatement(typeReference, typeName, start, end, step, embeddedStatement, nextExpressions);
				SetParent(nextExpressions, statement);
				
			} else SynErr(257);
			break;
		}
		case 99: {
			lexer.NextToken();
			Expr(
#line  2553 "VBNET.ATG" 
out expr);

#line  2553 "VBNET.ATG" 
			statement = new ErrorStatement(expr); 
			break;
		}
		case 165: {
			lexer.NextToken();

#line  2555 "VBNET.ATG" 
			bool isPreserve = false; 
			if (la.kind == 158) {
				lexer.NextToken();

#line  2555 "VBNET.ATG" 
				isPreserve = true; 
			}
			ReDimClause(
#line  2556 "VBNET.ATG" 
out expr);

#line  2558 "VBNET.ATG" 
			ReDimStatement reDimStatement = new ReDimStatement(isPreserve);
			statement = reDimStatement;
			SafeAdd(reDimStatement, reDimStatement.ReDimClauses, expr as InvocationExpression);
			
			while (la.kind == 12) {
				lexer.NextToken();
				ReDimClause(
#line  2562 "VBNET.ATG" 
out expr);

#line  2563 "VBNET.ATG" 
				SafeAdd(reDimStatement, reDimStatement.ReDimClauses, expr as InvocationExpression); 
			}
			break;
		}
		case 98: {
			lexer.NextToken();
			Expr(
#line  2567 "VBNET.ATG" 
out expr);

#line  2569 "VBNET.ATG" 
			EraseStatement eraseStatement = new EraseStatement();
			if (expr != null) { SafeAdd(eraseStatement, eraseStatement.Expressions, expr);}
			
			while (la.kind == 12) {
				lexer.NextToken();
				Expr(
#line  2572 "VBNET.ATG" 
out expr);

#line  2572 "VBNET.ATG" 
				if (expr != null) { SafeAdd(eraseStatement, eraseStatement.Expressions, expr); }
			}

#line  2573 "VBNET.ATG" 
			statement = eraseStatement; 
			break;
		}
		case 179: {
			lexer.NextToken();

#line  2575 "VBNET.ATG" 
			statement = new StopStatement(); 
			break;
		}
		case 114: {
			lexer.NextToken();

#line  2577 "VBNET.ATG" 
			Location ifStartLocation = t.Location; 
			Expr(
#line  2577 "VBNET.ATG" 
out expr);
			if (la.kind == 186) {
				lexer.NextToken();
			}
			if (la.kind == 1 || la.kind == 11) {
				EndOfStmt();
				Block(
#line  2580 "VBNET.ATG" 
out embeddedStatement);

#line  2582 "VBNET.ATG" 
				IfElseStatement ifStatement = new IfElseStatement(expr, embeddedStatement);
				ifStatement.StartLocation = ifStartLocation;
				Location elseIfStart;
				
				while (la.kind == 94 || 
#line  2588 "VBNET.ATG" 
IsElseIf()) {
					if (
#line  2588 "VBNET.ATG" 
IsElseIf()) {
						Expect(93);

#line  2588 "VBNET.ATG" 
						elseIfStart = t.Location; 
						Expect(114);
					} else {
						lexer.NextToken();

#line  2589 "VBNET.ATG" 
						elseIfStart = t.Location; 
					}

#line  2591 "VBNET.ATG" 
					Expression condition = null; Statement block = null; 
					Expr(
#line  2592 "VBNET.ATG" 
out condition);
					if (la.kind == 186) {
						lexer.NextToken();
					}
					EndOfStmt();
					Block(
#line  2593 "VBNET.ATG" 
out block);

#line  2595 "VBNET.ATG" 
					ElseIfSection elseIfSection = new ElseIfSection(condition, block);
					elseIfSection.StartLocation = elseIfStart;
					elseIfSection.EndLocation = t.Location;
					elseIfSection.Parent = ifStatement;
					ifStatement.ElseIfSections.Add(elseIfSection);
					
				}
				if (la.kind == 93) {
					lexer.NextToken();
					EndOfStmt();
					Block(
#line  2604 "VBNET.ATG" 
out embeddedStatement);

#line  2606 "VBNET.ATG" 
					ifStatement.FalseStatement.Add(embeddedStatement);
					
				}
				Expect(95);
				Expect(114);

#line  2610 "VBNET.ATG" 
				ifStatement.EndLocation = t.Location;
				statement = ifStatement;
				
			} else if (StartOf(38)) {

#line  2615 "VBNET.ATG" 
				IfElseStatement ifStatement = new IfElseStatement(expr);
				ifStatement.StartLocation = ifStartLocation;
				
				SingleLineStatementList(
#line  2618 "VBNET.ATG" 
ifStatement.TrueStatement);

#line  2620 "VBNET.ATG" 
				SetParent(ifStatement.TrueStatement, ifStatement);
				
				if (la.kind == 93) {
					lexer.NextToken();
					if (StartOf(38)) {
						SingleLineStatementList(
#line  2624 "VBNET.ATG" 
ifStatement.FalseStatement);
					}
				}

#line  2626 "VBNET.ATG" 
				ifStatement.EndLocation = t.Location; statement = ifStatement; 
			} else SynErr(258);
			break;
		}
		case 171: {
			lexer.NextToken();
			if (la.kind == 58) {
				lexer.NextToken();
			}
			Expr(
#line  2629 "VBNET.ATG" 
out expr);
			EndOfStmt();

#line  2630 "VBNET.ATG" 
			List<SwitchSection> selectSections = new List<SwitchSection>();
			Statement block = null;
			
			while (la.kind == 58) {

#line  2634 "VBNET.ATG" 
				List<CaseLabel> caseClauses = null; Location caseLocation = la.Location; 
				lexer.NextToken();
				CaseClauses(
#line  2635 "VBNET.ATG" 
out caseClauses);
				if (
#line  2635 "VBNET.ATG" 
IsNotStatementSeparator()) {
					lexer.NextToken();
				}
				EndOfStmt();

#line  2637 "VBNET.ATG" 
				SwitchSection selectSection = new SwitchSection(caseClauses);
				SetParent(caseClauses, selectSection);
				selectSection.StartLocation = caseLocation;
				
				Block(
#line  2641 "VBNET.ATG" 
out block);

#line  2643 "VBNET.ATG" 
				selectSection.Children = block.Children;
				selectSection.EndLocation = t.EndLocation;
				selectSections.Add(selectSection);
				
			}

#line  2649 "VBNET.ATG" 
			statement = new SwitchStatement(expr, selectSections);
			SetParent(selectSections, statement);
			
			Expect(95);
			Expect(171);
			break;
		}
		case 147: {

#line  2653 "VBNET.ATG" 
			OnErrorStatement onErrorStatement = null; 
			OnErrorStatement(
#line  2654 "VBNET.ATG" 
out onErrorStatement);

#line  2654 "VBNET.ATG" 
			statement = onErrorStatement; 
			break;
		}
		case 112: {

#line  2655 "VBNET.ATG" 
			GotoStatement goToStatement = null; 
			GotoStatement(
#line  2656 "VBNET.ATG" 
out goToStatement);

#line  2656 "VBNET.ATG" 
			statement = goToStatement; 
			break;
		}
		case 168: {

#line  2657 "VBNET.ATG" 
			ResumeStatement resumeStatement = null; 
			ResumeStatement(
#line  2658 "VBNET.ATG" 
out resumeStatement);

#line  2658 "VBNET.ATG" 
			statement = resumeStatement; 
			break;
		}
		case 2: case 3: case 4: case 5: case 6: case 7: case 8: case 9: case 16: case 25: case 44: case 48: case 50: case 51: case 52: case 53: case 55: case 60: case 61: case 62: case 63: case 64: case 65: case 66: case 67: case 69: case 70: case 71: case 74: case 75: case 76: case 77: case 78: case 79: case 80: case 81: case 82: case 83: case 84: case 89: case 91: case 102: case 103: case 109: case 110: case 120: case 127: case 129: case 134: case 135: case 138: case 141: case 144: case 146: case 158: case 170: case 175: case 176: case 181: case 185: case 189: case 191: case 192: case 193: case 194: case 195: case 196: case 197: {

#line  2661 "VBNET.ATG" 
			Expression val = null;
			AssignmentOperatorType op;
			
			bool mustBeAssignment = la.kind == Tokens.Plus  || la.kind == Tokens.Minus ||
			                        la.kind == Tokens.Not   || la.kind == Tokens.Times;
			
			SimpleExpr(
#line  2667 "VBNET.ATG" 
out expr);
			if (StartOf(39)) {
				AssignmentOperator(
#line  2669 "VBNET.ATG" 
out op);
				Expr(
#line  2669 "VBNET.ATG" 
out val);

#line  2669 "VBNET.ATG" 
				expr = new AssignmentExpression(expr, op, val); 
			} else if (la.kind == 1 || la.kind == 11 || la.kind == 93) {

#line  2670 "VBNET.ATG" 
				if (mustBeAssignment) Error("error in assignment."); 
			} else SynErr(259);

#line  2673 "VBNET.ATG" 
			// a field reference expression that stands alone is a
			// invocation expression without parantheses and arguments
			if(expr is MemberReferenceExpression || expr is IdentifierExpression) {
				expr = new InvocationExpression(expr);
			}
			statement = new ExpressionStatement(expr);
			
			break;
		}
		case 57: {
			lexer.NextToken();
			SimpleExpr(
#line  2680 "VBNET.ATG" 
out expr);

#line  2680 "VBNET.ATG" 
			statement = new ExpressionStatement(expr); 
			break;
		}
		case 198: {
			lexer.NextToken();

#line  2682 "VBNET.ATG" 
			Statement block;  
			if (
#line  2683 "VBNET.ATG" 
Peek(1).kind == Tokens.As) {

#line  2684 "VBNET.ATG" 
				LocalVariableDeclaration resourceAquisition = new LocalVariableDeclaration(Modifiers.None); 
				VariableDeclarator(
#line  2685 "VBNET.ATG" 
resourceAquisition.Variables);
				while (la.kind == 12) {
					lexer.NextToken();
					VariableDeclarator(
#line  2687 "VBNET.ATG" 
resourceAquisition.Variables);
				}
				Block(
#line  2689 "VBNET.ATG" 
out block);

#line  2691 "VBNET.ATG" 
				statement = new UsingStatement(resourceAquisition, block);
				SetParent(resourceAquisition.Variables, resourceAquisition);
				
			} else if (StartOf(27)) {
				Expr(
#line  2694 "VBNET.ATG" 
out expr);
				Block(
#line  2695 "VBNET.ATG" 
out block);

#line  2696 "VBNET.ATG" 
				statement = new UsingStatement(new ExpressionStatement(expr), block); 
			} else SynErr(260);
			Expect(95);
			Expect(198);
			break;
		}
		default: SynErr(261); break;
		}
	}

	void LocalDeclarationStatement(
#line  2387 "VBNET.ATG" 
out Statement statement) {

#line  2389 "VBNET.ATG" 
		ModifierList m = new ModifierList();
		LocalVariableDeclaration localVariableDeclaration;
		bool dimfound = false;
		
		while (la.kind == 72 || la.kind == 88 || la.kind == 177) {
			if (la.kind == 72) {
				lexer.NextToken();

#line  2395 "VBNET.ATG" 
				m.Add(Modifiers.Const, t.Location); 
			} else if (la.kind == 177) {
				lexer.NextToken();

#line  2396 "VBNET.ATG" 
				m.Add(Modifiers.Static, t.Location); 
			} else {
				lexer.NextToken();

#line  2397 "VBNET.ATG" 
				dimfound = true; 
			}
		}

#line  2400 "VBNET.ATG" 
		if(dimfound && (m.Modifier & Modifiers.Const) != 0) {
		Error("Dim is not allowed on constants.");
		}
		
		if(m.isNone && dimfound == false) {
			Error("Const, Dim or Static expected");
		}
		
		localVariableDeclaration = new LocalVariableDeclaration(m.Modifier);
		localVariableDeclaration.StartLocation = t.Location;
		
		VariableDeclarator(
#line  2411 "VBNET.ATG" 
localVariableDeclaration.Variables);
		while (la.kind == 12) {
			lexer.NextToken();
			VariableDeclarator(
#line  2412 "VBNET.ATG" 
localVariableDeclaration.Variables);
		}

#line  2414 "VBNET.ATG" 
		SetParent(localVariableDeclaration.Variables, localVariableDeclaration);
		statement = localVariableDeclaration;
		
	}

	void TryStatement(
#line  2900 "VBNET.ATG" 
out Statement tryStatement) {

#line  2902 "VBNET.ATG" 
		Statement blockStmt = null, finallyStmt = null;List<CatchClause> catchClauses = null;
		
		Expect(190);
		EndOfStmt();
		Block(
#line  2905 "VBNET.ATG" 
out blockStmt);
		if (la.kind == 59 || la.kind == 95 || la.kind == 104) {
			CatchClauses(
#line  2906 "VBNET.ATG" 
out catchClauses);
		}
		if (la.kind == 104) {
			lexer.NextToken();
			EndOfStmt();
			Block(
#line  2907 "VBNET.ATG" 
out finallyStmt);
		}
		Expect(95);
		Expect(190);

#line  2910 "VBNET.ATG" 
		tryStatement = new TryCatchStatement(blockStmt, catchClauses, finallyStmt);
		
	}

	void WithStatement(
#line  2880 "VBNET.ATG" 
out Statement withStatement) {

#line  2882 "VBNET.ATG" 
		Statement blockStmt = null;
		Expression expr = null;
		
		Expect(204);

#line  2885 "VBNET.ATG" 
		Location start = t.Location; 
		Expr(
#line  2886 "VBNET.ATG" 
out expr);
		EndOfStmt();

#line  2888 "VBNET.ATG" 
		withStatement = new WithStatement(expr);
		withStatement.StartLocation = start;
		
		Block(
#line  2891 "VBNET.ATG" 
out blockStmt);

#line  2893 "VBNET.ATG" 
		((WithStatement)withStatement).Body = (BlockStatement)blockStmt;
		
		Expect(95);
		Expect(204);

#line  2896 "VBNET.ATG" 
		withStatement.EndLocation = t.Location; 
	}

	void WhileOrUntil(
#line  2873 "VBNET.ATG" 
out ConditionType conditionType) {

#line  2874 "VBNET.ATG" 
		conditionType = ConditionType.None; 
		if (la.kind == 202) {
			lexer.NextToken();

#line  2875 "VBNET.ATG" 
			conditionType = ConditionType.While; 
		} else if (la.kind == 196) {
			lexer.NextToken();

#line  2876 "VBNET.ATG" 
			conditionType = ConditionType.Until; 
		} else SynErr(262);
	}

	void LoopControlVariable(
#line  2715 "VBNET.ATG" 
out TypeReference type, out string name) {

#line  2716 "VBNET.ATG" 
		ArrayList arrayModifiers = null;
		type = null;
		
		Qualident(
#line  2720 "VBNET.ATG" 
out name);
		if (
#line  2721 "VBNET.ATG" 
IsDims()) {
			ArrayTypeModifiers(
#line  2721 "VBNET.ATG" 
out arrayModifiers);
		}
		if (la.kind == 49) {
			lexer.NextToken();
			TypeName(
#line  2722 "VBNET.ATG" 
out type);

#line  2722 "VBNET.ATG" 
			if (name.IndexOf('.') > 0) { Error("No type def for 'for each' member indexer allowed."); } 
		}

#line  2724 "VBNET.ATG" 
		if (type != null) {
		if(type.RankSpecifier != null && arrayModifiers != null) {
			Error("array rank only allowed one time");
		} else if (arrayModifiers != null) {
			type.RankSpecifier = (int[])arrayModifiers.ToArray(typeof(int));
		}
		}
		
	}

	void ReDimClause(
#line  2794 "VBNET.ATG" 
out Expression expr) {
		SimpleNonInvocationExpression(
#line  2796 "VBNET.ATG" 
out expr);
		ReDimClauseInternal(
#line  2797 "VBNET.ATG" 
ref expr);
	}

	void SingleLineStatementList(
#line  2701 "VBNET.ATG" 
List<Statement> list) {

#line  2702 "VBNET.ATG" 
		Statement embeddedStatement = null; 
		if (la.kind == 95) {
			lexer.NextToken();

#line  2704 "VBNET.ATG" 
			embeddedStatement = new EndStatement(); 
		} else if (StartOf(35)) {
			EmbeddedStatement(
#line  2705 "VBNET.ATG" 
out embeddedStatement);
		} else SynErr(263);

#line  2706 "VBNET.ATG" 
		if (embeddedStatement != null) list.Add(embeddedStatement); 
		while (la.kind == 11) {
			lexer.NextToken();
			while (la.kind == 11) {
				lexer.NextToken();
			}
			if (la.kind == 95) {
				lexer.NextToken();

#line  2708 "VBNET.ATG" 
				embeddedStatement = new EndStatement(); 
			} else if (StartOf(35)) {
				EmbeddedStatement(
#line  2709 "VBNET.ATG" 
out embeddedStatement);
			} else SynErr(264);

#line  2710 "VBNET.ATG" 
			if (embeddedStatement != null) list.Add(embeddedStatement); 
		}
	}

	void CaseClauses(
#line  2833 "VBNET.ATG" 
out List<CaseLabel> caseClauses) {

#line  2835 "VBNET.ATG" 
		caseClauses = new List<CaseLabel>();
		CaseLabel caseClause = null;
		
		CaseClause(
#line  2838 "VBNET.ATG" 
out caseClause);

#line  2838 "VBNET.ATG" 
		if (caseClause != null) { caseClauses.Add(caseClause); } 
		while (la.kind == 12) {
			lexer.NextToken();
			CaseClause(
#line  2839 "VBNET.ATG" 
out caseClause);

#line  2839 "VBNET.ATG" 
			if (caseClause != null) { caseClauses.Add(caseClause); } 
		}
	}

	void OnErrorStatement(
#line  2735 "VBNET.ATG" 
out OnErrorStatement stmt) {

#line  2737 "VBNET.ATG" 
		stmt = null;
		GotoStatement goToStatement = null;
		
		Expect(147);
		Expect(99);
		if (
#line  2743 "VBNET.ATG" 
IsNegativeLabelName()) {
			Expect(112);
			Expect(18);
			Expect(5);

#line  2745 "VBNET.ATG" 
			long intLabel = Int64.Parse(t.val);
			if(intLabel != 1) {
				Error("invalid label in on error statement.");
			}
			stmt = new OnErrorStatement(new GotoStatement((intLabel * -1).ToString()));
			
		} else if (la.kind == 112) {
			GotoStatement(
#line  2751 "VBNET.ATG" 
out goToStatement);

#line  2753 "VBNET.ATG" 
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
			
		} else if (la.kind == 168) {
			lexer.NextToken();
			Expect(139);

#line  2767 "VBNET.ATG" 
			stmt = new OnErrorStatement(new ResumeStatement(true));
			
		} else SynErr(265);
	}

	void GotoStatement(
#line  2773 "VBNET.ATG" 
out GotoStatement goToStatement) {

#line  2775 "VBNET.ATG" 
		string label = String.Empty;
		
		Expect(112);
		LabelName(
#line  2778 "VBNET.ATG" 
out label);

#line  2780 "VBNET.ATG" 
		goToStatement = new GotoStatement(label);
		
	}

	void ResumeStatement(
#line  2822 "VBNET.ATG" 
out ResumeStatement resumeStatement) {

#line  2824 "VBNET.ATG" 
		resumeStatement = null;
		string label = String.Empty;
		
		if (
#line  2827 "VBNET.ATG" 
IsResumeNext()) {
			Expect(168);
			Expect(139);

#line  2828 "VBNET.ATG" 
			resumeStatement = new ResumeStatement(true); 
		} else if (la.kind == 168) {
			lexer.NextToken();
			if (StartOf(40)) {
				LabelName(
#line  2829 "VBNET.ATG" 
out label);
			}

#line  2829 "VBNET.ATG" 
			resumeStatement = new ResumeStatement(label); 
		} else SynErr(266);
	}

	void ReDimClauseInternal(
#line  2800 "VBNET.ATG" 
ref Expression expr) {

#line  2801 "VBNET.ATG" 
		List<Expression> arguments; bool canBeNormal; bool canBeRedim; string name; 
		while (la.kind == 16 || 
#line  2804 "VBNET.ATG" 
la.kind == Tokens.OpenParenthesis && Peek(1).kind == Tokens.Of) {
			if (la.kind == 16) {
				lexer.NextToken();
				IdentifierOrKeyword(
#line  2803 "VBNET.ATG" 
out name);

#line  2803 "VBNET.ATG" 
				expr = new MemberReferenceExpression(expr, name); 
			} else {
				InvocationExpression(
#line  2805 "VBNET.ATG" 
ref expr);
			}
		}
		Expect(25);
		NormalOrReDimArgumentList(
#line  2808 "VBNET.ATG" 
out arguments, out canBeNormal, out canBeRedim);
		Expect(26);

#line  2810 "VBNET.ATG" 
		expr = new InvocationExpression(expr, arguments);
		SetParent(arguments, expr);
		if (canBeRedim == false || canBeNormal && (la.kind == Tokens.Dot || la.kind == Tokens.OpenParenthesis)) {
			if (this.Errors.Count == 0) {
				// don't recurse on parse errors - could result in endless recursion
				ReDimClauseInternal(ref expr);
			}
		}
		
	}

	void CaseClause(
#line  2843 "VBNET.ATG" 
out CaseLabel caseClause) {

#line  2845 "VBNET.ATG" 
		Expression expr = null;
		Expression sexpr = null;
		BinaryOperatorType op = BinaryOperatorType.None;
		caseClause = null;
		
		if (la.kind == 93) {
			lexer.NextToken();

#line  2851 "VBNET.ATG" 
			caseClause = new CaseLabel(); 
		} else if (StartOf(41)) {
			if (la.kind == 122) {
				lexer.NextToken();
			}
			switch (la.kind) {
			case 28: {
				lexer.NextToken();

#line  2855 "VBNET.ATG" 
				op = BinaryOperatorType.LessThan; 
				break;
			}
			case 27: {
				lexer.NextToken();

#line  2856 "VBNET.ATG" 
				op = BinaryOperatorType.GreaterThan; 
				break;
			}
			case 31: {
				lexer.NextToken();

#line  2857 "VBNET.ATG" 
				op = BinaryOperatorType.LessThanOrEqual; 
				break;
			}
			case 30: {
				lexer.NextToken();

#line  2858 "VBNET.ATG" 
				op = BinaryOperatorType.GreaterThanOrEqual; 
				break;
			}
			case 10: {
				lexer.NextToken();

#line  2859 "VBNET.ATG" 
				op = BinaryOperatorType.Equality; 
				break;
			}
			case 29: {
				lexer.NextToken();

#line  2860 "VBNET.ATG" 
				op = BinaryOperatorType.InEquality; 
				break;
			}
			default: SynErr(267); break;
			}
			Expr(
#line  2862 "VBNET.ATG" 
out expr);

#line  2864 "VBNET.ATG" 
			caseClause = new CaseLabel(op, expr);
			
		} else if (StartOf(27)) {
			Expr(
#line  2866 "VBNET.ATG" 
out expr);
			if (la.kind == 188) {
				lexer.NextToken();
				Expr(
#line  2866 "VBNET.ATG" 
out sexpr);
			}

#line  2868 "VBNET.ATG" 
			caseClause = new CaseLabel(expr, sexpr);
			
		} else SynErr(268);
	}

	void CatchClauses(
#line  2915 "VBNET.ATG" 
out List<CatchClause> catchClauses) {

#line  2917 "VBNET.ATG" 
		catchClauses = new List<CatchClause>();
		TypeReference type = null;
		Statement blockStmt = null;
		Expression expr = null;
		string name = String.Empty;
		
		while (la.kind == 59) {
			lexer.NextToken();
			if (StartOf(13)) {
				Identifier();

#line  2925 "VBNET.ATG" 
				name = t.val; 
				if (la.kind == 49) {
					lexer.NextToken();
					TypeName(
#line  2925 "VBNET.ATG" 
out type);
				}
			}
			if (la.kind == 201) {
				lexer.NextToken();
				Expr(
#line  2926 "VBNET.ATG" 
out expr);
			}
			EndOfStmt();
			Block(
#line  2928 "VBNET.ATG" 
out blockStmt);

#line  2929 "VBNET.ATG" 
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
			case 10: s = "\"=\" expected"; break;
			case 11: s = "\":\" expected"; break;
			case 12: s = "\",\" expected"; break;
			case 13: s = "\"&\" expected"; break;
			case 14: s = "\"/\" expected"; break;
			case 15: s = "\"\\\\\" expected"; break;
			case 16: s = "\".\" expected"; break;
			case 17: s = "\"!\" expected"; break;
			case 18: s = "\"-\" expected"; break;
			case 19: s = "\"+\" expected"; break;
			case 20: s = "\"^\" expected"; break;
			case 21: s = "\"?\" expected"; break;
			case 22: s = "\"*\" expected"; break;
			case 23: s = "\"{\" expected"; break;
			case 24: s = "\"}\" expected"; break;
			case 25: s = "\"(\" expected"; break;
			case 26: s = "\")\" expected"; break;
			case 27: s = "\">\" expected"; break;
			case 28: s = "\"<\" expected"; break;
			case 29: s = "\"<>\" expected"; break;
			case 30: s = "\">=\" expected"; break;
			case 31: s = "\"<=\" expected"; break;
			case 32: s = "\"<<\" expected"; break;
			case 33: s = "\">>\" expected"; break;
			case 34: s = "\"+=\" expected"; break;
			case 35: s = "\"^=\" expected"; break;
			case 36: s = "\"-=\" expected"; break;
			case 37: s = "\"*=\" expected"; break;
			case 38: s = "\"/=\" expected"; break;
			case 39: s = "\"\\\\=\" expected"; break;
			case 40: s = "\"<<=\" expected"; break;
			case 41: s = "\">>=\" expected"; break;
			case 42: s = "\"&=\" expected"; break;
			case 43: s = "\"AddHandler\" expected"; break;
			case 44: s = "\"AddressOf\" expected"; break;
			case 45: s = "\"Alias\" expected"; break;
			case 46: s = "\"And\" expected"; break;
			case 47: s = "\"AndAlso\" expected"; break;
			case 48: s = "\"Ansi\" expected"; break;
			case 49: s = "\"As\" expected"; break;
			case 50: s = "\"Assembly\" expected"; break;
			case 51: s = "\"Auto\" expected"; break;
			case 52: s = "\"Binary\" expected"; break;
			case 53: s = "\"Boolean\" expected"; break;
			case 54: s = "\"ByRef\" expected"; break;
			case 55: s = "\"Byte\" expected"; break;
			case 56: s = "\"ByVal\" expected"; break;
			case 57: s = "\"Call\" expected"; break;
			case 58: s = "\"Case\" expected"; break;
			case 59: s = "\"Catch\" expected"; break;
			case 60: s = "\"CBool\" expected"; break;
			case 61: s = "\"CByte\" expected"; break;
			case 62: s = "\"CChar\" expected"; break;
			case 63: s = "\"CDate\" expected"; break;
			case 64: s = "\"CDbl\" expected"; break;
			case 65: s = "\"CDec\" expected"; break;
			case 66: s = "\"Char\" expected"; break;
			case 67: s = "\"CInt\" expected"; break;
			case 68: s = "\"Class\" expected"; break;
			case 69: s = "\"CLng\" expected"; break;
			case 70: s = "\"CObj\" expected"; break;
			case 71: s = "\"Compare\" expected"; break;
			case 72: s = "\"Const\" expected"; break;
			case 73: s = "\"Continue\" expected"; break;
			case 74: s = "\"CSByte\" expected"; break;
			case 75: s = "\"CShort\" expected"; break;
			case 76: s = "\"CSng\" expected"; break;
			case 77: s = "\"CStr\" expected"; break;
			case 78: s = "\"CType\" expected"; break;
			case 79: s = "\"CUInt\" expected"; break;
			case 80: s = "\"CULng\" expected"; break;
			case 81: s = "\"CUShort\" expected"; break;
			case 82: s = "\"Custom\" expected"; break;
			case 83: s = "\"Date\" expected"; break;
			case 84: s = "\"Decimal\" expected"; break;
			case 85: s = "\"Declare\" expected"; break;
			case 86: s = "\"Default\" expected"; break;
			case 87: s = "\"Delegate\" expected"; break;
			case 88: s = "\"Dim\" expected"; break;
			case 89: s = "\"DirectCast\" expected"; break;
			case 90: s = "\"Do\" expected"; break;
			case 91: s = "\"Double\" expected"; break;
			case 92: s = "\"Each\" expected"; break;
			case 93: s = "\"Else\" expected"; break;
			case 94: s = "\"ElseIf\" expected"; break;
			case 95: s = "\"End\" expected"; break;
			case 96: s = "\"EndIf\" expected"; break;
			case 97: s = "\"Enum\" expected"; break;
			case 98: s = "\"Erase\" expected"; break;
			case 99: s = "\"Error\" expected"; break;
			case 100: s = "\"Event\" expected"; break;
			case 101: s = "\"Exit\" expected"; break;
			case 102: s = "\"Explicit\" expected"; break;
			case 103: s = "\"False\" expected"; break;
			case 104: s = "\"Finally\" expected"; break;
			case 105: s = "\"For\" expected"; break;
			case 106: s = "\"Friend\" expected"; break;
			case 107: s = "\"Function\" expected"; break;
			case 108: s = "\"Get\" expected"; break;
			case 109: s = "\"GetType\" expected"; break;
			case 110: s = "\"Global\" expected"; break;
			case 111: s = "\"GoSub\" expected"; break;
			case 112: s = "\"GoTo\" expected"; break;
			case 113: s = "\"Handles\" expected"; break;
			case 114: s = "\"If\" expected"; break;
			case 115: s = "\"Implements\" expected"; break;
			case 116: s = "\"Imports\" expected"; break;
			case 117: s = "\"In\" expected"; break;
			case 118: s = "\"Infer\" expected"; break;
			case 119: s = "\"Inherits\" expected"; break;
			case 120: s = "\"Integer\" expected"; break;
			case 121: s = "\"Interface\" expected"; break;
			case 122: s = "\"Is\" expected"; break;
			case 123: s = "\"IsNot\" expected"; break;
			case 124: s = "\"Let\" expected"; break;
			case 125: s = "\"Lib\" expected"; break;
			case 126: s = "\"Like\" expected"; break;
			case 127: s = "\"Long\" expected"; break;
			case 128: s = "\"Loop\" expected"; break;
			case 129: s = "\"Me\" expected"; break;
			case 130: s = "\"Mod\" expected"; break;
			case 131: s = "\"Module\" expected"; break;
			case 132: s = "\"MustInherit\" expected"; break;
			case 133: s = "\"MustOverride\" expected"; break;
			case 134: s = "\"MyBase\" expected"; break;
			case 135: s = "\"MyClass\" expected"; break;
			case 136: s = "\"Namespace\" expected"; break;
			case 137: s = "\"Narrowing\" expected"; break;
			case 138: s = "\"New\" expected"; break;
			case 139: s = "\"Next\" expected"; break;
			case 140: s = "\"Not\" expected"; break;
			case 141: s = "\"Nothing\" expected"; break;
			case 142: s = "\"NotInheritable\" expected"; break;
			case 143: s = "\"NotOverridable\" expected"; break;
			case 144: s = "\"Object\" expected"; break;
			case 145: s = "\"Of\" expected"; break;
			case 146: s = "\"Off\" expected"; break;
			case 147: s = "\"On\" expected"; break;
			case 148: s = "\"Operator\" expected"; break;
			case 149: s = "\"Option\" expected"; break;
			case 150: s = "\"Optional\" expected"; break;
			case 151: s = "\"Or\" expected"; break;
			case 152: s = "\"OrElse\" expected"; break;
			case 153: s = "\"Overloads\" expected"; break;
			case 154: s = "\"Overridable\" expected"; break;
			case 155: s = "\"Overrides\" expected"; break;
			case 156: s = "\"ParamArray\" expected"; break;
			case 157: s = "\"Partial\" expected"; break;
			case 158: s = "\"Preserve\" expected"; break;
			case 159: s = "\"Private\" expected"; break;
			case 160: s = "\"Property\" expected"; break;
			case 161: s = "\"Protected\" expected"; break;
			case 162: s = "\"Public\" expected"; break;
			case 163: s = "\"RaiseEvent\" expected"; break;
			case 164: s = "\"ReadOnly\" expected"; break;
			case 165: s = "\"ReDim\" expected"; break;
			case 166: s = "\"Rem\" expected"; break;
			case 167: s = "\"RemoveHandler\" expected"; break;
			case 168: s = "\"Resume\" expected"; break;
			case 169: s = "\"Return\" expected"; break;
			case 170: s = "\"SByte\" expected"; break;
			case 171: s = "\"Select\" expected"; break;
			case 172: s = "\"Set\" expected"; break;
			case 173: s = "\"Shadows\" expected"; break;
			case 174: s = "\"Shared\" expected"; break;
			case 175: s = "\"Short\" expected"; break;
			case 176: s = "\"Single\" expected"; break;
			case 177: s = "\"Static\" expected"; break;
			case 178: s = "\"Step\" expected"; break;
			case 179: s = "\"Stop\" expected"; break;
			case 180: s = "\"Strict\" expected"; break;
			case 181: s = "\"String\" expected"; break;
			case 182: s = "\"Structure\" expected"; break;
			case 183: s = "\"Sub\" expected"; break;
			case 184: s = "\"SyncLock\" expected"; break;
			case 185: s = "\"Text\" expected"; break;
			case 186: s = "\"Then\" expected"; break;
			case 187: s = "\"Throw\" expected"; break;
			case 188: s = "\"To\" expected"; break;
			case 189: s = "\"True\" expected"; break;
			case 190: s = "\"Try\" expected"; break;
			case 191: s = "\"TryCast\" expected"; break;
			case 192: s = "\"TypeOf\" expected"; break;
			case 193: s = "\"UInteger\" expected"; break;
			case 194: s = "\"ULong\" expected"; break;
			case 195: s = "\"Unicode\" expected"; break;
			case 196: s = "\"Until\" expected"; break;
			case 197: s = "\"UShort\" expected"; break;
			case 198: s = "\"Using\" expected"; break;
			case 199: s = "\"Variant\" expected"; break;
			case 200: s = "\"Wend\" expected"; break;
			case 201: s = "\"When\" expected"; break;
			case 202: s = "\"While\" expected"; break;
			case 203: s = "\"Widening\" expected"; break;
			case 204: s = "\"With\" expected"; break;
			case 205: s = "\"WithEvents\" expected"; break;
			case 206: s = "\"WriteOnly\" expected"; break;
			case 207: s = "\"Xor\" expected"; break;
			case 208: s = "??? expected"; break;
			case 209: s = "invalid EndOfStmt"; break;
			case 210: s = "invalid OptionStmt"; break;
			case 211: s = "invalid OptionStmt"; break;
			case 212: s = "invalid GlobalAttributeSection"; break;
			case 213: s = "invalid GlobalAttributeSection"; break;
			case 214: s = "invalid NamespaceMemberDecl"; break;
			case 215: s = "invalid OptionValue"; break;
			case 216: s = "invalid TypeModifier"; break;
			case 217: s = "invalid NonModuleDeclaration"; break;
			case 218: s = "invalid NonModuleDeclaration"; break;
			case 219: s = "invalid Identifier"; break;
			case 220: s = "invalid TypeParameterConstraints"; break;
			case 221: s = "invalid TypeParameterConstraint"; break;
			case 222: s = "invalid NonArrayTypeName"; break;
			case 223: s = "invalid MemberModifier"; break;
			case 224: s = "invalid StructureMemberDecl"; break;
			case 225: s = "invalid StructureMemberDecl"; break;
			case 226: s = "invalid StructureMemberDecl"; break;
			case 227: s = "invalid StructureMemberDecl"; break;
			case 228: s = "invalid StructureMemberDecl"; break;
			case 229: s = "invalid StructureMemberDecl"; break;
			case 230: s = "invalid StructureMemberDecl"; break;
			case 231: s = "invalid InterfaceMemberDecl"; break;
			case 232: s = "invalid InterfaceMemberDecl"; break;
			case 233: s = "invalid Charset"; break;
			case 234: s = "invalid IdentifierForFieldDeclaration"; break;
			case 235: s = "invalid VariableDeclaratorPartAfterIdentifier"; break;
			case 236: s = "invalid AccessorDecls"; break;
			case 237: s = "invalid EventAccessorDeclaration"; break;
			case 238: s = "invalid OverloadableOperator"; break;
			case 239: s = "invalid VariableInitializer"; break;
			case 240: s = "invalid EventMemberSpecifier"; break;
			case 241: s = "invalid AssignmentOperator"; break;
			case 242: s = "invalid SimpleNonInvocationExpression"; break;
			case 243: s = "invalid SimpleNonInvocationExpression"; break;
			case 244: s = "invalid SimpleNonInvocationExpression"; break;
			case 245: s = "invalid SimpleNonInvocationExpression"; break;
			case 246: s = "invalid PrimitiveTypeName"; break;
			case 247: s = "invalid CastTarget"; break;
			case 248: s = "invalid ComparisonExpr"; break;
			case 249: s = "invalid Argument"; break;
			case 250: s = "invalid QualIdentAndTypeArguments"; break;
			case 251: s = "invalid AttributeArguments"; break;
			case 252: s = "invalid ParameterModifier"; break;
			case 253: s = "invalid Statement"; break;
			case 254: s = "invalid LabelName"; break;
			case 255: s = "invalid EmbeddedStatement"; break;
			case 256: s = "invalid EmbeddedStatement"; break;
			case 257: s = "invalid EmbeddedStatement"; break;
			case 258: s = "invalid EmbeddedStatement"; break;
			case 259: s = "invalid EmbeddedStatement"; break;
			case 260: s = "invalid EmbeddedStatement"; break;
			case 261: s = "invalid EmbeddedStatement"; break;
			case 262: s = "invalid WhileOrUntil"; break;
			case 263: s = "invalid SingleLineStatementList"; break;
			case 264: s = "invalid SingleLineStatementList"; break;
			case 265: s = "invalid OnErrorStatement"; break;
			case 266: s = "invalid ResumeStatement"; break;
			case 267: s = "invalid CaseClause"; break;
			case 268: s = "invalid CaseClause"; break;

			default: s = "error " + errorNumber; break;
		}
		this.Errors.Error(line, col, s);
	}
	
	private bool StartOf(int s)
	{
		return set[s, lexer.LookAhead.kind];
	}
	
	static bool[,] set = {
	{T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, x,T,T,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, x,T,T,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, x,T,T,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,T, T,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,T,x, x,x,x,x, T,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,T, T,T,x,T, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,T, x,x,x,x, x,x,x,x, x,x,T,T, T,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, T,x,T,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, T,x,x,x, x,T,T,x, x,T,x,x, x,x,x,x, x,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,T, T,T,x,T, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,T,T, T,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, T,x,x,x, x,T,x,x, x,T,x,x, x,x,x,x, x,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,T, T,x,x,x, x,x,x,x, x,x,T,x, x,T,T,T, T,x,x,x, x,x,x,x, x,T,x,x, T,x,T,x, x,x,T,T, x,x,x,x, x,x,x,x, x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,T, T,T,x,x, x,T,x,x, x,x,T,T, x,x,T,x, T,x,x,x, x,T,T,T, x,T,T,T, T,T,T,x, T,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,T,T, x,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,T, x,T,T,x, x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,T,T, x,T,x,T, x,T,T,x, T,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, T,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,x,x,x, x,x,x,x, x,T,x,x, T,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,T, T,T,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,T,T, x,T,x,T, T,T,T,x, T,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
	{x,T,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,T,T, x,T,x,T, T,T,T,x, T,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
	{x,T,T,T, T,T,T,T, T,T,x,T, x,x,x,x, T,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, T,x,T,T, T,T,x,T, x,T,x,x, T,T,T,T, T,T,T,T, x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x,x, T,T,T,T, x,x,x,x, x,x,T,T, x,T,T,T, x,T,x,x, x,T,T,x, T,x,T,x, x,x,x,x, T,x,x,x, x,x,x,T, x,T,x,x, x,x,T,T, x,x,T,x, x,T,x,x, T,x,T,T, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, x,T,x,T, T,T,T,T, x,x,x,T, T,T,x,T, x,T,x,x, T,T,x,T, x,T,T,T, T,T,T,T, T,T,T,x, x,x,T,x, T,x,x,x, x,x},
	{x,T,T,T, T,T,T,T, T,T,T,T, T,x,x,x, T,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, T,T,T,T, T,T,x,T, x,T,x,x, T,T,T,T, T,T,T,T, x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x,x, T,T,T,T, x,x,x,T, x,x,T,T, x,T,T,T, x,T,x,x, x,T,T,x, T,x,T,x, x,x,x,x, T,x,x,x, x,x,x,T, x,T,x,x, x,x,T,T, x,x,T,x, x,T,x,x, T,x,T,T, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, x,T,x,T, T,T,T,T, x,x,x,T, T,T,x,T, x,T,x,x, T,T,x,T, x,T,T,T, T,T,T,T, T,T,T,x, x,x,T,x, T,x,x,x, x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
	{x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, T,x,T,T, x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, T,x,T,T, T,T,x,T, x,x,x,x, T,T,T,T, T,T,T,T, x,T,T,T, x,x,T,T, T,T,T,T, T,T,T,T, T,x,x,x, x,T,x,T, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,T, x,T,x,x, x,x,T,T, x,x,T,x, T,T,x,x, T,x,T,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, T,x,x,x, x,T,x,x, x,T,x,x, x,T,x,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x},
	{x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, T,x,T,T, x,x,T,T, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, T,x,T,T, T,T,x,T, x,x,x,x, T,T,T,T, T,T,T,T, x,T,T,T, x,x,T,T, T,T,T,T, T,T,T,T, T,x,x,x, x,T,x,T, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,T, x,T,x,x, x,x,T,T, x,x,T,x, T,T,x,x, T,x,T,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, T,x,x,x, x,T,x,x, x,T,x,x, x,T,x,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x},
	{x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, T,x,T,T, T,T,x,T, x,x,x,x, T,T,T,T, T,T,T,T, x,T,T,T, x,x,T,T, T,T,T,T, T,T,T,T, T,x,x,x, x,T,x,T, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,T, x,T,x,x, x,x,T,T, x,x,T,x, x,T,x,x, T,x,T,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, T,x,x,x, x,T,x,x, x,T,x,x, x,T,x,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, T,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,T,T, T,T,x,T, x,T,T,x, x,x,T,T, T,T,x,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
	{x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
	{x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, T,x,T,T, x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, T,x,T,T, T,T,x,T, x,x,x,x, T,T,T,T, T,T,T,T, x,T,T,T, x,x,T,T, T,T,T,T, T,T,T,T, T,x,x,x, x,T,x,T, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,T, x,T,x,x, x,x,T,T, x,x,T,x, x,T,x,x, T,x,T,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, T,x,x,x, x,T,x,x, x,T,x,x, x,T,x,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
	{x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, T,x,T,T, T,T,x,T, x,T,x,x, T,T,T,T, T,T,T,T, x,T,T,T, x,T,T,T, T,T,T,T, T,T,T,T, T,x,x,x, x,T,T,T, x,x,x,x, x,x,T,T, x,T,T,T, x,T,x,x, x,T,T,x, T,x,T,x, x,x,x,x, T,x,x,x, x,x,x,T, x,T,x,x, x,x,T,T, x,x,T,x, x,T,x,x, T,x,T,T, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, x,T,x,T, T,T,T,T, x,x,x,T, T,x,x,T, x,T,x,x, T,T,x,T, x,T,T,T, T,T,T,T, T,T,T,x, x,x,T,x, T,x,x,x, x,x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x},
	{x,x,T,T, T,T,T,T, T,T,x,x, T,x,x,x, T,x,T,T, x,x,T,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, T,x,T,T, T,T,x,T, x,x,x,x, T,T,T,T, T,T,T,T, x,T,T,T, x,x,T,T, T,T,T,T, T,T,T,T, T,x,x,x, x,T,x,T, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,T, x,T,x,x, x,x,T,T, x,x,T,x, T,T,x,x, T,x,T,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, T,x,x,x, x,T,x,x, x,T,x,x, x,T,x,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x},
	{x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, T,x,T,T, T,T,x,T, x,T,x,x, T,T,T,T, T,T,T,T, x,T,T,T, x,T,T,T, T,T,T,T, T,T,T,T, T,x,x,x, x,T,T,T, x,x,x,T, x,x,T,T, x,T,T,T, x,T,x,x, x,T,T,x, T,x,T,x, x,x,x,x, T,x,x,x, x,x,x,T, x,T,x,x, x,x,T,T, x,x,T,x, x,T,x,x, T,x,T,T, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, x,T,x,T, T,T,T,T, x,x,x,T, T,x,x,T, x,T,x,x, T,T,x,T, x,T,T,T, T,T,T,T, T,T,T,x, x,x,T,x, T,x,x,x, x,x},
	{x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
	{x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x},
	{x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x}

	};
} // end Parser

}