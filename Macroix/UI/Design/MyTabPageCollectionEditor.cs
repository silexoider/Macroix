using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Design;

namespace Macroix.UI.Design
{
  using UI.Controls;
  internal class MyTabPageCollectionEditor : CollectionEditor
  {
    protected override CollectionForm CreateCollectionForm()
    {
      var baseForm = base.CreateCollectionForm();
      baseForm.Text = "MyTabPage Collection Editor";
      return baseForm;
    }

    public MyTabPageCollectionEditor(Type type)
        : base(type)
    { }

    protected override Type CreateCollectionItemType()
    {
      return typeof(MyTabPage);
    }

    protected override Type[] CreateNewItemTypes()
    {
      return new[] { typeof(MyTabPage) };
    }
  }
}
