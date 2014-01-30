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
