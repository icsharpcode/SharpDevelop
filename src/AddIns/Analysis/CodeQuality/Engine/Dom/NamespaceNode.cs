// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.CodeQuality.Engine.Dom
{
	public class NamespaceNode : INode
	{
		public NamespaceNode(string name)
		{
			this.Name = name;
		}
		
		public string Name { get; private set; }
	
		public IList<INode> Children {
			get {
				return Enumerable.Empty<INode>().ToList();
			}
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
		
		public Relationship GetRelationship(INode value)
		{
			Relationship r = new Relationship();
			if (value == this)
				r.AddRelationship(RelationshipType.Same);
			return r;
		}
	}
}
