/*
 * Created by SharpDevelop.
 * User: DG
 * Date: 02.09.2006
 * Time: 16:53
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// Describes a ProjectContentRegistry in the AddInTree.
	/// </summary>
	public sealed class ProjectContentRegistryDescriptor
	{
		Codon codon;
		
		ProjectContentRegistry registry;
		
		public ProjectContentRegistry Registry {
			get {
				return registry ?? (registry = (ProjectContentRegistry)codon.AddIn.CreateObject(codon.Properties["class"]));
			}
		}
		
		public bool UseRegistryForProject(IProject project)
		{
			return codon.GetFailedAction(project) == ConditionFailedAction.Nothing;
		}
		
		public ProjectContentRegistryDescriptor(Codon codon)
		{
			if (codon == null)
				throw new ArgumentNullException("codon");
			this.codon = codon;
		}
	}
}
