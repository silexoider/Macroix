using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing.Design;
using System.ComponentModel.Design.Serialization;

namespace Macroix.UI.Controls
{
	using UI.Design;

	[Designer(typeof(MyTabControlDesigner))]
	[DesignerSerializer(typeof(MyTabControlCodeDomSerializer), typeof(CodeDomSerializer))]
	public class MyTabControl : Control
	{
		int _hoverTabIndex = -1;
		int _activeTabIndex = -1;
		Color _textColor = Color.White;
		Color _hoverColor = Color.FromArgb(0x60, 0x00, 0x60);
		Color _activeColor = Color.Black;
		Color _borderColor = Color.Purple;
		Color _headerColor = Color.Purple;
		int _borderSize = 10;
		int _headerSize = 50;
		Panel _tabContainer = new Panel();

		public Panel TabContainer
		{
			get { return _tabContainer; }
		}
		public Color TextColor 
		{ 
			get { return _textColor; }
			set { SetAndRedraw(ref _textColor, value); }
		}
		public Color HoverColor 
		{ 
			get { return _hoverColor; }
			set { SetAndRedraw(ref _hoverColor, value); }
		}
		public Color ActiveColor 
		{ 
			get { return _activeColor; }
			set { SetAndRedraw(ref _activeColor, value); }
		}
		public Color BorderColor 
		{ 
			get { return _borderColor; }
			set { SetAndRedraw(ref _borderColor, value); }
		}
		public Color HeaderColor 
		{ 
			get { return _headerColor; }
			set { SetAndRedraw(ref _headerColor, value); }
		}
		public int BorderSize 
		{ 
			get { return _borderSize; }
			set { SetAndRedraw(ref _borderSize, value); }
		}
		public int HeaderSize
		{ 
			get { return _headerSize; }
			set { SetAndRedraw(ref _headerSize, value); }
		}
		public override Font Font 
		{
			get { return base.Font; }
			set
			{
				base.Font = value;
				Invalidate();
			}
		}
		public int ActiveTabIndex
		{
			get { return _activeTabIndex; }
			set { ActivateTab(value); }
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public MyTabPageCollection TabPages
		{
			get; private set;
		}

		public MyTabControl()
		{
			TabPages = new MyTabPageCollection(this);
			ResizeRedraw = true;
			DoubleBuffered = true;
			Font = new Font("Impact", 20);
			Width = 500;
			Height = 400;

			_tabContainer.Left = BorderSize;
			_tabContainer.Top = HeaderSize;
			_tabContainer.Width = Width - 2 * BorderSize;
			_tabContainer.Height = Height - BorderSize - HeaderSize;
			_tabContainer.Visible = true;
			_tabContainer.BackColor = Color.White;
			_tabContainer.ForeColor = Color.White;
			_tabContainer.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			Controls.Add(_tabContainer);
		}

		public void InvalidateHeader()
		{
			Invalidate(GetHeaderRect());
		}
		public void InvalidateHeaderItem(int index)
		{
			Rectangle? rect = GetHeaderItemRect(index);
			if (rect.HasValue)
				Invalidate(rect.Value);
		}
		public void ActivateTab(int index)
		{
			if (index >= 0 && index != _activeTabIndex)
			{
				if (_activeTabIndex >= 0)
				{
					InvalidateHeaderItem(_activeTabIndex);
					MyTabPage oldPage = TabPages[_activeTabIndex];
					oldPage.Visible = false;
				}
				_activeTabIndex = index;
				InvalidateHeaderItem(index);
				MyTabPage newPage = TabPages[_activeTabIndex];
				newPage.Visible = true;
			}
		}
		public void ActivateTab(MyTabPage page)
		{
			ActivateTab(TabPages.Find(page));
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			DrawFrame(e.Graphics);
		}
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			int index = GetHeaderItemIndex(e.Location);
			if (index != _hoverTabIndex)
			{
				if (_hoverTabIndex >= 0 && _hoverTabIndex != _activeTabIndex)
					InvalidateHeaderItem(_hoverTabIndex);
				_hoverTabIndex = index;
				if (_hoverTabIndex >= 0 && _hoverTabIndex != _activeTabIndex)
					InvalidateHeaderItem(_hoverTabIndex);
			}
		}
		protected override void OnMouseClick(MouseEventArgs e)
		{
			base.OnMouseClick(e);
			int index = GetHeaderItemIndex(e.Location);
			ActivateTab(index);
		}
		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);
			if (_hoverTabIndex >= 0)
			{
				Invalidate(GetHeaderItemRect(_hoverTabIndex).Value);
				_hoverTabIndex = -1;
			}
		}

		void SetAndRedraw<T>(ref T field, T value)
		{
			field = value;
			Invalidate();
		}
		int GetHeaderItemIndex(Point point)
		{
			Rectangle headerRect = GetHeaderRect();
			if (TabPages.Count > 0 && headerRect.Contains(point))
			{
				float itemWidth = (float)headerRect.Width / TabPages.Count;
				return (int)Math.Floor((point.X - headerRect.X) / itemWidth);
			}
			return -1;
		}
		public Rectangle GetHeaderRect()
		{
			return new Rectangle(BorderSize, 0, Width - BorderSize * 2, HeaderSize);
		}
		public Rectangle? GetHeaderItemRect(int index)
		{
			if (TabPages.Count == 0)
				return null;
			Rectangle headerRect = GetHeaderRect();
			float itemWidth = (float)headerRect.Width / TabPages.Count;
			return
				new Rectangle(
					BorderSize + (int)Math.Round(index * itemWidth),
					0,
					(int)Math.Round(itemWidth),
					headerRect.Height
				);
		}

		void DrawFrame(Graphics graphics)
		{
			DrawBorders(graphics);
			DrawHeader(graphics);
		}
		void DrawBorders(Graphics graphics)
		{
			Brush brush = new SolidBrush(BorderColor);
			graphics.FillRectangle(brush, new Rectangle(0, 0, BorderSize, Height));
			graphics.FillRectangle(brush, new Rectangle(BorderSize, Height - BorderSize, Width - 2 * BorderSize, BorderSize));
			graphics.FillRectangle(brush, new Rectangle(Width - BorderSize, 0, BorderSize, Height));
		}
		void DrawHeaderItem(Graphics graphics, int index)
		{
			Color color = HeaderColor;
			if (index == _hoverTabIndex)
				color = HoverColor;
			if (index == _activeTabIndex)
				color = ActiveColor;
			Brush brush = new SolidBrush(color);
			Rectangle? itemRect = GetHeaderItemRect(index);
			if (itemRect.HasValue)
			{
				graphics.FillRectangle(brush, itemRect.Value);
				string text = TabPages[index].Text;
				SizeF textSize = graphics.MeasureString(text, Font);
				PointF textPosition =
					new PointF(
						itemRect.Value.Left + (float)itemRect.Value.Width / 2 - textSize.Width / 2,
						(float)itemRect.Value.Top + (float)itemRect.Value.Height / 2 - textSize.Height / 2
					);
				graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
				graphics.DrawString(text, Font, new SolidBrush(TextColor), textPosition);
			}
		}
		void DrawHeader(Graphics graphics)
		{
			Brush brush = new SolidBrush(HeaderColor);
			if (TabPages.Count == 0)
				graphics.FillRectangle(brush, GetHeaderRect());
			for (int i = 0; i < TabPages.Count; i++)
				DrawHeaderItem(graphics, i);
		}

		[ToolboxItem(false)]
		[Editor(typeof(MyTabPageCollectionEditor), typeof(UITypeEditor))]
		public class MyTabPageCollection : Collection<MyTabPage>
		{
			MyTabControl _owner;
			public MyTabPageCollection(MyTabControl owner)
			{
				_owner = owner;
			}
			public int Find(MyTabPage page)
			{
				for (int i = 0; i < Count; i++)
					if (this[i] == page)
						return i;
				return -1;
			}

			protected override void ClearItems()
			{
				foreach (MyTabPage page in this)
					page.Deinit();
				base.ClearItems();
				_owner.ActiveTabIndex = -1;
			}
			protected override void InsertItem(int index, MyTabPage item)
			{
				base.InsertItem(index, item);
				item.Init(_owner);
				if (_owner.TabPages.Count == 1)
					_owner.ActiveTabIndex = 0;
			}
			protected override void RemoveItem(int index)
			{
				MyTabPage page = this[index];
				base.RemoveItem(index);
				page.Deinit();
				if (index <= _owner.ActiveTabIndex)
					_owner.ActiveTabIndex--;
			}
			protected override void SetItem(int index, MyTabPage item)
			{
				this[index].Deinit();
				base.SetItem(index, item);
				item.Init(_owner);
			}
		}
	}
}
