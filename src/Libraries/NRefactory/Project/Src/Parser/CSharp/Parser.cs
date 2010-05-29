using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Ast;
using ASTAttribute = ICSharpCode.NRefactory.Ast.Attribute;
using Types = ICSharpCode.NRefactory.Ast.ClassType;



using System;

namespace ICSharpCode.NRefactory.Parser.CSharp {


partial class Parser : AbstractParser
{
	public const int _EOF = 0;
	public const int _ident = 1;
	public const int _Literal = 2;
	public const int maxT = 145;  //<! max term (w/o pragmas)

	const  bool   T            = true;
	const  bool   x            = false;
	


	void Get () {

		lexer.NextToken();
	}

	void CS() {
		lexer.NextToken(); /* get the first token */
		while (la.kind == 71) {
			ExternAliasDirective();
		}
		while (la.kind == 121) {
			UsingDirective();
		}
		while (IsGlobalAttrTarget()) {
			GlobalAttributeSection();
		}
		while (StartOf(1)) {
			NamespaceMemberDecl();
		}
		Expect(0);
	}

	void ExternAliasDirective() {
		ExternAliasDirective ead = new ExternAliasDirective { StartLocation = la.Location };
		Expect(71);
		Identifier();
		if (t.val != "alias") Error("Expected 'extern alias'.");
		Identifier();
		ead.Name = t.val;
		Expect(11);
		ead.EndLocation = t.EndLocation;
		compilationUnit.AddChild(ead);
	}

	void UsingDirective() {
		string qualident = null; TypeReference aliasedType = null;

		Expect(121);
		Location startPos = t.Location;
		Qualident(out qualident);
		if (la.kind == 3) {
			Get();
			NonArrayType(out aliasedType);
		}
		Expect(11);
		if (qualident != null && qualident.Length > 0) {
		 INode node;
		 if (aliasedType != null) {
		     node = new UsingDeclaration(qualident, aliasedType);
		 } else {
		     node = new UsingDeclaration(qualident);
		 }
		 node.StartLocation = startPos;
		 node.EndLocation   = t.EndLocation;
		 compilationUnit.AddChild(node);
		}

	}

	void GlobalAttributeSection() {
		Expect(18);
		Location startPos = t.Location;
		Identifier();
		if (t.val != "assembly" && t.val != "module") Error("global attribute target specifier (assembly or module) expected");
		string attributeTarget = t.val;
		List<ASTAttribute> attributes = new List<ASTAttribute>();
		ASTAttribute attribute;

		Expect(9);
		Attribute(out attribute);
		attributes.Add(attribute);
		while (NotFinalComma()) {
			Expect(14);
			Attribute(out attribute);
			attributes.Add(attribute);
		}
		if (la.kind == 14) {
			Get();
		}
		Expect(19);
		AttributeSection section = new AttributeSection {
		   AttributeTarget = attributeTarget,
		   Attributes = attributes,
		   StartLocation = startPos,
		   EndLocation = t.EndLocation
		};
		compilationUnit.AddChild(section);

	}

	void NamespaceMemberDecl() {
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		ModifierList m = new ModifierList();
		string qualident;

		if (la.kind == 88) {
			Get();
			Location startPos = t.Location;
			Qualident(out qualident);
			INode node =  new NamespaceDeclaration(qualident);
			node.StartLocation = startPos;
			compilationUnit.AddChild(node);
			compilationUnit.BlockStart(node);

			Expect(16);
			while (la.kind == 71) {
				ExternAliasDirective();
			}
			while (la.kind == 121) {
				UsingDirective();
			}
			while (StartOf(1)) {
				NamespaceMemberDecl();
			}
			Expect(17);
			if (la.kind == 11) {
				Get();
			}
			node.EndLocation   = t.EndLocation;
			compilationUnit.BlockEnd();

		} else if (StartOf(2)) {
			while (la.kind == 18) {
				AttributeSection(out section);
				attributes.Add(section);
			}
			while (StartOf(3)) {
				TypeModifier(m);
			}
			TypeDecl(m, attributes);
		} else SynErr(146);
	}

	void Qualident(out string qualident) {
		Identifier();
		qualidentBuilder.Length = 0; qualidentBuilder.Append(t.val);
		while (DotAndIdent()) {
			Expect(15);
			Identifier();
			qualidentBuilder.Append('.');
			qualidentBuilder.Append(t.val); 

		}
		qualident = qualidentBuilder.ToString();
	}

	void NonArrayType(out TypeReference type) {
		Location startPos = la.Location;
		string name;
		int pointer = 0;
		type = null;

		if (StartOf(4)) {
			ClassType(out type, false);
		} else if (StartOf(5)) {
			SimpleType(out name);
			type = new TypeReference(name, true);
		} else if (la.kind == 123) {
			Get();
			Expect(6);
			pointer = 1; type = new TypeReference("System.Void", true);
		} else SynErr(147);
		if (la.kind == 12) {
			NullableQuestionMark(ref type);
		}
		while (IsPointer()) {
			Expect(6);
			++pointer;
		}
		if (type != null) {
		type.PointerNestingLevel = pointer; 
		type.EndLocation = t.EndLocation;
		type.StartLocation = startPos;
		} 

	}

	void Identifier() {
		switch (la.kind) {
		case 1: {
			Get();
			break;
		}
		case 126: {
			Get();
			break;
		}
		case 127: {
			Get();
			break;
		}
		case 128: {
			Get();
			break;
		}
		case 129: {
			Get();
			break;
		}
		case 130: {
			Get();
			break;
		}
		case 131: {
			Get();
			break;
		}
		case 132: {
			Get();
			break;
		}
		case 133: {
			Get();
			break;
		}
		case 134: {
			Get();
			break;
		}
		case 135: {
			Get();
			break;
		}
		case 136: {
			Get();
			break;
		}
		case 137: {
			Get();
			break;
		}
		case 138: {
			Get();
			break;
		}
		case 139: {
			Get();
			break;
		}
		case 140: {
			Get();
			break;
		}
		case 141: {
			Get();
			break;
		}
		case 142: {
			Get();
			break;
		}
		case 143: {
			Get();
			break;
		}
		case 144: {
			Get();
			break;
		}
		default: SynErr(148); break;
		}
	}

	void Attribute(out ASTAttribute attribute) {
		string qualident;
		string alias = null;

		Location startPos = la.Location;
		if (IdentAndDoubleColon()) {
			Identifier();
			alias = t.val;
			Expect(10);
		}
		Qualident(out qualident);
		List<Expression> positional = new List<Expression>();
		List<NamedArgumentExpression> named = new List<NamedArgumentExpression>();
		string name = (alias != null && alias != "global") ? alias + "." + qualident : qualident;

		if (la.kind == 20) {
			AttributeArguments(positional, named);
		}
		attribute = new ASTAttribute(name, positional, named); 
		attribute.StartLocation = startPos;
		attribute.EndLocation = t.EndLocation;

	}

	void AttributeArguments(List<Expression> positional, List<NamedArgumentExpression> named) {
		Expect(20);
		if (StartOf(6)) {
			AttributeArgument(positional, named);
			while (la.kind == 14) {
				Get();
				AttributeArgument(positional, named);
			}
		}
		Expect(21);
	}

	void AttributeArgument(List<Expression> positional, List<NamedArgumentExpression> named) {
		string name = null; bool isNamed = false; Expression expr;
		if (IsAssignment()) {
			isNamed = true;
			Identifier();
			name = t.val;
			Expect(3);
		} else if (IdentAndColon()) {
			Identifier();
			name = t.val;
			Expect(9);
		} else if (StartOf(6)) {
		} else SynErr(149);
		Expr(out expr);
		if (expr != null) {
		if (isNamed) {
			named.Add(new NamedArgumentExpression(name, expr));
		} else {
			if (named.Count > 0)
				Error("positional argument after named argument is not allowed");
			if (name != null)
				expr = new NamedArgumentExpression(name, expr);
			positional.Add(expr);
		}
		}

	}

	void Expr(out Expression expr) {
		expr = null; Expression expr1 = null, expr2 = null; AssignmentOperatorType op;
		Location startLocation = la.Location;
		UnaryExpr(out expr);
		if (StartOf(7)) {
			AssignmentOperator(out op);
			Expr(out expr1);
			expr = new AssignmentExpression(expr, op, expr1);
		} else if (la.kind == Tokens.GreaterThan && Peek(1).kind == Tokens.GreaterEqual) {
			AssignmentOperator(out op);
			Expr(out expr1);
			expr = new AssignmentExpression(expr, op, expr1);
		} else if (StartOf(8)) {
			ConditionalOrExpr(ref expr);
			if (la.kind == 13) {
				Get();
				Expr(out expr1);
				expr = new BinaryOperatorExpression(expr, BinaryOperatorType.NullCoalescing, expr1);
			}
			if (la.kind == 12) {
				Get();
				Expr(out expr1);
				Expect(9);
				Expr(out expr2);
				expr = new ConditionalExpression(expr, expr1, expr2); 
			}
		} else SynErr(150);
		if (expr != null) {
		if (expr.StartLocation.IsEmpty)
			expr.StartLocation = startLocation;
		if (expr.EndLocation.IsEmpty)
			expr.EndLocation = t.EndLocation;
		}

	}

	void AttributeSection(out AttributeSection section) {
		string attributeTarget = "";
		List<ASTAttribute> attributes = new List<ASTAttribute>();
		ASTAttribute attribute;


		Expect(18);
		Location startPos = t.Location;
		if (IsLocalAttrTarget()) {
			if (la.kind == 69) {
				Get();
				attributeTarget = "event";
			} else if (la.kind == 101) {
				Get();
				attributeTarget = "return";
			} else if (StartOf(9)) {
				Identifier();
				attributeTarget = t.val;
			} else SynErr(151);
			Expect(9);
		}
		Attribute(out attribute);
		attributes.Add(attribute);
		while (NotFinalComma()) {
			Expect(14);
			Attribute(out attribute);
			attributes.Add(attribute);
		}
		if (la.kind == 14) {
			Get();
		}
		Expect(19);
		section = new AttributeSection {
		   AttributeTarget = attributeTarget,
		   Attributes = attributes,
		   StartLocation = startPos,
		   EndLocation = t.EndLocation
		};

	}

	void TypeModifier(ModifierList m) {
		switch (la.kind) {
		case 89: {
			Get();
			m.Add(Modifiers.New, t.Location);
			break;
		}
		case 98: {
			Get();
			m.Add(Modifiers.Public, t.Location);
			break;
		}
		case 97: {
			Get();
			m.Add(Modifiers.Protected, t.Location);
			break;
		}
		case 84: {
			Get();
			m.Add(Modifiers.Internal, t.Location);
			break;
		}
		case 96: {
			Get();
			m.Add(Modifiers.Private, t.Location);
			break;
		}
		case 119: {
			Get();
			m.Add(Modifiers.Unsafe, t.Location);
			break;
		}
		case 49: {
			Get();
			m.Add(Modifiers.Abstract, t.Location);
			break;
		}
		case 103: {
			Get();
			m.Add(Modifiers.Sealed, t.Location);
			break;
		}
		case 107: {
			Get();
			m.Add(Modifiers.Static, t.Location);
			break;
		}
		case 126: {
			Get();
			m.Add(Modifiers.Partial, t.Location);
			break;
		}
		default: SynErr(152); break;
		}
	}

	void TypeDecl(ModifierList m, List<AttributeSection> attributes) {
		TypeReference type;
		List<TypeReference> names;
		List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
		string name;
		List<TemplateDefinition> templates;

		if (la.kind == 59) {
			m.Check(Modifiers.Classes);
			Get();
			TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
			templates = newType.Templates;
			compilationUnit.AddChild(newType);
			compilationUnit.BlockStart(newType);
			newType.StartLocation = m.GetDeclarationLocation(t.Location);

			newType.Type = Types.Class;

			Identifier();
			newType.Name = t.val;
			if (la.kind == 23) {
				TypeParameterList(templates);
			}
			if (la.kind == 9) {
				ClassBase(out names);
				newType.BaseTypes = names;
			}
			while (la.kind == 127) {
				TypeParameterConstraintsClause(templates);
			}
			newType.BodyStartLocation = t.EndLocation;
			Expect(16);
			ClassBody();
			Expect(17);
			if (la.kind == 11) {
				Get();
			}
			newType.EndLocation = t.EndLocation; 
			compilationUnit.BlockEnd();

		} else if (StartOf(10)) {
			m.Check(Modifiers.StructsInterfacesEnumsDelegates);
			if (la.kind == 109) {
				Get();
				TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
				templates = newType.Templates;
				newType.StartLocation = m.GetDeclarationLocation(t.Location);
				compilationUnit.AddChild(newType);
				compilationUnit.BlockStart(newType);
				newType.Type = Types.Struct; 

				Identifier();
				newType.Name = t.val;
				if (la.kind == 23) {
					TypeParameterList(templates);
				}
				if (la.kind == 9) {
					StructInterfaces(out names);
					newType.BaseTypes = names;
				}
				while (la.kind == 127) {
					TypeParameterConstraintsClause(templates);
				}
				newType.BodyStartLocation = t.EndLocation;
				StructBody();
				if (la.kind == 11) {
					Get();
				}
				newType.EndLocation = t.EndLocation; 
				compilationUnit.BlockEnd();

			} else if (la.kind == 83) {
				Get();
				TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
				templates = newType.Templates;
				compilationUnit.AddChild(newType);
				compilationUnit.BlockStart(newType);
				newType.StartLocation = m.GetDeclarationLocation(t.Location);
				newType.Type = Types.Interface;

				Identifier();
				newType.Name = t.val;
				if (la.kind == 23) {
					TypeParameterList(templates);
				}
				if (la.kind == 9) {
					InterfaceBase(out names);
					newType.BaseTypes = names;
				}
				while (la.kind == 127) {
					TypeParameterConstraintsClause(templates);
				}
				newType.BodyStartLocation = t.EndLocation;
				InterfaceBody();
				if (la.kind == 11) {
					Get();
				}
				newType.EndLocation = t.EndLocation; 
				compilationUnit.BlockEnd();

			} else if (la.kind == 68) {
				Get();
				TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
				compilationUnit.AddChild(newType);
				compilationUnit.BlockStart(newType);
				newType.StartLocation = m.GetDeclarationLocation(t.Location);
				newType.Type = Types.Enum;

				Identifier();
				newType.Name = t.val;
				if (la.kind == 9) {
					Get();
					IntegralType(out name);
					newType.BaseTypes.Add(new TypeReference(name, true));
				}
				newType.BodyStartLocation = t.EndLocation;
				EnumBody();
				if (la.kind == 11) {
					Get();
				}
				newType.EndLocation = t.EndLocation; 
				compilationUnit.BlockEnd();

			} else {
				Get();
				DelegateDeclaration delegateDeclr = new DelegateDeclaration(m.Modifier, attributes);
				templates = delegateDeclr.Templates;
				delegateDeclr.StartLocation = m.GetDeclarationLocation(t.Location);

				if (NotVoidPointer()) {
					Expect(123);
					delegateDeclr.ReturnType = new TypeReference("System.Void", true);
				} else if (StartOf(11)) {
					Type(out type);
					delegateDeclr.ReturnType = type;
				} else SynErr(153);
				Identifier();
				delegateDeclr.Name = t.val;
				if (la.kind == 23) {
					TypeParameterList(templates);
				}
				Expect(20);
				if (StartOf(12)) {
					FormalParameterList(p);
					delegateDeclr.Parameters = p;
				}
				Expect(21);
				while (la.kind == 127) {
					TypeParameterConstraintsClause(templates);
				}
				Expect(11);
				delegateDeclr.EndLocation = t.EndLocation;
				compilationUnit.AddChild(delegateDeclr);

			}
		} else SynErr(154);
	}

