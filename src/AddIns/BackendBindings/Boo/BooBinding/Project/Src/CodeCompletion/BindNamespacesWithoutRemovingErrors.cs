// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using Boo.Lang.Compiler.Steps;

namespace Grunwald.BooBinding.CodeCompletion
{
	/// <summary>
	/// The Boo 'BindNamespaces' step will remove imports that cannot be resolved.
	/// However, we need to keep those imports available for use inside SharpDevelop.
	/// </summary>
	public class BindNamespacesWithoutRemovingErrors : BindNamespaces
	{
		public override void OnImport(Boo.Lang.Compiler.Ast.Import import)
		{
			base.OnImport(import);
			ReplaceCurrentNode(import); // prevent removal of import
		}
	}
}
