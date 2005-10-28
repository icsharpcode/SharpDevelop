// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Windows.Forms;
using System.Collections;
using System.Threading;
using System.Resources;
using System.Drawing;
using System.Diagnostics;
using System.Reflection;
using System.Xml;

using ICSharpCode.Core;

namespace ICSharpCode.Core
{
	/// <summary>
	/// This Class contains two ResourceManagers, which handle string and image resources
	/// for the application. It do handle localization strings on this level.
	/// </summary>
	public static class ResourceService
	{
		readonly static string uiLanguageProperty = "CoreProperties.UILanguage";
		
		readonly static string stringResources = "StringResources";
		readonly static string imageResources = "BitmapResources";
		
		static string resourceDirectory;
		
		public static void InitializeService()
		{
			if (resourceDirectory != null)
				throw new InvalidOperationException("Service is already initialized.");
			resourceDirectory = FileUtility.Combine(PropertyService.DataDirectory, "resources");
			
			PropertyService.PropertyChanged += new PropertyChangedEventHandler(OnPropertyChange);
			LoadLanguageResources(PropertyService.Get(uiLanguageProperty, Thread.CurrentThread.CurrentUICulture.Name));
		}
		
		/// <summary>English strings (list of resource managers)</summary>
		static ArrayList strings = new ArrayList();
		/// <summary>Neutral/English images (list of resource managers)</summary>
		static ArrayList icons   = new ArrayList();
		
		/// <summary>Hashtable containing the local strings from the main application.</summary>
		static Hashtable localStrings = null;
		static Hashtable localIcons   = null;
		
		/// <summary>Strings resource managers for the current language</summary>
		static ArrayList localStringsResMgrs = new ArrayList();
		/// <summary>Image resource managers for the current language</summary>
		static ArrayList localIconsResMgrs   = new ArrayList();
		
		/// <summary>List of ResourceAssembly</summary>
		static ArrayList resourceAssemblies = new ArrayList();
		
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
		
		/// <summary>
		/// Take string/bitmap resources from an assembly and merge them in the resource service
		/// </summary>
		[Obsolete("Use should use RegisterStrings or RegisterImages instead.")]
		public static void RegisterAssembly(Assembly assembly)
		{
			if (assembly.GetManifestResourceInfo(stringResources+".resources") != null) {
				strings.Add(new ResourceManager(stringResources, assembly));
			}
			
			if (assembly.GetManifestResourceInfo(imageResources+".resources") != null) {
				icons.Add(new ResourceManager(imageResources, assembly));
			}
		}
		
		static void OnPropertyChange(object sender, PropertyChangedEventArgs e)
		{
			if (e.Key == uiLanguageProperty && e.NewValue != e.OldValue) {
				LoadLanguageResources((string)e.NewValue);
				if (LanguageChanged != null)
					LanguageChanged(null, e);
			}
		}
		
		public static event EventHandler LanguageChanged;
		static string currentLanguage;
		
		static void LoadLanguageResources(string language)
		{
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
		
		#region Font loading
		static Font courierNew10;
		
		public static Font CourierNew10 {
			get {
				if (courierNew10 == null)
					courierNew10 = LoadFont("Courier New", 10);
				return courierNew10;
			}
		}
		
		/// <summary>
		/// The LoadFont routines provide a safe way to load fonts.
		/// </summary>
		/// <param name="fontName">The name of the font to load.</param>
		/// <param name="size">The size of the font to load.</param>
		/// <returns>
		/// The font to load or the menu font, if the requested font couldn't be loaded.
		/// </returns>
		public static Font LoadFont(string fontName, int size)
		{
			return LoadFont(fontName, size, FontStyle.Regular);
		}
		
		/// <summary>
		/// The LoadFont routines provide a safe way to load fonts.
		/// </summary>
		/// <param name="fontName">The name of the font to load.</param>
		/// <param name="size">The size of the font to load.</param>
		/// <param name="style">The <see cref="System.Drawing.FontStyle"/> of the font</param>
		/// <returns>
		/// The font to load or the menu font, if the requested font couldn't be loaded.
		/// </returns>
		public static Font LoadFont(string fontName, int size, FontStyle style)
		{
			try {
				return new Font(fontName, size, style);
			} catch (Exception ex) {
				LoggingService.Warn(ex);
				return SystemInformation.MenuFont;
			}
		}
		
		/// <summary>
		/// The LoadFont routines provide a safe way to load fonts.
		/// </summary>
		/// <param name="fontName">The name of the font to load.</param>
		/// <param name="size">The size of the font to load.</param>
		/// <param name="unit">The <see cref="System.Drawing.GraphicsUnit"/> of the font</param>
		/// <returns>
		/// The font to load or the menu font, if the requested font couldn't be loaded.
		/// </returns>
		public static Font LoadFont(string fontName, int size, GraphicsUnit unit)
		{
			return LoadFont(fontName, size, FontStyle.Regular, unit);
		}
		
		/// <summary>
		/// The LoadFont routines provide a safe way to load fonts.
		/// </summary>
		/// <param name="fontName">The name of the font to load.</param>
		/// <param name="size">The size of the font to load.</param>
		/// <param name="style">The <see cref="System.Drawing.FontStyle"/> of the font</param>
		/// <param name="unit">The <see cref="System.Drawing.GraphicsUnit"/> of the font</param>
		/// <returns>
		/// The font to load or the menu font, if the requested font couldn't be loaded.
		/// </returns>
		public static Font LoadFont(string fontName, int size, FontStyle style, GraphicsUnit unit)
		{
			try {
				return new Font(fontName, size, style, unit);
			} catch (Exception ex) {
				LoggingService.Warn(ex);
				return SystemInformation.MenuFont;
			}
		}
		#endregion
		
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
				s = resourceManger.GetString(name);
				if (s != null) {
					break;
				}
			}
			
			if (s == null) {
				foreach (ResourceManager resourceManger in strings) {
					s = resourceManger.GetString(name);
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
		
		static object GetImageResource(string name)
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
						iconobj = resourceManger.GetObject(name);
						if (iconobj != null) {
							break;
						}
					}
				}
			}
			return iconobj;
		}
		
		/// <summary>
		/// Returns a icon from the resource database, it handles localization
		/// transparent for the user. In the resource database can be a bitmap
		/// instead of an icon in the dabase. It is converted automatically.
		/// </summary>
		/// <returns>
		/// The icon in the (localized) resource database.
		/// </returns>
		/// <param name="name">
		/// The name of the requested icon.
		/// </param>
		/// <exception cref="ResourceNotFoundException">
		/// Is thrown when the GlobalResource manager can't find a requested resource.
		/// </exception>
		public static Icon GetIcon(string name)
		{
			object iconobj = GetImageResource(name);
			
			if (iconobj == null) {
				return null;
			}
			if (iconobj is Icon) {
				return (Icon)iconobj;
			} else {
				return Icon.FromHandle(((Bitmap)iconobj).GetHicon());
			}
		}
		
		/// <summary>
		/// Returns a bitmap from the resource database, it handles localization
		/// transparent for the user.
		/// </summary>
		/// <returns>
		/// The bitmap in the (localized) resource database.
		/// </returns>
		/// <param name="name">
		/// The name of the requested bitmap.
		/// </param>
		/// <exception cref="ResourceNotFoundException">
		/// Is thrown when the GlobalResource manager can't find a requested resource.
		/// </exception>
		public static Bitmap GetBitmap(string name)
		{
			Bitmap b = (Bitmap)GetImageResource(name);
			Debug.Assert(b != null, "Resource " + name);
			return b;
		}
	}
}
