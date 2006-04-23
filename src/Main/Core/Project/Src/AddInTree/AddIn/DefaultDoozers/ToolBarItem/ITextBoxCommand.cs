// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="John Simons" email="johnsimons007@yahoo.com.au"/>
//     <version>$Revision: 1 $</version>
// </file>

using System;
using System.Collections;
using System.Reflection;
using System.Windows.Forms;

namespace ICSharpCode.Core
{
	public interface ITextBoxCommand : ICommand
	{
		bool IsEnabled {
			get;
			set;
		}
	}
}
