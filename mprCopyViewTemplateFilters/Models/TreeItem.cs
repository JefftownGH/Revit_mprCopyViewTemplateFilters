namespace mprCopyViewTemplateFilters.Models
{
    using System.Windows;
    using ModPlusAPI.Mvvm;

    /// <summary>
    /// Базовый элемент дерева
    /// </summary>
    public abstract class TreeItem : VmBase
    {
        private Visibility _visibility;
        private bool _isChecked;

        /// <summary>
        /// Видимость элемента в дереве
        /// </summary>
        public Visibility Visibility
        {
            get => _visibility;
            set
            {
                if (_visibility == value)
                    return;
                _visibility = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Элемент отмечен
        /// </summary>
        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                if (_isChecked == value)
                    return;
                _isChecked = value;
                OnPropertyChanged();
            }
        }
    }
}
