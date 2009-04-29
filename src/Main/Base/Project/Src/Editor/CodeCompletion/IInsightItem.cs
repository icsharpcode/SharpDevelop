// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.SharpDevelop.Editor
{
	/// <summary>
	/// An item in the insight window.
	/// </summary>
	public interface IInsightItem
	{
		object Header { get; }
		object Content { get; }
	}
}