	void TypeParameterList(List<TemplateDefinition> templates) {
		TemplateDefinition template;

		Expect(23);
		VariantTypeParameter(out template);
		templates.Add(template);
		while (la.kind == 14) {
			Get();
			VariantTypeParameter(out template);
			templates.Add(template);
		}
		Expect(22);
	}

	void ClassBase(out List<TypeReference> names) {
		TypeReference typeRef;
		names = new List<TypeReference>();

		Expect(9);
		ClassType(out typeRef, false);
		if (typeRef != null) { names.Add(typeRef); }
		while (la.kind == 14) {
			Get();
			TypeName(out typeRef, false);
			if (typeRef != null) { names.Add(typeRef); }
		}
	}

	void TypeParameterConstraintsClause(List<TemplateDefinition> templates) {
		string name = ""; TypeReference type;
		Expect(127);
		Identifier();
		name = t.val;
		Expect(9);
		TypeParameterConstraintsClauseBase(out type);
		TemplateDefinition td = null;
		foreach (TemplateDefinition d in templates) {
			if (d.Name == name) {
				td = d;
				break;
			}
		}
		if ( td != null && type != null) { td.Bases.Add(type); }

		while (la.kind == 14) {
			Get();
			TypeParameterConstraintsClauseBase(out type);
			td = null;
			foreach (TemplateDefinition d in templates) {
				if (d.Name == name) {
					td = d;
					break;
				}
			}
			if ( td != null && type != null) { td.Bases.Add(type); }

		}
	}

	void ClassBody() {
		AttributeSection section;
		while (StartOf(13)) {
			List<AttributeSection> attributes = new List<AttributeSection>();
			ModifierList m = new ModifierList();

			while (!(StartOf(14))) {SynErr(155); Get();}
			while (la.kind == 18) {
				AttributeSection(out section);
				attributes.Add(section);
			}
			MemberModifiers(m);
			ClassMemberDecl(m, attributes);
		}
	}

	void StructInterfaces(out List<TypeReference> names) {
		TypeReference typeRef;
		names = new List<TypeReference>();

		Expect(9);
		TypeName(out typeRef, false);
		if (typeRef != null) { names.Add(typeRef); }
		while (la.kind == 14) {
			Get();
			TypeName(out typeRef, false);
			if (typeRef != null) { names.Add(typeRef); }
		}
	}

	void StructBody() {
		AttributeSection section;
		Expect(16);
		while (StartOf(15)) {
			List<AttributeSection> attributes = new List<AttributeSection>();
			ModifierList m = new ModifierList();

			while (la.kind == 18) {
				AttributeSection(out section);
				attributes.Add(section);
			}
			MemberModifiers(m);
			StructMemberDecl(m, attributes);
		}
		Expect(17);
	}

	void InterfaceBase(out List<TypeReference> names) {
		TypeReference typeRef;
		names = new List<TypeReference>();

		Expect(9);
		TypeName(out typeRef, false);
		if (typeRef != null) { names.Add(typeRef); }
		while (la.kind == 14) {
			Get();
			TypeName(out typeRef, false);
			if (typeRef != null) { names.Add(typeRef); }
		}
	}

	void InterfaceBody() {
		Expect(16);
		while (StartOf(16)) {
			while (!(StartOf(17))) {SynErr(156); Get();}
			InterfaceMemberDecl();
		}
		Expect(17);
	}

	void IntegralType(out string name) {
		name = "";
		switch (la.kind) {
		case 102: {
			Get();
			name = "System.SByte";
			break;
		}
		case 54: {
			Get();
			name = "System.Byte";
			break;
		}
		case 104: {
			Get();
			name = "System.Int16";
			break;
		}
		case 120: {
			Get();
			name = "System.UInt16";
			break;
		}
		case 82: {
			Get();
			name = "System.Int32";
			break;
		}
		case 116: {
			Get();
			name = "System.UInt32";
			break;
		}
		case 87: {
			Get();
			name = "System.Int64";
			break;
		}
		case 117: {
			Get();
			name = "System.UInt64";
			break;
		}
		case 57: {
			Get();
			name = "System.Char";
			break;
		}
		default: SynErr(157); break;
		}
	}

	void EnumBody() {
		FieldDeclaration f;
		Expect(16);
		if (StartOf(18)) {
			EnumMemberDecl(out f);
			compilationUnit.AddChild(f);
			while (NotFinalComma()) {
				Expect(14);
				EnumMemberDecl(out f);
				compilationUnit.AddChild(f);
			}
			if (la.kind == 14) {
				Get();
			}
		}
		Expect(17);
	}

	void Type(out TypeReference type) {
		TypeWithRestriction(out type, true, false);
	}

	void FormalParameterList(List<ParameterDeclarationExpression> parameter) {
		ParameterDeclarationExpression p;
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();

		while (la.kind == 18) {
			AttributeSection(out section);
			attributes.Add(section);
		}
		FixedParameter(out p);
		p.Attributes = attributes;
		parameter.Add(p);

		while (la.kind == 14) {
			Get();
			attributes = new List<AttributeSection>();
			while (la.kind == 18) {
				AttributeSection(out section);
				attributes.Add(section);
			}
			FixedParameter(out p);
			p.Attributes = attributes; parameter.Add(p);
		}
	}

	void ClassType(out TypeReference typeRef, bool canBeUnbound) {
		TypeReference r; typeRef = null;
		if (StartOf(9)) {
			TypeName(out r, canBeUnbound);
			typeRef = r;
		} else if (la.kind == 91) {
			Get();
			typeRef = new TypeReference("System.Object", true); typeRef.StartLocation = t.Location;
		} else if (la.kind == 108) {
			Get();
			typeRef = new TypeReference("System.String", true); typeRef.StartLocation = t.Location;
		} else SynErr(158);
	}

	void TypeName(out TypeReference typeRef, bool canBeUnbound) {
		List<TypeReference> typeArguments = null;
		string alias = null;
		string qualident;
		Location startLocation = la.Location;

		if (IdentAndDoubleColon()) {
			Identifier();
			alias = t.val;
			Expect(10);
		}
		Qualident(out qualident);
		if (la.kind == 23) {
			TypeArgumentList(out typeArguments, canBeUnbound);
		}
		if (alias == null) {
		typeRef = new TypeReference(qualident, typeArguments);
		} else if (alias == "global") {
			typeRef = new TypeReference(qualident, typeArguments);
			typeRef.IsGlobal = true;
		} else {
			typeRef = new TypeReference(alias + "." + qualident, typeArguments);
		}

		while (DotAndIdent()) {
			Expect(15);
			typeArguments = null;
			Qualident(out qualident);
			if (la.kind == 23) {
				TypeArgumentList(out typeArguments, canBeUnbound);
			}
			typeRef = new InnerClassTypeReference(typeRef, qualident, typeArguments);
		}
		typeRef.StartLocation = startLocation;
	}

	void MemberModifiers(ModifierList m) {
		while (StartOf(19)) {
			switch (la.kind) {
			case 49: {
				Get();
				m.Add(Modifiers.Abstract, t.Location);
				break;
			}
			case 71: {
				Get();
				m.Add(Modifiers.Extern, t.Location);
				break;
			}
			case 84: {
				Get();
				m.Add(Modifiers.Internal, t.Location);
				break;
			}
			case 89: {
				Get();
				m.Add(Modifiers.New, t.Location);
				break;
			}
			case 94: {
				Get();
				m.Add(Modifiers.Override, t.Location);
				break;
			}
			case 96: {
				Get();
				m.Add(Modifiers.Private, t.Location);
				break;
			}
			case 97: {
				Get();
				m.Add(Modifiers.Protected, t.Location);
				break;
			}
			case 98: {
				Get();
				m.Add(Modifiers.Public, t.Location);
				break;
			}
			case 99: {
				Get();
				m.Add(Modifiers.ReadOnly, t.Location);
				break;
			}
			case 103: {
				Get();
				m.Add(Modifiers.Sealed, t.Location);
				break;
			}
			case 107: {
				Get();
				m.Add(Modifiers.Static, t.Location);
				break;
			}
			case 74: {
				Get();
				m.Add(Modifiers.Fixed, t.Location);
				break;
			}
			case 119: {
				Get();
				m.Add(Modifiers.Unsafe, t.Location);
				break;
			}
			case 122: {
				Get();
				m.Add(Modifiers.Virtual, t.Location);
				break;
			}
			case 124: {
				Get();
				m.Add(Modifiers.Volatile, t.Location);
				break;
			}
			case 126: {
				Get();
				m.Add(Modifiers.Partial, t.Location);
				break;
			}
			}
		}
	}

	void ClassMemberDecl(ModifierList m, List<AttributeSection> attributes) {
		Statement stmt = null;
		if (StartOf(20)) {
			StructMemberDecl(m, attributes);
		} else if (la.kind == 27) {
			m.Check(Modifiers.Destructors); Location startPos = la.Location;
			Get();
			Identifier();
			DestructorDeclaration d = new DestructorDeclaration(t.val, m.Modifier, attributes); 
			d.Modifier = m.Modifier;
			d.StartLocation = m.GetDeclarationLocation(startPos);

			Expect(20);
			Expect(21);
			d.EndLocation = t.EndLocation;
			if (la.kind == 16) {
				Block(out stmt);
			} else if (la.kind == 11) {
				Get();
			} else SynErr(159);
			d.Body = (BlockStatement)stmt;
			compilationUnit.AddChild(d);

		} else SynErr(160);
	}

