// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Dom;
using System;
using System.Collections.Generic;

namespace UnitTesting.Tests.Utils
{
	public class MockClass : IClass
	{
		IProjectContent projectContent;
		DomRegion region = DomRegion.Empty;
		IList<IAttribute> attributes = new List<IAttribute>();
		IList<IMethod> methods = new List<IMethod>();
		IList<IClass> innerClasses = new List<IClass>();
		string fullyQualifiedName = String.Empty;
		string name = String.Empty;
		string ns = String.Empty;
		IClass compoundClass;
		IClass baseClass;
		string dotNetName = String.Empty;
		IClass declaringType;
		
		public MockClass()
		{
		}
		
		public MockClass(string fullyQualifiedName) : this(fullyQualifiedName, fullyQualifiedName)
		{
		}
		
		public MockClass(string fullyQualifiedName, string dotNetName)
		{
			FullyQualifiedName = fullyQualifiedName;
			this.dotNetName = dotNetName;
		}
		
		public override string ToString()
		{
			return dotNetName;
		}
		
		public string FullyQualifiedName {
			get { return fullyQualifiedName; }
			set {
				fullyQualifiedName = value;
				int index = fullyQualifiedName.LastIndexOf('.');
				if (index > 0) {
					name = fullyQualifiedName.Substring(index + 1);
					ns = fullyQualifiedName.Substring(0, index);
				} else {
					name = fullyQualifiedName;
				}
			}
		}
		
		public string Name {
			get { return name; }
			set { name = value; }
		}
		
		public string Namespace {
			get { return ns; }
			set { ns = value; }
		}
		
		public ClassType ClassType {
			get { return ClassType.Class; }
		}
		
		public IProjectContent ProjectContent {
			get { return projectContent; }
			set { projectContent = value; }
		}
		
		public DomRegion Region {
			get { return region; }
			set { region = value; }
		}
		
		public IList<IMethod> Methods {
			get { return methods; }
		}
		
		public IClass BaseClass {
			get { return baseClass; }
			set { baseClass = value; }
		}
		
		public ModifierEnum Modifiers {
			get { return ModifierEnum.None; }
			set { }
		}
		
		public IList<IAttribute> Attributes {
			get { return attributes; }
		}
		
		public IClass GetCompoundClass()
		{
			return compoundClass;
		}
		
		public void SetCompoundClass(IClass c)
		{
			compoundClass = c;
		}
		
		public IReturnType DefaultReturnType {
			get {
				throw new NotImplementedException();
			}
		}
		
		public string DotNetName {
			get { return dotNetName; }
		}
		
		public ICompilationUnit CompilationUnit {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IUsingScope UsingScope {
			get {
				throw new NotImplementedException();
			}
		}
		
		public DomRegion BodyRegion {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IList<IReturnType> BaseTypes {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IList<IClass> InnerClasses {
			get { return innerClasses; }
		}
		
		public IList<IField> Fields {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IList<IProperty> Properties {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IList<IEvent> Events {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IList<ITypeParameter> TypeParameters {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IEnumerable<IClass> ClassInheritanceTree {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IReturnType BaseType {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool HasPublicOrInternalStaticMembers {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool HasExtensionMethods {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool IsPartial {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public IClass DeclaringType {
			get { return declaringType; }
			set { declaringType = value; }
		}
		
		public string Documentation {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool IsAbstract {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool IsSealed {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool IsStatic {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool IsConst {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool IsVirtual {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool IsPublic {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool IsProtected {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool IsPrivate {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool IsInternal {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool IsReadonly {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool IsProtectedAndInternal {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool IsProtectedOrInternal {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool IsOverride {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool IsOverridable {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool IsNew {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool IsSynthetic {
			get {
				throw new NotImplementedException();
			}
		}
		
		public object UserData {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public bool IsFrozen {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IReturnType GetBaseType(int index)
		{
			throw new NotImplementedException();
		}
		
		public IClass GetInnermostClass(int caretLine, int caretColumn)
		{
			throw new NotImplementedException();
		}
		
		public List<IClass> GetAccessibleTypes(IClass callingClass)
		{
			throw new NotImplementedException();
		}
		
		public IMember SearchMember(string memberName, LanguageProperties language)
		{
			throw new NotImplementedException();
		}
		
		public bool IsTypeInInheritanceTree(IClass possibleBaseClass)
		{
			throw new NotImplementedException();
		}
		
		public bool IsAccessible(IClass callingClass, bool isClassInInheritanceTree)
		{
			throw new NotImplementedException();
		}
		
		public void Freeze()
		{
			throw new NotImplementedException();
		}
		
		public int CompareTo(object obj)
		{
			throw new NotImplementedException();
		}
		
		public bool HasCompoundClass {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
	}
}
