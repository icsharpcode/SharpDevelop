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
		/// Resolves the member in the current solution snapshot.
		/// Returns null if the member could not be resolved.
		/// </summary>
		new IMember Resolve();
		
		/// <summary>
		/// Resolves the member in the specified solution snapshot.
		/// Returns null if the member could not be resolved.
		/// </summary>
		new IMember Resolve(ISolutionSnapshotWithProjectMapping solutionSnapshot);
		
		/// <summary>
		/// Updates the member model with the specified new member.
		/// </summary>
		void Update(IUnresolvedMember newMember);
	}
}
