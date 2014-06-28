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
using System.ComponentModel;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;

namespace CSharpBinding.FormattingStrategy
{
	public class CSharpFormattingOptionsPoliciesInitCommand : SimpleCommand
	{
		public override void Execute(object parameter)
		{
			// Initialize CSharpFormattingPolicies as early as possible (before solution is opened)
			CSharpFormattingPolicies.Instance.Initialize();
		}
	}
	
	/// <summary>
	/// Management class for formatting policies.
	/// </summary>
	internal class CSharpFormattingPolicies
	{
		static readonly Lazy<CSharpFormattingPolicies> LazyInstance =
			new Lazy<CSharpFormattingPolicies>(() => new CSharpFormattingPolicies());
		
		public static CSharpFormattingPolicies Instance {
			get { return LazyInstance.Value; }
		}
		
		public event EventHandler<CSharpFormattingPolicyUpdateEventArgs> FormattingPolicyUpdated;
		
		bool initialized;
		Dictionary<string, CSharpFormattingPolicy> projectOptions;
		
		public CSharpFormattingPolicies()
		{
			// Load global settings
			GlobalOptions = new CSharpFormattingPolicy(
				SD.PropertyService.MainPropertiesContainer, new CSharpFormattingOptionsContainer() {
					DefaultText = StringParser.Parse("${res:CSharpBinding.Formatting.GlobalOptionReference}")
				});
			GlobalOptions.FormattingPolicyUpdated += OnFormattingPolicyUpdated;
			GlobalOptions.Load();
		}
		
		public void Initialize()
		{
			if (initialized)
				return;
			
			initialized = true;
			projectOptions = new Dictionary<string, CSharpFormattingPolicy>();
			
			// Handlers for solution loading/unloading
			var projectService = SD.GetService<IProjectService>();
			if (projectService != null) {
				SD.ProjectService.SolutionOpened += SolutionOpened;
				SD.ProjectService.SolutionClosed += SolutionClosed;
			}
		}
		
		public static bool AutoFormatting {
			get {
				return SD.PropertyService.Get("CSharpBinding.Formatting.AutoFormatting", false);
			}
			set {
				SD.PropertyService.Set("CSharpBinding.Formatting.AutoFormatting", value);
			}
		}
		
		public CSharpFormattingPolicy GlobalOptions {
			get;
			private set;
		}
		
		public CSharpFormattingPolicy SolutionOptions {
			get;
			private set;
		}
		
		public CSharpFormattingPolicy GetProjectOptions(IProject project)
		{
			if (!initialized)
				return GlobalOptions;
			
			var csproject = project as CSharpProject;
			if (csproject != null) {
				string key = project.FileName;
				if (!projectOptions.ContainsKey(key)) {
					// Lazily create options container for project
					var projectFormattingPersistence = new CSharpFormattingPolicy(
						csproject.GlobalPreferences,
						new CSharpFormattingOptionsContainer((SolutionOptions ?? GlobalOptions).OptionsContainer) {
							DefaultText = StringParser.Parse("${res:CSharpBinding.Formatting.ProjectOptionReference}")
						});
					projectFormattingPersistence.FormattingPolicyUpdated += OnFormattingPolicyUpdated;
					projectFormattingPersistence.Load();
					projectOptions[key] = projectFormattingPersistence;
				}
				
				return projectOptions[key];
			}
			
			return SolutionOptions ?? GlobalOptions;
		}
		
		void SolutionOpened(object sender, SolutionEventArgs e)
		{
			// Load solution settings
			SolutionOptions = new CSharpFormattingPolicy(
				e.Solution.SDSettings,
				new CSharpFormattingOptionsContainer(GlobalOptions.OptionsContainer) {
					DefaultText = StringParser.Parse("${res:CSharpBinding.Formatting.SolutionOptionReference}")
				});
			SolutionOptions.FormattingPolicyUpdated += OnFormattingPolicyUpdated;
			SolutionOptions.Load();
		}
		
		void SolutionClosed(object sender, SolutionEventArgs e)
		{
			SolutionOptions.FormattingPolicyUpdated -= OnFormattingPolicyUpdated;
			SolutionOptions = null;
			projectOptions.Clear();
		}
		
		void OnFormattingPolicyUpdated(object sender, CSharpFormattingPolicyUpdateEventArgs e)
		{
			if (FormattingPolicyUpdated != null) {
				FormattingPolicyUpdated(sender, e);
			}
		}
	}
	
	/// <summary>
	/// Contains event data for formatting policy update events.
	/// </summary>
	internal class CSharpFormattingPolicyUpdateEventArgs : EventArgs
	{
		public CSharpFormattingPolicyUpdateEventArgs(CSharpFormattingOptionsContainer container)
		{
			OptionsContainer = container;
		}
		
		/// <summary>
		/// Returns updated options container.
		/// </summary>
		public CSharpFormattingOptionsContainer OptionsContainer
		{
			get;
			private set;
		}
	}
	
	/// <summary>
	/// Persistence helper for C# formatting options of a certain policy (e.g. global, solution, project).
	/// </summary>
	internal class CSharpFormattingPolicy
	{
		public event EventHandler<CSharpFormattingPolicyUpdateEventArgs> FormattingPolicyUpdated;
		
		readonly Properties propertiesContainer;
		readonly CSharpFormattingOptionsContainer optionsContainer;
		CSharpFormattingOptionsContainer optionsContainerWorkingCopy;
		
		/// <summary>
		/// Creates a new instance of formatting options policy, using given options to predefine the options container.
		/// </summary>
		/// <param name="propertiesContainer">Properties container to load from and save to.</param>
		/// <param name="initialContainer">Initial (empty) instance of formatting options container.</param>
		public CSharpFormattingPolicy(Properties propertiesContainer, CSharpFormattingOptionsContainer initialContainer)
		{
			if (initialContainer == null)
				throw new ArgumentNullException("initialContainer");
			
			this.propertiesContainer = propertiesContainer ?? new Properties();
			optionsContainer = initialContainer;
		}
		
		/// <summary>
		/// Returns the option container for this policy.
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
			OnFormattingPolicyUpdated(this, optionsContainer);
			return true;
		}
		
		void OnFormattingPolicyUpdated(object sender, CSharpFormattingOptionsContainer container)
		{
			if (FormattingPolicyUpdated != null) {
				FormattingPolicyUpdated(sender, new CSharpFormattingPolicyUpdateEventArgs(container));
			}
		}
	}
}
