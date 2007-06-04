// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;

using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.Visitors;
using NR = ICSharpCode.NRefactory;

namespace ICSharpCode.SharpDevelop.Dom.NRefactoryResolver
{
	/// <summary>
	/// NRefactoryResolver implements the IResolver interface for the NRefactory languages (C# and VB).
	/// </summary>
	/// <remarks>
	/// About implementing code-completion for other languages:
	/// 
	/// It possible to convert from your AST to NRefactory (to C# or VB) (or even let your parser create
	/// NRefactory AST objects directly), but then code-completion might be incorrect when the rules of your language
	/// differ from the C#/VB language rules.
	/// If you want to correctly implement code-completion for your own language, you should implement your own resolver.
	/// </remarks>
	public class NRefactoryResolver : IResolver
	{
		ICompilationUnit cu;
		IClass callingClass;
		IMember callingMember;
		ICSharpCode.NRefactory.Visitors.LookupTableVisitor lookupTableVisitor;
		IProjectContent projectContent;
		
		readonly NR.SupportedLanguage language;
		
		int caretLine;
		int caretColumn;
		
		public NR.SupportedLanguage Language {
			get {
				return language;
			}
		}
		
		public IProjectContent ProjectContent {
			get {
				return projectContent;
			}
			set {
				if (value == null)
					throw new ArgumentNullException("value");
				projectContent = value;
			}
		}
		
		public ICompilationUnit CompilationUnit {
			get {
				return cu;
			}
		}
		
		public IClass CallingClass {
			get {
				return callingClass;
			}
		}
		
		public IMember CallingMember {
			get {
				return callingMember;
			}
		}
		
		public int CaretLine {
			get {
				return caretLine;
			}
		}
		
		public int CaretColumn {
			get {
				return caretColumn;
			}
		}
		
		readonly LanguageProperties languageProperties;
		
		public LanguageProperties LanguageProperties {
			get {
				return languageProperties;
			}
		}
		
		public NRefactoryResolver(LanguageProperties languageProperties)
		{
			if (languageProperties == null)
				throw new ArgumentNullException("languageProperties");
			this.languageProperties = languageProperties;
			if (languageProperties is LanguageProperties.CSharpProperties) {
				language = NR.SupportedLanguage.CSharp;
			} else if (languageProperties is LanguageProperties.VBNetProperties) {
				language = NR.SupportedLanguage.VBNet;
			} else {
				throw new NotSupportedException("The language " + languageProperties.ToString() + " is not supported in the resolver");
			}
		}
		
		Expression ParseExpression(string expression)
		{
			Expression expr = SpecialConstructs(expression);
			if (expr == null) {
				using (NR.IParser p = NR.ParserFactory.CreateParser(language, new System.IO.StringReader(expression))) {
					expr = p.ParseExpression();
					if (expr != null) {
						expr.AcceptVisitor(new SetAllNodePoints(new NR.Location(caretColumn, caretLine),
						                                        new NR.Location(caretColumn + 1, caretLine)), null);
					}
				}
			}
			return expr;
		}
		
		sealed class SetAllNodePoints : NodeTrackingAstVisitor
		{
			readonly NR.Location start, end;
			
			public SetAllNodePoints(ICSharpCode.NRefactory.Location start, ICSharpCode.NRefactory.Location end)
			{
				this.start = start;
				this.end = end;
			}
			
			protected override void BeginVisit(INode node)
			{
				node.StartLocation = start;
				node.EndLocation = end;
			}
		}
		
		string GetFixedExpression(ExpressionResult expressionResult)
		{
			string expression = expressionResult.Expression;
			if (expression == null) {
				expression = "";
			}
			expression = expression.TrimStart();
			
			return expression;
		}
		
		public bool Initialize(ParseInformation parseInfo, int caretLineNumber, int caretColumn)
		{
			this.caretLine   = caretLineNumber;
			this.caretColumn = caretColumn;
			
			if (parseInfo == null) {
				return false;
			}
			
			cu = parseInfo.MostRecentCompilationUnit;
			if (cu == null || cu.ProjectContent == null) {
				return false;
			}
			this.ProjectContent = cu.ProjectContent;
			
			callingClass = cu.GetInnermostClass(caretLine, caretColumn);
			callingMember = GetCurrentMember();
			return true;
		}
		
		public ResolveResult Resolve(ExpressionResult expressionResult,
		                             int caretLineNumber,
		                             int caretColumn,
		                             ParseInformation parseInfo,
		                             string fileContent)
		{
			string expression = GetFixedExpression(expressionResult);
			
			if (!Initialize(parseInfo, caretLineNumber, caretColumn))
				return null;
			
			Expression expr = null;
			if (language == NR.SupportedLanguage.VBNet) {
				if (expression.Length == 0 || expression[0] == '.') {
					return WithResolve(expression, fileContent);
				} else if ("global".Equals(expression, StringComparison.InvariantCultureIgnoreCase)) {
					return new NamespaceResolveResult(null, null, "");
				}
			}
			if (expr == null) {
				expr = ParseExpression(expression);
				if (expr == null) {
					return null;
				}
				if (expressionResult.Context.IsObjectCreation) {
					Expression tmp = expr;
					while (tmp != null) {
						if (tmp is IdentifierExpression)
							return ResolveInternal(expr, ExpressionContext.Type);
						if (tmp is FieldReferenceExpression)
							tmp = (tmp as FieldReferenceExpression).TargetObject;
						else
							break;
					}
					expr = ParseExpression("new " + expression);
					if (expr == null) {
						return null;
					}
				}
			}
			
			if (expressionResult.Context.IsAttributeContext) {
				return ResolveAttribute(expr, new NR.Location(caretColumn, caretLineNumber));
			}
			
			RunLookupTableVisitor(fileContent);
			
			ResolveResult rr = CtrlSpaceResolveHelper.GetResultFromDeclarationLine(callingClass, callingMember as IMethodOrProperty, caretLine, caretColumn, expressionResult);
			if (rr != null) return rr;
			
			return ResolveInternal(expr, expressionResult.Context);
		}
		
