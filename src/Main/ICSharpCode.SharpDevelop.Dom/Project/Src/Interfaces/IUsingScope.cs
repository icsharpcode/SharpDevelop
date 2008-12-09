// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// A scope that can contain using declarations.
	/// In C#, every file is a using scope, and every "namespace" declaration inside
	/// the file is a nested using scope.
	/// </summary>
	public interface IUsingScope : IFreezable
	{
		/// <summary>
		/// Gets the region of the using scope.
		/// </summary>
		DomRegion Region { get; }
		
		/// <summary>
		/// Gets the parent scope.
		/// Returns null if this is a root scope.
		/// </summary>
		IUsingScope Parent { get; }
		
		/// <summary>
		/// Gets the usings in this using scope.
		/// </summary>
		IList<IUsing> Usings { get; }
		
		/// <summary>
		/// Gets the list of child scopes. Child scopes usually represent "namespace" declarations.
		/// </summary>
		IList<IUsingScope> ChildScopes { get; }
		
		/// <summary>
		/// Gets the name of the namespace represented by the using scope.
		/// </summary>
		string NamespaceName { get; }
	}
}
