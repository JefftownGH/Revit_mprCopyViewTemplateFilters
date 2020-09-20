namespace mprCopyViewTemplateFilters
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows.Input;
    using Autodesk.Revit.DB;
    using Models;
    using ModPlusAPI;
    using ModPlusAPI.Mvvm;
    using ModPlusAPI.Windows;

    /// <summary>
    /// Главный контекст
    /// </summary>
    public class MainContext : VmBase
    {
        private readonly ViewType[] _allowedViewTypes = new[]
        {
            ViewType.ThreeD, ViewType.Walkthrough, ViewType.CeilingPlan, ViewType.Elevation, ViewType.Section,
            ViewType.Detail, ViewType.FloorPlan, ViewType.EngineeringPlan, ViewType.AreaPlan, ViewType.Rendering,
            ViewType.DraftingView
        };

        private readonly Document _doc;
        private string _searchStringOnLeft = string.Empty;
        private string _searchStringOnRight;
        private ViewTypeGroup _viewTypeGroupOnLeft;
        private ViewTypeGroup _viewTypeGroupOnRight;
        private SourceCollectionType _sourceCollectionTypeOnLeft;
        private SourceCollectionType _sourceCollectionTypeOnRight;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainContext"/> class.
        /// </summary>
        /// <param name="doc">Текущий документ</param>
        public MainContext(Document doc)
        {
            _doc = doc;
            ViewTemplates = new ObservableCollection<ViewWrapper>();
            Views = new ObservableCollection<ViewWrapper>();
        }

        /// <summary>
        /// Шаблоны видов
        /// </summary>
        public ObservableCollection<ViewWrapper> ViewTemplates { get; }

        /// <summary>
        /// Обычные виды
        /// </summary>
        public ObservableCollection<ViewWrapper> Views { get; }

        /// <summary>
        /// Коллекция в левом дереве
        /// </summary>
        public ObservableCollection<ViewWrapper> LeftTreeCollection =>
            SourceCollectionTypeOnLeft == SourceCollectionType.Templates ? ViewTemplates : Views;

        /// <summary>
        /// Коллекция в правом дереве
        /// </summary>
        public ObservableCollection<ViewWrapper> RightTreeCollection =>
            SourceCollectionTypeOnRight == SourceCollectionType.Templates ? ViewTemplates : Views;

        /// <summary>
        /// Тип коллекции источника слева
        /// </summary>
        public SourceCollectionType SourceCollectionTypeOnLeft
        {
            get => _sourceCollectionTypeOnLeft;
            set
            {
                if (_sourceCollectionTypeOnLeft == value)
                    return;
                _sourceCollectionTypeOnLeft = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(LeftTreeCollection));
                Search(true);
            }
        }

        /// <summary>
        /// Тип коллекции-источника справа
        /// </summary>
        public SourceCollectionType SourceCollectionTypeOnRight
        {
            get => _sourceCollectionTypeOnRight;
            set
            {
                if (_sourceCollectionTypeOnRight == value)
                    return;
                _sourceCollectionTypeOnRight = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(RightTreeCollection));
                OnPropertyChanged(nameof(RightSideHeader));
                Search(false);
            }
        }

        /// <summary>
        /// Строка для поиска в левом дереве
        /// </summary>
        public string SearchStringOnLeft
        {
            get => _searchStringOnLeft;
            set
            {
                if (_searchStringOnLeft == value)
                    return;
                _searchStringOnLeft = value;
                OnPropertyChanged();
                Search(true);
            }
        }

        /// <summary>
        /// Строка для поиска в правом дереве
        /// </summary>
        public string SearchStringOnRight
        {
            get => _searchStringOnRight;
            set
            {
                if (_searchStringOnRight == value)
                    return;
                _searchStringOnRight = value;
                OnPropertyChanged();
                Search(false);
            }
        }

        /// <summary>
        /// Группа по типу вида слева
        /// </summary>
        public ViewTypeGroup ViewTypeGroupOnLeft
        {
            get => _viewTypeGroupOnLeft;
            set
            {
                if (_viewTypeGroupOnLeft == value)
                    return;
                _viewTypeGroupOnLeft = value;
                OnPropertyChanged();
                Search(true);
            }
        }

        /// <summary>
        /// Группа по типу вида справа
        /// </summary>
        public ViewTypeGroup ViewTypeGroupOnRight
        {
            get => _viewTypeGroupOnRight;
            set
            {
                if (_viewTypeGroupOnRight == value)
                    return;
                _viewTypeGroupOnRight = value;
                OnPropertyChanged();
                Search(false);
            }
        }

        /// <summary>
        /// Заголовок правой зоны
        /// </summary>
        public string RightSideHeader => SourceCollectionTypeOnRight == SourceCollectionType.Templates
            ? Language.GetItem(new ModPlusConnector().Name, "h2")
            : Language.GetItem(new ModPlusConnector().Name, "h16");

        /// <summary>
        /// Команда "Применить"
        /// </summary>
        public ICommand ApplyCommand => new RelayCommandWithoutParameter(Apply);

        /// <summary>
        /// Команда "Копировать отмеченные"
        /// </summary>
        public ICommand CopyCheckedCommand => new RelayCommandWithoutParameter(CopyChecked);

        /// <summary>
        /// Свернуть все узлы в левом дереве
        /// </summary>
        public ICommand CollapseAllOnLeftCommand => new RelayCommandWithoutParameter(() =>
        {
            foreach (var template in LeftTreeCollection)
            {
                template.IsExpandOnLeft = false;
            }
        });

        /// <summary>
        /// Развернуть все узлы в левом дереве
        /// </summary>
        public ICommand ExpandAllOnLeftCommand => new RelayCommandWithoutParameter(() =>
        {
            foreach (var template in LeftTreeCollection)
            {
                template.IsExpandOnLeft = true;
            }
        });

        /// <summary>
        /// Свернуть все узлы в правом дереве
        /// </summary>
        public ICommand CollapseAllOnRightCommand => new RelayCommandWithoutParameter(() =>
        {
            foreach (var viewWrapper in RightTreeCollection)
            {
                viewWrapper.IsExpandOnRight = false;
            }
        });

        /// <summary>
        /// Развернуть все узлы в правом дереве
        /// </summary>
        public ICommand ExpandAllOnRightCommand => new RelayCommandWithoutParameter(() =>
        {
            foreach (var viewWrapper in RightTreeCollection)
            {
                viewWrapper.IsExpandOnRight = true;
            }
        });

        /// <summary>
        /// Отметить все виды/шаблоны в правом дереве
        /// </summary>
        public ICommand CheckAllOnRightCommand => new RelayCommandWithoutParameter(() =>
        {
            foreach (var viewWrapper in RightTreeCollection)
            {
                viewWrapper.IsChecked = true;
            }
        });

        /// <summary>
        /// Снять отметку со всех видов/шаблонов в правом дереве
        /// </summary>
        public ICommand UncheckAllOnRightCommand => new RelayCommandWithoutParameter(() =>
        {
            foreach (var viewWrapper in RightTreeCollection)
            {
                viewWrapper.IsChecked = false;
            }
        });

        /// <summary>
        /// Инициализация (чтение шаблонов и видов)
        /// </summary>
        public void Init()
        {
            ViewTemplates.Clear();

            var viewTemplates = new FilteredElementCollector(_doc)
                .OfClass(typeof(View))
                .Cast<View>()
                .Where(v => v.IsTemplate && 
                            v.AreGraphicsOverridesAllowed() &&
                            _allowedViewTypes.Contains(v.ViewType))
                .OrderBy(v => v.Name)
                .ToList();

            foreach (var view in viewTemplates)
            {
                try
                {
                    var viewWrapper = new ViewWrapper(view);
                    ViewTemplates.Add(viewWrapper);
                }
                catch (Exception exception)
                {
                    ExceptionBox.Show(exception);
                }
            }

            Views.Clear();

            var views = new FilteredElementCollector(_doc)
                    .OfClass(typeof(View))
                    .Cast<View>()
                    .Where(v => !v.IsTemplate &&
                                v.ViewTemplateId == ElementId.InvalidElementId &&
                                v.AreGraphicsOverridesAllowed() &&
                                _allowedViewTypes.Contains(v.ViewType))
                    .OrderBy(v => v.Name)
                    .ToList();

            foreach (var view in views)
            {
                try
                {
                    var viewWrapper = new ViewWrapper(view);
                    Views.Add(viewWrapper);
                }
                catch (Exception exception)
                {
                    ExceptionBox.Show(exception);
                }
            }
        }

        private void Search(bool isOnLeft)
        {
            var searchString = string.IsNullOrWhiteSpace(SearchStringOnLeft)
                ? string.Empty
                : SearchStringOnLeft.Trim().ToUpper();

            if (isOnLeft)
            {
                foreach (var viewWrapper in LeftTreeCollection)
                {
                    viewWrapper.ChangeVisibilityByFilteringAndSearching(searchString, ViewTypeGroupOnLeft, true);
                }
            }
            else
            {
                foreach (var viewWrapper in RightTreeCollection)
                {
                    viewWrapper.ChangeVisibilityByFilteringAndSearching(searchString, ViewTypeGroupOnLeft, false);
                }
            }
        }

        private void CopyChecked()
        {
            var filters = LeftTreeCollection.SelectMany(t => t.Filters.Where(f => f.IsChecked)).ToList();
            foreach (var viewTemplateWrapper in RightTreeCollection.Where(t => t.IsChecked))
            {
                foreach (var filter in filters.Where(filter => !viewTemplateWrapper.ContainsFilter(filter)))
                {
                    viewTemplateWrapper.AddFilter(new FilterWrapper(viewTemplateWrapper, filter), FilterStatus.New);
                }
            }
        }

        private void Apply()
        {
            try
            {
                var transactionName = Language.GetItem(ModPlusConnector.Instance.Name, "h9");
                if (string.IsNullOrEmpty(transactionName))
                    transactionName = "Copying View Template Filters";
                using (var tr = new Transaction(_doc, transactionName))
                {
                    tr.Start();

                    foreach (var filter in RightTreeCollection.SelectMany(v =>
                        v.Filters.Where(f => f.FilterStatus != FilterStatus.Exits)))
                    {
                        if (filter.FilterStatus == FilterStatus.New)
                        {
                            var graphicSettings =
                                filter.OriginalParent.View.GetFilterOverrides(filter.FilterId);
                            var filterVisibility =
                                filter.OriginalParent.View.GetFilterVisibility(filter.FilterId);
                            filter.Parent.View.AddFilter(filter.FilterId);
                            filter.Parent.View.SetFilterOverrides(filter.FilterId, graphicSettings);
                            filter.Parent.View.SetFilterVisibility(filter.FilterId, filterVisibility);
                        }
                        else if (filter.FilterStatus == FilterStatus.Remove)
                        {
                            filter.Parent.View.RemoveFilter(filter.FilterId);
                        }
                    }

                    tr.Commit();
                }
            }
            catch (Exception exception)
            {
                ExceptionBox.Show(exception);
            }

            Init();
        }
    }
}
