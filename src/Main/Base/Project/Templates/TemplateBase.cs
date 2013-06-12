// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.SharpDevelop.Templates
{
	/// <summary>
	/// Common base class for <see cref="FileTemplate"/> and <see cref="ProjectTemplate"/>.
	/// </summary>
	public abstract class TemplateBase
	{
		public abstract string Name { get; }
		public abstract string DisplayName { get; }
		public abstract string Description { get; }
		public abstract IImage Icon { get; }
	}
}
