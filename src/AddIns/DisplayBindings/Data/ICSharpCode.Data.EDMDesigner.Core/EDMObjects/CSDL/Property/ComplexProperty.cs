// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;
using System.Collections;
using System.Linq;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Type;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Property
{
    public class ComplexProperty : PropertyBase
    {
        #region Fields

        private ComplexType _complexType;

        #endregion

        #region Constructors

        public ComplexProperty(ComplexType complexType)
        {
            ComplexType = complexType;
        }

        internal ComplexProperty(string complexTypeName)
        {
            ComplexTypeName = complexTypeName;
        }

        #endregion

        #region Properties

        protected override Func<TypeBase, IList> GetPropertyCollection
        {
            get { return entityType => entityType.ComplexProperties; }
        }

        public ComplexType ComplexType
        {
            get
            {
                if (_complexType == null)
                    _complexType = EntityType.Container.ComplexTypes.FirstOrDefault(ct => ct.Name == ComplexTypeName);
                return _complexType;
            }
            private set { _complexType = value; }
        }

        internal string ComplexTypeName { get; private set; }

        #endregion

        #region Methods

        protected override PropertyBase Create()
        {
            return new ComplexProperty(ComplexType);
        }

        #endregion
    }
}
