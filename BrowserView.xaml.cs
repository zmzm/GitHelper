using System;
using System.IO;
using System.Linq;
using System.Security;
using System.Windows;
using System.Windows.Documents;
using System.Diagnostics;
using System.Windows.Threading;
using GitSharp.Demo.CommitView;


namespace GitSharp.Demo
{

	public partial class BrowserView : IRepositoryView
	{
		private string selectedLabel = "";

        public BrowserView()
		{
			InitializeComponent();
			m_history_graph.CommitClicked += SelectCommit;
            m_history_graph.LabelClicked += SelectLabel;
		}

		public Repository Repository { get; private set; }

		private void SelectObject(AbstractObject node)
		{
			if (node.IsBlob)
			{
				var blob = node as Leaf;
				var text = blob.Data;
				var p = new Paragraph();
				p.Inlines.Add(text);
			}
		}

		private void SelectCommit(Commit commit)
		{
			if (commit == null || commit.Tree == null)
				return;
			m_commit_view.Commit = commit;
		}

        private void SelectLabel(Core.Ref label)
        {
            if (label == null)
                return;
            selectedLabel = label.Name;
            selected_branch.Content = "Выбраная ветка: " + selectedLabel;
        }

		private void OnDiffSelectedCommits(object sender, RoutedEventArgs e)
		{
		}

        private void CreateBranch(object sender, RoutedEventArgs e) 
        {
            Branch.Create(Repository, branch_name.Text);
            MessageBox.Show("Ветка " + branch_name.Text +" создана!");
            Update(Repository);
        }

        private void DeleteBranch(object sender, RoutedEventArgs e)
        {
            if (selectedLabel.Equals("") || selectedLabel.Equals("HEAD"))
            {
                MessageBox.Show("Выберите ветку!!!");
            }
            else 
            {
                Branch b = new Branch(Repository, selectedLabel);
                b.Delete();
                selectedLabel = String.Empty;
                selected_branch.Content = String.Empty;
                Update(Repository);
            }
        }
        private void CheckoutBranch(object sender, RoutedEventArgs e)
        {
            if (selectedLabel.Equals("") || selectedLabel.Equals("HEAD"))
            {
                MessageBox.Show("Выберите ветку!!!");
            }
            else
            {
                Branch b = new Branch(Repository, selectedLabel);
                b.Checkout();
                MessageBox.Show("Вы переключились на ветку " + b.Name);
            }
        }

		#region IRepositoryView Members

		public void Update(Repository repository)
		{
			Repository = repository;
			SelectCommit(Repository.Head.CurrentCommit);
			m_history_graph.Update(Repository);
		}

		#endregion
	}
}
