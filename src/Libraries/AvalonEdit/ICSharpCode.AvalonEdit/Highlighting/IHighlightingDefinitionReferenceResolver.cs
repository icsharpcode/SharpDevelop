// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.AvalonEdit.Highlighting
{
	/// <summary>
	/// Interface for resolvers that can solve cross-definition references.
	/// </summary>
	public interface IHighlightingDefinitionReferenceResolver
	{
		/// <summary>
		/// Gets the highlighting definition by name, or null if it is not found.
		/// </summary>
		IHighlightingDefinition GetDefinition(string name);
	}
}
