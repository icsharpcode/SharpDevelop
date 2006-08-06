/*
 * Created by SharpDevelop.
 * User: Daniel Grunwald
 * Date: 28.07.2006
 * Time: 23:10
 */
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.Gui;

namespace SharpDevelopInteraction
{
	/// <summary>
	/// If you want to control SharpDevelop's internals from the host application,
	/// you need to use a wrapper assembly and class like this.
	/// Make sure to inherit from MarshalByRefObject and create the instance using
	/// host.CreateInstanceInTargetDomain&lt;T&gt; in the host application.
	/// 
	/// The class itself will be responsible to use WorkbenchSingleton.SafeThread(Async)Call
	/// for all operations if SharpDevelop is running on its own thread.
	/// </summary>
	public class InteractionClass : MarshalByRefObject
	{
		public void MakeTransparent()
		{
			WorkbenchSingleton.SafeThreadAsyncCall(MakeTransparentInternal);
		}
		
		void MakeTransparentInternal()
		{
			WorkbenchSingleton.MainForm.Opacity *= 0.85;
			if (WorkbenchSingleton.MainForm.Opacity < 0.2)
				WorkbenchSingleton.MainForm.Opacity = 1;
		}
	}
}
