// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.CodeQuality.Engine.Dom
{
	public class AssemblyNode : INode
	{
		public AssemblyNode(IAssembly assembly)
		{
			this.AssemblyInfo = assembly;
			namespaces = new List<INode>();
		}
		
		public IAssembly AssemblyInfo { get; private set; }
		
		public string Name {
			get { return AssemblyInfo.AssemblyName; }
		}
		
		internal List<INode> namespaces;
		
		public IList<INode> Children {
			get { return namespaces; }
		}
		
		public IEnumerable<INode> Uses {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IEnumerable<INode> UsedBy {
			get {
				throw new NotImplementedException();
			}
		}
		
		public void CalculateMetricsAndFreeze(IEnumerable<AssemblyNode> assemblies)
		{
			
		}
		
		public Relationship GetRelationship(INode value)
		{
			Relationship r = new Relationship();
			if (value == this)
				r.AddRelationship(RelationshipType.Same);
			return r;
		}
	}
}