		ResolveResult WithResolve(string expression, string fileContent)
		{
			RunLookupTableVisitor(fileContent);
			
			WithStatement innermost = null;
			if (lookupTableVisitor.WithStatements != null) {
				foreach (WithStatement with in lookupTableVisitor.WithStatements) {
					if (IsInside(new NR.Location(caretColumn, caretLine), with.StartLocation, with.EndLocation)) {
						innermost = with;
					}
				}
			}
			if (innermost != null) {
				if (expression.Length > 1) {
					Expression expr = ParseExpression(DummyFindVisitor.dummyName + expression);
					if (expr == null) return null;
					DummyFindVisitor v = new DummyFindVisitor();
					expr.AcceptVisitor(v, null);
					if (v.result == null) return null;
					v.result.TargetObject = innermost.Expression;
					return ResolveInternal(expr, ExpressionContext.Default);
				} else {
					return ResolveInternal(innermost.Expression, ExpressionContext.Default);
				}
			} else {
				return null;
			}
		}
		private class DummyFindVisitor : AbstractAstVisitor {
			internal const string dummyName = "___withStatementExpressionDummy";
			internal FieldReferenceExpression result;
			public override object VisitFieldReferenceExpression(FieldReferenceExpression fieldReferenceExpression, object data)
			{
				IdentifierExpression ie = fieldReferenceExpression.TargetObject as IdentifierExpression;
				if (ie != null && ie.Identifier == dummyName)
					result = fieldReferenceExpression;
				return base.VisitFieldReferenceExpression(fieldReferenceExpression, data);
			}
		}
		
		public INode ParseCurrentMember(string fileContent)
		{
			CompilationUnit cu = ParseCurrentMemberAsCompilationUnit(fileContent);
			if (cu != null && cu.Children.Count > 0) {
				TypeDeclaration td = cu.Children[0] as TypeDeclaration;
				if (td != null && td.Children.Count > 0) {
					return td.Children[0];
				}
			}
			return null;
		}
		
		public CompilationUnit ParseCurrentMemberAsCompilationUnit(string fileContent)
		{
			System.IO.TextReader content = ExtractCurrentMethod(fileContent);
			if (content != null) {
				NR.IParser p = NR.ParserFactory.CreateParser(language, content);
				p.Parse();
				return p.CompilationUnit;
			} else {
				return null;
			}
		}
		
		void RunLookupTableVisitor(string fileContent)
		{
			lookupTableVisitor = new LookupTableVisitor(language);
			
			if (callingMember != null) {
				CompilationUnit cu = ParseCurrentMemberAsCompilationUnit(fileContent);
				if (cu != null) {
					lookupTableVisitor.VisitCompilationUnit(cu, null);
				}
			}
		}
		
		public void RunLookupTableVisitor(INode currentMemberNode)
		{
			lookupTableVisitor = new LookupTableVisitor(language);
			currentMemberNode.AcceptVisitor(lookupTableVisitor, null);
		}
		
		string GetAttributeName(Expression expr)
		{
			if (expr is IdentifierExpression) {
				return (expr as IdentifierExpression).Identifier;
			} else if (expr is FieldReferenceExpression) {
				TypeVisitor typeVisitor = new TypeVisitor(this);
				FieldReferenceExpression fieldReferenceExpression = (FieldReferenceExpression)expr;
				IReturnType type = fieldReferenceExpression.TargetObject.AcceptVisitor(typeVisitor, null) as IReturnType;
				if (type is TypeVisitor.NamespaceReturnType) {
					return type.FullyQualifiedName + "." + fieldReferenceExpression.FieldName;
				}
			}
			return null;
		}
		
		IClass GetAttribute(string name, NR.Location position)
		{
			if (name == null)
				return null;
			IClass c = SearchClass(name, position);
			if (c != null) {
				if (c.IsTypeInInheritanceTree(c.ProjectContent.SystemTypes.Attribute.GetUnderlyingClass()))
					return c;
			}
			return SearchClass(name + "Attribute", position);
		}
		
		ResolveResult ResolveAttribute(Expression expr, NR.Location position)
		{
			string attributeName = GetAttributeName(expr);
			IClass c = GetAttribute(attributeName, position);
			if (c != null) {
				return new TypeResolveResult(callingClass, callingMember, c);
			} else if (expr is InvocationExpression) {
				InvocationExpression ie = (InvocationExpression)expr;
				attributeName = GetAttributeName(ie.TargetObject);
				c = GetAttribute(attributeName, position);
				if (c != null) {
					List<IMethod> ctors = new List<IMethod>();
					foreach (IMethod m in c.Methods) {
						if (m.IsConstructor && !m.IsStatic)
							ctors.Add(m);
					}
					TypeVisitor typeVisitor = new TypeVisitor(this);
					return CreateMemberResolveResult(typeVisitor.FindOverload(ctors, null, ie.Arguments));
				}
			}
			return null;
		}
		
