// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.SharpDevelop.Tests.Utils
{
	public class MockProperty : IProperty
	{
		string name = String.Empty;

		public MockProperty(string name)
		{
			this.name = name;	
		}
		
		public string Name {
			get {
				return name;
			}
		}
		
		public IClass DeclaringType { get; set; }
		public bool IsConst { get; set; }
		public bool IsPrivate { get; set;}
		public bool IsOverridable { get; set;}
		
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
		
		public ModifierEnum GetterModifiers {
			get {
				throw new NotImplementedException();
			}
		}
		
		public ModifierEnum SetterModifiers {
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
				return false;
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
			return !IsPrivate;
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
		
		public ICompilationUnit CompilationUnit {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IProjectContent ProjectContent {
			get {
				throw new NotImplementedException();
			}
		}
		
		public EntityType EntityType {
			get { return EntityType.Property; }
		}
	}
}
