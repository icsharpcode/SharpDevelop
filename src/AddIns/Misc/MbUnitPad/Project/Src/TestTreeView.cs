// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using ICSharpCode.Core;
using MbUnit.Forms;
using MbUnit.Core.Remoting;

namespace MbUnitPad
{
	public class TestTreeView : ReflectorTreeView
	{
		public TestTreeView()
		{
			TypeTree.ContextMenu = null;
			TypeTree.ContextMenuStrip = MenuService.CreateContextMenu(this, "/MbUnitPad/ContextMenu");
		}
		
		/// <summary>
		/// Default MbUnit-GUI doesn't use shadow copy, we have to override that behaviour.
		/// </summary>
		public new void AddAssembly(string file)
		{
			if (this.TestDomains.ContainsTestAssembly(file)) {
				throw new ApplicationException(string.Format("The file {0} is already loaded.", file));
			} else {
				TreeTestDomain domain = this.TestDomains.Add(file);
				domain.ShadowCopyFiles = true;
				this.TestDomains.Watcher.Start();
			}
		}
		
		protected override void MessageOnStatusBar(string message, object[] args)
		{
			if (message.Length == 0) {
				StatusBarService.SetMessage(null);
			} else {
				string msg = string.Format(message, args);
				LoggingService.Debug(msg);
				StatusBarService.SetMessage("MbUnit: " + msg);
			}
		}
	}
}
