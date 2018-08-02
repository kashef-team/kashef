using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kashef.Services
{
    public interface ICollectionService
    {
        void AddCollection(string collectionId);

        List<string> GetAllFacesInCollection(string collectionId);

        void RemoveFacesFromCollection(string collectionId, List<string> removedFaces);
    }
}
