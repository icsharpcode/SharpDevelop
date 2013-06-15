// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// An NRefactory symbol as a model.
	/// </summary>
	public interface ISymbolModel : INotifyPropertyChanged
	{
		/// <summary>
		/// Gets the name of the entity.
		/// </summary>
		string Name { get; }
		
		/// <summary>
		/// Gets the symbol kind of the entity.
		/// </summary>
		SymbolKind SymbolKind { get; }
		
		/// <summary>
		/// Gets the parent project that contains this entity.
		/// May return null if the entity is not part of a project.
		/// </summary>
		IProject ParentProject { get; }
		
		/// <summary>
		/// Gets the region where this entity is defined.
		/// </summary>
		DomRegion Region { get; }
	}
}
