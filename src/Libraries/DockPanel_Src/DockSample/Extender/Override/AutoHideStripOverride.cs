using System;
using WeifenLuo.WinFormsUI;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace DockSample
{
	public class AutoHideStripOverride : AutoHideStripVS2003
	{
		protected internal AutoHideStripOverride(DockPanel dockPanel) : base(dockPanel)
		{
			BackColor = SystemColors.ControlLight;
		}
	}
}
