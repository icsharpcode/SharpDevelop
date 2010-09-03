// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.CodeDom;
using Boo.Lang.Compiler.Ast;
using Grunwald.BooBinding.CodeCompletion;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;

namespace Grunwald.BooBinding.Designer
{
	/// <summary>
	/// The CodeDomVisitor is able to convert from the Boo AST to System.CodeDom
	/// It makes use of the SharpDevelop parser service to get necessary additional
	/// information about the types.
	/// </summary>
	public class CodeDomVisitor : DepthFirstVisitor
	{
		CodeCompileUnit _compileUnit = new CodeCompileUnit();
		
		public CodeCompileUnit OutputCompileUnit {
			get {
				return _compileUnit;
			}
		}
		
		IProjectContent pc;
		
		public CodeDomVisitor(IProjectContent pc)
		{
			if (pc == null)
				throw new ArgumentNullException("pc");
			this.pc = pc;
		}
		
		CodeNamespace _namespace;
		CodeTypeDeclaration _class;
		CodeStatementCollection _statements;
		CodeExpression _expression;
		
		MemberAttributes ConvModifiers(TypeMember member)
		{
			bool strict = ConvertVisitor.IsStrictMode(pc);
			if (member is Field)
				return ConvModifiers(member.Modifiers, strict ? MemberAttributes.Private : MemberAttributes.Family);
			else
				return ConvModifiers(member.Modifiers, strict ? MemberAttributes.Private : MemberAttributes.Public);
		}
		
		MemberAttributes ConvModifiers(TypeMemberModifiers modifier, MemberAttributes defaultVisibility)
		{
			MemberAttributes attr = 0;
			if ((modifier & TypeMemberModifiers.Abstract) == TypeMemberModifiers.Abstract)
				attr |= MemberAttributes.Abstract;
			if ((modifier & TypeMemberModifiers.Final) == TypeMemberModifiers.Final)
				attr |= MemberAttributes.Const;
			if ((modifier & TypeMemberModifiers.Internal) == TypeMemberModifiers.Internal)
				attr |= MemberAttributes.Assembly;
			if ((modifier & TypeMemberModifiers.Override) == TypeMemberModifiers.Override)
				attr |= MemberAttributes.Override;
			if ((modifier & TypeMemberModifiers.Private) == TypeMemberModifiers.Private)
				attr |= MemberAttributes.Private;
			if ((modifier & TypeMemberModifiers.Protected) == TypeMemberModifiers.Protected)
				attr |= MemberAttributes.Family;
			if ((modifier & TypeMemberModifiers.Public) == TypeMemberModifiers.Public)
				attr |= MemberAttributes.Public;
			if ((modifier & TypeMemberModifiers.Static) == TypeMemberModifiers.Static)
				attr |= MemberAttributes.Static;
			if ((modifier & TypeMemberModifiers.Virtual) != TypeMemberModifiers.Virtual)
				attr |= MemberAttributes.Final;
			if ((modifier & TypeMemberModifiers.VisibilityMask) == TypeMemberModifiers.None)
				attr |= defaultVisibility;
			return attr;
		}
		
		CodeTypeReference ConvTypeRef(TypeReference tr)
		{
			if (tr == null) return null;
			string name = tr.ToString();
			if (BooAmbience.ReverseTypeConversionTable.ContainsKey(name))
				name = BooAmbience.ReverseTypeConversionTable[name];
			return new CodeTypeReference(name);
		}
		
		public override void OnCompileUnit(CompileUnit node)
		{
			foreach (Module m in node.Modules)
				m.Accept(this);
		}
		
		public override void OnModule(Module node)
		{
			if (node.Namespace == null) {
				_namespace = new CodeNamespace("Global");
				_compileUnit.Namespaces.Add(_namespace);
			} else {
				node.Namespace.Accept(this);
			}
			foreach (Import i in node.Imports)
				i.Accept(this);
			foreach (TypeMember m in node.Members)
				m.Accept(this);
		}
		
		public override void OnNamespaceDeclaration(NamespaceDeclaration node)
		{
			_namespace = new CodeNamespace(node.Name);
			_compileUnit.Namespaces.Add(_namespace);
		}
		
