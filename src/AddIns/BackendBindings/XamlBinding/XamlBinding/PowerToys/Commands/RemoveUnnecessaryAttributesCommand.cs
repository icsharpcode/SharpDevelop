// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.XamlBinding.PowerToys.Commands
{
	/// <summary>
	/// Description of RemoveUnneccessaryAttributesCommand
	/// </summary>
	public class RemoveUnnecessaryAttributesCommand : RemoveMarginCommand
	{
		protected override bool Refactor(ICSharpCode.SharpDevelop.Editor.ITextEditor editor, System.Xml.Linq.XDocument document)
		{
			RemoveRecursive(document.Root, "Margin");
			RemoveRecursive(document.Root, "MinWidth");
			RemoveRecursive(document.Root, "MinHeight");			
			return true;
		}
	}
}
