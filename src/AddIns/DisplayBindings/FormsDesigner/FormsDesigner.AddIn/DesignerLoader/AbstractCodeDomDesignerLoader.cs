// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel.Design.Serialization;

namespace ICSharpCode.FormsDesigner
{
	/// <summary>
	/// Description of AbstractCodeDomDesignerLoader.
	/// </summary>
	public abstract class AbstractCodeDomDesignerLoader : IDesignerLoader
	{
		protected AbstractCodeDomDesignerLoader(IDesignerGenerator generator)
		{
			this.Generator = generator;
		}
		
		public IDesignerGenerator Generator { get; set; }
		
		/// <summary>
		/// When overridden in derived classes, this method should return the current
		/// localization model of the designed file or None, if it cannot be determined.
		/// </summary>
		/// <returns>The default implementation always returns None.</returns>
		protected virtual CodeDomLocalizationModel GetCurrentLocalizationModelFromDesignedFile()
		{
			return CodeDomLocalizationModel.None;
		}
		
		public abstract System.CodeDom.CodeCompileUnit Parse();
		
		public abstract void Write(System.CodeDom.CodeCompileUnit unit);
		
		CodeDomLocalizationModel IDesignerLoader.GetLocalizationModel()
		{
			CodeDomLocalizationModel model = FormsDesigner.Gui.OptionPanels.LocalizationModelOptionsPanel.DefaultLocalizationModel;
			
			if (FormsDesigner.Gui.OptionPanels.LocalizationModelOptionsPanel.KeepLocalizationModel) {
				// Try to find out the current localization model of the designed form
				CodeDomLocalizationModel existingModel = this.GetCurrentLocalizationModelFromDesignedFile();
				if (existingModel != CodeDomLocalizationModel.None) {
					Core.LoggingService.Debug("Determined existing localization model, using that: " + existingModel.ToString());
					model = existingModel;
				} else {
					Core.LoggingService.Debug("Could not determine existing localization model, using default: " + model.ToString());
				}
			} else {
				Core.LoggingService.Debug("Using default localization model: " + model.ToString());
			}
			return model;
		}
		
		public virtual bool IsReloadNeeded(bool value)
		{
			return value;
		}
	}
}
