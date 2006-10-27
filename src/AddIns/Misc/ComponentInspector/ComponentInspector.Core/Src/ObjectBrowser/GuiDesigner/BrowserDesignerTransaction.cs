// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel.Design;

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
