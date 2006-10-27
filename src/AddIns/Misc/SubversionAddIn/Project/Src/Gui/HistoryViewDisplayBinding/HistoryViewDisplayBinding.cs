// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using ICSharpCode.Core;
using NSvn.Core;
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
		
		public ICSharpCode.SharpDevelop.Gui.ISecondaryViewContent[] CreateSecondaryViewContent(ICSharpCode.SharpDevelop.Gui.IViewContent viewContent)
		{
			return new ICSharpCode.SharpDevelop.Gui.ISecondaryViewContent[] { new HistoryView(viewContent) };
		}
		
		private static class SvnHelper {
			// load NSvn.Core only when a directory actually contains the ".svn" folder
			static Client client;
			static bool firstSvnClientException = true;
			
			// prevent initialization of SvnHelper and therefore Client (and NSvn.Core.dll) unless it is
			// really required
			static SvnHelper() {}
			
			internal static bool IsVersionControlled(string fileName)
			{
				if (client == null) {
					LoggingService.Info("SVN: HistoryViewDisplayBinding initializes client");
					client = new Client();
				}
				try {
					Status status = client.SingleStatus(Path.GetFullPath(fileName));
					return status != null && status.Entry != null;
				} catch (SvnClientException ex) {
					if (firstSvnClientException) {
						firstSvnClientException = false;
						MessageService.ShowWarning("An error occurred while getting the Subversion status: " + ex.Message);
					} else {
						LoggingService.Warn("Svn: IsVersionControlled Exception", ex);
					}
					return false;
				}
			}
		}
		
		public bool CanAttachTo(ICSharpCode.SharpDevelop.Gui.IViewContent content)
		{
			if (content.IsUntitled || content.FileName == null || !File.Exists(content.FileName)) {
				return false;
			}
			if (Commands.RegisterEventsCommand.CanBeVersionControlledFile(content.FileName)) {
				return SvnHelper.IsVersionControlled(content.FileName);
			} else {
				return false;
			}
		}
	}
}
