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
	public static class PropertyService
	{
		static string propertyFileName;
		
		static string configDirectory;
		static string dataDirectory;
		
		static Properties properties;
		
		public static bool Initialized {
			get { return properties != null; }
		}
		
		public static void InitializeServiceForUnitTests()
		{
			properties = null;
			configDirectory = null;
			dataDirectory = null;
			propertyFileName = null;
			properties = new Properties();
		}

		public static void InitializeService(string configDirectory, string dataDirectory, string propertiesName)
		{
			if (properties != null)
				throw new InvalidOperationException("Service is already initialized.");
			PropertyService.configDirectory = configDirectory;
			PropertyService.dataDirectory = dataDirectory;
			propertyFileName = propertiesName + ".xml";
			LoadPropertiesFromStream(Path.Combine(configDirectory, propertyFileName));
		}
		
		public static string ConfigDirectory {
			get {
				return configDirectory;
			}
		}
		
		public static string DataDirectory {
			get {
				return dataDirectory;
			}
		}
		
		/// <inheritdoc cref="Properties.Get{T}(string, T)"/>
		public static T Get<T>(string key, T defaultValue)
		{
			return properties.Get(key, defaultValue);
		}
		
		[Obsolete("Use the NestedProperties method instead", true)]
		public static Properties Get(string key, Properties defaultValue)
		{
			return properties.Get(key, defaultValue);
		}
		
		/// <inheritdoc cref="Properties.NestedProperties"/>
		public static Properties NestedProperties(string key)
		{
			return properties.NestedProperties(key);
		}
		
		/// <inheritdoc cref="Properties.SetNestedProperties"/>
		public static void SetNestedProperties(string key, Properties nestedProperties)
		{
			properties.SetNestedProperties(key, nestedProperties);
		}
		
		/// <inheritdoc cref="Properties.Contains"/>
		public static bool Contains(string key)
		{
			return properties.Contains(key);
		}
		
		/// <summary>
		/// Gets the main property container.
		/// </summary>
		internal static Properties PropertiesContainer {
			get { return properties; }
		}
		
		/// <inheritdoc cref="Properties.Set{T}(string, T)"/>
		public static void Set<T>(string key, T value)
		{
			properties.Set(key, value);
		}
		
		[Obsolete("Use the SetNestedProperties method instead", true)]
		public static void Set(string key, Properties value)
		{
			properties.Set(key, value);
		}
		
		/// <inheritdoc cref="Properties.GetList"/>
		public static IReadOnlyList<T> GetList<T>(string key)
		{
			return properties.GetList<T>(key);
		}
		
		/// <inheritdoc cref="Properties.SetList"/>
		public static void SetList<T>(string key, IEnumerable<T> value)
		{
			properties.SetList(key, value);
		}
		
		[Obsolete("Use the GetList method instead", true)]
		public static T[] Get<T>(string key, T[] defaultValue)
		{
			throw new InvalidOperationException();
		}
		
		[Obsolete("Use the SetList method instead", true)]
		public static void Set<T>(string key, T[] value)
		{
			throw new InvalidOperationException();
		}
		
		[Obsolete("Use the GetList method instead", true)]
		public static List<T> Get<T>(string key, List<T> defaultValue)
		{
			throw new InvalidOperationException();
		}
		
		[Obsolete("Use the SetList method instead", true)]
		public static void Set<T>(string key, List<T> value)
		{
			throw new InvalidOperationException();
		}
		
		[Obsolete("Use the GetList method instead", true)]
		public static ArrayList Get<T>(string key, ArrayList defaultValue)
		{
			throw new InvalidOperationException();
		}
		
		[Obsolete("Use the SetList method instead", true)]
		public static void Set<T>(string key, ArrayList value)
		{
			throw new InvalidOperationException();
		}
		
		/// <inheritdoc cref="Properties.Remove"/>
		public static void Remove(string key)
		{
			properties.Remove(key);
		}
		
		static bool LoadPropertiesFromStream(string fileName)
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
				MessageService.ShowError("Error loading properties: " + ex.Message + "\nSettings have been restored to default values.");
			}
			properties = new Properties();
			return false;
		}
		
		public static void Save()
		{
			if (string.IsNullOrEmpty(configDirectory) || string.IsNullOrEmpty(propertyFileName))
				throw new InvalidOperationException("No file name was specified on service creation");
			
			string fileName = Path.Combine(configDirectory, propertyFileName);
			using (LockPropertyFile()) {
				properties.Save(fileName);
			}
		}
		
		/// <summary>
		/// Acquires an exclusive lock on the properties file so that it can be opened safely.
		/// </summary>
		public static IDisposable LockPropertyFile()
		{
			Mutex mutex = new Mutex(false, "PropertyServiceSave-30F32619-F92D-4BC0-BF49-AA18BF4AC313");
			mutex.WaitOne();
			return new CallbackOnDispose(
				delegate {
					mutex.ReleaseMutex();
					mutex.Close();
				});
		}
		
		public static event PropertyChangedEventHandler PropertyChanged {
			add { properties.PropertyChanged += value; }
			remove { properties.PropertyChanged -= value; }
		}
	}
}