	void StructMemberDecl(ModifierList m, List<AttributeSection> attributes) {
		string qualident = null;
		TypeReference type;
		Expression expr;
		List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
		Statement stmt = null;
		List<TemplateDefinition> templates = new List<TemplateDefinition>();
		TypeReference explicitInterface = null;
		bool isExtensionMethod = false;

		if (la.kind == 60) {
			m.Check(Modifiers.Constants);
			Get();
			Location startPos = t.Location;
			Type(out type);
			Identifier();
			FieldDeclaration fd = new FieldDeclaration(attributes, type, m.Modifier | Modifiers.Const);
			fd.StartLocation = m.GetDeclarationLocation(startPos);
			VariableDeclaration f = new VariableDeclaration(t.val);
			f.StartLocation = t.Location;
			f.TypeReference = type;
			SafeAdd(fd, fd.Fields, f);

			Expect(3);
			Expr(out expr);
			f.Initializer = expr;
			while (la.kind == 14) {
				Get();
				Identifier();
				f = new VariableDeclaration(t.val);
				f.StartLocation = t.Location;
				f.TypeReference = type;
				SafeAdd(fd, fd.Fields, f);

				Expect(3);
				Expr(out expr);
				f.EndLocation = t.EndLocation; f.Initializer = expr;
			}
			Expect(11);
			fd.EndLocation = t.EndLocation; compilationUnit.AddChild(fd);
		} else if (NotVoidPointer()) {
			m.Check(Modifiers.PropertysEventsMethods);
			Expect(123);
			Location startPos = t.Location;
			if (IsExplicitInterfaceImplementation()) {
				TypeName(out explicitInterface, false);
				if (la.kind != Tokens.Dot || Peek(1).kind != Tokens.This) {
				qualident = TypeReference.StripLastIdentifierFromType(ref explicitInterface);
				 }
			} else if (StartOf(9)) {
				Identifier();
				qualident = t.val;
			} else SynErr(161);
			if (la.kind == 23) {
				TypeParameterList(templates);
			}
			Expect(20);
			if (la.kind == 111) {
				Get();
				isExtensionMethod = true; /* C# 3.0 */
			}
			if (StartOf(12)) {
				FormalParameterList(p);
			}
			Expect(21);
			MethodDeclaration methodDeclaration = new MethodDeclaration {
			Name = qualident,
			Modifier = m.Modifier,
			TypeReference = new TypeReference("System.Void", true),
			Parameters = p,
			Attributes = attributes,
			StartLocation = m.GetDeclarationLocation(startPos),
			EndLocation = t.EndLocation,
			Templates = templates,
			IsExtensionMethod = isExtensionMethod
			};
			if (explicitInterface != null)
				SafeAdd(methodDeclaration, methodDeclaration.InterfaceImplementations, new InterfaceImplementation(explicitInterface, qualident));
			compilationUnit.AddChild(methodDeclaration);
			compilationUnit.BlockStart(methodDeclaration);

			while (la.kind == 127) {
				TypeParameterConstraintsClause(templates);
			}
			if (la.kind == 16) {
				Block(out stmt);
			} else if (la.kind == 11) {
				Get();
			} else SynErr(162);
			compilationUnit.BlockEnd();
			methodDeclaration.Body  = (BlockStatement)stmt;

		} else if (la.kind == 69) {
			m.Check(Modifiers.PropertysEventsMethods);
			Get();
			EventDeclaration eventDecl = new EventDeclaration {
			Modifier = m.Modifier, 
			Attributes = attributes,
			StartLocation = t.Location
			};
			compilationUnit.AddChild(eventDecl);
			compilationUnit.BlockStart(eventDecl);
			EventAddRegion addBlock = null;
			EventRemoveRegion removeBlock = null;

			Type(out type);
			eventDecl.TypeReference = type;
			if (IsExplicitInterfaceImplementation()) {
				TypeName(out explicitInterface, false);
				qualident = TypeReference.StripLastIdentifierFromType(ref explicitInterface);
				eventDecl.InterfaceImplementations.Add(new InterfaceImplementation(explicitInterface, qualident));
			} else if (StartOf(9)) {
				Identifier();
				qualident = t.val;
			} else SynErr(163);
			eventDecl.Name = qualident; eventDecl.EndLocation = t.EndLocation;
			if (la.kind == 3) {
				Get();
				Expr(out expr);
				eventDecl.Initializer = expr;
			}
			if (la.kind == 16) {
				Get();
				eventDecl.BodyStart = t.Location;
				EventAccessorDecls(out addBlock, out removeBlock);
				Expect(17);
				eventDecl.BodyEnd   = t.EndLocation;
			}
			if (la.kind == 11) {
				Get();
			}
			compilationUnit.BlockEnd();
			eventDecl.AddRegion = addBlock;
			eventDecl.RemoveRegion = removeBlock;

		} else if (IdentAndLPar()) {
			m.Check(Modifiers.Constructors | Modifiers.StaticConstructors);
			Identifier();
			string name = t.val; Location startPos = t.Location;
			Expect(20);
			if (StartOf(12)) {
				m.Check(Modifiers.Constructors);
				FormalParameterList(p);
			}
			Expect(21);
			ConstructorInitializer init = null; 
			if (la.kind == 9) {
				m.Check(Modifiers.Constructors);
				ConstructorInitializer(out init);
			}
			ConstructorDeclaration cd = new ConstructorDeclaration(name, m.Modifier, p, init, attributes);
			cd.StartLocation = startPos;
			cd.EndLocation   = t.EndLocation;

			if (la.kind == 16) {
				Block(out stmt);
			} else if (la.kind == 11) {
				Get();
			} else SynErr(164);
			cd.Body = (BlockStatement)stmt; compilationUnit.AddChild(cd);
		} else if (la.kind == 70 || la.kind == 80) {
			m.Check(Modifiers.Operators);
			if (m.isNone) Error("at least one modifier must be set"); 
			bool isImplicit = true;
			Location startPos = Location.Empty;

			if (la.kind == 80) {
				Get();
				startPos = t.Location;
			} else {
				Get();
				isImplicit = false; startPos = t.Location;
			}
			Expect(92);
			Type(out type);
			TypeReference operatorType = type;
			Expect(20);
			Type(out type);
			Identifier();
			string varName = t.val;
			Expect(21);
			Location endPos = t.Location;
			if (la.kind == 16) {
				Block(out stmt);
			} else if (la.kind == 11) {
				Get();
				stmt = null;
			} else SynErr(165);
			List<ParameterDeclarationExpression> parameters = new List<ParameterDeclarationExpression>();
			parameters.Add(new ParameterDeclarationExpression(type, varName));
			OperatorDeclaration operatorDeclaration = new OperatorDeclaration {
				Name = (isImplicit ? "op_Implicit" : "op_Explicit"),
				Modifier = m.Modifier,
				Attributes = attributes, 
				Parameters = parameters, 
				TypeReference = operatorType,
				ConversionType = isImplicit ? ConversionType.Implicit : ConversionType.Explicit,
				Body = (BlockStatement)stmt,
				StartLocation = m.GetDeclarationLocation(startPos),
				EndLocation = endPos
			};
			compilationUnit.AddChild(operatorDeclaration);

		} else if (StartOf(21)) {
			TypeDecl(m, attributes);
		} else if (StartOf(11)) {
			Type(out type);
			Location startPos = t.Location; 
			if (la.kind == 92) {
				OverloadableOperatorType op;
				m.Check(Modifiers.Operators);
				if (m.isNone) Error("at least one modifier must be set");

				Get();
				OverloadableOperator(out op);
				TypeReference firstType, secondType = null; string secondName = null;
				Expect(20);
				Type(out firstType);
				Identifier();
				string firstName = t.val;
				if (la.kind == 14) {
					Get();
					Type(out secondType);
					Identifier();
					secondName = t.val;
				} else if (la.kind == 21) {
				} else SynErr(166);
				Location endPos = t.Location;
				Expect(21);
				if (la.kind == 16) {
					Block(out stmt);
				} else if (la.kind == 11) {
					Get();
				} else SynErr(167);
				if (op == OverloadableOperatorType.Add && secondType == null)
				op = OverloadableOperatorType.UnaryPlus;
				if (op == OverloadableOperatorType.Subtract && secondType == null)
					op = OverloadableOperatorType.UnaryMinus;
				OperatorDeclaration operatorDeclaration = new OperatorDeclaration {
					Modifier = m.Modifier,
					Attributes = attributes,
					TypeReference = type,
					OverloadableOperator = op,
					Name = GetReflectionNameForOperator(op),
					Body = (BlockStatement)stmt,
					StartLocation = m.GetDeclarationLocation(startPos),
					EndLocation = endPos
				};
				SafeAdd(operatorDeclaration, operatorDeclaration.Parameters, new ParameterDeclarationExpression(firstType, firstName));
				if (secondType != null) {
					SafeAdd(operatorDeclaration, operatorDeclaration.Parameters, new ParameterDeclarationExpression(secondType, secondName));
				}
				compilationUnit.AddChild(operatorDeclaration);

			} else if (IsVarDecl()) {
				m.Check(Modifiers.Fields);
				FieldDeclaration fd = new FieldDeclaration(attributes, type, m.Modifier);
				fd.StartLocation = m.GetDeclarationLocation(startPos); 

				if (m.Contains(Modifiers.Fixed)) {
					VariableDeclarator(fd);
					Expect(18);
					Expr(out expr);
					if (fd.Fields.Count > 0)
					fd.Fields[fd.Fields.Count-1].FixedArrayInitialization = expr;
					Expect(19);
					while (la.kind == 14) {
						Get();
						VariableDeclarator(fd);
						Expect(18);
						Expr(out expr);
						if (fd.Fields.Count > 0)
						fd.Fields[fd.Fields.Count-1].FixedArrayInitialization = expr;
						Expect(19);
					}
				} else if (StartOf(9)) {
					VariableDeclarator(fd);
					while (la.kind == 14) {
						Get();
						VariableDeclarator(fd);
					}
				} else SynErr(168);
				Expect(11);
				fd.EndLocation = t.EndLocation; compilationUnit.AddChild(fd);
			} else if (la.kind == 111) {
				m.Check(Modifiers.Indexers);
				Get();
				Expect(18);
				FormalParameterList(p);
				Expect(19);
				Location endLocation = t.EndLocation;
				Expect(16);
				PropertyDeclaration indexer = new PropertyDeclaration(m.Modifier | Modifiers.Default, attributes, "Item", p);
				indexer.StartLocation = startPos;
				indexer.EndLocation   = endLocation;
				indexer.BodyStart     = t.Location;
				indexer.TypeReference = type;
				PropertyGetRegion getRegion;
				PropertySetRegion setRegion;

				AccessorDecls(out getRegion, out setRegion);
				Expect(17);
				indexer.BodyEnd    = t.EndLocation;
				indexer.GetRegion = getRegion;
				indexer.SetRegion = setRegion;
				compilationUnit.AddChild(indexer);

			} else if (IsIdentifierToken(la)) {
				if (IsExplicitInterfaceImplementation()) {
					TypeName(out explicitInterface, false);
					if (la.kind != Tokens.Dot || Peek(1).kind != Tokens.This) {
					qualident = TypeReference.StripLastIdentifierFromType(ref explicitInterface);
					 }
				} else if (StartOf(9)) {
					Identifier();
					qualident = t.val;
				} else SynErr(169);
				Location qualIdentEndLocation = t.EndLocation;
				if (la.kind == 16 || la.kind == 20 || la.kind == 23) {
					if (la.kind == 20 || la.kind == 23) {
						m.Check(Modifiers.PropertysEventsMethods);
						if (la.kind == 23) {
							TypeParameterList(templates);
						}
						Expect(20);
						if (la.kind == 111) {
							Get();
							isExtensionMethod = true;
						}
						if (StartOf(12)) {
							FormalParameterList(p);
						}
						Expect(21);
						MethodDeclaration methodDeclaration = new MethodDeclaration {
						Name = qualident,
						Modifier = m.Modifier,
						TypeReference = type,
						Parameters = p, 
						Attributes = attributes
						};
						if (explicitInterface != null)
							methodDeclaration.InterfaceImplementations.Add(new InterfaceImplementation(explicitInterface, qualident));
						methodDeclaration.StartLocation = m.GetDeclarationLocation(startPos);
						methodDeclaration.EndLocation   = t.EndLocation;
						methodDeclaration.IsExtensionMethod = isExtensionMethod;
						methodDeclaration.Templates = templates;
						compilationUnit.AddChild(methodDeclaration);
						                                     
						while (la.kind == 127) {
							TypeParameterConstraintsClause(templates);
						}
						if (la.kind == 16) {
							Block(out stmt);
						} else if (la.kind == 11) {
							Get();
						} else SynErr(170);
						methodDeclaration.Body  = (BlockStatement)stmt;
					} else {
						Get();
						PropertyDeclaration pDecl = new PropertyDeclaration(qualident, type, m.Modifier, attributes); 
						if (explicitInterface != null)
						pDecl.InterfaceImplementations.Add(new InterfaceImplementation(explicitInterface, qualident));
						      pDecl.StartLocation = m.GetDeclarationLocation(startPos);
						      pDecl.EndLocation   = qualIdentEndLocation;
						      pDecl.BodyStart   = t.Location;
						      PropertyGetRegion getRegion;
						      PropertySetRegion setRegion;
						  
						AccessorDecls(out getRegion, out setRegion);
						Expect(17);
						pDecl.GetRegion = getRegion;
						pDecl.SetRegion = setRegion;
						pDecl.BodyEnd = t.EndLocation;
						compilationUnit.AddChild(pDecl);

					}
				} else if (la.kind == 15) {
					m.Check(Modifiers.Indexers);
					Get();
					Expect(111);
					Expect(18);
					FormalParameterList(p);
					Expect(19);
					PropertyDeclaration indexer = new PropertyDeclaration(m.Modifier | Modifiers.Default, attributes, "Item", p);
					indexer.StartLocation = m.GetDeclarationLocation(startPos);
					indexer.EndLocation   = t.EndLocation;
					indexer.TypeReference = type;
					if (explicitInterface != null)
					SafeAdd(indexer, indexer.InterfaceImplementations, new InterfaceImplementation(explicitInterface, "this"));
					      PropertyGetRegion getRegion;
					      PropertySetRegion setRegion;
					   
					Expect(16);
					Location bodyStart = t.Location;
					AccessorDecls(out getRegion, out setRegion);
					Expect(17);
					indexer.BodyStart = bodyStart;
					indexer.BodyEnd   = t.EndLocation;
					indexer.GetRegion = getRegion;
					indexer.SetRegion = setRegion;
					compilationUnit.AddChild(indexer);

				} else SynErr(171);
			} else SynErr(172);
		} else SynErr(173);
	}

	void InterfaceMemberDecl() {
		TypeReference type;

		AttributeSection section;
		Modifiers mod = Modifiers.None;
		List<AttributeSection> attributes = new List<AttributeSection>();
		List<ParameterDeclarationExpression> parameters = new List<ParameterDeclarationExpression>();
		string name;
		PropertyGetRegion getBlock;
		PropertySetRegion setBlock;
		Location startLocation = new Location(-1, -1);
		List<TemplateDefinition> templates = new List<TemplateDefinition>();

		while (la.kind == 18) {
			AttributeSection(out section);
			attributes.Add(section);
		}
		if (la.kind == 89) {
			Get();
			mod = Modifiers.New; startLocation = t.Location;
		}
		if (NotVoidPointer()) {
			Expect(123);
			if (startLocation.IsEmpty) startLocation = t.Location;
			Identifier();
			name = t.val;
			if (la.kind == 23) {
				TypeParameterList(templates);
			}
			Expect(20);
			if (StartOf(12)) {
				FormalParameterList(parameters);
			}
			Expect(21);
			while (la.kind == 127) {
				TypeParameterConstraintsClause(templates);
			}
			Expect(11);
			MethodDeclaration md = new MethodDeclaration {
			Name = name, Modifier = mod, TypeReference = new TypeReference("System.Void", true), 
			Parameters = parameters, Attributes = attributes, Templates = templates,
			StartLocation = startLocation, EndLocation = t.EndLocation
			};
			compilationUnit.AddChild(md);

		} else if (StartOf(22)) {
			if (StartOf(11)) {
				Type(out type);
				if (startLocation.IsEmpty) startLocation = t.Location;
				if (StartOf(9)) {
					Identifier();
					name = t.val; Location qualIdentEndLocation = t.EndLocation;
					if (la.kind == 20 || la.kind == 23) {
						if (la.kind == 23) {
							TypeParameterList(templates);
						}
						Expect(20);
						if (StartOf(12)) {
							FormalParameterList(parameters);
						}
						Expect(21);
						while (la.kind == 127) {
							TypeParameterConstraintsClause(templates);
						}
						Expect(11);
						MethodDeclaration md = new MethodDeclaration {
						Name = name, Modifier = mod, TypeReference = type,
						Parameters = parameters, Attributes = attributes, Templates = templates,
						StartLocation = startLocation, EndLocation = t.EndLocation
						};
						compilationUnit.AddChild(md);

					} else if (la.kind == 16) {
						PropertyDeclaration pd = new PropertyDeclaration(name, type, mod, attributes);
						compilationUnit.AddChild(pd);
						Get();
						Location bodyStart = t.Location;
						InterfaceAccessors(out getBlock, out setBlock);
						Expect(17);
						pd.GetRegion = getBlock; pd.SetRegion = setBlock; pd.StartLocation = startLocation; pd.EndLocation = qualIdentEndLocation; pd.BodyStart = bodyStart; pd.BodyEnd = t.EndLocation;
					} else SynErr(174);
				} else if (la.kind == 111) {
					Get();
					Expect(18);
					FormalParameterList(parameters);
					Expect(19);
					Location bracketEndLocation = t.EndLocation;
					PropertyDeclaration id = new PropertyDeclaration(mod | Modifiers.Default, attributes, "Item", parameters);
					id.TypeReference = type;
					  compilationUnit.AddChild(id);
					Expect(16);
					Location bodyStart = t.Location;
					InterfaceAccessors(out getBlock, out setBlock);
					Expect(17);
					id.GetRegion = getBlock; id.SetRegion = setBlock; id.StartLocation = startLocation;  id.EndLocation = bracketEndLocation; id.BodyStart = bodyStart; id.BodyEnd = t.EndLocation;
				} else SynErr(175);
			} else {
				Get();
				if (startLocation.IsEmpty) startLocation = t.Location;
				Type(out type);
				Identifier();
				EventDeclaration ed = new EventDeclaration {
				TypeReference = type, Name = t.val, Modifier = mod, Attributes = attributes
				};
				compilationUnit.AddChild(ed);

				Expect(11);
				ed.StartLocation = startLocation; ed.EndLocation = t.EndLocation;
			}
		} else SynErr(176);
	}

	void EnumMemberDecl(out FieldDeclaration f) {
		Expression expr = null;
		List<AttributeSection> attributes = new List<AttributeSection>();
		AttributeSection section = null;
		VariableDeclaration varDecl = null;

		while (la.kind == 18) {
			AttributeSection(out section);
			attributes.Add(section);
		}
		Identifier();
		f = new FieldDeclaration(attributes);
		varDecl         = new VariableDeclaration(t.val);
		f.Fields.Add(varDecl);
		f.StartLocation = t.Location;
		f.EndLocation = t.EndLocation;

		if (la.kind == 3) {
			Get();
			Expr(out expr);
			varDecl.Initializer = expr;
		}
	}

	void TypeWithRestriction(out TypeReference type, bool allowNullable, bool canBeUnbound) {
		Location startPos = la.Location;
		string name;
		int pointer = 0;
		type = null;

		if (StartOf(4)) {
			ClassType(out type, canBeUnbound);
		} else if (StartOf(5)) {
			SimpleType(out name);
			type = new TypeReference(name, true);
		} else if (la.kind == 123) {
			Get();
			Expect(6);
			pointer = 1; type = new TypeReference("System.Void", true);
		} else SynErr(177);
		List<int> r = new List<int>();
		if (allowNullable && la.kind == Tokens.Question) {
			NullableQuestionMark(ref type);
		}
		while (IsPointerOrDims()) {
			int i = 0;
			if (la.kind == 6) {
				Get();
				++pointer;
			} else if (la.kind == 18) {
				Get();
				while (la.kind == 14) {
					Get();
					++i;
				}
				Expect(19);
				r.Add(i);
			} else SynErr(178);
		}
		if (type != null) {
		type.RankSpecifier = r.ToArray();
		type.PointerNestingLevel = pointer;
		type.EndLocation = t.EndLocation;
		type.StartLocation = startPos;
		}

	}

