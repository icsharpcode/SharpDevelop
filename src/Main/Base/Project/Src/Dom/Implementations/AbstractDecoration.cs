// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Dom
{
	[Serializable]
	public abstract class AbstractDecoration : MarshalByRefObject, IDecoration
	{
		ModifierEnum            modifiers  = ModifierEnum.None;
		List<IAttributeSection> attributes = null;
		
		IClass declaringType;
		object userData = null;
		
		public IClass DeclaringType {
			get {
				return declaringType;
			}
		}
		
		public object UserData {
			get {
				return userData;
			}
			set {
				userData = value;
			}
		}
		
		public ModifierEnum Modifiers {
			get {
				return modifiers;
			}
			set {
				modifiers = value;
			}
		}
		
		public List<IAttributeSection> Attributes {
			get {
				if (attributes == null) {
					attributes = new List<IAttributeSection>();
				}
				return attributes;
			}
		}
		
		public abstract string DocumentationTag {
			get;
		}
		
		public bool IsAbstract {
			get {
				return (modifiers & ModifierEnum.Abstract) == ModifierEnum.Abstract;
			}
		}
		
		public bool IsSealed {
			get {
				return (modifiers & ModifierEnum.Sealed) == ModifierEnum.Sealed;
			}
		}
		
		public bool IsStatic {
			get {
				return ((modifiers & ModifierEnum.Static) == ModifierEnum.Static) || IsConst;
			}
		}
		
		public bool IsConst {
			get {
				return (modifiers & ModifierEnum.Const) == ModifierEnum.Const;
			}
		}
		
		public bool IsVirtual {
			get {
				return (modifiers & ModifierEnum.Virtual) == ModifierEnum.Virtual;
			}
		}
		
		public bool IsPublic {
			get {
				return (modifiers & ModifierEnum.Public) == ModifierEnum.Public;
			}
		}
		
		public bool IsProtected {
			get {
				return (modifiers & ModifierEnum.Protected) == ModifierEnum.Protected;
			}
		}
		
		public bool IsPrivate {
			get {
				return (modifiers & ModifierEnum.Private) == ModifierEnum.Private;
			}
		}
		
		public bool IsInternal {
			get {
				return (modifiers & ModifierEnum.Internal) == ModifierEnum.Internal;
			}
		}
		
		public bool IsProtectedAndInternal {
			get {
				return (modifiers & (ModifierEnum.Internal | ModifierEnum.Protected)) == (ModifierEnum.Internal | ModifierEnum.Protected);
			}
		}
		
		public bool IsProtectedOrInternal {
			get {
				return (modifiers & ModifierEnum.ProtectedOrInternal) == ModifierEnum.ProtectedOrInternal;
			}
		}
		
		public bool IsLiteral {
			get {
				return (modifiers & ModifierEnum.Const) == ModifierEnum.Const;
			}
		}
		
		public bool IsReadonly {
			get {
				return (modifiers & ModifierEnum.Readonly) == ModifierEnum.Readonly;
			}
		}
		
		public bool IsOverride {
			get {
				return (modifiers & ModifierEnum.Override) == ModifierEnum.Override;
			}
		}
		
		public bool IsFinal {
			get {
				return (modifiers & ModifierEnum.Final) == ModifierEnum.Final;
			}
		}
		
		public bool IsSpecialName {
			get {
				return (modifiers & ModifierEnum.SpecialName) == ModifierEnum.SpecialName;
			}
		}
		
		public bool IsNew {
			get {
				return (modifiers & ModifierEnum.New) == ModifierEnum.New;
			}
		}
		
		public AbstractDecoration(IClass declaringType)
		{
			this.declaringType = declaringType;
		}
		
		bool IsInnerClass(IClass c, IClass possibleInnerClass)
		{
			foreach (IClass inner in c.InnerClasses) {
				if (inner.FullyQualifiedName == possibleInnerClass.FullyQualifiedName) {
					return true;
				}
				if (IsInnerClass(inner, possibleInnerClass)) {
					return true;
				}
			}
			return false;
		}
		
		// TODO: check inner classes for protected members too.
		// TODO: look for FullyQualifiedName == FullyQualifiedName. Must be replaced by a function wich pays attention to the case.
		//       Look at NRefactoryResolver.IsSameName. Also pay attention if you can put this Function in IClass, and if you have to
		//       compare the names instead of the FullyQualifiedNames
		public bool IsAccessible(IClass callingClass, bool isClassInInheritanceTree)
		{
			if (IsInternal) {
				return true;
			}
			if (IsPublic) {
				return true;
			}
			if (isClassInInheritanceTree && IsProtected) {
				return true;
			}
			
			return callingClass != null && (DeclaringType.FullyQualifiedName == callingClass.FullyQualifiedName || IsInnerClass(DeclaringType, callingClass));
		}
		
		public bool MustBeShown(IClass callingClass, bool showStatic, bool isClassInInheritanceTree)
		{
			if (DeclaringType.ClassType == ClassType.Enum) {
				return true;
			}
			if (!showStatic &&  IsStatic || (showStatic && !(IsStatic || IsConst))) { // const is automatically static
				return false;
			}
			return IsAccessible(callingClass, isClassInInheritanceTree);
		}
		
		
		public virtual int CompareTo(IDecoration value)
		{
			int cmp;
			
			if (0 != (cmp = (int)(Modifiers - value.Modifiers))) {
				return cmp;
			}
			
			return DiffUtility.Compare(Attributes, value.Attributes);
		}
		
		int IComparable.CompareTo(object value)
		{
			return CompareTo((IDecoration)value);
		}
	}
}
