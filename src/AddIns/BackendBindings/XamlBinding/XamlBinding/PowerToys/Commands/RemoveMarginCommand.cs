// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.XamlBinding;

namespace ICSharpCode.XamlBinding.PowerToys.Commands
{
	public class RemoveMarginCommand : XamlMenuCommand
	{
		protected override bool Refactor(ITextEditor editor, XDocument document)
		{
			RemoveRecursive(document.Root, "Margin");
			return true;
		}
		
		protected void RemoveRecursive(XElement element, string name)
		{
			foreach (XAttribute a in element.Attributes(name))
				a.Remove();
			
			foreach (XElement e in element.Elements())
				RemoveRecursive(e, name);
		}
	}
}
