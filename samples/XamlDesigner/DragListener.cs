using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace ICSharpCode.XamlDesigner
{
    class DragListener
    {
        public DragListener(IInputElement target)
        {
            this.target = target;
            target.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(target_PreviewMouseLeftButtonDown);
            target.PreviewMouseMove += new MouseEventHandler(target_PreviewMouseMove);
        }

        public event MouseEventHandler DragStarted;
        
        IInputElement target;
        Window window;        
        Point startPoint;
        bool readyToRaise;

        Point GetMousePosition()
        {
            if (window == null) {
                window = Window.GetWindow(target as DependencyObject);
            }
            return Mouse.GetPosition(window);
        }

        bool IsMovementBigEnough()
        {
            Point currentPoint = GetMousePosition();
            return (Math.Abs(currentPoint.X - startPoint.X) >= SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(currentPoint.Y - startPoint.Y) >= SystemParameters.MinimumVerticalDragDistance);
        }

        void target_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            readyToRaise = true;
            startPoint = GetMousePosition();
        }

        void target_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (readyToRaise && IsMovementBigEnough()) {
                readyToRaise = false;
                if (DragStarted != null) {
                    DragStarted(target, e);
                }
            }
        }
    }
}
