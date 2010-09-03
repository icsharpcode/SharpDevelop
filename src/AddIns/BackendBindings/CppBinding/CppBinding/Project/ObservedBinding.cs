// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.SharpDevelop.Project;
using System.Windows.Forms;

namespace ICSharpCode.CppBinding.Project
{
	public class ObservedBinding<Output, ControlType> : ConfigurationGuiBinding
		where ControlType : Control
	{
		public delegate Output ObserverDelegate(ControlType c);
		public delegate void LoaderDelegate(ControlType c);
		
		public ObservedBinding(ControlType control, ObserverDelegate saveDelegate) : this(control, saveDelegate, null)
		{			
		}
		
		public ObservedBinding(ControlType control, ObserverDelegate saveDelegate, LoaderDelegate loadDelegate) {
			this.control = control;
			this.onLoad = loadDelegate;
			this.onSave = saveDelegate;
		}
		
		public override void Load() {
			if (onLoad != null)
				onLoad(control);
		}
		
		public override bool Save()
		{            
			if (onSave != null)
				if (Property != null)
					base.Set<Output>(onSave(control));			
				else
					onSave(control);
			return true;
		}
		
		private ControlType control;
		private LoaderDelegate onLoad;
		private ObserverDelegate onSave;
	}
}
