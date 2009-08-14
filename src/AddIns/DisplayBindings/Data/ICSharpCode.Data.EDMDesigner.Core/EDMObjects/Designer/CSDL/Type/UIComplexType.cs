#region Usings

using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Type;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.Common;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.CSDL.Type
{
    public class UIComplexType : UITypeBase<ComplexType>
    {
        internal UIComplexType(CSDLView view, ComplexType complexType)
            : base(view, complexType)
        {
        }
    }
}
