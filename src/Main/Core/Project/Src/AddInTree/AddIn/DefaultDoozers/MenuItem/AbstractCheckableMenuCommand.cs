// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.Core
{
	public abstract class AbstractCheckableMenuCommand : AbstractMenuCommand, ICheckableMenuCommand
	{
		bool isChecked = false;
		
		public virtual bool IsChecked {
			get {
				return isChecked;
			}
			set {
				isChecked = value;
			}
		}
		public override void Run()
		{
			IsChecked = !IsChecked;
		}
	}
}
