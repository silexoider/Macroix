using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Macroix.UI.Design
{
	internal class MyTabPageDesigner : ScrollableControlDesigner
	{
		protected override void PreFilterProperties(IDictionary properties)
		{
			properties.Remove("Location");
			properties.Remove("Size");
			properties.Remove("Anchor");
			properties.Remove("Dock");
			properties.Remove("Visible");

/*			StringBuilder message = new StringBuilder();
			foreach (object key in properties.Keys)
				message.AppendLine(string.Format("{0}: {1}", key, properties[key]));
			MessageBox.Show(message.ToString());*/

			base.PreFilterProperties(properties);
		}
	}
}
