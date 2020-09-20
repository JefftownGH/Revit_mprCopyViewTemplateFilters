namespace mprCopyViewTemplateFilters.Models
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows.Input;
    using Autodesk.Revit.DB;
    using ModPlusAPI.Mvvm;
    using Visibility = System.Windows.Visibility;

    /// <summary>
    /// Обертка для вида
    /// </summary>
    public class ViewWrapper : TreeItem
    {
        private readonly ObservableCollection<FilterWrapper> _filters;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewWrapper"/> class.
        /// </summary>
        /// <param name="view">Вид</param>
        public ViewWrapper(View view)
        {
            View = view;
            Name = view.Name;
            NameUpperCase = Name.ToUpper();
            IsTemplate = view.IsTemplate;
            ViewTypeGroup = GetViewTypeGroup(view);

            var filters = view.GetFilters().ToList();
            var filterWrappers = new List<FilterWrapper>();
            foreach (var elementId in filters)
            {
                filterWrappers.Add(new FilterWrapper(this, elementId));
            }

            _filters = new ObservableCollection<FilterWrapper>(filterWrappers.OrderBy(w => w.Name));
            Filters = new ReadOnlyObservableCollection<FilterWrapper>(_filters);
        }

        /// <summary>
        /// Вид
        /// </summary>
        public View View { get; }

        /// <summary>
        /// Название шаблона
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Название шаблона в верхнем регистре
        /// </summary>
        public string NameUpperCase { get; }

        /// <summary>
        /// Это шаблон вида
        /// </summary>
        public bool IsTemplate { get; }

        /// <summary>
        /// Группа по типу вида
        /// </summary>
        public ViewTypeGroup ViewTypeGroup { get; }

        /// <summary>
        /// Фильтры шаблона вида
        /// </summary>
        public ReadOnlyObservableCollection<FilterWrapper> Filters { get; }

        /// <summary>
        /// Доступно ли изменения статуса "Отмечен" для фильтров
        /// </summary>
        public bool CanChangeCheckStatusForFilters => Filters.Any();

        /// <summary>
        /// Отметить все фильтры
        /// </summary>
        public ICommand CheckAllFiltersCommand => new RelayCommandWithoutParameter(() =>
        {
            foreach (var filter in Filters)
            {
                filter.IsChecked = true;
            }
        });

        /// <summary>
        /// Снять отметку со всех фильтров
        /// </summary>
        public ICommand UncheckAllFiltersCommand => new RelayCommandWithoutParameter(() =>
        {
            foreach (var filter in Filters)
            {
                filter.IsChecked = false;
            }
        });

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

        /// <summary>
        /// Изменить видимость вида (шаблона) и дочерних фильтров в зависимости от условия фильтрации по группе по типу
        /// вида <see cref="ViewTypeGroup"/> и по поисковой строке
        /// </summary>
        /// <param name="searchStringUpper">Поисковая строка приведенная к верхнему регистру</param>
        /// <param name="viewTypeGroup">Группа по типу вида</param>
        /// <param name="isOnLeft">True - для левого дерева, False - для правого</param>
        public void ChangeVisibilityByFilteringAndSearching(
            string searchStringUpper, ViewTypeGroup viewTypeGroup, bool isOnLeft)
        {
            if (string.IsNullOrEmpty(searchStringUpper))
            {
                if (viewTypeGroup == ViewTypeGroup.All || ViewTypeGroup == viewTypeGroup)
                {
                    foreach (var filter in Filters)
                    {
                        if (isOnLeft)
                            filter.VisibilityOnLeft = Visibility.Visible;
                        else
                            filter.VisibilityOnRight = Visibility.Visible;
                    }

                    if (isOnLeft)
                        VisibilityOnLeft = Visibility.Visible;
                    else
                        VisibilityOnRight = Visibility.Visible;
                }
                else
                {
                    if (isOnLeft)
                        VisibilityOnLeft = Visibility.Collapsed;
                    else
                        VisibilityOnRight = Visibility.Collapsed;
                }
            }
            else
            {
                if (viewTypeGroup != ViewTypeGroup.All && ViewTypeGroup != viewTypeGroup)
                {
                    if (isOnLeft)
                        VisibilityOnLeft = Visibility.Collapsed;
                    else
                        VisibilityOnRight = Visibility.Collapsed;
                    return;
                }

                foreach (var filter in Filters)
                {
                    if (isOnLeft)
                    {
                        filter.VisibilityOnLeft = filter.NameUpperCase.Contains(searchStringUpper)
                           ? Visibility.Visible
                           : Visibility.Collapsed;
                    }
                    else
                    {
                        filter.VisibilityOnRight = filter.NameUpperCase.Contains(searchStringUpper)
                            ? Visibility.Visible
                            : Visibility.Collapsed;
                    }
                }

                if ((isOnLeft && Filters.All(f => f.VisibilityOnLeft == Visibility.Collapsed)) ||
                    (!isOnLeft && Filters.All(f => f.VisibilityOnRight == Visibility.Collapsed)))
                {
                    if (NameUpperCase.Contains(searchStringUpper))
                    {
                        foreach (var filter in Filters)
                        {
                            if (isOnLeft)
                                filter.VisibilityOnLeft = Visibility.Visible;
                            else
                                filter.VisibilityOnRight = Visibility.Visible;
                        }

                        if (isOnLeft)
                            VisibilityOnLeft = Visibility.Visible;
                        else
                            VisibilityOnRight = Visibility.Visible;
                    }
                    else
                    {
                        if (isOnLeft)
                            VisibilityOnLeft = Visibility.Collapsed;
                        else
                            VisibilityOnRight = Visibility.Collapsed;
                    }
                }
                else
                {
                    if (isOnLeft)
                        VisibilityOnLeft = Visibility.Visible;
                    else
                        VisibilityOnRight = Visibility.Visible;
                }
            }
        }

        private static ViewTypeGroup GetViewTypeGroup(View view)
        {
            switch (view.ViewType)
            {
                case ViewType.ThreeD:
                case ViewType.Walkthrough:
                    return ViewTypeGroup.ThreeDWalkthroughs;
                case ViewType.CeilingPlan:
                    return ViewTypeGroup.CeilingPlans;
                case ViewType.Elevation:
                case ViewType.Section:
                case ViewType.Detail:
                    return ViewTypeGroup.ElevationsSectionsDetailViews;
                case ViewType.FloorPlan:
                case ViewType.EngineeringPlan:
                case ViewType.AreaPlan:
                    return ViewTypeGroup.FloorStructuralAreaPlans;
                case ViewType.Rendering:
                case ViewType.DraftingView:
                    return ViewTypeGroup.RenderingDraftingViews;
                default:
                    throw new System.ArgumentException("Cannot resolve view type");
            }
        }
    }
}
