// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.Specialized;

using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Represents a collection of configuration or platform names.
	/// </summary>
	public interface IConfigurationOrPlatformNameCollection : IModelCollection<string>
	{
		/// <summary>
		/// Validates the input name.
		/// 
		/// If the name is valid, this method returns the normalized form of the input name.
		/// If the name is invalid, this method returns null.
		/// </summary>
		/// <remarks>
		/// Normalization will trim spaces around the name; and it will normalize between "AnyCPU" and "Any CPU".
		/// </remarks>
		string ValidateName(string name);
		/*
		 * if (MSBuildInternals.Escape(newName) != newName
			    || !FileUtility.IsValidDirectoryEntryName(newName)
			    || newName.Contains("'"))
			{
				return false;
			}
		 */
		
		/// <summary>
		/// Creates a new configuration/platform.
		/// Settings will be copied from the existing configuration/platform <paramref name="copyFrom"/>.
		/// If <paramref name="copyFrom"/> is null, no settings will be copied.
		/// </summary>
		void Add(string newName, string copyFrom = null);
		
		/// <summary>
		/// Removes the configuration/platform with the specified name.
		/// </summary>
		void Remove(string name);
		
		/// <summary>
		/// Renames the configuration or platform from 'oldName' to 'newName'.
		/// If the configuration or platform is active, the <see cref="IConfigurable.ActiveConfiguration"/> property will be changed.
		/// </summary>
		void Rename(string oldName, string newName);
	}
}
