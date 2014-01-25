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

using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;

namespace ICSharpCode.WpfDesign.Designer.Services
{	
	class DesignerKeyBindings : IKeyBindingService
    {
        private readonly DesignSurface _surface;
        private Collection<KeyBinding> _bindings;

        public DesignerKeyBindings(DesignSurface surface)
        {
            Debug.Assert(surface != null);
            this._surface = surface;
            _bindings = new Collection<KeyBinding>();
        }

        public void RegisterBinding(KeyBinding binding)
        {
            if(binding!=null) {
                _surface.InputBindings.Add(binding);
                _bindings.Add(binding);
            }
        }

        public void DeregisterBinding(KeyBinding binding)
        {
            if(_bindings.Contains(binding)) {
                _surface.InputBindings.Remove(binding);
                _bindings.Remove(binding);
            }
        }

        public KeyBinding GetBinding(KeyGesture gesture)
        {
            return _bindings.FirstOrDefault(binding => binding.Key == gesture.Key && binding.Modifiers == gesture.Modifiers);
        }

        public object Owner{
            get { return _surface; }
        }

    }
}
