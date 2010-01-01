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
	public class MockMethod : IMethod
	{
		IClass declaringType;
		ModifierEnum modifiers;
		DomRegion region = DomRegion.Empty;
		DomRegion bodyRegion = DomRegion.Empty;
		IList<IAttribute> attributes = new List<IAttribute>();
		string name = String.Empty;
		IList<IParameter> parameters = new List<IParameter>();
		IReturnType returnType;
		bool isVirtual;
		bool isOverride;
		
		public MockMethod() : this(String.Empty)
		{
		}
		
		public MockMethod(string name)
		{
			this.name = name;
		}
		
		public DomRegion BodyRegion {
			get {
				return bodyRegion;
			}
			set {
				bodyRegion = value;
			}
		}
		
		public IList<IParameter> Parameters {
			get {
				return parameters;
			}
		}
		
		public DomRegion Region {
			get {
				return region;
			}
			set {
				region = value;
			}
		}
		
		public string Name {
			get {
				return name;
			}
			set {
				name = value;
			}
		}
		
		public IReturnType ReturnType {
			get {
				return returnType;
			}
			set {
				returnType = value;
			}
		}
		
		public IClass DeclaringType {
			get {
				return declaringType;
			}
			set {
				declaringType = value;
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
				return attributes;
			}
		}
		
		public IList<ITypeParameter> TypeParameters {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool IsConstructor {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IList<string> HandlesClauses {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool IsExtensionMethod {
			get {
				throw new NotImplementedException();
			}
		}
		
		public string FullyQualifiedName {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IReturnType DeclaringTypeReference {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public IMember GenericMember {
			get {
				throw new NotImplementedException();
			}
		}
		
		public string Namespace {
			get {
				throw new NotImplementedException();
			}
		}
		
		public string DotNetName {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IList<ExplicitInterfaceImplementation> InterfaceImplementations {
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
			get { return isVirtual; }
			set { isVirtual = value; }
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
			get { return isOverride; }
			set { isOverride = value; }
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
		
		public IMember CreateSpecializedMember()
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
		
		public object Clone()
		{
			throw new NotImplementedException();
		}
		
	}
}
