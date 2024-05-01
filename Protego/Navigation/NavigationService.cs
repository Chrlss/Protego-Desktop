using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Protego.Navigation
{
    class NavigationService
    {
        private readonly Dictionary<string, Page> _pages = new Dictionary<string, Page>();
        private Frame _frame;

        public void Initialize(Frame frame)
        {
            _frame = frame ?? throw new ArgumentNullException(nameof(frame));
        }

        public void NavigateTo(string pageKey)
        {
            if (_pages.ContainsKey(pageKey))
            {
                _frame.Navigate(_pages[pageKey]);
            }
        }

        public void Configure(string pageKey, Page page)
        {
            if (_pages.ContainsKey(pageKey))
            {
                _pages[pageKey] = page;
            }
            else
            {
                _pages.Add(pageKey, page);
            }
        }
    }
}