		public ResolveResult ResolveInternal(Expression expr, ExpressionContext context)
		{
			TypeVisitor typeVisitor = new TypeVisitor(this);
			IReturnType type;
			
			if (expr is PrimitiveExpression) {
				if (((PrimitiveExpression)expr).Value is int)
					return new IntegerLiteralResolveResult(callingClass, callingMember, projectContent.SystemTypes.Int32);
			} else if (expr is InvocationExpression) {
				IMethodOrProperty method = typeVisitor.GetMethod(expr as InvocationExpression);
				if (method != null) {
					return CreateMemberResolveResult(method);
				} else {
					// InvocationExpression can also be a delegate/event call
					ResolveResult invocationTarget = ResolveInternal((expr as InvocationExpression).TargetObject, ExpressionContext.Default);
					if (invocationTarget == null)
						return null;
					type = invocationTarget.ResolvedType;
					if (type == null)
						return null;
					IClass c = type.GetUnderlyingClass();
					if (c == null || c.ClassType != ClassType.Delegate)
						return null;
					// We don't want to show "System.EventHandler.Invoke" in the tooltip
					// of "EventCall(this, EventArgs.Empty)", we just show the event/delegate for now
					
					// but for DelegateCall(params).* completion, we use the delegate's
					// return type instead of the delegate type itself
					method = c.Methods.Find(delegate(IMethod innerMethod) { return innerMethod.Name == "Invoke"; });
					if (method != null)
						invocationTarget.ResolvedType = method.ReturnType;
					
					return invocationTarget;
				}
			} else if (expr is IndexerExpression) {
				return CreateMemberResolveResult(typeVisitor.GetIndexer(expr as IndexerExpression));
			} else if (expr is FieldReferenceExpression) {
				FieldReferenceExpression fieldReferenceExpression = (FieldReferenceExpression)expr;
				if (fieldReferenceExpression.FieldName == null || fieldReferenceExpression.FieldName.Length == 0) {
					// NRefactory creates this "dummy" fieldReferenceExpression when it should
					// parse a primitive type name (int, short; Integer, Decimal)
					if (fieldReferenceExpression.TargetObject is TypeReferenceExpression) {
						type = TypeVisitor.CreateReturnType(((TypeReferenceExpression)fieldReferenceExpression.TargetObject).TypeReference, this);
						if (type != null) {
							return new TypeResolveResult(callingClass, callingMember, type);
						}
					}
				}
				type = fieldReferenceExpression.TargetObject.AcceptVisitor(typeVisitor, null) as IReturnType;
				if (type != null) {
					ResolveResult result = ResolveMemberReferenceExpression(type, fieldReferenceExpression);
					if (result != null)
						return result;
				}
			} else if (expr is IdentifierExpression) {
				ResolveResult result = ResolveIdentifier(((IdentifierExpression)expr).Identifier, expr.StartLocation, context);
				if (result != null)
					return result;
			} else if (expr is TypeReferenceExpression) {
				type = TypeVisitor.CreateReturnType(((TypeReferenceExpression)expr).TypeReference, this);
				if (type != null) {
					if (type is TypeVisitor.NamespaceReturnType)
						return new NamespaceResolveResult(callingClass, callingMember, type.FullyQualifiedName);
					IClass c = type.GetUnderlyingClass();
					if (c != null)
						return new TypeResolveResult(callingClass, callingMember, type, c);
				}
				return null;
			}
			type = expr.AcceptVisitor(typeVisitor, null) as IReturnType;
			
			if (type == null || type.FullyQualifiedName == "") {
				return null;
			}
			if (language == NR.SupportedLanguage.VBNet
			    && callingMember is IMethod && (callingMember as IMethod).IsConstructor
			    && (expr is BaseReferenceExpression || expr is ThisReferenceExpression))
			{
				return new VBBaseOrThisReferenceInConstructorResolveResult(callingClass, callingMember, type);
			}
			if (expr is ObjectCreateExpression) {
				List<IMethod> constructors = new List<IMethod>();
				foreach (IMethod m in type.GetMethods()) {
					if (m.IsConstructor && !m.IsStatic)
						constructors.Add(m);
				}
				
				if (constructors.Count == 0) {
					// Class has no constructors -> create default constructor
					IClass c = type.GetUnderlyingClass();
					if (c != null) {
						return CreateMemberResolveResult(Constructor.CreateDefault(c));
					}
				}
				IReturnType[] typeParameters = null;
				if (type.IsConstructedReturnType) {
					typeParameters = new IReturnType[type.CastToConstructedReturnType().TypeArguments.Count];
					type.CastToConstructedReturnType().TypeArguments.CopyTo(typeParameters, 0);
				}
				ResolveResult rr = CreateMemberResolveResult(typeVisitor.FindOverload(constructors, typeParameters, ((ObjectCreateExpression)expr).Parameters));
				if (rr != null) {
					rr.ResolvedType = type;
				}
				return rr;
			}
			return new ResolveResult(callingClass, callingMember, type);
		}
		
