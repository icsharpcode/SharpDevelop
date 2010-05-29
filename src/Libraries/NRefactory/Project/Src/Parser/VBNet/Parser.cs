using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.Parser.VB;
using ASTAttribute = ICSharpCode.NRefactory.Ast.Attribute;



using System;

namespace ICSharpCode.NRefactory.Parser.VB {


partial class Parser : AbstractParser
{
	public const int _EOF = 0;
	public const int _EOL = 1;
	public const int _ident = 2;
	public const int _LiteralString = 3;
	public const int _LiteralCharacter = 4;
	public const int _LiteralInteger = 5;
	public const int _LiteralDouble = 6;
	public const int _LiteralSingle = 7;
	public const int _LiteralDecimal = 8;
	public const int _LiteralDate = 9;
	public const int maxT = 222;  //<! max term (w/o pragmas)

	const  bool   T            = true;
	const  bool   x            = false;
	


	void Get () {

		lexer.NextToken();
	}

	void VBNET() {
		lexer.NextToken(); // get the first token
		compilationUnit = new CompilationUnit();

		while (la.kind == 1 || la.kind == 11) {
			EndOfStmt();
		}
		while (la.kind == 159) {
			OptionStmt();
			while (la.kind == 1 || la.kind == 11) {
				EndOfStmt();
			}
		}
		while (la.kind == 124) {
			ImportsStmt();
			while (la.kind == 1 || la.kind == 11) {
				EndOfStmt();
			}
		}
		while (IsGlobalAttrTarget()) {
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
			Get();
		} else if (la.kind == 11) {
			Get();
		} else SynErr(223);
	}

	void OptionStmt() {
		INode node = null; bool val = true;
		Expect(159);
		Location startPos = t.Location;
		if (la.kind == 108) {
			Get();
			if (la.kind == 156 || la.kind == 157) {
				OptionValue(ref val);
			}
			node = new OptionDeclaration(OptionType.Explicit, val);
		} else if (la.kind == 192) {
			Get();
			if (la.kind == 156 || la.kind == 157) {
				OptionValue(ref val);
			}
			node = new OptionDeclaration(OptionType.Strict, val);
		} else if (la.kind == 74) {
			Get();
			if (la.kind == 54) {
				Get();
				node = new OptionDeclaration(OptionType.CompareBinary, val);
			} else if (la.kind == 198) {
				Get();
				node = new OptionDeclaration(OptionType.CompareText, val);
			} else SynErr(224);
		} else if (la.kind == 126) {
			Get();
			if (la.kind == 156 || la.kind == 157) {
				OptionValue(ref val);
			}
			node = new OptionDeclaration(OptionType.Infer, val);
		} else SynErr(225);
		EndOfStmt();
		if (node != null) {
		node.StartLocation = startPos;
		node.EndLocation   = t.Location;
		compilationUnit.AddChild(node);
		}

	}

	void ImportsStmt() {
		List<Using> usings = new List<Using>();

		Expect(124);
		Location startPos = t.Location;
		Using u;

		ImportClause(out u);
		if (u != null) { usings.Add(u); }
		while (la.kind == 12) {
			Get();
			ImportClause(out u);
			if (u != null) { usings.Add(u); }
		}
		EndOfStmt();
		UsingDeclaration usingDeclaration = new UsingDeclaration(usings);
		usingDeclaration.StartLocation = startPos;
		usingDeclaration.EndLocation   = t.Location;
		compilationUnit.AddChild(usingDeclaration);

	}

	void GlobalAttributeSection() {
		Expect(28);
		Location startPos = t.Location;
		if (la.kind == 52) {
			Get();
		} else if (la.kind == 141) {
			Get();
		} else SynErr(226);
		string attributeTarget = t.val != null ? t.val.ToLower(System.Globalization.CultureInfo.InvariantCulture) : null;
		List<ASTAttribute> attributes = new List<ASTAttribute>();
		ASTAttribute attribute;

		Expect(11);
		Attribute(out attribute);
		attributes.Add(attribute);
		while (NotFinalComma()) {
			if (la.kind == 12) {
				Get();
				if (la.kind == 52) {
					Get();
				} else if (la.kind == 141) {
					Get();
				} else SynErr(227);
				Expect(11);
			}
			Attribute(out attribute);
			attributes.Add(attribute);
		}
		if (la.kind == 12) {
			Get();
		}
		Expect(27);
		EndOfStmt();
		AttributeSection section = new AttributeSection {
		AttributeTarget = attributeTarget,
		Attributes = attributes,
		StartLocation = startPos,
		EndLocation = t.EndLocation
		};
		compilationUnit.AddChild(section);

	}

	void NamespaceMemberDecl() {
		ModifierList m = new ModifierList();
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		string qualident;

		if (la.kind == 146) {
			Get();
			Location startPos = t.Location;

			Qualident(out qualident);
			INode node =  new NamespaceDeclaration(qualident);
			node.StartLocation = startPos;
			compilationUnit.AddChild(node);
			compilationUnit.BlockStart(node);

			EndOfStmt();
			NamespaceBody();
			node.EndLocation = t.Location;
			compilationUnit.BlockEnd();

		} else if (StartOf(2)) {
			while (la.kind == 28) {
				AttributeSection(out section);
				attributes.Add(section);
			}
			while (StartOf(3)) {
				TypeModifier(m);
			}
			NonModuleDeclaration(m, attributes);
		} else SynErr(228);
	}

	void OptionValue(ref bool val) {
		if (la.kind == 157) {
			Get();
			val = true;
		} else if (la.kind == 156) {
			Get();
			val = false;
		} else SynErr(229);
	}

	void ImportClause(out Using u) {
		string qualident  = null;
		TypeReference aliasedType = null;
		u = null;

		if (StartOf(4)) {
			Qualident(out qualident);
			if (la.kind == 10) {
				Get();
				TypeName(out aliasedType);
			}
			if (qualident != null && qualident.Length > 0) {
			if (aliasedType != null) {
				u = new Using(qualident, aliasedType);
			} else {
				u = new Using(qualident);
			}
			}

		} else if (la.kind == 28) {
			string prefix = null;
			Get();
			Identifier();
			prefix = t.val;
			Expect(10);
			Expect(3);
			u = new Using(t.literalValue as string, prefix);
			Expect(27);
		} else SynErr(230);
	}

	void Qualident(out string qualident) {
		string name;
		qualidentBuilder.Length = 0; 

		Identifier();
		qualidentBuilder.Append(t.val);
		while (DotAndIdentOrKw()) {
			Expect(16);
			IdentifierOrKeyword(out name);
			qualidentBuilder.Append('.'); qualidentBuilder.Append(name);
		}
		qualident = qualidentBuilder.ToString();
	}

	void TypeName(out TypeReference typeref) {
		ArrayList rank = null;
		NonArrayTypeName(out typeref, false);
		ArrayTypeModifiers(out rank);
		if (rank != null && typeref != null) {
		typeref.RankSpecifier = (int[])rank.ToArray(typeof(int));
		}

	}

	void Identifier() {
		if (StartOf(5)) {
			IdentifierForFieldDeclaration();
		} else if (la.kind == 85) {
			Get();
		} else SynErr(231);
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
		Expect(100);
		Expect(146);
		EndOfStmt();
	}

	void AttributeSection(out AttributeSection section) {
		string attributeTarget = "";List<ASTAttribute> attributes = new List<ASTAttribute>();
		ASTAttribute attribute;


		Expect(28);
		Location startPos = t.Location;
		if (IsLocalAttrTarget()) {
			if (la.kind == 106) {
				Get();
				attributeTarget = "event";
			} else if (la.kind == 180) {
				Get();
				attributeTarget = "return";
			} else if (StartOf(4)) {
				Identifier();
				string val = t.val.ToLower(System.Globalization.CultureInfo.InvariantCulture);
				if (val != "field"	|| val != "method" ||
					val != "module" || val != "param"  ||
					val != "property" || val != "type")
				Error("attribute target specifier (event, return, field," +
						"method, module, param, property, or type) expected");
				attributeTarget = t.val;

			} else SynErr(232);
			Expect(11);
		}
		Attribute(out attribute);
		attributes.Add(attribute);
		while (NotFinalComma()) {
			Expect(12);
			Attribute(out attribute);
			attributes.Add(attribute);
		}
		if (la.kind == 12) {
			Get();
		}
		Expect(27);
		section = new AttributeSection {
		AttributeTarget = attributeTarget,
		Attributes = attributes,
		StartLocation = startPos,
		EndLocation = t.EndLocation
		};

	}

	void TypeModifier(ModifierList m) {
		switch (la.kind) {
		case 173: {
			Get();
			m.Add(Modifiers.Public, t.Location);
			break;
		}
		case 172: {
			Get();
			m.Add(Modifiers.Protected, t.Location);
			break;
		}
		case 112: {
			Get();
			m.Add(Modifiers.Internal, t.Location);
			break;
		}
		case 170: {
			Get();
			m.Add(Modifiers.Private, t.Location);
			break;
		}
		case 185: {
			Get();
			m.Add(Modifiers.Static, t.Location);
			break;
		}
		case 184: {
			Get();
			m.Add(Modifiers.New, t.Location);
			break;
		}
		case 142: {
			Get();
			m.Add(Modifiers.Abstract, t.Location);
			break;
		}
		case 152: {
			Get();
			m.Add(Modifiers.Sealed, t.Location);
			break;
		}
		case 168: {
			Get();
			m.Add(Modifiers.Partial, t.Location);
			break;
		}
		default: SynErr(233); break;
		}
	}

	void NonModuleDeclaration(ModifierList m, List<AttributeSection> attributes) {
		TypeReference typeRef = null;
		List<TypeReference> baseInterfaces = null;

		switch (la.kind) {
		case 71: {
			m.Check(Modifiers.Classes);
			Get();
			TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
			newType.StartLocation = t.Location;
			compilationUnit.AddChild(newType);
			compilationUnit.BlockStart(newType);

			newType.Type       = ClassType.Class;

			Identifier();
			newType.Name = t.val;
			TypeParameterList(newType.Templates);
			EndOfStmt();
			newType.BodyStartLocation = t.Location;
			if (la.kind == 127) {
				ClassBaseType(out typeRef);
				SafeAdd(newType, newType.BaseTypes, typeRef);
			}
			while (la.kind == 123) {
				TypeImplementsClause(out baseInterfaces);
				newType.BaseTypes.AddRange(baseInterfaces);
			}
			ClassBody(newType);
			Expect(100);
			Expect(71);
			newType.EndLocation = t.EndLocation;
			EndOfStmt();
			compilationUnit.BlockEnd();

			break;
		}
		case 141: {
			Get();
			m.Check(Modifiers.VBModules);
			TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
			compilationUnit.AddChild(newType);
			compilationUnit.BlockStart(newType);
			newType.StartLocation = m.GetDeclarationLocation(t.Location);
			newType.Type = ClassType.Module;

			Identifier();
			newType.Name = t.val;
			EndOfStmt();
			newType.BodyStartLocation = t.Location;
			ModuleBody(newType);
			compilationUnit.BlockEnd();

			break;
		}
		case 194: {
			Get();
			m.Check(Modifiers.VBStructures);
			TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
			compilationUnit.AddChild(newType);
			compilationUnit.BlockStart(newType);
			newType.StartLocation = m.GetDeclarationLocation(t.Location);
			newType.Type = ClassType.Struct;

			Identifier();
			newType.Name = t.val;
			TypeParameterList(newType.Templates);
			EndOfStmt();
			newType.BodyStartLocation = t.Location;
			while (la.kind == 123) {
				TypeImplementsClause(out baseInterfaces);
				newType.BaseTypes.AddRange(baseInterfaces);
			}
			StructureBody(newType);
			compilationUnit.BlockEnd();

			break;
		}
		case 102: {
			Get();
			m.Check(Modifiers.VBEnums);
			TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
			newType.StartLocation = m.GetDeclarationLocation(t.Location);
			compilationUnit.AddChild(newType);
			compilationUnit.BlockStart(newType);

			newType.Type = ClassType.Enum;

			Identifier();
			newType.Name = t.val;
			if (la.kind == 50) {
				Get();
				NonArrayTypeName(out typeRef, false);
				SafeAdd(newType, newType.BaseTypes, typeRef);
			}
			EndOfStmt();
			newType.BodyStartLocation = t.Location;
			EnumBody(newType);
			compilationUnit.BlockEnd();

			break;
		}
		case 129: {
			Get();
			m.Check(Modifiers.VBInterfacs);
			TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
			newType.StartLocation = m.GetDeclarationLocation(t.Location);
			compilationUnit.AddChild(newType);
			compilationUnit.BlockStart(newType);
			newType.Type = ClassType.Interface;

			Identifier();
			newType.Name = t.val;
			TypeParameterList(newType.Templates);
			EndOfStmt();
			newType.BodyStartLocation = t.Location;
			while (la.kind == 127) {
				InterfaceBase(out baseInterfaces);
				newType.BaseTypes.AddRange(baseInterfaces);
			}
			InterfaceBody(newType);
			compilationUnit.BlockEnd();

			break;
		}
		case 90: {
			Get();
			m.Check(Modifiers.VBDelegates);
			DelegateDeclaration delegateDeclr = new DelegateDeclaration(m.Modifier, attributes);
			delegateDeclr.ReturnType = new TypeReference("System.Void", true);
			delegateDeclr.StartLocation = m.GetDeclarationLocation(t.Location);
			List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();

			if (la.kind == 195) {
				Get();
				Identifier();
				delegateDeclr.Name = t.val;
				TypeParameterList(delegateDeclr.Templates);
				if (la.kind == 25) {
					Get();
					if (StartOf(6)) {
						FormalParameterList(p);
					}
					Expect(26);
					delegateDeclr.Parameters = p;
				}
			} else if (la.kind == 114) {
				Get();
				Identifier();
				delegateDeclr.Name = t.val;
				TypeParameterList(delegateDeclr.Templates);
				if (la.kind == 25) {
					Get();
					if (StartOf(6)) {
						FormalParameterList(p);
					}
					Expect(26);
					delegateDeclr.Parameters = p;
				}
				if (la.kind == 50) {
					Get();
					TypeReference type;
					TypeName(out type);
					delegateDeclr.ReturnType = type;
				}
			} else SynErr(234);
			delegateDeclr.EndLocation = t.EndLocation;
			EndOfStmt();
			compilationUnit.AddChild(delegateDeclr);

			break;
		}
		default: SynErr(235); break;
		}
	}

	void TypeParameterList(List<TemplateDefinition> templates) {
		TemplateDefinition template;

		if (la.kind == Tokens.OpenParenthesis && Peek(1).kind == Tokens.Of) {
			Expect(25);
			Expect(155);
			TypeParameter(out template);
			if (template != null) templates.Add(template);

			while (la.kind == 12) {
				Get();
				TypeParameter(out template);
				if (template != null) templates.Add(template);

			}
			Expect(26);
		}
	}

	void TypeParameter(out TemplateDefinition template) {
		Identifier();
		template = new TemplateDefinition(t.val, null);
		if (la.kind == 50) {
			TypeParameterConstraints(template);
		}
	}

	void TypeParameterConstraints(TemplateDefinition template) {
		TypeReference constraint;

		Expect(50);
		if (la.kind == 23) {
			Get();
			TypeParameterConstraint(out constraint);
			if (constraint != null) { template.Bases.Add(constraint); }
			while (la.kind == 12) {
				Get();
				TypeParameterConstraint(out constraint);
				if (constraint != null) { template.Bases.Add(constraint); }
			}
			Expect(24);
		} else if (StartOf(7)) {
			TypeParameterConstraint(out constraint);
			if (constraint != null) { template.Bases.Add(constraint); }
		} else SynErr(236);
	}

	void TypeParameterConstraint(out TypeReference constraint) {
		constraint = null;
		if (la.kind == 71) {
			Get();
			constraint = TypeReference.ClassConstraint;
		} else if (la.kind == 194) {
			Get();
			constraint = TypeReference.StructConstraint;
		} else if (la.kind == 148) {
			Get();
			constraint = TypeReference.NewConstraint;
		} else if (StartOf(8)) {
			TypeName(out constraint);
		} else SynErr(237);
	}

	void ClassBaseType(out TypeReference typeRef) {
		typeRef = null;

		Expect(127);
		TypeName(out typeRef);
		EndOfStmt();
	}

	void TypeImplementsClause(out List<TypeReference> baseInterfaces) {
		baseInterfaces = new List<TypeReference>();
		TypeReference type = null;

		Expect(123);
		TypeName(out type);
		if (type != null) baseInterfaces.Add(type);

		while (la.kind == 12) {
			Get();
			TypeName(out type);
			if (type != null) baseInterfaces.Add(type);
		}
		EndOfStmt();
	}

	void ClassBody(TypeDeclaration newType) {
		AttributeSection section;
		while (la.kind == 1 || la.kind == 11) {
			EndOfStmt();
		}
		while (StartOf(9)) {
			List<AttributeSection> attributes = new List<AttributeSection>();
			ModifierList m = new ModifierList();

			while (la.kind == 28) {
				AttributeSection(out section);
				attributes.Add(section);
			}
			while (StartOf(10)) {
				MemberModifier(m);
			}
			ClassMemberDecl(m, attributes);
			while (la.kind == 1 || la.kind == 11) {
				EndOfStmt();
			}
		}
	}

