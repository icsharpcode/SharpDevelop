// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;

namespace ICSharpCode.Reports.Addin.Designer
{
	/// <summary>
	/// Description of SmartTagTransactions.
	/// </summary>
	public class SmartTagTransactions
	{
		IDesignerHost host;
		IComponentChangeService changeService;
		DesignerTransaction transaction;
		Control ctrl;
		DesignerActionList actionList;
		
		public SmartTagTransactions(string transactionname,DesignerActionList list,Control ctrl)
		{
			
			this.actionList = list;
			this.ctrl = ctrl;
			host = (IDesignerHost)this.actionList.GetService(typeof(IDesignerHost));
			this.transaction = host.CreateTransaction(transactionname);
			changeService = (IComponentChangeService)this.actionList.GetService(typeof(IComponentChangeService));
			changeService.OnComponentChanging(ctrl,null);                                                      
		}
		
		
		
		public void Commit ()
		{
			changeService.OnComponentChanged (ctrl,null,null,null);
			this.transaction.Commit();
			DesignerActionUIService designerActionUISvc = 
				(DesignerActionUIService)this.actionList.GetService(typeof(DesignerActionUIService));
			designerActionUISvc.Refresh(this.actionList.Component);
				                                                    
		}
	}
}
