// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Text;

namespace ICSharpCode.CodeQualityAnalysis
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
			if (type == RelationshipType.UseThis || type == RelationshipType.UsedBy)
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
}
