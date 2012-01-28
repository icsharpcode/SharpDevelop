// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ICSharpCode.CodeQuality.Engine.Dom;

namespace ICSharpCode.CodeQuality.Gui
{
    public class DependencyMatrix : VisibleMatrix<INode, Relationship>
    {
		protected override Relationship GetCellValue(int rowIndex, int columnIndex)
		{
			var toRelationship = HeaderRows[rowIndex].Value.GetRelationship(HeaderColumns[columnIndex].Value);
			var fromRelationship = HeaderColumns[columnIndex].Value.GetRelationship(HeaderRows[rowIndex].Value);
			
			toRelationship.From = HeaderRows[rowIndex].Value;
			toRelationship.To = HeaderColumns[columnIndex].Value;
			
			// add other way
			foreach (var relationship in fromRelationship.Relationships) {
				if (relationship == RelationshipType.Uses)
					toRelationship.AddRelationship(RelationshipType.UsedBy);
			}
			
			return toRelationship;
		}
    }
}
