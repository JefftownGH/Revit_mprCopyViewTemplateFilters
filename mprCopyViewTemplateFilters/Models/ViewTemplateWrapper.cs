namespace mprCopyViewTemplateFilters.Models
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using Autodesk.Revit.DB;

    /// <summary>
    /// Обертка для шаблона вида
    /// </summary>
    public class ViewTemplateWrapper : TreeItem
    {
        private readonly ObservableCollection<FilterWrapper> _filters;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewTemplateWrapper"/> class.
        /// </summary>
        /// <param name="viewTemplate">Шаблон вида</param>
        public ViewTemplateWrapper(View viewTemplate)
        {
            ViewTemplate = viewTemplate;
            Name = viewTemplate.Name;
            NameUpperCase = Name.ToUpper();

            _filters = new ObservableCollection<FilterWrapper>();
            var filterWrappers = viewTemplate.GetFilters().Select(id => new FilterWrapper(this, id));
            foreach (var filterWrapper in filterWrappers.OrderBy(f => f.Name))
            {
                AddFilter(filterWrapper, FilterStatus.Exits);
            }

            Filters = new ReadOnlyObservableCollection<FilterWrapper>(_filters);
        }

        /// <summary>
        /// Шаблон вида
        /// </summary>
        public View ViewTemplate { get; }

        /// <summary>
        /// Название шаблона
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Название шаблона в верхнем регистре
        /// </summary>
        public string NameUpperCase { get; }

        /// <summary>
        /// Фильтры шаблона вида
        /// </summary>
        public ReadOnlyObservableCollection<FilterWrapper> Filters { get; }

        /// <summary>
        /// Добавление фильтра в коллекцию дочерних
        /// </summary>
        /// <param name="filterWrapper">Фильтр</param>
        /// <param name="filterStatus">Статус добавляемого фильтра</param>
        public void AddFilter(FilterWrapper filterWrapper, FilterStatus filterStatus)
        {
            filterWrapper.FilterStatus = filterStatus;
            _filters.Add(filterWrapper);
        }

        /// <summary>
        /// Удалить или пометить как "Удаляемый" дочерний фильтр 
        /// </summary>
        /// <param name="filter">Фильтр</param>
        public void RemoveFilter(FilterWrapper filter)
        {
            if (filter.FilterStatus == FilterStatus.New)
            {
                _filters.Remove(filter);
            }
            else
            {
                filter.FilterStatus = FilterStatus.Remove;
            }
        }

        /// <summary>
        /// Содержит ли шаблон такой фильтр с проверкой по Id
        /// </summary>
        /// <param name="filter">Фильтр</param>
        public bool ContainsFilter(FilterWrapper filter)
        {
            foreach (var filterWrapper in Filters)
            {
                if (filterWrapper.FilterId.IntegerValue == filter.FilterId.IntegerValue)
                    return true;
            }

            return false;
        }
    }
}
