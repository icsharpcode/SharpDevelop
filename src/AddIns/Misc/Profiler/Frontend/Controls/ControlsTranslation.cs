// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using System;

namespace ICSharpCode.Profiler.Controls
{
	/// <summary>
	/// Description of ControlsTranslation.
	/// </summary>
	public class ControlsTranslation
	{
		public virtual string WaitBarText {
			get {
				return "Refreshing view, please wait ...";
			}
		}
	}
}
