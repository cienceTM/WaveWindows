using System;
using System.Windows.Controls;

namespace WaveWindows.Controls.Editor;

internal class TabItem : System.Windows.Controls.TabItem
{
	internal int Id { get; set; }

	internal Monaco GetEditor()
	{
		if (!(base.Content is Monaco result))
		{
			throw new ArgumentNullException("Editor");
		}
		return result;
	}

	public override void OnApplyTemplate()
	{
		base.OnApplyTemplate();
		if (!(GetTemplateChild("RemoveTabButton") is Button button))
		{
			throw new ArgumentNullException("RemoveTabButton");
		}
		button.Click += delegate
		{
			if (!(base.Parent is TabControl tabControl))
			{
				throw new ArgumentNullException("Parent");
			}
			if (tabControl.Items.Count >= 2)
			{
				tabControl.Items.Remove(this);
			}
		};
	}
}
