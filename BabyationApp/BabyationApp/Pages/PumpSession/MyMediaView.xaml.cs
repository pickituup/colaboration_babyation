using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using BabyationApp.Controls.Views;
using BabyationApp.Managers;
using BabyationApp.Models;
using Xamarin.Forms;
using BabyationApp.Interfaces;
using BabyationApp.Resources;
using BabyationApp.Helpers;

namespace BabyationApp.Pages.PumpSession
{
    /// <summary>
    /// This class is a composite view to show/add/delete/select media from the device in platform independent way
    /// </summary>
    public partial class MyMediaView : ContentView
    {
        public MyMediaViewModel ViewModel = new MyMediaViewModel();
        private PageBase _parentPage;
        private int _currentFileIndex = -1;
        private GalleryModel _galleryModel;

        public MyMediaView()
        {
            InitializeComponent();

            BtnLeft.Clicked += BtnLeft_Clicked;
            BtnRight.Clicked += BtnRight_Clicked;

            AddMediaControl.CommandEx = new Command(AddMediaActionHandler);

            _galleryModel = new GalleryModel(MediaManager.Instance.Media);
            _galleryModel.MediaRemoved += _galleryModel_MediaRemoved;
            _galleryModel.CellClicked += _galleryModel_CellClicked;
            _galleryModel.CellDelete += _galleryModel_CellDelete;
            listView.ItemsSource = _galleryModel;

            this.BindingContext = ViewModel;
        }

        public void Initialize(PageBase page)
        {
            _parentPage = page;
        }

        public void AboutToShow()
        {
            ViewModel.IsSingleViewMode = true;
            ViewModel.IsMediaEditMode = false;

            _parentPage.Title = AppResource.MyMedia_Upper;

            _parentPage.Titlebar.LeftButton.IsVisible = true;
            _parentPage.Titlebar.LeftButton.SetDynamicResource(StyleProperty, "BackButton");
            _parentPage.Titlebar.LeftButton.Clicked += BackButton_Clicked;

            if (MediaManager.Instance.CurrentPumpPicture != null)
            {
                _currentFileIndex = MediaManager.Instance.Media.IndexOf(MediaManager.Instance.CurrentPumpPicture);
            }
            else
            {
                _currentFileIndex = -1;
            }

            SyncImgNavButtons();
        }

        public void AboutToHide()
        {
            UpdateSingleMode();

            _parentPage.Titlebar.LeftButton.Clicked -= BackButton_Clicked;
        }

        public void UpdateSingleMode()
        {
            if (_currentFileIndex >= 0 && _currentFileIndex < MediaManager.Instance.Media.Count)
            {
                MediaManager.Instance.CurrentPumpPicture = MediaManager.Instance.Media[_currentFileIndex];
            }
            else
            {
                MediaManager.Instance.CurrentPumpPicture = null;
            }
        }

        public void RestoreEditMode()
        {
            ViewModel.IsSingleViewMode = true;
            ViewModel.IsMediaEditMode = false;

            _parentPage.Title = AppResource.MyMedia_Upper;

            if (_currentFileIndex >= MediaManager.Instance.Media.Count)
            {
                _currentFileIndex = MediaManager.Instance.Media.Count > 0 ? 0 : -1;
            }
            else if (_currentFileIndex < 0 && MediaManager.Instance.Media.Count > 0)
            {
                _currentFileIndex = 0;
            }

            SyncImgNavButtons();
        }

        #region Event habdlers

        void BackButton_Clicked(object sender, EventArgs e)
        {
            if (ViewModel.IsMediaEditMode)
            {
                RestoreEditMode();
            }
            else
            {
                if( _parentPage.GetType().Equals(typeof(PumpSessionPage)) )
                {
                    (_parentPage as PumpSessionPage).HideMediaView();
                }
            }
        }

        void BtnLeft_Clicked(object sender, EventArgs e)
        {
            if (_currentFileIndex > 0 && MediaManager.Instance.Media.Count > 0)
            {
                _currentFileIndex--;
                SyncImgNavButtons();
            }
        }

        void BtnRight_Clicked(object sender, EventArgs e)
        {
            if (_currentFileIndex < MediaManager.Instance.Media.Count - 1 && MediaManager.Instance.Media.Count > 0)
            {
                _currentFileIndex++;
                SyncImgNavButtons();
            }
        }

