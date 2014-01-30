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
using System.Windows.Input;

namespace ICSharpCode.WpfDesign.Designer.Services
{
	// See IToolService for description.
	sealed class DefaultToolService : IToolService, IDisposable
	{
		ITool _currentTool;
		IDesignPanel _designPanel;
		
		public DefaultToolService(DesignContext context)
		{
			_currentTool = this.PointerTool;
			context.Services.RunWhenAvailable<IDesignPanel>(
				delegate(IDesignPanel designPanel) {
					_designPanel = designPanel;
					_currentTool.Activate(designPanel);
				});
		}
		
		public void Dispose()
		{
			if (_designPanel != null) {
				_currentTool.Deactivate(_designPanel);
				_designPanel = null;
			}
		}
		
		public ITool PointerTool {
			get { return Services.PointerTool.Instance; }
		}
		
		public ITool CurrentTool {
			get { return _currentTool; }
			set {
				if (value == null)
					throw new ArgumentNullException("value");
				if (_currentTool == value) return;
				if (_designPanel != null) {
					_currentTool.Deactivate(_designPanel);
				}
				_currentTool = value;
				if (_designPanel != null) {
					_currentTool.Activate(_designPanel);
				}
				if (CurrentToolChanged != null) {
					CurrentToolChanged(this, EventArgs.Empty);
				}
			}
		}
		
		public event EventHandler CurrentToolChanged;

		public IDesignPanel DesignPanel {
			get { return _designPanel; }
		}
	}
}
