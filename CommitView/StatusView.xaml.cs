using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace GitSharp.Demo.CommitView
{
	/// <summary>
	/// Interaction logic for StatusWindow.xaml
	/// </summary>
	public partial class StatusView : IRepositoryView
	{
		public StatusView()
		{
			InitializeComponent();
			m_status_list.SelectionChanged += OnStatusViewSelectionChanged;
		}

		public void Update(Repository repo)
		{
			Repository = repo;
			m_status_list.Update(repo);
		}

		private void OnStatusViewSelectionChanged(IEnumerable<PathStatus> status_paths)
		{
			var status_path = status_paths.FirstOrDefault();
			if (status_path == null)
				return;
			byte[] a = new byte[0];
			var a_path = Path.Combine(Repository.WorkingDirectory, status_path.Path);
			if (new FileInfo(a_path).Exists)
			{
				if (Diff.IsBinary(a_path) == true)
					a = Encoding.ASCII.GetBytes("Binary content\nFile size: " + new FileInfo(a_path).Length);
				else
					a = File.ReadAllBytes(a_path);
			}
			byte[] b = new byte[0];
			var blob = Repository.Index[status_path.Path];
			if (blob != null)
				if (Diff.IsBinary(blob.RawData) == true)
					b = Encoding.ASCII.GetBytes("Binary content\nFile size: " + blob.RawData.Length);
				else
					b = blob.RawData;
			//m_diff_view.Init(new Diff(a, b));
		}

		public Repository Repository
		{
			get;
			private set;
		}

		private void OnCommitButtonClick(object sender, RoutedEventArgs e)
		{
			m_status_list.StartCommitDialog();
		}

        private void m_status_list_Loaded(object sender, RoutedEventArgs e)
        {

        }
	}
}
