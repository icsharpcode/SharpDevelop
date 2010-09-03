// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.MSL.CUDFunction;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Common;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.EDMObjects.MSL.CUDFunction
{
    public class CUDFunctionMapping
    {
        public SSDL.Function.Function SSDLFunction { get; set; }

        private EntityTypeCUDFunctionParametersMapping _parametersMapping;
        public EntityTypeCUDFunctionParametersMapping ParametersMapping
        {
            get
            {
                if (_parametersMapping == null)
                    _parametersMapping = new EntityTypeCUDFunctionParametersMapping();
                return _parametersMapping;
            }
        }

        private CUDFunctionResultsMapping _resultsMapping;
        public CUDFunctionResultsMapping ResultsMapping
        {
            get
            {
                if (_resultsMapping == null)
                    _resultsMapping = new CUDFunctionResultsMapping();
                return _resultsMapping;
            }
        }

        private EventedObservableCollection<CUDFunctionAssociationMapping> _associationMappings;
        public EventedObservableCollection<CUDFunctionAssociationMapping> AssociationMappings
        {
            get
            {
                if (_associationMappings == null)
                    _associationMappings = new EventedObservableCollection<CUDFunctionAssociationMapping>();
                return _associationMappings;
            }
        }
    }
}
