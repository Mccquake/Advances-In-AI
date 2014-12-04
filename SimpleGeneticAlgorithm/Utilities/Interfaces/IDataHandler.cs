namespace Utilities.Interfaces
{
    using System.Collections.Generic;

    public interface IDataHandler
    {
        IDictionary<IList<string>, int> ReadBinaryData(string path);

        IDictionary<IList<double>, int> ReadRealData(string path); 

        bool TryWriteData(string data, string path);
    }
}