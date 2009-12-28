// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Dom;

namespace RubyBinding.Tests.Utils
{
	/// <summary>
	/// Mock IMethod implementation.
	/// </summary>
	public class MockMethod : IMethod
	{
		public MockMethod()
		{
		}
		
		public DomRegion BodyRegion { get; set; }
		
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
		
		public IList<IParameter> Parameters {
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
		
		public DomRegion Region {
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
		
		public string DotNetName {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IReturnType ReturnType {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public IList<ExplicitInterfaceImplementation> InterfaceImplementations {
			get {
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
