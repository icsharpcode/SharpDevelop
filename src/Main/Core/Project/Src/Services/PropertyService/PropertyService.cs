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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;

namespace ICSharpCode.Core
{
		/// <summary>
	/// Compatibility class; forwards calls to the IPropertyService.
	/// TODO: Remove
	/// </summary>
	public static class PropertyService
	{
		static IPropertyService Service {
			get { return ServiceSingleton.GetRequiredService<IPropertyService>(); }
		}
		
		static Properties properties {
			get { return Service.MainPropertiesContainer; }
		}
		
		public static DirectoryName ConfigDirectory {
			get { return Service.ConfigDirectory; }
		}
		
		public static DirectoryName DataDirectory {
			get { return Service.DataDirectory; }
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
		
		public static void Save()
		{
			Service.Save();
		}
		
		public static event PropertyChangedEventHandler PropertyChanged {
			add { properties.PropertyChanged += value; }
			remove { properties.PropertyChanged -= value; }
		}
	}
}
