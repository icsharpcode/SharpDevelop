// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
