// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

namespace ICSharpCode.CodeQualityAnalysis
{
	/// <summary>
	/// Description of RelationshipType.
	/// </summary>
	public enum RelationshipType
	{
		OneWayTo,
		UseThis,
		UsedBy,
		Same,
		Contains,
		None
	}
}
