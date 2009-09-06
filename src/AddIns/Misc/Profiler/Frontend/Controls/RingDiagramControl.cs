// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ICSharpCode.Profiler.Controls
{
	public class RingDiagramControl : System.Windows.Controls.Grid
	{
		Stack<CallTreeNodeViewModel> hierarchyStack;
		SingleTask task;
		
		public static readonly DependencyProperty SelectedRootProperty =
			DependencyProperty.Register("SelectedRoot", typeof(CallTreeNodeViewModel), typeof(RingDiagramControl));
		
		public CallTreeNodeViewModel SelectedRoot
		{
			get { return (CallTreeNodeViewModel)GetValue(SelectedRootProperty); }
			set { SetValue(SelectedRootProperty, value); }
		}
		
		public static readonly DependencyProperty TranslationProperty = DependencyProperty.Register(
			"Translation", typeof(ControlsTranslation), typeof(RingDiagramControl));
		
		public ControlsTranslation Translation {
			set { SetValue(TranslationProperty, value); }
			get { return (ControlsTranslation)GetValue(TranslationProperty); }
		}
		
		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);
			if (e.Property == SelectedRootProperty)
				Update(this.SelectedRoot);
		}
		
		public RingDiagramControl()
		{
			this.hierarchyStack = new Stack<CallTreeNodeViewModel>();
			this.task = new SingleTask(this.Dispatcher);
			this.Translation = new ControlsTranslation();
		}

		void Update(CallTreeNodeViewModel item)
		{
			Debug.WriteLine("RingDiagram.Update: new root = " + item);
			
			this.task.Cancel();
			
			Debug.WriteLine("hierarchyStack count: " + this.hierarchyStack.Count);
			
			while (this.hierarchyStack.Count > 0 && !hierarchyStack.Peek().IsAncestorOf(item)) {
				this.hierarchyStack.Pop();
			}

			Debug.Assert(hierarchyStack.Count == 0 || hierarchyStack.Peek().IsAncestorOf(item));

			this.Children.Clear();
			
			if (item == null)
				return;
			
			List<Shape> newItems = new List<Shape>();
			
			Ellipse ell = new Ellipse();
			ell.Width = 40;
			ell.Height = 40;
			ell.VerticalAlignment = VerticalAlignment.Center;
			ell.HorizontalAlignment = HorizontalAlignment.Center;
			ell.Fill = Brushes.Gray;
			ell.Stroke = Brushes.Black;
			ell.ToolTip = item.CreateToolTip(Translation);
			ell.Tag = item;

			ell.MouseLeftButtonDown += (sender, e) =>
			{
				if (this.hierarchyStack.Count > 1 && this.hierarchyStack.Peek().Level > 1) {
					var oldItem = this.hierarchyStack.Pop();
					this.SelectedRoot = this.hierarchyStack.Peek();
					this.SelectedRoot.IsSelected = true;
					this.SelectedRoot.IsExpanded = true;
					oldItem.IsSelected = false;
				}
			};
			
			if (hierarchyStack.Count == 0 || hierarchyStack.Peek() != item)
				this.hierarchyStack.Push(item);
			
			List<PiePieceDescriptor> pieces = new List<PiePieceDescriptor>();
			
			this.task.Execute(
				() => {
					if (item.CpuCyclesSpent > 0)
						CreateTree(pieces, item, 0, item.CpuCyclesSpent, 0);
				},
				() => {
					this.Children.Add(ell);
					this.Children.AddRange(pieces.Select(p => CreatePiePiece(p.Radius, p.WedgeAngle, p.RotationAngle, p.Level, p.Node)));
					item.BringIntoView();
				},
				delegate { }
			);
		}
		
		private void CreateTree(List<PiePieceDescriptor> list, CallTreeNodeViewModel parent, double rotation, long totalCycles, int level)
		{
			if (level == 10 || Task.Current.IsCancelled)
				return;

			level++;

			int i = 0;

			foreach (CallTreeNodeViewModel child in parent.Children)
			{
				if (Task.Current.IsCancelled)
					return;
				
				double childWedgeAngle;
				childWedgeAngle = child.CpuCyclesSpent * 360.0 / totalCycles;
				if (childWedgeAngle >= 360)
					childWedgeAngle = 359.9999;
				if (childWedgeAngle > 0.5) {
					PiePieceDescriptor piePiece = new PiePieceDescriptor { Radius = 20, WedgeAngle = childWedgeAngle, RotationAngle = rotation, Level = level, Node = child };
					list.Add(piePiece);
					
					// create pie pieces for children
					CreateTree(list, child, rotation, totalCycles, level);
					
					rotation += childWedgeAngle;

					i++;
				}
			}
		}

		private PiePiece CreatePiePiece(int rad, double wedgeAngle, double rotationAngle, int level, CallTreeNodeViewModel node)
		{
			// prevent exception when ProfilerHook screws up and children are larger than their parent (e.g. when process is killed)
			if (rotationAngle > 360)
				rotationAngle %= 360;
			
			PiePiece p = new PiePiece();

			p.Radius = 20 + level * rad;
			p.InnerRadius = level * rad;
			p.WedgeAngle = wedgeAngle;
			p.RotationAngle = rotationAngle;
			p.Stroke = Brushes.Black;
			p.ToolTip = node.CreateToolTip(Translation);
			p.VerticalAlignment = VerticalAlignment.Center;
			p.HorizontalAlignment = HorizontalAlignment.Center;
			p.Tag = node;

			p.MouseLeftButtonDown += new MouseButtonEventHandler(
				delegate(object sender, MouseButtonEventArgs e)	{					
					node.IsExpanded = true;
					node.IsSelected = true; // expand the path to the node so that the treeview can select it
					var oldNode = this.SelectedRoot;
					this.SelectedRoot = node;
					oldNode.IsSelected = false;
				}
			);
			
			HSVColor hsv = new HSVColor {
				Hue = (float)rotationAngle,
				Saturation = 0.5f,
				Value = 0.6f - level / 50f
			};
			
			SolidColorBrush brush = new SolidColorBrush();
			p.Fill = brush;
			
			Color normalColor = hsv.ToColor();
			hsv.Value = 0.8f;
			Color highlightColor = hsv.ToColor();
			brush.Color = normalColor;
			
			p.IsMouseDirectlyOverChanged += (sender, e) => {
				if (p.IsMouseDirectlyOver) {
					brush.BeginAnimation(SolidColorBrush.ColorProperty,
					                     new ColorAnimation(highlightColor, new Duration(TimeSpan.FromSeconds(0.5)),
					                                        FillBehavior.HoldEnd));
				} else {
					brush.BeginAnimation(SolidColorBrush.ColorProperty,
					                     new ColorAnimation(normalColor, new Duration(TimeSpan.FromSeconds(0.5)),
					                                        FillBehavior.Stop));
				}
			};

			return p;
		}
	}
	
	class PiePieceDescriptor
	{
		public int Radius { get; set; }
		public double WedgeAngle { get; set; }
		public double RotationAngle { get; set; }
		public int Level { get; set; }
		public CallTreeNodeViewModel Node { get; set; }
	}
}
