using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace GitSharp.Demo
{
	/// <summary>
	/// Interaction logic for CommitDiffView.xaml
	/// </summary>
	public partial class CommitDiffView
	{
		public CommitDiffView()
		{
			InitializeComponent();
			m_treediff.SelectionChanged += OnSelectionChanged;
		}

		public event Action<Change> SelectionChanged;

		private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			FireSelectionChanged();
		}

		private void FireSelectionChanged()
		{
			if (SelectionChanged != null)
				SelectionChanged(m_treediff.SelectedValue as Change);
		}

		public void Init(Commit c1, Commit c2)
		{
			if (c1 == null)
			{
				m_treediff.ItemsSource = null;
				return;
			}
			//m_title.Content = "Differences between commits " + c1.ShortHash + " and " + c2.ShortHash;
			var changes = c1.CompareAgainst(c2);
			m_treediff.ItemsSource = changes;
			if (changes.Count() > 0)
			{
				m_treediff.SelectedIndex = 0;
			}
			FireSelectionChanged();
		}

		//private void onClose(object sender, RoutedEventArgs e)
		//{
		//    //Closing application
		//    this.Close();
		//}
	}

	public class ChangeColorConverter : IValueConverter
	{
		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var change = value as Change;
			if (change == null)
				return Brushes.Black;
			switch (change.ChangeType)
			{
				case ChangeType.Added:
					return Brushes.Plum;
				case ChangeType.Deleted:
					return Brushes.Red;
				case ChangeType.Modified:
					return Brushes.RoyalBlue;
				default:
					return Brushes.Black;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
