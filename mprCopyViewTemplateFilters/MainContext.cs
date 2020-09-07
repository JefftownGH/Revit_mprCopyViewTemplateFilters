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
    using Visibility = System.Windows.Visibility;

    /// <summary>
    /// Главный контекст
    /// </summary>
    public class MainContext : VmBase
    {
        private readonly Document _doc;
        private string _searchString;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainContext"/> class.
        /// </summary>
        /// <param name="doc">Текущий документ</param>
        public MainContext(Document doc)
        {
            _doc = doc;
            Init();
        }

        /// <summary>
        /// Шаблоны видов
        /// </summary>
        public ObservableCollection<ViewTemplateWrapper> ViewTemplates { get; private set; }

        /// <summary>
        /// Строка для поиска
        /// </summary>
        public string SearchString
        {
            get => _searchString;
            set
            {
                if (_searchString == value)
                    return;
                _searchString = value;
                OnPropertyChanged();
                Search();
            }
        }

        /// <summary>
        /// Команда "Применить"
        /// </summary>
        public ICommand ApplyCommand => new RelayCommandWithoutParameter(Apply);

        /// <summary>
        /// Команда "Копировать отмеченные"
        /// </summary>
        public ICommand CopyCheckedCommand => new RelayCommandWithoutParameter(CopyChecked);

        private void Init()
        {
            var viewTemplates = new FilteredElementCollector(_doc)
                .OfClass(typeof(View))
                .Cast<View>()
                .Where(v => v.IsTemplate && v.AreGraphicsOverridesAllowed())
                .OrderBy(v => v.Name);
            ViewTemplates = new ObservableCollection<ViewTemplateWrapper>(
                viewTemplates.Select(v => new ViewTemplateWrapper(v)));
            OnPropertyChanged(nameof(ViewTemplates));
        }

        private void Search()
        {
            var searchString = SearchString.Trim().ToUpper();
            if (string.IsNullOrEmpty(searchString))
            {
                foreach (var viewTemplate in ViewTemplates)
                {
                    foreach (var filter in viewTemplate.Filters)
                    {
                        filter.Visibility = Visibility.Visible;
                    }

                    viewTemplate.Visibility = Visibility.Visible;
                }
            }
            else
            {
                foreach (var viewTemplate in ViewTemplates)
                {
                    foreach (var filter in viewTemplate.Filters)
                    {
                        filter.Visibility = filter.NameUpperCase.Contains(searchString)
                            ? Visibility.Visible : Visibility.Collapsed;
                    }

                    if (viewTemplate.Filters.All(f => f.Visibility == Visibility.Collapsed))
                    {
                        viewTemplate.Visibility = viewTemplate.NameUpperCase.Contains(searchString)
                            ? Visibility.Visible : Visibility.Collapsed;
                    }
                    else
                    {
                        viewTemplate.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        private void CopyChecked()
        {
            var filters = ViewTemplates.SelectMany(t => t.Filters.Where(f => f.IsChecked)).ToList();
            foreach (var viewTemplateWrapper in ViewTemplates.Where(t => t.IsChecked))
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

                    foreach (var filter in ViewTemplates.SelectMany(v =>
                        v.Filters.Where(f => f.FilterStatus != FilterStatus.Exits)))
                    {
                        if (filter.FilterStatus == FilterStatus.New)
                        {
                            var graphicSettings =
                                filter.OriginalParentTemplate.ViewTemplate.GetFilterOverrides(filter.FilterId);
                            filter.ParentTemplate.ViewTemplate.AddFilter(filter.FilterId);
                            filter.ParentTemplate.ViewTemplate.SetFilterOverrides(filter.FilterId, graphicSettings);
                        }
                        else if (filter.FilterStatus == FilterStatus.Remove)
                        {
                            filter.ParentTemplate.ViewTemplate.RemoveFilter(filter.FilterId);
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