        void _galleryModel_CellClicked(GalleryModel model, GalleryCellModel cell)
        {
            if (ViewModel.IsMediaEditMode)
            {
                model.ClearSelection();
                cell.IsSelected = true;
            }
        }

        async void _galleryModel_CellDelete(GalleryModel model, GalleryCellModel cell)
        {
            if (ViewModel.IsMediaEditMode)
            {
                await MediaManager.Instance.Remove(cell.Media);
                model.RemoveCell(cell);
            }
        }

        void _galleryModel_MediaRemoved(GalleryModel model, IList<Media> list)
        {
            if (ViewModel.IsMediaEditMode)
            {
                //TODO: Refresh UI if needed
            }
        }

        async void AddMediaActionHandler(object obj)
        {
            if (ViewModel.IsSingleViewMode)
            {
                ViewModel.IsSingleViewMode = false;
                ViewModel.IsMediaEditMode = true;

                _parentPage.Title = AppResource.EditMyMedia_Upper;

                _galleryModel.ClearSelection();
            }
            else if( ViewModel.IsMediaEditMode )
            {
                ///
                /// Uncoment if it's necessary to use busy indicator
                /// 
                _isBusyIndicator_BusyIndicator.IsVisible = true;

                var photoResult = await PictureManager.Instance.SelectFromGalleryAsync(true);
                if (photoResult != null)
                {
                    foreach (ImageSource source in photoResult.Pictures)
                    {
                        var media = MediaManager.Instance.CreateImageMedia(source);
                        await MediaManager.Instance.Add(media);
                    }
                }

                ///
                /// Uncoment if it's necessary to use busy indicator
                /// 
                _isBusyIndicator_BusyIndicator.IsVisible = false;
            }
        }


        #endregion

        #region Private
        /// <summary>
        /// Update the states of the nav buttons based on the currently selected item
        /// </summary>
        private void SyncImgNavButtons()
        {
            try
            {
                BtnLeft.IsVisible = _currentFileIndex > 0 && MediaManager.Instance.Media.Count > 0;
                BtnRight.IsVisible = _currentFileIndex < MediaManager.Instance.Media.Count - 1 &&
                                     MediaManager.Instance.Media.Count > 0;
                if (_currentFileIndex >= 0 && _currentFileIndex < MediaManager.Instance.Media.Count)
                {
                    ImgCurrent.Source = MediaManager.Instance.Media[_currentFileIndex].Image;
                }
            }
            catch (Exception exc)
            {
                System.Diagnostics.Debug.WriteLine("EXCEPTION -- " + exc.Message);
            }
        }

        #endregion
    }


    /// <summary>
    /// The UI model class for the MyMediaView
    /// </summary>
    public class MyMediaViewModel : ModelItemBase
    {
        private bool _isSingleViewMode;
        /// <summary>
        /// Gets/Sets whether the view should be in signle-view mode or grid view mode
        /// </summary>
        public bool IsSingleViewMode
        {
            get => _isSingleViewMode;
            set
            {
                SetPropertyChanged(ref _isSingleViewMode, value);
                SetPropertyChanged(nameof(AddControlText));
                SetPropertyChanged(nameof(AddControlImage));
                SetPropertyChanged(nameof(AddControlOutlined));
            }
        }

        private bool _isMediaEditMode;
        /// <summary>
        /// Gets/Sets whether the view should let the user to edit (add/remove) items
        /// </summary>
        public bool IsMediaEditMode
        {
            get => _isMediaEditMode;
            set
            {
                SetPropertyChanged(ref _isMediaEditMode, value);
                SetPropertyChanged(nameof(AddControlText));
                SetPropertyChanged(nameof(AddControlImage));
                SetPropertyChanged(nameof(AddControlOutlined));
            }
        }

        public string AddControlText => (IsSingleViewMode ? AppResource.EditMedia : AppResource.AddMoreMedia);
        public ImageSource AddControlImage => (IsSingleViewMode ? "icon_edit" : "icon_plus_pink");
        public bool AddControlOutlined => IsSingleViewMode;
    }

    /// <summary>
    /// This class represents the data model (the entire grid of media) for the MyMediaView
    /// </summary>
    public class GalleryModel : ObservableCollection<GalleryRowModel>
    {
        public Action<GalleryModel, IList<Media>> MediaAdded;
        public Action<GalleryModel, IList<Media>> MediaRemoved;
        public Action<GalleryModel, GalleryCellModel> CellClicked;
        public Action<GalleryModel, GalleryCellModel> CellDelete;

