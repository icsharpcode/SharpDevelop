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
	/// <summary>
	/// DefaultReturnType is a reference to a normal class or a reference to a generic class where
	/// the type parameters are NOT specified.
	/// E.g. "System.Int32", "System.Void", "System.String", "System.Collections.Generic.List"
	/// </summary>
	public class DefaultReturnType : AbstractReturnType
	{
		public static bool Equals(IReturnType rt1, IReturnType rt2)
		{
			if (rt1 == rt2) return true;
			if (rt1 == null || rt2 == null) return false;
			IClass c1 = rt1.GetUnderlyingClass();
			IClass c2 = rt2.GetUnderlyingClass();
			if (c1 == null && c2 == null) {
				// guess if the classes are equal
				return rt1.FullyQualifiedName == rt2.FullyQualifiedName && rt1.TypeArgumentCount == rt2.TypeArgumentCount;
			} else {
				if (c1 == c2)
					return true;
				if (c1 == null || c2 == null)
					return false;
				return c1.FullyQualifiedName == c2.FullyQualifiedName && c1.TypeParameters.Count == c2.TypeParameters.Count;
			}
		}
		
		IClass c;
		
		public DefaultReturnType(IClass c)
		{
			if (c == null)
				throw new ArgumentNullException("c");
			this.c = c;
		}
		
		public override string ToString()
		{
			return c.FullyQualifiedName;
		}
		
		public override int TypeArgumentCount {
			get {
				return c.TypeParameters.Count;
			}
		}
		
		public override IClass GetUnderlyingClass()
		{
			return c;
		}
		
		bool getMembersBusy;
		
		public override List<IMethod> GetMethods()
		{
			if (getMembersBusy) return new List<IMethod>();
			getMembersBusy = true;
			List<IMethod> l = new List<IMethod>();
			l.AddRange(c.Methods);
			if (c.ClassType == ClassType.Interface) {
				if (c.BaseTypes.Count == 0) {
					AddMethodsFromBaseType(l, c.ProjectContent.SystemTypes.Object);
				} else {
					foreach (IReturnType baseType in c.BaseTypes) {
						AddMethodsFromBaseType(l, baseType);
					}
				}
			} else {
				AddMethodsFromBaseType(l, c.BaseType);
			}
			getMembersBusy = false;
			return l;
		}
		
		void AddMethodsFromBaseType(List<IMethod> l, IReturnType baseType)
		{
			if (baseType != null) {
				foreach (IMethod m in baseType.GetMethods()) {
					if (m.IsConstructor)
						continue;
					
					/*bool ok = true;
					if (m.IsOverridable) {
						StringComparer comparer = m.DeclaringType.ProjectContent.Language.NameComparer;
						foreach (IMethod oldMethod in c.Methods) {
							if (comparer.Equals(oldMethod.Name, m.Name)) {
								if (m.IsStatic == oldMethod.IsStatic && object.Equals(m.ReturnType, oldMethod.ReturnType)) {
									if (DiffUtility.Compare(oldMethod.Parameters, m.Parameters) == 0) {
										ok = false;
										break;
									}
								}
							}
						}
					}
					if (ok)
						l.Add(m);*/
					l.Add(m);
				}
			}
		}
		
		public override List<IProperty> GetProperties()
		{
			if (getMembersBusy) return new List<IProperty>();
			getMembersBusy = true;
			List<IProperty> l = new List<IProperty>();
			l.AddRange(c.Properties);
			if (c.ClassType == ClassType.Interface) {
				foreach (IReturnType baseType in c.BaseTypes) {
					AddPropertiesFromBaseType(l, baseType);
				}
			} else {
				AddPropertiesFromBaseType(l, c.BaseType);
			}
			getMembersBusy = false;
			return l;
		}
		
		void AddPropertiesFromBaseType(List<IProperty> l, IReturnType baseType)
		{
			if (baseType != null) {
				foreach (IProperty p in baseType.GetProperties()) {
					/*bool ok = true;
					if (p.IsOverridable) {
						StringComparer comparer = p.DeclaringType.ProjectContent.Language.NameComparer;
						foreach (IProperty oldProperty in c.Properties) {
							if (comparer.Equals(oldProperty.Name, p.Name)) {
								if (p.IsStatic == oldProperty.IsStatic && object.Equals(p.ReturnType, oldProperty.ReturnType)) {
									if (DiffUtility.Compare(oldProperty.Parameters, p.Parameters) == 0) {
										ok = false;
										break;
									}
								}
							}
						}
					}
					if (ok)
						l.Add(p);*/
					l.Add(p);
				}
			}
		}
		
		public override List<IField> GetFields()
		{
			if (getMembersBusy) return new List<IField>();
			getMembersBusy = true;
			List<IField> l = new List<IField>();
			l.AddRange(c.Fields);
			if (c.ClassType == ClassType.Interface) {
				foreach (IReturnType baseType in c.BaseTypes) {
					l.AddRange(baseType.GetFields());
				}
			} else {
				IReturnType baseType = c.BaseType;
				if (baseType != null) {
					l.AddRange(baseType.GetFields());
				}
			}
			getMembersBusy = false;
			return l;
		}
		
		public override List<IEvent> GetEvents()
		{
			if (getMembersBusy) return new List<IEvent>();
			getMembersBusy = true;
			List<IEvent> l = new List<IEvent>();
			l.AddRange(c.Events);
			if (c.ClassType == ClassType.Interface) {
				foreach (IReturnType baseType in c.BaseTypes) {
					l.AddRange(baseType.GetEvents());
				}
			} else {
				IReturnType baseType = c.BaseType;
				if (baseType != null) {
					l.AddRange(baseType.GetEvents());
				}
			}
			getMembersBusy = false;
			return l;
		}
		
		public override string FullyQualifiedName {
			get {
				return c.FullyQualifiedName;
			}
			set {
				throw new NotSupportedException();
			}
		}
		
		public override string Name {
			get {
				return c.Name;
			}
		}
		
		public override string Namespace {
			get {
				return c.Namespace;
			}
		}
		
		public override string DotNetName {
			get {
				return c.DotNetName;
			}
		}
	}
}