		public override void OnImport(Import node)
		{
			_namespace.Imports.Add(new CodeNamespaceImport(node.Namespace));
		}
		
		CodeTypeDeclaration ConvertTypeDefinition(TypeDefinition node)
		{
			CodeTypeDeclaration oldClass = _class;
			CodeTypeDeclaration newClass = new CodeTypeDeclaration(node.Name);
			_class = newClass;
			
			foreach (TypeReference b in node.BaseTypes)
				newClass.BaseTypes.Add(ConvTypeRef(b));
			
			foreach (TypeMember member in node.Members)
				member.Accept(this);
			
			if (oldClass == null)
				_namespace.Types.Add(newClass);
			else
				oldClass.Members.Add(newClass);
			_class = oldClass;
			return newClass;
		}
		
		public override void OnClassDefinition(ClassDefinition node)
		{
			ConvertTypeDefinition(node).IsClass = true;
		}
		
		public override void OnStructDefinition(StructDefinition node)
		{
			ConvertTypeDefinition(node).IsStruct = true;
		}
		public override void OnInterfaceDefinition(InterfaceDefinition node)
		{
			ConvertTypeDefinition(node).IsInterface = true;
		}
		
		public override void OnField(Field node)
		{
			CodeMemberField field = new CodeMemberField(ConvTypeRef(node.Type), node.Name);
			field.Attributes = ConvModifiers(node);
			if (node.Initializer != null) {
				_expression = null;
				//Visit(node.Initializer);
				field.InitExpression = _expression;
			}
			_class.Members.Add(field);
		}
		
		public override void OnConstructor(Boo.Lang.Compiler.Ast.Constructor node)
		{
			ConvertMethod(node, new CodeConstructor());
		}
		public override void OnMethod(Method node)
		{
			CodeMemberMethod cmm = new CodeMemberMethod();
			cmm.Name = node.Name;
			cmm.ReturnType = ConvTypeRef(node.ReturnType);
			ConvertMethod(node, cmm);
		}
		public override void OnDestructor(Boo.Lang.Compiler.Ast.Destructor node)
		{
			CodeMemberMethod cmm = new CodeMemberMethod();
			cmm.Name = "Finalize";
			ConvertMethod(node, cmm);
		}
		void ConvertMethod(Method node, CodeMemberMethod method)
		{
			method.Attributes = ConvModifiers(node);
			if (node.Parameters != null) {
				foreach (ParameterDeclaration p in node.Parameters)
					method.Parameters.Add(new CodeParameterDeclarationExpression(ConvTypeRef(p.Type), p.Name));
			}
			_statements = method.Statements;
			
			if (node.Body != null)
				node.Body.Accept(this);
			
			_class.Members.Add(method);
		}
		
		static CodeBinaryOperatorType GetOperatorType(BinaryOperatorType op)
		{
			switch (op) {
				case BinaryOperatorType.Addition:
					return CodeBinaryOperatorType.Add;
				case BinaryOperatorType.And:
					return CodeBinaryOperatorType.BooleanAnd;
				case BinaryOperatorType.BitwiseAnd:
					return CodeBinaryOperatorType.BitwiseAnd;
				case BinaryOperatorType.BitwiseOr:
					return CodeBinaryOperatorType.BitwiseOr;
				case BinaryOperatorType.Division:
					return CodeBinaryOperatorType.Divide;
				case BinaryOperatorType.Equality:
					return CodeBinaryOperatorType.ValueEquality;
				case BinaryOperatorType.GreaterThan:
					return CodeBinaryOperatorType.GreaterThan;
				case BinaryOperatorType.GreaterThanOrEqual:
					return CodeBinaryOperatorType.GreaterThanOrEqual;
				case BinaryOperatorType.LessThan:
					return CodeBinaryOperatorType.LessThan;
				case BinaryOperatorType.LessThanOrEqual:
					return CodeBinaryOperatorType.LessThanOrEqual;
				case BinaryOperatorType.Modulus:
					return CodeBinaryOperatorType.Modulus;
				case BinaryOperatorType.Multiply:
					return CodeBinaryOperatorType.Multiply;
				case BinaryOperatorType.Or:
					return CodeBinaryOperatorType.BooleanOr;
				case BinaryOperatorType.Subtraction:
					return CodeBinaryOperatorType.Subtract;
				default:
					return CodeBinaryOperatorType.Assign;
			}
		}
		
