using Microsoft.EntityFrameworkCore;
using Vendsys.Models;
using Vendsys.Interfaces;
using System.Reflection.PortableExecutable;
using System;
using System.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Collections.Generic;

namespace Vendsys.Data
{
    public class DbProcedures : IDbProcedures
    {
        private readonly Connection _context;
        public DbProcedures(Connection context)
        {
            _context = context;
        }
        public List<DexMeter> insertDexMeter(DexMeter DM)
        {
            _context.DexMeter.Add(DM);
            _context.SaveChanges(); // Save changes to persist
            return _context.DexMeter.FromSql($"EXEC GetDexMeterByGuid @Guid = {DM.Guid};").ToList();             
        }       
        public async Task<ResponseCommon> bulkInsertDexLaneMeter(DexLaneMaterList list) 
        {            
            _context.DexLaneMeter.AddRange(list.DexLaneList);
            var i = _context.SaveChanges();            
            return new ResponseCommon() { Result=true};
        }
        public void deleteAllDex() 
        {
            var result =  _context.Database.ExecuteSqlInterpolated($"EXEC DeleteAllDataFromDexTables");           
        }      
    }
}
