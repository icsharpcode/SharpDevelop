// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
