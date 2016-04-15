using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GitSharp.Demo.CommitView
{
	/// <summary>
	/// Interaction logic for CommitDialog.xaml
	/// </summary>
	public partial class CommitDialog : Window
	{
		public CommitDialog()
		{
			InitializeComponent();
			Width = double.NaN;
			Height = double.NaN;
		}

		private Repository m_repository;

		public Repository Repository
		{
			get { return m_repository; }
			set
			{
				m_repository = value;
			}
		}

		public void Init(IEnumerable<PathStatus> paths)
		{
			m_list.ItemsSource = paths;
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			if (m_message.Text.Trim().Length == 0)
			{
				MessageBox.Show("A commit message is required!");
				return;
			}
			Repository.Commit(m_message.Text, new Author(Repository.Config["user.name"] ?? "anonymous", Repository.Config["user.email"] ?? ""));
			Close();
		}

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}
