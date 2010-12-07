// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using Debugger;
using Debugger.AddIn.TreeModel;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Services;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	/// <summary>
	/// Interaction logic for CallStackPadContent.xaml
	/// </summary>
	public partial class CallStackPadContent : UserControl
	{
		Process debuggedProcess;
		
		public CallStackPadContent()
		{
			InitializeComponent();
			
			view.ContextMenu = CreateMenu();
		}
		
		ContextMenu CreateMenu()
		{
			MenuItem argNamesItem = new MenuItem();
			argNamesItem.Header = ResourceService.GetString("MainWindow.Windows.Debug.CallStack.ShowArgumentNames");
			argNamesItem.IsChecked = DebuggingOptions.Instance.ShowArgumentNames;
			argNamesItem.Click += delegate {
				argNamesItem.IsChecked = DebuggingOptions.Instance.ShowArgumentNames = !DebuggingOptions.Instance.ShowArgumentNames;
				RefreshPad();
			};
			
			MenuItem argValuesItem = new MenuItem();
			argValuesItem.Header = ResourceService.GetString("MainWindow.Windows.Debug.CallStack.ShowArgumentValues");
			argValuesItem.IsChecked = DebuggingOptions.Instance.ShowArgumentValues;
			argValuesItem.Click += delegate {
				argValuesItem.IsChecked = DebuggingOptions.Instance.ShowArgumentValues = !DebuggingOptions.Instance.ShowArgumentValues;
				RefreshPad();
			};
			
			MenuItem extMethodsItem = new MenuItem();
			extMethodsItem.Header = ResourceService.GetString("MainWindow.Windows.Debug.CallStack.ShowExternalMethods");
			extMethodsItem.IsChecked = DebuggingOptions.Instance.ShowExternalMethods;
			extMethodsItem.Click += delegate {
				extMethodsItem.IsChecked = DebuggingOptions.Instance.ShowExternalMethods = !DebuggingOptions.Instance.ShowExternalMethods;
				RefreshPad();
			};
			
			return new ContextMenu() {
				Items = {
					argNamesItem,
					argValuesItem,
					extMethodsItem
				}
			};
		}
		
		public void SelectProcess(Process process)
		{
			if (debuggedProcess != null) {
				debuggedProcess.Paused -= debuggedProcess_Paused;
			}
			debuggedProcess = process;
			if (debuggedProcess != null) {
				debuggedProcess.Paused += debuggedProcess_Paused;
			}
			RefreshPad();
		}

		void debuggedProcess_Paused(object sender, ProcessEventArgs e)
		{
			RefreshPad();
		}
		
		void View_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (debuggedProcess == null)
				return;
			if (debuggedProcess.IsPaused) {
				CallStackItem item = view.SelectedItem as CallStackItem;
				
				if (item == null)
					return;
				
				if (item.Frame.HasSymbols) {
					if (debuggedProcess.SelectedThread != null) {
						debuggedProcess.SelectedThread.SelectedStackFrame = item.Frame;
						debuggedProcess.OnPaused(); // Force refresh of pads
					}
				} else {
					MessageService.ShowMessage("${res:MainWindow.Windows.Debug.CallStack.CannotSwitchWithoutSymbols}", "${res:MainWindow.Windows.Debug.CallStack.FunctionSwitch}");
				}
			} else {
				MessageService.ShowMessage("${res:MainWindow.Windows.Debug.CallStack.CannotSwitchWhileRunning}", "${res:MainWindow.Windows.Debug.CallStack.FunctionSwitch}");
			}
		}
		
		void View_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter) {
				View_MouseLeftButtonUp(sender, null);
				e.Handled = true;
			}
		}
		
		public void RefreshPad()
		{
			if (debuggedProcess == null || debuggedProcess.IsRunning || debuggedProcess.SelectedThread == null) {
				view.ItemsSource = null;
				return;
			}
			
			List<CallStackItem> items = null;
			StackFrame activeFrame = null;
			using(new PrintTimes("Callstack refresh")) {
				try {
					Utils.DoEvents(debuggedProcess);
					items = CreateItems().ToList();
					activeFrame = debuggedProcess.SelectedThread.SelectedStackFrame;
				} catch(AbortedBecauseDebuggeeResumedException) {
				} catch(System.Exception) {
					if (debuggedProcess == null || debuggedProcess.HasExited) {
						// Process unexpectedly exited
					} else {
						throw;
					}
				}
			}
			view.ItemsSource = items;
			view.SelectedItem = items != null ? items.FirstOrDefault(item => object.Equals(activeFrame, item.Frame)) : null;
		}
		
		IEnumerable<CallStackItem> CreateItems()
		{
			bool showExternalMethods = DebuggingOptions.Instance.ShowExternalMethods;
			bool lastItemIsExternalMethod = false;
			
			foreach (StackFrame frame in debuggedProcess.SelectedThread.GetCallstack(100)) {
				CallStackItem item;
				if (frame.HasSymbols || showExternalMethods) {
					// Show the method in the list
					item = new CallStackItem() { Name = GetFullName(frame), Language = "" };
					lastItemIsExternalMethod = false;
				} else {
					// Show [External methods] in the list
					if (lastItemIsExternalMethod) continue;
					item = new CallStackItem() { Name = ResourceService.GetString("MainWindow.Windows.Debug.CallStack.ExternalMethods"), Language = "" };
					lastItemIsExternalMethod = true;
				}
				item.Frame = frame;
				yield return item;
				
				Utils.DoEvents(debuggedProcess);
			}
		}
		
		string GetFullName(StackFrame frame)
		{
			bool showArgumentNames = DebuggingOptions.Instance.ShowArgumentNames;
			bool showArgumentValues = DebuggingOptions.Instance.ShowArgumentValues;
			
			StringBuilder name = new StringBuilder();
			name.Append(frame.MethodInfo.DeclaringType.Name);
			name.Append('.');
			name.Append(frame.MethodInfo.Name);
			if (showArgumentNames || showArgumentValues) {
				name.Append("(");
				for (int i = 0; i < frame.ArgumentCount; i++) {
					string parameterName = null;
					string argValue = null;
					if (showArgumentNames) {
						try {
							parameterName = frame.MethodInfo.GetParameters()[i].Name;
						} catch { }
						if (parameterName == "") parameterName = null;
					}
					if (showArgumentValues) {
						try {
							argValue = frame.GetArgumentValue(i).AsString;
							if (argValue != null && argValue.Length > 100)
								argValue = argValue.Substring(0, 100) + "...";
						} catch { }
					}
					if (parameterName != null && argValue != null) {
						name.Append(parameterName);
						name.Append("=");
						name.Append(argValue);
					}
					if (parameterName != null && argValue == null) {
						name.Append(parameterName);
					}
					if (parameterName == null && argValue != null) {
						name.Append(argValue);
					}
					if (parameterName == null && argValue == null) {
						name.Append(ResourceService.GetString("Global.NA"));
					}
					if (i < frame.ArgumentCount - 1) {
						name.Append(", ");
					}
				}
				name.Append(")");
			}
			return name.ToString();
		}
	}
	
	public class CallStackItem
	{
		public string Name { get; set; }
		public string Language { get; set; }
		public StackFrame Frame { get; set; }
		
		public Brush FontColor {
			get { return Frame == null || Frame.HasSymbols ? Brushes.Black : Brushes.Gray; }
		}
	}
	
	public class CallStackPad : DebuggerPad
	{
		CallStackPadContent callStackList;
		
		public override object Control {
			get {
				return callStackList;
			}
		}
		
		protected override void InitializeComponents()
		{
			callStackList = new CallStackPadContent();
		}
		
		protected override void SelectProcess(Process process)
		{
			callStackList.SelectProcess(process);
		}
		
		public override void RefreshPad()
		{
			callStackList.RefreshPad();
		}
	}
}
