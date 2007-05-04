// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Dom
{
	#region ResolveResult
	/// <summary>
	/// The base class of all resolve results.
	/// This class is used whenever an expression is not one of the special expressions
	/// (having their own ResolveResult class).
	/// The ResolveResult specified the location where Resolve was called (Class+Member)
	/// and the type of the resolved expression.
	/// </summary>
	public class ResolveResult
	{
		IClass callingClass;
		IMember callingMember;
		IReturnType resolvedType;
		
		public ResolveResult(IClass callingClass, IMember callingMember, IReturnType resolvedType)
		{
			this.callingClass = callingClass;
			this.callingMember = callingMember;
			this.resolvedType = resolvedType;
		}
		
		/// <summary>
		/// Gets the class that contained the expression used to get this ResolveResult.
		/// Can be null when the class is unknown.
		/// </summary>
		public IClass CallingClass {
			get {
				return callingClass;
			}
		}
		
		/// <summary>
		/// Gets the member (method or property in <see cref="CallingClass"/>) that contained the
		/// expression used to get this ResolveResult.
		/// Can be null when the expression was not inside a member or the member is unknown.
		/// </summary>
		public IMember CallingMember {
			get {
				return callingMember;
			}
		}
		
		/// <summary>
		/// Gets the type of the resolved expression.
		/// Can be null when the type cannot be represented by a IReturnType (e.g. when the
		/// expression was a namespace name).
		/// </summary>
		public IReturnType ResolvedType {
			get {
				return resolvedType;
			}
			set {
				resolvedType = value;
			}
		}
		
		public virtual ArrayList GetCompletionData(IProjectContent projectContent)
		{
			return GetCompletionData(projectContent.Language, false);
		}
		
		protected ArrayList GetCompletionData(LanguageProperties language, bool showStatic)
		{
			if (resolvedType == null) return null;
			ArrayList res = new ArrayList();
			bool isClassInInheritanceTree = false;
			if (callingClass != null)
				isClassInInheritanceTree = callingClass.IsTypeInInheritanceTree(resolvedType.GetUnderlyingClass());
			foreach (IMethod m in resolvedType.GetMethods()) {
				if (language.ShowMember(m, showStatic) && m.IsAccessible(callingClass, isClassInInheritanceTree))
					res.Add(m);
			}
			foreach (IEvent e in resolvedType.GetEvents()) {
				if (language.ShowMember(e, showStatic) && e.IsAccessible(callingClass, isClassInInheritanceTree))
					res.Add(e);
			}
			foreach (IField f in resolvedType.GetFields()) {
				if (language.ShowMember(f, showStatic) && f.IsAccessible(callingClass, isClassInInheritanceTree))
					res.Add(f);
			}
			foreach (IProperty p in resolvedType.GetProperties()) {
				if (language.ShowMember(p, showStatic) && p.IsAccessible(callingClass, isClassInInheritanceTree))
					res.Add(p);
			}
			
			if (!showStatic && callingClass != null) {
				AddExtensions(language, res, callingClass, resolvedType);
			}
			
			return res;
		}
		
		/// <summary>
		/// Adds extension methods to <paramref name="res"/>.
		/// </summary>
		public static void AddExtensions(LanguageProperties language, ArrayList res, IClass callingClass, IReturnType resolvedType)
		{
			if (language == null)
				throw new ArgumentNullException("language");
			if (res == null)
				throw new ArgumentNullException("res");
			if (resolvedType == null)
				throw new ArgumentNullException("resolvedType");
			if (callingClass == null)
				throw new ArgumentNullException("callingClass");
			
			foreach (IMethodOrProperty mp in CtrlSpaceResolveHelper.FindAllExtensions(language, callingClass)) {
				TryAddExtension(language, res, mp, resolvedType);
			}
		}
		
		static void TryAddExtension(LanguageProperties language, ArrayList res, IMethodOrProperty ext, IReturnType resolvedType)
		{
			// now add the extension method if it fits the type
			if (MemberLookupHelper.IsApplicable(resolvedType, ext.Parameters[0].ReturnType)) {
				IMethod method = ext as IMethod;
				if (method != null && method.TypeParameters.Count > 0) {
					IReturnType[] typeArguments = new IReturnType[method.TypeParameters.Count];
					MemberLookupHelper.InferTypeArgument(method.Parameters[0].ReturnType, resolvedType, typeArguments);
					for (int i = 0; i < typeArguments.Length; i++) {
						if (typeArguments[i] != null) {
							ext = (IMethod)ext.CreateSpecializedMember();
							ext.ReturnType = ConstructedReturnType.TranslateType(ext.ReturnType, typeArguments, true);
							for (int j = 0; j < ext.Parameters.Count; ++j) {
								ext.Parameters[j].ReturnType = ConstructedReturnType.TranslateType(ext.Parameters[j].ReturnType, typeArguments, true);
							}
							break;
						}
					}
				}
				res.Add(ext);
			}
		}
		
		public virtual FilePosition GetDefinitionPosition()
		{
			// this is only possible on some subclasses of ResolveResult
			return FilePosition.Empty;
		}
	}
	#endregion
	
	#region MixedResolveResult
	/// <summary>
	/// The MixedResolveResult is used when an expression can have multiple meanings, for example
	/// "Size" in a class deriving from "Control".
	/// </summary>
	public class MixedResolveResult : ResolveResult
	{
		ResolveResult primaryResult, secondaryResult;

		public ResolveResult PrimaryResult {
			get {
				return primaryResult;
			}
		}
		
		public IEnumerable<ResolveResult> Results {
			get {
				yield return primaryResult;
				yield return secondaryResult;
			}
		}
		
		public TypeResolveResult TypeResult {
			get {
				if (primaryResult is TypeResolveResult)
					return (TypeResolveResult)primaryResult;
				if (secondaryResult is TypeResolveResult)
					return (TypeResolveResult)secondaryResult;
				return null;
			}
		}
		
		public MixedResolveResult(ResolveResult primaryResult, ResolveResult secondaryResult)
			: base(primaryResult.CallingClass, primaryResult.CallingMember, primaryResult.ResolvedType)
		{
			this.primaryResult = primaryResult;
			this.secondaryResult = secondaryResult;
		}
		
		public override FilePosition GetDefinitionPosition()
		{
			return primaryResult.GetDefinitionPosition();
		}
		
		public override ArrayList GetCompletionData(IProjectContent projectContent)
		{
			ArrayList result = primaryResult.GetCompletionData(projectContent);
			ArrayList result2 = secondaryResult.GetCompletionData(projectContent);
			if (result == null)  return result2;
			if (result2 == null) return result;
			foreach (object o in result2) {
				if (!result.Contains(o))
					result.Add(o);
			}
			return result;
		}
	}
	#endregion
	
	#region LocalResolveResult
	/// <summary>
	/// The LocalResolveResult is used when an expression was a simple local variable
	/// or parameter.
	/// </summary>
	/// <remarks>
	/// For fields in the current class, a MemberResolveResult is used, so "e" is not always
	/// a LocalResolveResult.
	/// </remarks>
	public class LocalResolveResult : ResolveResult
	{
		IField field;
		bool isParameter;
		
		public LocalResolveResult(IMember callingMember, IField field)
			: base(callingMember.DeclaringType, callingMember, field.ReturnType)
		{
			if (callingMember == null)
				throw new ArgumentNullException("callingMember");
			if (field == null)
				throw new ArgumentNullException("field");
			this.field = field;
			this.isParameter = field.IsParameter;
			if (!isParameter && !field.IsLocalVariable) {
				throw new ArgumentException("the field must either be a local variable-field or a parameter-field");
			}
		}
		
		public LocalResolveResult(IMember callingMember, IParameter parameter)
			: this(callingMember, new DefaultField.ParameterField(parameter.ReturnType, parameter.Name, parameter.Region, callingMember.DeclaringType))
		{}
		
		/// <summary>
		/// Gets the field representing the local variable.
		/// </summary>
		public IField Field {
			get {
				return field;
			}
		}
		
		/// <summary>
		/// Gets if the variable is a parameter (true) or a local variable (false).
		/// </summary>
		public bool IsParameter {
			get {
				return isParameter;
			}
		}
		
		public override FilePosition GetDefinitionPosition()
		{
			ICompilationUnit cu = this.CallingClass.CompilationUnit;
			if (cu == null) {
				return FilePosition.Empty;
			}
			if (cu.FileName == null || cu.FileName.Length == 0) {
				return FilePosition.Empty;
			}
			DomRegion reg = field.Region;
			if (!reg.IsEmpty) {
				return new FilePosition(cu.FileName, reg.BeginLine, reg.BeginColumn);
			} else {
				LoggingService.Warn("GetDefinitionPosition: field.Region is empty");
				return new FilePosition(cu.FileName);
			}
		}
	}
	#endregion
	
	#region NamespaceResolveResult
	/// <summary>
	/// The NamespaceResolveResult is used when an expression was the name of a namespace.
	/// <see cref="ResolveResult.ResolvedType"/> is always null on a NamespaceReturnType.
	/// </summary>
	/// <example>
	/// Example expressions:
	/// "System"
	/// "System.Windows.Forms"
	/// "using Win = System.Windows;  Win.Forms"
	/// </example>
	public class NamespaceResolveResult : ResolveResult
	{
		string name;
		
		public NamespaceResolveResult(IClass callingClass, IMember callingMember, string name)
			: base(callingClass, callingMember, null)
		{
			if (name == null)
				throw new ArgumentNullException("name");
			this.name = name;
		}
		
		/// <summary>
		/// Gets the name of the namespace.
		/// </summary>
		public string Name {
			get {
				return name;
			}
		}
		
		public override ArrayList GetCompletionData(IProjectContent projectContent)
		{
			return projectContent.GetNamespaceContents(name);
		}
	}
	#endregion
	
	#region IntegerLiteralResolveResult
	/// <summary>
	/// The IntegerLiteralResolveResult is used when an expression was an integer literal.
	/// It is a normal ResolveResult with a type of "int", but does not provide
	/// any code completion data.
	/// </summary>
	public class IntegerLiteralResolveResult : ResolveResult
	{
		public IntegerLiteralResolveResult(IClass callingClass, IMember callingMember, IReturnType systemInt32)
			: base(callingClass, callingMember, systemInt32)
		{
		}
		
		public override ArrayList GetCompletionData(IProjectContent projectContent)
		{
			return null;
		}
	}
	#endregion
	
	#region TypeResolveResult
	/// <summary>
	/// The TypeResolveResult is used when an expression was the name of a type.
	/// This resolve result makes code completion show the static members instead
	/// of the instance members.
	/// </summary>
	/// <example>
	/// Example expressions:
	/// "System.EventArgs"
	/// "using System;  EventArgs"
	/// </example>
	public class TypeResolveResult : ResolveResult
	{
		IClass resolvedClass;
		
		public TypeResolveResult(IClass callingClass, IMember callingMember, IClass resolvedClass)
			: base(callingClass, callingMember, resolvedClass.DefaultReturnType)
		{
			this.resolvedClass = resolvedClass;
		}
		
		public TypeResolveResult(IClass callingClass, IMember callingMember, IReturnType resolvedType, IClass resolvedClass)
			: base(callingClass, callingMember, resolvedType)
		{
			this.resolvedClass = resolvedClass;
		}
		
		public TypeResolveResult(IClass callingClass, IMember callingMember, IReturnType resolvedType)
			: base(callingClass, callingMember, resolvedType)
		{
			this.resolvedClass = resolvedType.GetUnderlyingClass();
		}
		
		/// <summary>
		/// Gets the class corresponding to the resolved type.
		/// This property can be null when the type has no class (for example a type parameter).
		/// </summary>
		public IClass ResolvedClass {
			get {
				return resolvedClass;
			}
		}
		
		public override ArrayList GetCompletionData(IProjectContent projectContent)
		{
			ArrayList ar = GetCompletionData(projectContent.Language, true);
			if (resolvedClass != null) {
				foreach (IClass baseClass in resolvedClass.ClassInheritanceTree) {
					ar.AddRange(baseClass.InnerClasses);
				}
			}
			return ar;
		}
		
		public override FilePosition GetDefinitionPosition()
		{
			if (resolvedClass == null) {
				return FilePosition.Empty;
			}
			ICompilationUnit cu = resolvedClass.CompilationUnit;
			if (cu == null || cu.FileName == null || cu.FileName.Length == 0) {
				return FilePosition.Empty;
			}
			DomRegion reg = resolvedClass.Region;
			if (!reg.IsEmpty)
				return new FilePosition(cu.FileName, reg.BeginLine, reg.BeginColumn);
			else
				return new FilePosition(cu.FileName);
		}
	}
	#endregion
	
	#region MemberResolveResult
	/// <summary>
	/// The TypeResolveResult is used when an expression was a member
	/// (field, property, event or method call).
	/// </summary>
	/// <example>
	/// Example expressions:
	/// "(any expression).fieldName"
	/// "(any expression).eventName"
	/// "(any expression).propertyName"
	/// "(any expression).methodName(arguments)" (methods only when method parameters are part of expression)
	/// "using System;  EventArgs.Empty"
	/// "fieldName" (when fieldName is a field in the current class)
	/// "new System.Windows.Forms.Button()" (constructors are also methods)
	/// "SomeMethod()" (when SomeMethod is a method in the current class)
	/// "System.Console.WriteLine(text)"
	/// </example>
	public class MemberResolveResult : ResolveResult
	{
		IMember resolvedMember;
		
		public MemberResolveResult(IClass callingClass, IMember callingMember, IMember resolvedMember)
			: base(callingClass, callingMember, resolvedMember.ReturnType)
		{
			if (resolvedMember == null)
				throw new ArgumentNullException("resolvedMember");
			this.resolvedMember = resolvedMember;
		}
		
		/// <summary>
		/// Gets the member that was resolved.
		/// </summary>
		public IMember ResolvedMember {
			get {
				return resolvedMember;
			}
		}
		
		public override FilePosition GetDefinitionPosition()
		{
			return GetDefinitionPosition(resolvedMember);
		}
		
		internal static FilePosition GetDefinitionPosition(IMember resolvedMember)
		{
			IClass declaringType = resolvedMember.DeclaringType;
			if (declaringType == null) {
				return FilePosition.Empty;
			}
			ICompilationUnit cu = declaringType.CompilationUnit;
			if (cu == null) {
				return FilePosition.Empty;
			}
			if (cu.FileName == null || cu.FileName.Length == 0) {
				return FilePosition.Empty;
			}
			DomRegion reg = resolvedMember.Region;
			if (!reg.IsEmpty)
				return new FilePosition(cu.FileName, reg.BeginLine, reg.BeginColumn);
			else
				return new FilePosition(cu.FileName);
		}
	}
	#endregion
	
	#region MethodResolveResult
	/// <summary>
	/// The MethodResolveResult is used when an expression was the name of a method,
	/// but there were no parameters specified so the exact overload could not be found.
	/// <see cref="ResolveResult.ResolvedType"/> is always null on a MethodReturnType.
	/// </summary>
	/// <example>
	/// Example expressions:
	/// "System.Console.WriteLine"
	/// "a.Add"      (where a is List&lt;string&gt;)
	/// "SomeMethod" (when SomeMethod is a method in the current class)
	/// </example>
	public class MethodResolveResult : ResolveResult
	{
		string name;
		IReturnType containingType;
		
		public MethodResolveResult(IClass callingClass, IMember callingMember, IReturnType containingType, string name)
			: base(callingClass, callingMember, null)
		{
			if (containingType == null)
				throw new ArgumentNullException("containingType");
			if (name == null)
				throw new ArgumentNullException("name");
			this.containingType = containingType;
			this.name = name;
		}
		
		/// <summary>
		/// Gets the name of the method.
		/// </summary>
		public string Name {
			get {
				return name;
			}
		}
		
		/// <summary>
		/// Gets the class that contains the method.
		/// </summary>
		public IReturnType ContainingType {
			get {
				return containingType;
			}
		}
		
		public IMethod GetMethodIfSingleOverload()
		{
			List<IMethod> methods = containingType.GetMethods();
			methods = methods.FindAll(delegate(IMethod m) { return m.Name == this.Name; });
			if (methods.Count == 1)
				return methods[0];
			else
				return null;
		}
		
		public override FilePosition GetDefinitionPosition()
		{
			IMethod m = GetMethodIfSingleOverload();
			if (m != null)
				return MemberResolveResult.GetDefinitionPosition(m);
			else
				return base.GetDefinitionPosition();
		}
	}
	#endregion
}
