namespace mprCopyViewTemplateFilters
{
    /// <summary>
    /// Логика взаимодействия для MainWindows.xaml
    /// </summary>
    public partial class MainWindows
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindows"/> class.
        /// </summary>
        public MainWindows()
        {
            InitializeComponent();
            Title = ModPlusAPI.Language.GetFunctionLocalName(ModPlusConnector.Instance);
        }
    }
}
