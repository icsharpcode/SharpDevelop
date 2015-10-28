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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using Debugger;
using ICSharpCode.Core.Presentation;
using Debugger.AddIn.TreeModel;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.SharpDevelop.Workbench;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	public class CallStackPad : AbstractPadContent
	{
		ListView listView;
		
		public override object Control {
			get { return this.listView; }
		}
		
		public CallStackPad()
		{
			var res = new CommonResources();
			res.InitializeComponent();
			
			listView = new ListView();
			listView.View = (GridView)res["callstackGridView"];
			listView.MouseDoubleClick += listView_MouseDoubleClick;
			listView.SetValue(GridViewColumnAutoSize.AutoWidthProperty, "100%");
			
			listView.ContextMenu = CreateMenu();
			
			WindowsDebugger.RefreshingPads += RefreshPad;
			RefreshPad();
		}

		ContextMenu CreateMenu()
		{
			MenuItem extMethodsItem = new MenuItem();
			extMethodsItem.Header = ResourceService.GetString("MainWindow.Windows.Debug.CallStack.ShowExternalMethods");
			extMethodsItem.IsChecked = DebuggingOptions.Instance.ShowExternalMethods;
			extMethodsItem.Click += delegate {
				extMethodsItem.IsChecked = DebuggingOptions.Instance.ShowExternalMethods = !DebuggingOptions.Instance.ShowExternalMethods;
				WindowsDebugger.RefreshPads();
			};
			
			MenuItem moduleItem = new MenuItem();
			moduleItem.Header = ResourceService.GetString("MainWindow.Windows.Debug.CallStack.ShowModuleNames");
			moduleItem.IsChecked = DebuggingOptions.Instance.ShowModuleNames;
			moduleItem.Click += delegate {
				moduleItem.IsChecked = DebuggingOptions.Instance.ShowModuleNames = !DebuggingOptions.Instance.ShowModuleNames;
				WindowsDebugger.RefreshPads();
			};
			
			MenuItem argNamesItem = new MenuItem();
			argNamesItem.Header = ResourceService.GetString("MainWindow.Windows.Debug.CallStack.ShowArgumentNames");
			argNamesItem.IsChecked = DebuggingOptions.Instance.ShowArgumentNames;
			argNamesItem.Click += delegate {
				argNamesItem.IsChecked = DebuggingOptions.Instance.ShowArgumentNames = !DebuggingOptions.Instance.ShowArgumentNames;
				WindowsDebugger.RefreshPads();
			};
			
			MenuItem argValuesItem = new MenuItem();
			argValuesItem.Header = ResourceService.GetString("MainWindow.Windows.Debug.CallStack.ShowArgumentValues");
			argValuesItem.IsChecked = DebuggingOptions.Instance.ShowArgumentValues;
			argValuesItem.Click += delegate {
				argValuesItem.IsChecked = DebuggingOptions.Instance.ShowArgumentValues = !DebuggingOptions.Instance.ShowArgumentValues;
				WindowsDebugger.RefreshPads();
			};
			
			MenuItem lineItem = new MenuItem();
			lineItem.Header = ResourceService.GetString("MainWindow.Windows.Debug.CallStack.ShowLineNumber");
			lineItem.IsChecked = DebuggingOptions.Instance.ShowLineNumbers;
			lineItem.Click += delegate {
				lineItem.IsChecked = DebuggingOptions.Instance.ShowLineNumbers = !DebuggingOptions.Instance.ShowLineNumbers;
				WindowsDebugger.RefreshPads();
			};
			
			return new ContextMenu() {
				Items = {
					extMethodsItem,
					new Separator(),
					moduleItem,
					argNamesItem,
					argValuesItem,
					lineItem
				}
			};
		}
		
		void listView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			CallStackItem item = listView.SelectedItem as CallStackItem;
			if ((item == null) || (item.Frame == null))
				return;
			
			if (item.Frame.Process.IsPaused) {
				if (item.Frame != null) {
					WindowsDebugger.CurrentStackFrame = item.Frame;
					WindowsDebugger.Instance.JumpToCurrentLine();
					WindowsDebugger.RefreshPads();
				}
			} else {
				MessageService.ShowMessage("${res:MainWindow.Windows.Debug.CallStack.CannotSwitchWhileRunning}", "${res:MainWindow.Windows.Debug.CallStack.FunctionSwitch}");
			}
		}
		
		void RefreshPad()
		{
			Thread thread = WindowsDebugger.CurrentThread;
			if (thread == null) {
				listView.ItemsSource = null;
			} else {
				var items = new ObservableCollection<CallStackItem>();
				bool previousItemIsExternalMethod = false;
				WindowsDebugger.CurrentProcess.EnqueueForEach(
					listView.Dispatcher,
					thread.GetCallstack(100),
					f => items.AddIfNotNull(CreateItem(f, ref previousItemIsExternalMethod))
				);
				listView.ItemsSource = items;
			}
		}
		
		CallStackItem CreateItem(StackFrame frame, ref bool previousItemIsExternalMethod)
		{
			bool showExternalMethods = DebuggingOptions.Instance.ShowExternalMethods;
			var symSource = WindowsDebugger.PdbSymbolSource;
			bool hasSymbols = symSource.Handles(frame.MethodInfo)
				&& !symSource.IsCompilerGenerated(frame.MethodInfo)
				&& frame.NextStatement != null && !string.IsNullOrWhiteSpace(frame.NextStatement.Filename);
			if (showExternalMethods || hasSymbols) {
				// Show the method in the list
				previousItemIsExternalMethod = false;
				return new CallStackItem() {
					Frame = frame,
					ImageSource = SD.ResourceService.GetImageSource("Icons.16x16.Method"),
					Name = GetFullName(frame, hasSymbols),
					HasSymbols = hasSymbols,
				};
			} else {
				// Show [External methods] in the list
				if (previousItemIsExternalMethod)
					return null;
				previousItemIsExternalMethod = true;
				return new CallStackItem() {
					Name = ResourceService.GetString("MainWindow.Windows.Debug.CallStack.ExternalMethods"),
					HasSymbols = false
				};
			}
		}
		
		static string GetFullName(StackFrame frame, bool hasSymbols)
		{
			StringBuilder name = new StringBuilder(64);
			if (DebuggingOptions.Instance.ShowModuleNames) {
				name.Append(frame.MethodInfo.ToString());
				name.Append('!');
			}
			name.Append(frame.MethodInfo.DeclaringType.FullName);
			name.Append('.');
			name.Append(frame.MethodInfo.Name);
			if (DebuggingOptions.Instance.ShowArgumentNames || DebuggingOptions.Instance.ShowArgumentValues) {
				name.Append('(');
				for (int i = 0; i < frame.MethodInfo.Parameters.Count; i++) {
					if (DebuggingOptions.Instance.ShowArgumentNames) {
						name.Append(frame.MethodInfo.Parameters[i].Name);
						if (DebuggingOptions.Instance.ShowArgumentValues) {
							name.Append('=');
						}
					}
					if (DebuggingOptions.Instance.ShowArgumentValues) {
						try {
							name.Append(frame.GetArgumentValue(i, hasSymbols).AsString(100));
						} catch (GetValueException) {
							name.Append(ResourceService.GetString("Global.NA"));
						}
					}
					if (i < frame.ArgumentCount - 1) {
						name.Append(", ");
					}
				}
				name.Append(')');
			}
			if (DebuggingOptions.Instance.ShowLineNumbers) {
				if (frame.NextStatement != null) {
					name.Append(':');
					name.Append(frame.NextStatement.StartLine.ToString());
				}
			}
			return name.ToString();
		}
	}
	
	public class CallStackItem
	{
		public StackFrame Frame { get; set; }
		public ImageSource ImageSource { get; set; }
		public string Name { get; set; }
		public bool HasSymbols { get; set; }
		
		public Brush FontColor {
			get { return this.HasSymbols ? Brushes.Black : Brushes.Gray; }
		}
	}
}
