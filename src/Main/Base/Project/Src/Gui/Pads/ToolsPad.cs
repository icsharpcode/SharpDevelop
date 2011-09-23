// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using System.Windows.Controls;
using ICSharpCode.Core;
using System.Collections.Generic;
using Internal.Doozers;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// A pad that shows a single child control determined by the document that currently has the focus.
	/// </summary>
	public class ToolsPad : AbstractPadContent
	{
		private ContentPresenter contentControl = new ContentPresenter();
		private Dictionary<string, ToolsPadDescriptor> _descriptors;
		
		public override object Control
		{
			get { return contentControl; }
		}
		
		public ToolsPad()
		{
			_descriptors = new Dictionary<string, ToolsPadDescriptor>();
			List<ToolsPadDescriptor> descriptors = AddInTree.BuildItems<ToolsPadDescriptor>("/SharpDevelop/Workbench/ToolBoxContent", this);
			foreach (ToolsPadDescriptor descriptor in descriptors)
			{
				if (descriptor.Holder != null && !_descriptors.ContainsKey(descriptor.Holder))
				{
					_descriptors.Add(descriptor.Holder, descriptor);
				}
			}

			WorkbenchSingleton.Workbench.ActiveViewContentChanged += WorkbenchActiveContentChanged;
			WorkbenchActiveContentChanged(null, null);
		}
		
		private void WorkbenchActiveContentChanged(object sender, EventArgs e)
		{
			IViewContent viewContent = WorkbenchSingleton.Workbench.ActiveViewContent;
			contentControl.SetContent(GetToolBoxContent(viewContent), viewContent);
		}
		
		public object GetToolBoxContent(IViewContent viewContent)
		{
			if (viewContent != null)
			{
				Type holderType = viewContent.GetType();
				if (_descriptors.ContainsKey(holderType.FullName))
				{
					return _descriptors[holderType.FullName].GetToolBoxContent();
				}
			}
		
			return StringParser.Parse("${res:SharpDevelop.SideBar.NoToolsAvailableForCurrentDocument}");;
		}
	}
}