	void SimpleType(out string name) {
		name = String.Empty;
		if (StartOf(23)) {
			IntegralType(out name);
		} else if (la.kind == 75) {
			Get();
			name = "System.Single";
		} else if (la.kind == 66) {
			Get();
			name = "System.Double";
		} else if (la.kind == 62) {
			Get();
			name = "System.Decimal";
		} else if (la.kind == 52) {
			Get();
			name = "System.Boolean";
		} else SynErr(179);
	}

	void NullableQuestionMark(ref TypeReference typeRef) {
		List<TypeReference> typeArguments = new List<TypeReference>(1);
		Expect(12);
		if (typeRef != null) typeArguments.Add(typeRef);
		typeRef = new TypeReference("System.Nullable", typeArguments) { IsKeyword = true };

	}

	void FixedParameter(out ParameterDeclarationExpression p) {
		TypeReference type;
		ParameterModifiers mod = ParameterModifiers.In;
		Location start = la.Location;
		Expression expr;

		if (la.kind == 93 || la.kind == 95 || la.kind == 100) {
			if (la.kind == 100) {
				Get();
				mod = ParameterModifiers.Ref;
			} else if (la.kind == 93) {
				Get();
				mod = ParameterModifiers.Out;
			} else {
				Get();
				mod = ParameterModifiers.Params;
			}
		}
		Type(out type);
		Identifier();
		p = new ParameterDeclarationExpression(type, t.val, mod);
		if (la.kind == 3) {
			Get();
			Expr(out expr);
			p.DefaultValue = expr; p.ParamModifier |= ParameterModifiers.Optional;
		}
		p.StartLocation = start; p.EndLocation = t.EndLocation;
	}

	void AccessorModifiers(out ModifierList m) {
		m = new ModifierList();
		if (la.kind == 96) {
			Get();
			m.Add(Modifiers.Private, t.Location);
		} else if (la.kind == 97) {
			Get();
			m.Add(Modifiers.Protected, t.Location);
			if (la.kind == 84) {
				Get();
				m.Add(Modifiers.Internal, t.Location);
			}
		} else if (la.kind == 84) {
			Get();
			m.Add(Modifiers.Internal, t.Location);
			if (la.kind == 97) {
				Get();
				m.Add(Modifiers.Protected, t.Location);
			}
		} else SynErr(180);
	}

	void Block(out Statement stmt) {
		Expect(16);
		BlockStatement blockStmt = new BlockStatement();
		blockStmt.StartLocation = t.Location;
		compilationUnit.BlockStart(blockStmt);
		if (!ParseMethodBodies) lexer.SkipCurrentBlock(0);

		while (StartOf(24)) {
			Statement();
		}
		while (!(la.kind == 0 || la.kind == 17)) {SynErr(181); Get();}
		Expect(17);
		stmt = blockStmt;
		blockStmt.EndLocation = t.EndLocation;
		compilationUnit.BlockEnd();

	}

	void EventAccessorDecls(out EventAddRegion addBlock, out EventRemoveRegion removeBlock) {
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		Statement stmt;
		addBlock = null;
		removeBlock = null;

		while (la.kind == 18) {
			AttributeSection(out section);
			attributes.Add(section);
		}
		if (la.kind == 130) {
			addBlock = new EventAddRegion(attributes);
			AddAccessorDecl(out stmt);
			attributes = new List<AttributeSection>(); addBlock.Block = (BlockStatement)stmt;
			while (la.kind == 18) {
				AttributeSection(out section);
				attributes.Add(section);
			}
			RemoveAccessorDecl(out stmt);
			removeBlock = new EventRemoveRegion(attributes); removeBlock.Block = (BlockStatement)stmt;
		} else if (la.kind == 131) {
			RemoveAccessorDecl(out stmt);
			removeBlock = new EventRemoveRegion(attributes); removeBlock.Block = (BlockStatement)stmt; attributes = new List<AttributeSection>();
			while (la.kind == 18) {
				AttributeSection(out section);
				attributes.Add(section);
			}
			AddAccessorDecl(out stmt);
			addBlock = new EventAddRegion(attributes); addBlock.Block = (BlockStatement)stmt;
		} else SynErr(182);
	}

	void ConstructorInitializer(out ConstructorInitializer ci) {
		Expression expr; ci = new ConstructorInitializer();
		Expect(9);
		if (la.kind == 51) {
			Get();
			ci.ConstructorInitializerType = ConstructorInitializerType.Base;
		} else if (la.kind == 111) {
			Get();
			ci.ConstructorInitializerType = ConstructorInitializerType.This;
		} else SynErr(183);
		Expect(20);
		if (StartOf(25)) {
			Argument(out expr);
			SafeAdd(ci, ci.Arguments, expr);
			while (la.kind == 14) {
				Get();
				Argument(out expr);
				SafeAdd(ci, ci.Arguments, expr);
			}
		}
		Expect(21);
	}

	void OverloadableOperator(out OverloadableOperatorType op) {
		op = OverloadableOperatorType.None;
		switch (la.kind) {
		case 4: {
			Get();
			op = OverloadableOperatorType.Add;
			break;
		}
		case 5: {
			Get();
			op = OverloadableOperatorType.Subtract;
			break;
		}
		case 24: {
			Get();
			op = OverloadableOperatorType.Not;
			break;
		}
		case 27: {
			Get();
			op = OverloadableOperatorType.BitNot;
			break;
		}
		case 31: {
			Get();
			op = OverloadableOperatorType.Increment;
			break;
		}
		case 32: {
			Get();
			op = OverloadableOperatorType.Decrement;
			break;
		}
		case 113: {
			Get();
			op = OverloadableOperatorType.IsTrue;
			break;
		}
		case 72: {
			Get();
			op = OverloadableOperatorType.IsFalse;
			break;
		}
		case 6: {
			Get();
			op = OverloadableOperatorType.Multiply;
			break;
		}
		case 7: {
			Get();
			op = OverloadableOperatorType.Divide;
			break;
		}
		case 8: {
			Get();
			op = OverloadableOperatorType.Modulus;
			break;
		}
		case 28: {
			Get();
			op = OverloadableOperatorType.BitwiseAnd;
			break;
		}
		case 29: {
			Get();
			op = OverloadableOperatorType.BitwiseOr;
			break;
		}
		case 30: {
			Get();
			op = OverloadableOperatorType.ExclusiveOr;
			break;
		}
		case 37: {
			Get();
			op = OverloadableOperatorType.ShiftLeft;
			break;
		}
		case 33: {
			Get();
			op = OverloadableOperatorType.Equality;
			break;
		}
		case 34: {
			Get();
			op = OverloadableOperatorType.InEquality;
			break;
		}
		case 23: {
			Get();
			op = OverloadableOperatorType.LessThan;
			break;
		}
		case 35: {
			Get();
			op = OverloadableOperatorType.GreaterThanOrEqual;
			break;
		}
		case 36: {
			Get();
			op = OverloadableOperatorType.LessThanOrEqual;
			break;
		}
		case 22: {
			Get();
			op = OverloadableOperatorType.GreaterThan;
			if (la.kind == 22) {
				Get();
				op = OverloadableOperatorType.ShiftRight;
			}
			break;
		}
		default: SynErr(184); break;
		}
	}

	void VariableDeclarator(FieldDeclaration parentFieldDeclaration) {
		Expression expr = null;
		Identifier();
		VariableDeclaration f = new VariableDeclaration(t.val); f.StartLocation = t.Location;
		if (la.kind == 3) {
			Get();
			VariableInitializer(out expr);
			f.Initializer = expr;
		}
		f.EndLocation = t.EndLocation; SafeAdd(parentFieldDeclaration, parentFieldDeclaration.Fields, f);
	}

	void AccessorDecls(out PropertyGetRegion getBlock, out PropertySetRegion setBlock) {
		List<AttributeSection> attributes = new List<AttributeSection>(); 
		AttributeSection section;
		getBlock = null;
		setBlock = null; 
		ModifierList modifiers = null;

		while (la.kind == 18) {
			AttributeSection(out section);
			attributes.Add(section);
		}
		if (la.kind == 84 || la.kind == 96 || la.kind == 97) {
			AccessorModifiers(out modifiers);
		}
		if (la.kind == 128) {
			GetAccessorDecl(out getBlock, attributes);
			if (modifiers != null) {getBlock.Modifier = modifiers.Modifier; }
			if (StartOf(26)) {
				attributes = new List<AttributeSection>(); modifiers = null;
				while (la.kind == 18) {
					AttributeSection(out section);
					attributes.Add(section);
				}
				if (la.kind == 84 || la.kind == 96 || la.kind == 97) {
					AccessorModifiers(out modifiers);
				}
				SetAccessorDecl(out setBlock, attributes);
				if (modifiers != null) {setBlock.Modifier = modifiers.Modifier; }
			}
		} else if (la.kind == 129) {
			SetAccessorDecl(out setBlock, attributes);
			if (modifiers != null) {setBlock.Modifier = modifiers.Modifier; }
			if (StartOf(27)) {
				attributes = new List<AttributeSection>(); modifiers = null;
				while (la.kind == 18) {
					AttributeSection(out section);
					attributes.Add(section);
				}
				if (la.kind == 84 || la.kind == 96 || la.kind == 97) {
					AccessorModifiers(out modifiers);
				}
				GetAccessorDecl(out getBlock, attributes);
				if (modifiers != null) {getBlock.Modifier = modifiers.Modifier; }
			}
		} else if (StartOf(9)) {
			Identifier();
			Error("get or set accessor declaration expected");
		} else SynErr(185);
	}

	void InterfaceAccessors(out PropertyGetRegion getBlock, out PropertySetRegion setBlock) {
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		getBlock = null; setBlock = null;
		PropertyGetSetRegion lastBlock = null;

		while (la.kind == 18) {
			AttributeSection(out section);
			attributes.Add(section);
		}
		Location startLocation = la.Location;
		if (la.kind == 128) {
			Get();
			getBlock = new PropertyGetRegion(null, attributes);
		} else if (la.kind == 129) {
			Get();
			setBlock = new PropertySetRegion(null, attributes);
		} else SynErr(186);
		Expect(11);
		if (getBlock != null) { getBlock.StartLocation = startLocation; getBlock.EndLocation = t.EndLocation; }
		if (setBlock != null) { setBlock.StartLocation = startLocation; setBlock.EndLocation = t.EndLocation; }
		attributes = new List<AttributeSection>();
		if (la.kind == 18 || la.kind == 128 || la.kind == 129) {
			while (la.kind == 18) {
				AttributeSection(out section);
				attributes.Add(section);
			}
			startLocation = la.Location;
			if (la.kind == 128) {
				Get();
				if (getBlock != null) Error("get already declared");
				                 else { getBlock = new PropertyGetRegion(null, attributes); lastBlock = getBlock; }
				             
			} else if (la.kind == 129) {
				Get();
				if (setBlock != null) Error("set already declared");
				                 else { setBlock = new PropertySetRegion(null, attributes); lastBlock = setBlock; }
				             
			} else SynErr(187);
			Expect(11);
			if (lastBlock != null) { lastBlock.StartLocation = startLocation; lastBlock.EndLocation = t.EndLocation; }
		}
	}

	void GetAccessorDecl(out PropertyGetRegion getBlock, List<AttributeSection> attributes) {
		Statement stmt = null;
		Expect(128);
		Location startLocation = t.Location;
		if (la.kind == 16) {
			Block(out stmt);
		} else if (la.kind == 11) {
			Get();
		} else SynErr(188);
		getBlock = new PropertyGetRegion((BlockStatement)stmt, attributes);
		getBlock.StartLocation = startLocation; getBlock.EndLocation = t.EndLocation;
	}

	void SetAccessorDecl(out PropertySetRegion setBlock, List<AttributeSection> attributes) {
		Statement stmt = null;
		Expect(129);
		Location startLocation = t.Location;
		if (la.kind == 16) {
			Block(out stmt);
		} else if (la.kind == 11) {
			Get();
		} else SynErr(189);
		setBlock = new PropertySetRegion((BlockStatement)stmt, attributes);
		setBlock.StartLocation = startLocation; setBlock.EndLocation = t.EndLocation;
	}

	void AddAccessorDecl(out Statement stmt) {
		stmt = null;
		Expect(130);
		Block(out stmt);
	}

	void RemoveAccessorDecl(out Statement stmt) {
		stmt = null;
		Expect(131);
		Block(out stmt);
	}

	void VariableInitializer(out Expression initializerExpression) {
		TypeReference type = null; Expression expr = null; initializerExpression = null;
		if (StartOf(6)) {
			Expr(out initializerExpression);
		} else if (la.kind == 16) {
			CollectionInitializer(out initializerExpression);
		} else if (la.kind == 106) {
			Get();
			Type(out type);
			Expect(18);
			Expr(out expr);
			Expect(19);
			initializerExpression = new StackAllocExpression(type, expr);
		} else SynErr(190);
	}

	void Statement() {
		Statement stmt = null;
		Location startPos = la.Location;

		while (!(StartOf(28))) {SynErr(191); Get();}
		if (IsLabel()) {
			Identifier();
			compilationUnit.AddChild(new LabelStatement(t.val));
			Expect(9);
			Statement();
		} else if (la.kind == 60) {
			Get();
			LocalVariableDecl(out stmt);
			if (stmt != null) { ((LocalVariableDeclaration)stmt).Modifier |= Modifiers.Const; }
			Expect(11);
			compilationUnit.AddChild(stmt);
		} else if (IsLocalVarDecl()) {
			LocalVariableDecl(out stmt);
			Expect(11);
			compilationUnit.AddChild(stmt);
		} else if (StartOf(29)) {
			EmbeddedStatement(out stmt);
			compilationUnit.AddChild(stmt);
		} else SynErr(192);
		if (stmt != null) {
		stmt.StartLocation = startPos;
		stmt.EndLocation = t.EndLocation;
		}

	}

	void Argument(out Expression argumentexpr) {
		argumentexpr = null;
		if (IdentAndColon()) {
			Token ident; Expression expr;
			Identifier();
			ident = t;
			Expect(9);
			ArgumentValue(out expr);
			argumentexpr = new NamedArgumentExpression(ident.val, expr) { StartLocation = ident.Location, EndLocation = t.EndLocation };
		} else if (StartOf(25)) {
			ArgumentValue(out argumentexpr);
		} else SynErr(193);
	}

	void CollectionInitializer(out Expression outExpr) {
		Expression expr = null;
		CollectionInitializerExpression initializer = new CollectionInitializerExpression();

		Expect(16);
		initializer.StartLocation = t.Location;
		if (StartOf(30)) {
			VariableInitializer(out expr);
			SafeAdd(initializer, initializer.CreateExpressions, expr);
			while (NotFinalComma()) {
				Expect(14);
				VariableInitializer(out expr);
				SafeAdd(initializer, initializer.CreateExpressions, expr);
			}
			if (la.kind == 14) {
				Get();
			}
		}
		Expect(17);
		initializer.EndLocation = t.Location; outExpr = initializer;
	}

	void ArgumentValue(out Expression argumentexpr) {
		Expression expr;
		FieldDirection fd = FieldDirection.None;

		if (la.kind == 93 || la.kind == 100) {
			if (la.kind == 100) {
				Get();
				fd = FieldDirection.Ref;
			} else {
				Get();
				fd = FieldDirection.Out;
			}
		}
		Expr(out expr);
		argumentexpr = fd != FieldDirection.None ? argumentexpr = new DirectionExpression(fd, expr) : expr;
	}