		public override void OnBinaryExpression(BinaryExpression node)
		{
			_expression = null;
			CodeBinaryOperatorType op = GetOperatorType(node.Operator);
			if (op == CodeBinaryOperatorType.Assign) {
				// non-standard op
				if (node.Operator == BinaryOperatorType.Assign) {
					node.Left.Accept(this);
					CodeExpression left = _expression;
					_expression = null;
					node.Right.Accept(this);
					if (left != null && _expression != null)
						_statements.Add(new CodeAssignStatement(left, _expression));
					_expression = null;
				} else if (node.Operator == BinaryOperatorType.InPlaceAddition) {
					node.Left.Accept(this);
					CodeEventReferenceExpression left = _expression as CodeEventReferenceExpression;
					_expression = null;
					node.Right.Accept(this);
					if (left != null && _expression != null)
						_statements.Add(new CodeAttachEventStatement(left, _expression));
					_expression = null;
				} else {
					LoggingService.Warn("CodeDomVisitor: ignoring unknown Binary Operator" + node.Operator);
				}
			} else {
				node.Left.Accept(this);
				CodeExpression left = _expression;
				_expression = null;
				node.Right.Accept(this);
				_expression = new CodeBinaryOperatorExpression(left, op, _expression);
			}
		}
		
		public override void OnTryCastExpression(TryCastExpression node)
		{
			_expression = null;
			node.Target.Accept(this);
			if (_expression == null)
				return;
			if (_expression is CodeMethodReferenceExpression) {
				_expression = new CodeObjectCreateExpression(ConvTypeRef(node.Type), _expression);
			} else {
				_expression = new CodeCastExpression(ConvTypeRef(node.Type), _expression);
			}
		}
		
		public override void OnCastExpression(CastExpression node)
		{
			_expression = null;
			node.Target.Accept(this);
			if (_expression == null)
				return;
			if (_expression is CodeMethodReferenceExpression) {
				_expression = new CodeObjectCreateExpression(ConvTypeRef(node.Type), _expression);
			} else {
				_expression = new CodeCastExpression(ConvTypeRef(node.Type), _expression);
			}
		}
		
		public override void OnBlock(Block node)
		{
			foreach (Statement n in node.Statements)
				n.Accept(this);
		}
		
		public override void OnExpressionStatement(ExpressionStatement node)
		{
			_expression = null;
			node.Expression.Accept(this);
			if (_expression != null)
				_statements.Add(new CodeExpressionStatement(_expression));
		}
		
		public override void OnGotoStatement(GotoStatement node)
		{
			_statements.Add(new CodeGotoStatement(node.Label.Name));
		}
		
		public override void OnNullLiteralExpression(NullLiteralExpression node)
		{
			_expression = new CodePrimitiveExpression(null);
		}
		
		public override void OnBoolLiteralExpression(BoolLiteralExpression node)
		{
			_expression = new CodePrimitiveExpression(node.Value);
		}
		
		public override void OnStringLiteralExpression(StringLiteralExpression node)
		{
			_expression = new CodePrimitiveExpression(node.Value);
		}
		
		public override void OnCharLiteralExpression(CharLiteralExpression node)
		{
			_expression = new CodePrimitiveExpression(node.Value);
		}
		
		public override void OnIntegerLiteralExpression(IntegerLiteralExpression node)
		{
			if (node.IsLong || node.Value < int.MinValue || node.Value > int.MaxValue)
				_expression = new CodePrimitiveExpression(node.Value);
			else
				_expression = new CodePrimitiveExpression((int)node.Value);
		}
		
		public override void OnDoubleLiteralExpression(DoubleLiteralExpression node)
		{
			_expression = new CodePrimitiveExpression(node.Value);
		}
		
		public override void OnMemberReferenceExpression(MemberReferenceExpression node)
		{
			_expression = null;
			node.Target.Accept(this);
			if (_expression != null) {
				if (_expression is CodeTypeReferenceExpression) {
					string baseName = ((CodeTypeReferenceExpression)_expression).Type.BaseType;
					_expression = CreateMemberExpression(_expression, baseName, node.Name, true);
				} else {
					_expression = CreateMemberExpression(_expression, node.Name);
				}
			}
		}
		
