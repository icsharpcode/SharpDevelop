// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Diagnostics;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using NoGoop.Win32;
using NoGoop.Util;

namespace NoGoop.ObjBrowser.GuiDesigner
{

	public class BrowserDesignerTransaction : DesignerTransaction
	{

        DesignerHost                _host;

        public BrowserDesignerTransaction(DesignerHost host) : base()
        {
            _host = host;
        }

        public BrowserDesignerTransaction(DesignerHost host, String name) : base(name)
        {
            _host = host;
        }

        protected override void OnCommit()
        {
            _host.CompleteTransaction();
        }

        protected override void OnCancel()
        {
            _host.CompleteTransaction();
        }

	}

}