	void AssignmentOperator(out AssignmentOperatorType op) {
		op = AssignmentOperatorType.None;
		if (la.kind == 3) {
			Get();
			op = AssignmentOperatorType.Assign;
		} else if (la.kind == 38) {
			Get();
			op = AssignmentOperatorType.Add;
		} else if (la.kind == 39) {
			Get();
			op = AssignmentOperatorType.Subtract;
		} else if (la.kind == 40) {
			Get();
			op = AssignmentOperatorType.Multiply;
		} else if (la.kind == 41) {
			Get();
			op = AssignmentOperatorType.Divide;
		} else if (la.kind == 42) {
			Get();
			op = AssignmentOperatorType.Modulus;
		} else if (la.kind == 43) {
			Get();
			op = AssignmentOperatorType.BitwiseAnd;
		} else if (la.kind == 44) {
			Get();
			op = AssignmentOperatorType.BitwiseOr;
		} else if (la.kind == 45) {
			Get();
			op = AssignmentOperatorType.ExclusiveOr;
		} else if (la.kind == 46) {
			Get();
			op = AssignmentOperatorType.ShiftLeft;
		} else if (la.kind == Tokens.GreaterThan && Peek(1).kind == Tokens.GreaterEqual) {
			Expect(22);
			Expect(35);
			op = AssignmentOperatorType.ShiftRight;
		} else SynErr(194);
	}

	void CollectionOrObjectInitializer(out Expression outExpr) {
		Expression expr = null;
		CollectionInitializerExpression initializer = new CollectionInitializerExpression();

		Expect(16);
		initializer.StartLocation = t.Location;
		if (StartOf(30)) {
			ObjectPropertyInitializerOrVariableInitializer(out expr);
			SafeAdd(initializer, initializer.CreateExpressions, expr);
			while (NotFinalComma()) {
				Expect(14);
				ObjectPropertyInitializerOrVariableInitializer(out expr);
				SafeAdd(initializer, initializer.CreateExpressions, expr);
			}
			if (la.kind == 14) {
				Get();
			}
		}
		Expect(17);
		initializer.EndLocation = t.Location; outExpr = initializer;
	}

	void ObjectPropertyInitializerOrVariableInitializer(out Expression expr) {
		expr = null;
		if (IdentAndAsgn()) {
			Identifier();
			MemberInitializerExpression mie = new MemberInitializerExpression(t.val, null);
			mie.StartLocation = t.Location;
			mie.IsKey = true;
			Expression r = null;
			Expect(3);
			if (la.kind == 16) {
				CollectionOrObjectInitializer(out r);
			} else if (StartOf(30)) {
				VariableInitializer(out r);
			} else SynErr(195);
			mie.Expression = r; mie.EndLocation = t.EndLocation; expr = mie;
		} else if (StartOf(30)) {
			VariableInitializer(out expr);
		} else SynErr(196);
	}

	void LocalVariableDecl(out Statement stmt) {
		TypeReference type;
		VariableDeclaration      var = null;
		LocalVariableDeclaration localVariableDeclaration; 
		Location startPos = la.Location;

		Type(out type);
		localVariableDeclaration = new LocalVariableDeclaration(type); localVariableDeclaration.StartLocation = startPos;
		LocalVariableDeclarator(out var);
		SafeAdd(localVariableDeclaration, localVariableDeclaration.Variables, var);
		while (la.kind == 14) {
			Get();
			LocalVariableDeclarator(out var);
			SafeAdd(localVariableDeclaration, localVariableDeclaration.Variables, var);
		}
		stmt = localVariableDeclaration; stmt.EndLocation = t.EndLocation;
	}

	void LocalVariableDeclarator(out VariableDeclaration var) {
		Expression expr = null;
		Identifier();
		var = new VariableDeclaration(t.val); var.StartLocation = t.Location;
		if (la.kind == 3) {
			Get();
			VariableInitializer(out expr);
			var.Initializer = expr;
		}
		var.EndLocation = t.EndLocation;
	}

	void EmbeddedStatement(out Statement statement) {
		TypeReference type = null;
		Expression expr = null;
		Statement embeddedStatement = null;
		statement = null;

		Location startLocation = la.Location;
		if (la.kind == 16) {
			Block(out statement);
		} else if (la.kind == 11) {
			Get();
			statement = new EmptyStatement();
		} else if (UnCheckedAndLBrace()) {
			Statement block; bool isChecked = true;
			if (la.kind == 58) {
				Get();
			} else if (la.kind == 118) {
				Get();
				isChecked = false;
			} else SynErr(197);
			Block(out block);
			statement = isChecked ? (Statement)new CheckedStatement(block) : (Statement)new UncheckedStatement(block);
		} else if (la.kind == 79) {
			IfStatement(out statement);
		} else if (la.kind == 110) {
			Get();
			List<SwitchSection> switchSections = new List<SwitchSection>();
			Expect(20);
			Expr(out expr);
			Expect(21);
			Expect(16);
			SwitchSections(switchSections);
			Expect(17);
			statement = new SwitchStatement(expr, switchSections);
		} else if (la.kind == 125) {
			Get();
			Expect(20);
			Expr(out expr);
			Expect(21);
			EmbeddedStatement(out embeddedStatement);
			statement = new DoLoopStatement(expr, embeddedStatement, ConditionType.While, ConditionPosition.Start);
		} else if (la.kind == 65) {
			Get();
			EmbeddedStatement(out embeddedStatement);
			Expect(125);
			Expect(20);
			Expr(out expr);
			Expect(21);
			Expect(11);
			statement = new DoLoopStatement(expr, embeddedStatement, ConditionType.While, ConditionPosition.End);
		} else if (la.kind == 76) {
			Get();
			List<Statement> initializer = null; List<Statement> iterator = null;
			Expect(20);
			if (StartOf(6)) {
				ForInitializer(out initializer);
			}
			Expect(11);
			if (StartOf(6)) {
				Expr(out expr);
			}
			Expect(11);
			if (StartOf(6)) {
				ForIterator(out iterator);
			}
			Expect(21);
			EmbeddedStatement(out embeddedStatement);
			statement = new ForStatement(initializer, expr, iterator, embeddedStatement);
		} else if (la.kind == 77) {
			Get();
			Expect(20);
			Type(out type);
			Identifier();
			string varName = t.val;
			Expect(81);
			Expr(out expr);
			Expect(21);
			EmbeddedStatement(out embeddedStatement);
			statement = new ForeachStatement(type, varName , expr, embeddedStatement);
		} else if (la.kind == 53) {
			Get();
			Expect(11);
			statement = new BreakStatement();
		} else if (la.kind == 61) {
			Get();
			Expect(11);
			statement = new ContinueStatement();
		} else if (la.kind == 78) {
			GotoStatement(out statement);
		} else if (IsYieldStatement()) {
			Expect(132);
			if (la.kind == 101) {
				Get();
				Expr(out expr);
				statement = new YieldStatement(new ReturnStatement(expr));
			} else if (la.kind == 53) {
				Get();
				statement = new YieldStatement(new BreakStatement());
			} else SynErr(198);
			Expect(11);
		} else if (la.kind == 101) {
			Get();
			if (StartOf(6)) {
				Expr(out expr);
			}
			Expect(11);
			statement = new ReturnStatement(expr);
		} else if (la.kind == 112) {
			Get();
			if (StartOf(6)) {
				Expr(out expr);
			}
			Expect(11);
			statement = new ThrowStatement(expr);
		} else if (StartOf(6)) {
			StatementExpr(out statement);
			while (!(la.kind == 0 || la.kind == 11)) {SynErr(199); Get();}
			Expect(11);
		} else if (la.kind == 114) {
			TryStatement(out statement);
		} else if (la.kind == 86) {
			Get();
			Expect(20);
			Expr(out expr);
			Expect(21);
			EmbeddedStatement(out embeddedStatement);
			statement = new LockStatement(expr, embeddedStatement);
		} else if (la.kind == 121) {
			Statement resourceAcquisitionStmt = null;
			Get();
			Expect(20);
			ResourceAcquisition(out resourceAcquisitionStmt);
			Expect(21);
			EmbeddedStatement(out embeddedStatement);
			statement = new UsingStatement(resourceAcquisitionStmt, embeddedStatement);
		} else if (la.kind == 119) {
			Get();
			Block(out embeddedStatement);
			statement = new UnsafeStatement(embeddedStatement);
		} else if (la.kind == 74) {
			Statement pointerDeclarationStmt = null;
			Get();
			Expect(20);
			ResourceAcquisition(out pointerDeclarationStmt);
			Expect(21);
			EmbeddedStatement(out embeddedStatement);
			statement = new FixedStatement(pointerDeclarationStmt, embeddedStatement);
		} else SynErr(200);
		if (statement != null) {
		statement.StartLocation = startLocation;
		statement.EndLocation = t.EndLocation;
		}

	}

	void IfStatement(out Statement statement) {
		Expression expr = null;
		Statement embeddedStatement = null;
		statement = null;

		Expect(79);
		Expect(20);
		Expr(out expr);
		Expect(21);
		EmbeddedStatement(out embeddedStatement);
		Statement elseStatement = null;
		if (la.kind == 67) {
			Get();
			EmbeddedStatement(out elseStatement);
		}
		statement = elseStatement != null ? new IfElseStatement(expr, embeddedStatement, elseStatement) : new IfElseStatement(expr, embeddedStatement);
		if (elseStatement is IfElseStatement && (elseStatement as IfElseStatement).TrueStatement.Count == 1) {
		/* else if-section (otherwise we would have a BlockStatment) */
		(statement as IfElseStatement).ElseIfSections.Add(
		             new ElseIfSection((elseStatement as IfElseStatement).Condition,
		                               (elseStatement as IfElseStatement).TrueStatement[0]));
		(statement as IfElseStatement).ElseIfSections.AddRange((elseStatement as IfElseStatement).ElseIfSections);
		(statement as IfElseStatement).FalseStatement = (elseStatement as IfElseStatement).FalseStatement;
		}

	}

	void SwitchSections(List<SwitchSection> switchSections) {
		SwitchSection switchSection = new SwitchSection();
		CaseLabel label;

		SwitchLabel(out label);
		SafeAdd(switchSection, switchSection.SwitchLabels, label);
		compilationUnit.BlockStart(switchSection);
		while (StartOf(31)) {
			if (la.kind == 55 || la.kind == 63) {
				SwitchLabel(out label);
				if (label != null) {
				if (switchSection.Children.Count > 0) {
					// open new section
					compilationUnit.BlockEnd(); switchSections.Add(switchSection);
					switchSection = new SwitchSection();
					compilationUnit.BlockStart(switchSection);
				}
				SafeAdd(switchSection, switchSection.SwitchLabels, label);
				}

			} else {
				Statement();
			}
		}
		compilationUnit.BlockEnd(); switchSections.Add(switchSection);
	}

	void ForInitializer(out List<Statement> initializer) {
		Statement stmt; 
		initializer = new List<Statement>();

		if (IsLocalVarDecl()) {
			LocalVariableDecl(out stmt);
			initializer.Add(stmt);
		} else if (StartOf(6)) {
			StatementExpr(out stmt);
			initializer.Add(stmt);
			while (la.kind == 14) {
				Get();
				StatementExpr(out stmt);
				initializer.Add(stmt);
			}
		} else SynErr(201);
	}

	void ForIterator(out List<Statement> iterator) {
		Statement stmt; 
		iterator = new List<Statement>();

		StatementExpr(out stmt);
		iterator.Add(stmt);
		while (la.kind == 14) {
			Get();
			StatementExpr(out stmt);
			iterator.Add(stmt);
		}
	}

	void GotoStatement(out Statement stmt) {
		Expression expr; stmt = null;
		Expect(78);
		if (StartOf(9)) {
			Identifier();
			stmt = new GotoStatement(t.val);
			Expect(11);
		} else if (la.kind == 55) {
			Get();
			Expr(out expr);
			Expect(11);
			stmt = new GotoCaseStatement(expr);
		} else if (la.kind == 63) {
			Get();
			Expect(11);
			stmt = new GotoCaseStatement(null);
		} else SynErr(202);
	}

	void StatementExpr(out Statement stmt) {
		Expression expr;
		Expr(out expr);
		stmt = new ExpressionStatement(expr);
	}

	void TryStatement(out Statement tryStatement) {
		Statement blockStmt = null, finallyStmt = null;
		CatchClause catchClause = null;
		List<CatchClause> catchClauses = new List<CatchClause>();

		Expect(114);
		Block(out blockStmt);
		while (la.kind == 56) {
			CatchClause(out catchClause);
			if (catchClause != null) catchClauses.Add(catchClause);
		}
		if (la.kind == 73) {
			Get();
			Block(out finallyStmt);
		}
		tryStatement = new TryCatchStatement(blockStmt, catchClauses, finallyStmt);
		if (catchClauses != null) {
			foreach (CatchClause cc in catchClauses) cc.Parent = tryStatement;
		}

	}

	void ResourceAcquisition(out Statement stmt) {
		stmt = null;
		Expression expr;

		if (IsLocalVarDecl()) {
			LocalVariableDecl(out stmt);
		} else if (StartOf(6)) {
			Expr(out expr);
			stmt = new ExpressionStatement(expr);
		} else SynErr(203);
	}

	void SwitchLabel(out CaseLabel label) {
		Expression expr = null; label = null;
		if (la.kind == 55) {
			Get();
			Expr(out expr);
			Expect(9);
			label =  new CaseLabel(expr);
		} else if (la.kind == 63) {
			Get();
			Expect(9);
			label =  new CaseLabel();
		} else SynErr(204);
	}

	void CatchClause(out CatchClause catchClause) {
		Expect(56);
		string identifier;
		Statement stmt;
		TypeReference typeRef;
		Location startPos = t.Location;
		catchClause = null;

		if (la.kind == 16) {
			Block(out stmt);
			catchClause = new CatchClause(stmt); 
		} else if (la.kind == 20) {
			Get();
			ClassType(out typeRef, false);
			identifier = null;
			if (StartOf(9)) {
				Identifier();
				identifier = t.val;
			}
			Expect(21);
			Block(out stmt);
			catchClause = new CatchClause(typeRef, identifier, stmt);
		} else SynErr(205);
		if (catchClause != null) {
		catchClause.StartLocation = startPos;
		catchClause.EndLocation = t.Location;
		}

	}

