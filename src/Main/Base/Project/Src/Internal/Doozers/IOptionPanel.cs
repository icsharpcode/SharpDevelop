// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Forms;

namespace ICSharpCode.SharpDevelop
{
	public interface IOptionPanel
	{
		/// <summary>
		/// Gets/sets the owner (the context object used when building the option panels
		/// from the addin-tree). This is null for IDE options or the IProject instance for project options.
		/// </summary>
		object Owner { get; set; }
		
		object Control {
			get;
		}
		
		void LoadOptions();
		bool SaveOptions();
	}
}
