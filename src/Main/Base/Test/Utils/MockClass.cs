// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Dom;
using System;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Tests.Utils
{
	/// <summary>
	/// Dummy class that implements the IClass interface. The
	/// only properties this mock class implements is DefaultReturnType and FullyQualifiedName.
	/// </summary>
	public class MockClass : IClass
	{
		public MockClass(string qualifiedName)
		{
			this.FullyQualifiedName = qualifiedName;
		}
		
		public string FullyQualifiedName { get; set; }
		public IReturnType DefaultReturnType { get; set;}
		
		public string DotNetName {
			get {
				throw new NotImplementedException();
			}
		}
		
		public string Name {
			get {
				throw new NotImplementedException();
			}
		}
		
		public string Namespace {
			get {
				throw new NotImplementedException();
			}
		}
		
		public ClassType ClassType {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IProjectContent ProjectContent {
			get {
				return DefaultProjectContent.DummyProjectContent;
			}
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
		
		public DomRegion Region {
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
			get {
				throw new NotImplementedException();
			}
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
		
		public IList<IMethod> Methods {
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
		
		public IEnumerable<IClass> ClassInheritanceTreeClassesOnly {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IClass BaseClass {
			get {
				return BaseType.GetUnderlyingClass();
			}
		}
		
		public IReturnType BaseType { get; set; }
		
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
			get {
				throw new NotImplementedException();
			}
		}
		
		public ModifierEnum Modifiers {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IList<IAttribute> Attributes {
			get {
				throw new NotImplementedException();
			}
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
		
		public IClass GetCompoundClass()
		{
			return this;
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
		
		public IEnumerable<IMember> AllMembers {
			get {
				throw new NotImplementedException();
			}
		}
		
		public EntityType EntityType {
			get { return EntityType.Class; }
		}
		
		public bool AddDefaultConstructorIfRequired {
			get {
				throw new NotImplementedException();
			}
		}
	}
}
