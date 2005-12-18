// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Dom
{
	[Serializable]
	public abstract class AbstractDecoration : IDecoration
	{
		ModifierEnum            modifiers  = ModifierEnum.None;
		IList<IAttribute> attributes = null;
		
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
		
		public IList<IAttribute> Attributes {
			get {
				if (attributes == null) {
					attributes = new List<IAttribute>();
				}
				return attributes;
			}
			set {
				attributes = value;
			}
		}
		
		string documentation;
		
		public string Documentation {
			get {
				if (documentation == null) {
					string documentationTag = this.DocumentationTag;
					if (documentationTag != null) {
						IProjectContent pc = null;
						if (this is IClass) {
							pc = ((IClass)this).ProjectContent;
						} else if (declaringType != null) {
							pc = declaringType.ProjectContent;
						}
						if (pc != null) {
							return pc.GetXmlDocumentation(documentationTag);
						}
					}
				}
				return documentation;
			}
			set {
				documentation = value;
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
				return IsProtected || IsInternal;
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
		public bool IsNew {
			get {
				return (modifiers & ModifierEnum.New) == ModifierEnum.New;
			}
		}
		public bool IsPartial {
			get {
				return (modifiers & ModifierEnum.Partial) == ModifierEnum.Partial;
			}
		}
		public bool IsSynthetic {
			get {
				return (modifiers & ModifierEnum.Synthetic) == ModifierEnum.Synthetic;
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
			return this.Modifiers - value.Modifiers;
		}
		
		int IComparable.CompareTo(object value)
		{
			return CompareTo((IDecoration)value);
		}
	}
}
