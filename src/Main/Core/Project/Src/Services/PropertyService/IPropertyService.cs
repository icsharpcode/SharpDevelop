// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace ICSharpCode.Core
{
	/// <summary>
	/// The property service.
	/// </summary>
	[SDService("SD.PropertyService")]
	public interface IPropertyService : INotifyPropertyChanged
	{
		/// <summary>
		/// Gets the configuration directory. (usually "%ApplicationData%\%ApplicationName%")
		/// </summary>
		/// <seealso cref="CoreStartup.ConfigDirectory"/>
		DirectoryName ConfigDirectory { get; }
		
		/// <summary>
		/// Gets the data directory (usually "ApplicationRootPath\data")
		/// </summary>
		/// <seealso cref="CoreStartup.DataDirectory"/>
		DirectoryName DataDirectory { get; }
		
		/// <summary>
		/// Gets the main properties container for this property service.
		/// </summary>
		Properties MainPropertiesContainer { get; }
		
		/// <inheritdoc cref="Properties.Get{T}(string, T)"/>
		T Get<T>(string key, T defaultValue);
		
		/// <inheritdoc cref="Properties.NestedProperties"/>
		Properties NestedProperties(string key);
		
		/// <inheritdoc cref="Properties.SetNestedProperties"/>
		void SetNestedProperties(string key, Properties nestedProperties);
		
		/// <inheritdoc cref="Properties.Contains"/>
		bool Contains(string key);
		
		/// <inheritdoc cref="Properties.Set{T}(string, T)"/>
		void Set<T>(string key, T value);
		
		/// <inheritdoc cref="Properties.GetList"/>
		IReadOnlyList<T> GetList<T>(string key);
		
		/// <inheritdoc cref="Properties.SetList"/>
		void SetList<T>(string key, IEnumerable<T> value);
		
		/// <inheritdoc cref="Properties.Remove"/>
		void Remove(string key);
		
		/// <summary>
		/// Saves the properties to disk.
		/// </summary>
		void Save();
	}
}
