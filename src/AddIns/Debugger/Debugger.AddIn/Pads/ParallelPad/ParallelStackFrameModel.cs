// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Widgets;

namespace Debugger.AddIn.Pads.ParallelPad
{
	public class ParallelStackFrameModel : ViewModelBase
	{
		FontWeight fontWeight;
		
		public FontWeight FontWeight {
			get { return fontWeight; }
			set {
				fontWeight = value;
				RaisePropertyChanged(() => FontWeight);
			}
		}
		
		Brush foreground;
		
		public Brush Foreground {
			get { return foreground; }
			set {
				foreground = value;
				RaisePropertyChanged(() => Foreground);
			}
		}
		
		ImageSource image;
		
		public ImageSource Image {
			get { return image; }
			set {
				image = value;
				RaisePropertyChanged(() => Image);
			}
		}
		
		string methodName;
		
		public string MethodName {
			get { return methodName; }
			set {
				methodName = value;
				RaisePropertyChanged(() => MethodName);
			}
		}
		
		bool isRunningStackFrame;
		
		public bool IsRunningStackFrame {
			get { return isRunningStackFrame; }
			set {
				isRunningStackFrame = value;
				RaisePropertyChanged(() => IsRunningStackFrame);
			}
		}
	}
	
	public class ThreadModel : ViewModelBase
	{
		Thread thread;
		
		public ThreadModel(Thread thread)
		{
			if (thread == null)
				throw new ArgumentNullException("thread");
			this.thread = thread;
			thread.NameChanged += delegate { RaisePropertyChanged(() => Name); };
		}
		
		public Thread Thread {
			get { return thread; }
		}
		
		public uint ID {
			get { return thread.ID; }
		}
		
		public string Name {
			get { return thread.Name; }
		}
		
		public string Priority {
			get {
				switch (thread.Priority) {
					case System.Threading.ThreadPriority.Highest:
						return ResourceService.GetString("MainWindow.Windows.Debug.Threads.Priority.Highest");
					case System.Threading.ThreadPriority.AboveNormal:
						return ResourceService.GetString("MainWindow.Windows.Debug.Threads.Priority.AboveNormal");
					case System.Threading.ThreadPriority.Normal:
						return ResourceService.GetString("MainWindow.Windows.Debug.Threads.Priority.Normal");
					case System.Threading.ThreadPriority.BelowNormal:
						return ResourceService.GetString("MainWindow.Windows.Debug.Threads.Priority.BelowNormal");
					case System.Threading.ThreadPriority.Lowest:
						return ResourceService.GetString("MainWindow.Windows.Debug.Threads.Priority.Lowest");
					default:
						return thread.Priority.ToString();
				}
			}
		}
		
		public string Location {
			get {
				if (thread.Process.IsPaused && thread.MostRecentStackFrame != null)
					return thread.MostRecentStackFrame.MethodInfo.Name;
				return ResourceService.GetString("Global.NA");
			}
		}
		
		public string Frozen {
			get {
				return ResourceService.GetString(thread.Suspended ? "Global.Yes" : "Global.No");
			}
		}
	}
	
	public class ModuleModel : ViewModelBase
	{
		Module module;
		
		public ModuleModel(Module module)
		{
			if (module == null)
				throw new ArgumentNullException("module");
			this.module = module;
			this.module.SymbolsUpdated += delegate {
				RaisePropertyChanged(() => Name);
				RaisePropertyChanged(() => Address);
				RaisePropertyChanged(() => Path);
				RaisePropertyChanged(() => Order);
				RaisePropertyChanged(() => Symbols);
			};
		}
		
		public Module Module {
			get { return module; }
		}
		
		public string Name {
			get { return module.Name; }
		}
		
		public string Address {
			get { return string.Format("{0:X8}", module.BaseAdress); }
		}
		
		public string Path {
			get {
				if (module.IsDynamic)
					return StringParser.Parse("${res:MainWindow.Windows.Debug.Modules.DynamicModule}");
				if (module.IsInMemory)
					return StringParser.Parse("${res:MainWindow.Windows.Debug.Modules.InMemoryModule}");
				
				return module.FullPath;
			}
		}
		
		public string Order {
			get {
				return module.OrderOfLoading.ToString();
			}
		}
		
		public string Symbols {
			get {
				if (module.HasSymbols)
					return StringParser.Parse("${res:MainWindow.Windows.Debug.Modules.HasSymbols}");
				return StringParser.Parse("${res:MainWindow.Windows.Debug.Modules.HasNoSymbols}");
			}
		}
	}
}
