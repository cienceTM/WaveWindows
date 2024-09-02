using System;
using System.Windows.Controls;
using System.Windows.Input;
using WaveWindows.Interfaces;

namespace WaveWindows.Controls.Editor;

internal class TabControl : System.Windows.Controls.TabControl
{
	private Monaco _CurrentEditor;

	private ScrollViewer ScrollViewer;

	private EditorInterface.EditorOptions EditorOptions;

	internal Monaco CurrentEditor
	{
		get
		{
			return _CurrentEditor;
		}
		set
		{
			_CurrentEditor = value;
		}
	}

	internal void SetEditorOptions(EditorInterface.EditorOptions Options)
	{
		EditorOptions = Options;
	}

	internal TabItem AddTab(string Header, string Content)
	{

		TabItem tabItem = new TabItem
		{
            Id = new Random().Next(),
			Header = Header,
			Content = new Monaco("http://localhost:6969", Content, EditorOptions)
        };

		AddChild(tabItem);
		base.SelectedItem = tabItem;
		CurrentEditor = tabItem.Content as Monaco;
		if (ScrollViewer != null)
		{
			ScrollViewer.ScrollToRightEnd();
		}

		return tabItem;
    }

    internal Monaco SelectById(int Id)
	{
		TabItem[] allTabs = GetAllTabs();
		TabItem[] array = allTabs;
		foreach (TabItem tabItem in array)
		{
			if (tabItem.Id == Id)
			{
				base.SelectedItem = tabItem;
				CurrentEditor = tabItem.Content as Monaco;
				return CurrentEditor;
			}
		}
		return null;
	}

	internal TabItem[] GetAllTabs()
	{
		TabItem[] array = new TabItem[base.Items.Count];
		for (int i = 0; i < base.Items.Count; i++)
		{
			array[i] = base.Items.GetItemAt(i) as TabItem;
		}
		return array;
	}

	internal Monaco[] GetAllEditors()
	{
		Monaco[] array = new Monaco[base.Items.Count];
		for (int i = 0; i < base.Items.Count; i++)
		{
			array[i] = (base.Items.GetItemAt(i) as TabItem).Content as Monaco;
		}
		return array;
	}

	internal static Monaco GetSelectedOrLastEditor(TabControl tabControl)
	{
		return (((tabControl.SelectedItem != null) ? tabControl.SelectedItem : tabControl.Items.GetItemAt(tabControl.Items.Count - 1)) as TabItem).Content as Monaco;
	}

	public override void OnApplyTemplate()
	{
		base.OnApplyTemplate();
		base.SelectionChanged += delegate(object sender, SelectionChangedEventArgs e)
		{
			if (e.Source is TabControl tabControl)
			{
				CurrentEditor = GetSelectedOrLastEditor(tabControl);
			}
		};
		ScrollViewer ScrollViewer = GetTemplateChild("ScrollViewer") as ScrollViewer;
		Button button = GetTemplateChild("AddTabButton") as Button;
		if (ScrollViewer == null)
		{
			throw new ArgumentNullException("ScrollViewer");
		}
		if (button == null)
		{
			throw new ArgumentNullException("AddTabButton");
		}
		this.ScrollViewer = ScrollViewer;
		ScrollViewer.PreviewMouseWheel += delegate(object sender, MouseWheelEventArgs e)
		{
			if (e.Delta > 0)
			{
				ScrollViewer.ScrollToHorizontalOffset(ScrollViewer.HorizontalOffset - 15.0);
			}
			else if (e.Delta < 0)
			{
				ScrollViewer.ScrollToHorizontalOffset(ScrollViewer.HorizontalOffset + 15.0);
			}
			e.Handled = true;
		};
		button.Click += delegate
		{
			AddTab("Untitled Tab", "print(\"Hello World!\")");
		};
	}
}
