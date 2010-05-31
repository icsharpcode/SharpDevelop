//Copyright (c) 2007-2010, Adolfo Marinucci
//All rights reserved.

//Redistribution and use in source and binary forms, with or without modification, 
//are permitted provided that the following conditions are met:
//
//* Redistributions of source code must retain the above copyright notice, 
//  this list of conditions and the following disclaimer.
//* Redistributions in binary form must reproduce the above copyright notice, 
//  this list of conditions and the following disclaimer in the documentation 
//  and/or other materials provided with the distribution.
//* Neither the name of Adolfo Marinucci nor the names of its contributors may 
//  be used to endorse or promote products derived from this software without 
//  specific prior written permission.
//
//THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
//AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
//WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. 
//IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, 
//INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, 
//PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
//HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, 
//OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, 
//EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE. 

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Diagnostics;
using System.ComponentModel;

namespace AvalonDock
{
    /// <summary>
    /// Provides drag-drop functionalities for dockable panes
    /// </summary>
    internal sealed class DragPaneServices
    {
        List<IDropSurface> Surfaces = new List<IDropSurface>();
        List<IDropSurface> SurfacesWithDragOver = new List<IDropSurface>();

        DockingManager _owner;

        public DockingManager DockManager
        {
            get { return _owner; }
        }

        public DragPaneServices(DockingManager owner)
        {
            if (DesignerProperties.GetIsInDesignMode(owner))
                throw new NotSupportedException("DragPaneServices not valid in design mode");

            if (owner == null)
                throw new ArgumentNullException("owner");

            _owner = owner;
        }

        public void Register(IDropSurface surface)
        {
            if (!Surfaces.Contains(surface))
                Surfaces.Add(surface);
        }

        public void Unregister(IDropSurface surface)
        {
            Surfaces.Remove(surface);
        }

        Point Offset;

        public bool IsDragging { get; private set; }

        public void StartDrag(FloatingWindow wnd, Point point, Point offset)
        {
            Debug.Assert(!IsDragging);

            IsDragging = true;

            Offset = offset;

            _wnd = wnd;

            if (Offset.X >= _wnd.Width)
                Offset.X = _wnd.Width / 2;


            _wnd.Left = point.X - Offset.X;
            _wnd.Top = point.Y - Offset.Y;
            _wnd.Show();

            int surfaceCount = 0;
            restart:
            surfaceCount = Surfaces.Count;
            foreach (IDropSurface surface in Surfaces)
            {
                if (surface.SurfaceRectangle.Contains(point))
                {
                    SurfacesWithDragOver.Add(surface);
                    surface.OnDragEnter(point);
                    Debug.WriteLine("Enter " + surface.ToString());
                    if (surfaceCount != Surfaces.Count)
                    { 
                        //Surfaces list has been changed restart cycle
                        SurfacesWithDragOver.Clear();
                        goto restart;
                    }
                }
            }
            
        }

        public void MoveDrag(Point point)
        {
            if (_wnd == null)
                return;

            _wnd.Left = point.X - Offset.X;
            _wnd.Top = point.Y - Offset.Y;

            List<IDropSurface> enteringSurfaces = new List<IDropSurface>();
            foreach (IDropSurface surface in Surfaces)
            {
                if (surface.SurfaceRectangle.Contains(point))
                {
                    if (!SurfacesWithDragOver.Contains(surface))
                        enteringSurfaces.Add(surface);
                    else
                        surface.OnDragOver(point);
                }
                else if (SurfacesWithDragOver.Contains(surface))
                {
                    SurfacesWithDragOver.Remove(surface);
                    surface.OnDragLeave(point);
                }
            }

            foreach (IDropSurface surface in enteringSurfaces)
            {
                SurfacesWithDragOver.Add(surface);
                surface.OnDragEnter(point);
            }
        }

        public void EndDrag(Point point)
        {
            IDropSurface dropSufrace = null;
            foreach (IDropSurface surface in Surfaces)
            {
                if (surface.SurfaceRectangle.Contains(point))
                {
                    if (surface.OnDrop(point))
                    {
                        dropSufrace = surface;
                        break;
                    }
                }
            }

            foreach (IDropSurface surface in SurfacesWithDragOver)
            {
                if (surface != dropSufrace)
                {
                    surface.OnDragLeave(point);
                }
            }

            SurfacesWithDragOver.Clear();

            _wnd.OnEndDrag();//notify floating window that drag operation is coming to end

            if (dropSufrace != null)
                _wnd.Close();
            else
            {
                _wnd.Visibility = Visibility.Visible;
                _wnd.Activate();
            }

            _wnd = null;

            IsDragging = false;

        }

        FloatingWindow _wnd;
        public FloatingWindow FloatingWindow
        {
            get { return _wnd; }
        }

    }
}
