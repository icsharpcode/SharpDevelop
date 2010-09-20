// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