		public override void OnDeclarationStatement(DeclarationStatement node)
		{
			CodeVariableDeclarationStatement var = new CodeVariableDeclarationStatement(ConvTypeRef(node.Declaration.Type),
			                                                                            node.Declaration.Name);
			if (node.Initializer != null) {
				_expression = null;
				node.Initializer.Accept(this);
				var.InitExpression = _expression;
			}
			_statements.Add(var);
		}
		
		
		CodeVariableDeclarationStatement GetLocalVariable(string name)
		{
			foreach (CodeStatement stmt in _statements) {
				CodeVariableDeclarationStatement var = stmt as CodeVariableDeclarationStatement;
				if (var != null && var.Name == name) {
					return var;
				}
			}
			return null;
		}
		
		public override void OnReferenceExpression(ReferenceExpression node)
		{
			if (pc.GetClass(node.Name, 0) != null) {
				_expression = new CodeTypeReferenceExpression(node.Name);
			} else if (pc.NamespaceExists(node.Name)) {
				_expression = new CodeTypeReferenceExpression(node.Name);
			} else if (GetLocalVariable(node.Name) != null) {
				_expression = new CodeVariableReferenceExpression(node.Name);
			} else {
				_expression = CreateMemberExpression(new CodeThisReferenceExpression(), node.Name);
			}
		}
		
		CodeExpression CreateMemberExpression(CodeExpression expr, string name)
		{
			if (expr is CodeTypeReferenceExpression) {
				string typeRef = ((CodeTypeReferenceExpression)_expression).Type.BaseType;
				return CreateMemberExpression(expr, typeRef, name, true);
			} else if (expr is CodeThisReferenceExpression) {
				string typeRef = _namespace.Name + "." + _class.Name;
				return CreateMemberExpression(expr, typeRef, name, false);
			} else if (expr is CodeFieldReferenceExpression || expr is CodePropertyReferenceExpression) {
				if (_fieldReferenceType == null)
					return new CodePropertyReferenceExpression(expr, name);
				return CreateMemberExpression(expr, _fieldReferenceType.FullyQualifiedName, name, false);
			} else if (expr is CodeVariableReferenceExpression) {
				string varName = ((CodeVariableReferenceExpression)expr).VariableName;
				CodeVariableDeclarationStatement varDecl = GetLocalVariable(varName);
				return CreateMemberExpression(expr, varDecl.Type.BaseType, name, false);
			} else {
				_fieldReferenceType = null;
				return new CodePropertyReferenceExpression(expr, name);
			}
		}
		
		IReturnType _fieldReferenceType;
		
		CodeExpression CreateMemberExpression(CodeExpression target, string parentName, string name, bool isStatic)
		{
			_fieldReferenceType = null;
			
			string combinedName = parentName + "." + name;
			if (pc.GetClass(combinedName, 0) != null)
				return new CodeTypeReferenceExpression(combinedName);
			else if (pc.NamespaceExists(combinedName))
				return new CodeTypeReferenceExpression(combinedName);
			
			GetClassReturnType rt = new GetClassReturnType(pc, parentName, 0);
			foreach (IProperty prop in rt.GetProperties()) {
				if (prop.IsStatic == isStatic && prop.Name == name) {
					_fieldReferenceType = prop.ReturnType;
					return new CodePropertyReferenceExpression(target, name);
				}
			}
			foreach (IEvent ev in rt.GetEvents()) {
				if (ev.IsStatic == isStatic && ev.Name == name) {
					_fieldReferenceType = ev.ReturnType;
					return new CodeEventReferenceExpression(target, name);
				}
			}
			foreach (IMethod me in rt.GetMethods()) {
				if (me.IsStatic == isStatic && me.Name == name) {
					_fieldReferenceType = me.ReturnType;
					CodeMethodReferenceExpression cmre = new CodeMethodReferenceExpression(target, name);
					cmre.UserData["methodUserData"] = me;
					return cmre;
				}
			}
			foreach (IField field in rt.GetFields()) {
				if (field.IsStatic == isStatic && field.Name == name) {
					_fieldReferenceType = field.ReturnType;
					return new CodeFieldReferenceExpression(target, name);
				}
			}
			// unknown member, guess:
			if (char.IsUpper(name, 0))
				return new CodePropertyReferenceExpression(target, name);
			else
				return new CodeFieldReferenceExpression(target, name);
		}
		
