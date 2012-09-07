// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.SharpDevelop.Bookmarks;
using ICSharpCode.SharpDevelop.Editor.Commands;

namespace ICSharpCode.ILSpyAddIn
{
	/// <summary>
	/// Implements a menu command to position .NET ILSpy on a class
	/// or class member.
	/// </summary>
	public sealed class OpenInILSpyCommand : ResolveResultMenuCommand
	{
		public override void Run(ResolveResult symbol)
		{
			var entity = GetEntity(symbol);
			if (entity != null) {
				ILSpyController.OpenInILSpy(entity);
			}
		}
	}
}
