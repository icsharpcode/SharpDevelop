// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System.Collections.Generic;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Common;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Property;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.EDMObjects.MSL.CUDFunction
{
    public class EntityTypeCUDFunctionParametersMapping : CUDFunctionParametersMapping
    {
        #region Fields

        private EventedObservableCollection<KeyValuePair<ComplexProperty, EntityTypeCUDFunctionParametersMapping>> _complexPropertiesMapping;

        #endregion

        #region Properties

        public EventedObservableCollection<KeyValuePair<ComplexProperty, EntityTypeCUDFunctionParametersMapping>> ComplexPropertiesMapping
        {
            get
            {
                if (_complexPropertiesMapping == null)
                    _complexPropertiesMapping = new EventedObservableCollection<KeyValuePair<ComplexProperty, EntityTypeCUDFunctionParametersMapping>>();
                return _complexPropertiesMapping;
            }
        }

        #endregion
    }
}
