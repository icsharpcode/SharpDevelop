using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace TreeMaps.Controls
{
  public class ValueDataTemplate : HierarchicalDataTemplate
  {
    #region fields

    private BindingBase _valueBinding;

    #endregion

    #region ctors

    public ValueDataTemplate()
    {
    }

    public ValueDataTemplate(object dataType)
      : base(dataType)
    {
    }

    #endregion

    #region properties

    public BindingBase AreaValue
    {
      get { return _valueBinding; }
      set { _valueBinding = value; }
    }

    #endregion
  }
}
