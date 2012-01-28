// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.CodeQuality.Engine.Dom
{
	/// <summary>
	/// Description of INode.
	/// </summary>
	public interface INode
	{
		string Name { get; }
		IList<INode> Children { get; }
		IEnumerable<INode> Uses { get; }
		IEnumerable<INode> UsedBy { get; }
		
		Relationship GetRelationship(INode value);
	}
}
