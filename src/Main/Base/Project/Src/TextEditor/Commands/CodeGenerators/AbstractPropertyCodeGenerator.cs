// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Commands
{
	public abstract class AbstractPropertyCodeGenerator : AbstractFieldCodeGenerator
	{
		public override int ImageIndex {
			get {
				return ClassBrowserIconService.PropertyIndex;
			}
		}
	}
}
