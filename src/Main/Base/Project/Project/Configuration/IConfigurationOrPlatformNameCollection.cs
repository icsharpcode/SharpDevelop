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
