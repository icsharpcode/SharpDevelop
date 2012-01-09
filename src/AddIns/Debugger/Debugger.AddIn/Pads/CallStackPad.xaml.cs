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
				RefreshPad();
			};
			
			MenuItem moduleItem = new MenuItem();
			moduleItem.Header = ResourceService.GetString("MainWindow.Windows.Debug.CallStack.ShowModuleNames");
			moduleItem.IsChecked = DebuggingOptions.Instance.ShowModuleNames;
			moduleItem.Click += delegate {
				moduleItem.IsChecked = DebuggingOptions.Instance.ShowModuleNames = !DebuggingOptions.Instance.ShowModuleNames;
				((GridView)view.View).Columns[0].Width = DebuggingOptions.Instance.ShowModuleNames ? 100d : 0d;
				RefreshPad();
			};
			
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
			
			MenuItem lineItem = new MenuItem();
			lineItem.Header = ResourceService.GetString("MainWindow.Windows.Debug.CallStack.ShowLineNumber");
			lineItem.IsChecked = DebuggingOptions.Instance.ShowLineNumbers;
			lineItem.Click += delegate {
				lineItem.IsChecked = DebuggingOptions.Instance.ShowLineNumbers = !DebuggingOptions.Instance.ShowLineNumbers;
				((GridView)view.View).Columns[2].Width = DebuggingOptions.Instance.ShowLineNumbers ? 50d : 0d;
				RefreshPad();
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

				if (item.Frame != null && debuggedProcess.SelectedThread != null) {
					// check for options - if these options are enabled, selecting the frame should not continue
					if (!item.Frame.HasSymbols && (debuggedProcess.Options.EnableJustMyCode || debuggedProcess.Options.StepOverNoSymbols)) {
						MessageService.ShowMessage("${res:MainWindow.Windows.Debug.CallStack.CannotSwitchWithoutSymbolsOrDecompiledCodeOptions}",
						                           "${res:MainWindow.Windows.Debug.CallStack.FunctionSwitch}");
						return;
					}
					debuggedProcess.SelectedThread.SelectedStackFrame = item.Frame;
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
		
		public void RefreshPad()
		{
			if (debuggedProcess == null || debuggedProcess.IsRunning || debuggedProcess.SelectedThread == null) {
				view.ItemsSource = null;
				return;
			}
			
			List<CallStackItem> items = null;
			using(new PrintTimes("Callstack refresh")) {
				try {
					Utils.DoEvents(debuggedProcess);
					items = CreateItems().ToList();
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
		}
		
		IEnumerable<CallStackItem> CreateItems()
		{
			bool showExternalMethods = DebuggingOptions.Instance.ShowExternalMethods;
			bool lastItemIsExternalMethod = false;
			
			foreach (StackFrame frame in debuggedProcess.SelectedThread.GetCallstack(100)) {
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
					lastItemIsExternalMethod = false;
					item.Frame = frame;
				} else {
					// Show [External methods] in the list
					if (lastItemIsExternalMethod) continue;
					item = new CallStackItem() {
						Name = ResourceService.GetString("MainWindow.Windows.Debug.CallStack.ExternalMethods"),
						Language = ""
					};
					lastItemIsExternalMethod = true;
				}
				
				yield return item;
				
				Utils.DoEvents(debuggedProcess);
			}
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