		ResolveResult ResolveMemberReferenceExpression(IReturnType type, FieldReferenceExpression fieldReferenceExpression)
		{
			IClass c;
			IMember member;
			if (type is TypeVisitor.NamespaceReturnType) {
				string combinedName;
				if (type.FullyQualifiedName == "")
					combinedName = fieldReferenceExpression.FieldName;
				else
					combinedName = type.FullyQualifiedName + "." + fieldReferenceExpression.FieldName;
				if (projectContent.NamespaceExists(combinedName)) {
					return new NamespaceResolveResult(callingClass, callingMember, combinedName);
				}
				c = GetClass(combinedName);
				if (c != null) {
					return new TypeResolveResult(callingClass, callingMember, c);
				}
				if (languageProperties.ImportModules) {
					// go through the members of the modules
					foreach (object o in projectContent.GetNamespaceContents(type.FullyQualifiedName)) {
						member = o as IMember;
						if (member != null && IsSameName(member.Name, fieldReferenceExpression.FieldName)) {
							return CreateMemberResolveResult(member);
						}
					}
				}
				return null;
			}
			member = GetMember(type, fieldReferenceExpression.FieldName);
			if (member != null)
				return CreateMemberResolveResult(member);
			c = type.GetUnderlyingClass();
			if (c != null) {
				foreach (IClass baseClass in c.ClassInheritanceTree) {
					List<IClass> innerClasses = baseClass.InnerClasses;
					if (innerClasses != null) {
						foreach (IClass innerClass in innerClasses) {
							if (IsSameName(innerClass.Name, fieldReferenceExpression.FieldName)) {
								return new TypeResolveResult(callingClass, callingMember, innerClass);
							}
						}
					}
				}
			}
			return ResolveMethod(type, fieldReferenceExpression.FieldName);
		}
		
		public TextReader ExtractCurrentMethod(string fileContent)
		{
			if (callingMember == null)
				return null;
			return ExtractMethod(fileContent, callingMember, language, caretLine);
		}
		
		/// <summary>
		/// Creates a new class containing only the specified member.
		/// This is useful because we only want to parse current method for local variables,
		/// as all fields etc. are already prepared in the AST.
		/// </summary>
		public static TextReader ExtractMethod(string fileContent, IMember member,
		                                       NR.SupportedLanguage language, int caretLine)
		{
			// As the parse information is always some seconds old, the end line could be wrong
			// if the user just inserted a line in the method.
			// We can ignore that case because it is sufficient for the parser when the first part of the
			// method body is ok.
			// Since we are operating directly on the edited buffer, the parser might not be
			// able to resolve invalid declarations.
			// We can ignore even that because the 'invalid line' is the line the user is currently
			// editing, and the declarations he is using are always above that line.
			
			
			// The ExtractMethod-approach has the advantage that the method contents do not have
			// do be parsed and stored in memory before they are needed.
			// Previous SharpDevelop versions always stored the SharpRefactory[VB] parse tree as 'Tag'
			// to the AST CompilationUnit.
			// This approach doesn't need that, so one could even go and implement a special parser
			// mode that does not parse the method bodies for the normal run (in the ParserUpdateThread or
			// SolutionLoadThread). That could improve the parser's speed dramatically.
			
			if (member.Region.IsEmpty) return null;
			int startLine = member.Region.BeginLine;
			if (startLine < 1) return null;
			DomRegion bodyRegion;
			if (member is IMethodOrProperty) {
				bodyRegion = ((IMethodOrProperty)member).BodyRegion;
			} else if (member is IEvent) {
				bodyRegion = ((IEvent)member).BodyRegion;
			} else {
				return null;
			}
			if (bodyRegion.IsEmpty) return null;
			int endLine = bodyRegion.EndLine;
			
			// Fix for SD2-511 (Code completion in inserted line)
			if (language == NR.SupportedLanguage.CSharp) {
				// Do not do this for VB: the parser does not correct create the
				// ForEachStatement when the method in truncated in the middle
				// VB does not have the "inserted line looks like variable declaration"-problem
				// anyways.
				if (caretLine > startLine && caretLine < endLine)
					endLine = caretLine;
			}
			
			int offset = 0;
			for (int i = 0; i < startLine - 1; ++i) { // -1 because the startLine must be included
				offset = fileContent.IndexOf('\n', offset) + 1;
				if (offset <= 0) return null;
			}
			int startOffset = offset;
			for (int i = startLine - 1; i < endLine; ++i) {
				int newOffset = fileContent.IndexOf('\n', offset) + 1;
				if (newOffset <= 0) break;
				offset = newOffset;
			}
			int length = offset - startOffset;
			string classDecl, endClassDecl;
			if (language == NR.SupportedLanguage.VBNet) {
				classDecl = "Class A";
				endClassDecl = "End Class\n";
			} else {
				classDecl = "class A {";
				endClassDecl = "}\n";
			}
			System.Text.StringBuilder b = new System.Text.StringBuilder(classDecl, length + classDecl.Length + endClassDecl.Length + startLine - 1);
			b.Append('\n', startLine - 1);
			b.Append(fileContent, startOffset, length);
			b.Append(endClassDecl);
			return new System.IO.StringReader(b.ToString());
		}
		
