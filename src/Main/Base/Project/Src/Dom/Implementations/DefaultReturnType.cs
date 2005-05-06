// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Dom
{
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
		
		public override List<IMethod> GetMethods()
		{
			List<IMethod> l = new List<IMethod>();
			foreach (IClass bc in c.ClassInheritanceTree) {
				if (bc.ClassType != c.ClassType)
					continue; // ignore explicit interface implementations
				
				// do not add methods that were overridden
				foreach (IMethod m in bc.Methods) {
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
				if (bc.ClassType != c.ClassType)
					continue; // ignore explicit interface implementations
				l.AddRange(bc.Properties);
			}
			return l;
		}
		
		public override List<IField> GetFields()
		{
			List<IField> l = new List<IField>();
			foreach (IClass bc in c.ClassInheritanceTree) {
				if (bc.ClassType != c.ClassType)
					continue; // ignore explicit interface implementations
				l.AddRange(bc.Fields);
			}
			return l;
		}
		
		public override List<IEvent> GetEvents()
		{
			List<IEvent> l = new List<IEvent>();
			foreach (IClass bc in c.ClassInheritanceTree) {
				if (bc.ClassType != c.ClassType)
					continue; // ignore explicit interface implementations
				l.AddRange(bc.Events);
			}
			return l;
		}
		
		public override List<IIndexer> GetIndexers()
		{
			return c.Indexer;
		}
		
		public override string FullyQualifiedName {
			get {
				return c.FullyQualifiedName;
			}
			set {
				
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
