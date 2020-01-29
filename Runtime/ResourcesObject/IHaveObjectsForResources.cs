using System.Collections.Generic;

/// <summary>
/// When you use this interface all objects returned by <see cref="GetResourcesObjectsGuids"/> will be moved in resources at build time and then back when build completes <see cref="ResourceMover"/>
/// </summary>
public interface IHaveObjectsForResources
{
    IEnumerable<string> GetResourcesObjectsGuids();
}