		#region Resolve Identifier
		ResolveResult ResolveIdentifier(string identifier, NR.Location position, ExpressionContext context)
		{
			ResolveResult result = ResolveIdentifierInternal(identifier, position);
			if (result is TypeResolveResult)
				return result;
			
			ResolveResult result2 = null;
			
			IReturnType t = SearchType(identifier, position);
			if (t != null) {
				result2 = new TypeResolveResult(callingClass, callingMember, t);
			} else {
				if (callingClass != null) {
					if (callingMember is IMethod) {
						foreach (ITypeParameter typeParameter in (callingMember as IMethod).TypeParameters) {
							if (IsSameName(identifier, typeParameter.Name)) {
								return new TypeResolveResult(callingClass, callingMember, new GenericReturnType(typeParameter));
							}
						}
					}
					foreach (ITypeParameter typeParameter in callingClass.TypeParameters) {
						if (IsSameName(identifier, typeParameter.Name)) {
							return new TypeResolveResult(callingClass, callingMember, new GenericReturnType(typeParameter));
						}
					}
				}
			}
			
			if (result == null)  return result2;
			if (result2 == null) return result;
			if (context == ExpressionContext.Type)
				return result2;
			return new MixedResolveResult(result, result2);
		}
		
		IField CreateLocalVariableField(LocalLookupVariable var)
		{
			IReturnType type = GetVariableType(var);
			IField f = new DefaultField.LocalVariableField(type, var.Name, new DomRegion(var.StartPos, var.EndPos), callingClass);
			if (var.IsConst) {
				f.Modifiers |= ModifierEnum.Const;
			}
			return f;
		}
		
		ResolveResult ResolveIdentifierInternal(string identifier, NR.Location position)
		{
			if (callingMember != null) { // LocalResolveResult requires callingMember to be set
				LocalLookupVariable var = SearchVariable(identifier, position);
				if (var != null) {
					return new LocalResolveResult(callingMember, CreateLocalVariableField(var));
				}
				IParameter para = SearchMethodParameter(identifier);
				if (para != null) {
					IField field = new DefaultField.ParameterField(para.ReturnType, para.Name, para.Region, callingClass);
					return new LocalResolveResult(callingMember, field);
				}
				if (IsSameName(identifier, "value")) {
					IProperty property = callingMember as IProperty;
					if (property != null && property.SetterRegion.IsInside(position.Line, position.Column)) {
						IField field = new DefaultField.ParameterField(property.ReturnType, "value", property.Region, callingClass);
						return new LocalResolveResult(callingMember, field);
					}
				}
			}
			if (callingClass != null) {
				IMember member = GetMember(callingClass.DefaultReturnType, identifier);
				if (member != null) {
					return CreateMemberResolveResult(member);
				}
				ResolveResult result = ResolveMethod(callingClass.DefaultReturnType, identifier);
				if (result != null)
					return result;
			}
			
			// try if there exists a static member in outer classes named typeName
			List<IClass> classes = cu.GetOuterClasses(position.Line, position.Column);
			foreach (IClass c in classes) {
				IMember member = GetMember(c.DefaultReturnType, identifier);
				if (member != null && member.IsStatic) {
					return new MemberResolveResult(callingClass, callingMember, member);
				}
			}
			
			string namespaceName = SearchNamespace(identifier, position);
			if (namespaceName != null && namespaceName.Length > 0) {
				return new NamespaceResolveResult(callingClass, callingMember, namespaceName);
			}
			
			if (languageProperties.CanImportClasses) {
				foreach (IUsing @using in cu.Usings) {
					foreach (string import in @using.Usings) {
						IClass c = GetClass(import);
						if (c != null) {
							IMember member = GetMember(c.DefaultReturnType, identifier);
							if (member != null) {
								return CreateMemberResolveResult(member);
							}
							ResolveResult result = ResolveMethod(c.DefaultReturnType, identifier);
							if (result != null)
								return result;
						}
					}
				}
			}
			
			if (languageProperties.ImportModules) {
				ArrayList list = new ArrayList();
				CtrlSpaceResolveHelper.AddImportedNamespaceContents(list, cu, callingClass);
				foreach (object o in list) {
					IClass c = o as IClass;
					if (c != null && IsSameName(identifier, c.Name)) {
						return new TypeResolveResult(callingClass, callingMember, c);
					}
					IMember member = o as IMember;
					if (member != null && IsSameName(identifier, member.Name)) {
						if (member is IMethod) {
							return new MethodResolveResult(callingClass, callingMember, member.DeclaringType.DefaultReturnType, member.Name);
						} else {
							return CreateMemberResolveResult(member);
						}
					}
				}
			}
			
			return null;
		}
		#endregion
		
		private ResolveResult CreateMemberResolveResult(IMember member)
		{
			if (member == null) return null;
			return new MemberResolveResult(callingClass, callingMember, member);
		}
		
