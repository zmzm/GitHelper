﻿using System;
using System.Linq;
using GitSharp.Core.RevPlot;

namespace GitSharp.Demo.HistoryGraph
{
    public partial class HistoryGraphView : IRepositoryView
    {
        public event Action<Commit> CommitClicked;
        public event Action<Core.Ref> LabelClicked;

        private PlotRenderer m_plot_renderer;
        private Repository m_repo;
        private PlotWalk m_revwalk;
        private Selection<Commit> m_selection; 

        public HistoryGraphView()
        {
            InitializeComponent();
            m_plot_renderer = new PlotRenderer();
            m_plot_renderer.Init(m_canvas);
            m_plot_renderer.CommitClicked += OnCommitClicked;
            m_plot_renderer.LabelClicked += OnLabelClicked;
            m_selection = Selection<Commit>.ExclusiveSelection(); 
            m_selection.OnSelect = OnSelect;
            m_selection.OnUnselect = OnUnselect;
        }

        public void Update(Repository repo)
        {
            m_repo = repo;
            var list = new PlotCommitList();
            m_revwalk = new PlotWalk(repo);
            m_revwalk.markStart(((Core.Repository)repo).getAllRefsByPeeledObjectId().Keys.Select(id => m_revwalk.parseCommit(id)));
            list.Source(m_revwalk);
            list.fillTo(1000);
            m_plot_renderer.Update(list);
            //var rw = new RevWalk(repo);
            //rw.RevSortStrategy.Add(RevSort.Strategy.COMMIT_TIME_DESC);
            //rw.RevSortStrategy.Add(RevSort.Strategy.TOPO);
            //rw.markStart(((Core.Repository)repo).getAllRefsByPeeledObjectId().Keys.Select(id => rw.parseCommit(id)));
            //m_renderer.Update(rw);
        }

        private void OnSelect(Commit c)
        {
            m_plot_renderer.Select(c.Hash);
        }

        private void OnUnselect(Commit c)
        {
            m_plot_renderer.Unselect(c.Hash);
        }

        private void OnCommitClicked(PlotCommit commit)
        {
            if (CommitClicked==null)
                return;
            var c = new Commit(m_repo, commit.Name);
            m_selection.Update(c);
            CommitClicked(c);
        }

        private void OnLabelClicked(Core.Ref @ref)
        {
            if (LabelClicked == null)
                return;
            if (@ref == null)
                return;
            LabelClicked(@ref);
        }

        private void ScrollViewer_ScrollChanged(object sender, System.Windows.Controls.ScrollChangedEventArgs e)
        {

        }
    }
}
