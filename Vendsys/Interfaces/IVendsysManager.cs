using Vendsys.Models;

namespace Vendsys.Interfaces
{
    public interface IVendsysManager
    {
        Task<ResponseCommon> ManageDexFile(string machine, string dexFile);
        Task<ResponseCommon> ManageDeleteRows();
    }
}
