// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.SharpDevelop.Editor
{
	/// <summary>
	/// Specifies that the given interface is a service that can be retrieved using an <see cref="IDocument"/> or
	/// an <see cref="ITextEditor"/> as service provider.
	/// </summary>
	/// <remarks>
	/// This attribute is intended to be used as documentation, it does not have any effect at runtime.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class)]
	public class DocumentServiceAttribute : Attribute
	{
	}
	
	/// <summary>
	/// Specifies that the given interface is a service that can be retrieved using an <see cref="ITextEditor"/> as service provider.
	/// </summary>
	/// <remarks>
	/// This attribute is intended to be used as documentation, it does not have any effect at runtime.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class)]
	public class TextEditorServiceAttribute : Attribute
	{
	}
}
