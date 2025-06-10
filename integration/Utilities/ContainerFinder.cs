namespace integration.Utilities;


public class Container
{
    public int Id { get; set; }
    public int? IdCapacity { get; set; }
    public int? IdContainerType { get; set; }
}

public static class ContainerFinder
{
    private static readonly  List<Container> _containers = new List<Container>
        {
            new Container { Id = 153, IdCapacity = 45, IdContainerType = 3 },
            new Container { Id = 99, IdCapacity = null, IdContainerType = 19 },
            new Container { Id = 142, IdCapacity = 42, IdContainerType = 7 },
            new Container { Id = 103, IdCapacity = 54, IdContainerType = 7 },
            new Container { Id = 98, IdCapacity = 5, IdContainerType = 4 },
            new Container { Id = 102, IdCapacity = 2516, IdContainerType = 38 },
            new Container { Id = 111, IdCapacity = 2446, IdContainerType = 3 },
            new Container { Id = 34, IdCapacity = 4, IdContainerType = 4 },
            new Container { Id = 114, IdCapacity = 1268, IdContainerType = 3 },
            new Container { Id = 105, IdCapacity = 1273, IdContainerType = 3 },
            new Container { Id = 137, IdCapacity = 2450, IdContainerType = 3 },
            new Container { Id = 139, IdCapacity = 2455, IdContainerType = 3 },
            new Container { Id = 132, IdCapacity = 2424, IdContainerType = 3 },
            new Container { Id = 138, IdCapacity = 1270, IdContainerType = 3 },
            new Container { Id = 136, IdCapacity = 1274, IdContainerType = 3 },
            new Container { Id = 135, IdCapacity = 1267, IdContainerType = 3 },
            new Container { Id = 145, IdCapacity = 1293, IdContainerType = 2 },
            new Container { Id = 143, IdCapacity = 1255, IdContainerType = 3 },
            new Container { Id = 37, IdCapacity = 3, IdContainerType = 4 },
            new Container { Id = 134, IdCapacity = 2460, IdContainerType = 3 },
            new Container { Id = 106, IdCapacity = 1263, IdContainerType = 3 },
            new Container { Id = 2, IdCapacity = 45, IdContainerType = 3 },
            new Container { Id = 160, IdCapacity = 1184, IdContainerType = 4 },
            new Container { Id = 96, IdCapacity = 2, IdContainerType = 4 },
            new Container { Id = 148, IdCapacity = 11, IdContainerType = 3 },
            new Container { Id = 146, IdCapacity = 15, IdContainerType = 3 },
            new Container { Id = 36, IdCapacity = 2517, IdContainerType = 29 },
            new Container { Id = 108, IdCapacity = 39, IdContainerType = 3 },
            new Container { Id = 97, IdCapacity = 1, IdContainerType = 4 },
            new Container { Id = 152, IdCapacity = 21, IdContainerType = 20 },
            new Container { Id = 157, IdCapacity = 2462, IdContainerType = 3 },
            new Container { Id = 117, IdCapacity = 2520, IdContainerType = 29 },
            new Container { Id = 113, IdCapacity = 2521, IdContainerType = 29 },
            new Container { Id = 110, IdCapacity = 2534, IdContainerType = 8 },
            new Container { Id = 133, IdCapacity = 2522, IdContainerType = 29 },
            new Container { Id = 115, IdCapacity = 2523, IdContainerType = 29 },
            new Container { Id = 129, IdCapacity = 2524, IdContainerType = 29 },
            new Container { Id = 128, IdCapacity = 2525, IdContainerType = 29 },
            new Container { Id = 140, IdCapacity = 2526, IdContainerType = 29 },
            new Container { Id = 107, IdCapacity = 2449, IdContainerType = 29 },
            new Container { Id = 130, IdCapacity = 2527, IdContainerType = 29 },
            new Container { Id = 118, IdCapacity = 2528, IdContainerType = 29 },
            new Container { Id = 19, IdCapacity = 2529, IdContainerType = 29 },
            new Container { Id = 112, IdCapacity = 2530, IdContainerType = 29 },
            new Container { Id = 151, IdCapacity = 2402, IdContainerType = 29 },
            new Container { Id = 155, IdCapacity = 1255, IdContainerType = 3 },
            new Container { Id = 154, IdCapacity = null, IdContainerType = 8 },
            new Container { Id = 131, IdCapacity = 2459, IdContainerType = 4 },
            new Container { Id = 104, IdCapacity = 6, IdContainerType = 4 },
            new Container { Id = 150, IdCapacity = 1187, IdContainerType = 4 },
            new Container { Id = 165, IdCapacity = 1262, IdContainerType = 3 },
            new Container { Id = 159, IdCapacity = 1586, IdContainerType = 3 },
            new Container { Id = 109, IdCapacity = 432, IdContainerType = 8 },
            new Container { Id = 162, IdCapacity = 2453, IdContainerType = 8 },
            new Container { Id = 166, IdCapacity = 2532, IdContainerType = 29 },
            new Container { Id = 164, IdCapacity = 2533, IdContainerType = 29 },
            new Container { Id = 161, IdCapacity = 2452, IdContainerType = 8 },
            new Container { Id = 141, IdCapacity = 2531, IdContainerType = 3 },
            new Container { Id = 156, IdCapacity = 45, IdContainerType = 3 },
            new Container { Id = 101, IdCapacity = 124, IdContainerType = 19 },
            new Container { Id = 149, IdCapacity = null, IdContainerType = 19 },
            new Container { Id = 158, IdCapacity = 1288, IdContainerType = 19 },
            new Container { Id = 163, IdCapacity = null, IdContainerType = 19 },
            new Container { Id = 100, IdCapacity = null, IdContainerType = 19 },
            new Container { Id = 144, IdCapacity = 1288, IdContainerType = 19 },
            new Container { Id = 147, IdCapacity = 1, IdContainerType = 4 }
        };
    
    public static int? FindContainerId(int? idCapacity, int? idContainerType)
    {
        return _containers
            .FirstOrDefault(c => 
                c.IdCapacity == idCapacity && 
                c.IdContainerType == idContainerType)?.Id;
    }
}