		#region ResolveMethod
		ResolveResult ResolveMethod(IReturnType type, string identifier)
		{
			if (type == null)
				return null;
			foreach (IMethod method in type.GetMethods()) {
				if (IsSameName(identifier, method.Name))
					return new MethodResolveResult(callingClass, callingMember, type, method.Name);
			}
			if (languageProperties.SupportsExtensionMethods && callingClass != null) {
				ArrayList list = new ArrayList();
				ResolveResult.AddExtensions(languageProperties, list, callingClass, type);
				foreach (IMethodOrProperty mp in list) {
					if (mp is IMethod && IsSameName(mp.Name, identifier)) {
						return new MethodResolveResult(callingClass, callingMember, type, mp.Name);
					}
				}
			}
			return null;
		}
		#endregion
		
		Expression SpecialConstructs(string expression)
		{
			if (language == NR.SupportedLanguage.VBNet) {
				// MyBase and MyClass are no expressions, only MyBase.Identifier and MyClass.Identifier
				if ("mybase".Equals(expression, StringComparison.InvariantCultureIgnoreCase)) {
					return new BaseReferenceExpression();
				} else if ("myclass".Equals(expression, StringComparison.InvariantCultureIgnoreCase)) {
					return new ClassReferenceExpression();
				} // Global is handled in Resolve() because we don't need an expression for that
			} else if (language == NR.SupportedLanguage.CSharp) {
				// generic type names are no expressions, only property access on them is an expression
				if (expression.EndsWith(">")) {
					FieldReferenceExpression expr = ParseExpression(expression + ".Prop") as FieldReferenceExpression;
					if (expr != null) {
						return expr.TargetObject;
					}
				}
				return null;
			}
			return null;
		}
		
		public bool IsSameName(string name1, string name2)
		{
			return languageProperties.NameComparer.Equals(name1, name2);
		}
		
		bool IsInside(NR.Location between, NR.Location start, NR.Location end)
		{
			if (between.Y < start.Y || between.Y > end.Y) {
				return false;
			}
			if (between.Y > start.Y) {
				if (between.Y < end.Y) {
					return true;
				}
				// between.Y == end.Y
				return between.X <= end.X;
			}
			// between.Y == start.Y
			if (between.X < start.X) {
				return false;
			}
			// start is OK and between.Y <= end.Y
			return between.Y < end.Y || between.X <= end.X;
		}
		
		IMember GetCurrentMember()
		{
			if (callingClass == null)
				return null;
			foreach (IMethod method in callingClass.Methods) {
				if (method.Region.IsInside(caretLine, caretColumn) || method.BodyRegion.IsInside(caretLine, caretColumn)) {
					return method;
				}
			}
			foreach (IProperty property in callingClass.Properties) {
				if (property.Region.IsInside(caretLine, caretColumn) || property.BodyRegion.IsInside(caretLine, caretColumn)) {
					return property;
				}
			}
			return null;
		}
		
		/// <remarks>
		/// use the usings to find the correct name of a namespace
		/// </remarks>
		public string SearchNamespace(string name, NR.Location position)
		{
			return projectContent.SearchNamespace(name, callingClass, cu, position.Line, position.Column);
		}
		
		public IClass GetClass(string fullName)
		{
			return projectContent.GetClass(fullName, 0);
		}
		
		/// <remarks>
		/// use the usings and the name of the namespace to find a class
		/// </remarks>
		public IClass SearchClass(string name, NR.Location position)
		{
			IReturnType t = SearchType(name, position);
			return (t != null) ? t.GetUnderlyingClass() : null;
		}
		
		public IReturnType SearchType(string name, NR.Location position)
		{
			if (position.IsEmpty)
				return projectContent.SearchType(new SearchTypeRequest(name, 0, callingClass, cu, caretLine, caretColumn)).Result;
			else
				return projectContent.SearchType(new SearchTypeRequest(name, 0, callingClass, cu, position.Line, position.Column)).Result;
		}
		
		#region Helper for TypeVisitor
		#region SearchMethod
		
		public List<IMethod> SearchMethod(string memberName)
		{
			List<IMethod> methods = SearchMethod(callingClass.DefaultReturnType, memberName);
			if (methods.Count > 0)
				return methods;
			
			if (languageProperties.CanImportClasses) {
				foreach (IUsing @using in cu.Usings) {
					foreach (string import in @using.Usings) {
						IClass c = projectContent.GetClass(import, 0);
						if (c != null) {
							methods = SearchMethod(c.DefaultReturnType, memberName);
							if (methods.Count > 0)
								return methods;
						}
					}
				}
			}
			
			if (languageProperties.ImportModules) {
				ArrayList list = new ArrayList();
				CtrlSpaceResolveHelper.AddImportedNamespaceContents(list, cu, callingClass);
				foreach (object o in list) {
					IMethod m = o as IMethod;
					if (m != null && IsSameName(m.Name, memberName)) {
						methods.Add(m);
					}
				}
			}
			return methods;
		}
		
		/// <summary>
		/// Gets the list of methods on the return type that have the specified name.
		/// </summary>
		public List<IMethod> SearchMethod(IReturnType type, string memberName)
		{
			List<IMethod> methods = new List<IMethod>();
			if (type == null)
				return methods;
			
			bool isClassInInheritanceTree = false;
			if (callingClass != null)
				isClassInInheritanceTree = callingClass.IsTypeInInheritanceTree(type.GetUnderlyingClass());
			
			if (language == NR.SupportedLanguage.VBNet && IsSameName(memberName, "New")) {
				foreach (IMethod m in type.GetMethods()) {
					if (m.IsConstructor && m.IsAccessible(callingClass, isClassInInheritanceTree)) {
						methods.Add(m);
					}
				}
			}
			
			foreach (IMethod m in type.GetMethods()) {
				if (IsSameName(m.Name, memberName)
				    && m.IsAccessible(callingClass, isClassInInheritanceTree)
				   ) {
					methods.Add(m);
				}
			}
			
			return methods;
		}
		
