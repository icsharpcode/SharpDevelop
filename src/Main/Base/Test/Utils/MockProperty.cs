// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.SharpDevelop.Tests.Utils
{
	public class MockProperty : IProperty
	{
		bool overridable;
		bool isConst;
		bool isPrivate;
		IClass declaringType;
		string name = String.Empty;

		public MockProperty(string name)
		{
			this.name = name;	
		}
		
		public DomRegion GetterRegion {
			get {
				throw new NotImplementedException();
			}
		}
		
		public DomRegion SetterRegion {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool CanGet {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool CanSet {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool IsIndexer {
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
		
		public DomRegion Region {
			get {
				throw new NotImplementedException();
			}
		}
		
		public string Name {
			get {
				return name;
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
		
		public DomRegion BodyRegion {
			get {
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
				return declaringType;
			}
			set {
				declaringType = value;
			}
		}
		
		public ModifierEnum Modifiers {
			get {
				throw new NotImplementedException();
			}
			set {
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
				return isConst;
			}
			set {
				isConst = value;
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
				return isPrivate;
			}
			set {
				isPrivate = value;
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
				return overridable;
			}
			set {
				overridable = value;
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
		
		public object Clone()
		{
			throw new NotImplementedException();
		}
		
	}
}
