// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.CodeQuality.Engine.Dom
{
	public class FieldNode : INode
	{
		public IField FieldDefinition { get; private set; }
		
		public FieldNode(IField fieldDefinition)
		{
			this.FieldDefinition = fieldDefinition;
		}
		
		public string Name {
			get { return FieldDefinition.PrintFullName(); }
		}
		
		public IList<INode> Children {
			get { return null; }
		}
		
		public IEnumerable<INode> Uses {
			get { return Enumerable.Empty<INode>(); }
		}
		
		public IEnumerable<INode> UsedBy {
			get { return Enumerable.Empty<INode>(); }
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
