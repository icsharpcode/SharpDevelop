// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
