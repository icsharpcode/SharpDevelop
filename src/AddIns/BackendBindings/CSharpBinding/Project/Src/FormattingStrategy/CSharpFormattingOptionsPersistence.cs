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
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;

namespace CSharpBinding.FormattingStrategy
{
	public class CSharpFormattingOptionsPersistenceInitCommand : SimpleCommand
	{
		public override void Execute(object parameter)
		{
			// Initialize CSharpFormattingOptionsPersistence as early as possible (before solution is opened)
			CSharpFormattingOptionsPersistence.Initialize();
		}
	}
	
	/// <summary>
	/// Persistence helper for C# formatting options.
	/// </summary>
	internal class CSharpFormattingOptionsPersistence
	{
		static bool initialized;
		static Dictionary<string, CSharpFormattingOptionsPersistence> projectOptions;
		
		static CSharpFormattingOptionsPersistence()
		{
			Initialize();
		}
		
		public static void Initialize()
		{
			if (initialized)
				return;
			
			initialized = true;
			projectOptions = new Dictionary<string, CSharpFormattingOptionsPersistence>();
			
			// Load global settings
			GlobalOptions = new CSharpFormattingOptionsPersistence(
				SD.PropertyService.MainPropertiesContainer, new CSharpFormattingOptionsContainer()
				{
					DefaultText = StringParser.Parse("${res:CSharpBinding.Formatting.GlobalOptionReference}")
				});
			GlobalOptions.Load();
			
			// Handlers for solution loading/unloading
			var projectService = SD.GetService<IProjectService>();
			if (projectService != null) {
				SD.ProjectService.SolutionOpened += SolutionOpened;
				SD.ProjectService.SolutionClosed += SolutionClosed;
			}
		}
		
		public static CSharpFormattingOptionsPersistence GlobalOptions
		{
			get;
			private set;
		}
		
		public static CSharpFormattingOptionsPersistence SolutionOptions
		{
			get;
			private set;
		}
		
		public static CSharpFormattingOptionsPersistence GetProjectOptions(IProject project)
		{
			var csproject = project as CSharpProject;
			if (csproject != null) {
				string key = project.FileName;
				if (!projectOptions.ContainsKey(key)) {
					// Lazily create options container for project
					projectOptions[key] = new CSharpFormattingOptionsPersistence(
						csproject.GlobalPreferences,
						new CSharpFormattingOptionsContainer((SolutionOptions ?? GlobalOptions).OptionsContainer)
						{
							DefaultText = StringParser.Parse("${res:CSharpBinding.Formatting.ProjectOptionReference}")
						});
				}
				
				return projectOptions[key];
			}
			
			return SolutionOptions ?? GlobalOptions;
		}
		
		static void SolutionOpened(object sender, SolutionEventArgs e)
		{
			// Load solution settings
			SolutionOptions = new CSharpFormattingOptionsPersistence(
				e.Solution.GlobalPreferences,
				new CSharpFormattingOptionsContainer(GlobalOptions.OptionsContainer)
				{
					DefaultText = StringParser.Parse("${res:CSharpBinding.Formatting.SolutionOptionReference}")
				});
		}
		
		static void SolutionClosed(object sender, SolutionEventArgs e)
		{
			SolutionOptions = null;
			projectOptions.Clear();
		}
		
		readonly Properties propertiesContainer;
		CSharpFormattingOptionsContainer optionsContainer;
		CSharpFormattingOptionsContainer optionsContainerWorkingCopy;
		
		/// <summary>
		/// Creates a new instance of formatting options persistence helper, using given options to predefine the options container.
		/// </summary>
		/// <param name="propertiesContainer">Properties container to load from and save to.</param>
		/// <param name="initialContainer">Initial (empty) instance of formatting options container.</param>
		public CSharpFormattingOptionsPersistence(Properties propertiesContainer, CSharpFormattingOptionsContainer initialContainer)
		{
			if (initialContainer == null)
				throw new ArgumentNullException("initialContainer");
			
			this.propertiesContainer = propertiesContainer ?? new Properties();
			optionsContainer = initialContainer;
		}
		
		/// <summary>
		/// Returns the option container managed by this helper.
		/// </summary>
		public CSharpFormattingOptionsContainer OptionsContainer
		{
			get {
				return optionsContainer;
			}
		}
		
		/// <summary>
		/// Starts editing operation by creating a working copy of current formatter settings.
		/// </summary>
		/// <returns>
		/// New working copy of managed options container.
		/// </returns>
		public CSharpFormattingOptionsContainer StartEditing()
		{
			optionsContainerWorkingCopy = optionsContainer.Clone();
			return optionsContainerWorkingCopy;
		}
		
		/// <summary>
		/// Loads formatting settings from properties container.
		/// </summary>
		public void Load()
		{
			optionsContainer.Load(propertiesContainer);
		}
		
		/// <summary>
		/// Saves formatting settings to properties container.
		/// </summary>
		/// <returns><c>True</c> if successful, <c>false</c> otherwise</returns>
		public bool Save()
		{
			// Apply all changes on working copy to main options container
			if (optionsContainerWorkingCopy != null) {
				optionsContainer.CloneFrom(optionsContainerWorkingCopy);
				optionsContainerWorkingCopy = null;
			}
			
			// Convert to SD properties
			optionsContainer.Save(propertiesContainer);
			return true;
		}
	}
}
