// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Debugger;
using Debugger.AddIn.TreeModel;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui.Pads;
using ICSharpCode.SharpDevelop.Services;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	/// <summary>
	/// Interaction logic for CallStackPadContent.xaml
	/// </summary>
	public partial class CallStackPadContent : UserControl
	{
		CallStackPad callStackPad;
		Process debuggedProcess;
		
		public CallStackPadContent(CallStackPad pad)
		{
			this.callStackPad = pad;
			InitializeComponent();
			
			view.ContextMenu = CreateMenu();
			
			((GridView)view.View).Columns[0].Width = DebuggingOptions.Instance.ShowModuleNames ? 100d : 0d;
			((GridView)view.View).Columns[2].Width = DebuggingOptions.Instance.ShowLineNumbers ? 50d : 0d;
		}
		
		ContextMenu CreateMenu()
		{
			MenuItem extMethodsItem = new MenuItem();
			extMethodsItem.Header = ResourceService.GetString("MainWindow.Windows.Debug.CallStack.ShowExternalMethods");
			extMethodsItem.IsChecked = DebuggingOptions.Instance.ShowExternalMethods;
			extMethodsItem.Click += delegate {
				extMethodsItem.IsChecked = DebuggingOptions.Instance.ShowExternalMethods = !DebuggingOptions.Instance.ShowExternalMethods;
				callStackPad.InvalidatePad();
			};
			
			MenuItem moduleItem = new MenuItem();
			moduleItem.Header = ResourceService.GetString("MainWindow.Windows.Debug.CallStack.ShowModuleNames");
			moduleItem.IsChecked = DebuggingOptions.Instance.ShowModuleNames;
			moduleItem.Click += delegate {
				moduleItem.IsChecked = DebuggingOptions.Instance.ShowModuleNames = !DebuggingOptions.Instance.ShowModuleNames;
				((GridView)view.View).Columns[0].Width = DebuggingOptions.Instance.ShowModuleNames ? 100d : 0d;
				callStackPad.InvalidatePad();
			};
			
			MenuItem argNamesItem = new MenuItem();
			argNamesItem.Header = ResourceService.GetString("MainWindow.Windows.Debug.CallStack.ShowArgumentNames");
			argNamesItem.IsChecked = DebuggingOptions.Instance.ShowArgumentNames;
			argNamesItem.Click += delegate {
				argNamesItem.IsChecked = DebuggingOptions.Instance.ShowArgumentNames = !DebuggingOptions.Instance.ShowArgumentNames;
				callStackPad.InvalidatePad();
			};
			
			MenuItem argValuesItem = new MenuItem();
			argValuesItem.Header = ResourceService.GetString("MainWindow.Windows.Debug.CallStack.ShowArgumentValues");
			argValuesItem.IsChecked = DebuggingOptions.Instance.ShowArgumentValues;
			argValuesItem.Click += delegate {
				argValuesItem.IsChecked = DebuggingOptions.Instance.ShowArgumentValues = !DebuggingOptions.Instance.ShowArgumentValues;
				callStackPad.InvalidatePad();
			};
			
			MenuItem lineItem = new MenuItem();
			lineItem.Header = ResourceService.GetString("MainWindow.Windows.Debug.CallStack.ShowLineNumber");
			lineItem.IsChecked = DebuggingOptions.Instance.ShowLineNumbers;
			lineItem.Click += delegate {
				lineItem.IsChecked = DebuggingOptions.Instance.ShowLineNumbers = !DebuggingOptions.Instance.ShowLineNumbers;
				((GridView)view.View).Columns[2].Width = DebuggingOptions.Instance.ShowLineNumbers ? 50d : 0d;
				callStackPad.InvalidatePad();
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
		
		public void SelectProcess(Process process)
		{
			if (debuggedProcess != null) {
				debuggedProcess.Paused -= debuggedProcess_Paused;
			}
			debuggedProcess = process;
			if (debuggedProcess != null) {
				debuggedProcess.Paused += debuggedProcess_Paused;
			}
			callStackPad.InvalidatePad();
		}

		void debuggedProcess_Paused(object sender, ProcessEventArgs e)
		{
			callStackPad.InvalidatePad();
		}
		
		void View_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (debuggedProcess == null)
				return;
			if (debuggedProcess.IsPaused) {
				CallStackItem item = view.SelectedItem as CallStackItem;
				
				if (item == null)
					return;

				if (item.Frame != null && debuggedProcess.SelectedThread != null) {
					// check for options - if these options are enabled, selecting the frame should not continue
					if (!item.Frame.HasSymbols && !debuggedProcess.Options.DecompileCodeWithoutSymbols) {
						MessageService.ShowMessage("${res:MainWindow.Windows.Debug.CallStack.CannotSwitchWithoutSymbolsOrDecompiledCodeOptions}",
						                           "${res:MainWindow.Windows.Debug.CallStack.FunctionSwitch}");
						return;
					}
					debuggedProcess.SelectedThread.SelectedStackFrame = item.Frame;
					debuggedProcess.PauseSession.PausedReason = PausedReason.CurrentFunctionChanged;
					debuggedProcess.OnPaused(); // Force refresh of pads - artificial pause
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
		
		internal void RefreshPad()
		{
			if (debuggedProcess == null || debuggedProcess.IsRunning || debuggedProcess.SelectedThread == null) {
				view.ItemsSource = null;
				return;
			}
			
			var items = new ObservableCollection<CallStackItem>();
			using(new PrintTimes("Callstack refresh")) {
				bool showExternalMethods = DebuggingOptions.Instance.ShowExternalMethods;
				bool previousItemIsExternalMethod = false;
				
				debuggedProcess.EnqueueForEach(
					Dispatcher,
					debuggedProcess.SelectedThread.GetCallstack(100),
					f => items.AddIfNotNull(CreateItem(f, showExternalMethods, ref previousItemIsExternalMethod))
				);
			}
			view.ItemsSource = items;
		}
		
		CallStackItem CreateItem(StackFrame frame, bool showExternalMethods, ref bool previousItemIsExternalMethod)
		{
			CallStackItem item;
			
			// line number
			string lineNumber = string.Empty;
			if (DebuggingOptions.Instance.ShowLineNumbers) {
				if (frame.NextStatement != null)
					lineNumber = frame.NextStatement.StartLine.ToString();
			}
			
			// show modules names
			string moduleName = string.Empty;
			if (DebuggingOptions.Instance.ShowModuleNames) {
				moduleName = frame.MethodInfo.DebugModule.ToString();
			}
			
			if (frame.HasSymbols || showExternalMethods) {
				// Show the method in the list
				
				item = new CallStackItem() {
					Name = GetFullName(frame), Language = "", Line = lineNumber, ModuleName = moduleName
				};
				previousItemIsExternalMethod = false;
				item.Frame = frame;
			} else {
				// Show [External methods] in the list
				if (previousItemIsExternalMethod) return null;
				item = new CallStackItem() {
					Name = ResourceService.GetString("MainWindow.Windows.Debug.CallStack.ExternalMethods"),
					Language = ""
				};
				previousItemIsExternalMethod = true;
			}
			
			return item;
		}
		
		internal static string GetFullName(StackFrame frame)
		{
			bool showArgumentNames = DebuggingOptions.Instance.ShowArgumentNames;
			bool showArgumentValues = DebuggingOptions.Instance.ShowArgumentValues;
			bool showLineNumber = DebuggingOptions.Instance.ShowLineNumbers;
			bool showModuleNames = DebuggingOptions.Instance.ShowModuleNames;
			
			StringBuilder name = new StringBuilder();
			name.Append(frame.MethodInfo.DeclaringType.FullName);
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
							argValue = frame.GetArgumentValue(i).AsString(100);
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
		public string Line { get; set; }
		public string ModuleName { get; set; }
		
		public Brush FontColor {
			get { return Frame == null || Frame.HasSymbols ? Brushes.Black : Brushes.Gray; }
		}
	}
	
	public class CallStackPad : DebuggerPad
	{
		CallStackPadContent callStackList;
		
		static CallStackPad instance;
		
		public static CallStackPad Instance {
			get { return instance; }
		}
		
		public CallStackPad()
		{
			instance = this;
		}
		
		public override object Control {
			get {
				return callStackList;
			}
		}
		
		protected override void InitializeComponents()
		{
			callStackList = new CallStackPadContent(this);
		}
		
		protected override void SelectProcess(Process process)
		{
			callStackList.SelectProcess(process);
		}
		
		protected override void RefreshPad()
		{
			callStackList.RefreshPad();
		}
	}
}
