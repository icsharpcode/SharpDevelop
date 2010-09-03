// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Diagnostics;

namespace ICSharpCode.WpfDesign.Designer.Controls
{
	public delegate void DragHandler(DragListener drag);

	public class DragListener
	{
		static DragListener()
		{
			InputManager.Current.PostProcessInput += new ProcessInputEventHandler(PostProcessInput);
		}

		public DragListener(IInputElement target)
		{
			Target = target;
			
			Target.PreviewMouseLeftButtonDown += Target_MouseDown;
			Target.PreviewMouseMove += Target_MouseMove;
			Target.PreviewMouseLeftButtonUp += Target_MouseUp;
		}

		static DragListener CurrentListener;

		static void PostProcessInput(object sender, ProcessInputEventArgs e)
		{
			if (CurrentListener != null) {
				var a = e.StagingItem.Input as KeyEventArgs;
				if (a != null && a.Key == Key.Escape) {
					Mouse.Capture(null);
					CurrentListener.IsDown = false;
					CurrentListener.IsCanceled = true;
					CurrentListener.Complete();
				}
			}
		}

		void Target_MouseDown(object sender, MouseButtonEventArgs e)
		{
			StartPoint = Mouse.GetPosition(null);
			CurrentPoint = StartPoint;
			DeltaDelta = new Vector();
			IsDown = true;
			IsCanceled = false;
		}

		void Target_MouseMove(object sender, MouseEventArgs e)
		{
			if (IsDown) {
				DeltaDelta = e.GetPosition(null) - CurrentPoint;
				CurrentPoint += DeltaDelta;

				if (!IsActive) {
					if (Math.Abs(Delta.X) >= SystemParameters.MinimumHorizontalDragDistance ||
					    Math.Abs(Delta.Y) >= SystemParameters.MinimumVerticalDragDistance) {
						IsActive = true;
						CurrentListener = this;

						if (Started != null) {
							Started(this);
						}
					}
				}

				if (IsActive && Changed != null) {
					Changed(this);
				}
			}
		}

		void Target_MouseUp(object sender, MouseButtonEventArgs e)
		{
			IsDown = false;
			if (IsActive) {
				Complete();
			}
		}

		void Complete()
		{
			IsActive = false;
			CurrentListener = null;

			if (Completed != null) {
				Completed(this);
			}
		}

		public event DragHandler Started;
		public event DragHandler Changed;
		public event DragHandler Completed;

		public IInputElement Target { get; private set; }
		public Point StartPoint { get; private set; }
		public Point CurrentPoint { get; private set; }
		public Vector DeltaDelta { get; private set; }
		public bool IsActive { get; private set; }
		public bool IsDown { get; private set; }
		public bool IsCanceled { get; private set; }
		
		public Vector Delta {
			get { return CurrentPoint - StartPoint; }
		}
	}
}
