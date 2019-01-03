using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace BabyationApp.Behaviors
{
    //Source: https://xamarinhelp.com/masked-entry-in-xamarin-forms/
    //
    public class MaskedBehavior : Behavior<Entry>
    {
        private string _mask = "";
        public string Mask
        {
            get => _mask;
            set
            {
                _mask = value;
                SetPositions();
            }
        }

        protected override void OnAttachedTo(Entry bindable)
        {
            bindable.TextChanged += OnEntryTextChanged;
            base.OnAttachedTo(bindable);
        }

        protected override void OnDetachingFrom(Entry bindable)
        {
            bindable.TextChanged -= OnEntryTextChanged;
            base.OnDetachingFrom(bindable);
        }

        IDictionary<int, char> _positions;

        void SetPositions()
        {
            if (string.IsNullOrEmpty(Mask))
            {
                _positions = null;
                return;
            }

            var list = new Dictionary<int, char>();
            for (var i = 0; i < Mask.Length; i++)
                if (Mask[i] != 'X')
                    list.Add(i, Mask[i]);

            _positions = list;
        }

        private void OnEntryTextChanged(object sender, TextChangedEventArgs args)
        {
            var entry = sender as Entry;

            string text = entry.Text;

            if (text.Contains(".") && text.Length == 2)
            {
                if (double.TryParse(text, out double x))
                {
                    text = string.Format($"{x:00.0}");
                }
            }

            if (string.IsNullOrWhiteSpace(text) || _positions == null)
                return;

            if (text.Length > _mask.Length)
            {
                entry.Text = text.Remove(text.Length - 1);
                return;
            }

            foreach (var position in _positions)
                if (text.Length >= position.Key + 1)
                {
                    var value = position.Value.ToString();
                    if (text.Substring(position.Key, 1) != value)
                        text = text.Insert(position.Key, value);
                }

            if (entry.Text != text)
                entry.Text = text;
        }
    }

    public class MaskedDoubleAmountNoLeadingZeroBehavior : Behavior<Entry>
    {
        private string _mask = "";
        public string Mask
        {
            get => _mask;
            set
            {
                _mask = value;
                SetPositions();
            }
        }

        protected override void OnAttachedTo(Entry bindable)
        {
            bindable.TextChanged += OnEntryTextChanged;
            base.OnAttachedTo(bindable);
        }

        protected override void OnDetachingFrom(Entry bindable)
        {
            bindable.TextChanged -= OnEntryTextChanged;
            base.OnDetachingFrom(bindable);
        }

        IDictionary<int, char> _positions;

        void SetPositions()
        {
            if (string.IsNullOrEmpty(Mask))
            {
                _positions = null;
                return;
            }

            var list = new Dictionary<int, char>();
            for (var i = 0; i < Mask.Length; i++)
                if (Mask[i] != 'X')
                    list.Add(i, Mask[i]);

            _positions = list;
        }

        private void OnEntryTextChanged(object sender, TextChangedEventArgs args)
        {
            var entry = sender as Entry;

            string text = entry.Text;

            if (string.IsNullOrWhiteSpace(text) || _positions == null)
            {
                text = "";
                entry.Text = text;
                return;
            }

            if (!string.IsNullOrEmpty(text))
            {
                if (text.Length == 2 && text.Last() == '.')
                {
                    text = string.Format(" {0}", text);
                }
                else if (text.Length < 3 && !char.IsNumber(text.Last()))
                {
                    text = text.Remove(text.Length - 1);
                    entry.Text = text;
                    return;
                }
            }

            if (text.Length > _mask.Length)
            {
                entry.Text = text.Remove(text.Length - 1);
                return;
            }

            foreach (var position in _positions)
                if (text.Length >= position.Key + 1)
                {
                    string maskPositionvalue = position.Value.ToString();

                    if (text.Substring(position.Key, 1) != maskPositionvalue)
                    {
                        if (text.Contains(maskPositionvalue))
                        {
                            text = text.Insert(position.Key - 1, "0");
                        }
                        else
                        {
                            text = text.Insert(position.Key, maskPositionvalue);
                        }
                    }

                }

            if (!string.IsNullOrEmpty(text) && text.Length >= 2 && text.First() == '0')
            {
                text = string.Format(" {0}", text.Substring(1));
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                text = "";
            }

            if (entry.Text != text)
                entry.Text = text;
        }
    }

    public class MaskedTimeNoLeadingZeroBehavior : Behavior<Entry>
    {
        private string _mask = "";
        public string Mask
        {
            get => _mask;
            set
            {
                _mask = value;
                SetPositions();
            }
        }

        protected override void OnAttachedTo(Entry bindable)
        {
            bindable.TextChanged += OnEntryTextChanged;
            base.OnAttachedTo(bindable);
        }

        protected override void OnDetachingFrom(Entry bindable)
        {
            bindable.TextChanged -= OnEntryTextChanged;
            base.OnDetachingFrom(bindable);
        }

        IDictionary<int, char> _positions;

        void SetPositions()
        {
            if (string.IsNullOrEmpty(Mask))
            {
                _positions = null;
                return;
            }

            var list = new Dictionary<int, char>();
            for (var i = 0; i < Mask.Length; i++)
                if (Mask[i] != 'X')
                    list.Add(i, Mask[i]);

            _positions = list;
        }

        private void OnEntryTextChanged(object sender, TextChangedEventArgs args)
        {
            var entry = sender as Entry;

            string text = entry.Text;

            if (string.IsNullOrWhiteSpace(text) || _positions == null)
            {
                text = "";
                entry.Text = text;
                return;
            }

            if (!string.IsNullOrEmpty(text) && !char.IsNumber(text.Last()))
            {
                text = text.Remove(text.Length - 1);
                entry.Text = text;
                return;
            }

            if (text.Length > _mask.Length)
            {
                entry.Text = text.Remove(text.Length - 1);
                return;
            }

            foreach (var position in _positions)
                if (text.Length >= position.Key + 1)
                {
                    string maskPositionvalue = position.Value.ToString();

                    if (text.Substring(position.Key, 1) != maskPositionvalue)
                    {
                        if (text.Contains(maskPositionvalue))
                        {
                            text = text.Insert(position.Key - 1, "0");
                        }
                        else
                        {
                            text = text.Insert(position.Key, maskPositionvalue);
                        }
                    }

                }

            if (!string.IsNullOrEmpty(text) && text.Length >= 2 && text.First() == '0')
            {
                text = string.Format(" {0}", text.Substring(1));
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                text = "";
            }

            if (entry.Text != text)
                entry.Text = text;
        }
    }
}
