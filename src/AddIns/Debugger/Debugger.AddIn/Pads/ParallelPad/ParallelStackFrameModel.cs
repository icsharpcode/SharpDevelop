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
}
