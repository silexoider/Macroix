using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Macroix.UI.Controls
{
	using System.Drawing;
	using UI.Design;

//	[ClassInterface(ClassInterfaceType.AutoDispatch)]
//	[ComVisible(true)]
//	[DesignTimeVisible(false)]
//	[ToolboxItem(false)]
	[Designer(typeof(MyTabPageDesigner))]
	public class MyTabPage : Panel
	{
		MyTabControl _owner;

		[Browsable(true)]
		[EditorBrowsable(EditorBrowsableState.Always)]
		public override string Text 
		{ 
			get => base.Text;
			set
			{
				base.Text = value;
				NotifyOwner();
			}
		}

		public MyTabPage()
		{
			Visible = false;
		}

		public void Init(MyTabControl owner)
		{
			_owner = owner;
			owner.TabContainer.Controls.Add(this);
			Dock = DockStyle.Fill;
			NotifyOwner();
		}
		public void Deinit()
		{
			_owner.TabContainer.Controls.Remove(this);
			_owner = null;
			NotifyOwner();
		}

		void NotifyOwner()
		{
			_owner?.InvalidateHeaderItem(_owner.TabPages.Find(this));
		}
	}
}
