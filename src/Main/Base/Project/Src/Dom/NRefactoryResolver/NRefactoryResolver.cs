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
		ICSharpCode.NRefactory.Parser.LookupTableVisitor lookupTableVisitor;
		IProjectContent projectContent = null;
		
		public IProjectContent ProjectContent {
			get {
				return projectContent;
			}
			set {
				projectContent = value;
			}
		}
		
		bool showStatic = false;
		bool inNew = false;
		
		bool caseSensitive = true;
		SupportedLanguages language = SupportedLanguages.CSharp;
		
		int caretLine;
		int caretColumn;
		
		
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
		
		public bool ShowStatic {
			get {
				return showStatic;
			}
			
			set {
				showStatic = value;
			}
		}
		
		public NRefactoryResolver(SupportedLanguages language)
		{
			this.language = language;
			this.projectContent = ParserService.CurrentProjectContent;
		}
		
		bool IsCaseSensitive(SupportedLanguages language) {
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
			if (!IsCaseSensitive(language)) {
				caseSensitive = false;
			}
			if (expression == null) {
				expression = "";
			}
			expression = expression.TrimStart(null);
			if (!caseSensitive) {
				expression = expression.ToLower();
			}
			// disable the code completion for numbers like 3.47
			if (IsNumber(expression)) {
				return null;
			}
			
			if (expression.StartsWith("new ")) {
				inNew = true;
				expression = expression.Substring(4);
			} else {
				inNew = false;
			}
			if (expression.StartsWith(GetUsingString())) {
				return UsingResolve(expression);
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
			
			NRefactoryASTConvertVisitor cSharpVisitor = new NRefactoryASTConvertVisitor();
			cu = (ICompilationUnit)cSharpVisitor.Visit(fileCompilationUnit, null);
			if (cu != null) {
				callingClass = projectContent.GetInnermostClass(cu, caretLine, caretColumn);
			}
			
			TypeVisitor typeVisitor = new TypeVisitor(this);
			IReturnType type = expr.AcceptVisitor(typeVisitor, null) as IReturnType;
			
			if (type == null || type.FullyQualifiedName == "" || type.PointerNestingLevel != 0) {
				return null;
			}
			if (type.ArrayDimensions != null && type.ArrayDimensions.Length > 0) {
				type = new ReturnType("System.Array");
			}
			IClass returnClass = SearchType(type.FullyQualifiedName, callingClass, cu);
			if (returnClass == null) {
				return TryNamespace(type);
			}
			if (inNew) {
				return new ResolveResult(returnClass, projectContent.ListTypes(new ArrayList(), returnClass, callingClass));
			} else {
				ArrayList result = projectContent.ListMembers(new ArrayList(), returnClass, callingClass, showStatic);
				if (!showStatic && language == SupportedLanguages.VBNet) {
					result = projectContent.ListMembers(result, returnClass, callingClass, true);
				}
				return new ResolveResult(returnClass, result);
			}
		}
		
		bool IsNumber(string expression)
		{
			try {
				int.Parse(expression);
				return true;
			} catch (Exception) {
				return false;
			}
		}
		
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
		
		string GetUsingString()
		{
			switch (language) {
				case SupportedLanguages.CSharp:
					return "using ";
				case SupportedLanguages.VBNet:
					return "imports ";
				default:
					throw new NotSupportedException("The language " + language + " is not supported in the resolver");
			} 
		}
		
		ResolveResult ImportsResolve(string expression)
		{
			// expression[expression.Length - 1] != '.'
			// the period that causes this Resove() is not part of the expression
			if (expression[expression.Length - 1] == '.') {
				return null;
			}
			int i;
			for (i = expression.Length - 1; i >= 0; --i) {
				if (!(Char.IsLetterOrDigit(expression[i]) || expression[i] == '_' || expression[i] == '.')) {
					break;
				}
			}
			// no Identifier before the period
			if (i == expression.Length - 1) {
				return null;
			}
			string t = expression.Substring(i + 1);
			string[] namespaces = projectContent.GetNamespaceList(t);
			if (namespaces == null || namespaces.Length <= 0) {
				return null;
			}
			return new ResolveResult(namespaces);
		}
		
		ResolveResult UsingResolve(string expression)
		{
			if (language == SupportedLanguages.VBNet) {
				return ImportsResolve(expression);
			}
			// expression[expression.Length - 1] != '.'
			// the period that causes this Resove() is not part of the expression
			if (expression[expression.Length - 1] == '.') {
				return null;
			}
			int i;
			for (i = expression.Length - 1; i >= 0; --i) {
				if (!(Char.IsLetterOrDigit(expression[i]) || expression[i] == '_' || expression[i] == '.')) {
					break;
				}
			}
			// no Identifier before the period
			if (i == expression.Length - 1) {
				return null;
			}
			string t = expression.Substring(i + 1);
			string[] namespaces = projectContent.GetNamespaceList(t);
			if (namespaces == null || namespaces.Length <= 0) {
				return null;
			}
			return new ResolveResult(namespaces);
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
		
		ResolveResult TryNamespace(IReturnType type)
		{
			string n = SearchNamespace(type.FullyQualifiedName, cu);
			if (n == null) {
				return null;
			}
			ArrayList content = projectContent.GetNamespaceContents(n);
			ArrayList classes = new ArrayList();
			for (int i = 0; i < content.Count; ++i) {
				if (content[i] is IClass) {
					if (inNew) {
						IClass c = (IClass)content[i];
						if ((c.ClassType == ClassType.Class) || (c.ClassType == ClassType.Struct)) {
							classes.Add(c);
						}
					} else {
						classes.Add((IClass)content[i]);
					}
				}
			}
			string[] namespaces = projectContent.GetNamespaceList(n);
			return new ResolveResult(namespaces, classes);
		}
		
		bool InStatic()
		{
			IProperty property = Get();
			if (property != null) {
				return property.IsStatic;
			}
			IMethod method = GetMethod(caretLine, caretColumn);
			if (method != null) {
				return method.IsStatic;
			}
			return false;
		}
		
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
		
		bool IsSameName(string name1, string name2)
		{
			if (IsCaseSensitive(language)) {
				return name1 == name2;
			} else {
				return name1.ToLower() == name2.ToLower();
			}
		}
		
		ArrayList SearchMethod(ArrayList methods, IClass curType, string memberName)
		{
			bool isClassInInheritanceTree = projectContent.IsClassInInheritanceTree(curType, callingClass);
			
			foreach (IMethod m in curType.Methods) {
				if (IsSameName(m.Name, memberName) &&
				    projectContent.MustBeShown(curType, m, callingClass, showStatic, isClassInInheritanceTree) &&
				    !((m.Modifiers & ModifierEnum.Override) == ModifierEnum.Override)) {
					methods.Add(m);
				}
			}
			IClass baseClass = projectContent.BaseClass(curType);
			if (baseClass != null) {
				return SearchMethod(methods, baseClass, memberName);
			}
			showStatic = false;
			return methods;
		}
		
		public ArrayList SearchIndexer(IReturnType type)
		{
			IClass curType = SearchType(type.FullyQualifiedName, null, null);
			if (curType != null) {
				return SearchIndexer(new ArrayList(), curType);
			}
			return new ArrayList(1);
		}
		
		public ArrayList SearchIndexer(ArrayList indexer, IClass curType)
		{
			bool isClassInInheritanceTree = projectContent.IsClassInInheritanceTree(curType, callingClass);
			foreach (IIndexer i in curType.Indexer) {
				if (projectContent.MustBeShown(curType, i, callingClass, showStatic, isClassInInheritanceTree) && !((i.Modifiers & ModifierEnum.Override) == ModifierEnum.Override)) {
					indexer.Add(i);
				}
			}
			IClass baseClass = projectContent.BaseClass(curType);
			if (baseClass != null) {
				return SearchIndexer(indexer, baseClass);
			}
			showStatic = false;
			return indexer;
		}
		
		// no methods or indexer
		public IReturnType SearchMember(IReturnType type, string memberName)
		{
			// TODO: use SearchMember from ParserService
			if (type == null || memberName == null || memberName == "") {
				return null;
			}
			IClass curType = SearchType(type.FullyQualifiedName, callingClass, cu);
			bool isClassInInheritanceTree = projectContent.IsClassInInheritanceTree(curType, callingClass);
			
			if (curType == null) {
				return null;
			}
			if (type.PointerNestingLevel != 0) {
				return null;
			}
			if (type.ArrayDimensions != null && type.ArrayDimensions.Length > 0) {
				curType = SearchType("System.Array", null, null);
			}
			if (curType.ClassType == ClassType.Enum) {
				foreach (IField f in curType.Fields) {
					if (IsSameName(f.Name, memberName) && projectContent.MustBeShown(curType, f, callingClass, showStatic, isClassInInheritanceTree)) {
						showStatic = false;
						return type; // enum members have the type of the enum
					}
				}
			}
			if (showStatic) {
				foreach (IClass c in curType.InnerClasses) {
					if (IsSameName(c.Name, memberName) && projectContent.IsAccessible(curType, c, callingClass, isClassInInheritanceTree)) {
						return new ReturnType(c.FullyQualifiedName);
					}
				}
			}
			foreach (IProperty p in curType.Properties) {
				if (IsSameName(p.Name, memberName) && projectContent.MustBeShown(curType, p, callingClass, showStatic, isClassInInheritanceTree)) {
					showStatic = false;
					return p.ReturnType;
				}
			}
			foreach (IField f in curType.Fields) {
				if (IsSameName(f.Name, memberName) && projectContent.MustBeShown(curType, f, callingClass, showStatic, isClassInInheritanceTree)) {
					showStatic = false;
					return f.ReturnType;
				}
			}
			foreach (IEvent e in curType.Events) {
				if (IsSameName(e.Name, memberName) && projectContent.MustBeShown(curType, e, callingClass, showStatic, isClassInInheritanceTree)) {
					showStatic = false;
					return e.ReturnType;
				}
			}
			foreach (string baseType in curType.BaseTypes) {
				IClass c = SearchType(baseType, curType);
				if (c != null) {
					IReturnType erg = SearchMember(new ReturnType(c.FullyQualifiedName), memberName);
					if (erg != null) {
						return erg;
					}
				}
			}
			return null;
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
		
		IReturnType SearchVariable(string name)
		{
			ArrayList variables = (ArrayList)lookupTableVisitor.variables[IsCaseSensitive(language) ? name : name.ToLower()];
			if (variables == null || variables.Count <= 0) {
				return null;
			}
			
			IReturnType found = null;
			foreach (LocalLookupVariable v in variables) {
				if (IsInside(new Point(caretColumn, caretLine), v.StartPos, v.EndPos)) {
					IClass c = SearchType(v.TypeRef.SystemType, callingClass, cu);
					if (c != null) {
						found = new ReturnType(c.FullyQualifiedName, v.TypeRef.RankSpecifier, v.TypeRef.PointerNestingLevel);
					} else {
						found = new ReturnType(v.TypeRef);
					}
					break;
				}
			}
			if (found == null) {
				return null;
			}
			return found;
		}
		
		/// <remarks>
		/// does the dynamic lookup for the typeName
		/// </remarks>
		public IReturnType DynamicLookup(string typeName)
		{
			// try if it exists a variable named typeName
			IReturnType variable = SearchVariable(typeName);
			if (variable != null) {
				showStatic = false;
				return variable;
			}
			
			if (callingClass == null) {
				return null;
			}
		
			// try if typeName is a method parameter
			IReturnType p = SearchMethodParameter(typeName);
			if (p != null) {
				showStatic = false;
				return p;
			}
			
			// check if typeName == value in set method of a property
			if (typeName == "value") {
				p = SearchProperty();
				if (p != null) {
					showStatic = false;
					return p;
				}
			}
			
			// try if there exists a nonstatic member named typeName
			showStatic = false;
			IReturnType t = SearchMember(callingClass == null ? null : new ReturnType(callingClass.FullyQualifiedName), typeName);
			if (t != null) {
				return t;
			}
			
			// try if there exists a static member named typeName
			showStatic = true;
			t = SearchMember(callingClass == null ? null : new ReturnType(callingClass.FullyQualifiedName), typeName);
			if (t != null) {
				showStatic = false;
				return t;
			}
			
			// try if there exists a static member in outer classes named typeName
			List<IClass> classes = projectContent.GetOuterClasses(cu, caretLine, caretColumn);
			foreach (IClass c in classes) {
				t = SearchMember(callingClass == null ? null : new ReturnType(c.FullyQualifiedName), typeName);
				if (t != null) {
					showStatic = false;
					return t;
				}
			}
			
			// Alex: look in namespaces
			// Alex: look inside namespaces
			string[] innamespaces = projectContent.GetNamespaceList("");
			foreach (string ns in innamespaces) {
				ArrayList objs = projectContent.GetNamespaceContents(ns);
				if (objs == null) continue;
				foreach (object o in objs) {
					if (o is IClass) {
						IClass oc=(IClass)o;
						if (IsSameName(oc.Name, typeName) || IsSameName(oc.FullyQualifiedName, typeName)) {
							//Debug.WriteLine(((IClass)o).Name);
							/// now we can set completion data
							objs.Clear();
							objs = null;
							return new ReturnType(oc.FullyQualifiedName);
						}
					}
				}
				if (objs == null) {
					break;
				}
			}
			innamespaces = null;
			return null;
		}
		
		IProperty Get()
		{
			foreach (IProperty property in callingClass.Properties) {
				if (property.BodyRegion != null && property.BodyRegion.IsInside(caretLine, caretColumn)) {
					return property;
				}
			}
			return null;
		}
		
		IMethod GetMethod(int caretLine, int caretColumn)
		{
			foreach (IMethod method in callingClass.Methods) {
				if (method.BodyRegion != null && method.BodyRegion.IsInside(caretLine, caretColumn)) {
					return method;
				}
			}
			return null;
		}
		
		IReturnType SearchProperty()
		{
			IProperty property = Get();
			if (property == null) {
				return null;
			}
			if (property.SetterRegion != null && property.SetterRegion.IsInside(caretLine, caretColumn)) {
				return property.ReturnType;
			}
			return null;
		}
		
		IReturnType SearchMethodParameter(string parameter)
		{
			IMethod method = GetMethod(caretLine, caretColumn);
			if (method == null) {
				return null;
			}
			foreach (IParameter p in method.Parameters) {
				if (IsSameName(p.Name, parameter)) {
					return p.ReturnType;
				}
			}
			return null;
		}
		
		/// <remarks>
		/// use the usings to find the correct name of a namespace
		/// </remarks>
		public string SearchNamespace(string name, ICompilationUnit unit)
		{
			if (projectContent.NamespaceExists(name)) {
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
			NRefactoryASTConvertVisitor cSharpVisitor = new NRefactoryASTConvertVisitor();
			cu = (ICompilationUnit)cSharpVisitor.Visit(fileCompilationUnit, null);
			if (cu != null) {
				callingClass = projectContent.GetInnermostClass(cu, caretLine, caretColumn);
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
		}
		
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
			NRefactoryASTConvertVisitor cSharpVisitor = new NRefactoryASTConvertVisitor();
			cu = (ICompilationUnit)cSharpVisitor.Visit(fileCompilationUnit, null);
			if (cu != null) {
				callingClass = projectContent.GetInnermostClass(cu, caretLine, caretColumn);
				if (callingClass != null) {
					IMethod method = GetMethod(caretLine, caretColumn);
					if (method != null) {
						foreach (IParameter p in method.Parameters) {
							result.Add(new Field(new ReturnType(p.ReturnType.Name, p.ReturnType.ArrayDimensions, p.ReturnType.PointerNestingLevel), p.Name, Modifier.None, method.Region));
						}
					}
					result.AddRange(projectContent.GetNamespaceContents(callingClass.Namespace));
					bool inStatic = InStatic();
					result = projectContent.ListMembers(result, callingClass, callingClass, inStatic);
					if (inStatic == false) {
						result = projectContent.ListMembers(result, callingClass, callingClass, !inStatic);
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
							result.Add(new Field(new ReturnType(v.TypeRef), name, Modifier.None, new DefaultRegion(v.StartPos, v.EndPos)));
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
