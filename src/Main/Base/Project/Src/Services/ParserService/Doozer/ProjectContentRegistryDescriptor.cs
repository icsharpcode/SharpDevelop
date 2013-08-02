// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
			// codon.GetFailedAction is obsolete, it doesn't consider inherited
			// conditions. However, I don't care to fix this as inherited conditions
			// aren't used with project content registries, and this code
			// will be removed in SD5.
			#pragma warning disable 618
			try {
				return codon.GetFailedAction(project) == ConditionFailedAction.Nothing;
			} catch (ObjectDisposedException) {
				// This method may be used on a background thread, so there's a chance that the project got disposed.
				return false;
			}
		}
		
		public ProjectContentRegistryDescriptor(Codon codon)
		{
			if (codon == null)
				throw new ArgumentNullException("codon");
			this.codon = codon;
		}
	}
}
