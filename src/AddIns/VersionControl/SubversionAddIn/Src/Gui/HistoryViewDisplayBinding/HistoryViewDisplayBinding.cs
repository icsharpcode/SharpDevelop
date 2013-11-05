// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Workbench;

namespace ICSharpCode.Svn
{
	public class HistoryViewDisplayBinding : ISecondaryDisplayBinding
	{
		/// <summary>
		/// When you return true for this property, the CreateSecondaryViewContent method
		/// is called again after the LoadSolutionProjects thread has finished.
		/// </summary>
		public bool ReattachWhenParserServiceIsReady {
			get {
				return false;
			}
		}
		
		public IViewContent[] CreateSecondaryViewContent(IViewContent viewContent)
		{
			return new IViewContent[] { new HistoryView(viewContent) };
		}
		
		public bool CanAttachTo(IViewContent content)
		{
			if (!AddInOptions.UseHistoryDisplayBinding) {
				return false;
			}
			OpenedFile file = content.PrimaryFile;
			if (file == null || file.IsUntitled || !File.Exists(file.FileName)) {
				return false;
			}
			
			return SvnClientWrapper.IsInSourceControl(file.FileName);
		}
	}
}