		public override void OnQuasiquoteExpression(QuasiquoteExpression node)
		{
			_expression = new CodeObjectCreateExpression(node.Node.GetType());
		}
		
		public override void OnMethodInvocationExpression(MethodInvocationExpression node)
		{
			_expression = null;
			node.Target.Accept(this);
			if (_expression != null) {
				CodeMethodInvokeExpression cmie;
				CodeObjectCreateExpression coce;
				
				if (_expression is CodeTypeReferenceExpression) {
					coce = new CodeObjectCreateExpression(((CodeTypeReferenceExpression)_expression).Type);
					ConvertExpressions(coce.Parameters, node.Arguments);
					_expression = coce;
				} else if (_expression is CodeMethodReferenceExpression) {
					cmie = new CodeMethodInvokeExpression((CodeMethodReferenceExpression)_expression);
					ConvertExpressions(cmie.Parameters, node.Arguments);
					FixArrayArguments(cmie);
					_expression = cmie;
				} else if (_expression is CodeFieldReferenceExpression) {
					// when a type is unknown, a MemberReferenceExpression is translated into a CodeFieldReferenceExpression
					CodeFieldReferenceExpression cfre = (CodeFieldReferenceExpression)_expression;
					cmie = new CodeMethodInvokeExpression(cfre.TargetObject, cfre.FieldName);
					ConvertExpressions(cmie.Parameters, node.Arguments);
					FixArrayArguments(cmie);
					_expression = cmie;
				} else {
					_expression = null;
				}
			}
		}
		
		/// <summary>
		/// Fixes the type of array literals used as arguments to the method.
		/// </summary>
		void FixArrayArguments(CodeMethodInvokeExpression cmie)
		{
			IMethod m = cmie.Method.UserData["methodUserData"] as IMethod;
			if (m != null) {
				int count = Math.Min(m.Parameters.Count, cmie.Parameters.Count);
				for (int i = 0; i < count; i++) {
					CodeArrayCreateExpression cace = cmie.Parameters[i] as CodeArrayCreateExpression;
					if (cace != null && (bool)cace.UserData["unknownType"]
					    && m.Parameters[i].ReturnType.IsArrayReturnType)
					{
						cace.CreateType = new CodeTypeReference(m.Parameters[i].ReturnType.FullyQualifiedName);
					}
				}
			}
		}
		
		/// <summary>Converts a list of expressions to CodeDom expressions.</summary>
		void ConvertExpressions(CodeExpressionCollection args, ExpressionCollection expressions)
		{
			foreach (Expression e in expressions) {
				_expression = null;
				e.Accept(this);
				args.Add(_expression);
			}
		}
		
		public override void OnReturnStatement(ReturnStatement node)
		{
			_expression = null;
			if (node.Expression != null)
				node.Expression.Accept(this);
			_statements.Add(new CodeMethodReturnStatement(_expression));
		}
		
		public override void OnSelfLiteralExpression(SelfLiteralExpression node)
		{
			_expression = new CodeThisReferenceExpression();
		}
		
		public override void OnSuperLiteralExpression(SuperLiteralExpression node)
		{
			_expression = new CodeBaseReferenceExpression();
		}
		
		public override void OnTypeofExpression(TypeofExpression node)
		{
			_expression = new CodeTypeOfExpression(ConvTypeRef(node.Type));
		}
		
		public override void OnArrayLiteralExpression(ArrayLiteralExpression node)
		{
			BooResolver resolver = new BooResolver();
			IReturnType createType = resolver.GetTypeOfExpression(node, null);
			if (createType == null)
				createType = pc.SystemTypes.Object;
			CodeExpression[] initializers = new CodeExpression[node.Items.Count];
			for (int i = 0; i < initializers.Length; i++) {
				_expression = null;
				node.Items[i].Accept(this);
				initializers[i] = _expression;
			}
			_expression = new CodeArrayCreateExpression(createType.FullyQualifiedName, initializers);
			_expression.UserData["unknownType"] = node.Type != null;
		}
	}
}
