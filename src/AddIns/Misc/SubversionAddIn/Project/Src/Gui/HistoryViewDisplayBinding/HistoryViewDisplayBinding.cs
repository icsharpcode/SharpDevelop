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
				}
			}
		}
		
		public bool CanAttachTo(ICSharpCode.SharpDevelop.Gui.IViewContent content)
		{
			if (content.IsUntitled || content.FileName == null || !File.Exists(content.FileName)) {
				return false;
			}
			string baseDir = Path.GetDirectoryName(content.FileName);
			string svnDir1 = Path.Combine(baseDir, ".svn");
			string svnDir2 = Path.Combine(baseDir, "_svn");
			if (!Directory.Exists(svnDir1) && !Directory.Exists(svnDir2))
				return false;
			return SvnHelper.IsVersionControlled(content.FileName);
		}
	}
}
