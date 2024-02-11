using System.Collections.Generic;
using DataSystem;

public interface IDataManager
{
    public List<ComposerData> GetComposers(int level);
}