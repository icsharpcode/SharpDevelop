// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using ICSharpCode.SharpDevelop;

namespace Debugger.AddIn.TreeModel
{
	public class CopyCommand : SimpleCommand
	{
		public override void Execute(object parameter)
		{
			var node = parameter as ValueNode;
			if (node == null) return;
			SD.Clipboard.SetText(node.FullText);
		}
	}
	
	public class ShowFullErrorCommand : SimpleCommand
	{
		public override void Execute(object parameter)
		{
			var node = parameter as ValueNode;
			if (node == null) return;
			SD.MessageService.ShowException(node.error, null);
		}
	}
}
