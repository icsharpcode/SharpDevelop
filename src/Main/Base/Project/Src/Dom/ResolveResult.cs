// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krueger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;

using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.Core;


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
//			if (callingMember != null && callingMember.DeclaringType != callingClass)
//				throw new ArgumentException("callingMember.DeclaringType must be equal to callingClass");
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
		}
		
		public virtual ArrayList GetCompletionData(IProjectContent projectContent)
		{
			if (resolvedType == null)
				return null;
			IClass c = projectContent.GetClass(resolvedType.FullyQualifiedName);
			if (c == null)
				return null;
			return c.GetAccessibleMembers(callingClass, false);
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
		
		public LocalResolveResult(IClass callingClass, IMember callingMember, IField field, bool isParameter)
			: base(callingClass, callingMember, field.ReturnType)
		{
			if (callingMember == null)
				throw new ArgumentNullException("callingMember");
			if (field == null)
				throw new ArgumentNullException("field");
			this.field = field;
			this.isParameter = isParameter;
		}
		
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
		
		public TypeResolveResult(IClass callingClass, IMember callingMember, IReturnType resolvedType, IClass resolvedClass)
			: base(callingClass, callingMember, resolvedType)
		{
			this.resolvedClass = resolvedClass;
		}
		
		/// <summary>
		/// Gets the class corresponding to the resolved type.
		/// </summary>
		public IClass ResolvedClass {
			get {
				return resolvedClass;
			}
		}
		
		public override ArrayList GetCompletionData(IProjectContent projectContent)
		{
			if (resolvedClass == null)
				return null;
			else
				return resolvedClass.GetAccessibleMembers(this.CallingClass, true);
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
	/// "SomeMethod" (when SomeMethod is a method in the current class)
	/// </example>
	public class MethodResolveResult : ResolveResult
	{
		string name;
		IClass containingClass;
		
		public MethodResolveResult(IClass callingClass, IMember callingMember, IClass containingClass, string name)
			: base(callingClass, callingMember, null)
		{
			if (containingClass == null)
				throw new ArgumentNullException("containingClass");
			if (name == null)
				throw new ArgumentNullException("name");
			this.containingClass = containingClass;
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
		/// Gets the class on that contains the method.
		/// </summary>
		public IClass ContainingClass {
			get {
				return containingClass;
			}
		}
	}
	#endregion
}
