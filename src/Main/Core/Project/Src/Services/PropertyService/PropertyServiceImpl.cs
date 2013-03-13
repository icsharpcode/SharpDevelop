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

namespace ICSharpCode.Core
{
	public sealed class PropertyServiceImpl : IPropertyService
	{
		string propertyFileName;
		
		string configDirectory;
		string dataDirectory;
		
		Properties properties;
		
		/// <summary>
		/// Initializes the service for unit-testing (reset properties to an empty property container).
		/// Use <c>SD.InitializeForUnitTests()</c> instead, that initializes the property service and more.
		/// </summary>
		public PropertyServiceImpl()
		{
			properties = new Properties();
		}

		public PropertyServiceImpl(string configDirectory, string dataDirectory, string propertiesName)
		{
			this.configDirectory = configDirectory;
			this.dataDirectory = dataDirectory;
			this.propertyFileName = propertiesName + ".xml";
			Directory.CreateDirectory(configDirectory);
			LoadPropertiesFromStream(FileName.Create(Path.Combine(configDirectory, propertyFileName)));
		}
		
		public string ConfigDirectory {
			get {
				return configDirectory;
			}
		}
		
		public string DataDirectory {
			get {
				return dataDirectory;
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
		
		bool LoadPropertiesFromStream(FileName fileName)
		{
			if (!File.Exists(fileName)) {
				properties = new Properties();
				return false;
			}
			try {
				using (LockPropertyFile()) {
					properties = Properties.Load(fileName);
					return true;
				}
			} catch (XmlException ex) {
				var msgService = ServiceSingleton.GetRequiredService<IMessageService>();
				msgService.ShowError("Error loading properties: " + ex.Message + "\nSettings have been restored to default values.");
			}
			properties = new Properties();
			return false;
		}
		
		public void Save()
		{
			if (string.IsNullOrEmpty(configDirectory) || string.IsNullOrEmpty(propertyFileName))
				throw new InvalidOperationException("No file name was specified on service creation");
			
			var fileName = FileName.Create(Path.Combine(configDirectory, propertyFileName));
			using (LockPropertyFile()) {
				properties.Save(fileName);
			}
		}
		
		/// <summary>
		/// Acquires an exclusive lock on the properties file so that it can be opened safely.
		/// </summary>
		public IDisposable LockPropertyFile()
		{
			Mutex mutex = new Mutex(false, "PropertyServiceSave-30F32619-F92D-4BC0-BF49-AA18BF4AC313");
			mutex.WaitOne();
			return new CallbackOnDispose(
				delegate {
					mutex.ReleaseMutex();
					mutex.Close();
				});
		}
		
		public event PropertyChangedEventHandler PropertyChanged {
			add { properties.PropertyChanged += value; }
			remove { properties.PropertyChanged -= value; }
		}
		
		public Properties MainPropertiesContainer {
			get { return properties; }
		}
	}
}
