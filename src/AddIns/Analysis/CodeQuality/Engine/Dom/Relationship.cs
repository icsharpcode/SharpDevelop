// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Text;

using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.CodeQuality.Engine.Dom
{
	/// <summary>
	/// Description of Relationship.
	/// </summary>
	public class Relationship : IValue
	{
		public ISet<RelationshipType> Relationships { get; private set; }
		
		public int OccurrenceCount { get; private set; }
		
		public string Text { get { return OccurrenceCount.ToString(); } }
		
		public INode To { get; set; }
		public INode From { get; set; }
		
		public Relationship()
		{
			Relationships = new HashSet<RelationshipType>();
		}
		
		public void AddRelationship(RelationshipType type)
		{
			if (type == RelationshipType.Uses || type == RelationshipType.UsedBy)
				OccurrenceCount++;
			
			Relationships.Add(type);
		}
		
		public override string ToString()
		{
			var builder = new StringBuilder();
			
			foreach (var relationship in Relationships)
				builder.Append(relationship + " ");
			
			builder.Append(OccurrenceCount);
			return builder.ToString();
		}
	}
	
	/// <summary>
	/// Type of relationship between two INodes.
	/// </summary>
	public enum RelationshipType
	{
		/// <summary>
		/// a and b are not related to each other.
		/// </summary>
		None,
		/// <summary>
		/// a contains b.
		/// </summary>
		Contains,
		/// <summary>
		/// a uses b.
		/// </summary>
		Uses,
		/// <summary>
		/// a is used by b.
		/// </summary>
		UsedBy, 
		/// <summary>
		/// a and b are the same INode
		/// </summary>
		Same
	}
}