        /// <summary>
        /// Constructor -- initialize the model
        /// </summary>
        /// <param name="source">The current items of media to show on the MyMediaView</param>
        public GalleryModel(ObservableCollection<Media> source)
        {
            AddItems(source);

            source.CollectionChanged += (sender, args) =>
            {
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                {
                    try
                    {
                        if (args.Action == NotifyCollectionChangedAction.Reset)
                        {
                            Clear();
                        }

                        if (args.OldItems != null && args.Action == NotifyCollectionChangedAction.Remove)
                        {
                            List<Media> oldItems = new List<Media>();
                            foreach (var item in args.OldItems)
                            {
                                oldItems.Add((Media)item);
                            }
                            RemoveCell(oldItems);
                            MediaRemoved?.Invoke(this, oldItems);
                        }

                        if (args.NewItems != null && args.Action == NotifyCollectionChangedAction.Add)
                        {
                            List<Media> newItems = new List<Media>();
                            foreach (var item in args.NewItems)
                            {
                                newItems.Add((Media)item);
                            }
                            GalleryRowModel currentModel = null;
                            if (Count > 0 && Items[Count - 1].Cell3 == null)
                            {
                                currentModel = Items[Count - 1];
                            }
                            AddItems(newItems, currentModel);
                            MediaAdded?.Invoke(this, newItems);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("Exeption in CollectionChanged : " + e.Message);
                    }
                });
            };
        }

        /// <summary>
        /// Add media items to the model
        /// </summary>
        /// <param name="source">The new media items to be added to</param>
        /// <param name="currentModel">the current row model on which the first item from source will be added to</param>
        private void AddItems(IList<Media> source, GalleryRowModel currentModel = null)
        {
            int i = 0;
            GalleryRowModel model = currentModel;
            while (i < source.Count)
            {
                if (model == null)
                {
                    model = new GalleryRowModel(this);
                }

                if (i < source.Count && model.Cell1 == null)
                {
                    model.Cell1 = new GalleryCellModel(model, source[i]);
                    model.Cell1.CellClickedAction += OnCellClickedAction;
                    model.Cell1.DeleteCommand = new Command(HandleAction);
                    i++;
                }
                if (i < source.Count && model.Cell2 == null)
                {
                    model.Cell2 = new GalleryCellModel(model, source[i]);
                    model.Cell2.CellClickedAction += OnCellClickedAction;
                    model.Cell2.DeleteCommand = new Command(HandleAction);
                    i++;
                }
                if (i < source.Count && model.Cell3 == null)
                {
                    model.Cell3 = new GalleryCellModel(model, source[i]);
                    model.Cell3.CellClickedAction += OnCellClickedAction;
                    model.Cell3.DeleteCommand = new Command(HandleAction);
                    i++;
                }
                if (!Contains(model))
                {
                    Add(model);
                }
                model = null;
            }
        }

        /// <summary>
        /// Fires the CellClicked event when a cell is clicked
        /// </summary>
        /// <param name="cell">The cell clicked on</param>
        private void OnCellClickedAction(GalleryCellModel cell)
        {
            CellClicked?.Invoke(this, cell);
        }

        void HandleAction(object obj)
        {
            CellDelete?.Invoke(this, (obj as GalleryCellModel));
        }


        /// <summary>
        /// Finds and returns the index of a cell in the grid
        /// </summary>
        /// <param name="cell">The cell for which the index to be found and return</param>
        /// <returns>If the cell is found, returns its index in the grid; otherwise returns -1</returns>
        public int IndexOfCell(GalleryCellModel cell)
        {
            int index = -1;
            for (int i = 0; i < Count; i++)
            {
                int cellIndex = Items[i].IndexOfCell(cell);
                if (cellIndex >= 0)
                {
                    index = i * 3 + cellIndex;
                }
            }
            return index;
        }

        /// <summary>
        /// Gets the number of selected media items
        /// </summary>
        public int SelectionCount
        {
            get
            {
                int count = 0;
                foreach (GalleryRowModel row in Items)
                {
                    count += row.SelectionCount;
                }
                return count;
            }
        }

