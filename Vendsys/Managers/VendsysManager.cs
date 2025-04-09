using Vendsys.Data;
using Vendsys.Interfaces;
using Vendsys.Models;

namespace Vendsys.Managers
{
    public class VendsysManager : IVendsysManager
    {
       private readonly IDbProcedures _dbProcedures;
       private readonly IConfiguration _configuration;
        public VendsysManager(IConfiguration configuration,IDbProcedures dbProcedures) 
        { 
            _dbProcedures = dbProcedures;
            _configuration = configuration;
        }
        public async Task<ResponseCommon> ManageDexFile(string machine, string dexFile)
        {
            #region Variables
            var listDLM = new List<DexLaneMeter>();
            var list = new DexLaneMaterList();
            DexLaneMeter DLM = new DexLaneMeter();

            // Create a new DexMeter object and initialize it with a unique identifier (GUID).
            DexMeter DM = new DexMeter((Guid.NewGuid()).ToString());
            DM.Machine = machine;
            // Set the current UTC time as the DexMeter's datetime
            DM.DexDatetime = DateTime.UtcNow;

            ResponseCommon responseCommon = new ResponseCommon();

            // Flag used to track the state of processing a DexLaneMeter
            bool dlmFlag = false;
            #endregion

            foreach (var row in dexFile.Split("\r\n"))
            {
                // Ensure that the row is not empty
                if (row.Length > 0)
                {
                    var parts = row.Split('*');

                    // If the flag is set, process the line with the "flag" handling logic
                    if (dlmFlag)
                        dexObject("flag", ref dlmFlag, ref DM, ref DLM, ref list, parts);
                    var field = row.Substring(0, 3);

                    // Process the row based on the extracted field, updating objects accordingly
                    dexObject(field, ref dlmFlag, ref DM, ref DLM, ref list, parts);
                }
            }

            // Call the database procedure to insert the DexMeter object and get the inserted DexMeter ID
            var insertDM = _dbProcedures.insertDexMeter(DM);

            // Loop through the DexLaneList and set the IdDexMeter for each DexLaneMeter
            foreach (var dlm in list.DexLaneList)
            {
                dlm.IdDexMeter = insertDM[0].Id; // Assign the inserted DexMeter's ID to each DexLaneMeter
            }

            // Perform a bulk insert of the DexLaneMeters into the database
            responseCommon = await _dbProcedures.bulkInsertDexLaneMeter(list);

            // Return the response object with the result of the bulk insert
            return responseCommon;
        }

        public Task<ResponseCommon> ManageDeleteRows()
        {
            ResponseCommon responseCommon = new ResponseCommon();

            _dbProcedures.deleteAllDex();            

            return Task.FromResult(responseCommon);
        }

        private void dexObject(string process, ref bool dlmFlag, ref DexMeter DM, ref DexLaneMeter DLM, ref DexLaneMaterList list, string[] parts )
        {
            switch (process)
            {
                case "PA1":
                    DLM.ProductId = parts[1];
                    DLM.Price = Decimal.Parse(parts[2]);
                    break;
                case "PA2":
                    DLM.NumberVends = int.Parse(parts[1]);
                    DLM.ValuePaidSales = Decimal.Parse(parts[2]);

                    list.DexLaneList.Add(new DexLaneMeter()
                    {
                        ProductId = DLM.ProductId,
                        Price = DLM.Price,
                        NumberVends = DLM.NumberVends,
                        ValuePaidSales = DLM.ValuePaidSales                         
                    });

                    dlmFlag = true;
                    break;
                case "ID1":
                    DM.MachineSerialNumber = parts[1];
                    break;
                case "VA1":
                    DM.ValuePaidVends = parts[1];
                    break;
                case "flag":
                    DLM.ProductId = "";
                    DLM.Price = 0;
                    DLM.NumberVends = 0;
                    DLM.ValuePaidSales = 0;
                    dlmFlag = false;
                    break;
            }
        }
    }
}
