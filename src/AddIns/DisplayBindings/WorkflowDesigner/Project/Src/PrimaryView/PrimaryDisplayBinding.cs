// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Russell Wilkins" email=""/>
//     <version>$Revision$</version>
// </file>

#region Using
using System;
using System.IO;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
#endregion

namespace WorkflowDesigner
{
	/// <summary>
	/// Description of PrimaryDisplayBinding.
	/// </summary>
	public class WorkflowPrimaryDisplayBinding : IDisplayBinding
	{
		public WorkflowPrimaryDisplayBinding()
		{
		}
		
		public bool CanCreateContentForFile(string fileName)
		{
			return Path.GetExtension(fileName).Equals(".xoml", StringComparison.OrdinalIgnoreCase) ;
		}
		
		public IViewContent CreateContentForFile(OpenedFile file)
		{
			return new WorkflowPrimaryViewContent(file);
		}
	}
}
