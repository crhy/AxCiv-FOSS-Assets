namespace RhyCiv.Engine.Advances
{
    public enum HistoryEventType
    {
        AdvanceDiscovered,
        CityBuilt
    }
    // Breadcrumb: if you add a new HistoryEvent here, consider also updating
    // HistoryUtils.ReconstructHistory to support 'backfilling' from legacy SAV/SCN files.
}