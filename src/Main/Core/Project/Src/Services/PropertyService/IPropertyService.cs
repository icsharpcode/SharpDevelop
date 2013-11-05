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
		DirectoryName ConfigDirectory { get; }
		
		/// <summary>
		/// Gets the data directory (usually "ApplicationRootPath\data")
		/// </summary>
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
		/// Saves the main properties to disk.
		/// </summary>
		void Save();
		
		/// <summary>
		/// Loads extra properties that are not part of the main properties container.
		/// Unlike <see cref="NestedProperties"/>, multiple calls to <see cref="LoadExtraProperties"/>
		/// will return different instances, as the properties are re-loaded from disk every time.
		/// To save the properties, you need to call <see cref="SaveExtraProperties"/>.
		/// </summary>
		/// <returns>Properties container that was loaded; or an empty properties container
		/// if no container with the specified key exists.</returns>
		Properties LoadExtraProperties(string key);
		
		/// <summary>
		/// Saves extra properties that are not part of the main properties container.
		/// </summary>
		void SaveExtraProperties(string key, Properties p);
	}
}
