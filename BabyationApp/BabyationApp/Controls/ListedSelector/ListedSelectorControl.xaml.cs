using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace BabyationApp.Controls.ListedSelector
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListedSelectorControl : ContentView
    {

        public static readonly BindableProperty SpacingProperty = BindableProperty.Create(
            nameof(Spacing),
            typeof(double),
            typeof(ListedSelectorControl),
            defaultValue: 6.0);

        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
            nameof(ItemsSource),
            typeof(IList),
            typeof(ListedSelectorControl),
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                if (bindable is ListedSelectorControl declarer)
                {
                    if (declarer.ItemTemplate != null)
                    {
                        declarer.FillLayout();

                        if (oldValue != null && oldValue is INotifyCollectionChanged)
                        {
                            ((INotifyCollectionChanged)newValue).CollectionChanged -= declarer.OnListedSelectorCollectionChanged;
                        }

                        if (newValue != null && newValue is INotifyCollectionChanged)
                        {
                            ((INotifyCollectionChanged)newValue).CollectionChanged += declarer.OnListedSelectorCollectionChanged;
                        }
                    }
                }
            });

        public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create(
            nameof(ItemTemplate),
            typeof(DataTemplate),
            typeof(ListedSelectorControl),
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                if (bindable is ListedSelectorControl declarer)
                {
                    if (declarer.ItemsSource != null)
                    {
                        declarer.FillLayout();
                    }
                }
            });

        public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(
            nameof(SelectedItem),
            typeof(object),
            typeof(ListedSelectorControl),
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                if (bindable is ListedSelectorControl declarer)
                {
                    ItemSelectableBase selectedItemSelectableBase = null;

                    if (newValue != null)
                    {
                        selectedItemSelectableBase = declarer._visualSelectableItems
                            .FirstOrDefault(selectableItem => selectableItem.BindingContext == newValue);

                        declarer._visualSelectableItems.Except(new[] { selectedItemSelectableBase }).ForEach(c => c.Deselected());

                        if (selectedItemSelectableBase != null)
                        {
                            declarer._selectedVisualItemSelectable = selectedItemSelectableBase;
                            declarer._selectedVisualItemSelectable.Selected();
                        }
                    }
                    else
                    {
                        declarer._visualSelectableItems.ForEach(c => c.Deselected());
                    }

                    declarer.ItemSelected(selectedItemSelectableBase, new EventArgs());
                }
            });

        public event EventHandler<EventArgs> ItemSelected = delegate { };

        private List<ItemSelectableBase> _visualSelectableItems = new List<ItemSelectableBase>();
        private ItemSelectableBase _selectedVisualItemSelectable;

        public ListedSelectorControl()
        {
            InitializeComponent();

            _mainContentSpot_Grid.SetBinding(Grid.RowSpacingProperty, new Binding(nameof(Spacing), source: this));
        }

        public IList ItemsSource
        {
            get => (IList)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public DataTemplate ItemTemplate
        {
            get => (DataTemplate)GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }

        public double Spacing
        {
            get => (double)GetValue(SpacingProperty);
            set => SetValue(SpacingProperty, value);
        }

        public object SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        private void FillLayout()
        {
            try
            {
                ClearVisualContent();

                if (ItemsSource == null)
                {
                    return;
                }

                foreach (object item in ItemsSource)
                {
                    AddSingleItemToVisualTree(PrepareSingleItem(item));
                }
            }
            catch (Exception exc)
            {
                throw new InvalidOperationException("ListedSelector.FillLayout error occured while filling layout. Look at the inner exception.", exc);
            }
        }

        private void AddSingleItemToVisualTree(ItemSelectableBase itemSelectable)
        {
            Grid.SetRow(itemSelectable, _mainContentSpot_Grid.Children.Count);
            _mainContentSpot_Grid.Children.Add(itemSelectable);
        }

        private void ClearVisualContent()
        {
            _visualSelectableItems?.ForEach<ItemSelectableBase>(selectableItem => selectableItem.Dispose());
            _selectedVisualItemSelectable?.Dispose();
            _visualSelectableItems?.Clear();
            _selectedVisualItemSelectable = null;

            _mainContentSpot_Grid.Children.Clear();
            _mainContentSpot_Grid.RowDefinitions.Clear();
        }

        private ItemSelectableBase PrepareSingleItem(object item)
        {
            try
            {
                ItemSelectableBase selectableItem = (ItemTemplate is DataTemplate)
                    ? (ItemTemplate is DataTemplateSelector)
                        ? (ItemSelectableBase)((DataTemplateSelector)ItemTemplate).SelectTemplate(item, this).CreateContent()
                        : (ItemSelectableBase)ItemTemplate.CreateContent()
                    : throw new Exception("Can't resolve ItemTemplate type.");

                selectableItem.SelectionAction = OnItemSelected;
                selectableItem.BindingContext = item;

                _visualSelectableItems.Add(selectableItem);

                return selectableItem;
            }
            catch (Exception exc)
            {
                throw new InvalidOperationException("ListedSelector.PrepareSingleItem - Check validity of ItemsSource or ItemTemplate. Look at the inner exception.", exc);
            }
        }

        private void OnItemSelected(ItemSelectableBase itemSelectableBase)
        {
            if (_selectedVisualItemSelectable != null)
            {
                _selectedVisualItemSelectable.Deselected();
            }

            _selectedVisualItemSelectable = itemSelectableBase;
            _selectedVisualItemSelectable.Selected();

            SelectedItem = itemSelectableBase.BindingContext;
        }

        private void OnListedSelectorCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            try
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    foreach (object item in e.NewItems)
                    {
                        AddSingleItemToVisualTree(PrepareSingleItem(item));
                    }
                }
                else if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    foreach (object item in e.OldItems)
                    {
                        ItemSelectableBase childToRemove = _visualSelectableItems?.FirstOrDefault(selectableItem => selectableItem.BindingContext == selectableItem);
                        _mainContentSpot_Grid.Children.Remove(childToRemove);
                        _visualSelectableItems.Remove(childToRemove);
                        childToRemove.Dispose();
                        ///
                        /// Maby its neccessary to check the selected visual item (need to test this case)
                        ///
                    }
                }
                else if (e.Action == NotifyCollectionChangedAction.Reset)
                {
                    ClearVisualContent();
                }
                else
                {
                    //
                    // TODO: handle other actions...
                    //
                    throw new NotImplementedException(string.Format("ListedSelector.OnListedSelectorCollectionChanged unhandled collection changed action - {0}.", e.Action));
                }
            }
            catch (Exception exc)
            {
                try {
                    FillLayout();
                }
                catch (Exception innerExc) {
                    throw new InvalidOperationException("ListedSelector.OnListedSelectorCollectionChanged. Exception while items collection changed. Look at the inner exception.", innerExc);
                }
            }
        }
    }
}