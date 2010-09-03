// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;

using ICSharpCode.NRefactory.Ast;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// Contains the changes that a WebReference has undergone after being
	/// refreshed.
	/// </summary>
	public class WebReferenceChanges
	{
		List<ProjectItem> newItems = new List<ProjectItem>();
		List<ProjectItem> itemsRemoved = new List<ProjectItem>();
		
		public WebReferenceChanges()
		{
		}
		
		/// <summary>
		/// Items that are new and need to be added to the project.
		/// </summary>
		public List<ProjectItem> NewItems {
			get {
				return newItems;
			}
		}
		
		/// <summary>
		/// Items that are missing and need to be removed from the project.
		/// </summary>
		public List<ProjectItem> ItemsRemoved {
			get {
				return itemsRemoved;
			}
		}
		
		/// <summary>
		/// Returns whether there are any changes.
		/// </summary>
		public bool Changed {
			get {
				return itemsRemoved.Count > 0 || newItems.Count > 0;
			}
		}
	}
}
