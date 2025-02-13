using integration.Context;

namespace integration.Factory
{
    public class LocationCreator : Creator
    {
        public override Data FactoryMethod()
        {
            return new LocationData();
        }
    }
}
