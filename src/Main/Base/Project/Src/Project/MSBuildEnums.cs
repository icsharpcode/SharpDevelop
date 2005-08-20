// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Project
{
	public enum RunPostBuildEvent {
		[Description("${res:Dialog.ProjectOptions.RunPostBuildEvent.Always}")]
		Always,
		[Description("${res:Dialog.ProjectOptions.RunPostBuildEvent.OnSuccessfulBuild}")]
		OnSuccessfulBuild,
		[Description("${res:Dialog.ProjectOptions.RunPostBuildEvent.OnOutputUpdated}")]
		OnOutputUpdated
	}
	
	public enum DebugSymbolType {
		None,
		Full,
		PdbOnly
	}
	
	public enum StartAction {
		Project,
		Program,
		StartURL
	}
	
	/// <summary>
	/// Specifies the possible locations where a property can be stored.
	/// </summary>
	public enum PropertyStorageLocation
	{
		Unchanged,
		/// <summary>
		/// Store the property globally for all configurations in the project file.
		/// </summary>
		BaseConfiguration,
		/// <summary>
		/// Store the property in the configuration-specific section(s) in the project file.
		/// </summary>
		SpecificConfiguration,
		///// <summary>
		///// Store the property globally for all configurations in the user file.
		///// </summary>
		//UserBaseConfiguration,
		/// <summary>
		/// Store the property in the configuration-specific section(s) in the user file.
		/// </summary>
		UserSpecificConfiguration,
	}
}
