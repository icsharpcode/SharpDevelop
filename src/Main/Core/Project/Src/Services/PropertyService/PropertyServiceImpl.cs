// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;
using ICSharpCode.Core;

namespace ICSharpCode.Core
{
	public class PropertyServiceImpl : IPropertyService
	{
		readonly Properties properties;
		
		/// <summary>
		/// Initializes the service for unit-testing (reset properties to an empty property container).
		/// Use <c>SD.InitializeForUnitTests()</c> instead, that initializes the property service and more.
		/// </summary>
		public PropertyServiceImpl()
		{
			properties = new Properties();
		}
		
		public PropertyServiceImpl(Properties properties)
		{
			if (properties == null)
				throw new ArgumentNullException("properties");
			this.properties = properties;
		}
		
		public virtual DirectoryName ConfigDirectory {
			get {
				throw new NotImplementedException();
			}
		}
		
		public virtual DirectoryName DataDirectory {
			get {
				throw new NotImplementedException();
			}
		}
		
		/// <inheritdoc cref="Properties.Get{T}(string, T)"/>
		public T Get<T>(string key, T defaultValue)
		{
			return properties.Get(key, defaultValue);
		}
		
		[Obsolete("Use the NestedProperties method instead", true)]
		public Properties Get(string key, Properties defaultValue)
		{
			return properties.Get(key, defaultValue);
		}
		
		/// <inheritdoc cref="Properties.NestedProperties"/>
		public Properties NestedProperties(string key)
		{
			return properties.NestedProperties(key);
		}
		
		/// <inheritdoc cref="Properties.SetNestedProperties"/>
		public void SetNestedProperties(string key, Properties nestedProperties)
		{
			properties.SetNestedProperties(key, nestedProperties);
		}
		
		/// <inheritdoc cref="Properties.Contains"/>
		public bool Contains(string key)
		{
			return properties.Contains(key);
		}
		
		/// <inheritdoc cref="Properties.Set{T}(string, T)"/>
		public void Set<T>(string key, T value)
		{
			properties.Set(key, value);
		}
		
		/// <inheritdoc cref="Properties.GetList"/>
		public IReadOnlyList<T> GetList<T>(string key)
		{
			return properties.GetList<T>(key);
		}
		
		/// <inheritdoc cref="Properties.SetList"/>
		public void SetList<T>(string key, IEnumerable<T> value)
		{
			properties.SetList(key, value);
		}
		
		/// <inheritdoc cref="Properties.Remove"/>
		public void Remove(string key)
		{
			properties.Remove(key);
		}
		
		public event PropertyChangedEventHandler PropertyChanged {
			add { properties.PropertyChanged += value; }
			remove { properties.PropertyChanged -= value; }
		}
		
		public Properties MainPropertiesContainer {
			get { return properties; }
		}
		
		public virtual void Save()
		{
		}

		public virtual Properties LoadExtraProperties(string key)
		{
			return new Properties();
		}

		public virtual void SaveExtraProperties(string key, Properties p)
		{
		}
	}
}
