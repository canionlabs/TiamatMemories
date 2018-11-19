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

        public void Service()
        {
            foreach (ResourceType resource in resources)
            {
                resource.Service();
            }
        }

        public void CleanUp()
        {
            foreach (ResourceType resource in resources)
            {
                resource.Dispose();
            }

            resources.Clear();
        }

        public ResourceType AcquireResource()
        {
            ResourceType resource = resources.Find((item) =>
            {
                return item.IsAvailable;
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
