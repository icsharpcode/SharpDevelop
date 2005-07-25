// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
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
