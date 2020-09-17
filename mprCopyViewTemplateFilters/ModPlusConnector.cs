namespace mprCopyViewTemplateFilters
{
    using System;
    using System.Collections.Generic;
    using ModPlusAPI.Interfaces;

    /// <inheritdoc />
    public class ModPlusConnector : IModPlusFunctionInterface
    {
        private static ModPlusConnector _instance;

        /// <summary>
        /// Singleton
        /// </summary>
        public static ModPlusConnector Instance => _instance ?? (_instance = new ModPlusConnector());

        /// <inheritdoc />
        public SupportedProduct SupportedProduct => SupportedProduct.Revit;

        /// <inheritdoc />
        public string Name => "mprCopyViewTemplateFilters";

#if R2017
        /// <inheritdoc />
        public string AvailProductExternalVersion => "2017";
#elif R2018
        /// <inheritdoc />
        public string AvailProductExternalVersion => "2018";
#elif R2019
        /// <inheritdoc />
        public string AvailProductExternalVersion => "2019";
#elif R2020
        /// <inheritdoc />
        public string AvailProductExternalVersion => "2020";
#elif R2021
        /// <inheritdoc />
        public string AvailProductExternalVersion => "2021";
#endif

        /// <inheritdoc />
        public string FullClassName => "mprCopyViewTemplateFilters.Command";

        /// <inheritdoc />
        public string AppFullClassName => string.Empty;

        /// <inheritdoc />
        public Guid AddInId => Guid.Empty;

        /// <inheritdoc />
        public string LName => "Фильтры видов";

        /// <inheritdoc />
        public string Description => "Управление фильтрами видов и шаблонов видов с возможностью копирования фильтров между шаблонами видов и/или видами с сохранением настроек переопределения графики";

        /// <inheritdoc />
        public string Author => "Пекшев Александр aka Modis";

        /// <inheritdoc />
        public string Price => "0";

        /// <inheritdoc />
        public bool CanAddToRibbon => true;

        /// <inheritdoc />
        public string FullDescription => string.Empty;

        /// <inheritdoc />
        public string ToolTipHelpImage => string.Empty;

        /// <inheritdoc />
        public List<string> SubFunctionsNames => new List<string>();

        /// <inheritdoc />
        public List<string> SubFunctionsLames => new List<string>();

        /// <inheritdoc />
        public List<string> SubDescriptions => new List<string>();

        /// <inheritdoc />
        public List<string> SubFullDescriptions => new List<string>();

        /// <inheritdoc />
        public List<string> SubHelpImages => new List<string>();

        /// <inheritdoc />
        public List<string> SubClassNames => new List<string>();
    }
}
