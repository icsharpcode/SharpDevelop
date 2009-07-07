/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: trecio
 * Data: 2009-07-06
 * Godzina: 22:31
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.Core;
using System.Windows.Forms;

namespace CppBinding.Project
{
	/// <summary>
	/// Application settings panel for c++ project.
	/// </summary>
	public class ApplicationOptions : ICSharpCode.SharpDevelop.Gui.OptionPanels.ApplicationSettings
	{
		public override void LoadPanelContents()
		{
			base.LoadPanelContents();
			ComboBox cbOutputType = Get<ComboBox>("outputType");
			helper.AddBinding("ConfigurationType", new ObservedBinding<string, ComboBox>(cbOutputType, ConvertOutputType));
			
			MSBuildItemDefinitionGroup group = new MSBuildItemDefinitionGroup(project, helper.Configuration, helper.Platform);
			string subsystem = group.GetElementMetadata("Link", "SubSystem");
			string configurationType = project.GetEvaluatedProperty("ConfigurationType");
			OutputType validOutputType = ConfigurationTypeToOutputType(configurationType, subsystem);
			cbOutputType.SelectedIndex = Array.IndexOf((OutputType[])Enum.GetValues(typeof(OutputType)), validOutputType);
			IsDirty = false;
		}
		
		#region OutputType <-> ConfigurationType property mapping
		
		/// <summary>
		/// Applies the OutputType property value from combo box control to the vcxproj project.
		/// <para>The OutputType property is translated to ConfigurationType and Subsystem properties</para>
		/// </summary>
		/// <returns>the ConfigurationType associated to OutputType</returns>
		string ConvertOutputType(ComboBox cbOutputType)
		{
			OutputType[] values = (OutputType[])Enum.GetValues(typeof(OutputType));
			OutputType outputType = values[cbOutputType.SelectedIndex];
			
			string subsystem = OutputTypeToSubsystem(outputType);
			MSBuildItemDefinitionGroup group = new MSBuildItemDefinitionGroup(project, 
			                                            helper.Configuration, helper.Platform);
			group.SetElementMetadata("Link", "SubSystem", subsystem);
			
			return OutputTypeToConfigurationType(outputType);
		}
		
		static string OutputTypeToConfigurationType(OutputType outputType)
		{
			switch (outputType)
			{
			    case OutputType.Exe:
			        return "Application";
			    case OutputType.Library:
			        return "DynamicLibrary";
			    case OutputType.Module:
			        //TODO: get an apropriate way to handle netmodule creation
			        //see: http://msdn.microsoft.com/en-us/library/k669k83h(VS.80).aspx
			        LoggingService.Info(".netmodule output not supported, will produce a class library");
			        return "DynamicLibrary";
			    case OutputType.WinExe:
			        return "Application";
			}
			throw new ArgumentException("Unknown OutputType value " + outputType);
		}
		
		static string OutputTypeToSubsystem(OutputType outputType)
		{
			if (OutputType.WinExe == outputType)
			    return "Windows";
			return "Console";
		}
		
		static OutputType ConfigurationTypeToOutputType(string configurationType, string subsystem)
		{
			if ("Application" == configurationType && "Windows" != subsystem)
			    return OutputType.Exe;
			else if ("Application" == configurationType && "Windows" == subsystem)
			    return OutputType.WinExe;
			else if ("DynamicLibrary" == configurationType)
			    return OutputType.Library;
			LoggingService.Info("ConfigurationType " +configurationType + " are not supported, will use Library output type");
			return OutputType.Library;
		}
		#endregion
    }
}
