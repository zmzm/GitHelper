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
                Update(Repository);
            }
        }
        private void MergeBranches(object sender, RoutedEventArgs e)
        {
            if (branch_one.Text.Equals("") || branch_two.Text.Equals(""))
            {
                MessageBox.Show("Пустые поля не допустимы!");
            }
            else
            {
                if (Repository.Branches.ContainsKey(branch_one.Text) && Repository.Branches.ContainsKey(branch_two.Text))
                {
                    Branch b1 = new Branch(Repository, branch_one.Text);
                    Branch b2 = new Branch(Repository, branch_two.Text);
                    System.Collections.Generic.List<Commit> parents1 = b1.CurrentCommit.Parents.ToList();
                    foreach (Commit commit in parents1)
                    {
                        if(commit.Equals(b2.CurrentCommit))
                        {
                            MessageBox.Show("Нет необходимости слияния!");
                            return;
                        }
                    }
                    if (b1.Fullname.Equals(b2.Fullname))
                    {
                        MessageBox.Show("Выберите разные ветки!");
                        return;
                    }
                    b1.Checkout();
                    GitSharp.Commands.MergeResult result = b1.Merge(b2, Commands.MergeStrategy.Ours);
                    Update(Repository);
                }
                else
                {
                    MessageBox.Show("Таких веток нет!");
                }
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