	void UnaryExpr(out Expression uExpr) {
		TypeReference type = null;
		Expression expr = null;
		ArrayList expressions = new ArrayList();
		uExpr = null;

		while (StartOf(32)) {
			if (la.kind == 4) {
				Get();
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.Plus));
			} else if (la.kind == 5) {
				Get();
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.Minus));
			} else if (la.kind == 24) {
				Get();
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.Not));
			} else if (la.kind == 27) {
				Get();
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.BitNot));
			} else if (la.kind == 6) {
				Get();
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.Dereference));
			} else if (la.kind == 31) {
				Get();
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.Increment));
			} else if (la.kind == 32) {
				Get();
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.Decrement));
			} else if (la.kind == 28) {
				Get();
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.AddressOf));
			} else {
				Expect(20);
				Type(out type);
				Expect(21);
				expressions.Add(new CastExpression(type));
			}
		}
		if (LastExpressionIsUnaryMinus(expressions) && IsMostNegativeIntegerWithoutTypeSuffix()) {
			Expect(2);
			expressions.RemoveAt(expressions.Count - 1);
			if (t.literalValue is uint) {
				expr = new PrimitiveExpression(int.MinValue, int.MinValue.ToString());
			} else if (t.literalValue is ulong) {
				expr = new PrimitiveExpression(long.MinValue, long.MinValue.ToString());
			} else {
				throw new Exception("t.literalValue must be uint or ulong");
			}

		} else if (StartOf(33)) {
			PrimaryExpr(out expr);
		} else SynErr(206);
		for (int i = 0; i < expressions.Count; ++i) {
		Expression nextExpression = i + 1 < expressions.Count ? (Expression)expressions[i + 1] : expr;
		if (expressions[i] is CastExpression) {
			((CastExpression)expressions[i]).Expression = nextExpression;
		} else {
			((UnaryOperatorExpression)expressions[i]).Expression = nextExpression;
		}
		}
		if (expressions.Count > 0) {
			uExpr = (Expression)expressions[0];
		} else {
			uExpr = expr;
		}

	}

	void ConditionalOrExpr(ref Expression outExpr) {
		Expression expr;  
		ConditionalAndExpr(ref outExpr);
		while (la.kind == 26) {
			Get();
			UnaryExpr(out expr);
			ConditionalAndExpr(ref expr);
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.LogicalOr, expr); 
		}
	}

	void PrimaryExpr(out Expression pexpr) {
		TypeReference type = null;
		Expression expr;
		pexpr = null;

		Location startLocation = la.Location;
		if (la.kind == 113) {
			Get();
			pexpr = new PrimitiveExpression(true, "true"); 
		} else if (la.kind == 72) {
			Get();
			pexpr = new PrimitiveExpression(false, "false");
		} else if (la.kind == 90) {
			Get();
			pexpr = new PrimitiveExpression(null, "null"); 
		} else if (la.kind == 2) {
			Get();
			pexpr = new PrimitiveExpression(t.literalValue, t.val) { LiteralFormat = t.literalFormat }; 
		} else if (StartOfQueryExpression()) {
			QueryExpression(out pexpr);
		} else if (IdentAndDoubleColon()) {
			Identifier();
			type = new TypeReference(t.val);
			Expect(10);
			pexpr = new TypeReferenceExpression(type);
			Identifier();
			if (type.Type == "global") { type.IsGlobal = true; type.Type = t.val ?? "?"; } else type.Type += "." + (t.val ?? "?");
		} else if (StartOf(9)) {
			Identifier();
			pexpr = new IdentifierExpression(t.val);
			if (la.kind == 23 || la.kind == 48) {
				if (la.kind == 48) {
					ShortedLambdaExpression((IdentifierExpression)pexpr, out pexpr);
				} else {
					List<TypeReference> typeList;
					TypeArgumentList(out typeList, false);
					((IdentifierExpression)pexpr).TypeArguments = typeList;
				}
			}
		} else if (IsLambdaExpression()) {
			LambdaExpression(out pexpr);
		} else if (la.kind == 20) {
			Get();
			Expr(out expr);
			Expect(21);
			pexpr = new ParenthesizedExpression(expr);
		} else if (StartOf(34)) {
			string val = null;
			switch (la.kind) {
			case 52: {
				Get();
				val = "System.Boolean";
				break;
			}
			case 54: {
				Get();
				val = "System.Byte";
				break;
			}
			case 57: {
				Get();
				val = "System.Char";
				break;
			}
			case 62: {
				Get();
				val = "System.Decimal";
				break;
			}
			case 66: {
				Get();
				val = "System.Double";
				break;
			}
			case 75: {
				Get();
				val = "System.Single";
				break;
			}
			case 82: {
				Get();
				val = "System.Int32";
				break;
			}
			case 87: {
				Get();
				val = "System.Int64";
				break;
			}
			case 91: {
				Get();
				val = "System.Object";
				break;
			}
			case 102: {
				Get();
				val = "System.SByte";
				break;
			}
			case 104: {
				Get();
				val = "System.Int16";
				break;
			}
			case 108: {
				Get();
				val = "System.String";
				break;
			}
			case 116: {
				Get();
				val = "System.UInt32";
				break;
			}
			case 117: {
				Get();
				val = "System.UInt64";
				break;
			}
			case 120: {
				Get();
				val = "System.UInt16";
				break;
			}
			case 123: {
				Get();
				val = "System.Void";
				break;
			}
			}
			pexpr = new TypeReferenceExpression(new TypeReference(val, true)) { StartLocation = t.Location, EndLocation = t.EndLocation };
		} else if (la.kind == 111) {
			Get();
			pexpr = new ThisReferenceExpression(); pexpr.StartLocation = t.Location; pexpr.EndLocation = t.EndLocation;
		} else if (la.kind == 51) {
			Get();
			pexpr = new BaseReferenceExpression(); pexpr.StartLocation = t.Location; pexpr.EndLocation = t.EndLocation;
		} else if (la.kind == 89) {
			NewExpression(out pexpr);
		} else if (la.kind == 115) {
			Get();
			Expect(20);
			if (NotVoidPointer()) {
				Expect(123);
				type = new TypeReference("System.Void", true);
			} else if (StartOf(11)) {
				TypeWithRestriction(out type, true, true);
			} else SynErr(207);
			Expect(21);
			pexpr = new TypeOfExpression(type);
		} else if (la.kind == 63) {
			Get();
			Expect(20);
			Type(out type);
			Expect(21);
			pexpr = new DefaultValueExpression(type);
		} else if (la.kind == 105) {
			Get();
			Expect(20);
			Type(out type);
			Expect(21);
			pexpr = new SizeOfExpression(type);
		} else if (la.kind == 58) {
			Get();
			Expect(20);
			Expr(out expr);
			Expect(21);
			pexpr = new CheckedExpression(expr);
		} else if (la.kind == 118) {
			Get();
			Expect(20);
			Expr(out expr);
			Expect(21);
			pexpr = new UncheckedExpression(expr);
		} else if (la.kind == 64) {
			Get();
			AnonymousMethodExpr(out expr);
			pexpr = expr;
		} else SynErr(208);
		if (pexpr != null) {
		if (pexpr.StartLocation.IsEmpty)
			pexpr.StartLocation = startLocation;
		if (pexpr.EndLocation.IsEmpty)
			pexpr.EndLocation = t.EndLocation;
		}

		while (StartOf(35)) {
			startLocation = la.Location;
			switch (la.kind) {
			case 31: {
				Get();
				pexpr = new UnaryOperatorExpression(pexpr, UnaryOperatorType.PostIncrement);
				break;
			}
			case 32: {
				Get();
				pexpr = new UnaryOperatorExpression(pexpr, UnaryOperatorType.PostDecrement);
				break;
			}
			case 47: {
				PointerMemberAccess(out pexpr, pexpr);
				break;
			}
			case 15: {
				MemberAccess(out pexpr, pexpr);
				break;
			}
			case 20: {
				Get();
				List<Expression> parameters = new List<Expression>();
				pexpr = new InvocationExpression(pexpr, parameters);
				if (StartOf(25)) {
					Argument(out expr);
					SafeAdd(pexpr, parameters, expr);
					while (la.kind == 14) {
						Get();
						Argument(out expr);
						SafeAdd(pexpr, parameters, expr);
					}
				}
				Expect(21);
				break;
			}
			case 18: {
				List<Expression> indices = new List<Expression>();
				pexpr = new IndexerExpression(pexpr, indices);

				Get();
				Expr(out expr);
				SafeAdd(pexpr, indices, expr);
				while (la.kind == 14) {
					Get();
					Expr(out expr);
					SafeAdd(pexpr, indices, expr);
				}
				Expect(19);
				break;
			}
			}
			if (pexpr != null) {
			if (pexpr.StartLocation.IsEmpty)
				pexpr.StartLocation = startLocation;
			if (pexpr.EndLocation.IsEmpty)
				pexpr.EndLocation = t.EndLocation;
			}

		}
	}

	void QueryExpression(out Expression outExpr) {
		QueryExpression q = new QueryExpression(); outExpr = q; q.StartLocation = la.Location; 
		QueryExpressionFromClause fromClause;

		QueryExpressionFromClause(out fromClause);
		q.FromClause = fromClause;
		QueryExpressionBody(ref q);
		q.EndLocation = t.EndLocation; 
		outExpr = q; /* set outExpr to q again if QueryExpressionBody changed it (can happen with 'into' clauses) */ 

	}

	void ShortedLambdaExpression(IdentifierExpression ident, out Expression pexpr) {
		LambdaExpression lambda = new LambdaExpression(); pexpr = lambda;
		Expect(48);
		lambda.StartLocation = ident.StartLocation;
		SafeAdd(lambda, lambda.Parameters, new ParameterDeclarationExpression(null, ident.Identifier));
		lambda.Parameters[0].StartLocation = ident.StartLocation;
		lambda.Parameters[0].EndLocation = ident.EndLocation;

		LambdaExpressionBody(lambda);
	}

	void TypeArgumentList(out List<TypeReference> types, bool canBeUnbound) {
		types = new List<TypeReference>();
		TypeReference type = null;

		Expect(23);
		if (canBeUnbound && (la.kind == Tokens.GreaterThan || la.kind == Tokens.Comma)) {
			types.Add(TypeReference.Null);
			while (la.kind == 14) {
				Get();
				types.Add(TypeReference.Null);
			}
		} else if (StartOf(11)) {
			Type(out type);
			if (type != null) { types.Add(type); }
			while (la.kind == 14) {
				Get();
				Type(out type);
				if (type != null) { types.Add(type); }
			}
		} else SynErr(209);
		Expect(22);
	}

	void LambdaExpression(out Expression outExpr) {
		LambdaExpression lambda = new LambdaExpression();
		lambda.StartLocation = la.Location;
		ParameterDeclarationExpression p;
		outExpr = lambda;

		Expect(20);
		if (StartOf(36)) {
			LambdaExpressionParameter(out p);
			SafeAdd(lambda, lambda.Parameters, p);
			while (la.kind == 14) {
				Get();
				LambdaExpressionParameter(out p);
				SafeAdd(lambda, lambda.Parameters, p);
			}
		}
		Expect(21);
		Expect(48);
		LambdaExpressionBody(lambda);
	}

	void NewExpression(out Expression pexpr) {
		pexpr = null;
		List<Expression> parameters = new List<Expression>();
		TypeReference type = null;
		Expression expr;

		Expect(89);
		if (StartOf(11)) {
			NonArrayType(out type);
		}
		if (la.kind == 16 || la.kind == 20) {
			if (la.kind == 20) {
				ObjectCreateExpression oce = new ObjectCreateExpression(type, parameters);
				Get();
				if (type == null) Error("Cannot use an anonymous type with arguments for the constructor");
				if (StartOf(25)) {
					Argument(out expr);
					SafeAdd(oce, parameters, expr);
					while (la.kind == 14) {
						Get();
						Argument(out expr);
						SafeAdd(oce, parameters, expr);
					}
				}
				Expect(21);
				pexpr = oce;
				if (la.kind == 16) {
					CollectionOrObjectInitializer(out expr);
					oce.ObjectInitializer = (CollectionInitializerExpression)expr;
				}
			} else {
				ObjectCreateExpression oce = new ObjectCreateExpression(type, parameters);
				CollectionOrObjectInitializer(out expr);
				oce.ObjectInitializer = (CollectionInitializerExpression)expr;
				pexpr = oce;
			}
		} else if (la.kind == 18) {
			Get();
			ArrayCreateExpression ace = new ArrayCreateExpression(type);
			/* we must not change RankSpecifier on the null type reference*/
			if (ace.CreateType.IsNull) { ace.CreateType = new TypeReference(""); }
			pexpr = ace;
			int dims = 0; List<int> ranks = new List<int>();

			if (la.kind == 14 || la.kind == 19) {
				while (la.kind == 14) {
					Get();
					dims += 1;
				}
				Expect(19);
				ranks.Add(dims); dims = 0;
				while (la.kind == 18) {
					Get();
					while (la.kind == 14) {
						Get();
						++dims;
					}
					Expect(19);
					ranks.Add(dims); dims = 0;
				}
				ace.CreateType.RankSpecifier = ranks.ToArray();
				CollectionInitializer(out expr);
				ace.ArrayInitializer = (CollectionInitializerExpression)expr;
			} else if (StartOf(6)) {
				Expr(out expr);
				if (expr != null) parameters.Add(expr);
				while (la.kind == 14) {
					Get();
					dims += 1;
					Expr(out expr);
					if (expr != null) parameters.Add(expr);
				}
				Expect(19);
				ranks.Add(dims); ace.Arguments = parameters; dims = 0;
				while (la.kind == 18) {
					Get();
					while (la.kind == 14) {
						Get();
						++dims;
					}
					Expect(19);
					ranks.Add(dims); dims = 0;
				}
				ace.CreateType.RankSpecifier = ranks.ToArray();
				if (la.kind == 16) {
					CollectionInitializer(out expr);
					ace.ArrayInitializer = (CollectionInitializerExpression)expr;
				}
			} else SynErr(210);
		} else SynErr(211);
	}

	void AnonymousMethodExpr(out Expression outExpr) {
		AnonymousMethodExpression expr = new AnonymousMethodExpression();
		expr.StartLocation = t.Location;
		BlockStatement stmt;
		List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
		outExpr = expr;

		if (la.kind == 20) {
			Get();
			if (StartOf(12)) {
				FormalParameterList(p);
				expr.Parameters = p;
			}
			Expect(21);
			expr.HasParameterList = true;
		}
		BlockInsideExpression(out stmt);
		expr.Body  = stmt;
		expr.EndLocation = t.Location;
	}

	void PointerMemberAccess(out Expression expr, Expression target) {
		List<TypeReference> typeList;
		Expect(47);
		Identifier();
		expr = new PointerReferenceExpression(target, t.val); expr.StartLocation = t.Location; expr.EndLocation = t.EndLocation;
		if (IsGenericInSimpleNameOrMemberAccess()) {
			TypeArgumentList(out typeList, false);
			((MemberReferenceExpression)expr).TypeArguments = typeList;
		}
	}

	void MemberAccess(out Expression expr, Expression target) {
		List<TypeReference> typeList;
		if (ShouldConvertTargetExpressionToTypeReference(target)) {
		TypeReference type = GetTypeReferenceFromExpression(target);
		if (type != null) {
			target = new TypeReferenceExpression(type) { StartLocation = t.Location, EndLocation = t.EndLocation };
		}
		}

		Expect(15);
		Location startLocation = t.Location;
		Identifier();
		expr = new MemberReferenceExpression(target, t.val); expr.StartLocation = startLocation; expr.EndLocation = t.EndLocation;
		if (IsGenericInSimpleNameOrMemberAccess()) {
			TypeArgumentList(out typeList, false);
			((MemberReferenceExpression)expr).TypeArguments = typeList;
		}
	}

	void LambdaExpressionParameter(out ParameterDeclarationExpression p) {
		Location start = la.Location; p = null;
		TypeReference type;
		ParameterModifiers mod = ParameterModifiers.In;

		if (Peek(1).kind == Tokens.Comma || Peek(1).kind == Tokens.CloseParenthesis) {
			Identifier();
			p = new ParameterDeclarationExpression(null, t.val);
			p.StartLocation = start; p.EndLocation = t.EndLocation;

		} else if (StartOf(36)) {
			if (la.kind == 93 || la.kind == 100) {
				if (la.kind == 100) {
					Get();
					mod = ParameterModifiers.Ref;
				} else {
					Get();
					mod = ParameterModifiers.Out;
				}
			}
			Type(out type);
			Identifier();
			p = new ParameterDeclarationExpression(type, t.val, mod);
			p.StartLocation = start; p.EndLocation = t.EndLocation;

		} else SynErr(212);
	}

	void LambdaExpressionBody(LambdaExpression lambda) {
		Expression expr; BlockStatement stmt;
		if (la.kind == 16) {
			BlockInsideExpression(out stmt);
			lambda.StatementBody = stmt;
		} else if (StartOf(6)) {
			Expr(out expr);
			lambda.ExpressionBody = expr;
		} else SynErr(213);
		lambda.EndLocation = t.EndLocation;
		lambda.ExtendedEndLocation = la.Location;
	}

	void BlockInsideExpression(out BlockStatement outStmt) {
		Statement stmt = null; outStmt = null;
		if (compilationUnit != null) {
		Block(out stmt);
		outStmt = (BlockStatement)stmt;
		} else {
		Expect(16);
		lexer.SkipCurrentBlock(0);
		Expect(17);
		}
	}

	void ConditionalAndExpr(ref Expression outExpr) {
		Expression expr;
		InclusiveOrExpr(ref outExpr);
		while (la.kind == 25) {
			Get();
			UnaryExpr(out expr);
			InclusiveOrExpr(ref expr);
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.LogicalAnd, expr); 
		}
	}

	void InclusiveOrExpr(ref Expression outExpr) {
		Expression expr;
		ExclusiveOrExpr(ref outExpr);
		while (la.kind == 29) {
			Get();
			UnaryExpr(out expr);
			ExclusiveOrExpr(ref expr);
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.BitwiseOr, expr); 
		}
	}

	void ExclusiveOrExpr(ref Expression outExpr) {
		Expression expr;
		AndExpr(ref outExpr);
		while (la.kind == 30) {
			Get();
			UnaryExpr(out expr);
			AndExpr(ref expr);
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.ExclusiveOr, expr); 
		}
	}

	void AndExpr(ref Expression outExpr) {
		Expression expr;
		EqualityExpr(ref outExpr);
		while (la.kind == 28) {
			Get();
			UnaryExpr(out expr);
			EqualityExpr(ref expr);
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.BitwiseAnd, expr); 
		}
	}

	void EqualityExpr(ref Expression outExpr) {
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;

		RelationalExpr(ref outExpr);
		while (la.kind == 33 || la.kind == 34) {
			if (la.kind == 34) {
				Get();
				op = BinaryOperatorType.InEquality;
			} else {
				Get();
				op = BinaryOperatorType.Equality;
			}
			UnaryExpr(out expr);
			RelationalExpr(ref expr);
			outExpr = new BinaryOperatorExpression(outExpr, op, expr); 
		}
	}

	void RelationalExpr(ref Expression outExpr) {
		TypeReference type;
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;

		ShiftExpr(ref outExpr);
		while (StartOf(37)) {
			if (StartOf(38)) {
				if (la.kind == 23) {
					Get();
					op = BinaryOperatorType.LessThan;
				} else if (la.kind == 22) {
					Get();
					op = BinaryOperatorType.GreaterThan;
				} else if (la.kind == 36) {
					Get();
					op = BinaryOperatorType.LessThanOrEqual;
				} else {
					Get();
					op = BinaryOperatorType.GreaterThanOrEqual;
				}
				UnaryExpr(out expr);
				ShiftExpr(ref expr);
				outExpr = new BinaryOperatorExpression(outExpr, op, expr);
			} else {
				if (la.kind == 85) {
					Get();
					TypeWithRestriction(out type, false, false);
					if (la.kind == Tokens.Question && !IsPossibleExpressionStart(Peek(1).kind)) {
						NullableQuestionMark(ref type);
					}
					outExpr = new TypeOfIsExpression(outExpr, type);
				} else {
					Get();
					TypeWithRestriction(out type, false, false);
					if (la.kind == Tokens.Question && !IsPossibleExpressionStart(Peek(1).kind)) {
						NullableQuestionMark(ref type);
					}
					outExpr = new CastExpression(type, outExpr, CastType.TryCast);
				}
			}
		}
	}

	void ShiftExpr(ref Expression outExpr) {
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;

		AdditiveExpr(ref outExpr);
		while (la.kind == 22 || la.kind == 37) {
			if (la.kind == 37) {
				Get();
				op = BinaryOperatorType.ShiftLeft;
			} else {
				Expect(22);
				Expect(22);
				op = BinaryOperatorType.ShiftRight;
			}
			UnaryExpr(out expr);
			AdditiveExpr(ref expr);
			outExpr = new BinaryOperatorExpression(outExpr, op, expr); 
		}
	}

	void AdditiveExpr(ref Expression outExpr) {
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;

		MultiplicativeExpr(ref outExpr);
		while (la.kind == 4 || la.kind == 5) {
			if (la.kind == 4) {
				Get();
				op = BinaryOperatorType.Add;
			} else {
				Get();
				op = BinaryOperatorType.Subtract;
			}
			UnaryExpr(out expr);
			MultiplicativeExpr(ref expr);
			outExpr = new BinaryOperatorExpression(outExpr, op, expr); 
		}
	}

	void MultiplicativeExpr(ref Expression outExpr) {
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;

		while (la.kind == 6 || la.kind == 7 || la.kind == 8) {
			if (la.kind == 6) {
				Get();
				op = BinaryOperatorType.Multiply;
			} else if (la.kind == 7) {
				Get();
				op = BinaryOperatorType.Divide;
			} else {
				Get();
				op = BinaryOperatorType.Modulus;
			}
			UnaryExpr(out expr);
			outExpr = new BinaryOperatorExpression(outExpr, op, expr);
		}
	}

	void VariantTypeParameter(out TemplateDefinition typeParameter) {
		typeParameter = new TemplateDefinition();
		AttributeSection section;

		while (la.kind == 18) {
			AttributeSection(out section);
			typeParameter.Attributes.Add(section);
		}
		if (la.kind == 81 || la.kind == 93) {
			if (la.kind == 81) {
				Get();
				typeParameter.VarianceModifier = VarianceModifier.Contravariant;
			} else {
				Get();
				typeParameter.VarianceModifier = VarianceModifier.Covariant;
			}
		}
		Identifier();
		typeParameter.Name = t.val; typeParameter.StartLocation = t.Location;
		typeParameter.EndLocation = t.EndLocation;
	}

	void TypeParameterConstraintsClauseBase(out TypeReference type) {
		TypeReference t; type = null;
		if (la.kind == 109) {
			Get();
			type = TypeReference.StructConstraint;
		} else if (la.kind == 59) {
			Get();
			type = TypeReference.ClassConstraint;
		} else if (la.kind == 89) {
			Get();
			Expect(20);
			Expect(21);
			type = TypeReference.NewConstraint;
		} else if (StartOf(11)) {
			Type(out t);
			type = t;
		} else SynErr(214);
	}

	void QueryExpressionFromClause(out QueryExpressionFromClause fc) {
		fc = new QueryExpressionFromClause();
		fc.StartLocation = la.Location;
		CollectionRangeVariable variable;

		Expect(137);
		QueryExpressionFromOrJoinClause(out variable);
		fc.EndLocation = t.EndLocation;
		fc.Sources.Add(variable);

	}

	void QueryExpressionBody(ref QueryExpression q) {
		QueryExpressionFromClause fromClause;     QueryExpressionWhereClause whereClause;
		QueryExpressionLetClause letClause;       QueryExpressionJoinClause joinClause;
		QueryExpressionOrderClause orderClause;
		QueryExpressionSelectClause selectClause; QueryExpressionGroupClause groupClause;

		while (StartOf(39)) {
			if (la.kind == 137) {
				QueryExpressionFromClause(out fromClause);
				SafeAdd<QueryExpressionClause>(q, q.MiddleClauses, fromClause);
			} else if (la.kind == 127) {
				QueryExpressionWhereClause(out whereClause);
				SafeAdd<QueryExpressionClause>(q, q.MiddleClauses, whereClause);
			} else if (la.kind == 141) {
				QueryExpressionLetClause(out letClause);
				SafeAdd<QueryExpressionClause>(q, q.MiddleClauses, letClause);
			} else if (la.kind == 142) {
				QueryExpressionJoinClause(out joinClause);
				SafeAdd<QueryExpressionClause>(q, q.MiddleClauses, joinClause);
			} else {
				QueryExpressionOrderByClause(out orderClause);
				SafeAdd<QueryExpressionClause>(q, q.MiddleClauses, orderClause);
			}
		}
		if (la.kind == 133) {
			QueryExpressionSelectClause(out selectClause);
			q.SelectOrGroupClause = selectClause;
		} else if (la.kind == 134) {
			QueryExpressionGroupClause(out groupClause);
			q.SelectOrGroupClause = groupClause;
		} else SynErr(215);
		if (la.kind == 136) {
			QueryExpressionIntoClause(ref q);
		}
	}

	void QueryExpressionFromOrJoinClause(out CollectionRangeVariable variable) {
		TypeReference type; Expression expr; variable = new CollectionRangeVariable();
		variable.Type = null;
		if (IsLocalVarDecl()) {
			Type(out type);
			variable.Type = type;
		}
		Identifier();
		variable.Identifier = t.val;
		Expect(81);
		Expr(out expr);
		variable.Expression = expr;
	}

	void QueryExpressionJoinClause(out QueryExpressionJoinClause jc) {
		jc = new QueryExpressionJoinClause(); jc.StartLocation = la.Location; 
		Expression expr;
		CollectionRangeVariable variable;

		Expect(142);
		QueryExpressionFromOrJoinClause(out variable);
		Expect(143);
		Expr(out expr);
		jc.OnExpression = expr;
		Expect(144);
		Expr(out expr);
		jc.EqualsExpression = expr;
		if (la.kind == 136) {
			Get();
			Identifier();
			jc.IntoIdentifier = t.val;
		}
		jc.EndLocation = t.EndLocation;
		jc.Source = variable;

	}

	void QueryExpressionWhereClause(out QueryExpressionWhereClause wc) {
		Expression expr; wc = new QueryExpressionWhereClause(); wc.StartLocation = la.Location;
		Expect(127);
		Expr(out expr);
		wc.Condition = expr;
		wc.EndLocation = t.EndLocation;
	}

	void QueryExpressionLetClause(out QueryExpressionLetClause wc) {
		Expression expr; wc = new QueryExpressionLetClause(); wc.StartLocation = la.Location;
		Expect(141);
		Identifier();
		wc.Identifier = t.val;
		Expect(3);
		Expr(out expr);
		wc.Expression = expr;
		wc.EndLocation = t.EndLocation;
	}

	void QueryExpressionOrderByClause(out QueryExpressionOrderClause oc) {
		QueryExpressionOrdering ordering; oc = new QueryExpressionOrderClause(); oc.StartLocation = la.Location;
		Expect(140);
		QueryExpressionOrdering(out ordering);
		SafeAdd(oc, oc.Orderings, ordering);
		while (la.kind == 14) {
			Get();
			QueryExpressionOrdering(out ordering);
			SafeAdd(oc, oc.Orderings, ordering);
		}
		oc.EndLocation = t.EndLocation;
	}

	void QueryExpressionSelectClause(out QueryExpressionSelectClause sc) {
		Expression expr; sc = new QueryExpressionSelectClause(); sc.StartLocation = la.Location;
		Expect(133);
		Expr(out expr);
		sc.Projection = expr;
		sc.EndLocation = t.EndLocation;
	}

	void QueryExpressionGroupClause(out QueryExpressionGroupClause gc) {
		Expression expr; gc = new QueryExpressionGroupClause(); gc.StartLocation = la.Location;
		Expect(134);
		Expr(out expr);
		gc.Projection = expr;
		Expect(135);
		Expr(out expr);
		gc.GroupBy = expr;
		gc.EndLocation = t.EndLocation;
	}

	void QueryExpressionIntoClause(ref QueryExpression q) {
		QueryExpression firstQuery = q;
		QueryExpression continuedQuery = new QueryExpression(); 
		continuedQuery.StartLocation = q.StartLocation;
		firstQuery.EndLocation = la.Location;
		continuedQuery.FromClause = new QueryExpressionFromClause();
		CollectionRangeVariable fromVariable = new CollectionRangeVariable();
		continuedQuery.FromClause.Sources.Add(fromVariable);
		fromVariable.StartLocation = la.Location;
		// nest firstQuery inside continuedQuery.
		fromVariable.Expression = firstQuery;
		continuedQuery.IsQueryContinuation = true;
		q = continuedQuery;

		Expect(136);
		Identifier();
		fromVariable.Identifier = t.val;
		continuedQuery.FromClause.EndLocation = t.EndLocation;
		QueryExpressionBody(ref q);
	}

	void QueryExpressionOrdering(out QueryExpressionOrdering ordering) {
		Expression expr; ordering = new QueryExpressionOrdering(); ordering.StartLocation = la.Location;
		Expr(out expr);
		ordering.Criteria = expr;
		if (la.kind == 138 || la.kind == 139) {
			if (la.kind == 138) {
				Get();
				ordering.Direction = QueryExpressionOrderingDirection.Ascending;
			} else {
				Get();
				ordering.Direction = QueryExpressionOrderingDirection.Descending;
			}
		}
		ordering.EndLocation = t.EndLocation;
	}


	
	void ParseRoot()
	{
		CS();
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
		{T,T,T,x, T,T,T,x, x,x,x,T, x,x,x,x, T,T,T,x, T,x,x,x, T,x,x,T, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, T,T,T,x, x,T,T,T, T,T,T,T, T,T,T,x, T,T,T,T, T,x,T,T, T,T,T,T, T,x,T,T, T,x,T,T, x,T,T,T, x,x,T,x, T,T,T,T, x,T,T,T, T,T,x,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, T,T,x,x, x,x,x,x, T,T,T,x, x,x,x,T, x,x,x,T, x,T,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,T,x,x, x,x,x,x, T,T,T,x, x,x,x,T, x,x,x,T, x,T,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, T,T,T,x, x,x,x,T, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
		{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,x, x,T,x,x, x,x,T,x, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
		{x,T,T,x, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, T,x,x,T, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,T,x, x,T,T,x, x,x,T,T, T,x,T,x, x,x,x,x, T,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,T,T,T, x,x,x,x, x,x,x,x, x,x,T,x, T,T,x,x, T,x,x,T, x,T,x,T, T,T,T,x, T,x,x,T, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
		{x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
		{x,T,x,T, T,T,T,T, T,T,x,T, T,T,T,T, T,T,T,T, T,T,T,T, x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, x,T,T,x, T,x,T,x, x,T,x,T, T,x,T,x, T,x,T,x, T,T,T,T, x,x,T,T, x,x,x,x, T,x,T,T, T,T,x,T, x,T,x,T, x,x,T,x, T,T,T,T, x,x,T,T, T,x,x,T, T,T,x,x, x,x,x,x, T,T,x,T, T,x,T,T, T,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
		{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
		{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,x, x,T,x,x, x,x,T,x, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,T, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,x, T,x,x,x, x,x,x,x, T,T,x,x, T,x,x,T, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
		{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,x, x,T,x,x, x,x,T,x, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,T, x,T,x,T, x,x,x,x, T,x,T,x, T,x,x,x, T,x,x,x, x,x,x,x, T,T,x,x, T,x,x,T, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
		{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, T,x,T,x, x,T,x,T, T,x,T,x, T,x,T,x, T,T,T,T, x,x,T,T, x,x,x,x, T,x,T,T, T,x,x,T, x,T,x,T, x,x,T,x, T,T,T,T, x,x,T,T, T,x,x,T, T,T,x,x, x,x,x,x, T,T,x,T, T,x,T,T, T,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
		{T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, T,x,T,x, x,T,x,T, T,x,T,x, T,x,T,x, T,T,T,T, x,x,T,T, x,x,x,x, T,x,T,T, T,x,x,T, x,T,x,T, x,x,T,x, T,T,T,T, x,x,T,T, T,x,x,T, T,T,x,x, x,x,x,x, T,T,x,T, T,x,T,T, T,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
		{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, T,x,T,x, x,T,x,T, T,x,T,x, T,x,T,x, T,T,T,T, x,x,T,T, x,x,x,x, T,x,T,T, T,x,x,T, x,T,x,T, x,x,T,x, T,T,T,T, x,x,T,T, T,x,x,T, T,T,x,x, x,x,x,x, T,T,x,T, T,x,T,T, T,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
		{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,x, x,T,x,x, x,x,T,x, x,x,T,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,T,x,T, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,x, T,x,x,x, x,x,x,x, T,T,x,x, T,x,x,T, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
		{T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,x, x,T,x,x, x,x,T,x, x,x,T,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,T,x,T, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,x, T,x,x,x, x,x,x,x, T,T,x,x, T,x,x,T, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
		{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,x, T,x,x,x, x,T,x,x, x,x,T,x, T,T,T,T, x,x,x,T, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,x, T,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
		{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,x, x,T,x,T, T,x,T,x, T,x,T,x, T,T,T,x, x,x,x,T, x,x,x,x, T,x,T,T, x,x,x,T, x,x,x,T, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,x, T,T,x,x, x,x,x,x, T,T,x,x, T,x,x,T, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
		{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,x, x,T,x,x, x,x,T,x, x,x,T,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,T, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,x, T,x,x,x, x,x,x,x, T,T,x,x, T,x,x,T, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
		{x,T,T,x, T,T,T,x, x,x,x,T, x,x,x,x, T,x,x,x, T,x,x,x, T,x,x,T, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,T,x, x,T,T,x, T,T,T,T, T,T,T,x, x,x,x,x, T,x,T,T, T,T,T,T, x,x,T,x, x,x,T,T, x,T,T,T, x,x,x,x, x,x,x,x, x,T,T,x, T,T,x,x, T,x,T,T, T,T,T,T, T,T,T,T, T,T,x,T, x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
		{x,T,T,x, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, T,x,x,T, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,T,x, x,T,T,x, x,x,T,T, T,x,T,x, x,x,x,x, T,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,T,T,T, x,T,x,x, x,x,x,x, T,x,T,x, T,T,x,x, T,x,x,T, x,T,x,T, T,T,T,x, T,x,x,T, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
		{T,T,T,x, T,T,T,x, x,x,x,T, x,x,x,x, T,x,x,x, T,x,x,x, T,x,x,T, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,T,x, x,T,T,x, T,T,T,T, T,T,T,x, x,x,x,x, T,x,T,T, T,T,T,T, x,x,T,x, x,x,T,T, x,T,T,T, x,x,x,x, x,x,x,x, x,T,T,x, T,T,x,x, T,x,T,T, T,T,T,T, T,T,T,T, T,T,x,T, x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
		{x,T,T,x, T,T,T,x, x,x,x,T, x,x,x,x, T,x,x,x, T,x,x,x, T,x,x,T, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,T,x, x,T,T,x, x,T,T,T, T,T,T,x, x,x,x,x, T,x,T,T, T,T,T,T, x,x,T,x, x,x,T,T, x,T,T,T, x,x,x,x, x,x,x,x, x,T,T,x, T,T,x,x, T,x,T,T, T,T,T,T, T,T,T,T, T,T,x,T, x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
		{x,T,T,x, T,T,T,x, x,x,x,x, x,x,x,x, T,x,x,x, T,x,x,x, T,x,x,T, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,T,x, x,T,T,x, x,x,T,T, T,x,T,x, x,x,x,x, T,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,T,T,T, x,x,x,x, x,x,x,x, x,x,T,x, T,T,T,x, T,x,x,T, x,T,x,T, T,T,T,x, T,x,x,T, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
		{x,T,T,x, T,T,T,x, x,x,x,T, x,x,x,x, T,x,x,x, T,x,x,x, T,x,x,T, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,T,T, x,T,T,x, T,T,T,T, T,T,T,x, x,x,x,x, T,x,T,T, T,T,T,T, x,x,T,x, x,x,T,T, x,T,T,T, x,x,x,x, x,x,x,x, x,T,T,x, T,T,x,x, T,x,T,T, T,T,T,T, T,T,T,T, T,T,x,T, x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
		{x,x,x,x, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, T,x,x,T, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
		{x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,T,x, x,T,T,x, x,x,T,T, T,x,T,x, x,x,x,x, T,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,T,T,T, x,x,x,x, x,x,x,x, x,x,T,x, T,T,x,x, T,x,x,T, x,T,x,T, T,T,T,x, T,x,x,T, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,x, x,T,x,x, x,x,T,x, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,T, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,x, T,x,x,x, x,x,x,x, T,T,x,x, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
		{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,x, x,T,x,x, x,x,T,x, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,T, x,T,x,x, x,x,x,x, T,x,T,x, T,x,x,x, T,x,x,x, x,x,x,x, T,T,x,x, T,x,x,T, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,T,x,x, T,T,T,x, x,x,x}

	};
	
	string ErrorDesc(int errorNumber)
	{
		switch (errorNumber) {
			case 0: return "EOF expected";
			case 1: return "ident expected";
			case 2: return "Literal expected";
			case 3: return "\"=\" expected";
			case 4: return "\"+\" expected";
			case 5: return "\"-\" expected";
			case 6: return "\"*\" expected";
			case 7: return "\"/\" expected";
			case 8: return "\"%\" expected";
			case 9: return "\":\" expected";
			case 10: return "\"::\" expected";
			case 11: return "\";\" expected";
			case 12: return "\"?\" expected";
			case 13: return "\"??\" expected";
			case 14: return "\",\" expected";
			case 15: return "\".\" expected";
			case 16: return "\"{\" expected";
			case 17: return "\"}\" expected";
			case 18: return "\"[\" expected";
			case 19: return "\"]\" expected";
			case 20: return "\"(\" expected";
			case 21: return "\")\" expected";
			case 22: return "\">\" expected";
			case 23: return "\"<\" expected";
			case 24: return "\"!\" expected";
			case 25: return "\"&&\" expected";
			case 26: return "\"||\" expected";
			case 27: return "\"~\" expected";
			case 28: return "\"&\" expected";
			case 29: return "\"|\" expected";
			case 30: return "\"^\" expected";
			case 31: return "\"++\" expected";
			case 32: return "\"--\" expected";
			case 33: return "\"==\" expected";
			case 34: return "\"!=\" expected";
			case 35: return "\">=\" expected";
			case 36: return "\"<=\" expected";
			case 37: return "\"<<\" expected";
			case 38: return "\"+=\" expected";
			case 39: return "\"-=\" expected";
			case 40: return "\"*=\" expected";
			case 41: return "\"/=\" expected";
			case 42: return "\"%=\" expected";
			case 43: return "\"&=\" expected";
			case 44: return "\"|=\" expected";
			case 45: return "\"^=\" expected";
			case 46: return "\"<<=\" expected";
			case 47: return "\"->\" expected";
			case 48: return "\"=>\" expected";
			case 49: return "\"abstract\" expected";
			case 50: return "\"as\" expected";
			case 51: return "\"base\" expected";
			case 52: return "\"bool\" expected";
			case 53: return "\"break\" expected";
			case 54: return "\"byte\" expected";
			case 55: return "\"case\" expected";
			case 56: return "\"catch\" expected";
			case 57: return "\"char\" expected";
			case 58: return "\"checked\" expected";
			case 59: return "\"class\" expected";
			case 60: return "\"const\" expected";
			case 61: return "\"continue\" expected";
			case 62: return "\"decimal\" expected";
			case 63: return "\"default\" expected";
			case 64: return "\"delegate\" expected";
			case 65: return "\"do\" expected";
			case 66: return "\"double\" expected";
			case 67: return "\"else\" expected";
			case 68: return "\"enum\" expected";
			case 69: return "\"event\" expected";
			case 70: return "\"explicit\" expected";
			case 71: return "\"extern\" expected";
			case 72: return "\"false\" expected";
			case 73: return "\"finally\" expected";
			case 74: return "\"fixed\" expected";
			case 75: return "\"float\" expected";
			case 76: return "\"for\" expected";
			case 77: return "\"foreach\" expected";
			case 78: return "\"goto\" expected";
			case 79: return "\"if\" expected";
			case 80: return "\"implicit\" expected";
			case 81: return "\"in\" expected";
			case 82: return "\"int\" expected";
			case 83: return "\"interface\" expected";
			case 84: return "\"internal\" expected";
			case 85: return "\"is\" expected";
			case 86: return "\"lock\" expected";
			case 87: return "\"long\" expected";
			case 88: return "\"namespace\" expected";
			case 89: return "\"new\" expected";
			case 90: return "\"null\" expected";
			case 91: return "\"object\" expected";
			case 92: return "\"operator\" expected";
			case 93: return "\"out\" expected";
			case 94: return "\"override\" expected";
			case 95: return "\"params\" expected";
			case 96: return "\"private\" expected";
			case 97: return "\"protected\" expected";
			case 98: return "\"public\" expected";
			case 99: return "\"readonly\" expected";
			case 100: return "\"ref\" expected";
			case 101: return "\"return\" expected";
			case 102: return "\"sbyte\" expected";
			case 103: return "\"sealed\" expected";
			case 104: return "\"short\" expected";
			case 105: return "\"sizeof\" expected";
			case 106: return "\"stackalloc\" expected";
			case 107: return "\"static\" expected";
			case 108: return "\"string\" expected";
			case 109: return "\"struct\" expected";
			case 110: return "\"switch\" expected";
			case 111: return "\"this\" expected";
			case 112: return "\"throw\" expected";
			case 113: return "\"true\" expected";
			case 114: return "\"try\" expected";
			case 115: return "\"typeof\" expected";
			case 116: return "\"uint\" expected";
			case 117: return "\"ulong\" expected";
			case 118: return "\"unchecked\" expected";
			case 119: return "\"unsafe\" expected";
			case 120: return "\"ushort\" expected";
			case 121: return "\"using\" expected";
			case 122: return "\"virtual\" expected";
			case 123: return "\"void\" expected";
			case 124: return "\"volatile\" expected";
			case 125: return "\"while\" expected";
			case 126: return "\"partial\" expected";
			case 127: return "\"where\" expected";
			case 128: return "\"get\" expected";
			case 129: return "\"set\" expected";
			case 130: return "\"add\" expected";
			case 131: return "\"remove\" expected";
			case 132: return "\"yield\" expected";
			case 133: return "\"select\" expected";
			case 134: return "\"group\" expected";
			case 135: return "\"by\" expected";
			case 136: return "\"into\" expected";
			case 137: return "\"from\" expected";
			case 138: return "\"ascending\" expected";
			case 139: return "\"descending\" expected";
			case 140: return "\"orderby\" expected";
			case 141: return "\"let\" expected";
			case 142: return "\"join\" expected";
			case 143: return "\"on\" expected";
			case 144: return "\"equals\" expected";
			case 145: return "??? expected";
			case 146: return "invalid NamespaceMemberDecl";
			case 147: return "invalid NonArrayType";
			case 148: return "invalid Identifier";
			case 149: return "invalid AttributeArgument";
			case 150: return "invalid Expr";
			case 151: return "invalid AttributeSection";
			case 152: return "invalid TypeModifier";
			case 153: return "invalid TypeDecl";
			case 154: return "invalid TypeDecl";
			case 155: return "this symbol not expected in ClassBody";
			case 156: return "this symbol not expected in InterfaceBody";
			case 157: return "invalid IntegralType";
			case 158: return "invalid ClassType";
			case 159: return "invalid ClassMemberDecl";
			case 160: return "invalid ClassMemberDecl";
			case 161: return "invalid StructMemberDecl";
			case 162: return "invalid StructMemberDecl";
			case 163: return "invalid StructMemberDecl";
			case 164: return "invalid StructMemberDecl";
			case 165: return "invalid StructMemberDecl";
			case 166: return "invalid StructMemberDecl";
			case 167: return "invalid StructMemberDecl";
			case 168: return "invalid StructMemberDecl";
			case 169: return "invalid StructMemberDecl";
			case 170: return "invalid StructMemberDecl";
			case 171: return "invalid StructMemberDecl";
			case 172: return "invalid StructMemberDecl";
			case 173: return "invalid StructMemberDecl";
			case 174: return "invalid InterfaceMemberDecl";
			case 175: return "invalid InterfaceMemberDecl";
			case 176: return "invalid InterfaceMemberDecl";
			case 177: return "invalid TypeWithRestriction";
			case 178: return "invalid TypeWithRestriction";
			case 179: return "invalid SimpleType";
			case 180: return "invalid AccessorModifiers";
			case 181: return "this symbol not expected in Block";
			case 182: return "invalid EventAccessorDecls";
			case 183: return "invalid ConstructorInitializer";
			case 184: return "invalid OverloadableOperator";
			case 185: return "invalid AccessorDecls";
			case 186: return "invalid InterfaceAccessors";
			case 187: return "invalid InterfaceAccessors";
			case 188: return "invalid GetAccessorDecl";
			case 189: return "invalid SetAccessorDecl";
			case 190: return "invalid VariableInitializer";
			case 191: return "this symbol not expected in Statement";
			case 192: return "invalid Statement";
			case 193: return "invalid Argument";
			case 194: return "invalid AssignmentOperator";
			case 195: return "invalid ObjectPropertyInitializerOrVariableInitializer";
			case 196: return "invalid ObjectPropertyInitializerOrVariableInitializer";
			case 197: return "invalid EmbeddedStatement";
			case 198: return "invalid EmbeddedStatement";
			case 199: return "this symbol not expected in EmbeddedStatement";
			case 200: return "invalid EmbeddedStatement";
			case 201: return "invalid ForInitializer";
			case 202: return "invalid GotoStatement";
			case 203: return "invalid ResourceAcquisition";
			case 204: return "invalid SwitchLabel";
			case 205: return "invalid CatchClause";
			case 206: return "invalid UnaryExpr";
			case 207: return "invalid PrimaryExpr";
			case 208: return "invalid PrimaryExpr";
			case 209: return "invalid TypeArgumentList";
			case 210: return "invalid NewExpression";
			case 211: return "invalid NewExpression";
			case 212: return "invalid LambdaExpressionParameter";
			case 213: return "invalid LambdaExpressionBody";
			case 214: return "invalid TypeParameterConstraintsClauseBase";
			case 215: return "invalid QueryExpressionBody";

			default: return "error " + errorNumber;
		}
	}

} // end Parser

} // end namespace
