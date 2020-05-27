using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Macroix.UI.Controls;
using System.Drawing;

namespace Macroix.UI.Design
{
	internal class MyTabControlDesigner : ParentControlDesigner
	{
		DesignerVerb _addVerb;
		DesignerVerb _removeVerb;
		DesignerVerbCollection _verbs;
		IDesignerHost _designerHost;
		ISelectionService _selectionService;

		MyTabControl MyControl
		{
			get { return (MyTabControl)Control; }
		}

		public override SelectionRules SelectionRules
		{
			get { return Control.Dock == DockStyle.Fill ? SelectionRules.Visible : base.SelectionRules; }
		}
		public override DesignerVerbCollection Verbs
		{
			get
			{
				UpdateVerbs();
				return _verbs;
			}
		}
		public IDesignerHost DesignerHost
		{
			get { return _designerHost ?? (_designerHost = (IDesignerHost)GetService(typeof(IDesignerHost))); }
		}
		public ISelectionService SelectionService
		{
			get { return _selectionService ?? (_selectionService = (ISelectionService)GetService(typeof(ISelectionService))); }
		}
		public MyTabControlDesigner()
		{
			_addVerb = new DesignerVerb("Add page", OnAddPage);
			_removeVerb = new DesignerVerb("Remove page", OnRemovePage);
			_verbs = new DesignerVerbCollection(new DesignerVerb[] { _addVerb, _removeVerb });
		}
		protected override bool GetHitTest(Point point)
		{
//			MessageBox.Show(string.Format("{0} {1}", MyControl.GetHeaderRect(), ));
			return MyControl.TabPages.Count > 0 && MyControl.GetHeaderRect().Contains(MyControl.PointToClient(point));
		}

		void UpdateVerbs()
		{
			_removeVerb.Enabled = MyControl.TabPages.Count > 0;
		}
		void OnAddPage(object sender, EventArgs args)
		{

		}
		void OnRemovePage(object sender, EventArgs args)
		{

		}
	}
}
