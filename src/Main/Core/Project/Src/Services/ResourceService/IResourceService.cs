// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Reflection;
using System.Resources;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Provides string and bitmap resources.
	/// </summary>
	[SDService("SD.ResourceService", FallbackImplementation = typeof(FallbackResourceService))]
	public interface IResourceService
	{
		/// <summary>
		/// Gets/Sets the current UI language.
		/// </summary>
		string Language { get; set; }
		
		event EventHandler LanguageChanged;
		
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
		string GetString(string name);
		
		object GetImageResource(string name);
		
				
		/// <summary>
		/// Registers string resources in the resource service.
		/// </summary>
		/// <param name="baseResourceName">The base name of the resource file embedded in the assembly.</param>
		/// <param name="assembly">The assembly which contains the resource file.</param>
		/// <example><c>ResourceService.RegisterStrings("TestAddin.Resources.StringResources", GetType().Assembly);</c></example>
		void RegisterStrings(string baseResourceName, Assembly assembly);
		
		void RegisterNeutralStrings(ResourceManager stringManager);
		
		/// <summary>
		/// Registers image resources in the resource service.
		/// </summary>
		/// <param name="baseResourceName">The base name of the resource file embedded in the assembly.</param>
		/// <param name="assembly">The assembly which contains the resource file.</param>
		/// <example><c>ResourceService.RegisterImages("TestAddin.Resources.BitmapResources", GetType().Assembly);</c></example>
		void RegisterImages(string baseResourceName, Assembly assembly);
		
		void RegisterNeutralImages(ResourceManager imageManager);
	}
	
	sealed class FallbackResourceService : IResourceService
	{
		event EventHandler IResourceService.LanguageChanged { add {} remove {} }
		
		string IResourceService.Language {
			get { return "en"; }
			set {
				throw new NotImplementedException();
			}
		}
		
		string IResourceService.GetString(string name)
		{
			return null;
		}
		
		object IResourceService.GetImageResource(string name)
		{
			return null;
		}
		
		void IResourceService.RegisterStrings(string baseResourceName, Assembly assembly)
		{
			throw new NotImplementedException();
		}
		
		void IResourceService.RegisterNeutralStrings(ResourceManager stringManager)
		{
			throw new NotImplementedException();
		}
		
		void IResourceService.RegisterImages(string baseResourceName, Assembly assembly)
		{
			throw new NotImplementedException();
		}
		
		void IResourceService.RegisterNeutralImages(ResourceManager imageManager)
		{
			throw new NotImplementedException();
		}
	}
}
