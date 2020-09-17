namespace mprCopyViewTemplateFilters.Models
{
    using System.Windows.Input;
    using Autodesk.Revit.DB;
    using ModPlusAPI.Mvvm;
    using Visibility = System.Windows.Visibility;

    /// <summary>
    /// Обертка для фильтра
    /// </summary>
    public class FilterWrapper : TreeItem
    {
        private FilterStatus _filterStatus = FilterStatus.Exits;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterWrapper"/> class.
        /// </summary>
        /// <param name="parent">Родительский шаблон</param>
        /// <param name="filterId">Идентификатор фильтра</param>
        public FilterWrapper(ViewWrapper parent, ElementId filterId)
        {
            OriginalParent = parent;
            Parent = parent;
            FilterId = filterId;
            Name = parent.View.Document.GetElement(filterId).Name;
            NameUpperCase = Name.ToUpper();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterWrapper"/> class.
        /// </summary>
        /// <param name="parent">Родительский шаблон</param>
        /// <param name="filter">Фильтр-исходник</param>
        public FilterWrapper(ViewWrapper parent, FilterWrapper filter)
        {
            FilterId = filter.FilterId;
            Name = filter.Name;
            NameUpperCase = filter.NameUpperCase;
            OriginalParent = filter.OriginalParent;
            Parent = parent;
        }

        /// <summary>
        /// Исходный родительский шаблон
        /// </summary>
        public ViewWrapper OriginalParent { get; }

        /// <summary>
        /// Родительский шаблон
        /// </summary>
        public ViewWrapper Parent { get; set; }

        /// <summary>
        /// Идентификатор фильтра
        /// </summary>
        public ElementId FilterId { get; }

        /// <summary>
        /// Название фильтра
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Название фильтра в верхнем регистре
        /// </summary>
        public string NameUpperCase { get; }

        /// <summary>
        /// Статус фильтра
        /// </summary>
        public FilterStatus FilterStatus
        {
            get => _filterStatus;
            set
            {
                if (_filterStatus == value)
                    return;
                _filterStatus = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsNewPointerVisibility));
                OnPropertyChanged(nameof(IsRemovePointerVisibility));
                OnPropertyChanged(nameof(IsCheckBoxVisible));
                OnPropertyChanged(nameof(CanRemove));
                OnPropertyChanged(nameof(CanRestore));
            }
        }

        /// <summary>
        /// Видимость указателя "Это новый фильтр"
        /// </summary>
        public Visibility IsNewPointerVisibility =>
            FilterStatus == FilterStatus.New ? Visibility.Visible : Visibility.Collapsed;

        /// <summary>
        /// Видимость указателя "Это удаляемый фильтр"
        /// </summary>
        public Visibility IsRemovePointerVisibility =>
            FilterStatus == FilterStatus.Remove ? Visibility.Visible : Visibility.Collapsed;

        /// <summary>
        /// Видимость чек-бокса
        /// </summary>
        public bool IsCheckBoxVisible => FilterStatus == FilterStatus.Exits;

        /// <summary>
        /// Можно ли удалить фильтр
        /// </summary>
        public bool CanRemove => FilterStatus != FilterStatus.Remove;

        /// <summary>
        /// Можно ли отменить статус "Удаляемый"
        /// </summary>
        public bool CanRestore => FilterStatus == FilterStatus.Remove;

        /// <summary>
        /// Команда "Удалить фильтр"
        /// </summary>
        public ICommand RemoveCommand => new RelayCommandWithoutParameter(() => Parent.RemoveFilter(this));

        /// <summary>
        /// Команда "Восстановить (отменить статус "Удаляемый")"
        /// </summary>
        public ICommand RestoreCommand => new RelayCommandWithoutParameter(() => FilterStatus = FilterStatus.Exits);
    }
}