        /// <summary>
        /// Returns the number of selected media cells
        /// </summary>
        /// <returns>the list of cells selected currently</returns>
        public List<GalleryCellModel> GetSelectedCells()
        {
            List<GalleryCellModel> items = new List<GalleryCellModel>();
            foreach (GalleryRowModel row in Items)
            {
                items.AddRange(row.GetSelectedCells());
            }
            return items;
        }

        /// <summary>
        /// Clears the current selection
        /// </summary>
        public void ClearSelection()
        {
            foreach (GalleryRowModel row in Items)
            {
                row.ClearSelection();
            }
        }

        /// <summary>
        /// Removes cells from the grid
        /// </summary>
        /// <param name="medias">The items to be removed</param>
        public void RemoveCell(IList<Media> medias)
        {
            foreach (Media media in medias)
            {
                GalleryCellModel cell = null;
                for (int i = 0; i < Count; i++)
                {
                    cell = Items[i].MapMediaToCell(media);
                    if (cell != null)
                    {
                        break;
                    }
                }
                if (cell != null)
                {
                    RemoveCell(cell);
                }
            }
        }

        /// <summary>
        /// Removes a single cell from the grid
        /// </summary>
        /// <param name="cell"></param>
        public void RemoveCell(GalleryCellModel cell)
        {
            for (int i = 0; i < Count; i++)
            {
                if (Items[i].RemoveCell(cell, i < Count - 1 ? Items[i + 1] : null))
                {
                    for (int j = i + 1; j < Count; j++)
                    {
                        Items[j].ShiftCell(j < Count - 1 ? Items[j + 1] : null);
                    }

                    break;
                }
            }

            if (Count > 0 && Items[Count - 1].Count == 0)
            {
                RemoveAt(Count - 1);
            }
        }
    }


    /// <summary>
    /// This class is the model of a row of media items in the GalleryModel
    /// </summary>
    /// <remarks>There can be 3 cells in row</remarks>
    public class GalleryRowModel : ModelItemBase
    {
        /// <summary>
        /// Constructor -- takes the parent GalleryModel to construct
        /// </summary>
        /// <param name="model">the parent grid model of this row model</param>
        public GalleryRowModel(GalleryModel model)
        {
            GalleryModel = model;
        }

        /// <summary>
        /// Gets the parent GalleryModel to this model
        /// </summary>
        public GalleryModel GalleryModel { get; private set; }

        /// <summary>
        /// Finds and maps the GalleryCellModel for a media
        /// </summary>
        /// <param name="media">The media item to be found and mapped to the cell model</param>
        /// <returns></returns>
        public GalleryCellModel MapMediaToCell(Media media)
        {
            if (Cell1 != null && Cell1.Media == media) return Cell1;
            if (Cell2 != null && Cell2.Media == media) return Cell2;
            if (Cell3 != null && Cell3.Media == media) return Cell3;
            return null;
        }

        private GalleryCellModel _cell1;

        /// <summary>
        /// Gets/Sets the first cell in this row model
        /// </summary>
        public GalleryCellModel Cell1
        {
            get { return _cell1; }
            set => SetPropertyChanged(ref _cell1, value);
        }

        private GalleryCellModel _cell2;

        /// <summary>
        /// Gets/Sets the second cell in this row model
        /// </summary>
        public GalleryCellModel Cell2
        {
            get { return _cell2; }
            set => SetPropertyChanged(ref _cell2, value);
        }

        private GalleryCellModel _cell3;
        /// <summary>
        /// Gets/Sets the third cell in this row model
        /// </summary>
        public GalleryCellModel Cell3
        {
            get { return _cell3; }
            set => SetPropertyChanged(ref _cell3, value);
        }


        /// <summary>
        /// Gets the number of cells set in this row model
        /// </summary>
        public int Count
        {
            get
            {
                int count = 0;
                if (Cell1 != null) count++;
                if (Cell2 != null) count++;
                if (Cell3 != null) count++;
                return count;
            }
        }

