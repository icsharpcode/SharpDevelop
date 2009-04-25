// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.SharpDevelop.Gui
{
	public abstract class AbstractOptionPanel : IOptionPanel
	{
		public virtual object Owner { get; set; }
		
		public abstract object Control { get; }
		public abstract void LoadOptions();
		public abstract bool SaveOptions();
	}
}
