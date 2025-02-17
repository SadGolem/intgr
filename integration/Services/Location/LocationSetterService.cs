using integration.Context;
using integration.Services.Interfaces;

namespace integration.Services.Location
{
    public class LocationSetterService : ServiceBase, ISetterService<LocationData>
    {
        public bool Check(LocationData data)
        {
            throw new NotImplementedException();
        }

        public Task<List<LocationData>> PutData()
        {
            throw new NotImplementedException();
        }

        public object MappingData(LocationData data)
        {
            throw new NotImplementedException();
        }

        public void Message(string ex)
        {
            throw new NotImplementedException();
        }

        public void PostAndPatch(LocationData data, bool isNew)
        {
            throw new NotImplementedException();
        }
    }
}