		public IList<IMethod> SearchExtensionMethods(string name)
		{
			List<IMethod> results = new List<IMethod>();
			foreach (IMethodOrProperty m in SearchAllExtensionMethods()) {
				if (IsSameName(name, m.Name)) {
					results.Add((IMethod)m);
				}
			}
			return results;
		}
		
		ReadOnlyCollection<IMethodOrProperty> cachedExtensionMethods;
		IClass cachedExtensionMethods_LastClass; // invalidate cache when callingClass != LastClass
		
		static readonly ReadOnlyCollection<IMethodOrProperty> emptyMethodOrPropertyList = new ReadOnlyCollection<IMethodOrProperty>(new IMethodOrProperty[0]);
		
		public ReadOnlyCollection<IMethodOrProperty> SearchAllExtensionMethods()
		{
			if (callingClass == null)
				return emptyMethodOrPropertyList;
			if (callingClass != cachedExtensionMethods_LastClass) {
				cachedExtensionMethods_LastClass = callingClass;
				cachedExtensionMethods = new ReadOnlyCollection<IMethodOrProperty>(CtrlSpaceResolveHelper.FindAllExtensions(languageProperties, callingClass));
			}
			return cachedExtensionMethods;
		}
		#endregion
		
		#region SearchMember
		// no methods or indexer
		public IReturnType SearchMember(IReturnType type, string memberName)
		{
			if (type == null)
				return null;
			IMember member = GetMember(type, memberName);
			if (member == null)
				return null;
			else
				return member.ReturnType;
		}
		
		public IMember GetMember(IReturnType type, string memberName)
		{
			if (type == null)
				return null;
			bool isClassInInheritanceTree = false;
			if (callingClass != null)
				isClassInInheritanceTree = callingClass.IsTypeInInheritanceTree(type.GetUnderlyingClass());
			foreach (IProperty p in type.GetProperties()) {
				if (IsSameName(p.Name, memberName)) {
					return p;
				}
			}
			foreach (IField f in type.GetFields()) {
				if (IsSameName(f.Name, memberName)) {
					return f;
				}
			}
			foreach (IEvent e in type.GetEvents()) {
				if (IsSameName(e.Name, memberName)) {
					return e;
				}
			}
			return null;
		}
		#endregion
		
		#region DynamicLookup
		/// <remarks>
		/// does the dynamic lookup for the identifier
		/// </remarks>
		public IReturnType DynamicLookup(string identifier, NR.Location position)
		{
			ResolveResult rr = ResolveIdentifierInternal(identifier, position);
			if (rr is NamespaceResolveResult) {
				return new TypeVisitor.NamespaceReturnType((rr as NamespaceResolveResult).Name);
			}
			return (rr != null) ? rr.ResolvedType : null;
		}
		
		IParameter SearchMethodParameter(string parameter)
		{
			IMethodOrProperty method = callingMember as IMethodOrProperty;
			if (method == null) {
				return null;
			}
			foreach (IParameter p in method.Parameters) {
				if (IsSameName(p.Name, parameter)) {
					return p;
				}
			}
			return null;
		}
		
		IReturnType GetVariableType(LocalLookupVariable v)
		{
			if (v == null) {
				return null;
			}
			
			if (v.TypeRef == null || v.TypeRef.IsNull || v.TypeRef.Type == "var") {
				if (v.IsLoopVariable) {
					return new ElementReturnType(this.projectContent,
					                             new InferredReturnType(v.Initializer, this));
				} else {
					return new InferredReturnType(v.Initializer, this);
				}
			} else {
				return TypeVisitor.CreateReturnType(v.TypeRef, this);
			}
		}
		
		LocalLookupVariable SearchVariable(string name, NR.Location position)
		{
			if (lookupTableVisitor == null || !lookupTableVisitor.Variables.ContainsKey(name))
				return null;
			List<LocalLookupVariable> variables = lookupTableVisitor.Variables[name];
			if (variables.Count <= 0) {
				return null;
			}
			
			foreach (LocalLookupVariable v in variables) {
				if (IsInside(position, v.StartPos, v.EndPos)) {
					return v;
				}
			}
			return null;
		}
		#endregion
		#endregion
		
		IClass GetPrimitiveClass(string systemType, string newName)
		{
			IClass c = projectContent.GetClass(systemType, 0);
			if (c == null)
				return null;
			DefaultClass c2 = new DefaultClass(c.CompilationUnit, newName);
			c2.ClassType = c.ClassType;
			c2.Modifiers = c.Modifiers;
			c2.Documentation = c.Documentation;
			c2.BaseTypes.AddRange(c.BaseTypes);
			c2.Methods.AddRange(c.Methods);
			c2.Fields.AddRange(c.Fields);
			c2.Properties.AddRange(c.Properties);
			c2.Events.AddRange(c.Events);
			return c2;
		}
		
