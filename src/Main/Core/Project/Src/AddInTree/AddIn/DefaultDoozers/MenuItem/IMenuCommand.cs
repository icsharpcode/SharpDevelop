// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Reflection;
using System.Windows.Forms;

namespace ICSharpCode.Core
{
	public interface IMenuCommand : ICommand
	{
		bool IsEnabled {
			get;
			set;
		}
	}
}
