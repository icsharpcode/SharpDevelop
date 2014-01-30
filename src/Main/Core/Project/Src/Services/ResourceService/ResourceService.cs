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
using System.Reflection;
using System.Resources;
using System.Threading;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Compatibility class; forwards calls to the IResourceService.
	/// TODO: Remove
	/// </summary>
	public static class ResourceService
	{
		static IResourceService Service {
			get { return ServiceSingleton.GetRequiredService<IResourceService>(); }
		}
		
		public static string GetString(string resourceName)
		{
			return Service.GetString(resourceName);
		}
		
		public static string Language {
			get { return Service.Language; }
		}
	}
	
	/// <summary>
	/// This Class contains two ResourceManagers, which handle string and image resources
	/// for the application. It do handle localization strings on this level.
	/// </summary>
	public class ResourceServiceImpl : IResourceService
	{
		const string uiLanguageProperty = "CoreProperties.UILanguage";
		
		const string stringResources = "StringResources";
		const string imageResources = "BitmapResources";
		
		string resourceDirectory;
		IPropertyService propertyService;
		
		public ResourceServiceImpl(string resourceDirectory, IPropertyService propertyService)
		{
			if (resourceDirectory == null)
				throw new ArgumentNullException("resourceDirectory");
			if (propertyService == null)
				throw new ArgumentNullException("propertyService");
			
			this.resourceDirectory = resourceDirectory;
			this.propertyService = propertyService;
			propertyService.PropertyChanged += new PropertyChangedEventHandler(OnPropertyChange);
			LoadLanguageResources(this.Language);
		}
		
		public string Language {
			get {
				return propertyService.Get(uiLanguageProperty, Thread.CurrentThread.CurrentUICulture.Name);
			}
			set {
				if (Language != value) {
					propertyService.Set(uiLanguageProperty, value);
				}
			}
		}
		
		readonly object loadLock = new object();
		
		/// <summary>English strings (list of resource managers)</summary>
		List<ResourceManager> strings = new List<ResourceManager>();
		/// <summary>Neutral/English images (list of resource managers)</summary>
		List<ResourceManager> icons  = new List<ResourceManager>();
		
		/// <summary>Hashtable containing the local strings from the main application.</summary>
		Hashtable localStrings = null;
		Hashtable localIcons  = null;
		
		/// <summary>Strings resource managers for the current language</summary>
		List<ResourceManager> localStringsResMgrs = new List<ResourceManager>();
		/// <summary>Image resource managers for the current language</summary>
		List<ResourceManager> localIconsResMgrs  = new List<ResourceManager>();
		
		/// <summary>List of ResourceAssembly</summary>
		List<ResourceAssembly> resourceAssemblies = new List<ResourceAssembly>();
		
		class ResourceAssembly
		{
			ResourceServiceImpl service;
			Assembly assembly;
			string baseResourceName;
			bool isIcons;
			
			public ResourceAssembly(ResourceServiceImpl service, Assembly assembly, string baseResourceName, bool isIcons)
			{
				this.service = service;
				this.assembly = assembly;
				this.baseResourceName = baseResourceName;
				this.isIcons = isIcons;
			}
			
			ResourceManager TrySatellite(string language)
			{
				// ResourceManager should automatically use satellite assemblies, but it doesn't work
				// and we have to do it manually.
				string fileName = Path.GetFileNameWithoutExtension(assembly.Location) + ".resources.dll";
				fileName = Path.Combine(Path.Combine(Path.GetDirectoryName(assembly.Location), language), fileName);
				if (File.Exists(fileName)) {
					LoggingService.Info("Loging resources " + baseResourceName + " loading from satellite " + language);
					return new ResourceManager(baseResourceName, Assembly.LoadFrom(fileName));
				} else {
					return null;
				}
			}
			
			public void Load()
			{
				string currentLanguage = service.currentLanguage;
				string logMessage = "Loading resources " + baseResourceName + "." + currentLanguage + ": ";
				ResourceManager manager = null;
				if (assembly.GetManifestResourceInfo(baseResourceName + "." + currentLanguage + ".resources") != null) {
					LoggingService.Info(logMessage + " loading from main assembly");
					manager = new ResourceManager(baseResourceName + "." + currentLanguage, assembly);
				} else if (currentLanguage.IndexOf('-') > 0
				           && assembly.GetManifestResourceInfo(baseResourceName + "." + currentLanguage.Split('-')[0] + ".resources") != null)
				{
					LoggingService.Info(logMessage + " loading from main assembly (no country match)");
					manager = new ResourceManager(baseResourceName + "." + currentLanguage.Split('-')[0], assembly);
				} else {
					// try satellite assembly
					manager = TrySatellite(currentLanguage);
					if (manager == null && currentLanguage.IndexOf('-') > 0) {
						manager = TrySatellite(currentLanguage.Split('-')[0]);
					}
				}
				if (manager == null) {
					LoggingService.Warn(logMessage + "NOT FOUND");
				} else {
					if (isIcons)
						service.localIconsResMgrs.Add(manager);
					else
						service.localStringsResMgrs.Add(manager);
				}
			}
		}
		
		/// <summary>
		/// Registers string resources in the resource service.
		/// </summary>
		/// <param name="baseResourceName">The base name of the resource file embedded in the assembly.</param>
		/// <param name="assembly">The assembly which contains the resource file.</param>
		/// <example><c>ResourceService.RegisterStrings("TestAddin.Resources.StringResources", GetType().Assembly);</c></example>
		public void RegisterStrings(string baseResourceName, Assembly assembly)
		{
			RegisterNeutralStrings(new ResourceManager(baseResourceName, assembly));
			ResourceAssembly ra = new ResourceAssembly(this, assembly, baseResourceName, false);
			resourceAssemblies.Add(ra);
			ra.Load();
		}
		
		public void RegisterNeutralStrings(ResourceManager stringManager)
		{
			strings.Add(stringManager);
		}
		
		/// <summary>
		/// Registers image resources in the resource service.
		/// </summary>
		/// <param name="baseResourceName">The base name of the resource file embedded in the assembly.</param>
		/// <param name="assembly">The assembly which contains the resource file.</param>
		/// <example><c>ResourceService.RegisterImages("TestAddin.Resources.BitmapResources", GetType().Assembly);</c></example>
		public void RegisterImages(string baseResourceName, Assembly assembly)
		{
			RegisterNeutralImages(new ResourceManager(baseResourceName, assembly));
			ResourceAssembly ra = new ResourceAssembly(this, assembly, baseResourceName, true);
			resourceAssemblies.Add(ra);
			ra.Load();
		}
		
		public void RegisterNeutralImages(ResourceManager imageManager)
		{
			icons.Add(imageManager);
		}
		
		void OnPropertyChange(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == uiLanguageProperty) {
				LoadLanguageResources(Language);
				EventHandler handler = LanguageChanged;
				if (handler != null)
					handler(this, e);
			}
		}
		
		public event EventHandler LanguageChanged;
		string currentLanguage;
		
		void LoadLanguageResources(string language)
		{
			lock (loadLock) {
				try {
					Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(language);
				} catch (Exception) {
					try {
						Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(language.Split('-')[0]);
					} catch (Exception) {}
				}
				
				localStrings = Load(stringResources, language);
				if (localStrings == null && language.IndexOf('-') > 0) {
					localStrings = Load(stringResources, language.Split('-')[0]);
				}
				
				localIcons = Load(imageResources, language);
				if (localIcons == null && language.IndexOf('-') > 0) {
					localIcons = Load(imageResources, language.Split('-')[0]);
				}
				
				localStringsResMgrs.Clear();
				localIconsResMgrs.Clear();
				currentLanguage = language;
				foreach (ResourceAssembly ra in resourceAssemblies) {
					ra.Load();
				}
			}
		}
		
		Hashtable Load(string fileName)
		{
			if (File.Exists(fileName)) {
				Hashtable resources = new Hashtable();
				ResourceReader rr = new ResourceReader(fileName);
				foreach (DictionaryEntry entry in rr) {
					resources.Add(entry.Key, entry.Value);
				}
				rr.Close();
				return resources;
			}
			return null;
		}
		
		Hashtable Load(string name, string language)
		{
			return Load(resourceDirectory + Path.DirectorySeparatorChar + name + "." + language + ".resources");
		}
		
		/// <summary>
		/// Returns a string from the resource database, it handles localization
		/// transparent for the user.
		/// </summary>
		/// <returns>
		/// The string in the (localized) resource database.
		/// </returns>
		/// <param name="name">
		/// The name of the requested resource.
		/// </param>
		/// <exception cref="ResourceNotFoundException">
		/// Is thrown when the GlobalResource manager can't find a requested resource.
		/// </exception>
		public string GetString(string name)
		{
			lock (loadLock) {
				if (localStrings != null && localStrings[name] != null) {
					return localStrings[name].ToString();
				}
				
				string s = null;
				foreach (ResourceManager resourceManger in localStringsResMgrs) {
					try {
						s = resourceManger.GetString(name);
					}
					catch (Exception) { }

					if (s != null) {
						break;
					}
				}
				
				if (s == null) {
					foreach (ResourceManager resourceManger in strings) {
						try {
							s = resourceManger.GetString(name);
						}
						catch (Exception) { }
						
						if (s != null) {
							break;
						}
					}
				}
				if (s == null) {
					throw new ResourceNotFoundException("string >" + name + "<");
				}
				
				return s;
			}
		}
		
		public object GetImageResource(string name)
		{
			lock (loadLock) {
				object iconobj = null;
				if (localIcons != null && localIcons[name] != null) {
					iconobj = localIcons[name];
				} else {
					foreach (ResourceManager resourceManger in localIconsResMgrs) {
						iconobj = resourceManger.GetObject(name);
						if (iconobj != null) {
							break;
						}
					}
					
					if (iconobj == null) {
						foreach (ResourceManager resourceManger in icons) {
							try {
								iconobj = resourceManger.GetObject(name);
							}
							catch (Exception) { }

							if (iconobj != null) {
								break;
							}
						}
					}
				}
				return iconobj;
			}
		}
	}
}
