namespace mprCopyViewTemplateFilters.Models
{
    /// <summary>
    /// Группа по типу вида
    /// </summary>
    public enum ViewTypeGroup
    {
        /// <summary>
        /// Все
        /// </summary>
        All = 0,

        /// <summary>
        /// 3D-виды, обходы
        /// </summary>
        ThreeDWalkthroughs = 1,

        /// <summary>
        /// Планы потолков
        /// </summary>
        CeilingPlans = 2,

        /// <summary>
        /// Фасады, разрезы, выносные элементы
        /// </summary>
        ElevationsSectionsDetailViews = 3,

        /// <summary>
        /// Планы этажей, несущих конструкций, зон
        /// </summary>
        FloorStructuralAreaPlans = 4
    }
}
