using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using BabyationApp.Common;
using BabyationApp.Models;



namespace BabyationApp.Controls.Views
{
    /// <summary>
    /// ListView extented with recyle of items set to true
    /// </summary>
    public class ListViewEx : ListViewBase
    {
        /// <summary>
        /// constructor
        /// </summary>
        public ListViewEx() : base(true)
        {
        }
    }

    /// <summary>
    /// ListView extented with recyle of items to set to false
    /// </summary>
    public class ListViewExNoRecycle : ListViewBase
    {
        /// <summary>
        /// constructor
        /// </summary>
        public ListViewExNoRecycle() : base(false)
        {
        }
    }

    /// <summary>
    /// Delete for cell setup when they are created
    /// </summary>
    /// <param name="cell"></param>
    /// <param name="index"></param>
    public delegate void ListCellSetup(Cell cell, int index);

    /// <summary>
    /// ListView to customize through platform dependent renderers
    /// </summary>
    public class ListViewBase : ListView
    {
        /// <summary>
        /// Event fires when a cell setup is made. User can bind to this to get a new cell created event
        /// </summary>
        public event ListCellSetup SetupCell;
        public ListViewBase(bool recyle = true) : base(recyle ? ListViewCachingStrategy.RecycleElement : ListViewCachingStrategy.RetainElement)
        {
            SeparatorVisibility = SeparatorVisibility.None;
            IsPullToRefreshEnabled = false;
            ItemSelected += (s,e) => {
                UpdateSelection(this.ItemsSource, e.SelectedItem as ModelItemBase);
            };
            SeparatorColor = Color.Transparent;
        }

        /// <summary>
        /// Update the selection of rows
        /// </summary>
        /// <param name="items">row items</param>
        /// <param name="selectedItem">selected item</param>
        void UpdateSelection(System.Collections.IEnumerable items, ModelItemBase selectedItem)
        {
            foreach (Object o in items)
            {
                var item = o as ModelItemBase;
                if (item != null)
                {
                    item.IsFocused = o == selectedItem;
                }
                else
                {
                    var list = o as System.Collections.IEnumerable;
                    if (list != null)
                    {
                        UpdateSelection(list, selectedItem);
                    }
                }
            }
        }


        /// <summary>
        /// Overrides the SetupContent of listview to set the index of the currently created cell
        /// </summary>
        /// <param name="content"></param>
        /// <param name="index"></param>
        protected override void SetupContent(Cell content, int index)
        {            
            var viewCell = content as ViewCell;
            if (viewCell != null && viewCell.View != null && viewCell.View.BindingContext != null)
            {
                ModelItemBase item = viewCell.View.BindingContext as ModelItemBase;
                if (item != null)
                {
                    item.Index = index;

                }
            }
            //viewCell.View.BackgroundColor = index % 2 == 0 ? Color.Blue : Color.Red;
            base.SetupContent(content, index);

            SetupCell?.Invoke(content, index);
        }
    }

    

    /// <summary>
    /// Generic model ListView.  
    /// </summary>
    /// <remarks>This model mainly runs on main thread and sync to another collection that may runs on other thread</remarks>
    /// <typeparam name="T"></typeparam>
    public class ListViewList<T> : ObservableCollection<T>
    {
        /// <summary>
        /// Notification delegate this collection usually when the internal collection changes.
        /// </summary>
        /// <param name="sender">Sender of this event</param>
        /// <param name="obj">Corresponding model-item chat changed</param>
        public delegate void ListViewListItemChanged(ListViewList<T> sender, T obj);


        /// <summary>
        /// Event fires when an item added to this collection
        /// </summary>
        public ListViewListItemChanged ItemAdded;

        /// <summary>
        /// Event fires when an item remove from this collection
        /// </summary>
        public ListViewListItemChanged ItemRemoved;


        /// <summary>
        /// Constructors
        /// </summary>
        /// <param name="source">Another colleciton that this collection sync to</param>
        public ListViewList(ObservableCollection<T> source)
        {
            foreach (T t in source)
            {
                Add(t);
            }

            source.CollectionChanged += (sender, args) =>
            {
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                {
                    try
                    {
                        if (args.Action == NotifyCollectionChangedAction.Reset)
                        {
                            foreach (T t in Items)
                            {
                                ItemRemoved?.Invoke(this, t);
                            }
                            Clear();
                        }

                        if (args.OldItems != null)
                        {
                            foreach (T t in args.OldItems)
                            {
                                if (Contains(t))
                                {
                                    Remove(t);
                                    ItemRemoved?.Invoke(this, t);
                                }
                            }
                        }

                        if (args.NewItems != null)
                        {
                            foreach (T t in args.NewItems)
                            {
                                if (!Contains(t))
                                {
                                    Add(t);
                                    ItemAdded?.Invoke(this, t);
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("Exeption in CollectionChanged : " + e.Message);
                    }
                });
            };
        }
    }
}