	void ModuleBody(TypeDeclaration newType) {
		AttributeSection section;
		while (la.kind == 1 || la.kind == 11) {
			EndOfStmt();
		}
		while (StartOf(9)) {
			List<AttributeSection> attributes = new List<AttributeSection>();
			ModifierList m = new ModifierList();

			while (la.kind == 28) {
				AttributeSection(out section);
				attributes.Add(section);
			}
			while (StartOf(10)) {
				MemberModifier(m);
			}
			ClassMemberDecl(m, attributes);
			while (la.kind == 1 || la.kind == 11) {
				EndOfStmt();
			}
		}
		Expect(100);
		Expect(141);
		newType.EndLocation = t.EndLocation;
		EndOfStmt();
	}

	void StructureBody(TypeDeclaration newType) {
		AttributeSection section;
		while (la.kind == 1 || la.kind == 11) {
			EndOfStmt();
		}
		while (StartOf(9)) {
			List<AttributeSection> attributes = new List<AttributeSection>();
			ModifierList m = new ModifierList();

			while (la.kind == 28) {
				AttributeSection(out section);
				attributes.Add(section);
			}
			while (StartOf(10)) {
				MemberModifier(m);
			}
			StructureMemberDecl(m, attributes);
			while (la.kind == 1 || la.kind == 11) {
				EndOfStmt();
			}
		}
		Expect(100);
		Expect(194);
		newType.EndLocation = t.EndLocation;
		EndOfStmt();
	}

	void NonArrayTypeName(out TypeReference typeref, bool canBeUnbound) {
		string name;
		typeref = null;
		bool isGlobal = false;

		if (StartOf(11)) {
			if (la.kind == 117) {
				Get();
				Expect(16);
				isGlobal = true;
			}
			QualIdentAndTypeArguments(out typeref, canBeUnbound);
			typeref.IsGlobal = isGlobal;
			while (la.kind == 16) {
				Get();
				TypeReference nestedTypeRef;
				QualIdentAndTypeArguments(out nestedTypeRef, canBeUnbound);
				typeref = new InnerClassTypeReference(typeref, nestedTypeRef.Type, nestedTypeRef.GenericTypes);
			}
		} else if (la.kind == 154) {
			Get();
			typeref = new TypeReference("System.Object", true);
			if (la.kind == 21) {
				Get();
				List<TypeReference> typeArguments = new List<TypeReference>(1);
				if (typeref != null) typeArguments.Add(typeref);
				typeref = new TypeReference("System.Nullable", typeArguments) { IsKeyword = true };

			}
		} else if (StartOf(12)) {
			PrimitiveTypeName(out name);
			typeref = new TypeReference(name, true);
			if (la.kind == 21) {
				Get();
				List<TypeReference> typeArguments = new List<TypeReference>(1);
				if (typeref != null) typeArguments.Add(typeref);
				typeref = new TypeReference("System.Nullable", typeArguments) { IsKeyword = true };

			}
		} else SynErr(238);
	}

	void EnumBody(TypeDeclaration newType) {
		FieldDeclaration f;
		while (la.kind == 1 || la.kind == 11) {
			EndOfStmt();
		}
		while (StartOf(13)) {
			EnumMemberDecl(out f);
			compilationUnit.AddChild(f);

			while (la.kind == 1 || la.kind == 11) {
				EndOfStmt();
			}
		}
		Expect(100);
		Expect(102);
		newType.EndLocation = t.EndLocation;
		EndOfStmt();
	}

	void InterfaceBase(out List<TypeReference> bases) {
		TypeReference type;
		bases = new List<TypeReference>();

		Expect(127);
		TypeName(out type);
		if (type != null) bases.Add(type);
		while (la.kind == 12) {
			Get();
			TypeName(out type);
			if (type != null) bases.Add(type);
		}
		EndOfStmt();
	}

	void InterfaceBody(TypeDeclaration newType) {
		while (la.kind == 1 || la.kind == 11) {
			EndOfStmt();
		}
		while (StartOf(14)) {
			InterfaceMemberDecl();
			while (la.kind == 1 || la.kind == 11) {
				EndOfStmt();
			}
		}
		Expect(100);
		Expect(129);
		newType.EndLocation = t.EndLocation;
		EndOfStmt();
	}

	void FormalParameterList(List<ParameterDeclarationExpression> parameter) {
		ParameterDeclarationExpression p;
		FormalParameter(out p);
		if (p != null) parameter.Add(p);
		while (la.kind == 12) {
			Get();
			FormalParameter(out p);
			if (p != null) parameter.Add(p);
		}
	}

	void MemberModifier(ModifierList m) {
		switch (la.kind) {
		case 142: {
			Get();
			m.Add(Modifiers.Abstract, t.Location);
			break;
		}
		case 89: {
			Get();
			m.Add(Modifiers.Default, t.Location);
			break;
		}
		case 112: {
			Get();
			m.Add(Modifiers.Internal, t.Location);
			break;
		}
		case 184: {
			Get();
			m.Add(Modifiers.New, t.Location);
			break;
		}
		case 166: {
			Get();
			m.Add(Modifiers.Override, t.Location);
			break;
		}
		case 143: {
			Get();
			m.Add(Modifiers.Abstract, t.Location);
			break;
		}
		case 170: {
			Get();
			m.Add(Modifiers.Private, t.Location);
			break;
		}
		case 172: {
			Get();
			m.Add(Modifiers.Protected, t.Location);
			break;
		}
		case 173: {
			Get();
			m.Add(Modifiers.Public, t.Location);
			break;
		}
		case 152: {
			Get();
			m.Add(Modifiers.Sealed, t.Location);
			break;
		}
		case 153: {
			Get();
			m.Add(Modifiers.Sealed, t.Location);
			break;
		}
		case 185: {
			Get();
			m.Add(Modifiers.Static, t.Location);
			break;
		}
		case 165: {
			Get();
			m.Add(Modifiers.Virtual, t.Location);
			break;
		}
		case 164: {
			Get();
			m.Add(Modifiers.Overloads, t.Location);
			break;
		}
		case 175: {
			Get();
			m.Add(Modifiers.ReadOnly, t.Location);
			break;
		}
		case 220: {
			Get();
			m.Add(Modifiers.WriteOnly, t.Location);
			break;
		}
		case 219: {
			Get();
			m.Add(Modifiers.WithEvents, t.Location);
			break;
		}
		case 92: {
			Get();
			m.Add(Modifiers.Dim, t.Location);
			break;
		}
		case 168: {
			Get();
			m.Add(Modifiers.Partial, t.Location);
			break;
		}
		default: SynErr(239); break;
		}
	}

	void ClassMemberDecl(ModifierList m, List<AttributeSection> attributes) {
		StructureMemberDecl(m, attributes);
	}

