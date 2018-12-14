using System;
using System.Collections.Generic;
using BabyationApp.Common;
using Xamarin.Forms;
using System.Windows.Input;
using BabyationApp.Managers;
using BabyationApp.Resources;

namespace BabyationApp.Controls.Views
{
    public delegate void CloseEventHandler(bool save, string text);

    public partial class NotepadView : ContentView
    {
        public NotepadViewModel ViewModel { get; set; }
        public event CloseEventHandler OnCloseNotepad;

        static readonly BindableProperty MaxLengthProperty = BindableProperty.Create(nameof(MaxLength), typeof(int), typeof(NotepadView), int.MaxValue);

        public int MaxLength
        {
            get => (int)GetValue(MaxLengthProperty);
            set
            {
                SetValue(MaxLengthProperty, value);
                if( null != ViewModel )
                {
                    ViewModel.MaxChars = value;
                }
            }
        }

        static readonly BindableProperty NoteTextProperty = BindableProperty.Create(nameof(NoteText), typeof(string), typeof(NotepadView), null);

        public string NoteText
        {
            get => (string)GetValue(NoteTextProperty);
            set
            {
                SetValue(NoteTextProperty, value);
                if (null != ViewModel)
                {
                    ViewModel.NoteText = value;
                }
            }
        }


        public NotepadView()
        {
            InitializeComponent();

            UpdateParams();
        }

        public void UpdateParams()
        {
            ViewModel = new NotepadViewModel(CloseNote, SaveNote);
            BindingContext = ViewModel;
            ViewModel.MaxChars = MaxLength;
        }

        private void CloseNote()
        {
            OnCloseNotepad?.Invoke(false, null);
        }

        private void SaveNote(string text)
        {
            OnCloseNotepad?.Invoke(true, text);
        }
    }

    public class NotepadViewModel : ObservableObject
    {
        private Action CloseNoteAction { get; set; }
        private Action<string> SaveNoteAction { get; set; }

        public NotepadViewModel(Action closeNoteAction, Action<string> saveNoteAction)
        {
            CloseNoteAction = closeNoteAction;
            SaveNoteAction = saveNoteAction;
            MaxChars = int.MaxValue;
        }

        #region Public UI properties

        private int _maxChars;
        public int MaxChars
        { 
            get => _maxChars;
            set => SetPropertyChanged(ref _maxChars, value);
        }

        private string _noteText;
        public string NoteText 
        {
            get => _noteText; 
            set
            {
                SetPropertyChanged(ref _noteText, value);
                SetPropertyChanged(nameof(CharsLeft));
            }
        }

        public string CharsLeft
        {
            get
            {
                int textLen = NoteText?.Length ?? 0;

                return int.MaxValue == MaxChars ? "" : String.Format("({0} {1})", (MaxChars - textLen) , AppResource.CharactersLeft);
            }
        }

        #endregion


        #region Commands

        private ICommand _closeCommand;
        public ICommand CloseCommand
        {
            get
            {
                _closeCommand = _closeCommand ?? new Command(() =>
                {
                    CloseNoteAction?.Invoke();
                });
                return _closeCommand;
            }
        }

        private ICommand _saveCommand;
        public ICommand SaveCommand
        {
            get
            {
                _saveCommand = _saveCommand ?? new Command(() =>
                {
                    SaveNoteAction?.Invoke(NoteText);
                });
                return _saveCommand;
            }
        }

        #endregion
    }
}