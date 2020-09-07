namespace mprCopyViewTemplateFilters.Models
{
    /// <summary>
    /// Статус фильтра
    /// </summary>
    public enum FilterStatus
    {
        /// <summary>
        /// Существующий
        /// </summary>
        Exits = 0,

        /// <summary>
        /// Создаваемый (копируемый)
        /// </summary>
        New = 1,

        /// <summary>
        /// Удаляемый
        /// </summary>
        Remove = 2
    }
}