        /// <summary>
        /// Removes a cell from the row and shits items to left
        /// </summary>
        /// <param name="cell">cell to be removed</param>
        /// <param name="nextRow">The next row model to be shifted to</param>
        /// <returns></returns>
        public bool RemoveCell(GalleryCellModel cell, GalleryRowModel nextRow)
        {
            if (Cell1 == cell)
            {
                Cell1 = Cell2;
                if (Cell2 != null)
                {
                    Cell2 = Cell3;
                }
                if (Cell3 != null)
                {
                    Cell3 = nextRow != null ? nextRow.Cell1 : null;
                }
                return true;
            }
            if (Cell2 == cell)
            {
                Cell2 = Cell3;
                if (Cell3 != null)
                {
                    Cell3 = nextRow != null ? nextRow.Cell1 : null;
                }
                return true;
            }
            if (Cell3 == cell)
            {
                Cell3 = nextRow != null ? nextRow.Cell1 : null;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Shifts cells to next row
        /// </summary>
        /// <param name="nextRow">next row model to shift to</param>
        public void ShiftCell(GalleryRowModel nextRow)
        {
            if (Cell1 != null)
            {
                Cell1 = Cell2;
            }
            if (Cell2 != null)
            {
                Cell2 = Cell3;
            }
            if (Cell3 != null)
            {
                Cell3 = nextRow != null ? nextRow.Cell1 : null;
            }
        }

        /// <summary>
        /// Gets the number of selected media in this row model
        /// </summary>
        public int SelectionCount
        {
            get
            {
                int count = 0;
                if (Cell1 != null && Cell1.IsSelected) count++;
                if (Cell2 != null && Cell2.IsSelected) count++;
                if (Cell3 != null && Cell3.IsSelected) count++;
                return count;
            }
        }

        /// <summary>
        /// Gets the selected cells in this row
        /// </summary>
        /// <returns>a list of selected cells</returns>
        public List<GalleryCellModel> GetSelectedCells()
        {
            List<GalleryCellModel> items = new List<GalleryCellModel>();
            if (Cell1 != null && Cell1.IsSelected) items.Add(Cell1);
            if (Cell2 != null && Cell2.IsSelected) items.Add(Cell2);
            if (Cell3 != null && Cell3.IsSelected) items.Add(Cell3);
            return items;
        }

        /// <summary>
        /// Clears the current selection
        /// </summary>
        public void ClearSelection()
        {
            if (Cell1 != null && Cell1.IsSelected) Cell1.IsSelected = false;
            if (Cell2 != null && Cell2.IsSelected) Cell2.IsSelected = false;
            if (Cell3 != null && Cell3.IsSelected) Cell3.IsSelected = false;
        }

        /// <summary>
        /// Resets the model
        /// </summary>
        public void Reset()
        {
            Cell1 = null;
            Cell2 = null;
            Cell3 = null;
        }

        /// <summary>
        /// Finds and returns the index of a cell in the grid
        /// </summary>
        /// <param name="cell">The cell for which the index to be found and return</param>
        /// <returns>If the cell is found, returns its index in the grid; otherwise returns -1</returns>
        public int IndexOfCell(GalleryCellModel cell)
        {
            if (Cell1 == cell)
            {
                return 0;
            }
            else if (Cell2 == cell)
            {
                return 1;
            }
            else if (Cell3 == cell)
            {
                return 2;
            }
            else
            {
                return -1;
            }
        }
    }

    /// <summary>
    /// This class is the model of a media items in the GalleryRowModel
    /// </summary>
    public class GalleryCellModel : ModelItemBase
    {
        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="rowModel">The parent row model</param>
        /// <param name="media">The current media to set on this cell model</param>
        public GalleryCellModel(GalleryRowModel rowModel, Media media)
        {
            RowModel = rowModel;
            Media = media;
            CellClicked = new Command(() =>
            {
                IsSelected = !IsSelected;
                CellClickedAction?.Invoke(this);
            });
        }

        /// <summary>
        /// Gets the parent row model
        /// </summary>
        public GalleryRowModel RowModel { get; private set; }

        /// <summary>
        /// Gets the media set on this model
        /// </summary>
        public Media Media { get; private set; }

        /// <summary>
        /// The event that fires when this cell is clicked
        /// </summary>
        public event Action<GalleryCellModel> CellClickedAction;

        private bool _isVideo = false;
        /// <summary>
        /// Gets/Sets whether this media is a video or not
        /// </summary>
        public bool IsVideo
        {
            get { return _isVideo; }
            set => SetPropertyChanged(ref _isVideo, value);
        }

        /// <summary>
        /// The Image source if the media is an image
        /// </summary>
        public ImageSource Picture
        {
            get
            {
                if (Media != null)
                {
                    return Media.Image;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Commands to binds in view to gets executed and this view-cell is clicked
        /// </summary>
        public ICommand CellClicked { get; set; }

        public ICommand DeleteCommand { get; set; }
    }
}
