using System.Collections.Generic;

namespace Memory.utils
{
    public class ResourcePool<ResourceType> where ResourceType : class, IDataResource, new()
    {
        // ========= PRIVATE MEMBERS ===================================================================================

        readonly List<ResourceType> resources;

        // ========= PRIVATE METHODS ===================================================================================

        public ResourcePool()
        {
            resources = new List<ResourceType>(50);
        }

        // ========= PUBLIC METHODS ====================================================================================

        public ResourceType AcquireResource()
        {
            ResourceType resource = resources.Find((item) =>
            {
                return item.Available();
            });

            if (resource == null)
            {
                resource = new ResourceType();
                resources.Add(resource);
            }

            return resource;
        }
    }
}
