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
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Diagnostics;
using System.Windows.Media;

namespace ICSharpCode.WpfDesign.Designer.Controls
{
	public delegate void DragHandler(DragListener drag);

	public class DragListener
	{
		static DragListener()
		{
			InputManager.Current.PostProcessInput += new ProcessInputEventHandler(PostProcessInput);
		}

		public Transform Transform { get; set; }
		
		public DragListener(IInputElement target)
		{
			Target = target;
			
			Target.PreviewMouseLeftButtonDown += Target_MouseDown;
			Target.PreviewMouseMove += Target_MouseMove;
			Target.PreviewMouseLeftButtonUp += Target_MouseUp;
		}
		
		public void ExternalStart()
		{
			Target_MouseDown(null, null);
		}
		
		public void ExternalMouseMove(MouseEventArgs e)
		{
			Target_MouseMove(null, e);
		}
		
		public void ExternalStop()
		{
			Target_MouseUp(null, null);
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
			if (MouseDown != null)
				MouseDown(this);
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

		public event DragHandler MouseDown;
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
			get { 
				if (Transform != null) {
					var matrix = Transform.Value;
					matrix.Invert();
					return matrix.Transform(CurrentPoint - StartPoint);
				}
				return CurrentPoint - StartPoint;
			}
		}
	}
}