		static void AddCSharpKeywords(ArrayList ar, BitArray keywords)
		{
			for (int i = 0; i < keywords.Length; i++) {
				if (keywords[i]) {
					ar.Add(NR.Parser.CSharp.Tokens.GetTokenString(i));
				}
			}
		}
		
		public ArrayList CtrlSpace(int caretLine, int caretColumn, ParseInformation parseInfo, string fileContent, ExpressionContext context)
		{
			if (!Initialize(parseInfo, caretLine, caretColumn))
				return null;
			
			ArrayList result = new ArrayList();
			if (language == NR.SupportedLanguage.VBNet) {
				foreach (KeyValuePair<string, string> pair in TypeReference.PrimitiveTypesVB) {
					if ("System." + pair.Key != pair.Value) {
						IClass c = GetPrimitiveClass(pair.Value, pair.Key);
						if (c != null) result.Add(c);
					}
				}
				result.Add("Global");
				result.Add("New");
				CtrlSpaceInternal(result, fileContent);
			} else {
				if (context == ExpressionContext.TypeDeclaration) {
					AddCSharpKeywords(result, NR.Parser.CSharp.Tokens.TypeLevel);
					AddCSharpPrimitiveTypes(result);
					CtrlSpaceInternal(result, fileContent);
				} else if (context == ExpressionContext.InterfaceDeclaration) {
					AddCSharpKeywords(result, NR.Parser.CSharp.Tokens.InterfaceLevel);
					AddCSharpPrimitiveTypes(result);
					CtrlSpaceInternal(result, fileContent);
				} else if (context == ExpressionContext.StatementStart) {
					result.Add("var");
					AddCSharpKeywords(result, NR.Parser.CSharp.Tokens.StatementStart);
					AddCSharpPrimitiveTypes(result);
					CtrlSpaceInternal(result, fileContent);
				} else if (context == ExpressionContext.Global) {
					AddCSharpKeywords(result, NR.Parser.CSharp.Tokens.GlobalLevel);
				} else if (context == ExpressionContext.InterfacePropertyDeclaration) {
					result.Add("get");
					result.Add("set");
				} else if (context == ExpressionContext.BaseConstructorCall) {
					result.Add("this");
					result.Add("base");
				} else if (context == ExpressionContext.PropertyDeclaration) {
					AddCSharpKeywords(result, NR.Parser.CSharp.Tokens.InPropertyDeclaration);
				} else if (context == ExpressionContext.EventDeclaration) {
					AddCSharpKeywords(result, NR.Parser.CSharp.Tokens.InEventDeclaration);
				} else if (context == ExpressionContext.FullyQualifiedType) {
					cu.ProjectContent.AddNamespaceContents(result, "", languageProperties, true);
				} else if (context == ExpressionContext.ParameterType || context == ExpressionContext.FirstParameterType) {
					result.Add("ref");
					result.Add("out");
					result.Add("params");
					if (context == ExpressionContext.FirstParameterType && languageProperties.SupportsExtensionMethods) {
						if (callingMember != null && callingMember.IsStatic) {
							result.Add("this");
						}
					}
					AddCSharpPrimitiveTypes(result);
					CtrlSpaceInternal(result, fileContent);
				} else if (context == ExpressionContext.Default) {
					AddCSharpKeywords(result, NR.Parser.CSharp.Tokens.ExpressionStart);
					AddCSharpPrimitiveTypes(result);
					CtrlSpaceInternal(result, fileContent);
				} else {
					// e.g. some ExpressionContext.TypeDerivingFrom()
					AddCSharpPrimitiveTypes(result);
					CtrlSpaceInternal(result, fileContent);
				}
			}
			return result;
		}
		
		void AddCSharpPrimitiveTypes(ArrayList result)
		{
			foreach (KeyValuePair<string, string> pair in TypeReference.PrimitiveTypesCSharp) {
				IClass c = GetPrimitiveClass(pair.Value, pair.Key);
				if (c != null) result.Add(c);
			}
		}
		
		void CtrlSpaceInternal(ArrayList result, string fileContent)
		{
			lookupTableVisitor = new LookupTableVisitor(language);
			
			if (callingMember != null) {
				CompilationUnit parsedCu = ParseCurrentMemberAsCompilationUnit(fileContent);
				if (parsedCu != null) {
					lookupTableVisitor.VisitCompilationUnit(parsedCu, null);
				}
			}
			
			CtrlSpaceResolveHelper.AddContentsFromCalling(result, callingClass, callingMember);
			
			foreach (KeyValuePair<string, List<LocalLookupVariable>> pair in lookupTableVisitor.Variables) {
				if (pair.Value != null && pair.Value.Count > 0) {
					foreach (LocalLookupVariable v in pair.Value) {
						if (IsInside(new NR.Location(caretColumn, caretLine), v.StartPos, v.EndPos)) {
							// convert to a field for display
							result.Add(CreateLocalVariableField(v));
							break;
						}
					}
				}
			}
			if (callingMember is IProperty) {
				IProperty property = (IProperty)callingMember;
				if (property.SetterRegion.IsInside(caretLine, caretColumn)) {
					result.Add(new DefaultField.ParameterField(property.ReturnType, "value", property.Region, callingClass));
				}
			}
			
			CtrlSpaceResolveHelper.AddImportedNamespaceContents(result, cu, callingClass);
		}
	}
}
