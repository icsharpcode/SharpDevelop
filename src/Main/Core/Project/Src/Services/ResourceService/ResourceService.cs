// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
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
	public sealed class ResourceService 
	{
		readonly static string uiLanguageProperty = "CoreProperties.UILanguage";

		readonly static string stringResources = "StringResources";
		readonly static string imageResources = "BitmapResources";
		
		static string resourceDirctory;
		
		static ResourceService()
		{
			resourceDirctory = FileUtility.Combine(PropertyService.DataDirectory, "resources");
			LoadLanguageResources();
		}
		
		static Hashtable userStrings = null;
		static Hashtable userIcons   = null;
		static Hashtable localUserStrings = null;
		static Hashtable localUserIcons   = null;
		
		static ArrayList strings = new ArrayList();
		static ArrayList icon    = new ArrayList();
		
		static Hashtable localStrings = null;
		static Hashtable localIcons   = null;

		static ArrayList localStringsResMgrs = new ArrayList();
		static ArrayList localIconsResMgrs   = new ArrayList();

		static ArrayList assemblies = new ArrayList();
		
		static void ChangeProperty(object sender, PropertyChangedEventArgs e)
		{
			if (e.Key == uiLanguageProperty && e.OldValue != e.NewValue) {
			    LoadLanguageResources();
			} 
		}
		
		static void LoadLanguageResources()
		{
			string language = PropertyService.Get(uiLanguageProperty, Thread.CurrentThread.CurrentUICulture.Name);
			
			try {
				Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(language);
			} catch (Exception) {
				try {
					Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(language.Split('-')[0]);
				} catch (Exception) {}
			}
// TODO: AppSettings is obsolete
//			if (System.Configuration.ConfigurationSettings.AppSettings["UserStrings"] != null) {
//				string resourceName = System.Configuration.ConfigurationSettings.AppSettings["UserStrings"];
//				resourceName = resourceName.Insert(resourceName.LastIndexOf(".resources"), "." + language);
//				localUserStrings = Load(resourceDirctory +  Path.DirectorySeparatorChar + resourceName);
//			}
//			if (System.Configuration.ConfigurationSettings.AppSettings["UserIcons"] != null) {
//				string iconResourceName = System.Configuration.ConfigurationSettings.AppSettings["UserIcons"];
//				iconResourceName = resourceName.Insert(resourceName.LastIndexOf(".resources"), "." + language);
//				localUserIcons   = Load(resourceDirctory +  Path.DirectorySeparatorChar + iconResourceName);
//			}

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
			Assembly assembly = Assembly.GetEntryAssembly();
			if (assembly != null) {
				if (assembly.GetManifestResourceInfo("Resources." + stringResources + ".resources") != null) {
					localStringsResMgrs.Add(new ResourceManager("Resources." + stringResources, assembly));
				}
				
				if (assembly.GetManifestResourceInfo("Resources." + imageResources + ".resources") != null) {	
					localIconsResMgrs.Add(new ResourceManager("Resources." + imageResources, assembly));
				}
			}
		}
		
		static void InitializeService()
		{
			RegisterAssembly(Assembly.GetEntryAssembly());
			
			PropertyService.PropertyChanged += new PropertyChangedEventHandler(ChangeProperty);
			
			LoadLanguageResources();
		}
		
		// core service : Can't use Initialize, because all other stuff needs this service before initialize is called.
		ResourceService()
		{
//			if (System.Configuration.ConfigurationSettings.AppSettings["UserStrings"] != null) {
//				userStrings = Load(resourceDirctory +  Path.DirectorySeparatorChar + System.Configuration.ConfigurationSettings.AppSettings["UserStrings"]);
//			}
//			if (System.Configuration.ConfigurationSettings.AppSettings["UserIcons"] != null) {
//				userIcons   = Load(resourceDirctory +  Path.DirectorySeparatorChar + System.Configuration.ConfigurationSettings.AppSettings["UserIcons"]);
//			}
			InitializeService();
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
			} catch (Exception) {
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
			} catch (Exception) {
				return SystemInformation.MenuFont;
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
			return Load(resourceDirctory + Path.DirectorySeparatorChar + name + "." + language + ".resources");
			
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
			if (localUserStrings != null && localUserStrings[name] != null) {
				return localUserStrings[name].ToString();
			}
			if (userStrings != null && userStrings[name] != null) {
				return userStrings[name].ToString();
			}
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
		
		/// <summary>
		/// Take string/bitmap resources from an assembly and merge them in the resource service
		/// </summary>
		public static void RegisterAssembly(Assembly assembly)
		{
			assemblies.Add(assembly.FullName);

			if (assembly.GetManifestResourceInfo(stringResources+".resources") != null) {
				strings.Add(new ResourceManager(stringResources, assembly));
			}
			
			if (assembly.GetManifestResourceInfo(imageResources+".resources") != null) {
				icon.Add(new ResourceManager(imageResources, assembly));
			}
		}
		
		static object GetImageResource(string name)
		{
			object iconobj = null;
			if (localUserIcons != null && localUserIcons[name] != null) {
				iconobj = localUserIcons[name];
			} else  if (userIcons != null && userIcons[name] != null) {
				iconobj = userIcons[name];
			} else  if (localIcons != null && localIcons[name] != null) {
				iconobj = localIcons[name];
			} else {
				foreach (ResourceManager resourceManger in localIconsResMgrs) {
					iconobj = resourceManger.GetObject(name);
					if (iconobj != null) {
						break;
					}
				}

				if (iconobj == null) {
					foreach (ResourceManager resourceManger in icon) {
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
