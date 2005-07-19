// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Andrea Paatz" email="andrea@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.NRefactory.Parser.AST;
using ICSharpCode.NRefactory.Parser;

namespace ICSharpCode.SharpDevelop.Dom.NRefactoryResolver
{
	public class NRefactoryResolver : IResolver
	{
		ICompilationUnit cu;
		IClass callingClass;
		IMember callingMember;
		ICSharpCode.NRefactory.Parser.LookupTableVisitor lookupTableVisitor;
		IProjectContent projectContent = null;
		
		SupportedLanguages language;
		
		int caretLine;
		int caretColumn;
		
		public SupportedLanguages Language {
			get {
				return language;
			}
		}
		
		public IProjectContent ProjectContent {
			get {
				return projectContent;
			}
			set {
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
		
		LanguageProperties languageProperties;
		
		public LanguageProperties LanguageProperties {
			get {
				return languageProperties;
			}
		}
		
		public NRefactoryResolver(SupportedLanguages language)
		{
			this.language = language;
			this.projectContent = ParserService.CurrentProjectContent;
			switch (language) {
				case SupportedLanguages.CSharp:
					languageProperties = LanguageProperties.CSharp;
					break;
				case SupportedLanguages.VBNet:
					languageProperties = LanguageProperties.VBNet;
					break;
				default:
					throw new NotSupportedException("The language " + language + " is not supported in the resolver");
			}
		}
		
		Expression ParseExpression(string expression)
		{
			using (ICSharpCode.NRefactory.Parser.IParser p = ParserFactory.CreateParser(language, new System.IO.StringReader(expression))) {
				return p.ParseExpression();
			}
		}
		
		public ResolveResult Resolve(string expression,
		                             int caretLineNumber,
		                             int caretColumn,
		                             string fileName,
		                             string fileContent)
		{
			if (expression == null) {
				expression = "";
			}
			expression = expression.TrimStart(null);
			
			this.caretLine   = caretLineNumber;
			this.caretColumn = caretColumn;
			
			ParseInformation parseInfo = ParserService.GetParseInformation(fileName);
			if (parseInfo == null) {
				return null;
			}
			Expression expr = null;
			if (language == SupportedLanguages.VBNet) {
				if (expression == "") {
					if ((expr = WithResolve()) == null) {
						return null;
					}
				} else if ("global".Equals(expression, StringComparison.InvariantCultureIgnoreCase)) {
					return new NamespaceResolveResult(null, null, "");
				}
			}
			if (expr == null) {
				expr = SpecialConstructs(expression);
				if (expr == null) {
					expr = ParseExpression(expression);
					if (expr == null) {
						return null;
					}
				}
			}
			lookupTableVisitor = new LookupTableVisitor(languageProperties.NameComparer);
			
			//NRefactoryASTConvertVisitor cSharpVisitor = new NRefactoryASTConvertVisitor(parseInfo.MostRecentCompilationUnit != null ? parseInfo.MostRecentCompilationUnit.ProjectContent : null);
			cu = parseInfo.MostRecentCompilationUnit; //(ICompilationUnit)cSharpVisitor.Visit(fileCompilationUnit, null);
			
			if (cu != null) {
				callingClass = cu.GetInnermostClass(caretLine, caretColumn);
				cu.FileName = fileName;
			}
			callingMember = GetCurrentMember();
			if (callingMember != null) {
				System.IO.TextReader content = ExtractMethod(fileContent, callingMember);
				if (content != null) {
					ICSharpCode.NRefactory.Parser.IParser p = ParserFactory.CreateParser(language, content);
					p.Parse();
					lookupTableVisitor.Visit(p.CompilationUnit, null);
				}
			}
			
			return ResolveInternal(expr);
		}
		
		ResolveResult ResolveInternal(Expression expr)
		{
			TypeVisitor typeVisitor = new TypeVisitor(this);
			IReturnType type;
			
			if (expr is PrimitiveExpression) {
				if (((PrimitiveExpression)expr).Value is int)
					return null;
			} else if (expr is InvocationExpression) {
				IMethod method = typeVisitor.GetMethod(expr as InvocationExpression, null);
				if (method != null) {
					return CreateMemberResolveResult(method);
				} else {
					// InvocationExpression can also be a delegate/event call
					ResolveResult invocationTarget = ResolveInternal((expr as InvocationExpression).TargetObject);
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
			} else if (expr is FieldReferenceExpression) {
				FieldReferenceExpression fieldReferenceExpression = (FieldReferenceExpression)expr;
				if (fieldReferenceExpression.FieldName == null || fieldReferenceExpression.FieldName.Length == 0) {
					// NRefactory creates this "dummy" fieldReferenceExpression when it should
					// parse a primitive type name (int, short; Integer, Decimal)
					if (fieldReferenceExpression.TargetObject is TypeReferenceExpression) {
						type = TypeVisitor.CreateReturnType(((TypeReferenceExpression)fieldReferenceExpression.TargetObject).TypeReference, this);
						if (type != null) {
							IClass c = type.GetUnderlyingClass();
							if (c != null)
								return new TypeResolveResult(callingClass, callingMember, type, c);
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
				ResolveResult result = ResolveIdentifier(((IdentifierExpression)expr).Identifier);
				if (result != null)
					return result;
			} else if (expr is TypeReferenceExpression) {
				type = TypeVisitor.CreateReturnType(((TypeReferenceExpression)expr).TypeReference, this);
				if (type != null) {
					IClass c = type.GetUnderlyingClass();
					if (c != null)
						return new TypeResolveResult(callingClass, callingMember, type, c);
				}
			}
			type = expr.AcceptVisitor(typeVisitor, null) as IReturnType;
			
			if (type == null || type.FullyQualifiedName == "") {
				return null;
			}
			if (expr is ObjectCreateExpression) {
				ArrayList constructors = new ArrayList();
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
				return CreateMemberResolveResult(typeVisitor.FindOverload(constructors, ((ObjectCreateExpression)expr).Parameters, null));
			}
			return new ResolveResult(callingClass, callingMember, type);
		}
		
		ResolveResult ResolveMemberReferenceExpression(IReturnType type, FieldReferenceExpression fieldReferenceExpression)
		{
			IClass c;
			IMember member;
			TypeVisitor.NamespaceReturnType namespaceRT = type as TypeVisitor.NamespaceReturnType;
			if (namespaceRT != null) {
				string combinedName = namespaceRT.FullyQualifiedName + "." + fieldReferenceExpression.FieldName;
				if (projectContent.NamespaceExists(combinedName)) {
					return new NamespaceResolveResult(callingClass, callingMember, combinedName);
				}
				c = projectContent.GetClass(combinedName);
				if (c != null) {
					return new TypeResolveResult(callingClass, callingMember, c);
				}
				if (languageProperties.ImportModules) {
					// go through the members of the modules
					foreach (object o in projectContent.GetNamespaceContents(namespaceRT.FullyQualifiedName)) {
						member = o as IMember;
						if (member != null && IsSameName(member.Name, fieldReferenceExpression.FieldName))
							return CreateMemberResolveResult(member);
					}
				}
				return null;
			}
			member = GetMember(type, fieldReferenceExpression.FieldName);
			if (member != null)
				return CreateMemberResolveResult(member);
			c = type.GetUnderlyingClass();
			if (c != null) {
				List<IClass> innerClasses = c.InnerClasses;
				if (innerClasses != null) {
					foreach (IClass innerClass in innerClasses) {
						if (IsSameName(innerClass.Name, fieldReferenceExpression.FieldName)) {
							return new TypeResolveResult(callingClass, callingMember, innerClass);
						}
					}
				}
			}
			return ResolveMethod(type, fieldReferenceExpression.FieldName);
		}
		
		/// <summary>
		/// Creates a new class containing only the specified member.
		/// This is useful because we only want to parse current method for local variables,
		/// as all fields etc. are already prepared in the AST.
		/// </summary>
		System.IO.TextReader ExtractMethod(string fileContent, IMember member)
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
			
			if (member.Region == null) return null;
			int startLine = member.Region.BeginLine;
			if (startLine < 1) return null;
			IRegion bodyRegion;
			if (member is IMethod) {
				bodyRegion = ((IMethod)member).BodyRegion;
			} else if (member is IProperty) {
				bodyRegion = ((IProperty)member).BodyRegion;
			} else if (member is IIndexer) {
				bodyRegion = ((IIndexer)member).BodyRegion;
			} else if (member is IEvent) {
				bodyRegion = ((IEvent)member).BodyRegion;
			} else {
				return null;
			}
			if (bodyRegion == null) return null;
			int endLine = bodyRegion.EndLine;
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
//			Console.WriteLine("<source from={0} to={1}>", startLine, endLine);
//			Console.Write(fileContent.Substring(startOffset, offset - startOffset));
//			Console.WriteLine("</source>");
			int length = offset - startOffset;
			string classDecl, endClassDecl;
			if (language == SupportedLanguages.VBNet) {
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
		ResolveResult ResolveIdentifier(string identifier)
		{
			string name = SearchNamespace(identifier, this.CompilationUnit);
			if (name != null && name != "") {
				return new NamespaceResolveResult(callingClass, callingMember, name);
			}
			
			ResolveResult result = ResolveIdentifierInternal(identifier);
			ResolveResult result2 = null;
			
			IClass c = SearchType(identifier, callingClass, cu);
			if (c != null) {
				result2 = new TypeResolveResult(callingClass, callingMember, c.DefaultReturnType, c);
			}
			
			if (result == null)  return result2;
			if (result2 == null) return result;
			return new MixedResolveResult(result, result2);
		}
		
		ResolveResult ResolveIdentifierInternal(string identifier)
		{
			if (callingMember != null) { // LocalResolveResult requires callingMember to be set
				LocalLookupVariable var = SearchVariable(identifier);
				if (var != null) {
					IReturnType type = GetVariableType(var);
					IField field = new DefaultField(type, identifier, ModifierEnum.None, new DefaultRegion(var.StartPos, var.EndPos), callingClass);
					return new LocalResolveResult(callingMember, field, false);
				}
				IParameter para = SearchMethodParameter(identifier);
				if (para != null) {
					IField field = new DefaultField(para.ReturnType, para.Name, ModifierEnum.None, para.Region, callingClass);
					return new LocalResolveResult(callingMember, field, true);
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
			List<IClass> classes = cu.GetOuterClasses(caretLine, caretColumn);
			foreach (IClass c2 in classes) {
				IMember member = GetMember(c2.DefaultReturnType, identifier);
				if (member != null && member.IsStatic) {
					return new MemberResolveResult(callingClass, callingMember, member);
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
					return new MethodResolveResult(callingClass, callingMember, type, identifier);
			}
			return null;
		}
		#endregion
		
		Expression WithResolve()
		{
			if (language != SupportedLanguages.VBNet) {
				return null;
			}
			Expression expr = null;
			// TODO :
//			if (lookupTableVisitor.WithStatements != null) {
//				foreach (WithStatement with in lookupTableVisitor.WithStatements) {
//					if (IsInside(new Point(caretColumn, caretLine), with.StartLocation, with.EndLocation)) {
//						expr = with.WithExpression;
//					}
//				}
//			}
			return expr;
		}
		
		Expression SpecialConstructs(string expression)
		{
			if (language == SupportedLanguages.VBNet) {
				// MyBase and MyClass are no expressions, only MyBase.Identifier and MyClass.Identifier
				if ("mybase".Equals(expression, StringComparison.InvariantCultureIgnoreCase)) {
					return new BaseReferenceExpression();
				} else if ("myclass".Equals(expression, StringComparison.InvariantCultureIgnoreCase)) {
					return new ClassReferenceExpression();
				} // Global is handled in Resolve() because we don't need an expression for that
			} else if (language == SupportedLanguages.CSharp) {
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
		
		bool IsInside(Point between, Point start, Point end)
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
				if (method.BodyRegion != null && method.BodyRegion.IsInside(caretLine, caretColumn)) {
					return method;
				}
			}
			foreach (IProperty property in callingClass.Properties) {
				if (property.BodyRegion != null && property.BodyRegion.IsInside(caretLine, caretColumn)) {
					return property;
				}
			}
			foreach (IIndexer indexer in callingClass.Indexer) {
				if (indexer.BodyRegion != null && indexer.BodyRegion.IsInside(caretLine, caretColumn)) {
					return indexer;
				}
			}
			return null;
		}
		
		/// <remarks>
		/// use the usings to find the correct name of a namespace
		/// </remarks>
		public string SearchNamespace(string name, ICompilationUnit unit)
		{
			return projectContent.SearchNamespace(name, unit, caretLine, caretColumn);
		}
		
		/// <remarks>
		/// use the usings and the name of the namespace to find a class
		/// </remarks>
		public IClass SearchType(string name, IClass curType)
		{
			return projectContent.SearchType(name, curType, caretLine, caretColumn);
		}
		
		/// <remarks>
		/// use the usings and the name of the namespace to find a class
		/// </remarks>
		public IClass SearchType(string name, IClass curType, ICompilationUnit unit)
		{
			return projectContent.SearchType(name, curType, unit, caretLine, caretColumn);
		}
		
		#region Helper for TypeVisitor
		#region SearchMethod
		/// <summary>
		/// Gets the list of methods on the return type that have the specified name.
		/// </summary>
		public ArrayList SearchMethod(IReturnType type, string memberName)
		{
			ArrayList methods = new ArrayList();
			if (type == null)
				return methods;
			
			bool isClassInInheritanceTree = false;
			if (callingClass != null)
				isClassInInheritanceTree = callingClass.IsTypeInInheritanceTree(type.GetUnderlyingClass());
			
			foreach (IMethod m in type.GetMethods()) {
				if (IsSameName(m.Name, memberName)
				    && m.IsAccessible(callingClass, isClassInInheritanceTree)
				   ) {
					methods.Add(m);
				}
			}
//			if (methods.Count == 0) {
//				foreach (IEvent m in type.GetEvents()) {
//					if (IsSameName(m.Name, memberName)
//					    && m.IsAccessible(callingClass, isClassInInheritanceTree)
//					   ) {
//						methods.Add(m);
//						break;
//					}
//				}
//			}
			return methods;
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
		public IReturnType DynamicLookup(string typeName)
		{
			// try if it exists a variable named typeName
			IReturnType variable = GetVariableType(SearchVariable(typeName));
			if (variable != null) {
				return variable;
			}
			
			if (callingClass == null) {
				return null;
			}
			
			// try if typeName is a method parameter
			IParameter parameter = SearchMethodParameter(typeName);
			if (parameter != null) {
				return parameter.ReturnType;
			}
			
			// check if typeName == value in set method of a property
			if (typeName == "value") {
				IProperty property = callingMember as IProperty;
				if (property != null && property.SetterRegion != null && property.SetterRegion.IsInside(caretLine, caretColumn)) {
					return property.ReturnType;
				}
			}
			
			// try if there exists a nonstatic member named typeName
			IReturnType t = SearchMember(callingClass.DefaultReturnType, typeName);
			if (t != null) {
				return t;
			}
			
			// try if there exists a static member in outer classes named typeName
			List<IClass> classes = cu.GetOuterClasses(caretLine, caretColumn);
			foreach (IClass c in classes) {
				IMember member = GetMember(c.DefaultReturnType, typeName);
				if (member != null && member.IsStatic) {
					return member.ReturnType;
				}
			}
			return null;
		}
		
		IParameter SearchMethodParameter(string parameter)
		{
			IMethod method = callingMember as IMethod;
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
			return TypeVisitor.CreateReturnType(v.TypeRef, this);
		}
		
		LocalLookupVariable SearchVariable(string name)
		{
			if (!lookupTableVisitor.Variables.ContainsKey(name))
				return null;
			List<LocalLookupVariable> variables = lookupTableVisitor.Variables[name];
			if (variables.Count <= 0) {
				return null;
			}
			
			foreach (LocalLookupVariable v in variables) {
				if (IsInside(new Point(caretColumn, caretLine), v.StartPos, v.EndPos)) {
					return v;
				}
			}
			return null;
		}
		#endregion
		#endregion
		
		public ArrayList CtrlSpace(int caretLine, int caretColumn, string fileName, string fileContent)
		{
			ArrayList result;
			if (language == SupportedLanguages.VBNet) {
				result = new ArrayList();
				foreach (string primitive in TypeReference.GetPrimitiveTypesVB()) {
					result.Add(Char.ToUpper(primitive[0]) + primitive.Substring(1));
				}
			} else {
				result = new ArrayList(TypeReference.GetPrimitiveTypes());
			}
			ParseInformation parseInfo = ParserService.GetParseInformation(fileName);
			if (parseInfo == null) {
				return null;
			}
			
			this.caretLine   = caretLine;
			this.caretColumn = caretColumn;
			
			lookupTableVisitor = new LookupTableVisitor(languageProperties.NameComparer);
			
			cu = parseInfo.MostRecentCompilationUnit;
			
			if (cu != null) {
				callingClass = cu.GetInnermostClass(caretLine, caretColumn);
				cu.FileName = fileName;
			}
			
			callingMember = GetCurrentMember();
			if (callingMember != null) {
				System.IO.TextReader content = ExtractMethod(fileContent, callingMember);
				if (content != null) {
					ICSharpCode.NRefactory.Parser.IParser p = ParserFactory.CreateParser(language, content);
					p.Parse();
					lookupTableVisitor.Visit(p.CompilationUnit, null);
				}
			}
			
			IMethod method = callingMember as IMethod;
			if (method != null) {
				foreach (IParameter p in method.Parameters) {
					result.Add(new DefaultField(p.ReturnType, p.Name, ModifierEnum.None, method.Region, callingClass));
				}
			}
			if (callingClass != null) {
				result.AddRange(projectContent.GetNamespaceContents(callingClass.Namespace));
			}
			
			bool inStatic = true;
			if (callingMember != null)
				inStatic = callingMember.IsStatic;
			
			if (callingClass != null) {
				if (!inStatic) {
					result.AddRange(callingClass.GetAccessibleMembers(callingClass, false));
				}
				result.AddRange(callingClass.GetAccessibleMembers(callingClass, true));
				result.AddRange(callingClass.GetAccessibleTypes(callingClass));
			}
			foreach (KeyValuePair<string, List<LocalLookupVariable>> pair in lookupTableVisitor.Variables) {
				if (pair.Value != null && pair.Value.Count > 0) {
					foreach (LocalLookupVariable v in pair.Value) {
						if (IsInside(new Point(caretColumn, caretLine), v.StartPos, v.EndPos)) {
							// convert to a field for display
							result.Add(new DefaultField(TypeVisitor.CreateReturnType(v.TypeRef, this), pair.Key, ModifierEnum.None, new DefaultRegion(v.StartPos, v.EndPos), callingClass));
							break;
						}
					}
				}
			}
			projectContent.AddNamespaceContents(result, "", languageProperties, true);
			foreach (IUsing u in cu.Usings) {
				if (u != null) {
					foreach (string name in u.Usings) {
						foreach (object o in projectContent.GetNamespaceContents(name)) {
							if (!(o is string))
								result.Add(o);
						}
					}
					foreach (string alias in u.Aliases.Keys) {
						result.Add(alias);
					}
				}
			}
			return result;
		}
	}
}
