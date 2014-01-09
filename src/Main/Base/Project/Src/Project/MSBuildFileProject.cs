// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// A project that is not a real project, but a MSBuild file included as project.
	/// </summary>
	public class MSBuildFileProject : AbstractProject
	{
		SolutionFormatVersion minimumSolutionVersion = SolutionFormatVersion.VS2005;
		
		public MSBuildFileProject(ProjectLoadInformation information) : base(information)
		{
			try {
				using (XmlReader r = XmlReader.Create(information.FileName, new XmlReaderSettings { IgnoreComments = true, XmlResolver = null })) {
					if (r.Read() && r.MoveToContent() == XmlNodeType.Element) {
						string toolsVersion = r.GetAttribute("ToolsVersion");
						Version v;
						if (Version.TryParse(toolsVersion, out v)) {
							if (v >= new Version(4, 0)) {
								minimumSolutionVersion = SolutionFormatVersion.VS2010; // use 4.0 Build Worker
							}
						}
					}
				}
			} catch (XmlException ex) {
				throw new ProjectLoadException(ex.Message, ex);
			} // IOException can also occur, but doesn't need to be converted to ProjectLoadException  
		}
		
		public override SolutionFormatVersion MinimumSolutionVersion {
			get { return minimumSolutionVersion; }
		}
		
		public override Task<bool> BuildAsync(ProjectBuildOptions options, IBuildFeedbackSink feedbackSink, IProgressMonitor progressMonitor)
		{
			return SD.MSBuildEngine.BuildAsync(this, options, feedbackSink, progressMonitor.CancellationToken);
		}
	}
}
