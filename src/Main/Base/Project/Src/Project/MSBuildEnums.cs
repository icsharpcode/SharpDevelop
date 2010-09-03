// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;

namespace ICSharpCode.SharpDevelop.Project
{
	public enum RunPostBuildEvent
	{
		[Description("${res:Dialog.ProjectOptions.RunPostBuildEvent.OnSuccessfulBuild}")]
		OnBuildSuccess,
		[Description("${res:Dialog.ProjectOptions.RunPostBuildEvent.Always}")]
		Always,
		[Description("${res:Dialog.ProjectOptions.RunPostBuildEvent.OnOutputUpdated}")]
		OnOutputUpdated
	}
	
	public enum DebugSymbolType
	{
		[Description("${res:Dialog.ProjectOptions.DebugSymbolType.None}")]
		None,
		[Description("${res:Dialog.ProjectOptions.DebugSymbolType.Full}")]
		Full,
		[Description("${res:Dialog.ProjectOptions.DebugSymbolType.PdbOnly}")]
		PdbOnly
	}
	
	public enum StartAction
	{
		Project,
		Program,
		StartURL
	}
	
	/// <summary>
	/// Specifies the possible locations where a property can be stored.
	/// </summary>
	[Flags]
	public enum PropertyStorageLocations
	{
		/// <summary>
		/// Store the property where the property was previously stored.
		/// This is the same value as <see cref="Unknown"/>.
		/// </summary>
		Unchanged = 0,
		/// <summary>
		/// Returned from GetProperty/FindProperty when the property does not exist.
		/// This is the same value as <see cref="Unchanged"/>.
		/// </summary>
		Unknown = 0,
		/// <summary>
		/// Store the property globally for all configurations in the project file.
		/// </summary>
		Base = 1,
		/// <summary>
		/// Store the property in the configuration-specific section.
		/// </summary>
		ConfigurationSpecific = 2,
		/// <summary>
		/// Store the property in the platform-specific section.
		/// </summary>
		PlatformSpecific = 4,
		/// <summary>
		/// The combination of the ConfigurationSpecific and PlatformSpecific flags.
		/// </summary>
		ConfigurationAndPlatformSpecific = ConfigurationSpecific | PlatformSpecific,
		/// <summary>
		/// Store the property in the user file.
		/// </summary>
		UserFile = 8
	}
}
