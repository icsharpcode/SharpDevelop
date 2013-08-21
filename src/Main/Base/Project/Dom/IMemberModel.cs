// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Parser;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// Observable model for a member.
	/// </summary>
	public interface IMemberModel : IEntityModel
	{
		/// <summary>
		/// Resolves the member in the current compilation.
		/// Returns null if the member could not be resolved.
		/// </summary>
		new IMember Resolve();
		
		/// <summary>
		/// Resolves the member in the specified compilation.
		/// Returns null if the member could not be resolved.
		/// </summary>
		new IMember Resolve(ICompilation compilation);
		
		/// <summary>
		/// Gets the unresolved member.
		/// </summary>
		IUnresolvedMember UnresolvedMember { get; }
		
		/// <summary>
		/// Gets if the member is virtual. Is true only if the "virtual" modifier was used, but non-virtual
		/// members can be overridden, too; if they are abstract or overriding a method.
		/// </summary>
		bool IsVirtual { get; }
		
		/// <summary>
		/// Gets whether this member is overriding another member.
		/// </summary>
		bool IsOverride { get; }
		
		/// <summary>
		/// Gets if the member can be overridden. Returns true when the member is "abstract", "virtual" or "override" but not "sealed".
		/// </summary>
		bool IsOverridable { get; }
	}
}
