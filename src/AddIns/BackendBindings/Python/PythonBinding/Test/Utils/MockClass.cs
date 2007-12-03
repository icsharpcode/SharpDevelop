// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Dom;

namespace PythonBinding.Tests.Utils
{
	/// <summary>
	/// Mock implementation of the IClass interface
	/// </summary>
	public class MockClass : IClass
	{
		string fullyQualifiedName = String.Empty;
		ICompilationUnit compilationUnit;
		
		public MockClass(string name)
		{
			fullyQualifiedName = name;
		}
		
		public string FullyQualifiedName {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public IReturnType DefaultReturnType {
			get { return new DefaultReturnType(this); }
		}
		
		public string DotNetName {
			get {
				throw new NotImplementedException();
			}
		}
		
		public string Name {
			get {
				int index = fullyQualifiedName.LastIndexOf('.');
				if (index > 0) {
					return fullyQualifiedName.Substring(index + 1);
				}
				return String.Empty;
			}
		}
		
		public string Namespace {
			get {
				int index = fullyQualifiedName.LastIndexOf('.');
				if (index > 0) {
					return fullyQualifiedName.Substring(0, index);
				}
				return String.Empty;
			}
		}
		
		public ClassType ClassType {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IProjectContent ProjectContent {
			get {
				throw new NotImplementedException();
			}
		}
		
		public ICompilationUnit CompilationUnit {
			get { return compilationUnit; }
			set { compilationUnit = value; }
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
		
		public System.Collections.Generic.List<IReturnType> BaseTypes {
			get {
				throw new NotImplementedException();
			}
		}
		
		public System.Collections.Generic.List<IClass> InnerClasses {
			get {
				throw new NotImplementedException();
			}
		}
		
		public System.Collections.Generic.List<IField> Fields {
			get {
				throw new NotImplementedException();
			}
		}
		
		public System.Collections.Generic.List<IProperty> Properties {
			get {
				throw new NotImplementedException();
			}
		}
		
		public System.Collections.Generic.List<IMethod> Methods {
			get {
				throw new NotImplementedException();
			}
		}
		
		public System.Collections.Generic.List<IEvent> Events {
			get {
				throw new NotImplementedException();
			}
		}
		
		public System.Collections.Generic.IList<ITypeParameter> TypeParameters {
			get {
				throw new NotImplementedException();
			}
		}
		
		public System.Collections.Generic.IEnumerable<IClass> ClassInheritanceTree {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IClass BaseClass {
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
			get { return null; }
		}
		
		public ModifierEnum Modifiers {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public System.Collections.Generic.IList<IAttribute> Attributes {
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
		
		public IReturnType GetBaseType(int index)
		{
			throw new NotImplementedException();
		}
		
		public IClass GetCompoundClass()
		{
			throw new NotImplementedException();
		}
		
		public IClass GetInnermostClass(int caretLine, int caretColumn)
		{
			throw new NotImplementedException();
		}
		
		public List<IClass> GetAccessibleTypes(IClass callingClass)
		{
			return new List<IClass>();
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
		
		public bool MustBeShown(IClass callingClass, bool showStatic, bool isClassInInheritanceTree)
		{
			throw new NotImplementedException();
		}
		
		public int CompareTo(object obj)
		{
			throw new NotImplementedException();
		}
		
	}
}
