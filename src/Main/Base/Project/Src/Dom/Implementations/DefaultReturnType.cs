// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
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
	[Serializable]
	public class DefaultReturnType : AbstractReturnType
	{
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
		
		public override int TypeParameterCount {
			get {
				return c.TypeParameters.Count;
			}
		}
		
		public override IClass GetUnderlyingClass()
		{
			return c;
		}
		
		public override List<IMethod> GetMethods()
		{
			List<IMethod> l = new List<IMethod>();
			foreach (IClass bc in c.ClassInheritanceTree) {
				if (bc.ClassType == ClassType.Interface && c.ClassType != ClassType.Interface)
					continue; // ignore explicit interface implementations
				
				foreach (IMethod m in bc.Methods) {
					// do not add base class constructors
					if (m.IsConstructor && c != bc)
						continue;
					
					// do not add methods that were overridden
					bool ok = true;
					foreach (IMethod oldMethod in l) {
						if (string.Equals(oldMethod.Name, m.Name, StringComparison.InvariantCultureIgnoreCase)) {
							if (m.IsStatic == oldMethod.IsStatic) {
								if (DiffUtility.Compare(oldMethod.Parameters, m.Parameters) == 0) {
									ok = false;
									break;
								}
							}
						}
					}
					if (ok)
						l.Add(m);
				}
			}
			return l;
		}
		
		public override List<IProperty> GetProperties()
		{
			List<IProperty> l = new List<IProperty>();
			foreach (IClass bc in c.ClassInheritanceTree) {
				if (bc.ClassType == ClassType.Interface && c.ClassType != ClassType.Interface)
					continue; // ignore explicit interface implementations
				
				foreach (IProperty p in bc.Properties) {
					// do not add methods that were overridden
					bool ok = true;
					foreach (IProperty oldProperty in l) {
						if (string.Equals(oldProperty.Name, p.Name, StringComparison.InvariantCultureIgnoreCase)) {
							if (p.IsStatic == oldProperty.IsStatic) {
								if (DiffUtility.Compare(oldProperty.Parameters, p.Parameters) == 0) {
									ok = false;
									break;
								}
							}
						}
					}
					if (ok)
						l.Add(p);
				}
			}
			return l;
		}
		
		public override List<IField> GetFields()
		{
			List<IField> l = new List<IField>();
			foreach (IClass bc in c.ClassInheritanceTree) {
				if (bc.ClassType == ClassType.Interface && c.ClassType != ClassType.Interface)
					continue; // ignore explicit interface implementations
				l.AddRange(bc.Fields);
			}
			return l;
		}
		
		public override List<IEvent> GetEvents()
		{
			List<IEvent> l = new List<IEvent>();
			foreach (IClass bc in c.ClassInheritanceTree) {
				if (bc.ClassType == ClassType.Interface && c.ClassType != ClassType.Interface)
					continue; // ignore explicit interface implementations
				l.AddRange(bc.Events);
			}
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
