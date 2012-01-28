// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.CodeQuality.Engine.Dom
{
	public class MethodNode : INode
	{
		public IMethod MethodDefinition { get; private set; }
		
		public MethodNode(IMethod methodDefinition)
		{
			this.MethodDefinition = methodDefinition;
			uses = new List<INode>();
			usedBy = new List<INode>();
		}
		
		public string Name {
			get { return MethodDefinition.PrintFullName(); }
		}
		
		public IList<INode> Children {
			get { return null; }
		}
		
		internal IList<INode> uses, usedBy;
		
		public IEnumerable<INode> Uses {
			get { return uses; }
		}
		
		public IEnumerable<INode> UsedBy {
			get { return usedBy; }
		}
		
		public Relationship GetRelationship(INode value)
		{
			Relationship r = new Relationship();
			if (value == this)
				r.AddRelationship(RelationshipType.Same);
			if (Uses.Contains(value))
				r.AddRelationship(RelationshipType.Uses);
			if (UsedBy.Contains(value))
				r.AddRelationship(RelationshipType.UsedBy);
			return r;
		}
	}
}