	void StructureMemberDecl(ModifierList m, List<AttributeSection> attributes) {
		TypeReference type = null;
		List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
		Statement stmt = null;
		List<VariableDeclaration> variableDeclarators = new List<VariableDeclaration>();
		List<TemplateDefinition> templates = new List<TemplateDefinition>();

		switch (la.kind) {
		case 71: case 90: case 102: case 129: case 141: case 194: {
			NonModuleDeclaration(m, attributes);
			break;
		}
		case 195: {
			Get();
			Location startPos = t.Location;

			if (StartOf(4)) {
				string name = String.Empty;
				MethodDeclaration methodDeclaration; List<string> handlesClause = null;
				List<InterfaceImplementation> implementsClause = null;

				Identifier();
				name = t.val;
				m.Check(Modifiers.VBMethods);

				TypeParameterList(templates);
				if (la.kind == 25) {
					Get();
					if (StartOf(6)) {
						FormalParameterList(p);
					}
					Expect(26);
				}
				if (la.kind == 121 || la.kind == 123) {
					if (la.kind == 123) {
						ImplementsClause(out implementsClause);
					} else {
						HandlesClause(out handlesClause);
					}
				}
				Location endLocation = t.EndLocation;
				if (IsMustOverride(m)) {
					EndOfStmt();
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
					Get();
					methodDeclaration = new MethodDeclaration {
					Name = name, Modifier = m.Modifier, Parameters = p, Attributes = attributes,
					StartLocation = m.GetDeclarationLocation(startPos), EndLocation = endLocation,
					TypeReference = new TypeReference("System.Void", true),
					Templates = templates,
					HandlesClause = handlesClause,
					InterfaceImplementations = implementsClause
					};
					compilationUnit.AddChild(methodDeclaration);

					if (ParseMethodBodies) {
					Block(out stmt);
					Expect(100);
					Expect(195);
					} else {
					// don't parse method body
					lexer.SkipCurrentBlock(Tokens.Sub); stmt = new BlockStatement();
					  }

					methodDeclaration.Body  = (BlockStatement)stmt;
					methodDeclaration.Body.EndLocation = t.EndLocation;
					EndOfStmt();
				} else SynErr(240);
			} else if (la.kind == 148) {
				Get();
				if (la.kind == 25) {
					Get();
					if (StartOf(6)) {
						FormalParameterList(p);
					}
					Expect(26);
				}
				m.Check(Modifiers.Constructors);
				Location constructorEndLocation = t.EndLocation;
				Expect(1);
				if (ParseMethodBodies) {
				Block(out stmt);
				Expect(100);
				Expect(195);
				} else {
				// don't parse method body
				lexer.SkipCurrentBlock(Tokens.Sub); stmt = new BlockStatement();
				  }

				Location endLocation = t.EndLocation;
				EndOfStmt();
				ConstructorDeclaration cd = new ConstructorDeclaration("New", m.Modifier, p, attributes);
				cd.StartLocation = m.GetDeclarationLocation(startPos);
				cd.EndLocation   = constructorEndLocation;
				cd.Body = (BlockStatement)stmt;
				cd.Body.EndLocation   = endLocation;
				compilationUnit.AddChild(cd);

			} else SynErr(241);
			break;
		}
		case 114: {
			Get();
			m.Check(Modifiers.VBMethods);
			string name = String.Empty;
			Location startPos = t.Location;
			MethodDeclaration methodDeclaration;List<string> handlesClause = null;
			List<InterfaceImplementation> implementsClause = null;
			AttributeSection returnTypeAttributeSection = null;

			Identifier();
			name = t.val;
			TypeParameterList(templates);
			if (la.kind == 25) {
				Get();
				if (StartOf(6)) {
					FormalParameterList(p);
				}
				Expect(26);
			}
			if (la.kind == 50) {
				Get();
				while (la.kind == 28) {
					AttributeSection(out returnTypeAttributeSection);
					if (returnTypeAttributeSection != null) {
					returnTypeAttributeSection.AttributeTarget = "return";
					attributes.Add(returnTypeAttributeSection);
					}

				}
				TypeName(out type);
			}
			if(type == null) {
			type = new TypeReference("System.Object", true);
			}

			if (la.kind == 121 || la.kind == 123) {
				if (la.kind == 123) {
					ImplementsClause(out implementsClause);
				} else {
					HandlesClause(out handlesClause);
				}
			}
			Location endLocation = t.EndLocation;
			if (IsMustOverride(m)) {
				EndOfStmt();
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
				Get();
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
				Block(out stmt);
				Expect(100);
				Expect(114);
				} else {
				// don't parse method body
				lexer.SkipCurrentBlock(Tokens.Function); stmt = new BlockStatement();
				}
				methodDeclaration.Body = (BlockStatement)stmt;
				methodDeclaration.Body.StartLocation = methodDeclaration.EndLocation;
				methodDeclaration.Body.EndLocation   = t.EndLocation;

				EndOfStmt();
			} else SynErr(242);
			break;
		}
		case 88: {
			Get();
			m.Check(Modifiers.VBExternalMethods);
			Location startPos = t.Location;
			CharsetModifier charsetModifer = CharsetModifier.None;
			string library = String.Empty;
			string alias = null;
			string name = String.Empty;

			if (StartOf(15)) {
				Charset(out charsetModifer);
			}
			if (la.kind == 195) {
				Get();
				Identifier();
				name = t.val;
				Expect(135);
				Expect(3);
				library = t.literalValue as string;
				if (la.kind == 46) {
					Get();
					Expect(3);
					alias = t.literalValue as string;
				}
				if (la.kind == 25) {
					Get();
					if (StartOf(6)) {
						FormalParameterList(p);
					}
					Expect(26);
				}
				EndOfStmt();
				DeclareDeclaration declareDeclaration = new DeclareDeclaration(name, m.Modifier, null, p, attributes, library, alias, charsetModifer);
				declareDeclaration.StartLocation = m.GetDeclarationLocation(startPos);
				declareDeclaration.EndLocation   = t.EndLocation;
				compilationUnit.AddChild(declareDeclaration);

			} else if (la.kind == 114) {
				Get();
				Identifier();
				name = t.val;
				Expect(135);
				Expect(3);
				library = t.literalValue as string;
				if (la.kind == 46) {
					Get();
					Expect(3);
					alias = t.literalValue as string;
				}
				if (la.kind == 25) {
					Get();
					if (StartOf(6)) {
						FormalParameterList(p);
					}
					Expect(26);
				}
				if (la.kind == 50) {
					Get();
					TypeName(out type);
				}
				EndOfStmt();
				DeclareDeclaration declareDeclaration = new DeclareDeclaration(name, m.Modifier, type, p, attributes, library, alias, charsetModifer);
				declareDeclaration.StartLocation = m.GetDeclarationLocation(startPos);
				declareDeclaration.EndLocation   = t.EndLocation;
				compilationUnit.AddChild(declareDeclaration);

			} else SynErr(243);
			break;
		}
		case 106: {
			Get();
			m.Check(Modifiers.VBEvents);
			Location startPos = t.Location;
			EventDeclaration eventDeclaration;
			string name = String.Empty;
			List<InterfaceImplementation> implementsClause = null;

			Identifier();
			name= t.val;
			if (la.kind == 50) {
				Get();
				TypeName(out type);
			} else if (StartOf(16)) {
				if (la.kind == 25) {
					Get();
					if (StartOf(6)) {
						FormalParameterList(p);
					}
					Expect(26);
				}
			} else SynErr(244);
			if (la.kind == 123) {
				ImplementsClause(out implementsClause);
			}
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
		case 2: case 45: case 49: case 51: case 52: case 53: case 54: case 57: case 74: case 91: case 94: case 103: case 108: case 113: case 120: case 126: case 130: case 133: case 156: case 162: case 169: case 188: case 197: case 198: case 208: case 209: case 215: {
			m.Check(Modifiers.Fields);
			FieldDeclaration fd = new FieldDeclaration(attributes, null, m.Modifier);

			IdentifierForFieldDeclaration();
			string name = t.val;
			fd.StartLocation = m.GetDeclarationLocation(t.Location);
			VariableDeclaratorPartAfterIdentifier(variableDeclarators, name);
			while (la.kind == 12) {
				Get();
				VariableDeclarator(variableDeclarators);
			}
			EndOfStmt();
			fd.EndLocation = t.EndLocation;
			fd.Fields = variableDeclarators;
			compilationUnit.AddChild(fd);

			break;
		}
		case 75: {
			m.Check(Modifiers.Fields);
			Get();
			m.Add(Modifiers.Const, t.Location); 
			FieldDeclaration fd = new FieldDeclaration(attributes, type, m.Modifier);
			fd.StartLocation = m.GetDeclarationLocation(t.Location);
			List<VariableDeclaration> constantDeclarators = new List<VariableDeclaration>();

			ConstantDeclarator(constantDeclarators);
			while (la.kind == 12) {
				Get();
				ConstantDeclarator(constantDeclarators);
			}
			fd.Fields = constantDeclarators;
			fd.EndLocation = t.Location;

			EndOfStmt();
			fd.EndLocation = t.EndLocation;
			compilationUnit.AddChild(fd);

			break;
		}
		case 171: {
			Get();
			m.Check(Modifiers.VBProperties);
			Location startPos = t.Location;
			List<InterfaceImplementation> implementsClause = null;
			AttributeSection returnTypeAttributeSection = null;
			Expression initializer = null;

			Identifier();
			string propertyName = t.val;
			if (la.kind == 25) {
				Get();
				if (StartOf(6)) {
					FormalParameterList(p);
				}
				Expect(26);
			}
			if (la.kind == 50) {
				Get();
				while (la.kind == 28) {
					AttributeSection(out returnTypeAttributeSection);
					if (returnTypeAttributeSection != null) {
					returnTypeAttributeSection.AttributeTarget = "return";
					attributes.Add(returnTypeAttributeSection);
					}

				}
				if (IsNewExpression()) {
					ObjectCreateExpression(out initializer);
					if (initializer is ObjectCreateExpression) {
					type = ((ObjectCreateExpression)initializer).CreateType.Clone();
					} else {
						type = ((ArrayCreateExpression)initializer).CreateType.Clone();
					}

				} else if (StartOf(8)) {
					TypeName(out type);
				} else SynErr(245);
			}
			if (la.kind == 10) {
				Get();
				Expr(out initializer);
			}
			if (la.kind == 123) {
				ImplementsClause(out implementsClause);
			}
			EndOfStmt();
			if (IsMustOverride(m) || IsAutomaticProperty()) {
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
				PropertyDeclaration pDecl = new PropertyDeclaration(propertyName, type, m.Modifier, attributes);
				pDecl.StartLocation = m.GetDeclarationLocation(startPos);
				pDecl.EndLocation   = t.Location;
				pDecl.BodyStart   = t.Location;
				pDecl.TypeReference = type;
				pDecl.InterfaceImplementations = implementsClause;
				pDecl.Parameters = p;
				PropertyGetRegion getRegion;
				PropertySetRegion setRegion;

				AccessorDecls(out getRegion, out setRegion);
				Expect(100);
				Expect(171);
				EndOfStmt();
				pDecl.GetRegion = getRegion;
				pDecl.SetRegion = setRegion;
				pDecl.BodyEnd = t.Location; // t = EndOfStmt; not "Property"
				compilationUnit.AddChild(pDecl);

			} else SynErr(246);
			break;
		}
		case 85: {
			Get();
			Location startPos = t.Location;
			Expect(106);
			m.Check(Modifiers.VBCustomEvents);
			EventAddRemoveRegion eventAccessorDeclaration;
			EventAddRegion addHandlerAccessorDeclaration = null;
			EventRemoveRegion removeHandlerAccessorDeclaration = null;
			EventRaiseRegion raiseEventAccessorDeclaration = null;
			List<InterfaceImplementation> implementsClause = null;

			Identifier();
			string customEventName = t.val;
			Expect(50);
			TypeName(out type);
			if (la.kind == 123) {
				ImplementsClause(out implementsClause);
			}
			EndOfStmt();
			while (StartOf(18)) {
				EventAccessorDeclaration(out eventAccessorDeclaration);
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
			Expect(100);
			Expect(106);
			EndOfStmt();
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
		case 147: case 158: case 217: {
			ConversionType opConversionType = ConversionType.None;
			if (la.kind == 147 || la.kind == 217) {
				if (la.kind == 217) {
					Get();
					opConversionType = ConversionType.Implicit;
				} else {
					Get();
					opConversionType = ConversionType.Explicit;
				}
			}
			Expect(158);
			m.Check(Modifiers.VBOperators);
			Location startPos = t.Location;
			TypeReference returnType = NullTypeReference.Instance;
			TypeReference operandType = NullTypeReference.Instance;
			string operandName;
			OverloadableOperatorType operatorType;
			AttributeSection section;
			List<ParameterDeclarationExpression> parameters = new List<ParameterDeclarationExpression>();

			OverloadableOperator(out operatorType);
			Expect(25);
			if (la.kind == 59) {
				Get();
			}
			Identifier();
			operandName = t.val;
			if (la.kind == 50) {
				Get();
				TypeName(out operandType);
			}
			parameters.Add(new ParameterDeclarationExpression(operandType, operandName, ParameterModifiers.In));
			while (la.kind == 12) {
				Get();
				if (la.kind == 59) {
					Get();
				}
				Identifier();
				operandName = t.val;
				if (la.kind == 50) {
					Get();
					TypeName(out operandType);
				}
				parameters.Add(new ParameterDeclarationExpression(operandType, operandName, ParameterModifiers.In));
			}
			Expect(26);
			Location endPos = t.EndLocation;
			if (la.kind == 50) {
				Get();
				while (la.kind == 28) {
					AttributeSection(out section);
					if (section != null) {
					section.AttributeTarget = "return";
					attributes.Add(section);
					}
				}
				TypeName(out returnType);
				endPos = t.EndLocation;
			}
			Expect(1);
			Block(out stmt);
			Expect(100);
			Expect(158);
			EndOfStmt();
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
		default: SynErr(247); break;
		}
	}

	void EnumMemberDecl(out FieldDeclaration f) {
		Expression expr = null;List<AttributeSection> attributes = new List<AttributeSection>();
		AttributeSection section = null;
		VariableDeclaration varDecl = null;

		while (la.kind == 28) {
			AttributeSection(out section);
			attributes.Add(section);
		}
		Identifier();
		f = new FieldDeclaration(attributes);
		varDecl = new VariableDeclaration(t.val);
		f.Fields.Add(varDecl);
		f.StartLocation = varDecl.StartLocation = t.Location;

		if (la.kind == 10) {
			Get();
			Expr(out expr);
			varDecl.Initializer = expr;
		}
		EndOfStmt();
	}

	void InterfaceMemberDecl() {
		TypeReference type =null;
		List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
		List<TemplateDefinition> templates = new List<TemplateDefinition>();
		AttributeSection section, returnTypeAttributeSection = null;
		ModifierList mod = new ModifierList();
		List<AttributeSection> attributes = new List<AttributeSection>();
		string name;

		if (StartOf(19)) {
			while (la.kind == 28) {
				AttributeSection(out section);
				attributes.Add(section);
			}
			while (StartOf(10)) {
				MemberModifier(mod);
			}
			if (la.kind == 106) {
				Get();
				mod.Check(Modifiers.VBInterfaceEvents);
				Location startLocation = t.Location;

				Identifier();
				name = t.val;
				if (la.kind == 25) {
					Get();
					if (StartOf(6)) {
						FormalParameterList(p);
					}
					Expect(26);
				}
				if (la.kind == 50) {
					Get();
					TypeName(out type);
				}
				EndOfStmt();
				EventDeclaration ed = new EventDeclaration {
				Name = name, TypeReference = type, Modifier = mod.Modifier,
				Parameters = p, Attributes = attributes,
				StartLocation = startLocation, EndLocation = t.EndLocation
				};
				compilationUnit.AddChild(ed);

			} else if (la.kind == 195) {
				Get();
				Location startLocation =  t.Location;
				mod.Check(Modifiers.VBInterfaceMethods);

				Identifier();
				name = t.val;
				TypeParameterList(templates);
				if (la.kind == 25) {
					Get();
					if (StartOf(6)) {
						FormalParameterList(p);
					}
					Expect(26);
				}
				EndOfStmt();
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

			} else if (la.kind == 114) {
				Get();
				mod.Check(Modifiers.VBInterfaceMethods);
				Location startLocation = t.Location;

				Identifier();
				name = t.val;
				TypeParameterList(templates);
				if (la.kind == 25) {
					Get();
					if (StartOf(6)) {
						FormalParameterList(p);
					}
					Expect(26);
				}
				if (la.kind == 50) {
					Get();
					while (la.kind == 28) {
						AttributeSection(out returnTypeAttributeSection);
					}
					TypeName(out type);
				}
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
			} else if (la.kind == 171) {
				Get();
				Location startLocation = t.Location;
				mod.Check(Modifiers.VBInterfaceProperties);

				Identifier();
				name = t.val; 
				if (la.kind == 25) {
					Get();
					if (StartOf(6)) {
						FormalParameterList(p);
					}
					Expect(26);
				}
				if (la.kind == 50) {
					Get();
					TypeName(out type);
				}
				if(type == null) {
				type = new TypeReference("System.Object", true);
				}

				EndOfStmt();
				PropertyDeclaration pd = new PropertyDeclaration(name, type, mod.Modifier, attributes);
				pd.Parameters = p;
				pd.EndLocation = t.EndLocation;
				pd.StartLocation = startLocation;
				compilationUnit.AddChild(pd);

			} else SynErr(248);
		} else if (StartOf(20)) {
			NonModuleDeclaration(mod, attributes);
		} else SynErr(249);
	}

	void Expr(out Expression expr) {
		expr = null;
		if (IsQueryExpression()) {
			QueryExpr(out expr);
		} else if (la.kind == 114) {
			LambdaExpr(out expr);
		} else if (StartOf(21)) {
			DisjunctionExpr(out expr);
		} else SynErr(250);
	}

	void ImplementsClause(out List<InterfaceImplementation> baseInterfaces) {
		baseInterfaces = new List<InterfaceImplementation>();
		TypeReference type = null;
		string memberName = null;

		Expect(123);
		NonArrayTypeName(out type, false);
		if (type != null) memberName = TypeReference.StripLastIdentifierFromType(ref type);
		baseInterfaces.Add(new InterfaceImplementation(type, memberName));
		while (la.kind == 12) {
			Get();
			NonArrayTypeName(out type, false);
			if (type != null) memberName = TypeReference.StripLastIdentifierFromType(ref type);
			baseInterfaces.Add(new InterfaceImplementation(type, memberName));
		}
	}

	void HandlesClause(out List<string> handlesClause) {
		handlesClause = new List<string>();
		string name;

		Expect(121);
		EventMemberSpecifier(out name);
		if (name != null) handlesClause.Add(name);
		while (la.kind == 12) {
			Get();
			EventMemberSpecifier(out name);
			if (name != null) handlesClause.Add(name);
		}
	}

	void Block(out  Statement stmt) {
		BlockStatement blockStmt = new BlockStatement();
		/* in snippet parsing mode, t might be null */
		if (t != null) blockStmt.StartLocation = t.EndLocation;
		compilationUnit.BlockStart(blockStmt);

		while (StartOf(22)) {
			if (IsEndStmtAhead()) {
				Expect(100);
				EndOfStmt();
				compilationUnit.AddChild(new EndStatement());
			} else {
				Statement();
				EndOfStmt();
			}
		}
		stmt = blockStmt;
		if (t != null) blockStmt.EndLocation = t.EndLocation;
		compilationUnit.BlockEnd();

	}

	void Charset(out CharsetModifier charsetModifier) {
		charsetModifier = CharsetModifier.None;
		if (la.kind == 114 || la.kind == 195) {
		} else if (la.kind == 49) {
			Get();
			charsetModifier = CharsetModifier.Ansi;
		} else if (la.kind == 53) {
			Get();
			charsetModifier = CharsetModifier.Auto;
		} else if (la.kind == 208) {
			Get();
			charsetModifier = CharsetModifier.Unicode;
		} else SynErr(251);
	}

	void IdentifierForFieldDeclaration() {
		switch (la.kind) {
		case 2: {
			Get();
			break;
		}
		case 45: {
			Get();
			break;
		}
		case 49: {
			Get();
			break;
		}
		case 51: {
			Get();
			break;
		}
		case 52: {
			Get();
			break;
		}
		case 53: {
			Get();
			break;
		}
		case 54: {
			Get();
			break;
		}
		case 57: {
			Get();
			break;
		}
		case 74: {
			Get();
			break;
		}
		case 91: {
			Get();
			break;
		}
		case 94: {
			Get();
			break;
		}
		case 103: {
			Get();
			break;
		}
		case 108: {
			Get();
			break;
		}
		case 113: {
			Get();
			break;
		}
		case 120: {
			Get();
			break;
		}
		case 126: {
			Get();
			break;
		}
		case 130: {
			Get();
			break;
		}
		case 133: {
			Get();
			break;
		}
		case 156: {
			Get();
			break;
		}
		case 162: {
			Get();
			break;
		}
		case 169: {
			Get();
			break;
		}
		case 188: {
			Get();
			break;
		}
		case 197: {
			Get();
			break;
		}
		case 198: {
			Get();
			break;
		}
		case 208: {
			Get();
			break;
		}
		case 209: {
			Get();
			break;
		}
		case 215: {
			Get();
			break;
		}
		default: SynErr(252); break;
		}
	}

	void VariableDeclaratorPartAfterIdentifier(List<VariableDeclaration> fieldDeclaration, string name) {
		Expression expr = null;
		TypeReference type = null;
		ArrayList rank = null;
		List<Expression> dimension = null;
		Location startLocation = t.Location;

		if (IsSize() && !IsDims()) {
			ArrayInitializationModifier(out dimension);
		}
		if (IsDims()) {
			ArrayNameModifier(out rank);
		}
		if (IsObjectCreation()) {
			Expect(50);
			ObjectCreateExpression(out expr);
			if (expr is ObjectCreateExpression) {
			type = ((ObjectCreateExpression)expr).CreateType.Clone();
			} else {
				type = ((ArrayCreateExpression)expr).CreateType.Clone();
			}

		} else if (StartOf(23)) {
			if (la.kind == 50) {
				Get();
				TypeName(out type);
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

			if (la.kind == 10) {
				Get();
				Expr(out expr);
			}
		} else SynErr(253);
		VariableDeclaration varDecl = new VariableDeclaration(name, expr, type);
		varDecl.StartLocation = startLocation;
		varDecl.EndLocation = t.Location;
		fieldDeclaration.Add(varDecl);

	}

	void VariableDeclarator(List<VariableDeclaration> fieldDeclaration) {
		Identifier();
		string name = t.val;
		VariableDeclaratorPartAfterIdentifier(fieldDeclaration, name);
	}

	void ConstantDeclarator(List<VariableDeclaration> constantDeclaration) {
		Expression expr = null;
		TypeReference type = null;
		string name = String.Empty;
		Location location;

		Identifier();
		name = t.val; location = t.Location;
		if (la.kind == 50) {
			Get();
			TypeName(out type);
		}
		Expect(10);
		Expr(out expr);
		VariableDeclaration f = new VariableDeclaration(name, expr);
		f.TypeReference = type;
		f.StartLocation = location;
		constantDeclaration.Add(f);

	}

	void ObjectCreateExpression(out Expression oce) {
		TypeReference type = null;
		CollectionInitializerExpression initializer = null;
		List<Expression> arguments = null;
		ArrayList dimensions = null;
		oce = null;
		bool canBeNormal; bool canBeReDim;

		Expect(148);
		if (StartOf(8)) {
			NonArrayTypeName(out type, false);
			if (la.kind == 25) {
				Get();
				NormalOrReDimArgumentList(out arguments, out canBeNormal, out canBeReDim);
				Expect(26);
				if (la.kind == 23 || la.kind == 25) {
					if (la.kind == Tokens.OpenParenthesis) {
						ArrayTypeModifiers(out dimensions);
						CollectionInitializer(out initializer);
					} else {
						CollectionInitializer(out initializer);
					}
				}
				if (canBeReDim && !canBeNormal && initializer == null) initializer = new CollectionInitializerExpression();
			}
		}
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

		if (la.kind == 113 || la.kind == 218) {
			if (la.kind == 218) {
				MemberInitializerExpression memberInitializer = null;

				Get();
				CollectionInitializerExpression memberInitializers = new CollectionInitializerExpression();
				memberInitializers.StartLocation = la.Location;

				Expect(23);
				MemberInitializer(out memberInitializer);
				memberInitializers.CreateExpressions.Add(memberInitializer);
				while (la.kind == 12) {
					Get();
					MemberInitializer(out memberInitializer);
					memberInitializers.CreateExpressions.Add(memberInitializer);
				}
				Expect(24);
				memberInitializers.EndLocation = t.Location;
				if(oce is ObjectCreateExpression)
				{
					((ObjectCreateExpression)oce).ObjectInitializer = memberInitializers;
				}

			} else {
				Get();
				CollectionInitializer(out initializer);
				if(oce is ObjectCreateExpression)
				((ObjectCreateExpression)oce).ObjectInitializer = initializer;

			}
		}
	}

	void AccessorDecls(out PropertyGetRegion getBlock, out PropertySetRegion setBlock) {
		List<AttributeSection> attributes = new List<AttributeSection>();
		AttributeSection section;
		getBlock = null;
		setBlock = null; 

		while (la.kind == 28) {
			AttributeSection(out section);
			attributes.Add(section);
		}
		if (StartOf(24)) {
			GetAccessorDecl(out getBlock, attributes);
			if (StartOf(25)) {
				attributes = new List<AttributeSection>();
				while (la.kind == 28) {
					AttributeSection(out section);
					attributes.Add(section);
				}
				SetAccessorDecl(out setBlock, attributes);
			}
		} else if (StartOf(26)) {
			SetAccessorDecl(out setBlock, attributes);
			if (StartOf(27)) {
				attributes = new List<AttributeSection>();
				while (la.kind == 28) {
					AttributeSection(out section);
					attributes.Add(section);
				}
				GetAccessorDecl(out getBlock, attributes);
			}
		} else SynErr(254);
	}

	void EventAccessorDeclaration(out EventAddRemoveRegion eventAccessorDeclaration) {
		Statement stmt = null;
		List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		eventAccessorDeclaration = null;

		while (la.kind == 28) {
			AttributeSection(out section);
			attributes.Add(section);
		}
		if (la.kind == 43) {
			Get();
			if (la.kind == 25) {
				Get();
				if (StartOf(6)) {
					FormalParameterList(p);
				}
				Expect(26);
			}
			Expect(1);
			Block(out stmt);
			Expect(100);
			Expect(43);
			EndOfStmt();
			eventAccessorDeclaration = new EventAddRegion(attributes);
			eventAccessorDeclaration.Block = (BlockStatement)stmt;
			eventAccessorDeclaration.Parameters = p;

		} else if (la.kind == 178) {
			Get();
			if (la.kind == 25) {
				Get();
				if (StartOf(6)) {
					FormalParameterList(p);
				}
				Expect(26);
			}
			Expect(1);
			Block(out stmt);
			Expect(100);
			Expect(178);
			EndOfStmt();
			eventAccessorDeclaration = new EventRemoveRegion(attributes);
			eventAccessorDeclaration.Block = (BlockStatement)stmt;
			eventAccessorDeclaration.Parameters = p;

		} else if (la.kind == 174) {
			Get();
			if (la.kind == 25) {
				Get();
				if (StartOf(6)) {
					FormalParameterList(p);
				}
				Expect(26);
			}
			Expect(1);
			Block(out stmt);
			Expect(100);
			Expect(174);
			EndOfStmt();
			eventAccessorDeclaration = new EventRaiseRegion(attributes);
			eventAccessorDeclaration.Block = (BlockStatement)stmt;
			eventAccessorDeclaration.Parameters = p;

		} else SynErr(255);
	}

	void OverloadableOperator(out OverloadableOperatorType operatorType) {
		operatorType = OverloadableOperatorType.None;
		switch (la.kind) {
		case 19: {
			Get();
			operatorType = OverloadableOperatorType.Add;
			break;
		}
		case 18: {
			Get();
			operatorType = OverloadableOperatorType.Subtract;
			break;
		}
		case 22: {
			Get();
			operatorType = OverloadableOperatorType.Multiply;
			break;
		}
		case 14: {
			Get();
			operatorType = OverloadableOperatorType.Divide;
			break;
		}
		case 15: {
			Get();
			operatorType = OverloadableOperatorType.DivideInteger;
			break;
		}
		case 13: {
			Get();
			operatorType = OverloadableOperatorType.Concat;
			break;
		}
		case 136: {
			Get();
			operatorType = OverloadableOperatorType.Like;
			break;
		}
		case 140: {
			Get();
			operatorType = OverloadableOperatorType.Modulus;
			break;
		}
		case 47: {
			Get();
			operatorType = OverloadableOperatorType.BitwiseAnd;
			break;
		}
		case 161: {
			Get();
			operatorType = OverloadableOperatorType.BitwiseOr;
			break;
		}
		case 221: {
			Get();
			operatorType = OverloadableOperatorType.ExclusiveOr;
			break;
		}
		case 20: {
			Get();
			operatorType = OverloadableOperatorType.Power;
			break;
		}
		case 32: {
			Get();
			operatorType = OverloadableOperatorType.ShiftLeft;
			break;
		}
		case 33: {
			Get();
			operatorType = OverloadableOperatorType.ShiftRight;
			break;
		}
		case 10: {
			Get();
			operatorType = OverloadableOperatorType.Equality;
			break;
		}
		case 29: {
			Get();
			operatorType = OverloadableOperatorType.InEquality;
			break;
		}
		case 28: {
			Get();
			operatorType = OverloadableOperatorType.LessThan;
			break;
		}
		case 31: {
			Get();
			operatorType = OverloadableOperatorType.LessThanOrEqual;
			break;
		}
		case 27: {
			Get();
			operatorType = OverloadableOperatorType.GreaterThan;
			break;
		}
		case 30: {
			Get();
			operatorType = OverloadableOperatorType.GreaterThanOrEqual;
			break;
		}
		case 81: {
			Get();
			operatorType = OverloadableOperatorType.CType;
			break;
		}
		case 2: case 45: case 49: case 51: case 52: case 53: case 54: case 57: case 74: case 85: case 91: case 94: case 103: case 108: case 113: case 120: case 126: case 130: case 133: case 156: case 162: case 169: case 188: case 197: case 198: case 208: case 209: case 215: {
			Identifier();
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
		default: SynErr(256); break;
		}
	}

	void GetAccessorDecl(out PropertyGetRegion getBlock, List<AttributeSection> attributes) {
		Statement stmt = null; Modifiers m;
		PropertyAccessorAccessModifier(out m);
		Expect(115);
		Location startLocation = t.Location;
		Expect(1);
		Block(out stmt);
		getBlock = new PropertyGetRegion((BlockStatement)stmt, attributes);
		Expect(100);
		Expect(115);
		getBlock.Modifier = m;
		getBlock.StartLocation = startLocation; getBlock.EndLocation = t.EndLocation;
		EndOfStmt();
	}

	void SetAccessorDecl(out PropertySetRegion setBlock, List<AttributeSection> attributes) {
		Statement stmt = null;
		List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
		Modifiers m;

		PropertyAccessorAccessModifier(out m);
		Expect(183);
		Location startLocation = t.Location;
		if (la.kind == 25) {
			Get();
			if (StartOf(6)) {
				FormalParameterList(p);
			}
			Expect(26);
		}
		Expect(1);
		Block(out stmt);
		setBlock = new PropertySetRegion((BlockStatement)stmt, attributes);
		setBlock.Modifier = m;
		setBlock.Parameters = p;

		Expect(100);
		Expect(183);
		setBlock.StartLocation = startLocation; setBlock.EndLocation = t.EndLocation;
		EndOfStmt();
	}

	void PropertyAccessorAccessModifier(out Modifiers m) {
		m = Modifiers.None;
		while (StartOf(28)) {
			if (la.kind == 173) {
				Get();
				m |= Modifiers.Public;
			} else if (la.kind == 172) {
				Get();
				m |= Modifiers.Protected;
			} else if (la.kind == 112) {
				Get();
				m |= Modifiers.Internal;
			} else {
				Get();
				m |= Modifiers.Private;
			}
		}
	}

	void ArrayInitializationModifier(out List<Expression> arrayModifiers) {
		arrayModifiers = null;

		Expect(25);
		InitializationRankList(out arrayModifiers);
		Expect(26);
	}

	void ArrayNameModifier(out ArrayList arrayModifiers) {
		arrayModifiers = null;

		ArrayTypeModifiers(out arrayModifiers);
	}

	void InitializationRankList(out List<Expression> rank) {
		rank = new List<Expression>();
		Expression expr = null;

		Expr(out expr);
		if (la.kind == 201) {
			Get();
			EnsureIsZero(expr);
			Expr(out expr);
		}
		if (expr != null) { rank.Add(expr); }
		while (la.kind == 12) {
			Get();
			Expr(out expr);
			if (la.kind == 201) {
				Get();
				EnsureIsZero(expr);
				Expr(out expr);
			}
			if (expr != null) { rank.Add(expr); }
		}
	}

	void CollectionInitializer(out CollectionInitializerExpression outExpr) {
		Expression expr = null;
		CollectionInitializerExpression initializer = new CollectionInitializerExpression();

		Expect(23);
		if (StartOf(29)) {
			Expr(out expr);
			if (expr != null) { initializer.CreateExpressions.Add(expr); }

			while (NotFinalComma()) {
				Expect(12);
				Expr(out expr);
				if (expr != null) { initializer.CreateExpressions.Add(expr); }
			}
		}
		Expect(24);
		outExpr = initializer;
	}

	void EventMemberSpecifier(out string name) {
		string eventName;
		if (StartOf(4)) {
			Identifier();
		} else if (la.kind == 144) {
			Get();
		} else if (la.kind == 139) {
			Get();
		} else SynErr(257);
		name = t.val;
		Expect(16);
		IdentifierOrKeyword(out eventName);
		name = name + "." + eventName;
	}

	void IdentifierOrKeyword(out string name) {
		Get();
		name = t.val; 
	}

	void QueryExpr(out Expression expr) {
		QueryExpressionVB qexpr = new QueryExpressionVB();
		qexpr.StartLocation = la.Location;
		expr = qexpr;

		FromOrAggregateQueryOperator(qexpr.Clauses);
		while (StartOf(30)) {
			QueryOperator(qexpr.Clauses);
		}
		qexpr.EndLocation = t.EndLocation;

	}

	void LambdaExpr(out Expression expr) {
		Expression inner = null;
		LambdaExpression lambda = new LambdaExpression();
		lambda.StartLocation = la.Location;

		Expect(114);
		if (la.kind == 25) {
			Get();
			if (StartOf(6)) {
				FormalParameterList(lambda.Parameters);
			}
			Expect(26);
		}
		Expr(out inner);
		lambda.ExpressionBody = inner;
		lambda.EndLocation = t.EndLocation; // la.Location?

		expr = lambda;

	}

	void DisjunctionExpr(out Expression outExpr) {
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;

		ConjunctionExpr(out outExpr);
		while (la.kind == 161 || la.kind == 163 || la.kind == 221) {
			if (la.kind == 161) {
				Get();
				op = BinaryOperatorType.BitwiseOr;
			} else if (la.kind == 163) {
				Get();
				op = BinaryOperatorType.LogicalOr;
			} else {
				Get();
				op = BinaryOperatorType.ExclusiveOr;
			}
			ConjunctionExpr(out expr);
			outExpr = new BinaryOperatorExpression(outExpr, op, expr); 
		}
	}

	void AssignmentOperator(out AssignmentOperatorType op) {
		op = AssignmentOperatorType.None;
		switch (la.kind) {
		case 10: {
			Get();
			op = AssignmentOperatorType.Assign;
			break;
		}
		case 42: {
			Get();
			op = AssignmentOperatorType.ConcatString;
			break;
		}
		case 34: {
			Get();
			op = AssignmentOperatorType.Add;
			break;
		}
		case 36: {
			Get();
			op = AssignmentOperatorType.Subtract;
			break;
		}
		case 37: {
			Get();
			op = AssignmentOperatorType.Multiply;
			break;
		}
		case 38: {
			Get();
			op = AssignmentOperatorType.Divide;
			break;
		}
		case 39: {
			Get();
			op = AssignmentOperatorType.DivideInteger;
			break;
		}
		case 35: {
			Get();
			op = AssignmentOperatorType.Power;
			break;
		}
		case 40: {
			Get();
			op = AssignmentOperatorType.ShiftLeft;
			break;
		}
		case 41: {
			Get();
			op = AssignmentOperatorType.ShiftRight;
			break;
		}
		default: SynErr(258); break;
		}
	}

	void SimpleExpr(out Expression pexpr) {
		string name;
		SimpleNonInvocationExpression(out pexpr);
		while (la.kind == 16 || la.kind == 17 || la.kind == 25) {
			if (la.kind == 16) {
				Get();
				IdentifierOrKeyword(out name);
				pexpr = new MemberReferenceExpression(pexpr, name);
				if (la.kind == Tokens.OpenParenthesis && Peek(1).kind == Tokens.Of) {
					Expect(25);
					Expect(155);
					TypeArgumentList(((MemberReferenceExpression)pexpr).TypeArguments);
					Expect(26);
				}
			} else if (la.kind == 17) {
				Get();
				IdentifierOrKeyword(out name);
				pexpr = new BinaryOperatorExpression(pexpr, BinaryOperatorType.DictionaryAccess, new PrimitiveExpression(name, name));
			} else {
				InvocationExpression(ref pexpr);
			}
		}
	}

	void SimpleNonInvocationExpression(out Expression pexpr) {
		Expression expr;
		CollectionInitializerExpression cie;
		TypeReference type = null;
		string name = String.Empty;
		pexpr = null;

		if (StartOf(31)) {
			switch (la.kind) {
			case 3: {
				Get();
				pexpr = new PrimitiveExpression(t.literalValue, t.val) { LiteralFormat = t.literalFormat }; 
				break;
			}
			case 4: {
				Get();
				pexpr = new PrimitiveExpression(t.literalValue, t.val) { LiteralFormat = t.literalFormat }; 
				break;
			}
			case 7: {
				Get();
				pexpr = new PrimitiveExpression(t.literalValue, t.val) { LiteralFormat = t.literalFormat }; 
				break;
			}
			case 6: {
				Get();
				pexpr = new PrimitiveExpression(t.literalValue, t.val) { LiteralFormat = t.literalFormat }; 
				break;
			}
			case 5: {
				Get();
				pexpr = new PrimitiveExpression(t.literalValue, t.val) { LiteralFormat = t.literalFormat }; 
				break;
			}
			case 9: {
				Get();
				pexpr = new PrimitiveExpression(t.literalValue, t.val) { LiteralFormat = t.literalFormat }; 
				break;
			}
			case 8: {
				Get();
				pexpr = new PrimitiveExpression(t.literalValue, t.val) { LiteralFormat = t.literalFormat }; 
				break;
			}
			case 202: {
				Get();
				pexpr = new PrimitiveExpression(true, "true"); 
				break;
			}
			case 109: {
				Get();
				pexpr = new PrimitiveExpression(false, "false");
				break;
			}
			case 151: {
				Get();
				pexpr = new PrimitiveExpression(null, "null"); 
				break;
			}
			case 25: {
				Get();
				Expr(out expr);
				Expect(26);
				pexpr = new ParenthesizedExpression(expr);
				break;
			}
			case 2: case 45: case 49: case 51: case 52: case 53: case 54: case 57: case 74: case 85: case 91: case 94: case 103: case 108: case 113: case 120: case 126: case 130: case 133: case 156: case 162: case 169: case 188: case 197: case 198: case 208: case 209: case 215: {
				Identifier();
				pexpr = new IdentifierExpression(t.val);
				pexpr.StartLocation = t.Location; pexpr.EndLocation = t.EndLocation;

				if (la.kind == Tokens.OpenParenthesis && Peek(1).kind == Tokens.Of) {
					Expect(25);
					Expect(155);
					TypeArgumentList(((IdentifierExpression)pexpr).TypeArguments);
					Expect(26);
				}
				break;
			}
			case 55: case 58: case 69: case 86: case 87: case 96: case 128: case 137: case 154: case 181: case 186: case 187: case 193: case 206: case 207: case 210: {
				string val = String.Empty;
				if (StartOf(12)) {
					PrimitiveTypeName(out val);
				} else {
					Get();
					val = "System.Object";
				}
				pexpr = new TypeReferenceExpression(new TypeReference(val, true));
				break;
			}
			case 139: {
				Get();
				pexpr = new ThisReferenceExpression();
				break;
			}
			case 144: case 145: {
				Expression retExpr = null;
				if (la.kind == 144) {
					Get();
					retExpr = new BaseReferenceExpression();
				} else {
					Get();
					retExpr = new ClassReferenceExpression();
				}
				Expect(16);
				IdentifierOrKeyword(out name);
				pexpr = new MemberReferenceExpression(retExpr, name);
				break;
			}
			case 117: {
				Get();
				Expect(16);
				Identifier();
				type = new TypeReference(t.val ?? "");
				type.IsGlobal = true;
				pexpr = new TypeReferenceExpression(type);
				break;
			}
			case 148: {
				ObjectCreateExpression(out expr);
				pexpr = expr;
				break;
			}
			case 23: {
				CollectionInitializer(out cie);
				pexpr = cie;
				break;
			}
			case 81: case 93: case 204: {
				CastType castType = CastType.Cast;
				if (la.kind == 93) {
					Get();
				} else if (la.kind == 81) {
					Get();
					castType = CastType.Conversion;
				} else {
					Get();
					castType = CastType.TryCast;
				}
				Expect(25);
				Expr(out expr);
				Expect(12);
				TypeName(out type);
				Expect(26);
				pexpr = new CastExpression(type, expr, castType);
				break;
			}
			case 63: case 64: case 65: case 66: case 67: case 68: case 70: case 72: case 73: case 77: case 78: case 79: case 80: case 82: case 83: case 84: {
				CastTarget(out type);
				Expect(25);
				Expr(out expr);
				Expect(26);
				pexpr = new CastExpression(type, expr, CastType.PrimitiveConversion);
				break;
			}
			case 44: {
				Get();
				Expr(out expr);
				pexpr = new AddressOfExpression(expr);
				break;
			}
			case 116: {
				Get();
				Expect(25);
				GetTypeTypeName(out type);
				Expect(26);
				pexpr = new TypeOfExpression(type);
				break;
			}
			case 205: {
				Get();
				SimpleExpr(out expr);
				Expect(131);
				TypeName(out type);
				pexpr = new TypeOfIsExpression(expr, type);
				break;
			}
			case 122: {
				ConditionalExpression(out pexpr);
				break;
			}
			}
		} else if (la.kind == 16) {
			Get();
			IdentifierOrKeyword(out name);
			pexpr = new MemberReferenceExpression(null, name);
		} else SynErr(259);
	}

	void TypeArgumentList(List<TypeReference> typeArguments) {
		TypeReference typeref;

		TypeName(out typeref);
		if (typeref != null) typeArguments.Add(typeref);
		while (la.kind == 12) {
			Get();
			TypeName(out typeref);
			if (typeref != null) typeArguments.Add(typeref);
		}
	}

	void InvocationExpression(ref Expression pexpr) {
		List<Expression> parameters = null;
		Expect(25);
		Location start = t.Location;
		ArgumentList(out parameters);
		Expect(26);
		pexpr = new InvocationExpression(pexpr, parameters);

		pexpr.StartLocation = start; pexpr.EndLocation = t.Location;
	}

	void PrimitiveTypeName(out string type) {
		type = String.Empty;
		switch (la.kind) {
		case 55: {
			Get();
			type = "System.Boolean";
			break;
		}
		case 86: {
			Get();
			type = "System.DateTime";
			break;
		}
		case 69: {
			Get();
			type = "System.Char";
			break;
		}
		case 193: {
			Get();
			type = "System.String";
			break;
		}
		case 87: {
			Get();
			type = "System.Decimal";
			break;
		}
		case 58: {
			Get();
			type = "System.Byte";
			break;
		}
		case 186: {
			Get();
			type = "System.Int16";
			break;
		}
		case 128: {
			Get();
			type = "System.Int32";
			break;
		}
		case 137: {
			Get();
			type = "System.Int64";
			break;
		}
		case 187: {
			Get();
			type = "System.Single";
			break;
		}
		case 96: {
			Get();
			type = "System.Double";
			break;
		}
		case 206: {
			Get();
			type = "System.UInt32";
			break;
		}
		case 207: {
			Get();
			type = "System.UInt64";
			break;
		}
		case 210: {
			Get();
			type = "System.UInt16";
			break;
		}
		case 181: {
			Get();
			type = "System.SByte";
			break;
		}
		default: SynErr(260); break;
		}
	}

	void CastTarget(out TypeReference type) {
		type = null;

		switch (la.kind) {
		case 63: {
			Get();
			type = new TypeReference("System.Boolean", true);
			break;
		}
		case 64: {
			Get();
			type = new TypeReference("System.Byte", true);
			break;
		}
		case 77: {
			Get();
			type = new TypeReference("System.SByte", true);
			break;
		}
		case 65: {
			Get();
			type = new TypeReference("System.Char", true);
			break;
		}
		case 66: {
			Get();
			type = new TypeReference("System.DateTime", true);
			break;
		}
		case 68: {
			Get();
			type = new TypeReference("System.Decimal", true);
			break;
		}
		case 67: {
			Get();
			type = new TypeReference("System.Double", true);
			break;
		}
		case 78: {
			Get();
			type = new TypeReference("System.Int16", true);
			break;
		}
		case 70: {
			Get();
			type = new TypeReference("System.Int32", true);
			break;
		}
		case 72: {
			Get();
			type = new TypeReference("System.Int64", true);
			break;
		}
		case 84: {
			Get();
			type = new TypeReference("System.UInt16", true);
			break;
		}
		case 82: {
			Get();
			type = new TypeReference("System.UInt32", true);
			break;
		}
		case 83: {
			Get();
			type = new TypeReference("System.UInt64", true);
			break;
		}
		case 73: {
			Get();
			type = new TypeReference("System.Object", true);
			break;
		}
		case 79: {
			Get();
			type = new TypeReference("System.Single", true);
			break;
		}
		case 80: {
			Get();
			type = new TypeReference("System.String", true);
			break;
		}
		default: SynErr(261); break;
		}
	}

	void GetTypeTypeName(out TypeReference typeref) {
		ArrayList rank = null;
		NonArrayTypeName(out typeref, true);
		ArrayTypeModifiers(out rank);
		if (rank != null && typeref != null) {
		typeref.RankSpecifier = (int[])rank.ToArray(typeof(int));
		}

	}

	void ConditionalExpression(out Expression expr) {
		ConditionalExpression conditionalExpression = new ConditionalExpression();
		BinaryOperatorExpression binaryOperatorExpression = new BinaryOperatorExpression();
		conditionalExpression.StartLocation = binaryOperatorExpression.StartLocation = la.Location;

		Expression condition = null;
		Expression trueExpr = null;
		Expression falseExpr = null;

		Expect(122);
		Expect(25);
		Expr(out condition);
		Expect(12);
		Expr(out trueExpr);
		if (la.kind == 12) {
			Get();
			Expr(out falseExpr);
		}
		Expect(26);
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

	void ArgumentList(out List<Expression> arguments) {
		arguments = new List<Expression>();
		Expression expr = null;

		if (StartOf(29)) {
			Argument(out expr);
		}
		while (la.kind == 12) {
			Get();
			arguments.Add(expr ?? Expression.Null); expr = null;
			if (StartOf(29)) {
				Argument(out expr);
			}
			if (expr == null) expr = Expression.Null;
		}
		if (expr != null) arguments.Add(expr);
	}

	void ConjunctionExpr(out Expression outExpr) {
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;

		NotExpr(out outExpr);
		while (la.kind == 47 || la.kind == 48) {
			if (la.kind == 47) {
				Get();
				op = BinaryOperatorType.BitwiseAnd;
			} else {
				Get();
				op = BinaryOperatorType.LogicalAnd;
			}
			NotExpr(out expr);
			outExpr = new BinaryOperatorExpression(outExpr, op, expr); 
		}
	}

	void NotExpr(out Expression outExpr) {
		UnaryOperatorType uop = UnaryOperatorType.None;
		while (la.kind == 150) {
			Get();
			uop = UnaryOperatorType.Not;
		}
		ComparisonExpr(out outExpr);
		if (uop != UnaryOperatorType.None)
		outExpr = new UnaryOperatorExpression(outExpr, uop);

	}

	void ComparisonExpr(out Expression outExpr) {
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;

		ShiftExpr(out outExpr);
		while (StartOf(32)) {
			switch (la.kind) {
			case 28: {
				Get();
				op = BinaryOperatorType.LessThan;
				break;
			}
			case 27: {
				Get();
				op = BinaryOperatorType.GreaterThan;
				break;
			}
			case 31: {
				Get();
				op = BinaryOperatorType.LessThanOrEqual;
				break;
			}
			case 30: {
				Get();
				op = BinaryOperatorType.GreaterThanOrEqual;
				break;
			}
			case 29: {
				Get();
				op = BinaryOperatorType.InEquality;
				break;
			}
			case 10: {
				Get();
				op = BinaryOperatorType.Equality;
				break;
			}
			case 136: {
				Get();
				op = BinaryOperatorType.Like;
				break;
			}
			case 131: {
				Get();
				op = BinaryOperatorType.ReferenceEquality;
				break;
			}
			case 132: {
				Get();
				op = BinaryOperatorType.ReferenceInequality;
				break;
			}
			}
			if (StartOf(33)) {
				ShiftExpr(out expr);
				outExpr = new BinaryOperatorExpression(outExpr, op, expr); 
			} else if (la.kind == 150) {
				Get();
				ShiftExpr(out expr);
				outExpr = new BinaryOperatorExpression(outExpr, op, new UnaryOperatorExpression(expr, UnaryOperatorType.Not)); 
			} else SynErr(262);
		}
	}

	void ShiftExpr(out Expression outExpr) {
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;

		ConcatenationExpr(out outExpr);
		while (la.kind == 32 || la.kind == 33) {
			if (la.kind == 32) {
				Get();
				op = BinaryOperatorType.ShiftLeft;
			} else {
				Get();
				op = BinaryOperatorType.ShiftRight;
			}
			ConcatenationExpr(out expr);
			outExpr = new BinaryOperatorExpression(outExpr, op, expr); 
		}
	}

	void ConcatenationExpr(out Expression outExpr) {
		Expression expr;
		AdditiveExpr(out outExpr);
		while (la.kind == 13) {
			Get();
			AdditiveExpr(out expr);
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.Concat, expr); 
		}
	}

	void AdditiveExpr(out Expression outExpr) {
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;

		ModuloExpr(out outExpr);
		while (la.kind == 18 || la.kind == 19) {
			if (la.kind == 19) {
				Get();
				op = BinaryOperatorType.Add;
			} else {
				Get();
				op = BinaryOperatorType.Subtract;
			}
			ModuloExpr(out expr);
			outExpr = new BinaryOperatorExpression(outExpr, op, expr); 
		}
	}

	void ModuloExpr(out Expression outExpr) {
		Expression expr;
		IntegerDivisionExpr(out outExpr);
		while (la.kind == 140) {
			Get();
			IntegerDivisionExpr(out expr);
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.Modulus, expr); 
		}
	}

	void IntegerDivisionExpr(out Expression outExpr) {
		Expression expr;
		MultiplicativeExpr(out outExpr);
		while (la.kind == 15) {
			Get();
			MultiplicativeExpr(out expr);
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.DivideInteger, expr); 
		}
	}

	void MultiplicativeExpr(out Expression outExpr) {
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;

		UnaryExpr(out outExpr);
		while (la.kind == 14 || la.kind == 22) {
			if (la.kind == 22) {
				Get();
				op = BinaryOperatorType.Multiply;
			} else {
				Get();
				op = BinaryOperatorType.Divide;
			}
			UnaryExpr(out expr);
			outExpr = new BinaryOperatorExpression(outExpr, op, expr);
		}
	}

	void UnaryExpr(out Expression uExpr) {
		Expression expr;
		UnaryOperatorType uop = UnaryOperatorType.None;
		bool isUOp = false;

		while (la.kind == 18 || la.kind == 19 || la.kind == 22) {
			if (la.kind == 19) {
				Get();
				uop = UnaryOperatorType.Plus; isUOp = true;
			} else if (la.kind == 18) {
				Get();
				uop = UnaryOperatorType.Minus; isUOp = true;
			} else {
				Get();
				uop = UnaryOperatorType.Dereference;  isUOp = true;
			}
		}
		ExponentiationExpr(out expr);
		if (isUOp) {
		uExpr = new UnaryOperatorExpression(expr, uop);
		} else {
			uExpr = expr;
		}

	}

	void ExponentiationExpr(out Expression outExpr) {
		Expression expr;
		SimpleExpr(out outExpr);
		while (la.kind == 20) {
			Get();
			SimpleExpr(out expr);
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.Power, expr); 
		}
	}

	void NormalOrReDimArgumentList(out List<Expression> arguments, out bool canBeNormal, out bool canBeRedim) {
		arguments = new List<Expression>();
		canBeNormal = true; canBeRedim = !IsNamedAssign();
		Expression expr = null;

		if (StartOf(29)) {
			Argument(out expr);
			if (la.kind == 201) {
				Get();
				EnsureIsZero(expr); canBeNormal = false;
				Expr(out expr);
			}
		}
		while (la.kind == 12) {
			Get();
			if (expr == null) canBeRedim = false;
			arguments.Add(expr ?? Expression.Null); expr = null;
			canBeRedim &= !IsNamedAssign();
			if (StartOf(29)) {
				Argument(out expr);
				if (la.kind == 201) {
					Get();
					EnsureIsZero(expr); canBeNormal = false;
					Expr(out expr);
				}
			}
			if (expr == null) { canBeRedim = false; expr = Expression.Null; }
		}
		if (expr != null) arguments.Add(expr); else canBeRedim = false;
	}

	void ArrayTypeModifiers(out ArrayList arrayModifiers) {
		arrayModifiers = new ArrayList();
		int i = 0;

		while (IsDims()) {
			Expect(25);
			if (la.kind == 12 || la.kind == 26) {
				RankList(out i);
			}
			arrayModifiers.Add(i);

			Expect(26);
		}
		if(arrayModifiers.Count == 0) {
		 arrayModifiers = null;
		}

	}

	void MemberInitializer(out MemberInitializerExpression memberInitializer) {
		memberInitializer = new MemberInitializerExpression();
		memberInitializer.StartLocation = la.Location;
		Expression initExpr = null;
		bool isKey = false;
		string name = null;

		Expect(16);
		IdentifierOrKeyword(out name);
		Expect(10);
		Expr(out initExpr);
		memberInitializer.Name = name;
		memberInitializer.Expression = initExpr;
		memberInitializer.IsKey = isKey;
		memberInitializer.EndLocation = t.EndLocation;

	}

	void FromOrAggregateQueryOperator(List<QueryExpressionClause> middleClauses) {
		QueryExpressionFromClause fromClause = null;
		QueryExpressionAggregateClause aggregateClause = null;

		if (la.kind == 113) {
			FromQueryOperator(out fromClause);
			middleClauses.Add(fromClause);
		} else if (la.kind == 45) {
			AggregateQueryOperator(out aggregateClause);
			middleClauses.Add(aggregateClause);
		} else SynErr(263);
	}

	void QueryOperator(List<QueryExpressionClause> middleClauses) {
		QueryExpressionJoinVBClause joinClause = null;
		QueryExpressionGroupVBClause groupByClause = null;
		QueryExpressionPartitionVBClause partitionClause = null;
		QueryExpressionGroupJoinVBClause groupJoinClause = null;
		QueryExpressionFromClause fromClause = null;
		QueryExpressionAggregateClause aggregateClause = null;

		if (la.kind == 113) {
			FromQueryOperator(out fromClause);
			middleClauses.Add(fromClause);
		} else if (la.kind == 45) {
			AggregateQueryOperator(out aggregateClause);
			middleClauses.Add(aggregateClause);
		} else if (la.kind == 182) {
			SelectQueryOperator(middleClauses);
		} else if (la.kind == 94) {
			DistinctQueryOperator(middleClauses);
		} else if (la.kind == 215) {
			WhereQueryOperator(middleClauses);
		} else if (la.kind == 162) {
			OrderByQueryOperator(middleClauses);
		} else if (la.kind == 188 || la.kind == 197) {
			PartitionQueryOperator(out partitionClause);
			middleClauses.Add(partitionClause);
		} else if (la.kind == 134) {
			LetQueryOperator(middleClauses);
		} else if (la.kind == 133) {
			JoinQueryOperator(out joinClause);
			middleClauses.Add(joinClause);
		} else if (la.kind == Tokens.Group && Peek(1).kind == Tokens.Join) {
			GroupJoinQueryOperator(out groupJoinClause);
			middleClauses.Add(groupJoinClause);
		} else if (la.kind == 120) {
			GroupByQueryOperator(out groupByClause);
			middleClauses.Add(groupByClause);
		} else SynErr(264);
	}

	void FromQueryOperator(out QueryExpressionFromClause fromClause) {
		fromClause = new QueryExpressionFromClause();
		fromClause.StartLocation = la.Location;

		Expect(113);
		CollectionRangeVariableDeclarationList(fromClause.Sources);
		fromClause.EndLocation = t.EndLocation;

	}

	void AggregateQueryOperator(out QueryExpressionAggregateClause aggregateClause) {
		aggregateClause = new QueryExpressionAggregateClause();
		aggregateClause.IntoVariables = new List<ExpressionRangeVariable>();
		aggregateClause.StartLocation = la.Location;
		CollectionRangeVariable source;

		Expect(45);
		CollectionRangeVariableDeclaration(out source);
		aggregateClause.Source = source;

		while (StartOf(30)) {
			QueryOperator(aggregateClause.MiddleClauses);
		}
		Expect(130);
		ExpressionRangeVariableDeclarationList(aggregateClause.IntoVariables);
		aggregateClause.EndLocation = t.EndLocation;

	}

	void SelectQueryOperator(List<QueryExpressionClause> middleClauses) {
		QueryExpressionSelectVBClause selectClause = new QueryExpressionSelectVBClause();
		selectClause.StartLocation = la.Location;

		Expect(182);
		ExpressionRangeVariableDeclarationList(selectClause.Variables);
		selectClause.EndLocation = t.Location;
		middleClauses.Add(selectClause);

	}

	void DistinctQueryOperator(List<QueryExpressionClause> middleClauses) {
		QueryExpressionDistinctClause distinctClause = new QueryExpressionDistinctClause();
		distinctClause.StartLocation = la.Location;

		Expect(94);
		distinctClause.EndLocation = t.EndLocation;
		middleClauses.Add(distinctClause);

	}

	void WhereQueryOperator(List<QueryExpressionClause> middleClauses) {
		QueryExpressionWhereClause whereClause = new QueryExpressionWhereClause();
		whereClause.StartLocation = la.Location;
		Expression operand = null;

		Expect(215);
		Expr(out operand);
		whereClause.Condition = operand;
		whereClause.EndLocation = t.EndLocation;

		middleClauses.Add(whereClause);

	}

	void OrderByQueryOperator(List<QueryExpressionClause> middleClauses) {
		QueryExpressionOrderClause orderClause = new QueryExpressionOrderClause();
		orderClause.StartLocation = la.Location;
		List<QueryExpressionOrdering> orderings = null;

		Expect(162);
		Expect(57);
		OrderExpressionList(out orderings);
		orderClause.Orderings = orderings;
		orderClause.EndLocation = t.EndLocation;
		middleClauses.Add(orderClause);

	}

	void PartitionQueryOperator(out QueryExpressionPartitionVBClause partitionClause) {
		partitionClause = new QueryExpressionPartitionVBClause();
		partitionClause.StartLocation = la.Location;
		Expression expr = null;

		if (la.kind == 197) {
			Get();
			partitionClause.PartitionType = QueryExpressionPartitionType.Take;
			if (la.kind == 216) {
				Get();
				partitionClause.PartitionType = QueryExpressionPartitionType.TakeWhile;
			}
		} else if (la.kind == 188) {
			Get();
			partitionClause.PartitionType = QueryExpressionPartitionType.Skip;
			if (la.kind == 216) {
				Get();
				partitionClause.PartitionType = QueryExpressionPartitionType.SkipWhile;
			}
		} else SynErr(265);
		Expr(out expr);
		partitionClause.Expression = expr;
		partitionClause.EndLocation = t.EndLocation;

	}

	void LetQueryOperator(List<QueryExpressionClause> middleClauses) {
		QueryExpressionLetVBClause letClause = new QueryExpressionLetVBClause();
		letClause.StartLocation = la.Location;

		Expect(134);
		ExpressionRangeVariableDeclarationList(letClause.Variables);
		letClause.EndLocation = t.EndLocation;
		middleClauses.Add(letClause);

	}

	void JoinQueryOperator(out QueryExpressionJoinVBClause joinClause) {
		joinClause = new QueryExpressionJoinVBClause();
		joinClause.StartLocation = la.Location;
		CollectionRangeVariable joinVariable = null;
		QueryExpressionJoinVBClause subJoin = null;
		QueryExpressionJoinConditionVB condition = null;


		Expect(133);
		CollectionRangeVariableDeclaration(out joinVariable);
		joinClause.JoinVariable = joinVariable;
		if (la.kind == 133) {
			JoinQueryOperator(out subJoin);
			joinClause.SubJoin = subJoin;
		}
		Expect(157);
		JoinCondition(out condition);
		SafeAdd(joinClause, joinClause.Conditions, condition);
		while (la.kind == 47) {
			Get();
			JoinCondition(out condition);
			SafeAdd(joinClause, joinClause.Conditions, condition);
		}
		joinClause.EndLocation = t.EndLocation;

	}

	void GroupJoinQueryOperator(out QueryExpressionGroupJoinVBClause groupJoinClause) {
		groupJoinClause = new QueryExpressionGroupJoinVBClause();
		groupJoinClause.StartLocation = la.Location;
		QueryExpressionJoinVBClause joinClause = null;

		Expect(120);
		JoinQueryOperator(out joinClause);
		Expect(130);
		ExpressionRangeVariableDeclarationList(groupJoinClause.IntoVariables);
		groupJoinClause.JoinClause = joinClause;
		groupJoinClause.EndLocation = t.EndLocation;

	}

	void GroupByQueryOperator(out QueryExpressionGroupVBClause groupByClause) {
		groupByClause = new QueryExpressionGroupVBClause();
		groupByClause.StartLocation = la.Location;

		Expect(120);
		ExpressionRangeVariableDeclarationList(groupByClause.GroupVariables);
		Expect(57);
		ExpressionRangeVariableDeclarationList(groupByClause.ByVariables);
		Expect(130);
		ExpressionRangeVariableDeclarationList(groupByClause.IntoVariables);
		groupByClause.EndLocation = t.EndLocation;

	}

	void OrderExpressionList(out List<QueryExpressionOrdering> orderings) {
		orderings = new List<QueryExpressionOrdering>();
		QueryExpressionOrdering ordering = null;

		OrderExpression(out ordering);
		orderings.Add(ordering);
		while (la.kind == 12) {
			Get();
			OrderExpression(out ordering);
			orderings.Add(ordering);
		}
	}

	void OrderExpression(out QueryExpressionOrdering ordering) {
		ordering = new QueryExpressionOrdering();
		ordering.StartLocation = la.Location;
		ordering.Direction = QueryExpressionOrderingDirection.None;
		Expression orderExpr = null;

		Expr(out orderExpr);
		ordering.Criteria = orderExpr;

		if (la.kind == 51 || la.kind == 91) {
			if (la.kind == 51) {
				Get();
				ordering.Direction = QueryExpressionOrderingDirection.Ascending;
			} else {
				Get();
				ordering.Direction = QueryExpressionOrderingDirection.Descending;
			}
		}
		ordering.EndLocation = t.EndLocation;
	}

	void ExpressionRangeVariableDeclarationList(List<ExpressionRangeVariable> variables) {
		ExpressionRangeVariable variable = null;

		ExpressionRangeVariableDeclaration(out variable);
		variables.Add(variable);
		while (la.kind == 12) {
			Get();
			ExpressionRangeVariableDeclaration(out variable);
			variables.Add(variable);
		}
	}

	void CollectionRangeVariableDeclarationList(List<CollectionRangeVariable> rangeVariables) {
		CollectionRangeVariable variableDeclaration;
		CollectionRangeVariableDeclaration(out variableDeclaration);
		rangeVariables.Add(variableDeclaration);
		while (la.kind == 12) {
			Get();
			CollectionRangeVariableDeclaration(out variableDeclaration);
			rangeVariables.Add(variableDeclaration);
		}
	}

	void CollectionRangeVariableDeclaration(out CollectionRangeVariable rangeVariable) {
		rangeVariable = new CollectionRangeVariable();
		rangeVariable.StartLocation = la.Location;
		TypeReference typeName = null;
		Expression inExpr = null;

		Identifier();
		rangeVariable.Identifier = t.val;
		if (la.kind == 50) {
			Get();
			TypeName(out typeName);
			rangeVariable.Type = typeName;
		}
		Expect(125);
		Expr(out inExpr);
		rangeVariable.Expression = inExpr;
		rangeVariable.EndLocation = t.EndLocation;

	}

	void ExpressionRangeVariableDeclaration(out ExpressionRangeVariable variable) {
		variable = new ExpressionRangeVariable();
		variable.StartLocation = la.Location;
		Expression rhs = null;
		TypeReference typeName = null;

		if (IsIdentifiedExpressionRange()) {
			Identifier();
			variable.Identifier = t.val;
			if (la.kind == 50) {
				Get();
				TypeName(out typeName);
				variable.Type = typeName;
			}
			Expect(10);
		}
		Expr(out rhs);
		variable.Expression = rhs;
		variable.EndLocation = t.EndLocation;

	}

	void JoinCondition(out QueryExpressionJoinConditionVB condition) {
		condition = new QueryExpressionJoinConditionVB();
		condition.StartLocation = la.Location;

		Expression lhs = null;
		Expression rhs = null;

		Expr(out lhs);
		Expect(103);
		Expr(out rhs);
		condition.LeftSide = lhs;
		condition.RightSide = rhs;
		condition.EndLocation = t.EndLocation;

	}

	void Argument(out Expression argumentexpr) {
		Expression expr;
		argumentexpr = null;
		string name;

		if (IsNamedAssign()) {
			Identifier();
			name = t.val; 
			Expect(11);
			Expect(10);
			Expr(out expr);
			argumentexpr = new NamedArgumentExpression(name, expr);

		} else if (StartOf(29)) {
			Expr(out argumentexpr);
		} else SynErr(266);
	}

	void QualIdentAndTypeArguments(out TypeReference typeref, bool canBeUnbound) {
		string name; typeref = null;
		Qualident(out name);
		typeref = new TypeReference(name);
		if (la.kind == Tokens.OpenParenthesis && Peek(1).kind == Tokens.Of) {
			Expect(25);
			Expect(155);
			if (canBeUnbound && (la.kind == Tokens.CloseParenthesis || la.kind == Tokens.Comma)) {
				typeref.GenericTypes.Add(NullTypeReference.Instance);
				while (la.kind == 12) {
					Get();
					typeref.GenericTypes.Add(NullTypeReference.Instance);
				}
			} else if (StartOf(8)) {
				TypeArgumentList(typeref.GenericTypes);
			} else SynErr(267);
			Expect(26);
		}
	}

	void RankList(out int i) {
		i = 0;
		while (la.kind == 12) {
			Get();
			++i;
		}
	}

	void Attribute(out ASTAttribute attribute) {
		string name;
		List<Expression> positional = new List<Expression>();
		List<NamedArgumentExpression> named = new List<NamedArgumentExpression>();

		if (la.kind == 117) {
			Get();
			Expect(16);
		}
		Qualident(out name);
		if (la.kind == 25) {
			AttributeArguments(positional, named);
		}
		attribute  = new ASTAttribute(name, positional, named);

	}

	void AttributeArguments(List<Expression> positional, List<NamedArgumentExpression> named) {
		bool nameFound = false;
		string name = "";
		Expression expr;

		Expect(25);
		if (IsNotClosingParenthesis()) {
			if (IsNamedAssign()) {
				nameFound = true;
				IdentifierOrKeyword(out name);
				if (la.kind == 11) {
					Get();
				}
				Expect(10);
			}
			Expr(out expr);
			if (expr != null) {
			if (string.IsNullOrEmpty(name)) { positional.Add(expr); }
			else { named.Add(new NamedArgumentExpression(name, expr)); name = ""; }
			}

			while (la.kind == 12) {
				Get();
				if (IsNamedAssign()) {
					nameFound = true;
					IdentifierOrKeyword(out name);
					if (la.kind == 11) {
						Get();
					}
					Expect(10);
				} else if (StartOf(29)) {
					if (nameFound) Error("no positional argument after named argument");
				} else SynErr(268);
				Expr(out expr);
				if (expr != null) { if(name == "") positional.Add(expr);
				else { named.Add(new NamedArgumentExpression(name, expr)); name = ""; }
				}

			}
		}
		Expect(26);
	}

	void FormalParameter(out ParameterDeclarationExpression p) {
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		TypeReference type = null;
		ParamModifierList mod = new ParamModifierList(this);
		Expression expr = null;
		p = null;
		ArrayList arrayModifiers = null;

		while (la.kind == 28) {
			AttributeSection(out section);
			attributes.Add(section);
		}
		while (StartOf(34)) {
			ParameterModifier(mod);
		}
		Identifier();
		string parameterName = t.val;
		if (IsDims()) {
			ArrayTypeModifiers(out arrayModifiers);
		}
		if (la.kind == 50) {
			Get();
			TypeName(out type);
		}
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
			Get();
			Expr(out expr);
		}
		mod.Check();
		p = new ParameterDeclarationExpression(type, parameterName, mod.Modifier, expr);
		p.Attributes = attributes;

	}

	void ParameterModifier(ParamModifierList m) {
		if (la.kind == 59) {
			Get();
			m.Add(ParameterModifiers.In);
		} else if (la.kind == 56) {
			Get();
			m.Add(ParameterModifiers.Ref);
		} else if (la.kind == 160) {
			Get();
			m.Add(ParameterModifiers.Optional);
		} else if (la.kind == 167) {
			Get();
			m.Add(ParameterModifiers.Params);
		} else SynErr(269);
	}

	void Statement() {
		Statement stmt = null;
		Location startPos = la.Location;
		string label = String.Empty;


		if (la.kind == 1 || la.kind == 11) {
		} else if (IsLabel()) {
			LabelName(out label);
			compilationUnit.AddChild(new LabelStatement(t.val));

			Expect(11);
			Statement();
		} else if (StartOf(35)) {
			EmbeddedStatement(out stmt);
			compilationUnit.AddChild(stmt);
		} else SynErr(270);
		if (stmt != null) {
		stmt.StartLocation = startPos;
		stmt.EndLocation = t.Location;
		}

	}

	void LabelName(out string name) {
		name = String.Empty;

		if (StartOf(4)) {
			Identifier();
			name = t.val;
		} else if (la.kind == 5) {
			Get();
			name = t.val;
		} else SynErr(271);
	}

	void EmbeddedStatement(out Statement statement) {
		Statement embeddedStatement = null;
		statement = null;
		Expression expr = null;
		string name = String.Empty;
		List<Expression> p = null;

		if (la.kind == 107) {
			Get();
			ExitType exitType = ExitType.None;
			switch (la.kind) {
			case 195: {
				Get();
				exitType = ExitType.Sub;
				break;
			}
			case 114: {
				Get();
				exitType = ExitType.Function;
				break;
			}
			case 171: {
				Get();
				exitType = ExitType.Property;
				break;
			}
			case 95: {
				Get();
				exitType = ExitType.Do;
				break;
			}
			case 111: {
				Get();
				exitType = ExitType.For;
				break;
			}
			case 203: {
				Get();
				exitType = ExitType.Try;
				break;
			}
			case 216: {
				Get();
				exitType = ExitType.While;
				break;
			}
			case 182: {
				Get();
				exitType = ExitType.Select;
				break;
			}
			default: SynErr(272); break;
			}
			statement = new ExitStatement(exitType);
		} else if (la.kind == 203) {
			TryStatement(out statement);
		} else if (la.kind == 76) {
			Get();
			ContinueType continueType = ContinueType.None;
			if (la.kind == 95 || la.kind == 111 || la.kind == 216) {
				if (la.kind == 95) {
					Get();
					continueType = ContinueType.Do;
				} else if (la.kind == 111) {
					Get();
					continueType = ContinueType.For;
				} else {
					Get();
					continueType = ContinueType.While;
				}
			}
			statement = new ContinueStatement(continueType);
		} else if (la.kind == 200) {
			Get();
			if (StartOf(29)) {
				Expr(out expr);
			}
			statement = new ThrowStatement(expr);
		} else if (la.kind == 180) {
			Get();
			if (StartOf(29)) {
				Expr(out expr);
			}
			statement = new ReturnStatement(expr);
		} else if (la.kind == 196) {
			Get();
			Expr(out expr);
			EndOfStmt();
			Block(out embeddedStatement);
			Expect(100);
			Expect(196);
			statement = new LockStatement(expr, embeddedStatement);
		} else if (la.kind == 174) {
			Get();
			Identifier();
			name = t.val;
			if (la.kind == 25) {
				Get();
				if (StartOf(36)) {
					ArgumentList(out p);
				}
				Expect(26);
			}
			statement = new RaiseEventStatement(name, p);

		} else if (la.kind == 218) {
			WithStatement(out statement);
		} else if (la.kind == 43) {
			Get();
			Expression handlerExpr = null;
			Expr(out expr);
			Expect(12);
			Expr(out handlerExpr);
			statement = new AddHandlerStatement(expr, handlerExpr);

		} else if (la.kind == 178) {
			Get();
			Expression handlerExpr = null;
			Expr(out expr);
			Expect(12);
			Expr(out handlerExpr);
			statement = new RemoveHandlerStatement(expr, handlerExpr);

		} else if (la.kind == 216) {
			Get();
			Expr(out expr);
			EndOfStmt();
			Block(out embeddedStatement);
			Expect(100);
			Expect(216);
			statement = new DoLoopStatement(expr, embeddedStatement, ConditionType.While, ConditionPosition.Start);

		} else if (la.kind == 95) {
			Get();
			ConditionType conditionType = ConditionType.None;

			if (la.kind == 209 || la.kind == 216) {
				WhileOrUntil(out conditionType);
				Expr(out expr);
				EndOfStmt();
				Block(out embeddedStatement);
				Expect(138);
				statement = new DoLoopStatement(expr, 
				                               embeddedStatement, 
				                               conditionType == ConditionType.While ? ConditionType.DoWhile : conditionType, 
				                               ConditionPosition.Start);

			} else if (la.kind == 1 || la.kind == 11) {
				EndOfStmt();
				Block(out embeddedStatement);
				Expect(138);
				if (la.kind == 209 || la.kind == 216) {
					WhileOrUntil(out conditionType);
					Expr(out expr);
				}
				statement = new DoLoopStatement(expr, embeddedStatement, conditionType, ConditionPosition.End);

			} else SynErr(273);
		} else if (la.kind == 111) {
			Get();
			Expression group = null;
			TypeReference typeReference;
			string        typeName;
			Location startLocation = t.Location;

			if (la.kind == 97) {
				Get();
				LoopControlVariable(out typeReference, out typeName);
				Expect(125);
				Expr(out group);
				EndOfStmt();
				Block(out embeddedStatement);
				Expect(149);
				if (StartOf(29)) {
					Expr(out expr);
				}
				statement = new ForeachStatement(typeReference, 
				                                typeName,
				                                group, 
				                                embeddedStatement, 
				                                expr);
				statement.StartLocation = startLocation;
				statement.EndLocation   = t.EndLocation;


			} else if (StartOf(37)) {
				Expression start = null;
				Expression end = null;
				Expression step = null;
				Expression variableExpr = null;
				Expression nextExpr = null;
				List<Expression> nextExpressions = null;

				if (IsLoopVariableDeclaration()) {
					LoopControlVariable(out typeReference, out typeName);
				} else {
					typeReference = null; typeName = null;
					SimpleExpr(out variableExpr);
				}
				Expect(10);
				Expr(out start);
				Expect(201);
				Expr(out end);
				if (la.kind == 190) {
					Get();
					Expr(out step);
				}
				EndOfStmt();
				Block(out embeddedStatement);
				Expect(149);
				if (StartOf(29)) {
					Expr(out nextExpr);
					nextExpressions = new List<Expression>();
					nextExpressions.Add(nextExpr);

					while (la.kind == 12) {
						Get();
						Expr(out nextExpr);
						nextExpressions.Add(nextExpr);
					}
				}
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

			} else SynErr(274);
		} else if (la.kind == 105) {
			Get();
			Expr(out expr);
			statement = new ErrorStatement(expr);
		} else if (la.kind == 176) {
			Get();
			bool isPreserve = false;
			if (la.kind == 169) {
				Get();
				isPreserve = true;
			}
			ReDimClause(out expr);
			ReDimStatement reDimStatement = new ReDimStatement(isPreserve);
			statement = reDimStatement;
			SafeAdd(reDimStatement, reDimStatement.ReDimClauses, expr as InvocationExpression);

			while (la.kind == 12) {
				Get();
				ReDimClause(out expr);
				SafeAdd(reDimStatement, reDimStatement.ReDimClauses, expr as InvocationExpression);
			}
		} else if (la.kind == 104) {
			Get();
			Expr(out expr);
			EraseStatement eraseStatement = new EraseStatement();
			if (expr != null) { SafeAdd(eraseStatement, eraseStatement.Expressions, expr);}

			while (la.kind == 12) {
				Get();
				Expr(out expr);
				if (expr != null) { SafeAdd(eraseStatement, eraseStatement.Expressions, expr); }
			}
			statement = eraseStatement;
		} else if (la.kind == 191) {
			Get();
			statement = new StopStatement();
		} else if (la.kind == Tokens.If) {
			Expect(122);
			Location ifStartLocation = t.Location;
			Expr(out expr);
			if (la.kind == 199) {
				Get();
			}
			if (la.kind == 1 || la.kind == 11) {
				EndOfStmt();
				Block(out embeddedStatement);
				IfElseStatement ifStatement = new IfElseStatement(expr, embeddedStatement);
				ifStatement.StartLocation = ifStartLocation;
				Location elseIfStart;

				while (la.kind == 98 || la.kind == 99) {
					if (IsElseIf()) {
						Expect(98);
						elseIfStart = t.Location;
						Expect(122);
					} else {
						Get();
						elseIfStart = t.Location;
					}
					Expression condition = null; Statement block = null;
					Expr(out condition);
					if (la.kind == 199) {
						Get();
					}
					EndOfStmt();
					Block(out block);
					ElseIfSection elseIfSection = new ElseIfSection(condition, block);
					elseIfSection.StartLocation = elseIfStart;
					elseIfSection.EndLocation = t.Location;
					elseIfSection.Parent = ifStatement;
					ifStatement.ElseIfSections.Add(elseIfSection);

				}
				if (la.kind == 98) {
					Get();
					if (la.kind == 1 || la.kind == 11) {
						EndOfStmt();
					}
					Block(out embeddedStatement);
					ifStatement.FalseStatement.Add(embeddedStatement);

				}
				Expect(100);
				Expect(122);
				ifStatement.EndLocation = t.Location;
				statement = ifStatement;

			} else if (StartOf(38)) {
				IfElseStatement ifStatement = new IfElseStatement(expr);
				ifStatement.StartLocation = ifStartLocation;

				SingleLineStatementList(ifStatement.TrueStatement);
				if (la.kind == 98) {
					Get();
					if (StartOf(38)) {
						SingleLineStatementList(ifStatement.FalseStatement);
					}
				}
				ifStatement.EndLocation = t.Location; statement = ifStatement;
			} else SynErr(275);
		} else if (la.kind == 182) {
			Get();
			if (la.kind == 61) {
				Get();
			}
			Expr(out expr);
			EndOfStmt();
			List<SwitchSection> selectSections = new List<SwitchSection>();
			Statement block = null;

			while (la.kind == 61) {
				List<CaseLabel> caseClauses = null; Location caseLocation = la.Location;
				Get();
				CaseClauses(out caseClauses);
				if (IsNotStatementSeparator()) {
					Expect(11);
				}
				EndOfStmt();
				SwitchSection selectSection = new SwitchSection(caseClauses);
				selectSection.StartLocation = caseLocation;

				Block(out block);
				selectSection.Children = block.Children;
				selectSection.EndLocation = t.EndLocation;
				selectSections.Add(selectSection);

			}
			statement = new SwitchStatement(expr, selectSections);

			Expect(100);
			Expect(182);
		} else if (la.kind == 157) {
			OnErrorStatement onErrorStatement = null;
			OnErrorStatement(out onErrorStatement);
			statement = onErrorStatement;
		} else if (la.kind == 119) {
			GotoStatement goToStatement = null;
			GotoStatement(out goToStatement);
			statement = goToStatement;
		} else if (la.kind == 179) {
			ResumeStatement resumeStatement = null;
			ResumeStatement(out resumeStatement);
			statement = resumeStatement;
		} else if (StartOf(37)) {
			Expression val = null;
			AssignmentOperatorType op;

			bool mustBeAssignment = la.kind == Tokens.Plus  || la.kind == Tokens.Minus ||
			                        la.kind == Tokens.Not   || la.kind == Tokens.Times;

			SimpleExpr(out expr);
			if (StartOf(39)) {
				AssignmentOperator(out op);
				Expr(out val);
				expr = new AssignmentExpression(expr, op, val);
			} else if (la.kind == 1 || la.kind == 11 || la.kind == 98) {
				if (mustBeAssignment) Error("error in assignment.");
			} else SynErr(276);
			if(expr is MemberReferenceExpression || expr is IdentifierExpression) {
			expr = new InvocationExpression(expr);
			}
			statement = new ExpressionStatement(expr);

		} else if (la.kind == 60) {
			Get();
			SimpleExpr(out expr);
			statement = new ExpressionStatement(expr);
		} else if (la.kind == 211) {
			Get();
			Statement block; 
			if (Peek(1).kind == Tokens.As) {
				LocalVariableDeclaration resourceAquisition = new LocalVariableDeclaration(Modifiers.None);
				VariableDeclarator(resourceAquisition.Variables);
				while (la.kind == 12) {
					Get();
					VariableDeclarator(resourceAquisition.Variables);
				}
				Block(out block);
				statement = new UsingStatement(resourceAquisition, block);

			} else if (StartOf(29)) {
				Expr(out expr);
				Block(out block);
				statement = new UsingStatement(new ExpressionStatement(expr), block);
			} else SynErr(277);
			Expect(100);
			Expect(211);
		} else if (StartOf(40)) {
			LocalDeclarationStatement(out statement);
		} else SynErr(278);
	}

	void LocalDeclarationStatement(out Statement statement) {
		ModifierList m = new ModifierList();
		LocalVariableDeclaration localVariableDeclaration;
		bool dimfound = false;

		while (la.kind == 75 || la.kind == 92 || la.kind == 189) {
			if (la.kind == 75) {
				Get();
				m.Add(Modifiers.Const, t.Location);
			} else if (la.kind == 189) {
				Get();
				m.Add(Modifiers.Static, t.Location);
			} else {
				Get();
				dimfound = true;
			}
		}
		if(dimfound && (m.Modifier & Modifiers.Const) != 0) {
		Error("Dim is not allowed on constants.");
		}

		if(m.isNone && dimfound == false) {
			Error("Const, Dim or Static expected");
		}

		localVariableDeclaration = new LocalVariableDeclaration(m.Modifier);
		localVariableDeclaration.StartLocation = t.Location;

		VariableDeclarator(localVariableDeclaration.Variables);
		while (la.kind == 12) {
			Get();
			VariableDeclarator(localVariableDeclaration.Variables);
		}
		statement = localVariableDeclaration;

	}

	void TryStatement(out Statement tryStatement) {
		Statement blockStmt = null, finallyStmt = null;List<CatchClause> catchClauses = null;

		Expect(203);
		EndOfStmt();
		Block(out blockStmt);
		if (la.kind == 62 || la.kind == 100 || la.kind == 110) {
			CatchClauses(out catchClauses);
		}
		if (la.kind == 110) {
			Get();
			EndOfStmt();
			Block(out finallyStmt);
		}
		Expect(100);
		Expect(203);
		tryStatement = new TryCatchStatement(blockStmt, catchClauses, finallyStmt);

	}

	void WithStatement(out Statement withStatement) {
		Statement blockStmt = null;
		Expression expr = null;

		Expect(218);
		Location start = t.Location;
		Expr(out expr);
		EndOfStmt();
		withStatement = new WithStatement(expr);
		withStatement.StartLocation = start;

		Block(out blockStmt);
		((WithStatement)withStatement).Body = (BlockStatement)blockStmt;

		Expect(100);
		Expect(218);
		withStatement.EndLocation = t.Location;
	}

	void WhileOrUntil(out ConditionType conditionType) {
		conditionType = ConditionType.None;
		if (la.kind == 216) {
			Get();
			conditionType = ConditionType.While;
		} else if (la.kind == 209) {
			Get();
			conditionType = ConditionType.Until;
		} else SynErr(279);
	}

	void LoopControlVariable(out TypeReference type, out string name) {
		ArrayList arrayModifiers = null;
		type = null;

		Qualident(out name);
		if (IsDims()) {
			ArrayTypeModifiers(out arrayModifiers);
		}
		if (la.kind == 50) {
			Get();
			TypeName(out type);
			if (name.IndexOf('.') > 0) { Error("No type def for 'for each' member indexer allowed."); }
		}
		if (type != null) {
		if(type.RankSpecifier != null && arrayModifiers != null) {
			Error("array rank only allowed one time");
		} else if (arrayModifiers != null) {
			type.RankSpecifier = (int[])arrayModifiers.ToArray(typeof(int));
		}
		}

	}

	void ReDimClause(out Expression expr) {
		SimpleNonInvocationExpression(out expr);
		ReDimClauseInternal(ref expr);
	}

	void SingleLineStatementList(List<Statement> list) {
		Statement embeddedStatement = null;
		if (la.kind == 100) {
			Get();
			embeddedStatement = new EndStatement();
		} else if (StartOf(35)) {
			EmbeddedStatement(out embeddedStatement);
		} else SynErr(280);
		if (embeddedStatement != null) list.Add(embeddedStatement);
		while (la.kind == 11) {
			Get();
			while (la.kind == 11) {
				Get();
			}
			if (la.kind == 100) {
				Get();
				embeddedStatement = new EndStatement();
			} else if (StartOf(35)) {
				EmbeddedStatement(out embeddedStatement);
			} else SynErr(281);
			if (embeddedStatement != null) list.Add(embeddedStatement);
		}
	}

	void CaseClauses(out List<CaseLabel> caseClauses) {
		caseClauses = new List<CaseLabel>();
		CaseLabel caseClause = null;

		CaseClause(out caseClause);
		if (caseClause != null) { caseClauses.Add(caseClause); }
		while (la.kind == 12) {
			Get();
			CaseClause(out caseClause);
			if (caseClause != null) { caseClauses.Add(caseClause); }
		}
	}

	void OnErrorStatement(out OnErrorStatement stmt) {
		stmt = null;
		GotoStatement goToStatement = null;

		Expect(157);
		Expect(105);
		if (IsNegativeLabelName()) {
			Expect(119);
			Expect(18);
			Expect(5);
			long intLabel = Int64.Parse(t.val);
			if(intLabel != 1) {
				Error("invalid label in on error statement.");
			}
			stmt = new OnErrorStatement(new GotoStatement((intLabel * -1).ToString()));

		} else if (la.kind == 119) {
			GotoStatement(out goToStatement);
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

		} else if (la.kind == 179) {
			Get();
			Expect(149);
			stmt = new OnErrorStatement(new ResumeStatement(true));

		} else SynErr(282);
	}

	void GotoStatement(out GotoStatement goToStatement) {
		string label = String.Empty;

		Expect(119);
		LabelName(out label);
		goToStatement = new GotoStatement(label);

	}

	void ResumeStatement(out ResumeStatement resumeStatement) {
		resumeStatement = null;
		string label = String.Empty;

		if (IsResumeNext()) {
			Expect(179);
			Expect(149);
			resumeStatement = new ResumeStatement(true);
		} else if (la.kind == 179) {
			Get();
			if (StartOf(41)) {
				LabelName(out label);
			}
			resumeStatement = new ResumeStatement(label);
		} else SynErr(283);
	}

	void ReDimClauseInternal(ref Expression expr) {
		List<Expression> arguments; bool canBeNormal; bool canBeRedim; string name;
		while (la.kind == 16 || la.kind == 25) {
			if (la.kind == 16) {
				Get();
				IdentifierOrKeyword(out name);
				expr = new MemberReferenceExpression(expr, name);
			} else {
				InvocationExpression(ref expr);
			}
		}
		Expect(25);
		NormalOrReDimArgumentList(out arguments, out canBeNormal, out canBeRedim);
		Expect(26);
		expr = new InvocationExpression(expr, arguments);
		if (canBeRedim == false || canBeNormal && (la.kind == Tokens.Dot || la.kind == Tokens.OpenParenthesis)) {
			if (this.Errors.Count == 0) {
				// don't recurse on parse errors - could result in endless recursion
				ReDimClauseInternal(ref expr);
			}
		}

	}

	void CaseClause(out CaseLabel caseClause) {
		Expression expr = null;
		Expression sexpr = null;
		BinaryOperatorType op = BinaryOperatorType.None;
		caseClause = null;

		if (la.kind == 98) {
			Get();
			caseClause = new CaseLabel();
		} else if (StartOf(42)) {
			if (la.kind == 131) {
				Get();
			}
			switch (la.kind) {
			case 28: {
				Get();
				op = BinaryOperatorType.LessThan;
				break;
			}
			case 27: {
				Get();
				op = BinaryOperatorType.GreaterThan;
				break;
			}
			case 31: {
				Get();
				op = BinaryOperatorType.LessThanOrEqual;
				break;
			}
			case 30: {
				Get();
				op = BinaryOperatorType.GreaterThanOrEqual;
				break;
			}
			case 10: {
				Get();
				op = BinaryOperatorType.Equality;
				break;
			}
			case 29: {
				Get();
				op = BinaryOperatorType.InEquality;
				break;
			}
			default: SynErr(284); break;
			}
			Expr(out expr);
			caseClause = new CaseLabel(op, expr);

		} else if (StartOf(29)) {
			Expr(out expr);
			if (la.kind == 201) {
				Get();
				Expr(out sexpr);
			}
			caseClause = new CaseLabel(expr, sexpr);

		} else SynErr(285);
	}

	void CatchClauses(out List<CatchClause> catchClauses) {
		catchClauses = new List<CatchClause>();
		TypeReference type = null;
		Statement blockStmt = null;
		Expression expr = null;
		string name = String.Empty;

		while (la.kind == 62) {
			Get();
			if (StartOf(4)) {
				Identifier();
				name = t.val;
				if (la.kind == 50) {
					Get();
					TypeName(out type);
				}
			}
			if (la.kind == 214) {
				Get();
				Expr(out expr);
			}
			EndOfStmt();
			Block(out blockStmt);
			catchClauses.Add(new CatchClause(type, name, blockStmt, expr));
		}
	}


	
	void ParseRoot()
	{
		VBNET();
		Expect(0); // expect end-of-file automatically added

	}
	
	private bool StartOf(int s)
	{
		return set[s, lexer.LookAhead.kind];
	}

	protected override void SynErr(int line, int col, int errorNumber)
	{
		this.Errors.Error(line, col, ErrorDesc(errorNumber));
	}

	static bool[,] set = {
		{T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,x, T,T,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,x, T,T,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,x, T,T,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,T, x,x,x,x, x,x,x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,T, x,x,x,x, x,x,x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, T,T,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, T,x,T,x, x,x,x,T, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,T, x,x,x,x, x,x,x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,T, x,T,T,x, x,x,x,x, x,x,x,x, x,T,x,T, x,x,T,x, x,x,x,x, x,x,x,x, x,T,T,T, x,x,x,T, x,x,T,x, T,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,T,x,x, T,x,x,x, x,x,T,x, T,x,T,x, x,T,x,x, x,T,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,T,x, T,x,x,x, x,x,T,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,T,T, T,x,x,x, x,T,T,x, x,T,T,x, x,x,x,x, x,x,T,T, T,T,T,x, x,x,x,T, x,x,x,x, x,x,x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,T, x,T,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,T,T, x,x,x,T, x,x,T,x, T,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,T,x,x, T,x,x,x, x,x,T,x, T,x,T,x, x,T,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,x, x,x,T,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,T,T, T,x,x,x, x,T,x,x, x,T,T,x, x,x,x,x, x,x,T,T, T,T,T,x, x,x,x,T, x,x,x,x, x,x,x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,T, x,x,x,x, x,x,x,x, x,T,x,x, T,T,T,T, T,x,T,x, x,x,x,x, x,x,T,T, x,x,T,x, T,x,x,x, T,T,T,x, x,x,x,x, T,x,x,x, x,x,T,x, x,T,T,x, x,T,x,x, x,x,x,x, x,T,T,T, x,x,x,T, x,x,x,x, T,T,x,x, T,x,T,x, x,x,T,x, T,T,T,x, T,T,T,T, T,T,x,T, x,x,x,x, x,x,x,x, T,T,x,x, T,x,x,x, x,x,T,T, x,T,T,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,T, x,T,x,T, T,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,x, T,T,T,x, T,x,T,x, T,T,x,T, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,T,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,T, x,x,x,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,T,T, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,T, x,x,x,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, T,x,x,x, x,x,x,x, x,x,T,x, x,x,T,x, x,x,x,x, T,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,T,T,T, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,x, T,T,T,x, T,x,T,T, T,T,x,T, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,T,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,T,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, T,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,x, T,T,T,x, T,x,T,T, T,T,x,T, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, T,x,T,T, x,x,T,T, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,T,x,T, T,T,T,T, x,T,T,x, x,x,x,T, T,T,T,T, T,T,T,x, T,T,T,x, x,T,T,T, T,T,T,T, T,T,T,T, x,x,x,T, x,T,T,x, T,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,x,x, T,T,x,x, T,x,T,x, x,x,T,x, T,x,T,x, x,T,x,x, x,T,x,T, x,x,x,x, T,T,x,x, T,x,T,T, x,x,T,x, T,x,x,x, x,x,T,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,T,T, T,x,x,x, x,T,x,x, x,T,T,x, x,x,T,x, T,T,T,T, T,T,T,x, x,x,x,T, x,x,x,x, x,x,x,x},
		{x,T,T,T, T,T,T,T, T,T,x,T, x,x,x,x, T,x,x,x, x,x,x,T, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,x,x, x,T,x,T, T,T,T,T, x,T,T,x, T,x,x,T, T,T,T,T, T,T,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, x,x,x,T, T,T,T,T, T,x,x,x, T,x,x,T, T,T,x,T, T,T,x,T, x,T,x,x, T,T,x,T, T,x,T,x, x,x,T,x, T,x,T,x, x,T,x,x, x,T,x,T, x,x,x,x, T,T,x,x, T,x,x,T, x,x,T,x, T,T,x,x, x,x,T,x, x,x,x,x, x,T,x,x, x,x,T,x, T,x,T,T, T,T,T,x, x,x,T,T, T,T,x,T, x,T,x,x, T,T,T,x, T,x,T,T, T,T,T,T, T,T,T,T, x,x,x,T, T,x,T,x, x,x,x,x},
		{x,T,T,T, T,T,T,T, T,T,T,T, T,x,x,x, T,x,x,x, x,x,x,T, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,x,x, x,T,T,T, T,T,T,T, x,T,T,x, T,x,x,T, T,T,T,T, T,T,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, x,x,x,T, T,T,T,T, T,x,T,x, T,x,x,T, T,T,x,T, T,T,x,T, x,T,x,x, T,T,x,T, T,x,T,x, x,x,T,x, T,x,T,x, x,T,x,x, x,T,x,T, x,x,x,x, T,T,x,x, T,x,x,T, x,x,T,x, T,T,x,x, x,x,T,x, x,x,x,x, x,T,x,x, x,x,T,x, T,x,T,T, T,T,T,x, x,x,T,T, T,T,x,T, x,T,x,x, T,T,T,x, T,x,T,T, T,T,T,T, T,T,T,T, x,x,x,T, T,x,T,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,T,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,T,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, T,x,T,T, x,x,T,T, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,T,x,T, T,T,T,T, x,T,T,x, x,x,x,T, T,T,T,T, T,T,T,x, T,T,T,x, x,T,T,T, T,T,T,T, T,T,T,T, x,x,x,T, x,T,T,x, T,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,T,x, T,T,x,x, T,x,T,x, x,x,T,x, T,x,T,x, x,T,x,x, x,T,x,T, x,x,x,x, T,T,x,x, T,x,T,T, x,x,T,x, T,x,x,x, x,x,T,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,T,T, T,x,x,x, x,T,x,x, x,T,T,x, x,x,T,x, T,T,T,T, T,T,T,x, x,x,x,T, x,x,x,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x},
		{x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,T,x,T, T,T,T,T, x,T,T,x, x,x,x,T, T,T,T,T, T,T,T,x, T,T,T,x, x,T,T,T, T,T,T,T, T,T,T,T, x,x,x,T, x,T,T,x, T,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,x,x, T,T,x,x, T,x,T,x, x,x,T,x, T,x,T,x, x,T,x,x, x,T,x,T, x,x,x,x, T,T,x,x, T,x,x,T, x,x,T,x, T,x,x,x, x,x,T,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,T,T, T,x,x,x, x,T,x,x, x,T,T,x, x,x,T,x, T,T,T,T, T,T,T,x, x,x,x,T, x,x,x,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, T,x,T,T, x,x,T,T, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,T,x,T, T,T,T,T, x,T,T,x, x,x,x,T, T,T,T,T, T,T,T,x, T,T,T,x, x,T,T,T, T,T,T,T, T,T,T,T, x,x,x,T, x,T,T,x, T,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,x,x, T,T,x,x, T,x,T,x, x,x,T,x, T,x,T,x, x,T,x,x, x,T,x,T, x,x,x,x, T,T,x,x, T,x,x,T, x,x,T,x, T,x,x,x, x,x,T,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,T,T, T,x,x,x, x,T,x,x, x,T,T,x, x,x,T,x, T,T,T,T, T,T,T,x, x,x,x,T, x,x,x,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, T,x,x,x, x,x,x,T, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,x,x, x,T,x,T, T,T,T,T, x,T,T,x, T,x,x,T, T,T,T,T, T,T,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, x,x,x,T, T,T,T,T, T,x,x,x, x,x,x,T, T,T,x,T, T,T,x,T, x,T,x,x, T,T,x,T, T,x,T,x, x,x,T,x, T,x,T,x, x,T,x,x, x,T,x,T, x,x,x,x, T,T,x,x, T,x,x,T, x,x,T,x, T,T,x,x, x,x,T,x, x,x,x,x, x,T,x,x, x,x,T,x, T,x,T,T, T,T,T,x, x,x,T,T, T,T,x,T, x,T,x,x, T,T,T,x, T,x,T,T, T,T,T,T, T,T,T,T, x,x,x,T, T,x,T,x, x,x,x,x},
		{x,x,T,T, T,T,T,T, T,T,x,x, T,x,x,x, T,x,T,T, x,x,T,T, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,T,x,T, T,T,T,T, x,T,T,x, x,x,x,T, T,T,T,T, T,T,T,x, T,T,T,x, x,T,T,T, T,T,T,T, T,T,T,T, x,x,x,T, x,T,T,x, T,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,T,x, T,T,x,x, T,x,T,x, x,x,T,x, T,x,T,x, x,T,x,x, x,T,x,T, x,x,x,x, T,T,x,x, T,x,T,T, x,x,T,x, T,x,x,x, x,x,T,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,T,T, T,x,x,x, x,T,x,x, x,T,T,x, x,x,T,x, T,T,T,T, T,T,T,x, x,x,x,T, x,x,x,x, x,x,x,x},
		{x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, T,x,x,x, x,x,x,T, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,T,x,T, T,T,T,T, x,T,T,x, x,x,x,T, T,T,T,T, T,T,T,x, T,T,T,x, x,T,T,T, T,T,T,T, T,T,T,T, x,x,x,T, x,T,T,x, T,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,x,x, T,T,x,x, T,x,T,x, x,x,T,x, T,x,T,x, x,T,x,x, x,T,x,T, x,x,x,x, T,T,x,x, T,x,x,T, x,x,T,x, T,x,x,x, x,x,T,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,T,T, T,x,x,x, x,T,x,x, x,T,T,x, x,x,T,x, T,T,T,T, T,T,T,x, x,x,x,T, x,x,x,x, x,x,x,x},
		{x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, T,x,x,x, x,x,x,T, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,x,x, x,T,x,T, T,T,T,T, x,T,T,x, T,x,x,T, T,T,T,T, T,T,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, x,x,x,T, T,T,T,T, T,x,x,x, T,x,x,T, T,T,x,T, T,T,x,T, x,T,x,x, T,T,x,T, T,x,T,x, x,x,T,x, T,x,T,x, x,T,x,x, x,T,x,T, x,x,x,x, T,T,x,x, T,x,x,T, x,x,T,x, T,T,x,x, x,x,T,x, x,x,x,x, x,T,x,x, x,x,T,x, T,x,T,T, T,T,T,x, x,x,T,T, T,T,x,T, x,T,x,x, T,T,T,x, T,x,T,T, T,T,T,T, T,T,T,T, x,x,x,T, T,x,T,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, T,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,T, x,x,x,x, x,x,x,x},
		{x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,T, x,x,x,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x}

	};
	
	string ErrorDesc(int errorNumber)
	{
		switch (errorNumber) {
			case 0: return "EOF expected";
			case 1: return "EOL expected";
			case 2: return "ident expected";
			case 3: return "LiteralString expected";
			case 4: return "LiteralCharacter expected";
			case 5: return "LiteralInteger expected";
			case 6: return "LiteralDouble expected";
			case 7: return "LiteralSingle expected";
			case 8: return "LiteralDecimal expected";
			case 9: return "LiteralDate expected";
			case 10: return "\"=\" expected";
			case 11: return "\":\" expected";
			case 12: return "\",\" expected";
			case 13: return "\"&\" expected";
			case 14: return "\"/\" expected";
			case 15: return "\"\\\\\" expected";
			case 16: return "\".\" expected";
			case 17: return "\"!\" expected";
			case 18: return "\"-\" expected";
			case 19: return "\"+\" expected";
			case 20: return "\"^\" expected";
			case 21: return "\"?\" expected";
			case 22: return "\"*\" expected";
			case 23: return "\"{\" expected";
			case 24: return "\"}\" expected";
			case 25: return "\"(\" expected";
			case 26: return "\")\" expected";
			case 27: return "\">\" expected";
			case 28: return "\"<\" expected";
			case 29: return "\"<>\" expected";
			case 30: return "\">=\" expected";
			case 31: return "\"<=\" expected";
			case 32: return "\"<<\" expected";
			case 33: return "\">>\" expected";
			case 34: return "\"+=\" expected";
			case 35: return "\"^=\" expected";
			case 36: return "\"-=\" expected";
			case 37: return "\"*=\" expected";
			case 38: return "\"/=\" expected";
			case 39: return "\"\\\\=\" expected";
			case 40: return "\"<<=\" expected";
			case 41: return "\">>=\" expected";
			case 42: return "\"&=\" expected";
			case 43: return "\"AddHandler\" expected";
			case 44: return "\"AddressOf\" expected";
			case 45: return "\"Aggregate\" expected";
			case 46: return "\"Alias\" expected";
			case 47: return "\"And\" expected";
			case 48: return "\"AndAlso\" expected";
			case 49: return "\"Ansi\" expected";
			case 50: return "\"As\" expected";
			case 51: return "\"Ascending\" expected";
			case 52: return "\"Assembly\" expected";
			case 53: return "\"Auto\" expected";
			case 54: return "\"Binary\" expected";
			case 55: return "\"Boolean\" expected";
			case 56: return "\"ByRef\" expected";
			case 57: return "\"By\" expected";
			case 58: return "\"Byte\" expected";
			case 59: return "\"ByVal\" expected";
			case 60: return "\"Call\" expected";
			case 61: return "\"Case\" expected";
			case 62: return "\"Catch\" expected";
			case 63: return "\"CBool\" expected";
			case 64: return "\"CByte\" expected";
			case 65: return "\"CChar\" expected";
			case 66: return "\"CDate\" expected";
			case 67: return "\"CDbl\" expected";
			case 68: return "\"CDec\" expected";
			case 69: return "\"Char\" expected";
			case 70: return "\"CInt\" expected";
			case 71: return "\"Class\" expected";
			case 72: return "\"CLng\" expected";
			case 73: return "\"CObj\" expected";
			case 74: return "\"Compare\" expected";
			case 75: return "\"Const\" expected";
			case 76: return "\"Continue\" expected";
			case 77: return "\"CSByte\" expected";
			case 78: return "\"CShort\" expected";
			case 79: return "\"CSng\" expected";
			case 80: return "\"CStr\" expected";
			case 81: return "\"CType\" expected";
			case 82: return "\"CUInt\" expected";
			case 83: return "\"CULng\" expected";
			case 84: return "\"CUShort\" expected";
			case 85: return "\"Custom\" expected";
			case 86: return "\"Date\" expected";
			case 87: return "\"Decimal\" expected";
			case 88: return "\"Declare\" expected";
			case 89: return "\"Default\" expected";
			case 90: return "\"Delegate\" expected";
			case 91: return "\"Descending\" expected";
			case 92: return "\"Dim\" expected";
			case 93: return "\"DirectCast\" expected";
			case 94: return "\"Distinct\" expected";
			case 95: return "\"Do\" expected";
			case 96: return "\"Double\" expected";
			case 97: return "\"Each\" expected";
			case 98: return "\"Else\" expected";
			case 99: return "\"ElseIf\" expected";
			case 100: return "\"End\" expected";
			case 101: return "\"EndIf\" expected";
			case 102: return "\"Enum\" expected";
			case 103: return "\"Equals\" expected";
			case 104: return "\"Erase\" expected";
			case 105: return "\"Error\" expected";
			case 106: return "\"Event\" expected";
			case 107: return "\"Exit\" expected";
			case 108: return "\"Explicit\" expected";
			case 109: return "\"False\" expected";
			case 110: return "\"Finally\" expected";
			case 111: return "\"For\" expected";
			case 112: return "\"Friend\" expected";
			case 113: return "\"From\" expected";
			case 114: return "\"Function\" expected";
			case 115: return "\"Get\" expected";
			case 116: return "\"GetType\" expected";
			case 117: return "\"Global\" expected";
			case 118: return "\"GoSub\" expected";
			case 119: return "\"GoTo\" expected";
			case 120: return "\"Group\" expected";
			case 121: return "\"Handles\" expected";
			case 122: return "\"If\" expected";
			case 123: return "\"Implements\" expected";
			case 124: return "\"Imports\" expected";
			case 125: return "\"In\" expected";
			case 126: return "\"Infer\" expected";
			case 127: return "\"Inherits\" expected";
			case 128: return "\"Integer\" expected";
			case 129: return "\"Interface\" expected";
			case 130: return "\"Into\" expected";
			case 131: return "\"Is\" expected";
			case 132: return "\"IsNot\" expected";
			case 133: return "\"Join\" expected";
			case 134: return "\"Let\" expected";
			case 135: return "\"Lib\" expected";
			case 136: return "\"Like\" expected";
			case 137: return "\"Long\" expected";
			case 138: return "\"Loop\" expected";
			case 139: return "\"Me\" expected";
			case 140: return "\"Mod\" expected";
			case 141: return "\"Module\" expected";
			case 142: return "\"MustInherit\" expected";
			case 143: return "\"MustOverride\" expected";
			case 144: return "\"MyBase\" expected";
			case 145: return "\"MyClass\" expected";
			case 146: return "\"Namespace\" expected";
			case 147: return "\"Narrowing\" expected";
			case 148: return "\"New\" expected";
			case 149: return "\"Next\" expected";
			case 150: return "\"Not\" expected";
			case 151: return "\"Nothing\" expected";
			case 152: return "\"NotInheritable\" expected";
			case 153: return "\"NotOverridable\" expected";
			case 154: return "\"Object\" expected";
			case 155: return "\"Of\" expected";
			case 156: return "\"Off\" expected";
			case 157: return "\"On\" expected";
			case 158: return "\"Operator\" expected";
			case 159: return "\"Option\" expected";
			case 160: return "\"Optional\" expected";
			case 161: return "\"Or\" expected";
			case 162: return "\"Order\" expected";
			case 163: return "\"OrElse\" expected";
			case 164: return "\"Overloads\" expected";
			case 165: return "\"Overridable\" expected";
			case 166: return "\"Overrides\" expected";
			case 167: return "\"ParamArray\" expected";
			case 168: return "\"Partial\" expected";
			case 169: return "\"Preserve\" expected";
			case 170: return "\"Private\" expected";
			case 171: return "\"Property\" expected";
			case 172: return "\"Protected\" expected";
			case 173: return "\"Public\" expected";
			case 174: return "\"RaiseEvent\" expected";
			case 175: return "\"ReadOnly\" expected";
			case 176: return "\"ReDim\" expected";
			case 177: return "\"Rem\" expected";
			case 178: return "\"RemoveHandler\" expected";
			case 179: return "\"Resume\" expected";
			case 180: return "\"Return\" expected";
			case 181: return "\"SByte\" expected";
			case 182: return "\"Select\" expected";
			case 183: return "\"Set\" expected";
			case 184: return "\"Shadows\" expected";
			case 185: return "\"Shared\" expected";
			case 186: return "\"Short\" expected";
			case 187: return "\"Single\" expected";
			case 188: return "\"Skip\" expected";
			case 189: return "\"Static\" expected";
			case 190: return "\"Step\" expected";
			case 191: return "\"Stop\" expected";
			case 192: return "\"Strict\" expected";
			case 193: return "\"String\" expected";
			case 194: return "\"Structure\" expected";
			case 195: return "\"Sub\" expected";
			case 196: return "\"SyncLock\" expected";
			case 197: return "\"Take\" expected";
			case 198: return "\"Text\" expected";
			case 199: return "\"Then\" expected";
			case 200: return "\"Throw\" expected";
			case 201: return "\"To\" expected";
			case 202: return "\"True\" expected";
			case 203: return "\"Try\" expected";
			case 204: return "\"TryCast\" expected";
			case 205: return "\"TypeOf\" expected";
			case 206: return "\"UInteger\" expected";
			case 207: return "\"ULong\" expected";
			case 208: return "\"Unicode\" expected";
			case 209: return "\"Until\" expected";
			case 210: return "\"UShort\" expected";
			case 211: return "\"Using\" expected";
			case 212: return "\"Variant\" expected";
			case 213: return "\"Wend\" expected";
			case 214: return "\"When\" expected";
			case 215: return "\"Where\" expected";
			case 216: return "\"While\" expected";
			case 217: return "\"Widening\" expected";
			case 218: return "\"With\" expected";
			case 219: return "\"WithEvents\" expected";
			case 220: return "\"WriteOnly\" expected";
			case 221: return "\"Xor\" expected";
			case 222: return "??? expected";
			case 223: return "invalid EndOfStmt";
			case 224: return "invalid OptionStmt";
			case 225: return "invalid OptionStmt";
			case 226: return "invalid GlobalAttributeSection";
			case 227: return "invalid GlobalAttributeSection";
			case 228: return "invalid NamespaceMemberDecl";
			case 229: return "invalid OptionValue";
			case 230: return "invalid ImportClause";
			case 231: return "invalid Identifier";
			case 232: return "invalid AttributeSection";
			case 233: return "invalid TypeModifier";
			case 234: return "invalid NonModuleDeclaration";
			case 235: return "invalid NonModuleDeclaration";
			case 236: return "invalid TypeParameterConstraints";
			case 237: return "invalid TypeParameterConstraint";
			case 238: return "invalid NonArrayTypeName";
			case 239: return "invalid MemberModifier";
			case 240: return "invalid StructureMemberDecl";
			case 241: return "invalid StructureMemberDecl";
			case 242: return "invalid StructureMemberDecl";
			case 243: return "invalid StructureMemberDecl";
			case 244: return "invalid StructureMemberDecl";
			case 245: return "invalid StructureMemberDecl";
			case 246: return "invalid StructureMemberDecl";
			case 247: return "invalid StructureMemberDecl";
			case 248: return "invalid InterfaceMemberDecl";
			case 249: return "invalid InterfaceMemberDecl";
			case 250: return "invalid Expr";
			case 251: return "invalid Charset";
			case 252: return "invalid IdentifierForFieldDeclaration";
			case 253: return "invalid VariableDeclaratorPartAfterIdentifier";
			case 254: return "invalid AccessorDecls";
			case 255: return "invalid EventAccessorDeclaration";
			case 256: return "invalid OverloadableOperator";
			case 257: return "invalid EventMemberSpecifier";
			case 258: return "invalid AssignmentOperator";
			case 259: return "invalid SimpleNonInvocationExpression";
			case 260: return "invalid PrimitiveTypeName";
			case 261: return "invalid CastTarget";
			case 262: return "invalid ComparisonExpr";
			case 263: return "invalid FromOrAggregateQueryOperator";
			case 264: return "invalid QueryOperator";
			case 265: return "invalid PartitionQueryOperator";
			case 266: return "invalid Argument";
			case 267: return "invalid QualIdentAndTypeArguments";
			case 268: return "invalid AttributeArguments";
			case 269: return "invalid ParameterModifier";
			case 270: return "invalid Statement";
			case 271: return "invalid LabelName";
			case 272: return "invalid EmbeddedStatement";
			case 273: return "invalid EmbeddedStatement";
			case 274: return "invalid EmbeddedStatement";
			case 275: return "invalid EmbeddedStatement";
			case 276: return "invalid EmbeddedStatement";
			case 277: return "invalid EmbeddedStatement";
			case 278: return "invalid EmbeddedStatement";
			case 279: return "invalid WhileOrUntil";
			case 280: return "invalid SingleLineStatementList";
			case 281: return "invalid SingleLineStatementList";
			case 282: return "invalid OnErrorStatement";
			case 283: return "invalid ResumeStatement";
			case 284: return "invalid CaseClause";
			case 285: return "invalid CaseClause";

			default: return "error " + errorNumber;
		}
	}

} // end Parser

} // end namespace
