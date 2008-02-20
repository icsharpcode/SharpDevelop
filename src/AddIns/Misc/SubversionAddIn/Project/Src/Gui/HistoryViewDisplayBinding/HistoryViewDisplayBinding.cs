// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;

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
		
		public ICSharpCode.SharpDevelop.Gui.IViewContent[] CreateSecondaryViewContent(ICSharpCode.SharpDevelop.Gui.IViewContent viewContent)
		{
			return new ICSharpCode.SharpDevelop.Gui.IViewContent[] { new HistoryView(viewContent) };
		}
		
		public bool CanAttachTo(ICSharpCode.SharpDevelop.Gui.IViewContent content)
		{
			if (!AddInOptions.UseHistoryDisplayBinding) {
				return false;
			}
			OpenedFile file = content.PrimaryFile;
			if (file == null || file.IsUntitled || !File.Exists(file.FileName)) {
				return false;
			}
			if (Commands.RegisterEventsCommand.CanBeVersionControlledFile(file.FileName)) {
				StatusKind status = OverlayIconManager.GetStatus(file.FileName);
				return status != StatusKind.None && status != StatusKind.Unversioned && status != StatusKind.Ignored;
			} else {
				return false;
			}
		}
	}
}
