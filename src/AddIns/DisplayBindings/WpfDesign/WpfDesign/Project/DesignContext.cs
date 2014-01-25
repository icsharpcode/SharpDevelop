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
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;

namespace ICSharpCode.WpfDesign
{
	/// <summary>
	/// The context that the designer uses.
	/// </summary>
	public abstract class DesignContext
	{
		readonly ServiceContainer _services = new ServiceContainer();
		
		/// <summary>
		/// Creates a new DesignContext instance.
		/// </summary>
		protected DesignContext()
		{
			_services.AddService(typeof(Extensions.ExtensionManager), new Extensions.ExtensionManager(this));
		}
		
		/// <summary>
		/// Gets the <see cref="ServiceContainer"/>.
		/// </summary>
		public ServiceContainer Services {
			[DebuggerStepThrough]
			get { return _services; }
		}
		
		/// <summary>
		/// Gets the root design item.
		/// </summary>
		public abstract DesignItem RootItem {
			get;
		}

		/// <summary>
		/// Save the designed elements as XML.
		/// </summary>
		public abstract void Save(XmlWriter writer);
		
		/// <summary>
		/// Opens a new change group used to batch several changes.
		/// ChangeGroups work as transactions and are used to support the Undo/Redo system.
		/// </summary>
		public abstract ChangeGroup OpenGroup(string changeGroupTitle, ICollection<DesignItem> affectedItems);
	}
}
