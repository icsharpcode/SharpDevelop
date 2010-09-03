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
	public class Relationship
	{
		public ISet<RelationshipType> Relationships { get; private set; }
		public int NumberOfOccurrences { get; set; }
		
		public Relationship()
		{
			Relationships = new HashSet<RelationshipType>();
			NumberOfOccurrences = 0;
		}
		
		public override string ToString()
		{
			var builder = new StringBuilder();
			
			foreach (var relationship in Relationships)
				builder.Append(relationship + " ");
			
			builder.Append(NumberOfOccurrences);
			return builder.ToString();
		}
	}
}
