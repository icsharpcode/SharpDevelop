// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 3335 $</version>
// </file>

using System;
using System.Windows.Input;

namespace ICSharpCode.WpfDesign.Designer.Services
{
	// See IToolService for description.
	class ToolService : IToolService, IDisposable
	{
		public static ToolService Instance = new ToolService();
		public event EventHandler CurrentToolChanged;

		ITool currentTool = Services.PointerTool.Instance;
		IDesignPanel designPanel;		

		public ITool PointerTool
		{
			get { return Services.PointerTool.Instance; }
		}

		public ITool CurrentTool
		{
			get { return currentTool; }
			set
			{
				if (value == null) {
					throw new ArgumentNullException("value");
				}
				if (currentTool != value) {
					
					DeactivateCurrentTool();
					currentTool = value;
					ActivateCurrentTool();

					if (CurrentToolChanged != null) {
						CurrentToolChanged(this, EventArgs.Empty);
					}
				}
			}
		}

		public void Reset()
		{
			CurrentTool = PointerTool;
		}

		public void SwitchDesignPanel(IDesignPanel newDesignPanel)
		{
			DeactivateCurrentTool();
			designPanel = newDesignPanel;
			ActivateCurrentTool();
		}

		public void Dispose()
		{
			DeactivateCurrentTool();
		}

		void ActivateCurrentTool()
		{
			if (designPanel != null) {
				currentTool.Activate(designPanel);
			}
		}

		void DeactivateCurrentTool()
		{
			if (designPanel != null) {
				currentTool.Deactivate(designPanel);
			}
		}
	}
}
