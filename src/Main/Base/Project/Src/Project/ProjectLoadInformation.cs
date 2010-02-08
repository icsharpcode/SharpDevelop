/*
 * Created by SharpDevelop.
 * User: Daniel
 * Date: 12.06.2009
 * Time: 20:48
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Parameter object for loading an existing project.
	/// </summary>
	public class ProjectLoadInformation
	{
		public Solution ParentSolution { get; private set; }
		public string FileName { get; private set; }
		public string Platform { get; internal set; }
		public string ProjectName { get; private set; }
		public string TypeGuid { get; set; }
		internal string Guid { get; set; }
		
		Gui.IProgressMonitor progressMonitor = new Gui.DummyProgressMonitor();
		
		/// <summary>
		/// Gets/Sets the progress monitor used during the load.
		/// This property never returns null.
		/// </summary>
		public Gui.IProgressMonitor ProgressMonitor {
			get { return progressMonitor; }
			set {
				if (value == null)
					throw new ArgumentNullException();
				progressMonitor = value;
			}
		}
		
		public ProjectLoadInformation(Solution parentSolution, string fileName, string projectName)
		{
			if (parentSolution == null)
				throw new ArgumentNullException("parentSolution");
			if (fileName == null)
				throw new ArgumentNullException("fileName");
			if (projectName == null)
				throw new ArgumentNullException("projectName");
			this.ParentSolution = parentSolution;
			this.FileName = fileName;
			this.ProjectName = projectName;
		}
	}
}
