/*
 * Created by SharpDevelop.
 * User: Daniel Grunwald
 * Date: 19.08.2005
 * Time: 12:09
 */

using System;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Project
{
	public class SolutionPreferences : IMementoCapable
	{
		Solution solution;
		
		internal SolutionPreferences(Solution solution)
		{
			this.solution = solution;
		}
		
		string startupProject = "";
		
		public IProject StartupProject {
			get {
				if (startupProject.Length == 0)
					return null;
				foreach (IProject project in solution.Projects) {
					if (project.IdGuid.Equals(startupProject, StringComparison.OrdinalIgnoreCase))
						return project;
				}
				return null;
			}
			set {
				startupProject = (value != null) ? value.IdGuid : "";
			}
		}
		
		/// <summary>
		/// Creates a new memento from the state.
		/// </summary>
		public Properties CreateMemento()
		{
			Properties p = new Properties();
			p.Set("StartupProject", startupProject);
			return p;
		}
		
		/// <summary>
		/// Sets the state to the given memento.
		/// </summary>
		public void SetMemento(Properties memento)
		{
			startupProject = memento.Get("StartupProject", "");
		}
	}
}
