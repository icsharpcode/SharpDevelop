// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using ICSharpCode.Core.Services;
using ICSharpCode.Core.WinForms;

namespace PythonBinding.Tests.Utils
{
	public class DummyServiceManager : ServiceManager
	{
		WinFormsMessageService messageService = new WinFormsMessageService();
		
		public DummyServiceManager()
		{
		}
		
		public override IMessageService MessageService {
			get { return messageService; }
		}
		
		public override object GetService(Type serviceType)
		{
			return null;
		}
	}
}
