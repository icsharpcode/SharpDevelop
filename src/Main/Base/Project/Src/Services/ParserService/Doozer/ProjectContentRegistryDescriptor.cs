// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

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
		
		public bool IsRegistryLoaded {
			get { return registry != null; }
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
