// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

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
		
		object Content {
			get;
		}
		
		void LoadOptions();
		bool SaveOptions();
	}
}
