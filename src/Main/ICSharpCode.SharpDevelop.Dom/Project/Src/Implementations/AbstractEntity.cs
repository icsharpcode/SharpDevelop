// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Dom
{
	public abstract class AbstractEntity : AbstractFreezable, IEntity
	{
		ModifierEnum modifiers = ModifierEnum.None;
		IList<IAttribute> attributes;
		DomRegion bodyRegion;
		
		IClass declaringType;
		
		string fullyQualifiedName = null;
		string name               = null;
		string nspace             = null;
		
		public AbstractEntity(IClass declaringType)
		{
			this.declaringType = declaringType;
		}
		
		public AbstractEntity(IClass declaringType, string name)
		{
			if (declaringType == null)
				throw new ArgumentNullException("declaringType");
			this.declaringType = declaringType;
			this.name = name;
			nspace = declaringType.FullyQualifiedName;
			
			// lazy-computing the fully qualified name for class members saves ~7 MB RAM (when loading the SharpDevelop solution).
			//fullyQualifiedName = nspace + '.' + name;
		}
		
		public override string ToString()
		{
			return String.Format("[{0}: {1}]", GetType().Name, FullyQualifiedName);
		}
		
		#region Naming
		static readonly char[] nameDelimiters = { '.', '+' };
		
		
		public string FullyQualifiedName {
			get {
				if (fullyQualifiedName == null) {
					if (name != null && nspace != null) {
						fullyQualifiedName = nspace + '.' + name;
					} else {
						return String.Empty;
					}
				}
				return fullyQualifiedName;
			}
			set {
				CheckBeforeMutation();
				if (fullyQualifiedName == value)
					return;
				fullyQualifiedName = value;
				name   = null;
				nspace = null;
				OnFullyQualifiedNameChanged(EventArgs.Empty);
			}
		}
		
		protected virtual void OnFullyQualifiedNameChanged(EventArgs e)
		{
		}
		
		public virtual string DotNetName {
			get {
				if (this.DeclaringType != null) {
					return this.DeclaringType.DotNetName + "." + this.Name;
				} else {
					return FullyQualifiedName;
				}
			}
		}
		
		public string Name {
			get {
				if (name == null && FullyQualifiedName != null) {
					int lastIndex = FullyQualifiedName.LastIndexOfAny(nameDelimiters);
					
					if (lastIndex < 0) {
						name = FullyQualifiedName;
					} else {
						name = FullyQualifiedName.Substring(lastIndex + 1);
					}
				}
				return name;
			}
		}

		public string Namespace {
			get {
				if (nspace == null && FullyQualifiedName != null) {
					int lastIndex = FullyQualifiedName.LastIndexOf('.');
					
					if (lastIndex < 0) {
						nspace = String.Empty;
					} else {
						nspace = FullyQualifiedName.Substring(0, lastIndex);
					}
				}
				return nspace;
			}
		}
		
		
		#endregion
		
		protected override void FreezeInternal()
		{
			attributes = FreezeList(attributes);
			base.FreezeInternal();
		}
		
		public IClass DeclaringType {
			get {
				return declaringType;
			}
		}
		
		public virtual DomRegion BodyRegion {
			get {
				return bodyRegion;
			}
			set {
				CheckBeforeMutation();
				bodyRegion = value;
			}
		}
		
		public object UserData { get; set; }
		
		public IList<IAttribute> Attributes {
			get {
				if (attributes == null) {
					attributes = new List<IAttribute>();
				}
				return attributes;
			}
			set {
				CheckBeforeMutation();
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
				CheckBeforeMutation();
				documentation = value;
			}
		}
		
		public abstract string DocumentationTag {
			get;
		}
		
		#region Modifiers
		public ModifierEnum Modifiers {
			get {
				return modifiers;
			}
			set {
				CheckBeforeMutation();
				modifiers = value;
			}
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
		public bool IsOverridable {
			get {
				return (IsOverride || IsVirtual || IsAbstract) && !IsSealed;
			}
		}
		public bool IsNew {
			get {
				return (modifiers & ModifierEnum.New) == ModifierEnum.New;
			}
		}
		public bool IsSynthetic {
			get {
				return (modifiers & ModifierEnum.Synthetic) == ModifierEnum.Synthetic;
			}
		}
		#endregion
		
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
		
		public virtual int CompareTo(IEntity value)
		{
			return this.Modifiers - value.Modifiers;
		}
		
		int IComparable.CompareTo(object value)
		{
			return CompareTo((IEntity)value);
		}
	}
}
