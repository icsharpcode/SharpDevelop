// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Reflection;
using System.Windows.Forms;

namespace ICSharpCode.Core
{
	public interface ICheckableMenuCommand : IMenuCommand
	{
		bool IsChecked {
			get;
			set;
		}
	}
}
