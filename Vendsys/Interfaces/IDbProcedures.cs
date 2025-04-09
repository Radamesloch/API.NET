using Vendsys.Models;

namespace Vendsys.Interfaces
{
    public interface IDbProcedures
    {
        List<DexMeter> insertDexMeter(DexMeter DM);
        Task<ResponseCommon> bulkInsertDexLaneMeter(DexLaneMaterList list);
        void deleteAllDex();
    }
}
