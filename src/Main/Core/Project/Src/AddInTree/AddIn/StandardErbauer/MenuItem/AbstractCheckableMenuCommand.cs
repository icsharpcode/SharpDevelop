// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.CodeDom.Compiler;
using System.Windows.Forms;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop
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
