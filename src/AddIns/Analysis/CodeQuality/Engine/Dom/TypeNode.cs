// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.CodeQuality.Engine.Dom
{
	public class TypeNode : INode
	{
		public ITypeDefinition TypeDefinition { get; private set; }
		
		public TypeNode(ITypeDefinition typeDefinition)
		{
			this.TypeDefinition = typeDefinition;
		}
		
		public string Name {
			get { return TypeDefinition.Name; }
		}
		
		public IList<INode> Children {
			get {
				return new List<INode>();
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
