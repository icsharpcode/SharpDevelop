// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Internal.ExternalTool;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Core;

namespace CPPBinding
{
	public abstract class AbstractCPPConfigPanel : AbstractOptionPanel
	{
		protected CPPCompilerParameters compilerParameters;
		protected System.Windows.Forms.PropertyGrid grid = new System.Windows.Forms.PropertyGrid();
			
		protected abstract void SetGridObject();
		
		public override void LoadPanelContents()
		{
			compilerParameters = (CPPCompilerParameters)((Properties)CustomizationObject).Get("Config");
			
			grid.Dock           = DockStyle.Fill;
			SetGridObject();
			Controls.Add(grid);
		}
		
		public override bool StorePanelContents()
		{
			return true;
		}
	}
	
	public class CPPCodeGenerationPanel : AbstractCPPConfigPanel
	{
		protected override void SetGridObject()
		{
			grid.SelectedObject = this.compilerParameters; 
		}
	}
	#region compiler panels
	public class GeneralCPPOptionsPanel : AbstractCPPConfigPanel
	{
		protected override void SetGridObject()
		{
			grid.SelectedObject = this.compilerParameters.generalCPPOptions; 
		}
	}
	public class OptimizeCPPOptionsPanel : AbstractCPPConfigPanel
	{
		protected override void SetGridObject()
		{
			grid.SelectedObject = this.compilerParameters.optimizeCPPOptions; 
		}
	}
	public class PreProcessorCPPOptionsPanel : AbstractCPPConfigPanel
	{
		protected override void SetGridObject()
		{
			grid.SelectedObject = this.compilerParameters.preProcessorCPPOptions; 
		}
	}
	
	public class CodeGenerationCPPOptionsPanel : AbstractCPPConfigPanel
	{
		protected override void SetGridObject()
		{
			grid.SelectedObject = this.compilerParameters.codeGenerationCPPOptions; 
		}
	}
	
	public class LanguageCPPOptionsPanel : AbstractCPPConfigPanel
	{
		protected override void SetGridObject()
		{
			grid.SelectedObject = this.compilerParameters.languageCPPOptions; 
		}
	}
	
	public class PreCompiledHeaderCPPOptionsPanel : AbstractCPPConfigPanel
	{
		protected override void SetGridObject()
		{
			grid.SelectedObject = this.compilerParameters.preCompiledHeaderCPPOptions; 
		}
	}
	
	public class OutputFileCPPOptionsPanel : AbstractCPPConfigPanel
	{
		protected override void SetGridObject()
		{
			grid.SelectedObject = this.compilerParameters.outputFileCPPOptions; 
		}
	}
	
	public class InformationSearchCPPOptionsPanel : AbstractCPPConfigPanel
	{
		protected override void SetGridObject()
		{
			grid.SelectedObject = this.compilerParameters.informationSearchCPPOptions; 
		}
	}
	
	public class ExtendedCPPOptionsPanel : AbstractCPPConfigPanel
	{
		protected override void SetGridObject()
		{
			grid.SelectedObject = this.compilerParameters.extendedCPPOptions; 
		}
	}
	#endregion

	public class GeneralLinkerOptionsPanel : AbstractCPPConfigPanel
	{
		protected override void SetGridObject()
		{
			grid.SelectedObject = this.compilerParameters.generalLinkerOptions; 
		}
	}
	public class InputLinkerOptionsPanel : AbstractCPPConfigPanel
	{
		protected override void SetGridObject()
		{
			grid.SelectedObject = this.compilerParameters.inputLinkerOptions; 
		}
	}
	public class DebugLinkerOptionsPanel : AbstractCPPConfigPanel
	{
		protected override void SetGridObject()
		{
			grid.SelectedObject = this.compilerParameters.debugLinkerOptions; 
		}
	}

	public class SystemLinkerOptionsPanel : AbstractCPPConfigPanel
	{
		protected override void SetGridObject()
		{
			grid.SelectedObject = this.compilerParameters.systemLinkerOptions; 
		}
	}

}

