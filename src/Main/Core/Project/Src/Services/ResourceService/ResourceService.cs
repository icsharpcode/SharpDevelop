// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Threading;

namespace ICSharpCode.Core
{
	/// <summary>
	/// This Class contains two ResourceManagers, which handle string and image resources
	/// for the application. It do handle localization strings on this level.
	/// </summary>
	public static class ResourceService
	{
		const string uiLanguageProperty = "CoreProperties.UILanguage";
		
		const string stringResources = "StringResources";
		const string imageResources = "BitmapResources";
		
		static string resourceDirectory;
		
		public static void InitializeService(string resourceDirectory)
		{
			if (ResourceService.resourceDirectory != null)
				throw new InvalidOperationException("Service is already initialized.");
			if (resourceDirectory == null)
				throw new ArgumentNullException("resourceDirectory");
			
			ResourceService.resourceDirectory = resourceDirectory;
			
			PropertyService.PropertyChanged += new PropertyChangedEventHandler(OnPropertyChange);
			LoadLanguageResources(ResourceService.Language);
		}
		
		public static string Language {
			get {
				return PropertyService.Get(uiLanguageProperty, Thread.CurrentThread.CurrentUICulture.Name);
			}
			set {
				PropertyService.Set(uiLanguageProperty, value);
			}
		}
		
		/// <summary>English strings (list of resource managers)</summary>
		static List<ResourceManager> strings = new List<ResourceManager>();
		/// <summary>Neutral/English images (list of resource managers)</summary>
		static List<ResourceManager> icons   = new List<ResourceManager>();
		
		/// <summary>Hashtable containing the local strings from the main application.</summary>
		static Hashtable localStrings = null;
		static Hashtable localIcons   = null;
		
		/// <summary>Strings resource managers for the current language</summary>
		static List<ResourceManager> localStringsResMgrs = new List<ResourceManager>();
		/// <summary>Image resource managers for the current language</summary>
		static List<ResourceManager> localIconsResMgrs   = new List<ResourceManager>();
		
		/// <summary>List of ResourceAssembly</summary>
		static List<ResourceAssembly> resourceAssemblies = new List<ResourceAssembly>();
		
		class ResourceAssembly
		{
			Assembly assembly;
			string baseResourceName;
			bool isIcons;
			
			public ResourceAssembly(Assembly assembly, string baseResourceName, bool isIcons)
			{
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
						localIconsResMgrs.Add(manager);
					else
						localStringsResMgrs.Add(manager);
				}
			}
		}
		
		/// <summary>
		/// Registers string resources in the resource service.
		/// </summary>
		/// <param name="baseResourceName">The base name of the resource file embedded in the assembly.</param>
		/// <param name="assembly">The assembly which contains the resource file.</param>
		/// <example><c>ResourceService.RegisterStrings("TestAddin.Resources.StringResources", GetType().Assembly);</c></example>
		public static void RegisterStrings(string baseResourceName, Assembly assembly)
		{
			RegisterNeutralStrings(new ResourceManager(baseResourceName, assembly));
			ResourceAssembly ra = new ResourceAssembly(assembly, baseResourceName, false);
			resourceAssemblies.Add(ra);
			ra.Load();
		}
		
		public static void RegisterNeutralStrings(ResourceManager stringManager)
		{
			strings.Add(stringManager);
		}
		
		/// <summary>
		/// Registers image resources in the resource service.
		/// </summary>
		/// <param name="baseResourceName">The base name of the resource file embedded in the assembly.</param>
		/// <param name="assembly">The assembly which contains the resource file.</param>
		/// <example><c>ResourceService.RegisterImages("TestAddin.Resources.BitmapResources", GetType().Assembly);</c></example>
		public static void RegisterImages(string baseResourceName, Assembly assembly)
		{
			RegisterNeutralImages(new ResourceManager(baseResourceName, assembly));
			ResourceAssembly ra = new ResourceAssembly(assembly, baseResourceName, true);
			resourceAssemblies.Add(ra);
			ra.Load();
		}
		
		public static void RegisterNeutralImages(ResourceManager imageManager)
		{
			icons.Add(imageManager);
		}
		
		static void OnPropertyChange(object sender, PropertyChangedEventArgs e)
		{
			if (e.Key == uiLanguageProperty && e.NewValue != e.OldValue) {
				LoadLanguageResources((string)e.NewValue);
				if (LanguageChanged != null)
					LanguageChanged(null, e);
			}
		}
		
		public static event EventHandler ClearCaches;
		
		public static event EventHandler LanguageChanged;
		static string currentLanguage;
		
		static void LoadLanguageResources(string language)
		{
			if (ClearCaches != null)
				ClearCaches(null, EventArgs.Empty);
			
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
		
		static Hashtable Load(string fileName)
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
		
		static Hashtable Load(string name, string language)
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
		public static string GetString(string name)
		{
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
		
		public static object GetImageResource(string name)
		{
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
