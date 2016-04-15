using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace GitSharp.Demo
{
    public partial class MainFrame : Window
    {
        public const string CURRENT_REPOSITORY = "repository";

        public MainFrame()
        {
            InitializeComponent();
            m_url_textbox.Text = UserSettings.GetString(CURRENT_REPOSITORY);
            Loaded += (o, args) => Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => LoadRepository(m_url_textbox.Text)));
        }


        Repository m_repository;

        private void OnLoadRepository(object sender, RoutedEventArgs e)
        {
            LoadRepository(m_url_textbox.Text);
        }

        private void LoadRepository(string url)
        {
            var git_url = Repository.FindRepository(url);
            if (git_url == null || !Repository.IsValid(git_url))
            {
                MessageBox.Show("Given path doesn't seem to refer to a git repository: " + url);
                return;
            }
            var repo = new Repository(git_url);
            m_url_textbox.Text = git_url;
            UserSettings.SetValue(CURRENT_REPOSITORY, git_url);
            var head = repo.Head.Target as Commit;
            Debug.Assert(head != null);
            m_repository = repo;
            foreach (TabItem tab in m_tab_control.Items)
            {
                var repo_view = tab.Content as IRepositoryView;
                if (repo_view == null)
                    continue;
                repo_view.Update(m_repository);
            }
        }

        private void OnSelectRepository(object sender, RoutedEventArgs e)
        {
            var dlg = new System.Windows.Forms.FolderBrowserDialog();
            dlg.SelectedPath = Path.GetDirectoryName(UserSettings.GetString(CURRENT_REPOSITORY));
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                m_url_textbox.Text = dlg.SelectedPath;
                LoadRepository(m_url_textbox.Text);
            }
        }

        private void OnMenuClose(object sender, RoutedEventArgs e)
        {
            this.Close();
            Application.Current.Shutdown();
        }
    }
}
