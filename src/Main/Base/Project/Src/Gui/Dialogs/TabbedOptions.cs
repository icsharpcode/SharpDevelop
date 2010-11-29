// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
					if (descriptor.ChildOptionPanelDescriptors != null) {
						AddOptionPanels(descriptor.ChildOptionPanelDescriptors);
					}
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
						this.SetContent(optionPanel.Control);
					}
				}
			}
		}
	}
}
