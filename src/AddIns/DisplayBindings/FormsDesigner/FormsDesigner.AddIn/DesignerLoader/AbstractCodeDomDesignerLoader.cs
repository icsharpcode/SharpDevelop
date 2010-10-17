// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.FormsDesigner
{
	/// <summary>
	/// Description of AbstractCodeDomDesignerLoader.
	/// </summary>
	public class AbstractCodeDomDesignerLoader : IDesignerLoader
	{
		/// <summary>
		/// When overridden in derived classes, this method should return the current
		/// localization model of the designed file or None, if it cannot be determined.
		/// </summary>
		/// <returns>The default implementation always returns None.</returns>
		protected virtual CodeDomLocalizationModel GetCurrentLocalizationModelFromDesignedFile()
		{
			return CodeDomLocalizationModel.None;
		}
		
		System.CodeDom.CodeCompileUnit IDesignerLoader.Parse()
		{
			throw new NotImplementedException();
		}
		
		void IDesignerLoader.Write(System.CodeDom.CodeCompileUnit unit)
		{
			throw new NotImplementedException();
		}
		
		CodeDomLocalizationModel IDesignerLoader.GetLocalizationModel()
		{
			CodeDomLocalizationModel model = FormsDesigner.Gui.OptionPanels.LocalizationModelOptionsPanel.DefaultLocalizationModel;
			
			if (FormsDesigner.Gui.OptionPanels.LocalizationModelOptionsPanel.KeepLocalizationModel) {
				// Try to find out the current localization model of the designed form
				CodeDomLocalizationModel existingModel = this.GetCurrentLocalizationModelFromDesignedFile();
				if (existingModel != CodeDomLocalizationModel.None) {
					LoggingService.Debug("Determined existing localization model, using that: " + existingModel.ToString());
					model = existingModel;
				} else {
					LoggingService.Debug("Could not determine existing localization model, using default: " + model.ToString());
				}
			} else {
				LoggingService.Debug("Using default localization model: " + model.ToString());
			}
			return model;
		}
	}
}
