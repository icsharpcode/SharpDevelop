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
		
		bool caseSensitive = true;
		SupportedLanguages language = SupportedLanguages.CSharp;
		
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
		
		public NRefactoryResolver(SupportedLanguages language)
		{
			this.language = language;
			this.projectContent = ParserService.CurrentProjectContent;
		}
		
		bool IsCaseSensitive(SupportedLanguages language)
		{
			switch (language) {
				case SupportedLanguages.CSharp:
					return true;
				case SupportedLanguages.VBNet:
					return false;
				default:
					throw new NotSupportedException("The language " + language + " is not supported in the resolver");
			}
		}
		
		public ResolveResult Resolve(string expression,
		                             int caretLineNumber,
		                             int caretColumn,
		                             string fileName)
		{
			caseSensitive = IsCaseSensitive(language);
			
			if (expression == null) {
				expression = "";
			}
			expression = expression.TrimStart(null);
			if (!caseSensitive) {
				expression = expression.ToLower();
			}
			
			this.caretLine     = caretLineNumber;
			this.caretColumn   = caretColumn;
			
			ParseInformation parseInfo = ParserService.GetParseInformation(fileName);
			if (parseInfo == null) {
				return null;
			}
			ICSharpCode.NRefactory.Parser.AST.CompilationUnit fileCompilationUnit = parseInfo.MostRecentCompilationUnit.Tag as ICSharpCode.NRefactory.Parser.AST.CompilationUnit;
			if (fileCompilationUnit == null) {
//				ICSharpCode.NRefactory.Parser.Parser fileParser = new ICSharpCode.NRefactory.Parser.Parser();
//				fileParser.Parse(new Lexer(new StringReader(fileContent)));
				return null;
			}
			Expression expr = null;
			if (expression == "") {
				if ((expr = WithResolve()) == null) {
					return null;
				}
			}
			if (expr == null) {
				expr = SpecialConstructs(expression);
				if (expr == null) {
					ICSharpCode.NRefactory.Parser.IParser p = ICSharpCode.NRefactory.Parser.ParserFactory.CreateParser(language, new System.IO.StringReader(expression));
					expr = p.ParseExpression();
					if (expr == null) {
						return null;
					}
				}
			}
			lookupTableVisitor = new LookupTableVisitor();
			lookupTableVisitor.Visit(fileCompilationUnit, null);
			
			NRefactoryASTConvertVisitor cSharpVisitor = new NRefactoryASTConvertVisitor(parseInfo.MostRecentCompilationUnit != null ? parseInfo.MostRecentCompilationUnit.ProjectContent : null);
			
			cu = (ICompilationUnit)cSharpVisitor.Visit(fileCompilationUnit, null);
			if (cu != null) {
				callingClass = cu.GetInnermostClass(caretLine, caretColumn);
			}
			callingMember = GetCurrentMember();
			
			TypeVisitor typeVisitor = new TypeVisitor(this);
			if (expr is InvocationExpression) {
				IMethod method = typeVisitor.GetMethod((InvocationExpression)expr, null);
				if (method != null)
					return new MemberResolveResult(callingClass, callingMember, method);
			} else if (expr is FieldReferenceExpression) {
				FieldReferenceExpression fieldReferenceExpression = (FieldReferenceExpression)expr;
				IReturnType returnType = fieldReferenceExpression.TargetObject.AcceptVisitor(typeVisitor, null) as IReturnType;
				if (returnType != null) {
					string name = SearchNamespace(returnType.FullyQualifiedName, this.CompilationUnit);
					if (name != null) {
						name += "." + fieldReferenceExpression.FieldName;
						string n = SearchNamespace(name, null);
						if (n != null) {
							return new NamespaceResolveResult(callingClass, callingMember, n);
						}
						IClass c = SearchType(name, this.CallingClass, this.CompilationUnit);
						if (c != null) {
							return new TypeResolveResult(callingClass, callingMember, new ReturnType(c.FullyQualifiedName), c);
						}
						return null;
					}
					IMember member = GetMember(returnType, fieldReferenceExpression.FieldName);
					if (member != null)
						return new MemberResolveResult(callingClass, callingMember, member);
					ResolveResult result = ResolveMethod(returnType, fieldReferenceExpression.FieldName);
					if (result != null)
						return result;
				}
			} else if (expr is IdentifierExpression) {
				ResolveResult result = ResolveIdentifier(((IdentifierExpression)expr).Identifier);
				if (result != null)
					return result;
			}
			IReturnType type = expr.AcceptVisitor(typeVisitor, null) as IReturnType;
			
			if (type == null || type.FullyQualifiedName == "" || type.PointerNestingLevel != 0) {
				return null;
			}
			if (type.ArrayDimensions != null && type.ArrayDimensions.Length > 0)
				return new ResolveResult(callingClass, callingMember, new ReturnType("System.Array"));
			else {
				return new ResolveResult(callingClass, callingMember, FixType(type));
			}
		}
		
		IReturnType FixType(IReturnType type)
		{
			IClass returnClass = SearchType(type.FullyQualifiedName, callingClass, cu);
			if (returnClass != null && returnClass.FullyQualifiedName != type.FullyQualifiedName)
				return new ReturnType(returnClass.FullyQualifiedName);
			else
				return type;
		}
		
		#region Resolve Identifier
		ResolveResult ResolveIdentifier(string identifier)
		{
			string name = SearchNamespace(identifier, this.CompilationUnit);
			if (name != null && name != "") {
				return new NamespaceResolveResult(callingClass, callingMember, name);
			}
			IClass c = SearchType(identifier, this.CallingClass, this.CompilationUnit);
			if (c != null) {
				return new TypeResolveResult(callingClass, callingMember, new ReturnType(c.FullyQualifiedName), c);
			}
			LocalLookupVariable var = SearchVariable(identifier);
			if (var != null) {
				IReturnType type = GetVariableType(var);
				IField field = new LocalVariableField(type, identifier, null, callingClass);
				return new LocalResolveResult(callingClass, callingMember, field, false);
			}
			IParameter para = SearchMethodParameter(identifier);
			if (para != null) {
				IField field = new LocalVariableField(para.ReturnType, para.Name, para.Region, callingClass);
				return new LocalResolveResult(callingClass, callingMember, field, true);
			}
			
			IMember member = GetMember(callingClass, identifier);
			if (member != null) {
				return new MemberResolveResult(callingClass, callingMember, member);
			}
			
			ResolveResult result = ResolveMethod(callingClass, identifier);
			if (result != null)
				return result;
			
			// try if there exists a static member in outer classes named typeName
			List<IClass> classes = cu.GetOuterClasses(caretLine, caretColumn);
			foreach (IClass c2 in classes) {
				member = GetMember(c2, identifier);
				if (member != null && member.IsStatic) {
					return new MemberResolveResult(callingClass, callingMember, member);
				}
			}
			return null;
		}
		
		private class LocalVariableField : AbstractField
		{
			public LocalVariableField(IReturnType type, string name, IRegion region, IClass declaringType) : base(declaringType)
			{
				this.returnType = type;
				this.FullyQualifiedName = name;
				this.region = region;
				this.modifiers = ModifierEnum.Private;
			}
		}
		#endregion
		
		#region ResolveMethod
		ResolveResult ResolveMethod(IReturnType type, string identifier)
		{
			if (type == null || type.PointerNestingLevel != 0) {
				return null;
			}
			IClass curType;
			if (type.ArrayDimensions != null && type.ArrayDimensions.Length > 0) {
				curType = SearchType("System.Array", null, null);
			} else {
				curType = SearchType(type.FullyQualifiedName, null, null);
				if (curType == null) {
					return null;
				}
			}
			return ResolveMethod(curType, identifier);
		}
		
		ResolveResult ResolveMethod(IClass c, string identifier)
		{
			foreach (IClass curType in c.ClassInheritanceTree) {
				foreach (IMethod method in c.Methods) {
					if (IsSameName(identifier, method.Name))
						return new MethodResolveResult(callingClass, callingMember, c, identifier);
				}
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
				if (expression == "mybase") {
					return new BaseReferenceExpression();
				} else if (expression == "myclass") {
					return new ClassReferenceExpression();
				}
			}
			return null;
		}
		
		bool IsSameName(string name1, string name2)
		{
			if (IsCaseSensitive(language)) {
				return name1 == name2;
			} else {
				return name1.ToLower() == name2.ToLower();
			}
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
			foreach (IProperty property in callingClass.Properties) {
				if (property.BodyRegion != null && property.BodyRegion.IsInside(caretLine, caretColumn)) {
					return property;
				}
			}
			foreach (IMethod method in callingClass.Methods) {
				if (method.BodyRegion != null && method.BodyRegion.IsInside(caretLine, caretColumn)) {
					return method;
				}
			}
			return null;
		}
		
		/// <remarks>
		/// use the usings to find the correct name of a namespace
		/// </remarks>
		public string SearchNamespace(string name, ICompilationUnit unit)
		{
			/*if (projectContent.NamespaceExists(name)) {
				return name;
			}
			
			if (CallingClass != null) {
				string callspace = this.CallingClass.Namespace;
				for (int split = callspace.Length; split > 0; split = callspace.LastIndexOf('.', split - 1)) {
					string fullname = callspace.Substring(0, split) + "." + name;
					if (projectContent.NamespaceExists(fullname)) {
						return fullname;
					}
				}
			}
			 */
			// TODO: look if all the stuff before is nessessary
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
			if (type == null || type.PointerNestingLevel != 0) {
				return new ArrayList(1);
			}
			IClass curType;
			if (type.ArrayDimensions != null && type.ArrayDimensions.Length > 0) {
				curType = SearchType("System.Array", null, null);
			} else {
				curType = SearchType(type.FullyQualifiedName, null, null);
				if (curType == null) {
					return new ArrayList(1);
				}
			}
			return SearchMethod(new ArrayList(), curType, memberName);
		}
		
		ArrayList SearchMethod(ArrayList methods, IClass curType, string memberName)
		{
			bool isClassInInheritanceTree = callingClass.IsTypeInInheritanceTree(curType);
			
			foreach (IMethod m in curType.Methods) {
				if (IsSameName(m.Name, memberName) &&
				    m.MustBeShown(callingClass, true, isClassInInheritanceTree) &&
				    !((m.Modifiers & ModifierEnum.Override) == ModifierEnum.Override)) {
					methods.Add(m);
				}
			}
			IClass baseClass = curType.BaseClass;
			if (baseClass != null) {
				return SearchMethod(methods, baseClass, memberName);
			}
			return methods;
		}
		#endregion
		
		#region SearchIndexer
		public ArrayList SearchIndexer(IReturnType type)
		{
			IClass curType = SearchType(type.FullyQualifiedName, null, null);
			if (curType != null) {
				return SearchIndexer(new ArrayList(), curType);
			}
			return new ArrayList(1);
		}
		
		ArrayList SearchIndexer(ArrayList indexer, IClass curType)
		{
			bool isClassInInheritanceTree = callingClass.IsTypeInInheritanceTree(curType);
			foreach (IIndexer i in curType.Indexer) {
				if (i.MustBeShown(callingClass, true, isClassInInheritanceTree) && !((i.Modifiers & ModifierEnum.Override) == ModifierEnum.Override)) {
					indexer.Add(i);
				}
			}
			IClass baseClass = curType.BaseClass;
			if (baseClass != null) {
				return SearchIndexer(indexer, baseClass);
			}
			return indexer;
		}
		#endregion
		
		#region SearchMember
		// no methods or indexer
		public IReturnType SearchMember(IReturnType type, string memberName)
		{
			if (type == null || memberName == null || memberName == "") {
				return null;
			}
			if (type.PointerNestingLevel != 0) {
				return null;
			}
			IClass curType;
			if (type.ArrayDimensions != null && type.ArrayDimensions.Length > 0) {
				curType = SearchType("System.Array", null, null);
			} else {
				curType = SearchType(type.FullyQualifiedName, callingClass, cu);
			}
			if (curType == null)
				return null;
			else
				return SearchMember(curType, memberName);
		}
		
		public IMember GetMember(IReturnType type, string memberName)
		{
			if (type == null || memberName == null || memberName == "") {
				return null;
			}
			if (type.PointerNestingLevel != 0) {
				return null;
			}
			IClass curType;
			if (type.ArrayDimensions != null && type.ArrayDimensions.Length > 0) {
				curType = SearchType("System.Array", null, null);
			} else {
				curType = SearchType(type.FullyQualifiedName, callingClass, cu);
			}
			if (curType == null)
				return null;
			else
				return GetMember(curType, memberName);
		}
		
		public IReturnType SearchMember(IClass curType, string memberName)
		{
			bool isClassInInheritanceTree = false;
			if (callingClass != null)
				isClassInInheritanceTree = callingClass.IsTypeInInheritanceTree(curType);
			foreach (IClass c in curType.InnerClasses) {
				if (IsSameName(c.Name, memberName) && c.IsAccessible(callingClass, isClassInInheritanceTree)) {
					return new ReturnType(c.FullyQualifiedName);
				}
			}
			IMember member = GetMember(curType, memberName);
			if (member == null)
				return null;
			else
				return member.ReturnType;
		}
		
		private IMember GetMember(IClass c, string memberName)
		{
			bool isClassInInheritanceTree = false;
			if (callingClass != null)
				isClassInInheritanceTree = callingClass.IsTypeInInheritanceTree(c);
			foreach (IClass curType in c.ClassInheritanceTree) {
				foreach (IProperty p in curType.Properties) {
					if (IsSameName(p.Name, memberName) && p.MustBeShown(callingClass, true, isClassInInheritanceTree)) {
						return p;
					}
				}
				foreach (IField f in curType.Fields) {
					if (IsSameName(f.Name, memberName) && f.MustBeShown(callingClass, true, isClassInInheritanceTree)) {
						return f;
					}
				}
				foreach (IEvent e in curType.Events) {
					if (IsSameName(e.Name, memberName) && e.MustBeShown(callingClass, true, isClassInInheritanceTree)) {
						return e;
					}
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
			IReturnType t = SearchMember(callingClass, typeName);
			if (t != null) {
				return t;
			}
			
			// try if there exists a static member in outer classes named typeName
			List<IClass> classes = cu.GetOuterClasses(caretLine, caretColumn);
			foreach (IClass c in classes) {
				IMember member = GetMember(c, typeName);
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
			IClass c = SearchType(v.TypeRef.SystemType, callingClass, cu);
			if (c != null) {
				return new ReturnType(c.FullyQualifiedName, v.TypeRef.RankSpecifier, v.TypeRef.PointerNestingLevel);
			} else {
				return new ReturnType(v.TypeRef);
			}
		}
		
		LocalLookupVariable SearchVariable(string name)
		{
			ArrayList variables = (ArrayList)lookupTableVisitor.variables[IsCaseSensitive(language) ? name : name.ToLower()];
			if (variables == null || variables.Count <= 0) {
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
		
		/*
		public ArrayList NewCompletion(int caretLine, int caretColumn, string fileName)
		{
			if (!IsCaseSensitive(language)) {
				caseSensitive = false;
			}
			ArrayList result = new ArrayList();
			ParseInformation parseInfo = ParserService.GetParseInformation(fileName);
			ICSharpCode.NRefactory.Parser.AST.CompilationUnit fileCompilationUnit = parseInfo.MostRecentCompilationUnit.Tag as ICSharpCode.NRefactory.Parser.AST.CompilationUnit;
			if (fileCompilationUnit == null) {
				return null;
			}
			NRefactoryASTConvertVisitor cSharpVisitor = new NRefactoryASTConvertVisitor(parseInfo.MostRecentCompilationUnit != null ? parseInfo.MostRecentCompilationUnit.ProjectContent : null);
			
			cu = (ICompilationUnit)cSharpVisitor.Visit(fileCompilationUnit, null);
			if (cu != null) {
				callingClass = cu.GetInnermostClass(caretLine, caretColumn);
				if (callingClass != null) {
					result.AddRange(projectContent.GetNamespaceContents(callingClass.Namespace));
				}
			}
			result.AddRange(projectContent.GetNamespaceContents(""));
			foreach (IUsing u in cu.Usings) {
				if (u != null && (u.Region == null || u.Region.IsInside(caretLine, caretColumn))) {
					foreach (string name in u.Usings) {
						result.AddRange(projectContent.GetNamespaceContents(name));
					}
					foreach (string alias in u.Aliases.Keys) {
						result.Add(alias);
					}
				}
			}
			return result;
		}*/
		
		public ArrayList CtrlSpace(int caretLine, int caretColumn, string fileName)
		{
			if (!IsCaseSensitive(language)) {
				caseSensitive = false;
			}
			ArrayList result = new ArrayList(TypeReference.GetPrimitiveTypes());
			ParseInformation parseInfo = ParserService.GetParseInformation(fileName);
			ICSharpCode.NRefactory.Parser.AST.CompilationUnit fileCompilationUnit = parseInfo.MostRecentCompilationUnit.Tag as ICSharpCode.NRefactory.Parser.AST.CompilationUnit;
			if (fileCompilationUnit == null) {
				return null;
			}
			lookupTableVisitor = new LookupTableVisitor();
			lookupTableVisitor.Visit(fileCompilationUnit, null);
			
			NRefactoryASTConvertVisitor cSharpVisitor = new NRefactoryASTConvertVisitor(parseInfo.MostRecentCompilationUnit != null ? parseInfo.MostRecentCompilationUnit.ProjectContent : null);
			cu = (ICompilationUnit)cSharpVisitor.Visit(fileCompilationUnit, null);
			if (cu != null) {
				callingClass = cu.GetInnermostClass(caretLine, caretColumn);
				if (callingClass != null) {
					IMethod method = callingMember as IMethod;
					if (method != null) {
						foreach (IParameter p in method.Parameters) {
							result.Add(new Field(new ReturnType(p.ReturnType.Name, p.ReturnType.ArrayDimensions, p.ReturnType.PointerNestingLevel), p.Name, Modifier.None, method.Region, callingClass));
						}
					}
					result.AddRange(projectContent.GetNamespaceContents(callingClass.Namespace));
					bool inStatic = true;
					if (callingMember != null)
						inStatic = callingMember.IsStatic;
					result.AddRange(callingClass.GetAccessibleMembers(callingClass, inStatic).ToArray());
					if (inStatic == false) {
						result.AddRange(callingClass.GetAccessibleMembers(callingClass, !inStatic).ToArray());
					}
				}
			}
			foreach (string name in lookupTableVisitor.variables.Keys) {
				ArrayList variables = (ArrayList)lookupTableVisitor.variables[name];
				if (variables != null && variables.Count > 0) {
					foreach (LocalLookupVariable v in variables) {
						if (IsInside(new Point(caretColumn, caretLine), v.StartPos, v.EndPos)) {
							// LocalLookupVariable in no known Type in DisplayBindings.TextEditor
							// so add Field for the Variables
							result.Add(new Field(new ReturnType(v.TypeRef), name, Modifier.None, new DefaultRegion(v.StartPos, v.EndPos), callingClass));
							break;
						}
					}
				}
			}
			result.AddRange(projectContent.GetNamespaceContents(""));
			foreach (IUsing u in cu.Usings) {
				if (u != null && (u.Region == null || u.Region.IsInside(caretLine, caretColumn))) {
					foreach (string name in u.Usings) {
						result.AddRange(projectContent.GetNamespaceContents(name));
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
