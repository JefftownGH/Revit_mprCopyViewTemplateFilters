namespace mprCopyViewTemplateFilters.Models
{
    using System.Windows;
    using ModPlusAPI.Mvvm;

    /// <summary>
    /// Базовый элемент дерева
    /// </summary>
    public abstract class TreeItem : VmBase
    {
        private Visibility _visibilityOnLeft;
        private Visibility _visibilityOnRight;
        private bool _isExpandOnLeft;
        private bool _isExpandOnRight;
        private bool _isChecked;

        /// <summary>
        /// Видимость элемента в дереве в левом дереве
        /// </summary>
        public Visibility VisibilityOnLeft
        {
            get => _visibilityOnLeft;
            set
            {
                if (_visibilityOnLeft == value)
                    return;
                _visibilityOnLeft = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Видимость элемента в дереве в правом дереве
        /// </summary>
        public Visibility VisibilityOnRight
        {
            get => _visibilityOnRight;
            set
            {
                if (_visibilityOnRight == value)
                    return;
                _visibilityOnRight = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Развернут ли узел дерева в левом дереве
        /// </summary>
        public bool IsExpandOnLeft
        {
            get => _isExpandOnLeft;
            set
            {
                if (_isExpandOnLeft == value)
                    return;
                _isExpandOnLeft = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Развернут ли узел дерева в правом дереве
        /// </summary>
        public bool IsExpandOnRight
        {
            get => _isExpandOnRight;
            set
            {
                if (_isExpandOnRight == value)
                    return;
                _isExpandOnRight = value;
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
