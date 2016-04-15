using System;
using System.Collections.Generic;

namespace GitSharp.Demo.HistoryGraph
{
    public class Selection<T>
    {
        public event Action SelectionChanged;

        public HashSet<T> SelectedItems
        {
            get { return m_selected_items; }
        }
        private readonly HashSet<T> m_selected_items = new HashSet<T>(); 


        #region --> Actions to be supplied by user


        public Action<T> OnSelect;

        public Action<T> OnUnselect;

        public Func<bool> IsCtrlDown;

        public Action<IEnumerable<T>> SelectionStrategy;


        #endregion

        #region --> Extern selecton manipulation interface


        public void Update(params T[] items)
        {
            SelectionStrategy(items);
        }

        public void Update(IEnumerable<T> items)
        {
            SelectionStrategy(items);
        }

        public void Clear()
        {
            foreach (var item in m_selected_items)
                OnUnselect(item);
            m_selected_items.Clear();
            if (SelectionChanged != null) SelectionChanged();
        }

        public void Select(T item)
        {
            Select(item, true);
        }

        public void Select(T item, bool notify)
        {
            m_selected_items.Add(item);
            OnSelect(item);
            if (notify && SelectionChanged != null) SelectionChanged();
        }

        public void Select(IEnumerable<T> items)
        {
            foreach (var item in items)
                Select(item, false);
            if (SelectionChanged != null) SelectionChanged();
        }

        public void Unselect(T item)
        {
            Unselect(item, true);
        }

        public void Unselect(T item, bool notify)
        {
            m_selected_items.Remove(item);
            OnUnselect(item);
            if (notify && SelectionChanged != null) SelectionChanged();
        }

        public void UnSelect(IEnumerable<T> items)
        {
            foreach (var item in items)
                Unselect(item, false);
            if (SelectionChanged != null) SelectionChanged();
        }


#endregion

        public bool IsSelected(T item)
        {
            return m_selected_items.Contains(item);
        }

        #region --> Preconfigured Selections


        public static Selection<T> ToggleSelecton()
        {
            var selecton = new Selection<T>();
            selecton.SelectionStrategy = new Action<IEnumerable<T>>(items =>
                {
                    foreach (var item in items)
                    {
                        if (selecton.m_selected_items.Contains(item))
                        {
                            selecton.OnUnselect(item);
                            selecton.m_selected_items.Remove(item);
                        }
                        else
                        {
                            selecton.OnSelect(item);
                            selecton.m_selected_items.Add(item);
                        }
                    }
                    if (selecton.SelectionChanged != null) selecton.SelectionChanged();
                });
            return selecton;
        }

        public static Selection<T> ExclusiveSelection()
        {
            var selection = new Selection<T>();
            selection.SelectionStrategy = new Action<IEnumerable<T>>(items =>
                {
                    selection.Clear();
                    foreach (var item in items)
                    {
                        selection.OnSelect(item);
                        selection.m_selected_items.Add(item);
                    }
                    if (selection.SelectionChanged != null) selection.SelectionChanged();
                });
            return selection;
        }

        public static Selection<T> StandardSelection()
        {
            var selection = new Selection<T>();
            selection.SelectionStrategy = new Action<IEnumerable<T>>(items =>
                {
                    if (!selection.IsCtrlDown())
                        selection.Clear();
                    foreach (var item in items)
                    {
                        if (selection.m_selected_items.Contains(item))
                        {
                            selection.OnUnselect(item);
                            selection.m_selected_items.Remove(item);
                        }
                        else
                        {
                            selection.OnSelect(item);
                            selection.m_selected_items.Add(item);
                        }
                    }
                    if (selection.SelectionChanged != null) selection.SelectionChanged();
                });
            return selection;
        }


        #endregion
    }
}