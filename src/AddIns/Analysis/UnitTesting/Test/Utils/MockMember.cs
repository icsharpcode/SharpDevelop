// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Dom;
using System;
using System.Collections.Generic;

namespace UnitTesting.Tests.Utils
{
	public class MockMember : IMember
	{
		public MockMember()
		{
		}
		
		public string FullyQualifiedName {
			get {
				return String.Empty;
			}
		}
		
		public DomRegion Region {
			get {
				return DomRegion.Empty;
			}
		}
		
		public string Name {
			get {
				return String.Empty;
			}
		}
		
		public string Namespace {
			get {
				return String.Empty;
			}
		}
		
		public string DotNetName {
			get {
				return String.Empty;
			}
		}
		
		public IReturnType ReturnType {
			get {
				return null;
			}
			set {
			}
		}
		
		public IClass DeclaringType {
			get {
				return null;
			}
		}
		
		public ModifierEnum Modifiers {
			get {
				return ModifierEnum.None;
			}
			set {
			}
		}
		
		public IList<IAttribute> Attributes {
			get {
				return null;
			}
		}
		
		public string Documentation {
			get {
				return String.Empty;
			}
		}
		
		public bool IsAbstract {
			get {
				return false;
			}
		}
		
		public bool IsSealed {
			get {
				return false;
			}
		}
		
		public bool IsStatic {
			get {
				return false;
			}
		}
		
		public bool IsConst {
			get {
				return false;
			}
		}
		
		public bool IsVirtual {
			get {
				return false;
			}
		}
		
		public bool IsPublic {
			get {
				return true;
			}
		}
		
		public bool IsProtected {
			get {
				return false;
			}
		}
		
		public bool IsPrivate {
			get {
				return false;
			}
		}
		
		public bool IsInternal {
			get {
				return false;
			}
		}
		
		public bool IsPartial {
			get {
				return false;
			}
		}
		
		public bool IsReadonly {
			get {
				return false;
			}
		}
		
		public bool IsProtectedAndInternal {
			get {
				return false;
			}
		}
		
		public bool IsProtectedOrInternal {
			get {
				return false;
			}
		}
		
		public bool IsOverride {
			get {
				return false;
			}
		}
		
		public bool IsOverridable {
			get {
				return false;
			}
		}
		
		public bool IsNew {
			get {
				return false;
			}
		}
		
		public bool IsSynthetic {
			get {
				return false;
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
		
		public IReturnType DeclaringTypeReference {
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
		
		public IMember GenericMember {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IMember CreateSpecializedMember()
		{
			throw new NotImplementedException();
		}
		
		public bool IsFrozen {
			get {
				return false;
			}
		}
		
		public void Freeze()
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
			get {
				throw new NotImplementedException();
			}
		}
	}
}
