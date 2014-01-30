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
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using ICSharpCode.Core;
using System.Windows.Threading;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// TabControl for option panels.
	/// </summary>
	public class TabbedOptions : TabControl, ICanBeDirty
	{
		public void AddOptionPanels(IEnumerable<IOptionPanelDescriptor> dialogPanelDescriptors)
		{
			if (dialogPanelDescriptors == null)
				throw new ArgumentNullException("dialogPanelDescriptors");
			foreach (IOptionPanelDescriptor descriptor in dialogPanelDescriptors) {
				if (descriptor != null) {
					if (descriptor.HasOptionPanel) {
						this.Items.Add(new OptionTabPage(this, descriptor));
					}
					AddOptionPanels(descriptor.ChildOptionPanelDescriptors);
				}
			}
			OnIsDirtyChanged(null, null);
		}
		
		bool oldIsDirty;
		
		public bool IsDirty {
			get {
				return oldIsDirty;
			}
		}
		
		public event EventHandler IsDirtyChanged;
		
		void OnIsDirtyChanged(object sender, EventArgs e)
		{
			bool isDirty = false;
			foreach (IOptionPanel op in OptionPanels) {
				ICanBeDirty dirty = op as ICanBeDirty;
				if (dirty != null && dirty.IsDirty) {
					isDirty = true;
					break;
				}
			}
			if (isDirty != oldIsDirty) {
				oldIsDirty = isDirty;
				if (IsDirtyChanged != null)
					IsDirtyChanged(this, EventArgs.Empty);
			}
		}
		
		public IEnumerable<IOptionPanel> OptionPanels {
			get {
				return
					from tp in Items.OfType<OptionTabPage>()
					where tp.optionPanel != null
					select tp.optionPanel;
			}
		}
		
		sealed class OptionTabPage : TabItem
		{
			IOptionPanelDescriptor descriptor;
			TextBlock placeholder;
			internal IOptionPanel optionPanel;
			TabbedOptions options;
			
			public OptionTabPage(TabbedOptions options, IOptionPanelDescriptor descriptor)
			{
				this.options = options;
				this.descriptor = descriptor;
				string title = StringParser.Parse(descriptor.Label);
				this.Header = title;
				placeholder = new TextBlock { Text = title };
				placeholder.IsVisibleChanged += placeholder_IsVisibleChanged;
				this.Content = placeholder;
			}
			
			void placeholder_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
			{
				Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(LoadPadContentIfRequired));
			}
			
			void LoadPadContentIfRequired()
			{
				if (placeholder != null && placeholder.IsVisible) {
					placeholder = null;
					optionPanel = descriptor.OptionPanel;
					if (optionPanel != null) {
						// some panels initialize themselves on the first LoadOptions call,
						// so we need to load the options before attaching event handlers
						optionPanel.LoadOptions();
						
						ICanBeDirty dirty = optionPanel as ICanBeDirty;
						if (dirty != null)
							dirty.IsDirtyChanged += options.OnIsDirtyChanged;
						SD.WinForms.SetContent(this, optionPanel.Control);
					}
				}
			}
		}
	}
}
